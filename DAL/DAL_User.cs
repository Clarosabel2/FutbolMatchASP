﻿using BE;
using MySqlConnector;
using System;
using System.Data;

namespace DAL
{
    public class DAL_User
    {
        public static BE_User GetUserByUsername(string username)
        {
            DAL_DB_Connection connection = new DAL_DB_Connection();
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection.OpenConnection();
            command.CommandText = "sp_GetUserByUsername";
            command.Parameters.AddWithValue("@p_username", username);
            command.CommandType = CommandType.StoredProcedure;
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                BE_User user = new BE_User
                {
                    Username = reader["username"].ToString(),
                    Password = reader["password"].ToString(),
                    Name = reader["name"].ToString(),
                    Lastname = reader["lastname"].ToString(),
                    Email = reader["email"].ToString(),
                    Phone = reader["phone"] != DBNull.Value ? reader["phone"].ToString() : "",
                    Role = reader["roleName"].ToString(),
                    Language = Convert.ToInt32(reader["language"]),
                    Blocked = Convert.ToBoolean(reader["blocked"]),
                    Removed = Convert.ToBoolean(reader["removed"])
                };
                command.Connection = connection.CloseConnection();
                return user;
            }
            else
            {
                command.Connection = connection.CloseConnection();
                return null;
            }
        }

        #region Métodos ABML

        public static bool DeleteUser(string username)
        {
            DAL_DB_Connection connection = new DAL_DB_Connection();
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection.OpenConnection();
            command.CommandText = @"UPDATE tb_User SET removed = 1 WHERE username = @p_username;";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@p_username", username);
            int rowsAffected = command.ExecuteNonQuery();
            command.Connection = connection.CloseConnection();
            return rowsAffected > 0;
        }

        public static bool BlockUser(string username)
        {
            DAL_DB_Connection connection = new DAL_DB_Connection();
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection.OpenConnection();
            command.CommandText = @"UPDATE tb_User SET blocked = 1 WHERE username = @p_username;";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@p_username", username);
            int rowsAffected = command.ExecuteNonQuery();
            command.Connection = connection.CloseConnection();
            return rowsAffected > 0;
        }

        public static DataTable GetUsers()
        {
            DAL_DB_Connection connection = new DAL_DB_Connection();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter("sp_GetUsers", connection.Connection);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.Fill(table);
            return table;
        }

        public static bool InsertUser(BE_User user)
        {
            DAL_DB_Connection connection = new DAL_DB_Connection();
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection.OpenConnection();
            command.CommandText = "sp_InsertUser";
            command.Parameters.AddWithValue("@p_username", user.Username);
            command.Parameters.AddWithValue("@p_password", user.Password);
            command.Parameters.AddWithValue("@p_name", user.Name);
            command.Parameters.AddWithValue("@p_lastname", user.Lastname);
            command.Parameters.AddWithValue("@p_email", user.Email);
            command.Parameters.AddWithValue("@p_phone", user.Phone);
            command.Parameters.AddWithValue("@p_roleName", user.Role);
            command.Parameters.AddWithValue("@p_language", user.Language);
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();
            command.Connection = connection.CloseConnection();
            //AGREGAR TRY CATCH
            return true;
        }

        public static bool UpdateUser(BE_User user)
        {
            DAL_DB_Connection cnn = new DAL_DB_Connection();
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                cmd.Connection = cnn.OpenConnection();
                cmd.CommandText = "sp_UpdateUser";
                cmd.Parameters.AddWithValue("@p_username", user.Username);
                cmd.Parameters.AddWithValue("@p_name", user.Name);
                cmd.Parameters.AddWithValue("@p_lastname", user.Lastname);
                cmd.Parameters.AddWithValue("@p_email", user.Email);
                cmd.Parameters.AddWithValue("@p_phone", user.Phone);
                cmd.Parameters.AddWithValue("@p_roleName", user.Role);
                cmd.Parameters.AddWithValue("@p_language", user.Language);
                cmd.Parameters.AddWithValue("@p_blocked", user.Blocked);
                cmd.Parameters.AddWithValue("@p_removed", user.Removed);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                cmd.Connection = cnn.CloseConnection();
                return true;

            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static bool UpdateMyAccount(BE_User user)
        {
            Console.WriteLine(" DAL UDPATE MY ACCOUNT");
            DAL_DB_Connection connection = new DAL_DB_Connection();
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection.OpenConnection();
            command.CommandText = "sp_UpdateMyAccount";
            command.Parameters.AddWithValue("@p_username", user.Username);
            command.Parameters.AddWithValue("@p_name", user.Name);
            command.Parameters.AddWithValue("@p_lastname", user.Lastname);
            command.Parameters.AddWithValue("@p_email", user.Email);
            command.Parameters.AddWithValue("@p_phone", user.Phone);
            command.Parameters.AddWithValue("@p_language", user.Language);
            command.Parameters.AddWithValue("@p_password", user.Password);
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();
            command.Connection = connection.CloseConnection();
            //AGREGAR TRY CATCH
            return true;
        }
        #endregion
    }
}
