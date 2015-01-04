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

using System.ComponentModel;
using System.Collections.ObjectModel;

namespace BatchRegex
{
    public partial class MainWindow : Window
    {
        private readonly Brush[] Colors = new Brush[] { Brushes.Aqua, Brushes.Yellow, Brushes.DarkOrange, Brushes.LightCoral, Brushes.LightSalmon, Brushes.LightPink, Brushes.LightGray, Brushes.GreenYellow };
        private readonly int NbOfColors = 8;

        public File CurrentFile
        {
            get;
            set;
        }
        private SimpleRegex Regex
        {
            set;
            get;
        }





        private FolderList FilesContainer
        {
            get;
            set;
        }



        public MainWindow()
        {
            Regex = new SimpleRegex();
            FilesContainer = new FolderList();
            CurrentFile = null;

            InitializeComponent();

            LoadOptions();
            SaveOptions();


        }




        // OPTIONS

        public void LoadOptions()
        {
            Microsoft.Win32.RegistryKey key;
            string loadedFormat = "";
            string loadedPattern = "";

            try
            {
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\BatchRegex", false);
                loadedFormat = (string)key.GetValue("Format", "$1");
                loadedPattern = (string)key.GetValue("Pattern", "(.*)");
            }
            catch (Exception)
            {
                return;
            }


            Regex.Pattern = loadedPattern;
            Regex.Format = loadedFormat;


            // Updating the text fields
            TextPointer begining = TextBoxPattern.Document.ContentStart;
            TextPointer end = TextBoxPattern.Document.ContentEnd;
            TextRange textRange = new TextRange(begining, end);

            textRange.Text = Regex.Pattern;



            begining = TextBoxFormat.Document.ContentStart;
            end = TextBoxFormat.Document.ContentEnd;
            textRange = new TextRange(begining, end);

            textRange.Text = Regex.Format;


            UpdateExemple();
            UpdateList();
        }




        public void SaveOptions()
        {
            Microsoft.Win32.RegistryKey key;

            try
            {
                key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\BatchRegex");

                if (Regex.Pattern == "")
                {
                    key.SetValue("Pattern", "(.*)");
                }
                else
                {
                    key.SetValue("Pattern", Regex.Pattern);
                }

                if (Regex.Format == "")
                {
                    key.SetValue("Format", "$1");
                }
                else
                {
                    key.SetValue("Format", Regex.Format);
                }

            }
            catch (Exception)
            {
                return;
            }
        }



        // LISTVIEW

        public void UpdateList()
        {
            FilesContainer.ApplyRegex(Regex);


            ObservableCollection<File> newList = new ObservableCollection<File>();

            foreach (Dir directory in FilesContainer.DirectoryList)
            {
                foreach (File file in directory.FileList)
                {
                    newList.Add(file);
                }
            }

            lvFiles.ItemsSource = newList;
        }


        // Event handlers


        public void lvFiles_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            File selectedFile = ((sender as ListBox).SelectedItem as File);

            CurrentFile = selectedFile;
            UpdateExemple();
        }

        public void lvCheckbox_Click(object sender, RoutedEventArgs args)
        {
            CheckBox checkbox = sender as CheckBox;
            File checkedFile = checkbox.Content as File;

            //checkedFile.IsEnabled = checkbox.IsChecked;

        }





        // Columns 

        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

