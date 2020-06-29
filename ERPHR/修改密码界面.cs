using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ERPorg
{
    public partial class 修改密码界面 : UserControl
    {
        #region   变量

        DataTable dtM;
        string strcon = CPublic.Var.strConn;
        string s;
        /// <summary>
        /// 指示是否修改过密码
        /// </summary>
        bool f_1 = false;
        /// <summary>
        /// 指示 是否必须强制修改密码
        /// </summary>
        bool f_2 = false;
        #endregion


        #region 加载
        public 修改密码界面()
        {
            InitializeComponent();
        }

        public 修改密码界面(string ss)
        {
            InitializeComponent();
            s = ss;
            label6.Text = s;
            f_2 = true;
        }
        private void 修改密码界面_Load(object sender, EventArgs e)
        {
            try
            {
                fun_load();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 函数

        private void fun_load()
        {
            string sql = string.Format("select * from 人事基础员工表 where 员工号='{0}'", CPublic.Var.LocalUserID);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dtM = new DataTable();

                da.Fill(dtM);

                if (dtM.Rows.Count == 1)
                {

                    textBox5.Text = dtM.Rows[0]["员工号"].ToString().Trim();
                    textBox1.Text = dtM.Rows[0]["姓名"].ToString().Trim();
                    textBox2.Text = dtM.Rows[0]["PWD"].ToString().Trim();

                }
                else
                {

                    MessageBox.Show("请联系信息部，数据有问题");
                }
            }

        }

        private void fun_check()
        {
            if (textBox3.Text == "")
            {
                throw new Exception("请输入新密码");
            }
            if (textBox4.Text == "")
            {
                throw new Exception("请确认新密码");
            }
            if (!isPasswd(textBox3.Text))
            {
                throw new Exception("新密码内含有不允许的字符");
            }

            if (textBox3.Text.Trim() != textBox4.Text.Trim())
            {
                throw new Exception("两次输入密码不一致，请确认");
            }

            if (textBox3.Text.Trim().Length < 4)
            {
                throw new Exception("新密码长度不可小于4位");
            }
            if (textBox2.Text.ToString().Trim() == textBox3.Text.Trim())
            {
                throw new Exception("新密码与旧密码相同");
            }

        }
        ////判断输入是否为中文 或者 ，"
        //public static bool HasChinese(string content)
        //{
        //    //判断是不是中文 
        //    string regexstr = "[\u4e00-\u9fa5]|\'|\"";
        //    if (Regex.IsMatch(content, regexstr))
        //    { return true; }
        //    else { return false; }
        //}

        public static bool isPasswd(string content)
        {
            string regexstr = "^\\w{3,11}$";
            object s= Regex.Matches(content,regexstr);
            if (Regex.IsMatch(content, regexstr))
            { return true; }
            else { return false; }
        }

        private void fun_save()
        {
            string sql = "select * from 人事基础员工表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                new SqlCommandBuilder(da);


                dtM.Rows[0]["PWD"] = textBox3.Text;
                da.Update(dtM);
            }
        }
        #endregion




        #region 界面操作
        //关闭
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (f_2 && f_1 == false)
                {
                    throw new Exception("未修改密码,请配合。");
                }
                CPublic.UIcontrol.ClosePage();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_check();
                if (MessageBox.Show("确认修改密码？", "确认", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    fun_save();

                    MessageBox.Show("修改完成");

                    f_1 = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        //try
        //        {
        //            if (f_2 && f_1 == false)
        //            {
        //                throw new Exception("所答非所问多福多寿过分");
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //            MessageBox.Show(ex.Message);
        //        }


    }
}
