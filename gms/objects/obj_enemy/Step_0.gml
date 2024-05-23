/*Searches for player in path, each 50 steps (the larger the period,
the longer it will take for the enemy to "notice" that the player may
have gone another way (i.e. reaction time), but better for the
performance)*/
sp_find_path_instance_loop(path, id, obj_player, 50);

//Follows the specified path
sp_follow_path(path, 1);