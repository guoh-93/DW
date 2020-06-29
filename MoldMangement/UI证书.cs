using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace MoldMangement
{
    public partial class UI证书 : UserControl
    {
        string strConn = CPublic.Var.strConn;
        DataTable dt = new DataTable();
        DataRow dr;
        DataRow r;
        string pathName = Path.GetTempFileName();

        public UI证书()
        {
            InitializeComponent();
        }

        private void UI证书_Load(object sender, EventArgs e)
        {

        }

        private void fun_load()
        {
            string sql = "select * from 计量器具检定证书表";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
            {
                dt = new DataTable();
                da.Fill(dt);
            }
            gc1.DataSource = dt;

        }

        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
        }
        //上传
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            fun_上传文件(pathName, r);
        }
        //下载
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_下载(pathName, r);
        }
        //预览
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
        //新增
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
        //保存
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
        //关闭
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        string strygh;
        private void fun_上传文件(string pathName, DataRow r)
        {
            FileInfo info = new FileInfo(pathName);      //判定上传文件的大小
            long maxlength = info.Length;
            if (maxlength > 1024 * 1024 * 8)
            {
                throw new Exception("上传的文件不能超过1M，请重新选择后再上传！");
            }

           // CZMaster.MasterFileService.strWSDL = CPublic.Var.strWSConn;
            CFileTransmission.CFileClient.strCONN = strConn;
            string strguid = "";  //记录系统自动返回的GUID

            strguid = CFileTransmission.CFileClient.sendFile(pathName);

            dt.Rows.Add(strygh, r["文件名称"].ToString(), strguid, Path.GetFileName(pathName));
            CZMaster.MasterSQL.Save_DataTable(dt, "计量器具检定证书表", CPublic.Var.strConn);

        }


        private void fun_下载(string pathName, DataRow r)
        {
            CFileTransmission.CFileClient.strCONN = strConn;
            CFileTransmission.CFileClient.Receiver(r["文件GUID"].ToString(), pathName);

        }

        private void fun_删除(DataRow r)
        {
            r["是否已上传"] = false;
            DataRow[] dr = dt.Select(string.Format("计量器具编号='{0}' and 文件名称='{1}'", strygh, r["文件名称"].ToString()));
            CFileTransmission.CFileClient.deleteFile(r["文件GUID"].ToString());
            dr[0].Delete();
            CZMaster.MasterSQL.Save_DataTable(dt, "计量器具检定证书表", CPublic.Var.strConn);

        }

    }
}
