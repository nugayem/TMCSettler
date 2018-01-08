namespace DALContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fundGate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FUNDGATE_REQUEST",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        action = c.String(maxLength: 25, storeType: "nvarchar"),
                        terminal = c.String(maxLength: 25, storeType: "nvarchar"),
                        amount = c.String(maxLength: 25, storeType: "nvarchar"),
                        destination = c.String(maxLength: 25, storeType: "nvarchar"),
                        clientRef = c.String(maxLength: 50, storeType: "nvarchar"),
                        endPoint = c.String(maxLength: 25, storeType: "nvarchar"),
                        lineType = c.String(maxLength: 25, storeType: "nvarchar"),
                        ipAddress = c.String(maxLength: 50, storeType: "nvarchar"),
                        created = c.DateTime(nullable: false, precision: 0),
                        sender = c.String(maxLength: 50, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.FUNDGATE_RESPONSE",
                c => new
                    {
                        respId = c.Int(nullable: false, identity: true),
                        action = c.String(maxLength: 50, storeType: "nvarchar"),
                        terminal = c.String(maxLength: 50, storeType: "nvarchar"),
                        etzRef = c.String(maxLength: 50, storeType: "nvarchar"),
                        respmessage = c.String(unicode: false),
                        clientRef = c.String(maxLength: 50, storeType: "nvarchar"),
                        created = c.DateTime(nullable: false, precision: 0),
                        respCode = c.String(maxLength: 5, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.respId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FUNDGATE_RESPONSE");
            DropTable("dbo.FUNDGATE_REQUEST");
        }
    }
}
