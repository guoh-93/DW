using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;

namespace MoldMangement
{
    
    public partial class frm模具信息打印 : UserControl
    {
        DataTable dt;
        string PrinterName = "";
        string strconn = CPublic.Var.strConn;
        public frm模具信息打印()
        {
            InitializeComponent(); 
        }

        private void fun_GetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                da.Fill(dt);
            }
        }

        private void fun_SetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                new SqlCommandBuilder(da);
                da.Update(dt);
            }
        }

        private void frm模具信息打印_Load(object sender, EventArgs e)
        {

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                string s = textBox1.Text.Replace(" ", "");

                if (checkBox3.Checked == true)
                {
                    if (s.Length < 3)
                    {
                        MessageBox.Show("请输入完整的库位");
                        return;
                    }
                }
                if (checkBox3.Checked == false && checkBox2.Checked == false && checkBox1.Checked == false)
                {
                    MessageBox.Show("请输入相关信息");
                    return;
                }
                dt = new DataTable();
                string sql = string.Format(@"select * from 模具管理基础信息表  where 1=1 ");
                if (checkBox2.Checked == true)
                {
                    sql = sql + string.Format(" and 模具编号 = '{0}'", textBox2.Text);
                }
                if (checkBox3.Checked == true)
                {
                   
                   
                        sql = sql + string.Format(" and 存放库位 like '{0}%'", textBox1.Text);
                   
                }

                if (checkBox1.Checked == true)
                {
                     sql=sql+ string.Format(@" and 模具编号 in ( select base.模具编号 from 模具管理基础信息表 base 
                    left  join  ( select  模具编号,图纸编号 from  模具物料信息关联表 a,基础数据物料信息表  b 
                    where  a.物料编码=b.物料编码 ) x on x.模具编号=base.模具编号
                    where 零件图号 like '%{0}%' or 图纸编号 like '%{0}%')", textBox3.Text);
                }
                fun_GetDataTable(dt, sql);
                dt.Columns.Add("选择", typeof(bool));
                gc.DataSource = dt;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                gc.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            if (MessageBox.Show(string.Format("确定打印？"), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                gv.CloseEditor();
                this.BindingContext[dt].EndCurrentEdit();
                try
                {
                    PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                    this.printDialog1.Document = this.printDocument1;
                    DialogResult dr = this.printDialog1.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                        fun_打印();
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void fun_打印()
        {
            Thread thDo;
            thDo = new Thread(Dowork);
            thDo.IsBackground = true;
            thDo.Start();
        }

        private void fun_打印1()
        {
            Thread thDo;
            thDo = new Thread(Dowork1);
            thDo.IsBackground = true;
            thDo.Start();
        }

        public void Dowork()
        {  
            DataView dv = new DataView(dt);
            dv.Sort = "模具编号";
            dv.RowFilter = "选择=1";  
            DataTable dt_dy = dv.ToTable();
            List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
            foreach (DataRow drr in dt_dy.Rows)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("tzxx", drr["零件图号"].ToString());
                dic.Add("hjxx", drr["存放库位"].ToString());
                dic.Add("mjxx", drr["模具编号"].ToString());
                dic.Add("mc", drr["产品型号"].ToString());
                li.Add(dic);
              
            }
            string path = Application.StartupPath + string.Format(@"\Mode\模具货架信息打印.Lab");
              ERPproduct.Lprinter lp = new  ERPproduct.Lprinter(path, li, PrinterName, 1);
            lp.DoWork();
        }
   
        public void Dowork1()
        {
            DataView dv = new DataView(dt);
            dv.Sort = "模具编号";
            dv.RowFilter = "选择=1";
            DataTable dt_dy = dv.ToTable();
            List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
            //两个一组   
            int ys = dt_dy.Rows.Count % 2;
             int c_组数= dt_dy.Rows.Count / 2 + ys;
             for (int i =1; i <= c_组数; i++)
             {
                 Dictionary<string, string> dic = new Dictionary<string, string>();
                
                 dic.Add("tzbh", dt_dy.Rows[2*(i-1)]["零件图号"].ToString());
                 dic.Add("cs", dt_dy.Rows[2*(i-1)]["产品型号"].ToString());
                 dic.Add("ljmc", dt_dy.Rows[2*(i-1)]["零件名称"].ToString());
                 if (ys == 0 || i < c_组数)
                 {
                     dic.Add("tzbh1", dt_dy.Rows[2 * i-1]["零件图号"].ToString());
                     dic.Add("cs1", dt_dy.Rows[2 * i-1]["产品型号"].ToString());
                     dic.Add("ljmc1", dt_dy.Rows[2 * i-1]["零件名称"].ToString());
                 }
                 else
                 {
                     dic.Add("tzbh1", "");
                     dic.Add("cs1", "");
                     dic.Add("ljmc1", "");

                 }
                 li.Add(dic);

             }

                 //foreach (DataRow drr in dt_dy.Rows)
                 //{
                 //    Dictionary<string, string> dic = new Dictionary<string, string>();
                 //    dic.Add("tzbh", drr["零件图号"].ToString());
                 //    dic.Add("cs", drr["产品型号"].ToString());
                 //    dic.Add("ljmc", drr["零件名称"].ToString());
                 //    li.Add(dic);
                 //}
            string path = Application.StartupPath + string.Format(@"\Mode\模具看板信息.Lab");
            ERPproduct.Lprinter lp = new ERPproduct.Lprinter(path, li, PrinterName, 1);
            lp.DoWork();
      
        } 

        private void fun_check_dy()
        {
            if (checkBox3.Checked != true && checkBox2.Checked != true)
            {
                throw new Exception("未选择物料或者货架号不可打印");
            }
            if (dt.Rows.Count > 100)
            {
                throw new Exception("当前打印标贴数大于100");
            }

        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show(string.Format("确定打印？"), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                gv.CloseEditor();
                this.BindingContext[dt].EndCurrentEdit();
                try
                {
                    PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                    this.printDialog1.Document = this.printDocument1;
                    DialogResult dr = this.printDialog1.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                        fun_打印1();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false; checkBox3.Checked = false;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                checkBox2.Checked = false; checkBox1.Checked = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false; checkBox3.Checked = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gv.DataRowCount; i++)
            {
                gv.GetDataRow(i)["选择"] = true;
            }
        }
    }
}
