using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace 郭恒的DEMO
{
    public partial class 基础属性列转行 : Form
    {
        string strconn = CPublic.Var.strConn;
        string sql = "select * from  基础数据物料信息表 ";
        DataTable dt_属性;

        public 基础属性列转行()
        {
            InitializeComponent();
        }

        private void 基础属性列转行_Load(object sender, EventArgs e)
        {
            //DataTable dt_物料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            //dt_属性 = new DataTable();
            //dt_属性.Columns.Add("物料编码", typeof(string));
            //dt_属性.Columns.Add("物料名称", typeof(string));
            //dt_属性.Columns.Add("规格型号", typeof(string));
            //dt_属性.Columns.Add("物料属性", typeof(string));
            //dt_属性.Columns.Add("商品分类", typeof(string));
            //dt_属性.Columns.Add("产品类别", typeof(string));


            //dt_属性.Columns.Add("字段名", typeof(string));
            //dt_属性.Columns.Add("属性值", typeof(string));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //
            string s = "select  * from [CRM产品需要字段表] ";
            string xx = "产品类别,物料类型,物料编码,物料名称,规格型号,物料属性";//单独成列得字段
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s,strconn);
            s = "";
            foreach(DataRow dr in dt.Rows)
            {
                s += dr["字段名"].ToString() + ",";
            }
            if (s.Length > 0)
                s = s + xx;
            else throw new Exception("CRM产品需要字段表中无内容");

            //string sql =string.Format(@"select {0} from 基础数据物料信息表 where 规格型号 like 'EH-7000%' " +
            //    "and left(物料编码,2) in ('05','10')",s);

            string sql = string.Format(@"select {0} from 基础数据物料信息表 where 物料属性<>''", s);
            DataTable dt_基础 = CZMaster.MasterSQL.Get_DataTable(sql,strconn);
            foreach (DataRow dr in dt_基础.Rows)
            {
                foreach (DataColumn dc in dt_基础.Columns)
                {
                    if (xx.Contains( dc.ColumnName) ) continue;

                    //如果属性值为空 不需要加入 10-28 
                    //等数据全了以后  加入限制   去除空的属性数据
                    if (dr[dc.ColumnName] == null || dr[dc.ColumnName].ToString() == "") continue;
                    DataRow dr_属性 = dt_属性.NewRow();
                    dt_属性.Rows.Add(dr_属性);
                    dr_属性["物料编码"] = dr["物料编码"];
                    dr_属性["物料名称"] = dr["物料名称"];
                    dr_属性["规格型号"] = dr["规格型号"];
                    dr_属性["物料属性"] = dr["物料属性"];
                    dr_属性["商品分类"] = dr["物料类型"];
                    dr_属性["产品类别"] = dr["产品类别"];

                    dr_属性["字段名"] = dc.ColumnName;
                    dr_属性["属性值"] = dr[dc.ColumnName];
                }
            }
            ERPorg.Corg.TableToExcel(dt_属性, @"C:\Users\GH\Desktop\10-28.xlsx");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            strconn = "server=rm-bp1d07z6f3766vn6qio.mysql.rds.aliyuncs.com;User Id=dw002;password=Dongwu002;Database=erp-dongwu;CharSet=utf8";

            //         server=rm-bp1d07z6f3766vn6qio.mysql.rds.aliyuncs.com;User Id=dw002;password=Dongwu002;Database=erp-dongwu;CharSet=utf8

            DataTable  dt_保存 = new DataTable();
            string sql = "SELECT * FROM inventory_new ";
            MySqlDataAdapter da = new MySqlDataAdapter(sql, strconn);
            da.Fill(dt_保存);


        }

        private void button3_Click(object sender, EventArgs e)
        {
            ERPorg.Corg cg = new ERPorg.Corg();
            textBox2.Text =cg.NumToChinese(textBox1.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string s = string.Format("select * from 生产记录生产工单表 where 生产工单号='{0}'", textBox3.Text);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s,strconn);
            DataSet ds_sn = fun_SN(dt);
            CZMaster.MasterSQL.Save_DataTable(ds_sn.Tables[1], "Print_ShareLockInfo", strconn);
            ///2019-10-16  这边要保存另一个数据库  目前我不知道怎么两个数据用类似事务的方式一起保存 
            string str_BQ = CPublic.Var.geConn("BQ");
            CZMaster.MasterSQL.Save_DataTable(ds_sn.Tables[0], "ShareLockInfo", str_BQ);
        }
        private string total_JY(string s)
        {
            string xx = "";
            int sum = 0;
            foreach (char c in s)
            {
                sum = sum + Convert.ToInt32(c.ToString());
            }
            xx = sum.ToString("000");
            return xx;
        }
        public DataSet fun_SN(DataTable t_MakeOrder)
        {
            DataSet ds = new DataSet();
            DateTime time = CPublic.Var.getDatetime();
            string tNo = time.Year.ToString().Substring(2, 2) + time.Month.ToString("00") + time.Day.ToString("00"); //时间流水
            string strcon_BQ = "";
            try
            {
                strcon_BQ = CPublic.Var.geConn("BQ");
            }
            catch (Exception ex)
            {
                throw new Exception("未正确配置标签数据库,请确认");
            }
            strcon_BQ = CPublic.Var.geConn("BQ");
            string s = "select * from ShareLockInfo where 1=2 ";
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon_BQ);
            s = " select * from Print_ShareLockInfo where 1=2";
            DataTable tt = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            ds.Tables.Add(dt);
            ds.Tables[0].TableName = "存东屋检验数据库记录";
            ds.Tables.Add(tt);
            ds.Tables[1].TableName = "存供应链数据库记录";
            ds.Tables.Add(t_MakeOrder);
            ds.Tables[2].TableName = "工单记录";

            foreach (DataRow dr in t_MakeOrder.Rows)
            {
                //生产数量
                int dec_M = Convert.ToInt32(dr["生产数量"]);
                string LabSpCode = "";
                int NumBegin = 0;
                //[Mac规则ID]=0  不需要生产条码
                string ss = string.Format("select [产品简码] as  LabSpCode,[Mac规则ID] as RuleID from [基础物料标签维护信息表] where 物料编号='{0}' ", dr["物料编码"]);
                DataTable t = CZMaster.MasterSQL.Get_DataTable(ss, strconn);
                if (dr["MaxNo"] != null && dr["MaxNo"].ToString() != "")
                    NumBegin = Convert.ToInt32(dr["MaxNo"]);
                if (NumBegin == 0) NumBegin++;
                if (t.Rows.Count > 0)
                {
                    LabSpCode = t.Rows[0]["LabSpCode"].ToString();
                    for (int x = 0; x < dec_M; x++)
                    {
                        string sn = "";
                        string stemp = LabSpCode + tNo + NumBegin.ToString().PadLeft(6, '0');
                        sn = stemp + total_JY(stemp);
                        if (t.Rows[0]["RuleID"].ToString() != "0")
                        {//如果ruleID为0 只需要供应链中生成SN号 不需要写到BQ数据库里面

                            DataRow r = dt.NewRow();
                            r["DevType"] = t.Rows[0]["RuleID"];
                            r["CTNo"] = sn;
                            r["CheckFlag"] = "0";
                            r["TaskNo"] = dr["生产工单号"];
                            dt.Rows.Add(r);
                        }
                        DataRow rr = tt.NewRow();
                        rr["DevType"] = t.Rows[0]["RuleID"];
                        rr["CTNo"] = sn;
                        rr["CheckFlag"] = "0";
                        rr["MakeOrder"] = dr["生产工单号"];
                        tt.Rows.Add(rr);
                        NumBegin++;
                    }
                    dr["MaxNo"] = NumBegin;
                }
            }
            return ds;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("SN", "01502191113");// sn前11位
            dic.Add("LockID", "12345"); //lockID 先去中间表取当前 锁号规则流水号是几位 x    len(LockID)-x
            dic.Add("FCCID", "123456");
            dic.Add("idls", "1"); //锁号的流水号
            dic.Add("snls", "1"); //sn的流水号

            dic.Add("qsyzm", "24"); //起始验证码
          
        string path = @"C:\Users\GH\Desktop\10-17模板\SN标签";
       ERPproduct.Lprinter lp = new ERPproduct.Lprinter(path, dic, "", 20);
        lp.DoWork();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string s = "select  * from [导入软件记录]";
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s,strconn);
            s = "select  * from [产品软件对应表] ";
            DataTable dt_产软对应 = CZMaster.MasterSQL.Get_DataTable(s, strconn);

            s = "select  * from [2019财务软件费用] ";
            DataTable dt_软件费用 = CZMaster.MasterSQL.Get_DataTable(s, strconn);

            s = "select  * from [软件单价基础表]";
            DataTable t_列 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            t_列=CZMaster.MasterSQL.Get_DataTable(s,strconn);

            foreach (DataRow dr  in dt.Rows)
            {
                 decimal dec_单价 = 0;
                foreach (DataRow rr  in t_列.Rows)
                {
                    if(dr[rr["软件名称"].ToString()].ToString()=="1")
                    {
                        dec_单价 += Convert.ToDecimal(rr["单价"]);
                        DataRow r = dt_产软对应.NewRow();
                        r["GUID"] = System.Guid.NewGuid();
                        r["物料编码"] = dr["产品编码"].ToString();
                        r["软件名称"] = rr["软件名称"].ToString();
                        dt_产软对应.Rows.Add(r);
                    }
             
                }
                DataRow rrx = dt_软件费用.NewRow();
                rrx["产品编码"] = dr["产品编码"].ToString();
                rrx["单价"] = dec_单价;
                dt_软件费用.Rows.Add(rrx);
            }
             // CZMaster.MasterSQL.Save_DataTable(dt_产软对应, "产品软件对应表", strconn);

            CZMaster.MasterSQL.Save_DataTable(dt_软件费用, "[2019财务软件费用]", strconn);

        }
    }
}
