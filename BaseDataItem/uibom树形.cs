using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraTreeList.Nodes;
using System.Data.SqlClient;
namespace BaseData
{
    public partial class uibom树形 : UserControl
    {
        DataTable dt_物料;
        DataTable dt_Bom;
        DataTable dt_仓库;

        string str_wl = "";
        string str_关联单号 = "";

        string strcon = CPublic.Var.strConn;
        public uibom树形(string s, string s_单号)
        {
            InitializeComponent();
            str_wl = s;
            str_关联单号 = s_单号;
        }
        private void newfun_tree()
#pragma warning restore IDE1006 // 命名样式
        {
            string s = string.Format("select  * from 基础数据物料信息表 where 物料编码='{0}' ", str_wl);
            DataRow r = CZMaster.MasterSQL.Get_DataRow(s, strcon);
            TreeListNode head = tv.AppendNode(new object[] { r["物料编码"] }, null);
            head.SetValue("物料编码", r["物料编码"].ToString());
            head.SetValue("产品名称", r["物料名称"].ToString());
            head.SetValue("规格型号", r["规格型号"].ToString());
            head.Tag = r;
            fun_TL(head, r["物料编码"].ToString());
            head.ExpandAll();

        }
        private void fun_TL(TreeListNode n, string str_fx)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                DataRow[] rr = dt_Bom.Select(string.Format("产品编码='{0}'", str_fx));

