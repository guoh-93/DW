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
    public partial class fm修改软件费用 : Form
    {
        public bool bl = false;
        public DataRow dr_cs;
   
        public fm修改软件费用()
        {
            InitializeComponent();
        }
        public fm修改软件费用(DataRow dr)
        {
            InitializeComponent();
            dr_cs = dr;
 
        }

        private void fm修改软件费用_Load(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = dr_cs["物料编码"].ToString();
                textBox2.Text = dr_cs["物料名称"].ToString();
                textBox4.Text = dr_cs["规格型号"].ToString();
                textBox3.Text = dr_cs["单价"].ToString();
                textBox5.Text = dr_cs["原因"].ToString();
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
                fun_check();
                dr_cs["单价"] = Convert.ToDecimal(textBox3.Text.Trim());
                dr_cs["原因"] = textBox5.Text.Trim();
                bl = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_check()
        {
            if (Convert.ToDecimal(textBox3.Text.Trim()) < 0)
            {
                throw new Exception("软件费用不可小于0");
            }
        }


        //只允许输入数字和小数点
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;

            //小数点的处理。
            if ((int)e.KeyChar == 12290)                           //小数点
            {
                if (textBox4.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(textBox4.Text, out oldf);
                    b2 = float.TryParse(textBox4.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }
    }
}
