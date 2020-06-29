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
    public partial class 测试下拉 : Form
    {
        DataTable dt_包装方式;
        DataView dv;
        DataTable dtM = new DataTable();
        string strcon = CPublic.Var.strConn;
        public 测试下拉()
        {
            InitializeComponent();
        }

        private void 测试下拉_Load(object sender, EventArgs e)
        {
            string sql_bzfs = "select  物料编码 as 编号 ,物料名称 as 包装方式 ,特殊备注 as 用量,规格型号,自定义项1,自定义项2 from 基础数据物料信息表 where 存货分类='包材配件包' and 停用=0";
            dt_包装方式 = CZMaster.MasterSQL.Get_DataTable(sql_bzfs, strcon);
            dv = new DataView(dt_包装方式);
            repositoryItemSearchLookUpEdit1.DataSource = dv;
            repositoryItemSearchLookUpEdit1.ValueMember = "编号";
            repositoryItemSearchLookUpEdit1.DisplayMember = "编号";

 

            dtM.Columns.Add("x");
            dtM.Columns.Add("编号");
            dtM.Columns.Add("编号1");


            gridControl1.DataSource = dtM;




        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "x")
            {
                //if (e.Value.ToString() == "1")
                //{
                //    repositoryItemSearchLookUpEdit1View.ActiveFilterString = "30";
                //    repositoryItemSearchLookUpEdit1View.FindFilterText = "安装配件包";
                //    dv.RowFilter = null;
                //    dv.RowFilter = "自定义项1='安装配件包'";
                //}
                //else
                //{
                //    repositoryItemSearchLookUpEdit1View.FindFilterText = "";
                //    dv.RowFilter = null;
                //}
  
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            DataRow dr= dtM.NewRow();
            dtM.Rows.Add(dr);
        }

        private void repositoryItemSearchLookUpEdit1_Popup(object sender, EventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);

            if (dr != null)
            {
                if (dr["x"].ToString() == "1")
                {
           
                    repositoryItemSearchLookUpEdit1View.FindFilterText = "控制器工业包装-迪堡金融";

                    //repositoryItemGridLookUpEdit1View.ApplyFindFilter("控制器工业包装-迪堡金融");

                }

            }
        }

        private void repositoryItemGridLookUpEdit1_Popup(object sender, EventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);

            if(dr!=null)
            {
                if (dr["x"].ToString() == "1")
                {
            

                    //repositoryItemGridLookUpEdit1View.ApplyFindFilter("控制器工业包装-迪堡金融");

                }

            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            

        }

        private void gridView1_CustomRowCellEditForEditing(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == "编号")
            {
               
                if (e.RowHandle == gridView1.FocusedRowHandle)
                {
                    e.RepositoryItem = repositoryItemSearchLookUpEdit1;

                }
                else
                {
                    e.RepositoryItem = repositoryItemTextEdit1;
                }
            }
        }
    }
}
