using CPublic;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraTab;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace FutureMain
{
    public partial class FutureMainFM : Form
    {
        #region 类启动

        fmLog flog = null;
        fm消息窗体 fm = new fm消息窗体();
        public FutureMainFM()
        {

            InitializeComponent();
            load_menu(); //加载空的


        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);
        private void FutureMainFM_Load(object sender, EventArgs e)
        {
            try
            {

                bar2.Appearance.Font = new Font("Tahoma", 12);
                #region gridcontrol汉化代码
                //DevExpress.Accessibility.AccLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressUtilsLocalizationCHS();
                ////DevExpress.XtraBars.Localization.BarLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraBarsLocalizationCHS();
                ////DevExpress.XtraCharts.Localization.ChartLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraChartsLocalizationCHS();
                //DevExpress.XtraEditors.Controls.Localizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraEditorsLocalizationCHS();
                //DevExpress.XtraGrid.Localization.GridLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraGridLocalizationCHS();
                //DevExpress.XtraLayout.Localization.LayoutLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraLayoutLocalizationCHS();
                ////DevExpress.XtraNavBar.NavBarLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraNavBarLocalizationCHS();
                ////DevExpress.XtraPivotGrid.Localization.PivotGridLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraPivotGridLocalizationCHS();
                //DevExpress.XtraPrinting.Localization.PreviewLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraPrintingLocalizationCHS();
                //DevExpress.XtraReports.Localization.ReportLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraReportsLocalizationCHS();
                //DevExpress.XtraRichEdit.Localization.XtraRichEditLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraRichEditLocalizationCHS();
                //DevExpress.XtraRichEdit.Localization.RichEditExtensionsLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraRichEditExtensionsLocalizationCHS();
                //DevExpress.XtraScheduler.Localization.SchedulerLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraSchedulerLocalizationCHS();
                //DevExpress.XtraScheduler.Localization.SchedulerExtensionsLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraSchedulerExtensionsLocalizationCHS();
                //DevExpress.XtraSpellChecker.Localization.SpellCheckerLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraSpellCheckerLocalizationCHS();
                //DevExpress.XtraTreeList.Localization.TreeListLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraTreeListLocalizationCHS();
                //DevExpress.XtraVerticalGrid.Localization.VGridLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraVerticalGridLocalizationCHS();
                //DevExpress.XtraWizard.Localization.WizardLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraWizardLocalizationCHS();
                #endregion




                //  label1.Left = this.Size.Width / 3;

                try
                {
                    CPublic.Var.localUserName = CZMaster.LocalDataSetting.getLocalData("APPUser")[0];
                }
                catch { }
                XTC.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.InActiveTabPageHeader;
                UIcontrol.XTC = this.XTC;

                //加载配置信息 查看当前机器是否是需要默认人员 登录打开默认界面
                string pcname = System.Net.Dns.GetHostName(); //当前设备名称
                string ss = string.Format(@"select 设备自动打开界面配置表.*,PWD from [设备自动打开界面配置表],人事基础员工表 
                where  [设备自动打开界面配置表].登录ID=人事基础员工表.员工号 and   设备名称='{0}'", pcname);
                using (SqlDataAdapter da = new SqlDataAdapter(ss, CPublic.Var.strConn))
                {
                    DataTable t = new DataTable();
                    da.Fill(t);
                    if (t.Rows.Count > 0)
                    {
                        Fun_验证(t.Rows[0]["登录ID"].ToString(), t.Rows[0]["PWD"].ToString());
                        bar2.Visible = Convert.ToBoolean(t.Rows[0]["菜单隐藏"]);
                        bar3.Visible = Convert.ToBoolean(t.Rows[0]["菜单隐藏"]);
                        XTC.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
                        Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", t.Rows[0]["程序集"].ToString())));  //  ERPproduct.dll
                        Type outerForm = outerAsm.GetType(t.Rows[0]["打开界面ID"].ToString(), false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                        // Form ui = Activator.CreateInstance(outerForm) as Form;
                        UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
                        CPublic.UIcontrol.Showpage(ui, t.Rows[0]["打开界面名称"].ToString());

                        //开机自启动时 该程序置顶
                        if (Convert.ToBoolean(t.Rows[0]["置顶锁定"]))
                        {
                            SetWindowPos(this.Handle, -1, 0, 0, 0, 0, 1 | 2);
                        }
                    }
                    else
                    {
                        if (flog == null)
                        {
                            flog = new fmLog();
                            //flog.Controls.Add(CZMaster.MasterLog.frmLog);
                            //CZMaster.MasterLog.frmLog.Dock = DockStyle.Fill;
                            if (flog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                            {

                                this.Close();


                            }

                        }
                    }
                }
                //// 3.30 郭恒  
                // //加权限
                //string str_ID = CPublic.Var.LocalUserID;
                //foreach (DevExpress.XtraBars.BarSubItemLink a in barM.MainMenu.ItemLinks)
                //{
                //  foreach (devExpress.)

                //    a.Visible = ERPorg.Corg.fun_权限(str_ID, a.Caption);

                //} 
                barM.BeginInit();
                bar2.BeginUpdate();
                load_menu();
                bar2.EndUpdate();
                barM.EndInit();
                string[] s = ERPorg.Corg.fun_版本号();

                bool bl = ERPorg.Corg.fun_isnd();
                if (bl)
                {
                    int length = 0;
                    try
                    {
                        length = s[1].IndexOf("V");

                    }
                    catch
                    {

                    }
                    if (length > 0)
                    {
                        s[1] = s[0] + "\r\n" + s[1].Substring(0, s[1].IndexOf("V", 2));
                    }
                    else
                    {
                        s[1] = s[0] + "\r\n" + s[1];
                    }
                    MessageBox.Show(s[1], "更新内容");

                }
                string sql = string.Format("select * from 人事基础员工表 where 员工号='{0}'", CPublic.Var.LocalUserID);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                CPublic.Var.localUserName = temp.Rows[0]["姓名"].ToString();
                if (temp.Rows.Count > 0)
                {
                    barStaticItem1.Caption = string.Format("当前登录人员:{0}_{1} 版本号:{2} 服务器地址:{3} 数据库:{4}",
                        CPublic.Var.LocalUserID, temp.Rows[0]["姓名"], s[0].Trim(), CPublic.Var.ServerIP, CPublic.Var.li_CFG["DataBase"]);

                }
                else
                {
                    barStaticItem1.Caption = string.Format("当前登录人员:{0}_{1} 版本号:{2} 服务器地址:{3} 数据库:{4}",
                        CPublic.Var.LocalUserID, CPublic.Var.localUserName, s[0].Trim(), CPublic.Var.ServerIP, CPublic.Var.li_CFG["DataBase"]);
                }

                //timer1.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void load_menu()
        {
            DataTable temp = new DataTable();
            temp.Columns.Add("菜单名称");
            temp.Columns.Add("菜单类型");

            temp.Columns.Add("菜单顺序");
            temp.Columns.Add("层级");

            temp.Columns.Add("dll全称");
            temp.Columns.Add("窗体显示名称");
            temp.Columns.Add("命名空间.窗体名称");
            temp.Columns.Add("图标");
            temp.Columns.Add("是否显示");
            temp.Columns.Add("上级菜单");
            temp.Columns.Add("是否分组");
            temp.Columns.Add("菜单级别");
            temp.Columns.Add("权限");
            temp.Columns.Add("备注");
            temp.Columns.Add("窗体类型");
            temp.TableName = "自定义菜单";
            temp.ReadXml(Path.Combine(Application.StartupPath, @"自定义菜单.xml"));

            if (CPublic.Var.LocalUserID == "admin")
            {
                Button_new_make(temp, XTC, barM, bar2);
            }
            else
            {
                string ss = string.Format("select 权限类型,上级权限 from [功能权限权限组权限表] where 权限组='{0}'", CPublic.Var.LocalUserTeam);
                DataTable dt_authorization = CZMaster.MasterSQL.Get_DataTable(ss, CPublic.Var.strConn);
                DataTable temp_1 = temp.Clone();
                foreach (DataRow dr in dt_authorization.Rows)
                {
                    string s_filter = "";
                    if (dr["上级权限"].ToString() == "")
                    {
                        s_filter = string.Format("菜单名称='{0}'", dr["权限类型"].ToString().Trim(), "菜单顺序");
                    }
                    else
                    {
                        s_filter = string.Format("菜单名称='{0}'and 上级菜单='{1}' ", dr["权限类型"], dr["上级权限"].ToString(), "菜单顺序");

                    }
                    // 上级菜单为空时 找不到 记录  有问题
                    //DataRow[] x = temp.Select(string.Format("菜单名称='{0}'and 上级菜单='{1}' ", dr["权限类型"], dr["上级权限"].ToString().Trim()));
                    DataRow[] x = temp.Select(s_filter);

                    //如果 x.lenth>1只要一个即可
                    if (x.Length > 0)
                    {
                        temp_1.ImportRow(x[0]);
                    }
                }
                Button_new_make(temp_1, XTC, barM, bar2);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="cj"></param>
        /// <param name="Xtc"></param>
        /// <param name="barManager"></param>
        /// <param name="bar"></param>
        /// <param name="i_总"></param>
        /// <param name="dt_all"></param>
        /// <param name="barSubItems">上级菜单</param>
        private void MakeButton(DataTable dt, int cj, XtraTabControl Xtc, BarManager barManager, Bar bar, int i_总, DataTable dt_all, BarSubItem barSubItems)
        {
            ERPorg.Corg x = new ERPorg.Corg();
            foreach (DataRow dr in dt.Rows)
            {
                //DataRow[] xxx = dt_authorization.Select(string.Format("权限类型='{0}' or 上级权限='{0}'", dr["菜单名称"].ToString()));
                //if (xxx.Length > 0)
                //{
                if (dr["菜单类型"].ToString().Contains("BarSubItem") == true)
                {
                    DevExpress.XtraBars.BarSubItem barSubItem = new DevExpress.XtraBars.BarSubItem();
                    barManager.Items.Add(barSubItem);
                    barManager.MaxItemId = i_总;
                    barSubItem.Caption = dr["菜单名称"].ToString();
                    barSubItem.Id = i_总;
                    barSubItem.Name = dr["菜单名称"].ToString();
                    if (barSubItems == null)
                        bar.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(barSubItem));
                    else barSubItems.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(barSubItem));

                    //if (cj == 1)
                    //{
                    //    barSubItem.Appearance.Font = new System.Drawing.Font("Tahoma",14.0F);
                    //}
                    DataRow[] rrr = dt_all.Select(string.Format("上级菜单='{0}' and 层级={1}", dr["菜单名称"].ToString(), cj + 1), "菜单顺序");

                    DataTable tt = dt.Clone();

                    foreach (DataRow de in rrr)
                    {
                        if (tt.Select(string.Format("菜单名称='{0}'", de["菜单名称"])).Length > 0) continue;
                        tt.ImportRow(de);

                    }
                    if (rrr.Length > 0)
                    {
                        MakeButton(tt, cj + 1, Xtc, barManager, bar, i_总, dt_all, barSubItem);
                    }
                }
                else
                {
                    DevExpress.XtraBars.BarButtonItem barButtonItem = new DevExpress.XtraBars.BarButtonItem();
                    //将本菜单添加到barManager中
                    barManager.Items.Add(barButtonItem);
                    //设置barManager的最大Item数
                    barManager.MaxItemId = i_总;
                    //设置菜单属性
                    barButtonItem.Caption = dr["菜单名称"].ToString();
                    barButtonItem.Id = i_总;
                    barButtonItem.Name = dr["菜单名称"].ToString();
                    if (barSubItems == null)
                        bar.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(barButtonItem));
                    else barSubItems.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(barButtonItem));
                    // barButtonItem.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
                    i_总++;
                    //生成单击事件
                    barButtonItem.ItemClick += (sender, e) =>
                    {
                        if (CPublic.Var.LocalUserID == "admin" || x.fun_权限(CPublic.Var.LocalUserID, e.Item.Caption) == true)
                        {
                            if (dr["窗体类型"].ToString() == "messagebox")
                            {
                                string[] s = ERPorg.Corg.fun_版本号();
                                int length = 0;
                                try
                                {
                                    length = s[1].IndexOf("V");
                                }
                                catch
                                {
                                }
                                if (length > 0)
                                {
                                    s[1] = s[0] + " \n " + s[1].Substring(0, s[1].IndexOf("V", 2));
                                }
                                else
                                {
                                    s[1] = s[0] + " \n " + s[1];
                                }
                                MessageBox.Show(s[1], "更新内容");
                            }
                            else if (dr["窗体类型"].ToString() == "form")
                            {
                                LoadInForm(dr);
                            }
                            else if (dr["窗体类型"].ToString() == "网址")
                            {
                            }
                            else
                            {
                                LoadInUserControl(dr, Xtc);
                            }
                        }
                        else
                        {

                            MessageBox.Show("你没有权限使用此功能");
                        }
                    };


                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtM"></param>
        /// <param name="Xtc"></param>
        /// <param name="barManager"></param>
        /// <param name="bar"></param>
        /// <param name="dt_authorization"> 权限组权限</param>
        public void Button_new_make(DataTable dtM, XtraTabControl Xtc, BarManager barManager, Bar bar)
        {

            bar.ClearLinks();
            // barManager.Items.Clear();
            bar.LinksPersistInfo.Clear();
            int i = 1;
            DataView dv = new DataView(dtM)
            {
                //dv.RowFilter = "层级 =1 and 上级菜单 is null";

                RowFilter = "层级 =1 and 上级菜单 is null",
                Sort = "菜单顺序"
            };
            DataTable temp = dv.ToTable();
            MakeButton(temp, 1, Xtc, barManager, bar, i, dtM, null);


        }


        public void Fun_生成菜单(DataTable dtM, XtraTabControl Xtc, BarManager barManager, Bar bar)
        {
            //清空菜单
            ERPorg.Corg x = new ERPorg.Corg();
            bar.ClearLinks();
            barManager.Items.Clear();
            bar.LinksPersistInfo.Clear();
            int i = 1;
            DataView dv = new DataView(dtM);
            Dictionary<string, BarSubItem> dic = new Dictionary<string, BarSubItem>();

            dv.RowFilter = "菜单级别 = '一级菜单'";
            dv.Sort = "一级菜单顺序";

            foreach (DataRow dr in dv.ToTable().Rows)
            {
                if (dr["菜单类型"].ToString().Contains("BarSubItem") == true)
                {
                    DevExpress.XtraBars.BarSubItem barSubItem = new DevExpress.XtraBars.BarSubItem();
                    barManager.Items.Add(barSubItem);
                    barManager.MaxItemId = i;
                    barSubItem.Caption = dr["菜单名称"].ToString();
                    barSubItem.Id = i;
                    barSubItem.Name = dr["菜单名称"].ToString();

                    dic.Add(barSubItem.Caption, barSubItem);
                    barSubItem.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
                    bar.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(barSubItem));
                }
                else
                {
                    DevExpress.XtraBars.BarButtonItem barButtonItem = new DevExpress.XtraBars.BarButtonItem();
                    //将本菜单添加到barManager中
                    barManager.Items.Add(barButtonItem);
                    //设置barManager的最大Item数
                    barManager.MaxItemId = i;
                    //设置菜单属性
                    barButtonItem.Caption = dr["菜单名称"].ToString();
                    barButtonItem.Id = i;
                    barButtonItem.Name = dr["菜单名称"].ToString();
                    bar.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(barButtonItem));


                    //生成单击事件
                    barButtonItem.ItemClick += (sender, e) =>
                    {
                        if (CPublic.Var.LocalUserID == "admin" || x.fun_权限(CPublic.Var.LocalUserID, e.Item.Caption) == true)
                        {
                            if (dr["窗体类型"].ToString() == "messagebox")
                            {

                            }
                            else if (dr["窗体类型"].ToString() == "form")
                            {
                                LoadInForm(dr);
                            }
                            else if (dr["窗体类型"].ToString() == "网址")
                            {
                            }
                            else
                            {
                                LoadInUserControl(dr, Xtc);
                            }
                        }
                        else
                        {

                            MessageBox.Show("你没有权限使用此功能");
                        }
                    };
                }






                i++;

            }

            dv.RowFilter = "菜单级别 = '二级菜单'";
            dv.Sort = "二级菜单顺序";
            foreach (DataRow dr in dv.ToTable().Rows)
            {
                if (dr["菜单类型"].ToString().Contains("BarSubItem") == true)
                {
                    DevExpress.XtraBars.BarSubItem barSubItem = new DevExpress.XtraBars.BarSubItem();
                    barManager.Items.Add(barSubItem);
                    barManager.MaxItemId = i;
                    barSubItem.Caption = dr["菜单名称"].ToString();
                    barSubItem.Id = i;
                    barSubItem.Name = dr["菜单名称"].ToString();

                    dic.Add(barSubItem.Caption, barSubItem);

                    if (dr["上级菜单"].ToString() != "")
                    {
                        foreach (BarSubItem barSubItems in dic.Values)
                        {
                            if (barSubItems.Caption == dr["上级菜单"].ToString())
                            {
                                barSubItems.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(barSubItem));
                                break;
                            }
                        }
                    }
                    else
                    {
                        bar.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(barSubItem));
                    }
                }
                else
                {
                    DevExpress.XtraBars.BarButtonItem barButtonItem = new DevExpress.XtraBars.BarButtonItem();
                    //将本菜单添加到barManager中
                    barManager.Items.Add(barButtonItem);
                    //设置barManager的最大Item数
                    barManager.MaxItemId = i;
                    //设置菜单属性
                    barButtonItem.Caption = dr["菜单名称"].ToString();
                    barButtonItem.Id = i;
                    barButtonItem.Name = dr["菜单名称"].ToString();
                    if (dr["上级菜单"].ToString() != "")
                    {
                        foreach (BarSubItem barSubItems in dic.Values)
                        {
                            if (barSubItems.Caption == dr["上级菜单"].ToString())
                            {
                                barSubItems.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(barButtonItem));
                                break;
                            }
                        }
                    }
                    else
                    {
                        bar.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(barButtonItem));
                    }

                    //生成单击事件
                    barButtonItem.ItemClick += (sender, e) =>
                    {
                        if (CPublic.Var.LocalUserID == "admin" || x.fun_权限(CPublic.Var.LocalUserID, e.Item.Caption) == true)
                        {
                            if (dr["窗体类型"].ToString() == "messagebox")
                            {
                                string[] s = ERPorg.Corg.fun_版本号();

                                int length = 0;
                                try
                                {
                                    length = s[1].IndexOf("V");

                                }
                                catch
                                {

                                }
                                if (length > 0)
                                {
                                    s[1] = s[0] + " \n " + s[1].Substring(0, s[1].IndexOf("V", 2));
                                }
                                else
                                {
                                    s[1] = s[0] + " \n " + s[1];
                                }
                                MessageBox.Show(s[1], "更新内容");
                            }
                            else if (dr["窗体类型"].ToString() == "form")
                            {
                                LoadInForm(dr);
                            }
                            else if (dr["窗体类型"].ToString() == "网址")
                            {
                                System.Diagnostics.Process.Start("explorer.exe", "http://login.baiten.cn/?rurl=http://www.baiten.cn/");

                            }
                            else
                            {

                                LoadInUserControl(dr, Xtc);
                            }
                        }
                        else
                        {

                            MessageBox.Show("你没有权限使用此功能");
                        }
                    };
                }
                i++;
            }

            dv.RowFilter = "菜单级别 = '三级菜单'";
            dv.Sort = "三级菜单顺序";
            foreach (DataRow dr in dv.ToTable().Rows)
            {
                if (dr["菜单类型"].ToString().Contains("BarSubItem") == true)
                {
                    DevExpress.XtraBars.BarSubItem barSubItem = new DevExpress.XtraBars.BarSubItem();
                    barManager.Items.Add(barSubItem);
                    barManager.MaxItemId = i;
                    barSubItem.Caption = dr["菜单名称"].ToString();
                    barSubItem.Id = i;
                    barSubItem.Name = dr["菜单名称"].ToString();

                    dic.Add(barSubItem.Caption, barSubItem);

                    if (dr["上级菜单"].ToString() != "")
                    {
                        foreach (BarSubItem barSubItems in dic.Values)
                        {
                            if (barSubItems.Caption == dr["上级菜单"].ToString())
                            {
                                barSubItems.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(barSubItem));
                                break;
                            }
                        }
                    }
                    else
                    {
                        bar.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(barSubItem));
                    }
                }
                else
                {
                    DevExpress.XtraBars.BarButtonItem barButtonItem = new DevExpress.XtraBars.BarButtonItem();
                    //将本菜单添加到barManager中
                    barManager.Items.Add(barButtonItem);
                    //设置barManager的最大Item数
                    barManager.MaxItemId = i;
                    //设置菜单属性
                    barButtonItem.Caption = dr["菜单名称"].ToString();
                    barButtonItem.Id = i;
                    barButtonItem.Name = dr["菜单名称"].ToString();
                    if (dr["上级菜单"].ToString() != "")
                    {
                        foreach (BarSubItem barSubItems in dic.Values)
                        {
                            if (barSubItems.Caption == dr["上级菜单"].ToString())
                            {
                                barSubItems.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(barButtonItem));
                                break;
                            }
                        }
                    }
                    else
                    {
                        bar.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(barButtonItem));
                    }

                    //生成单击事件
                    barButtonItem.ItemClick += (sender, e) =>
                    {
                        if (CPublic.Var.LocalUserID == "admin" || x.fun_权限(CPublic.Var.LocalUserID, e.Item.Caption) == true)
                        {
                            if (dr["窗体类型"].ToString() == "messagebox")
                            {

                            }
                            else if (dr["窗体类型"].ToString() == "form")
                            {
                                LoadInForm(dr);
                            }
                            else if (dr["窗体类型"].ToString() == "网址")
                            {

                            }
                            else
                            {

                                LoadInUserControl(dr, Xtc);
                            }
                        }
                        else
                        {

                            MessageBox.Show("你没有权限使用此功能");
                        }
                    };
                }
                i++;
            }


        }

        private void LoadInForm(DataRow dr)
        {
            try
            {

                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, @dr["dll全称"].ToString()));//dr["dll全路径"] = "动态载入dll.dll"
                Type outerForm = outerAsm.GetType(dr["命名空间.窗体名称"].ToString(), false);//动态载入dll.UI动态载入窗体


                Form fm = (Form)Activator.CreateInstance(outerForm);

                fm.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void LoadInUserControl(DataRow dr, XtraTabControl Xtc)
        {
            try
            {

                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, @dr["dll全称"].ToString()));//dr["dll全路径"] = "动态载入dll.dll"
                Type outerForm = outerAsm.GetType(dr["命名空间.窗体名称"].ToString(), false);//动态载入dll.UI动态载入窗体
                UserControl ui = Activator.CreateInstance(outerForm) as UserControl;

                if (!(ui == null))
                {
                    XtraTabPage xtp = Xtc.TabPages.Add(dr["窗体显示名称"].ToString());
                    xtp.ShowCloseButton = DefaultBoolean.Default;
                    xtp.Controls.Add(ui);
                    ui.AllowDrop = true;
                    xtp.AllowDrop = true;
                    ui.Dock = DockStyle.Fill;
                    Xtc.SelectedTabPage = xtp;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        /// <summary>
        /// 设备自动登录 模拟的登录 赋值
        /// </summary>
        private void Fun_验证(string s_用户, string pwd)
        {
            try
            {
                string strcon = CPublic.Var.strConn;
                string sql = string.Format("select * from 人事基础员工表 where 员工号 = '{0}' and PWD = '{1}'", s_用户, pwd);
                SqlDataAdapter daM = new SqlDataAdapter(sql, strcon);
                DataTable dt = new DataTable();
                daM.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    CPublic.Var.LocalUserID = s_用户;
                    CPublic.Var.localUserName = dt.Rows[0]["姓名"].ToString();
                    CPublic.Var.LocalUserTeam = dt.Rows[0]["权限组"].ToString();
                    CPublic.Var.localUser部门编号 = dt.Rows[0]["部门编号"].ToString();
                    // CPublic.Var.localUser部门名称 = dt.Rows[0]["部门"].ToString();

                    CPublic.Var.localUser课室编号 = dt.Rows[0]["课室编号"].ToString();
                    CPublic.Var.localUser工号简码 = dt.Rows[0]["工号简码"].ToString();
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    if (pwd.Length < 6 || pwd == "123456")
                    {
                        string s = "密码若为起始密码必须修改密码,并且密码不小于6位";
                        ERPorg.修改密码界面 fm = new ERPorg.修改密码界面(s);
                        CPublic.UIcontrol.Showpage(fm, "修改密码");
                    }
                }
                else
                {
                    throw new Exception("用户名或密码错误！");
                }
            }
            catch (Exception)
            {
                throw new Exception("用户名或密码错误！");
            }

        }

        private void XTC_CloseButtonClick(object sender, EventArgs e)
        {
            UIcontrol.ClosePage();
        }








        private void XTC_CloseButtonClick_1(object sender, EventArgs e)
        {
            UIcontrol.ClosePage();
        }

        private void barStaticItem1_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void barLargeButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {

            fmLog frm = new fmLog();
            frm.ShowDialog();
            UIcontrol.closeallpage();
            FutureMainFM_Load(null, null);
        }

        private void FutureMainFM_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (MessageBox.Show(string.Format("是否确认退出系统？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                string s = "";
                if (CPublic.Var.LocalUserID == "admin" || CPublic.Var.LocalUserTeam == "管理员权限")
                {
                    s = "select  * from [单据审核申请表]   where   作废=0 and  审核=0 ";
                }
                else
                {
                    s = string.Format(@" select  a.*  from [单据审核申请表] a 
                      left join 单据审批流表 splb on splb.单据类型=a.单据类型  and (待审核人ID=工号 or 待审核人ID=备用人工号 or 待审核人ID=[备用人工号1])
                      where  a.作废=0 and a.审核=0   and  (待审核人ID ='{0}' or [备用人工号1]='{0}' or 备用人工号='{0}') and 角色='审核人' ", CPublic.Var.LocalUserID);
                }
                DataTable dt_ll = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn);
                if (dt_ll.Rows.Count > 0)
                {
                    int x = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size.Width - 279;
                    int y = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size.Height - 176;
                    
                    

                    if (fm.IsHandleCreated)
                    {
                        fm.Text = CPublic.Var.getDatetime().ToString();

                        fm.Visible = true;
                        fm.WindowState = FormWindowState.Normal;
                        //fm.SetDesktopLocation(x, y);
                      

                    }
                    else
                    {
                        fm = new fm消息窗体();
                        //fm.Size = new Size(279, 176);
                        //fm.SetDesktopLocation(x, y);
                         
                    }
                    fm.TopMost = true;
                    //fm.Activate();
                    string ss = $"单据审核{dt_ll.Rows.Count}条待审";
                    fm.label1.Text = ss;
                    fm.Show();
                    fm.Location = new Point(x, y);
                    fm.Size = new Size(279, 176);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        

   
    }
}
