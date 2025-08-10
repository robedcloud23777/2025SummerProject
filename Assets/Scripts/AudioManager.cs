using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif

/// <summary>
/// 오디오 매니저 (싱글톤)
/// 사용법 요약:
/// 1) 빈 GameObject를 만들고 이름을 "AudioManager"로 변경
/// 2) 이 스크립트를 붙이고, Inspector에서 BGM Source와 SFX Source Pool, AudioMixer(선택)를 연결
/// 3) 다른 스크립트에서 AudioManager.Instance.PlayBGM(myClip), PlaySFX(myClip)등을 호출
/// 4) Photon으로 모든 클라이언트에서 동일한 SFX를 재생하려면 PlaySFXGlobal("clipName") 사용 (모든 클라이언트가 같은 AudioClip 이름을 Resources/Audio 폴더에 가지고 있어야 함)
/// 
/// Photon 관련 코드는 PHOTON_UNITY_NETWORKING 심볼로 감싸져 있습니다. Photon 패키지를 사용 중이라면 해당 심볼을 정의하거나 #if 블록을 제거하세요.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource oneShotSourcePrefab; // 풀용 프리팹
    [SerializeField] private int oneShotPoolSize = 8;

    [Header("Mixer (optional)")]
    [SerializeField] private AudioMixer audioMixer; // "Master", "BGM", "SFX" 같은 exposed params 사용 가능

    [Header("Settings")]
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float bgmVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    private Queue<AudioSource> sfxPool = new Queue<AudioSource>();

#if PHOTON_UNITY_NETWORKING
    private PhotonView photonView;
#endif

    private const string PREF_MASTER = "AM_Master";
    private const string PREF_BGM = "AM_BGM";
    private const string PREF_SFX = "AM_SFX";

    private void Awake()
    {
        // 싱글톤 처리
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // pool 초기화
        InitPool();

        // 저장된 설정 로드
        LoadVolumePrefs();
        ApplyVolumes();

#if PHOTON_UNITY_NETWORKING
        // PhotonView 확보 (있으면 사용, 없으면 추가)
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            photonView = gameObject.AddComponent<PhotonView>();
            photonView.ViewID = 0; // 로컬 전용 RPC 수신 용도로만 사용 (주의: Photon 설정에 따라 충돌 가능)
        }
