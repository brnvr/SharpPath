globalvar grid_demo;

/*creates a grid and populates it. Each cell where 'obj_obstacle' is over
will be considered an obstacle*/
grid_demo = sp_grid_create(0, 0, 23, 23, 16, 16);
sp_grid_add_obstacles_object(grid_demo, obj_obstacle);