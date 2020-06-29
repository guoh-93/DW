using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO;
using DevExpress.XtraPrinting;

namespace ERPStock
{
    public partial class frm仓库盘点明细 : UserControl
    {
        #region 变量
        //取一个制造部部长
        DataRow r_mm;
        DataTable dt_ndtomod;
        string str_盘点批次号;
        DataTable dtM;
        DataTable dt_盘点出入库明细;

        DataTable dt_其他出申请主表;
        DataTable dt_其他出申请子表;
        DataTable dt_其他出主表;
        DataTable dt_其他出子表;
        DataTable dt_其他入主表;
        DataTable dt_其他入子表;
        DataTable dt_出入库明细;

        DataTable dt_下拉仓库;
        DataTable dt_1;//存放仓库物料盘点表
        string strconn = CPublic.Var.strConn;
        DataTable dt_库存;
        DataTable dt_异常;

        #endregion

        #region 加载
        public frm仓库盘点明细()
        {
            InitializeComponent();
            this.str_盘点批次号 = "";
            barEditItem1.EditValue = "差异不为0数据";
        }

        public frm仓库盘点明细(string s)
        {
            this.str_盘点批次号 = s;
            InitializeComponent();


        }
        private void frm仓库盘点明细_Load(object sender, EventArgs e)
        {
            try
            {
                string s = "select  员工号,姓名 from 人事基础员工表 where 部门= '制造部' and  在职状态='在职' and 职务='部长'";
                r_mm = CZMaster.MasterSQL.Get_DataRow(s,strconn);
                fun_load();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        #endregion




        /// <summary>
        /// 读取Excel.xlsx
        /// </summary>
        /// <param name="fd">Excel.xlsx文件所在路径</param>
        public static DataTable ExcelXLSX(System.Windows.Forms.OpenFileDialog fd)
        {

            try
            {
                DataTable dt = new DataTable();
                string strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + fd.FileName + ";Extended Properties='Excel 12.0; HDR=YES; IMEX=1'"; //此連接可以操作.xls與.xlsx文件
                OleDbConnection conn = new OleDbConnection(strConn);
                conn.Open();
                DataSet ds = new DataSet();
                //dt1  为excel中 所有sheet名字集合
                DataTable dt1 = new DataTable();

                dt1 = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                String[] excelSheets = new String[dt1.Rows.Count];
                int i = 0;
                // 添加工作表名称到字符串数组    
                foreach (DataRow row in dt1.Rows)
                {
                    string strSheetTableName = row["TABLE_NAME"].ToString();
                    //过滤无效SheetName   
                    if (strSheetTableName.Contains("$") && strSheetTableName.Replace("'", "").EndsWith("$"))
                    {
                        excelSheets[i] = strSheetTableName.Substring(0, strSheetTableName.Length - 1);
                        OleDbDataAdapter odda = new OleDbDataAdapter(string.Format("SELECT * FROM [{0}]", excelSheets[i] + "$"), conn);//("select * from [Sheet1$]", conn);
                        odda.Fill(ds, excelSheets[i] + "$");
                    }
                    else
                    {
                        string str = excelSheets[i];
                    }


                    i++;
                }

                conn.Close();
                dt = ds.Tables[0];

                foreach (DataTable dt2 in ds.Tables)
                {
                    dt.Merge(dt2);
                }

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fun_计算()
        {
            gridView1.CloseEditor();
            this.BindingContext[dtM].EndCurrentEdit();
            label3.Visible = true;
            DateTime t = CPublic.Var.getDatetime();
            string str_盘点单号 = string.Format("WI{0:D2}{1:D2}{2:D2}", t.Year, t.Month, CPublic.CNo.fun_得到最大流水号
                         ("WI", t.Year, t.Month));
            int POS = 1;
            dt_盘点出入库明细 = new DataTable();
            string sql_盘点出入库 = "select * from 盘点调整库存出入库明细表 where 1<>1";
            dt_盘点出入库明细 = CZMaster.MasterSQL.Get_DataTable(sql_盘点出入库, strconn);
            string sql_库存 = "select * from 仓库物料数量表 where 1<>1";
            dt_库存 = CZMaster.MasterSQL.Get_DataTable(sql_库存, strconn);
            string sql_异常 = "select * from 盘点异常数据表 where 1<>1";
            dt_异常 = CZMaster.MasterSQL.Get_DataTable(sql_异常, strconn);
            int i_记数 = 0;
            int i_count = dtM.Rows.Count;
            //2017-10 因改变盘库模式,所以只需调整选择之后的记录
            DataView dvM = new DataView(dtM);
            dvM.RowFilter = "选择=1";
            dt_ndtomod = dvM.ToTable();
            dt_ndtomod.AcceptChanges();
            foreach (DataRow dr in dt_ndtomod.Rows)
            {
                if (dr["实物数"].ToString() == "")
                {
                    throw new Exception(string.Format("物料{0} 的盘点数量为空,请检查", dr["物料编号"]));
                }
                //  修改库存
                string sql_仓库 = string.Format(@"select kc.*  from 仓库物料数量表 kc  where  kc.物料编码='{0}' and kc.仓库号='{0}' ", dr["物料编号"],dr["仓库号"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql_仓库, strconn))
                {
                    da.Fill(dt_库存);
                    DataRow[] r_库存 = dt_库存.Select(string.Format("物料编码='{0}' and 仓库号='{1}'", dr["物料编号"],dr["仓库号"]));
                    r_库存[0]["库存总数"] = Convert.ToDecimal(r_库存[0]["库存总数"]) + Convert.ToDecimal(dr["偏差值"]);
                    r_库存[0]["有效总数"] = Convert.ToDecimal(r_库存[0]["有效总数"]) + Convert.ToDecimal(dr["偏差值"]);

                    //如果修改后库存为负  记录异常数据
                    //17-12-5 改为 不处理 修改状态 '异常' 
                    dr["财务确认"] = true;
                    if (Convert.ToDecimal(r_库存[0]["库存总数"]) < 0)
                    {
                        #region 一个流水号 不能存在两张表里   异常数据 不要
                        //DataRow r = dt_异常.NewRow();
                        //r["单号"] = str_盘点单号;
                        //r["明细号"] = str_盘点单号 + "-" + POS.ToString();
                        //r["POS"] = POS++;
                        //r["原物料编号"] = dr["物料编号"];
                        //if (Convert.ToDecimal(dr["偏差值"]) > 0)           //偏差值=盘点数-当时库存数
                        //{
                        //    r["出库入库"] = "入库";
                        //}
                        //else
                        //{
                        //    r["出库入库"] = "出库";
                        //}
                        //r["盘前库存"] = dr["库存总数"];
                        //r["盘后库存"] = dr["实物数"];
                        //r["盘亏盘盈数"] = dr["偏差值"];
                        //r["操作时间"] = CPublic.Var.getDatetime();
                        //r["操作人ID"] = CPublic.Var.LocalUserID;
                        //r["操作人"]   =  CPublic.Var.localUserName;
                        //dt_异常.Rows.Add(r);
                        #endregion
                        dr["财务确认"] = false;
                        dr["异常"] = true;
                        dt_库存.Rows.Remove(r_库存[0]);

                    }
                    else if (Convert.ToDecimal(dr["偏差值"]) != 0)     // 1.添加出入库记录     2.修改库存  
                    {
                        DataRow r = dt_盘点出入库明细.NewRow();
                        r["单号"] = str_盘点单号;
                        r["明细号"] = str_盘点单号 + "-" + POS.ToString();
                        r["POS"] = POS++;
                        r["物料编码"] = dr["物料编号"];
                        if (Convert.ToDecimal(dr["偏差值"]) > 0)           //偏差值=盘点数-当时库存数
                        {
                            r["出库入库"] = "入库";
                        }
                        else
                        {
                            r["出库入库"] = "出库";
                        }
                        r["盘前库存"] = dr["库存总数"];
                        r["盘后库存"] = dr["实物数"];
                        r["调整前库存"] = dr["当前库存"];
                        r["调整后库存"] = dr["调整后库存"];
                        r["盘亏盘盈数"] = dr["偏差值"];
                        r["操作时间"] = t;
                        r["操作人ID"] = CPublic.Var.LocalUserID;
                        r["操作人"] = CPublic.Var.localUserName;
                        r["仓库号"] = dr["仓库号"];
                        r["仓库名称"] = dr["仓库名称"];
                        dt_盘点出入库明细.Rows.Add(r);
                    }
                }
                label3.Text = string.Format("计算进度：{0}/{1}", i_记数++, i_count);
                Application.DoEvents();
            }
            string s = "select  * from  其他出入库申请主表 where 1<>1";
            dt_其他出申请主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select  * from  其他出入库申请子表 where 1<>1";
             dt_其他出申请子表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select  * from  其他出库主表 where 1<>1";
            dt_其他出主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select  * from  其他出库子表 where 1<>1";
             dt_其他出子表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select  * from  其他入库主表 where 1<>1";
              dt_其他入主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select  * from  其他入库子表 where 1<>1";
              dt_其他入子表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);

            s = "select * from 仓库出入库明细表 where 1<>1";
              dt_出入库明细 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
              string s申请_no = "";
              DataRow dr_申请主;
            DataView dv_入 = new DataView(dt_ndtomod);
            dv_入.RowFilter = "偏差值>0 and 异常=0 "; //其他入库
            DataTable dt_入 = dv_入.ToTable();
            int pos = 1;
            if (dt_入.Rows.Count > 0)
            {

                #region 其他申请主

                 s申请_no = string.Format("QWSQ{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
             t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QWSQ", t.Year, t.Month).ToString("0000"));
              dr_申请主 = dt_其他出申请主表.NewRow();

                dr_申请主["GUID"] = System.Guid.NewGuid();
                dr_申请主["出入库申请单号"] = s申请_no;
                dr_申请主["申请日期"] = t;
                dr_申请主["操作人员编号"] =r_mm["员工号"];
                dr_申请主["操作人员"] = r_mm["姓名"];
                dr_申请主["生效"] = true;
                dr_申请主["完成"] = true;
                dr_申请主["生效人员编号"] = CPublic.Var.LocalUserID;
                dr_申请主["生效日期"] = t;
                dr_申请主["完成日期"] = t;
                dr_申请主["备注"] = "盘点调整";
                dr_申请主["申请类型"] = "其他入库";
                dr_申请主["原因分类"] = "盘点";
                dt_其他出申请主表.Rows.Add(dr_申请主);

                #endregion
                #region 其他入库主
                string s入库_no = string.Format("QW{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                   t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QW", t.Year, t.Month).ToString("0000"));
                DataRow dr_入库主 = dt_其他入主表.NewRow();
                dr_入库主["GUID"] = System.Guid.NewGuid();
                dr_入库主["其他入库单号"] = s入库_no;
                dr_入库主["创建日期"] = t;
                dr_入库主["操作人员编号"] = CPublic.Var.LocalUserID;
                dr_入库主["操作人员"] = CPublic.Var.localUserName;
                dr_入库主["备注"] = "盘点调整";
                dr_入库主["生效"] = true;
                dr_入库主["生效人员编号"] = CPublic.Var.LocalUserID;
                dr_入库主["生效日期"] = t;
                dr_入库主["入库日期"] = t;

                dr_入库主["出入库申请单号"] = s申请_no;
                dt_其他入主表.Rows.Add(dr_入库主);
                #endregion

                foreach (DataRow r in dt_入.Rows)
                {
                    #region 其他申请子表记录
                    DataRow dr_其他出申请子表 = dt_其他出申请子表.NewRow();
                    dr_其他出申请子表["GUID"] = System.Guid.NewGuid();
                    dr_其他出申请子表["出入库申请单号"] = s申请_no;
                    dr_其他出申请子表["出入库申请明细号"] = s申请_no + "-" + pos.ToString("00");
                    dr_其他出申请子表["POS"] = pos;
                    dr_其他出申请子表["物料编码"] = r["物料编码"].ToString();
                    dr_其他出申请子表["物料名称"] = r["物料名称"].ToString();
                    dr_其他出申请子表["仓库号"] = r["仓库号"].ToString();
                    dr_其他出申请子表["仓库名称"] = r["仓库名称"].ToString();
                  //  dr_其他出申请子表["原ERP物料编号"] = r["物料编号"].ToString();
                    dr_其他出申请子表["数量"] = Convert.ToDecimal(r["偏差值"]); //入库偏差值为正
                    dr_其他出申请子表["规格型号"] = r["规格型号"].ToString();
                    dr_其他出申请子表["完成"] = true;
                    dr_其他出申请子表["完成日期"] = t;
                    dr_其他出申请子表["生效"] = true;
                    dr_其他出申请子表["生效人员编号"] = CPublic.Var.LocalUserID;
                    dr_其他出申请子表["生效日期"] = t;
                    dt_其他出申请子表.Rows.Add(dr_其他出申请子表);
                    #endregion
                    #region 其他入库子表记录
                    DataRow dr_其他入子表 = dt_其他入子表.NewRow();
                    dr_其他入子表["GUID"] = System.Guid.NewGuid();
                    dr_其他入子表["其他入库单号"] = s入库_no;
                    dr_其他入子表["其他入库明细号"] = s入库_no + "-" + pos.ToString("00");
                    dr_其他入子表["POS"] = pos;
                    dr_其他入子表["物料编码"] = r["物料编码"].ToString();
                    dr_其他入子表["物料名称"] = r["物料名称"].ToString();
                
                    dr_其他入子表["数量"] = Convert.ToDecimal(r["偏差值"].ToString());
                    dr_其他入子表["规格型号"] = r["规格型号"].ToString();
                    dr_其他入子表["生效"] = true;
                    dr_其他入子表["生效人员编号"] = CPublic.Var.LocalUserID;
                    dr_其他入子表["生效日期"] = t;
                    dr_其他入子表["出入库申请单号"] = s申请_no;
                    dr_其他入子表["出入库申请明细号"] = s申请_no + "-" + pos.ToString("00");
                    dt_其他入子表.Rows.Add(dr_其他入子表);

                    #endregion
                    #region  仓库出入库明细
                    DataRow drr = dt_出入库明细.NewRow();

                    drr["GUID"] = System.Guid.NewGuid();

                    drr["明细类型"] = "盘点入库";


                    drr["单号"] = s入库_no;


                    drr["相关单号"] = s申请_no;

                    drr["物料编码"] = r["物料编码"].ToString();


                    drr["物料名称"] = r["物料名称"].ToString();

                    drr["明细号"] = s入库_no + "-" + pos.ToString("00");


                    drr["出库入库"] = "入库";

                    drr["数量"] = (Decimal)0;


                    drr["标准数量"] = (Decimal)0;


                    drr["实效数量"] = Convert.ToDecimal(r["偏差值"].ToString());



                    drr["实效时间"] = t;
                    drr["出入库时间"] = t;


                    dt_出入库明细.Rows.Add(drr);


                    #endregion
                    pos++;

                }

            }

       
            #region 其他出库主
            DataView dv_出 = new DataView(dt_ndtomod);
            dv_出.RowFilter = "偏差值<0 and 异常=0 "; //其他出库
            DataTable dt_出 = dv_出.ToTable();
            if (dt_出.Rows.Count > 0)
            {
                #region 其他申请主

                s申请_no = string.Format("QWSQ{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
           t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QWSQ", t.Year, t.Month).ToString("0000"));
                dr_申请主 = dt_其他出申请主表.NewRow();

                dr_申请主["GUID"] = System.Guid.NewGuid();
                dr_申请主["出入库申请单号"] = s申请_no;
                dr_申请主["申请日期"] = t;
                dr_申请主["操作人员编号"] = CPublic.Var.LocalUserID;
                dr_申请主["操作人员"] = CPublic.Var.localUserName;
                dr_申请主["生效"] = true;
                dr_申请主["完成"] = true;
                dr_申请主["生效人员编号"] = CPublic.Var.LocalUserID;
                dr_申请主["生效日期"] = t;
                dr_申请主["完成日期"] = t;
                dr_申请主["备注"] = "盘点调整";
                dr_申请主["申请类型"] = "其他出库";
                dr_申请主["原因分类"] = "盘点";
                dt_其他出申请主表.Rows.Add(dr_申请主);
                string s出库_no = string.Format("QT{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                      t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QT", t.Year, t.Month).ToString("0000"));
                DataRow dr_出库主 = dt_其他出主表.NewRow();
                dr_出库主["GUID"] = System.Guid.NewGuid();
                dr_出库主["其他出库单号"] = s出库_no;
                dr_出库主["创建日期"] = t;
                dr_出库主["操作人员编号"] = CPublic.Var.LocalUserID;
                dr_出库主["操作人员"] = CPublic.Var.localUserName;
                dr_出库主["出库仓库"] = "";

                dr_出库主["生效"] = true;
                dr_出库主["生效人员编号"] = CPublic.Var.LocalUserID;
                dr_出库主["生效日期"] = t;
                dr_出库主["出库日期"] = t;
                dr_出库主["出库类型"] = "盘点出库";
                dr_出库主["出入库申请单号"] = s申请_no;
                dt_其他出主表.Rows.Add(dr_出库主);
            #endregion
            #endregion

                foreach (DataRow r in dt_出.Rows)
                {

                    #region  仓库出入库明细
                    DataRow drr = dt_出入库明细.NewRow();

                    drr["GUID"] = System.Guid.NewGuid();

                    drr["明细类型"] = "盘点出库";


                    drr["单号"] = s出库_no;


                    drr["相关单号"] = s申请_no;

                    drr["物料编码"] = r["物料编码"].ToString();


                    drr["物料名称"] = r["物料名称"].ToString();

                    drr["明细号"] = s出库_no + "-" + pos.ToString("00");


                    drr["出库入库"] = "出库";

                    drr["数量"] = (Decimal)0;


                    drr["标准数量"] = (Decimal)0;


                    drr["实效数量"] = Convert.ToDecimal(r["偏差值"]); // 盘点出库 偏差值为负


                    drr["实效时间"] = t;
                    drr["出入库时间"] = t;


                    dt_出入库明细.Rows.Add(drr);


                    #endregion

                    #region 其他申请子表记录

                    DataRow dr_其他出申请子表 = dt_其他出申请子表.NewRow();

                    dr_其他出申请子表["GUID"] = System.Guid.NewGuid();
                    dr_其他出申请子表["出入库申请单号"] = s申请_no;
                    dr_其他出申请子表["出入库申请明细号"] = s申请_no + "-" + pos.ToString("00");
                    dr_其他出申请子表["POS"] = pos;
                    dr_其他出申请子表["物料编码"] = r["物料编码"].ToString();
                    dr_其他出申请子表["物料名称"] = r["物料名称"].ToString();
               
                    dr_其他出申请子表["数量"] = -Convert.ToDecimal(r["偏差值"]);  // 申请明细 数量存正  
                    dr_其他出申请子表["规格型号"] = r["规格型号"].ToString();
                    dr_其他出申请子表["完成"] = true;
                    dr_其他出申请子表["完成日期"] = t;
                    dr_其他出申请子表["生效"] = true;
                    dr_其他出申请子表["生效人员编号"] = CPublic.Var.LocalUserID;
                    dr_其他出申请子表["生效日期"] = t;
                    dt_其他出申请子表.Rows.Add(dr_其他出申请子表);

                    #endregion

                    #region 其他出库子表记录
                    DataRow dr_其他出子表 = dt_其他出子表.NewRow();
                    dr_其他出子表["GUID"] = System.Guid.NewGuid();
                    dr_其他出子表["其他出库单号"] = s出库_no;
                    dr_其他出子表["其他出库明细号"] = s出库_no + "-" + pos.ToString("00");
                    dr_其他出子表["POS"] = pos;
                    dr_其他出子表["物料编码"] = r["物料编码"].ToString();
                    dr_其他出子表["物料名称"] = r["物料名称"].ToString();
                   // dr_其他出子表["原ERP物料编号"] = r["物料编号"].ToString();
                    dr_其他出子表["数量"] = -Convert.ToDecimal(r["偏差值"]); 
                    dr_其他出子表["规格型号"] = r["规格型号"].ToString();
                    dr_其他出子表["完成"] = true;
                    dr_其他出子表["完成日期"] = t;
                    dr_其他出子表["生效"] = true;
                    dr_其他出子表["生效人员编号"] = CPublic.Var.LocalUserID;
                    dr_其他出子表["生效日期"] = t;
                    dr_其他出子表["出入库申请单号"] = s申请_no;
                    dr_其他出子表["出入库申请明细号"] = s申请_no + "-" + pos.ToString("00");
                    dt_其他出子表.Rows.Add(dr_其他出子表);

                    #endregion
                    pos++;
                }
            }




            label3.Text = string.Format("计算完成");

        }
        //2017-10 做了手持终端盘库的功能 原本导入EXcel 盘库的功能 不需要了
        private void fun_load()
        {
            string s = "";
            if (barEditItem1.EditValue.ToString() == "差异不为0数据")
            {
                s = "and 偏差值<>0";
            }
            //2018-9-3盘点记录表中增加仓库号 仓库名称字段
            string sql = string.Format(@"select  [盘点记录表].*,s.库存总数 as 当前库存,(s.库存总数+偏差值) as 调整后库存,base.规格型号,base.物料编码  from [盘点记录表] 
             left join 基础数据物料信息表 base on base.物料编码=[盘点记录表].物料编号
             left join 仓库物料数量表 s on s.物料编码=base.物料编码 and s.仓库号=盘点记录表.仓库号  where 财务确认=0  and 盘点时间>'2019-5-1' {0} order by 偏差值  ", s);

            //            string sql = string.Format(@"select  [盘点记录表].*,s.库存总数 as 当前库存,(s.库存总数+偏差值) as 调整后库存,base.规格型号  from [盘点记录表] 
            //left join 基础数据物料信息表 base on base.物料编码=[盘点记录表].物料编号
            //left join 仓库物料数量明细表 s on s.ItemId=[盘点记录表].itemid  and s.仓库号=盘点记录表.仓库号  where 财务确认=0   {0} order by 偏差值  ", s);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                dtM = new DataTable();
                da.Fill(dtM);
            }
            dtM.Columns.Add("选择", typeof(bool));
            gridControl1.DataSource = dtM;
        }

        #region 界面操作
        //关闭
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }



        //导入
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {

                dtM = new DataTable();
                var ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    dtM = ExcelXLSX(ofd);
                    dtM.Columns.Add("当前库存", typeof(decimal));
                    dtM.Columns.Add("调整后库存", typeof(decimal));
                    SqlConnection coon = new SqlConnection(strconn);
                    SqlCommand cmd = coon.CreateCommand();
                    coon.Open();
                    SqlDataReader sdr;
                    foreach (DataRow dr in dtM.Rows)
                    {
                        cmd.CommandText = string.Format(@"select kc.* from 仓库物料数量表  where 物料编码='{0}' and  仓库号='{1}' ", dr["物料编码"], dr["仓库号"]);
                        sdr = cmd.ExecuteReader();
                        sdr.Read();
                        dr["当前库存"] = Convert.ToDecimal(sdr["库存总数"]);
                        dr["调整后库存"] = Convert.ToDecimal(sdr["库存总数"]) + Convert.ToDecimal(dr["偏差值"]);
                        sdr.Dispose();

                    }
                    gridControl1.DataSource = dtM;
                    coon.Close();
                }
                else
                {
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



        //刷新
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            frm仓库盘点明细_Load(null, null);



        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {   //
                gridView1.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                if (MessageBox.Show("确定需要调整所选择物料的库存？", "确认!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    fun_计算();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            ////刷新
            //barLargeButtonItem3_ItemClick(null, null);
        }




        //打印
        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {




        }
        #endregion

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
            }
        }
        private void fun_save()
        {
            string sql_盘点出入库 = "select * from 盘点调整库存出入库明细表 where 1<>1";

            string sql_库存 = "select * from 仓库物料数量表 where 1<>1";

            //string sql_异常 = "select * from 盘点异常数据表 where 1<>1";
            string sql_盘库记录 = "select * from 盘点记录表 where 1<>1";

            label3.Text = string.Format("正在保存,请稍候");
            Application.DoEvents();

            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("盘点调整");
            try
            {
                SqlCommand cmm_1 = new SqlCommand(sql_库存, conn, ts);
                SqlCommand cmm_2 = new SqlCommand(sql_盘点出入库, conn, ts);
                //SqlCommand cmm_3 = new SqlCommand(sql_异常, conn, ts);
                SqlCommand cmm_4 = new SqlCommand(sql_盘库记录, conn, ts);
                SqlDataAdapter da_库存 = new SqlDataAdapter(cmm_1);
                SqlDataAdapter da_盘点出入库 = new SqlDataAdapter(cmm_2);
                //SqlDataAdapter da_异常 = new SqlDataAdapter(cmm_3);
                SqlDataAdapter da_4 = new SqlDataAdapter(cmm_4);
 
                new SqlCommandBuilder(da_库存);
                new SqlCommandBuilder(da_盘点出入库);
                // new SqlCommandBuilder(da_异常);
                new SqlCommandBuilder(da_4);

                da_库存.Update(dt_库存);
                da_盘点出入库.Update(dt_盘点出入库明细);
                // da_异常.Update(dt_异常);
                da_4.Update(dt_ndtomod);


                string sql1 = "select * from 仓库出入库明细表 where 1<>1";
                SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
                SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_出入库明细);

                sql1 = "select * from 其他出入库申请主表  where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, ts);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_其他出申请主表);
                sql1 = "select * from 其他出入库申请子表  where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, ts);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_其他出申请子表);
                sql1 = "select * from 其他出库主表  where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, ts);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_其他出主表);
                sql1 = "select * from 其他出库子表  where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, ts);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_其他出子表);
                sql1 = "select * from 其他入库主表  where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, ts);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_其他入主表);
                sql1 = "select * from 其他入库子表  where 1<>1";
                cmd1 = new SqlCommand(sql1, conn, ts);
                da1 = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da1);
                da1.Update(dt_其他入子表);

                ts.Commit();
            }

            catch
            {
                ts.Rollback();
                throw new Exception("保存出错");
            }
            MessageBox.Show("ok");
            label3.Visible = false;
        }
        private void barLargeButtonItem6_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认保存？", "最后核实!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    fun_save();
                }
                frm仓库盘点明细_Load(null, null);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(e.RowHandle);
            decimal dec = Convert.ToDecimal(dr["偏差值"]);
            decimal dec_当前库存 = Convert.ToDecimal(dr["当前库存"]);
            if (dec > 0)
            {

                e.Appearance.BackColor = Color.FromArgb(205, 232, 254);
            }
            else if (dec < 0)
            {

                e.Appearance.BackColor = Color.Pink;
            }
            if (dec > (decimal)0.5 * dec_当前库存)
            {

                e.Appearance.BackColor = Color.Red;

            }
        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DataTable t = dtM.Clone();
                int count = gridView1.DataRowCount;
                for (int i = 0; i < count; i++)
                {
                    t.ImportRow(gridView1.GetDataRow(i));

                }
                t.Columns.Remove("ID");
                t.Columns.Remove("选择");
               
                t.Columns.Remove("itemid");
                t.Columns.Remove("财务确认");
                t.Columns.Remove("异常");
                t.Columns.Remove("机器码");
                t.Columns["备注1"].ColumnName = "颜色";
                t.Columns["备注2"].ColumnName = "月牙膜";
                t.Columns["货架描述"].ColumnName = "货位号";
                t.Columns.Remove("当前库存");
                t.Columns.Remove("调整后库存");
                ItemInspection.print_FMS.npoi_export财务盘点(saveFileDialog.FileName, t);
 
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                gridView1.GetDataRow(i)["选择"] = true;

            }
        }












    }
}
