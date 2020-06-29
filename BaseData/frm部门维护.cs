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
namespace BaseData
{
    public partial class frm部门维护 : UserControl
    {


        #region  类成员

        DataTable dtM;
        SqlDataAdapter daM;
        SqlCommandBuilder brM;
        DataTable dt;

        //数据库的连接字符串
        string strcon= "";
        #endregion


        #region  类的加载


        public frm部门维护()
        {
            InitializeComponent();
            strcon = CPublic.Var.strConn;
        }

        private void frm部门维护_Load(object sender, EventArgs e)
        {
            try
            {
                fun_下拉框();
                fun_Init();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #endregion


        #region  其他数据处理


        /// <summary>
        /// 数据的检查
        /// </summary>
        private void fun_Check()
        {
            foreach (DataRow r in dtM.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                //部门编号的检查
                if (r["部门编号"].ToString() == "")
                {
                    throw new Exception("部门编号不能为空，请检查！");
                }

                DataRow[] dr = dtM.Select(string.Format("部门编号='{0}'", r["部门编号"].ToString()));
                if (dr.Length >= 2)
                {
                    throw new Exception(string.Format("部门编号\"{0}\"有重复，请检查！", r["部门编号"].ToString()));
                }
                //部门名称的检查
                if (r["部门名称"].ToString() == "")
                {
                    throw new Exception("部门名称不能为空，请检查！");
                }

                DataRow[] dr1 = dtM.Select(string.Format("部门名称='{0}'", r["部门名称"].ToString()));
                if (dr.Length >= 2)
                {
                    throw new Exception(string.Format("部门名称\"{0}\"有重复，请检查！", r["部门名称"].ToString()));
                }

                if (r["上级部门"].ToString() != "")
                {
                    DataRow[] t = dtM.Select(string.Format("部门GUID='{0}'", r["上级部门"]));
                    int a = t[0]["部门编号"].ToString().Length;
                    int b = r["部门编号"].ToString().Length;
                    if (b <= a)
                    {
                        throw new Exception(string.Format("下级部门\"{0}\"的编号的长度不能小于上级部门\"{1}\"编号的长度，请检查！", r["部门名称"].ToString(), t[0]["部门名称"].ToString()));
                    }
                    if(r["部门编号"].ToString().Substring(0,a)!=t[0]["部门编号"].ToString())
                    {
                        throw new Exception(string.Format("下级部门\"{0}\"的编号前\"{1}\"位，与上级部门\"{2}\"编号不相等，请检查！", r["部门名称"].ToString(), a, t[0]["部门名称"].ToString()));
                    }
                }
                //部门领导的姓名
                DataRow[] dr3 = dt.Select(string.Format("员工号='{0}'", r["部门领导"].ToString()));
                if (dr3.Length > 0)
                {
                    r["领导姓名"] = dr3[0]["姓名"];

                }
            }
        }



        #endregion


        #region   数据的读取操作

        /// <summary>
        /// 填充下拉框:部门领导的选择
        /// </summary>
        private void fun_下拉框()
        {
            string sqlstr;
            dt = new DataTable();
            sqlstr = "select 员工号,姓名,部门 from 人事基础员工表 order by 员工号";
            SqlDataAdapter da = new SqlDataAdapter(sqlstr, strcon);
            da.Fill(dt);
            dt.Rows.Add(new string[] { "", "", "" });
            repositoryItemSearchLookUpEdit1.DataSource = dt;
            repositoryItemSearchLookUpEdit1.DisplayMember = "姓名";
            repositoryItemSearchLookUpEdit1.ValueMember = "员工号";
        }


        /// <summary>
        /// 展开节点。
        /// </summary>
        /// <param name="n"></param>
        private void Init(TreeListNode n)
        {
            string sqlstr = "上级部门 = '{0}' ";
            sqlstr = string.Format(sqlstr, (n.Tag as DataRow)["部门GUID"].ToString());
            DataRow[] rs = dtM.Select(sqlstr, "部门编号");
            foreach (DataRow r in rs)
            {
                TreeListNode nc = tv.AppendNode(new object[] { r["部门GUID"].ToString() }, n);
                nc.SetValue("组织架构", r["部门名称"].ToString());
                nc.SetValue("部门编号", r["部门编号"].ToString());
                nc.SetValue("部门名称", r["部门名称"].ToString());
                nc.SetValue("部门领导", r["部门领导"].ToString());
                nc.Tag = r ;
                Init(nc);
            }
        }


        /// <summary>
        /// 主节点
        /// </summary>
        private void fun_Init()
        {
            string sqlstr;

            sqlstr = "select * from 人事基础部门表 order by 部门编号";
            daM = new SqlDataAdapter(sqlstr, strcon);
            brM = new SqlCommandBuilder(daM);
            dtM = new DataTable();
            daM.Fill(dtM);

            DataRow[] t = dtM.Select("上级部门 = ''");
            foreach (DataRow r in t)
            {
                TreeListNode n = tv.AppendNode(new object[] { r["部门GUID"].ToString() }, null);
                n.SetValue("组织架构", r["部门名称"].ToString());
                n.SetValue("部门编号", r["部门编号"].ToString());
                n.SetValue("部门名称", r["部门名称"].ToString());
                n.SetValue("部门领导", r["部门领导"].ToString());
                n.Tag = r;
                Init(n);
                n.ExpandAll();
            }
        }

        //保存界面数据
        private void saveDataFromNode()
        {
            if (tv.Selection[0] == null) return;

            tv.Selection[0].SetValue("组织架构", tv.Selection[0].GetValue("部门名称"));

            //给DATAROW值
            DataRow r;
            r = tv.Selection[0].Tag as DataRow;
            r["部门编号"] = tv.Selection[0].GetValue("部门编号");
            r["部门名称"] = tv.Selection[0].GetValue("部门名称");
            r["部门领导"] = tv.Selection[0].GetValue("部门领导");

            /*
            foreach (TreeListNode n in tv.Selection[0].Nodes)
            {
                r = n.Tag as DataRow;
                r["上级部门"] = tv.Selection[0].GetValue("部门GUID");
            }
            */
        }

        /// <summary>
        /// 添加下级部门
        /// </summary>
        private void fun_添加下级()
        {
           // int i = 0;
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
            nc.SetValue("部门领导", "");
            DataRow r = dtM.NewRow();
            r["部门GUID"] = System.Guid.NewGuid();
            dtM.Rows.Add(r);
            nc.Tag = r;
            if (n.GetDisplayText("") == "组织架构")
            {
                r["上级部门"] = "";
            }
            else
            {
                r["上级部门"] = (n.Tag as DataRow)["部门GUID"].ToString();
            }
            n.ExpandAll();
        }

        /// <summary>
        /// 同级部门的添加
        /// </summary>
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
            nc.SetValue("部门领导", "");
            DataRow r = dtM.NewRow();
            r["部门GUID"] = System.Guid.NewGuid();
            dtM.Rows.Add(r);

            nc.Tag = r;
            if (n == null)
            {
                r["上级部门"] = "";
            }
            else
            {
                r["上级部门"] = (n.Tag as DataRow)["部门GUID"].ToString();
                n.ExpandAll();
            }
        }

        /// <summary>
        ///删除
        /// </summary>
        /// <param name="n"></param>
        private void deleteDept(TreeListNode n)
        {
            foreach (TreeListNode nc in n.Nodes)
            {
                deleteDept(nc);
            }
            (n.Tag as DataRow).Delete();
        }

        #endregion


        #region   界面操作

        /// <summary>
        /// 添加下级部门
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        //同级部门的添加
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


        //部门的删除
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (tv.Selection[0] == null) return;
                if (MessageBox.Show(string.Format("如果删除部门\"{0}\"，那么它的下级部门都将被删除，确定要删除吗？", tv.Selection[0].GetValue("部门名称")), "提示？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (tv.Selection[0].ParentNode == null)
                    {
                        deleteDept(tv.Selection[0]);
                        tv.Selection[0].Nodes.Remove(tv.Selection[0]);
                    }
                    else
                    {
                        deleteDept(tv.Selection[0]);
                        tv.Selection[0].ParentNode.Nodes.Remove(tv.Selection[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //保存
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            try
            {
                fun_Check();
                daM.Update(dtM);
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
        private void tv_HiddenEditor(object sender, EventArgs e)
        {
            saveDataFromNode();
        }

        //界面的刷新
        private void simpleButton5_Click(object sender, EventArgs e)
        {
            try
            {
                tv.ClearNodes();  //刷新之前，要先清空一下节点。不然就会在原有的基础上新增节点。
                fun_下拉框();
                fun_Init();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion









       







    }
}
