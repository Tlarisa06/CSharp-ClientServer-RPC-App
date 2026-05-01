using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Configuration;
using Triathlon.Model;

namespace Triathlon.Server.Repository
{
    public class EventDbRepo : IEventRepo
    {
        private string connectionString;

        public EventDbRepo()
        {
            connectionString = ConfigurationManager.ConnectionStrings["triathlonDB"].ConnectionString;
        }

        public void Add(Event e)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("INSERT INTO Events (id_ref, id_part, points) VALUES (@r, @p, @pts)", con))
                {
                    cmd.Parameters.AddWithValue("@r", e.IdRef);
                    cmd.Parameters.AddWithValue("@p", e.IdPart);
                    cmd.Parameters.AddWithValue("@pts", e.Points);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Remove(int id)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM Events WHERE id = @id", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Event e)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("UPDATE Events SET id_ref = @r, id_part = @p, points = @pts WHERE id = @id", con))
                {
                    cmd.Parameters.AddWithValue("@r", e.IdRef);
                    cmd.Parameters.AddWithValue("@p", e.IdPart);
                    cmd.Parameters.AddWithValue("@pts", e.Points);
                    cmd.Parameters.AddWithValue("@id", e.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Event FindById(int id)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("SELECT id, id_ref, id_part, points FROM Events WHERE id = @id", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Event(
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetInt32(2),
                                reader.GetInt32(3)
                            );
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<Event> GetAll()
        {
            IList<Event> allEvents = new List<Event>();
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("SELECT id, id_ref, id_part, points FROM Events", con))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allEvents.Add(new Event(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetInt32(2),
                            reader.GetInt32(3)
                        ));
                    }
                }
            }
            return allEvents;
        }

        public IEnumerable<Event> FindByRefereeId(int idReferee)
        {
            IList<Event> events = new List<Event>();
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("SELECT id, id_ref, id_part, points FROM Events WHERE id_ref = @idRef", con))
                {
                    cmd.Parameters.AddWithValue("@idRef", idReferee);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            events.Add(new Event(
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetInt32(2),
                                reader.GetInt32(3)
                            ));
                        }
                    }
                }
            }
            return events;
        }
        
        public Event FindByRefereeAndParticipant(int idReferee, int idParticipant)
        {
            using (var con = new SQLiteConnection(connectionString)) 
            {
                con.Open();
                using (var cmd = new SQLiteCommand("SELECT id, id_ref, id_part, points FROM Events WHERE id_ref = @idRef AND id_part = @idPart", con))
                {
                    cmd.Parameters.AddWithValue("@idRef", idReferee);
                    cmd.Parameters.AddWithValue("@idPart", idParticipant);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Event(
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetInt32(2),
                                reader.GetInt32(3)
                            );
                        }
                    }
                }
            }
            return null;
        }

        public int GetTotalPointsForParticipant(int idParticipant)
        {
            using (var con = new SQLiteConnection(connectionString)) 
            {
                con.Open();
                using (var cmd = new SQLiteCommand("SELECT SUM(points) FROM Events WHERE id_part = @idPart", con))
                {
                    cmd.Parameters.AddWithValue("@idPart", idParticipant);

                    var result = cmd.ExecuteScalar();
                    if (result == null || result == DBNull.Value) return 0;
                    return Convert.ToInt32(result);
                }
            }
        }
        
        public void RemoveAllByRefereeAndParticipant(int idReferee, int idParticipant)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM Events WHERE id_ref = @r AND id_part = @p", con))
                {
                    cmd.Parameters.AddWithValue("@r", idReferee);
                    cmd.Parameters.AddWithValue("@p", idParticipant);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}