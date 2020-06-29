using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace ERPreport
{
    public partial class ui其他出入库 : UserControl
    {

        #region 
        DataTable dtM = new DataTable();
        string strcon = CPublic.Var.strConn;

        string cfgfilepath = "";
        #endregion 

        public ui其他出入库()
        {
            InitializeComponent();
        }

        private void ui其他出入库_Load(object sender, EventArgs e)
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
           DateTime t=  CPublic.Var.getDatetime().AddMonths(-1);
           t =new DateTime (t.Year,t.Month,1) ;   //去上月月初 一般财务是要上个月的 数据
           dateEdit1.EditValue = t;
           dateEdit2.EditValue = t.AddMonths(1).AddSeconds(-1);


        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsExportOptions options = new DevExpress.XtraPrinting.XlsExportOptions();
                
                    gridView1.ExportToXlsx(saveFileDialog.FileName);
                
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }



        private void fun_search()
        {
            dtM = new DataTable();

            if (checkBox3.Checked == true)
            {
                if (comboBox1.Text=="其他出库")
                {
                    string sql = string.Format(@" select a.出入库申请单号 as 相关单号,明细号, 其他出库单号 as 单号,原因分类,b.申请类型,base.规格型号,base.物料编码 ,base.物料名称,业务单号,
 base.图纸编号,大类,小类,产品线,a.数量,a.生效日期 as 出库日期,b.备注,b.操作人员 as 申请人,b.部门名称 as 申请部门,    d.部门,c.姓名 as 操作人,crmx.仓库号,crmx.仓库名称
 from 其他出库子表 a,基础数据物料信息表 base,其他出入库申请主表 b,人事基础员工表 c,人事基础员工表 d,仓库出入库明细表 crmx  where a.物料编码=base.物料编码
  and a.生效日期 >'{0}' and   a.生效日期 <'{1}' and  c.员工号=a.生效人员编号  and d.员工号=b.操作人员编号
 and a.出入库申请单号=b.出入库申请单号 and crmx.明细号=a.其他出库明细号 ", Convert.ToDateTime(dateEdit1.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
                    dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

                }
                else if (comboBox1.Text == "其他入库")
                {

               string     sql = string.Format(@"select a.出入库申请单号 as 相关单号,明细号, 其他入库单号 as 单号,原因分类,b.申请类型 ,base.规格型号,base.物料编码,base.物料名称,业务单号,
 base.图纸编号,大类,小类,产品线, a.数量,a.生效日期 as 出库日期,b.备注,b.操作人员 as 申请人,b.部门名称 as 申请部门,d.部门,c.姓名 as 操作人,crmx.仓库号,crmx.仓库名称
 from 其他入库子表 a,基础数据物料信息表 base,其他出入库申请主表 b,人事基础员工表 c,人事基础员工表 d,仓库出入库明细表 crmx    where a.物料编码=base.物料编码
  and a.生效日期 >'{0}' and   a.生效日期 <'{1}' and c.员工号=a.生效人员编号 and d.员工号=b.操作人员编号 and crmx.明细号=a.其他入库明细号
  and a.出入库申请单号=b.出入库申请单号 ", Convert.ToDateTime(dateEdit1.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd"));

                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                    {
                        da.Fill(dtM);
                    }
                }


            }
            else
            {
                string sql = string.Format(@" select a.出入库申请单号 as 相关单号, a.其他出库明细号 as  明细号, 其他出库单号 as 单号,原因分类,b.申请类型,base.规格型号,base.物料编码 ,base.物料名称,业务单号,
 base.图纸编号,大类,小类,产品线,a.数量,a.生效日期 as 出库日期,b.备注,b.操作人员 as 申请人,b.部门名称 as 申请部门,d.部门,c.姓名 as 操作人,crmx.仓库号,crmx.仓库名称
 from 其他出库子表 a,基础数据物料信息表 base,其他出入库申请主表 b,人事基础员工表 c,人事基础员工表 d,仓库出入库明细表 crmx  where a.物料编码=base.物料编码
  and a.生效日期 >'{0}' and   a.生效日期 <'{1}' and  c.员工号=a.生效人员编号  and d.员工号=b.操作人员编号
 and a.出入库申请单号=b.出入库申请单号 and crmx.明细号=a.其他出库明细号 ", Convert.ToDateTime(dateEdit1.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
                dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

                sql = string.Format(@"select a.出入库申请单号 as 相关单号, a.其他入库明细号 as 明细号, 其他入库单号 as 单号,原因分类,b.申请类型 ,base.规格型号,base.物料编码,base.物料名称,业务单号,
 base.图纸编号,大类,小类,产品线, a.数量,a.生效日期 as 出库日期,b.备注,b.操作人员 as 申请人,b.部门名称 as 申请部门,d.部门,c.姓名 as 操作人,crmx.仓库号,crmx.仓库名称
 from 其他入库子表 a,基础数据物料信息表 base,其他出入库申请主表 b,人事基础员工表 c,人事基础员工表 d,仓库出入库明细表 crmx    where a.物料编码=base.物料编码
  and a.生效日期 >'{0}' and   a.生效日期 <'{1}' and c.员工号=a.生效人员编号 and d.员工号=b.操作人员编号 and crmx.明细号=a.其他入库明细号
  and a.出入库申请单号=b.出入库申请单号 ", Convert.ToDateTime(dateEdit1.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    da.Fill(dtM);
                }
            }

            gridControl1.DataSource = dtM;

        }
        private void fun_check()
        {
            if (dateEdit1.EditValue == null && dateEdit2.EditValue.ToString() == "")
            {
                throw new Exception("请选择时间");
            }
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

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
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
