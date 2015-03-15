--[[ -- Example usage
local donjon = wall_wrap(generate("default_dungeon", 60, 60, os.time()))
show2d(donjon)
--]]

bit = require("bit")

tileset_types = {
	default_dungeon = dofile("styles/default_dungeon.lua"),
	caves_limit_connectivity = dofile("styles/caves_limit_connectivity.lua"),
	caves_tiny_corridors = dofile("styles/caves_tiny_corridors.lua"),
	corner_caves = dofile("styles/corner_caves.lua"),
	horizontal_corridors_v1 = dofile("styles/horizontal_corridors_v1.lua"),
	horizontal_corridors_v2 = dofile("styles/horizontal_corridors_v2.lua"),
	horizontal_corridors_v3 = dofile("styles/horizontal_corridors_v3.lua"),
	limit_connectivity_fat = dofile("styles/limit_connectivity_fat.lua"),
	limited_connectivity = dofile("styles/limited_connectivity.lua"),
	maze_2_wide = dofile("styles/maze_2_wide.lua"),
	maze_plus_2_wide = dofile("styles/maze_plus_2_wide.lua"),
	open_areas = dofile("styles/open_areas.lua"),
	ref2_corner_caves = dofile("styles/ref2_corner_caves.lua"),
	rooms_and_corridors = dofile("styles/rooms_and_corridors.lua"),
	rooms_and_corridors_2_wide_diagonal_bias = dofile("styles/rooms_and_corridors_2_wide_diagonal_bias.lua"),
	rooms_limit_connectivity = dofile("styles/rooms_limit_connectivity.lua"),
	round_rooms_diagonal_corridors = dofile("styles/round_rooms_diagonal_corridors.lua"),
	simple_caves_2_wide = dofile("styles/simple_caves_2_wide.lua"),
	square_rooms_with_random_rects = dofile("styles/square_rooms_with_random_rects.lua")
}

function init2d(val, y, x)
        local ret = {}
        for j=1,y do
                ret[j] = {}
                for i=1,x do
                        ret[j][i] = val
                end
        end
        return ret
end

function show2d(matrix)
        for i,row in ipairs(matrix) do print(table.concat(row)) end
end

function wall_wrap(map)                                                                                              
        local upperY = #map
        local upperX = #(map[1])
        for i = 1, upperY
        do
            map[i][1] = '#'
            map[i][upperX] = '#'
        end
        for i = 2, upperX - 1
        do
            map[1][i] = '#'
            map[upperX][i] = '#'
        end
        return map
end

local function insert(matrix, items, y, x)
        local lim_y, lim_x = #matrix, #(matrix[1])
	for iy,row in ipairs(items) do
	        for ix,v in ipairs(row) do
	                if not (iy + y < 2 or ix + x < 2 or iy + y > lim_y or ix + x > lim_x) then
	                        matrix[iy + y - 1][ix + x - 1] = v
	                end
	        end
	end
	return matrix
end

local c_color, h_color, v_color = {{}}, {{}}, {{}} -- init2d(-1, 10, 10)

local function choose_tile_c(tiles, y_positions, x_positions)
--    private Tile chooseTile(Tile[] list, int numlist, int[] y_positions, int[] x_positions)
	local a = c_color[y_positions[1]][x_positions[1]]
	local b = c_color[y_positions[2]][x_positions[2]]
	local c = c_color[y_positions[3]][x_positions[3]]
	local d = c_color[y_positions[4]][x_positions[4]]
	local e = c_color[y_positions[5]][x_positions[5]]
	local f = c_color[y_positions[6]][x_positions[6]]
	local n = 0
	local match = math.huge
	for pass=1,2 do
	    n = 0
	    -- pass #1:
	    --	 count number of variants that match this partial set of constraints
	    -- pass #2:
	    --	 stop on randomly selected match
	    for i=1, #tiles do
		local tile = tiles[i]
		if
		    (a < 0 or a == tile.a_constraint) and
		    (b < 0 or b == tile.b_constraint) and
		    (c < 0 or c == tile.c_constraint) and
		    (d < 0 or d == tile.d_constraint) and
		    (e < 0 or e == tile.e_constraint) and
		    (f < 0 or f == tile.f_constraint)
		then
		    n = n + 1;
		    if (n >= match)
		    then
			-- update constraints to reflect what we placed
			c_color[y_positions[1]][x_positions[1]] = tile.a_constraint
			c_color[y_positions[2]][x_positions[2]] = tile.b_constraint
			c_color[y_positions[3]][x_positions[3]] = tile.c_constraint
			c_color[y_positions[4]][x_positions[4]] = tile.d_constraint
			c_color[y_positions[5]][x_positions[5]] = tile.e_constraint
			c_color[y_positions[6]][x_positions[6]] = tile.f_constraint
			return tile
		    end
		end
	    end
	    if (n == 0)
	    then
		return nil
	    end
	    match = math.random(n)
	end
	return nil
