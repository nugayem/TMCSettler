using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALContext.Model
{
    public class E_TRANSACTION
    {
        [Key]
        public int GLOBALID { get; set; }
        public int TRANSID { get; set; }
        public string CARD_NUM { get; set; }
        public string TRANS_NO { get; set; }
        public DateTime TRANS_DATE { get; set; }
        public string TRANS_DESCR { get; set; }
        public decimal TRANS_AMOUNT { get; set; }
        public string TRANS_TYPE { get; set; }
        public string TRANS_CODE { get; set; }
        public string MERCHANT_CODE { get; set; }
        public string CLOSED { get; set; }
        public string TRANS_REF { get; set; }
        public string EXTERNAL_TRANSID { get; set; }
        public string UNIQUE_TRANSID { get; set; }
        public char REP_STATUS { get; set; }
        public char INTSTATUS { get; set; }
        public int SBATCHID { get; set; }
        public char RECALC_BAL { get; set; }
        public string SERVICEID { get; set; }
        public char CHANNELID { get; set; }
        public char PROCESS_STATUS { get; set; }
        public decimal FEE { get; set; }
        public string CURRENCY { get; set; }
        public string SBATCH_NO { get; set; }
        public string FEE_BATCH { get; set; }
        public char SETTLE_BATCH { get; set; }
        public string TRACK2 { get; set; }
        public string TRANS_CHECK { get; set; }
        public string SOURCE_IDENTIFIER { get; set; }

    }

    public class E_SETTLEMENT_DOWNLOAD_BK
    {
        [Key]
        public int GLOBALID { get; set; }
        [MaxLength(17)]
        public int TRANSID { get; set; }
        [MaxLength(20)]
        public string CARD_NUM { get; set; }
        [MaxLength(30)]
        public string TRANS_NO { get; set; }
        
        public DateTime TRANS_DATE { get; set; }
        [MaxLength(200)]
        public string TRANS_DESCR { get; set; }
        
        public decimal TRANS_AMOUNT { get; set; }
        [MaxLength(1)]
        public string TRANS_TYPE { get; set; }
        [MaxLength(1)]
        public string TRANS_CODE { get; set; }
        [MaxLength(20)]
        public string MERCHANT_CODE { get; set; }
        [MaxLength(1)]
        [Column(TypeName = "char")]
        public string CLOSED { get; set; }
        [MaxLength(20)]
        public string TRANS_REF { get; set; }
        [MaxLength(50)]
        public string EXTERNAL_TRANSID { get; set; }
        [MaxLength(50)]
        public string UNIQUE_TRANSID { get; set; }
        [MaxLength(1)]
        [Column(TypeName = "char")]
        public char REP_STATUS { get; set; }
        [MaxLength(1)]
        [Column(TypeName = "char")]
        public char INTSTATUS { get; set; }
        [MaxLength(6)]
        public decimal SBATCHID { get; set; }
        [MaxLength(1)]
        [Column(TypeName = "char")]
        public char RECALC_BAL { get; set; }
        [MaxLength(40)]
        public string SERVICEID { get; set; }
        [MaxLength(2)]
        [Column(TypeName = "char")]
        public char CHANNELID { get; set; }
        [MaxLength(2)]
        [Column(TypeName = "char")]
        public char PROCESS_STATUS { get; set; }
        [MaxLength(9)]
        public decimal FEE { get; set; }
        [MaxLength(3)]
        public string CURRENCY { get; set; }
        [MaxLength(15)]
        public string SBATCH_NO { get; set; }
        [MaxLength(20)]
        public string FEE_BATCH { get; set; }
        [MaxLength(20)]
        public char SETTLE_BATCH { get; set; }
        [MaxLength(30)]
        public char CARD_NUM_ACCT { get; set; }
        [MaxLength(30)]
        public char MERCHANT_CODE_ACCT { get; set; }
        [MaxLength(1)]
        [Column(TypeName = "char")]
        public char GFLAG { get; set; }
        [MaxLength(17)]
        public decimal SOURCE_REF { get; set; }
        [MaxLength(17)]
        public decimal DEST_REF { get; set; }
        [MaxLength(15)]
        public string SBATCH_NO_BK { get; set; }
        [MaxLength(9)]
        public decimal SWITCH_FEE { get; set; }
        [MaxLength(9)]
        public decimal BANK_FEE { get; set; }
        [MaxLength(25)]
        public string TRACK2 { get; set; }

    }

    public class E_FEE_DETAIL_BK
    {
        [Key]
        public int GLOBALID { get; set; }
        public int TRANSID { get; set; }
        [MaxLength(20)]
        public string CARD_NUM { get; set; }
        [MaxLength(30)]
        public string TRANS_NO { get; set; }
        public DateTime TRANS_DATE { get; set; }
        [MaxLength(200)]
        public string TRANS_DESCR { get; set; }
        [MaxLength(9)]
        public decimal TRANS_AMOUNT { get; set; }
        [MaxLength(1)]
        public string TRANS_TYPE { get; set; }
        [MaxLength(1)]
        public string TRANS_CODE { get; set; }
        [MaxLength(20)]
        public string MERCHANT_CODE { get; set; }
        [MaxLength(1)]
        public string CLOSED { get; set; }
        [MaxLength(20)]
        public string TRANS_REF { get; set; }
        [MaxLength(50)]
        public string EXTERNAL_TRANSID { get; set; }
        [MaxLength(50)]
        public string UNIQUE_TRANSID { get; set; }
        [MaxLength(1)]
        [Column(TypeName = "char")]
        public char REP_STATUS { get; set; }
        [MaxLength(1)]
        [Column(TypeName = "char")]
        public char INTSTATUS { get; set; }
        public decimal SBATCHID { get; set; }
        [MaxLength(1)]
        [Column(TypeName = "char")]
        public char RECALC_BAL { get; set; }
        [MaxLength(40)]
        public string SERVICEID { get; set; }
        [MaxLength(2)]
        [Column(TypeName = "char")]
        public char CHANNELID { get; set; }
        [MaxLength(2)]
        [Column(TypeName = "char")]
        public char PROCESS_STATUS { get; set; }
        public decimal FEE { get; set; }
        [MaxLength(3)]
        public string CURRENCY { get; set; }
        [MaxLength(15)]
        public string SBATCH_NO { get; set; }
        [MaxLength(20)]
        public string FEE_BATCH { get; set; }
        [MaxLength(20)]
        [Column(TypeName = "char")]
        public char SETTLE_BATCH { get; set; }
        [MaxLength(1)]
        [Column(TypeName = "char")]
        public char GFLAG { get; set; }
        [MaxLength(3)]
        public string ISSUER_CODE { get; set; }
        [MaxLength(3)]
        public string SUB_CODE { get; set; }
        [MaxLength(20)]
        public string ORIGL_SETTLE_BATCH { get; set; }

    }

    public class E_CARDLOAD_COMMISSION_SPLIT
    {
        [Key]
        public int KEYID { get; set; }
        [MaxLength(6)]
        public string BANK_CODE { get; set; }
        [MaxLength(10)]
        public string SPLIT_CARD { get; set; }
        public decimal RATIO { get; set; }

        [Column(TypeName = "char")]
        [MaxLength(1)]
        public string AGENT { get; set; }

        [MaxLength(50)]
        public string SPLIT_DESCR { get; set; }

        [MaxLength(10)]
        public string COMM_SUSPENCE { get; set; }

        [Column(TypeName = "char")]
        [MaxLength(1)]
        public int MAIN_FLAG { get; set; }
        public DateTime CREATED { get; set; } = DateTime.Now;


    }
}
