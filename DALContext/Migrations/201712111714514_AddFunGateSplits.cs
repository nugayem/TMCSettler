namespace DALContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFunGateSplits : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.E_FUNDGATE_COMMISSION_SPLIT",
                c => new
                    {
                        KEYID = c.Int(nullable: false, identity: true),
                        CARD_NUM = c.String(maxLength: 20),
                        SPLIT_CARD = c.String(maxLength: 16),
                        SPLIT_DESCR = c.String(maxLength: 30),
                        RATIO = c.Int(nullable: false),
                        PARTY_NO = c.String(maxLength: 2),
                    })
                .PrimaryKey(t => t.KEYID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.E_FUNDGATE_COMMISSION_SPLIT");
        }
    }
}