end

local function choose_tile_hv(tiles, upright, y_positions, x_positions)
-- private Tile chooseTile(Tile[] list, int numlist, boolean upright, int[] y_positions, int[] x_positions)
        local a, b, c, d, e, f = 0, 0, 0, 0, 0, 0
        if (upright)
        then
            a = h_color[y_positions[1]][x_positions[1]]
            b = v_color[y_positions[2]][x_positions[2]]
            c = v_color[y_positions[3]][x_positions[3]]
            d = v_color[y_positions[4]][x_positions[4]]
            e = v_color[y_positions[5]][x_positions[5]]
            f = h_color[y_positions[6]][x_positions[6]]
        else
            a = h_color[y_positions[1]][x_positions[1]]
            b = h_color[y_positions[2]][x_positions[2]]
            c = v_color[y_positions[3]][x_positions[3]]
            d = v_color[y_positions[4]][x_positions[4]]
            e = h_color[y_positions[5]][x_positions[5]]
            f = h_color[y_positions[6]][x_positions[6]]
        end
        local n = 0
	local match = math.huge
	for pass=1,2
	do
            n = 0
            -- pass #1:
            --   count number of variants that match this partial set of constraints
            -- pass #2:
            --   stop on randomly selected match
            for i=1, #tiles
            do
                local tile = tiles[i];
                if
                   (a < 0 or a == tile.a_constraint) and
                   (b < 0 or b == tile.b_constraint) and
                   (c < 0 or c == tile.c_constraint) and
                   (d < 0 or d == tile.d_constraint) and
                   (e < 0 or e == tile.e_constraint) and
                   (f < 0 or f == tile.f_constraint)
                then
                    n = n + 1
                    if (n >= match)
                    then
                        -- update constraints to reflect what we placed
                        if (upright)
                        then
                            h_color[y_positions[1]][x_positions[1]] = tile.a_constraint
                            v_color[y_positions[2]][x_positions[2]] = tile.b_constraint
                            v_color[y_positions[3]][x_positions[3]] = tile.c_constraint
                            v_color[y_positions[4]][x_positions[4]] = tile.d_constraint
                            v_color[y_positions[5]][x_positions[5]] = tile.e_constraint
                            h_color[y_positions[6]][x_positions[6]] = tile.f_constraint
                        else
                            h_color[y_positions[1]][x_positions[1]] = tile.a_constraint
                            h_color[y_positions[2]][x_positions[2]] = tile.b_constraint
                            v_color[y_positions[3]][x_positions[3]] = tile.c_constraint
                            v_color[y_positions[4]][x_positions[4]] = tile.d_constraint
                            h_color[y_positions[5]][x_positions[5]] = tile.e_constraint
                            h_color[y_positions[6]][x_positions[6]] = tile.f_constraint
                        end
                        return tile
                    end
                end
            end
            if (n == 0)
            then
                return nil
            end
            match = math.random(n)
        end
        return nil
end


local function matchingAdjacent(y, x)
        return c_color[y][x] == c_color[y + 1][x + 1];
end
local function changeColor(old_color, num_options)
        local offset = math.random(num_options - 1) -- this is MEANT to produce a smaller range than num_options
        return (old_color + offset) % num_options;
end

