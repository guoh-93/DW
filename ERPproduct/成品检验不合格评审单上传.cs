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

namespace ERPproduct
{
    public partial class 成品检验不合格评审单上传 : Form
    {
        DataRow dr_成品检验单;
        public 成品检验不合格评审单上传(DataRow dr参)
        {
            InitializeComponent();
            dr_成品检验单 = dr参;
        }

        private void 成品检验不合格评审单上传_Load(object sender, EventArgs e)
        {
            fun_文件初始();
        }
        DataTable dt1;
#pragma warning disable IDE1006 // 命名样式
        private void fun_文件初始()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                string sql1 = string.Format("select * from 成品检不合格评审单上传 where 生产检验单号='{0}'", dr_成品检验单["生产检验单号"]);
                dt1 = MasterSQL.Get_DataTable(sql1, CPublic.Var.strConn);
                gcM1.DataSource = dt1;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_文件初始");
                throw new Exception(ex.Message);
            }

        }






        //刷新
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_文件初始();
        }
        //新增
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = dt1.NewRow();
            dr["文件类型"] = "成品检验不合格品审单";
            dt1.Rows.Add(dr);
        }
        //上传
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
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
        string strConn_FS = CPublic.Var.geConn("FS");
#pragma warning disable IDE1006 // 命名样式
        private void fun_文件上传(string pathName, DataRow r)
#pragma warning restore IDE1006 // 命名样式
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




            string sql3 = "select * from 成品检不合格评审单上传 where 1<>1";
            DataTable dt3 = MasterSQL.Get_DataTable(sql3, CPublic.Var.strConn);
            DataRow dr3 = dt3.NewRow();
            dr3["生产检验单号"] = dr_成品检验单["生产检验单号"].ToString();
            dr3["文件类型"] = r["文件类型"].ToString();
            dr3["录入时间"] = CPublic.Var.getDatetime();
            dr3["表单名称"] = Path.GetFileName(pathName);
            dr3["文件GUID"] = strguid;
            dt3.Rows.Add(dr3);
            CZMaster.MasterSQL.Save_DataTable(dt3, "成品检不合格评审单上传", CPublic.Var.strConn);

            string sff = string.Format("select * from 生产记录生产检验单主表 where 生产检验单号='{0}'", dr_成品检验单["生产检验单号"]);
            DataTable dt_上传表标记 = MasterSQL.Get_DataTable(sff, CPublic.Var.strConn);
            dt_上传表标记.Rows[0]["是否上传品审单"] = 1;
            CZMaster.MasterSQL.Save_DataTable(dt_上传表标记, "生产记录生产检验单主表", CPublic.Var.strConn);

            MessageBox.Show("上传成功");

            r["表单名称"] = Path.GetFileName(pathName);

        }



        //下载
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (dr_成品检验单["生产检验单号"] == "")
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
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 文件下载的方法
        /// </summary>
        private void fun_文件下载(string pathName, DataRow r)
#pragma warning restore IDE1006 // 命名样式
        {

            CFileTransmission.CFileClient.strCONN = strConn_FS;
            CFileTransmission.CFileClient.Receiver(r["文件GUID"].ToString(), pathName);

        }
        //删除
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (dr_成品检验单["生产检验单号"].ToString() == "")
                {
                    MessageBox.Show("请查询单号信息，再删除文件！");
                    fun_文件初始();
                    return;
                }

                DataRow r = (this.BindingContext[dt1].Current as DataRowView).Row;

                if (MessageBox.Show(string.Format("你确定要删除\"{1}\"的\"{0}\"文件吗？", dr_成品检验单["生产检验单号"].ToString(), r["文件类型"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
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

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 删除文件的方法
        /// </summary>
        private void fun_文件删除(DataRow r)
#pragma warning restore IDE1006 // 命名样式
        {
            CFileTransmission.CFileClient.strCONN = strConn_FS;
            CFileTransmission.CFileClient.deleteFile(r["文件GUID"].ToString());

            DataRow[] dr = dt1.Select(string.Format("文件GUID='{0}'", r["文件GUID"].ToString()));



            string sql2 = "select * from 成品检不合格评审单上传 where 文件GUID='" + r["文件GUID"] + "'";
            DataTable dt_主表删除数量 = MasterSQL.Get_DataTable(sql2, CPublic.Var.strConn);
            if (dt_主表删除数量.Rows.Count > 0) dr[0].Delete();
            

            MasterSQL.Save_DataTable(dt1, "成品检不合格评审单上传", CPublic.Var.strConn);
            
            string sff = string.Format("select * from 生产记录生产检验单主表 where 生产检验单号='{0}'", dr_成品检验单["生产检验单号"]);
            DataTable dt_上传表标记 = MasterSQL.Get_DataTable(sff, CPublic.Var.strConn);

            string see =string.Format("select * from 成品检不合格评审单上传 where 生产检验单号='{0}'",dr_成品检验单["生产检验单号"]);
            DataTable dt_判断是否存在表单 = MasterSQL.Get_DataTable(see, CPublic.Var.strConn);
            if (dt_判断是否存在表单.Rows.Count == 0)
            {
            dt_上传表标记.Rows[0]["是否上传品审单"] = 0;
            }
            CZMaster.MasterSQL.Save_DataTable(dt_上传表标记, "生产记录生产检验单主表", CPublic.Var.strConn);


        }
        //预览
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
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
        //关闭
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.Close();
        }



    }
}
