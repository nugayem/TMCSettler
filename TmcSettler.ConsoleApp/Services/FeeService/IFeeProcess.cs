using DALContext.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmcSettler.ConsoleApp.Services.FeeService
{
    public  interface IFeeProcess
    {
        List<E_FEE_DETAIL_BK> ProcessCardloadSplit(E_TRANSACTION e_transaction, List<E_COMMISSION_MAP> splitFormular);
    }
}
