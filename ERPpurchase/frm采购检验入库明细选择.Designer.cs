namespace ERPpurchase
{
    partial class frm采购检验入库明细选择
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.barLargeButtonItem1 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barLargeButtonItem2 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txt_checkall = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.gcJYD = new DevExpress.XtraGrid.GridControl();
            this.gvJYD = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.devGridControlCustom1 = new CZMaster.DevGridControlCustom();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcJYD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvJYD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).BeginInit();
            this.SuspendLayout();
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barLargeButtonItem1,
            this.barLargeButtonItem2});
            this.barManager1.MainMenu = this.bar2;
            this.barManager1.MaxItemId = 2;
            // 
            // bar2
            // 
            this.bar2.BarName = "Main menu";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem1, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem2, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // barLargeButtonItem1
            // 
            this.barLargeButtonItem1.Caption = "确定";
            this.barLargeButtonItem1.Glyph = global::ERPpurchase.Properties.Resources.Mark_32x32;
            this.barLargeButtonItem1.Id = 0;
            this.barLargeButtonItem1.Name = "barLargeButtonItem1";
            this.barLargeButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem1_ItemClick);
            // 
            // barLargeButtonItem2
            // 
            this.barLargeButtonItem2.Caption = "关闭";
            this.barLargeButtonItem2.Glyph = global::ERPpurchase.Properties.Resources.Remove_32x32;
            this.barLargeButtonItem2.Id = 1;
            this.barLargeButtonItem2.Name = "barLargeButtonItem2";
            this.barLargeButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem2_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1272, 60);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 707);
            this.barDockControlBottom.Size = new System.Drawing.Size(1272, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 60);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 647);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1272, 60);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 647);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.txt_checkall);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 60);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1272, 25);
            this.panel1.TabIndex = 4;
            // 
            // txt_checkall
            // 
            this.txt_checkall.AutoSize = true;
            this.txt_checkall.Location = new System.Drawing.Point(18, 6);
            this.txt_checkall.Name = "txt_checkall";
            this.txt_checkall.Size = new System.Drawing.Size(72, 16);
            this.txt_checkall.TabIndex = 0;
            this.txt_checkall.Text = "显示所有";
            this.txt_checkall.UseVisualStyleBackColor = true;
            this.txt_checkall.CheckedChanged += new System.EventHandler(this.txt_checkall_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.gcJYD);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 85);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1272, 622);
            this.panel2.TabIndex = 5;
            // 
            // gcJYD
            // 
            this.devGridControlCustom1.SetDevGridControlCustom(this.gcJYD, "采购检验入库明细选择");
            this.gcJYD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcJYD.Location = new System.Drawing.Point(0, 0);
            this.gcJYD.MainView = this.gvJYD;
            this.gcJYD.MenuManager = this.barManager1;
            this.gcJYD.Name = "gcJYD";
            this.gcJYD.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1,
            this.repositoryItemCheckEdit2});
            this.gcJYD.Size = new System.Drawing.Size(1272, 622);
            this.gcJYD.TabIndex = 0;
            this.gcJYD.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvJYD});
            // 
            // gvJYD
            // 
            this.gvJYD.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvJYD.Appearance.ColumnFilterButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvJYD.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvJYD.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.Black;
            this.gvJYD.Appearance.ColumnFilterButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvJYD.Appearance.ColumnFilterButton.Options.UseBackColor = true;
            this.gvJYD.Appearance.ColumnFilterButton.Options.UseBorderColor = true;
            this.gvJYD.Appearance.ColumnFilterButton.Options.UseForeColor = true;
            this.gvJYD.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gvJYD.Appearance.ColumnFilterButtonActive.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(190)))), ((int)(((byte)(243)))));
            this.gvJYD.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gvJYD.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Black;
            this.gvJYD.Appearance.ColumnFilterButtonActive.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvJYD.Appearance.ColumnFilterButtonActive.Options.UseBackColor = true;
            this.gvJYD.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = true;
            this.gvJYD.Appearance.ColumnFilterButtonActive.Options.UseForeColor = true;
            this.gvJYD.Appearance.Empty.BackColor = System.Drawing.Color.White;
            this.gvJYD.Appearance.Empty.Options.UseBackColor = true;
            this.gvJYD.Appearance.EvenRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(242)))), ((int)(((byte)(254)))));
            this.gvJYD.Appearance.EvenRow.ForeColor = System.Drawing.Color.Black;
            this.gvJYD.Appearance.EvenRow.Options.UseBackColor = true;
            this.gvJYD.Appearance.EvenRow.Options.UseForeColor = true;
            this.gvJYD.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvJYD.Appearance.FilterCloseButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvJYD.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvJYD.Appearance.FilterCloseButton.ForeColor = System.Drawing.Color.Black;
            this.gvJYD.Appearance.FilterCloseButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvJYD.Appearance.FilterCloseButton.Options.UseBackColor = true;
            this.gvJYD.Appearance.FilterCloseButton.Options.UseBorderColor = true;
            this.gvJYD.Appearance.FilterCloseButton.Options.UseForeColor = true;
            this.gvJYD.Appearance.FilterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gvJYD.Appearance.FilterPanel.ForeColor = System.Drawing.Color.White;
            this.gvJYD.Appearance.FilterPanel.Options.UseBackColor = true;
            this.gvJYD.Appearance.FilterPanel.Options.UseForeColor = true;
            this.gvJYD.Appearance.FixedLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(97)))), ((int)(((byte)(156)))));
            this.gvJYD.Appearance.FixedLine.Options.UseBackColor = true;
            this.gvJYD.Appearance.FocusedCell.BackColor = System.Drawing.Color.White;
            this.gvJYD.Appearance.FocusedCell.ForeColor = System.Drawing.Color.Black;
            this.gvJYD.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gvJYD.Appearance.FocusedCell.Options.UseForeColor = true;
            this.gvJYD.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(106)))), ((int)(((byte)(197)))));
            this.gvJYD.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White;
            this.gvJYD.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gvJYD.Appearance.FocusedRow.Options.UseForeColor = true;
            this.gvJYD.Appearance.FooterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvJYD.Appearance.FooterPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvJYD.Appearance.FooterPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvJYD.Appearance.FooterPanel.ForeColor = System.Drawing.Color.Black;
            this.gvJYD.Appearance.FooterPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvJYD.Appearance.FooterPanel.Options.UseBackColor = true;
            this.gvJYD.Appearance.FooterPanel.Options.UseBorderColor = true;
            this.gvJYD.Appearance.FooterPanel.Options.UseForeColor = true;
            this.gvJYD.Appearance.GroupButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvJYD.Appearance.GroupButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvJYD.Appearance.GroupButton.ForeColor = System.Drawing.Color.Black;
            this.gvJYD.Appearance.GroupButton.Options.UseBackColor = true;
            this.gvJYD.Appearance.GroupButton.Options.UseBorderColor = true;
            this.gvJYD.Appearance.GroupButton.Options.UseForeColor = true;
            this.gvJYD.Appearance.GroupFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvJYD.Appearance.GroupFooter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvJYD.Appearance.GroupFooter.ForeColor = System.Drawing.Color.Black;
            this.gvJYD.Appearance.GroupFooter.Options.UseBackColor = true;
            this.gvJYD.Appearance.GroupFooter.Options.UseBorderColor = true;
            this.gvJYD.Appearance.GroupFooter.Options.UseForeColor = true;
            this.gvJYD.Appearance.GroupPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gvJYD.Appearance.GroupPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvJYD.Appearance.GroupPanel.Options.UseBackColor = true;
            this.gvJYD.Appearance.GroupPanel.Options.UseForeColor = true;
            this.gvJYD.Appearance.GroupRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvJYD.Appearance.GroupRow.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gvJYD.Appearance.GroupRow.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.gvJYD.Appearance.GroupRow.ForeColor = System.Drawing.Color.Black;
            this.gvJYD.Appearance.GroupRow.Options.UseBackColor = true;
            this.gvJYD.Appearance.GroupRow.Options.UseBorderColor = true;
            this.gvJYD.Appearance.GroupRow.Options.UseFont = true;
            this.gvJYD.Appearance.GroupRow.Options.UseForeColor = true;
            this.gvJYD.Appearance.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvJYD.Appearance.HeaderPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gvJYD.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gvJYD.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gvJYD.Appearance.HeaderPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gvJYD.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.gvJYD.Appearance.HeaderPanel.Options.UseBorderColor = true;
            this.gvJYD.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvJYD.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(106)))), ((int)(((byte)(153)))), ((int)(((byte)(228)))));
            this.gvJYD.Appearance.HideSelectionRow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(224)))), ((int)(((byte)(251)))));
            this.gvJYD.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.gvJYD.Appearance.HideSelectionRow.Options.UseForeColor = true;
            this.gvJYD.Appearance.HorzLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gvJYD.Appearance.HorzLine.Options.UseBackColor = true;
            this.gvJYD.Appearance.OddRow.BackColor = System.Drawing.Color.White;
            this.gvJYD.Appearance.OddRow.ForeColor = System.Drawing.Color.Black;
            this.gvJYD.Appearance.OddRow.Options.UseBackColor = true;
            this.gvJYD.Appearance.OddRow.Options.UseForeColor = true;
            this.gvJYD.Appearance.Preview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.gvJYD.Appearance.Preview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(129)))), ((int)(((byte)(185)))));
            this.gvJYD.Appearance.Preview.Options.UseBackColor = true;
            this.gvJYD.Appearance.Preview.Options.UseForeColor = true;
            this.gvJYD.Appearance.Row.BackColor = System.Drawing.Color.White;
            this.gvJYD.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gvJYD.Appearance.Row.Options.UseBackColor = true;
            this.gvJYD.Appearance.Row.Options.UseForeColor = true;
            this.gvJYD.Appearance.RowSeparator.BackColor = System.Drawing.Color.White;
            this.gvJYD.Appearance.RowSeparator.Options.UseBackColor = true;
            this.gvJYD.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(126)))), ((int)(((byte)(217)))));
            this.gvJYD.Appearance.SelectedRow.ForeColor = System.Drawing.Color.White;
            this.gvJYD.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gvJYD.Appearance.SelectedRow.Options.UseForeColor = true;
            this.gvJYD.Appearance.VertLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gvJYD.Appearance.VertLine.Options.UseBackColor = true;
            this.gvJYD.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn10});
            this.gvJYD.GridControl = this.gcJYD;
            this.gvJYD.Name = "gvJYD";
            this.gvJYD.OptionsView.EnableAppearanceEvenRow = true;
            this.gvJYD.OptionsView.EnableAppearanceOddRow = true;
            this.gvJYD.OptionsView.ShowGroupPanel = false;
            this.gvJYD.OptionsView.ShowViewCaption = true;
            this.gvJYD.ViewCaption = "检验入库选择";
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "选择";
            this.gridColumn1.ColumnEdit = this.repositoryItemCheckEdit1;
            this.gridColumn1.FieldName = "选择";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            this.repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "检验单号";
            this.gridColumn2.FieldName = "检验记录单号";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "物料编码";
            this.gridColumn3.FieldName = "产品编号";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "供应商";
            this.gridColumn4.FieldName = "供应商";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "送检数量";
            this.gridColumn5.FieldName = "送检数量";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "抽检数量";
            this.gridColumn6.FieldName = "抽检数量";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 5;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "批次数量";
            this.gridColumn7.FieldName = "批次数量";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 6;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "已检数量";
            this.gridColumn8.FieldName = "已检数量";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 7;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "检验结果";
            this.gridColumn9.FieldName = "检验结果";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 8;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "已选择项";
            this.gridColumn10.ColumnEdit = this.repositoryItemCheckEdit2;
            this.gridColumn10.FieldName = "已选择项";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 9;
            // 
            // repositoryItemCheckEdit2
            // 
            this.repositoryItemCheckEdit2.AutoHeight = false;
            this.repositoryItemCheckEdit2.Name = "repositoryItemCheckEdit2";
            this.repositoryItemCheckEdit2.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            // 
            // devGridControlCustom1
            // 
            this.devGridControlCustom1.Authority = "default";
            this.devGridControlCustom1.AutoSave = true;
            this.devGridControlCustom1.strConn = "";
            this.devGridControlCustom1.UserName = "";
            // 
            // frm采购检验入库明细选择
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frm采购检验入库明细选择";
            this.Size = new System.Drawing.Size(1272, 707);
            this.Load += new System.EventHandler(this.frm采购检验入库明细选择_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcJYD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvJYD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem1;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem2;
        private System.Windows.Forms.Panel panel2;
        private DevExpress.XtraGrid.GridControl gcJYD;
        private DevExpress.XtraGrid.Views.Grid.GridView gvJYD;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit2;
        private System.Windows.Forms.CheckBox txt_checkall;
        private CZMaster.DevGridControlCustom devGridControlCustom1;
    }
}
