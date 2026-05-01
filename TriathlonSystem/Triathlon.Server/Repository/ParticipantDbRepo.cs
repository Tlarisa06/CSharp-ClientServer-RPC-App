using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Configuration;
using Triathlon.Model;

namespace Triathlon.Server.Repository
{
    public class ParticipantDbRepo : IParticipantRepo
    {
        private string connectionString;

        public ParticipantDbRepo()
        {
            connectionString = ConfigurationManager.ConnectionStrings["triathlonDB"].ConnectionString;
        }

        public void Add(Participant p)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("INSERT INTO Participants (name, total_points) VALUES (@n, @tp)", con))
                {
                    cmd.Parameters.AddWithValue("@n", p.Name);
                    cmd.Parameters.AddWithValue("@tp", p.TotalPoints);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Remove(int id)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM Participants WHERE id = @id", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Participant p)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("UPDATE Participants SET name = @n, total_points = @tp WHERE id = @id", con))
                {
                    cmd.Parameters.AddWithValue("@n", p.Name);
                    cmd.Parameters.AddWithValue("@tp", p.TotalPoints);
                    cmd.Parameters.AddWithValue("@id", p.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Participant FindById(int id)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("SELECT id, name, total_points FROM Participants WHERE id = @id", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Participant(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetInt32(2)
                            );
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<Participant> GetAll()
        {
            var participants = new List<Participant>();
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("SELECT id, name, total_points FROM Participants", con))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        participants.Add(new Participant(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetInt32(2)
                        ));
                    }
                }
            }
            return participants;
        }

        public IEnumerable<Participant> FindAllSortedByPoints()
        {
            var participants = new List<Participant>();
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                // Ordonăm descrescător (DESC) după punctaj pentru clasament
                using (var cmd = new SQLiteCommand("SELECT id, name, total_points FROM Participants ORDER BY total_points DESC", con))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        participants.Add(new Participant(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetInt32(2)
                        ));
                    }
                }
            }
            return participants;
        }
    }
}