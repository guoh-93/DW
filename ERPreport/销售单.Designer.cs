namespace ERPreport
{
    partial class 销售单
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource2 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.DataTable销售单BindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.DataSet销售单 = new ERPreport.DataSet销售单();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            ((System.ComponentModel.ISupportInitialize)(this.DataTable销售单BindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataSet销售单)).BeginInit();
            this.SuspendLayout();
            // 
            // DataTable销售单BindingSource
            // 
            this.DataTable销售单BindingSource.DataMember = "DataTable销售单";
            this.DataTable销售单BindingSource.DataSource = this.DataSet销售单;
            // 
            // DataSet销售单
            // 
            this.DataSet销售单.DataSetName = "DataSet销售单";
            this.DataSet销售单.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource2.Name = "DataSet销售";
            reportDataSource2.Value = this.DataTable销售单BindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource2);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "ERPreport.Report销售单.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.ServerReport.BearerToken = null;
            this.reportViewer1.Size = new System.Drawing.Size(1051, 529);
            this.reportViewer1.TabIndex = 0;
            // 
            // 销售单
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1051, 529);
            this.Controls.Add(this.reportViewer1);
            this.Name = "销售单";
            this.Text = "销售单";
            this.Load += new System.EventHandler(this.销售单_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DataTable销售单BindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataSet销售单)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource DataTable销售单BindingSource;
        private DataSet销售单 DataSet销售单;
    }
}