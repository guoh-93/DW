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
    public partial class fm导入库存 : Form
    {
        string strconn = CPublic.Var.strConn;
        DataTable dt_源 = null;
        DataTable dtM = null;
        DataTable dttt = null;

        public fm导入库存()
        {
            InitializeComponent();
        }
        DataTable dt;
        private void fm导入库存_Load(object sender, EventArgs e)
        {
            try
            {
                object s = System.Guid.NewGuid();
                string sql = "select * from 仓库物料表";
                dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                gcm.DataSource = dt;

                string sql2 = "select * from 仓库物料数量表";
                dttt = new DataTable();
                SqlDataAdapter daM = new SqlDataAdapter(sql2, strconn);
                daM.Fill(dttt);

                string sql3 = "select * from cpkc$";
                dt_源 = new DataTable();
                SqlDataAdapter da3 = new SqlDataAdapter(sql3, strconn);
                da3.Fill(dt_源);
                gc.DataSource = dt_源;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int count = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow[] ds = dt_源.Select(string.Format("cpbh = '{0}'", dr["物料编码"]));
                    if (ds.Length > 0)
                    {
                        dr["库存数量"] = Convert.ToDecimal(ds[0]["cpsl"]) + 1000000;
                        //dr["库存总数"] = Convert.ToDecimal(ds[0]["cpsl"]);
                        //dr["有效总数"] = Convert.ToDecimal(ds[0]["cpsl"]);
                        //dr["在途量"] = 0;
                        //dr["在制量"] = 0;
                        //dr["受订量"] = 0;
                        //dr["未领量"] = 0;
                        //dr["MRP计划采购量"] = 0;
                        //dr["MRP计划生产量"] = 0;
                        //dr["MRP库存锁定量"] = 0;
                        //i++;
                    }
                    else
                    {
                        dr["库存数量"] = 1000000;
                        //dr["库存总数"] = 0;
                        //dr["有效总数"] = 0;
                        //dr["在途量"] = 0;
                        //dr["在制量"] = 0;
                        //dr["受订量"] = 0;
                        //dr["未领量"] = 0;
                        //dr["MRP计划采购量"] = 0;
                        //dr["MRP计划生产量"] = 0;
                        //dr["MRP库存锁定量"] = 0;
                        //count++;
                    }
                }
                int i = 0;
                foreach (DataRow dr in dttt.Rows)
                {
                    DataRow[] ds = dt_源.Select(string.Format("cpbh = '{0}'", dr["物料编码"]));
                    if (ds.Length > 0)
                    {
                        dr["库存总数"] = Convert.ToDecimal(ds[0]["cpsl"]) + 1000000;
                        dr["有效总数"] = Convert.ToDecimal(ds[0]["cpsl"]) + 1000000;
                        dr["在途量"] = 1000000;
                        dr["在制量"] = 1000000;
                        dr["受订量"] = 1000000;
                        dr["未领量"] = 1000000;
                        dr["MRP计划采购量"] = 1000000;
                        dr["MRP计划生产量"] = 1000000;
                        dr["MRP库存锁定量"] = 1000000;
                        i++;
                    }
                    else
                    {
                        dr["库存总数"] = 1000000;
                        dr["有效总数"] = 1000000;
                        dr["在途量"] = 1000000;
                        dr["在制量"] = 1000000;
                        dr["受订量"] = 1000000;
                        dr["未领量"] = 1000000;
                        dr["MRP计划采购量"] = 1000000;
                        dr["MRP计划生产量"] = 1000000;
                        dr["MRP库存锁定量"] = 1000000;
                        count++;
                    }
                }
                MessageBox.Show("处理完成" + count.ToString() + "-" + i.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "select * from 仓库物料表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dt);

                string sqll = "select * from 仓库物料数量表 where 1<>1";
                SqlDataAdapter dal = new SqlDataAdapter(sqll, strconn);
                new SqlCommandBuilder(dal);
                dal.Update(dttt);

                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
