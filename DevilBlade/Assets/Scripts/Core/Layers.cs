namespace DevilBlade
{
    /// <summary>Layer vật lý dùng trong game (tên được đặt trong TagManager bởi Level1Builder).</summary>
    public static class Layers
    {
        public const int Ground = 6;
        public const int Player = 7;
        public const int Enemy = 8;

        public const int GroundMask = 1 << Ground;
        public const int PlayerMask = 1 << Player;
        public const int EnemyMask = 1 << Enemy;
    }
}
