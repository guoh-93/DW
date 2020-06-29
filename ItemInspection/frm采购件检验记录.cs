using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using CPublic;
using CZMaster;
using System.Drawing.Printing;
using DevExpress.XtraTab;
using System.Threading;
using System.IO;
namespace ItemInspection
{
    public partial class frm采购件检验记录 : UserControl
    {
        #region 成员
        /// <summary>
        /// 检验单单号
        /// </summary>
        public string strJYDDH = "";
        public string strCPBH = "";
        #endregion

        #region 私有成员
        DataTable dtM;
        DataTable dtP;

        DataRow drQ_通知单;
        DataRow drQ_通知单明细;

        string strWLConn = CPublic.Var.strConn;
        //string strWLConn = Var.geConn("WL");

        DataTable dtBHGYY;
        bool bl_免检 = false;
        string strCurrUser = "";

        string strRKTZD = "";
        string strConn_FS = CPublic.Var.geConn("FS");
        string strconn1 = CPublic.Var.strConn;
        string strconn2 = CPublic.Var.geConn("DW");
        string cfgfilepath = "";
        #endregion

        #region 类加载
        public frm采购件检验记录()
        {
            InitializeComponent();
        }
        public frm采购件检验记录(DataRow dr_1)
        {
            InitializeComponent();
            strJYDDH = dr_1["检验记录单号"].ToString();
            strCPBH = dr_1["产品编号"].ToString();
        }




        DataTable dt_送检单号;

        private void fun_load采购送检单()
        {
            try
            {
                //devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
                //devGridControlCustom1.strConn = CPublic.Var.strConn;
                string sql = "";
//                if (checkBox3.Checked == false)
//                {
//                    sql = string.Format(@"select 采购记录采购送检单明细表.*,基础数据物料信息表.物料编码,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.计量单位 
//                    from 采购记录采购送检单明细表 left join 采购记录采购检验默认人员表 on 采购记录采购检验默认人员表.物料编码 = 采购记录采购送检单明细表.物料编码 
//                    left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 采购记录采购送检单明细表.物料编码 
//                    where 采购记录采购送检单明细表.检验完成=0 and 采购记录采购送检单明细表.作废 = 0 and (采购记录采购检验默认人员表.默认检验员 is NULL or 采购记录采购检验默认人员表.默认检验员 = '{0}')
//                     and 采购记录采购送检单明细表.送检日期 >= '2016-11-01 00:00:00'",
//                    CPublic.Var.localUserName);
//                }
//                else
//                {
//                    sql = @"select 采购记录采购送检单明细表.*,基础数据物料信息表.物料编码,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.计量单位  from 采购记录采购送检单明细表 
//                    left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 采购记录采购送检单明细表.物料编码 
//                    where 采购记录采购送检单明细表.检验完成=0 and 采购记录采购送检单明细表.送检日期 >= '2016-11-01 00:00:00' and 采购记录采购送检单明细表.作废 = 0";
//                }
                sql = @" select b.*,a.物料编码,a.n原ERP规格型号,a.计量单位,采购单类型   from 采购记录采购送检单明细表 b
                    left join 基础数据物料信息表 a on a.物料编码 = b.物料编码 
                    left join 采购记录采购单主表 cgz on cgz.采购单号 =b.采购单号 
                    where b.检验完成=0 and b.送检日期 >= '2018-1-01' and b.作废 = 0 and b.送检单类型<>'拒收' and b.送检数量>0";
                dt_送检单号 = new DataTable();
                dt_送检单号 = MasterSQL.Get_DataTable(sql, strWLConn);
                dt_送检单号.Columns.Add("可检验数");

                foreach (DataRow r in dt_送检单号.Rows)
                {
                    r["可检验数"] = Convert.ToDecimal(r["送检数量"]) - Convert.ToDecimal(r["已检验数"]);
                }
                gc.DataSource = dt_送检单号;
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_load采购送检单");
                throw ex;
            }
        }

