using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraTreeList.Nodes;

namespace BaseData
{
    public partial class frm功能权限权限组权限 : UserControl
    {
        #region 成员
        DataTable dtP = new DataTable();
        DataTable dtM;
        SqlDataAdapter da;
        string strshow;
        string strconn = CPublic.Var.strConn;
        #endregion

        #region 自用类
        public frm功能权限权限组权限()
        {
            InitializeComponent();
        }

        private void frm功能权限权限组权限_Load(object sender, EventArgs e)
        {
            fun_下拉框值();
            fun_载入权限类型();
            treeList1.CellValueChanged += treeList1_CellValueChanged;
         
        }

        void treeList1_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {
            if (e.Column.Caption == "权限类型" || e.Column.Caption == "上级权限" || e.Column.Caption == "权限类型描述")
            {
                return;
            }
            fun_保存界面数据();
            fun();
        }
        #endregion

        #region 方法
        public void fun_下拉框值()
        {
            try
            {
                DataTable dt = new DataTable();
                string sql = "select * from 功能权限权限组表";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                foreach (DataRow r in dt.Rows)
                {
                    ((DevExpress.XtraEditors.Repository.RepositoryItemComboBox)this.barEditItem2.Edit).Items.Add(r["权限组"].ToString());
                }
            }
            catch { }
        }

        /// <summary>
        ///   头结点
        /// </summary>
        private void fun_TL_Head()
        {
            {
                DataRow[] dr = dtM.Select("上级权限 = ''");
                foreach (DataRow r in dr)
                {
                    TreeListNode head = treeList1.AppendNode(new object[] { r["权限类型"].ToString() }, null);
                    head.SetValue("权限类型", r["权限类型"].ToString());
                    head.SetValue("上级权限", r["上级权限"].ToString());
                    head.SetValue("权限值", r["权限值"]);
                    head.SetValue("权限类型描述", r["权限类型描述"].ToString());
                    head.Tag = r;
                    fun_TL(head);
                    head.ExpandAll();
                }
            }
        }

