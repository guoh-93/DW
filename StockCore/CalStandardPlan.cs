using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace StockCore
{
    public static class CalStandardPlan
    {
        #region 标准计划计算
#pragma warning disable IDE1006 // 命名样式
        public static DataTable fun_标准计划(string strItemNo, string Qty, string strconn)
#pragma warning restore IDE1006 // 命名样式
        {
            //string sql = string.Format("select * from 基础数据物料BOM表 where 产品编码 = '{0}' and 子项编码 <> '' and BOM类型 = '物料BOM'", r["子项编码"].ToString());
            //DataTable t = new DataTable();
            //SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            //da.Fill(t);


            return new DataTable();
        }

#pragma warning disable IDE1006 // 命名样式
        private static DataTable fun_穷尽子项(int iAs, int iMax, DataTable dt1, DataTable dt2, string strconn)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                iAs++;
                if (iAs >= iMax) throw new Exception("获取失败");
                foreach (DataRow r in dt1.Rows)
                {
                    //20160420 当BOM结构中是原材料时，不往下找。因为未来电器的BOM结构有问题
                    string sqlss = string.Format("select 物料类型 from 基础数据物料信息表 where 物料编码 = '{0}'", r["子项编码"].ToString().Trim());
                    SqlDataAdapter dass = new SqlDataAdapter(sqlss, strconn);
                    DataTable dtss = new DataTable();
                    dass.Fill(dtss);
                    if (dtss.Rows.Count > 0 && dtss.Rows[0]["物料类型"].ToString() == "原材料")
                    {
                        continue;
                    }
                    try
                    {
                        string sql = string.Format("select * from 基础数据物料BOM表 where 产品编码 = '{0}' and 子项编码 <> '' and BOM类型 = '物料BOM'", r["子项编码"].ToString());
                        DataTable t = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                        da.Fill(t);
                        t.Columns.Add("层级");
                        foreach (DataRow rrr in t.Rows)
                        {
                            rrr["层级"] = iAs + 1;
                        }
                        if (t.Rows.Count > 0)
                        {
                            //dt2.Merge(t);
                            fun_合并datatable(dt2, t);
                            fun_穷尽子项(iAs, iMax, t, dt2, strconn);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return dt2;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                iAs--;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private static DataTable fun_合并datatable(DataTable dt1, DataTable dt2)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                foreach (DataRow r in dt2.Rows)
                {
                    DataRow[] ds = dt1.Select(string.Format("产品编码 = '{0}' and 子项编码 = '{1}'", r["产品编码"].ToString(), r["子项编码"].ToString()));
                    if (ds.Length == 0)
                    {
                        DataRow dr = dt1.NewRow();
                        dr.ItemArray = r.ItemArray;
                        dt1.Rows.Add(dr);
                    }
                }
                return dt1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
