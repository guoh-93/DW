using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
namespace ERPpurchase
{
    public partial class ui请购单 : UserControl
    {
        #region 变量
        string strconn = CPublic.Var.strConn;
        /// <summary>
        /// i_Bl表示单据状态 0表示未提交审核，1表示提交审核 2表示已审核， 3表示完成 4表示关闭
        /// </summary>
        int i_Bl = 0;
        bool bl_add = true;

        DataTable dtP;
        DataRow dr_cs;
        DataTable dt_物料;

        #endregion

        public ui请购单()
        {
            InitializeComponent();
        }
        public ui请购单(DataRow dr)
        {
            InitializeComponent();
            dr_cs = dr;
            bl_add = false;

            fre_state(dr["请购单号"].ToString());

        }


        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show(string.Format("确认关闭当前界面？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                timer1.Stop();
                timer1.Dispose();
                CPublic.UIcontrol.ClosePage();
            }
        }
        //新增
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                bool bl_clear = false;
                if (i_Bl == 0) //尚未提交
                {
                    if (txt_caigousn.Text.Trim() == "") //尚未保存
                    {
                        if (MessageBox.Show("当前请购单尚未保存，是否继续！", "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            bl_clear = true;
                        }

                    }
                    // 已保存未提交
                    if (MessageBox.Show("当前请购单已保存尚未提交审核，是否继续！", "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        bl_clear = true;
                    }
                }
                else
                {
                    bl_clear = true;
                }
                if (bl_clear)
                {
                    dr_cs = null;
                    ui请购单_Load(null, null);


                }

                bl_add = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //保存
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_save(false);
                bl_add = false;
                fre_state(txt_caigousn.Text);
                MessageBox.Show("保存成功");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //新增明细
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                gv2.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                fun_新增明细();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //删除明细
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtP.Rows.Count <= 0)
                    throw new Exception("没有采购单明细可以删除！");
                DataRow r = gv2.GetDataRow(gv2.FocusedRowHandle);
                if(r.RowState!=DataRowState.Added)
                {
                    if (Convert.ToDecimal(r["已转采购数"]) > 0)
                        throw new Exception("已有已转采购数不可删除");
                }
                r.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bl_tjsh">是否提交审核</param>
        private void fun_save(bool bl_tjsh)
        {

            DataTable t_审核 = null;
            string s_z = "select  * from 请购单主表 where 1=2";
            string s_mx = "select  * from 请购单明细表 where 1=2";
            DataTable dt_z = CZMaster.MasterSQL.Get_DataTable(s_z, strconn);
            //dtP即为明细表
            DateTime time = CPublic.Var.getDatetime();
            if (bl_add)
            {
                DataRow drM = dt_z.NewRow();
                drM["请购单号"] = txt_caigousn.Text = string.Format("QG{0}{1:00}{2:00}{3:0000}", time.Year, time.Month,
                time.Day, CPublic.CNo.fun_得到最大流水号("QG", time.Year, time.Month));
                drM["部门编号"] = CPublic.Var.localUser部门编号;
                drM["部门名称"] = CPublic.Var.localUser部门名称;
                drM["申请人"] = CPublic.Var.localUserName;
                drM["申请人ID"] = CPublic.Var.LocalUserID;
                drM["备注"] = txt_cgyy.Text;
                drM["创建日期"] = time;
                drM["修改日期"] = time;
                if (bl_tjsh)
                {
                    drM["提交审核"] = true;
                    t_审核 = ERPorg.Corg.fun_PA("生效", "请购单", drM["请购单号"].ToString(), drM["部门名称"].ToString());
                }

                dt_z.Rows.Add(drM);
                int i = 1;
                string x = "00";
                if (dtP.DefaultView.Count >= 100) x = "000";
                foreach (DataRow dr in dtP.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    dr["GUID"] = System.Guid.NewGuid();
                    dr["请购单号"] = drM["请购单号"];
                    dr["请购单明细号"] = drM["请购单号"].ToString() + "-" + i.ToString(x);
                    dr["POS"] = i++;
                }
            }
            else //修改
            {
                string sss = string.Format("select * from 请购单主表 where 请购单号='{0}'", txt_caigousn.Text);
                dt_z = CZMaster.MasterSQL.Get_DataTable(sss, strconn);
                dt_z.Rows[0]["修改日期"] = time;
                dt_z.Rows[0]["备注"] = txt_cgyy.Text;
                int i = 1;
                string x = "00";
                foreach (DataRow dr in dtP.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    if (dr["GUID"].ToString() == "")
                    {
                        dr["GUID"] = System.Guid.NewGuid();
                        dr["请购单号"] = dt_z.Rows[0]["请购单号"];
                        int ii = Back_pos(i);
                        dr["请购单明细号"] = dt_z.Rows[0]["请购单号"].ToString() + "-" + ii.ToString(x);
                        dr["POS"] = ii;
                        i = ii+1;
                    }
                }
                if (bl_tjsh)
                {
                    dt_z.Rows[0]["提交审核"] = true;
                    t_审核 = ERPorg.Corg.fun_PA("生效", "请购单", dt_z.Rows[0]["请购单号"].ToString(), dt_z.Rows[0]["部门名称"].ToString());
                }
            }


            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("pur"); //事务的名称
            SqlCommand cmd1 = new SqlCommand(s_z, conn, ts);
            SqlCommand cmd = new SqlCommand(s_mx, conn, ts);

            try
            {
                SqlDataAdapter da;
                da = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da);
                da.Update(dt_z);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dtP);
                if (bl_tjsh)
                {
                    string s = "select  * from 单据审核申请表 where 1<>1";
                    cmd = new SqlCommand(s, conn, ts);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(t_审核);
                }

                ts.Commit();

                dr_cs = dt_z.Rows[0];
                fun_load();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw new Exception(ex.Message);
            }

        }
        private int Back_pos(int i)
        {
            int x = i;
            DataRow[] rr = dtP.Select(string.Format("POS={0}", i));
            if (rr.Length > 0)
            {
                x=Back_pos(++i);
            }
            return x;
        }

