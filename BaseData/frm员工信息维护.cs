using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CPublic;
using CZMaster;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.SqlClient;
using DevExpress.XtraPrinting;

namespace BaseData
{
    public partial class frm员工信息维护 : UserControl
    {

        #region  公有成员
        string strConn = "";
        string strConn_FS = "";
        string cfgfilepath = "";
        #endregion


        #region 私有成员

        /// <summary>
        /// 所有员工信息 主表
        /// </summary>
        DataTable dtM;

        /// <summary>
        /// 主表正在编辑的当前行
        /// </summary>
        DataRow drM;

        /// <summary>
        /// 属性表
        /// </summary>
        DataTable dt_属性;

        /// <summary>
        /// 人事基础员工文件
        /// </summary>
        DataTable dtP;

        /// <summary>
        /// 部门表
        /// </summary>
        DataTable dt_部门;
        DataTable dt_班组;
        DataTable dt_岗位;

        /// <summary>
        /// 接收列
        /// </summary>
        ArrayList arylie;

        /// <summary>
        /// 记录员工号
        /// </summary>
        string strygh = "";

        /// <summary>
        /// 权限的记录
        /// </summary>
        string strqx = "";

        /// <summary>
        /// 出错记录的字符串
        /// </summary>
        string strerror = "";

        /// <summary>
        /// 虚拟表，显示员工文件的上传与下载信息
        /// </summary>
        DataTable dt1;

        CurrencyManager cmM;

        #endregion


        #region  类加载

        string strcon_yg = "";

        public System.Windows.Forms.Form fmm = null;

        public frm员工信息维护()
        {
            InitializeComponent();
            strcon_yg = CPublic.Var.strConn;
            strConn_FS = CPublic.Var.geConn("FS");
            string permgroup = CPublic.Var.LocalUserTeam;
            string s = string.Format("select * from [权限组按钮权限表] where 权限组='{0}' and 权限类型='员工信息维护' ", permgroup);
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon_yg);
            foreach (object item in barManager1.Items)
            {
                if (item.GetType() == typeof(DevExpress.XtraBars.BarLargeButtonItem))
                {
                    DevExpress.XtraBars.BarLargeButtonItem x = item as DevExpress.XtraBars.BarLargeButtonItem;
                    x.Enabled = ERPorg.Corg.btn_perm(t, x.Caption);

                }
                //item.Enabled = ERPorg.Corg.btn_perm(t,item.Caption);

            }



        }


