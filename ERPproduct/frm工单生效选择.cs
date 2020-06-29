using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text.RegularExpressions;


namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm工单生效选择 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        #region   变量
        string strconn = CPublic.Var.strConn;
        bool ck = false;
        string sql;
        DataTable dt_bom;
        DataTable dtM;
        DataView dv;
        DataTable dt_下拉;   //工单负责人
        DataTable dt_班组;
        DataTable dt_指定领料人;         //指定领料人 
        DateTime time1;
        DataTable dt_刷新数量 = new DataTable();
        public int in_部分完工数;
        DataRow[] dr;
        string str_工单号;
        int index = 0; //用来生效后直接指向下一行 
        int ncopy_dy = 0;
        DataTable dt_dy;
        DataTable dtp = new DataTable();
        string s_checkmessage = "";
        string cfgfilepath = "";
        int 状态 = 1;//工单关闭跳转
        //int 次数 = 1;

        # endregion
        #region 加载
        public frm工单生效选择()
        {
            InitializeComponent();
            DateTime tt = CPublic.Var.getDatetime();
            barEditItem2.EditValue = Convert.ToDateTime(tt.AddMonths(-1).ToString("yyyy-MM-dd"));
            barEditItem3.EditValue = Convert.ToDateTime(tt.ToString("yyyy-MM-dd"));
            time1 = Convert.ToDateTime(tt.ToShortDateString()).AddDays(1).AddSeconds(-1);

            barEditItem1.EditValue = "已生效";
        }
        public frm工单生效选择(string str_工单号)
        {
            InitializeComponent();
            DateTime tt = CPublic.Var.getDatetime();
            barEditItem2.EditValue = Convert.ToDateTime(tt.ToString("yyyy-MM-dd"));
            barEditItem3.EditValue = Convert.ToDateTime(tt.ToString("yyyy-MM-dd"));

            time1 = Convert.ToDateTime(tt.ToShortDateString()).AddDays(1).AddSeconds(-1);

            this.str_工单号 = str_工单号;
            barEditItem1.EditValue = "未生效";
        }
