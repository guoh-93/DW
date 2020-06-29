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
    public partial class ui成品检验日报表 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        public ui成品检验日报表()
        {
            InitializeComponent();
        }

        private void ui成品检验日报表_Load(object sender, EventArgs e)
        {
            DateTime t = CPublic.Var.getDatetime();
            barEditItem1.EditValue = t.AddDays(-1);
            barEditItem2.EditValue = t;
           
        }
        //导出
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
                gc.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //关闭
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        //查询
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_加载();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_加载()
        {
//            string str = string.Format(@"select a.物料编码 ,a.规格型号,返工原因,SUM(数量)as 数量,SUM(a.未检验数量)as 未检验数量,SUM(a.送检数量)as 送检数量,SUM(a.已检验数量)as 已检验数量,SUM(a.合格数量)as 合格数量,SUM(a.不合格数量)as 不合格数量,
//                       substring(CONVERT(nvarchar,round(SUM(a.合格数量)/SUM(a.已检验数量)*100,2))+'%',0,6)+'%' as 合格率 from 
//                        (select 成品检验检验记录返工表.*,base.物料编码,base.规格型号,b.未检验数量,b.送检数量,b.已检验数量,b.合格数量,b.不合格数量 from 成品检验检验记录返工表,生产记录生产检验单主表 b,基础数据物料信息表 base
//                         where 成品检验检验记录返工表.生产检验单号 = b.生产检验单号  and b.检验日期>='{0}'
//                          and base.物料编码 =b.物料编码 and b.检验日期<='{1}' and b.合格数量 !=0)a  
//                          group by a.物料编码 ,a.规格型号,返工原因 order by 数量",
//                   barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));

            string str = string.Format(@" select b.生产工单号,a.*, substring(CONVERT(nvarchar,round((a.合格数量)/(a.已检验数量)*100,2))+'%',0,6)+'%' as 合格率,a.负责人员 from 生产记录生产检验单主表 a 
                                           left join 生产记录生产工单表 b on a.生产工单号 = b.生产工单号 where a.检验日期>'{0}' and a.检验日期<='{1}'", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));

            dtM = CZMaster.MasterSQL.Get_DataTable(str, strconn);       
            dtM.Columns.Add("几种返工原因", typeof(string));
            dtM.Columns.Add("返工原因", typeof(string));
            DataColumn[] pk_bom = new DataColumn[1];
            pk_bom[0] = dtM.Columns["生产检验单号"];
            str = "select * from 成品检验检验记录返工表";
            DataTable dt_返工原因 = CZMaster.MasterSQL.Get_DataTable(str, strconn);
            DataColumn[] pk_bom1 = new DataColumn[1];
            pk_bom1[0] = dt_返工原因.Columns["ID"];

            //foreach (DataRow dr in dtM.Rows)
            //{
            //    DataTable dt_行数 = new DataTable();
            //    using (SqlDataAdapter da1 = new SqlDataAdapter("select * from 成品检验检验记录返工表 where 生产检验单号='" + dr["生产检验单号"] + "'",strconn))
            //{
            //    da1.Fill(dt_行数);
            //}
            //    dr["几种返工原因"] = dt_行数.Rows.Count.ToString() + "种";

            //}
            foreach (DataRow dr in dtM.Rows)
            {
                DataRow[] dr_返工 = dt_返工原因.Select(string.Format("生产检验单号 = '{0}'", dr["生产检验单号"]));
                if (dr_返工.Length > 0)
                {
                    foreach (DataRow dr1 in dr_返工)
                    {
                        dr["返工原因"] = dr["返工原因"].ToString() + dr1["返工原因"].ToString() + ";";
                    }
                }
                dr["几种返工原因"] = dr_返工.Length.ToString() + "种";
            }

            gc.DataSource = dtM;
           

        
        }

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr_选中行 = gv.GetDataRow(gv.FocusedRowHandle);
                using (SqlDataAdapter da = new SqlDataAdapter("select * from 成品检验检验记录返工表 where 生产检验单号='" + dr_选中行["生产检验单号"] + "'", strconn))
                {
                    DataTable dt_返工 = new DataTable();
                    da.Fill(dt_返工);
                    gc1.DataSource = dt_返工;

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        //判断颜色
        private void gv_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
          DataRow r = gv.GetDataRow(e.RowHandle);

                if (r["几种返工原因"].ToString()!="0种" )
                {
                    e.Appearance.BackColor = Color.Pink;
                }
            
        }


     
    }
}
