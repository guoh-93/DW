using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CPublic
{
    /// <summary>
    /// 用户对文件操作权限 
    /// </summary>
    public class CFunDesc
    {
        /// <summary>
        /// 文件信息
        /// </summary>
        public Boolean ISinfo
        {
            get;
            internal set;
        }
        /// <summary>
        /// 查看
        /// </summary>
        public Boolean ISView
        {
            get;
            internal set;
        }
        /// <summary>
        /// 打印
        /// </summary>
        public Boolean ISPrint
        {
            get;
            internal set;
        }
        /// <summary>
        /// 新增或修改或删除文件
        /// </summary>
        public Boolean ISWrite
        {
            get;
            internal set;
        }
        /// <summary>
        /// 是否能看到历史版本
        /// </summary>
        public Boolean ISHis
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// 权限控制类
    /// </summary>
    public class CAccessControl
    {
        /// <summary>
        /// 功能权限检查
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="moudleName"></param>
        /// <returns></returns>
        public static Boolean CheckAccessControl(string UID, string moudleName)
        {
            return true;
            DataTable dt = new DataTable();
            string sqlstr = "select * from 用户,权限组权限 where 用户.权限组 = 权限组权限.权限组 and 用户.用户ID = '{0}' and 权限描述 = '{1}'";
            try
            {
                sqlstr = string.Format(sqlstr, UID, moudleName);
                new SqlDataAdapter(sqlstr, Var.strConn).Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return Boolean.Parse(dt.Rows[0]["权限"].ToString());
                }
                else
                {
                    return false;
                }

            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return false;
            }
        }


        /// <summary>
        /// 文件功能检查
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="NodeGUID"></param>
        /// <returns></returns>
        public static CFunDesc CheckFileControl_SN(string UID, string fileSN)
        {
            CFunDesc CD = new CFunDesc();
            CD.ISinfo = false;
            CD.ISPrint = false;
            CD.ISView = false;
            CD.ISWrite = false;
            if (UID == "admin")
            {
                CD.ISinfo = true;
                CD.ISPrint = true;
                CD.ISView = true;
                CD.ISWrite = true;
            }
            else
            {
                DataTable dt = new DataTable();
                string sqlstr = "select 查看权限组.* from 查看权限组,文件仓库文件,用户 where 查看权限组.文件类型 = 文件仓库文件.文件类型 and " +
                    " 用户.部门 = 查看权限组.部门ID and 用户.用户ID = '{0}' and 文件仓库文件.文件编号 = '{1}' and 文件仓库文件.默认版本 = 1 ";
                sqlstr = string.Format(sqlstr, UID, fileSN);
                new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    CD.ISinfo = false;
                    CD.ISPrint = false;
                    CD.ISView = false;
                    CD.ISWrite = false;
                }
                else
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        if (r["权限名称"].ToString() == "查看" && (Boolean)r["权限"])
                        {
                            CD.ISView = true;
                        }
                        if (r["权限名称"].ToString() == "打印" && (Boolean)r["权限"])
                        {
                            CD.ISPrint = true;
                            CD.ISView = true;
                        }
                        if (r["权限名称"].ToString() == "更新" && (Boolean)r["权限"])
                        {
                            CD.ISinfo = true;
                            CD.ISPrint = true;
                            CD.ISView = true;
                            CD.ISWrite = true;
                        }
                    }
                }

            }
            return CD;
        }

        /// <summary>
        /// 文件功能检查
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="NodeGUID"></param>
        /// <returns></returns>
        public static CFunDesc CheckFileControl(string UID, string NodeGUID)
        {
            CFunDesc CD = new CFunDesc();
            CD.ISinfo = false;
            CD.ISPrint = false;
            CD.ISView = false;
            CD.ISWrite = false;
            DataTable d1 = new DataTable();
            new SqlDataAdapter(string.Format("select * from 用户 where 用户ID = '{0}'", UID), CPublic.Var.strConn).Fill(d1);
            if (d1.Rows.Count == 0)
            {
                return CD;
            }
            if (UID == "admin" || d1.Rows[0]["权限组"].ToString() == "文控管理员")
            {
                CD.ISinfo = true;
                CD.ISPrint = true;
                CD.ISView = true;
                CD.ISWrite = true;
            }
            else
            {
                //先查看文件权限表
                DataTable dt = new DataTable();
                string sqlstr = "select 文件仓库文件权限.* from 文件仓库文件权限,文件仓库文件,用户 where 文件仓库文件权限.文件编号 = 文件仓库文件.文件编号 and 文件仓库文件权限.文件版本 = 文件仓库文件.文件版本 and " +
                    " 用户.部门 = 文件仓库文件权限.部门ID and 用户.用户ID = '{0}' and 文件仓库文件.节点GUID = '{1}' and 文件仓库文件.默认版本 = 1";
                sqlstr = string.Format(sqlstr, UID, NodeGUID);
                new SqlDataAdapter(sqlstr, Var.strConn).Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        if (r["权限名称"].ToString() == "查看" && (Boolean)r["权限"])
                        {
                            CD.ISView = true;
                        }
                        if (r["权限名称"].ToString() == "打印" && (Boolean)r["权限"])
                        {
                            CD.ISPrint = true;
                            CD.ISView = true;
                        }
                        if (r["权限名称"].ToString() == "更新" && (Boolean)r["权限"])
                        {
                            CD.ISinfo = true;
                            CD.ISPrint = true;
                            CD.ISView = true;
                            CD.ISWrite = true;
                        }
                    }
                }
                else
                {
                    //如果文件权限里没有数据，就查看权限模板。
                    dt = new DataTable();
                    sqlstr = "select 查看权限组.* from 查看权限组,文件仓库文件,用户 where 查看权限组.文件类型 = 文件仓库文件.文件类型 and " +
                        " 用户.部门 = 查看权限组.部门ID and 用户.用户ID = '{0}' and 文件仓库文件.节点GUID = '{1}' and 文件仓库文件.默认版本 = 1 ";
                    sqlstr = string.Format(sqlstr, UID, NodeGUID);
                    new SqlDataAdapter(sqlstr, Var.strConn).Fill(dt);
                    if (dt.Rows.Count == 0)
                    {
                        CD.ISinfo = false;
                        CD.ISPrint = false;
                        CD.ISView = false;
                        CD.ISWrite = false;
                    }
                    else
                    {
                        foreach (DataRow r in dt.Rows)
                        {
                            if (r["权限名称"].ToString() == "查看" && (Boolean)r["权限"])
                            {
                                CD.ISView = true;
                            }
                            if (r["权限名称"].ToString() == "打印" && (Boolean)r["权限"])
                            {
                                CD.ISPrint = true;
                                CD.ISView = true;
                            }
                            if (r["权限名称"].ToString() == "更新" && (Boolean)r["权限"])
                            {
                                CD.ISinfo = true;
                                CD.ISPrint = true;
                                CD.ISView = true;
                                CD.ISWrite = true;
                            }
                        }
                    }
                }

            }
            return CD;
        }
    }
}
