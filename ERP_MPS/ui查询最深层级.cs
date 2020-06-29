using System;
using System.Data;
using System.IO;
using System.Windows.Forms;


namespace ERP_MPS
{
    public partial class ui查询最深层级 : UserControl
    {
        string cfgfilepath = "";
        string strcon = CPublic.Var.strConn;
        DataTable dt_AddInv = new DataTable();
        DataTable t_bom = new DataTable();
        DataTable t_base = new DataTable();

        DataTable dtM;
        public ui查询最深层级()
        {
            InitializeComponent();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                fun_calu();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void ui查询最深层级_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg pz = new ERPorg.Corg();
                pz.UserLayout(this.panel1, this.Name, cfgfilepath);
                fun_load();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void fun_calu()
        {
            foreach (DataRow dr in dt_AddInv.Rows)
            {
                string s = $@" with temp_bom(产品编码, 子项编码, 仓库号, 仓库名称, wiptype, 子项类型, 数量, bom类型, bom_level ) as
         (select 产品编码, 子项编码, 仓库号, 仓库名称, WIPType, 子项类型, 数量, bom类型,1 as level from 基础数据物料BOM表
           where 子项编码 = '{dr["物料编码"].ToString()}'
           union all
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型,a.数量,a.bom类型,b.bom_level + 1  from 基础数据物料BOM表 a
     inner join temp_bom b on a.子项编码 = b.产品编码   ) 
          select MAX(bom_level) bom_level  from (
  select 产品编码 as 子项编码,fx.物料名称 as 子项名称,子项编码 as 产品编码,base.物料名称 as 产品名称,wiptype,子项类型,数量,bom类型,a.仓库号,a.仓库名称
  , bom_level,fx.规格型号 as 子项规格,fx.停用 from temp_bom a
  left  join 基础数据物料信息表 base on base.物料编码 = a.子项编码
     left  join 基础数据物料信息表 fx  on fx.物料编码 = a.产品编码 )dd  ";
                DataRow r = CZMaster.MasterSQL.Get_DataRow(s, strcon);
                if (r != null && r["bom_level"].ToString()!="")
                    dr["最深层级"] = r["bom_level"];
                else dr["最深层级"] = 0;


            }
        }

        private void infolink()
        {

            foreach (DataRow dr in dt_AddInv.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                try
                {
                    DataRow[] r = t_base.Select(string.Format("物料编码='{0}'", dr["物料编码"]));

                    dr["物料名称"] = r[0]["物料名称"].ToString();
                    dr["规格型号"] = r[0]["规格型号"].ToString();
                    dr["存货分类"] = r[0]["存货分类"].ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        private void fun_load()
        {
            string s = $@"select 物料编码,物料名称,规格型号,存货分类 from 基础数据物料信息表 ";
            t_base = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            dt_AddInv = new DataTable();
            dt_AddInv.Columns.Add("物料编码");
            dt_AddInv.Columns.Add("规格型号");
            dt_AddInv.Columns.Add("物料名称");
            dt_AddInv.Columns.Add("存货分类");
            dt_AddInv.Columns.Add("最深层级", typeof(int));

            gridControl2.DataSource = dt_AddInv;

        }

        private void gridControl2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.V)
                {
                    if (gridView2.FocusedColumn.Caption == "物料编码")
                    {
                        infolink();
                        fun_calu();
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ui查询最深层级 ui = new ui查询最深层级(); 
            CPublic.UIcontrol.Showpage(ui, "最深层级查询");
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
