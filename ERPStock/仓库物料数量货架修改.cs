using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPStock
{
    public partial class 仓库物料数量货架修改 : Form
    {
        public string hjms = "";
         public bool fl = false;
        DataRow dr_CR;
        string strcon = CPublic.Var.strConn;
        public 仓库物料数量货架修改(DataRow dr )
        {
            InitializeComponent();
            dr_CR = dr;
        }
        string 状态;
        public 仓库物料数量货架修改(DataRow dr ,string a )
        {
            InitializeComponent();
            状态 = a;
            dr_CR = dr;
        }

        private void fun_GetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                da.Fill(dt);
            }
        }

        private void fun_SetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                new SqlCommandBuilder(da);
                da.Update(dt);
            }
        }

        private void frm仓库物料数量货架修改_Load(object sender, EventArgs e)
        {
           textBox1.Text= dr_CR ["物料编码"].ToString();
           textBox3.Text= dr_CR["仓库名称"].ToString();
           //string sql = string.Format("select 物料名称 from 基础数据物料信息表 where  物料编码='{0}'", dr_CR["物料编码"].ToString());
           //DataRow drsas = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
           textBox4.Text = dr_CR["物料名称"].ToString();
           textBox2.Text=dr_CR ["货架描述"].ToString();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fl = true;
                DataTable dt = new DataTable();
               
                if(状态=="1"){
                    string sql = string.Format("select * from 仓库物料数量明细表 where guid ='{0}'", dr_CR["guid"].ToString());
                    fun_GetDataTable(dt, sql);

                    dt.Rows[0]["货架描述"] = textBox2.Text; //一个仓库一个物料只能放在一个货架上,可放于不同仓库

                    hjms = textBox2.Text;
                    string sql2 = string.Format("select * from 仓库物料数量明细表 where 1<>1");
                    fun_SetDataTable(dt, sql2);
                    MessageBox.Show("保存成功");

                }else{
                
                string sql = string.Format("select * from 仓库物料数量表 where 物料编码 ='{0}' and 仓库号='{1}'", dr_CR["物料编码"].ToString(),dr_CR["仓库号"].ToString());
                fun_GetDataTable(dt, sql);
             
                dt.Rows[0]["货架描述"] = textBox2.Text; //一个仓库一个物料只能放在一个货架上,可放于不同仓库
               
                hjms = textBox2.Text;
                string sql2 = string.Format("select * from 仓库物料数量表 where 1<>1");
                fun_SetDataTable(dt, sql2);
                MessageBox.Show("保存成功");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            this.Close();
        }
    }
}
