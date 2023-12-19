globalvar  grid_demo;

grid_demo = sp_grid_create(0, 0, 23, 23, 16, 16);
sp_grid_add_obstacles_object(grid_demo, obj_obstacle);

showing_debug_overlay = false;