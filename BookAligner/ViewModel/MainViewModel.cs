using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;
using System.IO;
using System.Windows.Input;
using System;
using BookAligner.Code;
using Miktemk;
using System.Collections.Generic;

namespace BookAligner.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private DocAlignment docAlign;

        public TextDocument CodeDocument1 { get; } = new TextDocument();
        public TextDocument CodeDocument2 { get; } = new TextDocument();
        //public string SelectedSubstr1 { get; set; }
        //public string SelectedSubstr2 { get; set; }
        public WordHighlight Highlight1 { get; set; } = new WordHighlight(0, 0);
        public WordHighlight Highlight2 { get; set; } = new WordHighlight(0, 0);
        public IEnumerable<WordHighlightWithBrush> EmphasizedWords1 { get; private set; }
        public IEnumerable<WordHighlightWithBrush> EmphasizedWords2 { get; private set; }

        public ICommand SelectionChanged1Command { get; }
        public ICommand SaveAlignmentDataCommand { get; }
        public ICommand ReloadAlignmentDataCommand { get; }
        public ICommand AddManualCommand { get; }
        public ICommand AddRegexCommand { get; }
        
        public string WelcomeTitle { get; set; } = "bitchin";

        public MainViewModel()
        {
            SelectionChanged1Command = new RelayCommand<int>(OnSelectionChanged1);
            SaveAlignmentDataCommand = new RelayCommand(SaveAlignmentData);
            ReloadAlignmentDataCommand = new RelayCommand(ReloadAlignmentData);
            AddManualCommand = new RelayCommand(AddManual);
            AddRegexCommand = new RelayCommand(AddRegex);
            
            docAlign = new DocAlignment();

            LoadFiles();
        }

        private void AddRegex()
        {
            docAlign.AlignData.Regexes.Add(new DocAlignmentRegex
            {
                Regex1 = CodeDocument1.Text.Substring(Highlight1.StartIndex, Highlight1.Length),
                Regex2 = CodeDocument2.Text.Substring(Highlight2.StartIndex, Highlight2.Length),
            });
            UpdateUI();
        }

        private void AddManual()
        {
            docAlign.AlignData.Manual.Add(new DocAlignmentManual
            {
                Position1 = Highlight1.StartIndex,
                Position2 = Highlight2.StartIndex,
                Length1 = Highlight1.Length,
                Length2 = Highlight2.Length,
            });
            UpdateUI();
        }

        private void ReloadAlignmentData()
        {
            // TODO: via config file
            docAlign.AlignData = XmlFactory.LoadFromFile<DocAlignmentData>(@"C:\_mik\datafiles\proust-chez-swann\proust-chez-swann-alignment.xml");
            UpdateUI();
        }

        private void SaveAlignmentData()
        {
            // TODO: via config file
            XmlFactory.WriteToFile(docAlign.AlignData, @"C:\_mik\datafiles\proust-chez-swann\proust-chez-swann-alignment.xml");
        }

        private void LoadFiles()
        {
            // TODO: via config file
            docAlign.Text1 = CodeDocument1.Text = File.ReadAllText(@"C:\_mik\datafiles\proust-chez-swann\proust-chez-swann-fr.txt");
            docAlign.Text2 = CodeDocument2.Text = File.ReadAllText(@"C:\_mik\datafiles\proust-chez-swann\proust-chez-swann-ru.txt");
            //docAlign.AlignData = new DocAlignmentData();
            docAlign.AlignData = XmlFactory.LoadFromFile<DocAlignmentData>(@"C:\_mik\datafiles\proust-chez-swann\proust-chez-swann-alignment.xml");
            UpdateUI();
        }

        private void OnSelectionChanged1(int selectionStart1)
        {
            Highlight2 = docAlign.GetSentence2(selectionStart1);
        }

        private void UpdateUI()
        {
            docAlign.RecomputeSentenceAlignment();
            EmphasizedWords1 = docAlign.GetEmphasizedWords1();
            EmphasizedWords2 = docAlign.GetEmphasizedWords2();
        }
    }
}