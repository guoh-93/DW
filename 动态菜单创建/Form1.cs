using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraTab;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
 
using CPublic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;

using DevExpress.XtraBars;
using DevExpress.XtraTab;
namespace 动态菜单创建
{
    public partial class Form1 : Form
    {
        DataTable dtM = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                dtM = new DataTable();
                dtM.Columns.Add("菜单名称");
                dtM.Columns.Add("菜单类型");
                dtM.Columns.Add("菜单顺序");

                dtM.Columns.Add("dll全称");
                dtM.Columns.Add("窗体显示名称");
                dtM.Columns.Add("命名空间.窗体名称");
                dtM.Columns.Add("图标");
                dtM.Columns.Add("是否显示");
                dtM.Columns.Add("上级菜单");
                dtM.Columns.Add("是否分组");
                dtM.Columns.Add("层级");
                dtM.Columns.Add("权限");
                dtM.Columns.Add("备注");
                dtM.Columns.Add("窗体类型");
                dtM.TableName = "自定义菜单";

                gc.DataSource = dtM;

                dtM.ReadXml(Path.Combine(Application.StartupPath, @"自定义菜单.xml"));
                string ss = string.Format("select 权限类型,上级权限 from [功能权限权限组权限表] where 权限组='admin'");

                DataTable dt_authorization = CZMaster.MasterSQL.Get_DataTable(ss, CPublic.Var.strConn);

