using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace ItemInspection
{
    public partial class 在线检验不良原因按产品 : UserControl
    {
        public 在线检验不良原因按产品()
        {
            InitializeComponent();
            barEditItem2.EditValue = System.DateTime.Today.ToString("yyyy-MM-dd");
            barEditItem1.EditValue = System.DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");
        }


        #region
        DataTable dt_左; //17-6-15弃用
        DataTable dt_产品;
        DataTable dt_返工原因; //17-6-15弃用
        DataTable dt_工单; //17-6-15弃用

        string strcon = CPublic.Var.strConn;
        #endregion

        private void 在线检验不良原因按产品_Load(object sender, EventArgs e)
        {


        }
        private void fun_loadxin()
        {  //7/9 注释
            //            string sql_1 = string.Format(@"select a.原ERP物料编号 ,a.原规格型号,返工原因,SUM(数量)as 数量  from 
            //                        (select 成品检验检验记录返工表.*,原ERP物料编号 ,生产记录生产检验单主表.原规格型号 from 成品检验检验记录返工表,生产记录生产检验单主表,基础数据物料信息表 
            //                         where 成品检验检验记录返工表.生产检验单号 = 生产记录生产检验单主表.生产检验单号  and 生产记录生产检验单主表.检验日期>='{0}'
            //                          and 基础数据物料信息表.物料编码 =生产记录生产检验单主表.物料编码 and 生产记录生产检验单主表.检验日期<='{1}')a  
            //                        group by a.原ERP物料编号 ,a.原规格型号,返工原因 order by 数量",
            //                   barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));
            string sql_1 = string.Format(@"select a.物料编码 ,a.规格型号,返工原因,SUM(数量)as 数量,SUM(a.未检验数量)as 未检验数量,SUM(a.送检数量)as 送检数量,SUM(a.已检验数量)as 已检验数量,SUM(a.合格数量)as 合格数量,SUM(a.不合格数量)as 不合格数量,substring(CONVERT(nvarchar,round(SUM(a.合格数量)/SUM(a.已检验数量)*100,2))+'%',0,6)+'%' as 合格率 from 
                        (select 快速检验检验记录返工表.*,base.物料编码,base.规格型号,b.未检验数量,b.送检数量,b.已检验数量,b.合格数量,b.不合格数量 from 快速检验检验记录返工表,快速检验生产检验单主表 b,基础数据物料信息表 base
                         where 快速检验检验记录返工表.生产检验单号 = b.生产检验单号  and b.检验日期>='{0}'
                          and base.物料编码 =b.物料编码 and b.检验日期<='{1}' and b.合格数量 !=0)a  
                          group by a.物料编码 ,a.规格型号,返工原因 order by 数量",
                   barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));

            DataTable dt_mx = new DataTable();
            dt_mx = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
            gridControl1.DataSource = dt_mx;


        }
        /// <summary>
        /// 17/6/15 弃用
        /// </summary>
//        private void fun_load()
//        {
//            //产品,总不合格数量
//            string sql = string.Format(@"select a.物料编码,a.规格型号,SUM(数量)as 数量  from 
//                (select 成品检验检验记录返工表.*,物料编码,规格型号 from 成品检验检验记录返工表,生产记录生产检验单主表 jyz 
//                         where 成品检验检验记录返工表.生产检验单号 = jyz.生产检验单号 and jyz.检验日期>='{0}'
//                           and jyz.检验日期<='{1}')a 
//                        group by a.物料编码,a.规格型号 ", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));

//            //然后 该产品 group by 不合格原因 数量
//            dt_产品 = new DataTable();
//            dt_产品 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
//            foreach (DataRow dr in dt_产品.Rows)
//            {
//                DataRow r = dt_左.NewRow();
//                r["规格型号"] = dr["规格型号"];
//                r["返工数量"] = dr["数量"];
//                dt_左.Rows.Add(r);
//                string sql_1 = string.Format(@"select a.物料编码,a.规格型号,返工原因,SUM(数量)as 数量  from 
//                        (select 成品检验检验记录返工表.*,物料编码,规格型号 from 成品检验检验记录返工表,生产记录生产检验单主表 jyz
//                         where 成品检验检验记录返工表.生产检验单号 = jyz.生产检验单号  and jyz.检验日期>='{0}'
//                           and jyz.检验日期<='{1}' )a where 物料编码 ='{2}' 
//                        group by a.物料编码,a.规格型号,返工原因 ",
//                    barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1), dr["物料编码"].ToString());
//                DataTable dt_mx = new DataTable();
//                dt_mx = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
//                foreach (DataRow dr_mx in dt_mx.Rows)
//                {
//                    DataRow r_1 = dt_左.NewRow();
//                    r_1["物料编码"] = dr_mx["物料编码"];
//                    r_1["返工原因"] = dr_mx["返工原因"].ToString().Trim();
//                    r_1["返工数量"] = dr_mx["数量"];
//                    dt_左.Rows.Add(r_1);
//                }
//                gridControl1.DataSource = dt_左;

//            }
//            DataRow x = dt_左.NewRow();
//            int i = 0;
//            foreach (DataRow y in dt_左.Rows)
//            {
//                i += Convert.ToInt32(y["返工数量"]);
//            }
//            x["返工原因"] = "总计:";
//            x["返工数量"] = i / 2;
//            dt_左.Rows.Add(x);


//        }
        //查询
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            //ui返工原因按产品_Load(null, null);

            fun_loadxin();
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string sql = string.Format(@"select 生产工单号,jyz.生产检验单号,返工数量,base.规格型号,大类,jyz.负责人员,jyz.检验日期 
                                from 快速检验检验记录返工表,快速检验生产检验单主表 jyz,基础数据物料信息表 base  where base.物料编码=jyz.物料编码 and
                                 快速检验检验记录返工表.生产检验单号=jyz.生产检验单号  and   检验日期>='{0}' and 检验日期<='{1}'
                                and base.物料编码='{2}' and 返工原因='{3}' order by 返工数量", Convert.ToDateTime(barEditItem1.EditValue), Convert.ToDateTime(barEditItem2.EditValue).AddDays(1)
                                 , dr["物料编码"], dr["返工原因"].ToString().Trim());
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl2.DataSource = dt;
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;
                gridControl1.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {

            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }






    }
}
