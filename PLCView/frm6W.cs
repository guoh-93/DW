using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CZMaster;
using System.Threading;

namespace PLCView
{
    public partial class frm6W : UserControl
    {

        #region 成员变量

        /// <summary>
        /// 工作台名称
        /// </summary>
        public string MachineName = "";

        /// <summary>
        /// 使用串口名称
        /// </summary>
        public string MachineComPort = "";

        /// <summary>
        /// 当前正在量测的产品号
        /// </summary>
        public string SN = "";

        /// <summary>
        /// 当前正在量测GUID
        /// </summary>
        public string GUID = "";

        /// <summary>
        /// 工作状态
        /// </summary>
        public string blWROK_OLD = "";

        /// <summary>
        /// 上个工作状态
        /// </summary>
        public int PLC_POS_OLD = -1;

        public int PLC_POS_team_POS_OLD = -1;

        public int PLC_POS_team_POS = -1;

        /// <summary>
        /// 开始检测时间
        /// </summary>
        DateTime StartTime;

        /// <summary>
        /// 最重要变量 用来驱动设备的设备类型,由连接按钮生成
        /// </summary>
        PLCC.W6_MachineAdapter mad;

        

        /// <summary>
        ///次重要变量,前置00动作。后置FE动作，重组后的分解动作表，由dt分解动作表生成
        /// </summary>
        DataTable dt分解动作重组表;


        /// <summary>
        /// 动作子表
        /// </summary>
        DataTable dt子动作表;

        /// <summary>
        /// 动作组表
        /// </summary>
        DataTable dt组动作表;

        /// <summary>
        /// 由dt子动作表生成，带有状态和量测结果列
        /// </summary>
        DataTable dt子动作列表;


        /// <summary>
        /// 机台设备类型，检测类型
        /// </summary>
        DataTable dt检测机台表;

        /// <summary>
        /// 所有检测动作
        /// </summary>
        DataTable dt动作表;

        /// <summary>
        /// 检测类别ID
        /// </summary>
        string strCurrJCLBID = "";
        /// <summary>
        /// 检测类别
        /// </summary>
        string strCurrJCLB = "";


        
        //检测动作主表
        DataTable dtM;
        
        string strCX = "";

        Boolean blClose = false;

        System.Timers.Timer tm_R;


        string strConn = "";
        #endregion

        #region 类初始化

        public frm6W()
        {
            InitializeComponent();   
        }

        private void frm6W_Load(object sender, EventArgs e)
        {
            strConn = CPublic.Var.geConn("PLC");
            fun_load动作主表();
            fun_加载检测机台表();
            fun_连接设备();
            


            ///启动R循环
            tm_R = new System.Timers.Timer() { Interval = 100, AutoReset = false };
            tm_R.Elapsed += tm_R_Elapsed;
            tm_R.Start();

        }


        #endregion
         
        #region 数据加载

