using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace WSAdapter
{
    public class webservers_getdata
    {
        



        private static WSAdapter.ERP_fms.MESWSMain WR = null;
        public static WSAdapter.ERP_fms.MESWSMain wsfun
        {
            get
            {
                if (WR == null)
                {
                    WR = new WSAdapter.ERP_fms.MESWSMain { Url = "http://192.168.2.38:2020/MESWSMain.asmx" };

                }
                return WR;
            }
        }
        //本机测试 web服务
        private static WSAdapter.ERP_MO.MESWSMain MO= null;
        public static WSAdapter.ERP_MO.MESWSMain wsmo
        {
            get
            {
                if (MO == null)
                {
                    MO = new WSAdapter.ERP_MO.MESWSMain { Url = "http://127.0.0.1:80/MESWSMain.asmx" };

                }
                return MO;
            }
        }
 
    }
}
