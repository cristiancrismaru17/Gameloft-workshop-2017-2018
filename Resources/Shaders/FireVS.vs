attribute vec3 a_posL;
attribute vec2 a_uv;
//attribute vec2 a_auv;
attribute vec2 a_buv;

const float density = 0.00011;
const float gradient = 0.8;

varying vec2 v_uv;
varying vec2 v_duv;

//lights
varying float visibility;
uniform vec4 cameraPos;

uniform mat4 transformationMatrix;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main() {
	vec4 posL = vec4(a_posL, 1.0);
	posL.y -= 7.5;
	vec4 worldPosition = transformationMatrix * posL;
	vec4 positionRelativeToCamera = viewMatrix * worldPosition;
	gl_Position = projectionMatrix * positionRelativeToCamera;

	v_uv = a_uv;
	v_duv = a_buv;

	float distance = length(positionRelativeToCamera.xyz);
	visibility = exp(-pow((distance * density), gradient));
}
