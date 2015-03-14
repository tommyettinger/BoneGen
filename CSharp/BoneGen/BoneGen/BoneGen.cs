using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace BoneGen
{
    /// <summary>
    /// Passed to BoneGen.Generate() to select a style of dungeon.
    /// </summary>
    public enum TilesetType
    {
        DEFAULT_DUNGEON = 0,
        CAVES_LIMIT_CONNECTIVITY,
        CAVES_TINY_CORRIDORS,
        CORNER_CAVES,
        HORIZONTAL_CORRIDORS_A,
        HORIZONTAL_CORRIDORS_B,
        HORIZONTAL_CORRIDORS_C,
        LIMIT_CONNECTIVITY_FAT,
        LIMITED_CONNECTIVITY,
        MAZE_A,
        MAZE_B,
        OPEN_AREAS,
        REFERENCE_CAVES,
        ROOMS_AND_CORRIDORS_A,
        ROOMS_AND_CORRIDORS_B,
        ROOMS_LIMIT_CONNECTIVITY,
        ROUND_ROOMS_DIAGONAL_CORRIDORS,
        SIMPLE_CAVES,
        SQUARE_ROOMS_WITH_RANDOM_RECTS
    }
    /// <summary>
    /// Part of the JSON that defines a tileset.
    /// </summary>
    public class Config
    {
        public bool is_corner;
        public int num_color_0, num_color_1, num_color_2, num_color_3, num_color_4 = 0, num_color_5 = 0;
        public int num_x_variants, num_y_variants, short_side_length;
        public Config()
        {
            is_corner = true;
            num_color_0 = 1;
            num_color_1 = 1;
            num_color_2 = 1;
            num_color_3 = 1;
            num_x_variants = 1;
            num_y_variants = 1;
        }
    }
    /// <summary>
    /// Part of the JSON that defines a tileset.
    /// </summary>
    public class Maximums
    {
        public int h, v;
        public Maximums()
        {
            h = 64;
            v = 64;
        }
    }
    /// <summary>
    /// Part of the JSON that defines a tileset.
    /// </summary>
    public class Tile
    {
        public int a_constraint, b_constraint, c_constraint, d_constraint, e_constraint, f_constraint;
        public string[] data;
        public Tile()
        {
            a_constraint = 0;
            b_constraint = 0;
            c_constraint = 0;
            d_constraint = 0;
            e_constraint = 0;
            f_constraint = 0;
            data = new string[] {};
        }
    }
    /// <summary>
    /// The outermost class in the JSON that defines a tileset.
    /// </summary>
    public class Tileset
    {
        public Config config;
        public Maximums max_tiles;
        public Tile[] h_tiles, v_tiles;
        public Tileset()
        {
            config = new Config();
            max_tiles = new Maximums();
            h_tiles = new Tile[] { };
            v_tiles = new Tile[] { };
        }
    }
    /// <summary>
    /// Contains methods for generating dungeons and caves
    /// with Sean T. Barrett's Herringbone Wang Tile dungeon
    /// generation algorithm.
    /// </summary>
    public class BoneGen
    {
        /// <summary>
        /// Can be set in the constructor or later; probably only relevant if you use a seeded RNG.
        /// </summary>
        public Random R;
        private Stream[] jsonStreams;
        private Tile ChooseTile(Tile[] list, int numlist, ref int[,] ccolor, int[] y_positions, int[] x_positions)
        {
            int a = ccolor[y_positions[0], x_positions[0]];
            int b = ccolor[y_positions[1], x_positions[1]];
            int c = ccolor[y_positions[2], x_positions[2]];
            int d = ccolor[y_positions[3], x_positions[3]];
            int e = ccolor[y_positions[4], x_positions[4]];
            int f = ccolor[y_positions[5], x_positions[5]];
            int i, n, match = int.MaxValue, pass;
            for (pass = 0; pass < 2; ++pass)
            {
                n = 0;
                // pass #1:
                //   count number of variants that match this partial set of constraints
                // pass #2:
                //   stop on randomly selected match
                for (i = 0; i < numlist; ++i)
                {
                    Tile tile = list[i];
                    if ((a < 0 || a == tile.a_constraint) &&
                        (b < 0 || b == tile.b_constraint) &&
                        (c < 0 || c == tile.c_constraint) &&
                        (d < 0 || d == tile.d_constraint) &&
                        (e < 0 || e == tile.e_constraint) &&
                        (f < 0 || f == tile.f_constraint))
                    {
                        n += 1;
                        if (n > match)
                        {
                            // use list[i]
                            // update constraints to reflect what we placed
                            ccolor[y_positions[0], x_positions[0]] = tile.a_constraint;
                            ccolor[y_positions[1], x_positions[1]] = tile.b_constraint;
                            ccolor[y_positions[2], x_positions[2]] = tile.c_constraint;
                            ccolor[y_positions[3], x_positions[3]] = tile.d_constraint;
                            ccolor[y_positions[4], x_positions[4]] = tile.e_constraint;
                            ccolor[y_positions[5], x_positions[5]] = tile.f_constraint;
                            return tile;
                        }
                    }
                }
                if (n == 0)
                {
                    throw new Exception("Could not find a matching tile");
                }
                match = R.Next(n);
            }
            throw new Exception("Could not find a matching tile");
        }
        private Tile ChooseTile(Tile[] list, int numlist, ref int[,] vcolor, ref int[,] hcolor, bool upright, int[] y_positions, int[] x_positions)
        {
            int a, b, c, d, e, f;
            if (upright)
            {
                a = hcolor[y_positions[0], x_positions[0]];
                b = vcolor[y_positions[1], x_positions[1]];
                c = vcolor[y_positions[2], x_positions[2]];
                d = vcolor[y_positions[3], x_positions[3]];
                e = vcolor[y_positions[4], x_positions[4]];
                f = hcolor[y_positions[5], x_positions[5]];
            }
            else
            {
                a = hcolor[y_positions[0], x_positions[0]];
                b = hcolor[y_positions[1], x_positions[1]];
                c = vcolor[y_positions[2], x_positions[2]];
                d = vcolor[y_positions[3], x_positions[3]];
                e = hcolor[y_positions[4], x_positions[4]];
                f = hcolor[y_positions[5], x_positions[5]];
            }
            int i, n, match = int.MaxValue, pass;
            for (pass = 0; pass < 2; ++pass)
            {
                n = 0;
                // pass #1:
                //   count number of variants that match this partial set of constraints
                // pass #2:
                //   stop on randomly selected match
                for (i = 0; i < numlist; ++i)
                {
                    Tile tile = list[i];
                    if ((a < 0 || a == tile.a_constraint) &&
                        (b < 0 || b == tile.b_constraint) &&
                        (c < 0 || c == tile.c_constraint) &&
                        (d < 0 || d == tile.d_constraint) &&
                        (e < 0 || e == tile.e_constraint) &&
                        (f < 0 || f == tile.f_constraint))
                    {
                        n += 1;
                        if (n > match)
                        {
                            // use list[i]
                            // update constraints to reflect what we placed
                            if (upright)
                            {
                                hcolor[y_positions[0], x_positions[0]] = tile.a_constraint;
                                vcolor[y_positions[1], x_positions[1]] = tile.b_constraint;
                                vcolor[y_positions[2], x_positions[2]] = tile.c_constraint;
                                vcolor[y_positions[3], x_positions[3]] = tile.d_constraint;
                                vcolor[y_positions[4], x_positions[4]] = tile.e_constraint;
                                hcolor[y_positions[5], x_positions[5]] = tile.f_constraint;
                            }
                            else
                            {
                                hcolor[y_positions[0], x_positions[0]] = tile.a_constraint;
                                hcolor[y_positions[1], x_positions[1]] = tile.b_constraint;
                                vcolor[y_positions[2], x_positions[2]] = tile.c_constraint;
                                vcolor[y_positions[3], x_positions[3]] = tile.d_constraint;
                                hcolor[y_positions[4], x_positions[4]] = tile.e_constraint;
                                hcolor[y_positions[5], x_positions[5]] = tile.f_constraint;
                            }
                            return tile;
                        }
                    }
                }
                if (n == 0)
                {
                    throw new Exception("Could not find a matching tile");
                }
                match = R.Next(n);
            }
            throw new Exception("Could not find a matching tile");
        }
        /// <summary>
        /// The main way of generating dungeons with BoneGen. Consider using BoneGen.WallWrap
        /// (a static method) to surround the edges with walls.
        /// </summary>
        /// <param name="tt">A TilesetType enum; try lots of these out to see how they look.</param>
        /// <param name="h">Height of the dungeon to generate in chars.</param>
        /// <param name="w">Width of the dungeon to generate in chars.</param>
        /// <returns>A row-major char[,] with h rows and w columns; it will be filled with '#' for walls and '.' for floors.</returns>
        public char[,] Generate(TilesetType tt, int h, int w)
        {
            Tileset ts = JsonConvert.DeserializeObject<Tileset>(new StreamReader(jsonStreams[(int)tt]).ReadToEnd());

            return Generate(ts, h, w);
        }
        /// <summary>
        /// Changes the outer edge of a char[,] to the wall char, '#'.
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static char[,] WallWrap(char[,] map)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                map[i, 0] = '#';
                map[i, map.GetUpperBound(1)] = '#';
            }
            for (int i = 0; i < map.GetLength(1); i++)
            {
                map[0, i] = '#';
                map[map.GetUpperBound(0), i] = '#';
            }
            return map;
        }

        private bool stbhw__match(int[,] ccolor, int y, int x)
        {
            return ccolor[y, x] == ccolor[y + 1, x + 1];
        }
        private int stbhw__change_color(int old_color, int num_options)
        {

            int offset = 1 + R.Next(num_options - 1);
            return (old_color + offset) % num_options;
        }
        /// <summary>
        /// If you have your own Tileset gained by parsing your own JSON, use
        /// this to generate a dungeon using it. Consider using BoneGen.WallWrap
        /// (a static method) to surround the edges with walls.
        /// </summary>
        /// <param name="ts">A Tileset; if you don't have one of these available, use a TilesetType enum instead to select a predefined one.</param>
        /// <param name="h">Height of the dungeon to generate in chars.</param>
        /// <param name="w">Width of the dungeon to generate in chars.</param>
        /// <returns>A row-major char[,] with h rows and w columns; it will be filled with '#' for walls and '.' for floors.</returns>
        public char[,] Generate(Tileset ts, int h, int w)
        {
            char[,] output = new char[h, w];
            int sidelen = ts.config.short_side_length;
            int xmax = (w / sidelen) + 6;
            int ymax = (h / sidelen) + 6;
            if (xmax > 1006)
            {
                throw new ArgumentOutOfRangeException("w", "Width too large!");
            }
            if (ymax > 1006)
            {
                throw new ArgumentOutOfRangeException("h", "Height too large!");
            }
            if (ts.config.is_corner)
            {

                int[,] c_color = new int[ymax, xmax];
                int i = 0, j = 0, ypos = -1 * sidelen;
                int[] cc = new int[] { ts.config.num_color_0, ts.config.num_color_1, ts.config.num_color_2, ts.config.num_color_3 };

                for (j = 0; j < ymax; ++j)
                {
                    for (i = 0; i < xmax; ++i)
                    {
                        int p = (i - j + 1) & 3; // corner type
                        c_color[j, i] = R.Next(cc[p]);
                    }
                }

               // Repetition reduction
               // now go back through and make sure we don't have adjacent 3x2 vertices that are identical,
               // to avoid really obvious repetition (which happens easily with extreme weights)
               for (j=0; j < ymax-3; ++j) {
                  for (i=0; i < xmax-3; ++i) {
                      int p = (i - j + 1) & 3; // corner type
                      if (i + 3 >= 1006) { throw new Exception("Internal failure (on x) trying to reduce repetition"); };
                      if (j + 3 >= 1006) { throw new Exception("Internal failure (on y) trying to reduce repetition"); };
                      if (stbhw__match(c_color, j, i) && stbhw__match(c_color, j + 1, i) && stbhw__match(c_color, j + 2, i)
                         && stbhw__match(c_color, j, i + 1) && stbhw__match(c_color, j+1, i+1) && stbhw__match(c_color, j+2, i+1)){
                        p = ((i+1)-(j+1)+1) & 3;
                        if (cc[p] > 1)
                            c_color[j + 1, i + 1] = stbhw__change_color(c_color[j + 1, i + 1], cc[p]);
                     }
//                     if (stbhw__match(i,j) && stbhw__match(i+1,j) && stbhw__match(i+2,j)
//                         && stbhw__match(i,j+1) && stbhw__match(i+1,j+1) && stbhw__match(i+2,j+1)) {

                      if (stbhw__match(c_color, j, i) && stbhw__match(c_color, j, i+1) && stbhw__match(c_color, j, i+2)
                         && stbhw__match(c_color, j+1, i) && stbhw__match(c_color, j + 1, i + 1) && stbhw__match(c_color, j + 1, i + 2))
                      {
                        p = ((i+2)-(j+1)+1) & 3;
                        if (cc[p] > 1)
                           c_color[j+1,i+2] = stbhw__change_color(c_color[j+1,i+2], cc[p]);
                     }
                  }
               }
                 

                for (j = -1; ypos < h; ++j)
                {
                    // a general herringbone row consists of:
                    //    horizontal left block, the bottom of a previous vertical, the top of a new vertical
                    int phase = (j & 3);
                    // displace horizontally according to pattern
                    if (phase == 0)
                    {
                        i = 0;
                    }
                    else
                    {
                        i = phase - 4;
                    }
                    for (; ; i += 4)
                    {
                        int xpos = i * sidelen;
                        if (xpos >= w)
                            break;
                        // horizontal left-block
                        if (xpos + sidelen * 2 >= 0 && ypos >= 0)
                        {
                            Tile t = ChooseTile(
                               ts.h_tiles, ts.h_tiles.Length, ref c_color,
                               new int[] { j + 2, j + 2, j + 2, j + 3, j + 3, j + 3 },
                               new int[] { i + 2, i + 3, i + 4, i + 2, i + 3, i + 4 });

                            if (t == null)
                                throw new Exception("Failed to generate a map.");

                            output.Insert(t.data, ypos, xpos);
                        }
                        xpos += sidelen * 2;
                        // now we're at the end of a previous vertical one
                        xpos += sidelen;
                        // now we're at the start of a new vertical one
                        if (xpos < w)
                        {
                            Tile t = ChooseTile(
                              ts.v_tiles, ts.v_tiles.Length, ref c_color,
                              new int[] { j + 2, j + 3, j + 4, j + 2, j + 3, j + 4 },
                              new int[] { i + 5, i + 5, i + 5, i + 6, i + 6, i + 6 });

                            if (t == null)
                                throw new Exception("Failed to generate a map.");
                            output.Insert(t.data, ypos, xpos);
                        }
                    }
                    ypos += sidelen;
                }
            }
            else
            {
                // @TODO edge-color repetition reduction
                int i = 0, j = -1, ypos;
                int[,] v_color = new int[ymax, xmax];
                int[,] h_color = new int[ymax, xmax];
                for (int yy = 0; yy < ymax; yy++)
                {
                    for (int xx = 0; xx < xmax; xx++)
                    {
                        v_color[yy, xx] = -1;
                        h_color[yy, xx] = -1;
                    }
                }

                ypos = -1 * sidelen;
                for (j = -1; ypos < h; ++j)
                {
                    // a general herringbone row consists of:
                    //    horizontal left block, the bottom of a previous vertical, the top of a new vertical
                    int phase = (j & 3);
                    // displace horizontally according to pattern
                    if (phase == 0)
                    {
                        i = 0;
                    }
                    else
                    {
                        i = phase - 4;
                    }
                    for (; ; i += 4)
                    {
                        int xpos = i * sidelen;
                        if (xpos >= w)
                            break;
                        // horizontal left-block
                        if (xpos + sidelen * 2 >= 0 && ypos >= 0)
                        {
                            Tile t = ChooseTile(
                               ts.h_tiles, ts.h_tiles.Length, ref v_color, ref h_color, false,
                               new int[] { j + 2, j + 2, j + 2, j + 2, j + 3, j + 3 },
                               new int[] { i + 2, i + 3, i + 2, i + 4, i + 2, i + 3 });

                            if (t == null)
                                throw new Exception("Failed to generate a map.");
                            output.Insert(t.data, ypos, xpos);
                        }
                        xpos += sidelen * 2;
                        // now we're at the end of a previous vertical one
                        xpos += sidelen;
                        // now we're at the start of a new vertical one
                        if (xpos < w)
                        {
                            Tile t = ChooseTile(
                               ts.v_tiles, ts.v_tiles.Length, ref v_color, ref h_color, true,
                               new int[] { j + 2, j + 2, j + 2, j + 3, j + 3, j + 4 },
                               new int[] { i + 5, i + 5, i + 6, i + 5, i + 6, i + 5 });

                            if (t == null)
                                throw new Exception("Failed to generate a map.");
                            output.Insert(t.data, ypos, xpos);
                        }
                    }
                    ypos += sidelen;
                }
            }
            return output;
        }
        /// <summary>
        /// Constructs a BoneGen that uses the default RNG.
        /// </summary>
        public BoneGen() : this(new Random())
        {
        }

        /// <summary>
        /// Constructs a BoneGen with the given RNG.
        /// </summary>
        /// <param name="r"></param>
        public BoneGen(Random r)
        {
            R = r;
            Assembly assembly = Assembly.GetExecutingAssembly();
            jsonStreams = new Stream[] {
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.default_dungeon.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.caves_limit_connectivity.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.caves_tiny_corridors.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.corner_caves.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.horizontal_corridors_v1.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.horizontal_corridors_v2.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.horizontal_corridors_v3.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.limit_connectivity_fat.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.limited_connectivity.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.maze_2_wide.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.maze_plus_2_wide.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.open_areas.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.ref2_corner_caves.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.rooms_and_corridors.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.rooms_and_corridors_2_wide_diagonal_bias.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.rooms_limit_connectivity.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.round_rooms_diagonal_corridors.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.simple_caves_2_wide.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.square_rooms_with_random_rects.js")
                                   };
            
        }
        /*
        public static void Main(string[] args)
        {
            BoneGen bg = new BoneGen();
            char[,] dungeon = WallWrap(bg.Generate(TilesetType.DEFAULT_DUNGEON, 80, 80));
            for (int y = 0; y < dungeon.GetLength(0); y++)
            {
                for (int x = 0; x < dungeon.GetLength(1); x++)
                {
                    Console.Write(dungeon[y, x]);
                }
                //                Console.WriteLine();
            }
        }
         */
    }
}
