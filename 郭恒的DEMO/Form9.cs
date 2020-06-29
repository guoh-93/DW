using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Drawing.Printing;
using System.Drawing;
using Spire.Xls;

namespace 郭恒的DEMO
{
    public partial class Form9 : Form
    {
        DataTable dt = new DataTable();
        string strcon = CPublic.Var.strConn;
        public Form9()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {


            string path = @"C:\Users\Administrator\Desktop\吴飞飞";

            DirectoryInfo TheFolder = new DirectoryInfo(path);
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                string s = NextFolder.Name;
                string[] ss = s.Split('-');

                int y = Convert.ToInt32(ss[0].ToString());
                int M = Convert.ToInt32(ss[1].ToString());

                //遍历文件
                foreach (FileInfo NextFile in NextFolder.GetFiles())
                {
                    if (NextFile.Name.Contains("成品"))  //成品 半成品 模板一样
                    {
                        string wllx = "";
                        if (NextFile.Name.Contains("半成品"))
                        {
                            wllx = "半成品";
                        }
                        else
                        {
                            wllx = "成品";
                        }
                        DataTable temp = new DataTable();
                        // temp = dt.Clone();
                        try
                        {
                            temp = ExcelXLSX(wllx, y, M, NextFile.FullName);

                            dt.Merge(temp);

                        }
                        catch
                        {


                        }
                    }
                    else
                    {
                        continue;
                    }
                }

            }

            string sql = "select  * from 结存表新 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                new SqlCommandBuilder(da);
                foreach (DataRow dr in dt.Rows)
                {
                    dr.SetAdded();
                }



                //dt.Columns[7].ColumnName = "上期结存";
                //dt.Columns[3].ColumnName = "物料编号";
                //dt.Columns[20].ColumnName = "入库合计";
                //dt.Columns[33].ColumnName = "出库合计";
                //dt.Columns[35].ColumnName = "期末结存";




