namespace ERPSale
{
    partial class frm销售记录成品出库详细界面_视图
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
            this.barLargeButtonItem1 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barLargeButtonItem2 = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txt_日期 = new System.Windows.Forms.TextBox();
            this.txt_仓库 = new System.Windows.Forms.TextBox();
            this.txt_客户 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txt_操作员 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_成品出库单号 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.gcP = new DevExpress.XtraGrid.GridControl();
            this.gvP = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.dataBindHelper1 = new CPublic.DataBindHelper();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.查看物料明细ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.barLargeButtonItem3 = new DevExpress.XtraBars.BarLargeButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvP)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
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
            this.barLargeButtonItem3});
            this.barManager1.MainMenu = this.bar2;
            this.barManager1.MaxItemId = 3;
            // 
            // bar2
            // 
            this.bar2.BarName = "Main menu";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem1, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barLargeButtonItem2, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem3)});
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // barLargeButtonItem1
            // 
            this.barLargeButtonItem1.Caption = "打印";
            this.barLargeButtonItem1.Glyph = global::ERPSale.Properties.Resources.Print_32x32;
            this.barLargeButtonItem1.Id = 0;
            this.barLargeButtonItem1.Name = "barLargeButtonItem1";
            this.barLargeButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem1_ItemClick);
            // 
            // barLargeButtonItem2
            // 
            this.barLargeButtonItem2.Caption = "关闭界面";
            this.barLargeButtonItem2.Glyph = global::ERPSale.Properties.Resources.Close_32x32;
            this.barLargeButtonItem2.Id = 1;
            this.barLargeButtonItem2.Name = "barLargeButtonItem2";
            this.barLargeButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem2_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.barDockControlTop.Size = new System.Drawing.Size(1493, 73);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 838);
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(1493, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 73);
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 765);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1493, 73);
            this.barDockControlRight.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 765);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.txt_日期);
            this.panel1.Controls.Add(this.txt_仓库);
            this.panel1.Controls.Add(this.txt_客户);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.txt_操作员);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.txt_成品出库单号);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 73);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1493, 104);
            this.panel1.TabIndex = 4;
            // 
            // txt_日期
            // 
            this.dataBindHelper1.SetBindFieldName(this.txt_日期, "日期");
            this.txt_日期.Enabled = false;
            this.txt_日期.Location = new System.Drawing.Point(792, 21);
            this.txt_日期.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txt_日期.Name = "txt_日期";
            this.txt_日期.ReadOnly = true;
            this.txt_日期.Size = new System.Drawing.Size(200, 25);
            this.txt_日期.TabIndex = 111;
            // 
            // txt_仓库
            // 
            this.dataBindHelper1.SetBindFieldName(this.txt_仓库, "仓库");
            this.txt_仓库.Enabled = false;
            this.txt_仓库.Location = new System.Drawing.Point(476, 55);
            this.txt_仓库.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txt_仓库.Name = "txt_仓库";
            this.txt_仓库.ReadOnly = true;
            this.txt_仓库.Size = new System.Drawing.Size(200, 25);
            this.txt_仓库.TabIndex = 110;
            // 
            // txt_客户
            // 
            this.dataBindHelper1.SetBindFieldName(this.txt_客户, "客户");
            this.txt_客户.Location = new System.Drawing.Point(152, 55);
            this.txt_客户.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txt_客户.Name = "txt_客户";
            this.txt_客户.ReadOnly = true;
            this.txt_客户.Size = new System.Drawing.Size(200, 25);
            this.txt_客户.TabIndex = 109;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(397, 61);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 15);
            this.label2.TabIndex = 106;
            this.label2.Text = "仓  库：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 61);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 15);
            this.label1.TabIndex = 105;
            this.label1.Text = "客      户：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(713, 26);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 15);
            this.label8.TabIndex = 103;
            this.label8.Text = "日  期：";
            // 
            // txt_操作员
            // 
            this.dataBindHelper1.SetBindFieldName(this.txt_操作员, "操作员");
            this.txt_操作员.Enabled = false;
            this.txt_操作员.Location = new System.Drawing.Point(476, 21);
            this.txt_操作员.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txt_操作员.Name = "txt_操作员";
            this.txt_操作员.ReadOnly = true;
            this.txt_操作员.Size = new System.Drawing.Size(200, 25);
            this.txt_操作员.TabIndex = 102;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(397, 26);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 15);
            this.label7.TabIndex = 101;
            this.label7.Text = "操作员：";
            // 
            // txt_成品出库单号
            // 
            this.dataBindHelper1.SetBindFieldName(this.txt_成品出库单号, "成品出库单号");
            this.txt_成品出库单号.Location = new System.Drawing.Point(152, 21);
            this.txt_成品出库单号.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txt_成品出库单号.Name = "txt_成品出库单号";
            this.txt_成品出库单号.ReadOnly = true;
            this.txt_成品出库单号.Size = new System.Drawing.Size(200, 25);
            this.txt_成品出库单号.TabIndex = 100;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 26);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 15);
            this.label4.TabIndex = 99;
            this.label4.Text = "销售出库单号：";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.gcP);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 177);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1493, 661);
            this.panel2.TabIndex = 5;
            // 
            // gcP
            // 
            this.gcP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcP.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gcP.Location = new System.Drawing.Point(0, 0);
            this.gcP.MainView = this.gvP;
            this.gcP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gcP.MenuManager = this.barManager1;
            this.gcP.Name = "gcP";
            this.gcP.Size = new System.Drawing.Size(1493, 661);
            this.gcP.TabIndex = 1;
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
            this.gvP.Appearance.HideSelectionRow.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.gvP.Appearance.HideSelectionRow.ForeColor = System.Drawing.Color.Black;
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
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn3,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn10,
            this.gridColumn8,
            this.gridColumn9});
            this.gvP.GridControl = this.gcP;
            this.gvP.Name = "gvP";
            this.gvP.OptionsBehavior.Editable = false;
            this.gvP.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
            this.gvP.OptionsFind.AlwaysVisible = true;
            this.gvP.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            this.gvP.OptionsView.EnableAppearanceEvenRow = true;
            this.gvP.OptionsView.EnableAppearanceOddRow = true;
            this.gvP.OptionsView.ShowGroupPanel = false;
            this.gvP.OptionsView.ShowViewCaption = true;
            this.gvP.ViewCaption = "出库明细";
            this.gvP.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gvP_RowCellClick);
            this.gvP.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gvP_KeyDown);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.Caption = "物料编码";
            this.gridColumn1.FieldName = "物料编码";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn2.Caption = "物料名称";
            this.gridColumn2.FieldName = "物料名称";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn4
            // 
            this.gridColumn4.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn4.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn4.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn4.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn4.Caption = "出库数量";
            this.gridColumn4.FieldName = "出库数量";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 4;
            // 
            // gridColumn5
            // 
            this.gridColumn5.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn5.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn5.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn5.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn5.Caption = "已出库数量";
            this.gridColumn5.FieldName = "已出库数量";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 3;
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn3.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn3.Caption = "可出库参考数量";
            this.gridColumn3.FieldName = "参考数量";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 5;
            // 
            // gridColumn6
            // 
            this.gridColumn6.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn6.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn6.Caption = "仓库名称";
            this.gridColumn6.FieldName = "仓库名称";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.AllowEdit = false;
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 8;
            // 
            // gridColumn7
            // 
            this.gridColumn7.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn7.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn7.Caption = "仓库号";
            this.gridColumn7.FieldName = "仓库号";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.OptionsColumn.AllowEdit = false;
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 7;
            // 
            // gridColumn10
            // 
            this.gridColumn10.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn10.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn10.Caption = "计量单位";
            this.gridColumn10.FieldName = "计量单位";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 6;
            // 
            // gridColumn8
            // 
            this.gridColumn8.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn8.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn8.Caption = "原ERP物料编号";
            this.gridColumn8.FieldName = "原ERP物料编号";
            this.gridColumn8.Name = "gridColumn8";
            // 
            // gridColumn9
            // 
            this.gridColumn9.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn9.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn9.Caption = "规格型号";
            this.gridColumn9.FieldName = "规格型号";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 2;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.查看物料明细ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(169, 28);
            // 
            // 查看物料明细ToolStripMenuItem
            // 
            this.查看物料明细ToolStripMenuItem.Name = "查看物料明细ToolStripMenuItem";
            this.查看物料明细ToolStripMenuItem.Size = new System.Drawing.Size(168, 24);
            this.查看物料明细ToolStripMenuItem.Text = "查看物料明细";
            this.查看物料明细ToolStripMenuItem.Click += new System.EventHandler(this.查看物料明细ToolStripMenuItem_Click);
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // barLargeButtonItem3
            // 
            this.barLargeButtonItem3.Caption = "打印送货单";
            this.barLargeButtonItem3.Id = 2;
            this.barLargeButtonItem3.Name = "barLargeButtonItem3";
            this.barLargeButtonItem3.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barLargeButtonItem3_ItemClick);
            // 
            // frm销售记录成品出库详细界面_视图
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frm销售记录成品出库详细界面_视图";
            this.Size = new System.Drawing.Size(1493, 838);
            this.Load += new System.EventHandler(this.frm销售记录成品出库详细界面_视图_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvP)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txt_操作员;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt_成品出库单号;
        private System.Windows.Forms.Label label4;
        private DevExpress.XtraGrid.GridControl gcP;
        private DevExpress.XtraGrid.Views.Grid.GridView gvP;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem1;
        private CPublic.DataBindHelper dataBindHelper1;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem2;
        private System.Windows.Forms.TextBox txt_客户;
        private System.Windows.Forms.TextBox txt_日期;
        private System.Windows.Forms.TextBox txt_仓库;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 查看物料明细ToolStripMenuItem;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraBars.BarLargeButtonItem barLargeButtonItem3;
    }
}
