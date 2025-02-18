using UnityEngine;

public class Biela2 : MonoBehaviour
{
    [Header("Configuración")]
    public Transform rueda;
    [Min(0.1f)] public float radio = 12.0f;
    [Min(0.2f)] public float longitudBiela = 100.0f;

    [Header("Debug (opcional)")]
    [SerializeField] private Transform puntoA; // Punto en la manivela
    [SerializeField] private Transform puntoB; // Punto del pistón (movimiento horizontal)

    [Header("Ajuste de profundidad")]
    [Tooltip("Desplazamiento extra en Z para empujar la biela hacia delante o atrás.")]
    public float zOffset = 0f;

    private void Update()
    {
        // 1. Extremo A (en la rueda, con giro invertido o no, según tu necesidad)
        Vector3 posA = GetCrankPosition();

        // 2. Extremo B (limitado a movimiento horizontal)
        Vector3 posB = GetPistonPosition(posA);

        // (Opcional) Visualizar en la escena
        if (puntoA != null) puntoA.position = posA;
        if (puntoB != null) puntoB.position = posB;

        // 3. Ajustar la biela
        UpdateBiela(posA, posB);
    }

    Vector3 GetCrankPosition()
    {
        float anguloRad = -rueda.eulerAngles.z * Mathf.Deg2Rad;  // negativo para sentido antihorario
        return rueda.position + new Vector3(
            radio * Mathf.Cos(anguloRad),
            radio * Mathf.Sin(anguloRad),
            0
        );
    }

    Vector3 GetPistonPosition(Vector3 posA)
    {
        float yPiston = rueda.position.y;
        float deltaY = posA.y - yPiston;
        float insideSqrt = Mathf.Pow(longitudBiela, 2) - Mathf.Pow(deltaY, 2);
        insideSqrt = Mathf.Max(insideSqrt, 0);
        
        float offsetX = posA.x - rueda.position.x;
        // Aquí eliges + o - según la orientación que necesites
        float xB = rueda.position.x + offsetX - Mathf.Sqrt(insideSqrt);

        return new Vector3(xB, yPiston, posA.z);
    }

    void UpdateBiela(Vector3 posA, Vector3 posB)
    {
        // Punto medio en X e Y
        float midX = (posA.x + posB.x) * 0.5f;
        float midY = (posA.y + posB.y) * 0.5f;
        // Para Z, tomas el promedio y le sumas zOffset
        float midZ = ((posA.z + posB.z) * 0.5f) + zOffset;

        // Posicionar la biela
        transform.position = new Vector3(midX, midY, midZ);

        // Rotarla mirando de A a B (en el plano XY)
        Vector3 direccion = posB - posA;
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }
}
