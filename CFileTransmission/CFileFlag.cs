using System;
using System.Collections.Generic;
using System.Text;

namespace CFileTransmission
{
    /// <summary>
    /// 
    /// </summary>
    public class CFileFlag
    {
        public CFileFlag(string fileName,string strState, string strMsg,int timeout)
        {
            FileName = fileName;
            State = strState;
            Msg = strMsg;
            Result = "-1";
            iTimeOut = timeout;
        }
        public CFileFlag()
        {
            State = "";
            Msg = "";
            Result = "-1";
            iTimeOut = 6000;
        }
        public int iTimeOut
        {
            get;
            set;
        }
        public string State
        {
            get;
            set;
        }
        public string Msg
        {
            get;
            set;
        }
        public string Result
        {
            get;
            set;
        }
        public string FileName
        {
            get;
            set;
        }
    }
}
