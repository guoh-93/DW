using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm待领料替换 : Form
#pragma warning restore IDE1006 // 命名样式
    {
        public frm待领料替换()
        {
            InitializeComponent();
        }
        #region
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataRow r;
        public bool flag = false;  //指示是否保存a
        bool can = true;
        int a = 0;
        public int 关闭 = 0;
        public string xiala = "";

        DataTable dt_waitSupplies;
        DataTable dt_stock;
        DataRow drM数据;

        #endregion
        /// <summary>
        /// 19-9-16增加的注释
        /// </summary>
        /// <param name="dr">生产工单信息</param>
        /// <param name="DT"> 领料明细</param>
        /// <param name="drrw">领料明细第一条</param>
        public frm待领料替换(DataRow dr, DataTable DT, DataRow drrw)
        {
            InitializeComponent();

            dt_waitSupplies = DT.Copy();
            dt_waitSupplies.Columns.Add("已领数参考数", typeof(decimal));
            dt_waitSupplies.Columns.Add("待领料参考数", typeof(decimal));
            foreach (DataRow drrr in dt_waitSupplies.Rows)
            {


                drrr["已领数参考数"] = drrr["已领数量"];
                drrr["待领料参考数"] = drrr["待领料总量"];
                if (drrr["库存总数"].ToString() == "")
                {

                    drrr["库存总数"] = 0;
                }
                if (drrr["有效总数"].ToString() == "")
                {

                    drrr["有效总数"] = 0;
                }
            }
            gridControl1.DataSource = dt_waitSupplies;
            r = dr;
            drM数据 = drrw;

        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //string sql = string.Format("select * from 生产记录生产工单待领料明细表 where 生产工单号='{0}'", r["生产工单号"]);
            string sql = string.Format(@"select a . * ,b.库存总数,b.有效总数  from 生产记录生产工单待领料明细表 a left join 仓库物料数量表 b on a.物料编码=b.物料编码  and a.仓库号=b.仓库号   
left join 生产记录生产工单待领料主表  dcc on  dcc.待领料单号=  a.待领料单号
where a. 生产工单号='{0}'  and  领料类型<>'生产补料'   order by a.待领料单明细号", r["生产工单号"]);


            dt_waitSupplies = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            dt_waitSupplies.Columns.Add("已领数参考数", typeof(decimal));
            dt_waitSupplies.Columns.Add("待领料参考数", typeof(decimal));
            foreach (DataRow dr in dt_waitSupplies.Rows)
            {

                dr["已领数参考数"] = dr["已领数量"];
                dr["待领料参考数"] = dr["待领料总量"];
                if (dr["库存总数"].ToString() == "")
                {

                    dr["库存总数"] = 0;
                }
                if (dr["有效总数"].ToString() == "")
                {

                    dr["有效总数"] = 0;
                }
            }

            gridControl1.DataSource = dt_waitSupplies;


        }//刷新

#pragma warning disable IDE1006 // 命名样式
        private void frm待领料替换_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
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
                x.UserLayout(panel3, this.Name, cfgfilepath);
                this.StartPosition = FormStartPosition.CenterScreen;
                string sql_3 = string.Format(@"select  物料编码,规格型号,物料名称,仓库号,仓库名称,计量单位编码,计量单位   from 基础数据物料信息表 where 停用='0'");//停用=0
                DataTable dtg = CZMaster.MasterSQL.Get_DataTable(sql_3, strcon);
                repositoryItemSearchLookUpEdit2.DataSource = dtg;
                repositoryItemSearchLookUpEdit2.DisplayMember = "物料编码";
                repositoryItemSearchLookUpEdit2.ValueMember = "物料编码";
                string sql22 = "select  属性字段1 as 仓库号,属性值 as 仓库名称  from  基础数据基础属性表 where 属性类别='仓库类别' and 布尔字段5=1"; //"可发料";//布尔字段5
                dt_stock = CZMaster.MasterSQL.Get_DataTable(sql22, strcon);
                repositoryItemSearchLookUpEdit3.DataSource = dt_stock;
                repositoryItemSearchLookUpEdit3.DisplayMember = "仓库号";
                repositoryItemSearchLookUpEdit3.ValueMember = "仓库号";





                strw = dt_waitSupplies.Rows[dt_waitSupplies.Rows.Count - 1]["待领料单明细号"].ToString();
                sArray = strw.Split('-');// 一定是单引 

                a = int.Parse(sArray[1].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }





        }
        string strw = "";
        string[] sArray = null;
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {



            DataRow dr = dt_waitSupplies.NewRow();
            dt_waitSupplies.Rows.Add(dr);





        }




        int p = 0;
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dt_waitSupplies].EndCurrentEdit();
                this.BindingContext[gridView1].EndCurrentEdit();
                if (can == false)
                {
                    throw new Exception("待发料总数小于已经领取数");
                }

                DataView dv = new DataView(dt_waitSupplies);

                for (int i = 0; i < dt_waitSupplies.Rows.Count; i++)
                {
                    DataRow dr = dt_waitSupplies.Rows[i];
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    if (dr["待领料总量"].ToString() == "")
                    {
                        throw new Exception("请输入待领料总量");
                    }
                    if (Convert.ToDecimal(dr["未领数量"]) > 0)
                    {
                        dr["完成"] = 0;
                    }
                    dr["待领料单号"] = drM数据["待领料单号"].ToString();
                    dr["生产工单号"] = drM数据["生产工单号"].ToString();
                    dr["生产制令单号"] = drM数据["生产制令单号"].ToString();
                    dr["生产工单类型"] = drM数据["生产工单类型"].ToString();
                    dr["制单人员ID"] = drM数据["制单人员ID"].ToString();
                    dr["制单人员"] = drM数据["制单人员"].ToString();
                    dr["创建日期"] = drM数据["创建日期"].ToString();
                    dr["工单负责人ID"] = drM数据["工单负责人ID"].ToString();
                    dr["工单负责人"] = drM数据["工单负责人"].ToString();
                    dr["领料人"] = drM数据["领料人"].ToString();
                    dr["领料人ID"] = drM数据["领料人ID"].ToString();
                    if (dr["BOM数量"] == null || dr["BOM数量"].ToString() == "" || dr["BOM数量"].ToString().Trim() == "0")
                    {
                        dr["BOM数量"] = Math.Round(Convert.ToDecimal(dr["待领料总量"]) / Convert.ToDecimal(r["生产数量"]), 6, MidpointRounding.AwayFromZero);
                    }
                    if (dr["物料编码"].ToString() == "")
                    {
                        throw new Exception("请选择物料");
                    }
                    if (dr["待领料单明细号"].ToString() == "")
                    {
                        a = a + 1;
                        dr["待领料单明细号"] = sArray[0].ToString() + "-" + (a).ToString("00");

                    }

                }
 
                using (SqlDataAdapter da = new SqlDataAdapter("select * from 生产记录生产工单待领料明细表 where 1<>1", strcon))
                {

                    new SqlCommandBuilder(da);
                    da.Update(dt_waitSupplies);
                    p = 0;
                    MessageBox.Show("保存成功");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
 
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            try
            {
                DataRow r = (this.BindingContext[dt_waitSupplies].Current as DataRowView).Row;
                if (bool.Parse(r["完成"].ToString()) == true)
                {
                    throw new Exception("改行已完成不可删除");
                }
                if (Convert.ToDecimal(r["已领数量"]) > 0)
                {
                    throw new Exception("已有发料数量，不可删除");

                }
                r.Delete();
           

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }






        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.Close();
        }

        

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }

        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);





                if (e.Column.Caption == "仓库号" && e.Value != null && e.Value.ToString() != "")
                {


                    dr["仓库号"] = e.Value;
                    DataRow[] ds = dt_stock.Select(string.Format("仓库号 = '{0}'", dr["仓库号"]));
                    dr["仓库名称"] = ds[0]["仓库名称"];
                    string sql = string.Format("select * from 仓库物料数量表 where 物料编码='{0}'and 仓库号='{1}'  ", dr["物料编码"].ToString(), e.Value.ToString());
                    DataTable dt_库存 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    if (dt_库存.Rows.Count > 0)
                    {
                        dr["库存总数"] = dt_库存.Rows[0]["库存总数"];
                        dr["有效总数"] = dt_库存.Rows[0]["有效总数"];


                    }
                    else
                    {
                        dr["库存总数"] = 0;
                        dr["有效总数"] = 0;

                    }



                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);



                if (e.Column.Caption == "待领料总量" && e.Value != null && e.Value.ToString() != "")
                {
                    //dr["已领数参考数"] = dr["已领数量"];
                    //dr["待领料参考数"] = dr["待领料总量"];
                    if (dr["待领料参考数"].ToString() != "")
                    {
                        if (decimal.Parse(dr["待领料总量"].ToString()) != decimal.Parse(dr["待领料参考数"].ToString()))
                        {

                            decimal a = decimal.Parse(dr["待领料总量"].ToString()) - decimal.Parse(dr["已领数参考数"].ToString());
                            if (a < 0)
                            {
                                can = false;
                                throw new Exception("待发料总数小于已经领取数，不可修改");
                            }


                            dr["未领数量"] = a;

                        }
                    }
                    else
                    {
                        dr["未领数量"] = decimal.Parse(dr["待领料总量"].ToString());
                        dr["已领数量"] = 0;
                    }
 

                }
                can = true;


                if (e.Column.Caption == "物料编码" && e.Value != null && e.Value.ToString() != "")
                {

 
                    string sql_32 = string.Format(@"select * from 仓库物料数量表 where 物料编码='{0}'", dr["物料编码"].ToString());
                    DataTable dtg = CZMaster.MasterSQL.Get_DataTable(sql_32, strcon);
                    DataTable dtTableDisinit = (DataTable)this.gridControl1.DataSource;
                    

                    dr["物料名称"] = dtg.Rows[0]["物料名称"];
                    dr["有效总数"] = dtg.Rows[0]["有效总数"];
                    dr["库存总数"] = dtg.Rows[0]["库存总数"];
                    dr["仓库号"] = dtg.Rows[0]["仓库号"];
                    dr["仓库名称"] = dtg.Rows[0]["仓库名称"];
                    //dr["BOM数量"] = dtg.Rows[0]["数量"];
                    dr["规格型号"] = dtg.Rows[0]["规格型号"];
                    dr["备注2"] = "修改人:" + CPublic.Var.localUserName + " 修改时间：" + CPublic.Var.getDatetime();
                    //dr["计量单位编号"] = dtg.Rows[0]["计量单位编码"];
                    //dr["计量单位"] = dtg.Rows[0]["计量单位"];
                    //MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
                    //DataTable dt_SaleOrder_1 = RBQ.SelectGroupByInto("", dtTableDisinit, "物料编码,sum(BOM数量) BOM数量", "", "物料编码");
                    //if (dtTableDisinit.Rows.Count != dt_SaleOrder_1.Rows.Count)
                    //{

                    //    throw new Exception("请勿添加重复数据!");

                    //}

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
