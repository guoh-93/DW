using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 郭恒的DEMO
{
    public partial class 校验码2 : Form
    {
        public 校验码2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_check();

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {

                    DataTable dtM = new DataTable();
                    dtM.Columns.Add("码");
                    dtM.Columns.Add("箱号");

                    int count = Convert.ToInt32(textBox2.Text);
                    string  str = textBox1.Text.Trim().Substring(0, textBox1.Text.Trim().Length - 7);
                    int beginN = Convert.ToInt32(textBox1.Text.Substring(textBox1.Text.Trim().Length - 7, 7));
                    string pch = textBox3.Text.Trim() ;
                    int xc = 1;
                    for (int i = 1; i <= count; i++)
                    {
                        string s = str + Convert.ToInt32(beginN++).ToString().PadLeft(7, '0');
                        string j = ERPorg.Corg.fun_gccode(s);
                        s = s + j;
                        DataRow dr = dtM.NewRow();
                        dr["码"] = s.ToString();

                        dr["箱号"] = pch + xc.ToString("0000") ;
                        if (i % 60 == 0)
                        {
                            xc++;
                        }

                        dtM.Rows.Add(dr);

                    }
                    button1.Text = "请稍等";
                    button1.Enabled = false;

                    ItemInspection.print_FMS.fun_生成校验码(dtM, saveFileDialog.FileName);
                    button1.Text = "生成excel";
                    button1.Enabled = true;
                    MessageBox.Show(this, "ok");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void fun_check()
        {
            if (textBox1.Text.Trim() == "")
            {
                throw new Exception("起始码未填");
            }
             
        }
    }
}
