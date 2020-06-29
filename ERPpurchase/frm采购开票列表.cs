using CZMaster;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ERPpurchase
{
    public partial class frm采购开票列表 : UserControl
    {
        string strcon = CPublic.Var.strConn;

        public frm采购开票列表()
        {
            InitializeComponent();
        }

        #region 变量

        DataTable dt_通知单主表;
        string cfgfilepath = "";
        DataTable dt_开票通知明细;

        #endregion


        private void frm采购开票列表_Load(object sender, EventArgs e)
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

                //txt_kaipiaotzd.EditValue = "";
                txt_kptime1.EditValue = Convert.ToDateTime(System.DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd"));
                txt_kptime2.EditValue = Convert.ToDateTime(System.DateTime.Today.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
                txt_danjustate.EditValue = "所有";
                fun_查询开票通知单();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //信息查询
        private void fun_查询开票通知单()
        {
            try
            {
                string sql = "";
                //if (txt_kaipiaotzd.EditValue.ToString() != "")
                //{
                //    sql = sql + string.Format(" 开票通知单号='{0}' and", txt_kaipiaotzd.EditValue.ToString());
                //}
                //else
                //{
                if (txt_kptime1.EditValue != null && txt_kptime2.EditValue != null && txt_kptime1.EditValue.ToString() != "" && txt_kptime2.EditValue.ToString() != "")
                {
                    if (Convert.ToDateTime(txt_kptime1.EditValue) > Convert.ToDateTime(txt_kptime2.EditValue))
                        throw new Exception("起始日期不能大于终止日期！");
                    sql = sql + string.Format(@" 录入日期>='{0}' and 录入日期<='{1}' and", txt_kptime1.EditValue.ToString()
                        , Convert.ToDateTime(txt_kptime2.EditValue).AddDays(1).AddSeconds(-1));
                }

                if (txt_danjustate.EditValue.ToString() == "已生效")
                {
                    sql = sql + " 生效=1 and";
                }
                if (txt_danjustate.EditValue.ToString() == "未生效")
                {
                    sql = sql + " 生效=0 and";
                }
                if (txt_danjustate.EditValue.ToString() == "已作废")
                {
                    sql = sql + " 作废=1 and";
                }
                //if (txt_danjustate.EditValue.ToString() == "未作废")
                //{
                //    sql = sql + " 作废=0 and";
                //}
                //}
                string s_add = "";
                if (CPublic.Var.LocalUserTeam == "开发部权限" || CPublic.Var.localUser部门名称.Contains("开发"))
                {
                    s_add = " and 部门名称 like '%开发%'";
                }
                else if (CPublic.Var.LocalUserTeam != "管理员权限" && CPublic.Var.LocalUserTeam != "财务部权限")
                {
                    s_add = " and 部门名称 not  like '%开发%'";

                    barLargeButtonItem7.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                }
                sql = " where" + sql.Substring(0, sql.Length - 3) + s_add;
                sql = string.Format("select * from 采购记录采购开票通知单主表 {0}  order by 开票通知单号 desc", sql);
                dt_通知单主表 = MasterSQL.Get_DataTable(sql, strcon);
                gcc1.DataSource = dt_通知单主表;

            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_searchSongjianDan");
                throw ex;
            }
        }


        #region 界面的操作
        //清空单号
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //txt_kaipiaotzd.EditValue = "";
        }

        //查询操作
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_查询开票通知单();
                if (dt_通知单主表.Rows.Count <= 0)
                    throw new Exception("查无数据！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //新增操作
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                UI采购开票 frm = new UI采购开票();
                CPublic.UIcontrol.AddNewPage(frm, "采购开票");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //界面关闭
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        //双击查询信息
        private void gvv1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                DataRow r = (this.BindingContext[dt_通知单主表].Current as DataRowView).Row;

                string sql = string.Format(@"select 采购记录采购开票通知单明细表.*  from 采购记录采购开票通知单明细表  
                                           where 开票通知单号='{0}'  ", r["开票通知单号"].ToString());
                SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                dt_开票通知明细 = new DataTable();
                da.Fill(dt_开票通知明细);



                gcM.DataSource = dt_开票通知明细;

                if (e.Clicks == 2)
                {

                    if (r["生效"].Equals(false))
                    {
                        UI采购开票 frm = new UI采购开票(r["开票通知单号"].ToString());
                        CPublic.UIcontrol.AddNewPage(frm, "采购开票");
                    }
                    else
                    {


                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        //发票核销的操作
        private void 发票核销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow r = gvv1.GetDataRow(gvv1.FocusedRowHandle);
                if (r["生效"].Equals(false))
                    throw new Exception("开票通知单尚未生效，不能进行发票录入！");
                if (r["作废"].Equals(true))
                    throw new Exception("开票通知单已经作废，不能进行发票录入！");
                frm采购发票核销界面 frm = new frm采购发票核销界面(r["开票通知单号"].ToString());
                CPublic.UIcontrol.AddNewPage(frm, "采购发票核销");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonIgtem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("确定作废？请核对。", "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                DataRow dr = gvv1.GetDataRow(gvv1.FocusedRowHandle);
                string sql = string.Format(@"update 采购记录采购开票通知单主表 set 作废=1,作废日期='{0}',作废人员='{1}',作废人员ID='{2}' 
                                    where 开票通知单号='{3}'    ", CPublic.Var.getDatetime(), CPublic.Var.localUserName, CPublic.Var.LocalUserID, dr["开票通知单号"]);
                CZMaster.MasterSQL.ExecuteSQL(sql, strcon);


                string sql_1 = string.Format(@"update 采购记录采购开票通知单明细表 set 作废=1,作废日期='{0}',作废人员='{1}',作废人员ID='{2}'
                                   where 开票通知单号='{3}'", CPublic.Var.getDatetime(), CPublic.Var.localUserName, CPublic.Var.LocalUserID, dr["开票通知单号"]);
                CZMaster.MasterSQL.ExecuteSQL(sql_1, strcon);
            }
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                //gridControl1.ExportToXls(saveFileDialog.FileName, options);  
                gcM.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //作废
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gvv1.GetDataRow(gvv1.FocusedRowHandle);
                ERPorg.Corg cc = new ERPorg.Corg();
                bool bl = true;
                if (dr["发票确认日期"] != null && dr["发票确认日期"].ToString() != "")
                {
                    bl = cc.isJZ(Convert.ToDateTime(dr["发票确认日期"]));
                }
                if (!bl) throw new Exception("改单据已结算不可作废");

                if (dr["作废"].Equals(true))
                {
                    throw new Exception("该记录已作废");
                }
                if (dr["发票确认"].Equals(true))
                {
                    throw new Exception("已发票确认不可作废");

                }
                if (MessageBox.Show("是否确认作废？", "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    if (dr["生效"].Equals(true))
                    {
                        fun_取消生效(dr["开票通知单号"].ToString());
                        fun_作废(dr["开票通知单号"].ToString());
                    }
                    else
                    {

                        fun_作废(dr["开票通知单号"].ToString());

                    }
 
                    MessageBox.Show("已删除");
                }
                fun_查询开票通知单();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gvv1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gvM_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }
        //取消生效
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gvv1.GetDataRow(gvv1.FocusedRowHandle);
                if (dr["生效"].Equals(false))
                {
                    throw new Exception("该记录未生效");
                }

                if (dr["作废"].Equals(true))
                {
                    throw new Exception("该记录已作废");

                }
                if (dr["发票确认"].Equals(true))
                {
                    throw new Exception("已发票确认不可取消生效");

                }
                //  生效字段赋为0，主表子表  已开票数量   未开票数量 成品出库单明细表
                if (MessageBox.Show("是否确认取消生效是状态？", "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    fun_取消生效(dr["开票通知单号"].ToString());
                }
                MessageBox.Show("ok");
                fun_查询开票通知单();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                CZMaster.MasterLog.WriteLog(ex.Message);
            }
        }
        private void fun_取消生效(string str_开票号)
        {
            string sql = string.Format("select * from 采购记录采购开票通知单主表 where 开票通知单号='{0}' and  作废=0", str_开票号);
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            dt.Rows[0]["生效"] = 0;

            //   dt.Rows[0]["生效日期"] = DBNull.Value;
            string sql_mx = string.Format("select * from 采购记录采购开票通知单明细表 where 开票通知单号='{0}'", str_开票号);
            DataTable dt_mx = new DataTable();
            dt_mx = CZMaster.MasterSQL.Get_DataTable(sql_mx, strcon);
            foreach (DataRow dr in dt_mx.Rows)
            {
                dr["生效"] = 0;
                // dr["生效日期"] = DBNull.Value;
            }
            string sql_开票数 = string.Format(@"select 采购记录采购单入库明细.*,开票数量 from 采购记录采购单入库明细,采购记录采购开票通知单明细表 
                where  采购记录采购单入库明细.入库明细号 in (select 入库明细号 from 采购记录采购开票通知单明细表 where 生效=1  )
                                   and 采购记录采购单入库明细.入库明细号=采购记录采购开票通知单明细表.入库明细号  and 开票通知单号='{0}'", str_开票号);
            DataTable dt_开票数 = new DataTable();
            dt_开票数 = CZMaster.MasterSQL.Get_DataTable(sql_开票数, strcon);
            foreach (DataRow dr in dt_开票数.Rows)
            {

                dr["已开票量"] = Convert.ToDecimal(dr["已开票量"]) - Convert.ToDecimal(dr["开票数量"]);
            }
            string sql_开票数_补 = string.Format(@" select L采购记录采购单入库明细L.*,开票数量 from L采购记录采购单入库明细L,采购记录采购开票通知单明细表 
            where  L采购记录采购单入库明细L.入库明细号 in (select 入库明细号 from 采购记录采购开票通知单明细表 where 生效=1  )
            and L采购记录采购单入库明细L.入库明细号=采购记录采购开票通知单明细表.入库明细号 and 开票通知单号='{0}'", str_开票号);
            DataTable dt_开票数_补 = new DataTable();
            dt_开票数_补 = CZMaster.MasterSQL.Get_DataTable(sql_开票数_补, strcon);
            foreach (DataRow dr in dt_开票数_补.Rows)
            {

                dr["已开票量"] = Convert.ToDecimal(dr["已开票量"]) - Convert.ToDecimal(dr["开票数量"]);


            }

            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction stc = conn.BeginTransaction("开票取消生效");
            try
            {
                {
                    sql = "select * from 采购记录采购开票通知单主表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, stc);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt);
                    }
                }
                {
                    sql_mx = "select * from 采购记录采购开票通知单明细表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql_mx, conn, stc);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_mx);
                    }
                }
                if (dt_开票数 != null)
                {
                    sql_开票数 = "select * from 采购记录采购单入库明细 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql_开票数, conn, stc);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_开票数);
                    }
                }

                if (dt_开票数_补 != null)
                {
                    sql_开票数_补 = "select * from L采购记录采购单入库明细L where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql_开票数_补, conn, stc);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_开票数_补);
                    }
                }
                stc.Commit();
            }
            catch (Exception ex)
            {
                stc.Rollback();
                throw ex;
            }


        }


        /// <summary>
        /// 20-3-30 新增记录作废明细
        /// 作废 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        private void fun_作废(string str_开票号)
        {
            DateTime t = CPublic.Var.getDatetime();
            string sql = string.Format("select * from 采购记录采购开票通知单主表 where 开票通知单号='{0}' and  作废=0", str_开票号);
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);


            //   dt.Rows[0]["生效日期"] = DBNull.Value;
            string sql_mx = string.Format("select * from 采购记录采购开票通知单明细表 where 开票通知单号='{0}'", str_开票号);
            DataTable dt_mx = new DataTable();
            dt_mx = CZMaster.MasterSQL.Get_DataTable(sql_mx, strcon);

            string sql_zfmx = string.Format("select * from 采购开票作废记录明细表 where 1=2");
            DataTable dt_zfmx = new DataTable();
            dt_zfmx = CZMaster.MasterSQL.Get_DataTable(sql_zfmx, strcon);
            foreach (DataRow dr in dt_mx.Rows)
            {
                DataRow rr = dt_zfmx.NewRow();
                rr["GUID"] = System.Guid.NewGuid();
                rr["开票通知单号"] = dr["开票通知单号"].ToString();
                rr["开票通知明细号"] = dr["通知单明细号"].ToString();
                rr["开票人"] = dr["操作人员"].ToString(); 
                rr["删除人"] = CPublic.Var.localUserName;
                rr["删除时间"] = t;
                rr["供应商名称"] = dt.Rows[0]["供应商名称"].ToString();
                rr["物料编码"] = dr["物料编码"].ToString();
                rr["开票数量"] = dr["开票数量"].ToString();
                rr["票号备注"] = dt.Rows[0]["备注1"].ToString();
                dt_zfmx.Rows.Add(rr);
            }
            //删除开票主表
            dt.Rows[0].Delete();
            //删除开票明细
            for (int i = dt_mx.Rows.Count - 1; i >= 0; i--)
            {
                dt_mx.Rows[i].Delete();
            }
            //string sql_开票数 = string.Format(@"select 采购记录采购单入库明细.*,开票数量 from 采购记录采购单入库明细,采购记录采购开票通知单明细表 
            //    where  采购记录采购单入库明细.入库明细号 in (select 入库明细号 from 采购记录采购开票通知单明细表 where 生效=1  )
            //                       and 采购记录采购单入库明细.入库明细号=采购记录采购开票通知单明细表.入库明细号  and 开票通知单号='{0}'", str_开票号);
            //DataTable dt_开票数 = new DataTable();
            //dt_开票数 = CZMaster.MasterSQL.Get_DataTable(sql_开票数, strcon);
            //foreach (DataRow dr in dt_开票数.Rows)
            //{

            //    dr["已开票量"] = Convert.ToDecimal(dr["已开票量"]) - Convert.ToDecimal(dr["开票数量"]);
            //}
            //string sql_开票数_补 = string.Format(@" select L采购记录采购单入库明细L.*,开票数量 from L采购记录采购单入库明细L,采购记录采购开票通知单明细表 
            //where  L采购记录采购单入库明细L.入库明细号 in (select 入库明细号 from 采购记录采购开票通知单明细表 where 生效=1  )
            //and L采购记录采购单入库明细L.入库明细号=采购记录采购开票通知单明细表.入库明细号 and 开票通知单号='{0}'", str_开票号);
            //DataTable dt_开票数_补 = new DataTable();
            //dt_开票数_补 = CZMaster.MasterSQL.Get_DataTable(sql_开票数_补, strcon);
            //foreach (DataRow dr in dt_开票数_补.Rows)
            //{

            //    dr["已开票量"] = Convert.ToDecimal(dr["已开票量"]) - Convert.ToDecimal(dr["开票数量"]);


            //}

            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction stc = conn.BeginTransaction("开票取消生效");
            try
            {

                sql = "select * from 采购记录采购开票通知单主表 where 1<>1";
                SqlCommand cmd = new SqlCommand(sql, conn, stc);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt);
                }


                sql_mx = "select * from 采购记录采购开票通知单明细表 where 1<>1";
                cmd = new SqlCommand(sql_mx, conn, stc);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt_mx);
                }

                //20-3-30 作废明细记录
                cmd = new SqlCommand(sql_zfmx, conn, stc);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt_zfmx);
                }

                //if (dt_开票数 != null)
                //{
                //    sql_开票数 = "select * from 采购记录采购单入库明细 where 1<>1";
                //    cmd = new SqlCommand(sql_开票数, conn, stc);
                //    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                //    {
                //        new SqlCommandBuilder(da);
                //        da.Update(dt_开票数);
                //    }
                //}

                //if (dt_开票数_补 != null)
                //{
                //    sql_开票数_补 = "select * from L采购记录采购单入库明细L where 1<>1";
                //    cmd = new SqlCommand(sql_开票数_补, conn, stc);
                //    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                //    {
                //        new SqlCommandBuilder(da);
                //        da.Update(dt_开票数_补);
                //    }
                //}
                stc.Commit();
            }
            catch (Exception ex)
            {
                stc.Rollback();
                throw ex;
            }


        }

        [DllImport("winspool.drv")]
        public static extern bool SetDefaultPrinter(String Name);
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr3 = gvv1.GetDataRow(gvv1.FocusedRowHandle);
            string s_供应商 = dr3["供应商名称"].ToString();
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
                SetDefaultPrinter(PrinterName);

                decimal dec_含税金额 = 0;
                decimal dec_含税金额总 = 0;
                decimal dec_不含税金额 = 0;
                decimal dec_不含税金额总 = 0;
                foreach (DataRow dr2 in dt_开票通知明细.Rows)
                {
                    dec_不含税金额 = Convert.ToDecimal(dr2["折扣后不含税金额"]);
                    dec_不含税金额总 += dec_不含税金额;
                    dec_含税金额 = Convert.ToDecimal(dr2["折扣后含税金额"]);
                    dec_含税金额总 += dec_含税金额;
                }
                DataView dv = new DataView(dt_开票通知明细);
                dv.Sort = "送检单号";
                DataTable dt_dy = dv.ToTable();
                ItemInspection.print_FMS.fun_print_采购开票清单核销(dt_dy, s_供应商, false, dec_不含税金额总, dec_含税金额总);
            }
        }

        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";//txt_gysmc
            DataRow dr3 = gvv1.GetDataRow(gvv1.FocusedRowHandle);
            string s_供应商 = dr3["供应商名称"].ToString();

            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {

                decimal dec_含税金额 = 0;
                decimal dec_含税金额总 = 0;
                decimal dec_不含税金额 = 0;
                decimal dec_不含税金额总 = 0;
                foreach (DataRow dr2 in dt_开票通知明细.Rows)
                {
                    dec_不含税金额 = Convert.ToDecimal(dr2["折扣后不含税金额"]);
                    dec_不含税金额总 += dec_不含税金额;
                    dec_含税金额 = Convert.ToDecimal(dr2["折扣后含税金额"]);
                    dec_含税金额总 += dec_含税金额;
                }
                DataView dv = new DataView(dt_开票通知明细);
                dv.Sort = "送检单号";
                DataTable dt_dy = dv.ToTable();
                ItemInspection.print_FMS.fun_print_采购开票清单核销(dt_dy, s_供应商, false, dec_不含税金额总, dec_含税金额总, saveFileDialog.FileName);

                MessageBox.Show("ok");
            }
        }

        private void gvv1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gvv1.GetFocusedRowCellValue(gvv1.FocusedColumn));
                e.Handled = true;
            }
        }

        private void gvM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gvM.GetFocusedRowCellValue(gvM.FocusedColumn));
                e.Handled = true;
            }
        }

        private void barLargeButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try

            {
                DataRow drM = (this.BindingContext[gcc1.DataSource].Current as DataRowView).Row;
                DataTable dtm = (DataTable)this.gcM.DataSource;
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPreport.结算单", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                                                                           //  CPublic.UIcontrol.Showpage(ui, t.Rows[0]["打开界面名称"].ToString());
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

        private void barLargeButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gvv1.GetDataRow(gvv1.FocusedRowHandle);
                if (Convert.ToBoolean(dr["bl_pz"])) throw new Exception("该单据已生成凭证,不可撤销");
                if (MessageBox.Show(string.Format("是否确认撤销此单据？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {


                    ERPorg.Corg cc = new ERPorg.Corg();
                    bool bl = true;
                    if (dr["发票确认日期"] != null && dr["发票确认日期"].ToString() != "")
                    {
                        bl = cc.isJZ(Convert.ToDateTime(dr["发票确认日期"]));
                    }
                    string sql = string.Format("select * from  采购记录采购开票通知单主表 where 开票通知单号 = '{0}'", dr["开票通知单号"]);
                    DataTable dt_主 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    sql = string.Format("select * from  采购记录采购开票通知单明细表 where 开票通知单号 = '{0}'", dr["开票通知单号"]);
                    DataTable dt_子 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    //sql = string.Format("select * from  采购记录采购开票通知发票核销表 where 开票通知单号 = '{0}'", dr["开票通知单号"]);
                    //DataTable dt_核销 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    if (dt_主.Rows.Count > 0)
                    {

                        if (!Convert.ToBoolean(dt_主.Rows[0]["发票确认"]))
                        {
                            throw new Exception("该单据没有确认发票，请核实！");
                        }
                        else
                        {
                            //for(int i = dt_核销.Rows.Count - 1; i >= 0; i--)
                            //{
                            //    dt_核销.Rows[i].Delete();
                            //}
                            foreach (DataRow dr_zi in dt_子.Rows)
                            {
                                dr_zi["发票确认"] = false;
                                dr_zi["发票确认人"] = "";
                                dr_zi["发票确认人ID"] = "";
                                dr_zi["发票确认日期"] = DBNull.Value;
                            }

                            dt_主.Rows[0]["发票确认"] = false;
                            dt_主.Rows[0]["发票确认人"] = "";
                            dt_主.Rows[0]["发票确认人ID"] = "";
                            dt_主.Rows[0]["发票确认日期"] = DBNull.Value;

                            SqlConnection conn = new SqlConnection(strcon);
                            conn.Open();
                            SqlTransaction ts = conn.BeginTransaction("撤销");
                            try
                            {
                                string sql1 = "select * from 采购记录采购开票通知单主表 where 1<>1";
                                string sql2 = "select * from 采购记录采购开票通知单明细表 where 1<>1";
                                //  string sql3 = "select * from 采购记录采购开票通知发票核销表 where 1<>1";


                                SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
                                SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);
                                //SqlCommand cmd3 = new SqlCommand(sql3, conn, ts);

                                SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                                new SqlCommandBuilder(da1);
                                da1.Update(dt_主);

                                SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                                new SqlCommandBuilder(da2);
                                da2.Update(dt_子);

                                //SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
                                //new SqlCommandBuilder(da3);
                                //da3.Update(dt_核销);
                                ts.Commit();
                                MessageBox.Show("撤回成功");
                                DataRow[] ds = dt_通知单主表.Select(string.Format("开票通知单号 = '{0}'", dr["开票通知单号"]));
                                if (ds.Length > 0)
                                {
                                    ds[0]["发票确认"] = 0;
                                    ds[0].AcceptChanges();
                                }
                                //dt_通知单主表.Rows[0]["发票确认"] = 0;
                                //dt_通知单主表.Rows[0].AcceptChanges();
                            }
                            catch (Exception ex)
                            {
                                ts.Rollback();
                                throw new Exception(ex.Message);
                            }


                        }




                    }





                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        private void refresh_single(string s_开票通知单号)
        {
            string sql = string.Format(@"select * from 采购记录采购开票通知单主表  where 开票通知单号='{0}'", s_开票通知单号);
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            DataRow[] r_1 = dt_通知单主表.Select(string.Format("开票通知单号='{0}'", s_开票通知单号));
            r_1[0].ItemArray = dt.Rows[0].ItemArray;

        }
        private void gvv1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gvv1.GetDataRow(gvv1.FocusedRowHandle);
                if (dr == null) return;
                refresh_single(dr["开票通知单号"].ToString());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
    }
}
