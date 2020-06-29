using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPSale
{
    public partial class frm片区年度指标维护 : UserControl
    {

        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        int i = 0;
        public frm片区年度指标维护()
        {
            InitializeComponent();
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_base();
                fun_load();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
                        
          
        }

        private void frm片区年度指标维护_Load(object sender, EventArgs e)
        {
            try
            {
                barEditItem1.EditValue = CPublic.Var.getDatetime().Year;
                fun_base();
                fun_load();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
                
        }

        private void fun_base()
        {
            string s = string.Format(@"select  属性值 as 片区 from 基础数据基础属性表  where 属性类别='片区'
             union  select 片区 from 销售片区年度指标对应表  where 年份={0} ", barEditItem1.EditValue.ToString().Trim());
            DataTable dt =new DataTable  ();
            dt=CZMaster.MasterSQL.Get_DataTable(s,strcon);
            repositoryItemSearchLookUpEdit1.DataSource = dt;
            repositoryItemSearchLookUpEdit1.DisplayMember = "片区";
            repositoryItemSearchLookUpEdit1.ValueMember = "片区";
             int year = CPublic.Var.getDatetime().Year;
             s = "select  min(年份)年份 from 销售片区年度指标对应表";
             DataTable temp = CZMaster.MasterSQL.Get_DataTable(s,strcon);
            int start = year;
            if(temp.Rows[0]["年份"].ToString().Trim()!="")
            {
                year = Convert.ToInt32(temp.Rows[0]["年份"]);
            }
            for (; start <= year + 1; start++)
            {
                repositoryItemComboBox1.Items.Add(start);
            }

        }

        private void fun_load()
        {

            if (barEditItem1.EditValue != null && barEditItem1.EditValue.ToString().Trim() != "")
            {
                string sql = string.Format("select  * from 销售片区年度指标对应表 where 年份={0} ", barEditItem1.EditValue.ToString());
                dtM = new DataTable();
                dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                gridControl1.DataSource = dtM;
                gridView1.ViewCaption = barEditItem1.EditValue.ToString().Trim()+"年度指标";
            }
            else
            {

                throw new Exception("年份未选择");
            }

        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr  = dtM.NewRow();
            if (dtM.Rows.Count == 0)
            {
                dr["编号"] = "001";
            }
            else
            {
              DataRow []r= dtM.Select("编号=max(编号)");
              dr["编号"] = (Convert.ToInt32(r[0]["编号"]) + 1).ToString("000");

            }
            dr["年份"] = barEditItem1.EditValue;
            dtM.Rows.Add(dr);
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);


            if (dr != null)
            {
                dr.Delete();
            }
            else
            {

                MessageBox.Show("无数据");
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridView1.CloseEditor();
            this.BindingContext[dtM].EndCurrentEdit();

            CZMaster.MasterSQL.Save_DataTable(dtM, "销售片区年度指标对应表", strcon);
            MessageBox.Show("ok");
            fun_load();
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
    }
}
