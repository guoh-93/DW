using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CZMaster;
using System.Data.SqlClient;

namespace ERPpurchase
{
    public partial class ui拒收操作 : UserControl
    {
        public ui拒收操作()
        {
            InitializeComponent();
        }
        string strcon = CPublic.Var.strConn;
        DataTable dt_送检单主表;

        DataTable t_片区 = ERPorg.Corg.fun_业务员片区(CPublic.Var.localUserName);
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "";


                DataTable view_权限 = ERPorg.Corg.fun_hr("采购", CPublic.Var.LocalUserID);

                if (view_权限.Rows.Count > 0)
                {
                    //sql += " and csjz.生效人员ID in (";
                    //foreach (DataRow r in view_权限.Rows)
                    //{
                    //    sql += "'" + r["工号"].ToString().Trim() + "',";
                    //}
                    //sql = sql.Substring(0, sql.Length - 1) + ")";
                }

                else if (CPublic.Var.LocalUserTeam != "管理员权限")
                {
                    throw new Exception("未配置此界面视图权限,请确认");
                }

                //sql = " where" + sql + " ";
                //    sql = @"select sjmx.POS,sjmx.送检单明细号,sjmx.送检日期,sjmx.送检单类型,sjmx.送检数量, hjms.货架描述,hjms.仓库名称, csjz.*,base.图纸编号,base.规格型号,cjyz.检验结果,cjyz.关闭 as 仓库关闭,检验完成,ISNULL(已入库数,0)已入库数,ISNULL(x.不合格数量,0)不合格数量,
                //入库完成,默认检验员,case when(检验结果='不合格'and 采购已处理=0) then 1 else 0 end as 优先级,采购已处理,采购供应商备注 from 采购记录采购送检单主表  csjz
                //           left join 基础数据物料信息表 base on  base.物料编码 = csjz.物料编码 
                //         left join 仓库物料数量表 hjms on hjms.物料编码= csjz.物料编码   and hjms.仓库号=base.仓库号
                //           left join [采购记录采购单检验主表] cjyz  on  cjyz.送检单号 = csjz.送检单号
                //           left join 采购记录采购送检单明细表 sjmx on  sjmx.送检单号 = csjz.送检单号
                //           left join  [采购记录采购检验默认人员表] on [采购记录采购检验默认人员表].物料编码=base.物料编码
                //           left join (select  送检单明细号,sum(已入库数) as 已入库,SUM(不合格数量)as 不合格数量 from [采购记录采购单检验主表] group by 送检单明细号)x
                //           on x.送检单明细号=sjmx.送检单明细号  where sjmx.作废='false'       ";


                sql = @"  select  hjms.货架描述,hjms.仓库名称,base.规格型号 ,检验完成,ISNULL(y.已入库,0)已入库数,ISNULL(x.不合格数量,0)不合格数量,
采购供应商备注,csjmx.*    from 采购记录采购送检单明细表 csjmx 
    left join 基础数据物料信息表 base on  base.物料编码 = csjmx.物料编码 
    left join 仓库物料数量表 hjms on hjms.物料编码=csjmx.物料编码 and hjms.仓库号=base.仓库号

    left join (select 送检单明细号,sum(入库量)as 已入库  from 采购记录采购单入库明细  group by 送检单明细号 )y on  y.送检单明细号=csjmx.送检单明细号  
    left join (select  送检单明细号,SUM(不合格数量)as 不合格数量 from [采购记录采购单检验主表]  where 关闭=0 group by 送检单明细号)x on x.送检单明细号=csjmx.送检单明细号  
      where  csjmx.送检单类型<>'拒收' and  x.不合格数量>0   and 已拒收数<不合格数量    and 拒收加载标记=1  ";
                ////  送检数量- 已入库数
                ///6.5号  去处不合格数
                //                sql = @"  select  hjms.货架描述,hjms.仓库名称,base.规格型号 ,检验完成,ISNULL(y.已入库,0)已入库数,ISNULL(x.不合格数量,0)不合格数量,
                //采购供应商备注,csjmx.*    from 采购记录采购送检单明细表 csjmx 
                //    left join 基础数据物料信息表 base on  base.物料编码 = csjmx.物料编码 
                //    left join 仓库物料数量表 hjms on hjms.物料编码=csjmx.物料编码 and hjms.仓库号=base.仓库号

