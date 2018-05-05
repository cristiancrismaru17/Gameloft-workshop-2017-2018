precision mediump float;

varying vec3 v_coord;
varying vec3 v_color;
varying float visibility;

uniform vec3 skyColor;
uniform samplerCube u_cube_texture;

void main()
{
	//texture
    gl_FragColor = textureCube(u_cube_texture, v_coord);

	//fog
	gl_FragColor = mix(vec4(skyColor, 1.0), gl_FragColor, visibility);
}
