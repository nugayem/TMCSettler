using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALContext.Model;

namespace TmcSettler.ConsoleApp.Services.FeeService
{
    public class ProccessRatioFee : IFeeProcess
    {
        public List<E_FEE_DETAIL_BK> ProcessCardloadSplit(E_TRANSACTION e_transaction, List<E_COMMISSION_MAP> splitFormular)
        {
            throw new NotImplementedException();
        }
    }
}
