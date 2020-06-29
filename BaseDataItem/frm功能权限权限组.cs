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
    public partial class frm功能权限权限组 : UserControl
    {
        #region 成员
        DataTable dtM = new DataTable();
        DataRow drM;
        SqlDataAdapter da;
        //int i = 0;
        string strshow;
        string strconn = CPublic.Var.strConn;
        //string strconn = "Persist Security Info=True;User ID=MESSA;Password=MESSA;Initial Catalog=ERPDB;Data Source=218.244.150.177";
        #endregion

        #region 自用
        public frm功能权限权限组()
        {
            InitializeComponent();
        }

        private void frm功能权限权限组_Load(object sender, EventArgs e)
        {
            string sql = "select * from 功能权限权限组表";
            da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Fill(dtM);
            gc.DataSource = dtM;
            dtM.Columns.Add("原权限组");
            foreach (DataRow dr in dtM.Rows )
            {
                dr["原权限组"] = dr["权限组"];

            }
            gv.Columns[0].OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            gv.Columns[1].OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            //gv.Columns[2].OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
        }
        #endregion

        #region 数据操作
        public void fun_新增()
        {
            drM = dtM.NewRow();
            dtM.Rows.Add(drM);
        }

        public void fun_删除()
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                dr.Delete();
                //strshow = "！";
            }
            catch
            {
                //strshow = "0";
            }
        }

        public void fun_保存()
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                try
                {
                    string s = "";
                    foreach(DataRow r in dtM.Rows)
                    {
                        if (r.RowState == DataRowState.Deleted)
                        {
                            continue;
                        }
                        if (r["GUID"].ToString() == "")
                        {
                            r["GUID"] = System.Guid.NewGuid();
                        }
                        if (r["原权限组"].ToString()!=null&& r["原权限组"].ToString()!="")
                        {
                            if (r["原权限组"].ToString() != r["权限组"].ToString().Trim())
                            {
                                //此处方便起见了 
                                s = s + string.Format(@"update 功能权限权限组权限表 set 权限组='{1}'   where  权限组='{0}'
                                update 人事基础员工表 set 权限组 = '{1}' where 权限组 = '{0}'", r["原权限组"].ToString(), r["权限组"]);
                            }
                        }
                    }
                    string sql = "select * from 功能权限权限组表 where 1<>1 ";
                    da = new SqlDataAdapter(sql, strconn);
                    new SqlCommandBuilder(da);
                    da.Update(dtM);
                    CZMaster.MasterSQL.ExecuteSQL(s, strconn);
                    strshow = "保存成功！";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


                //i = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 界面操作

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //新增
            fun_新增();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //删除
            if (MessageBox.Show("确定要删除该数据吗？", "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                //gv.CloseEditor();
                fun_删除();
                // MessageBox.Show(strshow);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //保存
            fun_保存();
            MessageBox.Show(strshow);
        }
        #endregion

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
    }
}
