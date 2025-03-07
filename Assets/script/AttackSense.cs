using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // ����InputSystem�����ռ�

public class AttackSense : MonoBehaviour
{
    [Header("HitPause Settings")]
    [SerializeField] private float _pauseShakeStrength = 0.3f;  // ��ͣʱ�������ǿ��
    [SerializeField] private bool _allowPauseStacking = false; // �Ƿ�������ͣ����

    [Header("Camera Shake Settings")]
    [SerializeField] private AnimationCurve _shakeAttenuation = AnimationCurve.Linear(0, 1, 1, 0); // ��˥������
    [SerializeField] private float _shakeFrequency = 0.05f;    // �𶯸���Ƶ��

    [Header("Controller Vibration Settings")]
    [SerializeField] private AnimationCurve _vibrationAttenuation = AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField] private float _maxVibrationIntensity = 0.7f;
    [SerializeField] private float _vibrationDuration = 0.2f;

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
    /// ��ǿ���ܻ���ͣЧ��
    /// </summary>
    public void HitPause(int durationFrames, bool applyShake = true, bool applyVibration = true)
    {
        if (_isPausing && !_allowPauseStacking) return;

        if (_currentPauseRoutine != null)
            StopCoroutine(_currentPauseRoutine);

        _currentPauseRoutine = StartCoroutine(PauseRoutine(
            durationFrames / 60f,
            applyShake ? _pauseShakeStrength*(1+durationFrames/5f): 0f,
            applyVibration 
        ));
    }

    private IEnumerator PauseRoutine(float pauseDuration, float shakeIntensity, bool applyVibration)
    {
        // ����ԭʼʱ��״̬
        var timeManager = TimeManager.Instance;
        float originalScale = timeManager.WorldTimeScale;
        bool wasFrozen = timeManager.IsEntityFrozen(TimeManager.EntityType.Global);

        // ��ʼ����ͣ״̬
        _isPausing = true;
        timeManager.FreezeEntityType(TimeManager.EntityType.Global, true);
        timeManager.SetGlobalTimeScale(0, true);

        // ��ʼ�����
        if (shakeIntensity > 0)
            CameraShake(pauseDuration, shakeIntensity);
        // �����ֱ���
        if (applyVibration && Gamepad.current != null)
        {
            StartCoroutine(VibrationRoutine(
                pauseDuration,
                _maxVibrationIntensity,
                _vibrationDuration
            ));
        }

        // �ȴ�ָ��ʱ��
        yield return new WaitForSecondsRealtime(pauseDuration);
        // ʹ����ʵʱ��ȴ�
        //float elapsed = 0;
        //while (elapsed < pauseDuration*5)
        //{
        //    elapsed += Time.unscaledDeltaTime;

        //    // �����֡Ч�����磺��Ļ���ף�
        //    if (Time.frameCount % 3 == 0)
        //      //  PostProcessingController.Instance.FlashScreen(0.1f);

        //    yield return null;
        //}

        // �ָ�ʱ��״̬
        timeManager.FreezeEntityType(TimeManager.EntityType.Global, wasFrozen);
        timeManager.SetGlobalTimeScale(originalScale, true);
        _isPausing = false;
    }
    private IEnumerator VibrationRoutine(float totalDuration, float maxIntensity, float vibrationDuration)
    {
        Gamepad gamepad = Gamepad.current;
        if (gamepad == null) yield break;

        float elapsed = 0f;

        while (elapsed < totalDuration)
        {
            float vibrationStrength = maxIntensity *
                _vibrationAttenuation.Evaluate(elapsed / totalDuration);

            gamepad.SetMotorSpeeds(
                vibrationStrength,
                vibrationStrength * 0.8f // ��������������Ӳ�θ�
            );

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // ��ֹͣ��
        float fadeTime = 0.1f;
        while (fadeTime > 0)
        {
            gamepad.SetMotorSpeeds(
                Mathf.Lerp(0, maxIntensity, fadeTime / 0.1f),
                Mathf.Lerp(0, maxIntensity * 0.8f, fadeTime / 0.1f)
            );
            fadeTime -= Time.unscaledDeltaTime;
            yield return null;
        }

        gamepad.ResetHaptics();
    }

    /// <summary>
    /// ��ǿ�������
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

            // ʹ��Perlin��������ƽ����
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
        // ȷ������ʱֹͣ��
        if (Gamepad.current != null)
        {
            Gamepad.current.ResetHaptics();
        }
        // ȷ���˳�ʱ�ָ�״̬
        if (_isPausing)
        {
            TimeManager.Instance?.SetGlobalTimeScale(1, true);
            Camera.main.transform.position = _cameraOriginalPos;
        }
    }
}
