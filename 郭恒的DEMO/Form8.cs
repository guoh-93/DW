using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using CPublic;
using CZMaster;
//using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Reflection;

//using NPOI;
//using NPOI.SS.UserModel;
//using NPOI.HPSF;
//using NPOI.HSSF;
//using NPOI.HSSF.UserModel;
//using NPOI.POIFS;
//using NPOI.Util;

using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Net.Mail;
using System.Net;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;
 

namespace 郭恒的DEMO
{
    public partial class Form8 : Form
    {
        string strcon = CPublic.Var.strConn;
        string strConn_FS = CPublic.Var.geConn("FS");
        System.Data.DataTable dtM;
        public Form8()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = "select * from L临时老系统入库单打印表";
            dtM = new System.Data.DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;

        }
        //        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        //        internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);

        //        [DllImport("user32.dll", SetLastError = true)]
        //        private static extern bool BringWindowToTop(IntPtr hWnd);
        //        public static void fun_print_其他出库_A5(System.Data.DataTable dtP, int count, bool f_视图, string str_打印机, bool blPreview = false)
        //        {
        //            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\财务临时用单.xlsx";


        //            if (System.IO.File.Exists(fileName).Equals(false))
        //            {
        //                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

        //                fs.Close();
        //                System.Data.DataTable dtPP = new System.Data.DataTable();
        //                string s = "select * from 基础记录打印模板表 where 模板名 = '财务临时用单'";
        //                new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtPP);
        //                if (dtPP.Rows.Count == 0) return;

        //                System.IO.File.WriteAllBytes(fileName, (byte[])dtPP.Rows[0]["数据"]);
        //            }
        //            //string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx";
        //            ApplicationClass excelApp = new ApplicationClass();

        //            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
        //            IntPtr PID = IntPtr.Zero;
        //            GetWindowThreadProcessId(hwnd, out PID);
        //            try
        //            {
        //                Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
        //                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //                                            Type.Missing, Type.Missing);
        //                excelApp.Visible = false;
        //                excelApp.DisplayAlerts = false;
        //                Worksheet ws = (Worksheet)wb.Worksheets[1];

        //                Microsoft.Office.Interop.Excel.Range range;
        //                //编号 日期 
        //                //string sql = string.Format("select * from 销售记录成品出库单主表 where 成品出库单号 = '{0}'", dr_传.Rows[0]["成品出库单号"].ToString());  、

        ////                string sql = string.Format(@"select right(出入库申请单号,10)as 编号,申请类型,操作人员,部门 from 其他出入库申请主表,人事基础员工表 where 出入库申请单号='{0}' 
        ////                                            and  其他出入库申请主表.操作人员编号=人事基础员工表.员工号 ", str_出入库申请单号);
        ////                System.Data.DataTable dt = new System.Data.DataTable();
        ////                new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dt);
        //                range = ws.get_Range("E4", Type.Missing);
        //                range.Value2 = dtP.Rows[0]["入库单号"].ToString();  //改为仓库
        //                range = ws.get_Range("E5", Type.Missing);
        //                range.Value2 = dtP.Rows[0]["采购单号"].ToString();  //改为仓库

        //                //range = ws.get_Range("L2", Type.Missing);
        //                //range.Value2 = dtP.Rows[0]["申请类型"].ToString();

        //                range = ws.get_Range("M5", Type.Missing);
        //                range.Value2 = dtP.Rows[0]["日期"].ToString();

        //                for (int i = 1; i < count; i++)
        //                {
        //                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(i)).Copy(Type.Missing, wb.Worksheets[i]);
        //                }

        //                int pos = 1;  //记数 循环次数
        //                int i_first = 7;      // 起始行 
        //                int i_count = 14; // 每页打多少条

        //                int ir = 1;//第几张sheet                                      
        //                foreach (System.Data.DataRow r in dtP.Rows)
        //                {
        //                    range = ws.get_Range("A" + i_first.ToString(), Type.Missing);
        //                    range.Value2 = pos.ToString();