        private void lvFilesColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                lvFiles.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            lvFiles.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        public class SortAdorner : Adorner
        {
            private static Geometry ascGeometry =
                    Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");

            private static Geometry descGeometry =
                    Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

            public ListSortDirection Direction { get; private set; }

            public SortAdorner(UIElement element, ListSortDirection dir)
                : base(element)
            {
                this.Direction = dir;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);

                if (AdornedElement.RenderSize.Width < 20)
                    return;

                TranslateTransform transform = new TranslateTransform
                        (
                                AdornedElement.RenderSize.Width - 15,
                                (AdornedElement.RenderSize.Height - 5) / 2
                        );
                drawingContext.PushTransform(transform);

                Geometry geometry = ascGeometry;
                if (this.Direction == ListSortDirection.Descending)
                    geometry = descGeometry;
                drawingContext.DrawGeometry(Brushes.Black, null, geometry);

                drawingContext.Pop();
            }
        }








        // EVENT HANDLING


        // PATTERN AND FORMAT

        private void UpdatePattern(object sender, TextChangedEventArgs args)
        {
            TextPointer begining = TextBoxPattern.Document.ContentStart;
            //TextPointer start = begining;
            TextPointer end = TextBoxPattern.Document.ContentEnd;
            TextRange textRange = new TextRange(begining, end);


            Regex.Pattern = textRange.Text.Replace(Environment.NewLine, "");
            UpdateExemple();
            UpdateList();



            /*
            if (!String.IsNullOrWhiteSpace(Regex.Pattern)) // Ca fait compiler longtemps, spam de ()()())(() Parcourir le string et utiliser index
            {
                int i = 0;
                int nbOfOpeningBrackets = 0;
                int nbOfClosingBrackets = 0;
                while (start != null && end != null || i > 10000)
                {
                    start = begining.GetPositionAtOffset(i, LogicalDirection.Backward);
                    end = begining.GetPositionAtOffset(i + 1, LogicalDirection.Backward);

                    if (start != null && end != null)
                    {
                        textRange = new TextRange(start, end);
                        if (textRange.Text == "(")
                        {
                            ++nbOfOpeningBrackets;
                            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, Colors[nbOfOpeningBrackets % NbOfColors]);
                        }
                        else if (textRange.Text == ")")
                        {
                            if (nbOfClosingBrackets < nbOfOpeningBrackets)
                            {
                                ++nbOfClosingBrackets;
                                textRange.ApplyPropertyValue(TextElement.BackgroundProperty, Colors[nbOfClosingBrackets % NbOfColors]);
                            }
                            else
                            {
                                textRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent);
                            }
                        }
                        else
                        {
                            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent);
                        }
                    }

                    i++;
                }
            }
            */
        }


        private void UpdateFormat(object sender, TextChangedEventArgs args)
        {
            TextPointer begining = TextBoxFormat.Document.ContentStart;
            TextPointer end = TextBoxFormat.Document.ContentEnd;
            TextRange textRange = new TextRange(begining, end);

            Regex.Format = textRange.Text.Replace(Environment.NewLine, "");
            UpdateExemple();
            UpdateList();
        }

        // Update the Textblocks
        private void UpdateExemple()
        {
            if (CurrentFile != null)
            {
                TextExempleFileName.Text = CurrentFile.FileName;
                TextExempleAfterRegex.Text = Regex.Apply(CurrentFile.FileName);
            }
            else
            {
                TextExempleFileName.Text = "File Name";
                TextExempleAfterRegex.Text = Regex.Apply("File Name");
            }
            FilesContainer.ApplyRegex(Regex);
        }



        // BUTTONS
        System.Windows.Forms.FolderBrowserDialog BrowseDialog = new System.Windows.Forms.FolderBrowserDialog();
        public void btnBrowse_Click(object sender, RoutedEventArgs args)
        {
            if (System.Windows.Forms.DialogResult.OK == BrowseDialog.ShowDialog())
            {
                tbFolderName.Text = BrowseDialog.SelectedPath;
            }
        }

        public void btnAddFolder_Click(object sender, RoutedEventArgs args)
        {
            if (!String.IsNullOrWhiteSpace(tbFolderName.Text))
            {
                FilesContainer.AddDir(tbFolderName.Text);
                UpdateList();
            }
        }
        public void btnUpdateAll_Click(object sender, RoutedEventArgs args)
        {
            FilesContainer.UpdateAll();
            UpdateList();
        }
        public void btnClearAll_Click(object sender, RoutedEventArgs args)
        {
            FilesContainer.Clear();
            UpdateList();
        }
        public void btnApplyRegexAll_Click(object sender, RoutedEventArgs args)
        {
            FilesContainer.ApplyRegex(Regex);
            FilesContainer.RenameFiles();
            UpdateList();
            SaveOptions();
        }


        public void btn_UndoAllClick(object sender, RoutedEventArgs args)
        {
            FilesContainer.UndoAll();
            FilesContainer.UpdateAll();
            UpdateList();
        }
        public void btn_UndoLastClick(object sender, RoutedEventArgs args)
        {
            FilesContainer.UndoLast();
            FilesContainer.UpdateAll();
            UpdateList();
        }



    }
}

