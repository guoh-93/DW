using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm快速检验记录查询 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        string strcon = "";
        string cfgfilepath = "";

        DataTable dtM_加载检验主表 = new DataTable();
        public frm快速检验记录查询()
        {
            strcon = CPublic.Var.strConn;
            InitializeComponent();
            dateEdit2.EditValue = Convert.ToDateTime(System.DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"));
            dateEdit1.EditValue = Convert.ToDateTime(System.DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd"));
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_刷新主表();
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm快速检验记录查询_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name)))
                {
                    gridView1.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }
                fun_加载不合格原因();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_刷新主表()
#pragma warning restore IDE1006 // 命名样式
        {
            DateTime t = Convert.ToDateTime(dateEdit2.EditValue).Date.AddDays(1).AddSeconds(-1);
            dtM_加载检验主表.Clear();
            string stre = string.Format(@"select a.*,b.备注5 as 东屋工单号 from  快速检验生产检验单主表 a
            left join 快速检验生产记录生产工单表 b on a.生产工单号 = b.生产工单号 where a.生效日期>='{0}'and a.生效日期<='{1}'", dateEdit1.EditValue, t);
            using (SqlDataAdapter da = new SqlDataAdapter(stre, strcon))
            {
                da.Fill(dtM_加载检验主表);
                gridControl1.DataSource = dtM_加载检验主表;
            }
        }

        // private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        // {

        //     fun_加载返工记录表();
        //}
#pragma warning disable IDE1006 // 命名样式
        DataTable dt_加载返工 = new DataTable();
        private void fun_加载返工记录表(string cc)
#pragma warning restore IDE1006 // 命名样式
        {

            try
            {

                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string strw = string.Format(@"select fg.* from 快速检验检验记录返工表 fg
                                        left join 快速检验生产检验单主表 a on a.生产检验单号=fg.生产检验单号
                                           where 1=1 {0} ", cc);
                using (SqlDataAdapter da = new SqlDataAdapter(strw, strcon))
                {
                    dt_加载返工 = new DataTable();
                    da.Fill(dt_加载返工);
                    gcP.DataSource = dt_加载返工;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        //private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        //{
        //    DataRow cv = gridView1.GetDataRow(e.RowHandle);
        //    if (Convert.ToDecimal(cv["不合格数量"]) != 0)
        //    {
        //        e.Appearance.BackColor = Color.Red;
        //    }
        //}

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_RowCellClick_1(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string s = "";
            DateTime t1 = Convert.ToDateTime(dr["生效日期"]);
            DateTime t2 = CPublic.Var.getDatetime();
             System.TimeSpan t3 =(t2.Date - t1.Date);
            if (t1.Year==t2.Year && t1.Month==t2.Month && t3.Days<=2)
            {
                panel3.Visible = true;
            }
            else
            {
                panel3.Visible = false;

            }
            s = string.Format(" and a.生产检验单号='{0}' ", dr["生产检验单号"].ToString());
            fun_加载返工记录表(s);

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
        //查询
#pragma warning disable IDE1006 // 命名样式
        private void button2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            ERPorg.Corg.FlushMemory();
            fun_刷新主表();
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow cv = gridView1.GetDataRow(e.RowHandle);
            if (Convert.ToDecimal(cv["不合格数量"]) != 0)
            {
                e.Appearance.BackColor = Color.Pink;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_ColumnPositionChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gridView1.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_ColumnWidthChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gridView1.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawGroupPanel(object sender, DevExpress.XtraGrid.Views.Base.CustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gridView1.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                int x = e.RowHandle;
                if (x < 0)
                {
                    DateTime t = Convert.ToDateTime(dateEdit2.EditValue).Date.AddDays(1).AddSeconds(-1);

                    object c = gridView1.GetGroupRowValue(x, (sender as DevExpress.XtraGrid.Views.Grid.GridView).GroupedColumns[0]);
                    string s = c.ToString();




                    if (c.GetType() == typeof(DateTime))
                    {
                        DateTime time = Convert.ToDateTime(s).Date;
                        c = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GroupedColumns[0];
                        string s1 = c.ToString();
                        s = string.Format(" and {0}>='{1}'and {0}<='{2}'", s1, time, time.AddDays(1).AddSeconds(-1));

                    }
                    else
                    {
                        c = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GroupedColumns[0];
                        string s1 = c.ToString();
                        s = string.Format(" and a.生效日期>='{0}'and a.生效日期<='{1}'  and {2}='{3}' ", dateEdit1.EditValue, t, s1, s);

                    }
                    fun_加载返工记录表(s);

                }
                else
                {

                    DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                    string s = "";
                    s = string.Format(" and a.生产检验单号='{0}' ", dr["生产检验单号"].ToString());
                    fun_加载返工记录表(s);
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                // DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions(TextExportMode.Text, false, false);
                /// gridControl1.ExportToXlsx(saveFileDialog.FileName, options);
                //    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ERPorg.Corg.TableToExcel(dtM_加载检验主表, saveFileDialog.FileName);
                MessageBox.Show("导出成功");
            }
        }

        //2020-3-25
        DataTable dt_返工原因表;
        private void fun_加载不合格原因()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = string.Format("select * from ZZ_FGYY ");//where dl = '{0}' //拉所有的原因
                dt_返工原因表 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt_返工原因表);
                repositoryItemSearchLookUpEdit1.DataSource = dt_返工原因表;
                repositoryItemSearchLookUpEdit1.DisplayMember = "fgyy";
                repositoryItemSearchLookUpEdit1.ValueMember = "fgyy";
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        //插入返工原因
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (dr == null)
                {
                    throw new Exception("请选择生产检验单修改表中的工单后操作");
                }
                DataRow rr = dt_加载返工.NewRow();

                rr["生产检验单号"] = dr["生产检验单号"].ToString();

                dt_加载返工.Rows.Add(rr);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);
                if (dr == null)
                {
                    throw new Exception("请选择生产检验单修改表中的工单后操作");
                }
                dr.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gvP_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "返工原因")
                {
                    DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);
                    if (dr.RowState == DataRowState.Added)
                    {
                        gridColumn24.OptionsColumn.AllowEdit = true;
                    }
                    else
                    {
                        gridColumn24.OptionsColumn.AllowEdit = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //保存
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActiveControl = null;
                DataRow rr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                decimal dec_fg = 0;
                foreach (DataRow r in dt_加载返工.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (r["返工原因"].ToString().Trim() == "") throw new Exception("返工原因为空");

                    if (r["数量"].ToString().Trim() == "") throw new Exception("返工数量为空");
                    decimal dec = 0;
                    if (!Decimal.TryParse(r["数量"].ToString(), out dec)) throw new Exception("返工数量输入有误");
                    if (dec <= 0) throw new Exception("返工数量不可小于0");
                    dec_fg += dec;
                }
                if (Convert.ToDecimal(rr["返工数量"]) != dec_fg) throw new Exception("插入返工原因数量与一开始录入数量不一致,请确认");

                rr["返工数量"] = dec_fg;
                rr["不合格数量"] = dec_fg;
                rr["合格数量"] = Convert.ToDecimal(rr["送检数量"]) - dec_fg;
                if (Convert.ToDecimal(rr["送检数量"]) < dec_fg) throw new Exception("返工数量已经大于检验数量");
                rr["一次合格率"] = Math.Round(Convert.ToDecimal(rr["合格数量"]) / Convert.ToDecimal(rr["送检数量"]) * 100, 2) + "%";


                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("生效");
                try
                {
                    string sql1 = "select * from 快速检验生产检验单主表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql1, conn, ts);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dtM_加载检验主表);
                    sql1 = "select * from 快速检验检验记录返工表 where 1<>1";
                    cmd = new SqlCommand(sql1, conn, ts);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_加载返工);

                    ts.Commit();
                    MessageBox.Show("保存成功");

                    fun_刷新主表();
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    MessageBox.Show(ex.Message);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gvP_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //选中物料编码下拉框 显示出来
            try
            {
                if (e.Column.FieldName == "返工原因")
                {

                    DataRow dr_当前行 = gvP.GetDataRow(gvP.FocusedRowHandle);
                    String str = e.Value.ToString();

                    // DataRow[] dr3 = dt_返工原因表.Select(string.Format("fgyy='{0}'", str));
                    DataRow[] dr3 = dt_返工原因表.Select("fgyy = '" + str + "'");
                    if (dr3 != null && dr3.Length > 0)
                    {
                        //DataRow row = dr3[0];
                        dr_当前行["返工编号"] = dr3[0]["fgbh"].ToString();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (dr != null)
                {
                    DateTime t1 = Convert.ToDateTime(dr["生效日期"]);
                    DateTime t2 = CPublic.Var.getDatetime();
                    System.TimeSpan t3 = (t2.Date - t1.Date);
                    if (t1.Year == t2.Year && t1.Month == t2.Month && t3.Days <= 2)
                    {
                        panel3.Visible = true;
                    }
                    else
                    {
                        panel3.Visible = false;

                    }
                    string s = "";
                    s = string.Format(" and a.生产检验单号='{0}' ", dr["生产检验单号"].ToString());
                    fun_加载返工记录表(s);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        
        }

        //        private void fun_加载导出()
        //        {
        //            DataTable dt_导出 = new DataTable();
        //            DateTime t = Convert.ToDateTime(dateEdit2.EditValue).Date.AddDays(1).AddSeconds(-1);
        //            string stre = string.Format(@"select 物料编码,物料名称,规格型号,sum(生产数量),sum(送检数量),sum(返工数量) from  快速检验生产检验单主表 a
        //            where a.生效日期>='{0}'and a.生效日期<='{1}' group by 物料编码,物料名称,规格型号", dateEdit1.EditValue, t);   
        //            using(SqlDataAdapter da = new SqlDataAdapter(stre,strcon))
        //            {
        //                da.Fill(dt_导出);
        //            }
        //            DataTable dt_返工1= new DataTable();
        //             stre11

        //            string stre1 = string.Format(@"select 物料编码,a.生产检验单号,b.返工原因 from  快速检验生产检验单主表 a left join 快速检验检验记录返工表 b         
        //             on a.生产检验单号=b.生产检验单号 where a.生效日期>='{0}'and a.生效日期<='{1}'", dateEdit1.EditValue, t);
        //            using (SqlDataAdapter da = new SqlDataAdapter(stre1,strcon))
        //            {
        //                da.Fill(dt_返工1);
        //            }

        //            foreach(DataRow r in dt_导出.Rows)
        //            {
        //                DataRow[] dr = dtM_加载检验主表.Select(string.Format("物料编码='{0}'",r["物料编码"]));
        //                foreach(DataRow rr in dr)
        //                {



        //                }



        //             }



        //        }





    }
}
