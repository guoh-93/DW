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
    public partial class frm基础数据辅助工时录入界面 : UserControl
    {
        #region
        string strconn = CPublic.Var.strConn;
        DataTable dt_人员;
        DataTable dt_工时 = null;
        DataTable dt;
        //string str_生产人员 = "";
        //string str_生产人员ID1 = "";
        string str_生产车间 = "";//部门
        string str_课室编号 = "";//课室编号
        #endregion

        #region
        public frm基础数据辅助工时录入界面()
        {
            InitializeComponent();
        }

        private void frm基础数据辅助工时录入界面_Load(object sender, EventArgs e)
        {
            try
            {
                frn_载入人员();
                fun_工时();
                fun_借入车间();

                string sql = string.Format("select 课室,课室编号 from 人事基础员工表 where 员工号 = '{0}'", CPublic.Var.LocalUserID);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                str_生产车间 = dt.Rows[0]["课室"].ToString();
                str_课室编号 = dt.Rows[0]["课室编号"].ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_人员_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                //DataRow dr = gv_人员.GetDataRow(gv_人员.FocusedRowHandle);
                //str_生产人员 = dr["姓名"].ToString();
                //str_生产人员ID1 = dr["员工号"].ToString();
                //str_生产车间 = dr["部门"].ToString();
                //fun_工时(dr["员工号"].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region
        private void frn_载入人员()
        {
            string str_工号 = CPublic.Var.LocalUserID;
            string sll = string.Format(@"select * from 人事基础员工表 where 员工号 = '{0}'", str_工号);
            SqlDataAdapter daa = new SqlDataAdapter(sll, strconn);
            DataTable tt = new DataTable();
            daa.Fill(tt);
            string sql = string.Format(@"select * from 人事基础员工表 where 在职状态 = '在职' and 课室 = '{0}'", tt.Rows[0]["课室"].ToString());
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            dt_人员 = new DataTable();
            da.Fill(dt_人员);
            dt_人员.Columns.Add("选择", typeof(Boolean));
            gc_人员.DataSource = dt_人员;
        }

        private void fun_工时()
        {
            //string sql = string.Format("select * from 基础数据辅助工时表 where 生产人员ID = '{0}' and 日期 >= '{1}'", str_生产人员ID, System.DateTime.Today.AddDays(-90));
            string sql = string.Format("select * from 基础数据辅助工时表 where 1<>1");
            dt_工时 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_工时);
            gc_工时录入.DataSource = dt_工时;
            //dt_工时.Columns.Add("课室编号");
        }

        private void gv_工时录入_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.Caption == "输入工时(小时)")
                {
                    //折算方法
                    DataRow dr = gv_工时录入.GetDataRow(gv_工时录入.FocusedRowHandle);
                    dr["折算工时"] = Convert.ToDecimal(dr["输入工时"]) / 8;
                }
                if (e.Column.Caption == "借入车间")
                {
                    DataRow dr = gv_工时录入.GetDataRow(gv_工时录入.FocusedRowHandle);
                  DataRow[] dr2 = dt.Select(string.Format("属性值='{0}'",dr["生产车间"]));
                  dr["课室编号"] = dr2[0]["课室编号"];
                }
            }
            catch { }
        }

        private void fun_借入车间()
        {
            string sql = "select 属性值,属性字段1 课室编号 from 基础数据基础属性表 where 属性类别 = '课室' and 属性字段1 <> ''";
            dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt);

            repositoryItemSearchLookUpEdit2.DataSource = dt;
            repositoryItemSearchLookUpEdit2.DisplayMember = "属性值";
            repositoryItemSearchLookUpEdit2.ValueMember = "属性值";
        }
        #endregion

        #region
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //save
            try
            {
                if (MessageBox.Show("确认保存？", "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    gv_工时录入.CloseEditor();
                    gc_工时录入.BindingContext[dt_工时].EndCurrentEdit();

                    foreach (DataRow r in dt_工时.Rows)
                    {
                        if (r.RowState == DataRowState.Deleted)
                        {
                            continue;
                        }
                        if (Convert.ToDecimal(r["输入工时"]) <= 0)
                        {
                            throw new Exception("输入工时不可小于等于0");
                        }
                        //r["生产人员ID"] = str_生产人员ID1;
                        r["日期"] = System.DateTime.Now;
                    }
                    string sql = "select * from 基础数据辅助工时表 where 1<>1";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    new SqlCommandBuilder(da);
                    da.Update(dt_工时);
                    MessageBox.Show("保存成功");
                    barLargeButtonItem3_ItemClick(null, null);
                }
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

        /*20170217不用了*/
        private void button1_Click(object sender, EventArgs e)
        {
            if (dt_工时 != null)
            {
                DataRow dr = dt_工时.NewRow();
                dt_工时.Rows.Add(dr);
                dr["GUID"] = System.Guid.NewGuid();
                dr["操作人ID"] = CPublic.Var.LocalUserID;
                dr["操作人"] = CPublic.Var.localUserName;
                dr["工作日期"] = CPublic.Var.getDatetime();

                //dr["生产人员"] = str_生产人员;
                dr["生产车间"] = str_生产车间;
                gv_工时录入.FocusedRowHandle = gv_工时录入.DataRowCount - 1;
            }
        }
        /*20170217不用了*/
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dt_工时 != null)
                {
                    DataRow dr = gv_工时录入.GetDataRow(gv_工时录入.FocusedRowHandle);
                    if (MessageBox.Show("是否要删除该行？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        dr.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        } 
        #endregion

        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gv_人员.CloseEditor();
                gc_人员.BindingContext[dt_人员].EndCurrentEdit();
                DataRow drr = gv_人员.GetDataRow(gv_人员.FocusedRowHandle);
                if (drr["选择"].ToString().ToLower() == "true")
                {
                    DataRow dr = dt_工时.NewRow();
                    dt_工时.Rows.Add(dr);
                    dr["GUID"] = System.Guid.NewGuid();
                    //dr["生产车间"] = drr["部门"].ToString();
                    dr["生产人员ID"] = drr["员工号"].ToString();
                    dr["生产人员"] = drr["姓名"].ToString();
                    //dr["日期"] = drr["物料名称"].ToString();
                    //dr["折算工时"] = drr["规格型号"].ToString();
                    dr["操作人ID"] = CPublic.Var.LocalUserID;
                    dr["操作人"] = CPublic.Var.localUserName;
                    //dr["输入工时"] = drr["销售订单明细号"].ToString();
                    //dr["生产备注"] = drr["n原ERP规格型号"].ToString();
                    dr["工作日期"] = CPublic.Var.getDatetime();
                    dr["生产车间"] = str_生产车间;
                    dr["课室编号"] = str_课室编号;
                                      
                }
                else
                {
                    DataRow[] ds = dt_工时.Select(string.Format("生产人员ID = '{0}'", drr["员工号"].ToString()));
                    if (ds.Length > 0)
                    {
                        ds[0].Delete();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                frn_载入人员();
                fun_工时();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_人员_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gv_工时录入_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
    }
}
