using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.OleDb;
using System.Text;

using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using BaseData;
using System.IO;

namespace MoldMangement
{
    public partial class frm_固定资产台账 : UserControl
    {
        string strConn = CPublic.Var.strConn;
        DataTable dt_固定资产;
        DataTable dt_大类;
        DataTable dt_使用人;
        DataTable dt_类型;
        DataTable dt_使用部门;
        DataTable dt_存放地点;
        DataTable dt_使用寿命;
        DataRow drm;
        string p_Name = CPublic.Var.localUserName;   //姓名         
        string P_ID = CPublic.Var.LocalUserID;    //员工号
        bool flag_使用寿命 = true;
        bool flag_pass = false;  //验证是否通过
        bool flag_add = false;   //新增状态   
        bool flag_ismodified = false;  //是否是修改过的状态
        bool flag_rowclick = false;    //判断是否处于行点击事件状态
        Regex reg1 = new Regex(@"^\d{1,}$");      //验证至少1位数字
        Regex reg2 = new Regex(@"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$");    //正浮点数
        Regex reg3 = new Regex(@"^[0-9]*[1-9][0-9]*$");  //正整数
        string modifiedDate = "";
        string modifiedContent = "修改了:";    //保存修改的记录
        string tipContent = "修改内容:" + "\n\n";   //修改记录的提示信息，点击保存时弹框提示使用
        string temp_资产名称 = "";
        string temp_大类 = "";
        string temp_小类 = "";
        string temp_型号 = "";
        string temp_单位 = "";
        string temp_数量 = "";
        string temp_经济用途 = "";
        string temp_使用状态 = "";
        string temp_变动方式 = "";
        string temp_使用人 = "";
        string temp_使用部门编号 = "";
        string temp_使用部门 = "";
        string temp_存放地点 = "";
        string temp_供应商 = "";
        string temp_供应商负责人 = "";
        string temp_供应商联系方式 = "";
        string 负责人 = "";
        string temp_供应商地址 = "";
        string temp_制造商 = "";
        string temp_产地 = "";
        string temp_折旧方法 = "";
        string temp_保养周期 = "";
        string temp_备注 = "";
        //string temp_Date1 = "";

        public frm_固定资产台账()
        {
            InitializeComponent();
            // strConn = string.Format(strConn, "a", "sa", "fmxs", "DESKTOP-1TBQHKE");
        }

        public frm_固定资产台账(string name, string ID)
        {
            InitializeComponent();
            // strConn = string.Format(strConn, "a", "sa", "fmxs", "DESKTOP-1TBQHKE");
            p_Name = name;
            P_ID = ID;
        }



        private void loadDT()
        {
            dt_固定资产 = new DataTable();
            string sql_固定资产 = "select * from 固定资产信息表";
            try
            {
                using (SqlDataAdapter da = new SqlDataAdapter(sql_固定资产, strConn))
                {
                    da.Fill(dt_固定资产);
                }
                gc.DataSource = dt_固定资产;
            }

            catch (Exception)
            {
                throw;
            }
        }




        //保存
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                validate();    //验证函数
                if (flag_pass)
                {
                    textBox12.Text = residualLife();   //计算剩余寿命
                    textBox10.Text = zheJiu();  //计算累计折旧
                    textBox21.Text = benQiZheJiu();  //计算本期折旧额
                    textBox11.Text = jingZhi();  //计算净值
                    getModifiedContent();   //获取修改内容
                    if (flag_add == true && flag_ismodified == false)   //如果是新增状态
                    {
                        DataRow[] dr = dt_固定资产.Select(string.Format("资产编码 = '{0}'", textBox1.Text));
                        if (dr.Length > 0)
                        {
                            MessageBox.Show("资产编码:" + textBox1.Text + "已存在！");    //判断资产编码是否重复
                            return;
                        }
                        else
                        {
                            //validate();    //验证函数
                            //if (flag_pass)
                            //{
                            DialogResult drt = MessageBox.Show("资产编码:" + textBox1.Text.Trim() + "\n" +
                                                               "资产名称:" + textBox2.Text.Trim() + "\n" +
                                                               "型号:" + textBox3.Text.Trim() + "\n" +
                                                               "数量:" + textBox5.Text.Trim() + "\n" +
                                                               "使用日期:" + dateEdit2.Text.Trim() + "\n" +
                                                               "使用寿命:" + searchLookUpEdit3.Text.Trim() + "\n" +
                                                               "购入金额:" + textBox9.Text.Trim() + "\n" +
                                                               "累计折旧:" + textBox10.Text.Trim() + "\n" +
                                                               "净值:" + textBox11.Text.Trim() + "\n\n" +
                                                               "确认新增该固定资产？", "提示", MessageBoxButtons.OKCancel);
                            if (drt == DialogResult.OK)
                            {
                                update_Add();    //新增更新数据库

                            }
                            else
                            {
                                flag_pass = false;
                            }
                            //}
                        }
                    }
                    else if (flag_add == false && flag_ismodified == true)   //如果不是新增状态并且有内容被修改过
                    {
                        //validate();    //验证函数
                        //if (flag_pass)
                        //{
                        DialogResult drt = MessageBox.Show(tipContent + "\n" + "确认修改该固定资产？", "提示", MessageBoxButtons.OKCancel);
                        if (drt == DialogResult.OK)
                        {
                            update_Modify();    //修改更新数据库
                            saveModifyRecord();  //保存修改记录
                            modifiedContent = "修改了:";
                            tipContent = "修改内容:" + "\n\n";
                        }
                        else  //修改了内容也通过了验证但是点了"取消"
                        {
                            flag_pass = false;
                            flag_ismodified = false;
                            modifiedContent = "修改了:";
                            tipContent = "修改内容:" + "\n\n";
                        }
                        //}
                        //else      //修改了内容但是没通过验证
                        //{
                        //    flag_ismodified = false;
                        //    modifiedContent = "修改了:";
                        //    tipContent = "修改内容:" + "\n\n";
                        //}
                    }
                    else if (flag_add == false && flag_ismodified == false)   //如果不是新增状态但是没有数据被修改过
                    {
                        flag_pass = false;
                        MessageBox.Show("没有修改的内容！");
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }



        private void fun_新增()
        {

            flag_add = true;   //设为新增状态
        
        


            clear();
            getModifiedContent();
            textBox1.ReadOnly = false;
        }



        //新增
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_新增();
        }

        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            loadDT();    //刷新dt_固定资产
        }

        //关闭



