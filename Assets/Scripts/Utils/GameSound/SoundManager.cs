using Mecury.Common.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitedSolution;using UnityEngine;

namespace UnitedSolution
{
    public partial class SoundManager : SingletonBehaviour<SoundManager>
    {
        [Serializable]
        public class SfxPair
        {
            public string Name;
            public AudioSource Source;
        }

        public event Action<bool> OnSoundManager_SongMuteChanged;
        public event Action<bool> OnSoundManager_SfxMuteChanged;

        public event Action<float> OnSoundManager_SongVolumeChanged;
        public event Action<float> OnSoundManager_SfxVolumeChanged;

        public AudioSource SongAudioSource;
        public List<SfxPair> SfxAudioSources;
        public AudioClip[] Songs;
        public AudioClip[] SfxEffects;

        protected bool isSongMuted = false, isSfxMuted = false;
        public bool IsSongMuted
        {
            get
            {
                return isSongMuted;
            }
        }
        public bool IsSfxMuted
        {
            get
            {
                return isSfxMuted;
            }
        }

        protected bool isApplicationFocus = true;
        public string defaultBackgroundMusic;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
            if(string.IsNullOrEmpty(defaultBackgroundMusic))
            {
                PlaySong(defaultBackgroundMusic);
            }
        }


        /// <summary>
        /// Need call before calling Initialize
        /// </summary>
        [ContextMenu("Load all songs and effects")]
        public void Load()
        {
            Clean();
            Songs = Resources.LoadAll<AudioClip>("Audio/BackgroundMusic");
            SongAudioSource = gameObject.AddComponent<AudioSource>();
            SongAudioSource.loop = true;
            SongAudioSource.clip = Songs.FirstOrDefault();
            SongAudioSource.playOnAwake = true;

            SfxEffects = Resources.LoadAll<AudioClip>("Audio/EffectSounds");
            AudioSource audioSource = null;
            SfxAudioSources = new List<SfxPair>();
            for (int i = 0; i < SfxEffects.Length; i++)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.clip = SfxEffects[i];
                SfxAudioSources.Add(new SfxPair
                {
                    Name = SfxEffects[i].name,
                    Source = audioSource
                });
            }
        }
        public void Clean()
        {
            var audioSources = gameObject.GetComponents<AudioSource>();
            for (int index = 0; index < audioSources.Length; index++)
            {
                DestroyImmediate(audioSources[index]);
            }
            Array.Clear(audioSources, 0, audioSources.Length);
            audioSources = null;
        }

        protected virtual void Initialize()
        {
            float soundVolume = PlayerPrefs.GetFloat(SettingsKeyConstant.SOUND_VOLUME_PREF_KEY, 1f);
            SongAudioSource.volume = soundVolume;
            if (OnSoundManager_SongVolumeChanged != null)
            {
                OnSoundManager_SongVolumeChanged(soundVolume);
            }
            float sfxVolume = PlayerPrefs.GetFloat(SettingsKeyConstant.SFX_VOLUME_PREF_KEY, 1f);
            foreach (var pair in SfxAudioSources)
            {
                pair.Source.volume = sfxVolume;
            }
            if (OnSoundManager_SfxVolumeChanged != null)
            {
                OnSoundManager_SfxVolumeChanged(sfxVolume);
            }
        }

        public virtual void SetSongVolume(float volume)
        {
            PlayerPrefs.SetFloat(SettingsKeyConstant.SOUND_VOLUME_PREF_KEY, volume);
            PlayerPrefs.Save();
            SongAudioSource.volume = volume;
        }

        public virtual void SetSfxVolume(float volume)
        {
            PlayerPrefs.SetFloat(SettingsKeyConstant.SFX_VOLUME_PREF_KEY, volume);
            PlayerPrefs.Save();

            foreach (var pair in SfxAudioSources)
            {
                pair.Source.volume = volume;
            }
        }

        public virtual void PlaySong(string name)
        {
            var clip = Songs.FirstOrDefault(s => s.name.Equals(name));
            if (clip != null)
            {
                PlaySong(clip);
            }
        }

        public virtual void PlaySong(AudioClip clip)
        {
            SongAudioSource.clip = clip;
            SongAudioSource.Play();
        }

        public virtual void StopSong()
        {
            SongAudioSource.Stop();
        }

        /// <summary>
        /// Toggle mute song
        /// </summary>
        /// <param name="isMute"></param>
        /// <param name="willNotify">only set true if called inside SoundManager class</param>
        public virtual void ToggleMuteSong(bool isMute, bool willNotify = false)
        {
            isSongMuted = isMute;
            SongAudioSource.mute = isMute;

            if (willNotify && OnSoundManager_SongMuteChanged != null)
            {
                OnSoundManager_SongMuteChanged(isMute);
            }
        }

        public virtual void ToggleMuteSong()
        {
            ToggleMuteSong(!isSongMuted);
        }

        /// <summary>
        /// Toggle mute sfx
        /// </summary>
        /// <param name="isMute"></param>
        /// <param name="willNotify">only set true if called inside SoundManager class</param>
        public virtual void ToggleMuteSfx(bool isMute, bool willNotify = false)
        {
            isSfxMuted = isMute;
            foreach (var pair in SfxAudioSources)
            {
                pair.Source.mute = isMute;
            }

            if (willNotify && OnSoundManager_SfxMuteChanged != null)
            {
                OnSoundManager_SfxMuteChanged(isMute);
            }
        }

        public virtual void ToggleMuteSfx()
        {
            ToggleMuteSfx(!isSfxMuted);
        }

        public virtual void PlaySfx(string name, bool isLoop = false)
        {
            var pair = SfxAudioSources.FirstOrDefault(p => p.Name.Equals(name));
            if (pair != null)
            {
                pair.Source.Play();
                pair.Source.loop = isLoop;
            }
        }

        public virtual void PlaySfxInCoroutine(string name)
        {
            var pair = SfxAudioSources.FirstOrDefault(p => p.Name.Equals(name));
            if (pair != null && isApplicationFocus && !IsSfxMuted)
            {
                Run.Coroutine(PlaySfxInCoroutine(pair.Source), null);
            }
        }

        protected IEnumerator PlaySfxInCoroutine(AudioSource audioSource)
        {
            yield return null;

            var clip = audioSource.clip;
            var tmpSource = gameObject.AddComponent<AudioSource>();
            tmpSource.clip = clip;
            tmpSource.volume = audioSource.volume;
            tmpSource.Play();

            yield return new WaitForSeconds(clip.length);

            Destroy(tmpSource);
        }

        public virtual void StopSfx(string name)
        {
            var pair = SfxAudioSources.FirstOrDefault(p => p.Name.Equals(name));
            if (pair != null)
            {
                pair.Source.Stop();
                pair.Source.loop = false;
            }
        }

        public virtual void StopAllSfx()
        {
            foreach (var pair in SfxAudioSources)
            {
                pair.Source.Stop();
                pair.Source.loop = false;
            }
        }


        protected virtual void OnApplicationFocus(bool isFocus)
        {
            isApplicationFocus = isFocus;

            // Focus
            if (isFocus)
            {
                ToggleMuteSong(isSongMuted, true);

                ToggleMuteSfx(isSfxMuted, true);
            }
            // out of focus
            else
            {
                SongAudioSource.mute = true;

                foreach (var sfx in SfxAudioSources)
                {
                    sfx.Source.mute = true;
                }
            }
        }
    }
}
