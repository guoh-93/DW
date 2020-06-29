using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Net;


namespace MoldMangement
{
    public partial class 上传证书 : Form
    {
       //static string strConn = "Password = a; Persist Security Info = True; User ID = sa; Initial Catalog = fms; Data Source = XINREN ";
       string strConn = CPublic.Var.strConn;
        DataTable dt = new DataTable();
        DataRow dr;
        DataRow r;
        DataRow dr_证;
        string pathName = Path.GetTempFileName();
        //string strygh ;
        string strConn_FS = CPublic.Var.geConn("FS");
        Boolean strygh = false;


        public 上传证书()
        {
            InitializeComponent();
        }

        public 上传证书(DataRow dr)
        {
            InitializeComponent();
            dr_证 = dr;
        }

        private void 上传证书_Load(object sender, EventArgs e)
        {
            fun_load();

        }

        private void fun_load()
        {
            string sql = string.Format("select * from 计量器具检定证书表 where 计量器具编号 = '{0}'", dr_证["计量器具编号"].ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
            {
                dt = new DataTable();
                da.Fill(dt);
            }
            gc1.DataSource = dt;
            dt.Columns.Add("已上传", typeof(bool));
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    r["已上传"] = true;
                }
            }
            //DateTime t_当天日期 = DateTime.Now.Date;//为当天日期
            //DateTime t_后一天 = t_当天日期.AddDays(1);  //为后一天日期
            //DateTime t_截止时间 = t_当天日期.AddHours(14).AddMinutes(25);//为当天的截止日
            //DateTime t_当前时间 = DateTime.Now;//为当前时间
            //DataTable dt_明细表 = new DataTable();
            //string sql2 = "select * from 销售记录销售订单明细表 where 送达日期 >='" + t_当天日期 + "' and 送达日期 < '" + t_后一天 + "'";
            //using (SqlDataAdapter da = new SqlDataAdapter(sql2, strConn))
            //{
            //    dt_明细表 = new DataTable();
            //    da.Fill(dt_明细表);


            //}
            //foreach (DataRow r in dt_明细表.Rows)
            //{
            //    if (r["明细完成日期"] == null || r["明细完成日期"] == "")//第一种没有值的时候
            //    {
            //        int a = 1;

            //    }
            //    else if (Convert.ToDateTime(r["明细完成日期"]) <= Convert.ToDateTime(r["送达日期"]).AddDays(1))//第二种符合情况的时候
            //    {
            //        int a = 2;
            //    }
            //}
        }

        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
        }
        //上传
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           
           try
           {
               
               fun_tc();              
               
            }
           catch (Exception ex)
           {
               MessageBox.Show(ex.Message );
            }
        }
        //下载
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           
            try 
            {
                
                fun_xz();
               
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message );
            
            }
        }
      
        //预览
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_预览();
        }
        //新增
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
             //DataRow r;
             r = dt.NewRow();
             dt.Rows.Add(r);

        }
        //保存
       
        //删除
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try {
                DataRow rr;
                rr = gv1.GetDataRow(gv1.FocusedRowHandle);
                fun_删除(rr);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        //关闭
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //CPublic.UIcontrol.ClosePage();
            this.Close();
        }
       
        private void fun_上传文件(string pathName )
        {

            FileInfo info = new FileInfo(pathName);      //判定上传文件的大小
            long maxlength = info.Length;
            if (maxlength > 1024 * 1024 * 8)
            {
                throw new Exception("上传的文件不能超过1M，请重新选择后再上传！");
            }

            //CZMaster.MasterFileService.strWSDL = CPublic.Var.strWSConn;
            CFileTransmission.CFileClient.strCONN = strConn_FS;
            string strguid = "";  //记录系统自动返回的GUID

            strguid = CFileTransmission.CFileClient.sendFile(pathName);
            r = gv1.GetDataRow(gv1.FocusedRowHandle);
            r["计量器具编号"] = dr_证["计量器具编号"];
            r["计量器具名称"] = dr_证["计量器具名称"];
            r["文件GUID"] = strguid;
            r["文件名称"] = Path.GetFileName(pathName);
            r["已上传"] = strygh;
            //dt.Rows.Add(r["计量器具编号"].ToString(), r["计量器具名称"].ToString(), strguid, r["文件类型"].ToString(), Path.GetFileName(pathName), strygh);
            //DataTable temp = dt.Clone();
            //temp.ImportRow(r);
            gv1.CloseEditor();
            this.BindingContext[dt].EndCurrentEdit();
            CZMaster.MasterSQL.Save_DataTable(dt, "计量器具检定证书表", CPublic.Var.strConn);
        }


        //private void fun_下载(string pathName, DataRow xr)
        //{
        //    CFileTransmission.CFileClient.strCONN = strConn_FS;
          
        //    Receiver(xr["文件GUID"].ToString(), pathName);
        //    //CZMaster.MasterSQL.Save_DataTable(dt, "计量器具检定证书表", CPublic.Var.strConn);

        //}
     
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
        private void fun_xz()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.InitialDirectory = "c:\\";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|*.jpg|*.png|All files (*.*)|*.*";
            saveFileDialog1.Title = "下载文件";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
            DataRow xr;
            xr = gv1.GetDataRow(gv1.FocusedRowHandle);
            CFileTransmission.CFileClient.strCONN = strConn_FS;               
            Receiver(xr["文件GUID"].ToString(), saveFileDialog1.FileName);
            DevExpress.XtraEditors.XtraMessageBox.Show("下载成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }


        private void fun_删除(DataRow r)
        {
            if (MessageBox.Show("确认删除吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                DataRow[] dr = dt.Select(string.Format("计量器具编号='{0}' and 文件名称='{1}'", r["计量器具编号"].ToString(), r["文件名称"].ToString()));
                //CZMaster.MasterFileService.strWSDL = CPublic.Var.strWSConn;
                CFileTransmission.CFileClient.strCONN = strConn_FS;
                CFileTransmission.CFileClient.deleteFile(r["文件GUID"].ToString());
                dr[0].Delete();
                CZMaster.MasterSQL.Save_DataTable(dt, "计量器具检定证书表", CPublic.Var.strConn);
                MessageBox.Show("删除成功");
            }
        }


        private void fun_预览()
        {

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            string pathname = "D:\\下载文件\\预览文件.jpg";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|*.jpg|*.png|All files (*.*)|*.*";
            saveFileDialog1.Title = "下载文件";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            DataRow yl;
            yl = gv1.GetDataRow(gv1.FocusedRowHandle);
            CFileTransmission.CFileClient.strCONN = strConn_FS;
            Receiver(yl["文件GUID"].ToString(), pathname);
           
            System.Diagnostics.Process.Start(pathname);   
                
           
        }

            

        private void 上传ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                fun_tc();

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

                fun_xz();

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
                DataRow rr;
                rr = gv1.GetDataRow(gv1.FocusedRowHandle);
                fun_删除(rr);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 预览ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fun_预览();
        }

        //右击菜单
        private void gc1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc1, new Point(e.X, e.Y));

            }
        }


        private void fun_tc()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|*.jpg|*.png|All files (*.*)|*.*";
            openFileDialog1.Title = "上传文件";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fun_上传文件(openFileDialog1.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("上传成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                r["已上传"] = true;
            }

           
        
        }

        //保存
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                gv1.CloseEditor();//关闭编辑状态
                this.BindingContext[dt].EndCurrentEdit();//关闭编辑状态
                string sql = "select * from 计量器具检定证书表 where 1<>1";
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

    }
}