        //                    range = ws.get_Range("E" + i_first.ToString(), Type.Missing);
        //                    //range.Value2 = r["规格型号"].ToString() + r["物料名称"].ToString().Trim();
        //                    range.Value2 = r["规格型号"].ToString() + r["产品名称"].ToString().Trim();

        //                    range = ws.get_Range("C" + i_first.ToString(), Type.Missing);
        //                    range.Value2 = r["物料编号"].ToString();

        //                    range = ws.get_Range("I" + i_first.ToString(), Type.Missing);
        //                    range.Value2 = r["入库数量"].ToString();


        //                        range = ws.get_Range("H" + i_first.ToString(), Type.Missing);
        //                        range.Value2 =r["计量单位"].ToString();


        //                        range = ws.get_Range("K" + i_first.ToString(), Type.Missing);


        //                        range.Value2 = r["库位名称"].ToString();


        //                        range = ws.get_Range("M" + i_first.ToString(), Type.Missing);

        //                            range.Value2 =r["库存数量"].ToString();




        //                    //超过 icount 条 换下一个sheet
        //                    if (pos % i_count == 0 && ir != count)
        //                    {
        //                        ir++;
        //                        ws = (Worksheet)wb.Worksheets.get_Item(pos / i_count + 1);
        //                        ws.Name = "sheet-" + (pos / i_count + 1).ToString();
        //                        i_first = 6;

        //                    }
        //                    i_first = i_first + 1;
        //                    pos++;

        //                }
        //                if (blPreview)
        //                {
        //                    excelApp.Visible = true;
        //                    wb.PrintPreview();
        //                }
        //                else
        //                {
        //                    excelApp.Visible = false;
        //                    //BringWindowToTop(hwnd);
        //                    wb.PrintOutEx(Type.Missing, Type.Missing, Type.Missing, Type.Missing, str_打印机, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        //                    excelApp.Quit();
        //                }

        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //            finally
        //            {
        //                if (PID != IntPtr.Zero)
        //                {
        //                    excelApp = null;
        //                    GcCollect();
        //                    KillProcess(PID);
        //                    //System.IO.File.Delete(fileName);
        //                }
        //            }
        //        }

        //        private void button2_Click(object sender, EventArgs e)
        //        {
        //            if (MessageBox.Show("确定打印？", "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
        //            {

        //                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);

        //                string sql_1 = string.Format("select * from L临时老系统入库单打印表 where 入库单号='{0}'",dr["入库单号"].ToString());
        //                System.Data.DataTable dt_dy = new System.Data.DataTable();
        //                dt_dy = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);


        //                int count = dt_dy.Rows.Count / 14;
        //                if (dt_dy.Rows.Count % 14 != 0)
        //                {
        //                    count++;
        //                }

        //                PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
        //                this.printDialog1.Document = this.printDocument1;
        //                DialogResult drt = this.printDialog1.ShowDialog();
        //                if (drt == DialogResult.OK)
        //                {
        //                    string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
        //                   fun_print_其他出库_A5(dt_dy, count, true, PrinterName);
        //                }
        //            }
        //        }


        //        /// <summary>
        //        ///  回收垃圾
        //        /// </summary>
        //        public static void GcCollect()
        //        {
        //            GC.Collect();
        //            GC.WaitForPendingFinalizers();
        //            GC.Collect();
        //            GC.WaitForPendingFinalizers();
        //        }

        //        /// <summary>
        //        /// 杀死进程
        //        /// </summary>
        //        /// <param name="H"></param>
        //        private static void KillProcess(IntPtr H)
        //        {
        //            System.Diagnostics.Process myproc = new System.Diagnostics.Process();

        //            try
        //            {
        //                foreach (System.Diagnostics.Process thisproc in System.Diagnostics.Process.GetProcessesByName("excel"))
        //                {
        //                    if (thisproc.Id == (int)H)
        //                    {
        //                        if (!thisproc.CloseMainWindow())
        //                        {
        //                            thisproc.Kill();
        //                            System.Threading.Thread.Sleep(1000);
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                System.Console.WriteLine(ex.Message);
        //            }
        //        }

