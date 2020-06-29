using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CPublic
{
    public class FCCommon
    {
        public static Dictionary<string, string> Dic = new Dictionary<string, string>();
        public static void StartCheck()
        {
            Dic.Clear();
        }

        public static String Check_file_GUID(string fileSN)
        {
            try
            {
                string fileSNDesc =  CPublic.CConstrFun.formats(fileSN).ToUpper().Trim();
                if (Dic.ContainsKey(fileSNDesc))
                {
                    return Dic[fileSNDesc];
                }
                //fileSNDesc = CPublic.CConstrFun.formats(fileSNDesc);
                int index = fileSNDesc.IndexOf("(");
                if (index >= 0)
                {
                    fileSNDesc = fileSNDesc.Substring(0, index);
                }
                string sql = string.Format("select * from 文件仓库文件 where (文件编号 like '{0}(%' or 文件编号 = '{0}') order by 默认版本 desc", fileSNDesc);
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.geConn("FC")))
                {
                    da.Fill(dt);
                }
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("这个编号找不到GUID");
                }
                Dic.Add(fileSN, dt.Rows[0]["节点GUID"].ToString());
                return dt.Rows[0]["节点GUID"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void InitData()
        {
            try
            {
                string sql = "select * from 产品CODE表 where 产品CODE号 not in (select 产品CODE号 from 产品CODE关联文件表) ";
                DataTable dtP = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
                {
                    da.Fill(dtP);
                }
                if (dtP.Rows.Count == 0) return;
                DataTable dtM = new DataTable();
                sql = "select *  from 产品CODE关联文件表 where 1<>1 ";
                SqlDataAdapter daM = new SqlDataAdapter(sql, CPublic.Var.strConn);
                new SqlCommandBuilder(daM);
                daM.Fill(dtM);
                foreach (DataRow r in dtP.Rows)
                {
                    dtM.Rows.Add(r["产品CODE号"], r["图号"], r["MPS文件"]);
                }
                daM.Update(dtM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetMGS(Boolean blALL = false)
        {
            try
            {
                InitData();
                string sql = "select 产品CODE表.产品CODE号,产品CODE表.物料描述,产品CODE表.产品编码,产品CODE表.图号,产品CODE表.MPS文件,产品CODE关联文件表.MG文件 from 产品CODE表,产品CODE关联文件表  where 产品CODE表.产品CODE号 = 产品CODE关联文件表.产品CODE号 and 产品CODE关联文件表.MG文件 = '' ";
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
                {
                    da.Fill(dt);
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetPIIS(Boolean blALL = false)
        {
            try
            {
                InitData();
                string sql = "select 产品CODE表.产品CODE号,产品CODE表.物料描述,产品CODE表.产品编码,产品CODE表.图号,产品CODE表.MPS文件,产品CODE关联文件表.PII成检记录,产品CODE关联文件表.PII过程检记录 from 产品CODE表,产品CODE关联文件表   where 产品CODE表.产品CODE号 = 产品CODE关联文件表.产品CODE号 and 产品CODE关联文件表.MG文件 = '' ";
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
                {
                    da.Fill(dt);
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
