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
    public partial class frm修改人事员工 : Form
    {
        DataTable dtP = new DataTable();
        string strconn = CPublic.Var.strConn;

        public frm修改人事员工()
        {
            InitializeComponent();
        }

        DataTable dt_新ERP;
        DataTable dt_老ERP;
        private void button1_Click(object sender, EventArgs e)
        {
            string sql = @"select * from 人事基础员工表 where 在职状态 = '在职'";
            dt_新ERP = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_新ERP);
            gridControl1.DataSource = dt_新ERP;

            string sql2 = @"select * from 在职人员信息$ where 在职状态 = '在职'";
            dt_老ERP = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
            da2.Fill(dt_老ERP);
            gridControl2.DataSource = dt_老ERP;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            #region
            foreach (DataRow dr in dt_新ERP.Rows)
            {
                DataRow[]ds = dt_老ERP.Select(string.Format("工号 = '{0}'", dr["员工号"].ToString()));
                if (ds.Length > 0)
                {
                    if (ds[0]["工作鞋尺寸"] == null || ds[0]["工作鞋尺寸"].ToString() == "") { dr["工作鞋尺寸"] = ""; }
                    else
                    {
                        dr["工作鞋尺寸"] = ds[0]["工作鞋尺寸"].ToString();
                    }
                    if (ds[0]["领用件数"] == null || ds[0]["领用件数"].ToString() == "") { dr["领用件数鞋"] = 0; }
                    else
                    {
                        dr["领用件数鞋"] = Convert.ToInt32(ds[0]["领用件数"]);
                    }
                    if (ds[0]["第一次领用时间鞋子"] == null || ds[0]["第一次领用时间鞋子"].ToString() == "") { dr["第一次领用时间鞋"] = DBNull.Value; }
                    else
                    {
                        dr["第一次领用时间鞋"] = ds[0]["第一次领用时间鞋子"].ToString();
                    }
                    if (ds[0]["第二次领用时间鞋子"] == null || ds[0]["第二次领用时间鞋子"].ToString() == "") { dr["第二次领用时间鞋"] = DBNull.Value; }
                    else
                    {
                        dr["第二次领用时间鞋"] = ds[0]["第二次领用时间鞋子"].ToString();
                    }
                    if (ds[0]["领用件数外套"] == null || ds[0]["领用件数外套"].ToString() == "") { dr["领用件数外套"] = 0; }
                    else
                    {
                        dr["领用件数外套"] = Convert.ToInt32(ds[0]["领用件数外套"]);
                    }
                    if (ds[0]["第一次领用时间外套"] == null || ds[0]["第一次领用时间外套"].ToString() == "") { dr["第一次领用时间外套"] = DBNull.Value; }
                    else
                    {
                        dr["第一次领用时间外套"] = ds[0]["第一次领用时间外套"].ToString();
                    }
                    if (ds[0]["第二次领用时间外套"] == null || ds[0]["第二次领用时间外套"].ToString() == "") { dr["第二次领用时间外套"] = DBNull.Value; }
                    else
                    {
                        if (ds[0]["第二次领用时间外套"].ToString() == "0")
                        {
                            dr["第二次领用时间外套"] = DBNull.Value; 
                            continue;
                        }
                        dr["第二次领用时间外套"] = ds[0]["第二次领用时间外套"].ToString();
                    }
                }
            }
            #endregion
            MessageBox.Show("1");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "select * from 人事基础员工表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dt_新ERP);
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
