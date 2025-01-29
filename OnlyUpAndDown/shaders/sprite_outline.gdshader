shader_type canvas_item;

uniform vec4 outline_color : source_color = vec4(0, 0, 0, 1);
uniform float outline_width = 1.0;

void fragment() {
	vec2 uv = UV;

	vec2 uv_up = uv + outline_width * vec2(0,TEXTURE_PIXEL_SIZE.y);
	vec2 uv_down = uv + outline_width * vec2(0,- TEXTURE_PIXEL_SIZE.y);
	vec2 uv_left = uv + outline_width * vec2(TEXTURE_PIXEL_SIZE.x,0);
	vec2 uv_right = uv + outline_width * vec2(-TEXTURE_PIXEL_SIZE.x,0);

	vec4 color_up = texture(TEXTURE, uv_up);
	vec4 color_down = texture(TEXTURE, uv_down);
	vec4 color_left = texture(TEXTURE, uv_left);
	vec4 color_right = texture(TEXTURE, uv_right);

	vec4 outline = color_up + color_down + color_left + color_right;
	outline.rgb = outline_color.rgb;

	vec4 color_original = texture(TEXTURE, UV);


	COLOR = mix(outline, color_original, color_original.a);


}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
