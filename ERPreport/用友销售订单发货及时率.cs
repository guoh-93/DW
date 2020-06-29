using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraPrinting;
using System.IO;

namespace ERPreport
{
    public partial class 用友销售订单发货及时率 : UserControl
    {
        public 用友销售订单发货及时率()
        {
            InitializeComponent();
        }
        string strcOOn = CPublic.Var.geConn("DW");
        string cfgfilepath;
        DataTable dtM;
        private void 用友销售订单发货及时率_Load(object sender, EventArgs e)
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


            fun_加载客户();// dateEdit2.EditValue]
            DateTime t = CPublic.Var.getDatetime().Date;
            dateEdit2.EditValue = t;

          t =  t.AddMonths(-3);
           t = new DateTime(t.Year,t.Month,1);
           dateEdit1.EditValue = t;
        }
        private void fun_加载客户()
        {
            try
            {
                SqlDataAdapter da;
                string sql = string.Format(@"select Customer.cCusCode 客户编码,Customer.cCusName 客户名称 from Customer ");

                da = new SqlDataAdapter(sql,strcOOn);
                DataTable dt_客户 = new DataTable();
                da.Fill(dt_客户);
                searchLookUpEdit2.Properties.DataSource = dt_客户;
                searchLookUpEdit2.Properties.DisplayMember = "客户名称";
                searchLookUpEdit2.Properties.ValueMember = "客户编码";
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_加载客户()");
                throw new Exception(ex.Message);
            }
        }

