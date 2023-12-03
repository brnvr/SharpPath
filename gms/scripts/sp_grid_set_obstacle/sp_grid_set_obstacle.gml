function sp_grid_set_obstacle(grid, xpos, ypos, is_obstacle) {
	var offset;
	
	sp_grid_validate_position(grid, xpos, ypos);
	
	offset = grid.width*ypos + xpos;
	
	buffer_poke(grid.buffer, offset, buffer_bool, is_obstacle ? 1 : 0);
	
	SpGridSetObstacle(grid, xpos, ypos, is_obstacle);
}