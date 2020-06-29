using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IAACA
{
    public partial class Form1 : Form
    {
        string strconn = CPublic.Var.strConn;
        bool bl_原因 = false;

        public Decimal de_单价 = -1;
        public string s_原因 = "";
        public string s_部门 = "";


        public bool flag = false;  //指示是否保存

        public Form1()
        {
            InitializeComponent();
        }
        public Form1(DataRow dr)
        {
            InitializeComponent();

            dataBindHelper1.DataFormDR(dr);
            searchLookUpEdit1.Visible = false;
            label6.Visible = false;
            searchLookUpEdit2.Visible = false;
            label7.Visible = false;
            //bl_原因 = true;
        }


        public Form1(DataRow dr, string str_原因, string depm)
        {
            InitializeComponent();

            string s = @"select  属性值 as 原因分类,属性字段1 as 说明 from  基础数据基础属性表 
            where 属性类别 = '原因分类'";
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            searchLookUpEdit1.Properties.DataSource = t;
            searchLookUpEdit1.Properties.DisplayMember = "原因分类";
            searchLookUpEdit1.Properties.ValueMember = "原因分类";

            s = @"select  部门编号,部门名称  from 人事基础部门表 ";
            t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            searchLookUpEdit2.Properties.DataSource = t;
            searchLookUpEdit2.Properties.DisplayMember = "部门名称";
            searchLookUpEdit2.Properties.ValueMember = "部门名称";

            dataBindHelper1.DataFormDR(dr);
            searchLookUpEdit1.EditValue = s_原因 = str_原因;
            searchLookUpEdit2.EditValue=s_部门 = depm;
            searchLookUpEdit1.Visible = true;
            label6.Visible = true;
            searchLookUpEdit2.Visible = true;
            label7.Visible = true;
            //label5.Visible = false;
            //textBox5.Visible = false;


            bl_原因 = true;
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_check();
                flag = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_check()
        {

            if (bl_原因)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                    throw new Exception("原因分类不可为空");
                s_原因 = searchLookUpEdit1.EditValue.ToString();

            }
            if (textBox5.Text.ToString().Trim() != "" && !decimal.TryParse(textBox5.Text.ToString().Trim(), out de_单价))
            {
                throw new Exception("输入单价有误");
            }
            if (searchLookUpEdit2.EditValue != null && searchLookUpEdit2.EditValue.ToString() != "")
                s_部门 = searchLookUpEdit2.EditValue.ToString();

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
    }
}
