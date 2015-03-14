/*******************
STB Herringbone Wang tile generator modified to output Lua.
*/

#include <stdlib.h>
#include <stdio.h>
#include <time.h>

#define STB_IMAGE_IMPLEMENTATION
#include "stb_image.h"        // http://nothings.org/stb_image.c

#define STB_IMAGE_WRITE_IMPLEMENTATION
#include "stb_image_write.h"  // http://nothings.org/stb/stb_image_write.h

#define STB_HERRINGBONE_WANG_TILE_IMPLEMENTATION
#include "stb_herringbone_wang_tile.h"

int submain(int argc, char **argv)
{
   unsigned char *data;
   int xs,ys, w,h;
   stbhw_tileset ts;

   if (argc != 4) {
      fprintf(stderr, "Usage: mapgen {tile-file} {xsize} {ysize}\n"
                      "generates file named 'test_map.png'\n");
      exit(1);
   }
   data = stbi_load(argv[1], &w, &h, NULL, 3);
   xs = atoi(argv[2]);
   ys = atoi(argv[3]);
   if (data == NULL) {
      fprintf(stderr, "Error opening or parsing '%s' as an image file\n", argv[1]);
      exit(1);
   }
   if (xs < 1 || xs > 1000) {
      fprintf(stderr, "xsize invalid or out of range\n");
      exit(1);
   }
   if (ys < 1 || ys > 1000) {
      fprintf(stderr, "ysize invalid or out of range\n");
      exit(1);
   }
   fprintf(writer, "return {\n");
   stbhw_build_tileset_from_image(&ts, data, w*3, w, h);
   free(data);
   unsigned char r = 0;
   unsigned char g = 0;
   unsigned char b = 0;
   fprintf(writer, "h_tiles = {");
   for (int i = 0; i < ts.num_h_tiles; i++)
   {
	   int idx = 0;
	   fprintf(writer, "{\n");
	   fprintf(writer, "a_constraint = %d,\n", (*ts.h_tiles[i]).a);
	   fprintf(writer, "b_constraint = %d,\n", (*ts.h_tiles[i]).b);
	   fprintf(writer, "c_constraint = %d,\n", (*ts.h_tiles[i]).c);
	   fprintf(writer, "d_constraint = %d,\n", (*ts.h_tiles[i]).d);
	   fprintf(writer, "e_constraint = %d,\n", (*ts.h_tiles[i]).e);
	   fprintf(writer, "f_constraint = %d,\n", (*ts.h_tiles[i]).f);
	   fprintf(writer, "data = {\n");
	   for (int y = 0; y < ts.short_side_len; y++)
	   {
		   fprintf(writer, "{");
		   for (int x = 0; x < ts.short_side_len * 2; x++)
		   {
			   r = (*ts.h_tiles[i]).pixels[idx++];
			   g = (*ts.h_tiles[i]).pixels[idx++];
			   b = (*ts.h_tiles[i]).pixels[idx++];
			   if (r == 255 && g == 255 && b == 255)
			   {
				   fprintf(writer, "'#',");
			   }
			   else
			   {
				   fprintf(writer, "'.',");
			   }
		   }
		   if (y == ts.short_side_len - 1)
			   fprintf(writer, "}}}\n");
		   else
			   fprintf(writer, "},\n");
	   }
	   if (i == ts.num_h_tiles - 1)
	   {
		   fprintf(writer, "},\n");
	   }
	   else
	   {
		   fprintf(writer, ",\n");
	   }
   }

   fprintf(writer, "v_tiles = {");
   for (int i = 0; i < ts.num_v_tiles; i++)
   {
	   int idx = 0;
	   fprintf(writer, "{\n");
	   fprintf(writer, "a_constraint = %d,\n", (*ts.v_tiles[i]).a);
	   fprintf(writer, "b_constraint = %d,\n", (*ts.v_tiles[i]).b);
	   fprintf(writer, "c_constraint = %d,\n", (*ts.v_tiles[i]).c);
	   fprintf(writer, "d_constraint = %d,\n", (*ts.v_tiles[i]).d);
	   fprintf(writer, "e_constraint = %d,\n", (*ts.v_tiles[i]).e);
	   fprintf(writer, "f_constraint = %d,\n", (*ts.v_tiles[i]).f);
	   fprintf(writer, "data = {\n");
	   for (int y = 0; y < ts.short_side_len * 2; y++)
	   {
		   fprintf(writer, "{");
		   for (int x = 0; x < ts.short_side_len; x++)
		   {
			   r = (*ts.v_tiles[i]).pixels[idx++];
			   g = (*ts.v_tiles[i]).pixels[idx++];
			   b = (*ts.v_tiles[i]).pixels[idx++];
			   if (r == 255 && g == 255 && b == 255)
			   {
				   fprintf(writer, "'#',");
			   }
			   else
			   {
				   fprintf(writer, "'.',");
			   }
		   }
		   if (y == ts.short_side_len * 2 - 1)
			   fprintf(writer, "}}}\n");
		   else
			   fprintf(writer, "},\n");
	   }
	   if (i == ts.num_v_tiles - 1)
	   {
		   fprintf(writer, "}\n");
	   }
	   else
	   {
		   fprintf(writer, ",\n");
	   }
   }

   fprintf(writer, "}\n");
   /*
   // allocate a buffer to create the final image to
   data = malloc(3 * xs * ys);
    
   srand(time(NULL));
   stbhw_generate_image(&ts, NULL, data, xs*3, xs, ys);

   stbi_write_png("test_map.png", xs, ys, 3, data, xs*3);

   stbhw_free_tileset(&ts);
   free(data);
   */
   return 0;
}

