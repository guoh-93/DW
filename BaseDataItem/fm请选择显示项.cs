using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace BaseData
{
    public partial class fm请选择显示项 : Form
    {
        #region 成员
        DataTable dt;
        SqlDataAdapter da;
        string strconn = CPublic.Var.strConn;
        public ArrayList arr;
        //public static DataTable dtt;
        #endregion

        #region 界面载入
        public fm请选择显示项()
        {
            InitializeComponent();
        }

        private void fm请选择显示项_Load(object sender, EventArgs e)
        {
            CZMaster.LocalDataSettingBIN.appDesc = "basedateitem";
            arr = new ArrayList(CZMaster.LocalDataSettingBIN.getLocalData("basedateitemchoose"));
            CZMaster.LocalDataSettingBIN.Delete("basedateitemchoose");
            fun_选择();
            fun_显示选择();        
        }
        #endregion

        #region 数据操作
        public void fun_选择()
        {
            dt = new DataTable();
            DataTable t = new DataTable();
            string sql = "select * from 基础数据物料信息表 where 1<>1";
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(t);
            
            dt.Columns.Add("选择项");
            dt.Columns.Add("选择", typeof(Boolean));

            foreach (DataColumn dc in t.Columns)
            {
                dt.Rows.Add(dc.ColumnName,false);
            }
            //dt.Rows.Add("物料编码",false);
            //dt.Rows.Add("物料编码", false);
            //dt.Rows.Add("物料名称", false);
            //dt.Rows.Add("规格型号", false);
            //dt.Rows.Add("图纸编号", false);
            //dt.Rows.Add("n仓库编号", false);
            //dt.Rows.Add("物料类型", false);
            //dt.Rows.Add("n仓库描述", false);
            //dt.Rows.Add("产品线", false);
            //dt.Rows.Add("n销售单价", false);
            //dt.Rows.Add("大类", false);
            //dt.Rows.Add("n核算单价", false);
            //dt.Rows.Add("小类", false);
            //dt.Rows.Add("n原ERP规格型号", false);
            //dt.Rows.Add("规格", false);
            //dt.Rows.Add("物料等级", false);
            //dt.Rows.Add("壳架等级", false);
            //dt.Rows.Add("极数", false);
            //dt.Rows.Add("电压", false);
            //dt.Rows.Add("客户", false);
            //dt.Rows.Add("计量单位", false);
            //dt.Rows.Add("标准单价", false);
            //dt.Rows.Add("库存上限",false);
            //dt.Rows.Add("库存下限",false);
            //dt.Rows.Add("克重", false);
            //dt.Rows.Add("环保", false);
            //dt.Rows.Add("库位编号", false);
            //dt.Rows.Add("库位描述", false);
            //dt.Rows.Add("物料来源", false);
            //dt.Rows.Add("采购周期", false);
            //dt.Rows.Add("默认供应商", false);
            //dt.Rows.Add("标签打印", false);
            //dt.Rows.Add("最小包装", false);
            //dt.Rows.Add("主辅料", false);
            //dt.Rows.Add("停用", false);
            //dt.Rows.Add("关闭", false);
            //dt.Rows.Add("生效", false);
            //dt.Rows.Add("细类", false); 
            //dt.Rows.Add("审核", false);
            //dt.Rows.Add("型号子项", false);
            dataGridView1.DataSource = dt;
        }

        public void fun_显示选择()
        {
            if (arr.Count > 0)
            {
                for (int j = 0; j < arr.Count; j++)          
                { 
                    for(int k =0;k<dt.Rows.Count;k++)
                    {
                        if (dt.Rows[k]["选择项"].ToString() == arr[j].ToString())
                        {
                            dt.Rows[k]["选择"] = true;
                        }
                    }
                }
            }
        }
        #endregion

        #region 界面操作
        private void button1_Click(object sender, EventArgs e)
        {
            //MasterCommon.LocalDataSettingBIN.Delete("basedateitemchoose");
            arr = new ArrayList();            
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["选择"].ToString() == true.ToString())
                {
                    arr.Add(dr["选择项"]);                    
                }
            }
            for (int ii = 0; ii < arr.Count; ii++)
            {
                CZMaster.LocalDataSettingBIN.addLocalData("basedateitemchoose", arr[ii].ToString());
            }
            this.Close();
        }
        #endregion
    }
}
