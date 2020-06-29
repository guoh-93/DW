using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using CPublic;
using System.IO;
using System.Reflection;

namespace 郭恒的Demo
{
    public partial class Form1 : Form
    {
        string strconn = CPublic.Var.strConn;
        DataTable dt = new DataTable();
        string fileName = "C:\\Program Files\\测试\\123.xml";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                //string sql = "select 物料编码,[原ERP物料编号] from 基础数据物料信息表";
                //string sql = "select 物料编码,库位号 from 仓库物料表";
                //string sql = "select 物料编码 from 仓库物料数量表";
                //string sql = "select * from 基础数据包装清单表 ";
                //string sql = "select * from 销售订单分析缺料记录表 ";
                string sql = "select 产品编码,子项编码 from 基础数据物料BOM表";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {

                    da.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        //dr["物料编码"] = dr["物料编码"].ToString().Trim();
                        //dr["原ERP物料编号"] = dr["原ERP物料编号"].ToString().Trim();

                        dr["产品编码"] = dr["产品编码"].ToString().Trim();
                        dr["子项编码"] = dr["子项编码"].ToString().Trim();
                    }
                    new SqlCommandBuilder(da);
                    da.Update(dt);
                    MessageBox.Show("ok");

                    //DataTable t = new DataTable();
                    //DataTable dt_bom = new DataTable();
                
                    //foreach (DataRow dr in dt.Rows)
                    //{
                    //    t = ERPorg.Corg.billofM(t, dr["物料编码"].ToString(), true,dt_bom);
                    //}
                    //string s = "物料编码 in (";
                    //foreach(DataRow dr in  t.Rows)
                    //{
                    //    s += "'" + dr["子项编码"].ToString() + "',";

                    //}
                    //s = s.Substring(0, s.Length - 1)+")";

                }
            }
            catch (Exception)
            {

                throw;
            }




        }

        private void button2_Click(object sender, EventArgs e)
        {
            dt = new DataTable();
            dt.Columns.Add("物料编码");
            dt.NewRow();
            dt.Rows.Add();
            dt.Rows[0]["物料编码"] = "00291";
            string sql = "select 库存总数 from  仓库物料数量表 where 物料编码='00291'";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {

                da.Fill(dt);
                gridControl1.DataSource = dt;
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {

            string sql = string.Format("select [规格型号],物料编码,大类,小类 from 基础数据物料信息表 where 物料编码='{0}'", textBox1.Text);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                da.Fill(dt);
            }
            gridControl1.DataSource = dt;
            textBox1.Text = "";
            textBox1.Focus();
        }


        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {

            //if (e.KeyValue == 13)
            //{
            //    button3_Click(null, null);
            //}
        }
        //分页加载
        private void button4_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            string sql_1 = "select COUNT (@@ROWCOUNT) from 基础数据物料BOM表 ";
            int count = Convert.ToInt32(CZMaster.MasterSQL.Get_DataTable(sql_1, strconn).Rows[0][0]);
            for (int i = 0; i * 5000 < count; i++)
            {
                string sql = string.Format("select top 5000 * from 基础数据物料BOM表 where 产品编码 not in (select top {0}产品编码 from 基础数据物料BOM表 )", 5000 * i);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    da.Fill(dt);
                    gridControl1.DataSource = dt;

                }
            }



            int a = dt.Rows.Count;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            //string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            //string Path_xtzh = DesktopPath + @"\形态转换明细.xlsx";
            //string Path_新老 = DesktopPath + @"\新老编码库存调整.xlsx";

            //if (!File.Exists(Path_xtzh))
            //{
            //    File.Create(Path_xtzh);
            //}

            string strcon_FS = CPublic.Var.geConn("FS");
            string   s = "select  * from [FCS1] ";
            DataRow dr = CZMaster.MasterSQL.Get_DataRow(s,strcon_FS);
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "(*.jpg,*.png,*.jpeg,*.bmp,*.gif)|*.jgp;*.png;*.jpeg;*.bmp;*.gif|All files(*.*)|*.*";
            save.FileName = "xxx";
            //save.FileName = drm["文件名"].ToString();

            if (save.ShowDialog() == DialogResult.OK)
            {
                //CFileTransmission.CFileClient.strCONN = strcon_FS;
               CFileTransmission.CFileClient.Receiver(dr["iGUID"].ToString(), save.FileName);
                //MessageBox.Show("文件下载成功！");

                File.WriteAllBytes(save.FileName, (byte[])dr["文件数据"]);
            }


        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //gridControl1.MainView.SaveLayoutToXml(fileName);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gridControl1.ForceInitialize();

            //gridControl1.MainView.RestoreLayoutFromXml(fileName);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ////string sql = "select * from 生产记录生产工单表";
            //string sql = "select 生产工单号 from 生产记录生产工单表";

            //using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            //{
            //    dt = new DataTable();
            //    da.Fill(dt);
            //    gridControl1.DataSource = dt;
            //    dt.Columns.Add("选择",typeof(bool));
            //}
            //searchLookUpEdit1.Properties.DataSource = dt;
            //searchLookUpEdit1.Properties.DisplayMember = "生产工单号";
            //searchLookUpEdit1.Properties.ValueMember = "生产工单号";
            //string s = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, @"ERPproduct.dll"));
            Type outerForm = outerAsm.GetType("ERPproduct.frm报工系统", false);
            Form ui = Activator.CreateInstance(outerForm) as Form;
            ui.Show();

        }
        private void CreateForm(string stringFormName, string path)
        {
            string strName = stringFormName;
            string assemblyPath = path;
            string s = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            Form form = (Form)Assembly.LoadFrom(assemblyPath).CreateInstance(strName);
            form.Show();
        }


        private void button7_Click(object sender, EventArgs e)
        {
            SqlConnection coon = new SqlConnection(strconn);
            SqlCommand cmd = coon.CreateCommand();

            coon.Open();
            SqlDataReader sdr;
            cmd.CommandText = string.Format(@"select  * from 生产记录生产工单待领料明细表 where 待领料单号='DL20161102577'");
            sdr = cmd.ExecuteReader();
            sdr.Read();

            textBox1.Text = sdr["物料编码"].ToString();


        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
            {
                textBox1.Text = searchLookUpEdit1.EditValue.ToString();

            }
        }

        //    private string systempath = "C:\\Program Files\\";
        //    /// <summary>
        //    /// 保存风格
        //    /// </summary>
        //    /// <param name="moduleid"></param>
        //    /// <param name="saveType"></param>
        //    private void SaveLayout(string moduleid, string saveType)
        //    {
        //        string path = systempath + "窗体风格\\" + moduleid + "\\";
        //        string file = saveType + "view.xml";
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }
        //        gridView1.SaveLayoutToXml(path + file);
        //    }
        //    /// <summary>
        //    /// 加载风格
        //    /// </summary>
        //    /// <param name="moduleid"></param>
        //    /// <param name="saveType"></param>
        //    private void LoadLayout(string moduleid, string saveType)
        //    {
        //        string path = systempath + "窗体风格\\" + moduleid + "\\";
        //        string file = saveType + "view.xml";
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }
        //        if (!File.Exists(path + file))
        //            return;
        //        gridView1.RestoreLayoutFromXml(path + file);

        //    }

        //    private void gridView1_Layout(object sender, EventArgs e)
        //    {
        //        SaveLayout("测试","123");
        //    }

    }
}
