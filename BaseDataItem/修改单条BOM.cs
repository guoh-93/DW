using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace BaseData
{
    public partial class 修改单条BOM : Form
    {

        #region
        string strcon = CPublic.Var.strConn;
        DataRow r;
        public bool flag = false;  //指示是否保存
        public string str = "";
        DataTable dt_物料名称;
        DataTable dt_check;
        #endregion
  
        public 修改单条BOM()
        {
            InitializeComponent();
        }
        public 修改单条BOM(DataRow dr,DataTable dt)
        {
            this.r = dr;
            this.dt_check = dt;
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择修改后物料");


                }
                DataRow []r =dt_check.Select(string.Format("子项编码='{0}'", searchLookUpEdit1.EditValue.ToString()));
                if (r.Length > 0)
                {
                    throw new Exception("BOM中已存在所选择的物料");
                }
                if (MessageBox.Show(string.Format("确定将{0}修改为{1}？", textBox1.Text, searchLookUpEdit1.EditValue), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    flag = true;
                    str = searchLookUpEdit1.EditValue.ToString();
                    this.Close();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void 修改单条BOM_Load(object sender, EventArgs e)
        {
            textBox1.Text = r["子项编码"].ToString();
            textBox3.Text = r["图纸编号"].ToString();

            string sql = "select (物料编码) as 子项编码,(物料名称) as 子项名称,物料编码,n原ERP规格型号,货架描述,规格,大类,小类,物料属性,图纸编号,计量单位 from 基础数据物料信息表 ";//where  停用 = 0 物料类型 = '原材料' or 物料类型 = '半成品' and
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            dt_物料名称 = new DataTable();
            dt_物料名称.Columns.Add("子项编码");
            dt_物料名称.Columns.Add("子项名称");
            da.Fill(dt_物料名称);

            searchLookUpEdit1.Properties.PopupFormSize = new Size(1400, 400);
            searchLookUpEdit1.Properties.DataSource = dt_物料名称;
            searchLookUpEdit1.Properties.DisplayMember = "子项编码";
            searchLookUpEdit1.Properties.ValueMember = "子项编码";
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if(searchLookUpEdit1.EditValue!=null && searchLookUpEdit1.EditValue.ToString() !="" )
            {
                DataRow[] xx = dt_物料名称.Select(String.Format("子项编码='{0}'", searchLookUpEdit1.EditValue.ToString()));
              textBox2.Text=xx[0]["图纸编号"].ToString();
            }
        }
    }
}
