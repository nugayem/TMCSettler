using AutoMapper;
using DALContext.Model;
using MySql.Data.MySqlClient;
using System.Data.Entity;


namespace DALContext
{
    //[DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    [DbConfigurationType(typeof(EFDbConfiguration))]
    public class TmcDataContext : DbContext
    {

        public TmcDataContext() : base("TMC145")
        {
            DbConfiguration.SetConfiguration(new EFDbConfiguration(ConnectionFactory.DBType.MySql));
        }

        public TmcDataContext(string connectionString) : base(connectionString)
        {
            Database.Connection.ConnectionString = connectionString;
        }


        public DbSet<E_TMCREQUEST> E_TMCREQUEST { get; set; }
        public DbSet<E_TMCHOST_RESP> E_TMCHOST_RESP { get; set; }
        public DbSet<E_TMCNODE> E_TMCNODE { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<E_TMCREQUEST>().Property(x => x.AMOUNT).HasPrecision(18, 2);
            modelBuilder.Entity<E_TMCREQUEST>().Property(x => x.FEE).HasPrecision(18, 2);
        }

    }

    
    
    public class EtzbkDataContext : DbContext
    {

        public EtzbkDataContext() : base("ETZBK130")
        {
            DbConfiguration.SetConfiguration(new EFDbConfiguration(ConnectionFactory.DBType.SQLServer));
           // Database.SetInitializer(new MigrateDatabaseToLatestVersion<SistemaContext, Sistema.DataAccess.Migrations.Configuration>());
        }

        public EtzbkDataContext(string connectionString) : base(connectionString)
        {
            Database.Connection.ConnectionString = connectionString;
        }

        public DbSet<E_FEE_DETAIL_BK> E_FEE_DETAIL_BK { get; set; }
        public DbSet<E_TRANSACTION> E_TRANSACTION { get; set; }
        public DbSet<E_CARDLOAD_COMMISSION_SPLIT> E_CARDLOAD_COMMISSION_SPLIT { get; set; }
        public DbSet<E_CATSCALE> E_CATSCALE { get; set; }
        public DbSet<E_MERCHANT> E_MERCHANT { get; set; }
        public DbSet<E_MERCHANT_COMMISSION_SPLIT> E_MERCHANT_COMMISSION_SPLIT { get; set; }
        public DbSet<E_MERCHANT_SPECIAL_SPLIT> E_MERCHANT_SPECIAL_SPLIT { get; set; }

        public DbSet<E_FUNDGATE_COMMISSION_SPLIT> E_FUNDGATE_COMMISSION_SPLIT { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<E_TRANSACTION>().Property(x => x.TRANS_AMOUNT).HasPrecision(18, 2);
            modelBuilder.Entity<E_TRANSACTION>().Property(x => x.FEE).HasPrecision(18, 2);
            modelBuilder.Entity<E_FUNDGATE_COMMISSION_SPLIT>().Property(x => x.RATIO).HasPrecision(18, 2);
        }
    }

    public class ConnectionFactory
    {
        public enum DBType
        {
            MySql, SQLServer, Sybase

        }
    }

}
