using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace PLCC
{
    public partial class frmVccUI : UserControl
    {

        static SerialPort sp;

        public frmVccUI()
        {
            InitializeComponent();
        }

        private void frmVccUI_Load(object sender, EventArgs e)
        {
            try
            {
                sp = new SerialPort();
                string ss = CPublic.Var.strConn;
                sp.PortName = CPublic.Var.li_CFG["Vcc_COM"];
                sp.BaudRate = 9600;
                sp.DataBits = 8; //数据位
                sp.Parity = System.IO.Ports.Parity.None; //无奇偶校验位
                sp.StopBits = System.IO.Ports.StopBits.One;//一个停止位
                sp.ReadBufferSize = 40960;                   //接收缓冲区大小
                //sp.Encoding = Encoding.BigEndianUnicode;
                sp.Open();
                sp.ReadExisting();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        #region 可编程电压指令控制
        /// <summary>
        /// 发送指令后的延时时间
        /// </summary>
        static int iTimeDelay = 100;
        static bool blaa = false;
        public static void fun_设置可编程电源电压(Decimal Vcc, Decimal Frequency = 50M)
        {
            if (blaa == false)
            {
                string str = new string(new char[] { (char)0x01, (char)0x57, (char)0x11, (char)0x03, (char)0x00, (char)0x00, (char)0x00 });
                fun_发送指令(str);
                System.Threading.Thread.Sleep(iTimeDelay);
                blaa = true;
            }

            string strP = new string(new char[] { (char)0x01, (char)0x57, (char)0x5e });
            strP = strP + fun_格式化数字(Vcc) + fun_格式化数字(Frequency);
            fun_发送指令(strP);

        }

        private static string fun_格式化数字(Decimal dec)
        {
            int iDec = (int)(dec * 10);
            //int id = iDec % 256 + iDec / 256;

            //string str = iDec.ToString("X").PadLeft(4, '0');
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
            string ss = "";
            foreach (byte b in LI_B)
            {
                ss += b.ToString("X") + " ";
            }
            sp.Write(LI_B.ToArray(), 0, LI_B.Count);
        }

        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            fun_设置可编程电源电压(0, 50);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            fun_设置可编程电源电压(155, 50);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fun_设置可编程电源电压(230, 50);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            fun_设置可编程电源电压(300, 50);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            fun_设置可编程电源电压(400, 50);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_设置可编程电源电压(int.Parse(textBox1.Text), 50);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            fun_设置可编程电源电压(158, 50);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            fun_设置可编程电源电压(395, 50);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            fun_设置可编程电源电压(49, 50);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            fun_设置可编程电源电压(Convert.ToDecimal(194.5), 50);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            fun_设置可编程电源电压(255, 50);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            fun_设置可编程电源电压(Convert.ToDecimal(299.5), 50);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            fun_设置可编程电源电压(273, 50);
        }


    }
}
