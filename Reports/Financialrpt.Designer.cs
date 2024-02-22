﻿namespace CRM.Reports
{
    partial class Financialrpt
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

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Financialrpt));
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.detailTable = new DevExpress.XtraReports.UI.XRTable();
            this.detailTableRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.productName = new DevExpress.XtraReports.UI.XRTableCell();
            this.quantity = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell4 = new DevExpress.XtraReports.UI.XRTableCell();
            this.unitPrice = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.lineTotal = new DevExpress.XtraReports.UI.XRTableCell();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.thankYouLabel = new DevExpress.XtraReports.UI.XRLabel();
            this.heartLabel = new DevExpress.XtraReports.UI.XRLabel();
            this.GroupHeader2 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.vendorLogo = new DevExpress.XtraReports.UI.XRPictureBox();
            this.invoiceLabel = new DevExpress.XtraReports.UI.XRLabel();
            this.GroupFooter1 = new DevExpress.XtraReports.UI.GroupFooterBand();
            this.subtotalTable = new DevExpress.XtraReports.UI.XRTable();
            this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.headerTable = new DevExpress.XtraReports.UI.XRTable();
            this.headerTableRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.productNameCaption = new DevExpress.XtraReports.UI.XRTableCell();
            this.quantityCaption = new DevExpress.XtraReports.UI.XRTableCell();
            this.unitPriceCaption = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.lineTotalCaptionCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.objectDataSource1 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            this.baseControlStyle = new DevExpress.XtraReports.UI.XRControlStyle();
            ((System.ComponentModel.ISupportInitialize)(this.detailTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.subtotalTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.headerTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.detailTable});
            this.Detail.HeightF = 35F;
            this.Detail.KeepTogether = true;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.StyleName = "baseControlStyle";
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // detailTable
            // 
            this.detailTable.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.detailTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.detailTable.Name = "detailTable";
            this.detailTable.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.detailTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.detailTableRow});
            this.detailTable.SizeF = new System.Drawing.SizeF(849F, 35F);
            this.detailTable.StylePriority.UseFont = false;
            this.detailTable.StylePriority.UsePadding = false;
            // 
            // detailTableRow
            // 
            this.detailTableRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.productName,
            this.quantity,
            this.xrTableCell4,
            this.unitPrice,
            this.xrTableCell3,
            this.lineTotal});
            this.detailTableRow.Name = "detailTableRow";
            this.detailTableRow.Weight = 12.343333333333334D;
            // 
            // productName
            // 
            this.productName.Name = "productName";
            this.productName.StylePriority.UsePadding = false;
            this.productName.StylePriority.UseTextAlignment = false;
            this.productName.Text = "[OrderSerial]";
            this.productName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.productName.Weight = 0.83415506437496911D;
            // 
            // quantity
            // 
            this.quantity.Name = "quantity";
            this.quantity.StylePriority.UsePadding = false;
            this.quantity.StylePriority.UseTextAlignment = false;
            this.quantity.Text = "[CustomerName]";
            this.quantity.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.quantity.Weight = 0.73605107795775171D;
            // 
            // xrTableCell4
            // 
            this.xrTableCell4.Multiline = true;
            this.xrTableCell4.Name = "xrTableCell4";
            this.xrTableCell4.StylePriority.UsePadding = false;
            this.xrTableCell4.StylePriority.UseTextAlignment = false;
            this.xrTableCell4.Text = "[CustomerTele]";
            this.xrTableCell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell4.Weight = 0.51285495749616261D;
            // 
            // unitPrice
            // 
            this.unitPrice.Name = "unitPrice";
            this.unitPrice.StylePriority.UsePadding = false;
            this.unitPrice.StylePriority.UseTextAlignment = false;
            this.unitPrice.Text = "[CountryName]";
            this.unitPrice.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.unitPrice.TextFormatString = "{0:$0.00}";
            this.unitPrice.Weight = 0.5128539831012292D;
            // 
            // xrTableCell3
            // 
            this.xrTableCell3.Multiline = true;
            this.xrTableCell3.Name = "xrTableCell3";
            this.xrTableCell3.StylePriority.UsePadding = false;
            this.xrTableCell3.StylePriority.UseTextAlignment = false;
            this.xrTableCell3.Text = "[OrderDate!dd-MM-yy]";
            this.xrTableCell3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell3.Weight = 0.46652698322202246D;
            // 
            // lineTotal
            // 
            this.lineTotal.CanShrink = true;
            this.lineTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(102)))), ((int)(((byte)(78)))));
            this.lineTotal.Name = "lineTotal";
            this.lineTotal.StylePriority.UseFont = false;
            this.lineTotal.StylePriority.UseForeColor = false;
            this.lineTotal.StylePriority.UsePadding = false;
            this.lineTotal.StylePriority.UseTextAlignment = false;
            this.lineTotal.Text = "[OrderTotal]";
            this.lineTotal.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lineTotal.TextFormatString = "{0:$0.00}";
            this.lineTotal.Weight = 0.3412179908503461D;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 70.08333F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.StylePriority.UseBackColor = false;
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.thankYouLabel,
            this.heartLabel});
            this.BottomMargin.HeightF = 50.20831F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // thankYouLabel
            // 
            this.thankYouLabel.CanGrow = false;
            this.thankYouLabel.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.thankYouLabel.LocationFloat = new DevExpress.Utils.PointFloat(84.58331F, 10.00001F);
            this.thankYouLabel.Name = "thankYouLabel";
            this.thankYouLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.thankYouLabel.SizeF = new System.Drawing.SizeF(110.4167F, 40F);
            this.thankYouLabel.StylePriority.UseFont = false;
            this.thankYouLabel.StylePriority.UseTextAlignment = false;
            this.thankYouLabel.Text = "Thank you!";
            this.thankYouLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // heartLabel
            // 
            this.heartLabel.CanGrow = false;
            this.heartLabel.Font = new System.Drawing.Font("Segoe UI", 24F);
            this.heartLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(102)))), ((int)(((byte)(78)))));
            this.heartLabel.LocationFloat = new DevExpress.Utils.PointFloat(45.04047F, 10.00001F);
            this.heartLabel.Name = "heartLabel";
            this.heartLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.heartLabel.SizeF = new System.Drawing.SizeF(39.54286F, 40F);
            this.heartLabel.StylePriority.UseFont = false;
            this.heartLabel.StylePriority.UseForeColor = false;
            this.heartLabel.StylePriority.UseTextAlignment = false;
            this.heartLabel.Text = "♥";
            this.heartLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // GroupHeader2
            // 
            this.GroupHeader2.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.vendorLogo,
            this.invoiceLabel});
            this.GroupHeader2.GroupFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("InvoiceNumber", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            this.GroupHeader2.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
            this.GroupHeader2.HeightF = 98.45839F;
            this.GroupHeader2.Level = 1;
            this.GroupHeader2.Name = "GroupHeader2";
            this.GroupHeader2.StyleName = "baseControlStyle";
            this.GroupHeader2.StylePriority.UseBackColor = false;
            // 
            // vendorLogo
            // 
            this.vendorLogo.ImageAlignment = DevExpress.XtraPrinting.ImageAlignment.TopLeft;
            this.vendorLogo.ImageSource = new DevExpress.XtraPrinting.Drawing.ImageSource("img", resources.GetString("vendorLogo.ImageSource"));
            this.vendorLogo.LocationFloat = new DevExpress.Utils.PointFloat(689F, 0F);
            this.vendorLogo.Name = "vendorLogo";
            this.vendorLogo.SizeF = new System.Drawing.SizeF(150F, 71F);
            this.vendorLogo.Sizing = DevExpress.XtraPrinting.ImageSizeMode.Squeeze;
            this.vendorLogo.StylePriority.UseBorders = false;
            this.vendorLogo.StylePriority.UsePadding = false;
            // 
            // invoiceLabel
            // 
            this.invoiceLabel.Font = new System.Drawing.Font("Segoe UI", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.invoiceLabel.LocationFloat = new DevExpress.Utils.PointFloat(9.999998F, 10.00001F);
            this.invoiceLabel.Name = "invoiceLabel";
            this.invoiceLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.invoiceLabel.SizeF = new System.Drawing.SizeF(185F, 50F);
            this.invoiceLabel.StylePriority.UseFont = false;
            this.invoiceLabel.Text = "Financial";
            // 
            // GroupFooter1
            // 
            this.GroupFooter1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.subtotalTable});
            this.GroupFooter1.HeightF = 12.00001F;
            this.GroupFooter1.Name = "GroupFooter1";
            this.GroupFooter1.StyleName = "baseControlStyle";
            // 
            // subtotalTable
            // 
            this.subtotalTable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(201)))), ((int)(((byte)(194)))));
            this.subtotalTable.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.subtotalTable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(86)))), ((int)(((byte)(85)))));
            this.subtotalTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 10.00001F);
            this.subtotalTable.Name = "subtotalTable";
            this.subtotalTable.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.subtotalTable.SizeF = new System.Drawing.SizeF(650F, 2F);
            this.subtotalTable.StylePriority.UseBorderColor = false;
            this.subtotalTable.StylePriority.UseFont = false;
            this.subtotalTable.StylePriority.UseForeColor = false;
            this.subtotalTable.StylePriority.UsePadding = false;
            // 
            // GroupHeader1
            // 
            this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.headerTable});
            this.GroupHeader1.HeightF = 50F;
            this.GroupHeader1.Name = "GroupHeader1";
            this.GroupHeader1.RepeatEveryPage = true;
            this.GroupHeader1.StyleName = "baseControlStyle";
            // 
            // headerTable
            // 
            this.headerTable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(201)))), ((int)(((byte)(194)))));
            this.headerTable.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
            this.headerTable.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.headerTable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(86)))), ((int)(((byte)(85)))));
            this.headerTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.headerTable.Name = "headerTable";
            this.headerTable.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.headerTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.headerTableRow});
            this.headerTable.SizeF = new System.Drawing.SizeF(849F, 32F);
            this.headerTable.StylePriority.UseBorderColor = false;
            this.headerTable.StylePriority.UseBorders = false;
            this.headerTable.StylePriority.UseFont = false;
            this.headerTable.StylePriority.UseForeColor = false;
            this.headerTable.StylePriority.UsePadding = false;
            // 
            // headerTableRow
            // 
            this.headerTableRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.productNameCaption,
            this.quantityCaption,
            this.unitPriceCaption,
            this.xrTableCell2,
            this.xrTableCell1,
            this.lineTotalCaptionCell});
            this.headerTableRow.Name = "headerTableRow";
            this.headerTableRow.Weight = 11.5D;
            // 
            // productNameCaption
            // 
            this.productNameCaption.Name = "productNameCaption";
            this.productNameCaption.StylePriority.UsePadding = false;
            this.productNameCaption.StylePriority.UseTextAlignment = false;
            this.productNameCaption.Text = "Order Serial";
            this.productNameCaption.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.productNameCaption.Weight = 0.96839863950238059D;
            // 
            // quantityCaption
            // 
            this.quantityCaption.Name = "quantityCaption";
            this.quantityCaption.StylePriority.UseTextAlignment = false;
            this.quantityCaption.Text = "Customer";
            this.quantityCaption.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.quantityCaption.Weight = 0.58474621416105155D;
            // 
            // unitPriceCaption
            // 
            this.unitPriceCaption.Name = "unitPriceCaption";
            this.unitPriceCaption.StylePriority.UseTextAlignment = false;
            this.unitPriceCaption.Text = "Customer Phone";
            this.unitPriceCaption.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.unitPriceCaption.Weight = 0.5072818773417197D;
            // 
            // xrTableCell2
            // 
            this.xrTableCell2.Multiline = true;
            this.xrTableCell2.Name = "xrTableCell2";
            this.xrTableCell2.StylePriority.UseTextAlignment = false;
            this.xrTableCell2.Text = "Country";
            this.xrTableCell2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell2.Weight = 0.5072818773417197D;
            // 
            // xrTableCell1
            // 
            this.xrTableCell1.Multiline = true;
            this.xrTableCell1.Name = "xrTableCell1";
            this.xrTableCell1.StylePriority.UseTextAlignment = false;
            this.xrTableCell1.Text = "Order Date";
            this.xrTableCell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell1.Weight = 0.46145788154140344D;
            // 
            // lineTotalCaptionCell
            // 
            this.lineTotalCaptionCell.Name = "lineTotalCaptionCell";
            this.lineTotalCaptionCell.StylePriority.UseTextAlignment = false;
            this.lineTotalCaptionCell.Text = "TOTAL";
            this.lineTotalCaptionCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lineTotalCaptionCell.Weight = 0.337510386369754D;
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataSource = typeof(global::CRM.ViewModels.FinancialVm);
            this.objectDataSource1.Name = "objectDataSource1";
            // 
            // baseControlStyle
            // 
            this.baseControlStyle.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.baseControlStyle.Name = "baseControlStyle";
            this.baseControlStyle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            // 
            // Financialrpt
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.GroupHeader2,
            this.GroupFooter1,
            this.GroupHeader1});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.objectDataSource1});
            this.DataSource = this.objectDataSource1;
            this.Font = new System.Drawing.Font("Arial", 9.75F);
            this.Margins = new System.Drawing.Printing.Margins(0, 1, 70, 50);
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.baseControlStyle});
            this.Version = "22.1";
            ((System.ComponentModel.ISupportInitialize)(this.detailTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.subtotalTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.headerTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRTable detailTable;
        private DevExpress.XtraReports.UI.XRTableRow detailTableRow;
        private DevExpress.XtraReports.UI.XRTableCell productName;
        private DevExpress.XtraReports.UI.XRTableCell quantity;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell4;
        private DevExpress.XtraReports.UI.XRTableCell unitPrice;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell3;
        private DevExpress.XtraReports.UI.XRTableCell lineTotal;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRLabel thankYouLabel;
        private DevExpress.XtraReports.UI.XRLabel heartLabel;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader2;
        private DevExpress.XtraReports.UI.XRPictureBox vendorLogo;
        private DevExpress.XtraReports.UI.XRLabel invoiceLabel;
        private DevExpress.XtraReports.UI.GroupFooterBand GroupFooter1;
        private DevExpress.XtraReports.UI.XRTable subtotalTable;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
        private DevExpress.XtraReports.UI.XRTable headerTable;
        private DevExpress.XtraReports.UI.XRTableRow headerTableRow;
        private DevExpress.XtraReports.UI.XRTableCell productNameCaption;
        private DevExpress.XtraReports.UI.XRTableCell quantityCaption;
        private DevExpress.XtraReports.UI.XRTableCell unitPriceCaption;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell2;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell1;
        private DevExpress.XtraReports.UI.XRTableCell lineTotalCaptionCell;
        private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource1;
        private DevExpress.XtraReports.UI.XRControlStyle baseControlStyle;
    }
}
