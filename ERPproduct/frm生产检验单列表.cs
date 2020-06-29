using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using CZMaster;
using System.IO;

namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm生产检验单列表 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        string strcon = "";

        public frm生产检验单列表()
        {
            strcon = CPublic.Var.strConn;
            InitializeComponent();
            txt_songjianriqi2.EditValue = Convert.ToDateTime(System.DateTime.Today.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
            txt_songjianri1.EditValue = Convert.ToDateTime(System.DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd"));

        }


        /// <summary>
        /// 生产检验单列表
        /// </summary>
        DataTable dt_JYDLB;
        DataTable dtM;


#pragma warning disable IDE1006 // 命名样式
        private void gc_checkdan_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //try
            //{
            //    fun_load();
            //    txt_jianyandanhao.EditValue = "";
            //    txt_songjianri1.EditValue = System.DateTime.Today.AddDays(-7);
            //    txt_songjianriqi2.EditValue = System.DateTime.Today.AddDays(1).AddSeconds(-1);
            //    txt_danjuzhuangtai.EditValue = "已生效";
            //    fun_筛选();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            DateTime dtime = Convert.ToDateTime(txt_songjianriqi2.EditValue).AddDays(1).AddSeconds(-1);
            string sql = string.Format(@"select 生产记录生产检验单主表.* ,图纸编号 from 生产记录生产检验单主表
                                         left join 基础数据物料信息表 on  基础数据物料信息表.物料编码=生产记录生产检验单主表.物料编码   
             where 生产记录生产检验单主表.送检日期>='{0}' and 生产记录生产检验单主表.送检日期<='{1}'", txt_songjianri1.EditValue.ToString(), dtime);

            dtM = MasterSQL.Get_DataTable(sql, strcon);
            dtM.Columns.Add("返工原因",typeof(string));
            DataColumn[] pk_bom = new DataColumn[1];
            pk_bom[0] = dtM.Columns["生产检验单号"];
            sql = "select * from 成品检验检验记录返工表";
            DataTable dt_返工原因=CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            DataColumn[] pk_bom1 = new DataColumn[1];
            pk_bom1[0] = dt_返工原因.Columns["ID"];
            foreach (DataRow dr in dtM.Rows)
            {
                DataRow[] dr_返工 = dt_返工原因.Select(string.Format("生产检验单号 = '{0}'", dr["生产检验单号"]));
                if (dr_返工.Length>0)
                {
                    foreach(DataRow dr1 in dr_返工)
                    {
                        dr["返工原因"] = dr["返工原因"].ToString() + dr1["返工原因"].ToString() + ";";
                    }
                } 
            }

        }


#pragma warning disable IDE1006 // 命名样式
        private void fun_筛选()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataView dv = new DataView(dtM);

                if (txt_songjianri1.EditValue.ToString() != "" && txt_songjianriqi2.EditValue.ToString() != "" && txt_songjianri1.EditValue != null && txt_songjianriqi2.EditValue != null)
                {
                    if (Convert.ToDateTime(txt_songjianri1.EditValue) > Convert.ToDateTime(txt_songjianriqi2.EditValue))
                        throw new Exception("起始日期不能够大于终止日期！请重新选择");
                    DateTime dtime = Convert.ToDateTime(txt_songjianriqi2.EditValue);
                    dtime.AddDays(1);
                    dv.RowFilter = string.Format("送检日期>'{0}' and 送检日期<'{1}'", txt_songjianri1.EditValue, dtime);

                }

                if (txt_danjuzhuangtai.EditValue.ToString() == "已生效")
                {
                    dv.RowFilter = "生效=1";
                }

                if (txt_danjuzhuangtai.EditValue.ToString() == "未生效")
                {
                    dv.RowFilter = "生效=0";
                }
                if (txt_danjuzhuangtai.EditValue.ToString() == "全部")
                {

                }
                gc_checkdan.DataSource = dv;

            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_SearchData");
                throw ex;
            }
        }

        #region 界面操作

        //查询操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                frm生产检验单列表_Load(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //新增操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                frm成品检验 frm = new frm成品检验();
                CPublic.UIcontrol.AddNewPage(frm, "成品检验");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //单号清空    按钮
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                txt_jianyandanhao.EditValue = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        private void 明细查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow r = (this.BindingContext[dt_JYDLB].Current as DataRowView).Row;

                if (r["生效"].Equals(true))
                {
                    //已经生效的检验单是转视图窗体
                }
                else
                {
                    frm成品检验 frm = new frm成品检验();
                    CPublic.UIcontrol.AddNewPage(frm, "成品检验");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //筛选下拉框 默认未生效
#pragma warning disable IDE1006 // 命名样式
        private void txt_danjuzhuangtai_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_checkdan_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (e.Clicks == 2 && e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    DataRow r = gv_checkdan.GetDataRow(gv_checkdan.FocusedRowHandle);

                    if (r["生效"].Equals(true))
                    {
                        frm成品检验_视图 fm = new frm成品检验_视图(r["生产检验单号"].ToString(), false);
                        CPublic.UIcontrol.AddNewPage(fm, "成品检验_视图");
                    }
                    else
                    {
                        frm成品检验 frm = new frm成品检验();
                        CPublic.UIcontrol.AddNewPage(frm, "成品检验");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm生产检验单列表_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(this.panel1, this.Name, cfgfilepath);

                fun_load();
                txt_jianyandanhao.EditValue = "";

                txt_danjuzhuangtai.EditValue = "已生效";
                fun_筛选();
                dtM_加载检验主表.ColumnChanged += dtM_加载检验主表_ColumnChanged;
                fun_加载不合格原因();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_checkdan_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv_checkdan.GetFocusedRowCellValue(gv_checkdan.FocusedColumn));
                e.Handled = true;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_checkdan_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }
        //刷新
        DataTable dtM_加载检验主表 = new DataTable();
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_刷新主表();
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_刷新主表()
#pragma warning restore IDE1006 // 命名样式
        {
            dtM_加载检验主表.Clear();
            string stre = string.Format("select * from  生产记录生产检验单主表 where 生效日期>='{0}'and 生效日期<='{1}'", txt_songjianri1.EditValue, txt_songjianriqi2.EditValue);
            using (SqlDataAdapter da = new SqlDataAdapter(stre, strcon))
            {

                da.Fill(dtM_加载检验主表);
                gridControl1.DataSource = dtM_加载检验主表;
            }
        }


#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {

            DataRow rr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (Convert.ToDecimal(rr["合格数量"]) > Convert.ToDecimal(rr["送检数量"]))
            {
                throw new Exception("合格数量不能大于送检数量！");
            }
            decimal aa = 0;
            if (dt_加载返工.Rows.Count > 0)
            {
                foreach (DataRow dr in dt_加载返工.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    aa += Convert.ToDecimal(dr["数量"]);

                }

                if (Convert.ToDecimal(rr["不合格数量"]) != aa)
                {
                    throw new Exception("返工数量不等于 不合格品数量，请修改返工数量！");
                }


            }


        }




