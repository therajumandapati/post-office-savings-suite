using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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

namespace PostOfficeSavingsSuite
{
    /// <summary>
    /// Interaction logic for CreateForm.xaml
    /// </summary>
    public partial class CreateFormWindow : Window
    {
        OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Sai\Data\SaiP.mdb;Jet OLEDB:Database Password=ijar;");
        public CreateFormWindow()
        {
            InitializeComponent();
            OpenDbConnection();
        }

        private void OpenDbConnection() 
        {
            con.Open();

            OleDbDataAdapter da = new OleDbDataAdapter("Select * from Emp", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
        }

        private void CreateForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit ?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}
