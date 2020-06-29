using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPSale
{
    public partial class ui销售出库视图 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string str_出库通知单号 = "";
        DataTable dtP;
        DataTable dt_mx;
        DataRow drM, dr_2;
        string s_相关单位 = "";

        public ui销售出库视图()
        {
            InitializeComponent();
        }
        public ui销售出库视图(string ssss ,string s_单位)
        {
            InitializeComponent();
            str_出库通知单号 = ssss;
            s_相关单位 = s_单位;
        }



        private void barLargeButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
      
                foreach  (DataRow dr in dtP.Rows)
                {
                  dr["厂区"] = comboBox1.Text;
                 if (dateEdit1.EditValue != null)
                 {
                        dr["包装日期"] = dateEdit1.EditValue;
                 }

                    if (checkBox1.Checked == true)
                    {
                        dr["是否加急"] = true;


                    }
                    else
                    {
                        dr["是否加急"] = false;
                    }



               }






                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction cktz = conn.BeginTransaction("出库通知修改");
                try
                {
                    string sql_z = string.Format(@"select * from 销售记录销售出库通知单主表 where 1<>1 ");
                    SqlCommand cmm_0 = new SqlCommand(sql_z, conn, cktz);         
                    SqlDataAdapter da = new SqlDataAdapter(cmm_0);     
                    new SqlCommandBuilder(da);
                  

                    da.Update(dtP);
                   /// da1.Update(dt_主);


                    dtP.AcceptChanges();
                  //  dt_主.AcceptChanges();
                    cktz.Commit();
                    MessageBox.Show("ok");
                }
                catch
                {
                    cktz.Rollback();
                    throw new Exception("保存失败,请重试");
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void fun_load()
        {
            try

            {
                if (str_出库通知单号 != "")
                {

                    string sql = string.Format("select * from 销售记录销售出库通知单主表  where 出库通知单号='{0}'", str_出库通知单号);
                     dtP = CZMaster.MasterSQL.Get_DataTable(sql,strconn);


                    string sql2 = string.Format("select * from 销售记录销售出库通知单明细表 where 出库通知单号='{0}'", str_出库通知单号);
                     dt_mx = CZMaster.MasterSQL.Get_DataTable(sql2, strconn);


                    gridControl1.DataSource = dt_mx;
                    dataBindHelper1.DataFormDR(dtP.Rows[0]);
                    checkBox1.Checked =bool.Parse( dtP.Rows[0]["是否加急"].ToString());

                    textBox2.Text = s_相关单位;

                    if (dtP.Rows[0]["包装日期"].ToString()!="")
                    {
                        dateEdit1.EditValue = DateTime.Parse(dtP.Rows[0]["包装日期"].ToString());

                    }
                    comboBox1.Text = dtP.Rows[0]["厂区"].ToString();

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            


        }



        private void ui销售出库视图_Load(object sender, EventArgs e)
        {
            try

            {

                fun_load();




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void barLargeButtonItem13_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
