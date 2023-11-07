using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PIIIProject
{
    /// <summary>
    /// Interaction logic for User_Result.xaml
    /// </summary>
    public partial class User_Result : Window
    {
        public User_Result(List<Results> result)
        {
            InitializeComponent();
            dgVisitors.ItemsSource = result;
        }
    }
}
