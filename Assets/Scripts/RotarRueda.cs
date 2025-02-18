using UnityEngine;

public class RotarRueda : MonoBehaviour
{
    public float velocidadRotacion = 100f; // Velocidad en grados/segundo
    public Vector3 ejeRotacion = Vector3.up; // Eje de rotación (ajustar según necesidad)

    void Update()
    {
        // Rotar el objeto en su eje local
        transform.Rotate(ejeRotacion * velocidadRotacion * Time.deltaTime);
    }
}