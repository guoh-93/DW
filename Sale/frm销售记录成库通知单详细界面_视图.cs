using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ERPSale
{
    public partial class frm销售记录成库通知单详细界面_视图 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string str_出库通知单号 = "";
        DataTable dtP;
        DataRow drM,dr_2;
        DataTable dt_审核;
        Boolean s_作废 = false;
        string s_目标客户 = "";
        [DllImport("winspool.drv")]

        public static extern bool SetDefaultPrinter(String Name); //调用win api将指定名称的打印机设置为默认打印机

        public frm销售记录成库通知单详细界面_视图(string str, DataRow dr)
        {
            InitializeComponent();
            str_出库通知单号 = str;
            drM = dr;

            //textBox3.Text = drM["出库通知单号"].ToString();
            if (dr["已出库"].Equals(false))
            {
                textBox1.ReadOnly =false;
                textBox2.ReadOnly = false;
            }
        }
        public frm销售记录成库通知单详细界面_视图(string str, DataRow dr, DataTable dt,string s_单位)
        {
            InitializeComponent();
            str_出库通知单号 = str;
            drM = dr;
            dtP = dt;
            s_目标客户 = s_单位;
            textBox6.Text = s_目标客户;
        }


        private void frm销售记录成库通知单详细界面_视图_Load(object sender, EventArgs e)
        {
            if (drM != null)
            {

                dr_2 = drM;

            }

            dataBindHelper1.DataFormDR(drM);
            //dateEdit1.EditValue = drM["客户提货日期"];
            fun_载入明细();
        }
        //刷新已通知数量和未通知数量         所有该销售明细的 出库通知单的 '出库数量' 总和
        private void fun_通知数量(string str_明细号)
        {
            string sql = @"select 销售订单明细号,sum(出库数量)a from 销售记录销售出库通知单明细表
                         where 作废=0 and 销售订单明细号='{1}' group by 销售订单明细号 ";
            DataTable dt_数量 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            if (dt_数量.Rows.Count > 0)
            {
                string sql_1 = string.Format("select * from  [销售记录销售订单明细表] where 销售订单明细号='{0}'", str_明细号);
                using (SqlDataAdapter da = new SqlDataAdapter(sql_1, strconn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dt.Rows[0]["已通知数量"] = dt_数量.Rows[0]["a"];

                    dt.Rows[0]["未通知数量"] = dt_数量.Rows[0]["a"];

                }
            }
        }
        private void fun_载入明细()
        {

            string sql = string.Format(@"select stcmx.*,(kc.库存总数) as 仓库数量,stcmx.仓库号,stcmx.仓库名称 ,smx.包装方式,smx.包装方式编号 from 销售记录销售出库通知单明细表 stcmx
                left join 仓库物料数量表 kc on kc.物料编码 = stcmx.物料编码 and kc.仓库号=stcmx.仓库号
                left join  销售记录销售订单明细表 smx on smx.销售订单明细号=stcmx.销售订单明细号 where 
                stcmx.出库通知单号 = '{0}'  /*and 销售记录销售出库通知单明细表.作废=0*/", str_出库通知单号);

            dtP = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);//仓库数量
            da.Fill(dtP);
            if (dtP.Rows.Count > 0)
            {
                string sql1 = string.Format(@" select 目标客户 from  销售记录销售订单主表 where 销售订单号 in (    
                    select   销售订单号 from 销售记录销售订单明细表 where 销售订单明细号 = '{0}'  )     ", dtP.Rows[0]["销售订单明细号"]);
                DataTable dt111 = CZMaster.MasterSQL.Get_DataTable(sql1, strconn);
                if (dt111.Rows.Count > 0)
                {
                    textBox6.Text = dt111.Rows[0]["目标客户"].ToString();
                }
                
            }
            gc.DataSource = dtP;
        }
        private bool fun_主完成状态(string str_出库通知单号)
        {
            bool bl = false;
            string sql = string.Format(@"select a.总数,isnull(b.完成数,0) as 完成数 from 销售记录销售出库通知单主表 
        left join  (select  出库通知单号,COUNT(*) 总数 from 销售记录销售出库通知单明细表  group by 出库通知单号)a  
             on a.出库通知单号=销售记录销售出库通知单主表.出库通知单号
         left join  (select 出库通知单号,isnull(COUNT(*),0) as 完成数 from 销售记录销售出库通知单明细表 where 完成=1 and 作废=0  group by 出库通知单号 )b  
        on b.出库通知单号=销售记录销售出库通知单主表.出库通知单号
            where 销售记录销售出库通知单主表.出库通知单号='{0}' ", str_出库通知单号);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            if (dt.Rows.Count > 0)
            {
                if (  Convert.ToDecimal(dt.Rows[0]["总数"]) == Convert.ToDecimal(dt.Rows[0]["完成数"]))
                {
                    bl = true;
                
                }
            }
            return bl;
        }
        private void fun_check()
        {
            foreach (DataRow dr in dtP.Rows)
            {
                if (Convert.ToDecimal(dr["已出库数量"]) > 0)
                {
                    throw new Exception("有明细已出库，不可进行当前操作");
                }
            }
        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printDialog1.Document = this.printDocument1;
            DialogResult dr = this.printDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                //Get the Copy times
                int nCopy = this.printDocument1.PrinterSettings.Copies;
                //Get the number of Start Page
                int sPage = this.printDocument1.PrinterSettings.FromPage;
                //Get the number of End Page
                int ePage = this.printDocument1.PrinterSettings.ToPage;
                string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                int count = 0;
                if (dtP.Rows.Count % 9 != 0)
                {
                    count = dtP.Rows.Count / 9 + 1;
                }
                else
                {
                    count = dtP.Rows.Count / 9;
                }
                string s = string.Format(@"select  操作员 from 销售记录成品出库单主表 
                   where 成品出库单号 in (  select 成品出库单号 from 销售记录成品出库单明细表  where 出库通知单号='{0}')", txt_出库通知单号.Text);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "";
                if (temp.Rows.Count > 0)
                {
                   s= temp.Rows[0]["操作员"].ToString();

                }
                ItemInspection.print_FMS.fun_print_销售出库通知单_A5(dtP, count, PrinterName, s);
            }

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {

                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr == null) return;
                if (Convert.ToInt32(dr["已出库数量"]) > 0)
                {
                    gridColumn4.OptionsColumn.AllowEdit = false;

                }
                else
                {

                    gridColumn4.OptionsColumn.AllowEdit = true;

                }

                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc, new Point(e.X, e.Y));
                    if (dr["完成"].Equals(true))
                    {
                        完成ToolStripMenuItem.Visible = false;
                    }
                    else
                    {
                        完成ToolStripMenuItem.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 查看物料明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            ERPStock.frm仓库物料数量明细 frm = new ERPStock.frm仓库物料数量明细(dr["物料编码"].ToString(),dr["仓库号"].ToString());
            CPublic.UIcontrol.AddNewPage(frm, "物料明细");
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                DataTable dt = dtP.Copy();
               
                //保存 修改的 已通知数量
                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (r["作废"].Equals(true)) 
                    {
                        DataTable dt_mx = new DataTable();
                        string sql = string.Format("select * from  [销售记录销售订单明细表] where 销售订单明细号='{0}'", r["销售订单明细号", DataRowVersion.Original]);
                        using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                        {
                            da.Fill(dt_mx);
                            string sql_1 = string.Format(@"select 销售订单明细号,sum(出库数量)a from 销售记录销售出库通知单明细表
                                                        where  作废=0 and 销售订单明细号='{0}' and 出库通知单明细号<>'{1}' group by 销售订单明细号 ",
                                           dt_mx.Rows[0]["销售订单明细号"],r["出库通知单明细号"].ToString ());          //字段出库数量 即为表中通知数量
                            DataTable dt_1 = new DataTable();
                            using (SqlDataAdapter da_1 = new SqlDataAdapter(sql_1, strconn))
                            {
                                da_1.Fill(dt_1); // dt_1.rows[0]["a"]已通知数量  
                            }

                            DataRow[] r_x = dt_mx.Select(string.Format("销售订单明细号='{0}'", r["销售订单明细号", DataRowVersion.Original]));
                            if (dt_1.Rows.Count == 0)
                            {
                                r_x[0]["已通知数量"] = 0;
                                r_x[0]["未通知数量"] = r_x[0]["数量"];
                            }
                            else
                            {
                                //r_x[0]["已通知数量"] = Convert.ToDecimal(dt_1.Rows[0]["a"]) - Convert.ToDecimal(r["出库数量", DataRowVersion.Original]);  // "出库数量" 为 此次删掉的明细通知数
                                r_x[0]["已通知数量"] = dt_1.Rows[0]["a"];
                                r_x[0]["未通知数量"] = Convert.ToDecimal(r_x[0]["数量"]) - Convert.ToDecimal(dt_1.Rows[0]["a"]);
                                
                                //int j = Convert.ToDecimal(r_x[0]["数量"]) - Convert.ToDecimal(dt_1.Rows[0]["a"]);
                                //if (Convert.ToDecimal(r["出库数量", DataRowVersion.Original]) + Convert.ToDecimal(r_x[0]["数量"]) - Convert.ToDecimal(dt_1.Rows[0]["a"]) > 0)
                                //{
                                //    r_x[0]["未通知数量"] = Convert.ToDecimal(r_x[0]["数量"]) - Convert.ToDecimal(dt_1.Rows[0]["a"]) + Convert.ToDecimal(r["出库数量", DataRowVersion.Original]);
                                //}
                                //else
                                //{
                                //    r_x[0]["未通知数量"] = 0;
                                //}
                            }
                            new SqlCommandBuilder(da);
                            da.Update(dt_mx);
                        }
                    }
                    //fun_通知数量(r_x["销售订单明细号", DataRowVersion.Original].ToString());
                    else if (r.RowState == DataRowState.Modified)
                    {
                        decimal dec = Convert.ToDecimal(r["出库数量", DataRowVersion.Original]) - Convert.ToDecimal(r["出库数量", DataRowVersion.Current]);
                        string sql = string.Format(@"update [销售记录销售订单明细表] set 已通知数量=已通知数量-({0}),未通知数量=未通知数量+({0})
                            where 销售订单明细号='{1}' ", dec, r["销售订单明细号"]);
                        CZMaster.MasterSQL.ExecuteSQL(sql, strconn);

                    }
                    StockCore.StockCorer.fun_物料数量_实际数量(r["物料编码"].ToString(),r["仓库号"].ToString(), true);
                }

                //string sql_z = string.Format(@"select * from 销售记录销售出库通知单明细表 where  出库通知单号  ='{0}'", txt_出库通知单号.Text.Trim());
                //using (SqlDataAdapter da = new SqlDataAdapter(sql_z, strconn))
                //{
                //    new SqlCommandBuilder(da);
                //    da.Update(dtP);
                //}

                bool flag= fun_主完成状态(txt_出库通知单号.Text);
                string  sql_主 = string.Format(@"select * from 销售记录销售出库通知单主表 where  出库通知单号  ='{0}'", txt_出库通知单号.Text.Trim());
                DataTable dt_主 = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(sql_主, strconn))
                {
             
                   da.Fill(dt_主);
                   dt_主.Rows[0]["送货方式"] = textBox1.Text;
                   dt_主.Rows[0]["备注"] = textBox2.Text;
                   dt_主.Rows[0]["快递单号"] = textBox4.Text;
                   // dt_主.Rows[0]["客户订单号"] = textBox5.Text;
                    //if (dateEdit1.EditValue!=null)
                    //{
                    //    dt_主.Rows[0]["客户提货日期"] = dateEdit1.EditValue;

                    //}
                   
                    if (flag)
                    {
                       dt_主.Rows[0]["完成"] = true;
                       dt_主.Rows[0]["完成日期"] = CPublic.Var.getDatetime() ;

                    }
                } 
                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction cktz = conn.BeginTransaction("出库通知修改");
                try
                {
                    string sql_z = string.Format(@"select * from 销售记录销售出库通知单明细表 where  出库通知单号  ='{0}'", txt_出库通知单号.Text.Trim());
                    string s_审核 = "select * from 单据审核申请表 where 1<>1";
                    SqlCommand cmm_0 = new SqlCommand(sql_z, conn, cktz);

                    SqlCommand cmm_1 = new SqlCommand(sql_主, conn, cktz);
                    SqlCommand cmm_2 = new SqlCommand(s_审核, conn, cktz);
                    SqlDataAdapter da= new SqlDataAdapter(cmm_0);
                    SqlDataAdapter da1 = new SqlDataAdapter(cmm_1);
                    SqlDataAdapter da2 = new SqlDataAdapter(cmm_2);

                    new SqlCommandBuilder(da);
                    new SqlCommandBuilder(da1);
                    new SqlCommandBuilder(da2);
                    da.Update(dtP);
                    da1.Update(dt_主);
                    if(s_作废 == true)
                    {
                        da2.Update(dt_审核);
                    }

                    dtP.AcceptChanges();
                    dt_主.AcceptChanges();
                    cktz.Commit();
                    MessageBox.Show("ok");
                }
                catch
                {
                    cktz.Rollback();
                    throw new Exception("保存失败,请重试");
                }
 
  
            }
            catch (Exception ex) 
            {

               MessageBox.Show(ex.Message);
            }
           
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("确定删除该条明细？", "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                    if (Convert.ToDecimal(dr["已出库数量"]) > 0)
                    {
                        throw new Exception("该明细已出库，不可删除");
                    }
                    dr["作废"] = 1;
                    dr["作废时间"] = CPublic.Var.getDatetime() ;
                    dr["作废人"] = CPublic.Var.localUserName;
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message);
            }
           

        }
        //刷新
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frm销售记录成库通知单详细界面_视图_Load(null, null);
        }

        private void gv_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.Caption == "通知出库数量")
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                dr["未出库数量"] = e.Value;

            }
        }

        private void gv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv.GetFocusedRowCellValue(gv.FocusedColumn));
                e.Handled = true;
            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认作废吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    fun_check();
                    //foreach (DataRow dr in dtP.Rows)
                    //{

                    //    dr["作废"] = 1;
                    //    dr["作废时间"] = CPublic.Var.getDatetime();
                    //    dr["作废人"] = CPublic.Var.localUserName;
                    //    StockCore.StockCorer.fun_物料数量_实际数量(dr["物料编码"].ToString(), dr["仓库号"].ToString(), true);
 
                    //}
                    DateTime dtime = CPublic.Var.getDatetime();
                    string sql = string.Format(@"update  [销售记录销售出库通知单主表] set 作废=1,作废日期='{0}',作废人='{1}' where 出库通知单号='{2}'
                                    update  [销售记录销售出库通知单明细表] set 作废=1,作废时间='{0}',作废人='{1}' where 出库通知单号='{2}'           "
                        , dtime, CPublic.Var.localUserName, str_出库通知单号);
                    CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
                    sql = string.Format(@"select * from 单据审核申请表 where 审核=0 and 作废=0 and  关联单号 = '{0}'", str_出库通知单号);
                    dt_审核 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_审核.Rows.Count > 0)
                    {
                        dt_审核.Rows[0]["作废"] = true;
                    }
                    s_作废 = true;
                    // fun_主完成状态(str_出库通知单号);
                    barLargeButtonItem3_ItemClick(null, null);

                    //MessageBox.Show("已作废");
                }

            }
            catch (Exception ex )
            {

                MessageBox.Show(ex.Message);
            }
            

        }

        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void 完成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

               //出库数量为 通知出库数量，已出库数量为仓库发出数量,订单明细上的 已通知数量 和 未通知数量要修改
            if (MessageBox.Show("确定完成该条出库通知？已通知数量将会改为已出库数量。", "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                decimal dec = Convert.ToDecimal(dr["出库数量"]) - Convert.ToDecimal(dr["已出库数量"]);
                dr["出库数量"] = dr["已出库数量"];
                dr["完成"] = 1;
                dr["完成日期"] = CPublic.Var.getDatetime();
                //2020-1-8 增加完成备注  
                 dr["完成备注"] = CPublic.Var.localUserName+"右击明细完成";

                //对应的 销售订单明细 已通知数量和未通知数量需更改
                string sql = string.Format("select * from 销售记录销售订单明细表 where 销售订单明细号='{0}'", dr["销售订单明细号"]);
                DataTable  dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                dt.Rows[0]["已通知数量"] = Convert.ToDecimal(dt.Rows[0]["已通知数量"])-dec;
                dt.Rows[0]["未通知数量"] = Convert.ToDecimal(dt.Rows[0]["数量"]) - Convert.ToDecimal(dt.Rows[0]["已通知数量"]);

                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("出库通知完成");
                try
                {
                    {
                        sql = "select * from 销售记录销售出库通知单明细表 where 1<>1";
                        SqlCommand cmd = new SqlCommand(sql, conn, ts);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            new SqlCommandBuilder(da);
                            da.Update(dtP);
                        }
                    }
                    {
                        sql = "select * from 销售记录销售订单明细表 where 1<>1";
                        SqlCommand cmd = new SqlCommand(sql, conn, ts);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            new SqlCommandBuilder(da);
                            da.Update(dt);
                        }
                    }
                    ts.Commit();

                    fun_主完成状态(txt_出库通知单号.Text.Trim());

                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    throw ex;
                }
            }

            frm销售记录成库通知单详细界面_视图_Load(null, null);


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                gc.ExportToXlsx(saveFileDialog.FileName);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void 查看销售明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                string[] s_截取 = dr["销售订单明细号"].ToString().Split('-');
                string s_销售订单号 = s_截取[0];
                if(s_销售订单号 == ""||s_销售订单号==null)
                {
                    throw new Exception("该记录无法查到销售明细");
                }
                frm销售单证详细界面_视图 fm = new frm销售单证详细界面_视图(dr, s_销售订单号);
                fm.Dock = System.Windows.Forms.DockStyle.Fill;
                CPublic.UIcontrol.AddNewPage(fm, "销售订单");
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
                if (bool.Parse(dr_2["审核"].ToString())==false)
                {
                    throw new Exception("当前单据审核未完成不可打印！");
                }



                string s = string.Format(@"select  操作员 from 销售记录成品出库单主表 
                   where 成品出库单号 in (  select 成品出库单号 from 销售记录成品出库单明细表  where 出库通知单号='{0}')", txt_出库通知单号.Text);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "";
                if (temp.Rows.Count > 0)
                {
                    s = temp.Rows[0]["操作员"].ToString();

                }
            
                string sql = string.Format(@"select a.*,isnull(c.领导姓名,'')as 审核人   from 销售记录销售出库通知单主表 a  
left  join 人事基础员工表 b  on a.操作员ID=b.员工号          
                    left  join  人事基础部门表 c on b.课室编号=c.部门编号   where  出库通知单号 = '{0}' ", dtP.Rows[0]["出库通知单号"].ToString());
                System.Data.DataTable dt = new System.Data.DataTable();
                new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dt);//          //// left  join 销售记录销售订单主表 d on a.   ,
                                                                      // string sq = string.Format("select * from  ");
                                                                      // 
                                                                      // DataRow drrr_目标客户=

                string s2 = dtP.Rows[0]["销售订单明细号"].ToString();
                string[] sArray = s2.Split('-');
                string sql_xiaoshou = string.Format("select * from 销售记录销售订单主表 where 销售订单号='{0}'",sArray[0].ToString());
                DataTable dt_目标客户 = CZMaster.MasterSQL.Get_DataTable(sql_xiaoshou,strconn);
                string sss = string.Format("select 地址 from 客户基础信息表 where 客户编号 = '{0}'", dtP.Rows[0]["客户编号"].ToString().Trim());
                System.Data.DataTable ttt = new System.Data.DataTable();
                new SqlDataAdapter(sss, CPublic.Var.strConn).Fill(ttt);
                DataTable dt_mx = dtP.Copy();
                for(int i= dt_mx.Rows.Count-1; i >=0; i--)
                {
                    if(Convert.ToBoolean(dt_mx.Rows[i]["作废"]) == true)
                    {
                        dt_mx.Rows[i].Delete();
                    }
                }
                //foreach(DataRow dr_mx in dt_mx.Rows)
                //{
                //    if (dr_mx.RowState == DataRowState.Deleted)
                //    {
                //        continue;
                //    }
                //}
                ERPreport.出库通知附件 frm = new ERPreport.出库通知附件(dt_mx, dt, ttt,temp, dt_目标客户);
                frm.ShowDialog();

                // ItemInspection.print_FMS.fun_print_销售出库通知单_A5(temp,);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        
    }
}
