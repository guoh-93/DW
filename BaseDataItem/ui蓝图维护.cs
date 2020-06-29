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
    public partial class ui蓝图维护 : UserControl
    {
        public static DevExpress.XtraTab.XtraTabControl XTC;

        DataRow r_M;
        DataTable dtP;
        string strcon = CPublic.Var.strConn;
        string strConn_FS = CPublic.Var.geConn("FS");
        public ui蓝图维护()
        {
            InitializeComponent();
        }
        public ui蓝图维护(DataRow dr)
        {

            InitializeComponent();
            r_M = dr;
            textBox1.Text = dr["物料编码"].ToString();
            textBox2.Text = dr["物料名称"].ToString();
            textBox3.Text = dr["规格型号"].ToString();

        }

        private void fun_initialize(string str)
        {
            string sql = string.Format("select * from 基础物料蓝图表 where 物料号='{0}' order by 版本", str);
            dtP = new DataTable();
            dtP = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtP;
        }
        private void fun_check()
        {

            DateTime t = CPublic.Var.getDatetime();
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                if (dr["版本"].ToString() == "")
                {
                    throw new Exception("有版本为空,请核对");
                }
                if (dr["图纸号"].ToString() == "")
                {
                    throw new Exception("有图纸为空,请核对");
                }
                dr["物料号"] = textBox1.Text;
                dr["修改时间"] = t;


            }
        }
        private void fun_save()
        {
            string sql = "select * from 基础物料蓝图表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                new SqlCommandBuilder(da);
                da.Update(dtP);
            }
            if (gridView1.RowCount > 0)
            {
                string s = string.Format("update 基础数据物料信息表  set 有无蓝图=1 where 物料编码='{0}'", textBox1.Text);
                CZMaster.MasterSQL.ExecuteSQL(s, strcon);

            }
        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                fun_check();
                fun_save();
                fun_initialize(r_M["物料编码"].ToString());
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ui蓝图维护_Load(object sender, EventArgs e)
        {
            try
            {
                fun_initialize(r_M["物料编码"].ToString());
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow r = dtP.NewRow();
            dtP.Rows.Add(r);
            if (dtP.Rows.Count == 1)
            {
                r["版本"] = "0";
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

                    dr.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_上传(string str_name, string pathName)
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
                dr["图纸号"] = str_name;
                dr["物料号"] = textBox1.Text;
                dr["文件地址"] = strguid;
                dr["后缀"] = type;
                dr["文件名"] = Path.GetFileName(pathName);
                dr["修改时间"] = CPublic.Var.getDatetime();
                dr["修改人"] = CPublic.Var.localUserName;
                //gridView1.CloseEditor();
                //this.BindingContext[dtP].EndCurrentEdit();
                DataTable dt = dtP.Clone();
                dt.ImportRow(dr);

                FileInfo info = new FileInfo(pathName);
                long maxinfo = info.Length;
                if (maxinfo > 1024 * 1024 * 8 * 5)
                {
                    throw new Exception("上传的照片不能超过5M，请重新选择上传！");
                }



                CZMaster.MasterSQL.Save_DataTable(dt, "基础物料蓝图表", strcon);

                //byte[] bs = System.IO.File.ReadAllBytes(pathName);
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fun_单条刷新()
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string sql = string.Format("select * from 基础物料蓝图表 where 物料号='{0}' and 版本='{1}'", textBox1.Text, dr["版本"].ToString());
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            DataRow[] r_1 = dtP.Select(string.Format("物料号='{0}' and 版本='{1}'", textBox1.Text, dr["版本"].ToString()));
            r_1[0].ItemArray = dt.Rows[0].ItemArray;
            r_1[0].AcceptChanges();
        }

        private bool fun_check(string str_版本)
        {
            DataRow[] r = dtP.Select(string.Format("版本='{0}'", str_版本));
            if (r.Length > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //上传
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {


                OpenFileDialog openfile = new OpenFileDialog();
                if (openfile.ShowDialog() == DialogResult.OK)
                {
                    int s = openfile.SafeFileName.LastIndexOf(".");
                    string str_图号 = openfile.SafeFileName.Substring(0, s);


                    fun_上传(str_图号, openfile.FileName);

                    fun_单条刷新();

                    MessageBox.Show("文件上传成功！");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                save.Filter = "(*.jpg,*.png,*.jpeg,*.bmp,*.gif)|*.jgp;*.png;*.jpeg;*.bmp;*.gif|All files(*.*)|*.*";
                //save.FileName = dr["文件地址"].ToString() + "." + dr["后缀"].ToString();
                save.FileName = dr["文件名"].ToString();

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

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //try
            //{
            //    CPublic.UIcontrol.ClosePage();
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
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

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Clicks == 2)
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

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.Caption == "版本" && e.Value != null && e.Value.ToString() != "")
                {
                    bool bl = fun_check(e.Value.ToString());

                    if (bl == false)
                    {
                        DataRow dr = gridView1.GetDataRow(e.RowHandle);
                        dr["版本"] = "";
                        throw new Exception("版本已重复");
                    }
                }

              
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.Caption == "启用" && Convert.ToBoolean(e.Value))
                {
                    DataRow dr = gridView1.GetDataRow(e.RowHandle);
                    foreach (DataRow dre in dtP.Rows)
                    {
                        if (dre.RowState == DataRowState.Deleted)
                            continue;
                        dre["启用"] = false;
                    }
                    dr["启用"] = true;

                }
            }
            catch (Exception ex) 
            {

                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
