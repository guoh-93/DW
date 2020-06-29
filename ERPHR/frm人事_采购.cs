using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CPublic;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System.Data.SqlClient;
using CZMaster;

namespace ERPorg
{
    public partial class frm人事_采购 : UserControl
    {
        #region 变量
        string strconn = CPublic.Var.strConn;
        DataTable dt;
        
        string sql = "select * from 人事记录组织采购关系表";
        DataTable dt_下拉;
        #endregion

        #region 加载
        public frm人事_采购()
        {
            InitializeComponent();

        }
        private void frm人事_采购_Load(object sender, EventArgs e)
        {
            try
            {
                fun_TLhead();
                fun_下拉();
                treeList1.CellValueChanged += treeList1_CellValueChanged;
                repositoryItemSearchLookUpEdit2.EditValueChanging += repositoryItemSearchLookUpEdit2_EditValueChanging;
                repositoryItemSearchLookUpEdit1.EditValueChanging += repositoryItemSearchLookUpEdit1_EditValueChanging;
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 函数
        /// <summary>
        ///   头结点
        /// </summary>
        private void fun_TLhead()
        {

            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                dt = new DataTable();
                da.Fill(dt);
                DataRow[] dr = dt.Select("上级用户ID=''");
                foreach (DataRow r in dr)
                {
                    TreeListNode head = treeList1.AppendNode(new object[] { r["用户ID"].ToString() }, null);
                    head.SetValue("用户ID", r["用户ID"].ToString());
                    head.SetValue("用户描述", r["用户描述"].ToString());
                    head.SetValue("上级用户ID", r["上级用户ID"].ToString());
                    head.SetValue("工号", r["工号"].ToString());
                    head.SetValue("上级工号", r["上级工号"].ToString());
                    head.Tag = r;
                    fun_TL(head);
                    head.ExpandAll();
                }

            }

        }
        void repositoryItemSearchLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
                 DataRow  []r_test=dt.Select(string.Format(" 工号='{0}'",e.NewValue ));
                // 如果已存在 此员工 提示 返回
                if (r_test.Length == 0)
                {
                    DataRow[] dr = dt_下拉.Select(string.Format("员工号='{0}'", e.NewValue));

                    if (dr.Length == 0)
                    {
                        treeList1.Selection[0]["用户ID"] = "";
                    }
                    else
                    {
                        treeList1.Selection[0]["用户ID"] = dr[0]["姓名"];
                    }
                }
                else
                {
                    MessageBox.Show("此员工已存在");
                    return;
                }
        }

      

   
     
        void treeList1_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (e.Column.Caption == "工号"||e.Column.Caption=="上级工号" )
            {
                return;
                //DataRow  []r_test=dt.Select(string.Format("员工号='{0}'",e.Value));
                //// 如果已存在 此员工 提示 返回
                //if (r_test.Length == 0)
                //{
                //    DataRow[] dr = dt_下拉.Select(string.Format("员工号='{0}'", e.Value));


                //    treeList1.Selection[0]["用户ID"] = dr[0]["姓名"];
                //}
                //else
                //{
                //    MessageBox.Show("此员工已存在");
                //    return;
                //}
            }
            
            fun_保存界面数据();
        }
        //更改上级
        void repositoryItemSearchLookUpEdit2_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            //DataRow r = treeList1.Selection[0].Tag as DataRow;
            //DataRow[] rr = dt.Select(string.Format("工号='{0}'", e.NewValue));
            //if (rr.Length > 0)
            //{
            //    r["上级GUID"] = rr[0]["GUID"];
            //}

            DataRow dr = treeList1.Selection[0].Tag as DataRow; //获取当前行 
             
