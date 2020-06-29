using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPpurchase
{
    public partial class ui委外补料 : UserControl
    {
        #region 变量
        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dt_下拉;
        //DataTable dt_生产;
        string str_采购明细;
        string str_物料号;
        bool fl_退料 = false;

        #endregion


        public ui委外补料()
        {
            InitializeComponent();
        }



        public ui委外补料(string str, string str_物料编号)
        {
            InitializeComponent();
            str_采购明细 = str;
            str_物料号 = str_物料编号;
        }
        public ui委外补料(string str, string str_物料编号, bool bl_退料)
        {
            InitializeComponent();
            str_采购明细 = str;
            str_物料号 = str_物料编号;
            fl_退料 = true;
        }
        private void ui委外补料_Load(object sender, EventArgs e)
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



        #region 函数
        private void fun_load()
        {
            string sql = string.Format(@"   
 select  *,case when 可退数1< 0 then 0  else 可退数1 end as 可退数 from (
 select fx.物料编码 as 父项编号, 子项编码 as 子项编号,zx.计量单位,base.数量,kc.有效总数,kc.库存总数,base.仓库号,base.仓库名称,备注='{1}'
             ,zx.物料名称,zx.规格型号, case when 可退数<xx.总-完成数量*base.数量 then 可退数 else xx.总-完成数量*base.数量 end as 可退数1  from [基础数据物料BOM表] base
             left join 基础数据物料信息表 zx  on zx.物料编码  =base.子项编码
             left  join 基础数据物料信息表 fx on fx.物料编码=base.产品编码 
             left join 仓库物料数量表 kc  on  kc.物料编码= zx.物料编码 
             left join ( select a.物料编码,a.总, a.总-isnull(b.已核销数量,0) as 可退数 from(
select 物料编码, SUM(数量) as 总 from 其他出入库申请子表 a where   备注='{1}' and 作废=0  group by 物料编码)a
left join(select  物料编码,SUM(委外已核量)已核销数量 from 其他出库子表 where 备注='{1}' group by 物料编码)b
on a.物料编码=b.物料编码)xx on xx.物料编码=子项编码
left join 采购记录采购单明细表 cmx on cmx.采购明细号='{1}'
             where fx.物料编码  ='{0}' and kc.仓库号=base.仓库号)yy", str_物料号, str_采购明细); 
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dtM = new DataTable();
                da.Fill(dtM);
                DataColumn dc = new DataColumn("选择",typeof(bool));
                dc.DefaultValue = false;
                dtM.Columns.Add(dc);

                if (dtM.Rows.Count > 0)
                {
                    if (fl_退料)
                    {
                        dtM.Columns.Add("输入退料数量");
                        gridView1.ViewCaption = "委外退料列表";
                        gridColumn4.FieldName = "输入退料数量";
                        gridColumn4.Caption = "输入退料数量";
                        gridColumn2.Visible = true; //可退数 // 补料得要考虑 正常发的+补料的-退料的+申请未审核的 = 可退的数量20-1-20

                    }
                    else
                    {
                        gridView1.ViewCaption = "委外补料列表";
                        gridColumn4.FieldName = "输入领料数量";
                        gridColumn4.Caption = "输入领料数量";
                        dtM.Columns.Add("输入领料数量");
                        gridColumn2.Visible = false;

                    }
                    gridControl1.DataSource = dtM;
                }
                else
                {
                    MessageBox.Show("未找到该物料的委外BOM");
                }
            }
        }

        private void fun_wwll()
        {
            DataSet ds = new DataSet();
            string s = "select  * from  其他出入库申请主表 where  1=2";
            DataTable dt_主 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = "select  * from  其他出入库申请子表 where 1<>1";
            DataTable dt_子 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DateTime t = CPublic.Var.getDatetime();
            //DateTime t =new DateTime (2020,4,30);

            string s_ID = CPublic.Var.LocalUserID;
            string s申请_no = string.Format("DWLS{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                             t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("DWLS", t.Year, t.Month).ToString("0000"));
            DataRow dr_申请主 = dt_主.NewRow();

            dr_申请主["GUID"] = System.Guid.NewGuid();
            dr_申请主["出入库申请单号"] = s申请_no;

            dr_申请主["申请日期"] = t;
            dr_申请主["操作人员编号"] = CPublic.Var.LocalUserID;
            dr_申请主["操作人员"] = CPublic.Var.localUserName;
            dr_申请主["生效"] = true;
            dr_申请主["审核"] = true;
            dr_申请主["审核日期"] = t;
            dr_申请主["审核人员"] ="系统自动审核";
            dr_申请主["待审核"] = true;
            dr_申请主["生效人员编号"] = s_ID;
            dr_申请主["生效日期"] = t;
            dr_申请主["备注"] = str_采购明细.Substring(0, 14);//关联采购单号
            dr_申请主["申请类型"] = "材料出库";
            dr_申请主["单据类型"] = "材料出库";
            if (fl_退料)
            {
                dr_申请主["原因分类"] = "委外退料";
                dr_申请主["红字回冲"] = true;
                dr_申请主["业务单号"] = str_采购明细.Substring(0, 14);
            }
            else
            {
                dr_申请主["原因分类"] = "委外补料";
            }
            dt_主.Rows.Add(dr_申请主);
            int pos = 1;
            foreach (DataRow r in dtM.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                if (r["选择"].Equals(true))
                {
                    //采购明细间 可能BOM中存在同一子项 不合并 每条申请明细都关联一条采购明细
                    DataRow dr_子 = dt_子.NewRow();
                    dr_子["GUID"] = System.Guid.NewGuid();
                    dr_子["出入库申请单号"] = s申请_no;
                    dr_子["出入库申请明细号"] = s申请_no + "-" + pos.ToString("00");
                    dr_子["POS"] = pos;
                    dr_子["物料编码"] = r["子项编号"].ToString();
                    dr_子["物料名称"] = r["物料名称"].ToString();
                    if (fl_退料)
                    {
                        dr_子["数量"] = -Convert.ToDecimal(r["输入退料数量"]);
                    }
                    else
                    {
                        dr_子["数量"] = r["输入领料数量"];
                    }
                    dr_子["委外bom数量"] = r["数量"];
                    dr_子["规格型号"] = r["规格型号"].ToString();
                    dr_子["仓库号"] = r["仓库号"];
                    dr_子["仓库名称"] = r["仓库名称"].ToString();
                    dr_子["备注"] = r["备注"];
                    dr_子["生效"] = true;
                    dr_子["生效人员编号"] = s_ID;
                    dr_子["生效日期"] = t;
                    dt_子.Rows.Add(dr_子);
                    pos++;
                }
            }
            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction wwbl = conn.BeginTransaction("委外补料");
            try
            {
                SqlCommand cmd = new SqlCommand("select * from 其他出入库申请主表 where 1<>1", conn, wwbl);
                SqlDataAdapter  aa = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(aa);
                aa.Update(dt_主);
                cmd = new SqlCommand("select * from 其他出入库申请子表 where 1<>1", conn, wwbl);
                aa = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(aa);
                aa.Update(dt_子);
                wwbl.Commit();
            }
            catch (Exception ex)
            {
                wwbl.Rollback();
                throw new Exception("生效失败，请重试");
            }


        }
        private void fun_check()
        {
            //int i = 0;
            DataView dv = new DataView(dtM);
            dv.RowFilter = "选择=1";
            if (dv.Count == 0)
            {
                throw new Exception("未选择需补料的物料");
            }
            foreach (DataRow r in dtM.Rows)
            {
                if (r["选择"].Equals(true))
                {
                    try
                    {
                        if (fl_退料)
                        {
                            decimal a = Convert.ToDecimal(r["输入退料数量"]);
                            decimal b = Convert.ToDecimal(r["可退数"]);

                            if (a > b) throw new Exception("输入数量已经大于可退数，请确认是否正确"); 
                            if (a <= 0)
                            {
                                throw new Exception("退料数量不能小于0,请重新输入");

                            }
                        }
                        else
                        {
                            decimal a = Convert.ToDecimal(r["输入领料数量"]);

                            if (a <= 0)
                            {
                                throw new Exception("领料数量不能小于0,请重新输入");

                            }
                            if (Convert.ToDecimal(r["输入领料数量"]) > Convert.ToDecimal(r["库存总数"]))
                            {
                                throw new Exception("库存总数不足！");
                            }
                        }
                    }
                    catch(Exception ex )
                    {
                        throw new Exception(ex.Message);

                    }

                 
                    //if (Convert.ToDecimal(r["输入领料数量"]) > Convert.ToDecimal(r["未领数量"]))
                    //{
                    //    i++;

                    //}
                }
            }
            //if (i > 0)
            //{
            //    if (MessageBox.Show("领料数量大于未领数量，是否继续？", "提醒", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
            //    {
            //        throw new Exception("请修改");
            //    }
            //}

        }

        #endregion

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                fun_check();

                fun_wwll();
                MessageBox.Show("申请成功");
                CPublic.UIcontrol.ClosePage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
