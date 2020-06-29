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
    public partial class ui包装方式 : UserControl
    {

        string strconn = CPublic.Var.strConn;
        DataTable dt_包装方式;
        DataTable dt_物料编码;
        DataTable dtM = new DataTable();
        public ui包装方式()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void ui包装方式_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_load();
                fun_load1();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
        //关闭
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult a = MessageBox.Show("确定要关闭当前窗口吗?", "关闭系统", messButton);
            if (a == DialogResult.OK)
            {
                CPublic.UIcontrol.ClosePage();
            }
            else
            {
                return;
            }
        }


#pragma warning disable IDE1006 // 命名样式
        private void fun_load1()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = "select 物料编码,物料名称,大类,小类,规格型号,产品线,物料类型 from 基础数据物料信息表";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    dt_物料编码 = new DataTable();
                    da.Fill(dt_物料编码);
                    //searchLookUpEdit1.Properties.DataSource = dt_产品编码;
                    //searchLookUpEdit1.Properties.ValueMember = "物料编码";
                    //searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                    //searchLookUpEdit1View.PopulateColumns();
                    repositoryItemSearchLookUpEdit1.DataSource = dt_物料编码;
                    repositoryItemSearchLookUpEdit1.DisplayMember = "物料名称";
                   repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = "select 属性值 as 包装方式,属性字段1,POS from 基础数据基础属性表 where 属性类别 = '包装方式'";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    DataTable dt = new DataTable();

                    dt_包装方式 = new DataTable();
                    da.Fill(dt_包装方式);

                    //if (!dt_包装方式.Columns.Contains("包装方式"))
                    //{
                    //    dt_包装方式.Columns.Add("包装方式");
                    //}
                    //if (!dt_包装方式.Columns.Contains("属性字段1"))
                    //{
                    //    dt_包装方式.Columns.Add("属性字段1");
                    //}
                    //foreach (DataRow dr in dt.Rows)
                    //{
                    //    DataRow r = dt_包装方式.NewRow();
                    //    dt_包装方式.Rows.Add(r);
                    //    r["包装方式"] = dr["属性值"];
                    //    r["属性字段1"] = dr["属性字段1"];
                    //}
                    //repositoryItemSearchLookUpEdit1.DataSource = dt_包装方式;
                    //repositoryItemSearchLookUpEdit1.ValueMember = "包装方式";
                    //repositoryItemSearchLookUpEdit1.DisplayMember = "包装方式";
                    searchLookUpEdit1.Properties.DataSource = dt_包装方式;
                    searchLookUpEdit1.Properties.ValueMember = "POS";
                    searchLookUpEdit1.Properties.DisplayMember = "包装方式";
                  //  searchLookUpEdit1View.PopulateColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //新增
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_check();
                DataRow dr = dtM.NewRow();
                dr["包装方式"] = searchLookUpEdit1.Text.ToString();
                dtM.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (MessageBox.Show("确认删除吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {


                    if (gv.RowCount > 0)
                    {
                        DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);

                        dr.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                dtM.Clear();

                ///DataRow sr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow);

                string sqlqwe = String.Format("select 属性值 as 包装方式,属性字段1,POS from 基础数据基础属性表 where 属性类别 = '包装方式' and POS = '{0}'", searchLookUpEdit1.EditValue.ToString());
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sqlqwe,strconn);
                
                


                if(dt.Rows.Count>0){
                    textBox2.Text = dt.Rows[0]["属性字段1"].ToString();
                }
                textBox1.Text = searchLookUpEdit1.EditValue.ToString();
                string sql = string.Format("select * from 包装方式表 where 编号 = '{0}'", searchLookUpEdit1.EditValue.ToString());
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                //if (dtM.Rows.Count > 0)
                //{
                //    string dr = dtM.Rows[0]["箱装数"].ToString();
                //    textBox1.Text = dr;
                //}
               
                gc.DataSource = dtM;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (e.Column.FieldName == "物料编码")
                {
                    DataRow dr_当前行 = gv.GetDataRow(gv.FocusedRowHandle);
                    string str = e.Value.ToString();
                    DataRow[] dr = dt_物料编码.Select(string.Format("物料编码='{0}'", str));
                    if (dr != null && dr.Length > 0)
                    {
                        DataRow row = dr[0];
                        dr_当前行["物料编码"] = row["物料编码"].ToString();
                        dr_当前行["包装材料名称"] = row["物料名称"].ToString();
                        dr_当前行["规格型号"] = row["规格型号"].ToString();
                    //    dr_当前行["包材数量"] = dr_当前行["包材数量"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            searchLookUpEdit1.EditValue = "";
            //textBox1.Text = "";
            dtM.Clear();
        }
        //保存
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                fun_check();

                fun_save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_save()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select * from 包装方式表 where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);


            // sql = string.Format("select 属性值,属性字段1,POS from 基础数据基础属性表 where 属性类别 = '包装方式'",searchLookUpEdit1.Text.ToString());
            // DataTable dtg = new DataTable();
            // dtg = CZMaster.MasterSQL.Get_DataTable(sql,strconn);
            ////string a="";
            //if(dtg.Rows.Count>0){
            //    a = dtg.Rows[0]["POS"].ToString();
            //}

            foreach(DataRow dr in dtM.Rows ){

                if (dr.RowState == DataRowState.Deleted)
                {

                    continue;
                }
                dr["编号"] = textBox1.Text;
                if(dr["包材数量"].ToString()==""||dr["单位数量"].ToString()==""){
                    throw new Exception("数量不可为空值");
                }

            //    dr["箱装数"] = textBox1.Text.ToString();

            }
            da.Update(dtM);
            MessageBox.Show("保存成功");
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {
            if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
            {
                throw new Exception("请选择包装方式");
            }
            foreach (DataRow dr in dtM.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;

                DataRow[] r = dtM.Select(string.Format("物料编码='{0}'", dr["物料编码"].ToString()));
                if (r.Length > 1)
                {
                    throw new Exception(string.Format("选择了重复物料{0},请确认", dr["产品编码"]));
                }
            }
        }





    }
}
