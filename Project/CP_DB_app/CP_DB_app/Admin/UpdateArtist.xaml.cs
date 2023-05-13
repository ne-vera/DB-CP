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

namespace CP_DB_app.Admin
{
    /// <summary>
    /// Логика взаимодействия для UpdateArtist.xaml
    /// </summary>
    public partial class UpdateArtist : Window
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();
        string artist, newArtistName, imageName;
        Int16 artistId;
        byte[] image, oldBlob, newBlob;

        public UpdateArtist()
        {
            InitializeComponent();
        }

        public UpdateArtist(Int16 artistId, string artist)
        {
            InitializeComponent();
            this.artistId = artistId;
            this.artist = artist;

            artistName.Text = artist;
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            System.Windows.Application.Current.Windows[0].Show();
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
            cmd.CommandText = "CP_ADMIN.GET_ARTIST_BY_ID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_ARTIST_ID", OracleDbType.Int32, 10).Value = artistId;
            cmd.Parameters["I_ARTIST_ID"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_ARTIST_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_ARTIST_CURS"].Direction = ParameterDirection.Output;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                oldBlob = reader.GetValue(2) as byte[];

                CheckFields();

                OracleCommand updateCmd = con.CreateCommand();
                updateCmd.CommandText = "CP_ADMIN.UPDATE_ARTIST";
                updateCmd.CommandType = CommandType.StoredProcedure;
                updateCmd.Parameters.Add("I_ARTIST_ID", OracleDbType.Int32, 10).Value = artistId;
                updateCmd.Parameters["I_ARTIST_ID"].Direction = ParameterDirection.Input;

                updateCmd.Parameters.Add("I_ARTIST_NAME", OracleDbType.NVarchar2, 30).Value = newArtistName;
                updateCmd.Parameters.Add("I_ARTIST_PHOTO", OracleDbType.Blob).Value = newBlob;

                try
                {
                    updateCmd.ExecuteNonQuery();
                    MessageBox.Show("Artist " + artistName.Text.Trim() + " updated");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
            con.Close();
        }

        private void CheckFields()
        {
            if (String.IsNullOrWhiteSpace(artistName.Text.Trim()))
            {
                newArtistName = artist;
            }
            else
            {
                newArtistName = artistName.Text;
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
