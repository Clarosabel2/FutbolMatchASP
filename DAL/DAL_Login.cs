﻿using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_Login
    {
        public static DataTable GetUsers()
        {
            DAL_DB_Connection connection = new DAL_DB_Connection();
            SqlCommand command = new SqlCommand();
            DataTable table = new DataTable();
            command.Connection = connection.OpenConnection();
            command.CommandText = "SELECT * FROM tb_User";
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            SqlDataReader reader = command.ExecuteReader();
            table.Load(reader);
            command.Connection = connection.CloseConnection();
            return table;
        }

        public static bool UserExist(string user, string password)
        {
            DAL_DB_Connection connection = new DAL_DB_Connection();
            SqlCommand command = new SqlCommand();
            BE_Login login = new BE_Login();
            command.Connection = connection.OpenConnection();
            command.CommandText = "sp_UserExist";
            command.Parameters.AddWithValue("@username", user);
            command.Parameters.AddWithValue("@password", password);
            command.CommandType = CommandType.StoredProcedure;
            int result = (int)command.ExecuteScalar();
            command.Connection = connection.CloseConnection();
            return result == 1;
        }
    }
}
