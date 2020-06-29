namespace ItemInspection
{
    partial class frm成品检验列表
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
            this.gc_checkdan = new DevExpress.XtraGrid.GridControl();
            this.gv_checkdan = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gc_checkdan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_checkdan)).BeginInit();
            this.SuspendLayout();
            // 
            // gc_checkdan
            // 
            this.gc_checkdan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gc_checkdan.Location = new System.Drawing.Point(0, 0);
            this.gc_checkdan.MainView = this.gv_checkdan;
            this.gc_checkdan.Name = "gc_checkdan";
            this.gc_checkdan.Size = new System.Drawing.Size(1285, 577);
            this.gc_checkdan.TabIndex = 5;
            this.gc_checkdan.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_checkdan});
            // 
            // gv_checkdan
            // 
            this.gv_checkdan.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_checkdan.Appearance.ColumnFilterButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv_checkdan.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_checkdan.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.Black;
            this.gv_checkdan.Appearance.ColumnFilterButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_checkdan.Appearance.ColumnFilterButton.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.ColumnFilterButton.Options.UseBorderColor = true;
            this.gv_checkdan.Appearance.ColumnFilterButton.Options.UseForeColor = true;
            this.gv_checkdan.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gv_checkdan.Appearance.ColumnFilterButtonActive.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(190)))), ((int)(((byte)(243)))));
            this.gv_checkdan.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gv_checkdan.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Black;
            this.gv_checkdan.Appearance.ColumnFilterButtonActive.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_checkdan.Appearance.ColumnFilterButtonActive.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = true;
            this.gv_checkdan.Appearance.ColumnFilterButtonActive.Options.UseForeColor = true;
            this.gv_checkdan.Appearance.Empty.BackColor = System.Drawing.Color.White;
            this.gv_checkdan.Appearance.Empty.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.EvenRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(242)))), ((int)(((byte)(254)))));
            this.gv_checkdan.Appearance.EvenRow.ForeColor = System.Drawing.Color.Black;
            this.gv_checkdan.Appearance.EvenRow.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.EvenRow.Options.UseForeColor = true;
            this.gv_checkdan.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_checkdan.Appearance.FilterCloseButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv_checkdan.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_checkdan.Appearance.FilterCloseButton.ForeColor = System.Drawing.Color.Black;
            this.gv_checkdan.Appearance.FilterCloseButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_checkdan.Appearance.FilterCloseButton.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.FilterCloseButton.Options.UseBorderColor = true;
            this.gv_checkdan.Appearance.FilterCloseButton.Options.UseForeColor = true;
            this.gv_checkdan.Appearance.FilterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gv_checkdan.Appearance.FilterPanel.ForeColor = System.Drawing.Color.White;
            this.gv_checkdan.Appearance.FilterPanel.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.FilterPanel.Options.UseForeColor = true;
            this.gv_checkdan.Appearance.FixedLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(97)))), ((int)(((byte)(156)))));
            this.gv_checkdan.Appearance.FixedLine.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.FocusedCell.BackColor = System.Drawing.Color.White;
            this.gv_checkdan.Appearance.FocusedCell.ForeColor = System.Drawing.Color.Black;
            this.gv_checkdan.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.FocusedCell.Options.UseForeColor = true;
            this.gv_checkdan.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(106)))), ((int)(((byte)(197)))));
            this.gv_checkdan.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White;
            this.gv_checkdan.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.FocusedRow.Options.UseForeColor = true;
            this.gv_checkdan.Appearance.FooterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_checkdan.Appearance.FooterPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv_checkdan.Appearance.FooterPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_checkdan.Appearance.FooterPanel.ForeColor = System.Drawing.Color.Black;
            this.gv_checkdan.Appearance.FooterPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_checkdan.Appearance.FooterPanel.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.FooterPanel.Options.UseBorderColor = true;
            this.gv_checkdan.Appearance.FooterPanel.Options.UseForeColor = true;
            this.gv_checkdan.Appearance.GroupButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_checkdan.Appearance.GroupButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_checkdan.Appearance.GroupButton.ForeColor = System.Drawing.Color.Black;
            this.gv_checkdan.Appearance.GroupButton.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.GroupButton.Options.UseBorderColor = true;
            this.gv_checkdan.Appearance.GroupButton.Options.UseForeColor = true;
            this.gv_checkdan.Appearance.GroupFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_checkdan.Appearance.GroupFooter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_checkdan.Appearance.GroupFooter.ForeColor = System.Drawing.Color.Black;
            this.gv_checkdan.Appearance.GroupFooter.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.GroupFooter.Options.UseBorderColor = true;
            this.gv_checkdan.Appearance.GroupFooter.Options.UseForeColor = true;
            this.gv_checkdan.Appearance.GroupPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gv_checkdan.Appearance.GroupPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_checkdan.Appearance.GroupPanel.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.GroupPanel.Options.UseForeColor = true;
            this.gv_checkdan.Appearance.GroupRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_checkdan.Appearance.GroupRow.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv_checkdan.Appearance.GroupRow.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.gv_checkdan.Appearance.GroupRow.ForeColor = System.Drawing.Color.Black;
            this.gv_checkdan.Appearance.GroupRow.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.GroupRow.Options.UseBorderColor = true;
            this.gv_checkdan.Appearance.GroupRow.Options.UseFont = true;
            this.gv_checkdan.Appearance.GroupRow.Options.UseForeColor = true;
            this.gv_checkdan.Appearance.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_checkdan.Appearance.HeaderPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv_checkdan.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv_checkdan.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gv_checkdan.Appearance.HeaderPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv_checkdan.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.HeaderPanel.Options.UseBorderColor = true;
            this.gv_checkdan.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gv_checkdan.Appearance.HideSelectionRow.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.gv_checkdan.Appearance.HideSelectionRow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(224)))), ((int)(((byte)(251)))));
            this.gv_checkdan.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.HideSelectionRow.Options.UseForeColor = true;
            this.gv_checkdan.Appearance.HorzLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gv_checkdan.Appearance.HorzLine.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.OddRow.BackColor = System.Drawing.Color.White;
            this.gv_checkdan.Appearance.OddRow.ForeColor = System.Drawing.Color.Black;
            this.gv_checkdan.Appearance.OddRow.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.OddRow.Options.UseForeColor = true;
            this.gv_checkdan.Appearance.Preview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.gv_checkdan.Appearance.Preview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(129)))), ((int)(((byte)(185)))));
            this.gv_checkdan.Appearance.Preview.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.Preview.Options.UseForeColor = true;
            this.gv_checkdan.Appearance.Row.BackColor = System.Drawing.Color.White;
            this.gv_checkdan.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gv_checkdan.Appearance.Row.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.Row.Options.UseForeColor = true;
            this.gv_checkdan.Appearance.RowSeparator.BackColor = System.Drawing.Color.White;
            this.gv_checkdan.Appearance.RowSeparator.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(126)))), ((int)(((byte)(217)))));
            this.gv_checkdan.Appearance.SelectedRow.ForeColor = System.Drawing.Color.White;
            this.gv_checkdan.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gv_checkdan.Appearance.SelectedRow.Options.UseForeColor = true;
            this.gv_checkdan.Appearance.VertLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gv_checkdan.Appearance.VertLine.Options.UseBackColor = true;
            this.gv_checkdan.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn10,
            this.gridColumn4,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn11,
            this.gridColumn12,
            this.gridColumn13});
            this.gv_checkdan.GridControl = this.gc_checkdan;
            this.gv_checkdan.IndicatorWidth = 40;
            this.gv_checkdan.Name = "gv_checkdan";
            //this.gv_checkdan.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            this.gv_checkdan.OptionsBehavior.Editable = false;
            this.gv_checkdan.OptionsCustomization.AllowSort = false;
            this.gv_checkdan.OptionsFind.AlwaysVisible = true;
            this.gv_checkdan.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gv_checkdan.OptionsSelection.EnableAppearanceHideSelection = false;
            this.gv_checkdan.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            this.gv_checkdan.OptionsView.EnableAppearanceEvenRow = true;
            this.gv_checkdan.OptionsView.EnableAppearanceOddRow = true;
            this.gv_checkdan.OptionsView.ShowGroupPanel = false;
            this.gv_checkdan.OptionsView.ShowViewCaption = true;
            this.gv_checkdan.ViewCaption = "生产检验单列表";
            this.gv_checkdan.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_checkdan_CustomDrawRowIndicator);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.Caption = "生产检验单号";
            this.gridColumn1.FieldName = "生产检验单号";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn2.Caption = "生产工单号";
            this.gridColumn2.FieldName = "生产工单号";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn10
            // 
            this.gridColumn10.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn10.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn10.Caption = "物料编码";
            this.gridColumn10.FieldName = "物料编码";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.OptionsColumn.AllowEdit = false;
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            this.gridColumn4.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn4.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn4.Caption = "物料名称";
            this.gridColumn4.FieldName = "物料名称";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            // 
            // gridColumn6
            // 
            this.gridColumn6.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn6.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn6.Caption = "规格型号";
            this.gridColumn6.FieldName = "规格型号";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 5;
            // 
            // gridColumn7
            // 
            this.gridColumn7.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn7.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn7.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn7.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn7.Caption = "送检数量";
            this.gridColumn7.DisplayFormat.FormatString = "#0.##";
            this.gridColumn7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn7.FieldName = "送检数量";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 6;
            // 
            // gridColumn8
            // 
            this.gridColumn8.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn8.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn8.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn8.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn8.Caption = "合格数量";
            this.gridColumn8.DisplayFormat.FormatString = "#0.##";
            this.gridColumn8.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn8.FieldName = "合格数量";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 7;
            // 
            // gridColumn9
            // 
            this.gridColumn9.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn9.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn9.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn9.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn9.Caption = "不合格数量";
            this.gridColumn9.DisplayFormat.FormatString = "#0.##";
            this.gridColumn9.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn9.FieldName = "不合格数量";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 8;
            // 
            // gridColumn11
            // 
            this.gridColumn11.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn11.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn11.Caption = "检验人员";
            this.gridColumn11.FieldName = "检验人员";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.OptionsColumn.AllowEdit = false;
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 9;
            // 
            // gridColumn12
            // 
            this.gridColumn12.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn12.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn12.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn12.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn12.Caption = "检验日期";
            this.gridColumn12.DisplayFormat.FormatString = "yyyy-MM-dd";
            this.gridColumn12.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn12.FieldName = "检验日期";
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.OptionsColumn.AllowEdit = false;
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 10;
            // 
            // gridColumn13
            // 
            this.gridColumn13.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn13.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn13.Caption = "规格型号";
            this.gridColumn13.FieldName = "原规格型号";
            this.gridColumn13.Name = "gridColumn13";
            this.gridColumn13.Visible = true;
            this.gridColumn13.VisibleIndex = 4;
            // 
            // frm成品检验列表
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1285, 577);
            this.Controls.Add(this.gc_checkdan);
            this.Name = "frm成品检验列表";
            this.Text = "frm成品检验列表";
            this.Load += new System.EventHandler(this.frm成品检验列表_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gc_checkdan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_checkdan)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gc_checkdan;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_checkdan;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
    }
}