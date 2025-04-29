using UnityEngine;
using System.Collections;

// Descripción:
// Script versátil para animar GameObjects en AR para el proyecto Metro Mágico CDMX.
// Permite configurar Rotación, Translación y Escalado vía Inspector.
// Diseñado para ser añadido a cada objeto 3D que requiera animación dentro de un marcador.
// Incluye logs de depuración para diagnosticar ejecución.
// Autor: [Tu Nombre o IA Asistente]
// Fecha: 14 de abril de 2024 (Actualizado para Derivación de Radio Orbital y Logs)

public class SimpleARAnimator : MonoBehaviour
{
    // --- SECCIÓN GENERAL ---
    [Header("General Settings")]
    [Tooltip("Delay inicial en segundos antes de que comience CUALQUIER animación para este objeto.")]
    public float initialDelay = 0f;
    private bool delayComplete = false;
    private float timeElapsed = 0f;

    // --- SECCIÓN DE ROTACIÓN ---
    [Header("Rotation Settings")]
    [Tooltip("Habilitar animación de rotación.")]
    public bool animateRotation = false;
    [Tooltip("Velocidad de rotación en grados por segundo para cada eje (X, Y, Z).")]
    public Vector3 rotationSpeed = Vector3.zero;
    [Tooltip("Espacio de coordenadas para la rotación (Self: local, World: global).")]
    public Space rotationSpace = Space.Self;

    // --- SECCIÓN DE TRANSLACIÓN ---
    [Header("Translation Settings")]
    [Tooltip("Habilitar animación de translación.")]
    public bool animateTranslation = false;

    public enum TranslationType { Constant, PingPong, Orbital, GentleBobbing }
    [Tooltip("Tipo de movimiento de translación.")]
    public TranslationType translationType = TranslationType.Constant;

    // Para Constant y PingPong
    [Tooltip("Dirección y magnitud del movimiento para Constant/PingPong (World Space).")]
    public Vector3 translationDirection = Vector3.up; // Dirección por defecto
    [Tooltip("Velocidad del movimiento en unidades por segundo.")]
    public float translationSpeed = 1f;

    // Solo para PingPong y GentleBobbing
    [Tooltip("Distancia máxima desde el punto inicial para PingPong / Amplitud para Bobbing.")]
    public float translationRange = 1f;

    // Solo para Orbital
    [Tooltip("Objeto alrededor del cual orbitar (Opcional: si es nulo, orbita alrededor de su punto inicial).")]
    public Transform orbitalCenter = null;
    [Tooltip("Radio de la órbita (<= 0 para usar distancia inicial).")]
    public float orbitalRadius = 0f; // Por defecto 0 para calcular
    [Tooltip("Eje alrededor del cual orbitar (Define el plano de la órbita). Típicamente Y (0,1,0) para órbita horizontal.")]
    public Vector3 orbitalAxis = Vector3.up;
    [Tooltip("Dirección inicial desde el centro para comenzar la órbita (relativo al centro). Si es Cero (0,0,0), se usa la posición inicial relativa.")]
    public Vector3 initialOrbitDirection = Vector3.zero; // Por defecto 0 para calcular


    // --- SECCIÓN DE ESCALADO ---
    [Header("Scaling Settings")]
    [Tooltip("Habilitar animación de escalado.")]
    public bool animateScaling = false;

    public enum ScalingType { Pulsing, GrowOnStart, Breathe }
    [Tooltip("Tipo de animación de escalado.")]
    public ScalingType scalingType = ScalingType.Pulsing;

    [Tooltip("Escala mínima relativa a la escala inicial (e.g., 0.8 para 80%). Para GrowOnStart, es la escala inicial del efecto.")]
    public float minScaleFactor = 0.8f;
    [Tooltip("Escala máxima relativa a la escala inicial (e.g., 1.2 para 120%). Para GrowOnStart, es la escala final.")]
    public float maxScaleFactor = 1.2f;
    [Tooltip("Velocidad de la animación de escalado.")]
    public float scaleSpeed = 1f;


    // --- VARIABLES PRIVADAS --- (¡Asegúrate de que esta sección existe!)
    // Translación
    private Vector3 startPosition;
    private Vector3 pingPongTarget;
    private int pingPongDir = 1;
    private float orbitalAngle = 0f;
    private Vector3 actualOrbitalCenter;
    private float actualOrbitalRadius;
    private Vector3 actualRelativeStartDirection;
    // Escalado
    private Vector3 initialScale;
    private bool hasGrown = false;

