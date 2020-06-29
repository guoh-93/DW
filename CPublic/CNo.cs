using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace CPublic
{

    /// <summary>
    /// 单据流水号控制函数
    /// </summary>
    public static class CNo
    {
        /// <summary>
        /// 查询当前流水号
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="Y"></param>
        /// <param name="M"></param>
        /// <returns></returns>
        public static int fun_查询当时流水号(string strType,int Y,int M)
        {
            string sql;
            sql = "select * from 单据流水控制表 where 单据类型 = '{0}' and 年 = '{1}' and 月 = '{2}'";
            sql = string.Format(sql, strType, Y, M);
            SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count  == 0)
            {
                return 0;
            }
            else
            {
                return (int)dt.Rows[0]["流水"];
            }
        }


        /// <summary>
        /// 得到最大流水号，流水自动加1,每年重置
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public static int fun_得到最大流水号(string strType, int Y)
        {
            string sql;       
            sql = string.Format("select * from 单据流水控制表 where 单据类型 = '{0}' and 年 = '{1}' and 月 =0 and 日=0",strType,Y);
           
            SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn);
            new SqlCommandBuilder(da);
            DataTable dt = new DataTable();
            da.Fill(dt);
            int ir = 0;
            if (dt.Rows.Count == 0)
            {
                ir = 1;
                dt.Rows.Add(new object[] { strType, Y.ToString(),0, 1 }); //单据类型，年，月，流水号
            }
            else
            {
                dt.Rows[0]["流水"] = (int)dt.Rows[0]["流水"] + 1;
                ir = (int)dt.Rows[0]["流水"];
            }
            try
            {
                da.Update(dt);
                return ir;
            }
            catch
            {
                return 0;
            }
        }



        /// <summary>
        /// 得到最大流水号，流水自动加1,每月重置
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="Y"></param>
        /// <param name="M"></param>
        /// <returns></returns>
        public static int fun_得到最大流水号(string strType, int Y, int M)
        {
            string sql;
            sql = "select * from 单据流水控制表 where 单据类型 = '{0}' and 年 = '{1}' and 月 = '{2}' and 日=0";
            sql = string.Format(sql, strType, Y, M);
            SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn);
            new SqlCommandBuilder(da);
            DataTable dt = new DataTable();
            da.Fill(dt);
            int ir = 0;
            if (dt.Rows.Count == 0)
            {
                ir = 1;
                dt.Rows.Add(new object[] { strType, Y.ToString(), M.ToString(), 1 });
            }
            else
            {
                dt.Rows[0]["流水"] = (int)dt.Rows[0]["流水"] + 1;
                ir = (int)dt.Rows[0]["流水"];
            }
            try
            {
                da.Update(dt);
                return ir;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 每天流水号从1开始,每日重置
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="Y"></param>
        /// <param name="M"></param>
        /// <param name="D"></param>
        /// <returns></returns>
        public static int fun_得到最大流水号(string strType, int Y, int M, int D)
        {
            string sql;
            sql = "select * from 单据流水控制表 where 单据类型 = '{0}' and 年 = '{1}' and 月 = '{2}' and 日='{3}'";
            sql = string.Format(sql, strType, Y, M, D);
            SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn);
            new SqlCommandBuilder(da);
            DataTable dt = new DataTable();
            da.Fill(dt);
            int ir = 0;
            if (dt.Rows.Count == 0)
            {
                ir = 1;
                dt.Rows.Add(new object[] { strType, Y.ToString(), M.ToString(), 1, D.ToString() });
            }
            else
            {
                dt.Rows[0]["流水"] = (int)dt.Rows[0]["流水"] + 1;
                ir = (int)dt.Rows[0]["流水"];
            }
            try
            {
                da.Update(dt);
                return ir;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 单据类型 永久叠加 品质生成检验标准 文件号
        /// </summary>
        /// <param name="strType"></param>
        /// <returns></returns>
        public static int fun_得到最大流水号(string strType)
        {
            string sql;
            sql = "select * from 单据流水控制表 where 单据类型 = '{0}'";
            sql = string.Format(sql, strType);
            SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn);
            new SqlCommandBuilder(da);
            DataTable dt = new DataTable();
            da.Fill(dt);
            int ir = 0;
            if (dt.Rows.Count == 0)
            {
                ir = 1;
                dt.Rows.Add(new object[] { strType});
                dt.Rows[0]["流水"] = 1;
            }
            else
            {
                dt.Rows[0]["流水"] = (int)dt.Rows[0]["流水"] + 1;
                ir = (int)dt.Rows[0]["流水"];
            }
            try
            {
                da.Update(dt);
                return ir;
            }
            catch
            {
                return 0;
            }
        }
    }
}
