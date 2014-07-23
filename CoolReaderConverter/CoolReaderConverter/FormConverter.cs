using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CoolReaderConverter
{
    public partial class FormConverter : Form
    {
        public FormConverter()
        {
            InitializeComponent();
        }

        private void openFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog;
            Stream myStream;
            if (!IsSelectedCorrectFile(out openFileDialog, out myStream)) return;

            using (myStream)
            {
                using (var sr = File.OpenText(openFileDialog.FileName))
                {
                    List<string> bookmarksList;
                    if (GetBookmarks(sr, out bookmarksList)) return;

                    DysplayBookmarksInCorrectFormat(bookmarksList);
                    SaveBookmarksInTextFile(bookmarksList);
                }
            }
        }

        private static void SaveBookmarksInTextFile(List<string> bookmarksList)
        {
            Stream myStream;
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            var pathSaving = String.Empty;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog.OpenFile()) != null)
                {
                    pathSaving = saveFileDialog.FileName;
                    Console.WriteLine(pathSaving);
                    myStream.Close();
                }
            }

            using (var file = new StreamWriter(pathSaving))
            {
                foreach (var bookmark in bookmarksList)
                {
                    file.WriteLine(bookmark);
                }
            }
        }

        private static void DysplayBookmarksInCorrectFormat(List<string> bookmarksList)
        {
            foreach (var bookmark in bookmarksList)
            {
                Console.WriteLine(bookmark);
            }
        }

        private static bool GetBookmarks(StreamReader sr, out List<string> bookmarksList)
        {
            var fullText = sr.ReadToEnd();
            Console.WriteLine(fullText);
            bookmarksList = new List<string>();
            const string symbolBeginingBookmark = "<<";
            const string symbolEndingBookmark = ">>";
            while (fullText.Contains(symbolBeginingBookmark))
            {
                try
                {
                    var currentBookmark = String.Empty;
                    var indexSymbolBegningBookmarks = fullText.IndexOf(symbolBeginingBookmark);
                    var indexSymbolEndingBookmarks = fullText.IndexOf(symbolEndingBookmark);
                    var lengthCurrentBookmark = indexSymbolEndingBookmarks - indexSymbolBegningBookmarks;
                    currentBookmark = fullText.Substring(indexSymbolBegningBookmarks + 2, lengthCurrentBookmark - 2);
                    fullText = fullText.Remove(0, indexSymbolEndingBookmarks + 5);

                    bookmarksList.Add("- " + currentBookmark);
                }
                catch (Exception)
                {
                    Console.WriteLine("Some Exception on loop while!");
                    return true;
                }
            }
            return false;
        }

        private bool IsSelectedCorrectFile(out OpenFileDialog openFileDialog, out Stream myStream)
        {
            openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Application.ExecutablePath,
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                myStream = null;
                return false;
            }

            myStream = null;
            try
            {
                if ((myStream = openFileDialog.OpenFile()) != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
            return false;
        }
    }
}
