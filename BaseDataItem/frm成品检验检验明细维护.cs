using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace BaseData
{
    public partial class frm成品检验检验明细维护 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM = null;
        DataTable dtP = null;
        string str_产品类型 = "";

        string cpbh = ""; //新增单品是 需要用
        #endregion

        #region 自用类
        public frm成品检验检验明细维护()
        {
            InitializeComponent();
        }

        private void frm成品检验检验明细维护_Load(object sender, EventArgs e)
        {
            label1.Text = "";
            fun_载入代办();
            gv.ShownEditor += gv_ShownEditor;
            gc.EditorKeyUp += gc_EditorKeyUp;
        }

        private void gc_EditorKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && (gv.ActiveEditor is DevExpress.XtraEditors.MemoEdit))
            {
                gv.CloseEditor();
                gv.RefreshData();
                gv.ShowEditor();
            }
        }

        private void gv_ShownEditor(object sender, EventArgs e)
        {
            if (gv.ActiveEditor is DevExpress.XtraEditors.MemoEdit)
            {
                DevExpress.XtraEditors.MemoEdit me = gv.ActiveEditor as DevExpress.XtraEditors.MemoEdit;
                try
                {
                    me.SelectionStart = me.Text.Length;
                }
                catch
                {
                }
            }
        }

        private void gvP_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            //代办
            try
            {
                DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);
                if (dr != null)
                {
                    str_产品类型 = dr["dl"].ToString();
                    cpbh = dr["cpbh"].ToString();
                    if (str_产品类型 != "")
                    {
                        fun_载入产品类型(str_产品类型);
                        label1.Text = string.Format("当前的产品或类型为：{0}", str_产品类型);
                    }
                }
 
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region 方法
        private void fun_载入代办()
        {
            //string sql = string.Format("select 物料编码,物料名称,规格型号,规格 from 基础数据物料信息表 where 物料类型 = '{0}'", "成品");
            string sql = string.Format("  select dl,cpbh from zz_jyxm group by dl,cpbh");
            dtP = new DataTable();
            SqlDataAdapter da_属性 = new SqlDataAdapter(sql, strconn);
            da_属性.Fill(dtP);
            //        ((DevExpress.XtraEditors.Repository.RepositoryItemComboBox)this.bar_产品类型.Edit).Items.Add(r["属性值"].ToString());
            gcP.DataSource = dtP;
        }

        private void fun_载入产品类型(string str_类型)
        {
            string sql = string.Format("select * from ZZ_JYXM where dl = '{0}'", str_类型);
            dtM = new DataTable();
            SqlDataAdapter daM = new SqlDataAdapter(sql, strconn);
            daM.Fill(dtM);
            gc.DataSource = dtM;
        }

        private void fun_保存()
        {
            DataView dv = new DataView(dtM);
            dv.Sort = "xh";
            int i = 1;
            foreach (DataRowView drv in dv)
            {
                DataRow r = drv.Row;
                r["xh"] = i++;
            }
            string sql = "select * from ZZ_JYXM where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dtM);
        }
        #endregion

        #region 界面操作
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            cpbh = "";
            frm新增成品检验类型 frm = new frm新增成品检验类型();
            frm.ShowDialog();
            if (frm.flag)
            {
                DataRow dr = dtP.NewRow();
                dr["dl"] = frm.ss[1];
                if(frm.ss[0]=="单品")
                {
                   dr["cpbh"] = frm.ss[2];
                }
              
                dtP.Rows.Add(dr);
                gvP.FocusedRowHandle = gvP.LocateByDisplayText(0, gridColumn6, frm.ss[1]);
                gvP_RowCellClick(null, null);
            }

        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);
                
                if (MessageBox.Show(string.Format("是否确认删除'{0}'？", dr["dl"].ToString().Trim()), "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    dr.Delete();
                    string sql = string.Format("delete [ZZ_JYXM] where dl='{0}'", dr["dl"].ToString());
                    CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
                    MessageBox.Show("已删除。。。");
                    fun_载入代办();
                }

            }
            catch (Exception)
            {
                
                throw;
            }
         
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();

                if (dtM != null)
                {
                    fun_保存();
                    MessageBox.Show("保存成功");
                }
                else
                {
                    MessageBox.Show("无任何检验明细");
                }
                fun_载入代办();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DataRow dr = null;
            try
            {
                dr = gv.GetDataRow(gv.FocusedRowHandle);
            }
            catch { }
            if (dr != null)
            {
                if (MessageBox.Show("确定要删除该数据吗？", "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    dr.Delete();
                }
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DataRow drM = dtM.NewRow();
            dtM.Rows.Add(drM);
            drM["dl"] = str_产品类型;
            drM["cpbh"] = cpbh;
        

        }


    }
}
