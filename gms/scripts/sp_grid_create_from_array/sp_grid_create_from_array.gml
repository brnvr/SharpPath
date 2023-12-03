function sp_grid_create_from_array(array) {
	var _width, _height, _buffer, grid;
	
	if (!is_array(array)) {
		throw $"{array} is not a valid array.";	
	}
	
	_width = array_length(array[0]);
	_height = array_length(array);
	
	if (_width == 0 || _height == 0) {
		throw "array must be bidimentional and not empty.";	
	}
	
	_buffer = buffer_create(_width*_height, buffer_fixed, 1);
	
	for (var yy = 0; yy < _height; yy++) {
		if (array_length(array[yy]) != _width) {
			throw "every row of the array must have the same size.";
		}
		
		for (var xx = 0; xx < _width; xx++) {
			buffer_write(_buffer, buffer_bool, array[yy][xx]);
		}
	}
	
	grid = {
		width: _width,
		height: _height,
		buffer: _buffer
	}
	
	SpGridAdd(grid, _width, buffer_base64_encode(_buffer, 0, buffer_get_size(_buffer)));
	
	return grid;
}