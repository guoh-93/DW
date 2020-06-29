using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace approval
{
    public partial class ui单据审核界面 : UserControl
    {
        #region 变量
        DataTable dt_ll;
        DataTable dt_r;
        string strcon = CPublic.Var.strConn;
        DataTable dt_权限;
        string strConn_FS = CPublic.Var.geConn("FS");
        string cfgfilepath = "";




        #endregion

        public ui单据审核界面()
        {
            InitializeComponent();
        }

        private DataSet Fun_退料记录(DataRow dr, DataTable dt2)
        {

            DataSet ds = new DataSet();
            //  DataRow dr_left = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            DateTime t = CPublic.Var.getDatetime();
            //return of material RM
            string str_退料单号 = string.Format("RM{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day,
            CPublic.CNo.fun_得到最大流水号("RM", t.Year, t.Month));
            string sql = "select * from 工单返库单主表 where 1<>1";
            DataTable dt_m = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            string s_mx = "select * from  工单返库单明细表 where 1<>1";
            DataTable dt_mx = CZMaster.MasterSQL.Get_DataTable(s_mx, strcon);
            DataRow dr_m = dt_m.NewRow();
            dr_m["退料单号"] = str_退料单号;
            dr_m["待退料号"] = dr["待退料号"];
            dr_m["工单号"] = dr["生产工单号"];
            dr_m["产品编码"] = dr["产品编号"];
            dr_m["产品名称"] = dr["产品名称"];
            dr_m["车间"] = dr["车间"];
            dr_m["日期"] = t;

            dr_m["操作人"] = CPublic.Var.localUserName;
            dt_m.Rows.Add(dr_m);
            dt_m.TableName = "主表";
            ds.Tables.Add(dt_m);
            int i = 1;
            foreach (DataRow r in dt2.Rows)
            {

                DataRow dr_mx = dt_mx.NewRow();
                dr_mx["退料单号"] = str_退料单号;
                dr_mx["退料明细号"] = str_退料单号 + "-" + i.ToString("00");
                dr_mx["待退料号"] = dr["待退料号"];
                dr_mx["工单号"] = dr["生产工单号"];
                dr_mx["物料编码"] = r["物料编码"];
                dr_mx["物料名称"] = r["物料名称"];
                dr_mx["返库数量"] = Convert.ToDecimal(r["已退料数量"]);
                dr_mx["入库人ID"] = CPublic.Var.LocalUserID;
                dr_mx["入库人员"] = CPublic.Var.localUserName;
                dr_mx["日期"] = t;

                dt_mx.Rows.Add(dr_mx);


            }
            dt_mx.TableName = "明细表";
            ds.Tables.Add(dt_mx);
            return ds;
        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
       

        //审核 
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_check();
                DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);

                bool bl_完结;

                 DataSet  ds_zhunb = new DataSet();
                 ds_zhunb = M_审核准备(dr, out bl_完结);

                if (MessageBox.Show(string.Format("确认审核单据{0}", dr["关联单号"]), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    #region  生成审核单并发送邮件 暂时不要
                    /*
                    //审核完成后 自动生成审核单 
                    string root = System.Windows.Forms.Application.StartupPath + "\\ApplyTemp\\";
                    DirectoryInfo rt = new DirectoryInfo(root);
                    if (!rt.Exists) rt.Create();
                    
                    string  path=root + "采购审核.xlsx";
                    //先下载 两个签名水印 放到 System.Windows.Forms.Application.StartupPath + "\\ApplyTemp\\"
                    DataRow dr1 = gv1.GetDataRow(gv1.FocusedRowHandle);
                     
                    //下载申请人 数字签名
                    string s = string.Format("select  文件GUID from  人事基础员工文件表 where 员工号='{0}' and 文件名称='数字签名'", dr1["申请人ID"]);
                    using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
                    {
                        DataTable temp = new DataTable();
                        da.Fill(temp);
                        fun_文件下载(root + dr1["申请人ID"].ToString() + ".jpg", temp.Rows[0]);
                    }
                    //下载审核人 数字签名   
                    string s_审核人ID = CPublic.Var.LocalUserID;
                    s = string.Format("select  文件GUID from  人事基础员工文件表 where 员工号='{0}' and 文件名称='数字签名'", s_审核人ID);
                    using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
                    {
                        DataTable temp = new DataTable();
                        da.Fill(temp);
                        fun_文件下载(root + s_审核人ID + ".jpg", temp.Rows[0]);
                    }
                    //ItemInspection.print_FMS.fun_采购单(dr["关联单号"].ToString(),"",path);
                    ItemInspection.print_FMS.fun_采购审核单(dr["关联单号"].ToString(), path, root + s_审核人ID + ".jpg",root + dr1["申请人ID"].ToString() + ".jpg");
                    //ItemInspection.print_FMS.fun_signPO(root + "采购审核.xlsx", root + dr1["申请人ID"].ToString() + ".jpg", root + s_审核人ID + ".jpg");
                     



                    string  path2=root+dr["关联单号"].ToString()+".pdf";
                   // string path2 = root + "采购审核.pdf";


                    //该PDF还需要 增加两个签名水印
                    if (!ERPorg.Corg.ConverterToPdf(path, path2))
                    {
                        throw new Exception("文件添加数字签名失败,请重试");
                    }
 

                    #region 往pdf 中加水印签名 18-3-22 改为直接在excel种先加签名
                    //PdfReader reader = new PdfReader(path2);
                    //iTextSharp.text.Rectangle psize = reader.GetPageSize(1);      //获取第一页 
                    //PdfStamper pdfStamper = new PdfStamper(reader, new FileStream(root+dr["关联单号"].ToString()+".pdf", FileMode.Create));
                    //Document doc = new Document();
      
                    //doc.Open();
                    //Image gif = Image.GetInstance(root + dr["申请人ID"].ToString() + ".jpg");
                    //Image gif1 = Image.GetInstance(root + s_审核人ID + ".jpg");

                    //int total = reader.NumberOfPages;
                    //for (int i = 1; i <= total; i++)
                    //{
                    //    PdfContentByte waterMarkContent;
                    //    waterMarkContent = pdfStamper.GetOverContent(i);
                     
                    //    gif.ScalePercent(13f);
                    //    gif.SetAbsolutePosition(95, 95);
               

                    //    waterMarkContent.AddImage(gif);

                      
                    //    gif1.ScalePercent(13f);
                    //    gif1.SetAbsolutePosition(315, 93);
                    //    waterMarkContent.AddImage(gif1);
                    //}
                    //doc.Close();
                    //pdfStamper.Close();
                    //reader.Close();

                    # endregion
                    //先上传 然后 
                    string  sql=string.Format(@"select  采购单号,电子邮件 as 电子邮箱,供应商邮箱,操作员 as 采购人员 from 采购记录采购单主表 a
                         left  join  人事基础员工表 b  on a.操作员ID=b.员工号 
                         left join 采购供应商表 c on c.供应商ID=a.供应商ID  where 采购单号='{0}'",dr["关联单号"].ToString());
                     DataRow r=CZMaster.MasterSQL.Get_DataRow(sql,strcon);
                     //2018-3-15 这里需要往表中插一条请求服务器发送邮件的记录 并上传 并把文件地址存于审核表中 
                     
                      string fileadress = fun_文件上传(root + dr["关联单号"].ToString() + ".pdf", r,CPublic.Var.localUserName);
                    */
                    #endregion
                    string fileadress = "";
                    //dt, dt_mx, dt_审核  这个在上面固定顺序 123
                    DataTable t_temp = dr.Table.Clone();
                    t_temp.ImportRow(dr);
                    fun_审核(bl_完结, t_temp, ds_zhunb, fileadress);

                    //2020-6-3 这边刷新四个量的也不要了 直接用存储过程
                    string x = "exec [FourNum]";
                    CZMaster.MasterSQL.ExecuteSQL(x, strcon);
 
                    MessageBox.Show("审核成功");
                    fun_load();
                    // ERPorg.Corg.DeleteFolder(root); //清除本地缓存文件
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 20-6-4 郭恒
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="bl_完结"></param>
        /// <returns></returns>
        private DataSet M_审核准备(DataRow dr, out bool bl_完结)
        {
            DataSet ds = new DataSet();

            string czlx = dr["操作类型"].ToString();
            string doctype = dr["单据类型"].ToString();
            string rec_num = dr["关联单号"].ToString();
            string str_申请单号 = dr["审核申请单号"].ToString();
            DateTime time = CPublic.Var.getDatetime();
            string CurrPerID = CPublic.Var.LocalUserID;
            string CurrPerName = CPublic.Var.localUserName;
            bl_完结 = false;
            //需要判断审批流是否完结
            DataRow r_iscomp = null;
            if (CPublic.Var.LocalUserTeam != "管理员权限")
            {
                r_iscomp = fun_iscomplete(doctype, CurrPerID);
            }
            string s = string.Format("select   主表名称 , 明细表名称,单号字段名,数量字段名, 料号字段名,名称字段名 from  单据审批流配置表 where 单据类型='{0}'  ", doctype);
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (t.Rows.Count == 0) throw new Exception("基础属性表中未维护该单据类型的相关属性值");

            s = string.Format("select  * from  {0}  where  作废=0 and {1}='{2}'", t.Rows[0]["主表名称"].ToString(), t.Rows[0]["单号字段名"].ToString(), rec_num);
           DataTable  dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = string.Format("select  * from  {0}  where  作废=0 and {1}='{2}'", t.Rows[0]["明细表名称"].ToString(), t.Rows[0]["单号字段名"].ToString(), rec_num);
           DataTable  dt_mx = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = string.Format("select  * from  [单据审核申请表] where  作废=0  and 审核申请单号='{0}' and 单据类型 = '{1}'", str_申请单号, doctype);
           DataTable  dt_审核 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (CPublic.Var.LocalUserTeam == "管理员权限" || r_iscomp["上级工号"].ToString() == "") //已完结
            {
                if (!dr["操作类型"].ToString().Contains("弃审"))
                {


                    dt.Rows[0]["审核"] = 1;
                    if (dt.Columns.Contains("审核人员"))
                    {
                        dt.Rows[0]["审核人员"] = CurrPerName;
                    }
                    if (dt.Columns.Contains("审核人员ID"))
                    {
                        dt.Rows[0]["审核人员ID"] = CurrPerID;
                    }
                    if (dt.Columns.Contains("审核人"))
                    {
                        dt.Rows[0]["审核人"] = CurrPerID;
                    }
                    if (dt.Columns.Contains("审核日期"))
                    {
                        dt.Rows[0]["审核日期"] = time;
                    }
                    else
                    {
                        dt.Rows[0]["审核时间"] = time;
                    }
                }

                dt_审核.Rows[0]["审核"] = 1;
                dt_审核.Rows[0]["最终审核人"] = CurrPerName;
                dt_审核.Rows[0]["最终审核人ID"] = CurrPerID;
                dt_审核.Rows[0]["审核时间"] = time;
                bl_完结 = true;
            }
            else
            {
                dt_审核.Rows[0]["待审核人"] = r_iscomp["上级用户ID"];
                dt_审核.Rows[0]["待审核人ID"] = r_iscomp["上级工号"];
                bl_完结 = false;
                if (dr["单据类型"].ToString() == "借用申请单")
                {
                    dt.Rows[0]["部门审核时间"] = time;
                    dt.Rows[0]["部门审核"] = true;
                }
            }
            //这三个顺序不要变 别的地方要用这个顺序传过去
            dt.TableName = t.Rows[0]["主表名称"].ToString();
            ds.Tables.Add(dt);
            dt_mx.TableName = t.Rows[0]["明细表名称"].ToString();
            ds.Tables.Add(dt_mx);
            dt_审核.TableName ="单据审核申请表";
            ds.Tables.Add(dt_审核);
 
            if (dr["单据类型"].ToString().Contains("弃审"))
            {
               DataTable  dt_审核意见 = CZMaster.MasterSQL.Get_DataTable("select * from 单据审核意见记录表 where 1<>1", strcon);

                //19-11-7
                审核意见 frm = new 审核意见(dr["审核申请单号"].ToString());
                frm.ShowDialog();
                if (frm.bl_保存 == true)
                {
                    DataRow dr_审核意见 = dt_审核意见.NewRow();
                    dt_审核意见.Rows.Add(dr_审核意见);
                    dr_审核意见["GUID"] = System.Guid.NewGuid();
                    dr_审核意见["审核申请单号"] = dr["审核申请单号"];
                    dr_审核意见["关联单号"] = dr["关联单号"];
                    dr_审核意见["审核人"] = CPublic.Var.localUserName;
                    dr_审核意见["审核人ID"] = CPublic.Var.LocalUserID;
                    dr_审核意见["弃审审核意见"] = frm.str_意见;
                    dr_审核意见["审核日期"] = CPublic.Var.getDatetime();
                    //-----------------
                    dt_审核意见.TableName = "单据审核意见记录表";
                    ds.Tables.Add(dt_审核意见);
     

                    if (bl_完结)
                    {
                        DataTable  dt_qsmx = qsmx(str_申请单号, dt, dt_mx, doctype);

                        dt_qsmx.TableName = "单据弃审关联单明细";
                        ds.Tables.Add(dt_qsmx);
                    }


                }

            }
      
            
            return ds;
        }

        //2019-9-18增加 申请单号 
        private void fun_save(string doctype, string rec_num, string str_申请单号, string str_文件地址, string number)
        {
            //    dr["单据类型"].ToString(), dr["关联单号"].ToString(), fileadress, dr["生产工单号"].ToString()
            DataRow dr_left = gv1.GetDataRow(gv1.FocusedRowHandle);

            //  string doctype, string rec_num, string str_文件地址,string number
            DateTime time = CPublic.Var.getDatetime();
            string CurrPerID = CPublic.Var.LocalUserID;
            string CurrPerName = CPublic.Var.localUserName;
            string s = string.Format("select   主表名称 , 明细表名称,单号字段名,数量字段名, 料号字段名,名称字段名 from  单据审批流配置表 where 单据类型='{0}'  ", doctype);
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (t.Rows.Count == 0) throw new Exception("基础属性表中未维护该单据类型的相关属性值");
            s = string.Format("select  * from  [单据审核申请表] where  作废=0 and 审核申请单号='{0}'", str_申请单号);
            DataTable dt_审核 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //需要判断审批流是否完结
            DataRow r_iscomp = fun_iscomplete(doctype, CurrPerID);
            if (r_iscomp["上级工号"].ToString() == "") //已完结
            {
                dt_审核.Rows[0]["审核"] = 1;
                dt_审核.Rows[0]["最终审核人"] = CurrPerName;
                dt_审核.Rows[0]["最终审核人ID"] = CurrPerID;
                dt_审核.Rows[0]["审核时间"] = time;
            }
            else
            {
                dt_审核.Rows[0]["待审核人"] = r_iscomp["上级工号"];
                dt_审核.Rows[0]["待审核人ID"] = r_iscomp["上级用户ID"];
            }
            #region  if 工单关闭
            DataSet ds_工单关闭 = new DataSet();
            if (doctype == "工单关闭")
            {
                string sql_检验 = string.Format("select * FROM 生产记录生产工单表 WHERE 生产工单号='{0}'", number);
                DataTable dt_检验 = new DataTable();
                using (SqlDataAdapter da检验 = new SqlDataAdapter(sql_检验, strcon))
                {
                    dt_检验 = new DataTable();
                    da检验.Fill(dt_检验);
                }
                DataRow dr = dt_检验.Rows[0];
                dr["状态"] = true;
                dr["关闭"] = true;
                dr["关闭日期"] = time;
                dr["关闭人员ID"] = CPublic.Var.LocalUserID;
                dr["关闭人员"] = CPublic.Var.localUserName;
                DataTable dt_制令 = new DataTable();
                DataTable dt_领料主 = new DataTable();

                string sql_zl = "";
                string sql_3 = "";
                sql_zl = string.Format("select * from 生产记录生产制令表 where 生产制令单号='{0}'", dr["生产制令单号"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql_zl, strcon))
                {
                    dt_制令 = new DataTable();
                    da.Fill(dt_制令);
                    dt_制令.Rows[0]["已排单数量"] = Convert.ToInt32(dt_制令.Rows[0]["已排单数量"]) - Convert.ToInt32(dr["生产数量"]);
                    dt_制令.Rows[0]["未排单数量"] = Convert.ToInt32(dt_制令.Rows[0]["未排单数量"]) + Convert.ToInt32(dr["生产数量"]);
                }
                // 关闭该条待领料 记录
                sql_3 = string.Format("select * from [生产记录生产工单待领料主表] where 生产工单号='{0}'", dr["生产工单号"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql_3, strcon))
                {
                    dt_领料主 = new DataTable();
                    da.Fill(dt_领料主);
                    if (dt_领料主.Rows.Count > 0)
                    {
                        dt_领料主.Rows[0]["关闭"] = 1;
                        dt_领料主.Rows[0]["关闭时间"] = time;
                    }
                }

                string sql_scrkmx = string.Format("select count(*)条数  from  生产记录成品入库单明细表 where 生产工单号 = '{0}' and 作废 =0", number);
                DataTable dt_scrkmx = CZMaster.MasterSQL.Get_DataTable(sql_scrkmx, strcon);
                if (Convert.ToInt32(dt_scrkmx.Rows[0]["条数"]) == 0)
                {
                    string str_BQ = CPublic.Var.geConn("BQ");
                    string sql_sc = string.Format("delete  [ShareLockInfo] where taskNo='{0}'", number);
                    CZMaster.MasterSQL.ExecuteSQL(sql_sc, str_BQ);
                }
                //  ds_工单关闭 = ERPorg.Corg.ReworkAuditing(dt_检验, dt_制令, dt_领料主);
                dt_检验.TableName = "检验";
                ds_工单关闭.Tables.Add(dt_检验);
                dt_制令.TableName = "制令";
                ds_工单关闭.Tables.Add(dt_制令);
                dt_领料主.TableName = "领料主";
                ds_工单关闭.Tables.Add(dt_领料主);
            }
            #endregion
            DataTable dt_history = fun_approvalhistory(doctype, rec_num);
            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("SH"); //事务的名称
            SqlCommand cmd1 = new SqlCommand(string.Format("select * from {0} where 1<>1", t.Rows[0]["主表名称"].ToString()), conn, ts);
            SqlCommand cmd = new SqlCommand(string.Format("select * from {0} where 1<>1", t.Rows[0]["明细表名称"].ToString()), conn, ts);
            try
            {
                SqlDataAdapter da;
                cmd = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_审核);
                cmd = new SqlCommand("select * from 单据审核日志表 where 1<>1", conn, ts);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_history);

                if (doctype == "工单关闭")
                {
                    cmd = new SqlCommand("select * from 生产记录生产工单表 where 1<>1", conn, ts);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(ds_工单关闭.Tables["检验"]);
                    cmd = new SqlCommand("select * from 生产记录生产制令表 where 1<>1", conn, ts);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(ds_工单关闭.Tables[1]);
                    cmd = new SqlCommand("select * from [生产记录生产工单待领料主表] where 1<>1", conn, ts);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(ds_工单关闭.Tables[2]);

                }
                ts.Commit();
            }
            catch
            {
                ts.Rollback();
                throw new Exception("事务保存失败");
            }
        }
        /// <summary>
        /// 2018-8-22 修改
        /// </summary>
        /// <param name="str_采购单"></param>
        private void fun_刷新四个量(object t)
        {
            DataTable dt_mx = t as DataTable;
            if (!dt_mx.Columns.Contains("物料编码"))
            {
                dt_mx.Columns["产品编码"].ColumnName = "物料编码";
            }
            DataTable dt = StockCore.StockCorer.fun_四个量(dt_mx);
            string ss = "select  * from 仓库物料数量表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(ss, strcon))
            {
                new SqlCommandBuilder(da);
                da.Update(dt);
            }




        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        private void ui单据审核界面_Load(object sender, EventArgs e)
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
        private void fun_文件下载(string pathName, DataRow r)
        {
            CFileTransmission.CFileClient.strCONN = strConn_FS;
            CFileTransmission.CFileClient.Receiver(r["文件GUID"].ToString(), pathName);
        }
        /// <summary>
        /// 文件上传的方法
        /// </summary>
        private string fun_文件上传(string pathName, DataRow r, string str_审核人)
        {
            //判定上传文件的大小

            FileInfo info = new FileInfo(pathName);
            long maxlength = info.Length;

            if (maxlength > 1024 * 1024 * 8)
            {
                throw new Exception("上传的文件不能超过1M，请重新选择后再上传！");
            }

            CFileTransmission.CFileClient.strCONN = strConn_FS;
            string strguid = "";  //记录系统自动返回的GUID

            strguid = CFileTransmission.CFileClient.sendFile(pathName);
            // string type = "";

            int s = pathName.LastIndexOf(".") + 1;
            //type = pathName.Substring(s, pathName.Length - s);
            string sql = "select  * from  [FMSemail请求表] where 1<>1 ";
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            DataRow dr = dt.NewRow();
            dr["单号"] = r["采购单号"];

            dr["审核人员"] = str_审核人;
            //dr["文件名"] = Path.GetFileName(pathName); ;
            dr["请求结果"] = -1; //-1 表示待处理，0 表示发送出错 1 
            dr["请求时间"] = CPublic.Var.getDatetime();
            dr["供应商邮箱"] = r["供应商邮箱"];
            dr["采购人员邮箱"] = r["电子邮箱"];
            dr["文件地址"] = strguid;
            dr["文件名"] = "采购审核单";

            dt.Rows.Add(dr);
            CZMaster.MasterSQL.Save_DataTable(dt, "[FMSemail请求表]", strcon);
            return strguid;
        }
        private void fun_load()
        {
            string s = "";
            if (CPublic.Var.LocalUserID == "admin" || CPublic.Var.LocalUserTeam == "管理员权限")
            {
                s = "select  * from [单据审核申请表]   where     作废=0 and  审核=0 ";
            }
            //   DataTable dt = ERPorg.Corg.fun_hr("采购", CPublic.Var.LocalUserID);
            //string sx = "";
            //if (dt.Rows.Count > 0)
            //{
            //    sx = "and 待审核人ID in (";
            //    foreach (DataRow r in dt.Rows)
            //    {
            //        sx = sx + string.Format("'{0}',", r["工号"]);
            //    }
            //    sx = sx.Substring(0, sx.Length - 1) + ")";
            //} 
            //            string s = string.Format(@" select  a.*,供应商,税率 from [单据审核申请表] a 
            //            left  join 采购记录采购单主表 b    on b.采购单号=a.关联单号 
            //             where    a.作废=0 and a.审核=0 and b.生效=0 and a.单据类型='采购' and 待审核=1 {0} ", sx);
            else
            {
                s = string.Format(@" select  a.*  from [单据审核申请表] a 
              left join 单据审批流表 splb on splb.单据类型=a.单据类型  and (待审核人ID=工号 or 待审核人ID=备用人工号 or 待审核人ID=[备用人工号1])
             where    a.作废=0 and a.审核=0   and  (待审核人ID ='{0}' or [备用人工号1]='{0}' or 备用人工号='{0}') and 角色='审核人' ", CPublic.Var.LocalUserID);

            }
            dt_ll = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = " select  属性值 as 单据类型,属性字段1 as 主表,属性字段2 as 明细表  from 基础数据基础属性表  where 属性类别='审批流单据类型' ";
            DataTable t_c = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //DataView dv = new DataView(dt_ll);
            //dv.RowFilter = string.Format("待审核人ID='{0}'", CPublic.Var.LocalUserID);
            gc1.DataSource = dt_ll;
            s = " select  * from " + t_c.Rows[0]["明细表"].ToString() + "  where 1=2";
            dt_r = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gcP.DataSource = dt_r;
            checkBox1.Checked = false;
        }
        /// <summary>
        /// 2018-8-22
        /// 判断审核是否完成
        /// 返回上级GUID 如果没有上级了 返回值为''
        /// perID 为当前审核人ID
        /// </summary>
        private DataRow fun_iscomplete(string doctype, string perID)
        {
            DataRow dr;
            //if(CPublic.Var.LocalUserTeam=="管理员权限") return dr; 
            string s = string.Format("select  *  from  单据审批流表  where  (工号='{0}' or 备用人工号='{0}' or [备用人工号1]='{0}') and 单据类型='{1}' and 角色='审核人' ", perID, doctype);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (dt.Rows.Count == 0) throw new Exception("单据类型为" + doctype + ",中未找到你的权限");
            dr = dt.Rows[0];
            return dr;
        }
        /// <summary>
        /// 2018-8-22 修改 
        /// doctype 单据类型
        /// ss 为单号
        /// </summary>
        /// <param name="ss"></param>
        private void fun_loadmx(string doctype, string ss)
        {
            string s = string.Format("select     *  from  单据审批流配置表 where 单据类型='{0}'  ", doctype);
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (t.Rows.Count == 0) throw new Exception("单据审批流配置表中未维护该单据类型的相关属性值");
            s = string.Format(@"select  a.*  from  {0} a    where  作废=0 and {1}='{2}'", t.Rows[0]["明细表名称"].ToString(), t.Rows[0]["单号字段名"].ToString(), ss);
            dt_r = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            gridColumn20.FieldName = t.Rows[0]["数量字段名"].ToString();
            gridColumn20.Caption = t.Rows[0]["数量字段名"].ToString();
            gridColumn15.FieldName = t.Rows[0]["名称字段名"].ToString();
            gridColumn15.Caption = t.Rows[0]["名称字段名"].ToString();
            gridColumn24.FieldName = t.Rows[0]["料号字段名"].ToString();
            gridColumn24.Caption = t.Rows[0]["料号字段名"].ToString();
            if (t.Rows[0]["含税单价字段"].ToString().Trim() != "")
            {
                gridColumn7.Visible = true;
                gridColumn7.FieldName = t.Rows[0]["含税单价字段"].ToString();
                gridColumn7.Caption = t.Rows[0]["含税单价字段"].ToString();
                gridColumn8.Visible = true;
                gridColumn8.FieldName = t.Rows[0]["含税金额字段"].ToString();
                gridColumn8.Caption = t.Rows[0]["含税金额字段"].ToString();
            }
            else
            {
                gridColumn7.Visible = false;
                gridColumn8.Visible = false;
            }
            gcP.DataSource = dt_r;
            if (doctype == "销售发货申请")
            {
                DataTable dtss = dt_r.Copy();
                dtss.Columns.Add("显示", typeof(bool));
                foreach (DataRow dr in dtss.Rows)
                {
                    string ass = dr["物料编码"].ToString();
                    ass = ass.Substring(0, 3);
                    if (ass.ToString().Equals("200") || ass.ToString().Equals("110"))
                    {
                        dr["显示"] = true;
                    }
                    else
                    {
                        dr["显示"] = false;
                    }
                }




                DataView dv = new DataView(dtss);
                dv.RowFilter = string.Format("显示=false");
                gcP.DataSource = dv;

            }


        }

        private DataTable fun_approvalhistory(string doctype, string rec_num)
        {
            string s = "select  *  from 单据审核日志表 where 1=2";
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataRow dr = dt.NewRow();
            dr["GUID"] = System.Guid.NewGuid();
            dr["单据类型"] = doctype;
            dr["审核申请单号"] = rec_num;
            dr["审核人员"] = CPublic.Var.localUserName;
            dr["审核人员ID"] = CPublic.Var.LocalUserID;
            dr["审核日期"] = CPublic.Var.getDatetime();
            dt.Rows.Add(dr);
            return dt;
        }
        //返回弃审明细 2019-12-20
        private DataTable qsmx(string str_申请单号, DataTable dt_z, DataTable dt_mx, string doctype)
        {
            string sql = "select * from  单据弃审关联单明细 where 1<>1";
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            DateTime t = CPublic.Var.getDatetime();
            foreach (DataRow dr in dt_mx.Rows)
            {
                DataRow dr_qsmx = dt.NewRow();
                dt.Rows.Add(dr_qsmx);
                dr_qsmx["审核申请单号"] = str_申请单号;
                dr_qsmx["弃审时间"] = t;
                dr_qsmx["物料编码"] = dr["物料编码"];
                dr_qsmx["物料名称"] = dr["物料名称"];
                dr_qsmx["规格型号"] = dr["规格型号"];
                if (doctype == "销售单弃审申请")
                {
                    dr_qsmx["关联单号"] = dr["销售订单号"];
                    dr_qsmx["关联单明细号"] = dr["销售订单明细号"];
                    dr_qsmx["数量"] = Convert.ToDecimal(dr["数量"]);
                    dr_qsmx["已完成数量"] = Convert.ToDecimal(dr["完成数量"]);
                    dr_qsmx["未完成数量"] = Convert.ToDecimal(dr["未完成数量"]);
                    dr_qsmx["关联单表体备注"] = dr["备注"];
                    dr_qsmx["关联单生效时间"] = dt_z.Rows[0]["生效日期"];
                    dr_qsmx["关联单预计发货时间"] = dr["送达日期"];
                    dr_qsmx["关联单表头备注"] = dt_z.Rows[0]["销售备注"];
                }
                if (doctype == "销售预订单弃审申请")
                {
                    dr_qsmx["关联单号"] = dr["销售预订单号"];
                    dr_qsmx["关联单明细号"] = dr["销售预订单明细号"];
                    dr_qsmx["数量"] = Convert.ToDecimal(dr["数量"]);
                    dr_qsmx["已完成数量"] = Convert.ToDecimal(dr["转换订单数量"]);
                    dr_qsmx["未完成数量"] = Convert.ToDecimal(dr["未转数量"]);
                    dr_qsmx["关联单表体备注"] = dr["备注"];
                    dr_qsmx["关联单生效时间"] = dt_z.Rows[0]["审核日期"];
                    dr_qsmx["关联单预计发货时间"] = dr["预计发货日期"];
                    dr_qsmx["关联单表头备注"] = dt_z.Rows[0]["备注"];
                }
                if (doctype == "借用申请单弃审申请")
                {
                    dr_qsmx["关联单号"] = dr["申请批号"];
                    dr_qsmx["关联单明细号"] = dr["申请批号明细"];
                    dr_qsmx["数量"] = Convert.ToDecimal(dr["申请数量"]);
                    dr_qsmx["已完成数量"] = Convert.ToDecimal(dr["已借出数量"]);
                    dr_qsmx["未完成数量"] = Convert.ToDecimal(dr["申请数量"]) - Convert.ToDecimal(dr["已借出数量"]);
                    dr_qsmx["关联单表体备注"] = dr["备注"];
                    dr_qsmx["关联单生效时间"] = dt_z.Rows[0]["部门审核时间"];
                    dr_qsmx["关联单预计发货时间"] = dt_z.Rows[0]["申请日期"];
                    dr_qsmx["关联单表头备注"] = dt_z.Rows[0]["备注"];
                }
            }

            return dt;

        }

        /// <summary>
        /// 2018-8-22 
        /// 20-6-4 修改
        /// <param name="czlx">操作类型</param>
        /// <param name="doctype">单据类型</param>
        /// <param name="rec_num">关联单号</param>
        /// <param name="dt_left">webservers不能序列号datarow 所以需要传 dt,只需要用里面的值 不要修改引用值</param>
        /// <param name="str_文件地址"></param>
        private void fun_审核(bool bl_最上级, DataTable dt_left,DataSet ds_cs, string str_文件地址)
        {
            DataTable dt = ds_cs.Tables[0];
            DataTable dt_mx = ds_cs.Tables[1];
            DataTable dt_审核 = ds_cs.Tables[2];
            DataRow dr_left = dt_left.Rows[0];
            string czlx = dr_left["操作类型"].ToString();
            string doctype = dr_left["单据类型"].ToString();
            string rec_num = dr_left["关联单号"].ToString();
            string str_申请单号 = dr_left["审核申请单号"].ToString();

            DateTime time = CPublic.Var.getDatetime();
            string CurrPerID = CPublic.Var.LocalUserID;
            string CurrPerName = CPublic.Var.localUserName;


            //20-1-17 判断送达日期是否大于等于 生效日期
            //销售订单 和 借用单审核 
            if (doctype == "销售单" && czlx == "生效")
            {
                DateTime t_today = CPublic.Var.getDatetime().Date;
                foreach (DataRow dr in dt_mx.Rows)
                {
                    DateTime t_sd = Convert.ToDateTime(dr["送达日期"]);
                    if (t_sd < t_today)
                    {
                        throw new Exception("要求到货日期不可以小于审核日期,请通知修改要求发货日期");
                    }
                }
            }
            DataTable dt_qsmx = new DataTable();
         
 
            if (bl_最上级)
            {
                if (czlx == "生效")
                {
                    if (dt.Columns.Contains("生效"))
                    {
                        dt.Rows[0]["生效"] = 1;
                        dt.Rows[0]["生效日期"] = time;
                    }
                    if (dt.Columns.Contains("生效人员"))
                    {
                        dt.Rows[0]["生效人员"] = dr_left["申请人"];
                    }
                    if (dt.Columns.Contains("生效人员ID"))
                    {
                        dt.Rows[0]["生效人员ID"] = dr_left["申请人ID"];
                    }
                    if (dt.Columns.Contains("生效人员编号"))
                    {
                        dt.Rows[0]["生效人员编号"] = dr_left["申请人ID"];
                    }
                    foreach (DataRow dr in dt_mx.Rows)
                    {
                        if (dt_mx.Columns.Contains("生效"))
                        {
                            dr["生效"] = 1;
                            dr["生效日期"] = time;
                        }
                        if (dt_mx.Columns.Contains("生效人员"))
                        {
                            dr["生效人员"] = dr_left["申请人"];
                        }
                        if (dt_mx.Columns.Contains("生效人员ID"))
                        {
                            dr["生效人员ID"] = dr_left["申请人ID"];
                        }
                        if (dt_mx.Columns.Contains("生效人员编号"))
                        {
                            dr["生效人员编号"] = dr_left["申请人ID"];
                        }

                    }
                }
                else if (czlx == "关闭")
                {
                    if (dt.Columns.Contains("关闭"))
                    {
                        dt.Rows[0]["关闭"] = 1;
                    }
                    if (dt.Columns.Contains("关闭日期"))
                    {
                        dt.Rows[0]["关闭日期"] = time;
                    }
                    if (dt.Columns.Contains("关闭人员"))
                    {
                        dt.Rows[0]["关闭人员"] = dr_left["申请人"];
                    }
                    if (dt.Columns.Contains("关闭人员ID"))
                    {
                        dt.Rows[0]["关闭人员ID"] = dr_left["申请人ID"];
                    }
                    if (dt.Columns.Contains("关闭人员编号"))
                    {
                        dt.Rows[0]["关闭人员编号"] = dr_left["申请人ID"];
                    }
                    foreach (DataRow dr in dt_mx.Rows)
                    {
                        if (dt_mx.Columns.Contains("关闭"))
                        {
                            dr["关闭"] = 1;

                        }
                        //if (dt_mx.Columns.Contains("作废"))
                        //{
                        //    dr["作废"] = 1;
                        //}
                        if (dt_mx.Columns.Contains("关闭日期"))
                        {

                            dr["关闭日期"] = time;
                        }
                        if (dt_mx.Columns.Contains("关闭人员"))
                        {
                            dr["关闭人员"] = dr_left["申请人"];
                        }
                        if (dt_mx.Columns.Contains("关闭人员ID"))
                        {
                            dr["关闭人员ID"] = dr_left["申请人ID"];
                        }
                        if (dt_mx.Columns.Contains("关闭人员编号"))
                        {
                            dr["关闭人员编号"] = dr_left["申请人ID"];
                        }

                    }

                }
            }
            //2020-6-3 大改--郭恒
            //2020-6-3 key 里填 需要保存的表名称 ,value 里放 需要保存的 dt
            //如果需要继续新增业务类型  按照现有格式将业务逻辑 写成 返回值类型为Dictionary<string, DataTable>的 函数
            Dictionary<string, DataTable> dic_save = new Dictionary<string, DataTable>();
            ERPorg.Corg cg = new ERPorg.Corg();
            //----------------------------------------------
            if (bl_最上级)
            {



                if (doctype == "返修申请")
                {
                    dic_save = save_fxsq(rec_num );
                }
                else if (doctype == "拆单申请")
                {
                    dic_save = save_chaid(rec_num);
                }
                else if (doctype == "借用转客户试用申请单")
                {
                    //string sql_归还申请主表 = string.Format("select * from 归还申请主表 where 归还批号 = '{0}'", rec_num);
                    //dt_归还申请主 = CZMaster.MasterSQL.Get_DataTable(sql_归还申请主表, strcon);
                    //string sql_归还申请子表 = string.Format("select * from 归还申请子表 where 归还批号 = '{0}'", rec_num);
                    //dt_归还申请子 = CZMaster.MasterSQL.Get_DataTable(sql_归还申请子表, strcon);
                    if (dt.Rows.Count > 0)
                    {
                        dic_save = save_jyzxs(dt, dt_mx);
                    }
                }
                else if (doctype == "借用转耗用申请单")
                {
                    if (dt.Rows.Count > 0)
                    {
                        dic_save = save_jyzhy(dt, dt_mx, dt_审核);
                    }
                }
                else if (doctype == "销售单弃审申请")
                {


                    dic_save = save_xsqs(rec_num, str_申请单号);


                }
                else if (doctype == "销售预订单弃审申请")
                {

                    dic_save = save_yddqs(rec_num, str_申请单号);
                }
                else if (doctype == "借用申请单弃审申请")
                {
                    dic_save = save_jydqs(rec_num);
                }
                else if (doctype == "销售发货申请")
                {
                    dic_save = save_fhsq(rec_num);

                }
                else if (doctype == "形态转换申请")
                {
                    dic_save = save_xtzh(rec_num);

                }
                else if (doctype == "ECN变更申请")
                {
                    //string sql_ecn = string.Format("select * from ECR变更申请单明细表 where 申请单号 = '{0}'", rec_num);
                    //DataTable dt_ecn = CZMaster.MasterSQL.Get_DataTable(sql_ecn, strcon);
                    //foreach (DataRow dr_物料 in dt_ecn.Rows)
                    //{
                    //    sql_ecn = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", dr_物料["物料编码"]);
                    //    SqlDataAdapter da = new SqlDataAdapter(sql_ecn, strcon);
                    //    da.Fill(dt_物料_1);
                    //    DataRow[] dr11 = dt_物料_1.Select(string.Format("物料编码 = '{0}'", dr_物料["物料编码"]));
                    //    if (dr11.Length > 0)
                    //    {
                    //        dr11[0]["在研"] = true;
                    //    }
                    //}

                }
                else if (doctype == "销售预订单变更申请")
                {
                    dic_save = yddbg(dt, dt_mx);
                }
                else if (doctype == "采购单" && dt.Rows[0]["采购单类型"].ToString().Trim() != "委外采购")
                {
                    dic_save = cgd(dt_mx);
                }
                else if (doctype == "工单关闭")
                {
                    dic_save = gdgb(dt, dt_mx);

                }
                else if (doctype == "BOM修改申请")
                {
                    dic_save = fun_BOM修改(dr_left, dt);
                }
                else if (doctype == "采购单" && dt.Rows[0]["采购单类型"].ToString().Trim() == "委外采购")
                {
                    //若为 委外采购 则需要生效对应的委外加工的 其他出入库申请  2018-5-22 
                    dic_save = wwcg(rec_num, dt_mx);

                }
                else if (doctype == "销售退货")
                {
                    dic_save = xsth(dt, dt_mx);
                }

                //if (doctype.Contains("弃审"))
                //{
                //    dic_save.Add("单据审核意见记录表", dt_审核意见);
                //    dic_save.Add("单据弃审关联单明细", dt_qsmx);
                //}
            }
            //以上业务逻辑都结束了 下面添加 一些固定要更新的dt  不是顶层 也需要更新下面的 
            DataTable dt_history = fun_approvalhistory(doctype, rec_num);
            dic_save.Add("单据审核日志表", dt_history);
            foreach (DataTable t in ds_cs.Tables)
            {
                if (!dic_save.ContainsKey(t.TableName))
                   dic_save.Add(t.TableName, t);
            }
            //dic_save.Add("单据审核申请表", dt_审核);
            //开始事务更新
            cg.save(dic_save);
        }

        /// <summary>
        /// 销售退货
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dt_mx"></param>
        /// <returns></returns>
        private Dictionary<string, DataTable> xsth(DataTable dt, DataTable dt_mx)
        {
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();
            ////劳务 19-11-21
            dt_mx.Columns.Add("物料名称");
            foreach (DataRow dr in dt_mx.Rows)
            {
                string x = $"select 物料名称 from 基础数据物料信息表 where 物料编码='{dr["物料编码"].ToString()}'";
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(x, strcon);
                dr["物料名称"] = temp.Rows[0]["物料名称"];
            }

            DataTable t_SaleCk = fun_lw(ref dt_mx);
            if (t_SaleCk != null)
                dic.Add("销售记录成品出库单明细表", t_SaleCk);
            DataRow[] rr = dt_mx.Select("完成=0");
            if (rr.Length == 0)
            {
                dt.Rows[0]["完成"] = 1;
                dt.Rows[0]["完成日期"] = CPublic.Var.getDatetime();
                dt.Rows[0]["备注1"] = "劳务系统自动完成";
            }
            //19-10-29  需要回馈给CRM销售订单号
            string Crm_R_No = "";

            if (Crm_R_No.Trim() != "")
            {
                string strcon_aliyun = string.Format("server={0};User Id={1};password={2};Database={3};CharSet=utf8", CPublic.Var.li_CFG["aliyun_server"].ToString(),
                CPublic.Var.li_CFG["aliyun_UID"].ToString(), CPublic.Var.li_CFG["aliyun_PWD"].ToString(), CPublic.Var.li_CFG["aliyun_database"].ToString());
                string s_x = string.Format(" select * from  sellreturn_main  where bl=0 and SRCode='{0}'", Crm_R_No);
                using (MySql.Data.MySqlClient.MySqlDataAdapter da = new MySql.Data.MySqlClient.MySqlDataAdapter(s_x, strcon_aliyun))
                {
                    DataTable dt_somain = new DataTable();
                    da.Fill(dt_somain);
                    dt_somain.Rows[0]["bl"] = true;
                    new MySql.Data.MySqlClient.MySqlCommandBuilder(da);
                    da.Update(dt_somain);
                }
            }

            return dic;


        }

        //销售退货劳务
        private DataTable fun_lw(ref DataTable dt)
        {
            DataTable t_ck = new DataTable();
            DateTime t = CPublic.Var.getDatetime();
            foreach (DataRow r in dt.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                if (r["物料名称"].ToString().Contains("劳务"))
                {
                    r["完成"] = 1;
                    r["完成日期"] = t;
                    string sql = string.Format("select  * from 销售记录成品出库单明细表 where 成品出库单明细号='{0}'", r["出库明细号"]);
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                    {
                        da.Fill(t_ck);//这里需要在退货申请的时候做限制 不可以有重复的物料

                        DataRow[] rrx = t_ck.Select(string.Format("成品出库单明细号='{0}'", r["出库明细号"].ToString()));
                        rrx[0]["累计退货数量"] = Convert.ToDecimal(rrx[0]["累计退货数量"]) + Convert.ToDecimal(r["数量"]);
                    }
                    string s_出库单号 = r["出库明细号"].ToString().Split('-')[0];
                    int p = Convert.ToInt32(r["出库明细号"].ToString().Split('-')[1]);
                    DataRow[] tr = t_ck.Select(string.Format("成品出库单明细号='{0}'  and 退货标识<>'是'", s_出库单号 + "-" + p.ToString("00")));
                    //如果退货数量+累计退货数量>出库数量 -已开票数量
                    //那 退货数量+累计退货数量 -（出库数量 -已开票数量） 部分 需要生成负的 出库记录
                    if (Convert.ToDecimal(tr[0]["累计退货数量"]) > Convert.ToDecimal(tr[0]["出库数量"]) - Convert.ToDecimal(tr[0]["已开票数量"]))
                    {
                        //成品出库明细
                        DataRow rr = t_ck.NewRow();
                        rr["GUID"] = System.Guid.NewGuid();
                        rr["成品出库单号"] = s_出库单号;
                        int pos = 0;

                        DataRow[] rg = t_ck.Select(string.Format("成品出库单号='{0}'  and 退货标识<>'是'", s_出库单号), "POS desc");
                        pos = Convert.ToInt32(rg[0]["POS"]);

                        //if (tr.Length > 0)
                        //    rr["POS"] = Convert.ToInt32(tr[0]["POS"]) + 1;
                        //else
                        //{
                        string s = string.Format("select  max(pos)POS from 销售记录成品出库单明细表 where 成品出库单号='{0}'", s_出库单号);
                        DataTable tt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                        pos = Convert.ToInt32(tt.Rows[0]["POS"]) > pos ? Convert.ToInt32(tt.Rows[0]["POS"]) + 1 : pos + 1;
                        rr["POS"] = pos;
                        //}
                        rr["成品出库单明细号"] = s_出库单号 + "-" + rr["POS"].ToString();
                        rr["备注1"] = "退货";
                        rr["退货标识"] = "是";
                        try
                        {
                            rr["销售订单号"] = tr[0]["销售订单号"];
                            rr["销售订单明细号"] = tr[0]["销售订单明细号"];
                            rr["出库通知单号"] = tr[0]["出库通知单号"];
                            rr["出库通知单明细号"] = tr[0]["出库通知单明细号"];
                        }
                        catch
                        { }
                        rr["物料编码"] = r["物料编码"];
                        rr["物料名称"] = r["物料名称"];
                        //那 退货数量+累计退货数量 -（出库数量 -已开票数量） 部分 需要生成负的 出库记录
                        rr["出库数量"] = -Convert.ToDecimal(tr[0]["累计退货数量"]) + Convert.ToDecimal(tr[0]["出库数量"]) - Convert.ToDecimal(tr[0]["已开票数量"]);
                        rr["已出库数量"] = -Convert.ToDecimal(tr[0]["累计退货数量"]) + Convert.ToDecimal(tr[0]["出库数量"]) - Convert.ToDecimal(tr[0]["已开票数量"]);
                        rr["未开票数量"] = -Convert.ToDecimal(tr[0]["累计退货数量"]) + Convert.ToDecimal(tr[0]["出库数量"]) - Convert.ToDecimal(tr[0]["已开票数量"]);
                        DataTable dt_1 = new DataTable();
                        string sql_1 = string.Format("select  * from 基础数据物料信息表 where 物料编码='{0}'", r["物料编码"]);
                        dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
                        rr["计量单位"] = dt_1.Rows[0]["计量单位"];
                        rr["规格型号"] = dt_1.Rows[0]["规格型号"];
                        rr["客户"] = tr[0]["客户"];
                        rr["客户编号"] = tr[0]["客户编号"];
                        rr["生效"] = true;
                        rr["生效日期"] = t;
                        rr["仓库号"] = tr[0]["仓库号"];
                        rr["仓库名称"] = tr[0]["仓库名称"];
                        t_ck.Rows.Add(rr);
                    }
                }
            }
            return t_ck;
        }
        /// <summary>
        /// 委外采购
        /// </summary>
        /// <param name="rec_num"></param>
        /// <returns></returns>
        private Dictionary<string, DataTable> wwcg(string rec_num, DataTable dt_mx)
        {
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();

            DateTime time = CPublic.Var.getDatetime();
            string s = string.Format("select * from 其他出入库申请主表 where 备注='{0}'", rec_num);
            DataTable dt_qsmain = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = string.Format("select * from 其他出入库申请子表 where 出入库申请单号='{0}'", dt_qsmain.Rows[0]["出入库申请单号"]);
            DataTable dt_qsdetail = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            dt_qsmain.Rows[0]["生效"] = true;
            dt_qsmain.Rows[0]["审核"] = true;
            dt_qsmain.Rows[0]["审核人员ID"] = dt_qsmain.Rows[0]["生效人员编号"] = CPublic.Var.LocalUserID;
            dt_qsmain.Rows[0]["审核人员"] = CPublic.Var.localUserName;
            dt_qsmain.Rows[0]["生效日期"] = time;
            foreach (DataRow dr in dt_qsdetail.Rows)
            {

                dr["生效"] = true;
                dr["生效人员编号"] = CPublic.Var.LocalUserID;
                dr["生效日期"] = time;
            }

            DataTable dt_计划通知明细;
            string sql_jhmx = $"select * from 主计划计划通知单明细 where 生效 = 1 and 关闭 = 0 and 完成 =0";
            dt_计划通知明细 = CZMaster.MasterSQL.Get_DataTable(sql_jhmx, strcon);
            foreach (DataRow dr in dt_mx.Rows)
            {
                if (dr["备注9"].ToString() != "")
                {
                    DataRow[] dr_1 = dt_计划通知明细.Select($"计划通知单明细号 = '{dr["备注9"]}'");
                    if (dr_1.Length > 0)
                    {
                        if ((Convert.ToDecimal(dr["采购数量"]) + Convert.ToDecimal(dr_1[0]["已转采购数量"])) >= Convert.ToDecimal(dr_1[0]["通知采购数量"]))
                        {
                            dr_1[0]["已转采购数量"] = Convert.ToDecimal(dr_1[0]["通知采购数量"]);
                            dr_1[0]["完成"] = true;
                        }
                        else
                        {
                            dr_1[0]["已转采购数量"] = Convert.ToDecimal(dr["采购数量"]) + Convert.ToDecimal(dr_1[0]["已转采购数量"]);
                        }

                    }
                }
            }
            dic.Add("主计划计划通知单明细", dt_计划通知明细);

            dic.Add("其他出入库申请主表", dt_qsmain);

            dic.Add("其他出入库申请子表", dt_qsdetail);

            return dic;

        }

        /// <summary>
        /// 工单关闭
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dt_mx"></param>
        /// <returns></returns>
        private Dictionary<string, DataTable> gdgb(DataTable dt, DataTable dt_mx)
        {
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();

            DataSet ds_工单关闭;
            //bl_是否有退料 = false;
            DataRow dr = dt.Rows[0];
            string sql_zl = "";
            string sql_3 = "";
            sql_zl = string.Format("select * from 生产记录生产制令表 where 生产制令单号='{0}'", dr["生产制令单号"]);
            DataTable dt_制令 = CZMaster.MasterSQL.Get_DataTable(sql_zl, strcon);
            sql_3 = string.Format("select * from [生产记录生产工单待领料主表] where 生产工单号='{0}'", dr["生产工单号"]);
            DataTable dt_领料主 = CZMaster.MasterSQL.Get_DataTable(sql_3, strcon);
            ds_工单关闭 = ds_return(dt, dt_mx, dt_制令, dt_领料主);

            foreach (DataTable t in ds_工单关闭.Tables)
            {
                dic.Add(t.TableName, t);
            }
            return dic;
        }

        /// <summary>
        /// 采购单有一个如果是从计划转的需要更新
        /// </summary>
        /// <param name="dt_mx"></param>
        /// <returns></returns>
        private Dictionary<string, DataTable> cgd(DataTable dt_mx)
        {

            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();

            DataTable dt_计划通知明细;
            string sql_jhmx = $"select * from 主计划计划通知单明细 where 生效 = 1 and 关闭 = 0 and 完成 =0";
            dt_计划通知明细 = CZMaster.MasterSQL.Get_DataTable(sql_jhmx, strcon);
            foreach (DataRow dr in dt_mx.Rows)
            {
                if (dr["备注9"].ToString() != "")
                {
                    DataRow[] dr_1 = dt_计划通知明细.Select($"计划通知单明细号 = '{dr["备注9"]}'");
                    if (dr_1.Length > 0)
                    {
                        if ((Convert.ToDecimal(dr["采购数量"]) + Convert.ToDecimal(dr_1[0]["已转采购数量"])) >= Convert.ToDecimal(dr_1[0]["通知采购数量"]))
                        {
                            dr_1[0]["已转采购数量"] = Convert.ToDecimal(dr_1[0]["通知采购数量"]);
                            dr_1[0]["完成"] = true;
                        }
                        else
                        {
                            dr_1[0]["已转采购数量"] = Convert.ToDecimal(dr["采购数量"]) + Convert.ToDecimal(dr_1[0]["已转采购数量"]);
                        }

                    }
                }
            }
            dic.Add("主计划计划通知单明细", dt_计划通知明细);
            return dic;


        }

        /// <summary>
        /// 预订单变更
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dt_mx"></param>
        /// <returns></returns>
        private Dictionary<string, DataTable> yddbg(DataTable dt, DataTable dt_mx)
        {

            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();
            DateTime time = CPublic.Var.getDatetime();
            string sql_预订主 = $"select * from 销售预订单主表 where 销售预订单号 = '{dt.Rows[0]["销售预订单号"]}'";
            DataTable dt_销售预主 = CZMaster.MasterSQL.Get_DataTable(sql_预订主, strcon);
            string sql_预订明细 = $"select * from 销售预订单明细表 where 销售预订单号 = '{dt.Rows[0]["销售预订单号"]}'";
            DataTable dt_销售预明细 = CZMaster.MasterSQL.Get_DataTable(sql_预订明细, strcon);

            dt_销售预主.Rows[0]["备注"] = dt.Rows[0]["销售备注"].ToString();
            foreach (DataRow dr_销售预明细 in dt_mx.Rows)
            {
                DataRow[] dr_ymx = dt_销售预明细.Select($"销售预订单明细号 = '{dr_销售预明细["销售预订单明细号"]}'");
                if (Convert.ToDecimal(dr_ymx[0]["转换订单数量"]) == Convert.ToDecimal(dr_销售预明细["数量"]))
                {
                    dr_ymx[0]["数量"] = Convert.ToDecimal(dr_销售预明细["数量"]);
                    dr_ymx[0]["未转数量"] = 0;
                    dr_ymx[0]["预计发货日期"] = Convert.ToDateTime(dr_销售预明细["预计发货日期"]);
                    dr_ymx[0]["特殊备注"] = time;//预订单明细表中特殊备注为变更后的审核日期
                    dr_ymx[0]["备注"] = dr_销售预明细["明细备注"];
                    dr_ymx[0]["完成"] = true;
                }
                else
                {
                    dr_ymx[0]["数量"] = Convert.ToDecimal(dr_销售预明细["数量"]);
                    dr_ymx[0]["未转数量"] = Convert.ToDecimal(dr_销售预明细["数量"]) - Convert.ToDecimal(dr_ymx[0]["转换订单数量"]);
                    dr_ymx[0]["预计发货日期"] = Convert.ToDateTime(dr_销售预明细["预计发货日期"]);
                    dr_ymx[0]["特殊备注"] = time;
                    dr_ymx[0]["备注"] = dr_销售预明细["明细备注"];
                }
            }
            DataRow[] dr_1 = dt_销售预明细.Select("完成 = 0");
            if (dr_1.Length == 0)
            {
                dt_销售预主.Rows[0]["完成"] = 1;
            }

            dic.Add("销售预订单主表", dt_销售预主);
            dic.Add("销售预订单明细表", dt_销售预明细);
            return dic;

        }

        /// <summary>
        /// 形态转换
        /// </summary>
        /// <param name="rec_num"></param>
        /// <returns></returns>
        private Dictionary<string, DataTable> save_xtzh(string rec_num)
        {
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();

            DateTime tt = CPublic.Var.getDatetime();
            string xtsql = string.Format(@"select * from 销售形态转换主表 where 形态转换单号 = '{0}'", rec_num);
            DataTable dt_形态转换 = CZMaster.MasterSQL.Get_DataTable(xtsql, strcon);
            string xtmx = string.Format(@"select * from 销售形态转换子表 where 形态转换单号 = '{0}'", rec_num);
            DataTable dt_形态转换子 = CZMaster.MasterSQL.Get_DataTable(xtmx, strcon);
            string sql_仓库出入库明细 = "select * from 仓库出入库明细表 where 1<>1";
            DataTable dt_仓库出入库明细1 = CZMaster.MasterSQL.Get_DataTable(sql_仓库出入库明细, strcon);
            dt_仓库出入库明细1.Columns.Add("规格型号");
            foreach (DataRow dr in dt_形态转换子.Rows)
            {
                if (dr["类型"].ToString() == "转换前")
                {
                    DataRow dr_出库明细 = dt_仓库出入库明细1.NewRow();
                    dr_出库明细["GUID"] = System.Guid.NewGuid();
                    dr_出库明细["明细类型"] = "形态转换出库";
                    dr_出库明细["单号"] = dr["形态转换单号"].ToString();
                    dr_出库明细["物料编码"] = dr["物料编码"].ToString();
                    dr_出库明细["物料名称"] = dr["物料名称"].ToString();
                    dr_出库明细["规格型号"] = dr["规格型号"].ToString();
                    dr_出库明细["相关单位"] = dt_形态转换.Rows[0]["部门名称"].ToString();
                    dr_出库明细["明细号"] = dr["形态转换明细号"].ToString();
                    dr_出库明细["单位"] = dr["计量单位"].ToString();
                    dr_出库明细["数量"] = -Convert.ToDecimal(dr["数量"].ToString());
                    dr_出库明细["实效数量"] = -Convert.ToDecimal(dr["数量"].ToString());
                    dr_出库明细["出库入库"] = "出库";
                    dr_出库明细["实效时间"] = tt;
                    dr_出库明细["出入库时间"] = tt;
                    dr_出库明细["相关单号"] = dr["形态转换单号"].ToString();
                    dr_出库明细["仓库号"] = dr["仓库号"].ToString();
                    dr_出库明细["仓库名称"] = dr["仓库名称"].ToString();
                    dt_仓库出入库明细1.Rows.Add(dr_出库明细);
                }
                else if (dr["类型"].ToString() == "转换后")
                {
                    DataRow dr_出库明细 = dt_仓库出入库明细1.NewRow();
                    dr_出库明细["GUID"] = System.Guid.NewGuid();
                    dr_出库明细["明细类型"] = "形态转换入库";
                    dr_出库明细["单号"] = dr["形态转换单号"].ToString();
                    dr_出库明细["物料编码"] = dr["物料编码"].ToString();
                    dr_出库明细["物料名称"] = dr["物料名称"].ToString();
                    dr_出库明细["规格型号"] = dr["规格型号"].ToString();
                    dr_出库明细["相关单位"] = dt_形态转换.Rows[0]["部门名称"].ToString();
                    dr_出库明细["明细号"] = dr["形态转换明细号"].ToString();
                    dr_出库明细["单位"] = dr["计量单位"].ToString();
                    dr_出库明细["数量"] = Convert.ToDecimal(dr["数量"].ToString());
                    dr_出库明细["实效数量"] = Convert.ToDecimal(dr["数量"].ToString());
                    dr_出库明细["出库入库"] = "入库";
                    dr_出库明细["实效时间"] = tt;
                    dr_出库明细["出入库时间"] = tt;
                    dr_出库明细["相关单号"] = dr["形态转换单号"].ToString();
                    dr_出库明细["仓库号"] = dr["仓库号"].ToString();
                    dr_出库明细["仓库名称"] = dr["仓库名称"].ToString();
                    dt_仓库出入库明细1.Rows.Add(dr_出库明细);
                }
            }
            DataTable t_形态 = ERPorg.Corg.fun_库存(1, dt_仓库出入库明细1);
            dic.Add("仓库出入库明细表", dt_仓库出入库明细1);
            dic.Add("仓库物料数量表", t_形态);

            return dic;

        }

        /// <summary>
        /// 销售发货申请
        /// </summary>
        /// <param name="rec_num"></param>
        /// <param name="dt_销售主"></param>
        /// <param name="dt_已通知数量"></param>
        /// <param name="dt_成品出库单主表"></param>
        /// <param name="dt_成品出库单明细表"></param>
        /// <param name="dt_通知主"></param>
        /// <param name="dt_t通知明细"></param>
        /// <param name="nameStrArray"></param>
        private Dictionary<string, DataTable> save_fhsq(string rec_num)
        {
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();
            //2020-6-3 不知道这个吊名字什么意思 
            bool bl_是否有维护 = false;
            string sql_通知明细 = string.Format("select * from  销售记录销售出库通知单明细表 where 出库通知单号 = '{0}'", rec_num);
            DataTable dt_t通知明细 = CZMaster.MasterSQL.Get_DataTable(sql_通知明细, strcon);
            string sql_通知 = string.Format("select * from  销售记录销售出库通知单主表 where 出库通知单号 = '{0}'", rec_num);
            DataTable dt_通知主 = CZMaster.MasterSQL.Get_DataTable(sql_通知, strcon);
            DataTable dt_成品出库单主表 = new DataTable();
            DataTable dt_成品出库单明细表 = new DataTable();
            DateTime ads = CPublic.Var.getDatetime();

            string s_销售订单 = dt_t通知明细.Rows[0]["销售订单明细号"].ToString();
            string[] str_销售订单 = s_销售订单.Split('-');
            foreach (DataRow r in dt_t通知明细.Rows)
            {
                string ass = r["物料编码"].ToString();
                ass = ass.Substring(0, 3);
                if (ass.ToString().Equals("200") || ass.ToString().Equals("110"))
                {
                    bl_是否有维护 = true;
                }
            }
            string s_成品出库单号 = "";
            int k = 1;

            if (bl_是否有维护)
            {
                s_成品出库单号 = string.Format("SA{0}{1}{2}{3}", ads.Year.ToString(), ads.Month.ToString("00"),
  ads.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SA", ads.Year, ads.Month).ToString("0000"));

                string s_ckz = "select * from 销售记录成品出库单主表 where 1<>1";
                dt_成品出库单主表 = CZMaster.MasterSQL.Get_DataTable(s_ckz, strcon);
                s_ckz = "select * from 销售记录成品出库单明细表 where 1<>1";
                dt_成品出库单明细表 = CZMaster.MasterSQL.Get_DataTable(s_ckz, strcon);
                DataRow dr_成品出库主 = dt_成品出库单主表.NewRow();
                dt_成品出库单主表.Rows.Add(dr_成品出库主);
                dr_成品出库主["GUID"] = System.Guid.NewGuid();
                dr_成品出库主["成品出库单号"] = s_成品出库单号;
                dr_成品出库主["操作员ID"] = CPublic.Var.LocalUserID;
                dr_成品出库主["操作员"] = CPublic.Var.localUserName;
                dr_成品出库主["日期"] = ads;
                dr_成品出库主["创建日期"] = ads;
                dr_成品出库主["修改日期"] = ads;
                dr_成品出库主["生效"] = true;
                dr_成品出库主["生效日期"] = ads;
                foreach (DataRow r in dt_t通知明细.Rows)
                {

                    string ass = r["物料编码"].ToString();
                    ass = ass.Substring(0, 3);
                    if (ass.ToString().Equals("200") || ass.ToString().Equals("110"))
                    {

                        bl_是否有维护 = true;

                        DataRow dr_stockOutDetaail = dt_成品出库单明细表.NewRow();
                        dt_成品出库单明细表.Rows.Add(dr_stockOutDetaail);
                        dr_stockOutDetaail["GUID"] = System.Guid.NewGuid();
                        dr_stockOutDetaail["成品出库单号"] = s_成品出库单号;
                        dr_stockOutDetaail["POS"] = k++;
                        dr_stockOutDetaail["成品出库单明细号"] = s_成品出库单号 + "-" + k.ToString("00");
                        string s_销售订单明细号 = r["销售订单明细号"].ToString();
                        string[] nameStrArray = s_销售订单明细号.Split('-');
                        dr_stockOutDetaail["销售订单号"] = nameStrArray[0].ToString();
                        dr_stockOutDetaail["销售订单明细号"] = r["销售订单明细号"];
                        dr_stockOutDetaail["出库通知单号"] = r["出库通知单号"];
                        dr_stockOutDetaail["出库通知单明细号"] = r["出库通知单明细号"];
                        dr_stockOutDetaail["物料编码"] = r["物料编码"];
                        dr_stockOutDetaail["物料名称"] = r["物料名称"];
                        dr_stockOutDetaail["出库数量"] = r["出库数量"];
                        dr_stockOutDetaail["已出库数量"] = r["出库数量"];
                        dr_stockOutDetaail["未开票数量"] = r["出库数量"];
                        dr_stockOutDetaail["规格型号"] = r["规格型号"];
                        //             dtM

                        dr_stockOutDetaail["客户"] = dt_通知主.Rows[0]["客户名"];
                        dr_stockOutDetaail["客户编号"] = dt_通知主.Rows[0]["客户编号"];
                        string s_xs = string.Format("select * from 销售记录销售订单明细表 where 销售订单明细号='{0}'", r["销售订单明细号"].ToString());
                        DataRow dr_xs = CZMaster.MasterSQL.Get_DataRow(s_xs, strcon);
                        if (dr_xs != null)
                        {
                            dr_stockOutDetaail["仓库号"] = dr_xs["仓库号"];
                            dr_stockOutDetaail["仓库名称"] = dr_xs["仓库名称"];
                        }
                        dr_stockOutDetaail["生效"] = true;
                        dr_stockOutDetaail["生效日期"] = ads;
                        //dr_成品出库明细["n原ERP规格型号"] = dr["n原ERP规格型号"];
                        //k++;
                    }
                }


            }

            //dr_stockOutDetaail["销售订单号"] = s_销售订单[0].ToString();

            DataTable dt_已通知数量 = new DataTable();
            string sql = string.Format("select * from 销售记录销售订单明细表 where 销售订单号 = '{0}'", str_销售订单[0].ToString().Trim());
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            da.Fill(dt_已通知数量);

            string sql_x = string.Format("select * from 销售记录销售订单主表  where  销售订单号='{0}'", str_销售订单[0].ToString().Trim());
            DataTable dt_销售主 = CZMaster.MasterSQL.Get_DataTable(sql_x, strcon);

            foreach (DataRow r in dt_t通知明细.Rows)
            {
                DataRow[] rrr = dt_已通知数量.Select(string.Format("销售订单明细号 = '{0}'", r["销售订单明细号"].ToString().Trim()));
                decimal a_未完成 = Convert.ToDecimal(rrr[0]["未完成数量"]);
                rrr[0]["已通知数量"] = Convert.ToDecimal(rrr[0]["已通知数量"]) + Convert.ToDecimal(r["出库数量"]);
                rrr[0]["未通知数量"] = Convert.ToDecimal(rrr[0]["未通知数量"]) - Convert.ToDecimal(r["出库数量"]);

                string ass = r["物料编码"].ToString();
                ass = ass.Substring(0, 3);
                if (ass.ToString().Equals("200"))
                {

                    bl_是否有维护 = true;
                    decimal xxxx = decimal.Parse(rrr[0]["未完成数量"].ToString()) - Convert.ToDecimal(r["出库数量"]);
                    if (xxxx == 0)
                    {
                        rrr[0]["明细完成"] = true;
                        rrr[0]["明细完成日期"] = ads;
                        rrr[0]["完成数量"] = decimal.Parse(rrr[0]["完成数量"].ToString()) + Convert.ToDecimal(r["出库数量"]);
                        rrr[0]["未完成数量"] = xxxx;
                    }



                    r["已出库数量"] = r["未出库数量"];
                    r["未出库数量"] = 0;
                    r["完成"] = true;
                    r["完成日期"] = ads;
                    r["生效"] = true;
                    r["生效日期"] = ads;

                }
            }

            if (bl_是否有维护)
            {
                if (k == dt_t通知明细.Rows.Count)
                {
                    foreach (DataRow dr in dt_通知主.Rows)
                    {
                        dr["完成"] = true;
                        dr["完成日期"] = ads;
                        dr["生效"] = true;
                        dr["生效日期"] = ads;
                    }
                }
                int a = 0;
                foreach (DataRow dr in dt_已通知数量.Rows)
                {
                    if (bool.Parse(dr["明细完成"].ToString()) == true)
                    {
                        a++;
                    }
                }
                if (a == dt_已通知数量.Rows.Count)
                {
                    dt_销售主.Rows[0]["完成"] = true;
                    dt_销售主.Rows[0]["完成日期"] = ads;

                    foreach (DataRow dr in dt_已通知数量.Rows)
                    {
                        dr["总完成"] = true;

                        dr["总完成日期"] = ads;
                    }

                }
                dt_通知主.AcceptChanges();
                dt_t通知明细.AcceptChanges();

            }

            dic.Add("销售记录销售订单明细表", dt_已通知数量);
            if (bl_是否有维护)
            {
                dic.Add("销售记录成品出库单主表", dt_成品出库单主表);
                dic.Add("销售记录成品出库单明细表", dt_成品出库单明细表);
                dic.Add("销售记录销售出库通知单主表", dt_通知主);
                dic.Add("销售记录销售订单主表", dt_销售主);
                dic.Add("销售记录销售出库通知单明细表", dt_t通知明细);

            }
            return dic;


        }

        /// <summary>
        /// 借用单弃审
        /// </summary>
        /// <param name="rec_num"></param>
        /// <returns></returns>
        private Dictionary<string, DataTable> save_jydqs(string rec_num)
        {
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();

            string sql_借用主 = string.Format("select * from 借还申请表 where 申请批号 = '{0}'", rec_num);
            DataTable dt_借用主 = CZMaster.MasterSQL.Get_DataTable(sql_借用主, strcon);

            //string sql_审 = string.Format("select  * from  [单据审核申请表] where  作废=0 and 关联单号='{0}' and 单据类型 = '借用申请单'", rec_num);
            //DataTable dt_审 = CZMaster.MasterSQL.Get_DataTable(sql_审, strcon);
            dt_借用主.Rows[0]["审核"] = 0;
            dt_借用主.Rows[0]["审核人员"] = "";
            dt_借用主.Rows[0]["审核人员ID"] = "";
            dt_借用主.Rows[0]["审核日期"] = DBNull.Value;
            dt_借用主.Rows[0]["提交审核"] = 0;
            dt_借用主.Rows[0]["部门审核"] = 0;
            dt_借用主.Rows[0]["部门审核时间"] = DBNull.Value;
            dt_借用主.Rows[0]["锁定"] = 0;
            //dt_撤销.Rows[0]["生效日期"] = DBNull.Value;

            //if (dt_审.Rows.Count > 0)
            //{
            //    dt_审.Rows[0].Delete();
            //}
            dic.Add("借还申请表", dt_借用主);
            //dic.Add("单据审核申请表", dt_审);

            return dic;
        }

        /// <summary>
        /// 预订单弃审
        /// </summary>
        /// <param name="rec_num"></param>
        /// <param name="str_申请单号"></param>
        /// <returns></returns>
        private Dictionary<string, DataTable> save_yddqs(string rec_num, string str_申请单号)
        {
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();

            string sql_销售预订主 = string.Format("select * from 销售预订单主表 where 销售预订单号 = '{0}'", rec_num);
            DataTable dt_销售预订主 = CZMaster.MasterSQL.Get_DataTable(sql_销售预订主, strcon);

            //string sql_审 = string.Format("select  * from  [单据审核申请表] where  作废=0 and 审核申请单号='{0}' and 单据类型 = '销售预订单'", str_申请单号);
            //DataTable dt_审 = CZMaster.MasterSQL.Get_DataTable(sql_审, strcon);
            dt_销售预订主.Rows[0]["审核"] = 0;
            dt_销售预订主.Rows[0]["审核人"] = "";
            dt_销售预订主.Rows[0]["提交审核"] = 0;
            dt_销售预订主.Rows[0]["审核日期"] = DBNull.Value;
            dt_销售预订主.Rows[0]["锁定"] = 0;
            //dt_撤销.Rows[0]["生效日期"] = DBNull.Value;

            //if (dt_审.Rows.Count > 0)
            //{
            //    dt_审.Rows[0].Delete();
            //}


            dic.Add("销售预订单主表", dt_销售预订主);
            //dic.Add("单据审核申请表", dt_审);

            return dic;

        }
        /// <summary>
        /// 借用转销售 （借出转客户试用）
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dt_mx"></param>
        /// <returns></returns>
        private Dictionary<string, DataTable> save_jyzxs(DataTable dt, DataTable dt_mx)
        {
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();
            DateTime tt = CPublic.Var.getDatetime();
            string sql_借还申请主表 = string.Format("select * from 借还申请表 where 申请批号 = '{0}'", dt.Rows[0]["申请批号"]);
            DataTable dt_借用主 = CZMaster.MasterSQL.Get_DataTable(sql_借还申请主表, strcon);
            string sql_借还申请子表 = string.Format("select * from 借还申请表附表 where 申请批号 = '{0}'", dt.Rows[0]["申请批号"]);
            DataTable dt_借用子 = CZMaster.MasterSQL.Get_DataTable(sql_借还申请子表, strcon);
            string sql_仓库出入库 = "select * from 仓库出入库明细表 where 1<>1";
            DataTable dt_仓库出入库明细 = CZMaster.MasterSQL.Get_DataTable(sql_仓库出入库, strcon);
            string s_关联单 = "select * from 销售记录销售订单主表 where 1<>1";
            DataTable dt_销售主 = CZMaster.MasterSQL.Get_DataTable(s_关联单, strcon);
            s_关联单 = "select * from 销售记录销售订单明细表 where 1<>1";
            DataTable dt_销售明细 = CZMaster.MasterSQL.Get_DataTable(s_关联单, strcon);
            s_关联单 = "select * from 销售记录销售出库通知单主表 where 1<>1";
            DataTable dt_通知主 = CZMaster.MasterSQL.Get_DataTable(s_关联单, strcon);
            s_关联单 = "select * from 销售记录销售出库通知单明细表 where 1<>1";
            DataTable dt_t通知明细 = CZMaster.MasterSQL.Get_DataTable(s_关联单, strcon);
            s_关联单 = string.Format("select * from 客户基础信息表 where 客户名称 = '{0}'", dt.Rows[0]["客户名称"]);
            DataTable dt_客户 = CZMaster.MasterSQL.Get_DataTable(s_关联单, strcon);
            s_关联单 = "select * from 销售记录成品出库单主表 where 1<>1";
            DataTable dt_成品出库单主表 = CZMaster.MasterSQL.Get_DataTable(s_关联单, strcon);
            s_关联单 = "select * from 销售记录成品出库单明细表 where 1<>1";
            DataTable dt_成品出库单明细表 = CZMaster.MasterSQL.Get_DataTable(s_关联单, strcon);
            string sql_归还 = "select * from 借还申请表归还记录 where 1<>1";
            DataTable dt_归还表 = CZMaster.MasterSQL.Get_DataTable(sql_归还, strcon);
            // dt_仓库出入库明细 = CZMaster.MasterSQL.Get_DataTable(s, strconn);

            // DateTime t = CPublic.Var.getDatetime();
            string s_销售单号 = string.Format("SO{0}{1}{2}{3}", tt.Year.ToString(), tt.Month.ToString("00"),
                tt.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SO", tt.Year, tt.Month, tt.Day).ToString("0000"));
            string s_出库通知单号 = string.Format("SK{0}{1}{2}{3}", tt.Year.ToString(), tt.Month.ToString("00"),
                tt.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SK", tt.Year, tt.Month).ToString("0000"));
            string s_成品出库单号 = string.Format("SA{0}{1}{2}{3}", tt.Year.ToString(), tt.Month.ToString("00"),
                tt.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SA", tt.Year, tt.Month).ToString("0000"));
            string s_归还单号 = string.Format("RA{0}{1}{2}{3}", tt.Year.ToString(), tt.Month.ToString("00"), tt.Day.ToString("00")
                , CPublic.CNo.fun_得到最大流水号("RA", tt.Year, tt.Month).ToString("0000"));
            dt.Rows[0]["归还完成"] = true;
            dt.Rows[0]["归还日期"] = tt;
            dt.Rows[0]["锁定"] = false;



            int iii = 0;
            foreach (DataRow dr_归还申请子 in dt_mx.Rows)
            {
                dr_归还申请子["归还日期"] = tt;
                dr_归还申请子["归还完成"] = true;

                dr_归还申请子["申请已归还数量"] = Convert.ToDecimal(dr_归还申请子["需归还数量"]);
                dr_归还申请子["录入归还数量"] = Convert.ToDecimal(dr_归还申请子["需归还数量"]);
                DataRow[] dr_借还申请子 = dt_借用子.Select(string.Format("申请批号明细 = '{0}'", dr_归还申请子["申请批号明细"]));
                dr_借还申请子[0]["归还数量"] = Convert.ToDecimal(dr_借还申请子[0]["归还数量"].ToString()) + Convert.ToDecimal(dr_归还申请子["需归还数量"].ToString());
                dr_归还申请子["已归还数量"] = Convert.ToDecimal(dr_借还申请子[0]["归还数量"]);
                dr_借还申请子[0]["正在申请数"] = Convert.ToDecimal(dr_借还申请子[0]["正在申请数"].ToString()) - Convert.ToDecimal(dr_归还申请子["需归还数量"].ToString());
                if (decimal.Parse(dr_借还申请子[0]["归还数量"].ToString()) == decimal.Parse(dr_借还申请子[0]["申请数量"].ToString()))
                {
                    dr_借还申请子[0]["归还完成"] = true;
                    dr_借还申请子[0]["归还日期"] = tt;
                    dr_借还申请子[0]["借还状态"] = "已归还";
                }

                DataRow dr_归还 = dt_归还表.NewRow();
                dt_归还表.Rows.Add(dr_归还);
                dr_归还["guid"] = System.Guid.NewGuid();
                dr_归还["申请批号"] = s_归还单号;
                dr_归还["申请批号明细"] = s_归还单号 + "-" + iii.ToString("00");
                dr_归还["借用申请明细号"] = dr_归还申请子["申请批号明细"];
                dr_归还["计量单位"] = dr_归还申请子["计量单位"];
                dr_归还["计量单位编码"] = dr_归还申请子["计量单位编码"];
                dr_归还["物料编码"] = dr_归还申请子["物料编码"];
                dr_归还["物料名称"] = dr_归还申请子["物料名称"];
                dr_归还["规格型号"] = dr_归还申请子["规格型号"];
                dr_归还["仓库号"] = dr_归还申请子["仓库号"];
                dr_归还["仓库名称"] = dr_归还申请子["仓库名称"];
                dr_归还["备注"] = "借用转客户试用" + "自动生成记录" + dr_归还申请子["申请批号"];

                decimal dec = decimal.Parse(dr_归还申请子["需归还数量"].ToString());
                // = Convert.ToDecimal(dr["申请数量"]) - Convert.ToDecimal(dr["归还数量"]);
                dr_归还["归还数量"] = dec;
                dr_归还["归还日期"] = tt;
                dr_归还["货架描述"] = dr_归还申请子["货架描述"];
                dr_归还["归还操作人"] = dt.Rows[0]["归还操作人"];

                DataRow dr_仓库出入库明细 = dt_仓库出入库明细.NewRow();
                dt_仓库出入库明细.Rows.Add(dr_仓库出入库明细);
                dr_仓库出入库明细["GUID"] = System.Guid.NewGuid();
                dr_仓库出入库明细["明细类型"] = "归还入库";
                dr_仓库出入库明细["单号"] = s_归还单号;
                dr_仓库出入库明细["物料编码"] = dr_归还申请子["物料编码"];
                dr_仓库出入库明细["物料名称"] = dr_归还申请子["物料名称"];
                dr_仓库出入库明细["明细号"] = dr_归还["申请批号明细"];
                dr_仓库出入库明细["出库入库"] = "入库";
                dr_仓库出入库明细["实效数量"] = Convert.ToDecimal(dr_归还申请子["需归还数量"]);
                dr_仓库出入库明细["实效时间"] = tt;
                dr_仓库出入库明细["出入库时间"] = tt;
                dr_仓库出入库明细["相关单号"] = dr_归还申请子["申请批号明细"];
                dr_仓库出入库明细["相关单位"] = dt.Rows[0]["客户名称"];
                dr_仓库出入库明细["仓库号"] = dr_归还申请子["仓库号"];
                dr_仓库出入库明细["仓库名称"] = dr_归还申请子["仓库名称"];
                dr_仓库出入库明细["单位"] = dr_归还申请子["计量单位"];

                iii++;

            }
            bool bl_确认 = true;
            foreach (DataRow dr_借还申请子 in dt_借用子.Rows)
            {
                if (Convert.ToBoolean(dr_借还申请子["归还完成"]) == false)
                {
                    bl_确认 = false;
                }
            }
            if (bl_确认)
            {
                dt_借用主.Rows[0]["归还"] = true;
                dt_借用主.Rows[0]["归还日期"] = tt;
                dt_借用主.Rows[0]["手动归还原因"] = "有赠送";
            }

            DataRow dr_销售订单主 = dt_销售主.NewRow();
            dt_销售主.Rows.Add(dr_销售订单主);
            dr_销售订单主["GUID"] = System.Guid.NewGuid();
            dr_销售订单主["销售订单号"] = s_销售单号;
            dr_销售订单主["录入人员"] = "系统自动生成";

            dr_销售订单主["待审核"] = true;
            dr_销售订单主["审核"] = true;
            dr_销售订单主["备注1"] = dt_借用主.Rows[0]["申请批号"]; //记录借用申请单号


            if (dt_客户.Rows.Count > 0)
            {
                dr_销售订单主["客户编号"] = dt_客户.Rows[0]["客户编号"];
                dr_销售订单主["客户名"] = dt.Rows[0]["客户名称"];
                dr_销售订单主["税率"] = dt_客户.Rows[0]["税率"];
                dr_销售订单主["业务员"] = dt_客户.Rows[0]["业务员"];
                //dr_销售订单主["客户名"] = dt_客户.Rows[0]["客户名称"];
            }
            dr_销售订单主["日期"] = tt;
            dr_销售订单主["销售备注"] = "借出转客户试用" + ":" + dt.Rows[0]["归还说明"];
            dr_销售订单主["部门编号"] = CPublic.Var.localUser部门编号;

            dr_销售订单主["税前金额"] = 0;
            dr_销售订单主["税后金额"] = 0;
            dr_销售订单主["生效"] = true;
            dr_销售订单主["生效日期"] = tt;
            dr_销售订单主["生效人员"] = "系统自动生成";
            //   dr_销售订单主["生效人员ID"] = CPublic.Var.LocalUserID;

            dr_销售订单主["创建日期"] = tt;
            dr_销售订单主["修改日期"] = tt;
            dr_销售订单主["完成"] = true;
            dr_销售订单主["完成日期"] = tt;


            DataRow dr_出库通知单主 = dt_通知主.NewRow();
            dt_通知主.Rows.Add(dr_出库通知单主);
            dr_出库通知单主["GUID"] = System.Guid.NewGuid();
            dr_出库通知单主["出库通知单号"] = s_出库通知单号;
            if (dt_客户.Rows.Count > 0)
            {
                dr_出库通知单主["客户编号"] = dt_客户.Rows[0]["客户编号"];
                dr_出库通知单主["客户名"] = dt_客户.Rows[0]["客户名称"];
            }
            dr_出库通知单主["出库日期"] = tt;
            dr_出库通知单主["创建日期"] = tt;
            dr_出库通知单主["修改日期"] = tt;
            //dr_出库通知单主["操作员ID"] = CPublic.Var.LocalUserID;
            dr_出库通知单主["操作员"] = "系统自动生成";
            dr_出库通知单主["生效"] = true;
            dr_出库通知单主["生效日期"] = tt;

            DataRow dr_成品出库主 = dt_成品出库单主表.NewRow();
            dt_成品出库单主表.Rows.Add(dr_成品出库主);
            dr_成品出库主["GUID"] = System.Guid.NewGuid();
            dr_成品出库主["成品出库单号"] = s_成品出库单号;
            // dr_成品出库主["操作员ID"] = CPublic.Var.LocalUserID;
            dr_成品出库主["操作员"] = "系统自动生成";
            if (dt_客户.Rows.Count > 0)
            {
                dr_成品出库主["客户"] = dt_客户.Rows[0]["客户名称"];
            }
            dr_成品出库主["日期"] = tt;
            dr_成品出库主["创建日期"] = tt;
            dr_成品出库主["修改日期"] = tt;
            dr_成品出库主["生效"] = true;
            dr_成品出库主["生效日期"] = tt;
            int jjj = 1;
            foreach (DataRow dr in dt_归还表.Rows)
            {
                DataRow dr_saleDetail = dt_销售明细.NewRow();
                dt_销售明细.Rows.Add(dr_saleDetail);
                dr_saleDetail["GUID"] = System.Guid.NewGuid();
                dr_saleDetail["销售订单号"] = s_销售单号;
                dr_saleDetail["POS"] = jjj;
                dr_saleDetail["销售订单明细号"] = s_销售单号 + "-" + jjj.ToString("00");
                dr_saleDetail["物料编码"] = dr["物料编码"];
                dr_saleDetail["数量"] = dr["归还数量"];
                dr_saleDetail["完成数量"] = dr["归还数量"];
                dr_saleDetail["未完成数量"] = 0;
                dr_saleDetail["已通知数量"] = dr["归还数量"];
                dr_saleDetail["未通知数量"] = 0;
                dr_saleDetail["物料名称"] = dr["物料名称"];
                //dr_销售订单子["n原ERP规格型号"] = dr["n原ERP规格型号"];
                dr_saleDetail["规格型号"] = dr["规格型号"];
                // dr_销售订单子["图纸编号"] = dr["图纸编号"];
                dr_saleDetail["仓库号"] = dr["仓库号"];
                dr_saleDetail["仓库名称"] = dr["仓库名称"];
                dr_saleDetail["计量单位"] = dr["计量单位"];
                // dr_saleDetail["销售备注"] = "借出转赠送";
                dr_saleDetail["税前金额"] = 0;
                dr_saleDetail["税后金额"] = 0;
                dr_saleDetail["税前单价"] = 0;
                dr_saleDetail["税后单价"] = 0;
                dr_saleDetail["送达日期"] = tt;
                if (dt_客户.Rows.Count > 0)
                {
                    dr_saleDetail["客户编号"] = dt_客户.Rows[0]["客户编号"];
                    dr_saleDetail["客户"] = dt_客户.Rows[0]["客户名称"];
                }
                dr_saleDetail["生效"] = true;
                dr_saleDetail["生效日期"] = tt;
                dr_saleDetail["明细完成"] = true;
                dr_saleDetail["明细完成日期"] = tt;
                dr_saleDetail["总完成"] = true;
                dr_saleDetail["总完成日期"] = tt;
                dr_saleDetail["已计算"] = true;
                //dr_saleDetail["录入人员ID"] = CPublic.Var.LocalUserID;
                dr_saleDetail["含税销售价"] = 0;

                DataRow dr_stockOutNotice = dt_t通知明细.NewRow();
                dt_t通知明细.Rows.Add(dr_stockOutNotice);
                dr_stockOutNotice["GUID"] = System.Guid.NewGuid();
                dr_stockOutNotice["出库通知单号"] = s_出库通知单号;
                dr_stockOutNotice["POS"] = jjj;
                dr_stockOutNotice["出库通知单明细号"] = s_出库通知单号 + "-" + jjj.ToString("00");
                dr_stockOutNotice["销售订单明细号"] = dr_saleDetail["销售订单明细号"];
                dr_stockOutNotice["物料编码"] = dr["物料编码"];
                dr_stockOutNotice["物料名称"] = dr["物料名称"];
                dr_stockOutNotice["出库数量"] = dr["归还数量"];
                dr_stockOutNotice["规格型号"] = dr["规格型号"];
                //dr_stockOutNotice["图纸编号"] = dr["图纸编号"];
                dr_stockOutNotice["操作员ID"] = CPublic.Var.LocalUserID;
                dr_stockOutNotice["操作员"] = CPublic.Var.localUserName;
                dr_stockOutNotice["生效"] = true;
                dr_stockOutNotice["生效日期"] = tt;
                dr_stockOutNotice["完成"] = true;
                dr_stockOutNotice["完成日期"] = tt;
                dr_stockOutNotice["计量单位"] = dr["计量单位"];
                dr_stockOutNotice["销售备注"] = "借出转客户试用" + ":" + dt.Rows[0]["归还说明"];


                if (dt_客户.Rows.Count > 0)
                {
                    dr_stockOutNotice["客户"] = dt_客户.Rows[0]["客户名称"];
                    dr_stockOutNotice["客户编号"] = dt_客户.Rows[0]["客户编号"];
                }
                dr_stockOutNotice["已出库数量"] = dr["归还数量"];
                dr_stockOutNotice["未出库数量"] = 0;
                //dr_出库通知单明细["n原ERP规格型号"] = dr["n原ERP规格型号"];

                DataRow dr_stockOutDetaail = dt_成品出库单明细表.NewRow();
                dt_成品出库单明细表.Rows.Add(dr_stockOutDetaail);
                dr_stockOutDetaail["GUID"] = System.Guid.NewGuid();
                dr_stockOutDetaail["成品出库单号"] = s_成品出库单号;
                dr_stockOutDetaail["POS"] = jjj;
                dr_stockOutDetaail["成品出库单明细号"] = s_成品出库单号 + "-" + jjj++.ToString("00");
                dr_stockOutDetaail["销售订单号"] = s_销售单号;
                dr_stockOutDetaail["销售订单明细号"] = dr_saleDetail["销售订单明细号"];
                dr_stockOutDetaail["出库通知单号"] = s_出库通知单号;
                dr_stockOutDetaail["出库通知单明细号"] = dr_stockOutNotice["出库通知单明细号"];
                dr_stockOutDetaail["物料编码"] = dr["物料编码"];
                dr_stockOutDetaail["物料名称"] = dr["物料名称"];
                dr_stockOutDetaail["出库数量"] = dr["归还数量"];
                dr_stockOutDetaail["已出库数量"] = dr["归还数量"];
                dr_stockOutDetaail["未开票数量"] = dr["归还数量"];
                dr_stockOutDetaail["规格型号"] = dr["规格型号"];
                dr_stockOutNotice["计量单位"] = dr["计量单位"];
                dr_stockOutNotice["销售备注"] = "借出转客户试用";
                //dr_stockOutDetaail["图纸编号"] = dr["图纸编号"];
                if (dt_客户.Rows.Count > 0)
                {
                    dr_stockOutDetaail["客户"] = dt_客户.Rows[0]["客户名称"];
                    dr_stockOutDetaail["客户编号"] = dt_客户.Rows[0]["客户编号"];
                }
                dr_stockOutDetaail["仓库号"] = dr["仓库号"];
                dr_stockOutDetaail["仓库名称"] = dr["仓库名称"];
                dr_stockOutDetaail["生效"] = true;
                dr_stockOutDetaail["生效日期"] = tt;
                //dr_成品出库明细["n原ERP规格型号"] = dr["n原ERP规格型号"];

                DataRow dr_stockcrmx = dt_仓库出入库明细.NewRow();
                dt_仓库出入库明细.Rows.Add(dr_stockcrmx);
                dr_stockcrmx["GUID"] = System.Guid.NewGuid();
                dr_stockcrmx["明细类型"] = "销售出库";
                dr_stockcrmx["单号"] = s_成品出库单号;
                dr_stockcrmx["物料编码"] = dr["物料编码"];
                dr_stockcrmx["物料名称"] = dr["物料名称"];
                dr_stockcrmx["明细号"] = dr_stockOutDetaail["成品出库单明细号"];
                dr_stockcrmx["出库入库"] = "出库";
                dr_stockcrmx["实效数量"] = "-" + dr["归还数量"];
                dr_stockcrmx["实效时间"] = tt;
                dr_stockcrmx["出入库时间"] = tt;
                dr_stockcrmx["相关单号"] = dr_saleDetail["销售订单明细号"];
                dr_stockcrmx["仓库号"] = dr["仓库号"];
                dr_stockcrmx["仓库名称"] = dr["仓库名称"];
                dr_stockcrmx["相关单位"] = dt.Rows[0]["客户名称"];
                dr_stockcrmx["单位"] = dr["计量单位"];


            }

            dic.Add("借还申请表", dt_借用主);
            dic.Add("借还申请表附表", dt_借用子);
            dic.Add("借还申请表归还记录", dt_归还表);
            dic.Add("仓库出入库明细表", dt_仓库出入库明细);
            dic.Add("销售记录销售订单主表", dt_销售主);
            dic.Add("销售记录销售出库通知单主表", dt_通知主);
            dic.Add("销售记录成品出库单主表", dt_成品出库单主表);
            dic.Add("销售记录销售订单明细表", dt_销售明细);
            dic.Add("销售记录销售出库通知单明细表", dt_t通知明细);
            dic.Add("销售记录成品出库单明细表", dt_成品出库单明细表);

            return dic;

        }

        /// <summary>
        /// 销售订单弃审 
        /// </summary>
        /// <param name="rec_num"></param>
        /// <param name="str_申请单号"></param>
        /// <returns></returns>
        private Dictionary<string, DataTable> save_xsqs(string rec_num, string str_申请单号)
        {
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();
            string sql_销售主 = string.Format("select * from 销售记录销售订单主表 where 销售订单号 = '{0}'", rec_num);
            DataTable dt_销售主 = CZMaster.MasterSQL.Get_DataTable(sql_销售主, strcon);
            string sql_销售明细 = string.Format("select * from 销售记录销售订单明细表 where 销售订单号 = '{0}'", rec_num);
            DataTable dt_销售明细 = CZMaster.MasterSQL.Get_DataTable(sql_销售明细, strcon);
            //string sql_审 = string.Format("select  * from  [单据审核申请表] where  作废=0 and 审核申请单号='{0}' and 单据类型 = '销售单'", str_申请单号);
            //DataTable dt_审 = CZMaster.MasterSQL.Get_DataTable(sql_审, strcon);
            dt_销售主.Rows[0]["审核"] = 0;
            dt_销售主.Rows[0]["审核人员"] = "";
            dt_销售主.Rows[0]["审核人员ID"] = "";
            dt_销售主.Rows[0]["待审核"] = 0;
            dt_销售主.Rows[0]["生效"] = 0;
            dt_销售主.Rows[0]["生效人员"] = "";
            dt_销售主.Rows[0]["生效人员ID"] = "";
            dt_销售主.Rows[0]["锁定"] = 0;
            //dt_撤销.Rows[0]["生效日期"] = DBNull.Value;
            foreach (DataRow dr_明细 in dt_销售明细.Rows)
            {
                dr_明细["生效"] = 0;
            }
            //if (dt_审.Rows.Count > 0)
            //{
            //    dt_审.Rows[0].Delete();
            //}
            dic.Add("销售记录销售订单主表", dt_销售主);
            dic.Add("销售记录销售订单明细表", dt_销售明细);
            //dic.Add("单据审核申请表", dt_审);

            return dic;

        }
        /// <summary>
        /// 借用转耗用申请单 2020-6-3 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dt_mx"></param>
        /// <param name="dt_审核"></param>
        private Dictionary<string, DataTable> save_jyzhy(DataTable dt, DataTable dt_mx, DataTable dt_审核)
        {
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();
            DateTime tt = CPublic.Var.getDatetime();
            string sql_借还申请主表 = string.Format("select * from 借还申请表 where 申请批号 = '{0}'", dt.Rows[0]["申请批号"]);
            DataTable dt_借用主 = CZMaster.MasterSQL.Get_DataTable(sql_借还申请主表, strcon);
            string sql_借还申请子表 = string.Format("select * from 借还申请表附表 where 申请批号 = '{0}'", dt.Rows[0]["申请批号"]);
            DataTable dt_借用子 = CZMaster.MasterSQL.Get_DataTable(sql_借还申请子表, strcon);
            string sql_仓库出入库 = "select * from 仓库出入库明细表 where 1<>1";
            DataTable dt_仓库出入库明细 = CZMaster.MasterSQL.Get_DataTable(sql_仓库出入库, strcon);
            string s_关联单 = "select * from 其他出入库申请主表 where 1<>1";
            DataTable dt_材料出库申请主 = CZMaster.MasterSQL.Get_DataTable(s_关联单, strcon);
            s_关联单 = "select * from 其他出入库申请子表 where 1<>1";
            DataTable dt_材料出库申请子 = CZMaster.MasterSQL.Get_DataTable(s_关联单, strcon);
            s_关联单 = "select * from 其他出库主表 where 1<>1";
            DataTable dt_材料出库主 = CZMaster.MasterSQL.Get_DataTable(s_关联单, strcon);
            s_关联单 = "select * from 其他出库子表 where 1<>1";
            DataTable dt_材料出库子 = CZMaster.MasterSQL.Get_DataTable(s_关联单, strcon);

            string sql_归还 = "select * from 借还申请表归还记录 where 1<>1";
            DataTable dt_归还表 = CZMaster.MasterSQL.Get_DataTable(sql_归还, strcon);
            // dt_仓库出入库明细 = CZMaster.MasterSQL.Get_DataTable(s, strconn);

            // DateTime t = CPublic.Var.getDatetime();
            string str_材料出库申请单号 = string.Format("DWLS{0}{1}{2}{3}", tt.Year.ToString(), tt.Month.ToString("00"),
                 tt.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("DWLS", tt.Year, tt.Month).ToString("0000"));
            string str_材料出库单号 = string.Format("LS{0}{1}{2}{3}", tt.Year.ToString(), tt.Month.ToString("00"),
               tt.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("LS", tt.Year, tt.Month).ToString("0000"));
            string s_归还单号 = string.Format("RA{0}{1}{2}{3}", tt.Year.ToString(), tt.Month.ToString("00"), tt.Day.ToString("00")
                , CPublic.CNo.fun_得到最大流水号("RA", tt.Year, tt.Month).ToString("0000"));
            dt.Rows[0]["归还完成"] = true;
            dt.Rows[0]["归还日期"] = tt;
            dt.Rows[0]["锁定"] = false;



            int iii = 0;
            foreach (DataRow dr_归还申请子 in dt_mx.Rows)
            {
                dr_归还申请子["归还日期"] = tt;
                dr_归还申请子["归还完成"] = true;

                dr_归还申请子["申请已归还数量"] = Convert.ToDecimal(dr_归还申请子["需归还数量"]);
                dr_归还申请子["录入归还数量"] = Convert.ToDecimal(dr_归还申请子["需归还数量"]);
                DataRow[] dr_借还申请子 = dt_借用子.Select(string.Format("申请批号明细 = '{0}'", dr_归还申请子["申请批号明细"]));
                dr_借还申请子[0]["归还数量"] = Convert.ToDecimal(dr_借还申请子[0]["归还数量"].ToString()) + Convert.ToDecimal(dr_归还申请子["需归还数量"].ToString());
                dr_归还申请子["已归还数量"] = Convert.ToDecimal(dr_借还申请子[0]["归还数量"]);
                dr_借还申请子[0]["正在申请数"] = Convert.ToDecimal(dr_借还申请子[0]["正在申请数"].ToString()) - Convert.ToDecimal(dr_归还申请子["需归还数量"].ToString());
                if (decimal.Parse(dr_借还申请子[0]["归还数量"].ToString()) == decimal.Parse(dr_借还申请子[0]["申请数量"].ToString()))
                {
                    dr_借还申请子[0]["归还完成"] = true;
                    dr_借还申请子[0]["归还日期"] = tt;
                    dr_借还申请子[0]["借还状态"] = "已归还";
                }

                DataRow dr_归还 = dt_归还表.NewRow();
                dt_归还表.Rows.Add(dr_归还);
                dr_归还["guid"] = System.Guid.NewGuid();
                dr_归还["申请批号"] = s_归还单号;
                dr_归还["申请批号明细"] = s_归还单号 + "-" + iii.ToString("00");
                dr_归还["借用申请明细号"] = dr_归还申请子["申请批号明细"];
                dr_归还["计量单位"] = dr_归还申请子["计量单位"];
                dr_归还["计量单位编码"] = dr_归还申请子["计量单位编码"];
                dr_归还["物料编码"] = dr_归还申请子["物料编码"];
                dr_归还["物料名称"] = dr_归还申请子["物料名称"];
                dr_归还["规格型号"] = dr_归还申请子["规格型号"];
                dr_归还["仓库号"] = dr_归还申请子["仓库号"];
                dr_归还["仓库名称"] = dr_归还申请子["仓库名称"];
                dr_归还["备注"] = "借用转耗用" + "自动生成记录" + dr_归还申请子["申请批号"];

                decimal dec = decimal.Parse(dr_归还申请子["需归还数量"].ToString());
                // = Convert.ToDecimal(dr["申请数量"]) - Convert.ToDecimal(dr["归还数量"]);
                dr_归还["归还数量"] = dec;
                dr_归还["归还日期"] = tt;
                dr_归还["货架描述"] = dr_归还申请子["货架描述"];
                dr_归还["归还操作人"] = dt.Rows[0]["归还操作人"];

                DataRow dr_仓库出入库明细 = dt_仓库出入库明细.NewRow();
                dt_仓库出入库明细.Rows.Add(dr_仓库出入库明细);
                dr_仓库出入库明细["GUID"] = System.Guid.NewGuid();
                dr_仓库出入库明细["明细类型"] = "归还入库";
                dr_仓库出入库明细["单号"] = s_归还单号;
                dr_仓库出入库明细["物料编码"] = dr_归还申请子["物料编码"];
                dr_仓库出入库明细["物料名称"] = dr_归还申请子["物料名称"];
                dr_仓库出入库明细["明细号"] = dr_归还["申请批号明细"];
                dr_仓库出入库明细["出库入库"] = "入库";
                dr_仓库出入库明细["实效数量"] = Convert.ToDecimal(dr_归还申请子["需归还数量"]);
                dr_仓库出入库明细["实效时间"] = tt;
                dr_仓库出入库明细["出入库时间"] = tt;
                dr_仓库出入库明细["相关单号"] = dr_归还申请子["申请批号明细"];
                dr_仓库出入库明细["相关单位"] = dt_审核.Rows[0]["相关单位"];
                dr_仓库出入库明细["仓库号"] = dr_归还申请子["仓库号"];
                dr_仓库出入库明细["仓库名称"] = dr_归还申请子["仓库名称"];
                dr_仓库出入库明细["单位"] = dr_归还申请子["计量单位"];

                iii++;

            }
            bool bl_确认 = true;
            foreach (DataRow dr_借还申请子 in dt_借用子.Rows)
            {
                if (Convert.ToBoolean(dr_借还申请子["归还完成"]) == false)
                {
                    bl_确认 = false;
                }
            }
            if (bl_确认)
            {
                dt_借用主.Rows[0]["归还"] = true;
                dt_借用主.Rows[0]["归还日期"] = tt;
                dt_借用主.Rows[0]["手动归还原因"] = "有耗用";
            }
            int i = 1;
            DataRow dr_材料申请主 = dt_材料出库申请主.NewRow();
            dt_材料出库申请主.Rows.Add(dr_材料申请主);
            dr_材料申请主["GUID"] = System.Guid.NewGuid();
            dr_材料申请主["出入库申请单号"] = str_材料出库申请单号;
            dr_材料申请主["申请日期"] = tt;
            dr_材料申请主["申请类型"] = "材料出库";
            dr_材料申请主["备注"] = "借用转耗用：" + dt.Rows[0]["归还说明"];
            // dr_材料申请主["操作人员编号"] = CPublic.Var.LocalUserID;
            dr_材料申请主["操作人员"] = "系统自动生成";
            dr_材料申请主["生效"] = true;
            dr_材料申请主["生效日期"] = tt;
            dr_材料申请主["生效人员编号"] = "系统自动生成";
            dr_材料申请主["完成"] = true;
            dr_材料申请主["完成日期"] = tt;
            dr_材料申请主["原因分类"] = dt.Rows[0]["原因分类"];
            dr_材料申请主["单据类型"] = "材料出库";
            dr_材料申请主["部门名称"] = dt_审核.Rows[0]["相关单位"];

            DataRow dr_材料出库主 = dt_材料出库主.NewRow();
            dt_材料出库主.Rows.Add(dr_材料出库主);
            dr_材料出库主["GUID"] = System.Guid.NewGuid();
            dr_材料出库主["其他出库单号"] = str_材料出库单号;
            dr_材料出库主["出库类型"] = "材料出库";
            //dr_材料出库主["操作人员编号"] = CPublic.Var.LocalUserID;
            dr_材料出库主["操作人员"] = "系统自动生成";
            dr_材料出库主["出库日期"] = tt;
            dr_材料出库主["生效"] = true;
            dr_材料出库主["生效日期"] = tt;
            dr_材料出库主["创建日期"] = tt;
            dr_材料出库主["出入库申请单号"] = str_材料出库申请单号;

            foreach (DataRow dr in dt_mx.Rows)
            {
                DataRow dr_材料申请子 = dt_材料出库申请子.NewRow();
                dt_材料出库申请子.Rows.Add(dr_材料申请子);
                dr_材料申请子["GUID"] = System.Guid.NewGuid();
                dr_材料申请子["出入库申请单号"] = str_材料出库申请单号;
                dr_材料申请子["POS"] = i;
                dr_材料申请子["出入库申请明细号"] = str_材料出库申请单号 + "-" + i.ToString("00");
                dr_材料申请子["物料编码"] = dr["物料编码"];
                dr_材料申请子["规格型号"] = dr["规格型号"];
                dr_材料申请子["物料名称"] = dr["物料名称"];
                dr_材料申请子["数量"] = dr["需归还数量"];//倒冲数量=bom数量*成品入库数量
                dr_材料申请子["已完成数量"] = dr["需归还数量"];
                //  dr_apply_detail["备注"] = dr["物料编码"].ToString();//其他出入库申请主表备注记录成品入库单号 这边子表备注就记录成品编码
                //19-6-23  计算 财务得 成本核算得时候 改为 工单号
                dr_材料申请子["备注"] = dr["申请批号明细"];//其他出入库申请主表备注记录成品入库单号 这边子表备注就记录成品编码

                dr_材料申请子["生效"] = true;
                dr_材料申请子["生效日期"] = tt;
                dr_材料申请子["生效人员编号"] = "系统自动生成";
                dr_材料申请子["完成"] = true;
                dr_材料申请子["完成日期"] = tt;
                dr_材料申请子["仓库号"] = dr["仓库号"];
                dr_材料申请子["仓库名称"] = dr["仓库名称"];
                dr_材料申请子["货架描述"] = dr["货架描述"];


                DataRow dr_材料出库子 = dt_材料出库子.NewRow();
                dt_材料出库子.Rows.Add(dr_材料出库子);
                dr_材料出库子["物料编码"] = dr["物料编码"];
                //dr_其他出库子["原ERP物料编号"] = dr_借用明细["原ERP物料编号"];
                dr_材料出库子["物料名称"] = dr["物料名称"];
                dr_材料出库子["数量"] = Convert.ToDecimal(dr_材料申请子["数量"]);

                dr_材料出库子["规格型号"] = dr["规格型号"];
                // dr_其他出库子["图纸编号"] = rr["图纸编号"];
                dr_材料出库子["其他出库单号"] = str_材料出库单号;
                dr_材料出库子["POS"] = i;
                dr_材料出库子["其他出库明细号"] = str_材料出库单号 + "-" + i.ToString("00");
                dr_材料出库子["GUID"] = System.Guid.NewGuid();
                dr_材料出库子["备注"] = dr["申请批号"];
                dr_材料出库子["生效"] = true;
                dr_材料出库子["生效日期"] = tt;
                dr_材料出库子["生效人员编号"] = "系统自动生成";
                dr_材料出库子["完成"] = true;
                dr_材料出库子["完成日期"] = tt;
                dr_材料出库子["完成人员编号"] = "系统自动生成";
                dr_材料出库子["出入库申请单号"] = str_材料出库申请单号;
                dr_材料出库子["出入库申请明细号"] = dr_材料申请子["出入库申请明细号"];

                DataRow dr_出入库 = dt_仓库出入库明细.NewRow();
                dt_仓库出入库明细.Rows.Add(dr_出入库);
                dr_出入库["GUID"] = System.Guid.NewGuid();
                dr_出入库["明细类型"] = "材料出库";
                dr_出入库["单号"] = str_材料出库单号;
                dr_出入库["出库入库"] = "出库";
                dr_出入库["物料编码"] = dr["物料编码"];
                dr_出入库["物料名称"] = dr["物料名称"];
                dr_出入库["仓库号"] = dr["仓库号"];
                dr_出入库["仓库名称"] = dr["仓库名称"];
                dr_出入库["明细号"] = dr_材料出库子["其他出库明细号"];
                dr_出入库["相关单号"] = str_材料出库申请单号;
                dr_出入库["相关单位"] = dt_审核.Rows[0]["相关单位"];
                //string ss = string.Format("select 车间名称 from 生产记录生产工单表 where 生产工单号='{0}'", dr["生产工单号"].ToString());
                //DataTable t_s = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
                //dr_出入库["相关单位"] = t_s.Rows[0]["车间名称"];
                dr_出入库["实效数量"] = -(Convert.ToDecimal(dr_材料出库子["数量"]));
                dr_出入库["实效时间"] = tt;
                dr_出入库["出入库时间"] = tt;
                i++;
            }
            dic.Add("借还申请表", dt_借用主);
            dic.Add("借还申请表附表", dt_借用子);
            dic.Add("借还申请表归还记录", dt_归还表);
            dic.Add("其他出入库申请主表", dt_材料出库申请主);
            dic.Add("其他出入库申请子表", dt_材料出库申请子);
            dic.Add("其他出库主表", dt_材料出库主);
            dic.Add("其他出库子表", dt_材料出库子);
            dic.Add("仓库出入库明细表", dt_仓库出入库明细);
            return dic;
        }

        /// <summary>
        ///  拆单申请 
        /// </summary>
        /// <param name="rec_num"></param>
        /// <returns></returns>
        private Dictionary<string, DataTable> save_chaid(string rec_num)
        {
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();
            DateTime tt = CPublic.Var.getDatetime();
            DataTable dt_仓库出入库明细1 = new DataTable();
            DataTable t_形态 = new DataTable();
            DataTable dt_仓库出入库明细2 = new DataTable();
            DataTable t_拆单 = new DataTable();
            //if (doctype == "拆单申请")
            //{
            string sql_拆单主 = string.Format(@"select * from 拆单申请主表 where 申请单号 = '{0}'", rec_num);
            DataTable dt_拆单主 = CZMaster.MasterSQL.Get_DataTable(sql_拆单主, strcon);
            string sql_拆单子 = string.Format(@"select * from 拆单申请子表 where 申请单号 = '{0}'", rec_num);
            DataTable dt_拆单子 = CZMaster.MasterSQL.Get_DataTable(sql_拆单子, strcon);
            string sql_仓库出入库明细 = "select * from 仓库出入库明细表 where 1<>1";
            dt_仓库出入库明细2 = CZMaster.MasterSQL.Get_DataTable(sql_仓库出入库明细, strcon);
            dt_仓库出入库明细2.Columns.Add("规格型号");
            if (dt_拆单主.Rows.Count > 0)
            {
                DataRow dr_出入库明细 = dt_仓库出入库明细2.NewRow();
                dr_出入库明细["GUID"] = System.Guid.NewGuid();
                dr_出入库明细["明细类型"] = "拆单申请出库";
                dr_出入库明细["单号"] = dt_拆单主.Rows[0]["申请单号"].ToString();
                dr_出入库明细["物料编码"] = dt_拆单主.Rows[0]["物料编码"].ToString();
                dr_出入库明细["物料名称"] = dt_拆单主.Rows[0]["物料名称"].ToString();
                dr_出入库明细["规格型号"] = dt_拆单主.Rows[0]["规格型号"].ToString();
                dr_出入库明细["相关单位"] = dt_拆单主.Rows[0]["部门名称"].ToString();
                dr_出入库明细["明细号"] = dt_拆单主.Rows[0]["申请单号"].ToString();
                dr_出入库明细["单位"] = dt_拆单主.Rows[0]["计量单位"].ToString();
                dr_出入库明细["数量"] = -Convert.ToDecimal(dt_拆单主.Rows[0]["数量"].ToString());
                dr_出入库明细["实效数量"] = -Convert.ToDecimal(dt_拆单主.Rows[0]["数量"].ToString());
                dr_出入库明细["出库入库"] = "出库";
                dr_出入库明细["实效时间"] = tt;
                dr_出入库明细["出入库时间"] = tt;
                dr_出入库明细["相关单号"] = dt_拆单主.Rows[0]["申请单号"].ToString();
                dr_出入库明细["仓库号"] = dt_拆单主.Rows[0]["仓库号"].ToString();
                dr_出入库明细["仓库名称"] = dt_拆单主.Rows[0]["仓库名称"].ToString();
                dt_仓库出入库明细2.Rows.Add(dr_出入库明细);
            }
            if (dt_拆单子.Rows.Count > 0)
            {
                foreach (DataRow dr in dt_拆单子.Rows)
                {
                    DataRow dr_出入库明细 = dt_仓库出入库明细2.NewRow();
                    dr_出入库明细["GUID"] = System.Guid.NewGuid();
                    dr_出入库明细["明细类型"] = "拆单申请入库";
                    dr_出入库明细["单号"] = dr["申请单号"].ToString();
                    dr_出入库明细["物料编码"] = dr["物料编码"].ToString();
                    dr_出入库明细["物料名称"] = dr["物料名称"].ToString();
                    dr_出入库明细["规格型号"] = dr["规格型号"].ToString();
                    dr_出入库明细["相关单位"] = dt_拆单主.Rows[0]["部门名称"].ToString();
                    dr_出入库明细["明细号"] = dr["申请明细号"].ToString();
                    dr_出入库明细["单位"] = dr["计量单位"].ToString();
                    dr_出入库明细["数量"] = Convert.ToDecimal(dr["数量"].ToString());
                    dr_出入库明细["实效数量"] = Convert.ToDecimal(dr["数量"].ToString());
                    dr_出入库明细["出库入库"] = "入库";
                    dr_出入库明细["实效时间"] = tt;
                    dr_出入库明细["出入库时间"] = tt;
                    dr_出入库明细["相关单号"] = dr["申请单号"].ToString();
                    dr_出入库明细["仓库号"] = dr["仓库号"].ToString();
                    dr_出入库明细["仓库名称"] = dr["仓库名称"].ToString();
                    dt_仓库出入库明细2.Rows.Add(dr_出入库明细);
                }
            }
            t_拆单 = ERPorg.Corg.fun_库存(1, dt_仓库出入库明细2);
            dic.Add("仓库出入库明细表", dt_仓库出入库明细2);
            dic.Add("仓库物料数量表", t_拆单);
            return dic;
        }

        /// <summary>
        /// 返修申请
        /// </summary>
        /// <param name="rec_num"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private Dictionary<string, DataTable> save_fxsq(string rec_num )
        {
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();
            string fxsql = string.Format(@"select 申请单号, 目标产品编码 as 物料编码,物料名称,规格型号,数量,a.车间编号,图纸编号,base.车间
                   ,仓库号,仓库名称,预完工日期,生产备注,工时 as 工时定额,制单人员,制单人员ID,班组编号,班组	  from 新_返修申请主表 a
                   left  join  基础数据物料信息表 base on base.物料编码=a.目标产品编码 where 申请单号='{0}'", rec_num);
            DataRow fxrow = CZMaster.MasterSQL.Get_DataRow(fxsql, strcon);
            fxsql = string.Format("select  * from 新_返修申请子表 where 申请单号='{0}'", rec_num);
            DataTable fxmx = CZMaster.MasterSQL.Get_DataTable(fxsql, strcon);
            fxsql = string.Format(@"select  a.*,物料名称 from 新_返修申请退料子表 a  left join 基础数据物料信息表  base on a.物料编码=base.物料编码 where 申请单号='{0}'", rec_num);
            DataTable tlmx = CZMaster.MasterSQL.Get_DataTable(fxsql, strcon);
            DataSet ds = ERPorg.Corg.ReworkAuditing(fxrow, fxmx, tlmx);
            foreach (DataTable t_save in ds.Tables)
            {
                dic.Add(t_save.TableName, t_save);
            }
            return dic;
        }


        //BOM修改 通过审核修改之前BOM， 生成新的版本
        private Dictionary<string, DataTable> fun_BOM修改(DataRow dr, DataTable dt)
        {
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();
            DataTable dt_BOMM_子;
            DataTable dt_BOMM主;
            DataTable dt_基础BOM;
            DataTable dt_版本;
            DataTable dt_BOMM_copy;
            DateTime time = CPublic.Var.getDatetime();


            //子表审核=1
            dt_BOMM_子 = new DataTable();
            string stre = string.Format("select * from 基础数据BOM修改明细表 where BOM修改单号='{0}' and 审核=0 ", dr["关联单号"].ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(stre, strcon))
            {
                da.Fill(dt_BOMM_子);

            }
            foreach (DataRow r in dt_BOMM_子.Rows)
            {
                r["审核"] = 1;
            }
            //主表审核=1
            string str1 = string.Format("select * from 基础数据BOM修改主表 where BOM修改单号='{0}' and 审核=0 ", dr["关联单号"].ToString());
            using (SqlDataAdapter da1 = new SqlDataAdapter(str1, strcon))
            {
                dt_BOMM主 = new DataTable();
                da1.Fill(dt_BOMM主);
                dt_BOMM主.Rows[0]["审核人员"] = CPublic.Var.localUserName;
                dt_BOMM主.Rows[0]["审核人员ID"] = CPublic.Var.LocalUserID;
                dt_BOMM主.Rows[0]["审核日期"] = time;
                dt_BOMM主.Rows[0]["审核"] = true;
            }
            //20-1-17  判断这个东西有没有工时 没有工时不允许审核
            //20-1-19暂不上线 等节后他们沟通好了再上传
            //string s = string.Format("select  count(*)x from 基础数据物料信息表 where 工时<>0 and 物料编码='{0}'", dt_BOMM主.Rows[0]["产品编码"].ToString());
            //DataTable dt = CZMaster.MasterSQL.Get_DataTable(s,strcon);
            //if (Convert.ToInt32(dt.Rows[0]["x"]) == 0) throw new Exception("工时尚未维护,暂不可审核");

            //加载当前BOM所对应的版本，并修改成最新版本
            string str2 = string.Format("select * from 基础数据物料BOM表 where 产品编码='{0}'", dt_BOMM主.Rows[0]["产品编码"].ToString());

            string sq = string.Format("select * from 生产记录生产工单表 where  物料编码='{0}' and 完成=0 and 关闭=0 ", dt_BOMM主.Rows[0]["产品编码"].ToString());
            DataTable dt_ck = CZMaster.MasterSQL.Get_DataTable(sq, strcon);
            if (dt_ck.Rows.Count > 0)
            {
                if (MessageBox.Show("当前物料有正在制作的生产工单，请确认继续？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                { }
                else
                {
                    throw new Exception("已取消");
                }
            }
            using (SqlDataAdapter da3 = new SqlDataAdapter(str2, strcon))
            {
                dt_基础BOM = new DataTable();
                da3.Fill(dt_基础BOM);
                dt_BOMM_copy = dt_基础BOM.Copy();
            }
            //更新新BOM
            int i = dt_基础BOM.Rows.Count - 1;
            for (; i >= 0; i--)
            {
                dt_基础BOM.Rows[i].Delete();
            }

            foreach (DataRow r in dt_BOMM_子.Rows)
            {
                DataRow rr = dt_基础BOM.NewRow();
                if (dr.RowState == DataRowState.Deleted) continue;
                //rr.ItemArray = r.ItemArray;
                for (int j = 0; j < dt_基础BOM.Columns.Count; j++)
                {
                    string s = r[j].ToString();
                    string ss = dt_基础BOM.Columns[j].ColumnName;
                    for (int t = 0; t < dt_BOMM_子.Columns.Count; t++)
                    {
                        if (ss == dt_BOMM_子.Columns[t].ColumnName)
                        {
                            rr[j] = r[t];
                            continue;
                        }

                    }
                }

                //基础物料表中BOM的更改
                if (r["BOM版本号"].ToString() == "" || r["BOM版本号"] == null)
                {
                    rr["BOM版本号"] = "1";
                }
                else
                {
                    rr["BOM版本号"] = Convert.ToInt32(r["BOM版本号"]) + 1;
                }

                dt_基础BOM.Rows.Add(rr);


            }

            //  在BOM版本中生成新的BOM版本
            dt_版本 = new DataTable();

            using (SqlDataAdapter da4 = new SqlDataAdapter("select * from [基础物料BOM版本表] where 1<>1", CPublic.Var.strConn))
            {
                da4.Fill(dt_版本);
            }
            if (dt_BOMM_copy.Rows.Count > 0)
            {
                foreach (DataRow dr_copy in dt_BOMM_copy.Rows)
                {
                    DataRow rr = dt_版本.NewRow();

                    if (dr_copy["BOM版本号"].ToString() == "" || dr_copy["BOM版本号"] == null)
                    {
                        rr["BOM版本号"] = "0";
                    }
                    else
                    {
                        rr["BOM版本号"] = Convert.ToInt32(dr_copy["BOM版本号"]);
                    }


                    rr["物料编码"] = dr_copy["产品编码"].ToString();
                    rr["子项编码"] = dr_copy["子项编码"].ToString();
                    rr["子项名称"] = dr_copy["子项名称"].ToString();
                    // rr["图纸编号"] = dr_copy["图纸编号"].ToString();
                    rr["总数量"] = Convert.ToDecimal(dr_copy["总装数量"]);
                    rr["总装数量"] = Convert.ToDecimal(dr_copy["总装数量"]);
                    rr["A面位号"] = dr_copy["A面位号"].ToString();
                    rr["B面位号"] = dr_copy["B面位号"].ToString();
                    rr["主辅料"] = dr_copy["主辅料"].ToString();
                    rr["子项类型"] = dr_copy["子项类型"].ToString();
                    rr["BOM类型"] = dr_copy["BOM类型"].ToString();
                    rr["计量单位"] = dr_copy["计量单位"].ToString();
                    rr["用途"] = dr_copy["用途"].ToString();
                    rr["组"] = dr_copy["组"].ToString();
                    //rr["货架"] = dr_copy["货架描述"].ToString();
                    rr["优先级"] = dr_copy["优先级"].ToString();
                    // rr["关于子项"] = dr[""].ToString();
                    rr["修改人员"] = CPublic.Var.localUserName;
                    rr["修改人员ID"] = CPublic.Var.LocalUserID;
                    rr["修改日期"] = time;
                    rr["仓库号"] = dr_copy["仓库号"].ToString();
                    rr["仓库名称"] = dr_copy["仓库名称"].ToString();
                    dt_版本.Rows.Add(rr);



                }

            }

            string cpbh = "";
            decimal dec_pb = 0;
            string dh = "";
            //20-5-12    配置的主表
            cpbh = dt.Rows[0]["产品编码"].ToString();
            dh = dt.Rows[0]["BOM修改单号"].ToString();
            if (cpbh.Substring(0, 2) == "05")
            {
                string xx = $@"select 产品编码,SUM(拼板数量)pb from 基础数据BOM修改明细表 a
                        left join 基础数据物料信息表 b on b.物料编码 = a.子项编码
                        where 产品编码 = '{cpbh}'and BOM修改单号='{dh}' group by 产品编码";
                DataTable ty = CZMaster.MasterSQL.Get_DataTable(xx, strcon);
                if (ty.Rows.Count > 0)
                {
                    dec_pb = Convert.ToDecimal(ty.Rows[0]["pb"]);
                }
                string x = string.Format("select  * from 基础数据物料信息表 where 物料编码='{0}'", cpbh);
                DataTable t_base = CZMaster.MasterSQL.Get_DataTable(x, strcon);
                t_base.Rows[0]["拼板数量"] = dec_pb;
                dic.Add("基础数据物料信息表", t_base);

                //SqlCommand cmd_base = new SqlCommand(x, conn, ts);
                //using (SqlDataAdapter da1 = new SqlDataAdapter(cmd_base))
                //{
                //    new SqlCommandBuilder(da1);
                //    da1.Update(t_base);
                //}

            }

            dic.Add("基础数据BOM修改明细表", dt_BOMM_子);
            dic.Add("基础数据物料BOM表", dt_基础BOM);
            dic.Add("基础物料BOM版本表", dt_版本);


            return dic;

        }



        //private void fun_审核( string str_采购单,string str_文件地址)
        //{
        //    DateTime time = CPublic.Var.getDatetime();
        //    string s = string.Format("select  * from  {} where  作废=0 and 采购单号='{0}'", str_采购单);
        //    DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
        //    s = string.Format(@"select  * from  {}  where  作废=0 and 采购单号='{0}'", str_采购单);
        //    DataTable dt_mx = CZMaster.MasterSQL.Get_DataTable(s, strcon);
        //    dt.Rows[0]["生效"]=1;
        //    //主表 生效人员 生效人员ID 操作员 操作员ID
        //    dt.Rows[0]["生效人员"] = dt.Rows[0]["操作员"];
        //    dt.Rows[0]["生效人员ID"] = dt.Rows[0]["操作员ID"];
        //    dt.Rows[0]["生效日期"] = time;
        //    dt.Rows[0]["审核"] = 1;
        //    dt.Rows[0]["审核人员"] = CPublic.Var.localUserName;
        //    dt.Rows[0]["审核人员ID"] = CPublic.Var.LocalUserID;
        //    dt.Rows[0]["审核日期"] = time;
        //    foreach (DataRow dr in dt_mx.Rows)
        //    {
        //        dr["生效"] = 1;
        //        dr["生效人员"] = dt.Rows[0]["操作员"];
        //        dr["生效人员ID"] = dt.Rows[0]["操作员ID"];
        //        dr["生效日期"] = time;
        //       dr["采购价"] =dr["单价"];
        //    }
        //    s = string.Format("select  * from  [单据审核申请表] where  作废=0 and 关联单号='{0}'", str_采购单);
        //    DataTable dt_审核=CZMaster.MasterSQL.Get_DataTable(s, strcon);
        //       dt_审核.Rows[0]["审核"] = 1;
        //    dt_审核.Rows[0]["最终审核人"] = CPublic.Var.localUserName;
        //    dt_审核.Rows[0]["最终审核人ID"] = CPublic.Var.LocalUserID;
        //    dt_审核.Rows[0]["审核时间"] = time;
        //    dt_审核.Rows[0]["文件地址"] = str_文件地址;

        //      SqlConnection conn = new SqlConnection(strcon);
        //        conn.Open();
        //        SqlTransaction ts = conn.BeginTransaction("PA"); //事务的名称
        //        SqlCommand cmd1 = new SqlCommand("select * from 采购记录采购单主表 where 1<>1", conn, ts);
        //        SqlCommand cmd = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, ts);

        //        try
        //        {
        //            SqlDataAdapter da;
        //            da = new SqlDataAdapter(cmd1);
        //            new SqlCommandBuilder(da);
        //            da.Update(dt);
        //            da = new SqlDataAdapter(cmd);
        //            new SqlCommandBuilder(da);
        //            da.Update(dt_mx);
        //             cmd = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);
        //            da = new SqlDataAdapter(cmd);
        //            new SqlCommandBuilder(da);
        //            da.Update(dt_审核);

        //            ts.Commit();
        //        }
        //        catch
        //        {
        //            ts.Rollback();
        //        }
        //    }

        //public static DataSet dss_return(DataTable )
        //{ 
        //   DataSet dss_return = new DataSet();

        //}


        public static DataSet ds_return(DataTable dt_检验, DataTable dt_mx, DataTable dt_制令, DataTable dt_领料主)
        {

            DataRow dr = dt_检验.Rows[0]; //这个就是 工单 记录 
            if (dr["生产工单类型"].ToString().Trim() == "返修工单" || Convert.ToDecimal(dr["部分完工数"]) > 0) //有过完工的 制令关闭  Convert.ToDecimal(dr["部分完工数"])>0
            {
                dt_制令.Rows[0]["关闭"] = 1;
                dt_制令.Rows[0]["关闭日期"] = CPublic.Var.getDatetime();
            }
            else   //没有过完工的 数量也返回 制令不关闭
            {
                dt_制令.Rows[0]["已排单数量"] = Convert.ToInt32(dt_制令.Rows[0]["已排单数量"]) - Convert.ToInt32(dr["生产数量"]);
                dt_制令.Rows[0]["未排单数量"] = Convert.ToInt32(dt_制令.Rows[0]["未排单数量"]) + Convert.ToInt32(dr["生产数量"]);
            }
            if (dt_领料主.Rows.Count > 0)
            {
                dt_领料主.Rows[0]["关闭"] = 1;
                dt_领料主.Rows[0]["关闭时间"] = CPublic.Var.getDatetime();
            }


            DataSet ds_return = new DataSet();

            if (dr["备注2"].Equals("报废"))
            {

                dr["作废"] = true;
                foreach (DataRow dr_son in dt_mx.Rows)
                {

                    dr_son["作废"] = true;

                }

            }

            dr["状态"] = true;
            dr["关闭"] = true;
            dr["关闭日期"] = CPublic.Var.getDatetime();
            dr["关闭人员ID"] = CPublic.Var.LocalUserID;
            dr["关闭人员"] = CPublic.Var.localUserName;
            dt_制令.TableName = "生产记录生产制令表";
            ds_return.Tables.Add(dt_制令);
            dt_领料主.TableName = "生产记录生产工单待领料主表";
            ds_return.Tables.Add(dt_领料主);

            return ds_return;
        }
        private void fun_check()
        {

            DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
            if (dr == null)
            {
                throw new Exception("当前未选择行");
            }
            string s = string.Format("select     主表名称,明细表名称,单号字段名,数量字段名,料号字段名,名称字段名 from  单据审批流配置表 where 单据类型='{0}'  ", dr["单据类型"].ToString());
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (t.Rows.Count == 0) throw new Exception("基础属性表中未维护该单据类型的相关属性值");

            // string s = string.Format("select * from  采购记录采购单主表 where 待审核=1 and 作废=0 and 采购单号='{0}'", dr["关联单号"]);

            if (t.Rows.Count == 0) //状态有变更
            {
                throw new Exception("该单据状态已更改，刷新后重试");

            }

            s = $"select  * from 单据审核申请表 where 作废=0 and 审核=0  and 审核申请单号='{dr["审核申请单号"].ToString()}'";
            t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (t.Rows.Count == 0)
            {
                throw new Exception("该单据状态已更改,刷新后重试");
            }


        }

        private void gv1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {

                DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
                if (dr == null) return;
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc1, new Point(e.X, e.Y));

                    发货信息完善ToolStripMenuItem.Visible = false;

                    if (dr["单据类型"].Equals("销售发货申请"))
                    {
                        发货信息完善ToolStripMenuItem.Visible = true;
                    }

                }
                if (dr["单据类型"].ToString() == "采购单" || dr["单据类型"].ToString() == "工单关闭" || dr["单据类型"].ToString() == "借用转耗用申请单")
                {
                    barLargeButtonItem4.Enabled = true;
                }
                else
                {
                    barLargeButtonItem4.Enabled = false;
                }


                fun_loadmx(dr["单据类型"].ToString(), dr["关联单号"].ToString());
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                DataView dv = new DataView(dt_ll);
                dv.RowFilter = string.Format("待审核人ID='{0}'", CPublic.Var.LocalUserID);
                gc1.DataSource = dv;

            }
            else
            {
                gc1.DataSource = dt_ll;
            }
        }

        private void gv1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gvP_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gv1_ColumnPositionChanged(object sender, EventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gv1.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void gv1_ColumnWidthChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gv1.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }





        private void 跳转详细信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                LookUp_RightClick();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        // 右击查看详情
        private void LookUp_RightClick()
        {
            DataRow drM = (this.BindingContext[gc1.DataSource].Current as DataRowView).Row;
            if (drM["单据类型"].ToString() == "销售单")
            {


                string sql = string.Format("select * from 销售记录销售订单主表  where 销售订单号='{0}'", drM["关联单号"].ToString());
                DataRow dr_buy = CZMaster.MasterSQL.Get_DataRow(sql, strcon);


                string sql2 = string.Format("select * from 销售记录销售订单明细表 where 销售订单号='{0}'", drM["关联单号"].ToString());
                DataTable dt_buy = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);



                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPSale.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPSale.frm销售单证详细界面", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                object[] drr = new object[3];
                drr[0] = drM["关联单号"].ToString();
                drr[1] = dr_buy;
                drr[2] = dt_buy;
                // Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;
                CPublic.UIcontrol.Showpage(ui, "订单录入");
                //    ui.ShowDialog();
            }


            else if (drM["单据类型"].ToString() == "销售单弃审申请")
            {
                string sql = string.Format("select * from 销售记录销售订单主表  where 销售订单号='{0}'", drM["关联单号"].ToString());
                DataRow dr_buy = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                string sql2 = string.Format("select * from 销售记录销售订单明细表 where 销售订单号='{0}'", drM["关联单号"].ToString());
                DataTable dt_buy = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPSale.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPSale.frm销售单证详细界面", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                object[] drr = new object[3];
                drr[0] = drM["关联单号"].ToString();
                drr[1] = dr_buy;
                drr[2] = dt_buy;
                // Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;
                CPublic.UIcontrol.Showpage(ui, "订单弃审");
                //    ui.ShowDialog();
            }


            else if (drM["单据类型"].ToString() == "销售预订单")
            {
                string sql = string.Format("select * from 销售预订单主表  where 销售预订单号='{0}'", drM["关联单号"].ToString());
                DataRow dr_buy = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                string sql2 = string.Format("select * from 销售预订单明细表 where 销售预订单号='{0}'", drM["关联单号"].ToString());
                DataTable dt_buy = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPSale.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPSale.ui销售预订单录入", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                object[] drr = new object[3];
                drr[0] = drM["关联单号"].ToString();
                drr[1] = dr_buy;
                drr[2] = dt_buy;
                // Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;
                CPublic.UIcontrol.Showpage(ui, "预订单查询");
                //    ui.ShowDialog();
            }

            else if (drM["单据类型"].ToString() == "销售预订单弃审申请")
            {
                string sql = string.Format("select * from 销售预订单主表  where 销售预订单号='{0}'", drM["关联单号"].ToString());
                DataRow dr_buy = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                string sql2 = string.Format("select * from 销售预订单明细表 where 销售预订单号='{0}'", drM["关联单号"].ToString());
                DataTable dt_buy = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPSale.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPSale.ui销售预订单录入", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                object[] drr = new object[3];
                drr[0] = drM["关联单号"].ToString();
                drr[1] = dr_buy;
                drr[2] = dt_buy;
                // Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;
                CPublic.UIcontrol.Showpage(ui, "预订单弃审");
                //    ui.ShowDialog();
            }

            else if (drM["单据类型"].ToString() == "采购单")
            {


                string sql = string.Format("select * from 采购记录采购单主表 where 采购单号='{0}'", drM["关联单号"].ToString());
                DataRow dr_buy = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                //string sql2 = string.Format("select * from 采购记录采购单主表 where 采购单号='{0}'", drM["关联单号"].ToString());
                //DataTable dt_buy = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);



                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPpurchase.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPpurchase.frm采购单明细", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                object[] drr = new object[2];
                //drr[0] = drM["关联单号"].ToString();
                drr[0] = dr_buy["GUID"].ToString();
                drr[1] = true;
                // Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;

                CPublic.UIcontrol.Showpage(ui, "特殊采购");


                //    ui.ShowDialog();


            }
            else if (drM["单据类型"].ToString() == "形态转换申请")
            {
                string sql = string.Format("select * from 销售形态转换主表  where 形态转换单号='{0}'", drM["关联单号"].ToString());
                DataRow dr_buy = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                string sql2 = string.Format("select * from 销售形态转换子表 where 形态转换单号='{0}'", drM["关联单号"].ToString());
                DataTable dt_buy = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);



                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPSale.dll")));  //  ERPSale.dll
                Type outerForm = outerAsm.GetType("ERPSale.ui形态转换单", false); //打开界面ID 字段 存的值为 ERPSale.frm报工系统
                object[] drr = new object[3];
                drr[0] = drM["关联单号"].ToString();
                drr[1] = dr_buy;
                drr[2] = dt_buy;

                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;

                CPublic.UIcontrol.Showpage(ui, "形态转换");
            }
            else if (drM["单据类型"].ToString() == "调拨申请单")
            {
                string sql = string.Format("select * from 调拨申请主表  where 调拨申请单号='{0}'", drM["关联单号"].ToString());
                DataRow dr_buy = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                string sql2 = string.Format("select * from 调拨申请明细表 where 调拨申请单号='{0}'", drM["关联单号"].ToString());
                DataTable dt_buy = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);



                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPStock.dll")));  //  ERPStock.dll
                Type outerForm = outerAsm.GetType("ERPStock.ui调拨", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                object[] drr = new object[3];
                drr[0] = drM["关联单号"].ToString();
                drr[1] = dr_buy;
                drr[2] = dt_buy;
                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;

                CPublic.UIcontrol.Showpage(ui, "调拨申请");
            }
            else if (drM["单据类型"].ToString() == "返修申请")
            {
                string sql = string.Format("select * from 新_返修申请主表  where 申请单号='{0}'", drM["关联单号"].ToString());
                DataRow dr_buy = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                string sql2 = string.Format("select * from 新_返修申请子表 where 申请单号='{0}'", drM["关联单号"].ToString());
                DataTable dt_buy = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);



                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ReworkMould.dll")));  //  ERPStock.dll
                Type outerForm = outerAsm.GetType("ReworkMould.ui返工申请", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                object[] drr = new object[3];
                drr[0] = drM["关联单号"].ToString();
                drr[1] = dr_buy;
                drr[2] = dt_buy;
                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;

                CPublic.UIcontrol.Showpage(ui, "返修申请");
            }
            else if (drM["单据类型"].ToString() == "借用申请单")
            {
                string sql = string.Format("select * from 借还申请表  where 申请批号='{0}'", drM["关联单号"].ToString());
                DataRow dr_buy = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                string sql2 = string.Format("select * from 借还申请表附表 where 申请批号='{0}'", drM["关联单号"].ToString());
                DataTable dt_buy = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);



                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "MoldMangement.dll")));
                Type outerForm = outerAsm.GetType("MoldMangement.frm借还申请", false);
                object[] drr = new object[3];
                drr[0] = drM["关联单号"].ToString();
                drr[1] = dr_buy;
                drr[2] = dt_buy;
                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;

                CPublic.UIcontrol.Showpage(ui, "借还申请");
            }
            else if (drM["单据类型"].ToString() == "拆单申请")
            {
                string sql = string.Format("select * from 拆单申请主表  where 申请单号='{0}'", drM["关联单号"].ToString());
                DataRow dr_buy = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                string sql2 = string.Format("select * from 拆单申请子表 where 申请单号='{0}'", drM["关联单号"].ToString());
                DataTable dt_buy = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);



                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPproduct.dll")));
                Type outerForm = outerAsm.GetType("ERPproduct.ui拆单申请", false);
                object[] drr = new object[3];
                drr[0] = drM["关联单号"].ToString();
                drr[1] = dr_buy;
                drr[2] = dt_buy;
                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;

                CPublic.UIcontrol.Showpage(ui, "拆单申请");
            }
            else if (drM["单据类型"].ToString() == "销售发货申请")
            {
                string sql = string.Format("select * from 销售记录销售出库通知单主表  where 出库通知单号='{0}'", drM["关联单号"].ToString());
                DataRow dr_buy = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                string sql2 = string.Format("select * from 销售记录销售出库通知单明细表 where 出库通知单号='{0}'", drM["关联单号"].ToString());
                DataTable dt_buy = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);



                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPSale.dll")));
                Type outerForm = outerAsm.GetType("ERPSale.frm销售记录成库通知单详细界面_视图", false);
                object[] drr = new object[4];
                drr[0] = drM["关联单号"].ToString();
                drr[1] = dr_buy;
                drr[2] = dt_buy;
                drr[3] = drM["相关单位"].ToString();
                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;

                CPublic.UIcontrol.Showpage(ui, "销售发货申请");
            }

            else if (drM["单据类型"].ToString() == "借用转耗用申请单")
            {
                string sql = string.Format("select * from 归还申请主表  where 归还批号='{0}'", drM["关联单号"].ToString());
                DataTable dt_ghz = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                string sql2 = string.Format("select * from 归还申请子表 where 归还批号='{0}'", drM["关联单号"].ToString());
                DataTable dt_ghmx = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);

                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "MoldMangement.dll")));
                Type outerForm = outerAsm.GetType("MoldMangement.归还转耗用查询", false);
                object[] drr = new object[2];

                drr[0] = dt_ghz;
                drr[1] = dt_ghmx;
                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;

                CPublic.UIcontrol.Showpage(ui, "借用转耗用申请查询");
            }

            else if (drM["单据类型"].ToString() == "BOM修改申请")
            {
                string sql = string.Format("select * from 基础数据BOM修改主表  where BOM修改单号='{0}'", drM["关联单号"].ToString());
                DataRow dr_buy = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                string sql2 = string.Format("select * from 基础数据BOM修改明细表 where BOM修改单号='{0}'", drM["关联单号"].ToString());
                DataTable dt_buy = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);

                //ui拆单申请 ui = new ui拆单申请(dr);
                //CPublic.UIcontrol.Showpage(ui, "拆单申请修改");
                //Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "BaseDataItem.dll")));
                //Type outerForm = outerAsm.GetType("BaseData.frmBOM修改查询", false);
                //object[] drr = new object[3];
                //drr[0] = drM["关联单号"].ToString();
                //drr[1] = dr_buy;
                //drr[2] = dt_buy;

                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "BaseDataItem.dll")));
                Type outerForm = outerAsm.GetType("BaseData.uibom树形", false);
                object[] drr = new object[2];
                drr[0] = dr_buy["产品编码"].ToString();
                drr[1] = drM["关联单号"].ToString();
                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;
                CPublic.UIcontrol.Showpage(ui, "BOM修改申请");
            }
            else if (drM["单据类型"].ToString() == "工单关闭")
            {
                string sql = string.Format("select * from 工单退料申请表  where 生产工单号='{0}'  and 退料类型='工单关闭退料'", drM["关联单号"].ToString());
                //  DataRow dr_buy = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                DataTable dtbuy = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

                if (dtbuy.Rows.Count > 0)
                {
                    string sql_mx = string.Format("select * from 工单退料申请明细表 where  待退料号='{0}' ", dtbuy.Rows[0]["待退料号"]);
                    DataTable dt_退料mx = CZMaster.MasterSQL.Get_DataTable(sql_mx, strcon);
                    Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPStock.dll")));
                    Type outerForm = outerAsm.GetType("ERPStock.ui工单关闭退料清单", false);
                    object[] drr = new object[2];
                    drr[0] = dtbuy;
                    drr[1] = dt_退料mx;
                    UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;
                    CPublic.UIcontrol.Showpage(ui, "工单关闭退料");

                }
                else
                {
                    throw new Exception("当前单据无退料");
                }

            }
            else if (drM["单据类型"].ToString() == "材料出库申请")
            {
                string sql = $"select * from 其他出入库申请主表  where 出入库申请单号='{drM["关联单号"].ToString()}' ";
                DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql, strcon);

                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "StockCore.dll")));
                Type outerForm = outerAsm.GetType("StockCore.ui材料出库申请", false);
                object[] drr = new object[2];
                drr[0] = dr;
                bool bl = true;
                drr[1] = bl;
                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;
                CPublic.UIcontrol.Showpage(ui, "材料出库申请");
            }
            else if (drM["单据类型"].ToString() == "销售退货")
            {
                 string sql = $"select * from 退货申请主表  where 退货申请单号='{drM["关联单号"].ToString()}' ";
                 DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql, strcon);

                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath,  @"ERPStock.dll" ));
                 Type outerForm = outerAsm.GetType("ERPStock.frm退货申请界面", false);
                  object[] drr = new object[2];
                 drr[0] = dr;
                 drr[1] = 2; //2表示浏览状态 
 
                 UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;
                 CPublic.UIcontrol.Showpage(ui, "退货申请");
            }
        }




        private void 发货信息完善ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try

            {
                DataRow drM = (this.BindingContext[gc1.DataSource].Current as DataRowView).Row;
                if (drM["单据类型"].ToString() == "销售发货申请")
                {
                    string sql = string.Format("select * from 销售记录销售出库通知单主表  where 出库通知单号='{0}'", drM["关联单号"].ToString());
                    DataRow dr_buy = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                    string sql2 = string.Format("select * from 销售记录销售出库通知单明细表 where 出库通知单号='{0}'", drM["关联单号"].ToString());
                    DataTable dt_buy = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);

                    Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPSale.dll")));
                    Type outerForm = outerAsm.GetType("ERPSale.ui销售出库视图", false);
                    object[] drr = new object[2];
                    drr[0] = drM["关联单号"].ToString();
                    drr[1] = drM["相关单位"].ToString();

                    UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;
                    CPublic.UIcontrol.Showpage(ui, "销售发货申请");
                }
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
                if (MessageBox.Show("是否确认驳回该单据？", "提示!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);


                    if (dr["单据类型"].ToString() == "采购单")
                    {
                        DataTable dt_审;
                        DataTable dt_采;
                        string sql_审 = string.Format("select * from  单据审核申请表 where 审核申请单号 ='{0}'", dr["审核申请单号"]);
                        dt_审 = CZMaster.MasterSQL.Get_DataTable(sql_审, strcon);
                        string sql_采 = string.Format("select * from  采购记录采购单主表 where 采购单号 ='{0}'", dr["关联单号"]);
                        dt_采 = CZMaster.MasterSQL.Get_DataTable(sql_采, strcon);

                        if (dt_采.Rows.Count > 0)
                        {
                            if (Convert.ToBoolean(dt_采.Rows[0]["待审核"]) == false)
                            {
                                throw new Exception("单据状态已改变,请刷新重试");
                            }
                        }
                        if (dt_审.Rows.Count > 0)
                        {
                            if (Convert.ToBoolean(dt_审.Rows[0]["作废"]) == true)
                            {
                                throw new Exception("单据状态已改变,请刷新重试");
                            }
                        }


                        驳回原因 fm = new 驳回原因(dr);
                        fm.ShowDialog();
                        if (fm.关闭 != 2)
                        {
                            if (fm.flag == true)
                            {
                                if (dt_采.Rows.Count > 0)
                                {
                                    dt_采.Rows[0]["驳回意见"] = fm.yijian;
                                    dt_采.Rows[0]["待审核"] = false;
                                }
                                if (dt_审.Rows.Count > 0)
                                {
                                    dt_审.Rows[0]["作废"] = true;
                                }
                                SqlConnection conn = new SqlConnection(strcon);
                                conn.Open();
                                SqlTransaction ts = conn.BeginTransaction("驳回");
                                try
                                {
                                    string sql1 = "select * from 单据审核申请表 where 1<>1";
                                    SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
                                    SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                                    new SqlCommandBuilder(da1);
                                    string sql2 = "select * from 采购记录采购单主表 where 1<>1";
                                    SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);
                                    SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                                    new SqlCommandBuilder(da2);
                                    da1.Update(dt_审);
                                    da2.Update(dt_采);
                                    ts.Commit();
                                    MessageBox.Show("驳回成功");
                                    barLargeButtonItem1_ItemClick(null, null);

                                }
                                catch (Exception ex)
                                {
                                    ts.Rollback();
                                    throw ex;
                                }
                            }
                        }


                    }
                    else if (dr["单据类型"].ToString() == "工单关闭")
                    {


                        DataTable dt_审;
                        DataTable dt_工;
                        DataTable dt_退料主 = new DataTable();
                        DataTable dt_退料子 = new DataTable();
                        string sql_审 = string.Format("select * from  单据审核申请表 where 审核申请单号 ='{0}'", dr["审核申请单号"]);
                        dt_审 = CZMaster.MasterSQL.Get_DataTable(sql_审, strcon);
                        string sql_工 = string.Format("select * from  生产记录生产工单表 where 生产工单号 ='{0}'", dr["关联单号"]);
                        dt_工 = CZMaster.MasterSQL.Get_DataTable(sql_工, strcon);

                        if (dt_工.Rows.Count > 0)
                        {
                            if (Convert.ToBoolean(dt_工.Rows[0]["状态"]) == false)
                            {
                                throw new Exception("单据状态已改变,请刷新重试");
                            }
                            else
                            {
                                dt_工.Rows[0]["状态"] = false;
                                dt_工.Rows[0]["备注2"] = "";
                                string sql_tlz = string.Format("select * from 工单退料申请表 where 生产工单号 = '{0}' and 退料类型 = '工单关闭退料' and 作废 =0", dr["关联单号"]);
                                dt_退料主 = CZMaster.MasterSQL.Get_DataTable(sql_tlz, strcon);
                                if (dt_退料主.Rows.Count > 0)
                                {
                                    dt_退料主.Rows[0]["作废"] = true;
                                    string sql_tlmx = string.Format("select * from 工单退料申请明细表 where 待退料号 = '{0}'", dt_退料主.Rows[0]["待退料号"]);
                                    dt_退料子 = CZMaster.MasterSQL.Get_DataTable(sql_tlmx, strcon);
                                    if (dt_退料子.Rows.Count > 0)
                                    {
                                        foreach (DataRow dr_退料子 in dt_退料子.Rows)
                                        {
                                            dr_退料子["关闭"] = true;
                                            dr_退料子["作废"] = true;
                                        }

                                    }
                                }
                            }
                        }
                        if (dt_审.Rows.Count > 0)
                        {
                            if (Convert.ToBoolean(dt_审.Rows[0]["作废"]) == true)
                            {
                                throw new Exception("单据状态已改变,请刷新重试");
                            }
                            else
                            {
                                dt_审.Rows[0]["作废"] = true;
                            }
                        }
                        SqlConnection conn = new SqlConnection(strcon);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("驳回");
                        try
                        {
                            string sql1 = "select * from 单据审核申请表 where 1<>1";
                            SqlCommand cmd1 = new SqlCommand(sql1, conn, ts);
                            SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da1);
                            string sql2 = "select * from 生产记录生产工单表 where 1<>1";
                            SqlCommand cmd2 = new SqlCommand(sql2, conn, ts);
                            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                            new SqlCommandBuilder(da2);
                            string sql3 = "select * from 工单退料申请表 where 1<>1";
                            SqlCommand cmd3 = new SqlCommand(sql3, conn, ts);
                            SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
                            new SqlCommandBuilder(da3);
                            string sql4 = "select * from 工单退料申请明细表 where 1<>1";
                            SqlCommand cmd4 = new SqlCommand(sql4, conn, ts);
                            SqlDataAdapter da4 = new SqlDataAdapter(cmd4);
                            new SqlCommandBuilder(da4);
                            da1.Update(dt_审);
                            da2.Update(dt_工);
                            da3.Update(dt_退料主);
                            da4.Update(dt_退料子);
                            ts.Commit();
                            MessageBox.Show("驳回成功");
                            barLargeButtonItem1_ItemClick(null, null);

                        }
                        catch (Exception ex)
                        {
                            ts.Rollback();
                            throw ex;
                        }

                    }
                    else if (dr["单据类型"].ToString() == "借用转耗用申请单")
                    {
                        //借用明细 的正在申请数 扣回去 
                        //归还申请主表 作废 
                        //单据审核申请 作废 
                        string s1 = $"select * from   单据审核申请表 where 审核申请单号 ='{dr["审核申请单号"].ToString()}'";
                        DataTable t_1 = CZMaster.MasterSQL.Get_DataTable(s1, strcon);
                        if (t_1.Rows.Count != 1) throw new Exception("单据有问题,请联系供应链");
                        t_1.Rows[0]["作废"] = true;

                        string s2 = $"select  * from 归还申请主表 where 归还批号='{dr["关联单号"].ToString()}'";
                        DataTable t_2 = CZMaster.MasterSQL.Get_DataTable(s2, strcon);
                        if (t_2.Rows.Count != 1) throw new Exception("单据有问题,请联系供应链");
                        t_2.Rows[0]["作废"] = true;
                        t_2.Rows[0]["备注x"] = "审核驳回";

                        string s3 = $"select * from 归还申请子表 where 归还批号 = '{dr["关联单号"].ToString()}'";
                        DataTable temp = CZMaster.MasterSQL.Get_DataTable(s3, strcon);

                        s3 = $"select  * from 借还申请表附表 where 申请批号='{t_2.Rows[0]["申请批号"].ToString()}'";
                        DataTable t_3 = CZMaster.MasterSQL.Get_DataTable(s3, strcon);
                        foreach (DataRow r_temp in temp.Rows)
                        {
                            DataRow[] fr = t_3.Select($"申请批号明细='{r_temp["申请批号明细"].ToString()}'");
                            if (fr.Length == 0) throw new Exception("单据有误,请联系供应链");
                            fr[0]["正在申请数"] = Convert.ToDecimal(fr[0]["正在申请数"]) - Convert.ToDecimal(r_temp["需归还数量"]);
                        }



                        SqlConnection conn = new SqlConnection(strcon);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("驳回转耗用");
                        try
                        {

                            SqlCommand cmd = new SqlCommand(s1, conn, ts);
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(da);
                            da.Update(t_1);


                            cmd = new SqlCommand(s2, conn, ts);
                            da = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(da);
                            da.Update(t_2);

                            cmd = new SqlCommand(s3, conn, ts);
                            da = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(da);
                            da.Update(t_3);


                            ts.Commit();
                            MessageBox.Show("驳回成功");
                            barLargeButtonItem1_ItemClick(null, null);

                        }
                        catch (Exception ex)
                        {
                            ts.Rollback();
                            throw ex;
                        }


                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void gvP_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //DevExpress.XtraGrid.GridControl c = sender as DevExpress.XtraGrid.GridControl;
                //DevExpress.XtraGrid.Views.Grid.GridView g = c.DefaultView as DevExpress.XtraGrid.Views.Grid.GridView;

                string s = "";
                DevExpress.XtraGrid.Views.Base.GridCell[] gcell = gvP.GetSelectedCells();
                if (gcell.Length > 0)
                {
                    IDataObject iData = Clipboard.GetDataObject();
                    string sx = (String)iData.GetData(DataFormats.Text);

                    int index = gcell[0].RowHandle;

                    for (int x = 0; x < gcell.Length; x++)
                    {
                        s += gvP.GetRowCellValue(gcell[x].RowHandle, gcell[x].Column);
                        if (x + 1 >= gcell.Length)
                        { }
                        else if (gcell[x + 1].RowHandle > gcell[x].RowHandle) s += "\r\n";
                        else
                        {
                            s += "\t";
                        }
                    }
                    Clipboard.SetDataObject(s);
                }
                else
                {

                    Clipboard.SetDataObject(gvP.GetFocusedRowCellValue(gvP.FocusedColumn).ToString());
                }
            }
            catch
            {

            }

        }

        private void gv1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.C)
                {


                    string s = "";
                    DevExpress.XtraGrid.Views.Base.GridCell[] gcell = gv1.GetSelectedCells();
                    if (gcell.Length > 0)
                    {
                        IDataObject iData = Clipboard.GetDataObject();
                        string sx = (String)iData.GetData(DataFormats.Text);

                        int index = gcell[0].RowHandle;

                        for (int x = 0; x < gcell.Length; x++)
                        {
                            s += gv1.GetRowCellValue(gcell[x].RowHandle, gcell[x].Column);
                            if (x + 1 >= gcell.Length)
                            { }
                            else if (gcell[x + 1].RowHandle > gcell[x].RowHandle) s += "\r\n";
                            else
                            {
                                s += "\t";
                            }
                        }
                        Clipboard.SetDataObject(s);
                    }
                    else
                    {

                        Clipboard.SetDataObject(gv1.GetFocusedRowCellValue(gv1.FocusedColumn).ToString());
                    }
                }
            }
            catch (Exception)
            {


            }


        }



        private void gv1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);


                if (dr["单据类型"].ToString() == "采购单" || dr["单据类型"].ToString() == "工单关闭")
                {
                    barLargeButtonItem4.Enabled = true;
                }
                else
                {
                    barLargeButtonItem4.Enabled = false;
                }


                fun_loadmx(dr["单据类型"].ToString(), dr["关联单号"].ToString());
            }
            catch (Exception)
            {

            }
        }
    }
}
