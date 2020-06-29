using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace ERPproduct
{
    public partial class ui其他占用明细 : UserControl
    {
        DataTable dtM;
        string strcon = CPublic.Var.strConn;
        string str_wl = "";
        string cfgfilepath = "";
        string str_单号 = "";
        public ui其他占用明细(string ss,string  s)
        {
            InitializeComponent();
            str_wl = ss;
            str_单号 = s;
        }

        private void fun_load(string str_wl)
        {
            /*select zl.生产制令单号,zl.物料编码 as 产成品编码,zl.物料名称 as 产成品名称,zl.制令数量,zl.预完工日期,制单人员,wl.物料编码 as 子项编码,wl.总需求数量,wl.总已领数量,wl.xx from 生产记录生产制令表 zl
            left join (select  a.*,ISNULL(总已领数量,0)总已领数量,a.总需求数量-ISNULL(总已领数量,0) as xx    from (
            select 生产制令单号,子项编码 as 物料编码,SUM(制令数量 * bom.数量)总需求数量,WIPType from 生产记录生产制令表 zl
            left join 基础数据物料BOM表 bom  on zl.物料编码 = bom.产品编码
            where 关闭 = 0 and 完成 =0  and 生产制令类型 <>'返修制令' and 子项编码 is not null and WIPType<>'入库倒冲' 
            group by 生产制令单号 ,子项编码,WIPType)a
            left join (select mx.生产制令单号,mx.物料编码,SUM(领料数量) as 总已领数量 from 生产记录生产领料单明细表 mx
            left join 生产记录生产领料单主表 zb on zb.领料出库单号=mx.领料出库单号
            where mx.生产制令单号 in (select  生产制令单号 from 生产记录生产制令表 where 关闭 = 0 and 完成 = 0  and 生产制令类型<>'返修制令')
            and 领料类型<>'生产补料'    group by mx.生产制令单号,mx.物料编码)x
            on x.生产制令单号=a.生产制令单号 and a.物料编码=x.物料编码 ) wl on zl.生产制令单号 =wl.生产制令单号 */
            string sql = string.Format(@"select  * from v_其他占用明细 where 子项编码='{0}' and 生产制令单号<>'{1}'",str_wl,str_单号);
            DataTable t = CZMaster.MasterSQL.Get_DataTable(sql,strcon);

            gridControl1.DataSource = t;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void ui其他占用明细_Load(object sender, EventArgs e)
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
                fun_load(str_wl);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
           
        }
    }
}
