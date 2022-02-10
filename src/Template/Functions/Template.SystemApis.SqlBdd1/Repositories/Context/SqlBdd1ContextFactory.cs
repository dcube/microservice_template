using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Template.SystemApis.SqlBdd1.Repositories.Context
{
    public class SqlBdd1ContextFactory : IDesignTimeDbContextFactory<SqlBdd1Context>
    {
        public SqlBdd1Context CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlBdd1Context>();
            optionsBuilder.UseSqlServer("Data Source=server;Initial Catalog=Bdd;User ID=user;  Password=pwd", opts => opts.MigrationsHistoryTable("__EFMigrationsHistory", "migr"));

            return new SqlBdd1Context(optionsBuilder.Options);
        }
    }
}
