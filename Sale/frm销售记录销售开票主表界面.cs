using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace ERPSale
{
    public partial class frm销售记录销售开票主表界面 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        DataRow drM;
        string str_选择条件 = "";
        DataTable dt_mx;
        DataTable t_片区 = ERPorg.Corg.fun_业务员片区(CPublic.Var.localUserName);
        string cfgfilepath = "";
        #endregion

        #region 自用类
        public frm销售记录销售开票主表界面()
        {
            InitializeComponent();
        }

        private void frm销售记录销售开票主表界面_Load(object sender, EventArgs e)
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
                DateTime t = CPublic.Var.getDatetime().Date;
                bar_日期后.EditValue = Convert.ToDateTime(t.ToString("yyyy-MM-dd"));
                bar_日期前.EditValue = Convert.ToDateTime(t.AddMonths(-2).ToString("yyyy-MM-dd"));
                bar_记录状态.EditValue = "全部";
                fun_载入开票票单();

                if (CPublic.Var.LocalUserTeam != "营销助理主管权限" && CPublic.Var.LocalUserTeam != "营销助理权限"
                    && CPublic.Var.LocalUserTeam != "管理员权限" && CPublic.Var.LocalUserTeam != "财务部权限")
                {
                    gridColumn7.Visible = false;
                    gridColumn8.Visible = false;
                    gridColumn14.Visible = false;
                    gridColumn15.Visible = false;
                    gridColumn16.Visible = false;
                    gridColumn17.Visible = false;


                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_载入明细(string str_开票单号)
        {
            try
            {
                //正常的 单子
                string sql = string.Format(@"select  x.*,税率 from (
                select a.*,e.销售订单明细号,d.客户名,f.出库通知单号,left(a.成品出库单明细号,14)成品出库单号,客户订单号,e.备注
             from 销售记录销售开票明细表 a           
             left join 销售记录销售出库通知单明细表 f on f.出库通知单明细号=a.出库通知单明细号
             left join 基础数据物料信息表 c on c.物料编码=a.产品编码
             left join 销售记录销售订单明细表 e  on  e.销售订单明细号=f.销售订单明细号
             left join  销售记录销售订单主表 d on d.销售订单号=e.销售订单号       ) x
             left  join 销售记录销售开票主表 y  on y.开票票号=x.销售开票通知单号  where x.销售开票通知单号 = '{0}'", str_开票单号);
                dt_mx = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_mx);

                //退货的 退金额的 记录 存在 L销售记录成品出库单明细表L 里
                sql = string.Format(@"select  x.*,税率 from (select a.*,销售订单明细号,b.客户,b.出库通知单号,b.成品出库单号 
                         from 销售记录销售开票明细表 a,L销售记录成品出库单明细表L b,基础数据物料信息表 c 
                        where a.成品出库单明细号=b.成品出库单明细号  and c.物料编码=a.产品编码 ) x
                         left  join 销售记录销售开票主表 y  on y.开票票号=x.销售开票通知单号 where x.销售开票通知单号= '{0}' ", str_开票单号);
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_mx);

                DataView dv = new DataView(dt_mx);
                dv.Sort = "ID";
                dt_mx = new DataTable();
                dt_mx = dv.ToTable();
                gridControl1.DataSource = dt_mx;
            }
            catch { }
        }

        private void refresh_single(string str_开票票号)
        {

            string sql = string.Format(@"select  skxzb.*,片区 from 销售记录销售开票主表 skxzb,客户基础信息表 kh  
            where skxzb.客户编号=kh.客户编号 and 作废=0 and 销售开票通知单号='{0}'", str_开票票号);
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            DataRow[] r_1 = dtM.Select(string.Format("销售开票通知单号='{0}'", str_开票票号));
            r_1[0].ItemArray = dt.Rows[0].ItemArray;

        }

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            drM = gv.GetDataRow(gv.FocusedRowHandle);
            string str_开票票号 = drM["销售开票通知单号"].ToString();
            refresh_single(str_开票票号);
            fun_载入明细(str_开票票号);
            //修改
            if(e.Clicks == 2)
            {
                //新增界面
                //if (drM["生效"].ToString() == "未生效" && "用户" == "用户")
                if (drM["提交审核"].Equals(false) && drM["作废"].Equals(false))
                {
                    frm销售记录销售开票详细界面 fm = new frm销售记录销售开票详细界面(str_开票票号, drM, dtM);
                    fm.Dock = System.Windows.Forms.DockStyle.Fill;
                    CPublic.UIcontrol.AddNewPage(fm, "销售开票");
                }
                //视图界面
                else
                {
                    frm销售记录销售开票详细界面_视图 fm = new frm销售记录销售开票详细界面_视图(str_开票票号, drM);
                    fm.Dock = System.Windows.Forms.DockStyle.Fill;
                    CPublic.UIcontrol.AddNewPage(fm, "销售开票视图");

                }
            
            }
            if(e.Button==MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc, new Point(e.X, e.Y));
            }
        }
        #endregion

        #region 方法
        private void fun_载入开票票单()
        {
            try
            {
                fun_搜索组合();
                string sql = string.Format("select  skxzb.*,片区 from 销售记录销售开票主表 skxzb,客户基础信息表 kh where skxzb.客户编号=kh.客户编号 and 作废=0 and {0}", str_选择条件);
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                gc.DataSource = dtM;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "开票主表界面_fun_载入开票票单");
            }
        }
        private void fun_取消生效(string str_开票号)
        {
            string sql = string.Format("select * from 销售记录销售开票主表 where 销售开票通知单号='{0}' and  作废=0", str_开票号);
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            dt.Rows[0]["生效"] = 0;
            dt.Rows[0]["生效日期"] = DBNull.Value;
            dt.Rows[0]["审核"] = 0;
            dt.Rows[0]["审核日期"] = DBNull.Value;
            string sql_mx = string.Format("select * from 销售记录销售开票明细表 where 销售开票通知单号='{0}'", str_开票号);
            DataTable dt_mx = new DataTable();
            dt_mx = CZMaster.MasterSQL.Get_DataTable(sql_mx, strconn);
            foreach (DataRow dr in dt_mx.Rows)
            {
                dr["生效"] = 0;
                dr["生效日期"] = DBNull.Value;

            }

            string sql_开票数 = string.Format(@"select 销售记录成品出库单明细表.*,开票数量 from 销售记录成品出库单明细表,销售记录销售开票明细表 
                where  销售记录成品出库单明细表.成品出库单明细号 in (select 成品出库单明细号 from 销售记录销售开票明细表 where 生效=1  )
                                   and 销售记录成品出库单明细表.成品出库单明细号=销售记录销售开票明细表.成品出库单明细号 and 作废=0   and 销售开票通知单号='{0}'", str_开票号);
            DataTable dt_开票数 = new DataTable();
            dt_开票数 = CZMaster.MasterSQL.Get_DataTable(sql_开票数, strconn);
            foreach (DataRow dr in dt_开票数.Rows)
            {

                dr["已开票数量"] = Convert.ToDecimal(dr["已开票数量"]) - Convert.ToDecimal(dr["开票数量"]);
                dr["未开票数量"] = Convert.ToDecimal(dr["未开票数量"]) + Convert.ToDecimal(dr["开票数量"]);

            }
            string sql_开票数_补 = string.Format(@"select L销售记录成品出库单明细表L.*,开票数量 from L销售记录成品出库单明细表L,销售记录销售开票明细表 
                where  L销售记录成品出库单明细表L.成品出库单明细号 in (select 成品出库单明细号 from 销售记录销售开票明细表 where 生效=1  )
                    and L销售记录成品出库单明细表L.成品出库单明细号=销售记录销售开票明细表.成品出库单明细号 and 作废=0  and 销售开票通知单号='{0}'", str_开票号);
            DataTable dt_开票数_补 = new DataTable();
            dt_开票数_补 = CZMaster.MasterSQL.Get_DataTable(sql_开票数_补, strconn);
            foreach (DataRow dr in dt_开票数_补.Rows)
            {

                dr["已开票数量"] = Convert.ToDecimal(dr["已开票数量"]) - Convert.ToDecimal(dr["开票数量"]);
                dr["未开票数量"] = Convert.ToDecimal(dr["未开票数量"]) + Convert.ToDecimal(dr["开票数量"]);

            }

            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction stc = conn.BeginTransaction("开票取消生效");
            try
            {
                {
                    sql = "select * from 销售记录销售开票主表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, stc);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt);
                    }
                }
                {
                    sql_mx = "select * from 销售记录销售开票明细表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql_mx, conn, stc);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_mx);
                    }
                }
                if (dt_开票数 != null)
                {
                    sql_开票数 = "select * from 销售记录成品出库单明细表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql_开票数, conn, stc);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_开票数);
                    }
                }

                if (dt_开票数_补 != null)
                {
                    sql_开票数_补 = "select * from L销售记录成品出库单明细表L where 1<>1";
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
        private void fun_搜索组合()
        {
            try
            {
                //视图权限
                DataTable dt_销售人员 = ERPorg.Corg.fun_hr("销售", CPublic.Var.LocalUserID);

                str_选择条件 = "";
                //if (CPublic.Var.LocalUserTeam != "管理员")
                //{
                //    if (dt_销售人员.Rows.Count != 0)
                //    {
                //        str_选择条件 += " ( ";
                //        foreach (DataRow r_x in dt_销售人员.Rows)
                //        {
                //            str_选择条件 += "开票员ID = '" + r_x["工号"].ToString().Trim() + "' or ";
                //        }
                //        str_选择条件 = str_选择条件.Substring(0, str_选择条件.Length - 3);
                //        str_选择条件 = str_选择条件 + " ) ";
                //        str_选择条件 += " and ";
                //        //str_选择条件 += " 开票员 = '" + CPublic.Var.localUserName + "' and ";
                //    }
                //    else
                //    {
                //        throw new Exception("你没有该视图权限");
                //    }
                //}
                //if (bar_出库记录单.EditValue != null && bar_出库记录单.EditValue.ToString() != "")
                //{
                //    str_选择条件 = str_选择条件 + " 成品出库单号 = '" + bar_出库记录单.EditValue.ToString() + "' and";
                //}
                //else
                {
                    if (bar_日期前.EditValue != null && bar_日期前.EditValue.ToString() != "" && bar_日期后.EditValue != null && bar_日期后.EditValue.ToString() != "")
                    {
                        str_选择条件 = str_选择条件 + "开票日期 >= '" + ((DateTime)bar_日期前.EditValue).ToString("yyyy-MM-dd HH:mm:ss") + "' and 开票日期 <= '" + ((DateTime)bar_日期后.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss") + "' and";
                    }
                    if (bar_记录状态.EditValue != null && bar_记录状态.EditValue.ToString() != "")
                    {
                        if (bar_记录状态.EditValue.ToString() == "已生效")
                        {
                            str_选择条件 = str_选择条件 + " 生效 = 1 and";
                        }
                        if (bar_记录状态.EditValue.ToString() == "未生效")
                        {
                            str_选择条件 = str_选择条件 + " 生效 = 0 and";
                        }
                        if (bar_记录状态.EditValue.ToString() == "已完成")
                        {
                            str_选择条件 = str_选择条件 + " 完成 = 1 and";
                        }
                        if (bar_记录状态.EditValue.ToString() == "未完成")
                        {
                            str_选择条件 = str_选择条件 + " 完成 = 0 and";
                        }
                        //if (bar_记录状态.EditValue.ToString() == "已废弃")
                        //{
                        //    str_选择条件 = str_选择条件 + " 废弃 = 1 and";
                        //}
                        //if (bar_记录状态.EditValue.ToString() == "未废弃")
                        //{
                        //    str_选择条件 = str_选择条件 + " 废弃 = 0 and";
                        //}
                        if (bar_记录状态.EditValue.ToString() == "全部")
                        { }
                    }
                }
                str_选择条件 = str_选择条件.Substring(0, str_选择条件.Length - 4);

                if (t_片区.Rows.Count > 0)
                {
                    string sx = "  and 片区 in (";
                    foreach (DataRow r in t_片区.Rows)
                    {
                        sx = sx + string.Format("'{0}',", r["片区"]);
                    }
                    sx = sx.Substring(0, sx.Length - 1) + ")";
                    str_选择条件 = str_选择条件 + sx;
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "开票主表界面_fun_搜索组合");
            }
        }
        #endregion

        #region 界面操作
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ERPorg.Corg.FlushMemory();
            fun_载入开票票单();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //drM = dtM.NewRow();
            //dtM.Rows.Add(drM);
            frm销售记录销售开票详细界面 fm = new frm销售记录销售开票详细界面();
            fm.Dock = System.Windows.Forms.DockStyle.Fill;
            CPublic.UIcontrol.AddNewPage(fm, "成库记录开票");
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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


                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");

            }
        }
        //作废
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr["作废"].Equals(true))
                {
                    throw new Exception("该记录已作废");
                }
                if (dr["审核"].Equals(true))
                {
                    throw new Exception("该记录已审核，不可作废");
                }
                if (MessageBox.Show("是否确认作废？", "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    if (dr["生效"].Equals(true))
                    {
                        fun_取消生效(dr["销售开票通知单号"].ToString());
                    }

                    string sql = string.Format("delete  销售记录销售开票主表  where 销售开票通知单号='{0}'", dr["销售开票通知单号"]);
                    CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
                    string sql_1 = string.Format("delete  销售记录销售开票明细表  where 销售开票通知单号='{0}'", dr["销售开票通知单号"]);
                    CZMaster.MasterSQL.ExecuteSQL(sql_1, strconn);

                    MessageBox.Show("已作废");
                }
                fun_载入开票票单();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                CZMaster.MasterLog.WriteLog(ex.Message);
            }


        }
        //取消生效，取消审核
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                //if (dr["提交审核"].Equals(false))
                //{
                //    throw new Exception("该记录未提交审核");
                //}

                if (dr["作废"].Equals(true))
                {
                    throw new Exception("该记录已作废");
                }
                //using (SqlDataAdapter da = new SqlDataAdapter("select * from 销售记录销售开票主表 where 销售开票通知单号 = '" + dr["销售开票通知单号"].ToString() + "'", CPublic.Var.strConn))
                //{
                //    DataTable dt_判断审核状态 = new DataTable();
                //    da.Fill(dt_判断审核状态);
                //    if (dt_判断审核状态.Rows[0]["审核"].Equals(true))
                //    {
                //        throw new Exception("此单已审核,可刷新查看");
                //    }
                //}
                //  生效字段赋为0，主表子表  已开票数量   未开票数量 成品出库单明细表
                if (MessageBox.Show("是否确认取消生效？", "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    fun_取消生效(dr["销售开票通知单号"].ToString());
                }
                MessageBox.Show("ok");
                fun_载入开票票单();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                CZMaster.MasterLog.WriteLog(ex.Message);
            }

        }


        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow rr = gv.GetDataRow(gv.FocusedRowHandle);
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string strSoNo = string.Format("{0}{1}{2}{3}", DateTime.Now.Year.ToString().Substring(2, 2), DateTime.Now.Month.ToString("00"),
                    DateTime.Now.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("XSKP", DateTime.Now.Year, DateTime.Now.Month).ToString("000"));

                string file = string.Format(dir + @"\\{0}_{1}.txt", strSoNo, System.DateTime.Today.ToString("yyyy-MM-dd"));

                string content = strSoNo + "," + dt_mx.Rows.Count + "," + rr["客户名称"].ToString().Trim() + ",,,,";
                foreach (DataRow dr in dt_mx.Rows)
                {
                    string str_税率 = "";
                    /*18-4-27
                    //
//                 string sql_1= string.Format(@"select 销售记录成品出库单明细表.*,销售记录销售订单明细表.税率  from 销售记录成品出库单明细表,销售记录销售订单明细表
//                    where 销售记录成品出库单明细表.销售订单明细号= 销售记录销售订单明细表.销售订单明细号 and 成品出库单明细号='{0}'", dr["成品出库单明细号"]);
//                    DataTable dt = new DataTable();
//                    dt = CZMaster.MasterSQL.Get_DataTable(sql_1,strconn);
                    
                  
                    //if (dt.Rows.Count > 0)
                    //{
                       // str_税率 =Convert.ToDecimal(Convert.ToDecimal(dt.Rows[0]["税率"])/100).ToString("0.00") ;

                    //}
                     * */
                    str_税率 = Convert.ToDecimal(Convert.ToDecimal(dr["税率"]) / 100).ToString("0.00");
                    content = content + Environment.NewLine + dr["产品名称"].ToString() + "," + dr["计量单位"].ToString() + "," + dr["n原ERP规格型号"].ToString()
                        + "," + dr["开票数量"].ToString() + "," + Convert.ToDecimal(dr["开票税后金额"]).ToString("0.00") + "," + str_税率 + ",1601,0";
                }
                if (File.Exists(file) == true)
                {
                    System.IO.File.WriteAllText(file, content);
                }
                else
                {
                    FileStream myFs = new FileStream(file, FileMode.Create);
                    StreamWriter mySw = new StreamWriter(myFs);
                    mySw.Write(content);
                    mySw.Close();
                    myFs.Close();
                }
                MessageBox.Show("已完成导出！");
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");

            }
        }

        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";//txt_gysmc
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            string s_供应商 = dr["客户名称"].ToString();

            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {

                decimal dec_含税金额 = 0;
                decimal dec_含税金额总 = 0;

                foreach (DataRow dr2 in dt_mx.Rows)
                {

                    dec_含税金额 = Convert.ToDecimal(dr2["开票税后金额"]);
                    dec_含税金额总 += dec_含税金额;
                }
                DataView dv = new DataView(dt_mx);
                dv.Sort = "销售订单明细号";
                DataTable dt_dy = dv.ToTable();
                ItemInspection.print_FMS.fun_print_销售发票明细(dt_dy, dr, false, dec_含税金额总, saveFileDialog.FileName);

                MessageBox.Show("ok");
            }
        }




        //撤回提交审核
        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr["审核"].Equals(true))
                {
                    throw new Exception("开票通知单号已审核");
                }
                using (SqlDataAdapter da = new SqlDataAdapter("select * from 销售记录销售开票主表 where 销售开票通知单号='" + dr["销售开票通知单号"] + "'", strconn))
                {
                    DataTable dt_提交审核 = new DataTable();
                    da.Fill(dt_提交审核);
                    dt_提交审核.Rows[0]["提交审核"] = false;
                    //dt_提交审核.Rows[0]["提交人"] = CPublic.Var.localUserName;
                    //dt_提交审核.Rows[0]["提交人ID"] = CPublic.Var.LocalUserID;
                    //dt_提交审核.Rows[0]["提交日期"] = CPublic.Var.getDatetime();
                    new SqlCommandBuilder(da);
                    da.Update(dt_提交审核);

                    dt_提交审核.AcceptChanges();//函数吊用返回选中那一个

                    dr["提交审核"] = false;
                    dr.AcceptChanges();
                    MessageBox.Show("撤销提交成功！");

                    // barLargeButtonItem2_ItemClick(null,null);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 上传相关文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //DataRow dr_x = gv.GetDataRow(gv.FocusedRowHandle);
            //  if (dr_x != null)
            //  {
            //      销售相关文件上传 f1 = new 销售相关文件上传(dr_x);
            //      f1.ShowDialog();

            //  }
        }

        private void gc_MouseClick(object sender, MouseEventArgs e)
        {
            //19-7-29 隐藏 暂时不用
            //if (e != null && e.Button == MouseButtons.Right)
            //   {
            //       contextMenuStrip1.Show(gc, new Point(e.X, e.Y));
            //   }
        }

        private void barLargeButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {



        }

        private void barLargeButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // DataRow drM = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
            DataRow dras = (this.BindingContext[gc.DataSource].Current as DataRowView).Row;

            DataTable dtm = (DataTable)this.gridControl1.DataSource;
            Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
            Type outerForm = outerAsm.GetType("ERPreport.销售开票", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统

            object[] drr = new object[2];

            drr[0] = dras;
            drr[1] = dtm;
            //   drr[2] = dr["出入库申请单号"].ToString();
            Form ui = Activator.CreateInstance(outerForm, drr) as Form;
            //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
            ui.ShowDialog();
        }
        private void 修改开票日期ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow foucedR = gv.GetDataRow(gv.FocusedRowHandle);
                string s = "select  MAX(结算日期)结算日期 from 仓库月出入库结转表";
                DataRow dr = CZMaster.MasterSQL.Get_DataRow(s, strconn);
                DateTime time;
                if (dr != null && dr["结算日期"].ToString() != "")
                {
                    time = Convert.ToDateTime(dr["结算日期"]);
                    if (time > Convert.ToDateTime(foucedR["开票日期"])) //结算过的单子不允许修改
                    {
                        throw new Exception("该单据已结算,不可修改");
                    }
                    else
                    {
                        修改时间 fm = new 修改时间(time, Convert.ToDateTime(foucedR["开票日期"]), foucedR["销售开票通知单号"].ToString());
                        fm.StartPosition = FormStartPosition.CenterScreen;
                        fm.ShowDialog();
                        if (fm.bl)
                        {
                            s = string.Format("select  * from [销售记录销售开票主表] where 销售开票通知单号='{0}'", fm.s_改);
                            DataRow rrr = CZMaster.MasterSQL.Get_DataRow(s, strconn);
                            foucedR.ItemArray = rrr.ItemArray;
                            foucedR.AcceptChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void gv_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                drM = gv.GetDataRow(gv.FocusedRowHandle);
                string str_开票票号 = drM["销售开票通知单号"].ToString();
                fun_载入明细(str_开票票号);
            }
            catch (Exception)
            {
 
            }
          
        }
    }
}
//上传文件
//private void 上传相关文件ToolStripMenuItem_Click(object sender, EventArgs e)
//{

//}
//右击
//private void gc_MouseClick(object sender, MouseEventArgs e)
//{

//}


//    }
//}
