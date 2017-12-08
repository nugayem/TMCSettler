using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DALContext.Services;

namespace DALContext.Model
{
    public class EtzBKViewModel : IHaveCustomMappings
    {

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<E_TMCREQUEST, E_TRANSACTION>(MemberList.None)
                .ForMember(o => o.CARD_NUM, opt => opt.MapFrom(d => d.PAN))
                .ForMember(o => o.CHANNELID, opt => opt.MapFrom(d => d.PAN.Substring(0, 3)))
                .ForMember(o => o.CLOSED, opt => opt.Ignore())
                .ForMember(o => o.CURRENCY, opt => opt.MapFrom(d => d.CURRENCY))
                .ForMember(o => o.EXTERNAL_TRANSID, opt => opt.MapFrom(d => d.TRANS_DATA.Substring(3, 23)))
                .ForMember(o => o.FEE_BATCH, opt => opt.Ignore())
                .ForMember(o => o.INTSTATUS, opt => opt.Ignore())
                .ForMember(o => o.MERCHANT_CODE, opt => opt.MapFrom(d => d.CARD_ACC_ID))
                .ForMember(o => o.PROCESS_STATUS, opt => opt.Ignore())
                .ForMember(o => o.RECALC_BAL, opt => opt.Ignore())
                .ForMember(o => o.REP_STATUS, opt => opt.Ignore())
                .ForMember(o => o.SBATCH_NO, opt => opt.Ignore())
                .ForMember(o => o.SERVICEID, opt => opt.Ignore())
                .ForMember(o => o.SETTLE_BATCH, opt => opt.Ignore())
                .ForMember(o => o.TRACK2, opt => opt.MapFrom(d => d.PAN))
                .ForMember(o => o.TRANSID, opt => opt.MapFrom(d => d.E_TMCHOST_RESP.REFERENCE))
                .ForMember(o => o.TRANS_AMOUNT, opt => opt.MapFrom(d => d.AMOUNT))
                .ForMember(o => o.TRANS_CHECK, opt => opt.MapFrom(d => d.BILL_ID.Substring(0, d.BILL_ID.Length - 1)))
                .ForMember(o => o.TRANS_CODE, opt => opt.ResolveUsing(d => GetTransCode(d.PRO_CODE)))
                .ForMember(o => o.TRANS_DATE, opt => opt.MapFrom(d => d.TRANS_DATE))
                .ForMember(o => o.TRANS_DESCR, opt => opt.MapFrom(d => d.CARD_ACC_NAME))
                .ForMember(o => o.TRANS_NO, opt => opt.MapFrom(d => d.BILLER_ID.Substring(d.BILLER_ID.IndexOf('#'))))
                .ForMember(o => o.TRANS_REF, opt => opt.Ignore())
                .ForMember(o => o.TRANS_TYPE, opt => opt.MapFrom(d => "1"))
                .ForMember(o => o.UNIQUE_TRANSID, opt => opt.MapFrom(d => d.TRANS_DATA));

           // select "insert into e_transaction (TRANS_CHECK,TRANSID, CARD_NUM, TRANS_NO, TRANS_DATE, TRANS_DESCR, TRANS_AMOUNT,  TRANS_TYPE, TRANS_CODE, MERCHANT_CODE, TRANS_REF, EXTERNAL_TRANSID, UNIQUE_TRANSID, SERVICEID, INTSTATUS, CHANNELID, FEE, CURRENCY) VALUES ('" + BILLER_ID + "'," + CONVERT(VARCHAR, TRANS_SEQ) + ",'" + PAN + "','" + substring(BILL_ID, CHARINDEX('#', BILL_ID) + 1, 15) + "','" + convert(varchar, GETDATE()) + "' ,'" + CARD_ACC_NAME + "', " + CONVERT(VARCHAR, AMOUNT) + ",'1','P','" + REVERSE(substring(reverse(BILL_ID), 1, charindex('#', reverse(BILL_ID)) - 1)) + "','','" + SUBSTRING(TRANS_DATA, 3, 23) + "','" + TRANS_DATA + "','" + REVERSE(substring(reverse(BILL_ID), 1, charindex('#', reverse(BILL_ID)) - 1)) + "','9','05', 0,'566')" FROM e_tmcrequest  WHERE TRANS_DATA IN('0508214161761459498390715040129705762731467233')

        }

        private char GetTransCode(string d)
        {
            
            char value = '\0';
            switch (d)
            {
                case "402020":
                    value = 'P';
                    break;
                case "422099":
                    value = 'T';
                    break;
                case "132000":
                    value = 'P';
                    break;
                case "552000":
                    value = 'D';
                    break;
                case "421099":
                    value = 'T';
                    break;
            }
            return value;
        }
    }

    public class EtzCardTranx : IHaveCustomMappings
    {
        public string TRANS_CODE { get; set; }
        public string CARD_NUM { get; set; }
        public string TRANSID { get; set; }
        public string MERCHANT_CODE { get; set; }
        public string TRANS_DESCR { get; set; }
        public string RESPONSE_CODE { get; set; }
        public decimal TRANS_AMOUNT { get; set; }
        public DateTime TRANS_DATE { get; set; }
        public string EXTERNAL_TRANSID { get; set; }
        public decimal FEE { get; set; }
        public string REVERSAL_KEY { get; set; }
        public string TRANS_NO { get; set; }
        public string TERMINAL_ID { get; set; }
        public string CARD_SCHEME { get; set; }
        public string REFERENCE { get; set; }
        public string RESP_RESPONSE_CODE { get; set; }


        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<EtzCardTranx, E_TRANSACTION>(MemberList.None)
                .ForMember(o => o.CARD_NUM, opt => opt.MapFrom(d => d.CARD_NUM));


        }
    }
}
