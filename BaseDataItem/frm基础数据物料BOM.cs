using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
namespace BaseData
{
    public partial class frm基础数据物料BOM : UserControl
    {
        #region 成员
        public string str_物料编码 = "";  //物料编码
        public string str_物料名称 = "";  //物料编码
        public string str_规格 = "";  //物料编码
        public string str_原规格型号 = "";

        string strconn = CPublic.Var.strConn;
        DataTable dtM = null;
        public DataTable dt_物料名称;
        DataTable dt_子项类型;
        DataTable dt_BOM类型;
        DataTable dt_右键查询;
        DataTable dt;    //dt存储 check专用
        DataTable dtM1;//存储BOM版本
        string str_成品名称 = "";
        DataTable dt_BOM修改;
        DataTable dt_包装;
        DataTable dt_版本;
        bool bo_判断行数新增或者删除 = false;
        /// <summary>
        /// 主窗体中的text中的值
        /// </summary>
        DataTable dt_仓库;
        string txt_成品编码;
        string show;
        DataTable dt_unit; //计量单位
        string cfgfilepath = "";
        public static DevExpress.XtraTab.XtraTabControl XTC;

        public static TextBox textBox1;

        #endregion

        #region 自用类
        public frm基础数据物料BOM()
        {
            InitializeComponent();

        }

