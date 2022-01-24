using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Template.SystemApis.SqlBdd1.Repositories
{
    public class OrderRepository : ICommandeAchatRepository
    {
        private readonly IMapper _mapper;
        private readonly OrderContext _context;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(ILogger<OrderRepository> logger, OrderContext context, IMapper mapper) 
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<CommandeAchatDto>> SaveCommandeAchatList(List<CommandeAchatDto> commandeList)
        {
            using (var transaction = await StartTransactionAsync())
            {
                try
                {
                    foreach (var commandeAchatDto in commandeList)
                    {
                        await InsertEntete(commandeAchatDto.Entete);
                        await InsertLines(commandeAchatDto.Entete, commandeAchatDto.Lignes);
                        await UpdateDemandeListStatus(StatutDemandeAchat.BdcGenere.Code, commandeAchatDto.Lignes.Select(l => l.DemandeAchatId));
                    }
                    await transaction.CommitAsync();
                    return commandeList;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task SaveCommandeAchatStatus(string numeroCommande, string statut)
        {
            EnteteCommandeAchat commandeToUpdate = context.EntetesCommandeAchat
                .Include(entete => entete.Items)
                .FirstOrDefault(commande => commande.NoCommande == numeroCommande);

            if (commandeToUpdate == null)
            {
                throw new ApplicationException($"Commande {numeroCommande} non trouvée");
            }

            var statutFound = await context
                .StatutsCommandeAchat
                .FirstOrDefaultAsync(s => s.Code == statut);

            if (statutFound != null)
            {
                commandeToUpdate.StatutId = statutFound.Id;
                commandeToUpdate.DateStatut = DateTime.Now;

                UpdateCommandeLinesStatus(commandeToUpdate, statutFound.Id);

                context.EntetesCommandeAchat.Update(commandeToUpdate);

                await context.SaveChangesAsync();
            }
        }

        private async Task<string> GetNewCommandeNo()
        {
            var beginValue = DateTime.Now.ToString("yyMM");

            var maxEntete = await context
                .EntetesCommandeAchat
                .Where(l => l.NoCommande.StartsWith(beginValue))
                .OrderByDescending(l => l.NoCommande)
                .FirstOrDefaultAsync();

            if (maxEntete is null)
            {
                return DateTime.Now.ToString("yyMM000001");
            }
            else
            {
                return (long.Parse(maxEntete.NoCommande) + 1).ToString();
            }
        }

        #region Insert

        private async Task<EnteteCommandeAchat> InsertEntete(EnteteCommandeAchat entete)
        {
            entete.NoCommande = await GetNewCommandeNo();
            await context.EntetesCommandeAchat.AddAsync(entete);
            await context.SaveChangesAsync();
            return entete;
        }

        private async Task<IEnumerable<LigneCommandeAchat>> InsertLines(EnteteCommandeAchat entete, IEnumerable<LigneCommandeAchat> lines)
        {
            var ligneCommandeAchats = lines.ToList();

            foreach (var ligneCommandeAchat in ligneCommandeAchats)
            {
                ligneCommandeAchat.EnteteCommandeId = entete.Id;
            }
            await context.LignesCommandeAchat.AddRangeAsync(ligneCommandeAchats);
            await context.SaveChangesAsync();
            return ligneCommandeAchats;
        }

        #endregion

        #region Update
        private void UpdateCommandeLinesStatus(EnteteCommandeAchat commandeAchat, int statutId)
        {
            foreach (var ligneCommandeAchat in commandeAchat.Items)
            {
                ligneCommandeAchat.StatutId = statutId;
                ligneCommandeAchat.DateStatut = DateTime.Now;
            }
        }

        private async Task UpdateDemandeListStatus(string statutDemande, IEnumerable<int> demandeAchatIds)
        {
            var statutFound = await context.StatutsDemandeAchat
                .Where(s => s.Code == statutDemande)
                .FirstOrDefaultAsync();

            if (statutFound != null)
            {
                var listDemandeAchat = await context.LignesDemandeAchat
                    .Where(l => demandeAchatIds.Contains(l.Id))
                    .ToListAsync();

                listDemandeAchat.ForEach(d => context
                    .Entry(d)
                    .Property(p => p.StatutId)
                    .CurrentValue = statutFound.Id);

                context.LignesDemandeAchat.UpdateRange(listDemandeAchat);
                await context.SaveChangesAsync();
            }
        }

        public async Task<int> UpdateEnteteCommandeAchat(EnteteCommandeAchat enteteCommandeAchat)
        {
            using var transaction = await context.StartTransactionAsync();

            try
            {
                var enteteCommandeAchatEntity = await context.EntetesCommandeAchat
                    .AsNoTracking()
                    .FirstOrDefaultAsync(entete => entete.Id == enteteCommandeAchat.Id);

                enteteCommandeAchat.ProxAfficheId = enteteCommandeAchatEntity?.ProxAfficheId;
                enteteCommandeAchat.CodeTypeCommande = enteteCommandeAchatEntity?.CodeTypeCommande;
                enteteCommandeAchat.CodePrestationTechnique = enteteCommandeAchatEntity?.CodePrestationTechnique;

                ManageAnnexeAchat(enteteCommandeAchat.Annexe, enteteCommandeAchat.UserModification);

                context.Update(enteteCommandeAchat);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return enteteCommandeAchat.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new EntityUpdateException("Erreur lors de la mise à jour des données dans la table EnteteCommandeAchat", ex);
            }
        }

        public async Task UpdateEnteteCommandeAchatLienBonDeCommande(string numeroCommande, string lienBonDeCommande)
        {
            using var transaction = await context.StartTransactionAsync();

            try
            {
                var commande = await context.EntetesCommandeAchat
                    .Select(commande => new EnteteCommandeAchat()
                    {
                        Id = commande.Id,
                        NoCommande = commande.NoCommande
                    })
                    .FirstAsync(entete => entete.NoCommande == numeroCommande);

                context
                    .Attach(commande)
                    .Property(p => p.LienBonDeCommande)
                    .CurrentValue = lienBonDeCommande;

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new EntityUpdateException($"Erreur lors de la mise à jour du lien BDC dans la table EnteteCommandeAchat, numéro: {numeroCommande}", ex);
            }
        }

        public async Task<List<LigneCommandeAchat>> UpdateLignesCommandeAchat(List<LigneCommandeAchat> lignesCommandeAchat)
        {
            using var transaction = await context.StartTransactionAsync();

            try
            {
                var result = new List<LigneCommandeAchat>();

                if (lignesCommandeAchat.Any())
                {
                    foreach (var line in lignesCommandeAchat)
                    {
                        ManageAnnexeAchat(line.Annexe, line.UserModification);
                    }

                    result.AddRange(lignesCommandeAchat);

                    context.LignesCommandeAchat.UpdateRange(lignesCommandeAchat);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new EntityUpdateException("Erreur lors de la mise à jour des données dans la table LigneCommandeAchat", ex);
            }
        }

        public async Task<IEnumerable<RapprochementCommandeAchat>> InsertRapprochementCommandeAchat(List<RapprochementCommandeAchat> rapprochements)
        {
            using (var transaction = await StartTransactionAsync())
            {
                try
                {
                    await context.RapprochementsCommandeAchat.AddRangeAsync(rapprochements);
                    await context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return rapprochements;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
        #endregion

        #region Private methods

        private void ManageAnnexeAchat(AnnexeAchat annexe, string userLogin)
        {
            if (annexe is null)
                return;

            if (annexe.Id > 0)
            {
                annexe.UserModification = userLogin;
                annexe.DateModification = DateTime.Now;
            }
            else
            {
                annexe.UserCreation = userLogin;
                annexe.DateCreation = DateTime.Now;
                annexe.IsActif = true;
            }
        }

        #endregion
    }
}
