using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace ERPSale
{
    public partial class UI售后信息录入 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string strconn2 = CPublic.Var.geConn("DW");
        DataRow dr_参;
        DataTable dt_产品;
        bool bl_查询 = false;

        public UI售后信息录入()
        {
          
            InitializeComponent();
            // //设定按字体来缩放控件 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //设定字体大小为12px     
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            barLargeButtonItem6.Enabled = false;
        }
        public UI售后信息录入(DataRow dr)
        {
            InitializeComponent();
            dr_参 = dr;
            barLargeButtonItem3.Enabled = false;

        }
        public UI售后信息录入(DataRow dr, bool v_查询)
        {
            InitializeComponent();
            dr_参 = dr;
            barLargeButtonItem3.Enabled = false;
            bl_查询 = v_查询;
        }
        //加载
        private void UI售后制成_Load(object sender, EventArgs e)
        {
            //fun_界面调节();
            //if(dr_参!=null)
            //{
            //fun_参数();
            //}
            comboBox2.Text = "制成";
            fun_部门加载();
            fun_加载物料信息();

            fun_加载信息来员();
            if (dr_参 != null)
            {
                fun_参数();
                comboBox2.Text = dr_参["服务类型"].ToString();
            }
            if (bl_查询)
            {
                checkBox3.Enabled = false;
                barLargeButtonItem1.Enabled = false;
                barLargeButtonItem2.Enabled = false;
                barLargeButtonItem4.Enabled = false;
                //barLargeButtonItem6.Enabled = false;
                searchLookUpEdit1.Enabled = false;
                searchLookUpEdit4.Enabled = false;
                checkedComboBoxEdit1.Enabled = false;
                searchLookUpEdit2.Enabled = false;
                textBox4.Enabled = false;
                simpleButton1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox13.Enabled = false;
                textBox11.Enabled = false;
                textBox7.Enabled = false;
            }
              
            //label25.Visible = false;
            // textBox6.Text = "1";
            //登录入员和部门
            
          //  searchLookUpEdit1.Text = CPublic.Var.localUser部门编号;
            
        }
        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_clear();
         }
        //新增
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_clear();
            //label25.Visible = true;
            
        }
        //保存
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_check();
                fun_保存();
                fun_clear();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //关闭
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
 




        private void fun_加载知识分类()
        {
            try
            {               
                 using (SqlDataAdapter da = new SqlDataAdapter("select * from 基础数据基础属性表 where 属性字段1='制成'", strconn))
                 {
                    DataTable dt_属性 = new DataTable();
                    da.Fill(dt_属性);
                    checkedComboBoxEdit1.Properties.DataSource = dt_属性;
                    checkedComboBoxEdit1.Properties.DisplayMember = "属性值";
                    checkedComboBoxEdit1.Properties.ValueMember = "属性值";

                 }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        


        private void fun_clear()
        {
            comboBox2.Text = null;
            售后单号.Text = null;
            searchLookUpEdit1.Text = null;
            searchLookUpEdit2.Text = null;
            textBox8.Text = null;
            searchLookUpEdit4.Text = null;
            textBox10.Text = null;
            textBox5.Text = null;
           // textBox6.Text = null;
           // textBox9.Text = null;
            textBox4.Text = null;
            textBox12.Text = null;
            textBox2.Text = null;
            textBox7.Text = null;
            textBox3.Text = null;
            textBox11.Text = null;
            textBox1.Text = null;
            textBox13.Text = null;
            dr_参 = null;
            barLargeButtonItem3.Enabled = true;
            label32.Visible = false;
            barLargeButtonItem1.Enabled = true;
            barLargeButtonItem2.Enabled = true;
            barLargeButtonItem4.Enabled = true;
            //textBox6.Text = "1";
            //  checkedComboBoxEdit1.Properties.Items.Clear();


        }




        private void fun_部门加载()
        {
            using (SqlDataAdapter da = new SqlDataAdapter("select [部门编号],[部门名称] from [人事基础部门表] order by 部门编号 DESC", strconn))
            {
                DataTable dt_部门 = new DataTable();
                da.Fill(dt_部门);
                searchLookUpEdit1.Properties.DataSource = dt_部门;
                searchLookUpEdit1.Properties.ValueMember = "部门编号";
                searchLookUpEdit1.Properties.DisplayMember = "部门编号";
                searchLookUpEdit1View.PopulateColumns();
            }
           
            }

        private void fun_加载物料信息()
        {
            try
            {
                using (SqlDataAdapter da = new SqlDataAdapter("select 物料编码,物料名称,规格型号 from 基础数据物料信息表 where 停用 =0", strconn))
                {
                    dt_产品 = new DataTable();
                    da.Fill(dt_产品);

                    searchLookUpEdit4.Properties.DataSource = dt_产品;
                    searchLookUpEdit4.Properties.ValueMember = "物料编码";
                    searchLookUpEdit4.Properties.DisplayMember = "物料编码";

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue != null)
            {
                try
                {
                    string proid = searchLookUpEdit1.EditValue.ToString().Trim();
                    string sql = "select 部门名称 from [人事基础部门表] where 部门编号 = '" + proid + "'";
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                    {
                        DataTable dt_部名 = new DataTable();
                        da.Fill(dt_部名);
                        textBox8.Text = dt_部名.Rows[0]["部门名称"].ToString();
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void searchLookUpEdit4_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit4.EditValue != null)
            {
                try
                {
                    DataRow[] ds = dt_产品.Select(string.Format("物料编码 = '{0}'", searchLookUpEdit4.EditValue.ToString()));
                    if (ds.Length > 0)
                    {
                        textBox5.Text = ds[0]["规格型号"].ToString();
                        textBox10.Text = ds[0]["物料名称"].ToString();
                    }                    
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
   

        private void fun_保存()
        {
            if (MessageBox.Show(string.Format("确定保存？"), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                string str = "";
                DataRow dr = null;
                if (dr_参 == null)
                {
                    str = "select * from 知识平台录入表 where 1<>1";
                }
                else
                {
                    str = "select * from 知识平台录入表 where 售后单号 ='" + dr_参["售后单号"] + "'";

                }
                using (SqlDataAdapter da = new SqlDataAdapter(str, strconn))
                {
                    DataTable dtM = new DataTable();
                    da.Fill(dtM);

                    if (dr_参 == null)
                    {
                        dr = dtM.NewRow();
                        dtM.Rows.Add(dr);
                        DateTime t = CPublic.Var.getDatetime();
                        售后单号.Text = string.Format("ZS{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("SH", t.Year, t.Month));
                    }
                    else
                    {
                        dr = dtM.Rows[0];
                    }
                    dr["售后单号"] = 售后单号.Text.ToString();
                    dr["服务类型"] = comboBox2.Text.ToString();
                    dr["部门编号"] = searchLookUpEdit1.Text.ToString();
                    dr["部门名称"] = textBox8.Text.ToString();                 
                    dr["操作人员"] = CPublic.Var.localUserName;
                    dr["操作人员ID"] = CPublic.Var.LocalUserID;
                    dr["产品编码"] = searchLookUpEdit4.Text.ToString();
                    dr["产品名称"] = textBox10.Text.ToString();
                    dr["产品型号"] = textBox5.Text.ToString();
                    dr["数量"] = 1;
                    dr["录入时间"] = CPublic.Var.getDatetime();
                    dr["知识点主题"] = textBox4.Text.ToString();
                    dr["状况描述"] = textBox2.Text.ToString();
                    dr["不良反应"] = textBox3.Text.ToString();
                    dr["问题主因"] = textBox13.Text.ToString();
                    dr["改善方法"] = textBox7.Text.ToString();
                    dr["变更点"] = textBox11.Text.ToString();
                    dr["原因分类"] = checkedComboBoxEdit1.Text.ToString();
                    dr["信息来员"] = textBox1.Text.ToString();
                    dr["信息来员ID"] = searchLookUpEdit2.Text.ToString();
                    new SqlCommandBuilder(da);
                    da.Update(dtM);
                    MessageBox.Show("保存成功！");
                }
            }
        }
        //原因分类上传
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (售后单号.Text == "")
                {
                    throw new Exception("请先保存后在审核界面上传，再上传文件");
                }
                string a_售后单号 = 售后单号.Text.ToString();
                知识平台文件上传 fm = new 知识平台文件上传(a_售后单号);
                fm.ShowDialog();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_check()
        {
            if (comboBox2.Text.Trim() == "")
          {
              throw new Exception("请填写状况描述");
          }
            if (searchLookUpEdit1.Text.Trim() =="")
            {
                throw new Exception("请填写部门编号");
            }
            if (checkBox3.Text.Equals(false))
            {
            if (searchLookUpEdit4.Text.Trim() == "")
            {
                throw new Exception("请填写产品编码");
            }
            }
            //if (textBox6.Text.Trim() =="")
            //{
            //    throw new Exception("请填写数量");
            //}
            //string str = textBox6.Text.Trim();
            //bool isNumber = System.Text.RegularExpressions.Regex.IsMatch(str, @"^[1-9]\d*$");
            //if(isNumber.Equals(false))
            //{
            //    throw new Exception("填写数量要为正整数");
            //}

            if (checkedComboBoxEdit1.Text.Trim() == "")
            {
                throw new Exception("请填写知识分类");
            }
           // if (textBox2.Text.Trim()=="")
           //{
           //    throw new Exception("请填写原因描述");
           //}
            if (searchLookUpEdit2.Text.Trim()== "")
           {
               throw new Exception("请填写信息来员");
           }
            if (textBox3.Text.Trim()=="")
            {
                throw new Exception("请填写不良反应");
           }
            if (textBox13.Text.Trim() == "")
            {
                throw new Exception("请填写问题主因");
            }
            if (textBox7.Text.Trim() == "")
            {
                throw new Exception("请填写改善方法");
            }
            if (textBox11.Text.Trim() == "")
            {
                throw new Exception("请填写变更点");
            }
            if (textBox4.Text.Trim() == "")
            {
                throw new Exception("请填写知识主题");
            }



      }
        private void fun_参数()
        {
            售后单号.Text = dr_参["售后单号"].ToString();
            comboBox2.Text= dr_参["服务类型"].ToString();
            searchLookUpEdit1.Text=dr_参["部门编号"].ToString();
            textBox8.Text = dr_参["部门名称"].ToString();
            //textBox9.Text = dr_参["操作人员"].ToString();
            searchLookUpEdit4.Text = dr_参["产品编码"].ToString();
            textBox10.Text = dr_参["产品名称"].ToString();
            textBox5.Text = dr_参["产品型号"].ToString();
           // textBox6.Text = dr_参["数量"].ToString();
          //  textBox4.Text = Convert.ToDateTime(dr_参["录入时间"]).ToString("yyyy-MM-dd");
            textBox2.Text = dr_参["状况描述"].ToString();
            textBox3.Text = dr_参["不良反应"].ToString();
            textBox7.Text = dr_参["改善方法"].ToString();
            textBox11.Text = dr_参["变更点"].ToString();
            checkedComboBoxEdit1.Text = dr_参["原因分类"].ToString();
            searchLookUpEdit2.Text = dr_参["信息来员ID"].ToString();
            textBox1.Text = dr_参["信息来员"].ToString();
            textBox13.Text = dr_参["问题主因"].ToString();
            textBox4.Text = dr_参["知识点主题"].ToString();
        }
        //作废
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show(string.Format("确定作废？"), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {

                    if (dr_参["审核"].Equals(true))
                    {
                        throw new Exception("此单已审核，不可以作废");

                    }

                    using (SqlDataAdapter da = new SqlDataAdapter("select * from 知识平台录入表 where 售后单号 ='" + dr_参["售后单号"] + "'", strconn))
                    {
                        DataTable dt_作废 = new DataTable();
                        da.Fill(dt_作废);
                        dt_作废.Rows[0]["作废"] = true;
                        new SqlCommandBuilder(da);
                        da.Update(dt_作废);
                        MessageBox.Show("作废成功！");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void fun_加载信息来员()
        {
            using (SqlDataAdapter da = new SqlDataAdapter("select 员工号,姓名 from 人事基础员工表", strconn))
            {
                DataTable dt_人员 = new DataTable();
                da.Fill(dt_人员);

                searchLookUpEdit2.Properties.DataSource = dt_人员;
                searchLookUpEdit2.Properties.ValueMember = "员工号";
                searchLookUpEdit2.Properties.DisplayMember = "员工号";

            }
        }
        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit2.EditValue != null)
            {
                try
                {
                    string proid = searchLookUpEdit2.EditValue.ToString().Trim();
                    using (SqlDataAdapter da = new SqlDataAdapter("select 员工号,姓名 from 人事基础员工表 where 员工号 ='" + proid + "'", strconn))
                    {
                        DataTable dt_人员姓名 = new DataTable();
                        da.Fill(dt_人员姓名);
                        textBox1.Text = dt_人员姓名.Rows[0]["姓名"].ToString();
                      
                    }
                }
                catch
                { }
            }
        }
        //知识类型
        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            fun_加载知识分类();
        }
        //修改
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barLargeButtonItem3.Enabled = true;
            label32.Visible = true;
            searchLookUpEdit1.Enabled = true;
            searchLookUpEdit4.Enabled = true;
            checkedComboBoxEdit1.Enabled = true;
            searchLookUpEdit2.Enabled = true;
            textBox4.Enabled = true;
            simpleButton1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            textBox13.Enabled = true;
            textBox11.Enabled = true;
            textBox7.Enabled = true;
            checkBox3.Enabled = true;
        }

        
        //选择
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked==true)
            {
                searchLookUpEdit4.Visible = false;
            }
            else
            {
                searchLookUpEdit4.Visible = true;
            }



        }







    }
}
