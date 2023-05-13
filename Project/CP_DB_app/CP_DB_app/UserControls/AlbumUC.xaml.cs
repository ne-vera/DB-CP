using CP_DB_app.Admin;
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
    /// Логика взаимодействия для AlbumUC.xaml
    /// </summary>
    public partial class AlbumUC : UserControl
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();
        string album;
        Int16 albumId;

        public AlbumUC()
        {
            InitializeComponent();
            CheckRole();
        }

        public AlbumUC(Int16 albumId, string albumName)
        {
            InitializeComponent();
            CheckRole();

            blockAlbumName.Text = albumName;
            album = albumName;
            this.albumId = albumId;

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_ALBUM_BY_ID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_ALBUM_ID", OracleDbType.Int32, 10).Value = albumId;
            cmd.Parameters["I_ALBUM_ID"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_ALBUM_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_ALBUM_CURS"].Direction = ParameterDirection.Output;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    if (!Convert.IsDBNull(reader.GetValue(2)))
                    {
                        DateTime date = (DateTime)reader.GetValue(2);
                        blockReleaseDate.Text = date.ToString();
                    }

                    byte[] imageData = reader.GetValue(3) as byte[];
                    if (imageData != null)
                    {
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = new MemoryStream(imageData);
                        image.EndInit();
                        albumCover.Source = image;
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
            ShowSongsByAlbum show = new ShowSongsByAlbum(albumId, album);
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

        private void editAlbum_Click(object sender, RoutedEventArgs e)
        {
            UpdateAlbum updateWin = new UpdateAlbum(this.albumId, this.album);
            updateWin.Show();
            Application.Current.Windows[0].Hide();
        }

        private void deleteAlbum_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete this album?", "Delete song", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "CP_ADMIN.DELETE_album";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_ALBUM_ID", OracleDbType.Int32, 10).Value = albumId;
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
