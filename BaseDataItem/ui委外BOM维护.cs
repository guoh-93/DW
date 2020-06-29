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
    public partial class ui委外BOM维护 : UserControl
    {
        public static DevExpress.XtraTab.XtraTabControl XTC;
        DataRow dr_父项;
        string strconn = CPublic.Var.strConn;
        DataTable dt_物料名称, dtM;
        public ui委外BOM维护()
        {
            InitializeComponent();
        }
        public ui委外BOM维护(DataRow dr)
        {
            InitializeComponent();
            dr_父项 = dr;
            textBox1.Text = dr["规格型号"].ToString();
            textBox2.Text = dr["物料名称"].ToString();
            textBox3.Text = dr["物料编码"].ToString();

        }
        private void fun_载入物料()
        {
            try
            {
                string sql = string.Format(@"select 产品编号,子项编号,b.物料名称 as 子项名称,[数量],b.[计量单位],[组],[优先级],b.图纸编号,kc.货架描述,kc.仓库名称,kc.仓库号,b.计量单位编码
                                    from 委外加工BOM表 a  left join 基础数据物料信息表 b on a.子项编号 = b.物料编码
                                    left join 仓库物料数量表 kc on kc.物料编码=子项编号
                                      where a.产品编号='{0}' ", dr_父项["物料编码"]);
                dtM = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                gc.DataSource = dtM;


                sql = @"select (a.物料编码) as 子项编号,(a.物料名称) as 子项名称,a.规格型号,b.货架描述,b.库存总数,b.仓库号,b.仓库名称,大类,小类,a.物料属性,a.图纸编号,a.计量单位,a.计量单位编码 
                from 基础数据物料信息表 a,仓库物料数量表 b where a.物料编码=b.物料编码 ";//where  停用 = 0 物料类型 = '原材料' or 物料类型 = '半成品' and
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                dt_物料名称 = new DataTable();
                da.Fill(dt_物料名称);
                repositoryItemSearchLookUpEdit1.PopupFormSize = new Size(1200, 400);
                repositoryItemSearchLookUpEdit1.DataSource = dt_物料名称;
                repositoryItemSearchLookUpEdit1.DisplayMember = "子项编号";
                repositoryItemSearchLookUpEdit1.ValueMember = "子项编号";


            }
            catch { }
        }

        private void ui委外BOM维护_Load(object sender, EventArgs e)
        {
            try
            {
                fun_载入物料();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        private void fun_check()
        {
            foreach (DataRow dr in dtM.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                if (dtM.Select(string.Format("子项编号='{0}'", dr["子项编号"])).Length > 1)
                {
                    throw new Exception("列表中含有重复项");
                }
                decimal dec = 0;
                try
                {
                      dec = Convert.ToDecimal(dr["数量"]);
                    
                }
                catch (Exception ex)
                {
                    throw new Exception("输入数量格式不正确");
                } 
                if(dec<=0)
                {
                    throw new Exception("数量必须大于0");

                }


            }
        }
        private void fun_save()
        {
            string s = "select  *  from  委外加工BOM表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(s, strconn))
            {
                new SqlCommandBuilder(da);
                da.Update(dtM);
            }
        }
        

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dr_父项 == null) { }
            else
            {
                DataRow dr = dtM.NewRow();
                dr["优先级"] = 1;
                dr["产品编号"] = dr_父项["物料编码"];
                dtM.Rows.Add(dr);
                gv.FocusedRowHandle = dtM.Rows.Count - 1;
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (MessageBox.Show(string.Format("是否确认删除{0}？", dr["子项编码"].ToString() + "--" + dr["子项名称"].ToString()), "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    dr.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_载入物料();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = null;
                fun_check();
                fun_save();
                MessageBox.Show("ok");

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (XTC.TabPages.Count == 1) { }
            if (XTC.SelectedTabPage.Text == "首页") { }
            DevExpress.XtraTab.XtraTabPage xtp = null;
            try
            {
                xtp = XTC.SelectedTabPage;
                XTC.SelectedTabPageIndex = XTC.SelectedTabPageIndex - 1;
            }
            catch { }
            try
            {
                xtp.Controls[0].Dispose();
                XTC.TabPages.Remove(xtp);
                xtp.Dispose();
            }
            catch { }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow d = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);

            dr["子项名称"] = d["子项名称"].ToString();
            dr["计量单位编码"] = d["计量单位编码"].ToString();
            dr["计量单位"] = d["计量单位"].ToString();
            dr["图纸编号"] = d["图纸编号"].ToString();
            dr["货架描述"] = d["货架描述"].ToString();
            dr["仓库号"] = d["仓库号"].ToString();
            dr["仓库名称"] = d["仓库名称"].ToString();
        }

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            DataRow d = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            dr["子项名称"] = d["子项名称"].ToString();
            dr["计量单位编码"] = d["计量单位编码"].ToString();
            dr["计量单位"] = d["计量单位"].ToString();
            dr["图纸编号"] = d["图纸编号"].ToString();
            dr["货架描述"] = d["货架描述"].ToString();
            dr["仓库号"] = d["仓库号"].ToString();
            dr["仓库名称"] = d["仓库名称"].ToString();
        }

      

    }
}
