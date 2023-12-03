function sp_grid_is_valid(grid) {
	var width, height, buffer;
	
	width = variable_struct_get(grid, "width");
	
	if (is_undefined(width) || !is_real(width) || width < 0) return false;
	
	height = variable_struct_get(grid, "height");
	
	if (is_undefined(height) || !is_real(height) || height < 0) return false;
	
	buffer = variable_struct_get(grid, "buffer");
	
	if (is_undefined(buffer) || !buffer_exists(buffer)) return false;
	
	return true;
}