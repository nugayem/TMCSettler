namespace DALContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class e_RequestlogAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.E_FEEBATCH",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        KEYID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.E_SETTLE_BATCH",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BATCH_ID = c.String(maxLength: 20),
                        BATCH_DATE = c.DateTime(nullable: false),
                        CLOSED = c.String(maxLength: 1),
                        START_DATE = c.DateTime(nullable: false),
                        END_DATE = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.E_SETTLEMENT_DOWNLOAD_BK",
                c => new
                    {
                        GLOBALID = c.Int(nullable: false, identity: true),
                        TRANSID = c.Int(nullable: false),
                        CARD_NUM = c.String(maxLength: 20),
                        TRANS_NO = c.String(maxLength: 30),
                        TRANS_DATE = c.DateTime(nullable: false),
                        TRANS_DESCR = c.String(maxLength: 200),
                        TRANS_AMOUNT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TRANS_TYPE = c.String(maxLength: 1),
                        TRANS_CODE = c.String(maxLength: 1),
                        MERCHANT_CODE = c.String(maxLength: 20),
                        CLOSED = c.String(maxLength: 1),
                        TRANS_REF = c.String(maxLength: 20),
                        EXTERNAL_TRANSID = c.String(maxLength: 50),
                        UNIQUE_TRANSID = c.String(maxLength: 50),
                        REP_STATUS = c.String(maxLength: 1),
                        INTSTATUS = c.String(maxLength: 1),
                        SBATCHID = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RECALC_BAL = c.String(maxLength: 1),
                        SERVICEID = c.String(maxLength: 40),
                        CHANNELID = c.String(maxLength: 2),
                        PROCESS_STATUS = c.String(maxLength: 2),
                        FEE = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CURRENCY = c.String(maxLength: 3),
                        SBATCH_NO = c.String(maxLength: 15),
                        FEE_BATCH = c.String(maxLength: 20),
                        SETTLE_BATCH = c.String(maxLength: 20),
                        CARD_NUM_ACCT = c.String(maxLength: 30),
                        MERCHANT_CODE_ACCT = c.String(maxLength: 30),
                        GFLAG = c.String(maxLength: 1),
                        SOURCE_REF = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DEST_REF = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SBATCH_NO_BK = c.String(maxLength: 15),
                        SWITCH_FEE = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BANK_FEE = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TRACK2 = c.String(maxLength: 25),
                    })
                .PrimaryKey(t => t.GLOBALID)
                .Index(t => t.UNIQUE_TRANSID, name: "ESETTLEMENT_UNIQUE_TRANSID");
            
            CreateIndex("dbo.E_CATSCALE", "CAT_ID", name: "CATSCALE_CATID");
            CreateIndex("dbo.E_FEE_DETAIL_BK", "UNIQUE_TRANSID", name: "EFEEDETAIL_UNIQUE_TRANSID");
            CreateIndex("dbo.E_MERCHANT_COMMISSION_SPLIT", "MERCHANT_CODE", name: "COMMSPLIT_MERCHANTCODE");
            CreateIndex("dbo.E_MERCHANT_SPECIAL_SPLIT", "MERCHANT_CODE", name: "SPECIALSPLIT_MERCHANTCODE");
            CreateIndex("dbo.E_TRANSACTION", "UNIQUE_TRANSID", name: "ETRANSACTION_UNIQUE_TRANSID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.E_TRANSACTION", "ETRANSACTION_UNIQUE_TRANSID");
            DropIndex("dbo.E_SETTLEMENT_DOWNLOAD_BK", "ESETTLEMENT_UNIQUE_TRANSID");
            DropIndex("dbo.E_MERCHANT_SPECIAL_SPLIT", "SPECIALSPLIT_MERCHANTCODE");
            DropIndex("dbo.E_MERCHANT_COMMISSION_SPLIT", "COMMSPLIT_MERCHANTCODE");
            DropIndex("dbo.E_FEE_DETAIL_BK", "EFEEDETAIL_UNIQUE_TRANSID");
            DropIndex("dbo.E_CATSCALE", "CATSCALE_CATID");
            DropTable("dbo.E_SETTLEMENT_DOWNLOAD_BK");
            DropTable("dbo.E_SETTLE_BATCH");
            DropTable("dbo.E_FEEBATCH");
        }
    }
}