    //--------------------------------------------------------------------------
    // MÉTODO START: Inicialización Mejorada
    //--------------------------------------------------------------------------
    void Start()
    {
        startPosition = transform.localPosition;
        initialScale = transform.localScale;

        // Inicializar Translación
        if (animateTranslation)
        {
            if (translationType == TranslationType.PingPong)
            {
                pingPongTarget = startPosition + translationDirection.normalized * translationRange;
            }
            else if (translationType == TranslationType.Orbital)
            {
                // Determinar el centro REAL de la órbita
                actualOrbitalCenter = orbitalCenter ? orbitalCenter.localPosition : startPosition; // Orbita sobre sí mismo si no hay centro asignado

                // Determinar la DIRECCIÓN INICIAL de la órbita
                Vector3 initialOffsetVector = startPosition - actualOrbitalCenter;
                if (initialOrbitDirection == Vector3.zero) // Si NO se especificó dirección en Inspector...
                {
                    // ...calcularla desde la posición actual en escena.
                    actualRelativeStartDirection = initialOffsetVector.normalized;
                    if (actualRelativeStartDirection == Vector3.zero) { actualRelativeStartDirection = Vector3.forward; Debug.LogWarning($"Objeto {gameObject.name} inicia en el centro orbital, usando dirección Forward."); }
                }
                else { actualRelativeStartDirection = initialOrbitDirection.normalized; }

                // Determinar el RADIO de la órbita
                if (orbitalRadius <= 0) // Si NO se especificó radio POSITIVO en Inspector...
                {
                    // ...calcularlo desde la distancia actual en escena.
                    actualOrbitalRadius = initialOffsetVector.magnitude;
                    if (actualOrbitalRadius <= 0 && initialOrbitDirection == Vector3.zero) { actualOrbitalRadius = 1.0f; Debug.LogWarning($"Objeto {gameObject.name} inicia en el centro orbital sin radio, usando radio 1.0."); }
                    else if (actualOrbitalRadius <= 0 && initialOrbitDirection != Vector3.zero) { actualOrbitalRadius = 1.0f; Debug.LogWarning($"Objeto {gameObject.name} sin distancia al centro, usando radio 1.0."); }
                }
                else { actualOrbitalRadius = orbitalRadius; } // Usar el radio del Inspector.


                // --- Log de depuración Orbital ---
                Debug.Log($"ORBITAL START [{gameObject.name}]: Center={actualOrbitalCenter}, InitialOffset={initialOffsetVector}, Radius Used={actualOrbitalRadius}, StartDir Used={actualRelativeStartDirection}");
            }
        }

        // Inicializar Escalado
        if (animateScaling && scalingType == ScalingType.GrowOnStart) { transform.localScale = initialScale * minScaleFactor; }

        // Inicializar Delay
        if (initialDelay <= 0f) { delayComplete = true; } else { delayComplete = false; }
        timeElapsed = 0f;
        hasGrown = false;

        // <<<--- LOG DE INICIO ---<<<
        Debug.LogWarning("START SCRIPT on: " + gameObject.name);
    }

    //--------------------------------------------------------------------------
    // MÉTODO UPDATE: Usa los valores calculados en Start
    //--------------------------------------------------------------------------
    void Update()
    {
        // <<<--- LOG DE UPDATE ---<<< (Descomentar si es necesario)
        // Debug.Log("UPDATE TICK on: " + gameObject.name + ", Time: " + Time.time);

        // Manejo de Delay
        if (!delayComplete)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= initialDelay) { delayComplete = true; timeElapsed = 0f; }
            else { return; }
        }
        timeElapsed += Time.deltaTime; // Tiempo para animaciones post-delay

        // Ejecutar Animaciones
        if (animateRotation) { HandleRotation(); }
        if (animateTranslation) { HandleTranslation(); }
        if (animateScaling) { HandleScaling(); }
    }

    //--------------------------------------------------------------------------
    // FUNCIONES AUXILIARES DE ANIMACIÓN
    //--------------------------------------------------------------------------
    void HandleRotation()
    {
        // <<<--- LOG DE ROTACIÓN ---<<<
        Debug.LogWarning(">>> HandleRotation CALLED on: " + gameObject.name + ", trying to rotate with speed: " + rotationSpeed);
        transform.Rotate(rotationSpeed * Time.deltaTime, rotationSpace);
    }

    void HandleTranslation()
    {
        switch (translationType)
        {
             case TranslationType.Constant:
                transform.Translate(translationDirection.normalized * translationSpeed * Time.deltaTime, Space.World);
                break;
            case TranslationType.PingPong:
                Vector3 targetPos = (pingPongDir == 1) ? pingPongTarget : startPosition;
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, translationSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.localPosition, targetPos) < 0.01f) {
                    pingPongDir *= -1;
                    pingPongTarget = (pingPongDir == 1) ? startPosition + translationDirection.normalized * translationRange : startPosition;
                }
                break;
             case TranslationType.GentleBobbing:
                 float bobbingOffset = Mathf.Sin(timeElapsed * translationSpeed) * translationRange;
                 transform.localPosition = startPosition + translationDirection.normalized * bobbingOffset;
                 break;
            case TranslationType.Orbital:
                 orbitalAngle += translationSpeed * Time.deltaTime;
                 Vector3 offsetDirection = Quaternion.AngleAxis(orbitalAngle, orbitalAxis) * actualRelativeStartDirection;
                 Vector3 newPos = actualOrbitalCenter + offsetDirection * actualOrbitalRadius;
                 transform.localPosition = newPos;
                 break;
        }
    }

    void HandleScaling()
    {
        float scaleFactor = 1f;
        switch (scalingType)
        {
             case ScalingType.Pulsing:
                 float t_pulse = Mathf.PingPong(timeElapsed * scaleSpeed, 1f);
                 scaleFactor = Mathf.Lerp(minScaleFactor, maxScaleFactor, t_pulse);
                 transform.localScale = initialScale * scaleFactor;
                 break;
            case ScalingType.GrowOnStart:
                 if (!hasGrown) {
                     float t_grow = Mathf.Clamp01(timeElapsed * scaleSpeed);
                     scaleFactor = Mathf.Lerp(minScaleFactor, maxScaleFactor, t_grow);
                     transform.localScale = initialScale * scaleFactor;
                     if (t_grow >= 1f) { hasGrown = true; }
                 }
                 break;
            case ScalingType.Breathe:
                 float t_breathe = (Mathf.Sin(timeElapsed * scaleSpeed) + 1f) / 2f;
                 scaleFactor = Mathf.Lerp(minScaleFactor, maxScaleFactor, t_breathe);
                 transform.localScale = initialScale * scaleFactor;
                 break;
        }
    }
}