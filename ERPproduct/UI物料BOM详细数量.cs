using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DevExpress.XtraTreeList.Nodes;
using System.IO;
using System.Reflection;
using CZMaster;
using System.Threading;

namespace ERPproduct
{
    public partial class UI物料BOM详细数量 : UserControl
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
        DataTable dt_Bom;
        DataTable dt_仓库;

        /// <summary>
        /// 输入的物料编码
        /// </summary>
        string strCpID = "";
        decimal dec_数量 = 1;
        string str_制令 = "";

        /// <summary>
        /// 标志位
        /// </summary>
        int flag = 0;
        DataTable dt_materialsCount;
        string sql = "";
        decimal a = 0;
        DataTable dt_物料;
        string cfgfilepath = "";
        #endregion

        #region  加载
        public UI物料BOM详细数量()
        {
            InitializeComponent();
            fun_searchMaterials();
            sql = "";
        }
        public UI物料BOM详细数量(string s, decimal dec_数量)
        {


            InitializeComponent();
            fun_searchMaterials();
            this.strCpID = s;
            this.dec_数量 = dec_数量;
            txt_shuliang.Text = dec_数量.ToString("#0.####");
            sql = string.Format
                (@"select 物料编码,物料名称,规格型号,物料类型,大类,小类 from 基础数据物料信息表 where 物料编码='{0}'", strCpID);
            la_1.Visible = false;
            cb_物料.Visible = false;
            cb_物料.EditValue = s;
            button2.Visible = false;



        }
        public UI物料BOM详细数量(string s, decimal dec_数量, string str_制令号)
        {
            InitializeComponent();
            fun_searchMaterials();

            this.strCpID = s;
            this.dec_数量 = dec_数量;
            txt_shuliang.Text = dec_数量.ToString("#0.####");
            sql = string.Format
                (@"select 物料编码,物料名称,规格型号,图纸编号,大类,小类 from 基础数据物料信息表  where  物料编码='{0}'", strCpID);
            la_1.Visible = false;
            cb_物料.Visible = false;
            cb_物料.EditValue = s;

            button2.Visible = false;

            str_制令 = str_制令号;

        }


        private void UI物料BOM详细数量_Load(object sender, EventArgs e)
        {
            try
            {
                string permgroup = CPublic.Var.LocalUserTeam;
                string s = string.Format("select * from [权限组按钮权限表] where 权限组='{0}' and 权限类型='BOM信息查询' ", permgroup);
                DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                foreach (object item in barManager1.Items)
                {
                    if (item.GetType() == typeof(DevExpress.XtraBars.BarLargeButtonItem))
                    {
                        DevExpress.XtraBars.BarLargeButtonItem xx = item as DevExpress.XtraBars.BarLargeButtonItem;
                        xx.Enabled = ERPorg.Corg.btn_perm(t, xx.Caption);

                    }
                    //item.Enabled = ERPorg.Corg.btn_perm(t,item.Caption);

                }

                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(xtraTabControl1, this.Name, cfgfilepath);
                //CZMaster.DevGridControlHelper.Helper(this);
                if (sql != "")
                {
                    fun_物料信息();
                    fun_load(strCpID);
                    fun_BOM子图详细();
                    if (dec_数量 > 1)
                    {
                        simpleButton2_Click_1(null, null);
                    }
                }
                //if (CPublic.Var.LocalUserTeam=="管理员权限"||CPublic.Var.LocalUserTeam== "开发部权限" || CPublic.Var.LocalUserTeam == "工艺部权限")
                //{
                //    barLargeButtonItem2.Enabled = true;
                //}
                //else
                //{
                //    barLargeButtonItem2.Enabled = false;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        #region  函数
        //物料编码的下拉框
#pragma warning disable IDE1006 // 命名样式
        private void fun_searchMaterials()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                SqlDataAdapter da;
                string sql = string.Format("select 物料编码,物料名称,规格型号,图纸编号,物料类型,物料等级,壳架等级,大类,小类 from 基础数据物料信息表 where left(物料编码,2) not in ('20','30','11')");//where 物料类型='成品' or 物料类型='半成品'
                da = new SqlDataAdapter(sql, strconn);
                dt_物料 = new DataTable();
                da.Fill(dt_物料);
                cb_物料.Properties.DataSource = dt_物料;
                cb_物料.Properties.DisplayMember = "物料编码";
                cb_物料.Properties.ValueMember = "物料编码";
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_searchMaterials");
                throw new Exception(ex.Message);
            }
        }

        //查询某一物料的BOM结构
#pragma warning disable IDE1006 // 命名样式
        private void fun_SearchMaterialsBom()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                tv.ClearNodes();
                TreeListNode n = tv.AppendNode(new object[] { strCpID }, null);
                n.SetValue("产品编码结构", strCpID);
                DataRow[] dr = dt_materialsBom.Select(string.Format("产品编码='{0}'", strCpID));
                if (dr.Length > 0)
                {
                    n.SetValue("产品名称", dr[0]["产品名称"]);
                }
                n.SetValue("物料编号", dr[0]["父项编号"]);
                n.SetValue("规格型号", dr[0]["父项规格"]);
                n.SetValue("图纸编号", dr[0]["父项图纸"]);

