using CZMaster;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ERPpurchase
{
    /// <summary>
    /// 2020-3-20  修改txt_riqitime 原来是录入日期,现在修改为发票确认日期
    /// 财务要求发票确认日期 根据这个时间选择
    /// 
    ///</summary>
    public partial class frm采购发票核销界面 : UserControl
    {
        public frm采购发票核销界面()
        {
            InitializeComponent();
        }

        string strconn = CPublic.Var.strConn;

        /// <summary>
        /// 采购开票通知单号
        /// </summary>
        string StrCgkp = "";

        /// <summary>
        /// 操作的drm行
        /// </summary>
        DataRow drm = null;

        DataTable dt_通知单主表;

        DataTable dt_通知单明细;

        DataTable dt_发票核销表;
        string cfgfilepath = "";
        public frm采购发票核销界面(string Strkp)
        {
            StrCgkp = Strkp;

            InitializeComponent();
            if (CPublic.Var.LocalUserTeam != "财务部权限")
            {
                barLargeButtonItem10.Enabled = false;
                barLargeButtonItem11.Enabled = false;
            }
        }

        private void frm采购发票核销界面_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(this.splitContainer1, this.Name, cfgfilepath);

                fun_查询数据(StrCgkp);
                bttn_state();
                gridView2.Columns["发票金额"].AppearanceCell.BackColor = Color.Aqua;



                //txt_jechazhi.BackColor = Color.Red;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void bttn_state()
        {
            gridColumn21.OptionsColumn.AllowEdit = true;
            gridColumn22.OptionsColumn.AllowEdit = true;
            if (drm["发票确认"].Equals(true))
            {
                gridView2.OptionsBehavior.Editable = false;
                barLargeButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                b_发票确认.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                b_撤回提交.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                b_驳回.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                b_提交.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                gridColumn21.OptionsColumn.AllowEdit = false;
                gridColumn22.OptionsColumn.AllowEdit = false;
                txt_riqitime.Enabled = false;
            }
            else if (drm["提交"].Equals(true))
            {
                b_撤回提交.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                b_提交.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                if (CPublic.Var.LocalUserTeam.Contains("管理员") || CPublic.Var.LocalUserTeam.Contains("财务") || CPublic.Var.LocalUserID == "admin")
                {
                    b_发票确认.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    b_驳回.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                }
            }
            else if (!drm["提交"].Equals(true))
            {
                b_撤回提交.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                b_驳回.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                b_发票确认.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                b_提交.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }

        }

        private void fun_查询数据(string getDanhao)
        {
            try
            {
                SqlDataAdapter da;
                string sql = "";
                sql = string.Format("select * from 采购记录采购开票通知单主表 where 开票通知单号='{0}'", getDanhao);
                da = new SqlDataAdapter(sql, strconn);
                dt_通知单主表 = new DataTable();
                da.Fill(dt_通知单主表);
                if (dt_通知单主表.Rows.Count > 0)
                {
                    drm = dt_通知单主表.Rows[0];
                    dataBindHelper1.DataFormDR(drm);
                    if (drm["发票确认日期"] == DBNull.Value) txt_riqitime.EditValue = CPublic.Var.getDatetime().Date;
                    else txt_riqitime.EditValue = Convert.ToDateTime(drm["发票确认日期"]);

                }
                sql = string.Format(@"select ckpmx.*,基础数据物料信息表.计量单位 from 采购记录采购开票通知单明细表 ckpmx
                                        left join 基础数据物料信息表 on ckpmx.物料编码=基础数据物料信息表.物料编码 
                                            where ckpmx.开票通知单号='{0}'", getDanhao);
                da = new SqlDataAdapter(sql, strconn);
                dt_通知单明细 = new DataTable();
                da.Fill(dt_通知单明细);

                sql = string.Format("select * from 采购记录采购开票通知发票核销表 where 开票通知单号='{0}'", getDanhao);
                da = new SqlDataAdapter(sql, strconn);
                dt_发票核销表 = new DataTable();
                da.Fill(dt_发票核销表);
                decimal dec = 0;
                if (dt_发票核销表.Rows.Count > 0)
                {
                    // textBox2.Text = dt_发票核销表.Rows[0]["备注"].ToString();
                    textBox1.Text = dt_发票核销表.Rows[0]["折扣"].ToString();
                    if (textBox1.Text == "") textBox1.Text = "1";
                    foreach (DataRow dr in dt_发票核销表.Rows)
                    {
                        dec += Convert.ToDecimal(dr["发票金额"]);

                    }
                    txt_fapiaozje.Text = dec.ToString();
                    txt_jechazhi.Text = (Convert.ToDecimal(txt_cgshuihouje.Text) - dec).ToString();
                }
                gridControl1.DataSource = dt_通知单明细;
                gridControl2.DataSource = dt_发票核销表;
                gridView1.ViewCaption = string.Format("采购开票通知单\"{0}\"的明细", getDanhao);

                dt_发票核销表.ColumnChanged += dt_发票核销表_ColumnChanged;

            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_查询数据");
                throw ex;
            }
        }

        //发票金额的计算
        void dt_发票核销表_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            try
            {
                if (e.Column.ColumnName == "发票金额")
                {
                    txt_fapiaozje.Text = "0.00";
                    decimal TotalJe = 0;
                    decimal ToSqJe = 0;
                    foreach (DataRow r in dt_发票核销表.Rows)
                    {
                        if (r.RowState == DataRowState.Deleted) continue;

                        if (r["发票金额"].ToString() != "")
                        {
                            r["未税发票金额"] = (Convert.ToDecimal(r["发票金额"]) / (1 + Convert.ToDecimal(txt_cgshuilv.Text) / 100)).ToString("0.00");
                            r["税金"] = (Convert.ToDecimal(r["发票金额"]) - Convert.ToDecimal(r["未税发票金额"])).ToString("0.00");
                            TotalJe = TotalJe + Convert.ToDecimal(r["发票金额"]);
                            ToSqJe += Convert.ToDecimal(r["未税发票金额"]);
                            r["总金额"] = TotalJe + Convert.ToDecimal(r["系统外结算金额"]);
                        }
                    }
                    txt_fapiaozje.Text = TotalJe.ToString("0.00");
                    txt_jechazhi.Text = (Convert.ToDecimal(drm["总金额"]) - TotalJe).ToString();


                    if (textBox1.Text != "")
                    {
                        button1_Click(null, null);
                    }
                }
                if (e.Column.ColumnName == "税金")
                {
                    txt_fapiaozje.Text = "0.00";
                    decimal TotalJe = 0;
                    foreach (DataRow r in dt_发票核销表.Rows)
                    {
                        if (r.RowState == DataRowState.Deleted) continue;
                        if (r["发票金额"].ToString() != "")
                        {
                            r["未税发票金额"] = Convert.ToDecimal(r["发票金额"]) - Convert.ToDecimal(r["税金"]);

                            TotalJe = TotalJe + Convert.ToDecimal(r["发票金额"]);
                            r["总金额"] = TotalJe + Convert.ToDecimal(r["系统外结算金额"]);
                        }
                    }
                    txt_fapiaozje.Text = TotalJe.ToString("0.00");
                    txt_jechazhi.Text = (Convert.ToDecimal(drm["总金额"]) - TotalJe).ToString();
                    if (textBox1.Text != "")
                    {
                        button1_Click(null, null);
                    }
                }
                if (e.Column.ColumnName == "系统外结算金额")
                {
                    txt_fapiaozje.Text = "0.00";
                    decimal TotalJe = 0;
                    foreach (DataRow r in dt_发票核销表.Rows)
                    {
                        if (r.RowState == DataRowState.Deleted) continue;
                        if (r["发票金额"].ToString() != "")
                        {
                            r["未税发票金额"] = Convert.ToDecimal(r["发票金额"]) - Convert.ToDecimal(r["税金"]);
                            TotalJe = TotalJe + Convert.ToDecimal(r["发票金额"]);
                            r["总金额"] = TotalJe + Convert.ToDecimal(r["系统外结算金额"]);
                        }
                    }
                    txt_fapiaozje.Text = TotalJe.ToString("0.00");
                    txt_jechazhi.Text = (Convert.ToDecimal(drm["总金额"]) - TotalJe).ToString();
                    if (textBox1.Text != "")
                    {
                        button1_Click(null, null);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #region 调用的方法

        //新增方法
        private void fun_新增发票()
        {
            DataRow r = dt_发票核销表.NewRow();
            r["系统外结算金额"] = 0;
            r["税金"] = 0;
            r["总金额"] = 0;
            dt_发票核销表.Rows.Add(r);
        }

        //保存发票时的数据检查
        private void fun_check发票()
        {
            try
            {


                if (textBox1.Text != "")
                {
                    try
                    {
                        decimal dec = Convert.ToDecimal(textBox1.Text.ToString().Trim());

                    }
                    catch (Exception)
                    {

                        throw new Exception("折扣有问题，请核实");
                    }
                }
                drm["录入日期"] = CPublic.Var.getDatetime(); //此处原来用的txt_riqitime 的时间
                drm["备注1"] = textBox2.Text;
                drm["驳回意见"] = "";


                foreach (DataRow r in dt_发票核销表.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (r["GUID"] == DBNull.Value)
                        r["GUID"] = System.Guid.NewGuid();
                    r["开票通知单号"] = StrCgkp;
                    if (r["发票号"].ToString() == "")
                        throw new Exception("发票号有空值，请检查，并填写发票号！");
                    if (r["发票号"].ToString().Trim().Length < 8)
                    {
                        throw new Exception("发票号小于8位,请检查！");
                    }
                    //发票金额
                    if (r["发票金额"].ToString() == "")
                        throw new Exception("发票金额有空值，请检查，并填写发票金额！");
                    try
                    {
                        decimal checkfp = Convert.ToDecimal(r["发票金额"]);
                    }
                    catch
                    {
                        throw new Exception("发票金额应该为数字，请重新录入！");
                    }
                    //未税发票金额
                    if (r["未税发票金额"].ToString() == "")
                        throw new Exception("未税发票金额有空值，请检查，并填写发票金额！");
                    try
                    {
                        decimal checkfp = Convert.ToDecimal(r["未税发票金额"]);
                    }
                    catch
                    {
                        throw new Exception("未税发票金额应该为数字，请重新录入！");
                    }
                    //税金
                    if (r["税金"].ToString() == "")
                        throw new Exception("税金有空值，请检查，并填写发票金额！");
                    try
                    {
                        decimal checkfp = Convert.ToDecimal(r["税金"]);
                    }
                    catch
                    {
                        throw new Exception("税金应该为数字，请重新录入！");
                    }
                    if (r["发票日期"].ToString() == "")
                        throw new Exception("发票日期不能为空，请选择！");
                    r["税率"] = txt_cgshuilv.Text;
                    if (r["操作人员ID"].ToString() == "")
                    {
                        r["发票录入日期"] = CPublic.Var.getDatetime();
                        r["操作人员ID"] = CPublic.Var.LocalUserID;
                        r["操作人员"] = CPublic.Var.localUserName;
                    }
                }
                // double decw = Math.Abs(Math.Round(Convert.ToDouble(txt_fapiaozje.Text), 2) - Math.Round(Convert.ToDouble(txt_cgshuiqianje.Text), 2));
                //if (0.01 < decw )
                //{
                //    throw new Exception("税前金额有差异,开票明细需要微调");
                //}
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_check发票");
                throw ex;
            }
        }

        //发票的保存
        private void fun_save发票()
        {
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction cghx = conn.BeginTransaction("采购核销");
            try
            {

                string sql = "select * from 采购记录采购开票通知发票核销表 where 1<>1";
                SqlCommand cmd = new SqlCommand(sql, conn, cghx);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt_发票核销表);
                }
                sql = "select * from 采购记录采购开票通知单主表 where 1<>1";
                cmd = new SqlCommand(sql, conn, cghx);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt_通知单主表);
                }
                sql = "select * from 采购记录采购开票通知单明细表 where 1<>1";
                cmd = new SqlCommand(sql, conn, cghx);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt_通知单明细);
                }

                cghx.Commit();
            }
            catch (Exception ex)
            {
                cghx.Rollback();
                MasterLog.WriteLog(ex.Message + this.Name + " fun_save发票");
                throw ex;

            }

            ////采购开票发票核销表
            //SqlDataAdapter da;
            //da = new SqlDataAdapter("select * from 采购记录采购开票通知发票核销表 where 1<>1", strconn);
            //new SqlCommandBuilder(da);
            //da.Update(dt_发票核销表);
            ////采购开票通知单主表
            //dataBindHelper1.DataToDR(drm);
            //da = new SqlDataAdapter("select * from 采购记录采购开票通知单主表 where 1<>1", strconn);
            //new SqlCommandBuilder(da);
            //da.Update(dt_通知单主表);
            ////采购开票通知单明细表
            //da = new SqlDataAdapter("select * from 采购记录采购开票通知单明细表 where 1<>1", strconn);
            //new SqlCommandBuilder(da);
            //da.Update(dt_通知单明细);

        }

        //发票确认的赋值
        private void fun_check发票确认()
        {
            try
            {    //开票通知单主表
                if (textBox1.Text.ToString() == "")
                {
                    throw new Exception("请录入折扣，没有折扣填 1");
                }
                decimal dec = Convert.ToDecimal(textBox1.Text); //折扣
                DateTime time1 = CPublic.Var.getDatetime();
                DateTime time2 = Convert.ToDateTime(txt_riqitime.EditValue).Date;

                //DateTime time1 = Convert.ToDateTime("2019-12-31 19:00:00");
                //if (drm["发票确认人ID"].ToString() == "")
                //{
                drm["发票确认人ID"] = CPublic.Var.LocalUserID;
                drm["发票确认人"] = CPublic.Var.localUserName;
                drm["发票确认日期"] = time2;
                //}
                drm["发票确认"] = true;
                drm["折扣"] = dec;
                drm["折扣前总金额"] = drm["总金额"];
                drm["折扣前未税金额"] = drm["未税金额"];
                drm["录入日期"] = time1; // txt_riqitime.EditValue.ToString();

                //开票通知单明细
                foreach (DataRow r in dt_通知单明细.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    r["发票确认"] = true;
                    //if (r["发票确认人ID"].ToString() == "")
                    //{
                    r["发票确认人ID"] = CPublic.Var.LocalUserID;
                    r["发票确认人"] = CPublic.Var.localUserName;
                    r["发票确认日期"] = time2;

                    //}
                    r["录入日期"] = time1; //txt_riqitime.EditValue.ToString();
                    if (textBox1.Text != "" && textBox1.Text != "1")
                    {
                        r["折扣"] = dec;
                        r["折扣后含税单价"] = Convert.ToDecimal(r["单价"]) * dec;
                        r["折扣后含税金额"] = Convert.ToDecimal(r["金额"]) * dec;
                        r["折扣后不含税单价"] = Convert.ToDecimal(r["未税单价"]) * dec;
                        r["折扣后不含税金额"] = Convert.ToDecimal(r["未税金额"]) * dec;

                    }
                }
                //发票核销表
                foreach (DataRow r in dt_发票核销表.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    r["发票确认"] = true;
                    r["折扣"] = textBox1.Text;
                    r["备注"] = textBox2.Text.ToString().Trim();
                    //if (r["发票确认人ID"].ToString() == "")
                    //{
                    r["发票确认人ID"] = CPublic.Var.LocalUserID;
                    r["发票确认人"] = CPublic.Var.localUserName;
                    r["发票确认日期"] = time2;
                    //}
                    //17-9-29  开票通知单主表 备注2  加入 所有的 发票号   ex: num1,num2,....
                    drm["备注2"] = drm["备注2"].ToString() + r["发票号"] + " ";

                }
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_check发票确认");
                throw ex;
            }
        }

        #endregion

        #region  界面的操作
        //刷新操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_查询数据(drm["开票通知单号"].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //新增发票操作
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_新增发票();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //删除发票的操作
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (dt_发票核销表 == null || dt_发票核销表.Rows.Count <= 0) return;
                DataRow r = (this.BindingContext[dt_发票核销表].Current as DataRowView).Row;
                if (MessageBox.Show(string.Format("你确定要删除发票号为\"{0}\"的发票吗？", r["发票号"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    r.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //界面的关闭
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        //发票的保存操作
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (dt_发票核销表 == null || dt_发票核销表.Rows.Count <= 0) return;
                gridView2.CloseEditor();
                this.BindingContext[dt_发票核销表].EndCurrentEdit();
                fun_check发票();
                if (Convert.ToDecimal(txt_jechazhi.Text) < 0)
                {
                    if (MessageBox.Show(string.Format("开票通知单\"{0}\"的税后金额小于发票总额了，确定要保存吗？", drm["开票通知单号"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        fun_save发票();
                        fun_查询数据(drm["开票通知单号"].ToString());
                        MessageBox.Show("保存成功！");
                    }
                }
                else
                {
                    fun_save发票();
                    fun_查询数据(drm["开票通知单号"].ToString());
                    MessageBox.Show("保存成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //发票确认操作
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //20-3-23 检查开票金额 与 采购金额是否金额差额1元以内 超过需要提交加个价格异动单
                ERPorg.Corg cg = new ERPorg.Corg();
                if (cg.price_changed(dt_通知单明细))
                {
                    if (MessageBox.Show(string.Format("请注意此张开票通知单采购需提供价格异动单,是否继续?"), "提醒!", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        throw new Exception("已取消确认");
                    }

                }
                string s = string.Format("select  * from 采购记录采购开票通知单主表 where 生效=1 and 发票确认=0 and 提交=1 and 开票通知单号='{0}'", txt_kaipiaotzd.Text);
                DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                if (t.Rows.Count == 0) throw new Exception("单据状态有误,请确认是否提交");

                gridView2.CloseEditor();
                this.BindingContext[dt_发票核销表].EndCurrentEdit();

                if (dt_发票核销表 == null || dt_发票核销表.Rows.Count <= 0) return;
                if (System.Math.Abs(Convert.ToDouble(txt_jechazhi.Text)) > 1)
                    throw new Exception(string.Format("开票通知单\"{0}\"的税后金额小于发票总额了,不允许进行发票确认！", drm["开票通知单号"].ToString()));
                fun_check发票();
                fun_check发票确认();
                fun_save发票();
                MessageBox.Show("发票确认成功！");
                gridView2.OptionsBehavior.Editable = false;
                bttn_state();

                txt_riqitime.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //导出
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                //gridControl1.ExportToXls(saveFileDialog.FileName, options);  
                gridControl1.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != null && textBox1.Text.ToString() != "")
            {
                txt_cgshuiqianje.Text = (Convert.ToDecimal(drm["未税金额"]) * Convert.ToDecimal(textBox1.Text)).ToString("0.00");
                txt_cgshuihouje.Text = (Convert.ToDecimal(drm["总金额"]) * Convert.ToDecimal(textBox1.Text)).ToString("0.00");
                txt_jechazhi.Text = (Convert.ToDecimal(txt_cgshuihouje.Text) - Convert.ToDecimal(txt_fapiaozje.Text)).ToString("0.00");
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }

        }

        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try

            {
                string sql = string.Format("select * from 采购记录采购开票通知单主表 where 开票通知单号='{0}'", txt_kaipiaotzd.Text);
                DataRow drM = CZMaster.MasterSQL.Get_DataRow(sql, strconn);
                sql = string.Format("select * from 采购记录采购开票通知单明细表 where  开票通知单号='{0}'", txt_kaipiaotzd.Text);

                DataTable dtm = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPreport.结算单", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统                                                         //  CPublic.UIcontrol.Showpage(ui, t.Rows[0]["打开界面名称"].ToString());
                object[] drr = new object[2];
                drr[0] = drM;
                drr[1] = dtm;
                //   drr[2] = dr["出入库申请单号"].ToString();
                Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
                ui.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //生成凭证
        private void barLargeButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //这边是一个开票通知单是一笔凭证
                fun_check();
                DateTime t_now = CPublic.Var.getDatetime();

                DateTime time = Convert.ToDateTime(txt_riqitime.EditValue);
                int year = time.Year;
                int month = time.Month;
                string strcon_u8 = CPublic.Var.geConn("DW");
                string ERP_凭证号 = "";
                string U8_凭证号 = "";
                string s_发票号 = "";
                int irow = 1;
                //20-1-15 生成凭证需要检查是不是当前未结账月




                foreach (DataRow rt in dt_发票核销表.Rows)
                {
                    if (rt.RowState == DataRowState.Deleted) continue;
                    s_发票号 += rt["发票号"].ToString();
                    if (irow++ != dt_发票核销表.Rows.Count) s_发票号 += "/";

                }

                string x = string.Format("select  * from 财务凭证表 where 单据号='{0}'", txt_kaipiaotzd.Text);
                DataTable t_erp = CZMaster.MasterSQL.Get_DataTable(x, strconn);

                if (t_erp.Rows.Count == 0)
                {
                    ERP_凭证号 = CPublic.CNo.fun_得到最大流水号("PZ", year, month).ToString();

                }
                else
                {
                    ERP_凭证号 = t_erp.Rows[0]["凭证号"].ToString();
                    U8_凭证号 = t_erp.Rows[0]["U8凭证号"].ToString();
                }
                //这边需要根据名称 去u8搜一下供应商编码  因为这边可能编码不一样
                string gys = string.Format("select cvencode from Vendor where cVenName = '{0}'", drm["供应商名称"]);
                DataTable dt_gys = CZMaster.MasterSQL.Get_DataTable(gys, strcon_u8);
                if (dt_gys.Rows.Count == 0) throw new Exception("因本系统与U8用该名称不一致,未找到供应商编号,请将两个供应商名称一致");
                string str_gys = dt_gys.Rows[0]["cvencode"].ToString();
                MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
                DataTable dt_凭证 = RBQ.SelectGroupByInto("", dt_通知单明细, "科目编码,科目名称,sum(未税金额) 不含税金额 ", "", "科目编码,科目名称");
                string s = string.Format("select * from GL_accvouch where iyear={0} and iperiod={1} and ino_id='{2}'", year, month, U8_凭证号);
                DataTable dt_u8 = CZMaster.MasterSQL.Get_DataTable(s, strcon_u8);
                if (U8_凭证号 != "") //已有数据 需要把原来的先删除 再增加
                {

                    for (int l = dt_u8.Rows.Count - 1; l >= 0; l--)
                    {
                        dt_u8.Rows[l].Delete();
                    }
                    for (int j = t_erp.Rows.Count - 1; j >= 0; j--)
                    {
                        t_erp.Rows[j].Delete();
                    }
                }
                else
                {
                    string xx = string.Format("select isnull(MAX(ino_id),0) 凭证号 from GL_accvouch where iyear={0} and iperiod={1}", year, month);
                    DataRow pzh = CZMaster.MasterSQL.Get_DataRow(xx, strcon_u8);
                    U8_凭证号 = (Convert.ToInt32(pzh[0]) + 1).ToString();
                }
                int i = 1; //行号
                //这里新增的是 按科目汇总后的开票明细
                foreach (DataRow r_pz in dt_凭证.Rows)
                {
                    DataRow r = dt_u8.NewRow();
                    r["iperiod"] = month;
                    r["csign"] = "记";
                    r["isignseq"] = 1;
                    r["ino_id"] = U8_凭证号;
                    r["inid"] = i;
                    r["dbill_date"] = t_now;
                    r["idoc"] = -1;
                    r["bdelete"] = 0;
                    r["bvouchedit"] = 1; //可修改
                    r["bvouchAddordele"] = 0; //bvouchAddordele 是否可增删
                    r["bvouchmoneyhold"] = 0; //凭证合计金额是否保值 
                    r["bvalueedit"] = 1; //分录数值是否可修改 
                    r["bcodeedit"] = 1; //分录科目是否可修改  
                    r["bPCSedit"] = 1; //分录往来项是否可修改   
                    r["bDeptedit"] = 1; //分录部门是否可修改    
                    r["bItemedit"] = 1; //分录项目是否可修改 
                    r["bCusSupInput"] = 0; //分录往来项是否必输  

                    r["cbill"] = CPublic.Var.localUserName;
                    r["ctext1"] = ERP_凭证号;
                    string digest1 = "购材料 " + txt_caigougys.Text + " 发票号:" + s_发票号;
                    if (digest1.Length > 120) //u8字段长度限制120 超出我就不写入
                        r["cdigest"] = "购材料 " + txt_caigougys.Text + " 发票号:";
                    else
                    {
                        r["cdigest"] = digest1;
                    }
                    //r["cdigest"] = "购材料 " + txt_caigougys.Text + " 发票号:" + s_发票号;
                    // ctext1里面存放我们的凭证号
                    r["ccode"] = r_pz["科目编码"];
                    r["md"] = r_pz["不含税金额"];
                    r["ccodeexch_equal"] = r["ccode_equal"] = "22020101"; //对应的都是进项税
                    r["coutaccset"] = "008";
                    r["doutbilldate"] = time;
                    r["RowGuid"] = System.Guid.NewGuid();
                    r["iyear"] = year;
                    r["iYPeriod"] = year.ToString() + month.ToString("00");
                    r["tvouchtime"] = t_now;
                    dt_u8.Rows.Add(r);

                    DataRow r_erp = t_erp.NewRow();
                    r_erp["凭证号"] = ERP_凭证号;
                    r_erp["U8凭证号"] = U8_凭证号;
                    r_erp["inid"] = i;
                    r_erp["摘要"] = "购材料 " + txt_caigougys.Text + " 发票号:" + s_发票号;
                    r_erp["制单日期"] = t_now;
                    r_erp["制单人"] = CPublic.Var.localUserName;
                    r_erp["年"] = year;
                    r_erp["月"] = month;
                    r_erp["科目编号"] = r_pz["科目编码"];
                    r_erp["科目名称"] = r_pz["科目名称"];
                    r_erp["借方金额"] = r_pz["不含税金额"];
                    r_erp["单据号"] = txt_kaipiaotzd.Text.Trim();
                    t_erp.Rows.Add(r_erp);
                    i++;
                }
                //科目明细项已经增加进去了 还要增加 进项税 和 贷方金额的总金额
                //19-11-28 财务要求 进项税 根据 有几张发票 增加几行进项税 
                #region 进项税
                foreach (DataRow r_jxs in dt_发票核销表.Rows)
                {
                    DataRow r1 = dt_u8.NewRow();
                    r1["iperiod"] = month;
                    r1["csign"] = "记";
                    r1["isignseq"] = 1;
                    r1["ino_id"] = U8_凭证号;
                    r1["inid"] = i;
                    r1["dbill_date"] = t_now;
                    r1["bdelete"] = 0;
                    r1["bvouchedit"] = 1; //可修改
                    r1["bvouchAddordele"] = 0; //bvouchAddordele 是否可增删
                    r1["bvouchmoneyhold"] = 0; //凭证合计金额是否保值 
                    r1["bvalueedit"] = 1; //分录数值是否可修改 
                    r1["bcodeedit"] = 1; //分录科目是否可修改  
                    r1["bPCSedit"] = 1; //分录往来项是否可修改   
                    r1["bDeptedit"] = 1; //分录部门是否可修改    
                    r1["bItemedit"] = 1; //分录项目是否可修改 
                    r1["bCusSupInput"] = 0; //分录往来项是否必输  
                    r1["idoc"] = -1;
                    r1["cbill"] = CPublic.Var.localUserName;
                    r1["ctext1"] = ERP_凭证号;

                    string digest1 = "购材料 " + txt_caigougys.Text + " 发票号:" + s_发票号;
                    if (digest1.Length > 120) //u8字段长度限制120 超出我就不写入
                        r1["cdigest"] = "购材料 " + txt_caigougys.Text + " 发票号:";
                    else
                    {
                        r1["cdigest"] = digest1;
                    }
                    //r1["cdigest"] = "购材料 " + txt_caigougys.Text + " 发票号:" + s_发票号;
                    // ctext1里面存放我们的凭证号
                    r1["ccode"] = "22210101"; //进项税
                    //这里是总的 税金                          //
                    //r1["md"] = Math.Round(Convert.ToDecimal(txt_cgshuijin.Text), 2, MidpointRounding.AwayFromZero);
                    //这是每个发票得 税金 明细
                    r1["md"] = Math.Round(Convert.ToDecimal(r_jxs["税金"]), 2, MidpointRounding.AwayFromZero);
                    r1["ccodeexch_equal"] = r1["ccode_equal"] = "22020101";          //对应的都是进项税
                    r1["coutaccset"] = "008";
                    r1["doutbilldate"] = time;
                    r1["RowGuid"] = System.Guid.NewGuid();
                    r1["iyear"] = year;
                    r1["iYPeriod"] = year.ToString() + month.ToString("00");
                    r1["tvouchtime"] = t_now;
                    dt_u8.Rows.Add(r1);


                    DataRow r_erp1 = t_erp.NewRow();
                    r_erp1["凭证号"] = ERP_凭证号;
                    r_erp1["U8凭证号"] = U8_凭证号;
                    r_erp1["inid"] = i;
                    r_erp1["摘要"] = "购材料 " + txt_caigougys.Text + " 发票号:" + s_发票号;
                    r_erp1["制单日期"] = t_now;
                    r_erp1["制单人"] = CPublic.Var.localUserName;
                    r_erp1["年"] = year;
                    r_erp1["月"] = month;
                    r_erp1["科目编号"] = "22210101";
                    r_erp1["科目名称"] = "进项税";

                    //这里是总的 税金                          //
                    //r_erp1["借方金额"] = Math.Round(Convert.ToDecimal(txt_cgshuijin.Text), 2, MidpointRounding.AwayFromZero);
                    //这是每个发票得 税金 明细
                    r_erp1["借方金额"] = Math.Round(Convert.ToDecimal(r_jxs["税金"]), 2, MidpointRounding.AwayFromZero);
                    r_erp1["单据号"] = txt_kaipiaotzd.Text.Trim();
                    t_erp.Rows.Add(r_erp1);
                    i++;
                }
                #endregion
                string exch = "";
                int int_ex = 1;
                foreach (DataRow exr in dt_u8.Rows)
                {
                    if (exr.RowState == DataRowState.Deleted) continue;

                    exch = exch + exr["ccode"];
                    if (int_ex++ != dt_u8.Rows.Count) exch = exch + ",";
                }
                #region 贷方金额  总金额
                DataRow rr = dt_u8.NewRow();
                rr["iperiod"] = month;
                rr["csign"] = "记";
                rr["isignseq"] = 1;
                rr["ino_id"] = U8_凭证号;
                rr["inid"] = i;
                rr["dbill_date"] = t_now;
                rr["bdelete"] = 0;
                rr["bvouchedit"] = 1; //可修改
                rr["bvouchAddordele"] = 0; //bvouchAddordele 是否可增删
                rr["bvouchmoneyhold"] = 0; //凭证合计金额是否保值 
                rr["bvalueedit"] = 1; //分录数值是否可修改 
                rr["bcodeedit"] = 1; //分录科目是否可修改  
                rr["bPCSedit"] = 1; //分录往来项是否可修改   
                rr["bDeptedit"] = 1; //分录部门是否可修改    
                rr["bItemedit"] = 1; //分录项目是否可修改 
                rr["bCusSupInput"] = 0; //分录往来项是否必输  
                rr["csup_id"] = str_gys;//这个需要录入供应商

                rr["idoc"] = -1;
                rr["cbill"] = CPublic.Var.localUserName;
                rr["ctext1"] = ERP_凭证号;
                string digest = "购材料 " + txt_caigougys.Text + " 发票号:" + s_发票号;
                if (digest.Length > 120) //u8字段长度限制120 超出我就不写入
                    rr["cdigest"] = "购材料 " + txt_caigougys.Text + " 发票号:";
                else
                {
                    rr["cdigest"] = digest;
                }
                // ctext1里面存放我们的凭证号
                rr["ccode"] = "22020101"; //正常应付材料
                rr["mc"] = Math.Round(Convert.ToDecimal(txt_cgshuihouje.Text), 2, MidpointRounding.AwayFromZero);
                rr["ccodeexch_equal"] = exch;          //对应的都是进项税

                if (exch.Length > 50)
                    rr["ccode_equal"] = exch.Substring(0, 50);                   //对应的都是进项税
                else
                    rr["ccode_equal"] = exch;                           //对应的都是进项税
                //19-12-26 u8中 ccode_equal字段长度 只有 50  
                rr["coutaccset"] = "008";
                rr["doutbilldate"] = time;
                rr["RowGuid"] = System.Guid.NewGuid();
                rr["iyear"] = year;
                rr["iYPeriod"] = year.ToString() + month.ToString("00");
                rr["tvouchtime"] = t_now;
                dt_u8.Rows.Add(rr);

                DataRow rr_erp = t_erp.NewRow();
                rr_erp["凭证号"] = ERP_凭证号;
                rr_erp["U8凭证号"] = U8_凭证号;
                rr_erp["inid"] = i;
                rr_erp["摘要"] = "购材料 " + txt_caigougys.Text + " 发票号:" + s_发票号;
                rr_erp["制单日期"] = t_now;
                rr_erp["制单人"] = CPublic.Var.localUserName;
                rr_erp["年"] = year;
                rr_erp["月"] = month;
                rr_erp["科目编号"] = "22020101";
                rr_erp["科目名称"] = "正常应付材料";
                rr_erp["贷方金额"] = Math.Round(Convert.ToDecimal(txt_cgshuihouje.Text), 2, MidpointRounding.AwayFromZero);
                rr_erp["单据号"] = txt_kaipiaotzd.Text.Trim();
                t_erp.Rows.Add(rr_erp);
                i++;
                #endregion

                drm["备注5"] = "U8凭证号:" + U8_凭证号;
                drm["bl_pz"] = true;



                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("pur"); //事务的名称
                SqlCommand cmd1 = new SqlCommand("select * from 采购记录采购开票通知单主表 where 1<>1", conn, ts);
                SqlCommand cmd = new SqlCommand(x, conn, ts);

                try
                {

                    SqlDataAdapter da;
                    da = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da);
                    da.Update(dt_通知单主表);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(t_erp);
                    CZMaster.MasterSQL.Save_DataTable(dt_u8, "GL_accvouch", strcon_u8);
                    ts.Commit();
                    MessageBox.Show("生成凭证成功");

                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    throw new Exception(ex.Message);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_check()
        {
            //if (Convert.ToBoolean(drm["凭证"]))
            //{
            //    throw new Exception("已生成凭证");
            //}
            if (!Convert.ToBoolean(drm["发票确认"]))
            { throw new Exception("尚未进行发票确认,不可生成凭证"); }
            foreach (DataRow dr in dt_通知单明细.Rows)
            {
                if (dr["科目编码"] == null || dr["科目编码"].ToString() == "")
                {
                    throw new Exception("存在科目编码为空请检查");
                }
                if (dr["科目名称"] == null || dr["科目名称"].ToString() == "")
                {
                    throw new Exception("存在科目名称为空请检查");
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string s = string.Format(@"select ckpmx.*,基础数据物料信息表.计量单位,对方科目编码,对方科目名称 from 采购记录采购开票通知单明细表 ckpmx
      left join 基础数据物料信息表 on ckpmx.物料编码=基础数据物料信息表.物料编码 
      left join (select  收发类别编码,收发类别名称,存货分类编码,存货分类名称,对方科目编码,对方科目名称  from 科目对应关系包含部门 
      where  存货分类编码 <>'' and 收发类别名称 ='采购入库'  group by 收发类别编码,收发类别名称,存货分类编码,存货分类名称,对方科目编码,对方科目名称)cc
      on cc.存货分类编码=left(ckpmx.物料编码,len(cc.存货分类编码))    where ckpmx.开票通知单号='{0}'", txt_kaipiaotzd.Text);
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            foreach (DataRow r in t.Rows)
            {
                DataRow[] rr = dt_通知单明细.Select(string.Format("通知单明细号='{0}'", r["通知单明细号"]));
                if (rr[0]["科目编码"] == null || rr[0]["科目编码"].ToString() == "")
                {
                    rr[0]["科目编码"] = r["对方科目编码"];
                    rr[0]["科目名称"] = r["对方科目名称"];
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                CZMaster.MasterSQL.Save_DataTable(dt_通知单明细, "采购记录采购开票通知单明细表", strconn);
                MessageBox.Show("保存成功");
                //SqlConnection conn = new SqlConnection(strconn);
                //conn.Open();
                //SqlTransaction ts = conn.BeginTransaction("采购发票财务科目");
                //string s = "select * from 销售记录销售出库通知单明细表 where  1=2";
                //string s_z = "select * from 销售记录销售出库通知单主表 where  1=2";
                //SqlCommand cmd = new SqlCommand(s, conn, ts);
                //try
                //{
                //    SqlDataAdapter da;
                //    da = new SqlDataAdapter(cmd);
                //    new SqlCommandBuilder(da);
                //    da.Update(dt_通知单明细);

                //    cmd = new SqlCommand(s_z, conn, ts);
                //    da = new SqlDataAdapter(cmd);
                //    new SqlCommandBuilder(da);
                //    da.Update(dt_通知单主表);
                //    ts.Commit();
                //}
                //catch (Exception ex)
                //{
                //    ts.Rollback();
                //    throw new Exception("");
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string x = drm["备注5"].ToString();
                if (x == "") throw new Exception("没有凭证可删除");
                string[] xx = x.Split(':');

                string u8_凭证号 = xx[1];
                if (u8_凭证号 == "") throw new Exception("没有凭证可删除");
                DateTime time = Convert.ToDateTime(txt_riqitime.EditValue);
                int year = time.Year;
                int month = time.Month;
                string sql = $"select count(*)xx from 仓库月出入库结转表 where 结算日期 >='{time}' ";
                DataRow r_temp = CZMaster.MasterSQL.Get_DataRow(sql, strconn);
                if (Convert.ToInt32(r_temp[0]) > 0)
                {
                    throw new Exception($"{year}年{month}月已结账不可删除");
                }

                if (MessageBox.Show(string.Format("是否确认删除U8凭证号'{0}'？", u8_凭证号), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    string strcon_u8 = CPublic.Var.geConn("DW");
                    string s = string.Format("delete GL_accvouch where iyear={0} and iperiod={1} and ino_id='{2}'", year, month, u8_凭证号);
                    CZMaster.MasterSQL.ExecuteSQL(s, strcon_u8);
                    s = string.Format(@"delete  财务凭证表 where U8凭证号='{0}'  and 年='{1}' and 月='{2}'
                       update 采购记录采购开票通知单主表 set 备注5='',bl_pz=0  where 开票通知单号='{3}'", u8_凭证号, year, month, txt_kaipiaotzd.Text.Trim());
                    CZMaster.MasterSQL.ExecuteSQL(s, strconn);
                    MessageBox.Show("凭证已删除");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //20-1-20 增肌采购提交 财务审核 和驳回 --为减少工作量 在生效和 发票确认中间增加 提交   发票确认改为财务审核
        private void barLargeButtonItem13_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //20-3-23 检查开票金额 与 采购金额是否金额差额1元以内 超过需要提交加个价格异动单
                ERPorg.Corg cg = new ERPorg.Corg();
                if (cg.price_changed(dt_通知单明细))
                {
                    if (MessageBox.Show(string.Format("请注意此张开票通知单需提供给财务价格异动单,是否继续?"), "提醒!", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        throw new Exception("已取消提交");
                    }

                }

                if (dt_发票核销表.Rows.Count == 0) throw new Exception("尚未录入发票信息,不可提交");

                string s = string.Format("select  * from 采购记录采购开票通知单主表 where 生效=1 and 发票确认=0 and 提交=0 and 开票通知单号='{0}'", txt_kaipiaotzd.Text);
                DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                if (t.Rows.Count == 1)
                {
                    s = string.Format("update  采购记录采购开票通知单主表  set 提交=1 where 开票通知单号='{0}' ", txt_kaipiaotzd.Text);
                    CZMaster.MasterSQL.ExecuteSQL(s, strconn);
                    MessageBox.Show("提交成功");
                    dt_通知单主表.Rows[0]["提交"] = true;

                    dt_通知单主表.AcceptChanges();
                    bttn_state();
                }
                else
                {
                    throw new Exception("单据状态有误");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void barLargeButtonItem14_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string s = string.Format("select  * from 采购记录采购开票通知单主表 where 生效=1 and 发票确认=0 and 提交=1 and 开票通知单号='{0}'", txt_kaipiaotzd.Text);
                DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                if (t.Rows.Count == 1)
                {
                    s = string.Format("update  采购记录采购开票通知单主表  set 提交=0 where 开票通知单号='{0}' ", txt_kaipiaotzd.Text);
                    MessageBox.Show("撤回成功");
                    b_提交.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    b_撤回提交.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    b_驳回.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                }
                else
                {
                    throw new Exception("单据状态有误");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //驳回
        private void barLargeButtonItem12_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fm采购开票驳回 fm = new fm采购开票驳回(txt_kaipiaotzd.Text, txt_caigougys.Text);
                fm.ShowDialog();
                if (fm.bl_enter)
                {
                    string s = string.Format("select  * from [采购记录采购开票通知单主表] where 开票通知单号='{0}'", txt_kaipiaotzd.Text);
                    DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                    s = "select  * from 采购开票驳回意见 where 1=2";
                    DataTable t_record = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                    t.Rows[0]["驳回意见"] = fm.s;
                    t.Rows[0]["提交"] = 0;
                    DataRow dr = t_record.NewRow();
                    dr["GUID"] = System.Guid.NewGuid();
                    dr["开票通知单号"] = txt_kaipiaotzd.Text;
                    dr["驳回意见"] = fm.s;
                    dr["供应商"] = txt_caigougys.Text;
                    dr["驳回人"] = CPublic.Var.localUserName;
                    dr["驳回时间"] = CPublic.Var.getDatetime();
                    t_record.Rows.Add(dr);


                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction cghx = conn.BeginTransaction("核销驳回");
                    try
                    {

                        string sql = "select * from 采购记录采购开票通知单主表 where 1<>1";
                        SqlCommand cmd = new SqlCommand(sql, conn, cghx);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            new SqlCommandBuilder(da);
                            da.Update(t);
                        }

                        cmd = new SqlCommand(s, conn, cghx);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            new SqlCommandBuilder(da);
                            da.Update(t_record);
                        }


                        cghx.Commit();
                        t.AcceptChanges();
                        drm.ItemArray = t.Rows[0].ItemArray;
                        dt_通知单主表.AcceptChanges();
                        MessageBox.Show("驳回成功");
                    }
                    catch (Exception ex)
                    {
                        cghx.Rollback();
                        MasterLog.WriteLog(ex.Message + this.Name + " fun_save发票");
                        throw ex;

                    }
                    bttn_state();


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }


    }
}
