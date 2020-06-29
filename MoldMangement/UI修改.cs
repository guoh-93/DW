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
    public partial class UI修改 : UserControl
    {
        //static string strConn = "Password = a; Persist Security Info = True; User ID = sa; Initial Catalog = fms; Data Source = XINREN "; 
        string strConn = CPublic.Var.strConn;
        DataTable dt_主 = new DataTable();
        DataTable dt_细 = new DataTable();
         DataRow dr_修;
         DataTable dt_已有 = new DataTable();
        /// <summary>
        /// true为新增状态
        /// </summary>
         bool bl = false;

        public UI修改()
        {
            InitializeComponent();
        }

        public UI修改(DataRow dr)
        {
            InitializeComponent();
            dr_修 = dr;
        }

        private void fun_load()
        {
        
            string sql = string.Format ("select * from 计量器具申请主表 where 申请单号= '{0}'",dr_修 ["申请单号"].ToString());
            using(SqlDataAdapter da = new SqlDataAdapter (sql,strConn ))
            {
                dt_主 = new DataTable();
                da.Fill(dt_主);
            
            }
            string sql2 = string.Format("select * from 计量器具申请明细表 where 申请单号 = '{0}'",dr_修 ["申请单号"].ToString());
            using (SqlDataAdapter da = new SqlDataAdapter (sql2 ,strConn ))
            {
                dt_细 = new DataTable();
                da.Fill(dt_细);
            
            }
            gc1.DataSource = dt_细;
            fun_下拉();
           bl = false;
           dt_细.RowDeleted += dt_细_RowDeleted; 

        }

        void dt_细_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            int i = 0;
            foreach (DataRow r in dt_细.Rows)
            {
                if (r.RowState != DataRowState.Deleted)
                {
                    r["申请单号"] = textBox1.Text;
                    string srr = dt_细.Rows[0][0].ToString();
                    //string ss = srr.Split('-')[1].ToString();
                    int a = 1 + i;
                    i++;
                    //string ss2 = srr.Split('-')[0].ToString();
                    r["申请明细号"] = r["申请单号"] + "-" + a;
                }
                
            }
               throw new NotImplementedException();
        }


        private void UI修改_Load(object sender, EventArgs e)
        {
            fun_load();
            textBox1.Text = dr_修["申请单号"].ToString();
            comboBox1.Text = dr_修["申请类别"].ToString();
            dateEdit1.Text = Convert.ToDateTime(dr_修["申请时间"]).ToString("yyyy-MM-dd");
            textBox5.Text = dr_修["申请人员"].ToString();
            textBox4.Text = dr_修["申请部门"].ToString();
            textBox2.Text = dr_修["申请原因"].ToString();
        }
        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
        }
        //生效
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //string sql = "select * from 计量器具申请明细表 where 1<>1";
            //using (SqlDataAdapter da = new SqlDataAdapter (sql,strConn ))
            //{
            //    new SqlCommandBuilder(da);
            //    da.Update(dt_细);
            
            //}
            if (dt_细.Rows.Count > 0)
            {
                foreach (DataRow r in dt_细.Rows)
                {
                    if (r.RowState != DataRowState.Deleted)
                    {
                        string a = r["计量器具名称"].ToString();
                        string aa = r["计量器具规格"].ToString();
                        string aaa = r["数量"].ToString();
                        if (a == "" || aa == "" || aaa == "")
                        {
                            MessageBox.Show("明细表新增时没有填写数据，请填写数据再保存");
                            return;
                        }
                        else if (Convert.ToInt32(aaa) < 0)
                        {
                            MessageBox.Show("数量填写错误，请重新填写");
                            return;

                        }
                    }
                }
                check();
                gv1.CloseEditor();//关闭编辑状态
                this.BindingContext[dt_细].EndCurrentEdit();//关闭编辑状态
                fun_主表();
                fun_子表();
            }


        }

        private void fun_主表()
        {
            DataTable dt_修 = new DataTable();
            try
            {
                string sql_修改 = string.Format("select * from 计量器具申请主表 where 申请单号='{0}'", textBox1.Text);
                using (SqlDataAdapter da = new SqlDataAdapter(sql_修改, strConn))
                {
                    da.Fill(dt_修);
                }
                dt_修.Rows [0]["申请原因"] = textBox2.Text;
                dateEdit1.Text = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
                dt_修.Rows[0]["申请时间"] = dateEdit1.Text;
                string sql = "select * from 计量器具申请主表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
                {

                    new SqlCommandBuilder(da);
                    da.Update(dt_修);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);


            }
        }

        private void fun_子表()
        {
            if (bl)
            {

               
                    string sql2 = string.Format("select max(申请明细号) from 计量器具申请明细表 where 申请单号 = '{0}'", textBox1.Text);
                    DataTable dt2 = new DataTable();

                    using (SqlDataAdapter da = new SqlDataAdapter(sql2, strConn))
                    {

                        da.Fill(dt2);

                        int i = 1;
                        //DataRow[] drr = dt_细.Select(string.Format("申请单号 = '{0}'", textBox1.Text));
                        foreach (DataRow der in dt_细.Rows)
                        {
                            string sa = der["申请明细号"].ToString();
                            if (sa == null || sa == "")
                            {
                                der["申请单号"] = textBox1.Text;
                                string srr = dt2.Rows[0][0].ToString();
                                string ss = srr.Split('-')[1].ToString();
                                int a = Convert.ToInt32(ss) + i;
                                i++;
                                string ss2 = srr.Split('-')[0].ToString();
                                der["申请明细号"] = ss2 + "-" + a.ToString ().PadLeft(2,'0');
                                
                            }
                           
                        }
                     }
            }

            try
            {

                string sql = "select * from 计量器具申请明细表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
                {
                   
                   
                    new SqlCommandBuilder(da);
                    da.Update(dt_细);
                }
                MessageBox.Show("修改成功");
                barLargeButtonItem1_ItemClick(null, null);

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
        DataRow dr;
        //申购新增明细
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            
            dr = dt_细.NewRow();
            dt_细.Rows.Add(dr);
            this.gridColumn1.OptionsColumn.AllowEdit = true;
            this.gridColumn2.OptionsColumn.AllowEdit = true ;
            bl = true;
        }
        //删除明细
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DataRow dr;
            dr = gv1.GetDataRow(gv1.FocusedRowHandle);
            if (MessageBox.Show("确认删除吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                if (dr == null)
                {
                    MessageBox.Show("没有选中行，无法删除");

                }
                else
                {
                    dr.Delete();
                   
                }
            }
        }
        //其他新增明细
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.Text != "")
            {
                DataRow dr_已有;
                DataRow[] drr = dt_已有.Select(string.Format("计量器具名称 = '{0}'", searchLookUpEdit1.Text));
                dr_已有 = dt_细.NewRow();
                foreach (DataRow dr in drr)
                {
                    dr_已有["计量器具名称"] = dr["计量器具名称"];
                    dr_已有["计量器具规格"] = dr["计量器具规格"];
                    dr_已有["数量"] = 1;
                    dr_已有["备注"] = "";

                }
                dt_细.Rows.Add(dr_已有);
                this.gridColumn1.OptionsColumn.AllowEdit = false;
                this.gridColumn2.OptionsColumn.AllowEdit = false;
                this.gridColumn3.OptionsColumn.AllowEdit = false;
                this.gridColumn4.OptionsColumn.AllowEdit = false;

            }
            else
            {
                MessageBox.Show("请先在旁边下拉按钮选择物品再新增");
                return;
            }
        }

        private void fun_下拉()
        {
           
            string sql2 = "select 计量器具编号,计量器具名称,计量器具规格,所属大类,所属部门,制造单位,状态 from 计量器具基础信息表";
            using (SqlDataAdapter da = new SqlDataAdapter(sql2, strConn))
            {
                dt_已有 = new DataTable();
                da.Fill(dt_已有);

            }
            searchLookUpEdit1.Properties.DataSource = dt_已有;
            searchLookUpEdit1.Properties.ValueMember = "计量器具编号";
            searchLookUpEdit1.Properties.DisplayMember = "计量器具名称";

        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            //if (comboBox1.Text == "申购")
            //{

            //    if (dt_细.Rows.Count > 0)
            //    {
            //        if (comboBox1.Text == "申购")
            //        {
            //            if (MessageBox.Show("填写的数据没保存，确定继续吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            //            {
            //                dt_细.Clear();
            //                textBox2.Clear();

            //            }
            //            else
            //            {

            //                return;
            //            }


            //        }
            //    }
            //    else
            //    {

            //        simpleButton1.Visible = true;
            //        simpleButton3.Visible = false;
            //        searchLookUpEdit1.Visible = false;
            //        label7.Visible = false;
            //        this.simpleButton2.Location = new System.Drawing.Point(130, 5);
            //    }
            //}
            //else
            //{
            //    if (dt_细.Rows.Count > 0)
            //    {
            //        if (comboBox1.Text != "申购")
            //        {

            //            if (MessageBox.Show("填写的数据没保存，确定继续吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            //            {
            //                dt_细.Clear();
            //                textBox2.Clear();

            //            }
            //            else
            //            {
            //                return;
            //            }

            //        }
            //        else
            //        {
            //            simpleButton1.Visible = false;
            //            simpleButton3.Visible = true;
            //            this.simpleButton3.Location = new System.Drawing.Point(12, 5);
            //            searchLookUpEdit1.Visible = true;
            //            this.searchLookUpEdit1.Location = new System.Drawing.Point(120, 11);
            //            this.simpleButton2.Location = new System.Drawing.Point(230, 5);
            //            label7.Visible = true;
            //            this.label7.Location = new System.Drawing.Point(350, 16);

            //        }
            //    }


            //}
        }

        private void check()
        {
            if (comboBox1.Text.Trim() == "")
            {
                throw new Exception("申请类别不可以为空");

            }
            if (textBox2.Text.Trim() == "")
            {
                throw new Exception("申请原因不可以为空");

            }

        }

        private void gv1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

    }
}
