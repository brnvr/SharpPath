# Creating a SP grid
**SP grids** store the information of what areas in your game map are empty (and so can be transitted) and what areas are obstacles. This information will then be used to find the shortest path between two points in the grid.
## sp_grid_create
Creates an empty SP Grid in the specified position and returns its index.

| Parameter   | Type | Description
|-------------|------|------------
| xstart      | Real | The left starting X position of the grid.
| ystart      | Real | The top starting Y position of the grid.
| hcells      | Real | The number of cells in the horizontal direction (columns).
| vcells      | Real | The number of cells in the vertical direction (rows).
| cell_width  | Real | The width of the cells.
| cell_height | Real | The height of the cells.

### Example

	//Creates a SP grid in the position (15, 25), with 75x75 cells, each cell
	//with the size of 16x16 pixels.
	
    sp_grid = sp_grid_create(15, 25, 75, 75, 16, 16);

## sp_grid_create_from_ds_grid
Creates a SP grid from an existing DS grid. Cells with value '0' in the DS grid will be considered empty and cells with value '1' will be considered obstacles (except if the **inverted** flag is set to true).

| Parameter   | Type    | Description
|-------------|---------|------------
| xstart      | Real    | The left starting X position of the grid.
| ystart      | Real    | The top starting Y position of the grid.
| cell_width  | Real    | The width of the cells.
| cell_height | Real    | The height of the cells.
| ds_grid     | DS Grid | The index of the DS grid.
| inverted    | Bool    | *Optional* If true, then cells with value '0' in the DS Grid will be considered obstacles and cells with value '1' will be considered empty.

### Example

	//Creates DS grid with 20x20 cells and sets the value of specified cells to '1'.
	
	ds_grid = ds_grid_create(20, 20);
	ds_grid[# 13, 1] = 1;
	ds_grid[# 15, 3] = 1;
	ds_grid[# 19, 2] = 1; 
		
	//Creates a SP grid from the DS grid in the position(0, 15). Every cell
	//with value '1' in the DS grid will be an obstacle in the SP grid.
	
	sp_grid = sp_grid_create_from_ds_grid(0, 15, 16, 16, ds_grid, false);

## sp_grid_create_from_array
Creates a SP Grid from a bidimentional array in the format:
	
	[[0, 0, 1, 1, 0],
	 [0, 1, 0, 0, 0],
	 [0, 0, 0, 0, 1],
	 [0, 1, 1, 1, 1]]
	 
Where positions with value '0' will be considered empty cells in the SP grid, and positions with value '1' will be considered obstacles (except if the **inverted** flag is set to true).

| Parameter   | Type    | Description
|-------------|---------|------------
| xstart      | Real    | The left starting X position of the grid.
| ystart      | Real    | The top starting Y position of the grid.
| cell_width  | Real    | The width of the cells.
| cell_height | Real    | The height of the cells.
| array       | Array   | The bidimentional array.
| inverted    | Bool    | *Optional* If true, then positions with value '0' will be considered obstacles in the DS grid, and positions with value '1' will be considered empty.

	//Creates a bidimentional array with values 0 and 1;
		
	array = [
		[0, 1, 0, 1, 0]
		[0, 1, 0, 0, 0]
		[0, 0, 1, 1, 0]
		[0, 0, 0, 0, 0]
	];
	
	//Creates a SP grid from the array in the position (0, 0). Every position in
	//the array with value 1 will be an obstacle in the SP grid.

	sp_grid = sp_grid_create_from_array(0, 0, 16, 16, array, false);
