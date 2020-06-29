using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;

namespace ERPproduct
{
    public partial class Ui客户购买信息统计 : UserControl
    {
        /// <summary>
        /// 用来 传递给 出入库明细的 起始时间
        /// </summary>
        DateTime t;
        DataTable dtM;
        public Ui客户购买信息统计()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                gridView1.ExportToXlsx(saveFileDialog.FileName);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Ui客户购买信息统计_Load(object sender, EventArgs e)
        {
            fun_load();

        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            DateTime t = CPublic.Var.getDatetime().Date;

            string sql = string.Format(@"select  片区,a.*,客户名称,f.物料编码,f.规格型号,规格,大类,小类,f.计量单位 ,isnull(月用量,0)月用量,isnull(季度用量,0)季度用量,isnull(半年用量,0)半年用量,年用量  from (select 客户编号  from 销售记录成品出库单明细表  where 生效日期>'2016-12-1' group by 客户编号   )a
  left  join (select 客户编号,物料编码,SUM(出库数量)年用量 from 销售记录成品出库单明细表   where 生效日期>'{0}' group by 客户编号,物料编码)e
on e.客户编号=a.客户编号  
left  join (select 客户编号,物料编码,SUM(出库数量)月用量 from 销售记录成品出库单明细表  where 生效日期>'{1}' group by 客户编号,物料编码)b
on b.客户编号=a.客户编号 and b.物料编码 =e.物料编码 
left  join (select 客户编号,物料编码,SUM(出库数量)季度用量 from 销售记录成品出库单明细表  where 生效日期>'{2}' group by 客户编号,物料编码)c
on c.客户编号=a.客户编号 and c.物料编码 =e.物料编码 
 left  join (select 客户编号,物料编码,SUM(出库数量)半年用量 from 销售记录成品出库单明细表  where 生效日期>'{3}' group by 客户编号,物料编码)d
on d.客户编号=a.客户编号 and d.物料编码 =e.物料编码 
left join 基础数据物料信息表 f  on f.物料编码=e.物料编码
left join  客户基础信息表 g on g.客户编号=a.客户编号
where  e.物料编码 is not null  order by 片区,客户名称,规格型号,大类,小类",t.AddYears(-1),t.AddMonths(-1),t.AddMonths(-3),t.AddMonths(-6));


            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                dtM = new DataTable();
                da.Fill(dtM);
                gridControl1.DataSource = dtM; 
            }


        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_load();
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void 查看明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);

            Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, @"ERPSale.dll"));
            Type outerForm = outerAsm.GetType("ERPSale.ui过往出库明细", false);
          //  Form ui = Activator.CreateInstance(outerForm) as Form;
            object []dic = new object[2];
            dic[0] = dr["物料编码"].ToString();
            dic[1] = t;
 
            UserControl ui = Activator.CreateInstance(outerForm, dic) as UserControl; // 过往出口明细 构造函数 有两个参数,string ,datetime 
            CPublic.UIcontrol.Showpage(ui,"过往出库明细");

           
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                if (e.Column.Caption.Substring(e.Column.Caption.Length - 2, 2)=="用量")
                {
                    contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                    gridView1.CloseEditor();
                    this.BindingContext[dtM].EndCurrentEdit();

                }
               
                DateTime tt=CPublic.Var.getDatetime().Date;
                if (e.Column.Caption == "月用量")
                {
                    t = tt.AddMonths(-1);
                }
                if (e.Column.Caption == "季度用量")
                {
                    t = tt.AddMonths(-3);

                }
                if (e.Column.Caption == "半年用量")
                {
                    t = tt.AddMonths(-6);
                }
                if (e.Column.Caption == "年用量")
                {
                    t = tt.AddYears(-1);
                }
            }
        }
    }
}