#pragma warning disable IDE1006 // 命名样式
        private void frm工单生效选择_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(this.panel2, this.Name, cfgfilepath);

            searchLookUpEdit1.EditValue = null;
            barLargeButtonItem6_ItemClick(null, null);
            fun_load();

            try
            {


                if (str_工单号 != null)
                {
                    gv.Focus();

                    gv.FocusedRowHandle = gv.LocateByDisplayText(0, gridColumn2, str_工单号);

                    gv.SelectRow(gv.FocusedRowHandle);

                    gridView1_RowCellClick(null, null);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        #endregion   
        #region 函数

#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {
            if (textBox1.Text == "")
            {
                throw new Exception("未选择生效工单");

            }
            else if (textBox7.Text == "")
            {
                throw new Exception("请输入生产数量");

            }
            //else if (textBox8.Text == "")
            //{
            //    throw new Exception("请选择生产车间");
            //}
            //else if (searchLookUpEdit2.Text == "")
            //{
            //    throw new Exception("请指定领料人");

            //}
            else if (dateEdit1.EditValue == null)
            {
                throw new Exception("请选择预计开工日期");
            }
            else if (dateEdit2.EditValue == null)
            {
                throw new Exception("请选择预计完工日期");
            }
            try
            {
                decimal a = Convert.ToDecimal(textBox7.Text);
            }
            catch
            {
                throw new Exception("请正确输入生产数量");
            }

            for (int i = 0; i <= gv.DataRowCount; i++)
            {
                if (gv.GetDataRow(i)["选择"].Equals(true))
                {
                    index = i;
                    break;
                }
            }
            dv.RowFilter = null;
            dv.RowFilter = "选择 = 1";
            if (dv.ToTable().Rows.Count <= 0)
            {
                throw new Exception("没有选择工单");
            }
            DataTable dt_check = dv.ToTable();
            foreach (DataRow dr in dt_check.Rows)
            {
                string sq = string.Format(@"select bz.*,bx.审核 from   基础数据BOM修改主表  bz 
                 left join   单据审核申请表 bx on bz.BOM修改单号=bx.关联单号   
                 where bz.产品编码 = '{0}' and bx.审核=0 and bz.作废=0 and bx.作废=0", dr["物料编码"].ToString());

                DataTable dt_ss = CZMaster.MasterSQL.Get_DataTable(sq, strconn);
                if (dt_ss.Rows.Count > 0)
                {
                    if (MessageBox.Show("当前物料有正在修改的BOM未审核，请确认继续？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        continue;
                    }
                    else
                    {
                        ck = true;
                        break;
                    }
                }


            }




        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 18-9-14  工单生效 判断有效库存够不够
        /// </summary>
        private List<string> fun_c_n()
#pragma warning restore IDE1006 // 命名样式
        {
            List<string> li = new List<string>();
            dv.RowFilter = null;
            dv.RowFilter = "选择 = 1";
            DataTable dt_c_n = dv.ToTable();
            //2018-9-13 生产要求 工单生效前先判断料够不够,总共多少条,成功多少条 失败多少条,即库存不够从生效清单去掉,单号放在一个字符串里,最后领料单生效成功了显示出来即可
            s_checkmessage = "选择记录共:" + dv.ToTable().Rows.Count.ToString() + "条,";
            DataTable dt_check_有效库存 = new DataTable();
            //先将所有需生效的工单的BOM加载出来,不需要重复的
            foreach (DataRow dr in dt_c_n.Rows)
            {
                //找一层BOM物料
                string s = string.Format(@"select 子项编码,数量,有效总数,bom.仓库号 from 基础数据物料BOM表 bom
                 left join 仓库物料数量表 kc  on bom.子项编码=kc.物料编码 and bom.仓库号=kc.仓库号  where 产品编码='{0}'", dr["物料编码"]);
                using (SqlDataAdapter da = new SqlDataAdapter(s, strconn))
                {
                    DataTable temp = new DataTable();
                    da.Fill(temp);
                    if (dt_check_有效库存.Columns.Count == 0)
                        da.Fill(dt_check_有效库存);
                    else
                    {
                        foreach (DataRow xx in temp.Rows)
                        {

                            //  decimal dec = Convert.ToDecimal(xx["数量"]) * Convert.ToDecimal(dr["生产数量"]);
                            DataRow[] r = dt_check_有效库存.Select(string.Format("子项编码='{0}' and 仓库号='{1}'", xx["子项编码"], xx["仓库号"]));
                            if (r.Length == 0)
                            {
                                dt_check_有效库存.ImportRow(r[0]);
                            }
                        }
                    }
                    foreach (DataRow xx in temp.Rows)
                    {
                        //此工单所需料数量
                        decimal dec = Convert.ToDecimal(xx["数量"]) * Convert.ToDecimal(dr["生产数量"]);
                        DataRow[] rxx = dt_check_有效库存.Select(string.Format("子项编码='{0}' and 仓库号='{1}'", xx["子项编码"], xx["仓库号"]));
                        dec = Convert.ToDecimal(rxx[0]["有效总数"]) - dec;
                        if (dec >= 0)
                        {
                            rxx[0]["有效总数"] = dec;
                        }
                        else //库存不够的
                        {
                            li.Add(dr["生产工单号"].ToString());
                            break;
                        }
                    }

                }
            }
            return li;
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                string s = "select  产品编码,子项编码  from 基础数据物料BOM表 ";
                dt_bom = new DataTable();
                dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                //  dt_指定领料人 = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
                //  dr = dt_指定领料人.Select(string.Format("用户描述='领料人'"));
                string sql = "";
                //if (dt_指定领料人.Rows.Count == 0)
                //{
                //    sql = string.Format("select 员工号,姓名 from 人事基础员工表 where  在职状态='在职'");
                //}
                //else
                //{

                //    sql = string.Format("select 员工号,姓名 from 人事基础员工表 where 生产部门 ='{0}' and  在职状态='在职'", dt_指定领料人.Rows[0]["生产车间"]);
                //}
                //DataTable dt = new DataTable();
                //dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                //searchLookUpEdit2.Properties.DataSource = dt;
                //searchLookUpEdit2.Properties.DisplayMember = "员工号";
                //searchLookUpEdit2.Properties.ValueMember = "员工号";

                //searchLookUpEdit1.Properties.DataSource = dt;
                //searchLookUpEdit1.Properties.DisplayMember = "员工号";
                //searchLookUpEdit1.Properties.ValueMember = "员工号";
                //if (dr.Length > 0)
                //{
                //    searchLookUpEdit2.EditValue = dr[0]["工号"];
                //}
                //19-4-17修改
                sql = "  select  属性字段1 as 班组编号,属性值 as 班组 from  基础数据基础属性表  where 属性类别='班组'";
                dt_班组 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                searchLookUpEdit1.Properties.DataSource = dt_班组;
                searchLookUpEdit1.Properties.DisplayMember = "班组编号";
                searchLookUpEdit1.Properties.ValueMember = "班组编号";
                sql = "select  员工号,姓名 from 人事基础员工表 where 班组<>''  and 在职状态='在职'";
                dt_指定领料人 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                searchLookUpEdit2.Properties.DataSource = dt_指定领料人;
                searchLookUpEdit2.Properties.DisplayMember = "员工号";
                searchLookUpEdit2.Properties.ValueMember = "员工号";


            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //private void fun_load有参数()
        //{
        //    using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
        //    {
        //        DataTable dt = new DataTable();
        //        da.Fill(dt);
        //        dataBindHelper1.DataFormDR(dt.Rows[0]);
        //    }
        //    string sql_1 = string.Format("select * from 生产记录生产工单表 where 生产工单号='{0}'", str_工单号);
        //    using (SqlDataAdapter da = new SqlDataAdapter(sql_1, strconn))
        //    {
        //        DataTable dt_1 = new DataTable();
        //        da.Fill(dt_1);
        //        gridControl1.DataSource = dt_1;
        //    }
        //}


#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 工单生效 函数    
        /// </summary>
        private DataTable fun_gdsx(decimal dec_生产数量, string str_生产制令单号, DataTable dt)
#pragma warning restore IDE1006 // 命名样式
        {


            string sql111 = string.Format
                         ("select * from 生产记录生产制令表 where 生产制令单号='{0}'",
                          str_生产制令单号);
            using (SqlDataAdapter da = new SqlDataAdapter(sql111, strconn))
            {
                if (dt.Columns.Count == 0)
                {
                    da.Fill(dt);
                }
                else if (dt.Select(string.Format("生产制令单号='{0}'", str_生产制令单号)).Length == 0)
                {
                    da.Fill(dt);

                }

                //DataRow dr = dt.Rows[0];
                //DataRow dr = dt.Rows[dt.Rows.Count - 1];
                DataRow dr = dt.Select(string.Format("生产制令单号='{0}'", str_生产制令单号))[0];
                dr["已排单数量"] = Convert.ToDecimal(dr["已排单数量"]) + dec_生产数量;
                if (Convert.ToDecimal(dr["未排单数量"]) - dec_生产数量 < 0)
                {
                    dr["未排单数量"] = 0;
                }
                else
                {
                    dr["未排单数量"] = Convert.ToDecimal(dr["未排单数量"]) - dec_生产数量;
                }


                //new SqlCommandBuilder(da);
                //da.Update(dt);
            }

            return (dt);
        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 工单生效之后生成相应的 待领料单存的 为 原料
        /// 并保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private DataSet fun_save(DataSet ds, DataRow drr)
#pragma warning restore IDE1006 // 命名样式
        {
            DateTime t = CPublic.Var.getDatetime();

            string str_待领料单号 = string.Format("DL{0}{1:00}{2:0000}",
                                                t.Year, t.Month,
                                                CPublic.CNo.fun_得到最大流水号("DL", t.Year, t.Month));
            string sql_主表 = "select * from 生产记录生产工单待领料主表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_主表, strconn))
            {
                if (ds.Tables[0].Columns.Count == 0)
                {
                    da.Fill(ds.Tables[0]);
                }
                DataRow dr = ds.Tables[0].NewRow();
                dr["待领料单号"] = str_待领料单号;
                dr["领料类型"] = "工单领料";
                dr["生产工单号"] = drr["生产工单号"];
                dr["生产制令单号"] = drr["生产制令单号"];
                dr["生产工单类型"] = drr["生产工单类型"];
                dr["生产车间"] = drr["生产车间"];   //已变成车间编号
                dr["产品编码"] = drr["物料编码"];
                dr["产品名称"] = drr["物料名称"];
                dr["领料人ID"] = searchLookUpEdit2.EditValue;
                dr["领料人"] = textBox16.Text;
                dr["规格型号"] = drr["规格型号"];
                dr["原规格型号"] = drr["原规格型号"];
                dr["图纸编号"] = drr["图纸编号"];
                dr["生产数量"] = Convert.ToDecimal(drr["生产数量"]);
                dr["创建日期"] = t;
                dr["加急状态"] = drr["加急状态"];
                dr["制单人员"] = CPublic.Var.localUserName;
                dr["制单人员ID"] = CPublic.Var.LocalUserID;
                dr["工单负责人"] = textBox15.Text;
                dr["工单负责人ID"] = searchLookUpEdit1.EditValue;
                ds.Tables[0].Rows.Add(dr);

            }
            //保存待领料主表

            //保存 待领料单明细表
            string sql_明细 = "select * from 生产记录生产工单待领料明细表 where 1<>1";
            string sql_BOM = string.Format(@"select 基础数据物料BOM表.*,基础数据物料信息表.规格型号 from 基础数据物料BOM表 left join  基础数据物料信息表
                                             on 基础数据物料BOM表.子项编码=基础数据物料信息表.物料编码  
                                             where 基础数据物料BOM表.产品编码='{0}' and  基础数据物料BOM表.主辅料='主料' and 优先级=1", drr["物料编码"].ToString().Trim());
            using (SqlDataAdapter da = new SqlDataAdapter(sql_BOM, strconn))
            {
                DataTable dt_bom = new DataTable();
                da.Fill(dt_bom);
                //dt_刷新数量 = new DataTable();
                da.Fill(dt_刷新数量);

                using (SqlDataAdapter da1 = new SqlDataAdapter(sql_明细, strconn))
                {
                    if (ds.Tables[1].Columns.Count == 0)
                    {
                        da1.Fill(ds.Tables[1]);
                    }
                    int pos = 0;
                    //先判断基础表中 改产品有无替代料
                    //没有就走原来代码 

                    //   
                    string sql = string.Format("select * from 基础数据物料信息表 where 物料编码='{0}'", drr["物料编码"]);
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt.Rows[0]["BOM有无备料"].Equals(true))
                    {
                        foreach (DataRow r in dt_bom.Rows)  //dt_bom只取优先级为1的 ，若不够 再取 同组 物料判断
                        {
                            decimal dec_总需数 = Convert.ToDecimal(drr["生产数量"]) * Convert.ToDecimal(r["数量"]);
                            string sql_库存 = string.Format("select * from 仓库物料数量表 where 物料编码='{0}'", r["子项编码"]);
                            DataRow r_库存 = CZMaster.MasterSQL.Get_DataRow(sql_库存, strconn);
                            decimal dec_剩 = dec_总需数 - Convert.ToDecimal(r_库存["库存总数"]); //库存总数 换成 有效总数

                            if (dec_剩 <= 0 || r["组"].ToString() == "")   //就 取这条
                            {
                                DataRow dr = ds.Tables[1].NewRow();
                                dr["待领料单号"] = str_待领料单号;
                                dr["待领料单明细号"] = str_待领料单号 + "-" + pos.ToString("00");
                                dr["生产工单号"] = drr["生产工单号"];
                                dr["生产制令单号"] = drr["生产制令单号"];
                                dr["生产工单类型"] = drr["生产工单类型"];
                                dr["生产车间"] = drr["生产车间"]; //车间编号
                                dr["A面位号"] = r["A面位号"];
                                dr["B面位号"] = r["B面位号"];

                                dr["物料编码"] = r["子项编码"];
                                dr["物料名称"] = r["子项名称"];
                                dr["规格型号"] = r["n原ERP规格型号"].ToString().Trim();
                                try
                                {
                                    dr["待领料总量"] = dec_总需数;
                                    dr["未领数量"] = dr["待领料总量"];
                                    dr["BOM数量"] = Convert.ToDecimal(r["数量"]);

                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(string.Format("物料{0}BOM数量有问题 速找开发部确认", r["产品编码"]));

                                }
                                dr["创建日期"] = t;
                                dr["修改日期"] = t;
                                dr["制单人员"] = CPublic.Var.localUserName;
                                dr["制单人员ID"] = CPublic.Var.LocalUserID;

                                dr["工单负责人"] = textBox15.Text;
                                dr["工单负责人ID"] = searchLookUpEdit1.EditValue;
                                dr["领料人ID"] = searchLookUpEdit2.EditValue;
                                dr["领料人"] = textBox16.Text;
                                pos++;
                                ds.Tables[1].Rows.Add(dr);
                                continue;
                            }
                            else
                            {
                                //取同组 优先级不为1 的 其他几种物料  先 判断 单个库存够得

                                DataTable dt_3 = new DataTable();
                                string sql_3 = string.Format(@"select 基础数据物料BOM表.*,基础数据物料信息表.n原ERP规格型号,库存总数,需要数=0,剩余数=0  from 基础数据物料BOM表 
                                left join  基础数据物料信息表 on 基础数据物料BOM表.子项编码=基础数据物料信息表.物料编码 
                                left join 仓库物料数量表 on  仓库物料数量表.物料编码=基础数据物料信息表.物料编码
                                  where 基础数据物料BOM表.产品编码='{0}' and  基础数据物料BOM表.主辅料<>'包装'and 优先级<>1 and 组='{1}' order by 优先级"
                                       , drr["物料编码"].ToString(), r["组"].ToString());
                                dt_3 = CZMaster.MasterSQL.Get_DataTable(sql_3, strconn);
                                foreach (DataRow r3 in dt_3.Rows)
                                {
                                    decimal dec = dec_总需数 - Convert.ToDecimal(r3["库存总数"]);

                                    if (dec <= 0)     //某一替代料库存够     就取这个物料
                                    {
                                        DataRow dr = ds.Tables[1].NewRow();
                                        dr["待领料单号"] = str_待领料单号;
                                        dr["待领料单明细号"] = str_待领料单号 + "-" + pos.ToString("00");
                                        dr["生产工单号"] = drr["生产工单号"];
                                        dr["生产制令单号"] = drr["生产制令单号"];
                                        dr["生产工单类型"] = drr["生产工单类型"];
                                        dr["生产车间"] = drr["生产车间"]; //车间编号
                                        dr["A面位号"] = r["A面位号"];
                                        dr["B面位号"] = r["B面位号"];

                                        dr["物料编码"] = r3["子项编码"];
                                        dr["物料名称"] = r3["子项名称"];
                                        dr["规格型号"] = r3["n原ERP规格型号"].ToString().Trim();
                                        try
                                        {
                                            dr["待领料总量"] = dec_总需数;
                                            dr["未领数量"] = dr["待领料总量"];
                                            dr["BOM数量"] = Convert.ToDecimal(r3["数量"]);
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception(string.Format("物料{0}BOM数量有问题 速找开发部确认", r3["产品编码"]));

                                        }
                                        dr["创建日期"] = t;
                                        dr["修改日期"] = t;
                                        dr["制单人员"] = CPublic.Var.localUserName;
                                        dr["制单人员ID"] = CPublic.Var.LocalUserID;

                                        dr["工单负责人"] = textBox15.Text;
                                        dr["工单负责人ID"] = searchLookUpEdit1.EditValue;
                                        dr["领料人ID"] = searchLookUpEdit2.EditValue;
                                        dr["领料人"] = textBox16.Text;
                                        pos++;
                                        ds.Tables[1].Rows.Add(dr);
                                        dec_总需数 = -1;   //此次循环 
                                        break;
                                    }
                                }


                            }

                            if (dec_总需数 >= 0)     // 几种物料 单独一个库存不够所需领取数量时 
                            {

                                DataTable dt_替代 = new DataTable();
                                dt_替代 = fun_替代料递归(dt_替代, drr["物料编码"].ToString(), dec_剩, r["组"].ToString(), 2);

                                //原优先级为1的物料需要多少数量
                                decimal a = 0;
                                if (dt_替代.Rows.Count > 0)
                                {
                                    a = Convert.ToDecimal(dt_替代.Rows[dt_替代.Rows.Count - 1]["剩余数"]);
                                }
                                else
                                {
                                    a = dec_总需数;
                                }
                                if (Convert.ToDecimal(r_库存["库存总数"]) + a > 0)
                                {
                                    DataRow rrr = dt_替代.NewRow();
                                    rrr["产品编码"] = r["产品编码"];
                                    rrr["子项编码"] = r["子项编码"];
                                    rrr["产品名称"] = r["产品名称"];
                                    rrr["子项名称"] = r["子项名称"];
                                    rrr["A面位号"] = r["A面位号"];
                                    rrr["B面位号"] = r["B面位号"];

                                    rrr["n原ERP规格型号"] = r["n原ERP规格型号"];
                                    rrr["需要数"] = Convert.ToDecimal(r_库存["库存总数"]) + a;
                                    dt_替代.Rows.Add(rrr);

                                }

                                foreach (DataRow dr_替代 in dt_替代.Rows)
                                {
                                    DataRow dr_1 = ds.Tables[1].NewRow();
                                    dr_1["待领料单号"] = str_待领料单号;
                                    dr_1["待领料单明细号"] = str_待领料单号 + "-" + pos.ToString("00");
                                    dr_1["生产工单号"] = drr["生产工单号"];
                                    dr_1["生产制令单号"] = drr["生产制令单号"];
                                    dr_1["生产工单类型"] = drr["生产工单类型"];
                                    dr_1["生产车间"] = drr["生产车间"]; //车间编号
                                    dr_1["A面位号"] = r["A面位号"];
                                    dr_1["B面位号"] = r["B面位号"];

                                    dr_1["物料编码"] = dr_替代["子项编码"];
                                    dr_1["物料名称"] = dr_替代["子项名称"];
                                    dr_1["规格型号"] = dr_替代["n原ERP规格型号"].ToString().Trim();
                                    try
                                    {
                                        dr_1["待领料总量"] = dr_替代["需要数"];
                                        dr_1["未领数量"] = dr_1["待领料总量"];
                                        dr_1["BOM数量"] = Convert.ToDecimal(r["数量"]);

                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception(string.Format("物料{0}BOM数量有问题 速找开发部确认", r["产品编码"]));

                                    }
                                    dr_1["创建日期"] = t;
                                    dr_1["修改日期"] = t;
                                    dr_1["制单人员"] = CPublic.Var.localUserName;
                                    dr_1["制单人员ID"] = CPublic.Var.LocalUserID;

                                    dr_1["工单负责人"] = textBox15.Text;
                                    dr_1["工单负责人ID"] = searchLookUpEdit1.EditValue;
                                    dr_1["领料人ID"] = searchLookUpEdit2.EditValue;
                                    dr_1["领料人"] = textBox16.Text;
                                    pos++;
                                    ds.Tables[1].Rows.Add(dr_1);
                                }
                            }



                        }
                    }
                    else      //BOM无备料 原来的流程
                    {
                        foreach (DataRow r in dt_bom.Rows)
                        {
                            DataRow dr = ds.Tables[1].NewRow();
                            dr["待领料单号"] = str_待领料单号;
                            dr["待领料单明细号"] = str_待领料单号 + "-" + pos.ToString("00");
                            dr["生产工单号"] = drr["生产工单号"];
                            dr["生产制令单号"] = drr["生产制令单号"];
                            dr["生产工单类型"] = drr["生产工单类型"];
                            dr["生产车间"] = drr["生产车间"]; //车间编号
                            dr["A面位号"] = r["A面位号"];
                            dr["B面位号"] = r["B面位号"];

                            dr["物料编码"] = r["子项编码"];
                            dr["物料名称"] = r["子项名称"];
                            dr["规格型号"] = r["n原ERP规格型号"].ToString().Trim();
                            try
                            {
                                dr["待领料总量"] = Convert.ToDecimal(drr["生产数量"]) * Convert.ToDecimal(r["数量"]);
                                dr["未领数量"] = dr["待领料总量"];

                                dr["BOM数量"] = Convert.ToDecimal(r["数量"]);

                            }
                            catch (Exception ex)
                            {
                                throw new Exception(string.Format("物料{0}BOM数量有问题 速找开发部确认", r["产品编码"]));

                            }
                            dr["创建日期"] = t;
                            dr["修改日期"] = t;
                            dr["制单人员"] = CPublic.Var.localUserName;
                            dr["制单人员ID"] = CPublic.Var.LocalUserID;

                            dr["工单负责人"] = textBox15.Text;
                            dr["工单负责人ID"] = searchLookUpEdit1.EditValue;
                            dr["领料人ID"] = searchLookUpEdit2.EditValue;
                            dr["领料人"] = textBox16.Text;
                            pos++;
                            ds.Tables[1].Rows.Add(dr);

                        }

                    }
                }
            }

            return (ds);

        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_生效刷新数量()
#pragma warning restore IDE1006 // 命名样式
        {



        }
#pragma warning disable IDE1006 // 命名样式
        private DataTable fun_替代料递归(DataTable dt_传递, string str_产品, decimal dec_总需, string str_组, int i_顺序)  //取替代料 
#pragma warning restore IDE1006 // 命名样式
        {


            DataTable dt = new DataTable();
            string sql = string.Format(@"select 基础数据物料BOM表.*,基础数据物料信息表.n原ERP规格型号,库存总数,需要数=0,剩余数=0  from 基础数据物料BOM表 
                                left join  基础数据物料信息表 on 基础数据物料BOM表.子项编码=基础数据物料信息表.物料编码 
                                left join 仓库物料数量表 on  仓库物料数量表.物料编码=基础数据物料信息表.物料编码
            where 基础数据物料BOM表.产品编码='{0}' and  基础数据物料BOM表.主辅料<>'包装'and 优先级='{1}' and 组='{2}' ", str_产品, i_顺序, str_组);
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);


            if (dt.Rows.Count > 0)
            {
                if (dt_传递.Columns.Count == 0)
                {
                    dt_传递 = dt.Clone();
                }
                if (dec_总需 > 0)
                {

                    if (dec_总需 <= Convert.ToDecimal(dt.Rows[0]["库存总数"]))   //顺序为i的替代料 库存够
                    {
                        dt.Rows[0]["需要数"] = dec_总需;
                        dt.Rows[0]["剩余数"] = 0;

                        dt_传递.ImportRow(dt.Rows[0]);
                        dec_总需 = 0;
                        return dt;
                    }
                    else if (Convert.ToDecimal(dt.Rows[0]["库存总数"]) > 0)        //有替代料 但仍然不够
                    {
                        dt.Rows[0]["需要数"] = Convert.ToDecimal(dt.Rows[0]["库存总数"]);

                        dec_总需 = dec_总需 - Convert.ToDecimal(dt.Rows[0]["库存总数"]);
                        dt.Rows[0]["剩余数"] = dec_总需;
                        dt_传递.ImportRow(dt.Rows[0]);

                        dt_传递 = fun_替代料递归(dt_传递, str_产品, dec_总需, str_组, ++i_顺序);

                    }
                }


            }
            return dt_传递;

        }
        //扣除 虚拟库存中 对应物料的量
#pragma warning disable IDE1006 // 命名样式
        private void fun_扣车间虚拟库存(string str_虚拟_工单)
#pragma warning restore IDE1006 // 命名样式
        {


            string sql_主 = string.Format(@"update 生产记录车间虚拟库存表 set 车间数量=生产记录车间虚拟库存表.车间数量-a.已领数量 from 
                                (select 生产记录车间虚拟库存表.物料编码,生产记录生产工单待领料明细表.已领数量 from [生产记录车间虚拟库存表],生产记录生产工单待领料明细表 
                                    where 生产记录车间虚拟库存表.物料编码 in (select 物料编码 from 生产记录生产工单待领料明细表 )
                            and  生产记录车间虚拟库存表.物料编码=生产记录生产工单待领料明细表.物料编码 and  生产记录生产工单待领料明细表.生产工单号='{0}')a 
                                            where 生产记录车间虚拟库存表.物料编码=a.物料编码", str_虚拟_工单);

            //using (SqlDataAdapter da = new SqlDataAdapter(sql_主, strconn)) ;
        }



        #endregion
        #region 界面操作

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            DataRow r = gv.GetDataRow(gv.FocusedRowHandle);
            if (r != null)
            {
                dataBindHelper1.DataFormDR(r);

                if (r["生效"].Equals(true))
                {
                    barLargeButtonItem2.Enabled = false;
                    //barLargeButtonItem3.Enabled = false;
                    //barLargeButtonItem4.Enabled = false;
                    //若工单已生效 数量不可更改
                    textBox7.Enabled = false;
                    if ((r["完成"].Equals(true) || r["完工"].Equals(true)) && r["部分完工"].Equals(false))
                    {
                        barLargeButtonItem3.Enabled = false;
                        barLargeButtonItem8.Enabled = false;
                        barLargeButtonItem10.Enabled = false;

                    }
                    else
                    {
                        barLargeButtonItem3.Enabled = true;
                        barLargeButtonItem4.Enabled = true;
                        barLargeButtonItem8.Enabled = true;
                        barLargeButtonItem10.Enabled = true;
                    }
                }
                else
                {
                    //若工单未生效 数量可更改
                    textBox7.Enabled = true;
                    barLargeButtonItem2.Enabled = true;
                    barLargeButtonItem3.Enabled = true;
                    barLargeButtonItem4.Enabled = true;
                    barLargeButtonItem8.Enabled = false;
                    barLargeButtonItem10.Enabled = false;
                    dateEdit1.EditValue = Convert.ToDateTime(CPublic.Var.getDatetime().ToString("yyyy-MM-dd")); //预计开工日期
                                                                                                                //dateEdit2.EditValue = System.DateTime.Now;   //预计完工日期
                                                                                                                //if (dr.Length > 0)
                                                                                                                //{
                                                                                                                //    searchLookUpEdit2.EditValue = dr[0]["工号"];
                                                                                                                //}
                }
                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc, new Point(e.X, e.Y));
                    gv.CloseEditor();
                    this.BindingContext[dtM].EndCurrentEdit();
                }
            }
        }
        //刷新
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            ERPorg.Corg.FlushMemory();
            str_工单号 = null;
            frm工单生效选择_Load(null, null);
            DataRow dr_k = dtM.NewRow();
            dataBindHelper1.DataFormDR(dr_k);

            //清空开工完工日期
            dateEdit1.EditValue = null;
            dateEdit2.EditValue = null;

            //恢复按钮视图
            barLargeButtonItem2.Enabled = true;
            //barLargeButtonItem3.Enabled = true;
        }
        //生效
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            gv.CloseEditor();
            this.BindingContext[dtM].EndCurrentEdit();
            this.BindingContext[dv].EndCurrentEdit();
            //    Dictionary<int, String> dict = new Dictionary<int, String>();
            // int i = 0;
            //string str_刷新在制量 = ""; string str_刷新未领量 = "";
            try
            {
                label25.Visible = true;
                fun_check();
                //  List<string> li = fun_c_n();
                //  s_checkmessage = "成功记录:" + (dv.ToTable().Rows.Count - li.Count).ToString() + "条";
                s_checkmessage = "成功记录:" + dv.ToTable().Rows.Count.ToString() + "条";

                //if (li.Count > 0)
                //{
                //    s_checkmessage = s_checkmessage + ",库存不足未生效:" + li.Count.ToString() + "条.未生效工单:";
                //    foreach (string s in li)
                //    {
                //        s_checkmessage = s_checkmessage + s + ",";
                //        DataRow[] r = dtM.Select(string.Format("生产工单号='{0}'", s));
                //        r[0]["选择"] = false;
                //    }
                //    s_checkmessage = s_checkmessage.Substring(0, s_checkmessage.Length - 1);
                //}
                ////ck
                ///不继续运行
                if (ck != true)
                {

                    DataTable dt_界面 = dv.ToTable();
                    dt_界面.AcceptChanges();
                    {
                        DateTime time = CPublic.Var.getDatetime();
                        string str_id = CPublic.Var.LocalUserID;
                        string str_name = CPublic.Var.localUserName;

                        DataTable dt_制令数量 = new DataTable();
                        DataTable dt_MIcach = new DataTable(); //原料库存缓存

                        DataSet ds = new DataSet();
                        DataSet ds_back = new DataSet();
                        ds.Tables.Add();
                        ds.Tables.Add();
                        ds.Tables.Add(dt_刷新数量);
                        ds.Tables.Add(dt_MIcach);


                        ds.DataSetName = "x";
                        ds.Tables[0].TableName = "ds0";
                        ds.Tables[1].TableName = "ds1";
                        ds.Tables[2].TableName = "list_原料刷新";
                        ds.Tables[3].TableName = "list_库存缓存";

                        // ds.Tables.Add(dt_MIcach);//2018-6-19 用于存放库存 而不是 每个工单都重新取一遍库存
                        int i = 0;
                        foreach (DataRow drM in dt_界面.Rows)
                        {
                            //取 dr["物料编码"]及其所有子项得 计算结果 
                            //DataRow dr


                            //19-5-1上线后用不到，功能已去除
                            if (drM["生产工单类型"].ToString() == "改制工单")
                            {
                                drM["生效"] = true;
                                drM["生效人"] = str_name;
                                drM["生效人ID"] = str_id;
                                drM["生效日期"] = time;
                                drM["预计开工日期"] = dateEdit1.EditValue;
                                // drM["预计完工日期"] = dateEdit2.EditValue;
                                drM["工单负责人ID"] = searchLookUpEdit1.EditValue;
                                drM["工单负责人"] = textBox15.Text;
                                dt_制令数量 = fun_gdsx(Convert.ToDecimal(drM["生产数量"].ToString()), drM["生产制令单号"].ToString(), dt_制令数量);
                                //ds.Tables[2].Rows.Add();
                                string s = string.Format(" select  物料编码 as 子项编码 from  生产记录生产工单待领料明细表  where 生产工单号='{0}'", drM["生产工单号"].ToString());
                                using (SqlDataAdapter da = new SqlDataAdapter(s, strconn))
                                {
                                    da.Fill(ds.Tables[2]);
                                }
                                continue;
                            }
                            //////
                            //                        string x = string.Format(@"select 产品编码,a.物料编码,库存总数,有效总数,b.计量单位 as bom单位,a.计量单位 as 库存单位,单位换算标识,单位换算标识 from 仓库物料数量表 a
                            //                        left  join 基础数据物料信息表 base on base.物料编码=a.物料编码
                            //                        Left  join 基础数据物料BOM表 b on a.物料编码=b.子项编码 and a.仓库号=b.仓库号 where   产品编码='{0}'", drM["物料编码"].ToString());
                            string x = string.Format(@"select 产品编码,a.物料编码,库存总数,有效总数,b.计量单位 as bom单位,单位换算标识,单位换算标识 from 仓库物料数量表 a
                         left  join 基础数据物料信息表 base on base.物料编码=a.物料编码
                         Left  join 基础数据物料BOM表 b on a.物料编码=b.子项编码 and a.仓库号=b.仓库号 where   产品编码='{0}'", drM["物料编码"].ToString());

                            DataTable temp = CZMaster.MasterSQL.Get_DataTable(x, strconn);
                            if (dt_MIcach.Columns.Count == 0)
                            {
                                dt_MIcach = CZMaster.MasterSQL.Get_DataTable(x, strconn);
                            }
                            else
                            {
                                DataRow[] r = dt_MIcach.Select(string.Format("产品编码='{0}'", drM["物料编码"].ToString()));
                                if (r.Length == 0) //该成品未加载过
                                {
                                    foreach (DataRow dr in temp.Rows)
                                    {
                                        if (dt_MIcach.Select(string.Format("物料编码='{0}'", dr["物料编码"].ToString())).Length == 0) //dt_MIcach 里先找有没有  没有就添进去
                                        {
                                            dt_MIcach.ImportRow(dr);
                                            if (dr["单位换算标识"].Equals(true)) //
                                            {
                                                string ss = string.Format("select  * from 计量单位换算表 where 物料编码='{0}'", dr["物料编码"]);
                                                using (SqlDataAdapter da = new SqlDataAdapter(ss, strconn))
                                                {
                                                    DataTable dt = new DataTable();
                                                    da.Fill(dt);
                                                    DataRow[] r1 = dt.Select(string.Format("计量单位='{0}'", dr["bom单位"].ToString().Trim()));
                                                    DataRow[] r2 = dt.Select(string.Format("计量单位='{0}'", dr["库存单位"].ToString().Trim()));
                                                    decimal dec = Convert.ToDecimal(r2[0]["换算率"]) / Convert.ToDecimal(r1[0]["换算率"]);
                                                    //DataRow []rr=  dt.Select(string.Format("计量单位='{0}'", dr["库存单位"].ToString().Trim()));
                                                    dr["有效总数"] = dec * Convert.ToDecimal(dr["有效总数"]);
                                                    dr["库存总数"] = dec * Convert.ToDecimal(dr["库存总数"]);
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                            ds.Tables.RemoveAt(3);
                            ds.Tables.Add(dt_MIcach);
                            ds.Tables[3].TableName = "list_库存缓存";

                            DataColumn[] pk_bom = new DataColumn[2];
                            pk_bom[0] = dt_MIcach.Columns["产品编码"];
                            pk_bom[1] = dt_MIcach.Columns["物料编码"];
                            dt_MIcach.PrimaryKey = pk_bom;


                            string s_版本 = string.Format(@" with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,bom_level ) as
                                            (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,1 as level from 基础数据物料BOM表 
                                             where 产品编码='{0}'
                                           union all 
                                           select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型,a.数量,a.bom类型,b.bom_level+1  from 基础数据物料BOM表 a
                                           inner join temp_bom b on a.产品编码=b.子项编码 
                                           ) 
                                           select 子项编码,子项名称,文件名 from (
                                              select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,wiptype,子项类型,数量,bom类型,a.仓库号,a.仓库名称,
                                              bom_level,base.规格型号 as 子项规格,isnull(文件名,'')文件名  from  temp_bom a
                                          left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
                                          left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码
                                           left join   (  select  a.* from 程序版本维护表 a
                                          inner join (select  物料号,MAX(版本)maxbb from 程序版本维护表 where 停用=0  group by 物料号) b 
                                          on a.物料号=b.物料号 and  a.版本=b.maxbb ) bb on bb.物料号 =子项编码 
                                          group by 产品编码,fx.物料名称 ,子项编码,base.物料名称 ,wiptype,子项类型,数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号,文件名
                                           ) aaaa group by 子项编码,子项名称,文件名
                                          ", drM["物料编码"]);
                            DataTable dt_Bomm = CZMaster.MasterSQL.Get_DataTable(s_版本, strconn);
                             
                            string sql11 = string.Format(@"   SELECT 文件名, 物料号, 版本 FROM 程序版本维护表 WHERE 版本 = (SELECT MAX(版本) FROM    程序版本维护表 where  物料号 ='{0}' and 停用='0' ) and 物料号 = '{0}'  and 停用='0' ", drM["物料编码"]);
                            DataRow dr_1 = CZMaster.MasterSQL.Get_DataRow(sql11, strconn);
                            string v_number = "";
                            if (dr_1 != null)
                            {
                                 v_number = dr_1["文件名"].ToString();
                            }
                          
                            foreach (DataRow drr in dt_Bomm.Rows)
                            {
                                

                                    if (drr["文件名"].ToString() != "")
                                    {
                                        if (v_number == "")
                                        {
                                            v_number = v_number + drr["文件名"].ToString();
                                        }
                                        else
                                        {
                                            v_number = v_number + ";" + drr["文件名"].ToString();
                                        }
                              

                                }
                            }
                            //DataRow[] dr2 = dt_界面.Select(string.Format("生产工单号='{0}'", drM["生产工单号"].ToString()));

                            //dr2[0]["版本备注"] = v_number.ToString();
                            ////dr["版本备注"] = v_number.ToString();


                            drM["领料人ID"] = searchLookUpEdit2.EditValue;
                            drM["领料人"] = textBox16.Text;
                            drM["生效"] = true;
                            drM["生效人"] = str_name;
                            drM["版本备注"] = v_number;
                            drM["生效人ID"] = str_id;
                            drM["生效日期"] = time;
                            drM["预计开工日期"] = dateEdit1.EditValue;
                            // drM["预计完工日期"] = dateEdit2.EditValue;
                            drM["工单负责人ID"] = searchLookUpEdit1.EditValue;
                            drM["工单负责人"] = textBox15.Text;
                            drM["班组"] = textBox15.Text;
                            drM["班组ID"] = searchLookUpEdit1.EditValue;
                            // 更改对应制令号的数量 已排单和未排单数量  in 生产记录生产制令表 

                            dt_制令数量 = fun_gdsx(Convert.ToDecimal(drM["生产数量"].ToString()), drM["生产制令单号"].ToString(), dt_制令数量);
                            label25.Text = "正在生成领料单...";
                            Application.DoEvents();
                            //工单生效 生成  待 领料单明细
                            // ds = fun_save(ds, drM);
                            //17-12-8 
                            //DateTime t = CPublic.Var.getDatetime();
                            DataTable dt_temp = drM.Table.Clone();
                            dt_temp.TableName = "drm";
                            dt_temp.ImportRow(drM);

                            string str_待领料单号 = string.Format("DL{0}{1:00}{2:0000}",
                                                    time.Year, time.Month,
                                                    CPublic.CNo.fun_得到最大流水号("DL", time.Year, time.Month));


                            //dt_MIcach.TableName = "list_库存缓存";
                            //ds = WSAdapter.webservers_getdata.wsfun.fun_lld(ds, dt_temp, str_id, str_name, searchLookUpEdit2.EditValue.ToString(), textBox16.Text, searchLookUpEdit1.EditValue.ToString(), textBox15.Text, str_待领料单号);
                            ds = StockCore.StockCorer.fun_lld(ds, dt_temp, str_id, str_name, searchLookUpEdit2.EditValue.ToString(), textBox16.Text, searchLookUpEdit1.EditValue.ToString(), textBox15.Text, str_待领料单号);
                            //本地webservers 测试


                            //ds = WSAdapter.webservers_getdata.wsmo.fun_lld(ds, dt_temp, CPublic.Var.LocalUserID, CPublic.Var.localUserName, searchLookUpEdit2.EditValue.ToString(), textBox16.Text, searchLookUpEdit1.EditValue.ToString(), textBox15.Text, str_待领料单号 );
                            dt_MIcach = ds.Tables[3];
                            // ds = StockCore.StockCorer.fun_lld(ds, dt_temp, CPublic.Var.LocalUserID, CPublic.Var.localUserName, searchLookUpEdit2.EditValue.ToString(), textBox16.Text, searchLookUpEdit1.EditValue.ToString(), textBox15.Text, str_待领料单号, dt_MIcach);

                        }

                         //19-11-21 有些物料仓库物料信息表中没有记录 需要添加进去 如果没有影响未领 导致计划池不准
                        DataTable dt_kc = StockCore.StockCorer.KCRecord(dt_界面);

                        DataSet ds_sn = null;
                        if (CPublic.Var.localUser部门名称 != "生产二厂") //19-11-14正式库还没启用 现有是研发部内部电脑做的服务器 二厂连不上
                        {
                            ERPorg.Corg xx = new ERPorg.Corg();
                            ds_sn = xx.fun_SN(dt_界面);
                        }
                        label25.Text = "正在保存...";
                        Application.DoEvents();
                        string sql_baocun = "select * from 生产记录生产工单表  where 1<>1";
                        string sql_制令数量 = "select * from 生产记录生产制令表 where 1<>1";
                        string sql_待主 = "select * from 生产记录生产工单待领料主表 where 1<>1";
                        string sql_待明细 = "select * from 生产记录生产工单待领料明细表 where 1<>1";

                        SqlConnection conn = new SqlConnection(strconn);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("工单生效");
                        try
                        {
                            SqlCommand cmm_0 = new SqlCommand(sql_baocun, conn, ts);
                            SqlDataAdapter da_cun = new SqlDataAdapter(cmm_0);


                            SqlCommand cmm_1 = new SqlCommand(sql_制令数量, conn, ts);
                            SqlCommand cmm_2 = new SqlCommand(sql_待主, conn, ts);
                            SqlCommand cmm_3 = new SqlCommand(sql_待明细, conn, ts);
                
                            SqlDataAdapter da_制令数量 = new SqlDataAdapter(cmm_1);
                            new SqlCommandBuilder(da_制令数量);
                            da_制令数量.Update(dt_制令数量);

                            SqlDataAdapter da_待主 = new SqlDataAdapter(cmm_2);
                            new SqlCommandBuilder(da_待主);
                            da_待主.Update(ds.Tables[0]);

                            SqlDataAdapter da_待明细 = new SqlDataAdapter(cmm_3);
                            new SqlCommandBuilder(da_待明细);
                            da_待明细.Update(ds.Tables[1]);

                            // da_cun.Update(dt_界面);
                            if (ds_sn != null)
                            {
                                new SqlCommandBuilder(da_cun);
                                da_cun.Update(ds_sn.Tables[2]);

                                sql_baocun = "select * from Print_ShareLockInfo where 1=2 ";
                                cmm_0 = new SqlCommand(sql_baocun, conn, ts);
                                da_cun = new SqlDataAdapter(cmm_0);
                                new SqlCommandBuilder(da_cun);
                                da_cun.Update(ds_sn.Tables[1]);
                            }
                            else
                            {
                                new SqlCommandBuilder(da_cun);
                                da_cun.Update(dt_界面);
                            }

                            sql_baocun = "select * from 仓库物料数量表 where 1=2 ";
                            cmm_0 = new SqlCommand(sql_baocun, conn, ts);
                            da_cun = new SqlDataAdapter(cmm_0);
                            new SqlCommandBuilder(da_cun);
                            da_cun.Update(dt_kc);


                            ts.Commit();
                            if (ds_sn != null)
                            {
                                ///2019-10-16  这边要保存另一个数据库  目前我不知道怎么两个数据用类似事务的方式一起保存 
                                string str_BQ = CPublic.Var.geConn("BQ");
                                CZMaster.MasterSQL.Save_DataTable(ds_sn.Tables[0], "ShareLockInfo", str_BQ);
                            }
                        }
                        catch (Exception ex)
                        {
                            ts.Rollback();
                            throw new Exception("工单生效失败");
                        }
                        //Thread ths;
                        DataSet ds_刷新 = new DataSet();
                        ds_刷新.Tables.Add(ds.Tables[2].Copy());
                        ds_刷新.Tables.Add(dv.ToTable().Copy());

                        try
                        {
                            fun_刷新(ds_刷新); //多线程好像经常挂掉,放在这边看看 究竟什么原因;

                            // MessageBox.Show("生效成功");
                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show("工单生效成功,但刷新在制量错误" + ex.Message);
                        }

                        //ths = new Thread(() => fun_多线程刷新(ds_刷新));
                        //ths.IsBackground = true;
                        //ths.Start();
                        label25.Visible = false;

                        str_工单号 = null;
                        barLargeButtonItem1_ItemClick(null, null);
                        dt_刷新数量 = new DataTable();
                        label25.Visible = false;

                        MessageBox.Show(s_checkmessage);
                        str_工单号 = null;
                        barLargeButtonItem1_ItemClick(null, null);
                        dt_刷新数量 = new DataTable();
                    }
                    barLargeButtonItem1_ItemClick(null, null);
                    if (index != 0 && index <= gv.DataRowCount)
                    {
                        gv.FocusedRowHandle = index;

                    }
                    else if (index > gv.DataRowCount)
                    {
                        gv.FocusedRowHandle = gv.DataRowCount;
                    }
                    //gv.MoveBy(0);
                }



            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                //barLargeButtonItem1_ItemClick(null, null);
            }

        }
        /// <summary>
        /// 19-11-21 发现库存记录中没有记录 未领量算不出来会影响 计划池
        /// </summary>
        private void fun_kcjl()
        {


        }
        //参数为 webservers 返回ds 的 ds.Tables[2].Copy()
        //参数为 webservers 返回ds 的 ds.Tables[2].Copy()
#pragma warning disable IDE1006 // 命名样式
        private void fun_刷新(DataSet ds_cs)
#pragma warning restore IDE1006 // 命名样式
        {


            DataTable dtx = ds_cs.Tables[1].Copy();
            dtx.TableName = "成品";
            int c = dtx.Rows.Count;
            for (int j = 0; j < c; j++)
            {
                DataRow[] rr = dtx.Select(string.Format("物料编码='{0}'", dtx.Rows[j]["物料编码"].ToString()));
                if (rr.Length > 1)
                {
                    dtx.Rows.Remove(dtx.Rows[j]);
                }
                j--;
                c--;
            }
            //DataTable dt_成品刷新 = StockCore.StockCorer.fun_四个量(dtx);
            DataTable dt_成品刷新 = new DataTable();

            dt_成品刷新 = StockCore.StockCorer.fun_四个量(dtx);
            //本地webservers 测试
            //dt_成品刷新 = WSAdapter.webservers_getdata.wsmo.fun_四个量(dtx);
            dt_刷新数量 = new DataTable();
            dt_刷新数量 = ds_cs.Tables[0].Copy();
            if (dt_刷新数量.Columns.Contains("子项编码"))
            {
                dt_刷新数量.Columns["子项编码"].ColumnName = "物料编码";

            }

            //DataTable dt_原料刷新 = StockCore.StockCorer.fun_四个量(dt_刷新数量);
            DataTable dt_原料刷新 = new DataTable();
            c = dt_刷新数量.Rows.Count;
            for (int j = 0; j < c; j++)
            {
                DataRow[] rr = dt_刷新数量.Select(string.Format("物料编码='{0}'", dt_刷新数量.Rows[j]["物料编码"].ToString()));
                if (rr.Length > 1)
                {
                    dt_刷新数量.Rows.Remove(dt_刷新数量.Rows[j]);
                }
                j--;
                c--;
            }
            dt_原料刷新 = StockCore.StockCorer.fun_四个量(dt_刷新数量);

            //本地webservers 测试
            // dt_原料刷新 = WSAdapter.webservers_getdata.wsmo.fun_四个量(dt_刷新数量);

            // 一起生效多张单子 有可能dt_成品中物料可能会出现在 dt_原料中 
            //去除重复然后合并 
            foreach (DataRow dr in dt_成品刷新.Rows)
            {
                DataRow[] r = dt_原料刷新.Select(string.Format("物料编码='{0}'", dr["物料编码"].ToString()));
                if (r.Length > 0)
                {
                    dt_原料刷新.Rows.Remove(r[0]);
                }
            }
            dt_成品刷新.Merge(dt_原料刷新.Copy());

            string ss = "select  * from 仓库物料数量表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(ss, strconn))
            {
                new SqlCommandBuilder(da);
                da.Update(dt_成品刷新);
            }

        }

        //关闭订单
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                this.BindingContext[dv].EndCurrentEdit();
                DateTime time = CPublic.Var.getDatetime();
                if (MessageBox.Show("确定关闭该工单？请核对。", "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                    string sql_退 = string.Format("select count(*)x from 工单退料申请表 where 生产工单号 = '{0}' and 完成=0 and 作废=0", dr["生产工单号"].ToString());
                    DataTable dt_tui = CZMaster.MasterSQL.Get_DataTable(sql_退, strconn);
                    if (Convert.ToInt32(dt_tui.Rows[0]["x"]) > 0)
                    {
                        throw new Exception("该单据有退料申请未完成，不可操作");
                    }
                    frm关闭工单原因 fm = new frm关闭工单原因(dr);
                    fm.ShowDialog();
                    if (fm.flag == true)
                    {
                        if (textBox1.Text != "" && dr["生产工单号"].ToString() == textBox1.Text)
                        {

                            DateTime t1   = CPublic.Var.getDatetime();
                            dateEdit1.EditValue = null;
                            dateEdit2.EditValue = null;
                            DataTable dt_工单;
                            DataTable dt_制令 = new DataTable();
                            DataTable dt_领料主 = new DataTable();
                            DataTable dt_领料明细;
                            string sql = string.Format("select * from 生产记录生产工单表 where 生产工单号='{0}'", dr["生产工单号"]);
                            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                            {
                                dt_工单 = new DataTable();
                                da.Fill(dt_工单);
                                dt_工单.Rows[0]["关闭"] = 1;
                                dt_工单.Rows[0]["关闭日期"] = t1;
                                dt_工单.Rows[0]["关闭人员ID"] = CPublic.Var.LocalUserID;
                                dt_工单.Rows[0]["关闭人员"] = CPublic.Var.localUserName;
                                dt_工单.Rows[0]["备注4"] = fm.str;
                            }
                            string sql_zl = "";
                            string sql_3 = "";
                            if (dr["生效"].Equals(true))
                            {
                                sql_zl = string.Format("select * from 生产记录生产制令表 where 生产制令单号='{0}'", dr["生产制令单号"]);
                                using (SqlDataAdapter da = new SqlDataAdapter(sql_zl, strconn))
                                {
                                    dt_制令 = new DataTable();
                                    da.Fill(dt_制令);
                                    dt_制令.Rows[0]["已排单数量"] = Convert.ToInt32(dt_制令.Rows[0]["已排单数量"]) - Convert.ToInt32(dr["生产数量"])+Convert.ToInt32(dr["部分完工数"]);
                                    dt_制令.Rows[0]["未排单数量"] = Convert.ToInt32(dt_制令.Rows[0]["未排单数量"]) + Convert.ToInt32(dr["生产数量"])- Convert.ToInt32(dr["部分完工数"]);
                                    
                                }
                                // 关闭该条待领料 记录
                                sql_3 = string.Format("select * from [生产记录生产工单待领料主表] where 生产工单号='{0}'", dr["生产工单号"]);
                                using (SqlDataAdapter da = new SqlDataAdapter(sql_3, strconn))
                                {
                                    dt_领料主 = new DataTable();
                                    da.Fill(dt_领料主);
                                    if (dt_领料主.Rows.Count > 0)
                                    {
                                        dt_领料主.Rows[0]["关闭"] = 1;
                                        dt_领料主.Rows[0]["关闭时间"] = t1;
                                    }
                                }
                            }
                            SqlConnection conn_close = new SqlConnection(strconn);
                            conn_close.Open();
                            SqlTransaction ts_close = conn_close.BeginTransaction("工单关闭");
                            try
                            {
                                SqlCommand cmm_1 = new SqlCommand(sql, conn_close, ts_close);
                                SqlDataAdapter da_工单 = new SqlDataAdapter(cmm_1);
                                new SqlCommandBuilder(da_工单);
                                da_工单.Update(dt_工单);
                                if (sql_zl != "")
                                {
                                    SqlCommand cmm_2 = new SqlCommand(sql_zl, conn_close, ts_close);
                                    SqlCommand cmm_3 = new SqlCommand(sql_3, conn_close, ts_close);
                                    SqlDataAdapter da_制令 = new SqlDataAdapter(cmm_2);
                                    SqlDataAdapter da_待主 = new SqlDataAdapter(cmm_3);
                                    new SqlCommandBuilder(da_制令);
                                    new SqlCommandBuilder(da_待主);
                                    da_制令.Update(dt_制令);
                                    da_待主.Update(dt_领料主);
                                }
                                ts_close.Commit();
                            }
                            catch (Exception ex)
                            {
                                ts_close.Rollback();
                                throw new Exception("关闭失败,刷新重试");
                            }
                            StockCore.StockCorer.fun_物料数量_实际数量(dr["物料编码"].ToString(), dr["仓库号"].ToString(), true);
                            string sql_领料明细 = string.Format("select * from [生产记录生产工单待领料明细表] where 生产工单号='{0}' and  已领数量>0 ", dr["生产工单号"]);
                            using (SqlDataAdapter da = new SqlDataAdapter(sql_领料明细, strconn))
                            {
                                dt_领料明细 = new DataTable();
                                da.Fill(dt_领料明细);
                                if (dt_领料明细.Rows.Count > 0)
                                {
                                    if (MessageBox.Show("该工单已发过料,是否跳转至工单退料申请界面", "询问!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                                    {
                                        ERPproduct.UI工单退料申请 ui = new UI工单退料申请(dr["生产工单号"].ToString());
                                        CPublic.UIcontrol.Showpage(ui, "工单退料申请");

                                    }
                                }

                            }
                        }

                    }

                    barLargeButtonItem1_ItemClick(null, null);
                    if (index != 0 && index <= gv.DataRowCount)
                    {
                        gv.FocusedRowHandle = index;

                    }
                    else if (index > gv.DataRowCount)
                    {
                        gv.FocusedRowHandle = gv.DataRowCount;
                    }
                    //gv.MoveBy(0);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_保存返库单(DataTable dt_1, DataTable dt_2)
#pragma warning restore IDE1006 // 命名样式
        {
            string sql_1 = "select * from  工单返库单主表 where 1<>1 ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_1, strconn))
            {

                new SqlCommandBuilder(da);
                da.Update(dt_1);
            }
            string sql_明细 = "select * from 工单返库单明细表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_明细, strconn))
            {
                new SqlCommandBuilder(da);
                da.Update(dt_2);
            }
        }
        //关闭
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }
        //筛选
#pragma warning disable IDE1006 // 命名样式
        private void barEditItem1_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_只保存()
#pragma warning restore IDE1006 // 命名样式
        {
            if (textBox1.Text != "")
            {



                string sql = string.Format("select * from 生产记录生产工单表 where 生产工单号='{0}'", textBox1.Text.Trim());
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataBindHelper2.DataToDR(dt.Rows[0]);
                    dt.Rows[0]["预计开工日期"] = dateEdit1.EditValue;
                    dt.Rows[0]["预计完工日期"] = dateEdit2.EditValue;
                    dt.Rows[0]["未检验数量"] = Convert.ToDecimal(textBox7.Text);
                    new SqlCommandBuilder(da);
                    da.Update(dt);
                }
                //string sql = string.Format("select * from 生产记录生产工单表 where  1<>1");
                //using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                //{

                //    new SqlCommandBuilder(da);
                //    da.Update(dt_1);
                //}



            }
        }
        //修改 保存工单但不生效
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //fun_check();
                fun_只保存();
                MessageBox.Show("保存成功");
                barLargeButtonItem1_ItemClick(null, null);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }


        #endregion

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //textBox16.Text = "";

                if (gv.GetRow(e.RowHandle) == null)
                {
                    return;
                }
                //int j = gv.RowCount;
                //for (int i = 0; i < j; i++)
                //{
                if (gv.GetRowCellValue(e.RowHandle, "加急状态").ToString() == "加急")
                {
                    e.Appearance.BackColor = Color.Red;
                    e.Appearance.BackColor2 = Color.Red;
                }
                if (gv.GetRowCellValue(e.RowHandle, "加急状态").ToString() == "急")
                {
                    e.Appearance.BackColor = Color.Pink;
                    e.Appearance.BackColor2 = Color.Pink;
                }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //工单负责人 


#pragma warning disable IDE1006 // 命名样式
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
            {
                //  DataRow[] dr = dt_下拉.Select(string.Format("员工号='{0}'", searchLookUpEdit1.EditValue));
                DataRow[] dr = dt_班组.Select(string.Format("班组编号='{0}'", searchLookUpEdit1.EditValue));

                if (dr.Length > 0)
                {
                    textBox15.Text = dr[0]["班组"].ToString().Trim();
                }

                sql = string.Format("select  员工号,姓名 from 人事基础员工表 where 班组编号='{0}' and 在职状态='在职'", searchLookUpEdit1.EditValue);
                dt_指定领料人 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                searchLookUpEdit2.Properties.DataSource = dt_指定领料人;
                searchLookUpEdit2.Properties.DisplayMember = "员工号";
                searchLookUpEdit2.Properties.ValueMember = "员工号";

            }
        }



        // 指定领料人
#pragma warning disable IDE1006 // 命名样式
        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (searchLookUpEdit2.EditValue != null && searchLookUpEdit2.EditValue.ToString() != "")
            {
                DataRow[] dr = dt_指定领料人.Select(string.Format("员工号='{0}'", searchLookUpEdit2.EditValue.ToString()));
                if (dr.Length > 0)
                {
                    textBox16.Text = dr[0]["姓名"].ToString().Trim();
                }
            }
        }
        //查找 筛选
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            ERPorg.Corg.FlushMemory();
            textBox16.Text = "";
            textBox15.Text = "";
            //dv = new DataView(dtM);

            barLargeButtonItem2.Enabled = true;
            barLargeButtonItem3.Enabled = true;
            barLargeButtonItem4.Enabled = true;

            string sql_条件 = "";
            if (barEditItem1.EditValue.ToString() == "未生效")
            {
                sql_条件 = " and 生产记录生产工单表.生效=0  and  生产记录生产工单表.关闭=0";


                //dv.RowFilter = "生效='false'";

            }
            else if (barEditItem1.EditValue.ToString() == "已生效")
            {
                //dv.RowFilter = "生效='true' and  完工='false' ";

                sql_条件 = " and 生产记录生产工单表.生效=1 and 完工=0  and  生产记录生产工单表.关闭=0";

            }
            else if (barEditItem1.EditValue.ToString() == "全部")
            {

            }
            else if (barEditItem1.EditValue.ToString() == "已完工")
            {
                sql_条件 = " and 完工=1  and  生产记录生产工单表.关闭=0";

                //dv.RowFilter = "完工='true'";


            }
            else if (barEditItem1.EditValue.ToString() == "未完工")
            {

                sql_条件 = " and 完工=0 and 生产记录生产工单表.生效 =1  and  生产记录生产工单表.关闭=0";
                //dv.RowFilter = "完工='false' and  生效='true'";


            }
            else if (barEditItem1.EditValue.ToString() == "已关闭")
            {

                sql_条件 = " and  生产记录生产工单表.关闭=1";
                //dv.RowFilter = "完工='false' and  生效='true'";

            }
            //    left join (select 生产工单号,检验人员,包装确认 from 生产记录生产检验单主表 group by 生产工单号,检验人员,包装确认)a  on a.生产工单号=生产记录生产工单表.生产工单号
            if (CPublic.Var.LocalUserID.ToLower() == "admin" || CPublic.Var.LocalUserTeam == "管理员权限")
            {
                sql = string.Format(@"select 生产记录生产工单表.*  from 生产记录生产工单表 
                left join   基础数据物料信息表 on 基础数据物料信息表.物料编码=生产记录生产工单表.物料编码  
                where 生产记录生产工单表.制单日期>='{0}' and 生产记录生产工单表.制单日期<='{1}'  {2}", Convert.ToDateTime(barEditItem2.EditValue).ToString("yyyy-MM-dd")
                 , Convert.ToDateTime(barEditItem3.EditValue).AddDays(1).AddSeconds(-1), sql_条件);
                DataTable dt = new DataTable();
                dt = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
                //string sql_1 = string.Format("select 员工号,姓名,部门编号 from 人事基础员工表 where 在职状态 ='在职'");
                //dt_下拉 = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
            }
            else
            {
                DataTable dt = new DataTable();
                dt = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("没有该权限");
                    return;
                }


                //string sql_1 = string.Format("select 员工号,姓名,部门编号 from 人事基础员工表 where 课室编号='{0}' and 在职状态 ='在职'", dt.Rows[0]["生产车间"].ToString());
                //dt_下拉 = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
                //,检验人员,包装确认 left join (select 生产工单号,检验人员,包装确认 from 生产记录生产检验单主表 group by 生产工单号,检验人员,包装确认)a  on a.生产工单号=生产记录生产工单表.生产工单号   
                if (dt.Rows[0]["用户描述"].ToString() == "班组长")
                {
                    工单关闭ToolStripMenuItem.Visible = false;
                    查看标签记录ToolStripMenuItem.Visible = false;

                    sql = string.Format(@"select 生产记录生产工单表.*  from 生产记录生产工单表 
                           left join   基础数据物料信息表 on 基础数据物料信息表.物料编码=生产记录生产工单表.物料编码  
                          
                           where   生产记录生产工单表.制单日期>='{0}' and 生产记录生产工单表.制单日期<='{1}' 
                           and 生产记录生产工单表.生产车间='{2}'  {3}"
                        , Convert.ToDateTime(barEditItem2.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(barEditItem3.EditValue).AddDays(1).AddSeconds(-1),
                        dt.Rows[0]["生产车间"], sql_条件);
                    repositoryItemComboBox1.Items.Clear();
                    repositoryItemComboBox1.Items.Add("未完工");
                    repositoryItemComboBox1.Items.Add("已完工");

                    //   barEditItem1.EditValue = "未完工";

                    //   barEditItem1.Enabled = false;
                    barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barLargeButtonItem4.Enabled = false;
                }
                else
                {
                    //,检验人员,包装确认   left join (select 生产工单号,检验人员,包装确认 from 生产记录生产检验单主表 group by 生产工单号,检验人员,包装确认)a  on a.生产工单号=生产记录生产工单表.生产工单号    
                    sql = string.Format(@"select 生产记录生产工单表.* from 生产记录生产工单表 
                               left join   基础数据物料信息表 on 基础数据物料信息表.物料编码=生产记录生产工单表.物料编码  
                             where   生产记录生产工单表.制单日期>='{0}' and 生产记录生产工单表.制单日期<='{1}' {2}", Convert.ToDateTime(barEditItem2.EditValue).ToString("yyyy-MM-dd")
                        , Convert.ToDateTime(barEditItem3.EditValue).AddDays(1).AddSeconds(-1), sql_条件);
                    barEditItem1.Enabled = true;

                    //and 生产记录生产工单表.生产车间='{2}'， dt.Rows[0]["生产车间"],
                }
            }

            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                dtM = new DataTable();
                da.Fill(dtM);
                dtM.Columns.Add("选择", typeof(bool));
                dv = new DataView(dtM);
            }
            DateTime time = CPublic.Var.getDatetime().Date;

            dateEdit1.EditValue = Convert.ToDateTime(time.ToString("yyyy-MM-dd"));
            dateEdit2.EditValue = Convert.ToDateTime(time.AddDays(3).ToString("yyyy-MM-dd"));
            try
            {
                if (gv.DataRowCount > 0)
                {
                    DataRow r = gv.GetDataRow(0);
                    if (r["完成"].Equals(true) || r["完工"].Equals(true))
                    {
                        barLargeButtonItem2.Enabled = false;
                        barLargeButtonItem3.Enabled = false;
                        barLargeButtonItem4.Enabled = false;
                        barLargeButtonItem8.Enabled = false;
                        barLargeButtonItem10.Enabled = false;

                    }
                    else if (r["生效"].Equals(true))
                    {
                        barLargeButtonItem2.Enabled = false;
                        barLargeButtonItem3.Enabled = false;
                        barLargeButtonItem4.Enabled = false;
                    }
                    else
                    {
                        barLargeButtonItem2.Enabled = true;
                        barLargeButtonItem3.Enabled = true;
                        barLargeButtonItem4.Enabled = true;
                        dateEdit1.EditValue = Convert.ToDateTime(time.ToString("yyyy-MM-dd")); //预计开工日期
                        dateEdit2.EditValue = Convert.ToDateTime(time.ToString("yyyy-MM-dd"));   //预计完工日期
                    }
                    dataBindHelper1.DataFormDR(r);
                }
                else
                {
                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox3.Text = "";
                    textBox4.Text = "";
                    textBox5.Text = "";
                    textBox6.Text = "";
                    textBox7.Text = "";
                    textBox8.Text = "";

                    textBox10.Text = "";
                    textBox11.Text = "";
                    textBox12.Text = "";
                    textBox13.Text = "";


                    textBox15.Text = "";
                    dateEdit1.EditValue = null;
                    dateEdit2.EditValue = null;
                    searchLookUpEdit1.EditValue = null;
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            //gc.DataSource = dv;
            gc.DataSource = dtM;


        }

        [DllImport("winspool.drv")]
        public static extern bool SetDefaultPrinter(String Name); //调用win api将指定名称的打印机设置为默认打印机
        //打印  0511 ZF
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {


                //  ERPorg.Corg


                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                this.BindingContext[dv].EndCurrentEdit();



                DataSet ds = new DataSet();
                DataView dv_1 = new DataView(dtM)
                {
                    RowFilter = "选择=1",
                    Sort = "生产工单号"
                };

                //this.ParentForm.TopMost = true;
                //  ncopy_dy = nCopy;
                dt_dy = dv_1.ToTable();

                DataRow[] rrr = dtM.Select(string.Format("选择=1"));
                foreach (DataRow r in rrr)
                {
                    r["选择"] = false;
                }

                if (dt_dy.Rows.Count <= 0)

                {
                    throw new Exception("当前无数据打印");
                }


                //  DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                ERPreport.frm生产工单 frm = new ERPreport.frm生产工单(dt_dy);
                frm.ShowDialog();











                //PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                //this.printDialog1.Document = this.printDocument1;
                //int nCopy = 0;
                //string file = @"C://ProductPrintNum.txt";
                //string PrinterName = "";
                //try
                //{
                //    if (File.Exists(file) == true)
                //    {
                //        nCopy = Convert.ToInt32(System.IO.File.ReadAllText(file));
                //        this.printDocument1.PrinterSettings.Copies = (short)nCopy;
                //    }
                //    PrinterName = CPublic.Var.li_CFG["printer_MO"].ToString();

                //}
                //catch { }
                //if (PrinterName == "")
                //{
                //    DialogResult dr = this.printDialog1.ShowDialog();
                //    if (dr == DialogResult.OK)
                //    {
                //        //打印份数
                //        nCopy = this.printDocument1.PrinterSettings.Copies;
                //        //Get the number of Start Page
                //        int sPage = this.printDocument1.PrinterSettings.FromPage;
                //        //Get the number of End Page
                //        int ePage = this.printDocument1.PrinterSettings.ToPage;

                //        PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                //        SetDefaultPrinter(PrinterName);
                //    }
                //    else
                //    {
                //        return;

                //    }
                //}



                //DataSet ds = new DataSet();
                //DataView dv_1 = new DataView(dtM)
                //{
                //    RowFilter = "选择=1",
                //    Sort = "生产工单号"
                //};

                ////this.ParentForm.TopMost = true;
                //ncopy_dy = nCopy;
                //dt_dy = dv_1.ToTable();
                //Thread thDo;
                //thDo = new Thread(Dowork)
                //{
                //    IsBackground = true
                //};
                //thDo.Start();
                //DataRow[] rrr = dtM.Select(string.Format("选择=1"));
                //foreach (DataRow r in rrr)
                //{
                //    r["选择"] = false;
                //}
                //try
                //{
                //    if (File.Exists(file) == true)
                //    {
                //        System.IO.File.WriteAllText(file, nCopy.ToString());
                //    }
                //    else
                //    {
                //        FileStream myFs = new FileStream(file, FileMode.Create);
                //        StreamWriter mySw = new StreamWriter(myFs);
                //        mySw.Write(nCopy.ToString());
                //        mySw.Close();
                //        myFs.Close();
                //    }
                //}
                //catch (Exception ex)
                //{
                //    CZMaster.MasterLog.WriteLog(ex.Message);
                //}


            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "工单打印");
                MessageBox.Show(ex.Message);
            }
        }
        public void Dowork()
        {

            foreach (DataRow drr in dt_dy.Rows)
            {
                ItemInspection.print_FMS.fun_print_生产工单_A5(drr, ncopy_dy, false, "");
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void textBox8_TextChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //if (textBox8.Text != null && textBox8.Text.Trim() != "")
            //{
            //    string sql = string.Format("select * from [人事基础部门表] where  部门编号='{0}'", textBox8.Text);
            //    DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql, strconn);
            //    textBox17.Text = dr["部门名称"].ToString();
            //}
            //else
            //{
            //    textBox17.Text = "";
            //}
        }
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                this.BindingContext[dv].EndCurrentEdit();
                DataRow dr1111 = gv.GetDataRow(gv.FocusedRowHandle);
                string sql_退 = string.Format("select count(*)x from 工单退料申请表 where 生产工单号 = '{0}' and 完成=0 and 作废=0", dr1111["生产工单号"].ToString());
                DataTable dt_tui = CZMaster.MasterSQL.Get_DataTable(sql_退, strconn);
                if (Convert.ToInt32(dt_tui.Rows[0]["x"]) > 0)
                {
                    throw new Exception("该单据有退料申请未完成，不可操作");
                }
                DateTime t = CPublic.Var.getDatetime();
                //6-11 如果状态字段为 1 说明已经申请 关闭  需要判断
                string sql_记录 = "select * from 生产工单完工记录表 where 1<>1";
                DataTable dt_记录 = CZMaster.MasterSQL.Get_DataTable(sql_记录, strconn);

                string s = "select  * from 其他出入库申请主表 where 1<>1";
                DataTable dt_apply_main = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select  * from 其他出入库申请子表 where 1<>1";
                DataTable dt_apply_detail = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select * from  其他出库主表 where 1<>1";
                DataTable dt_out_main = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select * from  其他出库子表 where 1<>1";
                DataTable dt_out_detail = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                s = "select * from  仓库出入库明细表 where 1<>1";
                DataTable dt_出入库明细 = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                int index = gv.FocusedRowHandle;
                DataView dv_1 = new DataView(dv.ToTable())
                {
                    RowFilter = "选择=1"
                };
                if (MessageBox.Show($"确定完成工单？您勾选了{dv_1.ToTable().Rows.Count}条工单"  , "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                
                    if (dv_1.ToTable().Rows.Count > 0)
                    {
                        for (int k = 0; k <= gv.DataRowCount; k++)
                        {
                            if (gv.GetDataRow(k)["选择"].Equals(true))
                            {
                                index = k;
                                break;
                            }
                        }
                        textBox13.Focus();
                        dv.RowFilter = null;
                        dv.RowFilter = "选择=1";
                        DataTable dt_界面 = dv.ToTable();
                        dt_界面.AcceptChanges();
                        string s_applyNo = "";
                        string s_out_No = "";
                        int i = 1; //后加 用于其他出库的计数
                        bool bl = false;//这里 只想 生成一条其他出入库申请主表记录  用来标识是否已生成
                        //董佳立增加 判断入库倒冲的料是否足够 -- 修改库存
                        DataTable dt_判断库存 = new DataTable();
                        foreach (DataRow dr in dt_界面.Rows)
                        {
                            if (Convert.ToBoolean(dr["状态"])) throw new Exception("该工单已申请关闭,请确认");
                            //2017-6-5 有未领物料不允许完工 待定
                            //2017-10-10  取消限制
                            //2018-4-8 确认加上限制
                            //2018-4-11 只针对生产提出的关键子项做限制 只对有关键子项的 放松限制 无关键子项的 全部限制
                            //2019-5 全部限制
                            //2019-9-16修改全部完工的判断条件  领料明细 必须所有都完成 不管什么工单类型
                            #region 历史版本 19-9-16前
                            //string sss = string.Format("select  *  from 基础数据物料BOM表 where   产品编码='{0}'", dr["物料编码"]);
                            //using (SqlDataAdapter da = new SqlDataAdapter(sss, strconn))
                            //{
                            //    DataTable tt = new DataTable();
                            //    da.Fill(tt);
                            //    if (tt.Rows.Count > 0)
                            //    {
                            //        string sql_1 = string.Format(@"select a.*,c.关键子项  from 生产记录生产工单待领料明细表 a,基础数据物料信息表 b,基础数据物料BOM表 c
                            //      ,[生产记录生产工单待领料主表] d  where a.物料编码 =b.物料编码 and a.待领料单号=d.待领料单号 and a.生产工单号='{0}' 
                            //       and a.完成=0 and a.物料编码=c.子项编码 and d.产品编码=c.产品编码 ", dr["生产工单号"]);
                            //        DataTable dt = new DataTable();
                            //        dt = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
                            //        if (dt.Rows.Count > 0)
                            //        {
                            //            string str = "工单：" + dr["生产工单号"].ToString() + "子项：";
                            //            foreach (DataRow rr in dt.Rows)
                            //            {
                            //                str = str + rr["物料编码"].ToString() + " " + rr["物料名称"].ToString();
                            //            }

                            //            throw new Exception(str + " 尚有物料未领请核实,料未领完不允许完工");
                            //        }
                            //    }
                            //    else
                            //    {
                            //        string sql_1 = string.Format(@"select a.*   from 生产记录生产工单待领料明细表 a,基础数据物料信息表 b
                            //  ,[生产记录生产工单待领料主表] d  where a.物料编码 =b.物料编码 and a.待领料单号=d.待领料单号 and a.生产工单号='{0}'  and a.完成=0   ", dr["生产工单号"]);
                            //        DataTable dt = new DataTable();
                            //        dt = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
                            //        if (dt.Rows.Count > 0)
                            //        {
                            //            string str = "工单：" + dr["生产工单号"].ToString();
                            //            foreach (DataRow rr in dt.Rows)
                            //            {
                            //                str = str + rr["物料编码"].ToString() + " " + rr["物料名称"].ToString();
                            //            }
                            //            //MessageBox.Show(str, "尚有物料未领请核实,料未领完不允许完工");
                            //            throw new Exception(str + " 尚有物料未领请核实,料未领完不允许完工");
                            //        }
                            //    }
                            //}

                            #endregion


                            string sss = string.Format(@"select  COUNT(*) x from 生产记录生产工单待领料明细表 a 
                            left join 生产记录生产工单待领料主表 b on a.待领料单号 = b.待领料单号
                            where a.完成 = 0 and 关闭 = 0 and b.完成 = 0 and b.生产工单号 ='{0}'", dr["生产工单号"]);
                            DataTable tt = CZMaster.MasterSQL.Get_DataTable(sss, strconn);
                            if (Convert.ToInt32(tt.Rows[0]["x"]) > 0)// 有未完成的 发料记录
                            {
                                throw new Exception("尚有物料未领请核实,料未领完不允许完工");
                            }


                            if (dr["完工"].Equals(true))
                            {
                                throw new Exception("工单状态已更改 ");
                            }
                            if (dr["部分完工"].Equals(true))
                            {
                                throw new Exception("已有部分完工,剩下部分也请继续部分完工做掉 ");
                            }
                            else
                            {
                                dr["完工"] = true;
                                dr["完工日期"] = t;
                                dr["班组"] = textBox15.Text;
                                dr["班组ID"] = searchLookUpEdit1.EditValue;
                                DataRow dr_记录 = dt_记录.NewRow();
                                dt_记录.Rows.Add(dr_记录);
                                dr_记录["生产送检单号"] = string.Format("SCSJ{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                                CPublic.CNo.fun_得到最大流水号("SCSJ", t.Year, t.Month));
                                dr_记录["生产工单号"] = dr["生产工单号"];
                                dr_记录["生产制令单号"] = dr["生产制令单号"];
                                dr_记录["生产工单类型"] = dr["生产工单类型"];
                                dr_记录["加急状态"] = dr["加急状态"];
                                dr_记录["物料编码"] = dr["物料编码"];
                                dr_记录["物料名称"] = dr["物料名称"];
                                dr_记录["规格型号"] = dr["规格型号"];
                                dr_记录["生产数量"] = Convert.ToDecimal(dr["生产数量"]);
                                dr_记录["生产车间"] = dr["生产车间"];
                                dr_记录["车间名称"] = dr["车间名称"];
                                dr_记录["仓库名称"] = dr["仓库名称"];
                                dr_记录["仓库号"] = dr["仓库号"];
                                dr_记录["完工数量"] = Convert.ToDecimal(dr["生产数量"]);
                                dr_记录["完工日期"] = t;
                                //19-12-24增加操作人，操作人ID计算机名字段
                                dr_记录["完工操作人"] = CPublic.Var.LocalUserID;
                                dr_记录["完工操作人ID"] = CPublic.Var.localUserName;
                                dr_记录["计算机名"] = System.Net.Dns.GetHostName();

                                s = string.Format("select 生产工单类型 from 生产记录生产工单表 where 生产工单号='{0}' and 生产工单类型<>'返修工单'", dr["生产工单号"]);
                                DataTable t_temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                                if (t_temp.Rows.Count > 0)
                                {

                                    //查询该明细子项有没有  wiptype 为入库倒冲的,如果有 则再其他出入库申请和其他出库中增加
                                    s = string.Format(@"with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,优先级,bom_level ) as
                                                (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,优先级,1 as level from 基础数据物料BOM表   
                                                 where 产品编码='{0}'
                                                 union all 
                                                 select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型, convert(decimal(18, 6),a.数量*b.数量) as 数量,
                                                 a.bom类型,a.优先级,b.bom_level+1  from 基础数据物料BOM表 a
                                                 inner join temp_bom b on a.产品编码=b.子项编码 where b.wiptype='虚拟') 
                                                 select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,base.规格型号 as 子项规格,wiptype,子项类型,
                                                 sum(数量)数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号,base.计量单位,base.计量单位编码 from  temp_bom a
                                                 left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
                                                 left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码 where wiptype ='入库倒冲' and 优先级=1
                                                 group by 产品编码,fx.物料名称 ,子项编码,base.物料名称 ,wiptype,子项类型 ,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号
                                                 ,base.计量单位,base.计量单位编码", dr["物料编码"]);
                                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                                    if (temp.Rows.Count > 0)
                                    {
                                        if (!bl)
                                        {
                                            s_applyNo = string.Format("DWLS{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                                             t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("DWLS", t.Year, t.Month).ToString("0000"));
                                            s_out_No = string.Format("LS{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                                            t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("LS", t.Year, t.Month).ToString("0000"));

                                            DataRow dr_apply_main = dt_apply_main.NewRow();
                                            dt_apply_main.Rows.Add(dr_apply_main);
                                            dr_apply_main["GUID"] = System.Guid.NewGuid();
                                            dr_apply_main["出入库申请单号"] = s_applyNo;
                                            dr_apply_main["申请日期"] = t;
                                            dr_apply_main["申请类型"] = "材料出库";
                                            dr_apply_main["备注"] = dr["生产工单号"];
                                            dr_apply_main["操作人员编号"] = CPublic.Var.LocalUserID;
                                            dr_apply_main["操作人员"] = CPublic.Var.localUserName;
                                            dr_apply_main["生效"] = true;
                                            dr_apply_main["生效日期"] = t;
                                            dr_apply_main["生效人员编号"] = CPublic.Var.LocalUserID;
                                            dr_apply_main["完成"] = true;
                                            dr_apply_main["完成日期"] = t;
                                            dr_apply_main["原因分类"] = "入库倒冲";
                                            dr_apply_main["单据类型"] = "材料出库";


                                            DataRow dr_out_main = dt_out_main.NewRow();
                                            dt_out_main.Rows.Add(dr_out_main);
                                            dr_out_main["GUID"] = System.Guid.NewGuid();
                                            dr_out_main["其他出库单号"] = s_out_No;
                                            dr_out_main["出库类型"] = "材料出库";
                                            dr_out_main["操作人员编号"] = CPublic.Var.LocalUserID;
                                            dr_out_main["操作人员"] = CPublic.Var.localUserName;
                                            dr_out_main["出库日期"] = t;
                                            dr_out_main["生效"] = true;
                                            dr_out_main["生效日期"] = t;
                                            dr_out_main["创建日期"] = t;
                                            dr_out_main["出入库申请单号"] = s_applyNo;

                                            // dt_审核 = ERPorg.Corg.fun_PA("生效", "其他出入库申请单", s_applyNo, "入库倒冲"); 
                                            bl = true;

                                        }
                                        //根据列表生成其他出入库申请子表记录 和 其他出库子表记录     审核申请表 记录
                                        foreach (DataRow rr in temp.Rows)
                                        {
                                            DataRow dr_apply_detail = dt_apply_detail.NewRow();
                                            dt_apply_detail.Rows.Add(dr_apply_detail);
                                            dr_apply_detail["GUID"] = System.Guid.NewGuid();
                                            dr_apply_detail["出入库申请单号"] = s_applyNo;
                                            dr_apply_detail["POS"] = i;
                                            dr_apply_detail["出入库申请明细号"] = s_applyNo + "-" + i.ToString("00");
                                            dr_apply_detail["物料编码"] = rr["子项编码"];

                                            dr_apply_detail["规格型号"] = rr["子项规格"];

                                            dr_apply_detail["物料名称"] = rr["子项名称"];
                                            dr_apply_detail["数量"] = Convert.ToDecimal(rr["数量"]) * Convert.ToDecimal(dr_记录["完工数量"]);//倒冲数量=bom数量*成品入库数量

                                            //  dr_apply_detail["备注"] = dr["物料编码"].ToString();//其他出入库申请主表备注记录成品入库单号 这边子表备注就记录成品编码
                                            //19-6-23  计算 财务得 成本核算得时候 改为 工单号
                                            dr_apply_detail["备注"] = dr["生产工单号"].ToString();//其他出入库申请主表备注记录成品入库单号 这边子表备注就记录成品编码

                                            dr_apply_detail["生效"] = true;
                                            dr_apply_detail["生效日期"] = t;
                                            dr_apply_detail["生效人员编号"] = CPublic.Var.LocalUserID;
                                            dr_apply_detail["完成"] = true;
                                            dr_apply_detail["完成日期"] = t;
                                            dr_apply_detail["仓库号"] = rr["仓库号"];
                                            dr_apply_detail["仓库名称"] = rr["仓库名称"];
                                            try
                                            {
                                                dt_判断库存 = ERPorg.Corg.fun_库存(-1, dt_apply_detail);
                                            }
                                            catch
                                            {
                                                throw new Exception( "入库倒冲的料不足");
                                            }
                                            DataRow dr_out_detail = dt_out_detail.NewRow();
                                            dt_out_detail.Rows.Add(dr_out_detail);
                                            dr_out_detail["物料编码"] = rr["子项编码"];
                                            //dr_其他出库子["原ERP物料编号"] = dr_借用明细["原ERP物料编号"];
                                            dr_out_detail["物料名称"] = rr["子项名称"];
                                            dr_out_detail["数量"] = Convert.ToDecimal(dr_apply_detail["数量"]);

                                            dr_out_detail["规格型号"] = rr["子项规格"];
                                            // dr_其他出库子["图纸编号"] = rr["图纸编号"];
                                            dr_out_detail["其他出库单号"] = s_out_No;
                                            dr_out_detail["POS"] = i;
                                            dr_out_detail["其他出库明细号"] = s_out_No + "-" + i.ToString("00");
                                            dr_out_detail["GUID"] = System.Guid.NewGuid();
                                            dr_out_detail["备注"] = dr["生产工单号"].ToString();
                                            dr_out_detail["生效"] = true;
                                            dr_out_detail["生效日期"] = t;
                                            dr_out_detail["生效人员编号"] = CPublic.Var.LocalUserID;
                                            dr_out_detail["完成"] = true;
                                            dr_out_detail["完成日期"] = t;
                                            dr_out_detail["完成人员编号"] = CPublic.Var.LocalUserID;
                                            dr_out_detail["出入库申请单号"] = s_applyNo;
                                            dr_out_detail["出入库申请明细号"] = dr_apply_detail["出入库申请明细号"];

                                            DataRow dr_出入库 = dt_出入库明细.NewRow();
                                            dt_出入库明细.Rows.Add(dr_出入库);
                                            dr_出入库["GUID"] = System.Guid.NewGuid();
                                            dr_出入库["明细类型"] = "入库倒冲";
                                            dr_出入库["单号"] = s_out_No;
                                            dr_出入库["出库入库"] = "出库";
                                            dr_出入库["物料编码"] = rr["子项编码"];
                                            dr_出入库["物料名称"] = rr["子项名称"];
                                            dr_出入库["仓库号"] = rr["仓库号"];
                                            dr_出入库["仓库名称"] = rr["仓库名称"];
                                            dr_出入库["明细号"] = dr_out_detail["其他出库明细号"];
                                            dr_出入库["相关单号"] = dr["生产工单号"];

                                            //string ss = string.Format("select 车间名称 from 生产记录生产工单表 where 生产工单号='{0}'", dr["生产工单号"].ToString());
                                            //DataTable t_s = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
                                            //dr_出入库["相关单位"] = t_s.Rows[0]["车间名称"];
                                            dr_出入库["实效数量"] = -(Convert.ToDecimal(dr_out_detail["数量"]));
                                            dr_出入库["实效时间"] = t;
                                            dr_出入库["出入库时间"] = t;
                                            i++;
                                        }
                                    }
                                }
                            }
                        }

                        SqlConnection conn = new SqlConnection(strconn);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("生效");
                        string sql1 = "select * from 生产记录生产工单表 where 1<>1";
                        SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
                        SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        string sql2 = "select * from 生产工单完工记录表 where 1<>1";
                        SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);
                        SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                        new SqlCommandBuilder(da2);
                        string sql3 = "select * from 其他出入库申请主表 where 1<>1";
                        SqlCommand cmd3 = new SqlCommand(sql3, conn, ts);
                        SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
                        new SqlCommandBuilder(da3);
                        string sql4 = "select * from 其他出入库申请子表 where 1<>1";
                        SqlCommand cmd4 = new SqlCommand(sql4, conn, ts);
                        SqlDataAdapter da4 = new SqlDataAdapter(cmd4);
                        new SqlCommandBuilder(da4);
                        string sql5 = "select * from 其他出库主表 where 1<>1";
                        SqlCommand cmd5 = new SqlCommand(sql5, conn, ts);
                        SqlDataAdapter da5 = new SqlDataAdapter(cmd5);
                        new SqlCommandBuilder(da5);
                        string sql6 = "select * from 其他出库子表 where 1<>1";
                        SqlCommand cmd6 = new SqlCommand(sql6, conn, ts);
                        SqlDataAdapter da6 = new SqlDataAdapter(cmd6);
                        new SqlCommandBuilder(da6);
                        string sql7 = "select * from 仓库出入库明细表 where 1<>1";
                        SqlCommand cmd7 = new SqlCommand(sql7, conn, ts);
                        SqlDataAdapter da7 = new SqlDataAdapter(cmd7);
                        new SqlCommandBuilder(da7);
                        string sql8 = "select * from 仓库物料数量表 where 1<>1";
                        SqlCommand cmd8 = new SqlCommand(sql8, conn, ts);
                        SqlDataAdapter da8 = new SqlDataAdapter(cmd8);
                        new SqlCommandBuilder(da8);
                        try
                        {
                            da1.Update(dt_界面);
                            da2.Update(dt_记录);
                            if (dt_apply_main.Rows.Count > 0)
                            {
                                da3.Update(dt_apply_main);
                                da4.Update(dt_apply_detail);
                                da5.Update(dt_out_main);
                                da6.Update(dt_out_detail);
                                da7.Update(dt_出入库明细);
                                da8.Update(dt_判断库存);
                            }

                            ts.Commit();

                        }
                        catch (Exception ex)
                        {
                            ts.Rollback();
                            throw ex;
                        }

                        //string sql_ls = string.Format("select * from  生产记录生产工单表 where 1<>1");
                        //using (SqlDataAdapter da = new SqlDataAdapter(sql_ls, strconn))
                        //{
                        //    new SqlCommandBuilder(da);
                        //    da.Update(dt_界面);
                        //}
                        CZMaster.MasterSQL.Save_DataTable(dtM, "生产记录生产工单表", strconn);
                        MessageBox.Show("保存成功");
                        //制造六课  完工需要 关联 模具  自动生成 工单性 保养记录
                        dtM.AcceptChanges();
                        barLargeButtonItem1_ItemClick(null, null);
                        if (index != 0 && index <= gv.DataRowCount)
                        {
                            gv.FocusedRowHandle = index;
                        }
                        else if (index > gv.DataRowCount)
                        {
                            gv.FocusedRowHandle = gv.DataRowCount;
                        }
                        gv.MoveBy(index);
                    }
                    else
                    {
                        throw new Exception("未选择完工工单");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                CZMaster.MasterLog.WriteLog(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            单独打印小标签界面 fm = new 单独打印小标签界面
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            fm.ShowDialog();
        }

#pragma warning disable IDE1006 // 命名样式gridView1_CustomDrawRowIndicator
        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }

        }



        //部分完工 
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                //string sql_退 = string.Format("select count(*)x from 工单退料申请表 where 生产工单号 = '{0}' and 完成=0 and 作废=0", dr["生产工单号"].ToString());
                //DataTable dt_tui = CZMaster.MasterSQL.Get_DataTable(sql_退, strconn);
                //if (Convert.ToInt32(dt_tui.Rows[0]["x"]) > 0)
                //{
                //    throw new Exception("该单据有退料申请未完成，不可操作");
                //}
                DateTime t = CPublic.Var.getDatetime();
                string s = "select  * from 其他出入库申请主表 where 1<>1";
                DataTable dt_apply_main = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select  * from 其他出入库申请子表 where 1<>1";
                DataTable dt_apply_detail = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select * from  其他出库主表 where 1<>1";
                DataTable dt_out_main = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select * from  其他出库子表 where 1<>1";
                DataTable dt_out_detail = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                string s_applyNo = "";
                string s_out_No = "";
                int i = 1; //后加 用于其他出库的计数
                bool bl = false;//这里 只想 生成一条其他出入库申请主表记录  用来标识是否已生成
                DataTable dt_判断库存 = new DataTable();


                s = "select * from  仓库出入库明细表 where 1<>1";
                DataTable dt_出入库明细 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                if (MessageBox.Show(string.Format("确定部分完工该工单'{0}'？", dr["生产工单号"]), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    工单部分完工填写数量界面 frm = new 工单部分完工填写数量界面();
                    frm.ShowDialog();

                    if (frm.in_部分完工数 != 0)
                    {
                        string sql = string.Format("select * from 生产记录生产工单表 where 生产工单号='{0}'", dr["生产工单号"]);
                        DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        string sql_记录 = "select * from 生产工单完工记录表 where 1<>1";
                        DataTable dt_记录 = CZMaster.MasterSQL.Get_DataTable(sql_记录, strconn);
                        DataRow rrr = dt.Rows[0];
                        if (Convert.ToDecimal(dt.Rows[0]["部分完工数"]) + frm.in_部分完工数 > Convert.ToInt32(dt.Rows[0]["生产数量"]))
                        {
                            throw new Exception("总数量大于工单数");
                        }
                        decimal dec_累计完工数 = Convert.ToDecimal(dt.Rows[0]["部分完工数"]) + frm.in_部分完工数;

                        dt.Rows[0]["班组ID"] = searchLookUpEdit1.EditValue;
                        dt.Rows[0]["班组"] = textBox15.Text;

                        sql = string.Format("select * from 生产记录生产工单待领料明细表 where 完成=0 and  生产工单号 ='{0}' ", dr["生产工单号"]);
                        DataTable temp = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                        if (rrr["生产工单类型"].ToString() == "返修工单")
                        {
                            if (temp.Rows.Count > 0)
                            {
                                throw new Exception("返修工单完工或者部分完工都需要将料领完");
                            }
                        }
                        else if (temp.Rows.Count > 0)
                        {
                            //需要判断领的料足不足 dec_累计完工数*bom数量  领料单上数量为0的 先不管
                            sql = string.Format(@" select min(已领数量/BOM数量) as 领料套数  from 生产记录生产工单待领料明细表 mx 
                             left join 生产记录生产工单待领料主表 zb on mx.待领料单号 =zb.待领料单号 
                             where zb.生产工单号 ='{0}' and 领料类型='工单领料' and BOM数量<>0 and mx.完成=0 ", dr["生产工单号"]);

                            DataTable cc = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                            if (cc.Rows[0]["领料套数"] != DBNull.Value && cc.Rows[0]["领料套数"] != null && cc.Rows[0]["领料套数"].ToString() != null)
                            {
                                if (Convert.ToDecimal(cc.Rows[0]["领料套数"]) < dec_累计完工数)
                                {

                                    throw new Exception("发料数量不足以完工这么多数量");
                                }

                            }
                        }

                        foreach (DataRow drr in dt.Rows)
                        {
                            DataRow dr_记录 = dt_记录.NewRow();
                            dt_记录.Rows.Add(dr_记录);
                            dr_记录["生产送检单号"] = string.Format("SCSJ{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                                CPublic.CNo.fun_得到最大流水号("SCSJ", t.Year, t.Month));
                            dr_记录["生产工单号"] = drr["生产工单号"];
                            dr_记录["生产制令单号"] = drr["生产制令单号"];
                            dr_记录["生产工单类型"] = drr["生产工单类型"];
                            dr_记录["加急状态"] = drr["加急状态"];
                            dr_记录["物料编码"] = drr["物料编码"];
                            dr_记录["物料名称"] = drr["物料名称"];
                            dr_记录["规格型号"] = drr["规格型号"];
                            dr_记录["生产数量"] = Convert.ToDecimal(drr["生产数量"]);
                            dr_记录["生产车间"] = drr["生产车间"];
                            dr_记录["车间名称"] = drr["车间名称"];
                            dr_记录["仓库名称"] = drr["仓库名称"];
                            dr_记录["仓库号"] = drr["仓库号"];
                            dr_记录["完工数量"] = Convert.ToDecimal(frm.in_部分完工数);
                            dr_记录["完工日期"] = t;
                            //19-12-24增加操作人，操作人ID计算机名字段
                            dr_记录["完工操作人"] = CPublic.Var.LocalUserID;
                            dr_记录["完工操作人ID"] = CPublic.Var.localUserName;
                            dr_记录["计算机名"] = System.Net.Dns.GetHostName();


                            s = string.Format("select 生产工单类型 from 生产记录生产工单表 where 生产工单号='{0}' and 生产工单类型<>'返修工单'", dr["生产工单号"]);
                            DataTable t_temp = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                            if (t_temp.Rows.Count > 0)
                            {

                                //查询该明细子项有没有  wiptype 为入库倒冲的,如果有 则再其他出入库申请和其他出库中增加
                                s = string.Format(@"with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,优先级,bom_level ) as
                                                (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,优先级,1 as level from 基础数据物料BOM表   
                                                 where 产品编码='{0}'
                                                 union all 
                                                 select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型, convert(decimal(18, 6),a.数量*b.数量) as 数量,
                                                 a.bom类型,a.优先级,b.bom_level+1  from 基础数据物料BOM表 a
                                                 inner join temp_bom b on a.产品编码=b.子项编码 where b.wiptype='虚拟') 
                                                 select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,base.规格型号 as 子项规格,wiptype,子项类型,
                                                 sum(数量)数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号,base.计量单位,base.计量单位编码 from  temp_bom a
                                                 left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
                                                 left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码 where wiptype ='入库倒冲' and 优先级=1
                                                 group by 产品编码,fx.物料名称 ,子项编码,base.物料名称 ,wiptype,子项类型 ,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号
                                                 ,base.计量单位,base.计量单位编码", dr["物料编码"]);
                                DataTable temp1 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                                if (temp1.Rows.Count > 0)
                                {
                                    if (!bl)
                                    {
                                        s_applyNo = string.Format("DWLS{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                                         t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("DWLS", t.Year, t.Month).ToString("0000"));
                                        s_out_No = string.Format("LS{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                                        t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("LS", t.Year, t.Month).ToString("0000"));

                                        DataRow dr_apply_main = dt_apply_main.NewRow();
                                        dt_apply_main.Rows.Add(dr_apply_main);
                                        dr_apply_main["GUID"] = System.Guid.NewGuid();
                                        dr_apply_main["出入库申请单号"] = s_applyNo;
                                        dr_apply_main["申请日期"] = t;
                                        dr_apply_main["申请类型"] = "材料出库";
                                        dr_apply_main["备注"] = dr["生产工单号"];
                                        dr_apply_main["操作人员编号"] = CPublic.Var.LocalUserID;
                                        dr_apply_main["操作人员"] = CPublic.Var.localUserName;
                                        dr_apply_main["生效"] = true;
                                        dr_apply_main["生效日期"] = t;
                                        dr_apply_main["生效人员编号"] = CPublic.Var.LocalUserID;
                                        dr_apply_main["完成"] = true;
                                        dr_apply_main["完成日期"] = t;
                                        dr_apply_main["原因分类"] = "入库倒冲";
                                        dr_apply_main["单据类型"] = "材料出库";


                                        DataRow dr_out_main = dt_out_main.NewRow();
                                        dt_out_main.Rows.Add(dr_out_main);
                                        dr_out_main["GUID"] = System.Guid.NewGuid();
                                        dr_out_main["其他出库单号"] = s_out_No;
                                        dr_out_main["出库类型"] = "材料出库";
                                        dr_out_main["操作人员编号"] = CPublic.Var.LocalUserID;
                                        dr_out_main["操作人员"] = CPublic.Var.localUserName;
                                        dr_out_main["出库日期"] = t;
                                        dr_out_main["生效"] = true;
                                        dr_out_main["生效日期"] = t;
                                        dr_out_main["创建日期"] = t;
                                        dr_out_main["出入库申请单号"] = s_applyNo;

                                        // dt_审核 = ERPorg.Corg.fun_PA("生效", "其他出入库申请单", s_applyNo, "入库倒冲"); 
                                        bl = true;

                                    }
                                    //根据列表生成其他出入库申请子表记录 和 其他出库子表记录     审核申请表 记录
                                    foreach (DataRow rr in temp1.Rows)
                                    {
                                        DataRow dr_apply_detail = dt_apply_detail.NewRow();
                                        dt_apply_detail.Rows.Add(dr_apply_detail);
                                        dr_apply_detail["GUID"] = System.Guid.NewGuid();
                                        dr_apply_detail["出入库申请单号"] = s_applyNo;
                                        dr_apply_detail["POS"] = i;
                                        dr_apply_detail["出入库申请明细号"] = s_applyNo + "-" + i.ToString("00");
                                        dr_apply_detail["物料编码"] = rr["子项编码"];

                                        dr_apply_detail["规格型号"] = rr["子项规格"];

                                        dr_apply_detail["物料名称"] = rr["子项名称"];
                                        dr_apply_detail["数量"] = Convert.ToDecimal(rr["数量"]) * Convert.ToDecimal(dr_记录["完工数量"]);//倒冲数量=bom数量*成品入库数量

                                        //  dr_apply_detail["备注"] = dr["物料编码"].ToString();//其他出入库申请主表备注记录成品入库单号 这边子表备注就记录成品编码
                                        //19-6-23  计算 财务得 成本核算得时候 改为 工单号
                                        dr_apply_detail["备注"] = dr["生产工单号"].ToString();//其他出入库申请主表备注记录成品入库单号 这边子表备注就记录成品编码

                                        dr_apply_detail["生效"] = true;
                                        dr_apply_detail["生效日期"] = t;
                                        dr_apply_detail["生效人员编号"] = CPublic.Var.LocalUserID;
                                        dr_apply_detail["完成"] = true;
                                        dr_apply_detail["完成日期"] = t;
                                        dr_apply_detail["仓库号"] = rr["仓库号"];
                                        dr_apply_detail["仓库名称"] = rr["仓库名称"];
                                        try
                                        {
                                            dt_判断库存 = ERPorg.Corg.fun_库存(-1, dt_apply_detail);
                                        }
                                        catch
                                        {
                                            throw new Exception("入库倒冲的料不足");
                                        }




                                        DataRow dr_out_detail = dt_out_detail.NewRow();
                                        dt_out_detail.Rows.Add(dr_out_detail);
                                        dr_out_detail["物料编码"] = rr["子项编码"];
                                        //dr_其他出库子["原ERP物料编号"] = dr_借用明细["原ERP物料编号"];
                                        dr_out_detail["物料名称"] = rr["子项名称"];
                                        dr_out_detail["数量"] = Convert.ToDecimal(dr_apply_detail["数量"]);

                                        dr_out_detail["规格型号"] = rr["子项规格"];
                                        // dr_其他出库子["图纸编号"] = rr["图纸编号"];
                                        dr_out_detail["其他出库单号"] = s_out_No;
                                        dr_out_detail["POS"] = i;
                                        dr_out_detail["其他出库明细号"] = s_out_No + "-" + i.ToString("00");
                                        dr_out_detail["GUID"] = System.Guid.NewGuid();
                                        dr_out_detail["备注"] = dr["生产工单号"].ToString();
                                        dr_out_detail["生效"] = true;
                                        dr_out_detail["生效日期"] = t;
                                        dr_out_detail["生效人员编号"] = CPublic.Var.LocalUserID;
                                        dr_out_detail["完成"] = true;
                                        dr_out_detail["完成日期"] = t;
                                        dr_out_detail["完成人员编号"] = CPublic.Var.LocalUserID;
                                        dr_out_detail["出入库申请单号"] = s_applyNo;
                                        dr_out_detail["出入库申请明细号"] = dr_apply_detail["出入库申请明细号"];

                                        DataRow dr_出入库 = dt_出入库明细.NewRow();
                                        dt_出入库明细.Rows.Add(dr_出入库);
                                        dr_出入库["GUID"] = System.Guid.NewGuid();
                                        dr_出入库["明细类型"] = "入库倒冲";
                                        dr_出入库["单号"] = s_out_No;
                                        dr_出入库["出库入库"] = "出库";
                                        dr_出入库["物料编码"] = rr["子项编码"];
                                        dr_出入库["物料名称"] = rr["子项名称"];
                                        dr_出入库["仓库号"] = rr["仓库号"];
                                        dr_出入库["仓库名称"] = rr["仓库名称"];
                                        dr_出入库["明细号"] = dr_out_detail["其他出库明细号"];
                                        dr_出入库["相关单号"] = dr["生产工单号"];

                                        //string ss = string.Format("select 车间名称 from 生产记录生产工单表 where 生产工单号='{0}'", dr["生产工单号"].ToString());
                                        //DataTable t_s = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
                                        //dr_出入库["相关单位"] = t_s.Rows[0]["车间名称"];
                                        dr_出入库["实效数量"] = -(Convert.ToDecimal(dr_out_detail["数量"]));
                                        dr_出入库["实效时间"] = t;
                                        dr_出入库["出入库时间"] = t;
                                        i++;
                                    }
                                }
                            }

                        }
                        
                        //全部完工  19-11-18
                        if (dec_累计完工数 == Convert.ToDecimal(dt.Rows[0]["生产数量"]))
                        {
                            #region 19-9-16 历史版本
                            //string sql_1 = string.Format(@"select a.*,c.关键子项  from 生产记录生产工单待领料明细表 a,基础数据物料信息表 b,基础数据物料BOM表 c
                            // ,[生产记录生产工单待领料主表] d  where a.物料编码 =b.物料编码 and a.待领料单号=d.待领料单号 and a.生产工单号='{0}' 
                            //and a.完成=0 and a.物料编码=c.子项编码 and d.产品编码=c.产品编码  and d.关闭=0 ", dr["生产工单号"]);
                            //DataTable tt = new DataTable();
                            //tt = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
                            //if (tt.Rows.Count > 0)
                            //{
                            //    string str = "工单：" + dr["生产工单号"].ToString() + "子项：";
                            //    foreach (DataRow r in tt.Rows)
                            //    {
                            //        str = str + r["物料编码"].ToString() + ":" + r["物料名称"].ToString() + " ";
                            //    }
                            //    //MessageBox.Show(str, "尚有物料未领请核实,料未领完不允许完工");
                            //    throw new Exception(str + " 尚未领用请核实,料未领完不允许完工");
                            //}
                            #endregion 
                            string sql_1 = string.Format(@"select  COUNT(*) x from 生产记录生产工单待领料明细表 a 
                            left join 生产记录生产工单待领料主表 b on a.待领料单号 = b.待领料单号
                            where a.完成 = 0 and 关闭 = 0 and b.完成 = 0 and b.生产工单号 ='{0}'", dr["生产工单号"]); ;
                            DataTable tt = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
                            if (Convert.ToInt32(tt.Rows[0]["x"]) > 0)// 有未完成的 发料记录
                            {
                                throw new Exception("尚有物料未领请核实,料未领完不允许完工");
                            }
                            dt.Rows[0]["完工"] = 1;
                            dt.Rows[0]["完工日期"] = CPublic.Var.getDatetime();
                        }
                        dr.AcceptChanges();
                        dt.Rows[0]["部分完工"] = 1;
                        dt.Rows[0]["上次完工数"] = frm.in_部分完工数;
                        dr["部分完工数"] = dt.Rows[0]["部分完工数"] = dec_累计完工数;

                        SqlConnection conn = new SqlConnection(strconn);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("生效");
                        string sql1 = "select * from 生产记录生产工单表 where 1<>1";
                        SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
                        SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        string sql2 = "select * from 生产工单完工记录表 where 1<>1";
                        SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);
                        SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                        new SqlCommandBuilder(da2);
                        string sql3 = "select * from 其他出入库申请主表 where 1<>1";
                        SqlCommand cmd3 = new SqlCommand(sql3, conn, ts);
                        SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
                        new SqlCommandBuilder(da3);
                        string sql4 = "select * from 其他出入库申请子表 where 1<>1";
                        SqlCommand cmd4 = new SqlCommand(sql4, conn, ts);
                        SqlDataAdapter da4 = new SqlDataAdapter(cmd4);
                        new SqlCommandBuilder(da4);
                        string sql5 = "select * from 其他出库主表 where 1<>1";
                        SqlCommand cmd5 = new SqlCommand(sql5, conn, ts);
                        SqlDataAdapter da5 = new SqlDataAdapter(cmd5);
                        new SqlCommandBuilder(da5);
                        string sql6 = "select * from 其他出库子表 where 1<>1";
                        SqlCommand cmd6 = new SqlCommand(sql6, conn, ts);
                        SqlDataAdapter da6 = new SqlDataAdapter(cmd6);
                        new SqlCommandBuilder(da6);
                        string sql7 = "select * from 仓库出入库明细表 where 1<>1";
                        SqlCommand cmd7 = new SqlCommand(sql7, conn, ts);
                        SqlDataAdapter da7 = new SqlDataAdapter(cmd7);
                        new SqlCommandBuilder(da7);
                        string sql8 = "select * from 仓库物料数量表 where 1<>1";
                        SqlCommand cmd8 = new SqlCommand(sql8, conn, ts);
                        SqlDataAdapter da8 = new SqlDataAdapter(cmd8);
                        new SqlCommandBuilder(da8);

                        try
                        {
                            da1.Update(dt);
                            da2.Update(dt_记录);
                            if (dt_apply_main.Rows.Count > 0)
                            {
                                da3.Update(dt_apply_main);
                                da4.Update(dt_apply_detail);
                                da5.Update(dt_out_main);
                                da6.Update(dt_out_detail);
                                da7.Update(dt_出入库明细);
                                da8.Update(dt_判断库存);
                            }
                            ts.Commit();
                            dtM.Rows.Remove(dr);
                        }
                        catch (Exception ex)
                        {
                            ts.Rollback();
                            throw ex;
                        }

                        //string sql_存 = "select * from  生产记录生产工单表 where 1<>1";
                        //using (SqlDataAdapter da = new SqlDataAdapter(sql_存, strconn))
                        //{
                        //    new SqlCommandBuilder(da);
                        //    da.Update(dt);
                        //    dtM.Rows.Remove(dr);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void repositoryItemCheckEdit2_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            //dr["选择"] = true;

            gridView1_RowCellClick(null, null);

        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_MouseUp(object sender, MouseEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //if (e.Button == MouseButtons.Left)
            //{


            //    int[] dr = gv.GetSelectedRows();
            //    if (dr.Length > 1)
            //    {
            //        for (int i = 0; i < dr.Length; i++)
            //        {
            //            DataRow r = gv.GetDataRow(dr[i]);
            //            if (r["选择"].Equals(true))
            //            {
            //                r["选择"] = 0;

            //            }
            //            else
            //            {
            //                r["选择"] = 1;
            //            }

            //        }

            //        //gridView1.FocusedRowHandle = dr[dr.Length - 1];
            //        gv.MoveBy(dr.Length - 1);
            //    }
            //}

        }
        #region 弃用
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 判断该料要领多少 返回 0则不要领 
        /// </summary>
        /// <returns></returns>
        private decimal fun_领料判断(string str_物料, string str_产品, decimal dec_生产数)
#pragma warning restore IDE1006 // 命名样式
        {

            return 0;
        }
#pragma warning disable IDE1006 // 命名样式
        private static Decimal fun_物料数量_末领量(string str_ItemNo, Boolean Refresh = false)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                Decimal dec末领量 = 0;
                string sql = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", str_ItemNo);
                DataTable dt_末领量 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn);
                da.Fill(dt_末领量);
                if (Refresh == false)
                {
                    dec末领量 = Convert.ToDecimal(dt_末领量.Rows[0]["末领量"].ToString());
                }
                else
                {
                    List<DataRow> li_末领量 = fun_物料数量_末领量_R(str_ItemNo);
                    if (li_末领量.Count > 0)
                    {
                        foreach (DataRow r in li_末领量)
                        {
                            dec末领量 = dec末领量 + Convert.ToDecimal(r["待领料总量"].ToString()) - Convert.ToDecimal(r["已领数量"].ToString());
                        }
                    }
                }
                return dec末领量;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_物料数量_末领量");
                return 0;
            }
        }


#pragma warning disable IDE1006 // 命名样式
        private static List<DataRow> fun_物料数量_末领量_R(string str_ItemNo)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                List<DataRow> li_末领量 = new List<DataRow>();
                string sql = string.Format("select * from 生产记录生产工单待领料明细表 where 物料编码 = '{0}' and 完成 = 0", str_ItemNo);// and 生效 = 1
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        li_末领量.Add(r);
                    }
                }
                return li_末领量;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "StockCorer.fun_物料数量_末领量_R");
                return null;
            }
        }

        private void 查看工单状态ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            ERPproduct.frm查看制令相关工单的状态 fm = new ERPproduct.frm查看制令相关工单的状态(dr["生产制令单号"].ToString());
            CPublic.UIcontrol.AddNewPage(fm, "工单状态查询");
        }
        #endregion



        private void 查看BOMToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DataRow r = gv.GetDataRow(gv.FocusedRowHandle);


            UI物料BOM详细数量 UI = new UI物料BOM详细数量(r["物料编码"].ToString().Trim(), 1);
            CPublic.UIcontrol.AddNewPage(UI, "物料BOM信息");
        }

        private void 工单补料ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            ERPproduct.UI生产补料 frm = new UI生产补料(dr["生产工单号"].ToString());
            CPublic.UIcontrol.Showpage(frm, "生产补料");
        }

        private void 返修打印ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            ERPproduct.ui返修打印 ui = new ui返修打印(dr);
            CPublic.UIcontrol.Showpage(ui, "返修打印");
        }

#pragma warning disable IDE1006 // 命名样式
        private void searchLookUpEdit1View_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }


        //制三课 打印小标签
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                //if (dr == DialogResult.OK)
                //{
                string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                DataRow r = gv.GetDataRow(gv.FocusedRowHandle);
                fun_制三标签B2(r, PrinterName, false, 0);

                //}
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 为补打才需要 赋值 i_打印数
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="str_dy"></param>
        /// <param name="bl_补打"></param>
        /// <param name="i_打印数"></param>
        public void fun_制三标签(DataRow dr, string str_dy, bool bl_补打, int i_打印数)
#pragma warning restore IDE1006 // 命名样式
        {
            int i_生产数 = 0;
            int i_起 = 0;
            if (bl_补打)   //补打小标签那边
            {
                i_生产数 = i_打印数;
                string s = string.Format("select  备注4 from 生产记录生产工单表 where 生产工单号='{0}' ", dr["生产工单号"].ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(s, strconn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows[0]["备注4"].ToString() != "")
                    {
                        i_起 = Convert.ToInt32(dt.Rows[0]["备注4"].ToString());
                    }
                }
            }
            else
            {
                i_生产数 = Convert.ToInt32(dr["生产数量"]);
            }
            string rgjm = "";
            string result = "";
            if (dr["工单负责人ID"].ToString() == "")
            {
                throw new Exception("工单负责人为空");
            }
            string sql = string.Format(@"select 工号简码 from 人事基础员工表 
                                    where  员工号='{0}' ", dr["工单负责人ID"].ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {

                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows[0]["工号简码"].ToString() == "")
                {
                    throw new Exception("员工简码为空");
                }
                rgjm = dt.Rows[0]["工号简码"].ToString();
            }

            sql = string.Format("select  * from 基础物料标签维护信息表 where 物料编号='{0}'", dr["原ERP物料编号"].ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows[0]["特征码"].ToString() == "")
                {
                    throw new Exception("特征码为空");
                }
                result = dt.Rows[0]["特征码"].ToString();
            }

            //Regex r = new Regex(string.Format(@"(?<={0}+).*", target));
            //string result = r.Match(dr["图纸编号"].ToString()).Value.Replace(".", "");
            string str_mo = dr["生产工单号"].ToString().Substring(2, 10) + "P";
            List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
            int i_余数 = i_生产数 % 4;

            int i_count = i_生产数 / 4;
            if (i_余数 != 0)
            {
                i_count = i_count + 1;
            }



            for (int j = 1; j <= i_count; j++)
            {

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("th", result); // 图号 
                dic.Add("rgjm", rgjm); // 工号简码
                string str = str_mo + (i_起 + 4 * j - 3).ToString("00000");
                dic.Add("gdh1", str);
                str = str + result + rgjm;
                dic.Add("rwm1", str);

                str = str_mo + (i_起 + 4 * j - 2).ToString("00000");
                dic.Add("gdh2", str);
                str = str + result + rgjm;
                dic.Add("rwm2", str);

                str = str_mo + (i_起 + 4 * j - 1).ToString("00000");
                dic.Add("gdh3", str);
                str = str + result + rgjm;
                dic.Add("rwm3", str);

                str = str_mo + (i_起 + 4 * j).ToString("00000");
                dic.Add("gdh4", str);
                str = str + result + rgjm;
                dic.Add("rwm4", str);
                li.Add(dic);


            }
            string path = Application.StartupPath + @"\Mode\制三标签.lab";
            Lprinter lp = new Lprinter(path, li, str_dy, 1);
            lp.Start();
            string ss = string.Format("select * from 生产记录生产工单表 where 生产工单号='{0}'", dr["生产工单号"].ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(ss, strconn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows[0]["备注4"].ToString() == "")
                {
                    dt.Rows[0]["备注4"] = i_count * 4;
                }
                else
                {
                    dt.Rows[0]["备注4"] = Convert.ToInt32(dt.Rows[0]["备注4"]) + i_count * 4;

                }
                new SqlCommandBuilder(da);
                da.Update(dt);
            }

        }


#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 9-25准备启用
        /// 报工打印用这个
        /// 补打用原来的
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="str_dy"></param>
        /// <param name="bl_补打"></param>
        /// <param name="i_打印数"></param>
        public void fun_制三标签B2(DataRow dr, string str_dy, bool bl_补打, int i_打印数)
#pragma warning restore IDE1006 // 命名样式
        {
            int i_生产数 = 0;
            int i_起 = 0;
            if (bl_补打)   //补打小标签那边
            {
                i_生产数 = i_打印数;
                string s = string.Format("select  备注4 from 生产记录生产工单表 where 生产工单号='{0}' ", dr["生产工单号"].ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(s, strconn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows[0]["备注4"].ToString().Trim() != "")
                    {
                        i_起 = Convert.ToInt32(dt.Rows[0]["备注4"].ToString());
                    }
                }
            }
            else
            {
                i_生产数 = Convert.ToInt32(dr["生产数量"]);
            }
            string rgjm = "";
            string result = "";
            if (dr["工单负责人ID"].ToString() == "")
            {
                throw new Exception("工单负责人为空");
            }
            string sql = string.Format(@"select 工号简码 from 人事基础员工表 
                                    where  员工号='{0}' ", dr["工单负责人ID"].ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {

                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows[0]["工号简码"].ToString() == "")
                {
                    throw new Exception("员工简码为空");
                }
                rgjm = dt.Rows[0]["工号简码"].ToString();
            }

            sql = string.Format("select  * from 基础物料标签维护信息表 where 物料编号='{0}'", dr["原ERP物料编号"].ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows[0]["特征码"].ToString() == "")
                {
                    throw new Exception("特征码为空");
                }
                result = dt.Rows[0]["特征码"].ToString();
            }

            //Regex r = new Regex(string.Format(@"(?<={0}+).*", target));
            //string result = r.Match(dr["图纸编号"].ToString()).Value.Replace(".", "");
            string str_mo = dr["生产工单号"].ToString().Substring(2, 10) + "P";
            List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
            int i_余数 = i_生产数 % 4;

            int i_count = i_生产数 / 4;
            if (i_余数 != 0)
            {
                i_count = i_count + 1;
            }

            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "th", result }, // 图号 
                { "rgjm", rgjm }, // 工号简码 

                { "count", i_起.ToString("00000") },
                // string str = str_mo + i_起.ToString("00000");
                { "gdh1", str_mo }
            };

            // dic.Add("rwm1",str);
            li.Add(dic);

            string path = Application.StartupPath + @"\Mode\制三标签B2.lab";
            Lprinter lp = new Lprinter(path, li, str_dy, i_count * 4);
            lp.Start();
            string ss = string.Format("select * from 生产记录生产工单表 where 生产工单号='{0}'", dr["生产工单号"].ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(ss, strconn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows[0]["备注4"].ToString().Trim() == "")
                {
                    dt.Rows[0]["备注4"] = i_count * 4;
                }
                else
                {
                    dt.Rows[0]["备注4"] = Convert.ToInt32(dt.Rows[0]["备注4"]) + i_count * 4;

                }
                new SqlCommandBuilder(da);
                da.Update(dt);
            }


        }
        //更改替代料
        private void 更改替代料ToolStripMenuItem_Click(object sender, EventArgs e)
        {


            try
            {

                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                this.BindingContext[dv].EndCurrentEdit();

                DataRow drM = (this.BindingContext[gc.DataSource].Current as DataRowView).Row;
                string sql_1 = string.Format(@"select a . * ,b.库存总数,b.有效总数  from 生产记录生产工单待领料明细表 a left join 仓库物料数量表 b on a.物料编码=b.物料编码  and a.仓库号=b.仓库号   
            left join 生产记录生产工单待领料主表  dcc on  dcc.待领料单号=  a.待领料单号
where a. 生产工单号='{0}'  and  领料类型<>'生产补料'order by a.待领料单明细号 ", drM["生产工单号"]);
                //                string sql_1 =  string.Format(@"select a . * ,b.库存总数,b.有效总数  from 生产记录生产工单待领料明细表 a left join 仓库物料数量表 b on a.物料编码=b.物料编码  and a.仓库号=b.仓库号   
                //left join 生产记录生产工单待领料主表  dcc on  dcc.待领料单号=  a.待领料单号
                //where a. 生产工单号='{0}'  and  领料类型<>'生产补料' ", drM["生产工单号"].ToString());
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);

                //if (bool.Parse(drM["生效"].ToString()) == true && dt.Rows.Count > 0)
                //{

                //    throw new Exception("该料已发料,无法进行该操作");
                //    //    barLargeButtonItem5_ItemClick(null, null);
                //    //退料
                //}
                //else if (bool.Parse(drM["生效"].ToString()) == true && dt.Rows.Count <= 0)
                //{
                if (bool.Parse(drM["完工"].ToString())) throw new Exception("该工单已完工,无法进行该操作");
                else if (bool.Parse(drM["生效"].ToString()))
                {
                    string sql = string.Format(@"select a . * ,b.库存总数,b.有效总数  from 生产记录生产工单待领料明细表 a left join 仓库物料数量表 b on a.物料编码=b.物料编码  and a.仓库号=b.仓库号   
left join 生产记录生产工单待领料主表  dcc on  dcc.待领料单号=  a.待领料单号
where a. 生产工单号='{0}'  and  领料类型<>'生产补料' ", drM["生产工单号"]);
                    DataTable dt_waitSupplies = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    // gridControl1.DataSource = dt_waitSupplies;

                    DataTable dtsdsa = dt_waitSupplies.Copy();
                    if (dtsdsa.Rows.Count == 0) throw new Exception("该工单没有发料清单");
                    DataRow drM数据 = dtsdsa.Rows[0];


                    frm待领料替换 fm = new frm待领料替换(drM, dtsdsa, drM数据);
                    fm.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                    fm.ShowDialog();



                }
                else
                {

                    throw new Exception("该料未生效,无法进行该操作");
                }




            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void 铭牌打印ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //DataRow r= gv.GetDataRow(gv.FocusedRowHandle);
            //MoldMangement.UI明牌 ui = new MoldMangement.UI明牌(r);
            //CPublic.UIcontrol.Showpage(ui,"铭牌打印");

        }

        private void 工单关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                this.BindingContext[dv].EndCurrentEdit();
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);

                string sql_dlz = string.Format("select * from  生产记录生产工单待领料主表 where 生产工单号 = '{0}'", dr["生产工单号"]);
                DataTable dt_dlz = CZMaster.MasterSQL.Get_DataTable(sql_dlz, strconn);
                if (dt_dlz.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(dt_dlz.Rows[0]["关闭"]) == true)
                    {
                        throw new Exception("该单据已关闭，无需重复操作");
                    }
                }
                else
                {
                    MessageBox.Show("未查到待领料单");
                }
                

                DateTime time = CPublic.Var.getDatetime();
                if (MessageBox.Show("确定关闭该工单？请核对。", "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {


                    frm异常关闭 fm = new frm异常关闭(dr);
                    fm.ShowDialog();
                    if (fm.关闭 != 2)
                    {
                        if (fm.flag == true && fm.xiala == "不需退料")
                        {
                            DateTime t = CPublic.Var.getDatetime();
                            string str_待退料号 = string.Format("WR{0}{1:00}{2:0000}",
                            t.Year, t.Month, CPublic.CNo.fun_得到最大流水号("WR", t.Year, t.Month));
                            dateEdit1.EditValue = null;
                            dateEdit2.EditValue = null;
                            if (textBox1.Text != "" && dr["生产工单号"].ToString() == textBox1.Text)
                            {
                                if (bool.Parse(dr["状态"].ToString()) == false)
                                {
                                    SqlConnection conn = new SqlConnection(strconn);
                                    conn.Open();
                                    SqlTransaction mt = conn.BeginTransaction("工单退料申请");
                                    try
                                    {
                                        SqlCommand cmd = new SqlCommand(sql, conn, mt);
                                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                                        new SqlCommandBuilder(da);
                                        DataTable dt_申请 = ERPorg.Corg.fun_PA("关闭", "工单关闭", dr["生产工单号"].ToString(), "");
                                        string 单据审核申请表 = "select * from 单据审核申请表 where 1<>1 ";
                                        cmd = new SqlCommand(单据审核申请表, conn, mt);
                                        da = new SqlDataAdapter(cmd);
                                        new SqlCommandBuilder(da);
                                        da.Update(dt_申请);
                                        string sqlaaa = string.Format("select * from 生产记录生产工单表 where 生产工单号='{0}'", dr["生产工单号"].ToString());
                                        DataTable dt_shen = CZMaster.MasterSQL.Get_DataTable(sqlaaa, strconn);
                                        foreach (DataRow di in dt_shen.Rows)
                                        {
                                            di["状态"] = true;
                                            di["备注2"] = fm.xiala.ToString();
                                            di["备注3"] = fm.str_关闭原因.ToString();
                                        }
                                        string sql_max = "select * from 生产记录生产工单表 where 1<>1 ";
                                        cmd = new SqlCommand(sql_max, conn, mt);
                                        da = new SqlDataAdapter(cmd);
                                        new SqlCommandBuilder(da);
                                        da.Update(dt_shen);
                                        mt.Commit();
                                    }
                                    catch (Exception ex)
                                    {
                                        mt.Rollback();
                                        throw new Exception("退料申请失败" + ex.Message);
                                    }
                                    MessageBox.Show("申请完成");
                                }
                                else
                                {
                                    throw new Exception("该工单还有未审核的单据");
                                }
                            }
                        }
                        else if (fm.flag == true && fm.xiala == "关闭")
                        {
                            barLargeButtonItem3_ItemClick(null, null);
                        }
                        else if (fm.flag == true && fm.xiala == "退料")
                        {
                            DataRow drM = (this.BindingContext[gc.DataSource].Current as DataRowView).Row;
                            ////  MessageBox.Show(drM["是否退料"].ToString());
                            if (bool.Parse(drM["状态"].ToString()) == false)
                            {
                                状态 = 1;
                                Form退料申请跳转 BC = new Form退料申请跳转(dr["生产工单号"].ToString(), 状态, drM, fm.xiala, dtp,fm.str_关闭原因);
                                BC.ShowDialog();
                                dtp = BC.dt_返回.Copy();
                            }
                            else
                            {
                                throw new Exception("该工单已申请关闭,锁定中");
                            }
                        }
                    }
                    barLargeButtonItem1_ItemClick(null, null);
                    if (index != 0 && index <= gv.DataRowCount)
                    {
                        gv.FocusedRowHandle = index;

                    }
                    else if (index > gv.DataRowCount)
                    {
                        gv.FocusedRowHandle = gv.DataRowCount;
                    }
                    //gv.MoveBy(0);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem12_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void p_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gv.CloseEditor();
            this.BindingContext[dtM].EndCurrentEdit();

            try
            {
                DataView dv_1 = new DataView(dtM)
                {
                    RowFilter = "选择=1",

                };
                //if (dv_1.Count == 0)
                //{
                //    throw new Exception("未勾选工单，请确认");
                //}
                DataTable dt2 = dv_1.ToTable();


                if (MessageBox.Show("可能造成当前界面当前操作内容丢失，请完成操作后刷新？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {

                    foreach (DataRow dr in dt2.Rows)
                    {

                        string v_number = "";

                        DataTable dt_x = new DataTable();
                        dt_x = ERPorg.Corg.billofM(dt_x, dr["物料编码"].ToString(), true, dt_bom);
                        if (dt_x.Rows.Count > 0)
                        {
                            foreach (DataRow drr in dt_x.Rows)
                            {
                                string sql1 = string.Format(@"   SELECT 文件名, 物料号, 版本 FROM 程序版本维护表 WHERE 版本 = (SELECT MAX(版本) FROM    程序版本维护表 where  物料号 ='{0}' and 停用='0' ) and 物料号 = '{0}'  and 停用='0' ", drr["子项编码"]);
                                DataRow dr_banbe = CZMaster.MasterSQL.Get_DataRow(sql1, strconn);
                                if (dr_banbe != null)
                                {
                                    if (dr_banbe["文件名"].ToString() != "")
                                    {
                                        if (v_number == "")
                                        {
                                            v_number = v_number + dr_banbe["文件名"].ToString();
                                        }
                                        else
                                        {
                                            v_number = v_number + ";" + dr_banbe["文件名"].ToString();
                                        }
                                        //break;
                                    }

                                }
                            }
                        }
                        DataRow[] dr2 = dtM.Select(string.Format("生产工单号='{0}'", dr["生产工单号"].ToString()));

                        dr2[0]["版本备注"] = v_number.ToString();
                        dr["版本备注"] = v_number.ToString();


                    }

                    SqlDataAdapter da;
                    string sql = "select * from 生产记录生产工单表 where 1<>1";
                    da = new SqlDataAdapter(sql, strconn);
                    new SqlCommandBuilder(da);
                    da.Update(dtM);

                    MessageBox.Show("刷新成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }
        bool Print_bl = false;
        private void 打印小标签ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string Printer_标签 = "";
                try
                {
                    Printer_标签 = CPublic.Var.li_CFG["printer_label"].ToString();
                }
                catch (Exception)
                {

                    throw new Exception("标签打印机未配置,printer_label未找到");
                }

                if (Print_bl) throw new Exception("正在打印标签请稍候");
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                string strc_BQ = CPublic.Var.geConn("BQ");
                string s = string.Format("select  * from [ShareLockInfo] where taskNo='{0}' ", dr["生产工单号"]);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strc_BQ);
                DataView dv = new DataView(dt);
                dv.RowFilter = "LockID=0";
                if (dv.Count > 0) throw new Exception("该工单尚有锁号未回写,请稍候再试");

                string l = string.Format(@"select  a.*,FCCID from  Print_ShareLockInfo  a
                left join 生产记录生产工单表 b  on a.MakeOrder = b.生产工单号
                left join [基础物料标签维护信息表] c on c.物料编号 = b.物料编码 where MakeOrder='{0}' order by ctNo ", dr["生产工单号"]);
                DataTable dt_2 = CZMaster.MasterSQL.Get_DataTable(l, strconn);

                if (dt_2.Rows.Count == 0) throw new Exception("选中记录没有SN号可以打印");

                foreach (DataRow r in dt_2.Rows)
                {
                    DataRow[] pr = dt.Select(string.Format("CTNo='{0}'", r["CTNo"]));
                    if (pr.Length > 0)
                    {
                        r["MacAddr"] = pr[0]["MacAddr"];
                        r["LockID"] = pr[0]["LockID"];
                        r["CheckFlag"] = pr[0]["CheckFlag"];
                    }
                }
                CZMaster.MasterSQL.Save_DataTable(dt_2, "Print_ShareLockInfo", strconn);
                Thread BG = new Thread(() =>
                {
                    //List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
                    //foreach (DataRow PrintRow in dt_2.Rows)
                    //{
                    //    Dictionary<string, string> dic = new Dictionary<string, string>();
                    //    dic.Add("SN", PrintRow["CTNo"].ToString().Trim());
                    //    dic.Add("LockID", PrintRow["LockID"].ToString().Trim());
                    //    dic.Add("FCCID", PrintRow["FCCID"].ToString().Trim());
                    //    li.Add(dic);
                    //}
                    string sn_f11 = dt_2.Rows[0]["CTNo"].ToString().Substring(0, 11); //sn20位 取前 11位 
                    string ruleid = dt_2.Rows[0]["DevType"].ToString();
                    string xx = string.Format(@"select  * from [LockIDRuleInfo] where RuleID='{0}' ", ruleid);
                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(xx, strc_BQ);
                    string str_len = temp.Rows[0]["lshlen"].ToString();
                    int len = 0;
                    int.TryParse(str_len, out len);
                    //if (len== 0)
                    //{
                    //    List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
                    //    foreach (DataRow PrintRow in dt_2.Rows)
                    //    {
                    //        Dictionary<string, string> dic = new Dictionary<string, string>();
                    //        dic.Add("SN", PrintRow["CTNo"].ToString().Trim());
                    //        //dic.Add("LockID", PrintRow["LockID"].ToString().Trim());
                    //        dic.Add("FCCID", PrintRow["FCCID"].ToString().Trim());
                    //        li.Add(dic);
                    //    }
                    //    string path = Application.StartupPath + string.Format(@"\Mode\SN标签_无锁号.lab");
                    //    Lprinter lp = new Lprinter(path, li, Printer_标签, 1);
                    //    lp.DoWork();
                    //    Print_bl = false;
                    //}
                    //else
                    //{
                    int lock_len = dt_2.Rows[0]["LockID"].ToString().Length;
                    int x = lock_len - len;
                    string lockid_f = "";
                    int idls = 0;
                    if (len > 0)
                    {
                        lockid_f = dt_2.Rows[0]["LockID"].ToString().Substring(0, x);
                        idls = Convert.ToInt32(dt_2.Rows[0]["LockID"].ToString().Substring(x, len));
                    }
                    int snls = Convert.ToInt32(dt_2.Rows[0]["CTNo"].ToString().Substring(11, 6));
                    ERPorg.Corg cg = new ERPorg.Corg();
                    int qsyzm = Convert.ToInt32(cg.total_JY(dt_2.Rows[0]["CTNo"].ToString().Substring(0, 11)));


                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("SN", sn_f11);// sn前11位

                    dic.Add("LockID", lockid_f); //lockID 先去中间表取当前 锁号规则流水号是几位 x    len(LockID)-x
                    dic.Add("FCCID", dt_2.Rows[0]["FCCID"].ToString());
                    dic.Add("idls", idls.ToString()); //锁号的流水号
                    dic.Add("snls", snls.ToString()); //sn的流水号
                    dic.Add("qsyzm", qsyzm.ToString()); //起始验证码
                    string path = "";
                    if (len == 0)
                    {
                        path = Application.StartupPath + string.Format(@"\Mode\SN标签_无锁号.lab");
                    }
                    else
                    {
                        path = Application.StartupPath + string.Format(@"\Mode\SN标签_锁号{0}位.lab", len.ToString());
                    }
                    Lprinter lp = new Lprinter(path, dic, Printer_标签, dt_2.Rows.Count);
                    lp.DoWork();
                    Print_bl = false;
                    //}
                });
                BG.IsBackground = true;
                BG.Start();
                Print_bl = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 查看完工记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                this.BindingContext[dv].EndCurrentEdit();
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                string sql = string.Format("select * from  生产记录生产工单表 where 生产工单号 ='{0}'", dr["生产工单号"]);
                DataTable dt_部分完工 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (Convert.ToBoolean(dt_部分完工.Rows[0]["部分完工"]) == false && Convert.ToBoolean(dt_部分完工.Rows[0]["完工"]) == false)
                {
                    throw new Exception("该工单没有完工记录");
                }
                部分完工记录 fm = new 部分完工记录(dr);
                fm.WindowState = FormWindowState.Maximized;
                fm.ShowDialog();



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 查看标签记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            ui_标签记录列表 ui = new ui_标签记录列表(dr);
            CPublic.UIcontrol.Showpage(ui, "标签记录列表");


        }

        private void 查看入库倒冲物料ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow r = gv.GetDataRow(gv.FocusedRowHandle);
                string sql = string.Format(@"with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,优先级,bom_level ) as
                   (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,优先级,1 as level from 基础数据物料BOM表   
                    where 产品编码='{0}'
                    union all 
                     select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型, convert(decimal(18, 6),a.数量*b.数量) as 数量,
                     a.bom类型,a.优先级,b.bom_level+1  from 基础数据物料BOM表 a
                     inner join temp_bom b on a.产品编码=b.子项编码 where b.wiptype='虚拟') 
                     select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,base.规格型号 as 子项规格,wiptype,子项类型,
                     sum(数量)数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号,base.计量单位,base.计量单位编码 from  temp_bom a
                     left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
                     left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码 where wiptype ='入库倒冲' and 优先级=1
                     group by 产品编码,fx.物料名称 ,子项编码,base.物料名称 ,wiptype,子项类型 ,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号
                     ,base.计量单位,base.计量单位编码", r["物料编码"]);
                DataTable dt_rkdc = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (dt_rkdc.Rows.Count == 0)
                {
                    throw new Exception("该物料没有需要入库倒冲的料");
                }
                ui_查看入库倒冲物料 UI = new ui_查看入库倒冲物料(r, dt_rkdc);
                CPublic.UIcontrol.AddNewPage(UI, "查看入库倒冲物料");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
