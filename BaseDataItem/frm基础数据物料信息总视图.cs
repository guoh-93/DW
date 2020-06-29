using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using CPublic;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList.Nodes;

namespace BaseData
{
    public partial class frm基础数据物料信息总视图 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        SqlDataAdapter da;
        //DataTable dt;
        #endregion

        #region 自用类
        public frm基础数据物料信息总视图()
        {
            InitializeComponent();
        }

        private void frm基础数据物料信息总视图_Load(object sender, EventArgs e)
        {
            fun_载入数据();
            label1.Text = "当前物料共有" + dtM.Rows.Count.ToString() + "行！";
            Init(); 
        }

        private void xtraTabPage1_Paint(object sender, PaintEventArgs e)
        {
            //
        }

        private void xtraTabPage2_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void xtra_CloseButtonClick(object sender, EventArgs e)
        {
            DevExpress.XtraTab.XtraTabPage xt = null;
            if (xtra.SelectedTabPage.Text == "所有物料" || xtra.SelectedTabPage.Text == "物料结构" || xtra.SelectedTabPage.Text == "BOM结构")
            {

            }
            else
            {
                try
                {
                    xt = xtra.SelectedTabPage;
                    xtra.SelectedTabPageIndex = xtra.SelectedTabPageIndex - 1;
                }
                catch { }
                try
                {
                    xt.Controls[0].Dispose();
                    xtra.TabPages.Remove(xt);
                    xt.Dispose();
                }
                catch { }
            }
        }
        #endregion

        #region 数据操作
        public void fun_xtra生效选择()
        {
            if (checkBox1.Checked == true)
            {
                checkBox2.Checked = false;
            }
            if (checkBox2.Checked == true)
            {
                checkBox1.Checked = false;
            }
        }

        public void fun_载入数据()  //基础数据界面  用于快速选择数据
        {
            dtM = new DataTable();
            string sql = "select * from 基础数据物料信息表";
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            gc.DataSource = dtM;
            foreach (DataColumn dc in dtM.Columns)
            {
                //gv.Columns[dc.ColumnName.ToString()].Visible = false;
                gv.Columns[dc.ColumnName.ToString()].OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                gv.Columns[dc.ColumnName.ToString()].OptionsColumn.AllowEdit = false;
            }
            //读取本地选择项，显示选择的列
            //System.Collections.Generic.List<string> ssss = MasterCommon.LocalDataSettingBIN.getLocalData("basedateitemchoose");
            //if (ssss.Count > 0)
            //{
            //    for (int j = 0; j < ssss.Count; j++)
            //    {
            //        for (int k = 0; k < dtM.Columns.Count; k++)
            //        {
            //            if (dtM.Columns[k].ToString() == ssss[j].ToString())
            //            {
            //                gv.Columns[k].Visible = true;
            //            }
            //        }
            //    }
            //}
        }
        #endregion

        #region 所有物料界面操作
        //显示选择项
        private void button1_Click(object sender, EventArgs e)
        {
            fm请选择显示项 fm = new fm请选择显示项();
            fm.ShowDialog();

            try
            {
                foreach (DataColumn dc in dtM.Columns)
                {
                    gv.Columns[dc.ColumnName].Visible = false;
                }
                for (int j = 0; j < fm.arr.Count; j++)
                {
                    foreach (DataColumn dc in dtM.Columns)
                    {
                        if (fm.arr[j].ToString().Equals(dc.ColumnName))
                        {
                            gv.Columns[dc.ColumnName].Visible = true;
                        }
                    }
                }
            }
            catch { }
        }