            DataRow[] r = dt.Select(string.Format("工号='{0}'", e.NewValue));
            //通过工号找到 界面上设置的上级工号
            if (e.NewValue == null)
            {
                dr["上级GUID"] = "";
                dr["上级用户ID"] = "";
                treeList1.Selection[0].SetValue("上级用户ID", "");
                
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
               

            }
            else
            {
                MessageBox.Show("未找到该人员信息");
                return;
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
                    TreeListNode nc = treeList1.AppendNode(new object[] { r["用户ID"].ToString() }, n);
                    nc.SetValue("用户ID", r["用户ID"].ToString());
                    nc.SetValue("用户描述", r["用户描述"].ToString());
                    nc.SetValue("上级用户ID", r["上级用户ID"].ToString());
                    nc.SetValue("工号", r["工号"].ToString());
                    nc.SetValue("上级工号", r["上级工号"].ToString());
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
            nc.SetValue("上级用户ID", n.GetValue("用户ID").ToString());
            nc.SetValue("上级工号", n.GetValue("工号").ToString());
            //**
            DataRow dr = dt.NewRow();
            dr["GUID"] = System.Guid.NewGuid();

            dt.Rows.Add(dr);
            nc.Tag = dr;
            dr["上级GUID"] = (n.Tag as DataRow)["GUID"].ToString();
            dr["用户描述"] = (n.Tag as DataRow)["用户描述"].ToString();
            // dr["上级用户ID"] = (n.Tag as DataRow)["用户ID"].ToString().Trim();
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
            //nc.SetValue("用户ID", "");
            //nc.SetValue("工号", "");
            //nc.SetValue("用户描述", "");
            DataRow dr = dt.NewRow();
            if (n == null)
            {
                //nc.SetValue("上级用户ID", "");

                dr["上级用户ID"] = "";
                dr["上级GUID"] = "";
            }
            else
            {
                nc.SetValue("上级用户ID", n.GetValue("用户ID"));
                nc.SetValue("上级工号", n.GetValue("工号"));
                //dr["上级用户ID"] = (n.Tag as DataRow)["用户ID"].ToString().Trim();
                dr["上级GUID"] = (n.Tag as DataRow)["GUID"].ToString().Trim();
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
        //及时保存界面数据
        private void fun_保存界面数据()
        {
             treeList1.CloseEditor();
             this.BindingContext[dt].EndCurrentEdit();

             if (treeList1.Selection[0] == null ) return;
        

            //给DATAROW值
            DataRow r;
            r = treeList1.Selection[0].Tag as DataRow;
            if (r.RowState == DataRowState.Deleted) return;
            r["上级用户ID"] = treeList1.Selection[0].GetValue("上级用户ID");
            r["用户ID"] = treeList1.Selection[0].GetValue("用户ID");
            r["用户描述"] = treeList1.Selection[0].GetValue("用户描述");
            r["工号"] = treeList1.Selection[0].GetValue("工号");
            r["上级工号"] = treeList1.Selection[0].GetValue("上级工号");

        }
     
       

     
        private void fun_下拉()
        {
           
            string sql_下拉 = "select 姓名,员工号,部门  from 人事基础员工表";
            dt_下拉 = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(sql_下拉, strconn))
            {
                da.Fill(dt_下拉);
 
            }
            repositoryItemSearchLookUpEdit1.DataSource = dt_下拉;

            repositoryItemSearchLookUpEdit1.DisplayMember = "员工号";
            repositoryItemSearchLookUpEdit1.ValueMember = "员工号";

            repositoryItemSearchLookUpEdit2.DataSource = dt_下拉;
            repositoryItemSearchLookUpEdit2.DisplayMember = "员工号";
            repositoryItemSearchLookUpEdit2.ValueMember = "员工号";
           
        }


        #endregion

        #region 界面操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                treeList1.ClearNodes(); ;
                fun_TLhead();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_保存界面数据();
            fun_添加下级();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_保存界面数据();
            fun_添加同级();
        }
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
        //保存
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_保存界面数据();
            treeList1.CloseEditor();
            this.BindingContext[dt].EndCurrentEdit();

            try
            {

                check();
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt);
                    MessageBox.Show("保存成功！");
                    barLargeButtonItem1_ItemClick(null, null);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        private void treeList1_CellValueChanged_1(object sender, CellValueChangedEventArgs e)
        {
            fun_保存界面数据();
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }


        private void check()
        {
            //检查是否有空行
            foreach (DataRow dr in dt.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                if (dr["工号"].ToString() == "")  throw new Exception("有工号为空的行,请检查");
            }
        }
     
       










    }
}
