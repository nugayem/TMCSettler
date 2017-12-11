namespace DALContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeDatatype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.E_MERCHANT_COMMISSION_SPLIT", "AGENT", c => c.String(maxLength: 1));
            AddColumn("dbo.E_MERCHANT_COMMISSION_SPLIT", "PARTY_NO", c => c.String(maxLength: 2));
            AddColumn("dbo.E_MERCHANT_COMMISSION_SPLIT", "MAIN_FLAG", c => c.String(maxLength: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.E_MERCHANT_COMMISSION_SPLIT", "MAIN_FLAG");
            DropColumn("dbo.E_MERCHANT_COMMISSION_SPLIT", "PARTY_NO");
            DropColumn("dbo.E_MERCHANT_COMMISSION_SPLIT", "AGENT");
        }
    }
}
