using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;

namespace ERPproduct
{
    public partial class UI生产补料 : UserControl
    {

        #region 变量
        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dt_下拉;
        DataTable dt_生产,dt_打印1,dt_打印2;
        string str_工单;
        DataTable dt_仓库;

        #endregion

        #region    加载

        public UI生产补料()
        {
            InitializeComponent();
        }
        public UI生产补料(string str)
        {
            InitializeComponent();
            str_工单 = str;
            barEditItem1.EditValue = str_工单;

        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            try
            {
                fun_load();
                if (barEditItem1.EditValue != null && barEditItem1.EditValue.ToString() != "")
                {
                    barLargeButtonItem1_ItemClick(null, null);
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
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {

            DateTime t = CPublic.Var.getDatetime().Date;
            string ss = "";
            if (CPublic.Var.LocalUserID != "admin")
            {
                dt_生产 = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
                if (dt_生产.Rows.Count == 0) throw new Exception("生产组织关系中没有维护相关信息");
                ss = string.Format("and  生产车间='{0}'", dt_生产.Rows[0]["生产车间"]);
            }
            string sql = string.Format(@"select gd.*,base.原ERP物料编号,计量单位 from 生产记录生产工单表 gd
            left join 基础数据物料信息表 base on gd.物料编码= base.物料编码
            where gd.生效日期>'{0}' and gd.关闭=0 and 完成=0  {1}", t.AddMonths(-6),ss );
            //原来是卡在 完工后就不能补料  后改为入库完成前都可以补料 
            //存在那种做完了检验完成有问题 需要补料 返修的 
            dt_下拉 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            repositoryItemSearchLookUpEdit1.DataSource = dt_下拉;
            repositoryItemSearchLookUpEdit1.DisplayMember = "生产工单号";
            repositoryItemSearchLookUpEdit1.ValueMember = "生产工单号";
            dt_仓库 = new DataTable();
            string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别'";
            SqlDataAdapter da = new SqlDataAdapter(sql4, strcon);
            da.Fill(dt_仓库);
            repositoryItemGridLookUpEdit1.DataSource = dt_仓库;
            repositoryItemGridLookUpEdit1.DisplayMember = "仓库号";
            repositoryItemGridLookUpEdit1.ValueMember = "仓库号";

        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_save()
#pragma warning restore IDE1006 // 命名样式
        {
            /////5.17 加的打印
            gridView1.CloseEditor();
            this.BindingContext[dtM].EndCurrentEdit();
            DataTable dtm = (DataTable)this.gridControl1.DataSource;
            DataView dv = new DataView(dtm);
            dv.RowFilter = "选择=1";           
            DataTable dts = dv.ToTable();
            dt_打印2 = new DataTable();
            dt_打印2 = dts.Copy();





            DateTime t = CPublic.Var.getDatetime().Date;
            string str_待领料单号 = string.Format("DL{0}{1:D2}{2:D4}", t.Year, t.Month, CPublic.CNo.fun_得到最大流水号("DL", t.Year, t.Month));
            string str_待领料号; //用来搜索对应的 明细 
            //现在 待领料主表中新增一条记录 
            DataTable dt;
            string sql = string.Format("select * from 生产记录生产工单待领料主表 where 生产工单号='{0}' ", barEditItem1.EditValue.ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dt = new DataTable();
                da.Fill(dt);

                str_待领料号 = dt.Rows[0]["待领料单号"].ToString();
                dt.Rows[0]["待领料单号"] = str_待领料单号;
                dt.Rows[0]["完成"] = 0;
                dt.Rows[0]["关闭"] = 0;
                dt.Rows[0]["创建日期"] = t;
                dt.Rows[0]["制单人员"] = CPublic.Var.localUserName;
                dt.Rows[0]["制单人员ID"] = CPublic.Var.LocalUserID;
                dt.Rows[0]["领料类型"] = "生产补料";
                dt.Rows[0]["备注1"] = textBox1.Text.Trim();
                dt.AcceptChanges();
                dt.Rows[0].SetAdded();

            }
            //待领料明细表 中
            string sql_mx = "select * from 生产记录生产工单待领料明细表 where 1<>1";
            DataTable dt_mx = CZMaster.MasterSQL.Get_DataTable(sql_mx, strcon);
            using (SqlDataAdapter da = new SqlDataAdapter(sql_mx, strcon))
            {
                dtM.AcceptChanges();
                int pos = 0;
                foreach (DataRow dr in dtM.Rows)
                {
                    if (dr["选择"].Equals(true))
                    {
                        DataRow r_mx = dt_mx.NewRow();

                        r_mx["待领料单号"] = str_待领料单号;
                        r_mx["待领料单明细号"] = str_待领料单号 +"-"+ pos++.ToString("00");
                        r_mx["待领料总量"] = dr["输入领料数量"];
                        r_mx["生产工单号"] = barEditItem1.EditValue.ToString();
                        r_mx["生产制令单号"] = dt.Rows[0]["生产制令单号"];
                        r_mx["生产工单类型"] = "生产补料";
                        r_mx["物料编码"] = dr["物料编码"];
                        r_mx["物料名称"] = dr["物料名称"];
                        r_mx["生产车间"] = dt.Rows[0]["生产车间"];
                        r_mx["领料人ID"] = dt.Rows[0]["领料人ID"];
                        r_mx["领料人"] = dt.Rows[0]["领料人"];
                        r_mx["生产车间"] = dt.Rows[0]["生产车间"];
                        r_mx["规格型号"] = dr["规格型号"];
                        r_mx["仓库号"] = dr["仓库号"];
                        r_mx["仓库名称"] = dr["仓库名称"];
                        r_mx["已领数量"] = 0;
                        r_mx["未领数量"] = dr["输入领料数量"];
                        r_mx["制单人员"] = CPublic.Var.localUserName;
                        r_mx["制单人员ID"] = CPublic.Var.LocalUserID;
                        r_mx["修改日期"] = r_mx["创建日期"] = t;
                        r_mx["完成"] = 0;

                        dt_mx.Rows.Add(r_mx);

                        //dr.AcceptChanges();

                        // dr.SetAdded();
                    }

                }

            }

            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction pb = conn.BeginTransaction("生产补料");
            try
            {
                SqlCommand cmm_1 = new SqlCommand(sql, conn, pb);
                SqlCommand cmm_2 = new SqlCommand(sql_mx, conn, pb);

                SqlDataAdapter da1 = new SqlDataAdapter(cmm_1);
                SqlDataAdapter da2 = new SqlDataAdapter(cmm_2);



                new SqlCommandBuilder(da1);
                new SqlCommandBuilder(da2);





                dt_打印1 = new DataTable();
                dt_打印1 = dt.Copy();
                //dt_打印2 = new DataTable();
                //dt_打印2 = dt_mx.Copy();
                da1.Update(dt);
                da2.Update(dt_mx);
                
           


                pb.Commit();


            }
            catch (Exception ex)
            {
                pb.Rollback();
                throw new Exception("生产补料失败，请重试");
            }


        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {
            //int i = 0;
            string sql = string.Format("select * from 生产记录生产工单表 where 生产工单号='{0}' ", barEditItem1.EditValue.ToString());
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dt.Rows[0]["关闭"]))
                {
                    throw new Exception("该工单已关闭，不可补料");
                }
                if (Convert.ToBoolean(dt.Rows[0]["检验完成"]))
                {
                    throw new Exception("该工单已检验完成，不可补料");
                }
            }


            DataView dv = new DataView(dtM);
            dv.RowFilter = "选择=1";
            if (dv.Count == 0)
            {
                throw new Exception("未选择需补料的物料");
            }
            foreach (DataRow r in dtM.Rows)
            {
                if (r["选择"].Equals(true))
                {
                    try
                    {
                        decimal a = Convert.ToDecimal(r["输入领料数量"]);

                        if (a <= 0)
                        {
                            throw new Exception("领料数量不能小于0,请重新输入");

                        }
                    }
                    catch
                    {
                        throw new Exception("请正确输入领料数量格式");

                    }
                    if (Convert.ToDecimal(r["输入领料数量"]) > Convert.ToDecimal(r["库存总数"]))
                    {
                        throw new Exception("库存总数不足！");
                    }
                    //if (Convert.ToDecimal(r["输入领料数量"]) > Convert.ToDecimal(r["未领数量"]))
                    //{
                    //    i++;

                    //}

                }
            }
            //if (i > 0)
            //{
            //    if (MessageBox.Show("领料数量大于未领数量，是否继续？", "提醒", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
            //    {
            //        throw new Exception("请修改");
            //    }
            //}

        }

        #endregion









        //查询 输入工单 
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (barEditItem1.EditValue != null && barEditItem1.EditValue.ToString() != "")
                {
                    string sql_1 = string.Format("select * from 生产记录生产工单待领料主表  where 生产工单号='{0}' ", barEditItem1.EditValue);
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
                    if (dt.Rows.Count > 0)
                    {
                        //                    string sql = string.Format(@"select a.*,人事基础部门表.部门名称 from (select 生产记录生产工单待领料明细表.*,仓库物料数量表.有效总数,仓库物料数量表.库存总数,基础数据物料信息表.原ERP物料编号 from 生产记录生产工单待领料明细表 
                        //                                              left join 仓库物料数量表  on   仓库物料数量表.物料编码= 生产记录生产工单待领料明细表.物料编码 
                        //                                              left join 基础数据物料信息表  on 基础数据物料信息表.物料编码=生产记录生产工单待领料明细表.物料编码
                        //                                              where 生产记录生产工单待领料明细表.生产工单号='{0}') a   
                        //                                              left join 人事基础部门表 on 人事基础部门表.部门编号 = a.生产车间 ", barEditItem1.EditValue.ToString());
                        //                    string sql = string.Format(@"select a.*,a.数量 as BOM数量,人事基础部门表.部门名称,a.计量单位 from (select 基础数据物料BOM表.*,车间编号,仓库物料数量表.有效总数,仓库物料数量表.库存总数
                        //                                              ,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.物料编码,基础数据物料信息表.物料名称,基础数据物料信息表.n原ERP规格型号 as 规格型号,{0} as 生产车间
                        //                                               from 基础数据物料BOM表 
                        //                                              left join 仓库物料数量表  on   仓库物料数量表.物料编码= 基础数据物料BOM表.子项编码 
                        //                                              left join 基础数据物料信息表  on 基础数据物料信息表.物料编码=基础数据物料BOM表.子项编码
                        //                                              where 基础数据物料BOM表.产品编码='{1}') a   
                        //                                              left join 人事基础部门表 on 人事基础部门表.部门编号 = a.生产车间 ", dt_生产.Rows[0]["生产车间"], dt.Rows[0]["产品编码"]);
                        string sql = string.Format(@"select a.*,a.数量 as BOM数量,a.计量单位,WIPType as 领料类型 from (select bom.*,车间编号,isnull(kc.有效总数,0)有效总数,isnull(kc.库存总数,0)库存总数
                                              ,base.原ERP物料编号,base.物料编码,base.物料名称,base.规格型号
                                               from 基础数据物料BOM表 bom
                                              left join 仓库物料数量表 kc  on   kc.物料编码= bom.子项编码 and kc.仓库号=bom.仓库号
                                              left join 基础数据物料信息表 base  on base.物料编码=bom.子项编码
                                              where bom.产品编码='{0}' and WIPType<>'入库倒冲') a ", dt.Rows[0]["产品编码"]);
                        using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                        {
                            dtM = new DataTable();
                            da.Fill(dtM);
                            if (dtM.Rows.Count > 0)
                            {
                                DataTable t = dtM.Copy();
                                foreach (DataRow dr in t.Rows)
                                {
                                    if (dr["WIPType"].ToString() == "虚拟")
                                    {
                                        string s = string.Format(@"select a.*,a.数量 as BOM数量,a.计量单位,'虚拟件子件' as 领料类型 from (select bom.*,车间编号,isnull(kc.有效总数,0)有效总数,isnull(kc.库存总数,0)库存总数
                                              ,base.原ERP物料编号,base.物料编码,base.物料名称,base.规格型号
                                               from 基础数据物料BOM表 bom
                                              left join 仓库物料数量表 kc  on kc.物料编码 = bom.子项编码 and kc.仓库号 = bom.仓库号
                                              left join 基础数据物料信息表 base  on base.物料编码 = bom.子项编码
                                              where bom.产品编码 = '{0}' and WIPType<>'入库倒冲') a", dr["子项编码"]);

                                        using (SqlDataAdapter a = new SqlDataAdapter(s, strcon))
                                        {
                                            a.Fill(dtM);
                                        }
                                    }

                                }
                                dtM.Columns.Add("选择", typeof(bool));
                                dtM.Columns.Add("输入领料数量");



                                gridControl1.DataSource = dtM;
                            }
                            else
                            {
                                MessageBox.Show("未搜索到该工单");
                            }


                        }
                    }
                    else
                    {
                        MessageBox.Show("请正确输入工单号");
                    }

                }

                if (barEditItem1.EditValue != null && barEditItem1.EditValue.ToString() == "")
                {
                    //                string sql = @"select *,仓库物料数量表.有效总数,仓库物料数量表.库存总数 from 生产记录生产工单待领料明细表 left join 仓库物料数量表
                    //                                          on   仓库物料数量表.物料编码=  生产记录生产工单待领料明细表.物料编码
                    //                                          where 1<>1";

                    string sql = @"select dlmx.*,isnull(kc.有效总数,0)有效总数,isnull(kc.库存总数,0)库存总数,base.原ERP物料编号,bm.部门名称  
                              from 生产记录生产工单待领料明细表 dlmx 
                              left join 仓库物料数量表 kc on kc.物料编码= dlmx.物料编码
                              left join 基础数据物料信息表 base on base.物料编码=dlmx.物料编码    
                              left join 人事基础部门表 bm on bm.部门编号 = dlmx.生产车间 where 1<>1 ";
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                    {
                        dtM = new DataTable();
                        da.Fill(dtM);
                        if (dtM.Rows.Count > 0)
                        {
                            dtM.Columns.Add("选择", typeof(bool));
                            dtM.Columns.Add("输入领料数量");

                            gridControl1.DataSource = dtM;
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //生效 存 待领料主表和 明细表 
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();

                fun_check();

                fun_save();
                if (MessageBox.Show("确认打印单据吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    fun_打印();
                }
                   




                fun_load();

                MessageBox.Show("生效成功");
                UserControl1_Load(null, null);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }


        private void fun_打印()
        {

            //gridView1.CloseEditor();
            //this.BindingContext[dtM].EndCurrentEdit();
            //DataTable dtm = (DataTable)this.gridControl1.DataSource;
            //DataView dv = new DataView(dtm);
            //dv.RowFilter = "选择=1";
            //DataTable dts = dv.ToTable();
            //foreach (DataRow dr in dts.Rows)
            //{
            //    if (dr["输入领料数量"].ToString() == "")
            //    {
            //        throw new Exception("请输入数量再打印");
            //    }
            //}

            //DataView dv222 = new DataView(dt_下拉);
            //dv222.RowFilter = string.Format("生产工单号='{0}' ", barEditItem1.EditValue);
            //DataTable dt_头 = dv222.ToTable();
            DataRow drM = dt_打印1.Rows[0];
            Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
            Type outerForm = outerAsm.GetType("ERPreport.生产补料", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                                                                        //CPublic.UIcontrol.Showpage(ui, t.Rows[0]["打开界面名称"].ToString());
            object[] drr = new object[2];

            drr[0] = drM;
            drr[1] = dt_打印2;
            //   drr[2] = dr["出入库申请单号"].ToString();
            Form ui = Activator.CreateInstance(outerForm, drr) as Form;
            // UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
            ui.ShowDialog();




        }



        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try

            {
                gridView1.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                DataTable dtm = (DataTable)this.gridControl1.DataSource;
                DataView dv = new DataView(dtm);
                dv.RowFilter = "选择=1";
                DataTable dts = dv.ToTable();
                foreach (DataRow dr in dts.Rows)
                {
                    if (dr["输入领料数量"].ToString() == "")
                    {
                        throw new Exception("请输入数量再打印");
                    }
                }


                DataView dv222 = new DataView(dt_下拉);
                dv222.RowFilter = string.Format("生产工单号='{0}' ", barEditItem1.EditValue);
                DataTable dt_头 = dv222.ToTable();
                DataRow drM = dt_头.Rows[0];
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPreport.生产补料", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                                                                            //CPublic.UIcontrol.Showpage(ui, t.Rows[0]["打开界面名称"].ToString());
                object[] drr = new object[2];

                drr[0] = drM;
                drr[1] = dts;
                //   drr[2] = dr["出入库申请单号"].ToString();
                Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                // UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
                ui.ShowDialog();





            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        



        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show(string.Format("是否确认关闭该界面？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                CPublic.UIcontrol.ClosePage();
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_MouseUp(object sender, MouseEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Button == MouseButtons.Left)
            {
                int[] dr = gridView1.GetSelectedRows();
                if (dr.Length > 1)
                {
                    for (int i = 0; i < dr.Length; i++)
                    {
                        DataRow r = gridView1.GetDataRow(dr[i]);
                        if (r["选择"].Equals(true))
                        {
                            r["选择"] = 0;

                        }
                        else
                        {
                            r["选择"] = 1;
                        }

                    }
                    //gridView1.FocusedRowHandle = dr[dr.Length - 1];
                    gridView1.MoveBy(dr[dr.Length - 1]);
                }
            }
        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if(e.Column.FieldName=="仓库号")
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string s = string.Format("select  仓库名称,库存总数,有效总数 from  仓库物料数量表 where 物料编码='{0}' and 仓库号='{1}'",dr["物料编码"],e.Value);
                DataTable t = CZMaster.MasterSQL.Get_DataTable(s,strcon);
                if(t.Rows.Count>0)
                {

                    dr["仓库名称"] = t.Rows[0]["仓库名称"];
                    dr["库存总数"] = t.Rows[0]["库存总数"];
                    dr["有效总数"] = t.Rows[0]["有效总数"];

                }
                else
                {
                    dr["仓库名称"] = dt_仓库.Select(string.Format("仓库号='{0}'",e.Value))[0]["仓库名称"];
                    dr["库存总数"] = 0;
                    dr["有效总数"] = 0;

                }
            }
        }
    }
}
