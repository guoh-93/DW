using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DevExpress.XtraTreeList.Nodes;

namespace ERPpurchase
{
    public partial class frm供应商维护 : UserControl
    {
        public frm供应商维护()
        {
            InitializeComponent();
            strconn = CPublic.Var.strConn;
        }

        #region 变量
        /// <summary>
        /// 供应商DT表
        /// </summary>
        DataTable dt_Provider;
        /// <summary>
        /// 操作行drM
        /// </summary>
        DataRow drM;
        int count = 0;
        DataTable dt_省;
        DataTable dt_市;
        string strconn = "";
        DataTable dt_下拉物料;
        DataTable dtP;
        #endregion

        private void frm供应商维护_Load(object sender, EventArgs e)
        {
            try
            {
                barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                string s = "select  *  from 供应商分类表 order by   供应商分类编码 ";
                DataTable tt = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                treeList1.OptionsBehavior.PopulateServiceColumns = true;
                treeList1.KeyFieldName = "GUID";
                treeList1.ParentFieldName = "上级类型GUID";
                treeList1.DataSource = tt;
                treeList1.CollapseAll();

                cb_供应商状态.EditValue = "在用";
                fun_下拉框();

                SqlDataAdapter da;
                da = new SqlDataAdapter("select * from 采购供应商表 order by POS", strconn);
                dt_Provider = new DataTable();
                da.Fill(dt_Provider);
                DataView dv = new DataView(dt_Provider);
                dv.RowFilter = "供应商状态 = '在用'";
                gc_provider.DataSource = dv;
                //new一个新的行
                drM = dt_Provider.NewRow();

                //DataView dv_provider = new DataView(dt_Provider);
                //dv_provider.Sort = "POS desc";
                //DataRow r = dv_provider[0].Row;
                //count = Convert.ToInt32(r["POS"]);
                //count++;
                //txt_providerID.Text = count.ToString("10000000");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_下拉框()
        {
            txt_providerGrade.Properties.Items.Clear();
            cb_税率.Properties.Items.Clear();
            cb_供应商状态.Properties.Items.Clear();

            string sql = "select * from 基础数据基础属性表 order by POS";
            DataTable dt_属性 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_属性);
            foreach (DataRow r in dt_属性.Rows)
            {
                if (r["属性类别"].ToString().Equals("供应商等级"))
                {
                    txt_providerGrade.Properties.Items.Add(r["属性值"].ToString());
                }
                if (r["属性类别"].ToString().Equals("税率"))
                {
                    cb_税率.Properties.Items.Add(r["属性值"].ToString());
                }
                if (r["属性类别"].ToString().Equals("供应商状态"))
                {
                    cb_供应商状态.Properties.Items.Add(r["属性值"].ToString());
                }
            }

            string sql_省 = "select * from S_Province";
            dt_省 = new DataTable();
            SqlDataAdapter da_省 = new SqlDataAdapter(sql_省, strconn);
            da_省.Fill(dt_省);
            foreach (DataRow r in dt_省.Rows)
            {
                cb_省.Properties.Items.Add(r["ProvinceName"].ToString());
            }
            //            string sql_1 = string.Format(@"select  base.物料编码,base.规格型号,base.物料名称,标准单价,库存总数,特殊备注 from 基础数据物料信息表 base,仓库物料数量表 
            //                                        where (物料类型<>'成品' or 可购=1) and base.物料编码=仓库物料数量表.物料编码");
            string sql_1 = string.Format(@"select  base.物料编码,base.规格型号,base.物料名称,标准单价,特殊备注 from 基础数据物料信息表 base 
                                        where   可购=1 or 委外=1  ");
            dt_下拉物料 = new DataTable();
            dt_下拉物料 = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
            repositoryItemSearchLookUpEdit1.DataSource = dt_下拉物料;
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";

        }

        #region  Check数据处理

        //检查数据的合法性
        private void fun_checkData()
        {
            try
            {
                if (drM["供应商GUID"] == DBNull.Value) //新增
                {
                    drM["供应商GUID"] = System.Guid.NewGuid().ToString();
                    drM["POS"] = count;
                    //DataTable t = CZMaster.MasterSQL.Get_DataTable(string.Format("select 供应商ID,供应商名称 from 采购供应商表 where 供应商ID='{0}'", drM["供应商ID"]), strconn);

                    //DataRow[] dr = t.Select(string.Format("供应商ID='{0}'", txt_providerID.Text));
                    //if (dr.Length > 0)
                    //    throw new Exception("供应商编码有重复，请重新填写！");  
                    string x = textBox7.Text;
                    string s = string.Format(@"select  max(供应商ID)供应商ID from 采购供应商表 where 供应商分类编码='{0}'and len(供应商ID)=6 ", x);
                    //x = x.PadRight(10, '0');
                    DataTable temp = new DataTable();
                    temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                    if (temp.Rows.Count == 0 || temp.Rows[0]["供应商ID"].ToString() == "") x = x + "0001";
                    else
                    {
                        s = temp.Rows[0]["供应商ID"].ToString();
                        s = (Convert.ToInt32(s.Substring(2, s.Length - 2)) + 1).ToString().PadLeft(4, '0');
                        x = x + s;
                        drM["供应商ID"] = x;
                    }
                    //  DataRow[] dr = dt_Provider.Select(string.Format("供应商ID = '{0}'", x));
                    ////string z = string.Format(@"select * from 采购供应商表 where 供应商分类编码='{0}'",x);
                    //if (dr.Length == 1)
                    //{

                    //    if (temp.Rows[0]["供应商ID"].ToString().Substring(0, 2) == textBox7.Text)
                    //    {
                    //        string maxs = temp.Rows[0]["供应商ID"].ToString().Substring(2, temp.Rows[0]["供应商ID"].ToString().Length-2);

                    //        maxs = (Convert.ToInt32(maxs) + 1).ToString().PadLeft(4, '0');

                    //        x = x + maxs;
                    //        drM["供应商ID"] = x;

                    //    }
                    //    else
                    //    {
                    //        x = textBox7.Text;
                    //        s = string.Format(@"select  max(right(供应商ID,4))供应商ID from 采购供应商表 where 供应商分类编码='{0}'", x);
                    //        //x = x.PadRight(10, '0');

                    //        temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                    //        if (temp.Rows.Count == 0 || temp.Rows[0]["供应商ID"].ToString() == "") x = x + "0001";
                    //        else
                    //        {
                    //            s = temp.Rows[0]["供应商ID"].ToString();
                    //            s = (Convert.ToInt32(s) + 1).ToString().PadLeft(4, '0');
                    //            x = x + s;
                    //            drM["供应商ID"] = x;
                    //        }
                    //    }
                    //}

                    drM["供应商分类编码"] = textBox7.Text;
                    txt_providerID.Text = x;


                    dt_Provider.Rows.Add(drM);
                }

                if (cb_供应商状态.Text.Trim() == "在用")
                {
                    //供应商名称
                    if (txt_providerName.Text == "")
                        throw new Exception("供应商名称不能为空，请填写！");
                    //供应商负责人
                    if (txt_providerMan.Text == "")
                        throw new Exception("供应商负责人不能为空，请填写！");
                    //供应商电话
                    if (txt_dianhua.Text == "")
                        throw new Exception("供应商电话不能为空，请填写！");
                    try
                    {
                        long i = Convert.ToInt64(txt_dianhua.Text);
                    }
                    catch
                    {
                        throw new Exception("电话应该为数字，请重新填写！");
                    }
                    //供应商地址
                    if (txt_providerdizhi.Text == "")
                        throw new Exception("供应商地址不能为空，请填写！");
                }
                dataBindHelper1.DataToDR(drM);
                drM["修改时间"] = CPublic.Var.getDatetime();

                if (txt_供应商邮箱.Text.Trim() != "")  //不填不检查
                {
                    bool bl = ERPorg.Corg.EmailIsMatch(txt_供应商邮箱.Text.Trim());
                    if (!bl)
                    {
                        throw new Exception("供应商邮箱填写错误");
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_checkData");
                throw new Exception(ex.Message);
            }
        }


        #endregion

        #region  调用的方法

        //新增数据
        private void fun_AddNewRow()
        {
            try
            {

                drM = dt_Provider.NewRow();
                //dt_Provider.Rows.Add(drM);
                drM["修改时间"] = CPublic.Var.getDatetime();
                dataBindHelper1.DataFormDR(drM);

                if (treeList1.Nodes.Count > 0)
                {
                    if (treeList1.Selection[0] == null) return;
                }
                else
                {
                    return;
                }


                TreeListNode n = treeList1.Selection[0];
                if (n.HasChildren) throw new Exception("此分类还有子级分类,不可在此分类下新增供应商");


                textBox6.Text = n.GetValue("供应商分类名称").ToString();
                textBox7.Text = n.GetValue("供应商分类编码").ToString();

                xtraTabControl1.SelectedTabPage = xtraTabPage1;


                DataView dv_provider = new DataView(dt_Provider);
                dv_provider.Sort = "POS desc";
                DataRow r = dv_provider[0].Row;
                count = Convert.ToInt32(r["POS"]);
                count++;
                //txt_providerID.Text = count.ToString("10000000");
                cb_供应商状态.EditValue = "在用";

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_AddNewRow");
                throw new Exception(ex.Message);
            }
        }

        //保存数据的方法
        private void fun_saveProviderDt()
        {
            SqlDataAdapter da;
            da = new SqlDataAdapter("select * from 采购供应商表 where 1<>1", strconn);
            new SqlCommandBuilder(da);
            da.Update(dt_Provider);
        }

        private void fun_loadagin()
        {
            SqlDataAdapter da;
            string sql = string.Format("select * from 采购供应商表 order by POS");
            da = new SqlDataAdapter(sql, strconn);
            dt_Provider = new DataTable();
            da.Fill(dt_Provider);
            DataView dv = new DataView(dt_Provider);
            dv.RowFilter = "供应商状态 = '在用'";
            gc_provider.DataSource = dv;
            if (dt_Provider.Rows.Count > 0)
            {
                DataRow[] dr = dt_Provider.Select(string.Format("供应商ID='{0}'", drM["供应商ID"].ToString()));
                if (dr.Length > 0)
                {
                    drM = dr[0];
                    dataBindHelper1.DataFormDR(drM);
                }
            }
        }

        private void fun_供应商物料单价(string str_供应ID, string str_供应商名)
        {
            //            string sql = string.Format(@"select  采购供应商物料单价表.*,供应商名称,原ERP物料编号,库存总数,基础数据物料信息表.特殊备注,采购供应商备注,n原ERP规格型号,基础数据物料信息表.图纸编号,基础数据物料信息表.物料名称  from  采购供应商物料单价表,基础数据物料信息表,采购供应商表,仓库物料数量表
            //                             where 采购供应商表.供应商ID=采购供应商物料单价表.供应商ID and 采购供应商物料单价表.物料编码=基础数据物料信息表.物料编码 and   基础数据物料信息表.物料编码= 仓库物料数量表.物料编码
            //                             and 采购供应商物料单价表.供应商ID='{0}'", str_供应ID);
            string sql = string.Format(@"select  cdj.*,供应商名称,base.特殊备注,采购供应商备注,规格型号,base.图纸编号,base.物料名称  from  采购供应商物料单价表 cdj,基础数据物料信息表 base ,采购供应商表 
                             where 采购供应商表.供应商ID=cdj.供应商ID and cdj.物料编码=base.物料编码  
                             and cdj.供应商ID='{0}'", str_供应ID);
            dtP = new DataTable();
            dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gridControl1.DataSource = dtP;

            xtraTabControl1.SelectedTabPage = xtraTabPage2;
            gridView1.ViewCaption = str_供应商名 + "对应物料单价";

        }
        #endregion

        #region  界面操作

        //新增
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_AddNewRow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //删除
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (dt_Provider.Rows.Count <= 0)
                    throw new Exception("没有数据可以删除！");
                DataRow r = (this.BindingContext[dt_Provider].Current as DataRowView).Row;
                if (MessageBox.Show(string.Format("你确定要删除编号为\"{0}\"的供应商吗？", r["供应商ID"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    r.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //保存数据
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = null;
                fun_checkData();

                fun_saveProviderDt();
                fun_loadagin();
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //查询功能，根据编号查询
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SqlDataAdapter da;
            da = new SqlDataAdapter("select * from 采购供应商表 order by POS", strconn);
            dt_Provider = new DataTable();
            da.Fill(dt_Provider);
            DataView dv = new DataView(dt_Provider);
            dv.RowFilter = "供应商状态 = '在用'";
            gc_provider.DataSource = dv;
            cb_供应商状态.EditValue = "在用";
        }

        //快速选择
        private void txt_chose_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //if (txt_chose.Checked == true)
                //{
                //    this.BindingContext[dt_Provider].PositionChanged += frm供应商维护_PositionChanged;
                //}
                //else
                //{
                //    this.BindingContext[dt_Provider].PositionChanged -= new EventHandler(frm供应商维护_PositionChanged);  //禁用事件
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //void frm供应商维护_PositionChanged(object sender, EventArgs e)
        //{
        //    DataRow r = (this.BindingContext[dt_Provider].Current as DataRowView).Row;
        //    drM = r;
        //    dataBindHelper1.DataFormDR(drM);
        //    txt_providerID.Enabled = false;
        //}

        //关闭窗体
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }


        #endregion

        //复制
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dt_Provider.Rows.Count > 0)
                {
                    DataRow r = (this.BindingContext[dt_Provider].Current as DataRowView).Row;
                    dataBindHelper1.DataFormDR(r);
                    DataView dv_provider = new DataView(dt_Provider);
                    dv_provider.Sort = "POS desc";
                    DataRow r1 = dv_provider[0].Row;
                    count = Convert.ToInt32(r1["POS"]);
                    count++;
                    txt_providerID.Text = count.ToString("10000000");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_provider_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (txt_chose.Checked == true)
                {
                    DataRow r = gv_provider.GetDataRow(gv_provider.FocusedRowHandle);
                    if (r == null) return;
                    drM = r;
                    dataBindHelper1.DataFormDR(drM);
                    //txt_providerID.Enabled = false;
                }

                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc_provider, new Point(e.X, e.Y));
                    gv_provider.CloseEditor();
                    this.BindingContext[dt_Provider].EndCurrentEdit();

                }


            }
            catch { }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                gc_provider.DataSource = dt_Provider;
            }
            else
            {
                SqlDataAdapter da;
                da = new SqlDataAdapter("select * from 采购供应商表 order by POS", strconn);
                dt_Provider = new DataTable();
                da.Fill(dt_Provider);
                DataView dv = new DataView(dt_Provider);
                dv.RowFilter = "供应商状态 = '在用'";
                gc_provider.DataSource = dv;
            }
        }

        private void cb_省_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cb_市.Properties.Items.Clear();
                cb_市.Text = "";
                DataRow[] ds = dt_省.Select(string.Format("ProvinceName = '{0}'", cb_省.Text.ToString()));
                string sql_市 = string.Format("select * from S_City where ProvinceID = '{0}'", ds[0]["ProvinceID"].ToString());
                dt_市 = new DataTable();
                SqlDataAdapter da_市 = new SqlDataAdapter(sql_市, strconn);
                da_市.Fill(dt_市);
                foreach (DataRow r in dt_市.Rows)
                {
                    cb_市.Properties.Items.Add(r["CityName"].ToString());
                }
            }
            catch
            //(Exception ee)
            {
                //MessageBox.Show(ee.Message);
            }
        }

