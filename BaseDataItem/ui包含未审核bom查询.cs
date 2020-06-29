using DevExpress.XtraTreeList.Nodes;
using System;
using System.Data;
using System.Windows.Forms;

namespace BaseData
{
    public partial class ui包含未审核bom查询 : UserControl
    {
        DataTable t_下拉;
        string strcon = CPublic.Var.strConn;
        DataTable dt_bom;
        string s_cs;
        public ui包含未审核bom查询()
        {
            InitializeComponent();
        }
        public ui包含未审核bom查询(string s)
        {
            InitializeComponent();
            s_cs = s;
        }


        private void ui包含未审核bom查询_Load(object sender, EventArgs e)
        {
            try
            {
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void fun_load()
        {
            string s = @"select  a.产品编码 as 物料编码,物料名称,规格型号,大类,小类  from (
                                 select 产品编码 from 基础数据物料BOM表  
                                 union  select  产品编码 from 基础数据BOM修改主表 where 审核=0)a 
                       left join 基础数据物料信息表 base on base.物料编码=a.产品编码 group by a.产品编码 ,物料名称,规格型号,大类,小类";
            t_下拉 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            cb_物料.Properties.DataSource = t_下拉;
            cb_物料.Properties.DisplayMember = "物料编码";
            cb_物料.Properties.ValueMember = "物料编码";

            s = @" with t  as (
    select BOM修改明细号 ,a.产品编码,子项编码,数量,bom类型,优先级,组,仓库号,仓库名称,WIPType,A面位号   from 基础数据BOM修改明细表 a
    left join 基础数据BOM修改主表 b on a.BOM修改单号=b.BOM修改单号
    where b.审核=0 and b.作废=0 ) ,
    t1 as (
     select  '' as BOM修改明细号,产品编码,子项编码,数量,bom类型,优先级,组,仓库号,仓库名称,WIPType,'已审核' zt,A面位号 from 基础数据物料BOM表
     where 产品编码 not in (select 产品编码 from t)
     union select  BOM修改明细号,产品编码,子项编码,数量,bom类型,优先级,组,仓库号,仓库名称,WIPType,'未审核' zt,A面位号 from t  ) 
     select  t1.*,base.物料名称,规格型号 from   t1 
     left join 基础数据物料信息表 base on t1.子项编码=base.物料编码 order by BOM修改明细号 ";
            dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
 
            if(s_cs!="")
            {
                cb_物料.EditValue = s_cs;
                button2_Click(null, null);
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                newfun_tree(cb_物料.EditValue.ToString());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void newfun_tree(string strCpID)
        {
            string s = string.Format("select  * from 基础数据物料信息表 where 物料编码='{0}' ", strCpID);
            DataRow r = CZMaster.MasterSQL.Get_DataRow(s, strcon);
            TreeListNode head = tv.AppendNode(new object[] { r["物料编码"] }, null);
            head.SetValue("物料编码", r["物料编码"].ToString());
            head.SetValue("产品名称", r["物料名称"].ToString());
            head.SetValue("规格型号", r["规格型号"].ToString());

            head.Tag = r;
            fun_TL(head, strCpID, 1);
            head.ExpandAll();

        }
        private void fun_TL(TreeListNode n, string str_fx, int cj)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                DataRow[] rr = dt_bom.Select($"产品编码='{str_fx}'");
                foreach (DataRow r in rr)
                {
                    TreeListNode nc = tv.AppendNode(new object[] { r["子项编码"].ToString() }, n);
                    nc.SetValue("物料编码", r["子项编码"].ToString());
                    nc.SetValue("产品名称", r["物料名称"].ToString());
                    nc.SetValue("规格型号", r["规格型号"].ToString());
                    nc.SetValue("BOM类型", r["BOM类型"].ToString());
                    nc.SetValue("数量", Convert.ToDecimal(r["数量"]));
                    nc.SetValue("WIPType", r["WIPType"].ToString());
                    nc.SetValue("层级", cj);
                    nc.SetValue("仓库号", r["仓库号"].ToString());
                    nc.SetValue("仓库名称", r["仓库名称"].ToString());
                    nc.SetValue("状态", r["zt"].ToString());
                    nc.SetValue("A面位号", r["A面位号"].ToString());


                    //  nc.SetValue("停用", r["停用"].ToString());
                    nc.Tag = r;
                    fun_TL(nc, r["子项编码"].ToString(), cj + 1);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void cb_物料_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_物料.EditValue != null && cb_物料.EditValue.ToString() != "")
                {
                    DataRow[] r = t_下拉.Select($"物料编码='{cb_物料.EditValue.ToString()}'");
                    
                    textBox4.Text = r[0]["规格型号"].ToString();
                    textBox2.Text = r[0]["物料名称"].ToString();
                    textBox7.Text = r[0]["大类"].ToString();
                    textBox8.Text = r[0]["小类"].ToString();


                }
                else
                {
                    textBox4.Text = "";
                    textBox2.Text = "";
                    textBox7.Text ="";
                    textBox8.Text ="";

                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void tv_CustomDrawNodeIndicator(object sender, DevExpress.XtraTreeList.CustomDrawNodeIndicatorEventArgs e)
        {
            DevExpress.XtraTreeList.TreeList tmpTree = sender as DevExpress.XtraTreeList.TreeList;
            DevExpress.Utils.Drawing.IndicatorObjectInfoArgs args = e.ObjectArgs as DevExpress.Utils.Drawing.IndicatorObjectInfoArgs;
            if (args != null)
            {
                int rowNum = tmpTree.GetVisibleIndexByNode(e.Node) + 1;
                // this.tv.IndicatorWidth = rowNum.ToString().Length * 10 + 12;
                args.DisplayText = rowNum.ToString();
            }
        }
    }
}
