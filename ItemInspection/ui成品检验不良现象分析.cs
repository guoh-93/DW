using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraCharts;
using System.Drawing.Imaging;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace ItemInspection
{
    public partial class ui成品检验不良现象分析 : UserControl
    {

        DataTable dt_所有数据;
        DataTable dt_返工前N项;
        public ui成品检验不良现象分析()
        {
            InitializeComponent();
        }

        private void ui成品检验不良现象分析_Load(object sender, EventArgs e)
        {
            DateTime t = CPublic.Var.getDatetime();
            barEditItem1.EditValue = t.AddDays(-7);
            barEditItem2.EditValue = t;
            barEditItem3.EditValue = "全部";
        }
        //导出
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            导出();
        }
        //关闭
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

     
   

        private void load()
        {
            string str = "";
            string str1 = "";
            dt_所有数据 = new DataTable();
            dt_返工前N项 = new DataTable();

            if (barEditItem3.EditValue.ToString()=="全部")
            {
             str = string.Format(@"select * from (select b.返工原因,(sum(b.数量))数量, ROW_NUMBER() over(order by (sum(b.数量))desc) as rows from 生产记录生产检验单主表 a left join 成品检验检验记录返工表 b on a.生产检验单号=b.生产检验单号
       where a.不合格数量 !=0 and 返工原因 is not null and 送检日期>='{0}' and 送检日期<='{1}' group by b.返工原因)c where c.rows <=5", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));
             str1 = string.Format(@"select b.生产检验单号,b.返工原因,a.物料编码,a.物料名称,a.规格型号,b.数量 from 成品检验检验记录返工表 b left join 生产记录生产检验单主表 a on a.生产检验单号=b.生产检验单号 
      where a.不合格数量 !=0 and 返工原因 is not null and 送检日期>='{0}' and 送检日期<='{1}'", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));
            }
            if (barEditItem3.EditValue.ToString() == "锁具")
            {
                str = string.Format(@"select * from (select b.返工原因,(sum(b.数量))数量, ROW_NUMBER() over(order by (sum(b.数量))desc) as rows from 生产记录生产检验单主表 a left join 成品检验检验记录返工表 b on a.生产检验单号=b.生产检验单号
       where a.负责人员='朱云' and a.不合格数量 !=0 and 返工原因 is not null and 送检日期>='{0}' and 送检日期<='{1}' group by b.返工原因)c where c.rows <=5", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));
                str1 = string.Format(@"select b.生产检验单号,b.返工原因,a.物料编码,a.物料名称,a.规格型号,b.数量 from 成品检验检验记录返工表 b left join 生产记录生产检验单主表 a on a.生产检验单号=b.生产检验单号 
      where a.负责人员='朱云' and a.不合格数量 !=0 and 返工原因 is not null and 送检日期>='{0}' and 送检日期<='{1}'", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));
            }
            if (barEditItem3.EditValue.ToString() == "输入单元")
            {
                str = string.Format(@"select * from (select b.返工原因,(sum(b.数量))数量, ROW_NUMBER() over(order by (sum(b.数量))desc) as rows from 生产记录生产检验单主表 a left join 成品检验检验记录返工表 b on a.生产检验单号=b.生产检验单号
       where a.负责人员='高永凤' and a.不合格数量 !=0 and 返工原因 is not null and 送检日期>='{0}' and 送检日期<='{1}' group by b.返工原因)c where c.rows <=5", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));
                str1 = string.Format(@"select b.生产检验单号,b.返工原因,a.物料编码,a.物料名称,a.规格型号,b.数量 from 成品检验检验记录返工表 b left join 生产记录生产检验单主表 a on a.生产检验单号=b.生产检验单号 
      where a.负责人员='高永凤' and a.不合格数量 !=0 and 返工原因 is not null and 送检日期>='{0}' and 送检日期<='{1}'", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));
            } 
            using(SqlDataAdapter da = new SqlDataAdapter(str,CPublic.Var.strConn))
            {
               da.Fill(dt_返工前N项);
            }
            DataTable dtDa = dt_返工前N项.Copy();
              //把时间段的所有返工原因 和 检验单号加载出来  
            using (SqlDataAdapter da1 = new SqlDataAdapter(str1, CPublic.Var.strConn))
            {
               da1.Fill(dt_所有数据);
            }
           
            
           //返工占比
            decimal a_总的返工数量=0;
            foreach(DataRow dr in dt_所有数据.Rows)
            {
              a_总的返工数量 +=Convert.ToDecimal(dr["数量"]);
            }
            dt_返工前N项.Columns.Add("返工占比",typeof(string));
            foreach(DataRow r in dt_返工前N项.Rows)
            {

                Decimal dbdata =Convert.ToDecimal(r["数量"]) / a_总的返工数量 * 100;
               decimal a = Math.Round(dbdata,2, MidpointRounding.AwayFromZero);//小数点保存两位

                r["返工占比"] = a + "%";

              }
             gridControl1.DataSource = dt_返工前N项;
             fun_饼状图(a_总的返工数量,dtDa);

          }
        //生成饼状图
        private void fun_饼状图(decimal a_总的返工数量,DataTable dtData)
        {
            string hv="";
            string jv="";
            string kv="";
            string lv="";
            string mv="";
            int ax = 0;
            int bx = 0;
            int cx = 0;
            int dx = 0;
            int ex = 0;
            int f_其他 = 0;
            try
            {
            //    DataTable dtData = new DataTable();
            //    dtData.Columns.Add("返工原因", typeof(string));
            //    dtData.Columns.Add("返工占比", typeof(decimal));

            //    string[] sArray = null;
            //    foreach (DataRow dr in dt_返工前N项.Rows)
            //    {
            //        DataRow rr = dtData.NewRow();
            //        dtData.Rows.Add(rr);
            //        rr["返工原因"] = dr["返工原因"].ToString();
            //        sArray = dr["返工占比"].ToString().Split('%');// 一定是单引 
            //        rr["返工占比"] = Convert.ToDecimal(sArray[0]);
            //     }
               
                if(dtData.Rows.Count >0)
                {
                foreach(DataRow rs in dtData.Rows)
                {
                    if (hv.Length == 0)
                    {
                        hv = rs["返工原因"].ToString();
                       ax = Convert.ToInt32(rs["数量"]);
                         continue;
                    }
                    if (jv.Length == 0)
                    {
                        jv = rs["返工原因"].ToString();
                        bx = Convert.ToInt32(rs["数量"]);
                        continue;
                    }
                    if (kv.Length == 0)
                    {
                        kv = rs["返工原因"].ToString();
                        cx = Convert.ToInt32(rs["数量"]);
                        continue;
                    }
                    if (lv.Length == 0)
                    {
                        lv = rs["返工原因"].ToString();
                        dx = Convert.ToInt32(rs["数量"]);
                        continue;
                    }
                    if (mv.Length == 0)
                    {
                        mv = rs["返工原因"].ToString();
                        ex = Convert.ToInt32(rs["数量"]);
                        continue;
                    }

                }
                f_其他 =Convert.ToInt32(a_总的返工数量) - (ax + bx + cx + dx + ex);
                }


                chartControl1.Series.Clear();
                Series s = new Series("不良现象结构图", ViewType.Pie);

                DataTable table = new DataTable("Table1");
                table.Columns.Add("Name", typeof(String));
                table.Columns.Add("Value", typeof(Int32));
                table.Rows.Add(new object[] { hv, ax });
                table.Rows.Add(new object[] { jv, bx });
                table.Rows.Add(new object[] { kv, cx });
                table.Rows.Add(new object[] { lv, dx });
                table.Rows.Add(new object[] { mv, ex });
                table.Rows.Add(new object[] { "其他原因", f_其他 });
                

                s.ValueDataMembers[0] = "Value";
                s.ArgumentDataMember = "Name";
                s.DataSource = table;
                s.LegendPointOptions.PointView = PointView.Argument;

                //s.ShowInLegend = false;
                s.Label.Font = new Font("宋体", 15, FontStyle.Bold);
                s.Label.LineLength = 6;
                ((PiePointOptions)(s.PointOptions)).PercentOptions.ValueAsPercent = true;
                ((PiePointOptions)(s.PointOptions)).PercentOptions.PercentageAccuracy = 4;
                ((PiePointOptions)(s.PointOptions)).ValueNumericOptions.Format = NumericFormat.Percent;
                ((PiePointOptions)(s.PointOptions)).PointView = PointView.Values;
                //s.LegendPointOptions.ValueNumericOptions.Format = NumericFormat.Percent;
                 chartControl1.Series.Add(s);
                (s.Label as PieSeriesLabel).Position = PieSeriesLabelPosition.TwoColumns;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }





        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow rr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                DataRow[] ds = dt_所有数据.Select(string.Format("返工原因='{0}'", rr["返工原因"].ToString()));
                DataTable dt_不良产品 = new DataTable();
                dt_不良产品.Columns.Add("物料编码", typeof(string));
                dt_不良产品.Columns.Add("物料名称", typeof(string));
                dt_不良产品.Columns.Add("规格型号", typeof(string));
                foreach (DataRow dr in ds)
                { 
                    DataRow[] rp = dt_不良产品.Select(string.Format("物料编码='{0}'",dr["物料编码"].ToString()));
                    if(rp.Length >0)
                    {
                        continue;
                    }
                    DataRow rs = dt_不良产品.NewRow();
                    rs["物料编码"] = dr["物料编码"].ToString();
                    rs["物料名称"] = dr["物料名称"].ToString();
                    rs["规格型号"] = dr["规格型号"].ToString();
                    dt_不良产品.Rows.Add(rs);

                }
                gridControl2.DataSource = dt_不良产品;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        //查找
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                load();
                //fun_图表();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //导出返工原因占比前五项的产品 
        string ss;
        string dd;
        //string ee;
        private void 导出()
        {
            try
            {
                if (Convert.ToInt32(textBox1.Text)==0 && textBox1.Text.ToString()=="")
                {
                    throw new Exception("请填写1以上数字！");
                }
                if(Convert.ToInt32(textBox1.Text) > dt_返工前N项.Rows.Count)
                {
                    throw new Exception("导出行数大于当前返工原因数！");
                }


                DataTable dt_取值 = new DataTable();
                dt_取值.Columns.Add("返工原因", typeof(string));
                dt_取值.Columns.Add("返工占比", typeof(string));
               // dt_取值.Columns.Add("物料名称", typeof(string));
                dt_取值.Columns.Add("规格型号", typeof(string));
                //dt_取值.Columns.Add("物料编码", typeof(string));
                DataRow[] rows = dt_返工前N项.Select("1=1");
                int a_行数 = Convert.ToInt32(textBox1.Text);
                for (int i = 0; i < a_行数; i++)
                {
                    dt_取值.ImportRow((DataRow)rows[i]);
                }
             //找出所有数据 不良原因和物料 做汇总
              string  str11 = string.Format(@"select b.返工原因,a.物料编码,a.规格型号 from 成品检验检验记录返工表 b left join 生产记录生产检验单主表 a on a.生产检验单号=b.生产检验单号 
      where a.不合格数量 !=0 and 返工原因 is not null and 送检日期>='{0}' and 送检日期<='{1}' group by  b.返工原因,a.物料编码,a.规格型号", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));
              DataTable dtf_数据 = new DataTable();
                using(SqlDataAdapter da = new SqlDataAdapter(str11,CPublic.Var.strConn))
                {
                    da.Fill(dtf_数据);
                }
                foreach(DataRow r in dt_取值.Rows)
                {
                  
                    ss = "";
                    dd = "";
                    //ee = "";
                    DataRow[] dr = dtf_数据.Select(string.Format("返工原因='{0}'", r["返工原因"].ToString()));
                    foreach (DataRow rr in dr)
                    {
                      
                        //if (ss == "")
                        //{
                        //    ss = rr["物料名称"].ToString();
                        //}
                        //else
                        //{
                        //    ss = ss + "," + rr["物料名称"].ToString();
                        //}
                        if (dd == "")
                        {
                            dd = rr["规格型号"].ToString();
                        }
                        else
                        {
                            dd = dd + " ," + rr["规格型号"].ToString();
                        }
                     
                       // r["物料名称"] = ss;
                        r["规格型号"] = dd;
                        
                    }
                }
                //导出图片
                string root = System.Windows.Forms.Application.StartupPath + "\\ApplyTemptupi\\";
                DirectoryInfo rt = new DirectoryInfo(root);
                if (!rt.Exists) rt.Create();
                //选择导出文件位置
                //if (fbd.ShowDialog() == DialogResult.OK)
                //{
                //导出路径
                //string outPath = fbd.SelectedPath.ToString();
                string outPath = root.ToString();
                //string fileName = outPath + "\\折线图" + DateTime.Now.ToString(OriConstants.FORMART_DATE_FILE) + ".png";
                string fileName = outPath + "\\饼图成品不良现象" + ".png";
                //输出图片到指定位置
                chartControl1.ExportToImage(fileName, ImageFormat.Png);
                //导出excel
                导出成品不良现象 fm = new 导出成品不良现象(dt_取值,fileName);
                fm.ShowDialog();
                //SaveFileDialog saveFileDialog = new SaveFileDialog();
                //saveFileDialog.Title = "导出Excel";
                //saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                //DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                //if (dialogResult == DialogResult.OK)
                //{
                //    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                //    options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;
                 
                //    ERPorg.Corg.TableToExcel(dt_取值, saveFileDialog.FileName);
                    

                //    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}

                 




            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        
        }
        /// <summary>
        /// Datable导出成Excel
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="file">导出路径(包括文件名与扩展名)</param>
        public void TableToExcel(DataTable dt, string file, string utPathh)
        {

            XSSFWorkbook xssfworkbook = new XSSFWorkbook();
          //  ISheet sheet = xssfworkbook.CreateSheet("Test");
            DataTable tblDatas = new DataTable("Datas");
            DataColumn dc = null;
            //赋值给dc，是便于对每一个datacolumn的操作
            dc = tblDatas.Columns.Add("编号", Type.GetType("System.Int32"));
            dc.AutoIncrement = true;//自动增加
            dc.AutoIncrementSeed = 1;//起始为1
            dc.AutoIncrementStep = 1;//步长为1
            dc.AllowDBNull = false;//

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
                    NPOI.SS.UserModel.ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                }
            }

           //图片
            dc = tblDatas.Columns.Add("问题图片一", Type.GetType("System.String"));
            //表头
            IRow row3 = sheet.CreateRow(20);
            //for (int i = 0; i < tblDatas.Columns.Count; i++)
            //{

                //ICell cell = row3.CreateCell(20);
                //cell.SetCellValue(tblDatas.Columns[0].ColumnName);
                //自动适应宽度
                sheet.AutoSizeColumn(20);
                //sheet.SetColumnWidth(i, sheet.GetColumnWidth(i));
            //}

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





        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }




        //折线图结构  12-26  暂时不用了 
       private void fun_图表()
        {
           
            //DataTable dtData = dt_返工前N项.Copy();
            DataTable dtData = new DataTable();
            dtData.Columns.Add("返工原因",typeof(string));
            dtData.Columns.Add("返工占比", typeof(decimal));

            string[] sArray = null;
            foreach(DataRow dr in dt_返工前N项.Rows)
            {
                DataRow rr = dtData.NewRow();
                dtData.Rows.Add(rr);
                rr["返工原因"] = dr["返工原因"].ToString();

                sArray = dr["返工占比"].ToString().Split('%');// 一定是单引 

                rr["返工占比"] = Convert.ToDecimal(sArray[0]) / 100;
             

            }

            //建立新的datatable，用来存储XY坐标
            DataTable dtXY = new DataTable();
            //横坐标的值
            dtXY.Columns.Add(new DataColumn("类型"));

            var list = new List<object>();
            //纵坐标的值
            list.Add("数值");

            //这里是控制x轴显示数据的数量，ArgumentScaleType类型设置为ScaleType.Qualitative时他不会自动控制x轴的数量
            //如果得到数据小于X_COUNT，则x轴显示全部得到的数据
            int X_COUNT = 5;

            if (X_COUNT < dtData.Rows.Count)
            {
                for (int i = 0; i < X_COUNT; i++)
                {
                    int index = (dtData.Rows.Count / X_COUNT) * i;
                    DataRow item = dtData.Rows[index];
                    dtXY.Columns.Add(new DataColumn(item["返工原因"].ToString(), typeof(decimal)));
                    list.Add(item["返工占比"]);
                }
            }
            //如果得到数据大于X_COUNT，则x轴X_COUNT条数据
            else
            {
                for (int i = 0; i < dtData.Rows.Count; i++)
                {
                    DataRow item = dtData.Rows[i];
                    dtXY.Columns.Add(new DataColumn(item["返工原因"].ToString(), typeof(string)));

                    list.Add(item["返工占比"]);

                }

            }
            var array = list.ToArray();
            dtXY.Rows.Add(array);





            //通过上面形成所需的折线图datatable 结构
             chartControl1.Series.Clear();
            Series s = new Series("返工原因每周折线图", ViewType.Line);

       
            s.ArgumentScaleType = ScaleType.Qualitative;
             (s.PointOptions).ValueNumericOptions.Format = NumericFormat.Percent;//让数字显示成百分比格式
            for (int i = 1; i < dtXY.Columns.Count; i++)
            {
                //string argument = dtM.Rows[0][i].ToString();//参数名称

                string argument = dtXY.Columns[i].Caption.ToString();//参数名称

                SeriesPoint point;
                if (dtXY.Columns[i].Caption.ToString() != "")
                {
                    decimal value = Convert.ToDecimal(dtXY.Rows[0][i]);//参数值

                    //string ss=value*100+"%";

                    point = new SeriesPoint(argument, value, 0);
                  
                    point.IsEmpty = false;
                    s.Points.Add(point);


                    point = new SeriesPoint(argument, value, 1);
                    point.IsEmpty = false;
                    s.Points.Add(point);

                }
                else
                {
                    point = new SeriesPoint(argument, 0, 0);
                    point.IsEmpty = true;
                    s.Points.Add(point);

                }

            }
            chartControl1.Series.Add(s);

            s.DataSource = dtXY;

         }
      
        //导出折线图
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
               


             // FolderBrowserDialog fbd = new FolderBrowserDialog();
              string root = System.Windows.Forms.Application.StartupPath + "\\ApplyTemptupi\\";
              DirectoryInfo rt = new DirectoryInfo(root);
              if (!rt.Exists) rt.Create();
                //选择导出文件位置
              //if (fbd.ShowDialog() == DialogResult.OK)
              //{
                  //导出路径
                  //string outPath = fbd.SelectedPath.ToString();
              string outPath = root.ToString();
                  //string fileName = outPath + "\\折线图" + DateTime.Now.ToString(OriConstants.FORMART_DATE_FILE) + ".png";
                  string fileName = outPath + "\\饼图成品不良现象" + ".png";
                  //输出图片到指定位置
                  chartControl1.ExportToImage(fileName, ImageFormat.Png);
                //  MessageBox.Show("导出饼图成功！");
              //}
            }
                catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
              }
         }

    







    }
}
