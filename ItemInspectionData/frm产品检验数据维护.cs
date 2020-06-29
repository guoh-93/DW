using CPublic;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraTab;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Windows.Forms;


namespace ItemInspectionData
{
    public partial class frm产品检验数据维护 : UserControl
    {
        #region 用户变量

        private DataTable dtP1, dtP2, dtM1, dtM2, dtM3, dtM4, dtM5;//检验要求表,检验要求表副本,编码表,检验项目,检验水平,AQL,检验要求表
        private CurrencyManager cmM;
        public static string str = "";
        public static bool f_Change = true;
        string strconn2 = CPublic.Var.geConn("DW");
        string strconn = CPublic.Var.strConn;
        //string strconn2 = "Password=dwDZ@123;Persist Security Info=True;User ID=sa;Initial Catalog=UFDATA_995_2018;Data Source=192.168.20.150;Pooling=true;Max Pool Size=40000;Min Pool Size=0";
        #endregion 用户变量

        #region 类自用

        public frm产品检验数据维护()
        {
            InitializeComponent();
            //DataTable temp = new DataTable();
            //temp.Columns.Add("dll全称");
            //temp.Columns.Add("窗体显示名称");
            //temp.Columns.Add("命名空间.窗体名称");
            //temp.TableName = "自定义菜单";
            //temp.ReadXml(Path.Combine(Application.StartupPath, @"自定义菜单.xml"));
            //fun_生成菜单(temp, XTC, barM, bar2);

        }

        private void frm检验数据维护_Load(object sender, EventArgs e)
        {

            //fun_readData();
            fillCMD();
            gvM.ShownEditor += gvM_ShownEditor;

            gcM.EditorKeyUp += gcM_EditorKeyUp;

            //if (str != "")
            //{
            //    DataRow[] ds = dtM1.Select(string.Format("cInvCode = '{0}'", str));
            //    this.searchLookUpEdit1.Text = ds[0]["cInvCode"].ToString();
            //    DataTableToUITop();
            //    DataTableToUIFoot();
            //}"
            string sql = "select * from  基础数据物料检验要求表 where 1<>1";
            dtP1 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtP1);
            gcM.DataSource = dtP1;
            
            txtTSFH.Text = "±ΦR~，≤≥（）∞+ -°";
        }

