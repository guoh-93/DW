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
    public partial class UI虚拟库存明细 : UserControl
    {
        #region  变量
        string str_物料;
        string strconn = CPublic.Var.strConn;
        string sql="";
        string str_生产车间;
        DataTable dtM; // 上面的表

        #endregion


        #region 加载

        
        public UI虚拟库存明细()
        {
            InitializeComponent();
           
        }
        public UI虚拟库存明细(string s)
        {
            this.str_物料 = s;
            InitializeComponent();
            barManager1.MainMenu.Visible = false;
            
        }

        private void UI车间虚拟库存_Load(object sender, EventArgs e)
        {
            try
            {
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
              
            }
        }


        #endregion

        #region 函数

        private void fun_load()
        {
            DataTable dt = new DataTable();
            dt = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
            if (dt.Rows.Count > 0)
            {
                str_生产车间 = dt.Rows[0]["生产车间"].ToString();
            }
            //须有最高权限  暂定
            //确定 sql
            if (str_生产车间 == "") //拥有最高权限
            {
                be_下拉框.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                bs_生产车间.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                if (str_物料 != null)  //传值了 数量界面
                {
                    sql = string.Format("select * from 生产记录车间虚拟库存表 where 物料编码='{0}'", str_物料);
                }
                else
                {
                    sql = "select * from 生产记录车间虚拟库存表";
                }
                //加载下拉框
                string sql_xl = "select 部门编号,部门名称 from 人事基础部门表 where 部门名称 like '%制造%' and 部门名称 like '%课%'";
                using (SqlDataAdapter da = new SqlDataAdapter(sql_xl, strconn))
                {
                    DataTable dt_xl = new DataTable();
                    da.Fill(dt_xl);
                    repositoryItemSearchLookUpEdit1.DataSource = dt_xl;
                    repositoryItemSearchLookUpEdit1.DisplayMember = "部门名称";
                    repositoryItemSearchLookUpEdit1.ValueMember = "部门编号";

                }
            }
            else  // 对应一个 生产线   无最高权限 生产车间筛选框不可见
            {
                
                be_下拉框.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bs_生产车间.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                if (str_物料 !=null)
                {
                    sql = string.Format("select * from 生产记录车间虚拟库存表 where 生产车间='{0}' and 物料编码='{1}' ", str_生产车间, str_物料);
                }
                else
                {
                    sql = string.Format("select * from 生产记录车间虚拟库存表 where 生产车间='{0}'", str_生产车间);
                }
            }

            using (SqlDataAdapter da =new SqlDataAdapter (sql,strconn))
            {
                dtM = new DataTable();
                da.Fill(dtM);
                gridControl1.DataSource = dtM;
            }

           

        }

        //点击上面的显示出下面的明细表
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string sql_明细=string .Format 
                ("select * from 生产记录车间虚拟库存明细表 where 物料编码='{0}' and 生产车间='{1}' ",
                r["物料编码"].ToString (),r["生产车间"].ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(sql_明细, strconn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                gridControl2.DataSource = dt;

            }
   
        }
     
        ////跳转至工单
        //private void 查看工单明细ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            

        //}
        //生产线下拉筛选
        private void be_下拉框_EditValueChanged(object sender, EventArgs e)
        {
            DataView dv = new DataView(dtM);
            gridControl1.DataSource = dv;
            dv.RowFilter = string.Format("生产车间='{0}'", be_下拉框.EditValue);
        }
        #endregion
      

        #region 界面操作
        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                UI车间虚拟库存_Load(null, null);
                gridControl2.DataSource = null;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
        }
        //关闭
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        #endregion
    

       

    }
}
