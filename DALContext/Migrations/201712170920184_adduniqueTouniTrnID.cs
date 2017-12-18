namespace DALContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adduniqueTouniTrnID : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.E_SETTLEMENT_DOWNLOAD_BK", "ESETTLEMENT_UNIQUE_TRANSID");
            DropIndex("dbo.E_TRANSACTION", "ETRANSACTION_UNIQUE_TRANSID");
            CreateIndex("dbo.E_SETTLEMENT_DOWNLOAD_BK", "UNIQUE_TRANSID", unique: true, name: "ESETTLEMENT_UNIQUE_TRANSID");
            CreateIndex("dbo.E_TRANSACTION", "UNIQUE_TRANSID", unique: true, name: "ETRANSACTION_UNIQUE_TRANSID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.E_TRANSACTION", "ETRANSACTION_UNIQUE_TRANSID");
            DropIndex("dbo.E_SETTLEMENT_DOWNLOAD_BK", "ESETTLEMENT_UNIQUE_TRANSID");
            CreateIndex("dbo.E_TRANSACTION", "UNIQUE_TRANSID", name: "ETRANSACTION_UNIQUE_TRANSID");
            CreateIndex("dbo.E_SETTLEMENT_DOWNLOAD_BK", "UNIQUE_TRANSID", name: "ESETTLEMENT_UNIQUE_TRANSID");
        }
    }
}
