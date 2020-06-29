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
    public partial class frm成品检验检验不合格原因维护 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM = null;
        string str_产品 = "";
        #endregion

        #region 自用类
        public frm成品检验检验不合格原因维护()
        {
            InitializeComponent();
        }

        private void frm成品检验检验不合格原因维护_Load(object sender, EventArgs e)
        {
            try
            {
                bar_大类.EditValue = "";
                label1.Text = "";
                fun_载入下拉框();
                //gv.ShownEditor += gv_ShownEditor;
                //gc.EditorKeyUp += gc_EditorKeyUp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //private void gc_EditorKeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter && (gv.ActiveEditor is DevExpress.XtraEditors.MemoEdit))
        //    {
        //        gv.CloseEditor();
        //        gv.RefreshData();
        //        gv.ShowEditor();
        //    }
        //}

        //private void gv_ShownEditor(object sender, EventArgs e)
        //{
        //    if (gv.ActiveEditor is DevExpress.XtraEditors.MemoEdit)
        //    {
        //        DevExpress.XtraEditors.MemoEdit me = gv.ActiveEditor as DevExpress.XtraEditors.MemoEdit;
        //        try
        //        {
        //            me.SelectionStart = me.Text.Length;
        //        }
        //        catch
        //        {
        //        }
        //    }
        //}

        #endregion

        #region 方法
        private void fun_载入下拉框()
        {
            string sql = string.Format("select * from 基础数据基础属性表 where 属性类别 = '{0}'", "成检错误代码");
            DataTable dt_属性 = new DataTable();
            SqlDataAdapter da_属性 = new SqlDataAdapter(sql, strconn);
            da_属性.Fill(dt_属性);
            if (dt_属性.Rows.Count > 0)
            {
                foreach (DataRow r in dt_属性.Rows)
                {
                    ((DevExpress.XtraEditors.Repository.RepositoryItemComboBox)this.bar_大类.Edit).Items.Add(r["属性值"].ToString());
                }
            }
        }

        private void fun_载入产品类型(string str_类型)
        {
            string sql = string.Format("select * from ZZ_FGYY where dl = '{0}'", str_类型);
            dtM = new DataTable();
            SqlDataAdapter daM = new SqlDataAdapter(sql, strconn);
            daM.Fill(dtM);
            gc.DataSource = dtM;
        }

        private void fun_保存()
        {
            string sql = "select * from ZZ_FGYY where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dtM);
        }
        #endregion

        #region 界面操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                str_产品 = bar_大类.EditValue.ToString();
                if (str_产品 != "")
                {
                    fun_载入产品类型(str_产品);
                    label1.Text = string.Format("当前的大类为：{0}", str_产品);
                }
                else
                {
                    string sql = string.Format("select * from ZZ_FGYY");
                    dtM = new DataTable();
                    SqlDataAdapter daM = new SqlDataAdapter(sql, strconn);
                    daM.Fill(dtM);
                    gc.DataSource = dtM;
                
                }
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
                if (bar_大类.EditValue.ToString()=="")
             {
                 throw new Exception("请先选择大类！");
             }
               if(gv.RowCount == 0)
               {
                   throw new Exception("请先选择刷新按钮！");
               }
                DataRow drM = dtM.NewRow();
                dtM.Rows.Add(drM);
                drM["dl"] = str_产品;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                if (dtM != null)
                {
                    fun_保存();
                }
                MessageBox.Show("保存成功");
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
        //导出
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;

                ERPorg.Corg.TableToExcel(dtM, saveFileDialog.FileName);


                DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }



        }
    }
}
