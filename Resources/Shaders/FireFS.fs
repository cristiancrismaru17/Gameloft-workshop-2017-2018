precision mediump float;

varying vec2 v_uv;
varying vec2 v_duv;


//lights
uniform vec3 skyColor;
varying float visibility;
uniform float brightness;

//Textures
uniform vec2	  texRatio2;
uniform vec2	  texRatio;
uniform float	  uTime;
uniform float	  u_DispMax;
uniform sampler2D u_texture0;
uniform sampler2D u_texture1;
uniform sampler2D u_texture2;


void main()
{
	vec2 v_auv;
	v_auv[0] = v_uv[0] * texRatio[0];
	v_auv[1] = v_uv[1] * texRatio[1];

	vec2 disp_blend = texture2D(u_texture0, vec2(v_uv.x * texRatio2[0], v_uv.y * texRatio2[1] + uTime)).rg;

	disp_blend.x -= 0.5;
	disp_blend.y -= 0.5;
	disp_blend *= 2.0;
	disp_blend *= u_DispMax;

	vec4 fireTex = texture2D(u_texture1, vec2(v_uv.x + disp_blend.x, v_uv.y + disp_blend.y));
	vec4 alpha_blend = texture2D(u_texture2, v_auv);

	vec4 c_final = fireTex * alpha_blend.r;
    gl_FragColor = vec4(mix(skyColor, vec3(c_final), visibility), c_final.a);
}
