using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace CPublic
{
    //记录操作日志


    class CLog
    {
        public static void writeLog(string UID,string UNAME,string strLOG)
        {
            writeLog(UID, UNAME, strLOG, new SqlDataAdapter("select * from tLog",CPublic.Var .strConn ));
        }
        public static void writeLog(string UID, string UNAME, string strLOG,SqlConnection conn,SqlTransaction tran)
        {
            SqlDataAdapter das = new SqlDataAdapter(new SqlCommand("select * from tLog", conn, tran));
            writeLog(UID, UNAME, strLOG, das);
        }
        public static void writeLog(string UID, string UNAME, string strLOG,SqlDataAdapter daS)
        {
            try
            {
                DataTable dt = new DataTable();
                daS.FillSchema(dt, SchemaType.Mapped);
                dt.Rows.Add(UID, UNAME, strLOG, DateTime.Now);
                daS.Update(dt);
            }
            catch (Exception ex)
            {
                throw new Exception("写日志出错：" + ex.Message);
            }
        }
    }
}
