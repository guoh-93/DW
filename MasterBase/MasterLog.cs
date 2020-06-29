using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace CZMaster
{
    public class MasterLog
    {

        static List<int> li = new List<int>();
        public static frmLog frmLog = new frmLog();
        
        
        public static void WriteLog(string LogMsg,string OwnerName = "")
        {
            try
            {
                frmLog.BeginInvoke(new MethodInvoker(() =>
                {
                    DataRow r = frmLog.dtLog.NewRow();

                    r[0] = System.DateTime.Now;
                    r[1] = OwnerName;
                    r[2] = LogMsg;
                    frmLog.dtLog.Rows.Add(r);
                    if (frmLog.dtLog.Rows.Count >= 1001)
                    {
                        frmLog.dtLog.Rows.RemoveAt(0);
                    }
                    //frmLog.dtLog.Rows.InsertAt(r,0);

                    //frmLog.dtLog.Rows.Add(System.DateTime.Now, OwnerName, LogMsg);
                }));
            }
            catch
            {
                lock (li)
                {

                    DataRow r = frmLog.dtLog.NewRow();

                    r[0] = System.DateTime.Now;
                    r[1] = OwnerName;
                    r[2] = LogMsg;
                    frmLog.dtLog.Rows.Add(r);
                    if (frmLog.dtLog.Rows.Count >= 1001)
                    {
                        frmLog.dtLog.Rows.RemoveAt(0);
                    }
                }
            }
        }
        
    }
}