                da.Update(dt);
            }


        }

        private void Form9_Load(object sender, EventArgs e)
        {


        }

        public DataTable ExcelXLSX(string str_物料类型, int y, int M, string filename)
        {

            try
            {
                DataTable dt = new DataTable();
                string strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + filename + ";Extended Properties='Excel 12.0; HDR=YES; IMEX=1'"; //此連接可以操作.xls與.xlsx文件
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
                    if (!strSheetTableName.Contains("进销存") && !strSheetTableName.Contains("收发存"))
                    {
                        continue;
                    }

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
                    DataColumn dc = new DataColumn("年", typeof(int));
                    dc.DefaultValue = y;
                    dt2.Columns.Add(dc);
                    DataColumn dc1 = new DataColumn("月", typeof(int));
                    dc1.DefaultValue = M;
                    dt2.Columns.Add(dc1);
                    DataColumn dc2 = new DataColumn("物料类型", typeof(string));
                    dc2.DefaultValue = str_物料类型;
                    dt2.Columns.Add(dc2);
                    dt2.Columns[3].ColumnName = "F4";
                    DataRow[] dr = dt2.Select(string.Format("F4='物料编号'"));
                    if (dr.Length > 0)
                    {
                        foreach (DataColumn c in dt.Columns)
                        {
                            if (dr[0][c.ColumnName].ToString() == "物料编号") { c.ColumnName = "物料编号"; continue; }

                            if (dr[0][c.ColumnName].ToString() == "上期结存") { c.ColumnName = "上期结存"; continue; }
                            if (dr[0][c.ColumnName].ToString() == "入库数量合计") { c.ColumnName = "入库合计"; continue; }
                            if (dr[0][c.ColumnName].ToString() == "出库合计") { c.ColumnName = "出库合计"; continue; }
                            if (dr[0][c.ColumnName].ToString() == "期末结存数量") { c.ColumnName = "期末结存"; continue; }

                        }

                    }


                    dt.Merge(dt2);
                }

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }


        private void button2_Click_2(object sender, EventArgs e)
        {
            dt = new DataTable();
            string path = @"C:\Users\Administrator\Desktop\吴飞飞";

            DirectoryInfo TheFolder = new DirectoryInfo(path);
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                string s = NextFolder.Name;
                string[] ss = s.Split('-');

                int y = Convert.ToInt32(ss[0].ToString());
                int M = Convert.ToInt32(ss[1].ToString());

                //遍历文件
                foreach (FileInfo NextFile in NextFolder.GetFiles())
                {
                    if (NextFile.Name.Contains("原材料"))  //成品 半成品 模板一样
                    {
                        string wllx = "原材料";

                        DataTable temp = new DataTable();
                        // temp = dt.Clone();
                        try
                        {
                            temp = ExcelXLSX(wllx, y, M, NextFile.FullName);

                            dt.Merge(temp);

                        }
                        catch
                        {


                        }
                    }
                    else
                    {
                        continue;
                    }
                }

            }

            string sql = "select  * from 结存表原材料新 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                new SqlCommandBuilder(da);
                foreach (DataRow dr in dt.Rows)
                {
                    dr.SetAdded();
                }

                //dt.Columns[7].ColumnName = "上期结存";
                //dt.Columns[3].ColumnName = "物料编号";
                //dt.Columns[16].ColumnName = "入库合计";
                //dt.Columns[25].ColumnName = "出库合计";
                //dt.Columns[27].ColumnName = "期末结存"

                da.Update(dt);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataTable dtm = new DataTable();
            string sql = "select  物料编号,年,月,入库合计,期末结存  from 结存表 where   年=2017 and 月=6 ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                da.Fill(dtm);
                dtm.Columns.Add("账龄", typeof(int));
                foreach (DataRow dr in dtm.Rows)
                {
                    //累加入库合计
                    decimal i = 0;
                    if (Convert.ToDecimal(dr["期末结存"]) <= Convert.ToDecimal(dr["入库合计"]))
                    {
                        dr["账龄"] = 0;
                    }
                    else
                    {
                        i = Convert.ToDecimal(dr["入库合计"]);
                        int year = Convert.ToInt32(dr["年"]);
                        int M = Convert.ToInt32(dr["月"]);
                        string s_物料 = dr["物料编号"].ToString();
                        int i_结存 = Convert.ToInt32(dr["期末结存"]);
                        decimal x = fun_dg(year, M, i, s_物料, i_结存);
                        dr["账龄"] = x;
                    }
                }

            }
            gridControl1.DataSource = dtm;
        }

        private decimal fun_dg(int year, int M, decimal i, string str_物料, decimal i_结存)
        {
            decimal x;
            M = M - 1;
            if (M == 0)
            {
                year = year - 1;
                M = 12;
            }
            if (M == 2013)
            {
                return -1;
            }
            string s = string.Format("select * from 结存表 where 年='{0}' and 月={1} and 物料编号='{2}'", year, M, str_物料);
            using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
            {
                DataTable t = new DataTable();
                da.Fill(t);
                if (Convert.ToDecimal(t.Rows[0]["入库合计"]) + i > i_结存)
                {
                    x = (2017 - year) * 12 + (6 - M);
                    return x;
                }
                else
                {
                    i = i + Convert.ToDecimal(t.Rows[0]["入库合计"]);
                    fun_dg(year, M, i, str_物料, i_结存);
                }
            }
            return -1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xls)|*.xls";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsExportOptions options = new DevExpress.XtraPrinting.XlsExportOptions();

                gridView1.ExportToXlsx(saveFileDialog.FileName);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string x = "413000501000000013285";
            fun_gccode(x);
        }

        private string fun_gccode(string sn)
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
            int mod = 10 - sum % 10;
            s = mod.ToString();
            return s;
        }
        //转 其他出入库 
        private void button6_Click(object sender, EventArgs e)
        {
            string s = "select  单号,出库入库 from  盘点调整库存出入库明细表  group by 单号,出库入库  order  by 单号";
            DataTable dt_单号 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = "select  * from  其他出入库申请主表 where 1<>1";
            DataTable dt_其他出申请主表 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select  * from  其他出入库申请子表 where 1<>1";
            DataTable dt_其他出申请子表 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = "select  * from  其他出库主表 where 1<>1";
            DataTable dt_其他出主表 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select  * from  其他出库子表 where 1<>1";
            DataTable dt_其他出子表 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = "select  * from  其他入库主表 where 1<>1";
            DataTable dt_其他入主表 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select  * from  其他入库子表 where 1<>1";
            DataTable dt_其他入子表 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            string sql = "select * from 仓库出入库明细表 where 1<>1";
            DataTable dt_出入库明细 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            da.Fill(dt_出入库明细);
            foreach (DataRow dr in dt_单号.Rows)
            {

                #region 其他申请主 不用区分 入库还是出库
                int year = Convert.ToInt32(dr["单号"].ToString().Substring(2, 4));
                int month = Convert.ToInt32(dr["单号"].ToString().Substring(6, 2));
                DateTime t = new DateTime(year, month, 25);
                string s申请_no = string.Format("QWSQ{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
             t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QWSQ", t.Year, t.Month).ToString("0000"));
                DataRow dr_申请主 = dt_其他出申请主表.NewRow();

                dr_申请主["GUID"] = System.Guid.NewGuid();
                dr_申请主["出入库申请单号"] = s申请_no;
                dr_申请主["申请日期"] = t;
                dr_申请主["操作人员编号"] = "8423";
                dr_申请主["操作人员"] = "谢刚华";
                dr_申请主["生效"] = true;
                dr_申请主["完成"] = true;
                dr_申请主["生效人员编号"] = "8423";
                dr_申请主["生效日期"] = t;
                dr_申请主["完成日期"] = t;
                dr_申请主["备注"] = "盘点调整";
                dr_申请主["申请类型"] = "其他" + dr["出库入库"].ToString();
                dr_申请主["原因分类"] = "盘点";
                dt_其他出申请主表.Rows.Add(dr_申请主);

                #endregion

                if (dr["出库入库"].ToString() == "出库")
                {
                    #region 其他出库主
                    string s出库_no = string.Format("QT{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                          t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QT", t.Year, t.Month).ToString("0000"));
                    DataRow dr_出库主 = dt_其他出主表.NewRow();
                    dr_出库主["GUID"] = System.Guid.NewGuid();
                    dr_出库主["其他出库单号"] = s出库_no;
                    dr_出库主["创建日期"] = t;
                    dr_出库主["操作人员编号"] = "8209";
                    dr_出库主["操作人员"] = "吴艳妃";
                    dr_出库主["出库仓库"] = "";

                    dr_出库主["生效"] = true;
                    dr_出库主["生效人员编号"] = "8209";
                    dr_出库主["生效日期"] = t;
                    dr_出库主["出库日期"] = t;
                    dr_出库主["出库类型"] = "盘点" + dr["出库入库"].ToString();
                    dr_出库主["出入库申请单号"] = s申请_no;
                    dt_其他出主表.Rows.Add(dr_出库主);
                    #endregion

                    //一条一个 其他出入库申请    
                    string ss = string.Format("select  盘点调整库存出入库明细表.*,物料编码,物料名称,n原ERP规格型号 from  盘点调整库存出入库明细表,基础数据物料信息表  where 盘点调整库存出入库明细表.原物料编号= 基础数据物料信息表.原ERP物料编号 and 单号='{0}' and 出库入库='{1}'", dr["单号"], dr["出库入库"]);
                    DataTable dt_盘点明细 = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
                    int pos = 1;
                    foreach (DataRow r in dt_盘点明细.Rows)
                    {
                        if (r.RowState == DataRowState.Deleted) continue;
                        #region  仓库出入库明细
                        DataRow drr = dt_出入库明细.NewRow();

                        drr["GUID"] = System.Guid.NewGuid();

                        drr["明细类型"] = "盘点" + dr["出库入库"].ToString();


                        drr["单号"] = s出库_no;


                        drr["相关单号"] = s申请_no;
                        drr["相关单位"] = "计划课";
                        drr["物料编码"] = r["物料编码"].ToString();


                        drr["物料名称"] = r["物料名称"].ToString();

                        drr["明细号"] = s出库_no + "-" + pos.ToString("00");


                        drr["出库入库"] = dr["出库入库"].ToString();

                        drr["数量"] = (Decimal)0;


                        drr["标准数量"] = (Decimal)0;


                        drr["实效数量"] = -Convert.ToDecimal(r["盘亏盘盈数"]);



                        drr["实效时间"] = t;
                        drr["出入库时间"] = t;


                        dt_出入库明细.Rows.Add(drr);


                        #endregion
                        #region 其他申请子表记录

                        DataRow dr_其他出申请子表 = dt_其他出申请子表.NewRow();

                        dr_其他出申请子表["GUID"] = System.Guid.NewGuid();
                        dr_其他出申请子表["出入库申请单号"] = s申请_no;
                        dr_其他出申请子表["出入库申请明细号"] = s申请_no + "-" + pos.ToString("00");
                        dr_其他出申请子表["POS"] = pos;
                        dr_其他出申请子表["物料编码"] = r["物料编码"].ToString();
                        dr_其他出申请子表["物料名称"] = r["物料名称"].ToString();
                        dr_其他出申请子表["原ERP物料编号"] = r["原物料编号"].ToString();
                        dr_其他出申请子表["数量"] = -Convert.ToDecimal(r["盘亏盘盈数"]);
                        dr_其他出申请子表["n原ERP规格型号"] = r["n原ERP规格型号"].ToString();
                        dr_其他出申请子表["完成"] = true;
                        dr_其他出申请子表["完成日期"] = t;
                        dr_其他出申请子表["生效"] = true;
                        dr_其他出申请子表["生效人员编号"] = "8423";
                        dr_其他出申请子表["生效日期"] = t;
                        dt_其他出申请子表.Rows.Add(dr_其他出申请子表);

                        #endregion

                        #region 其他出库子表记录
                        DataRow dr_其他出子表 = dt_其他出子表.NewRow();
                        dr_其他出子表["GUID"] = System.Guid.NewGuid();
                        dr_其他出子表["其他出库单号"] = s出库_no;
                        dr_其他出子表["其他出库明细号"] = s出库_no + "-" + pos.ToString("00");
                        dr_其他出子表["POS"] = pos;
                        dr_其他出子表["物料编码"] = r["物料编码"].ToString();
                        dr_其他出子表["物料名称"] = r["物料名称"].ToString();
                        dr_其他出子表["原ERP物料编号"] = r["原物料编号"].ToString();
                        dr_其他出子表["数量"] = -Convert.ToDecimal(r["盘亏盘盈数"]);
                        dr_其他出子表["n原ERP规格型号"] = r["n原ERP规格型号"].ToString();
                        dr_其他出子表["完成"] = true;
                        dr_其他出子表["完成日期"] = t;
                        dr_其他出子表["生效"] = true;
                        dr_其他出子表["生效人员编号"] = "8209";
                        dr_其他出子表["生效日期"] = t;
                        dr_其他出子表["出入库申请单号"] = s申请_no;
                        dr_其他出子表["出入库申请明细号"] = s申请_no + "-" + pos.ToString("00");
                        dt_其他出子表.Rows.Add(dr_其他出子表);

                        #endregion
                        pos++;
                    }
                }
                else  //走其他入库
                {

                    #region 其他入库主
                    string s入库_no = string.Format("QW{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                       t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QW", t.Year, t.Month).ToString("0000"));
                    DataRow dr_入库主 = dt_其他入主表.NewRow();
                    dr_入库主["GUID"] = System.Guid.NewGuid();
                    dr_入库主["其他入库单号"] = s入库_no;
                    dr_入库主["创建日期"] = t;
                    dr_入库主["操作人员编号"] = "8209";
                    dr_入库主["操作人员"] = "吴艳妃";


                    dr_入库主["生效"] = true;
                    dr_入库主["生效人员编号"] = "吴艳妃";
                    dr_入库主["生效日期"] = t;
                    dr_入库主["入库日期"] = t;

                    dr_入库主["出入库申请单号"] = s申请_no;
                    dt_其他入主表.Rows.Add(dr_入库主);
                    #endregion

                    //一条一个 其他出入库申请    
                    string ss = string.Format("select  盘点调整库存出入库明细表.*,物料编码,物料名称,n原ERP规格型号 from  盘点调整库存出入库明细表,基础数据物料信息表  where 盘点调整库存出入库明细表.原物料编号= 基础数据物料信息表.原ERP物料编号 and 单号='{0}' and 出库入库='{1}'", dr["单号"], dr["出库入库"]);
                    DataTable dt_盘点明细 = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
                    int pos = 1;
                    foreach (DataRow r in dt_盘点明细.Rows)
                    {
                        if (r.RowState == DataRowState.Deleted) continue;
                        #region  仓库出入库明细
                        DataRow drr = dt_出入库明细.NewRow();

                        drr["GUID"] = System.Guid.NewGuid();

                        drr["明细类型"] = "盘点入库";


                        drr["单号"] = s入库_no;


                        drr["相关单号"] = s申请_no;
                        drr["相关单位"] = "计划课";
                        drr["物料编码"] = r["物料编码"].ToString();


                        drr["物料名称"] = r["物料名称"].ToString();

                        drr["明细号"] = s入库_no + "-" + pos.ToString("00");


                        drr["出库入库"] = dr["出库入库"].ToString();

                        drr["数量"] = (Decimal)0;


                        drr["标准数量"] = (Decimal)0;


                        drr["实效数量"] = Convert.ToDecimal(r["盘亏盘盈数"].ToString());



                        drr["实效时间"] = t;
                        drr["出入库时间"] = t;


                        dt_出入库明细.Rows.Add(drr);


                        #endregion
                        #region 其他申请子表记录

                        DataRow dr_其他出申请子表 = dt_其他出申请子表.NewRow();

                        dr_其他出申请子表["GUID"] = System.Guid.NewGuid();
                        dr_其他出申请子表["出入库申请单号"] = s申请_no;
                        dr_其他出申请子表["出入库申请明细号"] = s申请_no + "-" + pos.ToString("00");
                        dr_其他出申请子表["POS"] = pos;
                        dr_其他出申请子表["物料编码"] = r["物料编码"].ToString();
                        dr_其他出申请子表["物料名称"] = r["物料名称"].ToString();
                        dr_其他出申请子表["原ERP物料编号"] = r["原物料编号"].ToString();
                        dr_其他出申请子表["数量"] = Convert.ToDecimal(r["盘亏盘盈数"].ToString());
                        dr_其他出申请子表["n原ERP规格型号"] = r["n原ERP规格型号"].ToString();
                        dr_其他出申请子表["完成"] = true;
                        dr_其他出申请子表["完成日期"] = t;
                        dr_其他出申请子表["生效"] = true;
                        dr_其他出申请子表["生效人员编号"] = "8423";
                        dr_其他出申请子表["生效日期"] = t;
                        dt_其他出申请子表.Rows.Add(dr_其他出申请子表);

                        #endregion

                        #region 其他入库子表记录
                        DataRow dr_其他入子表 = dt_其他入子表.NewRow();
                        dr_其他入子表["GUID"] = System.Guid.NewGuid();
                        dr_其他入子表["其他入库单号"] = s入库_no;
                        dr_其他入子表["其他入库明细号"] = s入库_no + "-" + pos.ToString("00");
                        dr_其他入子表["POS"] = pos;
                        dr_其他入子表["物料编码"] = r["物料编码"].ToString();
                        dr_其他入子表["物料名称"] = r["物料名称"].ToString();
                        dr_其他入子表["原ERP物料编号"] = r["原物料编号"].ToString();
                        dr_其他入子表["数量"] = Convert.ToDecimal(r["盘亏盘盈数"].ToString());
                        dr_其他入子表["n原ERP规格型号"] = r["n原ERP规格型号"].ToString();


                        dr_其他入子表["生效"] = true;
                        dr_其他入子表["生效人员编号"] = "8209";
                        dr_其他入子表["生效日期"] = t;
                        dr_其他入子表["出入库申请单号"] = s申请_no;
                        dr_其他入子表["出入库申请明细号"] = s申请_no + "-" + pos.ToString("00");
                        dt_其他入子表.Rows.Add(dr_其他入子表);

                        #endregion
                        pos++;
                    }
                }

            }


            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction zqt = conn.BeginTransaction("转其他出入库");
            try
            {

                string sql1 = "select * from 仓库出入库明细表 where 1<>1";
                SqlCommand cmd1 = new SqlCommand(sql1, conn, zqt);
                SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_出入库明细);

                sql1 = "select * from 其他出入库申请主表  where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, zqt);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_其他出申请主表);
                sql1 = "select * from 其他出入库申请子表  where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, zqt);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_其他出申请子表);
                sql1 = "select * from 其他出库主表  where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, zqt);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_其他出主表);
                sql1 = "select * from 其他出库子表  where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, zqt);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_其他出子表);
                sql1 = "select * from 其他入库主表  where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, zqt);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_其他入主表);
                sql1 = "select * from 其他入库子表  where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, zqt);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_其他入子表);

                zqt.Commit();

            }


            catch (Exception ex)
            {
                zqt.Rollback();
                throw ex;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            string s = "select  * from  仓库出入库明细表 where 出入库时间 ='2018-4-1' ";
            dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            foreach (DataRow dr in dt.Rows)
            {
                string x = dr["明细号"].ToString();
                int Lindex = x.LastIndexOf("-");
                x = x.Substring(0, Lindex);
                dr["明细号"] = x;
            }
            CZMaster.MasterSQL.Save_DataTable(dt, "仓库出入库明细表", strcon);


        }
        //mrp运算 新
        private void button8_Click(object sender, EventArgs e)
        {




        }

        private void fun_mrp_g()
        {

            DataTable dtM = new DataTable();
            dtM.Columns.Add("物料编码");
            dtM.Columns.Add("欠缺数量");
            dtM.Columns.Add("参考量");
            DataTable dt_订单 = new DataTable();
            DataTable dt_库存 = new DataTable();
            DataTable dt_bom = new DataTable();
            string sql = @" select  产品编码,子项编码,数量,b.物料类型 as 父项物料类型,c.物料类型 as 子项物料类型 from 基础数据物料BOM表 a 
                    left join 基础数据物料信息表 b on a.产品编码=b.物料编码  left join 基础数据物料信息表 c on a.子项编码=c.物料编码 ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                da.Fill(dt_bom);
            }
            sql = " select  物料编码,库存总数 from  仓库物料数量表 ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                da.Fill(dt_库存);
            }
            //未销售出库的销售明细 按 物料编码汇总 
            sql = @"select 物料编码,sum(未完成数量)未完成数量 from 销售记录销售订单明细表 where 明细完成=0 and 生效=1 and 作废=0 and 关闭=0 group by 物料编码";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                da.Fill(dt_订单);
            }
            foreach (DataRow dr in dt_订单.Rows)
            {
                DataRow[] r_子项 = dt_bom.Select(string.Format("产品编码='{0}'", dr["物料编码"]));
                if (r_子项.Length > 0) //有子项
                {
                    foreach (DataRow r_子项_2 in r_子项)
                    {

                        if (r_子项.Length > 0) // 
                        {

                        }
                        else  //叶子节点 
                        {


                        }
                    }
                }
                else
                {
                    // dtM.Select(string.Format("物料编码"))

                }

            }




        }

        private void button9_Click(object sender, EventArgs e)
        {
            //string s = " select 成品出库单号  from    销售记录成品出库单主表 where 创建日期<'2019-1-1'";
            //DataTable dt = new DataTable();
            //dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            //s = " select *  from    销售记录成品出库单明细表 where 1=2  ";
            //DataTable dt_save = new DataTable();
            //dt_save = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //foreach (DataRow dr in dt.Rows)
            //{
            //    s = string.Format(" select *  from   销售记录成品出库单明细表 where 成品出库单号='{0}'", dr["成品出库单号"]);
            //    dt_save = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //    int i = 1;
            //    foreach (DataRow r in dt_save.Rows)
            //    {
            //        r["POS"] = i;
            //        r["成品出库单明细号"] = r["成品出库单号"] + "-" + i.ToString("00");
            //        i++;
            //    }
            //    CZMaster.MasterSQL.Save_DataTable(dt_save, "销售记录成品出库单明细表", strcon);

            //}

            // ERPorg.Corg.result s = new ERPorg.Corg.result();

            //// s=  ERPorg.Corg.fun_pool(false);

            // gridControl2.DataSource = s.dtM ;
            // label1.Text = s.str_log;
            string strcon = CPublic.Var.geConn("BQ");
            string s = "select top 100 * from ShareLockInfo ";
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s,strcon);

        }

      

        // The PrintPage event is raised for each page to be printed.
        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            string line = null;
            FileStream fs = new FileStream(@"E:\futureERP\Future\EHERP\BIN\prttmp\其他出库单.xlsx", FileMode.Open);
            StreamReader streamToPrint = new StreamReader(fs, Encoding.Unicode);

            Font printFont = new Font("Arial", 10);
            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height /
               printFont.GetHeight(ev.Graphics);

            // Print each line of the file.
            while (count < linesPerPage &&
               ((line = streamToPrint.ReadLine()) != null))
            {
                yPos = topMargin + (count *
                   printFont.GetHeight(ev.Graphics));
                ev.Graphics.DrawString(line, printFont, Brushes.Black,
                   leftMargin, yPos, new StringFormat());
                count++;
            }

            //4  If more lines exist, print another page.
            //if (line != null)
            //    ev.HasMorePages = true;
            //else
            //    ev.HasMorePages = false;
            fs.Close();
        }


    }

}
