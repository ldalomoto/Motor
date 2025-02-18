using UnityEngine;
using MySql.Data.MySqlClient;
using System;

public class MySQLConnector : MonoBehaviour
{
    private string server = "localhost";   // Ejemplo: "192.168.1.100" o "localhost"
    private string database = "infra";
    private string user = "root";
    private string password = "root";
    private string port = "3306"; // Puerto por defecto de MySQL

    void Start()
    {
        GetLatestRecord();
    }

    void GetLatestRecord()
    {
        string connectionString = "Server=localhost;Database=infra;User Id=root;Password=root;SslMode=None;AllowPublicKeyRetrieval=True;";

       // string connectionString = $"Server={server};Database={database};User ID={user};Password={password};Port={port};SslMode=None;";
        
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                Debug.Log("Conectado a MySQL");

                string query = "SELECT * FROM mediciones ORDER BY id DESC LIMIT 1"; // Ajusta según tu estructura de datos

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string datos = "";
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                datos += $"{reader.GetName(i)}: {reader[i]} | ";
                            }
                            Debug.Log("Último registro: " + datos);
                        }
                        else
                        {
                            Debug.Log("No hay registros en la tabla.");
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error al conectar con MySQL: " + e.Message);
        }
    }
}
