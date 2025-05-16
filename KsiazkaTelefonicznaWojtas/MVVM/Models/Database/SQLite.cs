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
                        FirstName TEXT NULL,
                        LastName TEXT NULL,
                        AreaCode INTEGER NULL,
                        Number INTEGER
                    );";
            command.ExecuteNonQuery();
            Connection.Close();
        }
        return Instance;
    }

    public List<_Contact> GetContacts(string? search = null, OrderBy? orderBy = null, bool desc = false)
    {
        if (Connection == null) return new List<_Contact>();
        Connection.Open();
        string orderByColumn = orderBy switch
        {
            OrderBy.Id => "Id",
            OrderBy.FirstName => "FirstName",
            OrderBy.LastName => "LastName",
            OrderBy.AreaCode => "AreaCode",
            OrderBy.Number => "Number",
            _ => "Id"
        };
        
        string query = "SELECT * FROM Contacts";
        if (!string.IsNullOrWhiteSpace(search))
        {
            query += " WHERE (LOWER(FirstName) || ' ' || LOWER(LastName) || LOWER(Number)) LIKE @search ";
        }
        query += " ORDER BY " + orderByColumn + (!desc?"":" DESC");
        using (var command = new SqliteCommand(query, Connection))
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                command.Parameters.AddWithValue("@search", $"%{search.ToLower()}%");
            }
            SqliteDataReader reader = command.ExecuteReader();
            List<_Contact> contacts = new  List<_Contact>();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["Id"]);
                string? firstname = reader["FirstName"].ToString();
                string? lastname = reader["LastName"].ToString();
                short countryNumber = Convert.ToInt16(reader["AreaCode"]);
                int number = Convert.ToInt32(reader["Number"]);

                _Contact contact = new _Contact(firstname, lastname, countryNumber, number, id);
                contacts.Add(contact);
            }
            reader.Close();
            Connection.Close();
            return contacts;
        }
    }
    public _Contact? GetContact(int? _id)
    {
        if (Connection == null) return null;
        Connection.Open();

        
        string query = "SELECT * FROM Contacts WHERE Id = @id";

        using (var command = new SqliteCommand(query, Connection))
        {
            command.Parameters.AddWithValue("@id", _id);
            SqliteDataReader reader = command.ExecuteReader();
            
            if (reader.Read())
            {
                int id = Convert.ToInt32(reader["Id"]);
                string? firstname = reader["FirstName"].ToString();
                string? lastname = reader["LastName"].ToString();
                short countryNumber = Convert.ToInt16(reader["AreaCode"]);
                int number = Convert.ToInt32(reader["Number"]);
                reader.Close();
                Connection.Close();
                return new _Contact(firstname, lastname, countryNumber, number, id);
            }
            else
            {
                reader.Close();
                Connection.Close();
                return null;
            }
        }
    }

    public bool AddContacts(_Contact contact)
    {
        if (Connection == null) return false;
        Connection.Open();
        string sql = "INSERT INTO Contacts (FirstName, LastName, AreaCode, Number) VALUES (@FirstName, @LastName, @AreaCode, @Number)";
        
        using (SqliteCommand command = new SqliteCommand(sql, Connection))
        {
            command.Parameters.AddWithValue("@FirstName", contact.FirstName);
            command.Parameters.AddWithValue("@LastName", contact.LastName);
            command.Parameters.AddWithValue("@AreaCode", contact.AreaCode);
            command.Parameters.AddWithValue("@Number", contact.Number);
 
            int rowsAffected = command.ExecuteNonQuery();
            Connection.Close();
            return rowsAffected > 0;
        }
       
    }

    public bool DeleteContact(_Contact contact)
    {
        if (Connection == null) return false;
        Connection.Open();
        string sql = "DELETE FROM Contacts WHERE Id = @Id";
        SqliteCommand command = new SqliteCommand(sql, Connection);
        command.Parameters.AddWithValue("@Id", contact.Id);
        int rowsAffected = command.ExecuteNonQuery();
        Connection.Close();
        return rowsAffected > 0;
    }

    public bool UpdateContact(_Contact updatedContact)
    {
        if (Connection == null) return false;
        _Contact? existingContact = GetContact(updatedContact.Id);
        Connection.Open();
        if (existingContact == null)
        {
            Connection.Close();
            return false;
        }
        List<string> updates = new();
        var command = Connection.CreateCommand();

        
        if (existingContact.FirstName != updatedContact.FirstName)
        {
            updates.Add("FirstName = @FirstName");
            command.Parameters.AddWithValue("@FirstName", updatedContact.FirstName);
        }
        if (existingContact.LastName != updatedContact.LastName)
        {
            updates.Add("LastName = @LastName");
            command.Parameters.AddWithValue("@LastName", updatedContact.LastName);
        }

        if (existingContact.AreaCode != updatedContact.AreaCode)
        {
            updates.Add("AreaCode = @AreaCode");
            command.Parameters.AddWithValue("@AreaCode", updatedContact.AreaCode);
        }

        if (existingContact.Number != updatedContact.Number)
        {
            updates.Add("Number = @Number");
            command.Parameters.AddWithValue("@Number", updatedContact.Number);
        }

        if (updates.Count == 0)
        {
            Connection.Close();
            return true;
        }
        command.CommandText = $"UPDATE Contacts SET {string.Join(", ", updates)} WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", updatedContact.Id);

        int rowsAffected = command.ExecuteNonQuery();
        Connection.Close();

        return rowsAffected > 0;
    }
}