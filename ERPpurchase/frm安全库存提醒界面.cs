using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
namespace ERPpurchase
{
    public partial class frm安全库存提醒界面 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM; 
        string str_采购单号 = "";
        DataTable dt_主;
        DataTable dt_子;
        DataTable dt_供应商编号;
        DataRow drM;
        #endregion

        #region 自用类
        public frm安全库存提醒界面()
        {
            InitializeComponent();
        }

        private void frm安全库存提醒界面_Load(object sender, EventArgs e)
        {
            fun_载入();
            fun_供应商();
            checkBox1.Checked = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                DataView dv = new DataView(dtM);
                dv.RowFilter = "物料类型 = '原材料'";
                gc.DataSource = dv;
                checkBox2.Checked = false;
            }
            else
            {
                if (checkBox2.Checked == true)
                { }
                else
                {
                    checkBox2.Checked = true;
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                DataView dv = new DataView(dtM);
                dv.RowFilter = "物料类型 = '成品' or 物料类型 = '半成品'";
                gc.DataSource = dv;
                checkBox1.Checked = false;
            }
            else
            {
                if (checkBox1.Checked == true)
                { }
                else
                {
                    checkBox1.Checked = true;
                }
            }
        }

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            //判断右键菜单是否可用
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc, new Point(e.X, e.Y));
            }
        }
        #endregion

        #region 方法
        private void fun_载入()
        {
            string sql = @"select 基础数据物料信息表.物料编码,基础数据物料信息表.物料类型,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.物料名称,基础数据物料信息表.规格,基础数据物料信息表.n原ERP规格型号,
            基础数据物料信息表.库存下限,基础数据物料信息表.图纸编号,基础数据物料信息表.供应商编号,基础数据物料信息表.仓库号,基础数据物料信息表.仓库名称,基础数据物料信息表.标准单价
            ,基础数据物料信息表.默认供应商,仓库物料数量表.库存总数,仓库物料数量表.有效总数 from 基础数据物料信息表 left join 仓库物料数量表 
            on 基础数据物料信息表.物料编码 = 仓库物料数量表.物料编码 where 基础数据物料信息表.库存下限 >= 仓库物料数量表.库存总数 
            order by 基础数据物料信息表.供应商编号";//and 基础数据物料信息表.物料类型 = '原材料' 
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            dtM.Columns.Add("选择",typeof(Boolean));
            dtM.Columns.Add("输入数量");
        }

        private void fun_供应商()
        {
            //供应商表
            string sql = "select * from 采购供应商表";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            dt_供应商编号 = new DataTable();
            da.Fill(dt_供应商编号);
        }

        private void fun_转采购单()
        {
            try
            {
                SqlDataAdapter da;
                //采购单号          
                str_采购单号 = string.Format("PO{0}{1:00}{2:00}{3:0000}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, CPublic.CNo.fun_得到最大流水号("PO", DateTime.Now.Year, DateTime.Now.Month)); //采购单号
                //供应商的信息数据
                DataRow[] dr_供应商 = dt_供应商编号.Select(string.Format("供应商ID = '{0}'", dtM.Rows[0]["供应商编号"]));

                int pos = 1;
                decimal de_总金额 = 0;  //计算整个采购单的总金额

                foreach (DataRow r in dtM.Rows)
                {
                    if (r["选择"].ToString().ToLower() == "true")
                    {
                        DataRow r1 = dt_子.NewRow();
                        dt_子.Rows.Add(r1);
                        r1["GUID"] = System.Guid.NewGuid();
                        r1["采购单号"] = str_采购单号;
                        r1["采购明细号"] = str_采购单号 + "-" + pos.ToString("00");
                        r1["明细类型"] = "标准类型";
                        r1["采购POS"] = pos++;
                        r1["物料编码"] = r["物料编码"];
                        r1["物料名称"] = r["物料名称"];
                        r1["规格型号"] = r["规格"];
                        r1["图纸编号"] = r["图纸编号"];
                        r1["仓库号"] = r["仓库号"];
                        r1["仓库名称"] = r["仓库名称"];
                        r1["采购数量"] = r["输入数量"];
                        r1["单价"] = r["标准单价"];
                        r1["未税单价"] = Convert.ToDecimal(r["标准单价"]) / Convert.ToDecimal(1.17);
                        if (dr_供应商.Length > 0)
                        {
                            r1["供应商ID"] = dr_供应商[0]["供应商ID"];
                            r1["供应商"] = dr_供应商[0]["供应商"];
                            r1["供应商负责人"] = dr_供应商[0]["供应商负责人"];
                            r1["供应商电话"] = dr_供应商[0]["供应商电话"];
                        }
                        r1["税率"] = 17;
                        r1["金额"] = Convert.ToDecimal(r["输入数量"]) * Convert.ToDecimal(r["标准单价"]);
                        //总金额
                        de_总金额 += (decimal)r1["金额"];
                        r1["未税金额"] = ((decimal)r1["金额"] / (decimal)1.17);
                        r1["税金"] = ((decimal)r1["金额"] / (decimal)1.17) * (decimal)0.17;
                        r1["员工号"] = CPublic.Var.LocalUserID;
                        r1["采购人"] = CPublic.Var.localUserName;
                        r1["未完成数量"] = r["输入数量"];
                        r1["操作员ID"] = CPublic.Var.LocalUserID;
                        r1["操作员"] = CPublic.Var.localUserName;
                        r1["生成人员"] = CPublic.Var.localUserName;
                    }
                }

                drM = dt_主.NewRow();
                drM["GUID"] = System.Guid.NewGuid();  //GUID
                drM["采购单号"] = str_采购单号; //采购单号
                drM["采购计划日期"] = System.DateTime.Now;
                drM["未税金额"] = (de_总金额 / (decimal)1.17);
                drM["税率"] = 17;
                drM["总金额"] = de_总金额;
                drM["税金"] = (de_总金额 / (decimal)1.17) * (decimal)0.17;
                if (dr_供应商.Length > 0)
                {
                    drM["供应商ID"] = dr_供应商[0]["供应商ID"];
                    drM["供应商"] = dr_供应商[0]["供应商"];
                    drM["供应商负责人"] = dr_供应商[0]["供应商负责人"];
                    drM["供应商电话"] = dr_供应商[0]["供应商电话"];
                }
                drM["员工号"] = CPublic.Var.LocalUserID;
                drM["经办人"] = CPublic.Var.localUserName;
                drM["采购公司"] = "苏州未来电器股份有限公司";
                drM["录入日期"] = System.DateTime.Now;
                drM["创建日期"] = System.DateTime.Now;
                drM["修改日期"] = System.DateTime.Now;
                drM["操作员ID"] = CPublic.Var.LocalUserID;
                drM["操作员"] = CPublic.Var.localUserName;
                drM["生成人员"] = CPublic.Var.localUserName;
                dt_主.Rows.Add(drM);

                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("newPurchase");
                SqlCommand cmd_cgzb = new SqlCommand("select * from 采购记录采购单主表 where 1<>1", conn, ts);
                SqlCommand cmd_cgmx = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, ts);

                try
                {
                    //采购单主表
                    da = new SqlDataAdapter(cmd_cgzb);
                    new SqlCommandBuilder(da);
                    da.Update(dt_主);
                    //采购明细表
                    da = new SqlDataAdapter(cmd_cgmx);
                    new SqlCommandBuilder(da);
                    da.Update(dt_子);
                    ts.Commit();
                }
                catch
                {
                    ts.Rollback();
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_newPurchase");
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 界面操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_载入();
                DataView dv = new DataView(dtM);
                dv.RowFilter = "物料类型 = '原材料'";
                gc.DataSource = dv;
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
                gv.CloseEditor();
                gc.BindingContext[dtM].EndCurrentEdit();
                if (checkBox1.Checked == false)
                {
                    throw new Exception("不是原材料，不能转采购单！");
                }

                //检查供应商是否一致
                string str = "";
                foreach (DataRow r in dtM.Rows)
                {
                    if (r["选择"].ToString().ToLower() == "true")
                    {
                        if (str != "")
                        {
                            if (str == r["供应商编号"].ToString())
                            {

                            }
                            else
                            {
                                throw new Exception("所选原材料的供应商不一致，请重新选择！");
                            }
                        }
                        else
                        {
                            str = r["供应商编号"].ToString();
                        }
                    }
                }

                string sql_主 = "select * from 采购记录采购单主表 where 1<>1";
                dt_主 = new DataTable();
                SqlDataAdapter da_主 = new SqlDataAdapter(sql_主, strconn);
                da_主.Fill(dt_主);

                string sql_子 = "select * from 采购记录采购单明细表 where 1<>1";
                dt_子 = new DataTable();
                SqlDataAdapter da_子 = new SqlDataAdapter(sql_子, strconn);
                da_子.Fill(dt_子);

                fun_转采购单();
                if (MessageBox.Show(string.Format("采购单\"{0}\"生成成功，是否跳转到采购单明细界面？", str_采购单号), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    frm采购单明细 fm = new frm采购单明细(drM);
                    CPublic.UIcontrol.AddNewPage(fm, "采购单明细");
                }
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
        #endregion

        #region 右键菜单
        private void 查看物料详细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                //ERPStock.frm仓库物料数量明细 fm = new ERPStock.frm仓库物料数量明细(dr["物料编码"].ToString());
                //CPublic.UIcontrol.AddNewPage(fm, string.Format("物料{0}明细", dr["物料编码"].ToString()));

                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, @"ERPStock.dll"));
                Type outerForm = outerAsm.GetType("ERPStock.frm仓库物料数量明细", false);
                //  Form ui = Activator.CreateInstance(outerForm) as Form;
                object[] dic = new object[1];
                dic[0] = dr["物料编码"].ToString();


                UserControl ui = Activator.CreateInstance(outerForm, dic) as UserControl; // 过往出口明细 构造函数 有两个参数,string ,datetime 
                CPublic.UIcontrol.Showpage(ui, "仓库物料数量明细");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }  
        #endregion 
    }
}
