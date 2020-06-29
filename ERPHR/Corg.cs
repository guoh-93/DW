using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;



namespace ERPorg
{
    public class Corg
    {


        static string strcon_U8 = CPublic.Var.geConn("DW");
        static string strcon = CPublic.Var.strConn;

        /// <summary>
        ///   dt1是整个表 dt为返回值
        ///   orgname 为组织架构名称
        ///   gh 为工号
        ///   该函数返回 gh工号的 所有 下级 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="?"></param>     
        /// <returns></returns>
        public static DataTable fun_hr(string orgname, string gh)
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            string upperGUID;
            string sql1 = string.Format("select * from  人事记录组织{0}关系表", orgname);
            {
                using (SqlDataAdapter da = new SqlDataAdapter(sql1, CPublic.Var.strConn))
                {
                    da.Fill(dt1);
                }
            }

            string sql = string.Format("select * from 人事记录组织{0}关系表 where 工号='{1}'", orgname, gh);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {

                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    upperGUID = dt.Rows[0]["GUID"].ToString();
                    DataRow[] dr = dt1.Select(string.Format("上级GUID='{0}'", upperGUID));
                    for (int i = 0; i < dr.Length; i++)
                    {
                        DataRow rr = dt.NewRow();
                        rr.ItemArray = dr[i].ItemArray;
                        dt.Rows.Add(rr);
                        string a = dr[i]["GUID"].ToString().Trim();
                        fun_hr_zi(a, dt1, dt);
                    }
                }
                //else
                //{
                //MessageBox.Show("未找到此工号");
                //}
            }
            return dt;
        }
        /// <summary>
        /// a 为上级GUID
        /// 该函数为取子节点
        /// </summary>
        /// <param name="a"></param>
        /// <param name="dt"></param>
        private static DataTable fun_hr_zi(string a, DataTable dt1, DataTable dt)
        {
            DataRow[] dr = dt1.Select(string.Format("上级GUID='{0}'", a));
            foreach (DataRow r in dr)
            {
                DataRow rr = dt.NewRow();
                rr.ItemArray = r.ItemArray;
                dt.Rows.Add(rr);
                fun_hr_zi(r["GUID"].ToString().Trim(), dt1, dt);
            }

            return dt;
        }

        /// <summary>
        /// 该函数返回 该gh工号的直接上级
        /// 列名为  姓名  工号 
        /// 2018-8-22 改
        /// Doctype 单据类型 
        /// </summary>
        public static DataRow fun_hr_upper(string Doctype, string gh)
        {
            string sql = string.Format("select  上级用户ID as 姓名,上级工号 as 工号 from   单据审批流表   where  单据类型='{0}' and 工号='{1}' and 上级工号<>'' ", Doctype, gh);
            DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql, CPublic.Var.strConn);
            return dr;
        }



        //public static DataRow  fun_hr_upper(string orgname, string gh)
        //{
        //    string sql = string.Format("select  上级用户ID as 姓名,上级工号 as 工号 from  人事记录组织{0}关系表 where 工号='{1}'", orgname,gh);
        //    DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql,CPublic.Var.strConn);
        //    return dr;
        //}
        public bool fun_权限(string str_ID, string str_功能)
        {
            string sql = string.Format(@"select 人事基础员工表.权限组,人事基础员工表.员工号 , 功能权限权限组权限表.* from 人事基础员工表 
                                        left join 功能权限权限组权限表  on 人事基础员工表.权限组= 功能权限权限组权限表.权限组 
                                        where  人事基础员工表.员工号='{0}'", str_ID);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                DataTable dt_1 = new DataTable();
                da.Fill(dt_1);
                DataView dv = new DataView(dt_1);
                dv.RowFilter = "审核=1";
                DataTable dt = dv.ToTable();
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["权限类型"].ToString() == str_功能)
                    {
                        return true;
                    }
                    else
                    {
                        if (fun_qx(dt, str_功能) == true)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        private Boolean fun_qx(DataTable dt_传递, string str_功能)
        {
            string sql = string.Format("select * from 功能权限权限类型表 where 权限类型='{0}'", str_功能);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow[] dr = dt_传递.Select(string.Format("权限类型='{0}'", dt.Rows[0]["上级权限类型"].ToString()));
                    if (dr.Length > 0)
                    {
                        return true;
                    }
                    else if (dt.Rows[0]["上级权限类型"].ToString() == "")
                    {
                        return false;
                    }
                    else
                    {
                        fun_qx(dt, dt.Rows[0]["上级权限类型"].ToString());
                    }
                }
                return false;
            }
        }
        public static bool fun_isneedup()
        {
            Boolean blUP = false;

            try
            {
                string strDir = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                string sKey = "MES_keys";
                string path = strDir + "\\conn.cfg";
                DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
                DES.Key = ASCIIEncoding.UTF8.GetBytes(sKey);
                DES.IV = ASCIIEncoding.UTF8.GetBytes(sKey);
                ICryptoTransform desencrypt = DES.CreateDecryptor();
                byte[] data = File.ReadAllBytes(path);
                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
                MemoryStream ms = new MemoryStream(result);
                DataTable dt = new DataTable();
                dt.ReadXml(ms);
                Dictionary<string, string> li = new Dictionary<string, string>();
                foreach (DataRow r in dt.Rows)
                {
                    li.Add(r[0].ToString(), r[1].ToString());
                }
                string strAppName = "Future";
                string ole = string.Format("Provider=SQLOLEDB.1;Password={0};Persist Security Info=True;User ID={1};Initial Catalog={2};Data Source={3};Pooling=true;Max Pool Size=40000;Min Pool Size=0",
                        li["PWD"], li["UID"], li["DataBase"], li["SQLServer"]);
                string sql = "select moduleName,ver,hash,fileName,moduleDesc from moduleData where fileName='bb.txt'";
                DataTable dtCurrM = new DataTable();
                string str_dt_moduleVer = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + string.Format("\\{0}\\moduleVer.cfg", strAppName);
                try
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(str_dt_moduleVer));
                }
                catch { }
                if (File.Exists(str_dt_moduleVer))
                {
                    dtCurrM.ReadXml(str_dt_moduleVer);
                }
                DataTable dtM = new DataTable();
                using (OleDbDataAdapter da = new OleDbDataAdapter(sql, ole))
                {
                    da.Fill(dtM);
                    DataRow r1 = dtM.Rows[0];




                    //得到标准的文件路径
                    string strFile = r1["fileName"].ToString();

                    if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(strFile)) == false)
                    {
                        strFile = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\" +
                            r1["fileName"].ToString();
                    }
                    sql = string.Format("moduleName = '{0}'", r1["moduleName"].ToString());
                    if (dtCurrM.Rows.Count == 0)
                    {
                        blUP = true;
                    }
                    else
                    {
                        DataRow[] rs = dtCurrM.Select(sql);
                        if (rs.Length > 0)
                        {
                            if (System.IO.File.Exists(strFile) == false || (int)r1["ver"] > (int)rs[0]["ver"])
                            {
                                blUP = true;
                            }
                        }
                        else
                        {
                            blUP = true;
                        }
                    }

                }
            }
            catch (Exception)
            {
                //Debug.WriteLine(ex.Message);
            }



            return blUP;
        }

        /// <summary>
        /// 判断是否需要弹窗显示更新信息
        /// </summary>
        /// <returns></returns>
        public static bool fun_isnd()
        {
            bool flag = false;
            string str_人员ID = CPublic.Var.LocalUserID;
            string pcname = System.Net.Dns.GetHostName();

            string sql_bb = "select * from  moduleData where moduleName='FMS_BB'";
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql_bb, CPublic.Var.strConn);
            if (dt.Rows.Count > 0)
            {
                //ver 当前版本 
                int ver = Convert.ToInt32(dt.Rows[0]["ver"]);
                string sql = string.Format("select * from 用户登录注册表 where 计算机名='{0}' and 工号='{1}'", pcname, str_人员ID);
                DataTable dt_zcb = new DataTable();
                dt_zcb = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                if (dt_zcb.Rows.Count > 0)
                {
                    //ver_老 为 上次该用户 在此计算机登录时记录的版本号
                    int ver_老 = Convert.ToInt32(dt_zcb.Rows[0]["更新日志版本号"]);
                    if (ver_老 < ver)
                    {

                        dt_zcb.Rows[0]["更新日志版本号"] = ver;
                        dt_zcb.Rows[0]["修改时间"] = CPublic.Var.getDatetime();

                        flag = true;
                    }
                }
                else
                {
                    DataRow drr = dt_zcb.NewRow();
                    drr["工号"] = str_人员ID;
                    drr["姓名"] = CPublic.Var.localUserName;
                    drr["计算机名"] = pcname;
                    drr["更新日志版本号"] = ver;
                    drr["修改时间"] = CPublic.Var.getDatetime();
                    dt_zcb.Rows.Add(drr);

                    flag = true;

                }
                using (SqlDataAdapter da = new SqlDataAdapter("select  * from 用户登录注册表 where 1<>1", CPublic.Var.strConn))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt_zcb);
                }
            }
            return flag;
        }
        public static string[] fun_版本号()
        {
            string sql = "select  * from [moduleData]  where moduleName='FMS_BB' ";
            string strcon = CPublic.Var.strConn;
            SqlConnection con = new SqlConnection(strcon);
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            SqlDataReader dr = cmd.ExecuteReader();
            DataSet myds = new DataSet();
            da.Fill(myds);

            con.Close();
            string[] s = { "", "" };
            if (myds.Tables.Count > 0 && myds.Tables[0].Rows.Count > 0)
            {
                Byte[] Files = (Byte[])myds.Tables[0].Rows[0]["moduleData"];

                string path = Application.StartupPath;
                FileStream fs = new FileStream(path + @"\版本.txt", FileMode.Create, FileAccess.Write);
                fs.Write(Files, 0, Files.Length);
                fs.Close();
                // StreamReader sr = new StreamReader(path + @"\版本.txt", Encoding.GetEncoding("GB2312"));

                StreamReader sr = new StreamReader(path + @"\版本.txt", Encoding.GetEncoding("utf-8"));



                s[0] = sr.ReadLine();
                s[1] = sr.ReadToEnd();

                sr.Close();

            }
            return s;
        }

        /// <summary>
        /// 参数为sql 条件  
        /// </summary>
        /// <returns></returns>
        public static DataTable fun_供应商(string str_条件)
        {
            DataTable dt = new DataTable();
            string sql = "select * from 采购供应商表 ";
            if (str_条件 != "")
            {
                sql = sql + "where " + str_条件;
            }
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                da.Fill(dt);
            }
            return dt;
        }
        /// <summary>
        /// 参数为sql 条件  不需where 
        /// </summary>
        /// <returns></returns>
        public static DataTable fun_客户(string str_条件)
        {
            DataTable dt = new DataTable();
            string sql = "select * from 客户基础信息表 ";
            if (str_条件 != "")
            {
                sql = sql + "where " + str_条件;
            }
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                da.Fill(dt);
            }
            return dt;
        }
        /// <summary>
        /// T中需包含物料的仓库号信息,物料编码,数量
        /// 2018-10-23 T 中需有计量单位,如有单位需要换算,在函数中操作。
        /// 采购 漆包线 多少公斤   发料 发了几米  然后  发料的 流水表里 数量 和单位 记哪个
        /// </summary>
        /// <param name="i_正负"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        public static DataTable fun_库存(int i_正负, DataTable T)
        {
            if (T.Columns.Contains("计量单位"))
            {
                foreach (DataRow r in T.Rows)
                {
                    string s = string.Format("select  单位换算标识,计量单位 from 基础数据物料信息表 where 物料编码='{0}'", r["物料编码"]);
                    DataRow rr = CZMaster.MasterSQL.Get_DataRow(s, CPublic.Var.strConn);
                    if (rr["单位换算标识"].Equals(true))
                    {
                        string ss = string.Format("select  * from 计量单位换算表 where 物料编码='{0}'", r["物料编码"]);
                        using (SqlDataAdapter aa = new SqlDataAdapter(ss, CPublic.Var.strConn))
                        {
                            DataTable tt = new DataTable();
                            aa.Fill(tt);
                            DataRow[] r1 = tt.Select(string.Format("计量单位='{0}'", r["计量单位"].ToString().Trim())); //这里取得T里面的计量单位 

                            DataRow[] r2 = tt.Select(string.Format("计量单位='{0}'", rr["计量单位"].ToString().Trim()));  //这里取得 基础数据物料信息表的计量单位,与库存计量单位一致

                            decimal dec = Convert.ToDecimal(r1[0]["换算率"]) / Convert.ToDecimal(r2[0]["换算率"]);   // 例 1公斤 =5882 米      dec=1/5882
                            r["数量"] = Convert.ToDecimal(r["数量"]) * dec;
                        }

                    }

                }
            }
            DataTable dt = new DataTable();
            foreach (DataRow dr in T.Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                {
                    continue;
                }

                if (dt.Rows.Count == 0) //当 
                {
                    string sql = string.Format("select * from 仓库物料数量表 where 物料编码='{0}' and 仓库号='{1}'", dr["物料编码"].ToString(), dr["仓库号"].ToString());
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
                    {
                        da.Fill(dt);
                    }
                }
                DataRow[] x = dt.Select(string.Format("物料编码='{0}' and 仓库号='{1}'", dr["物料编码"].ToString(), dr["仓库号"].ToString()));
                if (x.Length == 0)
                {
                    string sql = string.Format("select * from 仓库物料数量表 where 物料编码='{0}' and 仓库号='{1}'", dr["物料编码"].ToString(), dr["仓库号"].ToString());
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
                    {
                        da.Fill(dt);
                    }
                    x = dt.Select(string.Format("物料编码='{0}' and 仓库号='{1}'", dr["物料编码"].ToString(), dr["仓库号"].ToString()));
                    if (x.Length == 0)
                    {
                        DataRow dd = dt.NewRow();
                        dd["GUID"] = System.Guid.NewGuid();
                        dd["物料编码"] = dr["物料编码"];
                        dd["物料名称"] = dr["物料名称"];
                        dd["规格型号"] = dr["规格型号"];
                        dd["仓库号"] = dr["仓库号"];
                        dd["库存总数"] = 0;
                        dd["在途量"] = 0;
                        dd["在制量"] = 0;
                        dd["受订量"] = 0;
                        dd["未领量"] = 0;
                        dd["仓库名称"] = dr["仓库名称"];
                        //if (T.Columns.Contains("新货架描述"))
                        //{
                        //    if(dr["新货架描述"].ToString() != "")
                        //    {
                        //        dd["货架描述"] = dr["新货架描述"];
                        //    }                                                      
                        //}
                        dt.Rows.Add(dd);
                    }
                }

                x = dt.Select(string.Format("物料编码='{0}' and 仓库号='{1}'", dr["物料编码"].ToString(), dr["仓库号"].ToString()));
                decimal dec = Convert.ToDecimal(x[0]["库存总数"]) + i_正负 * Convert.ToDecimal(dr["数量"].ToString());
                if (dec < 0)
                {
                    throw new Exception("库存不足,请确认");
                }

                x[0]["库存总数"] = dec;
                //if (T.Columns.Contains("新货架描述"))
                //{
                //    if (dr["新货架描述"].ToString() != "")
                //    {    
                //        x[0]["货架描述"] = dr["新货架描述"];
                //    }

                //}
                x[0]["有效总数"] = dec + Convert.ToDecimal(x[0]["在途量"]) + Convert.ToDecimal(x[0]["在制量"]) - Convert.ToDecimal(x[0]["受订量"]) - Convert.ToDecimal(x[0]["未领量"]);
                x[0]["出入库时间"] = CPublic.Var.getDatetime();

            }

            return dt;
        }
        //传入业务员姓名,返回片区集合dt 去除所有片区的 经理   组织架构中 的 营销一部 和 二部  领导
        //19-4-4 这里东屋用不到
        public static DataTable fun_业务员片区(string name)
        {
            DataTable dt = new DataTable();

            string sql = @"select   业务员,片区 from 客户基础信息表 
                          where 业务员<>'' group  by  业务员,片区  order by 业务员";
            dt = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);

            string ss = "select  * from 人事基础部门表 where 部门编号='00010101' or 部门编号='00010102'";
            DataTable temp = CZMaster.MasterSQL.Get_DataTable(ss, CPublic.Var.strConn);
            if (temp.Rows.Count > 0)
            {
                DataRow[] xr = temp.Select(string.Format("领导姓名='{0}'", name.Trim()));
                if (xr.Length > 0)
                {
                    DataTable t = new DataTable();

                    return t;
                }
                else
                {
                    DataRow[] r = dt.Select(string.Format("业务员='{0}'", name));
                    DataTable t = new DataTable();
                    t.Columns.Add("片区");
                    foreach (DataRow dr in r)
                    {
                        t.ImportRow(dr);
                    }
                    return t;
                }
            }
            else
            {

                DataTable t = new DataTable();

                return t;
            }

        }

        public class BardCodeHooK
        {
            public delegate void BardCodeDeletegate(BarCodes barCode);
            public event BardCodeDeletegate BarCodeEvent;


            public struct BarCodes
            {
                public int VirtKey;//虚拟吗  
                public int ScanCode;//扫描码  
                public string KeyName;//键名  
                public uint Ascll;//Ascll  
                public char Chr;//字符  


                public string BarCode;//条码信息  
                public bool IsValid;//条码是否有效  
                public DateTime Time;//扫描时间  
            }


            private struct EventMsg
            {
                public int message;
                public int paramL;
                public int paramH;
                public int Time;
                public int hwnd;
            }


            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);


            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            private static extern bool UnhookWindowsHookEx(int idHook);


            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            private static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);


            [DllImport("user32", EntryPoint = "GetKeyNameText")]
            private static extern int GetKeyNameText(int IParam, StringBuilder lpBuffer, int nSize);


            [DllImport("user32", EntryPoint = "GetKeyboardState")]
            private static extern int GetKeyboardState(byte[] pbKeyState);


            [DllImport("user32", EntryPoint = "ToAscii")]
            private static extern bool ToAscii(int VirtualKey, int ScanCode, byte[] lpKeySate, ref uint lpChar, int uFlags);


            delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
            BarCodes barCode = new BarCodes();
            int hKeyboardHook = 0;
            string strBarCode = "";


            private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
            {
                if (nCode == 0)
                {
                    EventMsg msg = (EventMsg)Marshal.PtrToStructure(lParam, typeof(EventMsg));
                    if (wParam == 0x100)//WM_KEYDOWN=0x100  
                    {
                        barCode.VirtKey = msg.message & 0xff;//虚拟吗  
                        barCode.ScanCode = msg.paramL & 0xff;//扫描码  
                        StringBuilder strKeyName = new StringBuilder(225);
                        if (GetKeyNameText(barCode.ScanCode * 65536, strKeyName, 255) > 0)
                        {
                            barCode.KeyName = strKeyName.ToString().Trim(new char[] { ' ', '\0' });
                        }
                        else
                        {
                            barCode.KeyName = "";
                        }
                        byte[] kbArray = new byte[256];
                        uint uKey = 0;
                        GetKeyboardState(kbArray);




                        if (ToAscii(barCode.VirtKey, barCode.ScanCode, kbArray, ref uKey, 0))
                        {
                            barCode.Ascll = uKey;
                            barCode.Chr = Convert.ToChar(uKey);
                        }


                        TimeSpan ts = DateTime.Now.Subtract(barCode.Time);


                        if (ts.TotalMilliseconds > 50)
                        {
                            strBarCode = barCode.Chr.ToString();
                        }
                        else
                        {
                            if ((msg.message & 0xff) == 13 && strBarCode.Length > 3)
                            {
                                barCode.BarCode = strBarCode;
                                barCode.IsValid = true;
                            }
                            strBarCode += barCode.Chr.ToString();
                        }
                        barCode.Time = DateTime.Now;
                        if (BarCodeEvent != null)
                        {
                            BarCodeEvent(barCode);//触发事件  
                        }

                        barCode.IsValid = false;
                    }
                }
                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
            }


            //安装钩子  
            public bool Start()
            {
                if (hKeyboardHook == 0)
                {
                    //WH_KEYBOARD_LL=13  
                    hKeyboardHook = SetWindowsHookEx(13, new HookProc(KeyboardHookProc), Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                }
                return (hKeyboardHook != 0);
            }


            //卸载钩子  
            public bool Stop()
            {
                if (hKeyboardHook != 0)
                {
                    return UnhookWindowsHookEx(hKeyboardHook);
                }
                return true;
            }



        }

        /// <summary>
        /// 取前21位 3-1 权重 各位乘权重后相加
        /// mod10(10-mod10(sum))
        /// 返回 string 
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static string fun_gccode(string sn)
        {
            //1.获取前21位 
            string s = sn.Substring(0, 21);
            char[] ss = s.ToCharArray();
            //权重值 奇数位为3 偶数位为1  
            //对应位的值乘以权重  并 累加
            int sum = 0;
            int i = 1;
            int weight = 1;
            foreach (char c in ss)
            {
                if (i % 2 == 0) //权重为1 
                {
                    weight = 1;
                }
                else
                {
                    weight = 3;
                }
                i++;
                sum = sum + Convert.ToInt32(c.ToString()) * weight;
            }
            // 10 - (和值模10) 
            int mod = (10 - sum % 10) % 10;

            s = mod.ToString();
            return s;
        }
        /// <summary>
        /// 读取Excel.xlsx
        /// </summary>
        /// <param name="fd">Excel.xlsx文件所在路径</param>
        public static DataTable ExcelXLSX(System.Windows.Forms.OpenFileDialog fd)
        {

            try
            {
                DataTable dt = new DataTable();
                string strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + fd.FileName + ";Extended Properties='Excel 12.0; HDR=YES; IMEX=1'"; //此連接可以操作.xls與.xlsx文件
                OleDbConnection conn = new OleDbConnection(strConn);
                conn.Open();
                DataSet ds = new DataSet();
                //dt1  为excel中 所有sheet名字集合
                DataTable dt1 = new DataTable();

                dt1 = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                String[] excelSheets = new String[dt1.Rows.Count];
                int i = 0;
                // 添加工作表名称到字符串数组    
                foreach (DataRow row in dt1.Rows)
                {
                    string strSheetTableName = row["TABLE_NAME"].ToString();
                    //过滤无效SheetName   
                    if (strSheetTableName.Contains("$") && strSheetTableName.Replace("'", "").EndsWith("$"))
                    {
                        excelSheets[i] = strSheetTableName.Substring(0, strSheetTableName.Length - 1);
                        OleDbDataAdapter odda = new OleDbDataAdapter(string.Format("SELECT * FROM [{0}]", excelSheets[i] + "$"), conn);//("select * from [Sheet1$]", conn);
                        odda.Fill(ds, excelSheets[i] + "$");
                    }
                    else
                    {
                        string str = excelSheets[i];
                    }


                    i++;
                }

                conn.Close();
                dt = ds.Tables[0];

                foreach (DataTable dt2 in ds.Tables)
                {
                    dt.Merge(dt2);
                }

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        public bool ConverterToPdf(string _lstrInputFile, string _lstrOutFile)
        {
            Microsoft.Office.Interop.Excel.Application lobjExcelApp = null;
            Microsoft.Office.Interop.Excel.Workbooks lobjExcelWorkBooks = null;
            Microsoft.Office.Interop.Excel.Workbook lobjExcelWorkBook = null;

            string lstrTemp = string.Empty;
            object lobjMissing = System.Reflection.Missing.Value;

            try
            {
                lobjExcelApp = new Microsoft.Office.Interop.Excel.Application();
                lobjExcelApp.Visible = false;
                lobjExcelWorkBooks = lobjExcelApp.Workbooks;
                lobjExcelWorkBook = lobjExcelWorkBooks.Open(_lstrInputFile, true, true, lobjMissing, lobjMissing, lobjMissing, true,
                    lobjMissing, lobjMissing, lobjMissing, lobjMissing, lobjMissing, false, lobjMissing, lobjMissing);

                //Microsoft.Office.Interop.Excel 12.0.0.0之后才有这函数              
                //lstrTemp = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".xls" + (lobjExcelWorkBook.HasVBProject ? 'm' : 'x');  
                //lstrTemp = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".xls";  
                //lobjExcelWorkBook.SaveAs(lstrTemp, Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel4Workbook, Type.Missing, Type.Missing, Type.Missing, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing,  
                //    false, Type.Missing, Type.Missing, Type.Missing);  
                //输出为PDF 第一个选项指定转出为PDF,还可以指定为XPS格式  
                lobjExcelWorkBook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, _lstrOutFile, Microsoft.Office.Interop.Excel.XlFixedFormatQuality.xlQualityStandard, Type.Missing, false, Type.Missing, Type.Missing, false, Type.Missing);
                lobjExcelWorkBook.Close();
                lobjExcelWorkBooks.Close();
                lobjExcelApp.Quit();
            }
            catch (Exception)
            {
                //其他日志操作；  
                return false;
            }
            finally
            {
                lobjExcelApp.Visible = false;
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)lobjExcelWorkBook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)lobjExcelWorkBooks);
                lobjExcelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)lobjExcelApp);
                //主动激活垃圾回收器，主要是避免超大批量转文档时，内存占用过多，而垃圾回收器并不是时刻都在运行！  
                GC.Collect();
                GC.WaitForPendingFinalizers();



            }
            return true;
        }
        /// <summary>
        /// 读打印机配置文件
        /// </summary>
        /// <param name="filePathName"></param>
        /// <returns></returns>
        public static List<String[]> ReadTxt(string filePathName)
        {
            List<String[]> ls = new List<String[]>();
            StreamReader fileReader = new StreamReader(filePathName);
            string strLine = "";
            while (strLine != null)
            {
                strLine = fileReader.ReadLine();
                if (strLine != null && strLine.Length > 0)
                {
                    ls.Add(strLine.Split('\n'));
                    //Debug.WriteLine(strLine);
                }
            }
            fileReader.Close();
            return ls;
        }

        //取该物料一个顶级父项
        public static DataTable fun_运算_成品(DataTable t_return, string str_物料编号, string str_产品线)
        {

            DataTable dt = new DataTable();
            string s = string.Format(@"select  c.原ERP物料编号,c.产品线 from 基础数据物料BOM表 a,基础数据物料信息表 b,基础数据物料信息表 c
            where a.子项编码=b.物料编码 and c.物料编码=产品编码 and b.原ERP物料编号 ='{0}'", str_物料编号);
            dt = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn); // 上一级父项 
            if (dt.Rows.Count == 0) //没有父项即为顶层物料
            {

                DataRow r = t_return.NewRow();
                r["父项编号"] = str_物料编号;
                r["产品线"] = str_产品线;
                t_return.Rows.Add(r);
                return t_return;

            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    t_return = fun_运算_成品(t_return, dr["原ERP物料编号"].ToString(), dr["产品线"].ToString());
                    if (t_return.Rows[t_return.Rows.Count - 1]["产品线"].ToString() != "生产原料")
                    {
                        break;
                    }
                }
            }

            return t_return;


        }


        ///<summary>
        /// 清空指定的文件夹，但不删除文件夹
        /// </summary>
        /// <param name="dir"></param>
        public void DeleteFolder(string dir)
        {

            foreach (string d in Directory.GetFileSystemEntries(dir))
            {
                if (File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    {
                        fi.Attributes = FileAttributes.Normal;
                    }

                    File.Delete(d);//直接删除其中的文件  
                }
                else
                {
                    DirectoryInfo d1 = new DirectoryInfo(d);
                    if (d1.GetFiles().Length != 0)
                    {
                        DeleteFolder(d1.FullName);////递归删除子文件夹
                    }
                    Directory.Delete(d);
                }
            }
        }
        /// <summary>
        /// 删除文件夹及其内容
        /// </summary>
        /// <param name="dir"></param>
        public void DeleteFolder1(string dir)
        {
            foreach (string d in Directory.GetFileSystemEntries(dir))
            {
                if (File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    {
                        fi.Attributes = FileAttributes.Normal;
                    }

                    File.Delete(d);//直接删除其中的文件  
                }
                else
                {
                    DeleteFolder(d);////递归删除子文件夹
                }

                Directory.Delete(d);
            }
        }




        public static bool EmailIsMatch(string adress)
        {
            bool bl = false;
            // string EmailStr=@"([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,5})+";
            string EmailStr = "^/w+([-+.]/w+)*@/w+([-.]/w+)*/./w+([-.]/w+)*$";
            Regex emailreg = new Regex(EmailStr);
            bl = emailreg.IsMatch(adress.Trim());
            return bl;
        }
        /// <summary>
        /// Reacquire the purchase price.
        /// </summary>
        /// <param name="supplier"></param>
        /// <param name="mnum"></param>
        /// <returns></returns>
        public static decimal ReacqPP(string supplier, string mnum)
        {
            //string ss="";
            //if (supplier != "")
            //{
            //  ss=string.Format("供应商ID ='{0}' and ",supplier);

            //}
            decimal dec = 0;
            string s = string.Format("select  单价 from  采购供应商物料单价表 where  供应商ID ='{0}' and 物料编码 ='{1}'", supplier, mnum);
            using (SqlDataAdapter da = new SqlDataAdapter(s, CPublic.Var.strConn))
            {
                DataTable temp = new DataTable();
                da.Fill(temp);
                if (temp.Rows.Count == 0)
                {

                    //throw new Exception("物料" + mnum + "在该供应商下没有维护单价");

                }
                else
                {
                    dec = Convert.ToDecimal(temp.Rows[0][0]);
                }

            }
            return dec;

        }
        /// <summary>
        /// 2018-6-25暂时用来控制界面按钮
        /// </summary>
        /// <returns></returns>
        public static bool btn_perm(DataTable perm_list, string btn_name)
        {
            bool bl = true;
            if (perm_list.Rows.Count == 0)  // 
            {
                bl = false;
            }
            else
            {
                DataRow[] dr = perm_list.Select(string.Format("按钮='{0}'", btn_name));
                if (dr.Length == 0)
                {
                    bl = false;
                }
            }
            return bl;
        }


        /// <summary>
        /// 生成待审核单
        /// </summary>
        /// <param name="czlx">操作类型 生效或者关闭 </param>
        /// <param name="doctype">单据类型</param>
        /// <param name="str_单号">单号</param>
        /// <param name="str_相关单位"></param>
        /// <returns></returns>
        public static DataTable fun_PA(string czlx, string doctype, string str_单号, string str_相关单位)
        {
            DataRow r_upper = ERPorg.Corg.fun_hr_upper(doctype, CPublic.Var.LocalUserID);
            if (str_相关单位 != "入库倒冲")
            {
                //DataRow r_upper = ERPorg.Corg.fun_hr_upper(doctype, CPublic.Var.LocalUserID);
                if (r_upper == null || r_upper["工号"].ToString() == "")
                {
                    throw new Exception(doctype + "审批流中未找到您相应的权限");
                }
            }

            DataTable dt_申请;
            string s = string.Format("select * from  单据审核申请表 where 关联单号='{0}' and 审核 = 0 and 作废 = 0", str_单号);
            dt_申请 = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn);
            DateTime t = CPublic.Var.getDatetime();
            string str_pa = "";
            if (dt_申请.Rows.Count == 0)
            {
                str_pa = string.Format("AP{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("AP", t.Year, t.Month));
                // 申请主表记录
                DataRow r_z = dt_申请.NewRow();
                r_z["审核申请单号"] = str_pa;
                r_z["关联单号"] = str_单号;
                r_z["相关单位"] = str_相关单位;
                r_z["操作类型"] = czlx;
                r_z["单据类型"] = doctype;
                //decimal dec = Convert.ToDecimal(txt_cgshhje.Text);
                //r_z["总金额"] = dec;
                r_z["申请人ID"] = CPublic.Var.LocalUserID;
                r_z["申请人"] = CPublic.Var.localUserName;
                r_z["申请时间"] = t;
                if (str_相关单位 == "入库倒冲")
                {
                    r_z["待审核人ID"] = r_z["申请人ID"];
                    r_z["待审核人"] = r_z["申请人"];
                }
                else
                {
                    r_z["待审核人ID"] = r_upper["工号"];
                    r_z["待审核人"] = r_upper["姓名"];
                }
                dt_申请.Rows.Add(r_z);
            }
            else
            {
                str_pa = dt_申请.Rows[0]["审核申请单号"].ToString();
                //decimal dec = Convert.ToDecimal(txt_cgshhje.Text);
                //dt_申请.Rows[0]["总金额"] = dec;
                dt_申请.Rows[0]["相关单位"] = str_相关单位;
                dt_申请.Rows[0]["待审核人ID"] = r_upper["工号"];
                dt_申请.Rows[0]["待审核人"] = r_upper["姓名"];
                dt_申请.Rows[0]["待审核人ID"] = r_upper["工号"];
                dt_申请.Rows[0]["待审核人"] = r_upper["姓名"];
                dt_申请.Rows[0]["申请时间"] = t;
                dt_申请.Rows[0]["申请人ID"] = CPublic.Var.LocalUserID;
                dt_申请.Rows[0]["申请人"] = CPublic.Var.localUserName;
            }

            return dt_申请;
        }
        public static DataTable fun_PA(string czlx, string doctype, string str_单号, string str_相关单位, string 申请人ID)
        {
            DataRow r_upper = ERPorg.Corg.fun_hr_upper(doctype, 申请人ID);
            string sql_id = string.Format("select 姓名,员工号 from 人事基础员工表 where 员工号 = '{0}'", 申请人ID);
            DataTable dt_员工 = CZMaster.MasterSQL.Get_DataTable(sql_id, strcon);
            if (str_相关单位 != "入库倒冲")
            {
                //DataRow r_upper = ERPorg.Corg.fun_hr_upper(doctype, CPublic.Var.LocalUserID);
                if (r_upper == null || r_upper["工号"].ToString() == "")
                {
                    throw new Exception(doctype + "审批流中未找到您相应的权限");
                }
            }

            DataTable dt_申请;
            string s = string.Format("select * from  单据审核申请表 where 关联单号='{0}' and 审核 = 0 and 作废 = 0", str_单号);
            dt_申请 = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn);
            DateTime t = CPublic.Var.getDatetime();
            string str_pa = "";
            if (dt_申请.Rows.Count == 0)
            {
                str_pa = string.Format("AP{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("AP", t.Year, t.Month));
                // 申请主表记录
                DataRow r_z = dt_申请.NewRow();
                r_z["审核申请单号"] = str_pa;
                r_z["关联单号"] = str_单号;
                r_z["相关单位"] = str_相关单位;
                r_z["操作类型"] = czlx;
                r_z["单据类型"] = doctype;
                //decimal dec = Convert.ToDecimal(txt_cgshhje.Text);
                //r_z["总金额"] = dec;
                if (dt_员工.Rows.Count > 0)
                {
                    r_z["申请人ID"] = dt_员工.Rows[0]["员工号"];
                    r_z["申请人"] = dt_员工.Rows[0]["姓名"];
                }

                r_z["申请时间"] = t;
                if (str_相关单位 == "入库倒冲")
                {
                    r_z["待审核人ID"] = r_z["申请人ID"];
                    r_z["待审核人"] = r_z["申请人"];
                }
                else
                {
                    r_z["待审核人ID"] = r_upper["工号"];
                    r_z["待审核人"] = r_upper["姓名"];
                }
                dt_申请.Rows.Add(r_z);
            }
            else
            {
                str_pa = dt_申请.Rows[0]["审核申请单号"].ToString();
                //decimal dec = Convert.ToDecimal(txt_cgshhje.Text);
                //dt_申请.Rows[0]["总金额"] = dec;
                dt_申请.Rows[0]["相关单位"] = str_相关单位;
                dt_申请.Rows[0]["待审核人ID"] = r_upper["工号"];
                dt_申请.Rows[0]["待审核人"] = r_upper["姓名"];
                dt_申请.Rows[0]["待审核人ID"] = r_upper["工号"];
                dt_申请.Rows[0]["待审核人"] = r_upper["姓名"];
                dt_申请.Rows[0]["申请时间"] = t;
                if (dt_员工.Rows.Count > 0)
                {
                    dt_申请.Rows[0]["申请人ID"] = dt_员工.Rows[0]["员工号"];
                    dt_申请.Rows[0]["申请人"] = dt_员工.Rows[0]["姓名"];
                }

            }

            return dt_申请;
        }

        /// <summary>
        /// 传入一个产品编码和需返回的dt 为该产品所有层级的子项,传入dt是为了外面可以循环调用,可以不停往里dt里写入
        /// </summary>
        /// <param name="dt_return"> 仅有一列 '子项编码'</param>
        /// <param name="str">产品编码 </param>
        /// dt_bom必须包含 产品编码和 子项编码两列
        /// <param name="includeItself">是否包含自身</param>
        public static DataTable billofM(DataTable dt_return, string str, bool includeItself, DataTable t_bom)
        {
            //19-2-25 
            //string s = string.Format("select  产品编码,子项编码  from 基础数据物料BOM表" );
            //DataTable t_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataTable dt = new DataTable();
            DataRow[] tr = t_bom.Select(string.Format("产品编码='{0}'", str));
            if (dt_return.Columns.Count == 0)
            {
                dt_return = new DataTable();
                dt_return.Columns.Add("子项编码", typeof(string));
            }
            dt = dt_return.Copy(); //dt每次都是new得
            if (tr.Length > 0)
            {
                foreach (DataRow dr in tr)
                {
                    DataRow trr = dt_return.NewRow();
                    trr["子项编码"] = dr["子项编码"];
                    dt_return.Rows.Add(trr);
                    DataRow trr1 = dt.NewRow();
                    trr1["子项编码"] = dr["子项编码"];
                    dt.Rows.Add(trr1);
                }


            }
            //s = string.Format("select  子项编码 from 基础数据物料BOM表 where 产品编码='{0}'", str);
            //using (SqlDataAdapter da = new SqlDataAdapter(s, CPublic.Var.strConn))
            //{
            //    da.Fill(dt_return);
            //    da.Fill(dt);
            //}
            if (includeItself) { DataRow dr = dt_return.NewRow(); dr["子项编码"] = str; dt_return.Rows.InsertAt(dr, 0); }
            DataTable dt_cp = dt.Copy();
            foreach (DataRow r in dt_cp.Rows)
            {
                //s = string.Format("select  子项编码 from 基础数据物料BOM表 where 产品编码='{0}'", r["子项编码"]);
                //DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn);

                //if (temp.Rows.Count > 0)
                //{
                //    fun_dg_billofM(dt_return, temp);
                //}
                //19-2-25
                DataRow[] rr = t_bom.Select(string.Format("产品编码='{0}'", r["子项编码"]));
                if (rr.Length > 0)
                {
                    DataTable temp = t_bom.Clone();
                    foreach (DataRow v in rr)
                    {
                        temp.ImportRow(v);
                    }
                    fun_dg_billofM(dt_return, temp, t_bom);
                }
            }
            return dt_return;
        }

        private static DataTable fun_dg_billofM(DataTable dt, DataTable dt_子, DataTable dt_bom)
        {
            if (dt_子.Rows.Count > 0)
            {
                foreach (DataRow xr in dt_子.Rows)
                {
                    if (dt.Select(string.Format("子项编码='{0}'", xr["子项编码"])).Length > 0)
                    {
                        continue;
                    }
                    else
                    {
                        dt.ImportRow(xr);
                    }
                    DataRow[] rr = dt_bom.Select(string.Format("产品编码='{0}'", xr["子项编码"]));
                    if (rr.Length > 0)
                    {
                        DataTable temp = dt_bom.Clone();
                        foreach (DataRow v in rr)
                        {
                            temp.ImportRow(v);
                        }
                        fun_dg_billofM(dt, temp, dt_bom);
                    }
                    //string s = string.Format("select  子项编码 from 基础数据物料BOM表 where 产品编码='{0}'", xr["子项编码"]);
                    //DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn);
                    //if (temp.Rows.Count > 0)
                    //{
                    //    fun_dg_billofM(dt, temp, dt_bom);
                    //}
                }
            }
            return dt;
        }



        public static DataTable billofM_带数量(DataTable dt_return, string str, bool includeItself)
        {
            //19-2-25 
            string s = string.Format("select  产品编码,子项编码,数量  from 基础数据物料BOM表");
            DataTable t_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataTable dt = new DataTable();

            s = string.Format("select  子项编码,数量 from 基础数据物料BOM表 where 产品编码='{0}'", str);
            using (SqlDataAdapter da = new SqlDataAdapter(s, CPublic.Var.strConn))
            {
                da.Fill(dt_return);
                da.Fill(dt);
            }
            if (includeItself) { DataRow dr = dt_return.NewRow(); dr["子项编码"] = str; dr["数量"] = 1; dt_return.Rows.InsertAt(dr, 0); }

            DataTable dt_cp = dt.Copy();
            foreach (DataRow r in dt_cp.Rows)
            {
                //s = string.Format("select  子项编码 from 基础数据物料BOM表 where 产品编码='{0}'", r["子项编码"]);
                //DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn);

                //if (temp.Rows.Count > 0)
                //{
                //    fun_dg_billofM(dt_return, temp);
                //}
                //19-2-25
                DataRow[] rr = t_bom.Select(string.Format("产品编码='{0}'", r["子项编码"]));
                if (rr.Length > 0)
                {
                    DataTable temp = t_bom.Clone();
                    foreach (DataRow v in rr)
                    {
                        temp.ImportRow(v);
                    }
                    fun_dg_billofM_带数量(dt_return, temp, t_bom, Convert.ToDecimal(r["数量"]));
                }
            }
            return dt_return;
        }

        private static DataTable fun_dg_billofM_带数量(DataTable dt, DataTable dt_子, DataTable dt_bom, decimal dec)
        {
            if (dt_子.Rows.Count > 0)
            {
                foreach (DataRow xr in dt_子.Rows)
                {
                    DataRow[] ff = dt.Select(string.Format("子项编码='{0}'", xr["子项编码"]));
                    if (ff.Length > 0)
                    {
                        ff[0]["数量"] = Convert.ToDecimal(ff[0]["数量"]) + Convert.ToDecimal(xr["数量"]) * dec;
                    }
                    else
                    {
                        xr["数量"] = Convert.ToDecimal(xr["数量"]) * dec;
                        dt.ImportRow(xr);
                    }
                    DataRow[] rr = dt_bom.Select(string.Format("产品编码='{0}'", xr["子项编码"]));
                    if (rr.Length > 0)
                    {
                        DataTable temp = dt_bom.Clone();
                        foreach (DataRow v in rr)
                        {

                            temp.ImportRow(v);
                        }
                        fun_dg_billofM_带数量(dt, temp, dt_bom, Convert.ToDecimal(xr["数量"]));
                    }
                    //string s = string.Format("select  子项编码 from 基础数据物料BOM表 where 产品编码='{0}'", xr["子项编码"]);
                    //DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn);
                    //if (temp.Rows.Count > 0)
                    //{
                    //    fun_dg_billofM(dt, temp, dt_bom);
                    //}
                }
            }
            return dt;
        }


        /// <summary>
        /// 父项编码,父项名称,父项规格,子项编码,子项名称,子项规格,数量
        /// </summary>
        /// <param name="str_物料号"></param>
        /// <returns></returns>
        public static DataTable get_u8bom(string str_物料号)
        {
            DataTable dtM = new DataTable();
            dtM.Columns.Add("父项编码");
            dtM.Columns.Add("父项名称");
            dtM.Columns.Add("父项规格");
            dtM.Columns.Add("子项编码");
            dtM.Columns.Add("子项名称");
            dtM.Columns.Add("子项规格");
            dtM.Columns.Add("计量单位编码");
            dtM.Columns.Add("计量单位");
            dtM.Columns.Add("仓库号");
            dtM.Columns.Add("仓库名称");
            dtM.Columns.Add("数量");

            // dtM.Columns.Add("数量");
            string s = string.Format(@"select  bas_part.PartId,bas_part.InvCode 父项编码,fx.cInvName as 父项名称,fx.cInvStd as 父项规格,[bom_parent].BomId,OpComponentId,ComponentId,a.InvCode as 子项编码
  ,zx.cInvName as 子项名称,zx.cInvStd as 子项规格,BaseQtyN/BaseQtyD 数量,zx.ccomunitcode 计量单位编码
  ,computationunit.ccomunitname as 计量单位,zx.cDefWareHouse as 仓库号,cWhName 仓库名称  from bas_part 
  inner  join [bom_parent] on [bom_parent].ParentId=bas_part.PartId 
  inner  join [bom_opcomponent] on [bom_opcomponent].BomId=[bom_parent].BomId
  inner   join bas_part a on  a.PartId=ComponentId
  inner  join  inventory  zx on zx.cInvCode=a.InvCode
  left join computationunit on computationunit.cComunitCode= zx.cComunitCode
  inner  join  inventory fx on fx.cInvCode=bas_part.InvCode 
  left  join Warehouse on Warehouse.cWhCode=zx.cDefWareHouse  where bas_part.InvCode='{0}'", str_物料号);
            DataTable dt1 = CZMaster.MasterSQL.Get_DataTable(s, strcon_U8);

            if (dt1.Rows.Count > 0)
            {
                dtM = get_u8_子项(dtM, dt1);
            }

            return dtM;
        }

        private static DataTable get_u8_子项(DataTable dtM, DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                DataRow r = dtM.NewRow();
                r["父项编码"] = dr["父项编码"];
                r["父项名称"] = dr["父项名称"];
                r["父项规格"] = dr["父项规格"];
                r["子项编码"] = dr["子项编码"];
                r["子项名称"] = dr["子项名称"];
                r["子项规格"] = dr["子项规格"];
                r["计量单位编码"] = dr["计量单位编码"];
                r["计量单位"] = dr["计量单位"];
                r["仓库号"] = dr["仓库号"];
                r["仓库名称"] = dr["仓库名称"];
                r["数量"] = dr["数量"];

                // r["父项编码"] = dr["父项编码"];
                dtM.Rows.Add(r);
                string s = string.Format(@"select  bas_part.PartId,bas_part.InvCode 父项编码,fx.cInvName as 父项名称,fx.cInvStd as 父项规格,[bom_parent].BomId,OpComponentId,ComponentId,a.InvCode as 子项编码
  ,zx.cInvName as 子项名称,zx.cInvStd as 子项规格,BaseQtyN/BaseQtyD 数量,zx.ccomunitcode 计量单位编码
  ,computationunit.ccomunitname as 计量单位,zx.cDefWareHouse as 仓库号,cWhName 仓库名称  from bas_part 
  inner  join [bom_parent] on [bom_parent].ParentId=bas_part.PartId 
  inner  join [bom_opcomponent] on [bom_opcomponent].BomId=[bom_parent].BomId
  inner   join bas_part a on  a.PartId=ComponentId
  inner  join  inventory  zx on zx.cInvCode=a.InvCode
  left join computationunit on computationunit.cComunitCode= zx.cComunitCode
  inner  join  inventory fx on fx.cInvCode=bas_part.InvCode 
  left  join Warehouse on Warehouse.cWhCode=zx.cDefWareHouse  where bas_part.InvCode='{0}'", dr["子项编码"]);
                DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon_U8);
                if (t.Rows.Count > 0)
                {
                    dtM = get_u8_子项(dtM, t);
                }
            }

            return dtM;
        }


        /// <summary>
        ///  返修审核,生成返修制令(已生效),返修工单(已生效),待领料单
        ///  row_主 为 新_返修申请主表 记录,里面有返修信息 由A->B 或 A->A
        ///  ds_back 其中dt 顺序为 制令,工单,待领料单主,待领料明细
        /// </summary>
        /// <param name="row_主">为'新_返修申请主表'记录,里面有返修信息 由A->B以及AB是什么(A可等于B);只需要目标产品编码,传入row_主时需先处理好,
        /// 包含物料编码,物料名称,规格,车间,仓库信息</param>
        /// <param name="dt_mx"> '新_返修申请子表'对应单号的记录,就是待领料信息,编码,名称,规格,车间,仓库</param>
        /// //20-6-3返回dt 对应需要更新的表名 需要赋值给 tablename
        /// <returns></returns>
        public static DataSet ReworkAuditing(DataRow row_主, DataTable dt_mx, DataTable dt_退料明细)
        {
            DataSet ds_back = new DataSet();
            DateTime t = CPublic.Var.getDatetime();
            #region 制令
            DataTable dt_制令 = new DataTable();
            dt_制令 = CZMaster.MasterSQL.Get_DataTable("select * from 生产记录生产制令表 where 1=2", CPublic.Var.strConn);
            DataRow r_制令 = dt_制令.NewRow(); //
            string ZlNumb = string.Format("PM{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                           CPublic.CNo.fun_得到最大流水号("PM", t.Year, t.Month));
            r_制令["物料名称"] = row_主["物料名称"];
            r_制令["规格型号"] = row_主["规格型号"];
            r_制令["图纸编号"] = row_主["图纸编号"];
            r_制令["GUID"] = System.Guid.NewGuid();
            r_制令["生产制令单号"] = ZlNumb;
            r_制令["物料编码"] = row_主["物料编码"];
            r_制令["生产制令类型"] = "返修制令";
            r_制令["生产车间"] = row_主["车间编号"];
            r_制令["仓库号"] = row_主["仓库号"];
            r_制令["仓库名称"] = row_主["仓库名称"];
            r_制令["已排单数量"] = r_制令["制令数量"] = row_主["数量"];
            r_制令["预完工日期"] = row_主["预完工日期"];
            r_制令["日期"] = t;
            r_制令["加急状态"] = "正常";
            r_制令["备注"] = row_主["生产备注"];

            r_制令["班组"] = row_主["班组"];
            r_制令["班组ID"] = row_主["班组编号"];

            r_制令["备注2"] = "返工退料" + row_主["申请单号"].ToString(); // 制令备注2/3中 存放 返修申请单号 作为备用,审核记录可通过 申请单号匹配字段'关联单号'得出
            r_制令["备注3"] = row_主["申请单号"].ToString();

            r_制令["生效"] = true;
            r_制令["生效人员"] = r_制令["操作人员"] = r_制令["制单人员"] = row_主["制单人员"];
            r_制令["生效人员ID"] = r_制令["操作人员ID"] = r_制令["制单人员ID"] = row_主["制单人员ID"];
            dt_制令.Rows.Add(r_制令);
            dt_制令.TableName = "生产记录生产制令表";
            ds_back.Tables.Add(dt_制令);

            #endregion
            #region 工单
            DataTable dt_工单 = new DataTable();
            dt_工单 = CZMaster.MasterSQL.Get_DataTable("select * from 生产记录生产工单表 where 1=2", CPublic.Var.strConn);
            string MoNo = string.Format("MO{0}{1:D2}{2:00}{3:0000}", Convert.ToInt32(t.Year.ToString().Substring(2, 2)), t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("MO", t.Year, t.Month, t.Day));
            DataRow r_工单 = dt_工单.NewRow(); //
            r_工单["生产工单号"] = MoNo;
            r_工单["生产工单类型"] = "返修工单";
            r_工单["加急状态"] = "正常";
            r_工单["GUID"] = System.Guid.NewGuid();
            r_工单["生产制令单号"] = ZlNumb;
            r_工单["物料编码"] = row_主["物料编码"];
            r_工单["物料名称"] = row_主["物料名称"];
            r_工单["规格型号"] = row_主["规格型号"];
            r_工单["仓库号"] = row_主["仓库号"];
            r_工单["仓库名称"] = row_主["仓库名称"];
            r_工单["生产数量"] = row_主["数量"];
            r_工单["生产车间"] = row_主["车间编号"];
            r_工单["车间名称"] = row_主["车间"];
            r_工单["备注1"] = row_主["生产备注"];
            r_工单["备注2"] = row_主["申请单号"];
            r_工单["班组"] = row_主["班组"];
            r_工单["班组ID"] = row_主["班组编号"];
            r_工单["预计完工日期"] = row_主["预完工日期"];
            r_工单["未检验数量"] = row_主["数量"];
            r_工单["生效人ID"] = r_制令["制单人员ID"] = CPublic.Var.LocalUserID;
            r_工单["生效人"] = r_制令["制单人员"] = CPublic.Var.localUserName;
            r_工单["生效日期"] = r_工单["预计开工日期"] = r_工单["制单日期"] = t;
            r_工单["生效"] = true;
            // decimal dec = 0;
            if (Convert.ToDecimal(row_主["工时定额"]) <= 0)
            {
                r_工单["工时"] = 0;
            }
            else
            {
                r_工单["工时"] = Convert.ToDecimal(row_主["数量"]) / Convert.ToDecimal(row_主["工时定额"]);
            }

            dt_工单.Rows.Add(r_工单);
            dt_工单.TableName = "生产记录生产工单表";
            ds_back.Tables.Add(dt_工单);
            #endregion
            #region 待领料主表明细表
            DataTable dt_待领主 = CZMaster.MasterSQL.Get_DataTable("select * from 生产记录生产工单待领料主表 where 1=2", CPublic.Var.strConn);
            DataRow r_待领主 = dt_待领主.NewRow();
            string str_待领料单号 = string.Format("DL{0}{1:00}{2:0000}", t.Year, t.Month, CPublic.CNo.fun_得到最大流水号("DL", t.Year, t.Month));
            r_待领主["待领料单号"] = str_待领料单号;
            r_待领主["生产工单号"] = MoNo;
            r_待领主["生产制令单号"] = ZlNumb;
            r_待领主["生产工单类型"] = "返修工单";
            r_待领主["产品编码"] = row_主["物料编码"];
            r_待领主["产品名称"] = row_主["物料名称"];
            r_待领主["生产数量"] = row_主["数量"];
            r_待领主["规格型号"] = row_主["规格型号"];
            //   r_待领主["原规格型号"] = row_主["原规格型号"]; //原规格型号 暂时保留 与规格型号相同
            //r_待领主["图纸编号"] = row_主["图纸编号"];  
            r_待领主["生产车间"] = row_主["车间编号"]; //原规格型号 暂时保留 与规格型号相同
            r_待领主["创建日期"] = t; //原规格型号 暂时保留 与规格型号相同
            r_待领主["制单人员ID"] = CPublic.Var.LocalUserID;
            r_待领主["制单人员"] = CPublic.Var.localUserName;
            r_待领主["领料类型"] = "返修领料";
            dt_待领主.Rows.Add(r_待领主);

            dt_待领主.TableName = "生产记录生产工单待领料主表";
            ds_back.Tables.Add(dt_待领主);
            DataTable dt_待领明细 = CZMaster.MasterSQL.Get_DataTable("select * from 生产记录生产工单待领料明细表 where 1=2", CPublic.Var.strConn);

            int i = 1;
            foreach (DataRow dr in dt_mx.Rows)
            {
                DataRow r_待领明细 = dt_待领明细.NewRow();

                r_待领明细["待领料单号"] = str_待领料单号;
                r_待领明细["待领料单明细号"] = str_待领料单号 + "-" + i++.ToString("00");
                r_待领明细["生产工单号"] = MoNo;
                r_待领明细["生产制令单号"] = ZlNumb;
                r_待领明细["生产工单类型"] = "返修工单";
                r_待领明细["物料编码"] = dr["物料编码"];
                r_待领明细["物料名称"] = dr["物料名称"];
                r_待领明细["规格型号"] = dr["规格型号"];
                r_待领明细["生产车间"] = row_主["车间编号"];
                r_待领明细["未领数量"] = r_待领明细["待领料总量"] = dr["数量"];
                r_待领明细["制单人员ID"] = CPublic.Var.LocalUserID;
                r_待领明细["制单人员"] = CPublic.Var.localUserName;
                r_待领明细["修改日期"] = r_待领明细["创建日期"] = t;
                r_待领明细["仓库号"] = dr["仓库号"];
                r_待领明细["仓库名称"] = dr["仓库名称"];
                dt_待领明细.Rows.Add(r_待领明细);
            }
            dt_待领明细.TableName = "生产记录生产工单待领料明细表";
            ds_back.Tables.Add(dt_待领明细);
            #endregion

            #region 工单退料申请 返工不一定需要退料，这边可能为空

            DataTable dt_退料主 = new DataTable();
            dt_退料主 = CZMaster.MasterSQL.Get_DataTable("select * from 工单退料申请表 where 1=2", CPublic.Var.strConn);
            DataTable dt_退 = CZMaster.MasterSQL.Get_DataTable("select * from 工单退料申请明细表 where 1=2", CPublic.Var.strConn);
            if (dt_退料明细.Rows.Count > 0)
            {
                DataRow r_退料主 = dt_退料主.NewRow(); //
                string tlNum = string.Format("WR{0}{1:00}{2:0000}", t.Year, t.Month, CPublic.CNo.fun_得到最大流水号("WR", t.Year, t.Month));
                r_退料主["待退料号"] = tlNum;
                r_退料主["生产工单号"] = MoNo;
                r_退料主["车间"] = row_主["车间"];
                r_退料主["产品编号"] = row_主["物料编码"];
                r_退料主["产品名称"] = row_主["物料名称"];
                r_退料主["操作人"] = CPublic.Var.localUserName;
                r_退料主["操作时间"] = t;
                r_退料主["退料类型"] = "返工退料";
                r_退料主["备注"] = row_主["申请单号"];
                dt_退料主.Rows.Add(r_退料主);
                i = 1;
                foreach (DataRow dr in dt_退料明细.Rows)
                {

                    DataRow r_mx = dt_退.NewRow();
                    r_mx["待退料号"] = tlNum;
                    r_mx["待退料明细号"] = tlNum + "-" + i.ToString("00");
                    r_mx["POS"] = i++;
                    r_mx["物料编码"] = dr["物料编码"];
                    r_mx["物料名称"] = dr["物料名称"];
                    r_mx["仓库号"] = dr["仓库号"];
                    r_mx["仓库名称"] = dr["仓库名称"];
                    r_mx["需退料数量"] = dr["数量"];
                    dt_退.Rows.Add(r_mx);

                }

            }
            dt_退料主.TableName = "工单退料申请表";
            ds_back.Tables.Add(dt_退料主);  //没有退料清单 也添进ds_back 返回审核后更新没问题
            dt_退.TableName = "工单退料申请明细表";
            ds_back.Tables.Add(dt_退);
            #endregion
            return ds_back;
        }


        /// <summary>
        /// 通过 npoi 将excel文件内容读取到DataTable数据表中
        /// </summary>
        /// <param name="fileName">文件完整路径名</param>
        /// <param name="sheetName">指定读取excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名：true=是，false=否</param>
        /// <returns>DataTable数据表</returns>
        public static DataTable ReadExcelToDataTable(string fileName, string sheetName = null, bool isFirstRowColumn = true)
        {
            //定义要返回的datatable对象
            DataTable data = new DataTable();
            //excel工作表
            NPOI.SS.UserModel.ISheet sheet = null;
            //数据开始行(排除标题行)
            int startRow = 0;
            try
            {
                if (!File.Exists(fileName))
                {
                    return null;
                }
                //根据指定路径读取文件
                //  FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                FileStream fs = System.IO.File.OpenRead(fileName);

                //根据文件流创建excel数据结构
                NPOI.SS.UserModel.IWorkbook workbook = NPOI.SS.UserModel.WorkbookFactory.Create(fs);
                //NPOI.SS.UserModel.IWorkbook workbook = null;
                //try
                //{
                //    workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(fs);

                //}
                //catch
                //{
                //    fs.Close();
                //    fs.Dispose();
                //    workbook = new NPOI.HSSF.UserModel.HSSFWorkbook(fs);
                //}
                //IWorkbook workbook = new HSSFWorkbook(fs);
                //如果有指定工作表名称
                if (!string.IsNullOrEmpty(sheetName))
                {
                    sheet = workbook.GetSheet(sheetName);
                    //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    //如果没有指定的sheetName，则尝试获取第一个sheet
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    NPOI.SS.UserModel.IRow firstRow = sheet.GetRow(0);
                    NPOI.SS.UserModel.IRow secondRow = sheet.GetRow(1);

                    //一行最后一个cell的编号 即总的列数
                    int cellCount = firstRow.LastCellNum;
                    //如果第一行是标题列名
                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            NPOI.SS.UserModel.ICell cell = firstRow.GetCell(i);
                            NPOI.SS.UserModel.ICell cell1 = secondRow.GetCell(i);

                            if (cell != null)
                            {
                                if (cell1 != null)
                                {

                                    CellType c = cell1.CellType;

                                    if (cell1.CellType == CellType.Numeric)
                                    {

                                        if (HSSFDateUtil.IsCellDateFormatted(cell1))//日期类型
                                        {
                                            string cellValue = cell.StringCellValue;

                                            if (cellValue != null)
                                            {
                                                DataColumn column = new DataColumn(cellValue, typeof(DateTime));
                                                data.Columns.Add(column);
                                            }
                                        }
                                        else
                                        {

                                            string cellValue = cell.StringCellValue;

                                            if (cellValue != null)
                                            {
                                                DataColumn column = new DataColumn(cellValue, typeof(decimal));
                                                data.Columns.Add(column);
                                            }
                                        }

                                    }
                                    else
                                    {
                                        string cellValue = cell.StringCellValue;

                                        if (cellValue != null)
                                        {
                                            DataColumn column = new DataColumn(cellValue);
                                            data.Columns.Add(column);
                                        }
                                    }
                                }
                                else
                                {
                                    string cellValue = cell.StringCellValue;

                                    if (cellValue != null)
                                    {
                                        DataColumn column = new DataColumn(cellValue);
                                        data.Columns.Add(column);
                                    }

                                }

                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }
                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;

                    for (int i = startRow; i <= rowCount; ++i)
                    {


                        NPOI.SS.UserModel.IRow row = sheet.GetRow(i);
                        if (row == null)
                        {
                            continue; //没有数据的行默认是null　　　　　　　
                        }

                        if (row.FirstCellNum < 0)
                        {
                            continue;
                        }

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                            {
                                // object s = row.GetCell(j);
                                if (row.GetCell(j).CellType == CellType.Numeric)
                                {
                                    if (HSSFDateUtil.IsCellDateFormatted(row.GetCell(j)))//日期类型
                                    {

                                        dataRow[j] = row.GetCell(j).DateCellValue;
                                    }
                                    else
                                    {
                                        dataRow[j] = row.GetCell(j).NumericCellValue;

                                    }
                                }
                                else if (row.GetCell(j).CellType == CellType.Blank)
                                {
                                    continue;

                                }
                                else
                                {

                                    dataRow[j] = row.GetCell(j).ToString();
                                }
                            }


                        }
                        data.Rows.Add(dataRow);
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Datable导出成Excel
        /// 传入多个dt 同一个excel,不同sheet
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="file">导出路径(包括文件名与扩展名)</param>
        public static void TableToExcel(DataSet ds, string file)
        {
            NPOI.SS.UserModel.IWorkbook workbook;
            string fileExt = Path.GetExtension(file).ToLower();
            if (fileExt == ".xlsx") { workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(); } else { workbook = null; }
            if (workbook == null) { return; }

            int x = 1;
            foreach (DataTable dt in ds.Tables)
            {
                NPOI.SS.UserModel.ISheet sheet = string.IsNullOrEmpty(dt.TableName) ? workbook.CreateSheet($"Sheet{x}") : workbook.CreateSheet(dt.TableName);

                NPOI.SS.UserModel.IRow header = sheet.CreateRow(0);


                //列名 
                NPOI.SS.UserModel.IRow row = sheet.CreateRow(0);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    NPOI.SS.UserModel.ICell cell = row.CreateCell(i);
                    cell.SetCellValue(dt.Columns[i].ColumnName);
                }

                //数据  
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow row1 = sheet.CreateRow(i + 1);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        System.Type ctp = dt.Rows[i][j].GetType();
                        if (ctp.Name == "DateTime")
                        {
                            NPOI.SS.UserModel.ICell cell = row1.CreateCell(j);

                            cell.SetCellValue(Convert.ToDateTime(dt.Rows[i][j]).ToString("yyyy-MM-dd HH:mm:ss"));
                            //cell.SetCellFormula("yyyy-MM-dd HH:mm:ss");
                        }
                        else if (ctp.Name == "Decimal" || ctp.Name == "Int" || ctp.Name == "Double")
                        {

                            NPOI.SS.UserModel.ICell cell = row1.CreateCell(j);

                            cell.SetCellValue(Convert.ToDouble(dt.Rows[i][j]));
                        }
                        else
                        {
                            NPOI.SS.UserModel.ICell cell = row1.CreateCell(j);

                            cell.SetCellValue(dt.Rows[i][j].ToString());
                        }
                    }


                }

                //自适应列宽
                for (int columnNum = 0; columnNum <= dt.Columns.Count; columnNum++)
                {

                    int columnWidth = sheet.GetColumnWidth(columnNum) / 256;
                    for (int rowNum = 1; rowNum <= sheet.LastRowNum; rowNum++)
                    {
                        NPOI.SS.UserModel.IRow currentRow;
                        //当前行未被使用过
                        if (sheet.GetRow(rowNum) == null)
                        {
                            currentRow = sheet.CreateRow(rowNum);
                        }
                        else
                        {
                            currentRow = sheet.GetRow(rowNum);
                        }

                        if (currentRow.GetCell(columnNum) != null)
                        {
                            NPOI.SS.UserModel.ICell currentCell = currentRow.GetCell(columnNum);
                            int length = System.Text.Encoding.Default.GetBytes(currentCell.ToString()).Length;
                            if (columnWidth < length)
                            {
                                columnWidth = length;
                            }
                        }
                    }
                    try
                    {
                        sheet.SetColumnWidth(columnNum, columnWidth * 256);
                    }
                    catch
                    {
                        sheet.SetColumnWidth(columnNum, 20000);
                    }



                }
                x++;
            }
            //转为字节数组  
            MemoryStream stream = new MemoryStream();
            workbook.Write(stream);
            var buf = stream.ToArray();

            //保存为Excel文件  
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }
        }



        /// <summary>
        /// Datable导出成Excel
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="file">导出路径(包括文件名与扩展名)</param>
        public static void TableToExcel(DataTable dt, string file)
        {
            NPOI.SS.UserModel.IWorkbook workbook;
            string fileExt = Path.GetExtension(file).ToLower();
            if (fileExt == ".xlsx") { workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(); } else { workbook = null; }
            if (workbook == null) { return; }
            NPOI.SS.UserModel.ISheet sheet = string.IsNullOrEmpty(dt.TableName) ? workbook.CreateSheet("Sheet1") : workbook.CreateSheet(dt.TableName);

            NPOI.SS.UserModel.IRow header = sheet.CreateRow(0);


            //列名 
            NPOI.SS.UserModel.IRow row = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                NPOI.SS.UserModel.ICell cell = row.CreateCell(i);
                cell.SetCellValue(dt.Columns[i].ColumnName);
            }

            //数据  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow row1 = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    System.Type ctp = dt.Rows[i][j].GetType();
                    if (ctp.Name == "DateTime")
                    {
                        NPOI.SS.UserModel.ICell cell = row1.CreateCell(j);

                        cell.SetCellValue(Convert.ToDateTime(dt.Rows[i][j]).ToString("yyyy-MM-dd HH:mm:ss"));
                        //cell.SetCellFormula("yyyy-MM-dd HH:mm:ss");
                    }
                    else if (ctp.Name == "Decimal" || ctp.Name == "Int" || ctp.Name == "Double")
                    {

                        NPOI.SS.UserModel.ICell cell = row1.CreateCell(j);

                        cell.SetCellValue(Convert.ToDouble(dt.Rows[i][j]));
                    }
                    else
                    {
                        NPOI.SS.UserModel.ICell cell = row1.CreateCell(j);

                        cell.SetCellValue(dt.Rows[i][j].ToString());
                    }
                }


            }

            //自适应列宽
            for (int columnNum = 0; columnNum <= dt.Columns.Count; columnNum++)
            {

                int columnWidth = sheet.GetColumnWidth(columnNum) / 256;
                for (int rowNum = 1; rowNum <= sheet.LastRowNum; rowNum++)
                {
                    NPOI.SS.UserModel.IRow currentRow;
                    //当前行未被使用过
                    if (sheet.GetRow(rowNum) == null)
                    {
                        currentRow = sheet.CreateRow(rowNum);
                    }
                    else
                    {
                        currentRow = sheet.GetRow(rowNum);
                    }

                    if (currentRow.GetCell(columnNum) != null)
                    {
                        NPOI.SS.UserModel.ICell currentCell = currentRow.GetCell(columnNum);
                        int length = System.Text.Encoding.Default.GetBytes(currentCell.ToString()).Length;
                        if (columnWidth < length)
                        {
                            columnWidth = length;
                        }
                    }
                }
                try
                {
                    sheet.SetColumnWidth(columnNum, columnWidth * 256);
                }
                catch
                {
                    sheet.SetColumnWidth(columnNum, 20000);
                }



            }

            //转为字节数组  
            MemoryStream stream = new MemoryStream();
            workbook.Write(stream);
            var buf = stream.ToArray();

            //保存为Excel文件  
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }
        }



        /// <summary>
        /// 查找传进来的物料 所有父项
        /// </summary>
        /// <param name="dt_存"></param>
        /// <param name="a_物料"></param>
        /// <param name="dec"></param>
        /// <param name="isIncludeSelf"> 是否包含传进来的物料</param>
        /// <returns></returns>
        public static DataTable fun_GetFather(DataTable dt_存, string a_物料, decimal dec, bool isIncludeSelf)
        {
            //19-2-25
            string s = string.Format("select  产品编码,子项编码  from 基础数据物料BOM表");
            DataTable t_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (dt_存 == null || dt_存.Columns.Count == 0)
            {
                dt_存.Columns.Add("产品编码");
            }
            if (isIncludeSelf)
            {
                DataRow r_self = dt_存.NewRow();
                r_self["产品编码"] = a_物料;
                dt_存.Rows.Add(r_self);
            }
            //19-2-25
            DataTable dt1 = new DataTable();
            dt1 = t_bom.Clone();
            DataRow[] rr = t_bom.Select(string.Format("子项编码='{0}'", a_物料));
            if (rr.Length > 0)
            {
                foreach (DataRow r in rr)
                {
                    dt1.ImportRow(r);
                }
                getFatherDg(dt_存, dt1, t_bom);
            }
            ////  dt1 = fun_GetDataByChildNo(a_物料,t_bom);
            //foreach (DataRow dr in dt1.Rows)
            //{
            //    DataTable dt2 = fun_GetDataByChildNo(dr["产品编码"].ToString(), t_bom);
            //    DataRow r = dt_存.NewRow();
            //    r["产品编码"] = dr["产品编码"].ToString();

            //    dt_存.Rows.Add(r);
            //    if (dt2.Rows.Count > 0)
            //    {
            //        fun_GetFather(dt_存, dr["产品编码"].ToString(), dec, false);
            //    }
            //}
            return dt_存;
        }
        private static DataTable getFatherDg(DataTable dt, DataTable dt_z, DataTable dt_bom)
        {

            if (dt_z.Rows.Count > 0)
            {
                foreach (DataRow xr in dt_z.Rows)
                {
                    if (dt.Select(string.Format("产品编码='{0}'", xr["产品编码"])).Length > 0)
                    {
                        continue;
                    }
                    else
                    {
                        DataRow vr = dt.NewRow();
                        vr["产品编码"] = xr["产品编码"];
                        dt.Rows.Add(vr);
                    }


                    DataRow[] rr = dt_bom.Select(string.Format("子项编码='{0}'", xr["产品编码"]));
                    if (rr.Length > 0)
                    {
                        DataTable temp = dt_bom.Clone();
                        foreach (DataRow v in rr)
                        {
                            temp.ImportRow(v);
                        }
                        getFatherDg(dt, temp, dt_bom);
                    }
                }
            }
            return dt;
        }
        //private static DataTable fun_GetDataByChildNo(string a_物料,DataTable dt_bom)
        //{
        //    DataTable dt_物料1 = new DataTable();
        //    dt_物料1.Columns.Add("产品编码", typeof(string));
        //    DataRow[] rr = dt_bom.Select(string.Format("子项编码='{0}'", a_物料));
        //    if (rr.Length > 0)
        //    {
        //        foreach (DataRow r in rr)
        //        {
        //            DataRow xx = dt_物料1.NewRow();
        //            xx["产品编码"] = r["产品编码"];
        //            dt_物料1.Rows.Add(xx);
        //        }
        //    }
        //    //using (SqlDataAdapter da = new SqlDataAdapter("select 产品编码 from 基础数据物料BOM表 where 子项编码='" + a_物料 + "'", CPublic.Var.strConn))
        //    //{
        //    //    da.Fill(dt_物料1);
        //    //}
        //    return dt_物料1;
        //}

        //
        public static string DataTable2Xml(DataTable dt)
        {
            if (null == dt)
            {
                return string.Empty;
            }

            StringWriter writer = new StringWriter();
            dt.WriteXml(writer);
            string xmlstr = writer.ToString();
            writer.Close();
            return xmlstr;
        }
        /// <summary>
        /// 将XML生成DataTable
        /// </summary>
        /// <param name="xmlStr">XML字符串</param>
        /// <returns></returns>
        public static DataTable XmlToDataTable(string xmlStr)
        {
            if (!string.IsNullOrEmpty(xmlStr))
            {
                StringReader StrStream = null;
                XmlTextReader Xmlrdr = null;
                try
                {
                    DataSet ds = new DataSet();
                    //读取字符串中的信息
                    StrStream = new StringReader(xmlStr);
                    //获取StrStream中的数据
                    Xmlrdr = new XmlTextReader(StrStream);
                    //ds获取Xmlrdr中的数据               
                    ds.ReadXml(Xmlrdr);
                    return ds.Tables[0];
                }
                catch (Exception)
                {
                    return null;
                }
                finally
                {
                    //释放资源
                    if (Xmlrdr != null)
                    {
                        Xmlrdr.Close();
                        StrStream.Close();
                        StrStream.Dispose();
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取 插入日期是当年第几周
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="ci"></param>
        /// <returns></returns>  
        public static int WeekOfYear(DateTime time, CultureInfo ci)
        { return ci.Calendar.GetWeekOfYear(time, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek); }



        /// <summary>
        /// 检验文件是否被占用
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsFileInUse(string fileName)
        {
            bool inUse = true;

            FileStream fs = null;
            try
            {

                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read,

                FileShare.None);

                inUse = false;
            }
            catch
            {

            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
            return inUse;//true表示正在使用,false没有使用
        }


        /// <summary>
        ///  excel转换成图片
        /// </summary>
        /// <param name="filename"></param>
        public static void ChangeExcel2Image(string filename)
        {


            Workbook workbook = new Workbook();

            workbook.LoadFromFile(filename);
            Worksheet sheet = workbook.Worksheets[0];
            //   PrintDocument pd = workbook.PrintDocument;

            //   pd.PrinterSettings.Copies = Convert.ToInt16(txtPrintNum.Text.Trim());
            //for (int num = 0; num < Convert.ToInt32(txtPrintNum.Text.Trim());num++ )
            //{
            //   pd.Print();

            int x = sheet.LastColumn;
            int y = sheet.LastRow;
            sheet.ToImage(1, 1, y - 4, x).Save("image1.png", ImageFormat.Png);



        }
        /// <summary>
        /// 给Excel添加边框
        /// </summary>
        public static ICellStyle SetCellStyle(XSSFWorkbook hssfworkbook, IFont font, NPOI.SS.UserModel.HorizontalAlignment ha)
        {
            ICellStyle cellstyle = hssfworkbook.CreateCellStyle();
            cellstyle.Alignment = ha;
            if (font != null)
            {
                cellstyle.SetFont(font);
            }
            //有边框
            cellstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cellstyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cellstyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            cellstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            return cellstyle;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="abc">文件路径 </param>
        /// <returns></returns>
        public static string fun(string abc)
        {
            string newfs = "";
            string newsavefilepath = "";
            XSSFWorkbook singlexssfwk;
            //注意，不同的NPOI版本调用的方法不一致，这里使用的版本是2.1.3.1

            //获取模板excel的路径
            string str = System.Environment.CurrentDirectory + "\\盘点表.xlsx";
            if (File.Exists(str))
            {
                using (FileStream fs = new FileStream(str, FileMode.Open, FileAccess.Read))
                {
                    singlexssfwk = new XSSFWorkbook(fs);

                    fs.Close();
                }
                //获取表
                XSSFSheet xssfsheet = (XSSFSheet)singlexssfwk.GetSheetAt(0);

                DataTable dt = ERPorg.Corg.Read盘点ToDataTable(abc);
                //创建行 
                IRow row = xssfsheet.CreateRow(1);

                ICell cell1 = row.CreateCell(0, CellType.String);
                cell1.SetCellValue("仓库");
                ICell cell2 = row.CreateCell(1, CellType.String);
                cell2.SetCellValue("货架描述");
                ICell cell3 = row.CreateCell(2, CellType.String);
                cell3.SetCellValue("物料编号");

                ICell cell4 = row.CreateCell(3, CellType.String);
                cell4.SetCellValue("物料名称");
                ICell cell5 = row.CreateCell(4, CellType.String);
                cell5.SetCellValue("盘前库存");
                ICell cell6 = row.CreateCell(5, CellType.String);
                cell6.SetCellValue("盘后库存");
                ICell cell7 = row.CreateCell(6, CellType.String);
                cell7.SetCellValue("偏差值");

                int row1 = 1;
                ICellStyle style1 = SetCellStyle(singlexssfwk, null, NPOI.SS.UserModel.HorizontalAlignment.Center);

                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    DataRow drr = dt.Rows[i];
                    XSSFRow xssfrow1 = (XSSFRow)xssfsheet.GetRow(i + 1);

                    row = xssfsheet.CreateRow(i + 2);


                    cell1 = row.CreateCell(0, CellType.String);
                    cell1.SetCellValue(drr["仓库"].ToString());
                    row.GetCell(0).CellStyle = style1;
                    cell2 = row.CreateCell(1, CellType.String);
                    cell2.SetCellValue(drr["货架描述"].ToString());
                    row.GetCell(1).CellStyle = style1;
                    cell3 = row.CreateCell(2, CellType.String);
                    cell3.SetCellValue(drr["物料编号"].ToString());
                    row.GetCell(2).CellStyle = style1;
                    cell4 = row.CreateCell(3, CellType.String);
                    cell4.SetCellValue(drr["物料名称"].ToString());
                    row.GetCell(3).CellStyle = style1;


                    cell5 = row.CreateCell(4, CellType.String);
                    cell5.SetCellValue(drr["盘前库存"].ToString());
                    row.GetCell(4).CellStyle = style1;

                    cell6 = row.CreateCell(5, CellType.String);
                    cell6.SetCellValue(drr["盘后库存"].ToString());
                    row.GetCell(5).CellStyle = style1;

                    cell7 = row.CreateCell(6, CellType.String);
                    cell7.SetCellValue(drr["偏差值"].ToString());
                    row.GetCell(6).CellStyle = style1;
                    //设置Excel行的样式（带边框）
                    //  row.GetCell(i).CellStyle = style1;
                    row1++;
                }


                row = xssfsheet.CreateRow(row1 + 1);
                cell1 = row.CreateCell(0, CellType.String);
                cell1.SetCellValue("盘点时间：");
                //row.GetCell(0).CellStyle = style1;
                cell2 = row.CreateCell(1, CellType.String);
                cell2.SetCellValue("阿斯达克就是电话");
                cell3 = row.CreateCell(2, CellType.String);
                cell3.SetCellValue("x");
                //row.GetCell(2).CellStyle = style1;
                cell4 = row.CreateCell(3, CellType.String);
                cell4.SetCellValue("盘点人:");
                cell5 = row.CreateCell(4, CellType.String);
                cell5.SetCellValue("阿斯达克就是电话");
                cell6 = row.CreateCell(5, CellType.String);
                cell6.SetCellValue("监盘人:");
                cell6 = row.CreateCell(6, CellType.String);
                cell6.SetCellValue("x");
                //   row = xssfsheet.CreateRow(row1 +2);
                //   // 设置单元格内容               
                //    xssfrow1.GetCell(0).SetCellValue(drr["仓库"].ToString());                 
                //    xssfrow1.GetCell(1).SetCellValue(drr["货架描述"].ToString());
                //  //  xssfrow1.GetCell(2).SetCellValue(drr["物料编号"].ToString());
                //    xssfrow1.GetCell(2).SetCellValue(drr["物料名称"].ToString());
                //    xssfrow1.GetCell(3).SetCellValue(drr["盘前库存"].ToString());
                //    xssfrow1.GetCell(4).SetCellValue(drr["盘后库存"].ToString());
                ////    xssfrow1.GetCell(6).SetCellValue(drr["偏差值"].ToString());

                SaveFileDialog savedialog = new SaveFileDialog(); //弹出让用户选择excel保存路径的窗口
                savedialog.Filter = " excel files(*.xlsx)|*.xlsx|All files(*.*)|*.*";
                savedialog.RestoreDirectory = true;
                savedialog.FileName = string.Format("销售订单审批单{0}", DateTime.Now.ToString("yyyyMMddHHmm"));
                if (savedialog.ShowDialog() == DialogResult.OK)
                {
                    // newsavefilepath是excel的保存路径
                    newsavefilepath = savedialog.FileName.ToString().Trim();
                    using (FileStream newfs2 = new FileStream(newsavefilepath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        newfs = newsavefilepath.ToString();
                        singlexssfwk.Write(newfs2); //将生成的excel写入用户选择保存的文件路径中
                        newfs2.Close();

                    }
                }

            }
            return newfs;

        }


        /// <summary>
        /// 
        /// </summary>
        /// 两个dt  第一个所选条目的dt
        /// 第二个 数据dt
        /// <returns></returns>
        public static void PushDt(DataTable dt1, DataTable dt2, string file)
        {
            NPOI.SS.UserModel.IWorkbook workbook;
            string fileExt = Path.GetExtension(file).ToLower();
            if (fileExt == ".xlsx") { workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(); } else { workbook = null; }
            if (workbook == null) { return; }
            NPOI.SS.UserModel.ISheet sheet = string.IsNullOrEmpty(dt1.TableName) ? workbook.CreateSheet("Sheet1") : workbook.CreateSheet(dt1.TableName);

            NPOI.SS.UserModel.IRow header = sheet.CreateRow(0);

            int k = 1;
            //列名 
            NPOI.SS.UserModel.IRow row = sheet.CreateRow(0);
            for (int i = 0; i < dt1.Columns.Count; i++)
            {
                NPOI.SS.UserModel.ICell cell = row.CreateCell(i);
                cell.SetCellValue(dt1.Columns[i].ColumnName);
            }

            //数据  
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow row1 = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt1.Columns.Count; j++)
                {
                    NPOI.SS.UserModel.ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(dt1.Rows[i][j].ToString());
                }

                k++;

            }

            NPOI.SS.UserModel.IRow header1 = sheet.CreateRow(++k);
            NPOI.SS.UserModel.ICell cell1 = header1.CreateCell(0);
            cell1.SetCellValue("需求材料：");


            NPOI.SS.UserModel.IRow row2 = sheet.CreateRow(++k
);
            for (int i = 0; i < dt2.Columns.Count; i++)
            {
                NPOI.SS.UserModel.ICell cell = row2.CreateCell(i);
                cell.SetCellValue(dt2.Columns[i].ColumnName);
            }

            //数据  
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow row1 = sheet.CreateRow(k + 1);
                for (int j = 0; j < dt2.Columns.Count; j++)
                {
                    NPOI.SS.UserModel.ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(dt2.Rows[i][j].ToString());
                }

                k++;

            }



            //转为字节数组  
            MemoryStream stream = new MemoryStream();
            workbook.Write(stream);
            var buf = stream.ToArray();

            //保存为Excel文件  
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }

        }




        /// <summary>
        ///  盘点打印方法
        /// </summary>
        /// <param name="filename"></param>
        public static DataTable Read盘点ToDataTable(string fileName, string sheetName = null, bool isFirstRowColumn = true)
        {


            //定义要返回的datatable对象
            DataTable data = new DataTable();
            //excel工作表
            NPOI.SS.UserModel.ISheet sheet = null;
            //数据开始行(排除标题行)
            int startRow = 0;
            try
            {
                if (!File.Exists(fileName))
                {
                    return null;
                }
                //根据指定路径读取文件
                //  FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                FileStream fs = System.IO.File.OpenRead(fileName);

                //根据文件流创建excel数据结构
                NPOI.SS.UserModel.IWorkbook workbook = NPOI.SS.UserModel.WorkbookFactory.Create(fs);

                //如果有指定工作表名称
                if (!string.IsNullOrEmpty(sheetName))
                {
                    sheet = workbook.GetSheet(sheetName);
                    //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    //如果没有指定的sheetName，则尝试获取第一个sheet
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    NPOI.SS.UserModel.IRow firstRow = sheet.GetRow(0);
                    //一行最后一个cell的编号 即总的列数
                    int cellCount = firstRow.LastCellNum;
                    //如果第一行是标题列名
                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            NPOI.SS.UserModel.ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }
                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;

                    for (int i = startRow; i <= rowCount; ++i)
                    {


                        NPOI.SS.UserModel.IRow row = sheet.GetRow(i);
                        if (row == null)
                        {
                            continue; //没有数据的行默认是null　　　　　　　
                        }

                        if (row.FirstCellNum < 0)
                        {
                            continue;
                        }

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                            {
                                dataRow[j] = row.GetCell(j).ToString();
                            }
                        }
                        data.Rows.Add(dataRow);
                    }

                    DataTable dtp = data.Copy();
                    DataRow drg = dtp.Rows[0];
                    data.Columns.Remove("盘点时间");
                    data.Columns.Remove("盘点人");
                    //for(int i=0; i<=data.Rows.Count+1;i++){
                    //    if(i==data.Rows.Count+1){

                    //DataRow dgg = data.NewRow();
                    //data.Rows.Add(dgg);
                    //dgg["仓库"] = "盘点时间：";
                    //dgg["货架描述"] = drg["盘点时间"].ToString();
                    //dgg["盘前库存"] = "盘点人：";
                    //dgg["盘后库存"] = drg["盘点人"].ToString();
                    //dgg["偏差值"] = "监盘人：";


                    //sheet.GetRow(i).GetCell(1).SetCellValue("盘点时间");
                    //sheet.GetRow(i).GetCell(2).SetCellValue(drg["盘点时间"].ToString());
                    //     sheet.GetRow(i).GetCell(5).SetCellValue("盘点人");
                    //sheet.GetRow(i).GetCell(6).SetCellValue(drg["盘点人"].ToString());
                    // sheet.GetRow(i).GetCell(7).SetCellValue(drg["监盘人"].ToString());
                    //}
                    //}
                    //foreach( DataRow dr in data.Rows ){
                    //    if(dr ){
                    //    }
                    // }
                }




                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 采购池计算 19-1-17 郭恒
        /// <summary>
        ///加载未完成销售明细,未完成采购明细,未完成工单,bom 和 库存
        ///bl =true 表示 采购计划池 反之 生产计划池
        ///2020-6-2 材料统算添加有订单用量但不缺料的
        /// </summary>
        #endregion
        public static result fun_pool_1(DateTime t1, DateTime t2, bool bl)
        {
            //19-5-27 
            string x = "exec FourNum";
            CZMaster.MasterSQL.ExecuteSQL(x, strcon);
            //     //销售未完成 物料 数量明细  
            //     string s = string.Format(@" select  smx.客户,销售订单明细号,smx.销售订单号,zb.目标客户,smx.客户 as 客户名称,zb.创建日期 as 下单日期,base.物料编码,base.物料名称,base.规格型号,smx.数量,未完成数量,smx.数量 as 销售数量,库存总数,未领量,在制量,smx.备注,在途量 
            //,送达日期 as 预计发货日期,存货分类  from 销售记录销售订单明细表 smx
            //left  join 销售记录销售订单主表  zb on zb.销售订单号=smx.销售订单号 
            //left join 基础数据物料信息表 base  on base.物料编码=smx.物料编码
            //left join 仓库物料数量表 kc on kc.物料编码=smx.物料编码 and  smx.仓库号=kc.仓库号       
            // where smx.生效=1 and smx.送达日期>='{0}' and smx.送达日期<'{1}'and 明细完成=0 and smx.关闭=0 and smx.作废=0 and left(存货分类编码,2)<>'20'  and left(存货分类编码,2)<>'11'", t1,t2);
            //     DataTable dt_SaleOrder_mx = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            string s = string.Format(@"  select  x.*,case when zlzb.销售订单明细号 is null then 0 else 1 end as 已关联 from [V_CalPoolTotal] x
          left join  (select 销售订单明细号 from 生产记录生产制令子表 group by 销售订单明细号) zlzb on zlzb.销售订单明细号 = x.销售订单明细号
                  where [预计发货日期]>'{0}' and 预计发货日期<'{1}'  ", t1, t2);
            DataTable dt_SaleOrder_mx = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //汇总 
            //s = string.Format(@"select  smx.物料编码,sum(未完成数量)数量,MIN(送达日期) as 最早发货日期 from 销售记录销售订单明细表 smx
            //left join 基础数据物料信息表 base  on base.物料编码=smx.物料编码
            //where smx.生效=1  and smx.送达日期>='{0}' and smx.送达日期<'{1}' and 明细完成=0 and smx.关闭=0 and 作废=0 and left(存货分类编码,2)<>'20' and left(存货分类编码,2)<>'11' group by smx.物料编码", t1, t2);
            //          
            s = string.Format("select 物料编码,sum(未完成数量)数量,MIN(预计发货日期) as 最早发货日期 from [V_CalPoolTotal] where [预计发货日期]>'{0}' and 预计发货日期<'{1}'    group by  物料编码", t1, t2);
            DataTable dt_SaleOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            //未完成工单 物料 数量 汇总   此处可能要加入生效日期限制,有部分数据是同步的用友数据 
            //         s = @" select  x.*,kc.仓库名称,库存总数,未领量,base.物料编码,base.物料名称,base.规格型号,base.存货分类 from (
            //              select  gd.生产工单号,物料编码,生产数量,sum(生产数量)-isnull(SUM(a.已入库数量),0) as 数量,仓库号  from 生产记录生产工单表  gd 
            //               left join (select  生产工单号,SUM(入库数量) as 已入库数量 from  生产记录成品入库单明细表  where 作废=0 group by 生产工单号 )a 
            //          on a.生产工单号=gd.生产工单号 
            //where gd.生效=1 and 完成=0 and gd.关闭=0 and 作废=0 group by 物料编码,生产数量,gd.生产工单号,仓库号)x
            //  left join 基础数据物料信息表 base on base.物料编码=x.物料编码 
            //  left join 仓库物料数量表  kc on kc.物料编码=base.物料编码 and x.仓库号=kc.仓库号
            //  order by x.物料编码";
            //            DataTable IncompleteWorkOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //            //未完成采购单 
            //            s=@"select  物料编码,SUM(未完成数量) as 数量 from 采购记录采购单明细表  where 生效=1 and 未完成数量>0 and 作废=0 group by 物料编码"
            //            DataTable IncompletePO = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = @" select  采购明细号,base.物料名称,base.物料编码,采购数量,未完成数量,mx.仓库号,mx.仓库名称,到货日期,存货分类,库存总数,在途量,未领量,在制量,受订量 from 采购记录采购单明细表 mx
            left join 采购记录采购单主表 zb  on zb.采购单号 =mx.采购单号
            left join 基础数据物料信息表 base on base.物料编码=mx.物料编码
            left join 仓库物料数量表  kc  on kc.物料编码=mx.物料编码 and kc.仓库号=mx.仓库号
            where mx.生效=1 and 未完成数量>0 and mx.作废=0 and zb.作废=0 and 明细完成日期 is null";
            DataTable IncompletePO = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            //20-4-13 替代料屏蔽
            //2020-6-24 增加字段,优先级、ECN     //  去除 where 优先级=1 
            s = @"select  产品编码,产品名称,fx.存货分类 as 父项分类,fx.规格型号 as 父项规格,子项编码,子项名称,数量,zx.委外 as 子项委外,zx.自制 as 子项自制,zx.可购 as 子项可购,zx.存货分类 as 子项分类
                ,zx.规格型号 as 子项规格,wiptype,zx.ECN,优先级     from 基础数据物料BOM表 bom 
               left join 基础数据物料信息表 zx on bom.子项编码=zx.物料编码 
               left join 基础数据物料信息表 fx on bom.产品编码=fx.物料编码  "; 
            DataTable dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn[] pk_bom = new DataColumn[2];
            pk_bom[0] = dt_bom.Columns["产品编码"];
            pk_bom[1] = dt_bom.Columns["子项编码"];
            dt_bom.PrimaryKey = pk_bom;
            //取库存,总数=库存-未领+在制+在途  不减受订 是为了下面 根据 这个来计算，下面会有算一遍 总数-受订量
            s = "select * from V_pooltotal ";
            DataTable dt_totalcount = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn[] pk = new DataColumn[1];
            pk[0] = dt_totalcount.Columns["物料编码"];
            dt_totalcount.PrimaryKey = pk;
            DataTable dtM = new DataTable();
            dtM.Columns.Add("未领量", typeof(decimal));
            dtM.Columns.Add("在途量", typeof(decimal));
            dtM.Columns.Add("在制量", typeof(decimal));
            dtM.Columns.Add("仓库号");
            dtM.Columns.Add("工时", typeof(decimal));
            dtM.Columns.Add("仓库名称");
            dtM.Columns.Add("物料编码");
            dtM.Columns.Add("物料名称");
            dtM.Columns.Add("规格型号");
            dtM.Columns.Add("库存总数", typeof(decimal));
            dtM.Columns.Add("最早发货日期", typeof(DateTime));
            dtM.Columns.Add("存货分类");
            dtM.Columns.Add("参考数量", typeof(decimal));
            dtM.Columns.Add("受订量", typeof(decimal));
            dtM.Columns.Add("自制", typeof(bool));
            dtM.Columns.Add("需求数量", typeof(decimal));
            dtM.Columns.Add("已转制令数", typeof(decimal));
            dtM.Columns.Add("已转工单数", typeof(decimal));
            dtM.Columns.Add("拼板数量", typeof(decimal));
            dtM.Columns.Add("采购周期");
            dtM.Columns.Add("最小包装", typeof(decimal));
            dtM.Columns.Add("订单用量", typeof(decimal));
            dtM.Columns.Add("停用", typeof(bool));
            dtM.Columns.Add("班组编号");
            dtM.Columns.Add("班组名称");
            //20-6-3
            dtM.Columns.Add("计划在途", typeof(decimal));




            result ss = new result();
            ss.salelist_mx = dt_SaleOrder_mx;
            ss.salelist = dt_SaleOrder;
            ss.Polist_mx = IncompletePO;
            ss.Bom = dt_bom;
            ss.TotalCount = dt_totalcount;
            ss.str_log = "";
            ss.dtM = dtM;

            ss = calu1(ss, bl);

            return ss;
        }



        #region 采购池计算 19-1-17 郭恒
        /// <summary>
        ///加载未完成销售明细,未完成采购明细,未完成工单,bom 和 库存
        ///bl =true 表示 采购计划池 反之 生产计划池
        /// </summary>
        public static result fun_pool(DateTime t1, DateTime t2, bool bl)
        {
            //19-5-27 
            string x = "exec FourNum";
            CZMaster.MasterSQL.ExecuteSQL(x, strcon);
            //     //销售未完成 物料 数量明细  
            //     string s = string.Format(@" select  smx.客户,销售订单明细号,smx.销售订单号,zb.目标客户,smx.客户 as 客户名称,zb.创建日期 as 下单日期,base.物料编码,base.物料名称,base.规格型号,smx.数量,未完成数量,smx.数量 as 销售数量,库存总数,未领量,在制量,smx.备注,在途量 
            //,送达日期 as 预计发货日期,存货分类  from 销售记录销售订单明细表 smx
            //left  join 销售记录销售订单主表  zb on zb.销售订单号=smx.销售订单号 
            //left join 基础数据物料信息表 base  on base.物料编码=smx.物料编码
            //left join 仓库物料数量表 kc on kc.物料编码=smx.物料编码 and  smx.仓库号=kc.仓库号       
            // where smx.生效=1 and smx.送达日期>='{0}' and smx.送达日期<'{1}'and 明细完成=0 and smx.关闭=0 and smx.作废=0 and left(存货分类编码,2)<>'20'  and left(存货分类编码,2)<>'11'", t1,t2);
            //     DataTable dt_SaleOrder_mx = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            string s = string.Format(@"  select  x.*,case when zlzb.销售订单明细号 is null then 0 else 1 end as 已关联 from [V_CalPoolTotal] x
          left join  (select 销售订单明细号 from 生产记录生产制令子表 group by 销售订单明细号) zlzb on zlzb.销售订单明细号 = x.销售订单明细号
                  where [预计发货日期]>'{0}' and 预计发货日期<'{1}'  ", t1, t2);
            DataTable dt_SaleOrder_mx = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //汇总 
            //s = string.Format(@"select  smx.物料编码,sum(未完成数量)数量,MIN(送达日期) as 最早发货日期 from 销售记录销售订单明细表 smx
            //left join 基础数据物料信息表 base  on base.物料编码=smx.物料编码
            //where smx.生效=1  and smx.送达日期>='{0}' and smx.送达日期<'{1}' and 明细完成=0 and smx.关闭=0 and 作废=0 and left(存货分类编码,2)<>'20' and left(存货分类编码,2)<>'11' group by smx.物料编码", t1, t2);
            //          
            s = string.Format("select 物料编码,sum(未完成数量)数量,MIN(预计发货日期) as 最早发货日期 from [V_CalPoolTotal] where [预计发货日期]>'{0}' and 预计发货日期<'{1}'    group by  物料编码", t1, t2);
            DataTable dt_SaleOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            //未完成工单 物料 数量 汇总   此处可能要加入生效日期限制,有部分数据是同步的用友数据 
            //         s = @" select  x.*,kc.仓库名称,库存总数,未领量,base.物料编码,base.物料名称,base.规格型号,base.存货分类 from (
            //              select  gd.生产工单号,物料编码,生产数量,sum(生产数量)-isnull(SUM(a.已入库数量),0) as 数量,仓库号  from 生产记录生产工单表  gd 
            //               left join (select  生产工单号,SUM(入库数量) as 已入库数量 from  生产记录成品入库单明细表  where 作废=0 group by 生产工单号 )a 
            //          on a.生产工单号=gd.生产工单号 
            //where gd.生效=1 and 完成=0 and gd.关闭=0 and 作废=0 group by 物料编码,生产数量,gd.生产工单号,仓库号)x
            //  left join 基础数据物料信息表 base on base.物料编码=x.物料编码 
            //  left join 仓库物料数量表  kc on kc.物料编码=base.物料编码 and x.仓库号=kc.仓库号
            //  order by x.物料编码";
            //            DataTable IncompleteWorkOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //            //未完成采购单 
            //            s=@"select  物料编码,SUM(未完成数量) as 数量 from 采购记录采购单明细表  where 生效=1 and 未完成数量>0 and 作废=0 group by 物料编码"
            //            DataTable IncompletePO = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = @" select  采购明细号,base.物料名称,base.物料编码,采购数量,未完成数量,mx.仓库号,mx.仓库名称,到货日期,存货分类,库存总数,在途量,未领量,在制量,受订量 from 采购记录采购单明细表 mx
            left join 采购记录采购单主表 zb  on zb.采购单号 =mx.采购单号
            left join 基础数据物料信息表 base on base.物料编码=mx.物料编码
            left join 仓库物料数量表  kc  on kc.物料编码=mx.物料编码 and kc.仓库号=mx.仓库号
            where mx.生效=1 and 未完成数量>0 and mx.作废=0 and zb.作废=0 and 明细完成日期 is null";
            DataTable IncompletePO = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = @"select  产品编码,产品名称,fx.存货分类 as 父项分类,fx.规格型号 as 父项规格,子项编码,子项名称,数量,zx.委外 as 子项委外,zx.自制 as 子项自制,zx.可购 as 子项可购,zx.存货分类 as 子项分类
                ,zx.规格型号 as 子项规格,wiptype       from 基础数据物料BOM表 bom 
               left join 基础数据物料信息表 zx on bom.子项编码=zx.物料编码 
               left join 基础数据物料信息表 fx on bom.产品编码=fx.物料编码  where 优先级=1 "; //20-4-13 替代料屏蔽
            DataTable dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn[] pk_bom = new DataColumn[2];
            pk_bom[0] = dt_bom.Columns["产品编码"];
            pk_bom[1] = dt_bom.Columns["子项编码"];
            dt_bom.PrimaryKey = pk_bom;
            //取库存,总数=库存-未领+在制+在途  不减受订 是为了下面 根据 这个来计算，下面会有算一遍 总数-受订量
            s = "select * from V_pooltotal ";
            DataTable dt_totalcount = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn[] pk = new DataColumn[1];
            pk[0] = dt_totalcount.Columns["物料编码"];
            dt_totalcount.PrimaryKey = pk;
            DataTable dtM = new DataTable();
            dtM.Columns.Add("未领量", typeof(decimal));
            dtM.Columns.Add("在途量", typeof(decimal));
            dtM.Columns.Add("在制量", typeof(decimal));
            dtM.Columns.Add("仓库号");
            dtM.Columns.Add("工时", typeof(decimal));
            dtM.Columns.Add("仓库名称");
            dtM.Columns.Add("物料编码");
            dtM.Columns.Add("物料名称");
            dtM.Columns.Add("规格型号");
            dtM.Columns.Add("库存总数", typeof(decimal));
            dtM.Columns.Add("最早发货日期", typeof(DateTime));
            dtM.Columns.Add("存货分类");
            dtM.Columns.Add("参考数量", typeof(decimal));
            dtM.Columns.Add("受订量", typeof(decimal));
            dtM.Columns.Add("自制", typeof(bool));
            dtM.Columns.Add("需求数量", typeof(decimal));
            dtM.Columns.Add("已转制令数", typeof(decimal));
            dtM.Columns.Add("已转工单数", typeof(decimal));
            dtM.Columns.Add("拼板数量", typeof(decimal));
            dtM.Columns.Add("采购周期");
            dtM.Columns.Add("最小包装", typeof(decimal));
            dtM.Columns.Add("订单用量", typeof(decimal));
            dtM.Columns.Add("停用", typeof(bool));
            dtM.Columns.Add("班组编号");
            dtM.Columns.Add("班组名称");

            dtM.Columns.Add("计划在途", typeof(decimal));


            result ss = new result();
            ss.salelist_mx = dt_SaleOrder_mx;
            ss.salelist = dt_SaleOrder;
            ss.Polist_mx = IncompletePO;
            ss.Bom = dt_bom;
            ss.TotalCount = dt_totalcount;
            ss.str_log = "";
            ss.dtM = dtM;

            ss = calu(ss, bl);

            return ss;
        }
        /// <summary>
        /// 按单计算
        /// </summary>
        /// 
        public static result fun_pool(string str_订单号, bool bl)
        {

            //19-5-27 
            string x = "exec FourNum";
            CZMaster.MasterSQL.ExecuteSQL(x, strcon);
            //销售未完成 物料 数量明细  
            string s = string.Format(@" select  x.*,case when zlzb.销售订单明细号 is null then 0 else 1 end as 已关联 from [V_CalPoolTotal] x
            left join  (select 销售订单明细号 from 生产记录生产制令子表 group by 销售订单明细号) zlzb on zlzb.销售订单明细号 = x.销售订单明细号
            where x.销售订单号='{0}'", str_订单号);
            DataTable dt_SaleOrder_mx = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //汇总
            s = string.Format(@"select 物料编码,sum(未完成数量)数量,MIN(预计发货日期) as 最早发货日期 from [V_CalPoolTotal] where 销售订单号='{0}' group by 物料编码", str_订单号);
            DataTable dt_SaleOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);


            //未完成工单 物料 数量 汇总   此处可能要加入生效日期限制,有部分数据是同步的用友数据 
            //         s = @" select  x.*,kc.仓库名称,库存总数,未领量,base.物料编码,base.物料名称,base.规格型号,base.存货分类 from (
            //              select  gd.生产工单号,物料编码,生产数量,sum(生产数量)-isnull(SUM(a.已入库数量),0) as 数量,仓库号  from 生产记录生产工单表  gd 
            //               left join (select  生产工单号,SUM(入库数量) as 已入库数量 from  生产记录成品入库单明细表  where 作废=0 group by 生产工单号 )a 
            //          on a.生产工单号=gd.生产工单号 
            //where gd.生效=1 and 完成=0 and gd.关闭=0 and 作废=0 group by 物料编码,生产数量,gd.生产工单号,仓库号)x
            //  left join 基础数据物料信息表 base on base.物料编码=x.物料编码 
            //  left join 仓库物料数量表  kc on kc.物料编码=base.物料编码 and x.仓库号=kc.仓库号
            //  order by x.物料编码";
            //            DataTable IncompleteWorkOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //            //未完成采购单 
            //            s=@"select  物料编码,SUM(未完成数量) as 数量 from 采购记录采购单明细表  where 生效=1 and 未完成数量>0 and 作废=0 group by 物料编码"
            //            DataTable IncompletePO = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = @"  select  采购明细号,base.物料名称,base.物料编码,采购数量,未完成数量,mx.仓库号,mx.仓库名称,到货日期,存货分类,库存总数,在途量,未领量,在制量,受订量 from 采购记录采购单明细表 mx
                left join 采购记录采购单主表 zb  on zb.采购单号 =mx.采购单号
                left join 基础数据物料信息表 base on base.物料编码=mx.物料编码
                 left join 仓库物料数量表  kc  on kc.物料编码=mx.物料编码 and kc.仓库号=mx.仓库号
                     where mx.生效=1 and 未完成数量>0 and mx.作废=0 and zb.作废=0 and 明细完成日期 is null";
            DataTable IncompletePO = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = @"select  产品编码,产品名称,fx.存货分类 as 父项分类,fx.规格型号 as 父项规格,子项编码,子项名称,数量,zx.委外 as 子项委外,zx.自制 as 子项自制,zx.可购 as 子项可购,zx.存货分类 as 子项分类
                ,zx.规格型号 as 子项规格       from 基础数据物料BOM表 bom 
               left join 基础数据物料信息表 zx on bom.子项编码=zx.物料编码 
               left join 基础数据物料信息表 fx on bom.产品编码=fx.物料编码         where 优先级=1 ";
            DataTable dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn[] pk_bom = new DataColumn[2];
            pk_bom[0] = dt_bom.Columns["产品编码"];
            pk_bom[1] = dt_bom.Columns["子项编码"];
            dt_bom.PrimaryKey = pk_bom;


            ///取库存,总数=库存-未领+在制+在途  不减受订 是为了下面 根据 这个来计算，下面会有算一遍 总数-受订量
            //       s = @"select kc.物料编码,base.物料名称,base.规格型号,存货分类,库存总数,受订量,在制量,未领量,自制,在途量,isNull(委外在途,0)委外在途,可购,库存总数+在制量+在途量-未领量 as 总数,0 需求数量
            //,工时,车间编号,新数据,采购周期,base.仓库号 as 默认仓库号,base.仓库名称,供应商编号,默认供应商,isnull(已转制令数,0)已转制令数,isnull(已转工单数,0)已转工单数,isnull(采购员,'')采购员  from  
            //(select  物料编码,sum(库存总数)库存总数,MAX(受订量) 受订量,MAX(在制量)在制量,max(未领量)未领量,max(在途量) as 在途量  from 仓库物料数量表
            // where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
            // left join 基础数据物料信息表 base   on  base.物料编码=kc.物料编码
            // left join( select  物料编码,SUM(未完成数量) as 委外在途 from 采购记录采购单明细表  where 明细类型 ='委外采购' and  生效=1 and 未完成数量 >0 and 作废=0  
            // group by 物料编码)ww on ww.物料编码=kc.物料编码 
            //  left join   (select  物料编码,SUM(制令数量)已转制令数,SUM(已排单数量)已转工单数  from 生产记录生产制令表  where 完成=0 and 关闭=0 group by 物料编码)zlgd
            // on zlgd.物料编码=base.物料编码      left join 物料默认采购员 mrcgy on mrcgy.物料编码=base.物料编码 ";
            s = "select * from V_pooltotal  ";
            DataTable dt_totalcount = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn[] pk = new DataColumn[1];
            pk[0] = dt_totalcount.Columns["物料编码"];
            dt_totalcount.PrimaryKey = pk;
            DataTable dtM = new DataTable();
            dtM.Columns.Add("未领量", typeof(decimal));
            dtM.Columns.Add("在途量", typeof(decimal));
            dtM.Columns.Add("在制量", typeof(decimal));
            dtM.Columns.Add("仓库号");
            dtM.Columns.Add("仓库名称");
            dtM.Columns.Add("物料编码");
            dtM.Columns.Add("物料名称");
            dtM.Columns.Add("规格型号");
            dtM.Columns.Add("库存总数", typeof(decimal));
            dtM.Columns.Add("最早发货日期", typeof(DateTime));
            dtM.Columns.Add("存货分类");
            dtM.Columns.Add("参考数量", typeof(decimal));
            dtM.Columns.Add("受订量", typeof(decimal));
            dtM.Columns.Add("自制", typeof(bool));
            dtM.Columns.Add("工时", typeof(decimal));
            dtM.Columns.Add("需求数量", typeof(decimal));
            dtM.Columns.Add("已转制令数", typeof(decimal));
            dtM.Columns.Add("已转工单数", typeof(decimal));
            dtM.Columns.Add("拼板数量", typeof(decimal));
            dtM.Columns.Add("采购周期");
            dtM.Columns.Add("最小包装", typeof(decimal));
            //19-11-06
            dtM.Columns.Add("订单用量", typeof(decimal));
            //20-1-14
            dtM.Columns.Add("停用", typeof(bool));
            dtM.Columns.Add("班组编号");
            dtM.Columns.Add("班组名称");
            result ss = new result();
            ss.salelist_mx = dt_SaleOrder_mx;
            ss.salelist = dt_SaleOrder;
            ss.Polist_mx = IncompletePO;
            ss.Bom = dt_bom;

            ss.TotalCount = dt_totalcount;
            ss.str_log = "";
            ss.dtM = dtM;


            ss = calu(ss, bl);

            return ss;
        }


        /// <summary>
        /// 20-2-23 主计划 直接计算计划池和采购池
        /// </summary>
        /// <param name="str_订单号"></param>
        /// <param name="bl"></param>
        /// <returns></returns>
        public static result_主计划 fun_pool_all(DataTable dt_cs)
        {

            //19-5-27 
            string x = "exec FourNum";
            CZMaster.MasterSQL.ExecuteSQL(x, strcon);
            string s = "";
            ////销售未完成 物料 数量明细  
            //string s = string.Format(@" select  x.*,case when zlzb.销售订单明细号 is null then 0 else 1 end as 已关联 from [V_CalPoolTotal] x
            //left join  (select 销售订单明细号 from 生产记录生产制令子表 group by 销售订单明细号) zlzb on zlzb.销售订单明细号 = x.销售订单明细号
            //where x.销售订单号='{0}'", str_订单号);
            //DataTable dt_SaleOrder_mx = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            ////汇总
            //s = string.Format(@"select 物料编码,sum(未完成数量)数量,MIN(预计发货日期) as 最早发货日期 from [V_CalPoolTotal] where 销售订单号='{0}' group by 物料编码", str_订单号);
            //DataTable dt_SaleOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataTable dt_SaleOrder_mx = dt_cs;

            MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
            if (!dt_SaleOrder_mx.Columns.Contains("预计发货日期"))
            {
                DataColumn dc = new DataColumn("预计发货日期", typeof(DateTime));
                dc.DefaultValue = CPublic.Var.getDatetime().Date;
                dt_SaleOrder_mx.Columns.Add(dc);

            }
            if (!dt_SaleOrder_mx.Columns.Contains("可转数量"))
            {
                dt_SaleOrder_mx.Columns["数量"].ColumnName = "可转数量";
            }
            DataTable dt_SaleOrder = RBQ.SelectGroupByInto("", dt_SaleOrder_mx, "物料编码,sum(可转数量) 数量,min(预计发货日期) 最早发货日期", "", "物料编码");
            //DataTable dt_开工日期 = RBQ.SelectGroupByInto("", dt_SaleOrder_mx, "物料编码,min(预计开工日期) 最早预计开工日期", "", "物料编码");

            ////20 - 1 - 13
            //if (!dt_SaleOrder_mx.Columns.Contains("最早预计开工日期"))
            //{
            //    DataColumn dc = new DataColumn("最早预计开工日期", typeof(DateTime));
            //    dt_SaleOrder_mx.Columns.Add(dc);
            //    foreach (DataRow dr in dt_SaleOrder.Rows)
            //    {
            //        DataRow[] dr1 = dt_开工日期.Select(string.Format("物料编码 = '{0}'",dr["物料编码"]));
            //        if (dr1.Length>0)
            //        {
            //            dr["最早预计开工日期"] = dr1[0]["最早预计开工日期"];
            //        }
            //    }
            //}

            //未完成工单 物料 数量 汇总   此处可能要加入生效日期限制,有部分数据是同步的用友数据 
            //         s = @" select  x.*,kc.仓库名称,库存总数,未领量,base.物料编码,base.物料名称,base.规格型号,base.存货分类 from (
            //              select  gd.生产工单号,物料编码,生产数量,sum(生产数量)-isnull(SUM(a.已入库数量),0) as 数量,仓库号  from 生产记录生产工单表  gd 
            //               left join (select  生产工单号,SUM(入库数量) as 已入库数量 from  生产记录成品入库单明细表  where 作废=0 group by 生产工单号 )a 
            //          on a.生产工单号=gd.生产工单号 
            //where gd.生效=1 and 完成=0 and gd.关闭=0 and 作废=0 group by 物料编码,生产数量,gd.生产工单号,仓库号)x
            //  left join 基础数据物料信息表 base on base.物料编码=x.物料编码 
            //  left join 仓库物料数量表  kc on kc.物料编码=base.物料编码 and x.仓库号=kc.仓库号
            //  order by x.物料编码";
            //            DataTable IncompleteWorkOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //            //未完成采购单 
            //            s=@"select  物料编码,SUM(未完成数量) as 数量 from 采购记录采购单明细表  where 生效=1 and 未完成数量>0 and 作废=0 group by 物料编码"
            //            DataTable IncompletePO = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = @"  select  采购明细号,base.物料名称,base.物料编码,采购数量,未完成数量,mx.仓库号,mx.仓库名称,到货日期,存货分类,库存总数,在途量,未领量,在制量,受订量 from 采购记录采购单明细表 mx
                left join 采购记录采购单主表 zb  on zb.采购单号 =mx.采购单号
                left join 基础数据物料信息表 base on base.物料编码=mx.物料编码
                 left join 仓库物料数量表  kc  on kc.物料编码=mx.物料编码 and kc.仓库号=mx.仓库号
                     where mx.生效=1 and 未完成数量>0 and mx.作废=0 and zb.作废=0 and 明细完成日期 is null";
            DataTable IncompletePO = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = @"select  产品编码,产品名称,fx.存货分类 as 父项分类,fx.规格型号 as 父项规格,子项编码,子项名称,数量,zx.委外 as 子项委外,zx.自制 as 子项自制,zx.可购 as 子项可购,zx.存货分类 as 子项分类
                ,zx.规格型号 as 子项规格       from 基础数据物料BOM表 bom 
               left join 基础数据物料信息表 zx on bom.子项编码=zx.物料编码 
               left join 基础数据物料信息表 fx on bom.产品编码=fx.物料编码          where 优先级=1";
            DataTable dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn[] pk_bom = new DataColumn[2];
            pk_bom[0] = dt_bom.Columns["产品编码"];
            pk_bom[1] = dt_bom.Columns["子项编码"];
            dt_bom.PrimaryKey = pk_bom;
            ///取库存,总数=库存-未领+在制+在途  不减受订 是为了下面 根据 这个来计算，下面会有算一遍 总数-受订量
            //       s = @"select kc.物料编码,base.物料名称,base.规格型号,存货分类,库存总数,受订量,在制量,未领量,自制,在途量,isNull(委外在途,0)委外在途,可购,库存总数+在制量+在途量-未领量 as 总数,0 需求数量
            //,工时,车间编号,新数据,采购周期,base.仓库号 as 默认仓库号,base.仓库名称,供应商编号,默认供应商,isnull(已转制令数,0)已转制令数,isnull(已转工单数,0)已转工单数,isnull(采购员,'')采购员  from  
            //(select  物料编码,sum(库存总数)库存总数,MAX(受订量) 受订量,MAX(在制量)在制量,max(未领量)未领量,max(在途量) as 在途量  from 仓库物料数量表
            // where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
            // left join 基础数据物料信息表 base   on  base.物料编码=kc.物料编码
            // left join( select  物料编码,SUM(未完成数量) as 委外在途 from 采购记录采购单明细表  where 明细类型 ='委外采购' and  生效=1 and 未完成数量 >0 and 作废=0  
            // group by 物料编码)ww on ww.物料编码=kc.物料编码 
            //  left join   (select  物料编码,SUM(制令数量)已转制令数,SUM(已排单数量)已转工单数  from 生产记录生产制令表  where 完成=0 and 关闭=0 group by 物料编码)zlgd
            // on zlgd.物料编码=base.物料编码      left join 物料默认采购员 mrcgy on mrcgy.物料编码=base.物料编码 ";
            s = " select * from V_pooltotal ";
            DataTable dt_totalcount = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn[] pk = new DataColumn[1];
            pk[0] = dt_totalcount.Columns["物料编码"];
            dt_totalcount.PrimaryKey = pk;
            DataTable dtM = new DataTable();
            dtM.Columns.Add("未领量", typeof(decimal));
            dtM.Columns.Add("在途量", typeof(decimal));
            dtM.Columns.Add("在制量", typeof(decimal));
            dtM.Columns.Add("仓库号");
            dtM.Columns.Add("仓库名称");
            dtM.Columns.Add("物料编码");
            dtM.Columns.Add("物料名称");
            dtM.Columns.Add("规格型号");
            dtM.Columns.Add("库存总数", typeof(decimal));
            dtM.Columns.Add("最早发货日期", typeof(DateTime));
            dtM.Columns.Add("存货分类");
            dtM.Columns.Add("参考数量", typeof(decimal));
            dtM.Columns.Add("受订量", typeof(decimal));
            dtM.Columns.Add("自制", typeof(bool));
            dtM.Columns.Add("工时", typeof(decimal));
            dtM.Columns.Add("需求数量", typeof(decimal));
            dtM.Columns.Add("已转制令数", typeof(decimal));
            dtM.Columns.Add("已转工单数", typeof(decimal));
            dtM.Columns.Add("拼板数量", typeof(decimal));
            dtM.Columns.Add("采购周期");
            dtM.Columns.Add("最小包装", typeof(decimal));
            //19-11-06
            dtM.Columns.Add("订单用量", typeof(decimal));
            //20-1-14
            dtM.Columns.Add("停用", typeof(bool));
            dtM.Columns.Add("班组编号");
            dtM.Columns.Add("班组名称");
            result_主计划 ss = new result_主计划();
            ss.salelist_mx = dt_SaleOrder_mx;
            ss.salelist = dt_SaleOrder;
            ss.Polist_mx = IncompletePO;
            ss.Bom = dt_bom;
            ss.TotalCount = dt_totalcount;
            ss.str_log = "";
            ss.dtM = dtM;
            //20-1-13 主计划用
            ss = caluu_主计划_all(ss);

            return ss;
        }

        /// <summary>
        /// 20-4-3 计划需求单 直接计算计划池和采购池
        /// </summary>
        /// <returns></returns>
        public static result_主计划 fun_pool_all(DateTime t1, DateTime t2)
        {

            //19-5-27 
            string x = "exec FourNum";
            CZMaster.MasterSQL.ExecuteSQL(x, strcon);
            string s = "";
            ////销售未完成 物料 数量明细  
            //string s = string.Format(@" select  x.*,case when zlzb.销售订单明细号 is null then 0 else 1 end as 已关联 from [V_CalPoolTotal] x
            //left join  (select 销售订单明细号 from 生产记录生产制令子表 group by 销售订单明细号) zlzb on zlzb.销售订单明细号 = x.销售订单明细号
            //where x.销售订单号='{0}'", str_订单号);
            //DataTable dt_SaleOrder_mx = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            ////汇总
            //s = string.Format(@"select 物料编码,sum(未完成数量)数量,MIN(预计发货日期) as 最早发货日期 from [V_CalPoolTotal] where 销售订单号='{0}' group by 物料编码", str_订单号);
            //DataTable dt_SaleOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = string.Format(@"  select  x.*,case when zlzb.销售订单明细号 is null then 0 else 1 end as 已关联 from [V_CalPoolTotal] x
          left join  (select 销售订单明细号 from 生产记录生产制令子表 group by 销售订单明细号) zlzb on zlzb.销售订单明细号 = x.销售订单明细号
                  where [预计发货日期]>'{0}' and 预计发货日期<'{1}'  ", t1, t2);
            DataTable dt_SaleOrder_mx = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
            DataTable dt_SaleOrder = RBQ.SelectGroupByInto("", dt_SaleOrder_mx, "物料编码,sum(数量) 数量,min(预计发货日期) 最早发货日期", "", "物料编码");
            //DataTable dt_开工日期 = RBQ.SelectGroupByInto("", dt_SaleOrder_mx, "物料编码,min(预计开工日期) 最早预计开工日期", "", "物料编码");

            ////20 - 1 - 13
            //if (!dt_SaleOrder_mx.Columns.Contains("最早预计开工日期"))
            //{
            //    DataColumn dc = new DataColumn("最早预计开工日期", typeof(DateTime));
            //    dt_SaleOrder_mx.Columns.Add(dc);
            //    foreach (DataRow dr in dt_SaleOrder.Rows)
            //    {
            //        DataRow[] dr1 = dt_开工日期.Select(string.Format("物料编码 = '{0}'",dr["物料编码"]));
            //        if (dr1.Length>0)
            //        {
            //            dr["最早预计开工日期"] = dr1[0]["最早预计开工日期"];
            //        }
            //    }
            //}

            //未完成工单 物料 数量 汇总   此处可能要加入生效日期限制,有部分数据是同步的用友数据 
            //         s = @" select  x.*,kc.仓库名称,库存总数,未领量,base.物料编码,base.物料名称,base.规格型号,base.存货分类 from (
            //              select  gd.生产工单号,物料编码,生产数量,sum(生产数量)-isnull(SUM(a.已入库数量),0) as 数量,仓库号  from 生产记录生产工单表  gd 
            //               left join (select  生产工单号,SUM(入库数量) as 已入库数量 from  生产记录成品入库单明细表  where 作废=0 group by 生产工单号 )a 
            //          on a.生产工单号=gd.生产工单号 
            //where gd.生效=1 and 完成=0 and gd.关闭=0 and 作废=0 group by 物料编码,生产数量,gd.生产工单号,仓库号)x
            //  left join 基础数据物料信息表 base on base.物料编码=x.物料编码 
            //  left join 仓库物料数量表  kc on kc.物料编码=base.物料编码 and x.仓库号=kc.仓库号
            //  order by x.物料编码";
            //            DataTable IncompleteWorkOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //            //未完成采购单 
            //            s=@"select  物料编码,SUM(未完成数量) as 数量 from 采购记录采购单明细表  where 生效=1 and 未完成数量>0 and 作废=0 group by 物料编码"
            //            DataTable IncompletePO = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = @"  select  采购明细号,base.物料名称,base.物料编码,采购数量,未完成数量,mx.仓库号,mx.仓库名称,到货日期,存货分类,库存总数,在途量,未领量,在制量,受订量 from 采购记录采购单明细表 mx
                left join 采购记录采购单主表 zb  on zb.采购单号 =mx.采购单号
                left join 基础数据物料信息表 base on base.物料编码=mx.物料编码
                 left join 仓库物料数量表  kc  on kc.物料编码=mx.物料编码 and kc.仓库号=mx.仓库号
                     where mx.生效=1 and 未完成数量>0 and mx.作废=0 and zb.作废=0 and 明细完成日期 is null";
            DataTable IncompletePO = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = @"select  产品编码,产品名称,fx.存货分类 as 父项分类,fx.规格型号 as 父项规格,子项编码,子项名称,数量,zx.委外 as 子项委外,zx.自制 as 子项自制,zx.可购 as 子项可购,zx.存货分类 as 子项分类
                ,zx.规格型号 as 子项规格       from 基础数据物料BOM表 bom 
               left join 基础数据物料信息表 zx on bom.子项编码=zx.物料编码 
               left join 基础数据物料信息表 fx on bom.产品编码=fx.物料编码          where 优先级=1";
            DataTable dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn[] pk_bom = new DataColumn[2];
            pk_bom[0] = dt_bom.Columns["产品编码"];
            pk_bom[1] = dt_bom.Columns["子项编码"];
            dt_bom.PrimaryKey = pk_bom;
            ///取库存,总数=库存-未领+在制+在途  不减受订 是为了下面 根据 这个来计算，下面会有算一遍 总数-受订量
            //       s = @"select kc.物料编码,base.物料名称,base.规格型号,存货分类,库存总数,受订量,在制量,未领量,自制,在途量,isNull(委外在途,0)委外在途,可购,库存总数+在制量+在途量-未领量 as 总数,0 需求数量
            //,工时,车间编号,新数据,采购周期,base.仓库号 as 默认仓库号,base.仓库名称,供应商编号,默认供应商,isnull(已转制令数,0)已转制令数,isnull(已转工单数,0)已转工单数,isnull(采购员,'')采购员  from  
            //(select  物料编码,sum(库存总数)库存总数,MAX(受订量) 受订量,MAX(在制量)在制量,max(未领量)未领量,max(在途量) as 在途量  from 仓库物料数量表
            // where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
            // left join 基础数据物料信息表 base   on  base.物料编码=kc.物料编码
            // left join( select  物料编码,SUM(未完成数量) as 委外在途 from 采购记录采购单明细表  where 明细类型 ='委外采购' and  生效=1 and 未完成数量 >0 and 作废=0  
            // group by 物料编码)ww on ww.物料编码=kc.物料编码 
            //  left join   (select  物料编码,SUM(制令数量)已转制令数,SUM(已排单数量)已转工单数  from 生产记录生产制令表  where 完成=0 and 关闭=0 group by 物料编码)zlgd
            // on zlgd.物料编码=base.物料编码      left join 物料默认采购员 mrcgy on mrcgy.物料编码=base.物料编码 ";
            s = " select * from V_pooltotal ";
            DataTable dt_totalcount = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn[] pk = new DataColumn[1];
            pk[0] = dt_totalcount.Columns["物料编码"];
            dt_totalcount.PrimaryKey = pk;
            DataTable dtM = new DataTable();
            dtM.Columns.Add("未领量", typeof(decimal));
            dtM.Columns.Add("在途量", typeof(decimal));
            dtM.Columns.Add("在制量", typeof(decimal));
            dtM.Columns.Add("仓库号");
            dtM.Columns.Add("仓库名称");
            dtM.Columns.Add("物料编码");
            dtM.Columns.Add("物料名称");
            dtM.Columns.Add("规格型号");
            dtM.Columns.Add("库存总数", typeof(decimal));
            dtM.Columns.Add("最早发货日期", typeof(DateTime));
            dtM.Columns.Add("存货分类");
            dtM.Columns.Add("参考数量", typeof(decimal));
            dtM.Columns.Add("受订量", typeof(decimal));
            dtM.Columns.Add("自制", typeof(bool));
            dtM.Columns.Add("工时", typeof(decimal));
            dtM.Columns.Add("需求数量", typeof(decimal));
            dtM.Columns.Add("已转制令数", typeof(decimal));
            dtM.Columns.Add("已转工单数", typeof(decimal));
            dtM.Columns.Add("拼板数量", typeof(decimal));
            dtM.Columns.Add("采购周期");
            dtM.Columns.Add("最小包装", typeof(decimal));
            //19-11-06
            dtM.Columns.Add("订单用量", typeof(decimal));
            //20-1-14
            dtM.Columns.Add("停用", typeof(bool));
            dtM.Columns.Add("班组编号");
            dtM.Columns.Add("班组名称");
            result_主计划 ss = new result_主计划();
            ss.salelist_mx = dt_SaleOrder_mx;
            ss.salelist = dt_SaleOrder;
            ss.Polist_mx = IncompletePO;
            ss.Bom = dt_bom;
            ss.TotalCount = dt_totalcount;
            ss.str_log = "";
            ss.dtM = dtM;
            //20-1-13 主计划用
            ss = caluu_主计划_all_1(ss);

            return ss;
        }

        /// <summary>
        /// 给采购根据生产计划算缺料情况 报预算用
        /// </summary>
        /// <param name="str_订单号"></param>
        /// <param name="bl"></param>
        /// <returns></returns>
        public static result fun_pool(DataTable dt_cs, bool bl)
        {

            //19-5-27 
            string x = "exec FourNum";
            CZMaster.MasterSQL.ExecuteSQL(x, strcon);
            string s = "";
            ////销售未完成 物料 数量明细  
            //string s = string.Format(@" select  x.*,case when zlzb.销售订单明细号 is null then 0 else 1 end as 已关联 from [V_CalPoolTotal] x
            //left join  (select 销售订单明细号 from 生产记录生产制令子表 group by 销售订单明细号) zlzb on zlzb.销售订单明细号 = x.销售订单明细号
            //where x.销售订单号='{0}'", str_订单号);
            //DataTable dt_SaleOrder_mx = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            ////汇总
            //s = string.Format(@"select 物料编码,sum(未完成数量)数量,MIN(预计发货日期) as 最早发货日期 from [V_CalPoolTotal] where 销售订单号='{0}' group by 物料编码", str_订单号);
            //DataTable dt_SaleOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataTable dt_SaleOrder_mx = dt_cs;
            if (!dt_SaleOrder_mx.Columns.Contains("最早发货日期"))
            {
                DataColumn dc = new DataColumn("最早发货日期", typeof(DateTime));
                dc.DefaultValue = CPublic.Var.getDatetime().Date;
                dt_SaleOrder_mx.Columns.Add(dc);
            }

            MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
            DataTable dt_SaleOrder = RBQ.SelectGroupByInto("", dt_SaleOrder_mx, "物料编码,sum(数量) 数量,min(最早发货日期) 最早发货日期", "", "物料编码");
            //DataTable dt_开工日期 = RBQ.SelectGroupByInto("", dt_SaleOrder_mx, "物料编码,min(预计开工日期) 最早预计开工日期", "", "物料编码");

            ////20 - 1 - 13
            //if (!dt_SaleOrder_mx.Columns.Contains("最早预计开工日期"))
            //{
            //    DataColumn dc = new DataColumn("最早预计开工日期", typeof(DateTime));
            //    dt_SaleOrder_mx.Columns.Add(dc);
            //    foreach (DataRow dr in dt_SaleOrder.Rows)
            //    {
            //        DataRow[] dr1 = dt_开工日期.Select(string.Format("物料编码 = '{0}'",dr["物料编码"]));
            //        if (dr1.Length>0)
            //        {
            //            dr["最早预计开工日期"] = dr1[0]["最早预计开工日期"];
            //        }
            //    }
            //}

            //未完成工单 物料 数量 汇总   此处可能要加入生效日期限制,有部分数据是同步的用友数据 
            //         s = @" select  x.*,kc.仓库名称,库存总数,未领量,base.物料编码,base.物料名称,base.规格型号,base.存货分类 from (
            //              select  gd.生产工单号,物料编码,生产数量,sum(生产数量)-isnull(SUM(a.已入库数量),0) as 数量,仓库号  from 生产记录生产工单表  gd 
            //               left join (select  生产工单号,SUM(入库数量) as 已入库数量 from  生产记录成品入库单明细表  where 作废=0 group by 生产工单号 )a 
            //          on a.生产工单号=gd.生产工单号 
            //where gd.生效=1 and 完成=0 and gd.关闭=0 and 作废=0 group by 物料编码,生产数量,gd.生产工单号,仓库号)x
            //  left join 基础数据物料信息表 base on base.物料编码=x.物料编码 
            //  left join 仓库物料数量表  kc on kc.物料编码=base.物料编码 and x.仓库号=kc.仓库号
            //  order by x.物料编码";
            //            DataTable IncompleteWorkOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //            //未完成采购单 
            //            s=@"select  物料编码,SUM(未完成数量) as 数量 from 采购记录采购单明细表  where 生效=1 and 未完成数量>0 and 作废=0 group by 物料编码"
            //            DataTable IncompletePO = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = @"  select  采购明细号,base.物料名称,base.物料编码,采购数量,未完成数量,mx.仓库号,mx.仓库名称,到货日期,存货分类,库存总数,在途量,未领量,在制量,受订量 from 采购记录采购单明细表 mx
                left join 采购记录采购单主表 zb  on zb.采购单号 =mx.采购单号
                left join 基础数据物料信息表 base on base.物料编码=mx.物料编码
                 left join 仓库物料数量表  kc  on kc.物料编码=mx.物料编码 and kc.仓库号=mx.仓库号
                     where mx.生效=1 and 未完成数量>0 and mx.作废=0 and zb.作废=0 and 明细完成日期 is null";
            DataTable IncompletePO = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = @"select  产品编码,产品名称,fx.存货分类 as 父项分类,fx.规格型号 as 父项规格,子项编码,子项名称,数量,zx.委外 as 子项委外,zx.自制 as 子项自制,zx.可购 as 子项可购,zx.存货分类 as 子项分类
                ,zx.规格型号 as 子项规格       from 基础数据物料BOM表 bom 
               left join 基础数据物料信息表 zx on bom.子项编码=zx.物料编码 
               left join 基础数据物料信息表 fx on bom.产品编码=fx.物料编码         where 优先级=1 ";
            DataTable dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn[] pk_bom = new DataColumn[2];
            pk_bom[0] = dt_bom.Columns["产品编码"];
            pk_bom[1] = dt_bom.Columns["子项编码"];
            dt_bom.PrimaryKey = pk_bom;
            ///取库存,总数=库存-未领+在制+在途  不减受订 是为了下面 根据 这个来计算，下面会有算一遍 总数-受订量
            //       s = @"select kc.物料编码,base.物料名称,base.规格型号,存货分类,库存总数,受订量,在制量,未领量,自制,在途量,isNull(委外在途,0)委外在途,可购,库存总数+在制量+在途量-未领量 as 总数,0 需求数量
            //,工时,车间编号,新数据,采购周期,base.仓库号 as 默认仓库号,base.仓库名称,供应商编号,默认供应商,isnull(已转制令数,0)已转制令数,isnull(已转工单数,0)已转工单数,isnull(采购员,'')采购员  from  
            //(select  物料编码,sum(库存总数)库存总数,MAX(受订量) 受订量,MAX(在制量)在制量,max(未领量)未领量,max(在途量) as 在途量  from 仓库物料数量表
            // where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
            // left join 基础数据物料信息表 base   on  base.物料编码=kc.物料编码
            // left join( select  物料编码,SUM(未完成数量) as 委外在途 from 采购记录采购单明细表  where 明细类型 ='委外采购' and  生效=1 and 未完成数量 >0 and 作废=0  
            // group by 物料编码)ww on ww.物料编码=kc.物料编码 
            //  left join   (select  物料编码,SUM(制令数量)已转制令数,SUM(已排单数量)已转工单数  from 生产记录生产制令表  where 完成=0 and 关闭=0 group by 物料编码)zlgd
            // on zlgd.物料编码=base.物料编码      left join 物料默认采购员 mrcgy on mrcgy.物料编码=base.物料编码 ";
            s = " select * from V_pooltotal ";
            DataTable dt_totalcount = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn[] pk = new DataColumn[1];
            pk[0] = dt_totalcount.Columns["物料编码"];
            dt_totalcount.PrimaryKey = pk;
            DataTable dtM = new DataTable();
            dtM.Columns.Add("未领量", typeof(decimal));
            dtM.Columns.Add("在途量", typeof(decimal));
            dtM.Columns.Add("在制量", typeof(decimal));
            dtM.Columns.Add("仓库号");
            dtM.Columns.Add("仓库名称");
            dtM.Columns.Add("物料编码");
            dtM.Columns.Add("物料名称");
            dtM.Columns.Add("规格型号");
            dtM.Columns.Add("库存总数", typeof(decimal));
            dtM.Columns.Add("最早发货日期", typeof(DateTime));
            //dtM.Columns.Add("最早预计开工日期", typeof(DateTime));
            dtM.Columns.Add("存货分类");
            dtM.Columns.Add("参考数量", typeof(decimal));
            dtM.Columns.Add("受订量", typeof(decimal));
            dtM.Columns.Add("自制", typeof(bool));
            dtM.Columns.Add("工时", typeof(decimal));
            dtM.Columns.Add("需求数量", typeof(decimal));
            dtM.Columns.Add("已转制令数", typeof(decimal));
            dtM.Columns.Add("已转工单数", typeof(decimal));
            dtM.Columns.Add("计划在途", typeof(decimal));

            dtM.Columns.Add("拼板数量", typeof(decimal));
            dtM.Columns.Add("采购周期");
            dtM.Columns.Add("最小包装", typeof(decimal));
            //19-11-06
            dtM.Columns.Add("订单用量", typeof(decimal));
            //20-1-14
            dtM.Columns.Add("停用", typeof(bool));
            dtM.Columns.Add("班组编号");
            dtM.Columns.Add("班组名称");
            result ss = new result();
            ss.salelist_mx = dt_SaleOrder_mx;
            ss.salelist = dt_SaleOrder;
            ss.Polist_mx = IncompletePO;
            ss.Bom = dt_bom;
            ss.TotalCount = dt_totalcount;
            ss.str_log = "";
            ss.dtM = dtM;
            ss = calu1(ss, bl);

            return ss;
        }

        /// <summary>
        ///  
        /// 19-10-10 计算未转成主计划所缺的料
        /// </summary>
        /// <param name="dt_cs"></param>
        /// <param name="bl"></param>
        /// <param name="dt_total">算完主计划后dt_totalcount</param>
        /// <returns></returns>
        public static result fun_pool(DataTable dt_cs, bool bl, DataTable dt_total)
        {
            //19-5-27 
            string x = "exec FourNum";
            CZMaster.MasterSQL.ExecuteSQL(x, strcon);
            string s = "";
            ////销售未完成 物料 数量明细  
            //string s = string.Format(@" select  x.*,case when zlzb.销售订单明细号 is null then 0 else 1 end as 已关联 from [V_CalPoolTotal] x
            //left join  (select 销售订单明细号 from 生产记录生产制令子表 group by 销售订单明细号) zlzb on zlzb.销售订单明细号 = x.销售订单明细号
            //where x.销售订单号='{0}'", str_订单号);
            //DataTable dt_SaleOrder_mx = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            ////汇总
            //s = string.Format(@"select 物料编码,sum(未完成数量)数量,MIN(预计发货日期) as 最早发货日期 from [V_CalPoolTotal] where 销售订单号='{0}' group by 物料编码", str_订单号);
            //DataTable dt_SaleOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataTable dt_SaleOrder_mx = dt_cs;
            if (!dt_SaleOrder_mx.Columns.Contains("最早发货日期"))
            {
                DataColumn dc = new DataColumn("最早发货日期", typeof(DateTime));
                dc.DefaultValue = CPublic.Var.getDatetime().Date;
                dt_SaleOrder_mx.Columns.Add(dc);
            }
            MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
            DataTable dt_SaleOrder = RBQ.SelectGroupByInto("", dt_SaleOrder_mx, "物料编码,sum(数量) 数量,min(最早发货日期) 最早发货日期 ", "", "物料编码");

            //未完成工单 物料 数量 汇总   此处可能要加入生效日期限制,有部分数据是同步的用友数据 
            //         s = @" select  x.*,kc.仓库名称,库存总数,未领量,base.物料编码,base.物料名称,base.规格型号,base.存货分类 from (
            //              select  gd.生产工单号,物料编码,生产数量,sum(生产数量)-isnull(SUM(a.已入库数量),0) as 数量,仓库号  from 生产记录生产工单表  gd 
            //               left join (select  生产工单号,SUM(入库数量) as 已入库数量 from  生产记录成品入库单明细表  where 作废=0 group by 生产工单号 )a 
            //          on a.生产工单号=gd.生产工单号 
            //where gd.生效=1 and 完成=0 and gd.关闭=0 and 作废=0 group by 物料编码,生产数量,gd.生产工单号,仓库号)x
            //  left join 基础数据物料信息表 base on base.物料编码=x.物料编码 
            //  left join 仓库物料数量表  kc on kc.物料编码=base.物料编码 and x.仓库号=kc.仓库号
            //  order by x.物料编码";
            //            DataTable IncompleteWorkOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //            //未完成采购单 
            //            s=@"select  物料编码,SUM(未完成数量) as 数量 from 采购记录采购单明细表  where 生效=1 and 未完成数量>0 and 作废=0 group by 物料编码"
            //            DataTable IncompletePO = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = @"  select  采购明细号,base.物料名称,base.物料编码,采购数量,未完成数量,mx.仓库号,mx.仓库名称,到货日期,存货分类,库存总数,在途量,未领量,在制量,受订量 from 采购记录采购单明细表 mx
                left join 采购记录采购单主表 zb  on zb.采购单号 =mx.采购单号
                left join 基础数据物料信息表 base on base.物料编码=mx.物料编码
                 left join 仓库物料数量表  kc  on kc.物料编码=mx.物料编码 and kc.仓库号=mx.仓库号
                     where mx.生效=1 and 未完成数量>0 and mx.作废=0 and zb.作废=0 and 明细完成日期 is null";
            DataTable IncompletePO = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = @"select  产品编码,产品名称,fx.存货分类 as 父项分类,fx.规格型号 as 父项规格,子项编码,子项名称,数量,zx.委外 as 子项委外,zx.自制 as 子项自制,zx.可购 as 子项可购,zx.存货分类 as 子项分类
                ,zx.规格型号 as 子项规格       from 基础数据物料BOM表 bom 
               left join 基础数据物料信息表 zx on bom.子项编码=zx.物料编码 
               left join 基础数据物料信息表 fx on bom.产品编码=fx.物料编码         where 优先级=1";
            DataTable dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn[] pk_bom = new DataColumn[2];
            pk_bom[0] = dt_bom.Columns["产品编码"];
            pk_bom[1] = dt_bom.Columns["子项编码"];
            dt_bom.PrimaryKey = pk_bom;
            ///取库存,总数=库存-未领+在制+在途  不减受订 是为了下面 根据 这个来计算，下面会有算一遍 总数-受订量
            //       s = @"select kc.物料编码,base.物料名称,base.规格型号,存货分类,库存总数,受订量,在制量,未领量,自制,在途量,isNull(委外在途,0)委外在途,可购,库存总数+在制量+在途量-未领量 as 总数,0 需求数量
            //,工时,车间编号,新数据,采购周期,base.仓库号 as 默认仓库号,base.仓库名称,供应商编号,默认供应商,isnull(已转制令数,0)已转制令数,isnull(已转工单数,0)已转工单数,isnull(采购员,'')采购员  from  
            //(select  物料编码,sum(库存总数)库存总数,MAX(受订量) 受订量,MAX(在制量)在制量,max(未领量)未领量,max(在途量) as 在途量  from 仓库物料数量表
            // where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
            // left join 基础数据物料信息表 base   on  base.物料编码=kc.物料编码
            // left join( select  物料编码,SUM(未完成数量) as 委外在途 from 采购记录采购单明细表  where 明细类型 ='委外采购' and  生效=1 and 未完成数量 >0 and 作废=0  
            // group by 物料编码)ww on ww.物料编码=kc.物料编码 
            //  left join   (select  物料编码,SUM(制令数量)已转制令数,SUM(已排单数量)已转工单数  from 生产记录生产制令表  where 完成=0 and 关闭=0 group by 物料编码)zlgd
            // on zlgd.物料编码=base.物料编码      left join 物料默认采购员 mrcgy on mrcgy.物料编码=base.物料编码 ";
            //  s = @"select * from [V_pooltotal]  ";
            DataTable dt_totalcount = dt_total;
            DataColumn[] pk = new DataColumn[1];
            pk[0] = dt_totalcount.Columns["物料编码"];
            dt_totalcount.PrimaryKey = pk;
            DataTable dtM = new DataTable();
            dtM.Columns.Add("未领量", typeof(decimal));
            dtM.Columns.Add("在途量", typeof(decimal));
            dtM.Columns.Add("在制量", typeof(decimal));
            dtM.Columns.Add("仓库号");
            dtM.Columns.Add("仓库名称");
            dtM.Columns.Add("物料编码");
            dtM.Columns.Add("物料名称");
            dtM.Columns.Add("规格型号");
            dtM.Columns.Add("库存总数", typeof(decimal));
            dtM.Columns.Add("最早发货日期", typeof(DateTime));
            dtM.Columns.Add("存货分类");
            dtM.Columns.Add("参考数量", typeof(decimal));
            dtM.Columns.Add("受订量", typeof(decimal));
            dtM.Columns.Add("自制", typeof(bool));
            dtM.Columns.Add("工时", typeof(decimal));
            dtM.Columns.Add("需求数量", typeof(decimal));
            dtM.Columns.Add("已转制令数", typeof(decimal));
            dtM.Columns.Add("已转工单数", typeof(decimal));
            dtM.Columns.Add("拼板数量", typeof(decimal));
            //19-11-06
            dtM.Columns.Add("订单用量", typeof(decimal));
            dtM.Columns.Add("停用", typeof(bool));
            dtM.Columns.Add("班组编号");
            dtM.Columns.Add("班组名称");
            result ss = new result();
            ss.salelist_mx = dt_SaleOrder_mx;
            ss.salelist = dt_SaleOrder;
            ss.Polist_mx = IncompletePO;
            ss.Bom = dt_bom;
            ss.TotalCount = dt_totalcount;
            ss.str_log = "";
            ss.dtM = dtM;
            ss = calu(ss, bl);

            return ss;
        }
        public struct result
        {
            //计算用数据
            public DataTable salelist;
            public DataTable salelist_mx;
            public DataTable Polist_mx;
            public DataTable Bom;
            public DataTable TotalCount;

            public string str_log;
            //这个作为 最后计算结果 
            public DataTable dtM;
        }
        //2020-2-23 为了给主计划用 主计划界面点击计算 返回 计划池结果 和 采购池结果

        public struct result_主计划
        {
            //计算用数据
            public DataTable salelist;
            public DataTable salelist_mx;
            public DataTable Polist_mx;
            public DataTable Bom;
            public DataTable TotalCount;

            public string str_log;
            //这个作为 最后计算结果 
            public DataTable dtM; //用作计划池
            public DataTable dtM_采购池;

        }

        /// <summary>
        /// 
        /// </summary> 0
        /// <param name="itemid">物料编码</param>
        /// <param name="dec_需求"></param>
        /// <param name="bl_made">是否自制</param>
        /// <param name="t">最早发货日期</param>
        private static result fun_dg(result ss, string itemid, decimal dec_需求, bool bl_made, DateTime? t)
        {
            if (bl_made)
            {
                if (ss.Bom.Select(string.Format("产品编码='{0}'", itemid)).Length == 0)
                {
                    ss.str_log = ss.str_log + (itemid + "属性为自制但是没有bom;");
                }
            }
            DataRow[] br = ss.Bom.Select(string.Format("产品编码='{0}'and (子项自制=1 or 子项委外=1)", itemid));
            if (br.Length > 0) //找到需要自制的半成品 
            {
                decimal dec_缺 = dec_需求;
                foreach (DataRow brr in br)
                {
                    decimal dec = dec_缺 * Convert.ToDecimal(brr["数量"]); //这是自制半成品所需的量 

                    DataRow stock_total = ss.TotalCount.Rows.Find(brr["子项编码"]);

                    //DataRow[] stock_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                    if (stock_total == null)
                    {
                        throw new Exception(brr["子项编码"].ToString() + "在总表中未找到数据");
                    }

                    decimal total_z = Convert.ToDecimal(stock_total["总数"]);
                    stock_total["需求数量"] = Convert.ToDecimal(stock_total["需求数量"]) + dec;
                    if (total_z >= dec) //库存加未完成>需求数
                    {
                        stock_total["总数"] = total_z - dec;
                    }
                    else
                    {
                        DataRow[] fr = ss.dtM.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                        if (fr.Length > 0)
                        {
                            fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec - total_z;
                            if (fr[0]["最早发货日期"] != DBNull.Value && t != null && (Convert.ToDateTime(fr[0]["最早发货日期"]) > t || Convert.ToDateTime(fr[0]["最早发货日期"]) == Convert.ToDateTime("2011-1-1")))
                            {
                                fr[0]["最早发货日期"] = t;
                            }

                        }
                        else
                        {
                            DataRow r_need = ss.dtM.NewRow();
                            r_need["在制量"] = stock_total["在制量"];
                            r_need["未领量"] = stock_total["未领量"];
                            r_need["在途量"] = stock_total["在途量"];
                            r_need["计划在途"] = stock_total["计划在途"];

                            r_need["仓库号"] = stock_total["默认仓库号"];
                            r_need["仓库名称"] = stock_total["仓库名称"];

                            r_need["工时"] = stock_total["工时"];
                            r_need["物料编码"] = stock_total["物料编码"];
                            r_need["物料名称"] = stock_total["物料名称"];
                            r_need["规格型号"] = stock_total["规格型号"];
                            r_need["存货分类"] = stock_total["存货分类"];
                            r_need["库存总数"] = stock_total["库存总数"];
                            r_need["受订量"] = stock_total["受订量"];
                            r_need["自制"] = stock_total["自制"];
                            if (t != null)
                            {
                                r_need["最早发货日期"] = t;
                            }

                            r_need["参考数量"] = dec - total_z;
                            r_need["已转制令数"] = stock_total["已转制令数"];
                            r_need["已转工单数"] = stock_total["已转工单数"];


                            r_need["订单用量"] = stock_total["订单用量"];

                            r_need["拼板数量"] = stock_total["拼板数量"];
                            r_need["停用"] = stock_total["停用"];

                            r_need["班组编号"] = stock_total["b_班组编号"];
                            r_need["班组名称"] = stock_total["b_班组名称"];

                            ss.dtM.Rows.Add(r_need);
                            stock_total["总数"] = 0;
                        }
                        fun_dg(ss, stock_total["物料编码"].ToString(), dec - total_z, Convert.ToBoolean(stock_total["自制"]), t);
                    }
                }
            }
            return ss;
        }
        //20-2-23 计划采购一起算
        private static result_主计划 fun_dg_主计划_all(result_主计划 ss, string itemid, decimal dec_需求, bool bl_made, DateTime? t)
        {
            if (bl_made)
            {
                if (ss.Bom.Select(string.Format("产品编码='{0}'", itemid)).Length == 0)
                {
                    ss.str_log = ss.str_log + (itemid + "属性为自制但是没有bom;");
                }
            }
            DataRow[] br = ss.Bom.Select(string.Format("产品编码='{0}'and (子项自制=1 or 子项委外=1)", itemid));
            if (br.Length > 0) //找到需要自制的半成品 
            {
                decimal dec_缺 = dec_需求;
                foreach (DataRow brr in br)
                {
                    decimal dec = dec_缺 * Convert.ToDecimal(brr["数量"]); //这是自制半成品所需的量 

                    DataRow stock_total = ss.TotalCount.Rows.Find(brr["子项编码"]);

                    //DataRow[] stock_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                    if (stock_total == null)
                    {
                        throw new Exception(brr["子项编码"].ToString() + "在总表中未找到数据");
                    }

                    decimal total_z = Convert.ToDecimal(stock_total["总数"]);
                    stock_total["需求数量"] = Convert.ToDecimal(stock_total["需求数量"]) + dec;
                    if (total_z >= dec) //库存加未完成>需求数
                    {
                        stock_total["总数"] = total_z - dec;
                    }
                    else
                    {
                        DataRow[] fr = ss.dtM.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                        if (fr.Length > 0)
                        {
                            fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec - total_z;

                            if (fr[0]["最早发货日期"] != DBNull.Value && t != null && (Convert.ToDateTime(fr[0]["最早发货日期"]) > t || Convert.ToDateTime(fr[0]["最早发货日期"]) == Convert.ToDateTime("2011-1-1")))
                            {
                                fr[0]["最早发货日期"] = t;
                            }

                        }
                        else
                        {
                            DataRow r_need = ss.dtM.NewRow();
                            r_need["在制量"] = stock_total["在制量"];
                            r_need["未领量"] = stock_total["未领量"];
                            r_need["在途量"] = stock_total["在途量"];


                            r_need["仓库号"] = stock_total["默认仓库号"];
                            r_need["仓库名称"] = stock_total["仓库名称"];

                            r_need["工时"] = stock_total["工时"];
                            r_need["物料编码"] = stock_total["物料编码"];
                            r_need["物料名称"] = stock_total["物料名称"];
                            r_need["规格型号"] = stock_total["规格型号"];
                            r_need["存货分类"] = stock_total["存货分类"];
                            r_need["库存总数"] = stock_total["库存总数"];
                            r_need["受订量"] = stock_total["受订量"];
                            r_need["自制"] = stock_total["自制"];
                            if (t != null)
                            {
                                r_need["最早发货日期"] = t;
                            }

                            r_need["参考数量"] = dec - total_z;
                            r_need["已转制令数"] = stock_total["已转制令数"];
                            r_need["已转工单数"] = stock_total["已转工单数"];


                            r_need["订单用量"] = stock_total["订单用量"];

                            r_need["拼板数量"] = stock_total["拼板数量"];
                            r_need["停用"] = stock_total["停用"];

                            r_need["班组编号"] = stock_total["b_班组编号"];
                            r_need["班组名称"] = stock_total["b_班组名称"];
                            ss.dtM.Rows.Add(r_need);
                            stock_total["总数"] = 0;
                        }

                        fun_dg_主计划_all(ss, stock_total["物料编码"].ToString(), dec - total_z, Convert.ToBoolean(stock_total["自制"]), t);
                    }
                }
            }
            return ss;
        }

        //20-4-3 计划需求单计划采购一起算
        private static result_主计划 fun_dg_主计划_all_1(result_主计划 ss, string itemid, decimal dec_需求, bool bl_made, DateTime? t)
        {
            if (bl_made)
            {
                if (ss.Bom.Select(string.Format("产品编码='{0}'", itemid)).Length == 0)
                {
                    ss.str_log = ss.str_log + (itemid + "属性为自制但是没有bom;");
                }
            }
            DataRow[] br = ss.Bom.Select(string.Format("产品编码='{0}'and (子项自制=1 or 子项委外=1)", itemid));
            if (br.Length > 0) //找到需要自制的半成品 
            {
                decimal dec_缺 = dec_需求;
                foreach (DataRow brr in br)
                {
                    decimal dec = dec_缺 * Convert.ToDecimal(brr["数量"]); //这是自制半成品所需的量 

                    DataRow stock_total = ss.TotalCount.Rows.Find(brr["子项编码"]);

                    //DataRow[] stock_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                    if (stock_total == null)
                    {
                        throw new Exception(brr["子项编码"].ToString() + "在总表中未找到数据");
                    }

                    decimal total_z = Convert.ToDecimal(stock_total["总数"]);
                    stock_total["需求数量"] = Convert.ToDecimal(stock_total["需求数量"]) + dec;
                    if (total_z >= dec) //库存加未完成>需求数
                    {
                        stock_total["总数"] = total_z - dec;
                    }
                    else
                    {
                        DataRow[] fr = ss.dtM.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                        if (fr.Length > 0)
                        {
                            fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec - total_z;
                            //if (!(fr[0]["最早预计开工日期"] == DBNull.Value))
                            //{
                            //    if (str_最早开工日期 != "")
                            //    {
                            //        if (Convert.ToDateTime(fr[0]["最早预计开工日期"]) > Convert.ToDateTime(str_最早开工日期))
                            //        {
                            //            fr[0]["最早预计开工日期"] = Convert.ToDateTime(str_最早开工日期);
                            //        }
                            //    }
                            //}
                            if (fr[0]["最早发货日期"] != DBNull.Value && t != null && (Convert.ToDateTime(fr[0]["最早发货日期"]) > t || Convert.ToDateTime(fr[0]["最早发货日期"]) == Convert.ToDateTime("2011-1-1")))
                            {
                                fr[0]["最早发货日期"] = t;
                            }

                        }
                        else
                        {
                            DataRow r_need = ss.dtM.NewRow();
                            r_need["在制量"] = stock_total["在制量"];
                            r_need["未领量"] = stock_total["未领量"];
                            r_need["在途量"] = stock_total["在途量"];


                            r_need["仓库号"] = stock_total["默认仓库号"];
                            r_need["仓库名称"] = stock_total["仓库名称"];

                            r_need["工时"] = stock_total["工时"];
                            r_need["物料编码"] = stock_total["物料编码"];
                            r_need["物料名称"] = stock_total["物料名称"];
                            r_need["规格型号"] = stock_total["规格型号"];
                            r_need["存货分类"] = stock_total["存货分类"];
                            r_need["库存总数"] = stock_total["库存总数"];
                            r_need["受订量"] = stock_total["受订量"];
                            r_need["自制"] = stock_total["自制"];
                            if (t != null)
                            {
                                r_need["最早发货日期"] = t;
                            }
                            //if (str_最早开工日期 != "")
                            //{
                            //    r_need["最早预计开工日期"] = Convert.ToDateTime(str_最早开工日期);
                            //}

                            r_need["参考数量"] = dec - total_z;
                            r_need["已转制令数"] = stock_total["已转制令数"];
                            r_need["已转工单数"] = stock_total["已转工单数"];


                            r_need["订单用量"] = stock_total["订单用量"];

                            r_need["拼板数量"] = stock_total["拼板数量"];
                            r_need["停用"] = stock_total["停用"];
                            r_need["班组编号"] = stock_total["b_班组编号"];
                            r_need["班组名称"] = stock_total["b_班组名称"];
                            ss.dtM.Rows.Add(r_need);
                            stock_total["总数"] = 0;
                        }

                        fun_dg_主计划_all_1(ss, stock_total["物料编码"].ToString(), dec - total_z, Convert.ToBoolean(stock_total["自制"]), t);
                    }
                }
            }
            return ss;
        }


        private static result fun_dg_主计划(result ss, string itemid, decimal dec_需求, bool bl_made, DateTime t, string str_最早开工日期)
        {
            if (bl_made)
            {
                if (ss.Bom.Select(string.Format("产品编码='{0}'", itemid)).Length == 0)
                {
                    ss.str_log = ss.str_log + (itemid + "属性为自制但是没有bom;");
                }
            }
            DataRow[] br = ss.Bom.Select(string.Format("产品编码='{0}'and (子项自制=1 or 子项委外=1)", itemid));
            if (br.Length > 0) //找到需要自制的半成品 
            {
                decimal dec_缺 = dec_需求;
                foreach (DataRow brr in br)
                {
                    decimal dec = dec_缺 * Convert.ToDecimal(brr["数量"]); //这是自制半成品所需的量 

                    DataRow stock_total = ss.TotalCount.Rows.Find(brr["子项编码"]);

                    //DataRow[] stock_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                    if (stock_total == null)
                    {
                        throw new Exception(brr["子项编码"].ToString() + "在总表中未找到数据");
                    }

                    decimal total_z = Convert.ToDecimal(stock_total["总数"]);
                    stock_total["需求数量"] = Convert.ToDecimal(stock_total["需求数量"]) + dec;
                    if (total_z >= dec) //库存加未完成>需求数
                    {
                        stock_total["总数"] = total_z - dec;
                    }
                    else
                    {
                        DataRow[] fr = ss.dtM.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                        if (fr.Length > 0)
                        {
                            fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec - total_z;
                            if (!(fr[0]["最早预计开工日期"] == DBNull.Value))
                            {
                                if (str_最早开工日期 != "")
                                {
                                    if (Convert.ToDateTime(fr[0]["最早预计开工日期"]) > Convert.ToDateTime(str_最早开工日期))
                                    {
                                        fr[0]["最早预计开工日期"] = Convert.ToDateTime(str_最早开工日期);
                                    }
                                }
                            }
                            if (fr[0]["最早发货日期"] != DBNull.Value && (Convert.ToDateTime(fr[0]["最早发货日期"]) > t || Convert.ToDateTime(fr[0]["最早发货日期"]) == Convert.ToDateTime("2011-1-1")))
                            {
                                fr[0]["最早发货日期"] = t;
                            }

                        }
                        else
                        {
                            DataRow r_need = ss.dtM.NewRow();
                            r_need["在制量"] = stock_total["在制量"];
                            r_need["未领量"] = stock_total["未领量"];
                            r_need["在途量"] = stock_total["在途量"];


                            r_need["仓库号"] = stock_total["默认仓库号"];
                            r_need["仓库名称"] = stock_total["仓库名称"];

                            r_need["工时"] = stock_total["工时"];
                            r_need["物料编码"] = stock_total["物料编码"];
                            r_need["物料名称"] = stock_total["物料名称"];
                            r_need["规格型号"] = stock_total["规格型号"];
                            r_need["存货分类"] = stock_total["存货分类"];
                            r_need["库存总数"] = stock_total["库存总数"];
                            r_need["受订量"] = stock_total["受订量"];
                            r_need["自制"] = stock_total["自制"];
                            r_need["最早发货日期"] = t;
                            if (str_最早开工日期 != "")
                            {
                                r_need["最早预计开工日期"] = Convert.ToDateTime(str_最早开工日期);
                            }

                            r_need["参考数量"] = dec - total_z;
                            r_need["已转制令数"] = stock_total["已转制令数"];
                            r_need["已转工单数"] = stock_total["已转工单数"];


                            r_need["订单用量"] = stock_total["订单用量"];
                            r_need["拼板数量"] = stock_total["拼板数量"];
                            r_need["停用"] = stock_total["停用"];
                            r_need["班组编号"] = stock_total["b_班组编号"];
                            r_need["班组名称"] = stock_total["b_班组名称"];
                            ss.dtM.Rows.Add(r_need);
                            stock_total["总数"] = 0;
                        }
                        fun_dg_主计划(ss, stock_total["物料编码"].ToString(), dec - total_z, Convert.ToBoolean(stock_total["自制"]), t, str_最早开工日期);
                    }
                }
            }
            return ss;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="bl">true 表示 采购计划 false 表示 生产计划</param>
        /// <returns></returns>
        private static result calu(result ss, bool bl)
        {
            //先计算销售列表中的产品的欠缺数量
            // salelist 即为 dt_SaleOrder  为 销售明细汇总数据  物料  sum(数量) 
            if (!ss.TotalCount.Columns.Contains("订单用量"))
            {
                DataColumn dc = new DataColumn("订单用量", typeof(decimal));
                dc.DefaultValue = 0;
                ss.TotalCount.Columns.Add(dc);
            }
          
 
            foreach (DataRow dr in ss.salelist.Rows)
            {
                
                //太慢 半小时都没算完 19-11-6  TotalCount 里面加了主键 瞬秒
                //DataTable dt_x = new DataTable();
                //dt_x = ERPorg.Corg.billofM_带数量(dt_x, dr["物料编码"].ToString(), false);
                //foreach (DataRow rr in dt_x.Rows)
                //{
                //    decimal dec = Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(rr["数量"]);
                //    DataRow[] rrr = ss.TotalCount.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                //    rrr[0]["订单用量"] = dec + Convert.ToDecimal(rrr[0]["订单用量"]);
                //}


                string s = string.Format(@"  with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,bom_level ) as
 (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,1 as level from 基础数据物料BOM表 
   where 产品编码='{0}'and 优先级=1
   union all 
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型, convert(decimal(18, 6),a.数量*b.数量) as 数量,a.bom类型,b.bom_level+1  from 基础数据物料BOM表 a
   inner join temp_bom b on a.产品编码=b.子项编码 where  优先级=1
   ) 
      select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,wiptype,子项类型,sum(数量)数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号 as 子项规格 from  temp_bom a
  left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
  left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码
  group by 产品编码,fx.物料名称 ,子项编码,base.物料名称 ,wiptype,子项类型 ,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号", dr["物料编码"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon); //这个搜出来没有自身 
                                                                              // DataRow[] dr_self = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                DataRow r_total = ss.TotalCount.Rows.Find(dr["物料编码"]);
                //DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                r_total["订单用量"] = Convert.ToDecimal(dr["数量"]) + Convert.ToDecimal(r_total["订单用量"]);

                foreach (DataRow r in temp.Rows)
                {

                    DataRow f = ss.TotalCount.Rows.Find(r["子项编码"]);
                    //DataRow[] rrr = ss.TotalCount.Select(string.Format("物料编码='{0}'", r["子项编码"]));
                    f["订单用量"] = Math.Round(Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(r["数量"]), 6, MidpointRounding.AwayFromZero) + Convert.ToDecimal(f["订单用量"]);
                }


                decimal dec_订单数 = Convert.ToDecimal(dr["数量"]);

                try
                {
                    decimal total = Convert.ToDecimal(r_total["总数"]);
                    decimal kczs = Convert.ToDecimal(r_total["库存总数"]);
                    decimal wwcgds = Convert.ToDecimal(r_total["在制量"]);
                    decimal dec_Unclaimed = Convert.ToDecimal(r_total["未领量"]);
                    decimal dec_InTransit = Convert.ToDecimal(r_total["在途量"]);
                    if (total >= dec_订单数) //库存加未完成>需求数
                    {
                        r_total["总数"] = total - dec_订单数;

                        //r_total[0]["订单用量"]=r_total[0]["订单用量"];

                    }
                    else
                    {
                        DataRow r_need = ss.dtM.NewRow();
                        r_need["在制量"] = wwcgds;
                        r_need["物料编码"] = r_total["物料编码"];
                        r_need["仓库号"] = r_total["默认仓库号"];
                        r_need["仓库名称"] = r_total["仓库名称"];
                        r_need["在途量"] = dec_InTransit;
                        r_need["最早发货日期"] = dr["最早发货日期"];
                        r_need["未领量"] = dec_Unclaimed;
                        r_need["物料名称"] = r_total["物料名称"];
                        r_need["规格型号"] = r_total["规格型号"];
                        r_need["存货分类"] = r_total["存货分类"];
                        r_need["库存总数"] = kczs;
                        r_need["受订量"] = r_total["受订量"];
                        r_need["自制"] = r_total["自制"];
                        r_need["工时"] = r_total["工时"];
                        r_need["已转制令数"] = r_total["已转制令数"];
                        r_need["已转工单数"] = r_total["已转工单数"];
                        r_need["参考数量"] = dec_订单数 - total;
                        r_need["拼板数量"] = r_total["拼板数量"];

                        r_need["订单用量"] = r_total["订单用量"];
                        r_need["停用"] = r_total["停用"];
                        r_need["班组编号"] = r_total["b_班组编号"];
                        r_need["班组名称"] = r_total["b_班组名称"];
                        r_need["计划在途"] = r_total["计划在途"];




                        ss.dtM.Rows.Add(r_need);
                        r_total["总数"] = 0;
                    }
                    r_total["需求数量"] = Convert.ToDecimal(r_total["需求数量"]) + dec_订单数;


                }
                catch (Exception ex)
                {
                    throw new Exception(dr["物料编码"].ToString() + ex.Message);
                }
            }
            //5-23 存在 库存+在制-未领<0 的也是缺的
            DataView v = new DataView(ss.TotalCount);
            v.RowFilter = "总数<0";
            DataTable tx = v.ToTable();


            foreach (DataRow rr in tx.Rows)
            {
                string s = string.Format(@"with parent_bom(产品编码,子项编码,仓库号,仓库名称,bom_level ) as
                   (select  产品编码,子项编码,仓库号,仓库名称,1 as level from 基础数据物料BOM表 
                    where 子项编码='{0}'
                      union all 
                   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,b.bom_level+1  from 基础数据物料BOM表 a
                   inner join parent_bom b on a.子项编码=b.产品编码  )
                      select  * from parent_bom ", rr["物料编码"].ToString());
                DataTable dtz = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                // dtz = ERPorg.Corg.fun_GetFather(dtz, dr["物料编码"].ToString(), 0, true);
                //加入他自身
                DataRow rrr = dtz.NewRow();
                rrr["产品编码"] = rr["物料编码"].ToString();
                dtz.Rows.Add(rrr);
                DataView dv = new DataView(ss.salelist);
                if (dtz.Rows.Count > 0)
                {
                    s = string.Format("物料编码 in (");
                    foreach (DataRow xx in dtz.Rows)
                    {
                        s = s + "'" + xx["产品编码"].ToString() + "',";
                    }
                    s = s.Substring(0, s.Length - 1) + ")";

                    dv.RowFilter = s;
                    dv.Sort = "最早发货日期";

                }
                DataRow r_need = ss.dtM.NewRow();
                r_need["在制量"] = rr["在制量"];
                r_need["物料编码"] = rr["物料编码"];
                r_need["仓库号"] = rr["默认仓库号"];
                r_need["仓库名称"] = rr["仓库名称"];
                r_need["在途量"] = rr["在途量"];
                r_need["计划在途"] = rr["计划在途"];


                if (dv.Count > 0)
                {
                    r_need["最早发货日期"] = dv.ToTable().Rows[0]["最早发货日期"];
                }
                else
                {
                    r_need["最早发货日期"] = DBNull.Value; //原 2011-1-1  修改时间 2020-3-27
                }

                // r_need["最早发货日期"] = DBNull.Value; //原 2011-1-1  修改时间 2020-3-27
                r_need["未领量"] = rr["未领量"];
                r_need["物料名称"] = rr["物料名称"];
                r_need["规格型号"] = rr["规格型号"];
                r_need["存货分类"] = rr["存货分类"];
                r_need["库存总数"] = rr["库存总数"];
                r_need["受订量"] = rr["受订量"];
                r_need["自制"] = rr["自制"];
                r_need["工时"] = rr["工时"];
                r_need["已转制令数"] = rr["已转制令数"];
                r_need["已转工单数"] = rr["已转工单数"];
                r_need["参考数量"] = -Convert.ToDecimal(rr["总数"]);

                r_need["订单用量"] = rr["订单用量"];
                r_need["停用"] = rr["停用"];
                r_need["班组编号"] = rr["b_班组编号"];
                r_need["班组名称"] = rr["b_班组名称"];
                ss.dtM.Rows.Add(r_need);
                DataRow r_total = ss.TotalCount.Rows.Find(rr["物料编码"]);
                // DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", rr["物料编码"]));
                if (r_total == null)
                {
                    throw new Exception(rr["物料编码"].ToString() + "在总表中没有找到数据");
                }
                r_total["总数"] = 0;
                r_need["拼板数量"] = r_total["拼板数量"];

            }
            DataTable dtMcopy = ss.dtM.Copy();
            //fun_dg(dtMcopy);
            foreach (DataRow dr in dtMcopy.Rows)
            {
                if (dr["自制"].Equals(true))
                {
                    if (ss.Bom.Select(string.Format("产品编码='{0}'", dr["物料编码"])).Length == 0)
                    {
                        ss.str_log = ss.str_log + dr["物料编码"].ToString() + "属性为自制但是没有bom";
                    }
                }
                ///19-8-23 增加子项委外 =1  委外的可能不是自制属性 但是也许呀往下算 下面可能还有自制件
                DataRow[] br = ss.Bom.Select(string.Format("产品编码='{0}'and (子项自制=1 or 子项委外=1)", dr["物料编码"].ToString()));
                if (br.Length > 0) //找到需要自制的半成品 
                {
                    decimal dec_缺 = Convert.ToDecimal(dr["参考数量"].ToString());
                    foreach (DataRow brr in br)
                    {
                        decimal dec = dec_缺 * Convert.ToDecimal(brr["数量"]); //这是自制半成品所需的量 

                        DataRow stock_total = ss.TotalCount.Rows.Find(brr["子项编码"]);
                        //DataRow[] stock_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                        decimal total_z = Convert.ToDecimal(stock_total["总数"]);
                        if (stock_total == null)
                        {
                            throw new Exception(brr["物料编码"].ToString() + "在总表中没有找到数据");
                        }
                        stock_total["需求数量"] = Convert.ToDecimal(stock_total["需求数量"]) + dec; //记录需求数量



                        if (total_z >= dec) //库存加未完成>需求数
                        {
                            stock_total["总数"] = total_z - dec;
                        }
                        else
                        {
                            DataRow[] fr = ss.dtM.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                            if (fr.Length > 0)
                            {
                                fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec - total_z;
                                if (fr[0]["最早发货日期"] != DBNull.Value && dr["最早发货日期"] != DBNull.Value && (Convert.ToDateTime(fr[0]["最早发货日期"]) > Convert.ToDateTime(dr["最早发货日期"]) || Convert.ToDateTime(fr[0]["最早发货日期"]) == Convert.ToDateTime("2011-1-1")))
                                {

                                    fr[0]["最早发货日期"] = dr["最早发货日期"];
                                }
                            }
                            else
                            {
                                DataRow r_need = ss.dtM.NewRow();
                                r_need["在制量"] = stock_total["在制量"];
                                r_need["未领量"] = stock_total["未领量"];
                                r_need["在途量"] = stock_total["在途量"];

                                r_need["物料编码"] = stock_total["物料编码"];
                                r_need["仓库号"] = stock_total["默认仓库号"];
                                r_need["仓库名称"] = stock_total["仓库名称"];
                                r_need["物料名称"] = stock_total["物料名称"];
                                r_need["规格型号"] = stock_total["规格型号"];
                                r_need["存货分类"] = stock_total["存货分类"];
                                r_need["库存总数"] = stock_total["库存总数"];
                                r_need["受订量"] = stock_total["受订量"];
                                r_need["自制"] = stock_total["自制"];
                                r_need["最早发货日期"] = dr["最早发货日期"];
                                r_need["工时"] = stock_total["工时"];
                                r_need["已转制令数"] = stock_total["已转制令数"];
                                r_need["已转工单数"] = stock_total["已转工单数"];
                                r_need["参考数量"] = dec - total_z;

                                r_need["拼板数量"] = stock_total["拼板数量"];
                                r_need["计划在途"] = stock_total["计划在途"];

                                r_need["订单用量"] = stock_total["订单用量"];
                                r_need["停用"] = stock_total["停用"];
                                r_need["班组编号"] = stock_total["b_班组编号"];
                                r_need["班组名称"] = stock_total["b_班组名称"];
                                ss.dtM.Rows.Add(r_need);
                                stock_total["总数"] = 0;
                            }
                            DateTime? t = null;
                            if (dr["最早发货日期"] != DBNull.Value)
                            {
                                t = Convert.ToDateTime(dr["最早发货日期"]);
                            }
                            //缺的才需要继续往叶子节点递归 不缺不需要
                            fun_dg(ss, stock_total["物料编码"].ToString(), dec - total_z, Convert.ToBoolean(dr["自制"]), t);

                        }
                    }
                }
            }
            //到这里生产计划算完








            if (bl) //请求的是 采购计划结果
            {
                //继续往下算 
                DataTable dtM_PurchasePool = new DataTable();

                dtM_PurchasePool.Columns.Add("未领量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("在途量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("委外在途", typeof(decimal));
                dtM_PurchasePool.Columns.Add("最早发货日期", typeof(DateTime));
                dtM_PurchasePool.Columns.Add("仓库号");
                dtM_PurchasePool.Columns.Add("仓库名称");
                dtM_PurchasePool.Columns.Add("未发量", typeof(decimal));

                dtM_PurchasePool.Columns.Add("供应商编号");
                dtM_PurchasePool.Columns.Add("默认供应商");
                dtM_PurchasePool.Columns.Add("采购员");
                dtM_PurchasePool.Columns.Add("物料编码");
                dtM_PurchasePool.Columns.Add("物料名称");
                dtM_PurchasePool.Columns.Add("规格型号");
                dtM_PurchasePool.Columns.Add("库存总数", typeof(decimal));
                dtM_PurchasePool.Columns.Add("存货分类");
                dtM_PurchasePool.Columns.Add("参考数量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("受订量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("可购", typeof(bool));
                dtM_PurchasePool.Columns.Add("自制", typeof(bool));
                dtM_PurchasePool.Columns.Add("委外", typeof(bool));
                dtM_PurchasePool.Columns.Add("ECN", typeof(bool));
                dtM_PurchasePool.Columns.Add("最小包装", typeof(decimal));
                dtM_PurchasePool.Columns.Add("采购周期");
                dtM_PurchasePool.Columns.Add("已采未审", typeof(decimal));
                dtM_PurchasePool.Columns.Add("采购未送检", typeof(decimal));
                dtM_PurchasePool.Columns.Add("已送未检", typeof(decimal));
                dtM_PurchasePool.Columns.Add("已检未入", typeof(decimal));
                dtM_PurchasePool.Columns.Add("需求数量", typeof(decimal));
                //19-6-10 
                dtM_PurchasePool.Columns.Add("库存下限", typeof(decimal));
                dtM_PurchasePool.Columns.Add("订单用量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("订单缺料", typeof(decimal));
                //20-1-14
                dtM_PurchasePool.Columns.Add("停用", typeof(bool));
                dtM_PurchasePool.Columns.Add("计划在途", typeof(decimal));


                //20-1-8
                dtM_PurchasePool.Columns.Add("供应状态");

                DataColumn[] pk_cg = new DataColumn[1];
                pk_cg[0] = dtM_PurchasePool.Columns["物料编码"];
                dtM_PurchasePool.PrimaryKey = pk_cg;


                foreach (DataRow dr in ss.dtM.Rows) //因为这里dtM就是算出的 计划池  就是算出的计划要生产的 量比如父项A 要生产100 子项B只要生产 50 个 
                {                                //原材料 只要算一层 即是所缺的原材料
                    DataRow[] x = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (x.Length == 0)
                    {
                        DataRow r_need = dtM_PurchasePool.NewRow();
                        DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        if (Convert.ToBoolean(r_total[0]["可购"]) || Convert.ToBoolean(r_total[0]["委外"]))
                        {
                            r_need["未领量"] = dr["未领量"];
                            r_need["在途量"] = dr["在途量"]; ;
                            r_need["最早发货日期"] = dr["最早发货日期"];
                            r_need["物料编码"] = dr["物料编码"];
                            r_need["仓库号"] = dr["仓库号"];
                            r_need["仓库名称"] = dr["仓库名称"];
                            r_need["供应商编号"] = r_total[0]["供应商编号"];
                            r_need["默认供应商"] = r_total[0]["默认供应商"];
                            r_need["采购员"] = r_total[0]["采购员"];
                            r_need["委外在途"] = r_total[0]["委外在途"];
                            r_need["物料名称"] = dr["物料名称"];
                            r_need["规格型号"] = dr["规格型号"];
                            r_need["存货分类"] = dr["存货分类"];
                            r_need["库存总数"] = r_total[0]["库存总数"];
                            r_need["受订量"] = r_total[0]["受订量"];
                            r_need["自制"] = r_total[0]["自制"];
                            r_need["可购"] = r_total[0]["可购"];
                            r_need["委外"] = r_total[0]["委外"];
                            r_need["ECN"] = r_total[0]["ECN"];
                            r_need["未发量"] = r_total[0]["未发量"];

                            r_need["已采未审"] = r_total[0]["已采未审"];
                            r_need["采购未送检"] = r_total[0]["采购未送检"];
                            r_need["已送未检"] = r_total[0]["已送未检"];
                            r_need["已检未入"] = r_total[0]["已检未入"];
                            r_need["参考数量"] = dr["参考数量"];
                            //19-6-10
                            r_need["库存下限"] = r_total[0]["库存下限"];
                            r_need["采购周期"] = r_total[0]["采购周期"];
                            r_need["最小包装"] = r_total[0]["最小包装"];

                            r_need["订单用量"] = r_total[0]["订单用量"];
                            //20-1-8
                            r_need["供应状态"] = r_total[0]["供应状态"];
                            //20-1-14
                            r_need["停用"] = r_total[0]["停用"];
                            r_need["计划在途"] = r_total[0]["计划在途"];



                            //r_need["订单缺料"] = Convert.ToDecimal(r_total[0]["总数"]) - Convert.ToDecimal(dr["在途量"]);

                            dtM_PurchasePool.Rows.Add(r_need);
                        }
                    }

                    DataRow[] r_PPool = ss.Bom.Select(string.Format("产品编码='{0}'and 子项自制=0 and (子项可购=1 or 子项委外=1)", dr["物料编码"]));
                    foreach (DataRow rr in r_PPool)
                    {
                        ///19-8-14  8-23 生产上面 委外的也往下算了 那么这里 加这个限制 无误
                        if (!Convert.ToBoolean(rr["子项委外"]))
                        {
                            decimal dec_需 = Convert.ToDecimal(dr["参考数量"]) * Convert.ToDecimal(rr["数量"]); //父项所缺数*bom数量
                            DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                            decimal total = 0;
                            decimal kczs = 0;
                            decimal dec_wl = 0;
                            decimal dec_zt = 0;
                            if (r_total.Length == 0)
                            {
                                total = 0;
                                kczs = 0;
                                dec_wl = 0;
                                dec_zt = 0;
                            }
                            total = Convert.ToDecimal(r_total[0]["总数"]);
                            kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                            dec_wl = Convert.ToDecimal(r_total[0]["未领量"]);
                            dec_zt = Convert.ToDecimal(r_total[0]["在途量"]);
                            //decimal dec_n = 0;
                            r_total[0]["需求数量"] = Convert.ToDecimal(r_total[0]["需求数量"]) + dec_需;
                            if (total - dec_需 > 0) //不缺
                            {
                                r_total[0]["总数"] = total - dec_需;
                            }
                            else //缺了
                            {
                                DataRow[] fr = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                                if (fr.Length > 0)
                                {
                                    fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec_需 - total;
                                    if (fr[0]["最早发货日期"] != DBNull.Value && dr["最早发货日期"] != DBNull.Value && (Convert.ToDateTime(fr[0]["最早发货日期"]) > Convert.ToDateTime(dr["最早发货日期"]) || Convert.ToDateTime(fr[0]["最早发货日期"]) == Convert.ToDateTime("2011-1-1")))
                                    {
                                        fr[0]["最早发货日期"] = dr["最早发货日期"];
                                    }
                                }
                                else
                                {
                                    DataRow r_need = dtM_PurchasePool.NewRow();
                                    r_need["未领量"] = dec_wl;
                                    r_need["在途量"] = dec_zt;
                                    r_need["委外在途"] = Convert.ToDecimal(r_total[0]["委外在途"]);
                                    r_need["最早发货日期"] = dr["最早发货日期"];
                                    r_need["物料编码"] = r_total[0]["物料编码"];
                                    r_need["仓库号"] = r_total[0]["默认仓库号"];
                                    r_need["仓库名称"] = r_total[0]["仓库名称"];
                                    r_need["未发量"] = r_total[0]["未发量"];

                                    r_need["计划在途"] = r_total[0]["计划在途"];


                                    r_need["供应商编号"] = r_total[0]["供应商编号"];
                                    r_need["默认供应商"] = r_total[0]["默认供应商"];
                                    r_need["采购员"] = r_total[0]["采购员"];
                                    r_need["物料名称"] = r_total[0]["物料名称"];
                                    r_need["规格型号"] = r_total[0]["规格型号"];
                                    r_need["存货分类"] = r_total[0]["存货分类"];
                                    r_need["库存总数"] = kczs;
                                    r_need["受订量"] = r_total[0]["受订量"];
                                    r_need["自制"] = r_total[0]["自制"];
                                    r_need["委外"] = r_total[0]["委外"];
                                    r_need["ECN"] = r_total[0]["ECN"];

                                    r_need["可购"] = r_total[0]["可购"];
                                    r_need["已采未审"] = r_total[0]["已采未审"];
                                    r_need["采购未送检"] = r_total[0]["采购未送检"];
                                    r_need["已送未检"] = r_total[0]["已送未检"];
                                    r_need["已检未入"] = r_total[0]["已检未入"];
                                    r_need["库存下限"] = r_total[0]["库存下限"];
                                    r_need["采购周期"] = r_total[0]["采购周期"];
                                    r_need["最小包装"] = r_total[0]["最小包装"];
                                    //20-1-8
                                    r_need["供应状态"] = r_total[0]["供应状态"];
                                    //20-1-14
                                    r_need["停用"] = r_total[0]["停用"];

                                    r_need["订单用量"] = r_total[0]["订单用量"];
                                    //r_need["订单缺料"] = Convert.ToDecimal(r_total[0]["总数"]) - Convert.ToDecimal(dr["在途量"]);
                                    r_need["参考数量"] = dec_需 - total;
                                    dtM_PurchasePool.Rows.Add(r_need);
                                    r_total[0]["总数"] = 0;
                                }
                            }
                        }

                    }
                }
                //18-12-3 使用人提出 加入 不缺但是有在途的 方便她催料

                //19-6-10 加入安全库存  
                DataColumn dcc = new DataColumn("参考数量(含安全库存)", typeof(decimal));
                dcc.DefaultValue = 0;
                dtM_PurchasePool.Columns.Add(dcc);

                DataView dv_add = new DataView(ss.TotalCount);
                dv_add.RowFilter = "在途量>0 or 委外在途>0 or 总数<库存下限";
                DataTable dt_1 = dv_add.ToTable();
                foreach (DataRow dr in dt_1.Rows)
                {
                    DataRow[] rrr = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (rrr.Length > 0)
                    {
                        continue;
                    }
                    else
                    {
                        DataRow r_need = dtM_PurchasePool.NewRow();
                        r_need["未领量"] = dr["未领量"];
                        r_need["在途量"] = dr["在途量"];
                        r_need["仓库号"] = dr["默认仓库号"];
                        r_need["仓库名称"] = dr["仓库名称"];
                        r_need["未发量"] = dr["未发量"];

                        r_need["供应商编号"] = dr["供应商编号"];
                        r_need["默认供应商"] = dr["默认供应商"];
                        r_need["采购员"] = dr["采购员"];
                        r_need["委外在途"] = dr["委外在途"];
                        r_need["物料编码"] = dr["物料编码"];
                        r_need["物料名称"] = dr["物料名称"];
                        r_need["规格型号"] = dr["规格型号"];
                        r_need["存货分类"] = dr["存货分类"];
                        r_need["库存总数"] = dr["库存总数"];
                        r_need["受订量"] = dr["受订量"];
                        r_need["自制"] = dr["自制"];
                        r_need["委外"] = dr["委外"];
                        r_need["ECN"] = dr["ECN"];

                        r_need["可购"] = dr["可购"];
                        r_need["已采未审"] = dr["已采未审"];
                        r_need["采购未送检"] = dr["采购未送检"];
                        r_need["已送未检"] = dr["已送未检"];
                        r_need["已检未入"] = dr["已检未入"];
                        r_need["库存下限"] = dr["库存下限"];
                        r_need["采购周期"] = dr["采购周期"];
                        r_need["最小包装"] = dr["最小包装"];
                        r_need["计划在途"] = dr["计划在途"];


                        //20-1-14
                        r_need["停用"] = dr["停用"];
                        //19-6-10 改  
                        r_need["参考数量"] = 0;
                        decimal dec = Convert.ToDecimal(dr["库存下限"]) - Convert.ToDecimal(dr["总数"]);
                        r_need["参考数量(含安全库存)"] = dec > 0 ? dec : 0;
                        //19-11-06
                        r_need["订单用量"] = dr["订单用量"];
                        //20-1-8
                        r_need["供应状态"] = dr["供应状态"];
                        //r_need["订单缺料"] = Convert.ToDecimal(dr["总数"]) - Convert.ToDecimal(dr["在途量"]);
                        dtM_PurchasePool.Rows.Add(r_need);
                    }
                }




                foreach (DataRow dr in dtM_PurchasePool.Rows)
                {
                    decimal dec = Convert.ToDecimal(dr["库存下限"]);
                    decimal dec_cksl = Convert.ToDecimal(dr["参考数量"]);
                    if (dec_cksl > 0)
                    {
                        dr["参考数量(含安全库存)"] = dec_cksl + dec;
                    }
                    // else //这一块已经在上面2969-2970行处理了
                    //{

                    //}
                    //decimal x = dec_cksl - dec_T_total_总 + dec;
                    //    dr["参考数量(含安全库存)"] = x>0?x:0;
                    DataRow rr = ss.TotalCount.Rows.Find(dr["物料编码"]);
                    //DataRow[] rr = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    dr["需求数量"] = rr["需求数量"];
                    dr["订单缺料"] = Convert.ToDecimal(rr["总数"]) - Convert.ToDecimal(rr["在途量"]) - Convert.ToDecimal(dr["参考数量"]);
                }

                ss.dtM = dtM_PurchasePool;
            }
            else //计划 
            {
                foreach (DataRow dr in ss.salelist.Rows)
                {
                    DataRow[] xxx = ss.dtM.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (xxx.Length == 0)
                    {
                        DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        decimal total = Convert.ToDecimal(r_total[0]["总数"]);
                        decimal kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                        decimal wwcgds = Convert.ToDecimal(r_total[0]["在制量"]);
                        DataRow r_need = ss.dtM.NewRow();
                        r_need["在制量"] = wwcgds;
                        r_need["仓库号"] = r_total[0]["默认仓库号"];
                        r_need["仓库名称"] = r_total[0]["仓库名称"];
                        r_need["物料编码"] = r_total[0]["物料编码"];
                        r_need["物料名称"] = r_total[0]["物料名称"];
                        r_need["规格型号"] = r_total[0]["规格型号"];
                        r_need["存货分类"] = r_total[0]["存货分类"];
                        r_need["库存总数"] = kczs;
                        r_need["受订量"] = r_total[0]["受订量"];
                        r_need["自制"] = r_total[0]["自制"];
                        r_need["工时"] = r_total[0]["工时"];
                        r_need["停用"] = r_total[0]["停用"];
                        r_need["参考数量"] = 0;
                        r_need["已转制令数"] = r_total[0]["已转制令数"];
                        r_need["已转工单数"] = r_total[0]["已转工单数"];
                        r_need["计划在途"] = r_total[0]["计划在途"];


                        ss.dtM.Rows.Add(r_need);
                    }
                }
                ss.dtM.Columns.Add("总耗时", typeof(decimal));
                foreach (DataRow dr in ss.dtM.Rows)
                {
                    DataRow[] rr = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    dr["需求数量"] = rr[0]["需求数量"];
                    dr["总耗时"] = Convert.ToDecimal(dr["参考数量"]) * Convert.ToDecimal(dr["工时"]);
                }
            }


            return ss;
        }

        private static result calu1(result ss, bool bl)
        {
            //先计算销售列表中的产品的欠缺数量
            // salelist 即为 dt_SaleOrder  为 销售明细汇总数据  物料  sum(数量) 
            if (!ss.TotalCount.Columns.Contains("订单用量"))
            {
                DataColumn dc = new DataColumn("订单用量", typeof(decimal));
                dc.DefaultValue = 0;
                ss.TotalCount.Columns.Add(dc);
            }
            int all = ss.salelist.Rows.Count;
            int i = 0;
            foreach (DataRow dr in ss.salelist.Rows)
            {
                i++;
                //太慢 半小时都没算完 19-11-6  TotalCount 里面加了主键 瞬秒
                //DataTable dt_x = new DataTable();
                //dt_x = ERPorg.Corg.billofM_带数量(dt_x, dr["物料编码"].ToString(), false);
                //foreach (DataRow rr in dt_x.Rows)
                //{
                //    decimal dec = Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(rr["数量"]);
                //    DataRow[] rrr = ss.TotalCount.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                //    rrr[0]["订单用量"] = dec + Convert.ToDecimal(rrr[0]["订单用量"]);
                //}
                string s = string.Format(@"  with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,bom_level ) as
 (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,1 as level from 基础数据物料BOM表 
   where 产品编码='{0}'and 优先级=1
   union all 
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型, convert(decimal(18, 6),a.数量*b.数量) as 数量,a.bom类型,b.bom_level+1  from 基础数据物料BOM表 a
   inner join temp_bom b on a.产品编码=b.子项编码 where  优先级=1
   ) 
      select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,wiptype,子项类型,sum(数量)数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号 as 子项规格 from  temp_bom a
  left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
  left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码
  group by 产品编码,fx.物料名称 ,子项编码,base.物料名称 ,wiptype,子项类型 ,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号", dr["物料编码"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon); //这个搜出来没有自身 
                                                                              // DataRow[] dr_self = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                DataRow r_total = ss.TotalCount.Rows.Find(dr["物料编码"]);
                //DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                r_total["订单用量"] = Convert.ToDecimal(dr["数量"]) + Convert.ToDecimal(r_total["订单用量"]);

                foreach (DataRow r in temp.Rows)
                {

                    DataRow f = ss.TotalCount.Rows.Find(r["子项编码"]);
                    //DataRow[] rrr = ss.TotalCount.Select(string.Format("物料编码='{0}'", r["子项编码"]));
                    f["订单用量"] = Math.Round(Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(r["数量"]), 6, MidpointRounding.AwayFromZero) + Convert.ToDecimal(f["订单用量"]);
                }


                decimal dec_订单数 = Convert.ToDecimal(dr["数量"]);

                try
                {
                    decimal total = Convert.ToDecimal(r_total["总数"]);
                    decimal kczs = Convert.ToDecimal(r_total["库存总数"]);
                    decimal wwcgds = Convert.ToDecimal(r_total["在制量"]);
                    decimal dec_Unclaimed = Convert.ToDecimal(r_total["未领量"]);
                    decimal dec_InTransit = Convert.ToDecimal(r_total["在途量"]);
                    if (total >= dec_订单数) //库存加未完成>需求数
                    {
                        r_total["总数"] = total - dec_订单数;

                        //r_total[0]["订单用量"]=r_total[0]["订单用量"];

                    }
                    else
                    {
                        DataRow r_need = ss.dtM.NewRow();
                        r_need["在制量"] = wwcgds;
                        r_need["物料编码"] = r_total["物料编码"];
                        r_need["仓库号"] = r_total["默认仓库号"];
                        r_need["仓库名称"] = r_total["仓库名称"];
                        r_need["在途量"] = dec_InTransit;
                        r_need["最早发货日期"] = dr["最早发货日期"];
                        r_need["未领量"] = dec_Unclaimed;
                        r_need["物料名称"] = r_total["物料名称"];
                        r_need["规格型号"] = r_total["规格型号"];
                        r_need["存货分类"] = r_total["存货分类"];
                        r_need["库存总数"] = kczs;
                        r_need["受订量"] = r_total["受订量"];
                        r_need["自制"] = r_total["自制"];
                        r_need["工时"] = r_total["工时"];
                        r_need["已转制令数"] = r_total["已转制令数"];
                        r_need["已转工单数"] = r_total["已转工单数"];
                        r_need["参考数量"] = dec_订单数 - total;
                        r_need["拼板数量"] = r_total["拼板数量"];

                        r_need["订单用量"] = r_total["订单用量"];
                        r_need["停用"] = r_total["停用"];
                        r_need["班组编号"] = r_total["b_班组编号"];
                        r_need["班组名称"] = r_total["b_班组名称"];

                        r_need["计划在途"] = r_total["计划在途"];



                        ss.dtM.Rows.Add(r_need);
                        r_total["总数"] = 0;
                    }
                    r_total["需求数量"] = Convert.ToDecimal(r_total["需求数量"]) + dec_订单数;


                }
                catch (Exception ex)
                {
                    throw new Exception(dr["物料编码"].ToString() + ex.Message);
                }
            }
            //5-23 存在 库存+在制-未领<0 的也是缺的
            DataView v = new DataView(ss.TotalCount);
            v.RowFilter = "总数<0";
            DataTable tx = v.ToTable();


            foreach (DataRow rr in tx.Rows)
            {
                string s = string.Format(@"with parent_bom(产品编码,子项编码,仓库号,仓库名称,bom_level ) as
                   (select  产品编码,子项编码,仓库号,仓库名称,1 as level from 基础数据物料BOM表 
                    where 子项编码='{0}'
                      union all 
                   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,b.bom_level+1  from 基础数据物料BOM表 a
                   inner join parent_bom b on a.子项编码=b.产品编码  )
                      select  * from parent_bom ", rr["物料编码"].ToString());
                DataTable dtz = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                // dtz = ERPorg.Corg.fun_GetFather(dtz, dr["物料编码"].ToString(), 0, true);
                //加入他自身
                DataRow rrr = dtz.NewRow();
                rrr["产品编码"] = rr["物料编码"].ToString();
                dtz.Rows.Add(rrr);
                DataView dv = new DataView(ss.salelist);
                if (dtz.Rows.Count > 0)
                {
                    s = string.Format("物料编码 in (");
                    foreach (DataRow xx in dtz.Rows)
                    {
                        s = s + "'" + xx["产品编码"].ToString() + "',";
                    }
                    s = s.Substring(0, s.Length - 1) + ")";

                    dv.RowFilter = s;
                    dv.Sort = "最早发货日期";

                }
                DataRow r_need = ss.dtM.NewRow();
                r_need["在制量"] = rr["在制量"];
                r_need["物料编码"] = rr["物料编码"];
                r_need["仓库号"] = rr["默认仓库号"];
                r_need["仓库名称"] = rr["仓库名称"];
                r_need["在途量"] = rr["在途量"];
                r_need["计划在途"] = rr["计划在途"];


                if (dv.Count > 0)
                {
                    r_need["最早发货日期"] = dv.ToTable().Rows[0]["最早发货日期"];
                }
                else
                {
                    r_need["最早发货日期"] = DBNull.Value; //原 2011-1-1  修改时间 2020-3-27
                }

                // r_need["最早发货日期"] = DBNull.Value; //原 2011-1-1  修改时间 2020-3-27
                r_need["未领量"] = rr["未领量"];
                r_need["物料名称"] = rr["物料名称"];
                r_need["规格型号"] = rr["规格型号"];
                r_need["存货分类"] = rr["存货分类"];
                r_need["库存总数"] = rr["库存总数"];
                r_need["受订量"] = rr["受订量"];
                r_need["自制"] = rr["自制"];
                r_need["工时"] = rr["工时"];
                r_need["已转制令数"] = rr["已转制令数"];
                r_need["已转工单数"] = rr["已转工单数"];
                r_need["参考数量"] = -Convert.ToDecimal(rr["总数"]);

                r_need["订单用量"] = rr["订单用量"];
                r_need["停用"] = rr["停用"];
                r_need["班组编号"] = rr["b_班组编号"];
                r_need["班组名称"] = rr["b_班组名称"];
                ss.dtM.Rows.Add(r_need);
                DataRow r_total = ss.TotalCount.Rows.Find(rr["物料编码"]);
                // DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", rr["物料编码"]));
                if (r_total == null)
                {
                    throw new Exception(rr["物料编码"].ToString() + "在总表中没有找到数据");
                }
                r_total["总数"] = 0;
                r_need["拼板数量"] = r_total["拼板数量"];

            }
            DataTable dtMcopy = ss.dtM.Copy();
            //fun_dg(dtMcopy);
            foreach (DataRow dr in dtMcopy.Rows)
            {
                if (dr["自制"].Equals(true))
                {
                    if (ss.Bom.Select(string.Format("产品编码='{0}'", dr["物料编码"])).Length == 0)
                    {
                        ss.str_log = ss.str_log + dr["物料编码"].ToString() + "属性为自制但是没有bom";
                    }
                }
                ///19-8-23 增加子项委外 =1  委外的可能不是自制属性 但是也许呀往下算 下面可能还有自制件
                DataRow[] br = ss.Bom.Select(string.Format("产品编码='{0}'and (子项自制=1 or 子项委外=1)", dr["物料编码"].ToString()));
                if (br.Length > 0) //找到需要自制的半成品 
                {
                    decimal dec_缺 = Convert.ToDecimal(dr["参考数量"].ToString());
                    foreach (DataRow brr in br)
                    {
                        decimal dec = dec_缺 * Convert.ToDecimal(brr["数量"]); //这是自制半成品所需的量 

                        DataRow stock_total = ss.TotalCount.Rows.Find(brr["子项编码"]);
                        //DataRow[] stock_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                        decimal total_z = Convert.ToDecimal(stock_total["总数"]);
                        if (stock_total == null)
                        {
                            throw new Exception(brr["物料编码"].ToString() + "在总表中没有找到数据");
                        }
                        stock_total["需求数量"] = Convert.ToDecimal(stock_total["需求数量"]) + dec; //记录需求数量



                        if (total_z >= dec) //库存加未完成>需求数
                        {
                            stock_total["总数"] = total_z - dec;
                        }
                        else
                        {
                            DataRow[] fr = ss.dtM.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                            if (fr.Length > 0)
                            {
                                fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec - total_z;
                                if (fr[0]["最早发货日期"] != DBNull.Value && dr["最早发货日期"] != DBNull.Value && (Convert.ToDateTime(fr[0]["最早发货日期"]) > Convert.ToDateTime(dr["最早发货日期"]) || Convert.ToDateTime(fr[0]["最早发货日期"]) == Convert.ToDateTime("2011-1-1")))
                                {

                                    fr[0]["最早发货日期"] = dr["最早发货日期"];
                                }
                            }
                            else
                            {
                                DataRow r_need = ss.dtM.NewRow();
                                r_need["在制量"] = stock_total["在制量"];
                                r_need["未领量"] = stock_total["未领量"];
                                r_need["在途量"] = stock_total["在途量"];
                                r_need["计划在途"] = stock_total["计划在途"];


                                r_need["物料编码"] = stock_total["物料编码"];
                                r_need["仓库号"] = stock_total["默认仓库号"];
                                r_need["仓库名称"] = stock_total["仓库名称"];
                                r_need["物料名称"] = stock_total["物料名称"];
                                r_need["规格型号"] = stock_total["规格型号"];
                                r_need["存货分类"] = stock_total["存货分类"];
                                r_need["库存总数"] = stock_total["库存总数"];
                                r_need["受订量"] = stock_total["受订量"];
                                r_need["自制"] = stock_total["自制"];
                                r_need["最早发货日期"] = dr["最早发货日期"];
                                r_need["工时"] = stock_total["工时"];
                                r_need["已转制令数"] = stock_total["已转制令数"];
                                r_need["已转工单数"] = stock_total["已转工单数"];
                                r_need["参考数量"] = dec - total_z;

                                r_need["拼板数量"] = stock_total["拼板数量"];

                                r_need["订单用量"] = stock_total["订单用量"];
                                r_need["停用"] = stock_total["停用"];
                                r_need["班组编号"] = stock_total["b_班组编号"];
                                r_need["班组名称"] = stock_total["b_班组名称"];
                                ss.dtM.Rows.Add(r_need);
                                stock_total["总数"] = 0;
                            }
                            DateTime? t = null;
                            if (dr["最早发货日期"] != DBNull.Value)
                            {
                                t = Convert.ToDateTime(dr["最早发货日期"]);
                            }
                            //缺的才需要继续往叶子节点递归 不缺不需要
                            fun_dg(ss, stock_total["物料编码"].ToString(), dec - total_z, Convert.ToBoolean(dr["自制"]), t);

                        }
                    }
                }
            }
            //到这里生产计划算完








            if (bl) //请求的是 采购计划结果
            {
                //继续往下算 
                DataTable dtM_PurchasePool = new DataTable();

                dtM_PurchasePool.Columns.Add("未领量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("在途量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("委外在途", typeof(decimal));
                dtM_PurchasePool.Columns.Add("最早发货日期", typeof(DateTime));
                dtM_PurchasePool.Columns.Add("仓库号");
                dtM_PurchasePool.Columns.Add("仓库名称");
                dtM_PurchasePool.Columns.Add("未发量", typeof(decimal));
                //20-6-3
                dtM_PurchasePool.Columns.Add("计划在途", typeof(decimal));


                dtM_PurchasePool.Columns.Add("供应商编号");
                dtM_PurchasePool.Columns.Add("默认供应商");
                dtM_PurchasePool.Columns.Add("采购员");
                dtM_PurchasePool.Columns.Add("物料编码");
                dtM_PurchasePool.Columns.Add("物料名称");
                dtM_PurchasePool.Columns.Add("规格型号");
                dtM_PurchasePool.Columns.Add("库存总数", typeof(decimal));
                dtM_PurchasePool.Columns.Add("存货分类");
                dtM_PurchasePool.Columns.Add("参考数量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("受订量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("可购", typeof(bool));
                dtM_PurchasePool.Columns.Add("自制", typeof(bool));
                dtM_PurchasePool.Columns.Add("委外", typeof(bool));
                dtM_PurchasePool.Columns.Add("ECN", typeof(bool));
                dtM_PurchasePool.Columns.Add("最小包装", typeof(decimal));
                dtM_PurchasePool.Columns.Add("采购周期");
                dtM_PurchasePool.Columns.Add("已采未审", typeof(decimal));
                dtM_PurchasePool.Columns.Add("采购未送检", typeof(decimal));
                dtM_PurchasePool.Columns.Add("已送未检", typeof(decimal));
                dtM_PurchasePool.Columns.Add("已检未入", typeof(decimal));
                dtM_PurchasePool.Columns.Add("需求数量", typeof(decimal));
                //19-6-10 
                dtM_PurchasePool.Columns.Add("库存下限", typeof(decimal));
                dtM_PurchasePool.Columns.Add("订单用量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("订单缺料", typeof(decimal));
                //20-1-14
                dtM_PurchasePool.Columns.Add("停用", typeof(bool));

                //20-1-8
                dtM_PurchasePool.Columns.Add("供应状态");

                DataColumn[] pk_cg = new DataColumn[1];
                pk_cg[0] = dtM_PurchasePool.Columns["物料编码"];
                dtM_PurchasePool.PrimaryKey = pk_cg;


                foreach (DataRow dr in ss.dtM.Rows) //因为这里dtM就是算出的 计划池  就是算出的计划要生产的 量比如父项A 要生产100 子项B只要生产 50 个 
                {                                //原材料 只要算一层 即是所缺的原材料
                    DataRow[] x = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (x.Length == 0)
                    {
                        DataRow r_need = dtM_PurchasePool.NewRow();
                        DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        if (Convert.ToBoolean(r_total[0]["可购"]) || Convert.ToBoolean(r_total[0]["委外"]))
                        {
                            r_need["未领量"] = dr["未领量"];
                            r_need["在途量"] = dr["在途量"]; ;
                            r_need["最早发货日期"] = dr["最早发货日期"];
                            r_need["物料编码"] = dr["物料编码"];
                            r_need["仓库号"] = dr["仓库号"];
                            r_need["仓库名称"] = dr["仓库名称"];
                            r_need["供应商编号"] = r_total[0]["供应商编号"];
                            r_need["默认供应商"] = r_total[0]["默认供应商"];
                            r_need["采购员"] = r_total[0]["采购员"];
                            r_need["委外在途"] = r_total[0]["委外在途"];
                            r_need["物料名称"] = dr["物料名称"];
                            r_need["规格型号"] = dr["规格型号"];
                            r_need["存货分类"] = dr["存货分类"];
                            r_need["库存总数"] = r_total[0]["库存总数"];
                            r_need["受订量"] = r_total[0]["受订量"];
                            r_need["自制"] = r_total[0]["自制"];
                            r_need["可购"] = r_total[0]["可购"];
                            r_need["委外"] = r_total[0]["委外"];
                            r_need["ECN"] = r_total[0]["ECN"];
                            r_need["未发量"] = r_total[0]["未发量"];

                            r_need["已采未审"] = r_total[0]["已采未审"];
                            r_need["采购未送检"] = r_total[0]["采购未送检"];
                            r_need["已送未检"] = r_total[0]["已送未检"];
                            r_need["已检未入"] = r_total[0]["已检未入"];
                            r_need["参考数量"] = dr["参考数量"];
                            //19-6-10
                            r_need["库存下限"] = r_total[0]["库存下限"];
                            r_need["采购周期"] = r_total[0]["采购周期"];
                            r_need["最小包装"] = r_total[0]["最小包装"];

                            r_need["订单用量"] = r_total[0]["订单用量"];
                            //20-1-8
                            r_need["供应状态"] = r_total[0]["供应状态"];
                            //20-1-14
                            r_need["停用"] = r_total[0]["停用"];


                            //r_need["订单缺料"] = Convert.ToDecimal(r_total[0]["总数"]) - Convert.ToDecimal(dr["在途量"]);

                            dtM_PurchasePool.Rows.Add(r_need);
                        }
                    }

                    DataRow[] r_PPool = ss.Bom.Select(string.Format("产品编码='{0}'and 子项自制=0 and (子项可购=1 or 子项委外=1)", dr["物料编码"]));
                    foreach (DataRow rr in r_PPool)
                    {
                        ///19-8-14  8-23 生产上面 委外的也往下算了 那么这里 加这个限制 无误
                        if (!Convert.ToBoolean(rr["子项委外"]))
                        {
                            decimal dec_需 = Convert.ToDecimal(dr["参考数量"]) * Convert.ToDecimal(rr["数量"]); //父项所缺数*bom数量
                            DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                            decimal total = 0;
                            decimal kczs = 0;
                            decimal dec_wl = 0;
                            decimal dec_zt = 0;
                            if (r_total.Length == 0)
                            {
                                total = 0;
                                kczs = 0;
                                dec_wl = 0;
                                dec_zt = 0;
                            }
                            total = Convert.ToDecimal(r_total[0]["总数"]);
                            kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                            dec_wl = Convert.ToDecimal(r_total[0]["未领量"]);
                            dec_zt = Convert.ToDecimal(r_total[0]["在途量"]);
                            //decimal dec_n = 0;
                            r_total[0]["需求数量"] = Convert.ToDecimal(r_total[0]["需求数量"]) + dec_需;
                            if (total - dec_需 > 0) //不缺
                            {
                                r_total[0]["总数"] = total - dec_需;
                            }
                            else //缺了
                            {
                                DataRow[] fr = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                                if (fr.Length > 0)
                                {
                                    fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec_需 - total;
                                    if (fr[0]["最早发货日期"] != DBNull.Value && dr["最早发货日期"] != DBNull.Value && (Convert.ToDateTime(fr[0]["最早发货日期"]) > Convert.ToDateTime(dr["最早发货日期"]) || Convert.ToDateTime(fr[0]["最早发货日期"]) == Convert.ToDateTime("2011-1-1")))
                                    {
                                        fr[0]["最早发货日期"] = dr["最早发货日期"];
                                    }
                                }
                                else
                                {
                                    DataRow r_need = dtM_PurchasePool.NewRow();
                                    r_need["未领量"] = dec_wl;
                                    r_need["在途量"] = dec_zt;
                                    r_need["委外在途"] = Convert.ToDecimal(r_total[0]["委外在途"]);
                                    r_need["最早发货日期"] = dr["最早发货日期"];
                                    r_need["物料编码"] = r_total[0]["物料编码"];
                                    r_need["仓库号"] = r_total[0]["默认仓库号"];
                                    r_need["仓库名称"] = r_total[0]["仓库名称"];
                                    r_need["未发量"] = r_total[0]["未发量"];

                                    r_need["供应商编号"] = r_total[0]["供应商编号"];
                                    r_need["默认供应商"] = r_total[0]["默认供应商"];
                                    r_need["采购员"] = r_total[0]["采购员"];
                                    r_need["物料名称"] = r_total[0]["物料名称"];
                                    r_need["规格型号"] = r_total[0]["规格型号"];
                                    r_need["存货分类"] = r_total[0]["存货分类"];
                                    r_need["库存总数"] = kczs;
                                    r_need["受订量"] = r_total[0]["受订量"];
                                    r_need["自制"] = r_total[0]["自制"];
                                    r_need["委外"] = r_total[0]["委外"];
                                    r_need["ECN"] = r_total[0]["ECN"];

                                    r_need["可购"] = r_total[0]["可购"];
                                    r_need["已采未审"] = r_total[0]["已采未审"];
                                    r_need["采购未送检"] = r_total[0]["采购未送检"];
                                    r_need["已送未检"] = r_total[0]["已送未检"];
                                    r_need["计划在途"] = r_total[0]["计划在途"];
                                    r_need["已检未入"] = r_total[0]["已检未入"];
                                    r_need["库存下限"] = r_total[0]["库存下限"];
                                    r_need["采购周期"] = r_total[0]["采购周期"];
                                    r_need["最小包装"] = r_total[0]["最小包装"];
                                    //20-1-8
                                    r_need["供应状态"] = r_total[0]["供应状态"];
                                    //20-1-14
                                    r_need["停用"] = r_total[0]["停用"];

                                    r_need["订单用量"] = r_total[0]["订单用量"];
                                    //r_need["订单缺料"] = Convert.ToDecimal(r_total[0]["总数"]) - Convert.ToDecimal(dr["在途量"]);
                                    r_need["参考数量"] = dec_需 - total;
                                    dtM_PurchasePool.Rows.Add(r_need);
                                    r_total[0]["总数"] = 0;
                                }
                            }
                        }

                    }
                }
                //18-12-3 使用人提出 加入 不缺但是有在途的 方便她催料

                //19-6-10 加入安全库存  
                DataColumn dcc = new DataColumn("参考数量(含安全库存)", typeof(decimal));
                dcc.DefaultValue = 0;
                dtM_PurchasePool.Columns.Add(dcc);

                DataView dv_add = new DataView(ss.TotalCount);
                dv_add.RowFilter = "在途量>0 or 委外在途>0 or 总数<库存下限";
                DataTable dt_1 = dv_add.ToTable();
                foreach (DataRow dr in dt_1.Rows)
                {
                    DataRow[] rrr = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (rrr.Length > 0)
                    {
                        continue;
                    }
                    else
                    {
                        DataRow r_need = dtM_PurchasePool.NewRow();
                        r_need["未领量"] = dr["未领量"];
                        r_need["在途量"] = dr["在途量"];
                        r_need["仓库号"] = dr["默认仓库号"];
                        r_need["仓库名称"] = dr["仓库名称"];
                        r_need["未发量"] = dr["未发量"];

                        r_need["供应商编号"] = dr["供应商编号"];
                        r_need["默认供应商"] = dr["默认供应商"];
                        r_need["采购员"] = dr["采购员"];
                        r_need["委外在途"] = dr["委外在途"];
                        r_need["物料编码"] = dr["物料编码"];
                        r_need["物料名称"] = dr["物料名称"];
                        r_need["规格型号"] = dr["规格型号"];
                        r_need["存货分类"] = dr["存货分类"];
                        r_need["库存总数"] = dr["库存总数"];
                        r_need["受订量"] = dr["受订量"];
                        r_need["自制"] = dr["自制"];
                        r_need["委外"] = dr["委外"];
                        r_need["ECN"] = dr["ECN"];

                        r_need["可购"] = dr["可购"];
                        r_need["已采未审"] = dr["已采未审"];
                        r_need["采购未送检"] = dr["采购未送检"];
                        r_need["已送未检"] = dr["已送未检"];

                        r_need["计划在途"] = dr["计划在途"];

                        r_need["已检未入"] = dr["已检未入"];
                        r_need["库存下限"] = dr["库存下限"];
                        r_need["采购周期"] = dr["采购周期"];
                        r_need["最小包装"] = dr["最小包装"];

                        //20-1-14
                        r_need["停用"] = dr["停用"];
                        //19-6-10 改  
                        r_need["参考数量"] = 0;
                        decimal dec = Convert.ToDecimal(dr["库存下限"]) - Convert.ToDecimal(dr["总数"]);
                        r_need["参考数量(含安全库存)"] = dec > 0 ? dec : 0;
                        //19-11-06
                        r_need["订单用量"] = dr["订单用量"];
                        //20-1-8
                        r_need["供应状态"] = dr["供应状态"];
                        //r_need["订单缺料"] = Convert.ToDecimal(dr["总数"]) - Convert.ToDecimal(dr["在途量"]);
                        dtM_PurchasePool.Rows.Add(r_need);
                    }
                }


                //有订单用量但不缺料的
                DataRow[] r_total1 = ss.TotalCount.Select("订单用量>0");
                foreach (DataRow dr in r_total1)
                {
                    DataRow[] rrr = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (rrr.Length > 0)
                    {
                        continue;
                    }
                    else
                    {
                        DataRow r_need = dtM_PurchasePool.NewRow();
                        r_need["未领量"] = dr["未领量"];
                        r_need["在途量"] = dr["在途量"];
                        r_need["仓库号"] = dr["默认仓库号"];
                        r_need["仓库名称"] = dr["仓库名称"];
                        r_need["未发量"] = dr["未发量"];
                        r_need["供应商编号"] = dr["供应商编号"];
                        r_need["默认供应商"] = dr["默认供应商"];
                        r_need["采购员"] = dr["采购员"];
                        r_need["委外在途"] = dr["委外在途"];
                        r_need["物料编码"] = dr["物料编码"];
                        r_need["物料名称"] = dr["物料名称"];
                        r_need["规格型号"] = dr["规格型号"];
                        r_need["存货分类"] = dr["存货分类"];
                        r_need["库存总数"] = dr["库存总数"];
                        r_need["受订量"] = dr["受订量"];
                        r_need["自制"] = dr["自制"];
                        r_need["委外"] = dr["委外"];
                        r_need["ECN"] = dr["ECN"];
                        r_need["可购"] = dr["可购"];
                        r_need["已采未审"] = dr["已采未审"];
                        r_need["采购未送检"] = dr["采购未送检"];
                        r_need["已送未检"] = dr["已送未检"];
                        r_need["已检未入"] = dr["已检未入"];
                        r_need["库存下限"] = dr["库存下限"];
                        r_need["采购周期"] = dr["采购周期"];
                        r_need["最小包装"] = dr["最小包装"];
                        r_need["计划在途"] = dr["计划在途"];


                        //20-1-14
                        r_need["停用"] = dr["停用"];
                        //19-6-10 改  
                        r_need["参考数量"] = 0;
                        decimal dec = Convert.ToDecimal(dr["库存下限"]) - Convert.ToDecimal(dr["总数"]);
                        r_need["参考数量(含安全库存)"] = dec > 0 ? dec : 0;
                        //19-11-06
                        r_need["订单用量"] = dr["订单用量"];
                        //20-1-8
                        r_need["供应状态"] = dr["供应状态"];
                        //r_need["订单缺料"] = Convert.ToDecimal(dr["总数"]) - Convert.ToDecimal(dr["在途量"]);
                        dtM_PurchasePool.Rows.Add(r_need);
                    }
                }

                foreach (DataRow dr in dtM_PurchasePool.Rows)
                {
                    decimal dec = Convert.ToDecimal(dr["库存下限"]);
                    decimal dec_cksl = Convert.ToDecimal(dr["参考数量"]);
                    if (dec_cksl > 0)
                    {
                        dr["参考数量(含安全库存)"] = dec_cksl + dec;
                    }
                    // else //这一块已经在上面2969-2970行处理了
                    //{

                    //}
                    //decimal x = dec_cksl - dec_T_total_总 + dec;
                    //    dr["参考数量(含安全库存)"] = x>0?x:0;
                    DataRow rr = ss.TotalCount.Rows.Find(dr["物料编码"]);
                    //DataRow[] rr = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    dr["需求数量"] = rr["需求数量"];
                    dr["订单缺料"] = Convert.ToDecimal(rr["总数"]) - Convert.ToDecimal(rr["在途量"]) - Convert.ToDecimal(dr["参考数量"]);
                }

                ss.dtM = dtM_PurchasePool;
            }
            else //计划 
            {
                foreach (DataRow dr in ss.salelist.Rows)
                {
                    DataRow[] xxx = ss.dtM.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (xxx.Length == 0)
                    {
                        DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        decimal total = Convert.ToDecimal(r_total[0]["总数"]);
                        decimal kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                        decimal wwcgds = Convert.ToDecimal(r_total[0]["在制量"]);
                        DataRow r_need = ss.dtM.NewRow();
                        r_need["在制量"] = wwcgds;
                        r_need["仓库号"] = r_total[0]["默认仓库号"];
                        r_need["仓库名称"] = r_total[0]["仓库名称"];
                        r_need["物料编码"] = r_total[0]["物料编码"];
                        r_need["物料名称"] = r_total[0]["物料名称"];
                        r_need["规格型号"] = r_total[0]["规格型号"];
                        r_need["存货分类"] = r_total[0]["存货分类"];
                        r_need["库存总数"] = kczs;
                        r_need["受订量"] = r_total[0]["受订量"];
                        r_need["自制"] = r_total[0]["自制"];
                        r_need["工时"] = r_total[0]["工时"];
                        r_need["停用"] = r_total[0]["停用"];
                        r_need["参考数量"] = 0;
                        r_need["已转制令数"] = r_total[0]["已转制令数"];
                        r_need["已转工单数"] = r_total[0]["已转工单数"];
                        ss.dtM.Rows.Add(r_need);
                    }
                }
                ss.dtM.Columns.Add("总耗时", typeof(decimal));
                foreach (DataRow dr in ss.dtM.Rows)
                {
                    DataRow[] rr = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    dr["需求数量"] = rr[0]["需求数量"];
                    dr["总耗时"] = Convert.ToDecimal(dr["参考数量"]) * Convert.ToDecimal(dr["工时"]);
                }
            }


            return ss;
        }


        #endregion
        //20-2-23 直接计算计划池和采购池结果
        private static result_主计划 caluu_主计划_all(result_主计划 ss)
        {
            //先计算销售列表中的产品的欠缺数量
            // salelist 即为 dt_SaleOrder  为 销售明细汇总数据  物料  sum(数量) 
            if (!ss.TotalCount.Columns.Contains("订单用量"))
            {
                DataColumn dc = new DataColumn("订单用量", typeof(decimal));
                dc.DefaultValue = 0;
                ss.TotalCount.Columns.Add(dc);
            }
            int all = ss.salelist.Rows.Count;
            int i = 0;
            foreach (DataRow dr in ss.salelist.Rows)
            {
                i++;
                //太慢 半小时都没算完 19-11-6  TotalCount 里面加了主键 瞬秒
                //DataTable dt_x = new DataTable();
                //dt_x = ERPorg.Corg.billofM_带数量(dt_x, dr["物料编码"].ToString(), false);
                //foreach (DataRow rr in dt_x.Rows)
                //{
                //    decimal dec = Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(rr["数量"]);
                //    DataRow[] rrr = ss.TotalCount.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                //    rrr[0]["订单用量"] = dec + Convert.ToDecimal(rrr[0]["订单用量"]);
                //}


                string s = string.Format(@"  with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,bom_level ) as
 (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,1 as level from 基础数据物料BOM表 
   where 产品编码='{0}'and 优先级=1
   union all 
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型, convert(decimal(18, 6),a.数量*b.数量) as 数量,a.bom类型,b.bom_level+1  from 基础数据物料BOM表 a
   inner join temp_bom b on a.产品编码=b.子项编码 
   where   优先级=1
   ) 
      select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,wiptype,子项类型,sum(数量)数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号 as 子项规格 from  temp_bom a
  left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
  left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码
  group by 产品编码,fx.物料名称 ,子项编码,base.物料名称 ,wiptype,子项类型 ,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号", dr["物料编码"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon); //这个搜出来没有自身 
                                                                              // DataRow[] dr_self = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                DataRow r_total = ss.TotalCount.Rows.Find(dr["物料编码"]);
                //DataRow[] r_total = ss.TotabvmblCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                r_total["订单用量"] = Convert.ToDecimal(dr["数量"]) + Convert.ToDecimal(r_total["订单用量"]);

                foreach (DataRow r in temp.Rows)
                {

                    DataRow f = ss.TotalCount.Rows.Find(r["子项编码"]);
                    //DataRow[] rrr = ss.TotalCount.Select(string.Format("物料编码='{0}'", r["子项编码"]));
                    f["订单用量"] = Math.Round(Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(r["数量"]), 6, MidpointRounding.AwayFromZero) + Convert.ToDecimal(f["订单用量"]);
                }


                decimal dec_订单数 = Convert.ToDecimal(dr["数量"]);

                try
                {
                    decimal total = Convert.ToDecimal(r_total["总数"]);
                    decimal kczs = Convert.ToDecimal(r_total["库存总数"]);
                    decimal wwcgds = Convert.ToDecimal(r_total["在制量"]);
                    decimal dec_Unclaimed = Convert.ToDecimal(r_total["未领量"]);
                    decimal dec_InTransit = Convert.ToDecimal(r_total["在途量"]);
                    if (total >= dec_订单数) //库存加未完成>需求数
                    {
                        r_total["总数"] = total - dec_订单数;

                        //r_total[0]["订单用量"]=r_total[0]["订单用量"];

                    }
                    else
                    {
                        DataRow r_need = ss.dtM.NewRow();
                        r_need["在制量"] = wwcgds;
                        r_need["物料编码"] = r_total["物料编码"];
                        r_need["仓库号"] = r_total["默认仓库号"];
                        r_need["仓库名称"] = r_total["仓库名称"];
                        r_need["在途量"] = dec_InTransit;
                        r_need["最早发货日期"] = dr["最早发货日期"];
                        r_need["未领量"] = dec_Unclaimed;
                        r_need["物料名称"] = r_total["物料名称"];
                        r_need["规格型号"] = r_total["规格型号"];
                        r_need["存货分类"] = r_total["存货分类"];
                        r_need["库存总数"] = kczs;
                        r_need["受订量"] = r_total["受订量"];
                        r_need["自制"] = r_total["自制"];
                        r_need["工时"] = r_total["工时"];
                        r_need["已转制令数"] = r_total["已转制令数"];
                        r_need["已转工单数"] = r_total["已转工单数"];
                        r_need["参考数量"] = dec_订单数 - total;
                        r_need["拼板数量"] = r_total["拼板数量"];

                        r_need["订单用量"] = r_total["订单用量"];
                        r_need["停用"] = r_total["停用"];
                        r_need["班组编号"] = r_total["b_班组编号"];
                        r_need["班组名称"] = r_total["b_班组名称"];
                        ss.dtM.Rows.Add(r_need);
                        r_total["总数"] = 0;
                    }
                    r_total["需求数量"] = Convert.ToDecimal(r_total["需求数量"]) + dec_订单数;


                }
                catch (Exception ex)
                {
                    throw new Exception(dr["物料编码"].ToString() + ex.Message);
                }
            }
            //5-23 存在 库存+在制-未领<0 的也是缺的
            DataView v = new DataView(ss.TotalCount);
            v.RowFilter = "总数<0";
            DataTable tx = v.ToTable();
            foreach (DataRow rr in tx.Rows)
            {
                string s = string.Format(@"with parent_bom(产品编码,子项编码,仓库号,仓库名称,bom_level ) as
                   (select  产品编码,子项编码,仓库号,仓库名称,1 as level from 基础数据物料BOM表 
                    where 子项编码='{0}'
                      union all 
                   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,b.bom_level+1  from 基础数据物料BOM表 a
                   inner join parent_bom b on a.子项编码=b.产品编码  )
                      select  * from parent_bom ", rr["物料编码"].ToString());
                DataTable dtz = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                // dtz = ERPorg.Corg.fun_GetFather(dtz, dr["物料编码"].ToString(), 0, true);
                //加入他自身
                DataRow rrr = dtz.NewRow();
                rrr["产品编码"] = rr["物料编码"].ToString();
                dtz.Rows.Add(rrr);
                DataView dv = new DataView(ss.salelist);
                if (dtz.Rows.Count > 0)
                {
                    s = string.Format("物料编码 in (");
                    foreach (DataRow xx in dtz.Rows)
                    {
                        s = s + "'" + xx["产品编码"].ToString() + "',";
                    }
                    s = s.Substring(0, s.Length - 1) + ")";

                    dv.RowFilter = s;
                    dv.Sort = "最早发货日期";

                }
                DataRow r_need = ss.dtM.NewRow();
                r_need["在制量"] = rr["在制量"];
                r_need["物料编码"] = rr["物料编码"];
                r_need["仓库号"] = rr["默认仓库号"];
                r_need["仓库名称"] = rr["仓库名称"];
                r_need["在途量"] = rr["在途量"];
                if (dv.Count > 0)
                {
                    r_need["最早发货日期"] = dv.ToTable().Rows[0]["最早发货日期"];
                }
                else
                {
                    r_need["最早发货日期"] = DBNull.Value; //原 2011-1-1  修改时间 2020-3-27
                }

                // r_need["最早发货日期"] = DBNull.Value; //原 2011-1-1  修改时间 2020-3-27
                r_need["未领量"] = rr["未领量"];
                r_need["物料名称"] = rr["物料名称"];
                r_need["规格型号"] = rr["规格型号"];
                r_need["存货分类"] = rr["存货分类"];
                r_need["库存总数"] = rr["库存总数"];
                r_need["受订量"] = rr["受订量"];
                r_need["自制"] = rr["自制"];
                r_need["工时"] = rr["工时"];
                r_need["已转制令数"] = rr["已转制令数"];
                r_need["已转工单数"] = rr["已转工单数"];
                r_need["参考数量"] = -Convert.ToDecimal(rr["总数"]);

                r_need["订单用量"] = rr["订单用量"];
                r_need["停用"] = rr["停用"];
                r_need["班组编号"] = rr["b_班组编号"];
                r_need["班组名称"] = rr["b_班组名称"];
                ss.dtM.Rows.Add(r_need);
                DataRow r_total = ss.TotalCount.Rows.Find(rr["物料编码"]);
                // DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", rr["物料编码"]));
                if (r_total == null)
                {
                    throw new Exception(rr["物料编码"].ToString() + "在总表中没有找到数据");

                }
                r_total["总数"] = 0;
                r_need["拼板数量"] = r_total["拼板数量"];

            }
            DataTable dtMcopy = ss.dtM.Copy();
            //fun_dg(dtMcopy);
            foreach (DataRow dr in dtMcopy.Rows)
            {
                if (dr["自制"].Equals(true))
                {
                    if (ss.Bom.Select(string.Format("产品编码='{0}'", dr["物料编码"])).Length == 0)
                    {
                        ss.str_log = ss.str_log + dr["物料编码"].ToString() + "属性为自制但是没有bom";
                    }
                }
                ///19-8-23 增加子项委外 =1  委外的可能不是自制属性 但是也需要往下算 下面可能还有自制件
                DataRow[] br = ss.Bom.Select(string.Format("产品编码='{0}'and (子项自制=1 or 子项委外=1 )", dr["物料编码"].ToString()));
                if (br.Length > 0) //找到需要自制的半成品 
                {
                    decimal dec_缺 = Convert.ToDecimal(dr["参考数量"].ToString());
                    foreach (DataRow brr in br)
                    {
                        decimal dec = dec_缺 * Convert.ToDecimal(brr["数量"]); //这是自制半成品所需的量 

                        DataRow stock_total = ss.TotalCount.Rows.Find(brr["子项编码"]);
                        //DataRow[] stock_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                        decimal total_z = Convert.ToDecimal(stock_total["总数"]);
                        if (stock_total == null)
                        {
                            throw new Exception(brr["物料编码"].ToString() + "在总表中没有找到数据");
                        }
                        stock_total["需求数量"] = Convert.ToDecimal(stock_total["需求数量"]) + dec; //记录需求数量

                        if (total_z >= dec) //库存加未完成>需求数
                        {
                            stock_total["总数"] = total_z - dec;
                        }
                        else
                        {
                            DataRow[] fr = ss.dtM.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                            if (fr.Length > 0)
                            {
                                fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec - total_z;
                                if (fr[0]["最早发货日期"] != DBNull.Value && dr["最早发货日期"] != DBNull.Value && (Convert.ToDateTime(fr[0]["最早发货日期"]) > Convert.ToDateTime(dr["最早发货日期"]) || Convert.ToDateTime(fr[0]["最早发货日期"]) == Convert.ToDateTime("2011-1-1")))
                                {
                                    fr[0]["最早发货日期"] = dr["最早发货日期"];
                                }

                            }
                            else
                            {
                                DataRow r_need = ss.dtM.NewRow();
                                r_need["在制量"] = stock_total["在制量"];
                                r_need["未领量"] = stock_total["未领量"];
                                r_need["在途量"] = stock_total["在途量"];
                                r_need["物料编码"] = stock_total["物料编码"];
                                r_need["仓库号"] = stock_total["默认仓库号"];
                                r_need["仓库名称"] = stock_total["仓库名称"];
                                r_need["物料名称"] = stock_total["物料名称"];
                                r_need["规格型号"] = stock_total["规格型号"];
                                r_need["存货分类"] = stock_total["存货分类"];
                                r_need["库存总数"] = stock_total["库存总数"];
                                r_need["受订量"] = stock_total["受订量"];
                                r_need["自制"] = stock_total["自制"];
                                r_need["最早发货日期"] = dr["最早发货日期"];
                                r_need["工时"] = stock_total["工时"];
                                r_need["已转制令数"] = stock_total["已转制令数"];
                                r_need["已转工单数"] = stock_total["已转工单数"];
                                r_need["参考数量"] = dec - total_z;
                                r_need["拼板数量"] = stock_total["拼板数量"];
                                r_need["订单用量"] = stock_total["订单用量"];
                                r_need["停用"] = stock_total["停用"];
                                r_need["班组编号"] = stock_total["b_班组编号"];
                                r_need["班组名称"] = stock_total["b_班组名称"];
                                ss.dtM.Rows.Add(r_need);
                                stock_total["总数"] = 0;
                            }

                            //缺的才需要继续往叶子节点递归 不缺不需要

                            DateTime? t = null;
                            if (dr["最早发货日期"] != DBNull.Value)
                            {
                                t = Convert.ToDateTime(dr["最早发货日期"]);
                            }

                            fun_dg_主计划_all(ss, stock_total["物料编码"].ToString(), dec - total_z, Convert.ToBoolean(dr["自制"]), t);

                        }
                    }
                }
            }
            //到这里生产计划算完

            foreach (DataRow dr in ss.salelist.Rows)
            {
                DataRow[] xxx = ss.dtM.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                if (xxx.Length == 0)
                {
                    DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    decimal total = Convert.ToDecimal(r_total[0]["总数"]);
                    decimal kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                    decimal wwcgds = Convert.ToDecimal(r_total[0]["在制量"]);
                    DataRow r_need = ss.dtM.NewRow();
                    r_need["在制量"] = wwcgds;
                    r_need["仓库号"] = r_total[0]["默认仓库号"];
                    r_need["仓库名称"] = r_total[0]["仓库名称"];
                    r_need["物料编码"] = r_total[0]["物料编码"];
                    r_need["物料名称"] = r_total[0]["物料名称"];
                    r_need["规格型号"] = r_total[0]["规格型号"];
                    r_need["存货分类"] = r_total[0]["存货分类"];
                    r_need["库存总数"] = kczs;
                    r_need["受订量"] = r_total[0]["受订量"];
                    r_need["自制"] = r_total[0]["自制"];
                    r_need["工时"] = r_total[0]["工时"];
                    r_need["订单用量"] = r_total[0]["订单用量"];

                    r_need["参考数量"] = 0;
                    r_need["已转制令数"] = r_total[0]["已转制令数"];
                    r_need["已转工单数"] = r_total[0]["已转工单数"];
                    r_need["停用"] = r_total[0]["停用"];
                    ss.dtM.Rows.Add(r_need);
                }
            }
            ss.dtM.Columns.Add("总耗时", typeof(decimal));
            foreach (DataRow dr in ss.dtM.Rows)
            {
                DataRow[] rr = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                dr["需求数量"] = rr[0]["需求数量"];
                dr["总耗时"] = Convert.ToDecimal(dr["参考数量"]) * Convert.ToDecimal(dr["工时"]);
            }


            //20-2-23 计划和采购都要计算 
            //if (bl) //请求的是 采购计划结果
            //{
            //继续往下算 
            DataTable dtM_PurchasePool = new DataTable();

            dtM_PurchasePool.Columns.Add("未领量", typeof(decimal));
            dtM_PurchasePool.Columns.Add("在途量", typeof(decimal));
            dtM_PurchasePool.Columns.Add("委外在途", typeof(decimal));
            dtM_PurchasePool.Columns.Add("最早发货日期", typeof(DateTime));
            //dtM_PurchasePool.Columns.Add("最早预计开工日期", typeof(DateTime));
            dtM_PurchasePool.Columns.Add("仓库号");
            dtM_PurchasePool.Columns.Add("仓库名称");
            dtM_PurchasePool.Columns.Add("未发量", typeof(decimal));

            dtM_PurchasePool.Columns.Add("供应商编号");
            dtM_PurchasePool.Columns.Add("默认供应商");
            dtM_PurchasePool.Columns.Add("采购员");
            dtM_PurchasePool.Columns.Add("物料编码");
            dtM_PurchasePool.Columns.Add("物料名称");
            dtM_PurchasePool.Columns.Add("规格型号");
            dtM_PurchasePool.Columns.Add("库存总数", typeof(decimal));
            dtM_PurchasePool.Columns.Add("存货分类");
            dtM_PurchasePool.Columns.Add("参考数量", typeof(decimal));
            dtM_PurchasePool.Columns.Add("受订量", typeof(decimal));
            dtM_PurchasePool.Columns.Add("可购", typeof(bool));
            dtM_PurchasePool.Columns.Add("自制", typeof(bool));
            dtM_PurchasePool.Columns.Add("委外", typeof(bool));
            dtM_PurchasePool.Columns.Add("ECN", typeof(bool));
            dtM_PurchasePool.Columns.Add("最小包装", typeof(decimal));
            dtM_PurchasePool.Columns.Add("采购周期");
            dtM_PurchasePool.Columns.Add("已采未审", typeof(decimal));
            dtM_PurchasePool.Columns.Add("采购未送检", typeof(decimal));
            dtM_PurchasePool.Columns.Add("已送未检", typeof(decimal));
            dtM_PurchasePool.Columns.Add("已检未入", typeof(decimal));
            dtM_PurchasePool.Columns.Add("需求数量", typeof(decimal));
            //19-6-10 
            dtM_PurchasePool.Columns.Add("库存下限", typeof(decimal));
            dtM_PurchasePool.Columns.Add("订单用量", typeof(decimal));
            dtM_PurchasePool.Columns.Add("订单缺料", typeof(decimal));
            //20-1-8
            dtM_PurchasePool.Columns.Add("供应状态");
            //20-1-14
            dtM_PurchasePool.Columns.Add("停用", typeof(bool));

            DataColumn[] pk_cg = new DataColumn[1];
            pk_cg[0] = dtM_PurchasePool.Columns["物料编码"];
            dtM_PurchasePool.PrimaryKey = pk_cg;


            foreach (DataRow dr in ss.dtM.Rows) //因为这里dtM就是算出的 计划池  就是算出的计划要生产的 量比如父项A 要生产100 子项B只要生产 50 个 
            {                                //原材料 只要算一层 即是所缺的原材料
                DataRow[] x = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                if (x.Length == 0)
                {
                    DataRow r_need = dtM_PurchasePool.NewRow();
                    DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (Convert.ToBoolean(r_total[0]["可购"]) || Convert.ToBoolean(r_total[0]["委外"]))
                    {
                        r_need["未领量"] = dr["未领量"];
                        r_need["在途量"] = dr["在途量"]; ;
                        r_need["最早发货日期"] = dr["最早发货日期"];
                        //if (!(dr["最早预计开工日期"] == DBNull.Value))
                        //{
                        //    r_need["最早预计开工日期"] = dr["最早预计开工日期"];
                        //}
                        r_need["物料编码"] = dr["物料编码"];
                        r_need["仓库号"] = dr["仓库号"];
                        r_need["仓库名称"] = dr["仓库名称"];
                        r_need["供应商编号"] = r_total[0]["供应商编号"];
                        r_need["默认供应商"] = r_total[0]["默认供应商"];
                        r_need["采购员"] = r_total[0]["采购员"];
                        r_need["委外在途"] = r_total[0]["委外在途"];
                        r_need["物料名称"] = dr["物料名称"];
                        r_need["规格型号"] = dr["规格型号"];
                        r_need["存货分类"] = dr["存货分类"];
                        r_need["库存总数"] = r_total[0]["库存总数"];
                        r_need["受订量"] = r_total[0]["受订量"];
                        r_need["自制"] = r_total[0]["自制"];
                        r_need["可购"] = r_total[0]["可购"];
                        r_need["委外"] = r_total[0]["委外"];
                        r_need["ECN"] = r_total[0]["ECN"];
                        r_need["未发量"] = r_total[0]["未发量"];

                        r_need["已采未审"] = r_total[0]["已采未审"];
                        r_need["采购未送检"] = r_total[0]["采购未送检"];
                        r_need["已送未检"] = r_total[0]["已送未检"];
                        r_need["已检未入"] = r_total[0]["已检未入"];
                        r_need["参考数量"] = dr["参考数量"];
                        //19-6-10
                        r_need["库存下限"] = r_total[0]["库存下限"];
                        r_need["采购周期"] = r_total[0]["采购周期"];
                        r_need["最小包装"] = r_total[0]["最小包装"];

                        r_need["订单用量"] = r_total[0]["订单用量"];
                        //20-1-8
                        r_need["供应状态"] = r_total[0]["供应状态"];
                        //20-1-14
                        r_need["停用"] = r_total[0]["停用"];
                        //r_need["订单缺料"] = Convert.ToDecimal(r_total[0]["总数"]) - Convert.ToDecimal(dr["在途量"]);

                        dtM_PurchasePool.Rows.Add(r_need);
                    }
                }

                DataRow[] r_PPool = ss.Bom.Select(string.Format("产品编码='{0}'and 子项自制=0 and (子项可购=1 or 子项委外=1)", dr["物料编码"]));
                foreach (DataRow rr in r_PPool)
                {
                    ///19-8-14  8-23 生产上面 委外的也往下算了 那么这里 加这个限制 无误
                    if (!Convert.ToBoolean(rr["子项委外"]))
                    {
                        decimal dec_需 = Convert.ToDecimal(dr["参考数量"]) * Convert.ToDecimal(rr["数量"]); //父项所缺数*bom数量
                        DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                        decimal total = 0;
                        decimal kczs = 0;
                        decimal dec_wl = 0;
                        decimal dec_zt = 0;
                        if (r_total.Length == 0)
                        {
                            total = 0;
                            kczs = 0;
                            dec_wl = 0;
                            dec_zt = 0;
                        }
                        total = Convert.ToDecimal(r_total[0]["总数"]);
                        kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                        dec_wl = Convert.ToDecimal(r_total[0]["未领量"]);
                        dec_zt = Convert.ToDecimal(r_total[0]["在途量"]);
                        //decimal dec_n = 0;
                        r_total[0]["需求数量"] = Convert.ToDecimal(r_total[0]["需求数量"]) + dec_需;
                        if (total - dec_需 > 0) //不缺
                        {
                            r_total[0]["总数"] = total - dec_需;
                        }
                        else //缺了
                        {
                            DataRow[] fr = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                            if (fr.Length > 0)
                            {
                                fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec_需 - total;
                                //if (!(dr["最早预计开工日期"] == DBNull.Value))
                                //{
                                //    if (Convert.ToDateTime(fr[0]["最早预计开工日期"]) > Convert.ToDateTime(dr["最早预计开工日期"]))
                                //    {
                                //        fr[0]["最早预计开工日期"] = dr["最早预计开工日期"];
                                //    }
                                //}
                                if (fr[0]["最早发货日期"] != DBNull.Value && dr["最早发货日期"] != DBNull.Value && (Convert.ToDateTime(fr[0]["最早发货日期"]) > Convert.ToDateTime(dr["最早发货日期"]) || Convert.ToDateTime(fr[0]["最早发货日期"]) == Convert.ToDateTime("2011-1-1")))
                                {
                                    fr[0]["最早发货日期"] = dr["最早发货日期"];
                                }
                            }
                            else
                            {
                                DataRow r_need = dtM_PurchasePool.NewRow();
                                r_need["未领量"] = dec_wl;
                                r_need["在途量"] = dec_zt;
                                r_need["委外在途"] = Convert.ToDecimal(r_total[0]["委外在途"]);
                                r_need["最早发货日期"] = dr["最早发货日期"];
                                //if (!(dr["最早预计开工日期"] == DBNull.Value))
                                //{
                                //    r_need["最早预计开工日期"] = dr["最早预计开工日期"];
                                //}
                                r_need["物料编码"] = r_total[0]["物料编码"];
                                r_need["仓库号"] = r_total[0]["默认仓库号"];
                                r_need["仓库名称"] = r_total[0]["仓库名称"];
                                r_need["未发量"] = r_total[0]["未发量"];

                                r_need["供应商编号"] = r_total[0]["供应商编号"];
                                r_need["默认供应商"] = r_total[0]["默认供应商"];
                                r_need["采购员"] = r_total[0]["采购员"];
                                r_need["物料名称"] = r_total[0]["物料名称"];
                                r_need["规格型号"] = r_total[0]["规格型号"];
                                r_need["存货分类"] = r_total[0]["存货分类"];
                                r_need["库存总数"] = kczs;
                                r_need["受订量"] = r_total[0]["受订量"];
                                r_need["自制"] = r_total[0]["自制"];
                                r_need["委外"] = r_total[0]["委外"];
                                r_need["ECN"] = r_total[0]["ECN"];

                                r_need["可购"] = r_total[0]["可购"];
                                r_need["已采未审"] = r_total[0]["已采未审"];
                                r_need["采购未送检"] = r_total[0]["采购未送检"];
                                r_need["已送未检"] = r_total[0]["已送未检"];
                                r_need["已检未入"] = r_total[0]["已检未入"];
                                r_need["库存下限"] = r_total[0]["库存下限"];
                                r_need["采购周期"] = r_total[0]["采购周期"];
                                r_need["最小包装"] = r_total[0]["最小包装"];
                                //20-1-8
                                r_need["供应状态"] = r_total[0]["供应状态"];
                                r_need["订单用量"] = r_total[0]["订单用量"];
                                r_need["停用"] = r_total[0]["停用"];
                                //r_need["订单缺料"] = Convert.ToDecimal(r_total[0]["总数"]) - Convert.ToDecimal(dr["在途量"]);
                                r_need["参考数量"] = dec_需 - total;
                                dtM_PurchasePool.Rows.Add(r_need);
                                r_total[0]["总数"] = 0;
                            }
                        }
                    }

                }
            }
            //18-12-3 使用人提出 加入 不缺但是有在途的 方便她催料

            //19-6-10 加入安全库存  
            DataColumn dcc = new DataColumn("参考数量(含安全库存)", typeof(decimal));
            dcc.DefaultValue = 0;
            dtM_PurchasePool.Columns.Add(dcc);

            DataView dv_add = new DataView(ss.TotalCount);
            dv_add.RowFilter = "在途量>0 or 委外在途>0 or 总数<库存下限";
            DataTable dt_1 = dv_add.ToTable();
            foreach (DataRow dr in dt_1.Rows)
            {
                DataRow[] rrr = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                if (rrr.Length > 0)
                {
                    continue;
                }
                else
                {
                    DataRow r_need = dtM_PurchasePool.NewRow();
                    r_need["未领量"] = dr["未领量"];
                    r_need["在途量"] = dr["在途量"];
                    r_need["仓库号"] = dr["默认仓库号"];
                    r_need["仓库名称"] = dr["仓库名称"];
                    r_need["未发量"] = dr["未发量"];

                    r_need["供应商编号"] = dr["供应商编号"];
                    r_need["默认供应商"] = dr["默认供应商"];
                    r_need["采购员"] = dr["采购员"];
                    r_need["委外在途"] = dr["委外在途"];
                    r_need["物料编码"] = dr["物料编码"];
                    r_need["物料名称"] = dr["物料名称"];
                    r_need["规格型号"] = dr["规格型号"];
                    r_need["存货分类"] = dr["存货分类"];
                    r_need["库存总数"] = dr["库存总数"];
                    r_need["受订量"] = dr["受订量"];
                    r_need["自制"] = dr["自制"];
                    r_need["委外"] = dr["委外"];
                    r_need["ECN"] = dr["ECN"];

                    r_need["可购"] = dr["可购"];
                    r_need["已采未审"] = dr["已采未审"];
                    r_need["采购未送检"] = dr["采购未送检"];
                    r_need["已送未检"] = dr["已送未检"];
                    r_need["已检未入"] = dr["已检未入"];
                    r_need["库存下限"] = dr["库存下限"];
                    r_need["采购周期"] = dr["采购周期"];
                    r_need["最小包装"] = dr["最小包装"];
                    //19-6-10 改  
                    r_need["参考数量"] = 0;
                    decimal dec = Convert.ToDecimal(dr["库存下限"]) - Convert.ToDecimal(dr["总数"]);
                    r_need["参考数量(含安全库存)"] = dec > 0 ? dec : 0;
                    //19-11-06
                    r_need["订单用量"] = dr["订单用量"];
                    //20-1-8
                    r_need["供应状态"] = dr["供应状态"];
                    //20-1-14
                    r_need["停用"] = dr["停用"];
                    //r_need["订单缺料"] = Convert.ToDecimal(dr["总数"]) - Convert.ToDecimal(dr["在途量"]);
                    dtM_PurchasePool.Rows.Add(r_need);
                }
            }

            foreach (DataRow dr in dtM_PurchasePool.Rows)
            {
                decimal dec = Convert.ToDecimal(dr["库存下限"]);
                decimal dec_cksl = Convert.ToDecimal(dr["参考数量"]);
                if (dec_cksl > 0)
                {
                    dr["参考数量(含安全库存)"] = dec_cksl + dec;
                }
                // else //这一块已经在上面2969-2970行处理了
                //{

                //}
                //decimal x = dec_cksl - dec_T_total_总 + dec;
                //    dr["参考数量(含安全库存)"] = x>0?x:0;
                DataRow rr = ss.TotalCount.Rows.Find(dr["物料编码"]);
                //DataRow[] rr = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                dr["需求数量"] = rr["需求数量"];
                dr["订单缺料"] = Convert.ToDecimal(rr["总数"]) - Convert.ToDecimal(rr["在途量"]) - Convert.ToDecimal(dr["参考数量"]);
            }
            ss.dtM_采购池 = dtM_PurchasePool;
            //}
            //else //计划 
            //{

            //}
            return ss;
        }

        //20-4-3 计划需求单计算计划池和采购池结果
        private static result_主计划 caluu_主计划_all_1(result_主计划 ss)
        {
            //先计算销售列表中的产品的欠缺数量
            // salelist 即为 dt_SaleOrder  为 销售明细汇总数据  物料  sum(数量) 
            if (!ss.TotalCount.Columns.Contains("订单用量"))
            {
                DataColumn dc = new DataColumn("订单用量", typeof(decimal));
                dc.DefaultValue = 0;
                ss.TotalCount.Columns.Add(dc);
            }
            int all = ss.salelist.Rows.Count;
            int i = 0;
            foreach (DataRow dr in ss.salelist.Rows)
            {
                i++;
                //太慢 半小时都没算完 19-11-6  TotalCount 里面加了主键 瞬秒
                //DataTable dt_x = new DataTable();
                //dt_x = ERPorg.Corg.billofM_带数量(dt_x, dr["物料编码"].ToString(), false);
                //foreach (DataRow rr in dt_x.Rows)
                //{
                //    decimal dec = Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(rr["数量"]);
                //    DataRow[] rrr = ss.TotalCount.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                //    rrr[0]["订单用量"] = dec + Convert.ToDecimal(rrr[0]["订单用量"]);
                //}


                string s = string.Format(@"  with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,bom_level ) as
 (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,1 as level from 基础数据物料BOM表 
   where 产品编码='{0}'and 优先级=1
   union all 
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型, convert(decimal(18, 6),a.数量*b.数量) as 数量,a.bom类型,b.bom_level+1  from 基础数据物料BOM表 a
   inner join temp_bom b on a.产品编码=b.子项编码 where   优先级=1
   ) 
      select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,wiptype,子项类型,sum(数量)数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号 as 子项规格 from  temp_bom a
  left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
  left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码
  group by 产品编码,fx.物料名称 ,子项编码,base.物料名称 ,wiptype,子项类型 ,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号", dr["物料编码"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon); //这个搜出来没有自身 
                                                                              // DataRow[] dr_self = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                DataRow r_total = ss.TotalCount.Rows.Find(dr["物料编码"]);
                //DataRow[] r_total = ss.TotabvmblCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                r_total["订单用量"] = Convert.ToDecimal(dr["数量"]) + Convert.ToDecimal(r_total["订单用量"]);

                foreach (DataRow r in temp.Rows)
                {

                    DataRow f = ss.TotalCount.Rows.Find(r["子项编码"]);
                    //DataRow[] rrr = ss.TotalCount.Select(string.Format("物料编码='{0}'", r["子项编码"]));
                    f["订单用量"] = Math.Round(Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(r["数量"]), 6, MidpointRounding.AwayFromZero) + Convert.ToDecimal(f["订单用量"]);
                }


                decimal dec_订单数 = Convert.ToDecimal(dr["数量"]);

                try
                {
                    decimal total = Convert.ToDecimal(r_total["总数"]);
                    decimal kczs = Convert.ToDecimal(r_total["库存总数"]);
                    decimal wwcgds = Convert.ToDecimal(r_total["在制量"]);
                    decimal dec_Unclaimed = Convert.ToDecimal(r_total["未领量"]);
                    decimal dec_InTransit = Convert.ToDecimal(r_total["在途量"]);
                    if (total >= dec_订单数) //库存加未完成>需求数
                    {
                        r_total["总数"] = total - dec_订单数;

                        //r_total[0]["订单用量"]=r_total[0]["订单用量"];

                    }
                    else
                    {
                        DataRow r_need = ss.dtM.NewRow();
                        r_need["在制量"] = wwcgds;
                        r_need["物料编码"] = r_total["物料编码"];
                        r_need["仓库号"] = r_total["默认仓库号"];
                        r_need["仓库名称"] = r_total["仓库名称"];
                        r_need["在途量"] = dec_InTransit;
                        r_need["最早发货日期"] = dr["最早发货日期"];
                        //r_need["最早预计开工日期"] = dr["最早预计开工日期"];
                        r_need["未领量"] = dec_Unclaimed;
                        r_need["物料名称"] = r_total["物料名称"];
                        r_need["规格型号"] = r_total["规格型号"];
                        r_need["存货分类"] = r_total["存货分类"];
                        r_need["库存总数"] = kczs;
                        r_need["受订量"] = r_total["受订量"];
                        r_need["自制"] = r_total["自制"];
                        r_need["工时"] = r_total["工时"];
                        r_need["已转制令数"] = r_total["已转制令数"];
                        r_need["已转工单数"] = r_total["已转工单数"];
                        r_need["参考数量"] = dec_订单数 - total;
                        r_need["拼板数量"] = r_total["拼板数量"];

                        r_need["订单用量"] = r_total["订单用量"];
                        r_need["停用"] = r_total["停用"];
                        r_need["班组编号"] = r_total["b_班组编号"];
                        r_need["班组名称"] = r_total["b_班组名称"];
                        ss.dtM.Rows.Add(r_need);
                        r_total["总数"] = 0;
                    }
                    r_total["需求数量"] = Convert.ToDecimal(r_total["需求数量"]) + dec_订单数;


                }
                catch (Exception ex)
                {
                    throw new Exception(dr["物料编码"].ToString() + ex.Message);
                }
            }
            //5-23 存在 库存+在制-未领<0 的也是缺的
            DataView v = new DataView(ss.TotalCount);
            v.RowFilter = "总数<0";
            DataTable tx = v.ToTable();
            foreach (DataRow rr in tx.Rows)
            {
                string s = string.Format(@"with parent_bom(产品编码,子项编码,仓库号,仓库名称,bom_level ) as
                   (select  产品编码,子项编码,仓库号,仓库名称,1 as level from 基础数据物料BOM表 
                    where 子项编码='{0}'
                      union all 
                   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,b.bom_level+1  from 基础数据物料BOM表 a
                   inner join parent_bom b on a.子项编码=b.产品编码  )
                      select  * from parent_bom ", rr["物料编码"].ToString());
                DataTable dtz = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                // dtz = ERPorg.Corg.fun_GetFather(dtz, dr["物料编码"].ToString(), 0, true);
                //加入他自身
                DataRow rrr = dtz.NewRow();
                rrr["产品编码"] = rr["物料编码"].ToString();
                dtz.Rows.Add(rrr);
                DataView dv = new DataView(ss.salelist);
                if (dtz.Rows.Count > 0)
                {
                    s = string.Format("物料编码 in (");
                    foreach (DataRow xx in dtz.Rows)
                    {
                        s = s + "'" + xx["产品编码"].ToString() + "',";
                    }
                    s = s.Substring(0, s.Length - 1) + ")";

                    dv.RowFilter = s;
                    dv.Sort = "最早发货日期";

                }
                DataRow r_need = ss.dtM.NewRow();
                r_need["在制量"] = rr["在制量"];
                r_need["物料编码"] = rr["物料编码"];
                r_need["仓库号"] = rr["默认仓库号"];
                r_need["仓库名称"] = rr["仓库名称"];
                r_need["在途量"] = rr["在途量"];
                if (dv.Count > 0)
                {
                    r_need["最早发货日期"] = dv.ToTable().Rows[0]["最早发货日期"];
                }
                else
                {
                    r_need["最早发货日期"] = DBNull.Value; //原 2011-1-1  修改时间 2020-3-27
                }

                // r_need["最早发货日期"] = DBNull.Value; //原 2011-1-1  修改时间 2020-3-27
                //r_need["最早预计开工日期"] = DBNull.Value;
                r_need["未领量"] = rr["未领量"];
                r_need["物料名称"] = rr["物料名称"];
                r_need["规格型号"] = rr["规格型号"];
                r_need["存货分类"] = rr["存货分类"];
                r_need["库存总数"] = rr["库存总数"];
                r_need["受订量"] = rr["受订量"];
                r_need["自制"] = rr["自制"];
                r_need["工时"] = rr["工时"];
                r_need["已转制令数"] = rr["已转制令数"];
                r_need["已转工单数"] = rr["已转工单数"];
                r_need["参考数量"] = -Convert.ToDecimal(rr["总数"]);

                r_need["订单用量"] = rr["订单用量"];
                r_need["停用"] = rr["停用"];
                r_need["班组编号"] = rr["b_班组编号"];
                r_need["班组名称"] = rr["b_班组名称"];
                ss.dtM.Rows.Add(r_need);
                DataRow r_total = ss.TotalCount.Rows.Find(rr["物料编码"]);
                // DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", rr["物料编码"]));
                if (r_total == null)
                {
                    throw new Exception(rr["物料编码"].ToString() + "在总表中没有找到数据");

                }
                r_total["总数"] = 0;
                r_need["拼板数量"] = r_total["拼板数量"];

            }
            DataTable dtMcopy = ss.dtM.Copy();
            //fun_dg(dtMcopy);
            foreach (DataRow dr in dtMcopy.Rows)
            {
                if (dr["自制"].Equals(true))
                {
                    if (ss.Bom.Select(string.Format("产品编码='{0}'", dr["物料编码"])).Length == 0)
                    {
                        ss.str_log = ss.str_log + dr["物料编码"].ToString() + "属性为自制但是没有bom";
                    }
                }
                ///19-8-23 增加子项委外 =1  委外的可能不是自制属性 但是也许呀往下算 下面可能还有自制件
                DataRow[] br = ss.Bom.Select(string.Format("产品编码='{0}'and (子项自制=1 or 子项委外=1)", dr["物料编码"].ToString()));
                if (br.Length > 0) //找到需要自制的半成品 
                {
                    decimal dec_缺 = Convert.ToDecimal(dr["参考数量"].ToString());
                    foreach (DataRow brr in br)
                    {
                        decimal dec = dec_缺 * Convert.ToDecimal(brr["数量"]); //这是自制半成品所需的量 

                        DataRow stock_total = ss.TotalCount.Rows.Find(brr["子项编码"]);
                        //DataRow[] stock_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                        decimal total_z = Convert.ToDecimal(stock_total["总数"]);
                        if (stock_total == null)
                        {
                            throw new Exception(brr["物料编码"].ToString() + "在总表中没有找到数据");
                        }
                        stock_total["需求数量"] = Convert.ToDecimal(stock_total["需求数量"]) + dec; //记录需求数量

                        if (total_z >= dec) //库存加未完成>需求数
                        {
                            stock_total["总数"] = total_z - dec;
                        }
                        else
                        {
                            DataRow[] fr = ss.dtM.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                            if (fr.Length > 0)
                            {
                                fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec - total_z;
                                if (fr[0]["最早发货日期"] != DBNull.Value && dr["最早发货日期"] != DBNull.Value && (Convert.ToDateTime(fr[0]["最早发货日期"]) > Convert.ToDateTime(dr["最早发货日期"]) || Convert.ToDateTime(fr[0]["最早发货日期"]) == Convert.ToDateTime("2011-1-1")))
                                {
                                    fr[0]["最早发货日期"] = dr["最早发货日期"];
                                }
                                //if (!(fr[0]["最早预计开工日期"] == DBNull.Value))
                                //{
                                //    if (!(dr["最早预计开工日期"] == DBNull.Value))
                                //    {
                                //        if (Convert.ToDateTime(fr[0]["最早预计开工日期"]) > Convert.ToDateTime(dr["最早预计开工日期"]))
                                //        {
                                //            fr[0]["最早预计开工日期"] = dr["最早预计开工日期"];
                                //        }
                                //    }
                                //}
                            }
                            else
                            {
                                DataRow r_need = ss.dtM.NewRow();
                                r_need["在制量"] = stock_total["在制量"];
                                r_need["未领量"] = stock_total["未领量"];
                                r_need["在途量"] = stock_total["在途量"];
                                r_need["物料编码"] = stock_total["物料编码"];
                                r_need["仓库号"] = stock_total["默认仓库号"];
                                r_need["仓库名称"] = stock_total["仓库名称"];
                                r_need["物料名称"] = stock_total["物料名称"];
                                r_need["规格型号"] = stock_total["规格型号"];
                                r_need["存货分类"] = stock_total["存货分类"];
                                r_need["库存总数"] = stock_total["库存总数"];
                                r_need["受订量"] = stock_total["受订量"];
                                r_need["自制"] = stock_total["自制"];
                                r_need["最早发货日期"] = dr["最早发货日期"];
                                //  r_need["最早预计开工日期"] = dr["最早预计开工日期"];
                                r_need["工时"] = stock_total["工时"];
                                r_need["已转制令数"] = stock_total["已转制令数"];
                                r_need["已转工单数"] = stock_total["已转工单数"];
                                r_need["参考数量"] = dec - total_z;
                                r_need["拼板数量"] = stock_total["拼板数量"];
                                r_need["订单用量"] = stock_total["订单用量"];
                                r_need["停用"] = stock_total["停用"];
                                r_need["班组编号"] = stock_total["b_班组编号"];
                                r_need["班组名称"] = stock_total["b_班组名称"];
                                ss.dtM.Rows.Add(r_need);
                                stock_total["总数"] = 0;
                            }

                            //缺的才需要继续往叶子节点递归 不缺不需要
                            // string str_最早开工日期 = "";
                            //if (!(dr["最早预计开工日期"] == DBNull.Value))
                            //{
                            //    str_最早开工日期 = dr["最早预计开工日期"].ToString();
                            //}
                            DateTime? t = null;
                            if (dr["最早发货日期"] != DBNull.Value)
                            {
                                t = Convert.ToDateTime(dr["最早发货日期"]);
                            }

                            fun_dg_主计划_all_1(ss, stock_total["物料编码"].ToString(), dec - total_z, Convert.ToBoolean(dr["自制"]), t);

                        }
                    }
                }
            }
            //到这里生产计划算完

            foreach (DataRow dr in ss.salelist.Rows)
            {
                DataRow[] xxx = ss.dtM.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                if (xxx.Length == 0)
                {
                    DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    decimal total = Convert.ToDecimal(r_total[0]["总数"]);
                    decimal kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                    decimal wwcgds = Convert.ToDecimal(r_total[0]["在制量"]);
                    DataRow r_need = ss.dtM.NewRow();
                    r_need["在制量"] = wwcgds;
                    r_need["仓库号"] = r_total[0]["默认仓库号"];
                    r_need["仓库名称"] = r_total[0]["仓库名称"];
                    r_need["物料编码"] = r_total[0]["物料编码"];
                    r_need["物料名称"] = r_total[0]["物料名称"];
                    r_need["规格型号"] = r_total[0]["规格型号"];
                    r_need["存货分类"] = r_total[0]["存货分类"];
                    r_need["库存总数"] = kczs;
                    r_need["受订量"] = r_total[0]["受订量"];
                    r_need["自制"] = r_total[0]["自制"];
                    r_need["工时"] = r_total[0]["工时"];
                    r_need["参考数量"] = 0;
                    r_need["已转制令数"] = r_total[0]["已转制令数"];
                    r_need["已转工单数"] = r_total[0]["已转工单数"];
                    r_need["停用"] = r_total[0]["停用"];
                    ss.dtM.Rows.Add(r_need);
                }
            }
            ss.dtM.Columns.Add("总耗时", typeof(decimal));
            foreach (DataRow dr in ss.dtM.Rows)
            {
                DataRow[] rr = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                dr["需求数量"] = rr[0]["需求数量"];
                dr["总耗时"] = Convert.ToDecimal(dr["参考数量"]) * Convert.ToDecimal(dr["工时"]);
            }


            //20-2-23 计划和采购都要计算 
            //if (bl) //请求的是 采购计划结果
            //{
            //继续往下算 
            DataTable dtM_PurchasePool = new DataTable();

            dtM_PurchasePool.Columns.Add("未领量", typeof(decimal));
            dtM_PurchasePool.Columns.Add("在途量", typeof(decimal));
            dtM_PurchasePool.Columns.Add("委外在途", typeof(decimal));
            dtM_PurchasePool.Columns.Add("最早发货日期", typeof(DateTime));
            // dtM_PurchasePool.Columns.Add("最早预计开工日期", typeof(DateTime));
            dtM_PurchasePool.Columns.Add("仓库号");
            dtM_PurchasePool.Columns.Add("仓库名称");
            dtM_PurchasePool.Columns.Add("未发量", typeof(decimal));

            dtM_PurchasePool.Columns.Add("供应商编号");
            dtM_PurchasePool.Columns.Add("默认供应商");
            dtM_PurchasePool.Columns.Add("采购员");
            dtM_PurchasePool.Columns.Add("物料编码");
            dtM_PurchasePool.Columns.Add("物料名称");
            dtM_PurchasePool.Columns.Add("规格型号");
            dtM_PurchasePool.Columns.Add("库存总数", typeof(decimal));
            dtM_PurchasePool.Columns.Add("存货分类");
            dtM_PurchasePool.Columns.Add("参考数量", typeof(decimal));
            dtM_PurchasePool.Columns.Add("受订量", typeof(decimal));
            dtM_PurchasePool.Columns.Add("可购", typeof(bool));
            dtM_PurchasePool.Columns.Add("自制", typeof(bool));
            dtM_PurchasePool.Columns.Add("委外", typeof(bool));
            dtM_PurchasePool.Columns.Add("ECN", typeof(bool));
            dtM_PurchasePool.Columns.Add("最小包装", typeof(decimal));
            dtM_PurchasePool.Columns.Add("采购周期");
            dtM_PurchasePool.Columns.Add("已采未审", typeof(decimal));
            dtM_PurchasePool.Columns.Add("采购未送检", typeof(decimal));
            dtM_PurchasePool.Columns.Add("已送未检", typeof(decimal));
            dtM_PurchasePool.Columns.Add("已检未入", typeof(decimal));
            dtM_PurchasePool.Columns.Add("需求数量", typeof(decimal));
            //19-6-10 
            dtM_PurchasePool.Columns.Add("库存下限", typeof(decimal));
            dtM_PurchasePool.Columns.Add("订单用量", typeof(decimal));
            dtM_PurchasePool.Columns.Add("订单缺料", typeof(decimal));
            //20-1-8
            dtM_PurchasePool.Columns.Add("供应状态");
            //20-1-14
            dtM_PurchasePool.Columns.Add("停用", typeof(bool));

            DataColumn[] pk_cg = new DataColumn[1];
            pk_cg[0] = dtM_PurchasePool.Columns["物料编码"];
            dtM_PurchasePool.PrimaryKey = pk_cg;


            foreach (DataRow dr in ss.dtM.Rows) //因为这里dtM就是算出的 计划池  就是算出的计划要生产的 量比如父项A 要生产100 子项B只要生产 50 个 
            {                                //原材料 只要算一层 即是所缺的原材料
                DataRow[] x = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                if (x.Length == 0)
                {
                    DataRow r_need = dtM_PurchasePool.NewRow();
                    DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (Convert.ToBoolean(r_total[0]["可购"]) || Convert.ToBoolean(r_total[0]["委外"]))
                    {
                        r_need["未领量"] = dr["未领量"];
                        r_need["在途量"] = dr["在途量"]; ;
                        r_need["最早发货日期"] = dr["最早发货日期"];
                        //if (!(dr["最早预计开工日期"] == DBNull.Value))
                        //{
                        //    r_need["最早预计开工日期"] = dr["最早预计开工日期"];
                        //}
                        r_need["物料编码"] = dr["物料编码"];
                        r_need["仓库号"] = dr["仓库号"];
                        r_need["仓库名称"] = dr["仓库名称"];
                        r_need["供应商编号"] = r_total[0]["供应商编号"];
                        r_need["默认供应商"] = r_total[0]["默认供应商"];
                        r_need["采购员"] = r_total[0]["采购员"];
                        r_need["委外在途"] = r_total[0]["委外在途"];
                        r_need["物料名称"] = dr["物料名称"];
                        r_need["规格型号"] = dr["规格型号"];
                        r_need["存货分类"] = dr["存货分类"];
                        r_need["库存总数"] = r_total[0]["库存总数"];
                        r_need["受订量"] = r_total[0]["受订量"];
                        r_need["自制"] = r_total[0]["自制"];
                        r_need["可购"] = r_total[0]["可购"];
                        r_need["委外"] = r_total[0]["委外"];
                        r_need["ECN"] = r_total[0]["ECN"];
                        r_need["未发量"] = r_total[0]["未发量"];

                        r_need["已采未审"] = r_total[0]["已采未审"];
                        r_need["采购未送检"] = r_total[0]["采购未送检"];
                        r_need["已送未检"] = r_total[0]["已送未检"];
                        r_need["已检未入"] = r_total[0]["已检未入"];
                        r_need["参考数量"] = dr["参考数量"];
                        //19-6-10
                        r_need["库存下限"] = r_total[0]["库存下限"];
                        r_need["采购周期"] = r_total[0]["采购周期"];
                        r_need["最小包装"] = r_total[0]["最小包装"];

                        r_need["订单用量"] = r_total[0]["订单用量"];
                        //20-1-8
                        r_need["供应状态"] = r_total[0]["供应状态"];
                        //20-1-14
                        r_need["停用"] = r_total[0]["停用"];
                        //r_need["订单缺料"] = Convert.ToDecimal(r_total[0]["总数"]) - Convert.ToDecimal(dr["在途量"]);

                        dtM_PurchasePool.Rows.Add(r_need);
                    }
                }

                DataRow[] r_PPool = ss.Bom.Select(string.Format("产品编码='{0}'and 子项自制=0 and (子项可购=1 or 子项委外=1)", dr["物料编码"]));
                foreach (DataRow rr in r_PPool)
                {
                    ///19-8-14  8-23 生产上面 委外的也往下算了 那么这里 加这个限制 无误
                    if (!Convert.ToBoolean(rr["子项委外"]))
                    {
                        decimal dec_需 = Convert.ToDecimal(dr["参考数量"]) * Convert.ToDecimal(rr["数量"]); //父项所缺数*bom数量
                        DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                        decimal total = 0;
                        decimal kczs = 0;
                        decimal dec_wl = 0;
                        decimal dec_zt = 0;
                        if (r_total.Length == 0)
                        {
                            total = 0;
                            kczs = 0;
                            dec_wl = 0;
                            dec_zt = 0;
                        }
                        total = Convert.ToDecimal(r_total[0]["总数"]);
                        kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                        dec_wl = Convert.ToDecimal(r_total[0]["未领量"]);
                        dec_zt = Convert.ToDecimal(r_total[0]["在途量"]);
                        //decimal dec_n = 0;
                        r_total[0]["需求数量"] = Convert.ToDecimal(r_total[0]["需求数量"]) + dec_需;
                        if (total - dec_需 > 0) //不缺
                        {
                            r_total[0]["总数"] = total - dec_需;
                        }
                        else //缺了
                        {
                            DataRow[] fr = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                            if (fr.Length > 0)
                            {
                                fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec_需 - total;
                                //if (!(dr["最早预计开工日期"] == DBNull.Value))
                                //{
                                //    if (Convert.ToDateTime(fr[0]["最早预计开工日期"]) > Convert.ToDateTime(dr["最早预计开工日期"]))
                                //    {
                                //        fr[0]["最早预计开工日期"] = dr["最早预计开工日期"];
                                //    }
                                //}
                                if (fr[0]["最早发货日期"] != DBNull.Value && (Convert.ToDateTime(fr[0]["最早发货日期"]) > Convert.ToDateTime(dr["最早发货日期"]) || Convert.ToDateTime(fr[0]["最早发货日期"]) == Convert.ToDateTime("2011-1-1")))
                                {
                                    fr[0]["最早发货日期"] = dr["最早发货日期"];
                                }
                            }
                            else
                            {
                                DataRow r_need = dtM_PurchasePool.NewRow();
                                r_need["未领量"] = dec_wl;
                                r_need["在途量"] = dec_zt;
                                r_need["委外在途"] = Convert.ToDecimal(r_total[0]["委外在途"]);
                                r_need["最早发货日期"] = dr["最早发货日期"];
                                //if (!(dr["最早预计开工日期"] == DBNull.Value))
                                //{
                                //    r_need["最早预计开工日期"] = dr["最早预计开工日期"];
                                //}
                                r_need["物料编码"] = r_total[0]["物料编码"];
                                r_need["仓库号"] = r_total[0]["默认仓库号"];
                                r_need["仓库名称"] = r_total[0]["仓库名称"];
                                r_need["未发量"] = r_total[0]["未发量"];

                                r_need["供应商编号"] = r_total[0]["供应商编号"];
                                r_need["默认供应商"] = r_total[0]["默认供应商"];
                                r_need["采购员"] = r_total[0]["采购员"];
                                r_need["物料名称"] = r_total[0]["物料名称"];
                                r_need["规格型号"] = r_total[0]["规格型号"];
                                r_need["存货分类"] = r_total[0]["存货分类"];
                                r_need["库存总数"] = kczs;
                                r_need["受订量"] = r_total[0]["受订量"];
                                r_need["自制"] = r_total[0]["自制"];
                                r_need["委外"] = r_total[0]["委外"];
                                r_need["ECN"] = r_total[0]["ECN"];

                                r_need["可购"] = r_total[0]["可购"];
                                r_need["已采未审"] = r_total[0]["已采未审"];
                                r_need["采购未送检"] = r_total[0]["采购未送检"];
                                r_need["已送未检"] = r_total[0]["已送未检"];
                                r_need["已检未入"] = r_total[0]["已检未入"];
                                r_need["库存下限"] = r_total[0]["库存下限"];
                                r_need["采购周期"] = r_total[0]["采购周期"];
                                r_need["最小包装"] = r_total[0]["最小包装"];
                                //20-1-8
                                r_need["供应状态"] = r_total[0]["供应状态"];
                                r_need["订单用量"] = r_total[0]["订单用量"];
                                r_need["停用"] = r_total[0]["停用"];
                                //r_need["订单缺料"] = Convert.ToDecimal(r_total[0]["总数"]) - Convert.ToDecimal(dr["在途量"]);
                                r_need["参考数量"] = dec_需 - total;
                                dtM_PurchasePool.Rows.Add(r_need);
                                r_total[0]["总数"] = 0;
                            }
                        }
                    }

                }
            }
            //18-12-3 使用人提出 加入 不缺但是有在途的 方便她催料

            //19-6-10 加入安全库存  
            DataColumn dcc = new DataColumn("参考数量(含安全库存)", typeof(decimal));
            dcc.DefaultValue = 0;
            dtM_PurchasePool.Columns.Add(dcc);

            DataView dv_add = new DataView(ss.TotalCount);
            dv_add.RowFilter = "在途量>0 or 委外在途>0 or 总数<库存下限";
            DataTable dt_1 = dv_add.ToTable();
            foreach (DataRow dr in dt_1.Rows)
            {
                DataRow[] rrr = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                if (rrr.Length > 0)
                {
                    continue;
                }
                else
                {
                    DataRow r_need = dtM_PurchasePool.NewRow();
                    r_need["未领量"] = dr["未领量"];
                    r_need["在途量"] = dr["在途量"];
                    r_need["仓库号"] = dr["默认仓库号"];
                    r_need["仓库名称"] = dr["仓库名称"];
                    r_need["未发量"] = dr["未发量"];

                    r_need["供应商编号"] = dr["供应商编号"];
                    r_need["默认供应商"] = dr["默认供应商"];
                    r_need["采购员"] = dr["采购员"];
                    r_need["委外在途"] = dr["委外在途"];
                    r_need["物料编码"] = dr["物料编码"];
                    r_need["物料名称"] = dr["物料名称"];
                    r_need["规格型号"] = dr["规格型号"];
                    r_need["存货分类"] = dr["存货分类"];
                    r_need["库存总数"] = dr["库存总数"];
                    r_need["受订量"] = dr["受订量"];
                    r_need["自制"] = dr["自制"];
                    r_need["委外"] = dr["委外"];
                    r_need["ECN"] = dr["ECN"];

                    r_need["可购"] = dr["可购"];
                    r_need["已采未审"] = dr["已采未审"];
                    r_need["采购未送检"] = dr["采购未送检"];
                    r_need["已送未检"] = dr["已送未检"];
                    r_need["已检未入"] = dr["已检未入"];
                    r_need["库存下限"] = dr["库存下限"];
                    r_need["采购周期"] = dr["采购周期"];
                    r_need["最小包装"] = dr["最小包装"];
                    //19-6-10 改  
                    r_need["参考数量"] = 0;
                    decimal dec = Convert.ToDecimal(dr["库存下限"]) - Convert.ToDecimal(dr["总数"]);
                    r_need["参考数量(含安全库存)"] = dec > 0 ? dec : 0;
                    //19-11-06
                    r_need["订单用量"] = dr["订单用量"];
                    //20-1-8
                    r_need["供应状态"] = dr["供应状态"];
                    //20-1-14
                    r_need["停用"] = dr["停用"];
                    //r_need["订单缺料"] = Convert.ToDecimal(dr["总数"]) - Convert.ToDecimal(dr["在途量"]);
                    dtM_PurchasePool.Rows.Add(r_need);
                }
            }

            foreach (DataRow dr in dtM_PurchasePool.Rows)
            {
                decimal dec = Convert.ToDecimal(dr["库存下限"]);
                decimal dec_cksl = Convert.ToDecimal(dr["参考数量"]);
                if (dec_cksl > 0)
                {
                    dr["参考数量(含安全库存)"] = dec_cksl + dec;
                }
                // else //这一块已经在上面2969-2970行处理了
                //{

                //}
                //decimal x = dec_cksl - dec_T_total_总 + dec;
                //    dr["参考数量(含安全库存)"] = x>0?x:0;
                DataRow rr = ss.TotalCount.Rows.Find(dr["物料编码"]);
                //DataRow[] rr = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                dr["需求数量"] = rr["需求数量"];
                dr["订单缺料"] = Convert.ToDecimal(rr["总数"]) - Convert.ToDecimal(rr["在途量"]) - Convert.ToDecimal(dr["参考数量"]);
            }
            ss.dtM_采购池 = dtM_PurchasePool;
            //}
            //else //计划 
            //{

            //}
            return ss;
        }

        /// <summary>
        /// 20-1-13增加最早开工日期，用于主计划计算
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="bl"></param>
        /// <returns></returns>
        private static result caluu_主计划(result ss, bool bl)
        {
            //先计算销售列表中的产品的欠缺数量
            // salelist 即为 dt_SaleOrder  为 销售明细汇总数据  物料  sum(数量) 
            if (!ss.TotalCount.Columns.Contains("订单用量"))
            {
                DataColumn dc = new DataColumn("订单用量", typeof(decimal));
                dc.DefaultValue = 0;
                ss.TotalCount.Columns.Add(dc);
            }
            int all = ss.salelist.Rows.Count;
            int i = 0;
            foreach (DataRow dr in ss.salelist.Rows)
            {
                i++;
                //太慢 半小时都没算完 19-11-6  TotalCount 里面加了主键 瞬秒
                //DataTable dt_x = new DataTable();
                //dt_x = ERPorg.Corg.billofM_带数量(dt_x, dr["物料编码"].ToString(), false);
                //foreach (DataRow rr in dt_x.Rows)
                //{
                //    decimal dec = Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(rr["数量"]);
                //    DataRow[] rrr = ss.TotalCount.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                //    rrr[0]["订单用量"] = dec + Convert.ToDecimal(rrr[0]["订单用量"]);
                //}


                string s = string.Format(@"  with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,bom_level ) as
 (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,1 as level from 基础数据物料BOM表 
   where 产品编码='{0}' and 优先级=1
   union all 
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型, convert(decimal(18, 6),a.数量*b.数量) as 数量,a.bom类型,b.bom_level+1  from 基础数据物料BOM表 a
   inner join temp_bom b on a.产品编码=b.子项编码  where   优先级=1
   ) 
      select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,wiptype,子项类型,sum(数量)数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号 as 子项规格 from  temp_bom a
  left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
  left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码
  group by 产品编码,fx.物料名称 ,子项编码,base.物料名称 ,wiptype,子项类型 ,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号", dr["物料编码"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon); //这个搜出来没有自身 
                                                                              // DataRow[] dr_self = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                DataRow r_total = ss.TotalCount.Rows.Find(dr["物料编码"]);
                //DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                r_total["订单用量"] = Convert.ToDecimal(dr["数量"]) + Convert.ToDecimal(r_total["订单用量"]);

                foreach (DataRow r in temp.Rows)
                {

                    DataRow f = ss.TotalCount.Rows.Find(r["子项编码"]);
                    //DataRow[] rrr = ss.TotalCount.Select(string.Format("物料编码='{0}'", r["子项编码"]));
                    f["订单用量"] = Math.Round(Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(r["数量"]), 6, MidpointRounding.AwayFromZero) + Convert.ToDecimal(f["订单用量"]);
                }


                decimal dec_订单数 = Convert.ToDecimal(dr["数量"]);

                try
                {
                    decimal total = Convert.ToDecimal(r_total["总数"]);
                    decimal kczs = Convert.ToDecimal(r_total["库存总数"]);
                    decimal wwcgds = Convert.ToDecimal(r_total["在制量"]);
                    decimal dec_Unclaimed = Convert.ToDecimal(r_total["未领量"]);
                    decimal dec_InTransit = Convert.ToDecimal(r_total["在途量"]);
                    if (total >= dec_订单数) //库存加未完成>需求数
                    {
                        r_total["总数"] = total - dec_订单数;

                        //r_total[0]["订单用量"]=r_total[0]["订单用量"];

                    }
                    else
                    {
                        DataRow r_need = ss.dtM.NewRow();
                        r_need["在制量"] = wwcgds;
                        r_need["物料编码"] = r_total["物料编码"];
                        r_need["仓库号"] = r_total["默认仓库号"];
                        r_need["仓库名称"] = r_total["仓库名称"];
                        r_need["在途量"] = dec_InTransit;
                        r_need["最早发货日期"] = dr["最早发货日期"];
                        r_need["最早预计开工日期"] = dr["最早预计开工日期"];
                        r_need["未领量"] = dec_Unclaimed;
                        r_need["物料名称"] = r_total["物料名称"];
                        r_need["规格型号"] = r_total["规格型号"];
                        r_need["存货分类"] = r_total["存货分类"];
                        r_need["库存总数"] = kczs;
                        r_need["受订量"] = r_total["受订量"];
                        r_need["自制"] = r_total["自制"];
                        r_need["工时"] = r_total["工时"];
                        r_need["已转制令数"] = r_total["已转制令数"];
                        r_need["已转工单数"] = r_total["已转工单数"];
                        r_need["参考数量"] = dec_订单数 - total;
                        r_need["拼板数量"] = r_total["拼板数量"];

                        r_need["订单用量"] = r_total["订单用量"];
                        r_need["停用"] = r_total["停用"];
                        ss.dtM.Rows.Add(r_need);
                        r_total["总数"] = 0;
                    }
                    r_total["需求数量"] = Convert.ToDecimal(r_total["需求数量"]) + dec_订单数;


                }
                catch (Exception ex)
                {
                    throw new Exception(dr["物料编码"].ToString() + ex.Message);
                }
            }
            //5-23 存在 库存+在制-未领<0 的也是缺的
            DataView v = new DataView(ss.TotalCount);
            v.RowFilter = "总数<0";
            DataTable tx = v.ToTable();
            foreach (DataRow rr in tx.Rows)
            {
                DataRow r_need = ss.dtM.NewRow();
                r_need["在制量"] = rr["在制量"];
                r_need["物料编码"] = rr["物料编码"];
                r_need["仓库号"] = rr["默认仓库号"];
                r_need["仓库名称"] = rr["仓库名称"];
                r_need["在途量"] = rr["在途量"];
                r_need["最早发货日期"] = "2011-1-1";
                r_need["最早预计开工日期"] = DBNull.Value;
                r_need["未领量"] = rr["未领量"];
                r_need["物料名称"] = rr["物料名称"];
                r_need["规格型号"] = rr["规格型号"];
                r_need["存货分类"] = rr["存货分类"];
                r_need["库存总数"] = rr["库存总数"];
                r_need["受订量"] = rr["受订量"];
                r_need["自制"] = rr["自制"];
                r_need["工时"] = rr["工时"];
                r_need["已转制令数"] = rr["已转制令数"];
                r_need["已转工单数"] = rr["已转工单数"];
                r_need["参考数量"] = -Convert.ToDecimal(rr["总数"]);

                r_need["订单用量"] = rr["订单用量"];
                r_need["停用"] = rr["停用"];
                r_need["班组编号"] = rr["b_班组编号"];
                r_need["班组名称"] = rr["b_班组名称"];
                ss.dtM.Rows.Add(r_need);
                DataRow r_total = ss.TotalCount.Rows.Find(rr["物料编码"]);
                // DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", rr["物料编码"]));
                if (r_total == null)
                {
                    throw new Exception(rr["物料编码"].ToString() + "在总表中没有找到数据");

                }
                r_total["总数"] = 0;
                r_need["拼板数量"] = r_total["拼板数量"];

            }
            DataTable dtMcopy = ss.dtM.Copy();
            //fun_dg(dtMcopy);
            foreach (DataRow dr in dtMcopy.Rows)
            {
                if (dr["自制"].Equals(true))
                {
                    if (ss.Bom.Select(string.Format("产品编码='{0}'", dr["物料编码"])).Length == 0)
                    {
                        ss.str_log = ss.str_log + dr["物料编码"].ToString() + "属性为自制但是没有bom";
                    }
                }
                ///19-8-23 增加子项委外 =1  委外的可能不是自制属性 但是也许呀往下算 下面可能还有自制件
                DataRow[] br = ss.Bom.Select(string.Format("产品编码='{0}'and (子项自制=1 or 子项委外=1)", dr["物料编码"].ToString()));
                if (br.Length > 0) //找到需要自制的半成品 
                {
                    decimal dec_缺 = Convert.ToDecimal(dr["参考数量"].ToString());
                    foreach (DataRow brr in br)
                    {
                        decimal dec = dec_缺 * Convert.ToDecimal(brr["数量"]); //这是自制半成品所需的量 

                        DataRow stock_total = ss.TotalCount.Rows.Find(brr["子项编码"]);
                        //DataRow[] stock_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                        decimal total_z = Convert.ToDecimal(stock_total["总数"]);
                        if (stock_total == null)
                        {
                            throw new Exception(brr["物料编码"].ToString() + "在总表中没有找到数据");
                        }
                        stock_total["需求数量"] = Convert.ToDecimal(stock_total["需求数量"]) + dec; //记录需求数量



                        if (total_z >= dec) //库存加未完成>需求数
                        {
                            stock_total["总数"] = total_z - dec;
                        }
                        else
                        {
                            DataRow[] fr = ss.dtM.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                            if (fr.Length > 0)
                            {
                                fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec - total_z;
                                if (fr[0]["最早发货日期"] != DBNull.Value && (Convert.ToDateTime(fr[0]["最早发货日期"]) > Convert.ToDateTime(dr["最早发货日期"]) || Convert.ToDateTime(fr[0]["最早发货日期"]) == Convert.ToDateTime("2011-1-1")))
                                {
                                    fr[0]["最早发货日期"] = dr["最早发货日期"];
                                }
                                if (!(fr[0]["最早预计开工日期"] == DBNull.Value))
                                {
                                    if (Convert.ToDateTime(fr[0]["最早预计开工日期"]) > Convert.ToDateTime(dr["最早预计开工日期"]))
                                    {
                                        fr[0]["最早预计开工日期"] = dr["最早预计开工日期"];
                                    }
                                }

                            }
                            else
                            {
                                DataRow r_need = ss.dtM.NewRow();
                                r_need["在制量"] = stock_total["在制量"];
                                r_need["未领量"] = stock_total["未领量"];
                                r_need["在途量"] = stock_total["在途量"];

                                r_need["物料编码"] = stock_total["物料编码"];
                                r_need["仓库号"] = stock_total["默认仓库号"];
                                r_need["仓库名称"] = stock_total["仓库名称"];
                                r_need["物料名称"] = stock_total["物料名称"];
                                r_need["规格型号"] = stock_total["规格型号"];
                                r_need["存货分类"] = stock_total["存货分类"];
                                r_need["库存总数"] = stock_total["库存总数"];
                                r_need["受订量"] = stock_total["受订量"];
                                r_need["自制"] = stock_total["自制"];
                                r_need["最早发货日期"] = dr["最早发货日期"];
                                r_need["最早预计开工日期"] = dr["最早预计开工日期"];
                                r_need["工时"] = stock_total["工时"];
                                r_need["已转制令数"] = stock_total["已转制令数"];
                                r_need["已转工单数"] = stock_total["已转工单数"];
                                r_need["参考数量"] = dec - total_z;

                                r_need["拼板数量"] = stock_total["拼板数量"];

                                r_need["订单用量"] = stock_total["订单用量"];
                                r_need["停用"] = stock_total["停用"];
                                ss.dtM.Rows.Add(r_need);
                                stock_total["总数"] = 0;
                            }

                            //缺的才需要继续往叶子节点递归 不缺不需要
                            string str_最早开工日期 = "";
                            if (!(dr["最早预计开工日期"] == DBNull.Value))
                            {
                                str_最早开工日期 = dr["最早预计开工日期"].ToString();
                            }


                            fun_dg_主计划(ss, stock_total["物料编码"].ToString(), dec - total_z, Convert.ToBoolean(dr["自制"]), Convert.ToDateTime(dr["最早发货日期"]), str_最早开工日期);

                        }
                    }
                }
            }
            //到这里生产计划算完


            if (bl) //请求的是 采购计划结果
            {
                //继续往下算 
                DataTable dtM_PurchasePool = new DataTable();

                dtM_PurchasePool.Columns.Add("未领量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("在途量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("委外在途", typeof(decimal));
                dtM_PurchasePool.Columns.Add("最早发货日期", typeof(DateTime));
                dtM_PurchasePool.Columns.Add("最早预计开工日期", typeof(DateTime));
                dtM_PurchasePool.Columns.Add("仓库号");
                dtM_PurchasePool.Columns.Add("仓库名称");
                dtM_PurchasePool.Columns.Add("未发量", typeof(decimal));

                dtM_PurchasePool.Columns.Add("供应商编号");
                dtM_PurchasePool.Columns.Add("默认供应商");
                dtM_PurchasePool.Columns.Add("采购员");
                dtM_PurchasePool.Columns.Add("物料编码");
                dtM_PurchasePool.Columns.Add("物料名称");
                dtM_PurchasePool.Columns.Add("规格型号");
                dtM_PurchasePool.Columns.Add("库存总数", typeof(decimal));
                dtM_PurchasePool.Columns.Add("存货分类");
                dtM_PurchasePool.Columns.Add("参考数量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("受订量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("可购", typeof(bool));
                dtM_PurchasePool.Columns.Add("自制", typeof(bool));
                dtM_PurchasePool.Columns.Add("委外", typeof(bool));
                dtM_PurchasePool.Columns.Add("ECN", typeof(bool));
                dtM_PurchasePool.Columns.Add("最小包装", typeof(decimal));
                dtM_PurchasePool.Columns.Add("采购周期");
                dtM_PurchasePool.Columns.Add("已采未审", typeof(decimal));
                dtM_PurchasePool.Columns.Add("采购未送检", typeof(decimal));
                dtM_PurchasePool.Columns.Add("已送未检", typeof(decimal));
                dtM_PurchasePool.Columns.Add("已检未入", typeof(decimal));
                dtM_PurchasePool.Columns.Add("需求数量", typeof(decimal));
                //19-6-10 
                dtM_PurchasePool.Columns.Add("库存下限", typeof(decimal));
                dtM_PurchasePool.Columns.Add("订单用量", typeof(decimal));
                dtM_PurchasePool.Columns.Add("订单缺料", typeof(decimal));
                //20-1-8
                dtM_PurchasePool.Columns.Add("供应状态");
                //20-1-14
                dtM_PurchasePool.Columns.Add("停用", typeof(bool));

                DataColumn[] pk_cg = new DataColumn[1];
                pk_cg[0] = dtM_PurchasePool.Columns["物料编码"];
                dtM_PurchasePool.PrimaryKey = pk_cg;


                foreach (DataRow dr in ss.dtM.Rows) //因为这里dtM就是算出的 计划池  就是算出的计划要生产的 量比如父项A 要生产100 子项B只要生产 50 个 
                {                                //原材料 只要算一层 即是所缺的原材料
                    DataRow[] x = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (x.Length == 0)
                    {
                        DataRow r_need = dtM_PurchasePool.NewRow();
                        DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        if (Convert.ToBoolean(r_total[0]["可购"]) || Convert.ToBoolean(r_total[0]["委外"]))
                        {
                            r_need["未领量"] = dr["未领量"];
                            r_need["在途量"] = dr["在途量"]; ;
                            r_need["最早发货日期"] = dr["最早发货日期"];
                            if (!(dr["最早预计开工日期"] == DBNull.Value))
                            {
                                r_need["最早预计开工日期"] = dr["最早预计开工日期"];
                            }
                            r_need["物料编码"] = dr["物料编码"];
                            r_need["仓库号"] = dr["仓库号"];
                            r_need["仓库名称"] = dr["仓库名称"];
                            r_need["供应商编号"] = r_total[0]["供应商编号"];
                            r_need["默认供应商"] = r_total[0]["默认供应商"];
                            r_need["采购员"] = r_total[0]["采购员"];
                            r_need["委外在途"] = r_total[0]["委外在途"];
                            r_need["物料名称"] = dr["物料名称"];
                            r_need["规格型号"] = dr["规格型号"];
                            r_need["存货分类"] = dr["存货分类"];
                            r_need["库存总数"] = r_total[0]["库存总数"];
                            r_need["受订量"] = r_total[0]["受订量"];
                            r_need["自制"] = r_total[0]["自制"];
                            r_need["可购"] = r_total[0]["可购"];
                            r_need["委外"] = r_total[0]["委外"];
                            r_need["ECN"] = r_total[0]["ECN"];
                            r_need["未发量"] = r_total[0]["未发量"];

                            r_need["已采未审"] = r_total[0]["已采未审"];
                            r_need["采购未送检"] = r_total[0]["采购未送检"];
                            r_need["已送未检"] = r_total[0]["已送未检"];
                            r_need["已检未入"] = r_total[0]["已检未入"];
                            r_need["参考数量"] = dr["参考数量"];
                            //19-6-10
                            r_need["库存下限"] = r_total[0]["库存下限"];
                            r_need["采购周期"] = r_total[0]["采购周期"];
                            r_need["最小包装"] = r_total[0]["最小包装"];

                            r_need["订单用量"] = r_total[0]["订单用量"];
                            //20-1-8
                            r_need["供应状态"] = r_total[0]["供应状态"];
                            //20-1-14
                            r_need["停用"] = r_total[0]["停用"];
                            //r_need["订单缺料"] = Convert.ToDecimal(r_total[0]["总数"]) - Convert.ToDecimal(dr["在途量"]);

                            dtM_PurchasePool.Rows.Add(r_need);
                        }
                    }

                    DataRow[] r_PPool = ss.Bom.Select(string.Format("产品编码='{0}'and 子项自制=0 and (子项可购=1 or 子项委外=1)", dr["物料编码"]));
                    foreach (DataRow rr in r_PPool)
                    {
                        ///19-8-14  8-23 生产上面 委外的也往下算了 那么这里 加这个限制 无误
                        if (!Convert.ToBoolean(rr["子项委外"]))
                        {
                            decimal dec_需 = Convert.ToDecimal(dr["参考数量"]) * Convert.ToDecimal(rr["数量"]); //父项所缺数*bom数量
                            DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                            decimal total = 0;
                            decimal kczs = 0;
                            decimal dec_wl = 0;
                            decimal dec_zt = 0;
                            if (r_total.Length == 0)
                            {
                                total = 0;
                                kczs = 0;
                                dec_wl = 0;
                                dec_zt = 0;
                            }
                            total = Convert.ToDecimal(r_total[0]["总数"]);
                            kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                            dec_wl = Convert.ToDecimal(r_total[0]["未领量"]);
                            dec_zt = Convert.ToDecimal(r_total[0]["在途量"]);
                            //decimal dec_n = 0;
                            r_total[0]["需求数量"] = Convert.ToDecimal(r_total[0]["需求数量"]) + dec_需;
                            if (total - dec_需 > 0) //不缺
                            {
                                r_total[0]["总数"] = total - dec_需;
                            }
                            else //缺了
                            {
                                DataRow[] fr = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                                if (fr.Length > 0)
                                {
                                    fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec_需 - total;
                                    if (!(dr["最早预计开工日期"] == DBNull.Value))
                                    {
                                        if (Convert.ToDateTime(fr[0]["最早预计开工日期"]) > Convert.ToDateTime(dr["最早预计开工日期"]))
                                        {
                                            fr[0]["最早预计开工日期"] = dr["最早预计开工日期"];
                                        }
                                    }
                                    if (fr[0]["最早发货日期"] != DBNull.Value && (Convert.ToDateTime(fr[0]["最早发货日期"]) > Convert.ToDateTime(dr["最早发货日期"]) || Convert.ToDateTime(fr[0]["最早发货日期"]) == Convert.ToDateTime("2011-1-1")))
                                    {
                                        fr[0]["最早发货日期"] = dr["最早发货日期"];
                                    }
                                }
                                else
                                {
                                    DataRow r_need = dtM_PurchasePool.NewRow();
                                    r_need["未领量"] = dec_wl;
                                    r_need["在途量"] = dec_zt;
                                    r_need["委外在途"] = Convert.ToDecimal(r_total[0]["委外在途"]);
                                    r_need["最早发货日期"] = dr["最早发货日期"];
                                    if (!(dr["最早预计开工日期"] == DBNull.Value))
                                    {
                                        r_need["最早预计开工日期"] = dr["最早预计开工日期"];
                                    }
                                    r_need["物料编码"] = r_total[0]["物料编码"];
                                    r_need["仓库号"] = r_total[0]["默认仓库号"];
                                    r_need["仓库名称"] = r_total[0]["仓库名称"];
                                    r_need["未发量"] = r_total[0]["未发量"];

                                    r_need["供应商编号"] = r_total[0]["供应商编号"];
                                    r_need["默认供应商"] = r_total[0]["默认供应商"];
                                    r_need["采购员"] = r_total[0]["采购员"];
                                    r_need["物料名称"] = r_total[0]["物料名称"];
                                    r_need["规格型号"] = r_total[0]["规格型号"];
                                    r_need["存货分类"] = r_total[0]["存货分类"];
                                    r_need["库存总数"] = kczs;
                                    r_need["受订量"] = r_total[0]["受订量"];
                                    r_need["自制"] = r_total[0]["自制"];
                                    r_need["委外"] = r_total[0]["委外"];
                                    r_need["ECN"] = r_total[0]["ECN"];

                                    r_need["可购"] = r_total[0]["可购"];
                                    r_need["已采未审"] = r_total[0]["已采未审"];
                                    r_need["采购未送检"] = r_total[0]["采购未送检"];
                                    r_need["已送未检"] = r_total[0]["已送未检"];
                                    r_need["已检未入"] = r_total[0]["已检未入"];
                                    r_need["库存下限"] = r_total[0]["库存下限"];
                                    r_need["采购周期"] = r_total[0]["采购周期"];
                                    r_need["最小包装"] = r_total[0]["最小包装"];
                                    //20-1-8
                                    r_need["供应状态"] = r_total[0]["供应状态"];
                                    r_need["订单用量"] = r_total[0]["订单用量"];
                                    r_need["停用"] = r_total[0]["停用"];
                                    //r_need["订单缺料"] = Convert.ToDecimal(r_total[0]["总数"]) - Convert.ToDecimal(dr["在途量"]);
                                    r_need["参考数量"] = dec_需 - total;
                                    dtM_PurchasePool.Rows.Add(r_need);
                                    r_total[0]["总数"] = 0;
                                }
                            }
                        }

                    }
                }
                //18-12-3 使用人提出 加入 不缺但是有在途的 方便她催料

                //19-6-10 加入安全库存  
                DataColumn dcc = new DataColumn("参考数量(含安全库存)", typeof(decimal));
                dcc.DefaultValue = 0;
                dtM_PurchasePool.Columns.Add(dcc);

                DataView dv_add = new DataView(ss.TotalCount);
                dv_add.RowFilter = "在途量>0 or 委外在途>0 or 总数<库存下限";
                DataTable dt_1 = dv_add.ToTable();
                foreach (DataRow dr in dt_1.Rows)
                {
                    DataRow[] rrr = dtM_PurchasePool.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (rrr.Length > 0)
                    {
                        continue;
                    }
                    else
                    {
                        DataRow r_need = dtM_PurchasePool.NewRow();
                        r_need["未领量"] = dr["未领量"];
                        r_need["在途量"] = dr["在途量"];
                        r_need["仓库号"] = dr["默认仓库号"];
                        r_need["仓库名称"] = dr["仓库名称"];
                        r_need["未发量"] = dr["未发量"];

                        r_need["供应商编号"] = dr["供应商编号"];
                        r_need["默认供应商"] = dr["默认供应商"];
                        r_need["采购员"] = dr["采购员"];
                        r_need["委外在途"] = dr["委外在途"];
                        r_need["物料编码"] = dr["物料编码"];
                        r_need["物料名称"] = dr["物料名称"];
                        r_need["规格型号"] = dr["规格型号"];
                        r_need["存货分类"] = dr["存货分类"];
                        r_need["库存总数"] = dr["库存总数"];
                        r_need["受订量"] = dr["受订量"];
                        r_need["自制"] = dr["自制"];
                        r_need["委外"] = dr["委外"];
                        r_need["ECN"] = dr["ECN"];

                        r_need["可购"] = dr["可购"];
                        r_need["已采未审"] = dr["已采未审"];
                        r_need["采购未送检"] = dr["采购未送检"];
                        r_need["已送未检"] = dr["已送未检"];
                        r_need["已检未入"] = dr["已检未入"];
                        r_need["库存下限"] = dr["库存下限"];
                        r_need["采购周期"] = dr["采购周期"];
                        r_need["最小包装"] = dr["最小包装"];
                        //19-6-10 改  
                        r_need["参考数量"] = 0;
                        decimal dec = Convert.ToDecimal(dr["库存下限"]) - Convert.ToDecimal(dr["总数"]);
                        r_need["参考数量(含安全库存)"] = dec > 0 ? dec : 0;
                        //19-11-06
                        r_need["订单用量"] = dr["订单用量"];
                        //20-1-8
                        r_need["供应状态"] = dr["供应状态"];
                        //20-1-14
                        r_need["停用"] = dr["停用"];
                        //r_need["订单缺料"] = Convert.ToDecimal(dr["总数"]) - Convert.ToDecimal(dr["在途量"]);
                        dtM_PurchasePool.Rows.Add(r_need);
                    }
                }

                foreach (DataRow dr in dtM_PurchasePool.Rows)
                {
                    decimal dec = Convert.ToDecimal(dr["库存下限"]);
                    decimal dec_cksl = Convert.ToDecimal(dr["参考数量"]);
                    if (dec_cksl > 0)
                    {
                        dr["参考数量(含安全库存)"] = dec_cksl + dec;
                    }
                    // else //这一块已经在上面2969-2970行处理了
                    //{

                    //}
                    //decimal x = dec_cksl - dec_T_total_总 + dec;
                    //    dr["参考数量(含安全库存)"] = x>0?x:0;
                    DataRow rr = ss.TotalCount.Rows.Find(dr["物料编码"]);
                    //DataRow[] rr = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    dr["需求数量"] = rr["需求数量"];
                    dr["订单缺料"] = Convert.ToDecimal(rr["总数"]) - Convert.ToDecimal(rr["在途量"]) - Convert.ToDecimal(dr["参考数量"]);
                }
                ss.dtM = dtM_PurchasePool;
            }
            else //计划 
            {
                foreach (DataRow dr in ss.salelist.Rows)
                {
                    DataRow[] xxx = ss.dtM.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (xxx.Length == 0)
                    {
                        DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        decimal total = Convert.ToDecimal(r_total[0]["总数"]);
                        decimal kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                        decimal wwcgds = Convert.ToDecimal(r_total[0]["在制量"]);
                        DataRow r_need = ss.dtM.NewRow();
                        r_need["在制量"] = wwcgds;
                        r_need["仓库号"] = r_total[0]["默认仓库号"];
                        r_need["仓库名称"] = r_total[0]["仓库名称"];
                        r_need["物料编码"] = r_total[0]["物料编码"];
                        r_need["物料名称"] = r_total[0]["物料名称"];
                        r_need["规格型号"] = r_total[0]["规格型号"];
                        r_need["存货分类"] = r_total[0]["存货分类"];
                        r_need["库存总数"] = kczs;
                        r_need["受订量"] = r_total[0]["受订量"];
                        r_need["自制"] = r_total[0]["自制"];
                        r_need["工时"] = r_total[0]["工时"];
                        r_need["参考数量"] = 0;
                        r_need["已转制令数"] = r_total[0]["已转制令数"];
                        r_need["已转工单数"] = r_total[0]["已转工单数"];
                        r_need["停用"] = r_total[0]["停用"];
                        ss.dtM.Rows.Add(r_need);
                    }
                }
                ss.dtM.Columns.Add("总耗时", typeof(decimal));
                foreach (DataRow dr in ss.dtM.Rows)
                {
                    DataRow[] rr = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    dr["需求数量"] = rr[0]["需求数量"];
                    dr["总耗时"] = Convert.ToDecimal(dr["参考数量"]) * Convert.ToDecimal(dr["工时"]);
                }
            }
            return ss;
        }

        public static DataTable datrowToDataTable(DataRow[] rows)
        {
            if (rows == null || rows.Length == 0)
            {
                return null;
            }

            DataTable tmp = rows[0].Table.Clone(); // 复制DataRow的表结构
            foreach (DataRow row in rows)
            {
                tmp.ImportRow(row); // 将DataRow添加到DataTable中
            }
            return tmp;
        }
        /// <summary>
        /// 传入一个产品编码和需返回的dt 为该产品所有末节的子项,传入dt是为了外面可以循环调用,可以不停往里dt里写入
        /// </summary>
        /// <param name="dt_return"> 仅有一列 'dt_return存储结构'</param>
        /// <param name="str">产品编码 </param>
        ///  <param name="dt_辅助">用来暂存每个物料的清单，每遍历一次清空一次</param>
        public static DataTable billofM_mo(string str_单号, string str_日期, DataTable dt_return, string str_产品, decimal dec_数量, DataTable dt_辅助, DataTable dt_集合)
        {

            //  DataTable datta = dt_集合.Copy();

            //DataTable dt = new DataTable();
            //string s = string.Format("select  子项编码,(数量 * '{0}')as 数量 from 基础数据物料BOM表 where 产品编码='{1}'", Convert.ToDecimal(dec_数量), str_产品);

            //using (SqlDataAdapter da = new SqlDataAdapter(s, CPublic.Var.strConn))
            //{
            //    //da.Fill(dt_return);
            //    da.Fill(dt);
            //}
            //  dec_数量 = 1;////默认为1 
            DataRow[] dr = dt_集合.Select(string.Format("产品编码='{0}'", str_产品.ToString()));
            DataTable dt = ERPorg.Corg.datrowToDataTable(dr);
            dt.Columns.Add("需求数量", typeof(decimal));
            foreach (DataRow drr in dt.Rows)
            {
                decimal a = 0;
                // drr["数量"] = 0;
                a = decimal.Parse(drr["数量"].ToString()) * dec_数量;
                drr["需求数量"] = a;
            }
            DataTable dt_cp = dt.Copy();
            foreach (DataRow r in dt_cp.Rows)
            {
                //s = string.Format("select  子项编码,(数量 * '{0}')as 数量 from 基础数据物料BOM表 where 产品编码='{1}'",Convert.ToDecimal(r["数量"]),r["子项编码"]);
                //DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn);

                DataRow[] ds = dt_集合.Select(string.Format("产品编码='{0}'", r["子项编码"]));
                if (ds.Length > 0)
                {
                    DataTable dtf = datrowToDataTable(ds);
                    dtf.Columns.Add("需求数量");
                    DataTable temp = dtf.Clone();
                    foreach (DataRow rs in dtf.Rows)
                    {

                        DataRow[] dss = dt_集合.Select(string.Format("产品编码='{0}' and 子项编码='{1}'", rs["产品编码"], rs["子项编码"]));
                        rs["数量"] = dss[0]["数量"];
                        rs["需求数量"] = Convert.ToDecimal(rs["数量"]) * Convert.ToDecimal(r["需求数量"]);
                        temp.ImportRow(rs);

                    }
                    fun_dg_billofM(dt_return, temp, str_单号, str_日期, str_产品, dt_辅助, dt_集合, dec_数量);

                }
                else
                {
                    DataRow[] rr = dt_集合.Select(string.Format("产品编码='{0}' and  子项编码='{1}'", r["产品编码"], r["子项编码"]));


                    DataRow dr1 = dt_return.NewRow();
                    dr1["订单号"] = str_单号.ToString();
                    dr1["产品编码"] = str_产品;
                    dr1["制令数量"] = dec_数量;
                    dr1["产品名称"] = rr[0]["产品名称"].ToString();
                    dr1["父项规格"] = rr[0]["父项规格"].ToString();
                    dr1["子项编码"] = rr[0]["子项编码"].ToString();
                    dr1["子项名称"] = rr[0]["子项名称"].ToString();
                    dr1["子项规格"] = rr[0]["子项规格"].ToString();
                    dr1["数量"] = Convert.ToDecimal(rr[0]["数量"]) * dec_数量;
                    dr1["日期"] = Convert.ToDateTime(str_日期);
                    dt_return.Rows.Add(dr1);


                }
            }
            return dt_return;
        }

        private static DataTable fun_dg_billofM(DataTable dt, DataTable dt_子, string str_单号, string str_日期, string str_产品, DataTable dt_辅助, DataTable dt_集合, Decimal 制令数)
        {
            //DataTable dt00 = dt_集合.Copy();
            if (dt_子.Rows.Count > 0)
            {
                foreach (DataRow xr in dt_子.Rows)
                {

                    DataRow[] ds = dt_集合.Select(string.Format("产品编码='{0}'", xr["子项编码"].ToString()));
                    if (ds.Length > 0 && xr["可购"].Equals(false))
                    {
                        DataTable dtg = datrowToDataTable(ds);
                        dtg.Columns.Add("需求数量");
                        DataTable temp = dtg.Clone();
                        foreach (DataRow rs in dtg.Rows)
                        {
                            rs["需求数量"] = Convert.ToDecimal(rs["数量"]) * Convert.ToDecimal(xr["需求数量"]);
                            temp.ImportRow(rs);
                        }


                        fun_dg_billofM(dt, temp, str_单号, str_日期, str_产品, dt_辅助, dt_集合, 制令数);
                    }

                    else
                    {
                        if (dt_辅助.Select(string.Format("子项编码='{0}'", xr["子项编码"])).Length > 0)
                        {
                            continue;
                        }
                        else
                        {
                            DataRow dr = dt_辅助.NewRow();
                            dr["子项编码"] = xr["子项编码"].ToString();
                            dt_辅助.Rows.Add(dr);
                            DataRow dr1 = dt.NewRow();
                            dr1["订单号"] = str_单号.ToString();
                            dr1["产品编码"] = str_产品;
                            dr1["制令数量"] = 制令数;
                            dr1["产品名称"] = xr["产品名称"].ToString();
                            dr1["父项规格"] = xr["父项规格"].ToString();
                            dr1["子项编码"] = xr["子项编码"].ToString();
                            dr1["子项名称"] = xr["子项名称"].ToString();
                            dr1["子项规格"] = xr["子项规格"].ToString();
                            dr1["数量"] = Convert.ToDecimal(xr["需求数量"]);
                            //  dr1["数量"] = Convert.ToDecimal(rr[0]["数量"]) * dec_数量;
                            dr1["日期"] = Convert.ToDateTime(str_日期);
                            dt.Rows.Add(dr1);
                        }

                    }
                }
            }


            return dt;
        }

        /// <summary>
        /// 传入控件，遍历所有子控件,如有gridview 尝试加载layout文件
        /// layout文件命名规则即为 界面name+gridview的name 保证唯一性
        /// </summary>
        /// <param name="x"></param>
        public void UserLayout(Control x, string FormName, string path)
        {

            foreach (Control c in x.Controls)
            {
                if (c is DevExpress.XtraGrid.GridControl)
                {
                    DevExpress.XtraGrid.GridControl g = (c as DevExpress.XtraGrid.GridControl);
                    if (File.Exists(path + string.Format(@"\{0}.xml", FormName + "_" + g.MainView.Name)))
                    {
                        g.MainView.RestoreLayoutFromXml(path + string.Format(@"\{0}.xml", FormName + "_" + g.MainView.Name));
                    }
                    break;
                }
                if (c.HasChildren)
                {
                    UserLayout(c, FormName, path);
                }

            }
        }
        /// <summary>
        /// 判断是否可以撤回单子 
        /// 结转后不允许撤回单子
        /// </summary>
        /// <param name="time"> 单据的日期</param>
        /// <returns></returns>
        public bool isJZ(DateTime time)
        {
            bool bl = false;
            string s = string.Format("select  top 1 * from 仓库月出入库结转表 where 结算日期>'{0}' order by 结算日期  desc", time);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            if (dt.Rows.Count == 0)
            {
                bl = true;
            }

            return bl;
        }

        /// <summary>
        /// 2020-3-17  将日期变成变量传入 重构一下 不改原来的了
        /// </summary>
        /// <param name="t_MakeOrder"> 生效工单的清单 需要回写最大序列号</param>
        public DataSet fun_SN(DataTable t_MakeOrder, DateTime time, int growth)
        {
            DataSet ds = new DataSet();

            string tNo = time.Year.ToString().Substring(2, 2) + time.Month.ToString("00") + time.Day.ToString("00");
            string strcon_BQ = "";
            try
            {
                strcon_BQ = CPublic.Var.geConn("BQ");
            }
            catch (Exception)
            {
                throw new Exception("未正确配置标签数据库,请确认");
            }
            strcon_BQ = CPublic.Var.geConn("BQ");
            string s = "select * from ShareLockInfo where 1=2 ";
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon_BQ);
            s = " select * from Print_ShareLockInfo where 1=2";
            DataTable tt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            ds.Tables.Add(dt);
            ds.Tables[0].TableName = "存东屋检验数据库记录";
            ds.Tables.Add(tt);
            ds.Tables[1].TableName = "存供应链数据库记录";
            ds.Tables.Add(t_MakeOrder);
            ds.Tables[2].TableName = "工单记录";

            foreach (DataRow dr in t_MakeOrder.Rows)
            {
                //生产数量 20-3-27 改为import  数量 growth 
                int dec_M = growth; // Convert.ToInt32(dr["生产数量"]); 原来的直接取得工单数量
                string LabSpCode = "";
                int NumBegin = 0;
                //[Mac规则ID]=0  不需要生产条码
                string ss = string.Format("select [产品简码] as  LabSpCode,[Mac规则ID] as RuleID from [基础物料标签维护信息表] where 物料编号='{0}' ", dr["物料编码"]);
                DataTable t = CZMaster.MasterSQL.Get_DataTable(ss, strcon);

                //if (dr["MaxNo"] != null && dr["MaxNo"].ToString() != "")

                // if (NumBegin == 0) NumBegin++;
                if (t.Rows.Count > 0)
                {
                    LabSpCode = t.Rows[0]["LabSpCode"].ToString();
                    NumBegin = ERPorg.Corg.fun_SN流水号(LabSpCode, time, dec_M);
                    for (int x = 0; x < dec_M; x++)
                    {
                        string sn = "";
                        string stemp = LabSpCode + tNo + NumBegin.ToString().PadLeft(6, '0');
                        sn = stemp + total_JY(stemp);
                        if (t.Rows[0]["RuleID"].ToString() != "0")
                        {//如果ruleID为0 只需要供应链中生成SN号 不需要写到BQ数据库里面
                            DataRow r = dt.NewRow();
                            r["DevType"] = t.Rows[0]["RuleID"];
                            r["CTNo"] = sn;
                            r["CheckFlag"] = "0";
                            r["TaskNo"] = dr["生产工单号"];
                            dt.Rows.Add(r);
                        }
                        DataRow rr = tt.NewRow();
                        rr["DevType"] = t.Rows[0]["RuleID"];
                        rr["CTNo"] = sn;
                        rr["CheckFlag"] = "0";
                        rr["MakeOrder"] = dr["生产工单号"];
                        tt.Rows.Add(rr);
                        NumBegin++;
                    }
                    dr["MaxNo"] = Convert.ToInt32(dr["MaxNo"]) + growth;  //2020-3-27 MaxNo 用作记录该工单已经生成多少个SN号 何立要求后面可以根据工单 先生成多少个 再生成多少个
                }
            }
            return ds;
        }

        /// <summary>
        /// 2019-10-15 生产检验程序所需的sn号 
        /// ds [0] 是 ‘存东屋检验数据库记录’ ShareLockInfo 
        /// ds [1] 是 ‘存供应链数据库记录’   Print_ShareLockInfo 
        /// ds [2] 是 ‘工单记录’             生产记录生产工单表 
        /// </summary>
        /// <param name="t_MakeOrder"> 生效工单的清单 需要回写最大序列号</param>
        public DataSet fun_SN(DataTable t_MakeOrder)
        {
            DataSet ds = new DataSet();
            DateTime time = CPublic.Var.getDatetime();
            string tNo = time.Year.ToString().Substring(2, 2) + time.Month.ToString("00") + time.Day.ToString("00");
            string strcon_BQ = "";
            try
            {
                strcon_BQ = CPublic.Var.geConn("BQ");
            }
            catch (Exception)
            {
                throw new Exception("未正确配置标签数据库,请确认");
            }
            strcon_BQ = CPublic.Var.geConn("BQ");
            string s = "select * from ShareLockInfo where 1=2 ";
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon_BQ);
            s = " select * from Print_ShareLockInfo where 1=2";
            DataTable tt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            ds.Tables.Add(dt);
            ds.Tables[0].TableName = "存东屋检验数据库记录";
            ds.Tables.Add(tt);
            ds.Tables[1].TableName = "存供应链数据库记录";
            ds.Tables.Add(t_MakeOrder);
            ds.Tables[2].TableName = "工单记录";

            foreach (DataRow dr in t_MakeOrder.Rows)
            {
                //生产数量 //这里是工单生效 直接按 生产数量
                int dec_M = Convert.ToInt32(dr["生产数量"]);
                string LabSpCode = "";
                int NumBegin = 0;
                //[Mac规则ID]=0  不需要生产条码
                string ss = string.Format("select [产品简码] as  LabSpCode,[Mac规则ID] as RuleID from [基础物料标签维护信息表] where 物料编号='{0}' ", dr["物料编码"]);
                DataTable t = CZMaster.MasterSQL.Get_DataTable(ss, strcon);

                //if (dr["MaxNo"] != null && dr["MaxNo"].ToString() != "")
                //    NumBegin = Convert.ToInt32(dr["MaxNo"]);
                //if (NumBegin == 0) NumBegin++;
                if (t.Rows.Count > 0)
                {
                    LabSpCode = t.Rows[0]["LabSpCode"].ToString();
                    NumBegin = ERPorg.Corg.fun_SN流水号(LabSpCode, time, dec_M);
                    for (int x = 0; x < dec_M; x++)
                    {
                        string sn = "";
                        string stemp = LabSpCode + tNo + NumBegin.ToString().PadLeft(6, '0');
                        sn = stemp + total_JY(stemp);
                        if (t.Rows[0]["RuleID"].ToString() != "0")
                        {//如果ruleID为0 只需要供应链中生成SN号 不需要写到BQ数据库里面

                            DataRow r = dt.NewRow();
                            r["DevType"] = t.Rows[0]["RuleID"];
                            r["CTNo"] = sn;
                            r["CheckFlag"] = "0";
                            r["TaskNo"] = dr["生产工单号"];
                            dt.Rows.Add(r);
                        }
                        DataRow rr = tt.NewRow();
                        rr["DevType"] = t.Rows[0]["RuleID"];
                        rr["CTNo"] = sn;
                        rr["CheckFlag"] = "0";
                        rr["MakeOrder"] = dr["生产工单号"];
                        tt.Rows.Add(rr);
                        NumBegin++;
                    }
                    dr["MaxNo"] = dec_M; //2020-3-27 MaxNo 用作记录该工单已经生成多少个SN号 何立要求后面可以根据工单 先生成多少个 再生成多少个

                }
            }
            return ds;
        }

        public string total_JY(string s)
        {
            string xx = "";
            int sum = 0;
            foreach (char c in s)
            {
                sum = sum + Convert.ToInt32(c.ToString());
            }
            xx = sum.ToString("000");
            return xx;
        }

        /// <summary>
        /// 阿拉伯数字转换成大写中文
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public string NumToChinese(string str)
        {
            string[] xx = str.Split('.');
            string x = xx[0];
            string y = "";
            string[] pArrayNum = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            //为数字位数建立一个位数组  
            string[] pArrayDigit = { "", "拾", "佰", "仟" };
            //为数字单位建立一个单位数组  
            string[] pArrayUnits = { "", "万", "億", "萬億" }; ///19-11-11采购要求 萬 --> 万
            var pStrReturnValue = ""; //返回值  
            var finger = 0; //字符位置指针  
            var pIntM = x.Length % 4; //取模  
            int pIntK;
            if (pIntM > 0)
            {
                pIntK = x.Length / 4 + 1;
            }
            else
            {
                pIntK = x.Length / 4;
            }
            //外层循环,四位一组,每组最后加上单位: ",万亿,",",亿,",",万,"  
            for (var i = pIntK; i > 0; i--)
            {
                var pIntL = 4;
                if (i == pIntK && pIntM != 0)
                {
                    pIntL = pIntM;
                }
                //得到一组四位数  
                var four = x.Substring(finger, pIntL);
                var P_int_l = four.Length;
                //内层循环在该组中的每一位数上循环  
                for (int j = 0; j < P_int_l; j++)
                {
                    //处理组中的每一位数加上所在的位  
                    int n = Convert.ToInt32(four.Substring(j, 1));
                    if (n == 0)
                    {
                        if (j < P_int_l - 1 && Convert.ToInt32(four.Substring(j + 1, 1)) > 0 && !pStrReturnValue.EndsWith(pArrayNum[n]))
                        {
                            pStrReturnValue += pArrayNum[n];
                        }
                    }
                    else
                    {
                        //if (!(n == 1 && (pStrReturnValue.EndsWith(pArrayNum[0]) | pStrReturnValue.Length == 0) && j == P_int_l - 2))
                        //    pStrReturnValue += pArrayNum[n];
                        //pStrReturnValue += pArrayDigit[P_int_l - j - 1];

                        if (!(n == 1 && (pStrReturnValue.EndsWith(pArrayNum[0]) | pStrReturnValue.Length == 0) && j == P_int_l - 2))
                        {
                            pStrReturnValue += pArrayNum[n];
                        }

                        pStrReturnValue += pArrayDigit[P_int_l - j - 1];
                    }
                }
                finger += pIntL;
                //每组最后加上一个单位:",万,",",亿," 等  
                if (i < pIntK) //如果不是最高位的一组  
                {
                    if (Convert.ToInt32(four) != 0)
                    {
                        //如果所有4位不全是0则加上单位",万,",",亿,"等  
                        pStrReturnValue += pArrayUnits[i - 1];
                    }
                }
                else
                {
                    //处理最高位的一组,最后必须加上单位  
                    pStrReturnValue += pArrayUnits[i - 1];
                }
            }
            pStrReturnValue = pStrReturnValue + "元";
            if (xx.Length > 1)
            {
                y = xx[1];
                ///分和角
                string dec_palce = "";
                int ii = 1;
                foreach (char s in y)
                {
                    int k = Convert.ToInt32(s.ToString());
                    if (k == 0)
                    {
                        dec_palce += "零";
                    }
                    else
                    {
                        dec_palce += pArrayNum[k];
                        if (ii == 1)
                        {
                            dec_palce += "角";

                        }
                        else
                        {
                            dec_palce += "分";
                        }
                    }
                    ii++;
                    if (ii > 2)
                    {
                        break;
                    }
                }
                if (dec_palce == "零零")
                {
                    dec_palce = "整";
                }
                else
                {
                    if (dec_palce.Substring(dec_palce.Length - 1, 1) == "零")
                    {
                        dec_palce = dec_palce.Substring(0, dec_palce.Length - 1);
                    }
                }
                pStrReturnValue += dec_palce;
            }
            else
            {
                pStrReturnValue = pStrReturnValue + "整";
            }
            return pStrReturnValue;
        }



        /// <summary>
        /// 根据出生日期，计算精确的年龄
        /// </summary>
        /// <param name="birthDate">生日</param>
        /// <returns></returns>
        public static int CalculateAge(string birthDay)
        {
            DateTime birthDate = DateTime.Parse(birthDay);
            DateTime nowDateTime = DateTime.Now;
            int age = nowDateTime.Year - birthDate.Year;
            //再考虑月、天的因素
            if (nowDateTime.Month < birthDate.Month || (nowDateTime.Month == birthDate.Month && nowDateTime.Day < birthDate.Day))
            {
                age--;
            }
            return age;
        }

        /// <summary>
        ///19-12-20  定义 生日年龄性别 实体
        ///
        /// </summary>
        public class BirthdayAgeSex
        {
            public string Birthday { get; set; }
            public int Age { get; set; }
            public string Sex { get; set; }
        }
        public static BirthdayAgeSex GetBirthdayAgeSex(string identityCard)
        {
            if (string.IsNullOrEmpty(identityCard))
            {
                return null;
            }
            else
            {
                if (identityCard.Length != 15 && identityCard.Length != 18)//身份证号码只能为15位或18位其它不合法
                {
                    return null;
                }
            }

            BirthdayAgeSex entity = new BirthdayAgeSex();
            string strSex = string.Empty;
            if (identityCard.Length == 18)//处理18位的身份证号码从号码中得到生日和性别代码
            {
                entity.Birthday = identityCard.Substring(6, 4) + "-" + identityCard.Substring(10, 2) + "-" + identityCard.Substring(12, 2);
                strSex = identityCard.Substring(14, 3);
            }
            if (identityCard.Length == 15)
            {
                entity.Birthday = "19" + identityCard.Substring(6, 2) + "-" + identityCard.Substring(8, 2) + "-" + identityCard.Substring(10, 2);
                strSex = identityCard.Substring(12, 3);
            }

            entity.Age = CalculateAge(entity.Birthday);//根据生日计算年龄
            if (int.Parse(strSex) % 2 == 0)//性别代码为偶数是女性奇数为男性
            {
                entity.Sex = "女";
            }
            else
            {
                entity.Sex = "男";
            }
            return entity;
        }

        /// <summary>
        /// 19-12-26 撤回生产入库单
        ///    ds.Tables.Add(dt_制令);
        ///    ds.Tables.Add(dt_工单);
        ///    ds.Tables.Add(dt_检验单);
        ///    ds.Tables.Add(dt_入库主表);
        ///    ds.Tables.Add(dt_入库明细);
        ///    ds.Tables.Add(dt_流水账);
        ///   ds.Tables.Add(dt_kc);
        /// </summary>
        public DataSet back_ruk(string rkdh)
        {
            DataSet ds = new DataSet();
            string s = string.Format(@"select  a.*,b.生产制令单号 from 生产记录成品入库单明细表  a
            left join 生产记录生产工单表 b on a.生产工单号 = b.生产工单号  where  成品入库单号 ='{0}' ", rkdh);
            DataTable dt_入库明细 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = string.Format(@"select  物料编码,入库仓库ID as 仓库号,入库数量 as 数量 from 生产记录成品入库单明细表  where  成品入库单号 ='{0}' ", rkdh);
            DataTable kc_temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataTable dt_kc = fun_库存(-1, kc_temp);
            DataTable dt_检验单 = new DataTable();
            DataTable dt_工单 = new DataTable();
            DataTable dt_制令 = new DataTable();

            foreach (DataRow dr in dt_入库明细.Rows)
            {
                //检验单
                if (dt_检验单.Columns.Count == 0)
                {
                    s = string.Format("select  * from 生产记录生产检验单主表  where 生产检验单号 ='{0}'", dr["生产检验单号"].ToString());
                    SqlDataAdapter da = new SqlDataAdapter(s, strcon);
                    da.Fill(dt_检验单);
                }
                else
                {
                    DataRow[] rr = dt_检验单.Select(string.Format("生产检验单号='{0}'", dr["生产检验单号"].ToString()));
                    if (rr.Length > 0)  //已经加载了  同一个检验单 不可能在同一张入库单里面  所以这边理论上是不存在的
                    {
                        throw new Exception("业务数据有误,请确认");
                    }
                    else
                    {
                        s = string.Format("select  * from 生产记录生产检验单主表  where 生产检验单号 ='{0}'", dr["生产检验单号"].ToString());
                        SqlDataAdapter da = new SqlDataAdapter(s, strcon);
                        da.Fill(dt_检验单);
                    }
                }
                DataRow[] r_检验 = dt_检验单.Select(string.Format("生产检验单号='{0}'", dr["生产检验单号"].ToString()));
                //这里一定会有 并且只有一个
                r_检验[0]["已入库数量"] = Convert.ToDecimal(r_检验[0]["已入库数量"]) - Convert.ToDecimal(dr["入库数量"]);
                r_检验[0]["完成"] = 0;
                r_检验[0]["完成日期"] = DBNull.Value;
                r_检验[0]["完成人员"] = "";
                r_检验[0]["完成人员ID"] = "";

                //工单
                if (dt_工单.Columns.Count == 0)
                {
                    s = string.Format("select  * from 生产记录生产工单表  where 生产工单号 ='{0}'", dr["生产工单号"].ToString());
                    SqlDataAdapter da = new SqlDataAdapter(s, strcon);
                    da.Fill(dt_工单);
                }
                else
                {
                    DataRow[] rr = dt_工单.Select(string.Format("生产工单号='{0}'", dr["生产工单号"].ToString()));
                    if (rr.Length > 0)  //这里工单是有可能有的,一个工单分批检验  根据检验单入库
                    {
                        //如果有了 不需要再加载
                        continue; //因为有过了所以状态上次也一定改过了  不需要重复执行 制令也一样
                    }
                    else
                    {
                        s = string.Format("select  * from 生产记录生产工单表  where 生产工单号 ='{0}'", dr["生产工单号"].ToString());
                        SqlDataAdapter da = new SqlDataAdapter(s, strcon);
                        da.Fill(dt_工单);
                    }
                }
                DataRow[] r_工单 = dt_工单.Select(string.Format("生产工单号='{0}'", dr["生产工单号"].ToString()));
                //这里一定会有 并且只有一个  工单上只需要修改 完成跟完成日期，不需要改数量  因此4040行可以continue 
                r_工单[0]["完成"] = 0;
                r_工单[0]["完成日期"] = DBNull.Value;

                //制令 和工单一样  
                if (dt_制令.Columns.Count == 0)
                {
                    s = string.Format("select  * from 生产记录生产制令表  where 生产制令单号='{0}'", dr["生产制令单号"].ToString());
                    SqlDataAdapter da = new SqlDataAdapter(s, strcon);
                    da.Fill(dt_制令);
                }
                else
                {
                    DataRow[] rr = dt_制令.Select(string.Format("生产制令单号='{0}'", dr["生产制令单号"].ToString()));
                    if (rr.Length > 0)
                    {

                        continue;
                    }
                    else
                    {
                        s = string.Format("select  * from 生产记录生产制令表  where 生产制令单号='{0}'", dr["生产制令单号"].ToString());
                        SqlDataAdapter da = new SqlDataAdapter(s, strcon);
                        da.Fill(dt_制令);
                    }
                }
                DataRow[] r_制令 = dt_制令.Select(string.Format("生产制令单号='{0}'", dr["生产制令单号"].ToString()));

                r_制令[0]["完成"] = 0;
                r_制令[0]["完成日期"] = DBNull.Value;


            }
            for (int i = dt_入库明细.Rows.Count - 1; i >= 0; i--)
            {
                dt_入库明细.Rows[i].Delete();
            }
            s = string.Format("select  * from 生产记录成品入库单主表 where 成品入库单号='{0}'", rkdh);
            DataTable dt_入库主表 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //有且只有一条数据
            dt_入库主表.Rows[0].Delete();
            s = string.Format("select  * from 仓库出入库明细表  where  单号='{0}'", rkdh);
            DataTable dt_流水账 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            for (int i = dt_流水账.Rows.Count - 1; i >= 0; i--)
            {
                dt_流水账.Rows[i].Delete();
            }
            ds.Tables.Add(dt_制令);
            ds.Tables.Add(dt_工单);
            ds.Tables.Add(dt_检验单);
            ds.Tables.Add(dt_入库主表);
            ds.Tables.Add(dt_入库明细);
            ds.Tables.Add(dt_流水账);
            ds.Tables.Add(dt_kc);
            return ds;
        }
        /// <summary>
        /// 19-12-27 撤销采购入库
        /// dt_采购单
        /// dt_检验单
        /// dt_入库主
        /// dt_入库明细
        /// dt_流水账
        /// dt_kc
        /// </summary>
        /// <param name="rkdh"></param>
        /// <returns></returns>
        public DataSet back_purrk(string rkdh)
        {
            DataSet ds = new DataSet();

            string s = string.Format(@"select * from  采购记录采购单入库明细 where  入库单号='{0}'", rkdh);
            DataTable dt_入库明细 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = string.Format(@"select  物料编码,仓库ID as 仓库号,入库量 as 数量 from 采购记录采购单入库明细  where  入库单号 ='{0}' ", rkdh);
            DataTable kc_temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataTable dt_kc = fun_库存(-1, kc_temp);
            DataTable dt_检验单 = new DataTable();

            DataTable dt_采购单 = new DataTable();

            foreach (DataRow dr in dt_入库明细.Rows)
            {
                //检验单
                if (dt_检验单.Columns.Count == 0)
                {
                    s = string.Format("select  * from 采购记录采购单检验主表  where 检验记录单号 ='{0}'", dr["检验记录单号"].ToString());
                    SqlDataAdapter da = new SqlDataAdapter(s, strcon);
                    da.Fill(dt_检验单);
                }
                else
                {
                    DataRow[] rr = dt_检验单.Select(string.Format("检验记录单号='{0}'", dr["检验记录单号"].ToString()));
                    if (rr.Length > 0)  //已经加载了  同一个检验单 不可能在同一张入库单里面  所以这边理论上是不存在的
                    {
                        throw new Exception("业务数据有误,请确认");
                    }
                    else
                    {
                        s = string.Format("select  * from 采购记录采购单检验主表  where 检验记录单号 ='{0}'", dr["检验记录单号"].ToString());
                        SqlDataAdapter da = new SqlDataAdapter(s, strcon);
                        da.Fill(dt_检验单);
                    }
                }
                DataRow[] r_检验 = dt_检验单.Select(string.Format("检验记录单号='{0}'", dr["检验记录单号"].ToString()));
                //这里一定会有 并且只有一个
                r_检验[0]["已入库数"] = Convert.ToDecimal(r_检验[0]["已入库数"]) - Convert.ToDecimal(dr["入库量"]);
                r_检验[0]["入库完成"] = 0;


                //送检单 不需要修改

                //采购明细
                if (dt_采购单.Columns.Count == 0)
                {
                    s = string.Format("select  * from 采购记录采购单明细表  where 采购明细号='{0}'", dr["采购单明细号"].ToString());
                    SqlDataAdapter da = new SqlDataAdapter(s, strcon);
                    da.Fill(dt_采购单);
                }
                else
                {
                    DataRow[] rr = dt_采购单.Select(string.Format("采购明细号='{0}'", dr["采购单明细号"].ToString()));
                    if (rr.Length > 0)
                    {


                    }
                    else
                    {
                        s = string.Format("select  * from 采购记录采购单明细表  where 采购明细号='{0}'", dr["采购单明细号"].ToString());
                        SqlDataAdapter da = new SqlDataAdapter(s, strcon);
                        da.Fill(dt_采购单);
                    }
                }
                DataRow[] r_采购明细 = dt_采购单.Select(string.Format("采购明细号='{0}'", dr["采购单明细号"].ToString()));
                r_采购明细[0]["明细完成日期"] = DBNull.Value;
                //采购明细表 的 明细完成 是 送检完成的意思
                r_采购明细[0]["完成数量"] = Convert.ToDecimal(r_采购明细[0]["完成数量"]) - Convert.ToDecimal(dr["入库量"]); ;
                r_采购明细[0]["未完成数量"] = Convert.ToDecimal(r_采购明细[0]["未完成数量"]) + Convert.ToDecimal(dr["入库量"]); ;



            }
            for (int i = dt_入库明细.Rows.Count - 1; i >= 0; i--)
            {
                dt_入库明细.Rows[i].Delete();
            }
            s = string.Format("select  * from 采购记录采购单入库主表 where 入库单号='{0}'", rkdh);
            DataTable dt_入库主表 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //有且只有一条数据
            dt_入库主表.Rows[0].Delete();
            s = string.Format("select  * from 仓库出入库明细表  where  单号='{0}'", rkdh);
            DataTable dt_流水账 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            for (int i = dt_流水账.Rows.Count - 1; i >= 0; i--)
            {
                dt_流水账.Rows[i].Delete();
            }

            ds.Tables.Add(dt_采购单);
            ds.Tables.Add(dt_检验单);
            ds.Tables.Add(dt_入库主表);
            ds.Tables.Add(dt_入库明细);
            ds.Tables.Add(dt_流水账);
            ds.Tables.Add(dt_kc);


            return ds;
        }

        /// <summary>
        /// 判断所有子项中是否有停产或将停产
        /// </summary>
        public bool determ_stop_product(string str_wul)
        {
            bool bl = false;
            string s = string.Format(@"with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,bom_level ) as
 (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,1 as level from 基础数据物料BOM表 
   where 产品编码='{0}'
   union all 
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型,a.数量,a.bom类型,b.bom_level+1  from 基础数据物料BOM表 a
   inner join temp_bom b on a.产品编码=b.子项编码 
   ) 
    select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,wiptype,子项类型,数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号 as 子项规格,base.供应状态 from  temp_bom a
  left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
  left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码
  where base.供应状态 in ('停产','将停产')
  group by 产品编码,fx.物料名称 ,子项编码,base.物料名称 ,wiptype,子项类型,数量,bom类型,a.仓库号,a.仓库名称,
   bom_level,base.规格型号,base.供应状态", str_wul);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (dt.Rows.Count > 0)
            {
                bl = true;
            }
            return bl;
        }

        public bool bl_结账(DateTime t)
        {
            bool bl = false;
            //我们系统没有结账功能  根据 仓库月结转表 和 财务凭证记录表 判断
            return bl;
        }

        //20-3-23 检查开票金额 与 采购金额是否金额差额1元以内 超过需要提交加个价格异动单 
        //20-3-26 开票金额大于原来的金额 1块钱
        public bool price_changed(DataTable dt_开票明细)
        {
            bool bl = false;
            decimal total_front = 0;
            decimal total_after = 0;
            foreach (DataRow r in dt_开票明细.Rows)
            {
                string s = $@"select   case when LEFT(入库单号,2)='DW'  then  CONVERT(decimal(18,6),a.备注6) 
        when a.采购单号 = ''  then  CONVERT(decimal(18,6),a.备注6)  else  b.未税单价 end as 采购未税单价 from 采购记录采购单入库明细 a
        left  join 采购记录采购单明细表 b  on a.采购单明细号 = b.采购明细号
        where  入库明细号='{r["入库明细号"].ToString()}' ";
                DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                total_front += Convert.ToDecimal(t.Rows[0]["采购未税单价"]) * Convert.ToDecimal(r["开票数量"]);
                total_after += Convert.ToDecimal(r["未税金额"]);
            }
            if (total_after - total_front > 1)
            {
                bl = true;//价格变动 需要提交价格异动单
            }

            return bl;
        }


        /// <summary>
        /// 20-3-24  主计划根据 计划员填写的计划 按日期和物料编码group by 然后逐条运算
        /// 20-4-1 计划单内都是成品半成品 且不包含 虚拟件  
        /// 
        /// </summary>
        /// 
        /// <param name="dt_计划单"> ds.Tables[0]</param>
        /// <param name="dt_来源明细_原材料"> ds.Tables[1] 这里面是来源销售订单只有原材料 总的缺的 </param>

        /// <param name="dt_bom"> ds.Tables[2]</param>
        /// <param name="dt_totalcount"> ds.Tables[3]</param>
        /// <param name="dt_采购池"> ds.Tables[4] 原采购池计算结果</param>
        /// 
        /// <returns></returns>
        public result main_Plan_calu(DataSet ds)
        {
            DataTable dt_计划单 = ds.Tables[0].Copy();
            DataTable dt_来源明细_原材料 = ds.Tables[1].Copy();
            //string sx = "exec FourNum";
            //CZMaster.MasterSQL.ExecuteSQL(sx, strcon);
            DataTable dt_bom = ds.Tables[2].Copy();
            //DataColumn[] pk_bom = new DataColumn[2];
            //pk_bom[0] = dt_bom.Columns["产品编码"];
            //pk_bom[1] = dt_bom.Columns["子项编码"];
            //dt_bom.PrimaryKey = pk_bom;
            //取库存,总数=库存-未领+在制+在途 + 计划在途量   是为了下面 根据 这个来计算, 下面会有算一遍 总数-受订量
            //sx = "select * from V_pooltotal ";
            DataTable dt_totalcount = ds.Tables[3].Copy();
            DataTable dt_采购池 = ds.Tables[4].Copy();

            //DataColumn[] pk = new DataColumn[1];
            //pk[0] = dt_totalcount.Columns["物料编码"];
            //dt_totalcount.PrimaryKey = pk;
            DataTable dtM = new DataTable();
            dtM.Columns.Add("计划在途量", typeof(decimal)); //计划已提交需求但采购未转的数量 
            dtM.Columns.Add("未领量", typeof(decimal));
            dtM.Columns.Add("在途量", typeof(decimal));
            dtM.Columns.Add("委外在途", typeof(decimal));
            dtM.Columns.Add("需求来料日期", typeof(DateTime));
            dtM.Columns.Add("预计开工日期", typeof(DateTime));
            dtM.Columns.Add("仓库号");
            dtM.Columns.Add("仓库名称");
            dtM.Columns.Add("未发量", typeof(decimal));
            dtM.Columns.Add("供应商编号");
            dtM.Columns.Add("默认供应商");
            dtM.Columns.Add("采购员");
            dtM.Columns.Add("物料编码");
            dtM.Columns.Add("物料名称");
            dtM.Columns.Add("规格型号");
            dtM.Columns.Add("库存总数", typeof(decimal));
            dtM.Columns.Add("存货分类");
            dtM.Columns.Add("参考数量", typeof(decimal));
            dtM.Columns.Add("受订量", typeof(decimal));
            dtM.Columns.Add("可购", typeof(bool));
            dtM.Columns.Add("自制", typeof(bool));
            dtM.Columns.Add("委外", typeof(bool));
            dtM.Columns.Add("ECN", typeof(bool));
            dtM.Columns.Add("最小包装", typeof(decimal));
            dtM.Columns.Add("采购周期");
            dtM.Columns.Add("已采未审", typeof(decimal));
            dtM.Columns.Add("采购未送检", typeof(decimal));
            dtM.Columns.Add("已送未检", typeof(decimal));
            dtM.Columns.Add("已检未入", typeof(decimal));
            dtM.Columns.Add("需求数量", typeof(decimal));
            dtM.Columns.Add("库存下限", typeof(decimal));
            dtM.Columns.Add("订单用量", typeof(decimal));
            dtM.Columns.Add("订单缺料", typeof(decimal));
            //20-1-14
            dtM.Columns.Add("停用", typeof(bool));
            //20-1-8
            dtM.Columns.Add("供应状态");
            DataColumn[] pk2 = new DataColumn[2];
            pk2[0] = dtM.Columns["物料编码"];
            pk2[1] = dtM.Columns["需求来料日期"];
            dtM.PrimaryKey = pk2;
            //准备计算
            //2020-6-5
            foreach(DataRow dr in dt_计划单.Rows)
            {
                string s = string.Format(@"  with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,bom_level ) as
 (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,1 as level from 基础数据物料BOM表 
   where 产品编码='{0}'and 优先级=1
   union all 
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型, convert(decimal(18, 6),a.数量*b.数量) as 数量,a.bom类型,b.bom_level+1  from 基础数据物料BOM表 a
   inner join temp_bom b on a.产品编码=b.子项编码 where  优先级=1
   ) 
      select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,wiptype,子项类型,sum(数量)数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号 as 子项规格 from  temp_bom a
  left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
  left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码
  group by 产品编码,fx.物料名称 ,子项编码,base.物料名称 ,wiptype,子项类型 ,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号", dr["物料编码"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon); //这个搜出来没有自身 
                                                                              // DataRow[] dr_self = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                DataRow r_total = dt_totalcount.Rows.Find(dr["物料编码"]);
                //DataRow[] r_total = ss.TotalCount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                r_total["订单用量"] = Convert.ToDecimal(dr["计划数量"]) + Convert.ToDecimal(r_total["订单用量"]);

                foreach (DataRow r in temp.Rows)
                {

                    DataRow f = dt_totalcount.Rows.Find(r["子项编码"]);
                    //DataRow[] rrr = ss.TotalCount.Select(string.Format("物料编码='{0}'", r["子项编码"]));
                    f["订单用量"] = Math.Round(Convert.ToDecimal(dr["计划数量"]) * Convert.ToDecimal(r["数量"]), 6, MidpointRounding.AwayFromZero) + Convert.ToDecimal(f["订单用量"]);
                }

            }

            foreach (DataRow dr in dt_计划单.Rows) //此处为计划单 根据计算单算 每个料对应的日期要多少  量比如父项A 要生产100 子项B只要生产 50 个 
            {                                //原材料 只要算一层 即是所缺的原材料 但是虚拟件需要往下算 dtM 
                main_Plan_calu_dg(dtM, dt_bom, dt_totalcount, dr["物料编码"].ToString(), Convert.ToDecimal(dr["计划数量"]), Convert.ToDateTime(dr["开工日期"]).Date);
            }
            ////算完之后需要,先按物料编码汇总 
            //再与 dt_来源明细_原材料 比对其中有直接来源订单的物料,数量或者记录缺需要在dtM中根据销售订单到货日期增加数量或者增加记录，最后得到结果

            MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
            DataTable Dt_计划已排 = RBQ.SelectGroupByInto("", dtM, "物料编码,sum(参考数量) 参考数量", "", "物料编码");

            DataColumn dc = new DataColumn("参考数_c", typeof(decimal));
            dc.DefaultValue = 0;
            dt_采购池.Columns.Add(dc);

            //取工单中不包含在订单中的料
            DataView dv = new DataView(dt_采购池);
            dv.RowFilter = "参考数量>0 ";
            DataTable dt_工单用量 = dv.ToTable();


            ///2020-6-5 既是自制又是可购的  
            for (int i = dt_采购池.Rows.Count - 1; i >= 0; i--)
            {
                DataRow[] tr = dt_来源明细_原材料.Select($"物料编码='{dt_采购池.Rows[i]["物料编码"].ToString()}'");
                if (tr.Length == 0)
                {
                    dt_采购池.Rows.RemoveAt(i);
                }
                else
                {
                    DataRow[] r_已排 = Dt_计划已排.Select($"物料编码='{dt_采购池.Rows[i]["物料编码"].ToString()}'");
                    if (r_已排.Length > 0)
                    {
                        if (Convert.ToDecimal(r_已排[0]["参考数量"]) >= Convert.ToDecimal(dt_采购池.Rows[i]["参考数量"]))
                        {
                            dt_采购池.Rows.RemoveAt(i);
                        }
                        else
                        {
                            dt_采购池.Rows[i]["参考数_c"] = Convert.ToDecimal(dt_采购池.Rows[i]["参考数量"]) - Convert.ToDecimal(r_已排[0]["参考数量"]);
                        }
                        //剩余需排的数量

                    }
                    else
                    {
                        dt_采购池.Rows[i]["参考数_c"] = dt_采购池.Rows[i]["参考数量"];
                    }

                }
            } //这一步把采购池中 直接下销售订单的原材料并且 根据计划需求单算出来的 数量不足的 挑出来了 
              //按照销售到货日期倒排序 
            foreach (DataRow dr in dt_采购池.Rows)
            {
                DataRow[] tr = dt_来源明细_原材料.Select($"物料编码='{dr["物料编码"].ToString()}'", "预计发货日期 desc");
                if (tr.Length > 0)
                {
                    decimal dec_total = Convert.ToDecimal(dr["参考数_c"]);
                    foreach (DataRow r_1 in tr)
                    {
                        decimal dec_sale = Convert.ToDecimal(r_1["未完成数量"]);
                        decimal dec_add = 0;
                        if (dec_sale > dec_total)
                        {
                            dec_add = dec_total;
                            //dec_total = 0;
                        }
                        else
                        {
                            dec_add = dec_sale;
                            //dec_total -= dec_sale;
                        }
                        dec_total -= dec_add;
                        dr["参考数_c"] = dec_total;

                        DataRow[] r_add = dtM.Select($"物料编码='{r_1["物料编码"].ToString()}' and 预计开工日期='{Convert.ToDateTime(r_1["预计发货日期"]).Date}'");
                        if (r_add.Length > 0)
                        {
                           
                            r_add[0]["参考数量"] = Convert.ToDecimal(r_add[0]["参考数量"]) + dec_add;

                        }
                        else
                        {
                            DataRow[] r_total = dt_totalcount.Select($"物料编码='{r_1["物料编码"].ToString()}'");
                            DataRow r_need = dtM.NewRow();
                            r_need["未领量"] = r_total[0]["未领量"].ToString();
                            r_need["在途量"] = r_total[0]["在途量"].ToString();
                            r_need["委外在途"] = Convert.ToDecimal(r_total[0]["委外在途"]);
                            r_need["预计开工日期"] = Convert.ToDateTime(r_1["预计发货日期"]).Date;
                            r_need["需求来料日期"] = Convert.ToDateTime(r_need["预计开工日期"]).AddDays(-3).Date;

                            r_need["物料编码"] = r_total[0]["物料编码"].ToString();
                            r_need["仓库号"] = r_total[0]["默认仓库号"].ToString();
                            r_need["仓库名称"] = r_total[0]["仓库名称"].ToString();
                            r_need["未发量"] = r_total[0]["未发量"].ToString();
                            r_need["供应商编号"] = r_total[0]["供应商编号"].ToString();
                            r_need["默认供应商"] = r_total[0]["默认供应商"].ToString();
                            r_need["采购员"] = r_total[0]["采购员"].ToString();
                            r_need["物料名称"] = r_total[0]["物料名称"].ToString();
                            r_need["规格型号"] = r_total[0]["规格型号"].ToString();
                            r_need["存货分类"] = r_total[0]["存货分类"].ToString();
                            r_need["库存总数"] = r_total[0]["库存总数"].ToString();
                            r_need["受订量"] = r_total[0]["受订量"].ToString();
                            r_need["自制"] = r_total[0]["自制"].ToString();
                            r_need["委外"] = r_total[0]["委外"].ToString();
                            r_need["ECN"] = r_total[0]["ECN"].ToString();
                            r_need["可购"] = r_total[0]["可购"].ToString();
                            r_need["已采未审"] = r_total[0]["已采未审"].ToString();
                            r_need["采购未送检"] = r_total[0]["采购未送检"].ToString();
                            r_need["已送未检"] = r_total[0]["已送未检"].ToString();
                            r_need["已检未入"] = r_total[0]["已检未入"].ToString();
                            r_need["库存下限"] = r_total[0]["库存下限"].ToString();
                            r_need["采购周期"] = r_total[0]["采购周期"].ToString();
                            r_need["最小包装"] = r_total[0]["最小包装"].ToString();
                            //20-1-8
                            r_need["供应状态"] = r_total[0]["供应状态"].ToString();
                            //20-1-14
                            r_need["停用"] = r_total[0]["停用"].ToString();
                            r_need["订单用量"] = r_total[0]["订单用量"].ToString();
                            r_need["参考数量"] = dec_add;
                            dtM.Rows.Add(r_need);
                        }
                        if (dec_total == 0)
                        {
                            break;
                        }
                    }
                }
            }





            foreach (DataRow dr_1 in dt_工单用量.Rows)
            {



                DataRow[] r_add = dtM.Select($"物料编码='{dr_1["物料编码"].ToString()}'");
                if (r_add.Length > 0)
                {
                    //MessageBox.Show("111");
                }
                else
                {
                    DataRow[] r_total = dt_totalcount.Select($"物料编码='{dr_1["物料编码"].ToString()}'");
                    DataRow r_need = dtM.NewRow();
                    r_need["未领量"] = r_total[0]["未领量"].ToString();
                    r_need["在途量"] = r_total[0]["在途量"].ToString();
                    r_need["委外在途"] = Convert.ToDecimal(r_total[0]["委外在途"]);
                    r_need["预计开工日期"] = DBNull.Value;
                    r_need["需求来料日期"] = CPublic.Var.getDatetime();
                    r_need["物料编码"] = r_total[0]["物料编码"].ToString();
                    r_need["仓库号"] = r_total[0]["默认仓库号"].ToString();
                    r_need["仓库名称"] = r_total[0]["仓库名称"].ToString();
                    r_need["未发量"] = r_total[0]["未发量"].ToString();
                    r_need["供应商编号"] = r_total[0]["供应商编号"].ToString();
                    r_need["默认供应商"] = r_total[0]["默认供应商"].ToString();
                    r_need["采购员"] = r_total[0]["采购员"].ToString();
                    r_need["物料名称"] = r_total[0]["物料名称"].ToString();
                    r_need["规格型号"] = r_total[0]["规格型号"].ToString();
                    r_need["存货分类"] = r_total[0]["存货分类"].ToString();
                    r_need["库存总数"] = r_total[0]["库存总数"].ToString();
                    r_need["受订量"] = r_total[0]["受订量"].ToString();
                    r_need["自制"] = r_total[0]["自制"].ToString();
                    r_need["委外"] = r_total[0]["委外"].ToString();
                    r_need["ECN"] = r_total[0]["ECN"].ToString();
                    r_need["可购"] = r_total[0]["可购"].ToString();
                    r_need["已采未审"] = r_total[0]["已采未审"].ToString();
                    r_need["采购未送检"] = r_total[0]["采购未送检"].ToString();
                    r_need["已送未检"] = r_total[0]["已送未检"].ToString();
                    r_need["已检未入"] = r_total[0]["已检未入"].ToString();
                    r_need["库存下限"] = r_total[0]["库存下限"].ToString();
                    r_need["采购周期"] = r_total[0]["采购周期"].ToString();
                    r_need["最小包装"] = r_total[0]["最小包装"].ToString();
                    //20-1-8
                    r_need["供应状态"] = r_total[0]["供应状态"].ToString();
                    //20-1-14
                    r_need["停用"] = r_total[0]["停用"].ToString();
                    r_need["订单用量"] = r_total[0]["订单用量"].ToString();
                    r_need["参考数量"] = Convert.ToDecimal(dr_1["参考数量"]);
                    dtM.Rows.Add(r_need);
                }
            }


            //18-12-3 使用人提出 加入 不缺但是有在途的 方便她催料
            //DataColumn dcc = new DataColumn("参考数量(含安全库存)", typeof(decimal));
            //dcc.DefaultValue = 0;
            //dtM.Columns.Add(dcc);
            //DataView dv_add = new DataView(dt_totalcount);
            //dv_add.RowFilter = "在途量>0 or 委外在途>0 or 总数<库存下限";
            //DataTable dt_1 = dv_add.ToTable();
            //foreach (DataRow dr in dt_1.Rows)
            //{
            //    DataRow[] rrr = dtM.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
            //    if (rrr.Length > 0) continue;
            //    else
            //    {
            //        DataRow r_need = dtM.NewRow();
            //        r_need["未领量"] = dr["未领量"].ToString();
            //        r_need["在途量"] = dr["在途量"].ToString();
            //        r_need["仓库号"] = dr["默认仓库号"].ToString();
            //        r_need["仓库名称"] = dr["仓库名称"].ToString();
            //        r_need["未发量"] = dr["未发量"].ToString();
            //        r_need["供应商编号"] = dr["供应商编号"].ToString();
            //        r_need["默认供应商"] = dr["默认供应商"].ToString();
            //        r_need["采购员"] = dr["采购员"].ToString();
            //        r_need["委外在途"] = dr["委外在途"].ToString();
            //        r_need["物料编码"] = dr["物料编码"].ToString();
            //        r_need["物料名称"] = dr["物料名称"].ToString();
            //        r_need["规格型号"] = dr["规格型号"].ToString();
            //        r_need["存货分类"] = dr["存货分类"].ToString();
            //        r_need["库存总数"] = dr["库存总数"].ToString();
            //        r_need["受订量"] = dr["受订量"].ToString();
            //        r_need["自制"] = dr["自制"].ToString();
            //        r_need["委外"] = dr["委外"].ToString();
            //        r_need["ECN"] = dr["ECN"].ToString();
            //        r_need["可购"] = dr["可购"].ToString();
            //        r_need["已采未审"] = dr["已采未审"].ToString();
            //        r_need["采购未送检"] = dr["采购未送检"].ToString();
            //        r_need["已送未检"] = dr["已送未检"].ToString();
            //        r_need["已检未入"] = dr["已检未入"].ToString();
            //        r_need["库存下限"] = dr["库存下限"].ToString();
            //        r_need["采购周期"] = dr["采购周期"].ToString();
            //        r_need["最小包装"] = dr["最小包装"].ToString();
            //        //20-1-14
            //        r_need["停用"] = dr["停用"].ToString();
            //        //19-6-10 改  
            //        r_need["参考数量"] = 0;
            //        decimal dec = Convert.ToDecimal(dr["库存下限"]) - Convert.ToDecimal(dr["总数"]);
            //        r_need["参考数量(含安全库存)"] = dec > 0 ? dec : 0;
            //        //19-11-06
            //        r_need["订单用量"] = dr["订单用量"].ToString();
            //        //20-1-8
            //        r_need["供应状态"] = dr["供应状态"].ToString();
            //        //r_need["订单缺料"] = Convert.ToDecimal(dr["总数"]) - Convert.ToDecimal(dr["在途量"]);
            //        dtM.Rows.Add(r_need);
            //    }
            //}
            //foreach (DataRow dr in dtM.Rows)
            //{
            //    decimal dec = Convert.ToDecimal(dr["库存下限"]);
            //    decimal dec_cksl = Convert.ToDecimal(dr["参考数量"]);
            //    if (dec_cksl > 0) dr["参考数量(含安全库存)"] = dec_cksl + dec;
            //    // else //这一块已经在上面2969-2970行处理了
            //    //{

            //    //}
            //    //decimal x = dec_cksl - dec_T_total_总 + dec;
            //    //    dr["参考数量(含安全库存)"] = x>0?x:0;
            //    DataRow rr = dt_totalcount.Rows.Find(new object[] { dr["物料编码"], dr["需求来料日期"] });
            //    // DataRow[] rr = dt_totalcount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
            //    dr["需求数量"] = rr["需求数量"];
            //    dr["订单缺料"] = Convert.ToDecimal(rr["总数"]) - Convert.ToDecimal(rr["在途量"]) - Convert.ToDecimal(dr["参考数量"]);
            //}
            result ss = new result();
            ss.salelist_mx = dt_计划单; //此处源头数据为计划员做的计划单
                                     //ss.salelist = dt_SaleOrder;
                                     //ss.Polist_mx = IncompletePO;
            ss.Bom = dt_bom;
            ss.TotalCount = dt_totalcount;
            ss.str_log = "";
            ss.dtM = dtM;
            return ss;
        }
        /// <summary>
        /// 20-4-1
        /// </summary>
        private void main_Plan_calu_dg(DataTable dtM, DataTable dt_bom, DataTable dt_totalcount, string str, decimal dec, DateTime time_kg)
        {
            DataRow[] r_PPool = dt_bom.Select($"产品编码='{str}' and (子项可购=1  or  子项自制=1  or 子项委外=1 or WIPType ='虚拟')");
            foreach (DataRow rr in r_PPool)
            {
                //这三种需要继续往下算
                //20-5-6 委外底下还有自制的料 ///|| Convert.ToBoolean(rr["子项自制"])
                if (Convert.ToBoolean(rr["子项委外"]) || rr["WIPType"].ToString().Trim() == "虚拟")
                {
                    decimal dec_需 = Convert.ToDecimal(dec) * Convert.ToDecimal(rr["数量"]); //父项所缺数*bom数量                                                                   //递归
                    #region 20-5-8 加
                    if (Convert.ToBoolean(rr["子项委外"])) //20-5-8
                    {
                        //decimal dec_需 = Convert.ToDecimal(dec) * Convert.ToDecimal(rr["数量"]); //父项所缺数*bom数量

                        DataRow[] r_total = dt_totalcount.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                        decimal total = 0;
                        decimal kczs = 0;
                        decimal dec_wl = 0;
                        decimal dec_zt = 0;
                        if (r_total.Length == 0)
                        {
                            total = 0;
                            kczs = 0;
                            dec_wl = 0;
                            dec_zt = 0;
                        }
                        total = Convert.ToDecimal(r_total[0]["总数"]);
                        kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                        dec_wl = Convert.ToDecimal(r_total[0]["未领量"]);
                        dec_zt = Convert.ToDecimal(r_total[0]["在途量"]);
                        //decimal dec_n = 0;
                        r_total[0]["需求数量"] = Convert.ToDecimal(r_total[0]["需求数量"]) + dec_需;
                        if (total - dec_需 > 0) //不缺
                        {
                            r_total[0]["总数"] = total - dec_需;
                        }
                        else //缺了
                        {
                            //同一天的同一物料累加
                            DataRow[] fr = dtM.Select($"物料编码='{ rr["子项编码"].ToString()}' and 预计开工日期='{time_kg}'");
                            if (fr.Length > 0)
                            {
                                fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec_需 - total;

                            }
                            else
                            {
                                DataRow r_need = dtM.NewRow();
                                r_need["未领量"] = dec_wl;
                                r_need["在途量"] = dec_zt;
                                r_need["委外在途"] = Convert.ToDecimal(r_total[0]["委外在途"]);
                                r_need["预计开工日期"] = time_kg;
                                r_need["需求来料日期"] = Convert.ToDateTime(r_need["预计开工日期"]).AddDays(-3).Date;
                                r_need["物料编码"] = r_total[0]["物料编码"].ToString();
                                r_need["仓库号"] = r_total[0]["默认仓库号"].ToString();
                                r_need["仓库名称"] = r_total[0]["仓库名称"].ToString();
                                r_need["未发量"] = r_total[0]["未发量"].ToString();
                                r_need["供应商编号"] = r_total[0]["供应商编号"].ToString();
                                r_need["默认供应商"] = r_total[0]["默认供应商"].ToString();
                                r_need["采购员"] = r_total[0]["采购员"].ToString();
                                r_need["物料名称"] = r_total[0]["物料名称"].ToString();
                                r_need["规格型号"] = r_total[0]["规格型号"].ToString();
                                r_need["存货分类"] = r_total[0]["存货分类"].ToString();
                                r_need["库存总数"] = kczs;
                                r_need["受订量"] = r_total[0]["受订量"].ToString();
                                r_need["自制"] = r_total[0]["自制"].ToString();
                                r_need["委外"] = r_total[0]["委外"].ToString();
                                r_need["ECN"] = r_total[0]["ECN"].ToString();
                                r_need["可购"] = r_total[0]["可购"].ToString();
                                r_need["已采未审"] = r_total[0]["已采未审"].ToString();
                                r_need["采购未送检"] = r_total[0]["采购未送检"].ToString();
                                r_need["已送未检"] = r_total[0]["已送未检"].ToString();
                                r_need["已检未入"] = r_total[0]["已检未入"].ToString();
                                r_need["库存下限"] = r_total[0]["库存下限"].ToString();
                                r_need["采购周期"] = r_total[0]["采购周期"].ToString();
                                r_need["最小包装"] = r_total[0]["最小包装"].ToString();
                                //20-1-8
                                r_need["供应状态"] = r_total[0]["供应状态"].ToString();
                                //20-1-14
                                r_need["停用"] = r_total[0]["停用"].ToString();
                                r_need["订单用量"] = r_total[0]["订单用量"].ToString();
                                //r_need["订单缺料"] = Convert.ToDecimal(r_total[0]["总数"]) - Convert.ToDecimal(dr["在途量"]);
                                r_need["参考数量"] = dec_需 - total;
                                dtM.Rows.Add(r_need);
                            }
                            r_total[0]["总数"] = 0;
                            main_Plan_calu_dg(dtM, dt_bom, dt_totalcount, rr["子项编码"].ToString(), dec_需 - total, time_kg);

                        }
                    }



                    #endregion



                }
                else if (Convert.ToBoolean(rr["子项可购"])) //直接原材料
                {
                    decimal dec_需 = Convert.ToDecimal(dec) * Convert.ToDecimal(rr["数量"]); //父项所缺数*bom数量

                    DataRow[] r_total = dt_totalcount.Select(string.Format("物料编码='{0}'", rr["子项编码"]));
                    decimal total = 0;
                    decimal kczs = 0;
                    decimal dec_wl = 0;
                    decimal dec_zt = 0;
                    if (r_total.Length == 0)
                    {
                        total = 0;
                        kczs = 0;
                        dec_wl = 0;
                        dec_zt = 0;
                    }
                    total = Convert.ToDecimal(r_total[0]["总数"]);
                    kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                    dec_wl = Convert.ToDecimal(r_total[0]["未领量"]);
                    dec_zt = Convert.ToDecimal(r_total[0]["在途量"]);
                    //decimal dec_n = 0;
                    r_total[0]["需求数量"] = Convert.ToDecimal(r_total[0]["需求数量"]) + dec_需;
                    if (total - dec_需 > 0) //不缺
                    {
                        r_total[0]["总数"] = total - dec_需;
                    }
                    else //缺了
                    {
                        //同一天的同一物料累加
                        DataRow[] fr = dtM.Select($"物料编码='{ rr["子项编码"].ToString()}' and 预计开工日期='{time_kg}'");
                        if (fr.Length > 0)
                        {
                            fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec_需 - total;
                        }
                        else
                        {
                            DataRow r_need = dtM.NewRow();
                            r_need["未领量"] = dec_wl;
                            r_need["在途量"] = dec_zt;
                            r_need["委外在途"] = Convert.ToDecimal(r_total[0]["委外在途"]);
                            r_need["预计开工日期"] = time_kg;
                            r_need["需求来料日期"] = Convert.ToDateTime(r_need["预计开工日期"]).AddDays(-3).Date;
                            r_need["物料编码"] = r_total[0]["物料编码"].ToString();
                            r_need["仓库号"] = r_total[0]["默认仓库号"].ToString();
                            r_need["仓库名称"] = r_total[0]["仓库名称"].ToString();
                            r_need["未发量"] = r_total[0]["未发量"].ToString();
                            r_need["供应商编号"] = r_total[0]["供应商编号"].ToString();
                            r_need["默认供应商"] = r_total[0]["默认供应商"].ToString();
                            r_need["采购员"] = r_total[0]["采购员"].ToString();
                            r_need["物料名称"] = r_total[0]["物料名称"].ToString();
                            r_need["规格型号"] = r_total[0]["规格型号"].ToString();
                            r_need["存货分类"] = r_total[0]["存货分类"].ToString();
                            r_need["库存总数"] = kczs;
                            r_need["受订量"] = r_total[0]["受订量"].ToString();
                            r_need["自制"] = r_total[0]["自制"].ToString();
                            r_need["委外"] = r_total[0]["委外"].ToString();
                            r_need["ECN"] = r_total[0]["ECN"].ToString();
                            r_need["可购"] = r_total[0]["可购"].ToString();
                            r_need["已采未审"] = r_total[0]["已采未审"].ToString();
                            r_need["采购未送检"] = r_total[0]["采购未送检"].ToString();
                            r_need["已送未检"] = r_total[0]["已送未检"].ToString();
                            r_need["已检未入"] = r_total[0]["已检未入"].ToString();
                            r_need["库存下限"] = r_total[0]["库存下限"].ToString();
                            r_need["采购周期"] = r_total[0]["采购周期"].ToString();
                            r_need["最小包装"] = r_total[0]["最小包装"].ToString();
                            //20-1-8
                            r_need["供应状态"] = r_total[0]["供应状态"].ToString();
                            //20-1-14
                            r_need["停用"] = r_total[0]["停用"].ToString();
                            r_need["订单用量"] = r_total[0]["订单用量"].ToString();
                            //r_need["订单缺料"] = Convert.ToDecimal(r_total[0]["总数"]) - Convert.ToDecimal(dr["在途量"]);
                            r_need["参考数量"] = dec_需 - total;
                            dtM.Rows.Add(r_need);
                            r_total[0]["总数"] = 0;
                        }
                    }
                }
                //即子项自制（只有自制属性,不可购,不委外）暂时不往下算
            }
        }
        /// <summary>
        /// 2020-3-30新增
        /// 每天流水号从1开始,每日重置
        /// 返回起始值并自动根据growth 增加相应数
        /// </summary>
        /// <param name="strType">产品简码</param>
        /// <param name="Y"></param>
        /// <param name="M"></param>
        /// <param name="D"></param>
        /// <returns></returns>
        public static int fun_SN流水号(string strType, DateTime t, int Growth)
        {
            string sql;
            sql = $"select * from [sn号每日生成流水表] where 产品简码 = '{strType}' and 年 = '{t.Year}' and 月 = '{t.Month}' and 日='{t.Day}'";
            SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn);
            new SqlCommandBuilder(da);
            DataTable dt = new DataTable();
            da.Fill(dt);
            int ir = 0;
            if (dt.Rows.Count == 0)
            {
                ir = 1;
                dt.Rows.Add(new object[] { strType, t.Year.ToString(), t.Month.ToString(), Growth, t.Day.ToString() });
            }
            else
            {
                ir = (int)dt.Rows[0]["流水"] + 1;
                dt.Rows[0]["流水"] = (int)dt.Rows[0]["流水"] + Growth;
            }
            try
            {
                da.Update(dt);
                return ir;
            }
            catch
            {
                return 0;
            }
        }


        //codesoft 打印总是有lppa.exe 进程占满内存的问题，加一个打印完了杀一下进程
        public void kill_lppa()
        {
            var process = Process.GetProcesses().Where(pr => pr.ProcessName.Contains("lppa.exe"));
            foreach (var pk in process)
            {
                try
                {
                    pk.Kill();
                }
                catch
                {
                    continue;
                }
            }
        }


        [DllImport("kernel32.dll")]
        private static extern bool SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        //刷新存储器 
        //2020-6-1 新增
        public static void FlushMemory()
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
                }
            }
            catch (Exception)
            {


            }

        }

        /// <summary>
        /// 事务保存 多个dt
        /// </summary>
        public void save(Dictionary<string, DataTable> dic)
        {
            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("bjsw");
            try
            {
                SqlCommand cmd = new SqlCommand("", conn, ts);
                SqlDataAdapter da;
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                foreach (var v in dic)
                {
                    cmd.CommandText = $"select * from {v.Key} where 1=2";
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(v.Value);

                }
                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw new Exception(ex.Message);
            }

        }


    }
}
