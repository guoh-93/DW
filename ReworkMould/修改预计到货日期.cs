using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ReworkMould
{
    public partial class 修改预计到货日期 : Form
    {
        DataRow dr_xg;
        public bool bl = false;
        public DataRow rr;
        public 修改预计到货日期(DataRow dr)
        {
            InitializeComponent();
            dr_xg = dr;
        }

        private void 修改预计到货日期_Load(object sender, EventArgs e)
        {
            textBox1.Text = dr_xg["物料编码"].ToString();
            textBox2.Text = dr_xg["物料名称"].ToString();
            textBox3.Text = dr_xg["规格型号"].ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_check();
                bl = true;
                rr = dr_xg.Table.NewRow();
                rr.ItemArray = dr_xg.ItemArray;
                rr["预计来料日期"] = Convert.ToDateTime(dateEdit1.EditValue).Date;
                rr["采购备注"] =  textBox4.Text;             
                this.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_check()
        {
            if (textBox4.Text == "")
            {
                throw new Exception("采购备注必填");
            }
            if (dateEdit1.EditValue == null || dateEdit1.EditValue.ToString() == "")
            {
                throw new Exception("请选择预计来料日期");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
