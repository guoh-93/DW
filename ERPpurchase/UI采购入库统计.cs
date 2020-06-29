using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace ERPpurchase
{
    public partial class UI采购入库统计 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";

        public UI采购入库统计()
        {
            InitializeComponent();
        }

        private void UI采购入库统计_Load(object sender, EventArgs e)
        {
            DateTime t = CPublic.Var.getDatetime().Date;
            dateEdit1.EditValue = t.AddMonths(-1);
            dateEdit2.EditValue =t;
            fun_load();
        }
        private void fun_显示列()
        {
            if (!CPublic.Var.LocalUserTeam.Contains("管理员") && !CPublic.Var.LocalUserTeam.Contains("采购") && !CPublic.Var.LocalUserTeam.Contains("财务"))
            {
                gridColumn9.Visible = false;
                gridColumn11.Visible = false;
                gridColumn17.Visible = false;
                gridView1.OptionsCustomization.AllowQuickHideColumns = false;
                gridView1.OptionsMenu.EnableColumnMenu = false;

            }
        }

        private void fun_load()
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name)))
            {

                gridView1.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
            }
            fun_显示列();

            string sql = string.Format(@"select left(明细类型+出库入库,4) as 出入库类型 from 仓库出入库明细表 group by 明细类型+出库入库");
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            DataTable dt_客户 = new DataTable();
            da.Fill(dt_客户);


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
            string sql_仓库 = "SELECT [属性值] as 仓库名称,属性字段1 as 仓库号 from [基础数据基础属性表] where 属性类别 ='仓库类别'";
            DataTable dt_仓库 = new DataTable();
            SqlDataAdapter da_仓库 = new SqlDataAdapter(sql_仓库, strcon);
            da_仓库.Fill(dt_仓库);
            searchLookUpEdit1.Properties.DataSource = dt_仓库;
            searchLookUpEdit1.Properties.ValueMember = "仓库号";
            searchLookUpEdit1.Properties.DisplayMember = "仓库名称";
            string sql4 = "select 物料编码,规格型号 from 基础数据物料信息表 where 停用=0";
            DataTable dt_物料 = new DataTable();
            SqlDataAdapter da_物料 = new SqlDataAdapter(sql4, strcon);
            da_物料.Fill(dt_物料);
            searchLookUpEdit5.Properties.DataSource = dt_物料;
            searchLookUpEdit5.Properties.ValueMember = "物料编码";
            searchLookUpEdit5.Properties.DisplayMember = "物料编码";


        }

        private void fun_check()
        {
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
            string sql = string.Format(@" select a.*,采购价,税率,采购价/(1.000000+税率/100.000000) 不含税单价,采购价/(1.000000+税率/100.000000)*实效数量 as 不含税金额 from
                     (select 明细类型,出库入库,单号,明细号,相关单号,ccrmx.仓库名称,ccrmx.仓库号,实效数量,出入库时间 ,ccrmx.物料名称,base.规格型号,相关单位,大类,小类
                     ,ccrmx.物料编码,n核算单价,base.计量单位,left(明细类型+出库入库,4)as 查询类型
                     from  仓库出入库明细表 ccrmx,基础数据物料信息表 base where ccrmx.物料编码=base.物料编码 and 出入库时间>'{0}' and 出入库时间<='{1}')a
                     left join 采购记录采购单明细表 cmx on cmx.采购明细号= a.相关单号 where (a.明细类型 = '采购入库'  or a.明细类型='采购退货')",
                 dateEdit1.EditValue, Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1));

         
            if (checkBox2.Checked == true)
            {
                sql = sql + string.Format(" and cmx.仓库号='{0}'", searchLookUpEdit1.EditValue.ToString());

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

            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dt;
            MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
            DataTable dt_汇总=new DataTable ();
            dt_汇总.Columns.Add("物料编码");
            dt_汇总.Columns.Add("大类");
            dt_汇总.Columns.Add("小类");
            dt_汇总.Columns.Add("物料名称");
            dt_汇总.Columns.Add("规格型号");
            dt_汇总.Columns.Add("相关单位");
            dt_汇总.Columns.Add("入库总数",typeof(decimal));

            dt_汇总 = RBQ.SelectGroupByInto("", dt, "物料编码,大类,小类,物料名称,规格型号,相关单位,实效数量,sum(实效数量) 入库总数", "",
                                                   "物料编码,大类,小类,物料名称,规格型号,相关单位");
            gridControl2.DataSource = dt_汇总;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
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

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                gridControl1.ExportToXlsx(saveFileDialog.FileName);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
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




    }
}
