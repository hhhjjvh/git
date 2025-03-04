using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using System;

/// <summary>
/// ��Ϸ������ʱ��ṹ�壨֧����/��/��/ʱ/��/�룩
/// </summary>
[System.Serializable]
public struct GameDateTime
{
    public int year;     // ��ݣ���1��ʼ��
    public int month;    // �·ݣ�1-12��
    public int day;      // ���ڣ�1-31�������·ݵ�����
    public int hour;     // Сʱ��0-23��
    public int minute;   // ���ӣ�0-59��
    public float second; // �루0-59.999...��

    /// <summary>
    /// ����ָ����������׼��ʱ��
    /// </summary>
    public void AddSeconds(float deltaSeconds)
    {
        second += deltaSeconds;
        NormalizeTime();
    }

    /// <summary>
    /// ʱ���׼�������Զ���λ��
    /// </summary>
    public void NormalizeTime()
    {
        // �� -> ���ӽ�λ
        minute += (int)(second / 60);
        second %= 60;

        // ���� -> Сʱ��λ
        hour += minute / 60;
        minute %= 60;

        // Сʱ -> ���λ
        day += hour / 24;
        hour %= 24;

        // �����·ݺ���ݽ�λ����ѭ��������������
        while (day > GetDaysInMonth(month, year))
        {
            day -= GetDaysInMonth(month, year);
            month++;
            if (month <= 12) continue;
            month -= 12;
            year++;
        }
    }

    /// <summary>
    /// ��ȡָ���·ݵ��������������꣩
    /// </summary>
    /// <param name="currentMonth">Ŀ���·ݣ�1-12��</param>
    /// <param name="currentYear">Ŀ����ݣ�����������㣩</param>
    public int GetDaysInMonth(int currentMonth, int currentYear)
    {
        return currentMonth switch
        {
            2 => IsLeapYear(currentYear) ? 29 : 28, // ��������������29�죩
            4 or 6 or 9 or 11 => 30,  // С��
            _ => 31,                   // ����
        };
    }

    /// <summary>
    /// �����жϣ����ϸ������������
    /// �����ܱ�4���������ܱ�100���������ܱ�400����
    /// </summary>
   public static bool IsLeapYear(int year)
    {
        return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
    }

    /// <summary>
    /// ��ʽ��ʱ���ַ�����yyyy-MM-dd HH:mm:ss.ff��
    /// </summary>
    public override string ToString()
    {
        return $"{year:0000}-{month:00}-{day:00} {hour:00}:{minute:00}:{second:00.0}";
    }
}

/// <summary>
/// ʱ�����ϵͳ������ģʽ��
/// ���ܰ�����
/// 1. ��Ϸʱ�����ٿ���
/// 2. ʵ��ʱ�����Ź���
/// 3. ʱ�䶳�Ṧ��
/// 4. ��ʱ�¼�����
/// </summary>
public class TimeManager : MonoBehaviour,ISaveManager
{
    // ����ʵ��
    public static TimeManager Instance { get; private set; }

    [Header("ʱ������")]
    [SerializeField] private GameDateTime _initialDateTime; // ��ʼ����ʱ��
    [SerializeField] private float _baseTimeScale = 1f;     // ��׼ʱ������
    [SerializeField] private float _timeScaleLerpSpeed = 5f;// ʱ�����Ź����ٶ�

    [Header("ʵ��ʱ������ϵ��")]
    [SerializeField] private float _globalTimeScale = 1f;   // ȫ�ֻ�������
    [SerializeField] private float _characterTimeScale = 1f;// ��ɫʱ������
    [SerializeField] private float _enemyTimeScale = 1f;    // ����ʱ������
    [SerializeField] private float _trapTimeScale = 1f;     // ����ʱ������

    [Header("ʱ�䶳��״̬")]
    [SerializeField] private bool _freezeGlobalTime;    // ȫ��ʱ�䶳��
    [SerializeField] private bool _freezeCharacters;    // ��ɫ����
    [SerializeField] private bool _freezeEnemies;       // ���˶���
    [SerializeField] private bool _freezeTraps;         // ���嶳��

