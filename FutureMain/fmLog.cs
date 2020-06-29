using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
namespace FutureMain
{
    public partial class fmLog : Form
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        string file = Application.StartupPath + @"\FormLayout\username.txt";
        #endregion
        #region 自用类
        public fmLog()
        {
            InitializeComponent();
        }

        private void fmLog_Load(object sender, EventArgs e)
        {
           



            this.Text = "登入窗口";
            try
            {
             
                if (File.Exists(file) == true)                                     
                {
                    txt_用户名称.Text = System.IO.File.ReadAllText(file);
                    txt_用户密码.TabIndex = 0;
                }   
            }
            catch { }
//            //加载配置信息 查看当前机器是否是需要默认人员 登录打开默认界面
//            string pcname = System.Net.Dns.GetHostName(); //当前设备名称
//            string ss = string.Format(@"select 设备自动打开界面配置表.*,PWD from [设备自动打开界面配置表],人事基础员工表 
//                where  [设备自动打开界面配置表].登录ID=人事基础员工表.员工号 and   设备名称='{0}'", pcname);
//            using (SqlDataAdapter da = new SqlDataAdapter(ss, CPublic.Var.strConn))
//            {
//                DataTable t = new DataTable();
//                da.Fill(t);
//                if (t.Rows.Count > 0)
//                {
//                    txt_用户名称.Text= t.Rows[0]["登录ID"].ToString();
//                    txt_用户密码.Text = t.Rows[0]["PWD"].ToString();
                    
//                    button1_Click(null, null);
//                    Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", t.Rows[0]["程序集"].ToString())));  //  ERPproduct.dll
//                    Type outerForm = outerAsm.GetType(t.Rows[0]["打开界面ID"].ToString(), false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
//                    // Form ui = Activator.CreateInstance(outerForm) as Form;
//                    UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
//                    CPublic.UIcontrol.Showpage(ui, t.Rows[0]["打开界面名称"].ToString());
//                }
//            }
        }
        #endregion

        #region 方法
        private void fun_验证()
        {
            try
            {

                string onjob = " and 在职状态='在职'";
                if (txt_用户名称.Text.ToString().ToLower().Trim() == "admin")
                {
                    onjob = "";
                }
                string sql = string.Format("select * from 人事基础员工表 where  员工号 = '{0}' and PWD = '{1}' {2}", txt_用户名称.Text.ToString(), txt_用户密码.Text.ToString(),onjob);
                SqlDataAdapter daM = new SqlDataAdapter(sql, strconn);
                dtM = new DataTable();
                daM.Fill(dtM);
                if (dtM.Rows.Count > 0)
                {
                    CPublic.Var.LocalUserID = txt_用户名称.Text.ToString();
                    CPublic.Var.localUserName = dtM.Rows[0]["姓名"].ToString();
                    CPublic.Var.LocalUserTeam = dtM.Rows[0]["权限组"].ToString();
                    //CPublic.Var.localUser组织关系 = dtM.Rows[0]["组织关系"].ToString();
                    CPublic.Var.localUser部门编号 = dtM.Rows[0]["部门编号"].ToString();
                    CPublic.Var.localUser课室编号 = dtM.Rows[0]["课室编号"].ToString();
                    CPublic.Var.localUser工号简码 = dtM.Rows[0]["工号简码"].ToString();
                    CPublic.Var.localUser部门名称 = dtM.Rows[0]["部门"].ToString();
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    if (txt_用户密码.Text.ToString().Length <4 || txt_用户密码.Text.ToString() == "123456")
                    {
                        string s = "密码为起始密码须修改密码";
                        ERPorg.修改密码界面 fm = new ERPorg.修改密码界面(s);
                        CPublic.UIcontrol.Showpage(fm, "修改密码");
                    }
                }
                else
                {
                    throw new Exception("用户名或密码错误！");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("用户名或密码错误！");
            }
           
        }

        private void fun_Check()
        {
            if (txt_用户名称.Text == "")
            {
                txt_用户名称.Focus();
                throw new Exception("请填写用户名称！");
            }
            if (txt_用户密码.Text == "")
            {
                txt_用户密码.Focus();
                throw new Exception("请填写用户密码！");
            }
        }
        #endregion

        #region 界面操作
        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel ;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

              
                fun_Check();
                fun_验证();

                try
                {
                    //FutureMainFM fm = new FutureMainFM();
                    //fm.ShowDialog();
                  
                    string content = txt_用户名称.Text;
                    if (File.Exists(file) == true)
                    {
                        System.IO.File.WriteAllText(file,txt_用户名称.Text);
                    }
                    else
                    {
                        FileStream myFs = new FileStream(file, FileMode.Create);
                        StreamWriter mySw = new StreamWriter(myFs);
                        mySw.Write(content);    
                        mySw.Close();
                        myFs.Close();
                    }
 
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
    }
}