#endif
    }

    private void InitPool()
    {
        if (oneShotSourcePrefab == null)
        {
            // 기본 프리팹이 없으면 임시로 만들어서 사용
            GameObject go = new GameObject("SFXSource_Prefab");
            oneShotSourcePrefab = go.AddComponent<AudioSource>();
            go.hideFlags = HideFlags.HideAndDontSave;
        }

        for (int i = 0; i < oneShotPoolSize; i++)
        {
            AudioSource src = Instantiate(oneShotSourcePrefab, transform);
            src.playOnAwake = false;
            sfxPool.Enqueue(src);
        }
    }

    #region BGM
    public void PlayBGM(AudioClip clip, float fadeDuration = 0.5f, bool loop = true)
    {
        if (clip == null) return;
        StopCoroutine("FadeBGM");
        StartCoroutine(FadeBGM(clip, fadeDuration, loop));
    }

    public void StopBGM(float fadeDuration = 0.5f)
    {
        StopCoroutine("FadeOutBGM");
        StartCoroutine(FadeOutBGM(fadeDuration));
    }

    private IEnumerator FadeBGM(AudioClip target, float duration, bool loop)
    {
        float startVol = bgmSource != null ? bgmSource.volume : 0f;
        float t = 0f;

        if (bgmSource == null)
            yield break;

        // 페이아웃
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(startVol, 0f, t / duration);
            yield return null;
        }

        bgmSource.clip = target;
        bgmSource.loop = loop;
        bgmSource.Play();

        // 페인
        t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(0f, bgmVolume * masterVolume, t / duration);
            yield return null;
        }

        bgmSource.volume = bgmVolume * masterVolume;
    }

    private IEnumerator FadeOutBGM(float duration)
    {
        if (bgmSource == null || !bgmSource.isPlaying) yield break;
        float start = bgmSource.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(start, 0f, t / duration);
            yield return null;
        }
        bgmSource.Stop();
        bgmSource.clip = null;
        bgmSource.volume = bgmVolume * masterVolume;
    }
    #endregion

    #region SFX
    /// <summary>
    /// 로컬에서 SFX 재생
    /// </summary>
    public AudioSource PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return null;
        if (sfxPool.Count == 0)
        {
            // 풀 확장
            AudioSource extra = Instantiate(oneShotSourcePrefab, transform);
            extra.playOnAwake = false;
            sfxPool.Enqueue(extra);
        }

        AudioSource src = sfxPool.Dequeue();
        src.clip = clip;
        src.volume = sfxVolume * masterVolume * volumeScale;
        src.loop = false;
        src.Play();
        StartCoroutine(ReleaseWhenDone(src, clip.length + 0.1f));
        return src;
    }

    private IEnumerator ReleaseWhenDone(AudioSource src, float wait)
    {
        yield return new WaitForSecondsRealtime(wait);
        src.Stop();
        src.clip = null;
        sfxPool.Enqueue(src);
    }

    /// <summary>
    /// 모든 클라이언트에서 동일한 SFX 재생 (Photon 사용 시)
    /// 모든 클라이언트가 해당 AudioClip을 Resources/Audio 폴더에 동일한 이름으로 가지고 있어야 합니다.
    /// </summary>
    public void PlaySFXGlobal(string resourcePath, float volumeScale = 1f)
    {
#if PHOTON_UNITY_NETWORKING
        if (photonView != null)
        {
            photonView.RPC(nameof(RPC_PlaySFX), RpcTarget.All, resourcePath, volumeScale);
        }
        else
        {
            // Photon이 없으면 로컬에서만 재생
            var clip = Resources.Load<AudioClip>(resourcePath);
            PlaySFX(clip, volumeScale);
        }
#else
        // Photon 사용하지 않으면 로컬에서 재생
        var clip = Resources.Load<AudioClip>(resourcePath);
        PlaySFX(clip, volumeScale);
#endif
    }

#if PHOTON_UNITY_NETWORKING
    [PunRPC]
    private void RPC_PlaySFX(string resourcePath, float volumeScale)
    {
        var clip = Resources.Load<AudioClip>(resourcePath);
        PlaySFX(clip, volumeScale);
    }
#endif

    #endregion

    #region Volume Control & Save
    public void SetMasterVolume(float v)
    {
        masterVolume = Mathf.Clamp01(v);
        ApplyVolumes();
        PlayerPrefs.SetFloat(PREF_MASTER, masterVolume);
    }

    public void SetBGMVolume(float v)
    {
        bgmVolume = Mathf.Clamp01(v);
        ApplyVolumes();
        PlayerPrefs.SetFloat(PREF_BGM, bgmVolume);
    }

    public void SetSFXVolume(float v)
    {
        sfxVolume = Mathf.Clamp01(v);
        ApplyVolumes();
        PlayerPrefs.SetFloat(PREF_SFX, sfxVolume);
    }

    private void LoadVolumePrefs()
    {
        masterVolume = PlayerPrefs.GetFloat(PREF_MASTER, masterVolume);
        bgmVolume = PlayerPrefs.GetFloat(PREF_BGM, bgmVolume);
        sfxVolume = PlayerPrefs.GetFloat(PREF_SFX, sfxVolume);
    }

    private void ApplyVolumes()
    {
        if (audioMixer != null)
        {
            // assumes exposed params named "Master", "BGM", "SFX" (in dB). We map linear 0..1 to -80..0 dB
            audioMixer.SetFloat("Master", LinearToDB(masterVolume));
            audioMixer.SetFloat("BGM", LinearToDB(bgmVolume));
            audioMixer.SetFloat("SFX", LinearToDB(sfxVolume));
        }
        else
        {
            if (bgmSource != null)
                bgmSource.volume = bgmVolume * masterVolume;
            foreach (var src in sfxPool)
                src.volume = sfxVolume * masterVolume;
        }
    }

    private float LinearToDB(float linear)
    {
        if (linear <= 0.0001f) return -80f;
        return Mathf.Log10(linear) * 20f;
    }
    #endregion

    #region Helpers
    public bool IsBGMPlaying() => bgmSource != null && bgmSource.isPlaying;
    #endregion
}