    [Header("����ʱ��ͳ��")]
    [SerializeField] private bool _countPlaytimeWhenPaused = false; // �Ƿ�����ͣʱͳ��ʱ��
    private float _totalPlaytimeSeconds;          // ������ʱ�䣨�룩
    private DateTime _lastSaveTime;               // ���һ�α���ʱ���

    // ����ʱ״̬
    private GameDateTime _currentDateTime;       // ��ǰ��Ϸʱ��
    private float _targetTimeScale;              // Ŀ��ʱ������
    private float _currentTimeScale;             // ��ǰʵ��ʱ������
    private List<TimeEvent> _scheduledEvents = new List<TimeEvent>(); // Ԥ���¼��б�

    // ���Է�����
    public GameDateTime CurrentTime => _currentDateTime; // ��ǰʱ�䣨ֻ����
    public float WorldTimeScale => _currentTimeScale;     // ��ǰ����ʱ������

    private void Awake()
    {
        // ����ģʽ��ʼ��
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeTimeSystem();
       
    }

    /// <summary>
    /// ��ʼ��ʱ��ϵͳ
    /// </summary>
    private void InitializeTimeSystem()
    {
        _currentDateTime = _initialDateTime;
        _targetTimeScale = _baseTimeScale;
        _currentTimeScale = _baseTimeScale;
        Time.fixedDeltaTime = 0.02f * _currentTimeScale; // ��ʼ������ʱ�䲽��
    }

    private void Update()
    {
        UpdateTimeScaling();         // ����ʱ�����Ź���
        if (!_freezeGlobalTime)
            UpdateWorldTime(Time.unscaledDeltaTime); // ʹ��δ����ʱ���ƽ���Ϸʱ��
        CheckScheduledEvents();     // ���Ԥ���¼�
        UpdatePlaytimeCounting();   // ��������ʱ��ͳ��
    }
    /// <summary>
    /// ��������ʱ��ͳ��
    /// </summary>
    private void UpdatePlaytimeCounting()
    {
        bool shouldCount = _countPlaytimeWhenPaused || !_freezeGlobalTime;
        if (shouldCount) _totalPlaytimeSeconds += Time.unscaledDeltaTime;
    }

