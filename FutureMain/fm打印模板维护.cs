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
using BaseData;
namespace FutureMain
{
    public partial class fm打印模板维护 : Form
    {
        DataTable dtM ;
        SqlDataAdapter da;
        CurrencyManager cmM;

        //private static string PWD = "a"; 
        //private static string UID = "sa";
       
        //private static string SQLSERVER = "192.168.2.38";
 
        //private static string DATABASE = "DWERP";
        //private static string strconn = string.Format("Password={0};Persist Security Info=True;User ID={1};Initial Catalog={2};Data Source={3};Pooling=true;Max Pool Size=40000;Min Pool Size=0", PWD, UID, DATABASE, SQLSERVER);
        string strconn = CPublic.Var.strConn;

        public fm打印模板维护()
        {
            InitializeComponent();
        }
       

        private void fm打印模板维护_Load(object sender, EventArgs e)
        {
            fun_加载打印模板();
            //dataGridView1.AutoGenerateColumns = false;
            //dataGridView1.DataSource = dtM;
            cmM = this.BindingContext[dtM] as CurrencyManager;
        }

        private void fun_加载打印模板()
        {
            dtM = new DataTable();
            string sql = "select 模板名,上传时间 from 基础记录打印模板表";
            da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Fill(dtM);
            gridControl1.DataSource = dtM;
        
        }

   
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow r = (this.BindingContext[dtM].Current as DataRowView).Row;
                if (r["模板名"].ToString() == "")
                {
                    throw new Exception("请先填写模板名");
                }
                if (MessageBox.Show("是否要上传该模板？", "询问？", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    


                    using (OpenFileDialog op = new OpenFileDialog())
                    {
                        if (op.ShowDialog() == DialogResult.OK)
                        {
                            string s = string.Format("select  * from 基础记录打印模板表 where 模板名='{0}'", r["模板名"]);
                            using (SqlDataAdapter da = new SqlDataAdapter(s, strconn))
                            {
                                DataTable t = new DataTable();
                                da.Fill(t);
                                if (t.Rows.Count > 0) //已有  替换
                                {
                                    t.Rows[0]["数据"] = System.IO.File.ReadAllBytes(op.FileName);
                                }
                                else
                                {
                                    DataRow dr = t.NewRow();
                                    dr["模板名"] = r["模板名"].ToString();
                                    dr["数据"] = System.IO.File.ReadAllBytes(op.FileName);
                                    dr["上传时间"] = CPublic.Var.getDatetime();
                                    t.Rows.Add(dr);

                                }
                                r["上传时间"] = CPublic.Var.getDatetime();

                                new SqlCommandBuilder(da);
                                da.Update(t);

                            }
 

 
                            MessageBox.Show("OK");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "打印模板维护");
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
            
            r["文件GUID"] = strguid;
            r["模板名"] = Path.GetFileName(pathName);
            r["上传时间"] = CPublic.Var.getDatetime();
            gridView1.CloseEditor();//关闭编辑状态
            this.BindingContext[dtM].EndCurrentEdit();//关闭编辑状态

            //dtP.Rows.Add(strygh, r["文件名称"].ToString(), strguid, Path.GetFileName(pathName));
            CZMaster.MasterSQL.Save_DataTable(dtM, "基础记录打印模板表", CPublic.Var.strConn);

        }







        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                cmM.AddNew();
            }
            catch
            {}
            
        }
        //下载
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow r = (this.BindingContext[dtM].Current as DataRowView).Row;
                if(r["模板名"].ToString()!="")
                {
                  SaveFileDialog save = new SaveFileDialog();
                    //save.FileName = r["表单名称"].ToString();     
                 // string fileName = System.Windows.Forms.Application.StartupPath + string.Format(@"\prttmp\{0}.xlsx", r["模板名"].ToString().Trim());
                  save.Filter = "所有文件(*.xlsx)|*.xlsx"; //保存类型
                    DialogResult dialogResult = save.ShowDialog(this);
                    if (dialogResult == DialogResult.OK)
                    {
                        string s = string.Format("select  * from 基础记录打印模板表 where 模板名='{0}'", r["模板名"]);
                        using (SqlDataAdapter da = new SqlDataAdapter(s, strconn))
                        {
                            DataTable t = new DataTable();
                            da.Fill(t);
                            if (t.Rows.Count >= 0)
                            {
                                System.IO.File.WriteAllBytes(save.FileName, (byte[])t.Rows[0]["数据"]);
                                MessageBox.Show("下载成功！");
                            }
                        }

                    }
                }
            }
            catch(Exception ex)
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

        //删除
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow rr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string s = string.Format("select  * from 基础记录打印模板表 where 模板名='{0}'",rr["模板名"]);
                using (SqlDataAdapter da = new SqlDataAdapter(s, strconn))
                {
                    DataTable t = new DataTable();
                    da.Fill(t);
                    if (t.Rows.Count >= 0)
                    {
                        t.Rows[0].Delete();
                        new SqlCommandBuilder(da);
                        da.Update(t);
                    }
                }
                dtM.Rows.Remove(rr);
                MessageBox.Show("删除成功！");
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

     
    }
}
