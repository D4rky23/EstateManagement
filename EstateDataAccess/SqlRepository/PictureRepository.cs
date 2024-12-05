using EstateDataAccess.Repository;
using EstateModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstateDataAccess.SqlRepository
{
    public class PictureRepository : SQLRepository<Picture>
    {
        public override Picture GetById(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Picture WHERE PictureId = @id", connection);
                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Picture
                        {
                            Id = (int)reader["PictureId"],
                            Name = reader["Name"].ToString(),
                            CreateDate = (DateTime)reader["CreateDate"],
                            Size = (long)reader["Size"]
                        };
                    }
                }
            }
            return null;
        }

        public override List<Picture> GetAll()
        {
            var pictures = new List<Picture>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Picture", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pictures.Add(new Picture
                        {
                            Id = (int)reader["PictureId"],
                            Name = reader["Name"].ToString(),
                            CreateDate = (DateTime)reader["CreateDate"],
                            Size = (long)reader["Size"]
                        });
                    }
                }
            }
            return pictures;
        }

        public override Picture Create(Picture entity)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "INSERT INTO Picture (Name, CreateDate, Size) OUTPUT INSERTED.PictureId VALUES (@Name, @CreateDate, @Size)",
                    connection);
                command.Parameters.AddWithValue("@Name", entity.Name);
                command.Parameters.AddWithValue("@CreateDate", entity.CreateDate);
                command.Parameters.AddWithValue("@Size", entity.Size);

                entity.Id = (int)command.ExecuteScalar();
            }
            return entity;
        }

        public override Picture Update(Picture entity)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "UPDATE Picture SET Name = @Name, CreateDate = @CreateDate, Size = @Size WHERE PictureId = @id",
                    connection);
                command.Parameters.AddWithValue("@Name", entity.Name);
                command.Parameters.AddWithValue("@CreateDate", entity.CreateDate);
                command.Parameters.AddWithValue("@Size", entity.Size);
                command.Parameters.AddWithValue("@id", entity.Id);

                command.ExecuteNonQuery();
            }
            return entity;
        }

        public override void Delete(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Picture WHERE PictureId = @id", connection);
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
        }
        public override IEnumerable<Picture> SearchAndSort(string searchTerm, string sortBy, bool descending)
        {
            var query = "SELECT * FROM Picture WHERE 1=1";

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query += " AND (FileName LIKE @SearchTerm)";
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query += $" ORDER BY {sortBy} {(descending ? "DESC" : "ASC")}";
            }

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(query, connection);

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                }

                using (var reader = command.ExecuteReader())
                {
                    var pictures = new List<Picture>();
                    while (reader.Read())
                    {
                        pictures.Add(new Picture
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Size = reader.GetInt64(2),
                            CreateDate = reader.GetDateTime(3),
                            EstateId = reader.GetInt32(4)
                        });
                    }

                    return pictures;
                }
            }
        }
    }

}