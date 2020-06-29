using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPrinting;
using System.IO;

namespace ERPSale
{
    public partial class UI售后信息审核 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string strConn_FS = CPublic.Var.geConn("FS");
        string cfgfilepath = "";
        public UI售后信息审核()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UI售后信息审核_Load(object sender, EventArgs e)
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();

                barEditItem2.EditValue = Convert.ToDateTime(t.AddDays(1).ToString("yyyy-MM-dd"));
                barEditItem1.EditValue = Convert.ToDateTime(t.AddMonths(-1).ToString("yyyy-MM-dd"));
                barEditItem3.EditValue = "未审核";
                //自动换行
                RepositoryItemMemoEdit repoMemo = new RepositoryItemMemoEdit();
                repoMemo.WordWrap = true;
                repoMemo.AutoHeight = true;
                this.gc.RepositoryItems.Add(repoMemo);
                gv.Columns[6].ColumnEdit = repoMemo;
                //gv.Columns[11].ColumnEdit = repoMemo;
                //gv.Columns[10].ColumnEdit = repoMemo;
                //gv.Columns[9].ColumnEdit = repoMemo;
                gv.Columns[13].ColumnEdit = repoMemo;
                gv.Columns[14].ColumnEdit = repoMemo;
                //gv.Columns[15].ColumnEdit = repoMemo;
                gv.Columns[12].ColumnEdit = repoMemo;
                gv.OptionsView.RowAutoHeight = true;
                fun_加载知识保密等级();

                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name)))
                {

                    gv.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        private void fun_load()
        {
            string str = "";
            if (barEditItem3.EditValue.ToString()=="已审核")
            {
                str = "SELECT * FROM 知识平台录入表 where 录入时间>='" + barEditItem1.EditValue + "' AND 录入时间 <='" + barEditItem2.EditValue + "' and 审核= 1  and 作废 = 0 order by 录入时间 DESC";
            }
            //string str = string.Format("select * from 知识平台录入表 where 录入时间 > '{0}'and 录入时间 < '{1}'and 审核='{2}',barEditItem1.EditValue,barEditItem2.EditValue,barEditItem3.EditValue ");
            if (barEditItem3.EditValue.ToString() == "未审核")
            {
                str = "SELECT * FROM 知识平台录入表 where 录入时间>='" + barEditItem1.EditValue + "' AND 录入时间 <='" + barEditItem2.EditValue + "' and 审核= 0  and 作废 =0  order by 录入时间 DESC";
            }
            if (barEditItem3.EditValue.ToString() == "已作废")
            {
                str = "SELECT * FROM 知识平台录入表 where 录入时间>='" + barEditItem1.EditValue + "' AND 录入时间 <='" + barEditItem2.EditValue + "' and 作废= 1   order by 录入时间 DESC";
            }
            if (barEditItem3.EditValue.ToString() == "所有")
            {
                str = "SELECT * FROM 知识平台录入表 where 录入时间>='" + barEditItem1.EditValue + "' AND 录入时间 <='" + barEditItem2.EditValue + "'  order by 录入时间 DESC";
            }
            using(SqlDataAdapter da = new SqlDataAdapter(str,strconn))
            {
                DataTable dt_加载 = new DataTable();
                da.Fill(dt_加载);
                gc.DataSource =dt_加载;

            }
        
        }
        //双击跳转售后信息录入界面
        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                string sql1 = string.Format("select * from 知识平台文件上传表 where 售后单号='{0}'", dr["售后单号"].ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(sql1, strconn))
                {
                    DataTable dt1 = new DataTable();
                    da.Fill(dt1);
                    gcM1.DataSource = dt1;
                }
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc, new Point(e.X, e.Y));
                }
                if (e.Clicks == 2 && e.Button == System.Windows.Forms.MouseButtons.Left)
                {

                    if (dr["审核"].Equals(true))
                    {
                        throw new Exception("此单已审核不可修改");
                    }
                    else
                    {
                        UI售后信息录入 fm = new UI售后信息录入(dr);
                        fm.Dock = System.Windows.Forms.DockStyle.Fill;
                        CPublic.UIcontrol.AddNewPage(fm, "信息录入");
                    }

                    //if (e.Clicks == 1)
                    //{
                         

                    //        string sql1 = string.Format("select * from 知识平台文件上传表 where 售后单号='{0}'", dr["售后单号"].ToString());
                    //        using (SqlDataAdapter da = new SqlDataAdapter(sql1, strconn))
                    //        {
                    //            DataTable dt1 = new DataTable();
                    //            da.Fill(dt1);
                    //            gcM1.DataSource = dt1;
                    //        }

                        


                    //}

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           // DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            

            //if (e.Clicks == 1)
            //{
            //    try
            //    {

            //        string sql1 = string.Format("select * from 知识平台文件上传表 where 售后单号='{0}'",dr["售后单号"].ToString());
            //        using(SqlDataAdapter da = new SqlDataAdapter(sql1,strconn))
            //        {
            //            DataTable dt1 = new DataTable();
            //            da.Fill(dt1);
            //            gcM1.DataSource = dt1;
            //        }
                   
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }


            //}



    {
      
    }
    {
    
    }


 }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gv.CloseEditor();
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            try
            {
                if (dr["保密等级"].ToString()=="")
                {
                    throw new Exception("请选择保密级别！");
                }
              if(dr["作废"].Equals(true))
              {
                  throw new Exception("此单已作废");
              }
                if(dr["审核"].Equals(true))
                {
                    throw new Exception("此单已审核，请勿重复审核");
                }

                //using (SqlDataAdapter da = new SqlDataAdapter("select * from 知识平台文件上传表 where 售后单号 = '" + dr["售后单号"] + "'", strconn))
                //{
                //    DataTable dt_文件 = new DataTable();
                //    da.Fill(dt_文件);
                //    if (dt_文件.Rows.Count==0)
                //    {
                //        throw new Exception("未上传文件，不可以审核！");
                //    }

                //}

               using(SqlDataAdapter da = new SqlDataAdapter("select * from 知识平台录入表 where 售后单号 = '"+dr["售后单号"]+"'",strconn))
                {
                    DataTable dt_审核 = new DataTable();
                    da.Fill(dt_审核);
                    dt_审核.Rows[0]["审核"] = true;
                    dt_审核.Rows[0]["审核人员ID"] = CPublic.Var.LocalUserID;
                    dt_审核.Rows[0]["审核人员"] = CPublic.Var.localUserName;
                    dt_审核.Rows[0]["审核日期"] = CPublic.Var.getDatetime();
                   // DataRow[] r = dt_属性.Select(string.Format("级别='{0}'", dr["保密等级"].ToString()));
                    dt_审核.Rows[0]["保密等级"] = dr["保密等级"].ToString();
                    dt_审核.Rows[0]["是否其部门开放"] = Convert.ToBoolean(dr["是否其部门开放"]);
                     new SqlCommandBuilder(da);
                     da.Update(dt_审核);
                     MessageBox.Show("审核完成");
                     barLargeButtonItem3_ItemClick(null,null);
                }



            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                if (barEditItem1.EditValue.ToString() == "" || barEditItem2.EditValue.ToString() == "" || barEditItem3.EditValue.ToString() == "")
                {
                    throw new Exception("请选择日期或者单据状态");
                }
                fun_load();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
         }
         
        private void gvM1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip2.Show(gc, new Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            


        }
        private void fun_预览文件()
        {
            try
            {

                DataRow dr = gvM1.GetDataRow(gvM1.FocusedRowHandle);
                if( dr["售后单号"].ToString()=="")
                {
                    throw new Exception("请选择一行知识单！");
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                string strcoo_路径 = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                string fileName = strcoo_路径 + "\\" + dr["表单名称"].ToString();
                // string strcoo_路径 = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\PDFTMP";
                saveFileDialog.Title = "下载文件";
                saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*|图片文件|*.bmp;*.jpg;*.jpeg;*.gif;*.png";



                CFileTransmission.CFileClient.strCONN = strConn_FS;
                CFileTransmission.CFileClient.Receiver(dr["文件GUID"].ToString(), fileName);
                //预览
                System.Diagnostics.Process.Start(fileName);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void 预览文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fun_预览文件();
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_预览文件();
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr["售后单号"].ToString() =="")
                {
                    throw new Exception("请先选择原因分类，再上传文件");
                }
                string a_售后单号 = dr["售后单号"].ToString();
                知识平台文件上传 fm = new 知识平台文件上传(a_售后单号);
                fm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //导出
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {


                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions(TextExportMode.Text, false, false);

                gc.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}
            }
        }
        DataTable dt_属性;
        private void fun_加载知识保密等级()
        {
            try
            {
                using (SqlDataAdapter da = new SqlDataAdapter("select 属性字段1 as 序号,属性值 as 级别 from 基础数据基础属性表 where 属性类别='知识保密等级'", strconn))
                    {
                         dt_属性 = new DataTable();
                        da.Fill(dt_属性);
                        repositoryItemSearchLookUpEdit3.DataSource = dt_属性;
                        repositoryItemSearchLookUpEdit3.DisplayMember = "级别";
                        repositoryItemSearchLookUpEdit3.ValueMember = "级别";

                    }
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        
        }

        private void gv_ColumnFilterChanged(object sender, EventArgs e)
        {
            try
            {
                if (cfgfilepath != "")
                {
                    gv.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void gv_ColumnPositionChanged(object sender, EventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gv.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }





    }
}
