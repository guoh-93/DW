using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class ui外加工查询 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {


        #region
        string strcon = CPublic.Var.strConn;
        DataTable dtM;

        #endregion

        public ui外加工查询()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
          
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc, new Point(e.X, e.Y));
                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();

            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format(@"select  生产工单号,b.物料编码,b.规格型号,b.物料名称,生产数量,a.完成,标记  from  生产记录生产工单表 a ,基础数据物料信息表 b
            where 工单负责人='张月清' and   a.物料编码=b.物料编码    and 生效日期>'{0}'  and a.关闭=0
          and 生效日期<'{1}'" , barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dtM=new DataTable ();
                da.Fill(dtM);
             
                gc.DataSource = dtM;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void ui外加工查询_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DateTime t = CPublic.Var.getDatetime().Date;
            barEditItem1.EditValue = t.AddMonths(-6);
            barEditItem2.EditValue = t;

            fun_load();
        }

        private void 标记ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow r = gv.GetDataRow(gv.FocusedRowHandle);

                if (Convert.ToBoolean(r["完成"].ToString()))
                {

                    string sql = string.Format(@"select * FROM 生产记录生产工单表 where 生产工单号='{0}' ", r["生产工单号"].ToString());
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dt.Rows[0]["标记"] = true;
                        new SqlCommandBuilder(da);
                        da.Update(dt);

                    }
                    fun_load();
                }
                else
                {
                    MessageBox.Show("该工单尚未全部入库");
                }
         
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_load();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
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
    }
}
