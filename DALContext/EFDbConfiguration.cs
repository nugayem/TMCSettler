using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALContext
{
    public class EFDbConfiguration : DbConfiguration
    {
        public EFDbConfiguration()
        {
            //this.SetProviderServices("System.Data.SqlClient",
            //            System.Data.Entity.SqlServer.SqlProviderServices.Instance);

        }
        public EFDbConfiguration(ConnectionFactory.DBType dbType)
        {
            if (dbType == ConnectionFactory.DBType.MySql)
            {

                //DbMigrationsConfiguration.SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
                //<add name="MySQL Data Provider" invariant="MySql.Data.MySqlClient" description=".Net Framework Data Provider for MySQL" type="MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data, Version=6.8.3.0" />
                string invariantname = MySql.Data.Entity.MySqlProviderInvariantName.ProviderName;//MySql.Data.MySqlClient
                this.SetDefaultConnectionFactory(new MySql.Data.Entity.MySqlConnectionFactory());
                //this.AddDependencyResolver(new SingletonDependencyResolver<IDbConnectionFactory>(new MySql.Data.Entity.MySqlConnectionFactory()));
                this.SetProviderFactory(invariantname, new MySql.Data.MySqlClient.MySqlClientFactory());
                this.SetProviderServices(invariantname, new MySql.Data.MySqlClient.MySqlProviderServices());
            }
            else if (dbType == ConnectionFactory.DBType.SQLServer)
            {
                this.SetDefaultConnectionFactory(new System.Data.Entity.Infrastructure.SqlConnectionFactory());
                this.SetProviderServices("System.Data.SqlClient",System.Data.Entity.SqlServer.SqlProviderServices.Instance);

                
                //this.AddDependencyResolver(new SingletonDependencyResolver<IDbConnectionFactory>(new System.Data.Entity.Infrastructure.SqlCeConnectionFactory(invariantname)));
                //SetProviderFactory("System.Data.SqlClient",new System.Data.SqlClient.DbProviderFactory());


            }
        }
    }
}
