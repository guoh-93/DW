using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.Util;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Drawing.Printing;
namespace ERPproduct
{
    public partial class frm盘点打印 : UserControl
    {
        public frm盘点打印()
        {
            InitializeComponent();
        }

      //  XSSFWorkbook singlexssfwk;  
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {


 
          
           
    
            //DataTable dt = ERPorg.Corg.Read盘点ToDataTable(textBox1.Text.ToString());

            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            //获取文件的路径，此路径为该程序的bin\debug下，注意我的用词是debug下，
            //言下之意是debug后面还有一个“\”，客官们还可以自行百度，获取其他相关路径

            //根据项目需求，我需要将文件导出到debug下的一个文件夹中，客官可根据自身需求修改
            //判断是否存在这么一个文件夹，没有的话，就创建一下。
            if (!Directory.Exists(filePath + "我是一个文件夹"))
            {
                Directory.CreateDirectory(filePath + "我是一个文件夹");
            }

            using (Stream fs = new FileStream(filePath + @"我是一个文件夹\我是一个文件夹.xlsx", FileMode.Create, FileAccess.Write))
            {



                string newsavefilepath = "";


                XSSFWorkbook singlexssfwk;
                //注意，不同的NPOI版本调用的方法不一致，这里使用的版本是2.1.3.1

                //获取模板excel的路径
                string str = System.Environment.CurrentDirectory + "\\盘点表.xlsx";
                if (File.Exists(str))
                {
                    using (FileStream fsg= new FileStream(str, FileMode.Open, FileAccess.Read))
                    {
                        singlexssfwk = new XSSFWorkbook(fsg);
                        fsg.Close();
                    }
                    //获取表
                    XSSFSheet xssfsheet = (XSSFSheet)singlexssfwk.GetSheetAt(0);

                    DataTable dt = ERPorg.Corg.Read盘点ToDataTable("C:/Users/10294/Desktop/盘点表.xlsx");
                    //创建行 

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        XSSFRow xssfrow1 = (XSSFRow)xssfsheet.GetRow(i);
                        DataRow drr = dt.Rows[i];
                        //设置单元格内容
                        xssfrow1.GetCell(0).SetCellValue(drr["仓库"].ToString());
                        xssfrow1.GetCell(1).SetCellValue(drr["货架描述"].ToString());
                        // xssfrow1.GetCell(2).SetCellValue(drr["物料编号"].ToString());
                        xssfrow1.GetCell(3).SetCellValue(drr["物料名称"].ToString());
                        xssfrow1.GetCell(4).SetCellValue(drr["盘前库存"].ToString());
                        xssfrow1.GetCell(5).SetCellValue(drr["盘后库存"].ToString());
                        // xssfrow1.GetCell(6).SetCellValue(drr["偏差值"].ToString());


                    }
                    singlexssfwk.Write(fs);
                    string s = fs.ToString();


                }
            



                //XSSFWorkbook work = new XSSFWorkbook();
                ////创建一个文件

                //ISheet sheet = work.CreateSheet("我是一个表单名");
                ////创建一个sheet，并命名


                ////添加表头 数 据
                //IRow row = sheet.CreateRow(0);
                ////创建sheet中的一行,0表示第一行

                ////下面的cell是指单元格，表示上面刚刚创建的那一行中的单元格
                ////其中0,1,2表示，这一行横着数第几个单元格，索引从0开始
                //ICell cell1 = row.CreateCell(0, CellType.STRING);
                //cell1.SetCellValue("仓库");
                //ICell cell2 = row.CreateCell(1, CellType.STRING);
                //cell2.SetCellValue("货架描述");
                //ICell cell3 = row.CreateCell(2, CellType.STRING);
                //cell3.SetCellValue("物料编号");

                //ICell cell4 = row.CreateCell(3, CellType.STRING);
                //cell4.SetCellValue("物料名称");
                //ICell cell5 = row.CreateCell(4, CellType.STRING);
                //cell5.SetCellValue("盘前库存");
                //ICell cell6 = row.CreateCell(5, CellType.STRING);
                //cell6.SetCellValue("盘后库存");
                //ICell cell7 = row.CreateCell(6, CellType.STRING);
                //cell7.SetCellValue("偏差值");


                ////添加数据，相关注释参考 表头
                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    DataRow drr = dt.Rows[i];
                //    row = sheet.CreateRow(i + 1);

                //    cell1 = row.CreateCell(0, CellType.STRING);
                //    cell1.SetCellValue(drr["仓库"].ToString());

                //    cell2 = row.CreateCell(1, CellType.STRING);
                //    cell2.SetCellValue(drr["货架描述"].ToString());

                //    cell3 = row.CreateCell(2, CellType.STRING);
                //    cell3.SetCellValue(drr["物料编号"].ToString());

                //    cell4 = row.CreateCell(3, CellType.STRING);
                //    cell4.SetCellValue(drr["物料名称"].ToString());

                //    cell5 = row.CreateCell(4, CellType.STRING);
                //    cell5.SetCellValue(drr["盘前库存"].ToString());

                //    cell6 = row.CreateCell(5, CellType.STRING);
                //    cell6.SetCellValue(drr["盘后库存"].ToString());


                //    cell7 = row.CreateCell(6, CellType.STRING);
                //    cell7.SetCellValue(drr["偏差值"].ToString());



                //}/ (newfs); 
             

            }




            //ERPorg.Corg.ChangeExcel2Image(filePath + @"盘点数据\盘点表da.xlsx");  //利用Spire将excel转换成图片
            //    if (printDialog1.ShowDialog() == DialogResult.OK)
            //    {
            //        printDocument1.Print();   //打印
            //    }
            



}

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
             ERPorg.Corg.ChangeExcel2Image("C:/Users/10294/Desktop/盘点表.xlsx");  //利用Spire将excel转换成图片
                if (printDialog1.ShowDialog() == DialogResult.OK)
                {
                    printDocument1.Print();   //打印
                }
            
            
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (textBox1.Text.ToString() == "")
                {
                    throw new Exception("请输入文件地址");
                }
                string path = ERPorg.Corg.fun(textBox1.Text.ToString());
                ERPorg.Corg.ChangeExcel2Image(path);  //利用Spire将excel转换成图片
                if (printDialog1.ShowDialog() == DialogResult.OK)
                {


                    printDocument1.Print();   //打印
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

           

          

            
        }
      
   
#pragma warning disable IDE1006 // 命名样式
protected void btnPrint_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
{
        
}
private void PrintPage(object o, PrintPageEventArgs e)
{
    
}
#pragma warning disable IDE1006 // 命名样式
        private void printDocument1_PrintPage_1(object sender, System.Drawing.Printing.PrintPageEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(@"D:\DWerp\BIN\我是一个文件夹名\.jpg");
            Point loc = new Point(100, 100);
            e.Graphics.DrawImage(img, loc);     
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {


            ERPorg.Corg.ChangeExcel2Image(@"D:\DB\销售订单审批单201812211351.xlsx");  //利用Spire将excel转换成图片
         

           

        }
 

    }
}
