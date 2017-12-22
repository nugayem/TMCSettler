namespace DALContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateEtranzaction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.E_TRANSACTION", "SBATCHID", c => c.Int(nullable: false));
            AddColumn("dbo.E_TRANSACTION", "SOURCE_IDENTIFIER", c => c.String(maxLength: 50));
            AlterColumn("dbo.E_TRANSACTION", "TRANS_CHECK", c => c.String(maxLength: 40));
            DropColumn("dbo.E_TRANSACTION", "CARD_NUM_ACCT");
            DropColumn("dbo.E_TRANSACTION", "MERCHANT_CODE_ACCT");
            DropColumn("dbo.E_TRANSACTION", "GFLAG");
            DropColumn("dbo.E_TRANSACTION", "SBATCH_NO_BK");
        }
        
        public override void Down()
        {
            AddColumn("dbo.E_TRANSACTION", "SBATCH_NO_BK", c => c.String(maxLength: 15));
            AddColumn("dbo.E_TRANSACTION", "GFLAG", c => c.String(maxLength: 1));
            AddColumn("dbo.E_TRANSACTION", "MERCHANT_CODE_ACCT", c => c.String(maxLength: 30));
            AddColumn("dbo.E_TRANSACTION", "CARD_NUM_ACCT", c => c.String(maxLength: 30));
            AlterColumn("dbo.E_TRANSACTION", "TRANS_CHECK", c => c.String(maxLength: 25));
            DropColumn("dbo.E_TRANSACTION", "SOURCE_IDENTIFIER");
            DropColumn("dbo.E_TRANSACTION", "SBATCHID");
        }
    }
}
