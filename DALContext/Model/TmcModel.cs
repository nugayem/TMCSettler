using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALContext.Model
{
    public class E_TMCREQUEST
    {
        public int ID { get; set; }
        [MaxLength(4)]
        public string MTI { get; set; }
        [MaxLength(19)]
        public string PAN { get; set; }
        [MaxLength(6)]
        public string PRO_CODE { get; set; }
        [MaxLength(10)]
        public string UTCDATE { get; set; }
        [MaxLength(6)]
        public string TIME { get; set; }
        [MaxLength(4)]
        public string LOC_DATE { get; set; }
        [MaxLength(3)]
        public string POS_MODE { get; set; }
        [MaxLength(50)]
        public string TRACK2 { get; set; }
        [MaxLength(20)]
        public string TERMINAL_ID { get; set; }
        [MaxLength(3)]
        public string CURRENCY { get; set; }
        [MaxLength(12)]
        public string STAN { get; set; }
        [MaxLength(20)]
        public string AQID { get; set; }
        [MaxLength(20)]
        public string FIID { get; set; }
        [MaxLength(28)]
        public string ACCT_ID1 { get; set; }
        [MaxLength(28)]
        public string ACCT_ID2 { get; set; }
        [MaxLength(15)]
        public string CARD_ACC_ID { get; set; }
        [MaxLength(40)]
        public string CARD_ACC_NAME { get; set; }
        [MaxLength(2)]
        public string RESPONSE_CODE { get; set; }

        public DateTime? RESPONSE_TIME { get; set; }
        [MaxLength(20)]
        public string TARGET { get; set; }
        [Column(TypeName = "char")]
        [MaxLength(1)]
        public string CANCEL { get; set; }

        public decimal AMOUNT { get; set; }

        public DateTime TRANS_DATE { get; set; } = DateTime.Now;
        [Column(TypeName = "char")]
        [MaxLength(1)]
        public string SETTLED { get; set; }
        [MaxLength(20)]
        public string EX_FIID { get; set; }
        [MaxLength(42)]
        public string TRANS_KEY { get; set; }
        [MaxLength(42)]
        public string REVERSAL_KEY { get; set; }


        public int? REV_ATTEMPT { get; set; }
        [MaxLength(42)]
        public string SWITCH_KEY { get; set; }

        public decimal FEE { get; set; }
        [MaxLength(12)]
        public string REFERENCE { get; set; }

        public int SRC_NODE { get; set; }

        public int TARGET_NODE { get; set; }
        [MaxLength(70)]
        public string BILLER_ID { get; set; }
        [MaxLength(70)]
        public string BILL_ID { get; set; }
        [Column(TypeName = "char")]
        [MaxLength(1)]
        public string STATUS { get; set; }
        [Column(TypeName = "char")]
        [MaxLength(2)]
        public string ADVISE_RESPONSE { get; set; }
        [MaxLength(12)]
        public string TRANS_SEQ { get; set; }
        [MaxLength(2)]
        public string ADVISE_CODE { get; set; }
        [MaxLength(4)]
        public string MERCHANT_TYPE { get; set; }
        [MaxLength(30)]
        public string SETTLED_ID { get; set; }
        [MaxLength(32)]
        public string PMV { get; set; }
        [MaxLength(20)]
        public string SETTLED_BATCH { get; set; }
        [MaxLength(6)]
        public string CARD_SCHEME { get; set; }
        [MaxLength(60)]
        [Index("TMCREQUEST_TRANSDATA", IsClustered = false)]
        public string TRANS_DATA { get; set; }

        [NotMapped]
        public virtual E_TMCHOST_RESP E_TMCHOST_RESP { get; set; }
    }

    public class E_TMCHOST_RESP
    {

        public int ID { get; set; }
        [MaxLength(70)]
        [Index("TMCHOSTRESP_TRANSDATA", IsClustered = false)]
        public string TRANS_DATA { get; set; }
        [MaxLength(6)]
        public string APPROVAL_CODE { get; set; }
        [MaxLength(5)]
        public string RESPONSE_CODE { get; set; }
        [MaxLength(12)]
        public string STAN { get; set; }
        [MaxLength(16)]
        public string TERMINAL_ID { get; set; }
        [MaxLength(12)]
        public string PROC_CODE { get; set; }
        [MaxLength(100)]
        public string DESCRIPTION { get; set; }
        [MaxLength(30)]
        public string PAN { get; set; }
        [MaxLength(15)]
        public string REFERENCE { get; set; }
        [MaxLength(100)]
        public string FEE { get; set; }
        [MaxLength(20)]
        public string SOURCE_ACCT { get; set; }
        [MaxLength(20)]
        public string DEST_ACCT { get; set; }

        public DateTime TRANS_DATE { get; set; }
        [MaxLength(20)]
        public string SWITCH_REF { get; set; }

    }

    public class E_TMCNODE
    {
        public int ID { get; set; }
        public int INCON_ID { get; set; }

        [MaxLength(20)]
        public string INCON_NAME { get; set; }

        [MaxLength(20)]
        public string IPADDRESS { get; set; }

        public int KEY_ID { get; set; }
        [MaxLength(3)]
        public string CURRENCY { get; set; }

        [Column(TypeName = "char")]
        [MaxLength(1)]
        public string ALLOW_PIN { get; set; }
        [Column(TypeName = "char")]
        [MaxLength(1)]
        public string STATUS { get; set; }
        [MaxLength(11)]
        public string FIID { get; set; }
        [Column(TypeName = "char")]
        [MaxLength(1)]
        public string ALLOW_FINANCIAL { get; set; }

        public DateTime LAST_CONNECTED { get; set; }
        [MaxLength(3)]
        public string AQISSUER_CODE { get; set; }
        [MaxLength(11)]
        public string AQID { get; set; }
        [Column(TypeName = "char")]
        [MaxLength(1)]
        public string CONN_TYPE { get; set; }

        public int REMOTE_PORT { get; set; }

        [Column(TypeName = "char")]
        [MaxLength(1)]
        public string ACTIVE { get; set; }
        public decimal INT_FEE { get; set; }
        public DateTime LAST_KEEPALIVE { get; set; } =  DateTime.Now;
        public int KEEP_ALIVE { get; set; }
        public int DEBUG { get; set; }
        [Column(TypeName = "char")]
        [MaxLength(1)]
        public string SIGN_ON { get; set; }
        [Column(TypeName = "char")]
        [MaxLength(1)]
        public string SEND_KEY { get; set; }
        [Column(TypeName = "char")]
        [MaxLength(1)]
        public string HOST_TYPE { get; set; }
        [Column(TypeName = "char")]
        [MaxLength(1)]
        public string ROUTE_BY { get; set; }
        [Column(TypeName = "char")]
        [MaxLength(1)]
        public string NOTIFY { get; set; }

        [Column(TypeName = "char")]
        [MaxLength(1)]
        public string X_STATUS { get; set; }
        [Column(TypeName = "char")]
        [MaxLength(1)]
        public string ENFORCE_IP { get; set; }

        [Column(TypeName = "char")]
        [MaxLength(1)]
        public string MOBILE_CMS_AUTH { get; set; }


        public int KEY_SYNC_TIME { get; set; }
        public int KEY_ID1 { get; set; }

        [MaxLength(3)]
        public string ISSUER_CODE { get; set; }


    }

    public class E_REQUESTLOG
    {
        public int ID { get; set; }
        [MaxLength(90)]
        public string transid { get; set; }
        [MaxLength(20)]
        public string card_num { get; set; }
        
        public DateTime trans_date { get; set; }
        [MaxLength(20)]
        public string trans_amount { get; set; }
        [MaxLength(4)]
        public string trans_code { get; set; }
        [MaxLength(40)]
        public string merchant_code { get; set; }
        [MaxLength(2)]
        public string response_code { get; set; }
       
        public DateTime response_time { get; set; }
        [MaxLength(100)]
        public string trans_descr { get; set; }
        [MaxLength(20)]
        public string client_id { get; set; }
        [MaxLength(100)]
        public string request_id { get; set; }

        public decimal fee { get; set; }
        [MaxLength(8)]
        public string currency { get; set; }
        [MaxLength(2)]
        public string channelid { get; set; }
        [MaxLength(6)]
        public string stan { get; set; }
        [MaxLength(1)]
        public string reversed { get; set; }

    }


}