        private void button3_Click(object sender, EventArgs e)
        {
            string ss = "select  * from 基础数据基础属性表 where 属性类别='岗位'  order by POS ";
            using (SqlDataAdapter da = new SqlDataAdapter(ss, strcon))
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    dr["属性字段1"] = Convert.ToInt32(dr["POS"]).ToString("0000");

                }
                new SqlCommandBuilder(da);
                da.Update(dt);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {


           // /StockCore.StockCorer.fun_物料数量_实际数量(textBox1.Text, true);



        }
        //public void toExcel(System.Data.DataTable data, string output)
        //{
        //    try
        //    {
        //        //创建工作薄  
        //        HSSFWorkbook workbook = new HSSFWorkbook();
        //        //创建一个表sheet  
        //        ISheet sheet = workbook.CreateSheet("sheet");
        //        //创建第一行,新创建的表是没有单元格的,每一个需要写入数据的单元格都要手动创建  
        //        IRow row = sheet.CreateRow(0);
        //        //将列名写入表的第一行  
        //        for (int i = 0; i < data.Columns.Count; i++)
        //        {
        //            row.CreateCell(i);
        //            sheet.GetRow(0).GetCell(i).SetCellValue(data.Columns[i].ColumnName);
        //        }
        //        //写入数据  
        //        for (int i = 1; i <= data.Rows.Count; i++)
        //        {
        //            row = sheet.CreateRow(i);
        //            for (int j = 0; j < data.Columns.Count; j++)
        //            {
        //                row.CreateCell(j);
        //                sheet.GetRow(i).GetCell(j).SetCellValue(data.Rows[i - 1][j].ToString());
        //            }
        //        }
        //        FileStream file = new FileStream(output, FileMode.Create);
        //        workbook.Write(file);
        //        file.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        private void button5_Click(object sender, EventArgs e)
        {
           // DataTable t = new DataTable();
            //t.Columns.Add("父项编号");
            //t.Columns.Add("产品线");


            //t = ERPorg.Corg.fun_运算_成品(t, "C11075", "");

            //string s = t.Rows[t.Rows.Count - 1]["父项编号"].ToString();
            ////Assembly outerAsm = Assembly.LoadFrom(Path.Combine(System.Windows.Forms.Application.StartupPath,"ERPSale.dll"));  //  ERPproduct.dll
            ////Type outerForm = outerAsm.GetType("ERPSale.ui销售出库扫码关联工单", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
            ////// Form ui = Activator.CreateInstance(outerForm) as Form;
            ////UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
            ////CPublic.UIcontrol.Showpage(ui,"ces");
            ////  Dictionary <int,int>dic =new Dictionary<int,int>();
            ////Random ran = new Random();
            ////for(int j=0;j<100;j++)
            ////{

            //// int i = ran.Next(5);
            //// if (i == 0 || i == 5)
            //// {
            ////     dic.Add(j,i);

            //// }

            ////}
            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.Title = "导出Excel";
            //saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            //DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            //if (dialogResult == DialogResult.OK)
            //{
            //    DevExpress.XtraPrinting.XlsExportOptions options = new DevExpress.XtraPrinting.XlsExportOptions();



            //    // DevExpress.XtraGrid.GridControl gc = (ActiveControl) as DevExpress.XtraGrid.GridControl;

            //    gridControl1.ExportToXlsx(saveFileDialog.FileName);



            //    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //}

        }

