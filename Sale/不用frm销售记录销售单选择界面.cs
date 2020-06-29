using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace ERPSale
{
    public partial class 不用frm销售记录销售单选择界面 : UserControl
    {
        #region 成员
        /// <summary>
        /// 装 销售订单明细
        /// </summary>
        DataTable dtM;
        string strconn = CPublic.Var.strConn;
        string str_搜索条件 = "";
        /// <summary>
        /// 装选择后的记录
        /// </summary>
        public DataTable dt_选择;
        DataView dv1;
        DataTable dt;
        #endregion

        #region 自用类
        public 不用frm销售记录销售单选择界面(string str, DataTable dtt)
        {
            InitializeComponent();
            str_搜索条件 = str;
            dt = dtt;
        }

        public 不用frm销售记录销售单选择界面(DataTable dtt)
        {
            InitializeComponent();
            dt = dtt;
        }

        private void frm销售记录销售单选择界面_Load(object sender, EventArgs e)
        {
            try
            {
                if (str_搜索条件 == "")
                {
                    fun_载入订单号(dt);
                }
                else
                {
                    fun_载入订单号();
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售记录销售单选择界面_Load");
            }
        }

        private void gv_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                if (checkBox2.Checked == true)
                {
                    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                    dv1.Sort = "销售订单明细号";
                    //DataTable t = dv1.ToTable();
                    //DataRow[] ds = t.Select(string.Format("销售订单明细号 = '{0}'", dr["销售订单明细号"].ToString()));
                    //if (ds.Length != 0)
                    if (dv1.Find(dr["销售订单明细号"].ToString()) != -1)
                    {
                        gv.OptionsBehavior.ReadOnly = false;
                    }
                    else
                    {
                        gv.OptionsBehavior.ReadOnly = true;
                    }
                }
                else
                {
                    gv.OptionsBehavior.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("卧槽，居然错了！");
            }
        }
        #endregion

        #region 界面操作
        //确定
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                fun_选择完毕();
                this.ParentForm.Close();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售记录销售单选择界面_确定");
            }
        }

        //取消
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ParentForm.Close();
        }

        //查看全部
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                gc.DataSource = dtM;
                gv.Columns["是否已选"].Visible = true;
            }
            else
            {
                gc.DataSource = dv1;
                gv.Columns["是否已选"].Visible = false;
            }
        }
        #endregion

        #region 方法
        private void fun_载入订单号()
        {
            dtM = new DataTable();
            dtM.Columns.Add("选择", typeof(Boolean));
            dtM.Columns.Add("是否已选", typeof(Boolean));
            string sql = "select * from 销售记录销售订单明细表 where 生效 = 1 and 作废 = 0 and 明细完成 = 0";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            foreach (DataRow r in dtM.Rows)
            {
                r["选择"] = false;
                r["是否已选"] = false;
                if (dt.Rows.Count != 0)
                {
                    foreach (DataRow rrr in dt.Rows)
                    {
                        if (rrr["销售订单明细号"].ToString() == r["销售订单明细号"].ToString())
                        {
                            r["是否已选"] = true;
                        }
                    }
                }
            }
            dv1 = new DataView(dtM);
            dv1.RowFilter = str_搜索条件 + " and 是否已选 = 0";
            gc.DataSource = dv1;
            gv.Columns["是否已选"].Visible = false;
        }

        private void fun_载入订单号(DataTable dt)
        {
            dtM = new DataTable();
            dtM.Columns.Add("选择", typeof(Boolean));
            dtM.Columns.Add("是否已选", typeof(Boolean));
            string sql = "select * from 销售记录销售订单明细表 where 生效 = 1 and 作废 = 0 and 明细完成 = 0 and 已计算 = 0";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            foreach (DataRow r in dtM.Rows)
            {
                r["选择"] = false;
                r["是否已选"] = false;
                if (dt.Rows.Count != 0)
                {
                    foreach (DataRow rrr in dt.Rows)
                    {
                        if (rrr["销售订单明细号"].ToString() == r["销售订单明细号"].ToString())
                        {
                            r["是否已选"] = true;
                            r["选择"] = true;
                        }
                    }
                }
            }
            gc.DataSource = dtM;
            gv.Columns["是否已选"].Visible = false;
        }

        private void fun_选择完毕()
        {
            dt_选择 = dtM.Clone();
            dt_选择.Clear();
            foreach (DataRow r in dtM.Rows)
            {
                if (r["选择"].ToString().ToLower() == "true")
                {
                    //将该行添加到成品出库明细界面中
                    dt_选择.Rows.Add(r.ItemArray);
                }
            }
          
        }
        #endregion
    }
}
