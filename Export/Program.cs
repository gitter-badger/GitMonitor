using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GitMonitor.Models;
using Newtonsoft.Json;

namespace GitMonitor.Export
{
    class Program
    {
        private static readonly Options arguments = new Options();
        private static readonly ExcelHelper MyExcel = new ExcelHelper();

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments(args, arguments);
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(arguments.ServiceEndPoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string urltopass;
                if (string.IsNullOrEmpty(arguments.RepositoryName))
                {
                    urltopass = $"/api/commits/{arguments.Days}";
                }
                else
                {
                    urltopass = @"/api/commits/default?repoName=" + arguments.RepositoryName + "&branchName=" + arguments.BranchName + "&days=" + arguments.Days;
                }

                var response = client.GetAsync(urltopass).Result;
                string content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var mi = JsonConvert.DeserializeObject<MonitoredPath>(content);
                    int row = 2;
                    if (arguments.Excel)
                    {
                        MyExcel.AddWorksheet("CommitData");
                        MyExcel.WriteHeaderRow("sha,author,weekofyear");
                    }

                    double i = 0;
                    var culture = new CultureInfo("en-US");
                    var calendar = culture.Calendar;

                    foreach (var commit in mi.Commits)
                    {
                        int column = 1;
                        //var pstTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(commit.DateCommitted, "Pacific Standard Time");

                        MyExcel.Write(row, column++, commit.Sha);
                        MyExcel.Write(row, column++, commit.Author);
                        //MyExcel.Write(row, column++, pstTime.ToString("dd MMM yyyy"));
                        //MyExcel.Write(row, column++, pstTime);
                        //MyExcel.Write(row, column++, pstTime.Day);
                        //MyExcel.Write(row, column++, pstTime.ToString("MMM"));
                        //MyExcel.Write(row, column++, pstTime.Year);
                        MyExcel.Write(row, column++, calendar.GetWeekOfYear(commit.CommitterWhen, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek));

                        row++;
                        i++;
                    }

                    string fileName = string.Empty;
                    if (string.IsNullOrWhiteSpace(fileName))
                    {
                        fileName = DateTime.Now.ToString("dd MMM yy hh-mm") + " ChangesetData.xlsx";
                    }

                    if (arguments.Excel)
                    {
                        MyExcel.SaveWorkBook(Path.Combine(Environment.CurrentDirectory, fileName), false);
                        MyExcel.Close();
                    }
                }
            }
        }
    }
}
