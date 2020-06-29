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
    public partial class frm物料类型维护 : UserControl
    {
        #region 私有成员
        DataTable dtM;
        SqlDataAdapter daM;
        SqlCommandBuilder brM;
        string strcon = CPublic.Var.strConn;
        #endregion

        #region 类加载
        public frm物料类型维护()
        {
            InitializeComponent();
        }

        private void frm物料类型维护_Load(object sender, EventArgs e)
        {
            try
            {
                fillCMD();   //填充类型级别的下拉框
                Init();      //树形结构显示读出
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region  其他数据的处理    
        /// <summary>
        /// 数据检查
        /// </summary>
        private void fun_Check()
        {
            foreach (DataRow r in dtM.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;

                if (r["物料类型编号"].ToString() == "")
                {
                    throw new Exception("物料类型编号不能为空，请检查！");
                }

                DataRow[] dr = dtM.Select(string.Format("物料类型编号='{0}'", r["物料类型编号"].ToString()));
                if (dr.Length >= 2)
                {
                    throw new Exception(string.Format("物料类型编号\"{0}\"有重复，请检查！",r["物料类型编号"].ToString()));
                }

                if (r["物料类型名称"].ToString() == "")
                {
                    throw new Exception("物料类型名称不能为空，请检查！");
                }

                //DataRow[] dr1 = dtM.Select(string.Format("物料类型名称='{0}'", r["物料类型名称"].ToString()));
                
                //if (dr1.Length >= 2)
                //{
                //    throw new Exception(string.Format("物料类型名称\"{0}\"有重复，请检查！", r["物料类型名称"].ToString()));
                //}

                if (r["上级类型GUID"].ToString() != "")  //有上级类型GUID的时候
                {
                    DataRow[] t = dtM.Select(string.Format("物料类型GUID='{0}'", r["上级类型GUID"]));
                    int a = t[0]["物料类型编号"].ToString().Length;
                    int b = r["物料类型编号"].ToString().Length;
                    if (b<=a)
                    {
                        throw new Exception(string.Format("有下级物料类型编号\"{0}\"的长度小于上级物料类型编号\"{1}\"的长度，请检查！", r["物料类型编号"].ToString(), t[0]["物料类型编号"].ToString()));
                    }
                    if (r["物料类型编号"].ToString().Substring(0, a) != t[0]["物料类型编号"].ToString())
                    {
                        throw new Exception(string.Format("下级物料类型编号\"{0}\"的前{1}位与上级物料类型编号\"{2}\"不一致,请检查！", r["物料类型编号"].ToString(), a.ToString(), t[0]["物料类型编号"].ToString()));
                    }
                }
            }
        }

        #endregion

        #region 数据库的读取与保存
        /// <summary>
        /// 填充下拉框：类型级别的下拉框
        /// </summary>
        private void fillCMD()
        {
            //string sqlstr;
            DataTable dt = new DataTable();
            //sqlstr = "select 属性值,属性类别 from 基础数据基础属性表 where 属性类别='产品类别' or 属性类别='计划员' order by POS";
            //SqlDataAdapter da = new SqlDataAdapter(sqlstr, CPublic.Var.strConn);
            //da.Fill(dt);
            //dt.Rows.Add(new string[] { "" });    //为什么要加一行？？？？？？？？？？？

            //foreach (DataRow r in dt.Rows)
            //{
            //    if (r["属性类别"].ToString() == "" || r["属性类别"].ToString() == "产品类别")
            //    {
            //        repositoryItemComboBox1.Items.Add(r["属性值"].ToString());
            //    }
            //    if (r["属性类别"].ToString() == "" || r["属性类别"].ToString() == "计划员")
            //    {
            //        reCoBo_计划员.Items.Add(r["属性值"].ToString());
            //    }
            //}
            //string sql = "select 员工号,姓名  from 人事基础员工表 where 课室 in ('计划课','采购课') and 在职状态='在职' ";
            //dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            //repositoryItemSearchLookUpEdit2.DataSource = dt;
            //repositoryItemSearchLookUpEdit2.DisplayMember="员工号";
            //repositoryItemSearchLookUpEdit2.ValueMember="员工号";


        }

        /// <summary>
        /// 展开节点：展开子节点
        /// </summary>
        /// <param name="n"></param>
        private void Init(TreeListNode n)
        {
            string sqlstr = "上级类型GUID = '{0}'";
            sqlstr = string.Format(sqlstr, (n.Tag as DataRow)["物料类型GUID"].ToString());
            DataRow[] rs = dtM.Select(sqlstr,"物料类型编号");
            foreach (DataRow r in rs)
            {
                TreeListNode nc = tv.AppendNode(new object[] { r["物料类型GUID"].ToString() }, n);
                nc.SetValue("物料结构", r["物料类型名称"].ToString());
                nc.SetValue("物料类型编号", r["物料类型编号"].ToString());
                nc.SetValue("物料类型名称", r["物料类型名称"].ToString());
                nc.SetValue("类型级别", r["类型级别"].ToString());
                nc.SetValue("是否成品", Convert.ToBoolean(r["是否成品"].ToString()));
                nc.SetValue("识别码", r["识别码"].ToString());
                nc.SetValue("计划员", r["计划员"].ToString());
                nc.Tag = r;
                Init(nc);
            }
        }

        /// <summary>
        /// 主节点 上级类型GUID,物料类型GUID
        /// </summary>
        private void Init()
        {
            string sqlstr;
            sqlstr = "select * from 基础数据物料类型表 order by 物料类型编号";
            daM = new SqlDataAdapter(sqlstr, CPublic.Var.strConn);
            brM = new SqlCommandBuilder(daM);
            dtM = new DataTable();
            daM.Fill(dtM);

            DataRow[] t = dtM.Select("上级类型GUID = ''");
            foreach (DataRow r in t)
            {
                TreeListNode n =tv.AppendNode(new object[] { r["物料类型GUID"].ToString() }, null);
                n.SetValue("物料结构", r["物料类型名称"].ToString());
                n.SetValue("物料类型编号", r["物料类型编号"].ToString());
                n.SetValue("物料类型名称", r["物料类型名称"].ToString());
                n.SetValue("类型级别", r["类型级别"].ToString());
                n.SetValue("是否成品", Convert.ToBoolean(r["是否成品"]));
                n.SetValue("识别码", r["识别码"].ToString());
                n.SetValue("计划员", r["计划员"].ToString());
                n.Tag = r;
                Init(n);
            }
        }

        /// <summary>
        /// 增加下级物料
        /// </summary>
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
            //检查不能添加下级物料的物料类型
            //DataRow r1 = n.Tag as DataRow;
            //if (r1["类型级别"].ToString() == "小类")
            //{
            //    throw new Exception(string.Format("物料\"{0}\"不能再添加下级物料，因为其类型级别是小类！",r1["物料类型名称"].ToString()));
            //}
            TreeListNode nc = tv.AppendNode(new object[] { "" }, n);
            nc.SetValue("层级", "");
            DataRow r = dtM.NewRow();
            r["物料类型GUID"] = System.Guid.NewGuid();
            dtM.Rows.Add(r);
            nc.Tag = r;
            if (n.GetDisplayText("") == "物料结构")
            {
                r["上级类型GUID"] = "";
            }
            else
            {
                r["上级类型GUID"] = (n.Tag as DataRow)["物料类型GUID"].ToString();
            }
            r["修改时间"] = CPublic.Var.getDatetime();
            n.ExpandAll();
        }

        /// <summary>
        /// 同级的添加
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
            nc.SetValue("类型级别", "");
            DataRow r = dtM.NewRow();
            r["物料类型GUID"] = System.Guid.NewGuid();
            dtM.Rows.Add(r);
            nc.Tag = r;
            r["修改时间"] = CPublic.Var.getDatetime();

            if (n == null)
            {
                r["上级类型GUID"] = "";
            }
            else
            {
                r["上级类型GUID"] = (n.Tag as DataRow)["物料类型GUID"].ToString();
                n.ExpandAll();
            }

        }

        //保存界面数据
        private void saveDataFromNode()
        {       
            if (tv.Selection[0] == null) return;
            tv.Selection[0].SetValue("物料结构", tv.Selection[0].GetValue("物料类型名称"));
            //给DATAROW值
            DataRow r;
            r = tv.Selection[0].Tag as DataRow;        
            r["物料类型编号"] = tv.Selection[0].GetValue("物料类型编号");     
            r["物料类型名称"] = tv.Selection[0].GetValue("物料类型名称");
            if (tv.Selection[0].ParentNode == null)   //如果是顶层节点，类型级别就一定是大类
            {
                r["类型级别"] = "大类";
            }
            else
            {
                if (tv.Selection[0].GetValue("类型级别").Equals(""))
                {
                    r["类型级别"] = "小类";
                }
                else
                {
                    r["类型级别"] = tv.Selection[0].GetValue("类型级别");
                }
            }

            r["是否成品"] = tv.Selection[0].GetValue("是否成品");
            if (tv.Selection[0].GetValue("计划员")==null||tv.Selection[0].GetValue("计划员").ToString() == "")
            {
                r["计划员"] = "";
            }
            else
            {
                r["计划员"] = tv.Selection[0].GetValue("计划员");
            }
            if (tv.Selection[0].GetValue("识别码").ToString() == "")
            {
                r["识别码"] = "";
            }
            else
            {
                r["识别码"] = tv.Selection[0].GetValue("识别码").ToString().ToUpper();  //识别码转换为大写
            }
            
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

        /// <summary>
        /// 删除的方法
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

        #region 界面操作
        /// <summary>
        /// 添加下级物料
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

        /// <summary>
        /// 添加同级物料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 数据的保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (tv.Selection[0] == null) return;
                if (MessageBox.Show(string.Format("如果删除物料\"{0}\"，那么它的下级物料都将删除，你确定要删除吗？",tv.Selection[0].GetValue("物料类型名称")), "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
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

        //界面刷新
        private void simpleButton5_Click(object sender, EventArgs e)
        {
            try
            {
                tv.ClearNodes();
                fillCMD();
                Init();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
               
                    this.tv.ExportToXlsx(saveFileDialog.FileName);
            
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
