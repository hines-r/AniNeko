﻿using Dapper;
using AniNeko.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace AniNeko
{
    public class SQLiteDataAccess
    {
        private static readonly string TableName = "Animes";

        public static List<AnimeModel> LoadAnime()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<AnimeModel>("SELECT * FROM " + TableName, new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SaveAnime(AnimeModel anime)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"INSERT INTO {TableName} (AnimeName, CurrentEpisode, TotalEpisodes, WatchStatus, Rating) " +
                    "VALUES (@AnimeName, @CurrentEpisode, @TotalEpisodes, @WatchStatus, @Rating)", anime);
            }
        }

        public static AnimeModel GetLastRecord()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<AnimeModel>($"SELECT * FROM {TableName} WHERE Id = (SELECT MAX(Id) FROM {TableName})").FirstOrDefault();
                return output;
            }
        }

        public static void UpdateAnime(AnimeModel anime)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"UPDATE {TableName} SET " +
                    "AnimeName = @AnimeName, " +
                    "CurrentEpisode = @CurrentEpisode, " +
                    "TotalEpisodes = @TotalEpisodes, " +
                    "WatchStatus = @WatchStatus, " +
                    "Rating = @Rating " +
                    "WHERE Id = @Id", anime);
            }
        }

        public static void DeleteAnime(AnimeModel anime)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"DELETE FROM {TableName} WHERE Id = @Id", anime);
            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
