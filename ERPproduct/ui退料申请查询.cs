using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class ui退料申请查询 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        public ui退料申请查询()
        {
            InitializeComponent();
            DateTime t = CPublic.Var.getDatetime().Date;
            barEditItem2.EditValue = t.AddDays(1).AddSeconds(-1);
            barEditItem1.EditValue = t.AddMonths(-1);
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_search()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format(@"select mx.*,wl.规格型号 from 工单退料申请表 mx    
left join  基础数据物料信息表 wl  on  mx.产品编号 = wl.物料编码
where  mx.操作时间 > '{0}' and mx.操作时间 < '{1}'", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));

            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;
            string s_mx = string.Format("select * from 工单退料申请明细表 where  1<>1");
            DataTable dtP = new DataTable();
            dtP = CZMaster.MasterSQL.Get_DataTable(s_mx, strcon);
            gridControl2.DataSource = dtP;

        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_mx(string str)
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format(@" select a.*,原ERP物料编号 as 物料编号,isnull(c.货架描述,'') as 货架号 ,b.规格型号  from 工单退料申请明细表 a 
     left join 基础数据物料信息表 b  on a.物料编码=b.物料编码 
     left join 仓库物料数量表 c on c.物料编码 =a.物料编码 and a.仓库号 =c.仓库号  where 待退料号='{0}' ", str);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl2.DataSource = dt;

        }
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_search();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (dr == null) return;
            fun_mx(dr["待退料号"].ToString());
        }

#pragma warning disable IDE1006 // 命名样式
        private void ui退料申请查询_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_search();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (dr == null)
                    throw new Exception("请先选择需要作废的记录");
                string sql_主 = string.Format("select  * from 工单退料申请表 where 待退料号='{0}'", dr["待退料号"]);
                DataTable dt_主 = CZMaster.MasterSQL.Get_DataTable(sql_主, strcon);


                if (dt_主.Rows[0]["完成"].Equals(true))
                    throw new Exception("该记录已完成，不需要作废");
                dt_主.Rows[0]["作废"] = true;
                dt_主.Rows[0]["作废时间"] = t;
                dt_主.Rows[0]["作废人"] = CPublic.Var.localUserName;

                string sql = string.Format("select * from 工单退料申请明细表 where 待退料号 = '{0}'", dr["待退料号"]);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt);
                foreach (DataRow r in dt.Rows)
                {
                    r["关闭"] = true;
                    r["关闭日期"] = t;

                }

                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction zuof = conn.BeginTransaction("作废工单退料");
                try
                {


                    SqlCommand cmm_1 = new SqlCommand("select * from 工单退料申请表 where 1<> 1", conn, zuof);
                    SqlCommand cmm_2 = new SqlCommand("select * from 工单退料申请明细表 where 1<> 1", conn, zuof);
                    SqlDataAdapter da_主表 = new SqlDataAdapter(cmm_1);
                    SqlDataAdapter da_明细表 = new SqlDataAdapter(cmm_2);
                    new SqlCommandBuilder(da_主表);
                    new SqlCommandBuilder(da_明细表);
                    da_主表.Update(dt_主);
                    da_明细表.Update(dt);


                    zuof.Commit();
                    MessageBox.Show("已作废:" + dr["待退料号"].ToString());

                }
                catch (Exception)
                {
                    zuof.Rollback();
                    throw new Exception("作废失败");
                }

                //da = new SqlDataAdapter(sql, strcon);
                //new SqlCommandBuilder(da);
                //da.Update(dt_主);

                //sql = "select * from 工单退料申请明细表 where 1<> 1";
                //da = new SqlDataAdapter(sql, strcon);
                //new SqlCommandBuilder(da);
                //da.Update(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            DataRow drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
            DataTable dtm = (DataTable)this.gridControl2.DataSource;

            Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
            Type outerForm = outerAsm.GetType("ERPreport.退料申请单打印", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统

            object[] drr = new object[2];

            drr[0] = drM;
            drr[1] = dtm;
            //   drr[2] = dr["出入库申请单号"].ToString();
            Form ui = Activator.CreateInstance(outerForm, drr) as Form;
            //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
            ui.ShowDialog();


        }

        private void gridControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
            }
        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try

            {

                DataRow drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;

                string sql = string.Format("select * from 工单退料申请表 where  待退料号='{0}' ", drM["待退料号"]);
                DataTable dt_tl = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                sql = string.Format("select * from 工单退料申请明细表 where  待退料号='{0}' ", drM["待退料号"]);
                DataTable dt_mx = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

                if (bool.Parse(dt_tl.Rows[0]["作废"].ToString()) == true)
                {
                    throw new Exception("当前单据已作废不可修改");
                }
                else if (bool.Parse(dt_tl.Rows[0]["完成"].ToString()) == true)
                {
                    throw new Exception("当前单据已完成不可修改");
                }
                退料修改 UI = new 退料修改(drM["待退料号"].ToString());
                CPublic.UIcontrol.AddNewPage(UI, "退料申请");






            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (dr == null) return;
                fun_mx(dr["待退料号"].ToString());
            }
            catch  
            {

          
            }
           
        }
    }
}
