using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using CZMaster;


namespace ERPpurchase
{
    public partial class frm采购单入库明细选择 : UserControl
    {

        #region   变量

        /// <summary>
        /// 数据库连接串
        /// </summary>
        string strcon = "";

        DataTable dt_明细副本;

        /// <summary>
        /// 选择入库回传的DT
        /// </summary>
        public DataTable dt_回传;

        /// <summary>
        /// 已经选择的入库明细dt，用作比较
        /// </summary>
        DataTable dt_compare;

        /// <summary>
        /// 已经选择的入库明细的DV视图
        /// </summary>
        DataView dv_compare;

        //DataTable dt_checkDetail;

        string cangkuid = "";

        DataView dv;

        string gysid = "";



        #endregion


        public frm采购单入库明细选择(string ckid,string id,DataTable dt)
        {
            cangkuid = ckid;
            gysid = id;
            dt_compare = dt;
            dv_compare = new DataView(dt_compare);
            dv_compare.Sort= "采购单明细号";
            InitializeComponent();
            strcon = CPublic.Var.strConn;
        }


        private void fun_明细load()
        {
            try
            {
               
                    //先查询出已经生效，明细为完成，也没有作废的采购明细
                    SqlDataAdapter da;
                    DataTable dt_采购明细 = new DataTable();
                    da = new SqlDataAdapter("select * from 采购记录采购单明细表 where 生效=1 and 明细完成=0 and 作废=0", strcon);
                    da.Fill(dt_采购明细);
                    dt_明细副本 = dt_采购明细.Copy();
                    dt_明细副本.Columns.Add("选择项", typeof(bool));
                    dt_明细副本.Columns.Add("已选择项", typeof(bool));
                    foreach (DataRow r in dt_compare.Rows)  //把入库明细的dt传过来，选择过的话，就把已选择项赋值为true
                    {
                        DataRow[] dr = dt_明细副本.Select(string.Format("采购明细号='{0}'", r["采购单明细号"].ToString()));
                        if (dr.Length > 0)
                        {
                            dr[0]["已选择项"] = true;
                        }
                    }
                    dv = new DataView(dt_明细副本);
                    dv.RowFilter = string.Format("仓库号='{0}' and 供应商ID='{1}' and 已选择项=false", cangkuid, gysid);
                    gcMX.DataSource = dv;
                      
                
                

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_明细load");
                throw new Exception(ex.Message);
            }
        }

        private void frm采购单入库明细选择_Load(object sender, EventArgs e)
        {
            try
            {
               
                fun_明细load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }


        /// <summary>
        /// 检查如入库的明细是否符合入库的要求：
        /// 1.入库的明细必要求是同一个供应商
        /// 2.入库的明细必须是已生效,未完成,未作废的明细
        /// 3.未完成的量必须要大于0才能入库
        /// </summary>
        private void fun_checkReturnDt()
        {
            try
            {
                //string gysid = "";
                //if (dt_回传.Rows.Count > 0)
                //{
                //    gysid = dt_回传.Rows[0]["供应商ID"].ToString();  //获取第一个供应商的名称做比较
                //}
                foreach (DataRow r in dt_回传.Rows)
                {
                    if (gysid != r["供应商ID"].ToString())
                        throw new Exception("采购明细的供应商与筛选条件中的供应商不一致，请重新选择！");
                    if (r["生效"].Equals(false))
                        throw new Exception("未生效的明细，是不能入库的，请重新选择！");
                    if (r["作废"].Equals(true))
                        throw new Exception("作废的明细，是不能入库的，请重新选择！");
                    if (r["明细完成"].Equals(true))
                        throw new Exception("已经完成的明细，是不能入库的，请重新选择！");
                    if (Convert.ToDecimal(r["未完成数量"]) <= 0)
                        throw new Exception("明细的未完成数量小于0，不能进行入库，请重新选择！");
                }
                
            }
            catch (Exception ex)
            {
                dt_回传 = new DataTable();
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_checkReturnDt");
                throw new Exception(ex.Message);
            }
        }



        //明细项的选择查询
        private void fun_查询明细()
        {
            try
            {
                if (txt_suoyou.Checked == true)
                {
                    dv = new DataView(dt_明细副本);

                    gcMX.DataSource = dv;

                    //this.BindingContext[dv].PositionChanged += frm采购单入库明细选择_PositionChanged;
                }
                else
                {
                    fun_明细load();
                }

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_查询明细");
                throw new Exception(ex.Message);
            }
        }

        void frm采购单入库明细选择_PositionChanged(object sender, EventArgs e)
        {


        }





        #region  界面的操作

        //dt的回传：选择需要入库的明细
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gvMX.CloseEditor();
                this.BindingContext[dt_明细副本].EndCurrentEdit();
                dt_回传 = dt_明细副本.Clone();
                foreach (DataRow r in dt_明细副本.Rows)
                {
                    if (r["选择项"].Equals(true))
                    {
                        dt_回传.Rows.Add(r.ItemArray);
                    }
                }
                fun_checkReturnDt();
                this.ParentForm.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //明细选择项的查询
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_查询明细();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //取消
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //this.ParentForm.Close();
            CPublic.UIcontrol.ClosePage();
        }

        #endregion



        private void txt_suoyou_CheckedChanged(object sender, EventArgs e)
        {
            fun_查询明细();
        }

        private void gvMX_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (dv.Count > 0)
            {
                DataRowView drv = this.BindingContext[dv].Current as DataRowView;

                if (dv_compare.Find(drv.Row["采购明细号"].ToString()) != -1)
                {
                    gvMX.OptionsBehavior.Editable = false;

                }
                else
                {

                    gvMX.OptionsBehavior.Editable = true;
                }
            }
        }

       








    }
}
