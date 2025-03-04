using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// �������ع�����������ģʽ�����������첽�������ء����ɶ�������Դ����
/// ʵ��ISaveManager�ӿ���֧�ֳ���״̬�ı���ͼ���
/// </summary>
public class SceneLoadManager : MonoBehaviour, ISaveManager
{
    // ����ʵ��
    public static SceneLoadManager Instance { get; private set; }

    [Header("���ò���")]
    [SerializeField] private GameSceneSO firstLoadScene;    // ��Ϸ����ʱ�״μ��صĳ���
    [SerializeField] private GameSceneSO fallbackScene;     // ����ʧ��ʱ�Ļ��˳���
    [SerializeField] private float fadeDuration = 0.5f;    // ���뵭����������ʱ��
    [SerializeField] private float minLoadTime = 1f;        // ��С����ʱ�䣨���ڱ�֤���ض���չʾ��

    [Header("������")]
    [SerializeField] private UiFadeScreen fadeScreen;       // ���뵭����Ļ�������
    [SerializeField] private GameObject loadingUI;          // ���ؽ��������
    [SerializeField] private Slider progressBar;            // ���ؽ�����
    [SerializeField] private SceneLoadEventSO sceneLoadEvent;// ���������¼�ͨ��

    // ����ʱ״̬
    private GameSceneSO currentScene;                       // ��ǰ����ĳ���
    private GameSceneSO targetScene;                        // Ŀ����س���
    private Vector3 targetPosition;                         // �������λ��
    private AsyncOperationHandle<SceneInstance> currentLoadOperation; // ��ǰ���ز������
    private CancellationTokenSource loadCancellationTokenSource;      // ȡ������Դ
    private bool isLoading;                                  // ����״̬���

    private void Awake()
    {
        // ����ģʽ��ʼ��
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �糡���־û�
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        await InitializeFirstScene(); // ��ʼ����һ������
    }

    // �¼�����/ȡ������
    private void OnEnable() => sceneLoadEvent.OnSceneLoad += OnSceneLoadRequest;
    private void OnDisable() => sceneLoadEvent.OnSceneLoad -= OnSceneLoadRequest;

    /// <summary>
    /// ��ʼ����һ����������Ϸ����ʱ���ã�
    /// </summary>
    private async UniTask InitializeFirstScene()
    {
        currentScene ??= firstLoadScene; // �պϲ���ֵ
        await TransitionScene(firstLoadScene, Vector3.zero);
    }

    /// <summary>
    /// ���������������¼�������
    /// </summary>
    private async void OnSceneLoadRequest(GameSceneSO scene, Vector3 position, bool _)
    {
        if (isLoading) return;
        await TransitionScene(scene, position);
    }

    /// <summary>
    /// �������ɺ��ķ��������������ļ������̣�
    /// </summary>
    public async UniTask TransitionScene(GameSceneSO newScene, Vector3 spawnPosition)
    {
        try
        {
            isLoading = true;
            loadCancellationTokenSource = new CancellationTokenSource();

            // �׶�1��Ԥ������Դ
            var preloadHandle = Addressables.DownloadDependenciesAsync(newScene.SceneReference.RuntimeKey);

            // ��ʾ���ؽ���
            await fadeScreen.FadeOut(fadeDuration);
            loadingUI?.SetActive(true);
            // ��ȡ����ť
            if (loadingUI.TryGetComponent<LoadingUI>(out var loadingUIControl))
            {
                loadingUIControl.ShowCancelButton(CancelLoading);
            }
            progressBar.value = 0;

           // ���еȴ�����С����ʱ�� + ʵ�ʼ��ؽ���
            await UniTask.WhenAll(
                TrackProgress(preloadHandle, loadCancellationTokenSource.Token),
                UniTask.Delay(TimeSpan.FromSeconds(minLoadTime),
                cancellationToken: loadCancellationTokenSource.Token
            ));

            // �׶�2��ж�ص�ǰ����
            if (currentScene != null)
            {
                await UnloadScene(currentScene);
            }

            // �׶�3�������³���
            targetScene = newScene;
            targetPosition = spawnPosition;
            await LoadSceneAdditive(newScene);

            // �׶�4��������ʼ��
            currentScene = newScene;
           // PlayerManager.instance.player.transform.position = spawnPosition;
            // CameraController.Instance.UpdateCameraBounds(); // ��������������߽����

            // ���ؼ��ؽ���
            await fadeScreen.FadeIn(fadeDuration);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("����������ȡ��");
            await HandleFallback();
        }
        catch (Exception e)
        {
            Debug.LogError($"��������ʧ��: {e.Message}");
            await HandleFallback();
        }
        finally
        {
            isLoading = false;
            loadingUI?.SetActive(false);
            loadCancellationTokenSource?.Dispose();
        }
    }