                n.SetValue("子项类型", dt_materialsBom.Rows[0]["子项类型"]);
                n.SetValue("BOM类型", dt_materialsBom.Rows[0]["BOM类型"]);
                n.SetValue("数量", 1);
                n.SetValue("A面位号", dr[0]["A面位号"]);

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
                    nc.SetValue("物料编号", r["子项编号"]);
                    nc.SetValue("规格型号", r["子项规格"]);
                    nc.SetValue("图纸编号", r["子项图纸"]);
                    nc.SetValue("产品名称", r["子项名称"]);
                    nc.SetValue("子项类型", r["子项类型"]);
                    nc.SetValue("BOM类型", r["BOM类型"]);
                    nc.SetValue("仓库号", r["仓库号"]);
                    nc.SetValue("仓库名称", r["仓库名称"]);
                    nc.SetValue("A面位号", r["A面位号"]);


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

#pragma warning disable IDE1006 // 命名样式
        private void fun_物料信息()
#pragma warning restore IDE1006 // 命名样式
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dataBindHelper1.DataFormDR(dt.Rows[0]);
                }
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_CalculateMaterialsCount()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                foreach (DataRow r in dt_materialsCount.Rows)
                {
                    r["库存可制量"] = (Convert.ToDecimal(r["库存总数"]) / Convert.ToDecimal(r["数量"])).ToString(".0000");
                    r["当前需求量"] = (Convert.ToDecimal(r["数量"]) * Convert.ToDecimal(txt_shuliang.Text)).ToString("#0.######");
                }
                gc_BOMchild.DataSource = dt_materialsCount;
                button1.Text = "显示主料";
                gv_BOMchild.ViewCaption = "物料BOM子项组成表(全部)";

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_CalculateMaterialsCount");
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        ///BOM中添加 该物料 的详细数量 在途量 在制量 等等 
        ///
        /// 
        /// 计算库存可制量
        /// 
        /// </summary>
        //private void fun_详细数量()
        //{
        //    //dt_materialsCount.Columns.Add("库存可制量");
        //    //dt_materialsCount.Columns.Add("库存总数");
        //    //dt_materialsCount.Columns.Add("有效总数");
        //    //dt_materialsCount.Columns.Add("在途量");
        //    //dt_materialsCount.Columns.Add("在制量");
        //    //dt_materialsCount.Columns.Add("受订量");
        //    //dt_materialsCount.Columns.Add("未领量");
        //    //dt_materialsCount.Columns.Add("MRP计划采购量");
        //    //dt_materialsCount.Columns.Add("MRP计划生产量");
        //    //dt_materialsCount.Columns.Add("MRP库存锁定量");

        //    foreach (DataRow dr_1 in dt_materialsCount.Rows)
        //    {
        //        //string sql_1 = string.Format("select * from 仓库物料数量表 where 物料编码='{0}'",
        //        //    dr_1["物料编码"].ToString());
        //        //DataTable dt_1 = new DataTable();
        //        //using (SqlDataAdapter da = new SqlDataAdapter(sql_1, strconn))
        //        //{
        //        //    da.Fill(dt_1);
        //        //    if (dt_1.Rows.Count > 0)
        //        //    {
        //        //        dr_1["库存总数"] = dt_1.Rows[0]["库存总数"];
        //        //        dr_1["有效总数"] = dt_1.Rows[0]["有效总数"];
        //        //        dr_1["在途量"] = dt_1.Rows[0]["在途量"];
        //        //        dr_1["在制量"] = dt_1.Rows[0]["在制量"];
        //        //        dr_1["受订量"] = dt_1.Rows[0]["受订量"];
        //        //        dr_1["未领量"] = dt_1.Rows[0]["未领量"];
        //        //        dr_1["MRP计划采购量"] = dt_1.Rows[0]["MRP计划采购量"];
        //        //        dr_1["MRP计划生产量"] = dt_1.Rows[0]["MRP计划生产量"];
        //        //        dr_1["MRP库存锁定量"] = dt_1.Rows[0]["MRP库存锁定量"];


        //        //    }

        //        //}
        //        try
        //        {
        //            dr_1["库存可制量"] = Math.Floor(Convert.ToDecimal(dr_1["库存总数"]) / Convert.ToDecimal(dr_1["BOM数量"]));

        //        }
        //        catch (Exception ex )
        //        {
        //            dr_1["库存可制量"] = 0;
        //        }
        //        //if(Convert.ToDecimal( dr_1["库存总数"])<Convert.ToDecimal((dr_1["物料数量"])) )
        //        //{


        //        //}
        //    }
        //}
