using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm成品检验单入库选项 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        #region
        public DataTable dt_fh;//给张琦用的
        DataTable dtM;
        DataTable dt_1;//接收的参数
        string sql;
        string strconn = CPublic.Var.strConn;
        string str_ckh;   //接收的仓库号

        #endregion

        #region 加载
        public frm成品检验单入库选项()
        {
            InitializeComponent();
        }
        public frm成品检验单入库选项(string str_仓库号, DataTable dt)
        {
            this.dt_1 = dt;
            this.str_ckh = str_仓库号;
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm成品检验单入库选项_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_load();
        }
        #endregion

        //#region
#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            sql = "select * from 生产记录生产检验单主表";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                dtM = new DataTable();

                da.Fill(dtM);

            }
            dtM.Columns.Add("请选择", typeof(bool));
            gridControl1.DataSource = dtM;
            //DataTable dt = ERPorg.Corg.fun_hr("仓库", CPublic.Var.LocalUserID);
            //if (dt.Rows.Count > 0)
            //{
            //    str_lckh = dt.Rows[0]["仓库号"].ToString();
            //}

        }
        //起始日期
#pragma warning disable IDE1006 // 命名样式
        private void barEditItem1_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataView dv = new DataView(dtM);
            DateTime t = Convert.ToDateTime(barEditItem1.EditValue);
            dv.RowFilter = string.Format("送检日期>='{0}'", t);
            gridControl1.DataSource = dv;

        }
        //结束日期
#pragma warning disable IDE1006 // 命名样式
        private void barEditItem2_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataView dv = new DataView(gridControl1.DataSource as DataTable);

            DateTime t = Convert.ToDateTime(barEditItem2.EditValue);
            t.AddHours(23);
            t.AddMinutes(59);
            t.AddSeconds(59);
            dv.RowFilter = string.Format("送检日期<{0}", t);
            gridControl1.DataSource = dv;

        }


        //保存
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            gridView1.CloseEditor();
            this.BindingContext[dtM].EndCurrentEdit();
            try
            {
                DataTable dt = dtM.Clone();
                foreach (DataRow dr in dtM.Rows)
                {
                    if (dr["请选择"].Equals(true))
                    {
                        //    DataRow drr = dt.NewRow();
                        //    drr = dr;
                        dt.Rows.Add(dr.ItemArray);
                    }
                }
                string sql_11 = "select * from 生产记录成品入库单明细表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql_11, strconn))
                {
                    dt_fh = new DataTable();
                    da.Fill(dt_fh);
                }
                foreach (DataRow r in dt.Rows)
                {
                    DataRow rr = dt_fh.NewRow();
                    rr["成品检验单号"] = r["生产检验单号"];
                    rr["物料编码"] = r["物料编码"];
                    rr["物料名称"] = r["物料名称"];
                    rr["规格型号"] = r["规格型号"];
                    rr["图纸编号"] = r["图纸编号"];
                    rr["入库数量"] = r["合格数量"];
                    //查询默认客户
                    //string sql_kh = string.Format("select * from 基础数据物料信息表 where 物料编码='{0}'", rr["物料编码"].ToString());
                    string sql_kh = string.Format
                        (@"select  基础数据物料信息表.物料编码,基础数据物料信息表.客户,客户基础信息表.客户编号,客户基础信息表.客户名称 
                            from 基础数据物料信息表 left join 客户基础信息表 
                            on 基础数据物料信息表.客户=客户基础信息表.客户编号 
                            where 基础数据物料信息表.物料编码='{0}'",rr["物料编码"].ToString());
                    using (SqlDataAdapter da = new SqlDataAdapter(sql_kh, strconn))
                    {
                        DataTable dt_kh = new DataTable();
                        da.Fill(dt_kh);
                        if (dt_kh.Rows.Count > 0)
                        {
                            rr["客户ID"] = dt_kh.Rows[0]["客户编号"];
                            rr["客户名称"] = dt_kh.Rows[0]["客户名称"];
                        }

                    }

                    //rr[""] = r[""];
                    dt_fh.Rows.Add(rr);
                }

                this.ParentForm.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //仓库不匹配不能勾选 明细中已选的也不能选  成品入库明细表 
#pragma warning disable IDE1006 // 命名样式
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            gridView1.CloseEditor();
            this.BindingContext[dtM].EndCurrentEdit();
            DataRow r;
            string str_lckh = "";
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string sql_ckh = string.Format("select * from 基础数据物料信息表 where 物料编码='{0}'", dr["物料编码"]);
            using (SqlDataAdapter da = new SqlDataAdapter(sql_ckh, strconn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                str_lckh = dt.Rows[0]["仓库号"].ToString();
            }
            if (e.FocusedRowHandle > 0)
            {
                r = gridView1.GetDataRow(gridView1.FocusedRowHandle);

            }
            else
            {
                r = (gridControl1.DataSource as DataTable).Rows[0];
            }
            DataRow[] rr = dt_1.Select(string.Format("成品检验单号='{0}'", r["生产检验单号"].ToString()));
            if (str_lckh != str_ckh || rr.Length > 0)
            {
                gridView1.OptionsBehavior.Editable = false;
            }
            else
            {
                gridView1.OptionsBehavior.Editable = true;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

        //关闭
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.ParentForm.Close();
        }

    }
}