        private void cb_市_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cb_县.Properties.Items.Clear();
                cb_县.Text = "";
                if (cb_市.Text != "")
                {
                    DataRow[] ds = dt_市.Select(string.Format("CityName = '{0}'", cb_市.Text.ToString()));
                    string sql_区县 = string.Format("select * from S_District where CityID = '{0}'", ds[0]["CityID"].ToString());
                    DataTable dt_区县 = new DataTable();
                    SqlDataAdapter da_区县 = new SqlDataAdapter(sql_区县, strconn);
                    da_区县.Fill(dt_区县);
                    foreach (DataRow r in dt_区县.Rows)
                    {
                        cb_县.Properties.Items.Add(r["DistrictName"].ToString());
                    }
                }
            }
            catch
            //(Exception ee)
            {
                //MessageBox.Show(ee.Message);
            }
        }

        private void 维护物料单价ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv_provider.GetDataRow(gv_provider.FocusedRowHandle);
            fun_供应商物料单价(dr["供应商ID"].ToString(), dr["供应商名称"].ToString());
            textBox1.Text = dr["供应商ID"].ToString();
            textBox2.Text = dr["供应商名称"].ToString();
            textBox4.Text = dr["税率"].ToString();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtP == null) throw new Exception("未选择供应商");

                dtP.NewRow();
                dtP.Rows.Add();
                gridView1.FocusedRowHandle = gridView1.LocateByDisplayText(0, gridColumn21, "");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            if (dtP == null)
            {
                MessageBox.Show("没有记录可删除");
                return;
            }
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            dr.Delete();
        }
        private void fun_check_单价保存()
        {

            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                    continue;
                if (dr["物料编码"].ToString().Trim() == "")
                {

                    throw new Exception("有物料未选择,请检查");
                }
            }



        }



        private void simpleButton4_Click(object sender, EventArgs e)
        {

            try
            {

                gridView1.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                fun_check_单价保存();
                DataTable dtAdd = dtP.GetChanges(DataRowState.Added);
                if (dtAdd != null)
                {
                    foreach (DataRow r in dtAdd.Rows)
                    {
                        string sql = string.Format("update 基础数据物料信息表 set  标准单价='{0}',特殊备注='{1}' where 物料编码='{2}'"
                             , r["单价"], r["特殊备注"], r["物料编码"]);
                        CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
                    }
                }
                DataTable dtModified = dtP.GetChanges(DataRowState.Modified);
                if (dtModified != null)
                {
                    foreach (DataRow r in dtModified.Rows)
                    {
                        //供应商编号='{0}',默认供应商='{1}',,采购供应商备注='{5}'  18-3-6  确认不需要修改默认供应商了

                        string sql = string.Format("update 基础数据物料信息表 set 标准单价='{0}',特殊备注='{1}' where 物料编码='{2}'"
                            , r["单价"], r["特殊备注"], r["物料编码"]);
                        CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
                    }
                }
                CZMaster.MasterSQL.Save_DataTable(dtP, "采购供应商物料单价表", strconn);
                MessageBox.Show("ok");
                fun_供应商物料单价(textBox1.Text, textBox2.Text);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }



        private void repositoryItemSearchLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (e.NewValue != null && e.NewValue.ToString() != "")
            {
                DataRow[] r = dt_下拉物料.Select(string.Format("物料编码='{0}'", e.NewValue));

                dr["物料名称"] = r[0]["物料名称"];
                dr["规格型号"] = r[0]["规格型号"];
                dr["单价"] = r[0]["标准单价"];
                dr["供应商ID"] = textBox1.Text;

                dr["特殊备注"] = r[0]["特殊备注"];
            }
            else
            {
                dr["物料编码"] = "";
                dr["物料名称"] = "";
                dr["规格型号"] = "";
                dr["单价"] = 0;
                dr["供应商ID"] = "";

                dr["特殊备注"] = "";

            }
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                gridControl1.ExportToXlsx(saveFileDialog.FileName);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printDialog1.Document = this.printDocument1;

            DialogResult result = this.printDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {

                string str_打印机 = this.printDocument1.PrinterSettings.PrinterName;

                string sql = string.Format(@"select 不含税单价,单价,a.物料编码,规格型号,图纸编号,物料名称,计量单位 from [采购供应商物料单价表] a,基础数据物料信息表 b
                                        where a.物料编码=b.物料编码 and a.供应商ID='{0}'", textBox1.Text.ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    DataTable dt_dy = new DataTable();
                    da.Fill(dt_dy);

                    DataView dv = new DataView(dt_dy);
                    dv.Sort = "物料编码";
                    dt_dy = dv.ToTable();

                    ItemInspection.print_FMS.fun_p_供应商单价(textBox2.Text.ToString(), str_打印机, textBox4.Text.ToString(), dt_dy);
                }
            }

        }


        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "不含税单价")
            {
                if (e.Value != null && e.Value.ToString().Trim() != "")
                {
                    decimal dec_税率 = Convert.ToDecimal(textBox4.Text.ToString()) / 100;
                    decimal dd = Math.Round(Convert.ToDecimal(e.Value) * (1 + dec_税率), 4); //保留四位小数
                    gridView1.GetDataRow(e.RowHandle)["单价"] = dd;
                }
                else
                {
                    gridView1.GetDataRow(e.RowHandle)["单价"] = 0;
                }
            }
        }

        private void treeList1_MouseClick(object sender, MouseEventArgs e)
        {
            if (treeList1.Nodes.Count > 0)
            {
                if (treeList1.Selection[0] == null) return;
            }
            else
            {
                return;
            }
            TreeListNode n = treeList1.Selection[0];
            string s = n.GetValue("供应商分类编码").ToString();
            DataView v = new DataView(dt_Provider);
            v.RowFilter = String.Format("供应商分类编码 like '{0}%'", s);
            gridControl2.DataSource = v;
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            if (e.Page.Name == "xtraTabPage1")
            {
                barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            else if (e.Page.Name == "xtraTabPage6")
            {
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

            }
        }

    }
}
