using CP_DB_app.Converters;
using CP_DB_app.Data;
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
using static System.Net.Mime.MediaTypeNames;

namespace CP_DB_app.Admin
{
    /// <summary>
    /// Логика взаимодействия для UpdateAlbum.xaml
    /// </summary>
    public partial class UpdateAlbum : Window
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();
        string album, newAlbumName, imageName;
        Int16 albumId;
        byte[] image, oldBlob, newBlob;
        DateTime oldReleaseDate, newReleaseDate;

        public UpdateAlbum()
        {
            InitializeComponent();
        }

        public UpdateAlbum(Int16 albumId, string album)
        {
            InitializeComponent();
            this.albumId = albumId;
            this.album = album;

            albumName.Text = album;
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
            cmd.CommandText = "CP_ADMIN.GET_ALBUM_BY_ID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_ALBUM_ID", OracleDbType.Int32, 10).Value = albumId;
            cmd.Parameters["I_ALBUM_ID"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_ALBUM_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_ALBUM_CURS"].Direction = ParameterDirection.Output;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                oldBlob = reader.GetValue(3) as byte[];

                if (!Convert.IsDBNull(reader.GetValue(2)))
                {
                    oldReleaseDate = Convert.ToDateTime(reader.GetValue(2));
                    releaseDate.Text = oldReleaseDate.ToString();
                }
            }

            CheckFields();

            OracleCommand updateCmd = con.CreateCommand();
            updateCmd.CommandText = "CP_ADMIN.UPDATE_ALBUM";
            updateCmd.CommandType = CommandType.StoredProcedure;
            updateCmd.Parameters.Add("I_ALBUM_ID", OracleDbType.Int32, 10).Value = albumId;
            updateCmd.Parameters["I_ALBUM_ID"].Direction = ParameterDirection.Input;

            updateCmd.Parameters.Add("I_ALBUM_NAME", OracleDbType.NVarchar2, 30).Value = newAlbumName;
            updateCmd.Parameters.Add("I_ALBUM_RELEASE_DATE", OracleDbType.Date).Value = newReleaseDate;
            updateCmd.Parameters.Add("I_ALBUM_COVER", OracleDbType.Blob).Value = newBlob;
            try
            {
                updateCmd.ExecuteNonQuery();
                MessageBox.Show("Album " + albumName.Text.Trim() + " updated");
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
            if (String.IsNullOrWhiteSpace(albumName.Text.Trim()))
            {
                newAlbumName = album;
            }
            else
            {
                newAlbumName = albumName.Text;
            }

            if (String.IsNullOrWhiteSpace(releaseDate.Text.Trim()))
            {
                newReleaseDate = oldReleaseDate;
            }
            else
            {
                newReleaseDate = Convert.ToDateTime(releaseDate.Text);
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
