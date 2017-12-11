namespace DALContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class spltCardDDependeTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.E_CATSCALE",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CAT_ID = c.String(maxLength: 3),
                        SCALE_ID = c.String(maxLength: 3),
                        SCALE_FROM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SCALE_TO = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SCALE_TYPE = c.String(maxLength: 1),
                        SCALE_VALUE = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ADDED_FEE = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.E_MERCHANT",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MERCHANT_CODE = c.String(maxLength: 10),
                        MERCHANT_ACCT = c.String(maxLength: 20),
                        MERCHANT_PIN = c.String(maxLength: 20),
                        MERCHANT_NAME = c.String(maxLength: 40),
                        FIRSTNAME = c.String(maxLength: 25),
                        LASTNAME = c.String(maxLength: 25),
                        STREET = c.String(maxLength: 40),
                        CITY = c.String(maxLength: 25),
                        STATE = c.String(maxLength: 25),
                        ZIP = c.String(maxLength: 10),
                        COUNTRY = c.String(maxLength: 40),
                        PHONE = c.String(maxLength: 30),
                        FAX = c.String(maxLength: 30),
                        EMAIL = c.String(maxLength: 80),
                        ABOUTUS = c.String(maxLength: 255),
                        HINTQUESTION = c.String(maxLength: 40),
                        HINTANSWER = c.String(maxLength: 40),
                        URL = c.String(maxLength: 60),
                        LOGO = c.String(maxLength: 100),
                        CHARGE_TYPE = c.String(maxLength: 1),
                        CHARGES = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CAT_ID = c.String(maxLength: 3),
                        ISSUER_CODE = c.String(maxLength: 3),
                        SUB_CODE = c.String(maxLength: 3),
                        CHANGE_PIN = c.String(maxLength: 1),
                        USER_LOCKED = c.String(maxLength: 1),
                        PIN_MISSED = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LAST_USED = c.DateTime(nullable: false),
                        MODIFIED = c.DateTime(nullable: false),
                        CREATED = c.DateTime(nullable: false),
                        ONLINE_DATE = c.DateTime(nullable: false),
                        ONLINE_BALANCE = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OFFLINE_DATE = c.DateTime(nullable: false),
                        OFFLINE_BALANCE = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FEE_STATUS = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FEE_RATIO = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EXTRA_FEE = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SPECIAL_SPLIT = c.String(maxLength: 1),
                        MERCHANT_ALIAS = c.String(maxLength: 100),
                        BASE_CURRENCY = c.String(maxLength: 3),
                        PROCESS_MODE = c.String(maxLength: 1),
                        SETTLEMENT_FREQ = c.String(maxLength: 1),
                        MERCHANT_ACCT1 = c.String(maxLength: 20),
                        MERCHANT_ID = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.E_MERCHANT_COMMISSION_SPLIT",
                c => new
                    {
                        KEYID = c.Int(nullable: false, identity: true),
                        MERCHANT_CODE = c.String(maxLength: 20),
                        SPLIT_CARD = c.String(maxLength: 20),
                        RATIO = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SPLIT_DESCR = c.String(maxLength: 30),
                        MIN_CHARGE = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MAX_CHARGE = c.Decimal(nullable: false, precision: 18, scale: 2),
                        COMM_SUSPENCE = c.String(maxLength: 20),
                        CREATED = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.KEYID);
            
            CreateTable(
                "dbo.E_MERCHANT_SPECIAL_SPLIT",
                c => new
                    {
                        KEYID = c.Int(nullable: false, identity: true),
                        MERCHANT_CODE = c.String(maxLength: 20),
                        SPLIT_CARD = c.String(maxLength: 20),
                        SVALUE = c.Int(nullable: false),
                        SPLIT_DESCR = c.String(maxLength: 50),
                        TRANS_CODE = c.String(maxLength: 1),
                        MAIN_FLAG = c.String(maxLength: 1),
                        PARTY_NO = c.String(maxLength: 2),
                        CREATED = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.KEYID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.E_MERCHANT_SPECIAL_SPLIT");
            DropTable("dbo.E_MERCHANT_COMMISSION_SPLIT");
            DropTable("dbo.E_MERCHANT");
            DropTable("dbo.E_CATSCALE");
        }
    }
}
