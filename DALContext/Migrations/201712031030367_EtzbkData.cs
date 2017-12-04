namespace DALContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EtzbkData : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.E_TRANSACTION",
                c => new
                    {
                        GLOBALID = c.Int(nullable: false, identity: true),
                        TRANSID = c.Int(nullable: false),
                        CARD_NUM = c.String(),
                        TRANS_NO = c.String(),
                        TRANS_DATE = c.DateTime(nullable: false),
                        TRANS_DESCR = c.String(),
                        TRANS_AMOUNT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TRANS_TYPE = c.String(),
                        TRANS_CODE = c.String(),
                        MERCHANT_CODE = c.String(),
                        CLOSED = c.String(),
                        TRANS_REF = c.String(),
                        EXTERNAL_TRANSID = c.String(),
                        UNIQUE_TRANSID = c.String(),
                        SBATCHID = c.Int(nullable: false),
                        SERVICEID = c.String(),
                        FEE = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CURRENCY = c.String(),
                        SBATCH_NO = c.String(),
                        TRACK2 = c.String(),
                        TRANS_CHECK = c.String(),
                        SOURCE_IDENTIFIER = c.String(),
                    })
                .PrimaryKey(t => t.GLOBALID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.E_TRANSACTION");
        }
    }
}
