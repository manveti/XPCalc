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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XPCalc {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private TextBox elBox, partyLevelBox, partySizeBox;

        public MainWindow() {
            InitializeComponent();
            this.elBox = new TextBox();
            this.elBox.Text = "1";
            Grid.SetRow(this.elBox, 0);
            Grid.SetColumn(this.elBox, 1);
            simpleGrid.Children.Add(this.elBox);
            this.partyLevelBox = new TextBox();
            this.partyLevelBox.Text = "1";
            Grid.SetRow(this.partyLevelBox, 0);
            Grid.SetColumn(this.partyLevelBox, 3);
            simpleGrid.Children.Add(this.partyLevelBox);
            this.partySizeBox = new TextBox();
            this.partySizeBox.Text = "4";
            Grid.SetRow(this.partySizeBox, 0);
            Grid.SetColumn(this.partySizeBox, 5);
            simpleGrid.Children.Add(this.partySizeBox);
        }
    }
}
