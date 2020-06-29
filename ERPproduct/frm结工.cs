using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class frm结工 : Form
    {
        public frm结工()
        {
            InitializeComponent();
        }
        DataTable dt_报工记录表;
#pragma warning disable IDE1006 // 命名样式
        private void textBox3_KeyUp(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (textBox1.Text != "" && textBox2.Text != "")
                    {
                        fun_清空();
                    }
                    label16.Visible = false;
                    string s1 = textBox3.Text.Substring(0, 2);
                    string s2 = textBox3.Text;
                    textBox3.Text = "";
                    if (s1 == "MO")
                    {
                        textBox1.Text = "";
                        textBox1.Text = s2;
                        string sql = string.Format("select * from 生产记录报工记录表 a,生产记录生产工单表 b where a.工单号=b.生产工单号 and  a.工单号='{0}'", textBox1.Text);
                        DataTable dt_生产工单表 = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                        if (dt_生产工单表.Rows.Count > 0)
                        {
                            string sql11 = string.Format("select * from 基础数据物料信息表 where 物料编码='{0}'", dt_生产工单表.Rows[0]["物料编码"].ToString());
                            DataTable dt_基础数据物料信息表 = CZMaster.MasterSQL.Get_DataTable(sql11, CPublic.Var.strConn);
                            text_wlnumber.Text = dt_基础数据物料信息表.Rows[0]["原ERP物料编号"].ToString();
                            text_wlname.Text = dt_生产工单表.Rows[0]["物料名称"].ToString();
                            text_gguigexl.Text = dt_生产工单表.Rows[0]["原规格型号"].ToString();
                            text_scsl.Text = ((decimal)dt_生产工单表.Rows[0]["生产数量"]).ToString("0.00");
                            text_cjname.Text = dt_生产工单表.Rows[0]["车间名称"].ToString();

                            textBox5.Text = dt_生产工单表.Rows[0]["结工时间"].ToString();
                            textBox4.Text = dt_生产工单表.Rows[0]["报工时间"].ToString();

                            checkBox1.Checked = (Boolean)dt_生产工单表.Rows[0]["是否结工"];
                        }
                        else
                        {
                            BaseData.frm消息弹框 fm1 = new BaseData.frm消息弹框("工单号不存在");
                            fm1.ShowDialog();
                            //MessageBox.Show("工单号不存在");
                            return;
                        }
                    }
                    else
                    {
                        string sql = string.Format("select * from 人事基础员工表 where left(卡号,10)='{0}'", s2);
                        DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                        if (dt.Rows.Count > 0)
                        {
                            textBox2.Text = "";
                            textBox2.Text = dt.Rows[0]["员工号"].ToString();
                            text_name.Text = dt.Rows[0]["姓名"].ToString();
                            text_class.Text = dt.Rows[0]["班组"].ToString();
                            text_dpt.Text = dt.Rows[0]["部门"].ToString();
                            text_job.Text = dt.Rows[0]["职务"].ToString();
                            text_team.Text = dt.Rows[0]["课室"].ToString();
                            string sql1 = string.Format("select * from 生产记录报工记录表 where 工号='{0}' and 是否结工=0", textBox2.Text);
                            dt_报工记录表 = CZMaster.MasterSQL.Get_DataTable(sql1, CPublic.Var.strConn);
                            if (dt_报工记录表.Rows.Count == 1) //若这个人只有一个在做的工单 自动赋值 不是的话 再扫要结工的工单
                            {
                                textBox1.Text = dt_报工记录表.Rows[0]["工单号"].ToString();
                                textBox4.Text = dt_报工记录表.Rows[0]["报工时间"].ToString();
                                textBox5.Text = dt_报工记录表.Rows[0]["结工时间"].ToString();
                                checkBox1.Checked = (Boolean)dt_报工记录表.Rows[0]["是否结工"];
                                string sql2 = string.Format("select * from 生产记录生产工单表 where 生产工单号='{0}'", textBox1.Text);
                                DataTable dt_生产工单表 = CZMaster.MasterSQL.Get_DataTable(sql2, CPublic.Var.strConn);
                                if (dt_生产工单表.Rows.Count > 0)
                                {
                                    string sql11 = string.Format("select * from 基础数据物料信息表 where 物料编码='{0}'", dt_生产工单表.Rows[0]["物料编码"].ToString());
                                    DataTable dt_基础数据物料信息表 = CZMaster.MasterSQL.Get_DataTable(sql11, CPublic.Var.strConn);
                                    text_wlnumber.Text = dt_基础数据物料信息表.Rows[0]["原ERP物料编号"].ToString();
                                    text_wlname.Text = dt_生产工单表.Rows[0]["物料名称"].ToString();
                                    text_gguigexl.Text = dt_生产工单表.Rows[0]["原规格型号"].ToString();
                                    text_scsl.Text = ((decimal)dt_生产工单表.Rows[0]["生产数量"]).ToString("0.00");
                                    text_cjname.Text = dt_生产工单表.Rows[0]["车间名称"].ToString();

                                }



                            }
                            else if (dt_报工记录表.Rows.Count == 0)
                            {
                                BaseData.frm消息弹框 fm1 = new BaseData.frm消息弹框("没有再生产的工单号");
                                fm1.ShowDialog();
                                //MessageBox.Show("没有再生产的工单号");
                                return;
                            }
                        }
                        else
                        {
                            BaseData.frm消息弹框 fm1 = new BaseData.frm消息弹框("员工不存在");
                            fm1.ShowDialog();
                            //MessageBox.Show("员工不存在");
                            return;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (textBox1.Text == "")
                {
                    BaseData.frm消息弹框 fm1 = new BaseData.frm消息弹框("工单号不能为空");
                    fm1.ShowDialog();
                    //MessageBox.Show("工单号不能为空");
                    return;
                }
                if (textBox2.Text == "")
                {
                    BaseData.frm消息弹框 fm1 = new BaseData.frm消息弹框("员工号不能为空");
                    fm1.ShowDialog();
                    //MessageBox.Show("员工号不能为空");
                    return;
                }
                string sql = string.Format("select * from 生产记录生产工单表 where 生产工单号='{0}'", textBox1.Text);
                DataTable dt_生产工单表 = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                if (dt_生产工单表.Rows.Count > 1)
                {
                    BaseData.frm消息弹框 fm1 = new BaseData.frm消息弹框("工单号重复");
                    fm1.ShowDialog();
                    //MessageBox.Show("工单号重复");
                    return;
                }
                if (dt_生产工单表.Rows.Count > 0)
                {
                    if (dt_生产工单表.Rows[0]["报工时间"].ToString() != "" || dt_生产工单表.Rows[0]["报工时间"] != null)
                    {
                        if (dt_生产工单表.Rows[0]["结工时间"].ToString() == "" || dt_生产工单表.Rows[0]["结工时间"] == null)
                        {
                            string sql1 = string.Format("select * from 人事基础员工表 where 员工号='{0}'", textBox2.Text);
                            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql1, CPublic.Var.strConn);
                            if (dt.Rows.Count > 0)
                            {
                                string str_name = dt.Rows[0]["姓名"].ToString();
                                string p_order = dt_生产工单表.Rows[0]["生产工单号"].ToString();

                                string str_pID = dt.Rows[0]["员工号"].ToString();
                                //dt_生产工单表.Rows[0]["工单负责人"] = str_name;
                                DateTime dtime = CPublic.Var.getDatetime();
                                dt_生产工单表.Rows[0]["结工时间"] = dtime;
                                dt_生产工单表.Rows[0]["完工日期"] = dtime;
                                dt_生产工单表.Rows[0]["完工"] = true;
                                SqlConnection conn = new SqlConnection(CPublic.Var.strConn);
                                conn.Open();
                                SqlTransaction me = conn.BeginTransaction("报工结工");
                                //SqlDataAdapter da;
                                string sql2 = "select * from 生产记录生产工单表 where 1<>1";
                                //da = new SqlDataAdapter(sql2, CPublic.Var.strConn);
                                //new SqlCommandBuilder(da);
                                //da.Update(dt_生产工单表);

                                string sql3 = string.Format("select * from 生产记录报工记录表 where 1<>1");
                                DataRow[] r = dt_报工记录表.Select(string.Format("工单号='{0}' and 工号='{1}'", p_order, str_pID));
                                if (r.Length > 0)
                                {
                                    dt_报工记录表.Rows[0]["结工时间"] = dtime;
                                    dt_报工记录表.Rows[0]["是否结工"] = true;
                                    textBox5.Text = dtime.ToShortDateString();

                                    //SqlDataAdapter da2;
                                    //da2 = new SqlDataAdapter(sql3, CPublic.Var.strConn);
                                    //new SqlCommandBuilder(da2);
                                    //da2.Update(dt_报工记录表);
                                }
                                try
                                {
                                    SqlCommand cmm_1 = new SqlCommand(sql2, conn, me);
                                    SqlCommand cmm_2 = new SqlCommand(sql3, conn, me);
                                    SqlDataAdapter da1 = new SqlDataAdapter(cmm_1);
                                    SqlDataAdapter da2 = new SqlDataAdapter(cmm_2);
                                    new SqlCommandBuilder(da1);
                                    new SqlCommandBuilder(da2);
                                    da1.Update(dt_生产工单表);
                                    da2.Update(dt_报工记录表);
                                    me.Commit();
                                }
                                catch (Exception ex)
                                {
                                    me.Rollback();
                                    throw new Exception("结工失败,请重试一遍");
                                }
                        

       
                            }
                            checkBox1.Checked = true;

                            BaseData.frm消息弹框 fm1 = new BaseData.frm消息弹框("结工成功！");
                            fm1.ShowDialog();
                            //MessageBox.Show("结工成功！");
                            label16.Visible = true;
                        }
                        else
                        {
                            BaseData.frm消息弹框 fm1 = new BaseData.frm消息弹框("工单号已结工");
                            fm1.ShowDialog();
                            //MessageBox.Show("工单号已结工");
                            return;
                        }
                    }
                    else
                    {
                        BaseData.frm消息弹框 fm1 = new BaseData.frm消息弹框("工单号未报工");
                        fm1.ShowDialog();
                        //MessageBox.Show("工单号未报工");
                        return;
                    }

                }
                else
                {
                    BaseData.frm消息弹框 fm1 = new BaseData.frm消息弹框("工单号不存在");
                    fm1.ShowDialog();
                    //MessageBox.Show("工单号不存在");
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void timer1_Tick(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            textBox3.Focus();
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_清空()
#pragma warning restore IDE1006 // 命名样式
        {
            textBox1.Text = "";
            text_wlnumber.Text = "";
            text_wlname.Text = "";
            text_gguigexl.Text = "";
            text_scsl.Text = "";
            text_cjname.Text = "";
            textBox2.Text = "";
            text_name.Text = "";
            text_class.Text = "";
            text_dpt.Text = "";
            text_job.Text = "";
            text_team.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            checkBox1.Checked = false;
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.Close();
        }


    }
}
