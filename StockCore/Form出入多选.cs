using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StockCore
{
    public partial class Form出入多选 : Form
    {

        public bool flag = false;  //指示是否有

        DataTable dt_物料下拉框;

        public DataTable dt_wul = null;
        string strconn = CPublic.Var.strConn;


        public Form出入多选()
        {
            InitializeComponent();
        }





     

        private void barLargeButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try

            {

                flag = false;
                gv.CloseEditor();
                this.BindingContext[dt_物料下拉框].EndCurrentEdit();


                DataView dv = new DataView(dt_物料下拉框);
                dv.RowFilter = "选择 = True";
                DataTable dt_cun = new DataTable();
                dt_cun = dv.ToTable();
                if (dt_cun.Rows.Count > 0)
                {
                    flag = true;
                    dt_wul = new DataTable();
                    dt_wul = dt_cun.Copy();


                }


                barLargeButtonItem2_ItemClick_1(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void Form出入多选_Load_1(object sender, EventArgs e)
        {
            string sql = @"select base.物料编码,base.物料名称,base.规格型号,base.图纸编号,isnull(a.库存总数,0)库存总数,a.货架描述
           ,a.仓库号,a.仓库名称, base.仓库号 as 默认仓库号,base.仓库名称 as 默认仓库 
           from 基础数据物料信息表 base
            left join 仓库物料数量表 a on base.物料编码 = a.物料编码 and  base.仓库号=a.仓库号  /*where   停用=0*/";
            dt_物料下拉框 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_物料下拉框);
            dt_物料下拉框.Columns.Add("选择",typeof(bool));

            gc.DataSource = dt_物料下拉框;
        }
    }
}
