using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using CZMaster;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace BaseData
{
    public partial class frm客户签订的合同 : UserControl
    {
        #region 成员
        DataTable dtM;               //主表
        SqlDataAdapter da;
        DataView dv;     
        string strconn = CPublic.Var.strConn;
        string str_客户编号 = "";
        string strWSDL = "";
        DataTable dt_客户;
        string strcon_FS = CPublic.Var.geConn("FS");
      //  strConn_FS
        DataTable dt_合同子表;
        #endregion

        #region 自用类
        public frm客户签订的合同()
        {
            InitializeComponent();
            //fun_读取数据();
        }

        public frm客户签订的合同(string str)
        {
            InitializeComponent();
            str_客户编号 = str;
         //   fun_读取数据();
        }


        private void frm客户签订的合同_Load(object sender, EventArgs e)
        {
            // fun_客户编号();
            //  fun_读取数据();
            fun_读取数据();
            dtM = new DataTable();
            //dtM.Clear();
            string sql = string.Format("select * from 客户签订合同表 where 合同状态 = '有效' ");
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
      ;
            gc.DataSource = dtM;

            dt_合同子表 = new DataTable();
            sql = "select * from 客户合同文件表 where 1<>1";
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_合同子表);

            fun_合同类型();
        }
        #endregion

        #region 方法
        private void fun_读取数据()
        {
            try
            {
                string sql = @"select base.物料编码,base.物料名称,base.规格型号 ,计量单位编码,计量单位,            
                isnull(a.库存总数,0)库存总数,base.货架描述,a.仓库号,a.仓库名称 from 仓库物料数量表 a
                 left join 基础数据物料信息表 base on a.物料编码 = base.物料编码 ";
                DataTable dt_wuliao = CZMaster.MasterSQL.Get_DataTable(sql,strconn);

                repositoryItemLookUpEdit1.DataSource = dt_wuliao;

                // repositoryItemSearchLookUpEdit1View.PopulateColumns();

                repositoryItemLookUpEdit1.DisplayMember = "物料编码";
                repositoryItemLookUpEdit1.ValueMember = "物料编码";



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //private void fun_读取数据()
        //{
        //    try
        //    {
        //        dtM = new DataTable();
        //        //dtM.Clear();
        //        string sql = string.Format("select * from 客户签订合同表");
        //        da = new SqlDataAdapter(sql, strconn);
        //        da.Fill(dtM);
        //        dv = new DataView(dtM);  //只显示有效合同
        //        dv.RowFilter = "合同状态 = '有效'";
        //        gc.DataSource = dv;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //public void fun_客户编号()
        //{
        //    dt_客户 = new DataTable();
        //    string sql = string.Format("SELECT * FROM 客户基础信息表 where 客户编号 = '{0}'", str_客户编号);
        //    //string strconn2 = "Persist Security Info=True;User ID=sa;Password=a;Initial Catalog=asasasas;Data Source=.";
        //    using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
        //    {
        //        try
        //        {
        //            da.Fill(dt_客户);
        //            //repositoryItemSearchLookUpEdit1.DataSource = dt;
        //            //repositoryItemSearchLookUpEdit1.ValueMember = "客户编号";
        //            //repositoryItemSearchLookUpEdit1.DisplayMember = "客户编号";
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }
        //}

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

        #region 界面操作
        //新增
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                dtM = new DataTable();
        
                string sql = string.Format("select * from 客户签订合同表 where 1<>1 ");
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                gc.DataSource = dtM;

                DataRow r = dtM.NewRow();
                //r["合同状态"] = "有效";
                //r["客户编号"] = str_客户编号;
                //r["客户名称"] = dt_客户.Rows[0]["客户名称"].ToString();
                dtM.Rows.Add(r);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //删除
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        //保存
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                        string a = string.Format("DQJR{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("DQJR",t.Year));
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
        }

        //所有合同
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                gc.DataSource = dtM;
            }   
            else
            {
                gc.DataSource = dv;
            }
        }

        //合同上传
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

        //合同下载
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

        //合同预览
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

        #endregion

        private void gc_Click(object sender, EventArgs e)
        {

        }
    }
}
