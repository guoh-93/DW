using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class frm作业指导预览 : Form
    {
        string strcon = CPublic.Var.strConn;
        string strConn_FS = CPublic.Var.geConn("FS");
        public frm作业指导预览()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm作业指导预览_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            textBox1.Focus();
        }



        protected override void WndProc(ref Message m)
        {
            if ((int)m.WParam == 516)
            {
                //按下鼠标右键
                m.Msg = 528;

                m.WParam = new IntPtr(513);

            }
            base.WndProc(ref m);
        }

     

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.Close();
        }

#pragma warning disable IDE1006 // 命名样式
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    string s = string.Format(@"select  b.原ERP物料编号,b.大类,b.小类 from  生产记录生产工单表 a
                                left  join  基础数据物料信息表 b  on a.物料编码=b.物料编码
                                         where 生产工单号='{0}'", textBox1.Text);
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    //先用物料号找 ，找不到用小类找，最后用大类
                    s = string.Format(@"select * from 作业指导书文件表 where 类别名称='{0}' and  版本= (select  MAX(版本) from 作业指导书文件表  where 类别名称='{0}')", dt.Rows[0]["原ERP物料编号"]);
                    DataTable tt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    if (tt.Rows.Count == 0)
                    {
                        s = string.Format(@"select * from 作业指导书文件表 where 类别名称='{0}' and  版本= (select  MAX(版本) from 作业指导书文件表  where 类别名称='{0}')", dt.Rows[0]["小类"]);
                        tt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                        if (tt.Rows.Count == 0)
                        {
                            s = string.Format(@"select * from 作业指导书文件表 where 类别名称='{0}' and  版本= (select  MAX(版本) from 作业指导书文件表  where 类别名称='{0}')", dt.Rows[0]["大类"]);
                            tt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                            if (tt.Rows.Count == 0)
                            {
                                throw new Exception("文件未上传");
                            }
                        }
                    }
                    //tt 文件相关信息

                    DataRow rr = tt.Rows[0];
                    string type = rr["后缀"].ToString();
                    string foldPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                    //     string fileName = foldPath + DateTime.Now.ToString("yyyy-MM-dd") + "T" + DateTime.Now.ToString("HH_mm_ss") + "Z" + "_" + Guid.NewGuid().ToString() + "." + type;
                    string fileName = foldPath + "预览." + type;
                    try
                    {
                        System.IO.Directory.Delete(foldPath, true);
                    }
                    catch (Exception)
                    {

                    }
                    CFileTransmission.CFileClient.strCONN = strConn_FS;
                    CFileTransmission.CFileClient.Receiver(rr["文件地址"].ToString(), fileName);

                    axAcroPDF1.setPageMode("thumbs");
                    axAcroPDF1.setPageMode("None");
                    axAcroPDF1.setShowToolbar(false);

                    axAcroPDF1.LoadFile(fileName);

                    textBox1.Text = "";
                    textBox1.Focus();


                }

                catch (Exception ex)
                {


                }
            }
        }
    }
}
