using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class Form包装抽检相关文件上传 : Form
    {
        public Form包装抽检相关文件上传()
        {
            InitializeComponent();
        }

        public Form包装抽检相关文件上传(string drM)
        {
            InitializeComponent();

            st_单号 = drM;
        }
        #region 变量
        string strConn = CPublic.Var.strConn;
        DataTable dt = new DataTable();
        DataRow dr;
        string pathName = Path.GetTempFileName();
        Boolean strygh = false;
        string strConn_FS = CPublic.Var.geConn("FS");
        DataRow drM1;//当前行数据
        string st_单号;


















        #endregion

        private void Form包装抽检相关文件上传_Load(object sender, EventArgs e)
        {
            try
            {
                fun_show();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }














        private void fun_show()
        {

            //根据drm1资产编码显示对应的上传内容（固定资产文件上传表）
            // string.Format("机台 = '{0}'", r["机台"])


        
            string sql = string.Format("select * from 包装抽检相关文件上传 where 出库通知单号='{0}'", st_单号);

            using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
            {
                dt = new DataTable();
                //new    SqlCommandBuilder(da);
                da.Fill(dt);
                // da.Update(dt);

                //da.Update();

            }

            gC1.DataSource = dt;



        }//加载

   



        private void fun_上传(string pathName)
        {

            FileInfo info = new FileInfo(pathName);      //判定上传文件的大小
            long maxlength = info.Length;
          

            //CZMaster.MasterFileService.strWSDL = CPublic.Var.strWSConn;
            CFileTransmission.CFileClient.strCONN = strConn_FS;
            string strguid = "";  //记录系统自动返回的GUID

            strguid = CFileTransmission.CFileClient.sendFile(pathName);
            dr = gV1.GetDataRow(gV1.FocusedRowHandle);
            dr["出库通知单号"] = st_单号;
            dr["文件GUID"] = strguid;
            dr["文件名"] = Path.GetFileName(pathName);
            dr["上传时间"] = DateTime.Now.ToString();
            //  dr["已上传"] = strygh;
            //dt.Rows.Add(r["计量器具编号"].ToString(), r["计量器具名称"].ToString(), strguid, r["文件类型"].ToString(), Path.GetFileName(pathName), strygh);
            //DataTable temp = dt.Clone();
            //temp.ImportRow(r);
            gV1.CloseEditor();
            this.BindingContext[dt].EndCurrentEdit();
            CZMaster.MasterSQL.Save_DataTable(dt, "包装抽检相关文件上传", CPublic.Var.strConn);



        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = "D:\\";
                //   openFileDialog1.Filter = "txt files (*.txt)|*.txt|*.jpg|*.png|All files (*.*)|*.*";
          //      openFileDialog1.Filter = "(*.jpg,*.jpeg,*.bmp,*.gif)|*.jgp;*.png;*.jpeg;*.bmp;*.gif|All files(*.*)|*.*";

               openFileDialog1.Filter = "(*.jpg,*.png,*.jpeg,*.bmp,*.gif)|*.jgp;*.png;*.jpeg;*.bmp;*.gif|All files(*.*)|*.*";
                openFileDialog1.Title = "上传文件";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    fun_上传(openFileDialog1.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("上传成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //dr["已上传"] = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
      //          DataRow r = (this.BindingContext[gC1].Current as DataRowView).Row;

                DataRow r = (this.BindingContext[gC1.DataSource].Current as DataRowView).Row;


                SaveFileDialog save = new SaveFileDialog();
                save.FileName = r["文件名"].ToString();
                save.Filter = "图片文件(*.jpg,*.gif,*.bmp)|*.jpg;*.gif;*.bmp|文本文件(*.txt)|*.txt|word文件(*.doc,*.docx)|*.doc;*.docx"; //保存类型
                if (save.ShowDialog() == DialogResult.OK)
                {
                    fun_文件下载(save.FileName, r);
                    MessageBox.Show("文件下载成功！");
                }







                //SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                //saveFileDialog1.InitialDirectory = "c:\\";
                //saveFileDialog1.Filter = "图片文件(*.jpg,*.gif,*.bmp)|*.jpg;*.gif;*.bmp|文本文件(*.txt)|*.txt|word文件(*.doc,*.docx)|*.doc;*.docx"; //保存类型
                //saveFileDialog1.Title = "下载文件";
                //saveFileDialog1.FilterIndex = 2;
                //saveFileDialog1.RestoreDirectory = true;

                //if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                //{
                //    DataRow xr;
                //    xr = gV1.GetDataRow(gV1.FocusedRowHandle);
                //    CFileTransmission.CFileClient.strCONN = strConn_FS;
                //    Receiver(xr["文件GUID"].ToString(), saveFileDialog1.FileName);
                //    DevExpress.XtraEditors.XtraMessageBox.Show("下载成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                //   SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                //   saveFileDialog1.Filter = "txt files (*.txt)|*.txt|*.jpg|*.png|All files (*.*)|*.*";
                //   saveFileDialog1.Title = "下载文件";
                //   saveFileDialog1.FilterIndex = 2;
                //   saveFileDialog1.RestoreDirectory = true;
                //   DataRow yl;
                //   yl = gV1.GetDataRow(gV1.FocusedRowHandle);
                //   CFileTransmission.CFileClient.strCONN = strConn_FS;
                ////   string ss = yl["文件类型"].ToString();
                //   // r["NG数"] = dt_result.Select(string.Format("机台 = '{0} 'and  NG='{1}'", r["机台"], r["NG"])).Length;
                //   // DataRow [] dr  = dt.Select(string.Format("文件类型='{0}'",yl["文件类型"].ToString()));


                //   string pathname = "D:\\下载文件\\预览文件." ;
                //   Receiver(yl["文件GUID"].ToString(), pathname);

                //   System.Diagnostics.Process.Start(pathname);

                DataRow drr = gV1.GetDataRow(gV1.FocusedRowHandle);
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                string strcoo_路径 = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                string fileName = strcoo_路径 + "\\" + drr["文件名"].ToString();
                // string strcoo_路径 = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\PDFTMP";
                saveFileDialog.Title = "下载文件";
                saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*|图片文件|*.bmp;*.jpg;*.jpeg;*.gif;*.png";



                CFileTransmission.CFileClient.strCONN = strConn_FS;
                CFileTransmission.CFileClient.Receiver(drr["文件GUID"].ToString(), fileName);
                //预览
                System.Diagnostics.Process.Start(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }





        private void fun_文件下载(string pathName, DataRow r)
        {


            CFileTransmission.CFileClient.strCONN = strConn_FS;
            CFileTransmission.CFileClient.Receiver(r["文件GUID"].ToString(), pathName);

        }



        public static int msgTimeout = 6000;
        public static int iDalyStep = 500;
        public static int Receiver(string remoteFile, string downFile)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(downFile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(downFile));
                }
                string iGUID = System.Guid.NewGuid().ToString();
                DataTable dt = new DataTable();
                string sql = string.Format("select * from FCS where iGUID = '{0}'", iGUID);
                SqlDataAdapter da = new SqlDataAdapter(sql, CFileTransmission.CFileClient.strCONN);
                new SqlCommandBuilder(da);
                da.Fill(dt);
                DataRow r = dt.Rows.Add(iGUID, "下载", -1, "", System.DateTime.Now, DBNull.Value, remoteFile);
                da.Update(dt);

                sql = string.Format("select * from FCS where iGUID = '{0}' and 请求结果 <> -1", iGUID);
                int iStep = 0;
                try
                {
                    while (iStep <= msgTimeout)
                    {
                        dt.Clear();
                        new SqlDataAdapter(sql, CFileTransmission.CFileClient.strCONN).Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            if ((int)dt.Rows[0]["请求结果"] == 0)
                            {
                                File.WriteAllBytes(downFile, (byte[])dt.Rows[0]["文件数据"]);
                                return (int)dt.Rows[0]["请求结果"];
                            }
                            else
                            {
                                throw new Exception(dt.Rows[0]["errDesc"].ToString());
                            }
                        }
                        System.Threading.Thread.Sleep(iDalyStep);
                        iStep += iDalyStep;
                    }
                    throw new Exception("服务器无响应，超时");
                }
                finally
                {
                    if (dt.Rows.Count > 0)
                    {
                        dt.Rows[0].Delete();
                        da.Update(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                DataRow dr;
                dr = gV1.GetDataRow(gV1.FocusedRowHandle);
                fun_delete(dr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void fun_delete(DataRow dr)
        {

            if (MessageBox.Show("确认删除吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                DataRow[] drM2 = dt.Select(string.Format("出库通知单号='{0}' and 文件名='{1}'", dr["出库通知单号"].ToString(), dr["文件名"].ToString()));
                //CZMaster.MasterFileService.strWSDL = CPublic.Var.strWSConn;
                CFileTransmission.CFileClient.strCONN = strConn_FS;
                CFileTransmission.CFileClient.deleteFile(dr["文件GUID"].ToString());
                drM2[0].Delete();

                using (SqlDataAdapter da = new SqlDataAdapter("select * from 包装抽检相关文件上传 where 1=2", strConn))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt);

                }
                //CZMaster.MasterSQL.Save_DataTable(dt, "固定资产文件上传表", CPublic.Var.strConn);
                MessageBox.Show("删除成功");
            }



        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                gV1.CloseEditor();//关闭编辑状态
                this.BindingContext[dt].EndCurrentEdit();//关闭编辑状态

               


                string sql = "select * from 包装抽检相关文件上传 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt);
                }
                MessageBox.Show("保存成功");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                dr = dt.NewRow();
         
                dt.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
