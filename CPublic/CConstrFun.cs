using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace CPublic
{

    public static class CConstrFun
    {
        /// <summary>
        /// 格式化字符串。去字符串中输入不正确的内容
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string formats(string s)
        {
            s = s.ToUpper().Replace("）", ")");
            s = s.Replace("／", "/");
            s = s.Replace(" ", "");
            s = s.Replace("　", "");
            s = s.Replace("\"", "");
            s = s.Replace("（", "(").Trim();
            return s;
        }

        /// <summary>
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
        /// 单表数据赋值
        /// 以下使用示例
        /// Dictionary<string, Object> Data = new Dictionary<string, object>();
        /// Data.Add("产品编号", "00002");
        /// Data.Add("生产日期", System.DateTime.Now);
        /// fun_单行数据赋值(r,Data)
        /// </summary>
        /// <param name="r">赋值的row</param>
        /// <param name="Data">赋值数组</param>
        public static void fun_单行数据赋值(DataRow r, Dictionary<string,Object> Data)
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


        public static object[] fun_GetFiledDataFormSQL(string strSQL)
        {
            List<object> li = new List<object>();
            DataTable dt; SqlDataAdapter da;
            dt = new DataTable();
            da = new SqlDataAdapter(strSQL, CPublic.Var.strConn);
            try
            {
                da.Fill(dt);
                foreach (DataRow r in dt.Rows)
                {
                    li.Add(r[0]);
                }
                return li.ToArray();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static Boolean fun_得到文件编号及文件名_BTK(string filePath, ref string fileSn, ref string fileName)
        {
            string fn = System.IO.Path.GetFileNameWithoutExtension(filePath);
            string[] ss = fn.Split(new string[] { ";", "；" }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (ss.Length == 2)
            {
                fileSn = formats(ss[0]);
                fileName = formats(ss[1]);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Boolean fun_得到文件编号及文件名(string filePath, ref string fileSn, ref string fileName)
        {
            List<string> s_en = new List<string>();
            List<string> s_CH = new List<string>();
            string[] ss = Path.GetFileNameWithoutExtension(filePath).Split(new char[] { '。' });
            foreach (string s in ss)
            {
                fun_字符串中英文分离(s, s_en, s_CH);
            }
            if (s_en.Count >= 2 && s_CH.Count >= 1)
            {
                fileSn = string.Format("{0}({1})", s_en[0].Trim(), s_en[s_en.Count - 1].Trim());
                fileName = Path.GetFileName(filePath);

                fileName = fileName.Replace("。", "");
                //fileName = fileName.Replace(s_en[0] + "，", "");

                fileName = fileName.Replace(s_en[0], "");
                fileName = fileName.Replace(s_en[s_en.Count - 1], "");

                return true;
            }
            else
            {
                return false;
            }
        }

        public static void fun_字符串中英文分离(string sSource, List<string> s_EN, List<string> s_CH)
        {
            try
            {
                Regex rx = new Regex(@"^[\u4e00-\u9fa5]+$");//中文字符unicode范围   
                string sEN = "", sCH = "";
                for (int i = 0; i < sSource.Length; i++)
                {

                    if (rx.IsMatch(sSource[i].ToString()))
                    {
                        if (sCH != "")
                        {
                            s_EN.Add(sCH);
                            sCH = "";
                        }
                        sEN += sSource[i].ToString();
                    }
                    else
                    {
                        if (sEN != "")
                        {
                            s_CH.Add(sEN);
                            sEN = "";
                        }
                        sCH += sSource[i].ToString();
                    }
                }
                if (sCH != "")
                {
                    s_EN.Add(sCH);
                }
                if (sEN != "")
                {
                    s_CH.Add(sEN);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ///// <summary>
        ///// 界面绑定，绑定方式
        ///// </summary>
        ///// <param name="dtM"></param>
        ///// <param name="tp"></param>
        //public static void fun_FormBind(DataTable dtM, System.Windows.Forms.ToolTip tp)
        //{

        //}
        ///// <summary>
        ///// 界面绑定，赋值方式
        ///// </summary>
        ///// <param name="dtM"></param>
        ///// <param name="tp"></param>
        //public static void fun_FormBind_Data_TO_UI(DataTable dtM, System.Windows.Forms.ToolTip tp)
        //{

        //}
        ///// <summary>
        ///// 界面绑定，赋值方式
        ///// </summary>
        ///// <param name="dtM"></param>
        ///// <param name="tp"></param>
        //public static void fun_FormBind_Data_TO_UI(DataTable dtM, System.Windows.Forms.ToolTip tp)
        //{

        //}


        //c# 获取字符串中的数字 
        ///

        /// 获取字符串中的数字 
        ///

        /// 字符串 
        /// 数字 
        public static Decimal  GetNumber(string str)
        {
            decimal result = 0;
            try
            {
                //string source = "47.64483, -122.141197";
                Regex reg = new Regex(@"-?[\d]+.?[\d]+");
                Match mm = reg.Match(str);
                MatchCollection mc = reg.Matches(str);
                foreach (Match m in mc)
                {
                    result = Decimal.Parse(m.Value.ToString());
                    System.Diagnostics.Debug.WriteLine  (m.Value);
                }

                //if (str != null && str != string.Empty)
                //{
                //    // 正则表达式剔除非数字字符（不包含小数点.） 
                //    str = Regex.Replace(str, @"[^/d./d]", "");
                //    // 如果是数字，则转换为decimal类型 
                //    if (Regex.IsMatch(str, @"^[+-]?/d*[.]?/d*$"))
                //    {
                //        result = Decimal.Parse(str);
                //    }
                //}
            }
            catch
            {
               
            }
            return result;
        }
        ///

        /// 获取字符串中的数字 
        ///

        /// 字符串 
        /// 数字 
        public static int GetNumberInt(string str)
        {
            int result = 0;
            if (str != null && str != string.Empty)
            {
                // 正则表达式剔除非数字字符（不包含小数点.） 
                str = Regex.Replace(str, @"[^/d./d]", "");
                // 如果是数字，则转换为decimal类型 
                if (Regex.IsMatch(str, @"^[+-]?/d*[.]?/d*$"))
                {
                    result = int.Parse(str);
                }
            }
            return result;
        }
    }
}