        //已生效
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox2.Checked = false;
            fun_xtra生效选择();
            DataTable dt = dtM.Clone();
            DataRow[] rs = dtM.Select("生效 = true and 停用 = false");
            for (int i = 0; i < rs.Length; i++)
            {
                dt.Rows.Add(rs[i].ItemArray);
            }
            if (checkBox1.Checked == true)
            {
                gc.DataSource = dt;
                label1.Text = string.Format("当前物料共有'{0}'行！", dt.Rows.Count.ToString());
            }
            else
            {
                gc.DataSource = dtM;
                label1.Text = string.Format("当前物料共有'{0}'行！", dtM.Rows.Count.ToString());
            }
        }

        //停用
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Checked = false;
            fun_xtra生效选择();
            DataTable dt = dtM.Clone();
            DataRow[] rs = dtM.Select("停用 = true");
            for (int i = 0; i < rs.Length; i++)
            {
                dt.Rows.Add(rs[i].ItemArray);
            }
            if (checkBox2.Checked == true)
            {
                gc.DataSource = dt;
                label1.Text = "当前物料共有" + dt.Rows.Count.ToString() + "行！";
            }
            else
            {
                gc.DataSource = dtM;
                label1.Text = "当前物料共有" + dtM.Rows.Count.ToString() + "行！";
            }
        }
                                //双击进入 基础数据物料信息视图 界面
        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Clicks == 2)
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                //BaseData.frm基础数据物料信息视图 fm = new BaseData.frm基础数据物料信息视图();
                BaseData.frm基础数据物料信息视图 fm = new BaseData.frm基础数据物料信息视图(dr["物料编码"].ToString());
                XtraTabPage page = new XtraTabPage();
                page.Text = "基础数据物料信息视图";  //显示标题  
                fm.Dock = System.Windows.Forms.DockStyle.Fill;
                page.Controls.Add(fm);
                xtra.TabPages.Add(page);
                xtra.SelectedTabPage = page;  //显示生成页 
            }
        }

        string str_跳转 = ""; 
        string str_跳转2 = "";
        private void button2_Click(object sender, EventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            if (dr["物料编码"].ToString() == "")
            {

            }
            else
            {
                str_跳转 = dr["物料编码"].ToString();
                str_跳转2 = dr["小类"].ToString();
                string str_组合 = string.Format("物料编码 = '{0}' and 小类 = '{1}'", str_跳转, str_跳转2);
                xtra.SelectedTabPage = xtraTabPage2;
                tv.Focus();
                tv.SetFocusedNode(dic4[str_组合]);
            }
        }
        #endregion

        #region 物料结构界面操作

        Dictionary<string, TreeListNode> dic1 = new Dictionary<string, TreeListNode>();
        Dictionary<string, TreeListNode> dic2 = new Dictionary<string, TreeListNode>();
        Dictionary<string, TreeListNode> dic3= new Dictionary<string, TreeListNode>();
        Dictionary<string, TreeListNode> dic4 = new Dictionary<string, TreeListNode>();
        // 主节点
        private void Init()
        {
            try
            {
                string sqll = "select 产品线, 大类 ,小类 from 基础数据物料信息表 group by 产品线, 大类 ,小类";
                DataTable dt = new DataTable();
                SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
                daa.Fill(dt);
                dic1.Clear(); dic2.Clear(); dic3.Clear();
                
                foreach (DataRow r in dt.Rows)
                {
                    string selectsql1 = string.Format("产品线= '{0}'", r["产品线"].ToString());
                    if (dic1.ContainsKey(selectsql1) == false)
                    {
                        dic1.Add(selectsql1, tv.AppendNode(new object[] { r["产品线"].ToString() == "" ? "未定义生产线" : r["产品线"].ToString(), }, null));
                        dic1[selectsql1].Tag = selectsql1;
                    }

                    string selectsql2 = string.Format("产品线= '{0}' and 大类= '{1}' ", r["产品线"].ToString(), r["大类"].ToString());              
                    if (dic1.ContainsKey(selectsql2) == false)
                    {
                        dic1.Add(selectsql2, tv.AppendNode(new object[] { r["大类"].ToString() }, dic1[selectsql1]));
                        dic1[selectsql2].Tag = selectsql2;
                    }

                    string selectsql3 = string.Format("产品线= '{0}' and 大类= '{1}' and 小类 = '{2}'", r["产品线"].ToString(), r["大类"].ToString(), r["小类"].ToString());
                    if (dic1.ContainsKey(selectsql3) == false)
                    {
                        dic3.Add(selectsql3, tv.AppendNode(new object[] { r["小类"].ToString() }, dic1[selectsql2]));
                        dic3[selectsql3].Tag = selectsql3;
                    }
                }

                foreach (string key in dic1.Keys)
                {
                    dic1[key].ExpandAll();
                }

                foreach (string key in dic3.Keys)
                {
                    DataRow[] ds = dtM.Select(key);
                    foreach (DataRow rr in ds)
                    {
                        TreeListNode nc = tv.AppendNode(new object[] { rr["小类"].ToString() }, dic3[key]);
                        nc.SetValue("物料编码", rr["物料编码"].ToString());
                        nc.SetValue("物料名称", rr["物料名称"].ToString());
                        nc.SetValue("规格型号", rr["规格型号"].ToString());
                        string selectsql4 = string.Format("物料编码 = '{0}' and 小类 = '{1}'", rr["物料编码"].ToString(), rr["小类"].ToString());
                        dic4.Add(selectsql4, nc);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
       
        //双击进入 基础数据物料信息视图 界面
        private void tv_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                string a = tv.FocusedNode.GetValue("物料编码").ToString();
                //if (a == "")
                //{

                //}
                //else
                {
                    //BaseData.frm基础数据物料信息视图 fm = new BaseData.frm基础数据物料信息视图();
                    BaseData.frm基础数据物料信息视图 fm = new BaseData.frm基础数据物料信息视图(a);
                    XtraTabPage page = new XtraTabPage();
                    page.Text = "基础数据物料信息视图";  //显示标题  
                    fm.Dock = System.Windows.Forms.DockStyle.Fill;
                    page.Controls.Add(fm);
                    xtra.TabPages.Add(page);
                    xtra.SelectedTabPage = page;  //显示生成页 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tv_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                //string a = tv.FocusedNode.GetDisplayText(0);
                if (tv.FocusedNode.Tag == null)
                {
                    string sql = tv.FocusedNode.ParentNode.Tag.ToString();
                    label1.Text = "当前类别下有" + dtM.Select(sql).Length.ToString() + "条数据!";
                }
                else
                {
                    string sql = tv.FocusedNode.Tag.ToString();
                    label1.Text = "当前类别下有" + dtM.Select(sql).Length.ToString() + "条数据!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
    }
}
