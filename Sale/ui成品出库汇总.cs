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
    public partial class ui成品出库汇总 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        DataTable dtM = null;
        DataTable dtP1 = null;
        DataTable dtP2 = null;

        public ui成品出库汇总()
        {
            InitializeComponent();
        }

        private void ui成品出库汇总_Load(object sender, EventArgs e)
        {
            try
            {
                DateTime today = CPublic.Var.getDatetime().Date;
                DateTime t1 = today.AddDays(-1);
                t1 = new DateTime(t1.Year, t1.Month, t1.Day);
                DateTime t2 = today.AddSeconds(-1);
                t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
                bar_前.EditValue =t1.ToString("yyyy-MM-dd");
                bar_后.EditValue =t2.ToString("yyyy-MM-dd");
                fun_载入(t1, t2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_载入(DateTime dt1, DateTime dt2)
        {
//            string sqq = string.Format(@"select SUM(ab.出库数量)as 出库数量,SUM(ab.税前金额)as 税前金额,SUM(ab.税后金额)as 税后金额 from
//					(select sum(销售记录成品出库单明细表.出库数量) as 出库数量 
//                    ,(sum(销售记录成品出库单明细表.出库数量) * sum(销售记录销售订单明细表.税前单价)) as 税前金额,
//                    (sum(销售记录成品出库单明细表.出库数量) * sum(销售记录销售订单明细表.税后单价)) as 税后金额
//                    from [销售记录成品出库单明细表] 
//
//                    left join 销售记录销售订单明细表 on 销售记录销售订单明细表.销售订单明细号 = 销售记录成品出库单明细表.销售订单明细号
//
//                    left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 销售记录成品出库单明细表.物料编码
//                    where 销售记录成品出库单明细表.生效 = 1 and 销售记录成品出库单明细表.作废 = 0 
//                    and 销售记录成品出库单明细表.生效日期 >= '{0}' and 销售记录成品出库单明细表.生效日期 <= '{1}'
//                    group by 大类,小类,销售记录成品出库单明细表.物料编码) ab", dt1, dt2);


            string sqq = string.Format(@" select SUM(ab.出库数量)as 出库数量,SUM(ab.税前金额)as 税前金额,SUM(ab.税后金额)as 税后金额 from
                    (select sum(出库数量)as 出库数量,sum(出库数量*税后单价) as 税后金额,sum(出库数量*税前单价) as 税前金额 
                              from 销售记录成品出库单明细表 scmx left join 销售记录销售订单明细表 smx on smx.销售订单明细号 = scmx.销售订单明细号
                     left join 销售记录销售订单主表 szb on smx.销售订单号 = szb.销售订单号

                    left join 基础数据物料信息表 base on base.物料编码 = scmx.物料编码
                    where scmx.生效 = 1 and scmx.作废 = 0  and   scmx.备注1<>'退货' and szb.备注10=''
                    and scmx.生效日期 >= '{0}' and scmx.生效日期 <= '{1}'
                    group by 大类,小类,scmx.物料编码) ab", dt1, dt2);
            DataTable dt = new DataTable();
            SqlDataAdapter daa = new SqlDataAdapter(sqq, strconn);
            daa.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                throw new Exception("没有查询到数据！");
            }

            string sql = string.Format(@"select scmx.物料编码,base.规格型号,base.大类,base.小类,sum(scmx.出库数量) as 出库数量,产品线,
         sum(scmx.出库数量 * smx.税前单价) as 税前金额,sum(scmx.出库数量 * smx.税后单价) as 税后金额   from 销售记录成品出库单明细表 scmx 
         left join 销售记录销售订单明细表 smx on smx.销售订单明细号 = scmx.销售订单明细号
         left join 销售记录销售订单主表 szb on smx.销售订单号 = szb.销售订单号
         left join 基础数据物料信息表 base on base.物料编码 = scmx.物料编码
         where scmx.生效 = 1 and scmx.作废 = 0  and scmx.备注1<>'退货'
         and scmx.生效日期 >= '{0}' and scmx.生效日期 <= '{1}' and szb.备注10=''
         group by 大类,小类,scmx.物料编码,base.规格型号,产品线", dt1, dt2);
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            dtM.Columns.Add("金额比例");
            dtM.Columns.Add("税后均价");
            dtM.Columns.Add("税前均价");

            //decimal dec_总数 = 0;
            //decimal dec_税前总额 = 0;
            //decimal dec_税后总额 = 0;

            foreach (DataRow dr in dtM.Rows)
            {
                if (dr["税前金额"].ToString() == "")
                {
                    dr["税前金额"] = 0;
                }
                if (dr["税后金额"].ToString() == "")
                {
                    dr["税后金额"] = 0;

                }

                //dec_总数 = dec_总数 + Convert.ToDecimal(dr["出库数量"]);
                //dec_税前总额 = dec_税前总额 + Convert.ToDecimal(dr["税前金额"]);
                //dec_税后总额 = dec_税后总额 + Convert.ToDecimal(dr["税后金额"]);
                if (Convert.ToDecimal(dr["税前金额"]) == 0)
                {
                    dr["金额比例"] = "0%";
                }
                else
                {
                    dr["金额比例"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dt.Rows[0]["税前金额"]) * 100).ToString("0.0000") + "%";
                }
                if (Convert.ToDecimal(dr["出库数量"]) == 0)
                {
                    dr["税前均价"] = Convert.ToDecimal(dr["税前金额"]);
                    dr["税后均价"] = Convert.ToDecimal(dr["税后金额"]);
                }
                else
                {
                    dr["税前均价"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dr["出库数量"])).ToString("0.0000");
                    dr["税后均价"] = (Convert.ToDecimal(dr["税后金额"]) / Convert.ToDecimal(dr["出库数量"])).ToString("0.0000");
                }
                
            }
            gcM.DataSource = dtM;

            //DataRow rr = dtM.NewRow();
            //rr["n原ERP规格型号"] = "合计";
            //rr["出库数量"] = dec_总数;
            //rr["税前金额"] = dec_税前总额;
            //rr["税后金额"] = dec_税后总额;
            //dtM.Rows.Add(rr);

            sql = string.Format(@"select ab.大类,ab.小类,SUM(ab.出库数量)as 出库数量,SUM(ab.税前金额)as 税前金额,SUM(ab.税后金额)as 税后金额 from (
             select scmx.物料编码,base.规格型号,base.大类,base.小类,sum(scmx.出库数量) as 出库数量,sum(scmx.出库数量 * smx.税前单价) as 税前金额,
             sum(scmx.出库数量 * smx.税后单价) as 税后金额  from 销售记录成品出库单明细表 scmx 
             left join 销售记录销售订单明细表 smx on smx.销售订单明细号 = scmx.销售订单明细号
             left join 销售记录销售订单主表 szb on smx.销售订单号 = szb.销售订单号      
             left join 基础数据物料信息表  base on base.物料编码 = scmx.物料编码
             where scmx.生效 = 1 and scmx.作废 = 0  and scmx.备注1<>'退货' and szb.备注10=''
             and scmx.生效日期 >= '{0}' and scmx.生效日期 <= '{1}' 
             group by 大类,小类,scmx.物料编码,base.规格型号) ab group by ab.大类,ab.小类", dt1, dt2);
            dtP1 = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtP1);
            dtP1.Columns.Add("金额比例");
            dtP1.Columns.Add("税前均价");
            dtP1.Columns.Add("税后均价");
            foreach (DataRow dr in dtP1.Rows)
            {
                if (dr["税前金额"].ToString() == "")
                {
                    dr["税前金额"] = 0;
                }
                if (dr["税后金额"].ToString() == "")
                {
                    dr["税后金额"] = 0;

                }
                //dr["金额比例"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dt.Rows[0]["税前金额"]) * 100).ToString("0.0000") + "%";
                //dr["税前均价"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dr["出库数量"])).ToString("0.0000");
                // dr["税后均价"] = (Convert.ToDecimal(dr["税后金额"]) / Convert.ToDecimal(dr["出库数量"])).ToString("0.0000");
                if (Convert.ToDecimal(dr["税前金额"]) == 0)
                {
                    dr["金额比例"] = "0%";
                }
                else
                {
                    dr["金额比例"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dt.Rows[0]["税前金额"]) * 100).ToString("0.0000") + "%";
                }
                if (Convert.ToDecimal(dr["出库数量"]) == 0)
                {
                    dr["税前均价"] = Convert.ToDecimal(dr["税前金额"]);
                    dr["税后均价"] = Convert.ToDecimal(dr["税后金额"]);
                }
                else
                {
                    dr["税前均价"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dr["出库数量"])).ToString("0.0000");
                    dr["税后均价"] = (Convert.ToDecimal(dr["税后金额"]) / Convert.ToDecimal(dr["出库数量"])).ToString("0.0000");
                }
            }
            gcP1.DataSource = dtP1;

            sql = string.Format(@"select ab.大类,SUM(ab.出库数量)as 出库数量,SUM(ab.税前金额)as 税前金额,SUM(ab.税后金额)as 税后金额 from (
                    select scmx.物料编码,base.规格型号,base.大类,base.小类,
                    sum(scmx.出库数量) as 出库数量,sum(scmx.出库数量 * smx.税前单价) as 税前金额,
                    sum(scmx.出库数量 * smx.税后单价) as 税后金额  from 销售记录成品出库单明细表 scmx
                     left join 销售记录销售订单明细表 smx on smx.销售订单明细号 = scmx.销售订单明细号

                     left join 销售记录销售订单主表 szb on smx.销售订单号 = szb.销售订单号

                 left join 基础数据物料信息表 base on base.物料编码 = scmx.物料编码
                      where scmx.生效 = 1 and scmx.作废 = 0 and scmx.备注1<>'退货' 
                        and scmx.生效日期 >= '{0}' and scmx.生效日期 <= '{1}' and szb.备注10=''
                         group by 大类,小类,scmx.物料编码,base.规格型号) ab group by ab.大类", dt1, dt2);
            dtP2 = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtP2);
            dtP2.Columns.Add("金额比例");
            dtP2.Columns.Add("税前均价");
            dtP2.Columns.Add("税后均价");
            foreach (DataRow dr in dtP2.Rows)
            {
                if (dr["税前金额"].ToString() == "")
                {
                    dr["税前金额"] = 0;
                }
                if (dr["税后金额"].ToString() == "")
                {
                    dr["税后金额"] = 0;

                }
                //dr["金额比例"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dt.Rows[0]["税前金额"]) * 100).ToString("0.0000") + "%";
                //dr["税前均价"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dr["出库数量"])).ToString("0.0000");
                //dr["税后均价"] = (Convert.ToDecimal(dr["税后金额"]) / Convert.ToDecimal(dr["出库数量"])).ToString("0.0000");
                if (Convert.ToDecimal(dr["税前金额"]) == 0)
                {
                    dr["金额比例"] = "0%";
                }
                else
                {
                    dr["金额比例"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dt.Rows[0]["税前金额"]) * 100).ToString("0.0000") + "%";
                }
                if (Convert.ToDecimal(dr["税前金额"]) == 0)
                {
                    dr["金额比例"] = "0%";
                }
                else
                {
                    dr["金额比例"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dt.Rows[0]["税前金额"]) * 100).ToString("0.0000") + "%";
                }
                if (Convert.ToDecimal(dr["出库数量"]) == 0)
                {
                    dr["税前均价"] = Convert.ToDecimal(dr["税前金额"]);
                    dr["税后均价"] = Convert.ToDecimal(dr["税后金额"]);
                }
                else
                {
                    dr["税前均价"] = (Convert.ToDecimal(dr["税前金额"]) / Convert.ToDecimal(dr["出库数量"])).ToString("0.0000");
                    dr["税后均价"] = (Convert.ToDecimal(dr["税后金额"]) / Convert.ToDecimal(dr["出库数量"])).ToString("0.0000");
                }
            }
            gcP2.DataSource = dtP2;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
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
                t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);

                fun_载入(t1,t2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                saveFileDialog.FileName = Convert.ToDateTime(bar_前.EditValue).ToString("yyyy-MM-dd") + "至" + Convert.ToDateTime(bar_后.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd");
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
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
                saveFileDialog.FileName = Convert.ToDateTime(bar_前.EditValue).ToString("yyyy-MM-dd") + "至"+ Convert .ToDateTime(bar_后.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd");
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
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
                saveFileDialog.FileName = Convert.ToDateTime(bar_前.EditValue).ToString("yyyy-MM-dd") + "至" + Convert.ToDateTime(bar_后.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd");
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
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

        private void gvP1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gvP2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gvM_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
    }
}
