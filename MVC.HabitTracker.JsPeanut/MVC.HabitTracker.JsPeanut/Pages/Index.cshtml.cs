﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using MVC.HabitTracker.JsPeanut.Models;
using System.Globalization;

namespace MVC.HabitTracker.JsPeanut.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public List<HabitLog> HabitLogs { get; set; }

        public List<HabitType> HabitTypes { get; set; }

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet(string sorttype)
        {
            HabitLogs = GetAllLogs();
            HabitTypes = GetAllHabitTypes();

            switch (sorttype)
            {
                case "Today":
                    HabitLogs = HabitLogs.Where(x => x.Date.Day == DateTime.Now.Day).Select(x => x).ToList();
                    break;
                case "Yesterday":
                    HabitLogs = HabitLogs.Where(x => x.Date.Day == (DateTime.Now.Day - 1)).Select(x => x).ToList();
                    break;
                case "DateAdded":
                    HabitLogs = HabitLogs.OrderByDescending(x => x.Date).ToList();
                    break;
                case "AddedOrder":
                    HabitLogs = GetAllLogs();
                    break;
            }
        }

        private List<HabitLog> GetAllLogs()
        {
            using (var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                    "SELECT * FROM habit_logs";

                var tableData = new List<HabitLog>();
                SqliteDataReader reader = tableCmd.ExecuteReader();

                while (reader.Read())
                {
                    tableData.Add(
                        new HabitLog
                        {
                            Id = reader.GetInt32(0),
                            HabitTypeName = reader.GetString(1),
                            //HabitType = new HabitType
                            //{
                            //    ImagePath = reader.GetString(1),
                            //    Name = reader.GetString(2),
                            //    UnitOfMeasurement = reader.GetString(3)
                            //},
                            Date = DateTime.Parse(reader.GetString(2), CultureInfo.InstalledUICulture),
                            Quantity = reader.GetInt32(3)
                        }
                    );
                }

                return tableData;
            }
        }

        private List<HabitType> GetAllHabitTypes()
        {
            using (var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                    "SELECT * FROM habit_types";

                var tableData = new List<HabitType>();
                SqliteDataReader reader = tableCmd.ExecuteReader();

                while (reader.Read())
                {
                    tableData.Add(
                        new HabitType
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            ImagePath = reader.GetString(2),
                            UnitOfMeasurement = reader.GetString(3)
                        }
                    );
                }

                return tableData;
            }
        }

        public IActionResult OnPost()
        {
            HabitLogs = GetAllLogs();

            HabitTypes = GetAllHabitTypes();

            HabitLogs = HabitLogs.Where(x => x.Date.Day == DateTime.Now.Day).Select(x => x).ToList();

            return Page();
        }
    }
}