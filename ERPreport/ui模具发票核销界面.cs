using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using CZMaster;
namespace ERPreport
{
    public partial class ui模具发票核销界面 : UserControl
    {


        /// <summary>
        /// 模具开票通知单号
        /// </summary>
        string StrModkp = "";
        string strcon = CPublic.Var.strConn;
        /// <summary>
        /// 操作的drm行
        /// </summary>
        DataRow drm = null;

        DataTable dt_通知单主表;

        DataTable dt_通知单明细;

        DataTable dt_发票核销表;

        public ui模具发票核销界面()
        {
            InitializeComponent();
        }
        public ui模具发票核销界面(string Strkp)
        {
            StrModkp = Strkp;
            InitializeComponent();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow r = dt_发票核销表.NewRow();
            dt_发票核销表.Rows.Add(r);
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (dt_发票核销表 == null || dt_发票核销表.Rows.Count <= 0) return;
                DataRow r = gridView2.GetDataRow (gridView2.FocusedRowHandle);
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
        private void fun_查询数据(string getDanhao)
        {
            try
            {
                SqlDataAdapter da;
                string sql = "";
                sql = string.Format("select * from 模具开票通知单主表 where 模具开票通知号='{0}'", getDanhao);
                da = new SqlDataAdapter(sql, strcon);
                dt_通知单主表 = new DataTable();
                da.Fill(dt_通知单主表);
                if (dt_通知单主表.Rows.Count > 0)
                {
                    drm = dt_通知单主表.Rows[0];
                    dataBindHelper1.DataFormDR(drm);
                }
                sql = string.Format(@"select * from 模具开票通知明细表 
                                 where 模具开票通知号='{0}'", getDanhao);
                da = new SqlDataAdapter(sql, strcon);
                dt_通知单明细 = new DataTable();
                da.Fill(dt_通知单明细);

                sql = string.Format("select * from 模具开票通知发票核销表 where 模具开票通知号='{0}'", getDanhao);
                da = new SqlDataAdapter(sql, strcon);
                dt_发票核销表 = new DataTable();
                da.Fill(dt_发票核销表);
                if (dt_发票核销表.Rows.Count > 0)
                {
                    textBox2.Text = dt_发票核销表.Rows[0]["备注"].ToString();
                
                }
                gridControl1.DataSource = dt_通知单明细;
                gridControl2.DataSource = dt_发票核销表;
                decimal d = 0;
                foreach (DataRow dr in dt_发票核销表.Rows)
                {
                    d += Convert.ToDecimal( dr["发票金额"].ToString());
                }
                txt_fapiaozje.Text = d.ToString();
                gridView1.ViewCaption = string.Format("模具开票通知单\"{0}\"的明细", getDanhao);

            
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_查询数据");
                throw ex;
            }
        }
        private void fun_check发票()
        {
            try
            {
                
                foreach (DataRow r in dt_发票核销表.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;

                    r["模具开票通知号"] = StrModkp;
                    r["厂商编号"] = txt_csbh.Text;
                    r["厂商名称"] = textBox1.Text;
                    if (r["发票号"].ToString() == "")
                        throw new Exception("发票号有空值，请检查，并填写发票号！");
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
                    //if (r["未税发票金额"].ToString() == "")
                    //    throw new Exception("未税发票金额有空值，请检查，并填写发票金额！");
                    //try
                    //{
                    //    decimal checkfp = Convert.ToDecimal(r["未税发票金额"]);
                    //}
                    //catch
                    //{
                    //    throw new Exception("未税发票金额应该为数字，请重新录入！");
                    //}
                    ////税金
                    //if (r["税金"].ToString() == "")
                    //    throw new Exception("税金有空值，请检查，并填写发票金额！");
                    //try
                    //{
                    //    decimal checkfp = Convert.ToDecimal(r["税金"]);
                    //}
                    //catch
                    //{
                    //    throw new Exception("税金应该为数字，请重新录入！");
                    //}
                    if (r["发票日期"].ToString() == "")
                        throw new Exception("发票日期不能为空，请选择！");
                    //r["税率"] = txt_cgshuilv.Text;
                    r["修改时间"] = CPublic.Var.getDatetime();
                    r["操作人ID"] = CPublic.Var.LocalUserID;
                    r["操作人"] = CPublic.Var.localUserName;
                }
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_check发票");
                throw ex;
            }
        }

        private void fun_check发票确认()
        {
            try
            {    //开票通知单主表
                //decimal dec = Convert.ToDecimal(textBox1.Text); //折扣

                drm["发票确认"] = true;
                
                drm["发票确认人"] = CPublic.Var.localUserName;
                drm["发票确认时间"] = CPublic.Var.getDatetime();
           
                drm["发票总额"] = Convert.ToDecimal(txt_totalm.Text);
             
                //开票通知单明细
                foreach (DataRow r in dt_通知单明细.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    //r["发票确认"] = true;
                    //r["发票确认人ID"] = CPublic.Var.LocalUserID;
                    //r["发票确认人"] = CPublic.Var.localUserName;
                    //r["发票确认日期"] = CPublic.Var.getDatetime();
                    //if (textBox1.Text != "" && textBox1.Text != "1")
                    //{
                    //    r["折扣"] = dec;
                    //    r["折扣后含税单价"] = Convert.ToDecimal(r["单价"]) * dec;
                    //    r["折扣后含税金额"] = Convert.ToDecimal(r["金额"]) * dec;
                    //    r["折扣后不含税单价"] = Convert.ToDecimal(r["未税单价"]) * dec;
                    //    r["折扣后不含税金额"] = Convert.ToDecimal(r["未税金额"]) * dec;

                    //}
                }
                //发票核销表
                foreach (DataRow r in dt_发票核销表.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
             
         
                    r["备注"] = textBox2.Text.ToString().Trim();
                 
                    r["发票确认时间"] = CPublic.Var.getDatetime();

                }
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_check发票确认");
                throw ex;
            }
        }
        private void fun_save发票()
        {
            try
            {   //采购开票发票核销表
                SqlDataAdapter da;
                da = new SqlDataAdapter("select * from 模具开票通知发票核销表 where 1<>1", strcon);
                new SqlCommandBuilder(da);
                da.Update(dt_发票核销表);
                //采购开票通知单主表
                dataBindHelper1.DataToDR(drm);
                da = new SqlDataAdapter("select * from  模具开票通知单主表 where 1<>1", strcon);
                new SqlCommandBuilder(da);
                da.Update(dt_通知单主表);
                //采购开票通知单明细表
                da = new SqlDataAdapter("select * from 模具开票通知明细表 where 1<>1", strcon);
                new SqlCommandBuilder(da);
                da.Update(dt_通知单明细);
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_save发票");
                throw ex;
            }
        }

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
                    if (MessageBox.Show(string.Format("开票通知单\"{0}\"的税后金额小于发票总额了，确定要保存吗？", drm["模具开票通知号"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        fun_save发票();
                        fun_查询数据(drm["模具开票通知号"].ToString());
                        MessageBox.Show("保存成功！");
                    }
                }
                else
                {
                    fun_save发票();
                    fun_查询数据(drm["模具开票通知号"].ToString());
                    MessageBox.Show("保存成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ui模具发票核销界面_Load(object sender, EventArgs e)
        {
            try
            {
          
                //txt_jechazhi2.ForeColor = Color.Red;
                fun_查询数据(StrModkp);
                txt_jechazhi.Text = (Convert.ToDecimal(drm["总金额"]) - Convert.ToDecimal(drm["发票总额"])).ToString();
                if (drm["发票确认"].Equals(true))
                {
                    gridView2.OptionsBehavior.Editable = false;
                    barLargeButtonItem6.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                  
                    barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    //barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    //txt_riqitime.Enabled = false;
                }
                gridView2.Columns["发票金额"].AppearanceCell.BackColor = Color.Aqua;
                //txt_jechazhi.BackColor = Color.Red;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gridView2_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.Caption == "发票金额")
                {
                    txt_fapiaozje.Text = "0.000000";
                    decimal TotalJe = 0;
                    foreach (DataRow r in dt_发票核销表.Rows)
                    {
                        if (r.RowState == DataRowState.Deleted) continue;

                        if (r["发票金额"].ToString() != "")
                        {
                           // r["未税发票金额"] = (Convert.ToDecimal(r["发票金额"]) / (1 + Convert.ToDecimal(txt_cgshuilv.Text) / 100)).ToString("0.000000");
                           // r["税金"] = (Convert.ToDecimal(r["发票金额"]) - Convert.ToDecimal(r["未税发票金额"])).ToString("0.000000");
                            TotalJe = TotalJe + Convert.ToDecimal(r["发票金额"]);
                        }
                    }
                    txt_fapiaozje.Text = TotalJe.ToString("0.000000");
                    txt_jechazhi.Text = (Convert.ToDecimal(drm["总金额"]) - TotalJe).ToString();
              
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView2.CloseEditor();
                this.BindingContext[dt_发票核销表].EndCurrentEdit();

                if (dt_发票核销表 == null || dt_发票核销表.Rows.Count <= 0) return;
                if (System.Math.Abs(Convert.ToDouble(txt_jechazhi.Text)) > 1)
                    throw new Exception(string.Format("开票通知单\"{0}\"的税后金额小于发票总额了,不允许进行发票确认！", drm["模具开票通知号"].ToString()));
                fun_check发票();
                fun_check发票确认();
                fun_save发票();
                MessageBox.Show("发票确认成功！");
                gridView2.OptionsBehavior.Editable = false;
                barLargeButtonItem6.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                //barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                //txt_riqitime.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
      
    }
}
