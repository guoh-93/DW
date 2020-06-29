using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System.Data.OleDb;


namespace BaseData
{
    public partial class 客户分类维护 : UserControl
    {
        public 客户分类维护()
        {
            InitializeComponent();
        }

        #region 私有成员
        DataTable dtM, dt_数据;
        SqlDataAdapter daM;
        SqlCommandBuilder brM;
        DataTable dt_客户;

        string strcon = CPublic.Var.strConn;
        TreeListNode nc;
        #endregion

  

        private void 客户分类维护_Load(object sender, EventArgs e)
        {
            try
            {

                Init();
                dt_数据 = dtM.Clone();

                //string sql = string.Format("select 客户编号,客户名称  from   客户基础信息表  ");
                //dt_客户 = new DataTable();
                //dt_客户 = CZMaster.MasterSQL.Get_DataTable(sql,strcon);

                //repositoryItemGridLookUpEdit1.DataSource = dt_客户;
                //repositoryItemGridLookUpEdit1.DisplayMember = "客户编号";
                //repositoryItemGridLookUpEdit1.ValueMember = "客户编号";


            



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

     

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_添加下级();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



        #region 数据库的读取与保存

        private void Init(TreeListNode n)
        {
            string sqlstr = "上级类型GUID = '{0}'";
            sqlstr = string.Format(sqlstr, (n.Tag as DataRow)["GUID"].ToString());
            DataRow[] rs = dtM.Select(sqlstr, "客户分类编码");
            foreach (DataRow r in rs)
            {
                TreeListNode nc = tv.AppendNode(new object[] { r["GUID"].ToString() }, n);

                nc.SetValue("客户分类编码", r["客户分类编码"].ToString());
                nc.SetValue("类别名称", r["类别名称"].ToString());


                nc.SetValue("GUID", r["GUID"].ToString());
                nc.SetValue("上级类型GUID", r["上级类型GUID"].ToString());


                nc.SetValue("层级", r["层级"].ToString());
                nc.SetValue("是否末级", Convert.ToBoolean(r["是否末级"]));

                nc.Tag = r;
                Init(nc);
            }
        }
        private void Init()
        {
            string sqlstr;
            sqlstr = "select * from 客户分类表 order by 客户分类编码  ";
            daM = new SqlDataAdapter(sqlstr, CPublic.Var.strConn);
            brM = new SqlCommandBuilder(daM);
            dtM = new DataTable();
            daM.Fill(dtM);

            DataRow[] t = dtM.Select("上级类型GUID = ''");
            foreach (DataRow r in t)
            {
                TreeListNode n = tv.AppendNode(new object[] { r["GUID"].ToString() }, null);
                n.SetValue("客户分类编码", r["客户分类编码"].ToString());
                n.SetValue("类别名称", r["类别名称"].ToString());
                //n.SetValue("GUID", r["GUID"].ToString());
                //n.SetValue("上级类型GUID", r["上级类型GUID"].ToString());


                n.SetValue("层级", r["层级"].ToString());
                n.SetValue("是否末级", Convert.ToBoolean(r["是否末级"]));
                n.Tag = r;
                Init(n);
            }
        }

        private void fun_添加下级()
        {
            if (tv.Nodes.Count > 0)
            {
                if (tv.Selection[0] == null) return;
            }
            else
            {
                return;
            }
            TreeListNode n = tv.Selection[0];
            TreeListNode nc = tv.AppendNode(new object[] { "" }, n);
            //  nc.SetValue("层级", "");
            DataRow r = dtM.NewRow();
            r["GUID"] = System.Guid.NewGuid();
            dtM.Rows.Add(r);
            nc.Tag = r;
            //if (n.GetDisplayText("") == "物料结构")
            //{
            //    r["上级类型GUID"] = "";
            //}
            //else
            //{
                r["上级类型GUID"] = (n.Tag as DataRow)["GUID"].ToString();
            //}

             n.ExpandAll();
        }


        private void fun_添加同级()
        {
            if (tv.Nodes.Count > 0)
            {
                if (tv.Selection[0] == null)
                {
                    return;
                }
                else
                {
                    //if (tv.Selection[0].ParentNode == null) return;
                }
            }
            TreeListNode n;
            if (tv.Selection[0] == null || tv.Selection[0].ParentNode == null)
            {
                n = null;
            }
            else
            {
                n = tv.Selection[0].ParentNode;
            }
            TreeListNode nc = tv.AppendNode(new object[] { "" }, n);
            //  nc.SetValue("层级", "");
            DataRow r = dtM.NewRow();
            r["GUID"] = System.Guid.NewGuid();
            dtM.Rows.Add(r);
            nc.Tag = r;


            if (n == null)
            {
                r["上级类型GUID"] = "";
            }
            else
            {
                r["上级类型GUID"] = (n.Tag as DataRow)["GUID"].ToString();
               n.ExpandAll();
            }

        }

        //保存界面数据
        private void saveDataFromNode()
        {
            if (tv.Selection[0] == null) return;
            //tv.Selection[0].SetValue("物料结构", tv.Selection[0].GetValue("存货分类名称"));
            //给DATAROW值
            DataRow r;
            r = tv.Selection[0].Tag as DataRow;
            r["客户分类编码"] = tv.Selection[0].GetValue("客户分类编码");
            r["类别名称"] = tv.Selection[0].GetValue("类别名称");
      

            if (tv.Selection[0].ParentNode == null)   //如果是顶层节点，类型级别就一定是大类
            {
                r["层级"] = "1";
            }
            else
            {
                r["层级"] = tv.Selection[0].GetValue("层级");
            }

            r["是否末级"] = tv.Selection[0].GetValue("是否末级");

            #region
            //if (tv.Selection[0].ParentNode == null)  //表示这个是根节点:如果是根节点的话，类型级别是大类
            //{
            //    r["类型级别"] = "大类"; 
            //}
            //if (tv.Selection[0].ParentNode!=null && tv.Selection[0].GetValue("类型级别").ToString().Equals(""))  //非根节点，用户没有选择，则类型级别是小类
            //{
            //    r["类型级别"] = "小类";
            //}
            //if (tv.Selection[0].ParentNode != null && !tv.Selection[0].GetValue("类型级别").ToString().Equals(""))  //按照用户的选择
            //{
            //    r["类型级别"] = tv.Selection[0].GetValue("类型级别");
            //}

            //foreach (TreeListNode n in tv.Selection[0].Nodes)
            //{
            //    r = n.Tag as DataRow;
            //    r["上级类型GUID"] = n.ParentNode.GetValue("物料类型GUID");
            //}
            #endregion
        }
        private void fun_Check()
        {
           tv.DataSource = dtM;
            tv.PopulateColumns();
         
            
            //tv.Columns.ColumnByFieldName("GUID").Visible = false;
            //tv.Columns.ColumnByFieldName("上级类型GUID").Visible = false;
            foreach (DataRow r in dtM.Rows)
            {
                if (r.RowState == DataRowState.Deleted)
                {

                    continue;
                }

                if (r["GUID"].Equals(""))
                {
                    r["GUID"] = Guid.NewGuid();
                }





                if (r.RowState == DataRowState.Deleted) continue;

                if (r["客户分类编码"].ToString() == "")
                {
                   throw new Exception("地区编码不能为空，请检查！");
                }

                DataRow[] dr = dtM.Select(string.Format("客户分类编码='{0}'", r["客户分类编码"].ToString()));
                if (dr.Length >= 2)
                {
                    throw new Exception(string.Format("客户分类编码\"{0}\"有重复，请检查！", r["客户分类编码"].ToString()));
                }
                if (r["层级"].ToString() == "")
                {
                    throw new Exception("层级不能为空");
                }
                if (r["类别名称"].ToString() == "")
                {
                    throw new Exception("类别名称不能为空，请检查！");
                }

            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                fun_添加同级();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (tv.Selection[0] == null) return;
                if (MessageBox.Show(string.Format("如果删除地区\"{0}\"，那么它的下级地区都将删除，你确定要删除吗？", tv.Selection[0].GetValue("地区")), "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    deleteDept(tv.Selection[0]);
                    if (tv.Selection[0].ParentNode == null)
                    {
                        tv.Selection[0].Nodes.Remove(tv.Selection[0]);
                    }
                    else
                    {
                        tv.Selection[0].ParentNode.Nodes.Remove(tv.Selection[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            try
            {
                fun_Check();

                string sql = "select  *  from  客户分类表  where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dtM);
                }
                tv.Columns.ColumnByFieldName("GUID").Visible = false;
                tv.Columns.ColumnByFieldName("上级类型GUID").Visible = false;

                tv.Columns.ColumnByFieldName("GUID").Visible = false;
                //tv.Columns.ColumnByFieldName("上级类型GUID").Visible = false;

                MessageBox.Show("保存成功！");
                simpleButton5_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
               // simpleButton5_Click_1(null, null);
            }
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            try
            {
                tv.ClearNodes();
                //string sql = string.Format("select 客户编号,客户名称  from   客户基础信息表  ");
                //dt_客户 = new DataTable();
                //dt_客户 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

                //repositoryItemGridLookUpEdit1.DataSource = dt_客户;
                //repositoryItemGridLookUpEdit1.DisplayMember = "客户编号";
                //repositoryItemGridLookUpEdit1.ValueMember = "客户编号";

                Init();
                tv.Columns.ColumnByFieldName("GUID").Visible = false;
                tv.Columns.ColumnByFieldName("上级类型GUID").Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tv_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
         

        }

        private void tv_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            //if (e.Column == tv.Columns.ColumnByFieldName("客户编码"))
            //{
            //    DataRow dr = tv.Selection[0].Tag as DataRow;
            //    dr["客户编码"] = e.Value.ToString();

            //    DataRow[] a = dt_客户.Select(string.Format("客户编号='{0}'", e.Value.ToString()));
            //    dr["客户"] = a[0]["客户名称"];
            //  //  TreeListNode nc = tv.AppendNode(new object[] { r["GUID"].ToString() }, n);
            //    //tv.Selection[0]["客户编码"].Tag
            //    tv.Selection[0].SetValue("客户", a[0]["客户名称"].ToString());

            //}



        }

        private void tv_HiddenEditor(object sender, EventArgs e)
        {
            try
            {
                saveDataFromNode();
            }
            catch (Exception ex)
            {
                //  throw ex;
                //MessageBox.Show(ex.Message);
            }
        }

        private void deleteDept(TreeListNode n)
        {
            foreach (TreeListNode nc in n.Nodes)
            {
                deleteDept(nc);
            }
            (n.Tag as DataRow).Delete();
        }
        #endregion


    }
}
