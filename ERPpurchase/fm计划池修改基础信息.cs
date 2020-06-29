using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPpurchase
{
    public partial class fm计划池修改基础信息 : Form
    {

        #region  变量
        string str_物料编码 = "";
        /// <summary>
        /// 是否保存
        /// </summary>
        string strcon = CPublic.Var.strConn;
        public bool bl = false;
        public decimal dec=0;
        DataTable dtM;
        string sql = "";
        #endregion

        public fm计划池修改基础信息(string s)
        {
            InitializeComponent();
            this.Text = "修改基础信息";
            str_物料编码 = s;
        }
        private void fun_checked()
        {
            try
            {
                decimal dec = 0;
                try
                {
                    dec = Convert.ToDecimal(textBox4.Text);
                }
                catch (Exception ex)
                {

                    throw new Exception("库存下限输入值有问题,请检查");
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private void fm计划池修改基础信息_Load(object sender, EventArgs e)
        {
            sql = string.Format("select * from 基础数据物料信息表 where  物料编码='{0}'",str_物料编码);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dtM = new DataTable();
                da.Fill(dtM);
                textBox1.Text = dtM.Rows[0]["物料编码"].ToString();
                textBox2.Text = dtM.Rows[0]["物料名称"].ToString();
                textBox3.Text = dtM.Rows[0]["n原ERP规格型号"].ToString();
       

            }
        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_checked();

                string ss = "select  * from  基础数据物料信息修改日志表 where 1<>1 ";
                DataTable dt_xgrz = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(ss, strcon))
                {
                    da.Fill(dt_xgrz);
                    DataRow dr = dt_xgrz.NewRow();
                    dr["GUID"] = System.Guid.NewGuid();
                    dr["姓名"] = CPublic.Var.localUserName;
                    dr["员工号"] = CPublic.Var.LocalUserID;
                    dr["内容"] = string.Format("修改了:库存下限 原:{0};现：{1}", dtM.Rows[0]["库存下限"].ToString(), Convert.ToDecimal(textBox4.Text));
                    dr["日期"] = CPublic.Var.getDatetime();
                    dr["GUID"] = System.Guid.NewGuid();
                    dr["物料编码"] = dtM.Rows[0]["物料编码"].ToString();
                    dtM.Rows[0]["库存下限"]=dec= Convert.ToDecimal(textBox4.Text);
                    dt_xgrz.Rows.Add(dr);

                }
                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction xg = conn.BeginTransaction("修改基础信息");
                try
                {
                    SqlCommand cmm_0 = new SqlCommand(sql, conn, xg);
                    SqlCommand cmm_1 = new SqlCommand(ss, conn, xg);


                    SqlDataAdapter da1 = new SqlDataAdapter(cmm_0);
                    SqlDataAdapter da2 = new SqlDataAdapter(cmm_1);


                    new SqlCommandBuilder(da1);
                    new SqlCommandBuilder(da2);

                    da1.Update(dtM);
                    da2.Update(dt_xgrz);

                    xg.Commit();

                    bl = true;
                }
                catch
                {
                    xg.Rollback();
                    throw new Exception("保存失败");
                }

                MessageBox.Show("修改成功");
                this.Close();
            }
            catch (Exception ex)
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
