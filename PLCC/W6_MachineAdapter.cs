using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Data.SqlClient;

namespace PLCC
{

    public class W6_MachineAdapter
    {
        #region PLC使用的全局变量_机台
        public DataTable dt_检测组动作子表;
        public DataTable dt_检测类型主表;
        public string strCOM = "";
        public string strComPara = "";
        public string strVCCOM = "";
        public string strVCCOMPara = "";
        public string strMachineName = "";
        public DateTime T_工作时间_开始时间 = System.DateTime.Now;
        public DateTime T_主动作时间_开始时间 = System.DateTime.Now;
        public DateTime T_组动作时间_开始时间 = System.DateTime.Now;
        public DateTime T_子动作时间_开始时间 = System.DateTime.Now;
        public TimeSpan T_工作时间;
        public TimeSpan T_主动作时间;
        public TimeSpan T_组动作时间;
        public TimeSpan T_子动作时间;
        public DataRow dr_Curr;

        #endregion

        #region PLC使用的全局变量_PLC
        //S指令
        public string PLC_S_机台类型 = "";
        public string PLC_S_PLC_COUNT_ID = "";
        public string PLC_S_主状态 = "";
        public string PLC_S_手自动 = "";
        public string PLC_S_组队 = "";
        public string PLC_S_检测ID = "";
        public string PLC_S_PLC动作 = "";
        public string PLC_S_PLC_POS = "";
        private string PLC_S_PLC_POS_OLD = "";
        //public string PLC_S_PLC_SYN = "";
        //private string PLC_S_PLC_SYN_OLD = "";

        public string PLC_S_激光AD = "";
        public string PLC_S_力AD = "";
        public string PLC_S_PLC_X = "";
        public string PLC_S_PLC_Y = "";
        public string PLC_S_动作已用时间 = "";
        public string PLC_S_产品已用时间 = "";
        public string PLC_S_断路器开合次数 = "";
        public string PLC_S_Ready = "";
        public string PLC_S_指令等待 = "";
        public string PLC_S_Work = "";
        public string PLC_S_Work_OLD = "";
        public string PLC_S_激光分闸标定值 = "";
        public string PLC_S_激光合闸标定值 = "";

        public string PLC_S_TF = "";
        public string PLC_S_TF_OLD = "";
        public string PLC_S_TS1 = "";
        public string PLC_S_TS2 = "";
        public string PLC_S_TS_T = "";

        /// <summary>
        /// 产品2P或4P,W特有属性
        /// </summary>
        public string PLC_W_P = "2";



        public int PLC_CURR_POS = -1;    //-1 无动作状态

        //public int vSyn = 0;

        //public int vSyn_OLD = 0;

        ////R指令 //FFF急停
        //public string PLC_R_检测ID = "";
        //public string PLC_R_动作ID = "";
        //public string PLC_R_动作POS = "";
        ////public string PLC_R_动作POSS = "";
        public string PLC_R_结果判定 = "";
        //public string PLC_R_错误代码 = "";
        //public string PLC_R_R1 = "";
        //public string PLC_R_R2 = "";
        //public string PLC_R_R3 = "";
        //public string PLC_R_R4 = "";
        //public string PLC_R_R5 = "";
        //public string PLC_R_R6 = "";
        //public string PLC_R_R7 = "";
        //public string PLC_R_R8 = "";
        //public string PLC_R_R9 = "";
        //public string PLC_R_R10 = "";

        public List<ResultR> ResultRS = new List<ResultR>();
        #endregion

        #region  组队控制
        public Boolean blTeam = false;
        //private Boolean blGoNext = false;
        #endregion

        #region 成员
        public System.Decimal Vcc0 = 230;
        public System.Decimal Vcc1 = 150;
        public System.Decimal Vcc2 = 280;
        /// <summary>
        /// 电压等级，0 对应常压 = 230V，1 对应低压 = 150V，2，对应高压280V
        /// </summary>
        public string VccLV = "0";
        public System.Decimal Vcc_电压 = 0;
        public System.DateTime Vtime = System.DateTime.Now;
        public Boolean BLVcc_WD = false;
        public string V额定电压 = "";
        public System.Decimal V电压误差 = 0;
        public string V交直流 = "";
        public double a激光合闸误差 = -1;
        public double a激光分闸误差 = -1;

        //public Boolean blwait = false;           //指令等待

        //Boolean blerror = false;                 //结果判定

