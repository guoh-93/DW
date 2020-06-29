using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace IAACA
{
    public partial class ui_软件费用维护界面 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        string cfgfilepath = "";
        public ui_软件费用维护界面()
        {
            InitializeComponent();
        }

        private void ui_软件费用维护界面_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(this.panel2, this.Name, cfgfilepath);
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //维护加载数据 
        private void fun_load()
        {
            string s_条件 = "";
            if (checkBox1.Checked)
            {
                s_条件 = @"union 
 select  xx.*,单价 from (select  a.物料编码,b.物料名称 ,b.规格型号 from 产品软件对应表 a
 left join 基础数据物料信息表 b on a.物料编码=b.物料编码 group by a.物料编码,b.规格型号,b.物料名称)xx
 left join [2019财务软件费用] c on c.产品编码 =xx.物料编码";
            }
            else
            {
                s_条件 = " and c.单价 is null";
            }
            //所有需要显示得物料编码  
            string s = $@"select aa.*,c.单价 from (
 select   a.物料编码,b.物料名称,b.规格型号 from 生产记录成品入库单明细表  a
 left join 基础数据物料信息表 b on a.物料编码=b.物料编码 where b.大类 not in ('展架类')  group by a.物料编码,b.规格型号,b.物料名称)aa 
 left join [2019财务软件费用] c on c.产品编码 =aa.物料编码 
 where left(物料编码,2)=10  {s_条件} ";

            dtM = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            DataColumn[] pkm = new DataColumn[1];
            pkm[0] = dtM.Columns["物料编码"];
            dtM.PrimaryKey = pkm;
            //所有列
            s = "select  * from [软件单价基础表]";
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            foreach (DataRow dr in t.Rows)
            {
                DataColumn dc = new DataColumn(dr["软件名称"].ToString(), typeof(bool));
                dc.DefaultValue = false;
                dtM.Columns.Add(dc);
            }
            s = "select  * from 产品软件对应表 "; //所有得对应关系
            DataTable t_all = CZMaster.MasterSQL.Get_DataTable(s, strconn);
    
            foreach (DataRow r in dtM.Rows)
            {
                DataRow[] L_R = t_all.Select($"物料编码='{r["物料编码"].ToString()}'");
                foreach (DataRow rr in L_R)
                {
                    r[rr["软件名称"].ToString()] = true;
                }
            }
            dtM.AcceptChanges();
            gridControl2.DataSource = dtM;
            gridView2.PopulateColumns();
            gridView2.Columns["物料编码"].BestFit();
            gridView2.Columns["物料名称"].BestFit();
            gridView2.Columns["规格型号"].BestFit();
          

            if(CPublic.Var.LocalUserTeam.Contains("财务") || CPublic.Var.LocalUserTeam.Contains("管理员") || CPublic.Var.LocalUserID=="admin")
            {
                gridView2.Columns["单价"].Visible = true;
            }
            else
            {
                gridView2.Columns["单价"].Visible = false;
            }
        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_load();
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
                this.ActiveControl = null;
                fun_save();
                MessageBox.Show("保存成功");
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_save()
        {
            DataView dv = new DataView(dtM);
            dv.RowStateFilter = DataViewRowState.ModifiedCurrent | DataViewRowState.Added | DataViewRowState.Deleted;
            DataTable dtcopy = dv.ToTable();

            SqlConnection conn = new SqlConnection(strconn);
            SqlCommand cmd = new SqlCommand();
            cmd = conn.CreateCommand();
            //cmd.CommandText = "select * from [2019财务软件费用] where 1=2";
        
            //DataTable dt_save = new DataTable();
            //da.Fill(dt_save);

            //int x = dt_save.Rows.Count;
            cmd.CommandText = "select * from 产品软件对应表";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable t_all = new DataTable();
            da.Fill(t_all);
            //所有列
            cmd.CommandText = "select  * from [软件单价基础表]";
            DataTable t_列 = new DataTable();
            da.Fill(t_列);
            cmd.CommandText = "  select  * from [2019财务软件费用]  where 1=2";
            DataTable t_软件单价 = new DataTable();
            da.Fill(t_软件单价);

            cmd.CommandText = "select * from [软件工时修改日志表] where 1=2";
            DataTable dt_修改日志 = new DataTable();
            da.Fill(dt_修改日志);
            DateTime time = CPublic.Var.getDatetime();
            int xc = t_软件单价.Rows.Count;
            foreach (DataRow dr in dtcopy.Rows)
            {
                decimal dec_单价 = 0;
                foreach (DataRow r_列 in  t_列.Rows)
                {
                    if(Convert.ToBoolean(dr[r_列["软件名称"].ToString()])) //若用户保存时打了勾
                    {
                        DataRow[] r_original = t_all.Select($"物料编码='{dr["物料编码"].ToString()}' and 软件名称='{r_列["软件名称"].ToString()}'");
                        if(r_original.Length==0)
                        {
                            DataRow r = t_all.NewRow();
                            r["GUID"] = System.Guid.NewGuid();
                            r["物料编码"] = dr["物料编码"].ToString();
                            r["软件名称"] = r_列["软件名称"].ToString();
                            t_all.Rows.Add(r);
                        }
                        dec_单价 += Convert.ToDecimal(r_列["单价"]);
                    }
                    else  //没打勾
                    {
                        DataRow[] r_original = t_all.Select($"物料编码='{dr["物料编码"].ToString()}' and 软件名称='{r_列["软件名称"].ToString()}'");
                        if (r_original.Length >0)
                        {
                            r_original[0].Delete(); //程序正确应只有一条
                        }
                    }
                }
                dr["单价"] = dec_单价;
                cmd.CommandText = $"select  * from [2019财务软件费用]  where 产品编码='{dr["物料编码"].ToString()}'";
                da.Fill(t_软件单价);
   
                if (xc == t_软件单价.Rows.Count) //表中没有  新增
                {
                    if (Convert.ToDecimal(dr["单价"]) == 0) continue;

                    DataRow r = t_软件单价.NewRow();
                    r["产品编码"] = dr["物料编码"].ToString();
                    r["单价"] =  dec_单价 ;
                    t_软件单价.Rows.Add(r);
                    //增加修改日志
                    DataRow rr = dt_修改日志.NewRow();
                    rr["GUID"] = System.Guid.NewGuid();
                    rr["类型"] = "软件费用";
                    rr["产品编码"] = dr["物料编码"].ToString();
                    rr["原始值"] = "";
                    rr["更新值"] = dr["单价"].ToString();
                    rr["修改人"] = CPublic.Var.localUserName;
                    rr["修改时间"] = time;
                    dt_修改日志.Rows.Add(rr);
                    xc++;
                }
                else
                {
                    t_软件单价.Rows[t_软件单价.Rows.Count - 1]["单价"] = dec_单价 ;
                    //增加修改日志
                    DataRow rr = dt_修改日志.NewRow();
                    rr["GUID"] = System.Guid.NewGuid();
                    rr["类型"] = "软件费用";
                    rr["产品编码"] = dr["物料编码"].ToString();
                    rr["原始值"] = dr["原始值"].ToString();
                    rr["更新值"] = dr["单价"].ToString();
                    rr["修改人"] = CPublic.Var.localUserName;
                    rr["修改时间"] = time;
                    rr["原因"] = dr["原因"].ToString();
                    dt_修改日志.Rows.Add(rr);
                }
            }

            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("软件费用维护");
            try
            {
                string ss = "select * from[2019财务软件费用] where 1 = 2";
                SqlCommand cmm = new SqlCommand(ss, conn, ts);
                SqlDataAdapter da_cun = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da_cun);
                da_cun.Update(t_软件单价);

                ss = "select * from [软件工时修改日志表] where 1=2";
                cmm = new SqlCommand(ss, conn, ts);
                da_cun = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da_cun);
                da_cun.Update(dt_修改日志);

                ss = "select * from [产品软件对应表] where 1=2";
                cmm = new SqlCommand(ss, conn, ts);
                da_cun = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da_cun);
                da_cun.Update(t_all);

                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw new Exception("保存失败");
            }


        }



        /*
        private void fun_save()
        {
            DataView dv = new DataView(dtM);
            dv.RowStateFilter = DataViewRowState.ModifiedCurrent | DataViewRowState.Added | DataViewRowState.Deleted;
            DataTable dtcopy = dv.ToTable();
            SqlConnection conn = new SqlConnection(strconn);
            SqlCommand cmd = new SqlCommand();
            cmd = conn.CreateCommand();
            cmd.CommandText = "select * from [2019财务软件费用] where 1=2";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt_save = new DataTable();
            da.Fill(dt_save);
            cmd.CommandText = "select * from [软件工时修改日志表] where 1=2";
            DataTable dt_软件工时 = new DataTable();
            da.Fill(dt_软件工时);
            int x = dt_save.Rows.Count;
            DateTime time = CPublic.Var.getDatetime();
            foreach (DataRow dr in dtcopy.Rows)
            {
                cmd.CommandText = $"select * from [2019财务软件费用] where 产品编码='{dr["物料编码"].ToString()}' ";
                da.Fill(dt_save);
                if (x == dt_save.Rows.Count) //表中没有  新增
                {
                    DataRow r = dt_save.NewRow();
                    r["产品编码"] = dr["物料编码"].ToString();
                    r["单价"] = dr["单价"].ToString();
                    dt_save.Rows.Add(r);
                    //增加修改日志

                    DataRow rr = dt_软件工时.NewRow();
                    rr["GUID"] = System.Guid.NewGuid();
                    rr["类型"] = "软件费用";
                    rr["产品编码"] = dr["物料编码"].ToString();

                    rr["原始值"] = "";
                    rr["更新值"] = dr["单价"].ToString();
                    rr["修改人"] = CPublic.Var.localUserName;
                    rr["修改时间"] = time;

                    dt_软件工时.Rows.Add(rr);

                    x++;
                }
                else
                {
                    dt_save.Rows[dt_save.Rows.Count - 1]["单价"] = dr["单价"].ToString();

      
                   
                    //增加修改日志

                    DataRow rr = dt_软件工时.NewRow();
                    rr["GUID"] = System.Guid.NewGuid();
                    rr["类型"] = "软件费用";
                    rr["产品编码"] = dr["物料编码"].ToString();
                    rr["原始值"] = dr["原始值"].ToString();
                    rr["更新值"] = dr["单价"].ToString();
                    rr["修改人"] = CPublic.Var.localUserName;
                    rr["修改时间"] = time;
                    rr["原因"] = dr["原因"].ToString();
                    dt_软件工时.Rows.Add(rr);
                }
            }
            string ss = "select * from [2019财务软件费用] where 1=2";
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("软件费用维护");
            try
            {
                SqlCommand cmm = new SqlCommand(ss, conn, ts);
                SqlDataAdapter da_cun = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da_cun);
                da_cun.Update(dt_save);

                ss = "select * from [软件工时修改日志表] where 1=2";
                cmm = new SqlCommand(ss, conn, ts);
                da_cun = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da_cun);
                da_cun.Update(dt_软件工时);
                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw new Exception("保存失败");
            }
        }*/

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("是否确认退出当前界面", "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                CPublic.UIcontrol.ClosePage();
            }
        }

        private void gridView2_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                //if (dr["单价", DataRowVersion.Original] == null || dr["单价", DataRowVersion.Original].ToString() == "")
                //{
                //    //gridColumn8.OptionsColumn.AllowEdit = true; //没有工时直接录入 
                //}
                //else
                //{
                //    //gridColumn8.OptionsColumn.AllowEdit = false; //原来有 需要右键修改 

                //}
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                //if (dr["单价", DataRowVersion.Original] == null || dr["单价", DataRowVersion.Original].ToString() == "")
                //{
                //   // gridColumn8.OptionsColumn.AllowEdit = true; //没有工时直接录入 
                //}
                //else
                //{
                //    //gridColumn8.OptionsColumn.AllowEdit = false; //原来有 需要右键修改 
                //    if (e != null && e.Button == MouseButtons.Right)
                //    {
                //        contextMenuStrip1.Show(gridControl2, new Point(e.X, e.Y));
                //        gridView2.CloseEditor();
                //        this.BindingContext[dtM].EndCurrentEdit();
                //    }
                //}
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
            fm修改软件费用 fm = new fm修改软件费用(dr);
            fm.ShowDialog();

        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                gridControl2.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
