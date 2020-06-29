using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm报工 : Form
#pragma warning restore IDE1006 // 命名样式
    {
        /// <summary>
        /// 标记 是否正在打印过程中
        /// </summary>
        bool flag = false;

        public frm报工()
        {
            InitializeComponent();
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
                string sql = string.Format(@"select 生产记录生产工单表.*,特征码,原ERP物料编号 from 生产记录生产工单表
                                            left join 基础数据物料信息表 on 基础数据物料信息表.物料编码=生产记录生产工单表.物料编码
                                            Left  join   基础物料标签维护信息表 on    基础数据物料信息表.原ERP物料编号=  基础物料标签维护信息表.物料编号                                
                                            where 生产工单号='{0}'", textBox1.Text);
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
                    if (dt_生产工单表.Rows[0]["报工时间"].ToString() == "" || dt_生产工单表.Rows[0]["报工时间"] == null)
                    {
                        string sql1 = string.Format("select * from 人事基础员工表 where 员工号='{0}'", textBox2.Text);
                        DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql1, CPublic.Var.strConn);
                        DataRow dr_工单 = dt_生产工单表.Rows[0];
                        if (dt.Rows.Count > 0)
                        {

                            SqlConnection conn = new SqlConnection(CPublic.Var.strConn);
                            conn.Open();
                            SqlTransaction mb = conn.BeginTransaction("报工开工");

                            string str_name = dt.Rows[0]["姓名"].ToString();
                            dt_生产工单表.Rows[0]["工单负责人"] = str_name;
                            dt_生产工单表.Rows[0]["工单负责人ID"] = dt.Rows[0]["员工号"].ToString();
                            DateTime dtime = CPublic.Var.getDatetime();
                            dt_生产工单表.Rows[0]["报工时间"] = dtime;
                            //SqlDataAdapter da;
                            string sql2 = "select * from 生产记录生产工单表 where 1<>1";
                            //da = new SqlDataAdapter(sql2, CPublic.Var.strConn);
                            //new SqlCommandBuilder(da);
                            //da.Update(dt_生产工单表);

                            string sql3 = string.Format("select * from 生产记录报工记录表 where 1<>1");
                            DataTable dt_报工记录表 = CZMaster.MasterSQL.Get_DataTable(sql3, CPublic.Var.strConn);
                            DataRow drr = dt_报工记录表.NewRow();
                            drr["工单号"] = textBox1.Text;
                            drr["工号"] = textBox2.Text;
                            drr["报工时间"] = dtime;
                            drr["姓名"] = str_name;
                            dt_报工记录表.Rows.Add(drr);
                        

                            //SqlDataAdapter da2;
                            //da2 = new SqlDataAdapter(sql3, CPublic.Var.strConn);
                            //new SqlCommandBuilder(da2);
                            //da2.Update(dt_报工记录表);

                            try
                            {
                                SqlCommand cmm_1 = new SqlCommand(sql2, conn, mb);
                                SqlCommand cmm_2 = new SqlCommand(sql3, conn, mb);
                                SqlDataAdapter da1 = new SqlDataAdapter(cmm_1);
                                SqlDataAdapter da2 = new SqlDataAdapter(cmm_2);
                                new SqlCommandBuilder(da1);
                                new SqlCommandBuilder(da2);
                                da1.Update(dt_生产工单表);
                                da2.Update(dt_报工记录表);

                                mb.Commit();
                            }
                            catch (Exception ex)
                            {
                                mb.Rollback();
                                throw new Exception("结工失败,请重试一遍");
                            }
                            try
                            {
                                if (dt.Rows[0]["课室编号"].ToString() == "0001030103" && dt_生产工单表.Rows[0]["特征码"]!=null && dt_生产工单表.Rows[0]["特征码"].ToString()!="" )
                                {
                                    if (MessageBox.Show(string.Format("是否打印小标签？"), "确认!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                                    {

                                    
                                            string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                                            ERPproduct.frm工单生效选择 frm = new frm工单生效选择();

                                            //第三参数为 true  才需要 赋值 第四个参数
                                            //frm.fun_制三标签B2(dr_工单, PrinterName, false, 0);
                                            Thread BG = new Thread(() => frm.fun_制三标签B2(dr_工单, PrinterName, false, 0));
                                            BG.IsBackground = true;
                                            BG.Start();
                                           
                                            //指示是否正在打印
                               
                                        //string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                                        //ERPproduct.frm工单生效选择 frm = new frm工单生效选择();

                                        ////第三参数为 true  才需要 赋值 第四个参数
                                        //frm.fun_制三标签B2(dr_工单, PrinterName, false,0);
                                    }
                                }
 
                            }
                            catch (Exception ex)
                            {

                                MessageBox.Show(ex.Message);
                            }
                            
                        }
                        label3.Visible = true;
                        BaseData.frm消息弹框 fm1 = new BaseData.frm消息弹框("报工成功！");
                        fm1.ShowDialog();
                        //MessageBox.Show("报工成功！");

                    }
                    else
                    {
                        MessageBox.Show("工单号已报工");
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
        private void frm报工_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            textBox3.Focus();
        }

#pragma warning disable IDE1006 // 命名样式
        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            textBox3.Focus();
        }

#pragma warning disable IDE1006 // 命名样式
        private void textBox3_KeyUp(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {

                    label3.Visible = false;
                    if (textBox1.Text != "" && textBox2.Text != "")
                    {
                        fun_清空();
                    }
                    string s1 = textBox3.Text.Substring(0, 2);
                    string s2 = textBox3.Text;
                    textBox3.Text = "";
                    if (s1 == "MO")
                    {
                        textBox1.Text = "";
                        textBox1.Text = s2;
                        string sql = string.Format("select * from 生产记录生产工单表 where 生产工单号='{0}'", textBox1.Text);
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
                            //string sql1 = string.Format("select * from 生产记录报工记录表 where 工号='{0}' and 是否结工 = 0", dt.Rows[0]["员工号"].ToString());
                            //DataTable dt2 = CZMaster.MasterSQL.Get_DataTable(sql1,CPublic.Var.strConn);
                            //if (dt2.Rows.Count > 0)
                            //{
                            //    BaseData.frm消息弹框 fm1 = new BaseData.frm消息弹框(string.Format("工号：{0} 已经报工！", dt.Rows[0]["员工号"].ToString()));
                            //    fm1.ShowDialog();
                            //    //MessageBox.Show(string.Format("工号：{0} 已经报工！", dt.Rows[0]["员工号"].ToString()));
                            //    return;
                            //}
                            textBox2.Text = "";
                            textBox2.Text = dt.Rows[0]["员工号"].ToString();
                            text_name.Text = dt.Rows[0]["姓名"].ToString();
                            text_class.Text = dt.Rows[0]["班组"].ToString();
                            text_dpt.Text = dt.Rows[0]["部门"].ToString();
                            text_job.Text = dt.Rows[0]["职务"].ToString();
                            text_team.Text = dt.Rows[0]["课室"].ToString();

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
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.Close();
        }


    }
}
