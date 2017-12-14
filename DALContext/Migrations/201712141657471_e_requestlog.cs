namespace DALContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class e_requestlog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.E_REQUESTLOG",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        transid = c.String(maxLength: 90, storeType: "nvarchar"),
                        card_num = c.String(maxLength: 20, storeType: "nvarchar"),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        trans_amount = c.String(maxLength: 20, storeType: "nvarchar"),
                        trans_code = c.String(maxLength: 4, storeType: "nvarchar"),
                        merchant_code = c.String(maxLength: 40, storeType: "nvarchar"),
                        response_code = c.String(maxLength: 2, storeType: "nvarchar"),
                        response_time = c.DateTime(nullable: false, precision: 0),
                        trans_descr = c.String(maxLength: 100, storeType: "nvarchar"),
                        client_id = c.String(maxLength: 20, storeType: "nvarchar"),
                        request_id = c.String(maxLength: 100, storeType: "nvarchar"),
                        fee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        currency = c.String(maxLength: 8, storeType: "nvarchar"),
                        channelid = c.String(maxLength: 2, storeType: "nvarchar"),
                        stan = c.String(maxLength: 6, storeType: "nvarchar"),
                        reversed = c.String(maxLength: 1, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateIndex("dbo.E_TMCHOST_RESP", "TRANS_DATA", name: "TMCHOSTRESP_TRANSDATA");
            CreateIndex("dbo.E_TMCREQUEST", "TRANS_DATA", name: "TMCREQUEST_TRANSDATA");
        }
        
        public override void Down()
        {
            DropIndex("dbo.E_TMCREQUEST", "TMCREQUEST_TRANSDATA");
            DropIndex("dbo.E_TMCHOST_RESP", "TMCHOSTRESP_TRANSDATA");
            DropTable("dbo.E_REQUESTLOG");
        }
    }
}
