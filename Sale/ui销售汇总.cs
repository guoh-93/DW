using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace ERPSale
{
    public partial class ui销售汇总 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        DataTable dtM = null;
        DataTable dtP1 = null;
        DataTable dtP2 = null;

        public ui销售汇总()
        {
            InitializeComponent();
        }

        private void ui销售汇总_Load(object sender, EventArgs e)
        {
            try
            {
                DateTime today = CPublic.Var.getDatetime().Date;
                string sql = "select 客户编号,客户名称,片区,业务员 from 客户基础信息表 where 停用=0";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    DataTable dt =new DataTable ();
                    da.Fill(dt);
                    repositoryItemSearchLookUpEdit1.DataSource = dt;
                    repositoryItemSearchLookUpEdit1.DisplayMember = "客户名称";
                    repositoryItemSearchLookUpEdit1.ValueMember = "客户编号";

                }
            
                DateTime t1 = Convert.ToDateTime(today.AddDays(-1));
                t1 = new DateTime(t1.Year, t1.Month,t1.Day);
                bar_前.EditValue =t1;
                bar_后.EditValue = t1;
                DateTime t2 = t1.AddDays(1).AddSeconds(-1);
                t2= new DateTime(t2.Year, t2.Month, t2.Day,t2.Hour,t2.Minute,t2.Second );

                fun_载入(t1,t2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_载入(DateTime dt1, DateTime dt2)
        {
            string str_条件 = "";
            if (barEditItem1.EditValue != null && barEditItem1.EditValue.ToString() != "")
            {
                str_条件 = string.Format(" and szb.客户编号='{0}'", barEditItem1.EditValue.ToString());

            }


            string sqq = string.Format(@"select sum(smx.数量) as 数量
                    ,SUM(smx.税后金额) as 税后金额,SUM(smx.税前金额) as 税前金额
                    from [销售记录销售订单明细表] smx
                    left join  销售记录销售订单主表 szb on  szb.销售订单号=  smx.销售订单号
                    left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = smx.物料编码
                    where szb.备注10='' and smx.生效=1 and smx.作废 = 0 and smx.关闭 = 0 and 创建日期 >= '{0}' and 创建日期 <= '{1}' {2}", dt1, dt2, str_条件);
            DataTable dt = new DataTable();
            SqlDataAdapter daa = new SqlDataAdapter(sqq, strconn);
            daa.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                throw new Exception("没有查询到数据！");
            }

            string sql = string.Format(@"select base.大类,base.小类,base.物料编码,base.规格型号,sum(smx.数量)as 数量
                     ,SUM(smx.税后金额) as 税后金额,SUM(smx.税前金额) as 税前金额,产品线
                     from [销售记录销售订单明细表] smx
                    left join  销售记录销售订单主表 szb on  szb.销售订单号=  smx.销售订单号
                     left join 基础数据物料信息表 base on base.物料编码 = smx.物料编码
                     where szb.备注10='' and  smx.生效=1 and smx.作废 = 0 and smx.关闭 = 0 and 创建日期 >= '{0}' and 创建日期 <= '{1}'{2}
                     group by 大类,小类,base.物料编码,base.规格型号,产品线 ", dt1, dt2, str_条件);
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            dtM.Columns.Add("金额比例");
            dtM.Columns.Add("税后均价");
            dtM.Columns.Add("税前均价");
                //decimal dec_总数=0;                                                                 
                //decimal dec_税前总额=0;
                //decimal dec_税后总额=0;
             

            foreach (DataRow dr in dtM.Rows)
            {
                //dec_总数=dec_总数+Convert.ToDecimal(dr["数量"]) ;
                //dec_税前总额=dec_税前总额+Convert.ToDecimal(dr["税前金额"]) ;
                //dec_税后总额=dec_税后总额+Convert.ToDecimal(dr["税后金额"]) ;
                if(Convert.ToDecimal(dr["税前金额"]) == 0)
                {
                    dr["金额比例"] = "0%";
                }
                else
                {
                    dr["金额比例"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dt.Rows[0]["税前金额"]) * 100).ToString("0.0000") + "%";
                }
               
                // dr["金额比例"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dtM.Rows[0]["税前金额"])).ToString("0.00000") ;


                if (Convert.ToDecimal(dr["数量"]) == 0)
                {
                    dr["税前均价"] = Convert.ToDecimal(dr["税前金额"]).ToString("0.0000");
                    dr["税后均价"] = Convert.ToDecimal(dr["税后金额"]).ToString("0.0000");
                }
                else
                {
                    dr["税前均价"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dr["数量"])).ToString("0.0000");
                    dr["税后均价"] = (Convert.ToDecimal(dr["税后金额"]) / Convert.ToDecimal(dr["数量"])).ToString("0.0000");
                }
            }
            gcM.DataSource = dtM;
            //DataRow rr = dtM.NewRow();
            //  rr["n原ERP规格型号"]="合计";
            //rr["数量"] = dec_总数;
            //rr["税前金额"] = dec_税前总额;
            //rr["税后金额"] = dec_税后总额;
            //dtM.Rows.Add(rr);

            sql = string.Format(@"select base.大类,base.小类,sum(smx.数量)as 数量
                     ,SUM(smx.税后金额) as 税后金额,SUM(smx.税前金额) as 税前金额
                     from [销售记录销售订单明细表] smx
                    left join  销售记录销售订单主表 szb on  szb.销售订单号=  smx.销售订单号

                    left join 基础数据物料信息表 base on base.物料编码 = smx.物料编码
                     where szb.备注10='' and  smx.生效=1 and  smx.作废 = 0  and smx.关闭 = 0 and 创建日期 >= '{0}' and 创建日期 <= '{1}' {2}
                     group by 大类,小类", dt1, dt2,str_条件);
            dtP1 = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtP1);
            dtP1.Columns.Add("金额比例");
            dtP1.Columns.Add("税前均价");
            dtP1.Columns.Add("税后均价");
            foreach (DataRow dr in dtP1.Rows)
            {
                //dr["金额比例"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dt.Rows[0]["税前金额"]) * 100).ToString("0.0000") + "%";
                // dr["金额比例"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dtM.Rows[0]["税前金额"])).ToString("0.00000");
                if (Convert.ToDecimal(dr["税前金额"]) == 0)
                {
                    dr["金额比例"] = "0%";
                }
                else
                {
                    dr["金额比例"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dt.Rows[0]["税前金额"]) * 100).ToString("0.0000") + "%";
                }
                if (Convert.ToDecimal(dr["数量"]) == 0)
                {
                    dr["税前均价"] = Convert.ToDecimal(dr["税前金额"]).ToString("0.0000");
                    dr["税后均价"] = Convert.ToDecimal(dr["税后金额"]).ToString("0.0000");
                }
                else
                {
                    dr["税前均价"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dr["数量"])).ToString("0.0000");
                    dr["税后均价"] = (Convert.ToDecimal(dr["税后金额"]) / Convert.ToDecimal(dr["数量"])).ToString("0.0000");
                }
            }
            gcP1.DataSource = dtP1;

            sql = string.Format(@"select base.大类,sum(smx.数量) as 数量
                     ,SUM(smx.税后金额) as 税后金额,SUM(smx.税前金额) as 税前金额
                     from [销售记录销售订单明细表] smx
                    left join  销售记录销售订单主表 szb on  szb.销售订单号=  smx.销售订单号
                     left join 基础数据物料信息表 base on base.物料编码 = smx.物料编码
                     where szb.备注10='' and  smx.作废 = 0 and smx.关闭 = 0 and 创建日期 >= '{0}' and 创建日期 <= '{1}' {2}
                     group by 大类", dt1, dt2,str_条件);
            dtP2 = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtP2);
            dtP2.Columns.Add("金额比例");
            dtP2.Columns.Add("税前均价");
            dtP2.Columns.Add("税后均价");
            foreach (DataRow dr in dtP2.Rows)
            {
                //dr["金额比例"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dt.Rows[0]["税前金额"]) * 100).ToString("0.0000") + "%";
                //dr["金额比例"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dtM.Rows[0]["税前金额"])).ToString("0.00000");
                //dr["税前均价"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dr["数量"])).ToString("0.0000");
                // dr["税后均价"] = (Convert.ToDecimal(dr["税后金额"]) / Convert.ToDecimal(dr["数量"])).ToString("0.0000");
                if (Convert.ToDecimal(dr["税前金额"]) == 0)
                {
                    dr["金额比例"] = "0%";
                }
                else
                {
                    dr["金额比例"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dt.Rows[0]["税前金额"]) * 100).ToString("0.0000") + "%";
                }
                if (Convert.ToDecimal(dr["数量"]) == 0)
                {
                    dr["税前均价"] = Convert.ToDecimal(dr["税前金额"]).ToString("0.0000");
                    dr["税后均价"] = Convert.ToDecimal(dr["税后金额"]).ToString("0.0000");
                }
                else
                {
                    dr["税前均价"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dr["数量"])).ToString("0.0000");
                    dr["税后均价"] = (Convert.ToDecimal(dr["税后金额"]) / Convert.ToDecimal(dr["数量"])).ToString("0.0000");
                }
            }
            gcP2.DataSource = dtP2;
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //try
            //{
                if (bar_前.EditValue == null || bar_前.EditValue.ToString() == "")
                {
                    throw new Exception("请选择时间");
                }
                if (bar_后.EditValue == null || bar_后.EditValue.ToString() == "")
                {

                    throw new Exception("请选择时间");
                }
                DateTime t1 = Convert.ToDateTime(bar_前.EditValue);
                t1 = new DateTime(t1.Year, t1.Month, t1.Day);
                DateTime t2 = Convert.ToDateTime(bar_后.EditValue).AddDays(1).AddSeconds(-1);
                t2 = new DateTime(t2.Year,t2.Month,t2.Day,t2.Hour,t2.Minute,t2.Second);
               // fun_载入((DateTime)bar_前.EditValue, ((DateTime)bar_后.EditValue).AddDays(1).AddSeconds(-1));
                fun_载入(t1,t2);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                saveFileDialog.FileName = ((DateTime)bar_前.EditValue).ToString("yyyy-MM-dd") + "至" + ((DateTime)bar_后.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd");
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    //saveFileDialog.FileName = ((DateTime)bar_前.EditValue).ToString() + ((DateTime)bar_后.EditValue).AddDays(1).AddSeconds(-1).ToString();
                    gcM.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                saveFileDialog.FileName = ((DateTime)bar_前.EditValue).ToString("yyyy-MM-dd") + "至" + ((DateTime)bar_后.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd");
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    //saveFileDialog.FileName = ((DateTime)bar_前.EditValue).ToString() + ((DateTime)bar_后.EditValue).AddDays(1).AddSeconds(-1).ToString();
                    gcP1.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                saveFileDialog.FileName = ((DateTime)bar_前.EditValue).ToString("yyyy-MM-dd") + "至" + ((DateTime)bar_后.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd");
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    //saveFileDialog.FileName = ((DateTime)bar_前.EditValue).ToString() + ((DateTime)bar_后.EditValue).AddDays(1).AddSeconds(-1).ToString();
                    gcP2.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
                MessageBox.Show(ex.Message);
            }
        }

        private void gvM_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gvP1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gvP2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

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

        private void gvP1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gvP1.GetFocusedRowCellValue(gvP1.FocusedColumn));
                e.Handled = true;
            }
        }

        private void gvP2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gvP2.GetFocusedRowCellValue(gvP2.FocusedColumn));
                e.Handled = true;
            }
        }
    }
}
