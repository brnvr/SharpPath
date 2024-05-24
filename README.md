# SharpPath

SharpPath is  a GameMaker Studio extension that provides asynchronous, grid-based pathfinding for GMS. Essentially, it processes the pathfinding algorithm asynchronously, which can potentially boost performance (when comparing it to GameMaker native pathfinding mechanisms), specially in situations with large maps or many concurrent executions (like when many enemies are chasing the player).

This repository contains the C# project, the extension files as well as a GMS example project with basic functionalities. Documentation is a work in progress but some of it can be found in /docs.

## Limitations

- There's no mechanism (yet) to smoothen the object's movement through the path, so It will always move either orthogonally or diagonally (8 directions).

- It must be taken into consideration that if an object has finished following its current path and there are no avalaible cores to process a new path, it will be standing still until a core is made avaliable. The game won't freeze, but this may affect gameplay in some situations.

- It's still only built for Windows, but that can change soon.

## Use

Build a grid and add obstacles:

```gml
//creates grid in position (0, 0), each cell having the dimentions (24, 24) 
grid = sp_grid_create(0, 0, 24, 24, 16, 16)
sp_grid_add_obstacles_object(grid, obj_obstacle)
```

Alternatively, you can also initialize the grid with obstacles using an array or a ds_grid:

```gml
//0 are empty cells and 1 are obstacles
grid = sp_grid_create_from_array(0, 0, 24, 24, [
	[0, 0, 1, 0, 1],
	[1, 0, 0, 0, 0],
	[1, 1, 0, 1, 0],
	[0, 0, 1, 0, 0]
])
```

Create a path:

```gml
path = sp_path_create(grid)
```

Search for the path:

```gml
sp_find_path(path, id, obj_target)
```

Alternatively, you can set a loop in the **Step** event so the path will periodically be searched for:

```gml
/*Searches for the path each 50 steps. Increasing the period will also increase performance,
but may cause delayed reactions on the follower object.*/
sp_find_path(path, id, obj_target, 50)
```

Update the path asynchronously inside the **Async - System** event:

```gml
sp_path_get_async(path)
```

Finally, inside the **Step** event, make the current instance follow the path:

```gml
//Follows the path with speed 1
sp_follow_path(path, id)
```

