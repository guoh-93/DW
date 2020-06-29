using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using LabelPrint;
using LabelManager2;

namespace LabelPrint
{


    /// <summary>
    /// 打印。
    /// 我们目前对打印功能是这么定义的。
    /// 每次最多打印100份。打完了继续切任务
    /// 多个打印作业错时运行。
    /// </summary>
    class LPrinter
    {
        /// <summary>
        /// 不能被修改常数=10
        /// </summary>
        const int ISETPMAX = 10;

        public int Left = 0;

        public int Top = 0;

        ApplicationClass applicationClass;
        /// <summary>
        /// 错误提示字段
        /// </summary>
        public string str_ErrMsg = "";
        /// <summary>
        /// 当前状态字段
        /// </summary>
        public string str_Log = "";
        /// <summary>
        /// 打印模板路径地址
        /// </summary>
        string FiledPath;
        /// <summary>
        /// 标签打印记录信息
        /// 字段名称,字段内容
        /// </summary>
        Dictionary<string, string> dic;
        List<Dictionary<string, string>> list;
        /// <summary>
        /// 打印机名称
        /// </summary>
        string PrinterName;
        /// <summary>
        /// 打印数量
        /// </summary>
        int PrintCount;
  
        Thread thDo;

        Boolean blCancle = false;
        /// <summary>
        /// 打印机当前是否可以打印_默认为false
        /// </summary>
        Boolean _blOver = false;
        public Boolean isOver
        {
            get { return _blOver; }
        }
        /// <summary>
        /// 重构
        /// </summary>
        /// <param name="_FiledPath"></param>
        /// <param name="_dic"></param>
        /// <param name="_PrinterName"></param>
        /// <param name="_PrintCount"></param>
        public LPrinter(string _FiledPath, Dictionary<string, string> _dic, string _PrinterName, int _PrintCount)
        {
            FiledPath = _FiledPath;
            dic = _dic;
            PrinterName = _PrinterName;
            PrintCount = _PrintCount;
            applicationClass = new ApplicationClass();
            applicationClass.Documents.Open(FiledPath, false);//打开模板（地址,只读_默认为false）
        }
        public LPrinter(string _FiledPath, List<Dictionary<string, string>> _list, string _PrinterName, int _PrintCount)
        {
            FiledPath = _FiledPath;
            list = _list;
            PrinterName = _PrinterName;
            PrintCount = _PrintCount;
            applicationClass = new ApplicationClass();
            applicationClass.Documents.Open(FiledPath, false);//打开模板（地址,只读_默认为false）
        }
        
        public void Cancel()
        {
            if (_blOver == false)
            {
                applicationClass.Quit();
                GC.Collect();
                str_Log = string.Format("{0},打印取消", PrinterName);
            }
        }
        /// <summary>
        /// 开始多线程
        /// </summary>
        public void Start()
        {
            thDo = new Thread(DoWork);
            thDo.Start();
        }
        public void DoWork()
        {
            LabelManager2.Document document = applicationClass.ActiveDocument;

            try
            {

                if (list != null)
                {
                    document.Printer.SwitchTo(PrinterName, "", true);
                    for (int j = 0; j < list.Count; j++)
                    {
                        dic = list[j];

                        foreach (KeyValuePair<string, string> current in dic)
                        {

                            bool flag = false;

                            for (int i = 1; i <=(int)document.Variables.FormVariables.Count; i++)
                            {
                              //  string s = document.Variables.FormVariables.Item(i).Name.ToString().ToLower().Trim();

                                if (current.Key.ToString().ToLower().Trim() == document.Variables.FormVariables.Item(i).Name.ToString().ToLower().Trim())
                                {

                                    flag = true;
                                }
                            }
                            if (flag)
                            {
                                document.Variables.FormVariables.Item(current.Key).Value = current.Value;

                            }
                        }
                        // 

                        document.PrintDocument(PrintCount);

                    }

                }
                else
                {
                    foreach (KeyValuePair<string, string> current in dic)
                    {
                        bool flag = false;
                        for (int i = 1; i <=(int)document.Variables.FormVariables.Count; i++)
                        {
                            if (current.Key.ToString().ToLower().Trim() == document.Variables.FormVariables.Item(i).Name.ToString().ToLower().Trim())
                            {
                                
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            document.Variables.FormVariables.Item(current.Key).Value = current.Value;
                        }
                    }
                    document.Printer.SwitchTo(PrinterName, "", true);

                    document.PrintDocument(PrintCount);
                }

                //document.Printer.SwitchTo(PrinterName,"",true);

                // document.PrintDocument(PrintCount);

                //System.Threading.Thread.Sleep(1500);


                document.Close(false);

                _blOver = true;
                
               
            }
            catch (Exception ex)
            {
                string file = @"C:\\errorlog.txt";

                if (File.Exists(file) == true)
                {

                    using (StreamWriter SW = File.AppendText(file))
                    {
                        SW.WriteLine(ex);
                        SW.Close();
                    }
                }
                else
                {
                    FileStream myFs = new FileStream(file, FileMode.Create);
                    StreamWriter mySw = new StreamWriter(myFs);
                    mySw.Write(ex);
                    mySw.Close();
                    myFs.Close();
                }

                str_ErrMsg += ex.Message;
            }
            finally
            {
                try
                {
                    applicationClass.Documents.CloseAll(false);
                    applicationClass.Quit();
                    GC.Collect();
                }
                catch
                {

                }
            }
        }
    }


}
