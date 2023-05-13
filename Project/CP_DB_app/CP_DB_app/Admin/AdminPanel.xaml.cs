using CP_DB_app.Data;
using CP_DB_app.UserControls;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace CP_DB_app.Admin
{
    /// <summary>
    /// Логика взаимодействия для AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Window
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();
        public AdminPanel()
        {
            InitializeComponent();
        }

        private void loadUsers()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_ALL_USERS";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("O_USER_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_USER_CURS"].Direction = ParameterDirection.Output;
            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                userList.Children.Clear();
                while (reader.Read()) 
                {
                    UserUC us = new UserUC(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
                    userList.Children.Add(us);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            con.Close();
        }

        private void xmlExportButton_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_DB.USERS_EXPORT";
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.ExecuteNonQuery();
                xmlExportButton.IsEnabled = false;
                xmlImportButton.IsEnabled = true;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Export error");
            }
            con.Close();
        }

        private void xmlImportButton_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_DB.ARTIST_IMPORT";
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.ExecuteNonQuery();
                xmlExportButton.IsEnabled = false;
                xmlImportButton.IsEnabled = true;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Import error");
            }
            con.Close();
        }

        private void clearButtonUsers_Click(object sender, RoutedEventArgs e)
        {
            loadUsers();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadUsers();
        }

        private void searchButtonUser_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.GET_USER_BY_LOGIN";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_USER_LOGIN", OracleDbType.NVarchar2, 30).Value = searchBarUsers.Text;
            cmd.Parameters["I_USER_LOGIN"].Direction = ParameterDirection.Input;
            cmd.Parameters.Add("O_USER_CURS", OracleDbType.RefCursor);
            cmd.Parameters["O_USER_CURS"].Direction = ParameterDirection.Output;
            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                userList.Children.Clear();
                while (reader.Read())
                {
                    UserUC us = new UserUC(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
                    userList.Children.Add(us);
                }
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
    }
}
