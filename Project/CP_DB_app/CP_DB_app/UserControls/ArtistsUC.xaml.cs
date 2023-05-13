using CP_DB_app.Admin;
using CP_DB_app.Data;
using CP_DB_app.ExtraWindows;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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
    /// Логика взаимодействия для ArtistsUC.xaml
    /// </summary>
    public partial class ArtistsUC : UserControl
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();
        string artist;
        Int16 artistId;
        bool isFollowing = false;

        public ArtistsUC()
        {
            InitializeComponent();
            CheckRole();
        }

        public ArtistsUC(Int16 artistId, string artistName)
        {
            InitializeComponent();
            CheckRole();

            blockArtistId.Text = artistId.ToString();
            blockArtistName.Text = artistName;

            this.artist = artistName;
            this.artistId = artistId;

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_ARTIST_BY_ID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_ARTIST_ID", OracleDbType.Int32, 10).Value = artistId;
            cmd.Parameters["I_ARTIST_ID"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_ARTIST_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_ARTIST_CURS"].Direction = ParameterDirection.Output;
            OracleDataReader reader = cmd.ExecuteReader();
            GetFollowersCount(artistId);
            CheckIsFollowing(artistId);
            while (reader.Read())
            {
                try
                {
                    byte[] imageData = reader.GetValue(2) as byte[];
                    if (imageData != null) 
                    {
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = new MemoryStream(imageData);
                        image.EndInit();
                        artistPhoto.Source = image;
                    }
                }
                catch(Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
            con.Close();
        }

        private void CheckIsFollowing(Int16 artistId)
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_FOLLOWED_ARTISTS";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_USER_ID", OracleDbType.Int32, 10).Value = CurrentUser.CurrentUserId;
            cmd.Parameters["I_USER_ID"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_ARTISTS_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_ARTISTS_CURS"].Direction = ParameterDirection.Output;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    if (artistId == Convert.ToInt16(reader.GetValue(0)))
                    {
                       isFollowing = true;
                        if (isFollowing)
                        {
                            followIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.AccountMinus;
                        }
                        else
                        {
                            followIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.AccountPlus;
                        }
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }

        private void showSongs_Click(object sender, RoutedEventArgs e)
        {
            ShowSongsByArtist show = new ShowSongsByArtist(artistId, artist);
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

        private void editArtist_Click(object sender, RoutedEventArgs e)
        {
            UpdateArtist updateArtist = new UpdateArtist(artistId, artist);
            updateArtist.Show();
            Application.Current.Windows[0].Hide();
        }

        private void deleteArtist_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete this artist", "Delete song", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "CP_ADMIN.DELETE_ARTIST";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_ARTIST_ID", OracleDbType.Int32, 10).Value = artistId;
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

        private void followButton_Click(object sender, RoutedEventArgs e)
        {
            if (isFollowing) 
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "CP_ADMIN.UNFOLLOW_ARTIST";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_ARTIST_ID", OracleDbType.Int32, 10).Value = artistId;
                cmd.Parameters.Add("I_USER_ID", OracleDbType.Int32, 10).Value = CurrentUser.CurrentUserId;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
                con.Close();
                isFollowing = false;
            }
            else
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "CP_ADMIN.FOLLOW_ARTIST";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_ARTIST_ID", OracleDbType.Int32, 10).Value = artistId;
                cmd.Parameters.Add("I_USER_ID", OracleDbType.Int32, 10).Value = CurrentUser.CurrentUserId;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
                con.Close();
                isFollowing = true;
            }
        }

        private void GetFollowersCount(Int16 artistId)
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_ARTIST_FOLLOWERS_AMOUNT";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_ARTIST_ID", OracleDbType.Int32, 10).Value = artistId;
            cmd.Parameters["I_ARTIST_ID"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_FOLLOWERS_AMOUNT", OracleDbType.Int32).Direction = ParameterDirection.Output;
            try
            {
                cmd.ExecuteNonQuery();

                OracleDecimal oracleDecimalValue = (OracleDecimal)cmd.Parameters["O_FOLLOWERS_AMOUNT"].Value;
                int intValue = oracleDecimalValue.ToInt32();
                followersCount.Text = intValue.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
