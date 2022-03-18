using System;
using System.Data.SqlClient;

namespace Server.Database
{
    public static class Database
    {
        public static string DBIP { get; set; }
        public static string DBName { get; set; }
        public static string DBID { get; set; }
        public static string DBPW { get; set; }

        public static SqlConnection Connection { get; private set; }
        public static void Init(string ip, string name, string id, string pw)
        {
            DBIP = ip;
            DBName = name;
            DBID = id;
            DBPW = pw;
            string ConnectionString = $"Data Source={DBIP};Database={DBName};User ID={DBID};Password={DBPW};";
            Connection = new SqlConnection(ConnectionString);
        }

        public static void CheckConnection()
        {
            if (Connection.State != System.Data.ConnectionState.Open)
                Connection.Open();
        }

        public static T Select<T>(string Table, string Restraints, params object[] args)
        {
            CheckConnection();
            object result = null;
            using (SqlCommand cmd = new SqlCommand($"SELECT TOP 1 * FROM {Table} AS Result WHERE {string.Format(Restraints, args)}", Connection))
            {
                var dr = cmd.ExecuteReader();
                dr.Read();
                if(dr["Result"] == DBNull.Value)
                {
                    if (typeof(T) == typeof(int))
                        result = -1;
                    else if (typeof(T) == typeof(long))
                        result = (long)-1;
                    else if (typeof(T) == typeof(short))
                        result = (short)-1;
                    else if (typeof(T) == typeof(string))
                        result = "";
                    else if (typeof(T) == typeof(bool))
                        result = false;
                    else if (typeof(T) == typeof(byte))
                        result = (byte)255;
                }
                else
                {
                    if (typeof(T) == typeof(int))
                        result = (int)dr["Result"];
                    else if (typeof(T) == typeof(long))
                        result = (long)dr["Result"];
                    else if (typeof(T) == typeof(short))
                        result = (short)dr["Result"];
                    else if (typeof(T) == typeof(string))
                        result = dr["Result"].ToString();
                    else if (typeof(T) == typeof(bool))
                        result = (bool)dr["Result"];
                    else if (typeof(T) == typeof(byte))
                        result = (byte)dr["Result"];
                }
                dr.Close();
            }
            return (T)result;
        }

    }
}