        //新增明细的方法
        private void fun_新增明细()
        {
            try
            {
                DataRow r = dtP.NewRow();
                dtP.Rows.Add(r);
                //gv2.FocusedRowHandle = gv2.LocateByDisplayText(0, gridColumn28, "");
                DateTime t = CPublic.Var.getDatetime().Date.AddDays(7);
                if (dtP.Rows.Count > 1)
                {
                    r["期望日期"] = dtP.Rows[0]["期望日期"]; //若第一条有时间 跟着第一条
                }
                else
                {
                    r["期望日期"] = t;//默认七天后

                }
               



            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_新增采购单明细");
            }
        }
        private void fun_下拉框()
        {

            string s = @"select base.物料编码,base.物料名称,base.规格型号,base.计量单位,base.计量单位编码,
                        a.仓库号,a.仓库名称,isnull(a.库存总数,0)库存总数,isnull(a.有效总数,0)有效总数,isnull(a.在途量,0)在途量,新数据
                       , base.仓库号 as 默认仓库号,base.仓库名称 as 默认仓库 from 基础数据物料信息表 base left join 仓库物料数量表 a  on base.物料编码 = a.物料编码  and a.仓库号=base.仓库号
                        where (base.可购=1 or 委外=1)  and base.停用= 0 and base.在研 = 0"; //布尔字段1 位是否 纳入可用量
            dt_物料 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            repositoryItemSearchLookUpEdit1.DataSource = dt_物料;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
        }
        private void fun_load()
        {
            string qgd = "";
            if (dr_cs == null)
            {
                txt_cgbumen.Text = CPublic.Var.localUser部门名称;
                txt_cgjbr.Text = CPublic.Var.localUserName;
            }
            else
            {
                txt_cgbumen.Text = dr_cs["部门名称"].ToString();
                txt_cgjbr.Text = dr_cs["申请人"].ToString();
                txt_cgyy.Text = dr_cs["备注"].ToString();
                txt_caigousn.Text=qgd = dr_cs["请购单号"].ToString();

            }
            string s = string.Format(@"select  a.*,物料名称,规格型号,新数据,计量单位 from 请购单明细表 a left join 基础数据物料信息表 base  
                 on base.物料编码=a.物料编码 where  请购单号='{0}'", qgd);
            dtP = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            gc2.DataSource = dtP;
            fun_下拉框();


        }
        string cfgfilepath = "";
        private void ui请购单_Load(object sender, EventArgs e)
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
                x.UserLayout(panel3, this.Name, cfgfilepath);
                fun_load();
                fre_state(txt_caigousn.Text);
                timer1.Start();
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
                fun_save(true);
                bl_add = false;
                i_Bl = 1;



                fre_state(txt_caigousn.Text);
                MessageBox.Show("提交成功");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //撤回提交
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("是否确认撤销提交？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string sql = string.Format("select * from 请购单主表 where 请购单号 = '{0}'", txt_caigousn.Text);
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_撤销 = new DataTable();
                    da.Fill(dt_撤销);
                    sql = string.Format(@"select * from 单据审核申请表  where 关联单号 = '{0}' and 单据类型='请购单' and 操作类型='生效'
                                            and 作废=0 and 审核=0", txt_caigousn.Text);
                    da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_审核申请 = new DataTable();
                    if (dt_审核申请.Rows.Count > 0)
                    {
                        throw new Exception("单据状态已更改,不可撤回提交");
                    }
                    da.Fill(dt_审核申请);

                    if (i_Bl == 1)
                    {

                        if (dt_撤销.Rows.Count > 0)
                        {
                            if (Convert.ToBoolean(dt_撤销.Rows[0]["提交审核"]))
                            {
                                dt_撤销.Rows[0]["提交审核"] = 0;
                                if (dt_审核申请.Rows.Count > 0)
                                {
                                    dt_审核申请.Rows[0].Delete();
                                }
                                sql = "select * from 单据审核申请表 where 1<>1";
                                da = new SqlDataAdapter(sql, strconn);
                                new SqlCommandBuilder(da);
                                da.Update(dt_审核申请);
                                sql = "select * from 请购单主表 where 1<>1";
                                da = new SqlDataAdapter(sql, strconn);
                                new SqlCommandBuilder(da);
                                da.Update(dt_撤销);
                                MessageBox.Show("撤销成功");
                                i_Bl = 0;
 
                            }
                        }
                    }
                    fre_state(txt_caigousn.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv2_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "物料编码")
            {
                DataRow drr = gv2.GetDataRow(gv2.FocusedRowHandle);
                DataRow[] dr = dt_物料.Select(string.Format("物料编码='{0}'", e.Value.ToString()));
                if (dr.Length > 0)
                {
                    drr["物料名称"] = dr[0]["物料名称"];
                    drr["计量单位"] = dr[0]["计量单位"];
                    drr["规格型号"] = dr[0]["规格型号"];
                    drr["新数据"] = dr[0]["新数据"];

                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if(txt_caigousn.Text!="")
                {
                    fre_state(txt_caigousn.Text);
                }
            }
            catch  
            {

             
            }
        }

        private void fre_state(string s_dh)
        {
            string s = string.Format("select  * from 请购单主表 where 请购单号='{0}' ", s_dh);
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            if (t.Rows.Count > 0)
            {
                if (Convert.ToBoolean(t.Rows[0]["关闭"]))
                {
                    label2.Text = "已关闭";
                    i_Bl = 4;
                }
                else if (Convert.ToBoolean(t.Rows[0]["完成"]))
                {
                    label2.Text = "已完成";
                    i_Bl = 3;
                }
                else if (Convert.ToBoolean(t.Rows[0]["审核"]))
                {

                    label2.Text = "已审核";
                    i_Bl = 2;
                }
                else if (Convert.ToBoolean(t.Rows[0]["提交审核"]))
                {
                    label2.Text = "已提交";
                    i_Bl = 1;
                }
                else
                {
                    label2.Text = "---";
                    i_Bl = 0;
                }
            }
        }

        /// <summary>
    }
}
