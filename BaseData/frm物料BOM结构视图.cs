using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CZMaster;
using System.Data.SqlClient;
using DevExpress.XtraTreeList.Nodes;


namespace BaseData
{
    public partial class frm物料BOM结构视图 : UserControl
    {
        string strcon = "";

        public frm物料BOM结构视图()
        {
            InitializeComponent();
            strcon = CPublic.Var.strConn;
        }

        #region  变量

        /// <summary>
        /// 接收树形的DT表
        /// </summary>
        DataTable dt_materialsBom;

        /// <summary>
        /// 用来计算的DT
        /// </summary>
        DataTable dt_materialsCount;

        /// <summary>
        /// 下拉框的物料的信息的DT
        /// </summary>
        DataTable dt_materialsDetial;

        /// <summary>
        /// 该物料的父项的DT
        /// </summary>
        DataTable dt_MaterialsParent;

        /// <summary>
        /// 输入的物料编码
        /// </summary>
        string strCpID = "";

        /// <summary>
        /// 标志位
        /// </summary>
        int flag = 0;

        #endregion


        #region 树形视图部分

        //查询某一物料的BOM结构
        private void fun_SearchMaterialsBom()
        {
            try
            {
                TreeListNode n = tv.AppendNode(new object[] { strCpID }, null);
                n.SetValue("产品编码结构", strCpID);
                DataRow[] dr = dt_materialsBom.Select(string.Format("产品编码='{0}'", strCpID));
                if (dr.Length > 0)
                {
                    n.SetValue("产品名称", dr[0]["产品名称"]);
                }
                n.SetValue("子项类型", dt_materialsBom.Rows[0]["子项类型"]);
                n.SetValue("BOM类型", dt_materialsBom.Rows[0]["BOM类型"]);
                n.SetValue("数量", 1);
                n.Tag = dt_materialsBom.Rows[0];
                Init(n);
                n.ExpandAll();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_SearchMaterialsBom");
                throw new Exception(ex.Message);
            }
        }

        private void Init(TreeListNode n)
        {
            try
            {
                DataRow[] t = null;
                if (flag == 0)
                {
                    t = dt_materialsBom.Select(string.Format("产品编码='{0}'", (n.Tag as DataRow)["产品编码"].ToString()));
                    flag++;
                }
                else
                {
                    t = dt_materialsBom.Select(string.Format("产品编码='{0}'", (n.Tag as DataRow)["子项编码"].ToString()));
                }
                foreach (DataRow r in t)
                {
                    TreeListNode nc = tv.AppendNode(new object[] { r["子项编码"] }, n);
                    nc.SetValue("产品编码结构", r["子项编码"]);
                    nc.SetValue("产品名称", r["子项名称"]);
                    nc.SetValue("子项类型", r["子项类型"]);
                    nc.SetValue("BOM类型", r["BOM类型"]);
                    nc.SetValue("数量", r["数量"]);
                    nc.Tag = r;
                    Init(nc);
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " Init");
                throw new Exception(ex.Message);
            }
        }

        #endregion

        //物料编码的下拉框
        private void fun_searchMaterials()
        {
            try
            {
                SqlDataAdapter da;
                string sql = string.Format("select 物料编码,物料编码,物料名称,规格型号,图纸编号,物料类型,物料等级,壳架等级,大类,小类 from 基础数据物料信息表 where 物料类型='成品' or 物料类型='半成品'");
                da = new SqlDataAdapter(sql, strcon);
                dt_materialsDetial = new DataTable();
                da.Fill(dt_materialsDetial);
                txt_materials.Properties.DataSource = dt_materialsDetial;
                txt_materials.Properties.DisplayMember = "物料编码";
                txt_materials.Properties.ValueMember = "物料编码";
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_searchMaterials");
                throw new Exception(ex.Message);
            }
        }

