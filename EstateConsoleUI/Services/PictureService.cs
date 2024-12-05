using System;
using System.IO;
using System.Drawing;
using System.Configuration;
using System.Data.SqlClient;

namespace EstateConsoleUI.Services
{
    public static class PictureService
    {
        private static string picturesFolder = System.Configuration.ConfigurationManager.AppSettings["PicturesFolder"];

        public static void AddPicture(int estateId, string pictureFileName, long fileSize)
        {
            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["EstateManagementDb"].ConnectionString))
                {
                    connection.Open();
                    var query = "INSERT INTO Picture (EstateId, Name, Size) VALUES (@EstateId, @Name, @Size)";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EstateId", estateId);
                    command.Parameters.AddWithValue("@Name", pictureFileName);
                    command.Parameters.AddWithValue("@Size", fileSize);

                    command.ExecuteNonQuery();
                    Console.WriteLine("Picture added successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding the picture: {ex.Message}");
            }
        }

        public static void ViewPictures(int estateId)
        {
            try
            {
                var files = Directory.GetFiles(picturesFolder, $"estate_{estateId}_*.jpg");

                if (files.Length > 0)
                {
                    Console.WriteLine("Pictures for Estate ID " + estateId + ":");
                    foreach (var file in files)
                    {
                        Console.WriteLine(file);
                    }
                }
                else
                {
                    Console.WriteLine("No pictures found for this estate.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error viewing pictures: {ex.Message}");
            }
        }
    }
}
