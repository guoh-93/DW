using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace BaseData
{
    public partial class 销售成套件BOM : UserControl
    {
        string strconn = CPublic.Var.strConn;
        public string str_物料编码 = "";  //物料编码
        public string str_物料名称 = "";  //物料名称
        public string str_规格 = "";  //规格型号
        public string str_原规格型号 = "";
        DataTable dtM = null;
        DataTable dtM1;//存储BOM版本
        string txt_成品编码;
        DataTable dt_物料名称;
        DataTable dt_子项类型;
        DataTable dt_BOM类型;
        DataTable dt_unit; //计量单位
        DataTable dt_BOM修改;
        //DataTable dt_BOM修改记录;
        string cfgfilepath = "";




        public static System.Windows.Forms.TabControl STC;

        public 销售成套件BOM()
        {
            InitializeComponent();
        }

        private void 销售成套件BOM_Load(object sender, EventArgs e)
        {
            fun_清空();
            fun_载入物料和子项类型();
            string sql = string.Format("select * from 基础数据BOM信息修改记录表 where 1<>1");
            dt_BOM修改 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_BOM修改);
            销售新增成套件.aaaa.FM2.Add(this);
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            else
            {
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(splitContainer1, this.Name, cfgfilepath);
            }
            
        }

        private void fun_载入物料和子项类型()
        {
            try
            {
                string sql = @" select (a.物料编码) as 子项编码,(a.物料名称) as 子项名称,a.规格型号,b.仓库号,b.仓库名称,b.货架描述,b.库存总数,大类,小类,a.物料属性,a.图纸编号,a.计量单位编码,a.计量单位,虚拟件
                  from 基础数据物料信息表 a   left join 仓库物料数量表 b on a.物料编码=b.物料编码";//where  停用 = 0 物料类型 = '原材料' or 物料类型 = '半成品' and
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                dt_物料名称 = new DataTable();
                da.Fill(dt_物料名称);

                sql = "select 属性值 from 基础数据基础属性表 where 属性类别 = 'BOM子项类型'";
                da = new SqlDataAdapter(sql, strconn);
                dt_子项类型 = new DataTable();
                da.Fill(dt_子项类型);

                sql = "select 属性值 from 基础数据基础属性表 where 属性类别 = 'BOM类型'";
                da = new SqlDataAdapter(sql, strconn);
                dt_BOM类型 = new DataTable();
                da.Fill(dt_BOM类型);
            
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

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_清空()
        {
            //string sql = "select  * from 基础数据物料BOM表 where 1<>1";
            //dtM = new DataTable();
            //SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            //da.Fill(dtM);
            //gcc1.DataSource = dtM;
            fun_载入数据();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //新增物料  str_物料编码:没有则不能新增；值为主界面 textbox中的值
                if (str_物料编码 == "") { }
                else
                { 
                    DataRow dr = dtM.NewRow();
                    dtM.Rows.Add(dr);
                    dr["产品编码"] = textBox1.Text;
                    dr["产品名称"] = textBox2.Text;
                    dr["BOM类型"] = "物料BOM";
                    dr["主辅料"] = "主料";
                    dr["WIPType"] = "领料";
                    dr["BOM版本号"] = 1;
                    dr["修改人员"] = CPublic.Var.localUserName;
                    dr["修改人员ID"] = CPublic.Var.LocalUserID;
                    dr["修改日期"] = CPublic.Var.getDatetime();

                    gvv1.FocusedRowHandle = dtM.Rows.Count - 1;
                    //// bo_判断行数新增或者删除 = true;
                    dtM.ColumnChanged += dtM_ColumnChanged;
                }
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void fun_载入数据()
        {
            try
            {
                
                if (dtM != null)
                {
                    dtM.Clear();
                }
                txt_成品编码 = textBox1.Text = str_物料编码;
                textBox2.Text = str_物料名称;
                textBox3.Text = str_规格;
                dtM = new DataTable();
                dtM1 = new DataTable();
                //  left  join 仓库物料数量表 kc on kc.物料编码=b.物料编码   ,kc.货架描述 kc.仓库号=a.仓库号 and
                string sql = string.Format(@"select 产品编码,子项编码,BOM版本号,BOM版本描述,产品名称,子项名称,b.物料名称 as 子项名称r,[数量],[子项类型],
                a.[主辅料],[用途],[修改人员],[修改人员ID],a.计量单位编码,a.[计量单位],a.[修改日期],[BOM类型],[物料替换],对应虚拟件编号,xn.物料名称 as 虚拟件名称
                ,[替换日期],[替换人ID],[替换人],[总装数量],[包装数量],[A面位号],B面位号,关键子项,[组],[优先级],b.图纸编号,a.仓库号,a.仓库名称,a.子件损耗率,a.WIPType from 基础数据物料BOM表 a
                left join 基础数据物料信息表 b on a.子项编码 = b.物料编码             
                left  join 基础数据物料信息表 xn on a.对应虚拟件编号 = xn.物料编码 
                where    a.产品编码 = '{0}'", txt_成品编码);
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                
                new SqlCommandBuilder(da);
               // dtM.Columns.Add("选择", typeof(Boolean));
                da.Fill(dtM);
                da.Fill(dtM1);
                //dtM1 = dtM.Copy();
                gcc1.DataSource = dtM;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                // CZMaster.MasterLog.WriteLog(ex.Message, "frm基础数据物料BOM_fun_载入数据");
            }
        }

        void dtM_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            dtM.ColumnChanged -= dtM_ColumnChanged;
            if (e.Column.Caption == "子项编码")
            {
                if (dtM.Select(string.Format("子项编码 = '{0}'", e.Row["子项编码"].ToString())).Length > 0)
                {
                    MessageBox.Show("BOM结构中已有此项，请重新选择");
                }
              
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
            dtM.ColumnChanged += dtM_ColumnChanged;
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gvv1.GetDataRow(gvv1.FocusedRowHandle);
                dr.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gvv1.CloseEditor();//关闭编辑状态
            this.BindingContext[dtM].EndCurrentEdit();//关闭编辑状态
            this.BindingContext[dt_BOM类型].EndCurrentEdit();
            this.BindingContext[dt_子项类型].EndCurrentEdit();
            this.BindingContext[dt_unit].EndCurrentEdit();
            try
            {
                fun_BOM修改主子表保存();
                
                fun_check();
                fun_事务保存();
                fun_清空();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        

        private void fun_加载BOM修改表数据()
        {
            string strt = string.Format("select * from 基础数据BOM修改明细表 where 产品编码 ='{0}'", str_物料编码);
            using (SqlDataAdapter da = new SqlDataAdapter(strt, strconn))
            {
                dtM = new DataTable();
                da.Fill(dtM);
                gcc1.DataSource = dtM;
                dtM.ColumnChanged += dtM_ColumnChanged;
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
                    da2.Update(dt_修改明细);
                }
                string sql3 = "select * from 基础数据物料BOM表 where 1<>1";
                SqlCommand cmd3 = new SqlCommand(sql3, conn, ts);
                using(SqlDataAdapter da3 = new SqlDataAdapter(cmd3))
                {
                    new SqlCommandBuilder(da3);
                    da3.Update(dtM);
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

        private void fun_check()
        {            
            foreach (DataRow dr in dtM.Rows)
            {

                if (dr["计量单位编码"].ToString() == "")
                {
                    throw new Exception("计量单位未选择");
                }
                if (dr["仓库号"].ToString() == "")
                {
                    throw new Exception("仓库未选择");
                }
                decimal dec = 0;
                if (!decimal.TryParse(dr["总装数量"].ToString(), out dec))
                {
                    throw new Exception("总装数量输入有误,请检查");
                }
              
            }
        }

        DataTable dt_修改主;
        DataTable dt_修改明细;
        private void fun_BOM修改主子表保存()
        {
            using (SqlDataAdapter da = new SqlDataAdapter("select * from 基础数据BOM修改主表 where 产品编码 ='" + str_物料编码 + "'", strconn))
            {
                dt_修改主 = new DataTable();
                da.Fill(dt_修改主);
            }
            using (SqlDataAdapter da1 = new SqlDataAdapter("select * from 基础数据BOM修改明细表 where 1<>1",strconn))
            {
                dt_修改明细 = new DataTable();
                da1.Fill(dt_修改明细);
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
                dr_改主["审核"] = true;
                dr_改主["是否提交"] = true;
                dt_修改主.Rows.Add(dr_改主);
                //BOM保存子表
                int i = 0;
                foreach (DataRow dr in dtM.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    DataRow dr_修改明细 = dt_修改明细.NewRow();
                    dt_修改明细.Rows.Add(dr_修改明细);
                    dr_修改明细["修改日期"] = t;
                    dr_修改明细["修改人员"] = CPublic.Var.localUserName;
                    dr_修改明细["修改人员ID"] = CPublic.Var.LocalUserID;
                    dr_修改明细["BOM修改单号"] = a;
                    dr_修改明细["BOM修改明细号"] = a + "-" + i;
                    dr_修改明细["GUID"] = System.Guid.NewGuid();
                    dr_修改明细["产品编码"] = str_物料编码;
                    dr_修改明细["产品名称"] = str_物料名称;
                    dr_修改明细["子项编码"] = dr["子项编码"];
                    dr_修改明细["子项名称"] = dr["子项名称"];
                    dr_修改明细["计量单位编码"] = dr["计量单位编码"];
                    dr_修改明细["计量单位"] = dr["计量单位"];
                    dr_修改明细["BOM类型"] = dr["BOM类型"];
                    dr_修改明细["总装数量"] = dr["总装数量"];
                    dr_修改明细["包装数量"] = dr["包装数量"];
                    dr_修改明细["子件损耗率"] = dr["子件损耗率"];
                    dr_修改明细["仓库号"] = dr["仓库号"];
                    dr_修改明细["仓库名称"] = dr["仓库名称"];
                    dr_修改明细["主辅料"] = dr["主辅料"];
                    dr_修改明细["子项类型"] = dr["子项类型"];                   
                    dr_修改明细["审核"] = true;
                    dr_修改明细["提交"] = true;
                    i++;
                }
            }
            else
            {
                a = dt_修改主.Rows[0]["BOM修改单号"].ToString();
                dt_修改主.Rows[0]["修改人员"] = CPublic.Var.localUserName;
                dt_修改主.Rows[0]["修改人员ID"] = CPublic.Var.LocalUserID;
                dt_修改主.Rows[0]["修改日期"] = CPublic.Var.getDatetime();
                dt_修改主.Rows[0]["审核"] = true;
                dt_修改主.Rows[0]["是否提交"] = true;
                int i = 0;
                foreach (DataRow dr in dtM.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    DataRow dr_修改明细 = dt_修改明细.NewRow();
                    dt_修改明细.Rows.Add(dr_修改明细);
                    dr_修改明细["修改日期"] = CPublic.Var.getDatetime();
                    dr_修改明细["修改人员"] = CPublic.Var.localUserName;
                    dr_修改明细["修改人员ID"] = CPublic.Var.LocalUserID;
                    dr_修改明细["BOM修改单号"] = a;
                    dr_修改明细["BOM修改明细号"] = a + "-" + i;
                    dr_修改明细["GUID"] = System.Guid.NewGuid();
                    dr_修改明细["产品编码"] = str_物料编码;
                    dr_修改明细["产品名称"] = str_物料名称;
                    dr_修改明细["子项编码"] = dr["子项编码"];
                    dr_修改明细["子项名称"] = dr["子项名称"];
                    dr_修改明细["计量单位编码"] = dr["计量单位编码"];
                    dr_修改明细["计量单位"] = dr["计量单位"];
                    dr_修改明细["BOM类型"] = dr["BOM类型"];
                    dr_修改明细["总装数量"] = dr["总装数量"];
                    dr_修改明细["包装数量"] = dr["包装数量"];
                    dr_修改明细["子件损耗率"] = dr["子件损耗率"];
                    dr_修改明细["仓库号"] = dr["仓库号"];
                    dr_修改明细["仓库名称"] = dr["仓库名称"];
                    dr_修改明细["主辅料"] = dr["主辅料"];
                    dr_修改明细["子项类型"] = dr["子项类型"];
                    dr_修改明细["审核"] = true;
                    dr_修改明细["提交"] = true;
                    i++;
                }

            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_清空();
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gvv1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow dr = gvv1.GetDataRow(e.RowHandle);
            
            try
            {
                if (e.Column.Caption == "计量单位编码")
                {
                    if (dr["计量单位编码"] == DBNull.Value)
                        dr["计量单位"] = "";
                    else
                    {
                        //string s = dt_unit.Select(string.Format("计量单位编码='{0}'",e.Value))[0]["计量单位"].ToString();
                        dr["计量单位"] = dt_unit.Select(string.Format("计量单位编码='{0}'", e.Value))[0]["计量单位"];
                    }


                }
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

        private void gridView4_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow sr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
            DataRow dr = gvv1.GetDataRow(gvv1.FocusedRowHandle);
            if (dtM.Select(string.Format("子项编码 = '{0}' ", sr)).Length > 0)
            {
                throw new Exception("BOM结构中已有此项，请重新选择");
            }          
            dr["子项名称"] = sr["子项名称"].ToString();
            //BOM表设计的时候 名称 就不应该放里面，取基础表中的 名称 单位 保持一致性 界面显示为末尾带r的
            dr["子项名称r"] = sr["子项名称"].ToString();
            dr["计量单位"] = sr["计量单位"].ToString();
            dr["图纸编号"] = sr["图纸编号"].ToString();
            dr["计量单位编码"] = sr["计量单位编码"].ToString();
            //dr["货架描述"] = sr["货架描述"].ToString();
            dr["仓库号"] = sr["仓库号"].ToString();
            dr["仓库名称"] = sr["仓库名称"].ToString();
        }


    }
}