int main(int argc, char **argv)
{
	char* names[] = {
		"herringbone/template_caves_limit_connectivity.png",
		"herringbone/template_caves_tiny_corridors.png",
		"herringbone/template_corner_caves.png",
		"herringbone/template_horizontal_corridors_v1.png",
		"herringbone/template_horizontal_corridors_v2.png",
		"herringbone/template_horizontal_corridors_v3.png",
		"herringbone/template_limit_connectivity_fat.png",
		"herringbone/template_limited_connectivity.png",
		"herringbone/template_maze_2_wide.png",
		"herringbone/template_maze_plus_2_wide.png",
		"herringbone/template_open_areas.png",
		"herringbone/template_ref2_corner_caves.png",
		"herringbone/template_rooms_and_corridors.png",
		"herringbone/template_rooms_and_corridors_2_wide_diagonal_bias.png",
		"herringbone/template_rooms_limit_connectivity.png",
		"herringbone/template_round_rooms_diagonal_corridors.png",
		"herringbone/template_sean_dungeon.png",
		"herringbone/template_simple_caves_2_wide.png",
		"herringbone/template_square_rooms_with_random_rects.png" };
	char* write_names[] = {
		"caves_limit_connectivity.lua",
		"caves_tiny_corridors.lua",
		"corner_caves.lua",
		"horizontal_corridors_v1.lua",
		"horizontal_corridors_v2.lua",
		"horizontal_corridors_v3.lua",
		"limit_connectivity_fat.lua",
		"limited_connectivity.lua",
		"maze_2_wide.lua",
		"maze_plus_2_wide.lua",
		"open_areas.lua",
		"ref2_corner_caves.lua",
		"rooms_and_corridors.lua",
		"rooms_and_corridors_2_wide_diagonal_bias.lua",
		"rooms_limit_connectivity.lua",
		"round_rooms_diagonal_corridors.lua",
		"default_dungeon.lua",
		"simple_caves_2_wide.lua",
		"square_rooms_with_random_rects.lua"
	};
	for (int i = 0; i < 19; i++)
	{
		writer = fopen(write_names[i], "w");
		char* av[] = {"mapgen", names[i], "40", "40"};
		submain(4, av);
		fclose(writer);
	}
}