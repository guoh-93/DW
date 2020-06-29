﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 此源代码是由 Microsoft.VSDesigner 4.0.30319.42000 版自动生成。
// 
#pragma warning disable 1591

namespace WSAdapter.WebReference {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using System.Data;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="DataGENSoap", Namespace="MasterEMSI")]
    public partial class DataGEN : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetDataOperationCompleted;
        
        private System.Threading.SendOrPostCallback SetDataOperationCompleted;
        
        private System.Threading.SendOrPostCallback SetEXSQLOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetData_ERPOperationCompleted;
        
        private System.Threading.SendOrPostCallback SetData_ERPOperationCompleted;
        
        private System.Threading.SendOrPostCallback SetEXSQL_ERPOperationCompleted;
        
        private System.Threading.SendOrPostCallback CheckAuthorityOperationCompleted;
        
        private System.Threading.SendOrPostCallback Host_ConnOperationCompleted;
        
        private System.Threading.SendOrPostCallback Host_CloseOperationCompleted;
        
        private System.Threading.SendOrPostCallback Host_StateOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public DataGEN() {
            this.Url = global::WSAdapter.Properties.Settings.Default.WSAdapter_WebReference_DataGEN;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event GetDataCompletedEventHandler GetDataCompleted;
        
        /// <remarks/>
        public event SetDataCompletedEventHandler SetDataCompleted;
        
        /// <remarks/>
        public event SetEXSQLCompletedEventHandler SetEXSQLCompleted;
        
        /// <remarks/>
        public event GetData_ERPCompletedEventHandler GetData_ERPCompleted;
        
        /// <remarks/>
        public event SetData_ERPCompletedEventHandler SetData_ERPCompleted;
        
        /// <remarks/>
        public event SetEXSQL_ERPCompletedEventHandler SetEXSQL_ERPCompleted;
        
        /// <remarks/>
        public event CheckAuthorityCompletedEventHandler CheckAuthorityCompleted;
        
        /// <remarks/>
        public event Host_ConnCompletedEventHandler Host_ConnCompleted;
        
        /// <remarks/>
        public event Host_CloseCompletedEventHandler Host_CloseCompleted;
        
        /// <remarks/>
        public event Host_StateCompletedEventHandler Host_StateCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("MasterEMSI/GetData", RequestNamespace="MasterEMSI", ResponseNamespace="MasterEMSI", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataTable GetData(string SQL) {
            object[] results = this.Invoke("GetData", new object[] {
                        SQL});
            return ((System.Data.DataTable)(results[0]));
        }
        
        /// <remarks/>
        public void GetDataAsync(string SQL) {
            this.GetDataAsync(SQL, null);
        }
        
        /// <remarks/>
        public void GetDataAsync(string SQL, object userState) {
            if ((this.GetDataOperationCompleted == null)) {
                this.GetDataOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetDataOperationCompleted);
            }
            this.InvokeAsync("GetData", new object[] {
                        SQL}, this.GetDataOperationCompleted, userState);
        }
        
        private void OnGetDataOperationCompleted(object arg) {
            if ((this.GetDataCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetDataCompleted(this, new GetDataCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("MasterEMSI/SetData", RequestNamespace="MasterEMSI", ResponseNamespace="MasterEMSI", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void SetData(string tableName, System.Data.DataTable dt) {
            this.Invoke("SetData", new object[] {
                        tableName,
                        dt});
        }
        
        /// <remarks/>
        public void SetDataAsync(string tableName, System.Data.DataTable dt) {
            this.SetDataAsync(tableName, dt, null);
        }
        
        /// <remarks/>
        public void SetDataAsync(string tableName, System.Data.DataTable dt, object userState) {
            if ((this.SetDataOperationCompleted == null)) {
                this.SetDataOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSetDataOperationCompleted);
            }
            this.InvokeAsync("SetData", new object[] {
                        tableName,
                        dt}, this.SetDataOperationCompleted, userState);
        }
        
        private void OnSetDataOperationCompleted(object arg) {
            if ((this.SetDataCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SetDataCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("MasterEMSI/SetEXSQL", RequestNamespace="MasterEMSI", ResponseNamespace="MasterEMSI", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int SetEXSQL(string SQL) {
            object[] results = this.Invoke("SetEXSQL", new object[] {
                        SQL});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void SetEXSQLAsync(string SQL) {
            this.SetEXSQLAsync(SQL, null);
        }
        
        /// <remarks/>
        public void SetEXSQLAsync(string SQL, object userState) {
            if ((this.SetEXSQLOperationCompleted == null)) {
                this.SetEXSQLOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSetEXSQLOperationCompleted);
            }
            this.InvokeAsync("SetEXSQL", new object[] {
                        SQL}, this.SetEXSQLOperationCompleted, userState);
        }
        
        private void OnSetEXSQLOperationCompleted(object arg) {
            if ((this.SetEXSQLCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SetEXSQLCompleted(this, new SetEXSQLCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("MasterEMSI/GetData_ERP", RequestNamespace="MasterEMSI", ResponseNamespace="MasterEMSI", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataTable GetData_ERP(string SQL) {
            object[] results = this.Invoke("GetData_ERP", new object[] {
                        SQL});
            return ((System.Data.DataTable)(results[0]));
        }
        
        /// <remarks/>
        public void GetData_ERPAsync(string SQL) {
            this.GetData_ERPAsync(SQL, null);
        }
        
        /// <remarks/>
        public void GetData_ERPAsync(string SQL, object userState) {
            if ((this.GetData_ERPOperationCompleted == null)) {
                this.GetData_ERPOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetData_ERPOperationCompleted);
            }
            this.InvokeAsync("GetData_ERP", new object[] {
                        SQL}, this.GetData_ERPOperationCompleted, userState);
        }
        
        private void OnGetData_ERPOperationCompleted(object arg) {
            if ((this.GetData_ERPCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetData_ERPCompleted(this, new GetData_ERPCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("MasterEMSI/SetData_ERP", RequestNamespace="MasterEMSI", ResponseNamespace="MasterEMSI", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void SetData_ERP(string tableName, System.Data.DataTable dt) {
            this.Invoke("SetData_ERP", new object[] {
                        tableName,
                        dt});
        }
        
        /// <remarks/>
        public void SetData_ERPAsync(string tableName, System.Data.DataTable dt) {
            this.SetData_ERPAsync(tableName, dt, null);
        }
        
        /// <remarks/>
        public void SetData_ERPAsync(string tableName, System.Data.DataTable dt, object userState) {
            if ((this.SetData_ERPOperationCompleted == null)) {
                this.SetData_ERPOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSetData_ERPOperationCompleted);
            }
            this.InvokeAsync("SetData_ERP", new object[] {
                        tableName,
                        dt}, this.SetData_ERPOperationCompleted, userState);
        }
        
        private void OnSetData_ERPOperationCompleted(object arg) {
            if ((this.SetData_ERPCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SetData_ERPCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("MasterEMSI/SetEXSQL_ERP", RequestNamespace="MasterEMSI", ResponseNamespace="MasterEMSI", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int SetEXSQL_ERP(string SQL) {
            object[] results = this.Invoke("SetEXSQL_ERP", new object[] {
                        SQL});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void SetEXSQL_ERPAsync(string SQL) {
            this.SetEXSQL_ERPAsync(SQL, null);
        }
        
        /// <remarks/>
        public void SetEXSQL_ERPAsync(string SQL, object userState) {
            if ((this.SetEXSQL_ERPOperationCompleted == null)) {
                this.SetEXSQL_ERPOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSetEXSQL_ERPOperationCompleted);
            }
            this.InvokeAsync("SetEXSQL_ERP", new object[] {
                        SQL}, this.SetEXSQL_ERPOperationCompleted, userState);
        }
        
        private void OnSetEXSQL_ERPOperationCompleted(object arg) {
            if ((this.SetEXSQL_ERPCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SetEXSQL_ERPCompleted(this, new SetEXSQL_ERPCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("MasterEMSI/CheckAuthority", RequestNamespace="MasterEMSI", ResponseNamespace="MasterEMSI", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool CheckAuthority(string UID, string AuthorityDesc) {
            object[] results = this.Invoke("CheckAuthority", new object[] {
                        UID,
                        AuthorityDesc});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void CheckAuthorityAsync(string UID, string AuthorityDesc) {
            this.CheckAuthorityAsync(UID, AuthorityDesc, null);
        }
        
        /// <remarks/>
        public void CheckAuthorityAsync(string UID, string AuthorityDesc, object userState) {
            if ((this.CheckAuthorityOperationCompleted == null)) {
                this.CheckAuthorityOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCheckAuthorityOperationCompleted);
            }
            this.InvokeAsync("CheckAuthority", new object[] {
                        UID,
                        AuthorityDesc}, this.CheckAuthorityOperationCompleted, userState);
        }
        
        private void OnCheckAuthorityOperationCompleted(object arg) {
            if ((this.CheckAuthorityCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CheckAuthorityCompleted(this, new CheckAuthorityCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("MasterEMSI/Host_Conn", RequestNamespace="MasterEMSI", ResponseNamespace="MasterEMSI", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void Host_Conn(string HostName, int iPort) {
            this.Invoke("Host_Conn", new object[] {
                        HostName,
                        iPort});
        }
        
        /// <remarks/>
        public void Host_ConnAsync(string HostName, int iPort) {
            this.Host_ConnAsync(HostName, iPort, null);
        }
        
        /// <remarks/>
        public void Host_ConnAsync(string HostName, int iPort, object userState) {
            if ((this.Host_ConnOperationCompleted == null)) {
                this.Host_ConnOperationCompleted = new System.Threading.SendOrPostCallback(this.OnHost_ConnOperationCompleted);
            }
            this.InvokeAsync("Host_Conn", new object[] {
                        HostName,
                        iPort}, this.Host_ConnOperationCompleted, userState);
        }
        
        private void OnHost_ConnOperationCompleted(object arg) {
            if ((this.Host_ConnCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Host_ConnCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("MasterEMSI/Host_Close", RequestNamespace="MasterEMSI", ResponseNamespace="MasterEMSI", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void Host_Close(string HostName) {
            this.Invoke("Host_Close", new object[] {
                        HostName});
        }
        
        /// <remarks/>
        public void Host_CloseAsync(string HostName) {
            this.Host_CloseAsync(HostName, null);
        }
        
        /// <remarks/>
        public void Host_CloseAsync(string HostName, object userState) {
            if ((this.Host_CloseOperationCompleted == null)) {
                this.Host_CloseOperationCompleted = new System.Threading.SendOrPostCallback(this.OnHost_CloseOperationCompleted);
            }
            this.InvokeAsync("Host_Close", new object[] {
                        HostName}, this.Host_CloseOperationCompleted, userState);
        }
        
        private void OnHost_CloseOperationCompleted(object arg) {
            if ((this.Host_CloseCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Host_CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("MasterEMSI/Host_State", RequestNamespace="MasterEMSI", ResponseNamespace="MasterEMSI", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string Host_State(string HostName) {
            object[] results = this.Invoke("Host_State", new object[] {
                        HostName});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void Host_StateAsync(string HostName) {
            this.Host_StateAsync(HostName, null);
        }
        
        /// <remarks/>
        public void Host_StateAsync(string HostName, object userState) {
            if ((this.Host_StateOperationCompleted == null)) {
                this.Host_StateOperationCompleted = new System.Threading.SendOrPostCallback(this.OnHost_StateOperationCompleted);
            }
            this.InvokeAsync("Host_State", new object[] {
                        HostName}, this.Host_StateOperationCompleted, userState);
        }
        
        private void OnHost_StateOperationCompleted(object arg) {
            if ((this.Host_StateCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Host_StateCompleted(this, new Host_StateCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void GetDataCompletedEventHandler(object sender, GetDataCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetDataCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetDataCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public System.Data.DataTable Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.Data.DataTable)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void SetDataCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void SetEXSQLCompletedEventHandler(object sender, SetEXSQLCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SetEXSQLCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SetEXSQLCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void GetData_ERPCompletedEventHandler(object sender, GetData_ERPCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetData_ERPCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetData_ERPCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public System.Data.DataTable Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.Data.DataTable)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void SetData_ERPCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void SetEXSQL_ERPCompletedEventHandler(object sender, SetEXSQL_ERPCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SetEXSQL_ERPCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SetEXSQL_ERPCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void CheckAuthorityCompletedEventHandler(object sender, CheckAuthorityCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CheckAuthorityCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CheckAuthorityCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void Host_ConnCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void Host_CloseCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void Host_StateCompletedEventHandler(object sender, Host_StateCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Host_StateCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Host_StateCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591