using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPpurchase
{
    public partial class ui开发出库 : UserControl
    {


        #region
        string str_出库号;
        string strcon = CPublic.Var.strConn;
        DataTable dtM = new DataTable();
        DataTable dt_下拉;
        #endregion
        public ui开发出库()
        {
            InitializeComponent();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_load_dtm();
                fun_load_下拉框();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void fun_load_dtm()
        {
            string sql = @"select 开发仓库数量表.*,n原ERP规格型号 from 开发仓库数量表,基础数据物料信息表 
                         where 库存总数>0 and  开发仓库数量表.物料编码=基础数据物料信息表.物料编码 and 1<>1";
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;
            dtM.Columns.Add("领料数量");

        }
        private void fun_load_下拉框()
        {
            string sql = @"select 开发仓库数量表.物料编码,开发仓库数量表.原ERP物料编号,开发仓库数量表.物料名称,开发仓库数量表.图纸编号,开发仓库数量表.规格型号
                        ,开发仓库数量表.出入库时间,开发仓库数量表.库存总数,n原ERP规格型号 from 开发仓库数量表,基础数据物料信息表
                         where 库存总数>0 and  开发仓库数量表.物料编码=基础数据物料信息表.物料编码";

            dt_下拉 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            repositoryItemSearchLookUpEdit1.DataSource = dt_下拉;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
        }
        private void fun_check()
        {
            foreach (DataRow dr in dtM.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                try
                {
                    decimal a = Convert.ToDecimal(dr["领料数量"]);

                    if (a <= 0)
                    {
                        throw new Exception(string.Format("物料'{0}'的领料数量不可小于0", dr["原ERP物料编号"]));

                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(string.Format("物料'{0}'的领料数量输入不正确", dr["原ERP物料编号"]));
                }

            }
        }
        private void fun_save()//出库记录 DA
        {
            DateTime t = CPublic.Var.getDatetime();
            string ss = t.Year.ToString().Substring(2,2);
            str_出库号 = string.Format("DA{0}{1:D2}{2:00}{3:000}", ss, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("DA", t.Year, t.Month));
            int POS=1  ;

            string sql = "select * from 仓库出入库明细表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dtM.Rows)
                {
                 
                            DataRow r = dt.NewRow();
                            r["GUID"] = System.Guid.NewGuid();
                            r["明细类型"] = "开发出库";
                            r["单号"] = str_出库号;
                            r["出库入库"] = "开发出库";
                            r["物料编码"] = dr["物料编码"];
                            r["物料名称"] = dr["物料名称"];

                            r["明细号"] = str_出库号 + POS.ToString("00");
                            r["实效数量"] = -(Convert.ToDecimal(dr["领料数量"]));
                            r["实效时间"] = t;
                            r["出入库时间"] = t;

                            string sql_pd = "select * from 仓库物料盘点表 where 有效=1";
                            using (SqlDataAdapter da1 = new SqlDataAdapter(sql_pd, strcon))
                            {

                                DataTable dt_批次号 = new DataTable();
                                da1.Fill(dt_批次号);
                                if (dt_批次号.Rows.Count > 0)
                                {
                                    r["盘点有效批次号"] = dt_批次号.Rows[0]["盘点批次号"];
                                }
                                else
                                {
                                    r["盘点有效批次号"] = "初始化";
                                }
                            }
                            dt.Rows.Add(r);

                            POS++;
                     }

                new SqlCommandBuilder(da);
                da.Update(dt);   
              

            }
        }
        private void fun_减少库存()
        {
            foreach (DataRow dr in dtM.Rows)
            {

                string sql = string.Format(@"update  [开发仓库数量表] set 库存总数=库存总数-{0},出入库时间='{1}' 
                                                where 物料编码='{2}'", dr["领料数量"], System.DateTime.Now, dr["物料编码"]);
                CZMaster.MasterSQL.ExecuteSQL(sql, strcon);

            }
        }
        private void repositoryItemSearchLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (e.NewValue == null)
            {
                DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                r = dtM.NewRow();
            }
            else if (e.NewValue.ToString() != "")
            {
                DataRow[] dr = dt_下拉.Select(string.Format("物料编码='{0}'", e.NewValue));
                if (dr.Length > 0)
                {
                    DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                    r["物料名称"] = dr[0]["物料名称"];
                    r["规格型号"] = dr[0]["规格型号"];
                    r["n原ERP规格型号"] = dr[0]["n原ERP规格型号"];
                    r["图纸编号"] = dr[0]["图纸编号"];
                    r["原ERP物料编号"] = dr[0]["原ERP物料编号"];
                    r["库存总数"] = dr[0]["库存总数"];
                }
            }
            else
            {
                MessageBox.Show("数据有误");
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DataRow dr =dtM.NewRow();
            dtM.Rows.Add(dr);
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            r.Delete();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        //生效
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                fun_check();
                fun_save();
                fun_减少库存();
                MessageBox.Show("ok");
                barLargeButtonItem1_ItemClick(null, null);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message);
                MessageBox.Show("生效出错");
            }
        }

        private void ui开发出库_Load(object sender, EventArgs e)
        {
            try
            {
                fun_load_dtm();
                fun_load_下拉框();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
