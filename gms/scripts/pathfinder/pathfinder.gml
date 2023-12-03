function pathfinder(_grid, _xcurrent, _ycurrent, _xdestination, _ydestination) constructor {
	var width, height;
	
	width = ds_grid_width(_grid);
	height = ds_grid_height(_grid);
	
	grid = ds_grid_create(width, height);
	xdestination = _xdestination;
	ydestination = _ydestination;
	most_promising_cell = undefined;
	
	set_start_position = function(xst, yst) {
		xstart = xst;
		ystart = yst;
		xcurrent = xst;
		ycurrent = yst;
		
		grid[# xst, yst].gcost = 0;
	}
	
	for (var xx = 0; xx < width; xx++) {
		for (var yy = 0; yy < height; yy++) {
			var is_obstacle;
			
			is_obstacle = _grid[# xx, yy];
			
			grid[# xx, yy] = {
				"is_obstacle": is_obstacle,
				"fcost": undefined,
				"gcost": undefined,
				"hcost": undefined,
				"parent_cell": undefined,
				"x": xx,
				"y": yy
			}
		}
	}
	
	set_start_position(_xcurrent, _ycurrent);
	
	get_adjacent_cells = function(xcenter, ycenter) {
		var arr_cells, width, height, array_push_cell;
		
		arr_cells = [];
		width = ds_grid_width(grid);
		height = ds_grid_height(grid);
		
		array_push_cell = function(arr, xcell, ycell) {
			var cell;
			
			cell = grid[# xcell, ycell];
			
			if (!cell.is_obstacle) array_push(arr, [xcell, ycell]);
		}
		
		if (xcenter > 0) array_push_cell(arr_cells, xcenter-1, ycenter);
		if (xcenter < width-1) array_push_cell(arr_cells, xcenter+1, ycenter);
		if (ycenter > 0) array_push_cell(arr_cells, xcenter, ycenter-1);
		if (ycenter < height-1) array_push_cell(arr_cells, xcenter, ycenter+1);
		
		return arr_cells;
	}
	
	find_path = function() {
		var path, cell;
		
		path = [];
		
		while (!process()) { }
		
		cell = grid[# xdestination, ydestination];
		
		while (!(cell.x == xstart && cell.y == ystart)) {
			array_push(path, cell);
			cell = cell.parent_cell;
		}
		
		return path;
	}
	
	process = function() {
		var n_cells, center_cell, adjacent_cells;
		
		if (xcurrent == xdestination && ycurrent == ydestination) {
			return true;	
		}
		
		center_cell = grid[# xcurrent, ycurrent];
		adjacent_cells = get_adjacent_cells(xcurrent, ycurrent);
		n_cells = array_length(adjacent_cells);
		
		for (var i = 0; i < n_cells; i++) {
			var cell, _gcost;
			
			cell = grid[# adjacent_cells[i][0], adjacent_cells[i][1]];
			
			_gcost = center_cell.gcost + 1;
			
			if (is_undefined(cell.gcost) || cell.gcost > _gcost) {
				cell.gcost = center_cell.gcost + 1;
				cell.parent_cell = center_cell;
			}
			
			if (is_undefined(cell.hcost)) {
				cell.hcost = get_displacement(cell.x, cell.y, xdestination, ydestination);
			}
			
			cell.fcost = cell.gcost + cell.hcost;
			
			if (is_undefined(most_promising_cell) || cell.fcost < most_promising_cell.fcost || (cell.fcost == most_promising_cell.fcost && cell.hcost < most_promising_cell.hcost)) {
				most_promising_cell = cell;
			}
		}
		
		xcurrent = most_promising_cell.x;
		ycurrent = most_promising_cell.y;
		
		return false;
	}
	
	get_displacement = function(x0, y0, x1, y1) {
		return abs(x1-x0) + abs(y1-y0);	
	}
}