        private void frm采购件检验记录_Load(object sender, EventArgs e)
        {
            
            try
            {


                barLargeButtonItem11.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;


                cfgfilepath = System.IO .Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name)))
                {

                    gv.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

               // textBox1.Text = "DWDH2018";
                fun_load采购送检单();
                strCurrUser = CPublic.Var.localUserName;
                barStaticItem1.Caption = string.Format("操作员:{0} ", strCurrUser);
                fun_加载人员下拉框数据();
               // CPublic.UIcontrol.GridControlResponseMouseWheel(gvM);
                fun_加载不合格原因();
                if (strJYDDH != "")
                {
                    fun_加载主数据(strJYDDH);
                    fun_加载入库通知单(dtM.Rows[0]["送检单明细号"].ToString(), dtM.Rows[0]["送检单号"].ToString());
                    //fun_加载入库通知单(dtM.Rows[0]["采购入库通知单号"].ToString());
                    fun_加载界面数据();
                    barLargeButtonItem2.Enabled = false;
                    barLargeButtonItem5.Enabled = false;
                    foreach (DataRow r in dtP.Rows)
                    {
                        if (r["检验水平"].ToString() == "" && r["AQL"].ToString() == "")
                        {
                            r["抽检数"] = DBNull.Value;
                            r["Ac"] = DBNull.Value;
                        }
                    }
                }
                else
                {
                    //strJYDDH = string.Format("IC{0}{1:00}{2:00000}",DateTime.Now.Year,DateTime.Now.Month, CPublic.CNo.fun_得到最大流水号("IC",DateTime.Now.Year,DateTime.Now.Month));

                    //txtJYDH.Text = strJYDDH;

                    fun_清空并新增数据();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 加载（添加DWsql）
        DataTable dt_DW;
       
        private void fun_查找送检单1()
        {
            //,d.fQuantity,d.iPerTaxRate,d.fUnitPrice,d.fTaxPrice,d.fMoney(括号原先left join PU_AppVouchs d on d.cInvCode = a.cInvCode)
          //19年1/2更改
//            string str_DW = @"select f.*,e.* from (select b.*,c.ID,a.iQuantity,c.cVenCode as ccc,c.cCode,c.dDate
//                 from PU_ArrivalVouchs a 
//            left join PU_ArrivalVouch c on c.ID = a.ID
//            left join Inventory b on a.cInvCode = b.cInvCode
            //             where c.cCode  ='"+"DWDH20"+ textBox1.Text.ToString() + "') f left join Vendor e   on e.cVenCode = f.ccc";
            string str_DW = @"select f.*,e.* from (select c.ID,sum(a.iQuantity)iQuantity,c.cVenCode as ccc,c.cCode,c.dDate,b.cInvCode,b.cInvName,b.cInvStd,b.iTaxRate
                 from PU_ArrivalVouchs a 
            left join Inventory b on a.cInvCode = b.cInvCode        
            left join PU_ArrivalVouch c on c.ID = a.ID
             where c.cCode  ='" + "DWDH20" + textBox1.Text.ToString() + "'group by c.ID,c.cVenCode,c.cCode,c.dDate,b.cInvCode,b.cInvName,b.cInvStd,b.iTaxRate) f left join Vendor e   on e.cVenCode = f.ccc";


            using (SqlDataAdapter da = new SqlDataAdapter(str_DW, strconn2))
            {
                dt_DW = new DataTable();
                da.Fill(dt_DW);

            }
            
        }
        DataTable dt_到货单是否存在 = new DataTable();
        private void fun_判断到货单是否存在()
        {

            DataTable dt_查找1 = new DataTable();
            string str1 = "select * from PU_ArrivalVouch  where cCode ='" + "DWDH20" + textBox1.Text.ToString() + "'";
            using (SqlDataAdapter da = new SqlDataAdapter(str1, strconn2))
            {

                da.Fill(dt_查找1);
            }
            try
            {
                string str22 = "select 送检单号 from 采购记录采购送检单主表 where 备注1 ='" + dt_查找1.Rows[0]["ID"].ToString() + "'";
                using (SqlDataAdapter da2 = new SqlDataAdapter(str22, strconn1))
                {

                    da2.Fill(dt_到货单是否存在);

                }
            }
            catch
            { }
        }
        DataTable dtM_检验主 = new DataTable();
        DataTable dtM_检验子 = new DataTable();
        private void fun_保存送检单1()
        {
            //try
            //{

            using (SqlDataAdapter da1 = new SqlDataAdapter("select * from 采购记录采购送检单主表 where 1<>1", strconn1))
                {
                    dtM_检验主 = new DataTable();
                    da1.Fill(dtM_检验主);


                }

            using (SqlDataAdapter da = new SqlDataAdapter("select * from 采购记录采购送检单明细表 where 1<>1", strconn1))
                {
                    dtM_检验子 = new DataTable();
                    da.Fill(dtM_检验子);
                    //保存子信息   
                    foreach (DataRow dr in dt_DW.Rows)
                    {
                        //保存主信息
                        //foreach (DataRow dr_送检主 in dt_DW.Rows)
                        //{
                        DataRow dr_存入 = dtM_检验子.NewRow();
                        dtM_检验子.Rows.Add(dr_存入);

                        dr_存入["GUID"] = System.Guid.NewGuid();
                        dr_存入["物料编码"] = dr["cInvCode"].ToString();
                        dr_存入["物料名称"] = dr["cInvName"].ToString();
                        dr_存入["规格型号"] = dr["cInvStd"].ToString();
                        dr_存入["送检数量"] = dr["iQuantity"].ToString();
                        dr_存入["税率"] = dr["iTaxRate"].ToString();
                        //送检单明细号 有唯一性
                        DateTime t = CPublic.Var.getDatetime();
                        string a = string.Format("SJ{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("SJ", t.Year, t.Month));
                        dr_存入["送检单号"] = a;
                        dr_存入["送检单明细号"] = a + "-" + 1;
                        
                        //dr_存入["送检单明细号"] = dr["AutoID"].ToString();
                        //dr_存入["送检单号"] = dr["ID"].ToString();
                        dr_存入["供应商ID"] = dr["cVenCode"].ToString();
                        dr_存入["供应商"] = dr["cVenName"].ToString();
                        dr_存入["供应商负责人"] = dr["cVenPerson"].ToString();
                        dr_存入["供应商电话"] = dr["cVenPhone"].ToString();
                        dr_存入["备注2"] = "DWDH20" + textBox1.Text.ToString();
                        //if (dr["iPerTaxRate"].ToString() != "")
                        //{
                        //dr_存入["税率"] = Convert.ToDecimal(dr["iPerTaxRate"]);
                        //}
                        //if (dr["fUnitPrice"].ToString() != "")
                        //{
                        //dr_存入["未税单价"] = Convert.ToDecimal(dr["fUnitPrice"]);
                        //}
                        //if (dr["fTaxPrice"].ToString() != "")
                        //{
                        //dr_存入["单价"] = Convert.ToDecimal(dr["fTaxPrice"]);
                        //}
                        //// dr_存入["未税金额"] = dt_DW.Rows[0]["cVenPhone"].ToString(); 表没有
                        //if (dr["fMoney"].ToString() != "")
                        //{
                        //    dr_存入["金额"] = Convert.ToDecimal(dr["fMoney"]);
                        //}
                        //if (dr["fQuantity"].ToString() != "")
                        //{
                        //dr_存入["采购数量"] =Convert.ToDecimal(dr["fQuantity"]);
                        //}
                        dr_存入["送检日期"] = CPublic.Var.getDatetime();
                            DataRow dr_存入主 = dtM_检验主.NewRow();
                            dtM_检验主.Rows.Add(dr_存入主);
                            dr_存入主["GUID"] = System.Guid.NewGuid();
                            dr_存入主["送检单号"] = a.ToString();
                            dr_存入主["物料编码"] = dr["cInvCode"].ToString();
                            dr_存入主["物料名称"] = dr["cInvName"].ToString();
                            dr_存入主["送检数量"] =dr["iQuantity"].ToString();
                            dr_存入主["供应商ID"] = dr["cVenCode"].ToString();
                            dr_存入主["供应商"] = dr["cVenName"].ToString();
                            dr_存入主["供应商负责人"] = dr["cVenPerson"].ToString();
                            dr_存入主["供应商电话"] = dr["cVenPhone"].ToString();
                            dr_存入主["录入日期"] = dr["dDate"].ToString();
                            dr_存入主["备注1"] = dr["ID"].ToString();



                    }
                }
            //}
            //catch{}
        }
        //需要判断 表中供应商是否存在？？？？
        //DataTable dt_供应商是否存在 = new DataTable();
        //private void fun_判断表中供应商是否存在()
        //{
          
        //        //string str123 = "select * from 采购供应商表 where 供应商ID ='" + dt_DW.Rows[0]["cVenCode"].ToString() + "'";
        //    using (SqlDataAdapter da = new SqlDataAdapter("select * from 采购供应商表 where 1<>1'", strconn1))
        //        {

        //            da.Fill(dt_供应商是否存在);

        //            foreach(DataRow dr in dt_DW.Rows)
        //            {
        //                string str123 = "select * from 采购供应商表 where 供应商ID ='" + dr["cVenCode"].ToString() + "'";
        //                if(str123 =="")
        //                {
                        
        //                }
        //                else  
        //                { 
                        
        //                }

        //            }
        //        }
            
        //}


        DataTable dt_供应商 = new DataTable();
        private void fun_保存供应商信息()
        {
            using (SqlDataAdapter da = new SqlDataAdapter("select * from 采购供应商表 where 1<>1", strconn1))
          {
              
              da.Fill(dt_供应商);
              foreach (DataRow dr in dt_DW.Rows)
              {
                  string str123 = "select * from 采购供应商表 where 供应商ID ='" + dr["cVenCode"].ToString() + "'";
                  DataTable dt_判断供应商是否存在 = new DataTable();
                  using(SqlDataAdapter da1 = new SqlDataAdapter(str123,strconn1))
                  {
                      
                      da1.Fill(dt_判断供应商是否存在);
                  }
                  if (dt_判断供应商是否存在.Rows.Count == 0 && dt_供应商.Select("供应商ID ='" + dr["cVenCode"].ToString() + "'").Length == 0)
                  {
                      DataRow dr_供应商 = dt_供应商.NewRow();
                      dt_供应商.Rows.Add(dr_供应商);
                      dr_供应商["供应商GUID"] = System.Guid.NewGuid();
                      dr_供应商["供应商ID"] = dr["cVenCode"].ToString();
                      dr_供应商["供应商名称"] = dr["cVenName"].ToString();
                      dr_供应商["供应商负责人"] = dr["cVenPerson"].ToString();
                      dr_供应商["供应商电话"] = dr["cVenPhone"].ToString();
                      dr_供应商["供应商地址"] = dr["cVenAddress"].ToString();
                      dr_供应商["供应商等级"] = dr["iGradeABC"].ToString();
                      //dr_供应商["省"] = dt_DW.Rows[0]["cVenCode"].ToString();
                      //dr_供应商["市"] = dt_DW.Rows[0]["cVenCode"].ToString();
                      //dr_供应商["县"] = dt_DW.Rows[0]["cVenCode"].ToString();
                      dr_供应商["税率"] = Convert.ToInt32(dr["iTaxRate"]);
                      dr_供应商["交期"] = dr["dLastDate"].ToString();
                      dr_供应商["供应商传真"] = dr["cVenFax"].ToString();
                      dr_供应商["备注"] = dr["cMemo"].ToString();
                      // dr_供应商["供应商状态"] = dt_DW.Rows[0]["cVenCode"].ToString();
                      dr_供应商["供应商邮箱"] = dr["cVenEmail"].ToString();
                      dr_供应商["供应商卡号"] = dr["cVenAccount"].ToString();
                      dr_供应商["供应商简码"] = dr["cVenBank"].ToString();//这是开户行
                  }
              }
          }
        
        }
        //需判断 物料是否已保存于表中？？？？？？
        //DataTable dt_物料是否已存在 = new DataTable();
        //private void fun_判断物料是否已保存于表中()
        //{
        //    try
        //    {
        //        string stee = "select * from 基础数据物料信息表 where 物料编码 ='" + dt_DW.Rows[0]["cInvCode"].ToString() + "'";
        //        using (SqlDataAdapter da = new SqlDataAdapter(stee, strconn1))
        //        {
        //            da.Fill(dt_物料是否已存在);
        //        }
        //    }
        //    catch { }
        //}

        DataTable dt_基础表 = new DataTable();
        private void fun_保存基础数据表()
        {
            using (SqlDataAdapter da = new SqlDataAdapter("select * from 基础数据物料信息表 where 1<>1", strconn1))
            {
                
                da.Fill(dt_基础表);
               foreach(DataRow dr_基础表 in dt_DW.Rows)
               {
                   string stee = "select * from 基础数据物料信息表 where 物料编码 ='" + dr_基础表["cInvCode"].ToString() + "'";
                   DataTable dt_物料是否已存在 = new DataTable();
                   using(SqlDataAdapter da1 = new SqlDataAdapter(stee,strconn1))
                  {
                      
                      da1.Fill(dt_物料是否已存在);
                  }
                   if (dt_物料是否已存在.Rows.Count == 0 && dt_基础表.Select( "物料编码 ='" + dr_基础表["cInvCode"].ToString() + "'").Length ==0)
                   {
                   DataRow dr_存入基 = dt_基础表.NewRow();
                   dt_基础表.Rows.Add(dr_存入基);
                   dr_存入基["物料编码"] = dr_基础表["cInvCode"].ToString();
                   dr_存入基["物料编码"] = dr_基础表["cInvCode"].ToString();
                   dr_存入基["物料名称"] = dr_基础表["cInvName"].ToString();
                   dr_存入基["规格型号"] = dr_基础表["cInvStd"].ToString();
                   dr_存入基["n原ERP规格型号"] = dr_基础表["cInvStd"].ToString();
                   dr_存入基["大类GUID"] = dr_基础表["cInvCCode"].ToString();
                   dr_存入基["供应商编号"] = dr_基础表["cVenCode"].ToString();
                   if (dr_基础表["iInvRCost"].ToString() != "")
                       {
                   dr_存入基["标准单价"] =Convert.ToDecimal(dr_基础表["iInvRCost"]);
                       }
                   if (dr_基础表["iTopSum"].ToString() != "")
                   {
                       dr_存入基["库存上限"] = Convert.ToDouble(dr_基础表["iTopSum"]);
                   }
                   if (dr_基础表["iLowSum"].ToString() != "")
                   {
                       dr_存入基["库存下限"] = Convert.ToDouble(dr_基础表["iLowSum"]);
                   }
                   if (dr_基础表["iInvWeight"].ToString() != "")
                   {
                       dr_存入基["克重"] = Convert.ToDecimal(dr_基础表["iInvWeight"]);
                   }
                   dr_存入基["生效时间"] = dr_基础表["dSDate"].ToString();
                   if (dr_基础表["dEDate"].ToString() != "")
                       {
                   dr_存入基["停用时间"] = dr_基础表["dEDate"].ToString();
                       }

                   dr_存入基["仓库号"] = dr_基础表["cDefWareHouse"].ToString();
                   //dr_存入基[""] = dr_基础表[""].ToString();
                   //dr_存入基[""] = dr_基础表[""].ToString();
                   //dr_存入基[""] = dr_基础表[""].ToString();
                   //dr_存入基[""] = dr_基础表[""].ToString();
                   } 
               }
            }
        
        }
        //保存 4张表
        private void fun_save_DW()
        {
            SqlConnection conn = new SqlConnection(strconn1);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");
            string sql1 = "select * from 采购记录采购送检单明细表 where 1<>1";
            SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
            SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
            new SqlCommandBuilder(da1);


            string sql2 = "select * from 采购记录采购送检单主表  where 1<>1";
            SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);
            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            new SqlCommandBuilder(da2);


            string sql3 = "select * from 采购供应商表 where 1<>1";
            SqlCommand cmd3 = new SqlCommand(sql3, conn, ts);
            SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
            new SqlCommandBuilder(da3);

            string sql4 = "select * from 基础数据物料信息表 where 1<>1";
            SqlCommand cmd4 = new SqlCommand(sql4, conn, ts);
            SqlDataAdapter da4 = new SqlDataAdapter(cmd4);
            new SqlCommandBuilder(da4);
            try
            {
                da1.Update(dtM_检验子);
                da2.Update(dtM_检验主);
                da3.Update(dt_供应商);
                da4.Update(dt_基础表);
                 

                ts.Commit();
                MessageBox.Show("保存成功");

            }
            catch (Exception ex)
            {
                ts.Rollback();
                MessageBox.Show(ex.Message);
             
            }
        
        
        }
        #endregion     



        #region DEV操作
        #endregion

        #region 按钮操作
        #endregion

        #region 其它数据加载
        private void fun_加载不合格原因()
        {
            dtBHGYY = MasterSQL.Get_DataTable("select * from 基础数据检验项目不合格表", CPublic.Var.strConn);
        }
        /// <summary>
        /// 入库通知单号
        /// </summary>
        /// <param name="str_送检单"></param>
        private void fun_加载入库通知单(string str_送检单, string str_送检单号)
        {
            try
            {
                string sql;
                string sss = str_送检单号.Substring(0, 4);
                if (sss == "DWDH")
                {
                    sql = string.Format("select * from 采购记录采购送检单主表 where 送检单号='{0}'", str_送检单);
                }
                else
                {
                    sql = string.Format("select * from 采购记录采购送检单主表 where 送检单号='{0}'", str_送检单号);
                }
                
                drQ_通知单 = MasterSQL.Get_DataRow(sql, strWLConn);   //在送检单主表找到该送检单号 
                //这里大概在10月份 上面的已经无用  用下面的明细表找 

                sql = string.Format("select * from 采购记录采购送检单明细表 where 送检单明细号='{0}'", str_送检单);
                drQ_通知单明细 = MasterSQL.Get_DataRow(sql, strWLConn);
                if (drQ_通知单明细 == null)     //该送检单主表无效
                {
                    throw new Exception("入库通知单无效");
                }

                //if (drQ_通知单 == null)     //该送检单主表无效
                //{
                //    throw new Exception("入库通知单无效");
                //}

                //DataTable dt_通知单明细;
                //dt_通知单明细 = MasterSQL.Get_DataTable(sql, strWLConn);

                //if (dt_通知单明细.Rows.Count >= 1)
                //{
                //    if (strCPBH != "")
                //    {
                //        DataRow[] dr = dt_通知单明细.Select(string.Format("物料编码='{0}'", strCPBH));
                //        if (dr.Length > 0)
                //        {
                //            drQ_通知单明细.ItemArray = dr[0].ItemArray;
                //        }
                //    }
                //    if (strCPBH == "")
                //    {
                //        if (ItemInspectionData.frm产品检验数据维护.str == "")
                //        {
                //            sql = "select * from 采购记录采购送检单明细表 where 1<>1";
                //            dt_通知单明细 = MasterSQL.Get_DataTable(sql, strWLConn);

                //            using (fm采购通知单明细 fm = new fm采购通知单明细())
                //            {
                //                fm.strTZJno = str_送检单;
                //                if (fm.ShowDialog() != DialogResult.OK)
                //                {
                //                    fm.Close();
                //                    return;
                //                }
                //                drQ_通知单明细 = dt_通知单明细.NewRow();
                //                drQ_通知单明细.ItemArray = fm.Dr.ItemArray;
                //            }
                //        }
                //        if (ItemInspectionData.frm产品检验数据维护.str != "")
                //        {
                //            DataRow[] dr = dt_通知单明细.Select(string.Format("物料编码='{0}'", ItemInspectionData.frm产品检验数据维护.str));
                //            if (dr.Length > 0)
                //            {
                //                drQ_通知单明细.ItemArray = dr[0].ItemArray;
                //                ItemInspectionData.frm产品检验数据维护.str = "";
                //            }

                //        }
                //    }
                //    strCPBH = "";
                if (Convert.ToBoolean(drQ_通知单明细["作废"])) throw new Exception("该送检单已作废，刷新后重试");

                txtBHG.Text = "";
                textBox2.Text = "";
                txtJYY.Text = CPublic.Var.localUserName;
                txtSJY.Text = drQ_通知单明细["操作人员"].ToString();


                txtCGDDMXBH.Text = drQ_通知单明细["采购单明细号"].ToString();
                //}
                txtItem.Text = drQ_通知单明细["物料编码"].ToString().Trim();
                txtDWBH.Text = drQ_通知单明细["供应商ID"].ToString();
                txtPCS.Text = drQ_通知单明细["采购数量"].ToString();
                if (drQ_通知单["确认到货日期"] == DBNull.Value)
                {
                    dateEdit1.EditValue = Convert.ToDateTime(drQ_通知单["生效日期"].ToString());
                }
                else
                {
                    dateEdit1.EditValue = Convert.ToDateTime(drQ_通知单["确认到货日期"].ToString());
                }

                //txtYJS.Text = fun_得到已检数量(str_送检单, txtItem.Text).ToString();
                // decimal decc=fun_得到已检数量(str_送检单, txtItem.Text);
                //string gg = decc.ToString();

                txtYJS.Text = drQ_通知单明细["已检验数"].ToString();
                decimal  df = Convert.ToDecimal(drQ_通知单明细["已检验数"].ToString());
                decimal de =Convert.ToDecimal(drQ_通知单明细["送检数量"].ToString());
                txtSJSL.Text = (de - df).ToString();

                //txtSJSL.Text = (Decimal.Parse(txtPCS.Text) - Decimal.Parse(txtYJS.Text)).ToString();
                DataRow rrr = MasterSQL.Get_DataRow(string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", txtItem.Text), strWLConn);
                if (rrr != null)
                {
                    DBH1.DataFormDR(rrr);
                }
                else
                {
                    MessageBox.Show("没有找到该物料");
                }
                //DataRow rrrr = MasterSQL.Get_DataRow(string.Format("select * from 采购供应商表 where 供应商ID = '{0}'", txtDWBH.Text), strWLConn);
                DataRow rrrr = MasterSQL.Get_DataRow(string.Format("select * from 采购供应商表 where 供应商ID = '{0}'", drQ_通知单明细["供应商ID"].ToString().Trim()), strWLConn);
                if (rrrr != null)
                {
                    DBH1.DataFormDR(rrrr);
                }
                else
                {
                    MessageBox.Show("没有找到该供应商");
                }
                //DBH1.DataFormDR(MasterSQL.Get_DataRow(string.Format("select * from cp where  cpbh = '{0}'", txtItem.Text), strWLConn));
                //DBH1.DataFormDR(MasterSQL.Get_DataRow(string.Format("select * from gys where  gysbh = '{0}'", txtDWBH.Text), strWLConn));
                txtJYJG.Text = "合格";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 得到入库通知单信息,作废
        /// </summary>
        /// <param name="str_送检单"></param>
        private void fun_得到入库通知单信息(string strTZJno)
        {

            string sql;
            sql = string.Format("select * from 采购记录采购送检单主表 where 送检单号='{0}'", strTZJno);
            drQ_通知单 = MasterSQL.Get_DataRow(sql, strWLConn);

            sql = string.Format("select * from 采购记录采购送检单明细表 where 送检单号='{0}'", strTZJno);
            drQ_通知单明细 = MasterSQL.Get_DataRow(sql, strWLConn);
        }
        /// <summary>
        /// 4-26 弃用
        /// </summary>
        /// <param name="strTZJno"></param>
        /// <param name="ylbh"></param>
        /// <returns></returns>
        private Decimal fun_得到已检数量(string strTZJno, string ylbh)
        {
            string sql = string.Format("select sum(送检数量) as 送检数量  from 采购记录采购单检验主表 where  关闭=0 and  送检单号 = '{0}'and [产品编号]='{1}' group by 送检单号", strTZJno, ylbh.ToString().Trim());
            DataRow r = MasterSQL.Get_DataRow(sql, CPublic.Var.strConn);
            if (r != null)
            {
                return (Decimal)r["送检数量"];
            }
            else
            {
                return 0;
            }
        }

        #endregion

        #region #region 界面&数据
        private void fun_加载产品明细(string strItem)
        {
            string sql = string.Format("select * from 基础数据物料检验要求表 where 产品编码 = '{0}' order by POS", strItem);
            DataTable dtCPCY = MasterSQL.Get_DataTable(sql, Var.strConn);
 
                dtP.Clear();
      
            
            //foreach (DataRow r in dr_传.Rows)
            //{
            //    if(r.RowState == DataRowState.Deleted)continue;
            //    r.Delete();
            //}

            foreach (DataRow rCPCY in dtCPCY.Rows)
            {
                DataRow rP = dtP.NewRow();
                rP["GUID"] = Guid.NewGuid().ToString();
                rP["检验记录明细号"] = string.Format("{0}-{1:00}", strJYDDH, rCPCY["POS"]);
                rP["检验记录单号"] = strJYDDH;
                rP["POS"] = rCPCY["POS"];
                rP["送检数"] = 0;
                if (rCPCY["检验项目"].ToString() != "" && rCPCY["AQL"].ToString() != "")
                {
                    rP["抽检数"] = 0;
                }
                if (rCPCY["检验水平"].ToString() == "全检")
                {
                    rP["抽检数"] = 0;
                }
                rP["检验项目"] = rCPCY["检验项目"];
                rP["检验要求"] = rCPCY["检验要求"];
                rP["检验水平"] = rCPCY["检验水平"];
                rP["AQL"] = rCPCY["AQL"];
                rP["扩大值"] = rCPCY["扩大值"];
                rP["允许下限"] = rCPCY["下限值"];
                rP["允许上限"] = rCPCY["上限值"];
                rP["检验下限"] = "";
                rP["检验上限"] = "";
                //rP["合格"] = false;
                rP["合格"] = true;
                rP["备注"] = "";
                rP["不合格原因"] = "";
                rP["不合格数量"] = 0;
                rP["产品编码"] = txtItem.Text.Trim();

                dtP.Rows.Add(rP);
            }

            //dtP.ColumnChanged += dtP_ColumnChanged;
        }


        private void fun_加载样本数()
        {
            int iMax = 0;
            int iYBS = 0, iAc = 0;

            foreach (DataRow rp in dtP.Rows)
            {
                if (rp.RowState == DataRowState.Deleted) continue;

                if (rp["检验水平"].ToString() == "" && rp["AQL"].ToString() == "")
                {
                    if (rp["检验项目"].ToString().Trim() == "ROHS")  //品质 要求的 17-12
                    {
                        rp["抽检数"] = 1;

                    }
                    continue;
                }
                if (rp["检验水平"].ToString() != "全检")
                {
                    fun_抽检样本数计算(System.Decimal.Parse(txtSJSL.Text), rp["检验水平"].ToString(), rp["AQL"].ToString(), ref iYBS, ref iAc);
                    rp["抽检数"] = iYBS;
                    rp["Ac"] = iAc;
                }
                int SJSL = 0;
                if (rp["检验水平"].ToString() == "全检")
                {

                    try
                    {

                        SJSL = Convert.ToInt32(MyClass.GetNumber(txtSJSL.Text.Trim()));
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    rp["抽检数"] = SJSL;
                    rp["Ac"] = 0;
                }
                if (iYBS > iMax)
                {
                    iMax = iYBS;
                }
                if (SJSL > iMax)
                {
                    iMax = SJSL;
                }
            }
            txtCJSL.Text = iMax.ToString();
        }

        private void fun_清空界面数据()
        {
            foreach (Control ct in panel1.Controls)
            {
                if (ct is TextBox) { ct.Text = ""; }
            }

        }
        private void fun_加载界面数据()
        {
            try
            {
                string SJSL = dtM.Rows[0]["送检数量"].ToString();
               // DBH1.DataFormDR(dtM.Rows[0]);
                DBH1.DataFormDR(drQ_通知单明细);

                DBH1.DataFormDR(MasterSQL.Get_DataRow(string.Format("select * from 基础数据物料信息表 where  物料编码 = '{0}'", txtItem.Text), strWLConn));
                DBH1.DataFormDR(MasterSQL.Get_DataRow(string.Format("select * from 采购供应商表 where  供应商ID = '{0}'", txtDWBH.Text), strWLConn));

                if (txtPCS.Text == "")
                {
                    txtPCS.Text = drQ_通知单明细["通知数量"].ToString();
                }
                txtYJS.Text = SJSL;
                //txtSJSL.Text = (Decimal.Parse(txtPCS.Text) - Decimal.Parse(txtJYDH.Text)).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void fun_网格行_设置网格编辑状态()
        {
            try
            {
                DataRow r = (this.BindingContext[dtP].Current as DataRowView).Row;
                if (r["允许下限"].ToString() == "" || r["允许上限"].ToString() == "")
                {
                    gridColumn3.OptionsColumn.AllowEdit = false;
                    gridColumn4.OptionsColumn.AllowEdit = false;
                }
                else
                {
                    gridColumn3.OptionsColumn.AllowEdit = true;
                    gridColumn4.OptionsColumn.AllowEdit = true;
                }

                repositoryItemCheckedComboBoxEdit1.Items.Clear();
                dtBHGYY.DefaultView.RowFilter = string.Format("检验项目 = '{0}'", r["检验项目"].ToString());
                foreach (DataRowView drv in dtBHGYY.DefaultView)
                {
                    repositoryItemCheckedComboBoxEdit1.Items.Add(drv.Row["不合格原因"].ToString());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void gvM_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            fun_网格行_设置网格编辑状态();
        }


        #endregion

        #region 数据读取和保存
        private void fun_加载人员下拉框数据()
        {
            string strkey = string.Format("{0}-{1}-", strCurrUser, this.Name);
            List<string> li = LocalDataSetting.getLocalData(strkey + txtJYY.Name);
            txtJYY.Items.Clear();
            txtJYY.Items.AddRange(li.ToArray());

            li = LocalDataSetting.getLocalData(strkey + txtSJY.Name);
            txtSJY.Items.Clear();
            txtSJY.Items.AddRange(li.ToArray());
        }

        private void fun_保存人员下拉框数据()
        {
            string strkey = string.Format("{0}-{1}-", strCurrUser, this.Name);
            if (txtJYY.Text.Trim() != "")
            {
                LocalDataSetting.addLocalData(strkey + txtJYY.Name, txtJYY.Text.Trim());
            }
            if (txtSJY.Text.Trim() != "")
            {
                LocalDataSetting.addLocalData(strkey + txtSJY.Name, txtSJY.Text.Trim());
            }
        }
        private void fun_加载主数据(string strJYDDH)
        {
            try
            {
                string sql = string.Format("select * from 采购记录采购单检验主表 where 检验记录单号 = '{0}'", strJYDDH);
                dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                if (dtM.Rows[0]["检验结果"].ToString() == "免检")
                {
                    checkBox4.Checked = true;
                }
                sql = string.Format("select * from 采购记录采购单检验明细表 where 检验记录单号 = '{0}' order by POS", strJYDDH);
                dtP = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                gcP.DataSource = dtP;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void fun_保存数据(decimal dec_不合格数量)
        {
            try
            {
                DataRow xr= gv.GetDataRow(gv.FocusedRowHandle);
                
                DataTable dt_cgmx = new DataTable();
                //DataRow[] r = dt_送检单号.Select(string.Format("送检单号='{0}'", xr["送检单明细号"].ToString()));
                //if (r.Length >= 0)
                //{
                //    r[0]["已检验数"] = Convert.ToDecimal(r[0]["已检验数"]) + Convert.ToDecimal(txtSJSL.Text);
                //    if (Convert.ToDecimal(r[0]["已检验数"]) >= Convert.ToDecimal(r[0]["送检数量"]))
                //    {
                //        r[0]["检验完成"] = 1;
                //    }
                //}
                string sql_送检主表;
                string sss = xr["送检单号"].ToString().Substring(0, 4);
                if (sss == "DWDH")
                {
                    sql_送检主表 = string.Format("select * from 采购记录采购送检单主表 where 送检单号='{0}'", xr["送检单明细号"]);
                }
                else
                {
                    sql_送检主表 = string.Format("select * from 采购记录采购送检单主表 where 送检单号 = '{0}'", xr["送检单号"]);
                }
                
                DataTable dt_送检主表 = CZMaster.MasterSQL.Get_DataTable(sql_送检主表, strconn1);
                if (dt_送检主表.Rows.Count > 0)
                {
                    dt_送检主表.Rows[0]["确认到货日期"] = Convert.ToDateTime(dateEdit1.EditValue);
                }




                xr["已检验数"] = Convert.ToDecimal(xr["已检验数"]) + Convert.ToDecimal(txtSJSL.Text);
               

                if (Convert.ToDecimal(xr["已检验数"]) >= Convert.ToDecimal(xr["送检数量"]))
                {

                    xr["检验完成"] = 1;//当前送检单
                }


                //19-10-17 有不合格品直接生成拒收单 19-12-11将已拒收数量移到判断之中，委外不做拒收
                Dictionary<string, int> dic = new Dictionary<string, int>();
                int pos = 0;
                if (dec_不合格数量 > 0 && xr["采购单类型"].ToString().TrimEnd()!="委外采购")
                {
                    //19-10-17 有不合格品直接生成拒收单
                    xr["已拒收数"] = Convert.ToDecimal(xr["已拒收数"]) + dec_不合格数量;
                    if (!dic.ContainsKey(xr["送检单号"].ToString()))
                    {
                        string jj = string.Format("select max(pos)pos from 采购记录采购送检单明细表  where 送检单号='{0}'", xr["送检单号"]);
                        DataTable temp = CZMaster.MasterSQL.Get_DataTable(jj, strconn1);
                        dic.Add(xr["送检单号"].ToString(), Convert.ToInt32(temp.Rows[0]["pos"]) + 1);
                        pos = Convert.ToInt32(temp.Rows[0]["pos"]) + 1;
                    }
                    //string sql = string.Format("select max(pos)pos from 采购记录采购送检单明细表  where 送检单号='{0}'", xr["送检单号"]);
                    //DataTable dt111 = CZMaster.MasterSQL.Get_DataTable(sql, strconn1);
                    //int pos = Convert.ToInt32(dt111.Rows[0]["POS"])+1;
                    DataRow dr_拒收 = dt_送检单号.NewRow();
                    dt_送检单号.Rows.Add(dr_拒收);
                   
                    dr_拒收["GUID"] = Guid.NewGuid();
                    dr_拒收["送检单号"] = xr["送检单号"];
                    //dr_mx["POS"] = pos++;
                    //dr_mx["送检单明细号"] = sss + "-" + pos;
                    // 2019-10-8 pos号和明细号不一致   
                    dr_拒收["POS"] = pos;
                    dr_拒收["送检单明细号"] = xr["送检单号"].ToString() + "-" + pos;

                    dr_拒收["采购单号"] = xr["采购单号"];
                    dr_拒收["采购单明细号"] = xr["采购单明细号"];
                    dr_拒收["供应商ID"] = xr["供应商ID"];
                    dr_拒收["供应商"] = xr["供应商"];
                    dr_拒收["物料编码"] = xr["物料编码"];
                    dr_拒收["物料名称"] = xr["物料名称"];
                    dr_拒收["规格型号"] = xr["规格型号"];
                    dr_拒收["送检日期"] = xr["送检日期"];
                    dr_拒收["生效日期"] = DateTime.Now.ToString();
                    dr_拒收["送检人员ID"] = xr["送检人员ID"];
                    dr_拒收["送检人员"] = xr["送检人员"];
                    dr_拒收["操作人员ID"] = xr["操作人员ID"];
                    dr_拒收["操作人员"] = xr["操作人员"];
                    dr_拒收["送检数量"] = decimal.Parse(dec_不合格数量.ToString()) * -1;
                    dr_拒收["送检单类型"] = "拒收";
                    dr_拒收["生效人员ID"] = xr["生效人员ID"];
                    dr_拒收["生效人员"] = xr["生效人员"];
                    dr_拒收["生效"] = xr["生效"];

                    string sql_cgmx = string.Format("select * from  采购记录采购单明细表 where 采购明细号 = '{0}'",xr["采购单明细号"]);
                    dt_cgmx = CZMaster.MasterSQL.Get_DataTable(sql_cgmx,strconn1);
                    dt_cgmx.Rows[0]["已送检数"] = Convert.ToDecimal(dt_cgmx.Rows[0]["已送检数"]) - dec_不合格数量;
                    dt_cgmx.Rows[0]["明细完成"] = false;
                   // dt_送检单号.Rows[0].ItemArray = dr_拒收.ItemArray;
                }

                SqlConnection conn = new SqlConnection(CPublic.Var.strConn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("来料检验");
                try
                {
                    string sql_1 = "select  * from 采购记录采购送检单明细表 where 1<>1 ";
                    SqlCommand cmm_1 = new SqlCommand(sql_1, conn, ts);
                    string sql_2 = "select * from 采购记录采购单检验主表 where 1<>1";
                    SqlCommand cmm_2 = new SqlCommand(sql_2, conn, ts);
                    string sql_5 = "select * from 采购记录采购送检单主表 where 1<>1";
                    SqlCommand cmm_5 = new SqlCommand(sql_5, conn, ts);
                    SqlDataAdapter da_1 = new SqlDataAdapter(cmm_1);
                    SqlDataAdapter da_2 = new SqlDataAdapter(cmm_2);
                    SqlDataAdapter da_5 = new SqlDataAdapter(cmm_5);
                    if (!bl_免检)
                    {
                        string sql_3 = "select * from 采购记录采购单检验明细表 where 1<>1";
                        SqlCommand cmm_3 = new SqlCommand(sql_3, conn, ts);
                        SqlDataAdapter da_3 = new SqlDataAdapter(cmm_3);
                        new SqlCommandBuilder(da_3);
                        da_3.Update(dtP);
                    }
                    new SqlCommandBuilder(da_1);
                    new SqlCommandBuilder(da_2);
                    new SqlCommandBuilder(da_5);
                    da_1.Update(dt_送检单号);
                    da_2.Update(dtM);
                    da_5.Update(dt_送检主表);
                    if (dec_不合格数量 > 0)
                    {
                        string sql_4 = "select * from 采购记录采购单明细表 where 1<>1";
                        SqlCommand cmm_4 = new SqlCommand(sql_4, conn, ts);
                        SqlDataAdapter da_4 = new SqlDataAdapter(cmm_4);
                        new SqlCommandBuilder(da_4);
                        da_4.Update(dt_cgmx);

                    }
                    ts.Commit();
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    MessageBox.Show(ex.Message);
                    throw new Exception("保存失败,刷新后重试");
                }
                //MasterSQL.Save_DataTable(dt_送检单号, "采购记录采购送检单明细表", CPublic.Var.strConn);
                //MasterSQL.Save_DataTable(dtM, "采购记录采购单检验主表", CPublic.Var.strConn);
                //MasterSQL.Save_DataTable(dtP, "采购记录采购单检验明细表", CPublic.Var.strConn);

                fun_采购单检验状态();

                fun_load采购送检单();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fun_保存前检查及处理数据(decimal dec_不合格数量)
        {
            if (checkBox4.Checked == true)
            {
                bl_免检 = true;
            }

            if (strRKTZD == "")
            {
                throw new Exception("请选择入库通知单号");
            }
            if (txtJYY.Text == "")
            {
                throw new Exception("你没有选择检查员");
            }
            if (txtSJY.Text == "")
            {
                throw new Exception("你没有选择送检员");
            }
            if (dec_不合格数量>Convert.ToDecimal(txtSJSL.Text))
            {
                throw new Exception("不合格数量大于此次检验数量，请确认");
            }
            if (txtSJSL.Text == "")
            {
                throw new Exception("送检数量不能为空");
            } 
            if (Convert.ToDecimal(txtSJSL.Text) < 0)
            {
                throw new Exception("送检数量不能小于0");
            }
            decimal  SJSL = 0;
            decimal PCS = 0;
            decimal YJS = 0;
            try
            {

                SJSL = Convert.ToDecimal(MyClass.GetNumber(txtSJSL.Text.Trim()));
                PCS = Convert.ToDecimal(MyClass.GetNumber(txtPCS.Text.Trim()));
                YJS = Convert.ToDecimal(MyClass.GetNumber(txtYJS.Text.Trim()));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (SJSL == 0)
            {
                throw new Exception("送检数量不能为0");
            }
            //if (PCS == YJS)
            //{
            //    throw new Exception("送检数量不能为0");
            //}
            if (txtBHG.Text == "")
            {
                txtBHG.Value = 0;
            }
            //先处理dtM
            //if (dtM == null)
            //{
            //    string sql = "select * from 检验记录采购件检验表 where 1<>1 ";
            //    dtM = MasterSQL.Get_DataTable(sql, strWLConn);
            //}

            //if (dtM.Rows.Count == 0)
            //{ 
            //    dtM.Rows.Add(dtM.NewRow()); 
            //}

            //DataRow r = dtM.Rows[0];
            DataRow r = dtM.NewRow();
            DBH1.DataToDR(r);
            if (r["GUID"] == DBNull.Value)
            {
                r["GUID"] = Guid.NewGuid().ToString();

                r["检验日期"] = CPublic.Var.getDatetime();
            }
            else
            {
                r["修改检验日期"] = CPublic.Var.getDatetime();
            }
            r["送检单明细号"] = drQ_通知单明细["送检单明细号"];
            r["采购单号"] = drQ_通知单明细["采购单号"];
            r["采购明细号"] = drQ_通知单明细["采购单明细号"];
            //  r["操作员"] = strCurrUser;
            r["操作员"] = CPublic.Var.localUserName;

            r["不合格数量"] = txtBHG.Value;




            //增加的部分
            r["采购数量"] = drQ_通知单明细["采购数量"];
            r["税率"] = drQ_通知单明细["税率"];
            r["未税单价"] = drQ_通知单明细["未税单价"];
            r["单价"] = drQ_通知单明细["单价"];
            r["未税金额"] = drQ_通知单明细["未税金额"];
            r["金额"] = drQ_通知单明细["金额"];
            r["价格核实"] = drQ_通知单明细["价格核实"];
            r["是否急单"] = drQ_通知单明细["是否急单"];
            r["操作员ID"] = CPublic.Var.LocalUserID;
           
            r["产品名称"] = txtCPMC.Text;
            r["供应商编号"] = txtDWBH.Text;
            r["检验员ID"] = str_检验员ID;
            r["送检人ID"] = str_送检人ID;
            if (!bl_免检)
            {
                foreach (DataRow rp in dtP.Rows)
                {
                    rp["检验日期"] = CPublic.Var.getDatetime();
                    if (rp["允许上限"].ToString().Trim() != "")
                    {
                        if (rp["检验上限"].ToString().Trim() == "" || rp["检验下限"].ToString().Trim() == "")
                        {
                            throw new Exception("有检验上限或者下限未输入");

                        }

                    }

                   fun_合格判定();
                }
            }
            else
            {
                r["检验结果"] = "免检";

            }
            dtM.Rows[0].ItemArray = r.ItemArray;
        }
        private void fun_采购单检验状态()
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                string sql = string.Format("select * from 采购记录采购送检单明细表 where  采购单号='{0}' and 作废=0 ", dr["采购单号"]);
                string sql_s = string.Format("select * from 采购记录采购单明细表 where 采购单号 ='{0}'  and 作废=0 ", dr["采购单号"]);
                DataTable dt_s = CZMaster.MasterSQL.Get_DataTable(sql_s, CPublic.Var.strConn);

                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                //5-21 郭恒   
                if (Convert.ToBoolean(dt_s.Rows[0]["明细完成"])) //送检全部完成的前提
                {
                    int i = 0;
                    foreach (DataRow r in dt.Rows) //取所有送检单
                    {
                        if (r["检验完成"].Equals(true))
                        {
                            i++;
                            if (i== dt.Rows.Count) //5-21 郭恒 这里判断有问题 采购分批送检 检验分批检验 这个数量不一定相等 i== dt_s.Rows.Count 改为 i== dt.Rows.Count
                            {
                                string sql_1 = string.Format("select * from 采购记录采购单主表 where 采购单号='{0}'", r["采购单号"]);
                                using (SqlDataAdapter da = new SqlDataAdapter(sql_1, CPublic.Var.strConn))
                                {
                                    DataTable dt_1 = new DataTable();
                                    da.Fill(dt_1);
                                    dt_1.Rows[0]["已检验"] = 1;

                                    new SqlCommandBuilder(da);
                                    da.Update(dt_1);
                                }
                            }
                            continue;

                        }
                        else
                        {
                            break;
                        }

                    }
                    //19-5-21 郭恒 注释掉
                    //if (txtJYJG.Text.Trim() == "不合格")    // 17/4/25 为了处理 检验不合格 有部分可以通过评审单然后入库,另一部分在途量一直存在的问题，在途量刷新时 明细完成日期 is null 则会统计 
                    //{                                // 为了 不影响 上传了不合格评审单的 可以继续入库 所以把 所有 不合格的 明细完成日期 事先 赋上值
                    //    string s = string.Format("select * from 采购记录采购送检单明细表 where 采购单明细号='{0}'", dr["采购单明细号"].ToString());
                    //    DataTable t_c = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn);
                    //    if (t_c.Rows[0]["检验完成"].Equals(true))
                    //    {
                    //        DataRow[] rr = dt_s.Select(string.Format("采购明细号='{0}'", dr["采购单明细号"].ToString()));
                    //        rr[0]["明细完成日期"] = CPublic.Var.getDatetime();
                    //        string sql_check = string.Format("select * from [检验上传表单记录表] where 采购入库通知单号='{0}'", txt_通知单号.Text);
                    //        DataTable t = CZMaster.MasterSQL.Get_DataTable(sql_check, CPublic.Var.strConn);
                    //        if (t.Rows.Count == 0)
                    //        {
                    //            string sql_cgmx = "select * from 采购记录采购单明细表 where 1<>1";
                    //            using (SqlDataAdapter da = new SqlDataAdapter(sql_cgmx, CPublic.Var.strConn))
                    //            {
                    //                new SqlCommandBuilder(da);
                    //                da.Update(dt_s);
                    //            }
                    //        }
                    //    }
                    //}

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 样本数计算
        /// <summary>
        /// 抽检样本数计算
        /// </summary>
        /// <param name="dQty">批量数</param>
        /// <param name="strYYSP">检查标准</param>
        /// <param name="strAQL">SQL</param>
        /// <param name="iYBS">样本数</param>
        /// <param name="iAc">AC</param>
        /// <returns></returns>
        private void fun_抽检样本数计算(Decimal dQty, string strYYSP, string strAQL, ref int iYBS, ref int iAc)
        {
            try
            {
                if (dQty <= 1)
                {
                    iYBS = 1;
                    iAc = 0;
                    return;
                }
                string sql = string.Format("select  基础数据检验抽样表.* from 基础数据样本量字码表,基础数据检验抽样表 where " +
                    " 基础数据样本量字码表.样本量字码 = 基础数据检验抽样表.样本量字码 and 基础数据样本量字码表.下限 <= {0} and  " +
                    "基础数据样本量字码表.上限 >= {0} and 基础数据样本量字码表.检验水平 ='{1}' and " +
                    "基础数据检验抽样表.AQL = '{2}'", dQty, strYYSP, strAQL);

                DataRow drCPCY = MasterSQL.Get_DataRow(sql, Var.strConn);

                //iYBS = int.Parse(drCPCY["样本量"].ToString());
                if (drCPCY == null)
                {
                    iYBS = 0;
                    iAc = 0;
                }
                else
                {
                    iYBS = int.Parse(drCPCY["抽检样本量"].ToString());

                    iAc = (int)drCPCY["AC"];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// dtP列变化
        /// 检验记录采购件检验明细表 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dtP_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            try
            {
                if (e.Column.ColumnName == "不合格数量")
                {
                    fun_合格判定();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message) ;
            }
            
        }
        /*
        //void fun_合格判定()
        //{
        //    txtJYJG.Text = "通过";
        //    int iMaxBHG = 0;
        //    foreach (DataRow r in dr_传.Rows)
        //    {
        //        if (r.RowState == DataRowState.Deleted) continue;
        //        if (r["检验水平"].ToString() == "" && r["AQL"].ToString() == "")
        //        {
        //            continue;
        //        }

        //        try
        //        {
        //            r["合格"] = true;
        //            if ((int)r["不合格数量"] > (int)r["Ac"])
        //            {
        //                r["合格"] = false;
        //                txtJYJG.Text = "不通过";
        //            }

        //            //if (r["允许下限"].ToString() != "")
        //            //{

        //            //    Decimal d = CPublic.CConstrFun.GetNumber(r["允许下限"].ToString());
        //            //    Decimal d1 = CPublic.CConstrFun.GetNumber(r["检验下限"].ToString());
        //            //    if (d1 < d)
        //            //    {
        //            //        r["合格"] = false;
        //            //        txtJYJG.Text = "不通过";
        //            //    }

        //            //     d = CPublic.CConstrFun.GetNumber(r["允许上限"].ToString());
        //            //     d1 = CPublic.CConstrFun.GetNumber(r["检验上限"].ToString());
        //            //    if (d1 > d)
        //            //    {
        //            //        r["合格"] = false;
        //            //        txtJYJG.Text = "不通过";
        //            //    }
        //            //}
                   
        //            if ((int)r["不合格数量"] > iMaxBHG)
        //            {
        //                iMaxBHG = (int)r["不合格数量"];
        //            }
        //        }
        //        catch { }
        //    }
        //    try
        //    {
        //        int.Parse(txtBHG.Text);
        //    }
        //    catch { txtBHG.Value = 0; }
        //    //if (iMaxBHG > txtBHG.Value)
        //    //{
        //        txtBHG.Text = iMaxBHG.ToString();
        //    //}
        //}
        */

        void fun_合格判定()
        {

            txtJYJG.Text = "合格";
            int iMaxBHG = 0;
            //int iMaxBHG = 0;
            //try
            //{
            //    iMaxBHG = int.Parse(txtBHG.Text);
            //}
            //catch { iMaxBHG = 0; }

            try

            {
                int num_QJ = 0;
                //int sumnum = 0;
                foreach (DataRow r in dtP.Rows)
                {
                    //try
                    //{
                    //    Convert.ToInt32(r["不合格数量"]);
                    //}
                    //catch { continue; }
                    if (r["检验水平"].ToString() != "" && r["AQL"].ToString() != "")
                    {
                        r["合格"] = true;
                        if ((int)r["不合格数量"] > (int)r["Ac"])
                        {
                            r["合格"] = false;
                            
                        }
                        if ((int)r["不合格数量"] > iMaxBHG)
                        {
                            iMaxBHG = (int)r["不合格数量"];
                           
                        }
                    }
                    if (r["检验水平"].ToString() == "" && r["AQL"].ToString() == "")
                    {
                        r["合格"] = true;

                        if ((int)r["不合格数量"] > 0)
                        {
                            r["合格"] = false;
                            txtJYJG.Text = "不合格";
                        }
                        if ((int)r["不合格数量"] > iMaxBHG)
                        {
                            iMaxBHG = (int)r["不合格数量"];
                        }
                    }
                    if (r["检验水平"].ToString() == "全检")
                    {
                        r["合格"] = true;
                        double db = Convert.ToDouble(r["抽检数"]) * 0.3;
                        if ((int)r["不合格数量"] > db)
                        {
                            r["合格"] = false;
                            txtJYJG.Text = "不合格";

                            //r["合格"] = true;
                            //txtJYJG.Text = "合格";
                        }
                        if ((int)r["不合格数量"] > iMaxBHG)
                        {
                            iMaxBHG = (int)r["不合格数量"];
                        }
                        num_QJ = (int)r["不合格数量"];
                        if (num_QJ > iMaxBHG)
                        {
                            iMaxBHG = (int)r["不合格数量"];
                            
                        }
                        
                    }
                    //if (iMaxBHG > Convert.ToInt32(r["抽检数"]))
                    //{
                    //    throw new Exception("不合格数超出抽检数");
                    //}
                    // sumnum += Convert.ToInt32(r["不合格数量"]);
                    //num_QJ = iMaxBHG;
                }
                //DataRow r = gvM.GetDataRow(gvM.FocusedRowHandle);
                // txtBHG.Text = sumnum.ToString();
                txtBHG.Text = iMaxBHG.ToString();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            //try
            //{
            //    int.Parse(txtBHG.Text);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    txtBHG.Value = 0;
            //}
            
        }
        #endregion

        #region 新增并清空数据
        private void fun_清空并新增数据()
        {
            if (txtItem.Text == "" && txtJYDH.Text != "")
            {
                return;
            }
            string tzdh = txt_通知单号.Text;
            string JYY = this.txtJYY.Text;
            string SJY = this.txtSJY.Text;
            string sql = "select * from 采购记录采购单检验主表 where 1<>1 ";
            dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);

            dtM.Rows.Add(dtM.NewRow());
            DBH1.DataFormDR(dtM.Rows[0]);

            txtJYY.Text = JYY;
            txtSJY.Text = SJY;

            sql = "select * from 采购记录采购单检验明细表 where 1<>1 ";
            dtP = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
            gcP.DataSource = dtP;
            string ss = CPublic.Var.getDatetime().Year.ToString().Substring(2, 2);
            strJYDDH = string.Format("IC{0}{1:00}{2:00}{3:0000}", ss, CPublic.Var.getDatetime().Month, CPublic.Var.getDatetime().Day, CPublic.CNo.fun_得到最大流水号("IC", CPublic.Var.getDatetime().Year, CPublic.Var.getDatetime().Month));
            txtJYDH.Text = strJYDDH;
            txt_通知单号.Text = tzdh;
        }
        #endregion

        #region 打印


        private void fun_Print()
        {
            if (txtJYJG.Text == "")
            {
                return;
            }
            //if (txtJYJG.Text == "通过")
            //{
            //    print_Check.fun_print_Check(strJYDDH);
            //    return;
            //}
            //if (txtJYJG.Text == "不通过")
            //{
            print_Check.fun_print_Check(strJYDDH);
            //print_Unqualified.fun_print_Unqualified(strJYDDH);
            return;
            //}
        }
        #endregion

        #region 界面操作事件
        /// <summary>
        /// 入库通知单号回车
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txttzdh_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ItemInspectionData.frm产品检验数据维护.str = "";
                bt查找_Click(null, null);
            }
        }

        /// <summary>
        /// 入库通知单号查询按钮响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt查找_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    ItemInspectionData.frm产品检验数据维护.str = "";
                }
                txt_通知单号.Text = txt_通知单号.Text.Trim();
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (txt_通知单号.Text.ToString() != "" && dtP!=null)
                {
                    // fun_加载入库通知单(txt_通知单号.Text);
                    fun_加载入库通知单(dr["送检单明细号"].ToString(), dr["送检单号"].ToString());
                    fun_加载产品明细(txtItem.Text);
                    fun_加载样本数();
                    strRKTZD = txt_通知单号.Text;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 送检数量查询按钮响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            fun_加载样本数();
        }
        /// <summary>
        /// 送检数量回车
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSJSL_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                simpleButton1_Click(null, null);
            }
        }

        #region 人员选择相关
        string str_送检人ID = "";
        string str_检验员ID = "";
        /// <summary>
        /// 检验员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            // string sql = "select 员工号 as 员工编号,姓名 as 员工姓名 from 人事基础员工表 where 部门='品质部'  and 在职状态 ='在职' ";
            string sql = "select 员工号 as 员工编号,姓名 as 员工姓名 from 人事基础员工表 where  在职状态 ='在职' ";

            fmDataSelect fm = new fmDataSelect("请选择员工", "员工姓名", sql, CPublic.Var.strConn);
            if (fm.ShowDialog() == DialogResult.OK)
            {
                txtJYY.Text = fm.strResult;
                str_检验员ID = fm.dtResult.Select(string.Format("员工姓名 = '{0}'", txtJYY.Text))[0]["员工编号"].ToString();
                if (txtJYY.Items.IndexOf(fm.strResult) == -1)
                {
                    txtJYY.Items.Add(fm.strResult);
                }
            }
        }
        /// <summary>
        /// 送检员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "select 员工号 as 员工编号,姓名 as 员工姓名 from 人事基础员工表";
                fmDataSelect fm = new fmDataSelect("请选择员工", "员工姓名", sql, CPublic.Var.strConn);
                if (fm.ShowDialog() == DialogResult.OK)
                {
                    txtSJY.Text = fm.strResult;
                    str_送检人ID = fm.dtResult.Select(string.Format("员工姓名 = '{0}'", txtJYY.Text))[0]["员工编号"].ToString();
                    if (txtSJY.Items.IndexOf(fm.strResult) == -1)
                    {
                        txtSJY.Items.Add(fm.strResult);
                    }
                }
            }
            catch { }
        }
        /// <summary>
        /// 操作员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string sql = "select 员工号 as 员工编号,姓名 as 员工姓名 from 人事基础员工表";
            fmDataSelect fm = new fmDataSelect("请选择员工", "员工姓名", sql, CPublic.Var.strConn);
            if (fm.ShowDialog() == DialogResult.OK)
            {
                strCurrUser = fm.strResult;
                barStaticItem1.Caption = string.Format("操作员:{0} ", strCurrUser);

            }
            fun_加载人员下拉框数据();
        }

        #endregion

        #endregion

        /// <summary>
        /// 新增按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_清空并新增数据();
        }

        /// <summary>
        /// 刷新按钮响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            textBox1.Text = null;
            frm采购件检验记录 frm = new frm采购件检验记录();
            UIcontrol.Showpage(frm, "来料检验");

            //try
            //{
            //    strJYDDH = "";

            //    frm采购件检验记录_Load(null, null);
            //    //ItemInspectionData.frm产品检验数据维护.str = "";
            //    bt查找_Click(null, null);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

        }

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gvM.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                this.ActiveControl = null;
                int i = gv.FocusedRowHandle;
                if (strCurrUser == "")
                {
                    throw new Exception("没有选择操作员");
                }
                if (MessageBox.Show("检验结果是否确认？", "保存提醒？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    //因为检查比较复杂。所以我这里执行二次。第一次有可能数据设置完了还有新的检查会不通过。
                    decimal dec_不合格数量 = txtBHG.Value;
                    
                    fun_保存前检查及处理数据(dec_不合格数量);
                    //fun_保存前检查及处理数据();
                    fun_保存人员下拉框数据();
                    //return;

                    fun_保存数据(dec_不合格数量);
                    // 打印先去掉
                    //Thread thDo;
                    //thDo = new Thread(Dowork);
                    ////Dowork();
                    //thDo.IsBackground = true;
                    //thDo.Start();

                    gv.FocusedRowHandle = i;
                    //string strDefaultPrinter = new PrintDocument().PrinterSettings.PrinterName;

                    //if (DialogResult.OK == MessageBox.Show("请问是否打印？", "提示", MessageBoxButtons.OKCancel))
                    //{
                    //    if (DialogResult.OK == MessageBox.Show(strDefaultPrinter, "打印机确认？", MessageBoxButtons.OKCancel))
                    //    {
                    //        fun_Print();
                    //    }
                    //}

                    {
                        string sql = string.Format("select * from 采购记录采购检验默认人员表 where 物料编码 = '{0}'", txtItem.Text.Trim());
                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(sql, strWLConn);
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            if (dt.Rows[0]["默认检验员"].ToString() == txtJYY.Text)
                            {

                            }
                            else
                            {
                                dt.Rows[0]["默认检验员"] = txtJYY.Text;


                                new SqlCommandBuilder(da);
                                da.Update(dt);
                            }
                        }
                        else
                        {
                            DataRow dr = dt.NewRow();
                            dt.Rows.Add(dr);
                            dr["物料编码"] = txtItem.Text;
                            dr["物料名称"] = txtCPMC.Text;
                            dr["默认检验员"] = txtJYY.Text;

                            new SqlCommandBuilder(da);
                            da.Update(dt);
                        }
                    }

                    fun_清空并新增数据();
                    MessageBox.Show("OK");
                }
               
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void Dowork()
        {
            DataTable dt_dy = dtM.Copy();
            dt_dy.Columns["产品名称"].ColumnName = "物料名称";

            ItemInspection.print_FMS.fun_P_采购入库通知单(dt_dy);
        }
        /// <summary>
        /// 打印按钮响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string strDefaultPrinter = new PrintDocument().PrinterSettings.PrinterName;
            try
            {
                if (DialogResult.OK == MessageBox.Show(strDefaultPrinter, "打印机确认？", MessageBoxButtons.OKCancel))
                {
                    fun_Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// excel导出按钮响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                //gridControl1.ExportToXls(saveFileDialog.FileName, options);  
               
                    gc.ExportToXlsx(saveFileDialog.FileName);
                
                    
                
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 表单按钮响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (fm表单 fm = new fm表单())
            {
                fm.strTZJno = txt_通知单号.Text;
                fm.strCPBM = txtItem.Text;
                if (fm.ShowDialog() != DialogResult.OK)
                {
                    fm.Close();
                    return;
                }
            }
        }
        /// <summary>
        /// 跳转按钮响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ItemInspectionData.frm产品检验数据维护 fm = new ItemInspectionData.frm产品检验数据维护();
            ItemInspectionData.frm产品检验数据维护.str = this.txtItem.Text;
            //UIcontrol.AddNewPage(fm, "物料检验项设定");

            int f_fm = 0;
            foreach (XtraTabPage x in (this.Parent.Parent as XtraTabControl).TabPages)
            {
                foreach (Control c in x.Controls)
                {
                    if (c.GetType() == fm.GetType())
                    {
                        x.Controls[0].Dispose();
                        (this.Parent.Parent as XtraTabControl).TabPages.Remove(x);
                        x.Dispose();
                        f_fm = 1;
                        break;
                    }
                }
                if (f_fm == 1)
                {
                    break;
                }
            }
            UIcontrol.AddNewPage(fm, "物料检验项设定");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ItemInspectionData.frm产品检验数据维护.f_Change == false && txt_通知单号.Text != "" && txtItem.Text.Trim() == ItemInspectionData.frm产品检验数据维护.str)
            {
                timer1.Enabled = false;
                //if (MessageBox.Show("数据已修改是否更新", "提醒", MessageBoxButtons.OKCancel) == DialogResult.OK)
                //{
                bt查找_Click(null, null);
                //}
                ItemInspectionData.frm产品检验数据维护.f_Change = true;
            }
            timer1.Enabled = true;
        }

        private void repositoryItemCalcEdit1_Click(object sender, EventArgs e)
        {
            ((DevExpress.XtraEditors.CalcEdit)(sender)).Text = ((DevExpress.XtraEditors.CalcEdit)(sender)).Value.ToString();
        }

        #region 特殊按钮

        //private void button1_MouseMove(object sender, MouseEventArgs e)
        //{
        //    foreach (Control item in this.Controls)
        //    {
        //        fum_(item);
        //    }
        //}

        //private void fum_(Control item)
        //{
        //    foreach (Control c in item.Controls)
        //    {
        //        if (c.Focused == true)
        //        {

        //            c.Text += "+";
        //        }
        //        if (c.Focused == false)
        //        {
        //            fum_(c);
        //        }
        //    }
        //}

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


        //双击弹出窗体
        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                //    if (e.Clicks == 2)
                //    {
                //        DataRow r = gv.GetDataRow(e.RowHandle);
                //        txt_通知单号.Text = r["送检单号"].ToString();
                //        bt查找_Click(null, null);
                //    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

 

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            fun_load采购送检单();
        }

        //撤销 送检单 
        private void barLargeButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            if (dr != null)
            {
                string sql_zb = string.Format("select  * from 采购记录采购送检单主表 where  送检单号='{0}'", dr["送检单号"]);
                string sql_mx = string.Format("select  * from 采购记录采购送检单明细表 where  送检单号='{0}'", dr["送检单号"]);

                DataTable dt_zb = new DataTable();
                DataTable dt_mx = new DataTable();
                dt_zb = CZMaster.MasterSQL.Get_DataTable(sql_zb, CPublic.Var.strConn);
                dt_mx = CZMaster.MasterSQL.Get_DataTable(sql_mx, CPublic.Var.strConn);
                if (dt_zb.Rows.Count > 0 && dt_mx.Rows.Count > 0)
                {
                    dt_mx.Rows[0].Delete();
                    dt_zb.Rows[0].Delete();
                }
                string sql_采购明细 = string.Format("select * from  采购记录采购单明细表 where  采购明细号='{0}' ", dr["采购单明细号"]);
                DataTable dt_采购明细 = new DataTable();
                dt_采购明细 = CZMaster.MasterSQL.Get_DataTable(sql_采购明细, CPublic.Var.strConn);
                if (dt_采购明细.Rows.Count > 0)
                {
                    dt_采购明细.Rows[0]["明细完成"] = 0;
                    dt_采购明细.Rows[0]["已送检数"] = Convert.ToDecimal(dt_采购明细.Rows[0]["已送检数"]) - Convert.ToDecimal(dr["送检数量"]);
                }

                sql_采购明细 = "select  * from 采购记录采购单明细表 where 1<>1 ";
                using (SqlDataAdapter da = new SqlDataAdapter(sql_采购明细, CPublic.Var.strConn))
                {
                    new SqlCommandBuilder(da);

                    da.Update(dt_采购明细);
                }
                sql_zb = "select * from 采购记录采购送检单主表 where 1<>1";
                sql_mx = "select * from 采购记录采购送检单明细表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql_zb, CPublic.Var.strConn))
                {
                    new SqlCommandBuilder(da);

                    da.Update(dt_zb);

                }
                using (SqlDataAdapter da = new SqlDataAdapter(sql_mx, CPublic.Var.strConn))
                {
                    new SqlCommandBuilder(da);

                    da.Update(dt_mx);

                }
                MessageBox.Show("已撤销");
                strJYDDH = "";
                frm采购件检验记录_Load(null, null);
                bt查找_Click(null, null);

            }

        }

        //private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        //{
        //    DataRow r = gv.GetDataRow(e.RowHandle);
        //    txt_通知单号.Text = r["送检单号"].ToString();
        //    checkBox4.Checked = false;
        //    bt查找_Click(null, null);
        //}

        private void gv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv.GetFocusedRowCellValue(gv.FocusedColumn));
                e.Handled = true;
            }
        }

        private void gvM_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.Caption == "不合格数")
            {
                fun_合格判定();
            }
        }



        private void barLargeButtonItem12_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            try
            {
              //  DataRow dr = gv.GetDataRow(gv.FocusedRowHandle); dr["物料编码]"  
                string strConn_FS = CPublic.Var.geConn("FS");
                string sql = string.Format(@"select * from [基础物料蓝图表] where 物料号='{0}'  and   版本 = (select MAX(版本)from [基础物料蓝图表] where  物料号='{0}' )", txtItem.Text.ToString());
                DataRow rr = CZMaster.MasterSQL.Get_DataRow(sql, strWLConn);

                if (rr == null || rr["文件地址"] == null || rr["文件地址"].ToString() == "")
                {
                    throw new Exception("未上传文件,没有文件可以查看");
                }
                string type = rr["后缀"].ToString();
                string foldPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                //     string fileName = foldPath + DateTime.Now.ToString("yyyy-MM-dd") + "T" + DateTime.Now.ToString("HH_mm_ss") + "Z" + "_" + Guid.NewGuid().ToString() + "." + type;
                string fileName = foldPath + "预览." + type;
                try
                {
                    System.IO.Directory.Delete(foldPath, true);
                }
                catch (Exception)
                {

                }
                CFileTransmission.CFileClient.strCONN = strConn_FS;
                CFileTransmission.CFileClient.Receiver(rr["文件地址"].ToString(), fileName);

                //ui预览文件 ui = new ui预览文件(fileName);
                //UIcontrol.Showpage(ui, "预览文件");
                System.Diagnostics.Process.Start(fileName);




                //axAcroPDF1.Visible = true;
                //axAcroPDF1.LoadFile(fileName);
                //axAcroPDF1.setView("readonly");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtJYY_TextChanged(object sender, EventArgs e)
        {
            string sql = string.Format("select 员工号,姓名 from 人事基础员工表 where 姓名='{0}'", txtJYY.Text);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    str_检验员ID = dt.Rows[0]["员工号"].ToString();
                }
            }
        }
        //检验标准
        private void simpleButton6_Click(object sender, EventArgs e)
        {

            try
            {
                string s = string.Format(@"9 ", txtItem.Text.Trim());

                DataRow dr = CZMaster.MasterSQL.Get_DataRow(s, strWLConn);
                if (dr == null)
                {
                    throw new Exception("未上传文件,没有文件可以查看");
                }
                string sql = string.Format(@"select * from [品质检验标准文件表] where 物料号='{0}' and 后缀='pdf'", dr["物料编码"]);
                DataRow rr = CZMaster.MasterSQL.Get_DataRow(sql, strWLConn);
                if (rr == null || rr["文件地址"] == null || rr["文件地址"].ToString() == "")
                {
                    throw new Exception("未上传文件,没有文件可以查看");
                }
                string type = rr["后缀"].ToString();
                string foldPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                //     string fileName = foldPath + DateTime.Now.ToString("yyyy-MM-dd") + "T" + DateTime.Now.ToString("HH_mm_ss") + "Z" + "_" + Guid.NewGuid().ToString() + "." + type;
                string fileName = foldPath + "预览." + type;
                try
                {
                    System.IO.Directory.Delete(foldPath, true);
                }
                catch (Exception)
                {

                }
                CFileTransmission.CFileClient.strCONN = strConn_FS;
                CFileTransmission.CFileClient.Receiver(rr["文件地址"].ToString(), fileName);

                ui预览文件 ui = new ui预览文件(fileName);
                UIcontrol.Showpage(ui, "检验标准");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //更新检验标准
        private void simpleButton7_Click(object sender, EventArgs e)
        {
            try
            {

                string root = System.Windows.Forms.Application.StartupPath + "\\品质检验标准\\";
                DirectoryInfo rt = new DirectoryInfo(root);

                if (!rt.Exists) rt.Create();


                string s = string.Format(@"select   b.物料编码,b.图纸编号,b.物料名称,b.物料编码,大类,小类 from 基础数据物料信息表  b
                 where  物料编码='{0}' ", txtItem.Text.Trim());
                // left join  采购记录采购单检验主表 a   on a.产品编号=b.物料编码   
                //and 检验日期>'2017-1-1'      2018-3-8 修改
                DataRow dr = CZMaster.MasterSQL.Get_DataRow(s, strWLConn);
                //if (dr == null)
                //{
                //    throw new Exception("没有该物料检验记录,无法更新检验标准");
                //}
                string strConn_FS = CPublic.Var.geConn("FS");
                string sql = string.Format(@"select * from [品质检验标准文件表] where 物料号='{0}' and 后缀='pdf'", dr["物料编码"]);
                DataRow rr = CZMaster.MasterSQL.Get_DataRow(sql, strWLConn);
                bool bl = false;
                if (rr == null || rr["文件地址"] == null || rr["文件地址"].ToString() == "")
                {
                    if (MessageBox.Show(string.Format("是否确认上传物料{0}的检验标准？", dr["物料编码"]), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        bl = true;
                    }
                }
                else
                {
                    if (MessageBox.Show(string.Format("是否确认覆盖物料{0}的检验标准？", dr["物料编码"]), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        #region 先删除原有文件 原有记录
                        sql = string.Format(@"select * from [品质检验标准文件表] where 物料号='{0}' and 后缀='xlsx'", dr["物料编码"]);
                        DataRow rrr = CZMaster.MasterSQL.Get_DataRow(sql, strWLConn);
                        try
                        {
                            CFileTransmission.CFileClient.deleteFile(rr["文件地址"].ToString()); //服务器删除对应pdf 文件 
                            CFileTransmission.CFileClient.deleteFile(rrr["文件地址"].ToString());//服务器删除对应xlsx 文件
                            sql = string.Format("delete [品质检验标准文件表] where 物料号='{0}' ", dr["物料编码"]);
                            CZMaster.MasterSQL.ExecuteSQL(sql, strWLConn);
                        }
                        catch { } //throw new Exception ("出错了,请重试")；

                        #endregion
                        bl = true;
                    }

                }
                if (bl)
                {
                    //取需打印数据,存成EXCEl 上传
                    //                    string ss = string.Format(@"select  a.检验记录单号,base.物料编码,base.小类,base.物料名称,base.图纸编号,s.物料编码 as 父项编号,s.物料名称 as 父项名称,s.n原ERP规格型号 as 父项规格,mx.* from 采购记录采购单检验主表 a
                    //                left join 基础数据物料信息表 base   on a.产品编号=base.物料编码  
                    //                left join  (select   max(产品编码)父项编码,子项编码 from  基础数据物料BOM表 group by 子项编码)b on b.子项编码=a.产品编号
                    //                left join 基础数据物料信息表 s on s.物料编码=b.父项编码
                    //                left join 采购记录采购单检验明细表 mx on mx.检验记录单号=a.检验记录单号
                    //                where 产品编号='{0}' and left(检验项目,2)='尺寸' and 检验日期=(select  MAX(检验日期) from 采购记录采购单检验主表 b where b.产品编号 =a.产品编号)", txtItem.Text.Trim());
                    string ss = string.Format(@"select  a.*,s.物料编码 as 父项编号,s.物料名称 as 父项名称,s.n原ERP规格型号 as 父项规格  from  [基础数据物料检验要求表]  a
                left join 基础数据物料信息表 base   on a.产品编码=base.物料编码  
                left join  (select   max(产品编码)父项编码,子项编码 from  基础数据物料BOM表 group by 子项编码)b on b.子项编码=a.产品编码
                left join 基础数据物料信息表 s on s.物料编码=b.父项编码
                 where a.产品编码='{0}' and left(a.检验项目,2)='尺寸'", txtItem.Text.Trim());

                    using (SqlDataAdapter da = new SqlDataAdapter(ss, strWLConn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        //调用生成文件函数  参数 dt,dr
                        string s_文件编号 = ItemInspection.print_FMS.fun_生成检验标准(dt, dr);
                        //文件名: 物料编码+"-"+小类
                        // root = System.Windows.Forms.Application.StartupPath + "\\品质检验标准\\";
                        string path = root + dr["物料编码"].ToString() + "_" + dr["小类"].ToString() + ".xlsx";
                        //CFileTransmission.CFileClient.sendFile();
                        fun_文件上传(s_文件编号, path, dr);
                        string path2 = root + dr["物料编码"].ToString() + "_" + dr["小类"].ToString() + ".pdf";

                        ERPorg.Corg x = new ERPorg.Corg();
                        x.ConverterToPdf(path, path2);
                        fun_文件上传(s_文件编号, path2, dr);
                        x.DeleteFolder(root);

                    }

                    MessageBox.Show("更新成功");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 文件上传的方法
        /// </summary>
        private void fun_文件上传(string s_文件编号, string pathName, DataRow r)
        {
            //判定上传文件的大小

            FileInfo info = new FileInfo(pathName);
            long maxlength = info.Length;

            if (maxlength > 1024 * 1024 * 8)
            {
                throw new Exception("上传的文件不能超过1M，请重新选择后再上传！");
            }

            MasterFileService.strWSDL = CPublic.Var.strWSConn;
            CFileTransmission.CFileClient.strCONN = strConn_FS;
            string strguid = "";  //记录系统自动返回的GUID

            strguid = CFileTransmission.CFileClient.sendFile(pathName);
            string type = "";

            int s = pathName.LastIndexOf(".") + 1;
            type = pathName.Substring(s, pathName.Length - s);
            string sql = "select  * from  [品质检验标准文件表] where 1<>1 ";
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strWLConn);
            DataRow dr = dt.NewRow();
            dr["物料号"] = r["物料编码"];
            dr["文件名"] = Path.GetFileName(pathName); ;
            dr["后缀"] = type;
            dr["文件地址"] = strguid;
            dr["小类"] = r["小类"];
            dr["文件编号"] = s_文件编号;

            dt.Rows.Add(dr);

            CZMaster.MasterSQL.Save_DataTable(dt, "品质检验标准文件表", strWLConn);




            //    dtP.Rows.Add(strygh, r["文件名称"].ToString(), strguid, Path.GetFileName(pathName));
            // MasterSQL.Save_DataTable(dtP, "人事基础员工文件表", CPublic.Var.strConn);
        }
        //上传检验标准
        private void simpleButton8_Click(object sender, EventArgs e)
        {

            try
            {
                string root = System.Windows.Forms.Application.StartupPath + "\\品质检验标准\\";
                DirectoryInfo rt = new DirectoryInfo(root);

                if (!rt.Exists) rt.Create();
                string s = string.Format(@"select   b.物料编码,b.图纸编号,b.物料名称,b.物料编码,大类,小类 from 基础数据物料信息表  b
                 where  物料编码='{0}' ", txtItem.Text.Trim());
                // left join  采购记录采购单检验主表 a   on a.产品编号=b.物料编码   
                //and 检验日期>'2017-1-1'      2018-3-8 修改
                DataRow dr = CZMaster.MasterSQL.Get_DataRow(s, strWLConn);
                string strConn_FS = CPublic.Var.geConn("FS");
                string sql = string.Format(@"select * from [品质检验标准文件表] where 物料号='{0}' and 后缀='pdf'", dr["物料编码"]);
                DataRow rr = CZMaster.MasterSQL.Get_DataRow(sql, strWLConn);
                bool bl = false;
                if (rr == null || rr["文件地址"] == null || rr["文件地址"].ToString() == "")
                {
                    if (MessageBox.Show(string.Format("是否确认上传物料{0}的检验标准？", dr["物料编码"]), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        bl = true;
                    }
                }
                else
                {
                    if (MessageBox.Show(string.Format("是否确认覆盖物料{0}的检验标准？", dr["物料编码"]), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        #region 先删除原有文件 原有记录
                        sql = string.Format(@"select * from [品质检验标准文件表] where 物料号='{0}' and 后缀='xlsx'", dr["物料编码"]);
                        DataRow rrr = CZMaster.MasterSQL.Get_DataRow(sql, strWLConn);
                        try
                        {
                            CFileTransmission.CFileClient.deleteFile(rr["文件地址"].ToString()); //服务器删除对应pdf 文件 
                            CFileTransmission.CFileClient.deleteFile(rrr["文件地址"].ToString());//服务器删除对应xlsx 文件
                            sql = string.Format("delete [品质检验标准文件表] where 物料号='{0}' ", dr["物料编码"]);
                            CZMaster.MasterSQL.ExecuteSQL(sql, strWLConn);
                        }
                        catch { } //throw new Exception ("出错了,请重试")；

                        #endregion

                        bl = true;
                    }
                }

                if (bl)
                {

                    string path = "";
                    OpenFileDialog openfile = new OpenFileDialog();
                    if (openfile.ShowDialog() == DialogResult.OK)
                    {
                        path = openfile.FileName;
                    }
                    from1 fm = new from1();
                    fm.ShowDialog();
                    fm.StartPosition = FormStartPosition.CenterScreen;
                    if (fm.flag)
                    {
                        // fm.str_文件编号

            
                   
               
                            fun_文件上传(fm.str_文件编号, path, dr);
                            string path2 = root + dr["物料编码"].ToString() + "_" + dr["小类"].ToString() + ".pdf";
                            ERPorg.Corg x = new ERPorg.Corg();
                            x.ConverterToPdf(path, path2);
                            fun_文件上传(fm.str_文件编号, path2, dr);
                            x.DeleteFolder(root);
                         
                       
                        MessageBox.Show("更新成功");
                    }
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        //保存dw 数据
        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text != "")
                {
                    fun_判断到货单是否存在();
                    fun_查找送检单1();

                    if (dt_到货单是否存在.Rows.Count == 0 && dt_DW.Rows.Count > 0)
                    {
                      
                        
                        //  fun_判断物料是否已保存于表中();

                        fun_保存送检单1();

                        fun_保存供应商信息();


                        fun_保存基础数据表();


                        fun_save_DW();
                        fun_load采购送检单();
                        textBox1.Text = null;
                    }
                    else
                    {
                        //到货单数量是否有变，变了的话，改变一下数量
                        if (dt_到货单是否存在.Rows.Count > 0)
                        {
                            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                            DialogResult a = MessageBox.Show("到货单已经录入，确定要变更送检物料总数吗?（注：同批采购单，有相同物料不要更新）", "更新系统", messButton);
                            if (a == DialogResult.OK)
                            {

                                string str = "select * from 采购记录采购送检单明细表 where 备注2 ='" + "DWDH20" + textBox1.Text.ToString() + "'";
                                using (SqlDataAdapter da = new SqlDataAdapter(str, strconn1))
                                {
                                    DataTable dt_更新数量 = new DataTable();
                                    da.Fill(dt_更新数量);

                                    foreach (DataRow dr in dt_更新数量.Rows)
                                    {
                                        DataRow[] dr1 = dt_DW.Select("cInvCode ='" + dr["物料编码"].ToString() + "'");

                                        if (Convert.ToDecimal(dr["送检数量"]) != Convert.ToDecimal(dr1[0]["iQuantity"]))
                                        {
                                            dr["送检数量"] = Convert.ToDecimal(dr1[0]["iQuantity"]);
                                        }

                                    }
                                    new SqlCommandBuilder(da);
                                    da.Update(dt_更新数量);
                                    MessageBox.Show("更新成功");

                                }
                            }
                            else
                            {
                                return;
                            }
                        }

                        else
                        {
                            MessageBox.Show("请正确输入到货单");
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_RowCellClick_1(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                barLargeButtonItem2.Enabled = true;
                barLargeButtonItem5.Enabled = true;
                DataRow r = gv.GetDataRow(e.RowHandle);
                txt_通知单号.Text = r["送检单号"].ToString();
                checkBox4.Checked = false;
                bt查找_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void gv_ColumnPositionChanged(object sender, EventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gv.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        private void gv_ColumnFilterChanged(object sender, EventArgs e)
        {
            try
            {
                if (cfgfilepath != "")
                {
                    gv.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        private void gv_CustomDrawCell_1(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            if (gv.GetRowCellValue(e.RowHandle, "送检单号").ToString() == dr["送检单号"].ToString())
            {
                e.Appearance.BackColor = Color.LightBlue;
                //e.Appearance.BackColor2 = Color.LightBlue;
            }

            if (gv.GetRowCellValue(e.RowHandle, "是否急单").Equals(true))
            {
                e.Appearance.BackColor = Color.Pink;
                //e.Appearance.BackColor2 = Color.LightBlue;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)//如果输入的是回车键
                {
                    if (textBox1.Text != "")
                    {
                        fun_判断到货单是否存在();
                        fun_查找送检单1();

                        if (dt_到货单是否存在.Rows.Count == 0 && dt_DW.Rows.Count > 0)
                        {


                            //  fun_判断物料是否已保存于表中();

                            fun_保存送检单1();

                            fun_保存供应商信息();


                            fun_保存基础数据表();


                            fun_save_DW();
                            fun_load采购送检单();
                            textBox1.Text = null;
                        }
                        else
                        {
                            //到货单数量是否有变，变了的话，改变一下数量
                            if (dt_到货单是否存在.Rows.Count > 0)
                            {
                                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                                DialogResult a = MessageBox.Show("到货单已经录入，确定要变更送检物料总数吗?（注：同批采购单，有相同物料不要更新）", "更新系统", messButton);
                                if (a == DialogResult.OK)
                                {

                                    string str = "select * from 采购记录采购送检单明细表 where 备注2 ='" + "DWDH20" + textBox1.Text.ToString() + "'";
                                    using (SqlDataAdapter da = new SqlDataAdapter(str, strconn1))
                                    {
                                        DataTable dt_更新数量 = new DataTable();
                                        da.Fill(dt_更新数量);

                                        foreach (DataRow dr in dt_更新数量.Rows)
                                        {
                                            DataRow[] dr1 = dt_DW.Select("cInvCode ='" + dr["物料编码"].ToString() + "'");

                                            if (Convert.ToDecimal(dr["送检数量"]) != Convert.ToDecimal(dr1[0]["iQuantity"]))
                                            {
                                                dr["送检数量"] = Convert.ToDecimal(dr1[0]["iQuantity"]);
                                            }

                                        }
                                        new SqlCommandBuilder(da);
                                        da.Update(dt_更新数量);
                                        MessageBox.Show("更新成功");

                                    }
                                }
                                else
                                {
                                    return;
                                }
                            }

                            else
                            {
                                MessageBox.Show("请正确输入到货单");
                                return;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                barLargeButtonItem2.Enabled = true;
                barLargeButtonItem5.Enabled = true;
                DataRow r = gv.GetDataRow(gv.FocusedRowHandle);
                txt_通知单号.Text = r["送检单号"].ToString();
                checkBox4.Checked = false;
                bt查找_Click(null, null);
            }
            catch  
            {

              
            }
            
        }

        private void txtSJSL_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //判断按键是不是要输入的类型。
                if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                    e.Handled = true;

                //小数点的处理。
                if ((int)e.KeyChar == 46)                           //小数点
                {
                    if (txtSJSL.Text.Length <= 0)
                        e.Handled = true;   //小数点不能在第一位
                    else
                    {
                        float f;
                        float oldf;
                        bool b1 = false, b2 = false;
                        b1 = float.TryParse(txtSJSL.Text, out oldf);
                        b2 = float.TryParse(txtSJSL.Text + e.KeyChar.ToString(), out f);
                        if (b2 == false)
                        {
                            if (b1 == true)
                                e.Handled = true;
                            else
                                e.Handled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
    }
}
