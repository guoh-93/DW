using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace BaseData
{
    public partial class frm基础数据物料信息维护 : UserControl
    {
        #region 成员
        DataTable dtM = new DataTable();
        DataRow drM;
        SqlDataAdapter da;
        string strshow;
        int a;
        int i = 0;//新增处用
        string strconn = CPublic.Var.strConn;
        //string strconn = "Persist Security Info=True;User ID=MESSA;Password=MESSA;Initial Catalog=ERPDB;Data Source=218.244.150.177";
        #endregion

        #region 界面
        public frm基础数据物料信息维护()
        {
            InitializeComponent();
        }

        private void frm基础数据物料信息维护_Load(object sender, EventArgs e)
        {
            fun_载入(); 
            //管理员删除按钮 never 出现
            barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }
        #endregion

        #region 数据操作
        public void fun_载入()
        {
            try
            {
                //dtM = new DataTable();
                string sql = "select * from 基础数据物料信息扩展属性表 order by POS";
                da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Fill(dtM);
                gcM.DataSource = dtM;
            }
            catch { }
        }
        
        public void fun_新增()
        {
            drM = dtM.NewRow();
            dtM.Rows.Add(drM);
            i = 1;
        }

        public void fun_保存()
        {
            {
                try          //一个小问题：必须选择其余行才能保存成功
                {   //对POS进行排序在设为1234……
                    DataView dv = new DataView(dtM);              
                    dv.Sort = "POS";     
                    int j = 1;
                    foreach (DataRowView drv in dv)
                    {
                        DataRow r = drv.Row;
                        drv.Row["POS"] = j++;  
                    }

                    string sql = "select * from 基础数据物料信息扩展属性表 where 1<> 1";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    new SqlCommandBuilder(da);
                    da.Update(dtM);
                    strshow = "数据保存成功!";
                    dtM.Clear();
                    fun_载入();
                }
                catch(Exception ex) 
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void fun_删除()
        {
            //if (MessageBox.Show("确定要删除该数据吗？", "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                DataTable dt = new DataTable(); 
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                string sqll = string.Format("select * from 基础数据物料信息扩展表 where 物料属性 = '{0}'", dr["物料信息扩展属性"].ToString());
                SqlDataAdapter da = new SqlDataAdapter(sqll, strconn);
                da.Fill(dt);
                a = dt.Rows.Count;
                if (dt.Rows.Count > 0)
                {
                    strshow = "该属性存在数据，无权删除！";
                }
                else
                {
                    try
                    {
                        //DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                        string sql = string.Format("delete from 基础数据物料信息扩展属性表 where 物料信息扩展属性 = '{0}'", dr["物料信息扩展属性"].ToString());
                        SqlConnection conn = new SqlConnection(strconn);
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        cmd.Dispose();
                        gvM.DeleteRow(gvM.FocusedRowHandle);
                        strshow = "删除成功!"; 
                        dtM.Clear(); 
                        fun_载入();
                    }
                    catch { }
                }
            }
        }

        public void fun_管理员删除()  
        {
            //缺    判断是否为管理员登录，是则显示此删除按钮，否则隐藏
            //if ()
            //{
            //    button1.Enabled = true;
            //}
            //else
            //{
            //    button1.Enabled = false;
            //}
            if (MessageBox.Show("确定要删除该数据吗？", "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    //删除对应属性的数据
                    DataTable dt = new DataTable();
                    DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                    string sqll = string.Format("delete from 基础数据物料信息扩展表 where 物料属性 = '{0}'", dr["物料信息扩展属性"].ToString());
                    SqlConnection connn = new SqlConnection(strconn);
                    connn.Open();
                    SqlCommand cmdd = new SqlCommand(sqll, connn);
                    cmdd.ExecuteNonQuery();
                    connn.Close();
                    cmdd.Dispose();
                    //删除属性
                    string sql = string.Format("delete from 基础数据物料信息扩展属性表 where 物料信息扩展属性 = '{0}'", dr["物料信息扩展属性"].ToString());
                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    cmd.Dispose();
                    gvM.DeleteRow(gvM.FocusedRowHandle);
                    MessageBox.Show("删除成功!"); dtM.Clear();
                    fun_载入();
                }
                catch { }
            }
        }
        #endregion

        #region 界面操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //新增
            fun_新增();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //删除
            if (MessageBox.Show("确定要删除该数据吗？", "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                fun_删除();
                if (a > 0)
                {
                    MessageBox.Show(strshow);
                }
                else
                {
                    MessageBox.Show(strshow);
                }
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gvM.CloseEditor();
            this.BindingContext[dtM].EndCurrentEdit();
            //保存
            if (i == 0)
            {
                MessageBox.Show("请先新增再操作！");
            }
            else
            {
                if (drM["物料信息扩展属性"].ToString() == "")
                {
                    MessageBox.Show("请输入数据再保存！");
                }
                else
                {
                    fun_保存();
                    MessageBox.Show(strshow);
                }
            }
        }
        #endregion

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //管理员删除
            //fun_管理员删除();
        }

    }
}
