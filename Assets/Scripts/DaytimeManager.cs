using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum DaytimePhase
{
    Night,
    Dawn,
    Day,
    Dusk
}

public class DaytimeManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private TimeOfDaySettings settings;

    [Header("Lights")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Light2D torchLight;

    [Header("Cycle State (debug)")]
    [SerializeField] private float cycleTime = 0f;       // tiempo dentro del ciclo actual
    [SerializeField] private float fullCycleDuration = 1f;
    [SerializeField] private DaytimePhase currentPhase = DaytimePhase.Night;

    [Header("Season State (debug)")]
    [SerializeField] private int currentSeasonIndex = 0;
    [SerializeField] private int currentDayInSeason = 1;

    public Season CurrentSeason
    {
        get
        {
            if (settings == null || settings.seasons == null || settings.seasons.Count == 0)
                return Season.Winter;

            return settings.seasons[currentSeasonIndex].season;
        }
    }

    public int CurrentDayInSeason => currentDayInSeason;
    public DaytimePhase CurrentPhase => currentPhase;

    public bool IsNight =>
        currentPhase == DaytimePhase.Night;

    public bool IsDaylight =>
        currentPhase == DaytimePhase.Dawn ||
        currentPhase == DaytimePhase.Day ||
        currentPhase == DaytimePhase.Dusk;

    private void Start()
    {
        if (settings == null)
        {
            Debug.LogError("DaytimeManager: falta asignar TimeOfDaySettings.");
            enabled = false;
            return;
        }

        fullCycleDuration = settings.dayDuration + settings.nightDuration;
        if (fullCycleDuration <= 0f)
            fullCycleDuration = 1f;

        // Seasons
        if (settings.seasons != null && settings.seasons.Count > 0)
        {
            currentSeasonIndex = Mathf.Clamp(currentSeasonIndex, 0, settings.seasons.Count - 1);
            if (currentDayInSeason <= 0)
                currentDayInSeason = 1;
        }
        else
        {
            currentSeasonIndex = 0;
            currentDayInSeason = 1;
        }

        cycleTime = 0f;
        currentPhase = GetPhase(0f);

        UpdateLights(0f);
    }

    private void Update()
    {
        if (settings == null)
            return;

        // Avanzar tiempo en el ciclo
        cycleTime += Time.deltaTime;
        if (cycleTime >= fullCycleDuration)
        {
            cycleTime -= fullCycleDuration;
        }

        float normalizedTime = cycleTime / fullCycleDuration; // 0..1

        // Calcular fase actual
        DaytimePhase previousPhase = currentPhase;
        currentPhase = GetPhase(normalizedTime);

        // Cuando pasamos de Night -> Dawn (nuevo amanecer), avanzamos un día de season
        if (previousPhase == DaytimePhase.Night &&
            (currentPhase == DaytimePhase.Dawn || currentPhase == DaytimePhase.Day))
        {
            AdvanceDay();
        }

        UpdateLights(normalizedTime);
    }

    private DaytimePhase GetPhase(float t)
    {
        t = Mathf.Repeat(t, 1f); // asegura 0..1

        // DAY: 0.00 → 0.50
        if (t >= 0f && t < settings.duskStart)
            return DaytimePhase.Day;

        // DUSK: 0.50 → 0.55
        if (t >= settings.duskStart && t < settings.nightStart)
            return DaytimePhase.Dusk;

        // NIGHT: 0.55 → 0.95
        if (t >= settings.nightStart && t < settings.dawnStart)
            return DaytimePhase.Night;

        // DAWN: 0.95 → 1.00
        return DaytimePhase.Dawn;
    }

    private void UpdateLights(float normalizedTime)
    {
        normalizedTime = Mathf.Clamp01(normalizedTime);

        // ---- GLOBAL LIGHT ----
        float globalCurveValue = Mathf.Clamp01(settings.globalLightCurve.Evaluate(normalizedTime));
        float globalIntensity = Mathf.Lerp(
            settings.globalMinIntensity,
            settings.globalMaxIntensity,
            globalCurveValue
        );

        if (globalLight != null)
            globalLight.intensity = globalIntensity;

        // ---- TORCH LIGHT (base) ----
        float torchCurveValue = Mathf.Clamp01(settings.torchLightCurve.Evaluate(normalizedTime));
        float torchBaseIntensity = Mathf.Lerp(
            settings.torchMinIntensity,
            settings.torchMaxIntensity,
            torchCurveValue
        );

        // Flicker con PerlinNoise
        if (torchLight != null)
        {
            float noise = Mathf.PerlinNoise(Time.time * settings.flickerSpeed, 0f); // 0..1
            float flickerFactor = 1f + (noise - 0.5f) * settings.flickerStrength * 2f;
            torchLight.intensity = torchBaseIntensity * flickerFactor;
        }
    }

    private void AdvanceDay()
    {
        if (settings.seasons == null || settings.seasons.Count == 0)
            return;

        currentDayInSeason++;

        var seasonData = settings.seasons[currentSeasonIndex];
        if (currentDayInSeason > seasonData.daysInSeason)
        {
            currentSeasonIndex = (currentSeasonIndex + 1) % settings.seasons.Count;
            currentDayInSeason = 1;
        }

        // Aquí podrías disparar eventos de cambio de estación si quieres
        // Debug.Log($"Season: {CurrentSeason}, Day: {currentDayInSeason}");
    }
}