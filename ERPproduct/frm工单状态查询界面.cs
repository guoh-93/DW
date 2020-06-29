using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class frm工单状态查询界面 : UserControl
    {
        #region 变量

        DataTable dt_生产关系 = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);

        string strcon = CPublic.Var.strConn;
        //DataTable dt_已完成;
        //DataTable dt_未完成;
        DataTable dt_未开工工单;
        DataTable dt_已入库工单;
        DataTable dt_在产工单数;
        DataTable dt_已检验未入库;
        DataTable dt_全部工单;
        DataTable dt_完工未检验;

        #endregion
        public frm工单状态查询界面()
        {
            InitializeComponent();
            DateTime dtime = Convert.ToDateTime(System.DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss"));
            barEditItem2.EditValue = dtime.AddDays(1).AddSeconds(-1);
            barEditItem1.EditValue = dtime.AddDays(-3);

        }

#pragma warning disable IDE1006 // 命名样式
        private void frm工单状态查询界面_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select 部门编号,部门名称 from [人事基础部门表] where 部门名称 like '%制造%' and 部门名称 like '%课%' ";
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            repositoryItemSearchLookUpEdit2.DataSource = dt;
            repositoryItemSearchLookUpEdit2.ValueMember = "部门编号";
            repositoryItemSearchLookUpEdit2.DisplayMember = "部门名称";




            if (dt_生产关系.Rows.Count > 0)
            {
                string sql_1 = string.Format("select * from 人事基础部门表 where 部门编号 ='{0}'", dt_生产关系.Rows[0]["生产车间"]);
                DataTable dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
                if (dt_1.Rows.Count > 0)
                {
                    barEditItem3.EditValue = dt_生产关系.Rows[0]["生产车间"];

                }

            }
            barLargeButtonItem1_ItemClick(null, null);


        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_未开工工单()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql_未开工工单 = "";
            if (dt_生产关系.Rows.Count > 0 && dt_生产关系.Rows[0]["生产车间"].ToString() != "")
            {
                sql_未开工工单 = string.Format(@"select 生产记录生产工单表.*,基础数据物料信息表.原ERP物料编号 from 生产记录生产工单表,基础数据物料信息表 
                                        where 生产记录生产工单表.物料编码=基础数据物料信息表.物料编码 and  生产记录生产工单表.生产车间='{0}' 
                                       and 生产记录生产工单表.关闭=0 and 生产记录生产工单表.制单日期 >= '{1}' and 生产记录生产工单表.制单日期 <='{2}' and 生产记录生产工单表.生效=0",
                            dt_生产关系.Rows[0]["生产车间"], barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1));
            }
            else
            {
                sql_未开工工单 = string.Format(@"select 生产记录生产工单表.*,基础数据物料信息表.原ERP物料编号 from 生产记录生产工单表,基础数据物料信息表 
                                                where 生产记录生产工单表.物料编码=基础数据物料信息表.物料编码 and 生产记录生产工单表.关闭=0
                                                and 生产记录生产工单表.制单日期 >= '{0}' and 生产记录生产工单表.制单日期 <='{1}' and 生产记录生产工单表.生效=0 "
                    , barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1));

            }
            dt_未开工工单 = CZMaster.MasterSQL.Get_DataTable(sql_未开工工单, strcon);


            gridControl2.DataSource = dt_未开工工单;


        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_已入库工单()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql_已入库工单 = "";
            if (dt_生产关系.Rows.Count > 0 && dt_生产关系.Rows[0]["生产车间"].ToString() != "")
            {
                sql_已入库工单 = string.Format(@"select 生产记录成品入库单明细表.*,生产制令单号,基础数据物料信息表.大类,基础数据物料信息表.标准单价,基础数据物料信息表.原ERP物料编号,生产记录生产工单表.工单负责人,库存总数 from 生产记录成品入库单明细表
                                        left join  基础数据物料信息表  on   生产记录成品入库单明细表.物料编码=基础数据物料信息表.物料编码
                                         left join 生产记录生产工单表 on  生产记录生产工单表.生产工单号= 生产记录成品入库单明细表.生产工单号
                                        left join 仓库物料数量表 on    仓库物料数量表.物料编码=生产记录生产工单表.物料编码
                                        where  生产记录成品入库单明细表.入库车间='{0}' and 生产记录成品入库单明细表.生效日期>='{1}' and  生产记录成品入库单明细表.作废=0
                                          and 生产记录成品入库单明细表.生效日期<='{2}' ", dt_生产关系.Rows[0]["生产车间"]
                                                         , barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1));
            }
            else
            {
                sql_已入库工单 = string.Format(@"select 生产记录成品入库单明细表.*,生产制令单号,基础数据物料信息表.大类,基础数据物料信息表.标准单价,基础数据物料信息表.原ERP物料编号,生产记录生产工单表.工单负责人,库存总数 from 生产记录成品入库单明细表
                                          left join  基础数据物料信息表      on   生产记录成品入库单明细表.物料编码=基础数据物料信息表.物料编码
                                                left join 生产记录生产工单表 on  生产记录生产工单表.生产工单号= 生产记录成品入库单明细表.生产工单号
                                             left join 仓库物料数量表 on    仓库物料数量表.物料编码=生产记录生产工单表.物料编码                                               
                                            where  生产记录成品入库单明细表.生效日期>='{0}' and  生产记录成品入库单明细表.作废=0
                                                and 生产记录成品入库单明细表.生效日期<='{1}' "
                        , barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1));
            }
            dt_已入库工单 = CZMaster.MasterSQL.Get_DataTable(sql_已入库工单, strcon);



            gridControl5.DataSource = dt_已入库工单;
            //求总合格率




        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_全部工单()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql_工单 = "";
            if (dt_生产关系.Rows.Count > 0 && dt_生产关系.Rows[0]["生产车间"].ToString() != "")
            {

                sql_工单 = string.Format(@"select 生产记录生产工单表.*,基础数据物料信息表.标准单价,基础数据物料信息表.原ERP物料编号 from 生产记录生产工单表,基础数据物料信息表 
                                      where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码 and 生产记录生产工单表.生产车间='{0}'and 生产记录生产工单表.关闭=0
                                      and 生产记录生产工单表.生效日期>='{1}' and  生产记录生产工单表.生效日期<='{2}' "
                    , barEditItem3.EditValue, barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1));
            }
            else
            {

                sql_工单 = string.Format(@"select 生产记录生产工单表.*,基础数据物料信息表.标准单价,基础数据物料信息表.原ERP物料编号 from 生产记录生产工单表,基础数据物料信息表 
                                         where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码 and 生产记录生产工单表.关闭=0
                                         and 生产记录生产工单表.生效日期>='{0}' and  生产记录生产工单表.生效日期<='{1}' ", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1));
            }
            dt_全部工单 = CZMaster.MasterSQL.Get_DataTable(sql_工单, strcon);

            gridControl1.DataSource = dt_全部工单;

        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_已检验未入库()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql_已检验未入库 = "";
            if (dt_生产关系.Rows.Count > 0 && dt_生产关系.Rows[0]["生产车间"].ToString() != "")
            {
                sql_已检验未入库 = string.Format(@"select 生产记录生产工单表.*,生产检验单号,已入库数量,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.n原ERP规格型号,生产记录生产检验单主表.包装确认,库存总数  from 生产记录生产工单表,基础数据物料信息表,生产记录生产检验单主表,仓库物料数量表  
                                                 where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码 and 仓库物料数量表.物料编码=生产记录生产工单表.物料编码 and 生产记录生产工单表.生产工单号= 生产记录生产检验单主表.生产工单号
                                                and  生产记录生产检验单主表.完成=0   and  生产记录生产工单表.关闭=0  and   生产记录生产检验单主表.生效=1 and 生产记录生产工单表.完成=0 
                                                and 检验日期 >= '{0}' and 检验日期 <= '{1}'  and  生产记录生产工单表.生产车间='{2}'",
                                                                                  barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1), dt_生产关系.Rows[0]["生产车间"]);
            }
            else
            {
                sql_已检验未入库 = string.Format(@" select 生产记录生产工单表.*,生产检验单号,已入库数量,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.n原ERP规格型号,生产记录生产检验单主表.包装确认,库存总数  from 生产记录生产工单表,基础数据物料信息表,生产记录生产检验单主表,仓库物料数量表  
                                                where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码 and 仓库物料数量表.物料编码=生产记录生产工单表.物料编码
                                                and 生产记录生产工单表.生产工单号= 生产记录生产检验单主表.生产工单号  and 生产记录生产检验单主表.完成=0 and  生产记录生产工单表.关闭=0 and 生产记录生产检验单主表.生效=1 and 生产记录生产工单表.完成=0 
                                                and 检验日期 >= '{0}' and 检验日期 <= '{1}' ", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1));
            }
            dt_已检验未入库 = CZMaster.MasterSQL.Get_DataTable(sql_已检验未入库, strcon);

            gridControl6.DataSource = dt_已检验未入库;

        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_在产工单()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql_在产工单 = "";
            if (dt_生产关系.Rows.Count > 0 && dt_生产关系.Rows[0]["生产车间"].ToString() != "")
            {
                sql_在产工单 = string.Format(@"select 生产记录生产工单表.*,基础数据物料信息表.原ERP物料编号  from 生产记录生产工单表,基础数据物料信息表 
                                        where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码 and  生产记录生产工单表.生效=1 
                                        and 生产记录生产工单表.完工=0 and 生产记录生产工单表.关闭=0
                                       and 生产记录生产工单表.生产车间='{0}' and 生产记录生产工单表.生效日期<='{1}' ",
                                      dt_生产关系.Rows[0]["生产车间"], Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1));
                //and  生产记录生产工单表.生产工单号 not in(select 生产工单号 from [生产记录成品入库单明细表])
            }
            else
            {
                sql_在产工单 = string.Format(@"select 生产记录生产工单表.*,基础数据物料信息表.原ERP物料编号  from 生产记录生产工单表,基础数据物料信息表 
                                        where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码 and  生产记录生产工单表.生效=1 and 生产记录生产工单表.关闭=0
                                        and 生产记录生产工单表.完工=0 and 生产记录生产工单表.生效日期<='{0}' ",
                        Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1));
            }
            dt_在产工单数 = CZMaster.MasterSQL.Get_DataTable(sql_在产工单, strcon);

            gridControl3.DataSource = dt_在产工单数;

        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_完工未检验()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql_完工未检验 = "";
            if (dt_生产关系.Rows.Count > 0 && dt_生产关系.Rows[0]["生产车间"].ToString() != "")
            {
                sql_完工未检验 = string.Format(@"select 生产记录生产工单表.*,基础数据物料信息表.原ERP物料编号  from 生产记录生产工单表,基础数据物料信息表 
                                    where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码 and 生产记录生产工单表.生效=1 and 生产记录生产工单表.完工=1  and 生产记录生产工单表.关闭=0
                                  and 生产记录生产工单表.生产车间='{0}'  and 检验完成=0
                                and 生产记录生产工单表.完工日期>='{1}' and  生产记录生产工单表.完工日期<='{2}'", dt_生产关系.Rows[0]["生产车间"]
                                                   , barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1));
            }
            else
            {
                sql_完工未检验 = string.Format(@"select 生产记录生产工单表.*,基础数据物料信息表.原ERP物料编号  from 生产记录生产工单表,基础数据物料信息表 
                           where  基础数据物料信息表.物料编码=生产记录生产工单表.物料编码 and 生产记录生产工单表.生效=1 and 生产记录生产工单表.完工=1  and 生产记录生产工单表.关闭=0
                        and 检验完成=0    and 生产记录生产工单表.完工日期>='{0}' and  生产记录生产工单表.完工日期<='{1}'", barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).Date .AddDays(1).AddSeconds(-1));
            }

            dt_完工未检验 = CZMaster.MasterSQL.Get_DataTable(sql_完工未检验, strcon);

            gridControl4.DataSource = dt_完工未检验;

        }
        //查询
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_全部工单();
            fun_未开工工单();
            fun_已检验未入库();
            fun_已入库工单();
            fun_在产工单();
            fun_完工未检验();
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView8_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView5_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView3_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void barEditItem3_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataView dv_全部工单 = new DataView(dt_全部工单);
            DataView dv_未开工工单 = new DataView(dt_未开工工单);
            DataView dv_已检验未入库 = new DataView(dt_已检验未入库);
            DataView dv_已入库工单 = new DataView(dt_已入库工单);
            DataView dv_在产工单数 = new DataView(dt_在产工单数);
            DataView dv_完工未检验 = new DataView(dt_完工未检验);


            dv_全部工单.RowFilter = string.Format("生产车间='{0}'", barEditItem3.EditValue);
            dv_未开工工单.RowFilter = string.Format("生产车间='{0}'", barEditItem3.EditValue);
            dv_已检验未入库.RowFilter = string.Format("生产车间='{0}'", barEditItem3.EditValue);
            dv_已入库工单.RowFilter = string.Format("入库车间='{0}'", barEditItem3.EditValue);
            dv_在产工单数.RowFilter = string.Format("生产车间='{0}'", barEditItem3.EditValue);
            dv_完工未检验.RowFilter = string.Format("生产车间='{0}'", barEditItem3.EditValue);

            gridControl3.DataSource = dv_在产工单数;
            gridControl6.DataSource = dv_已检验未入库;
            gridControl1.DataSource = dv_全部工单;
            gridControl2.DataSource = dv_未开工工单;
            gridControl5.DataSource = dv_已入库工单;
            gridControl4.DataSource = dv_完工未检验;

        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView4_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            if (ActiveControl.GetType().Equals(gridControl1.GetType()))
            {

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    //gridControl1.ExportToXls(saveFileDialog.FileName, options);  


                    DevExpress.XtraGrid.GridControl gc = (ActiveControl) as DevExpress.XtraGrid.GridControl;

                    gc.ExportToXlsx(saveFileDialog.FileName);

               

                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            else
            {

                MessageBox.Show("若要导出请先选中要导出的表格");
            }
        }

  






    }
}
