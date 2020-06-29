using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace approval
{
    public partial class ui审核查询 : UserControl
    {
        public ui审核查询()
        {
            InitializeComponent();
        }
        string strcon = CPublic.Var.strConn;
        DataTable dt_show;
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try

            {
                DateTime t1 = Convert.ToDateTime(barEditItem1.EditValue).Date;
                DateTime t2 = Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1);
                string sql = string.Format("select * from  单据审核申请表  where  申请时间>'{0}' and 申请时间<'{1}'  and 审核=0 ", t1, t2);
                dt_show = new DataTable();
                dt_show = CZMaster.MasterSQL.Get_DataTable(sql,strcon);

                gc.DataSource = dt_show;



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