        public int idzjp = -1; //动作节拍 -1,不工作，0，准备工作，1，准备开始，2，R指令完成,3 PLC同步完成，等待判定返回（R）。4.发送指定，等待PLC同步。

        public int iDZJP
        {
            get
            {
                return idzjp;
            }
            set
            {
                if (value == 0)
                {
                    if (idzjp == -1)
                    {
                        idzjp = 0;
                    }
                }
                if (value == 1)
                {
                    if (idzjp == 2 || idzjp == 0)
                    {
                        idzjp = 1;
                    }
                }
                if (value == 4)
                {
                    if (idzjp == 1)
                    {
                        idzjp = value;
                    }
                }
                if (value == 3)
                {
                    if (idzjp == 4)
                    {
                        idzjp = value;
                    }
                }
                if (value == 2)
                {
                    if (idzjp == 3) //只有动作3可以转到动作2
                    {
                        idzjp = 2;
                    }
                }
                if (value == -1)
                {
                    idzjp = -1;
                }
            }
        }


        public Boolean blwork
        {
            get
            {
                if (iDZJP == 1 || iDZJP == 2 || iDZJP == 3 || iDZJP == 4) return true;
                return false;
            }
        }

        /// <summary>
        /// 机台状态。
        /// 0 无动作，等待 ,1,通过，2，NG，3，放弃
        /// </summary>
        public int iMMresult = 0;

        // public Boolean blReadyWrok = false;


        //int V12_itime计时 = 0;

        //int iResetTime = 0;

        List<byte> LI_B = new List<byte>();

        Boolean Is开始计时_组 = false;
        Boolean Is开始计时_主 = false;
        Boolean Is开始计时_子 = false;
        string old_检测组POS = "";
        string old_主动作POS = "";
        string old_子动作POS = "";

        PLCAdapter plc;
        VCheckAdapter VCK;
        W6_PLCActResult PlcAR;
        string strconn = CPublic.Var.geConn("PLC");
        List<string> list_机构动作 = new List<string>();

        Boolean BLLClose = false;

        public string strCPSN = "";

        int iTeamSnedDleay = 1000;

        /// <summary>
        /// PLC同步指令用的记数指令
        /// </summary>
        int iTeamSend = 0;

        #endregion

        #region 多线程
        System.Timers.Timer tm指令分析;
        System.Timers.Timer tm钟摆;
        System.Timers.Timer tm心跳;
        #endregion

        #region 钟摆控制
        public void setGoNext()
        {
            {
                try
                {
                    dr_Curr = dt_检测组动作子表.Rows[PLC_CURR_POS];
                    if (PLC_CURR_POS > 0)
                    {
                        DataRow r = dt_检测组动作子表.Rows[PLC_CURR_POS - 1];
                    }
                }
                catch { }
                CZMaster.MasterLog.WriteLog(string.Format("{1} POS:{0} 开始进入下一个动作", PLC_CURR_POS, strMachineName));
                PLC_CURR_POS++;

            }

        }

        #endregion

        #region 开始
        public void Close()
        {
            BLLClose = true;

            try
            {
                tm心跳.Close();
                tm指令分析.Close();
                tm钟摆.Close();
            }
            catch { }

            plc.Close();
            VCK.Close();
            //VCControlAdapter.Close();
        }

