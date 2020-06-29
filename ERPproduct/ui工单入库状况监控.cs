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
    public partial class ui工单入库状况监控 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {

        #region 变量
        string strcon = CPublic.Var.strConn;
        DataTable dtM = new DataTable();
  


        #endregion 
        public ui工单入库状况监控()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_check();
                fun_search();
                decimal dec_总条数 = 0;
                decimal dec_生产 = 0;
                decimal dec_rk = 0;
      


                DataView dv = new DataView(dtM);
                dec_总条数 = dtM.Rows.Count;
                dv.RowFilter = "生产达成=1";
                dec_生产 = dv.Count;
                dv = new DataView(dtM);
                dv.RowFilter = "入库达成=1";
                dec_rk = dv.Count;

                label4.Text = "总条数：" + dec_总条数.ToString();
                label5.Text = "生产达成：" + dec_生产.ToString() + "  " + "未达成：" + (dec_总条数 - dec_生产).ToString();
                label7.Text = "入库达成：" + dec_rk.ToString() + "  未达成：" + (dec_总条数 - dec_rk).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {
           

            if (checkBox2.Checked == true)
            {
                if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == "")
                {
                    throw new Exception("未选择车间");
                }

            }
            if (checkBox4.Checked == true)
            {
                if (dateEdit5.EditValue == null || dateEdit6.EditValue == null || dateEdit5.EditValue.ToString() == "" || dateEdit6.EditValue.ToString() == "")
                {
                    throw new Exception("未选择计划确认日期");
                }

            }
          

        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            DateTime today = CPublic.Var.getDatetime().Date;
            dateEdit1.EditValue = Convert.ToDateTime(today.AddMonths(-1).ToString("yyyy-MM-dd"));
            dateEdit2.EditValue = Convert.ToDateTime(today.ToString("yyyy-MM-dd"));

            string sql_车间 = @"select 属性值 as 生产车间,属性字段1 as 车间编号 from [基础数据基础属性表] where 属性类别='课室' 
            and 属性值 like '制造_课%' and 属性字段1 <>''";
            DataTable dt_车间 = new DataTable();
            SqlDataAdapter da_车间 = new SqlDataAdapter(sql_车间, strcon);
            da_车间.Fill(dt_车间);
            searchLookUpEdit2.Properties.DataSource = dt_车间;
            searchLookUpEdit2.Properties.ValueMember = "车间编号";
            searchLookUpEdit2.Properties.DisplayMember = "生产车间";
  
     
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_search()
#pragma warning restore IDE1006 // 命名样式
        {
            string str_条件 = "";


            if (checkBox2.Checked == true)
            {
                str_条件 = str_条件 + string.Format(" and b.生产车间='{0}'", searchLookUpEdit2.EditValue.ToString());

            }

            if (checkBox4.Checked == true)
            {
                str_条件 = str_条件 + string.Format(" and  预计完工日期>='{0}' and 预计完工日期<='{1}'", dateEdit5.EditValue.ToString(), Convert.ToDateTime(dateEdit6.EditValue).AddDays(1).AddSeconds(-1));
            }
           
            //case when(aa.受订量 > isnull(s.制令量,0)) then aa.受订量 else isnull(s.制令量,0) end as 受订量a
            string sql = string.Format(@"select  a.生产工单号,生产检验单号,原ERP物料编号,c.物料名称,c.n原ERP规格型号,a.生产数量,送检数量,合格数量
            ,case when(包装时间 is null or 包装时间>预计完工日期+1) then CONVERT(bit,0) else CONVERT(bit,1) end as 生产达成
            ,case when(a.完成日期>预计完工日期+1 or a.完成日期 is null) then CONVERT(bit,0) else CONVERT(bit,1) end as 入库达成

            ,a.完成日期 as 入库时间,包装时间,预计完工日期  as 计划确认日期  from 生产记录生产检验单主表  a
             left  join 生产记录生产工单表 b on  b.生产工单号=a.生产工单号
             left join 基础数据物料信息表 c on a.物料编码=c.物料编码 where b.生效日期 >'{0}' and b.生效日期<'{1}' {2} ", dateEdit1.EditValue, Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1), str_条件);
            dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;
      
            //dtM.Columns.Add("入库达成", typeof(bool));
            //foreach (DataRow dr in dtM.Rows)
            //{
            //    if ()
            //    {
            //        dr["生产达成"] = true;

            //    }
            //    else
            //    {
            //        dr["生产达成"] = false;

            //    }

            //}


        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView2_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView2.GetFocusedRowCellValue(gridView2.FocusedColumn));
                e.Handled = true;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DevExpress.XtraGrid.GridControl gc = (ActiveControl) as DevExpress.XtraGrid.GridControl;
            if (gc == null)
            {
                MessageBox.Show("未选择需导出哪个表格");
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions(DevExpress.XtraPrinting.TextExportMode.Text, false, false);

                gc.ExportToXlsx(saveFileDialog.FileName, options);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void ui工单入库状况监控_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_load();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

     
    }
}
