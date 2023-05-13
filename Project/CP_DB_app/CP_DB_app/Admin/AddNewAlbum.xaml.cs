using CP_DB_app.Converters;
using CP_DB_app.Data;
using Microsoft.Win32;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
    /// Логика взаимодействия для AddNewAlbum.xaml
    /// </summary>
    public partial class AddNewAlbum : Window
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();
        string imageName;
        byte[] image;

        public AddNewAlbum()
        {
            InitializeComponent();
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
            if (!String.IsNullOrWhiteSpace(albumName.Text.Trim()))
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "CP_ADMIN.CREATE_ALBUM";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_ALBUM_NAME", OracleDbType.NVarchar2, 30).Value = albumName.Text.Trim();

                if (!String.IsNullOrWhiteSpace(releaseDate.Text.Trim()))
                {
                    DateTime relaseDateTime = Convert.ToDateTime(releaseDate.Text.Trim());
                    cmd.Parameters.Add("I_ALBUM_RELEASE_DATE", OracleDbType.Date).Value = relaseDateTime;
                }
                else
                {
                    cmd.Parameters.Add("I_ALBUM_RELEASE_DATE", OracleDbType.Date).Value = null;
                }

                if (imageName != null)
                {
                    byte[] blob = BlobConverter.ConvertToBlob(imageName);
                    cmd.Parameters.Add("I_ALBUM_COVER", OracleDbType.Blob).Value = blob;
                }
                else cmd.Parameters.Add("I_ALBUM_COVER", OracleDbType.Blob).Value = null;
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Album " + albumName.Text.Trim() + " added");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
                con.Close();
            }
            else
            {
                MessageBox.Show("Input all fields");
                return;
            }
        }
    }
}
