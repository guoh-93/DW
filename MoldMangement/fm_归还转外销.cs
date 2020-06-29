using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MoldMangement
{
    public partial class fm_归还转外销 : Form
    {
        string strconn = CPublic.Var.strConn;
        string strSoNo;
        
        DataTable dt_借还附表;
        DataTable dt_销售附表;
        DataTable dt_客户;
        DataTable dt_目标客户;

        public bool flag = false;  //指示是否保存


        private   DataRow dr_借还;
        DataTable  dt_归;

 
        public DataSet ds_外销;

        public fm_归还转外销()
        {
            InitializeComponent();
        }

        public fm_归还转外销(DataRow dr_借还, DataTable dt_仓库出入库明细, DataSet ds_外销,DataTable dt_归还)
        {
            // TODO: Complete member initialization
            InitializeComponent();
            this.dr_借还 = dr_借还;
            this.dt_归 = dt_归还;
            //this.dt_仓库出入库明细 = dt_仓库出入库明细;
            this.ds_外销 = ds_外销;
        }

        public fm_归还转外销(DataRow dr_借还,DataTable dt_归还列表)
        {
            // TODO: Complete member initialization
            InitializeComponent();
            this.dr_借还 = dr_借还;
            this.dt_归 = dt_归还列表;
   
        }


        private void fm_归还转外销_Load(object sender, EventArgs e)
        {
            try
            {
          
                loadBasedData();
                fun_load();
                txt_税率.Properties.Items.Clear();
                txt_订单方式.Properties.Items.Clear();
                string sql3 = "select 属性类别,属性值 from 基础数据基础属性表 where 属性类别 = '订单方式' or 属性类别 = '税率'";
                DataTable dt_属性值 = new DataTable();
                SqlDataAdapter da_属性值 = new SqlDataAdapter(sql3, strconn);
                da_属性值.Fill(dt_属性值);
                foreach (DataRow r in dt_属性值.Rows)
                {
                    if (r["属性类别"].ToString() == "税率")
                    {
                        txt_税率.Properties.Items.Add(r["属性值"].ToString());
                    }
                    if (r["属性类别"].ToString() == "订单方式")
                    {
                        txt_订单方式.Properties.Items.Add(r["属性值"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void loadBasedData()
        {
            try
            {
                string sql = "select  POS as 编号,属性值 as 包装方式,属性字段1 as 描述 from  基础数据基础属性表  where 属性类别='包装方式' order by 编号";
                DataTable dt_包装方式 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                searchLookUpEdit4.Properties.DataSource = dt_包装方式;
                searchLookUpEdit4.Properties.ValueMember = "包装方式";
                searchLookUpEdit4.Properties.DisplayMember = "包装方式";
                sql = @" select b.物料编码,b.物料名称 ,b.规格型号,
                         b.物料类型,b.大类,b.小类 from (select 父项编码 from 配件包 group by 父项编码) a
                         left join 基础数据物料信息表 b on a.父项编码  = b.物料编码";
                DataTable dt_配件包 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                searchLookUpEdit3.Properties.DataSource = dt_配件包;
                searchLookUpEdit3.Properties.ValueMember = "物料编码";
                searchLookUpEdit3.Properties.DisplayMember = "物料编码";

                txt_业务员.Properties.Items.Clear();
                sql = "select 属性值 from 基础数据基础属性表  where 属性类别='业务员' order by POS";
                DataTable dt_属性 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_属性);
                foreach (DataRow r in dt_属性.Rows)
                {
                    txt_业务员.Properties.Items.Add(r["属性值"].ToString());
                }

                sql = "select * from 客户基础信息表";
                dt_客户 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                searchLookUpEdit1.Properties.DataSource = dt_客户;
                searchLookUpEdit1.Properties.DisplayMember = "客户编号";
                searchLookUpEdit1.Properties.ValueMember = "客户编号";

                sql = "select 客户编号,客户名称 from 客户基础信息表 where 停用 = 0 ";
                dt_目标客户 = new DataTable();
                dt_目标客户 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                comboBox1.DataSource = dt_目标客户;
                comboBox1.ValueMember = "客户编号";
                comboBox1.DisplayMember = "客户名称";

                sql = "select  属性值 from  基础数据基础属性表 where 属性类别='账期' order by CONVERT(int,属性值)";
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                foreach (DataRow dr in temp.Rows)
                {
                    cd_账期.Properties.Items.Add(dr["属性值"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

      
        private void fun_load()
        {
            DateTime t = CPublic.Var.getDatetime();
            //DateTime t = Convert.ToDateTime("2020-3-31 09:46:19.353");
            try
            {
                DataTable dt_客户;
                string s = string.Format("select * from 客户基础信息表 where 客户名称 = '{0}'",dr_借还["相关单位"]);
                dt_客户 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
               // textBox1.Text = dr_借还["相关单位"].ToString();
                comboBox1.Text = dr_借还["相关单位"].ToString();
                s = string.Format(@"select 借还申请表附表.*,base.计量单位,base.图纸编号 from 借还申请表附表 
                        left join 基础数据物料信息表 base on 借还申请表附表.物料编码=base.物料编码 where 借还申请表附表.申请批号 = '{0}'", dr_借还["申请批号"]);
                    //where 借还申请表附表.物料编码=仓库物料数量表.物料编码 and 仓库物料数量表.物料编码=基础数据物料信息表.物料编码";
                dt_借还附表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select * from 销售记录销售订单明细表 where 1<>1";
                dt_销售附表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                //2019-8-27 增加
                dt_销售附表.Columns.Add("最大归还数",typeof(decimal));

              //  strSoNo = string.Format("SO{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
              //t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SO", t.Year, t.Month).ToString("0000"));
              //  txt_销售订单号.Text = strSoNo;
                if (dt_客户.Rows.Count > 0)
                {
                    searchLookUpEdit1.EditValue = dt_客户.Rows[0]["客户编号"];
                    txt_业务员.Text = dt_客户.Rows[0]["业务员"].ToString();                   
                }
                txt_日期.Text = t.ToString();
                txt_录入人员.Text = CPublic.Var.localUserName;

                int i = 1;
                foreach (DataRow dr in dt_归.Rows)
                {
                    if (Convert.ToDecimal(dr["最大归还数"]) > 0)
                    {
                        DataRow dr_销售明细 = dt_销售附表.NewRow();
                        dt_销售附表.Rows.Add(dr_销售明细);
                        dr_销售明细["GUID"] = System.Guid.NewGuid();
                        //dr_销售明细["销售订单号"] = txt_销售订单号.Text;
                        //dr_销售明细["POS"] = i;
                        //dr_销售明细["销售订单明细号"] = dr_销售明细["销售订单号"] + "-" + i++.ToString("00");
                        dr_销售明细["物料编码"] = dr["物料编码"];
                        dr_销售明细["数量"] = Convert.ToDecimal(dr["最大归还数"]);
                        dr_销售明细["最大归还数"] = Convert.ToDecimal(dr["最大归还数"]);

                        dr_销售明细["物料名称"] = dr["物料名称"];
                        //dr_销售明细["n原ERP规格型号"] = dr["n原ERP规格型号"];
                        dr_销售明细["送达日期"] = t;
                        dr_销售明细["规格型号"] = dr["规格型号"];
                        dr_销售明细["计量单位"] = dr["计量单位"];
                        dr_销售明细["计量单位编码"] = dr["计量单位编码"];
                        dr_销售明细["仓库号"] = dr["仓库号"];
                        dr_销售明细["仓库名称"] = dr["仓库名称"];
                        dr_销售明细["备注5"] = dr["申请批号明细"]; //备注5 其他的订单存放的是预订单明细 这里 存放 借用明细号方便传回判断 借用明细是否完成
                    }
                    // dr_销售明细["计量单位"] = dr["计量单位"];


                    // dr_销售明细["图纸编号"] = dr["图纸编号"];
                    //dr_销售明细["税前单价"] = Convert.ToDecimal(txt_税前单价.Text);
                    //dr_销售明细["税后单价"] = Convert.ToDecimal(txt_税后单价.Text);
                    //dr_销售明细["税前金额"] = Convert.ToDecimal(txt_税前金额.Text);
                    //dr_销售明细["税前金额"] = Convert.ToDecimal(txt_金额.Text);
                    //dr_销售明细["完成数量"] = dr["实际借用数量"];
                    //dr_销售明细["未完成数量"] = 0;
                    //dr_销售明细["已通知数量"] = dr["实际借用数量"];
                    //dr_销售明细["未通知数量"] = 0;
                    //dr_销售明细["客户编号"] = searchLookUpEdit1.EditValue;
                    //dr_销售明细["客户"] = txt_客户名称;
                    //dr_销售明细["生效"] = true;
                    //dr_销售明细["生效日期"] = t;
                    //dr_销售明细["明细完成"] = true;
                    //dr_销售明细["明细完成日期"] = t;
                    //dr_销售明细["总完成"] = true;
                    //dr_销售明细["总完成日期"] = t;
                    //dr_销售明细["已计算"] = true;
                    //dr_销售明细["录入人员ID"] = CPublic.Var.LocalUserID;
                }
                gc.DataSource = dt_销售附表;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }         
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DataRow[] ds = dt_客户.Select(string.Format("客户编号 = '{0}'", searchLookUpEdit1.EditValue));
                txt_客户名称.Text = ds[0]["客户名称"].ToString();
                txt_业务员.Text = ds[0]["业务员"].ToString();
                cd_账期.Text = ds[0]["账期"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //保存
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("是否确认归还转外销"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    fun_check();
                    fun_明细金额变化();
                    ds_外销 = fun_save(dr_借还["申请批号"].ToString());
                    flag = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_明细金额变化()
        {
            System.Decimal sum = 0;
            System.Decimal sum1 = 0;
            foreach (DataRow r in dt_销售附表.Rows)
            {
                if (r.RowState == DataRowState.Deleted)
                {
                    continue;
                }
                try
                {
                    r["税后金额"] = ((Decimal)r["税后单价"] * (Decimal)r["数量"]).ToString("0.000000");
                    sum += (Decimal)r["税后金额"];
                    r["税前金额"] = ((Decimal)r["税前单价"] * (Decimal)r["数量"]).ToString("0.000000");
                    sum1 += (Decimal)r["税前金额"];
                }
                catch(Exception)
                {
                    
                }
                txt_税前金额.Text = sum1.ToString();
                txt_金额.Text = sum.ToString();
            }
        }

        private void fun_check()
        {
            try
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("客户不能为空");
                }
                if (txt_日期.EditValue == null)
                {
                    throw new Exception("日期为空");
                }
                if (cd_账期.Text == "")
                {
                    throw new Exception("帐期为空");
                }
                if (txt_业务员.Text == "")
                {
                    throw new Exception("业务员为空");
                } 
                if(dt_销售附表.Rows.Count==0)
                    throw new Exception("没有明细可生效");

                foreach (DataRow dr in dt_销售附表.Rows)
                {
                    if (dr["税前单价"].ToString() == "")
                    {
                        throw new Exception("税前单价为空");
                    }

                    if(Convert.ToDecimal(dr["数量"])> Convert.ToDecimal(dr["最大归还数"]))
                    {
                        throw new Exception("已超过可归还最大数量");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DataSet fun_save(string str_借用单号)
        {
            DataSet ds =new DataSet();
            DateTime t = CPublic.Var.getDatetime();
            //DateTime t = Convert.ToDateTime("2020-3-31 09:46:19.353");
            try
            {
                DataTable dt_仓库人员 = new DataTable();

                DataTable dt_销售订单主表;
                DataTable dt_出库通知单主表;
                DataTable dt_出库通知单明细表;
                DataTable dt_成品出库单主表;
                DataTable dt_成品出库单明细表;
                DataTable dt_仓库出入库明细;

                string s = "select * from 销售记录销售订单主表 where 1<>1";
                dt_销售订单主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select * from 销售记录销售出库通知单主表 where 1<>1";
                dt_出库通知单主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select * from 销售记录销售出库通知单明细表 where 1<>1";
                dt_出库通知单明细表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select * from 销售记录成品出库单主表 where 1<>1";
                dt_成品出库单主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select * from 销售记录成品出库单明细表 where 1<>1";
                dt_成品出库单明细表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                s = "select * from 仓库出入库明细表  where 1<>1";
                dt_仓库出入库明细 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                //  string s_出库通知单号 = string.Format("SK{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                //t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SK", t.Year, t.Month).ToString("0000"));
                //  string s_成品出库单号 = string.Format("SA{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                //t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SA", t.Year, t.Month).ToString("0000"));
                //  strSoNo = string.Format("SO{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                //t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SO", t.Year, t.Month).ToString("0000"));


                strSoNo = string.Format("SO{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
        t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SO", t.Year, t.Month, t.Day).ToString("0000"));
                string s_出库通知单号 = string.Format("SK{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                    t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SK", t.Year, t.Month).ToString("0000"));
                string s_成品出库单号 = string.Format("SA{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                    t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SA", t.Year, t.Month).ToString("0000"));


                txt_销售订单号.Text = strSoNo;
                DataRow dr_销售订单主 = dt_销售订单主表.NewRow();
                dt_销售订单主表.Rows.Add(dr_销售订单主);
                dr_销售订单主["GUID"] = System.Guid.NewGuid();
                dr_销售订单主["销售订单号"] = strSoNo;
                dr_销售订单主["客户编号"] = searchLookUpEdit1.EditValue;
                dr_销售订单主["录入人员"] = CPublic.Var.localUserName;
                dr_销售订单主["录入人员ID"] = CPublic.Var.LocalUserID;
                dr_销售订单主["业务员"] = txt_业务员.Text;
                dr_销售订单主["帐期"] = cd_账期.Text;
                dr_销售订单主["密码类型"] = "默认";
                dr_销售订单主["待审核"] = true;
                dr_销售订单主["审核"] = true;
                dr_销售订单主["销售备注"] =string.Format("借出转外销:{0}", str_借用单号);
                dr_销售订单主["日期"] = t;
                dr_销售订单主["税率"] = txt_税率.Text;
                dr_销售订单主["订单方式"] = txt_订单方式.Text;
                dr_销售订单主["税前金额"] =Convert.ToDecimal(txt_税前金额.Text);
                dr_销售订单主["税后金额"] = Convert.ToDecimal(txt_金额.Text);
                dr_销售订单主["生效"] = true;
                dr_销售订单主["生效日期"] = t;
                dr_销售订单主["生效人员"] = CPublic.Var.localUserName;
                dr_销售订单主["生效人员ID"] = CPublic.Var.LocalUserID;
                dr_销售订单主["客户名"] = txt_客户名称.Text;
                dr_销售订单主["目标客户"] = comboBox1.Text;
                dr_销售订单主["部门编号"] = CPublic.Var.localUser部门编号;
                dr_销售订单主["销售部门"] = CPublic.Var.localUser部门名称;

                dr_销售订单主["创建日期"] = t;
                dr_销售订单主["修改日期"] = t;
                dr_销售订单主["完成"] = true;
                dr_销售订单主["完成日期"] = t;
                dr_销售订单主["备注1"] = str_借用单号;//记录借用申请单号

                ds.Tables.Add(dt_销售订单主表);

                 DataRow dr_出库通知单主 = dt_出库通知单主表.NewRow();
                dt_出库通知单主表.Rows.Add(dr_出库通知单主);
                dr_出库通知单主["GUID"] = System.Guid.NewGuid();
                dr_出库通知单主["出库通知单号"] = s_出库通知单号;
                dr_出库通知单主["客户编号"] = searchLookUpEdit1.EditValue;
                dr_出库通知单主["客户名"] =txt_客户名称.Text;
          

                dr_出库通知单主["出库日期"] = t;
                dr_出库通知单主["创建日期"] = t;
                dr_出库通知单主["修改日期"] = t;
                dr_出库通知单主["操作员ID"] = CPublic.Var.LocalUserID;
                dr_出库通知单主["操作员"] = CPublic.Var.localUserName;
                dr_出库通知单主["生效"] = true;
                dr_出库通知单主["生效日期"] = t;
                dr_出库通知单主["完成"] = true;
                dr_出库通知单主["完成日期"] = t;
                dr_出库通知单主["提交审核"] = true;
                dr_出库通知单主["审核"] = true;
                ds.Tables.Add(dt_出库通知单主表);

                 DataRow dr_成品出库主 = dt_成品出库单主表.NewRow();
                dt_成品出库单主表.Rows.Add(dr_成品出库主);
                dr_成品出库主["GUID"] = System.Guid.NewGuid();
                dr_成品出库主["成品出库单号"] = s_成品出库单号;
               
                foreach (DataRow dr in dt_销售附表.Rows)
                {
                    string sql_仓库人员 = string.Format("select * from 人员仓库对应表 where 仓库号 = '{0}'", dr["仓库号"].ToString());
                    dt_仓库人员 = CZMaster.MasterSQL.Get_DataTable(sql_仓库人员, strconn);
                  //  DataRow[] ds_仓库人员 = dt_仓库人员.Select("仓库号= '{0}'", dr["仓库号"].ToString());
                    if (dt_仓库人员.Rows.Count > 0)
                    {
                        dr_成品出库主["操作员ID"] = dt_仓库人员.Rows[0]["工号"];
                        dr_成品出库主["操作员"] = dt_仓库人员.Rows[0]["姓名"];
                        break;
                    }
                   

                }

                //dr_成品出库主["操作员ID"] = CPublic.Var.LocalUserID;
                //dr_成品出库主["操作员"] = CPublic.Var.localUserName;
                dr_成品出库主["客户"] = txt_客户名称.Text;
        

                dr_成品出库主["日期"] = t;
                dr_成品出库主["创建日期"] = t;
                dr_成品出库主["修改日期"] = t;
                dr_成品出库主["生效"] = true;
                dr_成品出库主["生效日期"] = t;
                ds.Tables.Add(dt_成品出库单主表);

                   int i = 1;
                foreach (DataRow dr in dt_销售附表.Rows)
                {
                    

                    dr["销售订单号"] = txt_销售订单号.Text;
                    dr["销售订单明细号"] = strSoNo + "-" + i.ToString("00");
                    dr["POS"] = i;

                    
                    //DataRow dr_销售订单子 = dt_销售附表.NewRow();
                    //dt_销售附表.Rows.Add(dr_销售订单子);
                    dt_销售附表.Rows[i-1]["GUID"] = System.Guid.NewGuid();
                    dt_销售附表.Rows[i - 1]["销售订单号"] = strSoNo;
                    dt_销售附表.Rows[i - 1]["POS"] = i;
                    dt_销售附表.Rows[i - 1]["销售订单明细号"] = strSoNo + "-" + i.ToString("00");
                    dt_销售附表.Rows[i - 1]["物料编码"] = dr["物料编码"];
                    dt_销售附表.Rows[i - 1]["数量"] = dr["数量"];
                    dt_销售附表.Rows[i - 1]["完成数量"] = dr["数量"];
                    dt_销售附表.Rows[i - 1]["未完成数量"] = 0;
                    dt_销售附表.Rows[i - 1]["已通知数量"] = dr["数量"];
                    dt_销售附表.Rows[i - 1]["未通知数量"] = 0;
                    dt_销售附表.Rows[i - 1]["物料名称"] = dr["物料名称"];
                    //dt_销售附表.Rows[i - 1]["n原ERP规格型号"] = dr["n原ERP规格型号"];
                    //dr_销售订单子["税前金额"] = 0;
                    //dr_销售订单子["税后金额"] = 0;
                    //dr_销售订单子["税前单价"] = 0;
                    //dr_销售订单子["税后单价"] = 0;
                    dt_销售附表.Rows[i - 1]["仓库号"] = dr["仓库号"];
                    dt_销售附表.Rows[i - 1]["仓库名称"] = dr["仓库名称"];

                    


                    dt_销售附表.Rows[i - 1]["送达日期"] = t;
                    dt_销售附表.Rows[i - 1]["税率"] = txt_税率.Text;
                    dt_销售附表.Rows[i - 1]["客户编号"] = searchLookUpEdit1.EditValue;
                    dt_销售附表.Rows[i - 1]["客户"] = txt_客户名称.Text;
                    dt_销售附表.Rows[i - 1]["生效"] = true;
                    dt_销售附表.Rows[i - 1]["生效日期"] = t;
                    dt_销售附表.Rows[i - 1]["明细完成"] = true;
                    dt_销售附表.Rows[i - 1]["明细完成日期"] = t;
                    dt_销售附表.Rows[i - 1]["总完成"] = true;
                    dt_销售附表.Rows[i - 1]["总完成日期"] = t;
                    dt_销售附表.Rows[i - 1]["已计算"] = true;
                    dt_销售附表.Rows[i - 1]["录入人员ID"] = CPublic.Var.LocalUserID;
                    dt_销售附表.Rows[i - 1]["含税销售价"] = dr["税后单价"];

                    DataRow dr_出库通知单明细 = dt_出库通知单明细表.NewRow();
                    dt_出库通知单明细表.Rows.Add(dr_出库通知单明细);
                    dr_出库通知单明细["GUID"] = System.Guid.NewGuid();
                    dr_出库通知单明细["出库通知单号"] = s_出库通知单号;
                    dr_出库通知单明细["POS"] = i;
                    dr_出库通知单明细["出库通知单明细号"] = s_出库通知单号 + "-" + i.ToString("00");
                    dr_出库通知单明细["销售订单明细号"] = dt_销售附表.Rows[i - 1]["销售订单明细号"];
                    dr_出库通知单明细["物料编码"] = dr["物料编码"];
                    dr_出库通知单明细["物料名称"] = dr["物料名称"];
                    dr_出库通知单明细["出库数量"] = dr["数量"];
                    dr_出库通知单明细["规格型号"] = dr["规格型号"];
                    dr_出库通知单明细["图纸编号"] = dr["图纸编号"];
                    dr_出库通知单明细["操作员ID"] = CPublic.Var.LocalUserID;
                    dr_出库通知单明细["操作员"] = CPublic.Var.localUserName;
                    dr_出库通知单明细["生效"] = true;
                    dr_出库通知单明细["销售备注"] = "借出转外销";
                    dr_出库通知单明细["生效日期"] = t;
                    dr_出库通知单明细["完成"] = true;
                    dr_出库通知单明细["完成日期"] = t;
                    dr_出库通知单明细["计量单位"] = dr["计量单位"];
                    dr_出库通知单明细["客户"] = txt_客户名称.Text;
                    dr_出库通知单明细["客户编号"] =searchLookUpEdit1.EditValue;
                    dr_出库通知单明细["已出库数量"] = dr["数量"];
                    dr_出库通知单明细["未出库数量"] = 0;
                    //dr_出库通知单明细["n原ERP规格型号"] = dr["n原ERP规格型号"];

                    DataRow dr_成品出库明细 = dt_成品出库单明细表.NewRow();
                    dt_成品出库单明细表.Rows.Add(dr_成品出库明细);
                    dr_成品出库明细["GUID"] = System.Guid.NewGuid();
                    dr_成品出库明细["成品出库单号"] = s_成品出库单号;
                    dr_成品出库明细["POS"] = i;
                    dr_成品出库明细["成品出库单明细号"] = s_成品出库单号 + "-" + i.ToString("00");
                    dr_成品出库明细["销售订单号"] = strSoNo;
                    dr_成品出库明细["销售订单明细号"] = dt_销售附表.Rows[i++ - 1]["销售订单明细号"];
                    dr_成品出库明细["出库通知单号"] = s_出库通知单号;
                    dr_成品出库明细["出库通知单明细号"] = dr_出库通知单明细["出库通知单明细号"];
                    dr_成品出库明细["物料编码"] = dr["物料编码"];
                    dr_成品出库明细["物料名称"] = dr["物料名称"];
                    dr_成品出库明细["出库数量"] = dr["数量"];
                    dr_成品出库明细["已出库数量"] = dr["数量"];
                    dr_成品出库明细["未开票数量"] = dr["数量"];
                    dr_成品出库明细["计量单位"] = dr["计量单位"];
                    dr_成品出库明细["图纸编号"] = dr["图纸编号"];
                    dr_成品出库明细["规格型号"] = dr["规格型号"];
                    dr_成品出库明细["客户"] = txt_客户名称.Text;
                    dr_成品出库明细["客户编号"] = searchLookUpEdit1.EditValue;
                    dr_成品出库明细["仓库号"] = dr["仓库号"];
                    dr_成品出库明细["仓库名称"] = dr["仓库名称"];
                    dr_成品出库明细["生效"] = true;
                    dr_成品出库明细["生效日期"] = t;
                    //dr_成品出库明细["n原ERP规格型号"] = dr["n原ERP规格型号"];

                    DataRow dr_仓库出入库明细 = dt_仓库出入库明细.NewRow();
                    dt_仓库出入库明细.Rows.Add(dr_仓库出入库明细);
                    dr_仓库出入库明细["GUID"] = System.Guid.NewGuid();
                    dr_仓库出入库明细["明细类型"] = "销售出库";
                    dr_仓库出入库明细["单号"] = s_成品出库单号;
                    dr_仓库出入库明细["物料编码"] = dr["物料编码"];
                    dr_仓库出入库明细["物料名称"] = dr["物料名称"];
                    dr_仓库出入库明细["明细号"] = dr_成品出库明细["成品出库单明细号"];
                    dr_仓库出入库明细["出库入库"] = "出库";
                    dr_仓库出入库明细["实效数量"] = "-" + dr["数量"];
                    dr_仓库出入库明细["实效时间"] = t;
                    dr_仓库出入库明细["出入库时间"] = t;
                    dr_仓库出入库明细["相关单号"] = dr_成品出库明细["销售订单明细号"];
                    dr_仓库出入库明细["相关单位"] = txt_客户名称.Text;
                    dr_仓库出入库明细["仓库号"] = dr["仓库号"];
                    dr_仓库出入库明细["仓库名称"] = dr["仓库名称"];
                }
                ds.Tables.Add(dt_销售附表);
                ds.Tables.Add(dt_出库通知单明细表);
                ds.Tables.Add(dt_成品出库单明细表);
                ds.Tables.Add(dt_仓库出入库明细);               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } 
            return ds;
        }

        //关闭
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void gv_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
               
                if (e.Value.ToString() != "")
                {
                    Decimal dec税率 = Convert.ToDecimal(Convert.ToInt32(txt_税率.EditValue) / (Decimal)100);
                    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                    if (e.Column.FieldName == "税后单价" || e.Column.Caption == "数量")
                    {
                        if (dr["税后单价"].ToString() != "" && Convert.ToDecimal(dr["税后单价"]) >= (Decimal)0)
                        {
                            //if (e.Row["税前单价"] == DBNull.Value || Convert.ToDecimal(e.Row["税前单价"]) != (Convert.ToDecimal(e.Row["税后单价"]) / ((Decimal)1 + dec税率)))
                            {
                                dr["税前单价"] = (Convert.ToDecimal(dr["税后单价"]) / ((Decimal)1 + dec税率)).ToString("0.000000");
                            }
                        }
                        fun_明细金额变化();
                    }
                    if (e.Column.FieldName == "税前单价")
                    {
                        if (Convert.ToDecimal(dr["税前单价"]) >= (Decimal)0)
                        {
                            //if (dr["税后单价"] == DBNull.Value || Convert.ToDecimal(dr["税前单价"]) != (Convert.ToDecimal(dr["税后单价"]) / ((Decimal)1 + dec税率)))
                            {
                                dr["税后单价"] = (Convert.ToDecimal(dr["税前单价"]) * ((Decimal)1 + dec税率)).ToString("0.000000");
                            }
                        }
                        fun_明细金额变化();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                dt_销售附表.Rows.Remove(dr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc, new Point(e.X, e.Y));
            }
        }



        //private void fun_保存主表明细(bool bl)
        //{
        //    DateTime t = CPublic.Var.getDatetime();
        //    string sql_kh = string.Format("select * from 客户基础信息表 where 客户编号 = '" + dr_借还["借用人员"].ToString() + "'");
        //    DataTable dt_客户 = CZMaster.MasterSQL.Get_DataTable(sql_kh, strconn);
        //    searchLookUpEdit1.EditValue = dt_客户.Rows[0]["客户编号"].ToString();
        //    try
        //    {
        //        string sql = "select * from 销售记录销售订单主表 where 1<>1";
        //        dt_销售主单 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
        //        DataRow dr_销售主单 = dt_销售主单.NewRow();
        //        dt_销售主单.Rows.Add(dr_销售主单);
        //        strSoNo = string.Format("SO{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
        //       t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SO", t.Year, t.Month, t.Day).ToString("0000"));
        //        txt_销售订单号.Text = strSoNo.ToString();

        //        if (dr_销售主单["GUID"].ToString() == "")
        //        {
        //            dr_销售主单["GUID"] = System.Guid.NewGuid();
        //            dr_销售主单["销售订单号"] = strSoNo.ToString();
        //            dr_销售主单["录入人员"] = CPublic.Var.localUserName;
        //            dr_销售主单["录入人员ID"] = CPublic.Var.LocalUserID;
        //            dr_销售主单["日期"] = CPublic.Var.getDatetime();
        //            dr_销售主单["创建日期"] = CPublic.Var.getDatetime();
        //        }
        //        if (bl == true)
        //        {
        //            dr_销售主单["生效"] = true;
        //            dr_销售主单["生效日期"] = CPublic.Var.getDatetime();
        //            dr_销售主单["生效人员"] = CPublic.Var.localUserName;
        //            dr_销售主单["生效人员ID"] = CPublic.Var.LocalUserID;
        //        }
        //        dr_销售主单["修改日期"] = CPublic.Var.getDatetime();
        //        dr_销售主单["作废"] = false;
        //        dr_销售主单["完成"] = true;
        //        dr_销售主单["完成日期"] = CPublic.Var.getDatetime();
        //        dr_销售主单["计划是否确认"] = true;
        //        if (dt_客户.Rows.Count > 0)
        //        {
        //            dr_销售主单["客户编号"] = dt_客户.Rows[0]["客户编号"].ToString();
        //            dr_销售主单["客户名"] = dt_客户.Rows[0]["客户名称"].ToString();
        //            dr_销售主单["业务员"] = dt_客户.Rows[0]["业务员"].ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    try
        //    {
        //        string sql = "select * from 销售记录销售订单明细表 where 1<>1";
        //        dt_销售附表 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
        //        int i = 1;
        //        string sql_jymx = string.Format("select * from 借还申请表附表 where 申请批号 = '"+dr_借还["申请批号"]+"'");
        //        dt_借还附表 = CZMaster.MasterSQL.Get_DataTable(sql_jymx, strconn);
        //        DataTable dt_基础;
        //        foreach (DataRow dr in dt_借还附表.Rows)
        //        {
        //            string sql_基础 = string.Format("select * from 基础数据物料信息表 where 物料编码 = '" + dr["物料编码"] + "'");
        //            dt_基础 = CZMaster.MasterSQL.Get_DataTable(sql_基础, strconn);
        //            DataRow dr_销售附表 = dt_销售附表.NewRow();
        //            dt_销售附表.Rows.Add(dr_销售附表);
        //            dr_销售附表["GUID"] = System.Guid.NewGuid(); 
        //            dr_销售附表["POS"] = i.ToString();
        //            dr_销售附表["销售订单明细号"] = strSoNo.ToString() + "-" + i.ToString("00");
        //            dr_销售附表["物料编码"] = dr["物料编码"].ToString();
        //            dr_销售附表["物料名称"] = dr["物料名称"].ToString();
        //            dr_销售附表["数量"] = dr["实际借用数量"].ToString();
        //            dr_销售附表["n原ERP规格型号"] = dr["n原ERP规格型号"].ToString();
        //            if (dt_客户.Rows.Count > 0)
        //            {
        //                dr_销售附表["客户编号"] = dt_客户.Rows[0]["客户编号"].ToString();
        //                dr_销售附表["客户"] = dt_客户.Rows[0]["客户名称"].ToString();
        //            }
        //            gc.DataSource = dt_销售附表;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }

        //}
    }
}
