using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPproduct
{
    public partial class 包装抽检确认 : UserControl
    {
        public 包装抽检确认()
        {
            InitializeComponent();
        }
        DataTable dt_ok, dt_mx;
        DataTable dt_bc;

        DataTable dt_借出主, dt_借出mx;
        /// /确认还是取消
        /// </summary>
        string strcon = CPublic.Var.strConn;
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        

        }

   
  
  

        private void 包装抽检确认_Load(object sender, EventArgs e)
        {

            try 

            {
                comboBox1.Text = "未确认";

                date_前.EditValue = Convert.ToDateTime(System.DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd"));
                date_后.EditValue = Convert.ToDateTime(System.DateTime.Today.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
                button1_Click(null, null);

                comboBox2.Text = "未确认";

                dateEdit1.EditValue = Convert.ToDateTime(System.DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd"));
                dateEdit2.EditValue = Convert.ToDateTime(System.DateTime.Today.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd"));

                button2_Click(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }

        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try

            {
                if (e != null && e.Button == MouseButtons.Right)
                {

                    contextMenuStrip1.Show(gridControl2, new System.Drawing.Point(e.X, e.Y));

                }




                DataRow drM = (this.BindingContext[gridControl2.DataSource].Current as DataRowView).Row;







                string sql = string.Format("select * from 销售记录销售出库通知单明细表 where 出库通知单号='{0}' and 作废=0  and 出库数量>0", drM["出库通知单号"]);
                dt_mx = new DataTable();
                dt_mx = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
             //   dt_mx.Columns.Add("有无文件", typeof(bool));
                dt_mx.Columns.Add("包装方式", typeof(string));
                foreach (DataRow dr in dt_mx.Rows)
                {
                    string sq = string.Format("select * from 销售记录销售订单明细表 where 销售订单明细号='{0}'", dr["销售订单明细号"]);
                    DataRow d22r = CZMaster.MasterSQL.Get_DataRow(sq, strcon);

                    dr["包装方式"] = d22r["包装方式"];

                    //sql = string.Format("select * from 包装抽检相关文件上传 where 出库通知单明细号='{0}'", drM["出库通知单明细号"].ToString());

                    //DataTable dt_wj = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    //if (dt_wj.Rows.Count > 0)
                    //{
                    //    dr["有无文件"] = true;
                    //}
                    //else
                    //{
                    //    dr["有无文件"] = false;
                    //}




                }









                gridControl1.DataSource = dt_mx;

                DataColumn dc = new DataColumn("选择", typeof(bool));
                dc.DefaultValue = false;
                dt_mx.Columns.Add(dc);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认保存吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {


                    if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
                    {



                        gridView2.CloseEditor();
                        this.BindingContext[dt_ok].EndCurrentEdit();
                        // this.BindingContext[dtM].EndCurrentEdit();

                        DataView dv = new DataView(dt_ok);
                        DateTime t = CPublic.Var.getDatetime();

                        dv.RowFilter = "选择 = True";
                        DataTable dt_cun = new DataTable();
                        dt_cun = dv.ToTable();
                        if (dt_cun.Rows.Count <= 0)
                        {
                            throw new Exception("请选择数据");
                        }
                        //foreach (DataRow dr in dt_cun.Rows)
                        //{
                        //    //DataRow[] dr_ok = dt_ok.Select(string.Format("出库通知单号='{0}'", dr["出库通知单号"]));
                        //    //dr_ok[0]["包装确认"] = true;
                        //    //dr_ok[0]["包装确认人员"] = CPublic.Var.localUserName;
                        //    //dr_ok[0]["包装确认人员ID"] = CPublic.Var.LocalUserID;
                        //    //dr_ok[0]["包装确认日期"] = t;
                        //    dr["包装确认"] = true;
                        //    dr["包装确认人员"] = CPublic.Var.localUserName;
                        //    dr["包装确认人员ID"] = CPublic.Var.LocalUserID;
                        //    dr["包装确认日期"] = t;

                        //}


                        fun_主(true, dt_cun);

                        try
                        {   //制令主表
                            SqlConnection conn = new SqlConnection(strcon);
                            conn.Open();
                            SqlTransaction ts = conn.BeginTransaction("保存");
                            try
                            {
                                string sql = "select *  from 销售记录销售出库通知单主表 where 1<>1 ";
                                SqlCommand cmm = new SqlCommand(sql, conn, ts);
                                SqlDataAdapter da = new SqlDataAdapter(cmm);
                                new SqlCommandBuilder(da);
                                da.Update(dt_bc);
                                //制令明细表
                                sql = "select * from 销售记录销售出库通知单明细表 where 1<>1";
                                cmm = new SqlCommand(sql, conn, ts);

                                da = new SqlDataAdapter(cmm);
                                new SqlCommandBuilder(da);
                                da.Update(dt_mx);
                                ts.Commit();

                                MessageBox.Show("确认成功");
                                button1_Click(null, null);

                            }
                            catch (Exception ex)
                            {
                                ts.Rollback();
                                throw new Exception(ex.Message);
                            }

                        }
                        catch (Exception ex)
                        {
                            CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_SaveData");
                            throw new Exception(ex.Message);
                        }


                    }
                    else
                    {







                        gridView3.CloseEditor();
                        this.BindingContext[dt_借出主].EndCurrentEdit();
                        // this.BindingContext[dtM].EndCurrentEdit();

                        DataView dv = new DataView(dt_借出主);
                        DateTime t = CPublic.Var.getDatetime();

                        dv.RowFilter = "选择 = True";
                        DataTable dt_cun = new DataTable();
                        dt_cun = dv.ToTable();
                        dt_cun.AcceptChanges();

                        foreach (DataRow dr in dt_cun.Rows)
                        {
                            //DataRow[] dr_ok = dt_ok.Select(string.Format("出库通知单号='{0}'", dr["出库通知单号"]));
                            //dr_ok[0]["包装确认"] = true;
                            //dr_ok[0]["包装确认人员"] = CPublic.Var.localUserName;
                            //dr_ok[0]["包装确认人员ID"] = CPublic.Var.LocalUserID;
                            //dr_ok[0]["包装确认日期"] = t;
                            dr["包装抽检"] = true;
                            dr["包装抽检人员"] = CPublic.Var.localUserName;
                            dr["包装抽检人员ID"] = CPublic.Var.LocalUserID;
                            dr["包装抽检日期"] = t;

                        }

                        fun_借出主(true, dt_cun);
                        //   fun_主(true, dt_cun);

                        try
                        {   //制令主表
                            SqlConnection conn = new SqlConnection(strcon);
                            conn.Open();
                            SqlTransaction ts = conn.BeginTransaction("保存");
                            try
                            {
                                string sql = "select *  from 借还申请表 where 1<>1 ";
                                SqlCommand cmm = new SqlCommand(sql, conn, ts);
                                SqlDataAdapter da = new SqlDataAdapter(cmm);
                                new SqlCommandBuilder(da);
                                da.Update(dt_cun);
                                //制令明细表
                                sql = "select * from 借还申请表附表 where 1<>1";
                                cmm = new SqlCommand(sql, conn, ts);

                                da = new SqlDataAdapter(cmm);
                                new SqlCommandBuilder(da);
                                da.Update(dt_借出mx);
                                ts.Commit();

                                MessageBox.Show("确认成功");
                                button2_Click(null, null);

                            }
                            catch (Exception ex)
                            {
                                ts.Rollback();
                                throw new Exception(ex.Message);
                            }

                        }
                        catch (Exception ex)
                        {
                            CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_SaveData");
                            throw new Exception(ex.Message);
                        }


                    }

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void barLargeButtonItem4_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try

            {
                DateTime t1 = Convert.ToDateTime(barEditItem1.EditValue).Date;
                DateTime t2 = Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1);
                //string sql = string.Format("select * from  销售记录销售出库通知单主表  where  生效日期>'{0}' and 生效日期<'{1}'  and 作废=0    and 包装确认=1 and 完成=1   ", t1, t2);

                string sql = string.Format(@"select tzzb.*,a.销售订单号,销售备注,客户订单号,a.总明细数,b.完成数,szb.部门编号,szb.客户订单号  from 销售记录销售出库通知单主表 tzzb
             left join(select 出库通知单号, left(销售订单明细号, 14) as 销售订单号, COUNT(*)总明细数 from 销售记录销售出库通知单明细表  where 作废 = 0 group by 出库通知单号, left(销售订单明细号, 14))a
             on tzzb.出库通知单号 = a.出库通知单号
                left  join(select 出库通知单号, COUNT(*)完成数 from 销售记录销售出库通知单明细表 where 作废 = 0 and 完成 = 1   group by 出库通知单号)b
                on b.出库通知单号 = tzzb.出库通知单号
               left  join  销售记录销售订单主表 szb on szb.销售订单号 = a.销售订单号   where tzzb.生效日期>'{0}' and tzzb.生效日期<'{1}' and tzzb.作废=0   and  tzzb.包装确认=1   and tzzb.审核=1   ", t1, t2);






                if (barEditItem3.EditValue.ToString() != "")
                {
                    if (barEditItem3.EditValue.ToString() == "全部")
                    {

                    }

                    if (barEditItem3.EditValue.ToString() == "已确认")
                    {

                        sql = sql + string.Format(" and 包装抽检=1");
                    }

                    if (barEditItem3.EditValue.ToString() == "未确认")
                    {

                        sql = sql + string.Format(" and 包装抽检=0");
                    }


                    sql = sql + string.Format(" order by 出库通知单号");
                }


                dt_ok = new DataTable();
                dt_ok = CZMaster.MasterSQL.Get_DataTable(sql, strcon);


                dt_ok.Columns.Add("已出库", typeof(bool));
                dt_ok.Columns.Add("有无文件", typeof(bool));
                dt_ok.Columns.Add("选择", typeof(bool));

                foreach (DataRow dr in dt_ok.Rows)
                {
                    if (dr["总明细数"] == DBNull.Value)
                    {
                        dr["总明细数"] = 0;
                    }
                    if (dr["完成数"] == DBNull.Value)
                    {
                        dr["完成数"] = 0;
                    }
                    if (Convert.ToInt32(dr["总明细数"]) > Convert.ToInt32(dr["完成数"]))
                    {

                        dr["已出库"] = false;

                    }
                    else
                    {
                        dr["已出库"] = true;
                    }

                    sql = string.Format("select * from 包装抽检相关文件上传 where 出库通知单号='{0}'", dr["出库通知单号"].ToString());

                    DataTable dt_wj = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    if (dt_wj.Rows.Count > 0)
                    {
                        dr["有无文件"] = true;
                    }
                    else
                    {
                        dr["有无文件"] = false;
                    }




                }



                //DataView dv = new DataView(dtM);



                //dv.RowFilter = "已出库 = True";
                //dt_ok = new DataTable();
                //dt_ok = dv.ToTable();


                gridControl2.DataSource = dt_ok;
                gridControl1.DataSource = null;
             



              




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                if (MessageBox.Show("确认保存吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {


                    if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
                    {

                        gridView2.CloseEditor();
                        this.BindingContext[dt_ok].EndCurrentEdit();
                        // this.BindingContext[dtM].EndCurrentEdit();
                        DataView dv = new DataView(dt_ok);
                        DateTime t = CPublic.Var.getDatetime();

                        dv.RowFilter = "选择 = True";
                        DataTable dt_cun = new DataTable();
                        dt_cun = dv.ToTable();
                        if (dt_cun.Rows.Count <= 0)
                        {
                            throw new Exception("请选择数据");
                        }
                        fun_主(false, dt_cun);


                        SqlConnection conn = new SqlConnection(strcon);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("保存");
                        try
                        {
                            string sql = "select *  from 销售记录销售出库通知单主表 where 1<>1 ";
                            SqlCommand cmm = new SqlCommand(sql, conn, ts);
                            SqlDataAdapter da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(dt_bc);
                            //制令明细表
                            sql = "select * from 销售记录销售出库通知单明细表 where 1<>1";
                            cmm = new SqlCommand(sql, conn, ts);
                            da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(dt_mx);
                            ts.Commit();

                            MessageBox.Show("确认成功");
                            button1_Click(null, null);
                        }
                        catch (Exception ex)
                        {
                            ts.Rollback();
                            throw new Exception(ex.Message);
                        }


                    }



                    else
                    {


                        gridView3.CloseEditor();
                        this.BindingContext[dt_借出主].EndCurrentEdit();
                        // this.BindingContext[dtM].EndCurrentEdit();

                        DataView dv = new DataView(dt_借出主);
                        DateTime t = CPublic.Var.getDatetime();

                        dv.RowFilter = "选择 = True";
                        DataTable dt_cun = new DataTable();
                        dt_cun = dv.ToTable();
                        dt_cun.AcceptChanges();

                        foreach (DataRow dr in dt_cun.Rows)
                        {
                            //DataRow[] dr_ok = dt_ok.Select(string.Format("出库通知单号='{0}'", dr["出库通知单号"]));
                            //dr_ok[0]["包装确认"] = true;
                            //dr_ok[0]["包装确认人员"] = CPublic.Var.localUserName;
                            //dr_ok[0]["包装确认人员ID"] = CPublic.Var.LocalUserID;
                            //dr_ok[0]["包装确认日期"] = t;
                            dr["包装抽检"] = false;
                            dr["包装抽检人员"] = "";
                            dr["包装抽检人员ID"] = "";
                            dr["包装抽检日期"] = DBNull.Value;
                        }

                        fun_借出主(false, dt_cun);
                        //   fun_主(true, dt_cun);

                        try
                        {   //制令主表
                            SqlConnection conn = new SqlConnection(strcon);
                            conn.Open();
                            SqlTransaction ts = conn.BeginTransaction("保存");
                            try
                            {
                                string sql = "select *  from 借还申请表 where 1<>1 ";
                                SqlCommand cmm = new SqlCommand(sql, conn, ts);
                                SqlDataAdapter da = new SqlDataAdapter(cmm);
                                new SqlCommandBuilder(da);
                                da.Update(dt_cun);
                                sql = "select * from 借还申请表附表 where 1<>1";
                                cmm = new SqlCommand(sql, conn, ts);

                                da = new SqlDataAdapter(cmm);
                                new SqlCommandBuilder(da);
                                da.Update(dt_借出mx);
                                ts.Commit();

                                MessageBox.Show("取消成功");
                                button2_Click(null, null);

                            }
                            catch (Exception ex)
                            {
                                ts.Rollback();
                                throw new Exception(ex.Message);
                            }

                        }
                        catch (Exception ex)
                        {
                            CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_SaveData");
                            throw new Exception(ex.Message);
                        }






                    }





                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }

        private void gridView2_CustomDrawRowIndicator_1(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {

            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {

            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

  















        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {

                if (MessageBox.Show("确认取消吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
                    {
                        gridView2.CloseEditor();
                        this.BindingContext[dt_mx].EndCurrentEdit();
                        // this.BindingContext[dtM].EndCurrentEdit();
                        fun_bc(false);

                        try
                        {   //制令主表
                            SqlConnection conn = new SqlConnection(strcon);
                            conn.Open();
                            SqlTransaction ts = conn.BeginTransaction("保存");
                            try
                            {
                                string sql = "select *  from 销售记录销售出库通知单主表 where 1<>1 ";
                                SqlCommand cmm = new SqlCommand(sql, conn, ts);
                                SqlDataAdapter da = new SqlDataAdapter(cmm);
                                new SqlCommandBuilder(da);
                                da.Update(dt_bc);
                                //制令明细表
                                sql = "select * from 销售记录销售出库通知单明细表 where 1<>1";
                                cmm = new SqlCommand(sql, conn, ts);

                                da = new SqlDataAdapter(cmm);
                                new SqlCommandBuilder(da);
                                da.Update(dt_mx);
                                ts.Commit();

                                MessageBox.Show("确认成功");
                                dt_ok.AcceptChanges();

                                button1_Click(null, null);


                            }
                            catch (Exception ex)
                            {
                                ts.Rollback();
                                throw new Exception(ex.Message);
                            }

                        }
                        catch (Exception ex)
                        {
                            CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_SaveData");
                            throw new Exception(ex.Message);
                        }

                    }
                    else
                    {


                        gridView4.CloseEditor();
                        this.BindingContext[dt_借出mx].EndCurrentEdit();
                        // this.BindingContext[dtM].EndCurrentEdit();
                        fun_借出bc(false);
                        try
                        {   //制令主表
                            SqlConnection conn = new SqlConnection(strcon);
                            conn.Open();
                            SqlTransaction ts = conn.BeginTransaction("保存");
                            try
                            {
                                string sql = "select *  from 借还申请表 where 1<>1 ";
                                SqlCommand cmm = new SqlCommand(sql, conn, ts);
                                SqlDataAdapter da = new SqlDataAdapter(cmm);
                                new SqlCommandBuilder(da);
                                da.Update(dt_bc);
                                //制令明细表
                                sql = "select * from 借还申请表附表 where 1<>1";
                                cmm = new SqlCommand(sql, conn, ts);

                                da = new SqlDataAdapter(cmm);
                                new SqlCommandBuilder(da);
                                da.Update(dt_借出mx);
                                ts.Commit();

                                MessageBox.Show("确认成功");
                                ///  dt_bc.AcceptChanges();

                                button2_Click(null, null);


                            }
                            catch (Exception ex)
                            {
                                ts.Rollback();
                                throw new Exception(ex.Message);
                            }

                        }
                        catch (Exception ex)
                        {
                            CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_SaveData");
                            throw new Exception(ex.Message);
                        }








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
            try
            {

                if (MessageBox.Show("确认保存吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
                    {
                        gridView1.CloseEditor();
                        this.BindingContext[dt_mx].EndCurrentEdit();
                        // this.BindingContext[dtM].EndCurrentEdit();
                        fun_bc(true);
                        try
                        {   //制令主表
                            SqlConnection conn = new SqlConnection(strcon);
                            conn.Open();
                            SqlTransaction ts = conn.BeginTransaction("保存");
                            try
                            {
                                string sql = "select *  from 销售记录销售出库通知单主表 where 1<>1 ";
                                SqlCommand cmm = new SqlCommand(sql, conn, ts);
                                SqlDataAdapter da = new SqlDataAdapter(cmm);
                                new SqlCommandBuilder(da);
                                da.Update(dt_bc);
                                //制令明细表
                                sql = "select * from 销售记录销售出库通知单明细表 where 1<>1";
                                cmm = new SqlCommand(sql, conn, ts);

                                da = new SqlDataAdapter(cmm);
                                new SqlCommandBuilder(da);
                                da.Update(dt_mx);
                                ts.Commit();

                                MessageBox.Show("确认成功");
                                dt_ok.AcceptChanges();

                                button1_Click(null, null);


                            }
                            catch (Exception ex)
                            {
                                ts.Rollback();
                                throw new Exception(ex.Message);
                            }

                        }
                        catch (Exception ex)
                        {
                            CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_SaveData");
                            throw new Exception(ex.Message);
                        }

                    }
                    else
                    {
                        gridView4.CloseEditor();
                        this.BindingContext[dt_借出mx].EndCurrentEdit();
                        // this.BindingContext[dtM].EndCurrentEdit();
                        fun_借出bc(true);
                        try
                        {   //制令主表
                            SqlConnection conn = new SqlConnection(strcon);
                            conn.Open();
                            SqlTransaction ts = conn.BeginTransaction("保存");
                            try
                            {
                                string sql = "select *  from 借还申请表 where 1<>1 ";
                                SqlCommand cmm = new SqlCommand(sql, conn, ts);
                                SqlDataAdapter da = new SqlDataAdapter(cmm);
                                new SqlCommandBuilder(da);
                                da.Update(dt_bc);
                                //制令明细表
                                sql = "select * from 借还申请表附表 where 1<>1";
                                cmm = new SqlCommand(sql, conn, ts);

                                da = new SqlDataAdapter(cmm);
                                new SqlCommandBuilder(da);
                                da.Update(dt_借出mx);
                                ts.Commit();

                                MessageBox.Show("确认成功");
                                //dt_ok.AcceptChanges();

                                button2_Click(null, null);


                            }
                            catch (Exception ex)
                            {
                                ts.Rollback();
                                throw new Exception(ex.Message);
                            }

                        }
                        catch (Exception ex)
                        {
                            CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_SaveData");
                            throw new Exception(ex.Message);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

     
        private void button1_Click(object sender, EventArgs e)
        {
            try

            {
                DateTime t1 = Convert.ToDateTime(date_前.EditValue).Date;
                DateTime t2 = Convert.ToDateTime(date_后.EditValue).Date.AddDays(1).AddSeconds(-1);
                //       string sql = string.Format("select * from  销售记录销售出库通知单主表  where  生效日期>'{0}' and 生效日期<'{1}'  and 作废=0   and 完成=1   ", t1, t2);

                string sql = "";

                sql = string.Format(@"select tzzb.*,a.销售订单号,销售备注,客户订单号,a.总明细数,b.完成数,szb.部门编号,szb.客户订单号  from 销售记录销售出库通知单主表 tzzb
             left join(select 出库通知单号, left(销售订单明细号, 14) as 销售订单号, COUNT(*)总明细数 from 销售记录销售出库通知单明细表  where 作废 = 0 group by 出库通知单号, left(销售订单明细号, 14))a
             on tzzb.出库通知单号 = a.出库通知单号
                left  join(select 出库通知单号, COUNT(*)完成数 from 销售记录销售出库通知单明细表 where 作废 = 0 and 完成 = 1   group by 出库通知单号)b
                on b.出库通知单号 = tzzb.出库通知单号
               left  join  销售记录销售订单主表 szb on szb.销售订单号 = a.销售订单号   where tzzb.生效日期>'{0}' and tzzb.生效日期<'{1}' and tzzb.作废=0    and tzzb.审核=1   and tzzb.包装确认=1     ", t1, t2);

                //@"select stcmx.*,(kc.库存总数) as 仓库数量 from 销售记录销售出库通知单明细表 stcmx
                //left join 仓库物料数量表 kc on kc.物料编码 = stcmx.物料编码 
                //left join 销售记录销售订单明细表 smx on smx.销售订单明细号=stcmx.销售订单明细号
                //where kc.仓库号=smx.仓库号 and  stcmx.出库通知单号 = '{0}'"

                if (comboBox1.Text.ToString() != "")
                {
                    if (comboBox1.Text.ToString() == "全部")
                    {

                    }

                    if (comboBox1.Text.ToString() == "已确认")
                    {

                        sql = sql + string.Format(" and tzzb.包装抽检=1");
                    }

                    if (comboBox1.Text.ToString() == "未确认")
                    {

                        sql = sql + string.Format(" and tzzb.包装抽检=0");
                    }

                    sql = sql + string.Format(" order by tzzb.出库通知单号 ");

                }



                dt_ok = new DataTable();
                dt_ok = CZMaster.MasterSQL.Get_DataTable(sql, strcon);


                dt_ok.Columns.Add("已出库", typeof(bool));

                dt_ok.Columns.Add("有无文件", typeof(bool));

                foreach (DataRow dr in dt_ok.Rows)
                {
                    if (dr["总明细数"] == DBNull.Value)
                    {
                        dr["总明细数"] = 0;
                    }
                    if (dr["完成数"] == DBNull.Value)
                    {
                        dr["完成数"] = 0;
                    }
                    if (Convert.ToInt32(dr["总明细数"]) > Convert.ToInt32(dr["完成数"]))
                    {

                        dr["已出库"] = false;

                    }
                    else
                    {
                        dr["已出库"] = true;
                    }



                    sql = string.Format("select * from 包装抽检相关文件上传 where 出库通知单号='{0}'", dr["出库通知单号"].ToString());

                    DataTable dt_wj = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    if (dt_wj.Rows.Count > 0)
                    {
                        dr["有无文件"] = true;
                    }
                    else
                    {
                        dr["有无文件"] = false;
                    }






                }








                gridControl2.DataSource = dt_ok;
                gridControl1.DataSource = null;
                dt_ok.Columns.Add("选择", typeof(bool));


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 相关文件查询ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

            if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
            {
                DataRow drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;

                Form包装抽检相关文件上传 a = new Form包装抽检相关文件上传(drM["出库通知单号"].ToString());


                a.ShowDialog();

            }
            else
            {
                DataRow drM = (this.BindingContext[gridControl3.DataSource].Current as DataRowView).Row;


         

                Form包装抽检相关文件上传 a = new Form包装抽检相关文件上传(drM["申请批号"].ToString());


                a.ShowDialog();
            }

           
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
         
        }














        private void fun_bc(bool a)
        {

            gridView1.CloseEditor();
            this.BindingContext[dt_mx].EndCurrentEdit();


            DateTime t = CPublic.Var.getDatetime();
            int wc = 0;
            if (a == true)
            {

                foreach (DataRow dr in dt_mx.Rows)
                {
                    if (bool.Parse(dr["选择"].ToString()) == true)
                    {
                        //    DataRow[] dr_ok = dt_bc.Select(string.Format("出库通知单号='{0}'", dr["出库通知单号"]));
                        dr["包装抽检"] = true;
                        dr["包装抽检人员"] = CPublic.Var.localUserName;
                        dr["包装抽检人员ID"] = CPublic.Var.LocalUserID;
                        dr["包装抽检日期"] = t;

                    }

                    if (bool.Parse(dr["包装抽检"].ToString()) == true)
                    {
                        wc++;
                    }

                }

            }
            else
            {

                foreach (DataRow dr in dt_mx.Rows)
                {
                    if (bool.Parse(dr["选择"].ToString()) == true)
                    {
                        //    DataRow[] dr_ok = dt_bc.Select(string.Format("出库通知单号='{0}'", dr["出库通知单号"]));
                        dr["包装抽检"] = false;
                        dr["包装抽检人员"] = "";
                        dr["包装抽检人员ID"] = "";
                        dr["包装抽检日期"] = DBNull.Value;
                    }
                }

            }



            string sql22 = string.Format("select * from  销售记录销售出库通知单主表 where  作废=0    and 出库通知单号='{0}' ", dt_mx.Rows[0]["出库通知单号"].ToString());

            dt_bc = new DataTable();
            dt_bc = CZMaster.MasterSQL.Get_DataTable(sql22, strcon);


            if (wc == dt_mx.Rows.Count)
            {

                DataRow[] dr_ok = dt_bc.Select(string.Format("出库通知单号='{0}'", dt_mx.Rows[0]["出库通知单号"]));
                dr_ok[0]["包装抽检"] = true;
                dr_ok[0]["包装抽检人员"] = CPublic.Var.localUserName;
                dr_ok[0]["包装抽检人员ID"] = CPublic.Var.LocalUserID;
                dr_ok[0]["包装抽检日期"] = t;
            }
            else
            {

                DataRow[] dr_ok = dt_bc.Select(string.Format("出库通知单号='{0}'", dt_mx.Rows[0]["出库通知单号"]));
                dr_ok[0]["包装抽检"] = false;
                dr_ok[0]["包装抽检人员"] = "";
                dr_ok[0]["包装抽检人员ID"] = "";
                dr_ok[0]["包装抽检日期"] = DBNull.Value;


            }


        }

        private void fun_主(bool a, DataTable dt)
        {
            DateTime t = CPublic.Var.getDatetime();
            List<string> strList = new List<string>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow drrrr = dt.Rows[i];

                strList.Add(dt.Rows[i]["出库通知单号"].ToString());//循环添加元素


            }
            string[] st_出库通知单号 = strList.ToArray();
            string sql_补 = "";
            for (int i = 0; i < st_出库通知单号.Length; i++)
            {
                string cai = st_出库通知单号[i].ToString();

                if (i == 0)
                {
                    sql_补 = sql_补 + string.Format("and 出库通知单号= '{0}'", st_出库通知单号[i].ToString());
                }
                else
                {
                    sql_补 = sql_补 + string.Format("or 出库通知单号= '{0}'", st_出库通知单号[i].ToString());
                }
            }
            string sql = string.Format("select * from  销售记录销售出库通知单明细表 where  1=1 {0}", sql_补.ToString());

            dt_mx = new DataTable();
            dt_mx = CZMaster.MasterSQL.Get_DataTable(sql, strcon);


            if (a == true)
            {

                string sql22 = string.Format("select * from  销售记录销售出库通知单主表 where  作废=0  and 包装抽检=0");
                dt_bc = new DataTable();
                dt_bc = CZMaster.MasterSQL.Get_DataTable(sql22, strcon);



                foreach (DataRow dr in dt.Rows)
                {
                    if (bool.Parse(dr["包装抽检"].ToString()) == true)
                    {
                        throw new Exception("勿选择已确认数据");

                    }
                    if (bool.Parse(dr["包装确认"].ToString()) == false)
                    {
                        throw new Exception("当前数据包装未确认");

                    }


                    DataRow[] dr_ok = dt_bc.Select(string.Format("出库通知单号='{0}'", dr["出库通知单号"]));
                    dr_ok[0]["包装抽检"] = true;
                    dr_ok[0]["包装抽检人员"] = CPublic.Var.localUserName;
                    dr_ok[0]["包装抽检人员ID"] = CPublic.Var.LocalUserID;
                    dr_ok[0]["包装抽检日期"] = t;

                }

                foreach (DataRow dr in dt_mx.Rows)
                {

                    if (bool.Parse(dr["包装确认"].ToString()) == false)
                    {
                        throw new Exception("当前数据包装未确认");

                    }
                    if (bool.Parse(dr["包装抽检"].ToString()) == true)
                    { }
                    else
                    {
                        dr["包装抽检"] = true;
                        dr["包装抽检人员"] = CPublic.Var.localUserName;
                        dr["包装抽检人员ID"] = CPublic.Var.LocalUserID;
                        dr["包装抽检日期"] = t;

                    }
                }
            }
            else
            {

                string sql22 = string.Format("select * from  销售记录销售出库通知单主表 where  作废=0  and 包装抽检=1");
                dt_bc = new DataTable();
                dt_bc = CZMaster.MasterSQL.Get_DataTable(sql22, strcon);

                foreach (DataRow dr in dt.Rows)
                {
                    if (bool.Parse(dr["包装抽检"].ToString()) == false)
                    {
                        throw new Exception("勿选择未确认数据");

                    }
                    if (bool.Parse(dr["包装确认"].ToString()) == false)
                    {
                        throw new Exception("当前数据包装未确认");

                    }

                    DataRow[] dr_ok = dt_bc.Select(string.Format("出库通知单号='{0}'", dr["出库通知单号"]));
                    dr_ok[0]["包装抽检"] = false;
                    dr_ok[0]["包装抽检人员"] = "";
                    dr_ok[0]["包装抽检人员ID"] = "";
                    dr_ok[0]["包装抽检日期"] = DBNull.Value;
                }

                foreach (DataRow dr in dt_mx.Rows)
                {
                    if (bool.Parse(dr["包装确认"].ToString()) == false)
                    {
                        throw new Exception("当前数据包装未确认");

                    }
                    dr["包装抽检"] = false;
                    dr["包装抽检人员"] = "";
                    dr["包装抽检人员ID"] = "";
                    dr["包装抽检日期"] = DBNull.Value;

                }
            }
        }

        private void fun_借出bc(bool a)
        {

            gridView4.CloseEditor();
            this.BindingContext[dt_借出mx].EndCurrentEdit();


            DateTime t = CPublic.Var.getDatetime();
            int wc = 0;
            if (a == true)
            {

                foreach (DataRow dr in dt_借出mx.Rows)
                {
                    if (bool.Parse(dr["包装确认"].ToString()) == false)
                    {
                        throw new Exception("当前数据包装未确认");

                    }

                    if (bool.Parse(dr["选择"].ToString()) == true)
                    {
                        //    DataRow[] dr_ok = dt_bc.Select(string.Format("出库通知单号='{0}'", dr["出库通知单号"]));
                        dr["包装抽检"] = true;
                        dr["包装抽检人员"] = CPublic.Var.localUserName;
                        dr["包装抽检人员ID"] = CPublic.Var.LocalUserID;
                        dr["包装抽检日期"] = t;

                    }

                    if (bool.Parse(dr["包装抽检"].ToString()) == true)
                    {
                        wc++;
                    }

                }

            }
            else
            {

                foreach (DataRow dr in dt_借出mx.Rows)
                {
                    if (bool.Parse(dr["包装确认"].ToString()) == false)
                    {
                        throw new Exception("当前数据包装未确认");

                    }
                    if (bool.Parse(dr["选择"].ToString()) == true)
                    {
                        //    DataRow[] dr_ok = dt_bc.Select(string.Format("出库通知单号='{0}'", dr["出库通知单号"]));
                        dr["包装抽检"] = false;
                        dr["包装抽检人员"] = "";
                        dr["包装抽检人员ID"] = "";
                        dr["包装抽检日期"] = DBNull.Value;
                    }
                }

            }



            string sql22 = string.Format("select * from  借还申请表 where  作废=0    and 申请批号='{0}' ", dt_借出mx.Rows[0]["申请批号"].ToString());

            dt_bc = new DataTable();
            dt_bc = CZMaster.MasterSQL.Get_DataTable(sql22, strcon);


            if (wc == dt_借出mx.Rows.Count)
            {

                DataRow[] dr_ok = dt_bc.Select(string.Format("申请批号='{0}'", dt_借出mx.Rows[0]["申请批号"]));
                dr_ok[0]["包装抽检"] = true;
                dr_ok[0]["包装抽检人员"] = CPublic.Var.localUserName;
                dr_ok[0]["包装抽检人员ID"] = CPublic.Var.LocalUserID;
                dr_ok[0]["包装抽检日期"] = t;
            }
            else
            {

                DataRow[] dr_ok = dt_bc.Select(string.Format("申请批号='{0}'", dt_借出mx.Rows[0]["申请批号"]));
                dr_ok[0]["包装抽检"] = false;
                dr_ok[0]["包装抽检人员"] = "";
                dr_ok[0]["包装抽检人员ID"] = "";
                dr_ok[0]["包装抽检日期"] = DBNull.Value;


            }


        }
        private void fun_借出主(bool a, DataTable dt)
        {
            DateTime t = CPublic.Var.getDatetime();
            List<string> strList = new List<string>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow drrrr = dt.Rows[i];

                strList.Add(dt.Rows[i]["申请批号"].ToString());//循环添加元素


            }
            string[] st_申请批号 = strList.ToArray();
            string sql_补 = "";
            for (int i = 0; i < st_申请批号.Length; i++)
            {
                string cai = st_申请批号[i].ToString();

                if (i == 0)
                {
                    sql_补 = sql_补 + string.Format("and 申请批号= '{0}'", st_申请批号[i].ToString());
                }
                else
                {
                    sql_补 = sql_补 + string.Format("or 申请批号= '{0}'", st_申请批号[i].ToString());
                }
            }
            string sql = string.Format("select * from  借还申请表附表 where  1=1 {0}", sql_补.ToString());

            dt_借出mx = new DataTable();
            dt_借出mx = CZMaster.MasterSQL.Get_DataTable(sql, strcon);


            if (a == true)
            {

                //string sql22 = string.Format("select * from  销售记录销售出库通知单主表 where  作废=0  and 包装抽检=0");
                //dt_bc = new DataTable();
                //dt_bc = CZMaster.MasterSQL.Get_DataTable(sql22, strcon);



                foreach (DataRow dr in dt_借出mx.Rows)
                {
                    if (bool.Parse(dr["包装确认"].ToString()) == false)
                    {
                        throw new Exception("当前数据包装未确认");

                    }
                    if (bool.Parse(dr["包装抽检"].ToString()) == true)
                    {
                       // dr["包装抽检"] = true;
                    }
                    else
                    {
                        dr["包装抽检"] = true;
                        dr["包装抽检人员"] = CPublic.Var.localUserName;
                        dr["包装抽检人员ID"] = CPublic.Var.LocalUserID;
                        dr["包装抽检日期"] = t;

                    }
                }
            }
            else
            {


                foreach (DataRow dr in dt_借出mx.Rows)
                {
                    if (bool.Parse(dr["包装确认"].ToString()) == false)
                    {
                        throw new Exception("当前数据包装未确认");

                    }
                    dr["包装抽检"] = false;
                    dr["包装抽检人员"] = "";
                    dr["包装抽检人员ID"] = "";
                    dr["包装抽检日期"] = DBNull.Value;

                }
            }
        }

        private void gridView3_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gridView4_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

    
        private void button2_Click(object sender, EventArgs e)
        {
            try

            {
                DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue).Date;
                DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).Date.AddDays(1).AddSeconds(-1);
                //       string sql = string.Format("select * from  销售记录销售出库通知单主表  where  生效日期>'{0}' and 生效日期<'{1}'  and 作废=0   and 完成=1   ", t1, t2);

                string sql = "";

                sql = string.Format(@"select * from  借还申请表   where  审核=1 and  作废=0   and 申请日期>'{0}' and  申请日期<'{1}' and 包装确认=1   ", t1, t2);
                if (comboBox2.Text.ToString() != "")
                {
                    if (comboBox2.Text.ToString() == "全部")
                    {

                    }

                    if (comboBox2.Text.ToString() == "已确认")
                    {
                        sql = sql + string.Format(" and 包装抽检=1");
                    }

                    if (comboBox2.Text.ToString() == "未确认")
                    {

                        sql = sql + string.Format(" and  包装抽检=0");
                    }

                    sql = sql + string.Format(" order by 申请日期 ");

                }








                dt_借出主 = new DataTable();
                dt_借出主 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);



                dt_借出主.Columns.Add("有无文件",typeof(bool));

                foreach (DataRow dr in  dt_借出主.Rows )
                {

                    sql = string.Format("select * from 包装抽检相关文件上传 where 出库通知单号='{0}'", dr["申请批号"].ToString());

                    DataTable dt_wj = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    if (dt_wj.Rows.Count > 0)
                    {
                        dr["有无文件"] = true;
                    }
                    else
                    {
                        dr["有无文件"] = false;
                    }
                }









                gridControl3.DataSource = dt_借出主;
                gridControl4.DataSource = null;
                dt_借出主.Columns.Add("选择", typeof(bool));


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gridView3_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gridControl3, new System.Drawing.Point(e.X, e.Y));
            }




            try

            {
                DataRow drM = (this.BindingContext[gridControl3.DataSource].Current as DataRowView).Row;

                string sql = string.Format("select * from 借还申请表附表 where 申请批号='{0}' and 作废=0  ", drM["申请批号"]);
                dt_借出mx = new DataTable();
                dt_借出mx = CZMaster.MasterSQL.Get_DataTable(sql, strcon);



                gridControl4.DataSource = dt_借出mx;

                DataColumn dc = new DataColumn("选择", typeof(bool));
                dc.DefaultValue = false;
                dt_借出mx.Columns.Add(dc);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }










    }
}
