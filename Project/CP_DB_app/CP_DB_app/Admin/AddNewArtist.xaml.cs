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
    /// Логика взаимодействия для AddNewArtist.xaml
    /// </summary>
    public partial class AddNewArtist : Window
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();
        string imageName;
        byte[] image;

        public AddNewArtist()
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
            if (!String.IsNullOrWhiteSpace(artistName.Text.Trim()))
            {

                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "CP_ADMIN.CREATE_ARTIST";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_ARTIST_NAME", OracleDbType.NVarchar2, 30).Value = artistName.Text.Trim();

                if (imageName != null)
                {
                    FileStream fls;
                    fls = new FileStream(imageName, FileMode.Open, FileAccess.Read);
                    byte[] blob = new byte[fls.Length];
                    fls.Read(blob, 0, System.Convert.ToInt32(fls.Length));
                    fls.Close();
                    cmd.Parameters.Add("I_ARTIST_PHOTO", OracleDbType.Blob).Value = blob;
                }
                else cmd.Parameters.Add("I_ARTIST_PHOTO", OracleDbType.Blob).Value = null;
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Artist " + artistName.Text.Trim() + " added");
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
