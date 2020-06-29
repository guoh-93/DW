using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace 郭恒的DEMO
{
    public partial class 补检验记录 : Form
    {

        DataTable dt_待办;
        string strcon = CPublic.Var.strConn;
        DataTable dtP;

        string strJYDDH = "";
        public 补检验记录()
        {
            InitializeComponent();
        }

        private void 补检验记录_Load(object sender, EventArgs e)
        {
            fun_load();
        }

        private void fun_load()
        {

            string s = "select   * from  采购记录采购单检验主表 where 检验日期>'2018-4-14'  and 检验结果 ='不合格' and 关闭=0  and 检验记录单号 not in (select  检验记录单号 from 采购记录采购单检验明细表  )";
            dt_待办 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gc.DataSource = dt_待办;

            s = "select * from 采购记录采购单检验明细表 where 1<>1 ";
            dtP = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn);

        }
        private void fun_加载产品明细(string strItem)
        {
            string sql = string.Format("select * from 基础数据物料检验要求表 where 产品编码 = '{0}' order by POS", strItem);
            DataTable dtCPCY = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

            dtP.Clear();
            //foreach (DataRow r in dr_传.Rows)
            //{
            //    if(r.RowState == DataRowState.Deleted)continue;
            //    r.Delete();
            //}

            foreach (DataRow rCPCY in dtCPCY.Rows)
            {
                DataRow rP = dtP.NewRow();
                rP["GUID"] = Guid.NewGuid().ToString();
                rP["检验记录明细号"] = string.Format("{0}-{1:00}", strJYDDH, rCPCY["POS"]);
                rP["检验记录单号"] = strJYDDH;
                rP["POS"] = rCPCY["POS"];
                rP["送检数"] = 0;
                if (rCPCY["检验项目"].ToString() != "" && rCPCY["AQL"].ToString() != "")
                {
                    rP["抽检数"] = 0;
                }
                if (rCPCY["检验水平"].ToString() == "全检")
                {
                    rP["抽检数"] = 0;
                }
                rP["检验项目"] = rCPCY["检验项目"];
                rP["检验要求"] = rCPCY["检验要求"];
                rP["检验水平"] = rCPCY["检验水平"];
                rP["AQL"] = rCPCY["AQL"];
                rP["扩大值"] = rCPCY["扩大值"];
                rP["允许下限"] = rCPCY["下限值"];
                rP["允许上限"] = rCPCY["上限值"];
                rP["检验下限"] = "";
                rP["检验上限"] = "";
                //rP["合格"] = false;
                rP["合格"] = true;
                rP["备注"] = "";
                rP["不合格原因"] = "";
                rP["不合格数量"] = 0;

                dtP.Rows.Add(rP);
            }

            //dtP.ColumnChanged += dtP_ColumnChanged;
        }


        private void fun_加载样本数()
        {
            int iMax = 0;
            int iYBS = 0, iAc = 0;

            foreach (DataRow rp in dtP.Rows)
            {
                if (rp.RowState == DataRowState.Deleted) continue;

                if (rp["检验水平"].ToString() == "" && rp["AQL"].ToString() == "")
                {
                    if (rp["检验项目"].ToString().Trim() == "ROHS")  //品质 要求的 17-12
                    {
                        rp["抽检数"] = 1;

                    }
                    continue;
                }
                if (rp["检验水平"].ToString() != "全检")
                {
                    fun_抽检样本数计算(System.Decimal.Parse(txtSJSL.Text), rp["检验水平"].ToString(), rp["AQL"].ToString(), ref iYBS, ref iAc);
                    rp["抽检数"] = iYBS;
                    rp["Ac"] = iAc;
                }
                int SJSL = 0;
                if (rp["检验水平"].ToString() == "全检")
                {

                    try
                    {

                        SJSL = Convert.ToInt32(ItemInspection.MyClass.GetNumber(txtSJSL.Text.Trim()));
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    rp["抽检数"] = SJSL;
                    rp["Ac"] = 0;
                }
                if (iYBS > iMax)
                {
                    iMax = iYBS;
                }
                if (SJSL > iMax)
                {
                    iMax = SJSL;
                }
            }
            txtCJSL.Text = iMax.ToString();
        }
        private void fun_抽检样本数计算(Decimal dQty, string strYYSP, string strAQL, ref int iYBS, ref int iAc)
        {
            try
            {
                if (dQty <= 1)
                {
                    iYBS = 1;
                    iAc = 0;
                    return;
                }
                string sql = string.Format("select  基础数据检验抽样表.* from 基础数据样本量字码表,基础数据检验抽样表 where " +
                    " 基础数据样本量字码表.样本量字码 = 基础数据检验抽样表.样本量字码 and 基础数据样本量字码表.下限 <= {0} and  " +
                    "基础数据样本量字码表.上限 >= {0} and 基础数据样本量字码表.检验水平 ='{1}' and " +
                    "基础数据检验抽样表.AQL = '{2}'", dQty, strYYSP, strAQL);

                DataRow drCPCY = CZMaster.MasterSQL.Get_DataRow(sql, strcon);

                //iYBS = int.Parse(drCPCY["样本量"].ToString());
                if (drCPCY == null)
                {
                    iYBS = 0;
                    iAc = 0;
                }
                else
                {
                    iYBS = int.Parse(drCPCY["抽检样本量"].ToString());

                    iAc = (int)drCPCY["AC"];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            strJYDDH = dr["检验记录单号"].ToString();
            txtSJSL.Text = dr["送检数量"].ToString();
            txtItem.Text = dr["产品编号"].ToString();
            fun_加载产品明细(dr["产品编号"].ToString());
            fun_加载样本数();
            gcP.DataSource = dtP;

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                gvM.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                string s = "select  *  from 采购记录采购单检验明细表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
                {

                    new SqlCommandBuilder(da);
                    da.Update(dtP);
                }
                fun_load();
                dtP = dtP.Clone();
                gcP.DataSource = dtP;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
