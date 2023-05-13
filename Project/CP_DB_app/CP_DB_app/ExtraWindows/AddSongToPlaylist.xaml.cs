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
    /// Логика взаимодействия для AddSongToPlaylist.xaml
    /// </summary>
    public partial class AddSongToPlaylist : Window
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();
        Int16 playlistId, songId;

        public AddSongToPlaylist()
        {
            InitializeComponent();
        }

        public AddSongToPlaylist(Int16 songId)
        {
            InitializeComponent();
            this.songId = songId;
        }

        public void AlbumComboBox()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_PLAYLIST_BY_USER";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_USER_ID", OracleDbType.Int32, 10).Value = CurrentUser.CurrentUserId;
            cmd.Parameters["I_USER_ID"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_PLAYLIST_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_PLAYLIST_CURS"].Direction = ParameterDirection.Output;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                playlistNameCombo.Items.Add(reader.GetString(0));
            }
            con.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AlbumComboBox();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Windows[0].Show();
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            playlistId = Convert.ToInt16(playlistNameCombo.Text);

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.ADD_SONG_TO_PLAYLIST";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_PLAYLIST_ID", OracleDbType.Int32, 10).Value = playlistId;
            cmd.Parameters.Add("I_SONG_ID", OracleDbType.Int32, 10).Value = songId;
            try
            {
                cmd.ExecuteReader();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            con.Close();
        }
    }
}
