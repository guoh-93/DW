using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace CZMaster
{
    /// <summary>
    /// 包含DataTable 一些常用函数 
    /// </summary>
    public class DataTableFun
    {

        /// <summary>
        /// 作者 屈大海 2015-12-03
        /// 追加关联数据，扩展原表
        /// 以下是使用示例， 
        /// "产品编号|cpbh" 表示 产品编号 是 dt的关联KEY，cpbh是dtCP的主KEY。
        /// "产品型号|ggxh" 表示 产品型号 是dt要追加的字段，ggxh是dtCP现有的字段。
        /// 如果没有映射字符"|",那么二个表的字段一致。
        ///    CPublic.CConstrFun.fun_数据关联扩展(dt, dtCP, new string[] { "产品编号|cpbh" }, new string[] { "产品型号|ggxh", "cpmc" });
        ///    CPublic.CConstrFun.fun_数据关联扩展(dt, dtGYS, new string[] { "供应商编号|gysbh" }, new string[] { "gysmc" });
        /// </summary>
        /// <param name="dtO">目标表</param>
        /// <param name="dtS">源数据表</param>
        /// <param name="keys">主键关联，字符 “|” 表示映射</param>
        /// <param name="maps">要追加的子项，字符"|" 表示映射</param>
        public static void fun_数据关联扩展(DataTable dtO, DataTable dtS, string[] keys, string[] maps)
        {
            bool blOne = true;
            foreach (DataRow ro in dtO.Rows)
            {
                if (ro.RowState == DataRowState.Deleted) continue;
                string sql = "";
                for (int i = 0; i < keys.Length; i++)
                {
                    string s1, s2;
                    string[] ss = keys[i].Split(new string[] { "|" }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (ss.Length == 1)
                    {
                        s1 = ss[0]; s2 = ss[0];
                    }
                    else
                    {
                        s1 = ss[0]; s2 = ss[1];
                    }
                    if (dtO.Columns[s1].GetType() != i.GetType())
                    {
                        sql += string.Format("{0} = '{1}' and ", s2, ro[s1]);
                    }
                    else
                    {
                        sql += string.Format("{0} = {1} and ", s2, ro[s1]);
                    }
                }
                sql = sql.Substring(0, sql.Length - 4);
                DataRow[] rs = dtS.Select(sql);
                if (rs.Length > 0)
                {

                    for (int i = 0; i < maps.Length; i++)
                    {
                        string s1, s2;
                        string[] ss = maps[i].Split(new string[] { "|" }, 2, StringSplitOptions.RemoveEmptyEntries);
                        if (ss.Length == 1)
                        {
                            s1 = ss[0]; s2 = ss[0];
                        }
                        else
                        {
                            s1 = ss[0]; s2 = ss[1];
                        }

                        if (blOne == true)
                        {
                            if (dtO.Columns.Contains(s1) == false)
                            {
                                dtO.Columns.Add(s1, rs[0][s2].GetType());
                            }
                        }
                        ro[s1] = rs[0][s2];
                    }
                    blOne = false;
                }

            }
        }


        /// <summary>
        /// /// 作者 屈大海 2015-12-03
        /// 单表数据赋值
        /// 以下使用示例
        /// fun_单行数据赋值(r,new object[]{"产品编号","00002","生产日期",System.Datetime.Now})
        /// </summary>
        /// <param name="r">赋值的row</param>
        /// <param name="Data">赋值数组</param>
        public static void fun_单行数据赋值(DataRow r, object[] Data)
        {
            try
            {
                for (int i = 0; i < Data.Length / 2; i++)
                {
                    r[Data[i * 2].ToString()] = Data[i * 2 + 1];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// /// 作者 屈大海 2015-12-03
        /// 单表数据赋值
        /// 以下使用示例
        /// Dictionary<string, Object> Data = new Dictionary<string, object>();
        /// Data.Add("产品编号", "00002");
        /// Data.Add("生产日期", System.DateTime.Now);
        /// fun_单行数据赋值(r,Data)
        /// </summary>
        /// <param name="r">赋值的row</param>
        /// <param name="Data">赋值数组</param>
        public static void fun_单行数据赋值(DataRow r, Dictionary<string, Object> Data)
        {

            try
            {

                foreach (KeyValuePair<string, Object> kvp in Data)
                {
                    r[kvp.Key] = kvp.Value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
