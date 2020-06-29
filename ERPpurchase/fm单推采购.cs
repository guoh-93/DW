using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ERPpurchase
{
    public partial class fm单推采购 : Form
    {
        public bool  bl_save=false;
        public decimal dec_数量 = 0;
        public DateTime time;

        public fm单推采购()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            check();
            dec_数量=Convert.ToDecimal(textBox1.Text);
            time =Convert.ToDateTime(dateEdit1.EditValue);
            bl_save = true;
            this.Close();
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //判断按键是不是要输入的类型。
                if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                    e.Handled = true;

                //小数点的处理。
                if ((int)e.KeyChar == 46)                           //小数点
                {
                    if (textBox1.Text.Length <= 0)
                        e.Handled = true;   //小数点不能在第一位
                    else
                    {
                        float f;
                        float oldf;
                        bool b1 = false, b2 = false;
                        b1 = float.TryParse(textBox1.Text, out oldf);
                        b2 = float.TryParse(textBox1.Text + e.KeyChar.ToString(), out f);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void check()
        {
            if (textBox1.Text.Trim() == "" )
            {
                throw new Exception("数量未输入");
            }
            if(dateEdit1.EditValue==null || dateEdit1.EditValue.ToString()=="")
            {
                throw new Exception("日期未选择");
            }
 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bl_save = false;
            this.Close();
        }
    }
}
