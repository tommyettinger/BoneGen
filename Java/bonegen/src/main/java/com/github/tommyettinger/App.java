package com.github.tommyettinger;

public class App 
{
    public static void main( String[] args )
    {
        BoneGen bg = new BoneGen();
        char[][] dungeon = BoneGen.wallWrap(bg.generate(TilesetType.CAVES_LIMIT_CONNECTIVITY, 80, 80));
        for(int y = 0; y < dungeon.length; y++)
        {
            for(int x = 0; x < dungeon[0].length; x++)
            {
                System.out.print(dungeon[y][x]);
            }
            System.out.println();
        }
    }
}
