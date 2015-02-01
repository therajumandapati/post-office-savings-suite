using PostOfficeSavingsSuite.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PostOfficeSavingsSuite
{
    public class DbWorker
    {
        private static OleDbConnection Connection = null;
        private List<Customer> Customers = new List<Customer>();

        static DbWorker()
        {
            var dataSource = ConfigurationManager.AppSettings["DataSource"];
            var password = ConfigurationManager.AppSettings["DataSourcePassword"];
            Connection = new OleDbConnection(String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Jet OLEDB:Database Password={1};", dataSource, password));
        }
        public static List<Customer> GetCustomers(string customerId)
        {
            Connection.Open();
            OleDbDataAdapter adapter = new OleDbDataAdapter("Select * from Emp", Connection);
            DataTable customers = new DataTable();
            adapter.Fill(customers);
            List<Customer> customerList = (from customer in customers.AsEnumerable()
                                          where customer.ItemArray[0].ToString().Contains(customerId)
                                          select new Customer 
                                          {
                                              AccountNumber = Convert.ToDouble(customer.ItemArray[0].ToString()),
                                              Name = customer.ItemArray[1].ToString(),
                                              Amount = Convert.ToDouble(customer.ItemArray[2].ToString()),
                                              StartMonth = DateTime.Parse(customer.ItemArray[3].ToString())
                                              //StartMonth = DateTime.ParseExact(customer.ItemArray[3].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture)
                                          }).ToList();
            Connection.Close();
            return customerList;
        }

        private static string GenerateNextSerialNumber(string slNo)
        {
            var regex = new Regex(@"SL/(?<year>\d+)/(?<serial>\d+)");
            var match = regex.Match(slNo);
            var year = match.Groups["year"].Value;
            var newSlNo = 
                Convert.ToInt32(year) == DateTime.Today.Year ? 
                String.Format("SL/{0}/{1}", year, (Convert.ToInt32(match.Groups["serial"].Value) + 1).ToString()) : String.Format("SL/{0}/0000", DateTime.Today.Year.ToString());
            return newSlNo;
        }

        internal static string SaveForm(List<Customer> SavedCustomersList)
        {
            var oldOrderSerial = GetCurrentSerialNumberForMasterOrder();
            var newOrderSerial = GenerateNextSerialNumber(oldOrderSerial);
            var orderTotal = Math.Round(SavedCustomersList.Select(x=>x.PayingTotal).Sum());
            var newOrder = new MasterOrder 
            {
                SLNo = newOrderSerial,
                Date = DateTime.Today,
                TotalAmount = orderTotal,
                TotalAmountInWords = Helper.ConvertCurrencyIntoWords(Convert.ToInt32(orderTotal))
            };
            PersistMasterOrder(newOrder);
            var masterDetails = new List<MasterDetail>();
            foreach (var customer in SavedCustomersList) 
            {
                masterDetails.Add(new MasterDetail 
                {
                    AccountNumber = customer.AccountNumber,
                    AccountStart = customer.StartMonth,
                    Amount = customer.Amount,
                    Extra = customer.ExtraAmount,
                    Name = customer.Name,
                    PayingTotal = customer.PayingTotal,
                    Total = customer.Total,
                    SerialNumber = newOrder.SLNo
                });
            }
            PersistMasterDetails(masterDetails);
            return newOrderSerial;
        }

        private static void PersistMasterOrder(MasterOrder newOrder)
        {
            try
            {
                string commandString = "INSERT INTO PORDMast (SLNo, SLDate, TAmount, ANWords)" + " VALUES (?, ?, ?, ?)";
                OleDbCommand commandStatement = new OleDbCommand(commandString, Connection);
                commandStatement.Parameters.Add("@SLNo", OleDbType.VarWChar, 30).Value = newOrder.SLNo;
                commandStatement.Parameters.Add("@SLDate", OleDbType.Date, 50).Value = newOrder.Date;
                commandStatement.Parameters.Add("@TAmount", OleDbType.Integer, 40).Value = newOrder.TotalAmount;
                commandStatement.Parameters.Add("@ANWords", OleDbType.VarWChar, 40).Value = newOrder.TotalAmountInWords;
                Connection.Open();
                commandStatement.ExecuteNonQuery();
                Connection.Close();
            }
            catch(Exception e)
            {
                Connection.Close();
                throw;
            }
        }

        private static void PersistMasterDetails(List<MasterDetail> masterDetails)
        {
            try
            {
                string commandString = "INSERT into PORDDetail (SLNo, ACNo, AcStart, Name, ADep, Extra, Amount, CardNo, Cost)" + " VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";
                foreach (var masterDetail in masterDetails) 
                {
                    OleDbCommand commandStatement = new OleDbCommand(commandString, Connection);
                    commandStatement.Parameters.Add("@SLNo", OleDbType.VarWChar, 30).Value = masterDetail.SerialNumber;
                    commandStatement.Parameters.Add("@ACNo", OleDbType.Integer, 50).Value = masterDetail.AccountNumber;
                    commandStatement.Parameters.Add("@AcStart", OleDbType.Date, 40).Value = masterDetail.AccountStart;
                    commandStatement.Parameters.Add("@Name", OleDbType.VarWChar, 40).Value = masterDetail.Name;
                    commandStatement.Parameters.Add("@ADep", OleDbType.Integer, 40).Value = masterDetail.Amount;
                    commandStatement.Parameters.Add("@Extra", OleDbType.Integer, 40).Value = masterDetail.Extra;
                    commandStatement.Parameters.Add("@Amount", OleDbType.Integer, 40).Value = masterDetail.PayingTotal;
                    //commandStatement.Parameters.Add("@New", OleDbType.VarWChar, 40).Value = "";
                    commandStatement.Parameters.Add("@CardNo", OleDbType.Integer, 40).Value = 0;
                    commandStatement.Parameters.Add("@Cost", OleDbType.Integer, 40).Value = masterDetail.Total;
                    Connection.Open();
                    commandStatement.ExecuteNonQuery();
                    Connection.Close();   
                }
            }
            catch (Exception e)
            {
                Connection.Close();
                throw;
            }
        }

        private static string GetCurrentSerialNumberForMasterOrder()
        {
            Connection.Open();
            OleDbDataAdapter adapter = new OleDbDataAdapter("Select * from PORDMast", Connection);
            DataTable masterOrders = new DataTable();
            adapter.Fill(masterOrders);
            MasterOrder currentOrder = (from masterOrder in masterOrders.AsEnumerable()
                                        select new MasterOrder
                                        {
                                            SLNo = masterOrder.ItemArray[0].ToString()
                                        }).Last();
            Connection.Close();
            return currentOrder.SLNo;
        }

        public static bool CheckIfAccountNumberIsTaken(string accountNumber) 
        {
            if (accountNumber.Length != 5)
            {
                return false;
            }
            else 
            {
                Connection.Open();
                OleDbDataAdapter adapter = new OleDbDataAdapter(String.Format("Select * from Emp WHERE Emp.AcNo = {0}", accountNumber), Connection);
                DataTable customers = new DataTable();
                adapter.Fill(customers);
                Connection.Close();
                return customers.AsEnumerable().ToList().Count != 0;
            }
        }

        public static bool AddCustomer(DbCustomer customer) 
        {
            try
            {
                string commandString = "INSERT into Emp (AcNo, Name, Amount, AcStart)" + " VALUES (?, ?, ?, ?)";
                OleDbCommand commandStatement = new OleDbCommand(commandString, Connection);
                commandStatement.Parameters.Add("@AcNo", OleDbType.Integer, 50).Value = customer.AccountNumber;
                commandStatement.Parameters.Add("@Name", OleDbType.VarWChar, 40).Value = customer.Name;
                commandStatement.Parameters.Add("@Amount", OleDbType.Integer, 40).Value = customer.Amount;
                commandStatement.Parameters.Add("@AcStart", OleDbType.Date, 40).Value = customer.AccountStarted;
                Connection.Open();
                commandStatement.ExecuteNonQuery();
                Connection.Close();
                return true;
            }
            catch (Exception e)
            {
                Connection.Close();
                throw;
            }
        }
    }
}