        public void Start()
        {
            try
            {
                BLLClose = false;
                string sql = "select 动作ID from ABB动作表 where 动作执行类型 = '机构动作'";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow r in dt.Rows)
                {
                    if (r["动作ID"].ToString() != "25")
                    {
                        list_机构动作.Add(r["动作ID"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            plc = new PLCAdapter();
            VCK = new VCheckAdapter();
            PlcAR = new W6_PLCActResult(this);

            plc.fun_串口初始化(strCOM);   //同时开始接收PLC的S指令；区分S和R指令
            VCK.fun_串口初始化(strVCCOM);

            VCK.fun_发送开始连接();  //开始读取电压表的值

            tm心跳 = new System.Timers.Timer() { Interval = 2000, AutoReset = false };
            tm心跳.Elapsed += tm心跳_Elapsed;
            tm心跳.Start();

            tm指令分析 = new System.Timers.Timer() { Interval = 50, AutoReset = false };
            tm指令分析.Elapsed += tm指令分析_Elapsed;
            tm指令分析.Start();

            tm钟摆 = new System.Timers.Timer() { Interval = 50, AutoReset = false };
            tm钟摆.Elapsed += tm钟摆_Elapsed;
            tm钟摆.Start();

            MachineManager.StartListen();
        }

        void tm心跳_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (blTeam == false)
                {
                    //sOK
                    plc.fun_发送指令(new string(new char[] { (char)0x73, (char)0x4f, (char)0x4b }));
                }
                if (blTeam == true)
                {
                    //sOT
                    plc.fun_发送指令(new string(new char[] { (char)0x73, (char)0x4f, (char)0x54 }));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {

                tm心跳.Start();
            }
        }


        int Time_参数 = 0;
        void tm钟摆_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (blwork == false) return;
                if (iDZJP == 1) //准备发送指令。
                {

                    setGoNext();
                    iDZJP = 4;
                    if (dr_Curr["动作ID"].ToString() == "FE")
                    {
                        //结束动作
                        System.Threading.Thread.Sleep(500);
                        plc.fun_发送指令(plc.fun_当前要发送的A指令(dr_Curr));
                        PlcAR.iPLCActResult = 1;
                        PlcAR.PLCActResultWork = false;
                        PlcAR.ErrorCode = 0;

                        ResultR Rr = new ResultR();
                        DataRow r = dr_Curr;
                        Rr.PLC_R_检测ID = r["检测ID"].ToString();
                        Rr.PLC_R_动作ID = r["动作ID"].ToString();
                        Rr.PLC_R_动作POS = r["动作POS"].ToString();
                        Rr.PLC_R_结果判定 = "FE";
                        Rr.PLC_R_错误代码 = "";
                        Rr.PLC_R_错误描述 = "";
                        Rr.PLC_R_R1 = "";
                        Rr.PLC_R_R2 = "";
                        Rr.PLC_R_R3 = "";
                        Rr.PLC_R_R4 = "";
                        Rr.PLC_R_R5 = "";
                        Rr.PLC_R_R6 = "";
                        Rr.PLC_R_R7 = "";
                        Rr.PLC_R_R8 = "";
                        Rr.PLC_R_R9 = "";
                        Rr.PLC_R_R10 = "";
                        lock (ResultRS)
                        {
                            ResultRS.Add(Rr);
                        }
                        //PLC_CURR_POS = -1;
                        iDZJP = -1;
                        iMMresult = 1;
                        return;
                    }



                    Time_参数 = fun_动作参数和电压参数();

                    plc.fun_发送指令(plc.fun_当前要发送的A指令(dr_Curr));

                    iTeamSend = -10;
                    CZMaster.MasterLog.WriteLog(string.Format("{3}: POS:{0} 动作名称:{1} 动作ID:{2} 开始发出指令", dr_Curr["动作POS"], dr_Curr["动作说明"], dr_Curr["动作ID"], strMachineName));



                }
                if (iDZJP == 4)
                {
                    iTeamSend++;
                    if (int.Parse(PLC_S_PLC_POS) == int.Parse(dr_Curr["动作POS"].ToString()))
                    {
                        CZMaster.MasterLog.WriteLog(string.Format("{3}: POS:{0} 同步完成,动作说明 {1} 动作ID:{2} ", dr_Curr["动作POS"], dr_Curr["动作说明"], dr_Curr["动作ID"], strMachineName));
                        iDZJP = 3;
                        PlcAR.StartCheck(dr_Curr, Time_参数);
                        //CZMaster.MasterLog.WriteLog(string.Format("{3}: POS:{0} 动作名称:{1} 动作ID:{2} 开始发出指令", dr_Curr["动作POS"], dr_Curr["动作说明"], dr_Curr["动作ID"], strMachineName));

                    }
                    else
                    {
                        ///补发指令，如果一直不同步。就补发指令
                        if (iTeamSend >= 4)
                        {
                            iTeamSend = 0;
                            plc.fun_发送指令(plc.fun_当前要发送的A指令(dr_Curr));
                            CZMaster.MasterLog.WriteLog(string.Format("{3}: POS:{0} 动作名称:{1} 动作ID:{2} 补发出指令", dr_Curr["动作POS"], dr_Curr["动作说明"], dr_Curr["动作ID"], strMachineName));
                            System.Diagnostics.Debug.WriteLine(string.Format("*aqua* {3}: POS:{0} 动作名称:{1} 动作ID:{2} 补发出指令", dr_Curr["动作POS"], dr_Curr["动作说明"], dr_Curr["动作ID"], strMachineName));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(strMachineName + " :  tm钟摆_Elapsed " + ex.Message);
            }
            finally
            {
                tm钟摆.Start();
            }
        }

        private int fun_动作参数和电压参数()
        {
            int Time_参数 = 0;
            //T_工作时间_开始时间 = System.DateTime.Now;


            ///FR6W 里不需要 10或11这二个动作。
            //if (dr_Curr["动作ID"].ToString() == "10" || dr_Curr["动作ID"].ToString() == "11")
            //{

            //    Vtime = System.DateTime.Now;
            //    V额定电压 = dr_Curr["P2"].ToString();
            //    V交直流 = dr_Curr["P3"].ToString();
            //    if (V交直流 == "1")  //交流 1，直流 0
            //    {
            //        V电压误差 = Convert.ToDecimal(dt_检测类型主表.Rows[0]["交流电压允许误差"].ToString());
            //    }
            //    if (V交直流 == "0")
            //    {
            //        V电压误差 = Convert.ToDecimal(dt_检测类型主表.Rows[0]["直流电压允许误差"].ToString());
            //    }
            //    VCControlAdapter.fun_设置可编程电源电压(System.Decimal.Parse(V额定电压), (System.Decimal)50);
            //    //开始电压检测                            
            //}

            ///电压等级切换动作。
            if (dr_Curr["动作ID"].ToString() == "16")
            {
                string VccLV = dr_Curr["P2"].ToString();

                if (VccLV == "1")
                {
                    Vcc_电压 = Vcc1;
                }
                if (VccLV == "2")
                {
                    Vcc_电压 = 280;
                }
                if (VccLV != "1" && VccLV != "2")
                {
                    VccLV = "0";
                    Vcc_电压 = 220;
                }

                V额定电压 = Vcc_电压.ToString();
            }

            try
            {
                Time_参数 = int.Parse((Convert.ToDouble(dr_Curr["P1"].ToString()) * 1000).ToString());
            }
            catch
            {
                Time_参数 = 500;
            }
            if (a激光分闸误差 == -1)
            {
                a激光分闸误差 = Convert.ToDouble(dt_检测类型主表.Rows[0]["分闸激光距离允许误差"].ToString());
            }
            if (a激光合闸误差 == -1)
            {
                a激光合闸误差 = Convert.ToDouble(dt_检测类型主表.Rows[0]["合闸激光距离允许误差"].ToString());
            }

            //Vtime 重新计时
            if (list_机构动作.IndexOf(dr_Curr["动作ID"].ToString()) >= 0)
            {
                Vtime = System.DateTime.Now;
            }

            return Time_参数;
        }

        void tm指令分析_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Vcc_电压 = VCK.Vcc电压;
                BLVcc_WD = VCK.blVcc_WD;
                //处理S
                fun_PLC处理指令_S(plc.PLC_S);

                fun_JSGZSJ();

                fun_PLC指令的后续处理();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(strMachineName + " , " + ex.Message + "    tm指令分析_Elapsed");
            }
            finally
            {
                tm指令分析.Start();
            }
        }

        private void fun_JSGZSJ()
        {
            #region 计算各种时间
            try
            {
                if (PLC_CURR_POS == -1) return;
                if (dt_检测组动作子表 == null) return;

                if (dt_检测组动作子表.Rows.Count == 0) return;
                DataRow rr = dt_检测组动作子表.Rows[PLC_CURR_POS];
                //计算工作时间

                T_工作时间 = System.DateTime.Now - T_工作时间_开始时间;

                //计算组工作时间
                if (old_检测组POS != rr["检测组POS"].ToString())
                {
                    old_检测组POS = rr["检测组POS"].ToString();
                    T_组动作时间_开始时间 = System.DateTime.Now;
                    Is开始计时_组 = true;
                }
                if (old_检测组POS == rr["检测组POS"].ToString() && Is开始计时_组 == true)
                {
                    T_组动作时间 = System.DateTime.Now - T_组动作时间_开始时间;
                }
                if (PlcAR.iPLCActResult == 2 || PLC_S_Work == "0")
                {
                    Is开始计时_组 = false;
                }

                //计算主工作时间
                if (old_主动作POS != rr["主动作POS"].ToString())
                {
                    old_主动作POS = rr["主动作POS"].ToString();
                    T_主动作时间_开始时间 = System.DateTime.Now;
                    Is开始计时_主 = true;
                }
                if (old_主动作POS == rr["主动作POS"].ToString() && Is开始计时_主 == true)
                {
                    T_主动作时间 = System.DateTime.Now - T_主动作时间_开始时间;
                }
                if (PlcAR.iPLCActResult == 2 || PLC_S_Work == "0")
                {
                    Is开始计时_主 = false;
                }

                //T_子动作时间
                if (old_子动作POS != rr["动作POS"].ToString())
                {
                    old_子动作POS = rr["动作POS"].ToString();
                    T_子动作时间_开始时间 = System.DateTime.Now;
                    Is开始计时_子 = true;
                }
                if (old_子动作POS == rr["动作POS"].ToString() && Is开始计时_子 == true)
                {
                    T_子动作时间 = System.DateTime.Now - T_子动作时间_开始时间;
                }
                if (PlcAR.iPLCActResult == 2 || PLC_S_Work == "0")
                {
                    Is开始计时_子 = false;
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(strMachineName + " , " + ex.Message + "   fun_JSGZSJ");
            }

            #endregion
        }
        public void fun_NG()
        {

            // 发送NG指令  0FD
            plc.fun_发送指令("a0002530000000000000000000000000000");
            iMMresult = 2;
            //PLC_CURR_POS = -1;
            iDZJP = -1;
        }

        #endregion

        #region 处理PLC的S、R指令
        private void fun_PLC指令的后续处理()
        {
            try
            {
                if (plc.PLC_S != "")   //S指令不为空
                {
                    //iResetTime += (int)tm指令分析.Interval;

                    if (MachineManager.BLWork == false && iDZJP == -1)
                    {
                        PLC_CURR_POS = -1;
                    }

                    if (blwork == true && PLC_S_Work == "0" && PLC_S_Work_OLD == "1")
                    {
                        PlcAR.iPLCActResult = 2;
                        PlcAR.PLCActResultWork = false;
                        PlcAR.ErrorCode = 0;

                        ResultR Rr = new ResultR();
                        DataRow r = dr_Curr;
                        Rr.PLC_R_检测ID = r["检测ID"].ToString();
                        Rr.PLC_R_动作ID = r["动作ID"].ToString();
                        Rr.PLC_R_动作POS = r["动作POS"].ToString();
                        Rr.PLC_R_结果判定 = "FF";
                        Rr.PLC_R_错误代码 = "";
                        Rr.PLC_R_错误描述 = "操作员终止检测";
                        Rr.PLC_R_R1 = "";
                        Rr.PLC_R_R2 = "";
                        Rr.PLC_R_R3 = "";
                        Rr.PLC_R_R4 = "";
                        Rr.PLC_R_R5 = "";
                        Rr.PLC_R_R6 = "";
                        Rr.PLC_R_R7 = "";
                        Rr.PLC_R_R8 = "";
                        Rr.PLC_R_R9 = "";
                        Rr.PLC_R_R10 = "";
                        lock (ResultRS)
                        {
                            ResultRS.Add(Rr);
                        }
                        //PLC_CURR_POS = -1;
                        iDZJP = -1;
                        iMMresult = 3;
                    }

                    ///主状态是量测状态。
                    if (PLC_S_Ready == "1" && PLC_S_主状态 == "0")
                    {
                        if (strCPSN != "")
                        {
                            if (blwork == false && iDZJP == -1 && MachineManager.BLWork == false)
                            {
                                lock (ResultRS)
                                {
                                    //动作节拍开始。
                                    T_工作时间_开始时间 = System.DateTime.Now;
                                    iDZJP = 0;
                                    iMMresult = 0;
                                    PLC_CURR_POS = 0;
                                    CZMaster.MasterLog.WriteLog(this.strMachineName + " 开始检测");
                                }
                            }
                        }
                    }


                    if (PlcAR.iPLCActResult == 2)
                    {
                        PlcAR.iPLCActResult = 0;
                        //检测出错，停下来 
                        //blwork = false;
                        //PLC_CURR_POS = -1;
                        iDZJP = -1;
                    }

                    PLC_S_Work_OLD = PLC_S_Work;

                }
                //if (plc.PLC_Other.Count != 0)
                //{
                //    //处理特殊指令 
                //    foreach (string s in plc.PLC_Other)
                //    {
                //        string ss = s.Substring(0, 1);
                //        if (ss == "R")     //R指令
                //        {
                //            fun_PLC处理指令_R(s);
                //            if (PLC_R_结果判定 == "2")
                //            {
                //                blerror = true;
                //            }
                //            if (PLC_R_动作ID == "255")
                //            {
                //                //急停
                //                blwork = false;
                //            }
                //        }
                //    }
                //}
                //if (PLC_R_结果判定 == "2")
                //{
                //    blerror = true;
                //}
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public void fun_PLC处理指令_S(string str)    //处理S指令
        {
            //理论92+17  实际length 
            int count_理论 = 121;
            int count_实际 = str.Length;
            if (count_实际 >= count_理论)
            {
                string[] s = str.Split(new char[] { '|' });
                PLC_S_机台类型 = s[1];
                PLC_S_PLC_COUNT_ID = s[2];
                PLC_S_主状态 = s[3];
                PLC_S_手自动 = s[4];
                PLC_S_组队 = s[5];
                PLC_S_检测ID = s[6];
                PLC_S_PLC动作 = s[7];
                PLC_S_PLC_POS = s[8];
                if (PLC_S_PLC_POS != PLC_S_PLC_POS_OLD)
                {
                    CZMaster.MasterLog.WriteLog(string.Format("{0}, POS 从 {1} 变化到 {2}  当前PLC_CURR_POS={3}", strMachineName, PLC_S_PLC_POS_OLD, PLC_S_PLC_POS, PLC_CURR_POS));

                    PLC_S_PLC_POS_OLD = PLC_S_PLC_POS;
                }
                PLC_S_激光AD = s[9];     //
                PLC_S_力AD = s[10];       //    //
                PLC_S_PLC_X = s[11];
                PLC_S_PLC_Y = s[12];
                PLC_S_动作已用时间 = s[13];
                PLC_S_产品已用时间 = s[14];
                PLC_S_断路器开合次数 = s[15];
                PLC_S_Ready = s[16];
                PLC_S_指令等待 = s[17];
                PLC_S_Work = s[18];
                PLC_S_激光合闸标定值 = s[19];
                PLC_S_激光分闸标定值 = s[20];

                PLC_S_TF = s[21];
                PLC_S_TS1 = s[22] + "0";
                PLC_S_TS2 = s[23];
                // PLC_S_PLC_SYN = s[24];

                //if (V12电压状态 == false)
                {
                    if (int.Parse(PLC_S_TS1) >= 20000)
                    {
                        PLC_S_TS_T = PLC_S_TS1;
                    }
                    else
                    {
                        PLC_S_TS_T = PLC_S_TS2;
                        //int iii = int.Parse(PLC_S_TS2);
                        //if (iii > 1)
                        //{
                        //    PLC_S_TS_T = (iii - 10).ToString();   //去掉继电器时间
                        //}
                    }
                }
                //else
                //{
                //    PLC_S_TS_T = ((int)(System.DateTime.Now - Vtime).TotalMilliseconds).ToString();
                //}
                //if (PLC_S_TF == "0")
                //{
                //    PLC_S_TS1_T = PLC_S_TS1;
                //    PLC_S_TS2_T = PLC_S_TS2;
                //}
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(string.Format("S指令长度不对，理论121，实际‘{0}’", count_实际));
            }
        }

        //public void fun_PLC处理指令_R(string str)    //处理R指令
        //{
        //    //动作ID FF  blwork = false    停止钟摆  全局变量清空
        //    //02H| 61H|检测ID(3)|动作ID(3)|动作POS(3)|结果判定(1)|错误代码(2)|R1(5) |R2(5) |R3(5) |P4(5) |P5(5) |P6(5) |P7(5) |P8(5) |P9(5) |P10(5)|03H
        //    int count_理论 = 63;
        //    int count_实际 = str.Length;
        //    if (count_实际 >= count_理论)
        //    {
        //        PLC_R_检测ID = str.Substring(1, 3);
        //        PLC_R_动作ID = str.Substring(4, 3);
        //        PLC_R_动作POS = str.Substring(7, 3);
        //        PLC_R_结果判定 = str.Substring(10, 1);
        //        PLC_R_错误代码 = str.Substring(11, 2);
        //        PLC_R_R1 = str.Substring(13, 5);
        //        PLC_R_R2 = str.Substring(18, 5);
        //        PLC_R_R3 = str.Substring(23, 5);
        //        PLC_R_R4 = str.Substring(28, 5);
        //        PLC_R_R5 = str.Substring(33, 5);
        //        PLC_R_R6 = str.Substring(38, 5);
        //        PLC_R_R7 = str.Substring(43, 5);
        //        PLC_R_R8 = str.Substring(48, 5);
        //        PLC_R_R9 = str.Substring(53, 5);
        //        PLC_R_R10 = str.Substring(58, 5);
        //    }
        //}

        public void fun_检测参数(DataTable dt)    //P指令
        {
            plc.fun_检测参数(dt);
        }
        #endregion
    }



}
