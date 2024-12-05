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
    public class OwnerRepository : SQLRepository<Owner>
    {
        public override Owner GetById(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Owner WHERE OwnerId = @id", connection);
                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Owner
                        {
                            Id = (int)reader["OwnerId"],
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Cnp = reader["Cnp"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        public override List<Owner> GetAll()
        {
            var owners = new List<Owner>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Owner", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        owners.Add(new Owner
                        {
                            Id = (int)reader["OwnerId"],
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Cnp = reader["Cnp"].ToString()
                        });
                    }
                }
            }
            return owners;
        }

        public override Owner Create(Owner entity)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "INSERT INTO Owner (Name, Email, Phone, Cnp) OUTPUT INSERTED.OwnerId VALUES (@Name, @Email, @Phone, @Cnp)",
                    connection);
                command.Parameters.AddWithValue("@Name", entity.Name);
                command.Parameters.AddWithValue("@Email", entity.Email);
                command.Parameters.AddWithValue("@Phone", entity.Phone);
                command.Parameters.AddWithValue("@Cnp", entity.Cnp);

                entity.Id = (int)command.ExecuteScalar();
            }
            return entity;
        }

        public override Owner Update(Owner entity)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "UPDATE Owner SET Name = @Name, Email = @Email, Phone = @Phone, Cnp = @Cnp WHERE OwnerId = @id",
                    connection);
                command.Parameters.AddWithValue("@Name", entity.Name);
                command.Parameters.AddWithValue("@Email", entity.Email);
                command.Parameters.AddWithValue("@Phone", entity.Phone);
                command.Parameters.AddWithValue("@Cnp", entity.Cnp);
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
                var command = new SqlCommand("DELETE FROM Owner WHERE OwnerId = @id", connection);
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
        }

        public override IEnumerable<Owner> SearchAndSort(string searchTerm, string sortBy, bool descending)
        {
            var query = "SELECT * FROM Owner WHERE 1=1";

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query += " AND (Name LIKE @SearchTerm OR Email LIKE @SearchTerm)";
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
                    var owners = new List<Owner>();
                    while (reader.Read())
                    {
                        owners.Add(new Owner
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Email = reader.GetString(2),
                            Phone = reader.GetString(3),
                          
                        });
                    }

                    return owners;
                }
            }
        }
    }
}