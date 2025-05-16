using UnityEngine;
using System.Collections;

// Script para hacer que un objeto se traslade ligeramente hacia adelante y atrás
// mientras escala un poco más grande y vuelve a su tamaño original, en un ciclo.
public class PulsatingObject : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("Habilitar esta animación.")]
    public bool enablePulsation = true;

    [Tooltip("Eje LOCAL a lo largo del cual el objeto se trasladará (e.g., (0,0,1) para Z local).")]
    public Vector3 translationAxis = Vector3.forward; // Vector3.forward es (0,0,1)

    [Tooltip("Distancia máxima que se trasladará hacia adelante desde su posición inicial.")]
    public float translationDistance = 0.1f;

    [Tooltip("Factor de escala máximo relativo a su escala inicial (e.g., 1.1 para 10% más grande).")]
    public float maxScaleFactor = 1.1f;

    [Tooltip("Velocidad del ciclo de pulsación (un ciclo completo de ida/vuelta y crecer/encoger por segundo a velocidad 1).")]
    public float pulsationSpeed = 1.0f;

    [Tooltip("Delay inicial en segundos antes de que comience la pulsación.")]
    public float initialDelay = 0f;

    // --- Variables Privadas ---
    private Vector3 initialLocalPosition;
    private Vector3 initialLocalScale;
    private float timeElapsed = 0f;
    private bool delayComplete = false;

    void Start()
    {
        // Guardar la posición y escala local iniciales
        initialLocalPosition = transform.localPosition;
        initialLocalScale = transform.localScale;

        // Manejar el delay inicial
        if (initialDelay <= 0f)
        {
            delayComplete = true;
        }
        else
        {
            // Se podría usar una Coroutine para el delay, pero para un solo efecto,
            // manejarlo en Update es simple.
            timeElapsed = 0f; // Reiniciar para el delay
        }
    }

    void Update()
    {
        if (!enablePulsation)
        {
            // Si se deshabilita, opcionalmente resetear a estado inicial
            // transform.localPosition = initialLocalPosition;
            // transform.localScale = initialLocalScale;
            return;
        }

        // --- Manejo de Delay ---
        if (!delayComplete)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= initialDelay)
            {
                delayComplete = true;
                timeElapsed = 0f; // Resetear tiempo para la animación
            }
            else
            {
                return; // Salir si el delay no ha terminado
            }
        }

        // --- Lógica de Pulsación (una vez que el delay ha pasado) ---
        timeElapsed += Time.deltaTime;

        // Usar una función Seno para un movimiento y escalado suave
        // Mathf.Sin va de -1 a 1. Lo mapearemos a 0 a 1 para Lerp y para el offset.
        // (Mathf.Sin(time * speed) + 1) / 2  ->  mapea -1 a 1  a  0 a 1
        float cycleValue = (Mathf.Sin(timeElapsed * pulsationSpeed * Mathf.PI * 2) + 1f) / 2f; // Mathf.PI*2 para que speed sea ciclos/seg

        // 1. Aplicar Traslación
        // El offset va de 0 (posición inicial) a translationDistance (posición máxima hacia adelante)
        Vector3 translationOffset = translationAxis.normalized * Mathf.Lerp(0, translationDistance, cycleValue);
        transform.localPosition = initialLocalPosition + translationOffset;

        // 2. Aplicar Escalado
        // El factor de escala va de 1 (escala inicial) a maxScaleFactor
        float currentScaleFactor = Mathf.Lerp(1f, maxScaleFactor, cycleValue);
        transform.localScale = initialLocalScale * currentScaleFactor;
    }

    // Opcional: Resetear si el componente se deshabilita en runtime
    void OnDisable()
    {
        if(initialLocalPosition != null && initialLocalScale != null) // Asegurar que Start se ejecutó
        {
            transform.localPosition = initialLocalPosition;
            transform.localScale = initialLocalScale;
            timeElapsed = 0f; // Resetear tiempo para la próxima vez que se active
            // delayComplete podría necesitar resetearse si es importante el delay cada vez
            // if (initialDelay > 0f) delayComplete = false;
        }
    }
}