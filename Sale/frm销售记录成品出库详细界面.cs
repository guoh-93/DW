using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ERPSale
{
    public partial class frm销售记录成品出库详细界面 : UserControl
    {
        #region 成员
        DataTable dt_主;//12-7 加
        DataTable dtM;
        DataRow drM;
        /// <summary>
        /// 出库明细用
        /// </summary>
        DataTable dtP = new DataTable();
        /// <summary>
        /// bar上的选择条件组合
        /// </summary>
        string str_选择条件 = "";
        /// <summary>
        /// 仅用于修改时
        /// </summary>
        string str_出库单号 = "";
        string strconn = CPublic.Var.strConn;
        /// <summary>
        /// 成品出库单号
        /// </summary>
        string strNo = "";
        /// <summary>
        /// false：修改；true：新增
        /// </summary>
        Boolean bl_新增or修改 = false;
        DataTable dt_已出库数量 = null;
        string str_客户编号 = "";
        string cfgfilepath = "";
        DataSet ds;
        DataTable dt_仓库;
        #endregion

        #region 自用类
        public frm销售记录成品出库详细界面()
        {
            InitializeComponent();
            bl_新增or修改 = true;
            fun_载入();
        }
        public frm销售记录成品出库详细界面(DataRow dr, DataTable dt)
        {
            InitializeComponent();
            bl_新增or修改 = true;
            drM = dr;
            dtM = dt;
        }
        public frm销售记录成品出库详细界面(string str, DataRow dr, DataTable dt)
        {
            InitializeComponent();
            bl_新增or修改 = false;
            str_出库单号 = str;
            drM = dr;
            dtM = dt;
        }

        private void frm销售记录成品出库详细界面_Load(object sender, EventArgs e)
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
                x.UserLayout(splitContainer1, this.Name, cfgfilepath);

                txt_日期.EditValue = CPublic.Var.getDatetime();
                txt_操作员.Text = CPublic.Var.localUserName;
                txt_仓库.EditValue = "成品库";

                fun_客户下拉框();
                fun_仓库下拉框();
                fun_载入明细();
                fun_load();
                fun_载入主表();
                //fun_载入待办();
                strNo = txt_成品出库单号.Text.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        //{
        //    
        //}
        #endregion

        #region 界面操作
        //没用了   //选择销售单号，新增出库明细
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //根据组合条件查询销售订单号
                fun_搜索组合();
                不用frm销售记录销售单选择界面 fm = new 不用frm销售记录销售单选择界面(str_选择条件, dtP);
                fm过往明细 from = new fm过往明细();
                from.Controls.Add(fm);
                fm.Dock = DockStyle.Fill;
                from.Text = "销售记录销售单选择";
                from.ShowDialog();
                fun_选择完后操作(fm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //新增
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataView dv = new DataView(dtM);
                dv.RowStateFilter = DataViewRowState.Added;
                if (dv.Count > 0)
                {
                    if (MessageBox.Show("当前有未保存的出库单，是否放弃保存？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        //清空
                        fun_清空();
                        dtP.Clear();
                        fun_载入明细(); fun_载入待办();
                    }
                }
                else
                {
                    fun_清空();
                    bl_新增or修改 = true;
                    dtP.Clear();
                    fun_载入明细();
                    fun_载入待办();

                    dtM.Clear();

                    drM = dtM.NewRow();
                    dtM.Rows.Add(drM);
                    strNo = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //保存 弃用
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                try
                {
                    gvP.CloseEditor();
                    this.BindingContext[dtP].EndCurrentEdit();
                    int i = 0;
                    foreach (DataRow r in dtP.Rows)
                    {
                        string sql = string.Format("select 物料状态,更改预计完成时间 from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                        DataTable t = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                        da.Fill(t);
                        if (t.Rows[0]["物料状态"].ToString() == "更改")
                        {
                            DateTime time = (DateTime)t.Rows[0]["更改预计完成时间"];
                            MessageBox.Show(string.Format("物料{0}为更改状态，不能出库，预计完成时间：{1}", r["物料编码"].ToString(), time.ToString("yyyy-MM-dd")));
                            i = 1;
                            break;
                        }
                    }
                    if (i == 1)
                    {
                    }
                    else
                    {
                        fun_Check明细();
                        fun_保存主表();
                        fun_保存明细();
                        fun_事务_保存();
                        fun_强载();
                        //保存完变成修改状态
                        bl_新增or修改 = false;
                        MessageBox.Show("保存成功");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //生效
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                gvP.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                if (dtP.Rows.Count == 0) throw new Exception("没有记录可生效");
                int i = 0;
                foreach (DataRow r in dtP.Rows)
                {
                    string sql = string.Format("select 物料状态,更改预计完成时间 from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                    DataTable t = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(t);
                    if (t.Rows[0]["物料状态"].ToString() == "更改")
                    {
                        DateTime time = (DateTime)t.Rows[0]["更改预计完成时间"];
                        MessageBox.Show(string.Format("物料{0}为更改状态，不能出库，预计完成时间：{1}", r["物料编码"].ToString(), time.ToString("yyyy-MM-dd")));
                        i = 1;
                        break;
                    }
                }
                if (i == 1) { }
                else
                {
                    fun_Check明细();
                    //fun_保存主表();
                    //fun_保存明细();
                    //fun_事务_保存();
                    //fun_强载();
                    //保存完变成修改状态  ???
                    bl_新增or修改 = true;

                    //if (txt_成品出库单号.Text != "")
                    //{
                    //第一步
                    fun_生效();
                    MessageBox.Show("生效成功！");
                    ////第二步
                    //fun_保存记录到出入库明细();  17/ 5/16  事务保存
                    //第三步
                    //foreach (DataRow r in dtP.Rows)
                    //{
                    //    //gh 11/11  为更新库存数   17/5/20 库存修改 改到 成品出库函数中  事务保存
                    ////StockCore.StockCorer.fun_刷新库存(r["物料编码"].ToString(),Convert.ToDecimal(r["出库数量"]), -1);
                    //    //
                    //  StockCore.StockCorer.fun_物料数量_实际数量(r["物料编码"].ToString(),r["仓库号"].ToString(),true);
                    //}
                    //20-6-12 刷新 受订等四个量
                    string x = "exec FourNum";
                    CZMaster.MasterSQL.ExecuteSQL(x, strconn);


                    //第四步 是否打印
                    if (MessageBox.Show("是否打印送货单？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
                            SetDefaultPrinter(PrinterName);

                            int count = 0;
                            if (dtP.Rows.Count % 8 != 0)
                            {
                                count = dtP.Rows.Count / 8 + 1;
                            }
                            else
                            {
                                count = dtP.Rows.Count / 8;
                            }

                            //DataSet ds = new DataSet();
                            //int ia = 0;
                            //DataTable t = dtP.Clone();
                            //ds.Tables.Add(t);
                            //foreach (DataRow r_x in dtP.Rows)
                            //{
                            //    if (ia > 0 && ia % 8 == 0)
                            //    {
                            //        t = dtP.Clone();
                            //        ds.Tables.Add(t);
                            //    }
                            //    DataRow rr = t.NewRow();
                            //    rr.ItemArray = r_x.ItemArray;
                            //    t.Rows.Add(rr);
                            //    ia++;
                            //}
                            //foreach (DataTable tt in ds.Tables)
                            //{
                            //    ItemInspection.print_FMS.fun_print_送货单(tt, 1);
                            //}

                            //*test
                            DataTable dt_dy = dtP.Copy();
                            ItemInspection.print_FMS.fun_print_送货单_修改(dt_dy, count);

                            //test
                            if (MessageBox.Show("是否打印出厂检验记录？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                printDialog1 = new System.Windows.Forms.PrintDialog();
                                this.printDialog1.Document = this.printDocument1;
                                // PrinterName = CPublic.Var.li_CFG["printer_jybg"].ToString();

                                // if (PrinterName=="")
                                dr = this.printDialog1.ShowDialog();
                                if (dr == DialogResult.OK)
                                {
                                    //Get the Copy times
                                    nCopy = this.printDocument1.PrinterSettings.Copies;
                                    //Get the number of Start Page
                                    sPage = this.printDocument1.PrinterSettings.FromPage;
                                    //Get the number of End Page
                                    ePage = this.printDocument1.PrinterSettings.ToPage;
                                    PrinterName = this.printDocument1.PrinterSettings.PrinterName;

                                    //SetDefaultPrinter(PrinterName);
                                    fun_打印出厂检验记录(PrinterName);
                                }
                            }
                        }
                    }

                    fun_清空();
                    bl_新增or修改 = true;
                    dtP.Clear();
                    fun_载入明细();
                    //fun_载入待办();
                    string a = "";
                    fun_载入待办(a);
                    fun_load();
                    drM = dtM.NewRow();
                    dtM.Rows.Add(drM);
                    strNo = "";
                    //}
                    //else
                    //{
                    //    MessageBox.Show("请确认是否已生成出库单号");
                    //}
                }
            }
            catch (Exception ex)
            {
                //fun_生效失败();
                MessageBox.Show(ex.Message);
            }
        }
        [DllImport("winspool.drv")]
        public static extern bool SetDefaultPrinter(String Name); //调用win api将指定名称的打印机设置为默认打印机
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        //刷新
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_清空();
                bl_新增or修改 = true;
                dtP.Clear();
                fun_载入明细();
                string a = "";
                fun_载入待办(a);
                fun_load();
                //fun_载入待办();
                dtM.Clear();
                drM = dtM.NewRow();
                dtM.Rows.Add(drM);
                strNo = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "小哥也不知道哪里出错了！");
            }
        }
        #endregion

        #region 待办 方法
        DataTable dt_待办; DataView dv;
        private void fun_载入待办()
        {
            string sql = string.Format(@"  select tzmx.*,tzzb.送货方式,tzzb.备注,基础数据物料信息表.原ERP物料编号 
   from 销售记录销售出库通知单明细表 tzmx left join [销售记录销售出库通知单主表] tzzb
    on tzzb.出库通知单号 = tzmx.出库通知单号 
   left join 基础数据物料信息表 on tzmx.物料编码 = 基础数据物料信息表.物料编码
  where tzmx.生效 = 1 and tzmx.作废=0  and tzmx.完成 = 0 and 出库数量>0 and left(tzmx.物料编码,3)<>'200'
  order by 出库通知单明细号");// and 操作员ID = '{0}', CPublic.Var.LocalUserID
            dt_待办 = new DataTable();
            dt_待办.Columns.Add("选择", typeof(Boolean));
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_待办);
            dv = new DataView(dt_待办);
            gc_待办.DataSource = dv;
        }



        private void fun_载入待办(string str_出库通知单号)
        {
            string sql = string.Format(@"select base.*,tz.送货方式,tz.备注,/*b.仓库号,b.仓库名称,*/
            isnull(库存总数,0)库存总数,[资产编码起],[资产编码止] from 销售记录销售出库通知单明细表 base 
            left join [销售记录销售出库通知单主表] tz   on tz.出库通知单号 = base.出库通知单号 
                /*left  join 销售记录销售订单明细表 smx on smx.销售订单明细号=base.销售订单明细号*/
                left join 仓库物料数量表 b on b.物料编码 = base.物料编码 and b.仓库号=base.仓库号
                left  join 销售箱贴信息维护表 c on   c.销售订单明细号= base.销售订单明细号   
                where base.生效 = 1 and base.作废=0  and base.完成 = 0 and 出库数量>0  and left(base.物料编码,3)<>'200' 
               and base.出库通知单号='{0}' order by 出库通知单明细号", str_出库通知单号);// and 操作员ID = '{0}', CPublic.Var.LocalUserID
            dt_待办 = new DataTable();
            dt_待办.Columns.Add("选择", typeof(Boolean));
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_待办);
            dv = new DataView(dt_待办);
            gc_待办.DataSource = dv;
        }
        private void fun_load()        //16-12-7加
        {
            string sql = @"select  * from 销售记录销售出库通知单主表  where 出库通知单号 in(
                         select  出库通知单号 from 销售记录销售出库通知单明细表 where 完成 =0 and 
                         作废=0  and 生效 = 1  and 出库数量>0 and left(物料编码,3)<>'200'  group by 出库通知单号 )";
            dt_主 = new DataTable();
            dt_主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gridControl1.DataSource = dt_主;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 使用事务保存主表数据、子表数据
        /// </summary>
        private void fun_事务_保存()
        {

            DataTable dt_出入明细 = fun_保存记录到出入库明细();

            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");
            try
            {
                {
                    string sql = "select * from 销售记录成品出库单明细表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dtP);
                    }
                }
                {
                    string sql = "select * from 销售记录成品出库单主表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dtM);
                    }
                }
                if (dt_已出库数量 != null)
                {
                    string sql = "select * from 销售记录销售出库通知单明细表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_已出库数量);
                    }
                }
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    string sql_主 = "select * from 销售记录销售订单主表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql_主, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(ds.Tables[0]);
                    }
                    string sql_1 = "select * from 销售记录销售订单明细表 where 1<>1";
                    SqlCommand cmd1 = new SqlCommand(sql_1, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(ds.Tables[1]);
                    }
                    string sql_2 = "select * from 仓库物料数量表 where 1<>1";
                    SqlCommand cmd2 = new SqlCommand(sql_2, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd2))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(ds.Tables[2]);
                    }

                    string sql_crmx = "select * from 仓库出入库明细表 where 1<>1";
                    SqlCommand cmd_crmx = new SqlCommand(sql_crmx, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd_crmx))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_出入明细);
                    }
                }

                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw new Exception(ex.Message);
            }
        }

        private void fun_成品出库单号生成()
        {
            DateTime t = CPublic.Var.getDatetime().Date;
            strNo = string.Format("SA{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("SA", t.Year, t.Month));
        }

        private void fun_保存主表()
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                string str_id = CPublic.Var.LocalUserID;
                string str_name = CPublic.Var.localUserName;

                if (bl_新增or修改 == true)
                {
                    drM["GUID"] = System.Guid.NewGuid();
                    drM["创建日期"] = t;
                    fun_成品出库单号生成();
                    txt_成品出库单号.Text = strNo;
                }
                try
                {
                    //drM["录入人员"] = CPublic.Var.localUserName;
                    drM["操作员ID"] = str_id;
                    drM["修改日期"] = t;
                    dataBindHelper1.DataToDR(drM);


                }
                catch (Exception)
                {
                    throw new Exception("保存出库单主表记录失败");
                };
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售记录成品出库详细界面_fun_保存主表");
                throw new Exception("保存主表失败" + ex.Message);
            }
        }

        private void fun_保存明细()
        {
            try
            {
                DataRow rr = gv_待办.GetDataRow(gv_待办.FocusedRowHandle);

                int i = 1;
                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    r["POS"] = i++;
                    r["成品出库单号"] = strNo;
                    r["已出库数量"] = Convert.ToDecimal(r["出库数量"]);
                    r["未开票数量"] = Convert.ToDecimal(r["出库数量"]);
                    r["成品出库单明细号"] = strNo + "-" + Convert.ToInt32(r["POS"]).ToString("00");

                    r["客户"] = rr["客户"];
                    r["客户编号"] = rr["客户编号"].ToString();

                    if (r["GUID"].ToString() != "") { }
                    else
                    {
                        r["GUID"] = System.Guid.NewGuid();
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售记录成品出库详细界面_fun_保存明细");
                throw new Exception("保存明细失败" + ex.Message);
            }
        }

        private void fun_载入明细()
        {
            try
            {
                string sql = "";
                //dr_传.Columns.Add("原ERP物料编号");
                if (bl_新增or修改 == true)
                {
                    sql = @"select 销售记录成品出库单明细表.*  from 销售记录成品出库单明细表 where 1<>1";
                    dtP.Columns.Add("库存总数");
                }
                else
                {
                    //sql = string.Format("select * from 销售记录成品出库单明细表 where 成品出库单号 = '{0}'", str_出库单号);
                    sql = string.Format(@"select stcmx.*,(kc.库存总数) as 库存总数 from 销售记录成品出库单明细表 stcmx
                    left join 仓库物料数量表 kc on kc.物料编码 = stcmx.物料编码 
                    left join 销售记录销售订单明细表 smx on smx.销售订单明细号 = stcmx.销售订单明细号
                    where  kc.仓库号=smx.仓库号 and  stcmx.成品出库单号 = '{0}'", str_出库单号);
                }
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtP);
                gcP.DataSource = dtP;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售记录成品出库详细界面_fun_载入明细");
            }
        }

        private void fun_载入主表()
        {
            if (bl_新增or修改 == true)
            {

            }
            else
            {
                dataBindHelper1.DataFormDR(drM);
            }
        }

        private void fun_搜索组合()
        {
            try
            {
                str_选择条件 = " ";
                if (txt_客户.EditValue != null && txt_客户.EditValue.ToString() != "")
                {
                    str_选择条件 = str_选择条件 + " 客户 = '" + txt_客户.EditValue.ToString() + "' and";
                }
                else
                {
                    throw new Exception("请先选择客户");
                }
                if (txt_仓库.EditValue != null && txt_仓库.EditValue.ToString() != "")
                {
                    str_选择条件 = str_选择条件 + " 仓库名称 = '" + txt_仓库.EditValue.ToString() + "' and";
                }
                else
                {
                    throw new Exception("请先选择仓库");
                }
                str_选择条件 = str_选择条件.Substring(0, str_选择条件.Length - 4);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售记录成品出库详细界面_fun_搜索组合");
                throw ex;
            }
        }

        private void fun_选择完后操作(不用frm销售记录销售单选择界面 fm)
        {
            try
            {
                //dr_传.Clear();
                foreach (DataRow r in fm.dt_选择.Rows)
                {
                    if (dtP.Select(string.Format("销售订单明细号 = '{0}'", r["销售订单明细号"].ToString())).Length > 0)
                    {

                    }
                    else
                    {
                        DataRow dr = dtP.NewRow();
                        dtP.Rows.Add(dr);
                        dr["销售订单号"] = r["销售订单号"].ToString();
                        dr["销售订单明细号"] = r["销售订单明细号"].ToString();
                        dr["物料编码"] = r["物料编码"].ToString();
                        dr["物料名称"] = r["物料名称"].ToString();
                        dr["出库数量"] = 0;
                        dr["已出库数量"] = r["完成数量"].ToString();
                        dr["参考数量"] = r["未完成数量"].ToString();

                        dr["仓库号"] = r["仓库号"].ToString();
                        dr["仓库名称"] = r["仓库名称"].ToString();
                        dr["计量单位"] = r["计量单位"].ToString();

                        dr["销售备注"] = r["备注"].ToString();
                    }
                }
                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (fm.dt_选择.Select(string.Format("销售订单明细号 = '{0}'", r["销售订单明细号"].ToString())).Length == 0)
                    {
                        r.Delete();
                    }
                }
                gcP.DataSource = dtP;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }
        DataTable dt_客户;
        private void fun_客户下拉框()
        {
            try
            {
                string sql = "select 客户编号,客户名称,联系人,固定电话,手机 from 客户基础信息表";
                dt_客户 = new DataTable();
                SqlDataAdapter da_客户 = new SqlDataAdapter(sql, strconn);
                da_客户.Fill(dt_客户);
                txt_客户.Properties.DataSource = dt_客户;
                txt_客户.Properties.DisplayMember = "客户名称";
                txt_客户.Properties.ValueMember = "客户名称";
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售记录成品出库详细界面_fun_客户下拉框");
            }
        }

        private void fun_仓库下拉框()
        {
            try
            {
                string sql = "select 属性值 as 仓库名称,属性字段1 as 仓库号 from 基础数据基础属性表 where 属性类别 = '仓库类别' and 布尔字段3 = 1";//只显示可用商品库
                dt_仓库 = new DataTable();
                SqlDataAdapter da_仓库 = new SqlDataAdapter(sql, strconn);
                da_仓库.Fill(dt_仓库);
                txt_仓库.Properties.DataSource = dt_仓库;
                txt_仓库.Properties.ValueMember = "仓库名称";
                txt_仓库.Properties.DisplayMember = "仓库名称";
                repositoryItemSearchLookUpEdit4.DataSource = dt_仓库;
                repositoryItemSearchLookUpEdit4.DisplayMember = "仓库号";
                repositoryItemSearchLookUpEdit4.ValueMember = "仓库号";
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售记录成品出库详细界面_fun_仓库下拉框");
            }
        }

        private void fun_载入()
        {
            try
            {
                string sql = "select * from 销售记录成品出库单主表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                dtM = new DataTable();
                da.Fill(dtM);
                drM = dtM.NewRow();
                dtM.Rows.Add(drM);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售记录成品出库详细界面_fun_载入");
            }
        }

        private void fun_清空()
        {
            //txt_操作员.Text = CPublic.Var.localUserName;
            txt_成品出库单号.Text = "";
            txt_客户.EditValue = "";
            txt_日期.EditValue = CPublic.Var.getDatetime();
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox6.Text = "";

        }
        /// <summary>
        /// 弃用
        /// </summary>
        private void fun_强载()
        {
            try
            {

                string sqll = string.Format(@"select base.*,b.n原ERP规格型号,b.原ERP物料编号,(a.库存总数) as 库存总数 from 销售记录成品出库单明细表 base
                    left join 仓库物料数量表 a on a.物料编码 = base.物料编码 
                    left join 基础数据物料信息表 b on   base.物料编码 = b.物料编码
                    where base.成品出库单号 = '{0}'", txt_成品出库单号.Text);
                SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
                dtP = new DataTable();
                daa.Fill(dtP);
                //dr_传.Columns.Add("原ERP物料编号");
                gcP.DataSource = dtP;

                string sql = string.Format("select * from 销售记录成品出库单主表 where 成品出库单号 = '{0}'", txt_成品出库单号.Text);
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                dtM = new DataTable();
                da.Fill(dtM);
                drM = dtM.Rows[0];
                dataBindHelper1.DataFormDR(drM);
            }
            catch { }
        }

        private void fun_Check明细()
        {
            try
            {
                if (txt_客户.EditValue == null || txt_客户.EditValue.ToString() == "")
                {
                    DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                    txt_客户.EditValue = dr["客户名"];
                }

                int i = 0;

                foreach (DataRow r in dt_待办.Rows)
                {
                    if (r["选择"].Equals(true))
                    {
                        string sql = string.Format("select * from 销售记录销售出库通知单明细表 where 出库通知单明细号='{0}' and 作废=0", r["出库通知单明细号"]);
                        DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        if (Convert.ToDecimal(r["已出库数量"]) != Convert.ToDecimal(dt.Rows[0]["已出库数量"]))
                        {
                            throw new Exception("已有明细被被修改过，刷新后重试");

                        }
                        if (dt.Rows.Count == 0)
                        {
                            throw new Exception("已有明细被作废,请刷新过后再进行操作");

                        }
                        i++;
                    }
                }
                if (i != dtP.Rows.Count)
                {
                    throw new Exception("需要保存的明细与选择的明细条目不相同，请刷新后重试");
                }
                foreach (DataRow r in dtP.Rows)
                {

                    if (Convert.ToDecimal(r["出库数量"]) == (Decimal)0)
                    {
                        throw new Exception(string.Format("物料{0}未填写出库数量", r["物料名称"].ToString()));
                    }
                    if (Convert.ToDecimal(r["出库数量"]) < 0)
                    {
                        throw new Exception("出库数量不正确");

                    }
                    if (Convert.ToDecimal(r["出库数量"]) > Convert.ToDecimal(r["库存总数"]))
                    {
                        throw new Exception("出库数量超出上限");
                    }
                    DataRow[] dr_未出库 = dt_待办.Select(string.Format("出库通知单明细号 = '{0}'", r["出库通知单明细号"].ToString()));
                    if (dr_未出库.Length > 0)
                    {
                        if (Convert.ToDecimal(r["出库数量"]) > Convert.ToDecimal(dr_未出库[0]["未出库数量"]))
                        {
                            throw new Exception("出库数量超出销售发货通知数量");
                        }
                    }


                    //if (str == "")
                    //{

                    //    str = r["销售订单明细号"].ToString().Substring(0, 14);

                    //}
                    //else
                    //{
                    //    if (str != r["销售订单明细号"].ToString().Substring(0, 14))
                    //    {
                    //        throw new Exception("选择了不同的销售单明细");
                    //    }
                    //}
                    if (r["仓库号"].ToString() == "")
                    {
                        throw new Exception("仓库号不能为空");
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fun_生效()
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                //主表生效
                drM["生效"] = 1;
                drM["生效日期"] = t;
                //drM["送货方式"]=
                fun_保存主表();
                //明细生效
                foreach (DataRow r in dtP.Rows)
                {
                    r["生效"] = 1;
                    r["生效日期"] = t;

                }

                ds = new DataSet();
                ds = StockCore.StockCorer.fun_出入库_成品出库(dtP);

                fun_保存明细();

                fun_已出库数量();
                fun_事务_保存();
            }
            catch (Exception ex)
            {

                CZMaster.MasterLog.WriteLog(ex.Message);
                throw ex;
            }
            //try
            //{
            //    fun_强载();

            //    //出库数量及一系列变化
            //    //foreach (DataRow r_x in dtP.Rows)
            //    //{
            //    //    StockCore.StockCorer.fun_出入库_成品出库(r_x["物料编码"].ToString(), (Decimal)r_x["出库数量"], r_x["销售订单明细号"].ToString());
            //    //}
            //}
            //catch (Exception ex)
            //{
            //    CZMaster.MasterLog.WriteLog(ex.Message, "明细界面生效");
            //    fun_生效失败();
            //    throw new Exception("生效失败" + ex.Message);
            //}
        }

        private void fun_生效失败()
        {
            try
            {
                //主表生效
                drM["生效"] = 0;
                drM["生效日期"] = DBNull.Value;
                fun_保存主表();
                //明细生效
                foreach (DataRow r in dtP.Rows)
                {
                    r["生效"] = 0;
                    r["生效日期"] = DBNull.Value;
                }
                fun_保存明细();
                fun_事务_保存();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售记录成品出库详细界面_fun_生效失败");
            }
        }

        private DataTable fun_保存记录到出入库明细()
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                string sql = "select * from 仓库出入库明细表 where 1<>1";
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                foreach (DataRow r in dtP.Rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["GUID"] = System.Guid.NewGuid();
                    dr["明细类型"] = "销售出库";
                    dr["单号"] = r["成品出库单号"].ToString();
                    dr["物料编码"] = r["物料编码"].ToString();
                    dr["物料名称"] = r["物料名称"].ToString();
                    dr["明细号"] = r["成品出库单明细号"].ToString();
                    dr["出库入库"] = "出库";

                    dr["相关单位"] = txt_客户.EditValue;
                    dr["相关单号"] = r["销售订单明细号"];
                    dr["仓库号"] = r["仓库号"];
                    dr["仓库名称"] = r["仓库名称"];
                    dr["数量"] = (Decimal)0;
                    dr["单位"] = r["计量单位"].ToString();
                    dr["标准数量"] = (Decimal)0;
                    dr["实效数量"] = Convert.ToDecimal("-" + r["出库数量"].ToString());
                    dr["实效时间"] = t;
                    dr["出入库时间"] = t;
                    dr["仓库人"] = CPublic.Var.localUserName;


                    dt.Rows.Add(dr);
                }

                return dt;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售记录成品出库详细界面_fun_保存出入库明细");
                throw ex;
            }
        }

        private void fun_已出库数量()
        {
            DateTime t = CPublic.Var.getDatetime();
            dt_已出库数量 = new DataTable();
            foreach (DataRow r in dtP.Rows)
            {
                string sql = string.Format("select * from 销售记录销售出库通知单明细表 where 出库通知单明细号 = '{0}' and 物料编码 = '{1}'", r["出库通知单明细号"].ToString().Trim(), r["物料编码"].ToString());
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_已出库数量);

                DataRow[] ds = dt_已出库数量.Select(string.Format("出库通知单明细号 = '{0}' and 物料编码 = '{1}'", r["出库通知单明细号"].ToString().Trim(), r["物料编码"].ToString()));
                ds[0]["已出库数量"] = Convert.ToDecimal(ds[0]["已出库数量"]) + Convert.ToDecimal(r["出库数量"]);
                ds[0]["未出库数量"] = Convert.ToDecimal(ds[0]["未出库数量"]) - Convert.ToDecimal(r["出库数量"]);
                if (Convert.ToDecimal(ds[0]["未出库数量"]) <= 0)
                {
                    ds[0]["未出库数量"] = 0;
                    ds[0]["完成"] = 1;
                    ds[0]["完成日期"] = t;
                }
            }
        }
        #endregion

        private void 查看物料明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);
            ERPStock.frm仓库物料数量明细 frm = new ERPStock.frm仓库物料数量明细(dr["物料编码"].ToString(), dr["仓库号"].ToString());
            CPublic.UIcontrol.AddNewPage(frm, "物料明细");
        }

        private void gvP_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gcP, new Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        #region 不用
        private void txt_客户_EditValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (  txt_客户.EditValue == null || txt_客户.EditValue.ToString() == "")
            //    {
            //        DataRow []r= dt_客户.Select(string.Format("客户名称='{0}'",txt_客户.EditValue.ToString()))  ;
            //        if(r.Length>0)
            //        {
            //            textBox2.Text = r[0]["客户编号"].ToString();
            //        }
            //    }


            //}
            //catch (Exception ex)
            //{
            //    CZMaster.MasterLog.WriteLog(ex.Message, "销售出库通知单界面_txt_客户编号_EditValueChanged");
            //}
        }
        /// <summary>
        /// 4/27
        /// gh

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //    try
            //    {
            //        if (textBox1.Text.ToString() == "" || textBox1.Text == null)
            //        {
            //            dv.RowFilter = "出库数量 > 0 ";
            //            foreach (DataRow r_x in dt_待办.Rows)
            //            {
            //                r_x["选择"] = false;
            //            }
            //            gc_待办.DataSource = dv;
            //        }
            //        else
            //        {
            //            DataRow[] ds = dt_客户.Select(string.Format("客户名称 = '{0}'", textBox1.Text));
            //            if (ds.Length != 0)
            //            {

            //                dv.RowFilter = string.Format("出库数量 > 0 and 客户 = '{0}'", textBox1.Text);
            //                dr_传.Clear();
            //                foreach (DataRow r_x in dt_待办.Rows)
            //                {
            //                    if (r_x["客户"] != textBox1.Text)
            //                    {
            //                        r_x["选择"] = false;
            //                    }
            //                }
            //                gc_待办.DataSource = dv;
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        CZMaster.MasterLog.WriteLog(ex.Message, "销售出库通知单界面_txt_客户编号_EditValueChanged");
            //    }
        }
        #endregion

        //打印
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //DataTable dtM = dt_待办.Clone();
                //foreach (DataRow r_x in dt_待办.Rows)
                //{
                //    if (r_x["选择"].ToString().ToLower() == "true")
                //    {
                //        DataRow rr = dtM.NewRow();
                //        dtM.Rows.Add(rr);
                //        rr.ItemArray = r_x.ItemArray;
                //    }
                //}
                //if (dtM.Rows.Count == 0) { MessageBox.Show("请选择需要打印的通知单"); return; }
                //PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                //this.printDialog1.Document = this.printDocument1;
                //DialogResult dr = this.printDialog1.ShowDialog();
                //if (dr == DialogResult.OK)
                //{

                //    int nCopy = this.printDocument1.PrinterSettings.Copies;

                //    int sPage = this.printDocument1.PrinterSettings.FromPage;

                //    int ePage = this.printDocument1.PrinterSettings.ToPage;
                //    string PrinterName = this.printDocument1.PrinterSettings.PrinterName;

                //    int count = 0;
                //    if (dtM.Rows.Count % 9 != 0)
                //    {
                //        count = dtM.Rows.Count / 9 + 1;
                //    }
                //    else
                //    {
                //        count = dtM.Rows.Count / 9;
                //    }
                //    DataSet ds = new DataSet();
                //    int ia = 0;
                //    DataTable t = dtM.Clone();
                //    ds.Tables.Add(t);
                //    foreach (DataRow r_x in dtM.Rows)
                //    {
                //        if (ia > 0 && ia % 9 == 0)
                //        {
                //            t = dtM.Clone();
                //            ds.Tables.Add(t);
                //        }
                //        DataRow rr = t.NewRow();
                //        rr.ItemArray = r_x.ItemArray;
                //        t.Rows.Add(rr);
                //        ia++;
                //    }
                //    foreach (DataTable ttt in ds.Tables)
                //    {
                //        ItemInspection.print_FMS.fun_print_销售出库通知单(ttt, 1, PrinterName);
                //    }

                //}
                if (dtP.Rows.Count == 0) throw new Exception("没有记录可打印");
                PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                this.printDialog1.Document = this.printDocument1;
                DialogResult dr = this.printDialog1.ShowDialog();
                if (dr == DialogResult.OK)
                {

                    int nCopy = this.printDocument1.PrinterSettings.Copies;

                    int sPage = this.printDocument1.PrinterSettings.FromPage;

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
                    ItemInspection.print_FMS.fun_print_销售出库通知单_A5(dtP, count, PrinterName, CPublic.Var.localUserName);
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "出库打印失败");
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_打印出厂检验记录(string str_打印机)
        {
            DataTable dt_主 = new DataTable();
            dt_主.Columns.Add("物料编码");
            dt_主.Columns.Add("物料名称");
            dt_主.Columns.Add("规格");
            dt_主.Columns.Add("大类");
            dt_主.Columns.Add("小类");
            dt_主.Columns.Add("发货数量");
            dt_主.Columns.Add("合格数");
            dt_主.Columns.Add("生产者");
            dt_主.Columns.Add("班组");
            dt_主.Columns.Add("部门");
            dt_主.Columns.Add("生产日期");
            foreach (DataRow r in dtP.Rows)
            {
                #region dtM
                string sql = string.Format("select 物料编码,物料名称,规格,大类,小类 from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"]);
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                DataTable dt = new DataTable();
                dt_主.Clear();
                da.Fill(dt);
                DataRow dr = dt_主.NewRow();
                dt_主.Rows.Add(dr);
                dr["物料编码"] = dt.Rows[0]["物料编码"];
                dr["物料名称"] = dt.Rows[0]["物料名称"];
                dr["规格"] = dt.Rows[0]["规格"];
                dr["大类"] = dt.Rows[0]["大类"];
                dr["小类"] = dt.Rows[0]["小类"];

                sql = string.Format(@"select 生产记录生产检验单主表.生产检验单号,(生产记录生产检验单主表.负责人员) as 生产者,(人事基础员工表.岗位) as 班组,
                                人事基础员工表.部门,(生产记录生产检验单主表.检验日期) as 生产日期 from 生产记录生产检验单主表 
                    left join 人事基础员工表 on 生产记录生产检验单主表.负责人员ID = 人事基础员工表.员工号 where 生产记录生产检验单主表.物料编码 = '{0}' 
                                order by 生产记录生产检验单主表.检验日期 desc", r["物料编码"]);
                da = new SqlDataAdapter(sql, strconn);
                dt = new DataTable();
                da.Fill(dt);
                try
                {
                    dr["生产者"] = dt.Rows[0]["生产者"];
                    dr["班组"] = dt.Rows[0]["班组"];
                    dr["部门"] = dt.Rows[0]["部门"];
                    dr["生产日期"] = dt.Rows[0]["生产日期"];
                }
                catch
                {
                    dr["生产者"] = "无";
                    dr["班组"] = "无";
                    dr["部门"] = "无";
                    dr["生产日期"] = CPublic.Var.getDatetime();
                }
                dr["发货数量"] = r["出库数量"];
                dr["合格数"] = r["出库数量"];
                #endregion

                #region dr_传
                try
                {
                    sql = string.Format("select * from 成品检验检验记录明细表 where 生产检验单号 = '{0}'", dt.Rows[0]["生产检验单号"]);
                }
                catch
                {
                    sql = "select * from 成品检验检验记录明细表 where 1<>1";
                }
                da = new SqlDataAdapter(sql, strconn);
                DataTable t = new DataTable();
                da.Fill(t);
                #endregion

                #region 打印
                ItemInspection.print_FMS.fun_print_出厂检验报告_原("销售", dt_主, t, 1, str_打印机, 0, 0);
                #endregion
            }
        }

        private void gv_待办_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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

        private void gv_待办_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv_待办.GetFocusedRowCellValue(gv_待办.FocusedColumn));
                e.Handled = true;
            }
        }

        private void gvP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gvP.GetFocusedRowCellValue(gvP.FocusedColumn));
                e.Handled = true;
            }
        }

        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            gv_待办.CloseEditor();
            gc_待办.BindingContext[dt_待办].EndCurrentEdit();
            DataRow drr = gv_待办.GetDataRow(gv_待办.FocusedRowHandle);
            //str_客户编号 = drr["客户编号"].ToString();
            try
            {
                //if (dr[""].ToString == "选择")
                {
                    if (drr["选择"].ToString().ToLower() == "true")
                    {
                        int count = 0;
                        foreach (DataRow rr in dtP.Rows)
                        {
                            if (rr.RowState == DataRowState.Deleted)
                            {
                                continue;
                            }
                            if (drr["出库通知单明细号"].ToString() == rr["出库通知单明细号"].ToString())
                            {
                                continue;
                            }
                            else
                            {
                                count++;
                            }
                        }
                        if (count == dtP.Rows.Count)
                        {
                            txt_客户.EditValue = drr["客户"].ToString();
                            // textBox1.Text = drr["送货地址"].ToString();
                            textBox5.Text = drr["备注1"].ToString();

                            DataRow dr = dtP.NewRow();
                            dtP.Rows.Add(dr);
                            dr["GUID"] = System.Guid.NewGuid();
                            dr["出库通知单号"] = drr["出库通知单号"].ToString();
                            dr["出库通知单明细号"] = drr["出库通知单明细号"].ToString();
                            dr["物料编码"] = drr["物料编码"].ToString();
                            dr["物料名称"] = drr["物料名称"].ToString();
                            dr["规格型号"] = drr["规格型号"].ToString();
                            dr["计量单位"] = drr["计量单位"].ToString();

                            dr["仓库号"] = drr["仓库号"].ToString();
                            dr["仓库名称"] = drr["仓库名称"].ToString();

                            dr["出库数量"] = Convert.ToDecimal(drr["出库数量"]) - Convert.ToDecimal(drr["已出库数量"]);

                            dr["销售订单明细号"] = drr["销售订单明细号"].ToString();

                            dr["特殊备注"] = drr["特殊备注"].ToString();
                            dr["送货方式"] = drr["送货方式"].ToString();
                            //dr["原ERP物料编号"] = drr["原ERP物料编号"].ToString();
                            dr["销售备注"] = drr["销售备注"].ToString();
                            dr["资产编码起"] = drr["资产编码起"].ToString();
                            dr["资产编码止"] = drr["资产编码止"].ToString();

                            try
                            {
                                string sqll = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}' and 仓库号='{1}'", drr["物料编码"].ToString(), drr["仓库号"]);
                                DataTable dtt = new DataTable();
                                SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
                                daa.Fill(dtt);
                                if (dtt.Rows.Count == 0)
                                {
                                    dr["库存总数"] = 0;
                                }
                                else
                                {
                                    dr["库存总数"] = dtt.Rows[0]["库存总数"].ToString();
                                }
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        DataRow[] ds = dtP.Select(string.Format("出库通知单明细号 = '{0}'", drr["出库通知单明细号"].ToString()));
                        if (ds.Length > 0)
                        {
                            ds[0].Delete();
                        }
                        if (dtP.Rows.Count == 0)
                        {
                            txt_客户.Text = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "出库_dt_待办_ColumnChanged");
            }
        }

        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
            if (dr == null) return;
            textBox2.Text = dr["送货方式"].ToString();
            textBox3.Text = dr["车号"].ToString();
            textBox4.Text = dr["快递公司ID"].ToString();
            textBox1.Text = dr["送货地址"].ToString();
            textBox5.Text = dr["备注"].ToString();



            fun_载入待办(dr["出库通知单号"].ToString());
            string sql = @"select base.*,a.原ERP物料编号,b.库存总数,b.仓库号,b.仓库名称
                            ,[资产编码起],[资产编码止] from 销售记录成品出库单明细表 base
                            left join 基础数据物料信息表 a on   base.物料编码 = a.物料编码  
                        left join 仓库物料数量表 b on  b.物料编码 = a.物料编码 
                         left  join 销售箱贴信息维护表 c on   c.销售订单明细号= base.销售订单明细号    where 1<>1";

            dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gcP.DataSource = dtP;
            checkBox2.Checked = false;
        }

        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void checkBox2_CheckStateChanged(object sender, EventArgs e)
        {

            if (checkBox2.CheckState == CheckState.Checked && dt_待办.Rows.Count > 0)
            {
                foreach (DataRow dr in dt_待办.Rows)
                {
                    dr["选择"] = true;
                    gv_待办.FocusedRowHandle = gv_待办.LocateByDisplayText(0, gridColumn3, dr["出库通知单明细号"].ToString());
                    repositoryItemCheckEdit1_CheckedChanged(null, null);

                }
            }
        }

        private void gvP_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);
                if (e.Column.FieldName == "仓库号")
                {
                    dr["仓库号"] = e.Value;
                    DataRow[] ds = dt_仓库.Select(string.Format("仓库号 = '{0}'", dr["仓库号"]));
                    dr["仓库名称"] = ds[0]["仓库名称"];
                    //dr["仓库名称"] = sr["仓库名称"].ToString();
                    string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_物料数量 = new DataTable();
                    da.Fill(dt_物料数量);
                    if (dt_物料数量.Rows.Count == 0)
                    {
                        dr["库存总数"] = 0;
                        // dr["有效总数"] = 0;
                    }
                    else
                    {
                        dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
                        //  dr["有效总数"] = dt_物料数量.Rows[0]["有效总数"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                    gridControl1.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);

            }
        }
    }
}