function generate(tileset, h, w, seed)
        if seed ~= nil then math.randomseed(seed) end
        local ts = tileset_types[tileset]
        local output = init2d('#', h, w)
        local sidelen = ts.config.short_side_length
        local xmax = (w / sidelen) + 6
        local ymax = (h / sidelen) + 6
        if xmax > 1006
        then
            return nil;
        end
        if ymax > 1006
        then
            return nil;
        end
        if ts.config.is_corner
        then
            c_color = init2d(-1, ymax, xmax)
            local ypos = -1 * sidelen + 1
            local cc = { ts.config.num_color_0, ts.config.num_color_1, ts.config.num_color_2, ts.config.num_color_3 }

            for j=1,ymax
            do
                for i=1,xmax
                do
                    local p = bit.band(i - j + 1, 3) + 1 -- corner type
                    c_color[j][i] = math.random(cc[p]) - 1
                end
            end

            -- Repetition reduction
            -- now go back through and make sure we don't have adjacent 3x2 vertices that are identical,
            -- to avoid really obvious repetition (which happens easily with extreme weights)

            for j=1,ymax - 3
            do
                    for i=1, xmax - 3
                    do
                            if i + 3 > 1006 then return nil end
                            if j + 3 > 1006 then return nil end
                            if matchingAdjacent(j, i) and matchingAdjacent(j + 1, i) and matchingAdjacent(j + 2, i)
                                    and matchingAdjacent(j, i + 1) and matchingAdjacent(j + 1, i + 1) and matchingAdjacent(j + 2, i + 1)
                            then
                                    local p = bit.band((i+1) - (j+1) + 1, 3) + 1 -- corner type
                                    if (cc[p] > 1)
                                    then
                                        c_color[j + 1][i + 1] = changeColor(c_color[j + 1][i + 1], cc[p])
                                    end
                            end
                            if matchingAdjacent(j, i) and matchingAdjacent(j, i + 1) and matchingAdjacent(j, i + 2)
                                    and matchingAdjacent(j + 1, i) and matchingAdjacent(j + 1, i + 1) and matchingAdjacent(j + 1, i + 2)
                            then
                                    local p = bit.band((i+2) - (j+1) + 1, 3) + 1 -- corner type
                                    if (cc[p] > 1)
                                    then
                                        c_color[j+1][i+2] = changeColor(c_color[j + 1][i + 2], cc[p])
                                    end
                            end
                    end
            end
            local j = -1
            local i = 0
            while ypos < h
            do
                -- a general herringbone row consists of:
                --    horizontal left block, the bottom of a previous vertical, the top of a new vertical
                local phase = bit.band(j, 3)
                -- displace horizontally according to pattern
                if (phase == 0)
                then
                    i = 0;
                else
                    i = phase - 4;
                end
                while(true)
                do
                    local xpos = i * sidelen + 1;
                    if xpos > w then break end -- Important! Stop writing horizontal blocks when you reach the end of the row.
                    -- horizontal left-block
                    if xpos + sidelen * 2 >= 1 and ypos >= 1
                    then
                        local t = choose_tile_c(
                                ts.h_tiles,
                                {j + 3, j + 3, j + 3, j + 4, j + 4, j + 4}, -- numbers here are different due to 0-indexing of i and j
                                {i + 3, i + 4, i + 5, i + 3, i + 4, i + 5})
                              --{j + 2, j + 2, j + 2, j + 3, j + 3, j + 3},
                              --{i + 2, i + 3, i + 4, i + 2, i + 3, i + 4});
                        if (t == nil) then return nil end

                        insert(output, t.data, ypos, xpos)
                    end
                    xpos = xpos + sidelen * 2;
                    -- now we're at the end of a previous vertical one
                    xpos = xpos + sidelen;
                    -- now we're at the start of a new vertical one
                    if xpos <= w
                    then
                        local t = choose_tile_c(
                                ts.v_tiles,
                                {j + 3, j + 4, j + 5, j + 3, j + 4, j + 5}, -- numbers here are different due to 0-indexing of i and j
                                {i + 6, i + 6, i + 6, i + 7, i + 7, i + 7})
                              --{j + 2, j + 3, j + 4, j + 2, j + 3, j + 4},
                              --{i + 5, i + 5, i + 5, i + 6, i + 6, i + 6});

                        if (t == nil) then return nil end
                        insert(output, t.data, ypos, xpos)
                    end
                    i = i + 4;
                end
                j = j + 1
                ypos = ypos + sidelen;
            end
        else
            local ypos = -1 * sidelen;
            
            v_color = init2d(-1, ymax, xmax)
            h_color = init2d(-1, ymax, xmax)
            
            local i = 0
            local j = -1
            while ypos < h
            do
                -- a general herringbone row consists of:
                --    horizontal left block, the bottom of a previous vertical, the top of a new vertical
                local phase = bit.band(j, 3)
                -- displace horizontally according to pattern
                if phase == 0
                then
                    i = 0;
                else
                    i = phase - 4;
                end
                while true
                do
                    local xpos = i * sidelen + 1
                    if xpos > w then break end -- Important! Stop writing horizontal blocks when you reach the end of the row.
                    -- horizontal left-block
                    if xpos + sidelen * 2 >= 1 and ypos >= 1
                    then
                        local t = choose_tile_hv(
                                ts.h_tiles, false,
                                {j + 3, j + 3, j + 3, j + 3, j + 4, j + 4},
                                {i + 3, i + 4, i + 3, i + 5, i + 3, i + 4})
                              --{j + 2, j + 2, j + 2, j + 2, j + 3, j + 3},
                              --{i + 2, i + 3, i + 2, i + 4, i + 2, i + 3});
                        if (t == nil) then return nil end
                        insert(output, t.data, ypos, xpos);
                    end
                    xpos = xpos + sidelen * 2;
                    -- now we're at the end of a previous vertical one
                    xpos = xpos + sidelen;
                    -- now we're at the start of a new vertical one
                    if xpos <= w
                    then
                        local t = choose_tile_hv(
                                ts.v_tiles, true,
                                {j + 3, j + 3, j + 3, j + 4, j + 4, j + 5},
                                {i + 6, i + 6, i + 7, i + 6, i + 7, i + 6});
                              --{j + 2, j + 2, j + 2, j + 3, j + 3, j + 4},
                              --{i + 5, i + 5, i + 6, i + 5, i + 6, i + 5});

                        if t == nil then return nil end
                        insert(output, t.data, ypos, xpos)
                    end
                    i = i + 4
                end
                j = j + 1
                ypos = ypos + sidelen
            end
        end
        
        return output
end
