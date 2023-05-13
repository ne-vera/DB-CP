using CP_DB_app.Converters;
using CP_DB_app.Data;
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

namespace CP_DB_app.Admin
{
    /// <summary>
    /// Логика взаимодействия для AddSongToAlbum.xaml
    /// </summary>
    public partial class AddSongToAlbum : Window
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();
        Int16 albumId, songId;

        public AddSongToAlbum()
        {
            InitializeComponent();
        }

        public AddSongToAlbum(Int16 songId)
        {
            InitializeComponent();
            this.songId = songId;
        }

        public void AlbumComboBox()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_ALL_ALBUMS";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("O_ALBUM_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_ALBUM_CURS"].Direction = ParameterDirection.Output;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                albumNameCombo.Items.Add(reader.GetString(0));
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
            albumId = Convert.ToInt16(albumNameCombo.Text);

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.ADD_SONG_TO_ALBUM";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_ALBUM_ID", OracleDbType.Int32, 10).Value = albumId;
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
