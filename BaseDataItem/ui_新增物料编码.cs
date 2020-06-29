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
    public partial class ui_新增物料编码 : UserControl
    {
        string strcon = CPublic.Var.strConn;

        public ui_新增物料编码()
        {
            InitializeComponent();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void ui_新增物料编码_Load(object sender, EventArgs e)
        {
            string qxz = CPublic.Var.LocalUserTeam;
            string s = $"select  * from [新增编码_权限组区域配置表] where 权限组='{qxz}'";
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s,strcon);
            foreach(DataRow dr  in  t.Rows)
            {
                Controls[dr["区域"].ToString()].Visible = true;
            }

        }
    }
}
