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
    public partial class frm新增成品检验类型 : Form
    {

        #region
        string strcon = CPublic.Var.strConn;
 
        public bool flag = false;  //指示是否保存
        /// <summary>
        /// 新增对应的dl 字段
        /// </summary>
        //public string str = "";
        public string[] ss = {"","",""};

        DataTable dtM;
        #endregion


        public frm新增成品检验类型()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


            if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
            {
                flag = true;
                //str = searchLookUpEdit1.EditValue.ToString().Trim();
                ss[0] = comboBox1.Text;  //类别 是 单品 还是 大类 还是 、。、、

                if (comboBox1.Text == "单品")
                {
                    DataRow[] r = dtM.Select(string.Format("物料编码='{0}'", searchLookUpEdit1.EditValue.ToString()));
                    ss[1] = r[0]["n原ERP规格型号"].ToString();
                    ss[2] = searchLookUpEdit1.EditValue.ToString();
                }
                else
                {
                    ss[1] = searchLookUpEdit1.EditValue.ToString().Trim();
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("请规范操作");
            }
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            string sql ="";
            if (comboBox1.Text == "单品")
            {
                sql = @"select  物料编码,n原ERP规格型号 from 基础数据物料信息表 where 物料类型<>'原材料' 
  and 物料编码 not in ( select cpbh from zz_jyxm  where cpbh is not null group by cpbh)  ";
            }
            else
            {
                sql = @"select  rtrim(物料类型名称) as 大类 from  基础数据物料类型表 where 类型级别='大类'
  and 物料类型名称 not in ( select rtrim(dl) from zz_jyxm  where dl is not null group by dl)   order by 物料类型名称";
            }
            using (SqlDataAdapter da=new SqlDataAdapter (sql,strcon))
            {

                dtM = new DataTable();
                da.Fill(dtM);

                searchLookUpEdit1.Properties.DataSource = dtM;
                searchLookUpEdit1View.PopulateColumns();
                searchLookUpEdit1.Properties.DisplayMember = dtM.Columns[0].ColumnName;
                searchLookUpEdit1.Properties.ValueMember = dtM.Columns[0].ColumnName;


            }

        }
    }
}
