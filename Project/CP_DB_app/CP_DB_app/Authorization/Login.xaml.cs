using CP_DB_app.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
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
using System.Windows.Shapes;

namespace CP_DB_app.Authorization
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        OracleConnection con = DbConnectionUtils.GetDBConnection();

        public Login()
        {
            InitializeComponent();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text == "ADMIN" && PasswordBox.Password == "ADMIN")
            {
                DbConnectionUtils.user = LoginBox.Text;
                DbConnectionUtils.password = PasswordBox.Password;
                CurrentUser.isAdmin = true;
            }
            
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "CP_ADMIN.LOG_IN_USER";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("I_USER_LOGIN", OracleDbType.NVarchar2, 30).Value = LoginBox.Text.Trim();
            cmd.Parameters.Add("I_USER_PASSWORD", OracleDbType.NVarchar2, 30).Value = PasswordBox.Password.Trim();
            cmd.Parameters.Add("O_USER_ID", OracleDbType.Int32, 10);
            cmd.Parameters["O_USER_ID"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("O_USER_LOGIN", OracleDbType.NVarchar2, 30);
            cmd.Parameters["O_USER_LOGIN"].Direction = ParameterDirection.Output;
            try
            {
                cmd.ExecuteNonQuery();
                string user = Convert.ToString(cmd.Parameters["O_USER_LOGIN"].Value);
                int id = Convert.ToInt32((decimal)(OracleDecimal)(cmd.Parameters["O_USER_ID"].Value));

                CurrentUser.CurrentUserId = id;
                CurrentUser.CurrentUserLogin = user;
                MessageBox.Show("User: " + user + "; id: " + id.ToString());

                this.NavigationService.Navigate(new Uri("./HomePage.xaml", UriKind.Relative));
            }
            catch (Exception exc)
            {
                MessageBox.Show("Incorrect login or password");
            }
            con.Close();
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("./Authorization/Registration.xaml", UriKind.Relative));
        }
    }
}
