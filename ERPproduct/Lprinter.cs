using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using LabelManager2;




namespace ERPproduct
{
    public class Lprinter 
    {
        /// <summary>
        /// 2016/ 5/3 改过
        /// </summary>
        //const int ISETPMAX = 10;
        const int ISETPMAX = 12;
        public int Left = 0;

        public int Top = 0;

        LabelManager2.ApplicationClass applicationClass;

        public string str_ErrMsg = "";
        public string str_Log = "";
        string FiledPath;
        Dictionary<string, string> dic;
        List<Dictionary<string, string>> list;
        Dictionary<Dictionary<string, string>, int> dic_plus;
        string PrinterName;
        int PrintCount;

        Thread thDo;



        Boolean blCancle = false;
        Boolean _blOver = false;
#pragma warning disable IDE1006 // 命名样式
        public Boolean isOver
#pragma warning restore IDE1006 // 命名样式
        {
            get { return _blOver; }
        }

        public Lprinter(string _FiledPath, Dictionary<string, string> _dic, string _PrinterName, int _PrintCount)
        {
            FiledPath = _FiledPath;
            dic = _dic;
            PrinterName = _PrinterName;
            PrintCount = _PrintCount;

            applicationClass = new LabelManager2.ApplicationClass();
            applicationClass.Documents.Open(FiledPath, false);
        }

        public Lprinter(string _FiledPath, List<Dictionary<string, string>> _list, string _PrinterName, int _PrintCount)
        {
            FiledPath = _FiledPath;
            list = _list;
            PrinterName = _PrinterName;
            PrintCount = _PrintCount;
            applicationClass = new LabelManager2.ApplicationClass();
            applicationClass.Documents.Open(FiledPath, false);

        }
        public Lprinter(string _FiledPath, Dictionary<Dictionary<string, string>, int> _list, string _PrinterName )
        {
            FiledPath = _FiledPath;
            dic_plus = _list;
            PrinterName = _PrinterName;
            //PrintCount = _PrintCount;
            applicationClass = new LabelManager2.ApplicationClass();
            applicationClass.Documents.Open(FiledPath, false);
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

        public void Start()
        {
            thDo = new Thread(DoWork);
            thDo.IsBackground = true;
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
                            for (int i = 1; i <= (int)document.Variables.FormVariables.Count; i++)
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
                        // 

                        document.PrintDocument(PrintCount);

                    }

                }
                else if (dic_plus != null)
                {
                    document.Printer.SwitchTo(PrinterName, "", true);

                    foreach (KeyValuePair<Dictionary<string, string>, int> queue in dic_plus)
                    {
                      
                            dic = queue.Key as Dictionary<string,string>;

                            foreach (KeyValuePair<string, string> current in dic)
                            {
                                bool flag = false;
                                for (int i = 1; i <= (int)document.Variables.FormVariables.Count; i++)
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
                            // 

                            document.PrintDocument(queue.Value);
 
                    }

                }
                else
                {
                    foreach (KeyValuePair<string, string> current in dic)
                    {
                        bool flag = false;
                        for (int i = 1; i <= (int)document.Variables.FormVariables.Count; i++)
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


                

                _blOver = true;
                //20-5-22 提到close 上面
                ERPorg.Corg cg = new ERPorg.Corg();
                cg.kill_lppa();

                document.Close(false);
               
            }
            catch (Exception ex)
            {
                str_ErrMsg += ex.Message;

            }
            finally
            {
                try
                {
                    ERPorg.Corg cg = new ERPorg.Corg();
                    cg.kill_lppa();

                    applicationClass.Documents.CloseAll();
                    applicationClass.Quit();
                 

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(applicationClass);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(document);
                    applicationClass = null;
                    document = null;
                    if (applicationClass != null)
                    { }
                       

                    GC.WaitForPendingFinalizers();
                    GC.Collect();
             

                }
                catch
                {
                    //applicationClass.Quit();
                    GC.Collect();
                }
            }
        }

        /// <summary>
        //实验不好用
        /// </summary>
        public void DoWork2()
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
                        Thread mythread2 = new Thread(() => CalculateThree(document ,dic));
                        mythread2.IsBackground = true;        
                        mythread2.Start(); 
                       

                    }

                }
                else
                {
                    foreach (KeyValuePair<string, string> current in dic)
                    {
                        bool flag = false;
                        for (int i = 1; i < (int)document.Variables.FormVariables.Count; i++)
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
                str_ErrMsg += ex.Message;

            }
            finally
            {
                try
                {

                    applicationClass.Documents.CloseAll();
                    applicationClass.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(applicationClass);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(document);
                    applicationClass = null;
                    document = null;
                    GC.Collect();
                    //GC.WaitForPendingFinalizers();
                    //GC.Collect(); 
                    //GC.WaitForPendingFinalizers();

                }
                catch
                {

                }
            }
         
        }


        private void CalculateThree(LabelManager2.Document document, Dictionary<string, string> d)            //带多个参数的委托函数  
        {
            try
            {
                foreach (KeyValuePair<string, string> current in d)
                {
                    bool flag = false;
                    for (int i = 1; i < (int)document.Variables.FormVariables.Count; i++)
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
                // 
                document.PrintDocument(PrintCount);
            }
            catch (Exception ex)
            {
                str_ErrMsg += ex.Message;
            }
            finally
            {
                try
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(document);
                    GC.Collect();
                    //GC.WaitForPendingFinalizers();
                    //GC.Collect();
                    //GC.WaitForPendingFinalizers();
                }
                catch
                {

                }
            }
        }

      
    }
}
