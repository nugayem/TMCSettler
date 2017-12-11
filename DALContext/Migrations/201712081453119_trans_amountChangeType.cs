namespace DALContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class trans_amountChangeType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.E_FEE_DETAIL_BK",
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
                        GFLAG = c.String(maxLength: 1),
                        ISSUER_CODE = c.String(maxLength: 3),
                        SUB_CODE = c.String(maxLength: 3),
                        ORIGL_SETTLE_BATCH = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.GLOBALID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.E_FEE_DETAIL_BK");
        }
    }
}
