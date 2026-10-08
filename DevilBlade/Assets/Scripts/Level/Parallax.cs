using UnityEngine;

namespace DevilBlade
{
    /// <summary>
    /// Lớp nền parallax lặp vô hạn theo trục X. Các bản sao (con) nằm cạnh nhau với độ rộng <see cref="width"/>.
    /// factor = 1 => đứng yên so với camera (xa vô tận); 0 => di chuyển như thế giới.
    /// </summary>
    public class Parallax : MonoBehaviour
    {
        [Range(0, 1)] public float factorX = 0.85f;
        [Range(0, 1)] public float factorY = 0.9f;
        public float width = 20f;
        public float baseY;

        Transform _cam;

        void Start()
        {
            if (Camera.main != null) _cam = Camera.main.transform;
        }

        void LateUpdate()
        {
            if (_cam == null) return;
            var cx = _cam.position.x;
            var x = cx * factorX;
            // dời gốc theo bội số width để luôn có bản sao phủ kín màn hình
            var offset = Mathf.Round((cx - x) / width) * width;
            transform.position = new Vector3(x + offset, baseY + (_cam.position.y - baseY) * factorY, transform.position.z);
        }
    }
}
