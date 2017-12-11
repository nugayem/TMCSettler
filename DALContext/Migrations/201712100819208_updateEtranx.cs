namespace DALContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateEtranx : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.E_MERCHANT", "ONLINE_BALANCE", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.E_MERCHANT", "ONLINE_BALANCE", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
