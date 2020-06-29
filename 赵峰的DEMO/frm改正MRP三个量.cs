using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace 赵峰的DEMO
{
    public partial class frm改正MRP三个量 : Form
    {
        string strconn = CPublic.Var.strConn;

        public frm改正MRP三个量()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }

        private void fun_()
        {

        }

        private void fun()
        {
            string str_str = "";
            string sql2 = "select 仓库物料数量表.*,基础数据物料信息表.物料类型 from 仓库物料数量表 join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 仓库物料数量表.物料编码 where 物料类型 = '成品'";
            DataTable dt2 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
            da2.Fill(dt2);
            gc.DataSource = dt2;

            foreach (DataRow dr in dt2.Rows)
            {
                try
                {
                    //if (dr["物料类型"].ToString() == "成品")
                    {
                        dr["MRP计划生产量"] = Convert.ToDecimal(dr["受订量"]) + Convert.ToDecimal(dr["未领量"]) - Convert.ToDecimal(dr["在制量"]) - Convert.ToDecimal(dr["在途量"]) - Convert.ToDecimal(dr["库存总数"]);
                        if (Convert.ToDecimal(dr["MRP计划生产量"]) > 0)
                        {
                            //代表确实缺
                        }
                        else
                        {
                            dr["MRP计划生产量"] = 0;
                        }

                        if ((Convert.ToDecimal(dr["库存总数"]) + Convert.ToDecimal(dr["在制量"]) + Convert.ToDecimal(dr["在途量"])) >= (Convert.ToDecimal(dr["受订量"]) + Convert.ToDecimal(dr["未领量"])))
                        {
                            dr["MRP库存锁定量"] = Convert.ToDecimal(dr["受订量"]) + Convert.ToDecimal(dr["未领量"]);
                        }
                        else
                        {
                            dr["MRP库存锁定量"] = Convert.ToDecimal(dr["库存总数"]) + Convert.ToDecimal(dr["在制量"]) + Convert.ToDecimal(dr["在途量"]);
                        }
                    }
                    

                }
                catch (Exception ex)
                {
                    str_str = str_str + ";" + dr["物料编码"].ToString();
                    continue;
                }
            }
            sql2 = "select * from 仓库物料数量表 where 1<>1";
            da2 = new SqlDataAdapter(sql2, strconn);
            new SqlCommandBuilder(da2);
            da2.Update(dt2);

            string file = @"C://成品.txt";
            if (File.Exists(file) == true)
            {
                System.IO.File.WriteAllText(file, str_str);
            }

            MessageBox.Show("OK 1");
        }

        private void fun_1()
        {
            string str_str = "";
            string sql2 = "select 仓库物料数量表.*,基础数据物料信息表.物料类型 from 仓库物料数量表 join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 仓库物料数量表.物料编码 where 物料类型 = '半成品'";
            DataTable dt2 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
            da2.Fill(dt2);
            gc.DataSource = dt2;
             
            foreach (DataRow dr in dt2.Rows)
            {
                try
                {
                    //if (dr["物料类型"].ToString() == "半成品")
                    {
                        string sqll = string.Format(@"select SUM(数量 * a.MRP计划生产量) - b.库存总数 - b.在制量 + b.未领量 as MRP计划生产量,b.库存总数,b.在制量,b.未领量,b.受订量,b.在途量 from 基础数据物料BOM表 
                            left join 仓库物料数量表 as a on a.物料编码 = 基础数据物料BOM表.产品编码 
                            left join 仓库物料数量表 as b on b.物料编码 = 基础数据物料BOM表.子项编码 
                            where a.MRP计划生产量 > 0 and 子项编码 = '{0}' group by b.库存总数,b.在制量,b.未领量,b.受订量,b.在途量", dr["物料编码"].ToString());
                        DataTable dtt = new DataTable();
                        SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
                        daa.Fill(dtt);
                        if (dtt.Rows.Count > 0)
                        {
                            if (Convert.ToDecimal(dtt.Rows[0]["MRP计划生产量"]) > 0)
                            {
                                dr["MRP计划生产量"] = Convert.ToDecimal(dtt.Rows[0]["MRP计划生产量"]);
                            }
                            else
                            {
                                dr["MRP计划生产量"] = 0;
                            }
                        }

                        sqll = string.Format(@"select SUM(数量 * a.MRP计划生产量) as 半成品欠缺数 from 基础数据物料BOM表 
                            left join 仓库物料数量表 as a on a.物料编码 = 基础数据物料BOM表.产品编码  
                            where a.MRP计划生产量 > 0 and 子项编码 = '{0}'", dr["物料编码"].ToString());
                        dtt = new DataTable();
                        daa = new SqlDataAdapter(sqll, strconn);
                        daa.Fill(dtt);
                        if ((Convert.ToDecimal(dtt.Rows[0]["半成品欠缺数"]) + Convert.ToDecimal(dr["未领量"])) >= (Convert.ToDecimal(dr["库存总数"]) + Convert.ToDecimal(dr["在制量"])))
                        {
                            dr["MRP库存锁定量"] = Convert.ToDecimal(dr["库存总数"]) + Convert.ToDecimal(dr["在制量"]);
                        }
                        else
                        {
                            dr["MRP库存锁定量"] = Convert.ToDecimal(dtt.Rows[0]["半成品欠缺数"]) + Convert.ToDecimal(dr["未领量"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    str_str = str_str + ";" + dr["物料编码"].ToString();
                    continue;
                }
            }
            sql2 = "select * from 仓库物料数量表 where 1<>1";
            da2 = new SqlDataAdapter(sql2, strconn);
            new SqlCommandBuilder(da2);
            da2.Update(dt2);
            new SqlCommandBuilder(da2);
            da2.Update(dt2);
            string file = @"C://半成品.txt";
            if (File.Exists(file) == true)
            {
                System.IO.File.WriteAllText(file, str_str);
            }
            //MessageBox.Show("OK 2");
        }

        private void fun_2()
        {
            string str_str = "";
            string sql2 = "select 仓库物料数量表.*,基础数据物料信息表.物料类型 from 仓库物料数量表 join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 仓库物料数量表.物料编码 where 物料类型 = '原材料'";
            DataTable dt2 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
            da2.Fill(dt2);
            gc.DataSource = dt2;
             
            foreach (DataRow dr in dt2.Rows)
            {
                try
                {
                    //if (dr["物料类型"].ToString() == "原材料")
                    {
                        string sqll = string.Format(@"select SUM(数量 * a.MRP计划生产量) - b.库存总数 - b.在制量 - b.在途量 + b.未领量 + b.受订量 as MRP计划采购量,b.库存总数,b.在制量,b.未领量,b.受订量,b.在途量 from 基础数据物料BOM表 
                            left join 仓库物料数量表 as a on a.物料编码 = 基础数据物料BOM表.产品编码 
                            left join 仓库物料数量表 as b on b.物料编码 = 基础数据物料BOM表.子项编码 
                            where a.MRP计划生产量 > 0 and 子项编码 = '{0}' group by b.库存总数,b.在制量,b.未领量,b.受订量,b.在途量", dr["物料编码"].ToString());
                        DataTable dtt = new DataTable();
                        SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
                        daa.Fill(dtt);
                        dr["MRP计划采购量"] = Convert.ToDecimal(dtt.Rows[0]["MRP计划采购量"]);
                        if (Convert.ToDecimal(dr["MRP计划采购量"]) > 0)
                        {
                            //代表确实缺
                        }
                        else
                        {
                            dr["MRP计划采购量"] = 0;
                        }

                        sqll = string.Format(@"select SUM(数量 * a.MRP计划生产量) as 原材料欠缺数 from 基础数据物料BOM表 
                            left join 仓库物料数量表 as a on a.物料编码 = 基础数据物料BOM表.产品编码  
                            where a.MRP计划生产量 > 0 and 子项编码 = '{0}'", dr["物料编码"].ToString());
                        dtt = new DataTable();
                        daa = new SqlDataAdapter(sqll, strconn);
                        daa.Fill(dtt);
                        if ((Convert.ToDecimal(dtt.Rows[0]["原材料欠缺数"]) + Convert.ToDecimal(dr["未领量"]) + Convert.ToDecimal(dr["受订量"])) >= (Convert.ToDecimal(dr["库存总数"]) + Convert.ToDecimal(dr["在制量"]) + Convert.ToDecimal(dr["在途量"])))
                        {
                            dr["MRP库存锁定量"] = Convert.ToDecimal(dr["库存总数"]) + Convert.ToDecimal(dr["在制量"]) + Convert.ToDecimal(dr["在途量"]);
                        }
                        else
                        {
                            dr["MRP库存锁定量"] = Convert.ToDecimal(dtt.Rows[0]["半成品欠缺数"]) + Convert.ToDecimal(dr["未领量"]) + Convert.ToDecimal(dr["受订量"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    str_str = str_str + ";" + dr["物料编码"].ToString();
                    continue;
                }
            }
            sql2 = "select * from 仓库物料数量表 where 1<>1";
            da2 = new SqlDataAdapter(sql2, strconn);
            new SqlCommandBuilder(da2);
            da2.Update(dt2);
            new SqlCommandBuilder(da2);
            da2.Update(dt2);
            string file = @"C://原材料.txt";
            if (File.Exists(file) == true)
            {
                System.IO.File.WriteAllText(file, str_str);
            }
            MessageBox.Show("OK 3");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fun();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            fun_1(); 
            button3_Click(null, null);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            fun_2();
        }
    }
}
