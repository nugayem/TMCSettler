namespace DALContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modelChanged : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.E_CARDLOAD_COMMISSION_SPLIT", "MAIN_FLAG", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.E_CARDLOAD_COMMISSION_SPLIT", "MAIN_FLAG", c => c.Int(nullable: false));
        }
    }
}
