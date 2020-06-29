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
    public partial class 销售相关文件上传 : Form
    {
        DataRow dr_xs;

        public 销售相关文件上传()
        {
            InitializeComponent();
        }
        public 销售相关文件上传(DataRow dr_x)
        {
            InitializeComponent();
            dr_xs = dr_x;
        }

        private void 销售相关文件上传_Load(object sender, EventArgs e)
        {
          
            fun_文件初始();
            //判断是否为已审核单，已审核单只可预览
            if(dr_xs["审核"].Equals(true))
            {
                barLargeButtonItem6.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem7.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
           
        }
        //上传
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                if (dt1.Rows.Count > 0)
                {
                    //throw new Exception("文件名称为空，无法上传，请检查！");

                    DataRow r = (this.BindingContext[dt1].Current as DataRowView).Row;



                    //if (r["是否已上传"].Equals(true))
                    //{
                    //    throw new Exception("该文件已存在，如需上传，请先删除！");
                    //}
                    OpenFileDialog open = new OpenFileDialog();
                    if (open.ShowDialog() == DialogResult.OK)
                    {
                        fun_文件上传(open.FileName, r);
                        fun_文件初始();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                fun_文件初始();

            }
        }
        DataTable dt1;
       // CurrencyManager cmM;
         DataTable dt_属性;
        private void fun_文件初始()
        {
            try
            {
                string sql = "select 属性值 from 基础数据基础属性表 where 属性类别 ='销售相关文件'";
                dt_属性 = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                repositoryItemSearchLookUpEdit2.DataSource = dt_属性;
                repositoryItemSearchLookUpEdit2.DisplayMember = "属性值";
                repositoryItemSearchLookUpEdit2.ValueMember = "属性值";

                string sql1 = string.Format("select * from 销售相关文件表 where 销售开票通知单号='{0}'", dr_xs["销售开票通知单号"]);
                dt1 = MasterSQL.Get_DataTable(sql1, CPublic.Var.strConn);
                //cmM = this.BindingContext[dt1] as CurrencyManager;
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




                       string sql3 = "select * from 销售相关文件表 where 1<>1";
                       DataTable dt3 = MasterSQL.Get_DataTable(sql3, CPublic.Var.strConn);
                        DataRow dr3 = dt3.NewRow();
                        dr3["销售开票通知单号"] = dr_xs["销售开票通知单号"].ToString();
                        dr3["文件名称"] = r["文件名称"].ToString();
                     dr3["上传文件全名"] = Path.GetFileName(pathName);
                      dr3["文件GUID"] = strguid;
                      dt3.Rows.Add(dr3);
                      CZMaster.MasterSQL.Save_DataTable(dt3, "销售相关文件表", CPublic.Var.strConn);
         
                  MessageBox.Show("上传成功");
                
                
               r["上传文件全名"] = Path.GetFileName(pathName);
           
        }

       



        //新增
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(dr_xs != null)
            {
               //cmM.AddNew();
                DataRow dr = dt1.NewRow();
                dr["销售开票通知单号"] = dr_xs["销售开票通知单号"].ToString();
                dt1.Rows.Add(dr);


               //if (dr.RowState == DataRowState.Added)
               //{


                   //foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gvM1.Columns)
                   //{
                   //    if (dc.FieldName == "数量" || dc.FieldName == "文件名称")
                   //    {
                   //        dc.OptionsColumn.AllowEdit = true;
                   //    }
                   //    else
                   //    {
                   //        dc.OptionsColumn.AllowEdit = false;
                   //    }
                   //}
               //}
            





            }
        }

        //下载
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if(dt1.Rows.Count >0)
                {
                if (dr_xs == null)
                {
                    MessageBox.Show("请查询相关员工信息，再下载文件！");
                    fun_文件初始();
                }
                else
                {
                    DataRow r = (this.BindingContext[dt1].Current as DataRowView).Row;
                    //if (!r["是否已上传"].Equals(true))
                    //{
                    //    throw new Exception(string.Format("文件\"{0}\"不存在，无法下载！", r["文件名称"].ToString()));
                    //}
                    //else
                    //{
                        SaveFileDialog save = new SaveFileDialog();
                        save.FileName = r["上传文件全名"].ToString();
                        save.Filter = "图片文件(*.jpg,*.gif,*.bmp)|*.jpg;*.gif;*.bmp|文本文件(*.txt)|*.txt|word文件(*.doc,*.docx)|*.doc;*.docx"; //保存类型
                        if (save.ShowDialog() == DialogResult.OK)
                        {
                            fun_文件下载(save.FileName, r);
                            MessageBox.Show("文件下载成功！");
                        }
                   
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

        
        //刷新
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_文件初始();
            //fun_刷新();
        }
     
    
        
       
        //删除
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (dr_xs == null)
                {
                    MessageBox.Show("请查询相关销售单信息，再删除文件！");
                    fun_文件初始();
                    return;
                }
                if(dt1.Rows.Count > 0)
                {
                DataRow r = (this.BindingContext[dt1].Current as DataRowView).Row;
               
                if (MessageBox.Show(string.Format("你确定要删除\"{1}\"的\"{0}\"文件吗？", dr_xs["销售开票通知单号"].ToString(), r["文件名称"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            fun_文件删除(r);
                            
                            //fun_保存销售订单代开票();
                            MessageBox.Show("文件删除成功！");
                            fun_文件初始();
                        }
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



            string sql2 = "select * from 销售相关文件表 where 销售开票通知单号='" + dr_xs["销售开票通知单号"] + "' and 文件名称='" + r["文件名称"] + "'";
            DataTable dt_主表删除数量 = MasterSQL.Get_DataTable(sql2, CPublic.Var.strConn);
            if (dr.Length > 0)
            {
                dr[0].Delete();
            }
            else
            {
                fun_文件初始();
            }
            
             MasterSQL.Save_DataTable(dt1, "销售相关文件表", CPublic.Var.strConn);
                //MasterSQL.Save_DataTable(dt_主表删除数量, "销售相关文件主表", CPublic.Var.strConn);
                //MasterSQL.Save_DataTable(dt_销售代开票状态, "销售记录销售订单明细表", CPublic.Var.strConn);
               //un_事务(dt1,dt_主表删除数量,dt_销售代开票状态);




            }
         
        //private void fun_事务(DataTable dt_子,DataTable dt_主,DataTable dt_销售代开票状态)
        //{
        //    SqlConnection conn = new SqlConnection(CPublic.Var.strConn);
        //    conn.Open();
        //    SqlTransaction ts = conn.BeginTransaction("保存，删除");

        //    string sql1 = "select * from 销售相关文件子表 where 1<>1";
        //    SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
        //    SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
        //    new SqlCommandBuilder(da1);

        //    string sql2 = "select * from 销售相关文件主表  where 1<>1";
        //    SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);
        //    SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
        //    new SqlCommandBuilder(da2);

        //    string sql3 = "select * from 销售记录销售订单明细表  where 1<>1";
        //    SqlCommand cmd3 = new SqlCommand(sql3, conn, ts);
        //    SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
        //    new SqlCommandBuilder(da3);

        //    try
        //    {
        //        da1.Update(dt_子);
        //        da2.Update(dt_主);
        //        da3.Update(dt_销售代开票状态);

        //        ts.Commit();
        //        //MessageBox.Show("保存成功");

        //    }
        //    catch (Exception ex)
        //    {
        //        ts.Rollback();
        //        MessageBox.Show(ex.Message);

        //    }
        
        //}
      
        //关闭
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
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
                dr["文件名称"] = row["属性值"].ToString();
            }
        }
        //预览
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(dt1.Rows.Count > 0)
            {
           DataRow drr = gvM1.GetDataRow(gvM1.FocusedRowHandle);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            string strcoo_路径 = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
            string fileName = strcoo_路径 + "\\" + drr["上传文件全名"].ToString();
           // string strcoo_路径 = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\PDFTMP";
            saveFileDialog.Title = "下载文件";
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*|图片文件|*.bmp;*.jpg;*.jpeg;*.gif;*.png";


            
            CFileTransmission.CFileClient.strCONN = strConn_FS;
            CFileTransmission.CFileClient.Receiver(drr["文件GUID"].ToString(), fileName);
            //预览
            System.Diagnostics.Process.Start(fileName);  
            }
        }
        //手动
        //private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    try
        //    {
        //         MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
        //        DialogResult a = MessageBox.Show("你确定要完成安装报告已吗?", "保存系统", messButton);

        //        if (a == DialogResult.OK)
        //        {
        //            string sql1 = string.Format("select * from 销售相关文件主表 where 销售订单明细号='{0}'and 类型='{1}'", dr_xs["销售订单明细号"], "安装确认报告");
        //            DataTable dt_安装确认报告 = MasterSQL.Get_DataTable(sql1, CPublic.Var.strConn);
        //            dt_安装确认报告.Rows[0]["状态"] = true;
        //            MasterSQL.Save_DataTable(dt_安装确认报告, "销售相关文件主表", CPublic.Var.strConn);
        //            fun_完成待开票();
        //            MessageBox.Show("安装报告已确认");
        //        }
        //    }
        //    catch
        //    { }

        //}

 }
}
