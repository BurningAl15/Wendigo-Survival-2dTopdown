using System;
using System.Collections.Generic;
using UnityEngine;

public enum Season
{
    Winter,
    Spring,
    Summer,
    Fall
}

[Serializable]
public class SeasonData
{
    public Season season;
    public int daysInSeason = 30;
}

[CreateAssetMenu(
    menuName = "TimeOfDay/Time Of Day Settings",
    fileName = "TimeOfDaySettings"
)]
public class TimeOfDaySettings : ScriptableObject
{
    [Header("Durations (seconds)")]
    public float dayDuration = 100f;
    public float nightDuration = 50f;

    [Header("Global Light (0..1 curve → intensity)")]
    public AnimationCurve globalLightCurve =
        AnimationCurve.Linear(0f, 0f, 1f, 1f);

    public float globalMinIntensity = 0.02f;
    public float globalMaxIntensity = 1.0f;

    [Header("Torch Light (0..1 curve → intensity)")]
    public AnimationCurve torchLightCurve =
        AnimationCurve.Linear(0f, 1f, 1f, 0f);

    public float torchMinIntensity = 0.1f;
    public float torchMaxIntensity = 1.2f;

    [Header("Torch Flicker")]
    public float flickerSpeed = 10f;
    public float flickerStrength = 0.2f;

    [Header("Day Phases Thresholds (normalized 0..1 in the cycle)")]
    [Tooltip("Night -> Dawn")]
    [Range(0f, 1f)] public float dawnStart = 0.10f;

    [Tooltip("Dawn -> Day")]
    [Range(0f, 1f)] public float dayStart = 0.25f;

    [Tooltip("Day -> Dusk")]
    [Range(0f, 1f)] public float duskStart = 0.65f;

    [Tooltip("Dusk -> Night")]
    [Range(0f, 1f)] public float nightStart = 0.80f;

    [Header("Seasons")]
    public List<SeasonData> seasons = new List<SeasonData>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Asegurar orden: dawn <= day <= dusk <= night
        dawnStart  = Mathf.Clamp01(dawnStart);
        dayStart   = Mathf.Clamp(dayStart,  dawnStart, 1f);
        duskStart  = Mathf.Clamp(duskStart, dayStart, 1f);
        nightStart = Mathf.Clamp(nightStart, duskStart, 1f);
    }
#endif
}