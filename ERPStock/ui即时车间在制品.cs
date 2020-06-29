using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPStock
{
    public partial class ui即时车间在制品 : UserControl
    {

        #region
        string strcon = CPublic.Var.strConn;
        DataTable dtM;


        #endregion

        public ui即时车间在制品()
        {
            InitializeComponent();
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

                MessageBox.Show("加载失败");
            }
        }
        private void fun_load()
        {
            dtM = new DataTable();
            string sql_在产 = @"select dlmx.生产工单号,b.物料编码 as 产品编码,b.规格型号 as 产品型号,b.物料名称 as 产品名称,gd.生产工单类型,gd.班组,车间名称,dlmx.物料名称 as 子项名称 ,待领料总量 
,已领数量,a.物料编码 as 子项编码,a.规格型号 as 子项型号,a.n核算单价 as 子项单价,b.n核算单价 as 父项单价,生产数量,isnull(j.已入库数量,0)已入库数量 
 ,(已领数量-待领料总量/生产数量*isnull(已入库数量,0))as 在制品   from 生产记录生产工单待领料明细表 dlmx
left  join  生产记录生产工单表 gd on gd.生产工单号=dlmx.生产工单号
left  join  基础数据物料信息表 a on a.物料编码=dlmx.物料编码
left  join  基础数据物料信息表 b on b.物料编码=gd.物料编码
left join  (select 生产工单号,SUM(入库数量)已入库数量 from 生产记录成品入库单明细表 group by 生产工单号)j 
			on j.生产工单号= dlmx.生产工单号
where dlmx.生产工单号 in (select 生产工单号 from 生产记录生产工单表 gd,基础数据物料信息表 
      where  基础数据物料信息表.物料编码=gd.物料编码 and  gd.生效=1 and gd.关闭=0 and  gd.完成=0 
    and gd.完工=0 and gd.生效日期>='2019-5-1' /*and gd.生效日期<'2017-1-1'*/)";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_在产, strcon))
            {
                da.Fill(dtM);
            }

            string sql_完工未检 = @"select dlmx.生产工单号,b.物料编码 as 产品编码,b.规格型号 as 产品型号,b.物料名称 as 产品名称,gd.生产工单类型,车间名称,gd.班组,dlmx.物料名称 as 子项名称 ,待领料总量 
,已领数量,a.物料编码 as 子项编码,a.规格型号 as 子项型号,a.n核算单价 as 子项单价,b.n核算单价 as 父项单价,生产数量,isnull(j.已入库数量,0)已入库数量,
(已领数量-待领料总量/生产数量*isnull(已入库数量,0))as 在制品   
from 生产记录生产工单待领料明细表 dlmx
left  join  生产记录生产工单表 gd on gd.生产工单号=dlmx.生产工单号
left  join  基础数据物料信息表 a on a.物料编码=dlmx.物料编码
left  join  基础数据物料信息表 b on b.物料编码=gd.物料编码
left join  (select 生产工单号,SUM(入库数量)已入库数量 from 生产记录成品入库单明细表 group by 生产工单号)j 
			on j.生产工单号= dlmx.生产工单号
where dlmx.生产工单号 in (select 生产工单号  from 生产记录生产工单表 gd,基础数据物料信息表 
 where  基础数据物料信息表.物料编码=gd.物料编码 and gd.生效=1 and gd.完工=1  and gd.关闭=0 and  gd.完成=0 
and gd.生产工单号 not in (select 生产工单号 from 生产记录生产检验单主表) and gd.完工日期>='2019-5-1' 
/*and gd.完工日期<'2017-1-1'*/ and gd.生效日期>'2019-5-1') ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_完工未检, strcon))
            {
                da.Fill(dtM);
            }

            string sql_已检未入 = @"select dlmx.生产工单号,b.物料编码 as 产品编码,b.规格型号 as 产品型号,b.物料名称 as 产品名称,gd.生产工单类型,车间名称,gd.班组,dlmx.物料名称 as 子项名称 ,待领料总量 
,已领数量,a.物料编码 as 子项编码,a.规格型号 as 子项型号,a.n核算单价 as 子项单价,b.n核算单价 as 父项单价,生产数量,isnull(j.已入库数量,0)已入库数量 
 ,(已领数量-待领料总量/生产数量*isnull(已入库数量,0))as 在制品    from 生产记录生产工单待领料明细表 dlmx
left  join  生产记录生产工单表 gd on gd.生产工单号=dlmx.生产工单号
left  join  基础数据物料信息表 a on a.物料编码=dlmx.物料编码
left  join  基础数据物料信息表 b on b.物料编码=gd.物料编码
left join  (select 生产工单号,SUM(入库数量)已入库数量 from 生产记录成品入库单明细表 group by 生产工单号)j 
			on j.生产工单号= dlmx.生产工单号
where dlmx.生产工单号 in ( select gd.生产工单号  from 生产记录生产工单表 gd,基础数据物料信息表,生产记录生产检验单主表,仓库物料数量表  
       where  基础数据物料信息表.物料编码=gd.物料编码 and 仓库物料数量表.物料编码=gd.物料编码
         and gd.生产工单号= 生产记录生产检验单主表.生产工单号
         and gd.生产工单号 in ( select 生产工单号 from 生产记录生产检验单主表 
         where   gd.关闭=0 and 生产记录生产检验单主表.生效=1 and 生产记录生产检验单主表.完成=0 and gd.完成=0
         and gd.生效日期>'2019-5-1'  and 检验日期 >= '2019-5-1' /*   and 检验日期< '2017-1-1' */))  ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_已检未入, strcon))
            {
                da.Fill(dtM);
            }
            //2020-3-19 暂时这么group by 一下,时间搞
            MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
           dtM = RBQ.SelectGroupByInto("", dtM, "生产工单号,产品编码,产品型号,产品名称,生产工单类型,车间名称,班组,子项名称,待领料总量,已领数量,子项编码,子项型号,子项单价,父项单价,生产数量,已入库数量,在制品"
               , "", "生产工单号,产品编码,产品型号,产品名称,生产工单类型,车间名称,班组,子项名称,待领料总量,已领数量,子项编码,子项型号,子项单价,父项单价,生产数量,已入库数量,在制品");

            gridControl1.DataSource = dtM;
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                //gridControl1.ExportToXls(saveFileDialog.FileName, options);  
               
                   gridControl1.ExportToXlsx(saveFileDialog.FileName);
              
                
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        
    }
}
