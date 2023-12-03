function sp_grid_delete(grid) {
	buffer_delete(grid.buffer);
	
	SpGridRemove(grid);
	
	delete grid;
}