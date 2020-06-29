using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DevExpress.XtraPrinting;

namespace BaseData
{
    public partial class frm员工培训计划导入查询 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        DataTable dtM = null;
        DataTable dtP = null;
        DataTable dt_人员;
        DataTable dt_课室;
        DataTable dt_内容分类;
        Boolean bl_sate=false;
        string strConn_FS = CPublic.Var.geConn("FS");
        public frm员工培训计划导入查询()
        {
            InitializeComponent();
        }

        private void frm员工培训计划导入查询_Load(object sender, EventArgs e)
        {
            try
            {
                time_前.EditValue = null;
                fun_载入(System.DateTime.Today.ToString(), System.DateTime.Today.AddDays(1).AddSeconds(-1).ToString());
                fun_员工();
                fun_部门();
               barLargeButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barStaticItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                time_前.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                time_后.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                checkBox1.Checked = true;
                 DateTime dtime=CPublic.Var.getDatetime();
                 DateTime dtime2=dtime.AddMonths(-1);
                dtime2=new DateTime (dtime2.Year,dtime2.Month,1);

                time_前.EditValue = dtime2;
                time_后.EditValue = dtime;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region 方法
        private void fun_载入(String time, String times)
        {
            string sql = "select * from 人事员工培训计划表 where 日期 >= '" + time + "' and 日期 <= '" + times + "'";
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            gcM.DataSource = dtM;
            gvM.OptionsBehavior.Editable = false;//如何控制GridView单元格是否可编辑
        }

        private void fun_员工()
        {
            string sql = "select 员工号,姓名,部门 from 人事基础员工表 where 在职状态  = '在职'";
            dt_人员 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_人员);
            repositoryItemSearchLookUpEdit1.DataSource = dt_人员;
            repositoryItemSearchLookUpEdit1.DisplayMember = "员工号";
            repositoryItemSearchLookUpEdit1.ValueMember = "员工号";
        }


        private void fun_上传(string pathName)
        {
            try
            {
                FileInfo info = new FileInfo(pathName);
                long maxinfo = info.Length;
                if (maxinfo > 1024 * 1024 * 8)
                {
                    throw new Exception("上传的照片不能超过1M，请重新选择上传！");
                }
                string type = "";
                //type = pathName.Substring(pathName.LastIndexOf("."), pathName.Length - pathName.LastIndexOf("."));
                //type = pathName.Substring(pathName.LastIndexOf("."), pathName.Length - pathName.LastIndexOf(".")).Replace(".", "");
                int s=pathName.LastIndexOf(".")+1;
                type = pathName.Substring(s, pathName.Length -s);
                CFileTransmission.CFileClient.strCONN = strConn_FS;
                string strguid = CFileTransmission.CFileClient.sendFile(pathName);
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                dr["附件上传"] = strguid;
                dr["附件后缀"] = type;
                //byte[] bs = System.IO.File.ReadAllBytes(pathName);
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fun_部门()
        {
            //19-4-2 课室-->部门
            string sql = "select 部门 from 人事基础员工表 where 部门 != '' and 在职状态  = '在职' group by 部门";
            dt_课室 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_课室);
            repositoryItemSearchLookUpEdit2.DataSource = dt_课室;
            repositoryItemSearchLookUpEdit2.DisplayMember = "部门";
            repositoryItemSearchLookUpEdit2.ValueMember = "部门";

            string sql_1 = "select 属性值 as 课程类别  from  [基础数据基础属性表] where 属性类别='课程类别' order by 属性值";
            dt_内容分类 = new DataTable();
            SqlDataAdapter da_1 = new SqlDataAdapter(sql_1, strconn);
            da_1.Fill(dt_内容分类);
            repositoryItemSearchLookUpEdit3.DataSource = dt_内容分类;
            repositoryItemSearchLookUpEdit3.DisplayMember = "课程类别";
            repositoryItemSearchLookUpEdit3.ValueMember = "课程类别";
        }
        #endregion

        #region 界面操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            checkBox2.Checked = true;
            DataRow dr = dtM.NewRow();
            dtM.Rows.Add(dr);
            dr["GUID"] = System.Guid.NewGuid();
            dr["制定人员ID"] = CPublic.Var.LocalUserID;
            dr["制定人员"] = CPublic.Var.localUserName;
            dr["培训计划单号"] = string.Format("PXJH{0}{1}{2}{3}", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("00"),
                DateTime.Now.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("PXJH", DateTime.Now.Year, DateTime.Now.Month).ToString("00"));

            str = dr["培训计划单号"].ToString();
            string sql = string.Format("select * from 人事员工培训计划子表 where 培训计划单号 = '{0}'", str);
            dtP = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtP);
            gcP.DataSource = dtP;
            gvM.OptionsBehavior.Editable = true;

          //  dtP.ColumnChanged += dtP_ColumnChanged;
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
            dr.Delete();

            foreach (DataRow r in dtP.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                r.Delete();
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gvM.CloseEditor();
                gcM.BindingContext[dtM].EndCurrentEdit();
                gvP.CloseEditor();
                gcP.BindingContext[dtP].EndCurrentEdit();
                if (dtP != null)
                {
                    string sql1 = "select * from 人事员工培训计划子表 where 1<>1";
                    SqlDataAdapter da1 = new SqlDataAdapter(sql1, strconn);
                    new SqlCommandBuilder(da1);
                    da1.Update(dtP);
                }
                //处理数据

                string sql = "select * from 人事员工培训计划表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dtM);
                checkBox2.Checked = true;
                MessageBox.Show("保存成功");
                barLargeButtonItem4_ItemClick(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (time_前.EditValue != null && time_后.EditValue != null)
            {
                DateTime tm = ((DateTime)time_后.EditValue).AddDays(1).AddSeconds(-1);
                fun_载入(time_前.EditValue.ToString(), tm.ToString());
            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dtP == null)
            {
                return;
            }
            DataRow dr = dtP.NewRow();
            dtP.Rows.Add(dr);
            dr["培训计划单号"] = str;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);
            if (dr != null)
            {
                dr.Delete();
            }
        }
        #endregion

        #region 触发事件
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                barLargeButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barStaticItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                time_前.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                time_后.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            else
            {
                barLargeButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barStaticItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                time_前.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                time_后.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
        }

        string str = "";
        private void gvM_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                str = dr["培训计划单号"].ToString();
                string sql = string.Format("select * from 人事员工培训计划子表 where 培训计划单号 = '{0}'", str);
                dtP = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtP);
                gcP.DataSource = dtP;
                if (e.Clicks == 2)
                {
                    if (bl_sate == false)
                    {
                        bl_sate = true;
                        barLargeButtonItem8_ItemClick(null, null);
                        bl_sate = false;
                    }
                }
                //dtP.ColumnChanged += dtP_ColumnChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                gvM.OptionsBehavior.Editable = true;
                gvP.OptionsBehavior.Editable = true;

            }
            else
            {
                gvM.OptionsBehavior.Editable = false;
                gvP.OptionsBehavior.Editable = false;

            }
        }

        void dtP_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            ////if (e.Column.Caption == "员工号")
            ////{
            ////    DataRow[] ds = dt_人员.Select(string.Format("员工号 = '{0}'",e.Row["员工号"]));
            ////    e.Row["培训人员名单"] = ds[0]["姓名"];
            ////}
            //if (e.Column.Caption == "课室")
            //{
            //    DataRow[] ds = dt_人员.Select(string.Format("课室 = '{0}'", e.Row["课室"]));
            //    for (int i = 0; i < ds.Length; i++)
            //    {
            //        if (i > 0)
            //        {
            //            DataRow rr = dtP.NewRow();
            //            dtP.Rows.Add(rr);
            //            rr["培训计划单号"] = str;
            //            rr["培训人员名单"] = ds[i]["姓名"];
            //            rr["员工号"] = ds[i]["员工号"];
            //        }
            //        if (i == 0)
            //        {
            //            e.Row["培训人员名单"] = ds[i]["姓名"];
            //            e.Row["员工号"] = ds[i]["员工号"];
            //        }
            //    }
            //}
        }

        private void gvP_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);

            if (e.Column.Caption == "部门" && e.Value != null && e.Value.ToString() != "")
            {
                DataRow[] ds = dt_人员.Select(string.Format("部门 = '{0}'", e.Value.ToString()));
                for (int i = 0; i < ds.Length; i++)
                {
                    if (i > 0)
                    {
                        DataRow rr = dtP.NewRow();
                        dtP.Rows.Add(rr);
                        rr["培训计划单号"] = str;
                        rr["培训人员名单"] = ds[i]["姓名"];
                        rr["员工号"] = ds[i]["员工号"];
                        rr["部门"] = e.Value.ToString();
                    }
                    if (i == 0)
                    {
                        dr["培训人员名单"] = ds[i]["姓名"];
                        dr["员工号"] = ds[i]["员工号"];
                        dr["部门"] = e.Value.ToString();
                    }
                }
            }
            if (e.Column.Caption == "培训人员工号" && e.Value!=null)
            {
                DataRow[] ds = dt_人员.Select(string.Format("员工号 = '{0}'", e.Value.ToString()));
                if (ds.Length > 0)
                {
                    dr["培训人员名单"] = ds[0]["姓名"];
                    dr["部门"] = ds[0]["部门"];
                    dr["员工号"] = ds[0]["员工号"];

                }

            }
        }
        #endregion

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                OpenFileDialog openpic = new OpenFileDialog();
                if (openpic.ShowDialog() == DialogResult.OK)
                {
                    fun_上传(openpic.FileName);
                    MessageBox.Show("文件上传成功！");

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
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                if (dr["附件上传"] == null || dr["附件上传"].ToString() == "")
                {
                    throw new Exception("没有文件可以下载，请先上传文件");
                }
                SaveFileDialog save = new SaveFileDialog();
                save.FileName = dr["附件上传"].ToString()+"."+dr["附件后缀"].ToString();
                if (save.ShowDialog() == DialogResult.OK)
                {
                    
                    CFileTransmission.CFileClient.strCONN = strConn_FS;
                    CFileTransmission.CFileClient.Receiver(dr["附件上传"].ToString(), save.FileName);
                    MessageBox.Show("文件下载成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gvP_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gvM_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                
                
                if (dr["附件上传"] == null || dr["附件上传"].ToString() == "")
                {
                    throw new Exception("没有文件可以查看，请先上传文件");
                }
                string type = dr["附件后缀"].ToString();
                string foldPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                string fileName = foldPath + DateTime.Now.ToString("yyyy-MM-dd") + "T" + DateTime.Now.ToString("HH_mm_ss") + "Z" + "_" + Guid.NewGuid().ToString() + "." + type;
                try
                {
                    System.IO.Directory.Delete(foldPath, true);
                }
                catch (Exception  )
                {
                 
                }
                CFileTransmission.CFileClient.strCONN = strConn_FS;
                CFileTransmission.CFileClient.Receiver(dr["附件上传"].ToString(), fileName);
                System.Diagnostics.Process.Start(fileName);
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new XlsxExportOptions(TextExportMode.Text, false, false);

                gcM.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

 

    }
}
