using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MNDataSearch.Helper
{
    public static class PrintHelper
    {
        public static FixedDocument CreateFixedDocument(DataGrid dg, string strHeader, double paramColumnHeaderHeight = 40, double paramRowHeight = 25)
        {
            int dpi = 96;
            int RowCount = 0;
            double Total_Width = 0;
            int VisbileColumnLastIndex = 0; // Visbile Column Last index

            FixedDocument fixedDocument = new FixedDocument();
            double temp_ActualHeight = double.NaN;
            double temp_ActualWidth = double.NaN;
            double temp_ColumnHeaderHeight = double.NaN;
            double temp_RowHeight = double.NaN;
            Thickness temp_Margin = new Thickness(0, 0, 0, 0);
            ScrollBarVisibility temp_ActualHorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            fixedDocument.DocumentPaginator.PageSize = new Size(dpi * 11, dpi * 8.5);
            try
            { 
                foreach (object data in dg.ItemsSource) RowCount++;
                foreach (DataGridColumn dgc in dg.Columns)
                {
                    if (dgc.Visibility == Visibility.Visible)
                    {
                        Total_Width += dgc.ActualWidth;
                        VisbileColumnLastIndex = dg.Columns.IndexOf(dgc);
                    }
                }
                int Rows_Per_Page = 40; // Total rows to show Per Page

                dg.RenderTransform = new ScaleTransform(GlobalClass.DataScaleFactor, GlobalClass.DataScaleFactor);//Scale 1 is normal 0.97 is smaller in size
                dg.UpdateLayout();

                for (int cnt = 1; cnt <= Math.Ceiling((double)RowCount / Rows_Per_Page) + 1; cnt++)
                {
                    PageContent page = new PageContent();

                    int Remaining = RowCount - ((cnt - 1) * Rows_Per_Page);
                    if (Remaining < 1) break;

                    FixedPage inner_page = new FixedPage();
                    //inner_page.Background = Brushes.Gray;
                    inner_page.Width = dpi * 11;
                    inner_page.Height = dpi * 8.5;

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
                        //dg.Margin = new Thickness(-100, temp_Margin.Top, -2000, temp_Margin.Bottom);
                        dg.Margin = new Thickness(GlobalClass.LeftPrintingMargin, temp_Margin.Top + 10, 2, temp_Margin.Bottom + 2);
                        dg.Width = Total_Width + 50;
                        inner_page.Width = (Total_Width > inner_page.Width) ? Total_Width + 25 : inner_page.Width;
                        dg.Height = inner_page.Height + 50;// + 170;
                        dg.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                        dg.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                        dg.ColumnHeaderHeight = paramColumnHeaderHeight;
                        dg.RowHeight = paramRowHeight;
                        dg.UpdateLayout();

                        //Total_Row to show depends upon:   dg.Height=  (Total_Row x dg.RowHeight ) + dg.ColumnHeaderHeight 
                        Rows_Per_Page = Convert.ToInt32((dg.Height - dg.ColumnHeaderHeight) / dg.RowHeight) - 1;
                    }
                    else if (cnt > 1)
                    {
                        //inner_page.Width = (Total_Width > inner_page.Width) ? Total_Width + 30 : inner_page.Width;

                        if (Remaining >= Rows_Per_Page)
                        {
                            dg.SelectedIndex = RowCount - 1;
                            dg.UpdateLayout();
                            dg.ScrollIntoView(dg.SelectedItem, dg.Columns[VisbileColumnLastIndex]);
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

                        dg.ScrollIntoView(dg.SelectedItem, dg.Columns[VisbileColumnLastIndex]);
                    }

                    dg.UpdateLayout();

                    RenderTargetBitmap rtb = new RenderTargetBitmap((int)dg.ActualWidth, (int)dg.ActualHeight, dpi, dpi, PixelFormats.Default);
                    rtb.Render(dg);
                    Image image = new Image
                    {
                        Source = rtb,
                        Height = dg.ActualHeight,
                        Width = dg.ActualWidth,
                    };

                    inner_page.Children.Add((UIElement)image);

                    //measure size of the layout
                    Size sz = new Size(dpi * 11, dpi * 8.5);
                    inner_page.Measure(sz);
                    inner_page.Arrange(new Rect(new Point(), sz));
                    inner_page.UpdateLayout();
                    ((IAddChild)page).AddChild(inner_page);
                    fixedDocument.Pages.Add(page);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Issue occurred while Exporting PDF: " + ex.Message);
            }
            finally
            {
                dg.Height = temp_ActualHeight;
                dg.Width = temp_ActualWidth;
                dg.HorizontalScrollBarVisibility = temp_ActualHorizontalScrollBarVisibility;
                dg.VerticalScrollBarVisibility = temp_ActualHorizontalScrollBarVisibility;
                dg.Margin = temp_Margin;
                dg.ColumnHeaderHeight = temp_ColumnHeaderHeight;
                dg.RowHeight = temp_RowHeight;
                //dg.IsEnabled = true;
                dg.SelectedIndex = 0;
                dg.RenderTransform = new ScaleTransform(1, 1);
                dg.UpdateLayout();
                dg.ScrollIntoView(dg.SelectedItem, dg.Columns[VisbileColumnLastIndex]);
                dg.UpdateLayout();
            }

            return fixedDocument;
        }

        public static FixedDocument CreateSinglePageDocument(ContentControl uielement)
        {
            int dpi = 96;
            FixedDocument fixedDocument = new FixedDocument();
            fixedDocument.DocumentPaginator.PageSize = new Size(dpi * 11, dpi * 8.5);
            try
            {
                PageContent page = new PageContent();
                FixedPage inner_page = new FixedPage();
                inner_page.Width = dpi * 11;
                inner_page.Height = dpi * 8.5;

                RenderTargetBitmap rtb = new RenderTargetBitmap((int)uielement.ActualWidth, (int)uielement.ActualHeight, dpi, dpi, PixelFormats.Default);
                rtb.Render(uielement);
                Image image = new Image
                {
                    Source = rtb,
                    Height = uielement.ActualHeight,
                    Width = uielement.ActualWidth,
                    Margin = new Thickness(25) // Margin for the control
                };

                inner_page.Children.Add((UIElement)image);

                //measure size of the layout
                Size sz = new Size(dpi * 11, dpi * 8.5);
                inner_page.Measure(sz);
                inner_page.Arrange(new Rect(new Point(), sz));
                inner_page.UpdateLayout();
                ((IAddChild)page).AddChild(inner_page);
                fixedDocument.Pages.Add(page);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Issue occurred while Exporting PDF: " + ex.Message);
            }
            finally
            {
            }

            return fixedDocument;
        }
    }
}
