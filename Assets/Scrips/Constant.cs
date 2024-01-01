
using JetBrains.Annotations;

namespace CONST
{
    /// <summary>
    /// ゲーム開始初期値
    /// </summary>
    public static class INITIALIZE 
    {
        /// <summary>
        /// 初期HP
        /// </summary>
        public static int PLAYER_HP = 20;
        /// <summary>
        /// 初期コスト（基礎）
        /// </summary>
        public static int BASE_MEMORY = 1;
        /// <summary>
        /// 初期コスト（未使用）
        /// </summary>
        public static int HAVE_MEMORY = 0;
        /// <summary>
        /// 初期カード枚数（先攻）
        /// </summary>
        public static int FIRST_CRAD_COUNT = 3;
        /// <summary>
        /// 初期カード枚数（後攻）
        /// </summary>
        public static int SECOND_CRAD_COUNT = 4;
    }

    public static class MAX_MIN
    {
        public static int MAX_COST = 8;        
        public static int MIN_COST = 0;
    }
}