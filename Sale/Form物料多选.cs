using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ERPSale
{

    public partial class Form物料多选 : Form
    {
        string str_wl="";
        public Form物料多选()
        {
            InitializeComponent();
        }
        //2020-5-29 根据成品选择标签和说明书
        public Form物料多选(string s)
        {
            InitializeComponent();
            str_wl = s;
        }
        public bool flag = false;  //指示是否有

        DataTable dt_物料下拉框;

        public DataTable dt_wul = null;
        string strconn = CPublic.Var.strConn;
             

        private void Form物料多选_Load(object sender, EventArgs e)
        {
            dt_物料下拉框 = new DataTable();
            string sql2 = "";
            if (str_wl == "")
            {
                 sql2 = @"select base.物料名称,新数据,base.物料编码,base.规格型号,a.仓库号,a.仓库名称,a.货架描述,base.仓库号 as 默认仓库号,base.仓库名称 as 默认仓库,自制,
                             base.计量单位,base.标准单价,n销售单价,base.特殊备注,isnull(a.有效总数,0)有效总数,isnull(a.库存总数,0)库存总数,isnull(a.在制量,0)在制量,isnull(a.受订量,0)受订量  
                             from 基础数据物料信息表 base    left  join 仓库物料数量表 a on base.物料编码 = a.物料编码 and a.仓库号=base.仓库号
                             where (base.内销= 1 or 外销=1)  and base.停用 = 0 and base.在研 = 0";
            }
            else 
            {
                //2020-5-29 根据成品选择标签和说明书 对照表
                //sql2 = "select  * from ";
            }
            dt_物料下拉框 = CZMaster.MasterSQL.Get_DataTable(sql2, strconn);
            dt_物料下拉框.Columns.Add("选择", typeof(Boolean));
            gc.DataSource = dt_物料下拉框;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                if (dt_cun.Rows.Count> 0)
                {
                    flag = true;
                    dt_wul = new DataTable();
                    dt_wul = dt_cun.Copy();


                }


                barLargeButtonItem2_ItemClick(null,null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
    }












}
