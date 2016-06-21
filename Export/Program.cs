// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="FreeToDev">Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Office.Interop.Excel;

namespace GitMonitor.Export
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using GitMonitor.Models;
    using Newtonsoft.Json;

    public class Program
    {
        private static readonly Options Arguments = new Options();
        private static readonly ExcelHelper MyExcel = new ExcelHelper();

        public static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments(args, Arguments);
            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Arguments.ServiceEndPoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string urltopass;
                if (string.IsNullOrEmpty(Arguments.RepositoryName))
                {
                    urltopass = $"/api/commits/{Arguments.Days}";
                }
                else
                {
                    urltopass = @"/api/commits/default?repoName=" + Arguments.RepositoryName + "&branchName=" + Arguments.BranchName + "&days=" + Arguments.Days;
                }

                var response = client.GetAsync(urltopass).Result;
                string content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var mi = JsonConvert.DeserializeObject<MonitoredPath>(content);
                    if (Arguments.Excel)
                    {
                        int row = 2;
                        MyExcel.AddWorksheet("CommitData");
                        MyExcel.WriteHeaderRow("Sha,CommitUrl,Author,AuthorEmail,AuthorWhen,Committer,CommitterEmail,CommitterWhen,IsMerge,Message,RepositoryFriendlyName,RepositoryName,DayOfWeek,WeekOfYear,Month,Year");
                        
                        double i = 0;
                        var culture = new CultureInfo("en-US");
                        var calendar = culture.Calendar;

                        //foreach (var commit in mi.Commits)
                        //{
                        //    int column = 1;
                        //    MyExcel.Write(row, column++, commit.Sha);
                        //    MyExcel.Write(row, column++, commit.CommitUrl);
                        //    MyExcel.Write(row, column++, commit.Author);
                        //    MyExcel.Write(row, column++, commit.AuthorEmail);
                        //    MyExcel.Write(row, column++, commit.AuthorWhen);
                        //    MyExcel.Write(row, column++, commit.Committer);
                        //    MyExcel.Write(row, column++, commit.CommitterEmail);
                        //    MyExcel.Write(row, column++, commit.CommitterWhen);
                        //    MyExcel.Write(row, column++, commit.IsMerge);
                        //    MyExcel.Write(row, column++, commit.Message);
                        //    MyExcel.Write(row, column++, commit.RepositoryFriendlyName);
                        //    MyExcel.Write(row, column++, commit.RepositoryName);
                        //    MyExcel.Write(row, column++, commit.CommitterWhen.ToString("ddd"));
                        //    MyExcel.Write(row, column++, calendar.GetWeekOfYear(commit.CommitterWhen, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek));
                        //    MyExcel.Write(row, column++, commit.CommitterWhen.ToString("MMM"));
                        //    MyExcel.Write(row, column++, commit.CommitterWhen.ToString("YYYY"));

                        //    row++;
                        //    i++;
                        //}

                        row = 0;
                        var data = new object[mi.CommitCount, 17];
                        foreach (var commit in mi.Commits)
                        {
                            int column = 0;
                            data[row, column++] = commit.Sha;
                            data[row, column++] = commit.CommitUrl;
                            data[row, column++] = commit.Author;
                            data[row, column++] = commit.AuthorEmail;
                            data[row, column++] = commit.AuthorWhen.ToString("F");
                            data[row, column++] = commit.Committer;
                            data[row, column++] = commit.CommitterEmail;
                            data[row, column++] = commit.CommitterWhen.ToString("F");
                            data[row, column++] = commit.IsMerge;
                            data[row, column++] = commit.Message;
                            data[row, column++] = commit.RepositoryFriendlyName;
                            data[row, column++] = commit.RepositoryName;
                            data[row, column++] = commit.CommitterWhen.ToString("ddd");
                            data[row, column++] = calendar.GetWeekOfYear(commit.CommitterWhen, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
                            data[row, column++] = commit.CommitterWhen.ToString("MMMM");
                            data[row, column++] = commit.CommitterWhen.ToString("yyyy");
                            row++;
                        }

                        MyExcel.Write(data, mi.CommitCount, 17);

                        string fileName = string.Empty;
                        if (string.IsNullOrWhiteSpace(fileName))
                        {
                            fileName = DateTime.Now.ToString("dd MMM yy hh-mm") + " ChangesetData.xlsx";
                        }

                        MyExcel.SaveWorkBook(Path.Combine(Environment.CurrentDirectory, fileName), false);
                        MyExcel.Close();
                    }
                }
            }
        }

        private static void WriteArray(int rows, int columns, Worksheet worksheet)
        {
            var data = new object[rows, columns];
            for (var row = 1; row <= rows; row++)
            {
                for (var column = 1; column <= columns; column++)
                {
                    data[row - 1, column - 1] = "Test";
                }
            }

            var startCell = (Range)worksheet.Cells[1, 1];
            var endCell = (Range)worksheet.Cells[rows, columns];
            var writeRange = worksheet.Range[startCell, endCell];

            writeRange.Value2 = data;
        }
    }
}
