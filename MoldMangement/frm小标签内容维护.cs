using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MoldMangement
{
    public partial class frm小标签内容维护 : UserControl
    {
        string name = "";
        DataTable dt_物料信息;
        DataTable dt_内容维护;
        CurrencyManager cmM;
        DataRow dr_当前行;
        public frm小标签内容维护()
        {
            InitializeComponent();
        }

        private void fun_SetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                new SqlCommandBuilder(da);
                da.Update(dt);
            }
        }

        private void fun_GetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                da.Fill(dt);
            }
        }

        private void frm小标签内容维护_Load(object sender, EventArgs e)
        {
            try
            {
                this.gv1.IndicatorWidth = 40;
                name = CPublic.Var.LocalUserID;
                dt_物料信息 = new DataTable();
                string sql = string.Format(@"select 物料编码,原ERP物料编号,物料名称,规格型号,图纸编号,n原ERP规格型号 from 基础数据物料信息表 
                    where 基础数据物料信息表.车间编号 in (select 课室编号 from 人事基础员工表 where 人事基础员工表.员工号 = '{0}')", name);
                fun_GetDataTable(dt_物料信息, sql);
                gc1.DataSource = dt_物料信息;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void gv1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                dt_内容维护 = new DataTable();
                dr_当前行 = gv1.GetDataRow(gv1.FocusedRowHandle);
                string sql = string.Format("select * from 小标签维护内容表 where 物料编码 = '{0}'", dr_当前行["物料编码"].ToString());
                fun_GetDataTable(dt_内容维护, sql);
                gc2.DataSource = dt_内容维护;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dt_内容维护.Rows.Count == 0)
                {
                    DataRow dr = dt_内容维护.NewRow();
                    dr["物料编码"] = dr_当前行["物料编码"];
                    dr["内容"] = "";
                    dt_内容维护.Rows.Add(dr);
                }
                else
                {
                    MessageBox.Show("一条记录只能关联一条内容！");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            cmM = BindingContext[dt_内容维护] as CurrencyManager;
            cmM.EndCurrentEdit();
            gv2.CloseEditor();
            try
            {
                (cmM.Current as DataRowView).Row.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = string.Format("select * from 小标签维护内容表 where 1<>1");
                fun_SetDataTable(dt_内容维护, sql);
                MessageBox.Show("保存成功！");
                gv1_RowCellClick(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            } 
        }
    }
}
