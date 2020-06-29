using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace CZMaster
{
    /// <summary>
    /// 封装了SQL简化操作
    /// </summary>
    public static class MasterSQL
    {

        #region 其它全局变量_路径
        static string baseDir = "";

        /// <summary>
        /// 得到当前路径
        /// </summary>
        public static string BaseDir
        {
            get
            {
                if (baseDir == "")
                {
                    baseDir = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                }
                return baseDir;
            }

        }

        /// <summary>
        /// 得到当前某路径，并可以设置是否要消除当前路径中的所有文件
        /// </summary>
        /// <param name="dir">要追加的路径</param>
        /// <param name="blClear">是否要清除</param>
        /// <returns></returns>
        public static string GetCurrCoustDir(string dir, Boolean blClear = true)
        {
            dir = BaseDir + "\\" + dir;
            if (blClear)
            {
                try
                {
                    System.IO.Directory.Delete(dir, true);

                }
                catch { }
            }
            try
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            catch { }
            return dir;
        }

        #endregion


        static List<string> LiFlag = new List<string>();

        /// <summary>
        /// 通过SQL填充DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="conn"></param>
        /// <param name="dt"></param>
        public static void Get_DataTable(string sql, SqlTransaction tr, DataTable dt)
        {
            try
            {
                lock (LiFlag)
                {
                    using (SqlCommand cmd = new SqlCommand(sql, tr.Connection, tr))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    //using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                    //{
                    //    da.Fill(dt);
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 通过SQL填充DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="conn"></param>
        /// <param name="dt"></param>
        public static void Get_DataTable(string sql, string conn, DataTable dt)
        {
            try
            {
                lock (LiFlag)
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 通过SQL得到DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static DataTable Get_DataTable(string sql, SqlTransaction tr)
        {
            try
            {

                DataTable dt = new DataTable();
                Get_DataTable(sql, tr, dt);
                return dt;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 通过SQL得到DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static DataTable Get_DataTable(string sql, string conn)
        {
            try
            {
                DataTable dt = new DataTable();
                Get_DataTable(sql, conn, dt);
                return dt;
            }
            catch (Exception ex)
            {
                        throw ex;
            }
        }
        /// <summary>
        /// 得到datarow
        /// 无返回的为null
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static DataRow Get_DataRow(string sql, string conn)
        {
            try
            {
                lock (LiFlag)
                {
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                    {
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            return dt.Rows[0];
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 快速保存数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="tableName"></param>
        public static void Save_DataTable(DataTable dt, string tableName, string conn)
        {
            try
            {
                lock (LiFlag)
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(string.Format("select * from {0} where 1<>1", tableName), conn))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 快速保存数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="tableName"></param>
        public static void Save_DataTable(DataTable dt, string tableName, SqlTransaction tr)
        {
            try
            {
                lock (LiFlag)
                {

                    using (SqlCommand cmd = new SqlCommand(string.Format("select * from {0} where 1<>1", tableName), tr.Connection, tr))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            new SqlCommandBuilder(da);
                            da.Fill(dt);
                        }
                    }

                    //using (SqlDataAdapter da = new SqlDataAdapter(string.Format("select * from {0} where 1<>1", tableName), conn))
                    //{
                    //    new SqlCommandBuilder(da);
                    //    da.Update(dt);
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int ExecuteSQL(string sql, string conn)
        {
            try
            {
                using (SqlConnection sconn = new SqlConnection(conn))
                {
                    sconn.Open();
                    SqlCommand cmd = new SqlCommand(sql, sconn);
                   
                    return cmd.ExecuteNonQuery();
                   
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