                button_new_make(dtM, XTC, barManager1, bar2, dt_authorization);
                //fun_生成菜单(dtM, XTC, barManager1, bar2);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region 生成菜单
        /// <summary>
        /// 生成菜单:4个参数，datatable、XtraTabControl、barManager、barManager里的bar
        /// </summary>
        /// <param name="dt">菜单配置datatable</param>
        /// <param name="Xtc">主界面的XtraTabControl</param>
        /// <param name="barManager">主界面的菜单barManager,一般为barManager1</param>
        /// <param name="bar">主界面的菜单bar,一般为bar2</param>
        public void fun_生成菜单s(DataTable dtM, XtraTabControl Xtc, BarManager barManager,Bar bar)
        {
            //清空菜单
            barManager.Items.Clear();
            bar.LinksPersistInfo.Clear();
            int i = 1;
            DataView dv = new DataView(dtM);

            //1. 添加下拉菜单（必须先添加下拉菜单，才能添加下级单击菜单）
            dv.RowFilter = "菜单类型 = '下拉菜单：BarSubItem'";
            dv.Sort = "下拉菜单顺序";
            foreach (DataRow dr in dv.ToTable().Rows)
            {
                //同下
                DevExpress.XtraBars.BarSubItem barSubItem = new DevExpress.XtraBars.BarSubItem();
                barManager.Items.Add(barSubItem);
                barManager.MaxItemId = i;
                barSubItem.Caption = dr["菜单名称"].ToString();
                barSubItem.Id = i;
                barSubItem.Name = dr["菜单名称"].ToString();

                if (dr["上级菜单"].ToString() != "")
                {
                    foreach (BarSubItem barSubItems in barManager.Items)
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
                i++;
            }

            //2. 添加单击菜单
            dv.RowFilter = "菜单类型 = '单击菜单：BarButtonItem'";
            dv.Sort = "单击菜单顺序";
            foreach (DataRow dr in dv.ToTable().Rows)
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
                //begingroup属性暂时没找到

                //LinksPersistInfo作用：1.决定菜单的顺序 2.决定菜单的位置（即是在bar上还是下拉菜单上）
                if (dr["上级菜单"].ToString() != "")
                {
                    foreach (BarSubItem barSubItem in barManager.Items)
                    {
                        if (barSubItem.Caption == dr["上级菜单"].ToString())
                        {
                            barSubItem.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(barButtonItem));
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
                    LoadInUserControl(dr, Xtc);
                };
                i++;
            }

        }

        private void makeButton(DataTable dt, int cj, XtraTabControl Xtc, BarManager barManager, Bar bar, int i_总, DataTable dt_all, BarSubItem barSubItems, DataTable dt_authorization)
        {
            ERPorg.Corg x = new ERPorg.Corg();
            foreach (DataRow dr in dt.Rows)
            {
                DataRow[] xxx = dt_authorization.Select(string.Format("权限类型='{0}' or 上级权限='{0}'", dr["菜单名称"].ToString()));
                if (xxx.Length > 0)
                {
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

                        if (cj == 1) barSubItem.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);


                        DataRow[] rrr = dt_all.Select(string.Format("上级菜单='{0}' and 层级={1}", dr["菜单名称"].ToString(), cj + 1));

                        DataTable tt = dt.Clone();

                        foreach (DataRow de in rrr)
                        {
                            if (tt.Select(string.Format("菜单名称='{0}'", de["菜单名称"])).Length > 0) continue;
                            tt.ImportRow(de);

                        }
                        if (rrr.Length > 0)
                        {
                            makeButton(tt, cj + 1, Xtc, barManager, bar, i_总, dt_all, barSubItem, dt_authorization);
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
                            if (CPublic.Var.LocalUserID == "admin" || x.fun_权限(CPublic.Var.LocalUserID,e.Item.Caption) == true)
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
        public void button_new_make(DataTable dtM, XtraTabControl Xtc, BarManager barManager, Bar bar, DataTable dt_authorization)
        {
            bar.ClearLinks();
            barManager.Items.Clear();
            bar.LinksPersistInfo.Clear();
            int i = 1;
            DataView dv = new DataView(dtM);
            //dv.RowFilter = "层级 =1 and 上级菜单 is null";

            dv.RowFilter = "层级 =1 and 上级菜单 is null";
            dv.Sort = "菜单顺序";
            DataTable temp = dv.ToTable();
            makeButton(temp, 1, Xtc, barManager, bar, i, dtM, null, dt_authorization);


        }
        public void fun_生成菜单(DataTable dtM, XtraTabControl Xtc, BarManager barManager, Bar bar)
        {
            //清空菜单
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
                        LoadInUserControl(dr, Xtc);
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
                        LoadInUserControl(dr, Xtc);
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
                        LoadInUserControl(dr, Xtc);
                    };
                }
                i++;
            }
        }

        private void LoadInUserControl(DataRow dr, XtraTabControl Xtc)
        {
            try
            {
                //比如你的程序路径是  c:\test\bin\debug\test.exe
                //则获取到得结果就是  c:\test\bin\debug
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, @dr["dll全称"].ToString()));//dr["dll全路径"] = "动态载入dll.dll"
                Type outerForm = outerAsm.GetType(dr["命名空间.窗体名称"].ToString(), false);//动态载入dll.UI动态载入窗体
                UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
                if (!(ui == null))
                {
                    XtraTabPage xtp = Xtc.TabPages.Add(dr["窗体显示名称"].ToString());
                    xtp.ShowCloseButton = DefaultBoolean.Default;
                    xtp.Controls.Add(ui);
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

        #region 界面操作
        private void button3_Click(object sender, EventArgs e)
        {
            DataRow dr = dtM.NewRow();
            dtM.Rows.Add(dr);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                dr.Delete();
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                gv.CloseEditor();
                gc.BindingContext[dtM].EndCurrentEdit();

                dtM.WriteXml(Path.Combine(Application.StartupPath, @"自定义菜单.xml"));
                MessageBox.Show("导出成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                dtM.Clear();

                dtM.ReadXml(Path.Combine(Application.StartupPath, @"自定义菜单.xml"));
                MessageBox.Show("导入成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void gv_ShownEditor(object sender, EventArgs e)
        {
            gv.ActiveEditor.MouseWheel += ActiveEditor_MouseWheel;
        }

        void ActiveEditor_MouseWheel(object sender, MouseEventArgs e)
        {
            gv.ActiveEditor.MouseWheel -= ActiveEditor_MouseWheel;
            gv.CloseEditor();
            this.BindingContext[gc.DataSource].EndCurrentEdit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataRow r =gv.GetDataRow(gv.FocusedRowHandle);
            int x=dtM.Rows.IndexOf(r);
            DataRow dr = dtM.NewRow();
            dtM.Rows.InsertAt(dr, x + 1);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dtM = new DataTable();
            dtM.Columns.Add("菜单名称");
            dtM.Columns.Add("菜单类型");
            dtM.Columns.Add("菜单顺序");

            dtM.Columns.Add("dll全称");
            dtM.Columns.Add("窗体显示名称");
            dtM.Columns.Add("命名空间.窗体名称");
            dtM.Columns.Add("图标");
            dtM.Columns.Add("是否显示");
            dtM.Columns.Add("上级菜单");
            dtM.Columns.Add("是否分组");
            dtM.Columns.Add("层级");
            dtM.Columns.Add("权限");
            dtM.Columns.Add("备注");
            dtM.Columns.Add("窗体类型");
            dtM.TableName = "自定义菜单";

            gc.DataSource = dtM;

            dtM.ReadXml(Path.Combine(Application.StartupPath, @"自定义菜单.xml"));
            string ss = string.Format("select 权限类型,上级权限 from [功能权限权限组权限表] where 权限组='admin'");
   
            DataTable dt_authorization = CZMaster.MasterSQL.Get_DataTable(ss, CPublic.Var.strConn);
  
            button_new_make(dtM, XTC, barManager1, bar2, dt_authorization);
          
         
           
        
           // this.barManager1.MaxItemId = 12;
            // 
            // bar2
            // 
  
        }


    }
}
