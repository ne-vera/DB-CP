using CP_DB_app.Admin;
using CP_DB_app.Data;
using CP_DB_app.ExtraWindows;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
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
    /// Логика взаимодействия для SongUC.xaml
    /// </summary>
    public partial class SongUC : UserControl
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();
        Int16 songId;
        string song, artist;
        bool isFavourite;

        public SongUC()
        {
            InitializeComponent();
            CheckRole();
        }

        public SongUC(Int16 id, string songName)
        {
            InitializeComponent();
            CheckRole();

            con.Open();
            blockSongName.Text = songName;
            song = songName;
            songId = id;
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_SONG_BY_ID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_SONG_ID", OracleDbType.Int32, 10).Value = songId;
            cmd.Parameters["I_SONG_ID"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_SONG_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_SONG_CURS"].Direction = ParameterDirection.Output;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    GetFavsCount(id);
                    CheckIsFavourite(id);
                    Int16 artistId = Convert.ToInt16(reader.GetValue(3));
                    OracleCommand artCmd = con.CreateCommand();
                    artCmd.CommandText = "CP_ADMIN.GET_ARTIST_BY_ID";
                    artCmd.CommandType = CommandType.StoredProcedure;
                    artCmd.Parameters.Add("I_ARTIST_ID", OracleDbType.Int32, 10).Value = artistId;
                    artCmd.Parameters["I_ARTIST_ID"].Direction = ParameterDirection.Input;
                    artCmd.Parameters.Add("O_ARTIST_CURS", OracleDbType.RefCursor);
                    artCmd.Parameters["O_ARTIST_CURS"].Direction = ParameterDirection.Output;
                    OracleDataReader artReader = artCmd.ExecuteReader();
                    while (artReader.Read())
                    {
                        blockArtistName.Text = artReader.GetValue(1).ToString();
                        artist = artReader.GetValue(1).ToString();
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
            con.Close();
        }

        private void playSong_Click(object sender, RoutedEventArgs e)
        {
            Player play = new Player(songId, artist, song);
            play.Show();
            Application.Current.Windows[0].Hide();
        }

        private void editSong_Click(object sender, RoutedEventArgs e)
        {
            //!!!
            UpdateSong updateSong = new UpdateSong(songId, song);
            updateSong.Show();
            Application.Current.Windows[0].Hide();
        }

        private void deleteSong_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete this song?", "Delete song", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "CP_ADMIN.DELETE_SONG";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_SONG_ID", OracleDbType.Int32, 10).Value = songId;
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

        private void toAlbum_Click(object sender, RoutedEventArgs e)
        {
            //!!!
            AddSongToAlbum addSongToAlbum = new AddSongToAlbum(songId);
            addSongToAlbum.Show();
            Application.Current.Windows[0].Hide();
        }

        private void toPlaylist_Click(object sender, RoutedEventArgs e)
        {            
            //!!!
            AddSongToPlaylist addSongToPlaylist = new AddSongToPlaylist(songId);
            addSongToPlaylist.Show();
            Application.Current.Windows[0].Hide();
        }

        public void CheckRole()
        {
            if (CurrentUser.isAdmin)
            {
                adminButtons.Visibility = Visibility.Visible;
            }
        }

        private void favsButton_Click(object sender, RoutedEventArgs e)
        {
            if (isFavourite)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "CP_ADMIN.DELETE_SONG_FROM_FAVOURITE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_SONG_ID", OracleDbType.Int32, 10).Value = songId;
                cmd.Parameters.Add("I_USER_ID", OracleDbType.Int32, 10).Value = CurrentUser.CurrentUserId;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
                con.Close();
                isFavourite = false;
            }
            else 
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "CP_ADMIN.ADD_SONG_TO_FAVOURITE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_SONG_ID", OracleDbType.Int32, 10).Value = songId;
                cmd.Parameters.Add("I_USER_ID", OracleDbType.Int32, 10).Value = CurrentUser.CurrentUserId;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
                con.Close();
                isFavourite = false;
            }
        }

        public void CheckIsFavourite(Int16 songId)
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_USER_FAVOURITES";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_USER_ID", OracleDbType.Int32, 10).Value = CurrentUser.CurrentUserId;
            cmd.Parameters["I_USER_ID"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_SONGS_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_SONGS_CURS"].Direction = ParameterDirection.Output;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    if (songId == Convert.ToInt16(reader.GetValue(0)))
                    {
                        isFavourite = true;
                        if (isFavourite)
                        {
                            favsIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Heart;
                        }
                        else
                        {
                           favsIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.HeartOutline;
                        }
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }

        public void GetFavsCount(Int16 songId)
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_AMOUNT_USERS_BY_FAVOURITES";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_SONG_ID", OracleDbType.Int32, 10).Value = songId;
            cmd.Parameters["I_SONG_ID"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_USERS_AMOUNT", OracleDbType.Int32);
            cmd.Parameters["O_USERS_AMOUNT"].Direction = ParameterDirection.Output;
            try
            {
                cmd.ExecuteNonQuery();

                OracleDecimal oracleDecimalValue = (OracleDecimal)cmd.Parameters["O_USERS_AMOUNT"].Value;
                int intValue = oracleDecimalValue.ToInt32();
                favsCount.Text = intValue.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
