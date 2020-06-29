using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Data;
using System.Collections;

namespace PLCC
{
    public class PLCAdapter
    {
        #region 成员
        private SerialPort sp;  
        //public int flag = 0;
        //static int buffersize = 10;                        
        //byte[] buffer = new Byte[buffersize];            
        string str_检测组动作子表转换成PLC的指令 = "";
        string str_检测类型主表转换成PLC指令 = "";

        List<byte> ls = new List<byte>();    //指令队列

        Boolean BLLClose = false;
        #endregion

        #region PLC指令
        public string PLC_S;                 //S指令
        public List<string> PLC_Other;       //特殊指令
        #endregion

        #region 串口相关
        public void Close()
        {
            BLLClose = true;

            try
            {
                tm1.Close();
                tm2.Close();
            }
            catch { }
            
            try
            {
                sp.ReadExisting();
                sp.Close();
            }
            catch { }

            

        }

        public void fun_串口初始化(string COM_NO)
        {
            try
            {
                BLLClose = false;
                sp = new SerialPort();
                sp.PortName = COM_NO;
                sp.BaudRate = 19200;
                sp.DataBits = 8; //数据位
                sp.Parity = System.IO.Ports.Parity.None; //无奇偶校验位
                sp.StopBits = System.IO.Ports.StopBits.One;//一个停止位
                //sp.ReadBufferSize = 1024;                   //接收缓冲区大小
                //sp.Encoding = Encoding.BigEndianUnicode;
                sp.Open();
                //sp.ReadExisting();

                fun_接收_NEW();
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine(string.Format("'{0}'连接失败！请检查串口'{1}'是否存在！","PLC接口",COM_NO));
            }
        }