        //新增更新数据库
        private void update_Add()
        {
            try
            {
                DataTable dt_新增 = assets_Add();
                SqlConnection conn = new SqlConnection(strConn);
                conn.Open();
                SqlTransaction tsc = conn.BeginTransaction();
                string sql_新增 = "select * from 固定资产信息表 where 1<>1";


                string sql_固定履历表 = "select * from 固定资产履历表 ORDER BY  ID";




                try
                {

                    DataTable dt_履历 = CZMaster.MasterSQL.Get_DataTable(sql_固定履历表, strConn);



                    DateTime t = CPublic.Var.getDatetime();
                    DataRow d = dt_履历.NewRow();
                    dt_履历.Rows.Add(d);
                    d["员工号"] = CPublic.Var.LocalUserID;
                    d["使用人"] = CPublic.Var.localUserName;
                    d["部门编号"] = searchLookUpEdit2.Text;
                    d["使用部门"] = textBox7.Text;
                    d["负责人"] = searchLookUpEdit5.Text.ToString();
                    d["负责人ID"] = searchLookUpEdit5.EditValue.ToString();
                    d["存放地点"] = comboBox4.Text;
                    d["资产编码"] = textBox1.Text.ToString();
                    d["资产名称"] = textBox2.Text.ToString();
                    d["起始使用时间"] = dateEdit2.Text.ToString();
                    SqlCommand cm_履历 = new SqlCommand("select * from 固定资产履历表 where 1<>1", conn, tsc);
                    SqlDataAdapter da_履历 = new SqlDataAdapter(cm_履历);
                    new SqlCommandBuilder(da_履历);
                    da_履历.Update(dt_履历);







                    SqlCommand cm_新增 = new SqlCommand(sql_新增, conn, tsc);
                    SqlDataAdapter da_新增 = new SqlDataAdapter(cm_新增);
                    new SqlCommandBuilder(da_新增);
                    if (dt_新增.Rows[0]["资产类型"].ToString() == "模具")
                    {
                        string sql = "select * from 模具管理基础信息表 where 1<>1";
                        SqlDataAdapter da = new SqlDataAdapter(sql, strConn);
                        DataTable dt_模具 = new DataTable();
                        da.Fill(dt_模具);
                        DataRow dr_模具 = dt_模具.NewRow();
                        dt_模具.Rows.Add(dr_模具);
                        dr_模具["模具编号"] = dt_新增.Rows[0]["资产编码"].ToString();
                        dr_模具["模具名称"] = dt_新增.Rows[0]["资产名称"].ToString();
                        dr_模具["审核"] = false;
                        // dr_模具["序号"]=CPublic.CNo.fun_得到最大流水号

                        SqlCommand cm_模具 = new SqlCommand(sql, conn, tsc);
                        SqlDataAdapter da_模具 = new SqlDataAdapter(cm_模具);
                        new SqlCommandBuilder(da_模具);
                        da_模具.Update(dt_模具);
                    }
                    else if (dt_新增.Rows[0]["资产类型"].ToString() == "计量器具")
                    {
                        string sql = "select * from 计量器具基础信息表 where 1<>1";
                        SqlDataAdapter da = new SqlDataAdapter(sql, strConn);
                        DataTable dt_计量器具 = new DataTable();
                        da.Fill(dt_计量器具);
                        DataRow dr_计量器具 = dt_计量器具.NewRow();
                        dt_计量器具.Rows.Add(dr_计量器具);
                        dr_计量器具["计量器具编号"] = dt_新增.Rows[0]["资产编码"].ToString();
                        dr_计量器具["计量器具名称"] = dt_新增.Rows[0]["资产名称"].ToString();
                        dr_计量器具["录入人员"] = CPublic.Var.localUserName;
                        dr_计量器具["录入时间"] = CPublic.Var.getDatetime();
                        SqlCommand cm_计量器具 = new SqlCommand(sql, conn, tsc);
                        SqlDataAdapter da_计量器具 = new SqlDataAdapter(cm_计量器具);
                        new SqlCommandBuilder(da_计量器具);
                        da_计量器具.Update(dt_计量器具);
                    }
                    //else
                    //{
                    //    string sql = "select * from 工装治具表 where 1<>1";
                    //    SqlDataAdapter da = new SqlDataAdapter(sql, strConn);
                    //    DataTable dt_工装治具 = new DataTable();
                    //    da.Fill(dt_工装治具);
                    //    DataRow drr = dt_工装治具.NewRow();
                    //    dt_工装治具.Rows.Add(drr);
                    //    drr["资产编号"] = dt_新增.Rows[0]["资产编码"].ToString();
                    //    drr["资产名称"] = dt_新增.Rows[0]["资产名称"].ToString();
                    //    drr["录入人员"] = CPublic.Var.localUserName;
                    //    drr["录入时间"] = CPublic.Var.getDatetime();
                    //    SqlCommand dtp_工装治具 = new SqlCommand(sql, conn, tsc);
                    //    SqlDataAdapter da_工装治具 = new SqlDataAdapter(dtp_工装治具);
                    //    new SqlCommandBuilder(da_工装治具);
                    //    da_工装治具.Update(dt_工装治具);


                    //}
                    try
                    {
                        da_新增.Update(dt_新增);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    tsc.Commit();
                    flag_pass = false;
                    saveModifyRecord();  //保存新增的记录


                    MessageBox.Show("保存成功");

                    barLargeButtonItem2_ItemClick(null, null);   //设为新增状态
                    barLargeButtonItem1_ItemClick(null, null);   //刷新界面dt
                }
                catch (Exception ex)
                {
                    tsc.Rollback();
                    throw ex;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        //修改更新数据库
        private void update_Modify()
        {
            try
            {
                DataTable dt_修改 = assets_Modify();
                string sql_修改 = "select * from 固定资产信息表 where 1<>1";
                // string sql_固定履历表 = "select * from 固定资产履历表  ORDER BY ID";

                SqlConnection conn = new SqlConnection(strConn);
                conn.Open();
                SqlTransaction tsc = conn.BeginTransaction();
                try
                {
                    string sql_固定履历表 = string.Format("select * from 固定资产履历表 where  资产编码='{0}' ORDER BY  ID ", textBox1.Text.ToString());

                    DataTable dt_履历 = CZMaster.MasterSQL.Get_DataTable(sql_固定履历表, strConn);
                    if (dt_履历.Rows.Count > 0)
                    {
                        if (dt_履历.Rows.Count > 0 && dt_履历.Rows[0]["负责人ID"].ToString() != searchLookUpEdit5.EditValue.ToString())
                        {

                            DataRow dr_履历 = dt_履历.Rows[dt_履历.Rows.Count - 1];

                            //if (dr_履历 != null && dr_履历["员工号"].ToString() == CPublic.Var.LocalUserID)
                            //{

                            //}
                            //else
                            //{
                            DateTime t = CPublic.Var.getDatetime();
                            if (dr_履历 != null)
                            {

                                dr_履历["结束使用时间"] = t;
                            }

                            DataRow d = dt_履历.NewRow();
                            dt_履历.Rows.Add(d);
                            d["员工号"] = CPublic.Var.LocalUserID;
                            d["使用人"] = CPublic.Var.localUserName;
                            d["部门编号"] = searchLookUpEdit2.Text;
                            d["使用部门"] = textBox7.Text;
                            d["资产编码"] = textBox1.Text.ToString();
                            d["资产名称"] = textBox2.Text.ToString();
                            d["存放地点"] = comboBox4.Text;
                            d["负责人"] = searchLookUpEdit5.Text.ToString();
                            d["负责人ID"] = searchLookUpEdit5.EditValue.ToString();
                            d["起始使用时间"] = t;
                            SqlCommand cm_履历 = new SqlCommand("select * from 固定资产履历表 where 1<>1", conn, tsc);
                            SqlDataAdapter da_履历 = new SqlDataAdapter(cm_履历);
                            new SqlCommandBuilder(da_履历);
                            da_履历.Update(dt_履历);

                        }
                    }
                    else
                    {
                        DateTime t = CPublic.Var.getDatetime();
                        DataRow d = dt_履历.NewRow();
                        dt_履历.Rows.Add(d);
                        d["员工号"] = CPublic.Var.LocalUserID;
                        d["使用人"] = CPublic.Var.localUserName;
                        d["部门编号"] = searchLookUpEdit2.Text;
                        d["使用部门"] = textBox7.Text;
                        d["资产编码"] = textBox1.Text.ToString();
                        d["资产名称"] = textBox2.Text.ToString();
                        d["存放地点"] = comboBox4.Text;
                        d["负责人"] = searchLookUpEdit5.Text.ToString();
                        d["负责人ID"] = searchLookUpEdit5.EditValue.ToString();
                        d["起始使用时间"] = dateEdit2.EditValue.ToString() == "" ? DBNull.Value : dateEdit2.EditValue;
                        d["结束使用时间"] = t;
                        SqlCommand cm_履历 = new SqlCommand("select * from 固定资产履历表 where 1<>1", conn, tsc);
                        SqlDataAdapter da_履历 = new SqlDataAdapter(cm_履历);
                        new SqlCommandBuilder(da_履历);
                        da_履历.Update(dt_履历);


                    }



                    ////}
                    //else
                    //{
                    //    DateTime t = CPublic.Var.getDatetime();
                    //    DataRow d = dt_履历.NewRow();
                    //    dt_履历.Rows.Add(d);
                    //    d["员工号"] = CPublic.Var.LocalUserID;
                    //    d["使用人"] = CPublic.Var.localUserName;
                    //    d["部门编号"] = searchLookUpEdit2.Text;
                    //    d["使用部门"] = textBox7.Text;
                    //    d["负责人"] = searchLookUpEdit5.Text.ToString();
                    //    d["负责人ID"] = searchLookUpEdit5.EditValue.ToString();
                    //    d["存放地点"] = comboBox4.Text;
                    //    d["资产编码"] = textBox1.Text.ToString();
                    //    d["资产名称"] = textBox2.Text.ToString();
                    //    d["起始使用时间"] = dateEdit2.Text.ToString();
                    //    SqlCommand cm_履历 = new SqlCommand("select * from 固定资产履历表 where 1<>1", conn, tsc);
                    //    SqlDataAdapter da_履历 = new SqlDataAdapter(cm_履历);
                    //    new SqlCommandBuilder(da_履历);
                    //    da_履历.Update(dt_履历);


                    //}








                    SqlCommand cm_修改 = new SqlCommand(sql_修改, conn, tsc);
                    SqlDataAdapter da_修改 = new SqlDataAdapter(cm_修改);
                    new SqlCommandBuilder(da_修改);
                    try
                    {
                        da_修改.Update(dt_修改);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    tsc.Commit();
                    flag_add = false;
                    flag_pass = false;
                    flag_ismodified = false;
                    //modifiedContent = "修改了:";    
                    //tipContent = "修改内容:" + "\n\n";
                    MessageBox.Show("保存成功");
                    barLargeButtonItem1_ItemClick(null, null);   //刷新界面dt
                }
                catch (Exception ex)
                {
                    tsc.Rollback();
                    throw ex;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //保存修改记录
        private void saveModifyRecord()
        {
            DataTable dt_修改记录 = new DataTable();
            try
            {
                string sql_修改记录 = "select * from 固定资产信息修改日志表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql_修改记录, strConn))
                {
                    da.Fill(dt_修改记录);
                }
                DataRow dr = dt_修改记录.NewRow();
                dr["GUID"] = Guid.NewGuid();
                dr["修改人"] = p_Name;
                dr["修改人ID"] = P_ID;
                dr["修改日期"] = System.DateTime.Now.ToString();
                dr["资产编码"] = textBox1.Text.Trim();
                if (flag_add)  //如果是新增状态
                {
                    dr["修改内容"] = "新增固定资产:" + textBox1.Text.Trim();
                }
                else
                {
                    dr["修改内容"] = modifiedContent;
                }
                dt_修改记录.Rows.Add(dr);
                SqlConnection conn = new SqlConnection(strConn);
                conn.Open();
                SqlTransaction tsc = conn.BeginTransaction();
                try
                {
                    SqlCommand cm_修改记录 = new SqlCommand(sql_修改记录, conn, tsc);
                    SqlDataAdapter da_修改记录 = new SqlDataAdapter(cm_修改记录);
                    new SqlCommandBuilder(da_修改记录);
                    try
                    {
                        da_修改记录.Update(dt_修改记录);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    tsc.Commit();
                }
                catch (Exception ex)
                {
                    tsc.Rollback();
                    throw ex;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        //获取新增的dt
        private DataTable assets_Add()
        {
            DataTable dt_新增 = new DataTable();
            try
            {
                string sql_新增 = "select * from 固定资产信息表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql_新增, strConn))
                {
                    da.Fill(dt_新增);
                }
                DataRow dr = dt_新增.NewRow();
                dr["GUID"] = Guid.NewGuid();
                dr["资产名称"] = textBox2.Text.Trim();
                dr["大类"] = comboBox1.Text.Trim();
                string sql_大类 = string.Format("select * from 基础数据基础属性表 where 属性类别 = '固定资产类别' and 属性值 = '{0}' order by POS", dr["大类"].ToString());
                DataTable dt_属性 = CZMaster.MasterSQL.Get_DataTable(sql_大类, strConn);
                string s_资产编码= "";
                if (textBox1.Text.Trim() == "")
                {
                    s_资产编码 = string.Format("{0}{1}", dt_属性.Rows[0]["属性字段1"].ToString(), CPublic.CNo.fun_得到最大流水号(dt_属性.Rows[0]["属性字段1"].ToString()).ToString("0000"));
                    textBox1.Text = s_资产编码;
                }
                else
                {
                    textBox1.Text=s_资产编码 = textBox1.Text.Trim();

                }

                dr["资产编码"] = textBox1.Text.Trim();
                dr["小类"] = comboBox2.Text.Trim();
                dr["型号"] = textBox3.Text.Trim();
                dr["单位"] = comboBox8.Text.Trim();
                dr["数量"] = textBox5.Text.Trim();
                dr["经济用途"] = comboBox7.Text.Trim();
                dr["变动方式"] = textBox6.Text.Trim();
                dr["入账日期"] = dateEdit1.Text.Trim();
                dr["使用日期"] = dateEdit2.Text.Trim();
                dr["使用状态"] = comboBox3.Text.Trim();
                dr["使用人"] = searchLookUpEdit1.Text.Trim();
                dr["使用部门"] = textBox7.Text.Trim();
                dr["部门编号"] = searchLookUpEdit2.Text.Trim();
                dr["存放地点"] = comboBox4.Text.Trim();
                dr["购入金额"] = textBox9.Text.Trim();
                dr["累计折旧"] = textBox10.Text.Trim();
                dr["净值"] = textBox11.Text.Trim();
                dr["预计净残值"] = textBox20.Text.Trim();
                dr["本期折旧额"] = textBox21.Text.Trim();
                dr["使用寿命"] = searchLookUpEdit3.Text.Trim();
                dr["剩余寿命"] = textBox12.Text.Trim();
                dr["折旧方法"] = comboBox6.Text.Trim();
                dr["保养周期"] = comboBox5.Text.Trim();
                dr["资产类型"] = comboBox9.Text.Trim();
                //dr["上次保养日期"] = textBox13.Text.Trim(); 
                dr["供应商"] = textBox14.Text.Trim();
                dr["供应商负责人"] = textBox15.Text.Trim();
                dr["供应商联系方式"] = textBox16.Text.Trim();
                dr["供应商地址"] = textBox17.Text.Trim();
                dr["制造商"] = textBox18.Text.Trim();
                dr["产地"] = textBox19.Text.Trim();
                dr["备注1"] = textBox8.Text.Trim();
                dr["修改人"] = p_Name;

                if (searchLookUpEdit5.EditValue==null || searchLookUpEdit5.EditValue.ToString() == "")
                {
                    dr["负责人"] = textBox4.Text.Trim();
                    dr["负责人ID"] = textBox5.Text.ToString();
                }
                else
                {
                    dr["负责人"] = searchLookUpEdit5.Text.Trim();
                    dr["负责人ID"] = searchLookUpEdit5.EditValue.ToString();
                }

                dr["修改人ID"] = P_ID;
                dr["修改日期"] = System.DateTime.Now.ToString();
                dt_新增.Rows.Add(dr);
            }
            catch (Exception)
            {

                throw;
            }
            return dt_新增;
        }

        //获取修改的dt
        private DataTable assets_Modify()
        {
            DataTable dt_修改 = new DataTable();
            try
            {
                string sql_修改 = string.Format("select * from 固定资产信息表 where 资产编码 = '{0}'", textBox1.Text.Trim());
                using (SqlDataAdapter da = new SqlDataAdapter(sql_修改, strConn))
                {
                    da.Fill(dt_修改);
                }
                dt_修改.Rows[0]["资产名称"] = textBox2.Text.Trim();
                dt_修改.Rows[0]["大类"] = comboBox1.Text.Trim();
                dt_修改.Rows[0]["小类"] = comboBox2.Text.Trim();
                dt_修改.Rows[0]["型号"] = textBox3.Text.Trim();
                dt_修改.Rows[0]["单位"] = comboBox8.Text.Trim();
                dt_修改.Rows[0]["数量"] = textBox5.Text.Trim();
                dt_修改.Rows[0]["经济用途"] = comboBox7.Text.Trim();
                dt_修改.Rows[0]["变动方式"] = textBox6.Text.Trim();
                dt_修改.Rows[0]["使用状态"] = comboBox3.Text.Trim();
                dt_修改.Rows[0]["使用人"] = searchLookUpEdit1.Text.Trim();
                dt_修改.Rows[0]["使用部门"] = textBox7.Text.Trim();
                dt_修改.Rows[0]["部门编号"] = searchLookUpEdit2.Text.Trim();
                dt_修改.Rows[0]["存放地点"] = comboBox4.Text.Trim();
                dt_修改.Rows[0]["折旧方法"] = comboBox6.Text.Trim();
                dt_修改.Rows[0]["保养周期"] = comboBox5.Text.Trim();
                dt_修改.Rows[0]["供应商"] = textBox14.Text.Trim();
                dt_修改.Rows[0]["供应商负责人"] = textBox15.Text.Trim();
                dt_修改.Rows[0]["供应商联系方式"] = textBox16.Text.Trim();
                dt_修改.Rows[0]["供应商地址"] = textBox17.Text.Trim();
                dt_修改.Rows[0]["制造商"] = textBox18.Text.Trim();
                dt_修改.Rows[0]["产地"] = textBox19.Text.Trim();
                dt_修改.Rows[0]["备注1"] = textBox8.Text.Trim();
                dt_修改.Rows[0]["修改人"] = p_Name;
                dt_修改.Rows[0]["修改人ID"] = P_ID;
                dt_修改.Rows[0]["修改日期"] = CPublic.Var.getDatetime();

            }
            catch (Exception)
            {

                throw;
            }
            return dt_修改;
        }

        private void validate()
        {
            try
            {
                //if (textBox1.Text.Trim() == "")
                //{
                //    MessageBox.Show("资产编码为空！");
                //    return;
                //}
                if (textBox2.Text.Trim() == "")
                {
                    MessageBox.Show("资产名称为空！");
                    return;
                }
                if (comboBox1.Text.Trim() == "")
                {
                    MessageBox.Show("资产大类为空");
                    return;
                }
                if (textBox5.Text.Trim() == "")
                {
                    MessageBox.Show("数量为空！");
                    return;
                }
                else if (textBox5.Text.Trim() != "" && reg3.IsMatch(textBox5.Text.Trim()) == false)
                {
                    MessageBox.Show("数量格式有误！");
                    return;
                }
                if (comboBox1.Text.Trim() == "")
                {
                    MessageBox.Show("使用状态为空");
                    return;
                }
                //if (dateEdit1.EditValue == null)
                //{
                //    MessageBox.Show("入账日期为空！");
                //    return;
                //}
                if (dateEdit2.EditValue == null)
                {
                    MessageBox.Show("使用日期为空！");
                    return;
                }
                if(searchLookUpEdit1.EditValue==null )
                {
                    throw new Exception("负责人未选择");
                }
                if (textBox9.Text.Trim() == "")
                {
                    throw new Exception("购入金额未填写");
                }
                if (textBox20.Text.Trim() == "")
                {
                    throw new Exception("预计净残值未填写");
                }


                //19-12-23 初次导入 这些信息没有 不能修改 所以先将限制去除
                //if (textBox9.Text.Trim() == "")
                //{
                //    MessageBox.Show("购入金额为空！");
                //    return;
                //}
                //else if (reg2.IsMatch(textBox9.Text.Trim()) == false)
                //{
                //    MessageBox.Show("购入金额格式有误！");
                //    return;
                //}
                //if (textBox20.Text.Trim() == "")
                //{
                //    MessageBox.Show("预计净残值为空！");
                //    return;
                //}
                //else if (reg2.IsMatch(textBox20.Text.Trim()) == false)
                //{
                //    MessageBox.Show("预计净残值格式有误！");
                //    return;
                //}
                //else if (Convert.ToDecimal(textBox20.Text.Trim()) >= Convert.ToDecimal(textBox9.Text.Trim()))
                //{
                //    MessageBox.Show("预计净残值应小于购入金额！");
                //    return;
                //}
                //if (searchLookUpEdit3.Text.Trim() == "")
                //{
                //    MessageBox.Show("使用寿命为空！");
                //    return;
                //}
                if (comboBox9.Text.Trim() == "")
                {
                    MessageBox.Show("资产类型为空");
                    return;
                }
                //if (textBox10.Text.Trim() == "")
                //{
                //    MessageBox.Show("累计折旧为空,请单击'累计折旧'文本框计算相应金额！");
                //    return;
                //}
                flag_pass = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        //计算剩余寿命
        private string residualLife()
        {
            try
            {
                if (dateEdit2.Text.ToString() != "")   //使用日期是否为空
                {
                    DateTime datetime1 = CPublic.Var.getDatetime();
                    DateTime datetime2 = Convert.ToDateTime(dateEdit2.Text.ToString());   //使用日期
                    int month = (datetime1.Year - datetime2.Year) * 12 + (datetime1.Month - datetime2.Month);   //当前日期和开始使用日期相差的月份 
                    if (searchLookUpEdit3.Text.ToString() != "")  //使用寿命是否为空
                    {
                        int life = Convert.ToInt32(searchLookUpEdit3.EditValue.ToString().TrimEnd('期'));
                        if (datetime2 <= datetime1)
                        {
                            string restLife = "";
                            if (life >= month)
                            {
                                restLife = Convert.ToString(life - month) + "期";
                            }
                            else
                            {
                                restLife = "0期";
                            }
                            return restLife;
                        }
                        else     //如果使用日期比当前日期还晚
                        {
                            return life.ToString() + "期";   //返回原使用寿命
                        }

                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }

            }
            catch (Exception)
            {

                throw;
            }
            //TimeSpan ts = datetime1.Subtract(datetime2);
            //MessageBox.Show(month.ToString());    //计算相差天数
        }

        //计算累计折旧
        private string zheJiu()
        {
            decimal lostvalue = 0.0M;
            if (reg2.IsMatch(textBox9.Text.Trim()) && reg2.IsMatch(textBox20.Text.Trim())) //如果购入金额和预计净残值满足要求
            {
                decimal price = Convert.ToDecimal(textBox9.Text);    //购入金额(原值)
                decimal restvalue = Convert.ToDecimal(textBox20.Text);    //预计净残值(残值)
                if (price > 0.00M && restvalue > 0.00M && price > restvalue && searchLookUpEdit3.Text.ToString() != "" && textBox12.Text != "")
                {
                    int life = Convert.ToInt32(searchLookUpEdit3.EditValue.ToString().TrimEnd('期'));   //使用寿命
                    int restlife = Convert.ToInt32(textBox12.Text.TrimEnd('期'));          //剩余寿命
                    lostvalue = (price - restvalue) / life * (life - restlife);   //累计折旧 = （原值-残值）/可使用年限*已使用年限
                    return lostvalue.ToString("#0.00");
                }
                else
                {
                    return "0";
                }
            }
            else
            {
                return "0";
            }
        }

        //计算本期折旧额
        private string benQiZheJiu()
        {
            decimal lostvalue = 0.0M;
            if (reg2.IsMatch(textBox9.Text.Trim()) && reg2.IsMatch(textBox20.Text.Trim()) && searchLookUpEdit3.Text != "")
            {
                decimal price = Convert.ToDecimal(textBox9.Text);    //购入金额
                decimal restvalue = Convert.ToDecimal(textBox20.Text);    //预计净残值
                if (price > 0.00M && restvalue > 0.00M && price > restvalue)
                {
                    int life = Convert.ToInt32(searchLookUpEdit3.EditValue.ToString().TrimEnd('期'));   //使用寿命
                    lostvalue = (price - restvalue) / life;   //（原值-残值）/可使用年限
                    return lostvalue.ToString("#0.00");
                }
                else
                {
                    return "0";
                }

            }
            else
            {
                return "0";
            }

        }

        //计算净值
        private string jingZhi()
        {
            try
            {
                decimal netvalue = 0.0M;    //净值
                if (reg2.IsMatch(textBox9.Text.Trim()) && textBox10.Text.Trim() != "")
                {
                    netvalue = Convert.ToDecimal(textBox9.Text) - Convert.ToDecimal(textBox10.Text);     //原值-累计折旧=净值
                    return netvalue.ToString("#0.00");
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception)
            {

                throw;
            }
        }



        //入账日期值改变事件
        private void dateEdit1_EditValueChanged(object sender, EventArgs e)
        {
            //if (flag_rowclick == false)  //如果dateEdit1入账日期不是在行点击事件时发生的值改变
            //{
            //    //如果使用日期不为空并且选择的入账日期晚于使用日期，则提示用户更改
            //    if (dateEdit2.Text != "" && Convert.ToDateTime(dateEdit1.EditValue) > Convert.ToDateTime(dateEdit2.EditValue))
            //    {
            //        dateEdit1.Text = temp_Date1;
            //        MessageBox.Show("入账日期不能晚于使用日期，请更改！");
            //    }
            //}
            //else
            //{
            //    flag_rowclick = false;
            //}

        }

        //使用日期值改变事件
        private void dateEdit2_EditValueChanged(object sender, EventArgs e)
        {
            textBox12.Text = residualLife();   //计算剩余寿命
            textBox10.Text = zheJiu();  //计算累计折旧
            textBox21.Text = benQiZheJiu();  //计算本期折旧额
            textBox11.Text = jingZhi();  //计算净值

        }

        //使用寿命值改变事件
        private void searchLookUpEdit3_EditValueChanged(object sender, EventArgs e)
        {
            textBox12.Text = residualLife();   //计算剩余寿命
            textBox10.Text = zheJiu();  //计算累计折旧
            textBox21.Text = benQiZheJiu();  //计算本期折旧额
            textBox11.Text = jingZhi();  //计算净值
        }

        //单击累计折旧事件
        private void textBox10_Click(object sender, EventArgs e)
        {
            //textBox12.Text = residualLife();   //计算剩余寿命
            //textBox10.Text = zheJiu();  //计算累计折旧
            //textBox21.Text = benQiZheJiu();  //计算本期折旧额
            //textBox11.Text = jingZhi();  //计算净值
        }

        //使用部门编号值改变事件
        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void Initialize()
        {
            comboBox3.SelectedIndex = 0;   //使用状态默认选择"在用"
            comboBox6.SelectedIndex = 0;
            dt_大类 = new DataTable();
            dt_类型 = new DataTable();
            dt_使用人 = new DataTable();
            dt_使用部门 = new DataTable();
            dt_存放地点 = new DataTable();
            dt_使用寿命 = new DataTable();
            string sql_资本类型 = " select 属性值,POS from 基础数据基础属性表 where 属性类别 = '固定资产类型' order by POS";
            string sql_大类 = "select 属性值,POS from 基础数据基础属性表 where 属性类别 = '固定资产类别' order by POS";
            string sql_使用人 = "select 员工号,姓名,部门 from 人事基础员工表 where 在职状态 = '在职'";
            string sql_使用部门 = "select 部门编号,部门名称 from 人事基础部门表   order by 部门编号";
            string sql_存放地点 = "select 属性值,POS from 基础数据基础属性表 where 属性类别 = '固定资产存放地点' order by POS";
            string sql_使用寿命 = "select 属性值,属性字段2 as 备注 from 基础数据基础属性表 where 属性类别 = '固定资产使用寿命' order by POS";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_大类, strConn))
            {
                da.Fill(dt_大类);
            }
            comboBox1.DataSource = dt_大类;
            comboBox1.ValueMember = "属性值";
            comboBox1.DisplayMember = "属性值";
            comboBox1.SelectedIndex = -1;

            using (SqlDataAdapter da = new SqlDataAdapter(sql_资本类型, strConn))
            {
                da.Fill(dt_类型);
            }
            comboBox9.DataSource = dt_类型;
            comboBox9.ValueMember = "属性值";
            comboBox9.DisplayMember = "属性值";
            comboBox9.SelectedIndex = -1;






            using (SqlDataAdapter da = new SqlDataAdapter(sql_使用人, strConn))
            {
                da.Fill(dt_使用人);
            }
            searchLookUpEdit1.Properties.DataSource = dt_使用人;
            searchLookUpEdit1.Properties.ValueMember = "姓名";
            searchLookUpEdit1.Properties.DisplayMember = "姓名";



            searchLookUpEdit5.Properties.DataSource = dt_使用人;
            searchLookUpEdit5.Properties.ValueMember = "员工号";
            searchLookUpEdit5.Properties.DisplayMember = "姓名";


            using (SqlDataAdapter da = new SqlDataAdapter(sql_使用部门, strConn))
            {
                da.Fill(dt_使用部门);
            }
            searchLookUpEdit2.Properties.DataSource = dt_使用部门;
            searchLookUpEdit2.Properties.ValueMember = "部门编号";
            searchLookUpEdit2.Properties.DisplayMember = "部门编号";
            searchLookUpEdit4.Properties.DataSource = dt_使用部门;
            searchLookUpEdit4.Properties.ValueMember = "部门编号";
            searchLookUpEdit4.Properties.DisplayMember = "部门名称";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_存放地点, strConn))
            {
                da.Fill(dt_存放地点);
            }
            comboBox4.DataSource = dt_存放地点;
            comboBox4.ValueMember = "属性值";
            comboBox4.DisplayMember = "属性值";
            comboBox4.SelectedIndex = -1;
            using (SqlDataAdapter da = new SqlDataAdapter(sql_使用寿命, strConn))
            {
                da.Fill(dt_使用寿命);
            }
            foreach (DataRow dr in dt_使用寿命.Rows)
            {
                string life = dr["属性值"].ToString();
                if (life.Substring(life.Length - 1, 1) != "期" || reg1.IsMatch(life.TrimEnd('期')) == false)
                {
                    flag_使用寿命 = false;
                    MessageBox.Show("使用寿命周期:\"" + life + "\"维护不正确，请联系管理员维护！");
                    break;
                }
            }
            if (flag_使用寿命 == true)
            {
                searchLookUpEdit3.Properties.DataSource = dt_使用寿命;
                searchLookUpEdit3.Properties.ValueMember = "属性值";
                searchLookUpEdit3.Properties.DisplayMember = "属性值";
            }
        }

        //清空控件值
        private void clear()
        {
            setNotReadOnly();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox8.Text = "";
            textBox9.Text = "";
            textBox10.Text = "";
            textBox11.Text = "";
            textBox13.Text = "";
            textBox14.Text = "";
            textBox15.Text = "";
            textBox16.Text = "";
            textBox17.Text = "";
            textBox18.Text = "";
            textBox19.Text = "";
            textBox20.Text = "";
            textBox21.Text = "";
            textBox23.Text = "";
            comboBox1.Text = null;
            comboBox2.Text = null;
            comboBox3.Text = null;
            comboBox3.SelectedIndex = 0;
            comboBox4.Text = null;
            comboBox5.Text = null;
            comboBox6.Text = null;
            comboBox6.SelectedIndex = 0;
            comboBox7.Text = null;
            comboBox8.Text = null;
            dateEdit1.EditValue = null;
            dateEdit2.EditValue = null;
            searchLookUpEdit1.Text = null;
            searchLookUpEdit2.Text = null;
            searchLookUpEdit3.Text = null;
        }

        //设置购入金额、预计净残值、使用寿命不可编辑
        private void setReadOnly()
        {
            textBox1.ReadOnly = true;
            textBox9.ReadOnly = true;
            textBox20.ReadOnly = true;
            dateEdit1.Properties.ReadOnly = true;
            dateEdit2.Properties.ReadOnly = true;
            //searchLookUpEdit3.Properties.ReadOnly = true;
            // 19-12-23初次使用暂时先取消限制
        }

        //设置购入金额、预计净残值、使用寿命可编辑
        private void setNotReadOnly()
        {
            textBox1.ReadOnly = true;
            textBox9.ReadOnly = false;
            textBox20.ReadOnly = false;
            dateEdit1.Properties.ReadOnly = false;
            dateEdit2.Properties.ReadOnly = false;
            searchLookUpEdit3.Properties.ReadOnly = false;
        }

        //把使用人、部门编号、使用寿命字段值传给searchlookupedit1、2、3,把使用日期字段值传给dateEdit2
        private void rowToForm()
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                searchLookUpEdit1.EditValue = dr["使用人"].ToString();
                searchLookUpEdit5.EditValue = dr["负责人ID"].ToString();
                searchLookUpEdit2.EditValue = dr["部门编号"].ToString();
                searchLookUpEdit3.EditValue = dr["使用寿命"].ToString();
                if (dr["使用日期"].ToString() != "")  //如果使用日期不为空
                {
                    dateEdit2.EditValue = Convert.ToDateTime(dr["使用日期"].ToString()).ToString("yyyy-MM-dd");
                }
                else
                {
                    dateEdit2.EditValue = dr["使用日期"].ToString();    //如果使用日期为空，直接赋空值过去
                }
                //if (dr["使用日期"].ToString() != "" && Convert.ToDateTime(dr["入账日期"].ToString()) <= Convert.ToDateTime(dr["使用日期"].ToString()))     //如果使用日期不为空并且使用日期不早于入账日期
                //{
                //    dateEdit2.EditValue = Convert.ToDateTime(dr["使用日期"].ToString()).ToString("yyyy-MM-dd");
                //}
                //else if (dr["使用日期"].ToString() == "")
                //{
                //    dateEdit2.EditValue = dr["使用日期"].ToString();    //如果使用日期为空，直接赋空值过去
                //}
                //else if (Convert.ToDateTime(dr["入账日期"].ToString()) > Convert.ToDateTime(dr["使用日期"].ToString()))  //如果入账日期晚于使用日期
                //{
                //    dateEdit2.EditValue = Convert.ToDateTime(dr["入账日期"].ToString()).ToString("yyyy-MM-dd");
                //    MessageBox.Show("使用日期不能比入账日期早，请对使用日期做相应更改！");
                //}
            }
            catch (Exception)
            {

                throw;
            }
        }

        //将行点击事件时获取的信息存在temp变量中
        private void rowToTemp()
        {

            DataRow drm = gv.GetDataRow(gv.FocusedRowHandle);
            textBox2.Text = drm["资产名称"].ToString();
            comboBox9.Text = drm["资产类型"].ToString();
            comboBox1.Text = drm[3].ToString();
            comboBox2.Text = drm["小类"].ToString();
            textBox3.Text = drm["型号"].ToString();
            comboBox8.Text = drm["单位"].ToString();
            textBox5.Text = drm["数量"].ToString();
            comboBox7.Text = drm["经济用途"].ToString();
            comboBox3.Text = drm["使用状态"].ToString();
            textBox6.Text = drm["变动方式"].ToString();
            searchLookUpEdit1.Text = drm["使用人"].ToString();
            searchLookUpEdit2.Text = drm["部门编号"].ToString();
            textBox7.Text = drm["使用部门"].ToString();
            comboBox4.Text = drm["存放地点"].ToString();
            textBox10.Text = drm["累计折旧"].ToString();
            textBox11.Text = drm["净值"].ToString();
            textBox14.Text = drm["供应商"].ToString();
            textBox15.Text = drm["供应商负责人"].ToString();
            textBox19.Text = drm["产地"].ToString();
            textBox17.Text = drm["供应商地址"].ToString();
            textBox18.Text = drm["制造商"].ToString();
            comboBox6.Text = drm["折旧方法"].ToString();
            comboBox5.Text = drm["保养周期"].ToString();
            textBox8.Text = drm["备注1"].ToString();
            dateEdit2.Text = drm["使用日期"].ToString();
            dateEdit1.Text = drm["入账日期"].ToString();
            textBox9.Text = drm["购入金额"].ToString();
            textBox1.Text = drm["资产编码"].ToString();
            textBox20.Text = drm["预计净残值"].ToString();
            textBox13.Text = drm["上次保养日期"].ToString();
            textBox23.Text = drm["序列号"].ToString();





            textBox22.Text = drm["负责人ID"].ToString();


            temp_使用部门编号 = searchLookUpEdit2.Text.Trim();
            temp_使用部门 = textBox7.Text.Trim();
            temp_存放地点 = comboBox4.Text.Trim();
            temp_供应商 = textBox14.Text.Trim();
            temp_供应商负责人 = textBox15.Text.Trim();
            temp_供应商联系方式 = textBox16.Text.Trim();
            temp_供应商地址 = textBox17.Text.Trim();
            temp_制造商 = textBox18.Text.Trim();
            temp_产地 = textBox19.Text.Trim();
            temp_折旧方法 = comboBox6.Text.Trim();
            temp_保养周期 = comboBox5.Text.Trim();
            temp_备注 = textBox8.Text.Trim();


            temp_资产名称 = textBox2.Text.Trim();
            temp_大类 = comboBox1.Text.Trim();
            temp_小类 = comboBox2.Text.Trim();
            temp_型号 = textBox3.Text.Trim();
            temp_单位 = comboBox8.Text.Trim();
            temp_数量 = textBox5.Text.Trim();
            temp_经济用途 = comboBox7.Text.Trim();
            temp_使用状态 = comboBox3.Text.Trim();
            temp_变动方式 = textBox6.Text.Trim();
            temp_使用人 = searchLookUpEdit1.Text.Trim();
        }

        //获取修改记录
        private void getModifiedContent()
        {
            try
            {
                if (flag_add)   //如果处于新增的状态,则不进行下面的操作
                {
                    return;
                }
                else
                {
                    DataTable dt_修改 = new DataTable();
                    string sql_修改 = string.Format("select * from 固定资产信息表 where 资产编码 = '{0}'", textBox1.Text.Trim());
                    using (SqlDataAdapter da = new SqlDataAdapter(sql_修改, strConn))
                    {
                        da.Fill(dt_修改);
                    }
                    modifiedDate = dt_修改.Rows[0]["修改日期"].ToString();     //把上次修改的日期保存在modifiedDate变量中
                    if (temp_资产名称 != textBox2.Text.Trim())
                    {
                        modifiedContent += "资产名称的值,原:" + temp_资产名称 + ",现:" + textBox2.Text.Trim() + ";";
                        tipContent += "资产名称 原:" + temp_资产名称 + "   现:" + textBox2.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_大类 != comboBox1.Text.Trim())
                    {
                        modifiedContent += "大类的值,原:" + temp_大类 + ",现:" + comboBox1.Text.Trim() + ";";
                        tipContent += "大类 原:" + temp_大类 + "   现:" + comboBox1.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_小类 != comboBox2.Text.Trim())
                    {
                        modifiedContent += "小类的值,原:" + temp_小类 + ",现:" + comboBox2.Text.Trim() + ";";
                        tipContent += "小类 原:" + temp_小类 + "   现:" + comboBox2.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_型号 != textBox3.Text.Trim())
                    {
                        modifiedContent += "型号的值,原:" + temp_型号 + ",现:" + textBox3.Text.Trim() + ";";
                        tipContent += "型号 原:" + temp_型号 + "   现:" + textBox3.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_单位 != comboBox8.Text.Trim())
                    {
                        modifiedContent += "单位的值,原:" + temp_单位 + ",现:" + comboBox8.Text.Trim() + ";";
                        tipContent += "单位 原:" + temp_单位 + "   现:" + comboBox8.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_数量 != textBox5.Text.Trim())
                    {
                        modifiedContent += "数量的值,原:" + temp_数量 + ",现:" + textBox5.Text.Trim() + ";";
                        tipContent += "数量 原:" + temp_数量 + "   现:" + textBox5.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_经济用途 != comboBox7.Text.Trim())
                    {
                        modifiedContent += "经济用途的值,原:" + temp_经济用途 + ",现:" + comboBox7.Text.Trim() + ";";
                        tipContent += "经济用途 原:" + temp_经济用途 + "   现:" + comboBox7.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_使用状态 != comboBox3.Text.Trim())
                    {
                        modifiedContent += "使用状态的值,原:" + temp_使用状态 + ",现:" + comboBox3.Text.Trim() + ";";
                        tipContent += "使用状态 原:" + temp_使用状态 + "   现:" + comboBox3.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_变动方式 != textBox6.Text.Trim())
                    {
                        modifiedContent += "变动方式的值,原:" + temp_变动方式 + ",现:" + textBox6.Text.Trim() + ";";
                        tipContent += "变动方式 原:" + temp_变动方式 + "   现:" + textBox6.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_使用人 != searchLookUpEdit1.Text.Trim())
                    {
                        modifiedContent += "使用人的值,原:" + temp_使用人 + ",现:" + searchLookUpEdit1.Text.Trim() + ";";
                        tipContent += "使用人 原:" + temp_使用人 + "   现:" + searchLookUpEdit1.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_使用部门编号 != searchLookUpEdit2.Text.Trim())
                    {
                        modifiedContent += "部门编号的值,原:" + temp_使用部门编号 + ",现:" + searchLookUpEdit2.Text.Trim() + ";";
                        tipContent += "部门编号 原:" + temp_使用部门编号 + "   现:" + searchLookUpEdit2.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_使用部门 != textBox7.Text.Trim())
                    {
                        modifiedContent += "使用部门的值,原:" + temp_使用部门 + ",现:" + textBox7.Text.Trim() + ";";
                        tipContent += "使用部门 原:" + temp_使用部门 + "   现:" + textBox7.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_存放地点 != comboBox4.Text.Trim())
                    {
                        modifiedContent += "存放地点的值,原:" + temp_存放地点 + ",现:" + comboBox4.Text.Trim() + ";";
                        tipContent += "存放地点 原:" + temp_存放地点 + "   现:" + comboBox4.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_供应商 != textBox14.Text.Trim())
                    {
                        modifiedContent += "供应商的值,原:" + temp_供应商 + ",现:" + textBox14.Text.Trim() + ";";
                        tipContent += "供应商 原:" + temp_供应商 + "   现:" + textBox14.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_供应商负责人 != textBox15.Text.Trim())
                    {
                        modifiedContent += "供应商负责人的值,原:" + temp_供应商负责人 + ",现:" + textBox15.Text.Trim() + ";";
                        tipContent += "供应商负责人 原:" + temp_供应商负责人 + "   现:" + textBox15.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_供应商联系方式 != textBox16.Text.Trim())
                    {
                        modifiedContent += "供应商联系方式的值,原:" + temp_供应商联系方式 + ",现:" + textBox16.Text.Trim() + ";";
                        tipContent += "供应商联系方式 原:" + temp_供应商联系方式 + "   现:" + textBox16.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    //if ( != comboBox6.Text.Trim())
                    //{
                    //    modifiedContent += "折旧方法的值,原:" + temp_折旧方法 + ",现:" + comboBox6.Text.Trim() + ";";
                    //    tipContent += "折旧方法 原:" + temp_折旧方法 + "   现:" + comboBox6.Text.Trim() + "\n";
                    //    flag_ismodified = true;
                    //}


                    if (temp_供应商地址 != textBox17.Text.Trim())
                    {
                        modifiedContent += "供应商地址的值,原:" + temp_供应商地址 + ",现:" + textBox17.Text.Trim() + ";";
                        tipContent += "供应商地址 原:" + temp_供应商地址 + "   现:" + textBox17.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_制造商 != textBox18.Text.Trim())
                    {
                        modifiedContent += "制造商的值,原:" + temp_制造商 + ",现:" + textBox18.Text.Trim() + ";";
                        tipContent += "制造商 原:" + temp_制造商 + "   现:" + textBox18.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_产地 != textBox19.Text.Trim())
                    {
                        modifiedContent += "产地的值,原:" + temp_产地 + ",现:" + textBox19.Text.Trim() + ";";
                        tipContent += "产地 原:" + temp_产地 + "   现:" + textBox19.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_折旧方法 != comboBox6.Text.Trim())
                    {
                        modifiedContent += "折旧方法的值,原:" + temp_折旧方法 + ",现:" + comboBox6.Text.Trim() + ";";
                        tipContent += "折旧方法 原:" + temp_折旧方法 + "   现:" + comboBox6.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_保养周期 != comboBox5.Text.Trim())
                    {
                        modifiedContent += "保养周期的值,原:" + temp_保养周期 + ",现:" + comboBox5.Text.Trim() + ";";
                        tipContent += "保养周期 原:" + temp_保养周期 + "   现:" + comboBox5.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (temp_备注 != textBox8.Text.Trim())
                    {
                        modifiedContent += "备注的值,原:" + temp_备注 + ",现:" + textBox8.Text.Trim() + ";";
                        tipContent += "备注 原:" + temp_备注 + "   现:" + textBox8.Text.Trim() + "\n";
                        flag_ismodified = true;
                    }
                    if (flag_ismodified)
                    {
                        modifiedContent += "修改日期的值,原:" + modifiedDate + ",现:" + System.DateTime.Now.ToString() + ";";
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc, new System.Drawing.Point(e.X, e.Y));
            }
        }

        private void 修改日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            modifiedLog fm = new modifiedLog(dr["资产编码"].ToString(), dr["资产名称"].ToString());
            fm.StartPosition = FormStartPosition.CenterScreen;    //跳出窗体时屏幕居中显示
            fm.ShowDialog();
        }




        //行号
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        //导出
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                gc.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //打印
        //private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    try
        //    {
        //        gv.CloseEditor();
        //        this.BindingContext[dt_固定资产].EndCurrentEdit();
        //        string str = "";
        //        string str_打印机;
        //        PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
        //        this.printDialog1.Document = this.printDocument1;
        //        DialogResult drt = this.printDialog1.ShowDialog();
        //        if (drt == DialogResult.OK)
        //        {
        //           ItemInspection.print_FMS.fun_固定资产打印(dt_固定资产, printDialog1.PrinterSettings.PrinterName, false);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}


        //删除
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr["资产类型"].ToString() == "模具")
                {
                    string sql = string.Format("select * from 模具保养记录 where 模具编号 = '{0}'", dr["资产编码"].ToString());
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strConn);
                    if (dt.Rows.Count == 0)
                    {
                        dr.Delete();
                    }
                    else if (dt.Rows.Count > 0)
                    {
                        MessageBox.Show("该条记录不能删除！");
                        return;
                    }
                }
                if (dr["资产类型"].ToString() == "计量器具")
                {
                    string sql = string.Format("select * from 计量器具明细卡表 where 计量器具编号 ='{0}'", dr["资产编码"].ToString());
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strConn);
                    if (dt.Rows.Count == 0)
                    {
                        dr.Delete();
                    }
                    else if (dt.Rows.Count > 0)
                    {
                        MessageBox.Show("该条记录不能删除！");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem4_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem3_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                validate();    //验证函数
                if (flag_pass)
                {

                    textBox12.Text = residualLife();   //计算剩余寿命
                    
                    textBox10.Text = zheJiu();  //计算累计折旧
                    textBox21.Text = benQiZheJiu();  //计算本期折旧额
                    textBox11.Text = jingZhi();  //计算净值
                    getModifiedContent();   //获取修改内容
                    if (flag_add == true && flag_ismodified == false)   //如果是新增状态
                    {
                        DataRow[] dr = dt_固定资产.Select(string.Format("资产编码 = '{0}'", textBox1.Text));
                        if (dr.Length > 0)
                        {
                            MessageBox.Show("资产编码:" + textBox1.Text + "已存在！");    //判断资产编码是否重复
                            return;
                        }
                        else
                        {
                            //validate();    //验证函数
                            //if (flag_pass)
                            //{
                            DialogResult drt = MessageBox.Show("资产编码:" + textBox1.Text.Trim() + "\n" +
                                                               "资产名称:" + textBox2.Text.Trim() + "\n" +
                                                               "型号:" + textBox3.Text.Trim() + "\n" +
                                                               "数量:" + textBox5.Text.Trim() + "\n" +
                                                               "使用日期:" + dateEdit2.Text.Trim() + "\n" +
                                                               "使用寿命:" + searchLookUpEdit3.Text.Trim() + "\n" +
                                                               "购入金额:" + textBox9.Text.Trim() + "\n" +
                                                               "累计折旧:" + textBox10.Text.Trim() + "\n" +
                                                               "净值:" + textBox11.Text.Trim() + "\n\n" +
                                                               "确认新增该固定资产？", "提示", MessageBoxButtons.OKCancel);
                            if (drt == DialogResult.OK)
                            {
                                update_Add();    //新增更新数据库

                            }
                            else
                            {
                                flag_pass = false;
                            }
                            //}
                        }
                    }
                    else if (flag_add == false && flag_ismodified == true)   //如果不是新增状态并且有内容被修改过
                    {
                        //validate();    //验证函数
                        //if (flag_pass)
                        //{
                        DialogResult drt = MessageBox.Show(tipContent + "\n" + "确认修改该固定资产？", "提示", MessageBoxButtons.OKCancel);
                        if (drt == DialogResult.OK)
                        {
                            update_Modify();    //修改更新数据库
                            saveModifyRecord();  //保存修改记录
                            modifiedContent = "修改了:";
                            tipContent = "修改内容:" + "\n\n";
                        }
                        else  //修改了内容也通过了验证但是点了"取消"
                        {
                            flag_pass = false;
                            flag_ismodified = false;
                            modifiedContent = "修改了:";
                            tipContent = "修改内容:" + "\n\n";
                        }
                        //}
                        //else      //修改了内容但是没通过验证
                        //{
                        //    flag_ismodified = false;
                        //    modifiedContent = "修改了:";
                        //    tipContent = "修改内容:" + "\n\n";
                        //}
                    }
                    else if (flag_add == false && flag_ismodified == false)   //如果不是新增状态但是没有数据被修改过
                    {
                        flag_pass = false;
                        MessageBox.Show("没有修改的内容！");
                    }
                }

            }
            catch (Exception ex)
            {
                modifiedContent = "";
                tipContent = "";
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem7_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr["资产类型"].ToString() == "模具")
                {
                    string sql = string.Format("select * from 模具保养记录 where 模具编号 = '{0}'", dr["资产编码"].ToString());
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strConn);
                    if (dt.Rows.Count == 0)
                    {
                        dr.Delete();
                    }
                    else if (dt.Rows.Count > 0)
                    {
                        MessageBox.Show("该条记录不能删除！");
                        return;
                    }
                }
                if (dr["资产类型"].ToString() == "计量器具")
                {
                    string sql = string.Format("select * from 计量器具明细卡表 where 计量器具编号 ='{0}'", dr["资产编码"].ToString());
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strConn);
                    if (dt.Rows.Count == 0)
                    {
                        dr.Delete();
                    }
                    else if (dt.Rows.Count > 0)
                    {
                        MessageBox.Show("该条记录不能删除！");
                        return;
                    }
                }


                if (dr["资产类型"].ToString() == "列管资产")
                {
                    string sql = string.Format("select * from 固定资产信息表 where 资产编码 ='{0}'", dr["资产编码"].ToString());
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strConn);
                    if (dt.Rows.Count == 0)
                    {
                        dr.Delete();
                    }
                    else if (dt.Rows.Count > 0)
                    {
                        MessageBox.Show("该条记录不能删除！");
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            loadDT();
        }

        private void barLargeButtonItem2_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_新增();
        }

        private void barLargeButtonItem6_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                gc.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        string cfgfilepath = "";
        private void frm_固定资产台账_Load(object sender, EventArgs e)
        {


            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(panel3, this.Name, cfgfilepath);
            //this.WindowState = FormWindowState.Maximized;
            loadDT();


            Initialize();
            flag_add = true;   //初始化界面时默认为新增状态
        }

        private void comboBox9_TextChanged_1(object sender, EventArgs e)
        {


            if (comboBox9.Text != "")
            {
                //comboBox1 = " ";
                string sql_大类 = string.Format("select 属性值,POS from 基础数据基础属性表 where 属性类别 = '固定资产类别'  and 属性字段2='{0}' order by POS ", comboBox9.Text.ToString());

                dt_大类 = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(sql_大类, strConn))
                {
                    da.Fill(dt_大类);
                }
                comboBox1.DataSource = dt_大类;
                comboBox1.ValueMember = "属性值";
                comboBox1.DisplayMember = "属性值";
                comboBox1.SelectedIndex = -1;
            }


        }

        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }

        }

        private void 对应关系ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow drM = (this.BindingContext[gc.DataSource].Current as DataRowView).Row;

            对应关系维护 a = new 对应关系维护(drM);


            a.ShowDialog();

        }

        private void 文件上传ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DataRow drM = (this.BindingContext[gc.DataSource].Current as DataRowView).Row;

            固定资产文件上传 a = new 固定资产文件上传(drM);


            a.ShowDialog();
        }

        private void 修改日志ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            modifiedLog fm = new modifiedLog(dr["资产编码"].ToString(), dr["资产名称"].ToString());
            fm.StartPosition = FormStartPosition.CenterScreen;    //跳出窗体时屏幕居中显示
            fm.ShowDialog();

        }

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {


            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dt_固定资产].EndCurrentEdit();

                flag_add = false;   //新增状态为false
                setReadOnly();
                dataBindHelper1.DataFormDR(gv.GetDataRow(gv.FocusedRowHandle));   //传参
                rowToForm();       //传参
                rowToTemp();   //将各个字段值传到temp变量中
                textBox12.Text = residualLife();   //计算剩余寿命
                textBox10.Text = zheJiu();  //计算累计折旧
                textBox11.Text = jingZhi();  //计算净值
                textBox21.Text = benQiZheJiu(); //计算本期折旧

            }
            catch (Exception)
            {

                throw;
            }

        }

        private void gv_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            对应关系ToolStripMenuItem.Visible = true;
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc, new System.Drawing.Point(e.X, e.Y));
            }

            DataRow drM = (this.BindingContext[gc.DataSource].Current as DataRowView).Row;

            if (drM["大类"].ToString() != "工装治具资产")
            {

                对应关系ToolStripMenuItem.Visible = false;

            }


        }

        private void searchLookUpEdit2_EditValueChanged_1(object sender, EventArgs e)
        {
            searchLookUpEdit4.EditValue = searchLookUpEdit2.EditValue;
            textBox7.Text = searchLookUpEdit4.Text.ToString();

        }//关闭


        DataTable dt1;
        DataTable dt2;
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {



            输入框 fm = new 输入框();
            fm.ShowDialog();
            DataTable dtM_fu2 = ImportExcelToDataTable2(fm.a.ToString());
            string sql_固定EXCEL = "select * from 固定资产信息表 where 1<>1 ";
            DataTable dt_excel = CZMaster.MasterSQL.Get_DataTable(sql_固定EXCEL, strConn);


            DataTable dt_excel导入 = dtM_fu2.Clone();


            foreach (DataRow d3 in dtM_fu2.Rows)
            {

                DataRow d4 = dt_excel导入.NewRow();
                dt_excel导入.Rows.Add(d4);
                d4["领用日期"] = d3["领用日期"];
                d4["领用人"] = d3["领用人"];
                d4["用途"] = d3["用途"];
                d4["领用部门"] = d3["领用部门"];
                d4["楼层"] = d3["楼层"];
                d4["地点"] = d3["地点"];
                d4["状态"] = d3["状态"];
                d4["资产类别"] = d3["资产类别"];
                d4["资产号码"] = d3["资产号码"];
                d4["品牌"] = d3["品牌"];
                d4["型号"] = d3["型号"];
                d4["CPU"] = d3["CPU"];

                d4["大类"] = d3["大类"];

                d4["内存"] = d3["内存"];
                d4["硬盘"] = d3["硬盘"];
                d4["序列号"] = d3["序列号"];
                d4["调配记录"] = d3["调配记录"];


            }




            foreach (DataRow dr in dt_excel导入.Rows)
            {
                string sql_大类 = string.Format("select * from 基础数据基础属性表 where 属性类别 = '固定资产类别' and 属性值 = '{0}' order by POS", dr["大类"].ToString());
                DataTable dt_属性 = CZMaster.MasterSQL.Get_DataTable(sql_大类, strConn);
                string s_资产编码 = string.Format("{0}{1}", dt_属性.Rows[0]["属性字段1"].ToString(), CPublic.CNo.fun_得到最大流水号(dt_属性.Rows[0]["属性字段1"].ToString()).ToString("0000"));
                DataRow dtp = dt_excel.NewRow();
                dt_excel.Rows.Add(dtp);
                dtp["GUID"] = Guid.NewGuid();
                if (dr["领用日期"].ToString() == "")
                {
                    dtp["使用日期"] = CPublic.Var.getDatetime();
                }
                else
                {
                    dtp["使用日期"] = Convert.ToDateTime(dr["领用日期"].ToString());
                }
                dtp["资产编码"] = s_资产编码;
                dtp["大类"] = dr["大类"];
                dtp["资产名称"] = dr["资产类别"].ToString();
                dtp["型号"] = dr["品牌"].ToString() + "-" + dr["型号"].ToString() + "-" + dr["CPU"].ToString() + "-" + dr["内存"].ToString() + "-" + dr["硬盘"].ToString();
                dtp["入账日期"] = Convert.ToDateTime(dtp["使用日期"].ToString());
                if (dr["状态"].ToString() == "闲置")
                {
                    dtp["使用状态"] = "封存";
                }
                else
                {
                    dtp["使用状态"] = "在用";
                }
                dtp["使用人"] = dr["领用人"];
                dtp["使用部门"] = dr["领用部门"];
                string sql = string.Format("select * from  人事基础部门表 where 部门名称='{0}'", dr["领用部门"]);
                dt1 = new DataTable();
                dt1 = CZMaster.MasterSQL.Get_DataTable(sql, strConn);
                DataRow dtm = dt1.Rows[0];
                dtp["部门编号"] = dtm["部门编号"];
                dtp["存放地点"] = dr["楼层"] + "-" + dr["地点"];
                dtp["数量"] = 1;
                dtp["资产类型"] = "列管资产";
                dtp["负责人"] = dr["领用人"];
                sql = string.Format("select 员工号,部门 from 人事基础员工表 where 在职状态 = '在职'and 姓名='{0}' ", dr["领用人"].ToString());
                dt1 = new DataTable();
                dt1 = CZMaster.MasterSQL.Get_DataTable(sql, strConn);
                dtm = dt1.Rows[0];
                dtp["负责人ID"] = dtm["员工号"];
            }



            using (SqlDataAdapter da = new SqlDataAdapter(sql_固定EXCEL, strConn))
            {
                new SqlCommandBuilder(da);
                da.Update(dt_excel);

                MessageBox.Show("chenggong");
            }




        }
        public static DataTable ImportExcelToDataTable2(string path)
        {
            string conStr = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data source={0}; Extended Properties=Excel 12.0;", path);
            using (OleDbConnection conn = new OleDbConnection(conStr))
            {
                conn.Open();
                //获取所有Sheet的相关信息
                DataTable dtSheet = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
                //获取第一个 Sheet的名称
                string sheetName = dtSheet.Rows[0]["Table_Name"].ToString();
                string sql = string.Format("select * from [{0}]", sheetName);
                using (OleDbDataAdapter oda = new OleDbDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    oda.Fill(dt);
                    return dt;
                }
            }
        }

        private void searchLookUpEdit4_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                string Printer_标签 = "";
                try
                {
                    Printer_标签 = CPublic.Var.li_CFG["printer_label"].ToString();
                }
                catch (Exception)
                {

                    throw new Exception("标签打印机未配置,printer_label未找到");
                }
                string s = "";
                DevExpress.XtraGrid.Views.Base.GridCell[] gcell = gv.GetSelectedCells();
                int[] rowindex = gv.GetSelectedRows();
                List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
                foreach (int x in rowindex)
                {
                    DataRow dr = gv.GetDataRow(x);
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("zcdm", dr["资产编码"].ToString());
                    li.Add(dic);
                }

                string path =  Application.StartupPath + string.Format(@"\Mode\固定资产标签.lab");
 
                ERPproduct.Lprinter lp = new ERPproduct.Lprinter(path, li, Printer_标签, 1);
                lp.DoWork();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
























