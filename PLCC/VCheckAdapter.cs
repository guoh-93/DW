using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Data;

namespace PLCC
{
    public class VCheckAdapter
    {
        #region 成员
        private SerialPort sp;  
        public System.Decimal Vcc电压 = 0;
        public bool blVcc_WD = false;


        /// <summary>
        /// 采样数
        /// </summary>
        public int iCYS = 5;

        /// <summary>
        /// 误差数
        /// </summary>
        public System.Decimal iWCS = 3;

        //static int buffersize = 10;                  
        //byte[] buffer = new Byte[buffersize];           
        List<byte> ls = new List<byte>();
        MachineAdapter ma = new MachineAdapter();
        List<Decimal> lt = new List<Decimal>();
        List<Decimal> ltt = new List<Decimal>();


        
        #endregion

        #region 串口相关
        public void fun_串口初始化(string COM_NO)
        {
            try
            {
                sp = new SerialPort();
                sp.PortName = COM_NO;
                sp.BaudRate = 115200;
                sp.DataBits = 8; //数据位
                sp.Parity = System.IO.Ports.Parity.None; 
                sp.StopBits = System.IO.Ports.StopBits.One;
                //sp.ReadBufferSize = 1024;                  
                //sp.Encoding = Encoding.BigEndianUnicode;
                sp.Open();
                sp.ReadExisting();
                fun_电压检测();
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine(string.Format("'{0}'连接失败！请检查串口'{1}'是否存在！", "电压接口", COM_NO));
            }
        }

        public void Close()
        {
            try
            {
                tm检测电压.Close();
                tm1.Close();
                tm2.Close();
            }
            catch { }
            try
            {
                sp.ReadExisting();
                sp.Close();
                sp = null;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }
        #endregion

        #region 电压判断是否通过
        System.Timers.Timer tm检测电压;
        
        public void fun_电压检测()
        {
            tm检测电压 = new System.Timers.Timer() { Interval = 50, AutoReset = false };
            tm检测电压.Elapsed += tm_Elapsed;
            tm检测电压.Start();
        }

        void tm_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                lock (lt)
                {
                    if (lt.Count >= iCYS)
                    {
                        lt.RemoveAt(0);
                    }

                    lt.Add(Vcc电压);
                }
                if (lt.Count >= iCYS)
                {
                    ltt.Clear(); 
                    ltt.AddRange(lt.ToArray());
                    ltt.Sort();
                    Decimal min = ltt[0];
                    Decimal max = ltt[ltt.Count - 1];
                    if (Math.Abs(max - min) < iWCS)
                    {
                        blVcc_WD = true;
                    }
                    else
                    {
                        blVcc_WD = false;
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                tm检测电压.Start();
            }
        }
        #endregion

        #region 发送接收电压指令并得出面板电压

        System.Timers.Timer tm1;
        System.Timers.Timer tm2;

        public void fun_发送开始连接()
        {
            List<byte> LI_B = new List<byte>();
            byte[] a = { 0xaa, 0x55, 0x02, 0xf1,0x00,0xf3 };
            LI_B.AddRange(a);
            sp.Write(LI_B.ToArray(), 0, LI_B.Count);

            tm1 = new System.Timers.Timer() { Interval = 50, AutoReset = false };
            tm1.Elapsed += tm1_Elapsed;
            tm1.Start();

            tm2 = new System.Timers.Timer() { Interval = 50, AutoReset = false };
            tm2.Elapsed += tm2_Elapsed;
            tm2.Start();           
        }

        //分析数据，得到面板电压
        void tm2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                lock (ls)
                {
                    int index = -1;
                    for (int i = ls.Count - 1; i >0; i--)
                    {
                        if (ls[i] == 0xaa && ls[i + 1] == 0x55 && ls[i + 2] == 0x04 && ls[i + 3] == 0xf6)
                        {
                            index = i;
                            break;

                        }
                    }
                    if (index == -1) return;
                    try
                    {
                        Vcc电压 = ((int)ls[index + 4] + ((int)ls[index + 5]) * 256) / 10M;
                        for (int i = 0; i < index + 6; i++)
                        {
                            ls.RemoveAt(0);
                        }
                    }
                    catch
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                (sender as System.Timers.Timer).Start();
            }
        }

        //取数据
        void tm1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                byte[] bs = new byte[sp.BytesToRead];
                //string ss = sp.Read
                sp.Read(bs, 0, sp.BytesToRead);
                lock (ls)
                {
                    ls.AddRange(bs);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                (sender as System.Timers.Timer).Start();
            }
        }
        #endregion
    }
}