        private void button6_Click(object sender, EventArgs e)
        {
            ERPorg.Corg x = new ERPorg.Corg();
            //遍历 然后 存成EXCEL
            foreach (DataRow dr in dtM.Rows)
            {
                //取需打印数据,存成EXCEl 上传
                string ss = string.Format(@"select  a.检验记录单号,base.原ERP物料编号,base.小类,base.物料名称,base.图纸编号,s.原ERP物料编号 as 父项编号,s.物料名称 as 父项名称,s.n原ERP规格型号 as 父项规格,mx.* from 采购记录采购单检验主表 a
                left join 基础数据物料信息表 base   on a.产品编号=base.物料编码  
                left join  (select   max(产品编码)父项编码,子项编码 from  基础数据物料BOM表 group by 子项编码)b on b.子项编码=a.产品编号
                left join 基础数据物料信息表 s on s.物料编码=b.父项编码
                left join 采购记录采购单检验明细表 mx on mx.检验记录单号=a.检验记录单号
                where 产品编号='{0}' and left(检验项目,2)='尺寸' and 检验日期=(select  MAX(检验日期) from 采购记录采购单检验主表 b where b.产品编号 =a.产品编号)", dr["物料编码"].ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(ss, strcon))
                {
                    System.Data.DataTable dt = new System.Data.DataTable();
                    da.Fill(dt);
                    //调用生成文件函数  参数 dt,dr
                    ItemInspection.print_FMS.fun_生成检验标准(dt, dr);
                    //文件名: 原ERP物料编号+"-"+小类
                    string root = System.Windows.Forms.Application.StartupPath + "\\品质检验标准\\";
                    string path = root + dr["原ERP物料编号"].ToString() + "_" + dr["小类"].ToString() + ".xlsx";
                    //CFileTransmission.CFileClient.sendFile();

                    fun_文件上传(path, dr);
                    string path2 = root + dr["原ERP物料编号"].ToString() + "_" + dr["小类"].ToString() + ".pdf";

                    x.ConverterToPdf(path, path2);
                    fun_文件上传(path2, dr);


                }

            }
        }
        /// <summary>
        /// 文件上传的方法
        /// </summary>
        private void fun_文件上传(string pathName, DataRow r)
        {
            //判定上传文件的大小

            FileInfo info = new FileInfo(pathName);
            long maxlength = info.Length;

            if (maxlength > 1024 * 1024 * 8)
            {
                throw new Exception("上传的文件不能超过1M，请重新选择后再上传！");
            }

            MasterFileService.strWSDL = CPublic.Var.strWSConn;
            CFileTransmission.CFileClient.strCONN = strConn_FS;
            string strguid = "";  //记录系统自动返回的GUID

            strguid = CFileTransmission.CFileClient.sendFile(pathName);
            string type = "";

            int s = pathName.LastIndexOf(".") + 1;
            type = pathName.Substring(s, pathName.Length - s);
            string sql = "select  * from  [品质检验标准文件表] where 1<>1 ";
            System.Data.DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            DataRow dr = dt.NewRow();
            dr["物料号"] = r["原ERP物料编号"];
            dr["文件名"] = Path.GetFileName(pathName); ;
            dr["后缀"] = type;
            dr["文件地址"] = strguid;
            dr["小类"] = r["小类"];
            dt.Rows.Add(dr);

            CZMaster.MasterSQL.Save_DataTable(dt, "品质检验标准文件表", strcon);




            //    dtP.Rows.Add(strygh, r["文件名称"].ToString(), strguid, Path.GetFileName(pathName));
            // MasterSQL.Save_DataTable(dtP, "人事基础员工文件表", CPublic.Var.strConn);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string s = string.Format(@"select  * from (
                select   b.物料编码,b.图纸编号,b.物料名称,b.原ERP物料编号,大类,小类 from 采购记录采购单检验主表  a
        left join  基础数据物料信息表 b   on a.产品编号=b.物料编码   where 大类='{0}' and 小类='{1}' and 检验日期>'2017-1-1'
        group by b.物料编码,b.图纸编号,b.物料名称,b.原ERP物料编号,大类,小类 
       )x  where 原ERP物料编号  not in (select  物料号 from  品质检验标准文件表 ) ", textBox2.Text.Trim(), textBox3.Text.Trim());
            using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
            {
                dtM = new System.Data.DataTable();
                da.Fill(dtM);
                gridControl2.DataSource = dtM;
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            ERPorg.Corg x = new ERPorg.Corg();
            string path = System.Windows.Forms.Application.StartupPath + "\\品质检验标准";
            DirectoryInfo TheFolder = new DirectoryInfo(path);

            //遍历文件
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                string s = System.IO.Path.GetFileNameWithoutExtension(NextFile.Name);

                x.ConverterToPdf(NextFile.FullName, path + "\\" + s + ".pdf");

            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string s = string.Format(@"select  * from  品质检验标准文件表 where 小类='{0}' ", textBox3.Text.Trim());
            using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
            {
                dtM = new System.Data.DataTable();
                da.Fill(dtM);
                gridControl2.DataSource = dtM;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            MasterFileService.strWSDL = CPublic.Var.strWSConn;
            CFileTransmission.CFileClient.strCONN = strConn_FS;
            foreach (DataRow dr in dtM.Rows)
            {
                string path = @"E:\FCFiles\" + dr["文件地址"].ToString();

                CFileTransmission.CFileClient.deleteFile(path);

            }
        }

        public static void setWatermark(string inputfilepath, string outputfilepath, string waterMarkName)
        {
            PdfReader pdfReader = null;
            PdfStamper pdfStamper = null;
            pdfReader = new PdfReader(inputfilepath);
            pdfStamper = new PdfStamper(pdfReader, new FileStream(outputfilepath, FileMode.Create));
            int total = pdfReader.NumberOfPages + 1;
            iTextSharp.text.Rectangle psize = pdfReader.GetPageSize(1);
            float width = psize.Width;
            float height = psize.Height;
            BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\simhei.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);//获取系统的字体 
            iTextSharp.text.Font font = new iTextSharp.text.Font(baseFont, 10);
            Phrase p = new Phrase("生产批号：" + waterMarkName, font);
            for (int i = 1; i < total; i++)
            {
                PdfContentByte over = pdfStamper.GetOverContent(i);//PdfContentBye类，用来设置图像和文本的绝对位置  
                ColumnText.ShowTextAligned(over, Element.ALIGN_CENTER, p, 10, 50, 90);
            }
            pdfStamper.Close();

        }

        private void button11_Click(object sender, EventArgs e)
        {



            //  PdfReader reader = new PdfReader(@"D:\futureERP\BIN\测试\通讯录20170315.pdf");  
            //  iTextSharp.text.Rectangle psize = reader.GetPageSize(1);      //获取第一页 
            //  Document doc = new Document(psize, 50, 50, 50, 50);
            //  PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream("通讯录20170315.pdf", FileMode.Open));

            //  doc.OpenDocument();
            ////  iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(@"D:\futureERP\BIN\测试\受控.png");           //插入图片
            //  Image jpeg = Image.GetInstance(@"D:\futureERP\BIN\测试\受控.png");
            //  jpeg.SetAbsolutePosition(50, 100);
            //  doc.Add(jpeg);
            //  //writer.DirectContent.AddImage(img);         //添加图片   

            // doc.Close();

            PdfReader reader = new PdfReader(@"D:\futureERP\BIN\测试\采购单.pdf");
            iTextSharp.text.Rectangle psize = reader.GetPageSize(1);      //获取第一页 
            PdfStamper pdfStamper = new PdfStamper(reader, new FileStream(@"D:\futureERP\BIN\测试\采购审核.pdf", FileMode.Create));
            Document doc = new Document();
            //  PdfWriter.GetInstance(doc, new FileStream(@"D:\futureERP\BIN\测试\通讯录20170315.pdf", FileMode.Open));
            doc.Open();
            Image gif = Image.GetInstance(@"D:\futureERP\BIN\测试\宋霞.jpg");
            Image gif1 = Image.GetInstance(@"D:\futureERP\BIN\测试\徐惠兴.jpg");

            int total = reader.NumberOfPages;
            for (int i = 1; i <= total; i++)
            {
                PdfContentByte waterMarkContent;
                waterMarkContent = pdfStamper.GetOverContent(i);
                //gif.WidthPercentage = 0;
                gif.ScalePercent(15f);
                gif.SetAbsolutePosition(95, 88);
                // gif.SetAbsolutePosition(90, 20);

                waterMarkContent.AddImage(gif);

                // gif1.WidthPercentage = 0;
                gif1.ScalePercent(15f);
                gif1.SetAbsolutePosition(315, 85);
                waterMarkContent.AddImage(gif1);
            }
            doc.Close();
            pdfStamper.Close();
            reader.Close();

        }
        public void SendMail()
        {
            MailMessage mailmsg = null;
            SmtpClient client = null;
            //创建一个身份凭证，即发送邮件的用户名和密码
            //发送邮件的实例，服务器和端口
            client = new SmtpClient("smtp.exmail.qq.com", 25);
            client.EnableSsl = false;
            client.UseDefaultCredentials = false;
            NetworkCredential credential = new NetworkCredential("hguo@szfuture.com", "Guo123456");

            //发送邮件的方式，通过网络发送
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //是否启用 SSL 

            //指定发送邮件的身份凭证
            client.Credentials = credential;

            //发送的邮件信息
            mailmsg = new MailMessage();

            // 指定发件人邮箱和显示的发件人名称
            mailmsg.From = new MailAddress("hguo@szfuture.com");

            // 指定收件人邮箱
            MailAddress mailto = new MailAddress("hgu@szfuture.com");

            //mailmsg.CC.Add(mailto);     // 抄送


            //mailmsg.Bcc.Add(mailto);    // 密送

            mailmsg.To.Add(mailto);     // 默认发送


            //邮件主题
            mailmsg.Subject = "测试邮件";
            mailmsg.SubjectEncoding = Encoding.UTF8;

            //邮件内容
            mailmsg.Body = "这是一封测试邮件勿回";
            mailmsg.BodyEncoding = Encoding.UTF8;

            //添加附件
            string url = @"D:\dwfergewfwregergergreg";    // 附件地址
            byte[] bts = System.IO.File.ReadAllBytes(url);
            File.WriteAllBytes(@"E:\服务\采购邮件\采购审核单.pdf", bts);
            url = @"E:\服务\采购邮件\采购审核单.pdf";
            DirectoryInfo rt = new DirectoryInfo(url);
            if (!rt.Exists) rt.Create();

            string name = "采购审核单";   // 附件名称

            mailmsg.Attachments.Add(new Attachment(@url));   // 本地路径可直接加载


            client.Send(mailmsg);   // 发送邮件
            //  UpdateState(Id, "1");   // 发送成功修改发送状态为 1

        }
        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                SendMail();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }





        }
        //
        private void button13_Click(object sender, EventArgs e)
        {
           // ApplicationClass excelApp = new ApplicationClass();
           // string fileName = @"D:\futureERP\BIN\ApplyTemp\采购单";
           // IntPtr hwnd = new IntPtr(excelApp.Hwnd);
           // IntPtr PID = IntPtr.Zero;
           //// GetWindowThreadProcessId(hwnd, out PID);
           // try
           // {

           //     Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
           //                                 Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
           //                                 Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
           //                                 Type.Missing, Type.Missing);

           //     excelApp.Visible = false;
           //     excelApp.ScreenUpdating = false;
           //     excelApp.DisplayAlerts = false;

          

           //     int  count= wb.Worksheets.Count;
           //     for (int i=1;i<=count;i++)
           //     {
           //       Worksheet ws = (Worksheet)wb.Worksheets[i];
                 
           //         Microsoft.Office.Interop.Excel.Range range;
           //         string path_pic1=@"C:\Users\Administrator\Desktop\数字签名\褚伟良.jpg";
           //         string path_pic2=@"C:\Users\Administrator\Desktop\数字签名\毛华芳.jpg";

           //         ws.Shapes.AddPicture(path_pic1,Microsoft.Office.Core.MsoTriState.msoFalse,Microsoft.Office.Core.MsoTriState.msoTrue ,400,902,75,46);
           //         ws.Shapes.AddPicture(path_pic2, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue,100,902, 75, 46);




           //     }

           //     wb.SaveAs(fileName);
           
           //     //excelApp.Visible = false;
           //     //BringWindowToTop(hwnd);


           //     excelApp.DisplayAlerts = false;
           //    excelApp.Quit();
           //    GcCollect();
             
           // }
           // catch(Exception ex )
           // {

           //     MessageBox.Show(ex.Message);
           // }
        }

        /// <summary>
        ///  回收垃圾
        /// </summary>
        public static void GcCollect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }





    }
}
