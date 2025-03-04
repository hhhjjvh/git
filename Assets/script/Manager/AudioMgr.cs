using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine.Audio;

public enum PlayMode { Sequential, Random, LoopSingle }
public enum FadeMode { FadeOutIn, CrossFade }

public class AudioMgr : MonoBehaviour
{
    public static AudioMgr Instance { get; private set; }

    [Header("Mixer Settings")]
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private string _masterVolumeParam = "MasterVolume";
    [SerializeField] private string _musicVolumeParam = "MusicVolume";
    [SerializeField] private string _sfxVolumeParam = "SFXVolume";

    [Header("Base Settings")]
    [SerializeField] private int _initialSFXPoolSize = 5;
    [SerializeField][Range(0, 1)] private float _defaultMusicVolume = 1.0f;
    [SerializeField][Range(0, 1)] private float _defaultSFXVolume = 1.0f;

    [Header("Advanced Settings")]
    [SerializeField] private float _fadeDuration = 1.5f;
    [SerializeField] private int _maxPriorityLevel = 3;

    // ����ϵͳ���
    private AudioSource _musicSource;
    private AudioSource _secondaryMusicSource; // ���ڽ��浭�뵭��
    private readonly Dictionary<string, AudioClip> _loadedMusic = new();
    private readonly Dictionary<string, AudioClip> _loadedSFX = new();

    // �����б�ϵͳ
    private List<string> _playlist = new();
    private int _currentTrackIndex = -1;
    private PlayMode _playMode = PlayMode.Sequential;

    // ��Чϵͳ
    private readonly List<SFXChannel> _sfxChannels = new();
    private readonly Queue<SFXChannel> _availableChannels = new();

    // Ԥ����ϵͳ
    private readonly HashSet<string> _preloadedMusic = new();
    private readonly HashSet<string> _preloadedSFX = new();

    #region ��ʼ��������ܹ�
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
            InitializeSFXPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        _musicSource = gameObject.AddComponent<AudioSource>();
        _secondaryMusicSource = gameObject.AddComponent<AudioSource>();

