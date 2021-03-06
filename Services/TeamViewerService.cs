using DOCToolBackend.Models;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;

namespace DOCToolBackend.Services {
    public static class TeamViewerService
    {
        static readonly string dbPath;

        static TeamViewerService() {
            dbPath = "DOCTool.db";
        }

        // <summary>
        // Retrieves all TeamViewerIDS rows from DB and returns them as a list.
        // </summmary>
        // <exceptions>
        // SqliteException : db problem
        // </exceptions>
        public static List<TeamViewer> GetAll() {
            List <TeamViewer> teamViewers = new List<TeamViewer>();
            using (var connection = new SqliteConnection("Data Source=" + dbPath)) {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = 
                @"
                    SELECT HostName, TeamViewerID, HWID, UserName FROM TeamViewerIDS;
                ";

                using (var reader = command.ExecuteReader()) {
                    while(reader.Read()) {
                        TeamViewer teamViewer = new TeamViewer();
                        teamViewer.HostName = reader.GetString(0);
                        teamViewer.TeamViewerID = reader.GetString(1);
                        if (!reader.IsDBNull(2)) {
                            teamViewer.HWID = reader.GetString(2);
                        }
                        if (!reader.IsDBNull(3)) {
                            teamViewer.UserName = reader.GetString(3);
                        }
                        teamViewers.Add(teamViewer);
                    }
                }                
                return teamViewers;
            }
        }

        public static TeamViewer? Get(string? hostName) {
            if (hostName is null) {
                return null;
            }

            using (var connection = new SqliteConnection("Data Source=" + dbPath)) {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = 
                @"
                    SELECT HostName, TeamViewerID, HWID, UserName FROM TeamViewerIDS
                    WHERE HostName=$HostName;
                ";
                command.Parameters.AddWithValue("$HostName", hostName);

                using (SqliteDataReader reader = command.ExecuteReader()) {
                    if(reader.Read()) {
                        TeamViewer teamViewer = new TeamViewer();
                        teamViewer.HostName = reader.GetString(0);
                        teamViewer.TeamViewerID = reader.GetString(1);
                        if (!reader.IsDBNull(2)) {
                            teamViewer.HWID = reader.GetString(2);
                        }
                        if (!reader.IsDBNull(3)) {
                            teamViewer.UserName = reader.GetString(3);
                        }
                        return teamViewer;
                    } else {
                        return null;
                    }
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
                    SELECT HWID FROM HWIWorkstationCache
                    WHERE AssetTag=$AssetTag;
                ";
                command.Parameters.AddWithValue("$AssetTag", "LHI" + teamViewer.HostName.Substring(3));

                using (SqliteDataReader reader = command.ExecuteReader()) {
                    if(reader.Read()) {
                        teamViewer.HWID = reader.GetString(0);
                    }
                }
            }

            using (var connection = new SqliteConnection("Data Source=" + dbPath)) {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = 
                @"
                    INSERT INTO TeamViewerIDS (HostName, TeamViewerID, HWID, UserName) VALUES ($HostName, $TeamViewerID, $HWID, $UserName)
                ";
                command.Parameters.AddWithValue("$HostName", teamViewer.HostName);
                command.Parameters.AddWithValue("$TeamViewerID", teamViewer.TeamViewerID);
                command.Parameters.AddWithValue("$HWID", (teamViewer.HWID != null ? teamViewer.HWID : DBNull.Value));
                command.Parameters.AddWithValue("$UserName", (teamViewer.UserName != null ? teamViewer.UserName : DBNull.Value));

                command.ExecuteNonQuery();
            }
        }

        public static void Delete(string hostName)
        {
            var teamViewer = Get(hostName);
            if(teamViewer is null)
                return;

            using (var connection = new SqliteConnection("Data Source=" + dbPath)) {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    DELETE FROM TeamViewerIDS
                    WHERE HostName=$HostName;
                ";
                command.Parameters.AddWithValue("$HostName", hostName);

                command.ExecuteNonQuery();
            }
        }

        public static void Update(TeamViewer teamViewer) {
            TeamViewer? existingTeamViewer = Get(teamViewer.HostName);

            if (existingTeamViewer is null) {
                return;
            }

            using (var connection = new SqliteConnection("Data Source=" + dbPath)) {
                connection.Open();
                
                var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE TeamViewerIDS
                    SET TeamViewerID = $TeamViewerID
                    WHERE HostName = $HostName;
                ";
                command.Parameters.AddWithValue("$HostName", teamViewer.HostName);
                command.Parameters.AddWithValue("$TeamViewerID", teamViewer.TeamViewerID);

                command.ExecuteNonQuery();
            }
        }
    }
}
