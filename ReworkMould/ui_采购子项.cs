using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace ReworkMould
{
    public partial class ui_采购子项 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dt_采购子项;
        string str_产品编码;
        public ui_采购子项()
        {
            InitializeComponent();
        }

        public ui_采购子项(DataTable dt_total,string str)
        {
            InitializeComponent();
            dt_采购子项 = dt_total;
            str_产品编码 = str;
        }

        private void ui_采购子项_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(panel1, this.Name, cfgfilepath);
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
                            
        }

        private void fun_load()
        {
            string sql = string.Format(@"  with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,bom_level ) as
 (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,1 as level from 基础数据物料BOM表 
   where 产品编码='{0}'
   union all 
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型,a.数量,a.bom类型,b.bom_level+1  from 基础数据物料BOM表 a
   inner join temp_bom b on a.产品编码=b.子项编码 
   ) 
      select  子项编码 from  temp_bom a
  left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
  left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码 where base.可购 = 1 or base.委外 = 1
  group by 子项编码", str_产品编码);
            DataTable dt_子项 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            //如果没有则不需要显示
            string str = " and 1=2";
            int x = 1;
            if (dt_子项.Rows.Count != 0)
            {
                str = "and 物料编码 in (";
                foreach (DataRow dr in dt_子项.Rows)
                {

                    str += $"'{dr["子项编码"].ToString()}',";

                    if (x == dt_子项.Rows.Count)
                    {
                        str = str.Substring(0, str.Length - 1) + ")";
                    }
                    x++;
                }
            }
            DataView dv = new DataView(dt_采购子项);
            dv.RowFilter = $"停用 = 0  {str}"; 
            gc2.DataSource = dv;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ParentForm.Close();
        }
    }
}