        public frm基础数据物料BOM(DataRow dr)
        {
            InitializeComponent();
            //DataRow drM = dr;
            //dataBindHelper1.DataFormDR(drM);

        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            fun_清空();
            //BOM修改记录加载
            // fun_加载BOM修改表数据();
            fun_载入物料和子项类型();
            string sql = string.Format("select * from 基础数据BOM信息修改记录表 where 1<>1");
            dt_BOM修改 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_BOM修改);
            frm基础物料数据信息.aaaa.FM2.Add(this);

            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            else
            {
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(panel4, this.Name, cfgfilepath);
            }
        }
        void dt_BOM修改记录_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            dt_BOM修改记录.ColumnChanged -= dt_BOM修改记录_ColumnChanged;
            if (e.Column.Caption == "子项编码")
            {
                if (dt_BOM修改记录.Select(string.Format("子项编码 = '{0}'", e.Row["子项编码"].ToString())).Length > 0)
                {
                    MessageBox.Show("BOM结构中已有此项，请重新选择");
                }
                //else
                //{
                //    string ss = e.Row["子项编码"].ToString();
                //    DataRow[] ds = dt_物料名称.Select(string.Format("子项编码 = '{0}'", ss));
                //    e.Row["子项名称"] = ds[0]["子项名称"].ToString();
                //    e.Row["物料编码"] = ds[0]["物料编码"].ToString();
                //    e.Row["计量单位"] = ds[0]["计量单位"].ToString();
                //    e.Row["计量单位编码"] = ds[0]["计量单位编码"].ToString();
                //    e.Row["图纸编号"] = ds[0]["图纸编号"].ToString();
                //    e.Row["货架描述"] = ds[0]["货架描述"].ToString();
                //}
            }
            else if (e.Column.Caption == "包装数量" || e.Column.Caption == "总装数量")
            {

                if (e.Row["包装数量"] == DBNull.Value)
                    e.Row["包装数量"] = 0;
                if (e.Row["总装数量"] == DBNull.Value)
                    e.Row["总装数量"] = 0;
                e.Row["数量"] = Convert.ToDecimal(e.Row["包装数量"]) + Convert.ToDecimal(e.Row["总装数量"]);
            }
            else if (e.Column.Caption == "计量单位编码")
            {
                if (e.Row["计量单位编码"] == DBNull.Value)
                    e.Row["计量单位"] = "";
                else
                {
                    e.Row["计量单位"] = dt_unit.Select(string.Format("计量单位编码='{0}'", e.Row["计量单位编码"]))[0]["计量单位"];
                }


            }
            dt_BOM修改记录.ColumnChanged += dt_BOM修改记录_ColumnChanged;
        }

        #endregion

        #region 方法
        /// <summary>
        /// 给下拉框载入数据
        /// </summary>
        private void fun_载入物料和子项类型()
        {
             
                string sql = @" select (a.物料编码) as 子项编码,(a.物料名称) as 子项名称,a.规格型号,b.仓库号,b.仓库名称,b.货架描述,b.库存总数,大类,小类,a.物料属性,a.图纸编号,a.计量单位编码,a.计量单位,虚拟件
                ,a.仓库号 as 默认仓库号,a.仓库名称 as  默认仓库,自制,可购  from 基础数据物料信息表 a   left join 仓库物料数量表 b on a.物料编码=b.物料编码 
                where  停用=0 and a.仓库号 not in ('20','09') and  a.物料名称 not like '%劳务%' ";//where  停用 = 0 物料类型 = '原材料' or 物料类型 = '半成品' and
             //20  固定资产仓  09  软件仓  
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                dt_物料名称 = new DataTable();
                da.Fill(dt_物料名称);

                dt_仓库 = new DataTable();
                string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别' and 布尔字段4 = 1";
                da = new SqlDataAdapter(sql4, strconn);
                da.Fill(dt_仓库);
                repositoryItemGridLookUpEdit1.DataSource = dt_仓库;
                repositoryItemGridLookUpEdit1.DisplayMember = "仓库号";
                repositoryItemGridLookUpEdit1.ValueMember = "仓库号";

                sql = "select 属性值 from 基础数据基础属性表 where 属性类别 = 'BOM子项类型'";
                da = new SqlDataAdapter(sql, strconn);
                dt_子项类型 = new DataTable();
                da.Fill(dt_子项类型);

                sql = "select 属性值 from 基础数据基础属性表 where 属性类别 = 'BOM类型'";
                da = new SqlDataAdapter(sql, strconn);
                dt_BOM类型 = new DataTable();
                da.Fill(dt_BOM类型);
                sql = "select 属性值 from 基础数据基础属性表 where 属性类别 = '主辅料'";
                da = new SqlDataAdapter(sql, strconn);
                DataTable dt_主辅料 = new DataTable();
                da.Fill(dt_主辅料);
                repositoryItemComboBox3.Items.Clear();
                foreach (DataRow dr in dt_主辅料.Rows)
                {
                    repositoryItemComboBox3.Items.Add(dr["属性值"].ToString());
                }
                repositoryItemSearchLookUpEdit5.PopupFormSize = new Size(1400, 400);
                repositoryItemSearchLookUpEdit5.DataSource = dt_物料名称;
                repositoryItemSearchLookUpEdit5.DisplayMember = "子项编码";
                repositoryItemSearchLookUpEdit5.ValueMember = "子项编码";
                repositoryItemSearchLookUpEdit5.View.BestFitColumns();

                repositoryItemSearchLookUpEdit6.DataSource = dt_子项类型;
                repositoryItemSearchLookUpEdit6.DisplayMember = "属性值";
                repositoryItemSearchLookUpEdit6.ValueMember = "属性值";

                repositoryItemSearchLookUpEdit7.DataSource = dt_BOM类型;
                repositoryItemSearchLookUpEdit7.DisplayMember = "属性值";
                repositoryItemSearchLookUpEdit7.ValueMember = "属性值";
                sql = "select 属性值 as 计量单位,属性字段1 as 计量单位编码 from 基础数据基础属性表 where 属性类别 = '计量单位'";
                da = new SqlDataAdapter(sql, strconn);
                dt_unit = new DataTable();
                da.Fill(dt_unit);
                repositoryItemSearchLookUpEdit8.DataSource = dt_unit;
                repositoryItemSearchLookUpEdit8.DisplayMember = "计量单位编码";
                repositoryItemSearchLookUpEdit8.ValueMember = "计量单位编码";

                sql = "select 属性值 as 领料类型 from 基础数据基础属性表 where 属性类别 = 'WIPType'";
                DataTable dt_领料类型 = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_领料类型);

                repositoryItemSearchLookUpEdit1.DataSource = dt_领料类型;
                repositoryItemSearchLookUpEdit1.DisplayMember = "领料类型";
                repositoryItemSearchLookUpEdit1.ValueMember = "领料类型";

            

        }

        public void fun_载入数据()
        {
            try
            {
                fun_加载BOM修改表数据();

                if (dtM != null)
                {
                    dtM.Clear();
                }
                txt_成品编码 = str_物料编码;
                dtM = new DataTable();
                dtM1 = new DataTable();
                //  left  join 仓库物料数量表 kc on kc.物料编码=b.物料编码   ,kc.货架描述 kc.仓库号=a.仓库号 and
                string sql = string.Format(@"select 产品编码,子项编码,BOM版本号,BOM版本描述,产品名称,子项名称,b.物料名称 as 子项名称r,[数量],[子项类型],b.规格型号,
                a.[主辅料],[用途],[修改人员],[修改人员ID],a.计量单位编码,a.[计量单位],a.[修改日期],[BOM类型],[物料替换],对应虚拟件编号,xn.物料名称 as 虚拟件名称
                ,[替换日期],[替换人ID],[替换人],[总装数量],[包装数量],[A面位号],B面位号,关键子项,[组],[优先级],b.图纸编号,a.仓库号,a.仓库名称,a.子件损耗率,a.WIPType from 基础数据物料BOM表 a
                left join 基础数据物料信息表 b on a.子项编码 = b.物料编码 
               
                left  join 基础数据物料信息表 xn on a.对应虚拟件编号 = xn.物料编码 
                where    a.产品编码 = '{0}' ", txt_成品编码);
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                dtM.Columns.Add("选择", typeof(Boolean));
                da.Fill(dtM);
                da.Fill(dtM1);
                //dtM1 = dtM.Copy();
                gc.DataSource = dtM;

                gv.ViewCaption = string.Format("物料：{0}-{1}-{2}-{3}的BOM信息", str_规格, str_物料编码, str_物料名称, str_规格);
                //}
                sql = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", str_物料编码);
                DataTable dt111 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (dt111.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(dt111.Rows[0]["BOM确认"]) == true)
                    {

                        label2.Text = "BOM已确认";
                    }
                    else if (Convert.ToBoolean(dt111.Rows[0]["BOM确认"]) == false)
                    {

                        label2.Text = "BOM未确认";
                    }
                }
                fun_();
                //fun_虚拟件判断();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void fun_虚拟件判断()
        {
            try
            {
                //加载该物料是否是虚拟件物料，是，可替代料，领料类型不一样
                string stp = string.Format("select 虚拟件 from 基础数据物料信息表 where 物料编码='{0}'", txt_成品编码);
                using (SqlDataAdapter da1 = new SqlDataAdapter(stp, strconn))
                {
                    DataTable dt_判断是否虚拟件 = new DataTable();
                    da1.Fill(dt_判断是否虚拟件);
                    if (dt_判断是否虚拟件.Rows.Count != 0)
                    {
                        if (dt_判断是否虚拟件.Rows[0]["虚拟件"].Equals(true))
                        {
                            gvv1.Columns["优先级"].OptionsColumn.AllowEdit = false;
                            gvv1.Columns["组"].OptionsColumn.AllowEdit = false;
                        }
                        else if (gvv1.Columns["优先级"] != null)
                        {

                            gvv1.Columns["优先级"].OptionsColumn.AllowEdit = true;
                            gvv1.Columns["组"].OptionsColumn.AllowEdit = true;

                        }
                    }

                }
            }
            catch
            {


            }

        }
        private void fun_BOM版本()
        {
            using (SqlDataAdapter da = new SqlDataAdapter("select * from [基础物料BOM版本表] where 1<>1", CPublic.Var.strConn))
            {
                dt_版本 = new DataTable();
                da.Fill(dt_版本);
                if (dtM1.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtM1.Rows)
                    {
                        DataRow rr = dt_版本.NewRow();
                        dt_版本.Rows.Add(rr);

                        if (dr["BOM版本号"].ToString() == "" || dr["BOM版本号"] == null)
                        {
                            rr["BOM版本号"] = "0";
                        }
                        else
                        {
                            rr["BOM版本号"] = Convert.ToInt32(dr["BOM版本号"]);
                        }
                        rr["物料编码"] = dr["产品编码"].ToString();
                        rr["子项编码"] = dr["子项编码"].ToString();
                        rr["子项名称"] = dr["子项名称r"].ToString();
                        rr["图纸编号"] = dr["图纸编号"].ToString();
                        rr["总数量"] = dr["总装数量"].ToString();
                        rr["总装数量"] = dr["总装数量"].ToString();
                        rr["A面位号"] = dr["A面位号"].ToString();
                        rr["B面位号"] = dr["B面位号"].ToString();
                        rr["主辅料"] = dr["主辅料"].ToString();
                        rr["子项类型"] = dr["子项类型"].ToString();
                        rr["BOM类型"] = dr["BOM类型"].ToString();
                        rr["计量单位"] = dr["计量单位"].ToString();
                        rr["用途"] = dr["用途"].ToString();
                        rr["组"] = dr["组"].ToString();
                        rr["货架"] = dr["货架描述"].ToString();
                        rr["优先级"] = dr["优先级"].ToString();
                        // rr["关于子项"] = dr[""].ToString();
                        rr["修改人员"] = CPublic.Var.localUserName;
                        rr["修改人员ID"] = CPublic.Var.LocalUserID;
                        rr["修改日期"] = CPublic.Var.getDatetime();
                        rr["仓库号"] = dr["仓库号"].ToString();
                        rr["仓库名称"] = dr["仓库名称"].ToString();
                    }
                }
            }





        }
        private void fun_BOM表中的版本修改()
        {
            using (SqlDataAdapter da = new SqlDataAdapter("select * from 基础数据物料BOM表 where 1<>1", CPublic.Var.strConn))
            {

                if (dtM.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtM.Rows)
                    {
                        if (dr.RowState == DataRowState.Deleted) continue;
                        if (dr["BOM版本号"].ToString() == "" || dr["BOM版本号"] == null)
                        {
                            dr["BOM版本号"] = "1";
                        }
                        else
                        {
                            dr["BOM版本号"] = Convert.ToInt32(dr["BOM版本号"]) + 1;
                        }
                    }
                }
            }



        }

        private void fun_保存()
        {
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("BOM保存");

            try
            {
                string sql1 = "select * from 基础数据BOM信息修改记录表 where 1<>1";
                SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
                using (SqlDataAdapter da1 = new SqlDataAdapter(cmd1))
                {
                    new SqlCommandBuilder(da1);
                    da1.Update(dt_BOM修改);
                }

                string sql2 = "select * from 基础数据包装清单表 where 1<>1";
                SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);
                using (SqlDataAdapter da2 = new SqlDataAdapter(cmd2))
                {
                    new SqlCommandBuilder(da2);
                    da2.Update(dt_包装);
                }

                string sql3 = "select * from 基础数据物料BOM表 where 1<>1";
                SqlCommand cmd3 = new SqlCommand(sql3, conn, ts);
                using (SqlDataAdapter da3 = new SqlDataAdapter(cmd3))
                {
                    new SqlCommandBuilder(da3);
                    da3.Update(dtM);
                }

                //string sql4 = "select * from 基础物料BOM版本表 where 1<>1";
                //SqlCommand cmd4 = new SqlCommand(sql4, conn, ts);
                //using (SqlDataAdapter da4 = new SqlDataAdapter(cmd4))
                //{
                //    new SqlCommandBuilder(da4);
                //    da4.Update(dt_版本);
                //}
                ts.Commit();
                show = "保存成功！";
            }
            catch (Exception ex)
            {
                ts.Rollback();
                show = ex.Message;
            }
        }
        private void fun_check()
        {
            if (txt_修改原因.Text == "")
            {
                throw new Exception("请先填写修改原因再进行保存！");
            }
            //19-10-28 
            DataTable dt = new DataTable();
            Random rd = new Random();
            int rand = rd.Next(0, 100);
            try
            {
                //              string s = string.Format(@"with temp_bom(产品编码, 子项编码, 仓库号, 仓库名称, wiptype, 子项类型, 数量, bom类型, bom_level ) as
                //       (select 产品编码, 子项编码, 仓库号, 仓库名称, WIPType, 子项类型, 数量, bom类型,1 as level from 基础数据物料BOM表
                //         where 子项编码 = '{0}'
                //         union all
                // select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型,a.数量,a.bom类型,b.bom_level + 1  from 基础数据物料BOM表 a
                //   inner join temp_bom b on a.子项编码 = b.产品编码   ) 
                //        select 子项编码, 子项名称, 产品编码,  产品名称,wiptype,子项类型,数量,bom类型, 仓库号, 仓库名称
                //, bom_level, 子项规格,停用  from (
                //select 产品编码 as 子项编码,fx.物料名称 as 子项名称,子项编码 as 产品编码,base.物料名称 as 产品名称,wiptype,子项类型,数量,bom类型,a.仓库号,a.仓库名称
                //, bom_level,fx.规格型号 as 子项规格,base.停用 from temp_bom a
                //left  join 基础数据物料信息表 base on base.物料编码 = a.子项编码
                //   left  join 基础数据物料信息表 fx  on fx.物料编码 = a.产品编码  )dd  
                //   group by 子项编码, 子项名称, 产品编码,  产品名称,wiptype,子项类型,数量,bom类型, 仓库号, 仓库名称, bom_level, 子项规格,停用", str_物料编码);
                //              dt = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                //2020-4-16 
                string ss = $" select * into temp_维护bom{rand}  from(select  产品编码,子项编码 from 基础数据物料BOM表 where 产品编码<>'{str_物料编码}')x";
                CZMaster.MasterSQL.ExecuteSQL(ss, strconn);
                DataTable ttemp = new DataTable();
                ttemp.Columns.Add("产品编码");
                ttemp.Columns.Add("子项编码");
                foreach (DataRow r in dt_BOM修改记录.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    DataRow rr = ttemp.NewRow();
                    rr["产品编码"] = str_物料编码;
                    rr["子项编码"] = r["子项编码"];
                    ttemp.Rows.Add(rr);
                }
                SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(strconn, SqlBulkCopyOptions.UseInternalTransaction);
                sqlbulkcopy.DestinationTableName = $"temp_维护bom{rand}";//数据库中的表名  
                sqlbulkcopy.WriteToServer(ttemp);
                string s = $@" with temp_bom(产品编码, 子项编码 , bom_level ) as
         (select 产品编码, 子项编码, 1 as level from temp_维护bom{rand}
           where 子项编码 = '{str_物料编码}'
           union all
         select a.产品编码, a.子项编码, b.bom_level + 1  from temp_维护bom{rand} a
         inner   join temp_bom b on a.子项编码 = b.产品编码 ) 
             select  * from temp_bom   ";

                dt = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                s = $@"if exists (select count(*) from [sys].[schemas] S JOIN [sys].[tables] T ON S.schema_id = T.schema_id where S.name='temp_维护bom{rand}')
                 drop table  temp_维护bom{rand}";
                CZMaster.MasterSQL.ExecuteSQL(s, strconn);
            }
            catch (Exception)
            {
                string s = $@"if exists (select count(*) from [sys].[schemas] S JOIN [sys].[tables] T ON S.schema_id = T.schema_id where S.name='temp_维护bom{rand}')
                 drop table  temp_维护bom{rand}";
                CZMaster.MasterSQL.ExecuteSQL(s, strconn);
                throw new Exception("该bom中存在此产品的父项,会引发死循环,请确认");
            }

            foreach (DataRow dr in dt_BOM修改记录.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                if (dr["子项编码"].ToString() == str_物料编码) throw new Exception("维护的列表中存在该产品自身,请确认");
                DataRow[] temp = dt.Select(string.Format("子项编码='{0}'", dr["子项编码"]));
                //这里前面用子项编码 是因为上面是从父项树形结构那边取过来的 那边为了方便 直接用的子项树形结构的数据结构 本意是父项编码 但是字段名是 子项编码
                if (temp.Length > 0)
                {
                    throw new Exception(string.Format("物料{0}为 此产品的父项,会引发死循环,请确认", dr["子项编码"]));
                }
                if (dr["计量单位编码"].ToString() == "")
                {
                    throw new Exception("计量单位未选择");
                }
                if (dr["主辅料"].ToString() == "")
                {
                    throw new Exception("主辅料未选择");
                }
                if (dr["BOM类型"].ToString() == "")
                {
                    throw new Exception("BOM类型未选择");
                }
                if (dr["子项编码"].ToString() == str_物料编码)
                {
                    throw new Exception("不能将自身设为子项");
                }
                DataRow[] tr = dt_BOM修改记录.Select(string.Format("子项编码='{0}'", dr["子项编码"]));
                if (tr.Length > 1) throw new Exception(string.Format("子项:'{0}'重复", dr["子项编码"]));
                if (dr["仓库号"].ToString() == "")
                {
                    throw new Exception("仓库未选择");
                }
                decimal dec = 0;
                if (!decimal.TryParse(dr["总装数量"].ToString(), out dec))
                {
                    throw new Exception("总装数量输入有误,请检查");
                }
                if (dec <= 0) throw new Exception("总装数量需大于0");

                //2020-6-8  限制去掉 供应链确定
                //string str = string.Format("select 虚拟件 from 基础数据物料信息表 where 物料编码='{0}'", dr["子项编码"]);
                //using (SqlDataAdapter da = new SqlDataAdapter(str, strconn))
                //{
                //    DataTable dt_判断是否虚拟件 = new DataTable();
                //    da.Fill(dt_判断是否虚拟件);
                //    if (dt_判断是否虚拟件.Rows[0]["虚拟件"].Equals(true))
                //    {
                //        if (dr["WIPType"].ToString() != "虚拟")
                //        {

                //            throw new Exception(dr["子项编码"] + "该子项为虚拟件，请在领料类型中选择虚拟类型！");
                //            // throw new Exception(string.Format("'{0}'该子项为虚拟件，请在领料类型中选择虚拟类型",dr["子项编码"]));
                //        }
                //    }
                //}
 
            }

        }

        private void fun_()
        {
            string sql = string.Format("select * from 基础数据包装清单表 where 成品编码 = '{0}'", txt_成品编码);
            dt_包装 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_包装);
        }

        private void fun_检查并保存包装清单()
        {
            if (dt_包装.Rows.Count == 0)
            {
                foreach (DataRow r in dtM.Rows)
                {
                    if (r.RowState == DataRowState.Added)
                    {
                        if (r["主辅料"] == null || r["主辅料"].ToString() == "")
                        {
                            throw new Exception("请先选择主辅料");
                        }
                    }
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (Convert.ToDecimal(r["包装数量"]) != 0)
                    {
                        //DataRow[] ds = dt_包装.Select(string.Format("物料编码 = '{0}'", r["子项编码"].ToString()));
                        DataRow dr = dt_包装.NewRow();
                        dt_包装.Rows.Add(dr);
                        dr["GUID"] = System.Guid.NewGuid();
                        dr["成品编码"] = txt_成品编码;
                        dr["成品名称"] = str_成品名称;
                        dr["物料编码"] = r["子项编码"];
                        dr["物料名称"] = r["子项名称"];
                        dr["数量"] = r["包装数量"];
                        string s = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", r["子项编码"].ToString().Trim());
                        SqlDataAdapter a = new SqlDataAdapter(s, strconn);
                        DataTable t = new DataTable();
                        a.Fill(t);
                        dr["大类"] = t.Rows[0]["大类"];
                        dr["小类"] = t.Rows[0]["小类"];
                        dr["规格型号"] = t.Rows[0]["规格型号"];
                        dr["图纸编号"] = t.Rows[0]["图纸编号"];
                    }
                }
            }
            else
            {
                foreach (DataRow r in dtM.Rows)
                {
                    if (r.RowState == DataRowState.Added)
                    {
                        if (r["主辅料"] == null || r["主辅料"].ToString() == "")
                        {
                            throw new Exception("请先选择主辅料");
                        }
                    }
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (Convert.ToDecimal(r["包装数量"]) != 0)
                    {
                        DataRow[] ds = dt_包装.Select(string.Format("物料编码 = '{0}'", r["子项编码"].ToString()));
                        if (ds.Length == 0)
                        {
                            DataRow dr = dt_包装.NewRow();
                            dt_包装.Rows.Add(dr);
                            dr["GUID"] = System.Guid.NewGuid();
                            dr["成品编码"] = txt_成品编码;
                            dr["成品名称"] = str_成品名称;
                            dr["物料编码"] = r["子项编码"];
                            dr["物料名称"] = r["子项名称"];
                            dr["数量"] = r["包装数量"];
                            string s = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", r["子项编码"].ToString().Trim());
                            SqlDataAdapter a = new SqlDataAdapter(s, strconn);
                            DataTable t = new DataTable();
                            a.Fill(t);
                            dr["大类"] = t.Rows[0]["大类"];
                            dr["小类"] = t.Rows[0]["小类"];
                            dr["规格型号"] = t.Rows[0]["规格型号"];
                            dr["图纸编号"] = t.Rows[0]["图纸编号"];
                        }
                        else
                        {
                            ds[0]["数量"] = r["包装数量"];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// str :子项编码 , strr :产品编码
        /// </summary>
        private void fun_check(string str_子项, string str_成品)
        {
            if (str_子项 == str_成品)
            {
                throw new Exception("子项不能为物料本身");
            }

            dt = new DataTable();
            string sql = string.Format("select * from 基础数据物料BOM表 where 产品编码 = '{0}'", str_成品);
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                int count = 0;
                foreach (DataRow r in dt.Rows)
                {
                    if (r["子项编码"].ToString() == str_子项)
                    {
                        count++;
                        if (count > 1) throw new Exception("出现重复项，保存失败，请检查");
                    }
                    else
                    {
                        fun_check(str_成品, r["子项编码"].ToString());
                    }
                }
            }
        }

        private void fun_清空()
        {
            string sql = "select  * from 基础数据物料BOM表 where 1<>1";
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            gc.DataSource = dtM;
            fun_载入数据();

        }
        #endregion

        #region 界面操作

        ////原先保存
        //private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    try
        //    {
        //        //判断dtM 是否被修改
        //        //if (dtM.GetChanges(System.Data.DataRowState.Modified) == null)
        //        //{
        //        //    if (bo_判断行数新增或者删除 == false)
        //        //    {
        //        //        throw new Exception("操作界面没做任何修改不可以保存");
        //        //    }
        //        //}



        //        //保存
        //        gv.CloseEditor();
        //        this.BindingContext[dtM].EndCurrentEdit();

        //        if (dtM.Rows.Count == 0) { }
        //        else
        //        {
        //            fun_check();
        //            //记录修改人信息
        //            string sql = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", txt_成品编码);
        //            DataTable tr = new DataTable();
        //            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
        //            da.Fill(tr);

        //            str_成品名称 = tr.Rows[0]["物料名称"].ToString();

        //            bool bl = false;
        //            DateTime t = CPublic.Var.getDatetime();
        //            foreach (DataRow dr in dtM.Rows)
        //            {
        //                //if (dr.RowState == DataRowState.Unchanged)
        //                //{
        //                //    throw new Exception("操作界面没做任何修改不可以保存");
        //                //}


        //                if (dr.RowState == DataRowState.Deleted)
        //                {
        //                    string str_原物料 = dr["子项编码", DataRowVersion.Original].ToString();
        //                    //string str_后物料 = dr["子项编码", DataRowVersion.Current].ToString();

        //                    DataRow rr = dt_BOM修改.NewRow();
        //                    rr["修改人"] = CPublic.Var.localUserName;
        //                    rr["修改人ID"] = CPublic.Var.LocalUserID;
        //                    rr["修改日期"] = t;
        //                    rr["修改原因"] = txt_修改原因.Text;
        //                    rr["成品编码"] = txt_成品编码;
        //                    rr["成品名称"] = str_成品名称;
        //                    rr["修改属性"] = "删除";
        //                    rr["更改前物料"] = str_原物料;
        //                    rr["更改前数量"] = dr["数量", DataRowVersion.Original].ToString();
        //                    dt_BOM修改.Rows.Add(rr);
        //                    continue;
        //                }

        //                if (dr.RowState == DataRowState.Modified)
        //                {
        //                    string str_原物料 = dr["子项编码", DataRowVersion.Original].ToString();
        //                    string str_后物料 = dr["子项编码", DataRowVersion.Current].ToString();

        //                    DataRow rr = dt_BOM修改.NewRow();
        //                    rr["修改人"] = CPublic.Var.localUserName;
        //                    rr["修改人ID"] = CPublic.Var.LocalUserID;
        //                    rr["修改日期"] = t;
        //                    rr["修改原因"] = txt_修改原因.Text;
        //                    rr["成品编码"] = txt_成品编码;
        //                    rr["成品名称"] = str_成品名称;
        //                    rr["修改属性"] = "修改";
        //                    rr["更改前物料"] = str_原物料;
        //                    rr["更改后物料"] = str_后物料;
        //                    rr["更改前数量"] = dr["数量", DataRowVersion.Original].ToString();
        //                    rr["更改后数量"] = dr["数量", DataRowVersion.Current].ToString();
        //                    dt_BOM修改.Rows.Add(rr);

        //                    dr["修改人员ID"] = CPublic.Var.LocalUserID;
        //                    dr["修改人员"] = CPublic.Var.localUserName;
        //                    dr["修改日期"] = t;
        //                }
        //                if (dr.RowState == DataRowState.Added)
        //                {

        //                    string str_后物料 = dr["子项编码", DataRowVersion.Current].ToString();
        //                    DataRow rr = dt_BOM修改.NewRow();
        //                    rr["修改人"] = CPublic.Var.localUserName;
        //                    rr["修改人ID"] = CPublic.Var.LocalUserID;
        //                    rr["修改日期"] = CPublic.Var.getDatetime();
        //                    rr["修改原因"] = txt_修改原因.Text;
        //                    rr["成品编码"] = txt_成品编码;
        //                    rr["成品名称"] = str_成品名称;
        //                    rr["修改属性"] = "增加";
        //                    rr["更改后物料"] = str_后物料;
        //                    rr["更改后数量"] = dr["数量", DataRowVersion.Current].ToString();

        //                    dt_BOM修改.Rows.Add(rr);
        //                    dr["修改人员ID"] = CPublic.Var.LocalUserID;
        //                    dr["修改人员"] = CPublic.Var.localUserName;
        //                    dr["修改日期"] = t;

        //                }
        //                dr["产品编码"] = txt_成品编码;
        //                dr["产品名称"] = tr.Rows[0]["物料名称"].ToString();
        //                if (dr["BOM版本号"] == null || dr["BOM版本号"].ToString() == "")
        //                {
        //                    dr["BOM版本号"] = "";
        //                }
        //                if (dr["BOM版本描述"] == null || dr["BOM版本描述"].ToString() == "")
        //                {
        //                    dr["BOM版本描述"] = "";
        //                }
        //                //if (dr["子项类型"] == null || dr["子项类型"].ToString() == "")
        //                //{
        //                //    throw new Exception("请选择子项类型");
        //                //}
        //                if (dr["用途"] == null || dr["用途"].ToString() == "")
        //                {
        //                    dr["用途"] = "";
        //                }
        //                if (dr["物料替换"] == null || dr["物料替换"].ToString() == "")
        //                {
        //                    dr["物料替换"] = "";
        //                }
        //                //if (dr["子项类型"] == null || dr["子项类型"].ToString() == "")
        //                //{
        //                //    dr["子项类型"] = "";
        //                //}
        //                //if (dr["子项类型"] == null || dr["子项类型"].ToString() == "")
        //                //{
        //                //    dr["子项类型"] = "";
        //                //}
        //            }

        //            //保存
        //            try
        //            {
        //                fun_检查并保存包装清单();
        //                //fun_保存修改原因();

        //                for (int i = 0; i < dtM.Rows.Count; i++)
        //                {
        //                    if (dtM.Rows[i].RowState == DataRowState.Deleted)
        //                    {
        //                        continue;
        //                    }
        //                    if (dtM.Rows[i]["子项编码"].ToString().IndexOf("└") != -1 || dtM.Rows[i]["子项编码"].ToString().IndexOf("├") != -1)
        //                    {
        //                        dtM.Rows[i].Delete();
        //                    }
        //                }
        //                foreach (DataRow dr in dtM.Rows)
        //                {
        //                    if (dr.RowState == DataRowState.Deleted)
        //                    {
        //                        continue;
        //                    }
        //                    fun_check(dr["子项编码"].ToString().Trim(), txt_成品编码);
        //                    if (!bl && dr["组"].ToString().Trim() != "")
        //                    {

        //                        DataRow[] r = dtM.Select(string.Format("组='{0}'", dr["组"]));
        //                        if (r.Length > 1)
        //                        {
        //                            bl = true;
        //                        }
        //                    }
        //                }
        //                fun_BOM版本();//boM版本更新
        //                fun_BOM表中的版本修改();
        //                fun_保存();//事务保存
        //                fun_载入数据();
        //                MessageBox.Show(show);
        //                bo_判断行数新增或者删除 = false;
        //                //保存信息到 基础数据物料信息表 中
        //                if (bl)
        //                {
        //                    tr.Rows[0]["BOM有无备料"] = true;
        //                }
        //                tr.Rows[0]["审核"] = "待审核";
        //                tr.Rows[0]["BOM修改人"] = CPublic.Var.localUserName;
        //                tr.Rows[0]["BOM修改人ID"] = CPublic.Var.LocalUserID;
        //                tr.Rows[0]["BOM修改日期"] = t;
        //                new SqlCommandBuilder(da);
        //                da.Update(tr);
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}
        #endregion

        #region 右键菜单


        private void chakan_Click(object sender, EventArgs e)
        {
            int j = gv.FocusedRowHandle;

            for (int i = 0; i < dt_右键查询.Rows.Count; i++)
            {
                DataRow r = dtM.NewRow();
                r["物料名称"] = dt_物料名称.Select(string.Format("物料编码 = '{0}'", dt_右键查询.Rows[i]["子项编码"].ToString()))[0]["物料名称"].ToString();
                if (i == dt_右键查询.Rows.Count - 1)
                {
                    string str = dt_右键查询.Rows[i]["子项编码"].ToString();
                    dt_右键查询.Rows[i]["子项编码"] = " └ " + str;
                }
                else
                {
                    string str = dt_右键查询.Rows[i]["子项编码"].ToString();
                    dt_右键查询.Rows[i]["子项编码"] = " ├ " + str;
                }
                r["子项编码"] = dt_右键查询.Rows[i]["子项编码"].ToString();
                r["子项类型"] = dt_右键查询.Rows[i]["子项类型"].ToString();
                r["BOM类型"] = dt_右键查询.Rows[i]["BOM类型"].ToString();
                r["数量"] = dt_右键查询.Rows[i]["数量"].ToString();
                dtM.Rows.InsertAt(r, j + 1);
                j++;
            }
        }

        private void guanbi_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dtM.Rows.Count; i++)
            {
                if (dtM.Rows[i]["子项编码"].ToString().IndexOf("└") != -1 || dtM.Rows[i]["子项编码"].ToString().IndexOf("├") != -1)
                {
                    dtM.Rows[i].Delete();
                }
            }
        }

        private void gv_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            if (dr["子项编码"].ToString().IndexOf("└") != -1 || dr["子项编码"].ToString().IndexOf("├") != -1)
            {
                gv.Columns[0].OptionsColumn.AllowEdit = false;
                gv.Columns[2].OptionsColumn.AllowEdit = false;
                gv.Columns[4].OptionsColumn.AllowEdit = false;
            }
            else
            {
                gv.Columns[0].OptionsColumn.AllowEdit = true;
                gv.Columns[2].OptionsColumn.AllowEdit = true;
                gv.Columns[4].OptionsColumn.AllowEdit = true;
            }
        }
        #endregion



        //private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    string str = str_物料编码;
        //    string strr = str_物料名称;
        //    fm基础数据包装清单_物料信息扩展界面 fm = new fm基础数据包装清单_物料信息扩展界面(str, strr);
        //    XtraTabPage xtp = XTC.TabPages.Add("包装清单");
        //    xtp.Name = "包装清单";
        //    xtp.ShowCloseButton = DefaultBoolean.Default;
        //    xtp.Controls.Add(fm);
        //    fm.Dock = DockStyle.Fill;
        //    XTC.SelectedTabPage = xtp;

        //    //try
        //    //{
        //    //    gc.BindingContext[dtM].EndCurrentEdit();
        //    //    gv.CloseEditor();
        //    //    DataTable dt = dtM.Clone();
        //    //    dt.Columns.Add("规格型号");
        //    //    dt.Columns.Add("大类");
        //    //    dt.Columns.Add("小类");
        //    //    string sql = ""; DataTable tr = new DataTable(); SqlDataAdapter da;
        //    //    foreach (DataRow dr in dtM.Rows)
        //    //    {
        //    //        if (dr.RowState == DataRowState.Deleted) continue;
        //    //        if (dr["选择"].ToString().ToLower() == "true" && dr["主辅料"].ToString() == "包装")
        //    //        {
        //    //            DataRow[] ds = dt_物料名称.Select(string.Format("子项编码 = '{0}'", dr["子项编码"].ToString()));
        //    //            DataRow r = dt.NewRow();
        //    //            dt.Rows.Add(r);
        //    //            r.ItemArray = dr.ItemArray;
        //    //            r["规格型号"] = ds[0]["n原ERP规格型号"].ToString();
        //    //            r["大类"] = ds[0]["大类"].ToString();
        //    //            r["小类"] = ds[0]["小类"].ToString();
        //    //        }
        //    //    }

        //    //    sql = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", txt_成品编码);
        //    //    tr.Clear();
        //    //    da = new SqlDataAdapter(sql, strconn);
        //    //    da.Fill(tr);

        //    //    fm基础数据包装清单_物料信息扩展界面 fm = new fm基础数据包装清单_物料信息扩展界面(txt_成品编码, tr.Rows[0]["物料名称"].ToString(), dt);
        //    //    DevExpress.XtraTab.XtraTabPage xtp = XTC.TabPages.Add("包装清单");
        //    //    xtp.ShowCloseButton = DevExpress.Utils.DefaultBoolean.Default;
        //    //    xtp.Controls.Add(fm);
        //    //    fm.Dock = DockStyle.Fill;
        //    //    XTC.SelectedTabPage = xtp;
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    CZMaster.MasterLog.WriteLog(ex.Message, "");
        //    //}
        //}

        private void gv_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (gv.GetRowCellValue(e.RowHandle, "主辅料").ToString() == "主料")
            {
                e.Appearance.BackColor = Color.LightBlue;
                e.Appearance.BackColor2 = Color.LightBlue;
            }
            if (gv.GetRowCellValue(e.RowHandle, "主辅料").ToString() == "辅料")
            {
                e.Appearance.BackColor = Color.LightGreen;
                e.Appearance.BackColor2 = Color.LightGreen;
            }
            if (gv.GetRowCellValue(e.RowHandle, "主辅料").ToString() == "包装")
            {
                e.Appearance.BackColor = Color.LightPink;
                e.Appearance.BackColor2 = Color.LightPink;
            }
        }

        private void gv_FocusedRowChanged_1(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (gv.GetDataRow(e.FocusedRowHandle) != null)
            {
                if (gv.GetDataRow(e.FocusedRowHandle).RowState == DataRowState.Added)
                {
                    gridColumn1.OptionsColumn.AllowEdit = true;
                }
                else
                {
                    gridColumn1.OptionsColumn.AllowEdit = false;
                }
            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

                    gc.ExportToXlsx(saveFileDialog.FileName);

                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    if (MessageBox.Show("是否确认该物料的BOM？", "提醒", MessageBoxButtons.OKCancel) == DialogResult.OK)
        //    {
        //        barLargeButtonItem3_ItemClick(null, null);

        //        string sql = string.Format("select * from  基础数据物料信息表 where 物料编码='{0}'", str_物料编码);
        //        using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
        //        {
        //            DataTable dt = new DataTable();

        //            da.Fill(dt);
        //            if (dt.Rows.Count > 0)
        //            {
        //                dt.Rows[0]["BOM确认"] = 1;
        //                dt.Rows[0]["BOM确认人员"] = CPublic.Var.localUserName;
        //                dt.Rows[0]["BOM确认时间"] = System.DateTime.Now;
        //                new SqlCommandBuilder(da);
        //                da.Update(dt);
        //                MessageBox.Show("ok");
        //            }
        //            else
        //            {
        //                MessageBox.Show("未找到该物料");
        //            }

        //        }
        //    }
        //}

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

          
            ERPorg.Corg.FlushMemory();
            fun_清空();
            fun_载入物料和子项类型();
            
            frm基础物料数据信息.aaaa.FM2.Add(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }





        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc, new Point(e.X, e.Y));
                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();

            }
        }



        //private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        //{

        //}

        //private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
        //    BaseData.修改单条BOM frm = new 修改单条BOM(dr, dtM);
        //    frm.ShowDialog();
        //    frm.Text = "修改BOM";
        //    if (frm.flag)
        //    {

        //        DataRow[] x = dt_物料名称.Select(string.Format("子项编码 = '{0}'", frm.str));

        //        dr["子项编码"] = x[0]["子项编码"].ToString();
        //        dr["子项名称"] = x[0]["子项名称"].ToString();

        //        // dr["物料编码"] = x[0]["物料编码"].ToString();
        //        dr["物料编码"] = x[0]["物料编码"].ToString();

        //        dr["计量单位"] = x[0]["计量单位"].ToString();
        //        dr["图纸编号"] = x[0]["图纸编号"].ToString();
        //        dr["货架描述"] = x[0]["货架描述"].ToString();

        //    }

        //}

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow sr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
                if (sr == null) return;
                //                if (sr["虚拟件"].Equals(true))
                //                {
                //                    string s = string.Format(@"select  xn.*,base.计量单位编码,base.计量单位,base.物料名称 as 子项名称,base.图纸编号,base.货架描述
                //                    ,fx.物料名称 as 父项名称 from 虚拟件对应关系表 xn
                //                    left  join 基础数据物料信息表 base on base.物料编码=xn.子项编码 
                //                    left  join 基础数据物料信息表  fx on base.物料编码=xn.父项编码 where 父项编码='{0}'", sr["子项编码"]);
                //                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                //                    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                //                    dr.Delete();
                //                    foreach (DataRow rr in temp.Rows)
                //                    {
                //                        DataRow r_ficti = dtM.NewRow();

                //                        r_ficti["子项编码"] = rr["子项编码"].ToString();
                //                        r_ficti["子项名称"] = rr["子项名称"].ToString();
                //                        r_ficti["子项名称r"] = rr["子项名称"].ToString(); //子项名称r 为物料基础表中名称,BOM表中本不应存该字段
                //                        r_ficti["计量单位"] = rr["计量单位"].ToString();
                //                        r_ficti["计量单位编码"] = rr["计量单位编码"].ToString();
                //                        r_ficti["图纸编号"] = rr["图纸编号"].ToString();
                //                        r_ficti["货架描述"] = rr["货架描述"].ToString();
                //                        r_ficti["仓库号"] = rr["仓库号"].ToString();
                //                        r_ficti["仓库名称"] = rr["仓库名称"].ToString();
                //                        r_ficti["对应虚拟件编号"] = rr["父项编码"].ToString();
                //                        r_ficti["虚拟件名称"] = rr["父项名称"].ToString();
                //                        dtM.Rows.Add(r_ficti);

                //                    }


                //                }
                //                else
                //                {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr == null) return;
                if (dtM.Select(string.Format("子项编码 = '{0}' ", sr)).Length > 0)
                {
                    throw new Exception("BOM结构中已有此项，请重新选择");
                }
                dr["子项名称"] = sr["子项名称"].ToString();
                //BOM表设计的时候 名称 就不应该放里面，取基础表中的 名称 单位 保持一致性 界面显示为末尾带r的
                dr["子项名称r"] = sr["子项名称"].ToString();
                dr["计量单位"] = sr["计量单位"].ToString();
                dr["图纸编号"] = sr["图纸编号"].ToString();
                dr["货架描述"] = sr["货架描述"].ToString();
                dr["仓库号"] = sr["仓库号"].ToString();
                dr["仓库名称"] = sr["仓库名称"].ToString();
                //}

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                DataRow sr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);

                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dtM.Select(string.Format("子项编码 = '{0}' ", sr)).Length > 0)
                {
                    throw new Exception("BOM结构中已有此项，请重新选择");
                }
                dr["子项名称"] = sr["子项名称"].ToString();

                dr["子项名称r"] = sr["子项名称"].ToString(); //子项名称r 为物料基础表中名称,BOM表中本不应存该字段
                dr["计量单位"] = sr["计量单位"].ToString();
                dr["计量单位编码"] = sr["计量单位编码"].ToString();
                if (Convert.ToBoolean(sr["虚拟件"]))
                {
                    dr["WIPType"] = "虚拟";

                }
                else
                {
                    dr["WIPType"] = "领料";
                }
                if (Convert.ToBoolean(sr["自制"]))
                {
                    dr["子项类型"] = "生产件";
                }
                else
                {
                    dr["子项类型"] = "采购件";
                }
                dr["图纸编号"] = sr["图纸编号"].ToString();
                dr["货架描述"] = sr["货架描述"].ToString();
                dr["仓库号"] = sr["仓库号"].ToString();
                dr["仓库名称"] = sr["仓库名称"].ToString();

                //}
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }



        //新增子项
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                //新增物料  str_物料编码:没有则不能新增；值为主界面 textbox中的值
                gvv1.CloseEditor();
                if (str_物料编码 == "") { }
                else
                {
                    DataRow dr = dt_BOM修改记录.NewRow();
                    dt_BOM修改记录.Rows.Add(dr);
                    dr["主辅料"] = "主料";
                    dr["子项类型"] = "采购件";
                    dr["修改人员"] = CPublic.Var.localUserName;
                    dr["修改人员ID"] = CPublic.Var.LocalUserID;
                    dr["修改日期"] = CPublic.Var.getDatetime();
                    dr["WIPType"] = "领料";
                    dr["子件损耗率"] = 0;
                    foreach (DataRow dr_1 in dt_BOM修改记录.Rows)
                    {
                        if (dr_1.RowState == DataRowState.Deleted) continue;
                        
                        if (dr_1["BOM版本号"].ToString() != "")
                        {
                            dr["BOM版本号"] = dr_1["BOM版本号"].ToString();
                        }
                        if (dr_1["BOM类型"].ToString() == "")
                        {
                            dr["BOM类型"] = "物料BOM";
                        }
                        else
                        {
                            dr["BOM类型"] = dr_1["BOM类型"].ToString();
                        }
                        
                    }
                     
                    gvv1.FocusedRowHandle = dt_BOM修改记录.Rows.Count - 1;
                    // bo_判断行数新增或者删除 = true;
                    dt_BOM修改记录.ColumnChanged += dt_BOM修改记录_ColumnChanged;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //删除子项
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {

                DataRow dr = gvv1.GetDataRow(gvv1.FocusedRowHandle);
                if (MessageBox.Show(string.Format("是否确认删除 ？", dr["子项编码"].ToString() + "--" + dr["子项名称"].ToString()), "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    //if (dr["包装数量"] != DBNull.Value)
                    //{
                    //    if (Convert.ToDecimal(dr["包装数量"]) > 0)
                    //    {
                    //        DataRow[] ds = dt_包装.Select(string.Format("物料编码 = '{0}'", dr["子项编码"].ToString()));
                    //        if (ds.Length > 0)
                    //        {
                    //            ds[0].Delete();
                    //        }
                    //    }
                    //}
                    //dr.Delete();
                    try
                    {

                        int[] dr1 = gvv1.GetSelectedRows();
                        if (dr1.Length > 0)
                        {
                            for (int i = dr1.Length - 1; i >= 0; i--)
                            {
                                DataRow dr_选中 = gvv1.GetDataRow(dr1[i]);
                                dr_选中.Delete();
                            }

                            DataRow drs = gvv1.GetDataRow(Convert.ToInt32(dr1[0]));
                            if (drs != null) gvv1.SelectRow(dr1[0]);
                            else if (gvv1.GetDataRow(Convert.ToInt32(dr1[0]) - 1) != null)
                                gvv1.SelectRow(Convert.ToInt32(dr1[0]) - 1);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    //bo_判断行数新增或者删除 = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //修改
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                barLargeButtonItem7_ItemClick(null, null);
                if (dt_BOM修改记录.Rows.Count == 0 && dtM.Rows.Count != 0)
                {
                    this.splitContainer1.SplitterDistance = 60;
                    foreach (DataRow dr in dtM.Rows)
                    {
                        DataRow rr = dt_BOM修改记录.NewRow();
                        rr["产品编码"] = dr["产品编码"].ToString();
                        rr["规格型号"] = dr["规格型号"].ToString();

                        rr["子项编码"] = dr["子项编码"].ToString();
                        rr["BOM版本号"] = dr["BOM版本号"].ToString();
                        rr["BOM版本描述"] = dr["BOM版本描述"].ToString();
                        rr["产品名称"] = dr["产品名称"].ToString();
                        rr["子项名称"] = dr["子项名称"].ToString();
                        rr["子项名称r"] = dr["子项名称"].ToString();
                        rr["数量"] = Convert.ToDecimal(dr["数量"]);
                        rr["子项类型"] = dr["子项类型"].ToString();
                        rr["主辅料"] = dr["主辅料"].ToString();
                        rr["用途"] = dr["用途"].ToString();
                        rr["修改人员"] = dr["修改人员"].ToString();
                        rr["修改人员ID"] = dr["修改人员ID"].ToString();
                        if (dr["计量单位编码"].ToString() != "")
                        {
                            rr["计量单位编码"] = dr["计量单位编码"].ToString();
                            rr["计量单位"] = dr["计量单位"].ToString();
                        }
                        if (dr["修改日期"].ToString() != "" && dr["修改日期"] != null)
                        {
                            rr["修改日期"] = Convert.ToDateTime(dr["修改日期"]);
                        }
                        rr["BOM类型"] = dr["BOM类型"].ToString();
                        rr["物料替换"] = dr["物料替换"].ToString();
                        if (dr["替换日期"].ToString() != "")
                        {
                            rr["替换日期"] = Convert.ToDateTime(dr["替换日期"]);
                        }
                        rr["替换人ID"] = dr["替换人ID"].ToString();
                        rr["替换人"] = dr["替换人"].ToString();
                        rr["总装数量"] = Convert.ToDecimal(dr["总装数量"]);
                        rr["包装数量"] = Convert.ToDecimal(dr["包装数量"]);
                        rr["A面位号"] = dr["A面位号"].ToString();
                        rr["B面位号"] = dr["B面位号"].ToString();
                        rr["组"] = dr["组"].ToString();
                        rr["优先级"] = Convert.ToInt32(dr["优先级"]);
                        rr["关键子项"] = Convert.ToBoolean(dr["关键子项"]);
                        rr["仓库号"] = dr["仓库号"].ToString();
                        rr["仓库名称"] = dr["仓库名称"].ToString();
                        //rr["对应虚拟件编号"] = dr["对应虚拟件编号"].ToString();
                        rr["WIPType"] = dr["WIPType"].ToString();
                        rr["子件损耗率"] = Convert.ToDecimal(dr["子件损耗率"]);
                        rr["图纸编号"] = dr["图纸编号"].ToString();
                        dt_BOM修改记录.Rows.Add(rr);
                    }

                    gcc1.DataSource = dt_BOM修改记录;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        DataTable dt_BOM修改记录;
        private void fun_加载BOM修改表数据()
        {
            string strt = string.Format("select * from 基础数据BOM修改明细表 where 产品编码 ='{0}' and 审核=0 order by BOM修改明细号", str_物料编码);
            using (SqlDataAdapter da = new SqlDataAdapter(strt, strconn))
            {
                dt_BOM修改记录 = new DataTable();
                da.Fill(dt_BOM修改记录);
                dt_BOM修改记录.ColumnChanged += dt_BOM修改记录_ColumnChanged;
                gcc1.DataSource = dt_BOM修改记录;
            }
        }
        private void repositoryItemSearchLookUpEdit8_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            try
            {
                DataRow dr = gvv1.GetDataRow(gvv1.FocusedRowHandle);
                if (e.NewValue != null && e.NewValue.ToString() != "")
                {
                    dr["计量单位"] = dt_unit.Select(string.Format("计量单位编码='{0}'", e.NewValue))[0]["计量单位"];
                }
                else
                {
                    dr["计量单位"] = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        DataTable dt_BOM审核申请;
        private void fun_单据审核申请()
        {
            //"生效","BOM修改申请","str_物料编码,"生产一厂"
            dt_BOM审核申请 = ERPorg.Corg.fun_PA("生效", "BOM修改申请", dt_修改主.Rows[0]["BOM修改单号"].ToString(), "生产一厂");
            if (dt_BOM审核申请.Rows[0]["作废"].Equals(true))
            {
                dt_BOM审核申请.Rows[0]["作废"] = false;
            }
        }

        //保存修改
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            gvv1.CloseEditor();//关闭编辑状态
            this.BindingContext[dt_BOM修改记录].EndCurrentEdit();//关闭编辑状态
            this.BindingContext[dt_BOM类型].EndCurrentEdit();
            this.BindingContext[dt_子项类型].EndCurrentEdit();
            this.BindingContext[dt_unit].EndCurrentEdit();
            try
            {
                fun_check();
                fun_BOM修改主子表保存();

                fun_事务保存();
                //dt_BOM修改记录.AcceptChanges();

                fun_加载BOM修改表数据();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        private void fun_事务保存()
        {
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("BOM修改保存");

            try
            {
                string sql1 = "select * from 基础数据BOM修改主表 where 1<>1";
                SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
                using (SqlDataAdapter da1 = new SqlDataAdapter(cmd1))
                {
                    new SqlCommandBuilder(da1);
                    da1.Update(dt_修改主);
                }

                string sql2 = "select * from 基础数据BOM修改明细表 where 1<>1";
                SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);
                using (SqlDataAdapter da2 = new SqlDataAdapter(cmd2))
                {
                    new SqlCommandBuilder(da2);
                    da2.Update(dt_BOM修改记录);
                }

                string sql3 = "select * from 单据审核申请表 where 1<>1";
                SqlCommand cmd3 = new SqlCommand(sql3, conn, ts);
                using (SqlDataAdapter da3 = new SqlDataAdapter(cmd3))
                {
                    new SqlCommandBuilder(da3);
                    if (dt_BOM审核申请 != null)
                    {
                        da3.Update(dt_BOM审核申请);
                    }
                }

                ts.Commit();
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw ex;
            }
        }
        private void fun_撤回事务保存(DataTable dt_撤回主, DataTable dt_子撤回, DataTable dt_单据审核作废)
        {
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("BOM修改保存");

            try
            {
                string sql1 = "select * from 基础数据BOM修改主表 where 1<>1";
                SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
                using (SqlDataAdapter da1 = new SqlDataAdapter(cmd1))
                {
                    new SqlCommandBuilder(da1);
                    da1.Update(dt_撤回主);
                }

                string sql2 = "select * from 基础数据BOM修改明细表 where 1<>1";
                SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);
                using (SqlDataAdapter da2 = new SqlDataAdapter(cmd2))
                {
                    new SqlCommandBuilder(da2);
                    da2.Update(dt_子撤回);
                }

                string sql3 = "select * from 单据审核申请表 where 1<>1";
                SqlCommand cmd3 = new SqlCommand(sql3, conn, ts);
                using (SqlDataAdapter da3 = new SqlDataAdapter(cmd3))
                {
                    new SqlCommandBuilder(da3);
                    if (dt_单据审核作废 != null)
                    {
                        da3.Update(dt_单据审核作废);
                    }
                }

                ts.Commit();
                MessageBox.Show("撤回成功！");
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw ex;
            }
        }



        //提交  
        private void simpleButton5_Click(object sender, EventArgs e)
        {
            try
            {
                fun_check();
                fun_BOM修改主子表保存();
                if (dt_修改主.Rows[0]["是否提交"].Equals(true))
                {
                    throw new Exception("BOM已经提交，如有新增请先撤回提交，再提交BOM！");
                }
                //提交主表提交状态
                dt_修改主.Rows[0]["修改人员"] = CPublic.Var.localUserName;
                dt_修改主.Rows[0]["修改人员ID"] = CPublic.Var.LocalUserID;
                dt_修改主.Rows[0]["修改日期"] = CPublic.Var.getDatetime();
                dt_修改主.Rows[0]["是否提交"] = true;
                //提交子表提交状态
                foreach (DataRow dr in dt_BOM修改记录.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    dr["提交"] = true;

                }
                //单据申请
                fun_单据审核申请();
                fun_事务保存();
                fun_加载BOM修改表数据();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //撤回

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr3 = gvv1.GetDataRow(gvv1.FocusedRowHandle);
                string str_bom修改单号 = "";
                if (dr3 == null)
                {
                    using (SqlDataAdapter da = new SqlDataAdapter("select * from 基础数据BOM修改主表 where 产品编码 ='" + str_物料编码 + "'and 审核 = 0", strconn))
                    {
                        dt_修改主 = new DataTable();
                        da.Fill(dt_修改主);
                        if (dt_修改主.Rows.Count > 0)
                        {
                            str_bom修改单号 = dt_修改主.Rows[0]["BOM修改单号"].ToString();
                        }
                        else
                        {
                            throw new Exception("没有修改记录！");
                        }
                    }


                }
                else if (dr3["BOM修改单号"].ToString() == "")
                {
                    throw new Exception("没有修改记录！");
                }

                else
                {
                    str_bom修改单号 = dr3["BOM修改单号"].ToString();

                }


                string stree = string.Format("select * from 基础数据BOM修改主表 where BOM修改单号 ='{0}'", str_bom修改单号);
                DataTable dt_撤回主 = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(stree, strconn))
                {
                    da.Fill(dt_撤回主);

                    //撤回主表提交
                    dt_撤回主.Rows[0]["修改人员"] = CPublic.Var.localUserName;
                    dt_撤回主.Rows[0]["修改人员ID"] = CPublic.Var.LocalUserID;
                    dt_撤回主.Rows[0]["修改日期"] = CPublic.Var.getDatetime();
                    dt_撤回主.Rows[0]["是否提交"] = false;
                }
                if (dt_撤回主.Rows[0]["审核"].Equals(true))
                {
                    throw new Exception("该BOM记录已经审核！");
                }
                string stree1 = string.Format("select * from 基础数据BOM修改明细表 where BOM修改单号 ='{0}'", str_bom修改单号);
                DataTable dt_子撤回 = new DataTable();
                using (SqlDataAdapter da1 = new SqlDataAdapter(stree1, strconn))
                {

                    da1.Fill(dt_子撤回);
                }
                //撤回子表提交
                foreach (DataRow dr in dt_子撤回.Rows)
                {
                    dr["提交"] = false;
                }
                //撤回单据申请
                string stre = string.Format("select * from 单据审核申请表 where 关联单号='{0}'and 审核=0 and 作废=0 ", dt_撤回主.Rows[0]["BOM修改单号"].ToString());
                DataTable dt_单据审核作废 = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(stre, strconn))
                {
                    da.Fill(dt_单据审核作废);

                }
                if (dt_单据审核作废.Rows.Count == 0)
                {
                    throw new Exception("撤回失败！");
                }
                dt_单据审核作废.Rows[0]["作废"] = 1;
                //dt_BOM审核申请 = dt_作废.Copy();
                fun_撤回事务保存(dt_撤回主, dt_子撤回, dt_单据审核作废);
                fun_加载BOM修改表数据();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        DataTable dt_修改主;
        private void fun_BOM修改主子表保存()
        {
            //BOM修改 生成单号没有生成单号的时候  主表
            using (SqlDataAdapter da = new SqlDataAdapter("select * from 基础数据BOM修改主表 where 产品编码 ='" + str_物料编码 + "'and 审核 = 0", strconn))
            {
                dt_修改主 = new DataTable();
                da.Fill(dt_修改主);
            }
            string sql_版本 = string.Format(" select top 1 BOM版本号 from  基础数据BOM修改明细表 where 产品编码 = '{0}' and 审核 = 1 order by BOM修改单号 desc", str_物料编码);
            DataTable dt_版本号 = CZMaster.MasterSQL.Get_DataTable(sql_版本, strconn);
            string s_版本号 = "";
            if (dt_版本号.Rows.Count == 0)
            {
                s_版本号 = "";
            }
            else
            {
                if (dt_版本号.Rows[0]["BOM版本号"].ToString() == "")
                {
                    s_版本号 = "1";
                }
                else
                {
                    s_版本号 = (Convert.ToInt32(dt_版本号.Rows[0]["BOM版本号"]) + 1).ToString();
                }
            }
            string a = "";
            if (dt_修改主.Rows.Count == 0)
            {
                DataRow dr_改主 = dt_修改主.NewRow();
                DateTime t = CPublic.Var.getDatetime();
                a = string.Format("BOMX{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("BOMX", t.Year, t.Month));
                dr_改主["GUID"] = System.Guid.NewGuid();
                dr_改主["BOM修改单号"] = a;
                dr_改主["产品编码"] = str_物料编码;
                dr_改主["产品名称"] = str_物料名称;
                dr_改主["规格型号"] = str_规格;
                dr_改主["修改人员"] = CPublic.Var.localUserName;
                dr_改主["修改人员ID"] = CPublic.Var.LocalUserID;
                dr_改主["修改日期"] = t;
                dt_修改主.Rows.Add(dr_改主);
                //BOM保存子表
                int i = 0;
                foreach (DataRow dr in dt_BOM修改记录.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    dr["BOM修改单号"] = a;
                    dr["BOM修改明细号"] = a + "-" + i.ToString("00");
                    dr["GUID"] = System.Guid.NewGuid();
                    dr["产品编码"] = str_物料编码;
                    dr["产品名称"] = str_物料名称;
                    dr["审核"] = false;
                    dr["提交"] = false;
                    dr["BOM版本号"] = s_版本号;
                    i++;
                }
            }
            else
            {
                a = dt_修改主.Rows[0]["BOM修改单号"].ToString();
                dt_修改主.Rows[0]["修改人员"] = CPublic.Var.localUserName;
                dt_修改主.Rows[0]["修改人员ID"] = CPublic.Var.LocalUserID;
                dt_修改主.Rows[0]["修改日期"] = CPublic.Var.getDatetime();
                int i = 0;
                foreach (DataRow dr in dt_BOM修改记录.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    dr["BOM修改单号"] = a;
                    dr["BOM修改明细号"] = a + "-" + i.ToString("00");
                    if (dr["GUID"].ToString() == "")
                    {
                        dr["GUID"] = System.Guid.NewGuid();
                    }
                    dr["产品编码"] = str_物料编码;
                    dr["产品名称"] = str_物料名称;
                    dr["BOM版本号"] = s_版本号;
                    i++;
                }
            }
        }

        //查找虚拟件 和 repositoryItemSearchLookUpEdit5 的事件行变化
        private void gridView4_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow sr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
                if (sr == null) return;
                DataRow dr = gvv1.GetDataRow(gvv1.FocusedRowHandle);
                if (dr == null) return;
                if (dt_BOM修改记录.Select(string.Format("子项编码 = '{0}' ", sr)).Length > 0)
                {
                    throw new Exception("BOM结构中已有此项，请重新选择");
                }
                dr["子项名称"] = sr["子项名称"].ToString();
                //BOM表设计的时候 名称 就不应该放里面，取基础表中的 名称 单位 保持一致性 界面显示为末尾带r的
                dr["子项名称r"] = sr["子项名称"].ToString();
                dr["计量单位"] = sr["计量单位"].ToString();
                dr["图纸编号"] = sr["图纸编号"].ToString();
                dr["货架描述"] = sr["货架描述"].ToString();
                dr["仓库号"] = sr["仓库号"].ToString();
                dr["仓库名称"] = sr["仓库名称"].ToString();
                dr["规格型号"] = sr["规格型号"].ToString();

                // dr["数量"] = sr["数量"].ToString();
            }

            //}
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }




        private void gv_CellValueChanging_1(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(e.RowHandle);

                if (e.Column.Caption == "包装数量")
                {
                    if (dr["包装数量"] == DBNull.Value)
                        dr["包装数量"] = 0;
                    if (dr["总装数量"] == DBNull.Value)
                        dr["总装数量"] = 0;
                    dr["数量"] = Convert.ToDecimal(e.Value) + Convert.ToDecimal(dr["总装数量"]);
                }
                else if (e.Column.Caption == "总装数量")
                {
                    if (dr["包装数量"] == DBNull.Value)
                        dr["包装数量"] = 0;
                    if (dr["总装数量"] == DBNull.Value)
                        dr["总装数量"] = 0;
                    dr["数量"] = Convert.ToDecimal(dr["包装数量"]) + Convert.ToDecimal(e.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void gridView7_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow sr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
                DataRow dr = gvv1.GetDataRow(gvv1.FocusedRowHandle);
                if (sr["计量单位"] != null && sr["计量单位"].ToString() != "")
                {
                    dr["计量单位"] = sr["计量单位"].ToString();
                }
            }
            catch
            { }
        }

        private void gridView4_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {

            DataRow sr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);

            DataRow dr = gvv1.GetDataRow(gvv1.FocusedRowHandle);
            if (dt_BOM修改记录.Select(string.Format("子项编码 = '{0}' ", sr)).Length > 0)
            {
                throw new Exception("BOM结构中已有此项，请重新选择");
            }
            dr["子项名称"] = sr["子项名称"].ToString();
            //BOM表设计的时候 名称 就不应该放里面，取基础表中的 名称 单位 保持一致性 界面显示为末尾带r的
            dr["子项名称r"] = sr["子项名称"].ToString();
            dr["计量单位"] = sr["计量单位"].ToString();
            dr["图纸编号"] = sr["图纸编号"].ToString();
            dr["计量单位编码"] = sr["计量单位编码"].ToString();
            dr["货架描述"] = sr["货架描述"].ToString();
            dr["仓库号"] = sr["仓库号"].ToString();
            dr["仓库名称"] = sr["仓库名称"].ToString();
            dr["规格型号"] = sr["规格型号"].ToString();
            if (Convert.ToBoolean(sr["虚拟件"]))
            {
                dr["WIPType"] = "虚拟";

            }
            else
            {
                dr["WIPType"] = "领料";
            }
            if (Convert.ToBoolean(sr["自制"]))
            {
                dr["子项类型"] = "生产件";
            }
            else
            {
                dr["子项类型"] = "采购件";
            }

        }
        //刷新
        private void simpleButton7_Click(object sender, EventArgs e)
        {
            this.splitContainer1.SplitterDistance = 208;
            fun_加载BOM修改表数据();
        }
        //领料描述
        private void repositoryItemSearchLookUpEdit1View_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow sr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
                DataRow dr1 = gvv1.GetDataRow(gvv1.FocusedRowHandle);
                if (sr["领料类型"] != null && sr["领料类型"].ToString() != "")
                {
                    dr1["WIPType"] = sr["领料类型"].ToString();
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }





        }




        private void infolink()
        {
            DateTime t = CPublic.Var.getDatetime().Date.AddDays(1);
            foreach (DataRow dr in dt_BOM修改记录.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                try
                {
                    if (dr["仓库号"].ToString() == "")
                    {
                        DataRow[] r = dt_物料名称.Select(string.Format("子项编码='{0}'", dr["子项编码"]));
                        //dr["新数据"] = r[0]["新数据"].ToString();
                        dr["子项名称"] = r[0]["子项名称"].ToString();

                        dr["子项名称r"] = r[0]["子项名称"].ToString();



                        dr["计量单位"] = r[0]["计量单位"].ToString();
                        dr["计量单位编码"] = r[0]["计量单位编码"].ToString();
                        dr["规格型号"] = r[0]["规格型号"].ToString();
                        dr["仓库号"] = r[0]["默认仓库号"].ToString();
                        dr["仓库名称"] = r[0]["默认仓库"].ToString();
                        dr["主辅料"] = "主料";
                        // dr["领料类型"] = "领料";
                        if (Convert.ToBoolean(r[0]["虚拟件"]))
                        {
                            dr["WIPType"] = "虚拟";

                        }
                        else
                        {
                            dr["WIPType"] = "领料";
                        }
                        dr["子件损耗率"] = 0;



                        if (dt_BOM修改记录.Rows.Count > 0 && dt_BOM修改记录.Rows[0]["BOM版本号"].ToString() != "")
                        {
                            dr["BOM版本号"] = dt_BOM修改记录.Rows[0]["BOM版本号"].ToString();
                        }
                        if (dt_BOM修改记录.Rows[0]["BOM类型"].ToString() == "")
                        {
                            dr["BOM类型"] = "物料BOM";
                        }
                        else
                        {
                            dr["BOM类型"] = dt_BOM修改记录.Rows[0]["BOM类型"].ToString();
                        }
                        if (Convert.ToBoolean(r[0]["自制"]))
                        {
                            dr["子项类型"] = "生产件";
                        }
                        else
                        {
                            dr["子项类型"] = "采购件";
                        }

                        dr["修改人员"] = CPublic.Var.localUserName;
                        dr["修改人员ID"] = CPublic.Var.LocalUserID;
                        dr["修改日期"] = CPublic.Var.getDatetime();
                    }
                }
                catch
                {

                }

            }

        }

        private void gvv1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (gvv1.FocusedColumn.Caption == "子项编码") infolink();
                if (gvv1.FocusedColumn.Caption == "仓库号") infolink_stock();
            }
        }
        private void infolink_stock()
        {
            DateTime t = CPublic.Var.getDatetime().Date.AddDays(1);
            foreach (DataRow dr in dt_BOM修改记录.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                try
                {
                    DataRow[] ds = dt_仓库.Select(string.Format("仓库号 = {0}", dr["仓库号"]));
                    dr["仓库名称"] = ds[0]["仓库名称"];

                }
                catch (Exception)
                {

                }

            }

        }
        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void simpleButton8_Click(object sender, EventArgs e)
        {
            string sql = string.Format(@"select * from  基础数据物料信息表 where 物料编码 = '{0}'", str_物料编码.ToString());
            DataTable dt_BOM确认 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            if (dt_BOM确认.Rows.Count > 0)
            {

                dt_BOM确认.Rows[0]["BOM确认"] = true;

                label2.Text = "已确认";
                MessageBox.Show("BOM确认完成！");

            }
            CZMaster.MasterSQL.Save_DataTable(dt_BOM确认, "基础数据物料信息表", strconn);

        }

        private void gvv1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gvv1.GetDataRow(e.RowHandle);

                if (e.Column.Caption == "包装数量")
                {
                    if (dr["包装数量"] == DBNull.Value)
                        dr["包装数量"] = 0;
                    if (dr["总装数量"] == DBNull.Value)
                        dr["总装数量"] = 0;
                    dr["数量"] = Convert.ToDecimal(e.Value) + Convert.ToDecimal(dr["总装数量"]);
                }
                else if (e.Column.Caption == "总装数量")
                {
                    if (dr["包装数量"] == DBNull.Value)
                        dr["包装数量"] = 0;
                    if (dr["总装数量"] == DBNull.Value)
                        dr["总装数量"] = 0;
                    dr["数量"] = Convert.ToDecimal(dr["包装数量"]) + Convert.ToDecimal(e.Value);
                }
                else if (e.Column.Caption == "仓库号")
                {
                    DataRow[] rr = dt_仓库.Select(string.Format("仓库号='{0}'", e.Value));
                    if (rr.Length > 0) dr["仓库名称"] = rr[0]["仓库名称"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
