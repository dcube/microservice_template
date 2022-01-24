using System.Collections.Generic;
using System.Threading.Tasks;

namespace Template.SystemApis.SqlBdd1.Interfaces
{
    public interface IOrderRepository : IRepository
    {
        Task<IEnumerable<CommandeAchatDto>> SaveCommandeAchatList(List<CommandeAchatDto> commandeList);

        Task<int> UpdateEnteteCommandeAchat(EnteteCommandeAchat entetCommandeAchat);

        Task<List<LigneCommandeAchat>> UpdateLignesCommandeAchat(List<LigneCommandeAchat> lignesCommandeAchatInput);

        Task SaveCommandeAchatStatus(string numeroCommande, string statut);

        Task UpdateEnteteCommandeAchatLienBonDeCommande(string numeroCommande, string lienBonDeCommande);

        Task<IEnumerable<RapprochementCommandeAchat>> InsertRapprochementCommandeAchat(List<RapprochementCommandeAchat> rapprochements);
    }
}
