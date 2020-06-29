using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPSale
{
    public partial class ui维护配件包 : UserControl
    {
        public ui维护配件包()
        {
            InitializeComponent();
        }
        #region 变量


        DataTable dt_配件,dt_物料,dt_明细;
        string strcon = CPublic.Var.strConn;


        #endregion

        private void ui维护配件包_Load(object sender, EventArgs e)
        {
            string sql = string.Format("select 物料名称,物料编码,规格型号,物料等级,存货分类,存货分类编码  from 基础数据物料信息表 ");
            dt_物料 = new DataTable();
            dt_物料 = CZMaster.MasterSQL.Get_DataTable(sql,strcon);
            repositoryItemSearchLookUpEdit1.DataSource = dt_物料;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";


            sql = string.Format("select * from 配件包 ");
            dt_配件 = new DataTable();
            dt_配件 = CZMaster.MasterSQL.Get_DataTable(sql,strcon);
            if(dt_配件.Rows.Count>0){
             
                DataView dv = new DataView(dt_配件);
                DataTable dt2 = dv.ToTable(true, "父项编码", "配件包名称");
                //comboBox1.DataSource = dt2;
                //comboBox1.ValueMember = "父项编码";
                //comboBox1.DisplayMember = "配件包名称";
                DataRow dr = dt2.NewRow();
                dr[0] = "0";
                dr[1] = "请选择";
                //插在第一位
                dt2.Rows.InsertAt(dr, 0);
                
                comboBox1.DataSource = dt2;
                comboBox1.DisplayMember = "配件包名称";
                comboBox1.ValueMember = "父项编码";

                textBox2.Text = comboBox1.SelectedValue.ToString();


            }
            //sql = string.Format("select * from 配件包  where 1 <>1");
            //dt_明细 = new DataTable();
            //dt_明细 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           
            try
            {
                if (comboBox1.Text != "" && textBox2.Text != "")
                {
                    DataRow dr = dt_明细.NewRow();
                    dt_明细.Rows.Add(dr);
                    dr["父项编码"] = textBox2.Text.Trim();
                    dr["配件包名称"] = comboBox1.Text.Trim();

                }
                else
                {
                    throw new Exception("输入完整信息");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

           


        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView2.CloseEditor();
                this.BindingContext[dt_明细].EndCurrentEdit();
                if (dt_明细.Rows.Count <= 0)
                {
                    throw new Exception("当前无数据");
                }
              
                foreach(DataRow dr in  dt_明细.Rows ){
                    if (dr.RowState == DataRowState.Deleted)
                    {

                        continue;
                    }

                }


                using (SqlDataAdapter da = new SqlDataAdapter("select *  from  配件包 where 1<>1", strcon))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt_明细);
                    MessageBox.Show("保存成功");
                    dt_明细.AcceptChanges();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
           // bool==true


          
            if (comboBox1.Text != "")
            {
                //textBox2.Text = comboBox1.SelectedValue.ToString();
                DataRow[] dr = dt_配件.Select(string.Format("配件包名称='{0}'", comboBox1.Text));
              //  dr_包装 = dt_包装方式.Select(string.Format("包装方式='{0}'", dr["cDefine22"].ToString()));

                if (dt_配件.Rows.Count > 0 && dr.Length > 0)
                {



                    string sql = string.Format("select * from 配件包 where 配件包名称='{0}'", comboBox1.Text.ToString());
                    dt_明细 = new DataTable();
                    dt_明细 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    if (dt_明细.Rows.Count > 0)
                    {
                        gridControl2.DataSource = dt_明细;
                        textBox2.Text = comboBox1.SelectedValue.ToString();
                    }
                    else
                    {
                        string s2ql = "select * from 配件包 where 1<>1";
                        dt_明细 = new DataTable();
                        dt_明细 = CZMaster.MasterSQL.Get_DataTable(s2ql, strcon);
                        gridControl2.DataSource = dt_明细;
                    }

                }
                else
                {
                    string s2ql = "select * from 配件包 where 1<>1";
                    dt_明细 = new DataTable();
                    dt_明细 = CZMaster.MasterSQL.Get_DataTable(s2ql, strcon);
                    gridControl2.DataSource = dt_明细;
                }




            }



        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow drg = (this.BindingContext[gridControl2.DataSource].Current as DataRowView).Row;
                if (drg == null)
                {
                    throw new Exception("未选中任意行不可删除");
                }
                drg.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
  
        }

        private void repositoryItemSearchLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
           DataRow drM = (this.BindingContext[gridControl2.DataSource].Current as DataRowView).Row;


           DataRow[] dr_wl = dt_物料.Select(string.Format("物料编码='{0}'", e.NewValue.ToString()));



           if (dr_wl.Length > 0)
           {
               drM["配件名称"] = dr_wl[0]["物料名称"].ToString();
           }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ui维护配件包_Load(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void gridControl2_Click(object sender, EventArgs e)
        {

        }






    }
}