        //加载动作表
        private void fun_加载动作表()
        {
            string sql = "select * from ABB动作表";
            dt动作表 = MasterSQL.Get_DataTable(sql,strConn);
        }
        private void fun_load子动作表()
        {
            string sql = string.Format("select * from ABB检测组动作子表 where 检测名称='{0}' order by 动作POS",txt_jianceleix.Text);
            dt子动作表 = MasterSQL.Get_DataTable(sql,strConn);
        }
        private void fun_加载检测机台表()
        {
            //加载设备框的数据，也就是是机台名称,当前电脑所拥有的机台的名称
            string sql = string.Format("select * from 检测机台表 where 使用='1' and 机台名称 ='{0}'", MachineName);    //在使用过程中的机台才能显示  使用的标志是1，要本台工控机能够使用的机台
            dt检测机台表 = MasterSQL.Get_DataTable(sql,strConn);
        
            text_shebei.Text = MachineName;
        }
        private void fun_load动作组表()
        {
            string sql = string.Format("select * from ABB检测类型动作组表 where 检测名称='{0}' order by 检测组POS", txt_jianceleix.Text);
            dt组动作表 = MasterSQL.Get_DataTable(sql, strConn);           
        }
        private void fun_load动作主表()
        {
            dtM = new DataTable();
            string sql = string.Format("select * from ABB检测类型主表");
            dtM = MasterSQL.Get_DataTable(sql, strConn);//检测类型主表

        }
        //private void fun_组显示()
        //{
        //    try
        //    {
        //        txt_JCNR.Text = dt组动作表.Rows[0]["检测内容"].ToString();
        //        txt_JCYQ.Text = dt组动作表.Rows[0]["检测要求"].ToString();
        //        txt_id.Text = dt组动作表.Rows[0]["检测ID"].ToString();

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}
        private void fun_生成子动作列表()
        {
            dt子动作列表 = dt子动作表;
            dt子动作列表.Columns.Add("状态");
            dt子动作列表.Columns.Add("动作判定要求");
            dt子动作列表.Columns.Add("执行");
            dt子动作列表.Columns.Add("工作时间");
            
            
            foreach (DataRow dr in dt子动作列表.Rows)
            {
                DataRow[] drr = dt动作表.Select(string.Format("动作ID='{0}'", dr["动作ID"].ToString()));
                if (dr["动作判定要求"].ToString() == "")
                {
                    if (Convert.ToInt32(drr[0]["动作参数个数"]) > 0)
                    {
                        for (int g = 1; g <= Convert.ToInt32(drr[0]["动作参数个数"]); g++)
                        {
                            dr["动作判定要求"] += " " + drr[0]["P" + g + ""].ToString() + @":" + dr["P" + g + ""].ToString();
                        }
                    }
                }

            }
        }
        //检测类别的下拉框，检测类别的下拉框是随着设备的不同，所能检测的类别也不同。检测类别就是ABB主表中的检测名称
        private void shebei_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txt_jianceleix.Text = "";
                txt_jianceleix.Items.Clear(); //每次要进行清空
                DataRow[] dr = dt检测机台表.Select(string.Format("机台名称='{0}'", text_shebei.Text));     //找到机台类型，即可以做的类型操作。这个只可能查出一条数据，机台类型就是检测大类
                if (dr.Length > 0)
                {
                    DataRow[] dr1 = dtM.Select(string.Format("检测大类='{0}'", dr[0]["机台类型"].ToString()));   //根据找出的机台类型去查找检测类别，检测类别也是ABB主表中的检测名称
                    foreach (DataRow t in dr1)
                    {
                        txt_jianceleix.Items.Add(t["检测名称"].ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region 连接设备
        //连接设备的方法
         private void fun_连接设备()
        {
            if (text_shebei.Text == null)
            {
                text_shebei.Text = "";
            }
            if (text_shebei.Text.ToString() == "")
                throw new Exception("请选择需要连接的设备");
            //判断所要进行连接的设备是不是已经注册了
            if (PLCC.MachineManager.CheckMachineExists(text_shebei.Text.ToString()) != null)  //如果被注册了,需要抛出错误
                throw new Exception("该设备已经被连接了，你需要重新选择设备！");
    
            //如果该设备没有被连接
            mad = new PLCC.W6_MachineAdapter();
            foreach (DataRow r in dt检测机台表.Rows)
            {
                if (r["机台名称"].ToString() == text_shebei.Text.ToString())
                {
                    mad.strCOM = r["COM"].ToString();    //com口
                    mad.strComPara = r["COM参数"].ToString();   //com口的参数
                    mad.strVCCOM = r["电压表COM"].ToString();   //电压表的COM
                    mad.strVCCOMPara = r["电压表COM参数"].ToString();   //电压表com参数
                    mad.strMachineName = r["机台名称"].ToString();   //机台的名称
                }
            }
            PLCC.W6_MachineManager.addMachine(mad);  //把设备加进去

            mad.Start();    //设备连接
        }
            

        private void fun_断开设备()
        {
            try
            {
                blClose = true;
                if (mad != null)
                {
                    PLCC.MachineManager.Remove(mad.strMachineName);
                    mad.Close();
                    tm_R.Close();
                    tm_UI.Enabled = false;

                    this.Dispose();

                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + "     fun_断开设备");
            }
        }

        #endregion

        #region 核心事情之一。界面 TM
        private void tm_UI_Tick(object sender, EventArgs e)
        {
            int i = 0;
            i++;
            tm_UI.Enabled = false;
            try
            {
                ///0.第一次刷新到检测类型后。要加载这个类型
                ///1.从MAD得到各种状态和参数，并对状态和参数做第一步处理 比如 PLC_POS_team_POS
                ///2.刷新界面状态，灯,必刷
                ///3.刷新dt子动作列表,mad.PLC_CURR_POS 和 PLC_POS_OLD不一样的时候
                ///4.刷新dt组，1.上面的文本框，2.下面的列表。 PLC_POS_team_POS_OLD 和 PLC_POS_team_POS 不一样的时候
                ///
                txtSN.Enabled = false;
                if (mad == null) return;  //设备还没有进行注册
                fun_得到MAD后初始化检测类型();//获取设备中是否带有检测类别
                fun_亮灯函数();
                fun_参数获取();

                ///3 示例
                if (mad.PLC_CURR_POS != PLC_POS_OLD)
                {
                    fun_刷新act2视图();
                    fun_刷新详细子动作列表();
                    PLC_POS_OLD = mad.PLC_CURR_POS;
                }


                ///4 示例
                if (PLC_POS_team_POS_OLD != PLC_POS_team_POS)
                {
                    fun_刷新组及组子视图();
                    PLC_POS_team_POS_OLD = PLC_POS_team_POS;
                }


            }
            catch
            {

            }
            finally
            {
                tm_UI.Enabled = true;
            }
        }

        #endregion

        #region 核心事情之二。R消息队列
        void tm_R_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {


            }
            catch
            {
            }
            finally
            {
                tm_R.Start();
            }
        }
        #endregion

        #region 切换检测类型，切换组视图

        private void  fun_刷新act2视图()
        {
            foreach (DataRow r in dt子动作列表.Rows)
            {
                if ((int)r["动作POS"] < mad.PLC_CURR_POS)
                {
                    r["状态"] = "已检";
                }

                if ((int)r["动作POS"] == mad.PLC_CURR_POS)
                {
                    r["状态"] = "在检";
                }

                if ((int)r["动作POS"] > mad.PLC_CURR_POS)
                {
                    r["状态"] = "末检";
                }
            }
            DataView dv = new DataView(dt子动作列表);
            dv.Sort = "动作POS";
            dv.RowFilter = string.Format("检测组POS = {0}", PLC_POS_team_POS);
            gc_Act2.DataSource = dv;
        }
        //gvAct 动作组子表着色功能
        private void gv_Act2_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (gv_Act2.GetRow(e.RowHandle) == null)
                {
                    return;
                }
                int j = gv_Act2.RowCount;

                for (int i = 0; i < j; i++)
                {
                    if (gv_Act2.GetRowCellValue(e.RowHandle, "状态").ToString() == "在检")
                    {
                        e.Appearance.BackColor = Color.Blue;
                        e.Appearance.BackColor2 = Color.Blue;
                    }

                    if (gv_Act2.GetRowCellValue(e.RowHandle, "状态").ToString() == "PASS")
                    {
                        e.Appearance.BackColor = Color.Green;
                        e.Appearance.BackColor2 = Color.Green;
                    }

                    if (gv_Act2.GetRowCellValue(e.RowHandle, "状态").ToString() == "NG")
                    {
                        e.Appearance.BackColor = Color.Red;
                        e.Appearance.BackColor2 = Color.Red;
                    }

                    if (gv_Act2.GetRowCellValue(e.RowHandle, "状态").ToString() == "放弃")
                    {
                        e.Appearance.BackColor = Color.Purple;
                        e.Appearance.BackColor2 = Color.Purple;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_刷新组及组子视图()
        {

            ///1.能过当前的 mad.PLC_CURR_POS得到组POS
            ///2.gc_Act2的datasource 来源是由组POS生成的视图
            ///

            ////刷新组
            //刷新检测内容，检测要求，team_POSID
            DataRow[] r = dt组动作表.Select(string.Format("检测组POS='{0}'", PLC_POS_team_POS));
            txt_JCNR.Text = r[0]["检测内容"].ToString();
            txt_JCYQ.Text = r[0]["检测要求"].ToString();
            txt_id.Text = PLC_POS_team_POS.ToString();

            

        }

        private void fun_亮灯函数()
        {

            string str = mad.PLC_S_主状态;

            //亮灯为1，灭灯为0
            //待机灯 绿色
            if (str == "1")
            {
                dajideng.BackColor = Color.DarkGreen;
            }
            if (str != "1")
            {
                dajideng.BackColor = Color.Gray;
            }

            //工作灯  白色
            if (str == "0")
            {
                gongzuo.BackColor = Color.Black;
            }
            if (str != "0")
            {
                gongzuo.BackColor = Color.Gray;
            }
            //出仓  紫色
            if (str == "3")
            {
                chucang.BackColor = Color.Purple;
            }
            if (str != "3")
            {
                chucang.BackColor = Color.Gray;
            }
            //入仓  紫色
            if (str == "2")
            {
                rucang.BackColor = Color.Purple;
            }
            if (str != "2")
            {
                rucang.BackColor = Color.Gray;
            }
        }

        private void fun_参数获取()
        {
            //获取SN，电压，工作时间
            txtSN.Text = mad.strCPSN;
            La_dianya.Text = mad.Vcc_电压.ToString();
            if (StartTime.ToString().Substring(0, 1) != "0")
            {
                TimeSpan times = DateTime.Now - StartTime;
                La_gzsj.Text = times.ToString().Substring(3, 5); //检测已用的时间
            }
            if (La_PASS.Text == "PASS")
            {
                LA_cwyy.Visible = false;
            }
            if (mad.ResultRS.Count > 0)  //必须要大于0的时候
            {
                PLCC.ResultR rr5 = mad.ResultRS[0];


                //错误原因，错误动作
                if (rr5.PLC_R_结果判定 == "2")    //根据结果的判定来取决于是否通过检测 1表示不通过 有错误
                {
                    LA_cwyy.Visible = true;
                    LA_cwyy.Text = "错误原因：" + rr5.PLC_R_错误描述.ToString();
                }
                if (rr5.PLC_R_结果判定 == "")
                {
                    La_PASS.Visible = false;
                }
                else
                {
                    if (rr5.PLC_R_结果判定 == "1")  //0表示检测通过
                    {
                        //表示检测通过了
                        La_PASS.Visible = true;
                        La_PASS.Text = "PASS";
                        La_PASS.ForeColor = Color.Green;
                    }
                    else
                    {
                        if (rr5.PLC_R_结果判定 == "2")
                        {
                            La_PASS.Visible = true;
                            La_PASS.Text = "NG";
                            La_PASS.ForeColor = Color.Red;
                        }
                    }
                }
            }      

        }

        private void fun得到当前组POS()
        {
            DataRow[] rs = dt分解动作重组表.Select(string.Format("动作POS = {0}", mad.PLC_CURR_POS));
            if (rs.Length > 0)
            {
                PLC_POS_team_POS = (int)rs[0]["检测组POS"];
            }
        }

        private void fun_刷新详细子动作列表()
        {
            DataView dv2 = new DataView(dt子动作列表);
            dv2.Sort = "动作POS";
            gc_Act2.DataSource = dv2;

        }

        /// <summary>
        /// 把dt子动作表 分解成 dt分解动作表
        /// </summary>
        private void fun_分解动作表()
        {
            ///1.生成新的分解动作表
            ///2.分解动作表前置0
            ///3.分解动作表后置FE
            /// dt_new动作子表 = new DataTable();
            try
            {

                if (dt子动作表.Rows.Count == 0)
                {
                    throw new Exception("这个检测类型没有子动作");
                }
                //动作分解表
                DataTable dt_动作分解;
                string sql = "select * from 动作分解表";
                dt_动作分解 = MasterSQL.Get_DataTable(sql, strConn);

                dt分解动作重组表 = dt子动作表.Clone();
                dt分解动作重组表.Columns["动作POS"].ColumnName = "主动作POS";  //修改列名
                dt分解动作重组表.Columns.Add("动作POS");
                dt分解动作重组表.Columns.Add("主动作ID");

                if (txt_jianceleix.Text == null) return;
                if (txt_jianceleix.Text == "") return;

                int i = 1;
                DataRow r3 = dt分解动作重组表.NewRow();
                r3["检测ID"] = dt子动作表.Rows[0]["检测ID"].ToString();
                r3["检测名称"] = dt子动作表.Rows[0]["检测名称"].ToString();
                r3["检测组POS"] = 0;
                r3["检测组内POS"] = 0;
                r3["主动作POS"] = 0;
                r3["动作ID"] = "00";
                r3["动作POS"] = i++;
                r3["P1"] = 0.5;
                dt分解动作重组表.Rows.Add(r3);

            
                foreach (DataRow r in dt子动作表.Rows)
                {

                    DataRow[] drfj = dt_动作分解.Select(string.Format("主动作ID='{0}'", r["动作ID"].ToString()));
                    if (drfj.Length > 0)
                    {
                        foreach (DataRow r1 in drfj)
                        {
                            DataRow[] drdz = dt动作表.Select(string.Format("动作ID='{0}'", r1["子动作ID"].ToString()));
                            if (drdz.Length > 0)
                            {
                                DataRow r2 = dt分解动作重组表.NewRow();
                                r2["动作子表GUID"] = System.Guid.NewGuid().ToString();
                                r2["检测ID"] = r["检测ID"];
                                r2["检测名称"] = r["检测名称"];
                                r2["检测组POS"] = r["检测组POS"];
                                r2["检测组内POS"] = r["检测组内POS"];
                                r2["主动作POS"] = r["动作POS"];
                                r2["动作ID"] = r1["子动作ID"];
                                r2["动作说明"] = drdz[0]["动作说明"];
                                r2["动作描述"] = drdz[0]["动作描述"];
                                r2["报表节点"] = r["报表节点"];    //报表节点
                                if (r1["P1"].ToString() != "" || r1["P2"].ToString() != "" || r1["P3"].ToString() != "" || r1["P4"].ToString() != "" || r1["P5"].ToString() != "")
                                {
                                    if (r2["动作ID"].ToString() == "40" || r2["动作ID"].ToString() == "41")   //如果分解动作有40和41的子动作的话
                                    {
                                        r2["P2"] = Convert.ToInt32(r1["P1"]) * 10000;

                                        if (r1["P3"].ToString() == "")  //如果参数P3为空的话   P3赋值为0
                                        {
                                            r2["P3"] = 0;
                                        }
                                        else
                                        {
                                            r2["P3"] = r1["P3"];
                                        }
                                    }
                                    else
                                    {
                                        r2["P2"] = r1["P2"];
                                        r2["P3"] = r1["P3"];
                                    }
                                    r2["P1"] = r1["P1"]; r2["P4"] = r1["P4"]; r2["P5"] = r1["P5"];
                                }
                                else
                                {
                                    if (r2["动作ID"].ToString() == "40" || r2["动作ID"].ToString() == "41")
                                    {
                                        r2["P2"] = Convert.ToInt32(r["P1"]) * 10000;
                                        if (r1["P3"].ToString() == "")  //如果参数P3为空的话   P3赋值为0
                                        {
                                            r2["P3"] = 0;
                                        }
                                        else
                                        {
                                            r2["P3"] = r["P3"];
                                        }
                                    }
                                    else
                                    {
                                        r2["P2"] = r["P2"];
                                        r2["P3"] = r["P3"];
                                    }
                                    r2["P1"] = r["P1"]; r2["P4"] = r["P4"]; r2["P5"] = r["P5"];
                                }
                                r2["参数个数及说明"] = drdz[0]["动作参数个数"].ToString() + @" 个 " + drdz[0]["动作参数说明"].ToString();
                                r2["动作POS"] = i;
                                r2["主动作ID"] = r["动作ID"];
                                dt分解动作重组表.Rows.Add(r2);
                                i++;
                            }
                        }
                    }
                    else
                    {
                        DataRow r2 = dt分解动作重组表.NewRow();
                        r2["动作子表GUID"] = r["动作子表GUID"];
                        r2["检测ID"] = r["检测ID"];
                        r2["检测名称"] = r["检测名称"];
                        r2["检测组POS"] = r["检测组POS"];
                        r2["检测组内POS"] = r["检测组内POS"];
                        r2["主动作POS"] = r["动作POS"];
                        r2["动作ID"] = r["动作ID"];
                        r2["动作说明"] = r["动作说明"];
                        r2["动作描述"] = r["动作描述"];
                        r2["报表节点"] = r["报表节点"];   //报表节点
                        r2["P1"] = r["P1"]; r2["P2"] = r["P2"]; r2["P3"] = r["P3"]; r2["P4"] = r["P4"]; r2["P5"] = r["P5"];   //参数
                        r2["P6"] = r["P6"]; r2["P7"] = r["P7"]; r2["P8"] = r["P8"]; r2["P9"] = r["P9"]; r2["P10"] = r["P10"];
                        r2["参数个数及说明"] = r["参数个数及说明"];
                        r2["动作POS"] = i;
                        r2["主动作ID"] = r["动作ID"];
                        dt分解动作重组表.Rows.Add(r2);
                        i++;
                    }
                }

                DataRow r4 = dt分解动作重组表.NewRow();
                r4["检测ID"] = dt子动作表.Rows[0]["检测ID"].ToString();
                r4["检测名称"] = dt子动作表.Rows[0]["检测名称"].ToString();
                r4["检测组POS"] = 0;
                r4["检测组内POS"] = 0;
                r4["主动作POS"] = 0;
                r4["动作ID"] = "FE";
                r4["动作POS"] = i++;
                r4["P1"] = 0.5;
                dt分解动作重组表.Rows.Add(r4);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm6W - 分解动作表");
                throw ex;
            }

        }

        private void fun_切换类型()
        {
            ///1.生到和类型相关的 主表，组表，子动作表
            ///2.分解子动作表为 动作分解表
            ///3.把动作主表，动作分解表给MAD
            ///4.生成 dt子动作列表
            ///张宇2016/1/12调试时注释
            if (mad == null)
            {
                throw new Exception("请选择设备");
            }
            fun_加载动作表();
            fun_load动作组表();
            fun_load子动作表();

            fun_生成子动作列表();
            fun_分解动作表();
            
            





            //fun_组显示();
                //显示组
            ///张宇2016/1/12调试时注释 
            DataTable dt_参数 = new DataTable();
            string sql = string.Format("select * from ABB检测类型主表 where 检测名称='{0}'", txt_jianceleix.Text);
            dt_参数 = MasterSQL.Get_DataTable(sql, strConn);
            //把参数传给你

            ///重要方法 AQUA注
            mad.fun_检测参数(dt_参数);

            strCurrJCLBID = dt_参数.Rows[0]["检测ID"].ToString();
            strCurrJCLB = txt_jianceleix.Text;

            mad.dt_检测类型主表 = dt_参数;
            //传递重新组合后的参数
            ///重要属性 AQUA注
            mad.dt_检测组动作子表 = dt分解动作重组表;

            
        }

        private void fun_得到MAD后初始化检测类型()
        {

            if (strCurrJCLBID == "")
            {
                try
                {

                    //从MAD获取ID
                    string ss = mad.PLC_S_检测ID;
                    //如果MAD的ID 为0 那么不做处理
                    //如果MAD的ID不为0 那么转移成整数后再转换成字符。得到ID

                    ss = Convert.ToInt32(ss).ToString();

                    if (ss == "0") return;

                    //从DT里得到DR
                    DataRow[] dr = dtM.Select(string.Format("检测ID='{0}'", ss));
                    if (dr.Length > 0)
                    {
                        //给下接框富值
                        txt_jianceleix.Text = dr[0]["检测名称"].ToString();
                        DataRow[] drr1 = dt检测机台表.Select(string.Format("机台名称='{0}'", text_shebei.Text.ToString()));
                        if (drr1.Length > 0)
                        {
                            strCX = drr1[0]["产线"].ToString();
                        }
                        //调用更换按钮
                        simpleButton2_Click(null, null);
                    }

                }
                catch (Exception ex)
                {
                    CZMaster.MasterLog.WriteLog(ex.Message + "  fun_得到MAD后初始化检测类型");
                }
            }
        }

        #endregion

        #region 界面操作

        //更换操作
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                fun_切换类型();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
            txtSN.Text = txtSN.Text.ToUpper().Trim();
            if (txtSN.Text == "") MessageBox.Show("SN号不能为空");
            mad.strCPSN = txtSN.Text;
            }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }

        #endregion

        private void frm6W_ParentChanged(object sender, EventArgs e)
        {
            if (this.Parent == null)
            {
                fun_断开设备();
                mad.blTeam = false;
            }
        }

    }
}
