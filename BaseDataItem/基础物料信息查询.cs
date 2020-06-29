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
using System.Threading;
using DevExpress.XtraTreeList.Nodes;

namespace BaseData
{
    public partial class 基础物料信息查询 : UserControl
    {
        string cfgfilepath = "";
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        public 基础物料信息查询()
        {
            InitializeComponent();
        }

        private void 基础物料信息查询_Load(object sender, EventArgs e)
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
                x.UserLayout(splitContainer1, this.Name, cfgfilepath);
                fun_载入刷新();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_载入刷新()
        {
            try
            {
                string s = "select count(*) from 基础数据物料信息表 ";
                DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                int total = Convert.ToInt32(t.Rows[0][0]);
                int j = total / 4000;
                if (total % 4000 != 0)
                {
                    j++;
                }
                dtM = new DataTable();
                for (int i = 0; i < j; i++)
                {
                    string sx = string.Format(@"select  top 4000 base.*,a.版本 as sop版本  from 基础数据物料信息表 base
               left  join (select 类别名称,max(版本) as 版本 from 作业指导书文件表 group by 类别名称) a on base.物料编码=a.类别名称
                where 物料编码 not in (select  top (4000*{0})  物料编码 from 基础数据物料信息表)", i);
                    using (SqlDataAdapter da = new SqlDataAdapter(sx, strconn))
                    {
                        da.Fill(dtM);
                    }
                }
                fun_界面设置();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
     
        private void fun_界面设置()
        {
            string s = "select  *  from 基础数据存货分类表 order by   存货分类编码 ";
            DataTable tt = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            treeList1.OptionsBehavior.PopulateServiceColumns = true;
            treeList1.KeyFieldName = "GUID";
            treeList1.ParentFieldName = "上级类型GUID";
            treeList1.DataSource = tt;
            treeList1.CollapseAll();
        }

        private void treeList1_MouseClick(object sender, MouseEventArgs e)
        {
            if (treeList1.Nodes.Count > 0)
            {
                if (treeList1.Selection[0] == null) return;
            }
            else
            {
                return;
            }
            TreeListNode n = treeList1.Selection[0];
            string s = n.GetValue("存货分类编码").ToString();
            DataView v = new DataView(dtM);
            v.RowFilter = String.Format("存货分类编码 like '{0}%'", s);
            gridControl1.DataSource = v;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                if (dtM == null || dtM.Columns.Count == 0 || dtM.Rows.Count == 0)
                {

                    throw new Exception("没有数据可以导出");
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    //DataTable tt = dtM.Copy();
                    //tt.Columns.Remove("作废");
                    gridControl1.ExportToXlsx(saveFileDialog.FileName);
                    //ERPorg.Corg.TableToExcel(tt, saveFileDialog.FileName);
                    MessageBox.Show("导出成功");
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
