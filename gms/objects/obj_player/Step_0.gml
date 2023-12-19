if (keyboard_check(vk_left) || keyboard_check(ord("A"))) {
	if (place_free(x-1, y)) x--;	
}

if (keyboard_check(vk_right) || keyboard_check(ord("D"))) {
	if (place_free(x+1, y)) x++;	
}

if (keyboard_check(vk_up) || keyboard_check(ord("W"))) {
	if (place_free(x, y-1)) y--;	
}

if (keyboard_check(vk_down) || keyboard_check(ord("S"))) {
	if (place_free(x, y+1)) y++;	
}