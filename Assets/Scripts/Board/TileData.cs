namespace Board
{
    public enum TileTag { Basic, Upgrade, Wild, Trap, Enemy }

    // Pure data holder
    public class TileData
    {
        public int x, y;
        public int value;      // For Basic/Upgrade tiles
        public int hp;         // For Enemy tiles
        public TileTag tag;

        public bool mergedThisStep; // Merge guard per step

        public int upgradeSpecId; // tag==Upgrade일 때 사용
        public int upgradeLevel;  // 1=승급, 2=승급+
        public int wildSpecId;    // tag==Wild일 때 사용
        public TileData(int x, int y, int value, TileTag tag = TileTag.Basic)
        { this.x = x; this.y = y; this.value = value; this.tag = tag; }
    }
}
