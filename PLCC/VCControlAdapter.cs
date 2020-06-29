using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace PLCC
{
    //可编程电源控制器


    static class VCControlAdapter
    {
        static SerialPort sp = null;
        static public string V可编程COM_NO;
        static public Decimal V电压值;

        /// <summary>
        /// 电压保护值，在多长时间内不接受相同的电压指令。
        /// </summary>
        public static int iPDelay = 5;
        static bool blNo = true;

        static List<int> iFalg = new List<int>();

        /// <summary>
        /// 之前的VCC电压;
        /// </summary>
        static Decimal Vcc_O = -1;

        /// <summary>
        /// VCC电压的保护周期
        /// </summary>
        static int iVcc_P = 0;


        static System.Timers.Timer tmP;

        #region 串口相关
        public static void fun_串口初始化(string COM_NO)
        {
            try
            {
                if(sp == null)
                {
                sp = new SerialPort();
                sp.PortName = COM_NO;
                sp.BaudRate = 9600;
                sp.DataBits = 8; //数据位
                sp.Parity = System.IO.Ports.Parity.None; //无奇偶校验位
                sp.StopBits = System.IO.Ports.StopBits.One;//一个停止位
                //sp.ReadBufferSize = 1024;                   //接收缓冲区大小
                //sp.Encoding = Encoding.BigEndianUnicode;
                sp.Open();
                //sp.ReadExisting();

                System.Threading.Thread.Sleep(500);

                string str = new string(new char[] { (char)0x01, (char)0x57, (char)0x11, (char)0x03, (char)0x00, (char)0x00, (char)0x00 });
                fun_发送指令(str);

                tmP = new System.Timers.Timer() { Interval = 500, AutoReset = false };
                tmP.Elapsed += tm_Elapsed;
                tmP.Start();
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine(string.Format("'{0}'连接失败！请检查串口'{1}'是否存在！", "可编程电源接口", COM_NO));
            }
        }

        
        static void tm_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {

                if (Vcc_O == -1) return;
                
                iVcc_P--;

                if (iVcc_P <= 0)
                {
                    lock (iFalg)
                    {
                        string strP = new string(new char[] { (char)0x01, (char)0x57, (char)0x5e });
                        strP = strP + fun_格式化数字(Vcc_O) + fun_格式化数字(50);
                        fun_发送指令(strP);
                    }
                    iVcc_P = iPDelay;
                    Vcc_O = -1;
                }
            }
            catch
            {

            }
            finally
            {
                tmP.Start();
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
                //flag = 0;
                sp.ReadExisting();
                sp.Close();
            }
            catch (Exception ex)
            { 
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
        #endregion
        //功能1 控制可编程电源及返回可编程电源状态

        #region 可编程电源电压设置
        public static void fun_设置可编程电源电压(Decimal Vcc, Decimal Frequency = 50M)
        {
            if (Vcc_O == Vcc) return;

            Vcc_O = Vcc;

            ///aqua 加入电压线性变化点
            if (Vcc > 250 ) Vcc = Vcc + 1;
            if (Vcc >= 300) Vcc = Vcc + 1;

            lock (iFalg)
            {
                iVcc_P = iPDelay;
                string strP = new string(new char[] { (char)0x01, (char)0x57, (char)0x5e });
                strP = strP + fun_格式化数字(Vcc) + fun_格式化数字(Frequency);
                fun_发送指令(strP);
            }

        }

        private static string fun_格式化数字(Decimal dec)
        {
            int iDec = (int)(dec * 10);
            char c = (char)(iDec % 256);
            string str = new string(new char[] { (char)(iDec % 256), (char)(iDec / 256) });
            return str;
        }

        public static void fun_发送指令(string str)
        {
            List<byte> LI_B = new List<byte>();
            foreach (char c in str)
            {
                LI_B.Add((byte)c);
            }
            byte jhy = 0;
            foreach (byte b in LI_B)
            {
                jhy += b;
            }
            LI_B.Add(jhy);
            sp.Write(LI_B.ToArray(), 0, LI_B.Count);
        }
        #endregion
    }
}