        //选择某一个信息
        private void txt_materials_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (txt_materials.EditValue.ToString() != "")
                {
                    DataRow[] dr = dt_materialsDetial.Select(string.Format("物料编码='{0}'", txt_materials.EditValue.ToString()));
                    if (dr.Length > 0)
                    {
                        dataBindHelper1.DataFormDR(dr[0]);
                    }

                    barButtonItem1.Caption = string.Format("\"{0}\"的详细数量", txt_materials.EditValue.ToString().Trim());
                    barButtonItem2.Caption = string.Format("\"{0}\"的详细信息", txt_materials.EditValue.ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frm物料BOM结构视图_Load(object sender, EventArgs e)
        {
            try
            {
               // CZMaster.DevGridControlHelper.Helper(this);
                txt_materials.EditValue = "";
                fun_searchMaterials();    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //计算BOM结构的物料数量
        private void fun_CalculateMaterialsCount()
        {
            try
            {
                foreach (DataRow r in dt_materialsCount.Rows)
                {
                    r["物料数量"] = (Convert.ToDecimal(r["物料数量"]) * Convert.ToDecimal(txt_shuliang.Text)).ToString(".0000");

                }
                gc_BOMchild.DataSource = dt_materialsCount;
                gv_BOMchild.Columns["节点标记"].Visible = false;
                gv_BOMchild.Columns["上级物料"].Visible = false;
                dt_materialsCount = StockCore.StockCorer.fun_物料_单_计算(strCpID, "", strcon, true);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_CalculateMaterialsCount");
                throw new Exception(ex.Message);
            }
        }

        #region  界面操作

        //查询
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                tv.ClearNodes();
                //得到树形的BOM结构
                strCpID = txt_materials.EditValue.ToString().Trim();   //物料编码
                DataSet ds = StockCore.StockCorer.fun_得到物料BOM结构(strCpID, strcon, "");
                dt_materialsBom = ds.Tables[0];
                dt_MaterialsParent = ds.Tables[3];
                if (dt_materialsBom.Rows.Count <= 0)
                    throw new Exception("该物料没有BOM结构，请重新选择或填写！");
                fun_SearchMaterialsBom();
                gc_BOM.DataSource = dt_MaterialsParent;
                //计算所需要的量
                dt_materialsCount = StockCore.StockCorer.fun_物料_单_计算(strCpID, "", strcon, true);
                gc_BOMchild.DataSource = dt_materialsCount;
                gv_BOMchild.Columns["节点标记"].Visible = false;
                gv_BOMchild.Columns["上级物料"].Visible = false;
                flag = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        //计算数量
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    decimal d = Convert.ToDecimal(txt_shuliang.Text);
                }
                catch
                {
                    throw new Exception("计算的数量应该为数字，请重新输入！");
                }

                fun_CalculateMaterialsCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //关闭
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        #endregion


        #region 界面操作：右键菜单

        //树形结构的右键菜单
        private void tv_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    tv.ContextMenuStrip = null;
                    DevExpress.XtraTreeList.TreeListHitInfo hInfo = tv.CalcHitInfo(new Point(e.X, e.Y));
                    TreeListNode node = hInfo.Node;
                    tv.FocusedNode = node;
                    if (node != null)
                    {
                        tv.ContextMenuStrip = contextMenuStrip1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //跳转到仓库物料数量明细
        private void 物料详细数量ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string c = contextMenuStrip1.SourceControl.Name;
                if (c == "gc_BOMchild")
                {
                    if (dt_materialsCount.Rows.Count > 0)
                    {
                        //DataRow r = (this.BindingContext[dt_materialsCount].Current as DataRowView).Row;
                        //ERPStock.frm仓库物料数量明细 fm = new ERPStock.frm仓库物料数量明细(r["物料编码"].ToString());
                        //CPublic.UIcontrol.AddNewPage(fm, "仓库物料数量明细");
                    }
                }
                else if (c == "gc_BOM")
                {

                    if (dt_MaterialsParent.Rows.Count > 0)
                    {
                        //DataRow r = (this.BindingContext[dt_MaterialsParent].Current as DataRowView).Row;
                        //ERPStock.frm仓库物料数量明细 fm = new ERPStock.frm仓库物料数量明细(r["产品编码"].ToString());
                        //CPublic.UIcontrol.AddNewPage(fm, "仓库物料数量明细");
                    }
                }
                else
                {
                    //string strmaterials = tv.FocusedNode.GetValue("产品编码结构").ToString().Trim();
                    //ERPStock.frm仓库物料数量明细 fm = new ERPStock.frm仓库物料数量明细(strmaterials);
                    //CPublic.UIcontrol.AddNewPage(fm, "仓库物料数量明细");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //查询物料编码的详细信息
        private void 物料详细信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string c = contextMenuStrip1.SourceControl.Name;
                if (c == "gc_BOMchild")
                {
                    if (dt_materialsCount.Rows.Count > 0)
                    {
                        DataRow r = (this.BindingContext[dt_materialsCount].Current as DataRowView).Row;
                        BaseData.frm基础数据物料信息视图 fm = new frm基础数据物料信息视图(r["物料编码"].ToString());
                        CPublic.UIcontrol.AddNewPage(fm, "物料数据信息视图");
                    }
                }
                else if (c == "gc_BOM")
                {

                    if (dt_MaterialsParent.Rows.Count > 0)
                    {
                        DataRow r = (this.BindingContext[dt_MaterialsParent].Current as DataRowView).Row;
                        BaseData.frm基础数据物料信息视图 fm = new frm基础数据物料信息视图(r["产品编码"].ToString());
                        CPublic.UIcontrol.AddNewPage(fm, "物料数据信息视图");
                    }
                }
                else
                {
                    string strmaterials = tv.FocusedNode.GetValue("产品编码结构").ToString().Trim();
                    BaseData.frm基础数据物料信息视图 fm = new frm基础数据物料信息视图(strmaterials);
                    CPublic.UIcontrol.AddNewPage(fm, "物料数据信息视图");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        //物料编码的详细数量
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (txt_materials.EditValue.ToString() != "")
                {
                    //ERPStock.frm仓库物料数量明细 fm = new ERPStock.frm仓库物料数量明细(txt_materials.EditValue.ToString());
                    //CPublic.UIcontrol.AddNewPage(fm, "仓库物料数量明细");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //物料编码的详细信息
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (txt_materials.EditValue.ToString() != "")
                {
                    BaseData.frm基础数据物料信息视图 fm = new frm基础数据物料信息视图(txt_materials.EditValue.ToString());
                    CPublic.UIcontrol.AddNewPage(fm, "物料数据信息视图");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_BOMchild_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv_BOMchild.GetFocusedRowCellValue(gv_BOMchild.FocusedColumn));
                e.Handled = true;
            }
        }















    }
}
