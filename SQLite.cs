﻿//This code was generated by a tool.
//Changes to this file will be lost if the code is regenerated.
// See the blog post here for help on using the generated code: http://erikej.blogspot.dk/2014/10/database-first-with-sqlite-in-universal.html
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Chat;
using Windows.Storage;
using SQLite.Net;
using SQLite.Net.Attributes;
using SQLite.Net.Platform.WinRT;

namespace Memory
{
    public class SQLiteDb
    {         
        private SQLiteConnection db;
        public SQLiteDb()
        {
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite");
            db = new SQLiteConnection(new SQLitePlatformWinRT(), path);
            //db.TraceListener = new DebugTraceListener();
        }

        public void Create()
        {
            db.CreateTable<Score>();
        }

        public void AddHighscore(String name, int time, int incorrectPairs, int points)
        {
            var score = new Score
            {
                Name = name,
                IncorrectPairs = incorrectPairs,
                TimeInSeconds = time,
                Points = points
            };
            db.Insert(score);
        }

        public List<Score> GetTopHighscores()
        {
            //get all
            List<Score> users = (from u in db.Table<Score>()
                                orderby u.IncorrectPairs, u.TimeInSeconds ascending
                                   select u
                                   ).Take(10).ToList();
            
            return users;
            /*s
            get user by id 
            User m = (from u in db.Table<User>()
                        where u.Id == Id
                        select u).FirstOrDefault();
                        */
        }

        public void DeleteAllHighscores()
        {
            db.DeleteAll<Score>();
        }

        public void Update()
        {
            User m = (from u in db.Table<User>()
                      where u.Naam == "test"
                      select u).FirstOrDefault();
            
            m.Naam = "testupdate";
            db.Update(m);
        }

        public void Saveandclose()
        {
            db.Close();
        }
    }

    public partial class Score
    {
        [PrimaryKey, AutoIncrement]
        public Int32 Id { get; set; }

        public string Name { get; set; }

        public int IncorrectPairs { get; set; }

        public int Points { get; set; }

        public int TimeInSeconds { get; set; }
    }

    public partial class User
    {
        [PrimaryKey, AutoIncrement]
        public Int64 Id { get; set; }

        public String Naam { get; set; }

    }

}
