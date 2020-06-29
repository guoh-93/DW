using System;
using System.Collections.Generic;
using System.Text;

namespace CZMaster
{
    public class MasterFileService

    {
        public static string strWSDL = SQLiteConnectionString.GetValueFormCFG("WS");

        private static WSAdapter.MasterBaseService.MasterBaseService WS_MMI = null;
        public static WSAdapter.MasterBaseService.MasterBaseService WS_Fun
        {
            get
            {
                if (WS_MMI == null)
                {
                    WS_MMI = new WSAdapter.MasterBaseService.MasterBaseService() { Url = strWSDL };
                }
                return WS_MMI;
            }
        }

        public static string BOLBUpload(byte[] bs)
        {
            try
            {
                return WS_Fun.BOLBUpload(bs); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static byte[] BOLBDownLoad(string strGUID)
        {
            try
            {
                return WS_Fun.BOLBDownLoad(strGUID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
