using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ItemInspection
{
    public partial class frm成品检验列表 : Form
    {


        #region
        DateTime t1;
        DateTime t2;
        string str_员工号;
        DataTable dtM;

        #endregion 

        public frm成品检验列表()
        {
            InitializeComponent();
        }
        public frm成品检验列表(DateTime t1,DateTime t2,string str )
        {
            InitializeComponent();
            this.t1 = t1;
            this.t2 = t2;

            str_员工号 = str;

        }
        private void funload()
        {
            string sql = string.Format(@"select 生产记录生产检验单主表.*  from 生产记录生产检验单主表
                                         left join 基础数据物料信息表 on  基础数据物料信息表.物料编码=生产记录生产检验单主表.物料编码   
             where 生产记录生产检验单主表.送检日期>='{0}' and 生产记录生产检验单主表.送检日期<='{1}' and 负责人员ID='{2}'", t1,t2,str_员工号);

            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                dtM = new DataTable();
                da.Fill(dtM);
                gc_checkdan.DataSource = dtM;
            }

        }
        private void frm成品检验列表_Load(object sender, EventArgs e)
        {
            try
            {
                funload();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void gv_checkdan_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }




    }
}
