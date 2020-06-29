using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace 郭恒的DEMO
{
    public partial class sync_u8_借出借用单 : Form
    {
        string strcon = CPublic.Var.strConn;
        string strcon_dw = CPublic.Var.geConn("DW");
        DataTable save_jcMain;
        DataTable save_jcDetail;
        DataTable save_jcgh;
        /// <summary>
        /// 其他出库
        /// </summary>
        DataTable save_qtckMain;
        /// <summary>
        /// 其他出库子
        /// </summary>
        DataTable save_qtckDetail;

        DataTable changed_list;

        /// <summary>
        /// 其他出入库申请主
        /// </summary>
        DataTable save_qtsqMain;
        /// <summary>
        /// 其他出入库申请子
        /// </summary>
        DataTable save_qtsqDetail;

        DataTable save_qtrkMain;
        DataTable save_qtrkDetail;

        DataTable save_saleMain;
        DataTable save_saleDetail;
        /// <summary>
        /// 销售出库通知
        /// </summary>
        DataTable save_saletzMain;
        DataTable save_saletzDetail;
        /// <summary>
        ///销售出库
        /// </summary>
        DataTable save_saleckMain;
        DataTable save_saleckDetail;
        DataTable u8_list;
        DataTable u8_jcmx;
        DataTable stock_crmx;

        public sync_u8_借出借用单()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadAlltable();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //所有借出借用单记录  //生成借出借用单 和 归还记录 仓库出入库明细 
            int total = u8_list.Rows.Count + changed_list.Rows.Count;
            int i = 1;
            foreach (DataRow dr in u8_list.Rows)
            {
                deal_2(dr);
                button3.Text = i.ToString() + "/" + total.ToString();
                i++;
                Application.DoEvents();
            }

            foreach (DataRow xr in changed_list.Rows)
            {
                fun_changed(xr);
                button3.Text = i.ToString() + "/" + total.ToString();
                i++;
                Application.DoEvents();
            }
            //再取有转换单记录得 根据 类型 分别生成  其他出库 或者 销售单销售出库。。。。

        }
        /// <summary>
        /// istatus=1 的 只要生成 借还申请表 和附表
        /// 弃用                弃用          弃用
        /// </summary>
        /// <param name="dr">u8_list中行数据</param>
        private void deal_1or4(DataRow dr)
        {
            DataRow r = save_jcMain.NewRow();
            r["申请批号"] = dr["cCODE"].ToString().Trim();
            r["借用类型"] = dr["cType"].ToString().Trim();
            //r["原因分类"] = dr["cType"].ToString().Trim();
            r["申请人"] = dr["cMaker"].ToString().Trim();
            // r["工号"] = dr["cMaker"].ToString().Trim();
            r["申请日期"] = dr["ddate"].ToString().Trim();
            r["备注"] = dr["cmemo"].ToString().Trim();
            r["目标客户"] = dr["cdefine2"] == null ? "" : dr["cdefine2"].ToString().Trim();
            r["联系地址"] = dr["cContactWay"] == null ? "" : dr["cContactWay"].ToString().Trim();
            // r["联系人"] = dr["cContactPerson"] == null ? "" : dr["cContactWay"].ToString().Trim();
            r["借用人员"] = dr["cContactPerson"] == null ? "" : dr["cContactPerson"].ToString().Trim();
            r["相关单位"] = dr["ccusName"] == null ? "" : dr["ccusName"].ToString().Trim();
            save_jcMain.Rows.Add(r);
            string s = string.Format(@"select  HY_DZ_BorrowOuts.*,物料名称,规格型号,cwhname from [192.168.20.150].UFDATA_008_2018.dbo.HY_DZ_BorrowOuts 
  left join 基础数据物料信息表 base on base.物料编码=cinvcode
  left  join [192.168.20.150].UFDATA_008_2018.dbo.Warehouse on Warehouse.cWhCode=HY_DZ_BorrowOuts.cWhCode
   where ID='{0}'  ", dr["ID"]);
            DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            int pos = 1;
            foreach (DataRow xx in temp.Rows)
            {
                DataRow rr = save_jcDetail.NewRow();
                rr["申请批号"] = dr["cCODE"].ToString().Trim();
                rr["申请批号明细"] = dr["cCODE"].ToString().Trim() + "-" + pos++.ToString("00");
                rr["物料名称"] = xx["物料名称"];
                rr["规格型号"] = xx["规格型号"];
                rr["物料编码"] = xx["物料编码"];
                rr["申请日期"] = dr["ddate"];
                rr["申请数量"] = xx["iquantity"];
                rr["仓库名称"] = xx["cwhname"];
                rr["仓库号"] = xx["cwhcode"];
                rr["备注"] = xx["cmemo"];
                rr["借还状态"] = "未借出";
                rr["备注1"] = xx["cfree1"] == null ? "" : xx["cfree1"].ToString();

                rr["备注2"] = xx["cfree2"] == null ? "" : xx["cfree2"].ToString();
                save_jcDetail.Rows.Add(rr);
                pos++;
            }



        }



        /// <summary>
        /// 先生成借用单据,
        /// </summary>
        /// <param name="dr"></param>
        private void deal_2(DataRow dr)
        {
            DataRow r = save_jcMain.NewRow();
            r["申请批号"] = dr["cCODE"].ToString().Trim();
            r["借用类型"] = dr["cType"].ToString().Trim();
            //r["原因分类"] = dr["cType"].ToString().Trim();
            r["申请人"] = dr["cMaker"].ToString().Trim();
            if (dr["cHandler"] != null)
            {
                r["审核人员"] = dr["cHandler"].ToString().Trim();
            }
            r["审核日期"] = dr["dVeriDate"];

            r["申请日期"] = dr["ddate"].ToString().Trim();
            if (dr["cmemo"] != null)
            {
                r["备注"] = dr["cmemo"].ToString().Trim();
            }

            r["目标客户"] = dr["cdefine2"] == null ? "" : dr["cdefine2"].ToString().Trim();
            r["联系地址"] = dr["cContactWay"] == null ? "" : dr["cContactWay"].ToString().Trim();
            // r["联系人"] = dr["cContactPerson"] == null ? "" : dr["cContactWay"].ToString().Trim();
            r["借用人员"] = dr["cContactPerson"] == null ? "" : dr["cContactPerson"].ToString().Trim();
            r["相关单位"] = dr["ccusName"] == null ? "" : dr["ccusName"].ToString().Trim();
            save_jcMain.Rows.Add(r);
            string s = string.Format(@"select  HY_DZ_BorrowOuts.*,物料名称,规格型号,cwhname from [192.168.20.150].UFDATA_008_2018.dbo.HY_DZ_BorrowOuts 
  left join 基础数据物料信息表 base on base.物料编码=cinvcode
  left  join [192.168.20.150].UFDATA_008_2018.dbo.Warehouse on Warehouse.cWhCode=HY_DZ_BorrowOuts.cWhCode
   where ID='{0}'  ", dr["ID"]);
            DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            int pos = 1;
            foreach (DataRow xx in temp.Rows)
            {
                DataRow rr = save_jcDetail.NewRow();
                rr["申请批号"] = dr["cCODE"].ToString().Trim();
                rr["申请批号明细"] = dr["cCODE"].ToString().Trim() + "-" + pos.ToString("00");
                rr["物料名称"] = xx["物料名称"];
                rr["规格型号"] = xx["规格型号"];
                rr["物料编码"] = xx["cinvcode"];
                rr["申请日期"] = dr["ddate"];
                rr["申请数量"] = xx["iquantity"];
                rr["仓库名称"] = xx["cwhname"];
                rr["仓库号"] = xx["cwhcode"];
                rr["备注"] = xx["cmemo"] == null ? "" : xx["cmemo"].ToString();
                rr["备注1"] = xx["cfree1"] == null ? "" : xx["cfree1"].ToString();

                rr["备注2"] = xx["cfree2"] == null ? "" : xx["cfree2"].ToString();
                if (xx["iQtyOut"] != null && xx["iQtyOut"] != DBNull.Value && Convert.ToDecimal(xx["iQtyOut"]) > 0) //
                {
                    //有借出记录    但是我们系统之前王立做的时候 没有借出记录表  后来改的时候 也就没加  直接记录在仓库出入库明细表中 
                    DataRow stock_Rr = stock_crmx.NewRow();
                    stock_crmx.Rows.Add(stock_Rr);
                    stock_Rr["GUID"] = System.Guid.NewGuid();
                    stock_Rr["明细类型"] = "借用出库";
                    stock_Rr["单号"] = dr["cCODE"].ToString().Trim();
                    stock_Rr["物料编码"] = xx["cinvcode"];
                    stock_Rr["物料名称"] = xx["物料名称"];
                    stock_Rr["明细号"] = rr["申请批号明细"];
                    stock_Rr["相关单号"] = rr["申请批号"];

                    stock_Rr["出库入库"] = "出库";
                    stock_Rr["实效数量"] = -Convert.ToDecimal(xx["iQtyOut"]);
                    stock_Rr["实效时间"] = dr["ddate"].ToString().Trim();
                    stock_Rr["出入库时间"] = dr["ddate"].ToString().Trim();
                    //  stock_Rr["相关单号"] = dr_借还["申请批号"];
                    stock_Rr["相关单位"] = r["相关单位"];
                    stock_Rr["仓库号"] = rr["仓库号"];
                    stock_Rr["仓库名称"] = rr["仓库名称"];
                    //stock_Rr["单位"] = dr["计量单位"];
                    rr["已借出数量"] = xx["iQtyOut"];

                }
                else
                {

                    rr["已借出数量"] = 0;
                    rr["借还状态"] = "未借出";
                }


                if (Convert.ToDecimal(rr["已借出数量"]) >= Convert.ToDecimal(rr["申请数量"]))
                {
                    rr["领取完成"] = true;
                    rr["借还状态"] = "已借出";
                }

                decimal dec_return = 0;
                decimal dec = 0;

                bool bl;
                bl = decimal.TryParse(xx["iQtyBack"].ToString(), out dec);
                dec_return = dec_return + dec;
                bl = decimal.TryParse(xx["iQtyCOver"].ToString(), out dec); //耗用  iQtyCOut   iQtyCOver 9-16 发现取错字段了
                dec_return = dec_return + dec;
                bl = decimal.TryParse(xx["iQtyCSale"].ToString(), out dec); //转销售
                dec_return = dec_return + dec;
                bl = decimal.TryParse(xx["iQtyCFree"].ToString(), out dec); //转赠送
                dec_return = dec_return + dec;
                rr["归还数量"] = dec_return;  //正常归还or转耗用or转销售or转赠送sum>0 
                if (dec_return > 0)
                {
                    rr["借还状态"] = "已归还";
                    //生成   借还申请表归还记录  和 仓库出入库明细表 
                    s = string.Format(@" select  ghz.ccode,ghmx.upAutoID,ghz.ctype,ghz.ddate,ghz.cmemo 归还备注 ,ghz.cmaker,ghz.chandler,ghmx.cinvcode,ghmx.cwhcode, Warehouse.cwhname,物料名称,规格型号,ghmx.iquantity
   from [192.168.20.150].UFDATA_008_2018.dbo.HY_DZ_BorrowOutBack ghz
  left join [192.168.20.150].UFDATA_008_2018.dbo.HY_DZ_BorrowOutBacks ghmx on ghz.ID=ghmx.ID
  left join [192.168.20.150].UFDATA_008_2018.dbo.HY_DZ_BorrowOuts jcmx on ghmx.upAutoID=jcmx.AutoID
  left join [192.168.20.150].UFDATA_008_2018.dbo.HY_DZ_BorrowOut jcz on jcz.ID=jcmx.ID
  left join 基础数据物料信息表 base on base.物料编码=ghmx.cinvcode
   left  join [192.168.20.150].UFDATA_008_2018.dbo.Warehouse on Warehouse.cWhCode=ghmx.cWhCode
  where jcmx.AutoID='{0}'", xx["AutoID"]);
                    //这里只能搜索到一条        借还申请表归还记录插入
                    DataRow gh_r = CZMaster.MasterSQL.Get_DataRow(s, strcon);
                    string str_returnRecord = "";
                    string str_cmaker = "";
                    DateTime t_back;
                    if (gh_r == null)  //用友里面不是归还得 比如转销售转赠送转耗用  这里搜不到记录
                    {
                       // DateTime t = CPublic.Var.getDatetime();
                        DateTime t = new DateTime (2019,4,30);

                        str_returnRecord = string.Format("RA{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
            t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("RA", t.Year, t.Month).ToString("0000"));
                        str_cmaker = "系统归还";
                        t_back = t;
                    }
                    else
                    {
                        str_returnRecord = gh_r["ccode"].ToString();
                        str_cmaker = gh_r["cMaker"].ToString();
                        t_back = Convert.ToDateTime(gh_r["ddate"]);
                    }
                    DataRow r_back = save_jcgh.NewRow();
                    r_back["guid"] = System.Guid.NewGuid();
                    r_back["申请批号"] = str_returnRecord;
                    r_back["申请批号明细"] = str_returnRecord + "-" + pos.ToString("00");
                    r_back["借用申请明细号"] = rr["申请批号明细"];
                    r_back["物料名称"] = xx["物料名称"];
                    r_back["物料编码"] = xx["cinvcode"];
                    r_back["归还日期"] = t_back;
                    r_back["规格型号"] = xx["规格型号"];
                    //r_back["货架描述"] = dr["货架描述"];
                    r_back["仓库号"] = xx["cwhcode"];
                    r_back["仓库名称"] = xx["cwhname"];
                    r_back["归还数量"] = xx["iquantity"];
                    r_back["归还操作人"] = str_cmaker;
                    save_jcgh.Rows.Add(r_back);
                    //仓库出入库明细表 插入数据
                    DataRow stock_Rr = stock_crmx.NewRow();
                    stock_crmx.Rows.Add(stock_Rr);
                    stock_Rr["GUID"] = System.Guid.NewGuid();
                    stock_Rr["明细类型"] = "归还入库";
                    stock_Rr["单号"] = r_back["申请批号"];
                    stock_Rr["物料编码"] = xx["cinvcode"];
                    stock_Rr["物料名称"] = xx["物料名称"];
                    stock_Rr["明细号"] = r_back["申请批号明细"];
                    stock_Rr["出库入库"] = "入库";
                    stock_Rr["实效数量"] = xx["iquantity"];
                    stock_Rr["实效时间"] = t_back;
                    stock_Rr["出入库时间"] = t_back;
                    stock_Rr["相关单号"] = rr["申请批号"];
                    stock_Rr["相关单位"] = r["相关单位"];  //这里取r得相关单位是一样得
                    stock_Rr["仓库号"] = xx["cwhcode"];
                    stock_Rr["仓库名称"] = xx["cwhname"];
                    //stock_Rr["单位"] = dr["计量单位"];
                }

                if (dec_return >= Convert.ToDecimal(xx["iquantity"]))
                    rr["归还完成"] = true;

                save_jcDetail.Rows.Add(rr);
                pos++;
                //if (dec > 0)  //转耗用得 
                //{
                //                    #region 先归还
                //                    //生成   借还申请表归还记录  和 仓库出入库明细表 
                //                    s = string.Format(@" select  ghz.ccode,ghmx.upAutoID,ghz.ctype,ghz.ddate,ghz.cmemo 归还备注 ,ghz.cmaker,ghz.chandler,ghmx.cinvcode,ghmx.cwhcode, Warehouse.cwhname,物料名称,规格型号,ghmx.iquantity
                //   from [192.168.20.150].UFDATA_008_2018.dbo.HY_DZ_BorrowOutBack ghz
                //  left join [192.168.20.150].UFDATA_008_2018.dbo.HY_DZ_BorrowOutBacks ghmx on ghz.ID=ghmx.ID
                //  left join [192.168.20.150].UFDATA_008_2018.dbo.HY_DZ_BorrowOuts jcmx on ghmx.upAutoID=jcmx.AutoID
                //  left join [192.168.20.150].UFDATA_008_2018.dbo.HY_DZ_BorrowOut jcz on jcz.ID=jcmx.ID
                //  left join 基础数据物料信息表 base on base.物料编码=ghmx.cinvcode
                //   left  join [192.168.20.150].UFDATA_008_2018.dbo.Warehouse on Warehouse.cWhCode=ghmx.cWhCode
                //  where jcmx.AutoID='{0}'", xx["AutoID"]);
                //                    //这里只能搜索到一条        借还申请表归还记录插入
                //                    DataRow gh_r = CZMaster.MasterSQL.Get_DataRow(s, strcon);
                //                    DataRow r_back = save_jcgh.NewRow();
                //                    r_back["guid"] = System.Guid.NewGuid();
                //                    r_back["申请批号"] = gh_r["ccode"];
                //                    r_back["申请批号明细"] = gh_r["ccode"] + "-" + pos.ToString("00");
                //                    r_back["借用申请明细号"] = rr["申请批号明细"];
                //                    r_back["物料名称"] = gh_r["物料名称"];
                //                    r_back["物料编码"] = gh_r["cinvcode"];
                //                    r_back["归还日期"] = gh_r["ddate"];
                //                    r_back["规格型号"] = gh_r["规格型号"];
                //                    r_back["仓库号"] = gh_r["cwhcode"];
                //                    r_back["仓库名称"] = gh_r["cwhname"];
                //                    r_back["归还数量"] = gh_r["iquantity"];
                //                    r_back["归还操作人"] = gh_r["cMaker"];
                //                    save_jcgh.Rows.Add(r_back);
                //                    //仓库出入库明细表 插入数据
                //                    DataRow stock_Rr = stock_crmx.NewRow();
                //                    stock_crmx.Rows.Add(stock_Rr);
                //                    stock_Rr["GUID"] = System.Guid.NewGuid();
                //                    stock_Rr["明细类型"] = "归还入库";
                //                    stock_Rr["单号"] = gh_r["ccode"];
                //                    stock_Rr["物料编码"] = gh_r["物料编码"];
                //                    stock_Rr["物料名称"] = gh_r["物料名称"];
                //                    stock_Rr["明细号"] = gh_r["申请批号明细"];
                //                    stock_Rr["出库入库"] = "入库";
                //                    stock_Rr["实效数量"] = gh_r["iquantity"];
                //                    stock_Rr["实效时间"] = gh_r["ddata"];
                //                    stock_Rr["出入库时间"] = gh_r["ddata"];
                //                    stock_Rr["相关单位"] = r["相关单位"];  //这里取r得相关单位是一样得
                //                    stock_Rr["仓库号"] = gh_r["cwhcode"];
                //                    stock_Rr["仓库名称"] = gh_r["cwhname"];
                //                    #endregion

                //                    #region 其他出入库申请出库  其他出库  仓库出入库明细表
                //                    //这里要取 相关其他出库记录 
                //                    s=string.Format(@"select  * from  [192.168.20.150].UFDATA_008_2018.dbo.HY_DZ_BorrowOut jcz 
                //  inner join  [192.168.20.150].UFDATA_008_2018.dbo.HY_DZ_BorrowOuts jcmx on jcz.ID=jcmx.ID 
                //inner join [192.168.20.150].UFDATA_008_2018.dbo.HY_DZ_BorrowOutChange zhd on zhd.UpID=jcz.ID
                //inner join  [192.168.20.150].UFDATA_008_2018.dbo.RdRecord09  qtcz on qtcz.ccode=zhd.DownstreamCode
                //inner join [192.168.20.150].UFDATA_008_2018.dbo.RdRecords09 qtcmx on qtcz.ID=qtcmx.ID
                //where jcmx.AutoID='{0}'", xx["AutoID"]);
                //                DataRow  t_qtc=CZMaster.MasterSQL.Get_DataRow(s,strcon);
                //                DataRow r_qtsqMain= save_qtsqMain.NewRow();
                //                save_qtsqMain.Rows.Add(r_qtsqMain);
                //                r_qtsqMain["GUID"] = System.Guid.NewGuid();
                //                r_qtsqMain["出入库申请单号"] =t_qtc[""]  ;   //用友里面没有申请记录 那这边就存 dwjc
                //                r_qtsqMain["申请日期"] =t_qtc["ddate"] ;
                //                r_qtsqMain["申请类型"] = "其他出库";
                //                r_qtsqMain["备注"] = "借出转耗用";
                //                r_qtsqMain["备注1"] = t_qtc["借用单号"];

                //               // r_qtsqMain["操作人员编号"] = CPublic.Var.LocalUserID;
                //                r_qtsqMain["操作人员"] =  t_qtc["cmaker"];
                //                r_qtsqMain["生效"] = true;
                //                r_qtsqMain["生效日期"] =  t_qtc["ddate"] ;
                //                //r_qtsqMain["生效人员编号"] = CPublic.Var.LocalUserID;
                //                r_qtsqMain["完成"] = true;
                //                r_qtsqMain["完成日期"] = t_qtc["ddate"] ;
                //                r_qtsqMain["原因分类"] = "耗用";


                //                DataRow r_qtck = save_qtckMain.NewRow();
                //                save_qtckMain.Rows.Add(r_qtck);
                //                r_qtck["GUID"] = System.Guid.NewGuid();
                //                r_qtck["其他出库单号"] = s_其他出库单号;
                //                r_qtck["出库类型"] = "其他出库";
                //                r_qtck["操作人员编号"] = CPublic.Var.LocalUserID;
                //                r_qtck["操作人员"] = CPublic.Var.localUserName;
                //                r_qtck["出库日期"] = t;
                //                r_qtck["生效"] = true;
                //                r_qtck["生效日期"] = t;
                //                r_qtck["创建日期"] = t;
                //                r_qtck["出入库申请单号"] = s_其他出入库申请单号;


                //                int i = 1;
                //                foreach (DataRow rr in dt_归还记录.Rows)
                //                {
                //                    DataRow r_qtsqmx = save_qtsqDetail.NewRow();
                //                    save_qtsqDetail.Rows.Add(r_qtsqmx);
                //                    r_qtsqmx["GUID"] = System.Guid.NewGuid();
                //                    r_qtsqmx["出入库申请单号"] = s_其他出入库申请单号;
                //                    r_qtsqmx["POS"] = i;
                //                    r_qtsqmx["出入库申请明细号"] = s_其他出入库申请单号 + "-" + i.ToString("00");
                //                    r_qtsqmx["物料编码"] = rr["物料编码"];
                //                    //dr_其他出入库申请子["原ERP物料编号"] = dr_借用明细["原ERP物料编号"];
                //                    r_qtsqmx["物料名称"] = rr["物料名称"];
                //                    r_qtsqmx["数量"] = rr["归还数量"];
                //                    //dr_其他出入库申请子["n原ERP规格型号"] = dr_借用明细["n原ERP规格型号"];
                //                    r_qtsqmx["备注"] = rr["借用申请明细号"];
                //                    r_qtsqmx["生效"] = true;
                //                    r_qtsqmx["生效日期"] = t;
                //                    r_qtsqmx["生效人员编号"] = CPublic.Var.LocalUserID;
                //                    r_qtsqmx["完成"] = true;
                //                    r_qtsqmx["完成日期"] = t;
                //                    r_qtsqmx["仓库号"] = rr["仓库号"];
                //                    r_qtsqmx["仓库名称"] = rr["仓库名称"];
                //                    r_qtsqmx["货架描述"] = rr["货架描述"];

                //                    DataRow r_qtckmx = save_qtckDetail.NewRow();
                //                    save_qtckDetail.Rows.Add(r_qtckmx);
                //                    r_qtckmx["物料编码"] = rr["物料编码"]; 
                //                    r_qtckmx["物料名称"] = rr["物料名称"];
                //                    r_qtckmx["数量"] = rr["归还数量"];

                //                    r_qtckmx["规格型号"] = rr["规格型号"];

                //                    r_qtckmx["其他出库单号"] = s_其他出库单号;
                //                    r_qtckmx["POS"] = i;
                //                    r_qtckmx["其他出库明细号"] = s_其他出库单号 + "-" + i++.ToString("00");
                //                    r_qtckmx["GUID"] = System.Guid.NewGuid();
                //                    r_qtckmx["备注"] = rr["借用申请明细号"];
                //                    r_qtckmx["生效"] = true;
                //                    r_qtckmx["生效日期"] = t;
                //                    r_qtckmx["生效人员编号"] = CPublic.Var.LocalUserID;
                //                    r_qtckmx["完成"] = true;
                //                    r_qtckmx["完成日期"] = t;
                //                    r_qtckmx["完成人员编号"] = CPublic.Var.LocalUserID;
                //                    r_qtckmx["出入库申请单号"] = s_其他出入库申请单号;
                //                    r_qtckmx["出入库申请明细号"] = r_qtsqmx["出入库申请明细号"];

                //                    DataRow r_stockcr = stock_crmx.NewRow();
                //                    stock_crmx.Rows.Add(r_stockcr);
                //                    r_stockcr["GUID"] = System.Guid.NewGuid();
                //                    r_stockcr["明细类型"] = "其他出库";
                //                    r_stockcr["单号"] = s_其他出库单号;
                //                    r_stockcr["物料编码"] = rr["物料编码"];
                //                    r_stockcr["物料名称"] = rr["物料名称"];
                //                    r_stockcr["明细号"] = r_qtckmx["其他出库明细号"];
                //                    r_stockcr["出库入库"] = "出库";
                //                    r_stockcr["实效数量"] = "-" + rr["归还数量"];
                //                    r_stockcr["实效时间"] = t;
                //                    r_stockcr["出入库时间"] = t;
                //                    r_stockcr["相关单号"] = s_其他出入库申请单号;
                //                    r_stockcr["仓库号"] = rr["仓库号"];
                //                    r_stockcr["仓库名称"] = rr["仓库名称"];
                //                    r_stockcr["相关单位"] = s_相关单位;



                //                }
                //                dec_return = dec_return + dec;


                //                bl = decimal.TryParse(xx["iQtyCSale"].ToString(), out dec); //转销售
                //                dec_return = dec_return + dec;
                //                bl = decimal.TryParse(xx["iQtyCFree"].ToString(), out dec); //转赠送
                //                dec_return = dec_return + dec;
                //                rr["归还数量"] = dec_return;
                //                if (dec_return >= Convert.ToDecimal(xx["iquantity"]))
                //                    rr["归还完成"] = true;
                //                save_jcDetail.Rows.Add(rr);
                //                pos++;
                //#endregion
            }



        }

        private void fun_changed(DataRow dr)
        {
            if (dr["ChangeType"].ToString() == "借出转耗用")
            {
                //这里要取 相关其他出库记录 
                string s = string.Format(@"select bobjectcode,ccusName,ccode,ddate,cmaker,chandler,cmemo
                ,cinvcode,物料名称,规格型号,cwhcode,cwhname,iquantity, 审核人员,  申请人员
                from (
select jcz.bobjectcode,ccusName,qtcz.ccode,qtcz.ddate,jcz.cmaker,jcz.chandler,jcz.cmemo
                ,qtcmx.cinvcode,物料名称,规格型号,qtcz.cwhcode,cwhname,qtcmx.iquantity,yg.员工号 as 审核人员,isnull(sqyg.员工号,'') as 申请人员
                 from  [192.168.20.150].UFDATA_008_2018.dbo.HY_DZ_BorrowOut jcz 
                inner join  [192.168.20.150].UFDATA_008_2018.dbo.HY_DZ_BorrowOuts jcmx on jcz.ID=jcmx.ID 
                inner join [192.168.20.150].UFDATA_008_2018.dbo.HY_DZ_BorrowOutChange zhd on zhd.UpID=jcz.ID
                inner join  [192.168.20.150].UFDATA_008_2018.dbo.RdRecord09  qtcz on qtcz.ccode=zhd.DownstreamCode
                inner join [192.168.20.150].UFDATA_008_2018.dbo.RdRecords09 qtcmx on qtcz.ID=qtcmx.ID
                inner join 基础数据物料信息表 base on base.物料编码=qtcmx.cinvcode
                left  join 人事基础员工表 yg on yg.姓名=jcz.chandler
                left  join 人事基础员工表 sqyg on sqyg.姓名=jcz.cmaker

                 INNER  join [192.168.20.150].UFDATA_008_2018.dbo.Warehouse on Warehouse.cWhCode=qtcz.cwhcode
                 left join   (select  ccuscode,ccusName from  [192.168.20.150].UFDATA_008_2018.dbo.Customer   
                    union select   cDepCode as ccuscode ,cdepname ccusName from  [192.168.20.150].UFDATA_008_2018.dbo.Department )a
                 on a.cCusCode=jcz.bObjectCode
                where jcz.ccode='{0}') x
                group  by bobjectcode,ccusName,ccode,ddate,cmaker,chandler,cmemo
                ,cinvcode,物料名称,规格型号,cwhcode,cwhname,iquantity, 审核人员,申请人员
                ", dr["借出单"]);
                DataTable t_qtc = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                DataRow r_qtsqMain = save_qtsqMain.NewRow();
                save_qtsqMain.Rows.Add(r_qtsqMain);
                r_qtsqMain["GUID"] = System.Guid.NewGuid();
                r_qtsqMain["出入库申请单号"] = t_qtc.Rows[0]["ccode"];   //用友里面没有申请记录 那这边就存 dwqc
                r_qtsqMain["申请日期"] = t_qtc.Rows[0]["ddate"];
                r_qtsqMain["申请类型"] = "其他出库";
                r_qtsqMain["备注"] = t_qtc.Rows[0]["cmemo"] == null ? "" : t_qtc.Rows[0]["cmemo"].ToString();
                r_qtsqMain["备注1"] = dr["借出单"];


                r_qtsqMain["操作人员编号"] = t_qtc.Rows[0]["申请人员"];
                r_qtsqMain["操作人员"] = t_qtc.Rows[0]["cmaker"];
                r_qtsqMain["生效"] = true;
                r_qtsqMain["生效日期"] = t_qtc.Rows[0]["ddate"];

                r_qtsqMain["审核人员"] = t_qtc.Rows[0]["chandler"];

                r_qtsqMain["生效人员编号"] = t_qtc.Rows[0]["申请人员"];
                r_qtsqMain["完成"] = true;
                r_qtsqMain["完成日期"] = t_qtc.Rows[0]["ddate"];
                r_qtsqMain["原因分类"] = "借出转耗用";
                r_qtsqMain["相关单位"] = t_qtc.Rows[0]["ccusName"] == null ? "" : t_qtc.Rows[0]["ccusName"].ToString(); ;

                DataRow r_qtck = save_qtckMain.NewRow();
                save_qtckMain.Rows.Add(r_qtck);
                r_qtck["GUID"] = System.Guid.NewGuid();
                r_qtck["其他出库单号"] = t_qtc.Rows[0]["ccode"];
                r_qtck["出库类型"] = "其他出库";
                r_qtck["操作人员编号"] = t_qtc.Rows[0]["审核人员"];
                r_qtck["操作人员"] = t_qtc.Rows[0]["chandler"];
                r_qtck["出库日期"] = t_qtc.Rows[0]["ddate"];
                r_qtck["生效"] = true;
                r_qtck["生效日期"] = t_qtc.Rows[0]["ddate"];
                r_qtck["创建日期"] = t_qtc.Rows[0]["ddate"];
                r_qtck["出入库申请单号"] = t_qtc.Rows[0]["ccode"];//dr["借出单"]

                r_qtck["备注"] = dr["借出单"];


                int pos = 1;
                foreach (DataRow mx in t_qtc.Rows)
                {
                    DataRow qtsq_detail = save_qtsqDetail.NewRow();
                    save_qtsqDetail.Rows.Add(qtsq_detail);
                    qtsq_detail["GUID"] = System.Guid.NewGuid();
                    qtsq_detail["出入库申请单号"] = mx["ccode"];
                    qtsq_detail["POS"] = pos;
                    qtsq_detail["出入库申请明细号"] = mx["ccode"] + "-" + pos.ToString("00");
                    qtsq_detail["物料编码"] = mx["cinvcode"];
                    //dr_其他出入库申请子["原ERP物料编号"] = dr_借用明细["原ERP物料编号"];
                    qtsq_detail["物料名称"] = mx["物料名称"];
                    qtsq_detail["数量"] = mx["iquantity"];
                    //dr_其他出入库申请子["n原ERP规格型号"] = dr_借用明细["n原ERP规格型号"];
                    qtsq_detail["备注"] = dr["借出单"];
                    qtsq_detail["生效"] = true;
                    qtsq_detail["生效日期"] = t_qtc.Rows[0]["ddate"];
                    qtsq_detail["生效人员编号"] = t_qtc.Rows[0]["申请人员"];
                    qtsq_detail["完成"] = true;
                    qtsq_detail["完成日期"] = t_qtc.Rows[0]["ddate"];
                    qtsq_detail["仓库号"] = mx["cwhcode"];
                    qtsq_detail["仓库名称"] = mx["cwhname"];
                    //qtsq_detail["货架描述"] = rr["货架描述"];

                    DataRow r_qtckmx = save_qtckDetail.NewRow();
                    save_qtckDetail.Rows.Add(r_qtckmx);
                    r_qtckmx["物料编码"] = mx["cinvcode"];

                    r_qtckmx["物料名称"] = mx["物料名称"];
                    r_qtckmx["数量"] = mx["iquantity"];

                    r_qtckmx["规格型号"] = mx["规格型号"];

                    r_qtckmx["其他出库单号"] = mx["ccode"];
                    r_qtckmx["POS"] = pos;
                    r_qtckmx["其他出库明细号"] = mx["ccode"] + "-" + pos++.ToString("00");
                    r_qtckmx["GUID"] = System.Guid.NewGuid();
                    r_qtckmx["备注"] = dr["借出单"];
                    r_qtckmx["生效"] = true;
                    r_qtckmx["生效日期"] = t_qtc.Rows[0]["ddate"];
                    r_qtckmx["生效人员编号"] = t_qtc.Rows[0]["审核人员"];
                    r_qtckmx["完成"] = true;
                    r_qtckmx["完成日期"] = t_qtc.Rows[0]["ddate"];
                    r_qtckmx["完成人员编号"] = t_qtc.Rows[0]["审核人员"];
                    r_qtckmx["出入库申请单号"] = mx["ccode"];
                    r_qtckmx["出入库申请明细号"] = qtsq_detail["出入库申请明细号"];

                    DataRow r_stockcrmx = stock_crmx.NewRow();
                    stock_crmx.Rows.Add(r_stockcrmx);
                    r_stockcrmx["GUID"] = System.Guid.NewGuid();
                    r_stockcrmx["明细类型"] = "其他出库";
                    r_stockcrmx["单号"] = mx["ccode"];
                    r_stockcrmx["物料编码"] = mx["cinvcode"];
                    r_stockcrmx["物料名称"] = mx["物料名称"];
                    r_stockcrmx["明细号"] = r_qtckmx["其他出库明细号"];
                    r_stockcrmx["出库入库"] = "出库";
                    r_stockcrmx["实效数量"] = "-" + mx["iquantity"];
                    r_stockcrmx["实效时间"] = mx["ddate"];
                    r_stockcrmx["出入库时间"] = mx["ddate"];
                    r_stockcrmx["相关单号"] = mx["ccode"];
                    r_stockcrmx["仓库号"] = mx["cwhcode"];
                    r_stockcrmx["仓库名称"] = mx["cwhname"];
                    r_stockcrmx["相关单位"] = t_qtc.Rows[0]["ccusName"];

                }



            }
            else  //转销售或者转赠品
            {
                // 生成销售订单、明细 通知单 、明细
                string s = string.Format(@"select  RIGHT(fh.cdlcode,10)流水号,fh.cinvoicecompany,fh.ccusperson,fh.cdlcode 通知单号,fh.cMaker,姓名 业务员 
  ,fh.dcreatesystime 制单时间,fh.cCusName 客户名称,fh.cCusCode 客户编号,fh.cPersonCode 业务员编码,fh.cAccounter 
  ,fh.dDate,fhmx.cwhcode,cwhname,fhmx.cinvcode,fhmx.iquantity,iUnitPrice 不含税单价,iTaxUnitprice 含税单价,iTaxUnitprice*iquantity as 含税金额, iMoney 不含税金额 ,iNatTax 税金
   ,fhmx.iTaxRate 税率,物料名称,规格型号,fhmx.cfree1,fhmx.cfree2   from   [192.168.20.150].UFDATA_008_2018.dbo.DispatchList fh   
              left join  [192.168.20.150].UFDATA_008_2018.dbo.DispatchLists fhmx on  fh.DLID=fhmx.DLID
              left join 基础数据物料信息表 on  基础数据物料信息表.物料编码=fhmx.cinvcode
            left  join [192.168.20.150].UFDATA_008_2018.dbo.Warehouse on Warehouse.cWhCode=fhmx.cwhcode
              --left join  [192.168.20.150].UFDATA_008_2018.dbo.rdrecord32 ck on  fh.cdlcode=ck.cbuscode
              left join 人事基础员工表 yg on yg.员工号= fh.cPersonCode   where cSourceCode='{0}'", dr["cCode"]);
                DataTable t_zsale = CZMaster.MasterSQL.Get_DataTable(s, strcon);

                string s_销售单号 = "SO" + t_zsale.Rows[0]["流水号"].ToString();
                DataRow r_sale = save_saleMain.NewRow();
                save_saleMain.Rows.Add(r_sale);
                r_sale["GUID"] = System.Guid.NewGuid();
                r_sale["销售订单号"] = s_销售单号;
                r_sale["录入人员"] = t_zsale.Rows[0]["cMaker"];
                //r_sale["录入人员ID"] = CPublic.Var.LocalUserID;
                r_sale["待审核"] = true;
                r_sale["审核"] = true;

                r_sale["客户编号"] = t_zsale.Rows[0]["客户编号"];
                r_sale["客户名"] = t_zsale.Rows[0]["客户名称"]; ;
                r_sale["税率"] = t_zsale.Rows[0]["税率"] == null ? "" : t_zsale.Rows[0]["税率"];
                r_sale["业务员"] = t_zsale.Rows[0]["业务员"] == null ? "" : t_zsale.Rows[0]["业务员"].ToString();
                r_sale["日期"] = t_zsale.Rows[0]["制单时间"];
                r_sale["销售备注"] = dr["ChangeType"].ToString(); //借出转销售或者 转 赠送
                r_sale["备注1"] = dr["借出单"];

                decimal dec_TaxMoney = 0;
                decimal dec_Money = 0;
                int i = 1;
                foreach (DataRow r in t_zsale.Rows)
                {
                    if (r["含税金额"] != null && r["含税金额"] != DBNull.Value)
                    {
                        dec_TaxMoney = dec_TaxMoney + Convert.ToDecimal(r["含税金额"]);
                        dec_Money = dec_Money + Convert.ToDecimal(r["不含税金额"]);
                    }


                    DataRow r_sale_mx = save_saleDetail.NewRow();

                    r_sale_mx["GUID"] = System.Guid.NewGuid();
                    r_sale_mx["销售订单号"] = s_销售单号;
                    r_sale_mx["POS"] = i;
                    r_sale_mx["销售订单明细号"] = s_销售单号 + "-" + i.ToString("00");
                    r_sale_mx["物料编码"] = r["cinvcode"];
                    r_sale_mx["数量"] = r["iquantity"];
                    r_sale_mx["完成数量"] = r["iquantity"];
                    r_sale_mx["未完成数量"] = 0;
                    r_sale_mx["已通知数量"] = r["iquantity"];
                    r_sale_mx["未通知数量"] = 0;
                    r_sale_mx["物料名称"] = r["物料名称"];
                    r_sale_mx["税率"] = t_zsale.Rows[0]["税率"] == null ? "" : t_zsale.Rows[0]["税率"]; ;
                    r_sale_mx["规格型号"] = r["规格型号"];
                    r_sale_mx["备注3"] = r["cfree1"] == null ? "" : r["cfree1"].ToString();
                    r_sale_mx["备注4"] = r["cfree2"] == null ? "" : r["cfree1"].ToString();
                    r_sale_mx["仓库号"] = r["cwhcode"];
                    r_sale_mx["仓库名称"] = r["cwhname"];
                    // r_sale_mx["计量单位"] = dr["计量单位"];
                    // dr_saleDetail["销售备注"] = "借出转赠送";
                    r_sale_mx["税前金额"] = r["不含税金额"] == null ? 0 : r["不含税金额"];
                    r_sale_mx["税后金额"] = r["含税金额"] == null ? 0 : r["含税金额"];
                    r_sale_mx["税前单价"] = r["不含税单价"] == null ? 0 : r["不含税单价"];
                    r_sale_mx["税后单价"] = r["含税单价"] == null ? 0 : r["含税单价"];
                    r_sale_mx["送达日期"] = r["制单时间"];

                    r_sale_mx["客户编号"] = r["客户编号"];
                    r_sale_mx["客户"] = r["客户名称"];

                    r_sale_mx["生效"] = true;
                    r_sale_mx["生效日期"] = r["制单时间"];
                    r_sale_mx["明细完成"] = true;
                    r_sale_mx["明细完成日期"] = r["制单时间"];
                    r_sale_mx["总完成"] = true;
                    r_sale_mx["总完成日期"] = r["制单时间"];
                    r_sale_mx["已计算"] = true;

                    r_sale_mx["含税销售价"] = r["含税单价"] == null ? 0 : r["含税单价"];
                    save_saleDetail.Rows.Add(r_sale_mx);
                    //19-4-20 销售提供的发货记录和出记录中已经包含 只需要将记录的销售订单号 和销售订单明细号 关联 即可
                    DataRow r_saletzmx = save_saletzDetail.NewRow();
                    save_saletzDetail.Rows.Add(r_saletzmx);
                    r_saletzmx["GUID"] = System.Guid.NewGuid();
                    r_saletzmx["出库通知单号"] = r["通知单号"];
                    r_saletzmx["POS"] = i;
                    r_saletzmx["出库通知单明细号"] = r["通知单号"] + "-" + i.ToString("00");
                    r_saletzmx["销售订单明细号"] = r_sale_mx["销售订单明细号"];
                    r_saletzmx["物料编码"] = r["cinvcode"];
                    r_saletzmx["物料名称"] = r["物料名称"];
                    r_saletzmx["出库数量"] = r["iquantity"];
                    r_saletzmx["规格型号"] = r["规格型号"];
                    //r_saletzmx["操作员ID"] = r["cMaker"];
                    r_saletzmx["操作员"] = r["cMaker"];
                    r_saletzmx["生效"] = true;
                    r_saletzmx["生效日期"] = r["制单时间"];
                    r_saletzmx["完成"] = true;
                    r_saletzmx["完成日期"] = r["制单时间"];
                    r_saletzmx["销售备注"] = "借出转赠送";
                    r_saletzmx["客户"] = r["客户名称"];
                    r_saletzmx["客户编号"] = r["客户编号"];
                    r_saletzmx["已出库数量"] = r["iquantity"];
                    r_saletzmx["未出库数量"] = 0;
                    i++;
                }
                r_sale["税金"] = dec_TaxMoney - dec_Money;
                r_sale["税前金额"] = dec_Money;
                r_sale["税后金额"] = dec_TaxMoney;
                r_sale["生效"] = true;
                r_sale["生效日期"] = t_zsale.Rows[0]["制单时间"];
                r_sale["生效人员"] = t_zsale.Rows[0]["cMaker"];
                r_sale["生效人员ID"] = t_zsale.Rows[0]["cMaker"];
                r_sale["创建日期"] = t_zsale.Rows[0]["制单时间"];
                r_sale["修改日期"] = t_zsale.Rows[0]["制单时间"];
                r_sale["完成"] = true;
                r_sale["完成日期"] = t_zsale.Rows[0]["制单时间"];

                //19-4-20 销售提供的发货记录和出记录中已经包含 只需要将记录的销售订单号 和销售订单明细号 关联 即可

                DataRow r_saletzmain = save_saletzMain.NewRow();
                save_saletzMain.Rows.Add(r_saletzmain);
                r_saletzmain["GUID"] = System.Guid.NewGuid();
                r_saletzmain["出库通知单号"] = t_zsale.Rows[0]["通知单号"];

                r_saletzmain["客户编号"] = t_zsale.Rows[0]["客户编号"];
                r_saletzmain["客户名"] = t_zsale.Rows[0]["客户名称"];

                r_saletzmain["出库日期"] = t_zsale.Rows[0]["制单时间"];
                r_saletzmain["创建日期"] = t_zsale.Rows[0]["制单时间"];
                r_saletzmain["修改日期"] = t_zsale.Rows[0]["制单时间"];
                // r_saletzmain["操作员ID"] = CPublic.Var.LocalUserID;
                r_saletzmain["操作员"] = t_zsale.Rows[0]["cMaker"];
                r_saletzmain["生效"] = true;
                r_saletzmain["生效日期"] = t_zsale.Rows[0]["制单时间"];
                // 还有销售出库单  和  仓库出入库明细表 
                s = string.Format(@"select  ck.*  from  [192.168.20.150].UFDATA_008_2018.dbo.rdrecord32  ck   
     left join  [192.168.20.150].UFDATA_008_2018.dbo.DispatchList fh on  fh.cdlcode=ck.cbuscode
     where ck.cBusCode='{0}' ", t_zsale.Rows[0]["通知单号"]);
                DataTable temp_tz = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                foreach (DataRow r in temp_tz.Rows) //出库单主记录
                {
                    DataRow r_salemain = save_saleckMain.NewRow();
                    save_saleckMain.Rows.Add(r_salemain);
                    r_salemain["GUID"] = System.Guid.NewGuid();
                    r_salemain["成品出库单号"] = r["ccode"];
                    // r_salemain["操作员ID"] = CPublic.Var.LocalUserID;
                    r_salemain["操作员"] = t_zsale.Rows[0]["cMaker"];
                    r_salemain["客户"] = t_zsale.Rows[0]["客户名称"];

                    r_salemain["日期"] = temp_tz.Rows[0]["ddate"];
                    r_salemain["创建日期"] = temp_tz.Rows[0]["ddate"];
                    r_salemain["修改日期"] = temp_tz.Rows[0]["ddate"];
                    r_salemain["生效"] = true;
                    r_salemain["生效日期"] = temp_tz.Rows[0]["ddate"];
                    r_salemain["备注2"] = temp_tz.Rows[0]["cinvoicecompany"]; //开票单位 编号
                    string x = string.Format("select  * from [192.168.20.150].UFDATA_008_2018.dbo.rdrecords32  where ID='{0}' ", r["ID"]);
                    DataTable temp_ck = CZMaster.MasterSQL.Get_DataTable(x, strcon);
                    int d = 1;
                    foreach (DataRow r_ck in temp_ck.Rows)
                    {
                        DataRow dr_stockOutDetaail = save_saleckDetail.NewRow();
                        save_saleckDetail.Rows.Add(dr_stockOutDetaail);
                        dr_stockOutDetaail["GUID"] = System.Guid.NewGuid();
                        dr_stockOutDetaail["成品出库单号"] = r["ccode"];
                        dr_stockOutDetaail["POS"] = d;
                        dr_stockOutDetaail["成品出库单明细号"] = r["ccode"] + "-" + d++.ToString("00");
                        dr_stockOutDetaail["销售订单号"] = s_销售单号;
                        DataRow[] xxr = save_saletzDetail.Select(string.Format("出库通知单号='{0}'and 物料编码='{1}'", t_zsale.Rows[0]["通知单号"], r_ck["cinvcode"]));
                        dr_stockOutDetaail["物料名称"] = xxr[0]["物料名称"];
                        dr_stockOutDetaail["规格型号"] = xxr[0]["规格型号"];
                        DataRow[] yyr = save_saleDetail.Select(string.Format("销售订单号='{0}'and 物料编码='{1}'", s_销售单号, r_ck["cinvcode"]));
                        dr_stockOutDetaail["仓库号"] = yyr[0]["仓库号"];
                        dr_stockOutDetaail["仓库名称"] = yyr[0]["仓库名称"];
                        dr_stockOutDetaail["销售订单明细号"] = yyr[0]["销售订单明细号"];
                        dr_stockOutDetaail["出库通知单明细号"] = xxr[0]["出库通知单明细号"];
                        dr_stockOutDetaail["出库通知单号"] = t_zsale.Rows[0]["通知单号"];
                        dr_stockOutDetaail["物料编码"] = r_ck["cinvcode"];
                        dr_stockOutDetaail["出库数量"] = r_ck["iquantity"];
                        dr_stockOutDetaail["已出库数量"] = r_ck["iquantity"];
                        dr_stockOutDetaail["未开票数量"] = r_ck["iquantity"];
                        dr_stockOutDetaail["客户"] = t_zsale.Rows[0]["客户名称"];
                        dr_stockOutDetaail["客户编号"] = t_zsale.Rows[0]["客户编号"];
                        dr_stockOutDetaail["生效"] = true;
                        dr_stockOutDetaail["生效日期"] = t_zsale.Rows[0]["ddate"];

                        DataRow r_stockcrmx = stock_crmx.NewRow();
                        stock_crmx.Rows.Add(r_stockcrmx);
                        r_stockcrmx["GUID"] = System.Guid.NewGuid();
                        r_stockcrmx["明细类型"] = "销售出库";
                        r_stockcrmx["单号"] = dr_stockOutDetaail["成品出库单号"];
                        r_stockcrmx["物料编码"] = r_ck["cinvcode"];
                        r_stockcrmx["物料名称"] = xxr[0]["物料名称"];
                        r_stockcrmx["明细号"] = dr_stockOutDetaail["成品出库单明细号"];
                        r_stockcrmx["出库入库"] = "出库";
                        r_stockcrmx["实效数量"] = "-" + r_ck["iquantity"];
                        r_stockcrmx["实效时间"] = t_zsale.Rows[0]["ddate"];
                        r_stockcrmx["出入库时间"] = t_zsale.Rows[0]["ddate"];
                        r_stockcrmx["相关单号"] = yyr[0]["销售订单明细号"];
                        r_stockcrmx["仓库号"] = yyr[0]["仓库号"];
                        r_stockcrmx["仓库名称"] = yyr[0]["仓库名称"];
                        r_stockcrmx["相关单位"] = t_zsale.Rows[0]["客户名称"];

                    }
                }

            }
        }
        private void loadAlltable()
        {
            //   where ccode='DWJC2018060072' 
            string s = @"select  HY_DZ_BorrowOut.*,ccusName from HY_DZ_BorrowOut 
                        left join   (select  ccuscode,ccusName from  Customer   union select   cDepCode as ccuscode ,cdepname ccusName from  Department )a
                        on a.cCusCode=HY_DZ_BorrowOut.bObjectCode  ";//所有借出借用单主记录
            u8_list = CZMaster.MasterSQL.Get_DataTable(s, strcon_dw);

            //所有有转换记录得
            s = @"select   jcz.ccode as 借出单 ,zhd.* from   HY_DZ_BorrowOut jcz  
            inner join HY_DZ_BorrowOutChange zhd on zhd.UpID=jcz.ID where zhd.iStatus=2  ";    //状态已审核
            changed_list = CZMaster.MasterSQL.Get_DataTable(s, strcon_dw);

            s = "select  * from 其他出入库申请主表 where 1<>1";
            save_qtsqMain = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select * from 其他出入库申请子表 where 1<>1";
            save_qtsqDetail = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select * from 其他出库主表 where 1<>1";
            save_qtckMain = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select * from 其他出库子表 where 1<>1";
            save_qtckDetail = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select * from 其他入库主表 where 1<>1";
            save_qtrkMain = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select * from 其他入库子表 where 1<>1";
            save_qtrkDetail = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = "select * from 销售记录销售订单主表 where 1<>1";
            save_saleMain = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select * from 销售记录销售订单明细表 where 1<>1";
            save_saleDetail = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select * from 销售记录销售出库通知单主表 where 1<>1";
            save_saletzMain = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select * from 销售记录销售出库通知单明细表 where 1<>1";
            save_saletzDetail = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select * from 销售记录成品出库单主表 where 1<>1";
            save_saleckMain = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select * from 销售记录成品出库单明细表 where 1<>1";
            save_saleckDetail = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = "select * from 借还申请表 where 1<>1";
            save_jcMain = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select * from 借还申请表附表 where 1<>1";
            save_jcDetail = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select  * from 借还申请表归还记录 where 1<>1";
            save_jcgh = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select  * from 仓库出入库明细表  where 1=2";
            stock_crmx = CZMaster.MasterSQL.Get_DataTable(s, strcon);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("save");
            try
            {
                string s = "select  * from 其他出入库申请主表 where 1<>1";
                SqlCommand cmm = new SqlCommand(s, conn, ts);
                SqlDataAdapter da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(save_qtsqMain);

                s = "select  * from 其他出入库申请子表 where 1<>1";
                cmm = new SqlCommand(s, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(save_qtsqDetail);

                s = "select  * from 其他出库主表 where 1<>1";
                cmm = new SqlCommand(s, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(save_qtckMain);

                s = "select  * from 其他出库子表 where 1<>1";
                cmm = new SqlCommand(s, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(save_qtckDetail);

                s = "select  * from 销售记录销售订单主表 where 1<>1";
                cmm = new SqlCommand(s, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(save_saleMain);

                s = "select  * from 销售记录销售订单明细表 where 1<>1";
                cmm = new SqlCommand(s, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(save_saleDetail);

                s = "select  * from 销售记录销售出库通知单主表 where 1<>1";
                cmm = new SqlCommand(s, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(save_saletzMain);

                s = "select  * from 销售记录销售出库通知单明细表 where 1<>1";
                cmm = new SqlCommand(s, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(save_saletzDetail);

                s = "select  * from 销售记录成品出库单主表 where 1<>1";
                cmm = new SqlCommand(s, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(save_saleckMain);

                s = "select  * from 销售记录成品出库单明细表 where 1<>1";
                cmm = new SqlCommand(s, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(save_saleckDetail);

                s = "select  * from 借还申请表 where 1<>1";
                cmm = new SqlCommand(s, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(save_jcMain);

                s = "select  * from 借还申请表附表 where 1<>1";
                cmm = new SqlCommand(s, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(save_jcDetail);
                s = "select  * from 借还申请表归还记录 where 1<>1";
                cmm = new SqlCommand(s, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(save_jcgh);

                s = "select  * from 仓库出入库明细表 where 1<>1";
                cmm = new SqlCommand(s, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(stock_crmx);

                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                MessageBox.Show(ex.Message);

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {


            string sql = "Select 发货单号  from[4-21U8发货单列表] where 表体订单号 = ''  and 数量<0  and 状态='审核' group by       发货单号";
            DataTable dt_groupby = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            //取所有 只有到货单号的记录  这个是 期初的   需要生成 销售记录和 出库记录 再去匹配所有的开票记录

            string sql1 = "select  * from [销售记录销售订单主表]  where 1=2";
            DataTable salemain = CZMaster.MasterSQL.Get_DataTable(sql1, strcon);
            string sql2 = "select  * from [销售记录销售订单明细表]  where 1=2";
            DataTable saledetail = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);
            string sql3 = "select  * from [销售记录销售出库通知单主表]  where 1=2";
            DataTable tzmain = CZMaster.MasterSQL.Get_DataTable(sql3, strcon);
            string sql4 = "select  * from [销售记录销售出库通知单明细表]  where 1=2";
            DataTable tzdetail = CZMaster.MasterSQL.Get_DataTable(sql4, strcon);
            string sql5 = "select  * from [销售记录成品出库单主表]  where 1=2";
            DataTable ckmain = CZMaster.MasterSQL.Get_DataTable(sql5, strcon);
            string sql6 = "select  * from [销售记录成品出库单明细表]  where 1=2";
            DataTable ckdetail = CZMaster.MasterSQL.Get_DataTable(sql6, strcon);
            string sql7 = "select  * from [销售记录销售开票主表]  where 1=2";
            DataTable kpmain = CZMaster.MasterSQL.Get_DataTable(sql7, strcon);
            string sql8 = "select  * from [销售记录销售开票明细表]  where 1=2";
            DataTable kpdetail = CZMaster.MasterSQL.Get_DataTable(sql8, strcon);

            sql = @" Select  * from [4-21U8发货单列表] where 表体订单号=''  and  数量<0 and 状态='审核' ";
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            foreach (DataRow dr in dt_groupby.Rows)
            {
                if (dr["表头订单号"].ToString() == "")  //个别没有销售订单号
                {


                }
                else
                {
                    string strSoNo = string.Format("SO{0}{1}{2}{3}", "2017", "01", "01",
                    CPublic.CNo.fun_得到最大流水号("SO", 2017, 01, 01).ToString("0000"));
                    DataRow r_sale_main = salemain.NewRow();
                    DataRow[] select_r = dt.Select(string.Format("发货单号='{0}'", dr["发货单号"]));
                    r_sale_main["GUID"] = System.Guid.NewGuid();
                    r_sale_main["销售订单号"] = strSoNo;
                    r_sale_main["录入人员"] = select_r[0]["制单人"];
                    r_sale_main["审核人员"] = select_r[0]["审核人"];
                    r_sale_main["审核日期"] = select_r[0]["审核日期"] == null ? DBNull.Value : select_r[0]["审核日期"];
                    r_sale_main["待审核"] = true;
                    r_sale_main["审核"] = true;
                    r_sale_main["客户编号"] = select_r[0]["客户编码"];
                    r_sale_main["客户名"] = select_r[0]["客户名称"];
                    r_sale_main["税率"] = select_r[0]["税率（%）"] == null ? "" : select_r[0]["税率（%）"];
                    r_sale_main["日期"] = select_r[0]["制单时间"];
                    r_sale_main["备注1"] = select_r[0]["备注"] == null ? "" : select_r[0]["备注"];

                    salemain.Rows.Add(r_sale_main);

                    DataRow r_saletzmain = tzmain.NewRow();
                    tzmain.Rows.Add(r_saletzmain);
                    r_saletzmain["GUID"] = System.Guid.NewGuid();
                    r_saletzmain["出库通知单号"] = dr["发货单号"];
                    r_saletzmain["客户编号"] = select_r[0]["客户编码"];
                    r_saletzmain["客户名"] = select_r[0]["客户名称"];
                    r_saletzmain["出库日期"] = select_r[0]["审核日期"];
                    r_saletzmain["创建日期"] = select_r[0]["制单时间"];
                    r_saletzmain["修改日期"] = select_r[0]["制单时间"];
                    // r_saletzmain["操作员ID"] = CPublic.Var.LocalUserID;
                    r_saletzmain["操作员"] = select_r[0]["制单人"];
                    r_saletzmain["生效"] = true;
                    r_saletzmain["生效日期"] = select_r[0]["制单时间"];


                    string ckno = string.Format("SA{0}{1}{2}{3}", "2017", "01", "01",
                     CPublic.CNo.fun_得到最大流水号("SA", 2017, 01, 01).ToString("0000"));
                    DataRow r_ckmain = ckmain.NewRow();
                    ckmain.Rows.Add(r_ckmain);
                    r_ckmain["GUID"] = System.Guid.NewGuid();
                    r_ckmain["成品出库单号"] = ckno;
                    // r_salemain["操作员ID"] = CPublic.Var.LocalUserID;
                    r_ckmain["操作员"] = select_r[0]["制单人"];
                    r_ckmain["客户"] = select_r[0]["客户名称"];

                    r_ckmain["日期"] = select_r[0]["制单时间"];
                    r_ckmain["创建日期"] = select_r[0]["制单时间"];
                    r_ckmain["修改日期"] = select_r[0]["制单时间"];
                    r_ckmain["生效"] = true;
                    r_ckmain["生效日期"] = select_r[0]["审核日期"];
                    int i = 1;
                    foreach (DataRow rr in select_r)
                    {
                        DataRow r_sale_mx = saledetail.NewRow();
                        r_sale_mx["GUID"] = System.Guid.NewGuid();
                        r_sale_mx["销售订单号"] = strSoNo;
                        r_sale_mx["POS"] = i;
                        r_sale_mx["销售订单明细号"] = strSoNo + "-" + i.ToString("00");
                        r_sale_mx["物料编码"] = rr["存货编码"];
                        r_sale_mx["数量"] = rr["数量"];
                        r_sale_mx["完成数量"] = rr["数量"];
                        r_sale_mx["未完成数量"] = 0;
                        r_sale_mx["已通知数量"] = rr["数量"];
                        r_sale_mx["未通知数量"] = 0;
                        r_sale_mx["物料名称"] = rr["存货名称"];
                        r_sale_mx["税率"] = rr["税率（%）"] == null ? "" : rr["税率（%）"];
                        r_sale_mx["规格型号"] = rr["规格型号"] == null ? "" : rr["规格型号"];

                        r_sale_mx["仓库号"] = rr["仓库编号"];
                        r_sale_mx["仓库名称"] = rr["仓库"];
                        // r_sale_mx["计量单位"] = dr["计量单位"];
                        // dr_saleDetail["销售备注"] = "借出转赠送";

                        r_sale_mx["送达日期"] = rr["发货日期"];

                        r_sale_mx["客户编号"] = rr["客户编码"];
                        r_sale_mx["客户"] = rr["客户名称"];

                        r_sale_mx["生效"] = true;
                        r_sale_mx["生效日期"] = rr["审核日期"];
                        r_sale_mx["明细完成"] = true;
                        r_sale_mx["明细完成日期"] = rr["审核日期"];
                        saledetail.Rows.Add(r_sale_mx);

                        //19-4-20 销售提供的发货记录和出记录中已经包含 只需要将记录的销售订单号 和销售订单明细号 关联 即可
                        DataRow r_saletzmx = tzdetail.NewRow();
                        tzdetail.Rows.Add(r_saletzmx);
                        r_saletzmx["GUID"] = System.Guid.NewGuid();
                        r_saletzmx["出库通知单号"] = dr["发货单号"];
                        r_saletzmx["POS"] = i;
                        r_saletzmx["出库通知单明细号"] = dr["发货单号"] + "-" + rr["行号"];
                        r_saletzmx["销售订单明细号"] = r_sale_mx["销售订单明细号"];
                        r_saletzmx["物料编码"] = rr["存货编码"];
                        r_saletzmx["物料名称"] = rr["存货名称"];
                        r_saletzmx["出库数量"] = rr["数量"];
                        r_saletzmx["规格型号"] = rr["规格型号"] == null ? "" : rr["规格型号"];
                        //r_saletzmx["操作员ID"] = r["cMaker"];
                        r_saletzmx["操作员"] = rr["制单人"];
                        r_saletzmx["生效"] = true;
                        r_saletzmx["生效日期"] = rr["审核日期"];
                        r_saletzmx["完成"] = true;
                        r_saletzmx["完成日期"] = rr["制单时间"];
                        r_saletzmx["销售备注"] = rr["表体备注"];
                        r_saletzmx["客户"] = rr["客户名称"];
                        r_saletzmx["客户编号"] = rr["客户编码"];
                        r_saletzmx["已出库数量"] = rr["数量"];
                        r_saletzmx["未出库数量"] = 0;

                        DataRow ckmx = ckdetail.NewRow();
                        ckdetail.Rows.Add(ckmx);
                        ckmx["GUID"] = System.Guid.NewGuid();
                        ckmx["成品出库单号"] = ckno;
                        ckmx["POS"] = i;
                        ckmx["成品出库单明细号"] = ckno + "-" + i.ToString("00");
                        ckmx["销售订单明细号"] = r_sale_mx["销售订单明细号"];
                        ckmx["销售订单号"] = strSoNo;
                        ckmx["出库通知单号"] = dr["发货单号"];
                        ckmx["出库通知单明细号"] = r_saletzmx["出库通知单明细号"];
                        ckmx["物料编码"] = rr["存货编码"];
                        ckmx["物料名称"] = rr["存货名称"];
                        ckmx["出库数量"] = rr["数量"];
                        ckmx["已出库数量"] = rr["数量"];
                        ckmx["已开票数量"] = rr["累计开票数量"] == null ? 0 : rr["累计开票数量"];
                        ckmx["未开票数量"] = rr["未开票数量"];
                        ckmx["规格型号"] = rr["规格型号"] == null ? "" : rr["规格型号"];
                        //r_saletzmx["操作员ID"] = r["cMaker"];
                        ckmx["仓库号"] = rr["仓库编号"];
                        ckmx["仓库名称"] = rr["仓库"];
                        ckmx["客户"] = rr["客户名称"];
                        ckmx["客户编号"] = rr["客户编码"];
                        //ckmx["操作员"] = rr["制单人"];
                        ckmx["生效"] = true;
                        ckmx["生效日期"] = rr["审核日期"];
                        ckmx["完成"] = true;
                        ckmx["完成日期"] = rr["制单时间"];
                        ckmx["销售备注"] = rr["表体备注"];
                        ckmx["退货标识"] = rr["退货标识"];
                        i++;



                    }


                }






            }

            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("save");
            try
            {

                SqlCommand cmm = new SqlCommand(sql1, conn, ts);
                SqlDataAdapter da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(salemain);

                cmm = new SqlCommand(sql2, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(saledetail);


                cmm = new SqlCommand(sql3, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(tzmain);


                cmm = new SqlCommand(sql4, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(tzdetail);


                cmm = new SqlCommand(sql5, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(ckmain);


                cmm = new SqlCommand(sql6, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(ckdetail);

                ts.Commit();
            }


            catch (Exception ex)
            {
                ts.Rollback();
                throw new Exception(ex.Message);
            }
        }

        //4-25
        private void button6_Click(object sender, EventArgs e)
        {
            //期初的搞定了

            string sql1 = "select  * from [销售记录销售订单主表]  where 1=2";
            DataTable salemain = CZMaster.MasterSQL.Get_DataTable(sql1, strcon);
            string sql2 = "select  * from [销售记录销售订单明细表]  where 1=2";
            DataTable saledetail = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);
            string sql3 = "select  * from [销售记录销售出库通知单主表]  where 1=2";
            DataTable tzmain = CZMaster.MasterSQL.Get_DataTable(sql3, strcon);
            string sql4 = "select  * from [销售记录销售出库通知单明细表]  where 1=2";
            DataTable tzdetail = CZMaster.MasterSQL.Get_DataTable(sql4, strcon);
            string sql5 = "select  * from [销售记录成品出库单主表]  where 1=2";
            DataTable ckmain = CZMaster.MasterSQL.Get_DataTable(sql5, strcon);
            string sql6 = "select  * from [销售记录成品出库单明细表]  where 1=2";
            DataTable ckdetail = CZMaster.MasterSQL.Get_DataTable(sql6, strcon);

            //取出 期初到货单
            string s = @"    select * from[4-21U8发货单列表] where 表体订单号='' and 表头订单号='' and 数量> 0 and 发货单号 not in(               
                select 发货单号 from[4-21U8发货单列表]  where 发货单号    in  
                (select 发货单号 from[4-21销售出库单列表]  where 备注 like '%借出借用单%'))";
            //生成销售记录和销售出库记录
            DataTable t_到货 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            t_到货 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            string ss = @"select 发货单号 from[4-21U8发货单列表] where 表体订单号='' and 表头订单号=''  and 数量> 0 and 发货单号 not in(               
                select 发货单号 from[4-21U8发货单列表]  where 发货单号    in  
                (select 发货单号 from[4-21销售出库单列表]  where 备注 like '%借出借用单%')) group by 发货单号";
            DataTable t_汇总 = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
            foreach (DataRow dr in t_汇总.Rows)
            {
                string strSoNo = string.Format("SO{0}{1}{2}{3}", "2017", "01", "01",
                CPublic.CNo.fun_得到最大流水号("SO", 2017, 01, 01).ToString("0000"));
                DataRow r_sale_main = salemain.NewRow();
                DataRow[] select_r = t_到货.Select(string.Format("发货单号='{0}'", dr["发货单号"]));
                r_sale_main["GUID"] = System.Guid.NewGuid();
                r_sale_main["销售订单号"] = strSoNo;
                r_sale_main["录入人员"] = select_r[0]["制单人"];
                r_sale_main["审核人员"] = select_r[0]["审核人"];
                r_sale_main["审核日期"] = select_r[0]["审核日期"] == null ? DBNull.Value : select_r[0]["审核日期"];
                r_sale_main["待审核"] = true;
                r_sale_main["审核"] = true;
                r_sale_main["客户编号"] = select_r[0]["客户编码"];
                r_sale_main["客户名"] = select_r[0]["客户名称"];
                r_sale_main["税率"] = select_r[0]["税率（%）"] == null ? "" : select_r[0]["税率（%）"];
                r_sale_main["日期"] = select_r[0]["制单时间"];
                r_sale_main["备注1"] = select_r[0]["备注"] == null ? "" : select_r[0]["备注"];
                r_sale_main["销售备注"] = "002期初剩余导入";
                salemain.Rows.Add(r_sale_main);

                DataRow r_saletzmain = tzmain.NewRow();
                tzmain.Rows.Add(r_saletzmain);
                r_saletzmain["GUID"] = System.Guid.NewGuid();
                r_saletzmain["出库通知单号"] = dr["发货单号"];
                r_saletzmain["客户编号"] = select_r[0]["客户编码"];
                r_saletzmain["客户名"] = select_r[0]["客户名称"];
                r_saletzmain["出库日期"] = select_r[0]["审核日期"];
                r_saletzmain["创建日期"] = select_r[0]["制单时间"];
                r_saletzmain["修改日期"] = select_r[0]["制单时间"];
                // r_saletzmain["操作员ID"] = CPublic.Var.LocalUserID;
                r_saletzmain["操作员"] = select_r[0]["制单人"];
                r_saletzmain["生效"] = true;
                r_saletzmain["生效日期"] = select_r[0]["制单时间"];
                r_saletzmain["备注"] = "002期初剩余导入";
                string ckno = string.Format("SA{0}{1}{2}{3}", "2017", "01", "01",
                 CPublic.CNo.fun_得到最大流水号("SA", 2017, 01, 01).ToString("0000"));
                DataRow r_ckmain = ckmain.NewRow();
                ckmain.Rows.Add(r_ckmain);
                r_ckmain["GUID"] = System.Guid.NewGuid();
                r_ckmain["成品出库单号"] = ckno;
                // r_salemain["操作员ID"] = CPublic.Var.LocalUserID;
                r_ckmain["操作员"] = select_r[0]["制单人"];
                r_ckmain["客户"] = select_r[0]["客户名称"];
                r_ckmain["日期"] = select_r[0]["制单时间"];
                r_ckmain["创建日期"] = select_r[0]["制单时间"];
                r_ckmain["修改日期"] = select_r[0]["制单时间"];
                r_ckmain["生效"] = true;
                r_ckmain["生效日期"] = select_r[0]["审核日期"];
                r_ckmain["出库备注"] = "002期初剩余导入";

                int i = 1;
                foreach (DataRow rr in select_r)
                {
                    DataRow r_sale_mx = saledetail.NewRow();
                    // r_sale_mx["GUID"] = System.Guid.NewGuid();
                    r_sale_mx["销售订单号"] = strSoNo;
                    r_sale_mx["POS"] = i;
                    r_sale_mx["销售订单明细号"] = strSoNo + "-" + i.ToString("00");
                    r_sale_mx["物料编码"] = rr["存货编码"];
                    r_sale_mx["数量"] = rr["数量"];
                    r_sale_mx["完成数量"] = rr["累计出库数量"];
                    r_sale_mx["未完成数量"] = Convert.ToInt32(r_sale_mx["数量"]) - Convert.ToInt32(rr["累计出库数量"]);
                    r_sale_mx["已通知数量"] = rr["数量"];

                    r_sale_mx["未通知数量"] = 0;
                    r_sale_mx["物料名称"] = rr["存货名称"];
                    r_sale_mx["税率"] = rr["税率（%）"] == null ? "" : rr["税率（%）"];
                    r_sale_mx["规格型号"] = rr["规格型号"] == null ? "" : rr["规格型号"];
                    r_sale_mx["仓库号"] = rr["仓库编号"];
                    r_sale_mx["仓库名称"] = rr["仓库"];
                    // r_sale_mx["计量单位"] = dr["计量单位"];
                    // dr_saleDetail["销售备注"] = "借出转赠送";

                    r_sale_mx["送达日期"] = rr["发货日期"];

                    r_sale_mx["客户编号"] = rr["客户编码"];
                    r_sale_mx["客户"] = rr["客户名称"];

                    r_sale_mx["生效"] = true;
                    r_sale_mx["生效日期"] = rr["审核日期"];
                    if (Convert.ToDecimal(r_sale_mx["未完成数量"]) == 0 || Convert.ToDecimal(rr["未开票数量"]) == 0)
                    {
                        r_sale_mx["明细完成"] = true;

                        r_sale_mx["明细完成日期"] = rr["审核日期"];
                    }

                    saledetail.Rows.Add(r_sale_mx);

                    //19-4-20 销售提供的发货记录和出记录中已经包含 只需要将记录的销售订单号 和销售订单明细号 关联 即可
                    DataRow r_saletzmx = tzdetail.NewRow();
                    tzdetail.Rows.Add(r_saletzmx);
                    r_saletzmx["GUID"] = System.Guid.NewGuid();
                    r_saletzmx["出库通知单号"] = rr["发货单号"];
                    r_saletzmx["POS"] = rr["行号"];
                    r_saletzmx["出库通知单明细号"] = rr["发货单号"] + "-" + rr["行号"];
                    r_saletzmx["销售订单明细号"] = r_sale_mx["销售订单明细号"];
                    r_saletzmx["物料编码"] = rr["存货编码"];
                    r_saletzmx["物料名称"] = rr["存货名称"];
                    r_saletzmx["出库数量"] = rr["数量"];
                    r_saletzmx["规格型号"] = rr["规格型号"] == null ? "" : rr["规格型号"];
                    //r_saletzmx["操作员ID"] = r["cMaker"];
                    r_saletzmx["操作员"] = rr["制单人"];
                    r_saletzmx["生效"] = true;
                    r_saletzmx["生效日期"] = rr["审核日期"];
                    r_saletzmx["完成"] = true;
                    r_saletzmx["完成日期"] = rr["制单时间"];
                    r_saletzmx["销售备注"] = rr["表体备注"];
                    r_saletzmx["客户"] = rr["客户名称"];
                    r_saletzmx["客户编号"] = rr["客户编码"];
                    r_saletzmx["已出库数量"] = rr["累计出库数量"];
                    r_saletzmx["未出库数量"] = Convert.ToInt32(rr["数量"]) - Convert.ToInt32(rr["累计出库数量"]);
                    r_saletzmx["累计开票数量"] = rr["累计开票数量"] == null ? 0 : rr["累计开票数量"];
                    DataRow ckmx = ckdetail.NewRow();
                    ckdetail.Rows.Add(ckmx);
                    ckmx["GUID"] = System.Guid.NewGuid();
                    ckmx["成品出库单号"] = ckno;
                    ckmx["POS"] = i;
                    ckmx["成品出库单明细号"] = ckno + "-" + i.ToString("00");
                    ckmx["销售订单明细号"] = r_sale_mx["销售订单明细号"];
                    ckmx["销售订单号"] = strSoNo;
                    ckmx["出库通知单号"] = dr["发货单号"];
                    ckmx["出库通知单明细号"] = r_saletzmx["出库通知单明细号"];
                    ckmx["物料编码"] = rr["存货编码"];
                    ckmx["物料名称"] = rr["存货名称"];
                    ckmx["出库数量"] = rr["数量"];
                    ckmx["已出库数量"] = rr["数量"];
                    ckmx["已开票数量"] = rr["累计开票数量"] == null ? 0 : rr["累计开票数量"];
                    ckmx["未开票数量"] = rr["未开票数量"];
                    ckmx["规格型号"] = rr["规格型号"] == null ? "" : rr["规格型号"];
                    //r_saletzmx["操作员ID"] = r["cMaker"];
                    ckmx["仓库号"] = rr["仓库编号"];
                    ckmx["仓库名称"] = rr["仓库"];
                    ckmx["客户"] = rr["客户名称"];
                    ckmx["客户编号"] = rr["客户编码"];
                    //ckmx["操作员"] = rr["制单人"];
                    ckmx["生效"] = true;
                    ckmx["生效日期"] = rr["审核日期"];
                    ckmx["完成"] = true;
                    ckmx["完成日期"] = rr["制单时间"];
                    ckmx["销售备注"] = rr["表体备注"];
                    ckmx["退货标识"] = rr["退货标识"];
                    i++;
                }
            }

            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("save");
            try
            {

                SqlCommand cmm = new SqlCommand(sql1, conn, ts);
                SqlDataAdapter da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(salemain);

                cmm = new SqlCommand(sql2, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(saledetail);


                cmm = new SqlCommand(sql3, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(tzmain);


                cmm = new SqlCommand(sql4, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(tzdetail);


                cmm = new SqlCommand(sql5, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(ckmain);


                cmm = new SqlCommand(sql6, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(ckdetail);

                ts.Commit();
            }


            catch (Exception ex)
            {
                ts.Rollback();
                throw new Exception(ex.Message);
            }

        }
        //无销售订单退货
        private void button7_Click(object sender, EventArgs e)
        {
            string sql1 = "select  * from [销售记录销售订单主表]  where 1=2";
            DataTable salemain = CZMaster.MasterSQL.Get_DataTable(sql1, strcon);
            string sql2 = "select  * from [销售记录销售订单明细表]  where 1=2";
            DataTable saledetail = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);
            string sql3 = "select  * from [销售记录销售出库通知单主表]  where 1=2";
            DataTable tzmain = CZMaster.MasterSQL.Get_DataTable(sql3, strcon);
            string sql4 = "select  * from [销售记录销售出库通知单明细表]  where 1=2";
            DataTable tzdetail = CZMaster.MasterSQL.Get_DataTable(sql4, strcon);
            string sql5 = "select  * from [销售记录成品出库单主表]  where 1=2";
            DataTable ckmain = CZMaster.MasterSQL.Get_DataTable(sql5, strcon);
            string sql6 = "select  * from [销售记录成品出库单明细表]  where 1=2";
            DataTable ckdetail = CZMaster.MasterSQL.Get_DataTable(sql6, strcon);


            string s = @"select * from[4-21U8发货单列表] where 表体订单号='' and 表头订单号='' and 数量< 0 and 发货单号 not in(               
                select 发货单号 from [4-21U8发货单列表]  where 发货单号 in (select 发货单号 from[4-21销售出库单列表]  where 备注 like '%借出借用单%'))";

            DataTable t_到货 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            t_到货 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            string ss = @"select 发货单号 from[4-21U8发货单列表] where 表体订单号='' and 表头订单号='' and 数量< 0 and 发货单号 not in(               
                select 发货单号 from [4-21U8发货单列表]  where 发货单号 in (select 发货单号 from[4-21销售出库单列表]  where 备注 like '%借出借用单%')) group by 发货单号";
            DataTable t_汇总 = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
            foreach (DataRow dr in t_汇总.Rows)
            {
                string strSoNo = string.Format("SO{0}{1}{2}{3}", "2017", "01", "01",
                CPublic.CNo.fun_得到最大流水号("SO", 2017, 01, 01).ToString("0000"));
                DataRow r_sale_main = salemain.NewRow();
                DataRow[] select_r = t_到货.Select(string.Format("发货单号='{0}'", dr["发货单号"]));
                r_sale_main["GUID"] = System.Guid.NewGuid();
                r_sale_main["销售订单号"] = strSoNo;
                r_sale_main["录入人员"] = select_r[0]["制单人"];
                r_sale_main["审核人员"] = select_r[0]["审核人"];
                if (select_r[0]["审核日期"] != null && select_r[0]["审核日期"].ToString().Trim() != "")
                    r_sale_main["审核日期"] = select_r[0]["审核日期"];
                r_sale_main["待审核"] = true;
                r_sale_main["审核"] = true;
                r_sale_main["客户编号"] = select_r[0]["客户编码"];
                r_sale_main["客户名"] = select_r[0]["客户名称"];
                r_sale_main["税率"] = select_r[0]["税率（%）"] == null ? "" : select_r[0]["税率（%）"];
                r_sale_main["日期"] = select_r[0]["制单时间"];
                r_sale_main["备注1"] = select_r[0]["备注"] == null ? "" : select_r[0]["备注"];
                r_sale_main["销售备注"] = "导入无销售订单退货";
                salemain.Rows.Add(r_sale_main);

                DataRow r_saletzmain = tzmain.NewRow();
                tzmain.Rows.Add(r_saletzmain);
                r_saletzmain["GUID"] = System.Guid.NewGuid();
                r_saletzmain["出库通知单号"] = dr["发货单号"];
                r_saletzmain["客户编号"] = select_r[0]["客户编码"];
                r_saletzmain["客户名"] = select_r[0]["客户名称"];
                if (select_r[0]["审核日期"] != null && select_r[0]["审核日期"].ToString().Trim() != "")
                    r_saletzmain["出库日期"] = select_r[0]["审核日期"];
                r_saletzmain["创建日期"] = select_r[0]["制单时间"];
                r_saletzmain["修改日期"] = select_r[0]["制单时间"];
                // r_saletzmain["操作员ID"] = CPublic.Var.LocalUserID;
                r_saletzmain["操作员"] = select_r[0]["制单人"];
                r_saletzmain["生效"] = true;
                r_saletzmain["生效日期"] = select_r[0]["制单时间"];
                r_saletzmain["备注"] = "导入无销售订单退货";
                string ckno = string.Format("SA{0}{1}{2}{3}", "2017", "01", "01",
                 CPublic.CNo.fun_得到最大流水号("SA", 2017, 01, 01).ToString("0000"));
                DataRow r_ckmain = ckmain.NewRow();
                ckmain.Rows.Add(r_ckmain);
                r_ckmain["GUID"] = System.Guid.NewGuid();
                r_ckmain["成品出库单号"] = ckno;
                // r_salemain["操作员ID"] = CPublic.Var.LocalUserID;
                r_ckmain["操作员"] = select_r[0]["制单人"];
                r_ckmain["客户"] = select_r[0]["客户名称"];
                r_ckmain["日期"] = select_r[0]["制单时间"];
                r_ckmain["创建日期"] = select_r[0]["制单时间"];
                r_ckmain["修改日期"] = select_r[0]["制单时间"];
                r_ckmain["生效"] = true;
                if (select_r[0]["审核日期"] != null && select_r[0]["审核日期"].ToString().Trim() != "")
                    r_ckmain["生效日期"] = select_r[0]["审核日期"];
                r_ckmain["出库备注"] = "导入无销售订单退货";

                int i = 1;
                foreach (DataRow rr in select_r)
                {
                    DataRow r_sale_mx = saledetail.NewRow();
                    r_sale_mx["GUID"] = System.Guid.NewGuid();
                    r_sale_mx["销售订单号"] = strSoNo;
                    r_sale_mx["POS"] = i;
                    r_sale_mx["销售订单明细号"] = strSoNo + "-" + i.ToString("00");
                    r_sale_mx["物料编码"] = rr["存货编码"];
                    r_sale_mx["数量"] = rr["数量"];
                    r_sale_mx["完成数量"] = rr["累计出库数量"];
                    decimal dec = 0; decimal dec1 = 0;
                    try
                    {
                        dec = Convert.ToInt32(r_sale_mx["数量"]);
                    }
                    catch
                    {


                    }
                    try
                    {
                        dec1 = Convert.ToInt32(rr["累计出库数量"]);
                    }
                    catch
                    {


                    }
                    r_sale_mx["未完成数量"] = dec - dec1;
                    r_sale_mx["已通知数量"] = rr["数量"];

                    r_sale_mx["未通知数量"] = 0;
                    r_sale_mx["物料名称"] = rr["存货名称"];
                    r_sale_mx["税率"] = rr["税率（%）"] == null ? "" : rr["税率（%）"];
                    r_sale_mx["规格型号"] = rr["规格型号"] == null ? "" : rr["规格型号"];
                    r_sale_mx["仓库号"] = rr["仓库编号"];
                    r_sale_mx["仓库名称"] = rr["仓库"];
                    // r_sale_mx["计量单位"] = dr["计量单位"];
                    // dr_saleDetail["销售备注"] = "借出转赠送";

                    r_sale_mx["送达日期"] = rr["发货日期"];

                    r_sale_mx["客户编号"] = rr["客户编码"];
                    r_sale_mx["客户"] = rr["客户名称"];

                    r_sale_mx["生效"] = true;
                    if (select_r[0]["审核日期"] != null && select_r[0]["审核日期"].ToString().Trim() != "")
                        r_sale_mx["生效日期"] = rr["审核日期"];
                    if (Convert.ToDecimal(r_sale_mx["未完成数量"]) == 0 || Convert.ToDecimal(rr["未开票数量"]) == 0)
                    {
                        r_sale_mx["明细完成"] = true;

                        r_sale_mx["明细完成日期"] = rr["审核日期"];
                    }

                    saledetail.Rows.Add(r_sale_mx);

                    //19-4-20 销售提供的发货记录和出记录中已经包含 只需要将记录的销售订单号 和销售订单明细号 关联 即可
                    DataRow r_saletzmx = tzdetail.NewRow();
                    tzdetail.Rows.Add(r_saletzmx);
                    r_saletzmx["GUID"] = System.Guid.NewGuid();
                    r_saletzmx["出库通知单号"] = dr["发货单号"];
                    r_saletzmx["POS"] = i;
                    r_saletzmx["出库通知单明细号"] = dr["发货单号"] + "-" + rr["行号"];
                    r_saletzmx["销售订单明细号"] = r_sale_mx["销售订单明细号"];
                    r_saletzmx["物料编码"] = rr["存货编码"];
                    r_saletzmx["物料名称"] = rr["存货名称"];
                    r_saletzmx["出库数量"] = rr["数量"];
                    r_saletzmx["规格型号"] = rr["规格型号"] == null ? "" : rr["规格型号"];
                    //r_saletzmx["操作员ID"] = r["cMaker"];
                    r_saletzmx["操作员"] = rr["制单人"];
                    r_saletzmx["生效"] = true;
                    if (select_r[0]["审核日期"] != null && select_r[0]["审核日期"].ToString().Trim() != "")
                        r_saletzmx["生效日期"] = rr["审核日期"];
                    r_saletzmx["完成"] = true;
                    r_saletzmx["完成日期"] = rr["制单时间"];
                    r_saletzmx["销售备注"] = rr["表体备注"];
                    r_saletzmx["客户"] = rr["客户名称"];
                    r_saletzmx["客户编号"] = rr["客户编码"];
                    r_saletzmx["已出库数量"] = rr["累计出库数量"];
                    dec = 0; dec1 = 0;
                    try
                    {
                        dec = Convert.ToInt32(r_sale_mx["数量"]);
                    }
                    catch
                    {


                    }
                    try
                    {
                        dec1 = Convert.ToInt32(rr["累计出库数量"]);
                    }
                    catch
                    {


                    }
                    r_saletzmx["未出库数量"] = dec - dec1;
                    r_saletzmx["累计开票数量"] = rr["累计开票数量"] == null ? 0 : rr["累计开票数量"];
                    DataRow ckmx = ckdetail.NewRow();
                    ckdetail.Rows.Add(ckmx);
                    ckmx["GUID"] = System.Guid.NewGuid();
                    ckmx["成品出库单号"] = ckno;
                    ckmx["POS"] = i;
                    ckmx["成品出库单明细号"] = ckno + "-" + i.ToString("00");
                    ckmx["销售订单明细号"] = r_sale_mx["销售订单明细号"];
                    ckmx["销售订单号"] = strSoNo;
                    ckmx["出库通知单号"] = dr["发货单号"];
                    ckmx["出库通知单明细号"] = r_saletzmx["出库通知单明细号"];
                    ckmx["物料编码"] = rr["存货编码"];
                    ckmx["物料名称"] = rr["存货名称"];
                    ckmx["出库数量"] = rr["数量"];
                    ckmx["已出库数量"] = rr["数量"];
                    ckmx["已开票数量"] = rr["累计开票数量"] == null ? 0 : rr["累计开票数量"];
                    ckmx["未开票数量"] = rr["未开票数量"];
                    ckmx["规格型号"] = rr["规格型号"] == null ? "" : rr["规格型号"];
                    //r_saletzmx["操作员ID"] = r["cMaker"];
                    ckmx["仓库号"] = rr["仓库编号"];
                    ckmx["仓库名称"] = rr["仓库"];
                    ckmx["客户"] = rr["客户名称"];
                    ckmx["客户编号"] = rr["客户编码"];
                    //ckmx["操作员"] = rr["制单人"];
                    ckmx["生效"] = true;
                    if (select_r[0]["审核日期"] != null && select_r[0]["审核日期"].ToString().Trim() != "")
                        ckmx["生效日期"] = rr["审核日期"];
                    ckmx["完成"] = true;
                    ckmx["完成日期"] = rr["制单时间"];
                    ckmx["销售备注"] = rr["表体备注"];
                    ckmx["退货标识"] = rr["退货标识"];
                    i++;
                }
            }

            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("save");
            try
            {

                SqlCommand cmm = new SqlCommand(sql1, conn, ts);
                SqlDataAdapter da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(salemain);

                cmm = new SqlCommand(sql2, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(saledetail);


                cmm = new SqlCommand(sql3, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(tzmain);


                cmm = new SqlCommand(sql4, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(tzdetail);


                cmm = new SqlCommand(sql5, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(ckmain);


                cmm = new SqlCommand(sql6, conn, ts);
                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(ckdetail);

                ts.Commit();
            }


            catch (Exception ex)
            {
                ts.Rollback();
                throw new Exception(ex.Message);
            }


        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            DataTable dt_in = new DataTable();
            dt_in.Columns.Add("物料编码");
            dt_in.Columns.Add("物料名称");
            dt_in.Columns.Add("规格型号");
            dt_in.Columns.Add("仓库号");
            dt_in.Columns.Add("仓库名称");
            dt_in.Columns.Add("数量");

            DataTable dt_out = new DataTable();
            dt_out.Columns.Add("物料编码");
            dt_out.Columns.Add("物料名称");
            dt_out.Columns.Add("规格型号");
            dt_out.Columns.Add("仓库号");
            dt_out.Columns.Add("仓库名称");
            dt_out.Columns.Add("数量");
            dt_out.Columns.Add("颜色或月牙膜");

            string s = @"select  cinvcode,iquantity,CFree1+cFree2 as 颜色或月牙膜,cwhcode,属性值 as 仓库名称,规格型号,物料名称 from [192.168.20.150].UFDATA_008_2018.dbo.currentstock 
     left join (select  属性值,属性字段1 from 基础数据基础属性表  where 属性类别='仓库类别') x on x.属性字段1=cwhcode
left join 基础数据物料信息表 base on base.物料编码=cinvcode
       where cinvcode in (select  原有编码  from [4月一码多物])";
            DataTable list = CZMaster.MasterSQL.Get_DataTable(s, strcon); //对照表


            s = "select  * from [4月一码多物]";
            DataTable t_对照 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            foreach (DataRow dr in list.Rows)
            {
                //物料号 颜色或月牙膜 名称 规格 仓库号 仓库名称  数量  出库   对应什么 入库 
                DataRow r_out = dt_out.NewRow();
                r_out["物料编码"] = dr["cinvcode"];
                r_out["物料名称"] = dr["物料名称"];
                r_out["数量"] = dr["iquantity"];
                r_out["颜色或月牙膜"] = dr["颜色或月牙膜"];
                r_out["规格型号"] = dr["规格型号"];
                r_out["仓库号"] = dr["cwhcode"];
                r_out["仓库名称"] = dr["仓库名称"];
                dt_out.Rows.Add(r_out);
                string color = dr["颜色或月牙膜"].ToString();

                DataRow[] rr = t_对照.Select(string.Format("原有编码='{0}'", dr["cinvcode"]));
                if (rr.Length > 0)
                {
                    foreach (DataRow rrr in rr)
                    {
                        if (dr["颜色或月牙膜"].ToString().Contains(rrr["颜色"].ToString()))
                        {
                            DataRow r_in = dt_in.NewRow();
                            r_in["物料编码"] = rrr["变更后新编码"];
                            r_in["数量"] = dr["iquantity"];
                            r_in["物料名称"] = rrr["物料名称"];
                            r_in["规格型号"] = rrr["新规格"];
                            r_in["仓库号"] = dr["cwhcode"];
                            r_in["仓库名称"] = dr["仓库名称"];
                            dt_in.Rows.Add(r_in);
                            break;
                        }
                    }
                }

            }

            ERPorg.Corg.TableToExcel(dt_in, @"C:\Users\GH\Desktop\未完成数据\其他入库数据.xlsx");
            ERPorg.Corg.TableToExcel(dt_out, @"C:\Users\GH\Desktop\未完成数据\其他出库数据.xlsx");
        }
        //返回 颜色 传入的是 U8现存量的 颜色+月牙膜
        private string KeyValue(string s)
        {
            string color = "";

            return color;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DataTable dt_in = new DataTable();
            dt_in.Columns.Add("物料编码");
            dt_in.Columns.Add("物料名称");
            dt_in.Columns.Add("规格型号");
            dt_in.Columns.Add("仓库号");
            dt_in.Columns.Add("仓库名称");
            dt_in.Columns.Add("数量");
            dt_in.Columns.Add("原编码");
            dt_in.Columns.Add("颜色或月牙膜");
            dt_in.Columns.Add("原数量");
            dt_in.Columns.Add("原仓库号");
            dt_in.Columns.Add("原仓库名称");
            dt_in.Columns.Add("原型号");
            dt_in.Columns.Add("原名称");
            DataTable dt_out = new DataTable();
            dt_out.Columns.Add("物料编码");
            dt_out.Columns.Add("物料名称");
            dt_out.Columns.Add("规格型号");
            dt_out.Columns.Add("仓库号");
            dt_out.Columns.Add("仓库名称");
            dt_out.Columns.Add("数量");
            dt_out.Columns.Add("颜色或月牙膜");

            string s = @"select  kc.物料编码,存货分类,物料名称,规格型号,备注1 颜色,备注2 月牙膜,库存总数,kc.仓库名称,备注1+备注2 as 颜色或月牙膜,kc.仓库号   from 仓库物料数量明细表 kc
            left join 基础数据物料信息表 base on base.物料编码=kc.物料编码
            where kc.物料编码 in (select  物料编码  from (select  x.物料编码,COUNT(*)种类  from (
            select  物料编码,备注1,备注2 from 仓库物料数量明细表 group by 物料编码,备注1,备注2)x 
            group by   物料编码  )xx where 种类>1  ) ";
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = "select  * from [4月一码多物]";
            DataTable t_对照 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            foreach (DataRow dr in t.Rows)
            {
                //物料号 颜色或月牙膜 名称 规格 仓库号 仓库名称  数量  出库   对应什么 入库 
                DataRow r_out = dt_out.NewRow();
                r_out["物料编码"] = dr["物料编码"];
                r_out["物料名称"] = dr["物料名称"];
                r_out["数量"] = dr["库存总数"];
                r_out["颜色或月牙膜"] = dr["颜色或月牙膜"];
                r_out["规格型号"] = dr["规格型号"];
                r_out["仓库号"] = dr["仓库号"];
                r_out["仓库名称"] = dr["仓库名称"];
                dt_out.Rows.Add(r_out);

                DataRow r_in = dt_in.NewRow();

                r_in["原数量"] = dr["库存总数"];
                r_in["原名称"] = dr["物料名称"];
                r_in["原型号"] = dr["规格型号"];
                r_in["原仓库号"] = dr["仓库号"];
                r_in["原仓库名称"] = dr["仓库名称"];
                r_in["原编码"] = dr["物料编码"];
                r_in["颜色或月牙膜"] = dr["颜色或月牙膜"];
                dt_in.Rows.Add(r_in);

                string color = dr["颜色或月牙膜"].ToString();

                DataRow[] rr = t_对照.Select(string.Format("原有编码='{0}'", dr["物料编码"]));
                if (rr.Length > 0)
                {
                    foreach (DataRow rrr in rr)
                    {
                        if (color.Contains(rrr["颜色"].ToString()))
                        {
                            r_in["物料编码"] = rrr["变更后新编码"];
                            r_in["数量"] = dr["库存总数"];
                            r_in["物料名称"] = rrr["物料名称"];
                            r_in["规格型号"] = rrr["新规格"];
                            r_in["仓库号"] = dr["仓库号"];
                            r_in["仓库名称"] = dr["仓库名称"];
                            break;
                        }
                        else if (rrr["颜色"].ToString().Contains(color))
                        {

                            r_in["物料编码"] = rrr["变更后新编码"];
                            r_in["数量"] = dr["库存总数"];
                            r_in["物料名称"] = rrr["物料名称"];
                            r_in["规格型号"] = rrr["新规格"];
                            r_in["仓库号"] = dr["仓库号"];
                            r_in["仓库名称"] = dr["仓库名称"];
                            break;
                        }

                    }

                }
                //找不到 需要根据新型号里面的 


            }


            ERPorg.Corg.TableToExcel(dt_in, @"C:\Users\GH\Desktop\未完成数据\其他入库数据.xlsx");
            ERPorg.Corg.TableToExcel(dt_out, @"C:\Users\GH\Desktop\未完成数据\其他出库数据.xlsx");


        }

        private void button9_Click(object sender, EventArgs e)
        {
            string sql1 = "select  * from [采购记录采购单主表]  where 1=2";
            DataTable salemain = CZMaster.MasterSQL.Get_DataTable(sql1, strcon);
            string sql2 = "select  * from [采购记录采购单明细表]  where 1=2";
            DataTable saledetail = CZMaster.MasterSQL.Get_DataTable(sql2, strcon);
            string sql3 = "select  * from [销售记录销售出库通知单主表]  where 1=2";
            DataTable tzmain = CZMaster.MasterSQL.Get_DataTable(sql3, strcon);
            string sql4 = "select  * from [销售记录销售出库通知单明细表]  where 1=2";
            DataTable tzdetail = CZMaster.MasterSQL.Get_DataTable(sql4, strcon);
            string sql5 = "select  * from [销售记录成品出库单主表]  where 1=2";
            DataTable ckmain = CZMaster.MasterSQL.Get_DataTable(sql5, strcon);
            string sql6 = "select  * from [销售记录成品出库单明细表]  where 1=2";
            DataTable ckdetail = CZMaster.MasterSQL.Get_DataTable(sql6, strcon);

            //取出 期初到货单
            string s = @"    select * from[4-21U8发货单列表] where 表体订单号='' and 表头订单号='' and 数量> 0 and 发货单号 not in(               
                select 发货单号 from[4-21U8发货单列表]  where 发货单号    in  
                (select 发货单号 from[4-21销售出库单列表]  where 备注 like '%借出借用单%'))";
            //生成销售记录和销售出库记录
            DataTable t_到货 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            t_到货 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            string ss = @"select 发货单号 from[4-21U8发货单列表] where 表体订单号='' and 表头订单号=''  and 数量> 0 and 发货单号 not in(               
                select 发货单号 from[4-21U8发货单列表]  where 发货单号    in  
                (select 发货单号 from[4-21销售出库单列表]  where 备注 like '%借出借用单%')) group by 发货单号";
            DataTable t_汇总 = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string s = "select  * from [5-4备份库存]";
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s,strcon);
            s = "  select  物料编码,仓库号,SUM(实效数量) as 总出入 from 仓库出入库明细表 where 出入库时间 >'2019-4-30' and 明细类型<>'归还入库'  group by 物料编码,仓库号";
            DataTable t_cr = CZMaster.MasterSQL.Get_DataTable(s,strcon);
            string ss = "select  * from 仓库物料数量表 ";
            DataTable t_库存 = CZMaster.MasterSQL.Get_DataTable(ss,strcon);

            foreach(DataRow  dr   in  t_cr.Rows)
            {
                //期初库存
               DataRow []xr=t.Select(string.Format("物料编码='{0}' and 仓库号='{1}'",dr["物料编码"], dr["仓库号"]));

                //应剩余库存
              //  xr[0]["库存总数"] = Convert.ToDecimal(xr[0]["库存总数"])+ Convert.ToDecimal(dr["总出入"]);

                //修改现有库存
                DataRow[] yr = t_库存.Select(string.Format("物料编码='{0}' and 仓库号='{1}'", dr["物料编码"], dr["仓库号"]));
                decimal dec_原库存 = 0;
                if (xr.Length != 0) dec_原库存 = Convert.ToDecimal(xr[0]["库存总数"]);
              yr[0]["库存总数"] = dec_原库存 + Convert.ToDecimal(dr["总出入"]);

            }
            s = "select  * from 仓库物料数量表 where 1=2";

            using (SqlDataAdapter a = new SqlDataAdapter(s, strcon))
            {
                new SqlCommandBuilder(a);
                a.Update(t_库存);
            }




        }
    }

}
