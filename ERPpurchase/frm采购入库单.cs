using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CZMaster;
using System.Data.SqlClient;
using System.Threading;
using System.IO;
namespace ERPpurchase
{
    public partial class frm采购入库单 : UserControl
    {
        /// <summary>
        /// 数据库连接的字符串 ERPDB数据库
        /// </summary>
        string sqlstrconn = "";
        string sql_ck = "";
        string cfgfilepath = "";
        DataTable dt_仓库;
        public frm采购入库单()
        {
            InitializeComponent();
            sqlstrconn = CPublic.Var.strConn;
        }

        #region   变量

        DateTime time1;
        DateTime time2;

        DataTable dt_操作员;

        DataTable dt_入库列表;


        #endregion


       
        private void frm采购入库单_Load(object sender, EventArgs e)
        {
            try
            {
                if (CPublic.Var.LocalUserTeam.Contains("管理员") || CPublic.Var.LocalUserID == "admin")
                {
                    barLargeButtonItem10.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                }
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name)))
                {

                    gv1.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            
                sql_ck = string.Format("select * from 人员仓库对应表 where 工号='{0}'", CPublic.Var.LocalUserID);
                dt_仓库 = new DataTable();
                dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ck, sqlstrconn);

                //查询条件赋上初值
                txt_rkdan.EditValue = "";
                txt_riqi1.EditValue = Convert.ToDateTime(System.DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd"));
                txt_riqi2.EditValue = Convert.ToDateTime(System.DateTime.Today.ToString("yyyy-MM-dd"));
                txt_rkzhuangtai.EditValue = "已生效";
                fun_查询数据();
                gc1.DataSource = dt_入库列表;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //入库单列表的数据查询
        private void fun_查询数据()
        {
            try
            {
            string strRKD = "";
            string strdjzt = "";
            strRKD = txt_rkdan.EditValue.ToString().Trim();   //入库单号  //19-12-27 改为采购单
           // strdjzt = txt_rkzhuangtai.EditValue.ToString();  //单据的状态
            string strsql = "";
            if (strRKD != "")  //采购单号不为空的话,直接查采购单
            {
                strsql = strsql + string.Format(" crmx.采购单号='{0}' and", strRKD);
            }
            else
            {
                if (txt_riqi1.EditValue != null && txt_riqi2.EditValue != null)
                {
                    time1 = Convert.ToDateTime(txt_riqi1.EditValue);
                    time2 = Convert.ToDateTime(txt_riqi2.EditValue).AddDays(1).AddSeconds(-1);
                    if (time1 > time2)
                        throw new Exception("第一个时间不能大于第二个时间！");

                    if (time1.ToString().Substring(0, 1) != "0" && time2.ToString().Substring(0, 1) != "0")    //录入的日期
                    {
                        strsql = strsql + string.Format(" crz.录入日期 >= '{0}' and crz.录入日期<= '{1}' and", time1, time2);
                    }
                }
                //if (strdjzt != "")     //单据的状态 生效  作废
                //{
                //    if (strdjzt == "未生效")
                //    {
                //        strsql = strsql + string.Format(" crz.生效=0 and");
                //    }

                //    if (strdjzt == "已生效")
                //    {
                //        strsql = strsql + string.Format(" crz.生效=1 and");
                //    }

                //    if (strdjzt == "未作废")
                //    {
                //        strsql = strsql + string.Format(" crz.作废=0 and");
                //    }

                //    if (strdjzt == "已作废")
                //    {
                //        strsql = strsql + string.Format(" crz.作废=1 and");
                //    }
                //}
            }
            if (strsql != "")
            {
                strsql = " where" + strsql.Substring(0, strsql.Length - 3);
            }
            sql_ck = "and cmx.仓库号  in(";
            if (dt_仓库.Rows.Count == 0 || CPublic.Var.LocalUserID=="admin"|| CPublic.Var.LocalUserTeam.Contains("管理员权限"))
            {
                strsql = string.Format(@"select crz.生效,crmx.送检单号,crz.录入日期,crmx.供应商ID,crz.作废,crmx.采购单明细号,crmx.检验记录单号,
                    cz.采购单类型,crmx.供应商,crmx.入库单号,cz.采购单类型,crmx.物料编码,czjy.检验日期,
                    crmx.物料名称,crmx.规格型号,crmx.送检数量,crz.生效人员,crmx.入库量
                    from 采购记录采购单入库明细 crmx
                    left join 采购记录采购单入库主表 crz on crz.入库单号=crmx.入库单号
                    left join 采购记录采购单明细表 cmx on cmx.采购明细号=crmx.采购单明细号
                    left join  采购记录采购单主表 cz on cz.采购单号 = crmx.采购单号
                    left join  采购记录采购单检验主表 czjy on czjy.检验记录单号 = crmx.检验记录单号
                    left join  基础数据物料信息表 on  crmx.物料编码=基础数据物料信息表.物料编码 {0}", strsql);
            }
            else
            {
                foreach (DataRow dr in dt_仓库.Rows)
                {
                    sql_ck = sql_ck + string.Format("'{0}',", dr["仓库号"]);

                }
                sql_ck = sql_ck.Substring(0, sql_ck.Length - 1) + ")";
                strsql = string.Format(@"select crz.生效,crmx.送检单号,crz.录入日期,crmx.供应商ID,crz.作废,crmx.采购单明细号,crmx.检验记录单号,
                        crmx.供应商,crmx.入库单号,crmx.物料编码,czjy.检验日期,cz.采购单类型,cz.采购单类型,crmx.物料名称,crmx.规格型号,crmx.送检数量,crz.生效人员,crmx.入库量 
                         from 采购记录采购单入库明细 crmx
                        left join 采购记录采购单入库主表 crz on crz.入库单号=crmx.入库单号
                        left join 采购记录采购单明细表 cmx on cmx.采购明细号=crmx.采购单明细号
                        left join  采购记录采购单主表 cz on cz.采购单号 = crmx.采购单号 
                        left join  采购记录采购单检验主表 czjy on czjy.检验记录单号 = crmx.检验记录单号
                        left join  基础数据物料信息表 on  crmx.物料编码=基础数据物料信息表.物料编码 {0} {1}", strsql, sql_ck);
            }

            dt_入库列表 = MasterSQL.Get_DataTable(strsql, sqlstrconn);

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_查询数据");
                throw new Exception(ex.Message);
            }
        }

      
        #region  界面的操作

        //查询操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                sql_ck = string.Format("select * from 人员仓库对应表 where 工号='{0}'", CPublic.Var.LocalUserID);
                dt_仓库 = new DataTable();
                dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ck, sqlstrconn);
                fun_查询数据();
                if (dt_入库列表.Rows.Count <= 0)
                {
                    gc1.DataSource = dt_入库列表;
                    throw new Exception("查无数据！");
                }
                gc1.DataSource = dt_入库列表;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //新增操作：转到新增入库单的界面
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                frm采购入库单明细 fm = new frm采购入库单明细();
                CPublic.UIcontrol.AddNewPage(fm, "采购单入库明细");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //单元格双击查询入库单的明细
        private void gv1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                //if (e.Clicks == 2 && e.Button==MouseButtons.Left)
                //{
                //    string strdan = "";

                //    if (dt_入库列表.Rows.Count>0)
                //    {
                //        DataRow r = (this.BindingContext[dt_入库列表].Current as DataRowView).Row;
                //        strdan = r["入库单号"].ToString();
                //        if (Convert.ToBoolean(r["生效"]) == true)
                //        {
                //            frm采购入库视图 frm = new frm采购入库视图(strdan);
                //            CPublic.UIcontrol.AddNewPage(frm, "采购入库视图");
                //        }
                //        else
                //        {
                //            frm采购入库单明细 frm = new frm采购入库单明细(strdan);
                //            CPublic.UIcontrol.AddNewPage(frm, "采购入库明细");
                //        }
                //    }




                //}   
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

 

        #endregion

        


        //清空单号
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txt_rkdan.EditValue = "";
        }

        //页面关闭
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

 
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                //gridControl1.ExportToXls(saveFileDialog.FileName, options);  
                gc1.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void gv1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            //行号设置 
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString("00");
            } 
        }

        private void gv1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv1.GetFocusedRowCellValue(gv1.FocusedColumn));
                e.Handled = true;
            }
        }

        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("是否打印当前选中行", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Thread thDo;
                thDo = new Thread(Dowork);
                //Dowork();
                thDo.IsBackground = true;
                thDo.Start();
            }
        }
        public void Dowork()
        {
            DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
            if (dr != null)
            {
                DataTable dt_dy = dt_入库列表.Clone();
                dt_dy.ImportRow(dr);
                ItemInspection.print_FMS.fun_P_来料入库单(dt_dy);
            }
        }

        private void gv1_ColumnPositionChanged(object sender, EventArgs e)
        {

            try
            {

                if (cfgfilepath != "")
                {
                    gv1.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void gv1_ColumnFilterChanged(object sender, EventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gv1.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        //撤回
        private void barLargeButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
            DateTime t = CPublic.Var.getDatetime().Date;
            t = new DateTime(t.Year, t.Month, 1);

            try
            {
                if (Convert.ToDateTime(dr["录入日期"]) < t) throw new Exception("往月数据不可撤销");
                if (MessageBox.Show("确定撤回该单据？请核对。", "提示!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    ERPorg.Corg cg = new ERPorg.Corg();
                    DataSet ds = cg.back_purrk(dr["入库单号"].ToString());

                    SqlConnection conn = new SqlConnection(sqlstrconn);
                    conn.Open();
                    SqlTransaction pts = conn.BeginTransaction("采购入库撤回");
                    try
                    {
                        SqlCommand cmm = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, pts);
                        SqlDataAdapter da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(ds.Tables[0]);

                        cmm = new SqlCommand("select * from 采购记录采购单检验主表  where 1<>1", conn, pts);
                        da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(ds.Tables[1]);


                        cmm = new SqlCommand("select  * from 采购记录采购单入库主表 where 1=2", conn, pts);
                        da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(ds.Tables[2]);

                        cmm = new SqlCommand("select  * from  采购记录采购单入库明细 where 1=2", conn, pts);
                        da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(ds.Tables[3]);

                        cmm = new SqlCommand("select  * from  仓库出入库明细表  where 1=2", conn, pts);
                        da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(ds.Tables[4]);

                        cmm = new SqlCommand("select  * from 仓库物料数量表 where 1=2 ", conn, pts);
                        da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(ds.Tables[5]);
                        pts.Commit();
                        MessageBox.Show("撤回成功");
                        dt_入库列表.Rows.Remove(dr);
                    }

                    catch (Exception ex)
                    {
                        pts.Rollback();
                        throw new Exception(ex.Message);
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
