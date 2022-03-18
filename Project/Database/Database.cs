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
            if(Connection.State != System.Data.ConnectionState.Open)
                Connection.Open();
        }

        public static void Select()
        {

        }
    }
}
