using System;
using System.Collections.Generic;
using System.Text;

namespace PLCC
{
    public static class MachineManager
    {

        #region        
        public static List<MachineAdapter> li_MachineAdapter = new List<MachineAdapter>();    //组队MachineAdapter控制器
        private static System.Timers.Timer tm = null;

        public static Boolean BLWork = false;

        public static int iPOS_TEAM = 0;
        #endregion

        #region 时序控制
        public static void  StartListen()
        {
            if (tm == null)
            {
                tm = new System.Timers.Timer() { Interval = 50, AutoReset = false };
                tm.Elapsed += tm_Elapsed;
                tm.Start();
            }

            VCControlAdapter.fun_串口初始化(CPublic.Var.li_CFG["Vcc_COM"]);
        }

        static void tm_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            Boolean bl = false;
            ///组队模式
            Boolean blTeamType = false;
            try
            {
                lock (li_MachineAdapter) 
                {
                    int count = 0;    //计数，是否所有machine处于等待状态
                    ///先判断Manager是否组队模式工作
                    foreach (MachineAdapter mm in li_MachineAdapter)
                    {
                        if (mm.blTeam == true)
                        {
                            //只要有一个工作在组队模式下。它的工作优先等级就比其它的都要高。
                            blTeamType = true;
                    
                        }
                        if (mm.blwork == true)
                        {
                            bl = true;
                        }
                    }
                    
                    if (blTeamType == true)
                    {
                        //如果在组队工作模式下。要组队都准备好后。才可以开始工作。
                        foreach (MachineAdapter mm in li_MachineAdapter)
                        {
                            lock (mm.ResultRS)
                            {
                                ///先统计组队中的多少准备完成
                                if (mm.blTeam == true)
                                {
                                    if (mm.iDZJP == 2 || mm.iDZJP == 0)  //判断是否处于等待状态
                                    {
                                        if (mm.PLC_CURR_POS != iPOS_TEAM)
                                        {
                                            count++;
                                        }
                                    }
                                    else
                                    {
                                        if (mm.PLC_CURR_POS >= 0)
                                        {
                                            if (mm.iDZJP != -1)
                                            {
                                                //只要一个组队中的没有完成
                                                count++;
                                            }
                                        }
                                        else
                                        {
                                            count++;
                                        }
                                    }
                                }
                            }
                        }

                        
                        if (count == 0)
                        {
                            iPOS_TEAM++;
                            //没有准备计数器为0，也就是准备好的意思，当所有的组队准备好后。组队就可以开始工作了。
                            foreach (MachineAdapter mm in li_MachineAdapter)
                            {
                                if (mm.blTeam == true && (mm.iDZJP == 0 || mm.iDZJP == 2) )
                                {
                                    mm.iDZJP = 1;
                                    //mm.setGoNext();
                                    
                                    bl = true;
                                    CZMaster.MasterLog.WriteLog(mm.strMachineName + " 组队等待完成 TEAMPOS:" + iPOS_TEAM);
                                }
                            }
                            
                        }

                    }
                    else
                    {
                        //如果不在组队工作模式下。单机随意工作。
                        foreach (MachineAdapter mm in li_MachineAdapter)
                        {
                            if (mm.iDZJP == 0 || mm.iDZJP == 2)   //判断是否处于等待状态
                            {
                                mm.iDZJP = 1;
                                //mm.blwork = true;
                                
                                //mm.setGoNext();
                                
                                bl = true;
                            }
                        }

                    }
                    //foreach (MachineAdapter mad in li_MachineAdapter)
                    //{
                        
                    //    ///先统计组队中的多少准备完成
                    //    if (mad.blTeam == true)
                    //    {
                    //        if (mad.blwait == true)  //判断是否处于等待状态
                    //        {

                    //        }
                    //        else
                    //        {
                    //            //只要一个组队中的没有完成
                    //            count++;
                    //            blTeamWork = false;
                    //        }
                    //    }
                    //}

                    //if (blTeamWork)
                    //{
                    //    if (count == 0)
                    //    {
                    //        foreach (MachineAdapter mm in li_MachineAdapter)
                    //        {
                    //            if (mm.blTeam == true)
                    //            {
                    //                mm.blwork = true;
                    //                mm.blwait = false;
                    //                mm.setGoNext();
                    //                bl = true;
                    //            }
                    //        }
                    //    }
                    //}
                    //else
                    //{

                    //    foreach (MachineAdapter mm in li_MachineAdapter)
                    //    {
                    //        if (mm.blTeam == false)
                    //        {
                    //            if (mm.blwait == true)  //判断是否处于等待状态
                    //            {
                    //                mm.blwait = false;
                    //                mm.setGoNext();
                    //                bl = true;
                    //            }
                    //        }
                    //    }

                    //}

                    //foreach (MachineAdapter mad in li_MachineAdapter)
                    //{
                    //    if (mad.blwork == true  )
                    //    {
                    //        if (mad.blTeam == true)   //判断是否组队
                    //        {
                    //            if (mad.blwait == true)  //判断是否处于等待状态
                    //            {

                    //            }
                    //            else
                    //            {
                    //                count++;
                    //            }
                    //            blTeamWork = true;
                    //        }
                    //        if (mad.blTeam == false)
                    //        {
                    //            if (mad.blwait == true)  //判断是否处于等待状态
                    //            {
                    //                mad.blwait = false;
                    //                mad.setGoNext();
                    //            }
                    //        }

                    //        bl = true;
                    //    }
                    //}
                    //if (blTeamWork == true)
                    //{
                    //    if (count == 0)
                    //    {
                    //        foreach (MachineAdapter mm in li_MachineAdapter)
                    //        {
                    //            if (mm.blwork == true && mm.blTeam == true)
                    //            {
                    //                mm.setGoNext();
                    //            }
                    //        }
                    //    }
                    //}
                    //else
                    //{

                    //}
                    BLWork = bl;

                    if (BLWork == false)
                    {
                        iPOS_TEAM = 0;
                    }
                }
                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                
                tm.Start();
            }
        }
        #endregion

        #region check添加移除
        /// <summary>
        /// 检查这个机台名称是不是已经在manager里注册
        /// </summary>
        /// <returns>如果有，返回这个机台的 MachineAdapter，如果没有,返回NULL</returns>
        public static MachineAdapter CheckMachineExists(string MachineName)
        {
            foreach (MachineAdapter ma in li_MachineAdapter)
            {
                if (ma.strMachineName.ToLower() == MachineName.ToLower())
                {
                    return ma;
                }
            }
            return null;    
        }

        public static void addMachine(MachineAdapter ma)
        {
            if (CheckMachineExists(ma.strMachineName) == null)
            {
                lock (li_MachineAdapter)
                {
                    li_MachineAdapter.Add(ma);
                }
            }
            else
            {
                throw new Exception("这个机台已经注册过");
            }
        }

        public static Boolean Remove(string MachineName)
        {
            try
            {
                lock (li_MachineAdapter)
                {
                    li_MachineAdapter.Remove(CheckMachineExists(MachineName));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
