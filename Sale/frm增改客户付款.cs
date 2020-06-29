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
    public partial class frm增改客户付款 : Form
    {
        bool bl_新增 = false;//指示是否新增  默认为修改状态
        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        DataRow r_传;
        DataTable dt_客户;
        public frm增改客户付款()
        {
            InitializeComponent();
            bl_新增 = true;
        }
        public frm增改客户付款(DataRow r)
        {
            InitializeComponent();
            r_传 = r;

        }

        private void frm增改客户付款_Load(object sender, EventArgs e)
        {
            try
            {
                dateEdit1.EditValue = CPublic.Var.getDatetime();
                fun_load();
                if (r_传 != null)
                {
                    dataBindHelper1.DataFormDR(r_传);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        private void fun_load()
        {
            string sql = string.Format(@"select 客户编号,客户名称 from 客户基础信息表");
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            dt_客户 = new DataTable();
            da.Fill(dt_客户);
            searchLookUpEdit1.Properties.DataSource = dt_客户;
            searchLookUpEdit1.Properties.DisplayMember = "客户编号";
            searchLookUpEdit1.Properties.ValueMember = "客户编号";
            if (r_传 != null)
            {
                dataBindHelper1.DataFormDR(r_传);
            }
            string sql_1 = "select  * from [客户付款记录表] where 1<>1";
            dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);

        }

        private void fun_save()
        {
            if (bl_新增)
            {
                string sql = string.Format("select  * from [客户付款记录表] where 单号='{0}'", textBox1.Text);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if (dt.Rows.Count > 0)
                {
                    throw new Exception("该单号已重复,请核对");
                }
                DataRow dr = dtM.NewRow();
                dataBindHelper1.DataToDR(dr);

                int yy = Convert.ToDateTime(dateEdit1.EditValue).Year;
                int mm = Convert.ToDateTime(dateEdit1.EditValue).Month;
                int dd = Convert.ToDateTime(dateEdit1.EditValue).Day;
                string str_流水号 = CPublic.CNo.fun_得到最大流水号("FD", yy, mm, dd).ToString("000");
                dr["流水号"] = yy.ToString("00") + mm.ToString("00") + dd.ToString("00") + "-" + str_流水号;
               
                //textBox1.Text = dr["单号"].ToString();
                dr["操作日期"] = CPublic.Var.getDatetime();
                dr["录入日期"] = CPublic.Var.getDatetime();
                dr["工号"] = CPublic.Var.LocalUserID;
                dr["录入人员"] = CPublic.Var.localUserName;

                dtM.Rows.Add(dr);
                CZMaster.MasterSQL.Save_DataTable(dtM, "客户付款记录表", strcon);

            }
            else
            {
                //string sql_检 = string.Format("select  * from [客户付款记录表] where 单号='{0}'", textBox1.Text);
                //DataTable dt_检 = CZMaster.MasterSQL.Get_DataTable(sql_检, strcon);
                //if (dt_检.Rows.Count >= 1)
                //{
                //    throw new Exception("该单号已重复,请核对");
                //}
                if (r_传 != null)
                {
                    string sql = "select  *  from 客户付款记录表 where 单号='" + r_传["单号"] + "'";
                    DataTable dt = new DataTable();
                    dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    if (dt.Rows.Count > 0)
                    {
                        dataBindHelper1.DataToDR(dt.Rows[0]);
                        dt.Rows[0]["操作日期"] = CPublic.Var.getDatetime();


                        dt.Rows[0]["录入人员"] = CPublic.Var.localUserName;
                        dt.Rows[0]["工号"] = CPublic.Var.LocalUserID;

                        CZMaster.MasterSQL.Save_DataTable(dt, "客户付款记录表", strcon);
                    }
                }


            }
        }
        private void fun_check()
        {
            if (searchLookUpEdit1.EditValue == null && searchLookUpEdit1.EditValue.ToString() == "")
            {
                throw new Exception("未选择客户");

            }
            if (dateEdit1.EditValue == null && dateEdit1.EditValue.ToString() == "")
            {
                throw new Exception("未选择付款日期");

            }
            if (textBox2.Text.ToString() == "")
            {
                throw new Exception("未填写总金额");
            }
            try
            {
                decimal a = Convert.ToDecimal(textBox2.Text);
            }
            catch (Exception ex)
            {
                throw new Exception("填写金额格式不对");
            }


            if (textBox5.Text.ToString() == "")
            {
                throw new Exception("未填货款金额");
            }
            try
            {
                decimal a = Convert.ToDecimal(textBox5.Text);
            }
            catch (Exception ex)
            {
                throw new Exception("填写金额格式不对");
            }
            if (textBox6.Text.ToString() == "")
            {
                throw new Exception("未填写模具金额");
            }
            try
            {
                decimal a = Convert.ToDecimal(textBox6.Text);
            }
            catch (Exception ex)
            {
                throw new Exception("填写金额格式不对");
            }
            if (textBox7.Text.ToString() == "")
            {
                throw new Exception("未填写其他金额");
            }
            try
            {
                decimal a = Convert.ToDecimal(textBox7.Text);
            }
            catch (Exception ex)
            {
                throw new Exception("填写金额格式不对");
            }
            if (Convert.ToDecimal(textBox2.Text) != Convert.ToDecimal(textBox7.Text) + Convert.ToDecimal(textBox6.Text) + Convert.ToDecimal(textBox5.Text))
            {
                throw new Exception("模具金额加上货款金额加上其他金额不等于总金额");
                 
            }


        }
        //保存
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                fun_check();
                fun_save();
                MessageBox.Show("ok");
                bl_新增 = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                CZMaster.MasterLog.WriteLog(ex.Message);
            }

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bl_新增 = true;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            searchLookUpEdit1.EditValue = "";
            dateEdit1.EditValue = CPublic.Var.getDatetime();
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
            {
                DataRow[] dr = dt_客户.Select(string.Format("客户编号='{0}'", searchLookUpEdit1.EditValue));
                textBox4.Text = dr[0]["客户名称"].ToString();
            }
        }
    }
}
