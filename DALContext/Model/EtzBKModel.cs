using DALContext.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace DALContext.Model
{
    public class E_TRANSACTION
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
        public decimal TRANS_AMOUNT { get; set; }
        [StringLength(1)]
        public string TRANS_TYPE { get; set; }
        [StringLength(1)]
        public string TRANS_CODE { get; set; }
        [MaxLength(20)]
        public string MERCHANT_CODE { get; set; }
        [StringLength(1)]
        public string CLOSED { get; set; }
        [MaxLength(20)]
        public string TRANS_REF { get; set; }
        [MaxLength(50)]
        public string EXTERNAL_TRANSID { get; set; }
        [MaxLength(50)]
        [Index("ETRANSACTION_UNIQUE_TRANSID", IsUnique =true, IsClustered = false)]
        public string UNIQUE_TRANSID { get; set; }
        [StringLength(1)]
        public string REP_STATUS { get; set; }
        [StringLength(1)]
        public string INTSTATUS { get; set; }
        public int SBATCHID { get; set; }
        [StringLength(1)]
        public string RECALC_BAL { get; set; }
        [MaxLength(40)]
        public string SERVICEID { get; set; }
        [StringLength(2)]
        public string CHANNELID { get; set; }
        [StringLength(2)]
        public string PROCESS_STATUS { get; set; }
        public decimal FEE { get; set; }
        [MaxLength(3)]
        public string CURRENCY { get; set; }
        [MaxLength(15)]
        public string SBATCH_NO { get; set; }
        [MaxLength(20)]
        public string FEE_BATCH { get; set; }
        [MaxLength(20)]
        public string SETTLE_BATCH { get; set; }
        [MaxLength(25)]
        public string TRACK2 { get; set; }
        [MaxLength(40)]
        public string TRANS_CHECK { get; set; }
        [MaxLength(50)]
        public string SOURCE_IDENTIFIER { get; set; }

    }



    public class E_SETTLEMENT_DOWNLOAD_BK:  IHaveCustomMappings
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

        public decimal TRANS_AMOUNT { get; set; }
        [StringLength(1)]
        public string TRANS_TYPE { get; set; }
        [StringLength(1)]
        public string TRANS_CODE { get; set; }
        [MaxLength(20)]
        public string MERCHANT_CODE { get; set; }
        [StringLength(1)]
        public string CLOSED { get; set; }
        [MaxLength(20)]
        public string TRANS_REF { get; set; }
        [MaxLength(50)]
        public string EXTERNAL_TRANSID { get; set; }
        [MaxLength(50)]
        [Index("ESETTLEMENT_UNIQUE_TRANSID", IsUnique = true, IsClustered = false)]
        public string UNIQUE_TRANSID { get; set; }
        [StringLength(1)]
        public string REP_STATUS { get; set; }
        [StringLength(1)]
        public string INTSTATUS { get; set; }
        public decimal SBATCHID { get; set; }
        [StringLength(1)]
        public string RECALC_BAL { get; set; }
        [MaxLength(40)]
        public string SERVICEID { get; set; }
        [StringLength(2)]
        public string CHANNELID { get; set; }
        [StringLength(2)]
        public string PROCESS_STATUS { get; set; }
        public decimal FEE { get; set; }
        [MaxLength(3)]
        public string CURRENCY { get; set; }
        [MaxLength(15)]
        public string SBATCH_NO { get; set; }
        [MaxLength(20)]
        public string FEE_BATCH { get; set; }
        [MaxLength(20)]
        public string SETTLE_BATCH { get; set; }
        [MaxLength(30)]
        public string CARD_NUM_ACCT { get; set; }
        [MaxLength(30)]
        public string MERCHANT_CODE_ACCT { get; set; }
        [StringLength(1)]

        public string GFLAG { get; set; }
        public decimal SOURCE_REF { get; set; }
        public decimal DEST_REF { get; set; }
        [MaxLength(15)]
        public string SBATCH_NO_BK { get; set; }
        public decimal SWITCH_FEE { get; set; }
        public decimal BANK_FEE { get; set; }
        [MaxLength(25)]
        public string TRACK2 { get; set; }// trans_check and source identifier

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<E_TRANSACTION, E_SETTLEMENT_DOWNLOAD_BK>(MemberList.None);
        }
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
        [Index("EFEEDETAIL_UNIQUE_TRANSID", IsClustered = false)]
        public string UNIQUE_TRANSID { get; set; }
        [StringLength(1)]
        public string REP_STATUS { get; set; }
        [StringLength(1)]
        public string INTSTATUS { get; set; }
        public decimal SBATCHID { get; set; }
        [StringLength(1)]
        public string RECALC_BAL { get; set; }
        [MaxLength(40)]
        public string SERVICEID { get; set; }
        [StringLength(2)]

        public string CHANNELID { get; set; }
        [StringLength(2)]

        public string PROCESS_STATUS { get; set; }
        public decimal FEE { get; set; }
        [MaxLength(3)]
        public string CURRENCY { get; set; }
        [MaxLength(15)]
        public string SBATCH_NO { get; set; }
        [MaxLength(20)]
        public string FEE_BATCH { get; set; }
        [MaxLength(20)]

        public string SETTLE_BATCH { get; set; }
        [StringLength(1)]

        public string GFLAG { get; set; }
        [MaxLength(3)]
        public string ISSUER_CODE { get; set; }
        [MaxLength(3)]
        public string SUB_CODE { get; set; }
        [MaxLength(20)]
        public string ORIGL_SETTLE_BATCH { get; set; }

    }



    public class E_CARDLOAD_COMMISSION_SPLIT : IMapFrom<CommissionMapViewModel>
    {
        [Key]
        public int KEYID { get; set; }
        [MaxLength(6)]
        public string BANK_CODE { get; set; }
        [MaxLength(10)]
        public string SPLIT_CARD { get; set; }
        public decimal RATIO { get; set; }

        [MaxLength(1)]
        public string AGENT { get; set; }

        [MaxLength(50)]
        public string SPLIT_DESCR { get; set; }

        [MaxLength(10)]
        public string COMM_SUSPENCE { get; set; }
        public string MAIN_FLAG { get; set; }
        public DateTime CREATED { get; set; } = DateTime.Now;

    }

    public class E_TRANSFER_COMMISSION_SPLIT : IMapFrom<CommissionMapViewModel>
    {
        [Key]
        public int KEYID { get; set; }
        [MaxLength(6)]
        public string BANK_CODE { get; set; }
        [MaxLength(10)]
        public string SPLIT_CARD { get; set; }
        public decimal RATIO { get; set; }

        [MaxLength(1)]
        public string AGENT { get; set; }

        [MaxLength(50)]
        public string SPLIT_DESCR { get; set; }

        [MaxLength(10)]
        public string COMM_SUSPENCE { get; set; }
        public string MAIN_FLAG { get; set; }
        public DateTime CREATED { get; set; } = DateTime.Now;

    }

    public class E_SETTLE_BATCH
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(20)]
        public string BATCH_ID { get; set; }
        public DateTime BATCH_DATE { get; set; }
        [MaxLength(1)]
        public string CLOSED { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
    }
    public class E_MERCHANT
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(10)]
        public string MERCHANT_CODE { get; set; }
        [MaxLength(20)]
        public string MERCHANT_ACCT { get; set; }
        [MaxLength(20)]
        public string MERCHANT_PIN { get; set; }
        [MaxLength(40)]
        public string MERCHANT_NAME { get; set; }
        [MaxLength(25)]
        public string FIRSTNAME { get; set; }
        [MaxLength(25)]
        public string LASTNAME { get; set; }
        [MaxLength(40)]
        public string STREET { get; set; }
        [MaxLength(25)]
        public string CITY { get; set; }
        [MaxLength(25)]
        public string STATE { get; set; }
        [MaxLength(10)]
        public string ZIP { get; set; }
        [MaxLength(40)]
        public string COUNTRY { get; set; }
        [MaxLength(30)]
        public string PHONE { get; set; }
        [MaxLength(30)]
        public string FAX { get; set; }
        [MaxLength(80)]
        public string EMAIL { get; set; }
        [MaxLength(255)]
        public string ABOUTUS { get; set; }
        [MaxLength(40)]
        public string HINTQUESTION { get; set; }
        [MaxLength(40)]
        public string HINTANSWER { get; set; }
        [MaxLength(60)]
        public string URL { get; set; }
        [MaxLength(100)]
        public string LOGO { get; set; }
        [MaxLength(1)]
        public string CHARGE_TYPE { get; set; } 
        public decimal CHARGES { get; set; }
        [MaxLength(3)]
        public string CAT_ID { get; set; }
        [MaxLength(3)]
        public string ISSUER_CODE { get; set; }
        [MaxLength(3)]
        public string SUB_CODE { get; set; }
        [MaxLength(1)]
        public string CHANGE_PIN { get; set; }
        [MaxLength(1)]
        public string USER_LOCKED { get; set; }
        public  decimal? PIN_MISSED { get; set; }
        public DateTime LAST_USED { get; set; }
        public DateTime MODIFIED { get; set; }
        public DateTime CREATED { get; set; }
        public DateTime ONLINE_DATE { get; set; }
        public decimal? ONLINE_BALANCE { get; set; }
        public DateTime OFFLINE_DATE { get; set; }
        public decimal OFFLINE_BALANCE { get; set; }
        public int FEE_STATUS { get; set; }
        public decimal FEE_RATIO { get; set; }
        public decimal EXTRA_FEE { get; set; }
        [MaxLength(1)]
        public string SPECIAL_SPLIT { get; set; }
        [MaxLength(100)]
        public string MERCHANT_ALIAS { get; set; }
        [MaxLength(3)]
        public string BASE_CURRENCY { get; set; }
        [MaxLength(1)]
        public string PROCESS_MODE { get; set; }
        [MaxLength(1)]
        public string SETTLEMENT_FREQ { get; set; }
        [MaxLength(20)]
        public string MERCHANT_ACCT1 { get; set; }
        [MaxLength(10)]
        public string MERCHANT_ID { get; set; }

    }

    public class E_CATSCALE
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(3)]
        [Index("CATSCALE_CATID", IsClustered = false)]
        public string CAT_ID { get; set; }
        [MaxLength(3)]
        public string SCALE_ID { get; set; } 
        public decimal SCALE_FROM { get; set; } 
        public decimal SCALE_TO { get; set; }
        [MaxLength(1)]
        public string SCALE_TYPE { get; set; } 
        public decimal SCALE_VALUE { get; set; } 
        public decimal ADDED_FEE { get; set; }
    }

    public class E_MERCHANT_SPECIAL_SPLIT
    {

        [Key]
        public int KEYID { get; set; }
        [MaxLength(20)]
        [Index("SPECIALSPLIT_MERCHANTCODE", IsClustered = false)]
        public string MERCHANT_CODE { get; set; }
        [MaxLength(20)]
        public string SPLIT_CARD { get; set; }
        public int SVALUE { get; set; }
        [MaxLength(50)]
        public string SPLIT_DESCR { get; set; }
        [MaxLength(1)]
        public string TRANS_CODE { get; set; }
        [MaxLength(1)]
        public string MAIN_FLAG { get; set; }
        [MaxLength(2)]
        public string PARTY_NO { get; set; } 
        public DateTime CREATED { get; set; } = DateTime.Now;

    }

    public class E_MERCHANT_COMMISSION_SPLIT
    {
        [Key]
        public int KEYID { get; set; }
        [MaxLength(20)]
        [Index("COMMSPLIT_MERCHANTCODE", IsClustered = false)]
        public string MERCHANT_CODE { get; set; }
        [MaxLength(20)]
        public string SPLIT_CARD { get; set; }
        public decimal RATIO { get; set; }
        [MaxLength(1)]
        public string AGENT { get; set; }
        [MaxLength(30)]
        public string SPLIT_DESCR { get; set; }
        public decimal MIN_CHARGE { get; set; }
        public decimal MAX_CHARGE { get; set; }
        [MaxLength(20)]
        public string COMM_SUSPENCE { get; set; }
        [MaxLength(2)]
        public string PARTY_NO { get; set; }
        [MaxLength(1)]
        public string MAIN_FLAG { get; set; }

        public DateTime CREATED { get; set; } = DateTime.Now;
    }

    public class E_FUNDGATE_COMMISSION_SPLIT : IHaveCustomMappings
    {
        [Key]
        public int KEYID { get; set; }
        [MaxLength(20)]
        public string CARD_NUM { get; set; }
        [MaxLength(16)]
        public string SPLIT_CARD { get; set; }
        [MaxLength(30)]
        public string SPLIT_DESCR { get; set; }
        public decimal RATIO { get; set; }
        [MaxLength(2)]
        public string PARTY_NO { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<E_FUNDGATE_COMMISSION_SPLIT, CommissionMapViewModel>(MemberList.None)
                .ForMember(o => o.COMM_SUSPENCE, opt => opt.MapFrom(d => d.CARD_NUM))
                .ForMember(o => o.SPLIT_CARD, opt => opt.MapFrom(d => d.SPLIT_CARD))
                .ForMember(o => o.SPLIT_DESCR, opt => opt.MapFrom(d => d.SPLIT_DESCR))
                .ForMember(o => o.RATIO, opt => opt.MapFrom(d => d.RATIO)).ReverseMap();
        }
    }
    
    public class E_FEEBATCH
    {
        public int ID { get; set; }
        public int KEYID { get; set; }
    }

}