        foreach (var source in new[] { _musicSource, _secondaryMusicSource })
        {
            source.loop = true;
            source.volume = 0;
            source.playOnAwake = false;
            source.outputAudioMixerGroup = _audioMixer.FindMatchingGroups("BGM")[0];
        }
    }

    private void InitializeSFXPool()
    {
        var sfxGroup = _audioMixer.FindMatchingGroups("SFX")[0];

        for (int i = 0; i < _initialSFXPoolSize; i++)
        {
            CreateNewSFXChannel(sfxGroup);
        }
    }

    private void CreateNewSFXChannel(AudioMixerGroup group)
    {
        var obj = new GameObject("SFX_Channel");
        obj.transform.SetParent(transform);

        var channel = new SFXChannel
        {
            Source = obj.AddComponent<AudioSource>(),
            Priority = _maxPriorityLevel
        };

        channel.Source.playOnAwake = false;
        channel.Source.outputAudioMixerGroup = group;
        _availableChannels.Enqueue(channel);
        _sfxChannels.Add(channel);
    }
    #endregion
    // ===========================================
    // ��Ƶ���������ϵͳ
    // ===========================================
    public void SetMasterVolume(float volume)
    {
        SetMixerVolume(_masterVolumeParam, volume);
        PlayerPrefs.SetFloat(_masterVolumeParam, volume);
    }

    public void SetMusicVolume(float volume)
    {
        SetMixerVolume(_musicVolumeParam, volume);
        PlayerPrefs.SetFloat(_musicVolumeParam, volume);
    }

    public void SetSFXVolume(float volume)
    {
        SetMixerVolume(_sfxVolumeParam, volume);
        PlayerPrefs.SetFloat(_sfxVolumeParam, volume);
    }

    private void SetMixerVolume(string parameter, float volume)
    {
        float dB = volume > 0 ? 20 * Mathf.Log10(volume) : -80;
        _audioMixer.SetFloat(parameter, dB);
    }

    private void LoadVolumeSettings()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(_masterVolumeParam, 1));
        SetMusicVolume(PlayerPrefs.GetFloat(_musicVolumeParam, 1));
        SetSFXVolume(PlayerPrefs.GetFloat(_sfxVolumeParam, 1));
    }

    // ===========================================
    // Ԥ����ϵͳ
    // ===========================================
    public IEnumerator PreloadMusic(IEnumerable<string> musicNames)
    {
        foreach (var name in musicNames)
        {
            if (_loadedMusic.ContainsKey(name) || _preloadedMusic.Contains(name)) continue;

            _preloadedMusic.Add(name);
            var handle = Addressables.LoadAssetAsync<AudioClip>(name);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _loadedMusic[name] = handle.Result;
            }
        }
    }

    public IEnumerator PreloadSFX(IEnumerable<string> sfxNames)
    {
        foreach (var name in sfxNames)
        {
            if (_loadedSFX.ContainsKey(name) || _preloadedSFX.Contains(name)) continue;

            _preloadedSFX.Add(name);
            var handle = Addressables.LoadAssetAsync<AudioClip>(name);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _loadedSFX[name] = handle.Result;
            }
        }
    }

    // ===========================================
    // ���ֲ���ϵͳ�������뵭����
    // ===========================================
    public void PlayMusic(string musicName, FadeMode fadeMode = FadeMode.FadeOutIn)
    {
        StartCoroutine(LoadAndPlayMusic(musicName, fadeMode));
    }

    private IEnumerator LoadAndPlayMusic(string musicName, FadeMode fadeMode)
    {
        if (!_loadedMusic.TryGetValue(musicName, out AudioClip clip))
        {
            var handle = Addressables.LoadAssetAsync<AudioClip>(musicName);
            yield return handle;
            if (handle.Status != AsyncOperationStatus.Succeeded) yield break;
            clip = handle.Result;
            _loadedMusic[musicName] = clip;
        }

        switch (fadeMode)
        {
            case FadeMode.FadeOutIn:
                yield return StartCoroutine(FadeOut(_musicSource, _fadeDuration));
                StartCoroutine(FadeIn(_musicSource, clip, _fadeDuration));
                break;
            case FadeMode.CrossFade:
                yield return StartCoroutine(CrossFade(clip, _fadeDuration));
                break;
        }
    }

    private IEnumerator CrossFade(AudioClip newClip, float duration)
    {
        AudioSource oldSource = _musicSource;
        AudioSource newSource = _secondaryMusicSource;

        newSource.clip = newClip;
        newSource.Play();

        float timer = 0;
        while (timer < duration)
        {
            float ratio = timer / duration;
            oldSource.volume = Mathf.Lerp(_defaultMusicVolume, 0, ratio);
            newSource.volume = Mathf.Lerp(0, _defaultMusicVolume, ratio);
            timer += Time.deltaTime;
            yield return null;
        }

        oldSource.Stop();
        oldSource.volume = 0;
        (_musicSource, _secondaryMusicSource) = (newSource, oldSource);
    }

    // ===========================================
    // ��Чϵͳ�������ȼ���
    // ===========================================
    public void PlaySFX(string sfxName, int priority = 0, float volumeScale = 1.0f)
    {
        if (priority > _maxPriorityLevel) priority = _maxPriorityLevel;
        StartCoroutine(LoadAndPlaySFX(sfxName, priority, volumeScale));
    }

    private IEnumerator LoadAndPlaySFX(string sfxName, int priority, float volumeScale)
    {
        if (!_loadedSFX.TryGetValue(sfxName, out AudioClip clip))
        {
            var handle = Addressables.LoadAssetAsync<AudioClip>(sfxName);
            yield return handle;
            if (handle.Status != AsyncOperationStatus.Succeeded) yield break;
            clip = handle.Result;
            _loadedSFX[sfxName] = clip;
        }

        SFXChannel targetChannel = GetAvailableChannel(priority);
        if (targetChannel == null) yield break;

        targetChannel.Source.PlayOneShot(clip, _defaultSFXVolume * volumeScale);
        StartCoroutine(ReleaseSFXChannel(targetChannel));
    }

    private SFXChannel GetAvailableChannel(int priority)
    {
        // ���Ի�ȡ����ͨ��
        if (_availableChannels.Count > 0)
        {
            var channel = _availableChannels.Dequeue();
            channel.Priority = priority;
            return channel;
        }

        // Ѱ�ҿ��жϵĵ����ȼ�ͨ��
        var replaceable = _sfxChannels
            .Where(c => c.InUse && c.Priority > priority)
            .OrderByDescending(c => c.Priority)
            .FirstOrDefault();

        if (replaceable != null)
        {
            replaceable.Source.Stop();
            replaceable.Priority = priority;
            return replaceable;
        }

        // û�п���ͨ��ʱ������ͨ��
        var sfxGroup = _audioMixer.FindMatchingGroups("SFX")[0];
        CreateNewSFXChannel(sfxGroup);
        return _availableChannels.Dequeue();
    }

    private IEnumerator ReleaseSFXChannel(SFXChannel channel)
    {
        yield return new WaitWhile(() => channel.Source.isPlaying);
        channel.Priority = _maxPriorityLevel;
        _availableChannels.Enqueue(channel);
    }

    // ===========================================
    // �����б�ϵͳ
    // ===========================================
    public void SetPlaylist(List<string> trackNames, PlayMode mode = PlayMode.Sequential)
    {
        _playlist = new List<string>(trackNames);
        _playMode = mode;
        _currentTrackIndex = -1;
        PlayNextTrack();
    }

    private void PlayNextTrack()
    {
        if (_playlist.Count == 0) return;

        switch (_playMode)
        {
            case PlayMode.Sequential:
                _currentTrackIndex = (_currentTrackIndex + 1) % _playlist.Count;
                break;
            case PlayMode.Random:
                _currentTrackIndex = GetUniqueRandomIndex();
                break;
            case PlayMode.LoopSingle:
                _currentTrackIndex = _currentTrackIndex == -1 ? 0 : _currentTrackIndex;
                break;
        }

        PlayMusic(_playlist[_currentTrackIndex], FadeMode.CrossFade);
        StartCoroutine(MonitorTrackCompletion());
    }

    private IEnumerator MonitorTrackCompletion()
    {
        yield return new WaitWhile(() => _musicSource.isPlaying);
        PlayNextTrack();
    }

    // ===========================================
    // ���߷���
    // ===========================================
    private IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float timer = 0f;

        while (timer < duration)
        {
            source.volume = Mathf.Lerp(startVolume, 0, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        source.Stop();
    }

    private IEnumerator FadeIn(AudioSource source, AudioClip clip, float duration)
    {
        source.clip = clip;
        source.Play();
        float timer = 0f;

        while (timer < duration)
        {
            source.volume = Mathf.Lerp(0, _defaultMusicVolume, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        source.volume = _defaultMusicVolume;
    }

    private int GetUniqueRandomIndex()
    {
        if (_playlist.Count == 1) return 0;
        int newIndex;
        do
        {
            newIndex = Random.Range(0, _playlist.Count);
        } while (newIndex == _currentTrackIndex);
        return newIndex;
    }

    // ===========================================
    // ��������ṹ
    // ===========================================
    private class SFXChannel
    {
        public AudioSource Source;
        public int Priority;
        public bool InUse => Source.isPlaying;
    }
}