using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
namespace IAACA
{
    public partial class ui工时维护界面 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable dtM;

        public ui工时维护界面()
        {
            InitializeComponent();
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

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void fun_load()
        {
            string s = @"select aa.*,c.工时,c.工时 as 原始值,'' 原因 from (
            select   a.物料编码,b.规格型号,b.物料名称  from 生产记录成品入库单明细表  a
            left join 基础数据物料信息表 b on a.物料编码=b.物料编码  group by a.物料编码,b.规格型号,b.物料名称)aa 
            left join [2019财务工时] c on c.产品编码 =aa.物料编码 
            where left(物料编码,2)=10 order by 工时";
            dtM = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gridControl2.DataSource = dtM;
        }
        private void  fun_save()
        {
            DataView dv = new DataView(dtM);
            dv.RowStateFilter = DataViewRowState.ModifiedCurrent | DataViewRowState.Added | DataViewRowState.Deleted;
            DataTable dtcopy = dv.ToTable();
            SqlConnection conn = new SqlConnection(strcon);
            SqlCommand cmd = new SqlCommand();
            cmd = conn.CreateCommand();
            cmd.CommandText = "select * from [2019财务工时] where 1=2";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt_save = new DataTable();
            da.Fill(dt_save);
            cmd.CommandText = "select * from [软件工时修改日志表] where 1=2";
            DataTable dt_软件工时=new DataTable ();
            da.Fill(dt_软件工时);

            string s_base= "select * from [基础数据物料信息表] where 1=2";
            cmd.CommandText = s_base;
           DataTable dt_base = new DataTable();
            da.Fill(dt_base);

            int x = dt_save.Rows.Count;
            DateTime time = CPublic.Var.getDatetime();
            foreach (DataRow dr in dtcopy.Rows)
            {
                cmd.CommandText = $"select * from [2019财务工时] where 产品编码='{dr["物料编码"].ToString()}' ";
                da.Fill(dt_save);
                if(x==dt_save.Rows.Count) //表中没有  新增
                {
                    DataRow r = dt_save.NewRow();
                    r["产品编码"] = dr["物料编码"].ToString();
                    r["工时"] = dr["工时"].ToString() ;
                    dt_save.Rows.Add(r);
                    //增加修改日志

                    DataRow rr = dt_软件工时.NewRow();
                    rr["GUID"] = System.Guid.NewGuid();
                    rr["类型"] = "工时";
                    rr["产品编码"] = dr["物料编码"].ToString();

                    rr["原始值"] = "";
                    rr["更新值"] = dr["工时"].ToString() ;
                    rr["修改人"] = CPublic.Var.localUserName;
                    rr["修改时间"] = time;
                    
                    dt_软件工时.Rows.Add(rr);

                    x++;
                }
                else
                {
                    dt_save.Rows[dt_save.Rows.Count-1]["工时"] = dr["工时"].ToString();

               
                    //增加修改日志

                    DataRow rr = dt_软件工时.NewRow();
                    rr["GUID"] = System.Guid.NewGuid();
                    rr["类型"] = "工时";
                    rr["产品编码"] = dr["物料编码"].ToString();
                    rr["原始值"] = dr["原始值"].ToString();
                    rr["更新值"] = dr["工时"].ToString();
                    rr["修改人"] = CPublic.Var.localUserName;
                    rr["修改时间"] = time;
                    rr["原因"] = dr["原因"].ToString();
                    dt_软件工时.Rows.Add(rr);
                }

                cmd.CommandText = $"select * from [基础数据物料信息表] where 物料编码='{dr["物料编码"].ToString()}' ";
                da.Fill(dt_base);
                dt_base.Rows[dt_base.Rows.Count-1]["工时"]= Convert.ToDecimal(dr["工时"]);

            }
            string  ss= "select * from [2019财务工时] where 1=2";
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("工时维护");
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

               
                cmm = new SqlCommand(s_base, conn, ts);
                da_cun = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da_cun);
                da_cun.Update(dt_base);
                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw new Exception("保存失败");
            }
           
        }
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("是否确认退出当前界面", "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                CPublic.UIcontrol.ClosePage();
            }
        }
        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
            fm修改工时 fm = new fm修改工时(dr);
            fm.ShowDialog();
        }

        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
            if(dr["工时",DataRowVersion.Original]==null || dr["工时", DataRowVersion.Original].ToString() == "")
            {
                gridColumn8.OptionsColumn.AllowEdit = true; //没有工时直接录入 
            }
            else
            {
                gridColumn8.OptionsColumn.AllowEdit = false; //原来有 需要右键修改 
                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gridControl2, new Point(e.X, e.Y));
                    gridView2.CloseEditor();
                    this.BindingContext[dtM].EndCurrentEdit();
                }
            }
        }

        private void gridView2_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                if (dr["工时", DataRowVersion.Original] == null || dr["工时", DataRowVersion.Original].ToString() == "")
                {
                    gridColumn8.OptionsColumn.AllowEdit = true; //没有工时直接录入 
                }
                else
                {
                    gridColumn8.OptionsColumn.AllowEdit = false; //原来有 需要右键修改 
                    
                }

            }
            catch (Exception ex)
            {
               
            }
        }
        string cfgfilepath = "";
        private void ui工时维护界面_Load(object sender, EventArgs e)
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
    }
}
