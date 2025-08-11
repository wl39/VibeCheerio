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
        public TileData(int x, int y, int value, TileTag tag = TileTag.Basic)
        { this.x = x; this.y = y; this.value = value; this.tag = tag; }
    }
}
