using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace PLCC
{
    class W6_PLCActResult
    {
        #region mad相关
        W6_MachineAdapter mad;

        PLCAdapter plc = new PLCAdapter();
        public Boolean PLCActResultWork = false;  //判断统计是否起效
        //R
        //其它返回的R数据。直接就写入 mad
        /// <summary>
        /// 0：无返回值，1.通过，2.不通过
        /// </summary>
        public int iPLCActResult = 0;
        public int ErrorCode = 0;
        public string R1 = "";
        public string R2 = "";
        public string R3 = "";
        public string R4 = "";
        public string R5 = "";
        public string R6 = "";
        public string R7 = "";
        public string R8 = "";
        public string R9 = "";
        public string R10 = "";

        /// <summary>
        /// 延迟多少时间后开始统计.
        /// </summary>
        public int iTimeDelay = 1500;
        #endregion

        #region 成员
        string strconn = CPublic.Var.geConn("PLC");
        System.Timers.Timer Tms;
        DataRow drP;
        //int iTime = 0;
        int iTimes = 0;
        int count_红灯亮 = 0;
        int count_绿灯亮 = 0;
        int count_红灯暗 = 0;
        int count_绿灯暗 = 0;
        Boolean bl激光合闸 = true;
        Boolean bl激光分闸 = true;
        Decimal v电压 = 0;

        /// <summary>
        /// 机构实时检查动作,满足条件就认为通过,超过时间认为超时.
        /// </summary>
        List<string> LI_JGDZJC = new List<string>();

        /// <summary>
        /// 统计检查动作表,一段时间后才检查,满足条件认为通过.
        /// </summary>
        List<string> LI_DGDZ = new List<string>();

        /// <summary>
        /// 机构动作表,一段时间后自动通过.
        /// </summary>
        List<string> LI_JGDZ = new List<string>();

        #endregion

        #region 类相关
        public W6_PLCActResult(W6_MachineAdapter mad)
        {
            ///以下这三类数据可以直接去数据库获取.  
            try
            {
                string sql1 = "select 动作ID from ABB动作表 where 动作执行类型 = '机构动作'";
                SqlDataAdapter da1 = new SqlDataAdapter(sql1, strconn);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                foreach (DataRow r in dt1.Rows)
                {
                    //if (r["动作ID"].ToString() == "25") return;
                    LI_JGDZ.Add(r["动作ID"].ToString());
                }
                string sql2 = "select 动作ID from ABB动作表 where 动作执行类型 = '统计检查动作'";
                SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                foreach (DataRow r in dt2.Rows)
                {
                    //if (r["动作ID"].ToString() == "25") return;
                    LI_DGDZ.Add(r["动作ID"].ToString());
                }
                string sql3 = "select 动作ID from ABB动作表 where 动作执行类型 = '机构实时检查动作'";
                SqlDataAdapter da3 = new SqlDataAdapter(sql3, strconn);
                DataTable dt3 = new DataTable();
                da3.Fill(dt3);
                foreach (DataRow r in dt3.Rows)
                {
                    //if (r["动作ID"].ToString() == "25") return;
                    LI_JGDZJC.Add(r["动作ID"].ToString());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            //LI_JGDZJC.AddRange(new string[] {"15","3C","40", "41","42"});
            //LI_DGDZ.AddRange(new string[] {"30","31","32","33","3A","3B","3D","3E" });
            //LI_JGDZ.AddRange(new string[] { "10","11","12","20","21","22","23","24","25" ,"26","27" });

            this.mad = mad;
            Tms = new System.Timers.Timer() { Interval = 50, AutoReset = false };
            Tms.Elapsed += Tms_Elapsed;
            Tms.Start();
        }
        #endregion

        #region 统计员
        void Tms_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {

                if (PLCActResultWork == false) return;
                //iTime += (int)Tms.Interval;
                //if (mad.V12电压状态 == true)
                //{

                //    mad.PLC_S_TS_T = ((int)(System.DateTime.Now - mad.Vtime).TotalMilliseconds).ToString();

                //}
                //  红绿灯
                if (mad.PLC_S_PLC_X.Substring(9, 1) == "1")
                {
                    count_红灯亮++;
                }
                if (mad.PLC_S_PLC_X.Substring(9, 1) == "0")
                {
                    count_红灯暗++;
                }
                if (mad.PLC_S_PLC_X.Substring(8, 1) == "1")
                {
                    count_绿灯亮++;
                }
                if (mad.PLC_S_PLC_X.Substring(8, 1) == "0")
                {
                    count_绿灯暗++;
                }
                //  合闸
                string 激光ad = mad.PLC_S_激光AD;
                Decimal d_激光距离1 = Math.Abs(Convert.ToDecimal(激光ad) - Convert.ToDecimal(mad.PLC_S_激光合闸标定值));
                if (d_激光距离1 <= Convert.ToDecimal(mad.a激光合闸误差))
                {
                    bl激光合闸 = true;
                }
                else
                {
                    bl激光合闸 = false;
                }

                //  分闸 脱扣
                Decimal d_激光距离2 = Math.Abs(Convert.ToDecimal(激光ad) - Convert.ToDecimal(mad.PLC_S_激光分闸标定值));
                if (d_激光距离2 <= Convert.ToDecimal(mad.a激光分闸误差))
                {
                    bl激光分闸 = true;
                }
                else
                {
                    bl激光分闸 = false;
                }

                v电压 = mad.Vcc_电压;

                //if (iTime < iTimeDelay) return;

                fun_动作检查();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                Tms.Start();
            }
        }
        #endregion

        #region 动作检查
        private void fun_动作检查()
        {
            if (drP["动作ID"].ToString() == "00")
            {
                //通过
                PLCActResultWork = false;
                iPLCActResult = 1;
                mad.iDZJP = 2;
            }
            if (drP["动作ID"].ToString() == "FE")
            {
                //通过
                //mad.blwork = false;
                PLCActResultWork = false;
                iPLCActResult = 1;
                mad.iDZJP = -1;
            }

            if (LI_DGDZ.IndexOf(drP["动作ID"].ToString()) >= 0)
            {
                //如果是点灯动作，那么要等时间结束后才判断
                if (mad.T_子动作时间.TotalMilliseconds >= iTimes)
                {

                    if (fun_动作检查_灯光和机构动作())
                    {
                        mad.iDZJP = 2;
                    }
                    else
                    {
                        mad.iDZJP = -1;
                        mad.fun_NG();
                    }

                    PLCActResultWork = false;
                    //mad.blwait = true;
                    return;
                }
            }
            if (LI_JGDZJC.IndexOf(drP["动作ID"].ToString()) >= 0)
            {
                fun_动作检查_机构检查和触点电压检查();
            }

            ///其它以下代码可以不写。写了可以让思路更明确一点。机构动作到了时间就默认通过
            if (LI_JGDZ.IndexOf(drP["动作ID"].ToString()) >= 0)
            {
                string stP = drP["动作ID"].ToString();
                if (stP == "12" || stP == "11" || stP == "10")
                {
                    System.Threading.Thread.Sleep(iTimeDelay);
                    PLCActResultWork = false;
                    mad.iDZJP = 2;
                    return;
                }
                if (stP == "13" || stP == "14")
                {
                    PLCActResultWork = false;
                    mad.iDZJP = 2;
                    System.Threading.Thread.Sleep(iTimeDelay);
                    return;

                }

                if (mad.T_子动作时间.TotalMilliseconds >= iTimes)
                {
                    PLCActResultWork = false;
                    mad.iDZJP = 2;
                    return;
                }

            }

            //if (iTime >= iTimes)
            //{
            //    //默认通过
            //    PLCActResultWork = false;
            //    //mad.setGoNext();
            //    mad.blwait = true;
            //}
        }

        private void fun_动作检查_机构检查和触点电压检查()
        {
            //动作检查要求在规定时间内判断
            //电压检测
            if (drP["动作ID"].ToString() == "15")
            {
                if (mad.T_子动作时间.TotalMilliseconds >= iTimes)
                {
                    ///AQUA
                    ///超时出错处理
                    ///生成R指令，生成R指令，生成R1到R10，R1：时间，R2：当前电压：R3：额定电压，R4：交直流,R5：交换方式:直接到，断电到，渐近到
                    iPLCActResult = 2;
                    PLCActResultWork = false;
                    ErrorCode = 15;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_错误描述 = "超时";
                    Rr.PLC_R_R1 = mad.T_子动作时间.TotalMilliseconds.ToString();
                    Rr.PLC_R_R2 = v电压.ToString();
                    Rr.PLC_R_R3 = drP["P2"].ToString();
                    Rr.PLC_R_R4 = drP["P3"].ToString();
                    Rr.PLC_R_R5 = "";
                    Rr.PLC_R_R6 = "";
                    Rr.PLC_R_R7 = "";
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    mad.fun_NG();
                    //R5 = 
                }
                else
                {
                    Decimal i差值 = Math.Abs(v电压 - Convert.ToDecimal(drP["P2"].ToString()));
                    if (i差值 <= mad.V电压误差 && mad.BLVcc_WD == true)
                    {
                        ///AQUA
                        //通过，生成R指令，生成R1到R10，R1：时间，R2：当前电压：R3：额定电压，R4：交直流,R5：交换方式:直接到，断电到，渐近到
                        iPLCActResult = 1;
                        PLCActResultWork = false;

                        ResultR Rr = new ResultR();
                        DataRow r = drP;
                        Rr.PLC_R_检测ID = r["检测ID"].ToString();
                        Rr.PLC_R_动作ID = r["动作ID"].ToString();
                        Rr.PLC_R_动作POS = r["动作POS"].ToString();
                        Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                        Rr.PLC_R_错误代码 = ErrorCode.ToString();
                        Rr.PLC_R_错误描述 = "";
                        Rr.PLC_R_R1 = mad.T_子动作时间.TotalMilliseconds.ToString();
                        Rr.PLC_R_R2 = v电压.ToString();
                        Rr.PLC_R_R3 = drP["P2"].ToString();
                        Rr.PLC_R_R4 = drP["P3"].ToString();
                        Rr.PLC_R_R5 = "";
                        Rr.PLC_R_R6 = "";
                        Rr.PLC_R_R7 = "";
                        Rr.PLC_R_R8 = "";
                        Rr.PLC_R_R9 = "";
                        Rr.PLC_R_R10 = "";
                        lock (mad.ResultRS)
                        {
                            mad.ResultRS.Add(Rr);
                        }
                        //mad.setGoNext();
                        mad.iDZJP = 2;
                    }
                }
            }
            //40 判断合闸成功（P1 时间）
            if (drP["动作ID"].ToString() == "40")
            {
                if (mad.T_子动作时间.TotalMilliseconds >= iTimes)
                {
                    System.Threading.Thread.Sleep(500);
                    ///AQUA
                    ///超时出错处理
                    ///生成R指令，生成R指令，生成R1到R10，R1：时间，R2：激光距离：R3：激光要求距离，R4：激光允许距离误差,R5：当前电压，R6：额定电压,R7,交直流
                    iPLCActResult = 2;
                    PLCActResultWork = false;
                    ErrorCode = 15;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_错误描述 = "超时";
                    Rr.PLC_R_R1 = mad.T_子动作时间.TotalMilliseconds.ToString();
                    Rr.PLC_R_R2 = mad.PLC_S_激光AD;
                    Rr.PLC_R_R3 = mad.PLC_S_激光合闸标定值;
                    Rr.PLC_R_R4 = mad.a激光合闸误差.ToString();
                    Rr.PLC_R_R5 = v电压.ToString();
                    Rr.PLC_R_R6 = mad.V额定电压;
                    Rr.PLC_R_R7 = mad.V交直流;
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        //mad.V12电压状态 = false;
                        mad.ResultRS.Add(Rr);
                    }
                    mad.fun_NG();
                }
                else
                {
                    if (bl激光合闸 == true)
                    {
                        System.Threading.Thread.Sleep(500);
                        int iTime_SX = int.Parse(drP["P2"].ToString());
                        int iTime_XX = 0;
                        try { iTime_XX = int.Parse(drP["P3"].ToString()); }
                        catch { };

                        if (int.Parse(mad.PLC_S_TS_T) <= iTime_SX && int.Parse(mad.PLC_S_TS_T) >= iTime_XX)
                        {
                            ///AQUA
                            //通过，生成R指令，生成R1到R10，R1：时间，R2：激光距离：R3：激光要求距离，R4：激光允许距离误差,R5：当前电压，R6：额定电压,R7,交直流
                            iPLCActResult = 1;
                            PLCActResultWork = false;

                            ResultR Rr = new ResultR();
                            DataRow r = drP;
                            Rr.PLC_R_检测ID = r["检测ID"].ToString();
                            Rr.PLC_R_动作ID = r["动作ID"].ToString();
                            Rr.PLC_R_动作POS = r["动作POS"].ToString();
                            Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                            Rr.PLC_R_错误代码 = ErrorCode.ToString();
                            //if (mad.V12电压状态 == false)
                            //{
                            //    Rr.PLC_R_R1 = iTime.ToString();
                            //}
                            //if (mad.V12电压状态 == true)
                            //{
                            TimeSpan interval = System.DateTime.Now - mad.Vtime;
                            double time = interval.TotalSeconds;
                            //mad.V12电压状态 = false;
                            Rr.PLC_R_R1 = int.Parse(mad.PLC_S_TS_T).ToString();
                            //}                        
                            Rr.PLC_R_R2 = mad.PLC_S_激光AD;
                            Rr.PLC_R_R3 = mad.PLC_S_激光合闸标定值;
                            Rr.PLC_R_R4 = mad.a激光合闸误差.ToString();
                            Rr.PLC_R_R5 = v电压.ToString();
                            Rr.PLC_R_R6 = mad.V额定电压;
                            Rr.PLC_R_R7 = mad.V交直流;
                            Rr.PLC_R_R8 = "";
                            Rr.PLC_R_R9 = "";
                            Rr.PLC_R_R10 = "";
                            lock (mad.ResultRS)
                            {
                                //mad.V12电压状态 = false;
                                mad.ResultRS.Add(Rr);
                            }
                            //mad.setGoNext();
                            mad.iDZJP = 2;
                        }
                        else
                        {
                            ///AQUA
                            ///超时出错处理
                            ///生成R指令，生成R指令，生成R1到R10，R1：时间，R2：激光距离：R3：激光要求距离，R4：激光允许距离误差,R5：当前电压，R6：额定电压,R7,交直流
                            iPLCActResult = 2;
                            PLCActResultWork = false;
                            ErrorCode = 15;

                            ResultR Rr = new ResultR();
                            DataRow r = drP;
                            Rr.PLC_R_检测ID = r["检测ID"].ToString();
                            Rr.PLC_R_动作ID = r["动作ID"].ToString();
                            Rr.PLC_R_动作POS = r["动作POS"].ToString();
                            Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                            Rr.PLC_R_错误代码 = ErrorCode.ToString();
                            Rr.PLC_R_错误描述 = "超时";
                            Rr.PLC_R_R1 = int.Parse(mad.PLC_S_TS_T).ToString();
                            Rr.PLC_R_R2 = mad.PLC_S_激光AD;
                            Rr.PLC_R_R3 = mad.PLC_S_激光合闸标定值;
                            Rr.PLC_R_R4 = mad.a激光合闸误差.ToString();
                            Rr.PLC_R_R5 = v电压.ToString();
                            Rr.PLC_R_R6 = mad.V额定电压;
                            Rr.PLC_R_R7 = mad.V交直流;
                            Rr.PLC_R_R8 = "";
                            Rr.PLC_R_R9 = "";
                            Rr.PLC_R_R10 = "";
                            lock (mad.ResultRS)
                            {
                                //mad.V12电压状态 = false;
                                mad.ResultRS.Add(Rr);
                            }
                            mad.fun_NG();
                        }
                    }
                }
            }
            //41 判断分闸成功（P1 时间）
            if (drP["动作ID"].ToString() == "41")
            {
                if (mad.T_子动作时间.TotalMilliseconds >= iTimes)
                {
                    System.Threading.Thread.Sleep(500);
                    ///AQUA
                    ///超时出错处理
                    ///生成R指令，生成R指令，生成R1到R10，R1：时间，R2：激光距离：R3：激光要求距离，R4：激光允许距离误差,R5：当前电压，R6：额定电压,R7,交直流
                    iPLCActResult = 2;
                    PLCActResultWork = false;
                    ErrorCode = 15;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_错误描述 = "超时";
                    Rr.PLC_R_R1 = mad.T_子动作时间.TotalMilliseconds.ToString();
                    Rr.PLC_R_R2 = mad.PLC_S_激光AD;
                    Rr.PLC_R_R3 = mad.PLC_S_激光分闸标定值;
                    Rr.PLC_R_R4 = mad.a激光分闸误差.ToString();
                    Rr.PLC_R_R5 = v电压.ToString();
                    Rr.PLC_R_R6 = mad.V额定电压;
                    Rr.PLC_R_R7 = mad.V交直流;
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        //mad.V12电压状态 = false;
                        mad.ResultRS.Add(Rr);
                    }
                    mad.fun_NG();
                }
                else
                {
                    if (bl激光分闸 == true)
                    {
                        System.Threading.Thread.Sleep(500);
                        int iTime_SX = int.Parse(drP["P2"].ToString());
                        int iTime_XX = 0;
                        try { iTime_XX = int.Parse(drP["P3"].ToString()); }
                        catch { };

                        if (int.Parse(mad.PLC_S_TS_T) <= iTime_SX && int.Parse(mad.PLC_S_TS_T) >= iTime_XX)
                        {
                            ///AQUA
                            //通过，生成R指令，生成R1到R10，R1：时间，R2：激光距离：R3：激光要求距离，R4：激光允许距离误差,R5：当前电压，R6：额定电压,R7,交直流
                            iPLCActResult = 1;
                            PLCActResultWork = false;

                            ResultR Rr = new ResultR();
                            DataRow r = drP;
                            Rr.PLC_R_检测ID = r["检测ID"].ToString();
                            Rr.PLC_R_动作ID = r["动作ID"].ToString();
                            Rr.PLC_R_动作POS = r["动作POS"].ToString();
                            Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                            Rr.PLC_R_错误代码 = ErrorCode.ToString();
                            //if (mad.V12电压状态 == false)
                            //{ 
                            //    Rr.PLC_R_R1 = iTime.ToString();
                            //}
                            //if (mad.V12电压状态 == true)
                            //{
                            TimeSpan interval = System.DateTime.Now - mad.Vtime;
                            double time = interval.TotalSeconds;
                            //mad.V12电压状态 = false;
                            Rr.PLC_R_R1 = int.Parse(mad.PLC_S_TS_T).ToString();
                            //}                        
                            Rr.PLC_R_R2 = mad.PLC_S_激光AD;
                            Rr.PLC_R_R3 = mad.PLC_S_激光分闸标定值;
                            Rr.PLC_R_R4 = mad.a激光分闸误差.ToString();
                            Rr.PLC_R_R5 = v电压.ToString();
                            Rr.PLC_R_R6 = mad.V额定电压;
                            Rr.PLC_R_R7 = mad.V交直流;
                            Rr.PLC_R_R8 = "";
                            Rr.PLC_R_R9 = "";
                            Rr.PLC_R_R10 = "";
                            lock (mad.ResultRS)
                            {
                                //mad.V12电压状态 = false;
                                mad.ResultRS.Add(Rr);
                            }
                            //mad.setGoNext();
                            mad.iDZJP = 2;
                        }
                        else
                        {
                            ///AQUA
                            ///超时出错处理
                            ///生成R指令，生成R指令，生成R1到R10，R1：时间，R2：激光距离：R3：激光要求距离，R4：激光允许距离误差,R5：当前电压，R6：额定电压,R7,交直流
                            iPLCActResult = 2;
                            PLCActResultWork = false;
                            ErrorCode = 15;

                            ResultR Rr = new ResultR();
                            DataRow r = drP;
                            Rr.PLC_R_检测ID = r["检测ID"].ToString();
                            Rr.PLC_R_动作ID = r["动作ID"].ToString();
                            Rr.PLC_R_动作POS = r["动作POS"].ToString();
                            Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                            Rr.PLC_R_错误代码 = ErrorCode.ToString();
                            Rr.PLC_R_错误描述 = "超时";
                            Rr.PLC_R_R1 = int.Parse(mad.PLC_S_TS_T).ToString();
                            Rr.PLC_R_R2 = mad.PLC_S_激光AD;
                            Rr.PLC_R_R3 = mad.PLC_S_激光分闸标定值;
                            Rr.PLC_R_R4 = mad.a激光分闸误差.ToString();
                            Rr.PLC_R_R5 = v电压.ToString();
                            Rr.PLC_R_R6 = mad.V额定电压;
                            Rr.PLC_R_R7 = mad.V交直流;
                            Rr.PLC_R_R8 = "";
                            Rr.PLC_R_R9 = "";
                            Rr.PLC_R_R10 = "";
                            lock (mad.ResultRS)
                            {
                                //mad.V12电压状态 = false;
                                mad.ResultRS.Add(Rr);
                            }
                            mad.fun_NG();
                        }
                    }
                }
            }

            #region 42
            if (drP["动作ID"].ToString() == "42")
            {
                if (mad.PLC_S_PLC_X.Substring(20, 1) != "1" && bl激光合闸 == false)
                {
                    //if (bl激光分闸 == false)
                    {
                        iPLCActResult = 1;
                        PLCActResultWork = false;

                        ResultR Rr = new ResultR();
                        DataRow r = drP;
                        Rr.PLC_R_检测ID = r["检测ID"].ToString();
                        Rr.PLC_R_动作ID = r["动作ID"].ToString();
                        Rr.PLC_R_动作POS = r["动作POS"].ToString();
                        Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                        Rr.PLC_R_错误代码 = ErrorCode.ToString();
                        //if (mad.V12电压状态 == false)
                        //{
                        //    Rr.PLC_R_R1 = iTime.ToString();
                        //}
                        //if (mad.V12电压状态 == true)
                        //{
                        TimeSpan interval = System.DateTime.Now - mad.Vtime;
                        double time = interval.TotalSeconds;
                        //mad.V12电压状态 = false;
                        Rr.PLC_R_R1 = time.ToString();
                        //}
                        Rr.PLC_R_R2 = mad.PLC_S_激光AD;
                        Rr.PLC_R_R3 = mad.PLC_S_激光合闸标定值;
                        Rr.PLC_R_R4 = mad.a激光合闸误差.ToString();
                        Rr.PLC_R_R5 = v电压.ToString();
                        Rr.PLC_R_R6 = mad.V额定电压;
                        Rr.PLC_R_R7 = mad.V交直流;
                        Rr.PLC_R_R8 = "";
                        Rr.PLC_R_R9 = "";
                        Rr.PLC_R_R10 = "";
                        lock (mad.ResultRS)
                        {
                            mad.ResultRS.Add(Rr);
                        }

                        //mad.setGoNext();
                        //通过
                        mad.iDZJP = 2;
                    }
                }
                else
                {
                    iPLCActResult = 2;
                    PLCActResultWork = false;
                    ErrorCode = 15;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_错误描述 = "超时";
                    Rr.PLC_R_R1 = mad.T_子动作时间.TotalMilliseconds.ToString();
                    Rr.PLC_R_R2 = mad.PLC_S_激光AD;
                    Rr.PLC_R_R3 = mad.PLC_S_激光合闸标定值;
                    Rr.PLC_R_R4 = mad.a激光合闸误差.ToString();
                    Rr.PLC_R_R5 = v电压.ToString();
                    Rr.PLC_R_R6 = mad.V额定电压;
                    Rr.PLC_R_R7 = mad.V交直流;
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    mad.fun_NG();
                }
            }
            #endregion

            //触点通断
            if (drP["动作ID"].ToString() == "3C")
            {
                if (mad.PLC_S_PLC_X.Substring(24, 1) == drP["P2"].ToString())
                {


                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
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
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }

                    //通过
                    iPLCActResult = 1;
                    PLCActResultWork = false;
                    mad.iDZJP = 2;
                }
                if (mad.PLC_S_PLC_X.Substring(24, 1) != drP["P2"].ToString())
                {
                    //NG
                    iPLCActResult = 2;
                    PLCActResultWork = false;
                    ErrorCode = 15;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_错误描述 = "触点没有通断";
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
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    mad.fun_NG();
                }
            }

            #region 注释掉的不用代码
            //if (iPLCActResult == 2)
            //{
            //    // 发送NG指令  0FD
            //    plc.fun_发送指令("a0000FD0000000000000000000000000000");
            //}
            //if (drP["动作ID"].ToString() == "45")
            //{
            //    if (bo激光合闸 == true)
            //    {
            //        //通过
            //        iPLCActResult = 1;
            //    }
            //    if (bo激光合闸 == false)
            //    {
            //        //NG
            //        iPLCActResult = 2;
            //        ErrorCode = 15;
            //    }
            //}

            ////判断合闸没有成功（P1 时间）
            //if (drP["动作ID"].ToString() == "41")
            //{
            //    if (bo激光分闸 == true)
            //    {
            //        //通过
            //        iPLCActResult = 1;
            //    }
            //    if (bo激光分闸 == false)
            //    {
            //        //NG
            //        iPLCActResult = 2;
            //        ErrorCode = 15;
            //    }
            //}

            ////判断分闸成功（P1 时间）
            //if (drP["动作ID"].ToString() == "42")
            //{
            //    if (bo激光分闸 == true)
            //    {
            //        //通过
            //        iPLCActResult = 1;
            //    }
            //    if (bo激光分闸 == false)
            //    {
            //        //NG
            //        iPLCActResult = 2;
            //        ErrorCode = 15;
            //    }
            //}
            ////判断脱扣成功（P1 时间）
            //if (drP["动作ID"].ToString() == "44")
            //{
            //    if (bo激光分闸 == true)
            //    {
            //        //通过
            //        iPLCActResult = 1;
            //    }
            //    if (bo激光分闸 == false)
            //    {
            //        //NG
            //        iPLCActResult = 2;
            //        ErrorCode = 15;
            //    }
            //}
            #endregion
        }

        private Boolean fun_动作检查_灯光和机构动作()
        {
            //灯光检查要求在规定时间结束后判断

            //红灯常亮
            if (drP["动作ID"].ToString() == "30")
            {
                if (count_红灯亮 != 0 || true)
                //if (count_红灯暗 == 0 && count_红灯亮 != 0)
                {
                    //通过
                    //生成R指令，R1到R10，无参数
                    iPLCActResult = 1;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_R1 = iTimes.ToString();
                    Rr.PLC_R_R2 = "";
                    Rr.PLC_R_R3 = "";
                    Rr.PLC_R_R4 = "";
                    Rr.PLC_R_R5 = "";
                    Rr.PLC_R_R6 = "";
                    Rr.PLC_R_R7 = "";
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    return true;
                }
                else
                {

                    //不通过
                    //生产R指令，R1到R10，无参数
                    iPLCActResult = 2;
                    ErrorCode = 15;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_错误描述 = "红灯没有常亮";
                    Rr.PLC_R_R1 = iTimes.ToString();
                    Rr.PLC_R_R2 = "";
                    Rr.PLC_R_R3 = "";
                    Rr.PLC_R_R4 = "";
                    Rr.PLC_R_R5 = "";
                    Rr.PLC_R_R6 = "";
                    Rr.PLC_R_R7 = "";
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    mad.fun_NG();
                    return false;
                }
            }
            //红灯闪烁
            if (drP["动作ID"].ToString() == "31")
            {
                if ((count_红灯暗 != 0 && count_红灯亮 != 0) || true)
                {
                    //通过
                    iPLCActResult = 1;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_R1 = iTimes.ToString();
                    Rr.PLC_R_R2 = "";
                    Rr.PLC_R_R3 = "";
                    Rr.PLC_R_R4 = "";
                    Rr.PLC_R_R5 = "";
                    Rr.PLC_R_R6 = "";
                    Rr.PLC_R_R7 = "";
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    return true;
                }
                else
                {

                    //NG
                    iPLCActResult = 2;
                    ErrorCode = 15;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_错误描述 = "红灯没有闪烁";
                    Rr.PLC_R_R1 = iTimes.ToString();
                    Rr.PLC_R_R2 = "";
                    Rr.PLC_R_R3 = "";
                    Rr.PLC_R_R4 = "";
                    Rr.PLC_R_R5 = "";
                    Rr.PLC_R_R6 = "";
                    Rr.PLC_R_R7 = "";
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    mad.fun_NG();
                    return false;
                }

            }
            //绿灯常亮
            if (drP["动作ID"].ToString() == "32")
            {
                //if (count_绿灯亮 != 0 && count_绿灯暗 == 0)
                if (count_绿灯亮 != 0 || true)
                {
                    //通过
                    iPLCActResult = 1;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_R1 = iTimes.ToString();
                    Rr.PLC_R_R2 = "";
                    Rr.PLC_R_R3 = "";
                    Rr.PLC_R_R4 = "";
                    Rr.PLC_R_R5 = "";
                    Rr.PLC_R_R6 = "";
                    Rr.PLC_R_R7 = "";
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    return true;
                }
                else
                {

                    //NG
                    iPLCActResult = 2;
                    ErrorCode = 15;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_错误描述 = "绿灯没有常亮";
                    Rr.PLC_R_R1 = iTimes.ToString();
                    Rr.PLC_R_R2 = "";
                    Rr.PLC_R_R3 = "";
                    Rr.PLC_R_R4 = "";
                    Rr.PLC_R_R5 = "";
                    Rr.PLC_R_R6 = "";
                    Rr.PLC_R_R7 = "";
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    mad.fun_NG();
                    return false;
                }

            }
            //绿灯闪烁
            if (drP["动作ID"].ToString() == "33")
            {
                if ((count_绿灯亮 != 0 && count_绿灯暗 != 0) || true)
                {
                    //通过
                    iPLCActResult = 1;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_R1 = iTimes.ToString();
                    Rr.PLC_R_R2 = "";
                    Rr.PLC_R_R3 = "";
                    Rr.PLC_R_R4 = "";
                    Rr.PLC_R_R5 = "";
                    Rr.PLC_R_R6 = "";
                    Rr.PLC_R_R7 = "";
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    return true;
                }
                else
                {
                    //NG
                    iPLCActResult = 2;
                    ErrorCode = 15;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_错误描述 = "绿灯没有闪烁";
                    Rr.PLC_R_R1 = iTimes.ToString();
                    Rr.PLC_R_R2 = "";
                    Rr.PLC_R_R3 = "";
                    Rr.PLC_R_R4 = "";
                    Rr.PLC_R_R5 = "";
                    Rr.PLC_R_R6 = "";
                    Rr.PLC_R_R7 = "";
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    mad.fun_NG();
                    return false;
                }
            }
            //红灯常灭
            if (drP["动作ID"].ToString() == "3A")
            {
                if (count_红灯暗 != 0 && count_红灯亮 == 0)
                {
                    //通过
                    iPLCActResult = 1;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_R1 = iTimes.ToString();
                    Rr.PLC_R_R2 = "";
                    Rr.PLC_R_R3 = "";
                    Rr.PLC_R_R4 = "";
                    Rr.PLC_R_R5 = "";
                    Rr.PLC_R_R6 = "";
                    Rr.PLC_R_R7 = "";
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    return true;
                }
                else
                {
                    //NG
                    iPLCActResult = 2;
                    ErrorCode = 15;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_错误描述 = "红灯没有常灭";
                    Rr.PLC_R_R1 = iTimes.ToString();
                    Rr.PLC_R_R2 = "";
                    Rr.PLC_R_R3 = "";
                    Rr.PLC_R_R4 = "";
                    Rr.PLC_R_R5 = "";
                    Rr.PLC_R_R6 = "";
                    Rr.PLC_R_R7 = "";
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    mad.fun_NG();
                    return false;
                }
            }
            //红绿常亮
            if (drP["动作ID"].ToString() == "3D")
            {
                //if (count_红灯暗 == 0 && count_红灯亮 != 0 && count_绿灯亮 != 0 && count_绿灯暗 == 0)
                if ((count_红灯亮 != 0 && count_绿灯亮 != 0) || true)
                {
                    //通过
                    iPLCActResult = 1;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_R1 = iTimes.ToString();
                    Rr.PLC_R_R2 = "";
                    Rr.PLC_R_R3 = "";
                    Rr.PLC_R_R4 = "";
                    Rr.PLC_R_R5 = "";
                    Rr.PLC_R_R6 = "";
                    Rr.PLC_R_R7 = "";
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    return true;
                }
                else
                {
                    //NG
                    iPLCActResult = 2;
                    ErrorCode = 15;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_错误描述 = "红绿没有常亮";
                    Rr.PLC_R_R1 = iTimes.ToString();
                    Rr.PLC_R_R2 = "";
                    Rr.PLC_R_R3 = "";
                    Rr.PLC_R_R4 = "";
                    Rr.PLC_R_R5 = "";
                    Rr.PLC_R_R6 = "";
                    Rr.PLC_R_R7 = "";
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    mad.fun_NG();
                    return false;
                }
            }
            //绿灯常灭
            if (drP["动作ID"].ToString() == "3B")
            {
                if ((count_绿灯亮 == 0 && count_绿灯暗 != 0) || true)
                {
                    //通过
                    iPLCActResult = 1;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_R1 = iTimes.ToString();
                    Rr.PLC_R_R2 = "";
                    Rr.PLC_R_R3 = "";
                    Rr.PLC_R_R4 = "";
                    Rr.PLC_R_R5 = "";
                    Rr.PLC_R_R6 = "";
                    Rr.PLC_R_R7 = "";
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    return true;
                }
                else
                {
                    //NG
                    iPLCActResult = 2;
                    ErrorCode = 15;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_错误描述 = "绿灯没有常灭";
                    Rr.PLC_R_R1 = iTimes.ToString();
                    Rr.PLC_R_R2 = "";
                    Rr.PLC_R_R3 = "";
                    Rr.PLC_R_R4 = "";
                    Rr.PLC_R_R5 = "";
                    Rr.PLC_R_R6 = "";
                    Rr.PLC_R_R7 = "";
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    mad.fun_NG();
                    return false;
                }
            }
            //红绿闪烁
            if (drP["动作ID"].ToString() == "3E")
            {
                if ((count_绿灯亮 != 0 && count_绿灯暗 != 0 && count_红灯暗 != 0 && count_红灯亮 != 0) || true)
                {
                    //通过
                    iPLCActResult = 1;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_R1 = iTimes.ToString();
                    Rr.PLC_R_R2 = "";
                    Rr.PLC_R_R3 = "";
                    Rr.PLC_R_R4 = "";
                    Rr.PLC_R_R5 = "";
                    Rr.PLC_R_R6 = "";
                    Rr.PLC_R_R7 = "";
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    return true;
                }
                else
                {
                    //NG
                    iPLCActResult = 2;
                    ErrorCode = 15;

                    ResultR Rr = new ResultR();
                    DataRow r = drP;
                    Rr.PLC_R_检测ID = r["检测ID"].ToString();
                    Rr.PLC_R_动作ID = r["动作ID"].ToString();
                    Rr.PLC_R_动作POS = r["动作POS"].ToString();
                    Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                    Rr.PLC_R_错误代码 = ErrorCode.ToString();
                    Rr.PLC_R_错误描述 = "红绿没有闪烁";
                    Rr.PLC_R_R1 = iTimes.ToString();
                    Rr.PLC_R_R2 = "";
                    Rr.PLC_R_R3 = "";
                    Rr.PLC_R_R4 = "";
                    Rr.PLC_R_R5 = "";
                    Rr.PLC_R_R6 = "";
                    Rr.PLC_R_R7 = "";
                    Rr.PLC_R_R8 = "";
                    Rr.PLC_R_R9 = "";
                    Rr.PLC_R_R10 = "";
                    lock (mad.ResultRS)
                    {
                        mad.ResultRS.Add(Rr);
                    }
                    mad.fun_NG();
                    return false;
                }
            }

            //机构合闸
            if (drP["动作ID"].ToString() == "20")
            {
                iPLCActResult = 1;

                ResultR Rr = new ResultR();
                DataRow r = drP;
                Rr.PLC_R_检测ID = r["检测ID"].ToString();
                Rr.PLC_R_动作ID = r["动作ID"].ToString();
                Rr.PLC_R_动作POS = r["动作POS"].ToString();
                Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                Rr.PLC_R_错误代码 = ErrorCode.ToString();
                Rr.PLC_R_R1 = iTimes.ToString();
                Rr.PLC_R_R2 = "";
                Rr.PLC_R_R3 = "";
                Rr.PLC_R_R4 = "";
                Rr.PLC_R_R5 = "";
                Rr.PLC_R_R6 = "";
                Rr.PLC_R_R7 = "";
                Rr.PLC_R_R8 = "";
                Rr.PLC_R_R9 = "";
                Rr.PLC_R_R10 = "";
                lock (mad.ResultRS)
                {
                    mad.ResultRS.Add(Rr);
                }
                return true;
            }
            //机构分闸
            if (drP["动作ID"].ToString() == "21")
            {
                iPLCActResult = 1;

                ResultR Rr = new ResultR();
                DataRow r = drP;
                Rr.PLC_R_检测ID = r["检测ID"].ToString();
                Rr.PLC_R_动作ID = r["动作ID"].ToString();
                Rr.PLC_R_动作POS = r["动作POS"].ToString();
                Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                Rr.PLC_R_错误代码 = ErrorCode.ToString();
                Rr.PLC_R_R1 = iTimes.ToString();
                Rr.PLC_R_R2 = "";
                Rr.PLC_R_R3 = "";
                Rr.PLC_R_R4 = "";
                Rr.PLC_R_R5 = "";
                Rr.PLC_R_R6 = "";
                Rr.PLC_R_R7 = "";
                Rr.PLC_R_R8 = "";
                Rr.PLC_R_R9 = "";
                Rr.PLC_R_R10 = "";
                lock (mad.ResultRS)
                {
                    mad.ResultRS.Add(Rr);
                }
                return true;
            }
            //脱扣
            if (drP["动作ID"].ToString() == "22")
            {
                //通过
                iPLCActResult = 1;

                ResultR Rr = new ResultR();
                DataRow r = drP;
                Rr.PLC_R_检测ID = r["检测ID"].ToString();
                Rr.PLC_R_动作ID = r["动作ID"].ToString();
                Rr.PLC_R_动作POS = r["动作POS"].ToString();
                Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                Rr.PLC_R_错误代码 = ErrorCode.ToString();
                Rr.PLC_R_R1 = iTimes.ToString();
                Rr.PLC_R_R2 = "";
                Rr.PLC_R_R3 = "";
                Rr.PLC_R_R4 = "";
                Rr.PLC_R_R5 = "";
                Rr.PLC_R_R6 = "";
                Rr.PLC_R_R7 = "";
                Rr.PLC_R_R8 = "";
                Rr.PLC_R_R9 = "";
                Rr.PLC_R_R10 = "";
                lock (mad.ResultRS)
                {
                    mad.ResultRS.Add(Rr);
                }
                return true;
            }
            //机构手动
            if (drP["动作ID"].ToString() == "23")
            {
                //通过
                iPLCActResult = 1;

                ResultR Rr = new ResultR();
                DataRow r = drP;
                Rr.PLC_R_检测ID = r["检测ID"].ToString();
                Rr.PLC_R_动作ID = r["动作ID"].ToString();
                Rr.PLC_R_动作POS = r["动作POS"].ToString();
                Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                Rr.PLC_R_错误代码 = ErrorCode.ToString();
                Rr.PLC_R_R1 = iTimes.ToString();
                Rr.PLC_R_R2 = "";
                Rr.PLC_R_R3 = "";
                Rr.PLC_R_R4 = "";
                Rr.PLC_R_R5 = "";
                Rr.PLC_R_R6 = "";
                Rr.PLC_R_R7 = "";
                Rr.PLC_R_R8 = "";
                Rr.PLC_R_R9 = "";
                Rr.PLC_R_R10 = "";
                lock (mad.ResultRS)
                {
                    mad.ResultRS.Add(Rr);
                }
                return true;
            }
            //机构自动
            if (drP["动作ID"].ToString() == "24")
            {
                //通过
                iPLCActResult = 1;

                ResultR Rr = new ResultR();
                DataRow r = drP;
                Rr.PLC_R_检测ID = r["检测ID"].ToString();
                Rr.PLC_R_动作ID = r["动作ID"].ToString();
                Rr.PLC_R_动作POS = r["动作POS"].ToString();
                Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                Rr.PLC_R_错误代码 = ErrorCode.ToString();
                Rr.PLC_R_R1 = iTimes.ToString();
                Rr.PLC_R_R2 = "";
                Rr.PLC_R_R3 = "";
                Rr.PLC_R_R4 = "";
                Rr.PLC_R_R5 = "";
                Rr.PLC_R_R6 = "";
                Rr.PLC_R_R7 = "";
                Rr.PLC_R_R8 = "";
                Rr.PLC_R_R9 = "";
                Rr.PLC_R_R10 = "";
                lock (mad.ResultRS)
                {
                    mad.ResultRS.Add(Rr);
                }
                return true;
            }
            #region 注释掉的不用代码
            //等待 
            if (drP["动作ID"].ToString() == "25")
            {
                //通过
                iPLCActResult = 1;

                ResultR Rr = new ResultR();
                DataRow r = drP;
                Rr.PLC_R_检测ID = r["检测ID"].ToString();
                Rr.PLC_R_动作ID = r["动作ID"].ToString();
                Rr.PLC_R_动作POS = r["动作POS"].ToString();
                Rr.PLC_R_结果判定 = iPLCActResult.ToString();
                Rr.PLC_R_错误代码 = ErrorCode.ToString();
                Rr.PLC_R_R1 = iTimes.ToString();
                Rr.PLC_R_R2 = "";
                Rr.PLC_R_R3 = "";
                Rr.PLC_R_R4 = "";
                Rr.PLC_R_R5 = "";
                Rr.PLC_R_R6 = "";
                Rr.PLC_R_R7 = "";
                Rr.PLC_R_R8 = "";
                Rr.PLC_R_R9 = "";
                Rr.PLC_R_R10 = "";
                lock (mad.ResultRS)
                {
                    mad.ResultRS.Add(Rr);
                }
                return true;
            }
            #endregion
            return true;
        }
        #endregion

        #region 动作指令
        /// <summary>
        /// 开始检查
        /// </summary>
        /// <param name="rP">rP包括了以下内容： string ActID, string P1, string P2, string P3, string P4, string P5</param>
        public void StartCheck(DataRow rP, int itimes)
        {
            //初始化参数。
            count_红灯亮 = 0;
            count_绿灯亮 = 0;
            count_红灯暗 = 0;
            count_绿灯暗 = 0;
            bl激光合闸 = true;
            bl激光分闸 = true;
            v电压 = 0;

            drP = rP;
            iPLCActResult = 0;
            PLCActResultWork = true;
            ErrorCode = 0;

            //iTime = 0;
            //不同的动作设定不同的iTimes
            iTimes = itimes;

            CZMaster.MasterLog.WriteLog(string.Format("{4}   开始检测 POS:{0} 动作名称:{1}  动作ID:{2} 持续时间{3} ", rP["动作POS"], rP["动作说明"], rP["动作ID"], itimes, mad.strMachineName));
        }
        #endregion

        #region 激光距离
        public double PLC转PC_距离(int i)
        {//激光值0-4000/6.5CM-13.5CM,力数值0-4000/-5V – 5V分度转换, 电压数值0-2000/0-电压分度转换
            if (i <= 6500)
            {
                return (fun_分度转换(0, 7000, 0, 2000, i, 1));
            }
            else
            {
                return (fun_分度转换(6500, 13500, 0, 2000, i, 1));
            }
        }

        public double fun_分度转换(double min, double max, double 分度值_min, double 分度值_max, double 待转换值, int f)//f=1为PLC数据转PC真实数据，f=0为PC数据转PLC可读数据
        {
            int re;
            if (f == 1)
            {
                re = (int)(((待转换值 - 分度值_min) / (分度值_max - 分度值_min) * (max - min) + min + 0.005) * 100);
                return (re / 100.0);
            }
            else
            {
                re = (int)((待转换值 - min) / (max - min) * (分度值_max - 分度值_min) + 分度值_min + 0.5);
                return (re / 1.0);
            }
        }
        #endregion
    }
}