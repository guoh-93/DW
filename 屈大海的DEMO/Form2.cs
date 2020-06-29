using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 屈大海的DEMO
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            PLCView.frm6WMain fm = new PLCView.frm6WMain();


            this.Controls.Add(fm);

            fm.Dock = DockStyle.Fill;


        }
    }
}
