using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DEMO张宇
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            fun_load();
        }



        private void fun_load()
        {
            PLCView.frm6W frm = new PLCView.frm6W();
            frm.MachineName = "HC_FR6W1";
            this.Controls.Add(frm);
        }
    }
}
