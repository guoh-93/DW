using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace BaseData
{
    public partial class frm基础数据物料类型维护 : UserControl
    {

        #region 公有成员
        #endregion

        #region 私有成员
        /// <summary>
        /// 主表
        /// 用来操作的数据主表
        /// </summary>
         DataTable dtM;

        /// <summary>
        /// 主表正在编辑的当前行
        /// </summary>
       // DataRow drM;

        /// <summary>
        /// 子表
        /// </summary>
        //DataTable dtP;

        /// <summary>
        /// 出错原因描述
        /// </summary>
        string strErrMsg = "";
        #endregion

        #region 类加载

        public frm基础数据物料类型维护()
        {
            InitializeComponent();
        }

        private void frm基础数据物料类型维护_Load(object sender, EventArgs e)
        {


        }

        #endregion

        #region 其它数据处理

        /// <summary>
        /// 窗体初始化
        /// </summary>
        void iniT()
        {
            dtt1.EditValue = System.DateTime.Today;
            dtt2.EditValue = System.DateTime.Today.AddDays(1).AddSeconds(-1);

            fun_Load();

        }

        void checkData()
        {
            if (textBox1.Text.Trim() == "")
            {
                throw new Exception ( "名字没有填写");
            }
        }

        Boolean CheckData()
        {
            if (textBox1.Text.Trim() == "")
            {
                strErrMsg = "名字没有填写";
                return false;
            }
            if (textBox2.Text.Trim() == "")
            {
                strErrMsg = "地址没有填写";
                return false;
            }
            return true;
        }

        string CheckData1()
        {
            if (textBox1.Text.Trim() == "")
            {
                return strErrMsg = "名字没有填写";
                //return false;
            }
            if (textBox2.Text.Trim() == "")
            {
                return strErrMsg = "地址没有填写";
                //return false;
            }
            return "";
        }
        #endregion

        #region 数据库读取和保存

        void fun_Load()
        {
        }

        void fun_Save()
        {

        }
        #endregion

        #region 界面操作
        /// <summary>
        /// 清空界面函数
        /// </summary>
        /// <param name="ID">要求清空的ID号</param>
        /// <returns>执行结果，0：成功，1，不成功</returns>
        int fun_清空界面(string ID)
        {
            return 0;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //DateTime d1 = (DateTime)dtt1.EditValue;
            //DateTime d2 = (DateTime)dtt2.EditValue;
            //string sql = string.Format("日期 > '{0}' and 日期 < '{1}'", d1.ToString("yyyy-MM-dd HH:mm:ss"), d2.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow r = (this.BindingContext[dtM].Current as DataRowView).Row;
                if (MessageBox.Show(string.Format("你确定要删除{0}吗", r[""].ToString()), "询问", MessageBoxButtons.YesNo) == DialogResult.OK)
                {
                    r.Delete();
                }
            }
            catch
            {
                MessageBox.Show("");
            }
        }


        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //int i = 0;
            //try
            //{
            //    i = int.Parse(textBox1.Text);
            //}
            //catch { }


            try
            {
                //////////////////////////////////////////////////////////
                //////1
                /////////////////////////////////////////////////////////
                if (CheckData())
                {
                    fun_Save();
                    MessageBox.Show("OK");
                }
                else
                {
                    MessageBox.Show(strErrMsg);
                }
                //////////////////////////////////////////////////////////
                //////2
                /////////////////////////////////////////////////////////
                string str = CheckData1();
                if (str == "")
                {
                    fun_Save();
                    MessageBox.Show("OK");
                }
                else
                {
                    MessageBox.Show(strErrMsg);
                }

                //////////////////////////////////////////////////////////
                //////3
                /////////////////////////////////////////////////////////
                checkData();
                fun_Save();
                MessageBox.Show("OK");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion


    }
}
