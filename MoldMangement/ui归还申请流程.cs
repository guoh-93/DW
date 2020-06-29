using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MoldMangement
{
    public partial class ui归还申请流程 : UserControl
    {
        public ui归还申请流程()
        {
            InitializeComponent();
        }
        DataRow dr_主;
        DataTable dt_m;
        string str_归还单号;
        DataTable dt_申请主;
        DataTable dt_仓库号;
        DataTable dt_申请子;
       

        public ui归还申请流程(DataRow dr, DataTable dt)
        {
            InitializeComponent();
            dr_主 = dr;
            dt_m = dt;
        }
        #region

        string strconn = CPublic.Var.strConn;
        string s_单号="";
        //DataTable dt_mx;
        #endregion


        public ui归还申请流程(string  s_归还单号)
        {
            InitializeComponent();
            s_单号 = s_归还单号;
        }

        private void ui归还申请流程_Load(object sender, EventArgs e)
        {
            try
            {
                if (s_单号!="")
                {
                    string sql = string.Format("select * from 归还申请主表 where 归还批号='{0}'",s_单号);
                    dt_申请主 = new DataTable();
                    dt_申请主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    dataBindHelper1.DataFormDR(dt_申请主.Rows[0]);
                    textBox11.Text = dt_申请主.Rows[0]["归还说明"].ToString();
                    sql = string.Format(@"  select  a.*,b.已借出数量-b.归还数量-正在申请数+需归还数量-已归还数量 as 当前最大申请数   from 归还申请子表 a
                   left join 借还申请表附表 b  on a.申请批号明细=b.申请批号明细 where 归还批号='{0}'  and 申请已归还数量='0' ", s_单号);
                    dt_申请子 = new DataTable();
                    dt_申请子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    textBox10.Text = s_单号;
                   // gridColumn11.OptionsColumn.AllowEdit = false;
                    gridColumn1.Visible = false;
                }
                else
                {
                    string sql = string.Format("select * from 归还申请主表 where 1<>1");
                    dt_申请主 = new DataTable();
                    dt_申请主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    sql = string.Format("select * from 归还申请子表 where 1<>1");
                    dt_申请子 = new DataTable();
                    dt_申请子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_申请子.Columns.Contains("当前最大申请数") != true)
                    {
                        dt_申请子.Columns.Add("当前最大申请数", typeof(decimal));
                    }
                    if (dt_申请子.Columns.Contains("正在申请数") != true)
                    {
                        dt_申请子.Columns.Add("正在申请数", typeof(decimal));

                    }
                    if (dr_主 != null)
                    {
                        dataBindHelper1.DataFormDR(dr_主);
                    }
                    if (dt_m.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt_m.Rows)
                        {
                            //bool.Parse(dr["领取完成"].ToString()) == false ||
                            if (dr.RowState == DataRowState.Deleted ||  bool.Parse(dr["归还完成"].ToString()) == true || bool.Parse(dr["作废"].ToString()) == true)
                            {
                                continue;
                            }

                            decimal dec= decimal.Parse(dr["已借出数量"].ToString()) - decimal.Parse(dr["归还数量"].ToString()) - decimal.Parse(dr["正在申请数"].ToString());
                            if (dec > 0)
                            {
                                DataRow drP = dt_申请子.NewRow();
                                dt_申请子.Rows.Add(drP);
                                drP["申请批号"] = dr["申请批号"];
                                drP["申请批号明细"] = dr["申请批号明细"];
                                drP["物料编码"] = dr["物料编码"];
                                drP["物料名称"] = dr["物料名称"];
                                drP["规格型号"] = dr["规格型号"];
                                drP["货架描述"] = dr["货架描述"];
                                drP["正在申请数"] = dr["正在申请数"];
                                //drP["仓库号"] = dr["仓库号"];
                                //drP["仓库名称"] = dr["仓库名称"];
                                //20-5-7 默认检验1

                                drP["仓库号"] = "96";
                                drP["仓库名称"] ="检验1";
                                drP["需归还数量"] = dec;
                               
                                drP["当前最大申请数"] = drP["需归还数量"];
                                drP["已归还数量"] = dr["归还数量"];
                                drP["计量单位"] = dr["计量单位"];
                                drP["计量单位编码"] = dr["计量单位编码"];
                                drP["借用数量"] = dr["申请数量"];
                            }
                            //drP[""] = dr[""];
                        }

                        if (dt_申请子.Columns.Contains("选择") != true)
                        {
                            DataColumn dc = new DataColumn("选择", typeof(bool));
                            dc.DefaultValue = false;
                            dt_申请子.Columns.Add(dc);


                        }

                    }
                }

                Fun_下拉框选择项();

                gridControl2.DataSource = dt_申请子;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        private void Fun_下拉框选择项()
        {
            dt_仓库号 = new DataTable();
            string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别'";
            SqlDataAdapter da = new SqlDataAdapter(sql4, strconn);
            da.Fill(dt_仓库号);
            repositoryItemGridLookUpEdit1.DataSource = dt_仓库号;
            repositoryItemGridLookUpEdit1.DisplayMember = "仓库号";
            repositoryItemGridLookUpEdit1.ValueMember = "仓库号";
        }
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try

            {
                //string sql = string.Format("select * from 借还申请表  where  作废='0' and 申请日期>'{0}' and 申请日期<'{1}' and 归还='0' ", barEditItem1.EditValue, barEditItem2.EditValue);
                //DataTable dt_main = CZMaster.MasterSQL.Get_DataTable(sql,CPublic.Var.strConn);
                //gridControl1.DataSource = dt_main;
 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {


            try
            {
                //DataRow   drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
                //   string sql = string.Format("select * from 借还申请表附表 where 申请批号='{0}'",drM["申请批号"]);
                //   dt_mx = new DataTable();
                //   dt_mx = CZMaster.MasterSQL.Get_DataTable(sql,strconn);

                //   if (dt_mx.Columns.Contains("选择")!=true)
                //   {
                //       dt_mx.Columns.Add("选择",typeof(bool));
                //   }
                //   gridControl2.DataSource = dt_mx;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (s_单号!="")///修改
            {
                fun_change();
            }
            else
            {
                fun_save();///正常保存
            }



            //  ui归还申请流程_Load(null,null);
        }
        private void fun_change()
        {
            try

            {
                gridView2.CloseEditor();
                this.BindingContext[dt_申请子].EndCurrentEdit();
                this.BindingContext[dt_申请主].EndCurrentEdit();

                foreach (DataRow drr in dt_申请子.Rows)
                {
                    if (decimal.Parse(drr["需归还数量"].ToString()) <= 0)
                    {
                        throw new Exception("请输入合适的数");
                    }
                    if ((decimal.Parse(drr["需归还数量"].ToString())) > decimal.Parse(drr["当前最大申请数"].ToString()))
                    {
                        throw new Exception("归还数总和超出借用数量");
                    }

                }


                foreach (DataRow   dr in dt_申请主.Rows)
                {
                    if (textBox11.Text!="" )
                    {
                        dr["归还说明"] = textBox11.Text;
                    }


                }

                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("生效");
                string sql1 = "select * from 归还申请主表 where 1<>1";
                SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
                SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);


                sql1 = "select * from 归还申请子表 where 1<>1";
                SqlCommand cmd2 = new SqlCommand(sql1, conn, ts);
                SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                new SqlCommandBuilder(da2);


        

                try
                {
                    da1.Update(dt_申请主);
                    da2.Update(dt_申请子);
              
                    ts.Commit();
                    MessageBox.Show("申请成功");
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }
    

        private void fun_save()
        {
            try

            {
                gridView2.CloseEditor();
                this.BindingContext[dt_申请子].EndCurrentEdit();

                DataView dv = new DataView(dt_申请子);
                dv.RowFilter = "选择=1";
                DataTable tt = dv.ToTable();
                if(tt.Rows.Count<=0) {
                    throw new Exception("当前未选择归还明细");
                }
                if (textBox10.Text == "")
                {

                    DateTime t = CPublic.Var.getDatetime();

                    //DateTime t   = Convert.ToDateTime("2019-07-14 10:10:40.207");

                    str_归还单号 = "";
                    str_归还单号 = string.Format("GH{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                    CPublic.CNo.fun_得到最大流水号("GH", t.Year, t.Month));
                    s_单号 =textBox10.Text = str_归还单号.ToString();
                    DataRow dr_申请主 = dt_申请主.NewRow();
                    dt_申请主.Rows.Add(dr_申请主);
                    dr_申请主["归还批号"] = str_归还单号;

                    dr_申请主["借用类型"] = textBox3.Text;
                    dr_申请主["原因分类"] = textBox2.Text;
                  //  dr_申请主["原因分类"] = textBox2.Text;
                    dr_申请主["申请批号"] = textBox1.Text;
                    dr_申请主["归还操作人"] = CPublic.Var.localUserName;
                    dr_申请主["备注"] = textBox6.Text;
                    dr_申请主["归还申请日期"] = t;
                    dr_申请主["归还说明"] = textBox11.Text;
                    dr_申请主["归还方式"] = "借用归还";

                }
                ///  + decimal.Parse(drr["已归还数量"].ToString()) + decimal.Parse(drr["正在申请数"].ToString()))
                int i = 1;
                foreach (DataRow drr in tt.Rows)
                {
                   if(decimal.Parse(drr["需归还数量"].ToString())<=0)
                    {
                        throw new Exception("请输入合适的数");
                    }
                    if ((decimal.Parse(drr["需归还数量"].ToString())) > decimal.Parse(drr["当前最大申请数"].ToString()))
                    {
                        throw new Exception("归还数总和超出借用数量");
                    }
                    DataRow[] dr_借出 = dt_m.Select(string.Format("   申请批号明细='{0}'and 申请批号='{1}'  ", drr["申请批号明细"], drr["申请批号"]));
                    dr_借出[0]["正在申请数"] = decimal.Parse(drr["需归还数量"].ToString()) + decimal.Parse(dr_借出[0]["正在申请数"].ToString());
                    // drr[""]
                    drr["POS"] = i;
                    drr["归还批号"] = str_归还单号;
                    drr["归还明细号"] = str_归还单号 + "-" + Convert.ToInt32(drr["POS"]).ToString("00");
                    i++;
                }

                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("生效");
                string sql1 = "select * from 归还申请主表 where 1<>1";
                SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
                SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);


                sql1 = "select * from 归还申请子表 where 1<>1";
                SqlCommand cmd2 = new SqlCommand(sql1, conn, ts);
                SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                new SqlCommandBuilder(da2);


                sql1 = "select * from 借还申请表附表 where 1<>1";
                SqlCommand cmd3 = new SqlCommand(sql1, conn, ts);
                SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
                new SqlCommandBuilder(da3);

                try
                {
                    da1.Update(dt_申请主);
                    da2.Update(tt);
                    da3.Update(dt_m);
                    ts.Commit();
                    MessageBox.Show("申请成功");
                    ui归还申请流程_Load(null, null);
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            


        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();

        }

        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {

            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
          
        }

        private void gridView2_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                if (e.Column.FieldName == "仓库号")
                {
                    dr["仓库号"] = e.Value;
                    DataRow[] ds = dt_仓库号.Select(string.Format("仓库号 = {0}", dr["仓库号"]));
                    dr["仓库名称"] = ds[0]["仓库名称"];
                    string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_物料数量 = new DataTable();
                    da.Fill(dt_物料数量);
                    if (dt_物料数量.Rows[0]["货架描述"].ToString() != "")
                    {
                        dr["货架描述"] = dt_物料数量.Rows[0]["货架描述"];
                    }
                    else
                    {
                        dr["货架描述"] = "";

                    }

                    if (dt_物料数量.Rows.Count == 0)
                    {
                        dr["库存总数"] = 0;
                    }
                    else
                    {
                        dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
                    }
                }
            }
            catch { }
        }
    }
}
