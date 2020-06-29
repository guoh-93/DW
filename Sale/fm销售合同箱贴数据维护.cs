using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPSale
{
    public partial class fm销售合同箱贴数据维护 : Form
    {

        #region  Variable

        DataRow dr_sale;
        string strcon = CPublic.Var.strConn;
        /// <summary>
        /// true 为新增
        /// false 为修改
        /// </summary>
        bool flag = false;

        /// <summary>
        /// 标志是否保存了数据  回到 销售单那边 赋 是否有箱贴 字段
        /// </summary>
        public bool bl = false;
        string s_ddh = "";

        #endregion


        public fm销售合同箱贴数据维护()
        {
            InitializeComponent();
        }
        public fm销售合同箱贴数据维护(string ddh,DataRow dr)
        {
            InitializeComponent();
            dr_sale = dr;
            s_ddh = ddh;
            if (dr_sale["是否有箱贴"].Equals(true))
            {
                bl = true;
                flag = false;
            }
            else
            {
                bl = false;
                flag = true;
            }
        }
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void load()
        {


            if (flag == false) //加载已有数据
            {
                string sql = string.Format(@"select  sxtwh.*
                    from 销售箱贴信息维护表 sxtwh  where  sxtwh.销售订单明细号='{0}'", dr_sale["销售订单明细号"].ToString());
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                DataRow dr = dt.Rows[0];
                dataBindHelper1.DataFormDR(dr);
                textBox6.Text = dr_sale["客户"].ToString();
                textBox22.Text =s_ddh;

            }
            else
            {
                textBox2.Text = dr_sale["销售订单明细号"].ToString();
                textBox4.Text = dr_sale["物料名称"].ToString();
                textBox6.Text = dr_sale["客户"].ToString();
                textBox15.Text = dr_sale["数量"].ToString();
                textBox7.Text = dr_sale["规格型号"].ToString();
                textBox3.Text = dr_sale["物料编码"].ToString();
                textBox22.Text = s_ddh;


                string sql = string.Format("select * from [客户产品单价表] where  客户编号='{0}' and 物料编码='{1}'", dr_sale["客户编号"].ToString(), dr_sale["物料编码"].ToString());
                DataTable dt = new DataTable();
                dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if (dt.Rows.Count == 1)
                {
                    textBox21.Text = dt.Rows[0]["客户规格"].ToString();
                    textBox13.Text = dt.Rows[0]["箱装数量"].ToString();
                    textBox19.Text = dt.Rows[0]["描述"].ToString();
                }
            }

        }

        private void check()
        {
            //if (textBox5.Text == "")
            //{
            //    throw new Exception("合同号为空");

            //} 
            //if (textBox14.Text == "")
            //{
            //    throw new Exception("项目编号为空");

            //}
            //if (textBox1.Text == "")
            //{
            //    throw new Exception("项目名称为空");

            //}
            //if (textBox8.Text == "")
            //{
            //    throw new Exception("电压为空");
            //}
            //else
            //{
            //    string s = textBox8.Text;
            //    s = textBox8.Text.Substring(s.Length - 1, 1);
            //    if (s.ToLower() != "v")
            //    {
            //        throw new Exception("电压未输入单位");
            //    }
            //}
            if (textBox7.Text == "")
            {
                throw new Exception("规格型号为空");

            }
            //if (textBox9.Text == "")
            //{
            //    throw new Exception("电流为空");

            //}
            //else
            //{
            //    string s = textBox9.Text;
            //    s = textBox9.Text.Substring(s.Length - 1, 1);
            //    if (s.ToLower() != "a")
            //    {
            //        throw new Exception("电流未输入单位");
            //    }
            //}
            //if (textBox10.Text == "")
            //{
            //    throw new Exception("计量条形码起为空");

            //}
            //if (textBox11.Text == "")
            //{
            //    throw new Exception("计量条形码止为空");

            //}
            //if (textBox12.Text == "")
            //{
            //    throw new Exception("极数为空");

            //}
            //if (textBox13.Text == "箱装数量为空")
            //{
            //    throw new Exception("");

            //}
        }

        private void fun_save()
        {
            DataTable dt = new DataTable();
            string sql = "";
            DateTime t = CPublic.Var.getDatetime();
            if (flag == true)
            {
                sql = "select * from 销售箱贴信息维护表 where 1<>1 ";
                dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                DataRow dr = dt.NewRow();
                dataBindHelper1.DataToDR(dr);
                dr["销售订单号"] = dr_sale["销售订单号"];
                dr["修改人"] = CPublic.Var.localUserName;
                dr["修改时间"] = t;
                dt.Rows.Add(dr);
            }
            else
            {
                sql = string.Format("select * from 销售箱贴信息维护表 where 销售订单明细号='{0}'", dr_sale["销售订单明细号"].ToString());
                dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                DataRow dr = dt.Rows[0];
                dataBindHelper1.DataToDR(dr);
                dr["修改人"] = CPublic.Var.localUserName;
                dr["修改时间"] = t;
                dr["销售订单号"] = dr_sale["销售订单号"];


            }
            sql = string.Format("select  * from 客户产品单价表 where 客户编号='{0}' and 物料编码='{1}'", dr_sale["客户编号"], dr_sale["物料编码"]);
            DataTable dt_客户产品 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            if (dt_客户产品.Rows.Count == 0)
            {
                DataRow dr = dt_客户产品.NewRow();
                dr["客户编号"] = dr_sale["客户编号"];
                dr["物料编码"] = dr_sale["物料编码"];
                dr["单价"] = dr_sale["税后单价"];
                dr["修改时间"] = t;
                dr["是否可修改"] = true;
                // dr["客户料号"] = "";
                dr["客户规格"] = textBox21.Text;
                dr["箱装数量"] = Convert.ToInt32(textBox13.Text);
                dr["客户描述"] = textBox19.Text;
                dt_客户产品.Rows.Add(dr);

            }
            else
            {
                DataRow dr = dt.Rows[0];
                dr["修改时间"] = t;
                // dr["客户料号"] = "";
                dr["客户规格"] = textBox21.Text;
                dr["箱装数量"] = Convert.ToInt32(textBox13.Text);
                dr["客户描述"] = textBox19.Text;
            }


            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("xtwh");
            try
            {
                SqlCommand cmd = new SqlCommand("select * from 销售箱贴信息维护表 where 1<>1", conn, ts);

                SqlDataAdapter da;
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt);
                cmd = new SqlCommand("select * from 客户产品单价表 where 1<>1", conn, ts);

                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_客户产品);
                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw new Exception("保存失败");
            }
            string ss = string.Format("update 销售记录销售订单明细表 set 是否有箱贴=1 where 销售订单明细号='{0}'", dr_sale["销售订单明细号"]);
            CZMaster.MasterSQL.ExecuteSQL(ss,strcon);
            flag = false;  //修改状态
            bl = true;    //有箱贴
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                check();
                fun_save();
                flag = false;
                bl = true;
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fm销售合同箱贴数据维护_Load(object sender, EventArgs e)
        {
            try
            {

                string s = "select  属性值 from 基础数据基础属性表 where 属性类别='箱贴-托盘' order by POS";
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);

                foreach (DataRow dr in dt.Rows)
                {
                    comboBox1.Items.Add(dr["属性值"].ToString());
                }

                load();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void textBox13_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

        }






    }
}
