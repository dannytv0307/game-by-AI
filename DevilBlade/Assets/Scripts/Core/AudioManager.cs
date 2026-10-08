using System.Collections;
using UnityEngine;

namespace DevilBlade
{
    /// <summary>Phát nhạc (có chuyển bài mượt), SFX và lời thoại. Clip được gán bởi Level1Builder.</summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Music")]
        public AudioClip musicLevel;
        public AudioClip musicBoss;
        [Range(0, 1)] public float musicVolume = 0.55f;

        [Header("SFX")]
        public AudioClip slash, hit, jump, land, hurt, rageFull, transformBurst, enemyDie;
        [Range(0, 1)] public float sfxVolume = 0.8f;

        [Header("Voice")]
        public AudioClip voiceIntro, voiceTransform, voiceBossTaunt;
        [Range(0, 1)] public float voiceVolume = 1f;

        AudioSource _music, _sfx, _voice;
        Coroutine _fade;

        void Awake()
        {
            Instance = this;
            _music = gameObject.AddComponent<AudioSource>();
            _music.loop = true;
            _music.volume = musicVolume;
            _sfx = gameObject.AddComponent<AudioSource>();
            _voice = gameObject.AddComponent<AudioSource>();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void PlayMusic(AudioClip clip, float fade = 1f)
        {
            if (clip == null || _music.clip == clip) return;
            if (_fade != null) StopCoroutine(_fade);
            _fade = StartCoroutine(CrossFade(clip, fade));
        }

        IEnumerator CrossFade(AudioClip clip, float fade)
        {
            for (float t = 0; t < fade && _music.isPlaying; t += Time.unscaledDeltaTime)
            {
                _music.volume = Mathf.Lerp(musicVolume, 0, t / fade);
                yield return null;
            }
            _music.clip = clip;
            _music.Play();
            for (float t = 0; t < fade; t += Time.unscaledDeltaTime)
            {
                _music.volume = Mathf.Lerp(0, musicVolume, t / fade);
                yield return null;
            }
            _music.volume = musicVolume;
        }

        public void Sfx(AudioClip clip, float volume = 1f, float pitchJitter = 0.06f)
        {
            if (clip == null) return;
            _sfx.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
            _sfx.PlayOneShot(clip, sfxVolume * volume);
        }

        public void Voice(AudioClip clip)
        {
            if (clip == null) return;
            _voice.Stop();
            _voice.clip = clip;
            _voice.volume = voiceVolume;
            _voice.Play();
        }

        // Gọi an toàn khi chưa có AudioManager (vd. trong test)
        public static void PlaySfx(System.Func<AudioManager, AudioClip> pick, float volume = 1f)
        {
            if (Instance != null) Instance.Sfx(pick(Instance), volume);
        }
    }
}