#pragma warning disable IDE1006 // 命名样式
        private void fun_BOM子图详细()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {


                tv.ClearNodes();
                /* 2018-4-19 备注
                //得到树形的BOM结构

                DataSet ds = StockCore.StockCorer.fun_得到物料BOM结构(strCpID, strconn, "");
                dt_materialsBom = new DataTable();
                dt_materialsBom = ds.Tables[0];
                //dt_MaterialsParent = ds.Tables[3];
                */
                newfun_tree();
                //if (dt_materialsBom.Rows.Count > 0)
                //{    //throw new Exception("该物料没有BOM结构，请重新选择或填写！");
                //    fun_SearchMaterialsBom();
                //    //gc_BOM.DataSource = dt_MaterialsParent;
                //}
                ////计算所需要的量

                //dt_materialsCount = StockCore.StockCorer.fun_物料_单_计算(strCpID, "", strconn, true);
                fun_BOMchild();
                //添加 各种数量
                fun_CalculateMaterialsCount();  //  计算库存可制量

                //string sql = string.Format(@"exec parbom '{0}'", strCpID);
                DateTime dtime = CPublic.Var.getDatetime().Date;                              
                DateTime dtime1 = dtime.AddYears(-1);  //一年前
                string t0 = dtime.ToString("yyyy-MM-dd");
                string t1 = dtime1.ToString("yyyy-MM-dd");
                Thread th = new Thread(() =>
                {
                    //                   string s = string.Format(@" with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,bom_level ) as
                    //(select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,1 as level from 基础数据物料BOM表 
                    //  where 子项编码='{0}'
                    //  union all 
                    //  select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型,a.数量,a.bom类型,b.bom_level+1  from 基础数据物料BOM表 a
                    //  inner join temp_bom b on a.子项编码=b.产品编码 
                    //  ) 
                    //         select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,wiptype,子项类型,数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号 as 子项规格 from  temp_bom a
                    // left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
                    // left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码", strCpID);

                    string s = string.Format(@"with temp_bom(产品编码, 子项编码, 仓库号, 仓库名称, wiptype, 子项类型, 数量, bom类型, bom_level ) as
         (select 产品编码, 子项编码, 仓库号, 仓库名称, WIPType, 子项类型, 数量, bom类型,1 as level from 基础数据物料BOM表
           where 子项编码 = '{0}'
           union all
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型,a.数量,a.bom类型,b.bom_level + 1  from 基础数据物料BOM表 a
     inner join temp_bom b on a.子项编码 = b.产品编码   ) 
          select 子项编码, 子项名称, 产品编码,  产品名称,wiptype,子项类型,数量,bom类型, 仓库号, 仓库名称
  , bom_level, 子项规格,停用,isnull(年用量,0)年用量   from (
  select 产品编码 as 子项编码,fx.物料名称 as 子项名称,子项编码 as 产品编码,base.物料名称 as 产品名称,wiptype,子项类型,数量,bom类型,a.仓库号,a.仓库名称
  , bom_level,fx.规格型号 as 子项规格,fx.停用,年用量 from temp_bom a
  left  join 基础数据物料信息表 base on base.物料编码 = a.子项编码
     left  join 基础数据物料信息表 fx  on fx.物料编码 = a.产品编码
     left join  (select 物料编码,-sum(实效数量)as 年用量  from 仓库出入库明细表 where  出库入库='出库'  and  出入库时间>='{1}' and 
     出入库时间<='{2}'  group by 物料编码)c on  a.产品编码=c.物料编码 )dd  
     group by 子项编码, 子项名称, 产品编码,  产品名称,wiptype,子项类型,数量,bom类型, 仓库号, 仓库名称, bom_level, 子项规格,停用,年用量", strCpID, t1, t0);
  //                  string s = string.Format(@"with temp_bom(产品编码, 子项编码, 仓库号, 仓库名称, wiptype, 子项类型, 数量, bom类型, bom_level ) as
  //       (select 产品编码, 子项编码, 仓库号, 仓库名称, WIPType, 子项类型, 数量, bom类型,1 as level from 基础数据物料BOM表
  //         where 子项编码 = '{0}'
  //         union all
  // select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型,a.数量,a.bom类型,b.bom_level + 1  from 基础数据物料BOM表 a
  //   inner join temp_bom b on a.子项编码 = b.产品编码   ) 
  //        select 子项编码, 子项名称, 产品编码,  产品名称,wiptype,子项类型,数量,bom类型, 仓库号, 仓库名称
  //, bom_level, 子项规格,停用   from (
  //select 产品编码 as 子项编码,fx.物料名称 as 子项名称,子项编码 as 产品编码,base.物料名称 as 产品名称,wiptype,子项类型,数量,bom类型,a.仓库号,a.仓库名称
  //, bom_level,fx.规格型号 as 子项规格,fx.停用  from temp_bom a
  //left  join 基础数据物料信息表 base on base.物料编码 = a.子项编码
  //   left  join 基础数据物料信息表 fx  on fx.物料编码 = a.产品编码 
  //     )dd  
  //   group by 子项编码, 子项名称, 产品编码,  产品名称,wiptype,子项类型,数量,bom类型, 仓库号, 仓库名称, bom_level, 子项规格,停用 ", strCpID);
                    // 19-12-26  这里产品 和子项 显示的是 反的   关联的 基础表条件（显示停用)需要是 反的 
                    DataTable dt = new DataTable();
                    dt = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                    //dt = ERPorg.Corg.fun_GetFather(dt, strCpID, 1, false);
                    //dt.Columns.Add("物料名称");
                    //dt.Columns.Add("存货分类");
                    //dt.Columns.Add("规格型号");
                    //dt.Columns.Add("计量单位");
                    //foreach (DataRow dr in dt.Rows)
                    //{
                    //    string s = string.Format("select  物料编码,物料名称,存货分类,规格型号,计量单位 from 基础数据物料信息表 where  物料编码='{0}' ", dr["产品编码"]);
                    //    DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                    //    dr["物料名称"] = t.Rows[0]["物料名称"];
                    //    dr["存货分类"] = t.Rows[0]["存货分类"];
                    //    dr["规格型号"] = t.Rows[0]["规格型号"];
                    //    dr["计量单位"] = t.Rows[0]["计量单位"];

                    //}

                    BeginInvoke(new MethodInvoker(() =>
                    {
                        newfun_tree_fx(dt);
                    }));
                });
                th.IsBackground = true;

                th.Start();





                //DataTable dt = new DataTable();
                //SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                //da.Fill(dt);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        int x = 0;
        private void newfun_tree_fx(DataTable dt_bom_fx)
        {
            x = 0;
            treeList1.ClearNodes();
            string s = string.Format("select  * from 基础数据物料信息表 where 物料编码='{0}' ", strCpID);
            DataRow r = CZMaster.MasterSQL.Get_DataRow(s, strconn);
            TreeListNode head = treeList1.AppendNode(new object[] { r["物料编码"] }, null);
            head.SetValue("物料编码", r["物料编码"].ToString());
            head.SetValue("产品名称", r["物料名称"].ToString());
            head.SetValue("规格型号", r["规格型号"].ToString());
            head.SetValue("停用", Convert.ToBoolean( r["停用"]));
            head.Tag = r;
            fun_FX(dt_bom_fx, head, r["物料编码"].ToString(), 0);
            head.ExpandAll();

        }
        private void fun_FX(DataTable dt_bom_fx, TreeListNode n, string str_fx, int cj)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                DataRow[] rr = dt_bom_fx.Select(string.Format("产品编码='{0}' and  bom_level={1}", str_fx, cj + 1));

                foreach (DataRow r in rr)
                {
                    //TreeListNode nc = treeList1.AppendNode(new object[] { r["权限类型"].ToString() }, n);
                    TreeListNode nc = treeList1.AppendNode(new object[] { r["子项编码"].ToString() }, n);
                    // nc.SetValue("产品编码结构", r["子项编码"].ToString());
                    nc.SetValue("子项类型", r["子项类型"].ToString());
                    nc.SetValue("物料编码", r["子项编码"].ToString());
                    nc.SetValue("产品名称", r["子项名称"].ToString());
                    nc.SetValue("规格型号", r["子项规格"].ToString());
                    nc.SetValue("BOM类型", r["BOM类型"].ToString());
                    nc.SetValue("数量", Convert.ToDecimal(r["数量"]));
                    nc.SetValue("WIPType", r["WIPType"].ToString());
                    nc.SetValue("层级", r["bom_level"].ToString());
                    nc.SetValue("仓库号", r["仓库号"].ToString());
                    nc.SetValue("仓库名称", r["仓库名称"].ToString());
                    nc.SetValue("停用", Convert.ToBoolean( r["停用"]));
                    nc.SetValue("年用量", Convert.ToDecimal(r["年用量"]));
                    nc.Tag = r;


                    fun_FX(dt_bom_fx, nc, r["子项编码"].ToString(), cj + 1);

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



#pragma warning disable IDE1006 // 命名样式
        private void fun_BOMchild()
#pragma warning restore IDE1006 // 命名样式
        {

            string sql = string.Format(@"exec wlBOM '{0}',{1}", strCpID, dec_数量);
            dt_materialsCount = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            dt_materialsCount.Columns.Add("此单可用量", typeof(decimal));
            if (str_制令 != "") //其他需求数量   总需数-当前制令数量*BOM数量（因为你不知道有没有生效）-其他已生效制令的已发料数量
            {
                foreach (DataRow r in dt_materialsCount.Rows)
                {
                    string xx = string.Format(@"exec othernc '{0}','{1}','{2}'", r["产品编码"], str_制令, r["子项编码"]);
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(xx, strconn);
                    if (dt.Rows.Count > 0)
                    {
                        decimal de = Convert.ToDecimal(r["其他需求量"]) - Convert.ToDecimal(dt.Rows[0][0]);
                        r["其他需求量"] = de < 0 ? 0 : de;
                    }
                    decimal kcky = 0;
                    kcky = Convert.ToDecimal(r["库存总数"]) - Convert.ToDecimal(r["其他需求量"]);
                    r["此单可用量"] = kcky < 0 ? 0 : kcky;
                }
            }
            gc_BOMchild.DataSource = dt_materialsCount;
        }
        #endregion
        //计算
        
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click_1(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
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
                //fun_详细数量();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_BOMchild_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            if (e.Column.Caption == "库存总数")
            {
                DataRow drrr = gv_BOMchild.GetDataRow(e.RowHandle);

                a = Convert.ToDecimal(drrr["数量"]) * Convert.ToDecimal(txt_shuliang.Text);
                if (Convert.ToDecimal(e.CellValue) < a)
                {
                    e.Appearance.BackColor = Color.Red;
                }
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_BOMchild_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow dr = gv_BOMchild.GetDataRow(gv_BOMchild.FocusedRowHandle);
                string sql = string.Format(@"select bom.* ,base.大类,base.小类,isnull(kc.库存总数,0)库存总数, isnull(kc.有效总数,0)有效总数,isnull(kc.在途量,0)在途量,base.规格型号,
                 isnull(kc.在制量,0)在制量,isnull(kc.受订量,0)受订量,isnull(kc.未领量,0)未领量,(isnull(库存总数,0)/数量) as 库存可制量,数量*{1} as 当前需求量,bom.仓库号,bom.仓库名称
                from 基础数据物料BOM表 bom left join 基础数据物料信息表 base on base.物料编码= bom.子项编码 
               left join 仓库物料数量表 kc on kc.物料编码= bom.子项编码 and kc.仓库号=bom.仓库号
                where bom.产品编码='{0}' ", dr["子项编码"], dec_数量);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                gridControl1.DataSource = dt;
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc_BOMchild, new Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 物料明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv_BOMchild.GetDataRow(gv_BOMchild.FocusedRowHandle);
            Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, @"ERPStock.dll"));
            Type outerForm = outerAsm.GetType("ERPStock.frm仓库物料数量明细", false);
            //  Form ui = Activator.CreateInstance(outerForm) as Form;
            object[] dic = new object[2];
            dic[0] = dr["子项编码"].ToString();
            dic[1] = dr["仓库号"].ToString();

            UserControl ui = Activator.CreateInstance(outerForm, dic) as UserControl; // 过往出口明细 构造函数 有两个参数,string ,datetime 
            CPublic.UIcontrol.Showpage(ui, "仓库物料数量明细");
            //ERPStock.frm仓库物料数量明细 frm = new ERPStock.frm仓库物料数量明细(dr["子项编码"].ToString());
            //CPublic.UIcontrol.AddNewPage(frm, "物料数量明细");
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (button1.Text == "显示主料")
            {
                DataView dv = new DataView(dt_materialsCount)
                {
                    RowFilter = "主辅料='主料'"
                };
                button1.Text = "显示辅料";
                gc_BOMchild.DataSource = dv;
                gv_BOMchild.ViewCaption = "物料BOM子项组成表(主料)";
            }
            else if (button1.Text == "显示辅料")
            {
                DataView dv = new DataView(dt_materialsCount)
                {
                    RowFilter = "主辅料='辅料'"
                };
                button1.Text = "显示全部";
                gc_BOMchild.DataSource = dv;
                gv_BOMchild.ViewCaption = "物料BOM子项组成表(辅料)";
            }
            else
            {
                gc_BOMchild.DataSource = dt_materialsCount;
                button1.Text = "显示主料";
                gv_BOMchild.ViewCaption = "物料BOM子项组成表(全部)";
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void button2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                if (cb_物料.EditValue == null) throw new Exception("未选择物料");
                flag = 0;
                this.strCpID = cb_物料.EditValue.ToString();
                this.dec_数量 = 1;
                txt_shuliang.Text = dec_数量.ToString();
                sql = string.Format
                    (@"select * from 基础数据物料信息表 
                    where 物料编码='{0}'", strCpID);
                //CZMaster.DevGridControlHelper.Helper(this);
                fun_物料信息();
                fun_load(strCpID);
                fun_BOM子图详细();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load(string s_wl)
        {
            string s = string.Format(@"with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,bom_level,A面位号 ) as
 (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,1 as level,A面位号 from 基础数据物料BOM表 
   where 产品编码='{0}'
   union all 
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型,a.数量,a.bom类型,b.bom_level+1,a.A面位号  from 基础数据物料BOM表 a
   inner join temp_bom b on a.产品编码=b.子项编码 
   ) 
      select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,wiptype,子项类型,数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号 as 子项规格,base.供应状态,A面位号 from  temp_bom a
  left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
  left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码
  group by 产品编码,fx.物料名称 ,子项编码,base.物料名称 ,wiptype,子项类型,数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号,base.供应状态,A面位号", s_wl);
            dt_Bom = CZMaster.MasterSQL.Get_DataTable(s, strconn);

            s = "select   属性字段1 as 仓库号,属性值 as 仓库名称 from [基础数据基础属性表] where 属性类别='仓库类别' and 布尔字段4 = 1";
            dt_仓库 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            repositoryItemGridLookUpEdit1.DataSource = dt_仓库;
            repositoryItemGridLookUpEdit1.DisplayMember = "仓库号";
            repositoryItemGridLookUpEdit1.ValueMember = "仓库号";


            s = "select 属性值 as 领料类型 from 基础数据基础属性表 where 属性类别 = 'WIPType'";
            DataTable dt_领料类型 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(s, strconn);
            da.Fill(dt_领料类型);
            foreach (DataRow dr in dt_领料类型.Rows)
            {
                repositoryItemComboBox1.Items.Add(dr["领料类型"].ToString());

            }
            //repositoryItemComboBox1.DisplayMember = "领料类型";
            //repositoryItemComboBox1.ValueMember = "领料类型";
        }


#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "导出Excel",
                    Filter = "Excel文件(*.xlsx)|*.xlsx"
                };
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    //gc_BOMchild.ExportToXls(saveFileDialog.FileName);
                    if (xtraTabControl1.SelectedTabPage.Name == "xtraTabPage2")
                    {
                        treeList1.ExportToXlsx(saveFileDialog.FileName);
                    }

                    else if (xtraTabControl1.SelectedTabPage.Name == "xtraTabPage1")
                    {
                        this.tv.ExportToXlsx(saveFileDialog.FileName);
                    }
                    else
                    {
                        gc_BOMchild.ExportToXlsx(saveFileDialog.FileName);

                    }
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);


                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
                MessageBox.Show(ex.Message);
            }
        }

        //#pragma warning disable IDE1006 // 命名样式
        //        private void gv_BOMchild_KeyDown(object sender, KeyEventArgs e)
        //#pragma warning restore IDE1006 // 命名样式
        //        {
        //            if (e.Control & e.KeyCode == Keys.C)
        //            {
        //                Clipboard.SetDataObject(gv_BOMchild.GetFocusedRowCellValue(gv_BOMchild.FocusedColumn));
        //                e.Handled = true;
        //            }
        //        }

        //#pragma warning disable IDE1006 // 命名样式
        //        private void gridView2_KeyDown(object sender, KeyEventArgs e)
        //#pragma warning restore IDE1006 // 命名样式
        //        {
        //            if (e.Control & e.KeyCode == Keys.C)
        //            {
        //                Clipboard.SetDataObject(gridView2.GetFocusedRowCellValue(gridView2.FocusedColumn));
        //                e.Handled = true;
        //            }
        //        }

        //#pragma warning disable IDE1006 // 命名样式
        //        private void gv_BOMchild_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        //#pragma warning restore IDE1006 // 命名样式
        //        {
        //            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
        //            {

        //                e.Info.DisplayText = (e.RowHandle + 1).ToString();

        //            }
        //        }

#pragma warning disable IDE1006 // 命名样式
        //        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        //#pragma warning restore IDE1006 // 命名样式
        //        {
        //            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
        //            {

        //                e.Info.DisplayText = (e.RowHandle + 1).ToString();

        //            }
        //        }

        private void 查看图纸ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv_BOMchild.GetDataRow(gv_BOMchild.FocusedRowHandle);
                string strConn_FS = CPublic.Var.geConn("FS");
                string sql = string.Format(@"select * from [基础物料蓝图表] where 物料号='{0}'  and 版本=0", dr["原ERP物料编号"]);
                DataRow rr = CZMaster.MasterSQL.Get_DataRow(sql, strconn);

                if (rr == null || rr["文件地址"] == null || rr["文件地址"].ToString() == "")
                {
                    throw new Exception("未上传文件,没有文件可以查看");
                }
                string type = rr["后缀"].ToString();
                string foldPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                //     string fileName = foldPath + DateTime.Now.ToString("yyyy-MM-dd") + "T" + DateTime.Now.ToString("HH_mm_ss") + "Z" + "_" + Guid.NewGuid().ToString() + "." + type;
                string fileName = foldPath + "预览." + type;
                try
                {
                    System.IO.Directory.Delete(foldPath, true);
                }
                catch (Exception)
                {

                }
                CFileTransmission.CFileClient.strCONN = strConn_FS;
                CFileTransmission.CFileClient.Receiver(rr["文件地址"].ToString(), fileName);

                ItemInspection.ui预览文件 ui = new ItemInspection.ui预览文件(fileName);
                CPublic.UIcontrol.Showpage(ui, "预览文件");
                //System.Diagnostics.Process.Start(fileName);




                //axAcroPDF1.Visible = true;
                //axAcroPDF1.LoadFile(fileName);
                //axAcroPDF1.setView("readonly");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void newfun_tree()
        {
            string s = string.Format("select  * from 基础数据物料信息表 where 物料编码='{0}' ", strCpID);
            DataRow r = CZMaster.MasterSQL.Get_DataRow(s, strconn);
            TreeListNode head = tv.AppendNode(new object[] { r["物料编码"] }, null);
            head.SetValue("物料编码", r["物料编码"].ToString());
            head.SetValue("产品名称", r["物料名称"].ToString());
            head.SetValue("规格型号", r["规格型号"].ToString());
            head.Tag = r;
            fun_TL(head, r["物料编码"].ToString(), 0);
            head.ExpandAll();

        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 展开子节点
        /// </summary>
        /// <param name="n"></param>
        //        private void fun_TL(TreeListNode n, string str_fx)
        //#pragma warning restore IDE1006 // 命名样式
        //        {
        //            try
        //            {
        //                DataTable dt = new DataTable();
        //                string s = string.Format(@" select   base.子件损耗率,base.WIPType,base.BOM类型,a.物料编码 as 父项编号,a.物料类型 as 父项类型,a.大类 as 父项大类
        //            ,a.小类 as 父项小类,a.规格型号 as 父项规格 ,b.物料编码 as 子项编码,b.物料名称 as 子项名称,
        //            b.物料类型 as 子项类型,b.图纸编号 as 子项图号 ,b.大类 as 子项大类,b.小类 as 子项小类,b.规格型号 as 子项规格 ,数量,base.仓库号,base.仓库名称  from 基础数据物料BOM表  base 
        //            left join 基础数据物料信息表 a  on base.产品编码=a.物料编码
        //            left join 基础数据物料信息表 b  on base.子项编码=b.物料编码 where   a.物料编码='{0}'",str_fx);
        //                using (SqlDataAdapter da = new SqlDataAdapter(s, strconn))
        //                {
        //                    da.Fill(dt);
        //                }
        //                foreach (DataRow r in dt.Rows)
        //                {
        //                   //TreeListNode nc = treeList1.AppendNode(new object[] { r["权限类型"].ToString() }, n);
        //                   TreeListNode nc = tv.AppendNode(new object[] { r["子项编码"].ToString() }, n);
        //                   // nc.SetValue("产品编码结构", r["子项编码"].ToString());
        //                    nc.SetValue("子项类型", r["子项类型"].ToString());
        //                      nc.SetValue("物料编码", r["子项编码"].ToString());
        //                    nc.SetValue("产品名称", r["子项名称"].ToString());
        //                    nc.SetValue("规格型号", r["子项规格"].ToString());
        //                    nc.SetValue("图纸编号", r["子项图号"].ToString());
        //                    nc.SetValue("BOM类型", r["BOM类型"].ToString());
        //                    nc.SetValue("数量", Convert.ToDecimal(r["数量"]));
        //                    nc.SetValue("仓库号", r["仓库号"]);
        //                    nc.SetValue("仓库名称",r["仓库名称"]);

        //                    nc.SetValue("子件损耗率", Convert.ToDecimal(r["子件损耗率"]));
        //                    nc.SetValue("WIPType", r["WIPType"].ToString());
        //                    nc.Tag = r;
        //                    fun_TL(nc, r["子项编码"].ToString());
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception(ex.Message);
        //            }
        //        }

        private void fun_TL(TreeListNode n, string str_fx, int cj)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                DataRow[] rr = dt_Bom.Select(string.Format("产品编码='{0}' and bom_level={1} ", str_fx, cj + 1));

                foreach (DataRow r in rr)
                {
                    //TreeListNode nc = treeList1.AppendNode(new object[] { r["权限类型"].ToString() }, n);
                    TreeListNode nc = tv.AppendNode(new object[] { r["子项编码"].ToString() }, n);
                    // nc.SetValue("产品编码结构", r["子项编码"].ToString());
                    nc.SetValue("子项类型", r["子项类型"].ToString());
                    nc.SetValue("物料编码", r["子项编码"].ToString());
                    nc.SetValue("产品名称", r["子项名称"].ToString());
                    nc.SetValue("规格型号", r["子项规格"].ToString());
                    nc.SetValue("BOM类型", r["BOM类型"].ToString());
                    nc.SetValue("数量", Convert.ToDecimal(r["数量"]));
                    nc.SetValue("WIPType", r["WIPType"].ToString());
                    nc.SetValue("层级", r["bom_level"].ToString());
                    nc.SetValue("仓库号", r["仓库号"].ToString());
                    nc.SetValue("仓库名称", r["仓库名称"].ToString());

                    nc.SetValue("供应状态", r["供应状态"].ToString());
                    nc.SetValue("A面位号", r["A面位号"].ToString());


                    //  nc.SetValue("停用", r["停用"].ToString());
                    nc.Tag = r;
                    fun_TL(nc, r["子项编码"].ToString(), cj + 1);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void tv_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                TreeListNode n = tv.Selection[0];

                if (e.Control & e.KeyCode == Keys.C)
                {
                    Clipboard.SetDataObject(n.GetValue(tv.FocusedColumn));
                    e.Handled = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try

            {


                //DataTable dt_表头 = new DataTable();
                //dt_表头.Columns.Add("领料出库单号", typeof(string));
                //dt_表头.Columns.Add("编号", typeof(string));
                //dt_表头.Columns.Add("物料号", typeof(string));
                //dt_表头.Columns.Add("规格", typeof(string));
                //dt_表头.Columns.Add("物料名称", typeof(string));
                //dt_表头.Columns.Add("生产数量", typeof(decimal));
                //dt_表头.Columns.Add("领用部门", typeof(string));
                //dt_表头.Columns.Add("领用人", typeof(string));
                //dt_表头.Columns.Add("申请人", typeof(string));
                //dt_表头.Columns.Add("仓管员", typeof(string));
                //dt_表头.Columns.Add("日期", typeof(DateTime));

                //DataRow dr = dt_表头.NewRow();
                //dr["编号"] = rr["生产工单号"];
                //dr["物料号"] = rr["产品编码"];
                //dr["规格"] = rr["规格型号"];

                //dr["物料名称"] = rr["产品名称"];

                //dr["生产数量"] = rr["生产数量"];
                //// dr["领用部门"] = "dsa13123";
                //dr["领用人"] = rr["领料人ID"].ToString() + "  " + rr["领料人"].ToString();
                //dr["申请人"] = rr["制单人员"].ToString();

                ////dr["仓管员"] = "dsa13123";
                //dr["日期"] = DateTime.Now.ToString();
                //dr["领料出库单号"] = rr["待领料单号"].ToString();
                //dt_表头.Rows.Add(dr);








                //DataRow drM = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
                //DataTable dtm = (DataTable)this.gcP.DataSource;
                //Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                //Type outerForm = outerAsm.GetType("ERPreport.Form其他出入库申请", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                //CPublic.UIcontrol.Showpage(ui, t.Rows[0]["打开界面名称"].ToString());
                //object[] drr = new object[2];

                //drr[0] = drM;
                //drr[1] = dtm;
                ////   drr[2] = dr["出入库申请单号"].ToString();
                //Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                ////  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
                //ui.ShowDialog();







            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cb_物料_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.C)
                {
                    if (cb_物料.EditValue != null && cb_物料.EditValue.ToString() != "")
                    {
                        Clipboard.SetDataObject(cb_物料.EditValue);
                        e.Handled = true;
                    }
                }
            }
            catch
            {

            }

        }

        private void tv_CustomDrawNodeIndicator(object sender, DevExpress.XtraTreeList.CustomDrawNodeIndicatorEventArgs e)
        {
            DevExpress.XtraTreeList.TreeList tmpTree = sender as DevExpress.XtraTreeList.TreeList;
            DevExpress.Utils.Drawing.IndicatorObjectInfoArgs args = e.ObjectArgs as DevExpress.Utils.Drawing.IndicatorObjectInfoArgs;
            if (args != null)
            {
                int rowNum = tmpTree.GetVisibleIndexByNode(e.Node) + 1;
                // this.tv.IndicatorWidth = rowNum.ToString().Length * 10 + 12;
                args.DisplayText = rowNum.ToString();
            }

        }

        private void repositoryItemGridLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            try
            {
                DataRow r = tv.Selection[0].Tag as DataRow;
                DataRow[] rr = dt_仓库.Select(string.Format("仓库号='{0}'", e.NewValue));
                if (rr.Length > 0)
                {
                    r["仓库名称"] = rr[0]["仓库名称"];
                    r["仓库号"] = e.NewValue;

                    tv.Selection[0].SetValue("仓库名称", rr[0]["仓库名称"].ToString());
                    tv.Selection[0].SetValue("仓库号", e.NewValue);


                }

            }
            catch
            {


            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认保存？", "确认!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DataView vv = new DataView(dt_Bom);
                    vv.RowStateFilter = DataViewRowState.ModifiedCurrent;
                    DataTable dt_save = vv.ToTable();
                    string s_bom = "select  * from 基础数据物料BOM表 where 1=2 ";
                    DataTable save_bom = CZMaster.MasterSQL.Get_DataTable(s_bom, strconn);
                    foreach (DataRow dr in dt_save.Rows)
                    {

                        string s = string.Format("select  * from 基础数据物料BOM表 where 产品编码='{0}' and 子项编码='{1}'", dr["产品编码"], dr["子项编码"]);
                        using (SqlDataAdapter da = new SqlDataAdapter(s, strconn))
                        {
                            da.Fill(save_bom);
                            DataRow[] tr = save_bom.Select(string.Format("产品编码='{0}' and 子项编码='{1}'", dr["产品编码"], dr["子项编码"]));
                            tr[0]["仓库号"] = dr["仓库号"];
                            tr[0]["仓库名称"] = dr["仓库名称"];
                            tr[0]["WIPType"] = dr["WIPType"];
                        }


                    }
                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("SH"); //事务的名称
                    SqlCommand cmd1 = new SqlCommand(s_bom, conn, ts);
                    try
                    {


                        SqlDataAdapter da;
                        da = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da);
                        da.Update(save_bom);


                        ts.Commit();
                        MessageBox.Show("保存成功");
                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();
                        throw new Exception("保存失败" + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (e.Page.Name == "xtraTabPage1")
            {
                treeListColumn10.OptionsColumn.AllowEdit = false;
                barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                if (CPublic.Var.localUser部门编号 == "00010603"||CPublic.Var.LocalUserTeam =="管理员权限")
                {
                    simpleButton1.Visible = true;
                    treeListColumn10.OptionsColumn.AllowEdit = true;
                }
                else
                {
                    simpleButton1.Visible = false;
                    treeListColumn10.OptionsColumn.AllowEdit = false;
                }
                //if (CPublic.Var.localUser部门编号 == "00010601")
                //{
                //    barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                //}
            }
            else
            {
                simpleButton1.Visible = false;
                treeListColumn10.OptionsColumn.AllowEdit = false;
            }
        }

        private void treeList1_CustomDrawNodeIndicator(object sender, DevExpress.XtraTreeList.CustomDrawNodeIndicatorEventArgs e)
        {
            DevExpress.XtraTreeList.TreeList tmpTree = sender as DevExpress.XtraTreeList.TreeList;
            DevExpress.Utils.Drawing.IndicatorObjectInfoArgs args = e.ObjectArgs as DevExpress.Utils.Drawing.IndicatorObjectInfoArgs;
            if (args != null)
            {
                int rowNum = tmpTree.GetVisibleIndexByNode(e.Node) + 1;
                // this.tv.IndicatorWidth = rowNum.ToString().Length * 10 + 12;
                args.DisplayText = rowNum.ToString();
            }
        }



        private void treeList1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                TreeListNode n = treeList1.Selection[0];

                if (e.Control & e.KeyCode == Keys.C)
                {
                    Clipboard.SetDataObject(n.GetValue(treeList1.FocusedColumn));
                    e.Handled = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataView vv = new DataView(dt_Bom);
                vv.RowStateFilter = DataViewRowState.ModifiedCurrent;
                DataTable dt_save = vv.ToTable();
                if (vv.Count == 0) throw new Exception("尚未未修改任何记录");

                DataSet ds = fun_bom类型修改(dt_save);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
  
        }


        private DataSet fun_bom类型修改(DataTable dt)
        {
            DataSet ds = new DataSet();
            foreach(DataRow dr in dt.Rows)
            {


            }
            return ds;
        }

        private void repositoryItemComboBox1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            try
            {
                DataRow r = tv.Selection[0].Tag as DataRow;
                r["WIPType"] = e.NewValue;
            }
            catch (Exception ex)
            {
               
                 
            }
            
        }
    }


}
