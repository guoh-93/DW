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
    public partial class fm模具信息 : Form
    {
        string strconn = CPublic.Var.strConn;
        DataTable dtM = new DataTable();
        DataTable dtP = new DataTable();


        public fm模具信息()
        {
            InitializeComponent();
        }

        private void fm模具信息_Load(object sender, EventArgs e)
        {

        }

        private void fun_载入()
        {
            string sql = "select * from 模具管理基础信息表";
            string sql1 = @"select 模具信息(没用).*,基础数据物料信息表.物料编码,基础数据物料信息表.物料名称 from 模具信息(没用) left join 基础数据物料信息表
                on 模具信息(没用).零件图号 = 基础数据物料信息表.图纸编号 where 模具信息(没用).零件图号 <> ''";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            SqlDataAdapter da1 = new SqlDataAdapter(sql1, strconn);
            da.Fill(dtM);
            da1.Fill(dtP);
            gridControl1.DataSource = dtM;
            gridControl2.DataSource = dtP;
        }

        private void fun_计算()
        {
            int i = 0;
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr["零件图号"] == DBNull.Value) continue;
                DataRow r = dtM.NewRow();
                dtM.Rows.Add(r);
                r["GUID"] = System.Guid.NewGuid();
                r["模具编号"] = "MJ" + i++.ToString("0000");
                //if (dr[""] == DBNull.Value)
                //{
                //    dr[""] = "";
                //}
                //r["模具名称"] = dr[""];
                if (dr["产品型号"] == DBNull.Value)
                {
                    dr["产品型号"] = "";
                }
                r["产品型号"] = dr["产品型号"];
                if (dr["零件编号"] == DBNull.Value)
                {
                    dr["零件编号"] = "";
                }
                r["零件编号"] = dr["零件编号"];
                if (dr["零件名称"] == DBNull.Value)
                {
                    dr["零件名称"] = "";
                }
                r["零件名称"] = dr["零件名称"];
                //if (dr["零件图号"] == DBNull.Value)
                //{
                //    dr["零件图号"] = "";
                //}
                r["零件图号"] = dr["零件图号"];
                if (dr["工装编号"] == DBNull.Value)
                {
                    dr["工装编号"] = "";
                }
                r["工装编号"] = dr["工装编号"];
                if (dr["存放库位"] == DBNull.Value)
                {
                    dr["存放库位"] = "";
                }
                r["存放库位"] = dr["存放库位"];
                if (dr["所用零件材料"] == DBNull.Value)
                {
                    dr["所用零件材料"] = "";
                }
                r["所用零件材料"] = dr["所用零件材料"];
                if (dr["使用情况"] == DBNull.Value)
                {
                    dr["使用情况"] = "";
                }
                r["使用情况"] = dr["使用情况"];
                if (dr["材料编号"] == DBNull.Value)
                {
                    dr["材料编号"] = "";
                }
                r["材料编号"] = dr["材料编号"];
                if (dr["材料名称"] == DBNull.Value)
                {
                    dr["材料名称"] = "";
                }
                r["材料名称"] = dr["材料名称"];
                if (dr["穴数"] == DBNull.Value)
                {
                    dr["穴数"] = 0;
                }
                r["穴数"] = dr["穴数"].ToString();
                if (dr["模具设计寿命"] == DBNull.Value)
                {
                    dr["模具设计寿命"] = "";
                }
                r["模具设计寿命"] = dr["模具设计寿命"];
                if (dr["已使用模次"] == DBNull.Value)
                {
                    dr["已使用模次"] = 0;
                }
                r["已使用模次"] = dr["已使用模次"];
                if (dr["主备模"] == DBNull.Value)
                {
                    dr["主备模"] = "";
                }
                r["主备模"] = dr["主备模"];
                if (dr["模具类型"] == DBNull.Value)
                {
                    dr["模具类型"] = "";
                }
                r["模具类型"] = dr["模具类型"];
                if (dr["模具价格"] == DBNull.Value)
                {
                    dr["模具价格"] = 0;
                }
                r["模具价格"] = Convert.ToDecimal(dr["模具价格"]);
                if (dr["入库日期"] == DBNull.Value)
                {
                    dr["入库日期"] = Convert.ToDateTime("1900-01-01");
                }
                r["入库日期"] = dr["入库日期"];
                if (dr["模具制造商"] == DBNull.Value)
                {
                    dr["模具制造商"] = "";
                }
                r["模具制造商"] = dr["模具制造商"];
                if (dr["浇杆重量"] == DBNull.Value)
                {
                    dr["浇杆重量"] = "";
                }
                r["浇杆重量"] = dr["浇杆重量"];
                if (dr["零件重量"] == DBNull.Value)
                {
                    dr["零件重量"] = "";
                }
                r["零件重量"] = dr["零件重量"];
                if (dr["在库状态"] == DBNull.Value)
                {
                    dr["在库状态"] = "";
                }
                r["在库状态"] = dr["在库状态"];
                if (dr["模具属性"] == DBNull.Value)
                {
                    dr["模具属性"] = "";
                }
                r["模具属性"] = dr["模具属性"];
                if (dr["成型周期"] == DBNull.Value)
                {
                    dr["成型周期"] = "";
                }
                r["成型周期"] = dr["成型周期"];
                if (dr["保养属性"] == DBNull.Value)
                {
                    dr["保养属性"] = "";
                }
                r["保养属性"] = dr["保养属性"];
                if (dr["一级保养周期"] == DBNull.Value)
                {
                    dr["一级保养周期"] = "";
                }
                r["一级保养周期"] = dr["一级保养周期"];
                if (dr["一级保养费用"] == DBNull.Value)
                {
                    dr["一级保养费用"] = 0;
                }
                r["一级保养费用"] = Convert.ToDecimal(dr["一级保养费用"]);
                if (dr["二级保养周期"] == DBNull.Value)
                {
                    dr["二级保养周期"] = "";
                }
                r["二级保养周期"] = dr["二级保养周期"];
                if (dr["三级保养周期"] == DBNull.Value)
                {
                    dr["三级保养周期"] = "";
                }
                r["三级保养周期"] = dr["三级保养周期"];

                if (dr["物料编码"] == DBNull.Value)
                {
                    dr["物料编码"] = "";
                }
                r["物料编码"] = dr["物料编码"];
                if (dr["物料名称"] == DBNull.Value)
                {
                    dr["物料名称"] = "";
                }
                r["物料名称"] = dr["物料名称"];
            }
        }

        private void fun_保存()
        {
            string sql = "select * from 模具管理基础信息表 where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dtM);
            MessageBox.Show("保存成功");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fun_载入();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fun_计算();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            fun_保存();
        }
    }
}
