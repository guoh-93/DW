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
    public partial class frmABB : UserControl
    {

        #region 设备控制

        PLCC.MachineAdapter mad;  //控制设备

        System.Timers.Timer tmR;


        Boolean blClose = false;
        #endregion

        public frmABB()
        {
            InitializeComponent();
        }

        #region 变量

        /// <summary>
        /// 检测参数主表
        /// </summary>
        DataTable dtM;

        DataTable dtM_fu;

        DataTable dt_检测机台表;

        DataTable dt_动作组表;

        DataTable dt_动作分解;

        /// <summary>
        /// 属性表的数据，给产线下拉框提供数据
        /// </summary>
        DataTable dt_产线;

        DataTable dt_动作子表;

        DataTable dt_动作表;

        DataTable dt_结果子表;

        DataTable dt_结果主表;

        DataTable dt_不合格产品表;

        DataTable dt_new动作子表;

        DataTable dt_new子表;

        /// <summary>
        /// 记录上次调用的动作顺序（POS）
        /// </summary>
        string S_oldpos = "";

        string R_oldpos = "";

        //结果主表的oldR
        string oldpos_r = "";

        /// <summary>
        /// 记录工作状态
        /// </summary>
        bool blwork_OLD=false;

        /// <summary>
        /// 开始检测时间
        /// </summary>
        DateTime StartTime ;

        /// <summary>
        /// 结束检测时间
        /// </summary>
        string EndTime="";
       

        /// <summary>
        /// 产品线
        /// </summary>
        string strCx = "";

        /// <summary>
        /// 产品的SN号
        /// </summary>
        ///string strCpSN = "";

        /// <summary>
        /// 标志位
        /// </summary>
        //int flag = 0;

        Dictionary<string, string> dic = new Dictionary<string, string>();

        /// <summary>
        /// 检测类别ID
        /// </summary>
        string strCurrJCLBID = "";
        /// <summary>
        /// 检测类别
        /// </summary>
        string strCurrJCLB = "";

        string jtmc = "";

        /// <summary>
        /// 动作组视图
        /// </summary>
        DataView dv;

        /// <summary>
        /// 动作子表视图
        /// </summary>
        DataView dv1;


        DataTable dt_all子表;

        DataTable dt_分解动作子表;

        string rpos = "";

        DataTable dt_结果总表;

        DataTable dt_结果组表;

        DataTable dt_结果主动作;

        DataTable dt_结果动作;
        
        /// <summary>
        /// 机台SN号码表
        /// </summary>
        DataTable dt_SN;

        //设置标志位的
        string flagguid = "";

        string zuguid = "";

        string zhuguid = "";

        string str总GUID = "";
        Dictionary<int, string> li_组GUID = new Dictionary<int, string>();
        Dictionary<int, string> li_主GUID = new Dictionary<int, string>();

        #endregion

        #region   类加载

        private void frmABB_Load(object sender, EventArgs e)
        {
            try
            {
                
                fun_load动作分解();
                fun_loadall子表();
               // fun_load动作组表();
               // fun_load动作组子表();
                fun_load检测参数();   //把检测参数主表所有的数据加载
                fun_加载检测机台表(); //设备的下拉框
                fun_加载动作表();
                fun_动作子表重组();    //把动作子表进行重新重组
                //多行输入的两个事件   动作组的gridcontrol
                gvTeam.ShownEditor += gvTeam_ShownEditor;
                gcTeam.EditorKeyUp += gcTeam_EditorKeyUp;
                //多行输入的  子表的gridcontrol
                gvAct.ShownEditor += gvAct_ShownEditor;
                gcAct.EditorKeyUp += gcAct_EditorKeyUp;
                
                TM.Start();   //Timer循环启动

                tmR = new System.Timers.Timer() { Interval = 100, AutoReset = false }; 
                tmR.Elapsed += tmR_Elapsed;
                tmR.Start();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



        #region   多行输入的方法

        //动作组表
        void gcTeam_EditorKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && (gvTeam.ActiveEditor is DevExpress.XtraEditors.MemoEdit))
            {
                gvTeam.CloseEditor();
                gvTeam.RefreshData();
                gvTeam.ShowEditor();
            }
        }

        void gvTeam_ShownEditor(object sender, EventArgs e)
        {
            if (gvTeam.ActiveEditor is DevExpress.XtraEditors.MemoEdit)
            {
                DevExpress.XtraEditors.MemoEdit me = gvTeam.ActiveEditor as DevExpress.XtraEditors.MemoEdit;
                try
                {
                    me.SelectionStart = me.Text.Length;
                }
                catch
                {
                }
            }
        }

        //动作子表
        void gcAct_EditorKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && (gvAct.ActiveEditor is DevExpress.XtraEditors.MemoEdit))
            {
                gvAct.CloseEditor();
                gvAct.RefreshData();
                gvAct.ShowEditor();
            }
        }

        void gvAct_ShownEditor(object sender, EventArgs e)
        {

            if (gvAct.ActiveEditor is DevExpress.XtraEditors.MemoEdit)
            {
                DevExpress.XtraEditors.MemoEdit me = gvAct.ActiveEditor as DevExpress.XtraEditors.MemoEdit;
                try
                {
                    me.SelectionStart = me.Text.Length;
                }
                catch
                {
                }
            };
        }


        #endregion
   
        #endregion

        #region   数据的加载

        //分解动作子表，保存R指令的表
        private void fun_load分解动作子表()
        {
            string sql = "select * from ABB分解动作子表 where 1<>1";
            dt_分解动作子表 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
        }
       
        //加载动作分解表
        private void fun_load动作分解()
        {
            string sql = "select * from 动作分解表";
            dt_动作分解 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
        }

        //把动作子表的所有数据加载进来
        private void fun_loadall子表()
        {
            string sql = "select * from ABB检测组动作子表 order by 检测ID,动作POS";
            dt_all子表 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
        }



        /// <summary>
        /// 重要函数 AQUA注
        /// 动作子表的重新组合  做为参数 传递的
        /// </summary>
        private void fun_动作子表重组()
        {
            dt_new动作子表 = new DataTable();
            dt_new动作子表 = dt_all子表.Clone();
            dt_new动作子表.Columns["动作POS"].ColumnName = "主动作POS";  //修改列名
            dt_new动作子表.Columns.Add("动作POS");
            dt_new动作子表.Columns.Add("主动作ID");
            int i = 1;
            string strjcid = "";
            foreach (DataRow r in dt_all子表.Rows)
            {
                if (strjcid!=r["检测ID"].ToString())
                {
                    strjcid = r["检测ID"].ToString();
                    i = 1;
                }

                DataRow[] drfj = dt_动作分解.Select(string.Format("主动作ID='{0}'", r["动作ID"].ToString()));
                if (drfj.Length > 0)
                {
                    foreach (DataRow r1 in drfj)
                    {
                        DataRow[] drdz = dt_动作表.Select(string.Format("动作ID='{0}'", r1["子动作ID"].ToString()));
                        if (drdz.Length > 0)
                        {
                            DataRow r2 = dt_new动作子表.NewRow();
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
                                r2["P1"] = r1["P1"];r2["P4"] = r1["P4"]; r2["P5"] = r1["P5"];
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
                                 r2["P1"] = r["P1"];r2["P4"] = r["P4"]; r2["P5"] = r["P5"];
                            }
                            r2["参数个数及说明"] = drdz[0]["动作参数个数"].ToString() + @" 个 " + drdz[0]["动作参数说明"].ToString();
                            r2["动作POS"] = i;
                            r2["主动作ID"] = r["动作ID"];
                            dt_new动作子表.Rows.Add(r2);
                            i++;
                        }
                    }
                }
                else
                {
                    DataRow r2 = dt_new动作子表.NewRow();
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
                    dt_new动作子表.Rows.Add(r2);
                    i++;
                }
            }
        }

        #region  注释 动作组表  动作子表
        ////加载动作组表 全部加载
        //private void fun_load动作组表()
        //{
        //    string sql = "select * from ABB检测类型动作组表 order by 检测组POS";
        //    dt_动作组表 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
        //    dt_动作组表.Columns.Add("状态");
        //    dt_动作组表.Columns.Add("工作时间");   //记录组动作的时间
        //    foreach (DataRow r in dt_动作组表.Rows)
        //    {
        //        r["状态"] = "待检";
        //    }
        //    dv = new DataView(dt_动作组表);
        //    fun_dvnewrow(); //增加一个所有行
        //}


        ////动作组子表
        //private void fun_load动作组子表()
        //{
        //    string sql = "select * from ABB检测组动作子表 order by 动作POS";
        //    dt_动作子表 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
        //    dt_动作子表.Columns.Add("状态");
        //    dt_动作子表.Columns.Add("动作判定要求");
        //    dt_动作子表.Columns.Add("执行");
        //    dt_动作子表.Columns.Add("工作时间");   //记录主动作的时间
        //    foreach (DataRow r in dt_动作子表.Rows)
        //    {
        //        r["状态"] = "待检";
        //    }
        //}

        #endregion

        private void fun_load动作组表(string mc)
        {
            string sql =string.Format("select * from ABB检测类型动作组表 where 检测名称='{0}' order by 检测组POS",mc);
            dt_动作组表 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            dt_动作组表.Columns.Add("状态");
            dt_动作组表.Columns.Add("工作时间");   //记录组动作的时间
            foreach (DataRow r in dt_动作组表.Rows)
            {
                r["状态"] = "待检";
            }
            dv = new DataView(dt_动作组表);
            fun_dvnewrow(); //增加一个所有行
        }

        //为DV新增一行 动作组新增一行 可以查看所有的
        private void fun_dvnewrow()
        {
            DataRowView dr = dv.AddNew();   //新增一个虚拟行
            dr["检测ID"] = dv.Table.Rows[0]["检测ID"];
            dr["检测名称"] = dv.Table.Rows[0]["检测名称"];
            dr["检测组POS"] = 256;
            dr["设备要求"] = "所有";
            dr["检测要求"] = "所有";
            dr["检测内容"] = "所有";
            dr["状态"] = "所有";
            //dv.Table.Rows.Add(dr);
        }

        private void fun_load动作组子表(string id)
        {
            string sql = string.Format("select * from ABB检测组动作子表 where 检测ID='{0}' order by 动作POS", id);
            dt_动作子表 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            dt_动作子表.Columns.Add("状态");
            dt_动作子表.Columns.Add("动作判定要求");
            dt_动作子表.Columns.Add("执行");
            dt_动作子表.Columns.Add("工作时间");   //记录主动作的时间
            foreach (DataRow r in dt_动作子表.Rows)
            {
                r["状态"] = "待检";
            }
        }

        //加载所有的检测参数
        private void fun_load检测参数()
        {
            string sql = string.Format("select * from ABB检测类型主表");
            dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
        }

        //参数视图
        private void fun_加载检测参数()
        {
            string sql = string.Format("select * from ABB检测类型主表 where 检测名称='{0}'",jianceleibie.EditValue.ToString());
            dtM_fu = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            if (dtM_fu.Rows.Count > 0)
            {
                int j = 0;
                dic.Clear();
                foreach (DataColumn dc in dtM_fu.Columns)
                {
                    string key = dc.ColumnName;
                    string value = dtM_fu.Rows[0][j].ToString();
                    dic.Add(key, value);
                    j++;
                }
                DataTable dtMfu = new DataTable();
                dtMfu.Columns.Add("名称");
                dtMfu.Columns.Add("参数");
                foreach (var c in dic)
                {
                    dtMfu.Rows.Add(c.Key, c.Value);
                }
                gcPara.DataSource = dtMfu;
            }
        }

        //加载动作表
        private void fun_加载动作表()
        {
            string sql = "select * from ABB动作表";
            dt_动作表 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
        }

        //加载设备下来框的数据，也就是是机台名称,当前电脑所拥有的机台的名称
        private void fun_加载检测机台表()
        {
            string sql =string.Format("select * from 检测机台表 where 使用='1' and 工控机='{0}'",System.Environment.MachineName);    //在使用过程中的机台才能显示  使用的标志是1，要本台工控机能够使用的机台
            dt_检测机台表 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            foreach (DataRow r in dt_检测机台表.Rows)
            {
                ((DevExpress.XtraEditors.Repository.RepositoryItemComboBox)this.shebei.Edit).Items.Add(r["机台名称"].ToString());                 
            }
        }

        //检测类别的下来框，检测类别的下拉框是随着设备的不同，所能检测的类别也不同。检测类别就是ABB主表中的检测名称
        private void shebei_EditValueChanged(object sender, EventArgs e)  
        {
            try
            {
                jianceleibie.EditValue = "";
                ((DevExpress.XtraEditors.Repository.RepositoryItemComboBox)this.jianceleibie.Edit).Items.Clear(); //每次要进行清空
                DataRow[] dr = dt_检测机台表.Select(string.Format("机台名称='{0}'", shebei.EditValue));     //找到机台类型，即可以做的类型操作。这个只可能查出一条数据，机台类型就是检测大类
                if (dr.Length > 0)
                {
                    DataRow[] dr1 = dtM.Select(string.Format("检测大类='{0}'", dr[0]["机台类型"].ToString()));   //根据找出的机台类型去查找检测类别，检测类别也是ABB主表中的检测名称
                    foreach (DataRow t in dr1)
                    {
                        ((DevExpress.XtraEditors.Repository.RepositoryItemComboBox)this.jianceleibie.Edit).Items.Add(t["检测名称"].ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //在gridcontrol中显示动作组
        private void fun_组显示()
        {
            try
            {
                fun_load动作组表(jianceleibie.EditValue.ToString());
                //dv.RowFilter = string.Format("检测名称='{0}'", jianceleibie.EditValue.ToString());   //按照检测名称筛选
                dv.Sort = "检测组POS";
                gcTeam.DataSource = dv;
                //加载时候就能显示第一行
                foreach (DataRowView drv in dv)
                {
                    fun_显示(drv.Row);                  
                    break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //为了显示第一行的动作组子表
        private void fun_显示(DataRow r)
        {
            textBox1.Text = r["检测要求"].ToString();
            textBox2.Text = r["设备要求"].ToString();
            textBox3.Text = r["检测内容"].ToString();
            fun_load动作组子表(r["检测ID"].ToString());
            dv1 = new DataView(dt_动作子表);
            if (r["状态"].ToString() == "所有")   //显示所有的动作顺序
            {
                dv1.RowFilter = string.Format("检测ID='{0}' and 检测名称='{1}'", r["检测ID"].ToString(), r["检测名称"].ToString());
            }

            if (r["状态"].ToString() != "所有")
            {
                dv1.RowFilter = string.Format("检测ID='{0}' and 检测名称='{1}' and 检测组POS='{2}'", r["检测ID"].ToString(), r["检测名称"].ToString(), r["检测组POS"].ToString());
            }

            //给动作判定要求进行组合
            foreach (DataRowView drv in dv1)
            {
                DataRow[] drr = dt_动作表.Select(string.Format("动作ID='{0}'", drv.Row["动作ID"].ToString()));
                if (drv.Row["动作判定要求"].ToString() == "")
                {
                    if (Convert.ToInt32(drr[0]["动作参数个数"]) > 0)
                    {
                        for (int g = 1; g <= Convert.ToInt32(drr[0]["动作参数个数"]); g++)
                        {
                            drv.Row["动作判定要求"] += " " + drr[0]["P" + g + ""].ToString() + @":" + drv.Row["P" + g + ""].ToString();
                        }
                    }
                }

                #region  注释方案
                //switch (Convert.ToInt32(drr[0]["动作参数个数"].ToString()))
                //{
                //    case 0: break;
                //    case 1: drv.Row["动作判定要求"] = drr[0]["P1"].ToString() + @":" + drv.Row["P1"].ToString(); break;
                //    case 2: drv.Row["动作判定要求"] = drr[0]["P1"].ToString() + @":" + drv.Row["P1"].ToString() + @" || " + drr[0]["P2"].ToString() + @":" + drv.Row["P2"].ToString(); break;
                //    case 3: drv.Row["动作判定要求"] = drr[0]["P1"].ToString() + @":" + drv.Row["P1"].ToString() + @" || " + drr[0]["P2"].ToString() + @":" + drv.Row["P2"].ToString() + @" || " + drr[0]["P3"].ToString() + @":" + drv.Row["P3"].ToString(); break;
                //    case 4: drv.Row["动作判定要求"] = drr[0]["P1"].ToString() + @":" + drv.Row["P1"].ToString() + @" || " + drr[0]["P2"].ToString() + @":" + drv.Row["P2"].ToString() + @" || " + drr[0]["P3"].ToString() + @":" + drv.Row["P3"].ToString() + @" || " + drr[0]["P4"].ToString() + @":" + drv.Row["P4"].ToString(); break;
                //    case 5: drv.Row["动作判定要求"] = drr[0]["P1"].ToString() + @":" + drv.Row["P1"].ToString() + @" || " + drr[0]["P2"].ToString() + @":" + drv.Row["P2"].ToString() + @" || " + drr[0]["P3"].ToString() + @":" + drv.Row["P3"].ToString() + @" || " + drr[0]["P4"].ToString() + @":" + drv.Row["P4"].ToString() + @" || " + drr[0]["P5"].ToString() + @":" + drv.Row["P5"].ToString(); break;
                //}
                #endregion
            }
            dv1.Sort = "动作POS";
            gcAct.DataSource = dv1;
        }

        #endregion

        #region 连接设备

        //连接设备的方法
        private void fun_连接设备()
        {
            if (shebei.EditValue == null)
            {
                shebei.EditValue = "";
            }
            if (shebei.EditValue.ToString() == "")
                throw new Exception("请选择需要连接的设备");
            //判断所要进行连接的设备是不是已经注册了
            if (PLCC.MachineManager.CheckMachineExists(shebei.EditValue.ToString()) != null)  //如果被注册了,需要抛出错误
                throw new Exception("该设备已经被连接了，你需要重新选择设备！");
    
            //如果该设备没有被连接
            mad = new PLCC.MachineAdapter();
            foreach (DataRow r in dt_检测机台表.Rows)
            {
                if (r["机台名称"].ToString() == shebei.EditValue.ToString())
                {
                    mad.strCOM = r["COM"].ToString();    //com口
                    mad.strComPara = r["COM参数"].ToString();   //com口的参数
                    mad.strVCCOM = r["电压表COM"].ToString();   //电压表的COM
                    mad.strVCCOMPara = r["电压表COM参数"].ToString();   //电压表com参数
                    mad.strMachineName = r["机台名称"].ToString();   //机台的名称
                }
            }
            PLCC.MachineManager.addMachine(mad);  //把设备加进去

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
                    tmR.Close();
                    TM.Enabled = false;

                    this.Dispose();

                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + "     fun_断开设备");
            }
        }

        #endregion

       
        #region 窗体核心事件 TM的循环

        #region 界面数据获取刷新部分 灯的部分

        private void fun_亮灯函数(char[] Y)
        {
            #region   灯的部分


            //亮灯为1，灭灯为0
            //LINK/待机灯 绿色
            if (Y[16] == '1')
            {
                daji.BackColor = Color.DarkGreen;
            }
            if (Y[16] == '0')
            {
                daji.BackColor = Color.Gray;
            }
            //组队/急停  红色
            if (Y[17] == '1')
            {
                zudui.BackColor = Color.DarkRed;
            }
            if (Y[17] == '0')
            {
                zudui.BackColor = Color.Gray;
            }
            //断路器  白色
            if (Y[18] == '1')
            {
                duanluqi.BackColor = Color.Black;
            }
            if (Y[18] == '0')
            {
                duanluqi.BackColor = Color.Gray;
            }
            //电源  紫色
            if (Y[19] == '1')
            {
                dianyuan.BackColor = Color.Purple;
            }
            if (Y[19] == '0')
            {
                dianyuan.BackColor = Color.Gray;
            }
            //调试   黄色
            if (Y[20] == '1')
            {
                tiaoshi.BackColor = Color.Yellow;
            }
            if (Y[20] == '0')
            {
                tiaoshi.BackColor = Color.Gray;
            }


            #endregion

            #region  参数获取的部分

            liuzhuanka.EditValue = mad.strCPSN;
            

            //获取电压，工作时间，断路器开合次数
            LA_dianya.Text = mad.Vcc_电压.ToString();

            if (StartTime.ToString().Substring(0, 1) != "0")
            {
                TimeSpan times = DateTime.Now - StartTime;
                LA_gzsj.Text = times.ToString().Substring(3, 5); //检测已用的时间
            }
            LA_dlqkhcs.Text = mad.PLC_S_断路器开合次数.ToString();

            //当前检测项目  当前检测状态  当前检测动作  交直流  额定电压
            LA_dqxm.Text = strCurrJCLB;
            LA_dqzt.Text = mad.PLC_S_主状态.ToString();
            //LA_dqdz.Text = mad.PLC_S_PLC动作;
            if (mad.V交直流 == "1")
            {
                LA_jzl.Text ="交流"; //交流
            }
            else
            {
                LA_jzl.Text ="直流"; //直流
            }
            LA_erddy.Text = mad.V额定电压.ToString();

            //当前执行的子动作及其时间
            DataRow[] dr = dt_new子表.Select(string.Format("动作POS='{0}'", mad.PLC_CURR_POS.ToString()));
            if (dr.Length > 0)
            {
                DataRow[] dr1 = dt_动作表.Select(string.Format("动作ID='{0}'", dr[0]["动作ID"].ToString()));
                if (dr1.Length > 0)
                {
                    LA_dqdz.Text = dr[0]["动作说明"].ToString(); //当前的检测动作
                    LA_dqzxdz.Text = dr[0]["动作说明"].ToString();//当前执行动作
                    LA_dqdzzxsj.Text = mad.T_子动作时间.ToString();//当前动作执行时间
                    LA_dzzxsjck.Text = dr[0]["P1"].ToString();//当前执行动作时间参考
                }     
            }

            //开始检测的时间
            LA_starttime.Text = StartTime.ToString("HH:mm:ss"); //开始检测的时间
            if (StartTime.ToString().Substring(0, 1) != "0")
            {
                TimeSpan times = DateTime.Now - StartTime;
                LA_yytimes.Text = times.ToString().Substring(3, 5); ;   //检测已用的时间
            }
           
            if (LA_PASS.Text == "PASS")
            {
                LA_cwdz.Visible = false;
                LA_cwyy.Visible = false;
            }

            if (mad.ResultRS.Count > 0)  //必须要大于0的时候
            {
                PLCC.ResultR rr5 = mad.ResultRS[0];
          

                //错误原因，错误动作
                if (rr5.PLC_R_结果判定 == "2")    //根据结果的判定来取决于是否通过检测 1表示不通过 有错误
                {
                    LA_cwdz.Visible = true;
                    LA_cwyy.Visible = true;
                    DataRow[] drsm = dt_动作表.Select(string.Format("动作ID='{0}'", rr5.PLC_R_动作ID.ToString()));
                    if (drsm.Length > 0)
                    {
                        LA_cwdz.Text = "错误动作：" + rr5.PLC_R_动作ID.ToString() + "  " + drsm[0]["动作说明"].ToString();
                    }
                    LA_cwyy.Text = "错误原因：" + rr5.PLC_R_错误描述.ToString();
                }
                if (rr5.PLC_R_结果判定 == "")
                {
                    LA_PASS.Visible = false;
                }
                else
                {
                    if (rr5.PLC_R_结果判定 == "1")  //0表示检测通过
                    {
                        //表示检测通过了
                        LA_PASS.Visible = true;
                        LA_PASS.Text = "PASS";
                        LA_PASS.ForeColor = Color.Green;
                    }
                    else
                    {
                        if (rr5.PLC_R_结果判定 == "2")
                        {
                            LA_PASS.Visible = true;
                            LA_PASS.Text = "NG";
                            LA_PASS.ForeColor = Color.Red;
                        }
                    }
                }
            }

            #endregion

        }

        #endregion


        #region   gridcontrol满足条件进行颜色标记  在检 NG PASS三种状态颜色进行标记

        //gvTeam 动作组主表着色功能
        private void gvTeam_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (gvTeam.GetRow(e.RowHandle) == null)
                {
                    return;
                }
                int j = gvTeam.RowCount;

                for (int i = 0; i < j; i++)
                {
                    if (gvTeam.GetRowCellValue(e.RowHandle, "状态").ToString() == "在检")
                    {
                        e.Appearance.BackColor = Color.Blue;
                        e.Appearance.BackColor2 = Color.Blue;
                    }

                    if (gvTeam.GetRowCellValue(e.RowHandle, "状态").ToString() == "PASS")
                    {
                        e.Appearance.BackColor = Color.Green;
                        e.Appearance.BackColor2 = Color.Green;
                    }

                    if (gvTeam.GetRowCellValue(e.RowHandle, "状态").ToString() == "NG")
                    {
                        e.Appearance.BackColor = Color.Red;
                        e.Appearance.BackColor2 = Color.Red;
                    }

                    if (gvTeam.GetRowCellValue(e.RowHandle, "状态").ToString() == "放弃")
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

        //gvAct 动作组子表着色功能
        private void gvAct_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (gvAct.GetRow(e.RowHandle) == null)
                {
                    return;
                }
                int j = gvAct.RowCount;

                for (int i = 0; i < j; i++)
                {
                    if (gvAct.GetRowCellValue(e.RowHandle, "状态").ToString() == "在检")
                    {
                        e.Appearance.BackColor = Color.Blue;
                        e.Appearance.BackColor2 = Color.Blue;
                    }

                    if (gvAct.GetRowCellValue(e.RowHandle, "状态").ToString() == "PASS")
                    {
                        e.Appearance.BackColor = Color.Green;
                        e.Appearance.BackColor2 = Color.Green;
                    }

                    if (gvAct.GetRowCellValue(e.RowHandle, "状态").ToString() == "NG")
                    {
                        e.Appearance.BackColor = Color.Red;
                        e.Appearance.BackColor2 = Color.Red;
                    }

                    if (gvAct.GetRowCellValue(e.RowHandle, "状态").ToString() == "放弃")
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

        #endregion


        #region  gvTeam gvAct 的变化

        //gridcontrol的变化
        private void fun_参数状态获取()   //S指令的POS
        {
            int pos = mad.PLC_CURR_POS;
            //aqua : 如果当时动作ID为00或FE 那么这个函数跳出。   
            DataRow[] dr = dt_new子表.Select(string.Format("动作POS='{0}'", pos.ToString()));
            if (dr[0]["动作ID"].ToString() == "00" || dr[0]["动作ID"].ToString() == "FE") return;   //如果遇到动作ID 00，FE
            int x = 0;
            //动作子表
            dv1.RowFilter = string.Format("检测ID='{0}' and 检测名称='{1}' and 检测组POS='{2}'", dr[0]["检测ID"], dr[0]["检测名称"], dr[0]["检测组POS"]);
            if (dr.Length > 0)
            {
                x = Convert.ToInt32(dr[0]["主动作POS"]) - 1;
                if (x >= 1)
                {
                    dv1.Table.Rows[x-1]["状态"] = "PASS";   //在检之前的都为PASS
                }
                dv1.Table.Rows[x]["状态"] = "在检";
                //if (Convert.ToInt32(mad.T_主动作时间.TotalSeconds) != 0)
                //{
                //    dv1.Table.Rows[x]["工作时间"] = mad.T_主动作时间.TotalSeconds.ToString();
                //}
                dv1.Table.Rows[x]["执行"] = dr[0]["动作说明"];
                if ((Convert.ToInt32(dr[0]["检测组POS"]) - 1) >= 1)
                {
                    dv.Table.Rows[Convert.ToInt32(dr[0]["检测组POS"]) - 2]["状态"] = "PASS";
                }
                dv.Table.Rows[Convert.ToInt32(dr[0]["检测组POS"]) - 1]["状态"] = "在检";
                if (Convert.ToInt32(mad.T_组动作时间.TotalSeconds) != 0)
                {
                    dv.Table.Rows[Convert.ToInt32(dr[0]["检测组POS"]) - 1]["工作时间"] = mad.T_组动作时间.TotalSeconds.ToString();
                }
                gvTeam.FocusedRowHandle = Convert.ToInt32(dr[0]["检测组POS"]) - 1;   //gridcontrol需要跟着滚动
                gvAct.FocusedRowHandle = Convert.ToInt32(dr[0]["检测组内POS"]) - 1;
            } 
        }

        private void fun_R指令变化(string rrpos,string rresult,PLCC.ResultR rr)
        {
            try
            {
                int Rpos = Convert.ToInt32(rrpos);  //R指令的POS

                DataRow[] dr = dt_new子表.Select(string.Format("动作POS='{0}'", Rpos.ToString()));
                if (dr[0]["动作ID"].ToString() == "00" || dr[0]["动作ID"].ToString() == "FE") return;   //如果遇到动作ID 00，FE
                int x = 0;
                dv1.RowFilter = string.Format("检测ID='{0}' and 检测名称='{1}' and 检测组POS='{2}'", dr[0]["检测ID"], dr[0]["检测名称"], dr[0]["检测组POS"]);
                if (dr.Length > 0)
                {
                    x = Convert.ToInt32(dr[0]["主动作POS"]) - 1;
                    //错误原因，错误动作
                    if (rresult == "2")    //根据结果的判定来取决于是否通过检测 1表示不通过 有错误
                    {
                        dv1.Table.Rows[x]["状态"] = "NG";   //如果检测不通过状态位标记为NG
                        dv1.Table.Rows[x]["工作时间"] = Convert.ToDouble(rr.PLC_R_R1) / 1000;
                        dv.Table.Rows[Convert.ToInt32(dv1.Table.Rows[x]["检测组POS"]) - 1]["状态"] = "NG";
                    }
                    DataRow[] dt = dt_new子表.Select(string.Format("动作POS='{0}'", (Rpos + 1).ToString()));
                    if (dt[0]["主动作POS"] != dr[0]["主动作POS"])
                    {
                        if (rresult == "1")  //0表示检测通过
                        {
                            //表示检测通过了
                            dv1.Table.Rows[x]["状态"] = "PASS";   //如果检测不通过状态位标记为NG
                            dv1.Table.Rows[x]["工作时间"] = Convert.ToDouble(rr.PLC_R_R1) / 1000;
                            if (dt[0]["检测组POS"] != dr[0]["检测组POS"])  //检测组要保证执行到最后一步动作
                            {
                                dv.Table.Rows[Convert.ToInt32(dv1.Table.Rows[x]["检测组POS"]) - 1]["状态"] = "PASS";
                            }
                        }
                    }
                    if (rresult == "FF")
                    {
                        dv1.Table.Rows[x]["状态"] = "放弃";   //如果检测不通过状态位标记为NG
                        dv.Table.Rows[Convert.ToInt32(dv1.Table.Rows[x]["检测组POS"]) - 1]["状态"] = "放弃";
                    }

                }
            }
            catch(Exception ex)

            {
                CZMaster.MasterLog.WriteLog(mad.strMachineName + "  fun_R指令变化 " + ex.Message );
            }

        }

        #endregion

        


        private void TM_Tick(object sender, EventArgs e)
        {
            TM.Enabled = false;

            try
            {
                chanxian.Enabled = false;
                liuzhuanka.Enabled = false;
                if (mad == null) return;  //设备还没有进行注册

                fun_得到MAD后初始化检测类型();   //获取设备中是否带有检测类别

                //设置界面指示灯，及其它状态。
                string strY = mad.PLC_S_PLC_Y;
                char[] Y = strY.ToCharArray();  //字符串转换成字符数组

                try
                {
                    fun_亮灯函数(Y);
                }
                catch(Exception ex)
                {
                    CZMaster.MasterLog.WriteLog(mad.strMachineName + "    " + ex.Message, "fun_亮灯函数");
                }
                

                // 如果mad在非工作状态和工作状态，那么gcTeam 和 gcACt 的处理方式不一样
                if (mad.blwork == false)
                {
                    jianceleibie.Enabled = true;  //锁定检测类别的框
                    barLargeButtonItem2.Enabled = true;//锁定更换按钮

                    gongzuo.BackColor = Color.Gray; //工作灯 
                    

                    EndTime = DateTime.Now.ToString();

                    //DataTable dt_count = MasterSQL.Get_DataTable(string.Format("select count(*) from ABB检测结果总表 where 机台名称='{0}'", shebei.EditValue.ToString()), CPublic.Var.geConn("PLC"));
                    //if (dt_count.Rows.Count > 0)
                    //{
                    //    LA_gongjiancount.Text = dt_count.Rows[0][0].ToString() + "件"; //spos
                    //}

                    //string testtime = DateTime.Now.ToString().Substring(0, 9);

                    //DataTable dt_c = MasterSQL.Get_DataTable(string.Format("select count(*) from ABB检测结果总表 where 机台名称='{0}' and 开始检测时间 like '{1}%' ", shebei.EditValue.ToString(),testtime), CPublic.Var.geConn("PLC"));
                    //if (dt_c.Rows.Count > 0)
                    //{

                    //    LA_jinricount.Text = dt_c.Rows[0][0].ToString() + "件";
                    //}

                    LA_starttime.Text = "";
                    LA_yytimes.Text = "";
                    LA_gzsj.Text= "00:00";
                    
                }

                //如果是工作状态。那么gcTeam 和 gcACt 的显示完全由mad的节奏决定 ，主要由
                if (mad.blwork == true)  //如果该设备在工作的状态下
                {
                   // blwork_OLD = mad.blwork; //标志位

                    if(blwork_OLD != mad.blwork)  barLargeButtonItem2_ItemClick(null, null);


                    jianceleibie.Enabled = false;  //锁定检测类别的框
                    barLargeButtonItem2.Enabled = false;//锁定更换按钮

                    if (blwork_OLD == false)
                    {
                        blwork_OLD = mad.blwork;
                        strCx = chanxian.EditValue.ToString();  //产线
                        //strCpSN = liuzhuanka.EditValue.ToString();   //流转卡
                        StartTime = DateTime.Now;  //开始检测时间
                        jtmc = shebei.EditValue.ToString();
                    }
                    //工作灯就要亮起来
                    gongzuo.BackColor = Color.DarkBlue; //蓝色

                    ///aqua : 用S指令来刷新界面 ，需要一个OLD_S 辅助。
                    if (mad.PLC_CURR_POS.ToString() != S_oldpos)
                    {
                        try
                        {
                            fun_参数状态获取();
                        }
                        catch(Exception ex)
                        {
                            CZMaster.MasterLog.WriteLog(mad.strMachineName + "    " + ex.Message, "fun_参数状态获取");
                        }
                        S_oldpos = mad.PLC_CURR_POS.ToString();

                    }
                }

                blwork_OLD = mad.blwork; //标志位
            }
            catch(Exception ex)
            {
                CZMaster.MasterLog.WriteLog(mad.strMachineName + "    " + ex.Message, "TM_Tick");
            }
            finally
            {
                if (blClose == false) TM.Enabled = true;
            }
        }

        #region  数据保存的数据检查

        private void check_结果子表(PLCC.ResultR rr)
        {
            try
            {
                DataRow[] t = dt_new子表.Select(string.Format("动作POS='{0}'", rr.PLC_R_动作POS));
                DataRow[] drr2 = dt_动作子表.Select(string.Format("动作POS='{0}'", t[0]["主动作POS"]));
                if (drr2.Length > 0)
                {
                    for (int cs = 1; cs <= 10; cs++)
                    {
                        if (drr2[0]["P" + cs + ""].ToString() != "")
                        {
                            try
                            {
                                Decimal d = Convert.ToDecimal(drr2[0]["P" + cs + ""]);
                            }
                            catch
                            {
                                throw new Exception("");
                            }

                        }
                    }
                }

            }
            catch
            {

            }


            //r["VR1"] = rr.PLC_R_R1.ToString(); r["VR2"] = rr3.PLC_R_R2.ToString(); r["VR3"] = rr3.PLC_R_R3.ToString(); r["VR4"] = rr3.PLC_R_R4.ToString(); r["VR5"] = rr3.PLC_R_R5.ToString();
          //  r["VR6"] = rr.PLC_R_R6.ToString(); r["VR7"] = rr3.PLC_R_R7.ToString(); r["VR8"] = rr3.PLC_R_R8.ToString(); r["VR9"] = rr3.PLC_R_R9.ToString(); r["VR10"] = rr3.PLC_R_R10.ToString();

        }

        private void check_不合格产品表()
        {


        }


        #endregion

        #region 结果总

        private void fun_load结果总(string zongGUID)
        {
            string sql = string.Format("select * from ABB检测结果总表 where 检测总GUID='{0}'", zongGUID);
            dt_结果总表 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
        }
        
        private void fun_删除总动作()
        {
            fun_load结果总(str总GUID);
            foreach (DataRow r in dt_结果总表.Rows)
            {
                r.Delete();
            }
            //dt_结果总表.Rows[0].Delete();
            MasterSQL.Save_DataTable(dt_结果总表, "ABB检测结果总表", CPublic.Var.geConn("PLC"));
            fun_load结果总(str总GUID);
        }

        private void fun_保存总动作(PLCC.ResultR rr)
        {
            try
            {
                if (rr.PLC_R_动作ID == "00" ) return;

                //if (rr.PLC_R_动作ID == "FE")
                //{
                //        //mad.PLC_CURR_POS = -1;
                //        mad.iDZJP = -1;

                //        return;
                //}
                //查看str总GUID是否存在，如果不存在，新增，如果存在， 查看R是否不是机构动作，如果不是。 删除后新增
                if (str总GUID == "")
                {
                    fun_load结果总(str总GUID);
                    str总GUID = System.Guid.NewGuid().ToString();
                    fun_save保存总动作(rr, str总GUID);
                }
                else
                {
                    //R 是否不是机构动作
                    DataRow[] dr = dt_动作表.Select(string.Format("动作ID='{0}'", rr.PLC_R_动作ID.ToString()));
                    if (dr.Length > 0)
                    {
                        //如果是，跳出
                        if (dr[0]["动作执行类型"].ToString() == "机构动作")
                        {
                            return;
                        }
                        else
                        {
                            //如果不是，执行如下动作
                            fun_删除总动作();
                            fun_save保存总动作(rr, str总GUID);
                        }
                    }
                    //FE是结尾标记
                    if (rr.PLC_R_动作ID.ToString() == "FE")
                    {
                        fun_删除总动作();
                        fun_save保存总动作(rr, str总GUID);
                    }
                }

            }
            catch
            {



            }

        }

        private void fun_save保存总动作(PLCC.ResultR R, string GUID)
        {
            //按R，和GUID新增总动作
            DataRow r = dt_结果总表.NewRow();
            r["检测总GUID"] = GUID;
            r["产品名称"] = "";
            r["产品SN号"] = mad.strCPSN;
            r["产品类型"] = "";
            r["产品产线"] = strCx;
            r["检测ID"] = strCurrJCLBID; //检测ID
            r["检测标准"] = strCurrJCLB;
            r["开始检测时间"] = StartTime; //开始检测的时间
            r["机台名称"] = jtmc;
            r["工作台名称"] = System.Environment.MachineName;
            r["操作员"] = CPublic.Var.localUserName;  //操作员

            if (R.PLC_R_结果判定 == "FE")  //检测结果PASS
            {
                r["检测是否通过"] = "PASS";
                try
                {
                    r["结束检测时间"] = DateTime.Now.ToString();
                    TimeSpan times = DateTime.Now - StartTime;
                    r["检测总时间"] = times.TotalSeconds.ToString();   //检测的总时间
                }
                catch
                {

                }
            }
            else
            {
                r["检测是否通过"] = "未知";
                try
                {
                    r["结束检测时间"] = DateTime.Now.ToString();
                    TimeSpan times = DateTime.Now - StartTime;
                    r["检测总时间"] = times.TotalSeconds.ToString();   //检测的总时间
                }
                catch
                {

                }
            }



            if (R.PLC_R_结果判定 == "2")   //检测结果NG
            {
                r["检测是否通过"] = "NG";
                try
                {
                    r["结束检测时间"] = DateTime.Now.ToString();
                    TimeSpan times = DateTime.Now - StartTime;
                    r["检测总时间"] = times.TotalSeconds.ToString();   //检测的总时间
                }
                catch
                {

                }
                DataRow[] t = dt_new子表.Select(string.Format("动作POS='{0}'", R.PLC_R_动作POS.ToString()));
                if (t.Length > 0)
                {
                    r["出错检测组POS"] = t[0]["检测组POS"];   //检测组POS
                    DataRow[] t1 = dt_动作组表.Select(string.Format("检测组POS='{0}'", t[0]["检测组POS"].ToString()));
                    if (t1.Length > 0)
                    {
                        r["出错检测要求"] = t1[0]["检测要求"];  //检测要求
                    }
                    r["出错主动作POS"] = t[0]["主动作POS"];  //出错主动作POS
                    r["出错主动作ID"] = t[0]["主动作ID"];
                }
                r["出错动作POS"] = R.PLC_R_动作POS;   //出错动作的POS
                //出错主动作ID
               
                //DataRow[] t2 = dt_动作分解.Select(string.Format("子动作ID='{0}'", R.PLC_R_动作ID.ToString()));
                //if (t2.Length > 0)
                //{
                //    r["出错主动作ID"] = t2[0]["主动作ID"];
                //}
                //else
                //{
                //    r["出错主动作ID"] = R.PLC_R_动作ID;
                //}
                //出错主动作说明
                DataRow[] t3 = dt_动作表.Select(string.Format("动作ID='{0}'", r["出错主动作ID"].ToString()));
                if (t3.Length > 0)
                {
                    r["出错主动作说明"] = t3[0]["动作说明"];
                }
                //出错动作ID
                r["出错动作ID"] = R.PLC_R_动作ID;
                DataRow[] t4 = dt_动作表.Select(string.Format("动作ID='{0}'", R.PLC_R_动作ID.ToString()));
                if (t4.Length > 0)
                {
                    r["出错动作说明"] = t[0]["动作说明"];
                }
                r["出错代码"] = R.PLC_R_错误代码;
                r["出错原因"] = R.PLC_R_错误描述;
            }
            //检测结果是放弃
            if (R.PLC_R_结果判定 == "FF")
            {
                try
                {
                    r["结束检测时间"] = DateTime.Now.ToString();
                    TimeSpan times = DateTime.Now - StartTime;
                    r["检测总时间"] = times.TotalSeconds.ToString();   //检测的总时间
                }
                catch
                {

                }
                r["检测是否通过"] = "放弃";
            }

            dt_结果总表.Rows.Add(r);
            MasterSQL.Save_DataTable(dt_结果总表, "ABB检测结果总表", CPublic.Var.geConn("PLC"));
            fun_load结果总(GUID);
        }

        #endregion

        #region   结果组保存

        private void fun_load结果组(string 组GUID)
        {
            string sql = string.Format("select * from ABB检测结果组表 where 检测组GUID='{0}'", 组GUID);
            dt_结果组表 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
        }

        private void fun_删除组动作(string str组GUID)
        {
            fun_load结果组(str组GUID);
            foreach (DataRow r in dt_结果组表.Rows)
            {
                r.Delete();
            }
            MasterSQL.Save_DataTable(dt_结果组表, "ABB检测结果组表", CPublic.Var.geConn("PLC"));
            fun_load结果组(str组GUID);
        }

        private void fun_保存组动作(PLCC.ResultR rr)
        {
            if (rr.PLC_R_动作ID == "00" || rr.PLC_R_动作ID=="FE") return;

            int i_组POS = 0;
            //通过 R 得到 i_组POS
            DataRow[] dr = dt_new子表.Select(string.Format("动作POS='{0}'", rr.PLC_R_动作POS.ToString()));
            if (dr.Length > 0)
            {
                i_组POS = Convert.ToInt32(dr[0]["检测组POS"]);
            }

            if (li_组GUID.ContainsKey(i_组POS) == false)
            {
                li_组GUID.Add(i_组POS, System.Guid.NewGuid().ToString());
                fun_save保存组动作(rr, str总GUID, li_组GUID[i_组POS]);
            }
            else
            {
                //R 是否不是机构动作
                DataRow[] dr1 = dt_动作表.Select(string.Format("动作ID='{0}'", rr.PLC_R_动作ID.ToString()));
                if (dr1.Length > 0)
                {
                    //如果是，跳出
                    if (dr1[0]["动作执行类型"].ToString() == "机构动作")
                    {
                        return;
                    }
                    else
                    {
                        //R 是否不是机构动作
                        //如果是，跳出
                        //如果不是，执行如下动作
                        fun_删除组动作(li_组GUID[i_组POS]);
                        fun_save保存组动作(rr, str总GUID, li_组GUID[i_组POS]);
                    }
                }
            }
        }

        private void fun_save保存组动作(PLCC.ResultR rr, string GUID总, string GUID组)
        {
                fun_load结果组(GUID组);
                DataRow r =dt_结果组表.NewRow();
                r["检测总GUID"] = GUID总;
                r["检测组GUID"] = GUID组;
                r["产品名称"] = "";
                r["产品SN号"] = mad.strCPSN;
                r["产品类型"] = "";
                r["产品产线"] =strCx;
                r["检测ID"] = strCurrJCLBID; //检测ID
                r["检测标准"] = strCurrJCLB;
                DataRow[] dr = dt_new子表.Select(string.Format("动作POS='{0}'", rr.PLC_R_动作POS.ToString()));
                if (dr.Length > 0)
                {
                    r["检测组POS"] = dr[0]["检测组POS"];
                    DataRow[] dr1 = dt_动作组表.Select(string.Format("检测组POS='{0}'", r["检测组POS"].ToString()));
                    if (dr1.Length > 0)
                    {
                        r["检测要求"] = dr1[0]["检测要求"];
                        r["设备要求"] = dr1[0]["设备要求"];
                        r["检测内容"] = dr1[0]["检测内容"];
                    }
                }
                r["机台名称"] = jtmc;
                r["工作台名称"] = System.Environment.MachineName;
                r["检测时间"] =mad.T_组动作时间.TotalMilliseconds.ToString();
                r["操作员"] = CPublic.Var.localUserName;

                if (rr.PLC_R_结果判定 == "1")
                {
                    r["是否通过"] = "PASS";
                }
                //出现NG的结果
                if (rr.PLC_R_结果判定 == "2")
                {
                      r["是否通过"] = "NG";  //检测的结果
                        DataRow[] t = dt_new子表.Select(string.Format("动作POS='{0}'", rr.PLC_R_动作POS.ToString()));
                        if (t.Length > 0)
                        {
                            r["出错主动作POS"] = t[0]["主动作POS"];  //出错主动作POS
                            r["出错主动作ID"] = t[0]["主动作ID"];
                        }
                        r["出错动作POS"] = rr.PLC_R_动作POS; //出错动作的POS    
                        //DataRow[] t2 = dt_动作分解.Select(string.Format("子动作ID='{0}'", rr.PLC_R_动作ID.ToString()));
                        //if (t2.Length > 0)
                        //{
                        //   r["出错主动作ID"] = t2[0]["主动作ID"];
                        //}
                        //else
                        //{
                        //  r["出错主动作ID"] = rr.PLC_R_动作ID;
                        //}
                        //出错主动作说明
                        DataRow[] t3 = dt_动作表.Select(string.Format("动作ID='{0}'",r["出错主动作ID"].ToString()));
                        if (t3.Length > 0)
                        {
                            r["出错主动作说明"] = t3[0]["动作说明"];
                        }
                        //出错动作ID
                        r["出错动作ID"] = rr.PLC_R_动作ID;
                        DataRow[] t4 = dt_动作表.Select(string.Format("动作ID='{0}'", rr.PLC_R_动作ID.ToString()));
                        if (t4.Length > 0)
                        {
                           r["出错动作说明"] = t[0]["动作说明"];
                        }
                        r["出错代码"] = rr.PLC_R_错误代码;
                        r["出错原因"] = rr.PLC_R_错误描述;
                }

                //出现放弃的结果
                if (rr.PLC_R_结果判定 == "FF")
                {
                        r["是否通过"] = "放弃";
                }

                dt_结果组表.Rows.Add(r);
                MasterSQL.Save_DataTable(dt_结果组表, "ABB检测结果组表", CPublic.Var.geConn("PLC"));
                fun_load结果组(GUID组);
        }

        #endregion

        #region   保存结果主表

        private void fun_load结果主(string 主GUID)
        {
            string sql = string.Format("select * from ABB检测结果主动作表 where 检测主GUID='{0}'", 主GUID);
            dt_结果主动作 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
        }

        private void fun_删除主动作(string str主GUID)
        {
            fun_load结果主(str主GUID);
            foreach (DataRow r in dt_结果主动作.Rows)
            {
                r.Delete();
            }
            MasterSQL.Save_DataTable(dt_结果主动作, "ABB检测结果主动作表", CPublic.Var.geConn("PLC"));
            fun_load结果主(str主GUID);
        }

        private void fun_保存主动作(PLCC.ResultR rr)
        {
            if (rr.PLC_R_动作ID == "00" || rr.PLC_R_动作ID=="FE") return;
            int i_主POS = 0;
            //通过 R 得到 i_组POS
            DataRow[] dr = dt_new子表.Select(string.Format("动作POS='{0}'", rr.PLC_R_动作POS.ToString()));
            if (dr.Length > 0)
            {
                i_主POS = Convert.ToInt32(dr[0]["主动作POS"]);
            }

            if (li_主GUID.ContainsKey(i_主POS) == false)
            {
                //fun_load结果主(li_主GUID[i_主POS]);
                li_主GUID.Add(i_主POS, System.Guid.NewGuid().ToString());
                fun_save保存主动作(rr, str总GUID, li_组GUID[Convert.ToInt32(dr[0]["检测组POS"])], li_主GUID[i_主POS]);
            }
            else
            {
                //R 是否不是机构动作
                DataRow[] dr1 = dt_动作表.Select(string.Format("动作ID='{0}'", rr.PLC_R_动作ID.ToString()));
                if (dr1.Length > 0)
                {
                    //如果是，跳出
                    if (dr1[0]["动作执行类型"].ToString() == "机构动作")
                    {
                        return;
                    }
                    else
                    {
                        fun_删除主动作(li_主GUID[i_主POS]);
                        fun_save保存主动作(rr, str总GUID, li_组GUID[Convert.ToInt32(dr[0]["检测组POS"])], li_主GUID[i_主POS]);
                    }
                }
            }
        }

        private void fun_save保存主动作(PLCC.ResultR rr, string GUID总, string GUID组, string GUID主)
        {
            fun_load结果主(GUID主);
            DataRow r1 = dt_结果主动作.NewRow();
            r1["检测总GUID"] = GUID总;
            r1["检测组GUID"] = GUID组;
            r1["检测主GUID"] = GUID主;
            r1["产品名称"] = "";
            r1["产品SN号"] = mad.strCPSN;
            r1["产品类型"] = "";
            r1["产品产线"] = strCx;
            r1["检测ID"] = strCurrJCLBID;
            r1["检测标准"] = strCurrJCLB;
            DataRow[] dr = dt_new子表.Select(string.Format("动作POS='{0}'", rr.PLC_R_动作POS.ToString()));
            if (dr.Length > 0)
            {
                r1["检测组POS"] = dr[0]["检测组POS"];  //检测组POS
                DataRow[] dr1 = dt_动作组表.Select(string.Format("检测组POS='{0}'", r1["检测组POS"].ToString()));
                if (dr1.Length > 0)
                {
                    r1["检测要求"] = dr1[0]["检测要求"];   //检测要求
                }
                r1["主动作POS"] = dr[0]["主动作POS"];     //主动作POS
                r1["主动作ID"] = dr[0]["主动作ID"];
            }
           
            //DataRow[] dr2 = dt_动作分解.Select(string.Format("子动作ID='{0}'", rr.PLC_R_动作ID.ToString()));
            //if (dr2.Length > 0)
            //{
            //    r1["主动作ID"] = dr2[0]["主动作ID"];
            //}
            //else
            //{
            //    r1["主动作ID"] = rr.PLC_R_动作ID;
            //}
            DataRow[] dr3 = dt_动作表.Select(string.Format("动作ID='{0}'", r1["主动作ID"].ToString()));
            if (dr3.Length > 0)
            {
                r1["主动作说明"] = dr3[0]["动作说明"];
            }
            r1["检测时间"] = mad.T_主动作时间.TotalMilliseconds.ToString();
            DataRow[] t = dt_new子表.Select(string.Format("动作POS='{0}'", rr.PLC_R_动作POS.ToString()));
            if (t.Length > 0)
            {
                r1["P1"] = t[0]["P1"]; r1["P2"] = t[0]["P2"]; r1["P3"] = t[0]["P3"]; r1["P4"] = t[0]["P4"]; r1["P5"] = t[0]["P5"];
                r1["P6"] = t[0]["P6"]; r1["P7"] = t[0]["P7"]; r1["P8"] = t[0]["P8"]; r1["P9"] = t[0]["P9"]; r1["P10"] = t[0]["P10"];
            }

            try
            {
                if (rr.PLC_R_动作ID == "41")
                {
                    r1["R1"] = Convert.ToInt32(rr.PLC_R_R1) - 2;
                }
                else
                {
                    r1["R1"] = rr.PLC_R_R1;
                }
            }
            catch
            {


            }
                     
            r1["R2"] = rr.PLC_R_R2; r1["R3"] = rr.PLC_R_R3; r1["R4"] = rr.PLC_R_R4; r1["R5"] = rr.PLC_R_R5;
            r1["R6"] = rr.PLC_R_R6; r1["R7"] = rr.PLC_R_R7; r1["R8"] = rr.PLC_R_R8; r1["R9"] = rr.PLC_R_R9; r1["R10"] = rr.PLC_R_R10;
            r1["机台名称"] = jtmc;
            r1["工作台名称"] = System.Environment.MachineName;
            r1["操作员"] = CPublic.Var.localUserName;  //当前的用户
            //检测结果是NG的时候
            if (rr.PLC_R_结果判定 == "2")
            {
                r1["检测是否通过"] = "NG";
                r1["出错动作POS"] = rr.PLC_R_动作POS;
                r1["出错动作ID"] = rr.PLC_R_动作ID;
                DataRow[] t0 = dt_动作表.Select(string.Format("动作ID='{0}'", rr.PLC_R_动作ID.ToString()));
                if (t0.Length > 0)
                {
                    r1["出错动作说明"] = t0[0]["动作说明"];
                }
                r1["出错代码"] = rr.PLC_R_错误代码;
                r1["出错原因"] = rr.PLC_R_错误描述;
            }

            //1是通过
            if (rr.PLC_R_结果判定 == "1")
            {
                r1["检测是否通过"] = "PASS";
            }

            //FF表示是放弃了
            if (rr.PLC_R_结果判定 == "FF")
            {
                r1["检测是否通过"] = "放弃";
            }
            dt_结果主动作.Rows.Add(r1);
            MasterSQL.Save_DataTable(dt_结果主动作, "ABB检测结果主动作表", CPublic.Var.geConn("PLC"));
        }

        #endregion

        #region 保存动作

        private void fun_load检测结果动作表()
        {
            string sql = "select * from ABB检测结果动作表 where 1<>1";
            dt_结果动作 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
        }

        private void fun_保存检测结果动作表(PLCC.ResultR rr)
        {
            if (rr.PLC_R_动作ID == "00" || rr.PLC_R_动作ID=="FE") return;
            fun_load检测结果动作表();
            DataRow[] dr = dt_new子表.Select(string.Format("动作POS='{0}'", rr.PLC_R_动作POS.ToString()));
            DataRow r = dt_结果动作.NewRow();
            r["检测总GUID"] = str总GUID;
            r["检测组GUID"] = li_组GUID[Convert.ToInt32(dr[0]["检测组POS"])];
            r["检测主GUID"] = li_主GUID[Convert.ToInt32(dr[0]["主动作POS"])];
            r["检测动作GUID"] = System.Guid.NewGuid().ToString();
            r["产品名称"] = "";
            r["产品SN号"] = mad.strCPSN;
            r["产品类型"] = "";
            r["产品产线"] = strCx;
            r["检测ID"] = strCurrJCLBID;
            r["检测标准"] = strCurrJCLB;
            if (dr.Length > 0)
            {
                r["检测组POS"] = dr[0]["检测组POS"];
                DataRow[] dr1 = dt_动作组表.Select(string.Format("检测组POS='{0}'", r["检测组POS"].ToString()));
                if (dr1.Length > 0)
                {
                    r["检测要求"] = dr1[0]["检测要求"];
                }
                r["主动作POS"] = dr[0]["主动作POS"];
                r["主动作ID"] = dr[0]["主动作ID"];
            }
            r["动作POS"] = rr.PLC_R_动作POS;
            //DataRow[] dr2 = dt_动作分解.Select(string.Format("子动作ID='{0}'", rr.PLC_R_动作ID.ToString()));
            //if (dr2.Length > 0)
            //{
            //    r["主动作ID"] = dr2[0]["主动作ID"];
            //}
            //else
            //{
            //    r["主动作ID"] = rr.PLC_R_动作ID;
            //}
            DataRow[] dr3 = dt_动作表.Select(string.Format("动作ID='{0}'", r["主动作ID"].ToString()));
            if (dr3.Length > 0)
            {
                r["主动作说明"] = dr3[0]["动作说明"];
            }
            r["动作ID"] = rr.PLC_R_动作ID;
            DataRow[] dr4 = dt_动作表.Select(string.Format("动作ID='{0}'", r["动作ID"].ToString()));
            if (dr4.Length > 0)
            {
                r["动作说明"] = dr4[0]["动作说明"];
            }
            DataRow[] t = dt_new子表.Select(string.Format("动作POS='{0}'", rr.PLC_R_动作POS.ToString()));
            if (t.Length > 0)
            {
                r["报表节点"] = t[0]["报表节点"];  //增加一个报表节点
                r["P1"] = t[0]["P1"]; r["P2"] = t[0]["P2"]; r["P3"] = t[0]["P3"]; r["P4"] = t[0]["P4"]; r["P5"] = t[0]["P5"];
                r["P6"] = t[0]["P6"]; r["P7"] = t[0]["P7"]; r["P8"] = t[0]["P8"]; r["P9"] = t[0]["P9"]; r["P10"] = t[0]["P10"];
            }

            try
            {
                if (rr.PLC_R_动作ID == "41")
                {
                    r["R1"] = Convert.ToInt32(rr.PLC_R_R1) - 2;
                }
                else
                {
                    r["R1"] = rr.PLC_R_R1;
                }
            }
            catch
            {


            }

            r["R2"] = rr.PLC_R_R2; r["R3"] = rr.PLC_R_R3; r["R4"] = rr.PLC_R_R4; r["R5"] = rr.PLC_R_R5;
            r["R6"] = rr.PLC_R_R6; r["R7"] = rr.PLC_R_R7; r["R8"] = rr.PLC_R_R8; r["R9"] = rr.PLC_R_R9; r["R10"] = rr.PLC_R_R10;
            r["检测时间"] = mad.T_子动作时间.TotalMilliseconds.ToString();    //子动作的工作时间
            //检测NG
            if (rr.PLC_R_结果判定 == "2")
            {
                r["检测是否通过"] = "NG";
                r["出错代码"] = rr.PLC_R_错误代码;
                r["出错原因"] = rr.PLC_R_错误描述;
            }
            //检测通过
            if (rr.PLC_R_结果判定 == "1")
            {
                r["检测是否通过"] = "PASS";
            }
            //检测放弃
            if (rr.PLC_R_结果判定 == "FF")
            {
                r["检测是否通过"] = "放弃";
            }
            r["操作员"] = CPublic.Var.localUserName;  //当前的用户名
            r["机台名称"] = jtmc;
            r["工作台名称"] = System.Environment.MachineName;
            dt_结果动作.Rows.Add(r);
            MasterSQL.Save_DataTable(dt_结果动作, "ABB检测结果动作表", CPublic.Var.geConn("PLC"));
            fun_load检测结果动作表();
        }

        #endregion

        #region   不合格品的保存

        private void fun_load不合格品()
        {
            string sql = "select * from ABB不合格产品表 where 1<>1";
            dt_不合格产品表 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
        }

        private void fun_不合格品保存(PLCC.ResultR rr3)
        {
            DataRow[] t = dt_new子表.Select(string.Format("动作POS='{0}'", rr3.PLC_R_动作POS)); //为了找出主动作POS
            if (rr3.PLC_R_结果判定 == "2")   //2表示不合格
            {
                fun_load不合格品();
                DataRow r = dt_不合格产品表.NewRow();
                r["不合格GUID"] = System.Guid.NewGuid().ToString();
                r["工作台名称"] = System.Environment.MachineName;
                r["机台名称"] = jtmc;
                r["产品SN号"] = mad.strCPSN;
                r["产品名称"] = "";
                r["产品产线"] = strCx;
                r["检测大类"] = strCurrJCLB;
                r["检测组POS"] = t[0]["检测组POS"];
                DataRow[] jcyq = dt_动作组表.Select(string.Format("检测组POS='{0}'", t[0]["检测组POS"].ToString()));
                if (jcyq.Length > 0)
               {
                  r["检测要求"] = jcyq[0]["检测要求"];
                }
                r["出错动作POS"] = rr3.PLC_R_动作POS;
                if (t.Length > 0)
                {
                    r["出错主动作POS"] = t[0]["主动作POS"];
                    r["出错主动作ID"] = t[0]["主动作ID"];
                }
               // DataRow[] df = dt_动作分解.Select(string.Format("子动作ID='{0}'", rr3.PLC_R_动作ID.ToString()));  //找到主动作ID
               // if (df.Length > 0)
               //{
               //    r["出错主动作ID"] = df[0]["主动作ID"];
               // }
               //else
               //{
               //    r["出错主动作ID"] = rr3.PLC_R_动作ID;
               //}
                    //保存动作说明
                DataRow[] dzsm = dt_动作表.Select(string.Format("动作ID='{0}'", r["出错主动作ID"].ToString()));
               if (dzsm.Length > 0)
               {
                        r["出错主动作说明"] = dzsm[0]["动作说明"];
                }
               r["出错动作ID"] = rr3.PLC_R_动作ID;
               DataRow[] dzsm0 = dt_动作表.Select(string.Format("动作ID='{0}'", rr3.PLC_R_动作ID.ToString()));
               if (dzsm0.Length > 0)
               {
                   r["出错动作说明"] = dzsm0[0]["动作说明"];
               }

                    //保存动作的输入参数
               DataRow[] dzsm1 = dt_new子表.Select(string.Format("动作POS='{0}' and 动作ID='{1}'", t[0]["动作POS"], r["出错动作ID"]));
              if (dzsm1.Length > 0)
              {
                   r["P1"] = dzsm1[0]["P1"]; r["P2"] = dzsm1[0]["P2"]; r["P3"] = dzsm1[0]["P3"]; r["P4"] = dzsm1[0]["P4"]; r["P5"] = dzsm1[0]["P5"];
                    r["P6"] = dzsm1[0]["P6"]; r["P7"] = dzsm1[0]["P7"]; r["P8"] = dzsm1[0]["P8"]; r["P9"] = dzsm1[0]["P9"]; r["P10"] = dzsm1[0]["P10"];
               }
                    r["出错代码"] = rr3.PLC_R_错误代码.ToString();
                    r["出错描述"] = rr3.PLC_R_错误描述;
                    r["出错时间"] = DateTime.Now.ToString();
                    //保存动作的输出参数
                    try
                    {
                        if (rr3.PLC_R_动作ID == "41")
                        {
                            r["R1"] = Convert.ToInt32(rr3.PLC_R_R1) - 2;
                        }
                        else
                        {
                            r["R1"] = rr3.PLC_R_R1;
                        }
                    }
                    catch
                    {


                    }
                    r["R2"] = rr3.PLC_R_R2; r["R3"] = rr3.PLC_R_R3; r["R4"] = rr3.PLC_R_R4; r["R5"] = rr3.PLC_R_R5;
                    r["R6"] = rr3.PLC_R_R6; r["R7"] = rr3.PLC_R_R7; r["R8"] = rr3.PLC_R_R8; r["R9"] = rr3.PLC_R_R9; r["R10"] = rr3.PLC_R_R10;
                    dt_不合格产品表.Rows.Add(r);
                    MasterSQL.Save_DataTable(dt_不合格产品表, "ABB不合格产品表", CPublic.Var.geConn("PLC"));
                    fun_load不合格品();
                }   
        }


        #endregion


        int iGetSN = 0;
        
        void tmR_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (mad == null) return;
                jtmc = shebei.EditValue.ToString();
                iGetSN += (int)tmR.Interval;
                if (mad.blwork == false && iGetSN >= 1000)
                {
                    iGetSN = 0;
                    fun_SN();
                }

                if (mad.ResultRS.Count == 0) return;
                PLCC.ResultR rr = mad.ResultRS[0];

                if (oldpos_r != rr.PLC_R_动作POS)
                {
                    ///1.对RR的行为处理。并显示到gridcontrol上
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        fun_R指令变化(rr.PLC_R_动作POS, rr.PLC_R_结果判定,rr);
                    }));

                    oldpos_r = rr.PLC_R_动作POS;
                }


                //保存动作

                try
                {
                    fun_保存总动作(rr);
                    fun_保存组动作(rr);
                    fun_保存主动作(rr);
                    fun_保存检测结果动作表(rr);
                    fun_不合格品保存(rr);
                }
                catch (Exception ex)
                {
                    CZMaster.MasterLog.WriteLog(mad.strMachineName + "  fun_保存  :" +ex.Message);
                }                                 
                if (mad.blwork == false)
                {
                    //StartTime = new DateTime();
                    li_组GUID.Clear();
                    li_主GUID.Clear();
                    str总GUID = "";
                }

                lock (mad.ResultRS)
                {
                    mad.ResultRS.RemoveAt(0);
                }

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(mad.strMachineName + "  tmR_Elapsed  :" + ex.Message);
            }
            finally
            {
                if(blClose == false) tmR.Start();
            }
        }

        /// <summary>
        /// 处理SN号
        /// </summary>
        private void fun_SN()
        {
            string sql = string.Format("select * from 机台扫描SN结果表 where 机台名称='{0}'", mad.strMachineName);
            dt_SN = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            if (dt_SN.Rows.Count <= 0)
            {
                return;
            }
            if (dt_SN.Rows[0]["SN"].ToString() == "")
            {
                return;
            }
            if (dt_SN.Rows[0]["SN"].ToString() != "")
            {
                mad.strCPSN = dt_SN.Rows[0]["SN"].ToString();
                dt_SN.Rows[0]["SN"] = "";
                MasterSQL.Save_DataTable(dt_SN, "机台扫描SN结果表", CPublic.Var.geConn("PLC"));
            }
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
                        jianceleibie.EditValue = dr[0]["检测名称"].ToString();
                        DataRow[] drr1 = dt_检测机台表.Select(string.Format("机台名称='{0}'", shebei.EditValue.ToString()));
                        if (drr1.Length > 0)
                        {
                            chanxian.EditValue = drr1[0]["产线"].ToString();
                        }
                        //调用更换按钮
                        barLargeButtonItem2_ItemClick(null, null);
                    }

                }
                catch(Exception ex)
                {
                    CZMaster.MasterLog.WriteLog(ex.Message + "  fun_得到MAD后初始化检测类型");
                }
            }
        }

        #endregion



        #region   界面上的相关操作

        /// <summary>
        /// 关闭后。断开设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmABB_ParentChanged(object sender, EventArgs e)
        {
            if (this.Parent == null)
            {
                fun_断开设备();
                mad.blTeam = false;
            }

        }

        //设备连接的操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //连接设备之前需要填写产线和SN号
                if (liuzhuanka.EditValue == null)  //产线和SN号
                {
                    liuzhuanka.EditValue = "";
                }
                //if (liuzhuanka.EditValue.ToString() == "")
                //{
                //    throw new Exception("请输入流转卡SN号！");
                //}

                fun_连接设备();
              //  shebei.Enabled = false;
                shebei.Enabled = false;  //锁住设备
                barLargeButtonItem4.Enabled = false;  //组队按钮
                barLargeButtonItem1.Enabled = false;  //链接按钮

                this.Parent.Text = string.Format("检测平台[{0}]",shebei.EditValue.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //更换操作
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (mad == null)
                {
                    throw new Exception("请选择设备");
                }
                fun_组显示();    //显示组
                fun_加载检测参数(); //随时改变视图的显示
                DataTable dt_参数 = new DataTable();
                string sql = string.Format("select * from ABB检测类型主表 where 检测名称='{0}'", jianceleibie.EditValue.ToString());
                dt_参数 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
                //把参数传给你

                ///重要方法 AQUA注
                 mad.fun_检测参数(dt_参数);

                strCurrJCLBID = dt_参数.Rows[0]["检测ID"].ToString();
                strCurrJCLB = jianceleibie.EditValue.ToString();

                mad.dt_检测类型主表 = dt_参数;
                //传递重新组合后的参数

                int a = 1;
                dt_new子表 = new DataTable();
                dt_new子表 = dt_new动作子表.Clone();
               // dt_new子表.Columns.Add("组最后");  //一组最后的标记
                DataRow r = dt_new子表.NewRow();
                r["检测ID"] = dt_参数.Rows[0]["检测ID"].ToString();
                r["检测名称"] = dt_参数.Rows[0]["检测名称"].ToString();
                r["检测组POS"] = 0;
                r["检测组内POS"] = 0;
                r["主动作POS"] = 0;
                r["动作ID"] = "00";
                r["动作POS"] = a;
                r["P1"] = 0.5;
                dt_new子表.Rows.Add(r);
                DataRow[] dr = dt_new动作子表.Select(string.Format("检测ID='{0}' and 检测名称='{1}'", dt_参数.Rows[0]["检测ID"].ToString(), dt_参数.Rows[0]["检测名称"].ToString()));
                foreach (DataRow t in dr)
                {
                    a++; 
                    t["动作POS"] = a;
                    dt_new子表.Rows.Add(t.ItemArray);
                }
                DataRow r1 = dt_new子表.NewRow(); 
                r1["检测ID"] = dt_参数.Rows[0]["检测ID"].ToString();
                r1["检测名称"] = dt_参数.Rows[0]["检测名称"].ToString();
                r1["检测组POS"] = 0;
                r1["检测组内POS"] = 0;
                r1["主动作POS"] = 0;     
                r1["动作ID"] = "FE";
                r1["动作POS"] =++a;
                r["P1"] = 0.5;
                dt_new子表.Rows.Add(r1);

                ///重要属性 AQUA注
                mad.dt_检测组动作子表 = dt_new子表;

                (this.BindingContext[dv] as CurrencyManager).PositionChanged += frmABB_PositionChanged;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void frmABB_PositionChanged(object sender, EventArgs e)
        {
            try
            {
                DataRowView r = this.BindingContext[dv].Current as DataRowView;
                //fun_load动作组子表(r["检测ID"].ToString());
                //dv1 = new DataView(dt_动作子表);
                textBox1.Text = r["检测要求"].ToString();
                textBox2.Text = r["设备要求"].ToString();
                textBox3.Text = r["检测内容"].ToString();

                if (r["状态"].ToString() == "所有")
                {
                    dv1.RowFilter = string.Format("检测ID='{0}' and 检测名称='{1}'", r["检测ID"].ToString(), r["检测名称"].ToString());
                }

                if (r["状态"].ToString() != "所有")
                {
                    dv1.RowFilter = string.Format("检测ID='{0}' and 检测名称='{1}' and 检测组POS='{2}'", r["检测ID"].ToString(), r["检测名称"].ToString(), r["检测组POS"].ToString());
                }

                foreach (DataRowView drv in dv1)
                {
                    DataRow[] drr = dt_动作表.Select(string.Format("动作ID='{0}'", drv.Row["动作ID"].ToString()));
                    if (drv.Row["动作判定要求"].ToString() == "")
                    {
                        if (Convert.ToInt32(drr[0]["动作参数个数"]) > 0)
                        {
                            for (int g = 1; g <= Convert.ToInt32(drr[0]["动作参数个数"]); g++)
                            {
                                drv.Row["动作判定要求"] += " " + drr[0]["P" + g + ""].ToString() + @":" + drv.Row["P" + g + ""].ToString();
                            }
                        }
                    }

                    #region 注释方案
                    //switch (Convert.ToInt32(drr[0]["动作参数个数"].ToString()))
                    //{
                    //    case 0: break;
                    //    case 1: drv.Row["动作判定要求"] = drr[0]["P1"].ToString() + @":" + drv.Row["P1"].ToString(); break;
                    //    case 2: drv.Row["动作判定要求"] = drr[0]["P1"].ToString() + @":" + drv.Row["P1"].ToString() + @" || " + drr[0]["P2"].ToString() + @":" + drv.Row["P2"].ToString(); break;
                    //    case 3: drv.Row["动作判定要求"] = drr[0]["P1"].ToString() + @":" + drv.Row["P1"].ToString() + @" || " + drr[0]["P2"].ToString() + @":" + drv.Row["P2"].ToString() + @" || " + drr[0]["P3"].ToString() + @":" + drv.Row["P3"].ToString(); break;
                    //    case 4: drv.Row["动作判定要求"] = drr[0]["P1"].ToString() + @":" + drv.Row["P1"].ToString() + @" || " + drr[0]["P2"].ToString() + @":" + drv.Row["P2"].ToString() + @" || " + drr[0]["P3"].ToString() + @":" + drv.Row["P3"].ToString() + @" || " + drr[0]["P4"].ToString() + @":" + drv.Row["P4"].ToString(); break;
                    //    case 5: drv.Row["动作判定要求"] = drr[0]["P1"].ToString() + @":" + drv.Row["P1"].ToString() + @" || " + drr[0]["P2"].ToString() + @":" + drv.Row["P2"].ToString() + @" || " + drr[0]["P3"].ToString() + @":" + drv.Row["P3"].ToString() + @" || " + drr[0]["P4"].ToString() + @":" + drv.Row["P4"].ToString() + @" || " + drr[0]["P5"].ToString() + @":" + drv.Row["P5"].ToString(); break;
                    //}
                    #endregion

                }
               dv1.Sort = "动作POS";
               gcAct.DataSource = dv1;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        //把设备移除的操作
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                mad.blTeam = false;

                //PLCC.MachineManager.Remove(shebei.EditValue.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

       
       
        //gcTeam的单击事件，变化不同的子表
        private void gcTeam_Click(object sender, EventArgs e)
        {
            try
            {
                DataRowView r = this.BindingContext[dv].Current as DataRowView;
                //fun_load动作组子表(r["检测ID"].ToString());
                //dv1 = new DataView(dt_动作子表);
                textBox1.Text = r["检测要求"].ToString();
                textBox2.Text = r["设备要求"].ToString();
                textBox3.Text = r["检测内容"].ToString();
           
                if (r["状态"].ToString() == "所有")
                {
                    dv1.RowFilter = string.Format("检测ID='{0}' and 检测名称='{1}'", r["检测ID"].ToString(), r["检测名称"].ToString());
                }

                if (r["状态"].ToString() != "所有")
                {
                    dv1.RowFilter = string.Format("检测ID='{0}' and 检测名称='{1}' and 检测组POS='{2}'", r["检测ID"].ToString(), r["检测名称"].ToString(), r["检测组POS"].ToString());
                }

                foreach (DataRowView drv in dv1)
                {
                    DataRow[] drr = dt_动作表.Select(string.Format("动作ID='{0}'", drv.Row["动作ID"].ToString()));
                    if (drv.Row["动作判定要求"].ToString() == "")
                    {
                        if (Convert.ToInt32(drr[0]["动作参数个数"]) > 0)
                        {
                            for (int g = 1; g <= Convert.ToInt32(drr[0]["动作参数个数"]); g++)
                            {
                                drv.Row["动作判定要求"] += " " + drr[0]["P" + g + ""].ToString() + @":" + drv.Row["P" + g + ""].ToString();
                            }
                        }

                    }

                    #region 注释方案
                    //switch (Convert.ToInt32(drr[0]["动作参数个数"].ToString()))
                    //  {
                    //case 0: break;
                    // case 1: drv.Row["动作判定要求"] = drr[0]["P1"].ToString() + @":" + drv.Row["P1"].ToString(); break;
                    //  case 2: drv.Row["动作判定要求"] = drr[0]["P1"].ToString() + @":" + drv.Row["P1"].ToString() + @" || " + drr[0]["P2"].ToString() + @":" + drv.Row["P2"].ToString(); break;
                    // case 3: drv.Row["动作判定要求"] = drr[0]["P1"].ToString() + @":" + drv.Row["P1"].ToString() + @" || " + drr[0]["P2"].ToString() + @":" + drv.Row["P2"].ToString() + @" || " + drr[0]["P3"].ToString() + @":" + drv.Row["P3"].ToString(); break;
                    //  case 4: drv.Row["动作判定要求"] = drr[0]["P1"].ToString() + @":" + drv.Row["P1"].ToString() + @" || " + drr[0]["P2"].ToString() + @":" + drv.Row["P2"].ToString() + @" || " + drr[0]["P3"].ToString() + @":" + drv.Row["P3"].ToString() + @" || " + drr[0]["P4"].ToString() + @":" + drv.Row["P4"].ToString(); break;
                    //  case 5: drv.Row["动作判定要求"] = drr[0]["P1"].ToString() + @":" + drv.Row["P1"].ToString() + @" || " + drr[0]["P2"].ToString() + @":" + drv.Row["P2"].ToString() + @" || " + drr[0]["P3"].ToString() + @":" + drv.Row["P3"].ToString() + @" || " + drr[0]["P4"].ToString() + @":" + drv.Row["P4"].ToString() + @" || " + drr[0]["P5"].ToString() + @":" + drv.Row["P5"].ToString(); break;
                    // }

                    #endregion

                }
                dv1.Sort = "动作POS";
                gcAct.DataSource = dv1;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


  
        }

    

        //组队链接按钮
        public void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_连接设备();
                button1.BackColor = Color.Orange;
                button1.Text = "已组队√";
                mad.blTeam = true;
                shebei.Enabled = false;  //锁住设备
                barLargeButtonItem4.Enabled = false;  //组队按钮
                barLargeButtonItem1.Enabled = false;  //链接按钮
                this.Parent.Text = string.Format("检测平台[{0}]", shebei.EditValue.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion





    }
}
      