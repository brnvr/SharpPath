function sp_grid_validate_position(grid, xpos, ypos) {
	sp_grid_validate(grid);
	
	if (!is_real(xpos)) {
		throw "Invalid x position.";	
	}
	
	if (!is_real(ypos)) {
		throw "Invalid y position.";	
	}
	
	if (xpos < 0 || xpos >= grid.width) {
		throw "x position out of bounds.";	
	}
	
	if (ypos < 0 || ypos >= grid.height) {
		throw "y position out of bounds.";	
	}
}