using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poubub.Core
{
    public static class Linq
    {
        public static CVG Process(this CVG inputdata, CVGFunction func, object otherdata  =  null)
        {
            return func.Process(inputdata , otherdata);
        }
    }
}
