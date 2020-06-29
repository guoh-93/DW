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
    public partial class fm拆分采购需求单 : Form
    {
        public DataRow rr;
        int pos;
        DataRow r_cs;
        public bool bl = false;
        public fm拆分采购需求单(DataRow dr,int max_pos)
        {
            InitializeComponent();
            r_cs = dr;
            pos = max_pos + 1;
        }

        private void fm拆分采购需求单_Load(object sender, EventArgs e)
        {
            textBox1.Text = r_cs["物料编码"].ToString();
            textBox2.Text = r_cs["物料名称"].ToString();
            textBox3.Text = r_cs["规格型号"].ToString();

        }
        private void fun_check()
        {
            if (dateEdit1.EditValue == null) throw new Exception("日期未选择");
            if (textBox4.Text.Trim()  =="") throw new Exception("数量未输入");
            decimal dec = 0;
            if(!decimal.TryParse(textBox4.Text,out dec))
            {
                throw new Exception("数量输入有误");
            }
            if(dec <=0) throw new Exception("数量不可小于0");
            if (dec > (Convert.ToDecimal(r_cs["通知采购数量"])- Convert.ToDecimal(r_cs["已通知采购数量"]))) throw new Exception("数量超出");

        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_check();
                bl = true;
                rr = r_cs.Table.NewRow();
                rr.ItemArray = r_cs.ItemArray;
                rr["需求来料日期"] = Convert.ToDateTime(dateEdit1.EditValue).Date;
                rr["参考数量_h"] = rr["通知采购数量"] =  Convert.ToDecimal(textBox4.Text);
                rr["POS"] =pos;
               // rr["GUID"] =System.Guid.NewGuid ();
                rr["计划单明细号"] = rr["计划单号"].ToString()+"-c-"+pos.ToString("0000");
            
                this.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
         
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
