using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 郭恒的DEMO
{
    public partial class fm校验未入库倒冲物料及数量 : Form
    {
        string strcon = CPublic.Var.strConn;
        DataTable dt_result = new DataTable();

        public fm校验未入库倒冲物料及数量()
        {
            InitializeComponent();
        }
        private void fm校验未入库倒冲物料及数量_Load(object sender, EventArgs e)
        {
            dt_result = new DataTable();
            dt_result.Columns.Add("生产工单号");
            dt_result.Columns.Add("物料编码");
            dt_result.Columns.Add("物料名称");
            dt_result.Columns.Add("规格型号");
            dt_result.Columns.Add("生产数量",typeof(decimal));
            dt_result.Columns.Add("bom用量",typeof(decimal));
            dt_result.Columns.Add("完工数量", typeof(decimal));

        }

        private void button1_Click(object sender, EventArgs e)
        {
             string x = @"select  *,case when 完工=1 then 生产数量 else 部分完工数 end as 完工数量  from 生产记录生产工单表 where 生效日期>'2019-5-1'
            and 生效日期<'2019-11-20'   and 生产工单类型<>'返修工单' and(完工 = 1 or 部分完工 = 1)";

           // string x = @"select  * from 生产工单完工记录表   where  完工日期 >'2019-5-1' and   完工日期 <'2019-11-19 17:29:00'";
            DataTable t_工单 = CZMaster.MasterSQL.Get_DataTable(x,strcon);
            foreach(DataRow dr in t_工单.Rows)
            {
                string s = string.Format(@"with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,优先级,bom_level ) as
 (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,优先级,1 as level from 基础数据物料BOM表    where 产品编码='{0}'
   union all 
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型, convert(decimal(18, 6),a.数量*b.数量) as 数量,a.bom类型,a.优先级,b.bom_level+1  from 基础数据物料BOM表 a
   inner join temp_bom b on a.产品编码=b.子项编码 where b.wiptype='虚拟'  
   ) 
    select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,wiptype,子项类型,sum(数量)数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号  
  ,base.计量单位,base.计量单位编码 from  temp_bom a
  left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
  left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码 where wiptype ='入库倒冲' and 优先级=1 and bom_level>1
  group by 产品编码,fx.物料名称 ,子项编码,base.物料名称 ,wiptype,子项类型 ,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号
  ,base.计量单位,base.计量单位编码",dr["物料编码"]);
                DataTable t_入库倒冲 = CZMaster.MasterSQL.Get_DataTable(s,strcon);
                if(t_入库倒冲.Rows.Count>0)
                {
                    foreach (DataRow r_chong in t_入库倒冲.Rows)
                    {
                        DataRow r = dt_result.NewRow();
                        r["生产工单号"] = dr["生产工单号"];
                        r["生产数量"] = dr["生产数量"];
                        r["完工数量"] = dr["完工数量"];
                        r["物料编码"] = r_chong["子项编码"];
                        r["物料名称"] = r_chong["子项名称"];
                        r["规格型号"] = r_chong["规格型号"];
                        r["bom用量"] = r_chong["数量"];
                        dt_result.Rows.Add(r);
                    }
                }
            }


            ERPorg.Corg.TableToExcel(dt_result, @"C:\Users\GH\Desktop\未入库倒冲的量.xlsx");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string x = @"select  *,case when 完工=1 then 生产数量 else 部分完工数 end as 完工数  from 生产记录生产工单表 where 生效日期>'2019-5-1'
            and 生效日期<'2019-11-19'   and 生产工单类型<>'返修工单'";
            DataTable t_工单 = CZMaster.MasterSQL.Get_DataTable(x, strcon);


            foreach (DataRow dr in t_工单.Rows)
            {
                string s = string.Format(@"with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,优先级,bom_level ) as
 (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,优先级,1 as level from 基础数据物料BOM表    where 产品编码='{0}'
   union all 
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型, convert(decimal(18, 6),a.数量*b.数量) as 数量,a.bom类型,a.优先级,b.bom_level+1  from 基础数据物料BOM表 a
   inner join temp_bom b on a.产品编码=b.子项编码 where b.wiptype='虚拟'  
   ) 
    select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,wiptype,子项类型,sum(数量)数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号  
  ,base.计量单位,base.计量单位编码 from  temp_bom a
  left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
  left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码 where wiptype ='入库倒冲' and 优先级=1  
  group by 产品编码,fx.物料名称 ,子项编码,base.物料名称 ,wiptype,子项类型 ,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号
  ,base.计量单位,base.计量单位编码", dr["物料编码"]);
                DataTable t_入库倒冲 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (t_入库倒冲.Rows.Count > 0)
                {
                    foreach (DataRow r_chong in t_入库倒冲.Rows)
                    {
                        DataRow r = dt_result.NewRow();
                        r["生产工单号"] = dr["生产工单号"];
                        r["生产数量"] = dr["生产数量"];
                        r["完工数量"] = dr["完工数"];
                        r["物料编码"] = r_chong["子项编码"];
                        r["物料名称"] = r_chong["子项名称"];
                        r["规格型号"] = r_chong["规格型号"];
                        r["bom用量"] = r_chong["数量"];
                        dt_result.Rows.Add(r);
                    }
                }
            }


            ERPorg.Corg.TableToExcel(dt_result, @"C:\Users\GH\Desktop\未入库倒冲的量.xlsx");
        }
    }
}
