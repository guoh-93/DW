﻿namespace ERPproduct
{
    partial class frm成品入库列表
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
            this.components = new System.ComponentModel.Container();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
            this.barEditItem1 = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.barEditItem3 = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemDateEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.barStaticItem2 = new DevExpress.XtraBars.BarStaticItem();
            this.barEditItem4 = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.barLargeButtonItem1 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barLargeButtonItem2 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barLargeButtonItem3 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gc = new DevExpress.XtraGrid.GridControl();
            this.gv = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.barEditItem2 = new DevExpress.XtraBars.BarEditItem();
            this.devGridControlCustom1 = new CZMaster.DevGridControlCustom();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit2.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv)).BeginInit();
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
            this.barLargeButtonItem2,
            this.barLargeButtonItem3,
            this.barStaticItem1,
            this.barEditItem1,
            this.barEditItem3,
            this.barStaticItem2,
            this.barEditItem4});
            this.barManager1.MainMenu = this.bar2;
            this.barManager1.MaxItemId = 12;
            this.barManager1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemDateEdit1,
            this.repositoryItemDateEdit2,
            this.repositoryItemComboBox1});
            // 
            // bar2
            // 
            this.bar2.BarName = "Main menu";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barStaticItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.barEditItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.barEditItem3),
            new DevExpress.XtraBars.LinkPersistInfo(this.barStaticItem2, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.barEditItem4),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem1, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem2, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem3, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // barStaticItem1
            // 
            this.barStaticItem1.Caption = "日期：";
            this.barStaticItem1.Id = 7;
            this.barStaticItem1.Name = "barStaticItem1";
            this.barStaticItem1.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // barEditItem1
            // 
            this.barEditItem1.Edit = this.repositoryItemDateEdit1;
            this.barEditItem1.Id = 8;
            this.barEditItem1.Name = "barEditItem1";
            this.barEditItem1.Width = 90;
            // 
            // repositoryItemDateEdit1
            // 
            this.repositoryItemDateEdit1.AutoHeight = false;
            this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
            this.repositoryItemDateEdit1.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            // 
            // barEditItem3
            // 
            this.barEditItem3.Edit = this.repositoryItemDateEdit2;
            this.barEditItem3.Id = 9;
            this.barEditItem3.Name = "barEditItem3";
            this.barEditItem3.Width = 81;
            // 
            // repositoryItemDateEdit2
            // 
            this.repositoryItemDateEdit2.AutoHeight = false;
            this.repositoryItemDateEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemDateEdit2.Name = "repositoryItemDateEdit2";
            this.repositoryItemDateEdit2.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            // 
            // barStaticItem2
            // 
            this.barStaticItem2.Caption = "单据状态：";
            this.barStaticItem2.Id = 10;
            this.barStaticItem2.Name = "barStaticItem2";
            this.barStaticItem2.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // barEditItem4
            // 
            this.barEditItem4.Edit = this.repositoryItemComboBox1;
            this.barEditItem4.Id = 11;
            this.barEditItem4.Name = "barEditItem4";
            this.barEditItem4.Width = 90;
            // 
            // repositoryItemComboBox1
            // 
            this.repositoryItemComboBox1.AutoHeight = false;
            this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox1.Items.AddRange(new object[] {
            "",
            "已生效",
            "未生效",
            "已完成",
            "未完成",
            "已作废",
            "未作废",
            "全部"});
            this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
            // 
            // barLargeButtonItem1
            // 
            this.barLargeButtonItem1.Caption = "刷新";
            this.barLargeButtonItem1.Glyph = global::ERPproduct.Properties.Resources.GenerateData_32x32;
            this.barLargeButtonItem1.Id = 4;
            this.barLargeButtonItem1.Name = "barLargeButtonItem1";
            this.barLargeButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem1_ItemClick);
            // 
            // barLargeButtonItem2
            // 
            this.barLargeButtonItem2.Caption = "新增";
            this.barLargeButtonItem2.Glyph = global::ERPproduct.Properties.Resources.AddToLibrary_32x32;
            this.barLargeButtonItem2.Id = 5;
            this.barLargeButtonItem2.Name = "barLargeButtonItem2";
            this.barLargeButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem2_ItemClick);
            // 
            // barLargeButtonItem3
            // 
            this.barLargeButtonItem3.Caption = "关闭";
            this.barLargeButtonItem3.Glyph = global::ERPproduct.Properties.Resources.Close_32x32;
            this.barLargeButtonItem3.Id = 6;
            this.barLargeButtonItem3.Name = "barLargeButtonItem3";
            this.barLargeButtonItem3.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem3_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1527, 60);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 765);
            this.barDockControlBottom.Size = new System.Drawing.Size(1527, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 60);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 705);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1527, 60);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 705);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.gc);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 60);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1527, 705);
            this.panel1.TabIndex = 4;
            // 
            // gc
            // 
            this.devGridControlCustom1.SetDevGridControlCustom(this.gc, "gcm成品入库列表");
            this.gc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gc.Location = new System.Drawing.Point(0, 0);
            this.gc.MainView = this.gv;
            this.gc.MenuManager = this.barManager1;
            this.gc.Name = "gc";
            this.gc.Size = new System.Drawing.Size(1527, 705);
            this.gc.TabIndex = 0;
            this.gc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv});
            // 
            // gv
            // 
            this.gv.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv.Appearance.ColumnFilterButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.Black;
            this.gv.Appearance.ColumnFilterButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv.Appearance.ColumnFilterButton.Options.UseBackColor = true;
            this.gv.Appearance.ColumnFilterButton.Options.UseBorderColor = true;
            this.gv.Appearance.ColumnFilterButton.Options.UseForeColor = true;
            this.gv.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gv.Appearance.ColumnFilterButtonActive.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(190)))), ((int)(((byte)(243)))));
            this.gv.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gv.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Black;
            this.gv.Appearance.ColumnFilterButtonActive.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv.Appearance.ColumnFilterButtonActive.Options.UseBackColor = true;
            this.gv.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = true;
            this.gv.Appearance.ColumnFilterButtonActive.Options.UseForeColor = true;
            this.gv.Appearance.Empty.BackColor = System.Drawing.Color.White;
            this.gv.Appearance.Empty.Options.UseBackColor = true;
            this.gv.Appearance.EvenRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(242)))), ((int)(((byte)(254)))));
            this.gv.Appearance.EvenRow.ForeColor = System.Drawing.Color.Black;
            this.gv.Appearance.EvenRow.Options.UseBackColor = true;
            this.gv.Appearance.EvenRow.Options.UseForeColor = true;
            this.gv.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv.Appearance.FilterCloseButton.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv.Appearance.FilterCloseButton.ForeColor = System.Drawing.Color.Black;
            this.gv.Appearance.FilterCloseButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv.Appearance.FilterCloseButton.Options.UseBackColor = true;
            this.gv.Appearance.FilterCloseButton.Options.UseBorderColor = true;
            this.gv.Appearance.FilterCloseButton.Options.UseForeColor = true;
            this.gv.Appearance.FilterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gv.Appearance.FilterPanel.ForeColor = System.Drawing.Color.White;
            this.gv.Appearance.FilterPanel.Options.UseBackColor = true;
            this.gv.Appearance.FilterPanel.Options.UseForeColor = true;
            this.gv.Appearance.FixedLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(97)))), ((int)(((byte)(156)))));
            this.gv.Appearance.FixedLine.Options.UseBackColor = true;
            this.gv.Appearance.FocusedCell.BackColor = System.Drawing.Color.White;
            this.gv.Appearance.FocusedCell.ForeColor = System.Drawing.Color.Black;
            this.gv.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gv.Appearance.FocusedCell.Options.UseForeColor = true;
            this.gv.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(106)))), ((int)(((byte)(197)))));
            this.gv.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White;
            this.gv.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gv.Appearance.FocusedRow.Options.UseForeColor = true;
            this.gv.Appearance.FooterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv.Appearance.FooterPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv.Appearance.FooterPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv.Appearance.FooterPanel.ForeColor = System.Drawing.Color.Black;
            this.gv.Appearance.FooterPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv.Appearance.FooterPanel.Options.UseBackColor = true;
            this.gv.Appearance.FooterPanel.Options.UseBorderColor = true;
            this.gv.Appearance.FooterPanel.Options.UseForeColor = true;
            this.gv.Appearance.GroupButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv.Appearance.GroupButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv.Appearance.GroupButton.ForeColor = System.Drawing.Color.Black;
            this.gv.Appearance.GroupButton.Options.UseBackColor = true;
            this.gv.Appearance.GroupButton.Options.UseBorderColor = true;
            this.gv.Appearance.GroupButton.Options.UseForeColor = true;
            this.gv.Appearance.GroupFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv.Appearance.GroupFooter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv.Appearance.GroupFooter.ForeColor = System.Drawing.Color.Black;
            this.gv.Appearance.GroupFooter.Options.UseBackColor = true;
            this.gv.Appearance.GroupFooter.Options.UseBorderColor = true;
            this.gv.Appearance.GroupFooter.Options.UseForeColor = true;
            this.gv.Appearance.GroupPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.gv.Appearance.GroupPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv.Appearance.GroupPanel.Options.UseBackColor = true;
            this.gv.Appearance.GroupPanel.Options.UseForeColor = true;
            this.gv.Appearance.GroupRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv.Appearance.GroupRow.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(247)))));
            this.gv.Appearance.GroupRow.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.gv.Appearance.GroupRow.ForeColor = System.Drawing.Color.Black;
            this.gv.Appearance.GroupRow.Options.UseBackColor = true;
            this.gv.Appearance.GroupRow.Options.UseBorderColor = true;
            this.gv.Appearance.GroupRow.Options.UseFont = true;
            this.gv.Appearance.GroupRow.Options.UseForeColor = true;
            this.gv.Appearance.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv.Appearance.HeaderPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(171)))), ((int)(((byte)(228)))));
            this.gv.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254)))));
            this.gv.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gv.Appearance.HeaderPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.gv.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.gv.Appearance.HeaderPanel.Options.UseBorderColor = true;
            this.gv.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gv.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(106)))), ((int)(((byte)(153)))), ((int)(((byte)(228)))));
            this.gv.Appearance.HideSelectionRow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(224)))), ((int)(((byte)(251)))));
            this.gv.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.gv.Appearance.HideSelectionRow.Options.UseForeColor = true;
            this.gv.Appearance.HorzLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gv.Appearance.HorzLine.Options.UseBackColor = true;
            this.gv.Appearance.OddRow.BackColor = System.Drawing.Color.White;
            this.gv.Appearance.OddRow.ForeColor = System.Drawing.Color.Black;
            this.gv.Appearance.OddRow.Options.UseBackColor = true;
            this.gv.Appearance.OddRow.Options.UseForeColor = true;
            this.gv.Appearance.Preview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.gv.Appearance.Preview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(129)))), ((int)(((byte)(185)))));
            this.gv.Appearance.Preview.Options.UseBackColor = true;
            this.gv.Appearance.Preview.Options.UseForeColor = true;
            this.gv.Appearance.Row.BackColor = System.Drawing.Color.White;
            this.gv.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gv.Appearance.Row.Options.UseBackColor = true;
            this.gv.Appearance.Row.Options.UseForeColor = true;
            this.gv.Appearance.RowSeparator.BackColor = System.Drawing.Color.White;
            this.gv.Appearance.RowSeparator.Options.UseBackColor = true;
            this.gv.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(126)))), ((int)(((byte)(217)))));
            this.gv.Appearance.SelectedRow.ForeColor = System.Drawing.Color.White;
            this.gv.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gv.Appearance.SelectedRow.Options.UseForeColor = true;
            this.gv.Appearance.VertLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(127)))), ((int)(((byte)(196)))));
            this.gv.Appearance.VertLine.Options.UseBackColor = true;
            this.gv.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn3,
            this.gridColumn5,
            this.gridColumn4,
            this.gridColumn2});
            this.gv.GridControl = this.gc;
            this.gv.Name = "gv";
            this.gv.OptionsBehavior.Editable = false;
            this.gv.OptionsFind.AlwaysVisible = true;
            this.gv.OptionsView.EnableAppearanceEvenRow = true;
            this.gv.OptionsView.EnableAppearanceOddRow = true;
            this.gv.OptionsView.ShowGroupPanel = false;
            this.gv.OptionsView.ShowViewCaption = true;
            this.gv.ViewCaption = "成品入库单列表";
            this.gv.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gv_RowCellClick);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "成品入库单号";
            this.gridColumn1.FieldName = "成品入库单号";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "仓库名称";
            this.gridColumn3.FieldName = "仓库名称";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 1;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "入库人员";
            this.gridColumn5.FieldName = "入库人员";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "录入日期";
            this.gridColumn4.FieldName = "录入日期";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "录入人员";
            this.gridColumn2.FieldName = "录入人员";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 4;
            // 
            // barEditItem2
            // 
            this.barEditItem2.Edit = this.repositoryItemDateEdit1;
            this.barEditItem2.Id = 8;
            this.barEditItem2.Name = "barEditItem2";
            this.barEditItem2.Width = 90;
            // 
            // devGridControlCustom1
            // 
            this.devGridControlCustom1.Authority = "default";
            this.devGridControlCustom1.AutoSave = true;
            this.devGridControlCustom1.strConn = "";
            this.devGridControlCustom1.UserName = "";
            // 
            // frm成品入库列表
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frm成品入库列表";
            this.Size = new System.Drawing.Size(1527, 765);
            this.Load += new System.EventHandler(this.frm成品入库_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit2.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraGrid.GridControl gc;
        private DevExpress.XtraGrid.Views.Grid.GridView gv;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem1;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem2;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem3;
        private DevExpress.XtraBars.BarStaticItem barStaticItem1;
        private DevExpress.XtraBars.BarEditItem barEditItem1;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repositoryItemDateEdit1;
        private DevExpress.XtraBars.BarEditItem barEditItem3;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repositoryItemDateEdit2;
        private DevExpress.XtraBars.BarEditItem barEditItem2;
        private DevExpress.XtraBars.BarStaticItem barStaticItem2;
        private DevExpress.XtraBars.BarEditItem barEditItem4;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private CZMaster.DevGridControlCustom devGridControlCustom1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
    }
}
