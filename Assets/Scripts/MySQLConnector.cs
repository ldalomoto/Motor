using UnityEngine;
using MySql.Data.MySqlClient;
using System;

public class MySQLConnector : MonoBehaviour
{
    [Header("MySQL Config")]
    [SerializeField] private string server = "localhost";
    [SerializeField] private string database = "sensores_2";
    [SerializeField] private string user = "root";
    [SerializeField] private string password = "Grupo_02";
    [SerializeField] private string port = "3306";

    [Header("Consulta")]
    [SerializeField] private float updateInterval = 1f; // Tiempo en segundos entre cada consulta

    private MySqlConnection connection;
    private float timer;

    [Header("Referencia a RotarRueda")]
    [Tooltip("Arrastra aquí el GameObject que tiene el script RotarRueda.")]
    public RotarRueda rotarRuedaScript;
    public Dinamo dinamoScript;

    void Awake()
    {
        // Construir cadena de conexión
        string connectionString = $"Server={server};" +
                                  $"Database={database};" +
                                  $"User ID={user};" +
                                  $"Password={password};" +
                                  $"Port={port};" +
                                  "SslMode=None;" +
                                  "AllowPublicKeyRetrieval=True;";

        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            Debug.Log("Conectado a MySQL correctamente.");
        }
        catch (Exception e)
        {
            Debug.LogError("Error al conectar con MySQL: " + e.Message);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            GetLatestRecord();
        }
    }

    private void GetLatestRecord()
    {
        if (connection == null || connection.State != System.Data.ConnectionState.Open)
        {
            Debug.LogError("La conexión con MySQL no está abierta. No se puede ejecutar la consulta.");
            return;
        }

        // Consulta para obtener el último registro
        string query = "SELECT * FROM medidas ORDER BY id DESC LIMIT 1";

        try
        {
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Construir una cadena con los datos para depuración
                        string datos = "";
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            datos += $"{reader.GetName(i)}: {reader[i]} | ";
                        }
                        Debug.Log("Último registro: " + datos);

                        // Extraer el valor de RPM (asumiendo que el campo se llama "RPM")
                        float rpmValue = Convert.ToSingle(reader["RPM"]);
                        Debug.Log("RPM value from DB: " + rpmValue);

                        // Convertir RPM a grados/segundo: (RPM * 360°/60s) = RPM * 6
                        float degreesPerSecond = rpmValue * 6f;

                        // Asignar la velocidad al script RotarRueda
                        if (rotarRuedaScript != null)
                        {
                            rotarRuedaScript.velocidadRotacion = degreesPerSecond;
                            dinamoScript.velocidadRotacion = degreesPerSecond;
                        }
                        else
                        {
                            Debug.LogWarning("No se asignó la referencia de RotarRueda en MySQLConnector.");
                        }
                    }
                    else
                    {
                        Debug.Log("No hay registros en la tabla 'medidas'.");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error al ejecutar la consulta: " + e.Message);
        }
    }

    void OnDestroy()
    {
        // Cerrar la conexión al destruir el objeto
        if (connection != null)
        {
            connection.Close();
            Debug.Log("Conexión con MySQL cerrada.");
        }
    }
}
