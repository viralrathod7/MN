using System;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MNDataSearch.View
{
    /// <summary>
    /// Interaction logic for CatalougeDetail.xaml
    /// </summary>
    public partial class CatalougeDetail : Window
    {
        bool EnableMovement = false;
        public CatalougeDetail()
        {
            InitializeComponent();
            if (EnableMovement)
            {
                gridFix.Visibility = Visibility.Collapsed;
                canvas.Visibility = Visibility.Visible;
                foreach (FrameworkElement item in canvas.Children)
                {
                    if (item is Button)
                        continue;
                    item.Cursor = Cursors.SizeAll;
                    item.PreviewMouseDown += TextBlock_MouseDown;
                    item.PreviewMouseMove += TextBlock_MouseMove;
                    item.PreviewMouseUp += TextBlock_MouseUp;
                }
            }
            this.KeyUp += CatalougeDetail_KeyUp;
        }

        private void CatalougeDetail_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Escape))
                DialogResult = false;
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key.Equals(Key.P))
                btnPrint_Click(null, null);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            PageMediaSize pageSize = new PageMediaSize(PageMediaSizeName.ISOA4);
            pd.PrintTicket.PageMediaSize = pageSize;
            pd.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;
            if (pd.ShowDialog() == true)
            {
                pd.PrintVisual(this, "Catlouge Details");
            }
            DialogResult = true;
        }

        UIElement tbMove;
        Point holdingPoint;
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tbMove == null)
            {
                holdingPoint = e.GetPosition(canvas);
                tbMove = sender as UIElement;
                holdingPoint.X = holdingPoint.X - (double)tbMove.GetValue(Canvas.LeftProperty);
                holdingPoint.Y = holdingPoint.Y - (double)tbMove.GetValue(Canvas.TopProperty);
            }
        }

        private void TextBlock_MouseMove(object sender, MouseEventArgs e)
        {
            if (tbMove != null && holdingPoint != null)
            {
                tbMove.SetValue(Canvas.TopProperty, e.GetPosition(canvas).Y - holdingPoint.Y);
                tbMove.SetValue(Canvas.LeftProperty, e.GetPosition(canvas).X - holdingPoint.X);
            }
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender as UIElement == tbMove)
            {
                tbMove = null;
            }
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }

}
