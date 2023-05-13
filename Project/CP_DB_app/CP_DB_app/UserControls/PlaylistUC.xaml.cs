using CP_DB_app.Data;
using CP_DB_app.ExtraWindows;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

namespace CP_DB_app.UserControls
{
    /// <summary>
    /// Логика взаимодействия для PlaylistUC.xaml
    /// </summary>
    public partial class PlaylistUC : UserControl
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();
        string playlist;
        Int16 playlistId;

        public PlaylistUC()
        {
            InitializeComponent();
            CheckRole();
        }

        public PlaylistUC(Int16 playlistId, string playlistName)
        {
            InitializeComponent();
            CheckRole();

            blockPlaylistName.Text = playlistName;
            this.playlistId = playlistId;
            this.playlist = playlistName;

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_PLAYLIST_BY_ID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_PLAYLIST_ID", OracleDbType.Int32, 10).Value = playlistId;
            cmd.Parameters["I_PLAYLIST_ID"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_PLAYLIST_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_PLAYLIST_CURS"].Direction = ParameterDirection.Output;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    string desc = reader.GetValue(3).ToString();
                    if (desc != null)
                    {
                        blockDescription.Text = desc;
                    }

                    byte[] imageData = reader.GetValue(4) as byte[];
                    if (imageData != null)
                    {
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = new MemoryStream(imageData);
                        image.EndInit();
                        playlistCover.Source = image;
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
            con.Close();
        }

        private void showSongs_Click(object sender, RoutedEventArgs e)
        {
            ShowSongsByPlaylist show = new ShowSongsByPlaylist(playlistId, playlist);
            show.Show();
            Application.Current.Windows[0].Hide();
        }

        public void CheckRole()
        {
            if (CurrentUser.isAdmin)
            {
                adminButtons.Visibility = Visibility.Visible;
            }
        }

        private void editPlaylist_Click(object sender, RoutedEventArgs e)
        {
            UpdatePlaylist updatePlaylist = new UpdatePlaylist(playlistId, playlist);
            updatePlaylist.Show();
            Application.Current.Windows[0].Hide();
        }

        private void deletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete this PLAYLIST?", "Delete PLAYLIST", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "CP_ADMIN.DELETE_PLAYLIST";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_PLAYLIST_ID", OracleDbType.Int32, 10).Value = playlistId;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
                con.Close();
            }
        }
    }
}
