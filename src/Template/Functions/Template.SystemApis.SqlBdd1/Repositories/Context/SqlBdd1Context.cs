using Microsoft.EntityFrameworkCore;
using Template.Sqlbdd1.Dto;

namespace Template.SystemApis.SqlBdd1.Repositories.Context
{
    public class SqlBdd1Context : DbContext
    {
        public SqlBdd1Context(DbContextOptions<SqlBdd1Context> options)
        {
        }

        public virtual DbSet<OrderDto> Order { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SqlBdd1Context).Assembly);
        }
    }
}
