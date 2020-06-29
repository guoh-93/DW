using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace ERPpurchase
{
    public partial class frm采购记录来料检验详细界面 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        DataRow drM;
        DataTable dtP;
        DataTable dt_待办;
        DataView dv_待办;
        Boolean bl_新增or修改 = false;
        DataTable dt_已通知数量 = null;
        string str_来料检验单号 = "";
        #endregion

        #region 自用类
        public frm采购记录来料检验详细界面()
        {
            InitializeComponent();
            fun_载入();
            bl_新增or修改 = true;
        }
        public frm采购记录来料检验详细界面(DataRow dr)
        {
            InitializeComponent();
            bl_新增or修改 = true;
        }
        public frm采购记录来料检验详细界面(DataRow dr, string s_检验单号)
        {
            InitializeComponent();
            bl_新增or修改 = false;
            drM = dr;
            str_来料检验单号 = s_检验单号;
        }
        private void frm采购记录来料检验详细界面_Load(object sender, EventArgs e)
        {
            devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
            devGridControlCustom1.strConn = CPublic.Var.strConn;
            txt_操作员.Text = CPublic.Var.localUserName;
            txt_操作员ID.Text = CPublic.Var.LocalUserID;
            fun_载入待办();
        }

        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            gv.CloseEditor();
            gc_待办.BindingContext[dt_待办].EndCurrentEdit();         
        }
        #endregion

        #region 待办 方法
        private void fun_载入待办()
        {
            string sql = "select * from 采购记录采购送检单明细表 where 生效 = 1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            dt_待办 = new DataTable();
            //dt_待办.Columns.Add("选择", typeof(Boolean));
            da.Fill(dt_待办);
            //foreach (DataRow r in dt_待办.Rows)
            //{
            //    r["选择"] = false;
            //}
            dv_待办 = new DataView(dt_待办);
            dv_待办.RowFilter = "";
            gc_待办.DataSource = dv_待办;
        }

        private void gv_待办_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gv_待办.GetDataRow(gv_待办.FocusedRowHandle);
                if (dr == null) return;
                txt_产品编号.Text = dr["物料编码"].ToString();
                txt_产品名称.Text = dr["物料名称"].ToString();
                txt_采购入库通知单号.Text = dr["采购单号"].ToString();
                txt_采购明细号.Text = dr["采购单明细号"].ToString();
                txt_供应商编号.Text = dr["供应商ID"].ToString();
                txt_送检数量.Text = dr["送检数量"].ToString();
                txt_检验日期.EditValue = System.DateTime.Now;
                drM["供应商编号"] = dr["供应商ID"];
                drM["送检单号"] = dr["送检单号"];
                drM["送检单明细号"] = dr["送检单明细号"];
                drM["供应商名称"] = dr["供应商"];
                drM["价格核实"] = dr["价格核实"];
                drM["是否急单"] = dr["是否急单"];
                drM["税率"] = dr["税率"];
                drM["未税单价"] = dr["未税单价"];
                drM["单价"] = dr["单价"];
                drM["未税金额"] = dr["未税金额"];
                drM["金额"] = dr["金额"];
                drM["采购数量"] = dr["采购数量"];
                //drM["采购明细号"] = dr[" 采购单明细号"];
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region 方法
        private void fun_载入()
        {
            try
            {
                string sql = "select * from 采购记录采购单检验主表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                dtM = new DataTable();
                da.Fill(dtM);
                drM = dtM.NewRow();
                dtM.Rows.Add(drM);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm采购记录来料检验详细界面_fun_载入");
            }
        }

        private void fun_载入空主表()
        {
            if (bl_新增or修改 == true)
            { }
            else
            {
                dataBindHelper1.DataFormDR(drM);
            }
        }

        private void fun_载入明细()
        {

        }

        private void fun_保存主表()
        {
            try
            {
                if (bl_新增or修改 == true)
                {
                    fun_来料检验单();
                    txt_检验记录单号.Text = str_来料检验单号;
                    drM["GUID"] = System.Guid.NewGuid();
                    //drM["创建日期"] = System.DateTime.Now;
                }
                try
                {
                    drM["操作员"] = CPublic.Var.localUserName;
                    drM["操作员ID"] = CPublic.Var.LocalUserID;
                    drM["修改检验日期"] = System.DateTime.Now;
                    dataBindHelper1.DataToDR(drM);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "来料检验单界面_fun_保存主表");
                throw ex;
            }
        }

        private void fun_保存明细()
        {

        }

        private void fun_已通知数量()
        {
            dt_已通知数量 = new DataTable();
            //foreach (DataRow r in dr_传.Rows)
            //{
            //    string sql = string.Format("select * from *******表 where 销售订单明细号 = '{0}'", r["销售订单明细号"].ToString().Trim());
            //    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            //    da.Fill(dt_已通知数量);
            //    dt_已通知数量.Rows[0]["已通知数量"] = Convert.ToDecimal(dt_已通知数量.Rows[0]["已通知数量"]) + Convert.ToDecimal(r["出库数量"]);
            //    dt_已通知数量.Rows[0]["未通知数量"] = Convert.ToDecimal(dt_已通知数量.Rows[0]["未通知数量"]) - Convert.ToDecimal(r["出库数量"]);
            //}
        }

        private void fun_事务_保存()
        {
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");
            try
            {
                //{
                //    string sql = "select * from 采购记录采购单检验明细表 where 1<>1";
                //    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                //    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                //    {
                //        new SqlCommandBuilder(da);
                //        da.Update(dr_传);
                //    }
                //}
                {
                    string sql = "select * from 采购记录采购单检验主表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dtM);
                    }
                }
                //if (dt_已通知数量 != null)
                //{
                //    string sql = "select * from ** where 1<>1";
                //    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                //    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                //    {
                //        new SqlCommandBuilder(da);
                //        da.Update(dt_已通知数量);
                //    }
                //}
                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw ex;
            }
        }

        private void fun_来料检验单()
        {
            str_来料检验单号 = string.Format("IC{0}{1:D2}{2:D4}", DateTime.Now.Year, DateTime.Now.Month, CPublic.CNo.fun_得到最大流水号("IC", DateTime.Now.Year, DateTime.Now.Month));
        }

        private void fun_清空()
        {
            txt_不合格数量.Text = "";
            txt_采购明细号.Text = "";
            txt_采购入库通知单号.Text = "";
            txt_产品编号.Text = "";
            txt_产品名称.Text = "";
            txt_抽检数量.Text = "";
            txt_供应商编号.Text = "";
            txt_检验记录单号.Text = "";
            txt_检验结果.SelectedIndex = -1;
            txt_检验日期.EditValue = System.DateTime.Now;
            txt_检验员.Text = "";
            txt_检验员ID.Text = "";
            txt_批次数量.Text = "";
            txt_送检人.Text = "";
            txt_送检人ID.Text = "";
            txt_送检数量.Text = "";
            txt_严重.Checked = false;
            txt_已检数量.Text = "";
            txt_操作员.Text = CPublic.Var.localUserName;
            txt_操作员ID.Text = CPublic.Var.LocalUserID;
        }

        private void fun_强载()
        {
            try
            {
                string sql = string.Format("select * from 采购记录采购单检验主表 where 检验记录单号 = '{0}'", str_来料检验单号);
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                drM = dtM.Rows[0];
                dataBindHelper1.DataFormDR(drM);
                //{
                //    string sqll = string.Format("select * from 销售记录销售出库通知单明细表 where 出库通知单号 = '{0}'", str_来料检验单号);
                //    using (SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn))
                //    {
                //        dr_传 = new DataTable();
                //        daa.Fill(dr_传);
                //        gc.DataSource = dr_传;
                //    }
                //}
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "采购记录来料检验详细界面_fun_强载");
            }
        }

        private void fun_生效()
        {
            drM["生效"] = 1;
            //drM["生效日期"] = System.DateTime.Now;
            fun_保存主表();
            //foreach (DataRow r in dr_传.Rows)
            //{
            //    r["生效"] = 1;
            //    r["生效日期"] = System.DateTime.Now;
            //}
            //fun_保存明细();
            //fun_已通知数量();
            fun_事务_保存();
        }
        #endregion

        #region 界面操作
        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
        //新增
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
        //保存
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                try
                {
                    //gv.CloseEditor();
                    //gc.BindingContext[dr_传].EndCurrentEdit();
                    //gv_待办.CloseEditor();
                    //gc_待办.BindingContext[dt_待办].EndCurrentEdit();
                    fun_保存主表();
                    //fun_保存明细();
                    fun_事务_保存();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                fun_强载();
                fun_载入待办();
                //保存完变成修改状态
                bl_新增or修改 = false;
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "来料检验单详细界面_保存");
                MessageBox.Show("保存失败");
            }
        }
        //生效
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_生效();
                fun_清空();
                bl_新增or修改 = true;
                //dr_传.Clear();
                //fun_载入明细();
                fun_载入待办();
                drM = dtM.NewRow();
                dtM.Rows.Add(drM);
                MessageBox.Show("生效成功！");
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "出库通知单_生效");
                MessageBox.Show(string.Format("生效失败！"));
            }
        }
        //关闭
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion

    }
}
