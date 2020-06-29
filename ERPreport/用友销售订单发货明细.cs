using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPreport
{
    public partial class 用友销售订单发货明细 : Form
    {
        DataRow drr;
        string strCOON = CPublic.Var.geConn("DW");
        public 用友销售订单发货明细()
        {
            InitializeComponent();
        }
        public 用友销售订单发货明细(DataRow dr)
        {
            InitializeComponent();
            drr = dr;
        }

        private void 用友销售订单发货明细_Load(object sender, EventArgs e)
        {
            string str = string.Format(@"select  rdrecords32.iordercode as 销售订单号,ccode as 出库单号, rdrecords32.irowno 出库行号,rdrecords32.iorderseq as 订单行号,a.cInvCode 物料编码,a.cInvName 物料名称
           ,a.cInvStd 规格型号, rdrecords32.iQuantity as 出库数量,dDate 最近出库日期,cc.iQuantity as 销售数量,cc.dPreDate as 预计到货日期 FROM   rdrecords32
           left join rdrecord32 on rdrecord32.ID=rdrecords32.ID 
           left join inventory a on a.cInvCode = rdrecords32.cInvCode
           left join SO_SODetails cc on cc.cSOCode =rdrecords32.iordercode and cc.iRowNo=rdrecords32.iorderseq
           where iordercode='{0}' and iorderseq='{1}'", drr["销售订单号"].ToString(),drr["行号"].ToString());
            using(SqlDataAdapter da = new SqlDataAdapter(str,strCOON))
            {
                DataTable dtM = new DataTable();
                da.Fill(dtM);
                gridControl1.DataSource = dtM;
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }



    }
}