        private void gcM_EditorKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && (gvM.ActiveEditor is DevExpress.XtraEditors.MemoEdit))
            {
                gvM.CloseEditor();
                gvM.RefreshData();
                gvM.ShowEditor();
            }
        }

        private void gvM_ShownEditor(object sender, EventArgs e)
        {
            if (gvM.ActiveEditor is DevExpress.XtraEditors.MemoEdit)
            {
                DevExpress.XtraEditors.MemoEdit me = gvM.ActiveEditor as DevExpress.XtraEditors.MemoEdit;
                try
                {
                    me.SelectionStart = me.Text.Length;
                }
                catch
                {
                }
            }
        }

        #endregion 类自用

        #region 数据库操作

        ///// <summary>
        ///// 读
        ///// </summary>
        //private void fun_readData()
        //{
        //    dtP1 = new DataTable();
        //    string sql = "SELECT * FROM [工作用临时数据库].[dbo].[基础数据物料检验要求表]";
        //    using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
        //    {
        //        try
        //        {
        //            da.Fill(dtP1);
        //            gcM.DataSource = dtP1;
        //            cmM = this.BindingContext[dtP1] as CurrencyManager;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }
        //}
        private void fillCMD()
        {
            dtM1 = new DataTable();
            dtM5 = new DataTable();
             string sql_1 = "select 物料编码,物料名称,规格型号,计量单位 from 基础数据物料信息表 where 停用 = 0";

            //string sql_1 = "select * from Inventory where bPurchase ='true'";//存货表中拉取外购数据 
            //using (SqlDataAdapter da = new SqlDataAdapter(sql_1, CPublic.Var.geConn("WL")))
            using (SqlDataAdapter da = new SqlDataAdapter(sql_1, strconn))
            {
                try
                {
                    da.Fill(dtM1);
                    searchLookUpEdit1.Properties.DataSource = dtM1;
                    searchLookUpEdit1.Properties.ValueMember = "物料编码";
                    searchLookUpEdit1.Properties.DisplayMember = "物料编码";

                    //da.Fill(dtM5);
                    searchLookUpEdit2.Properties.DataSource = dtM1;
                    searchLookUpEdit2.Properties.ValueMember = "物料编码";
                    searchLookUpEdit2.Properties.DisplayMember = "物料编码";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            dtM2 = new DataTable();
            string sql_2 = "SELECT  [POS] ,[检验项目] FROM [基础数据物料检验项目表] order by [POS]";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_2, CPublic.Var.strConn))
            {
                try
                {
                    da.Fill(dtM2);
                    repositoryItemSearchLookUpEdit1.DataSource = dtM2;
                    repositoryItemSearchLookUpEdit1.ValueMember = "检验项目";
                    repositoryItemSearchLookUpEdit1.DisplayMember = "检验项目";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            dtM3 = new DataTable();
            string sql_3 = "SELECT  [POS],[检验水平]FROM [基础数据检验水平项表] order by [POS] ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_3, CPublic.Var.strConn))
            {
                try
                {
                    da.Fill(dtM3);
                    repositoryItemSearchLookUpEdit2.DataSource = dtM3;
                    repositoryItemSearchLookUpEdit2.ValueMember = "检验水平";
                    repositoryItemSearchLookUpEdit2.DisplayMember = "检验水平";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            dtM4 = new DataTable();
            string sql_4 = "SELECT [POS] ,[AQL] FROM [基础数据AQL表] ORDER BY POS";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_4, CPublic.Var.strConn))
            {
                try
                {
                    da.Fill(dtM4);
                    repositoryItemSearchLookUpEdit3.DataSource = dtM4;
                    repositoryItemSearchLookUpEdit3.ValueMember = "AQL";
                    repositoryItemSearchLookUpEdit3.DisplayMember = "AQL";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 写
        /// </summary>
        private void fun_saveData(DataTable dt)
        {
            string sql = "SELECT * FROM [基础数据物料检验要求表]";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                new SqlCommandBuilder(da);
                try
                {
                    da.Update(dt);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        private void fun_save()
        {
            //foreach (DataRow r in dtP2.Rows)
            //{
            //    r.Delete();
            //}
            //fun_saveData(dtP2);
            //DataTable dt = dtP1.Clone();
            //foreach (DataRow r in dtP1.Rows)
            //{
            //    if (r.RowState == DataRowState.Deleted)
            //    {
            //        continue;
            //    }
            //    dt.Rows.Add(r.ItemArray);
            //}
          // fun_saveData(dt);
            foreach(DataRow dr in dtP1.Rows)
            {
                if(dr.RowState == DataRowState.Deleted)
                {
                    continue;
                }
                dr["产品编码"] = searchLookUpEdit1.EditValue;
            }
            string sql = "select * from 基础数据物料检验要求表 where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dtP1);
            MessageBox.Show("保存成功");
        }

        #endregion 数据库操作

        #region 数据处理

        /// <summary>
        /// UI上部赋值
        /// </summary>
        private void DataTableToUITop()
        {
            DataSet ds = new DataSet();
            try
            {
               // string sql = "select * from Inventory where cInvCode ='" + str + "'";
                string sql = "select * from 基础数据物料信息表 where 物料编码 = '" + str + "'";
                using (SqlConnection conn = new SqlConnection(strconn))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                dataBindHelper1.DataFormDR(ds.Tables[0].Rows[0]);
                //textBox1.Text = ds.Tables[0].Rows[0].ToString();
            }
            catch
            {
                
            }
        }

        /// <summary>
        /// UI下部赋值
        /// </summary>
        /// <param name="strCode"></param>
        private void DataTableToUIFoot()
        {
            try
            {
                dtP1 = new DataTable();
                dtP2 = new DataTable();
                string sql = "select * from   [基础数据物料检验要求表] where [产品编码]='" + str + "' order by [POS] ";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
                {
                    try
                    {
                        da.Fill(dtP1);
                        da.Fill(dtP2);

                         gcM.DataSource = dtP1;
                        cmM = this.BindingContext[dtP1] as CurrencyManager;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                dtP1.TableNewRow += dtP1_TableNewRow;
            }
            catch 
            {
                
            }
        }

        private void dtP1_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            e.Row["产品编码"] = str;
            e.Row["下限值"] = "";
            e.Row["上限值"] = "";
            e.Row["POS"] = "0";
        }

        /// <summary>
        /// 新增行
        /// </summary>
        private void fun_AddData()
        {
            try
            {
                DataRow dr = dtP1.NewRow();

                dtP1.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除行
        /// </summary>
        private void fun_DeleteData()
        {
            try
            {
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                dr.Delete();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 上移
        /// </summary>
        private void fun_DataUp()
        {
            //DataRow dr = (cmM.Current as DataRowView).Row;
            //DataTable dtP1_New = dtP1.Clone();
            //DataRow dr_Up = dtP1_New.NewRow();
            //int m = dtP1.Rows.IndexOf(dr);
            //m--;
            //while (m>-1)
            //{
            //    if (dtP1.Rows[m].RowState != DataRowState.Deleted)
            //    {
            //        dr_Up.ItemArray = dtP1.Rows[m].ItemArray;
            //        dtP1.Rows[m].ItemArray = dr.ItemArray;
            //        dr.ItemArray = dr_Up.ItemArray;
            //        break;
            //    }
            //    m--;
            //}

            //DataRow dr = (cmM.Current as DataRowView).Row;
            //DataTable dtP1_New = dtP1.Clone();
            //DataRow dr_Up = dtP1_New.NewRow();
            //int index_cmM = dtP1.Rows.IndexOf(dr);
            //int index_up = index_cmM ;
            //index_cmM--;
            //while (index_cmM > -1)
            //{
            //    if (dtP1.Rows[index_cmM].RowState != DataRowState.Deleted)
            //    {
            //        dr_Up.ItemArray = dtP1.Rows[index_cmM].ItemArray;
            //        dtP1.Rows[index_cmM].Delete();
            //        DataRow r = dtP1.NewRow();
            //        r.ItemArray = dr_Up.ItemArray;
            //        dtP1.Rows.InsertAt(r, index_up);
            //        break;
            //    }
            //    index_cmM--;
            //}

            DataRow dr = (cmM.Current as DataRowView).Row;
            int index_cmM = dtP1.Rows.IndexOf(dr);
            int index_up = index_cmM;
            index_cmM--;
            while (index_cmM > -1)
            {
                if (dtP1.Rows[index_cmM].RowState != DataRowState.Deleted)
                {
                    DataRow r = dtP1.NewRow();
                    r.ItemArray = dtP1.Rows[index_cmM].ItemArray;
                    dtP1.Rows[index_cmM].Delete();
                    dtP1.Rows.InsertAt(r, index_up);
                    break;
                }
                index_cmM--;
            }
        }

        /// <summary>
        /// 下移
        /// </summary>
        private void fun_DataDown()
        {
            DataRow dr = (cmM.Current as DataRowView).Row;
            int index_cmM = dtP1.Rows.IndexOf(dr);
            int index_Down = index_cmM;
            index_cmM++;
            while (index_cmM < dtP1.Rows.Count)
            {
                if (dtP1.Rows[index_cmM].RowState != DataRowState.Deleted)
                {
                    DataRow r = dtP1.NewRow();
                    r.ItemArray = dtP1.Rows[index_cmM].ItemArray;
                    dtP1.Rows[index_cmM].Delete();
                    dtP1.Rows.InsertAt(r, index_Down);
                    break;
                }
                index_cmM++;
            }
        }

        /// <summary>
        /// 保存前校验
        /// </summary>
        private void fun_Check()
        {
            try
            {
                foreach (DataRow r in dtP1.Rows)
                {
                    if (r.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    if (r["下限值"].ToString() == "" && r["上限值"].ToString() == "")
                    {
                        r["下限值"] = "";
                        r["上限值"] = "";
                    }

                    if (r["下限值"].ToString() != "" && r["上限值"].ToString() == "")
                    {
                        throw new Exception("上限值不能为空");
                    }

                    if (r["下限值"].ToString() == "" && r["上限值"].ToString() != "")
                    {
                        throw new Exception("下限值不能为空");
                    }
                    r["检验要求"] = r["检验要求"].ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 排序
        /// </summary>
        private void fun_Sort()
        {
            try
            {
                int count = dtP1.Rows.Count;
                for (int n = 1, i = 0; n <= count; n++, i++)
                {
                    if (dtP1.Rows[i].RowState == DataRowState.Deleted)
                    {
                        n--;
                        count--;
                        continue;
                    }
                    dtP1.Rows[i]["POS"] = n;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 复制
        /// </summary>
        private void fun_Copy(string str_Copy)
        {
            try
            {
                dtP1.Clear();
                string sql = "select * from  [基础数据物料检验要求表] where [产品编码]='" + str_Copy + "' order by [POS] ";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
                {
                    try
                    {
                        da.Fill(dtP1);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                foreach (DataRow r in dtP1.Rows)
                {
                    r["产品编码"] = str;
                }
                dtP1.TableNewRow += dtP1_TableNewRow;
                dtP1.AcceptChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion 数据处理

        #region 界面相关

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ////if (str != "")
                ////{
                ////    DataTableToUITop();
                ////    DataTableToUIFoot();
                ////}
                ////else
                //{
                //    str = this.searchLookUpEdit1.Text.Trim();
                //    DataTableToUITop();
                //    DataTableToUIFoot();
                //}
                DataRow[] ds = dtM1.Select(string.Format("物料编码 = '{0}'", searchLookUpEdit1.EditValue));
                if (ds.Length > 0)
                {
                    textBox1.Text = ds[0]["计量单位"].ToString();
                    textBox2.Text = ds[0]["规格型号"].ToString();
                    textBox3.Text = ds[0]["物料名称"].ToString();
                }
                string sql = string.Format("select * from  基础数据物料检验要求表 where 产品编码 = '{0}'", searchLookUpEdit1.EditValue);
                dtP1 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtP1);
                gcM.DataSource = dtP1;




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.Text.ToString() == "")
                {
                    throw new Exception("请先选择需要维护检验明细的物料！");
                }
             

                gvM.CloseEditor();
                gvM.UpdateCurrentRow();

                fun_Check();
                fun_Sort();
                fun_save();
              //  fun_DWsave();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                
            }
            //MessageBox.Show("OK");
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                CPublic.UIcontrol.ClosePage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
               
                    fun_AddData();
                    //gvM.CloseEditor();
                    //cmM.EndCurrentEdit();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.Text.ToString() != "")
                {
                    fun_DeleteData();
                    //fun_Sort();
                    //gvM.CloseEditor();
                    //cmM.EndCurrentEdit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 下移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            fun_DataUp();
            //fun_Sort();
            gvM.CloseEditor();
            cmM.EndCurrentEdit();
        }

        /// <summary>
        /// 上移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            fun_DataDown();
            //fun_Sort();
            gvM.CloseEditor();
            cmM.EndCurrentEdit();
        }

        /// <summary>
        /// 筛选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            DataRow r = searchLookUpEdit2.Properties.View.GetFocusedDataRow();
            this.txtCopy.Text = r["物料编码"].ToString();
        }

        /// <summary>
        /// 复制检验
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton5_Click(object sender, EventArgs e)
        {
            if (str == "")
            {
                MessageBox.Show("请选择目标！");
                return;
            }
            if (txtCopy.Text.Trim() == "")
            {
                MessageBox.Show("请选择来源！");
                return;
            }
            gvM.CloseEditor();
            cmM.EndCurrentEdit();
            try
            {
                fun_Copy(txtCopy.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion 界面相关

        #region 特殊符号

        ///±ΦR，≤≥（）∞+ -°

        /// <summary>
        /// ±
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_Clipboard("±");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Φ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                fun_Clipboard("Φ");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// R
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                fun_Clipboard("R");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// ，
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                fun_Clipboard("，");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// ≤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                fun_Clipboard("≤");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// ≥
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                fun_Clipboard("≥");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// （）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                fun_Clipboard("（）");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// ∞
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                fun_Clipboard("∞");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// +
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                fun_Clipboard("+");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                fun_Clipboard("-");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// °
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                fun_Clipboard("°");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void fun_Clipboard(string str)
        {
            Clipboard.Clear();//清空剪切板内容 
            Clipboard.SetData(DataFormats.Text, str);//复制内容到剪切板
        }
        #endregion

        #region 保存dw 存货数据 到基础数据表
        DataTable dt_dw数据 = new DataTable();
        //private void fun_DWsave()
        //{
        //    string str_dw = "select * from 基础数据物料信息表 where 物料编码 ='" + searchLookUpEdit1.EditValue.ToString() + "' ";
        //    DataTable dt_判断基础物料表是否存在物料 = new DataTable();
        //    using (SqlDataAdapter da = new SqlDataAdapter(str_dw, CPublic.Var.strConn))
        //    {

        //        da.Fill(dt_判断基础物料表是否存在物料);

        //    }
        //    using (SqlDataAdapter da2 = new SqlDataAdapter("select * from Inventory where cInvCode ='" + searchLookUpEdit1.EditValue.ToString() + "' ", strconn2))
        //    {

        //        da2.Fill(dt_dw数据);
        //    }

        //    if (dt_判断基础物料表是否存在物料.Rows.Count == 0)
        //    {
        //        using (SqlDataAdapter da1 = new SqlDataAdapter("select * from 基础数据物料信息表 where 1<>1", CPublic.Var.strConn))
        //        {
        //            DataTable dt_保存数据 = new DataTable();
        //            da1.Fill(dt_保存数据);
        //            DataRow drr = dt_保存数据.NewRow();
        //            dt_保存数据.Rows.Add(drr);
        //            drr["物料编码"] = dt_dw数据.Rows[0]["cInvCode"].ToString();
        //            drr["原ERP物料编号"] = dt_dw数据.Rows[0]["cInvCode"].ToString();
        //            drr["物料名称"] = dt_dw数据.Rows[0]["cInvName"].ToString();
        //            drr["规格型号"] = dt_dw数据.Rows[0]["cInvStd"].ToString();
        //            drr["大类"] = dt_dw数据.Rows[0]["cInvCCode"].ToString();
        //            drr["供应商编号"] = dt_dw数据.Rows[0]["cVenCode"].ToString();
        //            if (dt_dw数据.Rows[0]["iInvRCost"].ToString() != "")
        //            {
        //                drr["标准单价"] = Convert.ToDecimal(dt_dw数据.Rows[0]["iInvRCost"]);
        //            }
        //            if (dt_dw数据.Rows[0]["iTopSum"].ToString() != "")
        //            {
        //                drr["库存上限"] = Convert.ToDouble(dt_dw数据.Rows[0]["iTopSum"]);
        //            }
        //            if (dt_dw数据.Rows[0]["iLowSum"].ToString() != "")
        //            {
        //                drr["库存上限"] = Convert.ToDouble(dt_dw数据.Rows[0]["iLowSum"]);
        //            }
        //            if (dt_dw数据.Rows[0]["iInvWeight"].ToString() != "")
        //            {
        //                drr["克重"] = Convert.ToDecimal(dt_dw数据.Rows[0]["iInvWeight"]);
        //            }
        //            if (dt_dw数据.Rows[0]["dSDate"].ToString() != "")
        //            {
        //                drr["生效时间"] = dt_dw数据.Rows[0]["dSDate"].ToString();
        //            }

        //            drr["仓库号"] = dt_dw数据.Rows[0]["cDefWareHouse"].ToString();
        //            new SqlCommandBuilder(da1);
        //            da1.Update(dt_保存数据);
        //            for (int i = dt_dw数据.Rows.Count - 1; i >= 0; i--)
        //            {
        //                dt_dw数据.Rows.RemoveAt(i);
        //            }

        //        }


        //    }

        //}


        #endregion
      


      
        private void LoadInUserControl(DataRow drr)
        {
            try
            {
             
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, "BaseDataItem.dll"));//dr["dll全路径"] = "动态载入dll.dll"
                Type outerForm = outerAsm.GetType("BaseData.ui蓝图维护", false);//动态载入dll.UI动态载入窗体
                object[] dr = new object[1];
                dr[0] = drr;
 
                UserControl ui = Activator.CreateInstance(outerForm, dr) as UserControl;
                if (!(ui == null))
                {

                    CPublic.UIcontrol.Showpage(ui, "蓝图维护");

                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //上传蓝图
        DataTable dt_can;
        private void barLargeButtonItem7_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.Text.ToString()=="")
                {
                    throw new Exception("请先选择需要上传蓝图的物料！");
                }
                //XTC.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.InActiveTabPageHeader;
                //UIcontrol.XTC = this.XTC;
                string strr = "select * from 基础数据物料信息表 where 物料编码 ='" + searchLookUpEdit1.EditValue + "'";
                DataTable dt_物料 = CZMaster.MasterSQL.Get_DataTable(strr, strconn);
                DataRow dr = dt_物料.NewRow();
                dr = dt_物料.Rows[0];
                LoadInUserControl(dr);
                //using (SqlDataAdapter da = new SqlDataAdapter(strr, strconn))
                //{
                //    DataTable dt_can = new DataTable();
                //    da.Fill(dt_can);
                //    DataRow dr = dt_can.NewRow();

                //    if (dt_can.Rows.Count ==0)
                //    {
                        

                //    }

                //    dt_can.Columns["物料号"].ColumnName = "物料编码";
                //    dt_can.Columns.Add("物料名称", typeof(string));
                //    dt_can.Columns.Add("规格型号", typeof(string));
                //    dt_can.Rows[0]["物料名称"] = textBox3.Text;
                //    dt_can.Rows[0]["规格型号"] = textBox2.Text;


                //    //dt_can.Columns["cInvName"].ColumnName = "物料名称";
                //    //dt_can.Columns["cInvStd"].ColumnName = "规格型号";

                //    dr = dt_can.Rows[0];

                //    //DataRow[] dr1 = dt_can.Select(string.Format("clnvCode = '{0}'",searchLookUpEdit1.EditValue+"'"));



                //    LoadInUserControl(dr);
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

      




    }
}