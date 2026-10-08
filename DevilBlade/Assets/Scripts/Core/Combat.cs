using System.Collections.Generic;
using UnityEngine;

namespace DevilBlade
{
    public static class Combat
    {
        static readonly List<Collider2D> Buffer = new();
        static readonly HashSet<Health> Seen = new();

        /// <summary>Gây sát thương cho mọi Health trong hộp (mỗi đối tượng tối đa 1 lần). Trả về số mục tiêu trúng.</summary>
        public static int HitBox(Vector2 center, Vector2 size, int mask, int damage, float direction, GameObject self)
        {
            var filter = new ContactFilter2D { useLayerMask = true, layerMask = mask, useTriggers = true };
            Physics2D.OverlapBox(center, size, 0f, filter, Buffer);
            Seen.Clear();
            var hits = 0;
            for (var i = 0; i < Buffer.Count; i++)
            {
                var h = Buffer[i].GetComponentInParent<Health>();
                if (h == null || h.gameObject == self || !Seen.Add(h)) continue;
                if (h.TakeDamage(damage, direction)) hits++;
            }
            return hits;
        }
    }
}
