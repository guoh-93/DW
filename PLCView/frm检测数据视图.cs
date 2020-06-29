using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CZMaster;
using System.Data.SqlClient;


namespace PLCView
{
    public partial class frm检测数据视图 : UserControl
    {
        string strcon_PLC = "";  //自动检测数据库连接



        public frm检测数据视图()
        {
            InitializeComponent();

            strcon_PLC = CPublic.Var.geConn("PLC");

        }

        DataTable dt_zongtable;



        DataTable dtM;

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_查询();
        }


        private void fun_查询()
        {

            try
            {
                if (LA_cpsn.EditValue.ToString() == "")
                {
                    string sql = string.Format("select * from ABB检测结果动作表 where 检测标准='{0}' and 检测组POS='{1}' and 检测是否通过='{2}' order by 产品SN号,动作POS", LA_jcbz.EditValue.ToString(), LA_jcbuzhou.EditValue.ToString(), LA_jcjg.EditValue.ToString());
                    dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
                }
                else
                {
                    string sql = string.Format("select * from ABB检测结果动作表 where 检测标准='{0}' and 产品SN号='{1}' and 检测组POS='{2}' and 检测是否通过='{3}' order by 产品SN号,动作POS", LA_jcbz.EditValue.ToString(), LA_cpsn.EditValue.ToString(), LA_jcbuzhou.EditValue.ToString(), LA_jcjg.EditValue.ToString());
                    dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));

                }

                foreach (DataRow r in dtM.Rows)
                {
                    r["R1"] = Convert.ToDouble(r["R1"]) / 1000;
                }
            }
            catch
            {


            }
            gc1.DataSource = dtM;



        }


        private void frm检测数据视图_Load(object sender, EventArgs e)
        {
            try
            {
                txt_jiancebiaozhun.EditValue = "";   //检测标准
                txt_jiancejieguo.EditValue = "";    //检测结果
                txt_jiancetime1.EditValue = DateTime.Today.AddDays(-7);   //起始的时间
                txt_jiancetime2.EditValue = DateTime.Today.AddDays(1).AddSeconds(-1);  //结束时间



                LA_jcbz.EditValue = "";
                LA_cpsn.EditValue = "";
                LA_jcbuzhou.EditValue = "";
                LA_jcjg.EditValue = "";

                gv1.ShownEditor += gv1_ShownEditor;
                gc1.EditorKeyUp += gc1_EditorKeyUp;


            }
            catch
            {


            }







        }



        void gc1_EditorKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && (gv1.ActiveEditor is DevExpress.XtraEditors.MemoEdit))
            {
                gv1.CloseEditor();
                gv1.RefreshData();
                gv1.ShowEditor();
            }
        }

        void gv1_ShownEditor(object sender, EventArgs e)
        {
            if (gv1.ActiveEditor is DevExpress.XtraEditors.MemoEdit)
            {
                DevExpress.XtraEditors.MemoEdit me = gv1.ActiveEditor as DevExpress.XtraEditors.MemoEdit;
                try
                {
                    me.SelectionStart = me.Text.Length;
                }
                catch
                {
                }
            }
        }


        private void fun_search()
        {
            string strsql = "";

            if (txt_SNhao.Text != "")
            {
                strsql = strsql + string.Format(" 产品SN号 like '{0}%' and", txt_SNhao.Text);
            }

            if (txt_jiancebiaozhun.EditValue != "")
            {
                strsql = strsql + string.Format(" 检测标准='{0}' and", txt_jiancebiaozhun.EditValue);
            }

            if (txt_jiancejieguo.EditValue != "")
            {

            }

            if (txt_jiancetime1.Text != "" && txt_jiancetime2.Text != "")
            {

            }

            if (strsql != "")
            {
                strsql = " where" + strsql.Substring(0, strsql.Length - 3);
            }

            strsql = string.Format("select * from ABB检测结果总表 {0}", strsql);

            SqlDataAdapter da;
            da = new SqlDataAdapter(strsql, strcon_PLC);
            dt_zongtable = new DataTable();
            da.Fill(dt_zongtable);

             






        }



        private void simpleButton1_Click(object sender, EventArgs e)
        {

                 fun_search();


        }

    }
}
