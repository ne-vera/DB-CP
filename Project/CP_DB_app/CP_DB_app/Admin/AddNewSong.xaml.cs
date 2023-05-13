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
    /// Логика взаимодействия для AddNewSong.xaml
    /// </summary>
    public partial class AddNewSong : Window
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();
        string artist, song;
        byte[] audio;
        string audioName;

        public AddNewSong()
        {
            InitializeComponent();
        }

        public void ArtistComboBox()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_ALL_ARTISTS";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("O_ARTIST_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_ARTIST_CURS"].Direction = ParameterDirection.Output;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                artistNameCombo.Items.Add(reader.GetString(1));
            }
            con.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ArtistComboBox();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Windows[0].Show();
        }

        private void attachAudio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Filter = "Audio Files|*.mp3;*"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    if (!openFileDialog.FileName.ToLower().EndsWith(".mp3"))
                    {
                        throw new Exception("File must be .mp3");
                    }
                    audioName = openFileDialog.FileName;
                    audio = File.ReadAllBytes(openFileDialog.FileName);
                }
                openFileDialog = null;
                addBtn.IsEnabled = true;
            }
            catch (System.ArgumentException ae)
            {
                audioName = "";
                MessageBox.Show(ae.Message.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            song = songName.Text.Trim();
            artist = artistNameCombo.Text.Trim();

            if (String.IsNullOrWhiteSpace(artist) || String.IsNullOrWhiteSpace(song) || String.IsNullOrWhiteSpace(audioName))
            {
                MessageBox.Show("All field must contain data");
                return;
            }

            byte[] blob = BlobConverter.ConvertToBlob(audioName);

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.CREATE_SONG";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_SONG_NAME", OracleDbType.NVarchar2, 30).Value = song;
            cmd.Parameters.Add("I_SONG_FILE", OracleDbType.Blob).Value = blob;
            cmd.Parameters.Add("I_ARTIST_ID", OracleDbType.Int32, 10).Value = GetArtistID(artist);
            try
            {
                cmd.ExecuteReader();
                MessageBox.Show("Song " + song + " created");
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            con.Close();
        }

        private Int16 GetArtistID(string artist)
        {
            Int16 artistId = -1;
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_ARTIST_BY_NAME";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_ARTIST_NAME", OracleDbType.NVarchar2, 30).Value = artist;
            cmd.Parameters["I_ARTIST_NAME"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_ARTIST_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_ARTIST_CURS"].Direction = ParameterDirection.Output;
            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    artistId = Convert.ToInt16(reader.GetValue(0));
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            return artistId;
        }
    }
}
