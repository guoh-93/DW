using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using DevExpress.XtraPrinting;
using System.IO;

namespace ERPStock
{
    public partial class ui货架信息维护 : UserControl
    {
        public ui货架信息维护()    
        {
            InitializeComponent();
        }
        #region  变量
        string strconn = CPublic.Var.strConn;
        string PrinterName = "";
        string cfgfilepath = "";
        DataTable dt_货架;
    
        #endregion

        private void ui货架信息维护_Load(object sender, EventArgs e)
        {

            string sql_仓库 = "SELECT [属性值] as 仓库名称,属性字段1 as 仓库号 FROM [基础数据基础属性表] where 属性类别 ='仓库类别'";
          DataTable  dt_仓库 = new DataTable();
            dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_仓库, strconn);
            searchLookUpEdit2.Properties.DataSource = dt_仓库;
            searchLookUpEdit2.Properties.ValueMember = "仓库号";
            searchLookUpEdit2.Properties.DisplayMember = "仓库名称";


        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

            try
            {
                //if (searchLookUpEdit2.EditValue.ToString() == "")
                //{
                //    throw new Exception("请选择仓库！ ");


                //}
                
                    string sql = string.Format("select * from 仓库货架信息表 where 1=1  ");
                if (checkBox1.Checked == true)
                {
                    sql = sql + string.Format("and 仓库号 ='{0}'", searchLookUpEdit2.EditValue.ToString());
                }



                if (checkBox3.Checked == true)
                    {
                        sql = sql + string.Format("and 货架号 like'%{0}%'", textBox1.Text.ToString());
                    }




                    dt_货架 = new DataTable();
                    dt_货架 = CZMaster.MasterSQL.Get_DataTable(sql,strconn);
                    gridControl2.DataSource = dt_货架;





               


            }
            catch (Exception)
            {

                throw;
            }


        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                dt_货架 = new DataTable();
                string sql = "select * from 仓库货架信息表 where 1<>1 ";
                dt_货架 = CZMaster.MasterSQL.Get_DataTable(sql,strconn);
                gridControl2.DataSource = dt_货架;
                gridView2.OptionsBehavior.Editable = true;
                DataRow dr = dt_货架.NewRow();
                if (searchLookUpEdit2.Text.ToString()!="")
                {
                    dr["仓库号"] = searchLookUpEdit2.EditValue.ToString();
                    dr["仓库名称"] = searchLookUpEdit2.Text.ToString();
                }
                dt_货架.Rows.Add(dr);

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


            try
            {
                DataRow drM = (this.BindingContext[gridControl2.DataSource].Current as DataRowView).Row;




                drM.Delete();

            }
            catch (Exception)
            {

                throw;
            }
         
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                gridView2.CloseEditor();
                this.BindingContext[dt_货架].EndCurrentEdit();

                SqlDataAdapter da = new SqlDataAdapter();
                da = new SqlDataAdapter("select * from 仓库货架信息表 where 1<>1", strconn);
                new SqlCommandBuilder(da);
                da.Update(dt_货架);
                MessageBox.Show("保存成功");

            }
            catch (Exception ex)
            {

                throw new Exception();
            }




        }
        public void Dowork版本2()
        {
            DataTable dtx = dt_货架.Clone();
            for (int i = 0; i < gridView2.DataRowCount; i++)
            {
                dtx.ImportRow(gridView1.GetDataRow(i));
            }

            DataView dv = new DataView(dt_货架);
            dv.Sort = "货架号";
            DataTable dt_dy = dv.ToTable();
    


            List<Dictionary<string, string>> lI2 = new List<Dictionary<string, string>>();

            foreach (DataRow drr in dt_dy.Rows)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();


                //string sql = string.Format("select 规格型号,物料名称, 物料等级 from 基础数据物料信息表 where  物料编码='{0}'", drr["物料编码"]);
                //DataRow drj = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                //  dic.Add("wldj", drr["物料等级"].ToString());
                // dic.Add("itemid", drr["ItemId"].ToString());
                dic.Add("hjh", drr["货架号"].ToString());
                // dic.Add("wlmc", drr["物料名称"].ToString());
           //     dic.Add("wlbh", drr["物料编码"].ToString().Trim());
                // dic.Add("ggxh", drr["规格型号"].ToString().Trim());
                // dic.Add("ckmc", drr["仓库名称"].ToString().Trim());
                dic.Add("ckh", drr["仓库号"].ToString().Trim());
              
                lI2.Add(dic);

            }


            string path = Application.StartupPath + string.Format(@"\Mode\temp货架.lab");
            string path2 = Application.StartupPath + string.Format(@"\Mode\temp货架描述.lab");
 
            LabelPrint.LPrinter lp2 = new LabelPrint.LPrinter(path2, lI2, PrinterName, 1);

       
            lp2.DoWork();
        }
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // fun_check_dy();

                if (MessageBox.Show(string.Format("确定打印？"), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                    this.printDialog1.Document = this.printDocument1;
                    DialogResult dr = this.printDialog1.ShowDialog();

                    PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                    Dowork版本2();
                    //LabelPrint.LPrinter lp=new LabelPrint.LPrinter(path, dic, str_打印机, 1)
                    // }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
