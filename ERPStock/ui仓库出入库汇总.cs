using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.Office;
using DevExpress.XtraPrinting;
using System.IO;

namespace ERPStock
{
    public partial class ui仓库出入库汇总 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable dtM = new DataTable();
        MasterMESWS.DataSetHelper dset = new MasterMESWS.DataSetHelper();
        string cfgfilepath = "";
        public ui仓库出入库汇总()
        {
            InitializeComponent();
        }

        private void ui仓库出入库汇总_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(this.panel3, this.Name, cfgfilepath);

                DateTime dtime = CPublic.Var.getDatetime();
                dtime = new DateTime(dtime.Year, dtime.Month, dtime.Day);

                dateEdit1.EditValue = dtime.AddMonths(-1);
                dateEdit2.EditValue = dtime;
                fun_load();
            }
            catch (Exception ex)
            {


            }
        }

        private void fun_load()
        {

            string sql = string.Format(@"select  明细类型   from 仓库出入库明细表 group by 明细类型");
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            DataTable dt_客户 = new DataTable();
            da.Fill(dt_客户);
            //searchLookUpEdit2.Properties.DataSource = dt_客户;
            //searchLookUpEdit2.Properties.DisplayMember = "明细类型";
            //searchLookUpEdit2.Properties.ValueMember = "明细类型";
            foreach (DataRow dr in dt_客户.Rows)
            {
                checkedComboBoxEdit1.Properties.Items.Add(dr["明细类型"].ToString());
            }



            string sql2 = "select 物料类型名称 from 基础数据物料类型表 where 类型级别 = '大类' order by 物料类型名称";
            DataTable dt = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(sql2, strcon);
            da2.Fill(dt);

            searchLookUpEdit3.Properties.DataSource = dt;
            searchLookUpEdit3.Properties.ValueMember = "物料类型名称";
            searchLookUpEdit3.Properties.DisplayMember = "物料类型名称";
            string sql3 = "select 物料类型名称 from 基础数据物料类型表 where 类型级别 = '小类' order by 物料类型名称";
            DataTable dt_小类 = new DataTable();
            SqlDataAdapter da1 = new SqlDataAdapter(sql3, strcon);
            da1.Fill(dt_小类);
            searchLookUpEdit4.Properties.DataSource = dt_小类;
            searchLookUpEdit4.Properties.ValueMember = "物料类型名称";
            searchLookUpEdit4.Properties.DisplayMember = "物料类型名称";
            string sql_仓库 = "SELECT [属性值] as 仓库名称,属性字段1 as 仓库号 FROM  [基础数据基础属性表] where 属性类别 ='仓库类别'";
            DataTable dt_仓库 = new DataTable();
            SqlDataAdapter da_仓库 = new SqlDataAdapter(sql_仓库, strcon);
            da_仓库.Fill(dt_仓库);
            searchLookUpEdit1.Properties.DataSource = dt_仓库;
            searchLookUpEdit1.Properties.ValueMember = "仓库号";
            searchLookUpEdit1.Properties.DisplayMember = "仓库名称";
            string sql4 = "select 物料编码,规格型号 from 基础数据物料信息表  ";
            DataTable dt_物料 = new DataTable();
            SqlDataAdapter da_物料 = new SqlDataAdapter(sql4, strcon);
            da_物料.Fill(dt_物料);
            searchLookUpEdit5.Properties.DataSource = dt_物料;
            searchLookUpEdit5.Properties.ValueMember = "物料编码";
            searchLookUpEdit5.Properties.DisplayMember = "物料编码";


        }
        private void fun_check()
        {
            if (checkBox1.Checked == true)
            {
               
                if (checkedComboBoxEdit1.EditValue == null || checkedComboBoxEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择出入库类型");
                }

            }
            if (checkBox2.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择仓库");
                }

            }
            if (checkBox3.Checked == true)
            {
                if (searchLookUpEdit3.EditValue == null || searchLookUpEdit3.EditValue.ToString() == "")
                {
                    throw new Exception("未选择大类");
                }
            }
            if (checkBox4.Checked == true)
            {
                if (searchLookUpEdit4.EditValue == null || searchLookUpEdit4.EditValue.ToString() == "")
                {
                    throw new Exception("未选择小类");
                }

            }
            if (checkBox5.Checked == true)
            {
                if (searchLookUpEdit5.EditValue.ToString() == "")
                {
                    throw new Exception("未选择物料");
                }

            }
            if (checkBox6.Checked == true)
            {
                if (textBox1.Text.ToString() == "")
                {
                    throw new Exception("未填写单号");
                }

            }

        }
        private void fun_search()
        {
            DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue);
            t1 = new DateTime(t1.Year, t1.Month, t1.Day);
            DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1);
            t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);

            string sql = string.Format(@"   select 明细类型,出库入库,明细号,相关单号,物料类型,a.仓库名称,a.仓库号,a.仓库人,实效数量,出入库时间,b.规格型号,产品线,a.物料名称  
   ,a.相关单位,大类,小类,a.物料编码,n核算单价,n核算单价*实效数量 as 金额 ,b.计量单位,left(明细类型+出库入库,4)as 查询类型,
   gd.物料编码 as 产品编码,gd.物料名称 as 产品名称,出库通知单明细号,原因分类  from  仓库出入库明细表  a  with (NOLOCK)
   left join 基础数据物料信息表  b   on  a.物料编码=b.物料编码   
   left join 销售记录成品出库单明细表 sa  with (NOLOCK) on sa.成品出库单明细号=明细号 
   left join 生产记录生产工单表 gd  with (NOLOCK)   on gd.生产工单号=相关单号
   left join 其他出入库申请主表 qtm  with (NOLOCK) on qtm.出入库申请单号 =相关单号 
   where  出入库时间>'{0}' and 出入库时间<='{1}'",
                 t1.ToString("yyyy-MM-dd"), t2);

            if (checkBox1.Checked == true)
            {
                //sql = sql + string.Format(" and left(明细类型+出库入库,4)='{0}'", searchLookUpEdit2.EditValue.ToString());
                string xx = checkedComboBoxEdit1.EditValue.ToString();
                string[] s = xx.Split(',');
                sql+= " and 明细类型 in (";
                foreach (string xs in s)
                {
                    sql += "'" + xs.Trim() + "',";
                }
                sql = sql.Substring(0, sql.Length - 1) + ")";
                
            }
            if (checkBox2.Checked == true)
            {
                sql = sql + string.Format(" and a.仓库号='{0}'", searchLookUpEdit1.EditValue.ToString());

            }
            if (checkBox3.Checked == true)
            {
                sql = sql + string.Format(" and 大类='{0}'", searchLookUpEdit3.EditValue.ToString());
            }
            if (checkBox4.Checked == true)
            {
                sql = sql + string.Format(" and 小类='{0}'", searchLookUpEdit4.EditValue.ToString());

            }
            if (checkBox5.Checked == true)
            {
                sql = sql + string.Format(" and a.物料编码='{0}'", searchLookUpEdit5.EditValue.ToString());
            }
            if (checkBox6.Checked == true)
            {
                sql = sql + string.Format(" and 相关单号 like '%{0}%' ", textBox1.Text);
            }
            dtM = new DataTable();
            // DataTable dt_rwf = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;
            // string str="明细类型,大类,小类,相关单位,物料名称,物料编码,规格型号,n核算单价,计量单位,仓库名称" ;
            //dt_rwf=dset.SelectGroupByInto("",dtM,str+",sum(实效数量) 数量,sum(金额) 金额","",str);
            //gridControl3.DataSource = dt_rwf;



        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                fun_check();
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

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dtM != null && dtM.Columns.Count > 0 && dtM.Rows.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";

                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {

                    DevExpress.XtraPrinting.XlsxExportOptions options = new XlsxExportOptions(TextExportMode.Text, false, false);

                    //if (xtraTabControl1.SelectedTabPage.Name == "xtraTabPage1")
                    //{
                    gridControl1.ExportToXlsx(saveFileDialog.FileName);
                    //ERPorg.Corg.TableToExcel(dtM, saveFileDialog.FileName);
                    MessageBox.Show("导出成功");
                    //}
                    //else
                    //{
                    //    ERPorg.Corg.TableToExcel(dtM_PurchasePool, saveFileDialog.FileName);
                    //    MessageBox.Show("导出成功");
                    //}
                    // DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("无记录可导出");
            }



        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridView3_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gridView3.GetDataRow(gridView3.FocusedRowHandle);
            DataView dv = new DataView(dtM);
            dv.RowFilter = string.Format("物料编码='{0}'", dr["物料编码"]);

            DataTable dt = dv.ToTable();
            gridControl2.DataSource = dt;

        }

        private void gridView1_CustomDrawRowIndicator_1(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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

        private void gridView3_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void gridView1_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
            }
        }

        private void gridView1_ColumnWidthChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gridView1.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void gridView1_ColumnPositionChanged(object sender, EventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gridView1.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
