using CP_DB_app.Data;
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

namespace CP_DB_app
{
    /// <summary>
    /// Логика взаимодействия для Player.xaml
    /// </summary>
    public partial class Player : Window
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();
        Int32 songId;
        byte[] audioByteArr;
        MediaPlayer mediaPlayerObj = new MediaPlayer();

        public Player()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
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
                try
                {
                    Int16 artistId = Convert.ToInt16(reader.GetValue(3));
                    OracleCommand artCmd = con.CreateCommand();
                    artCmd.CommandText = "CP_ADMIN.GET_ARTIST_BY_ID";
                    artCmd.CommandType = CommandType.StoredProcedure;
                    artCmd.Parameters.Add("I_ARTIST_ID", OracleDbType.Int32, 10).Value = artistId;
                    artCmd.Parameters["I_ARTIST_ID"].Direction = ParameterDirection.Input;
                    artCmd.Parameters.Add("O_ARTIST_CURS", OracleDbType.RefCursor);
                    artCmd.Parameters["O_ARTIST_CURS"].Direction = ParameterDirection.Output;
                    OracleDataReader artReader = artCmd.ExecuteReader();
                    while (artReader.Read())
                    {
                        artistName.Text = artReader.GetValue(1).ToString();
                    }

                    songName.Text = reader.GetValue(1).ToString();

                    audioByteArr = reader.GetValue(2) as byte[];

                    using (FileStream bytesToAudio = File.Create("current.mp3"))
                    {
                        bytesToAudio.Write(audioByteArr, 0, audioByteArr.Length);
                        Stream audioFile = bytesToAudio;
                        bytesToAudio.Close();
                    }

                    mediaPlayerObj.Open(new Uri(@"D:\учеба\БД\Курсовой\Project\CP_DB_app\CP_DB_app\bin\Debug\current.mp3"));
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
            con.Close();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayerObj.Play();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayerObj.Pause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayerObj.Stop();
        }

        public Player(Int32 id, string artist, string song)
        {
            InitializeComponent();

            con.Open();
            this.songId = id;

            artistName.Text = artist;
            songName.Text = song;
           con.Close();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayerObj.Close();
            this.Close();
            Application.Current.Windows[0].Show();
        }
    }
}
