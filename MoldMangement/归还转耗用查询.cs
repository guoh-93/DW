using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace MoldMangement
{
    public partial class 归还转耗用查询 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        DataTable dt_归还主;
        DataTable dt_归还清单;
        DataRow dr_申请主;
        public 归还转耗用查询()
        {
            InitializeComponent();
        }
        public 归还转耗用查询(DataTable dt_ghz, DataTable dt)
        {
            InitializeComponent();
           
            dt_归还主 = dt_ghz;
            dt_归还清单 = dt;
        }
        private void 归还转耗用查询_Load(object sender, EventArgs e)
        {
            try
            {
                if (dt_归还主.Rows.Count>0)
                {
                    string sql = string.Format("select * from 借还申请表 where 申请批号 = '{0}'",dt_归还主.Rows[0]["申请批号"]);
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    textBox1.Text = dt_归还主.Rows[0]["申请批号"].ToString();
                    textBox5.Text = dt.Rows[0]["申请日期"].ToString();
                    textBox9.Text = dt.Rows[0]["借用人员"].ToString();
                    textBox3.Text = dt.Rows[0]["借用类型"].ToString();
                    textBox6.Text = dt_归还主.Rows[0]["备注"].ToString();
                    textBox2.Text = dt.Rows[0]["申请人"].ToString();
                    textBox7.Text = dt.Rows[0]["目标客户"].ToString();
                    textBox8.Text = dt.Rows[0]["相关单位"].ToString();
                    textBox4.Text = dt_归还主.Rows[0]["原因分类"].ToString();
                    textBox10.Text = dt_归还主.Rows[0]["归还说明"].ToString();
                    if (Convert.ToBoolean(dt_归还主.Rows[0]["附件"]))
                    {
                        checkBox1.Checked = true;
                    }
                    if (checkBox1.Checked)
                    {
                        button5.Enabled = true;
                    }
                    else
                    {
                        button5.Enabled = false;
                    }
                    gcP.DataSource = dt_归还清单;
                }
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        string strcon_FS = CPublic.Var.geConn("FS");
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (dt_归还主.Rows.Count == 0)
                {
                    throw new Exception("没有文件可以预览");
                }
                else
                {
                    dr_申请主 = dt_归还主.Rows[0];
                }
                if (dr_申请主["文件GUID"] == null || dr_申请主["文件GUID"].ToString() == "")
                {
                    throw new Exception("没有文件可以预览，请先上传文件");
                }
                //string type = dr["后缀"].ToString();

                string foldPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                //string fileName = foldPath + CPublic.Var.getDatetime().ToString("yyyy-MM-dd") + "T" + CPublic.Var.getDatetime().ToString("HH_mm_ss") + "Z" + "_" + Guid.NewGuid().ToString() + "." + type;
                string fileName = foldPath + dr_申请主["文件"].ToString();

                try
                {
                    System.IO.Directory.Delete(foldPath, true);
                }
                catch (Exception)
                {
                }
                CFileTransmission.CFileClient.strCONN = strcon_FS;
                CFileTransmission.CFileClient.Receiver(dr_申请主["文件GUID"].ToString(), fileName);
                System.Diagnostics.Process.Start(fileName);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
