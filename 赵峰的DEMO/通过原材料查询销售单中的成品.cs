using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 赵峰的DEMO
{
    public partial class 通过原材料查询销售单中的成品 : Form
    {
        DataTable dt;
        string strconn = CPublic.Var.strConn;

        public 通过原材料查询销售单中的成品()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void fun_载入()
        {
            string sql = string.Format(@"select a.物料编码,(a.受订量a - 仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量) as 受订量a,a.物料类型
,基础数据物料信息表.物料名称,基础数据物料信息表.大类,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.原ERP物料编号,
基础数据物料信息表.规格型号,基础数据物料信息表.图纸编号,基础数据物料信息表.计量单位,基础数据物料信息表.仓库名称,
基础数据物料信息表.供应商编号,基础数据物料信息表.默认供应商,基础数据物料信息表.仓库号 from 
((select aa.物料编码,case when(aa.受订量 > isnull(s.制令量,0)) then aa.受订量 else isnull(s.制令量,0) end as 受订量a,aa.物料类型 
from 
	(select [销售记录销售订单明细表].物料编码,SUM(未完成数量) as 受订量,物料类型 from [销售记录销售订单明细表] 
	left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = [销售记录销售订单明细表].物料编码
	where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
		and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
		and 生效日期 >= '{0}'and 基础数据物料信息表.物料类型 = '成品'
	group by [销售记录销售订单明细表].物料编码,物料类型) aa
left join 
	(select 生产记录生产制令表.物料编码,SUM(未排单数量) as 制令量,基础数据物料信息表.物料类型 
	from 生产记录生产制令表 
	left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产制令表.物料编码
	where 未排单数量 > 0 /*and 生产记录生产制令表.生效 = 1*/ and 完成 = 0 and 生产记录生产制令表.关闭 = 0
		and 生产记录生产制令表.日期 >= '{0}'and 基础数据物料信息表.物料类型 = '成品'
	group by 生产记录生产制令表.物料编码,基础数据物料信息表.物料类型) s
on s.物料编码 = aa.物料编码) 
union 
(select 生产记录生产制令表.物料编码,SUM(未排单数量) as 受订量a,基础数据物料信息表.物料类型 
from 生产记录生产制令表 
	left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产制令表.物料编码
	where 未排单数量 > 0 /*and 生产记录生产制令表.生效 = 1 */ and 完成 = 0 and 生产记录生产制令表.关闭 = 0 and 基础数据物料信息表.物料类型 = '成品'
		and 生产记录生产制令表.日期 >= '{0}' and 生产记录生产制令表.物料编码 not in 
		(select aa.物料编码 from 
			(select [销售记录销售订单明细表].物料编码 
			from [销售记录销售订单明细表] 
			where [明细完成] = 0 and [销售记录销售订单明细表].生效 = 1 and 作废 = 0 
			and (总完成 = 0 or 总完成 is null) and [销售记录销售订单明细表].关闭 = 0 and 未完成数量 > 0
			and 生效日期 >= '{0}' and 基础数据物料信息表.物料类型 = '成品'
			group by [销售记录销售订单明细表].物料编码) aa
		left join 
			(select 生产记录生产制令表.物料编码 
			from 生产记录生产制令表 
			where 未排单数量 > 0 /*and 生产记录生产制令表.生效 = 1*/ and 完成 = 0 and 生产记录生产制令表.关闭 = 0
			and 生产记录生产制令表.日期 >= '{0}' and 基础数据物料信息表.物料类型 = '成品'
			group by 生产记录生产制令表.物料编码) s
		on s.物料编码 = aa.物料编码
		where s.物料编码 is not null and 基础数据物料信息表.物料类型 = '成品')
	group by 生产记录生产制令表.物料编码,基础数据物料信息表.物料类型)
) a 
left join 仓库物料数量表 on 仓库物料数量表.物料编码 = a.物料编码 
left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = a.物料编码
where (a.受订量a - 仓库物料数量表.库存总数 - 仓库物料数量表.在制量 - 仓库物料数量表.在途量 + 仓库物料数量表.未领量) > 0", "2017/02/01 00:00:00");//and 生产记录生产制令表.生效 = 1 
            DataTable dt1 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt1);

            DataTable dtM = new DataTable();
            dtM.Columns.Add("物料编码");
            foreach (DataRow dr in dt1.Rows)
            {
                DataSet ds = StockCore.StockCorer.fun_得到物料BOM结构(dr["物料编码"].ToString(), strconn, "");
                DataTable dt = ds.Tables[0];
                if (dt.Select("产品编码 = '" + textBox1.Text + "'").Length > 0)
                {
                    DataRow r = dtM.NewRow();
                    dtM.Rows.Add(r);
                    r["物料编码"] = dr["物料编码"];
                }
                if (dt.Select("子项编码 = '" + textBox1.Text + "'").Length > 0)
                {
                    DataRow r = dtM.NewRow();
                    dtM.Rows.Add(r);
                    r["物料编码"] = dr["物料编码"];
                }
            }

            gc.DataSource = dtM;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            fun_载入();
        }
    }
}
