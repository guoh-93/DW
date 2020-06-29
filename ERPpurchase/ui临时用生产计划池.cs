using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Globalization;
using System.Data.SqlClient;

namespace ERPpurchase
{
    public partial class ui临时用生产计划池 : UserControl
    {

        string strcon = CPublic.Var.strConn;

        DataTable dtM;
        DataTable dt_SaleOrder = new DataTable();
        DataTable dt_SaleCrderCopy;
        DataTable dt_totalcount;
        DataTable IncompleteWorkOrder = new DataTable();
        DataTable IncompleteWorkOrdercopy;

        DataTable dt_库存;
        DataTable dt_bom = new DataTable();

        /// <summary>
        /// flag 指示用户进度 ,导入销售明细-1,导入未完成工单-2,同步BOM及库存-3 
        /// </summary>
        int flag = 0;
        bool bl_sync = false;
        bool bl_calculate = false;

        string str_log = "";
        string strcon_U8 = CPublic.Var.geConn("DW");

        public ui临时用生产计划池()
        {
            InitializeComponent();
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            try
            {
                if (flag < 3) throw new Exception("信息尚未准备完全,请按步骤操作");
                if (bl_calculate) throw new Exception("正在计算中..");

                Thread th = new Thread(Calculate);
                th.IsBackground = true;
                th.Start();
                bl_calculate = true;
                simpleButton4.Text = "正在计算中..";

                #region 孙杰 2018-12-7

                string sql2 = string.Format("select * from 用户登录注册表 where 工号='{0}' and 计算机名='{1}'", CPublic.Var.LocalUserID, Environment.MachineName.ToString() + "-1");
                DataTable dtsdas = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);
                if (dtsdas.Rows.Count > 0)
                {
                    DataRow dr = dtsdas.Rows[0];
                    dr["修改时间"] = CPublic.Var.getDatetime();
                    dr["更新日志版本号"] = int.Parse(dr["更新日志版本号"].ToString()) + 1;
                    using (SqlDataAdapter da = new SqlDataAdapter("select *  from  用户登录注册表 where 1<>1", strcon))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dtsdas);
                    }
                }
                else
                {
                    string sql = "select *  from  用户登录注册表 where 1<>1";
                    DataTable dt_cse = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    DataRow dr = dt_cse.NewRow();
                    dt_cse.Rows.Add(dr);
                    dr["工号"] = CPublic.Var.LocalUserID;
                    dr["姓名"] = CPublic.Var.localUserName;
                    dr["计算机名"] = Environment.MachineName.ToString() + "-1";
                    //string s =System.Net.Dns.GetHostName() ;
                    dr["修改时间"] = CPublic.Var.getDatetime();
                    dr["更新日志版本号"] = "1";
                    dr["备注"] = "1";

                    using (SqlDataAdapter da = new SqlDataAdapter("select *  from  用户登录注册表 where 1<>1", strcon))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_cse);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        //导入销售订单
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (flag == 0)
                {
                    var ofd = new OpenFileDialog();
                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {


                        bool bl = ERPorg.Corg.IsFileInUse(ofd.FileName);
                        if (bl) throw new Exception("文件已打开或被占用中");
                        //  dt_SaleOrder = ERPorg.Corg.ExcelXLSX(ofd);
                        Thread th = new Thread(() =>
                        {
                            BeginInvoke(new MethodInvoker(() =>
                            {
                                simpleButton1.Enabled = false;
                                simpleButton1.Text = "导入中..";
                            }));
                            dt_SaleOrder = ERPorg.Corg.ReadExcelToDataTable(ofd.FileName);

                            //DateTime t = CPublic.Var.getDatetime().Date;
                            //string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DW导入销售单\\计划";
                            //if (Directory.Exists(fileName) == false)
                            //{
                            //    Directory.CreateDirectory(fileName);
                            //}

                            int x = dt_SaleOrder.Rows.Count;
                            for (int i = x - 1; i >= 0; i--)
                            {
                                if (dt_SaleOrder.Rows[i]["物料编码"].ToString().Trim() == "")
                                {
                                    dt_SaleOrder.Rows.Remove(dt_SaleOrder.Rows[i]);
                                }
                            }
                            flag = 1;
                            BeginInvoke(new MethodInvoker(() =>
                            {
                                simpleButton1.Text = string.Format("销售明细:{0}条", dt_SaleOrder.Rows.Count);
                            }));
                        });
                        th.Start();
                    }
                }
                else
                {
                    throw new Exception("请按步骤操作");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //未完成工单
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (flag == 1)
                {

                    Thread th = new Thread(() =>
   {
       BeginInvoke(new MethodInvoker(() =>
       {
           simpleButton2.Enabled = false;
           simpleButton2.Text = "拉取数据中..";
       }));

       //未完成工单
       string s = @" select  mocode 生产订单号,invcode 物料编码,qty-QualifiedInQty 未完成数量,base.规格型号,存货分类,未领量,在途量,
 case when mocode in ( select  MoCode  from [192.168.20.150].UFDATA_008_2018.dbo.mom_moallocate 
 left join [192.168.20.150].UFDATA_008_2018.dbo.mom_orderdetail on mom_orderdetail.MoDId=mom_moallocate.MoDId
 left join [192.168.20.150].UFDATA_008_2018.dbo.mom_order  on  mom_order.MoId=mom_orderdetail.MoId
  where  mom_orderdetail.Status =3  and mom_moallocate.Qty-IssQty>0 group by MoCode) then 1 else 0 end as 标识
   from [192.168.20.150].UFDATA_008_2018.dbo.mom_orderdetail 
 left join [192.168.20.150].UFDATA_008_2018.dbo.mom_order  on mom_order.Moid=mom_orderdetail.moid
 left join 基础数据物料信息表  base on base.物料编码=invcode 
 left join  (select  物料编码,sum(库存总数)库存总数,max(未领量)未领量,max(在途量) as 在途量  from 仓库物料数量表
      where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
  on  base.物料编码=kc.物料编码
  where  mom_orderdetail.Status=3 and qty-QualifiedInQty>0 ";
       IncompleteWorkOrder = CZMaster.MasterSQL.Get_DataTable(s, strcon);

       flag = 2;
       BeginInvoke(new MethodInvoker(() =>
       {
           simpleButton2.Text = string.Format("未完成工单:{0}条 ", IncompleteWorkOrder.Rows.Count);
       }));
   });
                    th.Start();



                }
                else
                {
                    MessageBox.Show("请按步骤操作");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        //同步相关bom,库存
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            if (flag == 2)
            {
                simpleButton3.Text = "正在同步中..稍候";

                Thread th = new Thread(Fun_sync);
                th.IsBackground = true;
                th.Start();
                bl_sync = true;
            }
            else
            {
                MessageBox.Show("请按步骤操作");
            }
        }

        /// <summary>
        /// 同步Bom和库存 并 加载过来
        /// </summary>
        private void Fun_sync()
        {
            // string s = "";
            string s = "exec sync_u8_data  "; //基础数据
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            s = "exec sync_u8_stock"; //库存
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            s = "exec sync_u8_bom "; //bom
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            s = "exec  sync_UnclaimedCount"; //未领量
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);
            s = "exec  sync_u8_OnMake"; //在制量
            CZMaster.MasterSQL.ExecuteSQL(s, strcon);

            s = @"  select  产品编码,产品名称,fx.存货分类 as 父项分类,fx.规格型号 as 父项规格,子项编码,子项名称,数量,zx.自制 as 子项自制 ,zx.存货分类 as 子项分类
    ,zx.规格型号 as 子项规格       from 基础数据物料BOM表 bom 
   left join 基础数据物料信息表 zx on bom.子项编码=zx.物料编码 
   left join 基础数据物料信息表 fx on bom.产品编码=fx.物料编码 ";
            dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = @"select kc.物料编码,base.物料名称,base.规格型号,存货分类,库存总数,未领量,在制量,自制  from  
                        (select  物料编码,sum(库存总数)库存总数,sum(未领量)未领量,max(在制量)在制量  from 仓库物料数量表
                             where 仓库号 in (select  属性字段1 from 基础数据基础属性表 where 属性类别 ='仓库类别' and 布尔字段2=1) group by 物料编码)  kc 
                  left join 基础数据物料信息表 base   on  base.物料编码=kc.物料编码 ";
            dt_库存 = CZMaster.MasterSQL.Get_DataTable(s, strcon); //这里未领 和 在制 分别用 sum 和max 跟同步U8的未领 在制有关
            flag = 3;
            bl_sync = false;
            BeginInvoke(new MethodInvoker(() =>
            {
                simpleButton3.Text = "已同步,并加载完成";
                simpleButton3.Enabled = false;
            }));
        }

        private void Calcu_check()
        {

            if (!dt_SaleOrder.Columns.Contains("物料编码")) throw new Exception("销售订单中不含'物料编码'列");
            int x = dt_SaleOrder.Rows.Count;
            for (int i = x - 1; i >= 0; i--)
            {
                if (dt_SaleOrder.Rows[i]["物料编码"].ToString().Trim() == "")
                {
                    dt_SaleOrder.Rows.Remove(dt_SaleOrder.Rows[i]);
                }
            }
            x = IncompleteWorkOrder.Rows.Count;
            for (int i = x - 1; i >= 0; i--)
            {
                if (IncompleteWorkOrder.Rows[i]["物料编码"].ToString().Trim() == "")
                {
                    IncompleteWorkOrder.Rows.Remove(IncompleteWorkOrder.Rows[i]);
                }
            }
            //if (!IncompleteWorkOrder.Columns.Contains("生产订单号")) throw new Exception("生产订单中不含'生产订单号'列");
            if (!IncompleteWorkOrder.Columns.Contains("未完成数量")) throw new Exception("生产订单中不含'未完成数量'列");

            decimal dec = 0;
            int j = 2;
            foreach (DataRow dr in dt_SaleOrder.Rows)
            {
                string s = dr["物料编码"].ToString().Trim();
                if (s.Contains("S")) //contain 不区分大小写,先判断有没有 'smt'  有就去掉
                {
                    int d = s.IndexOf("S");
                    if (d == -1) d = s.IndexOf("s");
                    s = s.Substring(0, d + 1);

                }
                if (s.Length < 14) throw new Exception(string.Format("销售订单中第{0}行物料编码位数不对", j));
                if (!decimal.TryParse(dr["数量"].ToString(), out dec)) throw new Exception(string.Format("销售订单中第{0}行数量格式不正确", j));
                j++;

            }
            j = 2;
            foreach (DataRow dr in IncompleteWorkOrder.Rows)
            {
                string s = dr["物料编码"].ToString().Trim();
                if (s.Contains("S") || s.Contains("s")) //contain 不区分大小写,先判断有没有 'smt'  有就去掉
                {
                    int d = s.IndexOf("S");
                    if (d == -1) d = s.IndexOf("s");
                    s = s.Substring(0, d);
                }
                if (s.Length < 14) throw new Exception(string.Format("生产订单中第{0}行物料编码位数不对", j));
                if (!decimal.TryParse(dr["未完成数量"].ToString(), out dec)) throw new Exception(string.Format("生产订单中第{0}行数量格式不正确", j));
                j++;
            }
        }

        private void Calculate()
        {
            try
            {
                Calcu_check();
                dtM = new DataTable();
                dtM.Columns.Add("未完成工单数", typeof(decimal));
                dtM.Columns.Add("物料编码");
                dtM.Columns.Add("物料名称");
                dtM.Columns.Add("规格型号");
                dtM.Columns.Add("库存总数", typeof(decimal));
                dtM.Columns.Add("存货分类");
                dtM.Columns.Add("参考数量", typeof(decimal));
                dtM.Columns.Add("销售数量", typeof(decimal));
                dtM.Columns.Add("自制", typeof(bool));


                dt_SaleCrderCopy = dt_SaleOrder.Copy();
                IncompleteWorkOrdercopy = IncompleteWorkOrder.Copy();

                dt_SaleCrderCopy.Columns.Add("库存总数", typeof(decimal));
                dt_SaleCrderCopy.Columns.Add("未完成工单数", typeof(decimal));
                dt_SaleCrderCopy.Columns.Add("未领量", typeof(decimal));
                //IncompleteWorkOrdercopy.Columns.Add("未领量", typeof(decimal));
                //dt_SaleCrderCopy 加入库存数量和 未完成工单数  用于核对,gridcontrol1显示

                foreach (DataRow dr in dt_SaleCrderCopy.Rows)
                {
                    if (dr["物料编码"].ToString().Trim() != "")
                    {
                        // bool bl= dt_SaleCrderCopy.Columns.Contains("物料编码");
                        DataRow[] v = dt_库存.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        dr["库存总数"] = v[0]["库存总数"];
                        dr["未领量"] = v[0]["未领量"];
                        //DataRow[] v1 = IncompleteWorkOrder.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        //if (v1.Length > 0) dr["未完成工单数"] = v1[0]["未完成数量"];
                        dr["未完成工单数"]= v[0]["在制量"];

                    }
                }
                foreach (DataRow dr in IncompleteWorkOrdercopy.Rows)
                {
                    DataRow[] v = dt_库存.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    dr["未领量"] = v[0]["未领量"];
                }


                //销售订单和 未完成订单汇总 成两列 物料编码 和 数量 即按物料汇总 销售订单和 未完成工单 
                MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
                DataTable dt_SaleOrder_1 = RBQ.SelectGroupByInto("", dt_SaleOrder, "物料编码,sum(数量) 数量", "", "物料编码");

                DataTable IncompleteWorkOrder_1 = RBQ.SelectGroupByInto("", IncompleteWorkOrder, "物料编码,sum(未完成数量) 数量", "", "物料编码");

                //dt_totalcount 复制 dt_库存,库存总数+未完成订单数
                dt_totalcount = dt_库存.Copy();
                dt_totalcount.Columns.Add("总数", typeof(decimal));
                dt_totalcount.Columns.Add("未完成工单数", typeof(decimal));
                dt_totalcount.Columns.Add("销售数量", typeof(decimal));

                foreach (DataRow xr in dt_totalcount.Rows)
                {
                    decimal dec = 0;
                    //   DataRow[] r = IncompleteWorkOrder_1.Select(string.Format("物料编码='{0}'", xr["物料编码"]));


                    //if (r.Length > 0)
                    //{
                    //    dec = Convert.ToDecimal(r[0]["数量"]);
                    //}
                    dec = Convert.ToDecimal(xr["在制量"]);
                    xr["未完成工单数"] = dec;
                    // xr["总数"] = Convert.ToDecimal(xr["库存总数"]) + dec;
                    xr["总数"] = Convert.ToDecimal(xr["库存总数"]) + dec - Convert.ToDecimal(xr["未领量"]);
                    DataRow[] rs = dt_SaleOrder_1.Select(string.Format("物料编码='{0}'", xr["物料编码"]));
                    if (rs.Length > 0)
                    {
                        dec = Convert.ToDecimal(rs[0]["数量"]);
                    }
                    else
                    {

                        dec = 0;
                    }
                    xr["销售数量"] = dec;
                }


                //先计算销售列表中的产品的欠缺数量
                foreach (DataRow dr in dt_SaleOrder_1.Rows)
                {

                    decimal dec_订单数 = Convert.ToDecimal(dr["数量"]);

                    DataRow[] r_total = dt_totalcount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    decimal total = Convert.ToDecimal(r_total[0]["总数"]);
                    decimal kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                    decimal wwcgds = Convert.ToDecimal(r_total[0]["未完成工单数"]);

                    if (total >= dec_订单数) //库存加未完成>需求数
                    {
                        r_total[0]["总数"] = total - dec_订单数;

                    }
                    else
                    {
                        DataRow r_need = dtM.NewRow();
                        r_need["未完成工单数"] = wwcgds;
                        r_need["物料编码"] = r_total[0]["物料编码"];
                        r_need["物料名称"] = r_total[0]["物料名称"];
                        r_need["规格型号"] = r_total[0]["规格型号"];
                        r_need["存货分类"] = r_total[0]["存货分类"];
                        r_need["库存总数"] = kczs;
                        r_need["销售数量"] = r_total[0]["销售数量"];
                        r_need["自制"] = r_total[0]["自制"];
                        r_need["参考数量"] = dec_订单数 - total;
                        dtM.Rows.Add(r_need);
                        r_total[0]["总数"] = 0;
                    }
                }

                //到此处 计算了 所有在销售订单中出现的料 所缺的数量  参考数量为所缺量
                //下面计算 这些 物料的 子项 有自制属性的  所缺的量 
                DataTable dtMcopy = dtM.Copy();
                //fun_dg(dtMcopy);

                foreach (DataRow dr in dtMcopy.Rows)
                {
                    if (dr["自制"].Equals(true))
                    {
                        if (dt_bom.Select(string.Format("产品编码='{0}'", dr["物料编码"])).Length == 0) str_log = str_log + dr["物料编码"].ToString() + "属性为自制但是没有bom; ";
                    }


                    DataRow[] br = dt_bom.Select(string.Format("产品编码='{0}'and 子项自制=1", dr["物料编码"].ToString()));
                    if (br.Length > 0) //找到需要自制的半成品 
                    {
                        decimal dec_缺 = Convert.ToDecimal(dr["参考数量"].ToString());
                        foreach (DataRow brr in br)
                        {

                            decimal dec = dec_缺 * Convert.ToDecimal(brr["数量"]); //这是自制半成品所需的量 
                            DataRow[] stock_total = dt_totalcount.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                            decimal total_z = Convert.ToDecimal(stock_total[0]["总数"]);
                            if (total_z >= dec) //库存加未完成>需求数
                            {
                                stock_total[0]["总数"] = total_z - dec;
                            }
                            else
                            {
                                DataRow[] fr = dtM.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                                if (fr.Length > 0)
                                {
                                    fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec - total_z;
                                }
                                else
                                {
                                    DataRow r_need = dtM.NewRow();
                                    r_need["未完成工单数"] = stock_total[0]["未完成工单数"];
                                    r_need["物料编码"] = stock_total[0]["物料编码"];
                                    r_need["物料名称"] = stock_total[0]["物料名称"];
                                    r_need["规格型号"] = stock_total[0]["规格型号"];
                                    r_need["存货分类"] = stock_total[0]["存货分类"];
                                    r_need["库存总数"] = stock_total[0]["库存总数"];
                                    r_need["销售数量"] = stock_total[0]["销售数量"];
                                    r_need["自制"] = stock_total[0]["自制"];

                                    r_need["参考数量"] = dec - total_z;
                                    dtM.Rows.Add(r_need);
                                    stock_total[0]["总数"] = 0;
                                }

                                //缺的才需要继续往叶子节点递归 不缺不需要
                                bool bl_t = false;
                                if (dr["自制"] == null || dr["自制"].ToString().Trim() == "")
                                {
                                    bl_t = false;
                                }
                                else
                                {
                                    bl_t = Convert.ToBoolean(dr["自制"]);
                                }

                                Fun_dg(stock_total[0]["物料编码"].ToString(), dec - total_z, bl_t);

                            }
                        }
                    }
                }



                //不缺的 也要求显示 参考数量为0 

                foreach (DataRow dr in dt_SaleOrder_1.Rows)
                {
                    DataRow[] xxx = dtM.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (xxx.Length == 0)
                    {
                        DataRow[] r_total = dt_totalcount.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        decimal total = Convert.ToDecimal(r_total[0]["总数"]);
                        decimal kczs = Convert.ToDecimal(r_total[0]["库存总数"]);
                        decimal wwcgds = Convert.ToDecimal(r_total[0]["未完成工单数"]);
                        DataRow r_need = dtM.NewRow();
                        r_need["未完成工单数"] = wwcgds;
                        r_need["物料编码"] = r_total[0]["物料编码"];
                        r_need["物料名称"] = r_total[0]["物料名称"];
                        r_need["规格型号"] = r_total[0]["规格型号"];
                        r_need["存货分类"] = r_total[0]["存货分类"];
                        r_need["库存总数"] = kczs;
                        r_need["销售数量"] = r_total[0]["销售数量"];
                        r_need["自制"] = r_total[0]["自制"];
                        r_need["参考数量"] = 0;
                        dtM.Rows.Add(r_need);


                    }
                }


                if (str_log != "")
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        label4.Text = str_log;

                    }));
                }

                DataView search_source = new DataView(dtM);
                search_source.RowFilter = "销售数量>0";
                BeginInvoke(new MethodInvoker(() =>
               {
                   simpleButton4.Text = "计算完成";
               }));


                bl_calculate = false; //计算完成
                Method(gc2, gd =>
                {
                    DataView dv = new DataView(dtM);
                    dv.RowFilter = "自制='true'";
                    gd.DataSource = dv;

                    searchLookUpEdit1.Properties.DataSource = search_source;
                    searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                    searchLookUpEdit1.Properties.ValueMember = "物料编码";
                });

                #region 直接这么做是行的
                //BeginInvoke(new MethodInvoker(() =>
                //{
                //    DataView dv = new DataView(dtM);
                //    dv.RowFilter = "自制='true'";
                //    gc2.DataSource = dv;
                //    searchLookUpEdit1.Properties.DataSource = search_source;
                //    searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                //    searchLookUpEdit1.Properties.ValueMember = "物料编码";
                //}));
                #endregion
            }
            catch (Exception ex)
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    label4.Text = "错误原因:" + ex.Message;
                    simpleButton4.Text = "计算错误";
                }));
                bl_calculate = false;

            }
        }
        // public delegate void NoReturnWithPara(DevExpress.XtraGrid.GridControl gc);
        private void Method<T>(T c, Action<T> action) where T : DevExpress.XtraGrid.GridControl
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => action(c)));
            }
            else
                action(c);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemid">物料编码</param>
        /// <param name="dec_需求"></param>
        /// <param name="bl_made">是否自制</param>
        private void Fun_dg(string itemid, decimal dec_需求, bool bl_made)
        {
            if (bl_made)
            {
                if (dt_bom.Select(string.Format("产品编码='{0}'", itemid)).Length == 0) str_log = str_log + itemid + "属性为自制但是没有bom; ";
            }


            DataRow[] br = dt_bom.Select(string.Format("产品编码='{0}'and 子项自制=1", itemid));
            if (br.Length > 0) //找到需要自制的半成品 
            {
                decimal dec_缺 = dec_需求;

                foreach (DataRow brr in br)
                {
                    decimal dec = dec_缺 * Convert.ToDecimal(brr["数量"]); //这是自制半成品所需的量 
                    DataRow[] stock_total = dt_totalcount.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                    decimal total_z = Convert.ToDecimal(stock_total[0]["总数"]);
                    if (total_z >= dec) //库存加未完成>需求数
                    {
                        stock_total[0]["总数"] = total_z - dec;
                    }
                    else
                    {
                        DataRow[] fr = dtM.Select(string.Format("物料编码='{0}'", brr["子项编码"]));
                        if (fr.Length > 0)
                        {
                            fr[0]["参考数量"] = Convert.ToDecimal(fr[0]["参考数量"]) + dec - total_z;
                        }
                        else
                        {
                            DataRow r_need = dtM.NewRow();
                            r_need["未完成工单数"] = stock_total[0]["未完成工单数"];
                            r_need["物料编码"] = stock_total[0]["物料编码"];
                            r_need["物料名称"] = stock_total[0]["物料名称"];
                            r_need["规格型号"] = stock_total[0]["规格型号"];
                            r_need["存货分类"] = stock_total[0]["存货分类"];
                            r_need["库存总数"] = stock_total[0]["库存总数"];
                            r_need["销售数量"] = stock_total[0]["销售数量"];
                            r_need["自制"] = stock_total[0]["自制"];
                            r_need["参考数量"] = dec - total_z;
                            dtM.Rows.Add(r_need);
                            stock_total[0]["总数"] = 0;
                        }
                        Fun_dg(stock_total[0]["物料编码"].ToString(), dec - total_z, Convert.ToBoolean(stock_total[0]["自制"]));
                    }
                }



            }





        }
        #region
        ////合并 导入的 销售和未完成的 成品 半成品 
        //private void combine()
        //{
        //    dt_parent = new DataTable();
        //    dt_parent.Columns.Add("物料编码");

        //    foreach (DataRow dr in dt_SaleOrder.Rows)
        //    {
        //        DataRow r = dt_parent.NewRow();
        //        r["物料编码"] = dr["物料编码"];
        //        dt_parent.Rows.Add(r);
        //    }
        //    foreach (DataRow dr in IncompleteWorkOrder.Rows)
        //    {
        //        DataRow[] rr = dt_parent.Select(string.Format("物料编码='{0}'", dr["物料编码"].ToString()));
        //        if (rr.Length == 0)
        //        {
        //            DataRow r = dt_parent.NewRow();
        //            r["物料编码"] = dr["物料编码"];
        //            dt_parent.Rows.Add(r);
        //        }
        //    }
        //    foreach (DataRow dr in dt_parent.Rows)
        //    {
        //        DataTable temp = ERPorg.Corg.get_u8bom(dr["物料编码"].ToString());
        //        if (dt_bom == null || dt_bom.Columns.Count == 0)
        //        {
        //            dt_bom = new DataTable();
        //            dt_bom = temp.Copy();
        //        }
        //        else
        //        {
        //            //这边 取过来的BOM清单 可能 有重复 因为u8中维护的bom可能会有重复的 所以这边不用merge
        //            foreach (DataRow rr in temp.Rows)
        //            {
        //                DataRow[] xx = dt_bom.Select(string.Format("父项编码='{0}' and 子项编码='{1}'", rr["父项编码"], rr["子项编码"]));
        //                if (xx.Length == 0)
        //                {
        //                    DataRow x = dt_bom.NewRow();
        //                    x["父项编码"] = rr["父项编码"];
        //                    x["父项名称"] = rr["父项名称"];
        //                    x["父项规格"] = rr["父项规格"];
        //                    x["子项编码"] = rr["子项编码"];
        //                    x["子项名称"] = rr["子项名称"];
        //                    x["子项规格"] = rr["子项规格"];
        //                    x["计量单位编码"] = rr["计量单位编码"];
        //                    x["计量单位"] = rr["计量单位"];
        //                    x["仓库号"] = rr["仓库号"];
        //                    x["仓库名称"] = rr["仓库名称"];
        //                    x["数量"] = rr["数量"];
        //                    dt_bom.Rows.Add(x);
        //                }
        //            }
        //        }
        //    }
        //    string s = "select  * from 基础数据物料BOM表 where 1=2";
        //    DataTable dt_save_bom = CZMaster.MasterSQL.Get_DataTable(s,strcon);

        //    //往BOM表和库存表存数据
        //    foreach (DataRow dr in dt_bom.Rows)
        //    {
        //        //bom表添加记录
        //        DataRow x_b = dt_save_bom.NewRow();
        //        x_b["产品编码"] = dr["父项编码"];
        //        x_b["子项编码"] = dr["子项编码"];
        //        x_b["产品名称"] = dr["父项名称"];
        //        x_b["子项名称"] = dr["子项名称"];
        //        x_b["总装数量"]= x_b["数量"] = dr["数量"];
        //        x_b["主辅料"] = "主料";
        //        x_b["计量单位编码"] = dr["计量单位编码"];
        //        x_b["计量单位"] = dr["计量单位"];
        //        x_b["仓库号"] = dr["仓库号"];
        //        x_b["仓库名称"] = dr["仓库名称"];
        //        dt_save_bom.Rows.Add(x_b);
        //    }
        //    //同步库存呢   估计还是要用存储过程 JOB不太好





        //}
        #endregion

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            if (!bl_sync)
            {
                flag = 0;
                bl_calculate = false;
                label4.Text = "";
                simpleButton1.Enabled = true;
                simpleButton1.Text = "导入销售订单";

                simpleButton2.Enabled = true;
                simpleButton2.Text = "导入未完成工单";

                simpleButton3.Enabled = true;
                simpleButton3.Text = "同步相关BOM,库存";
                simpleButton4.Text = "开始计算";
            }
        }

        private void BarLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (bl_sync) throw new Exception("正在数据同步..");
                if (bl_calculate) throw new Exception("正在计算中..");
                CPublic.UIcontrol.ClosePage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Gv2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void Gv2_KeyDown(object sender, KeyEventArgs e)
        {
            if (gv2.FocusedColumn.Caption == "物料编码" || gv2.FocusedColumn.Caption == "规格型号")
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    Clipboard.SetDataObject(gv2.GetFocusedRowCellValue(gv2.FocusedColumn));
                    e.Handled = true;
                }
            }
        }
        private void Gv2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

            DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
            if (dr == null) return;
            Thread th = new Thread(() =>
            {
                DataTable dtz = new DataTable();
                dtz.Columns.Add("产品编码");
                dtz = ERPorg.Corg.fun_GetFather(dtz, dr["物料编码"].ToString(), 0, true);
                string s = "";
                if (dtz.Rows.Count > 0)
                {
                    s = "物料编码 in (";
                    foreach (DataRow xx in dtz.Rows)
                    {
                        s = s + "'" + xx["产品编码"].ToString() + "',";
                    }
                    s = s.Substring(0, s.Length - 1) + ")";
                    DataView dv = new DataView(dt_SaleCrderCopy);
                    dv.RowFilter = s;
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        gridControl1.DataSource = dv;
                    }));
                }
                else
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        gridControl1.DataSource = dt_SaleCrderCopy.Clone();
                    }));
                }
                DataTable dt_x = new DataTable();
                dt_x = ERPorg.Corg.billofM(dt_x, dr["物料编码"].ToString(), false,dt_bom);
                s = "";
                if (dt_x.Rows.Count > 0)
                {
                    s = "物料编码 in (";
                    foreach (DataRow xx in dt_x.Rows)
                    {
                        s = s + "'" + xx["子项编码"].ToString() + "',";
                    }
                    s = s.Substring(0, s.Length - 1) + ")";
                    DataView dv_z = new DataView(IncompleteWorkOrdercopy);
                    dv_z.RowFilter = s;
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        gridControl2.DataSource = dv_z;
                    }));
                }
                else
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        gridControl2.DataSource = IncompleteWorkOrdercopy.Clone();
                    }));
                }
            });
            th.Start();
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc2, new Point(e.X, e.Y));
                gv2.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                contextMenuStrip1.Tag = gv2;
            }
        }
        private void 查看bom明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView ff = ((sender as ToolStripDropDownItem).Owner as ContextMenuStrip).Tag as DevExpress.XtraGrid.Views.Grid.GridView;
            DataRow r = ff.GetDataRow(ff.FocusedRowHandle);

            Decimal dec = 1;
            if (contextMenuStrip1.Tag == gridView2) //917行 在哪个gridview 右击的设置该值
            {
                if (r["未完成数量"] != DBNull.Value && r["未完成数量"].ToString() != "")
                {
                    dec = Convert.ToDecimal(r["未完成数量"].ToString());
                }
                else
                {
                    dec = 1;
                }
            }
            else
            {
                if (r["参考数量"] != DBNull.Value && r["参考数量"].ToString() != "")
                {
                    dec = Convert.ToDecimal(r["参考数量"].ToString());
                }
                else
                {
                    dec = 1;
                }
            }
            ERPproduct.UI物料BOM详细数量 frm = new ERPproduct.UI物料BOM详细数量(r["物料编码"].ToString().Trim(), dec);
            CPublic.UIcontrol.AddNewPage(frm, "详细数量");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
                {
                    string s = "自制='true'";
                    DataTable ListM = new DataTable();
                    ListM = ERPorg.Corg.billofM(ListM, searchLookUpEdit1.EditValue.ToString(), true,dt_bom);
                    if (ListM.Rows.Count > 0)
                    {
                        DataView dv = new DataView(dtM);
                        s = s + " and 物料编码 in (";
                        foreach (DataRow dr in ListM.Rows)
                        {
                            s = s + string.Format("'{0}',", dr["子项编码"]);
                        }
                        s = s.Substring(0, s.Length - 1) + ")";
                        dv.RowFilter = s;
                        gc2.DataSource = dv;
                    }
                    else
                    {
                        MessageBox.Show("无数据");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            DataView dv = new DataView(dtM);
            dv.RowFilter = "自制='true'";
            gc2.DataSource = dv;
        }
        private void ui临时用生产计划池_Load(object sender, EventArgs e)
        {
            //CultureInfo uiCulture1 = CultureInfo.CurrentUICulture;
            //uiCulture1.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Wednesday;
            //DateTime t = CPublic.Var.getDatetime().Date;
            //int x = ERPorg.Corg.WeekOfYear(t, uiCulture1);
            //label5.Text= string.Format("{0}年第{1}周", t.Year, x);
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            DateTime t = CPublic.Var.getDatetime().Date;
            string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DW导入销售单\\计划";
            if (Directory.Exists(fileName) == false)
            {
                Directory.CreateDirectory(fileName);

            }



        }

        private void simpleButton6_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Excel文件(*.xlsx)|*.xlsx";
            if (save.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(save.FileName, FileMode.Create, FileAccess.Write);
                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = string.Format("select * from 基础记录打印模板表 where 模板名 = '计划池导入销售模板'");
                dtPP = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (dtPP.Rows.Count == 0) throw new Exception("没有该模板");
                System.IO.File.WriteAllBytes(save.FileName + ".xlsx", (byte[])dtPP.Rows[0]["数据"]);
                MessageBox.Show(" 下载成功");

            }

        }

        private void GridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gridControl2, new Point(e.X, e.Y));
                gridView2.CloseEditor();
                this.BindingContext[IncompleteWorkOrdercopy].EndCurrentEdit();
                contextMenuStrip1.Tag = gridView2;
            }
        }

        private void GridView2_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (gridView2.GetRowCellValue(e.RowHandle, "标识").ToString() == "1")
                {
                    e.Appearance.BackColor = Color.Pink;

                }
            }
            catch
            {


            }

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dtM != null && dtM.Columns.Count > 0 && dtM.Rows.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";

                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    ERPorg.Corg.TableToExcel(dtM, saveFileDialog.FileName);
                    MessageBox.Show("导出成功");
                }
            }
            else
            {
                MessageBox.Show("无记录可导出");
            }
        }
    }
}
