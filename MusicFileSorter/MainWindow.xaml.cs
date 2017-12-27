using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Data;
using System.ComponentModel;

class Mp3
{
    public bool Select { get; set; }
    public string FileName { get; set; }
    public string Title { get; set; }
    public string Album { get; set; }
    public string Artist { get; set; }
    public uint Year { get; set; }
    public string Genres { get; set; }
    public uint Track { get; set; }
    public uint Disc { get; set; }
    public string Location { get; set; }
    public string Comment { get; set; }

    //Select
    //FileName
    //Title
    //Album
    //Artist
    //Year
    //Genres
    //Track
    //Disc
    //Location
    //Comment

    public Mp3(bool select, string filename, string title, string album, string artist, uint year, string genres, uint track, uint disc, string location, string comment)
    {
        this.Select = select;
        this.FileName = filename;
        this.Title = title;
        this.Album = album;
        this.Artist = artist;
        this.Year = year;
        this.Genres = genres;
        this.Track = track;
        this.Disc = disc;
        this.Location = location;
        this.Comment = comment;
    }
}

namespace MusicFileSorter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static string txtPath;
        static string[] MusicFiles;
        static int intNumberOfFilesScanned;
        static int directoryCount;
        static bool startSelectWatch = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        //Buttons
        private void Button_Browse(object sender, RoutedEventArgs e)
        {
            BrowseForFile();
        }
        private void Button_Scan(object sender, RoutedEventArgs e)
        {
            MusicDataFile.HeadersVisibility = DataGridHeadersVisibility.All;
            startSelectWatch = true;
            //Reset Progress bar
            pbFileScan.Value = 0;

            

            //Read main directory path
            txtPath = txtEditor.Text;

            //Scan directory for given File
            if (System.IO.Directory.Exists(txtPath))
            {
                MusicFiles = Directory.GetFiles(txtPath, "*.mp3", cbSearchSubfolders.IsChecked.Value ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                intNumberOfFilesScanned = MusicFiles.Length;

                if (cbSearchSubfolders.IsChecked == true)
                {
                    directoryCount = System.IO.Directory.GetDirectories(txtPath).Length;
                }
                else
                {
                    directoryCount = 0;
                }

                NumberOfSubfolders.Content = directoryCount;
                NumberOfFiles.Content = intNumberOfFilesScanned;
                
                pbFileScan.Maximum = intNumberOfFilesScanned;

                Thread tScanFiles = new Thread(ScanFolderForFiles);
                tScanFiles.Start();
            }
            else
            {
                System.Windows.MessageBox.Show("The path specified does not exists!", "Music File Sorter - WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


            
        }
        private void Button_Delete(object sender, RoutedEventArgs e)
        {


            System.Windows.MessageBox.Show(cbSearchTarget.Text);
        }
        private void Button_Save(object sender, RoutedEventArgs e)
        {
            var selected = MusicDataFile.Items;

            int x = 0;

            foreach (var item in selected)
            {
                var musicFile = item as Mp3;

                if (musicFile.Select)
                {
                    x = x + 1;
                }

            }
            if (x > 0)
            {
                SaveFileModifications();
            }
            else 
            {
                System.Windows.MessageBox.Show("You have not selected any rows." + "\n" + "Make sure to use the checkboxes for each row you want to modify!", "WARNING - No Row(s) Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }

        //Menu Items
        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            BrowseForFile();
        }
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void txtEditor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtEditor.Text = "";
        }
        private void txtEditor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            txtEditor.Text = "";
        }

        //Methodes
        private void BrowseForFile()
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.ShowNewFolderButton = false;
            System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            { txtEditor.Text = folderDialog.SelectedPath; }
        }
        private void ScanFolderForFiles()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var items = new List<Mp3>();

            foreach (string MusicFile in MusicFiles)
            {
                //Setting variables to default
                string MusicFileName = "";
                string MusicTitle = "";
                string MusicAlbum = "";
                string MusicArtist = "";
                uint MusicYear = 0;
                string MusicGenres = "";
                uint MusicTrack = 0;
                uint MusicDisc = 0;
                string MusicPath = "";
                string MusicComment = "";
                bool bMusicError = false;
                
                //TabLig requires that the files are NOT Read Only, otherwise it cannot save changes
                pbFileScan.Dispatcher.Invoke(new Action(() => pbFileScan.Value++), System.Windows.Threading.DispatcherPriority.Normal, null);
                FileInfo ScanedFile = new FileInfo(MusicFile);
                ScanedFile.IsReadOnly = false;
                bMusicError = false;
                TagLib.File f = null;

                try
                {
                    f = TagLib.File.Create(MusicFile);
                }
                catch (Exception)
                {
                    bMusicError = true;
                }

                if (!bMusicError)
                {
                    if (String.IsNullOrEmpty(f.Tag.Title)) { MusicTitle = ""; } else { MusicTitle = f.Tag.Title; }
                    if (String.IsNullOrEmpty(f.Tag.Album)) { MusicAlbum = ""; } else { MusicAlbum = f.Tag.Album; }

                    try
                    {
                        if (String.IsNullOrEmpty(f.Tag.Performers[0].ToString())) { MusicArtist = ""; } else { MusicArtist = f.Tag.Performers[0]; }
                    }
                    catch (Exception)
                    {
                        MusicArtist = "";
                    }

                    if (String.IsNullOrEmpty(f.Tag.Year.ToString())) { MusicYear = 0; } else { MusicYear = f.Tag.Year; }

                    try
                    {
                        if (String.IsNullOrEmpty(f.Tag.Genres[0].ToString())) { MusicGenres = ""; } else { MusicGenres = f.Tag.Genres[0]; }
                    }
                    catch (Exception)
                    {
                        MusicGenres = "";
                    }

                    if (String.IsNullOrEmpty(f.Tag.Track.ToString())) { MusicTrack = 0; } else { MusicTrack = f.Tag.Track; }
                    if (String.IsNullOrEmpty(f.Tag.Disc.ToString())) { MusicDisc = 0; } else { MusicDisc = f.Tag.Disc; }
                    if (String.IsNullOrEmpty(f.Tag.Comment)) { MusicComment = ""; } else { MusicComment = f.Tag.Comment; }
                    
                }

                MusicPath = MusicFile.ToString();
                MusicFileName = System.IO.Path.GetFileName(MusicFile);

                items.Add(new Mp3(false, MusicFileName, MusicTitle, MusicAlbum, MusicArtist, MusicYear, MusicGenres, MusicTrack, MusicDisc, MusicPath, MusicComment));

            }
            ;
            MusicDataFile.Dispatcher.Invoke(new Action(() => MusicDataFile.ItemsSource = items),System.Windows.Threading.DispatcherPriority.Normal,null);
            MusicDataFile.Dispatcher.Invoke(new Action(() => MusicDataFile.Columns[9].IsReadOnly = true), System.Windows.Threading.DispatcherPriority.Normal, null);   
            //NumberOfFiles.Dispatcher.Invoke(new Action(() => NumberOfFiles.Content = intNumberOfFilesScanned), System.Windows.Threading.DispatcherPriority.Normal, null);
            
            //Fix Datagrid
            
            MusicDataFile.Dispatcher.Invoke(new Action(() => FixSelected()), System.Windows.Threading.DispatcherPriority.Normal, null);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            TimeSpan t = TimeSpan.FromMilliseconds(elapsedMs);
            string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                        t.Hours,
                        t.Minutes,
                        t.Seconds,
                        t.Milliseconds);
            ScanDurationInSec.Dispatcher.Invoke(new Action(() => ScanDurationInSec.Content = answer), System.Windows.Threading.DispatcherPriority.Normal, null);
            
        }
        private void SaveFileModifications()
        {

            var selected = MusicDataFile.Items;

            foreach (var item in selected)
            {

                var musicFile = item as Mp3;

                if (musicFile.Select)
                {
                    
                    FileInfo ScanedFile = new FileInfo(musicFile.Location);
                    ScanedFile.IsReadOnly = false;
                    TagLib.File f = TagLib.File.Create(musicFile.Location);

                    //Select
                    //FileName
                    //Title
                    //Album
                    //Artist
                    //Year
                    //Genres
                    //Track
                    //Disc
                    //Location
                    //Comment

                    f.Tag.Title = musicFile.Title;
                    f.Tag.Album = musicFile.Album;
                    f.Tag.Performers = new String[1] { musicFile.Artist };
                    f.Tag.Year = musicFile.Year;
                    f.Tag.Genres = new String[1] { musicFile.Genres };
                    f.Tag.Track = musicFile.Track;
                    f.Tag.Disc = musicFile.Disc;
                    f.Tag.Comment = musicFile.Comment;

                    try
                    {
                        f.Save();
                        System.Windows.MessageBox.Show("Save Successful");
                    }
                    catch (Exception)
                    {
                        System.Windows.MessageBox.Show("There was an error");
                    }

                    f.Save();

                    System.IO.File.Move(musicFile.Location, System.IO.Path.GetDirectoryName(musicFile.Location) + "\\" + musicFile.FileName);
                    //System.Windows.MessageBox.Show(System.IO.Path.GetDirectoryName(musicFile.Location));
                } 

            }

        }

        //Other Events----
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void FitToContent()
        {
            // where dg is my data grid's name...

            int szamol = 0;
            foreach (DataGridColumn column in MusicDataFile.Columns)
            {
                szamol++;
                if (szamol == 10)
                {

                } else {
                    ////if you want to size ur column as per the cell content
                    //column.Width = new DataGridLength(1.0, DataGridLengthUnitType.SizeToCells);
                    ////if you want to size ur column as per the column header
                    //column.Width = new DataGridLength(1.0, DataGridLengthUnitType.SizeToHeader);
                    ////if you want to size ur column as per both header and cell content
                    //column.Width = new DataGridLength(1.0, DataGridLengthUnitType.Auto);
                    column.Dispatcher.Invoke(new Action(() => column.Width = new DataGridLength(1.0, DataGridLengthUnitType.Auto)), System.Windows.Threading.DispatcherPriority.Normal, null);
                }

            }

        }
        private void FixSelected()
        {
            //Select
            //FileName
            //Title
            //Album
            //Artist
            //Year
            //Genres
            //Track
            //Disc
            //Location
            //Comment

            if (cbFileName.IsChecked == true)
            {
                MusicDataFile.Columns[1].Visibility = Visibility.Visible;
            }
            else
            {
                MusicDataFile.Columns[1].Visibility = Visibility.Hidden;
            }

            if (cbTitle.IsChecked == true)
            {
                MusicDataFile.Columns[2].Visibility = Visibility.Visible;
            }
            else
            {
                MusicDataFile.Columns[2].Visibility = Visibility.Hidden;
            }

            if (cbAlbum.IsChecked == true)
            {
                MusicDataFile.Columns[3].Visibility = Visibility.Visible;
            }
            else
            {
                MusicDataFile.Columns[3].Visibility = Visibility.Hidden;
            }

            if (cbArtist.IsChecked == true)
            {
                MusicDataFile.Columns[4].Visibility = Visibility.Visible;
            }
            else
            {
                MusicDataFile.Columns[4].Visibility = Visibility.Hidden;
            }

            if (cbYear.IsChecked == true)
            {
                MusicDataFile.Columns[5].Visibility = Visibility.Visible;
            }
            else
            {
                MusicDataFile.Columns[5].Visibility = Visibility.Hidden;
            }

            if (cbGenres.IsChecked == true)
            {
                MusicDataFile.Columns[6].Visibility = Visibility.Visible;
            }
            else
            {
                MusicDataFile.Columns[6].Visibility = Visibility.Hidden;
            }

            if (cbTrack.IsChecked == true)
            {
                MusicDataFile.Columns[7].Visibility = Visibility.Visible;
            }
            else
            {
                MusicDataFile.Columns[7].Visibility = Visibility.Hidden;
            }

            if (cbDisc.IsChecked == true)
            {
                MusicDataFile.Columns[8].Visibility = Visibility.Visible;
            }
            else
            {
                MusicDataFile.Columns[8].Visibility = Visibility.Hidden;
            }

            if (cbLocation.IsChecked == true)
            {
                MusicDataFile.Columns[9].Visibility = Visibility.Visible;
            }
            else
            {
                MusicDataFile.Columns[9].Visibility = Visibility.Hidden;
            }

            if (cbComment.IsChecked == true)
            {
                MusicDataFile.Columns[10].Visibility = Visibility.Visible;
            }
            else
            {
                MusicDataFile.Columns[10].Visibility = Visibility.Hidden;
            }

            FitToContent();

        }
        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
            txtSearch.GotFocus -= TextBox_GotFocus;
        }

        private void cbFileName_Checked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[1].Visibility = Visibility.Visible;
            }
            
        }
        private void cbFileName_Unchecked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[1].Visibility = Visibility.Hidden;
            }
            
        }
        private void cbTitle_Checked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[2].Visibility = Visibility.Visible;
            }
            
        }
        private void cbTitle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[2].Visibility = Visibility.Hidden;
            }
        }
        private void cbAlbum_Checked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[3].Visibility = Visibility.Visible;
            }
            
        }
        private void cbAlbum_Unchecked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[3].Visibility = Visibility.Hidden;
            }
            
        }
        private void cbArtist_Checked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[4].Visibility = Visibility.Visible;
            }
        }
        private void cbArtist_Unchecked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[4].Visibility = Visibility.Hidden;
            }
        }
        private void cbYear_Checked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[5].Visibility = Visibility.Visible;
            }
        }
        private void cbYear_Unchecked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[5].Visibility = Visibility.Hidden;
            }
        }
        private void cbGenres_Checked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[6].Visibility = Visibility.Visible;
            }
        }
        private void cbGenres_Unchecked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[6].Visibility = Visibility.Hidden;
            }
        }
        private void cbTrack_Checked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[7].Visibility = Visibility.Visible;
            }
        }
        private void cbTrack_Unchecked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[7].Visibility = Visibility.Hidden;
            }
        }
        private void cbDisc_Checked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[8].Visibility = Visibility.Visible;
            }
        }
        private void cbDisc_Unchecked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[8].Visibility = Visibility.Hidden;
            }
        }
        private void cbLocation_Checked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[9].Visibility = Visibility.Visible;
            }
        }
        private void cbLocation_Unchecked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[9].Visibility = Visibility.Hidden;
            }
        }
        private void cbComment_Checked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[10].Visibility = Visibility.Visible;
            }
        }
        private void cbComment_Unchecked(object sender, RoutedEventArgs e)
        {
            if (startSelectWatch)
            {
                MusicDataFile.Columns[10].Visibility = Visibility.Hidden;
            }
        }
        private void btnSearchTable_Click(object sender, RoutedEventArgs e)
        {
            

        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox t = (System.Windows.Controls.TextBox)sender;
            string filter = t.Text;
            if (startSelectWatch)
            {
                ICollectionView cv = CollectionViewSource.GetDefaultView(MusicDataFile.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        Mp3 p = o as Mp3;
                        if (t.Name == "txtId")
                            return (p.Title == filter);
                        //return (p.Title.ToUpper().StartsWith(filter.ToUpper()));

                        switch (cbSearchTarget.Text)
                        {
                            case "Title":
                                return (p.Title.ToUpper().Contains(filter.ToUpper()));
                                
                            case "FileName":
                                return (p.FileName.ToUpper().Contains(filter.ToUpper()));

                            case "Album":
                                return (p.Album.ToUpper().Contains(filter.ToUpper()));

                            case "Artist":
                                return (p.Artist.ToUpper().Contains(filter.ToUpper()));

                            case "Year":
                                return (p.Year.ToString().Contains(filter.ToString()));

                            case "Genres":
                                return (p.Genres.ToUpper().Contains(filter.ToUpper()));

                            case "Location":
                                return (p.Location.ToUpper().Contains(filter.ToUpper()));

                            default:
                                return (p.FileName.ToUpper().Contains(filter.ToUpper()));

                        }
                        
                    };
                }
            }

        }

    }
}
