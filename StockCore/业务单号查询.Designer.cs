namespace StockCore
{
    partial class 业务单号查询
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.gcP = new DevExpress.XtraGrid.GridControl();
            this.gvP = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn15 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ghinder1 = new ERPorg.ghinder(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvP)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.gcP);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1109, 596);
            this.panel1.TabIndex = 0;
            // 
            // gcP
            // 
            this.gcP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ghinder1.SetgridInder(this.gcP, "1");
            this.gcP.Location = new System.Drawing.Point(0, 0);
            this.gcP.MainView = this.gvP;
            this.gcP.Name = "gcP";
            this.gcP.Size = new System.Drawing.Size(1109, 596);
            this.gcP.TabIndex = 6;
            this.gcP.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvP});
            // 
            // gvP
            // 
            this.gvP.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.ColumnFilterButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvP.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.ColumnFilterButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvP.Appearance.ColumnFilterButton.Options.UseBackColor = true;
            this.gvP.Appearance.ColumnFilterButton.Options.UseBorderColor = true;
            this.gvP.Appearance.ColumnFilterButton.Options.UseForeColor = true;
            this.gvP.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gvP.Appearance.ColumnFilterButtonActive.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(190)))), ((int)(((byte)(243)))));
            this.gvP.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gvP.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.ColumnFilterButtonActive.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvP.Appearance.ColumnFilterButtonActive.Options.UseBackColor = true;
            this.gvP.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = true;
            this.gvP.Appearance.ColumnFilterButtonActive.Options.UseForeColor = true;
            this.gvP.Appearance.Empty.BackColor = System.Drawing.Color.White;
            this.gvP.Appearance.Empty.Options.UseBackColor = true;
            this.gvP.Appearance.EvenRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(242)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.EvenRow.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.EvenRow.Options.UseBackColor = true;
            this.gvP.Appearance.EvenRow.Options.UseForeColor = true;
            this.gvP.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.FilterCloseButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvP.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.FilterCloseButton.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.FilterCloseButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvP.Appearance.FilterCloseButton.Options.UseBackColor = true;
            this.gvP.Appearance.FilterCloseButton.Options.UseBorderColor = true;
            this.gvP.Appearance.FilterCloseButton.Options.UseForeColor = true;
            this.gvP.Appearance.FilterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gvP.Appearance.FilterPanel.ForeColor = System.Drawing.Color.White;
            this.gvP.Appearance.FilterPanel.Options.UseBackColor = true;
            this.gvP.Appearance.FilterPanel.Options.UseForeColor = true;
            this.gvP.Appearance.FixedLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(97)))), ((int)(((byte)(156)))));
            this.gvP.Appearance.FixedLine.Options.UseBackColor = true;
            this.gvP.Appearance.FocusedCell.BackColor = System.Drawing.Color.White;
            this.gvP.Appearance.FocusedCell.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gvP.Appearance.FocusedCell.Options.UseForeColor = true;
            this.gvP.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(106)))), ((int)(((byte)(197)))));
            this.gvP.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White;
            this.gvP.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gvP.Appearance.FocusedRow.Options.UseForeColor = true;
            this.gvP.Appearance.FooterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.FooterPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvP.Appearance.FooterPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.FooterPanel.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.FooterPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvP.Appearance.FooterPanel.Options.UseBackColor = true;
            this.gvP.Appearance.FooterPanel.Options.UseBorderColor = true;
            this.gvP.Appearance.FooterPanel.Options.UseForeColor = true;
            this.gvP.Appearance.GroupButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvP.Appearance.GroupButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvP.Appearance.GroupButton.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.GroupButton.Options.UseBackColor = true;
            this.gvP.Appearance.GroupButton.Options.UseBorderColor = true;
            this.gvP.Appearance.GroupButton.Options.UseForeColor = true;
            this.gvP.Appearance.GroupFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvP.Appearance.GroupFooter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvP.Appearance.GroupFooter.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.GroupFooter.Options.UseBackColor = true;
            this.gvP.Appearance.GroupFooter.Options.UseBorderColor = true;
            this.gvP.Appearance.GroupFooter.Options.UseForeColor = true;
            this.gvP.Appearance.GroupPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gvP.Appearance.GroupPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.GroupPanel.Options.UseBackColor = true;
            this.gvP.Appearance.GroupPanel.Options.UseForeColor = true;
            this.gvP.Appearance.GroupRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvP.Appearance.GroupRow.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvP.Appearance.GroupRow.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.gvP.Appearance.GroupRow.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.GroupRow.Options.UseBackColor = true;
            this.gvP.Appearance.GroupRow.Options.UseBorderColor = true;
            this.gvP.Appearance.GroupRow.Options.UseFont = true;
            this.gvP.Appearance.GroupRow.Options.UseForeColor = true;
            this.gvP.Appearance.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.HeaderPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvP.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvP.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.HeaderPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvP.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.gvP.Appearance.HeaderPanel.Options.UseBorderColor = true;
            this.gvP.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvP.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(106)))), ((int)(((byte)(153)))), ((int)(((byte)(228)))));
            this.gvP.Appearance.HideSelectionRow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(224)))), ((int)(((byte)(251)))));
            this.gvP.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.gvP.Appearance.HideSelectionRow.Options.UseForeColor = true;
            this.gvP.Appearance.HorzLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gvP.Appearance.HorzLine.Options.UseBackColor = true;
            this.gvP.Appearance.OddRow.BackColor = System.Drawing.Color.White;
            this.gvP.Appearance.OddRow.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.OddRow.Options.UseBackColor = true;
            this.gvP.Appearance.OddRow.Options.UseForeColor = true;
            this.gvP.Appearance.Preview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.gvP.Appearance.Preview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(129)))), ((int)(((byte)(185)))));
            this.gvP.Appearance.Preview.Options.UseBackColor = true;
            this.gvP.Appearance.Preview.Options.UseForeColor = true;
            this.gvP.Appearance.Row.BackColor = System.Drawing.Color.White;
            this.gvP.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gvP.Appearance.Row.Options.UseBackColor = true;
            this.gvP.Appearance.Row.Options.UseForeColor = true;
            this.gvP.Appearance.RowSeparator.BackColor = System.Drawing.Color.White;
            this.gvP.Appearance.RowSeparator.Options.UseBackColor = true;
            this.gvP.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(126)))), ((int)(((byte)(217)))));
            this.gvP.Appearance.SelectedRow.ForeColor = System.Drawing.Color.White;
            this.gvP.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gvP.Appearance.SelectedRow.Options.UseForeColor = true;
            this.gvP.Appearance.VertLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gvP.Appearance.VertLine.Options.UseBackColor = true;
            this.gvP.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn15,
            this.gridColumn10,
            this.gridColumn11,
            this.gridColumn1});
            this.gvP.GridControl = this.gcP;
            this.gvP.Name = "gvP";
            this.gvP.OptionsBehavior.Editable = false;
            this.gvP.OptionsView.EnableAppearanceEvenRow = true;
            this.gvP.OptionsView.EnableAppearanceOddRow = true;
            this.gvP.OptionsView.ShowGroupPanel = false;
            this.gvP.OptionsView.ShowViewCaption = true;
            this.gvP.PaintStyleName = "Office2003";
            this.gvP.ViewCaption = "业务单号对应明细表";
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "出入库申请明细号";
            this.gridColumn6.FieldName = "出入库申请明细号";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 1;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "物料编码";
            this.gridColumn7.FieldName = "物料编码";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 2;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "物料名称";
            this.gridColumn8.FieldName = "物料名称";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 3;
            // 
            // gridColumn9
            // 
            this.gridColumn9.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn9.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn9.Caption = "数量";
            this.gridColumn9.FieldName = "数量";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 5;
            // 
            // gridColumn15
            // 
            this.gridColumn15.Caption = "已处理数量";
            this.gridColumn15.DisplayFormat.FormatString = "0.0000";
            this.gridColumn15.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn15.FieldName = "已处理数量";
            this.gridColumn15.Name = "gridColumn15";
            this.gridColumn15.Visible = true;
            this.gridColumn15.VisibleIndex = 6;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "规格型号";
            this.gridColumn10.FieldName = "规格型号";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 4;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "备注";
            this.gridColumn11.FieldName = "备注";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 7;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "出入库申请单号";
            this.gridColumn1.FieldName = "出入库申请单号";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // ghinder1
            // 
            this.ghinder1.bool_V = true;
            this.ghinder1.EnableCtrlV = false;
            this.ghinder1.TotalCopy = true;
            this.ghinder1.UIName = "业务单号查询";
            this.ghinder1.Width = 40;
            // 
            // 业务单号查询
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1109, 596);
            this.Controls.Add(this.panel1);
            this.Name = "业务单号查询";
            this.Text = "业务单号查询";
            this.Load += new System.EventHandler(this.业务单号查询_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvP)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraGrid.GridControl gcP;
        private DevExpress.XtraGrid.Views.Grid.GridView gvP;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn15;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private ERPorg.ghinder ghinder1;
    }
}