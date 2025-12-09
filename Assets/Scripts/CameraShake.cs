using System;
using UnityEngine;
using Unity.Cinemachine;

[Serializable]
public class CameraShakeData
{
    public CinemachineComponentBase _cinemachineComponent;
    public float _shakeTimer;
}

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    public CameraShakeData cameraShakeData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (cameraShakeData._shakeTimer > 0f)
        {
            cameraShakeData._shakeTimer -= Time.deltaTime;
            if (cameraShakeData._shakeTimer <= 0f)
            {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                    cameraShakeData._cinemachineComponent.GetComponent<CinemachineBasicMultiChannelPerlin>();
                cinemachineBasicMultiChannelPerlin.AmplitudeGain = 0f;
            }
        }
    }

    public void ShakeCamera_p1(float intensity, float time)
    {
        CinemachineComponentBase baseComponent = cameraShakeData._cinemachineComponent;
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
            baseComponent.GetComponent<CinemachineBasicMultiChannelPerlin>();
        
        cinemachineBasicMultiChannelPerlin.AmplitudeGain = intensity;
        cameraShakeData._shakeTimer = time;
    }
}
