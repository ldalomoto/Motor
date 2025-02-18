using UnityEngine;

public class LedController : MonoBehaviour
{
    [Header("Umbrales para encender el LED")]
    [Tooltip("Voltaje mínimo para que el LED se encienda")]
    public float thresholdVoltage = 5.0f;
    [Tooltip("Corriente mínima para que el LED se encienda")]
    public float thresholdCurrent = 1.0f;

    [Header("Valores actuales (actualizados externamente)")]
    [Tooltip("Voltaje medido (por ejemplo, desde la base de datos)")]
    public float voltaje;
    [Tooltip("Corriente medida (por ejemplo, desde la base de datos)")]
    public float corriente;

    [Header("Componentes para visualización")]
    [Tooltip("Componente Light para simular el LED (opcional)")]
    public Light ledLight;
    [Tooltip("Material del LED (asegúrate de usar un shader con soporte de emisión)")]
    public Material ledMaterial;

    [Header("Colores de emisión")]
    public Color ledOnColor = Color.green;
    public Color ledOffColor = Color.black;

    [Header("Control de brillo")]
    [Tooltip("Brillo mínimo (cuando apenas supera el umbral)")]
    public float minBrightness = 0f;
    [Tooltip("Brillo máximo (cuando se alcanza o supera el valor para máximo brillo)")]
    public float maxBrightness = 5f;
    [Tooltip("Voltaje a partir del cual se alcanza el brillo máximo")]
    public float voltageForMaxBrightness = 20f;
    [Tooltip("Corriente a partir de la cual se alcanza el brillo máximo")]
    public float currentForMaxBrightness = 20f;

    // Estado interno del LED (encendido o apagado)
    private bool isOn = false;

    void Update()
    {
        // El LED se enciende si ambos valores superan sus umbrales
        bool shouldBeOn = (voltaje >= thresholdVoltage) && (corriente >= thresholdCurrent);

        if (shouldBeOn)
        {
            // Normalizar el voltaje y la corriente en función de los valores máximos deseados
            float normalizedVoltage = Mathf.InverseLerp(thresholdVoltage, voltageForMaxBrightness, voltaje);
            float normalizedCurrent = Mathf.InverseLerp(thresholdCurrent, currentForMaxBrightness, corriente);

            // Promediar ambos para obtener un factor de brillo (entre 0 y 1)
            float factor = (normalizedVoltage + normalizedCurrent) / 2f;

            // Interpolar entre el brillo mínimo y máximo
            float brightness = Mathf.Lerp(minBrightness, maxBrightness, factor);

            UpdateLedState(true, brightness);
        }
        else
        {
            UpdateLedState(false, 0);
        }
    }

    void UpdateLedState(bool on, float brightness)
    {
        isOn = on;

        // Opción 1: Actualizar el componente Light (si se asignó)
        if (ledLight != null)
        {
            ledLight.enabled = on;
            ledLight.intensity = brightness;
        }

        // Opción 2: Actualizar la emisión del material (asegúrate de usar un shader que soporte emisión)
        if (ledMaterial != null)
        {
            if (on)
            {
                ledMaterial.SetColor("_EmissionColor", ledOnColor * brightness);
                DynamicGI.SetEmissive(GetComponent<Renderer>(), ledOnColor * brightness);
            }
            else
            {
                ledMaterial.SetColor("_EmissionColor", ledOffColor);
                DynamicGI.SetEmissive(GetComponent<Renderer>(), ledOffColor);
            }
        }
    }
}