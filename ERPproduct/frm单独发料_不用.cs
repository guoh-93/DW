using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPproduct
{
    public partial class frm单独发料_不用 : UserControl
    {

        #region 变量
        string strconn = CPublic.Var.strConn;
        DataTable dt_仓库;
        string sql_ck = "";
        DataTable dt_左;
        DataTable dt_右;

        #endregion

       #region  加载
        public frm单独发料_不用()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm单独发料_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            sql_ck = string.Format("select * from 人员仓库对应表 where 工号='{0}'", CPublic.Var.LocalUserID);
            dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ck, strconn);
        }
        #endregion

#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
         
            sql_ck = "and 基础数据物料信息表.仓库号  in(";
            string sql_左 = "";
            if (dt_仓库.Rows.Count == 0)
            {
                sql_左 = string.Format(@"select 生产记录生产工单待领料主表.*,人事基础部门表.部门名称,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.n原ERP规格型号  from 生产记录生产工单待领料主表 
                                        left join  基础数据物料信息表 on 基础数据物料信息表.物料编码= 生产记录生产工单待领料主表.物料编码
                                        left join 人事基础部门表 on 人事基础部门表.部门编号= 生产记录生产工单待领料主表.生产车间
                                        where 待领料单号 in(select 待领料单号 from 生产记录生产工单待领料明细表,基础数据物料信息表 where 完成=0  and 领料类型='单独领料' and     
                                        基础数据物料信息表.物料编码=生产记录生产工单待领料明细表.物料编码  group by 待领料单号 )");
            }
            else
            {
                foreach (DataRow dr in dt_仓库.Rows)
                {
                    sql_ck = sql_ck + string.Format("'{0}',", dr["仓库号"]);

                }
                sql_ck = sql_ck.Substring(0, sql_ck.Length - 1) + ")";
                sql_左 = string.Format(@"select 生产记录生产工单待领料主表.*,人事基础部门表.部门名称,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.n原ERP规格型号 from 生产记录生产工单待领料主表
                            left join  基础数据物料信息表 on 基础数据物料信息表.物料编码= 生产记录生产工单待领料主表.物料编码
                            left join 人事基础部门表 on 人事基础部门表.部门编号= 生产记录生产工单待领料主表.生产车间                             
                          where 待领料单号 in(select 待领料单号 from 生产记录生产工单待领料明细表,基础数据物料信息表 where 完成=0  and     
                         基础数据物料信息表.物料编码=生产记录生产工单待领料明细表.物料编码 {0}  group by 待领料单号 )", sql_ck);
            }
            using (SqlDataAdapter da = new SqlDataAdapter(sql_左, strconn))
            {
                dt_左 = new DataTable();

                da.Fill(dt_左);

                gridControl1.DataSource = dt_左;
            }

            string sql_右 = "select * from 生产记录生产工单待领料明细表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_右, strconn))
            {
                dt_右 = new DataTable();
                da.Fill(dt_右);

                dt_右.Columns.Add("选择", typeof(bool));
                dt_右.Columns.Add("输入领料数量");
                gc_sclldetail.DataSource = dt_右;
            }
        }





    }
}
