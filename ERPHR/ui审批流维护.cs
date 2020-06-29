using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
//using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
namespace ERPorg
{
    public partial class ui审批流维护 : UserControl
    {
        #region 变量
        string strconn = CPublic.Var.strConn;
        DataTable dt;
        
        DataTable dt_下拉姓名;
        DataTable dt_单据类型; 
        #endregion
        public ui审批流维护()
        {
            InitializeComponent();
        }
        #region 函数
        /// <summary>
        ///   头结点
        /// </summary>
        private void fun_TLhead()
        {
            
            
            string sql =string.Format( "select * from 单据审批流表 where 单据类型 = '{0}'", barEditItem1.EditValue);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                dt = new DataTable();
                da.Fill(dt);
                //DataColumn[] pk  = new DataColumn[1];
                //pk[0] = dt.Columns["GUID"];
                ////pk[1] = dt.Columns["上级GUID"];
                ////pk[2] = dt.Columns["上级用户ID"];


                //dt.PrimaryKey = pk;

                DataRow[] dr = dt.Select("上级用户ID=''");
                foreach (DataRow r in dr)
                {
                    TreeListNode head = treeList1.AppendNode(new object[] { r["用户ID"].ToString() }, null);
                    head.SetValue("用户ID", r["用户ID"].ToString());
                    head.SetValue("用户描述", r["用户描述"].ToString());
                    head.SetValue("上级用户ID", r["上级用户ID"].ToString());
                    head.SetValue("单据类型", r["单据类型"].ToString());
                    head.SetValue("角色", r["角色"].ToString());
                    head.SetValue("工号", r["工号"].ToString());
                    head.SetValue("上级工号", r["上级工号"].ToString());
                    head.SetValue("备用人工号", r["备用人工号"].ToString());
                    head.SetValue("备用人姓名", r["备用人姓名"].ToString());
                    head.SetValue("是否启用", Convert.ToBoolean(r["是否启用"]));
                    head.Tag = r;
                    fun_TL(head);
                    head.ExpandAll();
                }
            }
        }
        private void fun_添加同级()
        {
            if (treeList1.Nodes.Count > 0)
            {
                if (treeList1.Selection[0] == null)
                {
                    return;
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
                dr["上级用户ID"] = "";
                dr["上级GUID"] = "";
                dr["角色"] = "审核人";

            }
            else
            {
                dr["上级用户ID"] = (n.Tag as DataRow)["用户ID"].ToString().Trim();
                dr["上级GUID"] = (n.Tag as DataRow)["GUID"].ToString().Trim();
                dr["角色"] = "录入人";
                nc.SetValue("上级用户ID", dr["上级用户ID"]);
                nc.SetValue("单据类型", (n.Tag as DataRow)["单据类型"].ToString().Trim());
                nc.SetValue("上级工号", (n.Tag as DataRow)["工号"].ToString().Trim());
                nc.SetValue("是否启用", (n.Tag as DataRow)["是否启用"].ToString().Trim());
             
                n.ExpandAll();
            }
            dr["GUID"] = System.Guid.NewGuid();
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
        /// <summary>
        /// 展开子节点
        /// </summary>
        /// <param name="n"></param>
        private void fun_TL(TreeListNode n)
        {
            try
            {
                DataRow[] dr = dt.Select(string.Format("上级GUID='{0}'", (n.Tag as DataRow)["GUID"].ToString().Trim()));
                foreach (DataRow r in dr)
                {
                    TreeListNode nc = treeList1.AppendNode(new object[] { r["用户ID"].ToString() }, n);
                    nc.SetValue("用户ID", r["用户ID"].ToString());
                    nc.SetValue("用户描述", r["用户描述"].ToString());
                    nc.SetValue("上级用户ID", r["上级用户ID"].ToString());
                    nc.SetValue("单据类型", r["单据类型"].ToString());
                    nc.SetValue("工号", r["工号"].ToString());
                    nc.SetValue("上级工号", r["上级工号"].ToString());
                    nc.SetValue("角色", r["角色"].ToString());
                    nc.SetValue("是否启用", Convert.ToBoolean(r["是否启用"]));
                    nc.SetValue("备用人工号", r["备用人工号"].ToString());
                    nc.SetValue("备用人姓名", r["备用人姓名"].ToString());
                    nc.Tag = r;
                    DataRow[] drr = dt.Select(string.Format("上级GUID='{0}'", r["GUID"].ToString().Trim()));
                    if (drr.Length > 0) fun_TL(nc);
          
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 及时将界面值保存至 dt
        /// </summary>
        private void fun_保存界面值()
        {
            treeList1.CloseEditor();
            this.BindingContext[dt].EndCurrentEdit();
            if (treeList1.Selection[0] == null) return;
            //给DATAROW值
            DataRow r;
            r = treeList1.Selection[0].Tag as DataRow;
            r["上级用户ID"] = treeList1.Selection[0].GetValue("上级用户ID");
            r["用户ID"] = treeList1.Selection[0].GetValue("用户ID");
            r["用户描述"] = treeList1.Selection[0].GetValue("用户描述");
            r["单据类型"] = treeList1.Selection[0].GetValue("单据类型");
            r["工号"] = treeList1.Selection[0].GetValue("工号");
            r["上级工号"] = treeList1.Selection[0].GetValue("上级工号");
           
            r["上级工号"] = treeList1.Selection[0].GetValue("上级工号");
           
            r["是否启用"] = treeList1.Selection[0].GetValue("是否启用") == null ? false : treeList1.Selection[0].GetValue("是否启用");
            r["备用人工号"] = treeList1.Selection[0].GetValue("备用人工号");
            r["角色"] = treeList1.Selection[0].GetValue("角色");
            r["备用人姓名"] = treeList1.Selection[0].GetValue("备用人姓名");

        }
        private void fun_添加下级()
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
            TreeListNode nc = treeList1.AppendNode(new object[] { "" }, n);
            ///***
            nc.SetValue("上级用户ID", (n.Tag as DataRow)["用户ID"].ToString());
            nc.SetValue("单据类型", (n.Tag as DataRow)["单据类型"].ToString().Trim());
            nc.SetValue("上级工号", (n.Tag as DataRow)["工号"].ToString());
            nc.SetValue("是否启用", false);

            //**
            DataRow dr = dt.NewRow();
            dr["GUID"] = System.Guid.NewGuid();
            dr["上级GUID"] = (n.Tag as DataRow)["GUID"].ToString().Trim();
            dt.Rows.Add(dr);
            nc.Tag = dr;
            n.ExpandAll();
        }
        private void fun_下拉()
        {
            string sql_下拉_姓名 = "select 员工号,姓名,部门 from 人事基础员工表";
            dt_下拉姓名 = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(sql_下拉_姓名, strconn))
            {
                da.Fill(dt_下拉姓名);
            }
            string sql_下拉_片区 = "select 属性值 from 基础数据基础属性表 where 属性类别='审批流单据类型'";
            dt_单据类型 = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(sql_下拉_片区, strconn))
            {
                da.Fill(dt_单据类型);
            }
            repositoryItemSearchLookUpEdit1.DataSource = dt_下拉姓名;
            repositoryItemSearchLookUpEdit1.DisplayMember = "员工号";
            repositoryItemSearchLookUpEdit1.ValueMember = "员工号";
            //更改上级
            repositoryItemSearchLookUpEdit3.DataSource = dt_下拉姓名;
            repositoryItemSearchLookUpEdit3.DisplayMember = "员工号";
            repositoryItemSearchLookUpEdit3.ValueMember = "员工号";

            repositoryItemSearchLookUpEdit2.DataSource = dt_单据类型;
            repositoryItemSearchLookUpEdit2.DisplayMember = "属性值";
            repositoryItemSearchLookUpEdit2.ValueMember = "属性值";

            repositoryItemSearchLookUpEdit5.DataSource = dt_单据类型;
            repositoryItemSearchLookUpEdit5.DisplayMember = "属性值";
            repositoryItemSearchLookUpEdit5.ValueMember = "属性值";
            




        }
        #endregion


        //删除
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (treeList1.Selection[0] == null) return;
                if (MessageBox.Show(string.Format
                    ("是否删除'{0}',若删除从属其的数据都将删除！", treeList1.Selection[0].GetValue("用户ID").ToString()),
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
        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                treeList1.ClearNodes(); 
                fun_TLhead();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //添加下级
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_保存界面值();
                fun_添加下级();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        //添加同级
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_保存界面值();
                fun_添加同级();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //保存
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = null;
            try
            {
                fun_保存界面值();

                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("审批流修改");
                try
                {
                    string sql = "select * from 单据审批流表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt);
                    ts.Commit();
                    MessageBox.Show("保存成功！");
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    throw new Exception(ex.Message);
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            barLargeButtonItem1_ItemClick(null, null);
        }
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void ui审批流维护_Load(object sender, EventArgs e)
        {
            try
            {
               // fun_TLhead();
                fun_下拉();


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void treeList1_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {
            if (e.Column.Caption == "工号")
            {
                DataRow xr = treeList1.Selection[0].Tag as DataRow; //获取当前行 

                DataRow[] r_test = dt.Select(string.Format("  工号='{0}' and 单据类型='{1}' and 角色='{2}'", e.Value, xr["单据类型"].ToString(),xr["角色"].ToString()));
                // 如果已存在 此员工 提示 返回
                if (r_test.Length == 0)
                {
                    DataRow[] dr = dt_下拉姓名.Select(string.Format(" 员工号='{0}' ", e.Value));
                    treeList1.Selection[0]["用户ID"] = dr[0]["姓名"];
                }
                else
                {
                    MessageBox.Show("此员工已存在");
                    return;
                }
            }
            else if (e.Column.Caption == "上级工号")
            {
                DataRow dr = treeList1.Selection[0].Tag as DataRow; //获取当前行 
                DataRow[] r = dt.Select(string.Format("工号='{0}'", e.Value));
                if (e.Value == null)
                {
                    dr["上级GUID"] = "";
                    dr["上级用户ID"] = "";
                    treeList1.Selection[0].SetValue("上级用户ID", "");
                    treeList1.Selection[0].SetValue("单据类型", "");
                }
                else if (dr["工号"].ToString() == e.Value.ToString())
                {
                    MessageBox.Show("不能将上级用户设置成自己");
                    return;
                }
                else if (r.Length > 0)
                {
                    dr["上级GUID"] = r[0]["GUID"];
                    dr["上级用户ID"] = r[0]["用户ID"];
                    treeList1.Selection[0].SetValue("上级用户ID", dr["上级用户ID"]);
                    treeList1.Selection[0].SetValue("单据类型", dr["单据类型"]);
                }
                else
                {
                    MessageBox.Show("未找到该人员信息");
                    return;
                }
            }
            else if (e.Column.Caption == "备用人工号")
            {
                DataRow xr = treeList1.Selection[0].Tag as DataRow; //获取当前行 


                // 如果已存在 此员工 提示 返回

                DataRow[] dr = dt_下拉姓名.Select(string.Format(" 员工号='{0}' ", e.Value));
                if (dr.Length > 0)
                {
                    treeList1.Selection[0]["备用人工号"] = dr[0]["员工号"];
                    treeList1.Selection[0]["备用人姓名"] = dr[0]["姓名"];
                }
                else
                {
                    treeList1.Selection[0]["备用人姓名"] =  "";
                }
                 

            }
            fun_保存界面值();
        }
        private void repositoryItemSearchLookUpEdit3_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            DataRow dr = treeList1.Selection[0].Tag as DataRow; //获取当前行 

            DataRow[] r = dt.Select(string.Format("工号='{0}' and 单据类型='{1}'", e.NewValue,dr["单据类型"]));
            //通过工号找到 界面上设置的上级工号
            if (e.NewValue == null)
            {
                dr["上级GUID"] = "";
                dr["上级用户ID"] = "";
                treeList1.Selection[0].SetValue("上级用户ID", "");
                treeList1.Selection[0].SetValue("单据类型", "");
            }
            else if (dr["工号"].ToString() == e.NewValue.ToString())
            {
                MessageBox.Show("不能将上级用户设置成自己");
                return;
            }

            else if (r.Length > 0)
            {
                dr["上级GUID"] = r[0]["GUID"];
                dr["上级用户ID"] = r[0]["用户ID"];
                treeList1.Selection[0].SetValue("上级用户ID", dr["上级用户ID"]);
                treeList1.Selection[0].SetValue("单据类型", dr["单据类型"]);

            }
            else
            {
                MessageBox.Show("未找到该人员信息");
                return;
            }
        }

        private void repositoryItemSearchLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            //    DataRow xr = treeList1.Selection[0].Tag as DataRow; //获取当前行 

            //    DataRow[] r_test = dt.Select(string.Format("  工号='{0}' and 单据类型='{1}'", e.NewValue, xr["单据类型"].ToString()));
            //    // 如果已存在 此员工 提示 返回
            //    if (r_test.Length == 0)
            //    {
            //        DataRow[] dr = dt_下拉姓名.Select(string.Format(" 员工号='{0}' ", e.NewValue));


            //        treeList1.Selection[0]["用户ID"] = dr[0]["姓名"];
            //    }
            //    else
            //    {
            //        MessageBox.Show("此员工已存在");
            //        return;
            //    }
        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                treeList1.ClearNodes();
                fun_TLhead();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
