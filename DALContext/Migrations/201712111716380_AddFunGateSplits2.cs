namespace DALContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFunGateSplits2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.E_FUNDGATE_COMMISSION_SPLIT", "RATIO", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.E_FUNDGATE_COMMISSION_SPLIT", "RATIO", c => c.Int(nullable: false));
        }
    }
}
