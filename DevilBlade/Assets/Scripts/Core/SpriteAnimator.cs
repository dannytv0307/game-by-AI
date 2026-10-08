using System;
using System.Collections.Generic;
using UnityEngine;

namespace DevilBlade
{
    /// <summary>
    /// Animator theo frame đơn giản cho sprite sheet do asset-pipeline sinh ra.
    /// Tránh phải dựng AnimatorController; logic gameplay đọc <see cref="Frame"/> để căn thời điểm ra đòn.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimator : MonoBehaviour
    {
        [Serializable]
        public class Clip
        {
            public string name;
            public Sprite[] frames;
            public float fps = 10f;
            public bool loop = true;
        }

        public List<Clip> clips = new();

        SpriteRenderer _renderer;
        Clip _current;
        float _time;

        public string Current => _current?.name;
        public int Frame { get; private set; }
        public bool Finished { get; private set; }

        void Awake() => _renderer = GetComponent<SpriteRenderer>();

        public bool Has(string clipName) => clips.Exists(c => c.name == clipName);

        public void Play(string clipName, bool restart = false)
        {
            if (!restart && _current != null && _current.name == clipName) return;
            var clip = clips.Find(c => c.name == clipName);
            if (clip == null || clip.frames == null || clip.frames.Length == 0)
            {
                Debug.LogWarning($"[SpriteAnimator] {name}: thiếu clip '{clipName}'");
                return;
            }
            _current = clip;
            _time = 0f;
            Frame = 0;
            Finished = false;
            _renderer.sprite = clip.frames[0];
        }

        /// <summary>Độ dài clip (giây) — dùng để đồng bộ trạng thái như biến hình.</summary>
        public float Duration(string clipName)
        {
            var clip = clips.Find(c => c.name == clipName);
            return clip == null ? 0f : clip.frames.Length / clip.fps;
        }

        void Update()
        {
            if (_current == null || Finished) return;
            _time += Time.deltaTime;
            var index = Mathf.FloorToInt(_time * _current.fps);
            if (index >= _current.frames.Length)
            {
                if (_current.loop)
                {
                    _time %= _current.frames.Length / _current.fps;
                    index = Mathf.FloorToInt(_time * _current.fps) % _current.frames.Length;
                }
                else
                {
                    index = _current.frames.Length - 1;
                    Finished = true;
                }
            }
            if (index != Frame || _renderer.sprite != _current.frames[index])
            {
                Frame = index;
                _renderer.sprite = _current.frames[index];
            }
        }
    }
}
