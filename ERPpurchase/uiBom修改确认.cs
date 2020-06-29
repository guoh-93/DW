using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
namespace ERPpurchase
{
    public partial class uiBom修改确认 : UserControl
    {
        DataTable dtM = new DataTable();
        string strcon = CPublic.Var.strConn;
        DataTable dt_ls;
        public uiBom修改确认()
        {
            InitializeComponent();
        }

        private void uiBom修改确认_Load(object sender, EventArgs e)
        {
            try
            {
                string sql_gys = string.Format("select * from [采购人员关联供应商表] where  员工号='{0}'", CPublic.Var.LocalUserID);
                dt_ls = CZMaster.MasterSQL.Get_DataTable(sql_gys, strcon);
                fun_search();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
           
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                fun_save(dr);
                fun_search();
                MessageBox.Show("ok");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
    

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        private void fun_save(DataRow dr )
        {
            dr["采购确认"] = true;

            string sql = "select  * from 基础数据BOM信息修改记录表 where  1<>1";
            //using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            //{
            //    new SqlCommandBuilder(da);
            //    da.Update(dtM);
            //}
            DataTable dt_库存下限 = new DataTable();
            string ss = string.Format("select  *  from 基础数据物料信息表 where 物料编码='{0}'", dr["更改前子项编码"].ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(ss, strcon))
            {
                da.Fill(dt_库存下限);
                DataRow[] r = dt_库存下限.Select(string.Format("物料编码='{0}'", dr["更改前子项编码"]));
                if(r.Length>0)
                {
                  r[0]["库存下限"]  =Convert.ToDecimal(dr["更改前安全库存"]) ;
                }
                
            }
            if (dr["更改后子项编码"].ToString() != dr["更改前子项编码"].ToString())
            {
                string sx = string.Format("select  *  from 基础数据物料信息表 where 物料编码='{0}'", dr["更改后子项编码"].ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(sx, strcon))
                {
                    da.Fill(dt_库存下限);
                    DataRow[] r = dt_库存下限.Select(string.Format("物料编码='{0}'", dr["更改后子项编码"]));
                    if (r.Length > 0)
                    {
                        r[0]["库存下限"] = Convert.ToDecimal(dr["更改后安全库存"]);
                    }

                }
            }
            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("BOM采购确认");

            try
            {
               
                SqlCommand cmd1 = new SqlCommand(sql, conn, ts);
                using (SqlDataAdapter da1 = new SqlDataAdapter(cmd1))
                {
                    new SqlCommandBuilder(da1);
                    da1.Update(dtM);
                }

                string sql2 = "select * from 基础数据物料信息表 where 1<>1";
                SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);
                using (SqlDataAdapter da2 = new SqlDataAdapter(cmd2))
                {
                    new SqlCommandBuilder(da2);
                    da2.Update(dt_库存下限);
                }

         
                ts.Commit();
             
            }
            catch (Exception ex)
            {
                ts.Rollback();
              
            }

        }
        private void fun_search()
        {
            string sql = string.Format(@"select bomxgjl.*,a.物料编码 as 成品编号,a.规格型号 as 成品规格,b.物料编码 as 更改前子项编码,b.物料名称 as 更改前物料名称
              ,b.图纸编号 as 更改前子项图号,b.库存下限 as 更改前安全库存,c.物料编码 as 更改后子项编码,c.物料名称 as 更改后物料名称,c.图纸编号 as 更改后子项图号,c.库存下限 as 更改后安全库存,c.默认供应商 as 修改后物料默认供应商
                from 基础数据BOM信息修改记录表 bomxgjl
               left  join 基础数据物料信息表 as a on bomxgjl.成品编码=a.物料编码
               left  join  基础数据物料信息表 as b  on bomxgjl.更改前物料=b.物料编码
               left  join  基础数据物料信息表 as c on bomxgjl.更改后物料=c.物料编码 
               where bomxgjl.采购确认=0");
               if (dt_ls.Rows.Count > 0)
                {
                        sql = sql + " and ( b.供应商编号= '' or";
                        foreach (DataRow dr in dt_ls.Rows)
                        {
                            sql = sql + string.Format(" b.供应商编号='{0}' or", dr["供应商ID"]);
                        }
                        sql = sql.Substring(0, sql.Length - 2);
                        sql = sql + ")";
                 }
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void 查看详细信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string column_name = gridView1.FocusedColumn.Caption;
            if (column_name == "更改前子项编码" || column_name == "更改后子项编码" || column_name == "成品编号")
            {
                string sql=string.Format("select  * from 基础数据物料信息表  where 物料编码='{0}'",dr[column_name]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql,strcon))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, @"ERPStock.dll"));
                    Type outerForm = outerAsm.GetType("ERPStock.frm仓库物料数量明细", false);
                    //  Form ui = Activator.CreateInstance(outerForm) as Form;
                    object[] dic = new object[1];
                    dic[0] = dr["物料编码"].ToString();


                    UserControl ui = Activator.CreateInstance(outerForm, dic) as UserControl; // 过往出口明细 构造函数 有两个参数,string ,datetime 
                    CPublic.UIcontrol.Showpage(ui, "仓库物料数量明细");


                }
            }



        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Column.Caption == "更改前子项编码" || e.Column.Caption == "更改后子项编码" || e.Column.Caption == "成品编号")
            {
                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                    gridView1.CloseEditor();
                    this.BindingContext[dtM].EndCurrentEdit();

                }


            }
        }
    }
}