                foreach (DataRow r in rr)
                {
                    //TreeListNode nc = treeList1.AppendNode(new object[] { r["权限类型"].ToString() }, n);
                    TreeListNode nc = tv.AppendNode(new object[] { r["子项编码"].ToString() }, n);
                    // nc.SetValue("产品编码结构", r["子项编码"].ToString());
                    nc.SetValue("子项类型", r["子项类型"].ToString());
                    nc.SetValue("物料编码", r["子项编码"].ToString());
                    nc.SetValue("产品名称", r["子项名称"].ToString());
                    nc.SetValue("规格型号", r["子项规格"].ToString());
                    nc.SetValue("BOM类型", r["BOM类型"].ToString());
                    nc.SetValue("数量", Convert.ToDecimal(r["数量"]));
                    nc.SetValue("WIPType", r["WIPType"].ToString());
                    nc.SetValue("层级", r["bom_level"].ToString());
                    nc.SetValue("仓库号", r["仓库号"].ToString());
                    nc.SetValue("仓库名称", r["仓库名称"].ToString());
                    nc.SetValue("优先级", r["优先级"].ToString());
                    nc.SetValue("A面位号", r["A面位号"].ToString());

                    nc.Tag = r;
                    fun_TL(nc, r["子项编码"].ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void uibom树形_Load(object sender, EventArgs e)
        {
            try
            {
                fun_load(str_wl);

                newfun_tree();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_load(string s_wl)
        {
            string s = string.Format(@"with temp_bom(产品编码,产品名称,子项编码,子项名称,wiptype,子项类型,数量,bom类型,仓库号,仓库名称,bom_level,组,优先级,A面位号 ) as
 ( select 产品编码,产品名称,子项编码,子项名称,wiptype,子项类型,数量,bom类型,仓库号,仓库名称,2 as level,组,优先级,A面位号 from 基础数据物料BOM表 
   where 产品编码 in (select  子项编码  as level from 基础数据BOM修改明细表 where  BOM修改单号='{0}')
   union all 
   select a.产品编码,a.产品名称,a.子项编码,a.子项名称,a.wiptype,a.子项类型,a.数量,a.bom类型,a.仓库号,a.仓库名称,b.bom_level+1,a.组,a.优先级,a.A面位号  from 基础数据物料BOM表 a
   inner join temp_bom b on a.产品编码=b.子项编码 
   where bom_level>1
   )
 select '' as BOM修改明细号, temp_bom.*,base.规格型号 as 子项规格,bbbz.文件名 from temp_bom
 left join 基础数据物料信息表 base  on base.物料编码=temp_bom.子项编码 
 left join (select  xx.物料号,版本,文件名  from 程序版本维护表 xx
              inner join (select  物料号,MAX(版本)maxbb from 程序版本维护表 where 停用=0  group by 物料号)y on xx.物料号=y.物料号 and xx.版本=y.maxbb
              where xx.停用=0) bbbz on bbbz.物料号 = temp_bom.子项编码
 
 union all
 select  BOM修改明细号,产品编码,产品名称,子项编码,子项名称,wiptype,子项类型,数量,bom类型,a.仓库号,a.仓库名称,1 as bom_level,a.组,a.优先级,A面位号,base.规格型号 as 子项规格,bbbz.文件名   from 基础数据BOM修改明细表 a
  left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
   left join  (select  xx.物料号,版本,文件名  from 程序版本维护表 xx
              inner join (select  物料号,MAX(版本)maxbb from 程序版本维护表 where 停用=0  group by 物料号)y on xx.物料号=y.物料号 and xx.版本=y.maxbb
              where xx.停用=0)  bbbz on bbbz.物料号 = a.子项编码
  where  BOM修改单号='{0}' order by BOM修改明细号 ", str_关联单号);
            dt_Bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
             
            s = "select   属性字段1 as 仓库号,属性值 as 仓库名称 from [基础数据基础属性表] where 属性类别='仓库类别' and 布尔字段4 = 1";
            dt_仓库 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            repositoryItemGridLookUpEdit1.DataSource = dt_仓库;
            repositoryItemGridLookUpEdit1.DisplayMember = "仓库号";
            repositoryItemGridLookUpEdit1.ValueMember = "仓库号";

            s = "select 属性值 as 领料类型 from 基础数据基础属性表 where 属性类别 = 'WIPType'";
            DataTable dt_领料类型 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            repositoryItemSearchLookUpEdit1.DataSource = dt_领料类型;
            repositoryItemSearchLookUpEdit1.DisplayMember = "领料类型";
            repositoryItemSearchLookUpEdit1.ValueMember = "领料类型";
        }

        private void repositoryItemGridLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            try
            {
                DataRow r = tv.Selection[0].Tag as DataRow;
                DataRow[] rr = dt_仓库.Select(string.Format("仓库号='{0}'", e.NewValue));
                if (rr.Length > 0)
                {
                    r["仓库名称"] = rr[0]["仓库名称"];
                    r["仓库号"] = e.NewValue;

                    tv.Selection[0].SetValue("仓库名称", rr[0]["仓库名称"].ToString());
                    tv.Selection[0].SetValue("仓库号", e.NewValue);


                }
            }
            catch
            {


            }
        }
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认保存？", "确认!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DataView vv = new DataView(dt_Bom);
                    vv.RowStateFilter = DataViewRowState.ModifiedCurrent;
                    DataTable dt_save = vv.ToTable();
                    string s_bom = "select  * from 基础数据物料BOM表 where 1=2 ";
                    DataTable save_bom = CZMaster.MasterSQL.Get_DataTable(s_bom, strcon);
                    string s_修改 = string.Format("select * from 基础数据BOM修改明细表 where BOM修改单号='{0}'", str_关联单号);
                    DataTable record_bom = CZMaster.MasterSQL.Get_DataTable(s_修改, strcon);
                    foreach (DataRow dr in dt_save.Rows)
                    {
                        if (Convert.ToDecimal(dr["bom_level"]) > 1) //基础数据物料BOM中 
                        {
                            string s = string.Format("select  * from 基础数据物料BOM表 where 产品编码='{0}' and 子项编码='{1}'", dr["产品编码"], dr["子项编码"]);
                            using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
                            {
                                da.Fill(save_bom);
                                DataRow[] tr = save_bom.Select(string.Format("产品编码='{0}' and 子项编码='{1}'", dr["产品编码"], dr["子项编码"]));
                                tr[0]["仓库号"] = dr["仓库号"];
                                tr[0]["仓库名称"] = dr["仓库名称"];
                                tr[0]["WIPType"] = dr["WIPType"];
                            }
                        }
                        else  //层级=1 的 在bom修改明细表 中
                        {
                            DataRow[] tr = record_bom.Select(string.Format("产品编码='{0}' and 子项编码='{1}'", dr["产品编码"], dr["子项编码"]));
                            tr[0]["仓库号"] = dr["仓库号"];
                            tr[0]["仓库名称"] = dr["仓库名称"];
                            tr[0]["WIPType"] = dr["WIPType"];
                        }
                    }
                    SqlConnection conn = new SqlConnection(strcon);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("SH"); //事务的名称
                    SqlCommand cmd1 = new SqlCommand(s_bom, conn, ts);
                    SqlCommand cmd = new SqlCommand(s_修改, conn, ts);
                    try
                    {


                        SqlDataAdapter da;
                        da = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da);
                        da.Update(save_bom);
                        da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);
                        da.Update(record_bom);

                        ts.Commit();
                        MessageBox.Show("保存成功");
                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();
                        throw new Exception("保存失败" + ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
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

        private void tv_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                TreeListNode n = tv.Selection[0];

                if (e.Control & e.KeyCode == Keys.C)
                {
                    Clipboard.SetDataObject(n.GetValue(tv.FocusedColumn));
                    e.Handled = true;
                }

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

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "导出Excel",
                Filter = "Excel文件(*.xlsx)|*.xlsx"
            };
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                this.tv.ExportToXlsx(saveFileDialog.FileName);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void repositoryItemSearchLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            try
            {
                DataRow r = tv.Selection[0].Tag as DataRow;
                r["WIPType"] = e.NewValue;
            }
            catch (Exception)
            {

                
            }
        }
    }
}
