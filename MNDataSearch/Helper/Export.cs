using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Imaging;
using FluxJpeg.Core;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Collections;
using Microsoft.Win32;

namespace MNDataSearch.Helper
{
    /// <summary>
    ///  A Helper class created to Export any UIElement or Data Grid in various formats like: JPG or PDF or CSV 
    /// <remarks>Created By Viral Rathod As on 27th Jan 2012</remarks>
    /// </summary>
    public static class Export
    {
        /// <summary>
        /// Save Any UIElement as jpg image
        /// </summary>
        /// <param name="uc">UIElement to pass</param>
        public static void SaveToJPG(UIElement uc, bool IsFull = false)
        {
            DataGrid dg = new DataGrid();
            double temp_ActualHeight = double.NaN;
            double temp_ActualWidth = double.NaN;
            Thickness temp_Margin = new Thickness(0, 0, 0, 0);
            ScrollBarVisibility temp_ActualHorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            try
            {
                WriteableBitmap bitmap = new WriteableBitmap(uc, null);

                if (bitmap != null)
                {
                    SaveFileDialog saveDlg = new SaveFileDialog();
                    saveDlg.Filter = "JPEG Files (*.jpeg)|*.jpeg";
                    saveDlg.DefaultExt = ".jpeg";

                    if ((bool)saveDlg.ShowDialog())
                    {
                        using (Stream fs = saveDlg.OpenFile())
                        {
                            MemoryStream stream;
                            if (IsFull)
                            {
                                if (uc is DataGrid)
                                {
                                    dg = uc as DataGrid;

                                    //Backup Original Data
                                    temp_ActualHeight = dg.Height.ToString() == double.NaN.ToString() ? double.NaN : dg.ActualHeight;
                                    temp_ActualWidth = dg.Width.ToString() == double.NaN.ToString() ? double.NaN : dg.ActualWidth;
                                    temp_Margin = dg.Margin;
                                    temp_ActualHorizontalScrollBarVisibility = dg.HorizontalScrollBarVisibility;

                                    int RowCount = 0; //Number of Rows in a Datagrid
                                    double Total_Width = 0; //Total Width of a Datagrid
                                    foreach (object data in dg.ItemsSource) RowCount++;
                                    foreach (DataGridColumn dgc in dg.Columns) Total_Width += dgc.ActualWidth;

                                    //Setting control for Proper GUI
                                    dg.Margin = new Thickness(-100, temp_Margin.Top, -2000, -25 * RowCount);
                                    dg.Width = Total_Width;
                                    dg.Height = double.NaN;
                                    dg.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                                    dg.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                                    dg.UpdateLayout();

                                    stream = GetImageStream(new WriteableBitmap(dg, null));
                                }
                                else
                                    stream = GetImageStream(bitmap);
                            }
                            else
                            {
                                stream = GetImageStream(bitmap);
                            }
                            //Get Bytes from memory stream and write into IO stream
                            byte[] binaryData = new Byte[stream.Length];
                            long bytesRead = stream.Read(binaryData, 0, (int)stream.Length);
                            fs.Write(binaryData, 0, binaryData.Length);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine("Note: Please make sure that Height and Width of the chart is set properly.");
                //System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (uc is DataGrid)
                {
                    dg.Height = temp_ActualHeight;
                    dg.Width = temp_ActualWidth;
                    dg.HorizontalScrollBarVisibility = temp_ActualHorizontalScrollBarVisibility;
                    dg.VerticalScrollBarVisibility = temp_ActualHorizontalScrollBarVisibility;
                    dg.Margin = temp_Margin;
                    dg.UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Save Any UserControl as PDF
        /// </summary>
        /// <param name="uc">UIElement</param>
        public static void SaveToPDF(UIElement uc, bool IsHeaderForPDF, bool IsFooterForPDF, string strHeader, double paramColumnHeaderHeight = 40, double paramRowHeight = 25)
        {
            try
            {
                SaveFileDialog saveDlg = new SaveFileDialog();
                saveDlg.Filter = "PDF Files (*.pdf)|*.pdf";
                saveDlg.DefaultExt = ".pdf";
                int RowCount = 0;
                double Total_Width = 0;
                if (String.IsNullOrEmpty(strHeader)) strHeader = "";

                if ((bool)saveDlg.ShowDialog())
                {
                    PdfDocument document = new PdfDocument();
                    MemoryStream mstream;
                    DataGrid dg = new DataGrid();
                    double temp_ActualHeight = double.NaN;
                    double temp_ActualWidth = double.NaN;
                    double temp_ColumnHeaderHeight = double.NaN;
                    double temp_RowHeight = double.NaN;
                    Thickness temp_Margin = new Thickness(0, 0, 0, 0);
                    ScrollBarVisibility temp_ActualHorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

                    try
                    {
                        if (uc is DataGrid)
                        {
                            dg = uc as DataGrid;
                            //dg.IsEnabled = false;

                            foreach (object data in dg.ItemsSource) RowCount++;
                            foreach (DataGridColumn dgc in dg.Columns) Total_Width += dgc.ActualWidth;
                            int Rows_Per_Page = 40; // Total rows to show Per Page

                            for (int cnt = 1; cnt <= Math.Ceiling(RowCount / Rows_Per_Page) + 1; cnt++)
                            {
                                int Remaining = RowCount - ((cnt - 1) * Rows_Per_Page);
                                if (Remaining < 1) break;

                                PdfPage inner_page = document.AddPage();
                                XGraphics inner_gfx1 = XGraphics.FromPdfPage(inner_page);

                                if (cnt == 1)
                                {
                                    //Backup Original Data
                                    temp_ActualHeight = dg.Height.ToString() == double.NaN.ToString() ? double.NaN : dg.ActualHeight;
                                    temp_ActualWidth = dg.Width.ToString() == double.NaN.ToString() ? double.NaN : dg.ActualWidth;
                                    temp_ColumnHeaderHeight = dg.ColumnHeaderHeight;
                                    temp_RowHeight = dg.RowHeight;
                                    temp_Margin = dg.Margin;
                                    temp_ActualHorizontalScrollBarVisibility = dg.HorizontalScrollBarVisibility;

                                    //Setting control for Proper GUI
                                    dg.Margin = new Thickness(-100, temp_Margin.Top, -2000, temp_Margin.Bottom);
                                    dg.Width = Total_Width;
                                    inner_page.Width = (Total_Width > inner_page.Width.Presentation) ? new XUnit(Total_Width + 25, XGraphicsUnit.Presentation) : inner_page.Width;
                                    dg.Height = inner_page.Height + 170;
                                    dg.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                                    dg.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                                    dg.ColumnHeaderHeight = paramColumnHeaderHeight;
                                    dg.RowHeight = paramRowHeight;
                                    dg.UpdateLayout();

                                    //Total_Row to show depends upon:   dg.Height=  (Total_Row x dg.RowHeight ) + dg.ColumnHeaderHeight 
                                    Rows_Per_Page = Convert.ToInt32((dg.Height - dg.ColumnHeaderHeight) / dg.RowHeight);

                                }
                                else if (cnt > 1)
                                {
                                    inner_page.Width = (Total_Width > inner_page.Width.Presentation) ? new XUnit(Total_Width + 30, XGraphicsUnit.Presentation) : inner_page.Width;

                                    if (Remaining >= Rows_Per_Page)
                                    {
                                        dg.SelectedIndex = RowCount - 1;
                                        dg.UpdateLayout();
                                        dg.ScrollIntoView(dg.SelectedItem, dg.Columns[dg.Columns.Count - 1]);
                                        dg.UpdateLayout();
                                        dg.SelectedIndex = (cnt - 1) * Rows_Per_Page;
                                        dg.UpdateLayout();
                                    }
                                    else if (Remaining < Rows_Per_Page)
                                    {
                                        dg.Height = (Remaining * dg.RowHeight) + dg.ColumnHeaderHeight;
                                        dg.SelectedIndex = RowCount - 1;
                                        dg.UpdateLayout();
                                    }

                                    if (dg.SelectedIndex >= RowCount) dg.SelectedIndex = RowCount - 1;

                                    dg.ScrollIntoView(dg.SelectedItem, dg.Columns[dg.Columns.Count - 1]);
                                }

                                if (IsHeaderForPDF)
                                {
                                    mstream = GetImageStream(new WriteableBitmap(new UC_CCG_Header(), null));
                                    inner_gfx1.DrawImage(XImage.FromStream(mstream), 5, 5);
                                    XPen line = new XPen(XColor.FromKnownColor(XKnownColor.Gray), 0.5);
                                    XBrush brush = new XSolidBrush(XColor.FromKnownColor(XKnownColor.Gray));
                                    inner_gfx1.DrawLine(line, new Point(70, 30), new Point(inner_page.Width - 10, 30));
                                    inner_gfx1.DrawString(strHeader, new XFont("Arial", 12), brush, new XPoint(70, 27));
                                }

                                dg.UpdateLayout();
                                mstream = GetImageStream(new WriteableBitmap(dg, null));
                                inner_gfx1.DrawImage(XImage.FromStream(mstream), 10, 50);

                                if (IsFooterForPDF)
                                {
                                    XPen line = new XPen(XColor.FromKnownColor(XKnownColor.Gray), 0.5);
                                    XBrush brush = new XSolidBrush(XColor.FromKnownColor(XKnownColor.Gray));
                                    inner_gfx1.DrawLine(line, new Point(5, inner_page.Height - 20), new Point(inner_page.Width - 10, inner_page.Height - 20));
                                    inner_gfx1.DrawString("(c) Copyright 2012 Clear Cell All Rights Reserved.", new XFont("Arial", 8), brush, new XPoint((inner_page.Width / 2) - 100, inner_page.Height - 10));//©
                                }
                            }
                        }
                        else
                        {
                            PdfPage inner_page = document.AddPage();
                            XGraphics inner_gfx1 = XGraphics.FromPdfPage(inner_page);

                            mstream = GetImageStream(new WriteableBitmap(uc, null));
                            XImage xImg = XImage.FromStream(mstream);
                            double temp_Width = xImg.PixelWidth;
                            inner_page.Width = (temp_Width > inner_page.Width.Presentation) ? new XUnit(temp_Width + 30, XGraphicsUnit.Presentation) : inner_page.Width;


                            if (IsHeaderForPDF)
                            {
                                mstream = GetImageStream(new WriteableBitmap(new UC_CCG_Header(), null));
                                inner_gfx1.DrawImage(XImage.FromStream(mstream), 5, 5);
                                XPen line = new XPen(XColor.FromKnownColor(XKnownColor.Gray), 0.5);
                                XBrush brush = new XSolidBrush(XColor.FromKnownColor(XKnownColor.Gray));
                                inner_gfx1.DrawLine(line, new Point(70, 30), new Point(inner_page.Width - 10, 30));
                                inner_gfx1.DrawString(strHeader, new XFont("Arial", 12), brush, new XPoint(70, 27));
                            }

                            inner_gfx1.DrawImage(xImg, 10, 50);

                            if (IsFooterForPDF)
                            {
                                XPen line = new XPen(XColor.FromKnownColor(XKnownColor.Gray), 0.5);
                                XBrush brush = new XSolidBrush(XColor.FromKnownColor(XKnownColor.Gray));
                                inner_gfx1.DrawLine(line, new Point(5, inner_page.Height - 20), new Point(inner_page.Width - 10, inner_page.Height - 20));
                                inner_gfx1.DrawString("(c) Copyright 2012 Clear Cell All Rights Reserved.", new XFont("Arial", 8), brush, new XPoint((inner_page.Width / 2) - 100, inner_page.Height - 10));//©
                            }
                        }
                        document.Save(saveDlg.OpenFile());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Issue occurred while Exporting PDF: " + ex.Message);
                    }
                    finally
                    {
                        if (uc is DataGrid)
                        {
                            dg.Height = temp_ActualHeight;
                            dg.Width = temp_ActualWidth;
                            dg.HorizontalScrollBarVisibility = temp_ActualHorizontalScrollBarVisibility;
                            dg.VerticalScrollBarVisibility = temp_ActualHorizontalScrollBarVisibility;
                            dg.Margin = temp_Margin;
                            dg.ColumnHeaderHeight = temp_ColumnHeaderHeight;
                            dg.RowHeight = temp_RowHeight;
                            //dg.IsEnabled = true;
                            dg.UpdateLayout();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Issue occurred while Exporting PDF: " + ex.Message);
            }
        }

        /// <summary>
        /// To Save the CSV format of and data of DataGrid passed as a parameter
        /// </summary>
        /// <param name="dg">DataGrid</param> 
        public static void SaveToCSV(DataGrid dg)
        {
            SaveFileDialog objSFD = new SaveFileDialog()
            {
                DefaultExt = "csv",
                Filter = "CSV Files (*.csv)|*.csv|Excel XML (*.xml)|*.xml|All files (*.*)|*.*",
                FilterIndex = 1
            };
            objSFD.Filter = "CSV Files (*.csv)|*.csv";
            try
            {
                if (dg.Columns.Count > 0)
                {
                    if (objSFD.ShowDialog() == true)
                    {
                        string strFormat = objSFD.SafeFileName.Substring(objSFD.SafeFileName.IndexOf('.') + 1).ToUpper();
                        StringBuilder strBuilder = new StringBuilder();
                        if (dg.ItemsSource == null) return;
                        List<string> lstFields = new List<string>();
                        if (dg.HeadersVisibility == DataGridHeadersVisibility.Column || dg.HeadersVisibility == DataGridHeadersVisibility.All)
                        {
                            foreach (DataGridColumn dgcol in dg.Columns)
                                lstFields.Add(FormatField(dgcol.Header.ToString(), strFormat));
                            BuildStringOfRow(strBuilder, lstFields, strFormat);
                        }
                        foreach (object data in dg.ItemsSource)
                        {
                            lstFields.Clear();
                            foreach (DataGridColumn col in dg.Columns)
                            {
                                string strValue = "";
                                BindingBase objBinding = null;
                                if (col is DataGridBoundColumn)
                                    objBinding = (col as DataGridBoundColumn).Binding;
                                if (col is DataGridTemplateColumn)
                                {
                                    //This is a template column... let us see the underlying dependency object
                                    DependencyObject objDO = (col as DataGridTemplateColumn).CellTemplate.LoadContent();
                                    FrameworkElement oFE = (FrameworkElement)objDO;
                                    FieldInfo oFI = oFE.GetType().GetField("TextProperty");
                                    if (oFI != null)
                                    {
                                        if (oFI.GetValue(null) != null)
                                        {
                                            if (oFE.GetBindingExpression((DependencyProperty)oFI.GetValue(null)) != null)
                                                objBinding = oFE.GetBindingExpression((DependencyProperty)oFI.GetValue(null)).ParentBinding;
                                        }
                                    }
                                }

                                if (objBinding != null)
                                {
                                    if (objBinding.BindingGroupName != "")
                                    {
                                        PropertyInfo pi = data.GetType().GetProperty(objBinding.BindingGroupName);
                                        if (pi != null) strValue = pi.GetValue(data, null).ToString();
                                    }
                                    //if (objBinding.Converter != null)
                                    //{
                                    //    try
                                    //    {
                                    //        if (strValue != "")
                                    //        {
                                    //            try
                                    //            {
                                    //                strValue = objBinding.Converter.Convert(strValue, typeof(string), objBinding.ConverterParameter, objBinding.ConverterCulture).ToString();
                                    //            }
                                    //            catch (Exception)
                                    //            {
                                    //                strValue = objBinding.Converter.Convert(data, typeof(string), objBinding.ConverterParameter, objBinding.ConverterCulture).ToString();
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            strValue = objBinding.Converter.Convert(data, typeof(string), objBinding.ConverterParameter, objBinding.ConverterCulture).ToString();
                                    //        }
                                    //    }
                                    //    catch (Exception)
                                    //    {
                                    //        strValue = "-";
                                    //    }
                                    //    strValue = RemoveSpecialCharacters(strValue);
                                    //}
                                }
                                lstFields.Add(FormatField(strValue, strFormat));
                            }
                            BuildStringOfRow(strBuilder, lstFields, strFormat);
                        }
                        StreamWriter sw = new StreamWriter(objSFD.OpenFile());

                        sw.Write(strBuilder.ToString());

                        sw.Close();
                    }
                }
                else
                {
                    MessageBox.Show("No Data Selected");
                    System.Diagnostics.Debug.WriteLine("No Data Selected");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Issue occurred while Exporting CSV: " + ex.Message);
            }
        }

        /// <summary>
        /// Save List of  UserControl as PDF
        /// </summary>
        /// <param name="uc">UIElement</param>
        public static void SaveMoreControlsToPDF(List<UIElement> ucs, bool IsHeaderForPDF, bool IsFooterForPDF, string strHeader, double paramColumnHeaderHeight = 40, double paramRowHeight = 25)
        {
            try
            {
                SaveFileDialog saveDlg = new SaveFileDialog();
                saveDlg.Filter = "PDF Files (*.pdf)|*.pdf";
                saveDlg.DefaultExt = ".pdf";
                int RowCount = 0;
                double Total_Width = 0;
                if (String.IsNullOrEmpty(strHeader)) strHeader = "";

                if ((bool)saveDlg.ShowDialog())
                {
                    PdfDocument document = new PdfDocument();
                    MemoryStream mstream;

                    foreach (UIElement uc in ucs)
                    {
                        DataGrid dg = new DataGrid();
                        double temp_ActualHeight = double.NaN;
                        double temp_ActualWidth = double.NaN;
                        double temp_ColumnHeaderHeight = double.NaN;
                        double temp_RowHeight = double.NaN;
                        Thickness temp_Margin = new Thickness(0, 0, 0, 0);
                        ScrollBarVisibility temp_ActualHorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

                        try
                        {
                            if (uc is DataGrid)
                            {
                                dg = uc as DataGrid;
                                //dg.IsEnabled = false;

                                foreach (object data in dg.ItemsSource) RowCount++;
                                foreach (DataGridColumn dgc in dg.Columns) Total_Width += dgc.ActualWidth;
                                int Rows_Per_Page = 40; // Total rows to show Per Page

                                for (int cnt = 1; cnt <= Math.Ceiling((double)RowCount / Rows_Per_Page) + 1; cnt++)
                                {
                                    int Remaining = RowCount - ((cnt - 1) * Rows_Per_Page);
                                    if (Remaining < 1) break;

                                    PdfPage inner_page = document.AddPage();
                                    XGraphics inner_gfx1 = XGraphics.FromPdfPage(inner_page);

                                    if (cnt == 1)
                                    {
                                        //Backup Original Data
                                        temp_ActualHeight = dg.Height.ToString() == double.NaN.ToString() ? double.NaN : dg.ActualHeight;
                                        temp_ActualWidth = dg.Width.ToString() == double.NaN.ToString() ? double.NaN : dg.ActualWidth;
                                        temp_ColumnHeaderHeight = dg.ColumnHeaderHeight;
                                        temp_RowHeight = dg.RowHeight;
                                        temp_Margin = dg.Margin;
                                        temp_ActualHorizontalScrollBarVisibility = dg.HorizontalScrollBarVisibility;

                                        //Setting control for Proper GUI
                                        dg.Margin = new Thickness(-100, temp_Margin.Top, -2000, temp_Margin.Bottom);
                                        dg.Width = Total_Width;
                                        inner_page.Width = (Total_Width > inner_page.Width.Presentation) ? new XUnit(Total_Width + 25, XGraphicsUnit.Presentation) : inner_page.Width;
                                        dg.Height = inner_page.Height + 170;
                                        dg.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                                        dg.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                                        dg.ColumnHeaderHeight = paramColumnHeaderHeight;
                                        dg.RowHeight = paramRowHeight;
                                        dg.UpdateLayout();

                                        //Total_Row to show depends upon:   dg.Height=  (Total_Row x dg.RowHeight ) + dg.ColumnHeaderHeight 
                                        Rows_Per_Page = Convert.ToInt32((dg.Height - dg.ColumnHeaderHeight) / dg.RowHeight);

                                    }
                                    else if (cnt > 1)
                                    {
                                        inner_page.Width = (Total_Width > inner_page.Width.Presentation) ? new XUnit(Total_Width + 30, XGraphicsUnit.Presentation) : inner_page.Width;

                                        if (Remaining >= Rows_Per_Page)
                                        {
                                            dg.SelectedIndex = RowCount - 1;
                                            dg.UpdateLayout();
                                            dg.ScrollIntoView(dg.SelectedItem, dg.Columns[dg.Columns.Count - 1]);
                                            dg.UpdateLayout();
                                            dg.SelectedIndex = (cnt - 1) * Rows_Per_Page;
                                            dg.UpdateLayout();
                                        }
                                        else if (Remaining < Rows_Per_Page)
                                        {
                                            dg.Height = (Remaining * dg.RowHeight) + dg.ColumnHeaderHeight;
                                            dg.SelectedIndex = RowCount - 1;
                                            dg.UpdateLayout();
                                        }

                                        if (dg.SelectedIndex >= RowCount) dg.SelectedIndex = RowCount - 1;

                                        dg.ScrollIntoView(dg.SelectedItem, dg.Columns[dg.Columns.Count - 1]);
                                    }

                                    if (IsHeaderForPDF)
                                    {
                                        mstream = GetImageStream(new WriteableBitmap(new UC_CCG_Header(), null));
                                        inner_gfx1.DrawImage(XImage.FromStream(mstream), 5, 5);
                                        XPen line = new XPen(XColor.FromKnownColor(XKnownColor.Gray), 0.5);
                                        XBrush brush = new XSolidBrush(XColor.FromKnownColor(XKnownColor.Gray));
                                        inner_gfx1.DrawLine(line, new XPoint(70, 30), new XPoint(inner_page.Width - 10, 30));
                                        inner_gfx1.DrawString(strHeader, new XFont("Arial", 12), brush, new XPoint(70, 27));
                                    }

                                    dg.UpdateLayout();
                                    mstream = GetImageStream(new WriteableBitmap(dg, null));
                                    inner_gfx1.DrawImage(XImage.FromStream(mstream), 10, 50);

                                    if (IsFooterForPDF)
                                    {
                                        XPen line = new XPen(XColor.FromKnownColor(XKnownColor.Gray), 0.5);
                                        XBrush brush = new XSolidBrush(XColor.FromKnownColor(XKnownColor.Gray));
                                        inner_gfx1.DrawLine(line, new Point(5, inner_page.Height - 20), new Point(inner_page.Width - 10, inner_page.Height - 20));
                                        inner_gfx1.DrawString("(c) Copyright 2012 Clear Cell All Rights Reserved.", new XFont("Arial", 8), brush, new XPoint((inner_page.Width / 2) - 100, inner_page.Height - 10));//©
                                    }
                                }
                            }
                            else
                            {
                                PdfPage inner_page = document.AddPage();
                                XGraphics inner_gfx1 = XGraphics.FromPdfPage(inner_page);
                                mstream = GetImageStream(new WriteableBitmap(uc, null));
                                XImage xImg = XImage.FromStream(mstream);
                                double temp_Width = xImg.PixelWidth;
                                inner_page.Width = (temp_Width > inner_page.Width.Presentation) ? new XUnit(temp_Width + 30, XGraphicsUnit.Presentation) : inner_page.Width;

                                if (IsHeaderForPDF)
                                {
                                    mstream = GetImageStream(new WriteableBitmap(new UC_CCG_Header(), null));
                                    inner_gfx1.DrawImage(XImage.FromStream(mstream), 5, 5);
                                    XPen line = new XPen(XColor.FromKnownColor(XKnownColor.Gray), 0.5);
                                    XBrush brush = new XSolidBrush(XColor.FromKnownColor(XKnownColor.Gray));
                                    inner_gfx1.DrawLine(line, new Point(70, 30), new Point(inner_page.Width - 10, 30));
                                    inner_gfx1.DrawString(strHeader, new XFont("Arial", 12), brush, new XPoint(70, 27));
                                }

                                inner_gfx1.DrawImage(xImg, 10, 50);

                                if (IsFooterForPDF)
                                {
                                    XPen line = new XPen(XColor.FromKnownColor(XKnownColor.Gray), 0.5);
                                    XBrush brush = new XSolidBrush(XColor.FromKnownColor(XKnownColor.Gray));
                                    inner_gfx1.DrawLine(line, new Point(5, inner_page.Height - 20), new Point(inner_page.Width - 10, inner_page.Height - 20));
                                    inner_gfx1.DrawString("(c) Copyright 2012 Clear Cell All Rights Reserved.", new XFont("Arial", 8), brush, new XPoint((inner_page.Width / 2) - 100, inner_page.Height - 10));//©
                                }
                            }
                        }
                        catch (Exception excep)
                        {
                            MessageBox.Show("Issue occurred while Exporting multiple controls into PDF: " + excep.Message);
                        }
                        finally
                        {
                            if (uc is DataGrid)
                            {
                                dg.Height = temp_ActualHeight;
                                dg.Width = temp_ActualWidth;
                                dg.HorizontalScrollBarVisibility = temp_ActualHorizontalScrollBarVisibility;
                                dg.VerticalScrollBarVisibility = temp_ActualHorizontalScrollBarVisibility;
                                dg.Margin = temp_Margin;
                                dg.ColumnHeaderHeight = temp_ColumnHeaderHeight;
                                dg.RowHeight = temp_RowHeight;
                                //dg.IsEnabled = true;
                                dg.UpdateLayout();
                            }
                        }
                    }
                    document.Save(saveDlg.OpenFile());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Issue occurred while Exporting multiple controls into PDF: " + ex.Message);
                //System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// To Remove Special Characters
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        private static string RemoveSpecialCharacters(string strValue)
        {
            strValue = strValue.Replace("₹", "");
            strValue = strValue.Replace("$", "");
            return strValue;
        }

        /// <summary>
        /// Get image MemoryStream from WriteableBitmap
        /// </summary>
        /// <param name="bitmap">WriteableBitmap</param>
        /// <returns>MemoryStream</returns>
        private static MemoryStream GetImageStream(WriteableBitmap bitmap)
        {
            byte[][,] raster = ReadRasterInformation(bitmap);
            return EncodeRasterInformationToStream(raster, ColorSpace.RGB);
        }

        /// <summary>
        /// Reads raster information from WriteableBitmap
        /// </summary>
        /// <param name="bitmap">WriteableBitmap</param>
        /// <returns>Array of bytes</returns>
        private static byte[][,] ReadRasterInformation(WriteableBitmap bitmap)
        {
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int bands = 3;
            byte[][,] raster = new byte[bands][,];

            for (int i = 0; i < bands; i++)
            {
                raster[i] = new byte[width, height];
            }

            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    int pixel = bitmap.Pixels[width * row + column];
                    raster[0][column, row] = (byte)(pixel >> 16);
                    raster[1][column, row] = (byte)(pixel >> 8);
                    raster[2][column, row] = (byte)pixel;
                }
            }

            return raster;
        }

        /// <summary>
        /// Encode raster information to MemoryStream
        /// </summary>
        /// <param name="raster">Raster information (Array of bytes)</param>
        /// <param name="colorSpace">ColorSpace used</param>
        /// <returns>MemoryStream</returns>
        private static MemoryStream EncodeRasterInformationToStream(byte[][,] raster, ColorSpace colorSpace)
        {
            ColorModel model = new ColorModel { colorspace = ColorSpace.RGB };
            FluxJpeg.Core.Image img = new FluxJpeg.Core.Image(model, raster);

            //Encode the Image as a JPEG
            MemoryStream stream = new MemoryStream();
            FluxJpeg.Core.Encoder.JpegEncoder encoder = new FluxJpeg.Core.Encoder.JpegEncoder(img, 100, stream);
            encoder.Encode();

            // Back to the start
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        /// <summary>
        /// For Building Row
        /// </summary>
        /// <param name="strBuilder"></param>
        /// <param name="lstFields"></param>
        /// <param name="strFormat"></param>
        private static void BuildStringOfRow(StringBuilder strBuilder, List<string> lstFields, string strFormat)
        {
            switch (strFormat)
            {
                case "CSV":
                    strBuilder.AppendLine(String.Join(",", lstFields.ToArray()));
                    break;
            }
        }

        /// <summary>
        /// For the Format of the Field
        /// </summary>
        /// <param name="data"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private static string FormatField(string data, string format)
        {
            switch (format)
            {
                case "CSV":
                    return String.Format("\"{0}\"", data.Replace("\"", "\"\"\"").Replace("\n", "").Replace("\r", ""));
            }
            return data;
        }
    }
}