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

namespace CP_DB_app.Admin
{
    /// <summary>
    /// Логика взаимодействия для UpdateSong.xaml
    /// </summary>
    public partial class UpdateSong : Window
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();
        string song, newSongName, audioName, artist;
        Int16 songId;
        byte[] audio, oldBlob, newBlob;

        public UpdateSong()
        {
            InitializeComponent();
        }

        public UpdateSong(Int16 songId, string song)
        { 
            this.songId = songId;
            this.song = song;

            songName.Text = song;
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
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_SONG_BY_ID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_SONG_ID", OracleDbType.Int32, 10).Value = songId;
            cmd.Parameters["I_SONG_ID"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_SONG_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_SONG_CURS"].Direction = ParameterDirection.Output;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                oldBlob = reader.GetValue(2) as byte[];

                CheckFields();

                OracleCommand updateCmd = con.CreateCommand();
                updateCmd.CommandText = "CP_ADMIN.UPDATE_SONG";
                updateCmd.CommandType = CommandType.StoredProcedure;
                updateCmd.Parameters.Add("I_SONG_ID", OracleDbType.Int32, 10).Value = songId;
                updateCmd.Parameters["I_SONG_ID"].Direction = ParameterDirection.Input;

                updateCmd.Parameters.Add("I_SONG_NAME", OracleDbType.NVarchar2, 30).Value = newSongName;
                updateCmd.Parameters.Add("I_SONG_FILE", OracleDbType.Blob).Value = newBlob;
                updateCmd.Parameters.Add("I_ARTIST_ID", OracleDbType.Int32, 10).Value = GetArtistID(artist);

                try
                {
                    updateCmd.ExecuteReader();
                    MessageBox.Show("Song " + songName.Text.Trim() + " updated");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
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

        private void CheckFields()
        {
            if (String.IsNullOrWhiteSpace(songName.Text.Trim()))
            {
                newSongName = song;
            }
            else
            {
                newSongName = songName.Text;
            }

            if (audioName == null)
            {
                newBlob = oldBlob;
            }
            else
            {
                newBlob = oldBlob;
            }
        }
    }
}
