using KsiazkaTelefonicznaWojtas.Enums;
using Microsoft.Data.Sqlite;
namespace KsiazkaTelefonicznaWojtas.MVVM.Models.Database;

public class SQLite
{

    private static SQLite? Instance { get; set; }
    private static SqliteConnection? Connection { get; set; }
    private SQLite() { }

    public static SQLite GetInstance()
    {
        if (Instance == null)
        {
            Instance = new SQLite();
            var dbFileName = "contactsdb1.sqlite3";
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), dbFileName);
           
            Connection = new SqliteConnection($"Data Source={dbPath}");
            Connection.Open();

            using var command = Connection.CreateCommand();
            command.CommandText =
                @"CREATE TABLE IF NOT EXISTS Contacts (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT,
                        AreaCode INTEGER,
                        Number INTEGER
                    );";
            command.ExecuteNonQuery();
            Connection.Close();
        }
        return Instance;
    }

    public List<_Contact> GetContacts(string? search = null, OrderBy orderBy = OrderBy.Name)
    {
        if (Connection == null) return new List<_Contact>();
        Connection.Open();
        
        string query = "SELECT * FROM Contacts";
        if (!string.IsNullOrWhiteSpace(search))
        {
            query += " WHERE LOWER(Name) LIKE @search";
        }
        using (var command = new SqliteCommand(query, Connection))
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                command.Parameters.AddWithValue("@search", $"%{search.ToLower()}%");
            }
            
            command.ExecuteNonQuery();
            
            SqliteDataReader reader = command.ExecuteReader();
            List<_Contact> contacts = new  List<_Contact>();
            while (reader.Read())
            {
                string? name = reader["Name"].ToString();
                short countryNumber = Convert.ToInt16(reader["AreaCode"]);
                int number = Convert.ToInt32(reader["Number"]);

                _Contact contact = new _Contact(name, countryNumber, number);
                contacts.Add(contact);
            }
            reader.Close();
            Connection.Close();
            return contacts;
        }
    }

    public bool AddContacts(_Contact contact)
    {
        if (Connection == null) return false;
        Connection.Open();
        string sql = "INSERT INTO Contacts (Name, AreaCode, Number) VALUES (@Name, @AreaCode, @Number)";
        
        using (SqliteCommand command = new SqliteCommand(sql, Connection))
        {
            command.Parameters.AddWithValue("@Name", contact.Name);
            command.Parameters.AddWithValue("@AreaCode", contact.AreaCode);
            command.Parameters.AddWithValue("@Number", contact.Number);
 
            int rowsAffected = command.ExecuteNonQuery();
            Connection.Close();
            return rowsAffected > 0;
        }
       
    }
}