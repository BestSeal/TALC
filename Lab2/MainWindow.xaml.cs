using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Lab2.BlackMagic;
using Microsoft.Win32;

namespace Lab2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Automata.Automata Automata { get; set; }

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
                var config = await File.ReadAllTextAsync(openFileDialog.FileName);
                Automata = new Automata.Automata(config);
                AppendAutomataInfo(Automata.ParsingErrors);
                
                if(string.IsNullOrEmpty(Automata.ParsingErrors))
                {
                    await UpdateAutomataInfo();
                }
            }
        }

        private async void ExpressionEnteredHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && !string.IsNullOrEmpty(AutomataExpression.Text) && Automata != null)
            {
                AppendAutomataInfo($"Entered expression: {AutomataExpression.Text}");
                try
                {
                    AppendAutomataInfo(Automata.ParseExpression(AutomataExpression.Text)
                        ? $"Expression successfully parsed with final state {Automata.CurrentState.Name}"
                        : $"Expression parsing failed with state {Automata.CurrentState.Name}");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    AppendAutomataInfo(exception.Message);
                }
            }
        }

        private void AppendAutomataInfo(string info)
        {
            if (!string.IsNullOrEmpty(info))
            {
                AutomataInfo.Text += $"\n{info}";
            }
        }

        private async void btnToDfa_OnClick(object sender, RoutedEventArgs e)
        {
            if (Automata == null)
            {
                AutomataInfo.Text = "Provide automata config first";
                return;
            }
            
            Automata.ToDfa();
            await UpdateAutomataInfo();
        }

        private async Task UpdateAutomataInfo()
        {
            var result = await SnakeWizard.CastMagic(Automata.States);
            AutomataGraph.Source = result;
            
            AutomataInfo.Text = "";
            AppendAutomataInfo(Automata.GetAutomataInfo());
            AppendAutomataInfo(Automata.GetConfig());
        }
    }
}