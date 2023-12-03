function sp_grid_validate(grid) {
	if (!sp_grid_is_valid(grid)) {
		throw $"sp_grid {grid} is not valid.";	
	}
}