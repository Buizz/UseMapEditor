using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Map
{
    public partial class MapData
    {
        private int SetUPRP(CUPRP cUPRP)
        {
            for (int i = 0; i < 64; i++)
            {
                if(UPUS[i] == 0)
                {
                    //여기에 기록
                    UPUS[i] = 1;
                    UPRP[i] = cUPRP;

                    return (i + 1);
                }
                if(UPRP[i] == cUPRP)
                {
                    return (i + 1);
                }
            }
            return 0;
        }

    }
}
