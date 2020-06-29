using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace PLCC
{
    /// <summary>
    /// 6W设备工作台得到扫码枪扫枪的适配类型
    /// </summary>
    public static class W6_SNSCAN
    {

        #region 串口 
        static public string strCOM = "";
        static public string strCOMpara = "";

        static SerialPort sp = null;

        static System.Timers.Timer tmP;

        static List<byte> ls = new List<byte>();    //指令队列

        public static void Start(string COM_NO)
        {
            try
            {
                if (sp == null)
                {
                    strCOM = COM_NO;

                    sp = new SerialPort();
                    sp.PortName = COM_NO;
                    sp.BaudRate = 9600;
                    sp.DataBits = 8; //数据位
                    sp.Parity = System.IO.Ports.Parity.None; //无奇偶校验位
                    sp.StopBits = System.IO.Ports.StopBits.One;//一个停止位
                    //sp.ReadBufferSize = 1024;                   //接收缓冲区大小
                    //sp.Encoding = Encoding.BigEndianUnicode;
                    sp.Open();
                    sp.ReadExisting();

                    System.Threading.Thread.Sleep(800);

                    tmP = new System.Timers.Timer() { Interval = 500, AutoReset = false };
                    tmP.Elapsed += tmP_Elapsed;
                    tmP.Start();
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine(string.Format("'{0}'连接失败！请检查串口'{1}'是否存在！", "可编程电源接口", COM_NO));
            }
        }

        static void tmP_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ///读取扫码枪扫码到的字符
            try
            {

                byte[] bs = new byte[sp.BytesToRead];
                sp.Read(bs, 0, sp.BytesToRead);
                lock (ls)
                {
                    ls.AddRange(bs);

                    if (ls.Count > 2)
                    {
                        if (ls[ls.Count - 1] == 0xa && ls[ls.Count - 2] == 0xd)
                        {
                            string[] SsN = System.Text.Encoding.ASCII.GetString(ls.GetRange(0, ls.Count - 2).ToArray()).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                            string ssn = SsN[SsN.Length - 1];

                            if (sN1 != "")
                            {
                                sN2 = ssn;
                            }
                            else
                            {
                                sN1 = ssn;
                            }
                            ls.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                CZMaster.MasterLog.WriteLog(ex.Message, "W6_SNSCAN:tmP_Elapsed ");
            }
            finally
            {
                (sender as System.Timers.Timer).Start();
            }
        }

        public static void Close()
        {
            try
            {
                tmP.Close();
            }
            catch { }
            try
            {
                sp.ReadExisting();
                sp.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
        #endregion

         
        #region SN号属性

        /// <summary>
        /// 当前扫码
        /// </summary>
        private static string sN1 = "";

        /// <summary>
        /// 第一个扫码
        /// </summary>
        private static string sN2 = "";


        /// <summary>
        /// 当前扫码
        /// </summary>
        public static string SN1
        {
            get
            {
                return sN1;
            }
        }

        /// <summary>
        /// 第一个扫码
        /// </summary>
        public static string SN2
        {
            get
            {
                return sN2;
            }
        }
        #endregion



        #region 清除扫码
        /// <summary>
        /// 清除第一个编码，并第一个编码代替第一个编码
        /// </summary>
        public static void ClearSN1()
        {
            sN1 = sN2;
            sN1 ="";
        }

        /// <summary>
        /// 清除第二个编码
        /// </summary>
        public static void ClearSN2()
        {
            sN2 = "";
        }

        /// <summary>
        /// 清除二个编码
        /// </summary>
        public static void ClaerSN()
        {
            sN1 = "";
            sN2 = "";
        }
        #endregion


    }
}
