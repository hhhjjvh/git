using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackSense : MonoBehaviour
{
    [Header("HitPause Settings")]
    [SerializeField] private float _pauseShakeStrength = 0.3f;  // 暂停时的相机震动强度
    [SerializeField] private bool _allowPauseStacking = false; // 是否允许暂停叠加

    [Header("Camera Shake Settings")]
    [SerializeField] private AnimationCurve _shakeAttenuation = AnimationCurve.Linear(0, 1, 1, 0); // 震动衰减曲线
    [SerializeField] private float _shakeFrequency = 0.05f;    // 震动更新频率

    private Coroutine _currentPauseRoutine;
    private Vector3 _cameraOriginalPos;
    private bool _isPausing;

    public static AttackSense instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        _cameraOriginalPos = Camera.main.transform.position;
    }

    /// <summary>
    /// 增强版受击暂停效果
    /// </summary>
    public void HitPause(int durationFrames, bool applyShake = true)
    {
        if (_isPausing && !_allowPauseStacking) return;

        if (_currentPauseRoutine != null)
            StopCoroutine(_currentPauseRoutine);

        _currentPauseRoutine = StartCoroutine(PauseRoutine(
            durationFrames / 60f,
            applyShake ? _pauseShakeStrength*(1+durationFrames/5f): 0f
        ));
    }

    private IEnumerator PauseRoutine(float pauseDuration, float shakeIntensity)
    {
        // 保存原始时间状态
        var timeManager = TimeManager.Instance;
        float originalScale = timeManager.WorldTimeScale;
        bool wasFrozen = timeManager.IsEntityFrozen(TimeManager.EntityType.Global);

        // 初始化暂停状态
        _isPausing = true;
        timeManager.FreezeEntityType(TimeManager.EntityType.Global, true);
        timeManager.SetGlobalTimeScale(0, true);

        // 初始相机震动
        if (shakeIntensity > 0)
            CameraShake(pauseDuration, shakeIntensity);

        // 等待指定时间
        yield return new WaitForSecondsRealtime(pauseDuration);
        // 使用真实时间等待
        //float elapsed = 0;
        //while (elapsed < pauseDuration*5)
        //{
        //    elapsed += Time.unscaledDeltaTime;

        //    // 添加逐帧效果（如：屏幕闪白）
        //    if (Time.frameCount % 3 == 0)
        //      //  PostProcessingController.Instance.FlashScreen(0.1f);

        //    yield return null;
        //}

        // 恢复时间状态
        timeManager.FreezeEntityType(TimeManager.EntityType.Global, wasFrozen);
        timeManager.SetGlobalTimeScale(originalScale, true);
        _isPausing = false;
    }

    /// <summary>
    /// 增强版相机震动
    /// </summary>
    public void CameraShake(float duration, float maxStrength)
    {
        StartCoroutine(ShakeRoutine(duration, maxStrength));
    }

    private IEnumerator ShakeRoutine(float duration, float maxStrength)
    {
        Transform camTransform = Camera.main.transform;
        float elapsed = 0;

        while (elapsed < duration)
        {
            float currentStrength = maxStrength * _shakeAttenuation.Evaluate(elapsed / duration);

            // 使用Perlin噪声生成平滑震动
            float noiseX = Mathf.PerlinNoise(Time.unscaledTime * 10, 0) * 2 - 1;
            float noiseY = Mathf.PerlinNoise(0, Time.unscaledTime * 10) * 2 - 1;

            camTransform.position = _cameraOriginalPos + new Vector3(
                noiseX * currentStrength,
                noiseY * currentStrength,
                0
            );

            elapsed += Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(_shakeFrequency);
        }

        camTransform.position = _cameraOriginalPos;
    }

    private void OnDisable()
    {
        // 确保退出时恢复状态
        if (_isPausing)
        {
            TimeManager.Instance?.SetGlobalTimeScale(1, true);
            Camera.main.transform.position = _cameraOriginalPos;
        }
    }
}
