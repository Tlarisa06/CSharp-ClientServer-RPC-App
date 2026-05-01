using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Configuration;
using Triathlon.Model;

namespace Triathlon.Server.Repository
{
    public class RefereeDbRepo : IRefereeRepo
    {
        private string connectionString;

        public RefereeDbRepo()
        {
            connectionString = ConfigurationManager.ConnectionStrings["triathlonDB"].ConnectionString;
        }

        public Referee FindByUsernameAndPassword(string username, string password)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("SELECT id, name, password, id_event FROM Referees WHERE name=@user AND password=@pass", con))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Referee(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetInt32(3)
                            );
                        }
                    }
                }
            }
            return null;
        }

        public void Add(Referee element)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("INSERT INTO Referees (name, password, id_event) VALUES (@name, @pass, @idEv)", con))
                {
                    cmd.Parameters.AddWithValue("@name", element.Name);
                    cmd.Parameters.AddWithValue("@pass", element.Password);
                    cmd.Parameters.AddWithValue("@idEv", element.IdEvent);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Remove(int id)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM Referees WHERE id=@id", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Referee element)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("UPDATE Referees SET name=@name, password=@pass, id_event=@idEv WHERE id=@id", con))
                {
                    cmd.Parameters.AddWithValue("@name", element.Name);
                    cmd.Parameters.AddWithValue("@pass", element.Password);
                    cmd.Parameters.AddWithValue("@idEv", element.IdEvent);
                    cmd.Parameters.AddWithValue("@id", element.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Referee FindById(int id)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("SELECT id, name, password, id_event FROM Referees WHERE id=@id", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Referee(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetInt32(3)
                            );
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<Referee> GetAll()
        {
            IList<Referee> referees = new List<Referee>();
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("SELECT id, name, password, id_event FROM Referees", con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            referees.Add(new Referee(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetInt32(3)
                            ));
                        }
                    }
                }
            }
            return referees;
        }
    }
}