using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class Dialog : Window
    {
        public Dialog()
        {
            InitializeComponent();
        }

        public string ResponseText
        {
            get { return TxtKeyLength.Text; }
            set { TxtKeyLength.Text = value; }
        }


        private void BtnKeyLength_Click(object sender, RoutedEventArgs e)
        {
            var length = 0;
            int.TryParse(TxtKeyLength.Text, out length);
            if (length < 384 || length > 16384 || length % 8 != 0)
            {
                MessageBox.Show("Only enter numbers wich are devideable by 8 and are between 384 and 16384", "Foutieve ingave");
            }
            else
            {
                DialogResult = true;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
