using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ERPStock
{
    public partial class frm呆滞品查询界面 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        DateTime t_today = CPublic.Var.getDatetime().Date;
        public frm呆滞品查询界面()
        {
            InitializeComponent();
            barEditItem1.EditValue = System.DateTime.Today.AddMonths(-6);
            barEditItem2.EditValue = System.DateTime.Today;
        }

        private void frm呆滞品查询界面_Load(object sender, EventArgs e)
        {
           // barLargeButtonItem1_ItemClick(null, null);
        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           DateTime dtime=Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1);
           string sql = string.Format(@"select 仓库物料数量表.*,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.n原ERP规格型号,n核算单价,case when (仓库物料数量表.物料编码 in 
                         (select 目标物料编码 from 改制对应关系表 )) then '是' else '否'  end as 可改制 from 仓库物料数量表
                         left join 基础数据物料信息表 on 基础数据物料信息表.物料编码=仓库物料数量表.物料编码 where 仓库物料数量表.物料编码 not in
                        (select  物料编码 from 仓库出入库明细表 where 出入库时间>='{0}' and 出入库时间<='{1}' group by 物料编码) ", barEditItem1.EditValue, dtime);
            dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;
        }

        private void fun_load(DateTime t1,DateTime t2)
        {


            string sql = string.Format(@"select 仓库物料数量表.*,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.n原ERP规格型号,n核算单价,case when (仓库物料数量表.物料编码 in 
                         (select 目标物料编码 from 改制对应关系表 )) then '是' else '否'  end as 可改制 from 仓库物料数量表
                         left join 基础数据物料信息表 on 基础数据物料信息表.物料编码=仓库物料数量表.物料编码 where 仓库物料数量表.物料编码 not in
                        (select  物料编码 from 仓库出入库明细表 where 出入库时间>='{0}' and 出入库时间<='{1}' group by 物料编码) ", t1, t2);
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
             
            gridControl1.DataSource = dtM;
            
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (button1.Text == "显示可改制")
            {

                DateTime dtime = Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1);
                string sql = string.Format(@"select 仓库物料数量表.*,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.n原ERP规格型号,货架描述,n核算单价, from 仓库物料数量表
               left join 基础数据物料信息表 on 基础数据物料信息表.物料编码=仓库物料数量表.物料编码 where 仓库物料数量表.物料编码 not in
               (select  物料编码 from 仓库出入库明细表 where 实效时间>='{0}' and 实效时间<='{1}') and  仓库物料数量表.物料编码 in 
               (select 目标物料编码 from 改制对应关系表 ) ", barEditItem1.EditValue, dtime);
                dtM = new DataTable();
                dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                gridControl1.DataSource = dtM;


                button1.Text = "显示所有";
            }
            else
            {
                barLargeButtonItem1_ItemClick(null, null);
                button1.Text = "显示可改制";
            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (dr != null)
            {
                string sql = string.Format(@"select 改制对应关系表.*,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.规格型号,仓库物料数量表.货架描述,n核算单价,基础数据物料信息表.规格型号,库存总数 from 改制对应关系表 
                                                left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 =  改制对应关系表.可改制物料编码
                                                left join 仓库物料数量表  on 仓库物料数量表.物料编码 =  改制对应关系表.可改制物料编码
                                                where 目标物料编码='{0}'", dr["物料编码"]);
                 DataTable dt_改制列表 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                 gridControl2.DataSource = dt_改制列表;

            }
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                gridView1.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
            }
        }

        private void 维护改制对应关系ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            ERPproduct.frm改制对应关系维护 frm = new ERPproduct.frm改制对应关系维护(dr["物料编码"].ToString());
            CPublic.UIcontrol.AddNewPage(frm, "改制对应关系维护");
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();
             

            }
        }
        //三年以上呆滞品
        private void button2_Click(object sender, EventArgs e)
        {
            fun_load(t_today.AddYears(-3), t_today.AddDays(1));
        }
       
   
        //一年
        private void button5_Click(object sender, EventArgs e)
        {
            fun_load(t_today.AddYears(-1), t_today.AddDays(1));

        }
        //两年
        private void button4_Click(object sender, EventArgs e)
        {
            fun_load(t_today.AddYears(-2), t_today.AddDays(1));
        }

 
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
             
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions(DevExpress.XtraPrinting.TextExportMode.Text, false, false);

                gridControl1.ExportToXlsx(saveFileDialog.FileName, options);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

       

     
    }
}
