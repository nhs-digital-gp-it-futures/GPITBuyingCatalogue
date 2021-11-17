using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration
{
    public class Migrator
    {
        private readonly string BAPIConnectionString;
        private readonly string ISAPIConnectionString;
        private readonly string ORDAPIConnectionString;
        private readonly string GPITBuyingCatalogueConnectionString;

        public Migrator()
        {
            BAPIConnectionString = GetConnectionString("BAPI");
            ISAPIConnectionString = GetConnectionString("ISAPI");
            ORDAPIConnectionString = GetConnectionString("ORDAPI");
            GPITBuyingCatalogueConnectionString = GetConnectionString("GPITBuyingCatalogue");
        }

        public void RunMigration()
        {
            LoadLegacyUsers();

            LoadCurrentUsers();
        }

        private void LoadLegacyUsers()
        {
            using (var sqlConnection = new SqlConnection(ISAPIConnectionString))
            {
                sqlConnection.Open();
                
                string sql = $"select * from AspNetUsers";

                var entities = sqlConnection.Query<LegacyModels.AspNetUser>(sql).ToList();

              //  var entities = sqlConnection.Query(sql).ToList();

                System.Diagnostics.Trace.WriteLine($"Loaded {entities.Count} users from legacy database");
            }
        }

        private void LoadCurrentUsers()
        {
            var entities = CurrentDbContext.AspNetUsers.ToList();

            System.Diagnostics.Trace.WriteLine($"Loaded {entities.Count} users from current database");
        }

        private EntityFramework.BuyingCatalogueDbContext CurrentDbContext
        {
            get
            {
                var options = new DbContextOptionsBuilder<EntityFramework.BuyingCatalogueDbContext>()
                    .UseSqlServer(GPITBuyingCatalogueConnectionString)
                    .EnableSensitiveDataLogging()
                    .Options;

                return new EntityFramework.BuyingCatalogueDbContext(options, new IdentityServiceStub());
            }
        }


        private string GetConnectionString(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException($"Failed to get connection string config for {connectionStringName}");

            return connectionString;
        }
    }
}
