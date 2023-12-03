function sp_grid_get_path(callback){
	var path64, error;

	path64 = async_load[? "sp_path64"];
	error = async_load[? "sp_error"];

	if (!is_undefined(error)) {
		throw error;	
	}

	if (!is_undefined(path64)) {
		var buffer, n_items, items;

		items = [];
		buffer = buffer_base64_decode(path64);
		n_items = buffer_get_size(buffer)/8;

		repeat(n_items) {
			var xx, yy;
		
			xx = buffer_read(buffer, buffer_s32);
			yy = buffer_read(buffer, buffer_s32);
		
			array_push(items, [xx, yy]);
		}
	
		callback(items);
	}
}