        private void fun_load()
        {
            DateTime t = CPublic.Var.getDatetime().AddMonths(-3);
            try
            {
                string str = "";
                string str1 = string.Format(@"select  *,(substring(CONVERT(nvarchar,round(按期出库数量/销售数量*100,2))+'%',0,6)+'%')及时率 from (
       select SO_SODetails.cSOCode 销售订单号,iRowNo 行号,Customer.cCusCode 客户编码,Customer.cCusName 客户名称,SO_SODetails.cInvCode 物料编码,SO_SODetails.cInvName 物料名称,inventory.cInvStd 规格型号,iQuantity 销售数量,dPreDate 预计发货日期
       ,isnull(累计出库数量,0)累计出库数量 ,最近出库日期 ,isnull(按期出库数量,0)按期出库数量,isnull(有退货,CONVERT(bit,0))有退货,dcreatesystime 订单创建日期 ,SO_SODetails.cSCloser  from   SO_SODetails 
     left join inventory  on inventory.cInvCode=SO_SODetails.cInvCode 
     left join SO_SOMain on SO_SOMain.cSOCode=SO_SODetails.cSOCode
    left join  Customer on Customer.cCusCode=SO_SOMain.cCusCode
    left join (select  rdrecords32.iordercode,rdrecords32.iorderseq,SUM(rdrecords32.iQuantity) 累计出库数量, MAX(dDate) 最近出库日期 FROM   rdrecords32 
		  left join rdrecord32 on rdrecord32.ID=rdrecords32.ID  group by iordercode,iorderseq)a  
    on a.iordercode=SO_SODetails.cSOCode and a.iorderseq=iRowNo
     left join (select  rdrecords32.iordercode,rdrecords32.iorderseq,SUM(rdrecords32.iQuantity) 按期出库数量  FROM   rdrecords32 
		  left join  SO_SODetails on iordercode=SO_SODetails.csocode and SO_SODetails.iRowNo=iorderseq
		  left join rdrecord32 on rdrecord32.ID=rdrecords32.ID 
	      where DATEADD(HOUR,23,dPreDate)>=rdrecord32.dDate group by iordercode,iorderseq) b
     on b.iordercode=SO_SODetails.cSOCode and b.iorderseq=iRowNo
     left  join (select  rdrecords32.iordercode,rdrecords32.iorderseq,CONVERT(bit,1)有退货 FROM   rdrecords32 
			left join rdrecord32 on rdrecord32.ID=rdrecords32.ID  where iQuantity<0
			group by iordercode,iorderseq)c 
      on  c.iordercode=SO_SODetails.cSOCode and c.iorderseq=iRowNo
        where SO_SOMain.iStatus=1 ) result  where (累计出库数量>0 or 有退货=1 or  cSCloser is null or cSCloser='asuser')");

                if (checkBox1.Checked == false && checkBox2.Checked == false && checkBox3.Checked == false)
                {
                    str = str1;
                }
                //单独日期
                if (checkBox3.Checked == true)
                {
                    if (dateEdit1.Text.ToString() =="" || dateEdit2.Text.ToString()=="")
                    {
                        throw new Exception("请选择日期！");
                    }
                    str = str1 + string.Format(" and 订单创建日期 >='{0}' and 订单创建日期 <='{1}'", dateEdit1.EditValue, Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1));
                }
                //单独客户名称
                if (checkBox1.Checked == true)
                {
                    if (searchLookUpEdit2.Text.ToString() == "")
                    {
                        throw new Exception("请选择客户名称！");
                    }
                    str = str1 + string.Format(" and 客户编码 ='{0}'", searchLookUpEdit2.EditValue);
                }
                //单独销售订单
                if (checkBox2.Checked == true)
                {
                    if (textBox1.Text.ToString() == "")
                    {
                        throw new Exception("请正确填写销售订单号！");
                    }
                    str = str1 + string.Format("and 销售订单号 like %'{0}'%",textBox1.Text.ToString());
                }
                ////日期和客户名称
                //if (checkBox1.Checked == true && checkBox2.Checked == false && checkBox3.Checked == true)
                //{
                //    if (dateEdit1.Text.ToString() == "" || dateEdit2.Text.ToString() == "")
                //    {
                //        throw new Exception("请选择日期！");
                //    }
                //    if (searchLookUpEdit2.Text.ToString() == "")
                //    {
                //        throw new Exception("请选择客户！");
                //    }
                //    str = str1 + string.Format("where 订单创建日期 >='{0}' and 订单创建日期 <='{1}' and  客户编码 ='{2}'", dateEdit1.EditValue, dateEdit2.EditValue, searchLookUpEdit2.EditValue);
                //}
                ////日期和销售订单
                //if (checkBox1.Checked == false && checkBox2.Checked == true && checkBox3.Checked == true)
                //{
                //    if (dateEdit1.Text.ToString() == "" || dateEdit2.Text.ToString() == "")
                //    {
                //        throw new Exception("请选择日期！");
                //    }
                //    if (textBox1.Text.ToString() == "")
                //    {
                //        throw new Exception("请正确填写销售订单号！");
                //    }
                //    str = str1 + string.Format("where 订单创建日期 >='{0}' and 订单创建日期 <='{1}' and  销售订单号 like '{2}'",dateEdit1.EditValue,dateEdit2.EditValue,searchLookUpEdit2.EditValue,textBox1.Text.ToString());
                //}
                ////客户名称和销售订单
                //if (checkBox1.Checked == true && checkBox2.Checked == true && checkBox3.Checked == false)
                //{
                //    if (searchLookUpEdit2.EditValue.ToString() == "")
                //    {
                //        throw new Exception("请选择客户名称！");
                //    }
                //    if (textBox1.Text.ToString() == "")
                //    {
                //        throw new Exception("请正确填写销售订单号！");
                //    }
                //    str = str1 + string.Format("where 客户名称 ='{0}' and  销售订单号 like '{1}'",searchLookUpEdit2.EditValue, textBox1.Text.ToString());
                //}
                using(SqlDataAdapter da = new SqlDataAdapter(str,strcOOn))
                {
                     dtM = new DataTable();
                    da.Fill(dtM);
                    gridControl1.DataSource = dtM;
                }

        



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fun_load();



        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Clicks == 2)
                {
                    DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                    用友销售订单发货明细 fm = new 用友销售订单发货明细(dr);
                    fm.ShowDialog();
                }
            }
            catch(Exception ex)
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
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }
        //导出
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
            // DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions(TextExportMode.Text, false, false);
            /// gridControl1.ExportToXlsx(saveFileDialog.FileName, options);
            //    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ERPorg.Corg.TableToExcel(dtM,saveFileDialog.FileName);
                MessageBox.Show("导出成功");
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
