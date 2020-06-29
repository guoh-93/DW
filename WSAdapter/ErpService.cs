using System;
using System.Collections.Generic;
using System.Text;

namespace WSAdapter
{
    public  class MasterErpService
    {
        public static string strWSDL = CPublic.Var.strWSConn;

        private static WSAdapter.MasterMESWSMain.MESWSMain WS_MMI = null;
        public static WSAdapter.MasterMESWSMain.MESWSMain WS_Fun
        {
            get
            {
                if (WS_MMI == null)
                {
                    WS_MMI = new WSAdapter.MasterMESWSMain.MESWSMain() { Url = strWSDL };
                }
                return WS_MMI;
            }
        }
    }
}
