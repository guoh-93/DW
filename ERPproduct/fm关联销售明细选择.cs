using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;

using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class fm关联销售明细选择 : Form
#pragma warning restore IDE1006 // 命名样式
    {

        #region   变量


        DataTable dt_销售记录销售订单明细表;
        //public DataTable dt_保存打钩选择;
        DataTable dt_保存打钩选择;
        public DataTable dt;


        /// <summary>
        /// 回传过来比较的dt表
        /// </summary>
      
        string sss = "";
        SqlDataAdapter da;

        DataView dv_compare;

        DataView dv;

        /// <summary>
        /// 物料编码ID,进行筛选
        /// </summary>
        string strWuliao = "";

        string strConn = CPublic.Var.strConn;

        DataTable dt_不出现;

        #endregion

        /// <summary>
        /// new
        /// </summary>
        /// <param name="dt_回传"></param>
        /// <param name="wID"></param>
        /// <param name="ss_zl"></param>
        public fm关联销售明细选择(DataTable dt_回传,string wID,string ss_zl)
        {
            dt = dt_回传;
          
            strWuliao = wID;
            sss = ss_zl;
            dv_compare = new DataView(dt);
            InitializeComponent();
        }
        /// <summary>
        /// old
        /// </summary>
        /// <param name="dt_回传"></param>
        /// <param name="wID"></param>
        public fm关联销售明细选择(DataTable dt_回传, string wID )
        {
            dt = dt_回传;
            strWuliao = wID;
     
            dv_compare = new DataView(dt);
            InitializeComponent();
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_loadData()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                dt_销售记录销售订单明细表 = new DataTable();
                //                string sql = string.Format(@"select *  from  销售记录销售订单明细表 where 销售订单明细号 not in (                          
                //   select 销售记录销售订单明细表.销售订单明细号  from 销售记录销售订单明细表,(select  生产记录生产制令子表.销售订单明细号,sum(制令数量)as 关联数量  from 生产记录生产制令表,生产记录生产制令子表 
                //   where  生产记录生产制令表.生产制令单号 =  生产记录生产制令子表 .生产制令单号 
                //    group by 生产记录生产制令子表.销售订单明细号)a   
                //   where 销售记录销售订单明细表.销售订单明细号= a.销售订单明细号 and a.关联数量>=销售记录销售订单明细表.数量 )and  明细完成=0 and 未完成数量>0 and 生效=1 and 作废=0 and 物料编码='{0}'", strWuliao);

                string sql = string.Format(@"select *  from  销售记录销售订单明细表 where 物料编码='{0}' and 生效=1 and  明细完成=0 and 作废=0  ", strWuliao);
                da = new SqlDataAdapter(sql, strConn);
                da.Fill(dt_销售记录销售订单明细表);
                dt_销售记录销售订单明细表.Columns.Add("选择", typeof(bool));
                dt_销售记录销售订单明细表.Columns.Add("已选择", typeof(bool));



                if (dt != null )
                {
                    foreach (DataRow r in dt_销售记录销售订单明细表.Rows)
                    {
                        DataRow[] dr = dt.Select(string.Format("销售订单明细号='{0}'", r["销售订单明细号"].ToString()));
                        if (dr.Length > 0)
                        {
                            r["已选择"] = 1;
                            r["选择"] = 1;

                        }
                        else
                        {
                            r["已选择"] = 0;
                            r["选择"] = 0;
                        }
                    }
                    dv = new DataView(dt_销售记录销售订单明细表);
                    dv.RowFilter = "已选择=0 ";
                }
                else
                {
                    dv = new DataView(dt_销售记录销售订单明细表);
           

                }
              

                gridControl1.DataSource = dv;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_loadData");
                throw new Exception(ex.Message);
            }
        }


        //加载数据
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
                devGridControlCustom1.strConn = CPublic.Var.strConn;
                fun_loadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void check(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (checkBox1.Checked == true)
                {
                    dv = new DataView(dt_销售记录销售订单明细表);
                    gridControl1.DataSource = dv;
                    //gridControl1.DataSource = dt_销售记录销售订单明细表;
                    foreach (DataRow drr in dt_销售记录销售订单明细表.Rows)
                    {
                        if (drr["已选择"].ToString() == "True")
                        {
                            //gridView1.OptionsBehavior.Editable = false;
                            //gridControl1.Columns["选择"].ReadOnly = true;
                        }
                    }
                }
                else if (checkBox1.Checked == false)
                {
                    gridView1.CloseEditor();
                    this.BindingContext[dt_销售记录销售订单明细表].EndCurrentEdit();
                    dv = new DataView(dt_销售记录销售订单明细表);
                    dv.RowFilter = "已选择=False";
                    gridControl1.DataSource = dv;
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " check");
                throw new Exception(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        public void dvdt()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dt_销售记录销售订单明细表].EndCurrentEdit();
                dt_保存打钩选择 = dt_销售记录销售订单明细表.Clone();
                foreach (DataRow r in dt_销售记录销售订单明细表.Rows)
                {
                    if (r["已选择"].Equals(false))
                    {
                        if (r["选择"].Equals(true))
                        {
                            DataRow r_zlzb = dt.NewRow();
                            r_zlzb["GUID"] = System.Guid.NewGuid();
                            r_zlzb["生产制令单号"] = sss;
                            r_zlzb["销售订单明细号"] = r["销售订单明细号"];
                            r_zlzb["销售订单号"] = r["销售订单号"];
                            r_zlzb["物料编码"] = r["物料编码"];
                            r_zlzb["物料名称"] = r["物料名称"];
                            r_zlzb["客户"] = r["客户"];
                            r_zlzb["送达日期"] = r["送达日期"];
                            r_zlzb["规格型号"] = r["规格型号"];
                            r_zlzb["图纸编号"] = r["图纸编号"];
                            r_zlzb["数量"] = r["数量"];
                            r_zlzb["计量单位"] = r["计量单位"];
                            r_zlzb["销售备注"] = r["备注"];
                            dt.Rows.Add(r_zlzb);
                            //dt_保存打钩选择.Rows.Add(r.ItemArray);
                        }
                    }
                    else
                    {
                        if (r["选择"].Equals(false))
                        {
                            dt.Select(string.Format("销售订单明细号='{0}'", r["销售订单明细号"]))[0].Delete() ;
                        }

                    }
 
                }
           
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " dvdt");
                throw new Exception(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.Close();
        }

        //确定按钮
#pragma warning disable IDE1006 // 命名样式
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dt_销售记录销售订单明细表].EndCurrentEdit();

                dvdt();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (e.FocusedRowHandle >= 0)
                {
                    DataRowView drv = this.BindingContext[dv].Current as DataRowView;
                    if (drv.Row["已选择"].Equals(true))
                    {
                        gridView1.OptionsBehavior.Editable = false;
                    }
                    else
                    {
                        gridView1.OptionsBehavior.Editable = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}
