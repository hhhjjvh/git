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
/// 场景加载管理器（单例模式），负责处理异步场景加载、过渡动画和资源管理
/// 实现ISaveManager接口以支持场景状态的保存和加载
/// </summary>
public class SceneLoadManager : MonoBehaviour, ISaveManager
{
    // 单例实例
    public static SceneLoadManager Instance { get; private set; }

    [Header("配置参数")]
    [SerializeField] private GameSceneSO firstLoadScene;    // 游戏启动时首次加载的场景
    [SerializeField] private GameSceneSO fallbackScene;     // 加载失败时的回退场景
    [SerializeField] private float fadeDuration = 0.5f;    // 淡入淡出动画持续时间
    [SerializeField] private float minLoadTime = 1f;        // 最小加载时间（用于保证加载动画展示）

    [Header("依赖项")]
    [SerializeField] private UiFadeScreen fadeScreen;       // 淡入淡出屏幕控制组件
    [SerializeField] private GameObject loadingUI;          // 加载界面根物体
    [SerializeField] private Slider progressBar;            // 加载进度条
    [SerializeField] private SceneLoadEventSO sceneLoadEvent;// 场景加载事件通道

    // 运行时状态
    private GameSceneSO currentScene;                       // 当前激活的场景
    private GameSceneSO targetScene;                        // 目标加载场景
    private Vector3 targetPosition;                         // 玩家生成位置
    private AsyncOperationHandle<SceneInstance> currentLoadOperation; // 当前加载操作句柄
    private CancellationTokenSource loadCancellationTokenSource;      // 取消令牌源
    private bool isLoading;                                  // 加载状态标记

    private void Awake()
    {
        // 单例模式初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 跨场景持久化
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        await InitializeFirstScene(); // 初始化第一个场景
    }

    // 事件订阅/取消订阅
    private void OnEnable() => sceneLoadEvent.OnSceneLoad += OnSceneLoadRequest;
    private void OnDisable() => sceneLoadEvent.OnSceneLoad -= OnSceneLoadRequest;

    /// <summary>
    /// 初始化第一个场景（游戏启动时调用）
    /// </summary>
    private async UniTask InitializeFirstScene()
    {
        currentScene ??= firstLoadScene; // 空合并赋值
        await TransitionScene(firstLoadScene, Vector3.zero);
    }

    /// <summary>
    /// 场景加载请求处理（事件监听）
    /// </summary>
    private async void OnSceneLoadRequest(GameSceneSO scene, Vector3 position, bool _)
    {
        if (isLoading) return;
        await TransitionScene(scene, position);
    }

