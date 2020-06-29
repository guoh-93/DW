using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;

namespace ERPStock
{
    public partial class ui调拨申请查询 : UserControl
    {

        string strconn = CPublic.Var.strConn;
        DataTable dtM = new DataTable();

        public ui调拨申请查询()
        {
            InitializeComponent();
        }
        string cfgfilepath = "";
        private void ui调拨查询_Load(object sender, EventArgs e)
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
                x.UserLayout(this, this.Name, cfgfilepath);

                DateTime t = CPublic.Var.getDatetime().Date;
                barEditItem2.EditValue = t.AddDays(1).AddSeconds(-1);
                barEditItem1.EditValue = t.AddDays(-14);
       
                fun_载入();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_载入()
        {
            try
            {
                if (dtM != null)
                {
                    dtM.Clear();
                }
                string sql = "select * from 调拨申请主表 where 1=1 and ";


                if (barEditItem1.EditValue != null && barEditItem2.EditValue != null && barEditItem1.EditValue.ToString() != "" && barEditItem2.EditValue.ToString() != "")
                {
                    sql += " 申请日期 >= '" + ((DateTime)barEditItem1.EditValue) + "'" + " and 申请日期 <= '" + ((DateTime)barEditItem2.EditValue).AddDays(1).AddSeconds(-1) + "'";
                }
                
                
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                gcM.DataSource = dtM;
            }
            catch (Exception ex)
            {
                // CZMaster.MasterLog.WriteLog(ex.Message, "退货申请主表_刷新操作");
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_mx()
        {
            DataRow fr = gvM.GetDataRow(gvM.FocusedRowHandle);
            string s = string.Format(@"select mx.*,base.物料名称,base.规格型号,kc.库存总数 from 调拨申请明细表 mx Left join 基础数据物料信息表 base on  mx.物料编码=base.物料编码   
                                      left  join 仓库物料数量表 kc on kc.物料编码=mx.物料编码 
                                        where 调拨申请单号='{0}' and kc.仓库号=mx.原仓库号",fr["调拨申请单号"]);
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);

            gcP.DataSource = t;


        }
        private void gvM_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gvP_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gvM_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                fun_mx();



                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gcM, new Point(e.X, e.Y));
                    gvM.CloseEditor();
                    this.BindingContext[dtM].EndCurrentEdit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                //这边最好重新从数据库查一下,因为可能界面缓存
                string sql = string.Format("select  * from 调拨申请主表 where 调拨申请单号='{0}' ", dr["调拨申请单号"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (temp.Rows.Count == 1)
                {
                    dr = temp.Rows[0];
                }
                else  
                {
                    throw new Exception("当前单号出现问题,请确认");
                }
                if (dr["作废"].Equals(true))
                {
                    throw new Exception("该记录已作废,不可修改");
                }
                else if (dr["审核"].Equals(true))
                {
                    MessageBox.Show("该记录已审核不能修改");
                }
                else
                {
                    ui调拨 ui = new ui调拨(dr);
                    CPublic.UIcontrol.Showpage(ui, "申请明细");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_载入();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("该调拨单是否确认作废？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                    string sql = string.Format("select  * from 调拨申请主表 where 调拨申请单号='{0}' ", dr["调拨申请单号"]);
                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (temp.Rows.Count == 1)
                    {
                        dr = temp.Rows[0];
                    }
                    else
                    {
                        throw new Exception("当前单号出现问题,请确认");
                    }
                    if (dr["作废"].Equals(true))
                    {
                        throw new Exception("该记录已作废,无需作废");
                    }
                    else if (dr["审核"].Equals(true))
                    {
                        MessageBox.Show("该记录已审核,不能作废");
                    }
                    else //撤销审核 只需要删除 单据审核申请表中的 
                    {
                        string s1 = string.Format("select  * from 单据审核申请表 where 关联单号='{0}' and 作废=0", dr["调拨申请单号"].ToString());
                        SqlDataAdapter da = new SqlDataAdapter(s1, strconn);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count>0)
                        {
                            dt.Rows[0].Delete();
                        }
                      
                        temp.Rows[0]["作废"] = 1;
                        temp.Rows[0]["作废人"] = CPublic.Var.localUserName;
                        temp.Rows[0]["作废日期"] = DateTime.Now.ToString();
                        try
                        {

                            sql = "select * from 单据审核申请表 where 1<>1";
                            da = new SqlDataAdapter(sql, strconn);
                            new SqlCommandBuilder(da);
                            da.Update(dt);
                            sql = "select * from 调拨申请主表 where 1<>1";
                            da = new SqlDataAdapter(sql, strconn);
                            new SqlCommandBuilder(da);
                            da.Update(temp);
                        }
                        catch
                        {
                            throw new Exception("作废失败,刷新后重试");
                        }
                        MessageBox.Show("已作废");
                        fun_载入();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 撤销审核ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("该调拨单是否确认作废？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                    //这边最好重新从数据库查一下,因为可能界面缓存
                    string sql = string.Format("select  * from 调拨申请主表 where 调拨申请单号='{0}' ", dr["调拨申请单号"]);
                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (temp.Rows.Count == 1)
                    {
                        dr = temp.Rows[0];
                    }
                    else
                    {
                        throw new Exception("当前单号出现问题,请确认");
                    }

                    if (dr["作废"].Equals(true))
                    {
                        throw new Exception("该记录已作废,不可修改");
                    }
                    else if (dr["审核"].Equals(true))
                    {
                        MessageBox.Show("该记录已审核不能修改");
                    }
                    else //撤销审核 只需要删除 单据审核申请表中的 
                    {
                        string s1 = string.Format("select  * from 单据审核申请表 where 关联单号='{0}'", dr["调拨申请单号"].ToString());
                        SqlDataAdapter da = new SqlDataAdapter(s1, strconn);

                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        string s = string.Format("select  * from 单据审核日志表 where 审核申请单号='{0}' ", dt.Rows[0]["审核申请单号"].ToString());
                        DataTable dt_history = CZMaster.MasterSQL.Get_DataTable(s, strconn);


                        int count = dt_history.Rows.Count;
                        for (int i = count - 1; i >= 0; i--)
                        {
                            dt_history.Rows[i].Delete();
                        }
                        dt.Rows[0].Delete();
                        //SqlConnection conn = new SqlConnection(strconn);
                        //conn.Open();
                        //SqlTransaction ts = conn.BeginTransaction("cxdb"); //事务的名称
                        try
                        {

                            sql = "select * from 单据审核申请表 where 1<>1";
                            da = new SqlDataAdapter(sql, strconn);
                            new SqlCommandBuilder(da);
                            da.Update(dt);
                            sql = "select * from 单据审核日志表 where 1<>1";
                            da = new SqlDataAdapter(sql, strconn);
                            new SqlCommandBuilder(da);
                            da.Update(dt_history);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("撤销失败,刷新后重试");
                        }
                        MessageBox.Show("已撤销提交");

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow drM = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
            DataTable dtm = (DataTable)this.gcP.DataSource;
            Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
            Type outerForm = outerAsm.GetType("ERPreport.调拨单", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统

            object[] drr = new object[2];

            drr[0] = drM;
            drr[1] = dtm;
            //   drr[2] = dr["出入库申请单号"].ToString();
            Form ui = Activator.CreateInstance(outerForm, drr) as Form;
            //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
            ui.ShowDialog();
        }
    }
}
