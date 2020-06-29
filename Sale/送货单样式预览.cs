using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPSale
{
    public partial class 送货单样式预览 : Form
    {
        public static string dd;
        public 送货单样式预览()
        {
            InitializeComponent();
        }

        //加载
        private void 送货单样式预览_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable dtM = new DataTable();
                string sql = "select * from 基础记录打印模板表 where 模板类型 ='送货单样式'";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
                {
                    da.Fill(dtM);
                }

                //DataRow dr = dtM.NewRow();
                //dtM.Rows.Add(dr);
                gcM1.DataSource = dtM;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }


        //预览
        string strConn_FS = CPublic.Var.geConn("FS");
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


            try
            {
                DataRow drr = gvM1.GetDataRow(gvM1.FocusedRowHandle);
                if (drr["模板名"].ToString() == "")
                {
                    throw new Exception("请先选择模板名或到模板维护界面上传文件再预览！");
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                string strcoo_路径 = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                string fileName = strcoo_路径 + "\\" + drr["模板名"].ToString();
                // string strcoo_路径 = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\PDFTMP";
                saveFileDialog.Title = "下载文件";
                saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*|图片文件|*.bmp;*.jpg;*.jpeg;*.gif;*.png";



                CFileTransmission.CFileClient.strCONN = strConn_FS;
                CFileTransmission.CFileClient.Receiver(drr["文件GUID"].ToString(), fileName);
                //预览
                System.Diagnostics.Process.Start(fileName);
                dd = drr["模板名"].ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
