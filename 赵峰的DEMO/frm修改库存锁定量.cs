using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 赵峰的DEMO
{
    public partial class frm修改库存锁定量 : Form
    {
        string strconn = CPublic.Var.strConn;
        DataTable dt;
        public frm修改库存锁定量()
        {
            InitializeComponent();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            fun_计算();
            //Decimal de = 0;
            //foreach (DataRow r in dt.Rows)
            //{
            //    string sql = string.Format("select 数量 from 基础数据物料BOM表 where 产品编码 = '{0}' and 子项编码 = '00381'", r["物料编码"].ToString());
            //    DataTable t = new DataTable();
            //    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            //    da.Fill(t);
            //    de = de + Convert.ToDecimal(r["数量"]) * Convert.ToDecimal(t.Rows[0]["数量"]);
            //}
            //MessageBox.Show(de.ToString());
        }

        private void fm刷新界面B_Load(object sender, EventArgs e)
        {
//            string sql = @"  select * from (
//  select [销售记录销售订单明细表].物料编码,[销售记录销售订单明细表].数量 from [销售记录销售订单明细表] 
//  right join 基础数据物料BOM表 on [销售记录销售订单明细表].物料编码 = 基础数据物料BOM表.产品编码 
//  where 基础数据物料BOM表.子项编码 = '00381') as a where 物料编码 != ''";
//            dt = new DataTable();
//            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
//            da.Fill(dt);
//            gridControl1.DataSource = dt;
            fun_载入();
            gridControl1.DataSource = dtt;
        }

        DataTable dtt;
        DataTable dtt2;
        DataTable dtt3;
        private void fun_载入()
        {
            string sql = @"select 仓库物料数量表.* from 仓库物料数量表 left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 仓库物料数量表.物料编码 
            where 基础数据物料信息表.物料类型 = '成品'";//基础数据物料信息表.*,
            dtt = new DataTable();
            SqlDataAdapter daa = new SqlDataAdapter(sql, strconn);
            daa.Fill(dtt);

            string sql2 = @"select 仓库物料数量表.* from 仓库物料数量表 left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 仓库物料数量表.物料编码 
            where 基础数据物料信息表.物料类型 = '半成品'";//基础数据物料信息表.*,
            dtt2 = new DataTable();
            SqlDataAdapter daa2 = new SqlDataAdapter(sql2, strconn);
            daa2.Fill(dtt2);

            string sql3 = @"select 仓库物料数量表.* from 仓库物料数量表 left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 仓库物料数量表.物料编码 
            where 基础数据物料信息表.物料类型 = '原材料'";//基础数据物料信息表.*,
            dtt3 = new DataTable();
            SqlDataAdapter daa3 = new SqlDataAdapter(sql3, strconn);
            daa3.Fill(dtt3);
        }

        private Decimal fun(string str)
        {
            string sqll = string.Format(@" select [销售记录销售订单明细表].物料编码,基础数据物料BOM表.数量,仓库物料数量表.受订量, 仓库物料数量表.库存总数 from [销售记录销售订单明细表] 
                                        left join 基础数据物料BOM表 on [销售记录销售订单明细表].物料编码 = 基础数据物料BOM表.产品编码 
                                        left join 仓库物料数量表 on 仓库物料数量表.物料编码 = [销售记录销售订单明细表].物料编码
                                        where [销售记录销售订单明细表].明细完成 = 0 and 销售记录销售订单明细表.已计算 = 1 and 基础数据物料BOM表.子项编码 = '{0}'
                                        group by [销售记录销售订单明细表].物料编码,基础数据物料BOM表.数量,仓库物料数量表.受订量, 仓库物料数量表.库存总数", str);
            DataTable dt_锁定计算 = new DataTable();
            SqlDataAdapter da_锁定计算 = new SqlDataAdapter(sqll, strconn);
            da_锁定计算.Fill(dt_锁定计算);
            Decimal de_求和 = 0;
            foreach (DataRow r in dt_锁定计算.Rows)
            {
                if ((Convert.ToDecimal(r["受订量"]) - Convert.ToDecimal(r["库存总数"])) > 0)
                {
                    de_求和 = de_求和 + (Convert.ToDecimal(r["受订量"]) - Convert.ToDecimal(r["库存总数"])) * Convert.ToDecimal(r["数量"]);
                }
            }
            return de_求和;
            //if (Convert.ToDecimal(dt.Rows[0]["MRP库存锁定量"]) > (Convert.ToDecimal(dt.Rows[0]["受订量"]) + de_求和))
            //{
            //    dt.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(dt.Rows[0]["受订量"]) + de_求和;
            //}
        }

        private void fun_计算()
        {
            foreach (DataRow dr in dtt.Rows)
            {
                if (Convert.ToDecimal(dr["受订量"]) >= Convert.ToDecimal(dr["MRP库存锁定量"]))
                {}
                else
                {
                    Decimal de = 0;
                    try
                    {
                        de = fun(dr["物料编码"].ToString());
                    }
                    catch
                    {
                        de = 0;
                    }
                    dr["MRP库存锁定量"] = Convert.ToDecimal(dr["受订量"]);
                }
                if (Convert.ToDecimal(dr["MRP计划生产量"]) < 0)
                {
                    dr["MRP计划生产量"] = 0;
                }
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            foreach (DataRow dr in dtt2.Rows)
            {
                {
                    Decimal de =0;
                    try
                    {
                        de = fun(dr["物料编码"].ToString());
                    }
                    catch
                    {
                        de = 0;
                    }
                    if (Convert.ToDecimal(dr["MRP库存锁定量"]) > (Convert.ToDecimal(dr["受订量"]) + de))
                    {
                        dr["MRP库存锁定量"] = Convert.ToDecimal(dr["受订量"]) + de;
                    }
                }
                if (Convert.ToDecimal(dr["MRP计划生产量"]) < 0)
                {
                    dr["MRP计划生产量"] = 0;
                }
            }
            //int i = 0;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (DataRow dr in dtt3.Rows)
            {
                {
                    Decimal de = 0;
                    try                                             
                    {
                        de = fun(dr["物料编码"].ToString());
                    }
                    catch
                    {
                        de = 0;
                    }
                    if (Convert.ToDecimal(dr["MRP库存锁定量"]) > (Convert.ToDecimal(dr["受订量"]) + de))
                    {
                        dr["MRP库存锁定量"] = Convert.ToDecimal(dr["受订量"]) + de;
                    }
                }
                if (Convert.ToDecimal(dr["MRP计划采购量"]) < 0)
                {
                    dr["MRP计划采购量"] = 0;
                }
            }
           // int i = 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string sql = @"select * from 仓库物料数量表 where 1 <> 1";
            SqlDataAdapter daa = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(daa);
            daa.Fill(dtt);

            daa.Fill(dtt2);

            daa.Fill(dtt3);
        }
    }
}