        public void fun_关闭串口(string COM_NO)
        {
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

        #region 接受指令
        System.Timers.Timer tm1;
        System.Timers.Timer tm2;
        System.Timers.Timer tm3; //发送
        /// <summary>
        /// 从PLC得到数据指令
        /// 把所有指令分解成二种:S或特殊
        /// 要求：不管PLC发送的数据速度是快还是慢，不能漏收。
        /// </summary>
        public void fun_接收_NEW()
        {
            tm1 = new System.Timers.Timer() { Interval = 100, AutoReset = false };
            tm1.Elapsed += tm1_Elapsed;
            tm1.Start();

            tm2 = new System.Timers.Timer() { Interval = 50, AutoReset = false };
            tm2.Elapsed += tm2_Elapsed;
            tm2.Start();


            
        }



        void tm2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                lock (ls)
                {
                    int index1 = -1, index2 = -1;
                    //长度最少2
                    if ((index1 = ls.LastIndexOf(2)) == -1) return;
                    index2 = ls.IndexOf(3, index1);
                    if (index1 >= 0 && index2 > index1)
                    {
                        fun_字符处理(System.Text.Encoding.ASCII.GetString(ls.GetRange(index1 + 1, index2 - index1).ToArray()));
                        ls.RemoveRange(0, index2 + 1);
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

        void tm1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                byte[] bs = new byte[sp.BytesToRead];
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

        public void fun_字符处理(string str) //将指令分为S指令和特殊指令，再单独处理
        {
            try
            {
                if (str == "")
                {
                    System.Diagnostics.Debug.WriteLine("解析文本不能为空");
                }
                else
                {
                    //char[] s = str.ToCharArray();
                    if (str[0] == 'S')
                    {
                        if (str.Length >= 109)
                        {
                            if (PLC_S != str)
                            {
                                System.Diagnostics.Debug.WriteLine(str);
                            }
                            PLC_S = str;
                        }
                    }
                    if (str[0] == 'S') //R指令 //回馈指令
                    {
                        PLC_Other.Add(str);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
        #endregion

        #region 指令发送
        #region 动作指令
        /// <summary>
        /// 检测组动作子表转换成PLC指令
        /// 
        /// </summary>
        /// <param name="dt_动作"></param>
        public string fun_当前要发送的A指令(DataRow r)
        {
            str_检测组动作子表转换成PLC的指令 = "a";
            str_检测组动作子表转换成PLC的指令 = str_检测组动作子表转换成PLC的指令 + string.Format("{0:000}", int.Parse(r["检测ID"].ToString()));

            int i1 = Convert.ToInt32("0x" + r["动作ID"].ToString(), 16);
            str_检测组动作子表转换成PLC的指令 = str_检测组动作子表转换成PLC的指令 + string.Format("{0:000}", i1);

            //str_检测组动作子表转换成PLC的指令 = str_检测组动作子表转换成PLC的指令 + string.Format("{0:000}", int.Parse(r["动作ID"].ToString()));
            str_检测组动作子表转换成PLC的指令 = str_检测组动作子表转换成PLC的指令 + string.Format("{0:000}", int.Parse(r["动作POS"].ToString()));
            for (int i = 1; i <= 5;i++ )
                try
                {
                    str_检测组动作子表转换成PLC的指令 = str_检测组动作子表转换成PLC的指令 + string.Format("{0:00000}", int.Parse(r["P" + i.ToString()].ToString()));
                }
                catch
                {
                    str_检测组动作子表转换成PLC的指令 += "00000";
                }

            //str_检测组动作子表转换成PLC的指令 = str_检测组动作子表转换成PLC的指令 + string.Format("{0:00000}", int.Parse(r["P2"].ToString()));
            //str_检测组动作子表转换成PLC的指令 = str_检测组动作子表转换成PLC的指令 + string.Format("{0:00000}", int.Parse(r["P3"].ToString()));
            //str_检测组动作子表转换成PLC的指令 = str_检测组动作子表转换成PLC的指令 + string.Format("{0:00000}", int.Parse(r["P4"].ToString()));
            //str_检测组动作子表转换成PLC的指令 = str_检测组动作子表转换成PLC的指令 + string.Format("{0:00000}", int.Parse(r["P5"].ToString()));
            return str_检测组动作子表转换成PLC的指令;
        }

        List<byte> LI_B = new List<byte>();       

        /// <summary>
        ///   pc向PLC发送 动作指令str
        ///   指令标志:a,61H
        /// </summary>
        public void fun_发送指令(string str)
        {
            lock (LI_B)
            {

                LI_B.Clear();
                LI_B.Add(2);
                byte[] a = Encoding.ASCII.GetBytes(str);
                LI_B.AddRange(a);
                LI_B.Add(3);
                sp.Write(LI_B.ToArray(), 0, LI_B.Count);
            }
        }


        #endregion

        #region 检测参数
        /// <summary>
        /// 检测类型主表转换成PLC指令
        /// </summary>
        /// <param name="dt_检测参数"></param>
        public string fun_dt主表转PLC指令(DataRow r_检测参数)
        {
            str_检测类型主表转换成PLC指令 = "p";
            str_检测类型主表转换成PLC指令 = str_检测类型主表转换成PLC指令 + string.Format("{0:000}", int.Parse(r_检测参数["检测ID"].ToString()));
            str_检测类型主表转换成PLC指令 = str_检测类型主表转换成PLC指令 + string.Format("{0:0000}", int.Parse(r_检测参数["分闸激光距离允许误差"].ToString()));
            str_检测类型主表转换成PLC指令 = str_检测类型主表转换成PLC指令 + string.Format("{0:0000}", int.Parse(r_检测参数["合闸激光距离允许误差"].ToString()));
            str_检测类型主表转换成PLC指令 = str_检测类型主表转换成PLC指令 + string.Format("{0:0000}", int.Parse(r_检测参数["手自动力下限"].ToString()));
            str_检测类型主表转换成PLC指令 = str_检测类型主表转换成PLC指令 + string.Format("{0:0000}", int.Parse(r_检测参数["手自动力上限"].ToString()));
            str_检测类型主表转换成PLC指令 = str_检测类型主表转换成PLC指令 + string.Format("{0:000}", int.Parse(r_检测参数["电压默认等待时间上限"].ToString()));
            str_检测类型主表转换成PLC指令 = str_检测类型主表转换成PLC指令 + string.Format("{0:00}", int.Parse(r_检测参数["合闸默认等待时间上限"].ToString()));
            str_检测类型主表转换成PLC指令 = str_检测类型主表转换成PLC指令 + string.Format("{0:00}", int.Parse(r_检测参数["分闸默认等待时间上限"].ToString()));
            str_检测类型主表转换成PLC指令 = str_检测类型主表转换成PLC指令 + string.Format("{0:00}", int.Parse(r_检测参数["脱扣默认等待时间上限"].ToString()));
            str_检测类型主表转换成PLC指令 = str_检测类型主表转换成PLC指令 + string.Format("{0:00}", int.Parse(r_检测参数["机构合闸动作时间"].ToString()));
            str_检测类型主表转换成PLC指令 = str_检测类型主表转换成PLC指令 + string.Format("{0:00}", int.Parse(r_检测参数["机构分闸动作时间"].ToString()));
            str_检测类型主表转换成PLC指令 = str_检测类型主表转换成PLC指令 + string.Format("{0:00}", int.Parse(r_检测参数["机构脱扣动作时间"].ToString()));
            str_检测类型主表转换成PLC指令 = str_检测类型主表转换成PLC指令 + string.Format("{0:00}", int.Parse(r_检测参数["机构手动动作时间"].ToString()));
            str_检测类型主表转换成PLC指令 = str_检测类型主表转换成PLC指令 + string.Format("{0:00}", int.Parse(r_检测参数["机构自动动作时间"].ToString()));
            str_检测类型主表转换成PLC指令 = str_检测类型主表转换成PLC指令 + string.Format("{0:00}", int.Parse(r_检测参数["自动合闸动作时间"].ToString()));
            str_检测类型主表转换成PLC指令 = str_检测类型主表转换成PLC指令 + string.Format("{0:00}", int.Parse(r_检测参数["自动分闸动作时间"].ToString()));
            str_检测类型主表转换成PLC指令 = str_检测类型主表转换成PLC指令 + string.Format("{0:00}", int.Parse(r_检测参数["分合闸动作时间误差"].ToString()));
            str_检测类型主表转换成PLC指令 = str_检测类型主表转换成PLC指令 + string.Format("{0:00}", int.Parse(r_检测参数["其它动作时间误差"].ToString()));
            return str_检测类型主表转换成PLC指令;
        }

        //界面调用  向PLC传递参数
        public void fun_检测参数(DataTable dt)
        {
            try
            {
                fun_发送指令(fun_dt主表转PLC指令(dt.Rows[0]));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        #endregion
        #endregion
    }
}
