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
    public partial class ui返工原因及部门 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        public ui返工原因及部门()
        {
            InitializeComponent();
        }


        #region 变量

        DataTable CheckFinished;
        DataRow drM;
        string strcon = CPublic.Var.strConn;


        #endregion



#pragma warning disable IDE1006 // 命名样式
        private void ui返工原因及部门_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DateTime t = CPublic.Var.getDatetime();
             
            barEditItem1.EditValue = DateTime.Parse(t.AddDays(-7).ToString());

            barEditItem2.EditValue = t.AddDays(1).AddSeconds(-1);





        }


#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
           try
            {
                gridView2.CloseEditor();
                this.BindingContext[CheckFinished].EndCurrentEdit();
                if (gridView1.RowCount ==0)
               {
                   throw new Exception("当前不可保存");
               }

                if(CheckFinished.Rows.Count<=0){

                    throw new Exception("当前不可保存");
                }
                if (gridView2.RowCount ==0)
               {
                   throw new Exception("当前不可保存");
               }
                foreach (DataRow dr in CheckFinished.Rows)
                {
                    if (dr["关系部门"].ToString() == "")
                    {
                        throw new Exception("请填选择关系部门后再保存！");
                    }

                }
              

           


                using (SqlDataAdapter da = new SqlDataAdapter("select * from 成品检验检验记录返工表 where 1<>1", strcon))
                {

                     new SqlCommandBuilder(da);
                    da.Update(CheckFinished);
                    MessageBox.Show("保存成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }//保存

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            string sql = string.Format("select * from 生产记录生产检验单主表 where 检验日期 >'{0}'and 检验日期<'{1}' and 返工数量>0 ",barEditItem1.EditValue.ToString(),barEditItem2.EditValue.ToString());
            DataTable dt_CheckMenu = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dt_CheckMenu;



        }//刷新

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;

            string sql = string.Format("select * from 成品检验检验记录返工表 where 生产检验单号='{0}' ",drM["生产检验单号"].ToString());
           CheckFinished = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl2.DataSource = CheckFinished;




        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }


    }
}
