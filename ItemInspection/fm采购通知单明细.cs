using CPublic;
using CZMaster;
using System;
using System.Data;
using System.Windows.Forms;

namespace ItemInspection
{
    public partial class fm采购通知单明细 : Form
    {
        #region 用户变量

        public DataRow Dr;
        public string strTZJno;

        private DataRow drQ_通知单;
        private DataTable dtP;
        private DataTable dtM;

        private string strWLConn = CPublic.Var.strConn;
        //private string strWLConn = Var.geConn("WL");

        #endregion 用户变量

        #region 类自用

        public fm采购通知单明细()
        {
            InitializeComponent();
        }

        private void fm采购通知单明细_Load(object sender, EventArgs e)
        {
            string sql;
            sql = string.Format("select * from 采购记录采购送检单主表 where 送检单号='{0}'", strTZJno);
            drQ_通知单 = MasterSQL.Get_DataRow(sql, strWLConn);

            sql = string.Format(" select 采购记录采购送检单明细表.*,基础数据物料信息表.物料名称,基础数据物料信息表.规格型号 from 采购记录采购送检单明细表,基础数据物料信息表 where 采购记录采购送检单明细表.物料编码=基础数据物料信息表.物料编码 and 送检单号='{0}'", strTZJno);
            dtP = MasterSQL.Get_DataTable(sql, strWLConn);

            sql = "select * from 采购记录采购单检验明细表 where 1<>1";
            dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);

            txtDWBH.Text = drQ_通知单["供应商ID"].ToString().Trim();

            try
            {
                dataBindHelper1.DataFormDR(MasterSQL.Get_DataRow(string.Format("select * from 采购供应商表 where  供应商ID = '{0}'", txtDWBH.Text), strWLConn));
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog("采购通知单明细" + ex.Message);
            }

            fun_CountSL();

            gcM.DataSource = dtP;
            gvM.ViewCaption = "采购送检单明细";
        }

        #endregion 类自用

        #region 数据库操作

        private void fun_CountSL()
        {
            //dr_传.Columns.Add("批次数量");
            dtP.Columns.Add("已检数量");
            dtP.Columns.Add("待检数量");
            foreach (DataRow r in dtP.Rows)
            {
                Decimal yjsl = fun_得到已检数量(strTZJno, r["物料编码"].ToString());//已检数量
                Decimal sjsl = Decimal.Parse(r["采购数量"].ToString()) - yjsl;//送检数量
                int djsl = fun_加载样本数(sjsl);

                r["已检数量"] = yjsl;
                r["待检数量"] = djsl;
            }
        }

        #endregion 数据库操作

        #region 数据处理

        private Decimal fun_得到已检数量(string strTZJno, string ylbh)
        {
            string sql = string.Format("select sum(送检数量) as 送检数量  from 采购记录采购单检验主表 where 送检单号 = '{0}'and [产品编号]='{1}' group by 送检单号", strTZJno, ylbh);
            DataRow r = MasterSQL.Get_DataRow(sql, CPublic.Var.strConn);
            if (r != null)
            {
                return (Decimal)r["送检数量"];
            }
            else
            {
                return 0;
            }
        }

        private int fun_加载样本数(Decimal sjsl)
        {
            int iMax = 0;
            int iYBS = 0, iAc = 0;

            foreach (DataRow rp in dtM.Rows)
            {
                if (rp.RowState == DataRowState.Deleted) continue;

                if (rp["检验水平"].ToString() == "" && rp["AQL"].ToString() == "")
                {
                    continue;
                }

                fun_抽检样本数计算(sjsl, rp["检验水平"].ToString(), rp["AQL"].ToString(), ref iYBS, ref iAc);
                rp["抽检数"] = iYBS;
                rp["Ac"] = iAc;
                if (iYBS > iMax)
                {
                    iMax = iYBS;
                }
            }
            return iMax;
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

                DataRow drCPCY = MasterSQL.Get_DataRow(sql, Var.strConn);

                //iYBS = int.Parse(drCPCY["样本量"].ToString());
                iYBS = int.Parse(drCPCY["抽检样本量"].ToString());

                iAc = (int)drCPCY["AC"];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion 数据处理

        #region 界面相关

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            dtP.Columns.Remove("已检数量");
            dtP.Columns.Remove("待检数量");
            dtP.Columns.Remove("物料名称");
            dtP.Columns.Remove("规格型号");
            Dr = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;

            DialogResult = DialogResult.OK;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void gcM_DoubleClick(object sender, EventArgs e)
        {
            dtP.Columns.Remove("已检数量");
            dtP.Columns.Remove("待检数量");
            dtP.Columns.Remove("物料名称1");
            dtP.Columns.Remove("规格型号1");
            Dr = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;

            DialogResult = DialogResult.OK;
        }

        #endregion 界面相关
    }
}