    /// <summary>
    /// 场景过渡核心方法（包含完整的加载流程）
    /// </summary>
    public async UniTask TransitionScene(GameSceneSO newScene, Vector3 spawnPosition)
    {
        try
        {
            isLoading = true;
            loadCancellationTokenSource = new CancellationTokenSource();

            // 阶段1：预加载资源
            var preloadHandle = Addressables.DownloadDependenciesAsync(newScene.SceneReference.RuntimeKey);

            // 显示加载界面
            await fadeScreen.FadeOut(fadeDuration);
            loadingUI?.SetActive(true);
            // 绑定取消按钮
            if (loadingUI.TryGetComponent<LoadingUI>(out var loadingUIControl))
            {
                loadingUIControl.ShowCancelButton(CancelLoading);
            }
            progressBar.value = 0;

           // 并行等待：最小加载时间 + 实际加载进度
            await UniTask.WhenAll(
                TrackProgress(preloadHandle, loadCancellationTokenSource.Token),
                UniTask.Delay(TimeSpan.FromSeconds(minLoadTime),
                cancellationToken: loadCancellationTokenSource.Token
            ));

            // 阶段2：卸载当前场景
            if (currentScene != null)
            {
                await UnloadScene(currentScene);
            }

            // 阶段3：加载新场景
            targetScene = newScene;
            targetPosition = spawnPosition;
            await LoadSceneAdditive(newScene);

            // 阶段4：场景初始化
            currentScene = newScene;
           // PlayerManager.instance.player.transform.position = spawnPosition;
            // CameraController.Instance.UpdateCameraBounds(); // 按需启用摄像机边界更新

            // 隐藏加载界面
            await fadeScreen.FadeIn(fadeDuration);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("场景加载已取消");
            await HandleFallback();
        }
        catch (Exception e)
        {
            Debug.LogError($"场景加载失败: {e.Message}");
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
    /// 跟踪加载进度并更新进度条
    /// </summary>
    private async UniTask TrackProgress(AsyncOperationHandle handle, CancellationToken ct)
    {
        while (!handle.IsDone)
        {
           // progressBar.value = handle.PercentComplete;
           loadingUI.GetComponent<LoadingUI>().UpdateProgress(handle.PercentComplete);
            Debug.Log(handle.PercentComplete);
            await UniTask.Yield(ct); // 每帧更新
        }
    }
    // 新增进度跟踪方法
    //private async UniTask TrackOverallProgress(AsyncOperationHandle preloadHandle, CancellationToken ct)
    //{
    //    // 预加载阶段 (0-50%)
    //    while (!preloadHandle.IsDone)
    //    {
    //        progressBar.value = preloadHandle.PercentComplete * 0.5f;
    //        await UniTask.Yield(ct);
    //    }

    //    // 卸载旧场景阶段 (50-70%)
    //    if (currentScene != null)
    //    {
    //        var unloadOperation = UnloadScene(currentScene);
    //        while (!unloadOperation.IsDone)
    //        {
    //            progressBar.value = 0.5f + unloadOperation.PercentComplete * 0.2f;
    //            await UniTask.Yield(ct);
    //        }
    //    }

    //    // 加载新场景阶段 (70-100%)
    //    var loadOperation = LoadSceneAdditive(targetScene);
    //    while (!loadOperation.IsDone)
    //    {
    //        progressBar.value = 0.7f + loadOperation.PercentComplete * 0.3f;
    //        await UniTask.Yield(ct);
    //    }
    //}
    /// <summary>
    /// 异常处理：加载回退场景
    /// </summary>
    private async UniTask HandleFallback()
    {
        //// 检查当前场景有效性
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
                Debug.Log("回退场景加载成功");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"回退场景加载失败: {e.Message}");
            // 强制加载默认场景
            SceneManager.LoadScene("Game2");
        }
        await fadeScreen.FadeIn(fadeDuration);
    }

    /// <summary>
    /// 取消加载操作
    /// </summary>
    public void CancelLoading()
    {
        loadCancellationTokenSource?.Cancel();
    }

    /// <summary>
    /// 附加模式加载场景（Addressables系统）
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
            // 设置新场景为活跃场景
            if (currentLoadOperation.Result.Scene.IsValid())
            {
                SceneManager.SetActiveScene(currentLoadOperation.Result.Scene);
            }
            if (currentLoadOperation.Status != AsyncOperationStatus.Succeeded)
                throw new Exception($"场景加载失败: {scene.name}");
        }
        catch
        {
            // 异常时尝试加载回退场景
            currentLoadOperation = fallbackScene.SceneReference.LoadSceneAsync(
                LoadSceneMode.Additive);
            await currentLoadOperation;
            throw;
        }
    }

    /// <summary>
    /// 卸载场景
    /// </summary>
    private async UniTask UnloadScene(GameSceneSO scene)
    {
        var unloadOperation = scene.SceneReference.UnLoadScene();
        await unloadOperation;

        if (unloadOperation.Status != AsyncOperationStatus.Succeeded)
            Debug.LogError($"场景卸载失败: {scene.name}");
    }

    /// <summary>
    /// 预加载场景资源（可用于提前加载常用场景）
    /// </summary>
    public void PreloadScene(GameSceneSO scene)
    {
        Addressables.DownloadDependenciesAsync(scene.SceneReference.RuntimeKey);
    }

    // 存档系统接口实现
    public void LoadData(GameData data) => currentScene = data.LoadGameScene();
    public void SaveData(ref GameData data) => data.SaveGameScene(currentScene);

    /// <summary>
    /// 销毁时资源清理
    /// </summary>
    private void OnDestroy()
    {
        loadCancellationTokenSource?.Cancel();
        loadCancellationTokenSource?.Dispose();

        if (currentLoadOperation.IsValid())
            Addressables.Release(currentLoadOperation);
    }
}