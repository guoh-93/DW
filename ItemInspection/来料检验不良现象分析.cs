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

namespace ItemInspection
{
    public partial class 来料检验不良现象分析 : UserControl
    {
        public 来料检验不良现象分析()
        {
            InitializeComponent();
        }
        string strconn = CPublic.Var.strConn;
        DataTable dt_不合格前5项;
        private void 来料检验不良现象分析_Load(object sender, EventArgs e)
        {
            DateTime t = CPublic.Var.getDatetime();
            barEditItem2.EditValue = t.AddDays(-7);
            barEditItem3.EditValue = t;
        }
        DataTable dt_所有数据;
        private void fun_加载不良品()
        {
            string str = string.Format(@"select * from (select a.不合格原因,sum(a.不合格数量)as 不合格数量1, ROW_NUMBER() over(order by (sum(a.不合格数量))desc) as rows from 采购记录采购单检验明细表 a left join 采购记录采购单检验主表 b on a.检验记录单号=b.检验记录单号
       where a.不合格数量 !=0 and 不合格原因 !='' and b.检验日期>='{0}' and b.检验日期<='{1}' group by a.不合格原因)c where c.rows <=5", barEditItem2.EditValue, Convert.ToDateTime(barEditItem3.EditValue).AddDays(1).AddSeconds(-1));
        
            
            using(SqlDataAdapter da = new SqlDataAdapter(str,strconn))
           {
                dt_不合格前5项 = new DataTable();
               da.Fill(dt_不合格前5项);
           }
             DataTable dtDa = dt_不合格前5项.Copy();

             string str1 = string.Format(@"select a.不合格原因,a.不合格数量,b.产品编号,b.产品名称,b.规格型号 from 采购记录采购单检验明细表 a left join 采购记录采购单检验主表 b on a.检验记录单号 = b.检验记录单号
          where a.不合格数量 !=0 and a.检验日期 >='{0}' and a.检验日期 <='{1}'" , barEditItem2.EditValue, Convert.ToDateTime(barEditItem3.EditValue).AddDays(1).AddSeconds(-1));
             dt_所有数据 = new DataTable();
             using (SqlDataAdapter da = new SqlDataAdapter(str1, strconn))
             {
                 da.Fill(dt_所有数据);
             }

             //返工占比
             decimal a_总的不合格数量 = 0;
             foreach (DataRow dr in dt_所有数据.Rows)
             {
                 a_总的不合格数量 += Convert.ToDecimal(dr["不合格数量"]);
             }
             dt_不合格前5项.Columns.Add("不良占比", typeof(string));
             foreach (DataRow r in dt_不合格前5项.Rows)
             {

                 Decimal dbdata = Convert.ToDecimal(r["不合格数量1"]) / a_总的不合格数量 * 100;
                 decimal a = Math.Round(dbdata, 2, MidpointRounding.AwayFromZero);//小数点保存两位

                 r["不良占比"] = a + "%";

             }
             gridControl1.DataSource = dt_不合格前5项;
             fun_饼状图(a_总的不合格数量, dtDa);

        }
         //查找
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_加载不良品();
            }
            catch(Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }
        //关闭
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        //生成饼状图
        private void fun_饼状图(decimal a_总的不合格数量, DataTable dtData)
        {
            string hv = "";
            string jv = "";
            string kv = "";
            string lv = "";
            string mv = "";
            int ax = 0;
            int bx = 0;
            int cx = 0;
            int dx = 0;
            int ex = 0;
            int f_其他 = 0;
            try
            {
              
                if (dtData.Rows.Count > 0)
                {
                    foreach (DataRow rs in dtData.Rows)
                    {
                        if (hv.Length == 0)
                        {
                            hv = rs["不合格原因"].ToString();
                            ax = Convert.ToInt32(rs["不合格数量1"]);
                            continue;
                        }
                        if (jv.Length == 0)
                        {
                            jv = rs["不合格原因"].ToString();
                            bx = Convert.ToInt32(rs["不合格数量1"]);
                            continue;
                        }
                        if (kv.Length == 0)
                        {
                            kv = rs["不合格原因"].ToString();
                            cx = Convert.ToInt32(rs["不合格数量1"]);
                            continue;
                        }
                        if (lv.Length == 0)
                        {
                            lv = rs["不合格原因"].ToString();
                            dx = Convert.ToInt32(rs["不合格数量1"]);
                            continue;
                        }
                        if (mv.Length == 0)
                        {
                            mv = rs["不合格原因"].ToString();
                            ex = Convert.ToInt32(rs["不合格数量1"]);
                            continue;
                        }

                    }
                    f_其他 = Convert.ToInt32(a_总的不合格数量) - (ax + bx + cx + dx + ex);
                }


                chartControl1.Series.Clear();
                Series s = new Series("不良现象分析图", ViewType.Pie);

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
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow rr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                DataRow[] ds = dt_所有数据.Select(string.Format("不合格原因='{0}'", rr["不合格原因"].ToString()));
                DataTable dt_不良产品 = new DataTable();
                dt_不良产品.Columns.Add("物料编码", typeof(string));
                dt_不良产品.Columns.Add("物料名称", typeof(string));
                dt_不良产品.Columns.Add("规格型号", typeof(string));
                foreach (DataRow dr in ds)
                {
                    DataRow[] rp = dt_不良产品.Select(string.Format("物料编码='{0}'", dr["产品编号"].ToString()));
                    if (rp.Length > 0)
                    {
                        continue;
                    }
                    DataRow rs = dt_不良产品.NewRow();
                    rs["物料编码"] = dr["产品编号"].ToString();
                    rs["物料名称"] = dr["产品名称"].ToString();
                    rs["规格型号"] = dr["规格型号"].ToString();
                    dt_不良产品.Rows.Add(rs);

                }
                gridControl2.DataSource = dt_不良产品;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        //导出excel
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            导出();
        }
        //导出返工原因占比前五项的产品 
        string ss;
        string dd;
        private void 导出()
        {
            try
            {
                if (Convert.ToInt32(textBox1.Text) == 0 && textBox1.Text.ToString() == "")
                {
                    throw new Exception("请填写1以上数字！");
                }
                if (Convert.ToInt32(textBox1.Text) > dt_不合格前5项.Rows.Count)
                {
                    throw new Exception("导出行数大于当前不良原因行数！");
                }


                DataTable dt_取值 = new DataTable();
                dt_取值.Columns.Add("不合格原因", typeof(string));
                dt_取值.Columns.Add("不良占比", typeof(string));
                dt_取值.Columns.Add("物料名称", typeof(string));
                dt_取值.Columns.Add("规格型号", typeof(string));
                //dt_取值.Columns.Add("物料编码", typeof(string));
                DataRow[] rows = dt_不合格前5项.Select("1=1");
                int a_行数 = Convert.ToInt32(textBox1.Text);
                for (int i = 0; i < a_行数; i++)
                {
                    dt_取值.ImportRow((DataRow)rows[i]);
                }

                foreach (DataRow r in dt_取值.Rows)
                {

                    ss = "";
                    dd = "";
                    //ee = "";
                    DataRow[] dr = dt_所有数据.Select(string.Format("不合格原因='{0}'", r["不合格原因"].ToString()));
                    foreach (DataRow rr in dr)
                    {
                        //DataRow[] rn = dt_取值.Select(string.Format("物料编码 like '{0}'",rr["物料编码"].ToString()));
                        //if(rn.Length > 0)
                        //{
                        //    continue;
                        //}
                        if (ss == "")
                        {
                            ss = rr["产品名称"].ToString();
                        }
                        else
                        {
                            ss = ss + "," + rr["产品名称"].ToString();
                        }
                        if (dd == "")
                        {
                            dd = rr["规格型号"].ToString();
                        }
                        else
                        {
                            dd = dd + "," + rr["规格型号"].ToString();
                        }
                        //if (ee == "")
                        //{
                        //    ee = rr["物料编码"].ToString();
                        //}
                        //else
                        //{
                        //    ee = ee+ "," + rr["物料编码"].ToString();
                        //}
                        r["物料名称"] = ss;
                        r["规格型号"] = dd;
                        //r["物料编码"] = ee;
                    }
                }
                //导出excel
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;
                    //gc.ExportToXlsx(saveFileDialog.FileName, options);
                    ERPorg.Corg.TableToExcel(dt_取值, saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }






            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //导出饼图
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {

              FolderBrowserDialog fbd = new FolderBrowserDialog();
                //选择导出文件位置
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    //导出路径
                    string outPath = fbd.SelectedPath.ToString();
                    // string fileName = outPath + "\\折线图" + DateTime.Now.ToString(OriConstants.FORMART_DATE_FILE) + ".png";
                    string fileName = outPath + "\\饼图" + ".png";
                    //输出图片到指定位置
                    chartControl1.ExportToImage(fileName, ImageFormat.Png);
                    MessageBox.Show("导出饼图成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



    }
}
