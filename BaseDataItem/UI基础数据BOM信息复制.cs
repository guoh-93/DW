using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace BaseData
{
    public partial class UI基础数据BOM信息复制 : UserControl
    {
        #region  变量
        string str_物料编码;
        string strcon = CPublic.Var.strConn;
        DataTable dt_源;
        DataTable dt_源_BOM;
        DataTable dt_中间_BOM = new DataTable();
        DataTable dt_下拉_bom;
        DataTable dt_中间_包装清单 = new DataTable();
        DataTable dt_右 = new DataTable();
        DataTable dt_主辅料;
        DataTable dt_仓库;


        public static DevExpress.XtraTab.XtraTabControl XTC;

        //DataTable dt_下拉_包装清单;

        DataTable dt_源_包装清单;

        #endregion

        #region 加载

        public UI基础数据BOM信息复制()
        {
            InitializeComponent();
        }
        string cfgfilepath = "";
        public UI基础数据BOM信息复制(string str_物料编码)
        {
            InitializeComponent();
            this.str_物料编码 = str_物料编码;

        }
        private void UI基础数据BOM信息复制_Load(object sender, EventArgs e)
        {
            try
            {

                fun_load();

                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(this.splitContainer1, this.Name, cfgfilepath);
                x.UserLayout(this.panel3, this.Name, cfgfilepath);
                if (str_物料编码 != "")
                {
                    searchLookUpEdit1.EditValue = str_物料编码;
                }






            }
            catch (Exception)
            {

                throw;
            }




        }


        #endregion

        #region 函数
        //选择成品 复制源   跳出对应的一级BOM信息 和包装清单 
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue != null)
            {
                string sql = string.Format(@"select bom.*,规格型号,组,优先级,base.大类,base.图纸编号,base.小类
                                    from  基础数据物料BOM表 bom left join 基础数据物料信息表 base on bom.子项编码=base.物料编码   
                                       where  bom.产品编码='{0}'", searchLookUpEdit1.EditValue.ToString());

                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    dt_源_BOM = new DataTable();
                    da.Fill(dt_源_BOM);
                    dt_源_BOM.Columns.Add("选择", typeof(bool));
                    gridControl1.DataSource = dt_源_BOM;
                    //dt_中间_BOM = dt_源_BOM.Clone();

                }
                string sql1 = string.Format("select * from 基础数据包装清单表 where 成品编码='{0}'", searchLookUpEdit1.EditValue);
                using (SqlDataAdapter da = new SqlDataAdapter(sql1, strcon))
                {
                    dt_源_包装清单 = new DataTable();
                    da.Fill(dt_源_包装清单);
                    dt_源_包装清单.Columns.Add("选择", typeof(bool));
                    gridControl2.DataSource = dt_源_包装清单;
                    //dt_中间_包装清单 = dt_源_包装清单.Clone();

                }

                // string sql_大类 = string.Format(" select *  from 基础数据物料信息表 where 物料编码='{0}'", searchLookUpEdit1.EditValue);
                string sql_存货分类编码 = string.Format(" select * from 基础数据物料信息表 where 物料编码='{0}'", searchLookUpEdit1.EditValue);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql_存货分类编码, strcon);
                if (dt.Rows.Count == 0)
                {
                    textBox3.Text = "";
                    textBox4.Text = "";
                }
                else
                {
                    textBox3.Text = dt.Rows[0]["物料名称"].ToString();
                    textBox4.Text = dt.Rows[0]["规格型号"].ToString().Trim();

                    //string s = dt.Rows[0]["存货分类编码"].ToString();
                    //s = s.Substring(0, s.Length - 2);
                    /// where left(存货分类编码,LEN(存货分类编码)-2)='{0}' 
                    /// 19-6-17 用户要求 不需要限制
                    string sql_右 = string.Format(@"select 物料编码,物料名称,规格型号,存货分类 from 基础数据物料信息表 where 停用=0 and (自制=1 or ( 可购=1 and 委外=1)) ");
                    using (SqlDataAdapter da = new SqlDataAdapter(sql_右, strcon))
                    {
                        dt_右 = new DataTable();
                        da.Fill(dt_右);
                        gridControl5.DataSource = dt_右;
                    }
                }
            }
        }
        DataTable dt_unit = new DataTable();
        private void fun_load()
        {
            try
            {
                //主辅料
                string sql_主辅料 = "select  属性类别,属性值  from [基础数据基础属性表] where  属性类别='主辅料'";

                dt_主辅料 = new DataTable();
                dt_主辅料 = CZMaster.MasterSQL.Get_DataTable(sql_主辅料, strcon);
                repositoryItemSearchLookUpEdit3.DataSource = dt_主辅料;
                repositoryItemSearchLookUpEdit3.DisplayMember = "属性值";
                repositoryItemSearchLookUpEdit3.ValueMember = "属性值";
                for (int i = 0; i < searchLookUpEdit1View.Columns.Count; i++)
                {
                    searchLookUpEdit1View.Columns[i].BestFit();
                }

                //源 下拉框
                string sql = @"select base.物料编码,base.物料名称,base.规格型号,base.物料类型
                    from 基础数据物料信息表 base  left join (SELECT [产品编码]  FROM [基础数据物料BOM表] group by 产品编码  )a
                    on base.物料编码=a.产品编码 /* where base.物料类型='成品' or base.物料类型='半成品'*/";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {

                    dt_源 = new DataTable();
                    da.Fill(dt_源);
                    searchLookUpEdit1.Properties.DataSource = dt_源;
                    searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                    searchLookUpEdit1.Properties.ValueMember = "物料编码";

                    //gridControl5.DataSource = dt_源;
                }

                //中间的

                /* left join ( SELECT [子项编码]   FROM [基础数据物料BOM表] group by 子项编码 ) a
                      on  基础数据物料信息表.物料编码 = a.子项编码  where 基础数据物料信息表.物料类型='原材料' or 基础数据物料信息表.物料类型='半成品' */
                string sql_bom_下拉 = @"select 物料编码,物料名称,规格型号,图纸编号,物料类型,大类,小类 from 基础数据物料信息表   
                    where  停用=0   order by 物料编码";
                 
                using (SqlDataAdapter da = new SqlDataAdapter(sql_bom_下拉, strcon))
                {
                    dt_下拉_bom = new DataTable();
                    da.Fill(dt_下拉_bom);


                    repositoryItemSearchLookUpEdit1.DataSource = dt_下拉_bom;
                    repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
                    repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";

                    repositoryItemSearchLookUpEdit2.DataSource = dt_下拉_bom;
                    repositoryItemSearchLookUpEdit2.DisplayMember = "物料编码";
                    repositoryItemSearchLookUpEdit2.ValueMember = "物料编码";


                }
                string sql_1 = string.Format(@"select 基础数据物料BOM表.*,规格型号,图纸编号  from 基础数据物料BOM表 
                                            left join  基础数据物料信息表 on 基础数据物料BOM表.子项编码=基础数据物料信息表.物料编码 where 1<>1");
                using (SqlDataAdapter da = new SqlDataAdapter(sql_1, strcon))
                {
                    dt_中间_BOM = new DataTable();



                    da.Fill(dt_中间_BOM);

                    gridControl3.DataSource = dt_中间_BOM;


                }
                string sql1 = string.Format("select * from 基础数据包装清单表 where 1<>1");
                using (SqlDataAdapter da = new SqlDataAdapter(sql1, strcon))
                {
                    dt_中间_包装清单 = new DataTable();
                    da.Fill(dt_中间_包装清单);

                    gridControl4.DataSource = dt_中间_包装清单;


                }

                string sql2 = string.Format(@"select 基础数据物料BOM表.*,规格型号,图纸编号  from  基础数据物料BOM表 left join 基础数据物料信息表  
                 on 基础数据物料信息表.物料编码=基础数据物料BOM表.子项编码     where  1<>1");
                using (SqlDataAdapter da = new SqlDataAdapter(sql2, strcon))
                {
                    dt_源_BOM = new DataTable();
                    da.Fill(dt_源_BOM);
                    dt_源_BOM.Columns.Add("选择", typeof(bool));
                    gridControl1.DataSource = dt_源_BOM;
                    //dt_中间_BOM = dt_源_BOM.Clone();

                }
                string sql3 = string.Format("select * from 基础数据包装清单表 where 1<>1");
                using (SqlDataAdapter da = new SqlDataAdapter(sql3, strcon))
                {
                    dt_源_包装清单 = new DataTable();
                    da.Fill(dt_源_包装清单);
                    dt_源_包装清单.Columns.Add("选择", typeof(bool));
                    gridControl2.DataSource = dt_源_包装清单;
                    //dt_中间_包装清单 = dt_源_包装清单.Clone();

                }
                string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别' and 布尔字段4 = 1";
                dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql4, strcon);
                repositoryItemSearchLookUpEdit4.DataSource = dt_仓库;
                repositoryItemSearchLookUpEdit4.DisplayMember = "仓库号";
                repositoryItemSearchLookUpEdit4.ValueMember = "仓库号";

                sql = "select 属性值 from 基础数据基础属性表 where 属性类别 = 'BOM子项类型'";

                DataTable dt_子项类型 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);


                sql = "select 属性值 from 基础数据基础属性表 where 属性类别 = 'BOM类型'";

                DataTable dt_BOM类型 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);




                repositoryItemGridLookUpEdit2.DataSource = dt_子项类型;
                repositoryItemGridLookUpEdit2.DisplayMember = "属性值";
                repositoryItemGridLookUpEdit2.ValueMember = "属性值";

                repositoryItemGridLookUpEdit3.DataSource = dt_BOM类型;
                repositoryItemGridLookUpEdit3.DisplayMember = "属性值";
                repositoryItemGridLookUpEdit3.ValueMember = "属性值";

                sql = "select 属性值 as 计量单位,属性字段1 as 计量单位编码 from 基础数据基础属性表 where 属性类别 = '计量单位'";

                dt_unit = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

                repositoryItemGridLookUpEdit1.DataSource = dt_unit;
                repositoryItemGridLookUpEdit1.DisplayMember = "计量单位编码";
                repositoryItemGridLookUpEdit1.ValueMember = "计量单位编码";

                sql = "select 属性值 as 领料类型 from 基础数据基础属性表 where 属性类别 = 'WIPType'";
                DataTable dt_领料类型 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                repositoryItemGridLookUpEdit4.DataSource = dt_领料类型;
                repositoryItemGridLookUpEdit4.DisplayMember = "领料类型";
                repositoryItemGridLookUpEdit4.ValueMember = "领料类型";





            }
            catch (Exception ex)
            {

                CZMaster.MasterLog.WriteLog(ex.Message, "Load");

            }


        }
        private void fun_save()
        {
            string sql_bom = "select * from 基础数据物料BOM表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_bom, strcon))
            {
                new SqlCommandBuilder(da);
                da.Update(dt_中间_BOM);
            }
            //string sql_包装清单 = "select * from 基础数据包装清单表 where 1<>1";
            //using (SqlDataAdapter da = new SqlDataAdapter(sql_包装清单, strcon))
            //{
            //    new SqlCommandBuilder(da);
            //    da.Update(dt_中间_包装清单);
            //}
        }


        //双击最右边 中间出详细信息跟下拉框效果一样
        private void gridView5_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Clicks == 2)
            {
                DataRow dr = gridView5.GetDataRow(gridView5.FocusedRowHandle);
                textBox1.Text = dr["物料编码"].ToString();
                textBox2.Text = dr["物料名称"].ToString();
                textBox5.Text = dr["规格型号"].ToString();
                string sql = string.Format(@"select 基础数据物料BOM表.*,规格型号,图纸编号  from 基础数据物料BOM表 
                                            left join  基础数据物料信息表 on 基础数据物料BOM表.子项编码=基础数据物料信息表.物料编码 where 产品编码='{0}'", dr["物料编码"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    dt_中间_BOM = new DataTable();



                    da.Fill(dt_中间_BOM);

                    gridControl3.DataSource = dt_中间_BOM;


                }
                string sql1 = string.Format("select * from 基础数据包装清单表 where 成品编码='{0}'", dr["物料编码"].ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(sql1, strcon))
                {
                    dt_中间_包装清单 = new DataTable();
                    da.Fill(dt_中间_包装清单);

                    gridControl4.DataSource = dt_中间_包装清单;


                }
                gridView2.Focus();
                gridView1.Focus();


            }
        }


        //勾选到中间BOM
        //private void repositoryItemCheckEdit3_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        //{
        //    //if (textBox1.Text.ToString() == "")
        //    //{
        //    //    MessageBox.Show("请先选择要修改BOM或包装清单信息的产品");
        //    //    return;
        //    //}
        //    //DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);


        //    //if (e != null && e.NewValue.Equals(true))
        //    //{

        //    //    if (dt_中间_BOM.Select(string.Format("子项编码='{0}'", r["子项编码"])).Length > 0)
        //    //    {
        //    //        return;
        //    //    }
        //    //    DataRow dr = dt_中间_BOM.NewRow();
        //    //    dr["子项编码"] = r["子项编码"];
        //    //    dr["子项名称"] = r["子项名称"];
        //    //    dr["规格"] = r["规格"];
        //    //    dr["图纸编号"] = r["图纸编号"];
        //    //    dr["数量"] = r["数量"];
        //    //    dr["包装数量"] = r["包装数量"];
        //    //    dr["总装数量"] = r["总装数量"];

        //    //    dr["产品编码"] = textBox1.Text;
        //    //    dr["产品名称"] = textBox2.Text;

        //    //    dr["主辅料"] = r["主辅料"];
        //    //    dr["子项类型"] = r["子项类型"];

        //    //    dr["组"] = r["组"];
        //    //    dr["优先级"] = r["优先级"];


        //    //    dr["计量单位"] = r["计量单位"];
        //    //    dr["BOM类型"] = r["BOM类型"];

        //    //    dr["修改人"] = CPublic.Var.localUserName;
        //    //    dr["修改人员ID"] = CPublic.Var.LocalUserID;
        //    //    dr["修改日期"] = System.DateTime.Now;
        //    //    dt_中间_BOM.Rows.Add(dr);
        //    //}
        //    //else
        //    //{
        //    //    if (dt_中间_BOM.Select(string.Format("子项编码='{0}'", r["子项编码"])).Length > 0)
        //    //    {
        //    //        dt_中间_BOM.Select(string.Format("子项编码='{0}'", r["子项编码"]))[0].Delete();
        //    //    }


        //    //}
        //}
        //包装清单
        private void repositoryItemCheckEdit2_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {

            if (textBox1.Text.ToString() == "")
            {
                MessageBox.Show("请先选择要修改BOM的产品");
                return;
            }
            DataRow r = gridView2.GetDataRow(gridView2.FocusedRowHandle);

            if (e.NewValue != null && e.NewValue.Equals(true))
            {
                if (dt_中间_包装清单.Select(string.Format("物料编码='{0}'", r["物料编码"])).Length > 0)
                {
                    return;
                }
                DataRow dr = dt_中间_包装清单.NewRow();
                dr["GUID"] = System.Guid.NewGuid();
                dr["物料编码"] = r["物料编码"];
                dr["物料名称"] = r["物料名称"];
                dr["成品编码"] = textBox1.Text;
                dr["成品名称"] = textBox2.Text;
                dr["数量"] = r["数量"];
                dr["图纸编号"] = r["图纸编号"];

                dr["大类"] = r["大类"];
                dr["小类"] = r["小类"];

                dr["备注"] = r["备注"];
                dr["规格型号"] = r["规格型号"];

                dt_中间_包装清单.Rows.Add(dr);
            }
            else
            {
                if (dt_中间_包装清单.Select(string.Format("物料编码='{0}'", r["物料编码"])).Length > 0)
                {
                    dt_中间_包装清单.Select(string.Format("物料编码='{0}'", r["物料编码"]))[0].Delete();
                }


            }
            gridControl4.DataSource = dt_中间_包装清单;
        }

        //BOM 信息 改变 子项编号

        private void repositoryItemSearchLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (e.NewValue != null)
            {
                string sql = string.Format("select * from 基础数据物料信息表 where 物料编码='{0}'", e.NewValue);
                DataRow rl = CZMaster.MasterSQL.Get_DataRow(sql, strcon);

                DataRow r = gridView3.GetDataRow(gridView3.FocusedRowHandle);

                r["子项编码"] = rl["物料编码"];
                r["仓库号"] = rl["仓库号"];
                r["仓库名称"] = rl["仓库名称"];
                r["子项名称"] = rl["物料名称"];
                r["规格型号"] = rl["规格型号"];
                r["图纸编号"] = rl["图纸编号"];
                //r["数量"] = rl["数量"];
                r["产品编码"] = textBox1.Text;
                r["产品名称"] = textBox2.Text;
                r["主辅料"] = rl["主辅料"];
                if (r["主辅料"].ToString() == "")
                {
                    r["主辅料"] = "主料";
                }
                if (Convert.ToBoolean(rl["自制"]))
                { r["子项类型"] = "生产件"; }
                else
                {
                    r["子项类型"] = "采购件";
                }
                //采购件 生产件
                r["计量单位"] = rl["计量单位"];
                r["计量单位编码"] = rl["计量单位编码"];

                r["BOM类型"] = "物料BOM";

                r["修改人员"] = CPublic.Var.localUserName;
                r["修改人员ID"] = CPublic.Var.LocalUserID;
                r["修改日期"] = CPublic.Var.getDatetime();
                r["WIPType"] = "领料";

            }
        }
        //包装清单  修改物料编码下拉框
        private void repositoryItemSearchLookUpEdit2_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (e.NewValue != null)
            {
                string sql = string.Format("select * from 基础数据物料信息表 where 物料编码='{0}'", e.NewValue);
                DataRow rl = CZMaster.MasterSQL.Get_DataRow(sql, strcon);

                DataRow r = gridView4.GetDataRow(gridView4.FocusedRowHandle);


                r["GUID"] = System.Guid.NewGuid();
                r["物料编码"] = rl["物料编码"];
                r["物料名称"] = rl["物料名称"];
                r["规格型号"] = rl["规格型号"];
                r["图纸编号"] = rl["图纸编号"];
                r["大类"] = rl["大类"];
                r["小类"] = rl["小类"];
                //r["数量"] = rl["数量"];

                r["成品编码"] = textBox1.Text;
                r["成品名称"] = textBox2.Text;

                //r["主辅料"] = r["主辅料"];
                //r["子项类型"] = rl["子项类型"];

                //r["计量单位"] = r["计量单位"];
                //r["BOM类型"] = r["BOM类型"];


            }
        }

        private void fun_check()
        {
            foreach (DataRow dr in dt_中间_BOM.Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                {
                    continue;
                }
                if (dr["主辅料"].ToString() == "")
                {
                    throw new Exception("主辅料不能为空");
                }
                if (dr["包装数量"].ToString() == "")
                {
                    throw new Exception("包装数量不能为空");
                }
                if (dr["总装数量"].ToString() == "")
                {
                    throw new Exception("总装数量不能为空");
                }
                if (Convert.ToDecimal(dr["总装数量"].ToString()) <= 0)
                {
                    throw new Exception("总装数量不能小于零");
                }
            }
        }
        #endregion



        #region 按钮
        //增加 BOM信息
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            dt_中间_BOM.NewRow();
            dt_中间_BOM.Rows.Add();
            gridView3.FocusedRowHandle = dt_中间_BOM.Rows.Count - 1;
        }

        //删除中间 BOM
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            //gridView3.GetDataRow(gridView3.FocusedRowHandle).Delete();
            try
            {
                DataRow dr = gridView3.GetDataRow(gridView3.FocusedRowHandle);
                if (MessageBox.Show(string.Format("是否确认删除{0}？", dr["子项编码"].ToString() + "--" + dr["子项名称"].ToString()), "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        if (Convert.ToDecimal(dr["包装数量"]) > 0)
                        {
                            DataRow[] ds = dt_中间_包装清单.Select(string.Format("物料编码 = '{0}'", dr["子项编码"].ToString()));
                            if (ds.Length > 0)
                            {
                                ds[0].Delete();
                            }
                        }
                    }
                    catch { }
                    dr.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        //增加 包装清单
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            dt_中间_包装清单.NewRow();
            dt_中间_包装清单.Rows.Add();
            gridView4.FocusedRowHandle = dt_中间_包装清单.Rows.Count - 1;

        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            gridView4.GetDataRow(gridView4.FocusedRowHandle).Delete();

        }
        //关闭 
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (XTC.TabPages.Count == 1) { }
            if (XTC.SelectedTabPage.Text == "首页") { }
            DevExpress.XtraTab.XtraTabPage xtp = null;
            try
            {
                xtp = XTC.SelectedTabPage;
                XTC.SelectedTabPageIndex = XTC.SelectedTabPageIndex - 1;
            }
            catch { }
            try
            {
                xtp.Controls[0].Dispose();
                XTC.TabPages.Remove(xtp);
                xtp.Dispose();
            }
            catch { }
        }
        //保存
        DataTable dt_修改主;
        DataTable dt_BOM修改记录;
        private void fun_加载BOM修改表数据()
        {
            string strt = string.Format("select * from 基础数据BOM修改明细表 where 产品编码 ='{0}' and 审核=0 order by BOM修改明细号", textBox1.Text.ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(strt, strcon))
            {
                dt_BOM修改记录 = new DataTable();
                da.Fill(dt_BOM修改记录);
                // gcc1.DataSource = dt_BOM修改记录;
                //dt_BOM修改记录.ColumnChanged += dt_BOM修改记录_ColumnChanged;
            }
        }
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {


                gridView4.CloseEditor();
                gridView3.CloseEditor();
                this.BindingContext[dt_中间_BOM].EndCurrentEdit();
                fun_加载BOM修改表数据();
                //if (searchLookUpEdit1.EditValue.ToString() != "" && textBox1.Text != "")
                if (textBox1.Text != "")
                {
                    fun_check();
                    fun_检查并保存包装清单();
                    //   fun_save();

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
                        dr["提交"] = true;

                    }
                    //单据申请
                    fun_单据审核申请();
                    fun_事务保存();

                    textBox1.Text = "";
                    textBox2.Text = "";
                    searchLookUpEdit1.EditValue = "";
                    fun_load();

                    MessageBox.Show("ok");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);


            }



        }

        DataTable dt_BOM审核申请;
        private void fun_BOM修改主子表保存()
        {
            //BOM修改 生成单号没有生成单号的时候    主表
            using (SqlDataAdapter da = new SqlDataAdapter("select * from 基础数据BOM修改主表 where 产品编码 ='" + textBox1.Text.ToString() + "'and 审核 = 0", strcon))
            {
                dt_修改主 = new DataTable();
                da.Fill(dt_修改主);
            }
            string a = "";
            DataTable d_fuu = dt_中间_BOM.Copy();
            if (dt_修改主.Rows.Count == 0)
            {
                DataRow dr_改主 = dt_修改主.NewRow();
                DateTime t = CPublic.Var.getDatetime();
                a = string.Format("BOMX{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("BOMX", t.Year, t.Month));
                dr_改主["GUID"] = System.Guid.NewGuid();
                dr_改主["BOM修改单号"] = a;
                dr_改主["产品编码"] = textBox1.Text.ToString();
                dr_改主["产品名称"] = textBox2.Text;
                dr_改主["规格型号"] = textBox5.Text;
                dr_改主["修改人员"] = CPublic.Var.localUserName;
                dr_改主["修改人员ID"] = CPublic.Var.LocalUserID;
                dr_改主["修改日期"] = t;
                dt_修改主.Rows.Add(dr_改主);
                //BOM保存子表
                int i = 1;

                d_fuu.Columns.Add("BOM修改单号", typeof(string));
                d_fuu.Columns.Add("BOM修改明细号", typeof(string));
                d_fuu.Columns.Add("GUID", typeof(string));
                d_fuu.Columns.Add("子项名称r", typeof(string));
                d_fuu.Columns.Add("货架描述", typeof(string));
                d_fuu.Columns.Add("审核", typeof(bool));
                d_fuu.Columns.Add("提交", typeof(bool));
                foreach (DataRow dr2 in d_fuu.Rows)
                {
                    if (dr2.RowState == DataRowState.Deleted) continue;


                    DataRow[] ds = dt_BOM修改记录.Select(string.Format("子项编码 = '{0}'", dr2["子项编码"].ToString()));
                    if (ds.Length == 0)
                    {

                        //DataRow dr = dt_BOM修改记录.NewRow();
                        dr2["BOM修改单号"] = a;
                        dr2["BOM修改明细号"] = a + "-" + i.ToString("00");
                        dr2["GUID"] = System.Guid.NewGuid();
                        dr2["子项名称r"] = dr2["子项名称"].ToString();

                        string sql = string.Format("select 货架描述 from 仓库物料数量表 where 物料编码='{0}' and 仓库号='{1}'", dr2["子项编码"], dr2["仓库号"]);
                        DataRow drm = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                        if (drm != null)
                        {
                            dr2["货架描述"] = drm["货架描述"];
                        }

                        //dr["产品名称"] = dr2["产品名称"];
                        //dr["子项编码"] = dr2["子项编码"];
                        //dr["子项名称"] = dr2["子项名称"];
                        dr2["审核"] = false;
                        dr2["提交"] = true;
                        i++;
                        dt_BOM修改记录.ImportRow(dr2);
                    }


                }
            }
            else
            {

                throw new Exception("当前物料有未审bom修改");
            }
            //    a = dt_修改主.Rows[0]["BOM修改单号"].ToString();
            //    dt_修改主.Rows[0]["修改人员"] = CPublic.Var.localUserName;
            //    dt_修改主.Rows[0]["修改人员ID"] = CPublic.Var.LocalUserID;
            //    dt_修改主.Rows[0]["修改日期"] = CPublic.Var.getDatetime();
            //    int i = 1;
            //    foreach (DataRow drr in d_fuu.Rows)
            //    {
            //        if (drr.RowState == DataRowState.Deleted) continue;

            //        DataRow[] ds = dt_BOM修改记录.Select(string.Format("子项编码 = '{0}'  order by BOM修改明细号", drr["子项编码"].ToString()));
            //        if (ds.Length == 0)
            //        {
            //            //DataRow dr = dt_BOM修改记录.NewRow();
            //            //dr = drr;

            //            drr["BOM修改单号"] = a;
            //            drr["BOM修改明细号"] = a + "-" + i.ToString("00");
            //            if (drr["GUID"].ToString() == "")
            //            {
            //                drr["GUID"] = System.Guid.NewGuid();
            //            }
            //            drr["子项名称r"] = drr["子项名称"].ToString();

            //            string sql = string.Format("select 货架描述 from 仓库物料数量表 where 物料编码='{0}' and 仓库号='{1}'", drr["子项编码"], drr["仓库号"]);
            //            DataRow drm = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
            //            if (drm != null)
            //            {
            //                drr["货架描述"] = drm["货架描述"];
            //            }
            //            i++;
            //            dt_BOM修改记录.ImportRow(drr);
            //        }
            //        //else
            //        //{
            //        //    ds[0]["数量"] = drr["数量"];
            //        //    ds[0]["计量单位编码"] = drr["计量单位编码"];
            //        //    ds[0]["计量单位"] = drr["计量单位"];
            //        //}


            //    }

            //}


        }
        private void fun_单据审核申请()
        {
            //"生效","BOM修改申请","str_物料编码,"生产一厂"
            dt_BOM审核申请 = ERPorg.Corg.fun_PA("生效", "BOM修改申请", dt_修改主.Rows[0]["BOM修改单号"].ToString(), "生产一厂");
            if (dt_BOM审核申请.Rows[0]["作废"].Equals(true))
            {
                dt_BOM审核申请.Rows[0]["作废"] = false;
            }
        }
        private void fun_事务保存()
        {
            SqlConnection conn = new SqlConnection(strcon);
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
        private void fun_检查并保存包装清单()
        {
            if (dt_中间_包装清单.Rows.Count == 0)
            {
                foreach (DataRow r in dt_中间_BOM.Rows)
                {
                    //if (r.RowState == DataRowState.Added)
                    //{
                    //    if (r["主辅料"] == null || r["主辅料"].ToString() == "")
                    //    {
                    //        throw new Exception("请先选择主辅料");
                    //    }
                    //}
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (Convert.ToDecimal(r["包装数量"]) != 0)
                    {
                        //DataRow[] ds = dt_包装.Select(string.Format("物料编码 = '{0}'", r["子项编码"].ToString()));
                        DataRow dr = dt_中间_包装清单.NewRow();
                        dt_中间_包装清单.Rows.Add(dr);
                        dr["GUID"] = System.Guid.NewGuid();
                        dr["成品编码"] = textBox1.Text;
                        dr["成品名称"] = textBox2.Text;
                        dr["物料编码"] = r["子项编码"];
                        dr["物料名称"] = r["子项名称"];
                        dr["数量"] = r["包装数量"];
                        string s = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", r["子项编码"].ToString().Trim());
                        SqlDataAdapter a = new SqlDataAdapter(s, strcon);
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
                foreach (DataRow r in dt_中间_BOM.Rows)
                {
                    //if (r.RowState == DataRowState.Added)
                    //{
                    //    if (r["主辅料"] == null || r["主辅料"].ToString() == "")
                    //    {
                    //        throw new Exception("请先选择主辅料");
                    //    }
                    //}
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (Convert.ToDecimal(r["包装数量"]) != 0)
                    {
                        DataRow[] ds = dt_中间_包装清单.Select(string.Format("物料编码 = '{0}'", r["子项编码"].ToString()));
                        if (ds.Length == 0)
                        {
                            DataRow dr = dt_中间_包装清单.NewRow();
                            dt_中间_包装清单.Rows.Add(dr);
                            dr["GUID"] = System.Guid.NewGuid();
                            dr["成品编码"] = textBox1.Text;
                            dr["成品名称"] = textBox2.Text;
                            dr["物料编码"] = r["子项编码"];
                            dr["物料名称"] = r["子项名称"];
                            dr["数量"] = r["包装数量"];
                            string s = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", r["子项编码"].ToString().Trim());
                            SqlDataAdapter a = new SqlDataAdapter(s, strcon);
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
        #endregion


        //控制 左边 BOM信息 若 中间的BOM表中 有 则变灰 
        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (e == null)
                {

                }
                else if (e.RowHandle > -1)
                {

                    DataRow dr = gridView1.GetDataRow(e.RowHandle);

                    if (dt_中间_BOM.Select(string.Format("子项编码='{0}'", dr["子项编码"])).Length > 0)
                    {
                        e.Appearance.BackColor = Color.Gray;

                    }
                    else
                    {
                        if (e.RowHandle % 2 == 0)
                        {
                            //  e.Appearance.BackColor = Color.LimeGreen;
                            // e.Appearance.BackColor = Color.LightBlue;

                        }

                    }

                    return;

                }
            }
            catch
            {


            }

        }

        //控制 左边 包装清单信息 若 中间的包装清单表中 有 则变灰 

        private void gridView2_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e == null)
            {

            }
            else if (e.RowHandle > -1)
            {

                DataRow dr = gridView2.GetDataRow(e.RowHandle);

                if (dt_中间_包装清单.Select(string.Format("物料编码='{0}'", dr["物料编码"])).Length > 0)
                {
                    e.Appearance.BackColor = Color.Gray;

                }
                else
                {
                    e.Appearance.BackColor = Color.White;
                }

                return;

            }
        }
        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            UI基础数据BOM信息复制_Load(null, null);
            searchLookUpEdit1.EditValue = "";
            gridControl5.DataSource = new DataTable();
        }

        //private void gridView3_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        //{
        //    DataRow dr = gridView3.GetDataRow(e.RowHandle);
        //    if (e.Column.Caption == "包装数量" || e.Column.Caption == "总装数量")
        //    {

        //        if (dr["包装数量"] == DBNull.Value)
        //            dr["包装数量"] = 0;
        //        if (dr["总装数量"] == DBNull.Value)
        //            dr["总装数量"] = 0;
        //        dr["数量"] = Convert.ToDecimal(dr["包装数量"]) + Convert.ToDecimal(dr["总装数量"]);
        //    }
        //    else if (e.Column.Caption == "仓库号")
        //    {
        //        DataRow[] rr = dt_仓库.Select(string.Format("仓库号='{0}'", dr["仓库号"]));
        //        if (rr.Length > 0) dr["仓库名称"] = rr[0]["仓库名称"];
        //    }
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                gridView1.GetDataRow(i)["选择"] = true;

                gridView1.FocusedRowHandle = gridView1.LocateByDisplayText(0, gridColumn3, gridView1.GetDataRow(i)["子项编码"].ToString());
                repositoryItemCheckEdit1_CheckedChanged(null, null);
            }
        }



        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            gridView1.CloseEditor();

            gridControl1.BindingContext[dt_源_BOM].EndCurrentEdit();

            if (textBox1.Text.ToString() == "")
            {
                MessageBox.Show("请先选择要修改BOM的产品");
                return;
            }
            DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);


            if (r["选择"].Equals(true))
            {

                if (dt_中间_BOM.Select(string.Format("子项编码='{0}'", r["子项编码"])).Length > 0)
                {
                    return;
                }
                DataRow dr = dt_中间_BOM.NewRow();
                dr["子项编码"] = r["子项编码"];
                dr["子项名称"] = r["子项名称"];
                dr["规格型号"] = r["规格型号"];
                dr["图纸编号"] = r["图纸编号"];
                dr["数量"] = r["数量"];
                dr["包装数量"] = r["包装数量"];
                dr["总装数量"] = r["总装数量"];
                dr["A面位号"] = r["A面位号"];
                dr["B面位号"] = r["B面位号"];
                dr["产品编码"] = textBox1.Text;
                dr["产品名称"] = textBox2.Text;
                dr["主辅料"] = r["主辅料"];

                dr["子项类型"] = r["子项类型"]; //采购件 生产件
                dr["组"] = r["组"];
                dr["优先级"] = r["优先级"];
                dr["WIPType"] = r["WIPType"];
                dr["A面位号"] = r["A面位号"];

                dr["仓库号"] = r["仓库号"];
                dr["仓库名称"] = r["仓库名称"];
                dr["计量单位编码"] = r["计量单位编码"];
                dr["计量单位"] = r["计量单位"];
                dr["BOM类型"] = r["BOM类型"];

                dr["修改人员"] = CPublic.Var.localUserName;
                dr["修改人员ID"] = CPublic.Var.LocalUserID;
                dr["修改日期"] = CPublic.Var.getDatetime();
                dt_中间_BOM.Rows.Add(dr);
            }
            else
            {
                if (dt_中间_BOM.Select(string.Format("子项编码='{0}'", r["子项编码"])).Length > 0)
                {
                    dt_中间_BOM.Select(string.Format("子项编码='{0}'", r["子项编码"]))[0].Delete();
                }


            }
        }

        private void gridView3_ColumnFilterChanged(object sender, EventArgs e)
        {
            try
            {
                if (cfgfilepath != "")
                {
                    gridView3.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        private void gridView3_ColumnPositionChanged(object sender, EventArgs e)
        {
            try
            {
                if (cfgfilepath != "")
                {
                    gridView3.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void gridView3_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow dr = gridView3.GetDataRow(e.RowHandle);
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
                dr["数量"] = Convert.ToDecimal(e.Value) + Convert.ToDecimal(dr["包装数量"]);
            }
            else if (e.Column.Caption == "仓库号")
            {
                DataRow[] rr = dt_仓库.Select(string.Format("仓库号='{0}'", e.Value));
                if (rr.Length > 0) dr["仓库名称"] = rr[0]["仓库名称"];
            }
            else if (e.Column.Caption == "计量单位编码")
            {
                DataRow[] rr = dt_unit.Select(string.Format("计量单位编码='{0}'", e.Value));
                if (rr.Length > 0) dr["计量单位"] = rr[0]["计量单位"];
            }


        }
    }
}
