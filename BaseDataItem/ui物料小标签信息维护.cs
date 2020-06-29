using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace BaseData
{
    public partial class ui物料小标签信息维护 : UserControl
    {
        public static DevExpress.XtraTab.XtraTabControl XTC;
        DataRow r_M;
        DataTable dtP;
        string strcon = CPublic.Var.strConn;

        //19-10-16 增加
        public bool bl_save = false;
       

        public ui物料小标签信息维护()
        {
            InitializeComponent();
        }
        public ui物料小标签信息维护(DataRow dr)
        {
            InitializeComponent();
            r_M = dr;
            textBox1.Text = dr["物料编码"].ToString();
            textBox2.Text = dr["物料名称"].ToString();
            textBox5.Text = dr["规格型号"].ToString();
        }
        private void fun_initialize(string str)
        {
            string s = " select   *  from lockidruleinfo";
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.geConn("BQ"));
            searchLookUpEdit1.Properties.DataSource = dt;
            searchLookUpEdit1.Properties.DisplayMember = "RuleID";
            searchLookUpEdit1.Properties.ValueMember = "RuleID";

            string sql = string.Format("select * from 基础物料标签维护信息表 where 物料编号='{0}' ", str);
            dtP = new DataTable();
            dtP = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            if (dtP.Rows.Count > 0)
            {
      
                textBox3.Text = dtP.Rows[0]["FCCID"].ToString();
                textBox4.Text = dtP.Rows[0]["产品简码"].ToString();
                textBox6.Text = dtP.Rows[0]["名称简称"].ToString();
                searchLookUpEdit1.EditValue= dtP.Rows[0]["Mac规则ID"].ToString();
            }
            else
            {
                DataRow r = dtP.NewRow();
                dtP.Rows.Add(r);
                r["物料编号"] = r_M["物料编码"];
            }
        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_check();
                fun_save();
                MessageBox.Show("保存成功");
                this.ParentForm.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ui物料小标签信息维护_Load(object sender, EventArgs e)
        {
            try
            {
  
               

                fun_initialize(r_M["物料编码"].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (XTC.TabPages.Count == 1) { }
            if (XTC.SelectedTabPage.Text == "首页") { }
            DevExpress.XtraTab.XtraTabPage xtp = null;
            try
            {
                xtp = XTC.SelectedTabPage;
                XTC.SelectedTabPageIndex = XTC.SelectedTabPageIndex - 1;
            }
            catch { }
            try
            {
                xtp.Controls[0].Dispose();
                XTC.TabPages.Remove(xtp);
                xtp.Dispose();
            }
            catch { }
        }

        private void fun_check()
        {
            if (textBox4.Text == "")
            {
                throw new Exception("产品简码为空");
            }
            if(searchLookUpEdit1.EditValue==null || searchLookUpEdit1.EditValue.ToString()=="")
            {
                throw new Exception("Mac规则ID未选择");
            }
        }
        private void fun_save()
        {
            string sql = "select * from 基础物料标签维护信息表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dtP.Rows[0]["产品简码"] = textBox4.Text;
                dtP.Rows[0]["修改人"] = CPublic.Var.localUserName;
                dtP.Rows[0]["Mac规则ID"] = searchLookUpEdit1.EditValue;
                dtP.Rows[0]["FCCID"] = textBox3.Text;
                dtP.Rows[0]["名称简称"]=  textBox6.Text;
                dtP.Rows[0]["修改时间"] = CPublic.Var.getDatetime();
                //19-10-16
          
               // new SqlCommandBuilder(da);
               // da.Update(dtP);
            }

            sql = string.Format("select * from 基础数据物料信息表 where 物料编码='{0}'", textBox1.Text);
            DataTable dt_base = CZMaster.MasterSQL.Get_DataTable(sql,strcon);
            dt_base.Rows[0]["标签打印"] = true;



            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("xbq");
            SqlCommand cmd_zb = new SqlCommand("select * from 基础数据物料信息表 where 1<>1", conn, ts);
            SqlCommand cmd_mx = new SqlCommand("select * from 基础物料标签维护信息表 where 1<>1", conn, ts);
 

            try
            {   
               SqlDataAdapter  da = new SqlDataAdapter(cmd_zb);
                new SqlCommandBuilder(da);
                da.Update(dt_base);
  
                da = new SqlDataAdapter(cmd_mx);
                new SqlCommandBuilder(da);
                da.Update(dtP);
              
                ts.Commit();
                bl_save = true;
            }
            catch (Exception ex)
            {
                ts.Rollback();
                bl_save = false;
                throw ex;
            }


        }
    }
}
