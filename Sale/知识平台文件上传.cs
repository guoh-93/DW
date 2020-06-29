using CZMaster;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ERPSale
{
    public partial class 知识平台文件上传 : Form
    {
        string a_参;
        public 知识平台文件上传()
        {
            InitializeComponent();
        }
        public 知识平台文件上传(string a_售后单号)
        {
            InitializeComponent();
            a_参 = a_售后单号;
        }
        public 知识平台文件上传(DataRow dr)
        {
            InitializeComponent();
            a_参 = dr["售后单号"].ToString();
            barLargeButtonItem7.Enabled = false;
            barLargeButtonItem2.Enabled = false;
            barLargeButtonItem3.Enabled = false;
            barLargeButtonItem4.Enabled = false;
        }
        //上传
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
             try
            {
              
                    DataRow r = (this.BindingContext[dt1].Current as DataRowView).Row;
                

                    if (r["文件类型"].ToString() == "")
                    {
                        throw new Exception("文件类型为空，无法上传，请检查！");
                    }
                   
                    OpenFileDialog open = new OpenFileDialog();
                    if (open.ShowDialog() == DialogResult.OK)
                    {
                        fun_文件上传(open.FileName, r);
                        fun_文件初始();
                     }
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                fun_文件初始();

            }
        }
        //加载
        DataTable dt_属性;
        private void 知识平台文件上传_Load(object sender, EventArgs e)
        {
            fun_文件初始();
            string sql = "select 属性值 from 基础数据基础属性表 where 属性类别 ='知识平台文件'";
            dt_属性 = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
            repositoryItemSearchLookUpEdit1.DataSource = dt_属性;
            repositoryItemSearchLookUpEdit1.DisplayMember = "属性值";
            repositoryItemSearchLookUpEdit1.ValueMember = "属性值";




        }
        DataTable dt1;
        private void fun_文件初始()
        {
            try
            {

                string sql1 = string.Format("select * from 知识平台文件上传表 where 售后单号='{0}'", a_参);
                dt1 = MasterSQL.Get_DataTable(sql1, CPublic.Var.strConn);
                gcM1.DataSource = dt1;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_文件初始");
                throw new Exception(ex.Message);
            }

        }
        string strConn_FS = CPublic.Var.geConn("FS");
        private void fun_文件上传(string pathName, DataRow r)
        {
            FileInfo info = new FileInfo(pathName);      //判定上传文件的大小
            long maxlength = info.Length;
            if (maxlength > 1024 * 1024 * 8)
            {
                throw new Exception("上传的文件不能超过1M，请重新选择后再上传！");
            }

            MasterFileService.strWSDL = CPublic.Var.strWSConn;
            CFileTransmission.CFileClient.strCONN = strConn_FS;
            string strguid = "";  //记录系统自动返回的GUID

            strguid = CFileTransmission.CFileClient.sendFile(pathName);




            string sql3 = "select * from 知识平台文件上传表 where 1<>1";
            DataTable dt3 = MasterSQL.Get_DataTable(sql3, CPublic.Var.strConn);
            DataRow dr3 = dt3.NewRow();
            dr3["售后单号"] = a_参.ToString();
            dr3["文件类型"] = r["文件类型"].ToString();
            dr3["表单名称"] = Path.GetFileName(pathName);
            dr3["文件GUID"] = strguid;
            dt3.Rows.Add(dr3);
            CZMaster.MasterSQL.Save_DataTable(dt3, "知识平台文件上传表", CPublic.Var.strConn);

            MessageBox.Show("上传成功");

            r["表单名称"] = Path.GetFileName(pathName);

        }
        //下载
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (a_参 =="")
                {
                    MessageBox.Show("请查询单号信息，再下载文件！");
                    fun_文件初始();
                }
                else
                {
                    DataRow r = (this.BindingContext[dt1].Current as DataRowView).Row;
                   
                    SaveFileDialog save = new SaveFileDialog();
                    save.FileName = r["表单名称"].ToString();
                    save.Filter = "图片文件(*.jpg,*.gif,*.bmp)|*.jpg;*.gif;*.bmp|文本文件(*.txt)|*.txt|word文件(*.doc,*.docx)|*.doc;*.docx"; //保存类型
                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        fun_文件下载(save.FileName, r);
                        MessageBox.Show("文件下载成功！");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 文件下载的方法
        /// </summary>
        private void fun_文件下载(string pathName, DataRow r)
        {

            CFileTransmission.CFileClient.strCONN = strConn_FS;
            CFileTransmission.CFileClient.Receiver(r["文件GUID"].ToString(), pathName);

        }
        //删除
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (a_参 == "")
                {
                    MessageBox.Show("请查询单号信息，再删除文件！");
                    fun_文件初始();
                    return;
                }

                DataRow r = (this.BindingContext[dt1].Current as DataRowView).Row;

                if (MessageBox.Show(string.Format("你确定要删除\"{1}\"的\"{0}\"文件吗？",a_参, r["文件类型"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    fun_文件删除(r);

                    //fun_保存销售订单代开票();
                    MessageBox.Show("文件删除成功！");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                fun_文件初始();
            }
        }

        /// <summary>
        /// 删除文件的方法
        /// </summary>
        private void fun_文件删除(DataRow r)
        {
            CFileTransmission.CFileClient.strCONN = strConn_FS;
            CFileTransmission.CFileClient.deleteFile(r["文件GUID"].ToString());

            DataRow[] dr = dt1.Select(string.Format("文件GUID='{0}'", r["文件GUID"].ToString()));



            string sql2 = "select * from 知识平台文件上传表 where 文件GUID='" + r["文件GUID"] + "'";
            DataTable dt_主表删除数量 = MasterSQL.Get_DataTable(sql2, CPublic.Var.strConn);
            dr[0].Delete();

            MasterSQL.Save_DataTable(dt1, "知识平台文件上传表", CPublic.Var.strConn);
  


        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_文件初始();
        }
        //预览
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            DataRow drr = gvM1.GetDataRow(gvM1.FocusedRowHandle);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            string strcoo_路径 = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
            string fileName = strcoo_路径 + "\\" + drr["表单名称"].ToString();
            // string strcoo_路径 = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\PDFTMP";
            saveFileDialog.Title = "下载文件";
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*|图片文件|*.bmp;*.jpg;*.jpeg;*.gif;*.png";



            CFileTransmission.CFileClient.strCONN = strConn_FS;
            CFileTransmission.CFileClient.Receiver(drr["文件GUID"].ToString(), fileName);
            //预览
            System.Diagnostics.Process.Start(fileName);  
        }
        //新增
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = dt1.NewRow();
            dt1.Rows.Add(dr);
        }
        //值变化
        private void gvM1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow dr = gvM1.GetDataRow(gvM1.FocusedRowHandle);
            string str = e.Value.ToString();
            DataRow[] drr = dt_属性.Select(string.Format(" 属性值='{0}'", str));
            if (drr != null && drr.Length > 0)
            {
                DataRow row = drr[0];
                dr["文件类型"] = row["属性值"].ToString();
            }
        }


    }
}
