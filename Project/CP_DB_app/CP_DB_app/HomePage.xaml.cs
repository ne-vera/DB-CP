using CP_DB_app.Admin;
using CP_DB_app.Data;
using CP_DB_app.ExtraWindows;
using CP_DB_app.UserControls;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace CP_DB_app
{
    /// <summary>
    /// Логика взаимодействия для Home.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();

        public HomePage()
        {
            InitializeComponent();
        }

        public void CheckRole()
        {
            if (CurrentUser.isAdmin)
            {
                deleteUserButton.IsEnabled = false;
                addNewArtistButton.Visibility = Visibility.Visible;
                addNewAlbumButton.Visibility = Visibility.Visible;
                addNewSongButton.Visibility = Visibility.Visible;
                tableControlButton.Visibility = Visibility.Visible;
            }
        }

        public void loadArtists()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_ALL_ARTISTS";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("O_ARTIST_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_ARTIST_CURS"].Direction = ParameterDirection.Output;
            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                artistList.Children.Clear();
                while (reader.Read())
                {
                    ArtistsUC artist = new ArtistsUC(reader.GetInt16(0), reader.GetString(1));
                    artistList.Children.Add(artist);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            con.Close();
        }

        public void loadAlbums()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_ALL_ALBUMS";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("O_ALBUMS_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_ALBUMS_CURS"].Direction = ParameterDirection.Output;
            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                albumList.Children.Clear();
                while (reader.Read())
                {
                    AlbumUC album = new AlbumUC(reader.GetInt16(0), reader.GetString(1));
                    albumList.Children.Add(album);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            con.Close();
        }

        public void loadSongs()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_ALL_SONGS";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("O_SONG_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_SONG_CURS"].Direction = ParameterDirection.Output;
            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                songList.Children.Clear();
                while (reader.Read())
                {
                    SongUC song = new SongUC(reader.GetInt16(0), reader.GetString(1));
                    songList.Children.Add(song);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            con.Close();
        }

        public void loadPlaylists()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_PLAYLIST_BY_USER";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_USER_ID", OracleDbType.Int32, 10).Value = CurrentUser.CurrentUserId;
            cmd.Parameters["I_USER_ID"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_PLAYLIST_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_PLAYLIST_CURS"].Direction = ParameterDirection.Output;
            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                playlistList.Children.Clear();
                while (reader.Read())
                {
                    PlaylistUC song = new PlaylistUC(reader.GetInt16(0), reader.GetString(1));
                    playlistList.Children.Add(song);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            con.Close();
        }

        public void loadFavs()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_USER_FAVOURITES";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_USER_ID", OracleDbType.Int32, 10).Value = CurrentUser.CurrentUserId;
            cmd.Parameters["I_USER_ID"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_SONGS_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_SONGS_CURS"].Direction = ParameterDirection.Output;
            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                favouritesList.Children.Clear();
                while (reader.Read())
                {
                    SongUC song = new SongUC(reader.GetInt16(0), reader.GetString(1));
                    favouritesList.Children.Add(song);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            con.Close();
        }

        public void loadFollowing()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_FOLLOWED_ARTISTS";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_USER_ID", OracleDbType.Int32, 10).Value = CurrentUser.CurrentUserId;
            cmd.Parameters["I_USER_ID"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_ARTISTS_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_ARTISTS_CURS"].Direction = ParameterDirection.Output;
            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                followingList.Children.Clear();
                while (reader.Read())
                {
                    ArtistsUC artist = new ArtistsUC(reader.GetInt16(0), reader.GetString(1));
                    followingList.Children.Add(artist);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            con.Close();
        }

        public void loadHome()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_TOP_FOLLOWED_ARTISTS";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("O_TOP_FOLLOWED_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_TOP_FOLLOWED_CURS"].Direction = ParameterDirection.Output;
            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                popularArtistList.Children.Clear();
                while (reader.Read())
                {
                    OracleCommand artCmd = con.CreateCommand();
                    artCmd.CommandText = "CP_ADMIN.GET_ARTIST_BY_ID";
                    artCmd.CommandType = CommandType.StoredProcedure;
                    artCmd.Parameters.Add("I_ARTIST_ID", OracleDbType.Int32, 10).Value = reader.GetInt16(0);
                    artCmd.Parameters["I_ARTIST_ID"].Direction = ParameterDirection.Input;
                    artCmd.Parameters.Add("O_ARTIST_CURS", OracleDbType.RefCursor);
                    artCmd.Parameters["O_ARTIST_CURS"].Direction = ParameterDirection.Output;
                    OracleDataReader artReader = artCmd.ExecuteReader();

                    while (artReader.Read())
                    {
                        if (popularArtistList.Children.Count < 5)
                        {
                            ArtistsUC song = new ArtistsUC(reader.GetInt16(0), artReader.GetString(1));
                            popularArtistList.Children.Add(song);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            con.Close();
        }

        public void settingsDataFill()
        {
            settingUserLogin.Text = CurrentUser.CurrentUserLogin;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadArtists();
            loadAlbums();
            loadSongs();
            loadPlaylists();
            loadFavs();
            loadFollowing();
            loadHome();
            settingsDataFill();
            CheckRole();
        }

        private void searchButtonArtist_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_ARTIST_BY_NAME";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_ARTIST_NAME", OracleDbType.NVarchar2, 30).Value = searchBarArtist.Text.Trim();
            cmd.Parameters["I_ARTIST_NAME"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_ARTIST_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_ARTIST_CURS"].Direction = ParameterDirection.Output;
            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                artistList.Children.Clear();
                while (reader.Read())
                {
                    ArtistsUC artist = new ArtistsUC(reader.GetInt16(0), reader.GetString(1));
                    artistList.Children.Add(artist);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            con.Close();
        }

        private void searchButtonAlbum_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_ALBUM_BY_NAME";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_ALBUM_NAME", OracleDbType.NVarchar2, 30).Value = searchBarAlbum.Text.Trim();
            cmd.Parameters["I_ALBUM_NAME"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_ALBUMS_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_ALBUMS_CURS"].Direction = ParameterDirection.Output;
            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                albumList.Children.Clear();
                while (reader.Read())
                {
                    AlbumUC album = new AlbumUC(reader.GetInt16(0), reader.GetString(1));
                    albumList.Children.Add(album);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            con.Close();
        }

        private void searchButtonSong_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_SONG_BY_NAME";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_SONG_NAME", OracleDbType.NVarchar2, 30).Value = searchBarSong.Text.Trim();
            cmd.Parameters["I_SONG_NAME"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_SONG_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_SONG_CURS"].Direction = ParameterDirection.Output;
            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                songList.Children.Clear();
                while (reader.Read())
                {
                    SongUC song = new SongUC(reader.GetInt16(0), reader.GetString(1));
                    songList.Children.Add(song);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            con.Close();
        }

        private void addNewArtistButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewArtist addNewArtist = new AddNewArtist();
            addNewArtist.Show();
            Application.Current.Windows[0].Hide();
        }

        private void addNewAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewAlbum addNewAlbum = new AddNewAlbum();
            addNewAlbum.Show();
            Application.Current.Windows[0].Hide();
        }

        private void addNewSongButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewSong addNewSong = new AddNewSong();
            addNewSong.Show();
            Application.Current.Windows[0].Hide();
        }

        private void addNewPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewPlaylist addNewPlaylist = new AddNewPlaylist();
            addNewPlaylist.Show();
            Application.Current.Windows[0].Hide();
        }

        private void backSettingButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.isAdmin = false;
            CurrentUser.CurrentUserId = -1;
            CurrentUser.CurrentUserLogin = null;
            this.NavigationService.Navigate(new Uri("./Authorization/Login.xaml", UriKind.Relative));
        }

        private void settingChangeButton_Click(object sender, RoutedEventArgs e)
        {
            settingUserPassword.IsReadOnly = false;
            settingUserLogin.IsReadOnly = false;

            settingUpdateButton.IsEnabled = true;
            settingCancelButton.IsEnabled = true;
            settingChangeButton.IsEnabled = false;
        }

        private void settingUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to change your password?", "Update password", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                string newPassword;
                string newLogin;
                
                if (String.IsNullOrWhiteSpace(settingUserLogin.Text.Trim()))
                {
                    newLogin = CurrentUser.CurrentUserLogin;
                }
                else
                {
                    newLogin = settingUserLogin.Text.Trim();
                }

                if (String.IsNullOrWhiteSpace(settingUserPassword.Text.Trim()))
                {
                    MessageBox.Show("Input password");
                }
                newPassword = settingUserPassword.Text.Trim();

                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "CP_ADMIN.UPDATE_USER";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_USER_ID", OracleDbType.Int32, 10).Value = CurrentUser.CurrentUserId;
                cmd.Parameters.Add("I_USER_LOGIN", OracleDbType.NVarchar2, 30).Value = newLogin;
                cmd.Parameters.Add("I_USER_PASSWORD", OracleDbType.NVarchar2, 30).Value = newPassword;
                try
                {
                    cmd.ExecuteNonQuery();
                    CurrentUser.CurrentUserLogin = newLogin;
                    settingChangeButton.IsEnabled = true;
                    settingUpdateButton.IsEnabled = false;
                    settingCancelButton.IsEnabled = false;

                    settingUserPassword.IsReadOnly = true;
                    settingUserLogin.IsReadOnly = true;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
                con.Close();
            }
            else
            {
                settingsDataFill();
            }
        }

        private void settingCancelButton_Click(object sender, RoutedEventArgs e)
        {
            settingChangeButton.IsEnabled = true;
            settingUpdateButton.IsEnabled = false;
            settingCancelButton.IsEnabled = false;

            settingUserPassword.IsReadOnly = true;
            settingUserLogin.IsReadOnly = true;

            settingsDataFill();
        }

        private void deleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete this account", "Delete user", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "CP_ADMIN.DELETE_USER";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_USER_ID", OracleDbType.Int32, 10).Value = CurrentUser.CurrentUserId;
                try
                {
                    cmd.ExecuteNonQuery();
                    this.NavigationService.Navigate(new Uri("./Authorization/Login.xaml", UriKind.Relative));
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
                con.Close();
            }
        }

        private void clearButtonArtist_Click(object sender, RoutedEventArgs e)
        {
            loadArtists();
        }

        private void clearButtonSong_Click(object sender, RoutedEventArgs e)
        {
            loadSongs();
        }

        private void clearButtonAlbum_Click(object sender, RoutedEventArgs e)
        {
            loadAlbums();
        }

        private void clearButtonPlaylist_Click(object sender, RoutedEventArgs e)
        {
            loadPlaylists();
        }

        private void clearButtonFavourite_Click(object sender, RoutedEventArgs e)
        {
            loadFavs();
        }

        private void clearButtonFollowing_Click(object sender, RoutedEventArgs e)
        {
            loadFollowing();
        }

        private void tableControlButton_Click(object sender, RoutedEventArgs e)
        {
            AdminPanel adm = new AdminPanel();
            adm.Show();
            Application.Current.Windows[0].Hide();
        }
    }
}
