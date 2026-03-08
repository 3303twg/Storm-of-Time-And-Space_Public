using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Header("BGM Settings")] public AudioSource bgmAudioSource;
    public AudioClip[] bgmClips;
    public AudioClip bossBGMClip;

    [Header("SFX Settings")] private List<AudioSource> sfxPool = new List<AudioSource>();
    private List<AudioSource> loopableSfxPool = new List<AudioSource>();
    private Dictionary<string, AudioSource> activeLoopingSfx = new Dictionary<string, AudioSource>();

    [Header("SFX Clips")] public AudioClip wallCollisionSFX;
    public AudioClip bulletHitMonsterSFX;
    public AudioClip monsterDeathSFX;
    public AudioClip uiClickSFX;
    public AudioClip playerJumpSFX;
    public AudioClip playerHitSFX;
    public AudioClip playerDeathSFX;
    public AudioClip itemPickupSFX;
    public AudioClip repeatingEngineSoundClip;
    public AudioClip repeatingBeamSoundClip;

    private Dictionary<string, AudioClip> sfxClipDictionary = new Dictionary<string, AudioClip>();

    private float _sfxGlobalVolume;

    public static SoundManager _instance { get; private set; }

    private void Start()
    {
        sfxClipDictionary.Add("WallCollision", wallCollisionSFX);
        sfxClipDictionary.Add("BulletHitMonster", bulletHitMonsterSFX);
        sfxClipDictionary.Add("MonsterDeath", monsterDeathSFX);
        sfxClipDictionary.Add("UIClick", uiClickSFX);
        sfxClipDictionary.Add("PlayerJump", playerJumpSFX);
        sfxClipDictionary.Add("PlayerHit", playerHitSFX);
        sfxClipDictionary.Add("PlayerDeath", playerDeathSFX);
        sfxClipDictionary.Add("ItemPickup", itemPickupSFX);
        sfxClipDictionary.Add("RepeatingEngineSound", repeatingEngineSoundClip);
        sfxClipDictionary.Add("RepeatingBeamSound", repeatingBeamSoundClip);
        PlayDefaultBGM(); // 기본 BGM 재생
    }

    public void PlayDefaultBGM()
    {
        if (bgmAudioSource != null && bgmClips.Length > 0 && bgmClips[0] != null)
        {
            if (bgmAudioSource.clip != bgmClips[0] || !bgmAudioSource.isPlaying)
            {
                bgmAudioSource.clip = bgmClips[0]; // 첫 번째 BGM을 기본으로 사용
                bgmAudioSource.loop = true; // BGM 반복 재생
                bgmAudioSource.Play();
            }
        }
    }

    public void PlaySpecificBGM(AudioClip clipToPlay)
    {
        if (bgmAudioSource != null && clipToPlay != null)
        {
            if (bgmAudioSource.clip != clipToPlay || !bgmAudioSource.isPlaying)
            {
                bgmAudioSource.clip = clipToPlay;
                bgmAudioSource.loop = true;
                bgmAudioSource.Play();
            }
        }
    }
    
    
    public void PlayBossBGM()
    {
        if (PhotonNetwork.IsConnectedAndReady && photonView != null) 
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RPC_PlayBossBGM", RpcTarget.All);
            }
        }
        else
        {
            PlaySpecificBGM(bossBGMClip);
        }
    }
    
    [PunRPC]
    private void RPC_PlayBossBGM()
    {
        if (bossBGMClip != null)
        {
            PlaySpecificBGM(bossBGMClip);
        }
    }

    public void PlaySFX(string sfxName, Vector3 position = default, float volume = 1.0f)
    {
        InternalPlaySFX(sfxName, position, volume);

        if (PhotonNetwork.IsMasterClient || PhotonNetwork.IsConnectedAndReady)
        {
            photonView.RPC("RPC_PlaySFX", RpcTarget.Others, sfxName, position.x, position.y, position.z, volume);
        }
    }

    public void StartRepeatingSFX(string sfxName, Vector3 position = default, float volume = 1.0f)
    {
        AudioClip clipToPlay = GetSfxClip(sfxName);

        if (clipToPlay == null)
        {
            return;
        }

        if (activeLoopingSfx.ContainsKey(sfxName))
        {
            return;
        }

        AudioSource sfxSource = GetPooledLoopableSFXSource();
        if (sfxSource == null)
        {
            return;
        }

        sfxSource.clip = clipToPlay;
        sfxSource.volume = _sfxGlobalVolume * volume;
        sfxSource.loop = true;
        sfxSource.transform.position = position;
        sfxSource.spatialBlend = (position == default(Vector3) ? 0.0f : 1.0f);

        sfxSource.gameObject.SetActive(true);
        sfxSource.Play();
        activeLoopingSfx.Add(sfxName, sfxSource);

        if (PhotonNetwork.IsMasterClient || PhotonNetwork.IsConnectedAndReady)
        {
            photonView.RPC("RPC_StartRepeatingSFX", RpcTarget.Others, sfxName, position.x, position.y, position.z,
                volume);
        }
    }

    public void StopRepeatingSFX(string sfxName)
    {
        if (activeLoopingSfx.TryGetValue(sfxName, out AudioSource sfxSource))
        {
            sfxSource.Stop();
            sfxSource.gameObject.SetActive(false);
            activeLoopingSfx.Remove(sfxName);
        }

        if (PhotonNetwork.IsMasterClient || PhotonNetwork.IsConnectedAndReady)
        {
            photonView.RPC("RPC_StopRepeatingSFX", RpcTarget.Others, sfxName);
        }
    }

    public void StopAllRepeatingSFX()
    {
        foreach (var entry in activeLoopingSfx.ToList())
        {
            entry.Value.Stop();
            entry.Value.gameObject.SetActive(false);
            activeLoopingSfx.Remove(entry.Key);
        }

        if (PhotonNetwork.IsMasterClient || PhotonNetwork.IsConnectedAndReady)
        {
            photonView.RPC("RPC_StopAllRepeatingSFX", RpcTarget.Others);
        }
    }

    [PunRPC]
    void RPC_StartRepeatingSFX(string sfxName, float x, float y, float z, float volume)
    {
        Vector3 position = new Vector3(x, y, z);
        AudioClip clipToPlay = GetSfxClip(sfxName);

        if (clipToPlay == null)
        {
            return;
        }

        if (activeLoopingSfx.ContainsKey(sfxName))
        {
            return;
        }

        AudioSource sfxSource = GetPooledLoopableSFXSource();
        if (sfxSource == null)
        {
            return;
        }

        sfxSource.clip = clipToPlay;
        sfxSource.volume = _sfxGlobalVolume * volume;
        sfxSource.loop = true;
        sfxSource.transform.position = position;
        sfxSource.spatialBlend = (position == default(Vector3) ? 0.0f : 1.0f);

        sfxSource.gameObject.SetActive(true);
        sfxSource.Play();
        activeLoopingSfx.Add(sfxName, sfxSource);
    }

    [PunRPC]
    void RPC_StopRepeatingSFX(string sfxName)
    {
        if (activeLoopingSfx.TryGetValue(sfxName, out AudioSource sfxSource))
        {
            sfxSource.Stop();
            sfxSource.gameObject.SetActive(false);
            activeLoopingSfx.Remove(sfxName);
        }
    }

    [PunRPC]
    void RPC_StopAllRepeatingSFX()
    {
        StopAllRepeatingSFX();
    }

    private AudioClip GetSfxClip(string sfxName)
    {
        if (sfxClipDictionary.TryGetValue(sfxName, out AudioClip clip))
        {
            return clip;
        }

        return null;
    }

    private AudioSource GetPooledSFXSource()
    {
        foreach (AudioSource source in sfxPool)
        {
            if (!source.gameObject.activeInHierarchy)
            {
                return source;
            }
        }

        GameObject go = new GameObject("SFXAudioSource_OneShot");
        go.transform.SetParent(this.transform);
        AudioSource newSource = go.AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        newSource.volume = _sfxGlobalVolume;
        newSource.spatialBlend = 0.0f;
        sfxPool.Add(newSource);
        return newSource;
    }

    private AudioSource GetPooledLoopableSFXSource()
    {
        foreach (AudioSource source in loopableSfxPool)
        {
            if (!source.gameObject.activeInHierarchy || !source.isPlaying)
            {
                return source;
            }
        }

        GameObject go = new GameObject("SFXAudioSource_Loopable");
        go.transform.SetParent(this.transform);
        AudioSource newSource = go.AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        newSource.volume = _sfxGlobalVolume;
        newSource.loop = true;
        newSource.spatialBlend = 0.0f;
        loopableSfxPool.Add(newSource);
        return newSource;
    }

    private void InternalPlaySFX(string sfxName, Vector3 position, float volume)
    {
        if (sfxClipDictionary.TryGetValue(sfxName, out AudioClip clip))
        {
            AudioSource sfxSource = GetPooledSFXSource();
            if (sfxSource != null)
            {
                sfxSource.clip = clip;
                sfxSource.volume = _sfxGlobalVolume;

                if (position != default(Vector3))
                {
                    sfxSource.transform.position = position;
                    sfxSource.spatialBlend = 1.0f;
                    sfxSource.rolloffMode = AudioRolloffMode.Linear;
                    sfxSource.maxDistance = 50f;
                }
                else
                {
                    sfxSource.spatialBlend = 0.0f;
                }

                sfxSource.gameObject.SetActive(true);
                sfxSource.Play();

                StartCoroutine(DeactivateSFXSourceAfterPlay(sfxSource, clip.length));
            }
        }
    }

    [PunRPC]
    private void RPC_PlaySFX(string sfxName, float posX, float posY, float posZ, float volume)
    {
        Vector3 position = new Vector3(posX, posY, posZ);
        InternalPlaySFX(sfxName, position, volume);
    }

    private System.Collections.IEnumerator DeactivateSFXSourceAfterPlay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.gameObject.SetActive(false);
    }

    public void SetBGMVolume(float volume)
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = Mathf.Clamp01(volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        _sfxGlobalVolume = Mathf.Clamp01(volume);
        foreach (AudioSource source in sfxPool)
        {
            if (source.gameObject.activeInHierarchy && source.clip != null)
            {
                source.volume = _sfxGlobalVolume;
            }
        }

        foreach (var entry in activeLoopingSfx.Values)
        {
            if (entry != null && entry.clip != null)
            {
                entry.volume = _sfxGlobalVolume * GetSfxClipIndividualVolume(entry.clip.name);
            }
        }

        foreach (AudioSource source in loopableSfxPool)
        {
            if (source != null)
            {
                source.volume = _sfxGlobalVolume;
            }
        }
    }

    private float GetSfxClipIndividualVolume(string sfxName)
    {
        return 1.0f;
    }
}