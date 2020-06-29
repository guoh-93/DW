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


namespace StockCore
{
    public partial class ui_ECR变更申请 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        #region 变量
        DataTable dt_产品编码;
        DataTable dt_涉及物料;
        DataTable dt_变更影响范围;
        DataTable dt_审核部门;
        DataTable dt_变更申请主;
        DataTable dt_变更申请子;
        DataTable dt_变更影响;
        DataTable dt_变更审核;
        DataTable dt_采购料况;
        DataTable dt_生产料况;
        DataTable dt_库存料况;
        DataTable dt_制令料况;
        DataTable dt_销售料况;
        DataTable dt_物料;
        DataTable dt_负责人;
        bool bl_变更查询 = false;
        bool bl_新增 = true;
        string str_申请单号;
        DataRow dr_申请;
        #endregion
        public ui_ECR变更申请()
        {
            InitializeComponent();
        }

        public ui_ECR变更申请(DataRow dr)
        {
            InitializeComponent();
            dr_申请 = dr;
            bl_新增 = false;
            fun_下拉框();
        }

        private void ui_ECR变更申请_Load(object sender, EventArgs e)
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
                x.UserLayout(this.panel8, this.Name, cfgfilepath);
                string sql = "select 物料编码,物料名称,规格型号 from 基础数据物料信息表 where 1<>1";
                dt_产品编码 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                gridControl1.DataSource = dt_产品编码;
                DateTime t = CPublic.Var.getDatetime().Date;
                if (bl_新增)
                {
                    textBox9.Text = CPublic.Var.LocalUserID;
                    fun_下拉框();
                    fun_load();
                    dateEdit1.EditValue = t;
                }
                else
                {
                    fun_load_1();
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load_1()
        {
            string sql = string.Format("select * from ECR变更申请单主表 where 申请单号 = '{0}'", dr_申请["申请单号"].ToString());
            dt_变更申请主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            sql = string.Format("select * from ECR变更申请单明细表 where 申请单号 = '{0}'", dr_申请["申请单号"].ToString());
            dt_变更申请子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            sql = string.Format("select * from ECR变更申请采购料况表 where 申请单号 = '{0}'", dr_申请["申请单号"].ToString());
            dt_采购料况 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            sql = string.Format("select * from ECR变更申请生产料况表 where 申请单号 = '{0}'", dr_申请["申请单号"].ToString());
            dt_生产料况 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            sql = string.Format("select * from ECR变更申请库存料况 where 申请单号 = '{0}'", dr_申请["申请单号"].ToString());
            dt_库存料况 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            sql = string.Format("select * from ECR变更申请销售料况表 where 申请单号 = '{0}'", dr_申请["申请单号"].ToString());
            dt_销售料况 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            sql = string.Format("select * from ECR变更申请制令料况表 where 申请单号 = '{0}'", dr_申请["申请单号"].ToString());
            dt_制令料况 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            sql = string.Format("select * from ECR变更申请影响范围表 where 申请单号 = '{0}'", dr_申请["申请单号"].ToString());
            dt_变更影响 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            sql = string.Format("select * from ECR变更申请审核表 where 申请单号 = '{0}' ", dr_申请["申请单号"].ToString());
            dt_变更审核 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            sql = string.Format("select * from ECR变更申请影响物料表 where 申请单号 = '{0}' ", dr_申请["申请单号"].ToString());
            dt_涉及物料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            DataColumn dcc = new DataColumn("选择", typeof(bool));
            dcc.DefaultValue = true;
            dt_变更审核.Columns.Add(dcc);
            DataColumn dca = new DataColumn("选择", typeof(bool));
            dca.DefaultValue = true;
            dt_涉及物料.Columns.Add(dca);
            fun_编辑();
            fun_显示();
            
            



        }

        private void fun_显示()
        {
            textBox2.Text = dt_变更申请主.Rows[0]["提出单位"].ToString();
            textBox4.Text = dt_变更申请主.Rows[0]["单位主管"].ToString();
            searchLookUpEdit6.EditValue = dt_变更申请主.Rows[0]["ECO紧急度"].ToString();
            searchLookUpEdit1.EditValue = dt_变更申请主.Rows[0]["变更来源"].ToString();
            searchLookUpEdit2.EditValue = dt_变更申请主.Rows[0]["变更类别"].ToString();
            searchLookUpEdit3.EditValue = dt_变更申请主.Rows[0]["导入方式"].ToString();
            searchLookUpEdit4.EditValue = dt_变更申请主.Rows[0]["变更性质"].ToString();
            dateEdit1.EditValue = Convert.ToDateTime(dt_变更申请主.Rows[0]["提出日期"].ToString());
            dateEdit2.EditValue = Convert.ToDateTime(dt_变更申请主.Rows[0]["期望完成日期"].ToString());
            searchLookUpEdit7.EditValue = dt_变更申请主.Rows[0]["变更阶段"].ToString();
            textBox9.Text = dt_变更申请主.Rows[0]["申请人"].ToString();
            textBox11.Text = dt_变更申请主.Rows[0]["变更原因"].ToString();
            textBox10.Text = dt_变更申请主.Rows[0]["变更内容"].ToString();
            textBox12.Text = dt_变更申请主.Rows[0]["新旧品兼容性影响"].ToString();
            checkBox1.Checked =Convert.ToBoolean(dt_变更申请主.Rows[0]["制造MES影响"].ToString());
            checkBox2.Checked = Convert.ToBoolean(dt_变更申请主.Rows[0]["涉及SOP变更"].ToString());         
            searchLookUpEdit5.EditValue = dt_变更申请主.Rows[0]["ECR提出人ID"].ToString();
            textBox1.Text = dt_变更申请主.Rows[0]["ECR提出人"].ToString();
            gridControl1.DataSource = dt_变更申请子;
            gridControl8.DataSource = dt_销售料况;
            gridControl7.DataSource = dt_采购料况;
            gridControl9.DataSource = dt_制令料况;
            gridControl4.DataSource = dt_生产料况;
            gridControl5.DataSource = dt_库存料况;
            gridControl2.DataSource = dt_变更影响;
            gridControl6.DataSource = dt_变更审核;
            gridControl3.DataSource = dt_涉及物料;
        }

        private void fun_编辑()
        {
            gridView1.OptionsBehavior.Editable = false;
            gridView2.OptionsBehavior.Editable = false;
            gridView6.OptionsBehavior.Editable = false;
            gridView11.OptionsBehavior.Editable = false;
            gridView4.OptionsBehavior.Editable = false;
            gridView5.OptionsBehavior.Editable = false;
            gridView16.OptionsBehavior.Editable = false;
            panel3.Visible = false;
            panel7.Visible = false;
            barLargeButtonItem2.Enabled = false;
            searchLookUpEdit5.Enabled = false;
            searchLookUpEdit6.Enabled = false;
            searchLookUpEdit1.Enabled = false;
            searchLookUpEdit2.Enabled = false;
            searchLookUpEdit3.Enabled = false;
            searchLookUpEdit4.Enabled = false;
            dateEdit1.Enabled = false;
            dateEdit2.Enabled = false;
            searchLookUpEdit7.Enabled = false;
            checkBox1.Enabled = false;
            checkBox2.Enabled = false;
            textBox11.Enabled = false;
            textBox10.Enabled = false;
            textBox12.Enabled = false;
        }

        private void fun_下拉框()
        {
            string sql = "select 物料编码,物料名称,规格型号 from  基础数据物料信息表 where 停用 =0";
            dt_物料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            repositoryItemSearchLookUpEdit1.PopupFormSize = new Size(700, 400);
            repositoryItemSearchLookUpEdit1.DataSource = dt_物料;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";

            sql = "select 员工号,姓名 from 人事基础员工表 where 在职状态 <>'离职'";
            dt_负责人 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            searchLookUpEdit5.Properties.DataSource = dt_负责人;
            searchLookUpEdit5.Properties.ValueMember = "员工号";
            searchLookUpEdit5.Properties.DisplayMember = "员工号";

            repositoryItemSearchLookUpEdit3.DataSource = dt_负责人;
            repositoryItemSearchLookUpEdit3.ValueMember = "员工号";
            repositoryItemSearchLookUpEdit3.DisplayMember = "员工号";

            sql = "select  属性值 as ECO紧急度   from 基础数据基础属性表 where 属性类别 = 'ECO紧急度'";
            DataTable dt_ECO紧急度 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            searchLookUpEdit6.Properties.DataSource = dt_ECO紧急度;
            searchLookUpEdit6.Properties.ValueMember = "ECO紧急度";
            searchLookUpEdit6.Properties.DisplayMember = "ECO紧急度";

            sql = "select  属性值 as 变更来源   from 基础数据基础属性表 where 属性类别 = '变更来源'";
            DataTable dt_变更来源 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            searchLookUpEdit1.Properties.DataSource = dt_变更来源;
            searchLookUpEdit1.Properties.ValueMember = "变更来源";
            searchLookUpEdit1.Properties.DisplayMember = "变更来源";

            sql = "select  属性值 as 变更类别   from 基础数据基础属性表 where 属性类别 = '变更类别'";
            DataTable dt_变更类别 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            searchLookUpEdit2.Properties.DataSource = dt_变更类别;
            searchLookUpEdit2.Properties.ValueMember = "变更类别";
            searchLookUpEdit2.Properties.DisplayMember = "变更类别";

            sql = "select  属性值 as 导入方式   from 基础数据基础属性表 where 属性类别 = '导入方式'";
            DataTable dt_导入方式 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            searchLookUpEdit3.Properties.DataSource = dt_导入方式;
            searchLookUpEdit3.Properties.ValueMember = "导入方式";
            searchLookUpEdit3.Properties.DisplayMember = "导入方式";

            sql = "select  属性值 as 变更性质   from 基础数据基础属性表 where 属性类别 = '变更性质'";
            DataTable dt_变更性质 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            searchLookUpEdit4.Properties.DataSource = dt_变更性质;
            searchLookUpEdit4.Properties.ValueMember = "变更性质";
            searchLookUpEdit4.Properties.DisplayMember = "变更性质";

            sql = "select 属性值 as 变更阶段   from 基础数据基础属性表 where 属性类别 = '变更阶段'";
            DataTable dt_变更阶段 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            searchLookUpEdit7.Properties.DataSource = dt_变更阶段;
            searchLookUpEdit7.Properties.ValueMember = "变更阶段";
            searchLookUpEdit7.Properties.DisplayMember = "变更阶段";
        }

        private void fun_load()
        {
            string sql = "select 属性类别,属性值 as 变更影响的范围,属性字段1 as 说明  from 基础数据基础属性表 where 属性类别 = 'ECR变更影响'";
            dt_变更影响范围 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            DataColumn dc = new DataColumn("bl", typeof(bool));
            dc.DefaultValue = false;
            dt_变更影响范围.Columns.Add(dc);
            gridControl2.DataSource = dt_变更影响范围;




            sql = "select 属性值 as 审核部门  from  基础数据基础属性表 where 属性类别 = 'ECR变更审核部门'";
            DataTable dt1 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            dt_审核部门 = new DataTable();
            dt_审核部门.Columns.Add("审核部门",typeof(string));
            dt_审核部门.Columns.Add("部门负责人", typeof(string));
            dt_审核部门.Columns.Add("部门负责人ID", typeof(string));
            dt_审核部门.Columns.Add("审核意见", typeof(string));
            DataColumn dcc = new DataColumn("选择", typeof(bool));
            dcc.DefaultValue = false;
            dt_审核部门.Columns.Add(dcc);
            DataTable dt_姓名;
            foreach (DataRow dr in dt1.Rows)
            {
                DataRow dr_1 = dt_审核部门.NewRow();
                dt_审核部门.Rows.Add(dr_1);
                dr_1["审核部门"] = dr["审核部门"];
                sql = string.Format("select 部门领导,领导姓名 from 人事基础部门表 where 部门名称 = '{0}'", dr["审核部门"].ToString());
                dt_姓名 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);                      
                if (dt_姓名.Rows.Count > 0)
                {
                    dr_1["部门负责人ID"] = dt_姓名.Rows[0]["部门领导"];
                    dr_1["部门负责人"] = dt_姓名.Rows[0]["领导姓名"];
                }
                if (dr["审核部门"].ToString() == "公司意见")
                {
                    dr_1["选择"] = true;
                }
            }
            gridControl6.DataSource = dt_审核部门;
        }

