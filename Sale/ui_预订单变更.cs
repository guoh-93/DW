using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace ERPSale
{
    public partial class ui_预订单变更 : UserControl
    {
        DataTable dt_预;
        DataTable dt_mx;
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        string s_申请单号 = "";
        bool bl_新增 = true;


        public ui_预订单变更()
        {
            InitializeComponent();
        }

        public ui_预订单变更(DataTable dt_1, DataTable dt_2)
        {
            InitializeComponent();
            dt_预 = dt_1;
            dt_mx = dt_2;
            dt_mx.AcceptChanges();
            timer1.Start(); 
        }

        private void ui_预订单变更_Load(object sender, EventArgs e)
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
                x.UserLayout(this.panel2, this.Name, cfgfilepath);

                fun_load();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {
            txt_预订单号.Text = dt_预.Rows[0]["销售预订单号"].ToString();
            textBox2.Text = dt_预.Rows[0]["业务员"].ToString();
            txt_录入人员.Text = dt_预.Rows[0]["制单人"].ToString();
            txt_日期.EditValue = Convert.ToDateTime(dt_预.Rows[0]["制单日期"].ToString());
            textBox3.Text = dt_预.Rows[0]["客户编号"].ToString();
            txt_客户名称.Text = dt_预.Rows[0]["客户名称"].ToString();
            textBox1.Text = dt_预.Rows[0]["部门名称"].ToString();
            txt_销售备注.Text = dt_预.Rows[0]["备注"].ToString();
            
            gc.DataSource = dt_mx;
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[dt_mx].EndCurrentEdit();
                this.ActiveControl = null;

                DataView dv = new DataView(dt_mx);
                dv.RowStateFilter = DataViewRowState.ModifiedCurrent;
                DataTable dt = dv.ToTable();

                
                DateTime t = CPublic.Var.getDatetime();

                s_申请单号 = string.Format("DWDC{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                    t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("DWDC", t.Year, t.Month, t.Day).ToString("0000"));

                string sql = "select * from 销售预订单变更申请 where 1<>1";
                DataTable dt_变更申请主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                sql = "select * from 销售预订单变更申请明细 where 1<>1";
                DataTable dt_变更申请明细 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                DataRow dr = dt_变更申请主.NewRow();
                dr["变更申请单号"] = s_申请单号;
                dr["提交审核"] = true;
                dr["提交日期"] = t;
                dr["提交人"] = CPublic.Var.localUserName;
                dr["提交人ID"] = CPublic.Var.LocalUserID;
                dr["部门名称"] = CPublic.Var.localUser部门名称;
                dr["销售备注"] = txt_销售备注.Text;
                dr["销售预订单号"] = txt_预订单号.Text;
                dt_变更申请主.Rows.Add(dr);

                int i = 1;
                foreach(DataRow dr_1 in dt.Rows)
                {
                    if (dr_1["更改数量"].ToString() ==""|| Convert.ToDecimal(dr_1["更改数量"]) < 0)
                    {
                        throw new Exception("目标数量不可为空或负数");
                    }
                    DataRow dr_mx = dt_变更申请明细.NewRow();
                    dt_变更申请明细.Rows.Add(dr_mx);
                    dr_mx["变更申请单号"] = s_申请单号;
                    dr_mx["POS"] = i++;
                    dr_mx["变更申请明细号"] = s_申请单号 + "-" + Convert.ToInt32(dr_mx["POS"]).ToString("00");
                    dr_mx["销售预订单明细号"] = dr_1["销售预订单明细号"];
                    dr_mx["物料编码"] = dr_1["物料编码"];
                    dr_mx["物料名称"] = dr_1["物料名称"];
                    dr_mx["规格型号"] = dr_1["规格型号"];
                    dr_mx["预计发货日期"] =Convert.ToDateTime(dr_1["预计发货日期"]);
                    DataRow []rr= dt_mx.Select($"销售预订单明细号='{dr_1["销售预订单明细号"]}'");
                    dr_mx["数量"] = Convert.ToDecimal(dr_1["转换订单数量"])+ Convert.ToDecimal(dr_1["更改数量"]);
                    dr_mx["变更前数量"] = Convert.ToDecimal(dr_1["数量"]);
                    dr_mx["变更前发货日期"] = Convert.ToDateTime(rr[0]["预计发货日期", DataRowVersion.Original]);
                    dr_mx["变更前备注"] = rr[0]["备注", DataRowVersion.Original];
                    dr_mx["明细备注"] = dr_1["备注"];

                }
                DataTable dt_审核 = ERPorg.Corg.fun_PA("生效", "销售预订单变更申请", s_申请单号, txt_客户名称.Text);

                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("销售预订单变更申请"); //事务的名称
                SqlCommand cmd = new SqlCommand("select * from 销售预订单变更申请 where 1<>1", conn, ts);
                SqlCommand cmd1 = new SqlCommand("select * from 销售预订单变更申请明细 where 1<>1", conn, ts);
                SqlCommand cmd2 = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);

                try
                {
                    SqlDataAdapter da;
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_变更申请主);
                    da = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da);
                    da.Update(dt_变更申请明细);

                    da = new SqlDataAdapter(cmd2);
                    new SqlCommandBuilder(da);
                    da.Update(dt_审核);
                    ts.Commit();
                    MessageBox.Show("已提交审核");
                    bl_新增 = false;
                    barLargeButtonItem1.Enabled = false;
                    barLargeButtonItem3.Enabled = true;


                }
                catch
                {
                    ts.Rollback();
                    throw new Exception("提交出错了,请刷新后重试");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (bl_新增)
                {                     
                    label27.Visible = false;
                }
                else
                {                     
                        string s = string.Format("select * from 销售预订单变更申请 where 变更申请单号='{0}'", s_申请单号);
                        DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                        if (t.Rows.Count > 0)
                        {
                            if (Convert.ToBoolean(t.Rows[0]["审核"]))
                            {           
                                label27.Visible = true;
                                label27.Text = "已审核";
                            }
                            else
                            {    
                                label27.Visible = true;
                                label27.Text =  "审核中";
                            }                        
                        }
                        else
                        {              
                            label27.Visible = false;
                        }
                }
            }
            catch (Exception)
            {


            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string sql = $"select * from 销售预订单变更申请 where 变更申请单号  ='{s_申请单号 }'";
                DataTable dt_变更 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (dt_变更.Rows.Count>0)
                {
                    if (Convert.ToBoolean(dt_变更.Rows[0]["审核"]))
                    {
                        throw new Exception("该单据已审核，不可作废");
                    }
                    dt_变更.Rows[0]["提交审核"] = 0;
                    dt_变更.Rows[0]["作废"] = 1;
                    dt_变更.Rows[0]["提交人"] = "";
                    dt_变更.Rows[0]["提交人ID"] = "";
                    dt_变更.Rows[0]["提交日期"] = DBNull.Value;
                    sql = $"select * from 单据审核申请表 where 关联单号 = '{s_申请单号}'";
                    DataTable dt_审核 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                    if (dt_审核.Rows.Count > 0)
                    {
                        dt_审核.Rows[0].Delete();
                    }
                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("pur"); //事务的名称
                    SqlCommand cmd1 = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);
                    SqlCommand cmd = new SqlCommand("select * from 销售预订单变更申请 where 1<>1", conn, ts);
                    try
                    {

                        SqlDataAdapter  da = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da);
                        da.Update(dt_审核);
                        da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);
                        da.Update(dt_变更);
                         
                        ts.Commit();
                        MessageBox.Show("作废成功");
                        bl_新增 = true;
                        barLargeButtonItem3.Enabled = false;
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        ts.Rollback();
                        throw new Exception("提交出错了,请刷新后重试");
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
