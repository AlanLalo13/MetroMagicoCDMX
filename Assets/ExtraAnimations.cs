using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random; // Especificar para evitar ambigüedad

// Descripción:
// Script ADICIONAL para manejar animaciones específicas NO CUBIERTAS
// por SimpleARAnimator: Rotación Oscilante y Translación Shake.
// Diseñado para añadirse JUNTO a SimpleARAnimator si es necesario.
// Autor: [Tu Nombre o IA Asistente]
// Fecha: 14 de abril de 2024 (v8 - Script Adicional Separado)

public class ExtraAnimations : MonoBehaviour
{
    // --- SECCIÓN DE ROTACIÓN OSCILANTE ---
    [Header("--- Oscillating Rotation (NEW) ---")]
    [Tooltip("HABILITAR Rotación OSCILANTE (cabeceo, viento) desde este script.")]
    public bool enableOscillation = false;
    [Tooltip("Eje local alrededor del cual oscilar (e.g., (1,0,0)).")]
    public Vector3 oscillationAxis = Vector3.right;
    [Tooltip("Rango angular total de la oscilación (grados).")]
    public float oscillationRange = 30f;
    [Tooltip("Velocidad de la oscilación (ciclos aprox. por segundo).")]
    public float oscillationSpeed = 1f;
    [Tooltip("Delay específico para la oscilación.")]
    public float oscillationDelay = 0f;
    private bool oscillationDelayComplete = false;
    private float oscillationTimeElapsed = 0f;
    private Quaternion startRotationOscillation; // Necesita su propia rotación inicial


    // --- SECCIÓN DE TRANSLACIÓN SHAKE ---
    [Header("--- Shake Translation (NEW) ---")]
    [Tooltip("HABILITAR translación tipo SHAKE (temblor) desde este script.")]
    public bool enableShake = false;
    [Tooltip("Intensidad del temblor (cuánto se mueve).")]
    public float shakeIntensity = 0.05f;
    [Tooltip("Delay específico para el shake.")]
    public float shakeDelay = 0f;
    private bool shakeDelayComplete = false;
    private float shakeTimeElapsed = 0f; // Sólo para el delay del shake
    private Vector3 startPositionShake; // Necesita su propia posición inicial


    // --- MÉTODO START ---
    void Start()
    {
        // Guardar estados iniciales específicos para este script
        startRotationOscillation = transform.localRotation; // Guardar rotación local para oscilación
        startPositionShake = transform.localPosition; // Guardar posición local para shake

        // Inicializar delays
        if (oscillationDelay <= 0f) { oscillationDelayComplete = true; }
        if (shakeDelay <= 0f) { shakeDelayComplete = true; }

        Debug.LogWarning($"START ExtraAnimations on: {gameObject.name}");
    }

    // --- MÉTODO UPDATE ---
    void Update()
    {
        // --- Manejo de Delay y Ejecución para Oscilación ---
        if (enableOscillation)
        {
            if (!oscillationDelayComplete)
            {
                oscillationTimeElapsed += Time.deltaTime;
                if (oscillationTimeElapsed >= oscillationDelay) { oscillationDelayComplete = true; oscillationTimeElapsed = 0f; } // Reiniciar tiempo post-delay
            }
            if (oscillationDelayComplete)
            {
                oscillationTimeElapsed += Time.deltaTime; // Incrementar tiempo para la fórmula Sin
                HandleOscillation();
            }
        }

        // --- Manejo de Delay y Ejecución para Shake ---
        if (enableShake)
        {
            if (!shakeDelayComplete)
            {
                shakeTimeElapsed += Time.deltaTime;
                if (shakeTimeElapsed >= shakeDelay) { shakeDelayComplete = true; }
            }
            if (shakeDelayComplete)
            {
                HandleShake(); // Ejecutar shake una vez pasado el delay
            }
        }
    }

    // --- HANDLERS para las animaciones de este script ---
    void HandleOscillation()
    {
        float angleOffset = Mathf.Sin(oscillationTimeElapsed * oscillationSpeed * Mathf.PI * 2) * (oscillationRange / 2f);
        Quaternion offsetRotation = Quaternion.AngleAxis(angleOffset, oscillationAxis.normalized);
        // Aplicar RELATIVO a la rotación que tenía el objeto AL INICIO de este script
        transform.localRotation = startRotationOscillation * offsetRotation;
    }

    void HandleShake()
    {
        Vector3 shakeOffset = Random.insideUnitSphere * shakeIntensity;
        // Aplicar RELATIVO a la posición que tenía el objeto AL INICIO de este script
        transform.localPosition = startPositionShake + shakeOffset;
    }
}