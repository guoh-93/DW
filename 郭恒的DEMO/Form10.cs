using DevExpress.XtraTreeList.Nodes;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace 郭恒的DEMO
{
    public partial class Form10 : Form
    {
        #region
        private DataTable dt_bom_all;
        private string strcon = CPublic.Var.strConn;
        private DataTable dt_root;

        private DataTable dtM;
        #endregion

        public Form10()
        {
            InitializeComponent();
        }

        private void Form10_Load(object sender, EventArgs e)
        {
            string s = @"select  a.原ERP物料编号 as 父项编号,a.物料类型 as 父项类型,a.大类 as 父项大类,a.小类 as 父项小类,a.n原ERP规格型号 as 父项规格 ,a.图纸编号 as 父项图纸,
            b.原ERP物料编号 as 子项编号,b.物料类型 as 子项类型,b.大类 as 子项大类,b.小类 as 子项小类,b.n原ERP规格型号 as 子项规格,b.图纸编号 as 子项图纸,数量  from 基础数据物料BOM表  base
            left join 基础数据物料信息表 a  on base.产品编码=a.物料编码
            left join 基础数据物料信息表 b  on base.子项编码=b.物料编码";
            using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
            {
                dt_bom_all = new DataTable();
                da.Fill(dt_bom_all);
            }
            s = @"  select  * from (
           select  原ERP物料编号 as 父项编号 ,a.物料类型 as 父项类型,a.大类 as 父项大类,a.小类 as 父项小类,a.n原ERP规格型号 as 父项规格
              from 基础数据物料BOM表 base
             left join 基础数据物料信息表 a  on base.产品编码=a.物料编码
             group  by 原ERP物料编号,物料类型,大类,小类,n原ERP规格型号)x where 父项编号 in (
            select  产品编码 from 基础数据物料BOM表  where 产品编码 not in  (select  子项编码 from 基础数据物料BOM表 group by 子项编码) group by 产品编码)";

            using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
            {
                dt_root = new DataTable();
                da.Fill(dt_root);
            }
        }

        #region 数据操作

        /// <summary>
        ///   头结点
        /// </summary>
        private void fun_TLhead(DataRow r)
        {
            TreeListNode head = treeList1.AppendNode(new object[] { r["父项编号"].ToString() }, null);
            head.SetValue("物料编号", r["父项编号"].ToString());
            head.SetValue("物料类型", r["父项类型"].ToString());
            head.SetValue("大类", r["父项大类"].ToString());
            head.SetValue("小类", r["父项小类"].ToString());
            head.SetValue("规格型号", r["父项规格"].ToString());
            head.Tag = r;
            fun_TL(head, r["父项编号"].ToString());

            head.ExpandAll();
        }

        /// <summary>
        /// 展开子节点
        /// </summary>
        /// <param name="n"></param>
        private void fun_TL(TreeListNode n, string str_fx)
        {
            try
            {
                DataRow[] dr = dt_bom_all.Select(string.Format("父项编号='{0}'", str_fx));
                foreach (DataRow r in dr)
                {
                    //TreeListNode nc = treeList1.AppendNode(new object[] { r["权限类型"].ToString() }, n);
                    TreeListNode nc = treeList1.AppendNode(new object[] { r["子项编号"].ToString() }, n);

                    nc.SetValue("物料编号", r["子项编号"].ToString());
                    nc.SetValue("物料类型", r["子项类型"].ToString());
                    nc.SetValue("大类", r["子项大类"].ToString());
                    nc.SetValue("小类", r["子项小类"].ToString());
                    nc.SetValue("规格型号", r["子项规格"].ToString());

                    nc.SetValue("数量", r["数量"].ToString());

                    nc.Tag = r;
                    fun_TL(nc, r["子项编号"].ToString());
                }
            }
            catch (Exception)
            {
                throw new Exception("错误");
            }
        }

        #endregion

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                foreach (DataRow dr in dt_root.Rows)
                { 
                    fun_TLhead(dr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsExportOptions options = new DevExpress.XtraPrinting.XlsExportOptions();

                    this.treeList1.ExportToXlsx(saveFileDialog.FileName);

                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
                MessageBox.Show(ex.Message);
            }
        }
    }
}