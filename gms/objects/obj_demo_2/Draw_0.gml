draw_set_color(c_blue);

sp_grid_iterate(grid_demo, function(xcell, ycell, is_obstacle) {
	if (is_obstacle) {
		draw_rectangle(xcell, ycell, xcell+grid_demo.cell_width, ycell+grid_demo.cell_height, false);	
	}
})