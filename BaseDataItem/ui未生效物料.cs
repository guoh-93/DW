using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace BaseData
{
    public partial class ui未生效物料 : UserControl
    {


        #region 变量

        string strcon = CPublic.Var.strConn;
        DataTable dtM;


        #endregion
        public ui未生效物料()
        {
            InitializeComponent();
        }

        private void ui未生效物料_Load(object sender, EventArgs e)
        {
            fun_load();
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
        }
        private void  fun_load()
        {
            string  s="select  * from  基础数据物料信息表 where 生效=0";

            dtM=new DataTable ();
            using (SqlDataAdapter da =new SqlDataAdapter (s,strcon))
            {
                da.Fill(dtM);
                dtM.Columns.Add("选择", typeof(bool));
                gc1.DataSource=dtM;
            }

        }
        private void fun_check()
        {
            DataView dv = new DataView(dtM);
            dv.RowFilter = "选择=1";
            if (dv.ToTable().Rows.Count == 0)
            {

                throw new Exception("未勾选任何行");
            }

        }
        private void fun_生效()
        {
            foreach (DataRow dr in dtM.Rows)
            {
                if (dr["选择"].Equals(true))
                {
                    dr["生效"] = true;
                    dr["生效时间"] = CPublic.Var.getDatetime();
                    dr["审核人"] = CPublic.Var.localUserName;
                }
            }
            string s="select  * from  基础数据物料信息表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
            {
                new SqlCommandBuilder(da);
                da.Update(dtM);

            }
        }
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv1.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                if (MessageBox.Show("确认勾选物料需要生效?", "确认!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    fun_生效();
                    MessageBox.Show("生效成功");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

            fun_load();
        }

        private void gv1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gv1.DataRowCount; i++)
            {
                gv1.GetDataRow(i)["选择"] = true;

            }
        }

 
    
    }
}
