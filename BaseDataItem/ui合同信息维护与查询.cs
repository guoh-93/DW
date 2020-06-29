using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using CZMaster;
namespace BaseData
{
    public partial class ui合同信息维护与查询 : UserControl
    {
        public ui合同信息维护与查询()
        {
            InitializeComponent();
        }

        private void gc_Click(object sender, EventArgs e)
        {

        }//上传


        #region 成员
        DataTable dtM;               //主表
        SqlDataAdapter da;
        DataView dv;
        string strconn = CPublic.Var.strConn;   
        DataTable dt_客户;
        string strcon_FS = CPublic.Var.geConn("FS");
        //  strConn_FS
        DataTable dt_合同子表;
        #endregion

        #region 方法
        private void fun_读取数据(string str)
        {
            try
            {
                dtM = new DataTable();
                //dtM.Clear();
                string sql = string.Format("select * from 客户签订合同表 where 客户编号 = '{0}'", str);
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                dv = new DataView(dtM);  //只显示有效合同
                dv.RowFilter = "合同状态 = '有效'";
                gc.DataSource = dv;

                dt_合同子表 = new DataTable();
                sql = "select * from 客户合同文件表 where 1<>1";
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_合同子表);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_读取数据()
        {
            try
            {
                dtM = new DataTable();
                //dtM.Clear();
                string sql = string.Format("select * from 客户签订合同表");
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                dv = new DataView(dtM);  //只显示有效合同
                dv.RowFilter = "合同状态 = '有效'";
                gc.DataSource = dv;

                dt_合同子表 = new DataTable();
                sql = "select * from 客户合同文件表 where 1<>1";
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_合同子表);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void fun_客户编号()
        {
            dt_客户 = new DataTable();
            string sql = string.Format("SELECT *  FROM 客户基础信息表 ");
            //string strconn2 = "Persist Security Info=True;User ID=sa;Password=a;Initial Catalog=asasasas;Data Source=.";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                try
                {
                    da.Fill(dt_客户);
                    repositoryItemSearchLookUpEdit3.DataSource = dt_客户;
                    repositoryItemSearchLookUpEdit3.ValueMember = "客户编号";
                    repositoryItemSearchLookUpEdit3.DisplayMember = "客户编号";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void fun_合同类型()
        {
            DataTable dt = new DataTable();
            string sql_1 = "SELECT 属性值 FROM 基础数据基础属性表 where 属性类别 = '合同类型'";
            //string strconn2 = "Persist Security Info=True;User ID=sa;Password=a;Initial Catalog=asasasas;Data Source=.";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_1, strconn))
            {
                try
                {
                    da.Fill(dt);
                    repositoryItemSearchLookUpEdit2.DataSource = dt;
                    repositoryItemSearchLookUpEdit2.ValueMember = "属性值";
                    repositoryItemSearchLookUpEdit2.DisplayMember = "属性值";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        private void ui合同信息维护与查询_Load(object sender, EventArgs e)
        {
            fun_读取数据();
            fun_客户编号();
            fun_合同类型();

        }



        private void fun_load()
        {
            string sql = "select * from 客户签订合同表";
            DataTable dt_合同 = CZMaster.MasterSQL.Get_DataTable(sql,strconn);
            gc.DataSource = dt_合同;


        }

        private void 上传_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog open = new OpenFileDialog();
                if (open.ShowDialog() == DialogResult.OK)
                {
                    FileInfo info = new FileInfo(open.FileName);      //判定上传文件的大小
                    //long maxlength = info.Length;
                    //if (maxlength > 1024 * 1024 * 8)
                    //{
                    //    throw new Exception("上传的文件不能超过1M，请重新选择后再上传！");
                    //}
                    MasterFileService.strWSDL = CPublic.Var.strWSConn;
                    CFileTransmission.CFileClient.strCONN = strcon_FS;
                    string strguid = "";  //记录系统自动返回的GUID
                    strguid = CFileTransmission.CFileClient.sendFile(open.FileName);
                    DataRow rm = (this.BindingContext[gc.DataSource].Current as DataRowView).Row;
                    if (rm.RowState == DataRowState.Added)
                        throw new Exception("该记录是新增的合同信息，尚未保存！请先保存后在上传合同！");
                    rm["合同文件GUID"] = strguid;
                    rm["合同文件"] = Path.GetFileName(open.FileName);
                    //存放合同的历史版本
                    DataRow r_htzb = dt_合同子表.NewRow();
                    r_htzb["客户GUID"] = rm["GUID"];
                    r_htzb["合同GUID"] = strguid;
                    r_htzb["合同名称"] = Path.GetFileName(open.FileName);
                    r_htzb["上传时间"] = System.DateTime.Now;
                    dt_合同子表.Rows.Add(r_htzb);
                    MasterSQL.Save_DataTable(dt_合同子表, "客户合同文件表", strconn);
                    MasterSQL.Save_DataTable(dtM, "客户签订合同表", strconn);
                    MessageBox.Show("合同上传成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 下载_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow rm = (this.BindingContext[gc.DataSource].Current as DataRowView).Row;
                SaveFileDialog save = new SaveFileDialog();
                save.FileName = rm["合同文件"].ToString();
                if (save.ShowDialog() == DialogResult.OK)
                {
                    CFileTransmission.CFileClient.strCONN = strcon_FS;
                    CFileTransmission.CFileClient.Receiver(rm["合同文件GUID"].ToString(), save.FileName);
                    MessageBox.Show("合同下载成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 预览_Click(object sender, EventArgs e)
        {
            try
            {
                string dir = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\tmpview";
                //如果该文件夹存在的话，删除。
                try
                {
                    System.IO.Directory.Delete(dir, true);
                }
                catch
                {
                }
                //删除之后进行新增
                try
                {
                    System.IO.Directory.CreateDirectory(dir);
                }
                catch
                {

                }
                DataRow rm = (this.BindingContext[gc.DataSource].Current as DataRowView).Row;
                CFileTransmission.CFileClient.strCONN = strcon_FS;
                CFileTransmission.CFileClient.Receiver(rm["合同文件GUID"].ToString(), dir + "\\" + rm["合同文件"].ToString());
                System.Diagnostics.Process.Start(dir + "\\" + rm["合同文件"].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow r = dtM.NewRow();
                r["合同状态"] = "有效";
                r["合同份数"] = 1;
                //  r["客户编号"] = str_客户编号;
                //    r["客户名称"] = dt_客户.Rows[0]["客户名称"].ToString();
                dtM.Rows.Add(r);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }//新增

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[dv].EndCurrentEdit();
                string sql = "select * from 客户签订合同表 where 1<>1";
                SqlDataAdapter daa = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(daa);
                DateTime t = CPublic.Var.getDatetime();
                foreach (DataRow r in dtM.Rows)
                {
                    if (r["合同名称"].ToString() == "")
                    {
                        throw new Exception("请填写完整数据");
                    }
                    if (r.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    if (r["合同号"].ToString() == "")
                    {
                        string a = string.Format("DQJR{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("DQJR", t.Year));
                        r["合同号"] = a;
                    }
                    if (r["GUID"].ToString() == "")
                    {
                        r["GUID"] = System.Guid.NewGuid();
                    }
                }
                daa.Update(dtM);
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }//保存

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认删除该条记录吗？", "询问！！", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DataRow r = gv.GetDataRow(gv.FocusedRowHandle);
                    //r["合同状态"] = "废除";
                    r.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "客户编号")
            {
                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                DataRow drM = (this.BindingContext[gc.DataSource].Current as DataRowView).Row;
                DataRow[] dr = dt_客户.Select(string.Format("客户编号='{0}'", drM["客户编号"].ToString()));
                drM["客户名称"] = dr[0]["客户名称"].ToString();

            }

        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            ui销售订单与合同关系维护 frm = new ui销售订单与合同关系维护(dr);

            CPublic.UIcontrol.Showpage(frm, "销售订单与合同关系维护");

        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            form合同文件上传 frm = new form合同文件上传(dr);
            frm.ShowDialog();

        }
    
    
    
    
    
    }
}
