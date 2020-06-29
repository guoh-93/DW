using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ERPSale
{
    public partial class frm销售记录成库通知单详细界面 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataRow drM;
        DataTable dtM;
        DataTable dtP;
        DataTable dt_客户信息;
        string str_出库通知单号 = "";
        Boolean bl_新增or修改 = false;
        DataTable dt_待办;
        DataTable dt_已通知数量 = null;
        DataTable dt_主; //12/8 加
        DataTable dt_审核;

        DataTable dt_仓库, dt_成品出库单主表, dt_成品出库单明细表;
        bool bl_维护 = false;

        bool bl_是否生成审核 = false;
        #endregion

        #region 自用类
        public frm销售记录成库通知单详细界面()
        {
            InitializeComponent();
            bl_新增or修改 = true;
            fun_载入();
        }
        //2019-12-5   查询到好像没有用到，暂不处理
        public frm销售记录成库通知单详细界面(DataRow dr, DataTable dt)
        {
            InitializeComponent();
            bl_新增or修改 = true;
            drM = dr;
            dtM = dt;
        }
        //19-15-5  销售出库通知查询界面跳转过来
        public frm销售记录成库通知单详细界面(string str, DataRow dr, DataTable dt)
        {
            InitializeComponent();
            bl_新增or修改 = false;
            str_出库通知单号 = str;
            drM = dr;
            dtM = dt;
            barLargeButtonItem7.Enabled = true;
        }

        private void frm销售记录成库通知单详细界面_Load(object sender, EventArgs e)
        {
            try
            {
                //devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
                //devGridControlCustom1.strConn = CPublic.Var.strConn;
                txt_操作员ID.Text = CPublic.Var.LocalUserID;
                txt_操作员.Text = CPublic.Var.localUserName;
                txt_出库日期.EditValue = CPublic.Var.getDatetime();
                //
                string sql = "select 属性值 as 快递公司  from 基础数据基础属性表 where 属性类别 = '快递公司' ";//只显示可用商品库
                DataTable dt_快递公司 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                searchLookUpEdit3.Properties.DataSource = dt_快递公司;
                searchLookUpEdit3.Properties.DisplayMember = "快递公司";
                searchLookUpEdit3.Properties.ValueMember = "快递公司";
                fun_load();
                string a = "";
                if (!bl_新增or修改)
                    a = drM["销售订单号"].ToString();
                fun_载入待办(a);
                //
                fun_载入空主表();
                //fun_载入明细();
                fun_客户编号();
                //fun_载入待办();
                searchLookUpEdit1.Properties.DataSource = dt_客户信息;
                searchLookUpEdit1.Properties.DisplayMember = "客户编号";
                searchLookUpEdit1.Properties.ValueMember = "客户编号";

                fun_仓库();



            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }

        private void fun_仓库()
        {
            string sql = "select 属性值 as 仓库名称,属性字段1 as 仓库号 from 基础数据基础属性表 where 属性类别 = '仓库类别' and 布尔字段3 = 1";//只显示可用商品库
            dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            repositoryItemSearchLookUpEdit1.DataSource = dt_仓库;
            repositoryItemSearchLookUpEdit1.DisplayMember = "仓库号";
            repositoryItemSearchLookUpEdit1.ValueMember = "仓库号";
        }

        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            gv_待办.CloseEditor();
            gc_待办.BindingContext[dt_待办].EndCurrentEdit();
            DataRow drr = gv_待办.GetDataRow(gv_待办.FocusedRowHandle);
            try
            {
                if (drr["选择"].Equals(true))
                {
                    DataRow[] rr = dtP.Select(string.Format("销售订单明细号='{0}'", drr["销售订单明细号"]));
                    if (rr.Length == 0)
                    {
                        //foreach (DataRow rr in dtP.Rows)
                        //{
                        //    if (rr.RowState == DataRowState.Deleted)
                        //    {
                        //        continue;
                        //    }
                        //    if (drr["销售订单明细号"].ToString() == rr["销售订单明细号"].ToString())
                        //    {
                        //        continue;
                        //    }
                        //    else
                        //    {
                        //        count++;
                        //    }
                        //}
                        //if (count == dtP.Rows.Count)
                        //{
                        txt_客户编号.Text = drr["客户编号"].ToString();
                        txt_客户名.Text = drr["客户"].ToString();
                        DataRow dr = dtP.NewRow();
                        dtP.Rows.Add(dr);

                        //string str_销售单号 = drr["销售订单明细号"].ToString().Substring(0, 14);
                        //string sql = string.Format("select * from 销售记录销售订单主表 where  销售订单号='{0}'", str_销售单号);
                        //DataTable dtM = new DataTable();
                        //dtM = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        //if (dtM.Rows.Count > 0)
                        //{
                        //    dr["客户订单号"] = dtM.Rows[0]["客户订单号"].ToString();
                        //}
                        txt_备注.Text = drr["发货备注"].ToString();
                        dr["GUID"] = System.Guid.NewGuid();
                        dr["销售订单明细号"] = drr["销售订单明细号"].ToString();
                        dr["物料编码"] = drr["物料编码"].ToString();
                        dr["物料名称"] = drr["物料名称"].ToString();
                        dr["规格型号"] = drr["规格型号"].ToString();
                        dr["图纸编号"] = drr["图纸编号"].ToString();
                        dr["计量单位"] = drr["计量单位"].ToString();
                        dr["仓库号"] = drr["仓库号"].ToString();
                        dr["仓库名称"] = drr["仓库名称"].ToString();
                        dr["出库数量"] = Convert.ToDecimal(drr["未通知数量"]) - Convert.ToDecimal(drr["已通知未审"]); ;

                        dr["特殊备注"] = drr["特殊备注"].ToString();

                        // dr["n原ERP规格型号"] = drr["n原ERP规格型号"].ToString();

                        dr["仓库数量"] = drr["库存数量"].ToString();
                        dr["销售备注"] = drr["备注"].ToString();


                    }
                }
                else
                {
                    //txt_客户编号.Text = "";
                    DataRow[] ds = dtP.Select(string.Format("销售订单明细号 = '{0}'", drr["销售订单明细号"].ToString()));
                    if (ds.Length > 0)
                    {
                        ds[0].Delete();
                    }
                    if (dtP.Rows.Count == 0)
                    {
                        txt_客户编号.Text = "";

                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "出库通知_repositoryItemCheckEdit1_CheckedChanged");
            }
        }
        void dt_待办_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            try
            {
                if (e.Column.ColumnName == "选择")
                {
                    if (e.Row["选择"].ToString().ToLower() == "true")
                    {
                        int count = 0;
                        foreach (DataRow rr in dtP.Rows)
                        {
                            if (rr.RowState == DataRowState.Deleted)
                            {
                                continue;
                            }
                            if (e.Row["销售订单明细号"].ToString() == rr["销售订单明细号"].ToString())
                            {
                                continue;
                            }
                            else
                            {
                                count++;
                            }
                        }
                        if (count == dtP.Rows.Count)
                        {
                            txt_客户编号.Text = e.Row["客户编号"].ToString();
                            DataRow dr = dtP.NewRow();
                            dtP.Rows.Add(dr);
                            dr["GUID"] = System.Guid.NewGuid();
                            dr["销售订单明细号"] = e.Row["销售订单明细号"].ToString();
                            dr["物料编码"] = e.Row["物料编码"].ToString();
                            dr["物料名称"] = e.Row["物料名称"].ToString();
                            dr["规格型号"] = e.Row["规格型号"].ToString();
                            dr["图纸编号"] = e.Row["图纸编号"].ToString();
                            dr["计量单位"] = e.Row["计量单位"].ToString();
                            dr["出库数量"] = e.Row["未通知数量"].ToString();

                            // dr["n原ERP规格型号"] = e.Row["n原ERP规格型号"].ToString();
                            //string sqll = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", e.Row["物料编码"].ToString());
                            //DataTable dtt = new DataTable();
                            //SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn);
                            //daa.Fill(dtt);
                            //dr["仓库数量"] = dtt.Rows[0]["有效总数"].ToString();   
                            dr["仓库数量"] = e.Row["库存数量"].ToString();


                            dr["销售备注"] = e.Row["备注"].ToString();

                        }
                    }
                    else
                    {
                        DataRow[] ds = dtP.Select(string.Format("销售订单明细号 = '{0}'", e.Row["销售订单明细号"].ToString()));
                        if (ds.Length > 0)
                        {
                            ds[0].Delete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }
        private void fun_载入待办(string str_销售)
        {
            try
            {
                string ss = "";
                string s2 = "";
                if (bl_新增or修改) //false 跳转过来的 正常的都要限制
                {
                    ss = "and 未通知数量> ISNULL(已通知未审, 0)";
                }
                else
                {
                    s2 = string.Format("and 出库通知单号<>'{0}'", str_出库通知单号);
                }

                string sql = string.Format(@"select smx.*,isnull(kc.库存总数,0) as 库存数量,原ERP物料编号,(sz.销售备注) as 发货备注,ISNULL(已通知未审,0)已通知未审 from 销售记录销售订单明细表 smx
                   left join 仓库物料数量表 kc on smx.物料编码=kc.物料编码   and  smx.仓库号=kc.仓库号
                    right join 销售记录销售订单主表 sz on   sz.销售订单号=smx.销售订单号
                    left join 基础数据物料信息表 base on    base.物料编码=smx.物料编码
                    left join (select 销售订单明细号,SUM(出库数量)已通知未审 from  销售记录销售出库通知单明细表 where 生效=0 and 作废=0 {2}  group by 销售订单明细号)dd
                    on dd.销售订单明细号=smx.销售订单明细号                    
					 where    smx.生效 = 1 and smx.作废 = 0 and smx.明细完成 = 0 and smx.关闭=0 
					 {1} and smx.销售订单号='{0}' order by 销售订单明细号", str_销售, ss, s2);
                dt_待办 = new DataTable();
                dt_待办.Columns.Add("选择", typeof(Boolean));
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_待办);
                fun_载入明细(); //19-7-10 载入dtP

                // dv.RowFilter = "未通知数量 > 0";
                if (!bl_新增or修改) //false 跳转过来的 正常的都要限制
                {
                    foreach (DataRow r in dtP.Rows)
                    {
                        DataRow[] rp = dt_待办.Select(string.Format("销售订单明细号='{0}'", r["销售订单明细号"]));
                        if (rp.Length > 0)
                        {
                            rp[0]["选择"] = true;
                        }

                    }

                }
                gc_待办.DataSource = dt_待办;

                //dt_待办.ColumnChanged += dt_待办_ColumnChanged;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
                throw ex;
            }
        }
        private void fun_load()
        {
            string sql = string.Format(@"  select 销售记录销售订单主表.*,联系人,地址  from  销售记录销售订单主表 
                    left join 客户基础信息表 khxx on khxx.客户编号 = 销售记录销售订单主表.客户编号
                      
                    where 销售记录销售订单主表.销售订单号 in 
                    (select  销售订单号 from 销售记录销售订单明细表 
                    left join (select 销售订单明细号,SUM(出库数量)已通知未审 from  销售记录销售出库通知单明细表 where 生效=0 and 作废=0  group by 销售订单明细号)dd
                    on dd.销售订单明细号=销售记录销售订单明细表.销售订单明细号
                    where 生效 = 1 and 作废 = 0 and 关闭 = 0 and 明细完成 = 0 and 未通知数量>ISNULL(已通知未审,0) group by 销售订单号)                        
                    and 销售记录销售订单主表.关闭 = 0 and 销售记录销售订单主表.作废 = 0");

            string sql1 = "";
            string localuser = CPublic.Var.localUserName;
            string locakteam = CPublic.Var.LocalUserTeam;

            if (localuser != "admin" && locakteam != "管理员权限")
            {
                sql1 = "and 销售记录销售订单主表.部门编号 = '" + CPublic.Var.localUser部门编号 + "'";
                sql = sql + sql1;
            }
            dt_主 = new DataTable();
            dt_主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gridControl1.DataSource = dt_主;
            sql = "select  属性值  from 基础数据基础属性表 where 属性类别='送货方式'";
            DataTable temp = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            comboBox1.Items.Clear();
            foreach (DataRow r in temp.Rows)
            {
                comboBox1.Items.Add(r["属性值"]);
            }



        }
        private void txt_客户编号_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DataRow[] ds = dt_客户信息.Select(string.Format("客户编号 = '{0}'", txt_客户编号.EditValue));
                if (ds.Length != 0)
                {
                    txt_客户名.Text = ds[0]["客户名称"].ToString();
                    //dv.RowFilter = string.Format("未通知数量 > 0 and 客户 = '{0}'", ds[0]["客户名称"].ToString());
                    //dr_传.Clear();
                    //foreach (DataRow r_x in dt_待办.Rows)
                    //{
                    //    if (r_x["客户编号"] != txt_客户编号.EditValue)
                    //    {
                    //        r_x["选择"] = false;
                    //    }
                    //}
                    //gc_待办.DataSource = dv;
                }
                if (txt_客户编号.EditValue.ToString() == "")
                {
                    txt_客户名.Text = "";
                    //dv.RowFilter = "未通知数量 > 0 ";
                    //foreach (DataRow r_x in dt_待办.Rows)
                    //{
                    //    r_x["选择"] = false;
                    //}
                    //gc_待办.DataSource = dv;
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售出库通知单界面_txt_客户编号_EditValueChanged");
            }
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DataRow[] ds = dt_客户信息.Select(string.Format("客户编号 = '{0}'", searchLookUpEdit1.EditValue));
                if (ds.Length != 0)
                {
                    txt_客户名.Text = ds[0]["客户名称"].ToString();
                    txt_客户编号.EditValue = ds[0]["客户编号"].ToString();
                }
                if (searchLookUpEdit1.EditValue.ToString() == "")
                {
                    txt_客户名.Text = "";
                    txt_客户编号.EditValue = "";
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售出库通知单界面_txt_客户编号_EditValueChanged");
            }
        }
        #endregion

        #region 待办  方法
        DataView dv;
        /// <summary>
        /// 不用
        /// </summary>
        private void fun_载入待办()
        {
            try
            {
                string sql = string.Format(@"select 销售记录销售订单明细表.*,仓库物料数量表.库存总数 as 库存数量 ,(销售记录销售订单主表.销售备注) as 发货备注 from 销售记录销售订单明细表
                   left join 仓库物料数量表 on 销售记录销售订单明细表.物料编码=仓库物料数量表.物料编码 
                    right join 销售记录销售订单主表 on   销售记录销售订单主表.销售订单号=销售记录销售订单明细表.销售订单号
                     where    销售记录销售订单明细表.生效 = 1 and 销售记录销售订单明细表.作废 = 0 and 
                      销售记录销售订单明细表.明细完成 = 0 and 未通知数量>0  order by 销售订单明细号 ");

                dt_待办 = new DataTable();
                dt_待办.Columns.Add("选择", typeof(Boolean));
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_待办);

                dv = new DataView(dt_待办);
                dv.RowFilter = "未通知数量 > 0";
                gc_待办.DataSource = dv;
                //dt_待办.ColumnChanged += dt_待办_ColumnChanged;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
                throw ex;
            }
        }
        #endregion

        #region 通知  方法
        private void fun_载入()
        {
            try
            {
                string sql = "select * from 销售记录销售出库通知单主表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                dtM = new DataTable();
                da.Fill(dtM);
                drM = dtM.NewRow();
                dtM.Rows.Add(drM);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售记录成品出库通知单界面_fun_载入");
            }
        }

        private void fun_载入空主表()
        {
            if (bl_新增or修改 == true)
            {
                string sql = string.Format("select * from 销售记录销售出库通知单主表 where 1=2");
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                drM = dtM.NewRow();
                dtM.Rows.Add(drM);

            }
            else
            {
                dataBindHelper1.DataFormDR(drM);
            }
        }

        private void fun_载入明细()
        {
            string sql = "";
            if (bl_新增or修改 == true)
            {
                sql = @"select 销售记录销售出库通知单明细表.*,(仓库物料数量表.库存总数) as 仓库数量 from 销售记录销售出库通知单明细表,仓库物料数量表
                           
                                    where 仓库物料数量表.物料编码=销售记录销售出库通知单明细表.物料编码 and 1<>1";

                //sql = "select * from 销售记录销售出库通知单明细表 where 1<>1";
            }
            else
            {
                sql = string.Format(@"select stcmx.*,原ERP物料编号,(kc.库存总数)as 仓库数量 
                           from 销售记录销售出库通知单明细表 stcmx,仓库物料数量表 kc,基础数据物料信息表 base,销售记录销售订单明细表 smx 
                            where kc.物料编码=stcmx.物料编码 and base.物料编码=stcmx.物料编码  and smx.仓库号=kc.仓库号 and smx.销售订单明细号=stcmx.销售订单明细号
                            and 出库通知单号 = '{0}'", str_出库通知单号);

                //sql = string.Format("select * from 销售记录销售出库通知单明细表 where 出库通知单号 = '{0}'", str_出库通知单号);
            }
            dtP = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtP);
            //dtP.Columns.Add("客户订单号");
            //dr_传.Columns.Add("仓库数量");
            gc.DataSource = dtP;
        }

        private void fun_保存主表()
        {
            try
            {

                DateTime t = CPublic.Var.getDatetime();
                string s_部门 = CPublic.Var.localUser部门名称;
                if (bl_新增or修改 == true)
                {
                    fun_销售出库通知单();
                    txt_出库通知单号.Text = str_出库通知单号;
                    drM["GUID"] = System.Guid.NewGuid();
                    drM["创建日期"] = t;
                }
                try
                {
                    drM["出库日期"] = txt_出库日期.EditValue;
                    drM["包装日期"] = txt_出库日期.EditValue;

                    drM["操作员"] = CPublic.Var.localUserName;
                    drM["操作员ID"] = CPublic.Var.LocalUserID;
                    drM["修改日期"] = t;
                    if (dateEdit1.EditValue != null)
                    {
                        drM["客户提货日期"] = dateEdit1.EditValue;
                    }

                    // drM["快递公司ID"] = searchLookUpEdit2.EditValue;
                    dataBindHelper1.DataToDR(drM);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                string s_订单号 = "";
                foreach (DataRow dr in dtP.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    s_订单号 = dr["销售订单明细号"].ToString().Substring(0, 14);
                    break;
                }
                //string s_订单号 = dtP.Rows[0]["销售订单明细号"].ToString().Substring(0, 14);


                string sql = string.Format(@"select * from 销售记录销售订单主表 where 销售订单号 = '{0}'", s_订单号);
                DataTable dt_目标客户 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                dt_审核 = new DataTable();
                dt_审核 = ERPorg.Corg.fun_PA("生效", "销售发货申请", str_出库通知单号, dt_目标客户.Rows[0]["目标客户"].ToString()); //此函数内已经区分是新增或修改了
                drM["提交审核"] = true;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单界面保存订单");
                throw ex;
            }
        }

        private void fun_保存明细()
        {
            try
            {
                int i = 1;
                int a = 0;
                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    string s = r["物料编码"].ToString();
                    s = s.Substring(0, 3);// 取得字符串数据的前4位
                    if (s.ToString().Equals("200") || s.ToString() == "110")
                    {
                        a++;
                    }


                    r["POS"] = i++;
                    r["出库通知单号"] = str_出库通知单号;
                    r["出库通知单明细号"] = str_出库通知单号 + "-" + Convert.ToInt32(r["POS"]).ToString("00");
                    r["操作员"] = CPublic.Var.localUserName;
                    r["操作员ID"] = CPublic.Var.LocalUserID;
                    r["客户"] = txt_客户名.Text;
                    r["客户编号"] = txt_客户编号.Text;
                    //r_x["未开票数量"] = Convert.ToDecimal(r_x["出库数量"]);
                    r["未出库数量"] = Convert.ToDecimal(r["出库数量"]);
                }


                if (a == dtP.Rows.Count)
                {
                    bl_维护 = true;

                }


            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单界面保存明细");
                throw new Exception("保存失败！");
            }
        }
        /// <summary>
        /// 增加审核节点后 这边提交审核不需要运行此函数
        /// </summary>
        private void fun_已通知数量()
        {
            dt_已通知数量 = new DataTable();
            foreach (DataRow r in dtP.Rows)
            {
                string sql = string.Format("select * from 销售记录销售订单明细表 where 销售订单明细号 = '{0}'", r["销售订单明细号"].ToString().Trim());
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_已通知数量);
                DataRow[] rrr = dt_已通知数量.Select(string.Format("销售订单明细号 = '{0}'", r["销售订单明细号"].ToString().Trim()));
                rrr[0]["已通知数量"] = Convert.ToDecimal(rrr[0]["已通知数量"]) + Convert.ToDecimal(r["出库数量"]);
                rrr[0]["未通知数量"] = Convert.ToDecimal(rrr[0]["未通知数量"]) - Convert.ToDecimal(r["出库数量"]);

                //if (Convert.ToDecimal(dt_已通知数量.Rows[0]["未通知数量"]) <= 0)
                //{
                //    dt_已通知数量.Rows[0]["明细完成"] = 1;
                //}
            }
        }

        private void fun_事务_保存()
        {
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");
            try
            {
                {
                    string sql = "select * from 销售记录销售出库通知单明细表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dtP);
                    }
                }
                {
                    string sql = "select * from 销售记录销售出库通知单主表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dtM);
                    }
                }

                if (bl_维护)
                {
                    {

                        dt_审核.Rows[0]["审核"] = true;
                        dt_审核.Rows[0]["审核时间"] = DateTime.Now.ToString();

                        string sql = "select * from 销售记录成品出库单主表 where 1<>1";
                        SqlCommand cmd = new SqlCommand(sql, conn, ts);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            new SqlCommandBuilder(da);
                            da.Update(dt_成品出库单主表);
                        }
                    }

                    {
                        string sql = "select * from 销售记录成品出库单明细表 where 1<>1";
                        SqlCommand cmd = new SqlCommand(sql, conn, ts);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            new SqlCommandBuilder(da);
                            da.Update(dt_成品出库单明细表);
                        }
                    }

                    {
                        string sql = "select * from 销售记录销售订单主表 where 1<>1";
                        SqlCommand cmd = new SqlCommand(sql, conn, ts);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            new SqlCommandBuilder(da);
                            da.Update(dt_销售主);
                        }
                    }


                    {
                        string sql = "select * from 销售记录销售订单明细表 where 1<>1";
                        SqlCommand cmd = new SqlCommand(sql, conn, ts);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            new SqlCommandBuilder(da);
                            da.Update(dt_销售mx);
                        }
                    }
                }


                {
                    string sql = "select * from 单据审核申请表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt_审核);
                    }
                }



                //if (dt_已通知数量 != null)
                //{
                //    string sql = "select * from 销售记录销售订单明细表 where 1<>1";
                //    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                //    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                //    {
                //        new SqlCommandBuilder(da);
                //        da.Update(dt_已通知数量);
                //    }
                //}
                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw ex;
            }
        }

        private void fun_销售出库通知单()
        {
            DateTime t = CPublic.Var.getDatetime();
            str_出库通知单号 = string.Format("SK{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("SK", t.Year, t.Month));
        }

        /// <summary>
        /// 给combobox客户编号下拉框 赋值  17/11/17 加入快递公司加载
        /// </summary>
        private void fun_客户编号()
        {
            txt_客户编号.Properties.Items.Clear();

            string sql = "select 客户编号,客户名称 from 客户基础信息表";
            dt_客户信息 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_客户信息);

            foreach (DataRow r in dt_客户信息.Rows)
            {
                txt_客户编号.Properties.Items.Add(r["客户编号"].ToString());
            }
            string ss = "select  编号,名称 from  [快递公司基础信息维护表] where  停用=0";

            DataTable dt = CZMaster.MasterSQL.Get_DataTable(ss, strconn);
            searchLookUpEdit2.Properties.DataSource = dt;
            searchLookUpEdit2.Properties.DisplayMember = "名称";
            searchLookUpEdit2.Properties.ValueMember = "编号";




        }

        private void fun_清空()
        {
            //txt_操作员.Text = CPublic.Var.localUserName;
            txt_出库通知单号.Text = "";
            txt_客户编号.EditValue = "";
            txt_客户名.Text = "";
            txt_出库日期.EditValue = CPublic.Var.getDatetime();
            txt_备注.Text = "";
            //txt_送货方式.Text = "";
            comboBox1.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox5.Text = "";
            textBox4.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            barLargeButtonItem3.Enabled = true;
            barLargeButtonItem7.Enabled = false;
            bl_新增or修改 = true;

            //fun_载入待办();
        }

        private void fun_强载()
        {
            try
            {
                string sql = string.Format("select * from 销售记录销售出库通知单主表 where 出库通知单号 = '{0}'", txt_出库通知单号.Text);
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                drM = dtM.Rows[0];
                dataBindHelper1.DataFormDR(drM);
                //if (dr_传 != null)
                //{
                //    dr_传.Clear();
                //}
                {
                    string sqll = string.Format("select * from 销售记录销售出库通知单明细表 where 出库通知单号 = '{0}'", txt_出库通知单号.Text);
                    using (SqlDataAdapter daa = new SqlDataAdapter(sqll, strconn))
                    {
                        dtP = new DataTable();
                        daa.Fill(dtP);
                        gc.DataSource = dtP;
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "通知单详细界面_fun_强载");
            }
        }

        private void fun_生效()
        {
            //drM["生效"] = 1;
            //drM["生效日期"] = CPublic.Var.getDatetime() ;

            bl_维护 = false;
            fun_保存主表();

            fun_保存明细();
            //fun_已通知数量()
            ////维保增加出库单


            if (bl_维护)
            {
                fun_ck();

            }


            fun_事务_保存();
        }


        #endregion
        DataTable dt_销售主;
        DataTable dt_销售mx;
        string[] nameStrArray = null;
        private void fun_ck()
        {

            //string sql_通知明细 = string.Format("select * from  销售记录销售出库通知单明细表 where 出库通知单号 = '{0}'", rec_num);
            //dt_t通知明细 = CZMaster.MasterSQL.Get_DataTable(sql_通知明细, strconn);
            //string sql_通知 = string.Format("select * from  销售记录销售出库通知单主表 where 出库通知单号 = '{0}'", rec_num);

            //dt_通知主 = CZMaster.MasterSQL.Get_DataTable(sql_通知, strconn);


            DateTime ads = CPublic.Var.getDatetime();
            string s_成品出库单号 = string.Format("SA{0}{1}{2}{3}", ads.Year.ToString(), ads.Month.ToString("00"),
     ads.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SA", ads.Year, ads.Month).ToString("0000"));

            string s_ckz = "select * from 销售记录成品出库单主表 where 1<>1";
            dt_成品出库单主表 = CZMaster.MasterSQL.Get_DataTable(s_ckz, strconn);
            s_ckz = "select * from 销售记录成品出库单明细表 where 1<>1";
            dt_成品出库单明细表 = CZMaster.MasterSQL.Get_DataTable(s_ckz, strconn);
            DataRow dr_成品出库主 = dt_成品出库单主表.NewRow();
            dt_成品出库单主表.Rows.Add(dr_成品出库主);
            dr_成品出库主["GUID"] = System.Guid.NewGuid();
            dr_成品出库主["成品出库单号"] = s_成品出库单号;
            dr_成品出库主["操作员ID"] = CPublic.Var.LocalUserID;
            dr_成品出库主["操作员"] = CPublic.Var.localUserName;

            dr_成品出库主["日期"] = ads;
            dr_成品出库主["创建日期"] = ads;
            dr_成品出库主["修改日期"] = ads;
            dr_成品出库主["生效"] = true;
            dr_成品出库主["生效日期"] = ads;
            // string s_销售订单 = dtP.Rows[0]["销售订单明细号"].ToString();
            string s_销售订单 = "";
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                s_销售订单 = dr["销售订单明细号"].ToString();
                break;
            }
         
            string[] str_销售订单 = s_销售订单.Split('-');
            //dr_stockOutDetaail["销售订单号"] = s_销售订单[0].ToString();
            int k = 0;
            string sql = string.Format("select * from 销售记录销售订单明细表 where 销售订单号 = '{0}'", str_销售订单[0].ToString().Trim());
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            dt_销售mx = new DataTable();
            da.Fill(dt_销售mx);


            string sql_x = string.Format("select * from 销售记录销售订单主表  where  销售订单号='{0}'", str_销售订单[0].ToString().Trim());

            dt_销售主 = new DataTable();
            dt_销售主 = CZMaster.MasterSQL.Get_DataTable(sql_x, strconn);

            foreach (DataRow r in dtP.Rows)
            {


                DataRow[] rrr = dt_销售mx.Select(string.Format("销售订单明细号 = '{0}'", r["销售订单明细号"].ToString().Trim()));
                decimal a_未完成 = Convert.ToDecimal(rrr[0]["未完成数量"]);


                rrr[0]["已通知数量"] = Convert.ToDecimal(rrr[0]["已通知数量"]) + Convert.ToDecimal(r["出库数量"]);
                rrr[0]["未通知数量"] = Convert.ToDecimal(rrr[0]["未通知数量"]) - Convert.ToDecimal(r["出库数量"]);

                string ass = r["物料编码"].ToString();
                ass = ass.Substring(0, 3);
                string aaa = r["物料编码"].ToString();
                aaa = aaa.Substring(0, 2);
                if (ass.ToString().Equals("200") || aaa.ToString() == "11")
                {

                    decimal xxxx = decimal.Parse(rrr[0]["未完成数量"].ToString()) - Convert.ToDecimal(r["出库数量"]);
                    if (xxxx == 0)
                    {
                        rrr[0]["明细完成"] = true;
                        rrr[0]["明细完成日期"] = ads;
                        rrr[0]["完成数量"] = decimal.Parse(rrr[0]["完成数量"].ToString()) + Convert.ToDecimal(r["出库数量"]);
                        rrr[0]["未完成数量"] = xxxx;
                    }



                    r["已出库数量"] = r["未出库数量"];
                    r["未出库数量"] = 0;
                    r["完成"] = true;
                    r["完成日期"] = ads;
                    r["生效"] = true;
                    r["生效日期"] = ads;
                    DataRow dr_stockOutDetaail = dt_成品出库单明细表.NewRow();
                    dt_成品出库单明细表.Rows.Add(dr_stockOutDetaail);
                    dr_stockOutDetaail["GUID"] = System.Guid.NewGuid();
                    dr_stockOutDetaail["成品出库单号"] = s_成品出库单号;
                    dr_stockOutDetaail["POS"] = k++;
                    dr_stockOutDetaail["成品出库单明细号"] = s_成品出库单号 + "-" + k.ToString("00");
                    string s_销售订单明细号 = r["销售订单明细号"].ToString();
                    nameStrArray = s_销售订单明细号.Split('-');
                    dr_stockOutDetaail["销售订单号"] = nameStrArray[0].ToString();
                    dr_stockOutDetaail["销售订单明细号"] = r["销售订单明细号"];
                    dr_stockOutDetaail["出库通知单号"] = r["出库通知单号"];
                    dr_stockOutDetaail["出库通知单明细号"] = r["出库通知单明细号"];
                    dr_stockOutDetaail["物料编码"] = r["物料编码"];
                    dr_stockOutDetaail["物料名称"] = r["物料名称"];
                    dr_stockOutDetaail["出库数量"] = r["出库数量"];
                    dr_stockOutDetaail["已出库数量"] = r["出库数量"];
                    dr_stockOutDetaail["未开票数量"] = r["出库数量"];
                    dr_stockOutDetaail["规格型号"] = r["规格型号"];
                    //             dtM

                    dr_stockOutDetaail["客户"] = dtM.Rows[0]["客户名"];
                    dr_stockOutDetaail["客户编号"] = dtM.Rows[0]["客户编号"];
                    string s_xs = string.Format("select * from 销售记录销售订单明细表 where 销售订单明细号='{0}'", r["销售订单明细号"].ToString());
                    DataRow dr_xs = CZMaster.MasterSQL.Get_DataRow(s_xs, strconn);
                    if (dr_xs != null)
                    {
                        dr_stockOutDetaail["仓库号"] = dr_xs["仓库号"];
                        dr_stockOutDetaail["仓库名称"] = dr_xs["仓库名称"];
                    }
                    dr_stockOutDetaail["生效"] = true;
                    dr_stockOutDetaail["生效日期"] = ads;
                    //dr_成品出库明细["n原ERP规格型号"] = dr["n原ERP规格型号"];
                    //k++;
                }
            }



            if (k == dtP.Rows.Count)
            {

                foreach (DataRow dr in dtM.Rows)
                {
                    dr["完成"] = true;
                    dr["完成日期"] = ads;
                    dr["生效"] = true;
                    dr["生效日期"] = ads;
                }
            }
            int a = 0;
            foreach (DataRow dr in dt_销售mx.Rows)
            {
                if (bool.Parse(dr["明细完成"].ToString()) == true)
                {
                    a++;
                }

            }
            if (a == dt_销售mx.Rows.Count)
            {
                dt_销售主.Rows[0]["完成"] = true;
                dt_销售主.Rows[0]["完成日期"] = ads;

                foreach (DataRow dr in dt_销售mx.Rows)
                {
                    dr["总完成"] = true;

                    dr["总完成日期"] = ads;
                }

            }
            // dt_通知主.AcceptChanges();





        }












        #region 界面操作
        //新增
        /// <summary>
        /// 不用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataView dv = new DataView(dtM);
                dv.RowStateFilter = DataViewRowState.Added;
                if (dv.Count > 0)
                {
                    if (MessageBox.Show("当前有未保存的出库通知单，是否放弃保存？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        fun_清空();
                        dtP.Clear();
                        fun_载入明细(); fun_载入待办();
                    }
                }
                else
                {
                    fun_清空();
                    bl_新增or修改 = true;
                    dtP.Clear();
                    fun_载入明细();
                    fun_载入待办();
                    drM = dtM.NewRow();
                    dtM.Rows.Add(drM);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_check()
        {
            //if (comboBox1.Text == "")
            //{
            //    throw new Exception("送货方式为必填项");
            //}
            //else if (comboBox1.Text == "自送")
            //{
            //    if (textBox2.Text == "")
            //    {

            //        throw new Exception("车号未填写");
            //    }

            //}
            //else //外送
            //{

            //    if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString()=="")
            //    {

            //        throw new Exception("快递公司未选择");
            //    }
            //}
            string str = "";
            DataView dv_1 = new DataView(dt_待办);
            dv_1.RowFilter = "选择=1";
            if (dv_1.Count == 0) throw new Exception("未选择任何明细,请确认");
            foreach (DataRow dr in dv_1.ToTable().Rows)
            {
                if (str == "")
                    str = dr["销售订单号"].ToString();
                else
                {
                    if (str != dr["销售订单号"].ToString())
                    {
                        throw new Exception("选择了不同的销售单明细，请检查");
                    }
                }
            }
            string s_物料 = "";
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                if (dt_待办.Rows.Count > 0)
                {
                    s_物料 = dr["物料编码"].ToString().Substring(0, 3);
                    DataRow[] r = dt_待办.Select(string.Format("销售订单明细号='{0}'", dr["销售订单明细号"]));
                    if (Convert.ToDecimal(r[0]["未通知数量"]) < Convert.ToDecimal(dr["出库数量"]))
                    {
                        throw new Exception("通知数量超出订单上限,请确认");
                    }
                    if (Convert.ToDecimal(r[0]["未通知数量"]) - Convert.ToDecimal(r[0]["已通知未审"]) < Convert.ToDecimal(dr["出库数量"]))
                    {
                        throw new Exception("数量超出上限,请确认是否有未审核通知单");

                    }
                    if (s_物料 != "200" && s_物料 != "110")
                    {
                        if (dr["仓库号"].ToString() == "")
                        {
                            throw new Exception("仓库号没有选择");
                        }
                    }


                }
                if (Convert.ToDecimal(dr["出库数量"]) == 0)
                {
                    throw new Exception("数量不能为0！");
                }

            }
        }
        //保存
        /// <summary>
        /// 不用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                try
                {
                    gv.CloseEditor();
                    gc.BindingContext[dtP].EndCurrentEdit();
                    gv_待办.CloseEditor();
                    gc_待办.BindingContext[dt_待办].EndCurrentEdit();
                    fun_check();


                    foreach (DataRow dr in dtP.Rows)
                    {
                        if (Convert.ToDecimal(dr["出库数量"]) > Convert.ToDecimal(dr["仓库数量"]))
                        {
                            //if (MessageBox.Show("本次出库数量", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            //{
                            throw new Exception("本次通知数量以超过库存数量！");
                            //}
                        }
                    }

                    fun_保存主表();
                    fun_保存明细();

                    fun_事务_保存();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                fun_强载();
                fun_载入待办();
                //保存完变成修改状态
                bl_新增or修改 = false;
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "通知单详细界面_保存");
                MessageBox.Show("保存失败");
            }
        }
        //生效
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {

                gv.CloseEditor();
                gc.BindingContext[dtP].EndCurrentEdit();
                gv_待办.CloseEditor();
                gc_待办.BindingContext[dt_待办].EndCurrentEdit();

                fun_check();

                //foreach (DataRow dr in dtP.Rows)
                //{
                //    if (Convert.ToDecimal(dr["出库数量"]) > Convert.ToDecimal(dr["仓库数量"]))
                //    {
                //        //if (MessageBox.Show("本次出库数量", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                //        //{
                //        throw new Exception("本次通知数量已超过库存数量！");
                //        //}
                //    }
                //}


                fun_生效();

                dt_审核.Clear();


                MessageBox.Show("提交成功！");
                barLargeButtonItem3.Enabled = false;

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "出库通知单_生效");
                MessageBox.Show(string.Format(ex.Message + "提交失败！"));
            }
        }
        //关闭
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc, new Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 物料明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            //  ERPStock.frm仓库物料数量明细 frm = new ERPStock.frm仓库物料数量明细(dr["物料编码"].ToString(),);
            // CPublic.UIcontrol.AddNewPage(frm, "物料明细");
        }

        private void gv_待办_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (gv_待办.GetRow(e.RowHandle) == null)
                {
                    return;
                }
                int j = gv_待办.RowCount;
                for (int i = 0; i < j; i++)
                {
                    if (Convert.ToDecimal(gv_待办.GetRowCellValue(e.RowHandle, "库存数量")) >= Convert.ToDecimal(gv_待办.GetRowCellValue(e.RowHandle, "未通知数量")))
                    {
                        e.Appearance.BackColor = Color.LightBlue;
                        e.Appearance.BackColor2 = Color.LightBlue;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txt_操作员ID.Text = CPublic.Var.LocalUserID;
                txt_操作员.Text = CPublic.Var.localUserName;
                txt_出库日期.EditValue = CPublic.Var.getDatetime();

                fun_载入空主表();
                string a = "";
                if (!bl_新增or修改)
                    a = drM["销售订单号"].ToString();
                fun_载入待办(a);
                //fun_载入明细();
                fun_客户编号();
                fun_load();
                //fun_载入待办();
                searchLookUpEdit1.Properties.DataSource = dt_客户信息;
                searchLookUpEdit1.Properties.DisplayMember = "客户编号";
                searchLookUpEdit1.Properties.ValueMember = "客户编号";
                fun_清空();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            //行号设置 
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gv_待办_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            //行号设置 
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gv_待办_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv_待办.GetFocusedRowCellValue(gv_待办.FocusedColumn));
                e.Handled = true;
            }
        }

        private void gv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv.GetFocusedRowCellValue(gv.FocusedColumn));
                e.Handled = true;
            }
        }

        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString("00");
            }

        }

        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (txt_出库通知单号.Text != "")
            {
                bl_新增or修改 = true;
                fun_载入空主表();
            }
            fun_清空();
            DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
            if (dr == null) return;
            fun_载入待办(dr["销售订单号"].ToString());

            string sql = string.Format(@"select stcmx.*,原ERP物料编号,isnull(kc.库存总数,0)as 仓库数量 
                 from 销售记录销售出库通知单明细表 stcmx,仓库物料数量表 kc,基础数据物料信息表 base
                 where kc.物料编码=stcmx.物料编码 and base.物料编码=stcmx.物料编码 and 1=0");
            dtP = new DataTable();
            dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            string sql1 = string.Format(@"select * from 客户基础信息表 where 客户编号 = '{0}' and  地址 <> ''", dr["客户编号"].ToString());
            DataTable dt_地址 = CZMaster.MasterSQL.Get_DataTable(sql1, strconn);
            if (dt_地址.Rows.Count > 0)
            {
                textBox4.Text = dt_地址.Rows[0]["地址"].ToString();
            }
            gc.DataSource = dtP;
            textBox1.Text = dr["客户订单号"].ToString();
            textBox5.Text = dr["联系人"].ToString();
            txt_客户编号.Text = dr["客户编号"].ToString();
            txt_客户名.Text = dr["客户名"].ToString();
            textBox6.Text = dr["地址"].ToString();
            txt_备注.Text = dr["销售备注"].ToString();
            checkBox1.Checked = false;

        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox1.CheckState == CheckState.Checked && dt_待办.Rows.Count > 0)
            {
                foreach (DataRow dr in dt_待办.Rows)
                {
                    dr["选择"] = true;
                    gv_待办.FocusedRowHandle = gv_待办.LocateByDisplayText(0, gridColumn8, dr["销售订单明细号"].ToString());
                    repositoryItemCheckEdit1_CheckedChanged(null, null);

                }
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "自送")
            {
                label10.Text = "车号";
                textBox2.Visible = true;
                searchLookUpEdit2.Visible = false;
            }
            else
            {
                label10.Text = "物流公司";
                textBox2.Visible = false;
                searchLookUpEdit2.Visible = true;
                searchLookUpEdit2.Location = new System.Drawing.Point(73, 100);


            }
        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认作废此单据？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string sql = string.Format("select * from  销售记录销售出库通知单主表 where 出库通知单号 = '{0}'", txt_出库通知单号.Text);
                    DataTable dtt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dtt.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(dtt.Rows[0]["审核"]))
                        {
                            throw new Exception("当前单据状态已更改，请确认");
                        }
                        string sql_1 = string.Format("select * from 销售记录销售出库通知单明细表 where 出库通知单号 = '{0}'", txt_出库通知单号.Text);
                        DataTable dtt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
                        DateTime t = CPublic.Var.getDatetime();
                        dtt.Rows[0]["作废"] = true;
                        dtt.Rows[0]["作废人"] = CPublic.Var.localUserName;
                        dtt.Rows[0]["作废日期"] = t;
                        foreach (DataRow dr in dtt_1.Rows)
                        {
                            dr["作废"] = true;
                            dr["作废人"] = CPublic.Var.localUserName;
                            dr["作废时间"] = t;
                        }
                        string sql_2 = string.Format("select * from  单据审核申请表 where 关联单号 = '{0}'", txt_出库通知单号.Text);
                        DataTable dtt_2 = CZMaster.MasterSQL.Get_DataTable(sql_2, strconn);
                        if (dtt_2.Rows.Count > 0)
                        {
                            dtt_2.Rows[0]["作废"] = true;
                        }

                        SqlConnection conn = new SqlConnection(strconn);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("作废");
                        try
                        {

                            string sq212l = "select * from 销售记录销售出库通知单主表 where 1<>1";
                            SqlCommand cmd4 = new SqlCommand(sq212l, conn, ts);
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd4))
                            {
                                new SqlCommandBuilder(da);
                                da.Update(dtt);
                            }

                            string sql7 = "select * from 销售记录销售出库通知单明细表 where 1<>1";
                            SqlCommand cmd7 = new SqlCommand(sql7, conn, ts);
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd7))
                            {
                                new SqlCommandBuilder(da);
                                da.Update(dtt_1);
                            }


                            string sql8 = "select * from 单据审核申请表 where 1<>1";
                            SqlCommand cmd8 = new SqlCommand(sql8, conn, ts);
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd8))
                            {
                                new SqlCommandBuilder(da);
                                da.Update(dtt_2);
                            }

                            ts.Commit();
                            MessageBox.Show("作废成功");
                            barLargeButtonItem5_ItemClick(null, null);
                        }
                        catch (Exception ex)
                        {
                            ts.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                送货单样式预览 f1 = new 送货单样式预览();
                f1.ShowDialog();
                if (送货单样式预览.dd != null)
                {
                    textBox7.Text = 送货单样式预览.dd.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (e.Column.FieldName == "仓库号")
                {
                    dr["仓库号"] = e.Value;
                    DataRow[] ds = dt_仓库.Select(string.Format("仓库号 = '{0}'", dr["仓库号"]));
                    dr["仓库名称"] = ds[0]["仓库名称"];
                    //dr["仓库名称"] = sr["仓库名称"].ToString();
                    string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_物料数量 = new DataTable();
                    da.Fill(dt_物料数量);
                    if (dt_物料数量.Rows.Count == 0)
                    {
                        dr["仓库数量"] = 0;
                        // dr["有效总数"] = 0;
                    }
                    else
                    {
                        dr["仓库数量"] = dt_物料数量.Rows[0]["库存总数"];
                        //  dr["有效总数"] = dt_物料数量.Rows[0]["有效总数"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
