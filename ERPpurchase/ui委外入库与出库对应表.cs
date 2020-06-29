using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace ERPpurchase
{
    public partial class ui委外入库与出库对应表 : UserControl
    {
        #region variable
        DataTable dt_核销明细, dt_出库;
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";
        #endregion

        public ui委外入库与出库对应表()
        {
            InitializeComponent();
        }

        private void ui委外入库与出库对应表_Load(object sender, EventArgs e)
        {
            DateTime t = CPublic.Var.getDatetime();
            DateTime t1 = new DateTime(t.Year, t.Month, 1);
            DateTime t2 = t1.AddMonths(1).AddSeconds(-1);
            dateEdit1.EditValue = dateEdit3.EditValue = t1;
            dateEdit2.EditValue = dateEdit4.EditValue = t2;
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(this.tabControl1, this.Name, cfgfilepath);
        }
        private string fun_条件1()
        {
            string s = "";
            if (checkBox1.Checked)
            {
                DateTime t1 = Convert.ToDateTime(dateEdit3.EditValue);
                DateTime t2 = Convert.ToDateTime(dateEdit4.EditValue);
                s = string.Format("核销日期>'{0}' and 核销日期<'{1}'", t1, t2);

            }
            if (checkBox2.Checked)
            {
                DateTime t3 = Convert.ToDateTime(dateEdit6.EditValue);
                DateTime t4 = Convert.ToDateTime(dateEdit5.EditValue);
                s = string.Format("g.生效日期>'{0}' and g.生效日期<'{1}'", t3, t4);
            }
            if (s == "")
            {
                throw new Exception("未选择任何时间条件");
            }
            return s;
        }
        private void fun_委外核销(string s_condition)
        {
            string s = string.Format(@"select 采购单明细号,a.入库单号,采购单类型,b.供应商,b.税率,c.物料名称 as  加工入库物料名称,经办人,c.物料编码 as 加工入库物料,c.规格型号 as 加工入库物料规格,a.采购数量,入库量 , 
            a.生效日期 as 入库日期,d.其他出库明细号,e.物料编码  as 委外物料,e.物料名称 as 委外物料名称,e.规格型号 as 委外物料规格
            ,物料核销数,核销日期,核销人员,g.数量 as 出库数量,(g.数量-g.委外已核量)该单未核物料数 ,f.原因分类 as 出库原因   from [采购记录采购单入库明细] a    left join 采购记录采购单主表 b on a.采购单号=b.采购单号   
                left join 基础数据物料信息表 c on c.物料编码=a.物料编码      left  join 委外核销明细表  d  on  d.入库单号=a.入库单号
                left  join 其他出库子表 g on g.其他出库明细号=d.其他出库明细号 
                left  join 其他出入库申请主表 f on g.出入库申请单号=f.出入库申请单号 
                left join 基础数据物料信息表 e on e.物料编码=d.子项编码  where  采购单类型='委外采购' and a.作废=0 and {0}", s_condition);
            dt_核销明细 = new DataTable();
            dt_核销明细 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gc_核销明细.DataSource = dt_核销明细;
        }
        private void fun_出库(DateTime t1, DateTime t2)
        {
            string s = string.Format(@"select 采购单明细号,入库单号,采购单类型,b.供应商,b.税率,c.物料名称 as  加工入库物料名称,经办人,c.物料编码 as 加工入库物料,c.n原ERP规格型号 as 加工入库物料规格,a.采购数量,入库量 , 
  a.生效日期 as 入库日期,其他出库明细号,d.物料编码 as 委外物料,d.物料名称 as 委外物料名称,e.n原ERP规格型号 as 委外物料规格,d.委外已核量,d.数量 as 出库数量,原因分类,d.生效日期 as 出库日期    
  from [采购记录采购单入库明细] a  
  left join 采购记录采购单主表 b on a.采购单号=b.采购单号   
  left join 基础数据物料信息表 c on c.物料编码=a.物料编码 
  left  join 其他出入库申请子表 h on h.备注=a.采购单明细号
  left  join 其他出库子表 d  on d.出入库申请明细号=h.出入库申请明细号
  left  join 其他出入库申请主表 f on f.出入库申请单号=d.出入库申请单号
    left join 基础数据物料信息表 e on e.物料编码=d.物料编码 
  where 采购单类型='委外采购' and a.生效日期>'{0}' and a.生效日期<'{1}' and a.作废=0 ", t1, t2);
            dt_出库 = new DataTable();
            dt_出库 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gridControl1.DataSource = dt_出库;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {

            try
            {
                DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue);
                DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue);
                fun_出库(t1, t2);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                string s = fun_条件1();
                fun_委外核销(s);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (ActiveControl != null && ActiveControl.GetType().Equals(gc_核销明细.GetType()))
            {

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    DevExpress.XtraGrid.GridControl gc = (ActiveControl) as DevExpress.XtraGrid.GridControl;
                    gc.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            else
            {

                MessageBox.Show("若要导出请先选中要导出的表格");
            }
        }

        private void gridView2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView2.GetFocusedRowCellValue(gridView2.FocusedColumn));
                e.Handled = true;
            }
        }

        private void 撤回ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认撤回核销吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                    DataRow []rr= dt_核销明细.Select(string.Format("入库单号='{0}'", r["入库单号"]));
                    string s = string.Format(@"delete 委外核销明细表 where 入库单号 ='{0}' 
                    update 采购记录采购单入库明细  set 委外核销=0 where 入库单号 ='{0}'  ", r["入库单号"]);
                    foreach(DataRow drr in rr)
                    {
                       s = s+string.Format("update 其他出库子表 set 委外已核量=委外已核量-{0} where 其他出库明细号 ='{1}'",drr["物料核销数"],drr["其他出库明细号"]);
                        dt_核销明细.Rows.Remove(drr);
                    }

                    CZMaster.MasterSQL.ExecuteSQL(s,strcon);

                    MessageBox.Show("撤销成功");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc_核销明细, new Point(e.X, e.Y));
                gridView1.CloseEditor();
                contextMenuStrip1.Tag = gc_核销明细;

            }
        }

        private void gridView2_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            DataRow t1 = gridView2.GetDataRow(e.RowHandle1);
            DataRow t2 = gridView2.GetDataRow(e.RowHandle2);
            string s1 = t1["采购单明细号"].ToString();
            string s2 = t2["采购单明细号"].ToString();


            string r1 = t1["入库单号"].ToString();
            string r2 = t2["入库单号"].ToString();
            if (e.Column.FieldName != "采购单明细号" )
            {
                if (e.Column.FieldName =="入库量" && r1 != r2)
                {
                    e.Merge = false;
                    e.Handled = true;
                }
                else if (e.Column.FieldName == "委外物料"|| e.Column.FieldName == "委外已核量" ||  e.Column.FieldName == "出库数量" || e.Column.FieldName == "出库日期")
                {
                    if (t1["其他出库明细号"].ToString() != t2["其他出库明细号"].ToString())
                    {
                        e.Merge = false;
                        e.Handled = true;
                    }
                }
                else
                {
                    if(s1!=s2)
                    {
                        e.Merge = false;
                        e.Handled = true;
                    }
                }
            }

        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
            }
        }



    }
}
