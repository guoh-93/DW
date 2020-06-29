using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PLCView
{
    public partial class frm缩略视图查看 : UserControl
    {
        string strJtmc = "";

        PLCC.MachineAdapter mad;  //控制设备


        public frm缩略视图查看(string mc)
        {
            InitializeComponent();
            strJtmc = mc;   //获取机台名称
        }



        private void frm缩略视图查看_Load(object sender, EventArgs e)
        {

            LA_jtmc.Text = strJtmc;   //给机台名称进行赋值







            //if (mad.strMachineName == strJtmc)
            //PLCView.frmABB fm = new PLCView.frmABB();
            //fm.shebei.EditValue = strJtmc;   //把机台名称赋给设备


        }


        private void fun_gcS指令刷新()
        {
            

        }


        private void fun_R指令刷新()
        {


        }




        #region  核心刷新事件部分



        private void TME_Tick(object sender, EventArgs e)
        {




            //TME.Enabled = false;

            //try
            //{
                
            //    if (mad == null) return;  //设备还没有进行注册

            //    fun_得到MAD后初始化检测类型();   //获取设备中是否带有检测类别
            //    //设置界面指示灯，及其它状态。
            //    string strY = mad.PLC_S_PLC_Y;
            //    char[] Y = strY.ToCharArray();  //字符串转换成字符数组
            //    fun_亮灯函数(Y);

            //    // 如果mad在非工作状态和工作状态，那么gcTeam 和 gcACt 的处理方式不一样
            //    if (mad.blwork == false)
            //    {
            //        barLargeButtonItem1.Enabled = true;//锁定设备连接按钮
            //        jianceleibie.Enabled = true;  //锁定检测类别的框
            //        barLargeButtonItem2.Enabled = true;//锁定更换按钮

            //        gongzuo.BackColor = Color.Gray; //工作灯 
            //        StartTime = "";
            //        EndTime = DateTime.Now.ToString();
            //        flag = 0; //标志位
            //    }

            //    //如果是工作状态。那么gcTeam 和 gcACt 的显示完全由mad的节奏决定 ，主要由
            //    if (Convert.ToBoolean(mad.blwork) == true)  //如果该设备在工作的状态下
            //    {
            //        ///aqua : 用S指令来刷新界面 ，需要一个OLD_S 辅助。
            //        if (mad.PLC_S_PLC_POS != S_oldpos)
            //        {
            //            fun_参数状态获取();
            //            S_oldpos = mad.PLC_S_PLC_POS;
            //        }

            //        barLargeButtonItem1.Enabled = false;//锁定设备连接按钮
            //        jianceleibie.Enabled = false;  //锁定检测类别的框
            //        barLargeButtonItem2.Enabled = false;//锁定更换按钮

            //        if (flag == 0)
            //        {
            //            oldblwork = mad.blwork;
            //            strCx = chanxian.EditValue.ToString();  //产线
            //            strCpSN = liuzhuanka.EditValue.ToString();   //流转卡
            //            StartTime = DateTime.Now.ToString();  //开始检测时间
            //            jtmc = shebei.EditValue.ToString();
            //            flag = 1;
            //        }
            //        //工作灯就要亮起来
            //        gongzuo.BackColor = Color.DarkBlue; //蓝色
            //    }
            //}
            //catch
            //{

            //}
            //finally
            //{
            //    TME.Enabled = true;
            //}




        }



        #endregion
    }
}