    /// <summary>
    /// ��ȡ��ʽ����������ʱ��
    /// </summary>
    public string GetFormattedPlaytime()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(_totalPlaytimeSeconds);
        return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }

    

    /// <summary>
    /// ƽ������ʱ������
    /// </summary>
    private void UpdateTimeScaling()
    {
        _currentTimeScale = Mathf.Lerp(
            _currentTimeScale,
            _targetTimeScale,
            _timeScaleLerpSpeed * Time.unscaledDeltaTime // ʹ��δ����ʱ����в�ֵ
        );
        Time.timeScale = _currentTimeScale;               // ����Unityʱ������
        Time.fixedDeltaTime = 0.02f * _currentTimeScale;  // ͬ������ʱ�䲽��
    }

    /// <summary>
    /// ������Ϸ����ʱ��
    /// </summary>
    /// <param name="deltaTime">δ���ŵ�ʱ������</param>
    private void UpdateWorldTime(float deltaTime)
    {
        // Ӧ��ȫ��ʱ������ϵ��
        float effectiveDelta = deltaTime * _globalTimeScale;
        _currentDateTime.AddSeconds(effectiveDelta);
    }

    /// <summary>
    /// ����ȫ��ʱ������
    /// </summary>
    /// <param name="scale">Ŀ�����٣�1=������</param>
    /// <param name="instant">�Ƿ�������Ч</param>
    public void SetGlobalTimeScale(float scale, bool instant = false)
    {
        _targetTimeScale = scale;
        if (instant) _currentTimeScale = scale;
    }

    /// <summary>
    /// ����ʱ��Ϊ��׼����
    /// </summary>
    public void ResetTimeScale(bool instant = false)
    {
        SetGlobalTimeScale(_baseTimeScale, instant);
    }

    /// <summary>
    /// ��ȡʵ��ʱ������ϵ��������ȫ�����źͶ���״̬��
    /// </summary>
    public float GetEntityTimeScale(EntityType entityType)
    {
        if (IsEntityFrozen(entityType)) return 0f;

        // ��ȡ��������ϵ��
        float baseScale = entityType switch
        {
            EntityType.Character => _characterTimeScale,
            EntityType.Enemy => _enemyTimeScale,
            EntityType.Trap => _trapTimeScale,
            _ => 1f
        };

        // �ۺ�ȫ�����š�ʵ�����ź͵�ǰʱ������
        return baseScale * _globalTimeScale * _currentTimeScale;
    }

    /// <summary>
    /// ���ʵ���Ƿ񱻶���
    /// </summary>
    public bool IsEntityFrozen(EntityType entityType)
    {
        return entityType switch
        {
            EntityType.Character => _freezeCharacters,
            EntityType.Enemy => _freezeEnemies,
            EntityType.Trap => _freezeTraps,
            EntityType.Global => _freezeGlobalTime,
            _ => false
        };
    }

    /// <summary>
    /// ����ʵ�����Ͷ���״̬
    /// </summary>
    public void FreezeEntityType(EntityType entityType, bool freeze)
    {
        switch (entityType)
        {
            case EntityType.Character: _freezeCharacters = freeze; break;
            case EntityType.Enemy: _freezeEnemies = freeze; break;
            case EntityType.Trap: _freezeTraps = freeze; break;
            case EntityType.Global: _freezeGlobalTime = freeze; break;
        }
    }

    /// <summary>
    /// ��Ӷ�ʱ�¼�������ʱ�䵥λΪ�룩
    /// </summary>
    public void ScheduleEvent(TimeEvent timeEvent)
    {
        _scheduledEvents.Add(timeEvent);
        // ������ʱ����������
        _scheduledEvents = _scheduledEvents.OrderBy(e => e.triggerTime).ToList();
    }

    /// <summary>
    /// ��鲢ִ�е����¼���������������������⣩
    /// </summary>
    private void CheckScheduledEvents()
    {
        for (int i = _scheduledEvents.Count - 1; i >= 0; i--)
        {
            var e = _scheduledEvents[i];
            if (GetTotalElapsedSeconds() >= e.triggerTime)
            {
                e.action.Invoke();
                _scheduledEvents.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// ��ȡ��Ϸ�����������������
    /// </summary>
    public float GetTotalElapsedSeconds()
    {
        int totalDays = 0;
        // ������ʷ�������
        for (int y = 1; y < _currentDateTime.year; y++)
            totalDays += GameDateTime.IsLeapYear(y) ? 366 : 365;

        // ���㱾���ѹ��·�����
        for (int m = 1; m < _currentDateTime.month; m++)
            totalDays += _currentDateTime.GetDaysInMonth(m, _currentDateTime.year);

        totalDays += _currentDateTime.day - 1; // ��ǰ���ѹ�����

        return totalDays * 86400 +          // ����ת��
               _currentDateTime.hour * 3600 +    // Сʱת��
               _currentDateTime.minute * 60 +    // ����ת��
               _currentDateTime.second;         // ��ǰ����
    }

    /// <summary>
    /// ��ת��ָ��ʱ�䣨�Զ���׼����
    /// </summary>
    public void JumpToDateTime(GameDateTime newDateTime)
    {
        newDateTime.NormalizeTime(); // �ȹ淶��ʱ������
        _currentDateTime = newDateTime;
    }

    public void LoadData(GameData data)
    {
        _totalPlaytimeSeconds = data._totalPlaytimeSeconds;
        _lastSaveTime = DateTime.Now;
    }

    public void SaveData(ref GameData data)
    {
        data._totalPlaytimeSeconds = _totalPlaytimeSeconds;
        _lastSaveTime = DateTime.Now;
    }

    /// <summary>
    /// ��ʱ�¼��ṹ��
    /// </summary>
    public struct TimeEvent
    {
        public float triggerTime;   // ����ʱ�䣨��������
        public UnityAction action;  // ��������
    }

    /// <summary>
    /// ʵ������ö��
    /// </summary>
    public enum EntityType { Character, Enemy, Trap, Global }
}