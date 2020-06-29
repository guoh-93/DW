using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;


namespace BaseData
{
    public partial class frm功能权限权限类型 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dt;

        string sql = "select * from 功能权限权限类型表";

        #endregion

        #region 自用
        public frm功能权限权限类型()
        {
            InitializeComponent();
            //if (CPublic.Var.LocalUserTeam == "公司高管权限" || CPublic.Var.LocalUserTeam == "ADMIN权限")
            //{
            //    barLargeButtonItem9.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

            //}
            //else
            //{

            //    barLargeButtonItem9.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //}


        }

        private void frm功能权限权限类型_Load(object sender, EventArgs e)
        {
            try
            {
                fun_下拉框值();


                fun_TLhead();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 数据操作
        /// <summary>
        ///   头结点
        /// </summary>
        private void fun_TLhead()
        {

            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                dt = new DataTable();
                da.Fill(dt);
                dt.Columns.Add("权限值", typeof(bool));
                dt.Columns.Add("审核", typeof(bool));

               // DataColumn[] pk = new DataColumn[3];
               // pk[0] = dt.Columns["GUID"];
               // pk[1] = dt.Columns["上级GUID"];
               //// pk[2] = dt.Columns["上级GUID"];



               // dt.PrimaryKey = pk;


                DataRow[] dr = dt.Select("上级GUID=''");
                foreach (DataRow r in dr)
                {
                    TreeListNode head = treeList1.AppendNode(new object[] { r["权限类型"].ToString() }, null);
                    head.SetValue("权限类型", r["权限类型"].ToString());
                    head.SetValue("权限类型描述", r["权限类型描述"].ToString());
                    head.SetValue("上级权限类型", r["上级权限类型"].ToString());
                    head.SetValue("GUID", r["GUID"].ToString());
                    head.SetValue("上级GUID", r["上级GUID"].ToString());
                    head.Tag = r;
                    fun_TL(head);
                    //head.ExpandAll();

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
                DataRow[] dr = dt.Select(string.Format("上级GUID='{0}'", (n.Tag as DataRow)["GUID"].ToString()).Trim());
                foreach (DataRow r in dr)
                {
                    TreeListNode nc = treeList1.AppendNode(new object[] { r["权限类型"].ToString() }, n);
                    nc.SetValue("权限类型", r["权限类型"].ToString());
                    nc.SetValue("权限类型描述", r["权限类型描述"].ToString());
                    nc.SetValue("上级权限类型", r["上级权限类型"].ToString());
                    nc.SetValue("GUID", r["GUID"].ToString());
                    nc.SetValue("上级GUID", r["上级GUID"].ToString());
                    nc.Tag = r;
                    fun_TL(nc);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void fun_添加下级()
        {
            treeList1.CloseEditor();
            this.BindingContext[dt].EndCurrentEdit();

            if (treeList1.Nodes.Count > 0)
            {
                if (treeList1.Selection[0] == null) return;
            }
            else
            {
                return;
            }
            TreeListNode n = treeList1.Selection[0];
            TreeListNode nc = treeList1.AppendNode(new object[] { "" }, n);
            //
            ///***
            nc.SetValue("上级权限类型", n.GetValue("权限类型").ToString());

            //**
            DataRow dr = dt.NewRow();
            dr["GUID"] = System.Guid.NewGuid();

            dt.Rows.Add(dr);
            nc.Tag = dr;
            dr["上级GUID"] = (n.Tag as DataRow)["GUID"].ToString();
            nc.SetValue("GUID", dr["GUID"]);

            n.ExpandAll();
        }

        private void fun_添加同级()
        {
            treeList1.CloseEditor();
            this.BindingContext[dt].EndCurrentEdit();
            if (treeList1.Nodes.Count > 0)
            {
                if (treeList1.Selection[0] == null)
                {
                    return;
                }
                else
                {

                }
            }
            TreeListNode n;
            if (treeList1.Selection[0] == null || treeList1.Selection[0].ParentNode == null)
            {
                n = null;
            }
            else
            {
                n = treeList1.Selection[0].ParentNode;
            }
            TreeListNode nc = treeList1.AppendNode(new object[] { "" }, n);

            DataRow dr = dt.NewRow();
            if (n == null)
            {
                //nc.SetValue("上级用户ID", "");

                dr["上级权限类型"] = "";
                dr["上级GUID"] = "";
            }
            else
            {
                nc.SetValue("上级权限类型", n.GetValue("权限类型"));

                dr["上级GUID"] = (n.Tag as DataRow)["GUID"].ToString().Trim();
                n.ExpandAll();
            }
            dr["GUID"] = System.Guid.NewGuid();
            nc.SetValue("GUID", dr["GUID"]);
            dt.Rows.Add(dr);
            nc.Tag = dr;



        }

        private void fun_删除(TreeListNode n)
        {
            foreach (TreeListNode nc in n.Nodes)
            {
                fun_删除(nc);
            }
            (n.Tag as DataRow).Delete();
        }

        private void ConfirmDt()
        {

            foreach (TreeListNode n in treeList1.Nodes)
            {
                DataRow r = n.Tag as DataRow;
                if (n.GetValue("权限值") == null)
                    r["权限值"] = false;
                else 
                r["权限值"] = n.GetValue("权限值");
                if (n.HasChildren == true)
                {
                    Conf_dg(n);
                }
            }
        }
        private void Conf_dg(TreeListNode n)
        {
            foreach (TreeListNode nz in n.Nodes)
            {
                DataRow r = nz.Tag as DataRow;
                if (nz.GetValue("权限值") == null) r["权限值"] = false;
                else
                r["权限值"] = nz.GetValue("权限值");
            }
        }

        private void fun_保存界面数据()
        {
            treeList1.CloseEditor();
            this.BindingContext[dt].EndCurrentEdit();

            if (treeList1.Selection[0] == null) return;


            //给DATAROW值
            DataRow r;
            r = treeList1.Selection[0].Tag as DataRow;

            r["上级权限类型"] = treeList1.Selection[0].GetValue("上级权限类型");
            r["权限类型"] = treeList1.Selection[0].GetValue("权限类型");
            r["权限类型描述"] = treeList1.Selection[0].GetValue("权限类型描述");

            if (treeList1.Selection[0].GetValue("权限值") == null)
            {
                r["权限值"] = false;
            }
            else
            {
                r["权限值"] = treeList1.Selection[0].GetValue("权限值");
            }
            //if (treeList1.Selection[0].GetValue("审核") == null)
            //{
            //    r["审核"] = false;
            //}
            //else
            //{
            //    r["审核"] = treeList1.Selection[0].GetValue("审核");
            //}
        }
        private void fun_check()
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                if (dr["权限类型"].ToString() == "")
                {
                    throw new Exception("权限类型不可为空");
                }
            }
        }
        private void fun_选择权限组()
        {
            try
            {
                foreach (TreeListNode n in treeList1.Nodes)
                {
                    string sql = string.Format("select * from 功能权限权限组权限表 where 权限组='{0}' and GUID='{1}' and 上级GUID='{2}'", barEditItem1.EditValue, n.GetValue("GUID").ToString(), n.GetValue("上级GUID").ToString());

                    DataRow r = CZMaster.MasterSQL.Get_DataRow(sql, strconn);
                    if (r == null)
                    {
                        n.SetValue("权限值", false);
                        n.SetValue("审核", false);
                    }
                    else
                    {
                        n.SetValue("权限值", true);
                        dt.Select(string.Format("上级权限类型='{0}' and GUID='{1}'", n.GetValue("上级权限类型"), n.GetValue("GUID").ToString()))[0]["权限值"] = true;
                        if (r["审核"].Equals(true))
                        {
                            n.SetValue("审核", true);
                            dt.Select(string.Format("上级权限类型='{0}' and GUID='{1}'", n.GetValue("上级权限类型"), n.GetValue("GUID").ToString()))[0]["审核"] = true;
                        }
                    }

                    if (n.HasChildren == true)
                    {
                        fun_dg(n);
                    }

                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void fun_dg(TreeListNode n)
        {
            foreach (TreeListNode nz in n.Nodes)
            {

                string s = nz.GetValue("GUID").ToString();

                string sql = string.Format("select * from 功能权限权限组权限表 where 权限组='{0}' and GUID='{1}'", barEditItem1.EditValue, nz.GetValue("GUID").ToString());

                DataRow r = CZMaster.MasterSQL.Get_DataRow(sql, strconn);

                if (r == null)
                {
                    nz.SetValue("权限值", false);
                }
                else
                {
                    nz.SetValue("权限值", true);
                    dt.Select(string.Format("上级权限类型='{0}' and GUID='{1}'", nz.GetValue("上级权限类型"), nz.GetValue("GUID").ToString()))[0]["权限值"] = true;
                }
                if (r == null)
                {
                    nz.SetValue("审核", false);
                }
                else if (r["审核"].Equals(true))
                {
                    nz.SetValue("审核", true);
                    dt.Select(string.Format("上级权限类型='{0}' and GUID='{1}'", nz.GetValue("上级权限类型"), nz.GetValue("GUID").ToString()))[0]["审核"] = true;
                }
                if (nz.HasChildren == true)
                {
                    fun_dg(nz);
                }

            }

        }

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
                    ((DevExpress.XtraEditors.Repository.RepositoryItemComboBox)this.barEditItem1.Edit).Items.Add(r["权限组"].ToString());
                }
            }
            catch { }
        }


        private void fun()
        {


            //如果选中大权限，小权限自动打钩
            DataRow r = treeList1.Selection[0].Tag as DataRow;
            r["上级权限类型"] = treeList1.Selection[0].GetValue("上级权限类型");
            r["权限值"] = treeList1.Selection[0].GetValue("权限值");
            TreeListNode n = treeList1.Selection[0];
 
            if (n.HasChildren == true)
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

        /// <summary>
        /// 只有在打勾的时候才触发此函数，取消勾的仍然保留原来的效果，
        /// </summary>
        private void fun_选取父节点(TreeListNode n)
        {

            DataRow r = n.Tag as DataRow;
            r["权限值"] = true;
            n.SetValue("权限值", true);
            if (n.ParentNode != null)
            {
                fun_选取父节点(n.ParentNode);
            }
        }


        /// <summary>
        /// 取消节点的时候 判断 父节点是否要取消勾
        /// </summary>
        private void fun_取消父节点(TreeListNode n)
        {
            DataRow r = n.Tag as DataRow;
            r["权限值"] = false;
            n.SetValue("权限值", false);
            if (n.ParentNode != null)
            {
                bool flag = false;
                foreach (TreeListNode a in n.ParentNode.Nodes)
                {
                    if (Convert.ToBoolean(a.GetValue("权限值")))
                    { flag = true; break; }
                }
                if (!flag)
                {
                    n.ParentNode.SetValue("权限值", false);
                    DataRow r1 = n.ParentNode.Tag as DataRow;
                    r1["权限值"] = false;
                    if (n.ParentNode.ParentNode != null)
                    {
                        fun_取消父节点(n.ParentNode);
                    }
                }

            }

        }

        #endregion

        #region 界面操作

        //关闭
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        //刷新
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                treeList1.ClearNodes();
                fun_TLhead();
                barEditItem1.EditValue = "";
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        //添加子权限
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            fun_保存界面数据();
            fun_添加下级();
        }
        //添加同级权限
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_保存界面数据();
            fun_添加同级();
        }

        //删除
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (treeList1.Selection[0] == null) return;
                if (MessageBox.Show(string.Format
                    ("是否删除'{0}',若删除从属其的数据都将删除！", treeList1.Selection[0].GetValue("权限类型").ToString()),
                    "警告！", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (treeList1.Selection[0].ParentNode == null)
                    {
                        fun_删除(treeList1.Selection[0]);
                        treeList1.Selection[0].Nodes.Remove(treeList1.Selection[0]);
                    }
                    else
                    {
                        fun_删除(treeList1.Selection[0]);
                        treeList1.Selection[0].ParentNode.Nodes.Remove(treeList1.Selection[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //保存
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            fun_保存界面数据();
            treeList1.CloseEditor();
            this.BindingContext[dt].EndCurrentEdit();


            try
            {
                fun_check();
                ConfirmDt();
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt);

                }
                #region 保存 权限组权限

                if (barEditItem1.EditValue != null || barEditItem1.EditValue != DBNull.Value || barEditItem1.EditValue.ToString() != "")
                {
                    DataTable dt_权限组权限;
                    dt_权限组权限 = CZMaster.MasterSQL.Get_DataTable("select * from 功能权限权限组权限表 ", strconn);

                    foreach (DataRow dr in dt.Rows)
                    {

                        if (dr["权限值"].Equals(true))
                        {
                            DataRow[] r = dt_权限组权限.Select(string.Format("权限组='{0}' and 权限类型='{1}' and GUID='{2}' ", barEditItem1.EditValue, dr["权限类型"], dr["GUID"]));
                            //DataRow[] r = dt_权限组权限.Select(string.Format(" GUID='{0}'",dr["GUID"]));

                            if (r.Length > 0)
                            {
                                continue;

                            }
                            else
                            {
                                DataRow rr = dt_权限组权限.NewRow();
                                rr["权限组"] = barEditItem1.EditValue;
                                rr["权限类型"] = dr["权限类型"];
                                rr["上级权限"] = dr["上级权限类型"];
                                rr["权限值"] = true;
                                rr["GUID"] = dr["GUID"];
                                rr["上级GUID"] = dr["上级GUID"];
                                rr["权限类型描述"] = dr["权限类型描述"];
                                rr["修改人"] = CPublic.Var.localUserName;
                                rr["修改日期"] = CPublic.Var.getDatetime();
                                dt_权限组权限.Rows.Add(rr);
                            }
                        }
                        else
                        {
                            DataRow[] r = dt_权限组权限.Select(string.Format("权限组='{0}' and 权限类型='{1}' ", barEditItem1.EditValue, dr["权限类型"]));
                            if (r.Length > 0)
                            {
                                r[0].Delete();
                            }
                            else
                            {
                                continue;
                            }

                        }


                    }
                    using (SqlDataAdapter da = new SqlDataAdapter("select * from 功能权限权限组权限表 where 1<>1", strconn))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_权限组权限);

                    }
                }

                #endregion

                MessageBox.Show("保存成功！");
                //刷新
                fun_选择权限组();
                // barLargeButtonItem5_ItemClick(null,null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }






        }
        #endregion

          



        //选择权限组
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ERPorg.Corg.FlushMemory();
            fun_选择权限组();
            MessageBox.Show("搜索完成");
        }


        private void repositoryItemCheckEdit1_EditValueChanged(object sender, EventArgs e)
        {
            fun_保存界面数据();
            treeList1.CloseEditor();
            this.BindingContext[dt].EndCurrentEdit();
            TreeListNode n = treeList1.Selection[0];
            if (n == null) return;
            if (n.HasChildren)
            {
                fun();

                if (Convert.ToBoolean(n.GetValue("权限值")))
                {

                    fun_选取父节点(n);
                }
                else //取消勾的时候 
                {
                    fun_取消父节点(n);

                }
            }
            else
            {
                if (n.ParentNode != null && Convert.ToBoolean(n.GetValue("权限值")))
                {

                    fun_选取父节点(n);
                }
                else //取消勾的时候 
                {
                    fun_取消父节点(n);

                }

            }
        }

        private void treeList1_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            fun_保存界面数据();
        }

        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions xls = new DevExpress.XtraPrinting.XlsxExportOptions();
                    treeList1.ExportToXlsx(saveFileDialog.FileName, xls);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          
        }
        //   17/12/3 未来电器审计 要求加入审核 先只要增加权限需要审核   删除先不管 
        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (barEditItem1.EditValue != null && barEditItem1.EditValue.ToString() != "")
                {
                    string sql = string.Format("update   功能权限权限组权限表 set 审核=1,审核日期='{0}',审核人='{1}' where 权限组='{2}'"
                    , CPublic.Var.getDatetime(), CPublic.Var.localUserName, barEditItem1.EditValue.ToString());
                    CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
                    MessageBox.Show("审核成功");
                }
                else
                {

                    MessageBox.Show("未选择任何权限组");

                }
                fun_选择权限组();
            }
            catch (Exception ex)
            {

                MessageBox.Show("出错了,错误：" + ex.Message);
            }

        }
    }
}
