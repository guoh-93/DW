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
namespace ERPSale
{
    public partial class frm项目维护 : UserControl
    {
        public frm项目维护()
        {
            InitializeComponent();
        }

        DataTable dt;
        DataRow dr;
        string strConn = CPublic.Var.strConn;
        int flag=0 ;
    
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            fun_刷新();
        }

        private void fun_cheek(){
            if(textBox1.Text==""){
                throw new Exception("项目名称为空");
            }
            if (textBox2.Text == "")
            {
                throw new Exception("项目号为空");
            }
            //if (searchLookUpEdit1.Text == "")
            //{
            //    throw new Exception("公司为空");
            //}
        }
      
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_cheek();

                DataTable dt_hetong = new DataTable();
                if (guid.ToString()=="")
                {
                    string sql2222 = string.Format("select *  from 基础信息项目管理表 where 1<>1");
                     dt_hetong = CZMaster.MasterSQL.Get_DataTable(sql2222, strConn);

                }
                else
                {
                    string sql2222 = string.Format("select *  from 基础信息项目管理表 where guid='{0}'", guid);
                     dt_hetong = CZMaster.MasterSQL.Get_DataTable(sql2222, strConn);
                }

                if (dt_hetong.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt_hetong.Rows)
                    {
                        dr["guid"] = dr["guid"].ToString();
                        dr["项目名称"] = textBox1.Text.ToString();
                        dr["项目号"] = textBox2.Text.ToString();
                        dr["客户"] = searchLookUpEdit1.Text.ToString();
                        dr["客户编号"] = searchLookUpEdit1.EditValue.ToString();
                        dr["录入人员"] = CPublic.Var.localUserName;
                        dr["录入人员ID"] = CPublic.Var.LocalUserID;
                        dr["录入日期"] = CPublic.Var.getDatetime();
                        
                              dr["项目开始日期"] = dateEdit1.EditValue;
                        dr["项目结束日期"] = dateEdit2.EditValue;

                        if (ck_结束.Checked == true)
                        {
                            dr["状态"] = "结束";
                           
                        }
                        else if (ck_暂停.Checked == true)
                        {
                            dr["状态"] = "暂停";

                        }
                        else if (ck_在研.Checked == true)
                        {
                            dr["状态"] = "在研";
                        }
                        else
                        {
                            dr["状态"] = "正常";

                        }



                    }

                    string sql2 = "select * from 基础信息项目管理表 where 1<>1";
                    using (SqlDataAdapter da = new SqlDataAdapter(sql2, strConn))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_hetong);

                        MessageBox.Show("保存成功");
                        fun_刷新();
                    }

                }
                else
                {
                    DateTime t = CPublic.Var.getDatetime();
                    //textBox2.Text = string.Format("HT{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                    //     t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("HT", t.Year, t.Month, t.Day).ToString("0000"));
                    string sql = "select * from 基础信息项目管理表 where 1<>1";
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strConn);
                    string sq2l = "select * from 基础信息项目管理表";
                 DataTable    dtp = CZMaster.MasterSQL.Get_DataTable(sq2l, strConn);
                    if (dtp.Rows.Count>0)
                    {
                        foreach (DataRow drrr in dtp.Rows)
                        {
                            if (textBox2.Text.ToString() == drrr["项目号"].ToString())
                            {
                                throw new Exception("当前项目号已存在");
                            }


                        }
                    }

                  




                    DataRow drm = dt.NewRow();
                    dt.Rows.Add(drm);
                    drm["guid"] = Guid.NewGuid();
                    drm["项目名称"] = textBox1.Text.ToString();
                    drm["项目号"] = textBox2.Text.ToString();
                    if (searchLookUpEdit1.Text.ToString()!="")
                    {
                        drm["客户"] = searchLookUpEdit1.Text.ToString();

                        drm["客户编号"] = searchLookUpEdit1.EditValue.ToString();
                    }
                 
                    drm["录入人员"] = CPublic.Var.localUserName;
                    drm["录入人员ID"] = CPublic.Var.LocalUserID;
                    drm["录入日期"] = t;


                    drm["项目开始日期"] =DateTime.Parse(dateEdit1.Text.ToString())   ;
                    drm["项目结束日期"] = DateTime.Parse(dateEdit2.Text.ToString());

                    if (ck_结束.Checked == true)
                    {
                        drm["状态"] = "结束";

                    }
                    else if (ck_暂停.Checked == true)
                    {
                        drm["状态"] = "暂停";

                    }
                    else if (ck_在研.Checked == true)
                    {
                        drm["状态"] = "在研";
                    }
                    else
                    {
                        drm["状态"] = "正常";

                    }
                    string sql2 = "select * from 基础信息项目管理表 where 1<>1";
                    using (SqlDataAdapter da = new SqlDataAdapter(sql2, strConn))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt);

                        MessageBox.Show("保存成功");
                        fun_刷新();
                    }

                }
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
    

        private void fun_刷新()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            searchLookUpEdit1.Text = "";
            dateEdit1.Text = "";
            dateEdit2.Text = "";
            ck_结束.Checked = false;
            ck_暂停.Checked = false;
            ck_在研.Checked = false;
            string sql = "select * from 基础信息项目管理表";
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strConn);
            gridControl1.DataSource = dt;
            guid = "";
        }
        private void banfun_刷新()
        {
        
            string sql = "select * from 基础信息项目管理表";
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strConn);
            gridControl1.DataSource = dt;
            guid = "";
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_刷新();
        }

        string guid;
        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            ck_结束.Checked = false;
            ck_暂停.Checked = false;
            ck_在研.Checked = false;
            DataRow   drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
         textBox1.Text = drM["项目名称"].ToString();
         textBox2.Text = drM["项目号"].ToString();
         searchLookUpEdit1.EditValue = drM["客户编号"].ToString();
            dateEdit1.Text = drM["项目开始日期"].ToString();
            dateEdit2.Text = drM["项目结束日期"].ToString();
            if (drM["状态"].ToString()== "结束")
            {
                ck_结束.Checked = true;
            }
            else if (drM["状态"].ToString() == "暂停")
            {
                ck_暂停.Checked = true;

            }


           else  if (drM["状态"].ToString() == "在研")
            {
                ck_在研.Checked = true;
            }
      


            guid = drM["guid"].ToString();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
            drM.Delete();
            if (MessageBox.Show("确认删除吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                string sql2 = "select * from 基础信息项目管理表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql2, strConn))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt);

                    MessageBox.Show("删除成功");
                }
            }
            fun_刷新();
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void frm项目维护_Load(object sender, EventArgs e)
        {
            string sql = "select * from 基础信息项目管理表";
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strConn);
            gridControl1.DataSource = dt;


            sql = "select 客户编号,客户名称,客户类型,客户简称 from 客户基础信息表";
            DataTable dt_keh = CZMaster.MasterSQL.Get_DataTable(sql, strConn);
            searchLookUpEdit1.Properties.ValueMember = "客户编号";
            searchLookUpEdit1.Properties.DisplayMember = "客户名称";
            searchLookUpEdit1.Properties.DataSource = dt_keh;
            
        }

        private void ck_暂停_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
