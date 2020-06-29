using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
namespace ERPpurchase
{
    public partial class ui委外发料核销界面 : UserControl
    {


        #region variable
        string strcon = CPublic.Var.strConn;
        DataTable dt_入库 = new DataTable();
        DataTable dt_发料 = new DataTable();
        string cfgfilepath = "";

        #endregion

        #region formload

        public ui委外发料核销界面()
        {
            InitializeComponent();
        }

        private void ui委外发料核销界面_Load(object sender, EventArgs e)
        {
            try
            {

                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(this.splitContainer1, this.Name, cfgfilepath);
                fun_load();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        #endregion



        #region function
        private void fun_load()
        {
            /*(入库单号 in 
   ( select  入库单号   from 其他出库子表 a
   left  join 采购记录采购单入库明细 b on a.备注=b.采购单明细号
   inner join  采购记录采购单主表  c on b.采购单号 =c.采购单号
   where c.采购单类型 ='委外采购'  and 委外已核量=0
   group by 入库单号) or 委外核销=0)*/
            string sql = @"select a.*,d.已送检数 as 总送检数,sjmx.送检数量 ,采购单类型,b.供应商,b.税率,c.物料名称,经办人 ,未完成数量,不合格数量,送检单总入量
   ,d.明细完成日期 as 记录完成 from [采购记录采购单入库明细] a  
   left join 采购记录采购单主表 b on a.采购单号=b.采购单号
    left join 基础数据物料信息表 c on c.物料编码=a.物料编码 
    left  join 采购记录采购单明细表 d on d.采购明细号 =a.采购单明细号  
    left join 采购记录采购送检单明细表 sjmx on sjmx.送检单明细号=a.送检单明细号
    left join (select  送检单明细号,SUM(不合格数量)as 不合格数量 from [采购记录采购单检验主表] group by 送检单明细号)x on x.送检单明细号=sjmx.送检单明细号 
     left join (select 送检单明细号,sum(入库量)as 送检单总入量,仓库ID,仓库名称  from 采购记录采购单入库明细  group by 送检单明细号,仓库ID,仓库名称 )y  on  y.送检单明细号=sjmx.送检单明细号
    where 采购单类型='委外采购' and  委外核销=0 and a.作废=0   and a.生效日期>'2019-5-1'  ";
            dt_入库 = new DataTable();
            dt_入库 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dt_入库;
        }

        private void fun_detail(string str,decimal dec)
        {
            //20-4-29加 
            string s_add = "";
            if(dec<0) //入库量 小于0 就是退货的
            {
                s_add= "and b.数量<0";
            }
            string sql = $@"select  a.*,b.备注 as 备注b,d.物料编码 ,d.物料名称 as 名称,d.规格型号,供应商,f.数量 as BOM数量,原因分类  from  其他出库子表 a
inner  join 其他出入库申请子表 b  on  a.出入库申请明细号=b.出入库申请明细号
inner  join 其他出入库申请主表 c  on  c.出入库申请单号=b.出入库申请单号
inner  join 基础数据物料信息表 d  on  d.物料编码=a.物料编码 

inner join 采购记录采购单明细表 e on e.采购明细号=b.备注
inner join 基础数据物料信息表 fx on fx.物料编码=e.物料编码
left  join 基础数据物料BOM表 f on f.产品编码=fx.物料编码 and f.子项编码=a.物料编码
where 原因分类 in ('委外加工','委外补料','委外退料')   and abs(委外已核量)<abs(a.数量) and b.备注='{str}' {s_add}  " ;
            dt_发料 = new DataTable();
            dt_发料 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            dt_发料.Columns.Add("核销数量", typeof(decimal));
            DataColumn dc = new DataColumn("选择", typeof(bool));
            dc.DefaultValue = false;
            dt_发料.Columns.Add(dc);
            gcM.DataSource = dt_发料;

        }
        bool bl = true;
        bool p_Need_complete = false;
        private void fun_check_common()
        {

            if (dt_发料.Rows.Count == 0)
            {

                throw new Exception("没有明细");
            }
            DataView f = new DataView(dt_发料);
            f.RowFilter = "选择=1";
            if (f.Count == 0)
            {
                throw new Exception("未勾选明细");
            }
            //2020-3-16 判断是否超过总量
            MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
            DataTable dt_group = RBQ.SelectGroupByInto("", f.ToTable(), "物料编码,备注,sum(核销数量) 核销数量", "", "物料编码,备注");
            foreach (DataRow dr in dt_group.Rows)
            {
                string s = $@"select  a.物料编码,sum(a.数量)总发出,SUM(b.数量) as 总需数量,isnull(SUM(物料核销数),0) 已核销总,a.备注,b.委外bom数量 from 其他出库子表 a
                left join 其他出入库申请子表 b  on a.出入库申请明细号 = b.出入库申请明细号
                left join  委外核销明细表 c on c.其他出库明细号 = a.其他出库明细号
                 where a.备注='{dr["备注"].ToString()}' and a.物料编码='{dr["物料编码"].ToString()}'
                 group by a.物料编码,a.备注,b.委外bom数量 order by a.备注";
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (temp.Rows.Count > 0)
                {
                    decimal dec = 0;
                    dec = Convert.ToDecimal(dr["核销数量"]) + Convert.ToDecimal(temp.Rows[0]["已核销总"]);
                    if (dec > Convert.ToDecimal(temp.Rows[0]["总发出"]))
                    {
                        throw new Exception("核销数量大于总发出数量");
                    }
                    if (dec == Convert.ToDecimal(temp.Rows[0]["总发出"]) && Convert.ToDecimal(temp.Rows[0]["总发出"]) == Convert.ToDecimal(temp.Rows[0]["总需数量"]))
                    {
                        //采购明细表 里面的明细完成是送检完成的意思
                        string ss = $"select * from 采购记录采购单明细表 where 采购明细号='{dr["备注"].ToString()}' and 明细完成=0";
                        DataTable tt = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
                        if (tt.Rows.Count > 0) //  满足上面的条件 并且又未送检完成的采购明细 比如一个委外明细 发多个料 只要其中一个料发完了剩余不送检
                        {
                            //按供应链要求 若采购明细尚有未送检数量 该采购明细需要自动完成 并且提示采购人员
                            if (MessageBox.Show("已满足自动关闭采购单明细的要求,即将自动完成,是否确认继续？", "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                            {
                                p_Need_complete = true;
                            }
                            else
                            {
                                p_Need_complete = false;
                                throw new Exception("已取消");
                            }
                        }
                        
                    }
                    
                }
            }

            bool bl = false;
            foreach (DataRow xr in dt_发料.Rows)
            {
                if (xr["选择"].Equals(true))
                {
                    if (xr["原因分类"].ToString() == "委外退料")
                    {
                        if (Convert.ToDecimal(xr["核销数量"]) > 0) throw new Exception("委外退料需核销数量为负数,请确认");
                        bl = true;
                    }
                    else
                    {
                        if (Convert.ToDecimal(xr["核销数量"]) < 0) throw new Exception("委外发料或补料需核销数量需大于0,请确认");
                    }
                    if (Math.Abs(Convert.ToDecimal(xr["核销数量"])) > Math.Abs(Convert.ToDecimal(xr["数量"])) - Math.Abs(Convert.ToDecimal(xr["委外已核量"]))) //没有全部核掉
                    {
                        throw new Exception("输入核销量大于未核量");
                    }

                    if (Math.Abs(Convert.ToDecimal(xr["核销数量"])) < Math.Abs(Convert.ToDecimal(xr["核销数量", DataRowVersion.Original])))
                    {
                        throw new Exception("输入核销量不可小于推荐值");
                    }
                    ////2019-11-20 戴会计限定暂时为8%
                    /////2020-3-16 供应链觉得不按每笔 按该采购明细总量限制,并且物料核销完了需要把采购明细完成 不管有没有未送检的
                    //if (Math.Abs(Convert.ToDecimal(xr["核销数量"])) > Math.Abs(Convert.ToDecimal(xr["核销数量", DataRowVersion.Original])) * (decimal)1.08)
                    //{
                    //    bl = false;
                    //    throw new Exception("输入核销量不可大于推荐值上限8%");
                    //}
                    //if (Math.Abs(Convert.ToDecimal(xr["核销数量"])) > Math.Abs(Convert.ToDecimal(xr["核销数量", DataRowVersion.Original])))
                    //{
                    //    if (MessageBox.Show("输入核销数量已经大于推荐值,是否确认？", "警告", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    //    {
                    //        throw new Exception("已取消");
                    //    }

                    //}
                }
            }
            if (bl)
            {
                
                    MessageBox.Show("明细中有委外退料,请自行确认好累加起来的核销数量是否正确");
            }
        }
        private void fun_check_islast()
        {
            // 先判断 当前入库单 是否为当前采购明细的 最后一条未核销入库单 
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (dr["记录完成"] != DBNull.Value && Convert.ToDecimal(dr["未完成数量"]) == 0) //已全部入库，只要判断dt_入库 是否还有该采购明细的入库单  //2018-7-3 有可能会有剩余不送检的
            {
                DataRow[] r = dt_入库.Select(string.Format("采购单明细号='{0}' and 入库单号<>'{1}' ", dr["采购单明细号"].ToString(), dr["入库单号"].ToString()));
                if (r.Length == 0) //该条核销记录是最后一条，则明细需要全部核销完成，判断明细是否全部核销完
                {
                    foreach (DataRow xr in dt_发料.Rows)
                    {
                        if (!xr["选择"].Equals(true))
                        {
                            throw new Exception("该入库单为该采购明细最后一张未处理单据,剩余发料明细都核销即可");
                        }
                        else
                        {
                            if (Math.Abs(Convert.ToDecimal(xr["核销数量"])) < Math.Abs(Convert.ToDecimal(xr["数量"])) - Math.Abs(Convert.ToDecimal(xr["委外已核量"]))) //没有全部核掉
                            {
                                //throw new Exception("该入库单为该采购明细最后一张未处理单据,剩余发料明细都核销即可");
                            }
                        }
                    }
                }
            }
        }
        private void fun_save()
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            dr["委外核销"] = true; //dt_入库
            DateTime t = CPublic.Var.getDatetime();
            string s = "select  * from  委外核销明细表 where 1=2";
            DataTable dtP = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            foreach (DataRow xr in dt_发料.Rows)//dt_发料
            {
                if (xr["选择"].Equals(true))
                {
                    xr["委外已核量"] = Convert.ToDecimal(xr["核销数量"]) + Convert.ToDecimal(xr["委外已核量"]);
                    DataRow dr_p = dtP.NewRow();
                    dr_p["入库单号"] = dr["入库单号"];
                    dr_p["采购明细号"] = dr["采购单明细号"];
                    dr_p["产品入库量"] = dr["入库量"];
                    dr_p["其他出库明细号"] = xr["其他出库明细号"];
                    dr_p["物料核销数"] = xr["核销数量"];
                    dr_p["产品编码"] = dr["物料编码"];
                    dr_p["子项编码"] = xr["物料编码"];
                    dr_p["出库原因"] = xr["原因分类"];
                    //核销日期取  入库单的日期
                    dr_p["核销日期"] = dr["生效日期"];
                    dr_p["操作日期"] = t;
                    dr_p["核销人员"] = CPublic.Var.localUserName;
                    dr_p["核销人员ID"] = CPublic.Var.LocalUserID;
                    dtP.Rows.Add(dr_p);
                }
            }
            //2020-3-16 
            DataTable dt_P = new DataTable() ;
            if(p_Need_complete)
            {
                string p = $"select  * from 采购记录采购单明细表 where 采购明细号='{ dr["采购单明细号"].ToString()}'";
                dt_P = CZMaster.MasterSQL.Get_DataTable(p,strcon);
                dt_P.Rows[0]["明细完成"] = 1;
                dt_P.Rows[0]["备注2"] = $"委外物料全部核销系统自动完成,剩余不送检 {t}";
            }

            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction wwhx = conn.BeginTransaction("wwhx");
            try
            {
                SqlCommand cmd = new SqlCommand("select * from 采购记录采购单入库明细 where 1<>1", conn, wwhx);
                SqlDataAdapter aa = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(aa);
                aa.Update(dt_入库);
                cmd = new SqlCommand("select * from 其他出库子表 where 1<>1", conn, wwhx);
                aa = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(aa);
                aa.Update(dt_发料);
                cmd = new SqlCommand("select * from 委外核销明细表 where 1<>1", conn, wwhx);
                aa = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(aa);
                aa.Update(dtP);
                if (p_Need_complete)
                {
                    cmd = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, wwhx);
                    aa = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(aa);
                    aa.Update(dt_P);
                }
                wwhx.Commit();
            }
            catch (Exception ex)
            {
                wwhx.Rollback();
                throw new Exception(ex.Message + "保存失败");
            }
        }
        #endregion

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (dr == null) return;
            decimal rkl = Convert.ToDecimal( dr["入库量"].ToString());

            fun_detail(dr["采购单明细号"].ToString(), rkl);
            foreach (DataRow wl in dt_发料.Rows)
            {
                if (wl["原因分类"].ToString() == "委外补料")
                {
                    wl["核销数量"] = Convert.ToDecimal(wl["数量"]) - Convert.ToDecimal(wl["委外已核量"]);
                }
                else
                {
                    if (wl["BOM数量"] == DBNull.Value)
                    {
                        wl["BOM数量"] = Convert.ToDecimal(wl["数量"]) / Convert.ToDecimal(dr["采购数量"]);
                    }
                    decimal dec = Convert.ToDecimal(dr["入库量"]) * Convert.ToDecimal(wl["BOM数量"]);
                    wl["核销数量"] = dec;
                    if (dec>0 && Convert.ToDecimal(wl["数量"]) - Convert.ToDecimal(wl["委外已核量"]) < dec)
                    {
                        wl["核销数量"] = Convert.ToDecimal(wl["数量"]) - Convert.ToDecimal(wl["委外已核量"]);
                    }
                }
            }
            dt_发料.AcceptChanges();
        }
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("是否确认核销？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    gvM.CloseEditor();
                    this.BindingContext[dt_发料].EndCurrentEdit();
                    this.ActiveControl = null;
                    fun_check_common();
                    fun_check_islast();
                    fun_save();
                    MessageBox.Show("核销成功");
                    fun_load();
                    DataTable dt = dt_发料.Clone();
                    gcM.DataSource = dt;
                }
           
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //刷新 
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_load();
                DataTable t = dt_发料.Clone();
                gcM.DataSource = t;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }





    }
}
