using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace ERPproduct
{
    public partial class frm包装清单 : UserControl
    {
        public frm包装清单()
        {
            InitializeComponent();
        }
        DataTable dtM,dt_包装方式;
        DataRow[] dr_包装;
        string strcon = CPublic.Var.strConn;
#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string s = string.Format(@"select  Customer.cCusAbbName,cDLCode,iQuantity 数量,cInvCode 存货编码,cDefine22,cInvName 存货名称,isnull(cShipAddress,'')发货地址 from  [192.168.20.150].UFDATA_008_2018.dbo.DispatchLists
 left  join [192.168.20.150].UFDATA_008_2018.dbo.DispatchList on DispatchList.DLID=DispatchLists.DLID 
  inner join 基础数据物料信息表 base on base.物料编码=cInvCode
    left join [192.168.20.150].UFDATA_008_2018.dbo.Customer on Customer.ccuscode=DispatchList.cCusCode  where cDLCode like '%{0}' ", textBox1.Text);
                dtM = new DataTable();
                dtM = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                //textBox1.Text = dtM.Rows[0]["cCusAbbName"].ToString();
                //textBox4.Text = dtM.Rows[0]["发货地址"].ToString();
                dtM.Columns.Add("选择", typeof(bool));
                foreach (DataRow dr in dtM.Rows)
                {
                    dr["选择"] = true;
                }
                gridControl1.DataSource = dtM;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

       
          

        }

#pragma warning disable IDE1006 // 命名样式
        private void frm包装清单_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            string sql="select * from 包装方式表";
            dt_包装方式 = new DataTable();
            dt_包装方式 = CZMaster.MasterSQL.Get_DataTable(sql,strcon);



        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (gridControl2.DataSource != null)
                {
                    DataTable dtf = (DataTable)gridControl2.DataSource;
                    DataTable dt_1 = (DataTable)gridControl1.DataSource;
                    DataView dv = new DataView(dt_1);
                    dv.RowFilter = " 选择='true'";
                    DataTable dv_fu = dv.ToTable();
                    dv_fu.Columns.Remove("选择");
                    dv_fu.Columns["cCusAbbName"].ColumnName = "名称";
                    dv_fu.Columns["cDLCode"].ColumnName = "发货单号";
                    dv_fu.Columns["cDefine22"].ColumnName = "包装方式";





                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Title = "导出Excel";
                    saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                    DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                    if (dialogResult == DialogResult.OK)
                    {
                        DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                        options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;
                        //gc.ExportToXlsx(saveFileDialog.FileName, options);
                        ERPorg.Corg.PushDt(dv_fu, dtf, saveFileDialog.FileName);
                        DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    throw new Exception("别瞎点");
                }


             



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }     
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (dtM == null)
            {
                throw new Exception("无数据可操作");
            }
            gridView1.CloseEditor();
            this.BindingContext[dtM].EndCurrentEdit();
            DataTable dt_save = new DataTable();
            dt_save.Columns.Add("包装方式");
            dt_save.Columns.Add("物料名称");
            dt_save.Columns.Add("包装材料名称");
            dt_save.Columns.Add("规格型号");
            dt_save.Columns.Add("需求数量");
            dt_save.Columns.Add("物料编码");


            foreach (DataRow dr in dtM.Rows)
            {
                if (bool.Parse(dr["选择"].ToString()) == false)
                {
                    continue;
                }
                dr_包装 = null;
                string[] ss = dr["cDefine22"].ToString().Split(' ');
                dr_包装 = dt_包装方式.Select(string.Format("包装方式='{0}'", ss[0].ToString()));
                if (dr_包装 != null)
                {
                    foreach (DataRow dr_package in dr_包装)
                    {  //////计算
                        int math = 0;
                        decimal bom = Convert.ToDecimal(dr["数量"].ToString()) / Convert.ToDecimal(dr_package["单位数量"].ToString());
                        decimal sun = bom * Convert.ToDecimal(dr_package["包材数量"].ToString());
                        ////需要四舍五入的话在这加
                        //  sun=sun+0.5;
                        if ((int)sun != sun)
                        {
                            math = (int)sun + 1;
                        }
                        else
                        {
                            math = (int)sun;
                        }
                        DataRow drg = dt_save.NewRow();
                        drg["包装方式"] = dr["cDefine22"].ToString();
                        drg["物料名称"] = dr["存货名称"].ToString();
                        drg["包装材料名称"] = dr_package["包装材料名称"].ToString();
                        drg["规格型号"] = dr_package["规格型号"].ToString();
                        drg["物料编码"] = dr_package["物料编码"].ToString();
                        drg["需求数量"] = math;
                        dt_save.Rows.Add(drg);
                    }
                }

                //for (int i = 0; i <= dr_包装.Length; i++)
                //{
                //    DataRow ndr = dt_save.NewRow();
                //    ndr = dr_包装[i];
                //    dt_save.ImportRow(ndr);
                //}
            }
            //MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
            //DataTable dtfw = new DataTable();
            //dtfw = RBQ.SelectGroupByInto("", dt_save, @"物料名称,包装材料名称,规格型号,物料编码,sum(需求数量)  需求数量", "", "包装方式,物料名称,包装材料名称,规格型号,物料编码,需求数量");//,季度用
            //// gridControl1.DataSource = dt_save;
            ////foreach(DataRow dr_save in dt_save.Rows ){
            ////    if(){
            ////    }
            ////}
            DataView dv = new DataView(dt_save);
            DataTable dt_all = new DataTable();
            dt_all = dv.ToTable(true, "包装材料名称", "物料编码", "规格型号");
            dt_all.Columns.Add("需求数量");

            foreach (DataRow dr_all in dt_all.Rows)
            {
                int xuiqu = 0;
                foreach (DataRow dr in dt_save.Rows)
                {
                    if (dr_all["物料编码"].ToString() == dr["物料编码"].ToString())
                    {

                        xuiqu = xuiqu + int.Parse(dr["需求数量"].ToString());
                        dr_all["需求数量"] = xuiqu;
                    }
                }
            }



            gridControl2.DataSource = dt_all;

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView2_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
            }

        }


      



    }
}
