// TODO:
// * Documentation
// * Variables
// * See Parser

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
using System.Collections.ObjectModel;

namespace Expressions
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ReadOnlyCollection<string> _symbols;
        private decimal _result;

        public MainWindow()
        {
            InitializeComponent();
            char[] vars = {'x', 'y', 'z', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'a', 'b', 'c', 'd'};
            initVarGrid(VarGrid, vars);
        }

        private void initVarGrid(Panel parent, char[] vars)
        {
            int n = vars.Length;
            Grid[] cell = new Grid[n];
            ColumnDefinition[] lblCol = new ColumnDefinition[n];
            ColumnDefinition[] txtCol = new ColumnDefinition[n];
            Label[] lbl = new Label[n];
            TextBox[] txt = new TextBox[n];

            for (int i = 0; i < n; i++)
            {
                cell[i] = new Grid();
                cell[i].Margin = new Thickness(5);

                lblCol[i] = new ColumnDefinition();
                lblCol[i].Width = new GridLength(1, GridUnitType.Star);
                cell[i].ColumnDefinitions.Add(lblCol[i]);
                lbl[i] = new Label();
                lbl[i].Content = vars[i].ToString() + " =";
                Grid.SetColumn(lbl[i], 0);

                txtCol[i] = new ColumnDefinition();
                txtCol[i].Width = new GridLength(3, GridUnitType.Star);
                cell[i].ColumnDefinitions.Add(txtCol[i]);
                txt[i] = new TextBox();
                txt[i].Name = vars[i].ToString() + "Var";
                txt[i].VerticalContentAlignment = VerticalAlignment.Center;
                Grid.SetColumn(txt[i], 1);

                cell[i].Children.Add(lbl[i]);
                cell[i].Children.Add(txt[i]);
                
                parent.Children.Add(cell[i]);
            }
        }

        // Event handler for TextChanged in Input
        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _symbols = Lexer.GetSymbols(Input.Text);
                if (_symbols.Count > 0)
                    _result = Parser.GetResult(_symbols);
                else
                    _result = 0;
                Output.Content = _result;
            }
            catch (ArgumentException)
            {
                Output.Content = "---";
            }
        }
    }
}
