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
    public partial class ui委外领料出库未核销报表 : UserControl
    {

        string strcon = CPublic.Var.strConn;
        string ss = " ";
        string cfgfilepath = "";
        public ui委外领料出库未核销报表()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_check();
                //呵呵
                fun_search();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
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

            string sql2 = "select * from 采购供应商表 where 供应商状态='在用'";
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);
            txt_gysbh.Properties.DataSource = dt;
            txt_gysbh.Properties.DisplayMember = "供应商名称";
            txt_gysbh.Properties.ValueMember = "供应商ID";
        }
        private void fun_search()
        {

            /*
             *select  a.采购单号,a.明细完成,采购明细号,a.税率,e.原ERP物料编号,单价 as 父项采购单价,e.计量单位 as 父项单位,采购数量,ISNULL(入库总量,0) 入库总量,d.委外已核量,出库总数,e.物料名称 as 父项名称,e.n原ERP规格型号 as 父项型号,e.大类 as 父项大类
 ,e.小类 as 父项小类,a.供应商ID ,a.供应商,h.原ERP物料编号 as 子项编号,h.物料名称 as 子项名称,h.计量单位 as 子项单位,h.大类 as 子项大类,h.小类 as 子项小类
 ,h.n原ERP规格型号 as 子项型号,明细完成日期  from 采购记录采购单明细表  a
 inner join 采购记录采购单主表 b on  a.采购单号=b.采购单号
 left  join  (select  采购单明细号,sum(入库量)入库总量  from 采购记录采购单入库明细  group by 采购单明细号) t on t.采购单明细号=a.采购明细号 
 left join (  select a.备注,原ERP物料编号 ,SUM(a.数量)出库总数,SUM(委外已核量) 委外已核量 from 其他出库子表 a
 inner join 其他出入库申请主表 b  on a.出入库申请单号=b.出入库申请单号 
  where b.原因分类='委外加工' group by a.备注,原ERP物料编号 )ck on ck.备注=a.采购明细号 
  
 inner join 其他出入库申请子表 c on c.备注=a.采购明细号 
 left join 其他出库子表 d on  d.出入库申请明细号 =c.出入库申请明细号
left join 基础数据物料信息表 e on e.物料编码=a.物料编码 
left join 基础数据物料信息表 h on h.原ERP物料编号 =d.原ERP物料编号  
 where b.采购单类型='委外采购' and ck.原ERP物料编号=d.原ERP物料编号  and d.委外已核量<d.数量  
             * 
             * */
            string s = string.Format(@"select  a.采购单号,a.明细完成,采购明细号,a.税率,e.物料编码,单价 as 父项采购单价,e.计量单位 as 父项单位,采购数量,ISNULL(入库总量,0) 入库总量,ck.委外已核量 ,出库总数,未核销数量,e.物料名称 as 父项名称,e.规格型号 as 父项型号,e.大类 as 父项大类
 ,e.小类 as 父项小类,a.供应商ID ,a.供应商,h.物料编码 as 子项编号,h.物料名称 as 子项名称,h.计量单位 as 子项单位,h.大类 as 子项大类,h.小类 as 子项小类
 ,h.规格型号 as 子项型号,明细完成日期,d.结算单价 as 核销单价  from 采购记录采购单明细表  a
 inner join 采购记录采购单主表 b on  a.采购单号=b.采购单号
 left  join  (select  采购单明细号,sum(入库量)入库总量  from 采购记录采购单入库明细  group by 采购单明细号) t on t.采购单明细号=a.采购明细号 
 left join (  select a.备注,物料编码 ,SUM(a.数量)出库总数,SUM(委外已核量) 委外已核量,SUM(a.数量)-SUM(委外已核量) as 未核销数量 from 其他出库子表 a
 inner join 其他出入库申请主表 b  on a.出入库申请单号=b.出入库申请单号 
  where b.原因分类='委外加工' group by a.备注,物料编码 )ck on ck.备注=a.采购明细号 
 inner join 其他出入库申请子表 c on c.备注=a.采购明细号 
 left join 其他出库子表 d on  d.出入库申请明细号 =c.出入库申请明细号
left join 基础数据物料信息表 e on e.物料编码=a.物料编码 
left join 基础数据物料信息表 h on h.物料编码 =d.物料编码 
 where b.采购单类型='委外采购' and ck.物料编码=d.物料编码  and d.委外已核量<d.数量 {0}", ss);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = @"select  采购单明细号 from 采购记录采购单入库明细 a
            inner join 采购记录采购单主表 b on  a.采购单号=b.采购单号
            where 采购单类型='委外采购' and 委外核销=0 group by 采购单明细号";
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //dt 中需要去除一部分 ，明细完成日期 is not null  并且 再 t 用 采购单明细号中找不到的数据 说明 该采购单有剩余不送检完成  
            int i = dt.Rows.Count-1;
            for (; i >= 0; i--)
            {
                if (dt.Rows[i]["明细完成日期"] != DBNull.Value)
                {
                    if (t.Select(string.Format("采购单明细号='{0}'", dt.Rows[i]["采购明细号"])).Length == 0)
                    {
                        dt.Rows.RemoveAt(i);
                    }
                }

            }


            gc_委外领料未核销.DataSource = dt;

        }
        private void fun_check()
        {
            if (checkBox1.Checked == true)
            {
                if (txt_gysbh.EditValue == null || txt_gysbh.EditValue.ToString() == "")
                {
                    throw new Exception("未选择供应商");

                }
                ss = string.Format(" and   b.供应商ID='{0}'", txt_gysbh.EditValue.ToString());
            }
            //if (checkBox6.Checked == true)
            //{
            //    if (dateEdit3.EditValue == null || dateEdit4.EditValue == null || dateEdit3.EditValue.ToString() == "" || dateEdit4.EditValue.ToString() == "")
            //    {
            //        throw new Exception("未选择日期");
            //    }
            //     ss=ss+string.Format(" and a.供应商ID='{0}'", txt_gysbh.EditValue.ToString())
            //}


        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void ui委外领料出库未核销报表_Load(object sender, EventArgs e)
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

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
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

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                gc_委外领料未核销.ExportToXlsx(saveFileDialog.FileName);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }



    }
}
