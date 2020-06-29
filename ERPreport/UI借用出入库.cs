using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ERPreport
{
    public partial class UI借用出入库 : UserControl
    {

        string str_条件 = "";
        string str_条件2 = "";

        string strcon = CPublic.Var.strConn;


        public UI借用出入库()
        {
            InitializeComponent();
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
                if (xtraTabControl1.SelectedTabPage.Name == "xtraTabPage1")
                {
                    gridView1.ExportToXlsx(saveFileDialog.FileName);
                }
                else if (xtraTabControl1.SelectedTabPage.Name == "xtraTabPage2")
                {
                    gridView2.ExportToXlsx(saveFileDialog.FileName);
                }
                else  
                {
                    gridView3.ExportToXlsx(saveFileDialog.FileName);
                }
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void UI借用出入库_Load(object sender, EventArgs e)
        {
            try
            {
                string sql = "select 物料编码,物料名称,大类,小类,规格型号 from 基础数据物料信息表 ";
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                searchLookUpEdit1.Properties.DataSource = dt;
                searchLookUpEdit1.Properties.ValueMember = "物料编码";
                searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                comboBox1.Text = (CPublic.Var.getDatetime().Month - 1).ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_check()
        {
            if (checkBox1.Checked == true)
            {
                if (dateEdit1.EditValue == null || dateEdit2.EditValue == null || dateEdit1.EditValue.ToString() == "" || dateEdit2.EditValue.ToString() == "")
                {
                    throw new Exception("未选择申请时间");
                }
            }

            if (checkBox4.Checked == true)
            {
                if (dateEdit3.EditValue == null || dateEdit4.EditValue == null || dateEdit3.EditValue.ToString() == "" || dateEdit4.EditValue.ToString() == "")
                {
                    throw new Exception("未选择归还时间");
                }
            }
            if (checkBox2.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择物料");

                }

            }
            if (checkBox3.Checked == true)
            {

                if (textBox1.Text == "")
                {
                    throw new Exception("未填写单号");

                }
            }
        }
        private void fun_条件()
        {
            if (checkBox1.Checked == true)
            {
                str_条件 = string.Format(" and mx.申请日期>'{0}' and mx.申请日期<'{1}'",
                    Convert.ToDateTime(dateEdit1.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).ToString("yyyy-MM-dd"));
                str_条件2 = string.Format(" and mx.申请日期>'{0}' and mx.申请日期<'{1}'",
                      Convert.ToDateTime(dateEdit1.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).ToString("yyyy-MM-dd"));
            }

            if (checkBox4.Checked == true)
            {
                str_条件 = string.Format(" and 归还日期>='{0}' and 归还日期<='{1}'",
                    Convert.ToDateTime(dateEdit3.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dateEdit4.EditValue).AddDays(1).ToString("yyyy-MM-dd"));
            }
            if (checkBox2.Checked == true)
            {
                str_条件 = string.Format(" and back.物料编码='{0}'", searchLookUpEdit1.EditValue.ToString());
                str_条件2 = string.Format(" and mx.物料编码='{0}'", searchLookUpEdit1.EditValue.ToString());

            }
            if (checkBox3.Checked == true)
            {
                str_条件 = string.Format(" and  (back.申请批号='{0}' or  mx.申请批号 ='{0}')", textBox1.Text);
                str_条件2 = string.Format(" and  mx.申请批号='{0}'", textBox1.Text);

            }
        }
        private void fun_search()
        {
//            string sql = string.Format(@"select a.申请批号 as 归还单号,a.申请批号明细 归还明细号 ,a.物料编码 归还料号,a.物料名称 归还物料,a.归还数量,a.归还日期,e.申请人
//   ,c.申请批号 as 借用批号,c.申请批号明细 借用明细号,c.物料编码 借出料号,c.物料名称 借出物料,d.出入库时间 借出时间,c.实际借用数量,e.备注 from 借还申请表归还记录 a
//  left join 借还申请批量归还关联 b on b.关联批号 = a.申请批号
//  left join 借还申请表附表 c on c.申请批号 = b.归还批号
//  left join 借还申请表 e on e.申请批号 = c.申请批号
//  left  join 仓库出入库明细表 d  on  d.明细号=c.申请批号明细 where 1=1  {0}  order by a.申请批号 ", str_条件);
            string sql = string.Format(@"select  back.*,申请数量 as 借出数量,借还申请表.申请日期,申请人 as 借用人,借用类型,借还申请表.相关单位 from 借还申请表归还记录 back
  left  join 借还申请表附表 mx on back.借用申请明细号 =mx.申请批号明细
  left join 借还申请表  on  借还申请表.申请批号=mx.申请批号   where 1=1  {0}  order by back.申请批号", str_条件);



            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dt;
            //            sql = string.Format(@"select a.申请批号,a.申请批号明细,a.物料编码 申请料号,a.物料名称 申请物料,a.规格型号 申请规格,实际借用数量,
            //            b.备注,出入库时间 借用时间 ,b.申请人 from  借还申请表附表 a 
            //   left  join 借还申请表 b on a.申请批号=b.申请批号 
            //   left  join 仓库出入库明细表 d on d.明细号=a.申请批号明细
            //  where  /*b.借还状态='已领取物料' */ a.申请批号 not in (select 归还批号  from 借还申请批量归还关联 ) and  a.实际借用数量 is not null {0}
            //  order by 申请料号,申请批号 ", str_条件2);
            //2019-12-3 借还申请表.借用人员对应界面上的负责人
            sql = string.Format(@"select  mx.*,申请人 as 借用人,借还申请表.申请日期,借还申请表.相关单位,借还申请表.借用人员  from 借还申请表附表 mx
 
  left join 借还申请表  on  借还申请表.申请批号=mx.申请批号
   where mx.领取完成=1 and  归还完成 =0 {0}", str_条件2);

            DataTable dt_1 = new DataTable();
            dt_1 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl2.DataSource = dt_1;
            str_条件 = "";
            str_条件2 = "";

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_check();
                fun_条件();
                fun_search();
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dateTimePicker1.Text == "" || comboBox1.Text == "")
                {
                    throw new Exception("时间未选择");
                }

                string sql = string.Format(@"select a.*,b.借用人员 from 历史借用未归还记录 a left join 借还申请表 b on a.申请批号 = b.申请批号  where 年='{0}' and 月='{1}'", dateTimePicker1.Text, comboBox1.Text);
                DataTable dt = new DataTable();
                dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                gridControl3.DataSource = dt;
                gridView3.ViewCaption = string.Format("{0}年{1}月历史记录", dateTimePicker1.Text, comboBox1.Text);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void gridView3_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
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

        private void gridView2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
            }
        }
    }
}
