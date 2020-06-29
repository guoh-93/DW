using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BaseData
{
    public partial class uibom申请修改 : UserControl
    {
        public uibom申请修改()
        {
            InitializeComponent();
        }

        string s_修改_单号;
        DataTable dt_修改子;
        string strcon = CPublic.Var.strConn;
        public uibom申请修改(string adc)
        {
            InitializeComponent();
            s_修改_单号 = adc;
        }
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void uibom申请修改_Load(object sender, EventArgs e)
        {

            try

            {
                //DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string sql = string.Format("select * from 基础数据BOM修改明细表 where BOM修改单号 = '{0}'", s_修改_单号);
                dt_修改子 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                gridControl2.DataSource = dt_修改子;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

         
        }
    }
}
