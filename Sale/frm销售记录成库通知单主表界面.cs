using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;

namespace ERPSale
{
    public partial class frm销售记录成库通知单主表界面 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM = new DataTable();
        DataRow drM;
        string str_下拉框条件;
        DataTable dt_销售人员;
        string cfgfilepath = "";
        #endregion
        //视图权限
        string localuserid = CPublic.Var.LocalUserID;
        string lusname = CPublic.Var.localUserName;
          
    #region 自用类
    public frm销售记录成库通知单主表界面()
        {
            InitializeComponent();
        }

        private void frm销售记录成库通知单主表界面_Load(object sender, EventArgs e)
        {
            try
            {
                if (localuserid == "910480" || localuserid == "910082")
                {
                    localuserid = "admin";
                    lusname = "admin";
                    //
                    gridColumn65.Visible = false;
                    gridColumn66.Visible = false;
                    gridColumn67.Visible = false;
                    gridColumn68.Visible = false;
                    gridColumn69.Visible = false;
 
                }
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(this.panel2, this.Name, cfgfilepath);

                DateTime t = CPublic.Var.getDatetime().Date;
             
                bar_日期_后.EditValue = Convert.ToDateTime(t.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
                bar_日期_前.EditValue = Convert.ToDateTime(t.AddDays(-15).ToString("yyyy-MM-dd"));
                //bar_日期_前.EditValue = System.DateTime.Today.AddDays(-7);
                bar_单据状态.EditValue = "全部";
                fun_条件();
                fun_载入主表(str_下拉框条件);
                 dtM.Columns.Add("已出库",typeof(bool)) ;
                 foreach (DataRow dr in dtM.Rows)
                 {
                     if (dr["总明细数"] == DBNull.Value)
                     {
                         dr["总明细数"] = 0;
                     }
                     if (dr["完成数"] == DBNull.Value)
                     {
                         dr["完成数"] = 0;
                     }
                     if (Convert.ToInt32(dr["总明细数"]) > Convert.ToInt32(dr["完成数"]))
                     {
                         dr["已出库"] = false;

                     }
                     else
                     {
                         dr["已出库"] = true;
                     }
                 }
                Thread th = new Thread(() =>
                {
                    fun_载入明细_l();
                });
                th.Start();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_载入明细_l()
        {
            DataTable dt_出库通知明细_1 = new DataTable();
            if (localuserid != "admin" && CPublic.Var.LocalUserTeam != "管理员权限")
            {
                string sql = string.Format(@"  with t as ( select tzzb.出库通知单号 from 销售记录销售出库通知单主表 tzzb
             left join (select 出库通知单号,left(销售订单明细号,14)as 销售订单号,COUNT(*)总明细数 from 销售记录销售出库通知单明细表  where 作废=0  group by 出库通知单号,left(销售订单明细号,14))a
             on tzzb.出库通知单号=a.出库通知单号
                left  join (select 出库通知单号,COUNT(*)完成数 from  销售记录销售出库通知单明细表 where 作废=0 and 完成 = 1   group by 出库通知单号)b
                on b.出库通知单号=tzzb.出库通知单号  
               left  join  销售记录销售订单主表 szb on szb.销售订单号=a.销售订单号
               left join (select 关联单号,待审核人  from  单据审核申请表 where 单据类型='销售发货申请' and 作废=0 ) djsh on djsh.关联单号=tzzb.出库通知单号
               where  szb.作废=0  and szb.部门编号 = '{0}'
               and tzzb.创建日期 >= '{1}' and tzzb.创建日期 <= '{2}')
  
         select stcmx.*,salezb.目标客户,isnull(kc.库存总数,0) as 仓库数量,ygb.部门编号,salemx.税前单价,salemx.税前金额,salemx.税后单价,salemx.税后金额,币种,tzzb.出库日期 as 要求出库日期
                 from 销售记录销售出库通知单明细表 stcmx
                left join 仓库物料数量表 kc on kc.物料编码 = stcmx.物料编码  and stcmx.仓库号 = kc.仓库号
                left join 销售记录销售出库通知单主表 tzzb on tzzb.出库通知单号=stcmx.出库通知单号
                left join 人事基础员工表 ygb on ygb.员工号 = stcmx.操作员ID      
                left join 销售记录销售订单明细表 salemx on salemx.销售订单明细号=stcmx.销售订单明细号   
                left join 销售记录销售订单主表 salezb on salemx.销售订单号=salezb.销售订单号      
                where stcmx.作废 = 0 and stcmx.出库通知单号 in (select 出库通知单号  from t) ", CPublic.Var.localUser部门编号, bar_日期_前.EditValue, bar_日期_后.EditValue);
                dt_出库通知明细_1 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            }
            else
            {
                string sql = string.Format(@"  with t as ( select tzzb.出库通知单号 from 销售记录销售出库通知单主表 tzzb
             left join (select 出库通知单号,left(销售订单明细号,14)as 销售订单号,COUNT(*)总明细数 from 销售记录销售出库通知单明细表  where 作废=0  group by 出库通知单号,left(销售订单明细号,14))a
             on tzzb.出库通知单号=a.出库通知单号
                left  join (select 出库通知单号,COUNT(*)完成数 from  销售记录销售出库通知单明细表 where 作废=0 and 完成 = 1   group by 出库通知单号)b
                on b.出库通知单号=tzzb.出库通知单号  
               left  join  销售记录销售订单主表 szb on szb.销售订单号=a.销售订单号
               left join (select 关联单号,待审核人  from  单据审核申请表 where 单据类型='销售发货申请' and 作废=0 ) djsh on djsh.关联单号=tzzb.出库通知单号
               where  szb.作废=0  
               and tzzb.创建日期 >= '{0}' and tzzb.创建日期 <= '{1}')
  
             select stcmx.*,salezb.目标客户,isnull(kc.库存总数,0) as 仓库数量,ygb.部门编号,salemx.税前单价,salemx.税前金额,salemx.税后单价,salemx.税后金额,币种,tzzb.出库日期 as 要求出库日期
                 from 销售记录销售出库通知单明细表 stcmx
                left join 仓库物料数量表 kc on kc.物料编码 = stcmx.物料编码  and stcmx.仓库号 = kc.仓库号
                left join 销售记录销售出库通知单主表 tzzb on tzzb.出库通知单号=stcmx.出库通知单号
                left join 人事基础员工表 ygb on ygb.员工号 = stcmx.操作员ID      
                left join 销售记录销售订单明细表 salemx on salemx.销售订单明细号=stcmx.销售订单明细号   
                left join 销售记录销售订单主表 salezb on salemx.销售订单号=salezb.销售订单号      
                where stcmx.作废 = 0 and stcmx.出库通知单号 in (select 出库通知单号  from t) ", bar_日期_前.EditValue, bar_日期_后.EditValue);
                dt_出库通知明细_1 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            }
             

            BeginInvoke(new MethodInvoker(() =>
            {
                gridControl2.DataSource = dt_出库通知明细_1;
            }));
            
        }
        private void f_s(string dh)
        {
            string s = $@"select tzzb.*,a.销售订单号,销售备注,客户订单号,a.总明细数,b.完成数,szb.部门编号,szb.客户订单号,待审核人,szb.目标客户 from 销售记录销售出库通知单主表 tzzb
             left join(select 出库通知单号, left(销售订单明细号,14)as 销售订单号,COUNT(*)总明细数 from 销售记录销售出库通知单明细表 where 作废 = 0  group by 出库通知单号,left(销售订单明细号, 14))a
                                on tzzb.出库通知单号 = a.出库通知单号
             left join(select 出库通知单号, COUNT(*) 完成数 from 销售记录销售出库通知单明细表 where 作废 = 0 and 完成 = 1   group by 出库通知单号)b
                              on b.出库通知单号 = tzzb.出库通知单号
              left join  销售记录销售订单主表 szb on szb.销售订单号 = a.销售订单号
              left join(select 关联单号, 待审核人 from  单据审核申请表 where 单据类型= '销售发货申请' and 作废 = 0) djsh on djsh.关联单号 = tzzb.出库通知单号 where tzzb.出库通知单号='{dh}' ";

            DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);

            DataRow[] r_1 = dtM.Select(string.Format("出库通知单号='{0}'", dh));
            r_1[0].ItemArray = t.Rows[0].ItemArray;


        }

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
           
                if (e != null && e.Button == MouseButtons.Right)
                {

                    contextMenuStrip1.Show(gc, new System.Drawing.Point(e.X, e.Y));

                }
 
            drM = gv.GetDataRow(gv.FocusedRowHandle);
        
            if (drM == null) return;
            f_s(drM["出库通知单号"].ToString());
            DataTable dasda = (DataTable)this.gc.DataSource;
            
            fun_载入明细(drM["出库通知单号"].ToString().Trim());

            //修改
            if (e.Clicks == 2 && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                string str_出库通知单 = drM["出库通知单号"].ToString();
                //新增界面
                //if (drM["生效"].ToString() == "未生效" && "用户" == "用户")
                if (drM["生效"].ToString().ToLower() == "false")
                {
                    frm销售记录成库通知单详细界面 fm = new frm销售记录成库通知单详细界面(str_出库通知单, drM, dtM);
                    fm.Dock = System.Windows.Forms.DockStyle.Fill;
                    CPublic.UIcontrol.AddNewPage(fm, "出库通知单");
                }
                //视图界面
                else 
                {
                    frm销售记录成库通知单详细界面_视图 fm = new frm销售记录成库通知单详细界面_视图(str_出库通知单, drM);
                    fm.Dock = System.Windows.Forms.DockStyle.Fill;
                    CPublic.UIcontrol.AddNewPage(fm, "出库通知单视图");
                    //frm销售记录成库通知单主表界面_Load(null,null);   
                }
            }
        }
        #endregion

        #region 方法

        private void fun_载入明细( string str_出库通知单号)
        {

            string sql = string.Format(@"select stcmx.*,isnull(kc.库存总数,0) as 仓库数量 from 销售记录销售出库通知单明细表 stcmx
                left join 仓库物料数量表 kc on kc.物料编码 = stcmx.物料编码  and stcmx.仓库号 = kc.仓库号
                left join 销售记录销售订单明细表 smx on smx.销售订单明细号=stcmx.销售订单明细号 
                where stcmx.作废 = 0 and  stcmx.出库通知单号 = '{0}' ", str_出库通知单号);
            
            DataTable  dt  = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);//仓库数量
            da.Fill(dt);
            gridControl1.DataSource = dt;
        }
        private void fun_载入主表(string str_条件)
        {
            try
            {
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(str_条件, strconn);
                da.Fill(dtM);
                gc.DataSource = dtM;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_条件()
        {
            if (dtM != null)
            {
                dtM.Clear();
            }
            
            dt_销售人员 = ERPorg.Corg.fun_hr("销售", localuserid);

            //str_下拉框条件 = "select * from 销售记录销售出库通知单主表 {0}";
           // tzzb.*,a.销售订单号,销售备注,客户订单号,a.总明细数,b.完成数,szb.部门编号,szb.客户订单号,待审核人
            str_下拉框条件 = @"select tzzb.*,a.销售订单号,销售备注,客户订单号,a.总明细数,b.完成数,szb.部门编号,szb.客户订单号,待审核人,szb.目标客户 from 销售记录销售出库通知单主表 tzzb
             left join (select 出库通知单号,left(销售订单明细号,14)as 销售订单号,COUNT(*)总明细数 from 销售记录销售出库通知单明细表  where 作废=0  group by 出库通知单号,left(销售订单明细号,14))a
                              on tzzb.出库通知单号=a.出库通知单号
             left  join (select 出库通知单号,COUNT(*)完成数 from  销售记录销售出库通知单明细表 where 作废=0 and 完成 = 1   group by 出库通知单号)b
                             on b.出库通知单号=tzzb.出库通知单号  
              left  join  销售记录销售订单主表 szb on szb.销售订单号=a.销售订单号
              left join (select 关联单号,待审核人  from  单据审核申请表 where 单据类型='销售发货申请' and 作废=0 ) djsh on djsh.关联单号=tzzb.出库通知单号 ";
            //string s_组合1 = "where ";


            //if (CPublic.Var.LocalUserTeam != "管理员")
            //{
            //    if (dt_销售人员.Rows.Count != 0)
            //    {
            //        //s_组合1 += "操作员ID = '" + CPublic.Var.LocalUserID + "' and ";
            //        s_组合1 += " ( ";
            //        foreach (DataRow r_x in dt_销售人员.Rows)
            //        {
            //            s_组合1 += "操作员ID = '" + r_x["工号"].ToString().Trim() + "' or ";
            //        }
            //        s_组合1 = s_组合1.Substring(0, s_组合1.Length - 3);
            //        s_组合1 = s_组合1 + " ) ";
            //        s_组合1 += " and ";
            //    }
            //    else
            //    {
            //        throw new Exception("你没有该视图权限");
            //    }
            //}
            //if (bar_销售订单号.EditValue.ToString() != "")
            //{
            //    s_组合1 += "销售订单号 = '" + bar_销售订单号.EditValue.ToString() + "'" + " and ";
            //}
            //else
            string s_组合1 = "where tzzb.作废=0 and ";
            {
                if (bar_日期_前.EditValue != null && bar_日期_后.EditValue != null && bar_日期_前.EditValue.ToString() != "" && bar_日期_后.EditValue.ToString() != "")
                {
                    s_组合1 += " tzzb.创建日期 >= '" + ((DateTime)bar_日期_前.EditValue).ToString("yyyy-MM-dd HH:mm:ss") + "'" + " and tzzb.创建日期 <= '" + ((DateTime)bar_日期_后.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss") + "' and ";
                }
                if (bar_单据状态.EditValue != null && bar_单据状态.EditValue.ToString() != "")
                {
                    if (bar_单据状态.EditValue.ToString() == "已生效")
                    {
                        s_组合1 += "tzzb.生效 = 1 and ";
                    }
                    if (bar_单据状态.EditValue.ToString() == "未生效")
                    {
                        s_组合1 += "tzzb.生效 = 0 and ";
                    }
                    //if (bar_单据状态.EditValue.ToString() == "已完成")
                    //{
                    //    s_组合1 += "销售记录销售出库通知单主表.完成 = 1 and ";
                    //}
                    //if (bar_单据状态.EditValue.ToString() == "未完成")
                    //{
                    //    s_组合1 += "销售记录销售出库通知单主表.完成 = 0 and ";
                    //}
                    if (bar_单据状态.EditValue.ToString() == "全部")
                    { }
                }
            }
            //if (s_组合1 != "where ")
            if (s_组合1 != "and ")
            {
                s_组合1 = s_组合1.Substring(0, s_组合1.Length - 4);
                //str_下拉框条件 = string.Format(str_下拉框条件, s_组合1);
                str_下拉框条件 = str_下拉框条件+ s_组合1;
            }
            string sql1 = "";
            if (lusname != "admin" && CPublic.Var.LocalUserTeam != "管理员权限" )
            {
                sql1 = "and szb.部门编号 = '" + CPublic.Var.localUser部门编号 + "'";
                str_下拉框条件 = str_下拉框条件 + sql1;
            }
        }
        #endregion
        #region 界面操作
        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        
        }

        //新增
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
          
        }
        #endregion
        private void fun_search_销售()
        {
            string sql = string.Format(@"select tzzb.*,a.销售订单号,销售备注,客户订单号,a.总明细数,b.完成数,szb.部门编号,szb.客户订单号,待审核人,szb.目标客户 from 销售记录销售出库通知单主表 tzzb
             left join (select 出库通知单号,left(销售订单明细号,14)as 销售订单号,COUNT(*)总明细数 from 销售记录销售出库通知单明细表  where 作废=0  group by 出库通知单号,left(销售订单明细号,14))a
             on tzzb.出库通知单号=a.出库通知单号
                left  join (select 出库通知单号,COUNT(*)完成数 from  销售记录销售出库通知单明细表 where 作废=0 and 完成 = 1   group by 出库通知单号)b
                on b.出库通知单号=tzzb.出库通知单号  
               left  join  销售记录销售订单主表 szb on szb.销售订单号=a.销售订单号
              left join (select 关联单号,待审核人  from  单据审核申请表 where 单据类型='销售发货申请' and 作废=0 ) djsh on djsh.关联单号=tzzb.出库通知单号 where  szb.作废=0  and tzzb.出库通知单号 in 
            (select  出库通知单号  from [销售记录销售出库通知单明细表] where left(销售订单明细号,14) like '%{0}%')", barEditItem1.EditValue.ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                dtM = new DataTable();
                da.Fill(dtM);
                dtM.Columns.Add("已出库", typeof(bool));
                foreach (DataRow dr in dtM.Rows)
                {
                    if (dr["总明细数"] == DBNull.Value)
                    {
                        dr["总明细数"] = 0;
                    }
                    if (dr["完成数"] == DBNull.Value)
                    {
                        dr["完成数"] = 0;
                    }
                    if (Convert.ToInt32(dr["总明细数"]) > Convert.ToInt32(dr["完成数"]))
                    {
                        dr["已出库"] = false;
                    }
                    else
                    {
                        dr["已出库"] = true;
                    }
                }
                gc.DataSource = dtM;
            }
        }
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
         
        }
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (barEditItem1.EditValue != null && barEditItem1.EditValue.ToString() != "")
                {
                    fun_search_销售();
                }
                else
                {
                    throw new Exception("未输入销售号");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_条件();
            fun_载入主表(str_下拉框条件);
            dtM.Columns.Add("已出库", typeof(bool));
            foreach (DataRow dr in dtM.Rows)
            {
                if (dr["总明细数"] == DBNull.Value)
                {
                    dr["总明细数"] = 0;
                }
                if (dr["完成数"] == DBNull.Value)
                {
                    dr["完成数"] = 0;
                }
                if (Convert.ToInt32(dr["总明细数"]) > Convert.ToInt32(dr["完成数"]))
                {

                    dr["已出库"] = false;

                }
                else
                {
                    dr["已出库"] = true;
                }
            }
            Thread th = new Thread(() =>
            {
                fun_载入明细_l();
            });
            th.Start();
        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //drM = dtM.NewRow();
            //dtM.Rows.Add(drM);
            frm销售记录成库通知单详细界面 fm = new frm销售记录成库通知单详细界面();
            fm.Dock = System.Windows.Forms.DockStyle.Fill;
            CPublic.UIcontrol.AddNewPage(fm, "新增出库通知单");
            //fun_载入();
        }

        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void 包装抽检图片查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPproduct.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPproduct.Form包装抽检相关文件上传", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                object[] drr = new object[1];
                DataRow drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
                string sql = string.Format("select * from 包装抽检相关文件上传 where 出库通知单号='{0}'", drM["出库通知单号"]);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql,strconn);
                if (dt.Rows.Count>0 )
                {
                    drr[0] = drM["出库通知单号"];
                    Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                    ui.ShowDialog();
                }
                else
                {
                    throw new Exception("当前无图片");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
 
        }

        private void gv_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                drM = gv.GetDataRow(gv.FocusedRowHandle);
                fun_载入明细(drM["出库通知单号"].ToString().Trim());

            }
            catch (Exception)
            {

         
            }
            
        }

        private void barLargeButtonItem3_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                Control c = GetFocusedControl();
                if (c != null && c.GetType().Equals(gridControl1.GetType()))
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Title = "导出Excel";
                    saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                    DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                    if (dialogResult == DialogResult.OK)
                    {
                        DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                        DevExpress.XtraGrid.GridControl gc = (c) as DevExpress.XtraGrid.GridControl;

                        gc.ExportToXlsx(saveFileDialog.FileName);
                        DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                }
                else
                {

                    MessageBox.Show("若要导出请先选中要导出的表格(鼠标点一下表格)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        [DllImport("user32.dll")]
        public static extern int GetFocus();

        private Control GetFocusedControl()
        {
            Control c = null;
            // string focusedControl = null;
            IntPtr handle = (IntPtr)GetFocus();

            if (handle == null)
                this.FindForm().KeyPreview = true;
            else
            {
                c = Control.FromHandle(handle);//这就是
                //focusedControl =
                //c.Parent.TopLevelControl.Name.ToString();
            }

            return c;
        }
    }
}