        private void frm员工信息维护_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name)))
                {

                    gvM.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }
                da_日期_前.EditValue = Convert.ToDateTime(CPublic.Var.getDatetime().AddDays(-7).ToString("yyyy-MM-dd"));
                da_日期_后.EditValue = Convert.ToDateTime(CPublic.Var.getDatetime());
                //devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
                //devGridControlCustom1.strConn = CPublic.Var.strConn;

                fun_下拉框处理();
                fun_主数据载入();
                fun_文件初始();  //员工相关文件
                //this.barEditItem1.Edit.KeyDown += Edit_KeyDown;   //查询框加一个回车事件，按下回车执行查询
                fun_合同到期提醒();
                //gvM.Columns["PWD"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        #endregion


        #region  其他数据处理

        /// <summary>
        /// 下拉框的数据的处理
        /// </summary>
        private void fun_下拉框处理()
        {
            try
            {
                cb_职工种类.Properties.Items.Clear();
                ygxz.Properties.Items.Clear();
                gszhicheng.Properties.Items.Clear();
                gjzhicheng.Properties.Items.Clear();
                zhiwu.Properties.Items.Clear();
                zhibie.Properties.Items.Clear();
                //renshiguanxi.Properties.Items.Clear();
                xueli.Properties.Items.Clear();
                zaizhizhuangtai.Properties.Items.Clear();
                bumen.Properties.Items.Clear();
                cb_课室.Properties.Items.Clear();
                txt_短袖尺寸.Properties.Items.Clear();
                txt_工作鞋尺寸.Properties.Items.Clear();
                txt_外套尺寸.Properties.Items.Clear();
                txt_长袖尺寸.Properties.Items.Clear();
                //属性表的下拉框
                string sql = "select * from 基础数据基础属性表 order by 属性类别,POS";
                dt_属性 = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                foreach (DataRow r in dt_属性.Rows)
                {
                    if (r["属性类别"].ToString().Equals("员工性质"))   //员工性质
                    {
                        ygxz.Properties.Items.Add(r["属性值"].ToString());
                    }
                    if (r["属性类别"].ToString().Equals("职工种类"))   //
                    {
                        cb_职工种类.Properties.Items.Add(r["属性值"].ToString());
                    }
                    //if (r["属性类别"].ToString().Equals("岗位"))   //岗位
                    //{
                    //    gangwei.Properties.Items.Add(r["属性值"].ToString());
                    //}

                    if (r["属性类别"].ToString().Equals("公司职称"))   //公司职称
                    {
                        gszhicheng.Properties.Items.Add(r["属性值"].ToString());
                    }

                    if (r["属性类别"].ToString().Equals("国家职称"))    //国家职称
                    {
                        gjzhicheng.Properties.Items.Add(r["属性值"].ToString());
                    }

                    if (r["属性类别"].ToString().Equals("职务"))      //职务
                    {
                        zhiwu.Properties.Items.Add(r["属性值"].ToString());
                    }

                    if (r["属性类别"].ToString().Equals("职别"))     //职别
                    {
                        zhibie.Properties.Items.Add(r["属性值"].ToString());
                    }

                    //if (r["属性类别"].ToString().Equals("人事关系"))    //人事关系
                    //{
                    //    renshiguanxi.Properties.Items.Add(r["属性值"].ToString());
                    //}

                    if (r["属性类别"].ToString().Equals("学历"))      //学历
                    {
                        xueli.Properties.Items.Add(r["属性值"].ToString());
                    }

                    if (r["属性类别"].ToString().Equals("在职状态"))
                    {
                        zaizhizhuangtai.Properties.Items.Add(r["属性值"].ToString());
                    }

                    //if (r["属性类别"].ToString().Equals("课室"))
                    //{
                    //    cb_课室.Properties.Items.Add(r["属性值"].ToString());
                    //}

                    if (r["属性类别"].ToString().Equals("短袖尺寸"))
                    {
                        txt_短袖尺寸.Properties.Items.Add(r["属性值"].ToString());
                    }

                    if (r["属性类别"].ToString().Equals("工作鞋尺寸"))
                    {
                        txt_工作鞋尺寸.Properties.Items.Add(r["属性值"].ToString());
                    }

                    if (r["属性类别"].ToString().Equals("外套尺寸"))
                    {
                        txt_外套尺寸.Properties.Items.Add(r["属性值"].ToString());
                    }

                    if (r["属性类别"].ToString().Equals("长袖尺寸"))
                    {
                        txt_长袖尺寸.Properties.Items.Add(r["属性值"].ToString());
                    }

                    if (r["属性类别"].ToString().Equals("社保"))
                    {
                        comboBox1.Items.Add(r["属性值"].ToString());
                    }
                    if (r["属性类别"].ToString().Equals("在职性质"))
                    {
                        comboBoxEdit1.Properties.Items.Add(r["属性值"].ToString());

                    }



                }
                //部门表的下拉框
                dt_部门 = MasterSQL.Get_DataTable("select 部门名称 from 人事基础部门表 where len(部门编号)=8 order by 部门名称 ", CPublic.Var.strConn);
                foreach (DataRow r in dt_部门.Rows)
                {
                    bumen.Properties.Items.Add(r["部门名称"].ToString());
                }

                //                string ss= @"select  部门名称 from 人事基础部门表 where 部门GUID not in (
                //                            select  上级部门  from 人事基础部门表) order by 部门名称";
                string ss = "select 部门名称 from 人事基础部门表 where len(部门编号)=10 order by 部门名称 ";
                DataTable dt_课室 = MasterSQL.Get_DataTable(ss, CPublic.Var.strConn);
                foreach (DataRow r in dt_课室.Rows)
                {
                    cb_课室.Properties.Items.Add(r["部门名称"].ToString());
                }

                string sql_班组 = "select  属性字段1 as 班组编号,属性值 as 班组  from 基础数据基础属性表 where 属性类别='班组'";
                dt_班组 = new DataTable();
                dt_班组 = CZMaster.MasterSQL.Get_DataTable(sql_班组, CPublic.Var.strConn);
                sl_班组编号.Properties.DataSource = dt_班组;
                sl_班组编号.Properties.DisplayMember = "班组编号";
                sl_班组编号.Properties.ValueMember = "班组编号";
                string sql_岗位 = "select  属性字段1 as 岗位编号,属性值 as 岗位  from 基础数据基础属性表 where 属性类别='岗位'";
                dt_岗位 = new DataTable();
                dt_岗位 = CZMaster.MasterSQL.Get_DataTable(sql_岗位, CPublic.Var.strConn);
                sl_岗位编号.Properties.DataSource = dt_岗位;
                sl_岗位编号.Properties.DisplayMember = "岗位编号";
                sl_岗位编号.Properties.ValueMember = "岗位编号";

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " 员工信息维护 fun_下拉框处理");
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 输入数据合法性的检查
        /// </summary>
        private void fun_Check()
        {
            //工号不能为空
            if (gonghao.Text == "")
            {
                gonghao.Focus();
                throw new Exception("员工工号不能为空");
            }
            //工号不能重复,在新增情况下
            if (strygh == "")
            {
                DataRow[] dr = dtM.Select(string.Format("员工号='{0}' ", gonghao.Text));
                if (dr.Length > 0)
                {
                    gonghao.Focus();
                    throw new Exception("员工工号重复了，请检查！");
                }
            }
            //姓名不能为空
            if (xingming.Text == "")
            {
                xingming.Focus();
                throw new Exception("姓名不能为空，请检查！");
            }
            //性别不能够为空
            if (sex.Text == "")
            {
                sex.Focus();
                throw new Exception("性别不能为空，请检查！");
            }
            //如果出生日期不写
            if (birthday.Text == "")
            {
                birthday.EditValue = DBNull.Value;
            }

            Int64 i = 0;
            //入职年月如果是空值
            if (ruzhiriqi.Text == "")
            {
                ruzhiriqi.EditValue = DBNull.Value;
            }

            //手机号码只能是11位，只能数字
            if (shouji.Text != "")
            {
                if (shouji.Text.Length != 11)
                {
                    shouji.Focus();
                    throw new Exception("手机号码是11位，请检查！");
                }

                try
                {
                    i = Convert.ToInt64(shouji.Text);
                }
                catch
                {
                    shouji.Focus();
                    throw new Exception("手机号只能输入数字，请检查！");
                }
            }

            //紧急电话只能为数字
            if (jinjidianhua.Text != "")
            {
                try
                {
                    i = Convert.ToInt64(jinjidianhua.Text);
                }
                catch
                {
                    jinjidianhua.Focus();
                    throw new Exception("紧急电话只能输入数字，请检查！");
                }
            }

            //宅电也只能为数字
            //if (zhaidian.Text != "")
            //{
            //    try
            //    {
            //        i = Convert.ToInt64(zhaidian.Text);
            //    }
            //    catch
            //    {
            //        zhaidian.Focus();
            //        throw new Exception("宅电只能输入数字，请检查！");
            //    }
            //}

            //邮箱格式的验证
            if (email.Text != "")
            {
                //Regex reg = new Regex("^([a-z0-9A-Z]+[-|\\.]?)+[a-z0-9A-Z]@([a-z0-9A-Z]+(-[a-z0-9A-Z]+)?\\.)+[a-zA-Z]{2,}$");
                Regex reg = new Regex("^/w+([-+.]/w+)*@/w+([-.]/w+)*/./w+([-.]/w+)*$");
                if (!reg.IsMatch(email.Text))
                {
                    //   zhaidian.Focus();
                    throw new Exception("邮箱格式不正确，请检查！");
                }
            }

            //身份证号码的验证
            if (shenfenzheng.Text.Length != 18)
            {
                shenfenzheng.Focus();
                throw new Exception("身份证号码为18位,请检查！");
            }

            try
            {
                i = Convert.ToInt64(shenfenzheng.Text.Substring(0, 17));
                if (shenfenzheng.Text.Substring(17, 1) != "X")  //如果最后一位不是输入X
                {
                    i = Convert.ToInt64(shenfenzheng.Text.Substring(17, 1));
                }
            }
            catch
            {
                shenfenzheng.Focus();
                throw new Exception("身份证号前17位输入数字，最后一位为X或者是数字，请检查！");
            }

            // 在职状态
            if (zaizhizhuangtai.Text == "离职")   //如果选择离职的话 离职时间是必须要填写的
            {
                if (lizhitime.Text == "")
                {
                    lizhitime.Focus();
                    throw new Exception("选择离职状态后，离职时间不能够为空！");
                }
            }
            else
            {
                lizhitime.EditValue = DBNull.Value;
            }

            //毕业日期可以填写
            if (biyeriqi.Text == "")
            {
                biyeriqi.EditValue = DBNull.Value;
            }

            //合同到期日期可以填写
            if (hetongendtime.Text == "")
            {
                hetongendtime.EditValue = DBNull.Value;
            }

            // 入股时间
            //if (rugutime.Text == "")
            //{
            //    rugutime.EditValue = DBNull.Value;
            //}

            //入股数
            //if (rugushu.Text == "")
            //{
            //    //drM["入股数"] = 0;
            //    rugushu.Text = Convert.ToString(0);
            //}

            //年龄的计算
            if (shenfenzheng.Text != "")
            {
                nianling.Text = Convert.ToString(drM["年龄"] = DateTime.Now.Year - Convert.ToInt32(shenfenzheng.Text.Substring(6, 4)));
                try
                {
                    birthday.EditValue = Convert.ToDateTime(shenfenzheng.Text.Substring(6, 4) + "-" + shenfenzheng.Text.Substring(10, 2) + "-" + shenfenzheng.Text.Substring(12, 2));
                }
                catch
                {
                    throw new Exception("请检查身份证号是否填写正确！");
                }
            }
            else
            {
                nianling.Text = Convert.ToString(0);
                //drM["年龄"] = 0;
            }

            //工龄的计算
            if (ruzhiriqi.Text != "")
            {
                gongling.Text = Convert.ToString(drM["工龄"] = DateTime.Now.Year - ((DateTime)ruzhiriqi.EditValue).Year);
            }
            else
            {
                gongling.Text = Convert.ToString(0);
                //drM["工龄"] = 0;
            }
            if (txt_领用件数鞋.Text == "")
            {
                txt_领用件数鞋.Text = "0";
            }
            if (txt_领用件数长袖.Text == "")
            {
                txt_领用件数长袖.Text = "0";
            }
            if (txt_领用件数短袖.Text == "")
            {
                txt_领用件数短袖.Text = "0";
            }
            if (txt_领用件数外套.Text == "")
            {
                txt_领用件数外套.Text = "0";
            }

            if (txt_长袖尺寸.Text == "")
            {
                txt_长袖尺寸.Text = "";
            }
            if (txt_短袖尺寸.Text == "")
            {
                txt_短袖尺寸.Text = "";
            }
            if (txt_外套尺寸.Text == "")
            {
                txt_外套尺寸.Text = "";
            }
            if (txt_工作鞋尺寸.Text == "")
            {
                txt_工作鞋尺寸.Text = "";
            }

            if (txt第一次外套时间.Text == "")
            {
                txt第一次外套时间.EditValue = DBNull.Value;
            }
            if (txt第二次外套时间.Text == "")
            {
                txt第二次外套时间.EditValue = DBNull.Value;
            }
            if (txt第一次鞋时间.Text == "")
            {
                txt第一次鞋时间.EditValue = DBNull.Value;
            }
            if (txt第二次鞋时间.Text == "")
            {
                txt第二次鞋时间.EditValue = DBNull.Value;
            }
            if (txt第一次长袖时间.Text == "")
            {
                txt第一次长袖时间.EditValue = DBNull.Value;
            }
            if (txt第二次长袖时间.Text == "")
            {
                txt第二次长袖时间.EditValue = DBNull.Value;
            }
            if (txt第一次短袖时间.Text == "")
            {
                txt第一次短袖时间.EditValue = DBNull.Value;
            }
            if (txt第二次短袖时间.Text == "")
            {
                txt第二次短袖时间.EditValue = DBNull.Value;
            }
            if (hetongstarttime.Text == "")
            {
                hetongstarttime.EditValue = DBNull.Value;
            }
            if (hetongendtime.Text == "")
            {
                hetongendtime.EditValue = DBNull.Value;
            }

            if (cb_课室.Text == "")
            {
                cb_课室.EditValue = "";
            }
            if (drM["员工GUID"] != DBNull.Value)
            {
                string sql = @"select *,case when([是否特岗]='true') then '是' else  '否' end as 特岗,DATEDIFF(dd, 出生年月, getdate())/365 as 年龄
                                                from 人事基础员工表 ";
                dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                DataRow[] dr1 = dtM.Select(string.Format("员工号='{0}'", gonghao.Text));
                if (dr1.Length > 0)
                {
                    drM = dr1[0];

                }
            }


            if (cb_课室.EditValue != null)
            {
                string sql = string.Format("select *  from [人事基础部门表] where 部门名称='{0}'", cb_课室.EditValue.ToString().Trim());
                using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        textBox3.Text = dt.Rows[0]["部门编号"].ToString();
                    }
                }
            }

            if (bumen.EditValue != null)
            {
                string sql = string.Format("select *  from [人事基础部门表] where 部门名称='{0}'", bumen.EditValue.ToString().Trim());
                using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        textBox4.Text = dt.Rows[0]["部门编号"].ToString();
                    }
                }
            }


            dataBindHelper1.DataToDR(drM);   //check无误之后就直接会写到drM








        }

        /// <summary>
        /// 窗口载入时，相关员工文件的dt初始状态
        /// </summary>
        private void fun_文件初始()
        {
            try
            {


                dt1 = new DataTable();
                dt1.Columns.Add("文件名称");
                dt1.Columns.Add("是否已上传", true.GetType());
                DataRow[] dr = dt_属性.Select("属性类别='员工文件类型'");
                foreach (DataRow r in dr)
                {
                    dt1.Rows.Add(r["属性值"].ToString());
                }
                cmM = this.BindingContext[dt1] as CurrencyManager;
                gcM1.DataSource = dt1;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_文件初始");
                throw new Exception(ex.Message);
            }
        }
        private void fun_合同到期提醒()
        {
            if (CPublic.Var.LocalUserID == "8614") // 
            {
                string str = "";

                DataView dv = new DataView(dtM);
                dv.RowFilter = "在职状态='在职'";
                foreach (DataRow dr in dv.ToTable().Rows)
                {
                    if (dr["合同到期日期"].ToString() == "") continue;
                    DateTime dtme = Convert.ToDateTime(dr["合同到期日期"]);
                    DateTime today = CPublic.Var.getDatetime();
                    TimeSpan dtime = dtme - today;
                    if (dtime.Days <= 15)
                    {
                        str = str + dr["员工号"] + "" + dr["姓名"] + "" + dr["课室"] + "\n";
                    }
                }
                if (str != "")
                    MessageBox.Show(str, "到期或即将到期");
            }

        }
        #endregion


        #region  数据库的读取与保存

        private void fun_单个刷(string str)
        {
            string sql = string.Format(@"select *,case when ([是否特岗]='true') then '是' else  '否' end as 特岗,DATEDIFF(dd, 出生年月, getdate())/365 as 年龄
                                                from 人事基础员工表 where 员工号='{0}'", str);
            DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql, CPublic.Var.strConn);
            DataRow[] r = dtM.Select(string.Format("员工号='{0}'", str));
            r[0].ItemArray = dr.ItemArray;
            r[0].AcceptChanges();
        }
        /// <summary>
        /// 数据读取主数据载入的方法（所有的员工信息）
        /// </summary>
        private void fun_主数据载入()
        {
            string sql = @"select *,case when([是否特岗]='true') then '是' else  '否' end as 特岗,DATEDIFF(dd, 出生年月,getdate())/365 as 年龄  from 人事基础员工表 ";
            dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
            gcM.DataSource = dtM;
            drM = dtM.NewRow();



            //先把列全部隐藏不可见
            // foreach (DataColumn d in dtM.Columns)
            //{
            //   gvM.Columns[d.ColumnName].Visible = false;
            //}

            //读取存储在本地的列信息

            //arylie = new ArrayList(LocalDataSettingBIN.getLocalData("basedatalie"));
            //for (int j = 0; j < arylie.Count; j++)
            //{
            //     for (int i = 0; i < dtM.Columns.Count; i++)
            //    {
            //        if (arylie[j].ToString().Equals(gvM.Columns[i].ToString()))
            //       {
            //                gvM.Columns[i].Visible = true;
            //        }
            //     }
            //}                                  
        }


        /// <summary>
        /// 员工基础信息的查询（刷新）
        /// </summary>
        private void fun_刷新()
        {
            if (barEditItem1.EditValue == null)    //如果为空
            {
                throw new Exception("请输入要查询的工号！");
            }
            //查询某个员工的相关文件：后加载
            string sql = "select * from 人事基础员工文件表 where 员工号='" + barEditItem1.EditValue.ToString() + "'";
            dtP = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
            //MasterFileService.strWSDL = CPublic.Var.strWSConn;
            //先载入员工的信息

            DataRow[] dr = dtM.Select(string.Format("员工号='{0}' ", barEditItem1.EditValue.ToString()));
            if (dr.Length > 0)
            {
                drM = dr[0];
                dataBindHelper1.DataFormDR(dr[0]);   //写到界面
                nianling.Text = drM["年龄"].ToString();
                strygh = dr[0]["员工号"].ToString();  //查询出来之后用一个全局变量记住工号
                comboBox3.Text = dr[0]["厂区"].ToString();
                gonghao.Enabled = false;   //工号是不能修改的
                //shenfenzheng.Enabled = false;    //身份证号也是不能改的
                xtraTabControl1.SelectedTabPage = xtraTabPage1;//查询成功之后进行切换
            }
            else
            {
                throw new Exception("查不到该员工的数据！");
            }

            DataRow[] pic = dtP.Select("文件名称='照片'");
            if (pic.Length > 0)
            {
                string dir = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\tmpview";
                //如果该文件夹存在的话，删除。
                try
                {
                    System.IO.Directory.Delete(dir, true);
                }
                catch
                {
                }
                //删除之后进行新增
                try
                {
                    System.IO.Directory.CreateDirectory(dir);
                }
                catch
                {

                }
                fun_文件下载(dir + "\\" + pic[0]["上传文件全名"].ToString(), pic[0]);
                byte[] bs = System.IO.File.ReadAllBytes(dir + "\\" + pic[0]["上传文件全名"].ToString());
                Image img = Image.FromStream(new System.IO.MemoryStream(bs));
                pictureEdit1.Image = img;
            }
            else
            {
                pictureEdit1.Image = null;
            }
            ////最后加载员工的照片
            //DataRow[] pic = dtP.Select("文件名称='照片'");
            //if (pic.Length > 0)
            //{
            //    byte[] bs = MasterFileService.BOLBDownLoad(pic[0]["文件GUID"].ToString());
            //    Image img = Image.FromStream(new System.IO.MemoryStream(bs));
            //    pictureEdit1.Image = img;
            //}
            //else
            //{
            //    pictureEdit1.Image = null;
            //}

            fun_文件刷新();  //员工相关文件
        }

        /// <summary>
        /// 文件的刷新
        /// </summary>
        private void fun_文件刷新()
        {
            //查询的时候，先清空一下，可以保留属性表中的文件类型
            dt1.Clear();
            DataRow[] drfile = dt_属性.Select("属性类别='员工文件类型'");
            foreach (DataRow r in drfile)
            {
                dt1.Rows.Add(r["属性值"].ToString());
            }
            //把查出的员工相关文件，一一与dt1中的文件名称进行比对，如果有这个文件只要打个勾表示已上传，如果没有这个文件请加进去
            foreach (DataRow r in dtP.Rows)
            {
                if (dt1.Columns.IndexOf("上传文件全名") < 0)
                {
                    dt1.Columns.Add("上传文件全名");
                }
                if (dt1.Columns.IndexOf("文件GUID") < 0)
                {
                    dt1.Columns.Add("文件GUID");
                }
                DataRow[] drr = dt1.Select(string.Format("文件名称='{0}'", r["文件名称"].ToString()));
                if (drr.Length > 0)    //存在
                {
                    drr[0]["是否已上传"] = true;
                    drr[0]["上传文件全名"] = r["上传文件全名"];
                    drr[0]["文件GUID"] = r["文件GUID"];
                }
                else
                {
                    dt1.Rows.Add(r["文件名称"], false, r["上传文件全名"], r["文件GUID"].ToString());
                }
                //else       //不存在
                //{



                //}
            }
        }

        /// <summary>
        /// 新增的方法
        /// </summary>
        private void fun_新增()
        {
            drM = dtM.NewRow();   //新增一个NewRow s还不属于dtM

            dataBindHelper1.DataFormDR(drM);
            gonghao.Enabled = true;  //新增信息的时候 工号和身份证号不锁住
            shenfenzheng.Enabled = true;

            barEditItem1.EditValue = "";
            strygh = "";

            nianling.Text = ""; //年龄
            gongling.Text = ""; //工龄
            shebaohao.Text = "";  //社保号
            gjjhao.Text = "";
            nianling.Text = ""; //年龄
            gongling.Text = ""; //工龄
            birthday.Text = "";  //出生日期
            ruzhiriqi.Text = "";  //入职日期
            biyeriqi.Text = "";   //毕业日期
            lizhitime.Text = "";   //离职时间
            hetongendtime.Text = "";    //合同到期日期
            hetongstarttime.Text = "";
            txt第二次短袖时间.Text = "";
            txt第二次外套时间.Text = "";
            txt第二次鞋时间.Text = "";
            txt第二次长袖时间.Text = "";
            txt第一次短袖时间.Text = "";
            txt第一次外套时间.Text = "";
            txt第一次鞋时间.Text = "";
            txt第一次长袖时间.Text = "";
            //rugutime.Text = "";     //入股时间
            fun_文件初始();  //相关文件dt恢复成初始
        }

        private void fun_清空()
        {
            DataRow r = dtM.NewRow();   //新增一个NewRow s还不属于dtM

            dataBindHelper1.DataFormDR(r);

            gonghao.Enabled = true;  //新增信息的时候 工号和身份证号不锁住
            shenfenzheng.Enabled = true;
            //barEditItem1.EditValue = "";
            strygh = "";
            nianling.Text = ""; //年龄
            gongling.Text = ""; //工龄
            shebaohao.Text = "";  //社保号
            gjjhao.Text = "";
            nianling.Text = ""; //年龄
            gongling.Text = ""; //工龄
            birthday.Text = "";  //出生日期
            ruzhiriqi.Text = "";  //入职日期
            biyeriqi.Text = "";   //毕业日期
            lizhitime.Text = "";   //离职时间
            hetongendtime.Text = "";    //合同到期日期
            hetongstarttime.Text = "";
            txt第二次短袖时间.Text = "";
            txt第二次外套时间.Text = "";
            txt第二次鞋时间.Text = "";
            txt第二次长袖时间.Text = "";
            txt第一次短袖时间.Text = "";
            txt第一次外套时间.Text = "";
            txt第一次鞋时间.Text = "";
            txt第一次长袖时间.Text = "";

            //rugutime.Text = "";     //入股时间

            pictureEdit1.EditValue = "";
        }

        /// <summary>
        /// 员工信息删除的方法
        /// </summary>
        private void fun_删除()
        {
            drM.Delete();
            MasterSQL.Save_DataTable(dtM, "人事基础员工表", CPublic.Var.strConn);
            //删除该员工相关的所有文件
            DataRow[] dr = dtP.Select(string.Format("员工号='{0}'", strygh));
            foreach (DataRow r in dr)
            {
                r.Delete();
            }
            MasterSQL.Save_DataTable(dtP, "人事基础员工文件表", CPublic.Var.strConn);
            //删除成功之后，清空
            fun_新增();
        }



        /// <summary>
        /// 员工信息的保存的方法
        /// </summary>
        private void fun_保存()
        {
            //如果员工GUID为空话，说明是新增。不为空的话说明是修改的情况
            if (drM["员工GUID"] == DBNull.Value)
            {
                drM["员工GUID"] = System.Guid.NewGuid().ToString();
                drM["课室编号"] = textBox3.Text;
                drM["部门编号"] = textBox4.Text;

                dtM.Rows.Add(drM);
            }

            MasterSQL.Save_DataTable(dtM, "人事基础员工表", CPublic.Var.strConn);

            gonghao.Enabled = false;   //保存之后锁住工号和身份证号
            //shenfenzheng.Enabled = false;

            strygh = gonghao.Text;   //保存后记住工号，以便随时删除
        }

        /// <summary>
        /// 文件上传的方法
        /// </summary>
        private void fun_文件上传(string pathName, DataRow r)
        {
            FileInfo info = new FileInfo(pathName);      //判定上传文件的大小
            long maxlength = info.Length;
            if (maxlength > 1024 * 1024 * 8)
            {
                throw new Exception("上传的文件不能超过1M，请重新选择后再上传！");
            }

            MasterFileService.strWSDL = CPublic.Var.strWSConn;
            CFileTransmission.CFileClient.strCONN = strConn_FS;
            string strguid = "";  //记录系统自动返回的GUID

            strguid = CFileTransmission.CFileClient.sendFile(pathName);

            dtP.Rows.Add(strygh, r["文件名称"].ToString(), strguid, Path.GetFileName(pathName));
            MasterSQL.Save_DataTable(dtP, "人事基础员工文件表", CPublic.Var.strConn);
        }

        /// <summary>
        /// 文件下载的方法
        /// </summary>
        private void fun_文件下载(string pathName, DataRow r)
        {


            CFileTransmission.CFileClient.strCONN = strConn_FS;
            CFileTransmission.CFileClient.Receiver(r["文件GUID"].ToString(), pathName);

        }

        private void fun_文件查看()
        {
            try
            {
                DataRow r = (this.BindingContext[dt1].Current as DataRowView).Row;
                if (r["是否已上传"].Equals(true))
                {
                    string dir = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\tmpview";
                    //如果该文件夹存在的话，删除。
                    try
                    {
                        System.IO.Directory.Delete(dir, true);
                    }
                    catch { }
                    //删除之后进行新增
                    try
                    {
                        System.IO.Directory.CreateDirectory(dir);
                    }
                    catch { }
                    fun_文件下载(dir + "\\" + r["上传文件全名"].ToString(), r);
                    System.Diagnostics.Process.Start(dir + "\\" + r["上传文件全名"].ToString());
                }
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_文件查看");
                throw ex;
            }
        }

        /// <summary>
        /// 照片的上传
        /// </summary>
        private void fun_上传照片(string pathName)
        {
            try
            {
                FileInfo info = new FileInfo(pathName);
                long maxinfo = info.Length;
                if (maxinfo > 1024 * 1024 * 8)
                {
                    throw new Exception("上传的照片不能超过1M，请重新选择上传！");
                }
                CFileTransmission.CFileClient.strCONN = strConn_FS;
                MasterFileService.strWSDL = CPublic.Var.strWSConn;
                string strguid = "";
                strguid = CFileTransmission.CFileClient.sendFile(pathName);

                //strguid = MasterFileService.BOLBUpload(System.IO.File.ReadAllBytes(pathName)); //上传
                dtP.Rows.Add(strygh, "照片", strguid, Path.GetFileName(pathName));
                MasterSQL.Save_DataTable(dtP, "人事基础员工文件表", CPublic.Var.strConn);
                byte[] bs = System.IO.File.ReadAllBytes(pathName);
                //byte[] bs = MasterFileService.BOLBDownLoad(strguid);
                Image img = Image.FromStream(new System.IO.MemoryStream(bs));
                pictureEdit1.Image = img;
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " ");
                throw ex;
            }
        }



        /// <summary>
        /// 删除文件的方法
        /// </summary>
        private void fun_文件删除(DataRow r)
        {
            r["是否已上传"] = false;
            DataRow[] dr = dtP.Select(string.Format("员工号='{0}' and 文件名称='{1}'", strygh, r["文件名称"].ToString()));
            CFileTransmission.CFileClient.deleteFile(r["文件GUID"].ToString());
            dr[0].Delete();
            MasterSQL.Save_DataTable(dtP, "人事基础员工文件表", CPublic.Var.strConn);
        }




        #endregion


        #region  界面操作

        /// <summary>
        /// 回车后查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Edit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    fun_刷新();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        /// <summary>
        /// 界面的刷新操作（也是员工信息的查询操作）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_清空();
                //fun_刷新();
                fun_主数据载入();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        /// <summary>
        /// 员工信息的新增操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_新增();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// 员工信息的删除操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (strygh == "")
            {
                MessageBox.Show("请查询需要删除员工的信息");
                return;
            }
            else
            {
                try
                {
                    if (MessageBox.Show(string.Format("你确定要删除工号\"{0}\"的员工信息吗？", strygh), "提示？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        fun_删除();
                        MessageBox.Show("删除成功！");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        /// <summary>
        /// 员工信息的的保存操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                fun_Check();
                fun_保存();
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        /// <summary>
        /// 选择需要查看的列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            fm员工信息列 fm = new fm员工信息列();
            fm.ShowDialog();
            //先全部隐藏
            foreach (DataColumn d in dtM.Columns)
            {
                gvM.Columns[d.ColumnName].Visible = false;
            }
            //把对应的显示出来
            for (int i = 0; i < fm.arry.Count; i++)
            {
                foreach (DataColumn d in dtM.Columns)
                {
                    if (d.ColumnName.Equals(fm.arry[i].ToString()))
                    {
                        gvM.Columns[d.ColumnName].Visible = true;
                    }
                }
            }
        }


        /// <summary>
        /// 默认显示列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            //gvM.Columns["员工GUID"].Visible = false;
            for (int x = 1; x < 9; x++)
            {
                gvM.Columns[x].Visible = true;
            }

            for (int x = 9; x < 34; x++)
            {
                gvM.Columns[x].Visible = false;
            }
        }


        /// <summary>
        /// 员工相关文件类型的新增操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            try
            {
                if (strygh == "")
                {
                    MessageBox.Show("请查询相关员工信息，再新增文件！");
                    fun_文件初始();
                }
                else
                {
                    cmM.AddNew();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// 员工相关文件的上传操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton5_Click(object sender, EventArgs e)
        {
            try
            {
                if (strygh == "")
                {
                    MessageBox.Show("请查询相关员工信息，再上传文件！");
                    fun_文件初始();
                }
                else
                {
                    DataRow r = (this.BindingContext[dt1].Current as DataRowView).Row;
                    if (r["文件名称"].ToString() == "")
                    {
                        throw new Exception("文件名称为空，无法上传，请检查！");
                    }
                    if (r["是否已上传"].Equals(true))
                    {
                        throw new Exception("该文件已存在，如需上传，请先删除！");
                    }
                    OpenFileDialog open = new OpenFileDialog();
                    if (open.ShowDialog() == DialogResult.OK)
                    {
                        fun_文件上传(open.FileName, r);
                        fun_文件刷新();
                        MessageBox.Show("文件上传成功！");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        /// <summary>
        /// 员工相关文件文件下载操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton6_Click(object sender, EventArgs e)
        {
            try
            {
                if (strygh == "")
                {
                    MessageBox.Show("请查询相关员工信息，再下载文件！");
                    fun_文件初始();
                }
                else
                {
                    DataRow r = (this.BindingContext[dt1].Current as DataRowView).Row;
                    if (!r["是否已上传"].Equals(true))
                    {
                        throw new Exception(string.Format("文件\"{0}\"不存在，无法下载！", r["文件名称"].ToString()));
                    }
                    else
                    {
                        SaveFileDialog save = new SaveFileDialog();
                        save.FileName = r["上传文件全名"].ToString();
                        //save.Filter = "图片文件(*.jpg,*.gif,*.bmp)|*.jpg;*.gif;*.bmp|文本文件(*.txt)|*.txt|word文件(*.doc,*.docx)|*.doc;*.docx"; //保存类型
                        if (save.ShowDialog() == DialogResult.OK)
                        {
                            fun_文件下载(save.FileName, r);
                            MessageBox.Show("文件下载成功！");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 员工相关文件的删除操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton7_Click(object sender, EventArgs e)
        {
            try
            {
                if (strygh == "")
                {
                    MessageBox.Show("请查询相关员工信息，再删除文件！");
                    fun_文件初始();
                }
                else
                {
                    DataRow r = (this.BindingContext[dt1].Current as DataRowView).Row;
                    if (!r["是否已上传"].Equals(true))
                    {
                        throw new Exception("该文件不存在，无法删除！");
                    }
                    else
                    {
                        if (MessageBox.Show(string.Format("你确定要删除员工\"{0}\"的\"{1}\"文件吗？", strygh, r["文件名称"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            fun_文件删除(r);
                            MessageBox.Show("文件删除成功！");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// 界面上传照片的操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (strygh == "")
                {
                    throw new Exception("请先查询相关员工的信息，再上传照片！");
                }
                else
                {
                    //查询该员工是否有照片
                    DataRow[] dr = dtP.Select(string.Format("员工号='{0}' and 文件名称='照片'", strygh));
                    if (dr.Length > 0)
                    {
                        throw new Exception("照片文件已有，如需重新上传，请先删除照片文件！");
                    }
                    else
                    {
                        OpenFileDialog openpic = new OpenFileDialog();
                        if (openpic.ShowDialog() == DialogResult.OK)
                        {
                            fun_上传照片(openpic.FileName);
                            fun_文件刷新();  //相关文件需要显示照片已上传
                            MessageBox.Show("照片上传成功！");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                // MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 相关文件的刷新操作（未查询员工信息时的刷新，查询员工信息后的刷新）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton8_Click(object sender, EventArgs e)
        {
            try
            {
                if (strygh == "")
                {
                    fun_文件初始();
                }
                else
                {
                    fun_文件刷新();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion



        //查询信息:通过gridcontrol列表右键查询详细信息
        private void 查询信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtM.Rows.Count > 0)
                {
                    fun_清空();
                    //DataRow r = (this.BindingContext[dtM].Current as DataRowView).Row;
                    DataRow r = gvM.GetDataRow(gvM.FocusedRowHandle);
                    barEditItem1.EditValue = r["员工号"].ToString();
                    fun_单个刷(r["员工号"].ToString());
                    fun_刷新();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //直接打开文件
        private void 文件打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                fun_文件查看();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gvM_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        //导出
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (xtraTabControl1.SelectedTabPage == 人员基础分析)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {

                    DevExpress.XtraPrinting.XlsxExportOptions options = new XlsxExportOptions(TextExportMode.Text, false, false);
                    gcM.ExportToXlsx(saveFileDialog.FileName, options);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataView dv = new DataView(dtM);
            DateTime time1 = Convert.ToDateTime(da_日期_前.EditValue).Date;
            DateTime time2 = Convert.ToDateTime(da_日期_后.EditValue).Date.AddDays(1).AddSeconds(-1);
            dv.RowFilter = (string.Format("入职年月 >= '{0}' and 入职年月 <= '{1}'", time1, time2));
            gcM.DataSource = dv;
        }
        private void refalsh_single(DataRow dr,string ID)
        {
             
            string sql = $"select * from  人事基础员工表  where 员工号 = '{ID}'";
            DataRow r = CZMaster.MasterSQL.Get_DataRow(sql, CPublic.Var.strConn);
            dr.ItemArray = r.ItemArray;
            dr.AcceptChanges();
        }

        private void gvM_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                //2020-4-2 
                DataRow r = gvM.GetDataRow(gvM.FocusedRowHandle);
                refalsh_single(r, r["员工号"].ToString());

                //判断右键菜单是否可用
                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gcM, new Point(e.X, e.Y));
                    if (CPublic.Var.LocalUserTeam == "管理员权限")
                    {

                        重置密码ToolStripMenuItem.Visible = true;
                        if (r["权限组"].ToString() != "" && r["在职状态"].ToString().Trim() == "在职")
                        {
                            if (r["权限组"].ToString().Contains("停用"))
                            {
                                权限启用ToolStripMenuItem.Visible = true;
                            }
                            else
                            {
                                权限注销ToolStripMenuItem.Visible = true;
                            }
                        }
                        else
                        {
                            权限注销ToolStripMenuItem.Visible = false;
                            权限启用ToolStripMenuItem.Visible = false;
                        }
                    }
                    else
                    {
                        重置密码ToolStripMenuItem.Visible = false;
                        权限注销ToolStripMenuItem.Visible = false;
                        权限启用ToolStripMenuItem.Visible = false;

                    }
                }//单击事件
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
          
        }

        private void gvM1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            //判断右键菜单是否可用
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip2.Show(gcM1, new Point(e.X, e.Y));
            }//单击事件
        }

        private void sl_班组编号_EditValueChanged(object sender, EventArgs e)
        {
            if (sl_班组编号.EditValue != null && sl_班组编号.EditValue.ToString() != "")
            {
                DataRow[] r = dt_班组.Select(string.Format("班组编号='{0}'", sl_班组编号.EditValue));
                if (r.Length > 0) txt_班组.Text = r[0]["班组"].ToString();

            }
            else
            {
                txt_班组.Text = "";

            }
        }
        private void sl_岗位编号_EditValueChanged(object sender, EventArgs e)
        {
            if (sl_岗位编号.EditValue != null && sl_岗位编号.EditValue.ToString() != "")
            {
                DataRow[] r = dt_岗位.Select(string.Format("岗位编号='{0}'", sl_岗位编号.EditValue));
                if (r.Length > 0) txt_岗位.Text = r[0]["岗位"].ToString();
            }
            else
            {
                txt_岗位.Text = "";
            }
        }
        private void zaizhizhuangtai_TextChanged(object sender, EventArgs e)
        {
            if (zaizhizhuangtai.EditValue.ToString() == "在职")
            {
                comboBoxEdit1.Visible = true;
                label62.Visible = true;
            }
            else
            {
                comboBoxEdit1.Visible = false;
                label62.Visible = false;
            }
        }
        //选择课室 给 课室编号 赋值 补坑
        //private void cb_课室_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        //{
        //    if (cb_课室.EditValue != null && cb_课室.EditValue.ToString() != "")
        //    {
        //        string sql = string.Format("select *  from [人事基础部门表] where 部门名称='{0}'",e.NewValue.ToString().Trim());
        //        using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
        //        {
        //            DataTable dt = new DataTable();
        //            da.Fill(dt);
        //            if (dt.Rows.Count > 0)
        //            {
        //                textBox3.Text = dt.Rows[0]["部门编号"].ToString();
        //            }
        //        }
        //    }

        //}

        //private void bumen_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        //{
        //    if (bumen.EditValue != null && bumen.EditValue.ToString() != "")
        //    {
        //        string sql = string.Format("select *  from [人事基础部门表] where 部门名称='{0}'",e.NewValue.ToString().Trim());
        //        using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
        //        {
        //            DataTable dt = new DataTable();
        //            da.Fill(dt);
        //            if (dt.Rows.Count > 0)
        //            {
        //                textBox4.Text = dt.Rows[0]["部门编号"].ToString();
        //            }
        //        }
        //    }

        //}
        //手机只能输入数字
        private void shouji_KeyPress(object sender, KeyPressEventArgs e)
        {

            if ((Control.ModifierKeys & Keys.Control) == Keys.Control) return;
            e.Handled = !(((e.KeyChar >= '0') && (e.KeyChar <= '9')) || (e.KeyChar == (char)8));


            //if ((e.KeyChar >= 48 && e.KeyChar <= 57) || (e.KeyChar >= 64 && e.KeyChar <= 123) || e.KeyChar == 8 || e.KeyChar == 3 || e.KeyChar == 22 || e.KeyChar == ',')
            //{
            //    e.Handled = false;
            //}
            //else
            //{
            //    e.Handled = true;
            //}

        }

        private void gvM_ColumnPositionChanged(object sender, EventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gvM.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void gvM_ColumnFilterChanged(object sender, EventArgs e)
        {
            try
            {
                if (cfgfilepath != "")
                {
                    gvM.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void label56_Click(object sender, EventArgs e)
        {

        }

        //输入身份证 
        private void shenfenzheng_KeyUp(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string identityCard = shenfenzheng.Text.Trim();
                    ERPorg.Corg.BirthdayAgeSex entity = new ERPorg.Corg.BirthdayAgeSex();
                    entity = ERPorg.Corg.GetBirthdayAgeSex(identityCard);
                    if (entity == null) throw new Exception("输入的身份证有误");
                    else
                    {
                        birthday.Text = entity.Birthday;
                        sex.Text = entity.Sex;
                        nianling.Text = entity.Age.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void 重置密码ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("是否确认重置该用户密码", "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                    string sql = string.Format("update 人事基础员工表 set PWD = '123456' where 员工号 = '{0}'", dr["员工号"]);
                    CZMaster.MasterSQL.ExecuteSQL(sql, CPublic.Var.strConn);
                    MessageBox.Show("重置成功");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message) ;
            }
        }

        private void 权限注销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("是否确认注销该用户权限", "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                    if (dr["权限组"].ToString()=="" )
                    {
                        throw new Exception("当前用户没有分配权限,不需要注销");
                    }
                    else if (dr["权限组"].ToString().Contains("(停用)"))
                    {
                        throw new Exception("当前用户权限已注销停用,不要继续操作");
                    }
                    //string sql = string.Format("selet * from  人事基础员工表  where 员工号 = '{0}'", dr["员工号"]);
                    //DataTable dt_员工 = CZMaster.MasterSQL.Get_DataTable(sql, strConn);
                    //if (dt_员工.Rows[0]["权限"] == null|| dt_员工.Rows[0]["权限"].ToString() == "")
                    //{
                    //    throw new Exception("");
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
