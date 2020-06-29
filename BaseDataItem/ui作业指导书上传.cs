using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace BaseData
{
    public partial class ui作业指导书上传 : UserControl
    {
        public static DevExpress.XtraTab.XtraTabControl XTC;
        string s_类别名称 = ""; //切换combox1会更改
        DataRow r_M;
        DataTable dtP;
        string strcon = CPublic.Var.strConn;
        string strConn_FS = CPublic.Var.geConn("FS");


        public ui作业指导书上传()
        {
            InitializeComponent();
        }
        public ui作业指导书上传(DataRow dr)
        {
            InitializeComponent();
            r_M = dr;
            textBox1.Text = dr["物料编码"].ToString();
            textBox2.Text = dr["物料名称"].ToString();
            textBox3.Text = dr["n原ERP规格型号"].ToString();
            textBox6.Text = dr["大类"].ToString();
            textBox5.Text = dr["小类"].ToString();
            comboBox1.Text = "单个产品";
        }
        private void fun_initialize(string str)
        {
         
                string sql = string.Format("select * from 作业指导书文件表 where 类别分组='{0}'and 类别名称='{1}'  order by 版本",comboBox1.Text,str);
                dtP = new DataTable();
                dtP = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                gridControl1.DataSource = dtP;
            
        }
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            s_类别名称 = "";
            if (comboBox1.Text == "大类")
            {
                s_类别名称 = textBox6.Text;
            }
            else if (comboBox1.Text == "小类")
            {
                s_类别名称 = textBox5.Text;
            }
            else
            {
                s_类别名称 = textBox1.Text;
            }
            fun_initialize(s_类别名称);
        }
        //新增
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow r = dtP.NewRow();
            dtP.Rows.Add(r);
            if (dtP.Rows.Count == 1)
            {
                r["版本"] = "0";
            }
            else
            {
              r["版本"]= Convert.ToInt32(dtP.Select("版本=max(版本)")[0]["版本"])+1;

            }
        }
        //上传
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_check();
                OpenFileDialog openfile = new OpenFileDialog();
                if (openfile.ShowDialog() == DialogResult.OK)
                {
                    fun_上传( openfile.FileName);

                    fun_单条刷新();

                    MessageBox.Show("文件上传成功！");

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
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (MessageBox.Show(string.Format("是否确认删除版本:{0}？", dr["版本"].ToString()), "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CFileTransmission.CFileClient.strCONN = strConn_FS;
                    CFileTransmission.CFileClient.deleteFile(dr["文件地址"].ToString());
                    dr.Delete();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
               
                fun_save();
                fun_initialize(s_类别名称);
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_上传( string pathName)
        {
            try
            {

                gridView1.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                CFileTransmission.CFileClient.strCONN = strConn_FS;

                string strguid = CFileTransmission.CFileClient.sendFile(pathName);
                string type = "";
                //type = pathName.Substring(pathName.LastIndexOf("."), pathName.Length - pathName.LastIndexOf(".")).Replace(".", "");
                int s = pathName.LastIndexOf(".") + 1;
                type = pathName.Substring(s, pathName.Length - s);
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
              //  dr["图纸号"] = str_name;
                dr["类别名称"] = s_类别名称;
                dr["类别分组"] = comboBox1.Text;
                dr["文件地址"] = strguid;
                dr["后缀"] = type;
                dr["文件名"] = Path.GetFileName(pathName);
                dr["上传时间"] = CPublic.Var.getDatetime();
                dr["修改时间"] = CPublic.Var.getDatetime();
                dr["修改人"] = CPublic.Var.localUserName;
                //gridView1.CloseEditor();
                //this.BindingContext[dtP].EndCurrentEdit();
                DataTable dt = dtP.Clone();
                dt.ImportRow(dr);

                FileInfo info = new FileInfo(pathName);
                long maxinfo = info.Length;

                if (maxinfo > 1024 * 1024 * 8)
                {
                    throw new Exception("上传的文件不能超过1M，请重新选择上传！");
                }



                CZMaster.MasterSQL.Save_DataTable(dt, "作业指导书文件表", strcon);

                //byte[] bs = System.IO.File.ReadAllBytes(pathName);
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void fun_check()
        {
            if (comboBox1.Text == "")
            {
                throw new Exception("类别未选择");

            }
            DataRow drr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (drr == null)
            {
                throw new Exception("未选中任何行上传文件");
            }
            if (drr["文件地址"].ToString() != "")
            {

                throw new Exception("选中版本已上传文件,若要替换请先删除原先版本,再新增");

            }
            DateTime t = CPublic.Var.getDatetime();
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                if (dr["版本"].ToString() == "")
                {
                    throw new Exception("有版本为空,请核对");
                }
                dr["修改时间"] = t;
            }

        }
        private void fun_单条刷新()
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string sql = string.Format("select * from 作业指导书文件表 where 类别名称='{0}' and 版本='{1}'", s_类别名称, dr["版本"].ToString());
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            DataRow[] r_1 = dtP.Select(string.Format("类别名称='{0}' and 版本='{1}'", s_类别名称, dr["版本"].ToString()));
            r_1[0].ItemArray = dt.Rows[0].ItemArray;
            r_1[0].AcceptChanges();
        }

        private void fun_save()
        {
            string sql = "select * from 作业指导书文件表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                new SqlCommandBuilder(da);
                da.Update(dtP);
            }
          
        }
        //下载
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (dr["文件地址"] == null || dr["文件地址"].ToString() == "")
                {
                    throw new Exception("没有文件可以下载，请先上传文件");
                }
                SaveFileDialog save = new SaveFileDialog();
                save.FileName = dr["文件地址"].ToString() + "." + dr["后缀"].ToString();
                if (save.ShowDialog() == DialogResult.OK)
                {
                    CFileTransmission.CFileClient.strCONN = strConn_FS;
                    CFileTransmission.CFileClient.Receiver(dr["文件地址"].ToString(), save.FileName);
                    MessageBox.Show("文件下载成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);


                if (dr["文件地址"] == null || dr["文件地址"].ToString() == "")
                {
                    throw new Exception("没有文件可以查看，请先上传文件");
                }
                string type = dr["后缀"].ToString();
                string foldPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                //string fileName = foldPath + CPublic.Var.getDatetime().ToString("yyyy-MM-dd") + "T" + CPublic.Var.getDatetime().ToString("HH_mm_ss") + "Z" + "_" + Guid.NewGuid().ToString() + "." + type;
                string fileName = foldPath + "预览文件" + "." + type;

                try
                {
                    System.IO.Directory.Delete(foldPath, true);
                }
                catch (Exception)
                {
                }
                CFileTransmission.CFileClient.strCONN = strConn_FS;
                CFileTransmission.CFileClient.Receiver(dr["文件地址"].ToString(), fileName);
                System.Diagnostics.Process.Start(fileName);




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //关闭
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (XTC.TabPages.Count == 1) { }
            if (XTC.SelectedTabPage.Text == "首页") { }
            DevExpress.XtraTab.XtraTabPage xtp = null;
            try
            {
                xtp = XTC.SelectedTabPage;
                XTC.SelectedTabPageIndex = XTC.SelectedTabPageIndex - 1;
            }
            catch { }
            try
            {
                xtp.Controls[0].Dispose();
                XTC.TabPages.Remove(xtp);
                xtp.Dispose();
            }
            catch { }
        }

        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
            sop批量上传 ui = new sop批量上传(r_M);
            CPublic.UIcontrol.Showpage(ui, "批量上传");
        }

    }
}
