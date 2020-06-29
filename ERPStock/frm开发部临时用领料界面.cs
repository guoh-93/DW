using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPStock
{
    public partial class frm开发部临时用领料界面 : UserControl
    {
        #region
        string strcon = CPublic.Var.strConn;
        DataTable dtM = new DataTable();
        DataTable dt_下拉 = new DataTable();
   
        #endregion

        #region 加载
        public frm开发部临时用领料界面()
        {
            InitializeComponent();

            

        }
        private void frm开发部临时用领料界面_Load(object sender, EventArgs e)
        {
            try
            {
                barEditItem1.EditValue = "";
                barEditItem2.EditValue =System.DateTime.Today.AddYears(-2);
                barEditItem3.EditValue = System.DateTime.Today.AddYears(-1);

                fun_load();
                fun_load_下拉();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
        #endregion 

        private void fun_load()
        {
            try
            {
                string sql ="";
                if ( barEditItem1.EditValue.ToString() != "")
                {
                   sql = string.Format("select * from 临时用领料单表 where 领料编号='{0}' ", barEditItem1.EditValue);
                 if (barEditItem2.EditValue.ToString() != "")
                {
                    sql = sql + string.Format("and  日期>='{0}' and  日期<'{1}'", barEditItem2.EditValue, barEditItem3.EditValue);
                }
                }
                else
                {
                    if (barEditItem2.EditValue.ToString() != "")
                    {
                        sql = string.Format("select * from 临时用领料单表 ");

                        sql = sql + string.Format("where  日期>='{0}' and  日期<'{1}'", barEditItem2.EditValue, barEditItem3.EditValue);

                    }
                }
               
                dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                 
                gridControl1.DataSource = dtM;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_load_下拉()
        {
            if (barEditItem2.EditValue.ToString() != "")
            {
                string sql_xl = string.Format(@"SELECT [领料编号],日期  FROM [FMS].[dbo].[临时用领料单表] where 日期>='{0}' and 日期 <'{1}' 
                               group by 领料编号,日期 order by 日期 ", barEditItem2.EditValue,barEditItem3.EditValue);
                dt_下拉 = CZMaster.MasterSQL.Get_DataTable(sql_xl, strcon);
                repositoryItemSearchLookUpEdit1.DataSource = dt_下拉;
                repositoryItemSearchLookUpEdit1.DisplayMember = "领料编号";
                repositoryItemSearchLookUpEdit1.ValueMember = "领料编号";
            }

           
        }
       
       //刷新
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            fun_load();                                
            
            fun_load_下拉();

        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
         //打印
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(barEditItem1.EditValue.ToString()=="")
            {
                 MessageBox.Show("请选择一个领料单编号");
            }
            else
            {
              ItemInspection.print_FMS.fun_print_财务领料单(barEditItem1.EditValue.ToString(), false);
            }

        }
        //批量打印
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DataRow dr in dt_下拉.Rows)
            {
                ItemInspection.print_FMS.fun_print_财务领料单(dr["领料编号"].ToString(), false);

            }
        }
    }
}
