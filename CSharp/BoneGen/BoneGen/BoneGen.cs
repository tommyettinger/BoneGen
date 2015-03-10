using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BoneGen
{
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
    public class Maxiumums
    {
        public int h, v;
        public Maxiumums()
        {
            h = 64;
            v = 64;
        }
    }
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
    public class Tileset
    {
        public Config config;
        public Maxiumums max_tiles;
        public Tile[] h_tiles, v_tiles;
        public Tileset()
        {
            config = new Config();
            max_tiles = new Maxiumums();
            h_tiles = new Tile[] { };
            v_tiles = new Tile[] { };
        }
    }
    public class BoneGen
    {
        //public BoneGen()
        public static void Main(string[] args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream[] jsonStreams = {
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.caves_limit_connectivity.js"),
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
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.default_dungeon.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.simple_caves_2_wide.js"),
                                       assembly.GetManifestResourceStream("BoneGen.herringbone.square_rooms_with_random_rects.js")
                                   };
            Tileset test = JsonConvert.DeserializeObject<Tileset>(new StreamReader(jsonStreams[16]).ReadToEnd());
            foreach (string s in test.h_tiles[0].data)
            {
                Console.WriteLine(s);
            }
            Console.Read();
        }
    }
}
