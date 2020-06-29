using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Text;

namespace ItemInspectionData
{
    public class AddData
    {
        public static string erroNo { get; set; }//批次日志
        public static string GUID { get; set; }//批次GUID
        public static string Operator { get; set; }//批次操作员

        public static System.Data.DataTable dtM { get; set; }
        public static System.Windows.Forms.OpenFileDialog Ofd { get; set; }//地址

        public static string Operate { get; set; }//操作
        public static string CurrentState { get; set; }//当前状态
        public static int AllRow { get; set; }//总行（行）
        public static int NewRow { get; set; }//当前行（行）
        public static int AllRowF { get; set; }//分组总行（行）
        public static int NewRowF { get; set; }//当前分组行（行）
        public static int AllCode { get; set; }//Code总数(组)
        public static int NewRowCode { get; set; }//成功导入Code（组）


        private static System.Data.DataTable dt_产品CODE表;

        //读取Excel**********************************************

        /// <summary>
        /// 读取Excel.xlsx
        /// </summary>
        /// <param name="fd">Excel.xlsx文件所在路径</param>
        public static void ExcelXLSX(System.Windows.Forms.OpenFileDialog fd)
        {
            CurrentState = "读取Excel数据...";
            try
            {
                dtM = new DataTable();
                string strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + fd.FileName + ";Extended Properties='Excel 12.0; HDR=YES; IMEX=1'"; //此連接可以操作.xls與.xlsx文件
                OleDbConnection conn = new OleDbConnection(strConn);
                conn.Open();
                DataSet ds = new DataSet();
                OleDbDataAdapter odda = new OleDbDataAdapter(string.Format("SELECT * FROM [{0}]", "data$"), conn); //("select * from [Sheet1$]", conn);
                odda.Fill(ds, "data$");
                conn.Close();
                dtM = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int DataArray()
        {
            System.Windows.Forms.OpenFileDialog fd = Ofd;
            Operate = "";
            CurrentState = "";
            AllRow = 0;
            NewRow = 0;
            AllRowF = 0;
            NewRowF = 0;
            AllCode = 0;
            NewRowCode = 0;
            
            //int i=0;

            ///准备数据 - 数据库
            //产品CODE表
            System.Data.DataTable dt1 = new DataTable(); 
            fun_readDataCode(dt1);

            //数据库CODE号集合
            List<string> lis_1 = new List<string>();

            foreach (DataRow r in dt1.Rows)
            {
                if (lis_1.Contains(r["产品CODE号"].ToString()))
                {

                }
                else
                {
                    lis_1.Add(r["产品CODE号"].ToString());
                }
            }


            ///导入数据 - excel---dtM
            ExcelXLSX(fd);

            AllRow = dtM.Rows.Count;

            CPublic.FCCommon.StartCheck();//哈希表

            

            ///接下来是主体:
            ///
            Dictionary<string, List<DataRow>> CSD=new Dictionary<string,List<DataRow>> ();
            GetCodeDic(CSD);

            AllCode = CSD.Count;
            NewRowCode = 0;
            CurrentState = "数据导入数据库";

            foreach (KeyValuePair<string, List<DataRow>> kvp in CSD)
            {
                //一组数据kvp

                ///检查Rs工序是否合要求
                ///如果以上后个检查不通过 记录到日志
                ///如果以上二个检查 通过..
                if (CheckDataArray(kvp, lis_1) == 1)
                {
                    continue;
                }

                dt_产品CODE表 = new System.Data.DataTable();

                //    ///从数据库得到二表个值.删除二个表的值     
                fun_deleteDT(kvp.Key, dt_产品CODE表);
                //    ///回写新增的数据到三个表里
                fun_addDT(dt_产品CODE表, kvp);
                //    ///提交数据,事务处理    
                SqlTransactionSaveData(dt_产品CODE表, kvp);
                //    NewRowCode++;
                //    System.Windows.Forms.Application.DoEvents();
                //}
                //if (erroNo != "")
                //{
                //    ex(erroNo);
                //}
            }
            return 0;
        }


        /// <summary>
        /// 读——产品CODE表(CODE)
        /// </summary>
        private static void fun_readDataCode(System.Data.DataTable dt)
        {
            string sql = "select [产品CODE号] from [MESDB].[dbo].[产品CODE表]";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                try
                {
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        /// <summary>
        /// 分批
        /// </summary>
        /// <param name="dtM">Excel文件</param>
        /// <returns></returns>
        private static void GetCodeDic(Dictionary<string, List<DataRow>> Dic)
        {
            CurrentState = "Excel数据分组";
            AllRowF = dtM.Rows.Count;
            string oCode = "";
            string str = "";
            string str_1 = "";
            for (int i = 0; i < dtM.Rows.Count; i++)
            {
                if (dtM.Rows[i]["SAP编码"].ToString() != "")
                {
                    str = dtM.Rows[i]["SAP编码"].ToString();
                }


                if (oCode != str)//str一直存在和oCode变量配合用来判断一组数据的范围(进行状态翻转)
                {
                    oCode = str;
                    if (Dic.ContainsKey(str))//数据组CODE重复
                    {
                        oCode = "";

                        if (str_1 != str)//str_1使一组出错数据只向日志内添加一条记录
                        {
                            erroNo += "出错行号：" + dtM.Rows[i]["POS"].ToString() + ",出错CODE号：" + str + "，原因：数据组CODE重复！" + System.Environment.NewLine;
                        }
                        str_1 = str;
                    }
                    else
                    {
                        Dic.Add(str, new List<DataRow>());
                        Dic[str].Add(dtM.Rows[i]);
                    }
                }
                else
                {
                    Dic[str].Add(dtM.Rows[i]);
                }
                NewRowF = i + 1;
            }
        }
        /// <summary>
        /// 检查
        /// </summary>
        /// <param name="kvp"></param>
        /// <param name="f"></param>
        /// <param name="li_1"></param>
        /// <param name="li_2"></param>
        /// <param name="li_3"></param>
        /// <returns></returns>
        private static int CheckDataArray(KeyValuePair<string, List<DataRow>> kvp , List<string> li_1)
        {
            int i = 0;
            i = CheckNewData( li_1, kvp);
            if (i == 1)
            {
                return 1;
            }
            return 0;
        }
        /// <summary>
        /// 新增、覆盖与数据库是否重复
        /// </summary>
        /// <param name="f"></param>
        /// <param name="li_1"></param>
        /// <param name="li_2"></param>
        /// <param name="kvp"></param>
        /// <param name="li_3"></param>
        /// <returns></returns>
        private static int CheckNewData( List<string> li_1, KeyValuePair<string, List<DataRow>> kvp)
        {
           
            //判断是否与数据库重复
            if (li_1.Contains(kvp.Key))
            {
                erroNo += "出错行号：" + "出错CODE号:" + kvp.Key + "，原因：新增CODE与数据库重复！" + System.Environment.NewLine;
                return 1;
            }
            return 0;
        }
        /// <summary>
        /// 从数据库得到一表个值.删除一个表的值,二表初始化
        /// </summary>
        /// <param name="CODE"></param>
        /// <param name="dtM1"></param>
        /// <param name="dtM2"></param>
        private static void fun_deleteDT(string CODE, System.Data.DataTable dtM1)
        {
            string sql_1 = "select * from [MESDB].[dbo].[产品CODE表] where [产品CODE号]= '" + CODE + "'";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_1, CPublic.Var.strConn))
            {
                try
                {
                    da.Fill(dtM1);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            foreach (DataRow r in dtM1.Rows)
            {
                r.Delete();
            }
        }



        private static void fun_addDT(System.Data.DataTable dtM1, KeyValuePair<string, List<DataRow>> kvp)
        {
            DataRow r1 = dtM1.NewRow();
            r1["产品CODE号"] = kvp.Key;
            r1["产品编码"] = kvp.Value[0]["K3编码"].ToString().Trim();
            r1["物料描述"] = kvp.Value[0]["物料描述"].ToString().Trim();
            r1["图号"] = kvp.Value[0]["图号"].ToString().Trim() + kvp.Value[0]["版本"].ToString().Trim();//图号+图号版本号

            if (r1["流转卡类型"].ToString() == "产品加工流转卡_条码" || r1["流转卡类型"] .ToString()== "无菌产品加工流转卡_条码")
            {
                r1["材料编码"] = kvp.Value[0]["K3用料"].ToString().Trim();
                //r1["SAP材料编码"] = kvp.Value[0]["DMF1用料"].ToString().Trim();

                r1["材料规格"] = "φ" + kvp.Value[0]["用料规格"].ToString().Trim();
                r1["材料牌号"] = "";
                r1["材料定额"] = kvp.Value[0]["用料数量"].ToString().Trim();
                r1["材料消耗"] = kvp.Value[0]["SAP用料长度"].ToString().Trim();
                
            }

            r1["流转卡编号"] = "";//MPS+MPS版本号
            r1["新产品"] = "";







            r1["发送"] = true;
            r1["停用"] = false;
            r1["修改确认"] = false;
            r1["包装材料"] = "";
            r1["工序起始编号"] = 0;
            r1["默认单元"] = "";
            r1["成品类型"] = "";
            r1["包装件"] = "";
            r1["流转卡类型"] = "";

            string mark = "螺钉" + DateTime.Now.ToString("_yyyy_MM_dd_HHmm");

            dtM1.Rows.Add(r1);

        }


        private static void SqlTransactionSaveData(System.Data.DataTable dt1, KeyValuePair<string, List<DataRow>> kvp)
        {
            //string str = "";

            using (SqlConnection conn = new SqlConnection(CPublic.Var.strConn))
            {
                SqlTransaction transaction = null;
                try
                {
                    ///文件断层使用的数据表
                    System.Data.DataTable dt_CODE = new System.Data.DataTable();

                    string sql;

                    conn.Open();
                    transaction = conn.BeginTransaction("CFileVibrationTransaction");


                    sql = "select * from [MESDB].[dbo].[产品CODE表] where 1<>1 ";
                    SqlDataAdapter da_产品CODE表 = new SqlDataAdapter(new SqlCommand(sql, conn, transaction));
                    new SqlCommandBuilder(da_产品CODE表);
                    da_产品CODE表.Fill(dt_CODE);

                    dt_CODE = dt1;
                    try
                    {
                        da_产品CODE表.Update(dt_CODE);
                        transaction.Commit();
                        dt_CODE.Clear();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        erroNo += "出错行数：" + kvp.Value[0]["POS"].ToString() + "，出错原因：" + ex.Message + System.Environment.NewLine;

                        NewRowCode--;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
