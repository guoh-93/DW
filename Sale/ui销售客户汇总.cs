using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPSale
{



    public partial class ui销售客户汇总 : UserControl
    {
        #region
        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        #endregion

        public ui销售客户汇总()
        {
            InitializeComponent();
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }  
        private void fun_load()
        {
            DateTime t = CPublic.Var.getDatetime().Date;
            DateTime t1 = new DateTime(t.Year, t.Month, 1);
            dateEdit1.EditValue = t1;
            dateEdit2.EditValue = t;


            string sql = string.Format(@"select 客户编号,客户名称 from 客户基础信息表");
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            DataTable dt_客户 = new DataTable();
            da.Fill(dt_客户);
            searchLookUpEdit1.Properties.DataSource = dt_客户;
            searchLookUpEdit1.Properties.DisplayMember = "客户名称";
            searchLookUpEdit1.Properties.ValueMember = "客户编号";
       
            string sql_片区 = "SELECT [属性值] as 片区 FROM  [基础数据基础属性表] where 属性类别 ='片区'";
            DataTable dt_片区 = new DataTable();
            SqlDataAdapter da_片区 = new SqlDataAdapter(sql_片区, strcon);
            da_片区.Fill(dt_片区);

            comboBox1.DataSource = dt_片区;
            comboBox1.ValueMember = "片区";
            comboBox1.DisplayMember = "片区";
        }

   

        private void fun_check()
        {

            if (checkBox2.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择客户");
                }
            }

            if (checkBox3.Checked == true)
            {
                if (comboBox1.Text == null || comboBox1.Text.ToString() == "")
                {
                    throw new Exception("未选择小类");
                }
            }



        }

        private void fun_search()
        {
            DateTime dt1 = Convert.ToDateTime(dateEdit1.EditValue);
            DateTime dt2 = Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1);

            string sql = string.Format(@"select   base.客户编号,base.客户名称,片区,isnull(订单金额,0)订单金额,isnull(通知金额,0)通知金额,isnull(出库金额,0)出库金额,isnull(退货通知金额,0)退货通知金额,isnull(退货金额,0)退货金额,isnull(开票金额,0)开票金额  from   客户基础信息表 base
left  join (select  客户编号,SUM(税后金额)订单金额 from 销售记录销售订单明细表 where 生效=1 and  关闭=0 and 作废=0 and 生效日期>'{0}' and 生效日期<'{1}'  group by 客户编号 ) a 
 on base.客户编号=a.客户编号
left join (select  st.客户编号,sum(st.出库数量*s.税后单价)通知金额 from 销售记录销售出库通知单明细表 st 
			left join 销售记录销售订单明细表 s on st.销售订单明细号=s.销售订单明细号
           where   st.作废=0  and st.生效日期>'{0}' and st.生效日期<'{1}'    group by st.客户编号) b 
 on  base.客户编号=b.客户编号
left join (select  scm.客户编号,sum(scm.出库数量*s.税后单价)出库金额 from 销售记录成品出库单明细表 scm
		   left join 销售记录销售订单明细表 s on scm.销售订单明细号=s.销售订单明细号
		   left join 销售记录销售订单主表 sz on sz.销售订单号=s.销售订单号 where  scm.生效日期>'{0}' and scm.生效日期<'{1}'  group by scm.客户编号) c  
on base.客户编号=c.客户编号     
left  join (select  tz.客户编号,SUM(tm.税后金额)退货通知金额 from 退货申请子表 tm
			left join  退货申请主表 tz on tm.退货申请单号=tz.退货申请单号  where tm.生效日期>'{0}'and tm.生效日期<'{1}' group by 客户编号) d 
on  base.客户编号=d.客户编号 
left  join (select  ts.客户编号,SUM(t.税后金额)退货金额 from 退货入库子表 t
			left join  退货申请主表 ts on t.退货申请单号=ts.退货申请单号  where ts.生效日期>'{0}' and ts.生效日期<'{1}'   group by 客户编号) e 
on base.客户编号=e.客户编号 
left  join ( select  客户编号,SUM(开票税后金额)开票金额 from  销售记录销售开票主表  where  生效=1  and 开票日期>'{0}' and 开票日期<'{1}'   group  by 客户编号) f
on base.客户编号=f.客户编号 	
where   (订单金额 is not null or 通知金额 is not null or 出库金额 is not null or 退货金额 is not null or 开票金额 is not null)  ", dt1, dt2);
          
            if (checkBox3.Checked == true)
            {
                sql = sql + string.Format(" and base.片区='{0}'", comboBox1.Text);
            }
            if (checkBox2.Checked == true)
            {
                sql = sql + string.Format(" and base.客户编号='{0}'", searchLookUpEdit1.EditValue.ToString());

            }
            if (comboBox2.Text == "销售部")
            {
                sql = sql + string.Format(" and base.客户编号 not in (select  客户编号 from 销售记录销售订单主表 where 备注10<>''  group by 客户编号)");
            }
            else if (comboBox2.Text == "生产部")
            {
                sql = sql + string.Format(" and base.客户编号  in (select  客户编号 from 销售记录销售订单主表 where 备注10<>''  group by 客户编号)");
            }
            sql = sql + " order  by 片区";
          
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
           // DataView dv = new DataView(dtM);
           //dv.RowFilter="订单金额<>0 or 通知金额<>0 or 出库金额<>0";
            gridControl1.DataSource = dtM;

        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_check();
                fun_search();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) 
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void ui销售客户汇总_Load(object sender, EventArgs e)
        {
            fun_load();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;
                gridControl1.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
