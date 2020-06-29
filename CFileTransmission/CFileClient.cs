using System;
using System.Collections.Generic;
using System.Text;
//using CSharpWin;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

namespace CFileTransmission
{

    public class CFileClient
    {

        public static string strCONN = "";
        const string strSplite = "||||";
        public static int msgTimeout = 6000;
        public static int iDalyStep = 500;
        static Boolean blDelay = false;
        public static int Downloadfile_count = 0;
        static Thread thwork;
        static DataTable dt_Main;
        static Boolean bl_countine = false;

        //public static string strCONN = "";
        //const string  strSplite  = "||||";
        //public static int msgTimeout = 6000;
        //public static int iDalyStep = 500;

        #region 工具函数
        private static void fun_单行数据赋值(DataRow r, object[] dataC)
        {
            for (int i = 0; i < dataC.Length / 2; i++)
            {
                r[dataC[i * 2].ToString()] = dataC[i * 2 + 1];
            }
        }
        #endregion

        #region 删除及替换文件
        /// <summary>
        /// 删除目标文件
        /// </summary>
        /// <param name="remoteFile">目标全路径</param>
        /// <returns>第一节0，成功，1，文件不存在，2，文件不能删除，其它错误我们直接抛出错误</returns>
        public static  int deleteFile(string remoteFile)
        {
            try
            {
                string iGUID = System.Guid.NewGuid().ToString();
                DataTable dt = new DataTable();
                Random ran = new Random();
                int rand_i = ran.Next(1,5);
                string sql = string.Format("select * from FCS{1} where iGUID = '{0}'", iGUID,rand_i);
                SqlDataAdapter da = new SqlDataAdapter(sql, strCONN);
                new SqlCommandBuilder(da);
                da.Fill(dt);
                dt.Rows.Add(iGUID, "删除", -1, "", System.DateTime.Now, DBNull.Value, remoteFile);
                da.Update(dt);
                
                sql = string.Format("select * from FCS{1} where iGUID = '{0}' and 请求结果 <> -1", iGUID,rand_i);
                int iStep = 0;
                try
                {
                    while (iStep <= msgTimeout)
                    {
                        dt.Clear();
                        new SqlDataAdapter(sql, strCONN).Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            return (int)dt.Rows[0]["请求结果"];
                        }
                        System.Threading.Thread.Sleep(iDalyStep);
                        iStep += iDalyStep;
                    }
                    throw new Exception("服务器无响应，超时");
                }
                finally
                {
                    if (dt.Rows.Count > 0)
                    {
                        dt.Rows[0].Delete();
                        da.Update(dt);
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public static int deleteFile_p(string GUID)
        {
            try
            {
                DataTable dt_文件路径索引 = new DataTable();
                string sql1 = string.Format("select * from 文件路径索引 where GUID='{0}'", GUID);
                SqlDataAdapter da1 = new SqlDataAdapter(sql1, strCONN);
                new SqlCommandBuilder(da1);
                da1.Fill(dt_文件路径索引);
                string remoteFile = dt_文件路径索引.Rows[0]["文件路径"].ToString();
                string iGUID = System.Guid.NewGuid().ToString();
                DataTable dt = new DataTable();
                string sql = string.Format("select * from FCS where iGUID = '{0}'", iGUID);
                SqlDataAdapter da = new SqlDataAdapter(sql, strCONN);
                new SqlCommandBuilder(da);
                da.Fill(dt);
                dt.Rows.Add(iGUID, "删除P", -1, "", System.DateTime.Now, DBNull.Value, remoteFile);
                da.Update(dt);

                sql = string.Format("select * from FCS where iGUID = '{0}' and 请求结果 <> -1", iGUID);
                int iStep = 0;
                try
                {
                    while (iStep <= msgTimeout)
                    {
                        dt.Clear();
                        new SqlDataAdapter(sql, strCONN).Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            dt_文件路径索引.Rows[0].Delete();
                            da1.Update(dt_文件路径索引);
                            return (int)dt.Rows[0]["请求结果"];
                        }
                        System.Threading.Thread.Sleep(iDalyStep);
                        iStep += iDalyStep;
                    }
                    throw new Exception("服务器无响应，超时");
                }
                finally
                {
                    if (dt.Rows.Count > 0)
                    {
                        dt.Rows[0].Delete();
                        da.Update(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public string[] deleteFile(string remoteFile)
        //{
        //    string[] strMsgs = null;
        //    if (udpSendFile == null || peerMsg == null || udpReceiveFile == null)
        //    {
        //        throw new Exception("本地服务没有初始化");
        //    }
        //    string str1 = "deleteFile" + strSplite + remoteFile;
        //    SendCell msgCell = new SendCell(0, str1);

        //    CFileFlag cmw = new CFileFlag(remoteFile, "删除文件", "", msgTimeout);
        //    lock (ALmsgWait)
        //    {
        //        ALmsgWait.Add(remoteFile, cmw);
        //    }
        //    peerMsg.Send(msgCell, remoteMsgIPP);
        //    waitServer(remoteFile);
        //    if (cmw.Result != "0")
        //    {
        //        strMsgs = new string[] {"error","文件：" + cmw.FileName + "," + cmw.Msg};
        //        //throw new Exception("文件：" + cmw.FileName + "," + cmw.Msg);
        //    }
        //    else
        //    {
        //        strMsgs = new string[] { "ok" };
        //    }
        //    return strMsgs;
        //}
        #endregion

        #region 发送
        public static string sendFile(string filePath)
        {
            try
            {
                byte[] bts = System.IO.File.ReadAllBytes(filePath);
                string iGUID = System.Guid.NewGuid().ToString();
                DataTable dt = new DataTable();
                Random ran=new Random();
                int rand_i = ran.Next(1, 5);
                string sql = string.Format("select * from FCS{1} where iGUID = '{0}'", iGUID,rand_i);
                SqlDataAdapter da = new SqlDataAdapter(sql, strCONN);
                new SqlCommandBuilder(da);
                da.Fill(dt);
                DataRow r =  dt.Rows.Add(iGUID, "上传", -1, "", System.DateTime.Now, DBNull.Value, "");
                r["文件数据"] = bts;
                da.Update(dt);

                sql = string.Format("select * from FCS{1} where iGUID ='{0}' and 请求结果 <> -1", iGUID,rand_i);
                int iStep = 0;
                try
                {
                    while (iStep <= msgTimeout)
                    {
                        dt.Clear();
                        new SqlDataAdapter(sql, strCONN).Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            if ((int)dt.Rows[0]["请求结果"] == 0)
                            {
                                return dt.Rows[0]["文件路径"].ToString();
                            }
                            else
                            {
                                throw new Exception(dt.Rows[0]["errDesc"].ToString());
                            }
                        }
                        System.Threading.Thread.Sleep(iDalyStep);
                        iStep += iDalyStep;
                    }
                    throw new Exception("服务器无响应，超时");
                }
                finally
                {
                    if (dt.Rows.Count > 0)
                    {
                        dt.Rows[0].Delete();
                        da.Update(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public string[] sendFile(string filePath)
        //{
        //    string[] strMsgs;
        //    if (udpSendFile == null || peerMsg == null || udpReceiveFile == null)
        //    {
        //        throw new Exception("本地服务没有初始化");
        //    }
        //    if (System.IO.File.Exists(filePath) == false)
        //    {
        //        throw new Exception("文件不存在");
        //    }
        //    string strMD5 = CSharpWin.MD5Helper.CretaeMD5(filePath);
        //    string str1 = "RequestReceiveFile" + strSplite + strMD5;

        //    ///开始发送消息
        //    SendCell msgCell = new SendCell(0, str1);
        //    //发送之前。先把数据入入结构体
        //    CFileFlag cmw = new CFileFlag(filePath, "发送申请状态", "", msgTimeout);
        //    lock (ALmsgWait)
        //    {
        //        ALmsgWait.Add(strMD5, cmw);
        //    }
        //    peerMsg.Send(msgCell, remoteMsgIPP);
        //    //等待并得到结果
        //    waitServer(strMD5);
        //    if (cmw.Result != "0")
        //    {
        //        throw new Exception("文件：" + cmw.FileName + "," + cmw.Msg);
        //    }
        //    strMsgs = cmw.Msg.Split(new string[] { strSplite }, StringSplitOptions.RemoveEmptyEntries);

        //    cmw = new CFileFlag(filePath, "文件发送状态", "", msgTimeout);
        //    lock (ALmsgWait)
        //    {
        //        ALmsgWait.Add(strMD5, cmw);
        //    }
        //    udpSendFile.SendFile(filePath);
        //    waitServer(strMD5);
        //    return strMsgs;
        //}

        public static string sendFile_p(string filePath)
        {
            try
            {

                byte[] bts = System.IO.File.ReadAllBytes(filePath);
                string iGUID = System.Guid.NewGuid().ToString();
                DataTable dt = new DataTable();
                string sql = string.Format("select * from FCS where iGUID = '{0}'", iGUID);
                SqlDataAdapter da = new SqlDataAdapter(sql, strCONN);
                new SqlCommandBuilder(da);
                da.Fill(dt);
                DataRow r = dt.Rows.Add(iGUID, "上传P", -1, "", System.DateTime.Now, DBNull.Value, "");
                r["文件数据"] = bts;
                da.Update(dt);

                sql = string.Format("select * from FCS where iGUID = '{0}' and 请求结果 <> -1", iGUID);
                string sql1 = "select * from 文件路径索引";
                int iStep = 0;
                try
                {
                    while (iStep <= msgTimeout)
                    {
                        dt.Clear();
                        new SqlDataAdapter(sql, strCONN).Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            if ((int)dt.Rows[0]["请求结果"] == 0)
                            {
                                DataTable dt_文件路径索引 = new DataTable();
                                SqlDataAdapter da1 = new SqlDataAdapter(sql1, strCONN);
                                new SqlCommandBuilder(da1);
                                da1.Fill(dt_文件路径索引);
                                dt_文件路径索引.Rows.Add(iGUID, dt.Rows[0]["文件路径"].ToString());
                                //DataRow r1 = dt.Rows.Add(iGUID, dt.Rows[0]["文件路径"].ToString());
                                da1.Update(dt_文件路径索引);
                                return iGUID;
                            }
                            else
                            {
                                throw new Exception(dt.Rows[0]["errDesc"].ToString());
                            }
                        }
                        System.Threading.Thread.Sleep(iDalyStep);
                        iStep += iDalyStep;
                    }
                    throw new Exception("服务器无响应，超时");
                }
                finally
                {
                    if (dt.Rows.Count > 0)
                    {
                        dt.Rows[0].Delete();
                        da.Update(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 接收
        public static int Receiver(string remoteFile, string downFile)
        {
            try
            {
                if(! Directory.Exists(Path.GetDirectoryName(downFile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(downFile));
                }
                string iGUID = System.Guid.NewGuid().ToString();
                DataTable dt = new DataTable();
                Random ran = new Random();
                int rand_i = ran.Next(1, 5);
                string sql = string.Format("select * from FCS{1} where iGUID = '{0}'", iGUID,rand_i);
                SqlDataAdapter da = new SqlDataAdapter(sql, strCONN);
                new SqlCommandBuilder(da);
                da.Fill(dt);
                DataRow r = dt.Rows.Add(iGUID, "下载", -1, "", System.DateTime.Now, DBNull.Value, remoteFile);
                da.Update(dt);

                sql = string.Format("select * from FCS{1} where iGUID = '{0}' and 请求结果 <> -1", iGUID,rand_i);
                int iStep = 0;
                try
                {
                    while (iStep <= msgTimeout)
                    {
                        dt.Clear();
                        new SqlDataAdapter(sql, strCONN).Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            if ((int)dt.Rows[0]["请求结果"] == 0)
                            {
                                File.WriteAllBytes(downFile, (byte[])dt.Rows[0]["文件数据"]);
                                return (int)dt.Rows[0]["请求结果"];
                            }
                            else
                            {
                                throw new Exception(dt.Rows[0]["errDesc"].ToString());
                            }
                        }
                        System.Threading.Thread.Sleep(iDalyStep);
                        iStep += iDalyStep;
                    }
                    throw new Exception("服务器无响应，超时");
                }
                finally
                {
                    if (dt.Rows.Count > 0)
                    {
                        dt.Rows[0].Delete();
                        da.Update(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int Receiver_p(string GUID, string downFile)
        {
            try
            {
                DataTable dt_文件路径索引 = new DataTable();
                string sql1 = string.Format("select * from 文件路径索引 where GUID='{0}'", GUID);
                SqlDataAdapter da1 = new SqlDataAdapter(sql1, strCONN);
                new SqlCommandBuilder(da1);
                da1.Fill(dt_文件路径索引);
                string remoteFile = dt_文件路径索引.Rows[0]["文件路径"].ToString();
                if (!Directory.Exists(Path.GetDirectoryName(downFile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(downFile));
                }
                string iGUID = System.Guid.NewGuid().ToString();
                DataTable dt = new DataTable();
                string sql = string.Format("select * from FCS where iGUID = '{0}'", iGUID);
                SqlDataAdapter da = new SqlDataAdapter(sql, strCONN);
                new SqlCommandBuilder(da);
                da.Fill(dt);
                DataRow r = dt.Rows.Add(iGUID, "下载P", -1, "", System.DateTime.Now, DBNull.Value, remoteFile);
                da.Update(dt);

                sql = string.Format("select * from FCS where iGUID = '{0}' and 请求结果 <> -1", iGUID);
                int iStep = 0;
                try
                {
                    while (iStep <= msgTimeout)
                    {
                        dt.Clear();
                        new SqlDataAdapter(sql, strCONN).Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            if ((int)dt.Rows[0]["请求结果"] == 0)
                            {
                                File.WriteAllBytes(downFile, (byte[])dt.Rows[0]["文件数据"]);
                                return (int)dt.Rows[0]["请求结果"];
                            }
                            else
                            {
                                throw new Exception(dt.Rows[0]["errDesc"].ToString());
                            }
                        }
                        System.Threading.Thread.Sleep(iDalyStep);
                        iStep += iDalyStep;
                    }
                    throw new Exception("服务器无响应，超时");
                }
                finally
                {
                    if (dt.Rows.Count > 0)
                    {
                        dt.Rows[0].Delete();
                        da.Update(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static int Receiver_fileTeam_p(DataTable dt_M)
        {
            if (blDelay) return 1;
            dt_Main = new DataTable();
            dt_Main = dt_M;
            blDelay = true;
            Downloadfile_count = 0;
            bl_countine = false;
            thwork = new Thread(Work_download);
            thwork.Start();
            return 0;

        }
        static void Work_download()
        {
            try
            {
                foreach (DataRow dr in dt_Main.Rows)
                {
                    if (bl_countine)
                        return;
                    Boolean bd = true;
                    DataTable dt_文件路径索引 = new DataTable();
                    string sql1 = string.Format("select * from 文件路径索引 where GUID='{0}'", dr["GUID"].ToString());
                    SqlDataAdapter da1 = new SqlDataAdapter(sql1, strCONN);
                    new SqlCommandBuilder(da1);
                    da1.Fill(dt_文件路径索引);
                    string remoteFile = dt_文件路径索引.Rows[0]["文件路径"].ToString();

                    if (!Directory.Exists(Path.GetDirectoryName(dr["文件路径"].ToString())))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(dr["文件路径"].ToString()));
                    }

                    string iGUID = System.Guid.NewGuid().ToString();
                    DataTable dt = new DataTable();
                    string sql = string.Format("select * from FCS where iGUID = '{0}'", iGUID);
                    SqlDataAdapter da = new SqlDataAdapter(sql, strCONN);
                    new SqlCommandBuilder(da);
                    da.Fill(dt);
                    DataRow r = dt.Rows.Add(iGUID, "下载P", -1, "", System.DateTime.Now, DBNull.Value, remoteFile);
                    da.Update(dt);

                    sql = string.Format("select * from FCS where iGUID = '{0}' and 请求结果 <> -1", iGUID);
                    int iStep = 0;
                    try
                    {
                        while (iStep <= msgTimeout && bd == true)
                        {
                            dt.Clear();
                            new SqlDataAdapter(sql, strCONN).Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                if ((int)dt.Rows[0]["请求结果"] == 0)
                                {
                                    File.WriteAllBytes(dr["文件路径"].ToString(), (byte[])dt.Rows[0]["文件数据"]);
                                    Downloadfile_count++;
                                    bd = false;
                                }
                                else
                                {
                                    throw new Exception(dt.Rows[0]["errDesc"].ToString());
                                }
                            }
                            System.Threading.Thread.Sleep(iDalyStep);
                            iStep += iDalyStep;
                        }
                        if (bd == true)
                        {
                            throw new Exception("服务器无响应，超时");
                        }
                    }
                    finally
                    {
                        if (dt.Rows.Count > 0)
                        {
                            dt.Rows[0].Delete();
                            da.Update(dt);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                blDelay = false;
            }
        }

        public static void stop_download()
        {
            try
            {
                bl_countine = true;
                blDelay = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

    }
}
