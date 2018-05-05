precision mediump float;

attribute vec3 a_posL;
attribute vec2 a_uv;
attribute vec2 a_buv;
attribute vec3 a_color;

const float density = 0.00031;
const float gradient = 0.7;

varying vec3 normal;
varying vec4 v_posL;
varying vec2 v_uv;
varying vec2 v_buv;
varying vec3 v_color;
varying float visibility;

varying float genX;
varying float genZ;

uniform vec3 cameraPos;
uniform sampler2D u_texture3;
uniform mat4 transformationMatrix;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main() {

	v_posL = vec4(a_posL, 1.0);
	normal = vec3(0.0, 1.0, 0.0);
	
	vec4 c_blend = texture2D(u_texture3, a_buv);

	//////heightmap
	v_posL.y += c_blend.r*20.0 + c_blend.g*0.0 - c_blend.b*10.0;

	vec4 worldPosition = transformationMatrix * v_posL;
	vec4 positionRelativeToCamera = viewMatrix * worldPosition;
	gl_Position = projectionMatrix * positionRelativeToCamera;
	
	v_uv = a_uv;
	v_buv = a_buv;
	v_color = a_color;
	
	v_posL = worldPosition;

	float distance = length(positionRelativeToCamera.xyz);
	visibility = exp(-pow((distance * density), gradient));
}
