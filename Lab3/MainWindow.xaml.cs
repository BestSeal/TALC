using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Lab3.StackAutomata;
using Microsoft.Win32;

namespace Lab3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StackAutomata.StackAutomata Automata { get; set; }

        private Config AutomataConfig { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            AutomataInfo.Text = "";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    AutomataConfig = new Config(openFileDialog.FileName);
                    AppendAutomataInfo("Commands:");
                    foreach (var command in AutomataConfig.Commands)
                    {
                        AppendAutomataInfo(command.ToString());
                    }
                }
                catch (Exception exception)
                {
                    AppendAutomataInfo(exception.Message);
                }
            }
        }

        private async void ExpressionEnteredHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && !string.IsNullOrEmpty(AutomataExpression.Text) && AutomataConfig != null)
            {
                try
                {
                    Automata = new StackAutomata.StackAutomata(AutomataConfig, AutomataExpression.Text, InitStack.Text);
                    AppendAutomataInfo(Automata.ParseExpression() ? "Expression is valid" : "Expression is invalid");
                    AppendAutomataInfo( $"({AutomataExpression.Text}, {InitStack.Text}) |- " + Automata.GetExecutionOrder());
                }
                catch (Exception exception)
                {
                    AppendAutomataInfo(exception.Message);
                }
            }
        }

        private void AppendAutomataInfo(string info) => AutomataInfo.Text += $"{info}\n";
    }
}