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

namespace BaseData
{
    public partial class frm计划人员关联大小类 : UserControl
    {

        #region    变量
        string strcon = CPublic.Var.strConn;
        DataTable dt_计划人员;
        DataTable dtM;

        #endregion


        #region 加载
        public frm计划人员关联大小类()
        {
            InitializeComponent();
        }
        private void frm计划人员关联大小类_Load(object sender, EventArgs e)
        {
            try
            {
                fun_load();

            }
            catch (Exception ex) 
            {

                MessageBox.Show(ex.Message);
            }                                                                                

        }
        #endregion
        #region 函数
        private void fun_load()
        {
            string sql_人员 = string.Format(@"SELECT 员工号,姓名 FROM [FMS].[dbo].[人事基础员工表] where 课室='计划课' and 在职状态='在职'");
            using (SqlDataAdapter da = new SqlDataAdapter(sql_人员, strcon))
            {
                dt_计划人员 = new DataTable();
                da.Fill(dt_计划人员);
                gridControl1.DataSource = dt_计划人员;
            }
            string sql_大类 = "SELECT  *  FROM [FMS].[dbo].[基础数据物料类型表]";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_大类, strcon))
            {
                dtM = new DataTable();
                
                da.Fill(dtM);
                dtM.Columns.Add("选择", typeof(bool));
           
             
            }

            fun_TLhead();


        }
        /// <summary>
        ///   头结点
        /// </summary>
        private void fun_TLhead()
        {
            string sql = "SELECT * FROM [FMS].[dbo].[基础数据物料类型表] ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dtM = new DataTable();
                da.Fill(dtM);
                dtM.Columns.Add("选择", typeof(bool));
                DataRow[] dr = dtM.Select("上级类型GUID=''");
                foreach (DataRow r in dr)
                {
                    TreeListNode head = treeList1.AppendNode(new object[] { r["物料类型编号"].ToString() }, null);
                    head.SetValue("物料类型编号", r["物料类型编号"].ToString());
                    head.SetValue("物料类型名称", r["物料类型名称"].ToString());

                    head.SetValue("物料类型GUID", r["物料类型GUID"].ToString());
                    
                    head.SetValue("上级类型GUID", r["上级类型GUID"].ToString());
                   
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
                DataRow[] dr = dtM.Select(string.Format("上级类型GUID='{0}'", (n.Tag as DataRow)["物料类型GUID"].ToString()).Trim());
                foreach (DataRow r in dr)
                {
                    TreeListNode nc = treeList1.AppendNode(new object[] { r["物料类型编号"].ToString() }, n);
                    nc.SetValue("物料类型编号", r["物料类型编号"].ToString());
                    nc.SetValue("物料类型名称", r["物料类型名称"].ToString());
                    nc.SetValue("物料类型GUID", r["物料类型GUID"].ToString());

                    nc.SetValue("上级类型GUID", r["上级类型GUID"].ToString());
                   
                    nc.Tag = r;
                    fun_TL(nc);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_选择()
        {

            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
               
                foreach (TreeListNode n in treeList1.Nodes)
                {
                  

                   string sql = string.Format("select * from [计划人员大小类对应表] where 员工号='{0}' and 物料类型编号='{1}'", dr["员工号"], n.GetValue("物料类型编号").ToString());

                   
                    DataRow r = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                    if (r == null)
                    {
                        n.SetValue("选择", false);

                    }
                    else
                    {
                        n.SetValue("选择", true);
                        
                        //string b = n.GetValue(treeListColumn4).ToString();

                        //string a = n.GetValue(treeListColumn5).ToString();

                        DataRow[] drrr = dtM.Select(string.Format("物料类型编号='{0}'", n.GetValue("物料类型编号").ToString()));
                       drrr [0]["选择"] = true;
                    }

                    fun_dg(n);

                }

            

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void fun_dg(TreeListNode n)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);

            foreach (TreeListNode nz in n.Nodes)
            {
                string sql = string.Format("select * from [计划人员大小类对应表] where 员工号='{0}' and 物料类型编号='{1}'", dr["员工号"], nz.GetValue("物料类型编号").ToString());
                
               
                DataRow r = CZMaster.MasterSQL.Get_DataRow(sql, strcon);

                if (r == null)
                {
                    nz.SetValue("选择", false);
                }
                else
                {
                    nz.SetValue("选择", true);
                    dtM.Select(string.Format("物料类型编号='{0}'", nz.GetValue("物料类型编号").ToString()))[0]["选择"] = true;

                }

                if (nz.HasChildren == true)
                {
                    fun_dg(nz);
                }

            }

        }
  

        private void fun_save()
        {
            treeList1.CloseEditor();
            this.BindingContext[dtM].EndCurrentEdit();
            DataTable dt_存 = new DataTable();
            string sql_c = "select * from [计划人员大小类对应表] ";
            SqlDataAdapter da = new SqlDataAdapter(sql_c,strcon);
            da.Fill(dt_存);
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            foreach (DataRow r in dtM.Rows)
            {
                if (r["选择"].Equals(true))
                {
                    DataRow[] dr_cha = dt_存.Select(string.Format("员工号='{0}' and 物料类型编号='{1}' ", dr["员工号"], r["物料类型编号"].ToString()));
                    if (dr_cha.Length == 0)
                    {
                        DataRow rr = dt_存.NewRow();
                        rr["GUID"] = System.Guid.NewGuid();
                        rr["员工号"] = dr["员工号"];
                        rr["姓名"] = dr["姓名"];
                        rr["物料类型编号"] = r["物料类型编号"];
                        rr["物料类型名称"] = r["物料类型名称"];
                        rr["大小类"] = r["类型级别"];
                        dt_存.Rows.Add(rr);
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    DataRow[] dr_cha = dt_存.Select(string.Format("员工号='{0}' and 物料类型编号='{1}' ", dr["员工号"], r["物料类型编号"].ToString()));
                    if (dr_cha.Length > 0)
                    {
                        dr_cha[0].Delete();
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            new SqlCommandBuilder(da);
            da.Update(dt_存);
        }
        private void fun_保存界面数据()
        {
            treeList1.CloseEditor();
            this.BindingContext[dtM].EndCurrentEdit();
            if (treeList1.Selection[0] == null) return;
            //给DATAROW值
            DataRow r;
            r = treeList1.Selection[0].Tag as DataRow;

            r["物料类型编号"] = treeList1.Selection[0].GetValue("物料类型编号");
            r["物料类型名称"] = treeList1.Selection[0].GetValue("物料类型名称");
           

            if (treeList1.Selection[0].GetValue("选择") == null)
            {
                r["选择"] = false;
            }
            else
            {
                r["选择"] = treeList1.Selection[0].GetValue("选择");
            }
        }
        private void fun_search()
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string sql = string.Format("select * from [计划人员大小类对应表] where 员工号='{0}'", dr["员工号"]);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow r in dt.Rows)
                {
                    dtM.Select(string.Format("物料类型编号='{0}'", r["物料类型编号"]))[0]["选择"] = true;
                }
            }

        }
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            dtM.Columns.Remove("选择");
            dtM.Columns.Add("选择", typeof(bool));
            fun_选择();

        }
        #endregion
        //保存
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //gridView2.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                fun_save();
                MessageBox.Show("ok");
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败");
            }

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
        }

        private void treeList1_HiddenEditor(object sender, EventArgs e)
        {
            fun_保存界面数据();
        }

       

    }
}
