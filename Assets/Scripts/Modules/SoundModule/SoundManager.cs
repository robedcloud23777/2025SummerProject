using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class SoundManager : SingletonPun<SoundManager>
{
    // ȿ������ BGM�� ������ ��ųʸ� (�̸����� ����)
    private Dictionary<string, AudioClip> soundEffects;
    private List<(GameObject, AudioSource, int)> activeSounds;
    private Dictionary<string, AudioClip> bgmClips;
    private AudioSource bgmSource;
    private GameObject channelPrefab;
    private PhotonView pv;

    [Range(0f, 1f)]
    public float MasterVolume = 1.0f;  // ��ü ����
    [Range(0f, 1f)]
    public float SFXVolume = 1.0f;  // ȿ���� ����
    [Range(0f, 1f)]
    public float BGMVolume = 1.0f;  // BGM ����

    public override void Awake()
    {
        base.Awake();
        activeSounds = new();
        activeSounds.Clear();
        LoadAllSoundsFromResources();
        channelPrefab = Resources.Load("Prefabs/SoundChannel") as GameObject;
        bgmSource = transform.AddComponent<AudioSource>();
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
/*        PlaySound(null, "StartBGM", 1, int.MaxValue);*/
    }

    private void Update()
    {
        ManageActiveSound();
    }

    // Resources/Audio ������ ��� ����� ������ �ҷ��ͼ� ��ųʸ��� ����
    private void LoadAllSoundsFromResources()
    {
        soundEffects = new Dictionary<string, AudioClip>();
        bgmClips = new Dictionary<string, AudioClip>();

        // Resources/Audio ���� ���� ��� AudioClip�� �ҷ���
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds");

        foreach (var clip in clips)
        {
            // ���� �̸��� "BGM"�� ���Ե� ��� BGM���� ����
            if (clip.name.Contains("BGM"))
            {
                bgmClips[clip.name] = clip;
            }
            else
            {
                soundEffects[clip.name] = clip;  // ���� �̸��� Ű�� ��ųʸ��� ����
            }
        }

        Debug.Log("Loaded " + soundEffects.Count + " sound effects and " + bgmClips.Count + " BGM clips.");
    }

    // Ư�� ���带 ����ϴ� �޼��� (BGM�� SFX�� �����Ͽ� ó��)
    public void PlaySound(GameObject soundObject, string soundName, float volume, int repeatCount)
    {
        // BGM���� ȿ�������� ����
        if (bgmClips.ContainsKey(soundName))
        {
            // BGM ���
            PlayBGM(soundName, volume, repeatCount);
        }
        else if (soundEffects.ContainsKey(soundName))
        {
            // ȿ���� ���
            PlaySFX(soundObject, soundName, volume, repeatCount);
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found!");
        }
    }

    public void PlayPublicSound(GameObject soundObject, string soundName, float volume, int repeatCount)
    {
        PhotonView pv = soundObject.GetComponent<PhotonView>();
        if (!pv) return;

        this.pv.RPC("RPCPlaySound", RpcTarget.All, pv.ViewID, soundName, volume, repeatCount);
    }

    public void PlayPublicSoundExceptYou(GameObject soundObject, string soundName, float volume, int repeatCount)
    {
        PhotonView pv = soundObject.GetComponent<PhotonView>();
        if (!pv) return;

        this.pv.RPC("RPCPlaySound", RpcTarget.Others, pv.ViewID, soundName, volume, repeatCount);
    }

    [PunRPC]
    public void RPCPlaySound(int pvId, string soundName, float volume, int repeatCount)
    {
        PlaySound(PhotonView.Find(pvId).gameObject, soundName, volume, repeatCount);
    }

    // BGM ��� �޼���
    public void PlayBGM(string bgmName, float volume, int repeatCount)
    {
        if (bgmClips.TryGetValue(bgmName, out AudioClip clip))
        {
            bgmSource.clip = clip;
            bgmSource.volume = volume * BGMVolume * MasterVolume;
            bgmSource.loop = (repeatCount == 0);
            bgmSource.Play();
        }
    }

    // ȿ���� ��� �޼���
    private void PlaySFX(GameObject soundObject, string sfxName, float volume, int repeatCount)
    {
        if (soundEffects.TryGetValue(sfxName, out AudioClip clip))
        {
            var channel = GetAvaliableChannel();
            // ���ο� AudioSource�� �����Ͽ� ȿ���� ���
            channel.transform.SetParent(soundObject.transform, false);
            channel.clip = clip;
            channel.volume = volume * SFXVolume * MasterVolume;
            channel.loop = false;
            channel.spatialBlend = 1;
            channel.maxDistance = channel.volume * 30;
            channel.rolloffMode = AudioRolloffMode.Custom;
            channel.Play();

            // ��� ���� ���带 ��ųʸ��� �߰�
            activeSounds.Add((soundObject, channel, repeatCount));
        }
    }

    private AudioSource GetAvaliableChannel()
    {
        /*        AudioSource[] list = obj.GetComponents<AudioSource>();
                foreach (var i in list)
                {
                    if (!i.isPlaying) return i;
                }
                return obj.AddComponent<AudioSource>();*/
        AudioSource channel;
        for (int i = 0; i < transform.childCount; i++)
        {
            channel = transform.GetChild(i).GetComponent<AudioSource>();
            if (channel && !channel.gameObject.activeSelf)
            {
                channel.gameObject.SetActive(true);
                channel.transform.localPosition = Vector3.zero;
                return channel;
            }
        }

        channel = Instantiate(channelPrefab).GetComponent<AudioSource>();
        channel.transform.parent = transform;
        channel.transform.localPosition = Vector3.zero;
        channel.gameObject.SetActive(true);
        return channel;
    }

    public void UnableChannel(AudioSource source)
    {
        source.Stop();
        source.gameObject.SetActive(false);
        source.transform.parent = transform;
    }

    // Ư�� ���带 ���ߴ� �޼���
    public void StopSound(GameObject soundObject, string soundName)
    {
        if (bgmClips.ContainsKey(soundName) && bgmSource.clip != null && bgmSource.clip.name == soundName)
        {
            bgmSource.Stop();
            return;
        }

        var sounds = activeSounds.FindAll(tuple => tuple.Item1 == soundObject && tuple.Item2.clip.name == soundName);
        if (sounds.Count > 0)
        {
            foreach (var sound in sounds)
            {
                sound.Item2.Stop();
                UnableChannel(sound.Item2);
                activeSounds.Remove(sound);
            }
        }
    }

    public void ChangeVolume(GameObject soundObject, string soundName, float rate)
    {
        var sounds = activeSounds.FindAll(tuple => tuple.Item1 == soundObject && tuple.Item2.clip.name == soundName);
        if (sounds.Count > 0)
        {
            foreach (var sound in sounds)
            {
                sound.Item2.volume = rate;
            }
        }
    }

    List<(GameObject, AudioSource, int)> removeQueue = new();
    private void ManageActiveSound()
    {
        for (int i = 0; i < activeSounds.Count;)
        {
            if (activeSounds[i].Item2 == null)
            {
                activeSounds.RemoveAt(i);
                continue;
            }
            var sound = activeSounds[i];
            if (!sound.Item2.isPlaying)
            {
                if (sound.Item3 - 1 <= 0)
                {
                    UnableChannel(sound.Item2);
                    activeSounds.RemoveAt(i);
                    continue;
                }
                sound.Item2.Play();
                activeSounds[i] = (sound.Item1, sound.Item2, sound.Item3 - 1);
            }
            i++;
        }
        if (removeQueue.Count > 0) activeSounds.RemoveAll((tmp) => removeQueue.Contains(tmp));
        removeQueue.Clear();
    }

    /*    private IEnumerator PlaySoundRepeatedly(AudioSource source, AudioClip clip, float volume, int repeatCount)
        {
            for (int i = 0; i < repeatCount; i++)
            {
                source.PlayOneShot(clip, volume * SFXVolume * MasterVolume);
                yield return new WaitForSeconds(clip.length);
            }

            // �ݺ� ����� ������ AudioSource�� �����ϰ� ��ųʸ����� ����
            activeSounds.Remove((source.gameObject,));
            Destroy(source);
        }*/

    public void SetMasterVolume(float volume)
    {
        MasterVolume = volume;
        UpdateAllVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = volume;
        UpdateAllVolumes();
    }

    public void SetBGMVolume(float volume)
    {
        BGMVolume = volume;
        bgmSource.volume = BGMVolume * MasterVolume;
    }

    private void UpdateAllVolumes()
    {
        foreach (var source in activeSounds)
        {
            source.Item2.volume = SFXVolume * MasterVolume;  // ���� ��� AudioSource�� ������ SFXVolume�� MasterVolume���� ����
        }

        // BGM�� ������ ����
        bgmSource.volume = BGMVolume * MasterVolume;
    }

    // �����̴��� ����� �� ȣ��� �޼���
    public void OnMasterVolumeChanged(float value)
    {
        SetMasterVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        SetSFXVolume(value);
    }

    public void OnBGMVolumeChanged(float value)
    {
        SetBGMVolume(value);
    }
    /*    public bool DetectPlayer(Transform rayOrigin)
        {
            Ray ray = new Ray(rayOrigin.position, player.transform.position - transform.position);
            Debug.DrawRay(rayOrigin.position, player.transform.position - rayOrigin.position);
            RaycastHit[] hits = Physics.RaycastAll(ray, (player.transform.position - rayOrigin.position).magnitude, layerMask: LayerMask.GetMask("Wall"));
            if (hits.Length > 0) return false;
            return true;
        }*/
}
