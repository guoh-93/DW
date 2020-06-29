using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm成品包装确认界面 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM = null;
        DataTable dt_车间 = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
        #endregion

        #region 自用类
        public frm成品包装确认界面()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm成品包装确认界面_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
            devGridControlCustom1.strConn = CPublic.Var.strConn;
            fun_载入数据();
        }
        #endregion

        #region 方法
#pragma warning disable IDE1006 // 命名样式
        private void fun_载入数据()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = "";
                dtM = new DataTable();
                if (CPublic.Var.LocalUserID.ToLower() == "admin" || dt_车间.Rows.Count == 0)
                {
                    sql = string.Format(@"select 生产记录生产检验单主表.*,基础数据物料信息表.原ERP物料编号,图纸编号  from 生产记录生产检验单主表 
                                        left  join    基础数据物料信息表 on  基础数据物料信息表.物料编码= 生产记录生产检验单主表.物料编码
                                    where 生产记录生产检验单主表.包装确认 = 0  and 生产记录生产检验单主表.生效=1");
                }
                else if (dt_车间.Rows.Count>0)
                {
                    sql = string.Format(@"select 生产记录生产检验单主表.*,基础数据物料信息表.原ERP物料编号,图纸编号  from 生产记录生产检验单主表 
                                        left  join    基础数据物料信息表 on  基础数据物料信息表.物料编码= 生产记录生产检验单主表.物料编码
                                    where 生产记录生产检验单主表.包装确认 = 0  and 生产记录生产检验单主表.生效=1 and 生产记录生产检验单主表.生产车间='{0}'", dt_车间.Rows[0]["生产车间"]);
                }
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                DataView dv = new DataView(dtM);
                dv.RowFilter = "包装确认 = 0";
                gc.DataSource = dv;

            }
            catch (Exception ex)
            {
                MessageBox.Show("载入数据出错");
            }
        }
        #endregion

        #region 界面操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_载入数据();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                int index = 0;
                if (MessageBox.Show("是否确认本单子？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                    //2017-6-5 备注 完工时判断 如果有未领料 不允许完工
                    string sql_1 = string.Format(@"select 生产记录生产工单待领料明细表.*,原ERP物料编号  from 生产记录生产工单待领料明细表,基础数据物料信息表
  
                                        where 生产记录生产工单待领料明细表.物料编码 =基础数据物料信息表.物料编码 and 生产工单号='{0}' and 完成=0", dr["生产工单号"]);
                    DataTable dt = new DataTable();
                    dt = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
                    if (dt.Rows.Count > 0)
                    {
                        string str = "";
                        foreach (DataRow rr in dt.Rows)
                        {
                            str = str + rr["原ERP物料编号"].ToString() + " " + rr["物料名称"].ToString()+"/n";
                        }
                        MessageBox.Show(str, "尚有物料未领请核实");
                    }

                    index = gv.FocusedRowHandle;
                 
                    //制造六课 完工后 要 打印小标签
                    if (CPublic.Var.localUser课室编号 == "0001030106")
                    {
                        if (MessageBox.Show("是否需要打印标贴", "确认？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            string str_打印机 = "";
                            PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();

                            this.printDialog1.Document = this.printDocument1;

                            DialogResult drx = this.printDialog1.ShowDialog();

                            if (drx == DialogResult.OK)
                            {
                                str_打印机 = this.printDocument1.PrinterSettings.PrinterName;
                              
                                 
                                   
                                    string s = string.Format("select 物料编码,最小包装 from 基础数据物料信息表 where 物料编码 ='{0}'", dr["物料编码"]);
                                    using (SqlDataAdapter daa = new SqlDataAdapter(s, strconn))
                                    {
                                        DataTable temp = new DataTable();
                                        daa.Fill(temp);
                                        int i_最小包装 = Convert.ToInt32(temp.Rows[0]["最小包装"]);
                                        if (i_最小包装 == 0)
                                        {

                                            fm补打箱贴标签 fm = new fm补打箱贴标签(dr);
                                            fm.StartPosition = FormStartPosition.CenterParent;
                                            fm.ShowDialog();

                                            if (fm.zxbz == 0)
                                            {
                                                throw new Exception(string.Format("未完成对物料 {0} 的最小包装的维护", dr["图纸编号"].ToString()));
                                            }
                                            else
                                            {
                                                i_最小包装 = fm.zxbz;
                                                temp.Rows[0]["最小包装"] = i_最小包装;
                                                new SqlCommandBuilder(daa);
                                                daa.Update(temp);


                                            }
                                        }

                                        if (i_最小包装 != 0)
                                        {
                                            int count = Convert.ToInt32(dr["合格数量"]) / i_最小包装;
                                            int i_余数 = Convert.ToInt32(dr["合格数量"]) % i_最小包装;
                                            if (i_余数 != 0)
                                            {
                                                count++;
                                            }

                                            Dictionary<string, string> dic = new Dictionary<string, string>();
                                            dic.Add("gdh", dr["生产工单号"].ToString());

                                            dic.Add("pch", dr["生产工单号"].ToString() + "-");
                                            dic.Add("dyzs", count.ToString("0"));
                                            dic.Add("ybh", dr["原ERP物料编号"].ToString());
                                            dic.Add("tzbh", dr["图纸编号"].ToString());
                                            dic.Add("scsj", CPublic.Var.getDatetime().ToString("yyyy-MM-dd"));
                                            dic.Add("sl", i_最小包装.ToString("0"));
                                            dic.Add("ys", i_余数.ToString("0"));

                                            string path = Application.StartupPath + @"\Mode\制六标签.lab";
                                            Lprinter lp = new Lprinter(path, dic, str_打印机, count);
                                            lp.Start();
                                         
                                        }



                                    }

                                }



                            }
                    }

                    dr["包装确认"] = 1;
                    //17-11-17 添加 包装时间
                    dr["包装时间"] = CPublic.Var.getDatetime();

                    string sql = "select * from 生产记录生产检验单主表 where 1<>1";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    new SqlCommandBuilder(da);
                    da.Update(dtM);
                    if (index != 0 && index <= gv.DataRowCount)
                    {
                        gv.FocusedRowHandle = index;

                    }
                    else if (index > gv.DataRowCount)
                    {
                        gv.FocusedRowHandle = gv.DataRowCount;
                    }
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("非常抱歉,暂时无法确认");
                CZMaster.MasterLog.WriteLog(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion

#pragma warning disable IDE1006 // 命名样式
        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void 包装方式维护ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