#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {


        }

        DataTable dt_加载返工 = new DataTable();
#pragma warning disable IDE1006 // 命名样式
        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_加载返工记录表();
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_加载返工记录表()
#pragma warning restore IDE1006 // 命名样式
        {

            try
            {
                dt_加载返工.Clear();
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string strw = string.Format("select * from 成品检验检验记录返工表 where 生产检验单号='{0}'", dr["生产检验单号"]);
                using (SqlDataAdapter da = new SqlDataAdapter(strw, strcon))
                {

                    da.Fill(dt_加载返工);
                    gcP.DataSource = dt_加载返工;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }



#pragma warning disable IDE1006 // 命名样式
        private void dtM_加载检验主表_ColumnChanged(object sender, DataColumnChangeEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
           
          
            try
            {
                if (e.Row != null && Convert.ToDecimal(e.Row["已入库数量"]) > 0) throw new Exception("已有入库不可修改");
                dtM_加载检验主表.ColumnChanged -= dtM_加载检验主表_ColumnChanged;
                if (e.Column.Caption == "送检数量" || e.Column.Caption == "合格数量" || e.Column.Caption == "不合格数量" || e.Column.Caption == "返工数量" || e.Column.Caption == "重检合格数" || e.Column.Caption == "总计合格率")
                {
                    //if (e.Row["包装数量"] == DBNull.Value)
                    //    e.Row["包装数量"] = 0;
                    //if (e.Row["总装数量"] == DBNull.Value)
                    //    e.Row["总装数量"] = 0;
                    e.Row["不合格数量"] = Convert.ToDecimal(e.Row["送检数量"]) - Convert.ToDecimal(e.Row["合格数量"]);
                    e.Row["返工数量"] = Convert.ToDecimal(e.Row["送检数量"]) - Convert.ToDecimal(e.Row["合格数量"]);
                    e.Row["重检合格数"] = Convert.ToDecimal(e.Row["送检数量"]) - Convert.ToDecimal(e.Row["合格数量"]);
                    //e.Row["总计合格率"] ="100.00%";
                    e.Row["一次合格率"] = Math.Round(Convert.ToDecimal(e.Row["合格数量"]) / Convert.ToDecimal(e.Row["送检数量"]) * 100, 2, MidpointRounding.AwayFromZero) + "%";
                }
                dtM_加载检验主表.ColumnChanged += dtM_加载检验主表_ColumnChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //插入返工原因
#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void button2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }
        DataTable dt_返工原因表;
#pragma warning disable IDE1006 // 命名样式
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

#pragma warning disable IDE1006 // 命名样式
        private void gvP_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {



        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_checkdan_RowCellClick_1(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_RowCellClick_1(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_加载返工记录表();
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_checkdan_RowCellClick_2(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (e.Clicks == 2 && e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    DataRow r = gv_checkdan.GetDataRow(gv_checkdan.FocusedRowHandle);
                    if (r == null) return;
                    if (r["生效"].Equals(true))
                    {
                        frm成品检验_视图 fm = new frm成品检验_视图(r["生产检验单号"].ToString(), false);
                        CPublic.UIcontrol.AddNewPage(fm, "成品检验_视图");
                    }
                    else
                    {
                        frm成品检验 frm = new frm成品检验();
                        CPublic.UIcontrol.AddNewPage(frm, "成品检验");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gvP_CellValueChanging_1(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
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

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click_1(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_刷新主表();
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click_1(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                fun_check();
                gridView1.CloseEditor();//关闭编辑状态
                this.BindingContext[dtM_加载检验主表].EndCurrentEdit();//关闭编辑状态
                gvP.CloseEditor();
                this.BindingContext[dt_加载返工].EndCurrentEdit();
                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("生效");

                string sql1 = "select * from 生产记录生产检验单主表 where 1<>1";
                SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
                SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);

                string sql2 = "select * from 成品检验检验记录返工表 where 1<>1";
                SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);
                SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                new SqlCommandBuilder(da2);


                try
                {

                    da1.Update(dtM_加载检验主表);
                    da2.Update(dt_加载返工);


                    ts.Commit();
                    MessageBox.Show("保存成功");
                    // fun_刷新主表();
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    MessageBox.Show(ex.Message);

                }
            }
            catch (Exception x)
            {

                MessageBox.Show(x.Message);

            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void button1_Click_1(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
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
                fun_加载不合格原因();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void button2_Click_1(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
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

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawCell_1(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow cv = gridView1.GetDataRow(e.RowHandle);
            if (Convert.ToDecimal(cv["不合格数量"]) != 0)
            {
                e.Appearance.BackColor = Color.Red;
            }
        }
        //上传成品不合格评审单
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow dr = gv_checkdan.GetDataRow(gv_checkdan.FocusedRowHandle);
                if (dr["生产检验单号"].ToString() == "")
                {
                    throw new Exception("请选择生产检验单后再上传！");
                }

                成品检验不合格评审单上传 fm = new 成品检验不合格评审单上传(dr);
                fm.ShowDialog();
                frm生产检验单列表_Load(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_checkdan_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            DataRow r = gv_checkdan.GetDataRow(e.RowHandle);
            if (r != null)
            {
                if (r["是否上传品审单"].Equals(true))
                {
                    e.Appearance.BackColor = Color.Red;
                }

            }



        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    DevExpress.XtraGrid.GridControl gc = (ActiveControl) as DevExpress.XtraGrid.GridControl;
                    gc_checkdan.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gv_checkdan.GetDataRow(gv_checkdan.FocusedRowHandle);
                string sql_生产工单 = string.Format("select * from 生产记录生产工单表 where 生产工单号 = '{0}'", dr["生产工单号"]);
                DataTable dt_生产工单 = CZMaster.MasterSQL.Get_DataTable(sql_生产工单, strcon);
                string sql_生产制令 = string.Format("select * from 生产记录生产制令表 where 生产制令单号 = '{0}'", dt_生产工单.Rows[0]["生产制令单号"]);
                DataTable dt_生产制令 = CZMaster.MasterSQL.Get_DataTable(sql_生产制令, strcon);
                string sql_生产检验主 = string.Format("select * from 生产记录生产检验单主表 where 生产检验单号 = '{0}'", dr["生产检验单号"]);
                DataTable dt_生产检验主 = CZMaster.MasterSQL.Get_DataTable(sql_生产检验主, strcon);
                string sql_生产检验子 = string.Format("select * from 成品检验检验记录明细表 where 生产检验单号 = '{0}'", dr["生产检验单号"]);
                DataTable dt_生产检验子 = CZMaster.MasterSQL.Get_DataTable(sql_生产检验子, strcon);
                string sql_返工 = string.Format("select * from 成品检验检验记录返工表 where 生产检验单号 = '{0}'", dr["生产检验单号"]);
                DataTable dt_返工 = CZMaster.MasterSQL.Get_DataTable(sql_返工, strcon);
                string sql_序列号 = string.Format("select * from 生产检验单与产品序列号对应关系表 where 生产检验单号 = '{0}'", dr["生产检验单号"]);
                DataTable dt_序列号 = CZMaster.MasterSQL.Get_DataTable(sql_序列号, strcon);
                string sql_原因 = string.Format("select * from 序列号返工原因对应表 where 生产检验单号 = '{0}'", dr["生产检验单号"]);
                DataTable dt_原因 = CZMaster.MasterSQL.Get_DataTable(sql_原因, strcon);

                foreach (DataRow dr_jy in dt_生产检验主.Rows)
                {
                    if (Convert.ToDecimal(dr["已入库数量"]) > 0)
                    {
                        throw new Exception("该单据已有入库记录，不可撤回");
                    }
                }
                if (MessageBox.Show(string.Format("确认撤销该条检验记录？"), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DataRow[] dr_工单 = dt_生产工单.Select(string.Format("生产工单号 = '{0}'", dr["生产工单号"]));
                    dr_工单[0]["已检验数量"] = Convert.ToDecimal(dr_工单[0]["已检验数量"]) - Convert.ToDecimal(dr["送检数量"]);
                    dr_工单[0]["未检验数量"] = Convert.ToDecimal(dr_工单[0]["未检验数量"]) + Convert.ToDecimal(dr["送检数量"]);
                    dr_工单[0]["检验完成"] = false;
                    dr_工单[0]["检验完成日期"] = DBNull.Value;
                    dr_工单[0]["完成"] = false;
                    dr_工单[0]["完成日期"] = DBNull.Value;

                    DataRow[] dr_制令 = dt_生产制令.Select(string.Format("生产制令单号 = '{0}'", dr_工单[0]["生产制令单号"]));
                    dr_制令[0]["完成"] = false;
                    dr_制令[0]["完成日期"] = DBNull.Value;

                    dt_生产检验主.Rows[0].Delete();
                    for (int i = dt_生产检验子.Rows.Count - 1; i >= 0; i--)
                    {
                        dt_生产检验子.Rows[i].Delete();
                    }
                    for (int i = dt_返工.Rows.Count - 1; i >= 0; i--)
                    {
                        dt_返工.Rows[i].Delete();
                    }
                    for (int i = dt_序列号.Rows.Count - 1; i >= 0; i--)
                    {
                        dt_序列号.Rows[i].Delete();
                    }
                    for (int i = dt_原因.Rows.Count - 1; i >= 0; i--)
                    {
                        dt_原因.Rows[i].Delete();
                    }
                    SqlConnection conn = new SqlConnection(strcon);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("撤销");

                    try
                    {
                        string sql1 = "select * from 生产记录生产工单表 where 1<>1";
                        string sql2 = "select * from 生产记录生产制令表 where 1<>1";
                        string sql3 = "select * from 生产记录生产检验单主表 where 1<>1";
                        string sql4 = "select * from 成品检验检验记录明细表 where 1<>1";
                        string sql5 = "select * from 成品检验检验记录返工表 where 1<>1";
                        string sql6 = "select * from 生产检验单与产品序列号对应关系表 where 1<>1";
                        string sql7 = "select * from 序列号返工原因对应表 where 1<>1";

                        SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
                        SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);
                        SqlCommand cmd3 = new SqlCommand(sql3, conn, ts);
                        SqlCommand cmd4 = new SqlCommand(sql4, conn, ts);
                        SqlCommand cmd5 = new SqlCommand(sql5, conn, ts);
                        SqlCommand cmd6 = new SqlCommand(sql6, conn, ts);
                        SqlCommand cmd7 = new SqlCommand(sql7, conn, ts);

                        SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(dt_生产工单);

                        SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                        new SqlCommandBuilder(da2);
                        da2.Update(dt_生产制令);

                        SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
                        new SqlCommandBuilder(da3);
                        da3.Update(dt_生产检验主);

                        SqlDataAdapter da4 = new SqlDataAdapter(cmd4);
                        new SqlCommandBuilder(da4);
                        da4.Update(dt_生产检验子);

                        SqlDataAdapter da5 = new SqlDataAdapter(cmd5);
                        new SqlCommandBuilder(da5);
                        da5.Update(dt_返工);

                        SqlDataAdapter da6 = new SqlDataAdapter(cmd6);
                        new SqlCommandBuilder(da6);
                        da6.Update(dt_序列号);

                        SqlDataAdapter da7 = new SqlDataAdapter(cmd7);
                        new SqlCommandBuilder(da7);
                        da7.Update(dt_原因);


                        //fun_载入();
                        ts.Commit();
                        MessageBox.Show("撤回成功");

                        dtM.Rows.Remove(dr);
                    }
                    catch (Exception)
                    {
                        ts.Rollback();
                        throw;
                    }
                }



            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
