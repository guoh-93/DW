using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace ERPpurchase
{
    public partial class frm采购检验入库明细选择 : UserControl
    {

        string strcon = "";

        public frm采购检验入库明细选择()
        {
            InitializeComponent();
            strcon = CPublic.Var.strConn;
        }

        /// <summary>
        /// 供应商编号
        /// </summary>
        string gysID = "";

        /// <summary>
        /// 供应商名称
        /// </summary>
        string gysName = "";

        /// <summary>
        /// 供应商表
        /// </summary>
        DataTable dt_GYS;

        /// <summary>
        /// 采购检验单主表
        /// </summary>
        DataTable dt_CheckDetail;

        /// <summary>
        ///回传选择的检验的物料
        /// </summary>
        public DataTable dt_returnDetail;

        /// <summary>
        /// 检验单入库比较dt
        /// </summary>
        DataTable dt_compare;

        /// <summary>
        /// 检验单入库比较的dv
        /// </summary>
        DataView dv_compare;

        /// <summary>
        /// 用作界面显示的采购检验单
        /// </summary>
        DataTable dt_CheckDetailSecond;

        DataView dv;


        public frm采购检验入库明细选择(string gys,DataTable dt)
        {
            dt_compare = dt;
            dv_compare = new DataView(dt_compare);
            gysID = gys;
            InitializeComponent();
            strcon = CPublic.Var.strConn;
        }

        //先加载界面的选择项数据
        private void fun_loadData()
        {
            try
            {
                //查找检验单的主表，检验结果为合格的所有DT。
                SqlDataAdapter da;
                dt_CheckDetail = new DataTable();
                string sql = "select * from 采购记录采购单检验主表 where 检验结果='合格'";
                da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt_CheckDetail);
                //copy一个界面显示的DT。
                dt_CheckDetailSecond = dt_CheckDetail.Copy();
                dt_CheckDetailSecond.Columns.Add("供应商");
                dt_CheckDetailSecond.Columns.Add("物料名称");
                dt_CheckDetailSecond.Columns.Add("选择",typeof(bool));
                dt_CheckDetailSecond.Columns.Add("已选择项",typeof(bool));
                //赋值物料名称和供应商的名称
                DataTable dt_new=new DataTable();
                foreach (DataRow r in dt_CheckDetailSecond.Rows)
                {
                    DataRow[] dr = dt_GYS.Select(string.Format("供应商ID='{0}'", r["供应商编号"].ToString()));
                    if (dr.Length > 0)
                        r["供应商"] = dr[0]["供应商名称"];
                    dt_new.Clear();
                    string sql_1 = string.Format("select 物料编码,物料名称 from 基础数据物料信息表 where 物料编码='{0}'", r["产品编号"].ToString());
                    da = new SqlDataAdapter(sql_1, strcon);
                    da.Fill(dt_new);
                    if (dt_new.Rows.Count > 0)
                        r["物料名称"] = dt_new.Rows[0]["物料名称"];
                }
                //如果已经选择的。已选择项为TRUE
                foreach (DataRow r in dt_compare.Rows)
                {
                    DataRow[] dr = dt_CheckDetailSecond.Select(string.Format("检验记录单号='{0}'", r["检验记录单号"]));
                    if (dr.Length > 0)
                        dr[0]["已选择项"] = true;
                }
                 
                dv = new DataView(dt_CheckDetailSecond);
                dv.RowFilter = string.Format("供应商编号='{0}' and 已选择项=false", gysID);
                gcJYD.DataSource = dv;

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_loadData");
                throw new Exception(ex.Message);
            }
        }



        private void frm采购检验入库明细选择_Load(object sender, EventArgs e)
        {
            try
            {
                devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
                devGridControlCustom1.strConn = CPublic.Var.strConn;
                //查找供应商表
                SqlDataAdapter da;
                da = new SqlDataAdapter("select * from 采购供应商表", strcon);
                dt_GYS = new DataTable();
                da.Fill(dt_GYS);
                fun_loadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// 检查选择的返回的DT
        /// </summary>
        private void fun_checkReturnDt()
        {
            try
            {
                foreach (DataRow r in dt_returnDetail.Rows)
                {
                    if (r["供应商编号"].ToString() != gysID)
                        throw new Exception(string.Format("只能选择供应商为\"{0}\"的检验单,请重新选择",gysID));
                }
            }
            catch (Exception ex)
            {
                dt_returnDetail = new DataTable();
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_checkReturnDt");
                throw new Exception(ex.Message);
            }
        }






        //回传检验的选择项
        private void fun_returndt()
        {
            try
            {
                dt_returnDetail=dt_CheckDetailSecond.Clone();  //克隆结构

                foreach (DataRow r in dt_CheckDetailSecond.Rows)
                {
                    if (r["选择"].Equals(true))
                    {
                        dt_returnDetail.Rows.Add(r.ItemArray);
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_returndt");
                throw new Exception(ex.Message);
            }
        }


        #region 界面的操作

        //确定选择项
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gvJYD.CloseEditor();
                this.BindingContext[dt_CheckDetailSecond].EndCurrentEdit();
                fun_returndt();
                fun_checkReturnDt();
                this.ParentForm.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //关闭页面
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ParentForm.Close();
        }

        //显示所有
        private void txt_checkall_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                fun_IsChecked();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //是否选中项
        private void fun_IsChecked()
        {
            try
            {
                if (txt_checkall.Checked == true)
                {
                    dv = new DataView(dt_CheckDetailSecond);
                    gcJYD.DataSource = dv;
                    this.BindingContext[dt_CheckDetailSecond].PositionChanged += frm采购检验入库明细选择_PositionChanged;
                }
                else
                {
                    fun_loadData();

                }

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_IsChecked");
                throw new Exception(ex.Message);
            }
        }


        void frm采购检验入库明细选择_PositionChanged(object sender, EventArgs e)
        {
            if (dv.Count > 0)
            {
                DataRowView r = this.BindingContext[dv].Current as DataRowView;

                if (dv_compare.Find(r["检验记录单号"]) != -1)
                {
                    gvJYD.OptionsBehavior.Editable = false;
                }
                else
                {
                    gvJYD.OptionsBehavior.Editable = true;
                }
            }
        }

        #endregion








    }
}
