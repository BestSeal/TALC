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

namespace Lab4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public class Init()
    {
        string sentence;
            string lineNumbersContent;
            bool unusedTextBox;
            string textBlockVisibility;

            string predictiveAnalysisTableName;
            DataView predictiveAnalysisDataTable;
            string startNonterminal;
            Dictionary<string, List<string>> FIRST;
            Dictionary<string, List<string>> FOLLOW;

            DefaultDialogService dialogService = new DefaultDialogService();

            string[] machineFromTXT = File.ReadAllLines("new grammar rules.txt");
            if (machineFromTXT.Length != 0)
            {
                Dictionary<string, List<string>> grammar;
                startNonterminal = dialogService.BuildGrammarDictionary(machineFromTXT, out grammar);


                FIRST = new Dictionary<string, List<string>>();
                foreach (var grammarRules in grammar)
                    FIRST = dialogService.ConstructFIRST(grammar, grammarRules.Value, grammarRules.Key, FIRST);

                FOLLOW = dialogService.ConstructFOLLOW(FIRST, startNonterminal, grammar);

                DataTable predictiveAnalysisTable = dialogService.GeneratePredictiveAnalysisTable(grammar, FIRST, FOLLOW);

                string Sentence = File.ReadAllText("example.txt");
                string[] textRows = Sentence.Replace("\r\n", "\n").Split('\n');
                int[] numberOfCharactersInEachRow = new int[textRows.Count()];
                for (int i = 0; i < textRows.Count(); i++)
                    numberOfCharactersInEachRow[i] = textRows[i].Count();
                dialogService.TextCorrectnessVerification(predictiveAnalysisTable, FIRST, FOLLOW,
                    startNonterminal, Sentence, numberOfCharactersInEachRow);
            }
    }
}