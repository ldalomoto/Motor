using UnityEngine;

public class BielaController : MonoBehaviour
{
    [Header("Configuración Biela")]
    public Transform rueda;
    [Min(0.1f)] public float radio = 12.0f;
    [Min(0.2f)] public float longitudBiela = 100.0f;

    [Header("Desfase")]
    [Tooltip("Ángulo en grados para desfasar la segunda biela respecto a la primera.")]
    public float phaseOffset = 180f;

    [Header("Objetos para Debug (opcional)")]
    public Transform puntoA; // Extremo en la manivela
    public Transform puntoB; // Extremo deslizante

    [Header("Pistón")]
    public Transform piston;

    [Header("Ajustes visuales")]
    public float zOffsetBiela = 0f;
    public float xOffsetPiston = -7f; // Si necesitas ese ajuste como en la otra biela

    private void Update()
    {
        // 1. Calcular posición de A
        Vector3 posA = GetCrankPosition();

        // 2. Calcular posición de B
        Vector3 posB = GetPistonPosition(posA);

        // (Opcional) Debug
        if (puntoA != null) puntoA.position = posA;
        if (puntoB != null) puntoB.position = posB;

        // 3. Actualizar la biela
        UpdateBiela(posA, posB);

        // 4. Mover el pistón
        UpdatePiston(posB);
    }

    Vector3 GetCrankPosition()
    {
        // Obtenemos la rotación base de la rueda en Z
        // y le sumamos el desfase en grados (phaseOffset).
        // Usamos "-" si queremos sentido antihorario.
        float anguloRad = (rueda.eulerAngles.z + phaseOffset) * Mathf.Deg2Rad;

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
        // Ajusta el signo (+ / -) según la orientación que necesites
        float xB = rueda.position.x + offsetX - Mathf.Sqrt(insideSqrt);

        // Si necesitas restar 7, lo agregas:
        xB -= xOffsetPiston;

        return new Vector3(xB, yPiston, posA.z);
    }

    void UpdateBiela(Vector3 posA, Vector3 posB)
    {
        // Centro de la biela = punto medio
        float midX = (posA.x + posB.x) * 0.5f;
        float midY = (posA.y + posB.y) * 0.5f;
        float midZ = ((posA.z + posB.z) * 0.5f) + zOffsetBiela;

        transform.position = new Vector3(midX, midY, midZ);

        Vector3 direccion = posB - posA;
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }

    void UpdatePiston(Vector3 posB)
    {
        if (piston == null) return;

        // Ejemplo: mover el pistón en X e Y, conservando su Z original
        Vector3 newPistonPos = new Vector3(posB.x - 7.0f, posB.y, piston.position.z);
        piston.position = newPistonPos;

        // Rotación del pistón si fuera necesaria (normalmente no gira).
    }
}




/*using UnityEngine;

public class BielaController : MonoBehaviour
{
    [Header("Configuración Biela")]
    public Transform rueda;
    [Min(0.1f)] public float radio = 12.0f;
    [Min(0.2f)] public float longitudBiela = 100.0f;

    [Header("Objetos para Debug (opcional)")]
    [SerializeField] private Transform puntoA; // Extremo en la manivela
    [SerializeField] private Transform puntoB; // Extremo deslizante de la biela

    [Header("Pistón")]
    [Tooltip("Objeto del pistón que se moverá en función de B.")]
    public Transform piston;

    [Header("Ajustes visuales")]
    [Tooltip("Desplazamiento extra en Z para empujar la biela hacia delante o atrás.")]
    public float zOffsetBiela = 0f;

    private void Update()
    {
        // 1. Calcular posición del extremo A (rueda)
        Vector3 posA = GetCrankPosition();

        // 2. Calcular posición del extremo B (limitado a movimiento horizontal)
        Vector3 posB = GetPistonPosition(posA);

        // (Opcional) Para ver en escena
        if (puntoA != null) puntoA.position = posA;
        if (puntoB != null) puntoB.position = posB;

        // 3. Actualizar la biela
        UpdateBiela(posA, posB);

        // 4. Mover el pistón según posB
        UpdatePiston(posB);
    }

    // --------------------------
    //     MÉTODOS PRINCIPALES
    // --------------------------

    Vector3 GetCrankPosition()
    {
        // Cambia el signo si quieres invertir el sentido de giro
        float anguloRad = -rueda.eulerAngles.z * Mathf.Deg2Rad;
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

        // Elige + o - para reflejar a un lado u otro
        float xB = rueda.position.x + offsetX - Mathf.Sqrt(insideSqrt);

        return new Vector3(xB, yPiston, posA.z);
    }

    void UpdateBiela(Vector3 posA, Vector3 posB)
    {
        // Centro de la biela = punto medio entre A y B
        float midX = (posA.x + posB.x) * 0.5f;
        float midY = (posA.y + posB.y) * 0.5f;
        float midZ = ((posA.z + posB.z) * 0.5f) + zOffsetBiela;

        transform.position = new Vector3(midX, midY, midZ);

        // Orientación de la biela
        Vector3 direccion = posB - posA;
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }

    void UpdatePiston(Vector3 posB)
    {
        if (piston == null) return;

        // Si quieres que el pistón coincida EXACTAMENTE con B (en X e Y), pero mantenga su Z:
        // (útil cuando el cilindro está a la misma altura y no giramos el pistón)
        Vector3 newPistonPos = new Vector3(posB.x - 7.0f, posB.y, piston.position.z);

        // O, si prefieres que el pistón herede también la Z de B, usa:
        // Vector3 newPistonPos = posB;

        piston.position = newPistonPos;

        // Si necesitas rotar el pistón, puedes añadirlo aquí.
        // Normalmente un pistón solo se traslada, pero si requieres un
        // giro mínimo o algún ajuste, lo puedes hacer con:
        // piston.rotation = ...
    }
}

*/
