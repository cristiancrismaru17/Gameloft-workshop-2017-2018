attribute vec3 a_posL;
attribute vec3 a_color;

varying vec3 v_color;

uniform mat4 transformationMatrix;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main() {
	vec4 posL = vec4(a_posL, 1.0);
	vec4 worldPosition = transformationMatrix * posL;
	vec4 positionRelativeToCamera = viewMatrix * worldPosition;
	gl_Position = projectionMatrix * positionRelativeToCamera;
	v_color = a_color;
}
