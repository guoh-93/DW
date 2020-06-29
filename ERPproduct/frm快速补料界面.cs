using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPproduct
{
    public partial class frm快速补料界面 : Form
    {

        string strcon = CPublic.Var.strConn;
        DataTable dtM;

        public frm快速补料界面()
        {
            InitializeComponent();
        }



        //
      

#pragma warning disable IDE1006 // 命名样式
        private void  fun_scanMO(string s)
#pragma warning restore IDE1006 // 命名样式
        {
            string sql_1 = string.Format("select * from 生产记录生产工单待领料主表  where 生产工单号='{0}' ", s);

            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
            if (dt.Rows.Count > 0)
            {

                //                    string sql = string.Format(@"select a.*,人事基础部门表.部门名称 from (select 生产记录生产工单待领料明细表.*,仓库物料数量表.有效总数,仓库物料数量表.库存总数,基础数据物料信息表.原ERP物料编号 from 生产记录生产工单待领料明细表 
                //                                              left join 仓库物料数量表  on   仓库物料数量表.物料编码= 生产记录生产工单待领料明细表.物料编码 
                //                                              left join 基础数据物料信息表  on 基础数据物料信息表.物料编码=生产记录生产工单待领料明细表.物料编码
                //                                              where 生产记录生产工单待领料明细表.生产工单号='{0}') a   
                //                                              left join 人事基础部门表 on 人事基础部门表.部门编号 = a.生产车间 ", barEditItem1.EditValue.ToString());

                string sql = string.Format(@"select a.*,a.数量 as BOM数量,a.计量单位 from (select 基础数据物料BOM表.*,车间编号,仓库物料数量表.有效总数,仓库物料数量表.库存总数
                                              ,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.物料编码,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号 as 规格型号 
                                               from 基础数据物料BOM表 
                                              left join 仓库物料数量表  on   仓库物料数量表.物料编码= 基础数据物料BOM表.子项编码 
                                              left join 基础数据物料信息表  on 基础数据物料信息表.物料编码=基础数据物料BOM表.子项编码
                                              where 基础数据物料BOM表.物料编码='{0}') a  ", dt.Rows[0]["物料编码"]);


                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    dtM = new DataTable();
                    da.Fill(dtM);
                    if (dtM.Rows.Count > 0)
                    {
                        dtM.Columns.Add("选择", typeof(bool));
                        dtM.Columns.Add("输入领料数量");
                        gridControl1.DataSource = dtM;
                    }
                    else
                    {
                        MessageBox.Show("未搜索到物料BOM清单");
                    }


                }
            }
            else
            {
                MessageBox.Show("请正确输入工单号");
            }



        }

#pragma warning disable IDE1006 // 命名样式
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.KeyCode == Keys.Enter)
            {
                    string s1 = textBox1.Text.Substring(0, 2);
                    string s2 = textBox1.Text;
                    textBox1.Text = "";
                    if (s1 == "MO")
                    {
                        textBox3.Text = s2;
                        fun_scanMO(s2);
                    }
                    else
                    {
                        textBox2.Text = s2;
                        string s = string.Format("select  员工号 from 人事基础员工表 where 卡号='{0}'",s2);
                         DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn);
                         if (dt.Rows.Count > 0)
                         {
                             textBox2.Text = dt.Rows[0]["员工号"].ToString();

                         }
                         else
                         {
                            BaseData.frm消息弹框 fm1 = new BaseData.frm消息弹框("员工不存在");
                            fm1.ShowDialog();
                            textBox2.Text = "";
                            //MessageBox.Show("员工不存在");
                            return;
                        
                         }
                    }

            }

        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {
            //int i = 0;
            DataView dv = new DataView(dtM);
            dv.RowFilter = "选择=1";
            if (dv.Count == 0)
            {
                throw new Exception("未选择需补料的物料");
            }
            foreach (DataRow r in dtM.Rows)
            {
                if (r["选择"].Equals(true))
                {
                    try
                    {
                        decimal a = Convert.ToDecimal(r["输入领料数量"]);

                        if (a <= 0)
                        {
                            throw new Exception("领料数量不能小于0,请重新输入");

                        }
                    }
                    catch
                    {
                        throw new Exception("请正确输入领料数量格式");

                    }
                    if (Convert.ToDecimal(r["输入领料数量"]) > Convert.ToDecimal(r["库存总数"]))
                    {
                        throw new Exception("库存总数不足！");
                    }
                    //if (Convert.ToDecimal(r["输入领料数量"]) > Convert.ToDecimal(r["未领数量"]))
                    //{
                    //    i++;

                    //}

                }
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_save()
#pragma warning restore IDE1006 // 命名样式
        {

            string s=string.Format("select  员工号,姓名 from  人事基础员工表 where 员工号 ='{0}' ",textBox2.Text);
            DataTable t_人  =CZMaster.MasterSQL.Get_DataTable(s,strcon);

            DateTime t = CPublic.Var.getDatetime().Date;
            string str_待领料单号 = string.Format("DL{0}{1:D2}{2:D4}", t.Year, t.Month, CPublic.CNo.fun_得到最大流水号("DL", t.Year, t.Month));
            string str_待领料号; //用来搜索对应的 明细 
            //现在 待领料主表中新增一条记录 
            DataTable dt;
            string sql = string.Format("select * from 生产记录生产工单待领料主表 where 生产工单号='{0}' ", textBox3.Text);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dt = new DataTable();
                da.Fill(dt);

                str_待领料号 = dt.Rows[0]["待领料单号"].ToString();

                dt.Rows[0]["待领料单号"] = str_待领料单号;
                dt.Rows[0]["完成"] = 0;
                dt.Rows[0]["关闭"] = 0;
                dt.Rows[0]["创建日期"] = t;
                dt.Rows[0]["制单人员"] =  t_人.Rows[0]["姓名"];
                dt.Rows[0]["制单人员ID"] = textBox2.Text;
                dt.Rows[0]["领料类型"] = "生产补料";

                dt.AcceptChanges();
                dt.Rows[0].SetAdded();

            }
            //待领料明细表 中
            string sql_mx = "select * from 生产记录生产工单待领料明细表 where 1<>1";
            DataTable dt_mx = CZMaster.MasterSQL.Get_DataTable(sql_mx, strcon);
            using (SqlDataAdapter da = new SqlDataAdapter(sql_mx, strcon))
            {
                dtM.AcceptChanges();
                int pos = 0;
                foreach (DataRow dr in dtM.Rows)
                {
                    if (dr["选择"].Equals(true))
                    {
                        DataRow r_mx = dt_mx.NewRow();

                        r_mx["待领料单号"] = str_待领料单号;
                        r_mx["待领料单明细号"] = str_待领料单号 + pos++.ToString("00");
                        r_mx["待领料总量"] = dr["输入领料数量"];
                        r_mx["生产工单号"] = textBox3.Text;
                        r_mx["生产制令单号"] = dt.Rows[0]["生产制令单号"];
                        r_mx["生产工单类型"] = "生产补料";
                        r_mx["物料编码"] = dr["物料编码"];
                        r_mx["物料名称"] = dr["物料名称"];
                        r_mx["生产车间"] = dt.Rows[0]["生产车间"];
                        r_mx["领料人ID"] = dt.Rows[0]["领料人ID"];
                        r_mx["领料人"] = dt.Rows[0]["领料人"];
                        r_mx["生产车间"] = dt.Rows[0]["生产车间"];
                        r_mx["规格型号"] = dr["规格型号"];

                        r_mx["已领数量"] = 0;
                        r_mx["未领数量"] = dr["输入领料数量"];
                        r_mx["制单人员"] =  t_人.Rows[0]["姓名"];
                        r_mx["制单人员ID"] = textBox2.Text;
                        r_mx["修改日期"] = r_mx["创建日期"] = t;
                        r_mx["完成"] = 0;

                        dt_mx.Rows.Add(r_mx);

                        //dr.AcceptChanges();

                        // dr.SetAdded();
                    }

                }

            }

            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction pb = conn.BeginTransaction("生产补料");
            try
            {
                SqlCommand cmm_1 = new SqlCommand(sql, conn, pb);
                SqlCommand cmm_2 = new SqlCommand(sql_mx, conn, pb);

                SqlDataAdapter da1 = new SqlDataAdapter(cmm_1);
                SqlDataAdapter da2 = new SqlDataAdapter(cmm_2);



                new SqlCommandBuilder(da1);
                new SqlCommandBuilder(da2);

                da1.Update(dt);
                da2.Update(dt_mx);

                pb.Commit();
            }
            catch (Exception ex)
            {
                pb.Rollback();
                throw new Exception("生产补料失败，请重试");
            }


        }
        //确认领料
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (MessageBox.Show(string.Format("是否确认补领这些料？"), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    gridView1.CloseEditor();
                    this.BindingContext[dtM].EndCurrentEdit();

                    fun_check();

                    fun_save();
                    dtM = dtM.Clone();
                    gridControl1.DataSource = dtM;
                    textBox2.Text = "";
                    textBox3.Text = "";

                    MessageBox.Show("生效成功");
                  
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
            this.Close();
        }
    }
}
