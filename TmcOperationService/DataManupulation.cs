using DALContext.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmcOperationService
{
    public static class DataManupulation
    {

        public static List<T> MergeEntityList<T>( List<List<T>> list)
        {
            int mergedSize = 0;
            foreach(var item   in list )
            {
                mergedSize += item.Count;
            }
            var allTmcData = new List<T>(mergedSize);

            foreach (var item in list)
            {
                allTmcData.AddRange(item);
            }

            return allTmcData;
        }
    }
}
