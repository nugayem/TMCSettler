﻿using AutoMapper;
using DALContext.Model;
using MySql.Data.MySqlClient;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace DALContext
{
    //[DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    [DbConfigurationType(typeof(EFDbConfiguration))]
    public class TmcDataContext : DbContext
    {

        public TmcDataContext() : base("TMC145")
        {

            //SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
            DbConfiguration.SetConfiguration(new EFDbConfiguration(ConnectionFactory.DBType.MySql));
            
            this.Configuration.LazyLoadingEnabled = false;
            var objcontext = (this as IObjectContextAdapter).ObjectContext;
            objcontext.CommandTimeout = 300;
        }

        public TmcDataContext(string connectionString) : base(connectionString)
        {
            Database.Connection.ConnectionString = connectionString;

        }
        

        public DbSet<E_TMCREQUEST> E_TMCREQUEST { get; set; }
        public DbSet<E_TMCHOST_RESP> E_TMCHOST_RESP { get; set; }
        public DbSet<E_TMCNODE> E_TMCNODE { get; set; }
        public DbSet<E_REQUESTLOG> E_REQUESTLOG { get; set; }

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
           //DbConfiguration.SetConfiguration(new EFDbConfiguration(ConnectionFactory.DBType.SQLServer));
            // Database.SetInitializer(new MigrateDatabaseToLatestVersion<SistemaContext, Sistema.DataAccess.Migrations.Configuration>());

            DbConfiguration.SetConfiguration(new EFDbConfiguration(ConnectionFactory.DBType.MySql));

            this.Configuration.LazyLoadingEnabled = false;
            var objcontext = (this as IObjectContextAdapter).ObjectContext;
            objcontext.CommandTimeout = 300;

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
        public DbSet<E_SETTLEMENT_DOWNLOAD_BK> E_SETTLEMENT_DOWNLOAD_BK { get; set; }
        public DbSet<E_SETTLE_BATCH> E_SETTLE_BATCH { get; set; }
        public DbSet<E_FEEBATCH> E_FEEBATCH { get; set; }
        public DbSet<E_SWITCHIT_TRANSFORMER> E_SWITCHIT_TRANSFORMER { get; set; }
        public DbSet<E_MERCHANT_CODE_INTERCEPT> E_MERCHANT_CODE_INTERCEPT { get; set; }        
        public DbSet<E_FUNDGATE_COMMISSION_SPLIT> E_FUNDGATE_COMMISSION_SPLIT { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<E_TRANSACTION>().Property(x => x.TRANS_AMOUNT).HasPrecision(18, 2);
            modelBuilder.Entity<E_TRANSACTION>().Property(x => x.FEE).HasPrecision(18, 2);
            modelBuilder.Entity<E_FUNDGATE_COMMISSION_SPLIT>().Property(x => x.RATIO).HasPrecision(18, 2);
        }
    }


    public class FundGateDataContext : DbContext
    {

        public FundGateDataContext() : base("FundGate")
        {
            //DbConfiguration.SetConfiguration(new EFDbConfiguration(ConnectionFactory.DBType.SQLServer));
            // Database.SetInitializer(new MigrateDatabaseToLatestVersion<SistemaContext, Sistema.DataAccess.Migrations.Configuration>());

            DbConfiguration.SetConfiguration(new EFDbConfiguration(ConnectionFactory.DBType.MySql));

            this.Configuration.LazyLoadingEnabled = false;
            var objcontext = (this as IObjectContextAdapter).ObjectContext;
            objcontext.CommandTimeout = 300;

        }

        public DbSet<FUNDGATE_REQUEST> FUNDGATE_REQUEST { get; set; }
        public DbSet<FUNDGATE_RESPONSE> FUNDGATE_RESPONSE { get; set; }

    }

    public class ConnectionFactory
    {
        public enum DBType
        {
            MySql, SQLServer, Sybase

        }
    }
    

}
