using CP_DB_app.Data;
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
using System.Windows.Shapes;

namespace CP_DB_app.ExtraWindows
{
    /// <summary>
    /// Логика взаимодействия для ShowSongsByAlbum.xaml
    /// </summary>
    public partial class ShowSongsByAlbum : Window
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();
        string album;
        Int16 albumId;

        public ShowSongsByAlbum()
        {
            InitializeComponent();
        }

        public ShowSongsByAlbum(Int16 albumId, string album)
        {
            InitializeComponent();

            this.album = album;
            this.albumId = albumId;
            albumName.Text = album;
        }

        public void loadSongs()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_SONGS_BY_ALBUM";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_ALBUM_ID", OracleDbType.Int32, 10).Value = albumId;
            cmd.Parameters["I_ALBUM_ID"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_SONGS_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_SONGS_CURS"].Direction = ParameterDirection.Output;
            OracleDataReader reader = cmd.ExecuteReader();
            songList.Children.Clear();
            while (reader.Read())
            {
                SongUC song = new SongUC(reader.GetInt16(0), reader.GetString(1));
                songList.Children.Add(song);
            }
            con.Close();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Windows[0].Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadSongs();
        }
    }
}
