using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace BaseData
{
    public partial class frmBOM修改查询 : UserControl
    {
        string cfgfilepath = "";
        string strconn = CPublic.Var.strConn;
        string s_单号 = "";
        DataRow drM;
        DataTable dtP;
        DataTable dt_仓库;
        public frmBOM修改查询()
        {
            InitializeComponent();
        }

        public frmBOM修改查询(string s_BOM修改单号, DataRow dr, DataTable dt)
        {
            InitializeComponent();
            s_单号 = s_BOM修改单号;
            drM = dr;
            dtP = dt;
        }

        private void frmBOM修改查询_Load(object sender, EventArgs e)
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(this.panel2, this.Name, cfgfilepath);
            fun_load();
            fun_下拉框();
        }

        private void fun_下拉框()
        {
            try
            {
                dt_仓库 = new DataTable();
                string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别'";
                SqlDataAdapter da = new SqlDataAdapter(sql4, strconn);
                da.Fill(dt_仓库);
                repositoryItemSearchLookUpEdit1.DataSource = dt_仓库;
                repositoryItemSearchLookUpEdit1.DisplayMember = "仓库号";
                repositoryItemSearchLookUpEdit1.ValueMember = "仓库号";
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {
            try
            {
                textBox1.Text = drM["产品编码"].ToString();
                textBox2.Text = drM["产品名称"].ToString();
                textBox3.Text = drM["规格型号"].ToString();
                //19-7-30 根据 单号重新获取数据
                string s = string.Format("select  * from 基础数据BOM修改明细表  where BOM修改单号='{0}' order by BOM修改明细号", s_单号);
                dtP = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                gc.DataSource = dtP;
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message); 
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (e.Column.FieldName == "仓库号")
                {
                    dr["仓库号"] = e.Value;
                    DataRow[] ds = dt_仓库.Select(string.Format("仓库号 = '{0}'", dr["仓库号"]));
                    dr["仓库名称"] = ds[0]["仓库名称"];
                    ////dr["仓库名称"] = sr["仓库名称"].ToString();
                    //string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
                    //SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    //DataTable dt_物料数量 = new DataTable();
                    //da.Fill(dt_物料数量);
                    //if (dt_物料数量.Rows.Count == 0)
                    //{
                    //    dr["库存总数"] = 0;
                    //    // dr["有效总数"] = 0;
                    //}
                    //else
                    //{
                    //    dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
                    //    //  dr["有效总数"] = dt_物料数量.Rows[0]["有效总数"];
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = null;
                string s = "select  * from 基础数据BOM修改明细表 where 1=2";
                using (SqlDataAdapter da = new SqlDataAdapter(s, strconn))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dtP);
                    dtP.AcceptChanges();
                    MessageBox.Show("保存成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                
            }
        }
    }
}
