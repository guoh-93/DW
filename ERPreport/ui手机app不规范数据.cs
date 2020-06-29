using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPreport
{
    public partial class ui手机app不规范数据 : UserControl
    {

        #region 
        string strcon = CPublic.Var.strConn;




        #endregion

        public ui手机app不规范数据()
        {
            InitializeComponent();
        }

        private void ui手机app不规范数据_Load(object sender, EventArgs e)
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

        private void fun_load()
        {
            string s = @" select  原ERP物料编号,base.物料名称,n原ERP规格型号,base.规格,大类编号,base.大类,小类编号,base.小类 from 基础数据物料信息表 base
 left join (  select  大类编号,大类,小类编号,小类 from (
 select c.客户编号,c.客户名称 ,COUNT(*) as 订单数,sum(数量) as 数量,SUM(m.税前金额) as 不含税金额,SUM(m.税后金额) as 含税金额
,产品线,g.物料类型编号 as 大类编号,大类,h.物料类型编号 as 小类编号,小类,f.编号,c.片区,c.业务员,员工号  from 销售记录销售订单明细表 m
left  join 销售记录销售订单主表 x  on x.销售订单号 =m.销售订单号
 left join  基础数据物料信息表 b  on b.物料编码=m.物料编码 
 left join  客户基础信息表 c  on c.客户编号 =m.客户编号 
 left join  (select 属性值,属性字段1 as 员工号 from  基础数据基础属性表 where 属性类别='业务员' ) e on e.属性值=c.业务员 
 left join  (select  编号,片区 from 销售片区年度指标对应表 )f on f.片区=c.片区
 left join (select 物料类型编号,物料类型名称 FROM  [基础数据物料类型表] where 类型级别='大类')g on  物料类型名称=大类
 left join (select 物料类型编号,物料类型名称 FROM  [基础数据物料类型表] where 类型级别='小类')h on h.物料类型名称=小类 and left(h.物料类型编号,2)=g.物料类型编号
 where m.生效=1 and x.创建日期 >'2017-1-1'  and  (h.物料类型编号 is   null or  g.物料类型编号 is   null )and m.作废=0 and m.关闭=0 and c.业务员<>'' 
 and x.备注10='' group by 产品线,g.物料类型编号,h.物料类型编号,大类,小类,c.客户编号,c.客户名称 ,f.编号,c.片区,c.业务员,员工号 )xx group by 大类编号,大类,小类编号,小类)x
 on base.小类=x.小类 and base.大类=x.大类
 where base.物料编码  in  (select  物料编码 from 销售记录销售订单明细表  where 生效日期>'2017-1-1')
 and x.大类 is not null   group by 原ERP物料编号,base.物料名称,n原ERP规格型号,base.规格,大类编号,base.大类,小类编号,base.小类  
union 
 select  原ERP物料编号,base.物料名称,n原ERP规格型号,base.规格,大类编号,base.大类,小类编号,base.小类 from 基础数据物料信息表 base
 left join (  select  大类编号,大类,小类编号,小类 from (
select c.客户编号,客户名称,COUNT(*) as 订单数,sum(出库数量) as 数量,SUM(出库数量*税前单价) as 不含税金额,SUM(出库数量*税后单价) as 含税金额
,产品线,g.物料类型编号 as 大类编号,大类,h.物料类型编号 as 小类编号,小类,f.编号,c.片区,c.业务员,员工号  from 销售记录成品出库单明细表 m
 left join  销售记录销售订单明细表 a on a.销售订单明细号=m.销售订单明细号 
 left  join 销售记录销售订单主表 x  on x.销售订单号 =a.销售订单号
 left join  基础数据物料信息表 b  on b.物料编码=m.物料编码 
 left join  客户基础信息表 c on c.客户编号 =m.客户编号 
 left join  (select 属性值,属性字段1 as 员工号 from  基础数据基础属性表 where 属性类别='业务员' ) e on e.属性值=c.业务员 
 left join  (select  编号,片区 from 销售片区年度指标对应表 )f on f.片区=c.片区
left join (select 物料类型编号,物料类型名称 FROM  [基础数据物料类型表] where 类型级别='大类')g on  物料类型名称=大类
left join (select 物料类型编号,物料类型名称 FROM  [基础数据物料类型表] where 类型级别='小类')h on h.物料类型名称=小类 and  left(h.物料类型编号,2)=g.物料类型编号
 where m.生效=1 and m.生效日期>'2017-1-1' and m.生效日期<'2017-11-3' and x.备注10='' and  (h.物料类型编号 is   null or  g.物料类型编号 is   null) and m.作废=0 and a.作废=0 and a.关闭=0 and c.业务员<>''
 group by 产品线,g.物料类型编号,h.物料类型编号,大类,小类,c.客户编号,客户名称,f.编号,c.片区,c.业务员,员工号)x)z
 on base.小类=z.小类 and base.大类=z.大类
 where base.物料编码  in  (select  物料编码 from 销售记录成品出库单明细表  where 生效日期>'2017-1-1')
 and z.大类 is not null   group by 原ERP物料编号,base.物料名称,n原ERP规格型号,base.规格,大类编号,base.大类,小类编号,base.小类 order by 大类";
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(s,strcon);
             da.Fill(dt);
             gridControl1.DataSource = dt;
             s = @"select  销售订单明细号,客户名称,c.客户编号,c.业务员 from 销售记录销售订单明细表 
 left join  客户基础信息表 c  on c.客户编号 =销售记录销售订单明细表.客户编号 
 left  join 销售记录销售订单主表 d on 销售记录销售订单明细表.销售订单号=d.销售订单号
 where c.业务员='' and 销售记录销售订单明细表.生效日期>'2017-1-1' and d.备注10=''";
             da = new SqlDataAdapter(s,strcon);
            DataTable dt1=new DataTable ();
            da.Fill(dt1);
            gridControl2.DataSource = dt1;


            s = @" select  a.*,图纸编号,物料名称  from (select 原ERP物料编号,大类编号,大类,小类编号,小类 from (
  select c.供应商ID,c.供应商名称,b.原ERP物料编号,COUNT(*) as 订单数,sum(采购数量) as 数量,SUM(未税金额) as 不含税金额,SUM(金额) as 含税金额
,g.物料类型编号 as 大类编号,大类,h.物料类型编号 as 小类编号,小类 from 采购记录采购单明细表 m
  left join  基础数据物料信息表 b  on b.物料编码=m.物料编码 
 left join  采购供应商表 c  on c.供应商ID =m.供应商ID 
 left join (select 物料类型编号,物料类型名称 FROM  [基础数据物料类型表] where 类型级别='大类')g on  g.物料类型名称=b.大类
 left join (select 物料类型编号,物料类型名称 FROM  [基础数据物料类型表] where 类型级别='小类')h on h.物料类型名称=b.小类 and  left(h.物料类型编号,2)=g.物料类型编号
  where 生效日期>'2017-1-1'   and  m.生效=1 and  (h.物料类型编号 is   null  or  g.物料类型编号 is  null)
group by  c.供应商ID,c.供应商名称,b.原ERP物料编号,g.物料类型编号,h.物料类型编号,大类,小类)a where a.数量>0 group by 原ERP物料编号,大类编号,大类,小类编号,小类) a
 left  join 基础数据物料信息表 on a.原ERP物料编号=基础数据物料信息表.原ERP物料编号 ";
            da = new SqlDataAdapter(s, strcon);
            DataTable dt2 = new DataTable();
            da.Fill(dt2);
            gridControl3.DataSource = dt2;




        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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

        private void gridView2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView2.GetFocusedRowCellValue(gridView2.FocusedColumn));
                e.Handled = true;
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (ActiveControl.GetType().Equals(gridControl1.GetType()))
            {

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsExportOptions options = new DevExpress.XtraPrinting.XlsExportOptions();
                


                    DevExpress.XtraGrid.GridControl gc = (ActiveControl) as DevExpress.XtraGrid.GridControl;

                    gc.ExportToXlsx(saveFileDialog.FileName);

                  

                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            else
            {

                MessageBox.Show("若要导出请先选中要导出的表格");
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ui手机app不规范数据_Load(null, null);
        }

        private void gridView3_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gridView3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView2.GetFocusedRowCellValue(gridView2.FocusedColumn));
                e.Handled = true;
            }
        }


   
    }
}
