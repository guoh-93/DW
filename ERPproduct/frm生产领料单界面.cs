using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using ItemInspection;
using System.IO;
using System.Threading;

namespace ERPproduct
{
    public partial class frm生产领料单界面 : UserControl
    {
        #region 变量
        string strconn = CPublic.Var.strConn;
        DataTable dt_左;
        //DataView dv_左;
        DataTable dt_右;

        string str_领料单号;
        //DataTable dt_StockDt;
        //DataTable dt_员工;
        DataTable dt_仓库;
        DataTable dt_仓库号;
        string sql_ck = "";
        //Thread thDo;
        string cfgfilepath = "";
        #endregion

        #region 加载
        public frm生产领料单界面()
        {

            InitializeComponent();

        }

#pragma warning disable IDE1006 // 命名样式
        private void frm生产领料单界面_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
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
                barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                //////sdm  特殊需求
                string userid = CPublic.Var.LocalUserID;
                string localusr = CPublic.Var.LocalUserTeam;
                //        CPublic.Var.LocalUserID == "910055" || CPublic.Var.LocalUserTeam == "管理员权限"
                if (userid == "910055" || localusr == "管理员权限" || userid == "910276" || userid == "910244" || userid == "910173" || userid == "910523")
                {
                    userid = "admin";
                }
                sql_ck = string.Format("select * from 人员仓库对应表 where 工号='{0}'", userid);
                dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ck, strconn);
                if (CPublic.Var.localUser部门编号 != "00010602")
                {
                    barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                }
                fun_load();
                fun_下拉框();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void fun_下拉框()
        {
            dt_仓库号 = new DataTable();
            string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别' and 布尔字段5 = 1"; // 布尔字段5 可发料
            SqlDataAdapter da = new SqlDataAdapter(sql4, strconn);
            da.Fill(dt_仓库号);
            repositoryItemSearchLookUpEdit2.DataSource = dt_仓库号;
            repositoryItemSearchLookUpEdit2.DisplayMember = "仓库号";
            repositoryItemSearchLookUpEdit2.ValueMember = "仓库号";
        }
        #endregion

        #region 界面操作
        //刷新操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            txt_lingliaodan.Text = "";
            textBox8.Text = "";

            textBox9.Text = "";
            txt_lingliaorenName.Text = "";
            txt_lingliaoyongtu.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            string userid = CPublic.Var.LocalUserID;
            string localusr = CPublic.Var.LocalUserTeam;
            if (userid == "910055" || localusr == "管理员权限" || userid == "910276" || userid == "910244" || userid == "910173" || userid == "910523")
            {
                userid = "admin";
            }

            sql_ck = string.Format("select * from 人员仓库对应表 where 工号='{0}'", userid);
            dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ck, strconn);
            fun_load();

        }

        DataTable dt_出库批次 = new DataTable();