        private void repositoryItemSearchLookUpEdit1View_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow d = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (dr == null) return;
                if (d != null)
                {
                    dr["物料名称"] = d["物料名称"];
                    dr["物料编码"] = d["物料编码"];
                    dr["规格型号"] = d["规格型号"];
                }
                else
                {
                    dr["物料名称"] = "";
                    dr["物料编码"] = "";
                    dr["规格型号"] = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dt_产品编码].EndCurrentEdit();
                this.ActiveControl = null;
                 
                fun_check();
                fun_加载();                                
                xtraTabControl1.SelectedTabPage = xtraTabPage2;
                bl_变更查询 = false;
                 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void fun_加载()
        {

            //string sql = "select * from  采购记录采购单明细表 where 1<>1";
            //dt_采购料况 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            //sql = "select * from  生产记录生产工单表 where 1<>1";
            //dt_生产料况 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            //sql = "select * from 仓库物料数量表 where 1<>1";
            //dt_库存料况 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            //DataTable dt1 = new DataTable();
            //dt1.Columns.Add("产品编码", typeof(string));
            //foreach (DataRow dr in dt_产品编码.Rows)
            //{
            //    string sql_采购 = string.Format("select * from 采购记录采购单明细表 where 物料编码 = '{0}' and 作废 = 0 and 明细完成 = 0 and 总完成 = 0", dr["物料编码"]);
            //    SqlDataAdapter da = new SqlDataAdapter(sql_采购, strconn);
            //    da.Fill(dt_采购料况);
            //    string sql_库存 = string.Format("select * from  仓库物料数量表 where 物料编码 = '{0}'", dr["物料编码"]);
            //    da = new SqlDataAdapter(sql_库存, strconn);
            //    da.Fill(dt_库存料况);
            //    string sql_父项 = string.Format("select 产品编码 from 基础数据物料BOM表 where 子项编码 = '{0}'", dr["物料编码"]);
            //    da = new SqlDataAdapter(sql_父项, strconn);
            //    da.Fill(dt1);
            //    DataRow dr1 = dt1.NewRow();
            //    dt1.Rows.Add(dr1);
            //    dr1["产品编码"] = dr["物料编码"];
            //}
            //foreach (DataRow dr in dt1.Rows)
            //{
            //    string sql_工单 = string.Format("select * from  生产记录生产工单表 where 物料编码 = '{0}' and 关闭 = 0 and 完成 = 0", dr["产品编码"]);
            //    SqlDataAdapter da = new SqlDataAdapter(sql_工单, strconn);
            //    da.Fill(dt_生产料况);
            //}
            //gridControl7.DataSource = dt_采购料况;
            //gridControl4.DataSource = dt_生产料况;
            //gridControl5.DataSource = dt_库存料况;
            DataTable dt = new DataTable();
            foreach (DataRow dr in dt_产品编码.Rows)
            {

                string sql = string.Format(@"with temp_bom(产品编码, 子项编码, 仓库号, 仓库名称, wiptype, 子项类型, 数量, bom类型, bom_level ) as
         (select 产品编码, 子项编码, 仓库号, 仓库名称, WIPType, 子项类型, 数量, bom类型,1 as level from 基础数据物料BOM表
           where 子项编码 = '{0}'
           union all
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型,a.数量,a.bom类型,b.bom_level + 1  from 基础数据物料BOM表 a
     inner join temp_bom b on a.子项编码 = b.产品编码   ) 
          select 子项编码 as 物料编码, 子项名称 as 物料名称 ,子项规格 as 规格型号   from (
  select 产品编码 as 子项编码,fx.物料名称 as 子项名称,子项编码 as 产品编码,base.物料名称 as 产品名称,wiptype,子项类型,数量,bom类型,a.仓库号,a.仓库名称
  , bom_level,fx.规格型号 as 子项规格,fx.停用 from temp_bom a
  left  join 基础数据物料信息表 base on base.物料编码 = a.子项编码
     left  join 基础数据物料信息表 fx  on fx.物料编码 = a.产品编码  )dd  
     where 停用 = 0
     group by 子项编码, 子项名称, 产品编码,  产品名称,wiptype,子项类型,数量,bom类型, 仓库号, 仓库名称, bom_level, 子项规格,停用", dr["物料编码"]);
                
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                DataRow dr1 = dt.NewRow();
                dt.Rows.Add(dr1);
                dr1["物料编码"] = dr["物料编码"];
                dr1["物料名称"] = dr["物料名称"];
                dr1["规格型号"] = dr["规格型号"];
            }
            MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();

            dt_涉及物料 = RBQ.SelectGroupByInto("", dt, "物料编码,物料名称,规格型号 ", "", "物料编码,物料名称,规格型号");
            DataColumn dc = new DataColumn("选择", typeof(bool));
            dc.DefaultValue = false;
            dt_涉及物料.Columns.Add(dc);
            gridControl3.DataSource = dt_涉及物料;

        }

        private void fun_check()
        {
             
            if(dt_产品编码.Rows.Count==0)
            {
                throw new Exception("未添加需变更的产品");
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = dt_产品编码.NewRow();
                dt_产品编码.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
                        
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                dr.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView2.CloseEditor();
                this.BindingContext[dt_变更影响范围].EndCurrentEdit();
                gridView1.CloseEditor();
                this.BindingContext[dt_产品编码].EndCurrentEdit();
                gridView6.CloseEditor();
                this.BindingContext[dt_审核部门].EndCurrentEdit();
                DateTime t = CPublic.Var.getDatetime();
                fun_check_1();
                if (bl_新增)
                {
                    str_申请单号 = string.Format("BG{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                    t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("BG", t.Year, t.Month, t.Day).ToString("0000"));
                    string sql_申请主 = "select * from ECR变更申请单主表 where 1<>1";
                    dt_变更申请主 = CZMaster.MasterSQL.Get_DataTable(sql_申请主, strconn);
                    DataRow dr_申请主 = dt_变更申请主.NewRow();
                    dt_变更申请主.Rows.Add(dr_申请主);
                    dr_申请主["申请单号"] = str_申请单号;
                    dr_申请主["申请人"] = textBox9.Text;
                    dr_申请主["申请人ID"] = CPublic.Var.LocalUserID;
                    dr_申请主["GUID"] = System.Guid.NewGuid();
                    dr_申请主["ECR提出人ID"] = searchLookUpEdit5.EditValue;
                    dr_申请主["ECR提出人"] = textBox1.Text;
                    dr_申请主["提出单位"] = textBox2.Text;
                    dr_申请主["单位主管"] = textBox4.Text;
                    dr_申请主["ECO紧急度"] = searchLookUpEdit6.EditValue;
                    dr_申请主["变更来源"] = searchLookUpEdit1.EditValue;
                    dr_申请主["变更类别"] = searchLookUpEdit2.EditValue;
                    dr_申请主["导入方式"] = searchLookUpEdit3.EditValue;
                    dr_申请主["变更性质"] = searchLookUpEdit4.EditValue;
                    dr_申请主["提出日期"] = dateEdit1.EditValue;
                    dr_申请主["期望完成日期"] = dateEdit2.EditValue;
                    dr_申请主["变更阶段"] = searchLookUpEdit7.EditValue;
                    dr_申请主["制造MES影响"] = checkBox1.Checked;
                    dr_申请主["涉及SOP变更"] = checkBox2.Checked;
                    dr_申请主["变更原因"] = textBox11.Text;
                    dr_申请主["变更内容"] = textBox10.Text;
                    dr_申请主["新旧品兼容性影响"] = textBox12.Text;
                    dr_申请主["提交审核"] = true;                    
                    dr_申请主["申请日期"] = t;

                    string sql = "select * from ECR变更申请单明细表 where 1<>1";
                    dt_变更申请子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    int i = 1;
                    foreach(DataRow dr in dt_产品编码.Rows)
                    {
                        DataRow dr_申请子 = dt_变更申请子.NewRow();
                        dt_变更申请子.Rows.Add(dr_申请子);
                        dr_申请子["申请单号"] = str_申请单号;
                        dr_申请子["POS"] = i;
                        dr_申请子["申请单明细号"] = str_申请单号 + "-" + i++.ToString("00");
                        dr_申请子["物料编码"] = dr["物料编码"];
                        dr_申请子["物料名称"] = dr["物料名称"];
                        dr_申请子["规格型号"] = dr["规格型号"];
                    }
                    sql = "select * from ECR变更申请影响范围表 where 1<>1";
                    dt_变更影响 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    foreach (DataRow dr in dt_变更影响范围.Rows)
                    {
                        DataRow dr1 = dt_变更影响.NewRow();
                        dt_变更影响.Rows.Add(dr1);
                        dr1["申请单号"] = str_申请单号;
                        dr1["变更影响的范围"] = dr["变更影响的范围"];
                        dr1["说明"] = dr["说明"];
                        dr1["bl"] = dr["bl"];
                    }
                    sql = "select * from ECR变更申请审核表 where 1<>1";
                    dt_变更审核 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    foreach (DataRow dr in dt_审核部门.Rows)
                    {
                        if (Convert.ToBoolean(dr["选择"]))
                        {
                            DataRow dr1 = dt_变更审核.NewRow();
                            dt_变更审核.Rows.Add(dr1);
                            dr1["申请单号"] = str_申请单号;
                            dr1["审核部门"] = dr["审核部门"];
                            dr1["部门负责人ID"] = dr["部门负责人ID"];
                            dr1["部门负责人"] = dr["部门负责人"];
                        }
                    }

                    sql = "select * from ECR变更申请影响物料表 where 1<>1";
                    DataTable dt_涉及物料_1 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    foreach (DataRow dr_涉及物料 in dt_涉及物料.Rows)
                    {
                        if(Convert.ToBoolean(dr_涉及物料["选择"]))
                        {
                            DataRow dr_物料 = dt_涉及物料_1.NewRow();
                            dt_涉及物料_1.Rows.Add(dr_物料);
                            dr_物料["申请单号"] = str_申请单号;
                            dr_物料["物料编码"] = dr_涉及物料["物料编码"];
                            dr_物料["物料名称"] = dr_涉及物料["物料名称"];
                            dr_物料["规格型号"] = dr_涉及物料["规格型号"];
                        }
                    }

                    sql = "select * from  ECR变更申请采购料况表 where 1<>1";
                    DataTable dt_采购料况_1 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_采购料况.Rows.Count>0)
                    {                                            
                        foreach (DataRow dr_采购料况 in dt_采购料况.Rows)
                        {
                            DataRow dr_料况 = dt_采购料况_1.NewRow();
                            dt_采购料况_1.Rows.Add(dr_料况);
                            dr_料况["申请单号"] = str_申请单号;
                            dr_料况["采购单号"] = dr_采购料况["采购单号"];
                            dr_料况["采购单明细号"] = dr_采购料况["采购明细号"];
                            dr_料况["供应商ID"] = dr_采购料况["供应商ID"];
                            dr_料况["供应商"] = dr_采购料况["供应商"];
                            dr_料况["物料编码"] = dr_采购料况["物料编码"];
                            dr_料况["物料名称"] = dr_采购料况["物料名称"];
                            dr_料况["规格型号"] = dr_采购料况["规格型号"];
                            dr_料况["采购数量"] = Convert.ToDecimal(dr_采购料况["采购数量"]);                            
                            dr_料况["完成数量"] = Convert.ToDecimal(dr_采购料况["完成数量"]);
                            dr_料况["未完成数量"] = Convert.ToDecimal(dr_采购料况["未完成数量"]);
                            dr_料况["已送检数"] = Convert.ToDecimal(dr_采购料况["已送检数"]);
                            if (dr_采购料况["到货日期"].ToString() == "")
                            {
                                dr_料况["到货日期"] = DBNull.Value;
                            }
                            else
                            {
                                dr_料况["到货日期"] = Convert.ToDateTime(dr_采购料况["到货日期"]);
                            }
                            
                            dr_料况["预计到货日期"] = Convert.ToDateTime(dr_采购料况["预计到货日期"]);
                        }
                    }
                    sql = "select * from  ECR变更申请生产料况表 where 1<>1";
                    DataTable dt_生产料况_1 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_生产料况.Rows.Count > 0)
                    {                                             
                        foreach (DataRow dr_生产料况 in dt_生产料况.Rows)
                        {
                            DataRow dr_料况 = dt_生产料况_1.NewRow();
                            dt_生产料况_1.Rows.Add(dr_料况);
                            dr_料况["申请单号"] = str_申请单号;
                            dr_料况["生产工单号"] = dr_生产料况["生产工单号"];
                            dr_料况["生产制令单号"] = dr_生产料况["生产制令单号"];
                            dr_料况["生产工单类型"] = dr_生产料况["生产工单类型"];
                            dr_料况["加急状态"] = dr_生产料况["加急状态"];
                            dr_料况["物料编码"] = dr_生产料况["物料编码"];
                            dr_料况["物料名称"] = dr_生产料况["物料名称"];
                            dr_料况["规格型号"] = dr_生产料况["规格型号"];
                            dr_料况["生产数量"] = Convert.ToDecimal(dr_生产料况["生产数量"]);
                            dr_料况["预计开工日期"] =Convert.ToDateTime(dr_生产料况["预计开工日期"]);
                            dr_料况["预计完工日期"] = Convert.ToDateTime(dr_生产料况["预计完工日期"]);
                            dr_料况["已检验数量"] = Convert.ToDecimal(dr_生产料况["已检验数量"]);
                            dr_料况["未检验数量"] = Convert.ToDecimal(dr_生产料况["未检验数量"]);
                            dr_料况["制单日期"] = Convert.ToDateTime(dr_生产料况["制单日期"]);
                            if (dr_生产料况["完工日期"].ToString() == "")
                            {
                                dr_料况["完工日期"] = DBNull.Value;
                            }
                            else
                            {
                                dr_料况["完工日期"] = Convert.ToDateTime(dr_生产料况["完工日期"]);
                            }
                            
                            dr_料况["部分完工"] = dr_生产料况["部分完工"];
                            dr_料况["部分完工数"] = Convert.ToDecimal(dr_生产料况["部分完工数"]);
                            dr_料况["完工"] = dr_生产料况["完工"];
                        }

                    }
                    sql = "select * from ECR变更申请库存料况 where 1<>1";
                    DataTable dt_库存料况_1 = CZMaster.MasterSQL.Get_DataTable(sql,strconn);                    
                    if (dt_库存料况.Rows.Count > 0)
                    {
                        foreach (DataRow dr_库存料况 in dt_库存料况.Rows)
                        {
                            DataRow dr_料况 = dt_库存料况_1.NewRow();
                            dt_库存料况_1.Rows.Add(dr_料况);
                            dr_料况["申请单号"] = str_申请单号;                           
                            dr_料况["物料编码"] = dr_库存料况["物料编码"];
                            dr_料况["物料名称"] = dr_库存料况["物料名称"];
                            dr_料况["规格型号"] = dr_库存料况["规格型号"];
                            dr_料况["库存总数"] = Convert.ToDecimal(dr_库存料况["库存总数"]);
                            dr_料况["有效库存"] = Convert.ToDecimal(dr_库存料况["有效库存"]);
                            dr_料况["在途量"] = Convert.ToDecimal(dr_库存料况["在途量"]);
                            dr_料况["在制量"] = Convert.ToDecimal(dr_库存料况["在制量"]);
                            dr_料况["受订量"] = Convert.ToDecimal(dr_库存料况["受订量"]);
                            dr_料况["未领量"] = Convert.ToDecimal(dr_库存料况["未领量"]);
                            dr_料况["仓库号"] =  dr_库存料况["仓库号"];
                            dr_料况["仓库名称"] = dr_库存料况["仓库名称"];
                             
                        }
                    }
                    sql = "select * from ECR变更申请销售料况表 where 1<>1";
                    DataTable dt_销售料况_1 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    foreach (DataRow dr_销售料况 in dt_销售料况.Rows)
                    {
                        DataRow dr_料况 = dt_销售料况_1.NewRow();
                        dt_销售料况_1.Rows.Add(dr_料况);
                        dr_料况["申请单号"] = str_申请单号;
                        dr_料况["销售订单号"] = dr_销售料况["销售订单号"];
                        dr_料况["销售订单明细号"] = dr_销售料况["销售订单明细号"];
                        dr_料况["物料编码"] = dr_销售料况["物料编码"];
                        dr_料况["物料名称"] = dr_销售料况["物料名称"];
                        dr_料况["规格型号"] = dr_销售料况["规格型号"];
                        dr_料况["数量"] = Convert.ToDecimal(dr_销售料况["数量"]);
                        dr_料况["完成数量"] = Convert.ToDecimal(dr_销售料况["完成数量"]);
                        dr_料况["未完成数量"] = Convert.ToDecimal(dr_销售料况["未完成数量"]);
                        dr_料况["生效"] = dr_销售料况["生效"];
                        if (dr_销售料况["生效日期"].ToString() == "")
                        {
                            dr_料况["生效日期"] = DBNull.Value;
                        }
                        else
                        {
                            dr_料况["生效日期"] = Convert.ToDateTime(dr_销售料况["生效日期"]);
                        }
                    }

                    sql = "select * from ECR变更申请制令料况表 where 1<>1";
                    DataTable dt_制令料况_1 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    foreach (DataRow dr_制令料况 in dt_制令料况.Rows)
                    {
                        DataRow dr_料况 = dt_制令料况_1.NewRow();
                        dt_制令料况_1.Rows.Add(dr_料况);
                        dr_料况["申请单号"] = str_申请单号;
                        dr_料况["生产制令单号"] = dr_制令料况["生产制令单号"];
                        dr_料况["生产制令类型"] = dr_制令料况["生产制令类型"];
                        dr_料况["物料编码"] = dr_制令料况["物料编码"];
                        dr_料况["物料名称"] = dr_制令料况["物料名称"];
                        dr_料况["规格型号"] = dr_制令料况["规格型号"];
                        dr_料况["制令数量"] = Convert.ToDecimal(dr_制令料况["制令数量"]);
                        dr_料况["已排单数量"] = Convert.ToDecimal(dr_制令料况["已排单数量"]);
                        dr_料况["未排单数量"] = Convert.ToDecimal(dr_制令料况["未排单数量"]);
                        dr_料况["生效"] = dr_制令料况["生效"];
                        if (dr_制令料况["预开工日期"].ToString() == "")
                        {
                            dr_料况["预开工日期"] = DBNull.Value;
                        }
                        else
                        {
                            dr_料况["预开工日期"] = Convert.ToDateTime(dr_制令料况["预开工日期"]);
                        }
                        if (dr_制令料况["预完工日期"].ToString() == "")
                        {
                            dr_料况["预完工日期"] = DBNull.Value;
                        }
                        else
                        {
                            dr_料况["预完工日期"] = Convert.ToDateTime(dr_制令料况["预完工日期"]);
                        }
                        if (dr_制令料况["生效日期"].ToString() == "")
                        {
                            dr_料况["生效日期"] = DBNull.Value;
                        }
                        else
                        {
                            dr_料况["生效日期"] = Convert.ToDateTime(dr_制令料况["生效日期"]);
                        }
                    }
                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("生效");
                    try
                    {

                        string sql1 = "select * from ECR变更申请单主表 where 1<>1";
                        SqlCommand cmd = new SqlCommand(sql1, conn, ts);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);                      
                        new SqlCommandBuilder(da);
                        da.Update(dt_变更申请主);                        

                        string sql2 = "select * from ECR变更申请单明细表 where 1<>1";
                        SqlCommand cmd1 = new SqlCommand(sql2, conn, ts);
                        SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(dt_变更申请子);

                        string sql3 = "select * from ECR变更申请影响范围表 where 1<>1";
                        SqlCommand cmd2 = new SqlCommand(sql3, conn, ts);
                        SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                        new SqlCommandBuilder(da2);
                        da2.Update(dt_变更影响);

                        string sql4 = "select * from ECR变更申请审核表 where 1<>1";
                        SqlCommand cmd3 = new SqlCommand(sql4, conn, ts);
                        SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
                        new SqlCommandBuilder(da3);
                        da3.Update(dt_变更审核);

                        if (dt_采购料况.Rows.Count>0)
                        {
                            string sql5 = "select * from ECR变更申请采购料况表 where 1<>1";
                            SqlCommand cmd4 = new SqlCommand(sql5, conn, ts);
                            SqlDataAdapter da4 = new SqlDataAdapter(cmd4);
                            new SqlCommandBuilder(da4);
                            da4.Update(dt_采购料况_1);
                        }
                        if (dt_生产料况.Rows.Count > 0)
                        {
                            string sql6 = "select * from ECR变更申请生产料况表 where 1<>1";
                            SqlCommand cmd5 = new SqlCommand(sql6, conn, ts);
                            SqlDataAdapter da5 = new SqlDataAdapter(cmd5);
                            new SqlCommandBuilder(da5);
                            da5.Update(dt_生产料况_1);
                        }
                        if (dt_库存料况.Rows.Count > 0)
                        {
                            string sql7 = "select * from ECR变更申请库存料况 where 1<>1";
                            SqlCommand cmd6 = new SqlCommand(sql7, conn, ts);
                            SqlDataAdapter da6 = new SqlDataAdapter(cmd6);
                            new SqlCommandBuilder(da6);
                            da6.Update(dt_库存料况_1);
                        }
                        if (dt_制令料况.Rows.Count > 0)
                        {
                            string sql7 = "select * from ECR变更申请制令料况表 where 1<>1";
                            SqlCommand cmd6 = new SqlCommand(sql7, conn, ts);
                            SqlDataAdapter da6 = new SqlDataAdapter(cmd6);
                            new SqlCommandBuilder(da6);
                            da6.Update(dt_制令料况_1);
                        }
                        if (dt_销售料况.Rows.Count > 0)
                        {
                            string sql7 = "select * from ECR变更申请销售料况表 where 1<>1";
                            SqlCommand cmd6 = new SqlCommand(sql7, conn, ts);
                            SqlDataAdapter da6 = new SqlDataAdapter(cmd6);
                            new SqlCommandBuilder(da6);
                            da6.Update(dt_销售料况_1);
                        }
                        if (dt_涉及物料_1.Rows.Count > 0)
                        {
                            string sql7 = "select * from ECR变更申请影响物料表 where 1<>1";
                            SqlCommand cmd6 = new SqlCommand(sql7, conn, ts);
                            SqlDataAdapter da6 = new SqlDataAdapter(cmd6);
                            new SqlCommandBuilder(da6);
                            da6.Update(dt_涉及物料_1);
                        }
                        ts.Commit();
                        MessageBox.Show("提交成功");
                        barLargeButtonItem2.Enabled = false;
                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_check_1()
        {
            if (dateEdit2.EditValue ==null||dateEdit2.EditValue.ToString() == "")
            {
                throw new Exception("期望完成日期未填");
            }
            foreach(DataRow dr in dt_审核部门.Rows)
            {
                if (Convert.ToBoolean(dr["选择"]))
                {
                    if (dr["部门负责人ID"].ToString() == "")
                    {
                        throw new Exception("未选择部门审核人");
                    }
                }
            }
            if (!bl_变更查询)
            {
                throw new Exception("物料料况没有查询，请确认");
            }
        }

         
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void searchLookUpEdit5_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string sql = string.Format("select 姓名,员工号,部门,部门编号 from  人事基础员工表 where 员工号 = '{0}'",searchLookUpEdit5.EditValue.ToString());
                DataTable dt_提出人 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (dt_提出人.Rows.Count > 0)
                {
                    textBox1.Text = dt_提出人.Rows[0]["姓名"].ToString();
                    textBox2.Text = dt_提出人.Rows[0]["部门"].ToString();
                    string sql1 = string.Format("select * from 人事基础部门表 where 部门名称 = '{0}'",textBox2.Text);
                    DataTable dt_提出部门 = CZMaster.MasterSQL.Get_DataTable(sql1, strconn);
                    if (dt_提出部门.Rows.Count > 0)
                    {
                        textBox4.Text = dt_提出部门.Rows[0]["领导姓名"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
                        
        }

        private void gridView6_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

        }

    
        

        private void gridView6_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gridView6.GetDataRow(gridView6.FocusedRowHandle);
                if (e.Column.FieldName == "部门负责人ID")
                {
                    DataRow[] dr1 = dt_负责人.Select(string.Format("员工号 = '{0}'", e.Value));
                    dr["部门负责人"] = dr1[0]["姓名"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); ;
            }
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            gridView16.CloseEditor();
            this.BindingContext[dt_涉及物料].EndCurrentEdit();
            this.ActiveControl = null;
            try
            {
                string sql = "select * from  采购记录采购单明细表 where 1<>1";
                dt_采购料况 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                sql = "select * from  生产记录生产工单表 where 1<>1";
                dt_生产料况 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                sql = "select * from 仓库物料数量表 where 1<>1";
                dt_库存料况 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                sql = "select * from 生产记录生产制令表 where 1<>1";
                dt_制令料况 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                sql = "select * from 销售记录销售订单明细表 where 1<>1";
                dt_销售料况 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                foreach (DataRow dr in dt_涉及物料.Rows)
                { 
                    if (Convert.ToBoolean(dr["选择"]))
                    {
                        string sql_采购 = string.Format("select * from 采购记录采购单明细表 where 物料编码 = '{0}' and 作废 = 0 and 明细完成 = 0 and 总完成 = 0", dr["物料编码"]);
                        SqlDataAdapter da = new SqlDataAdapter(sql_采购, strconn);
                        da.Fill(dt_采购料况);
                        string sql_库存 = string.Format("select * from  仓库物料数量表 where 物料编码 = '{0}'", dr["物料编码"]);
                        da = new SqlDataAdapter(sql_库存, strconn);
                        da.Fill(dt_库存料况);
                        string sql_工单 = string.Format("select * from  生产记录生产工单表 where 物料编码 = '{0}' and 关闭 = 0 and 完成 = 0", dr["物料编码"]);
                        da = new SqlDataAdapter(sql_工单, strconn);
                        da.Fill(dt_生产料况);
                        string sql_制令 = string.Format("select * from  生产记录生产制令表 where 物料编码 = '{0}' and 关闭 = 0 and 完成 = 0", dr["物料编码"]);
                        da = new SqlDataAdapter(sql_制令, strconn);
                        da.Fill(dt_制令料况);
                        string sql_销售 = string.Format("select * from  销售记录销售订单明细表 where 物料编码 = '{0}' and 作废 = 0 and 明细完成 = 0", dr["物料编码"]);
                        da = new SqlDataAdapter(sql_销售, strconn);
                        da.Fill(dt_销售料况);
                    }
                }
                gridControl8.DataSource = dt_销售料况;
                gridControl7.DataSource = dt_采购料况;
                gridControl9.DataSource = dt_制令料况;
                gridControl4.DataSource = dt_生产料况;
                gridControl5.DataSource = dt_库存料况;
                bl_变更查询 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

 
    }
}
