using System.Windows;
using System.Windows.Controls;

namespace MNDataSearch.View
{
    /// <summary>
    /// Interaction logic for CatalougeDetail.xaml
    /// </summary>
    public partial class CatalougeDetail : Window
    {
        public CatalougeDetail()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                pd.PrintVisual(this, "Catlouge Details");
            }
            DialogResult = true;
        }
    }

}
