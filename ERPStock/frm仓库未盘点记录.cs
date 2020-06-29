using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using DevExpress.XtraPrinting;
using System.IO;

namespace ERPStock
{
    public partial class frm仓库未盘点记录 : UserControl
    {
        public frm仓库未盘点记录()
        {
            InitializeComponent();
        }
        #region 成员
        string strcon = CPublic.Var.strConn;
        DataTable dtm_zong;

        string PrinterName = "";
        string cfgfilepath = "";
        DataTable dt;
        DataTable dt_仓库;

        #endregion

        private void frm仓库未盘点记录_Load(object sender, EventArgs e)
        {
            string s = "exec sync_u8_data;exec sync_u8_stockmx "; //基础数据
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);

            string sql = string.Format("select 属性值,POS from 基础数据基础属性表 where 属性类别 = '仓库类别' order by POS ");

            dt_仓库 = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                da.Fill(dt_仓库);
            }
            checkedComboBoxEdit1.Properties.DataSource = dt_仓库;
            checkedComboBoxEdit1.Properties.ValueMember = "属性值";
            checkedComboBoxEdit1.Properties.DisplayMember = "属性值";
  

           //  and 属性字段2='{0}' 


        }



        private void fun_search()
        {

//            string sql = string.Format(@"select  kc.*,规格型号,物料名称 from 仓库物料数量明细表 kc
//left join 基础数据物料信息表 base on kc.物料编码=base.物料编码
//where kc.仓库号 in (select  属性字段1 from 基础数据基础属性表  gc where gc.属性类别='仓库类别' and gc.属性值='{0}'  and gc.属性字段2='{1}')
//and  ItemId +kc.仓库号 not in (select  itemid+仓库号 from 盘点记录表 where 财务确认=0 )", checkedComboBoxEdit1.Text, comboBox1.Text);

            string sqldat = string.Format(@"select  kc.*,规格型号,物料名称 from 仓库物料数量明细表 kc
left join 基础数据物料信息表 base on kc.物料编码=base.物料编码
where kc.仓库号 in (select  属性字段1 from 基础数据基础属性表  gc   where gc.属性类别='仓库类别'");

            if (checkBox3.Checked == true)
            {
                sqldat = sqldat + string.Format("and  gc.属性字段2='{0}'", comboBox1.Text);
            }//厂区
            if (checkBox1.Checked == true)
            {
              //  sql = sql + string.Format("and  gc.属性值='{0}'",checkedComboBoxEdit1.Text);

                string value = checkedComboBoxEdit1.EditValue.ToString().Trim();
                string[] arrays = value.Split(',');
                string p = "";
                for (int i = 0; i < arrays.Length;i++ )
                {
                    if(i==0){
                        p += string.Format("and  gc.属性值='{0}'", arrays[i].ToString().Trim());
                    }
                    if (i == arrays.Length-1)
                    {
                        p += string.Format("or  gc.属性值='{0}'", arrays[i].ToString().Trim());
                    }else if(i!=0&& i!=arrays.Length-1){
                  
                        p += string.Format("or  gc.属性值='{0}'", arrays[i].ToString().Trim());
                   
                    }
                   
                 
                   
                }
                // string.Format("'{0}'"
                sqldat = sqldat +p.ToString();

            }//仓库
           
      //      and  ItemId +kc.仓库号 not in (select  itemid+仓库号 from 盘点记录表 where 财务确认=0 )

            sqldat = sqldat + string.Format(")and  ItemId +kc.仓库号 not in (select  itemid+仓库号 from 盘点记录表 where 财务确认=0 )");
            using (SqlDataAdapter da = new SqlDataAdapter(sqldat, strcon))
            {
                dt = new DataTable();

                //dt = WSAdapter.webservers_getdata.wsfun.GetData_ERP(sql);
                da.Fill(dt);
            }
            gridControl1.DataSource = dt;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                //fun_check();
                fun_search();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {






        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }


    }
}
