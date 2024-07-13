﻿using BE;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public static class DAL_Establishment
    {
        public static string GetEstablishment(string username)
        {
            DAL_DB_Connection con = new DAL_DB_Connection();
            MySqlCommand command = new MySqlCommand();

            try
            {
                command.Connection = con.OpenConnection();

                command.CommandText = @"
            SELECT e.establishmentName
            FROM tb_Establishment e
            INNER JOIN tb_EstablishmentUser eu ON e.idEstablishment = eu.idEstablishment
            WHERE eu.username = @p_username";
                command.Parameters.AddWithValue("@p_username", username);

                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return result.ToString();
                }
                else
                {
                    return null; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null; 
            }
        }

        public static DataTable GetEstablishments()
        {
            DAL_DB_Connection connection = new DAL_DB_Connection();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM tb_Establishment", connection.Connection);
            adapter.SelectCommand.CommandType = CommandType.Text;
            adapter.Fill(table);
            return table;
        }

        public static DataTable GetUserEstablishments(BE_User user)
        {
            DAL_DB_Connection connection = new DAL_DB_Connection();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter(@"SELECT e.idEstablishment, e.establishmentName, eu.username FROM tb_EstablishmentUser eu JOIN tb_Establishment e ON e.idEstablishment = eu.idEstablishment WHERE eu.username = @username", connection.Connection);
            adapter.SelectCommand.Parameters.AddWithValue("username", user.Username);
            adapter.SelectCommand.CommandType = CommandType.Text;
            adapter.Fill(table);
            return table;
        }

        public static bool RegisterEstablishment(BE_Establishment s)
        {
            DAL_DB_Connection con = new DAL_DB_Connection();
            MySqlCommand command = new MySqlCommand();

            try
            {
                command.Connection = con.OpenConnection();

                command.CommandText = "INSERT INTO tb_Establishment (establishmentName, direction, phone, email) VALUES (@name, @direction, @phone, @email)";
                command.Parameters.AddWithValue("@name", s.Name);
                command.Parameters.AddWithValue("@direction", s.Adress);
                command.Parameters.AddWithValue("@phone", s.Phone);
                command.Parameters.AddWithValue("@email", s.Email);

                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                con.CloseConnection();
            }
        }

        public static bool SetUserEstablishment(string username, string establishmentName)
        {
            DAL_DB_Connection con = new DAL_DB_Connection();
            MySqlCommand command = new MySqlCommand();

            try
            {
                command.Connection = con.OpenConnection();
                command.CommandText = "sp_SetEstablishmentUser";
                command.Parameters.AddWithValue("@p_establishmentName", establishmentName);
                command.Parameters.AddWithValue("@p_username", username);
                command.CommandType = CommandType.StoredProcedure;
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                con.CloseConnection();
            }
        }
    }
}