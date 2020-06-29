using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
namespace 郭恒的DEMO
{
    public partial class 存货核算和成本核算 : Form
    {

        DataTable dt_存货核算;
        //工单成本
        DataTable dt_工单;
        DataTable dt_工单耗用;

        bool bl_calculate = false;
        string strcon = CPublic.Var.strConn;
        public 存货核算和成本核算()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (bl_calculate) MessageBox.Show("正在计算中,请稍候");
                else
                {
                    Thread th = new Thread(() =>
                    {
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            button1.Text = "计算中";
                        }));
                        DateTime t1 = new DateTime(2019, 7, 1); //结算月 初 
                        DateTime t2 = new DateTime(2019, 8, 1); //结算月 末

                        IAACA.IA ia = new IAACA.IA();
                        dt_存货核算 = ia.Cal_inv(t1, t2);
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            button1.Text = "计算完成";
                            bl_calculate = false;
                        }));
                    });
                th.IsBackground = true;
                th.Start();
                bl_calculate = true;
            }

                // ia.fun_返写存货单据单价(dt_存货核算, t1, t2);
                //ERPorg.Corg.TableToExcel(dt_存货核算, @"C:\Users\GH\Desktop\存货核算.xlsx");
        }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                bl_calculate = false;
            }
}

private void button2_Click(object sender, EventArgs e)
{
    try
    {
        if (bl_calculate) MessageBox.Show("正在计算中,请稍候");
        else
        {
            Thread th = new Thread(() =>
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    button2.Text = "计算中";
                }));
                DateTime t1 = new DateTime(2019, 7, 1); //结算月 初 
                        DateTime t2 = new DateTime(2019, 8, 1); //结算月 末
                        DateTime tx = t1.AddMonths(-1);
                string s = string.Format(" select  * from 仓库月出入库结转表 where 年='{0}' and 月='{1}'", tx.Year, tx.Month);
                DataTable t_结转表 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                s = string.Format(@"select a.*,物料名称,规格型号,存货分类,存货分类编码 from C_存货核算物料单价表 a
                             left join 基础数据物料信息表 base on a.物料编码 = base.物料编码
                             where 年 = '{0}' and 月 = '{1}'", t1.Year, t1.Month);
                DataTable t_save = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                DataSet ds = new DataSet();
                ds.Tables.Add(t_save);
                ds.Tables.Add(t_结转表);
                IAACA.IA ia = new IAACA.IA();
                decimal dec_sum辅材 = Convert.ToDecimal(textBox1.Text);
                decimal dec_sum制造 = Convert.ToDecimal(textBox2.Text);
                decimal dec_sum人工 = Convert.ToDecimal(textBox3.Text);
                DataSet ds_return = ia.Cal_成本(ds, t1, t2, dec_sum辅材, dec_sum制造, dec_sum人工);
                dt_工单 = ds_return.Tables[0];
                dt_工单耗用 = ds_return.Tables[1];
                ia.fun_保存过程数量(ds_return, t1, t2);
                string x = "";
                BeginInvoke(new MethodInvoker(() =>
                {
                    button2.Text = "计算完成";
                    bl_calculate = false;
                }));
            });
            th.IsBackground = true;
            th.Start();
            bl_calculate = true;
        }
    }
    catch (Exception ex)
    {

        button2.Text = "成本--计算出错";
        bl_calculate = false;
    }

}
private void fun_check()
{
    if (textBox1.Text.Trim().ToString() == "")
    {
        throw new Exception("辅材金额未输入");
    }
    if (textBox2.Text.Trim().ToString() == "")
    {
        throw new Exception("制造费用未输入");
    }
    if (textBox3.Text.Trim().ToString() == "")
    {
        throw new Exception("人工费用未输入");
    }
}
private void button3_Click(object sender, EventArgs e)
{
    try
    {
        fun_check();
        DataTable dt_exp工单 = dt_工单.Copy();
        dt_exp工单.TableName = "导出工单";
        DataColumn dc = new DataColumn("辅材分摊", typeof(decimal));
        DataColumn dc1 = new DataColumn("工时", typeof(decimal));
        DataColumn dc2 = new DataColumn("工单工时", typeof(decimal));
        DataColumn dc3 = new DataColumn("制造费用", typeof(decimal));
        DataColumn dc4 = new DataColumn("人工费用", typeof(decimal));
        DataColumn dc5 = new DataColumn("软件费用", typeof(decimal));
        dc.DefaultValue = 0;
        dc3.DefaultValue = 0;
        dc4.DefaultValue = 0;
        dc5.DefaultValue = 0;
        dt_exp工单.Columns.Add(dc);
        dt_exp工单.Columns.Add(dc1);
        dt_exp工单.Columns.Add(dc2);
        dt_exp工单.Columns.Add(dc3);
        dt_exp工单.Columns.Add(dc4);
        dt_exp工单.Columns.Add(dc5);

        decimal dec_sum辅材 = Convert.ToDecimal(textBox1.Text);
        decimal dec_sum制造 = Convert.ToDecimal(textBox2.Text);
        decimal dec_sum人工 = Convert.ToDecimal(textBox3.Text);
        decimal dec_总工时 = 0;
        string s = "select  产品编码,工时 from [2019财务工时] ";
        DataTable t_工时 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

        s = "select  产品编码,单价 from [2019财务软件费用] ";
        DataTable t_软件费用 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
        //先给工时 然后算 总工时
        foreach (DataRow dr in dt_exp工单.Rows)
        {
            DataRow[] r_软件费 = t_软件费用.Select(string.Format("产品编码='{0}'", dr["物料编码"]));
            if (r_软件费.Length > 0)
                dr["软件费用"] = Convert.ToDecimal(r_软件费[0]["单价"]) * Convert.ToDecimal(dr["当期完成数量"]);

            if (dr["生产工单类型"].ToString() == "返修工单")
            {
                //若为返修工单 还需要判断是否需要扣除 领料的软件费用
                DataRow[] r_返修领料 = dt_工单耗用.Select(string.Format("生产工单号='{0}'", dr["生产工单号"]));
                foreach (DataRow r in r_返修领料)
                {
                    if (r["子项编码"].ToString().Substring(0, 2) == "10")
                    {
                        DataRow[] rr = t_软件费用.Select(string.Format("产品编码='{0}'", r["子项编码"]));
                        if (rr.Length > 0)
                        {
                            dr["软件费用"] = Convert.ToDecimal(dr["软件费用"]) - Convert.ToDecimal(rr[0]["单价"]) * Convert.ToDecimal(dr["当期完成数量"]);
                        }
                    }
                }
            }
            else
            {
                DataRow[] r_工时 = t_工时.Select(string.Format("产品编码='{0}'", dr["物料编码"]));
                if (r_工时.Length > 0)
                {
                    dr["工时"] = r_工时[0]["工时"];
                    dr["工单工时"] = Convert.ToDecimal(dr["工时"]) * Convert.ToDecimal(dr["当期完成数量"]);
                    dec_总工时 += Convert.ToDecimal(dr["工单工时"]);
                }
            }
        }
        foreach (DataRow dr in dt_exp工单.Rows)
        {
            if (dr["生产工单类型"].ToString() != "返修工单")
            {

                if (dr["工单工时"] != DBNull.Value && dr["工单工时"].ToString() != "")
                {
                    dr["辅材分摊"] = Convert.ToDecimal(dr["工单工时"]) / dec_总工时 * dec_sum辅材;
                    dr["人工费用"] = Convert.ToDecimal(dr["工单工时"]) / dec_总工时 * dec_sum人工;
                    dr["制造费用"] = Convert.ToDecimal(dr["工单工时"]) / dec_总工时 * dec_sum制造;
                }

            }
        }

        ERPorg.Corg.TableToExcel(dt_exp工单, @"C:\Users\GH\Desktop\工单分摊.xlsx");
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message);
    }
}

private void button4_Click(object sender, EventArgs e)
{
    //根据第一遍算出的发出单价 给 形态转换 和 新老编码入库的给对应的发出单价 再重新计算 整个发出单价
    //赋单价 重新算发出单价



}

private void 存货核算和成本核算_Load(object sender, EventArgs e)
{

}
    }
}
