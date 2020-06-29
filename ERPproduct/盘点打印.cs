using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using NPOI.HSSF.UserModel;
//using NPOI.HPSF;
//using NPOI.POIFS.FileSystem;
//using NPOI.Util;
//using System.IO;
//using NPOI.XSSF.UserModel;


//using NPOI.SS.UserModel;

namespace ERPproduct
{
    public partial class 盘点打印 : Form
    {
        public 盘点打印()
        {
            InitializeComponent();
        }
        string filepath = "";
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            //HSSFWorkbook wb;
          
            //wb = new HSSFWorkbook();

            //HSSFSheet sheet;
            //FileStream fs = System.IO.File.OpenRead(filepath);
            //NPOI.SS.UserModel.IWorkbook workbook = NPOI.SS.UserModel.WorkbookFactory.Create(filepath);

        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //HSSFWorkbook wb;
            //FileStream file;
            //file = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            //wb = new HSSFWorkbook(file);
            //file.Close();

           


        }
        bool isprint;
        string newsavefilepath = "C:/Users/10294/Desktop/盘点表.xlsx";
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            isprint = false;
            barLargeButtonItem2_ItemClick(sender, e); //使用NPOI生成excel
            if (newsavefilepath != "" && isprint == true)
            {
                isprint = false;
                //ChangeExcel2Image(newsavefilepath);  //利用Spire将excel转换成图片
                if (printDialog1.ShowDialog() == DialogResult.OK)
                {
                   
                    printDocument1.Print();   //打印
                }
            } 
        }




        //public void ChangeExcel2Image(string filename)
        //        {
        //            Workbook workbook = new Workbook();
        //            workbook.LoadFromFile(filename);
        //            Worksheet sheet = workbook.Worksheets[0];
        //            sheet.SaveToImage(imagepath); //图片后缀.bmp ,imagepath自己设置
        //  }





        //public static DataTable ReadExcelToDataTable(string fileName, string sheetName = null, bool isFirstRowColumn = true)
        //{
        //    //定义要返回的datatable对象
        //   // DataTable data = new DataTable();
        //   // //excel工作表
        //   //// NPOI.SS.UserModel.ISheet sheet = null;
        //   // //数据开始行(排除标题行)
        //   // int startRow = 1;
        //   // try
        //   // {
        //   //     if (!File.Exists(fileName))
        //   //     {
        //   //         return null;
        //   //     }
        //   //     //根据指定路径读取文件
        //     //  FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        //     FileStream fs = System.IO.File.OpenRead(fileName);

        //   //     //根据文件流创建excel数据结构
        //   //    // NPOI.SS.UserModel.IWorkbook workbook = NPOI.SS.UserModel.WorkbookFactory.Create(fs);
        //   //     //NPOI.SS.UserModel.IWorkbook workbook = null;
        //   //     //try
        //   //     //{
        //   //     //    workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(fs);

        //   //     //}
        //   //     //catch
        //   //     //{
        //   //     //    fs.Close();
        //   //     //    fs.Dispose();
        //   //     //    workbook = new NPOI.HSSF.UserModel.HSSFWorkbook(fs);
        //   //     //}
        //   //     //IWorkbook workbook = new HSSFWorkbook(fs);
        //   //     //如果有指定工作表名称
        //   //     if (!string.IsNullOrEmpty(sheetName))
        //   //     {
        //   //         sheet = workbook.GetSheet(sheetName);
        //   //         //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
        //   //         if (sheet == null)
        //   //         {
        //   //             sheet = workbook.GetSheetAt(0);
        //   //         }
        //   //     }
        //   //     else
        //   //     {
        //   //         //如果没有指定的sheetName，则尝试获取第一个sheet
        //   //         sheet = workbook.GetSheetAt(0);
        //   //     }
        //   //     if (sheet != null)
        //   //     {
        //   //         NPOI.SS.UserModel.IRow firstRow = sheet.GetRow(0);
        //   //         //一行最后一个cell的编号 即总的列数
        //   //         int cellCount = firstRow.LastCellNum;
        //   //         //如果第一行是标题列名
        //   //         if (isFirstRowColumn)
        //   //         {
        //   //             for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
        //   //             {
        //   //                 NPOI.SS.UserModel.ICell cell = firstRow.GetCell(i);
        //   //                 if (cell != null)
        //   //                 {
        //   //                     string cellValue = cell.StringCellValue;
        //   //                     if (cellValue != null)
        //   //                     {
        //   //                         DataColumn column = new DataColumn(cellValue);
        //   //                         data.Columns.Add(column);
        //   //                     }
        //   //                 }
        //   //             }
        //   //             startRow = sheet.FirstRowNum + 1;
        //   //         }
        //   //         else
        //   //         {
        //   //             startRow = sheet.FirstRowNum;
        //   //         }
        //   //         //最后一列的标号
        //   //         int rowCount = sheet.LastRowNum;

        //   //         for (int i = startRow; i <= rowCount; ++i)
        //   //         {


        //   //             NPOI.SS.UserModel.IRow row = sheet.GetRow(i);
        //   //             if (row == null) continue; //没有数据的行默认是null　　　　　　　
        //   //             if (row.FirstCellNum < 0) continue;
        //   //             DataRow dataRow = data.NewRow();
        //   //             for (int j = row.FirstCellNum; j < cellCount; ++j)
        //   //             {
        //   //                 if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
        //   //                     dataRow[j] = row.GetCell(j).ToString();
        //   //             }
        //   //             data.Rows.Add(dataRow);
        //   //         }
        //   //         DataTable dtp = data.Copy();
        //   //         DataRow drg = dtp.Rows[0];
        //   //         data.Columns.Remove("盘点时间");
        //   //         data.Columns.Remove("盘点人");
        //   //         //for(int i=0; i<=data.Rows.Count+1;i++){
        //   //         //    if(i==data.Rows.Count+1){

        //   //         DataRow dgg = data.NewRow();
        //   //         data.Rows.Add(dgg);
        //   //         dgg["仓库"] = "盘点时间：";
        //   //         dgg["货架描述"] = drg["盘点时间"].ToString();
        //   //         dgg["盘前库存"] = "盘点人：";
        //   //         dgg["盘后库存"] = drg["盘点人"].ToString();
        //   //         dgg["偏差值"] = "监盘人：";


        //   //         //sheet.GetRow(i).GetCell(1).SetCellValue("盘点时间");
        //   //         //sheet.GetRow(i).GetCell(2).SetCellValue(drg["盘点时间"].ToString());
        //   //         //     sheet.GetRow(i).GetCell(5).SetCellValue("盘点人");
        //   //         //sheet.GetRow(i).GetCell(6).SetCellValue(drg["盘点人"].ToString());
        //   //         // sheet.GetRow(i).GetCell(7).SetCellValue(drg["监盘人"].ToString());
        //   //         //}
        //   //         //}
        //   //         //foreach( DataRow dr in data.Rows ){
        //   //         //    if(dr ){
        //   //         //    }
        //   //         // }
        //   //     }




        //   //     return data;
        //   // }
        //   // catch (Exception ex)
        //   // {
        //   //     throw ex;
        //   // }
        //}

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
    {
        CreateExcel(); //使用NPOI生成excel内容
        SaveFileDialog savedialog = new SaveFileDialog(); //弹出让用户选择excel保存路径的窗口
        savedialog.Filter = " excel files(*.xlsx)|*.xlsx|All files(*.*)|*.*";
        savedialog.RestoreDirectory = true;
        savedialog.FileName = string.Format("销售订单审批单{0}", DateTime.Now.ToString("yyyyMMddHHmm"));
        if (savedialog.ShowDialog() == DialogResult.OK)
        {
            //newsavefilepath是excel的保存路径
            newsavefilepath = savedialog.FileName.ToString().Trim();
            //using (FileStream newfs = new FileStream(newsavefilepath, FileMode.Create, FileAccess.ReadWrite))
            //{
            //    // singlexssfwk.Write(newfs); //将生成的excel写入用户选择保存的文件路径中
            //    newfs.Close();
            //}
        } 
    }

    private void CreateExcel()
    {
        //获取模板excel的路径
    //    string str = System.Environment.CurrentDirectory + "\\XXXX.xlsx";
    //    if (File.Exists(str))
    //    {
    //        using (FileStream fs = new FileStream(str, FileMode.Open, FileAccess.Read))
    //        {
    //            singlexssfwk = new XSSFWorkbook(fs);
    //            fs.Close();
    //        }
    //        获取表
    //        XSSFSheet xssfsheet = (XSSFSheet)singlexssfwk.GetSheetAt(0);
    //        创建行 
    //        XSSFRow xssfrow1 = (XSSFRow)xssfsheet.GetRow(1);
    //        设置单元格内容
    //        xssfrow1.GetCell(0).SetCellValue("...");
    //        ... ...
    //    }
    //    else
    //    {
    //         ... ...
    //    }
    }

    private void 盘点打印_Load(object sender, EventArgs e)
    {
      //  printPreviewDialog1.Document = printDocument1; 
        //printPreviewDialog1.ShowDialog();
    }


//XSSFWorkbook singlexssfwk;  

//注意，不同的NPOI版本调用的方法不一致，这里使用的版本是2.1.3.1
private void CreatExcel() 
      { 
           
   }
DataTable dt;

#pragma warning disable IDE1006 // 命名样式
private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
{

}
//private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
//{

    // dt = ReadExcelToDataTable(newsavefilepath);
    //Entity[] list = getData();
    //Entity在这里是一个实体集合，主要用于承载数据，你想用什么装数据都行
    //getData()是一个获取数据的方法，我是直接从数据库查出来的，这里就不给大家看了
 
    //这里，对这个刚刚获取的数据承载体进行一下判空什么的，我这里也不写了

    //下面这是对电脑中文件夹的操作和导出数据没有直接关系，但是我还是想给大家看一下
   
    
    
    
    
    }
}