#pragma warning disable IDE1006 // 命名样式
        private void fun_出库批次()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string slq = "";
                foreach (DataRow r in dt_右.Rows)
                {
                    slq = slq + " 物料编码 = '" + r["物料编码"].ToString() + "' or";
                }
                slq = slq.Substring(0, slq.Length - 2);
                string sqll = "select * from 领料出库批次记录表 where (" + slq + ") and 计算数量 > 0 order by 日期";
                SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
                new SqlCommandBuilder(daa);
                daa.Fill(dt_出库批次);

                foreach (DataRow r in dt_右.Rows)
                {
                    int icount = 0; //仓库只要显示3个批次    17/1/6
                    if (r["选择"].ToString().ToLower() == "true")
                    {
                        DataRow[] dt_批次记录 = dt_出库批次.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString()));

                        Decimal dec_qty = Convert.ToDecimal(r["输入领料数量"]);
                        string str = string.Format("物料{0}本次出库需出：", r["物料编码"].ToString());
                        if (dt_批次记录.Length > 0)
                        {
                            foreach (DataRow dr in dt_批次记录)
                            {
                                if (Convert.ToDecimal(dr["计算数量"]) < dec_qty)
                                {
                                    dec_qty = dec_qty - Convert.ToDecimal(dr["计算数量"]);
                                    str = str + "入库单号：" + dr["入库单号"].ToString() + "-" + "日期：" + dr["日期"].ToString() + "-" + "数量：" + dr["计算数量"].ToString() + "；";
                                    if (++icount > 3)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    str = str + "入库单号：" + dr["入库单号"].ToString() + "-" + "日期：" + dr["日期"].ToString() + "-" + "数量：" + dec_qty.ToString() + "；";
                                    dr["计算数量"] = Convert.ToDecimal(dr["计算数量"]) - dec_qty;

                                    break;
                                }
                            }
                            r["备注1"] = str;
                        }
                        else
                        {
                            r["备注1"] = string.Format("物料{0}目前没有先进先出数据", r["物料编码"].ToString());
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //private DataTable fun_库存(DataTable T)
        //{
        //    DataTable dt = new DataTable();
        //    foreach (DataRow dr in T.Rows)
        //    {
        //        if (dr["选择"].Equals(true))
        //        {
        //            string sql = string.Format("select * from 仓库物料数量表 where 物料编码='{0}'", dr["物料编码"].ToString());
        //            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
        //            {
        //                da.Fill(dt);
        //            }
        //            DataRow[] x = dt.Select(string.Format("物料编码='{0}'", dr["物料编码"].ToString()));
        //            x[0]["库存总数"] = Convert.ToDecimal(x[0]["库存总数"]) - Convert.ToDecimal(dr["输入领料数量"].ToString());
        //            x[0]["出入库时间"] = CPublic.Var.getDatetime();

        //        }

        //    }

        //    return dt;
        //}


        //生效
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            gv_sclldetail.CloseEditor();
            this.BindingContext[dt_右].EndCurrentEdit();
            try
            {

                DataRow rr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                if (rr == null) throw new Exception("没有记录可生效");


                if (MessageBox.Show(string.Format("确定生效领料单？"), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    fun_check();
                    //要将单位换算成标准单位,根据单位换算表
                    //foreach (DataRow dr in dt_右.Rows)
                    //{
                    //    if (dr["选择"].Equals(true))
                    //    {
                    //        string str_物料编码 = dr["物料编码"].ToString();
                    //        decimal dec = Convert.ToDecimal(dr["输入领料数量"]);
                    //        string str_待领料单号 = textBox13.Text;
                    //        string str_工单号 = textBox2.Text;

                    //    }
                    //}
                    //fun_出库批次(); string str = "";
                    DataSet ds_1 = fun_save();

                    //foreach (DataRow r in dt_右.Rows)
                    //{
                    //    if (r["选择"].ToString().ToLower() == "true")
                    //    {
                    //        str = str + r["备注1"].ToString() + "\n";
                    //    }
                    //}
                    //MessageBox.Show(str);
                    DataTable dt_2 = fun_完成状态();
                    DataTable dt_xxx = dt_右.Copy(); // 2018-9-18
                    //DataRow []rr=  dt_xxx.Select("单位换算标识=true");
                    //if (rr.Length > 0)
                    //{
                    //    foreach (DataRow vv in rr)
                    //    {
                    //        string ss = string.Format("select  * from 计量单位换算表 where 物料编码='{0}'", vv["物料编码"]);
                    //        using (SqlDataAdapter aa = new SqlDataAdapter(ss, strconn))
                    //        {
                    //            DataTable tt = new DataTable();
                    //            aa.Fill(tt);
                    //            DataRow[] r1 = tt.Select(string.Format("计量单位='{0}'", vv["bom单位"].ToString().Trim()));
                    //            DataRow[] r2 = tt.Select(string.Format("计量单位='{0}'", vv["库存单位"].ToString().Trim()));
                    //            decimal dec = Convert.ToDecimal(r1[0]["换算率"]) / Convert.ToDecimal(r2[0]["换算率"]);   // 例 1公斤 =5882 米      dec=1/5882
                    //            vv["输入领料数量"] = Convert.ToDecimal(vv["输入领料数量"]) * dec;
                    //        }
                    //    }
                    //}
                    DataTable dt = fun_save出入库明细(dt_xxx);
                    DataSet ds_2 = fun_save车间虚拟库存();
                    //  DataTable dt_库存 = fun_库存(dt_右);

                    DataView dv = new DataView(dt_xxx);
                    dv.RowFilter = "选择=1";
                    DataTable temp = dv.ToTable();
                    temp.Columns["输入领料数量"].ColumnName = "数量";
                    DataTable dt_库存 = ERPorg.Corg.fun_库存(-1, temp);
                    string sql_领料主表 = "select * from 生产记录生产领料单主表 where 1<>1";
                    string sql_领料明细表 = "select * from 生产记录生产领料单明细表 where 1<>1";
                    string sql_出入库明细 = "select * from 仓库出入库明细表 where 1<>1";
                    string sql_虚拟主表 = "select * from 生产记录车间虚拟库存表 where 1<>1";
                    string sql_虚拟明细表 = "select * from 生产记录车间虚拟库存明细表 where 1<>1";
                    string sql_完成状态 = "select * from  生产记录生产工单待领料主表 where 1<>1";
                    string s_待细 = "select * from 生产记录生产工单待领料明细表 where 1<>1";
                    string sqll = "select * from 领料出库批次记录表 where 1<>1";
                    string s = "select * from 仓库物料数量表 where 1<>1";


                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("生产领料");
                    try
                    {
                        SqlCommand cmm_1 = new SqlCommand(sql_领料主表, conn, ts);
                        SqlCommand cmm_2 = new SqlCommand(sql_领料明细表, conn, ts);
                        SqlCommand cmm_3 = new SqlCommand(sql_出入库明细, conn, ts);
                        SqlCommand cmm_4 = new SqlCommand(sql_虚拟主表, conn, ts);
                        SqlCommand cmm_5 = new SqlCommand(sql_虚拟明细表, conn, ts);
                        SqlCommand cmm_6 = new SqlCommand(sql_完成状态, conn, ts);
                        SqlCommand cmm_7 = new SqlCommand(sqll, conn, ts);
                        SqlCommand cmm_8 = new SqlCommand(s, conn, ts);
                        SqlCommand cmm_9 = new SqlCommand(s_待细, conn, ts);
                        SqlDataAdapter da_领料主表 = new SqlDataAdapter(cmm_1);
                        SqlDataAdapter da_领料明细表 = new SqlDataAdapter(cmm_2);
                        SqlDataAdapter da_出入库明细 = new SqlDataAdapter(cmm_3);
                        SqlDataAdapter da_虚拟主表 = new SqlDataAdapter(cmm_4);
                        SqlDataAdapter da_虚拟明细表 = new SqlDataAdapter(cmm_5);
                        SqlDataAdapter da_完成状态 = new SqlDataAdapter(cmm_6);
                        SqlDataAdapter daa = new SqlDataAdapter(cmm_7);
                        SqlDataAdapter da_ck = new SqlDataAdapter(cmm_8);
                        SqlDataAdapter da_待细 = new SqlDataAdapter(cmm_9);
                        new SqlCommandBuilder(da_领料主表);
                        new SqlCommandBuilder(da_领料明细表);
                        new SqlCommandBuilder(da_出入库明细);
                        new SqlCommandBuilder(da_虚拟主表);
                        new SqlCommandBuilder(da_虚拟明细表);
                        new SqlCommandBuilder(da_完成状态);
                        new SqlCommandBuilder(daa);
                        new SqlCommandBuilder(da_ck);
                        new SqlCommandBuilder(da_待细);

                        da_领料主表.Update(ds_1.Tables[0]);
                        da_领料明细表.Update(ds_1.Tables[1]);
                        da_出入库明细.Update(dt);
                        da_虚拟主表.Update(ds_2.Tables[0]);
                        da_虚拟明细表.Update(ds_2.Tables[1]);
                        if (dt_2 != null)
                            da_完成状态.Update(dt_2);
                        da_待细.Update(dt_右);
                        daa.Update(dt_出库批次);
                        da_ck.Update(dt_库存);

                        ts.Commit();

                        // stockcore 中函数                          
                        fun_save_zf();
                        MessageBox.Show("生效成功");
                        //if (checkBox1.Checked.Equals(true))
                        //{
                        //    fun_打印();
                        //}
                        //MessageBox.Show("请再次核对物料！");
                        barLargeButtonItem1_ItemClick(null, null);
                    }

                    catch (Exception ex)
                    {
                        ts.Rollback();

                        CZMaster.MasterLog.WriteLog(ex.Message);
                        throw new Exception("领料生效失败,刷新后重试");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                barLargeButtonItem1_ItemClick(null, null);


            }
        }

        //public void Start()
        //{
        //    thDo = new Thread(fun_打印);
        //    thDo.IsBackground = true;
        //    thDo.Start();
        //}
        //保存操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (MessageBox.Show("确认保存吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    fun_check();
                    fun_save();
                    MessageBox.Show("保存成功");
                }   //barLargeButtonItem1_ItemClick(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //关闭界面的操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

        //单据关闭
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }
        #endregion

        #region  函数
#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            if (CPublic.Var.localUser部门编号 == "00010602" || CPublic.Var.LocalUserTeam == "管理员权限" || CPublic.Var.LocalUserID == "admin")
            {
                barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            else
            {
                barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
            sql_ck = "and  dlmx.仓库号  in(";
            string sql_左 = "";
            if (dt_仓库.Rows.Count == 0 && (CPublic.Var.LocalUserID == "admin" || CPublic.Var.LocalUserID == "910055" || CPublic.Var.LocalUserTeam == "管理员权限" || CPublic.Var.LocalUserID == "910276"))
            {
                //  left join  基础数据物料信息表 base on base.物料编码= dlz.物料编码
                sql_左 = string.Format(@"select dlz.*,bm.部门名称  from 生产记录生产工单待领料主表 dlz
                                left join 生产记录生产工单表 gd  on gd.生产工单号 =dlz.生产工单号 
                                left join (select  属性值 部门名称 ,属性字段1 部门编号 from 基础数据基础属性表 where 属性类别 ='生产车间') bm on bm.部门编号= dlz.生产车间
                                where 待领料单号 in(select 待领料单号 from 生产记录生产工单待领料明细表 dlmx where 完成=0   group by 待领料单号 ) and gd.状态=0  and dlz.关闭=0");
            }
            else
            {
                if (dt_仓库.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt_仓库.Rows)
                    {
                        sql_ck = sql_ck + string.Format("'{0}',", dr["仓库号"]);

                    }
                    sql_ck = sql_ck.Substring(0, sql_ck.Length - 1) + ")";
                }
                else
                {
                    sql_ck = "";
                    throw new Exception("未找到你所管的仓库,请确认");

                }
                // --left join  基础数据物料信息表 base on base.物料编码= dlz.物料编码
                sql_左 = string.Format(@"select dlz.*,bm.部门名称 from 生产记录生产工单待领料主表 dlz
                   
                    left join(select  属性值 部门名称 ,属性字段1 部门编号 from 基础数据基础属性表 where 属性类别 ='生产车间') bm on bm.部门编号= dlz.生产车间
                    left join 生产记录生产工单表  sc on sc.生产工单号=dlz.生产工单号                           
                    where 待领料单号 in(select 待领料单号 from 生产记录生产工单待领料明细表 dlmx where 完成=0  {0}  group by 待领料单号 ) and dlz.关闭=0 and sc.状态=0 ", sql_ck);
            }
            using (SqlDataAdapter da = new SqlDataAdapter(sql_左, strconn))
            {
                dt_左 = new DataTable();

                da.Fill(dt_左);

                gridControl1.DataSource = dt_左;
            }

            string sql_右 = "select * from 生产记录生产工单待领料明细表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_右, strconn))
            {
                dt_右 = new DataTable();
                da.Fill(dt_右);

                dt_右.Columns.Add("选择", typeof(bool));
                dt_右.Columns.Add("输入领料数量");
                gc_sclldetail.DataSource = dt_右;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_打印()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataView dv_dy = new DataView(dt_右);
                dv_dy.RowFilter = "选择=1";
                DataTable dt_dy = new DataTable();
                dt_dy = dv_dy.ToTable();

                int count = 1;
                if (dt_dy.Rows.Count % 15 != 0)
                {
                    count = (dt_dy.Rows.Count / 15) + 1;
                }
                else
                {
                    count = dt_dy.Rows.Count / 15;
                }
                PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                this.printDialog1.Document = this.printDocument1;
                DialogResult dr = this.printDialog1.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    //string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                    //SetDefaultPrinter(PrinterName);
                    string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                    DataRow rr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                    print_FMS.fun_p_领料A5(rr, dt_dy, count, PrinterName, false);

                }
            }
            catch (Exception ex)
            {

                System.Threading.Thread.CurrentThread.Abort();
                MessageBox.Show(ex.Message);
            }
        }

        [DllImport("winspool.drv")]
        public static extern bool SetDefaultPrinter(String Name); //调用win api将指定名称的打印机设置为默认打印机

#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {
            int i = 0;
            string s = string.Format("select  * from 生产记录生产工单表 where 状态=0 and 生产工单号='{0}'", textBox2.Text.Trim());
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            if (dt.Rows.Count == 0) throw new Exception("改工单状态已更改,需要停止发料");

            foreach (DataRow r in dt_右.Rows)
            {
                if (r["选择"].Equals(true))
                {
                    try
                    {
                        decimal a = Convert.ToDecimal(r["输入领料数量"]);
                        if (a < 0)
                        {
                            throw new Exception();
                        }
                    }
                    catch
                    {
                        throw new Exception("请正确输入领料数量格式");
                    }
                    string sql = string.Format("select * from 仓库物料数量表 where  物料编码='{0}' and 仓库号='{1}' ", r["物料编码"].ToString(), r["仓库号"].ToString());

                    DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql, strconn);
                    if (dr == null || dr["库存总数"].ToString() == "")
                        throw new Exception("库存总数不足！");

                    if (Convert.ToDecimal(r["输入领料数量"]) > Convert.ToDecimal(dr["库存总数"]))
                    {
                        throw new Exception("库存总数不足！");
                    }
                    if (Convert.ToDecimal(r["输入领料数量"]) > Convert.ToDecimal(r["未领数量"]))
                    {
                        i++;
                    }
                    if (r["仓库号"].ToString() == "")
                    {
                        throw new Exception("仓库号必填！");
                    }
                    DataRow[] ds = dt_仓库号.Select(string.Format("仓库号 = '{0}'", r["仓库号"].ToString()));
                    if (ds.Length == 0)
                    {
                        throw new Exception("仓库号不对！");
                    }

                }



            }
            if (i > 0)
            {
                if (MessageBox.Show("领料数量大于未领数量，是否继续？", "提醒", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    throw new Exception("请修改");
                }
            }

        }

        DataTable dt_领料出库明细;
#pragma warning disable IDE1006 // 命名样式
        private DataSet fun_save()
#pragma warning restore IDE1006 // 命名样式
        {
            DataSet ds = new DataSet();
            DateTime t = CPublic.Var.getDatetime();
            string str_id = CPublic.Var.LocalUserID;
            string str_name = CPublic.Var.localUserName;


            if (txt_lingliaodan.Text == "")  //新建的 领料出库单
            {

                str_领料单号 = string.Format("ML{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                    CPublic.CNo.fun_得到最大流水号("ML", t.Year, t.Month));
                //保存 主表
                string sql = "select * from 生产记录生产领料单主表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    txt_lingliaodan.Text = str_领料单号;
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    DataRow dr = dt.NewRow();
                    dt.Rows.Add(dr);
                    dr["GUID"] = System.Guid.NewGuid();
                    //dr["领料仓库ID"] = txt_cangkuID.EditValue;
                    //dr["领料仓库"] = txt_cangkuName.Text;
                    dr["领料人员ID"] = textBox9.Text;
                    dr["领料人员"] = txt_lingliaorenName.Text;
                    dr["创建日期"] = t;
                    dr["修改日期"] = t;
                    dr["领料类型"] = "工单领料";
                    dr["生效"] = true;
                    dr["生效人员"] = str_name;
                    dr["生效人员ID"] = str_id;
                    dr["生效日期"] = t;

                    //dr["生产制令单号"] = str_领料单号;
                    dataBindHelper1.DataToDR(dr);
                    dt.TableName = "主表";
                    ds.Tables.Add(dt);
                    //new SqlCommandBuilder(da);
                    //da.Update(dt);
                }
                string sql1 = "select * from 生产记录生产领料单明细表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql1, strconn))
                {
                    dt_领料出库明细 = new DataTable();
                    da.Fill(dt_领料出库明细);
                    //new SqlCommandBuilder(da);

                    //DataTable dt = dv_右.ToTable();
                    int pos = 1;
                    foreach (DataRow r in dt_右.Rows)
                    {
                        if (r["选择"].Equals(true))
                        {
                            DataRow dr = dt_领料出库明细.NewRow();
                            dt_领料出库明细.Rows.Add(dr);
                            dr["GUID"] = System.Guid.NewGuid();
                            dr["领料出库单号"] = str_领料单号;
                            dr["POS"] = pos.ToString("00");
                            dr["领料出库明细号"] = str_领料单号 + "-" + pos.ToString("00");
                            dr["待领料单明细号"] = r["待领料单明细号"];
                            dr["待领料单明细号"] = r["待领料单明细号"];
                            dr["领料仓库"] = r["仓库名称"];
                            dr["领料仓库ID"] = r["仓库号"];

                            //dr["领料仓库"] = txt_cangkuName.Text;
                            dr["生产工单号"] = textBox2.Text;
                            dr["生产工单类型"] = textBox3.Text;
                            dr["生产制令单号"] = textBox6.Text;
                            dr["生产车间"] = r["生产车间"];
                            dr["规格型号"] = r["规格型号"];

                            dr["物料名称"] = r["物料名称"];
                            dr["物料编码"] = r["物料编码"];
                            dr["工单负责人"] = textBox7.Text;
                            dr["领料数量"] = r["输入领料数量"];
                            //这里不需要 已领数量和未领数量
                            dr["生效"] = true;
                            dr["生效人员ID"] = str_id;
                            dr["生效人员"] = str_name;
                            dr["生效日期"] = t;
                            dr["领料人员ID"] = textBox9.Text;
                            dr["领料人员"] = txt_lingliaorenName.Text;
                            dr["操作人员ID"] = str_id;
                            dr["操作人员"] = str_name;
                            dr["创建日期"] = t;
                            dr["备注1"] = r["备注1"];
                            pos++;
                        }
                    }
                    dt_领料出库明细.TableName = "明细表";
                    ds.Tables.Add(dt_领料出库明细);
                    //new SqlCommandBuilder(da);
                    //da.Update(dt);
                }
            }
            else    //  从列表界面跳转过来的未生效的出库单 并修改 领料出库单
            {
                string sql = string.Format("select * from 生产记录生产领料单主表 where 领料出库单号='{0}'", str_领料单号);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    DataRow dr = dt.Rows[0];
                    dr["修改日期"] = t;
                    dataBindHelper1.DataToDR(dr);
                    dt.TableName = "主表";
                    ds.Tables.Add(dt);
                    //new SqlCommandBuilder(da);
                    //da.Update(dt);
                }
                string sql1 = "select * from 生产记录生产领料单明细表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql1, strconn))
                {
                    ds.Tables.Add(dt_右);
                    //new SqlCommandBuilder(da);
                    //da.Update(dt_右);
                }
            }

            //保存 明细表

            return (ds);
        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 修改 待领料单主表中记录的 完成状态 完成则在代办事项中隐藏
        /// </summary>
        private DataTable fun_完成状态()
#pragma warning restore IDE1006 // 命名样式
        {
            DataTable dt = new DataTable();
            DataTable dt_1 = new DataTable();

            //string sql_MX = string.Format("select * from 生产记录生产工单待领料明细表 where 生产工单号='{0}'", textBox2.Text);
            string str_条件 = "";
            bool bl = true;
            foreach (DataRow dr in dt_右.Rows)
            {
                if (dr["选择"].Equals(true))
                {


                    if (Convert.ToDecimal(dr["输入领料数量"]) >= Convert.ToDecimal(dr["未领数量"]))
                    {
                        str_条件 = str_条件 + ",'" + dr["物料编码"].ToString() + "'";
                        dr["完成"] = true;
                        dr["完成日期"] = CPublic.Var.getDatetime();
                    }
                    else
                    {
                        bl = false;
                    }

                    dr["已领数量"] = Convert.ToDecimal(dr["已领数量"]) + Convert.ToDecimal(dr["输入领料数量"]);
                    dr["未领数量"] = Convert.ToDecimal(dr["未领数量"]) - Convert.ToDecimal(dr["输入领料数量"]);
                }
                else
                {
                    bl = false;
                }

            }
            if (str_条件.Length > 1)
            {
                str_条件 = str_条件.Substring(1, str_条件.Length - 1);
                str_条件 = string.Format("and 物料编码 not in ({0})", str_条件);
            }
            string sql_MX = string.Format("select * from 生产记录生产工单待领料明细表 where 待领料单号='{0}' and 完成=0  {1}", textBox13.Text, str_条件);

            using (SqlDataAdapter da = new SqlDataAdapter(sql_MX, strconn))
            {
                da.Fill(dt);
            }
            if (dt.Rows.Count > 0)
            {
                bl = false;

            }

            if (bl)
            {
                //string sql = string.Format("select * from  生产记录生产工单待领料主表 where 生产工单号='{0}' ", textBox2.Text);
                string sql = string.Format("select * from  生产记录生产工单待领料主表 where 待领料单号='{0}' ", textBox13.Text);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {

                    da.Fill(dt_1);
                    if (dt_1.Rows.Count > 0)
                    {
                        dt_1.Rows[0]["完成"] = true;
                        dt_1.Rows[0]["完成日期"] = CPublic.Var.getDatetime();
                    }

                    //new SqlCommandBuilder(da);
                    //da.Update(dt_1);

                }
            }
            return (dt_1);

        }
#pragma warning disable IDE1006 // 命名样式
        private DataSet fun_save车间虚拟库存()
#pragma warning restore IDE1006 // 命名样式
        {

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataTable dt_明细 = new DataTable();
            DateTime t = CPublic.Var.getDatetime();
            foreach (DataRow dr in dt_右.Rows)
            {
                if (dr["选择"].Equals(true))
                {
                    //保存主表
                    string sql = string.Format
                        ("select * from 生产记录车间虚拟库存表 where  生产车间='{0}'and 物料编码='{1}'",
                        textBox1.Text, dr["物料编码"]);
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                    {
                        dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0) //找到记录, 主表中只应有一条  修改
                        {
                            dt.Rows[0]["车间数量"] = Convert.ToDecimal(dt.Rows[0]["车间数量"]) + Convert.ToDecimal(dr["输入领料数量"]);
                            dt.Rows[0]["未用数量"] = Convert.ToDecimal(dt.Rows[0]["未用数量"]) + Convert.ToDecimal(dr["输入领料数量"]);
                            dt.Rows[0]["修改日期"] = t;
                        }
                        else  //没找到 新增
                        {
                            DataRow r = dt.NewRow();
                            r["GUID"] = System.Guid.NewGuid();
                            r["物料编码"] = dr["物料编码"];
                            r["物料名称"] = dr["物料名称"];
                            //r["规格型号"] = dr["规格型号"];
                            //r["图纸编号"] = dr["图纸编号"];
                            r["车间数量"] = dr["输入领料数量"];
                            r["未用数量"] = dr["输入领料数量"];
                            //r["生产工单号"] = dr["生产工单号"];
                            r["生产车间"] = textBox1.Text;
                            dt.Rows.Add(r);

                        }
                        //new SqlCommandBuilder(da);
                        //da.Update(dt);

                    }

                    // 保存明细表
                    string sql_mx = string.Format("select * from 生产记录车间虚拟库存明细表 where 物料编码='{0}'and 生产车间='{1}' and 生产工单号='{2}'",
                        dr["物料编码"], textBox1.Text, textBox2.Text);
                    using (SqlDataAdapter da = new SqlDataAdapter(sql_mx, strconn))
                    {
                        dt_明细 = new DataTable();
                        da.Fill(dt_明细);
                        if (dt_明细.Rows.Count > 0) //找到记录, 主表中只应有一条  修改
                        {
                            dt_明细.Rows[0]["领料数量"] = Convert.ToDecimal(dt_明细.Rows[0]["领料数量"]) + Convert.ToDecimal(dr["输入领料数量"]);
                            dt_明细.Rows[0]["未用数量"] = Convert.ToDecimal(dt_明细.Rows[0]["未用数量"]) + Convert.ToDecimal(dr["输入领料数量"]);
                            dt_明细.Rows[0]["修改日期"] = t;

                        }
                        else  //没找到 新增
                        {
                            DataRow r = dt_明细.NewRow();
                            //r["领料出库单号"] = dr["领料出库单号"];
                            r["物料编码"] = dr["物料编码"];
                            r["物料名称"] = dr["物料名称"];
                            //r["规格型号"] = dr["规格型号"];
                            //r["图纸编号"] = dr["图纸编号"];

                            r["领料数量"] = dr["输入领料数量"];

                            r["未用数量"] = dr["输入领料数量"];
                            r["生产工单号"] = dr["生产工单号"];
                            r["生产车间"] = textBox1.Text;
                            r["领料人"] = txt_lingliaorenName.Text;
                            //r["领料人ID"] = textBox9.Text = "";
                            r["领料人ID"] = textBox9.Text;

                            r["创建日期"] = t;
                            r["修改日期"] = t;
                            dt_明细.Rows.Add(r);

                        }

                    }
                }
            }
            ds.Tables.Add(dt);
            ds.Tables.Add(dt_明细);
            return (ds);
        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// dtt 为 dt_右 copy 然后 输入物料数量 已转换单位  
        /// </summary>
        /// <param name="dtt"></param>
        /// <returns></returns>
        private DataTable fun_save出入库明细(DataTable dtt)
#pragma warning restore IDE1006 // 命名样式
        {
            int POS = 1;
            DataTable dt = new DataTable();
            DateTime t = CPublic.Var.getDatetime();
            foreach (DataRow dr in dtt.Rows)
            {
                if (dr["选择"].Equals(true))
                {
                    string sql = "select * from 仓库出入库明细表 where 1<>1";
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                    {

                        da.Fill(dt);
                        DataRow r = dt.NewRow();
                        r["GUID"] = System.Guid.NewGuid();
                        r["明细类型"] = "领料出库";
                        r["单号"] = txt_lingliaodan.Text;
                        r["出库入库"] = "出库";
                        r["物料编码"] = dr["物料编码"];
                        r["物料名称"] = dr["物料名称"];
                        r["仓库号"] = dr["仓库号"];
                        r["仓库名称"] = dr["仓库名称"];
                        if (dr["生产工单号"].ToString().Trim() != "")  //只有正常发料和工单补料才有工单号
                        {
                            r["相关单号"] = dr["生产工单号"];
                            string ss = string.Format("select 车间名称 from 生产记录生产工单表 where 生产工单号='{0}'", dr["生产工单号"].ToString());
                            DataTable t_s = CZMaster.MasterSQL.Get_DataTable(ss, strconn);
                            r["相关单位"] = t_s.Rows[0]["车间名称"];
                        }
                        r["明细号"] = txt_lingliaodan.Text + "-" + POS.ToString("00");
                        r["实效数量"] = -(Convert.ToDecimal(dr["输入领料数量"]));
                        r["实效时间"] = t;
                        r["出入库时间"] = t;
                        r["仓库人"] = CPublic.Var.localUserName;

                        dt.Rows.Add(r);

                    }
                    POS++;

                }
            }
            return (dt);
        }
        //刷新数量 
#pragma warning disable IDE1006 // 命名样式
        private void fun_save_zf()
#pragma warning restore IDE1006 // 命名样式
        {

            foreach (DataRow dr in dt_右.Rows)
            {
                if (dr["选择"].Equals(true))
                {


                    StockCore.StockCorer.fun_物料数量_实际数量(dr["物料编码"].ToString().Trim(), dr["仓库号"].ToString(), true);



                }
            }
        }

        #endregion


#pragma warning disable IDE1006 // 命名样式
        private void gv_sclldetail_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Column.Name == "输入领料数量")
            {
                DataRow dr = gv_sclldetail.GetDataRow(gv_sclldetail.FocusedRowHandle);
                if (dt_仓库.Rows.Count > 0 && dt_仓库.Rows[0]["仓库号"] == dr["仓库号"])
                {
                    gridColumn6.OptionsColumn.AllowEdit = true;
                }
                else
                {
                    gridColumn6.OptionsColumn.AllowEdit = false;
                }
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_sclldetail_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gv_sclldetail.GetDataRow(e.RowHandle);

            if (Convert.ToDecimal(dr["库存总数"]) < Convert.ToDecimal(dr["未领数量"]))
            {
                e.Appearance.BackColor = Color.LightPink;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView2_RowCellClick_1(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow r = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                textBox5.Text = r["产品名称"].ToString();
                textBox4.Text = r["产品编码"].ToString();
                textBox2.Text = r["生产工单号"].ToString();
                textBox3.Text = r["生产工单类型"].ToString();
                textBox6.Text = r["生产制令单号"].ToString();
                textBox1.Text = r["生产车间"].ToString();
                textBox13.Text = r["待领料单号"].ToString();
                textBox11.Text = r["部门名称"].ToString();
                textBox12.Text = r["生产数量"].ToString();
                textBox14.Text = r["领料类型"].ToString();

                textBox7.Text = r["工单负责人"].ToString();
                //textBox8.Text = r["原ERP物料编号"].ToString();
                textBox9.Text = r["领料人ID"].ToString();
                textBox10.Text = r["加急状态"].ToString();
                txt_lingliaorenName.Text = r["领料人"].ToString();
                //txt_cangkuName.Text = r["仓库名称"].ToString();
                //18-9-18 此处计量单位为

                string sql = string.Format(@"select dlmx.*,未领数量 as 输入领料数量,人事基础部门表.部门名称,bom.计量单位 as bom单位,base.计量单位 as 库存单位,有效总数,单位换算标识,
                bom.主辅料,组,优先级,kc.货架描述 ,isnull(kc.库存总数,0)库存总数,kc.在途量
                from 生产记录生产工单待领料明细表 dlmx
                        left join 基础数据物料BOM表 bom on bom.子项编码= dlmx.物料编码 and bom.产品编码='{0}'
                        left join 人事基础部门表 on 人事基础部门表.部门编号= dlmx.生产车间
                        left join 基础数据物料信息表 base on base.物料编码=dlmx.物料编码
                        left join 仓库物料数量表 kc on kc.物料编码=dlmx.物料编码  and kc.仓库号=dlmx.仓库号 
                        where dlmx.待领料单号='{1}' and dlmx.完成=0 {2}  order by dlmx.物料编码", r["产品编码"].ToString(), r["待领料单号"], sql_ck);


                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    dt_右 = new DataTable();
                    da.Fill(dt_右);
                    dt_右.Columns.Add("选择", typeof(bool));

                    foreach (DataRow dr in dt_右.Rows)
                    {
                        dr["选择"] = true;

                        //dr["仓库号"] = dr["仓库号1"];
                        //dr["仓库名称"] = dr["仓库名称1"];
                        //  dr["剩余数量"] = Math.Round(Convert.ToDecimal(dr["库存总数"]) - Convert.ToDecimal(dr["输入领料数量"]), 2);


                    }
                    gc_sclldetail.DataSource = dt_右;

                    if (e != null && e.Button == MouseButtons.Right)
                    {
                        contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                        gv_sclldetail.CloseEditor();
                        this.BindingContext[dt_左].EndCurrentEdit();

                    }
                }
            }
            catch (Exception ex)
            {
                gc_sclldetail.DataSource = null;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void gridView2_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            gridView2_RowCellClick_1(null, null);

        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_sclldetail_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }

        }


#pragma warning disable IDE1006 // 命名样式
        private void barButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            //fun_打印(dt_dy);//try
            //{
            //    Start();

            //}
            //catch (Exception)
            //{
            //    if (thDo != null)
            //    {
            //        thDo.Interrupt();
            //        thDo.Abort();
            //        thDo.Join();

            //        GC.Collect();
            //    }

            //}

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView2_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //textBox16.Text = "";

                if (gridView2.GetRow(e.RowHandle) == null)
                {
                    return;
                }
                //int j = gv.RowCount;
                //for (int i = 0; i < j; i++)
                //{
                if (gridView2.GetRowCellValue(e.RowHandle, "加急状态").ToString() == "加急")
                {
                    e.Appearance.BackColor = Color.Pink;

                }
                if (gridView2.GetRowCellValue(e.RowHandle, "加急状态").ToString() == "急")
                {

                    e.Appearance.BackColor = Color.MistyRose;
                }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void textBox13_TextChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView2_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView2.GetFocusedRowCellValue(gridView2.FocusedColumn));
                e.Handled = true;
            }
        }

        private void 完成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认是否完成", "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {



                    foreach (DataRow dr in dt_右.Rows)
                    {

                        //减去 未领量和 库存锁定量
                        string sql = string.Format(@"update  仓库物料数量表 set 未领量=未领量-'{0}' where 物料编码='{1}'",
                            Convert.ToDecimal(dr["未领数量"]), dr["物料编码"].ToString());
                        CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
                        string sql_1 = string.Format(@"update 生产记录生产工单待领料明细表 set 完成=1,完成日期='{0}' where 待领料单明细号='{1}'",
                                                    Convert.ToDateTime(CPublic.Var.getDatetime().ToString("yyyy-MM-dd HH:mm:ss")), dr["待领料单明细号"].ToString());
                        CZMaster.MasterSQL.ExecuteSQL(sql_1, strconn);


                    }
                    //搜索 该待领料单 所有明细是否完成 若完成 主表赋为完成
                    DataRow r = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                    string sql_mx = string.Format("select * from 生产记录生产工单待领料明细表   where   待领料单号='{0}'", r["待领料单号"].ToString());
                    using (SqlDataAdapter da = new SqlDataAdapter(sql_mx, strconn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        int i = 0;
                        foreach (DataRow rr in dt.Rows)
                        {
                            if (rr["完成"].Equals(true))
                            {
                                i++;
                            }
                        }
                        if (i >= dt.Rows.Count)// 全部完成则 待领料主表赋为完成
                        {
                            string sql_z = string.Format("update 生产记录生产工单待领料主表 set 完成=1,完成日期='{0}' where  待领料单号='{1}'"
                                    , Convert.ToDateTime(CPublic.Var.getDatetime().ToString("yyyy-MM-dd HH:mm:ss")), r["待领料单号"].ToString());
                            CZMaster.MasterSQL.ExecuteSQL(sql_z, strconn);
                        }
                    }

                    fun_load();
                }
            }
            catch (Exception ex)
            {

                CZMaster.MasterLog.WriteLog(ex.Message);
                MessageBox.Show("关闭失败，请联系信息部");
            }


        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (dt_右.Rows.Count == 0) throw new Exception("没有任何明细");

                DataView dv_dy = new DataView(dt_右);
                dv_dy.RowFilter = "选择=1";
                DataTable dt_dy = new DataTable();
                dt_dy = dv_dy.ToTable();

                DataRow rr = gridView2.GetDataRow(gridView2.FocusedRowHandle);

                DataTable dt_表头 = new DataTable();
                dt_表头.Columns.Add("领料出库单号", typeof(string));
                dt_表头.Columns.Add("编号", typeof(string));
                dt_表头.Columns.Add("物料号", typeof(string));
                dt_表头.Columns.Add("规格", typeof(string));
                dt_表头.Columns.Add("物料名称", typeof(string));
                dt_表头.Columns.Add("生产数量", typeof(decimal));
                dt_表头.Columns.Add("领用部门", typeof(string));
                dt_表头.Columns.Add("领用人", typeof(string));
                dt_表头.Columns.Add("申请人", typeof(string));
                dt_表头.Columns.Add("仓管员", typeof(string));
                dt_表头.Columns.Add("日期", typeof(DateTime));

                DataRow dr = dt_表头.NewRow();
                dr["编号"] = rr["生产工单号"];
                dr["物料号"] = rr["产品编码"];
                dr["规格"] = rr["规格型号"];

                dr["物料名称"] = rr["产品名称"];

                dr["生产数量"] = rr["生产数量"];
                // dr["领用部门"] = "dsa13123";
                dr["领用人"] = rr["领料人ID"].ToString() + "  " + rr["领料人"].ToString();
                dr["申请人"] = rr["制单人员"].ToString();

                //dr["仓管员"] = "dsa13123";
                dr["日期"] = DateTime.Now.ToString();
                dr["领料出库单号"] = rr["待领料单号"].ToString();
                dt_表头.Rows.Add(dr);



                //  DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                ERPreport.frm发料打印 frm = new ERPreport.frm发料打印(dt_dy, dt_表头, rr);
                frm.ShowDialog();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




            //try
            //{
            //    if (dt_右.Rows.Count == 0) throw new Exception("没有任何明细");

            // fun_打印();
            //}
            //catch (Exception ex)
            //{

            //    MessageBox.Show(ex.Message);
            //} 

        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {






        }

        private void repositoryItemSearchLookUpEdit2View_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {


        }

        private void gv_sclldetail_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow dr = gv_sclldetail.GetDataRow(gv_sclldetail.FocusedRowHandle);
            if (e.Column.FieldName == "仓库号")
            {
                dr["仓库号"] = e.Value;
                DataRow[] ds = dt_仓库号.Select(string.Format("仓库号 = '{0}'", dr["仓库号"]));
                dr["仓库名称"] = ds[0]["仓库名称"];
                //dr["仓库名称"] = sr["仓库名称"].ToString();
                string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                DataTable dt_物料数量 = new DataTable();
                da.Fill(dt_物料数量);
                if (dt_物料数量.Rows.Count == 0)
                {
                    dr["库存总数"] = 0;
                    dr["有效总数"] = 0;
                    dr["货架描述"] = "";
                }
                else
                {
                    dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
                    dr["有效总数"] = dt_物料数量.Rows[0]["有效总数"];
                    dr["货架描述"] = dt_物料数量.Rows[0]["货架描述"];//19-9-17解决货架更新
                }
            }
        }

        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    //DataTable tt = dtM.Copy();
                    //tt.Columns.Remove("作废");
                    gridControl1.ExportToXlsx(saveFileDialog.FileName);
                    //ERPorg.Corg.TableToExcel(tt, saveFileDialog.FileName);
                    MessageBox.Show("导出成功");
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        //19-12-25 加
        private void button1_Click(object sender, EventArgs e)
        {
            string s = string.Format(@"select dlz.*,bm.部门名称  from 生产记录生产工单待领料主表 dlz
                                left join 生产记录生产工单表 gd  on gd.生产工单号 =dlz.生产工单号 
                                left join (select  属性值 部门名称 ,属性字段1 部门编号 from 基础数据基础属性表 where 属性类别 ='生产车间') bm on bm.部门编号= dlz.生产车间
         where   待领料单号 in  (
           select b.待领料单号   from 生产记录生产工单待领料明细表 a
            left join 生产记录生产工单待领料主表 b on a.待领料单号 = b.待领料单号
            left join 生产记录生产工单表 gd  on gd.生产工单号 = b.生产工单号
            where a.完成 = 0 and b.关闭 = 0  and a.物料编码 = '{0}' and gd.状态 = 0  group by b.待领料单号)  ", textBox16.Text.Trim());
            dt_左 = CZMaster.MasterSQL.Get_DataTable(s,strconn);
            gridControl1.DataSource = dt_左;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                foreach(DataRow r in  dt_右.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    r["选择"] = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataRow r in dt_右.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    r["选择"] = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




        //左边勾选待领料 赋值到右边 领料出库
        //private void repositoryItemCheckEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        //{
        //    DataRow r = gridView2.GetDataRow(gridView2.FocusedRowHandle);
        //    if (e.NewValue.Equals(true))
        //    {

        //        DataRow dr = dt_右.NewRow();
        //        dr["GUID"] = System.Guid.NewGuid();
        //        dr["待领料单号"] = r["待领料单号"];
        //        textBox2.Text = r["生产工单号"].ToString();
        //        textBox3.Text = r["生产工单类型"].ToString();
        //        dr["生产制令单号"] = r["生产制令单号"];
        //        dr["生产工单号"] = r["生产工单号"];
        //        dr["生产工单类型"] = r["生产工单类型"];
        //        dr["生产车间"] = r["生产车间"];
        //        dr["物料编码"] = r["物料编码"];
        //        dr["物料名称"] = r["物料名称"];
        //        dr["规格型号"] = r["规格型号"];
        //        dr["原规格型号"] = r["原规格型号"];
        //        dr["图纸编号"] = r["图纸编号"];

        //        dr["创建日期"] = System.DateTime.Now;
        //        dt_右.Rows.Add(dr);
        //    }
        //    else
        //    {
        //       DataRow  []dr = dt_右.Select(string.Format("生产工单号='{0}'and 物料编码='{1}'",r["生产工单号"].ToString(),r["物料编码"].ToString()));
        //       if (dr.Length > 0)
        //       {
        //           dr[0].Delete();
        //       }
        //       int i = 0;
        //       foreach (DataRow rr in dv_左.ToTable().Rows)
        //       {
        //           if (rr["选择"].Equals(true))
        //           {
        //               i++;
        //           }
        //       }
        //       if (i == 0)
        //       {
        //           textBox2.Text = "";
        //       }
        //    }

        //    gc_sclldetail.DataSource = dt_右;
        //}




        //结束
    }
}
