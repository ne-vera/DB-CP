using CP_DB_app.Data;
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

namespace CP_DB_app.Authorization
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Page
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();

        public Registration()
        {
            InitializeComponent();
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(LoginBox.Text.Trim()) || String.IsNullOrWhiteSpace(PasswordBox.Password.Trim()) || String.IsNullOrWhiteSpace(RepeatPasswordBox.Password.Trim()))
            {
                MessageBox.Show("Fill all fields");
                return;
            }

            if (PasswordBox.Password.Trim() != RepeatPasswordBox.Password.Trim())
            {
                MessageBox.Show("Passwords don`t match");
                return;
            }

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.REGISTER_USER";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_USER_LOGIN", OracleDbType.NVarchar2, 30).Value = LoginBox.Text.Trim();
            cmd.Parameters.Add("I_USER_PASSWORD", OracleDbType.NVarchar2, 30).Value = PasswordBox.Password.Trim();
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("User created!");
                this.NavigationService.Navigate(new Uri("./Authorization/Login.xaml", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("This username is already taken!");
            }
            con.Close();

        }
        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("./Authorization/Login.xaml", UriKind.Relative));
        }
    }
}
