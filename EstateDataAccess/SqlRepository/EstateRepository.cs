using EstateDataAccess.Repository;
using EstateModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace EstateDataAccess.SqlRepository
{
    public class EstateRepository : SQLRepository<Estate>
    {
        public override Estate GetById(int id)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    var query = "SELECT EstateId, Name, Address, OwnerId, Price, Type FROM Estate WHERE EstateId = @id";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", id);

                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        var estate = new Estate
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("EstateId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            Price = Convert.ToDouble(reader.GetDecimal(reader.GetOrdinal("Price"))),
                            Type = (EstateType)reader.GetInt32(reader.GetOrdinal("Type"))
                        };

                        var pictureRepository = new PictureRepository();
                        estate.Pictures = pictureRepository.GetAll().Where(p => p.EstateId == estate.Id).ToList();

                        return estate;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching estate by ID: {ex.Message}");
                throw;
            }
        }




        public override List<Estate> GetAll()
        {
            var estates = new List<Estate>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM Estate", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var estate = new Estate
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EstateId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                CreateDate = reader.GetDateTime(reader.GetOrdinal("CreateDate")),
                            };

                            if (reader["Price"] != DBNull.Value)
                            {
                                estate.Price = (double)reader.GetDecimal(reader.GetOrdinal("Price"));
                            }
                            else
                            {
                                estate.Price = 0.0;
                            }


                            if (reader["Type"] != DBNull.Value)
                            {
                                estate.Type = (EstateType)reader.GetInt32(reader.GetOrdinal("Type"));
                            }
                            else
                            {
                                estate.Type = EstateType.Unknown;
                            }

                            estates.Add(estate);
                        }
                    }
                }
            }

            return estates;
        }


        public override Estate Create(Estate entity)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "INSERT INTO Estate (Name, Address, Price, Type, CreateDate, OwnerId) OUTPUT INSERTED.EstateId VALUES (@Name, @Address, @Price, @Type, @CreateDate, @OwnerId)",
                    connection);
                command.Parameters.AddWithValue("@Name", entity.Name);
                command.Parameters.AddWithValue("@Address", entity.Address);
                command.Parameters.AddWithValue("@Price", entity.Price);
                command.Parameters.AddWithValue("@Type", entity.Type);
                command.Parameters.AddWithValue("@CreateDate", entity.CreateDate);
                command.Parameters.AddWithValue("@OwnerId", entity.OwnerId);

                entity.Id = (int)command.ExecuteScalar();
            }
            return entity;
        }

        public override Estate Update(Estate entity)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "UPDATE Estate SET Name = @Name, Address = @Address, Price = @Price, Type = @Type, OwnerId = @OwnerId WHERE EstateId = @id",
                    connection);
                command.Parameters.AddWithValue("@Name", entity.Name);
                command.Parameters.AddWithValue("@Address", entity.Address);
                command.Parameters.AddWithValue("@Price", entity.Price);
                command.Parameters.AddWithValue("@Type", entity.Type);
                command.Parameters.AddWithValue("@OwnerId", entity.OwnerId);
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
                var command = new SqlCommand("DELETE FROM Estate WHERE EstateId = @id", connection);
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
        }

        public override IEnumerable<Estate> SearchAndSort(string searchTerm, string sortBy, bool descending)
        {
            var validColumns = new List<string> { "Name", "Price", "Type", "CreateDate" };
            if (!validColumns.Contains(sortBy))
            {
                throw new ArgumentException("Invalid sort column");
            }

            var query = "SELECT * FROM Estate WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query += " AND (Name LIKE @SearchTerm OR Address LIKE @SearchTerm)";
            }

            query += $" ORDER BY {sortBy} {(descending ? "DESC" : "ASC")}";

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
                    var estates = new List<Estate>();
                    while (reader.Read())
                    {
                        estates.Add(new Estate
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("EstateId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            Price = reader["Price"] != DBNull.Value ? Convert.ToDouble(reader["Price"]) : 0.0,
                            Type = reader["Type"] != DBNull.Value && Enum.IsDefined(typeof(EstateType), reader.GetInt32(reader.GetOrdinal("Type")))
                                   ? (EstateType)reader.GetInt32(reader.GetOrdinal("Type"))
                                   : EstateType.Unknown,
                            CreateDate = reader["CreateDate"] != DBNull.Value
                                         ? reader.GetDateTime(reader.GetOrdinal("CreateDate"))
                                         : DateTime.MinValue,
                            OwnerId = reader["OwnerId"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("OwnerId")) : 0
                        });
                    }
                    return estates;
                }
            }
        }




    }
}
