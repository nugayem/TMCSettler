namespace DALContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class spltCardDDependeTablevvvv : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.E_MERCHANT", "PIN_MISSED", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.E_MERCHANT", "PIN_MISSED", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
