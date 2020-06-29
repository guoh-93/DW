using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MoldMangement
{
    public partial class fm_手动完成备注 : Form
    {
        public static int s_状态 = 0;
        public static string s_手动完成原因;
        public static string s_图片字符串;
        public static byte[] bts;
        public static Image myImage;
        public static string bt;
        string type = "";
        public static string ss = "";
        public fm_手动完成备注()
        {
            InitializeComponent();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                s_手动完成原因 = textBox1.Text;
                if (s_手动完成原因.Trim() == "") throw new Exception("原因不可为空");
                s_状态 = 1;
                this.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
           
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (OpenFileDialog op = new OpenFileDialog())
            {
                if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (op.OpenFile() != null)
                    {
                        string str = op.FileName;//文件带有完整路径的名字
                        bt = Path.GetFileNameWithoutExtension(op.FileName);//只有名字
                        type = op.FileName.Substring(op.FileName.LastIndexOf("."), op.FileName.Length - op.FileName.LastIndexOf(".")).Replace(".", "");
                        ss = bt + (".") + type;
                        bts = System.IO.File.ReadAllBytes(str);
                        byte[] imagedata = (bts);
                        MemoryStream myStream = new MemoryStream();
                        foreach (byte a in imagedata)
                        {
                            myStream.WriteByte(a);
                        }
                        myImage = Image.FromStream(myStream);
                        myStream.Close();

                    }
                }
            }
        }


    }
}
