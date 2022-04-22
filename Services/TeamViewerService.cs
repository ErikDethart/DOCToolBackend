using DOCToolBackend.Models;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace DOCToolBackend.Services {
    public static class TeamViewerService
    {
        static readonly string dbPath;
        static List<TeamViewer> TeamViewers { get; }

        static TeamViewerService() {
            TeamViewers = new List<TeamViewer> ();
            dbPath = "DOCTool.db";
        }

        // <summary>
        // Retrieves all TeamViewerIDS rows from DB and 
        // </summmary>
        public static List<TeamViewer> GetAll() {
            List <TeamViewer> teamViewers = new List<TeamViewer>();
            using (var connection = new SqliteConnection("Data Source=" + dbPath)) {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = 
                @"
                    SELECT * FROM TeamViewerIDS;
                ";

                try {
                    using (var reader = command.ExecuteReader()) {
                        while(reader.Read()) {
                            TeamViewer teamViewer = new TeamViewer();
                            teamViewer.HostName = reader.GetString(0);
                            teamViewer.TeamViewerID = reader.GetString(1);
                            teamViewers.Add(teamViewer);
                        }
                    }
                } catch (SqliteException e) {
                    throw e;
                }
                
                return teamViewers;
            }
        }

        public static TeamViewer Get(string hostName) {
            using (var connection = new SqliteConnection("Data Source=" + dbPath)) {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = 
                @"
                    SELECT * FROM TeamViewerIDS
                    WHERE HostName=$HostName;
                ";
                command.Parameters.AddWithValue("$HostName", hostName);

                try {
                    using (SqliteDataReader reader = command.ExecuteReader()) {
                        if(reader.Read()) {
                            TeamViewer teamViewer = new TeamViewer();
                            teamViewer.HostName = reader.GetString(0);
                            teamViewer.TeamViewerID = reader.GetString(1);
                            return teamViewer;
                        } else {
                            throw new System.ArgumentException("Entry not found: " + hostName + " does not exist in db.");
                        }
                    }
                } catch (SqliteException e) {
                    throw e;
                }
            }
        }

        public static void Add(TeamViewer teamViewer)
        {
            using (var connection = new SqliteConnection("Data Source=" + dbPath)) {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = 
                @"
                    INSERT INTO TeamViewerIDS (HostName, TeamViewerID) VALUES ($HostName, $TeamViewerID)
                ";
                command.Parameters.AddWithValue("$HostName", teamViewer.HostName);
                command.Parameters.AddWithValue("$TeamViewerID", teamViewer.TeamViewerID);

                try {
                    command.ExecuteNonQuery();
                } catch (SqliteException e) {
                    throw e;
                }
            }
        }

        public static void Delete(string hostName)
        {
            var teamViewer = Get(hostName);
            if(teamViewer is null)
                return;

            TeamViewers.Remove(teamViewer);
        }

        public static void Update(TeamViewer teamViewer)
        {
            var index = TeamViewers.FindIndex(p => p.HostName == teamViewer.HostName);
            if(index == -1)
                return;

            TeamViewers[index] = teamViewer;
        }
    }
}
