using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;

using System.Windows.Forms;
using System.Data.SqlClient;

namespace MoldMangement
{
    public partial class UI点检 : UserControl
    {
        //static string strConn = "Password = a; Persist Security Info = True; User ID = sa; Initial Catalog = fms; Data Source=XINREN";
        string strConn = CPublic.Var.strConn;
        DataTable dt = new DataTable();
        DataRow dr;
        DataTable dt_部门 = new DataTable();
        /// <summary>
        /// true代表新增状态
        /// </summary>
        //bool bl = false;
        //DateTime dateaaa;                         
        DateTime t = CPublic.Var.getDatetime ().AddMonths(1);
        //string t1;
        //string aa;
        //string ttt;


        public UI点检()
        {
            InitializeComponent();
        }

        private void fun_load()
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd");

            //t = t.AddMonths(1);

            string sql = "select * from 计量器具基础信息表 where 有效期 is not null  order by 有效期 asc";
            //string sql = string.Format("select * from 计量器具点检周期信息表 where 有效期>= '{0}' union select * from 计量器具点检周期信息表 where 有效期< '{0}'  order by 有效期 asc ", time);
            //string sql = "select * from 计量器具点检周期信息表";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
            {
                dt = new DataTable();
                da.Fill(dt);

            }
            gc1.DataSource = dt;

            //fun_xl();
            //bl = false;

            //string datea;
            //DateTime dateaa;
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{

            //    datea = dt.Rows[i]["有效期"].ToString();//有效期
            //    dateaa = Convert.ToDateTime(dt.Rows[i]["有效期"].ToString());
            //    //string dateaaa = dateaa.ToString("yyyy-MM-dd");


            //}
            dt.Columns.Add("点检确认", typeof(bool));
            dt.Columns.Add("履历情况");
            dt.Columns.Add("点检备注");

        }

        private void UI点检_Load(object sender, EventArgs e)
        {
            fun_load();
           

        }

        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
        }
        //点检确认
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //check();
               

                    gv1.CloseEditor();//关闭编辑状态
                    this.BindingContext[dt].EndCurrentEdit();//关闭编辑状态
                    DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
                    DateTime date_年月 = Convert.ToDateTime(Convert.ToDateTime(dr["有效期"]).ToString("yyyy-MM"));
                    DateTime t_年月 = Convert.ToDateTime(t.ToString("yyyy-MM"));
                    if (DateTime.Compare(t_年月, date_年月) >= 0)//根据年月判断
                    {
                        int date_年 = Convert.ToInt32(Convert.ToDateTime(dr["有效期"].ToString()).Year);
                        int t_年 = Convert.ToInt32(t.Year.ToString());
                        if (date_年 <= t_年)//根据年再筛选
                        {
                            int date_月 = Convert.ToInt32(Convert.ToDateTime(dr["有效期"].ToString()).Year);
                            int t_月 = Convert.ToInt32(t.Year.ToString());
                            if (date_月 <= t_月)//根据月再得出提前一个月的
                            {
                                if (MessageBox.Show("确认生效点检确认吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                                {
                                    fun_保存记录();
                                DataRow[] drr = dt.Select(string.Format("点检确认= 'true'"));

                                foreach (DataRow r in drr)
                                {
                                    int a;
                                    a = Convert.ToInt32(r["检定周期"].ToString());
                                    //r["有效期"] = t.AddMonths(a);
                                    DateTime aaa = Convert.ToDateTime(r["有效期"].ToString());
                                    r["有效期"] = aaa.AddMonths(a);

                                }
                               
                                fun_保存周期();
                                }
                            }
                            
                        }
                    }
                    else
                    { 
                        MessageBox.Show("请选择有背景颜色的数据确认"); 
                    }

               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        //关闭
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }


        private void fun_保存周期()
        {
            try
            {

            string sql = "select * from 计量器具基础信息表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
            {
                new SqlCommandBuilder(da);
                da.Update(dt);
            }
            MessageBox.Show("保存点检周期表成功");
            barLargeButtonItem1_ItemClick(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

        }

        private void fun_保存记录()
        {
             try
            {
                string t_new = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
            DataTable dt_虚拟 = new DataTable();
            //dr = gv1 .GetDataRow (gv1 .FocusedRowHandle );
            String sql = "select * from 计量器具明细卡表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
            {
                da.Fill(dt_虚拟);

                DataRow[] drr = dt.Select(string.Format("点检确认= 'true'"));
                foreach (DataRow rr in drr)
                {
                    DataRow dr_历史 = dt_虚拟.NewRow();
                    DateTime t = CPublic.Var.getDatetime();
                    dr_历史["点检单号"] = string.Format("DJ{0}{1:00}{2:00}{3:0000}", t.Year, t.Month,
                            t.Day, CPublic.CNo.fun_得到最大流水号("DJ", t.Year, t.Month));
                    dr_历史["计量器具编号"] = rr["计量器具编号"].ToString();
                    dr_历史["计量器具名称"] = rr["计量器具名称"].ToString();
                    dr_历史["计量器具规格"] = rr["计量器具规格"].ToString();
                    dr_历史["出厂编号"] = rr["出厂编号"].ToString();
                    dr_历史["制造单位"] = rr["制造单位"].ToString();
                    dr_历史["测量范围"] = rr["测量范围"].ToString();
                    //dr_历史["分度值"] = rr["分度值"].ToString();
                    //dr_历史["准确度"] = rr["准确度"].ToString();
                    dr_历史["出厂日期"] = rr["出厂日期"];
                    dr_历史["购置日期"] = rr["购置日期"];
                    dr_历史["领用日期"] = rr["领用日期"];
                    dr_历史["检定周期"] = rr["检定周期"].ToString();
                    dr_历史["检定日期"] = rr["有效期"].ToString(); ;
                    dr_历史["检定人"] = rr["检定单位"].ToString();
                    dr_历史["检定结果"] = rr["检定结果"].ToString();
                    dr_历史["检定单位"] = rr["检定单位"].ToString();
                    dr_历史["使用人或地点"] = rr["使用人"].ToString();
                    dr_历史["履历情况"] = rr["履历情况"].ToString();
                    dr_历史["点检备注"] = rr["点检备注"].ToString();
                 
                   
                    dt_虚拟.Rows.Add(dr_历史);
                }

                new SqlCommandBuilder(da);
                da.Update(dt_虚拟);


            }
            }
             catch (Exception ex)
             {
                 MessageBox.Show(ex.Message);

             }

        }

        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            dr = gv1.GetDataRow(gv1.FocusedRowHandle);
            dataBindHelper1.DataFormDR(dr);
        }

       

       

        private void UI点检_Load_1(object sender, EventArgs e)
        {
            fun_load();
        }
        //变色
        private void gv1_CustomDrawCell_1(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            DataRow x = gv1.GetDataRow(e.RowHandle);
            //ttt = x["计量器具名称"].ToString();      
            if (x != null)
            {
                DateTime date_年月 = Convert.ToDateTime(Convert.ToDateTime(x["有效期"]).ToString("yyyy-MM"));//数据的有效期年月时间
                DateTime t_年月 = Convert.ToDateTime(t.ToString("yyyy-MM"));//当前时间加一个月的年月时间

                if (DateTime.Compare(t_年月, date_年月) >= 0)//根据年月判断
                {
                    int date_年 = Convert.ToInt32(Convert.ToDateTime(x["有效期"].ToString()).Year);
                    int t_年 = Convert.ToInt32(t.Year.ToString());
                    if (date_年 <= t_年)//根据年再筛选
                    {
                        int date_月 = Convert.ToInt32(Convert.ToDateTime(x["有效期"].ToString()).Month);
                        int t_月 = Convert.ToInt32(t.Month.ToString());
                        if (date_月 < t_月)//根据月再得出提前一个月的
                        {

                            e.Appearance.BackColor = Color.LightCoral;

                        } if (date_月 == t_月)
                        {
                            e.Appearance.BackColor = Color.Turquoise;
                        }
                    }
                }

            }
        }

        //排序
        private void gv1_CustomDrawRowIndicator_1(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        //button点击事件
        private void button1_Click(object sender, EventArgs e)
        {

           
            foreach (DataRow r in dt.Rows)
            {
                DateTime date_年月 = Convert.ToDateTime(Convert.ToDateTime(r["有效期"]).ToString("yyyy-MM"));//数据的有效期年月时间
                DateTime t_年月 = Convert.ToDateTime(t.ToString("yyyy-MM"));//当前时间加一个月的年月时间

                if (DateTime.Compare(t_年月, date_年月) >= 0)//根据年月判断
                {
                    int date_年 = Convert.ToInt32(Convert.ToDateTime(r["有效期"].ToString()).Year);
                    int t_年 = Convert.ToInt32(t.Year.ToString());
                    if (date_年 <= t_年)//根据年再筛选
                    {
                        int date_月 = Convert.ToInt32(Convert.ToDateTime(r["有效期"].ToString()).Month);
                        int t_月 = Convert.ToInt32(t.Month.ToString());
                        if (date_月 < t_月)//根据月再得出提前一个月的
                        {

                            r["点检确认"] = true;
                        }
                        //else if(date_月 == t_月)
                        //{
                        //    r["点检确认"] = true;
                        //}
                        
                    }
                }
            }
            
                                           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (DataRow r in dt.Rows)
            {
                DateTime date_年月 = Convert.ToDateTime(Convert.ToDateTime(r["有效期"]).ToString("yyyy-MM"));//数据的有效期年月时间
                DateTime t_年月 = Convert.ToDateTime(t.ToString("yyyy-MM"));//当前时间加一个月的年月时间

                if (DateTime.Compare(t_年月, date_年月) >= 0)//根据年月判断
                {
                    int date_年 = Convert.ToInt32(Convert.ToDateTime(r["有效期"].ToString()).Year);
                    int t_年 = Convert.ToInt32(t.Year.ToString());
                    if (date_年 <= t_年)//根据年再筛选
                    {
                        int date_月 = Convert.ToInt32(Convert.ToDateTime(r["有效期"].ToString()).Month);
                        int t_月 = Convert.ToInt32(t.Month .ToString());
                        if (date_月 < t_月)//根据月再得出提前一个月的
                        {

                            r["点检确认"] = false ;
                        }
                    }
                }
            }
        }

       

        private void fun_查()
        {
            dt.Clear();
            //DataTable dt_查 = new DataTable();
            string a = Convert.ToString(barEditItem1.EditValue);
            string aa = Convert.ToString(barEditItem2.EditValue);
            string sql = "select * from 计量器具基础信息表 where 有效期 >= '" + a + "' and 有效期 <= '" + aa + "'order by 有效期 asc";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
            {
                da.Fill(dt);

            }
            gc1.DataSource = dt;
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_查();
        }

        //private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    DataRow dr;
        //    dr = gv1.GetDataRow(gv1.FocusedRowHandle);
          
        //    上传证书 f_上传 = new 上传证书(dr);
        //    f_上传.ShowDialog();
          
        //}

        private void barLargeButtonItem5_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr;
            dr = gv1.GetDataRow(gv1.FocusedRowHandle);

            上传证书 f_上传 = new 上传证书(dr);
            f_上传.ShowDialog();
        }

        //private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    DataRow dr;
        //    dr = gv1.GetDataRow(gv1.FocusedRowHandle);
        //    //if ()
        //    //{
        //        上传证书 f_上传 = new 上传证书(dr);
        //        f_上传.ShowDialog();
        //    //}
        //    //else
        //    //{
        //    //    MessageBox.Show("请选择要上传的器具");
        //    //}
            
        //}

    }
}
