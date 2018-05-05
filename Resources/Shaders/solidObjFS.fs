precision mediump float;

varying vec3 v_color;
uniform vec3 solidColor;

void main()
{

	//solidColor
	gl_FragColor = vec4(solidColor, 1.0);

}
