using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace BaseData
{
    public partial class frm基础数据包装清单界面 : UserControl
    {
        #region
        string strconn = CPublic.Var.strConn;
        public  DataTable dt_成品;
        DataTable dt_包装 = null;
        DataTable dt_包装下;
        DataTable dt_原料;
        string str_成品编码1 = "";
        string str_成品名称 = "";
        string str_物料="";
        #endregion

        #region
        public frm基础数据包装清单界面()
        {
            InitializeComponent();
        }
        public frm基础数据包装清单界面(string str_物料)
        {
            InitializeComponent();
            this.str_物料 = str_物料;
        }
        private void frm基础数据包装清单界面_Load(object sender, EventArgs e)
        {
            fun_成品();
            fun_原料();
            if (str_物料 != "")
            {
                gv_成品.FocusedRowHandle = gv_成品.LocateByDisplayText(0, gridColumn1, str_物料);
                gv_成品.SelectRow(gv_成品.FocusedRowHandle);
                gv_成品_RowCellClick(null, null);
            }
        }

        private void gv_成品_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow r = gv_成品.GetDataRow(gv_成品.FocusedRowHandle);
                str_成品编码1 = r["物料编码"].ToString();
                str_成品名称 = r["物料名称"].ToString();
                fun_包装(r["物料编码"].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region
        private  void fun_成品()
        {

            //string sql = "select * from 基础数据物料信息表 where 物料编码 in (select 成品编码 from 基础数据包装清单表_backup1107 group by 成品编码)";
            string sql = "select * from 基础数据物料信息表 where 物料编码 in (select 成品编码 from 基础数据包装清单表_backup1107 group by 成品编码) or 物料类型='成品' or 物料类型='半成品' ";

            dt_成品 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_成品);
            gc_成品.DataSource = dt_成品;
        }
        private void fun_save_日志()
        {
            DataRow r = gv_成品.GetDataRow(gv_成品.FocusedRowHandle);
            string sql = "select * from [基础数据包装清单修改日志表] where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

               DataRow dr = dt.NewRow();

               dr["修改人ID"] = CPublic.Var.LocalUserID;
               dr["修改人"] = CPublic.Var.localUserName;
               dr["修改时间"] = System.DateTime.Now;
               dr["产品编码"] = r["物料编码"];
               dr["产品型号"] = r["n原ERP规格型号"];
               dr["产品名称"] = r["物料名称"];

                dt.Rows.Add(dr);
                new SqlCommandBuilder(da);
                da.Update(dt);
            }
 
            
        }
        private void fun_包装(string str_成品编码)
        {
            string sql = string.Format("select * from 基础数据包装清单表 where 成品编码 = '{0}'", str_成品编码);
            dt_包装 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_包装);
            gc_原料.DataSource = dt_包装;
            dt_包装.ColumnChanged += dt_包装_ColumnChanged;

            string sql_1 = string.Format("select * from 基础数据包装清单表_backup1107 where 成品编码 = '{0}'", str_成品编码);
            dt_包装下 = new DataTable();
            SqlDataAdapter da_1 = new SqlDataAdapter(sql_1, strconn);
            da_1.Fill(dt_包装下);
           gridControl1.DataSource = dt_包装下;
        }

        void dt_包装_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            //try
            //{
            //    if (e.Column.ColumnName == "物料编码")
            //    {
            //        DataRow[] ds = dt_原料.Select(string.Format("物料编码 = '{0}'",e.Row["物料编码"].ToString()));
            //        if (ds.Length > 0)
            //        {
            //            e.Row["物料名称"] = ds[0]["物料名称"].ToString();
            //            e.Row["大类"] = ds[0]["大类"].ToString();
            //            e.Row["小类"] = ds[0]["小类"].ToString();
            //            e.Row["规格型号"] = ds[0]["n原ERP规格型号"].ToString();
            //            e.Row["图纸编号"] = ds[0]["图纸编号"].ToString();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void fun_原料()
        {
            //string sql = "select 物料编码,物料名称,规格型号,大类,小类,n原ERP规格型号,图纸编号 from 基础数据物料信息表 where 物料类型 = '原材料' and 停用 = 0";
            //dt_原料 = new DataTable();
            //SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            //da.Fill(dt_原料);
            //repositoryItemSearchLookUpEdit1.DataSource = dt_原料;
            //repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
            //repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
        }
        #endregion

        #region
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ////save
            //try
            //{
            //    gv_原料.CloseEditor();
            //    gc_原料.BindingContext[dt_包装].EndCurrentEdit();

            //    foreach (DataRow r in dt_包装.Rows)
            //    {
            //        if (r.RowState == DataRowState.Deleted)
            //        {
            //            continue;
            //        }
            //        r["成品编码"] = str_成品编码1;
            //        r["成品名称"] = str_成品名称;
            //    }
            //    string sql = "select * from 基础数据包装清单表 where 1<>1";
            //    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            //    new SqlCommandBuilder(da);
            //    da.Update(dt_包装);
            //    MessageBox.Show("保存成功");
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (dt_包装 != null)
            //{
            //    DataRow dr = dt_包装.NewRow();
            //    dt_包装.Rows.Add(dr);
            //    dr["GUID"] = System.Guid.NewGuid();
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //if (dt_包装 != null)
            //{
            //    DataRow dr = gv_原料.GetDataRow(gv_原料.FocusedRowHandle);
            //    if (MessageBox.Show("是否要删除该行？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //    {
            //        dr.Delete();
            //    }
            //}
        }
        #endregion

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("确认替换吗？", "提醒", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                string sql="select * from [基础数据包装清单表] where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    foreach (DataRow dr in dt_包装.Rows)
                    {
                        dr.Delete();
                    }
                    foreach (DataRow dr in dt_包装下.Rows)
                    {
                        dr.SetAdded();
                    }
                    new SqlCommandBuilder(da);
                    da.Update(dt_包装);
                    da.Update(dt_包装下);
                    fun_save_日志();
                    MessageBox.Show("ok");
                    try
                    {
                        gv_成品_RowCellClick(null, null);

                    }
                    catch (Exception)
                    {
                        
                       
                    }

                }
            }
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e == null)
            {

            }
            else if (e.RowHandle > -1)
            {

                DataRow dr = gridView1.GetDataRow(e.RowHandle);

                if (dt_包装.Select(string.Format("物料编码='{0}'", dr["物料编码"])).Length > 0)
                {
                    e.Appearance.BackColor = Color.White;

                }
                else
                {
                    e.Appearance.BackColor = Color.Yellow;
                }

                return;

            }
        }

        private void gv_原料_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show(string.Format("确定该产品的包装清单？"), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                DataRow dr = gv_成品.GetDataRow(gv_成品.FocusedRowHandle);
                string sql = string.Format("select * from [需确认包装清单表] where 物料编码='{0}'", dr["物料编码"].ToString());
                DataTable dt = new DataTable();
                dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (dt.Rows.Count > 0)  //确认记录
                {
                    string sql_1 = string.Format(@"update 需确认包装清单表 set 修改完成=1,确认人='{0}',确认人ID='{1}',确认日期='{2}'
                                        where 物料编码='{3}'", CPublic.Var.localUserName, CPublic.Var.LocalUserID, CPublic.Var.getDatetime().ToString("yyyy-MM-dd HH:mm:ss"), dr["物料编码"].ToString());
                    CZMaster.MasterSQL.ExecuteSQL(sql_1, strconn);
                }
                else  //添加该物料记录 并 赋修改完成
                {
                    string sql_2 = string.Format(@"insert into 需确认包装清单表 (物料编码,修改完成,确认人,确认人ID,确认日期) values('{0}',1,'{1}','{2}','{3}')"
                             , dr["物料编码"].ToString(), CPublic.Var.localUserName, CPublic.Var.LocalUserID,CPublic.Var.getDatetime().ToString("yyyy-MM-dd HH:mm:ss"));
                    CZMaster.MasterSQL.ExecuteSQL(sql_2, strconn);
                }
            }
        }

       
    }
}
