using DALContext;
using DALContext.Model;
using LoggerHelper.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmcOperationService
{
    public static class Settlement
    {
       private static readonly Logger logger = new Logger();

        public static ResponseViewModel ProcessPaymentSettlement(E_TRANSACTION item)
        {
            ResponseViewModel response = new ResponseViewModel();            


            using (EtzbkDataContext etzbk = new EtzbkDataContext())
            {
                try
                {
                    var merchantIntercept = etzbk.E_MERCHANT_CODE_INTERCEPT.Where(a => a.INITIATOR_CODE == item.CARD_NUM.Substring(0, 3) && a.MERCHANT_CODE == item.MERCHANT_CODE && a.CHANNELID==item.CHANNELID && a.TRANS_CODE==item.TRANS_CODE&&a.INTERCEPT_STATUS=="1").FirstOrDefault();

                    if (merchantIntercept != null)
                    {
                        if (merchantIntercept.INTERCEPT_MERCHANT_CODE != null || merchantIntercept.INTERCEPT_MERCHANT_CODE.Trim() != "")
                            item.MERCHANT_CODE = merchantIntercept.INTERCEPT_MERCHANT_CODE;
                        if (merchantIntercept.CARD_NUM != null || merchantIntercept.CARD_NUM.Trim() != "")
                            item.CARD_NUM = merchantIntercept.CARD_NUM;
                    }
                    string merchantCode = item.MERCHANT_CODE;
                    List<E_FEE_DETAIL_BK> feeDetailList = new List<E_FEE_DETAIL_BK>();

                    var merchantScaleQuery = from A in etzbk.E_MERCHANT
                                             join B in etzbk.E_CATSCALE
                                             on new { X = A.CAT_ID } equals new { X = B.CAT_ID } into jointData
                                             from joinRecord in jointData.DefaultIfEmpty()
                                             where (A.MERCHANT_CODE == merchantCode)
                                             select new
                                             {
                                                 A.FEE_STATUS,
                                                 A.SPECIAL_SPLIT,
                                                 A.MERCHANT_NAME,
                                                 joinRecord.SCALE_VALUE,
                                                 joinRecord.SCALE_TYPE,
                                                 joinRecord.CAT_ID
                                             };

                    var merchantScale = merchantScaleQuery.FirstOrDefault();
                    if (merchantScale == null)
                    {
                        response = new ResponseViewModel() { ErrorType = ErrorType.NoMerchantCode, MerchantCode = merchantCode, Response = false };
                        return response;
                        
                        ///Write Code to handle No Merchant Code or Split category configured
                    }
                    if (merchantScale.SPECIAL_SPLIT == "0")
                    {
                        // Check If Fee is Charged if not, ignore and comparee value
                        if (merchantScale.SCALE_TYPE == "1" & item.FEE == 0)
                            item.FEE = FeeProcessing.CalculateFeeBeneficiary(merchantScale.SCALE_VALUE, item.TRANS_AMOUNT);
                        var query = from A in etzbk.E_MERCHANT_COMMISSION_SPLIT
                                    where (A.MERCHANT_CODE == item.MERCHANT_CODE)
                                    select new CommissionMapViewModel
                                    {
                                        AGENT = "",
                                        MAIN_FLAG = A.MAIN_FLAG,
                                        SPLIT_CARD = A.SPLIT_CARD,
                                        RATIO = A.RATIO,
                                        SPLIT_DESCR = A.SPLIT_DESCR,
                                        COMM_SUSPENCE = item.MERCHANT_CODE

                                    };
                        List<CommissionMapViewModel> commission = AutoMapper.Mapper.Map<List<CommissionMapViewModel>>(query.ToList());

                        var e_settlement = AutoMapper.Mapper.Map<E_SETTLEMENT_DOWNLOAD_BK>(item);

                        feeDetailList = FeeProcessing.ProcessRatioPaymentSplit(item, commission);

                        var e_Fee_detail = RemoveSettledFeeDetails(feeDetailList);
                        etzbk.E_FEE_DETAIL_BK.AddRange(e_Fee_detail);
                        
                        decimal bankFee = feeDetailList.Where(fee => fee.MERCHANT_CODE.EndsWith("9999")).Select(FEE => FEE.TRANS_AMOUNT).FirstOrDefault();
                        e_settlement.BANK_FEE = bankFee;
                        if(!CheckSettledFee(e_settlement))
                            etzbk.E_SETTLEMENT_DOWNLOAD_BK.Add(e_settlement);



                    }
                    else if (merchantScale.SPECIAL_SPLIT == "1")
                    {
                        var query = from A in etzbk.E_MERCHANT_SPECIAL_SPLIT
                                    where (A.MERCHANT_CODE == item.MERCHANT_CODE)
                                    select new CommissionMapViewModel
                                    {
                                        AGENT = "",
                                        MAIN_FLAG = A.MAIN_FLAG,
                                        SPLIT_CARD = A.SPLIT_CARD,
                                        RATIO = A.SVALUE,
                                        SPLIT_DESCR = A.SPLIT_DESCR,
                                        COMM_SUSPENCE = item.MERCHANT_CODE

                                    };

                        List<CommissionMapViewModel> commission = AutoMapper.Mapper.Map<List<CommissionMapViewModel>>(query.ToList());

                        E_SETTLEMENT_DOWNLOAD_BK e_settlement = AutoMapper.Mapper.Map<E_SETTLEMENT_DOWNLOAD_BK>(item);

                        feeDetailList = FeeProcessing.ProcessRatioPaymentSplit(item, commission);

                        var e_Fee_detail = RemoveSettledFeeDetails(feeDetailList);
                        etzbk.E_FEE_DETAIL_BK.AddRange(e_Fee_detail);

                        decimal bankFee = feeDetailList.Where(fee => fee.MERCHANT_CODE.EndsWith("9999")).Select(FEE => FEE.FEE).FirstOrDefault();
                        e_settlement.BANK_FEE = bankFee;

                        if(!CheckSettledFee(e_settlement))
                            etzbk.E_SETTLEMENT_DOWNLOAD_BK.Add(e_settlement);

                    }
                    else
                    {                        
                        response = new ResponseViewModel() { ErrorType= ErrorType.InvalidSplitType, MerchantCode = merchantCode,  Response = false };
                        return response;
                    }

                    item.PROCESS_STATUS = "1";
                    etzbk.E_TRANSACTION.Add(item);
                    etzbk.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    

                    etzbk.SaveChanges();
                   // Console.WriteLine(nameof(PaymentProducer) + " Final round saved to database ");
                    return new ResponseViewModel() { ErrorType=ErrorType.Valid, MerchantCode = merchantCode, Response = true };
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Exception from " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + nameof(Settlement) + " " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogInfoMessage("Exception from Run " + nameof(Settlement) + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));
                    return new ResponseViewModel() { ErrorType=ErrorType.Exception, MerchantCode= item.UNIQUE_TRANSID , Message= ex.Message, Response = false };
                }


            }

        }

        internal static void ProcessCardLoadSettlement(E_TRANSACTION item)
        {
            List<E_CARDLOAD_COMMISSION_SPLIT> splitFormular = CachingProvider.GetCachedData<List<E_CARDLOAD_COMMISSION_SPLIT>>("CardLoad");
            List<CommissionMapViewModel> commission = AutoMapper.Mapper.Map<List<CommissionMapViewModel>>(splitFormular);

            using (EtzbkDataContext etzbk = new EtzbkDataContext())
            {

                try
                {
                    List<E_FEE_DETAIL_BK> feeDetailList = new List<E_FEE_DETAIL_BK>();
                    feeDetailList = FeeProcessing.ProcessCardloadSplit(item, commission);


                    var e_Fee_detail = RemoveSettledFeeDetails(feeDetailList);
                    etzbk.E_FEE_DETAIL_BK.AddRange(e_Fee_detail);

                    E_SETTLEMENT_DOWNLOAD_BK e_settlement = AutoMapper.Mapper.Map<E_SETTLEMENT_DOWNLOAD_BK>(item);
                    decimal bankFee = feeDetailList.Where(fee => fee.MERCHANT_CODE.EndsWith("9999")).Select(FEE => FEE.FEE).FirstOrDefault();
                    e_settlement.BANK_FEE = bankFee;

                    if (!CheckSettledFee(e_settlement))
                        etzbk.E_SETTLEMENT_DOWNLOAD_BK.Add(e_settlement);

                    item.PROCESS_STATUS = "1";
                    etzbk.E_TRANSACTION.Add(item);
                    etzbk.Entry(item).State = System.Data.Entity.EntityState.Modified;

                    etzbk.SaveChanges();
                    logger.LogInfoMessage(System.Reflection.MethodBase.GetCurrentMethod().Name + " round saved to database ");

                }
                catch (Exception ex)
                {

                    Console.WriteLine("Exception from " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + nameof(Settlement) + " " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogInfoMessage("Exception from Run " + nameof(Settlement) + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));
                }

            }
        }

        internal static void ProcessTransferSettlement(E_TRANSACTION item)
        {
            List<E_TRANSFER_COMMISSION_SPLIT> splitFormular = CachingProvider.GetCachedData<List<E_TRANSFER_COMMISSION_SPLIT>>("Transfer");
            List<E_FUNDGATE_COMMISSION_SPLIT> fundGatesplitFormular = CachingProvider.GetCachedData<List<E_FUNDGATE_COMMISSION_SPLIT>>("FundGate");

            List<CommissionMapViewModel> commission = AutoMapper.Mapper.Map<List<CommissionMapViewModel>>(splitFormular);
            List<CommissionMapViewModel> fundGatecommission = AutoMapper.Mapper.Map<List<CommissionMapViewModel>>(fundGatesplitFormular);

            using (EtzbkDataContext etzbk = new EtzbkDataContext())
            {
                try
                {
                    List<E_FEE_DETAIL_BK> feeDetailList = new List<E_FEE_DETAIL_BK>();
                    if (item.CHANNELID == "09")
                        feeDetailList = FeeProcessing.ProcessCardloadSplit(item, fundGatecommission);
                    else
                        feeDetailList = FeeProcessing.ProcessCardloadSplit(item, commission);

                    var e_Fee_detail = RemoveSettledFeeDetails(feeDetailList);
                    etzbk.E_FEE_DETAIL_BK.AddRange(e_Fee_detail);

                    E_SETTLEMENT_DOWNLOAD_BK e_settlement = AutoMapper.Mapper.Map<E_SETTLEMENT_DOWNLOAD_BK>(item);
                    decimal bankFee = feeDetailList.Where(fee => fee.MERCHANT_CODE.EndsWith("9999")).Select(FEE => FEE.FEE).FirstOrDefault();
                    e_settlement.BANK_FEE = bankFee;

                    if (!CheckSettledFee(e_settlement))
                        etzbk.E_SETTLEMENT_DOWNLOAD_BK.Add(e_settlement);

                    item.PROCESS_STATUS = "1";
                    etzbk.E_TRANSACTION.Add(item);
                    etzbk.Entry(item).State = System.Data.Entity.EntityState.Modified;

                    etzbk.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception from " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + nameof(Settlement) + " " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogInfoMessage("Exception from Run " + nameof(Settlement) + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));
                }
            }
        }


        public static List<E_FEE_DETAIL_BK> RemoveSettledFeeDetails(List<E_FEE_DETAIL_BK> allTmcData)
        {
            using (EtzbkDataContext db = new EtzbkDataContext())
            {
                var etrxData = new List<E_FEE_DETAIL_BK>();
                try
                {
                    var uniqueIDs = allTmcData.Select(u => u.UNIQUE_TRANSID).Distinct().ToArray();
                    var uniqueIDsOnDB = db.E_FEE_DETAIL_BK.Where(u => uniqueIDs.Contains(u.EXTERNAL_TRANSID)).Select(u => u.UNIQUE_TRANSID).ToArray();
                    etrxData = allTmcData.Where(u => !uniqueIDsOnDB.Contains(u.UNIQUE_TRANSID)).ToList();


                    return etrxData;
                }
                catch (Exception ex)
                {
                    return etrxData;
                }
            }
        }
        public static bool CheckSettledFee(E_SETTLEMENT_DOWNLOAD_BK e_settlement)
        {
            using (EtzbkDataContext db = new EtzbkDataContext())
            {
               // var etrxData = new E_SETTLEMENT_DOWNLOAD_BK();
                try
                {
                    var etrxData = db.E_SETTLEMENT_DOWNLOAD_BK.Where(s => s.UNIQUE_TRANSID == e_settlement.UNIQUE_TRANSID).ToList();
                    if (etrxData.Count() > 0)
                        return true;
                }
                catch (Exception ex)
                {
                    return false ;
                }
                return false;
            }
        }
        public static String GenerateBatch()
        {

            using (EtzbkDataContext etzbk = new EtzbkDataContext())
            {


                string currentTime = DateTime.Now.ToString("yyMMddHHmm");
                string[] month = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                "K", "L" };
                string[] day = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K",
                "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W",
                "X", "Y", "Z", "1", "2", "3", "4", "5" };
                string[] hour = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V",
                "W", "X" };
                string finalkey = "";
                try
                {
                    int keyID = 0;
                    var batchnumber = etzbk.E_FEEBATCH.FirstOrDefault();
                    if (batchnumber != null)
                    {
                        keyID = batchnumber.KEYID + 1;

                        if (keyID == 999998)
                            keyID = 1;
                        batchnumber = new E_FEEBATCH() { KEYID = keyID };
                        etzbk.E_FEEBATCH.Add(batchnumber);

                    }
                    else
                    {
                        keyID = 1;
                        batchnumber = new E_FEEBATCH() { KEYID = 1 };
                        etzbk.E_FEEBATCH.Add(batchnumber);
                    }

                    String key = keyID.ToString("D6");
                    int mm = int.Parse(currentTime.Substring(2, 2));
                    int dd = int.Parse(currentTime.Substring(4, 2));
                    int hh = int.Parse(currentTime.Substring(6, 2));
                    if (mm == 0)
                        mm = 1;
                    if (dd == 0)
                        dd = 1;
                    if (hh == 0)
                        hh = 1;
                    finalkey = currentTime.Substring(0, 2) + month[mm - 1]
                          + day[dd - 1] + hour[hh - 1] + currentTime.Substring(8)
                          + key;
                    etzbk.SaveChanges();
                }
                catch (Exception _e)
                {
                    Console.WriteLine(_e.Message);
                }
                return finalkey;
            }
        }


        public static string SetSettleBatch()
        {
            string settleBatch = GenerateBatch();

            try
            {
                using (EtzbkDataContext etzbk = new EtzbkDataContext())
                {


                    E_SETTLE_BATCH batch = new E_SETTLE_BATCH() { BATCH_DATE = DateTime.Now, BATCH_ID = settleBatch, CLOSED = "0", START_DATE = DateTime.Today, END_DATE = DateTime.Today.AddDays(1).AddTicks(-1) };

                    etzbk.E_SETTLE_BATCH.Add(batch);

                    etzbk.SaveChanges();
                }
            }
            catch
            {
            }
            return settleBatch;

        }
        public static E_SETTLE_BATCH GetSettleBatch()
        {
            E_SETTLE_BATCH settle_batch = new E_SETTLE_BATCH();
            try
            {
                using (EtzbkDataContext etzbk = new EtzbkDataContext())
                {

                    settle_batch = etzbk.E_SETTLE_BATCH.Where(c => c.CLOSED == "0").OrderBy(c => c.BATCH_DATE).LastOrDefault();
                }
            }
            catch { }

            return settle_batch;
        }

    }
}
