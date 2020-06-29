using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 郭恒的DEMO
{
    public partial class 多行复制测试 : Form
    {
        string strcon = CPublic.Var.strConn;
        DataTable dt_物料;
        DataTable dt_多选下拉;
        DataTable tM = new DataTable();
        public 多行复制测试()
        {
            InitializeComponent();
        }
         
        private void 多行复制测试_Load(object sender, EventArgs e)
        {
            string s = "select  物料编码,物料名称 from 基础数据物料信息表 where 可售=1";
            dt_物料 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

             s = $"select  物料编码,物料名称,规格型号 from 基础数据物料信息表 where  物料名称 like '箱柜%' and 物料名称 like '%配件包%' ";
            dt_多选下拉 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn dc = new DataColumn("选择",typeof(bool));
            dt_多选下拉.Columns.Add(dc);

            repositoryItemGridLookUpEdit1.DataSource = dt_物料;
            repositoryItemGridLookUpEdit1.DisplayMember = "物料编码";
            repositoryItemGridLookUpEdit1.DataSource = "物料编码"; 

            foreach (DataRow dr in dt_多选下拉.Rows)
            {
                this.repositoryItemCheckedComboBoxEdit2.Items.Add(dr["物料编码"].ToString(), dr["物料名称"].ToString()+" " +dr["规格型号"].ToString());
            }

            s = "select  top 20 * from 销售记录销售订单明细表   ";
             
          
          
            tM = CZMaster.MasterSQL.Get_DataTable(s,strcon);
            DataColumn dc1 = new DataColumn("安装配件包");
            tM.Columns.Add(dc1);
            gridControl1.DataSource = tM;

        }

        private void button1_Click(object sender, EventArgs e)
        {
                DevExpress.XtraGrid.Views.Base.GridCell []gcell = gridView1.GetSelectedCells();
            IDataObject iData = Clipboard.GetDataObject();
            string sx = (String)iData.GetData(DataFormats.Text);
            string s= "";
            int index = gcell[0].RowHandle;

            for(int x=0;x<gcell.Length;x++)
            {
              s+= gridView1.GetRowCellValue(gcell[x].RowHandle, gcell[x].Column);
                if(x+1>= gcell.Length)
                { }
                else if(gcell[x+1].RowHandle> gcell[x].RowHandle)  s += "\r\n";
                else
                {
                    s += "\t";
                }
            }
            //foreach (DevExpress.XtraGrid.Views.Base.GridCell j in gcell)
            //{
            //    s += gridView1.GetRowCellValue(j.RowHandle, j.Column);
            //    if (index < j.RowHandle)
            //        s += "\r\n";
            //    else
            //        s += "\t";
                

            //}
            Clipboard.SetDataObject(s);

        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if(e.Column.FieldName.Contains("数量"))
            {


            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (DataRow r in tM.Rows )
            {

            }
        }
    }
}