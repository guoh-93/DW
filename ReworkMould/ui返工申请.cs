using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ReworkMould
{
    public partial class ui返工申请 : UserControl
    {
        #region
        string strcon = CPublic.Var.strConn;
        DataTable dt_PickingList;
        DataTable dt_inventory;
        string str_单号 = "";
        DataRow dr_cs;
        bool bl_新增 = true;
        bool bl_提交审核 = true;
        DataTable dt_bom;
        DataTable dt_退料清单;
        DataTable dt_仓库;
        #endregion
        public ui返工申请()
        {
            InitializeComponent();
        }
        /// <summary>
        /// ss为需修改的申请单号
        /// </summary>
        /// <param name="ss"></param>
        public ui返工申请(DataRow rr)
        {
            InitializeComponent();
            str_单号 = rr["申请单号"].ToString();
            dr_cs = rr;
            bl_新增 = false;
            string sql_mx = string.Format("select * from 新_返修申请子表 where 物料编码 = '{0}' and 申请单号 = '{1}'", rr["返修产品编码"],rr["申请单号"]);
            DataTable dt111 = CZMaster.MasterSQL.Get_DataTable(sql_mx, strcon);
            label11.Text = dt111.Rows[0]["仓库号"].ToString();
            label13.Text = dt111.Rows[0]["仓库名称"].ToString();
            // fun_load();


        }
        public ui返工申请(string s_申请单号, DataRow dr, DataTable dt)
        {
            InitializeComponent();
            bl_新增 = false;
            str_单号 = s_申请单号;
            dr_cs = dr;
            dt_PickingList = dt;
            bl_提交审核 = false;


        }

        private void fun_load()
        {

            //加载BOM
            string std = @"select 产品编码,子项编码  from 基础数据物料BOM表 ";
            dt_bom = CZMaster.MasterSQL.Get_DataTable(std, strcon);

            string s = @"select  base.物料编码,base.物料名称,base.规格型号,kc.仓库号,kc.仓库名称,库存总数,base.计量单位编码,base.计量单位 from 基础数据物料信息表 base
            left  join 仓库物料数量表 kc on kc.物料编码=base.物料编码 
            where 自制=1 and 停用=0 and base.物料编码 in  ( select  产品编码 from 基础数据物料BOM表  group  by 产品编码)  ";

            dt_inventory = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            searchLookUpEdit1.Properties.DataSource = dt_inventory;
            searchLookUpEdit1.Properties.DisplayMember = "物料编码";
            searchLookUpEdit1.Properties.ValueMember = "物料编码";

            dt_仓库 = new DataTable();
            string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别'  and 布尔字段3 = 1";
            SqlDataAdapter da = new SqlDataAdapter(sql4, strcon);
            da.Fill(dt_仓库);
            repositoryItemSearchLookUpEdit1.DataSource = dt_仓库;
            repositoryItemSearchLookUpEdit1.DisplayMember = "仓库号";
            repositoryItemSearchLookUpEdit1.ValueMember = "仓库号";
            s = @" select  属性字段1 as 班组编号,属性值 as 班组 from  基础数据基础属性表  where 属性类别='班组'";
            DataTable dt_班组 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            searchLookUpEdit3.Properties.DataSource = dt_班组;
            searchLookUpEdit3.Properties.DisplayMember = "班组";
            searchLookUpEdit3.Properties.ValueMember = "班组编号";

            if (!bl_新增)
            {
                textBox1.Text = str_单号;
                comboBoxEdit1.EditValue = dr_cs["返修类型"].ToString();
                searchLookUpEdit2.EditValue = dr_cs["目标产品编码"].ToString();
                textBox6.Text = dr_cs["生产备注"].ToString();
                dateEdit1.EditValue = dr_cs["预完工日期"];
                textBox7.Text = dr_cs["数量"].ToString();
                searchLookUpEdit3.EditValue = dr_cs["班组编号"].ToString(); //20-3-9
                s = string.Format("select *  from 新_返修申请主表 where 申请单号='{0}'", str_单号);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                searchLookUpEdit1.EditValue = dt.Rows[0]["返修产品编码"].ToString(); //触发事件 加载dt_PickingList
                s = string.Format("select  * from 新_返修申请子表 where 申请单号='{0}' order by POS", str_单号);
                dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                int c = dt.Rows.Count;

                for (int i = c - 1; i >= 0; i--)
                {
                    DataRow[] r = dt_PickingList.Select(string.Format("物料编码 ='{0}'", dt.Rows[i]["物料编码"]));
                    if (r.Length > 0) { r[0]["选择"] = true; r[0]["数量"] = dt.Rows[i]["数量"]; }
                    else
                    {

                        dt.Rows[i].Delete(); //从后往前遍历 若要删除不影响前面的索引
                    }
                }
                gridControl1.DataSource = dt_PickingList;
                s = string.Format(@"select 物料编码,数量 from 新_返修申请退料子表 where 申请单号='{0}'", dr_cs["申请单号"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (temp.Rows.Count > 0)
                {
                    if (dt_退料清单 == null || dt_退料清单.Columns.Count == 0)
                    {
                        dt_退料清单 = new DataTable();


                        dt_退料清单 = ERPorg.Corg.billofM(dt_退料清单, searchLookUpEdit1.EditValue.ToString(), false, dt_bom);
                        string x = "base.物料编码 in(";
                        foreach (DataRow dr in dt_退料清单.Rows)
                        {
                            x += "'" + dr["子项编码"].ToString() + "',";
                        }
                        x = x.Substring(0, x.Length - 1) + ") order by 物料编码";
                        x = @"select base.物料编码,base.物料名称,base.规格型号,base.仓库号,base.仓库名称,库存总数,计量单位编码,计量单位 from 基础数据物料信息表 base
                    Left join 仓库物料数量表 kc on kc.物料编码=base.物料编码 and base.仓库号=kc.仓库号  where " + x;
                        dt_退料清单 = CZMaster.MasterSQL.Get_DataTable(x, strcon);
                        dt_退料清单.Columns.Add("数量", typeof(decimal));
                        dt_退料清单.Columns.Add("选择", typeof(bool));
                    }
                    foreach (DataRow dr in temp.Rows)
                    {
                        DataRow[] xr = dt_退料清单.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        if (xr.Length == 0) throw new Exception("bom已修改,重新勾选需退料物料");
                        xr[0]["选择"] = true;
                        xr[0]["数量"] = dr["数量"];

                    }
                }

            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bl_提交"> 指示是否提交,区分保存与提交审核</param>
        private void fun_save(bool bl_提交)
        {
            string s_车间 = "";
            DateTime t = CPublic.Var.getDatetime();
            string s_主 = string.Format("select  * from 新_返修申请主表 where 申请单号='{0}'", str_单号);
            DataTable dt_主 = CZMaster.MasterSQL.Get_DataTable(s_主, strcon);
            if (dt_主.Rows.Count > 0)
            {
                dt_主.Rows[0]["返修类型"] = comboBoxEdit1.EditValue;
                dt_主.Rows[0]["返修产品编码"] = searchLookUpEdit1.EditValue.ToString();
                string str_目标产品 = "";
                if (comboBoxEdit1.EditValue.ToString() == "A->A")
                {
                    dt_主.Rows[0]["目标产品编码"] = str_目标产品 = searchLookUpEdit1.EditValue.ToString();
                }
                else
                {
                    dt_主.Rows[0]["目标产品编码"] = str_目标产品 = searchLookUpEdit2.EditValue.ToString();
                }
                dt_主.Rows[0]["数量"] = Convert.ToDecimal(textBox7.Text);
                dt_主.Rows[0]["制单人员"] = CPublic.Var.localUserName;
                dt_主.Rows[0]["制单人员ID"] = CPublic.Var.LocalUserID;
                dt_主.Rows[0]["制单日期"] = t;
                dt_主.Rows[0]["班组"] = searchLookUpEdit3.Text;
                dt_主.Rows[0]["班组编号"] = searchLookUpEdit3.EditValue;
                dt_主.Rows[0]["生产备注"] = textBox6.Text;
                dt_主.Rows[0]["预完工日期"] = Convert.ToDateTime(dateEdit1.EditValue);

                string s =string.Format( @"select gx.*,部门名称 from  人事记录组织生产关系表 gx 
                left join 人事基础部门表 bm  on gx.生产车间 = bm.部门编号  where 工号='{0}'",CPublic.Var.LocalUserID);
                DataTable dt_bm = CZMaster.MasterSQL.Get_DataTable(s,strcon);
                if (dt_bm.Rows.Count == 0 || dt_bm.Rows[0]["生产车间"] == null || dt_bm.Rows[0]["生产车间"].ToString() == "")
                {

                    s = string.Format("select  车间编号,车间 from 基础数据物料信息表 where 物料编码='{0}'", str_目标产品);
                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    dt_主.Rows[0]["车间编号"] = temp.Rows[0]["车间编号"];
                    dt_主.Rows[0]["车间名称"] = s_车间 = temp.Rows[0]["车间"].ToString();
                }
                else
                {
                    dt_主.Rows[0]["车间编号"] = dt_bm.Rows[0]["生产车间"];
                    dt_主.Rows[0]["车间名称"] = s_车间 = dt_bm.Rows[0]["部门名称"].ToString();

                }

            }
            else
            {
                DataRow dr = dt_主.NewRow();
                string s_单号 = string.Format("RW{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                                CPublic.CNo.fun_得到最大流水号("RW", t.Year, t.Month));
                dr["申请单号"] = textBox1.Text = s_单号;
                str_单号 = s_单号;
                dr["返修类型"] = comboBoxEdit1.EditValue;
                dr["返修产品编码"] = searchLookUpEdit1.EditValue.ToString();
                string str_目标产品 = "";
                if (comboBoxEdit1.EditValue.ToString() == "A->A")
                {
                    dr["目标产品编码"] = str_目标产品 = searchLookUpEdit1.EditValue.ToString();
                }
                else
                {
                    dr["目标产品编码"] = str_目标产品 = searchLookUpEdit2.EditValue.ToString();
                }
                dr["数量"] = Convert.ToDecimal(textBox7.Text);
                dr["制单人员"] = CPublic.Var.localUserName;
                dr["制单人员ID"] = CPublic.Var.LocalUserID;
                dr["制单日期"] = t;
                dr["生产备注"] = textBox6.Text;
                dr["预完工日期"] = Convert.ToDateTime(dateEdit1.EditValue);
                string s = string.Format("select  车间编号,车间 from 基础数据物料信息表 where 物料编码='{0}'", str_目标产品);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                dr["车间编号"] = temp.Rows[0]["车间编号"];
                dr["车间名称"] = s_车间 = temp.Rows[0]["车间"].ToString();
                dr["班组"] = searchLookUpEdit3.Text;
                dr["班组编号"] = searchLookUpEdit3.EditValue;
                dt_主.Rows.Add(dr);
            }
            DataTable dt_子 = new DataTable();
            DataTable dt_退料子 = new DataTable();

            DataTable dt_审核 = new DataTable(); 
            if (bl_提交)
            {

                dt_审核 = ERPorg.Corg.fun_PA("生效", "返修申请", textBox1.Text, s_车间); //此函数内已经区分是新增或修改了
                                                                               // dt_主.Rows[0]["提交审核"] = true;
            }
            if (bl_新增)
            {
                string s_子 = string.Format("select * from 新_返修申请子表 where 1=2");
                dt_子 = CZMaster.MasterSQL.Get_DataTable(s_子, strcon);
                DataView dv = new DataView(dt_PickingList);
                dv.RowFilter = "选择=1";
                int i = 1;
                DataTable t_z = dv.ToTable();
                //这边是3-21 号更改 
                DataRow[] r_产品 = dt_inventory.Select(string.Format("物料编码='{0}'", searchLookUpEdit1.EditValue));
                DataRow r_a = t_z.NewRow();
                r_a["物料编码"] = r_产品[0]["物料编码"];
                r_a["物料名称"] = r_产品[0]["物料名称"];
                r_a["规格型号"] = r_产品[0]["规格型号"];
                r_a["仓库号"] = label11.Text;
                r_a["仓库名称"] = label13.Text ;
                r_a["数量"] = Convert.ToDecimal(textBox7.Text);
                r_a["计量单位编码"] = r_产品[0]["计量单位编码"];
                r_a["计量单位"] = r_产品[0]["计量单位"];
                r_a["库存总数"] = r_产品[0]["库存总数"];
                t_z.Rows.InsertAt(r_a, 0);

                //19-3-21 更改 需返工产品数量已经填到表头,417行返修品加入领料清单已经去除，这边生成领料清单 需要再加进去
                foreach (DataRow r in t_z.Rows)
                {
                    DataRow r_子 = dt_子.NewRow();
                    //r_子["GUID"] = System.Guid.NewGuid() ;
                    r_子["申请单号"] = textBox1.Text;
                    r_子["POS"] = i;
                    r_子["申请明细号"] = textBox1.Text + "-" + i++.ToString("00");
                    r_子["物料编码"] = r["物料编码"];
                    r_子["物料名称"] = r["物料名称"];
                    r_子["规格型号"] = r["规格型号"];
                    r_子["数量"] = r["数量"];
                    r_子["计量单位编码"] = r["计量单位编码"];
                    r_子["计量单位"] = r["计量单位"];
                    r_子["仓库号"] = r["仓库号"];
                    r_子["仓库名称"] = r["仓库名称"];
                    dt_子.Rows.Add(r_子);
                }


                if (dt_退料清单 != null && dt_退料清单.Columns.Count > 0)
                {
                    s_子 = string.Format("select * from 新_返修申请退料子表 where 1=2");
                    dt_退料子 = CZMaster.MasterSQL.Get_DataTable(s_子, strcon);
                    DataView dv_1 = new DataView(dt_退料清单);
                    dv_1.RowFilter = "选择=1";
                    i = 1;
                    foreach (DataRow r in dv_1.ToTable().Rows)
                    {
                        DataRow r_子 = dt_退料子.NewRow();
                        // r_子["GUID"] = System.Guid.NewGuid();
                        r_子["申请单号"] = textBox1.Text;
                        r_子["POS"] = i;
                        r_子["申请明细号"] = textBox1.Text + "-t-" + i++.ToString("00");
                        r_子["物料编码"] = r["物料编码"];
                        //r_子["物料名称"] = r["物料名称"];
                        //r_子["规格型号"] = r["规格型号"];
                        r_子["数量"] = r["数量"];
                        r_子["计量单位编码"] = r["计量单位编码"];
                        r_子["计量单位"] = r["计量单位"];
                        r_子["仓库号"] = r["仓库号"];
                        r_子["仓库名称"] = r["仓库名称"];
                        dt_退料子.Rows.Add(r_子);
                    }
                }
            }
            else
            {


                string s_子 = string.Format("select * from 新_返修申请子表 where 申请单号='{0}'", textBox1.Text);
                dt_子 = CZMaster.MasterSQL.Get_DataTable(s_子, strcon);

                DataView dv = new DataView(dt_PickingList);
                dv.RowFilter = "选择=1";
                DataTable dtx = dv.ToTable();
                int c = dt_子.Rows.Count;
                //先遍历dt_子 去掉取消勾的
                for (int i = c - 1; i >= 0; i--)
                {
                    DataRow[] r = dtx.Select(string.Format("物料编码='{0}'", dt_子.Rows[i]["物料编码"]));
                    if (r.Length == 0 && dt_子.Rows[i]["物料编码"].ToString() != searchLookUpEdit1.EditValue.ToString())
                        dt_子.Rows[i].Delete();
                }
                //再遍历 dtx ,同步dt_子中数据或者新增
                int x = 2;
                int y = 1;
                foreach (DataRow dr in dtx.Rows)
                {
                    DataRow[] r = dt_子.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (r.Length > 0)
                    {
                        r[0]["数量"] = dr["数量"];
                        r[0]["申请明细号"] = textBox1.Text + "-" + x.ToString("00");
                        r[0]["POS"] = x++;
                    }
                    else
                    {
                        DataRow r_子 = dt_子.NewRow();
                        // r_子["GUID"] = System.Guid.NewGuid();
                        r_子["申请单号"] = textBox1.Text;
                        r_子["POS"] = x;
                        r_子["申请明细号"] = textBox1.Text + "-" + x++.ToString("00");
                        r_子["物料编码"] = dr["物料编码"];
                        r_子["物料名称"] = dr["物料名称"];
                        r_子["规格型号"] = dr["规格型号"];
                        r_子["数量"] = dr["数量"];
                        r_子["计量单位编码"] = dr["计量单位编码"];
                        r_子["计量单位"] = dr["计量单位"];
                        r_子["仓库号"] = dr["仓库号"];
                        r_子["仓库名称"] = dr["仓库名称"];
                        dt_子.Rows.Add(r_子);
                    }

                }
                DataRow[] re = dt_子.Select(string.Format("物料编码='{0}'", searchLookUpEdit1.EditValue.ToString()));
                if (re.Length > 0)
                {
                    DataRow[] r_产品 = dt_inventory.Select(string.Format("物料编码='{0}'", searchLookUpEdit1.EditValue));
                    re[0]["物料编码"] = r_产品[0]["物料编码"];
                    re[0]["物料名称"] = r_产品[0]["物料名称"];
                    re[0]["规格型号"] = r_产品[0]["规格型号"];
                    re[0]["仓库号"] = label11.Text;
                    re[0]["仓库名称"] = label13.Text;
                    re[0]["数量"] = Convert.ToDecimal(textBox7.Text);
                    re[0]["计量单位编码"] = r_产品[0]["计量单位编码"];
                    re[0]["计量单位"] = r_产品[0]["计量单位"];

                }
                else
                {
                    DataRow[] r_产品 = dt_inventory.Select(string.Format("物料编码='{0}'", searchLookUpEdit1.EditValue));
                    DataRow r_a = dt_子.NewRow();
                    r_a["物料编码"] = r_产品[0]["物料编码"];
                    r_a["物料名称"] = r_产品[0]["物料名称"];
                    r_a["规格型号"] = r_产品[0]["规格型号"];
                    r_a["仓库号"] = label11.Text;
                    r_a["仓库名称"] = label13.Text;
                    r_a["数量"] = Convert.ToDecimal(textBox7.Text);
                    r_a["计量单位编码"] = r_产品[0]["计量单位编码"];
                    r_a["计量单位"] = r_产品[0]["计量单位"];
                    dt_子.Rows.InsertAt(r_a, 0);
                }
                foreach (DataRow xx in dt_子.Rows)
                {
                    if (xx.RowState == DataRowState.Deleted) continue;
                    if (xx["申请明细号"].ToString() == "")
                    {
                        xx["申请单号"] = textBox1.Text;
                        xx["申请明细号"] = textBox1.Text + "-" + x.ToString("00");
                        xx["POS"] = x++;
                    }
                   

                }
                if (dt_退料清单 != null)
                {
                    s_子 = string.Format("select * from 新_返修申请退料子表 where 申请单号='{0}'", textBox1.Text);
                    dt_退料子 = CZMaster.MasterSQL.Get_DataTable(s_子, strcon);

                    DataView dv_1 = new DataView(dt_退料清单);
                    dv_1.RowFilter = "选择=1";
                    DataTable dtx_1 = dv_1.ToTable();
                    c = dt_退料子.Rows.Count;
                    //先遍历dt_退料子 去掉取消勾的
                    for (int i = c - 1; i >= 0; i--)
                    {
                        DataRow[] r = dtx_1.Select(string.Format("物料编码='{0}'", dt_退料子.Rows[i]["物料编码"]));
                        if (r.Length == 0) dt_退料子.Rows[i].Delete();
                    }
                    //再遍历 dtx_1 ,同步dt_退料子中数据或者新增
                    foreach (DataRow dr in dtx_1.Rows)
                    {
                        DataRow[] r = dt_退料子.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        if (r.Length > 0)
                        {
                            r[0]["数量"] = dr["数量"];

                            r[0]["申请明细号"] = textBox1.Text + "-t-" + y.ToString("00");
                            r[0]["POS"] = y++;
                        }
                        else
                        {
                            DataRow r_子 = dt_退料子.NewRow();
                            // r_子["GUID"] = System.Guid.NewGuid();
                            r_子["申请单号"] = textBox1.Text;
                            r_子["POS"] = y;
                            r_子["申请明细号"] = textBox1.Text + "-" + y++.ToString("00");
                            r_子["物料编码"] = dr["物料编码"];
                            //r_子["物料名称"] = dr["物料名称"];
                            //r_子["规格型号"] = dr["规格型号"];
                            r_子["数量"] = dr["数量"];
                            r_子["计量单位编码"] = dr["计量单位编码"];
                            r_子["计量单位"] = dr["计量单位"];
                            r_子["仓库号"] = dr["仓库号"];
                            r_子["仓库名称"] = dr["仓库名称"];
                            dt_退料子.Rows.Add(r_子);
                        }

                    }

                }
            }
            bl_新增 = false;
            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("rw");
            try
            {
                SqlDataAdapter da;
                SqlCommand cmd = new SqlCommand("select * from 新_返修申请主表 where 1<>1", conn, ts);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_主);
                cmd = new SqlCommand("select * from 新_返修申请子表 where 1=2", conn, ts);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_子);
                if (dt_审核.Columns.Count > 0)
                {
                    cmd = new SqlCommand("select * from 单据审核申请表 where 1=2", conn, ts);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_审核);
                }
                if (dt_退料子.Columns.Count > 0)
                {
                    cmd = new SqlCommand("select * from 新_返修申请退料子表 where 1=2", conn, ts);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_退料子);
                }
                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw new Exception("生效失败");
            }
        }
        private void comboBoxEdit1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit1.EditValue.ToString() == "A->B") //返修品与最终产品不一致  
            {
                label6.Visible = true;
                label7.Visible = true;
                label8.Visible = true;
                textBox4.Visible = true;
                textBox5.Visible = true;
                searchLookUpEdit2.Visible = true;
                searchLookUpEdit2.Properties.DataSource = dt_inventory;
                searchLookUpEdit2.Properties.DisplayMember = "物料编码";
                searchLookUpEdit2.Properties.ValueMember = "物料编码";
            }
            else
            {
                label6.Visible = false;
                label7.Visible = false;
                label8.Visible = false;
                textBox4.Visible = false;
                textBox5.Visible = false;
                searchLookUpEdit2.Visible = false;
            }
        }
        //private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DataRow[] r_产品 = dt_inventory.Select(string.Format("物料编码='{0}'", searchLookUpEdit1.EditValue));

        //        textBox2.Text = r_产品[0]["物料名称"].ToString();
        //        textBox3.Text = r_产品[0]["规格型号"].ToString();
        //        if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() != "")
        //        {
        //            //先把所有子项加载出来
        //            dt_PickingList = new DataTable();
        //            dt_PickingList = ERPorg.Corg.billofM(dt_PickingList, searchLookUpEdit1.EditValue.ToString(), false, dt_bom);
        //            if (dt_PickingList.Rows.Count > 0)
        //            {
        //                string s = "base.物料编码 in (";
        //                foreach (DataRow dr in dt_PickingList.Rows)
        //                {
        //                    s += "'" + dr["子项编码"].ToString() + "',";
        //                }
        //                s = s.Substring(0, s.Length - 1) + ") order by 物料编码";
        //                s = @"select base.物料编码,base.物料名称,base.规格型号,base.仓库号,base.仓库名称,库存总数,计量单位编码,计量单位 from 基础数据物料信息表 base
        //            Left join 仓库物料数量表 kc on kc.物料编码=base.物料编码 and base.仓库号=kc.仓库号  where " + s;
        //                dt_PickingList = CZMaster.MasterSQL.Get_DataTable(s, strcon);
        //                dt_PickingList.Columns.Add("数量", typeof(decimal));
        //                dt_PickingList.Columns.Add("选择", typeof(bool));
        //                ////再把返修品 插到第一行
        //                gridControl1.DataSource = dt_PickingList;
        //            }
        //            else
        //            {
        //                MessageBox.Show("所选产品没有BOM");
        //            }
        //            dt_退料清单 = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        MessageBox.Show(ex.Message);
        //    }

        //}
        private void fun_check()
        {
            if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
            {
                throw new Exception("未选择需返修的产品");
            }
            if (searchLookUpEdit3.EditValue == null || searchLookUpEdit3.EditValue.ToString() == "")
            {
                throw new Exception("班组未选择");
            }
            if (searchLookUpEdit2.Visible == true && (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == ""))
            {
                throw new Exception("未选择返修后产品");
            }
            if (comboBoxEdit1.EditValue == null || comboBoxEdit1.EditValue.ToString() == "") throw new Exception("未选择返修类型");
            DataView dv = new DataView(dt_PickingList);
            dv.RowFilter = "选择=1";
            // if (dv.Count == 0) throw new Exception("未选择领任何料");
            foreach (DataRow dr in dv.ToTable().Rows)
            {
                decimal dec = 0;
                if (!decimal.TryParse(dr["数量"].ToString(), out dec)) throw new Exception("数量输入有误,请检查");
                else if (dec < 0) throw new Exception("输入数量不可小于0");
            }
            if (dateEdit1.EditValue == null || dateEdit1.EditValue.ToString() == "")
                throw new Exception("未选择预完工日期");
            string cj = "";
            if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == "")
                cj = searchLookUpEdit1.EditValue.ToString();
            else
            {
                cj = searchLookUpEdit2.EditValue.ToString();
            }
            string s = string.Format("select  车间编号,车间 from 基础数据物料信息表 where 物料编码='{0}'", cj); //取最终产品的车间
            DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (temp.Rows[0]["车间编号"] == null || temp.Rows[0]["车间编号"].ToString() == "") throw new Exception("生产的产品尚未维护车间信息");
        }
        private void ui返修申请_Load(object sender, EventArgs e)
        {
            try
            {
                fun_load();
                if (Convert.ToBoolean(bl_提交审核) == false)
                {
                    fun_编辑();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_编辑()
        {
            barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            textBox1.Enabled = false;
            comboBoxEdit1.Enabled = false;
            gridView1.OptionsBehavior.Editable = false;
            dateEdit1.Enabled = false;
            textBox7.Enabled = false;
            searchLookUpEdit1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            searchLookUpEdit2.Enabled = false;
            textBox5.Enabled = false;
            textBox4.Enabled = false;
            textBox6.Enabled = false;
            simpleButton1.Enabled = false;
        }

        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit2.EditValue != null && searchLookUpEdit2.EditValue.ToString() != "")
            {
                DataRow[] r = dt_inventory.Select(string.Format("物料编码='{0}'", searchLookUpEdit2.EditValue));
                textBox5.Text = r[0]["物料名称"].ToString();
                textBox4.Text = r[0]["规格型号"].ToString();

                //先把所有子项加载出来
                dt_PickingList = new DataTable();
                dt_PickingList = ERPorg.Corg.billofM(dt_PickingList, searchLookUpEdit2.EditValue.ToString(), false, dt_bom);
                if (dt_PickingList.Rows.Count > 0)
                {
                    string s = "base.物料编码 in (";
                    foreach (DataRow dr in dt_PickingList.Rows)
                    {
                        s += "'" + dr["子项编码"].ToString() + "',";
                    }
                    s = s.Substring(0, s.Length - 1) + ") order by 物料编码";
                    s = @"select base.物料编码,base.物料名称,base.规格型号,base.仓库号,base.仓库名称,库存总数,计量单位编码,计量单位 from 基础数据物料信息表 base
                    Left join 仓库物料数量表 kc on kc.物料编码=base.物料编码 and base.仓库号=kc.仓库号  where " + s;
                    dt_PickingList = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    dt_PickingList.Columns.Add("数量", typeof(decimal));
                    dt_PickingList.Columns.Add("选择", typeof(bool));
                    ////再把返修品 插到第一行
                    gridControl1.DataSource = dt_PickingList;
                }
            }
        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        //save
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                if (dt_PickingList == null) throw new Exception("尚未有任何数据,请确认");

                if (MessageBox.Show("是否确认领料清单与退料清单已完善？", "确认!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    gridView1.CloseEditor();
                    this.BindingContext[dt_PickingList].EndCurrentEdit();
                    this.ActiveControl = null;

                    fun_check();
                    fun_save(false);

                    MessageBox.Show("保存成功");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        //提交审核
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (dt_PickingList == null) throw new Exception("尚未有任何数据,请确认");
                if (MessageBox.Show("是否确认领料清单与退料清单已完善？", "提交审核!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    gridView1.CloseEditor();
                    this.BindingContext[dt_PickingList].EndCurrentEdit();
                    this.ActiveControl = null;
                    fun_check();
                    fun_save(true);
                    MessageBox.Show("提交成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
            {
                if (dt_退料清单 == null || dt_退料清单.Columns.Count == 0)
                {
                    dt_退料清单 = new DataTable();

                    dt_退料清单 = ERPorg.Corg.billofM(dt_退料清单, searchLookUpEdit1.EditValue.ToString(), false, dt_bom);
                    string s = "base.物料编码 in(";
                    foreach (DataRow dr in dt_退料清单.Rows)
                    {
                        s += "'" + dr["子项编码"].ToString() + "',";
                    }
                    s = s.Substring(0, s.Length - 1) + ") order by 物料编码";
                    s = @"select base.物料编码,base.物料名称,base.规格型号,base.仓库号,base.仓库名称,库存总数,计量单位编码,计量单位 from 基础数据物料信息表 base
                    Left join 仓库物料数量表 kc on kc.物料编码=base.物料编码 and base.仓库号=kc.仓库号  where " + s;
                    dt_退料清单 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    dt_退料清单.Columns.Add("数量", typeof(decimal));
                    dt_退料清单.Columns.Add("选择", typeof(bool));
                }




                ui返工退料清单 fm = new ui返工退料清单(dt_退料清单);
                fm.ShowDialog();
                fm.StartPosition = FormStartPosition.CenterScreen;
                if (fm.issave) dt_退料清单 = fm.dt_退料列表;

            }
            else
            {
                MessageBox.Show("未选择返工产品");

            }



        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (e.Column.FieldName == "仓库号")
                {
                    dr["仓库号"] = e.Value;
                    DataRow[] ds = dt_仓库.Select(string.Format("仓库号 = '{0}'", dr["仓库号"]));
                    dr["仓库名称"] = ds[0]["仓库名称"];
                    //dr["仓库名称"] = sr["仓库名称"].ToString();
                    string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                    DataTable dt_物料数量 = new DataTable();
                    da.Fill(dt_物料数量);
                    if (dt_物料数量.Rows.Count == 0)
                    {
                        dr["库存总数"] = 0;
                        // dr["有效总数"] = 0;
                    }
                    else
                    {
                        dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
                        //  dr["有效总数"] = dt_物料数量.Rows[0]["有效总数"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

       

        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow d = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
            try
            {

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DataRow[] r_产品 = dt_inventory.Select(string.Format("物料编码='{0}'", searchLookUpEdit1.EditValue));

                textBox2.Text = r_产品[0]["物料名称"].ToString();
                textBox3.Text = r_产品[0]["规格型号"].ToString();
                if ( !(searchLookUpEdit2.EditValue != null && searchLookUpEdit2.EditValue.ToString() != ""))
                {
                    //先把所有子项加载出来
                    dt_PickingList = new DataTable();
                    dt_PickingList = ERPorg.Corg.billofM(dt_PickingList, searchLookUpEdit1.EditValue.ToString(), false, dt_bom);
                    if (dt_PickingList.Rows.Count > 0)
                    {
                        string s = "base.物料编码 in (";
                        foreach (DataRow dr in dt_PickingList.Rows)
                        {
                            s += "'" + dr["子项编码"].ToString() + "',";
                        }
                        s = s.Substring(0, s.Length - 1) + ") order by 物料编码";
                        s = @"select base.物料编码,base.物料名称,base.规格型号,base.仓库号,base.仓库名称,库存总数,计量单位编码,计量单位 from 基础数据物料信息表 base
                        Left join 仓库物料数量表 kc on kc.物料编码=base.物料编码 and base.仓库号=kc.仓库号  where " + s;
                        dt_PickingList = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                        dt_PickingList.Columns.Add("数量", typeof(decimal));
                        dt_PickingList.Columns.Add("选择", typeof(bool));
                        ////再把返修品 插到第一行
                        gridControl1.DataSource = dt_PickingList;
                    }
                    else
                    {
                        MessageBox.Show("所选产品没有BOM");
                    }
                    dt_退料清单 = null;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        

        private void searchLookUpEdit1View_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow d = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
            try
            {
                DataRow[] r_产品 = dt_inventory.Select(string.Format("物料编码='{0}' and 仓库号 = '{1}'", d["物料编码"].ToString(), d["仓库号"]));

                textBox2.Text = r_产品[0]["物料名称"].ToString();
                textBox3.Text = r_产品[0]["规格型号"].ToString();
                label11.Text = d["仓库号"].ToString();
                label13.Text = d["仓库名称"].ToString();
                if (!(searchLookUpEdit2.EditValue != null && searchLookUpEdit2.EditValue.ToString() != ""))
                {
                    //先把所有子项加载出来
                    dt_PickingList = new DataTable();
                    dt_PickingList = ERPorg.Corg.billofM(dt_PickingList, d["物料编码"].ToString(), false, dt_bom);
                    if (dt_PickingList.Rows.Count > 0)
                    {
                        string s = "base.物料编码 in (";
                        foreach (DataRow dr in dt_PickingList.Rows)
                        {
                            s += "'" + dr["子项编码"].ToString() + "',";
                        }
                        s = s.Substring(0, s.Length - 1) + ") order by 物料编码";
                        s = @"select base.物料编码,base.物料名称,base.规格型号,base.仓库号,base.仓库名称,库存总数,计量单位编码,计量单位 from 基础数据物料信息表 base
                    Left join 仓库物料数量表 kc on kc.物料编码=base.物料编码 and base.仓库号=kc.仓库号  where " + s;
                        dt_PickingList = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                        dt_PickingList.Columns.Add("数量", typeof(decimal));
                        dt_PickingList.Columns.Add("选择", typeof(bool));
                        ////再把返修品 插到第一行
                        gridControl1.DataSource = dt_PickingList;
                    }
                    else
                    {
                        MessageBox.Show("所选产品没有BOM");
                    }
                    dt_退料清单 = null;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
