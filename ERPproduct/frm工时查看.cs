using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class frm工时查看 : Form
    {
        public frm工时查看()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void textBox2_KeyUp(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {


                    string s1 = textBox2.Text.Substring(0, 2);
                    string s2 = textBox2.Text;
                    textBox2.Text = "";
                    if (s1 == "MO")
                    {
                        //textBox1.Text = "";
                        //textBox1.Text = s2;
                    }
                    else
                    {
                        string sql = string.Format("select * from 人事基础员工表 where 卡号='{0}'", s2);
                        DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                        if (dt.Rows.Count > 0)
                        {
                            textBox1.Text = "";
                            textBox1.Text = dt.Rows[0]["员工号"].ToString();
                            label3.Text = dt.Rows[0]["姓名"].ToString();
                            label5.Text = dt.Rows[0]["班组"].ToString();
                            label7.Text = dt.Rows[0]["课室"].ToString();
                            label9.Text = "";
                            label11.Text = "";
                            label13.Text = "";
                            DateTime dtime = CPublic.Var.getDatetime();
                            DateTime startMonth = dtime.AddDays(1 - dtime.Day);  //本月月初 
                            DateTime endMonth = startMonth.AddMonths(1).AddDays(-1);  //本月月末
                            DateTime startMonth1 = Convert.ToDateTime(startMonth.ToString("yyyy-MM-dd"));
                            DateTime endMonth1 = Convert.ToDateTime(endMonth.ToString("yyyy-MM-dd") + " " + "23:59:59");
                        //    string sql1 = string.Format("select * from 生产记录报工记录表 where 工号='{0}' and 是否结工 = 1 and 结工时间 >='{1}'and 结工时间<='{2}'", textBox1.Text, startMonth1, endMonth1);
                        //    DataTable 报工记录表 = CZMaster.MasterSQL.Get_DataTable(sql1, CPublic.Var.strConn);
                        //    if (报工记录表.Rows.Count > 0)
                        //    {
                        //        string sql2 = string.Format("select * from 生产记录生产工单表 where 工单负责人ID='{0}'", textBox1.Text);
                        //        DataTable dt_生产工单表 = CZMaster.MasterSQL.Get_DataTable(sql2, CPublic.Var.strConn);
                        //        DataTable dt_Main = new DataTable();
                        //        dt_Main = 报工记录表.Clone();
                        //        dt_Main.Columns.Add("工时");
                        //        dt_Main.Columns.Add("物料编码");
                        //        dt_Main.Columns.Add("物料名称");
                        //        dt_Main.Columns.Add("规格型号");
                        //        dt_Main.Columns.Add("生产数量");
                        //        decimal sum = 0;
                        //        foreach (DataRow dr in 报工记录表.Rows)
                        //        {
                        //            DataRow[] drr = dt_生产工单表.Select(string.Format("生产工单号='{0}'", dr["工单号"].ToString()));
                        //            DataRow dr1 = dt_Main.NewRow();
                        //            dr1["工单号"] = dr["工单号"].ToString();
                        //            dr1["报工时间"] = dr["报工时间"].ToString();
                        //            dr1["工号"] = dr["工号"].ToString();
                        //            dr1["报工时间"] = dr["报工时间"].ToString();
                        //            dr1["结工时间"] = dr["结工时间"].ToString();
                        //            dr1["姓名"] = dr["姓名"].ToString();
                        //            dr1["是否结工"] = dr["是否结工"].ToString();
                        //            dr1["工时"] = drr[0]["工时"].ToString();
                        //            dr1["物料编码"] = drr[0]["物料编码"].ToString();
                        //            dr1["物料名称"] = drr[0]["物料名称"].ToString();
                        //            dr1["规格型号"] = drr[0]["规格型号"].ToString();
                        //            dr1["生产数量"] = drr[0]["生产数量"].ToString();
                        //            dt_Main.Rows.Add(dr1);
                        //            sum += (decimal)drr[0]["工时"];
                        //        }
                        //        label9.Text = sum.ToString();
                        //        gridControl1.DataSource = dt_Main;
                                


                        //    }
                        //    else
                        //    {
                        //        MessageBox.Show("没有已经完成的工单号");
                        //        return;
                        //    }


                            string sql22 = string.Format(@"select d.原ERP物料编号,a.工单号,c.物料名称,a.报工时间,a.结工时间,c.工时,c.生产数量,c.原规格型号
                                            from 生产记录报工记录表 a,人事基础员工表 b, 生产记录生产工单表 c,基础数据物料信息表 d where a.工单号 =c.生产工单号 and a.工号=b.员工号
                                            and c.物料编码 =d.物料编码 and a.工号='{0}' and a.是否结工 = 1 and a.结工时间 >='{1}'and a.结工时间<='{2}'", textBox1.Text, startMonth1, endMonth1);
                            DataTable dt_M = CZMaster.MasterSQL.Get_DataTable(sql22, CPublic.Var.strConn);
                            decimal sum = 0;
                            foreach (DataRow dr in dt_M.Rows)
                            {
                                sum += (decimal)dr["工时"];
                            }
                            label11.Text = sum.ToString("0.00");
                            gridControl1.DataSource = dt_M;

                            string sql33 = string.Format("select * from 基础数据辅助工时表 where 生产人员ID='{0}' and 工作日期 >='{1}'and 工作日期<='{2}'", textBox1.Text, startMonth1, endMonth1);
                            DataTable dt_M2 = CZMaster.MasterSQL.Get_DataTable(sql33, CPublic.Var.strConn);
                            decimal sum2 = 0;
                            foreach (DataRow dr in dt_M2.Rows)
                            {
                                sum2 += (decimal)dr["折算工时"];
                            }
                            label13.Text = sum2.ToString("0.00");
                            label9.Text = (sum + sum2).ToString("0.00");
                            gridControl2.DataSource = dt_M2;
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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void textBox1_TextChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.Close();
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm工时查看_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            label3.Text = "";
            label5.Text = "";
            label7.Text = "";
            label9.Text = "";
            label11.Text = "";
            label13.Text = "";
        }

#pragma warning disable IDE1006 // 命名样式
        private void timer1_Tick(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            textBox2.Focus();
        }
    }
}