                //    left join (select 送检单明细号,sum(入库量)as 已入库  from 采购记录采购单入库明细  group by 送检单明细号 )y on  y.送检单明细号=csjmx.送检单明细号  
                //    left join (select  送检单明细号,SUM(不合格数量)as 不合格数量 from [采购记录采购单检验主表] group by 送检单明细号)x on x.送检单明细号=csjmx.送检单明细号  
                //      where  csjmx.送检单类型<>'拒收' ";
                if (checkBox2.Checked == true)
                {
                    if (dateEdit1.EditValue != null && dateEdit2.EditValue != null && dateEdit1.EditValue.ToString() != "" && dateEdit2.EditValue.ToString() != "")
                    {
                        if (Convert.ToDateTime(dateEdit1.EditValue) > Convert.ToDateTime(dateEdit2.EditValue))
                            throw new Exception("起始日期不能大于终止日期！");
                        sql = sql + string.Format(" and csjmx.送检日期>='{0}' and csjmx.送检日期<='{1}' ", dateEdit1.EditValue.ToString()
                        , Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1));
                    }
                }
                //  string sql_补 = "";
                if (checkBox1.Checked == true)
                {
                    sql = sql + string.Format(@" and csjmx.供应商ID = '{0}'", searchLookUpEdit1.EditValue.ToString());

                }
                //if (checkBox2.Checked == true)
                //{
                //    sql_补 = string.Format(@" and xz.目标客户 like '%{0}%'", searchLookUpEdit3.EditValue.ToString());
                //    sql += sql_补;
                //}
                if (checkBox3.Checked == true)
                {
                    sql = sql + string.Format(" and csjmx.送检单号='{0}'", textBox1.Text.ToString());
                }
                if (checkBox4.Checked == true)
                {
                    sql = sql + string.Format(" and csjmx.物料编码='{0}'", searchLookUpEdit2.EditValue.ToString());
                }
                //sql += sql_补;
                // sql = sql + string.Format("order by 优先级 desc, csjmx.采购已处理,送检单号 desc"); 
                dt_送检单主表 = MasterSQL.Get_DataTable(sql, strcon);
                //  dt_送检单主表 = WSAdapter.webservers_getdata.wsmo.GetData_ERP(sql);
                if (dt_送检单主表.Columns.Contains("选择") == false)
                {
                    DataColumn dc = new DataColumn("选择", typeof(bool));
                    dc.DefaultValue = false;
                    dt_送检单主表.Columns.Add(dc);
                    //dt_送检单主表.Columns.Add("选择",typeof(bool));

                }
                if (dt_送检单主表.Columns.Contains("拒收数量") == false)
                {
                    dt_送检单主表.Columns.Add("拒收数量", typeof(decimal));

                }
                gcc1.DataSource = dt_送检单主表;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gvv1.CloseEditor();
                this.BindingContext[dt_送检单主表].EndCurrentEdit();
                DataTable dt_拒收 = dt_送检单主表.Clone();
                //dt_拒收.Columns.Add("POS",typeof(int));
                foreach (DataRow dr in dt_送检单主表.Rows)
                {
                    if (bool.Parse(dr["选择"].ToString()) == true)
                    {
                        dt_拒收.ImportRow(dr);
                    }
                }
                if (dt_拒收.Rows.Count <= 0)
                {
                    throw new Exception("无明细");
                }

                List<string> strList = new List<string>();
                List<string> st_送检 = new List<string>();
                for (int i = 0; i < dt_拒收.Rows.Count; i++)
                {
                    DataRow drrrr = dt_拒收.Rows[i];

                    strList.Add(dt_拒收.Rows[i]["采购单明细号"].ToString());//循环添加元素
                    st_送检.Add(dt_拒收.Rows[i]["送检单明细号"].ToString());//送检单增加拒收数量

                }
                string[] st_采购明细号 = strList.ToArray();
                string sql_补 = "";
                for (int i = 0; i < st_采购明细号.Length; i++)
                {
                    string cai = st_采购明细号[i].ToString();

                    if (i == 0)
                    {
                        sql_补 = sql_补 + string.Format("and 采购明细号= '{0}'", st_采购明细号[i].ToString());
                    }
                    else
                    {
                        sql_补 = sql_补 + string.Format("or 采购明细号= '{0}'", st_采购明细号[i].ToString());
                    }
                }
                string sql = string.Format("select * from  采购记录采购单明细表 where  1=1 {0}", sql_补.ToString());
                DataTable dt_cai = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                dt_cai.Columns.Add("送检参考数", typeof(decimal));

                foreach (DataRow dr in dt_cai.Rows)
                {
                    dr["送检参考数"] = dr["已送检数"];
                }
                string[] st_送检单明细号 = st_送检.ToArray();
                sql_补 = "";
                for (int i = 0; i < st_送检单明细号.Length; i++)
                {
                    string cai = st_送检单明细号[i].ToString();

                    if (i == 0)
                    {
                        sql_补 = sql_补 + string.Format("and 送检单明细号= '{0}'", st_送检单明细号[i].ToString());
                    }
                    else
                    {
                        sql_补 = sql_补 + string.Format("or 送检单明细号= '{0}'", st_送检单明细号[i].ToString());
                    }
                }


