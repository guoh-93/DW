using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DevExpress.XtraTreeList.Nodes;
using CZMaster;

namespace ERPStock
{
    public partial class UI物料BOM视图 : UserControl
    {

        #region 变量
         string strconn = CPublic.Var.strConn;
         /// <summary>
         /// 接收树形的DT表
         /// </summary>
         DataTable dt_materialsBom;
         /// <summary>
         /// 该物料的父项的DT
         /// </summary>
         DataTable dt_MaterialsParent;

         /// <summary>
         /// 输入的物料编码
         /// </summary>
         string strCpID = "";

         /// <summary>
         /// 标志位
         /// </summary>
         int flag = 0;
         DataTable dt_materialsCount;
         string sql = "";
        #endregion
 
        #region  加载

        public UI物料BOM视图()
        {
            InitializeComponent(); 
        }
        public UI物料BOM视图( string s)
        {

            this.strCpID = s;
            InitializeComponent();
            sql = string.Format
                (@"select * from 基础数据物料信息表 
                    where 物料类型='成品' or 物料类型='半成品'and 物料编码='{0}'",strCpID);
        }
        private void UI物料BOM视图_Load(object sender, EventArgs e)
        {
            CZMaster.DevGridControlHelper.Helper(this);
            fun_物料信息();
            fun_BOM子图详细();
          
        }
        #endregion 

        #region  函数
        //查询某一物料的BOM结构
        private void fun_SearchMaterialsBom()
        {
            try
            {
                TreeListNode n = tv.AppendNode(new object[] { strCpID }, null);
                n.SetValue("产品编码结构", strCpID);
                DataRow[] dr = dt_materialsBom.Select(string.Format("产品编码='{0}'", strCpID));
                if (dr.Length > 0)
                {
                    n.SetValue("产品名称", dr[0]["产品名称"]);
                }
                n.SetValue("子项类型", dt_materialsBom.Rows[0]["子项类型"]);
                n.SetValue("BOM类型", dt_materialsBom.Rows[0]["BOM类型"]);
                n.SetValue("数量", 1);
                n.Tag = dt_materialsBom.Rows[0];
                Init(n);
                n.ExpandAll();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_SearchMaterialsBom");
                throw new Exception(ex.Message);
            }
        }
        private void Init(TreeListNode n)
        {
            try
            {
                DataRow[] t = null;
                if (flag == 0)
                {
                    t = dt_materialsBom.Select(string.Format("产品编码='{0}'", (n.Tag as DataRow)["产品编码"].ToString()));
                    flag++;
                }
                else
                {
                    t = dt_materialsBom.Select(string.Format("产品编码='{0}'", (n.Tag as DataRow)["子项编码"].ToString()));
                }
                foreach (DataRow r in t)
                {
                    TreeListNode nc = tv.AppendNode(new object[] { r["子项编码"] }, n);
                    nc.SetValue("产品编码结构", r["子项编码"]);
                    nc.SetValue("产品名称", r["子项名称"]);
                    nc.SetValue("子项类型", r["子项类型"]);
                    nc.SetValue("BOM类型", r["BOM类型"]);
                    nc.SetValue("数量", r["数量"]);
                    nc.Tag = r;
                    Init(nc);
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " Init");
                throw new Exception(ex.Message);
            }
        }

        private void fun_物料信息()
        {
            DataTable dt =new DataTable ();
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dataBindHelper1.DataFormDR(dt.Rows[0]);
                }
            }
        }

        private void fun_CalculateMaterialsCount()
        {
            try
            {
                foreach (DataRow r in dt_materialsCount.Rows)
                {
                    r["物料数量"] = (Convert.ToDecimal(r["物料数量"]) * Convert.ToDecimal(txt_shuliang.Text)).ToString(".0000");

                }
               gc_BOMchild.DataSource = dt_materialsCount;

                dt_materialsCount = StockCore.StockCorer.fun_物料_单_计算(strCpID, "", strconn, true);

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_CalculateMaterialsCount");
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        ///BOM中添加 该物料 的详细数量 在途量 在制量 等等 
        /// </summary>
        private void fun_详细数量()
        {
            dt_materialsCount.Columns.Add("库存总数");
            dt_materialsCount.Columns.Add("有效总数");
            dt_materialsCount.Columns.Add("在途量");
            dt_materialsCount.Columns.Add("在制量");
            dt_materialsCount.Columns.Add("受订量");
            dt_materialsCount.Columns.Add("未领量");
            dt_materialsCount.Columns.Add("MRP计划采购量");
            dt_materialsCount.Columns.Add("MRP计划生产量");
            dt_materialsCount.Columns.Add("MRP库存锁定量");

            foreach (DataRow dr_1 in dt_materialsCount.Rows)
            {
                string sql_1 = string.Format(@"select * from 仓库物料数量表 where 物料编码='{0}'",
                    dr_1["物料编码"].ToString());
                DataTable dt_1 = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(sql_1, strconn))
                {
                    da.Fill(dt_1);
                    if (dt_1.Rows.Count > 0)
                    {
                      
                        dr_1["库存总数"] = dt_1.Rows[0]["库存总数"];
                        dr_1["有效总数"] = dt_1.Rows[0]["有效总数"];
                        dr_1["在途量"] = dt_1.Rows[0]["在途量"];
                        dr_1["在制量"] = dt_1.Rows[0]["在制量"];
                        dr_1["受订量"] = dt_1.Rows[0]["受订量"];
                        dr_1["未领量"] = dt_1.Rows[0]["未领量"];
                        dr_1["MRP计划采购量"] = dt_1.Rows[0]["MRP计划采购量"];
                        dr_1["MRP计划生产量"] = dt_1.Rows[0]["MRP计划生产量"];
                        dr_1["MRP库存锁定量"] = dt_1.Rows[0]["MRP库存锁定量"];
                    }
                }
            }
        }
        private void fun_BOM子图详细()
        {
            try
            {
                tv.ClearNodes();
                //得到树形的BOM结构
  
                DataSet ds = StockCore.StockCorer.fun_得到物料BOM结构(strCpID, strconn, "");
                dt_materialsBom = ds.Tables[0];
                dt_MaterialsParent = ds.Tables[3];
                if (dt_materialsBom.Rows.Count <= 0)
                    throw new Exception("该物料没有BOM结构，请重新选择或填写！");
                fun_SearchMaterialsBom();
                gc_BOM.DataSource = dt_MaterialsParent;
                ////计算所需要的量
                dt_materialsCount = StockCore.StockCorer.fun_物料_单_计算(strCpID, "", strconn, true);
                //添加 各种数量
                fun_详细数量();

                gc_BOMchild.DataSource = dt_materialsCount;
                //gv_BOMchild.Columns["节点标记"].Visible = false;
                //gv_BOMchild.Columns["上级物料"].Visible = false;
                //flag = 0;
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
                try
                {
                    decimal d = Convert.ToDecimal(txt_shuliang.Text);
                }
                catch
                {
                    throw new Exception("计算的数量应该为数字，请重新输入！");
                }

                fun_CalculateMaterialsCount();
                fun_详细数量();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion 

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    decimal d = Convert.ToDecimal(txt_shuliang.Text);
                }
                catch
                {
                    throw new Exception("计算的数量应该为数字，请重新输入！");
                }

                fun_CalculateMaterialsCount();
                fun_详细数量();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
     

       

    }
}
