using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alpaca.Markets;
using System.Data.SqlClient;
using System.Net;
using System.Threading;
using System.Drawing;

namespace LootLoaderDataLoaderFormBased
{
    internal static class RunMe
    {
        private static string API_KEY = "";
        private static string API_SECRET_KEY = "";
        private static string windowsUserName = System.Environment.UserName;//gives windows username

        private static TimeSpan start = new TimeSpan(8, 29, 55); //almost 8:30 military time
        private static TimeSpan end = new TimeSpan(15, 00, 05); //almost 3 o'clock military time
        private static TimeSpan now = DateTime.Now.TimeOfDay;
        private static DayOfWeek theDayToday = DateTime.Today.DayOfWeek;
        private static DateTime TodaysDate = DateTime.Today;

        private static List<string> getData;
        private static SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-8KIMCEV\SQLEXPRESS;Initial Catalog=LootLoader;Integrated Security=True");
        private static SqlCommand command = new SqlCommand();
        private static string sqlINSERTQueryString = "INSERT INTO ";

        //public static Form1 form = new Form1();

        public static async Task RunMeMethod(string stockListing, int numberOfShares)
        {
            string today = theDayToday.ToString();
            string todaysDate = TodaysDate.ToString("yyyy'-'MM'-'dd");

            //ready the getLastEquity web call (TradingClient)
            var client = Alpaca.Markets.Environments.Paper.GetAlpacaTradingClient(new SecretKey(API_KEY, API_SECRET_KEY));

            //ready the getLastTrade web call (DataClient)
            var clientData = Alpaca.Markets.Environments.Paper.GetAlpacaDataClient(new SecretKey(API_KEY, API_SECRET_KEY));

            if ((start > now) || (now >= end) || today == "Saturday" || today == "Sunday")
            {
              
                //while NOT within stock market hours
                MessageBox.Show("***************************************************************\r\r" +
                                "The Stock Market is closed. Please try again Monday through Friday 8:30 AM to 3:00 PM CST.\r\r" +
                                "***************************************************************", "The Stock Market is Closed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                
                System.Windows.Forms.Application.Exit();

            }
            else
            {

                connection.Open();
                //MessageBox.Show("Status: " + connection.State);

                int i = 1;

                //while within stock market hours
                while ((start <= now) && (now < end) && today != "Saturday" && today != "Sunday")
                {
                    

                    //refresh form1

                    //form.Refresh();

                    try
                    {


                        //WriteToSQLTable_lootloader_PriceData_2021



                        //get price from Alpaca and convert to decimal number
                        var getLastTradeObject = await clientData.GetLastTradeAsync(stockListing);
                        var newPriceObject = getLastTradeObject.Price;
                        decimal oldMarketPrice = decimal.Parse(newPriceObject.ToString());


                        //OpenSqlConnection.OpenSqlConnectionMethod();
                        
                        string INSERT_INTO_lootloader_PriceData_2021 = sqlINSERTQueryString + "lootloader_PriceData_2021 (ID, Date, Time, Price, StockListing) VALUES(@id,@date,@time,@price,@stockListing)";

                        using (command = new SqlCommand(INSERT_INTO_lootloader_PriceData_2021, connection))
                        {

                            // Create and set the parameters values 
                            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = i;
                            i++;
                            command.Parameters.Add("@date", System.Data.SqlDbType.Date).Value = TodaysDate;
                            //decimal theTimeNow = (decimal)now.TotalSeconds;
                            now = DateTime.Now.TimeOfDay;
                            command.Parameters.Add("@time", System.Data.SqlDbType.Time).Value = now;





                            command.Parameters.Add("@price", System.Data.SqlDbType.Decimal).Value = oldMarketPrice;
                            newPriceObject = decimal.Zero;
                            command.Parameters.Add("@stockListing", System.Data.SqlDbType.NChar).Value = stockListing;

                            if (connection.State == System.Data.ConnectionState.Closed)
                            {
                                connection.Open();
                                //MessageBox.Show("Status: " + connection.State);
                            }
                            

                            command.ExecuteNonQuery();

                            
                        }
                    }
                    catch (WebException ex)
                    {
                        MessageBox.Show("Could not access Alpaca Network through pluggable protocol.\r\r" + ex, "WebException", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        connection.Close();
                        //MessageBox.Show("Status: " + connection.State);

                    }
                    catch (HttpListenerException ex)
                    {
                        MessageBox.Show("Could not access Alpaca Network through pluggable protocol.\r\r" + ex, "HttpListenerException", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        connection.Close();
                        //MessageBox.Show("Status: " + connection.State);

                    }
                    catch (Alpaca.Markets.RestClientErrorException ex)
                    {
                        MessageBox.Show("Something went wrong  because: \r\r" + ex, "Rest Client Error Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        connection.Close();
                        //MessageBox.Show("Status: " + connection.State);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Something is broke because: \r\r" + ex, "General Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        connection.Close();
                        //MessageBox.Show("Status: " + connection.State);

                    }

                    //Only a maximum of 200 hits to ALPACA server is allowed
                    Thread.Sleep(1000);
                }
                connection.Close();
                //MessageBox.Show("Status: " + connection.State);

            }

            System.Windows.Forms.Application.Exit();

        }

    }
}
