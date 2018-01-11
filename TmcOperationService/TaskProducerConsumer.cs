using DALContext;
using DALContext.Model;
using LoggerHelper.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmcOperationService
{
    public class TaskProducerConsumer
    {
        private static BlockingCollection<E_TRANSACTION> enqueData;//= new BlockingCollection<E_Transaction>();
        private readonly static List<ResponseViewModel> responseList = new List<ResponseViewModel>();
        List<E_TRANSACTION> itemsToRemove = new List<E_TRANSACTION>();
        Logger logger = new Logger();

        private string MailTemplate(string body)
        {
            string mailBody = "<b>Dear Team</b><p>";
            mailBody += body;

            return mailBody;
        }
        private string MapTables(List<string> list)
        {

            var db = new EtzbkDataContext();
            var objs = db.E_MERCHANT.Where(a => list.Contains(a.MERCHANT_CODE)).ToList();

            var table = "<table><th>Merchant Code </th><th>Account Number</th><th>Merchant Name </th>";

            foreach (var itm in objs)
            {
                table += "<tr><td>" + itm.MERCHANT_CODE + "</td><td>" + itm.MERCHANT_ACCT + "</td><td>" + itm.MERCHANT_NAME + "</td></tr>";
            }
            table += "</table>";

            return table;
        }

        public void Run()
        {
            enqueData = new BlockingCollection<E_TRANSACTION>();
            Task t1 = Task.Factory.StartNew(Producer);
            Task t2 = Task.Factory.StartNew(Consumer);

            List<Task> taskList = new List<Task> { t1, t2 };

            //Task[] tasksArray = taskList.Where(t => t != null).ToArray();
            //if (tasksArray.Length > 0) Task.WaitAll(tasksArray);

            try
            {
                Task.WaitAll(taskList.ToArray());
            }
            catch ( Exception ex)
            {
                    Logger logger = new Logger();
                  Console.WriteLine("Exception from TaskProducerConsumer Run " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogInfoMessage("Exception from Run TaskProducerConsumer Procuder Method" + ExceptionExtensions.GetFullMessage(ex));

                    return;
               
            }
            List<ErrorType> itemsList = responseList.Select(s => s.ErrorType).Distinct().ToList();
            foreach (var item in itemsList)
            {
                List<string> list = responseList.Where(a => a.ErrorType == item).Select(a => a.MerchantCode).ToList();
                if (item == ErrorType.Exception)
                {
                    if (list.Count > 0)
                    {
                        string table = MapTables(list);
                        MailMessanger.SendMail(Settings.mailsettlementsupport, "ope.adenuga@etranzact.com", "Exception On While processing round", "");
                    }
                }
                else if (item == ErrorType.InvalidSplitType)
                {
                    if (list.Count > 0)
                    {
                        string table = MapTables(list);
                        MailMessanger.SendMail(Settings.mailTrxProcessing, Settings.mailTrxProcessing, "Invalid Split on Type on e_Merchant", MailTemplate("The split type setup for the merchant Code(s) in the table below is/are wrong <p>" + table));
                    }
                }
                else if (item == ErrorType.NoMerchantCode)
                {
                    if (list.Count > 0)
                    {
                        string table = MapTables(list);
                        MailMessanger.SendMail(Settings.mailTrxProcessing, Settings.mailpayoutlet, "Merchant Code not available on e_Merchant", MailTemplate("The following merchant Code(s) are not setup on E_MERCHANT table <p>" + table));
                    }
                }
                else if (item == ErrorType.NoSplitOnMerchant)
                {
                    if (list.Count > 0)
                    {

                        string table = MapTables(list);
                        MailMessanger.SendMail(Settings.mailTrxProcessing, Settings.mailpayoutlet, "Split Not Setup", MailTemplate("Split is not setip for the following merchant Code(s) <p>" + table));
                    }
                }


            }
            Console.WriteLine("TaskProducerConsumer Round Complete");
        }

        private void Producer()
        {
            using (EtzbkDataContext db = new EtzbkDataContext())
            {
                try
                {
                    List<E_TRANSACTION> etzTrx = db.E_TRANSACTION.Where(a => a.PROCESS_STATUS == "0" || a.PROCESS_STATUS == null).ToList();


                    Parallel.ForEach(etzTrx, new ParallelOptions { MaxDegreeOfParallelism = Settings.settlementThreadNumber }, item =>
                    {

                        bool successful = DataManupulation.CheckTransactionStatusOnTMC(item.UNIQUE_TRANSID, item.TRANS_CODE);

                        if (successful)
                        {
                            enqueData.Add(item);
                            Console.WriteLine("Equeued Data " + item.UNIQUE_TRANSID);
                        }
                        else
                        {
                            itemsToRemove.Add(item);
                        }
                    });


                    enqueData.CompleteAdding();
                    DataManupulation.RemoveTransactionFromSettlement(itemsToRemove);
                    //DataManupulation.UpdateTransactionAsProcccessed(etzTrx);
                }
                catch (Exception ex)
                {
                    Logger logger = new Logger();
                    Console.WriteLine("Exception from EtranzactChannelTransaction Run " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogInfoMessage("Exception from Run TaskProducerConsumer Procuder Method" + ExceptionExtensions.GetFullMessage(ex));
                }
            }

        }



        private void Consumer()

        {

        
                List<E_TRANSFER_COMMISSION_SPLIT> splitFormular = CachingProvider.GetCachedData<List<E_TRANSFER_COMMISSION_SPLIT>>("Transfer");
                List<E_FUNDGATE_COMMISSION_SPLIT> fundGatesplitFormular = CachingProvider.GetCachedData<List<E_FUNDGATE_COMMISSION_SPLIT>>("FundGate");

                List<CommissionMapViewModel> commission = AutoMapper.Mapper.Map<List<CommissionMapViewModel>>(splitFormular);
                List<CommissionMapViewModel> fundGatecommission = AutoMapper.Mapper.Map<List<CommissionMapViewModel>>(fundGatesplitFormular);


            Logger logger = new Logger();
            Task loadingTask2 = Task.Factory.StartNew(() =>
                {
                    Parallel.ForEach(enqueData.GetConsumingEnumerable(), new ParallelOptions { MaxDegreeOfParallelism = Settings.settlementThreadNumber }, item =>
                    {
                        try
                        {
                            logger.LogInfoMessage("Parrallel Entered with " +item.TRANS_CODE ); 
                            switch (item.TRANS_CODE)
                            {
                                case "P":
                                    logger.LogInfoMessage("PaymentEntered");
                                    var response = Settlement.ProcessPaymentSettlement(item);
                                    logger.LogInfoMessage("PaymentEntered with Response");
                                    if (response.Response == false)
                                    {
                                        //object obj = new object();
                                        lock (responseList)
                                        {
                                            if (responseList.Where(res => res.MerchantCode == response.MerchantCode).ToList().Count == 0)
                                                responseList.Add(response);
                                        }
                                    }
                                    break;
                                case "T":
                                    logger.LogInfoMessage("transfer Entered");
                                    Settlement.ProcessTransferSettlement(item);
                                    logger.LogInfoMessage("transfer Entered returned");
                                    break;
                                case "D":
                                    logger.LogInfoMessage("CaredloadEntered");
                                    Settlement.ProcessCardLoadSettlement(item);
                                    logger.LogInfoMessage("CaredloadEntered and returned");
                                    break;
                                case "W":

                                    logger.LogInfoMessage("Withdraw Entered");
                                    Settlement.ProcessCardLoadSettlement(item);

                                    logger.LogInfoMessage("Wihdraw Reyurned");
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception from TaskProducerConsumer Consumer Method " + ExceptionExtensions.GetFullMessage(ex));
                            logger.LogInfoMessage("Exception from Run TaskProducerConsumer Consumer Method " + ExceptionExtensions.GetFullMessage(ex));

                        }
                    });
                });
            }
           
        

    }
     
}
