namespace DALContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeFeeStatusToInt : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.E_MERCHANT", "FEE_STATUS", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.E_MERCHANT", "FEE_STATUS", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
