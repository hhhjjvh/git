using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;


    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    public bool playBGM;
    private int bgmIndex;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (bgm.Length > 0)
        {
            if (!playBGM)
            {
                StopAllBGM();
            }
            else
            {
                if (!bgm[bgmIndex].isPlaying)
                {
                    PlayBGM(bgmIndex);
                }
            }
        }
        
    }
    public void PlaySFX(int index,Transform transform)
    {
       // Debug.Log(index);
        //if(sfx[index].isPlaying) return;

        if (transform!=null&&Vector2.Distance(PlayerManager.instance.player.transform.position, transform.position) > 15f) return;

        if (index < sfx.Length)
        {
            sfx[index].pitch= Random.Range(0.9f, 1.1f);
            sfx[index].Play();
        }
    }
    public void PlayPlayerHitSFX()
    {
        int rand= Random.Range(3,6);
        sfx[rand].pitch = Random.Range(0.9f, 1.1f);
        sfx[rand].Play();
    }

    public void StopSFX(int index)
    {
        sfx[index].Stop();
    }
    public void StopSFXWithTime(int index)
    {
       if(index > sfx.Length)  return;
        StartCoroutine(DecreaseVolume(sfx[index]));
    }
    private IEnumerator DecreaseVolume(AudioSource audioSource)
    {
        float defaultVolume = audioSource.volume;
        while (audioSource.volume > 0.1f)
        {
            audioSource.volume -= audioSource.volume *0.2f;
            yield return new WaitForSeconds(0.25f);
            if (audioSource.volume <= 0.1f)
            {
                audioSource.Stop();
                audioSource.volume = defaultVolume;
                break;
            }
        }
    }
    public void StopBGM(int index)
    {
        bgm[index].Stop();
    }
    public void PlayRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }
    public void PlayBGM(int index)
    {
        bgmIndex = index;
        StopAllBGM();
        bgm[bgmIndex].Play();
    }
    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }
    public void StopAllSFX()
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            sfx[i].Stop();
        }
    }
}
