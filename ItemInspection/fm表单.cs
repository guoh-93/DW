using CZMaster;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace ItemInspection
{
    public partial class fm表单 : Form
    {
        #region 用户变量

        public string strTZJno;
        public string strCPBM;

        private DataTable dtP, dtM;
        private CurrencyManager cmM;

        #endregion 用户变量

        #region 类自用

        public fm表单()
        {
            InitializeComponent();
        }

        private void fm表单_Load(object sender, EventArgs e)
        {
            fun_readData();
            fillCMD();
            strConn_FS = CPublic.Var.geConn("FS");
        }

        #endregion 类自用

        #region 数据库操作

        /// <summary>
        /// 读
        /// </summary>
        private void fun_readData()
        {
            dtP = new DataTable();
            string sql = string.Format("select * FROM 检验上传表单记录表 where [采购入库通知单号] ='{0}' and [产品编号]='{1}'", strTZJno, strCPBM);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                try
                {
                    da.Fill(dtP);
                    fun_Change();
                    gcM.DataSource = dtP;
                    cmM = this.BindingContext[dtP] as CurrencyManager;
                    gvM.ViewCaption = "上传表单数量：" + dtP.Rows.Count.ToString();
                    dtP.TableNewRow += dtP_TableNewRow;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void dtP_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            e.Row["采购入库通知单号"] = strTZJno;
            e.Row["产品编号"] = strCPBM;
            e.Row["已上传"] = false;
        }

        /// <summary>
        /// 下拉填充
        /// </summary>
        private void fillCMD()
        {
            dtM = new DataTable();
            string sql = "SELECT * FROM 表单类型表";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                try
                {
                    da.Fill(dtM);

                    foreach (DataRow r in dtM.Rows)
                    {
                        repositoryItemComboBox1.Items.Add(r["表单类型"]);
                    }
                    repositoryItemComboBox1.NullText = "";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 写
        /// </summary>
        private void fun_saveData()
        {
            string sql = "select * FROM 检验上传表单记录表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                new SqlCommandBuilder(da);
                try
                {    
                    DataRow r1 = gvM.GetDataRow(gvM.FocusedRowHandle);
                    if(r1["表单类型"].ToString() =="")
                    {
                       throw new Exception("请选择表单类型");
                    }
                    if (r1["产品编号"].ToString() == "")
                    {
                        throw new Exception("产品编码没有值，请重新上传");
                    }
                    da.Update(dtP);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion 数据库操作

        #region 数据处理

        private void fun_Change()
        {
            dtP.Columns.Add("已上传", true.GetType());
            foreach (DataRow r in dtP.Rows)
            {
                if (r["GUID"].ToString() != "")
                {
                    r["已上传"] = true;
                }
            }
        }

        /// <summary>
        /// 新增行
        /// </summary>
        private void fun_AddData()
        {
            try
            {
                cmM.AddNew();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void fun_deletefile()
        {
            try
            {
                DataRow rm = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
                string guid = rm["GUID"].ToString();
 
                  CFileTransmission.CFileClient.strCONN = strConn_FS;
                    
                  CFileTransmission.CFileClient.deleteFile(guid);
   
      
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 删除行
        /// </summary>
        private void fun_DeleteData()
        {
            try
            {
                fun_deletefile();
                (cmM.Current as DataRowView).Row.Delete();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        string strConn_FS = "";

        /// <summary>
        /// 上传
        /// </summary>
        private void fun_UpLoad()
        {
            try
            {
                DataRow rm = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
                using (OpenFileDialog op = new OpenFileDialog())
                {
                    if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        CFileTransmission.CFileClient.strCONN = strConn_FS;  //zf20160810
                        string str_guid = CFileTransmission.CFileClient.sendFile(op.FileName);  //zf20160810
                   
                        //string guid = MasterFileService.BOLBUpload(System.IO.File.ReadAllBytes(op.FileName));
                        rm["GUID"] = str_guid;
                        rm["表单名称"] = op.SafeFileName;
                        rm["扩展名"] = op.SafeFileName.Substring(op.SafeFileName.LastIndexOf('.') + 1);
                        rm["已上传"] = true;
                        MessageBox.Show("上传成功！");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 下载
        /// </summary>
        private void fun_DownLoad()
        {
            try
            {
                DataRow rm = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
                string guid = rm["GUID"].ToString();

                //byte[] data = MasterFileService.BOLBDownLoad(guid);

                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "下载目标位置";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string foldPath = dialog.SelectedPath;
                    string fileName = foldPath + "\\" + rm["表单名称"].ToString();// + "." + rm["扩展名"].ToString() //zf

                    CFileTransmission.CFileClient.strCONN = strConn_FS;
                  CFileTransmission.CFileClient.Receiver(guid, fileName);
                    //CFileTransmission.CFileClient.Receiver_p(guid, fileName);
                    MessageBox.Show("下载完成！");
                    //System.IO.Directory.CreateDirectory(foldPath);//zf
                    //if (System.IO.File.Exists(fileName) == true)//zf
                    //{
                    //    if (MessageBox.Show("文件已存在是否覆盖", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)//zf
                    //    {
                    //        System.IO.File.WriteAllBytes(fileName, data);//zf
                    //    }
                    //}
                    //if (System.IO.File.Exists(fileName) == false)//zf
                    //{
                    //    System.IO.File.WriteAllBytes(fileName, data);//zf
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //没用到
        private void fun_DownLoad_ML_WJ()
        {
            try
            {
                string str_ML = "";
                string str_WJ = "";
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "目标位置";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    System.IO.DirectoryInfo TheFolder = new System.IO.DirectoryInfo(dialog.SelectedPath);
                    //遍历文件夹
                    foreach (System.IO.DirectoryInfo NextFolder in TheFolder.GetDirectories())
                        str_ML+=NextFolder.Name+"|";
                    //遍历文件
                    foreach (System.IO.FileInfo NextFile in TheFolder.GetFiles())
                        str_WJ += NextFile.Name + "|";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void fun_Preview()
        {
            try
            {
                DataRow rm = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
                string guid = rm["GUID"].ToString();

                //byte[] data = MasterFileService.BOLBDownLoad(guid);

                string foldPath = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\PDFTMP";
                //string fileName = string.Format("{0}\\{1}.{2}", foldPath, Guid.NewGuid().ToString(), rm["扩展名"].ToString());
                string fileName = foldPath + "\\" + rm["表单名称"].ToString();// + "." + rm["扩展名"].ToString() //zf
                try
                {
                    System.IO.Directory.Delete(foldPath, true);
                }
                catch { }
                try
                {
                    System.IO.Directory.CreateDirectory(foldPath);
                }
                catch { }


                CFileTransmission.CFileClient.strCONN = strConn_FS;
               CFileTransmission.CFileClient.Receiver(guid, fileName);
                //CFileTransmission.CFileClient.Receiver_p(guid, fileName);
                System.Diagnostics.Process.Start(fileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion 数据处理

        #region 界面相关

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_UpLoad();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_DownLoad();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_AddData();
                gvM.CloseEditor();
                cmM.EndCurrentEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                fun_DeleteData();
                gvM.CloseEditor();
                cmM.EndCurrentEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gvM.CloseEditor();
            cmM.EndCurrentEdit();
            try
            {
                fun_saveData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                fun_readData();
                return;
            }
            MessageBox.Show("OK");
        }

        /// <summary>
        /// 预览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_Preview();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion 界面相关

        private void 上传ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                fun_UpLoad();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 下载ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                fun_DownLoad();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                (cmM.Current as DataRowView).Row.Delete();
                gvM.CloseEditor();
                cmM.EndCurrentEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 预览ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                fun_Preview();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 双击预览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvM_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                fun_Preview();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}