        /// <summary>
        /// 展开子节点
        /// </summary>
        /// <param name="n"></param>
        private void fun_TL(TreeListNode n)
        {
            try
            {
                DataRow[] dr = dtM.Select(string.Format("上级权限 = '{0}'", (n.Tag as DataRow)["权限类型"].ToString()));
                foreach (DataRow r in dr)
                {
                    TreeListNode nc = treeList1.AppendNode(new object[] { r["权限类型"].ToString() }, n);
                    nc.SetValue("权限类型", r["权限类型"].ToString());
                    nc.SetValue("上级权限", r["上级权限"].ToString());
                    nc.SetValue("权限值", r["权限值"]);
                    nc.SetValue("权限类型描述", r["权限类型描述"].ToString());
                    nc.Tag = r;
                    fun_TL(nc);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_载入权限类型()
        {
            try
            {
                string sql = "select 权限类型,上级权限类型,权限类型描述 from 功能权限权限类型表";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                dtP.Columns.Add("权限组");
                da.Fill(dtP);
                dtP.Columns.Add("权限值");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_选择权限组()
        {
            treeList1.ClearNodes();
            try
            {
                string sql = string.Format("select * from 功能权限权限组权限表 where 权限组 = '{0}'", this.barEditItem2.EditValue.ToString());
                da = new SqlDataAdapter(sql, strconn);
                dtM = new DataTable();
                da.Fill(dtM);
                dtM.Columns.Add("是否");

                foreach (DataRow dr in dtP.Rows)
                {
                    int count = 0;
                    foreach (DataRow r in dtM.Rows)
                    {
                        if (dr["权限类型"].ToString() == r["权限类型"].ToString())
                        {
                            //如果已经有了，无视
                            r["上级权限"] = dr["上级权限类型"].ToString();
                            r["权限类型描述"] = dr["权限类型描述"].ToString();
                            r["是否"] = "是";
                            break;
                        }
                        else
                        {
                            count++;
                        }
                    }
                    if (count == dtM.Rows.Count)
                    {
                        //添加新的权限类型
                        DataRow drr = dtM.NewRow();
                        drr["权限类型"] = dr["权限类型"].ToString();
                        drr["权限组"] = this.barEditItem2.EditValue.ToString();
                        drr["上级权限"] = dr["上级权限类型"].ToString();
                        drr["权限类型描述"] = dr["权限类型描述"].ToString();
                        drr["权限值"] = 0;
                        drr["是否"] = "是";
                        dtM.Rows.Add(drr);
                    }
                }
                foreach (DataRow r in dtM.Rows)
                {
                    if (r.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    if (r["是否"].ToString() != "是")
                    {
                        r.Delete();
                    }
                }
                fun_TL_Head();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void fun_保存()
        {
            try
            {
                foreach (DataRow r in dtM.Rows)
                {
                    if (r.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    if (r["是否"].ToString() != "是")
                    {
                        r.Delete();
                    }
                }
                new SqlCommandBuilder(da);
                da.Update(dtM);
                fun_选择权限组();
                strshow = "保存成功!";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //及时保存界面数据
        private void fun_保存界面数据()
        {
            treeList1.CloseEditor();
            this.BindingContext[dtM].EndCurrentEdit();
            if (treeList1.Selection[0] == null) return;

            //给DATAROW值
            DataRow r = treeList1.Selection[0].Tag as DataRow;
            r["权限类型"] = treeList1.Selection[0].GetValue("权限类型");
            r["上级权限"] = treeList1.Selection[0].GetValue("上级权限");
            r["权限类型描述"] = treeList1.Selection[0].GetValue("权限类型描述");
            r["权限值"] = treeList1.Selection[0].GetValue("权限值");
        }

        private void fun()
        {
            treeList1.CloseEditor();
            this.BindingContext[dtM].EndCurrentEdit();
            if (treeList1.Selection[0] == null) return;

            //如果选中大权限，小权限自动打钩
            DataRow r = treeList1.Selection[0].Tag as DataRow;
            r["上级权限"] = treeList1.Selection[0].GetValue("上级权限");
            r["权限值"] = treeList1.Selection[0].GetValue("权限值");
            TreeListNode n = treeList1.Selection[0];
            if(n.HasChildren == true)
            //if (r["上级权限"].ToString() == "")
            {
                //获取它之下的所有小权限
                if (r["权限值"].ToString().ToLower() == "true")
                {
                    //fun_小权限打钩(r["权限类型"].ToString()); 
                    foreach (TreeListNode a in n.Nodes)
                    {
                        a.SetValue("权限值", true);
                        fun_递归(a);
                    }
                }
                else
                {
                    //fun_小权限去钩(r["权限类型"].ToString());
                    foreach (TreeListNode a in n.Nodes)
                    {
                        a.SetValue("权限值", false);
                        fun_递归(a);
                    }
                }
                //treeList1.ClearNodes();
                //fun_TL_Head();
            }
        }
        private void fun_递归(TreeListNode n)
        {
            DataRow r = n.Tag as DataRow;
            r["权限值"] = n.GetValue("权限值");
            if (n.HasChildren == true)
            {
                //获取它之下的所有小权限
                if (r["权限值"].ToString().ToLower() == "true")
                {
                    foreach (TreeListNode a in n.Nodes)
                    {
                        a.SetValue("权限值", true);
                        fun_递归(a);
                    }
                }
                else
                {
                    foreach (TreeListNode a in n.Nodes)
                    {
                        a.SetValue("权限值", false);
                        fun_递归(a);
                    }
                }
            }
        }

        //private void fun_小权限打钩(string str)
        //{
        //    DataRow[] ds = dtM.Select(string.Format("上级权限 = '{0}'", str));
        //    if (ds.Length > 0)
        //    {
        //        foreach (DataRow r in ds)
        //        {
        //            r["权限值"] = true;
        //            fun_小权限打钩(r["权限类型"].ToString());
        //        }
        //    }
        //}
        //private void fun_小权限去钩(string str)
        //{
        //    DataRow[] ds = dtM.Select(string.Format("上级权限 = '{0}'", str));
        //    if (ds.Length > 0)
        //    {
        //        foreach (DataRow r in ds)
        //        {
        //            r["权限值"] = false;
        //            fun_小权限去钩(r["权限类型"].ToString());
        //        }
        //    }
        //}
        #endregion

        #region 界面操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //搜索
            if (this.barEditItem2.EditValue == null)
            {
                MessageBox.Show("请先选择");
            }
            else
            {
                fun_选择权限组();
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_保存界面数据();
            treeList1.CloseEditor();
            this.BindingContext[dtM].EndCurrentEdit();
            //保存
            fun_保存();
            MessageBox.Show(strshow);
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion

        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            treeList1.CloseEditor();
            this.BindingContext[dtM].EndCurrentEdit();
        }
    }
}