    /// <summary>
    /// ���ټ��ؽ��Ȳ����½�����
    /// </summary>
    private async UniTask TrackProgress(AsyncOperationHandle handle, CancellationToken ct)
    {
        while (!handle.IsDone)
        {
           // progressBar.value = handle.PercentComplete;
           loadingUI.GetComponent<LoadingUI>().UpdateProgress(handle.PercentComplete);
            Debug.Log(handle.PercentComplete);
            await UniTask.Yield(ct); // ÿ֡����
        }
    }
    // �������ȸ��ٷ���
    //private async UniTask TrackOverallProgress(AsyncOperationHandle preloadHandle, CancellationToken ct)
    //{
    //    // Ԥ���ؽ׶� (0-50%)
    //    while (!preloadHandle.IsDone)
    //    {
    //        progressBar.value = preloadHandle.PercentComplete * 0.5f;
    //        await UniTask.Yield(ct);
    //    }

    //    // ж�ؾɳ����׶� (50-70%)
    //    if (currentScene != null)
    //    {
    //        var unloadOperation = UnloadScene(currentScene);
    //        while (!unloadOperation.IsDone)
    //        {
    //            progressBar.value = 0.5f + unloadOperation.PercentComplete * 0.2f;
    //            await UniTask.Yield(ct);
    //        }
    //    }

    //    // �����³����׶� (70-100%)
    //    var loadOperation = LoadSceneAdditive(targetScene);
    //    while (!loadOperation.IsDone)
    //    {
    //        progressBar.value = 0.7f + loadOperation.PercentComplete * 0.3f;
    //        await UniTask.Yield(ct);
    //    }
    //}
    /// <summary>
    /// �쳣�������ػ��˳���
    /// </summary>
    private async UniTask HandleFallback()
    {
        //// ��鵱ǰ������Ч��
        //if (currentScene == null || !currentScene.SceneReference.RuntimeKeyIsValid())
        //{
        //    await LoadSceneAdditive(fallbackScene);
        //    currentScene = fallbackScene;
        //}
        try
        {
            if (currentScene == null || !currentScene.SceneReference.RuntimeKeyIsValid())
            {
                await LoadSceneAdditive(fallbackScene);
                currentScene = fallbackScene;
                Debug.Log("���˳������سɹ�");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"���˳�������ʧ��: {e.Message}");
            // ǿ�Ƽ���Ĭ�ϳ���
            SceneManager.LoadScene("Game2");
        }
        await fadeScreen.FadeIn(fadeDuration);
    }

    /// <summary>
    /// ȡ�����ز���
    /// </summary>
    public void CancelLoading()
    {
        loadCancellationTokenSource?.Cancel();
    }

    /// <summary>
    /// ����ģʽ���س�����Addressablesϵͳ��
    /// </summary>
    private async UniTask LoadSceneAdditive(GameSceneSO scene)
    {
        try
        {
            currentLoadOperation = scene.SceneReference.LoadSceneAsync(
                LoadSceneMode.Additive,
                activateOnLoad: true,
                priority: 100
            );

            await currentLoadOperation.ToUniTask();
            // �����³���Ϊ��Ծ����
            if (currentLoadOperation.Result.Scene.IsValid())
            {
                SceneManager.SetActiveScene(currentLoadOperation.Result.Scene);
            }
            if (currentLoadOperation.Status != AsyncOperationStatus.Succeeded)
                throw new Exception($"��������ʧ��: {scene.name}");
        }
        catch
        {
            // �쳣ʱ���Լ��ػ��˳���
            currentLoadOperation = fallbackScene.SceneReference.LoadSceneAsync(
                LoadSceneMode.Additive);
            await currentLoadOperation;
            throw;
        }
    }

    /// <summary>
    /// ж�س���
    /// </summary>
    private async UniTask UnloadScene(GameSceneSO scene)
    {
        var unloadOperation = scene.SceneReference.UnLoadScene();
        await unloadOperation;

        if (unloadOperation.Status != AsyncOperationStatus.Succeeded)
            Debug.LogError($"����ж��ʧ��: {scene.name}");
    }

    /// <summary>
    /// Ԥ���س�����Դ����������ǰ���س��ó�����
    /// </summary>
    public void PreloadScene(GameSceneSO scene)
    {
        Addressables.DownloadDependenciesAsync(scene.SceneReference.RuntimeKey);
    }

    // �浵ϵͳ�ӿ�ʵ��
    public void LoadData(GameData data) => currentScene = data.LoadGameScene();
    public void SaveData(ref GameData data) => data.SaveGameScene(currentScene);

    /// <summary>
    /// ����ʱ��Դ����
    /// </summary>
    private void OnDestroy()
    {
        loadCancellationTokenSource?.Cancel();
        loadCancellationTokenSource?.Dispose();

        if (currentLoadOperation.IsValid())
            Addressables.Release(currentLoadOperation);
    }
}