using CP_DB_app.Converters;
using CP_DB_app.Data;
using CP_DB_app.UserControls;
using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace CP_DB_app.ExtraWindows
{
    /// <summary>
    /// Логика взаимодействия для UpdatePlaylist.xaml
    /// </summary>
    public partial class UpdatePlaylist : Window
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();
        string playlist, newPlaylistName, imageName, oldDescr, newDescr;
        Int16 playlistId;
        byte[] image, oldBlob, newBlob;

        public UpdatePlaylist()
        {
            InitializeComponent();
        }

        public UpdatePlaylist(Int16 playlistId, string playlist)
        {
            InitializeComponent();
            this.playlistId = playlistId;
            this.playlist = playlist;

            playlistName.Text = playlist;
        }

        private void attachPhoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Filter = "Image Files|*.jpg;*.png;"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    imageName = openFileDialog.FileName;
                    image = File.ReadAllBytes(openFileDialog.FileName);
                    var bitmImg = new BitmapImage();
                    using (var mem = new MemoryStream(image))
                    {
                        mem.Position = 0;
                        bitmImg.BeginInit();
                        bitmImg.CacheOption = BitmapCacheOption.OnLoad;
                        bitmImg.StreamSource = mem;
                        bitmImg.EndInit();
                    }
                    bitmImg.Freeze();
                }
                openFileDialog = null;
            }
            catch (System.ArgumentException ae)
            {
                imageName = "";
                MessageBox.Show(ae.Message.ToString());
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
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
                oldBlob = reader.GetValue(4) as byte[];
                oldDescr = reader.GetValue(3) as string;
            }

            CheckFields();

            OracleCommand updateCmd = con.CreateCommand();
            updateCmd.CommandText = "CP_ADMIN.UPDATE_PLAYLIST";
            updateCmd.CommandType = CommandType.StoredProcedure;
            updateCmd.Parameters.Add("I_PLAYLIST_ID", OracleDbType.Int32, 10).Value = playlistId;
            updateCmd.Parameters["I_PLAYLIST_ID"].Direction = ParameterDirection.Input;
            updateCmd.Parameters.Add("I_PLAYLIST_NAME", OracleDbType.NVarchar2, 30).Value = newPlaylistName;
            updateCmd.Parameters.Add("I_USER_ID", OracleDbType.Int32, 10).Value = CurrentUser.CurrentUserId;
            updateCmd.Parameters["I_USER_ID"].Direction = ParameterDirection.Input;
            updateCmd.Parameters.Add("I_PLAYLIST_DESCRIPTION", OracleDbType.NVarchar2).Value = newDescr;
            updateCmd.Parameters.Add("I_PLAYLIST_COVER", OracleDbType.Blob).Value = newBlob;
            try
            {
                updateCmd.ExecuteNonQuery();
                MessageBox.Show("Playlist " + playlistName.Text.Trim() + " updated");
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            con.Close();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            System.Windows.Application.Current.Windows[0].Show();
        }

        private void CheckFields()
        {
            if (String.IsNullOrWhiteSpace(playlistName.Text.Trim()))
            {
                newPlaylistName = playlist;
            }
            else
            {
                newPlaylistName = playlistName.Text;
            }

            if (String.IsNullOrWhiteSpace(descrtiption.Text.Trim()))
            {
                newDescr = oldDescr;
            }
            else
            {
                newDescr = descrtiption.Text;
            }

            if (imageName == null)
            {
                newBlob = oldBlob;
            }
            else
            {
                newBlob = BlobConverter.ConvertToBlob(imageName);
            }
        }
    }
}