                sql = string.Format("select * from  采购记录采购送检单明细表 where  1=1 {0}", sql_补.ToString());
                // sql = string.Format("select * from  采购记录采购单明细表 where  1=1 {0}", sql_补.ToString());
                DataTable dt_送检 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);



                if (dt_cai.Rows.Count > 0)
                {
                    foreach (DataRow drrrr in dt_拒收.Rows)
                    {
                        //       DataRow[] dr = dt_cai.Select(string.Format("送检单明细号='{0}' and 采购单明细号='{1}' ", drrrr["送检单明细号"].ToString(), drrrr["采购单明细号"].ToString()));
                        drrrr["已拒收数"] = decimal.Parse(drrrr["已拒收数"].ToString()) + decimal.Parse(drrrr["拒收数量"].ToString());

                        if (decimal.Parse(drrrr["已拒收数"].ToString()) > decimal.Parse(drrrr["不合格数量"].ToString()))
                        {
                            throw new Exception("当前已拒收数超出不合格数");
                        }
                        DataRow[] dr = null;

                        dr = dt_cai.Select(string.Format("采购明细号='{0}' ", drrrr["采购单明细号"].ToString()));

                        if (dr.Length != 0)
                        {
                            decimal a = decimal.Parse(dr[0]["送检参考数"].ToString()) - decimal.Parse(dr[0]["完成数量"].ToString());
                            decimal c = decimal.Parse(drrrr["拒收数量"].ToString());
                            decimal d = decimal.Parse(dr[0]["拒收数量"].ToString());
                            decimal b = decimal.Parse(drrrr["拒收数量"].ToString()) + decimal.Parse(dr[0]["拒收数量"].ToString());


                            //if (decimal.Parse(dr[0]["送检参考数"].ToString()) - decimal.Parse(dr[0]["完成数量"].ToString()) + decimal.Parse(dr[0]["拒收数量"].ToString()) < decimal.Parse(drrrr["拒收数量"].ToString()) + decimal.Parse(dr[0]["拒收数量"].ToString()))
                            //{
                            //    throw new Exception("拒收数量超出");
                            //}
                           
                            dr[0]["已送检数"] = decimal.Parse(dr[0]["已送检数"].ToString()) - decimal.Parse(drrrr["拒收数量"].ToString());
                            if (decimal.Parse(dr[0]["已送检数"].ToString()) < decimal.Parse(dr[0]["采购数量"].ToString()))
                            {
                                dr[0]["明细完成"] = false;
                            }
                            if (decimal.Parse(dr[0]["已送检数"].ToString()) >= decimal.Parse(dr[0]["采购数量"].ToString()))
                            {
                                dr[0]["明细完成"] = true;
                            }
                            dr[0]["拒收数量"] = decimal.Parse(dr[0]["拒收数量"].ToString()) + decimal.Parse(drrrr["拒收数量"].ToString());
                        }
                    }
                }
                sql = "select * from 采购记录采购送检单明细表  where 1<>1"; // 10-8 没看到这个用在哪里  不动它
                DataTable dt_cx = dt_拒收.Copy();
                //dt_cx.Columns.Remove("ID");
                //dt_cx.Columns.Remove("GUID");
                DateTime t = CPublic.Var.getDatetime();
                // string sss = string.Format("SJ{0}{1:00}{2:00}{3:0000}",t.Year,t.Month,t.Day,CPublic.CNo.fun_得到最大流水号("SJ",t.Year,t.Month,t.Day));
                //2019-10-8 孙杰这边流水号 取得是月的 不是每日的 跟送检单 规则不一致 有重复 单号
                //10-8 东屋要求 拒收单号就用 来源单据的 单号 
                int pos = 0;
                //10-8 dic 里面存 送检单号 和 此单的最大流水号 
                Dictionary<string, int> dic = new Dictionary<string, int>();

                foreach (DataRow d in dt_cx.Rows)
                {
                    DataRow[] dr = dt_送检.Select(string.Format("送检单明细号='{0}'  ", d["送检单明细号"].ToString()));
                    dr[0]["已拒收数"] = d["已拒收数"];

                    //2019-10-8 加
                    if (!dic.ContainsKey(d["送检单号"].ToString()))
                    {
                        string jj = string.Format("select max(pos)pos from 采购记录采购送检单明细表  where 送检单号='{0}'", d["送检单号"]);
                        DataTable temp = CZMaster.MasterSQL.Get_DataTable(jj, strcon);
                        dic.Add(d["送检单号"].ToString(),Convert.ToInt32(temp.Rows[0]["pos"])+1);
                        pos = Convert.ToInt32(temp.Rows[0]["pos"])+1;
                    }
                    else
                    {
                        pos=dic[d["送检单号"].ToString()]+1;
                        dic[d["送检单号"].ToString()] = pos;
                    }
                    //d["GUID"] = Guid.NewGuid();
                    //dt_cx.ImportRow(d);
                    DataRow dr_mx = dt_送检.NewRow();
                    dt_送检.Rows.Add(dr_mx);
                    dr_mx["GUID"] = Guid.NewGuid();
                    dr_mx["送检单号"] = d["送检单号"];
                    //dr_mx["POS"] = pos++;
                    //dr_mx["送检单明细号"] = sss + "-" + pos;
                    // 2019-10-8 pos号和明细号不一致   
                    dr_mx["POS"] = pos;
                    dr_mx["送检单明细号"] = d["送检单号"].ToString() + "-"+ pos;

                    dr_mx["采购单号"] = d["采购单号"];
                    dr_mx["采购单明细号"] = d["采购单明细号"];
                    dr_mx["供应商ID"] = d["供应商ID"];
                    dr_mx["供应商"] = d["供应商"];
                    dr_mx["物料编码"] = d["物料编码"];
                    dr_mx["物料名称"] = d["物料名称"];
                    dr_mx["规格型号"] = d["规格型号"];
                    dr_mx["送检日期"] = d["送检日期"];
                    dr_mx["生效日期"] =t;
                    dr_mx["送检人员ID"] = d["送检人员ID"];
                    dr_mx["送检人员"] = d["送检人员"];
                    dr_mx["操作人员ID"] = d["操作人员ID"];
                    dr_mx["操作人员"] = d["操作人员"];
                    dr_mx["送检数量"] = decimal.Parse(d["拒收数量"].ToString()) * -1;
                    dr_mx["送检单类型"] = "拒收";
                    dr_mx["生效人员ID"] = d["生效人员ID"];
                    dr_mx["生效人员"] = d["生效人员"];
                    dr_mx["生效"] = d["生效"];
                }
                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction st = conn.BeginTransaction("归还");
                try
                {
                    SqlCommand cmd = new SqlCommand("select  * from 采购记录采购送检单明细表 where 1=2", conn, st);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_送检);
                    cmd = new SqlCommand("select  * from 采购记录采购单明细表 where 1=2", conn, st);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_cai);
                    st.Commit();
                    MessageBox.Show("生效成功");
                    simpleButton1_Click(null, null);
                }
                catch (Exception ex)
                {
                    st.Rollback();
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ui拒收操作_Load(object sender, EventArgs e)
        {
            try
            {
                string sql = string.Format(@"select 供应商ID,供应商名称 from 采购供应商表 where 供应商状态='在用'");
                //if (t_片区.Rows.Count > 0)
                //{
                //    string sx = " and  片区 in (";
                //    foreach (DataRow r in t_片区.Rows)
                //    {
                //        sx = sx + string.Format("'{0}',", r["片区"]);
                //    }
                //    sx = sx.Substring(0, sx.Length - 1) + ")";
                //    sql = sql + sx;
                //}
                SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                DataTable dt_供应商 = new DataTable();
                da.Fill(dt_供应商);
                searchLookUpEdit1.Properties.DataSource = dt_供应商;
                searchLookUpEdit1.Properties.DisplayMember = "供应商名称";
                searchLookUpEdit1.Properties.ValueMember = "供应商ID";

                //string sql2 = string.Format(@"select 客户编号,客户名称 from 客户基础信息表 where 停用=0");
                //SqlDataAdapter da2 = new SqlDataAdapter(sql, strcon);
                //DataTable dt_目标客户 = new DataTable();
                //da.Fill(dt_目标客户);
                //searchLookUpEdit3.Properties.DataSource = dt_目标客户;
                //searchLookUpEdit3.Properties.DisplayMember = "客户名称";
                //searchLookUpEdit3.Properties.ValueMember = "客户名称";

                string sql_1 = string.Format(@"select 物料编码,规格型号,物料名称,大类,小类 from 基础数据物料信息表 where 停用=0");
                SqlDataAdapter da_1 = new SqlDataAdapter(sql_1, strcon);
                DataTable dt_物料 = new DataTable();
                da_1.Fill(dt_物料);
                searchLookUpEdit2.Properties.DataSource = dt_物料;
                searchLookUpEdit2.Properties.DisplayMember = "物料编码";
                searchLookUpEdit2.Properties.ValueMember = "物料编码";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try

            {
                gcc1.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
