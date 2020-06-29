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
    public partial class Ui单位换算 : UserControl
    {

        #region 
        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dt_计量单位;
        DataRow dr_cs;
        #endregion

        public Ui单位换算(DataRow dr )
        {
            InitializeComponent();
            dr_cs = dr;
        }

        private void Ui单位换算_Load(object sender, EventArgs e)
        {
            string s = "select  属性值 as 计量单位,属性字段1 as 计量单位编码 from 基础数据基础属性表 where 属性类别='计量单位' ";
            dt_计量单位= CZMaster.MasterSQL.Get_DataTable(s, strcon);
            repositoryItemSearchLookUpEdit1.DataSource = dt_计量单位;
            repositoryItemSearchLookUpEdit1.DisplayMember = "计量单位编码";
            repositoryItemSearchLookUpEdit1.ValueMember = "计量单位编码";

            textBox1.Text = dr_cs["物料编码"].ToString();
            textBox2.Text = dr_cs["物料名称"].ToString();
            textBox3.Text = dr_cs["规格型号"].ToString();
            textBox4.Text = dr_cs["计量单位"].ToString();

            fun_search();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
           DataRow r = dtM.NewRow();
           r["物料编码"] = dr_cs["物料编码"];
           r["主计量单位标识"] =false;
           dtM.Rows.Add(r);
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            dr.Delete();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "计量单位编码" && e.Value!=null)
            {
              DataRow []r= dt_计量单位.Select(string.Format("计量单位编码='{0}'",e.Value));
              DataRow xx = gridView1.GetDataRow(e.RowHandle);
              xx["计量单位"] = r[0]["计量单位"];
                    


            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                this.ActiveControl = null;

                fun_save();
                MessageBox.Show("保存成功");
                fun_search();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        private void fun_save()
        {
            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("dwhs");
            try
            {
                string s = "select  * from 计量单位换算表 where 1=2";
                SqlCommand cmd = new SqlCommand(s, conn, ts);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dtM);
                if (dtM.Rows.Count > 0)
                {
                    s = string.Format("select  * from 基础数据物料信息表 where 物料编码='{0}'", dr_cs["物料编码"]);
                    DataTable dt_base = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    dt_base.Rows[0]["单位换算标识"] = true;
                    cmd = new SqlCommand(s, conn, ts);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_base);
                }
                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw new Exception("保存失败" + ex.Message) ;
            }
          


        }
        private void fun_check()
        {
            int i_主计量 = 0;
            foreach (DataRow row in dtM.Rows)
            {
                decimal dec = 0;
                if (!decimal.TryParse(row["换算率"].ToString(),out dec))
                {
                    throw new Exception("换算率输入有误");

                }
                if(row["主计量单位标识"].Equals(true))
                {
                    i_主计量++;
                }
            }
            if (i_主计量 == 0)
            {
                throw new Exception("没有添加主计量单位的记录");
            }
            else if (i_主计量>1)
            {
                throw new Exception("添加了多条主计量单位的计量");
            }

        }
        private void fun_search()
        {
            string s = string.Format("select  * from 计量单位换算表 where 物料编码='{0}'", dr_cs["物料编码"]);
            dtM = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gridControl1.DataSource = dtM;
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }



       
    }
}
