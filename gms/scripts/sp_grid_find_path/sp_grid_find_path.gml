function sp_grid_find_path(grid, xst, yst, xdest, ydest) {
	sp_grid_validate_position(grid, xst, yst);
	sp_grid_validate_position(grid, xdest, ydest);
	
	SpGridFindPath(grid, xst, yst, xdest, ydest, 1, true);
}