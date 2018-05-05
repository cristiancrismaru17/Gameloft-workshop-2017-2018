attribute vec3 a_posL;
attribute vec2 a_uv;
attribute vec3 a_color;
attribute vec3 a_normals;

const float density = 0.00031;
const float gradient = 0.7;

varying vec3 normal;
varying vec4 v_posL;
varying vec2 v_uv;
varying vec3 v_color;
varying float visibility;

uniform mat4 transformationMatrix;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main() {
	vec4 posL = vec4(a_posL, 1.0);
	vec4 worldPosition = transformationMatrix * posL;
	vec4 positionRelativeToCamera = viewMatrix * worldPosition;
	gl_Position = projectionMatrix * positionRelativeToCamera;

	v_posL = worldPosition;
	normal = a_normals;

	v_uv = a_uv;
	v_color = a_color;

	float distance = length(positionRelativeToCamera.xyz);
	visibility = exp(-pow((distance * density), gradient));
}
