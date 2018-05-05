precision mediump float;

#define NR_POINT_LIGHTS 10


varying vec4 v_posL;
varying vec2 v_uv;
varying vec2 v_buv;
varying vec3 v_color;
varying vec3 normal;
varying float visibility;

varying float genX;
varying float genY;

uniform vec3 cameraPos;

uniform sampler2D u_texture0;
uniform sampler2D u_texture1;
uniform sampler2D u_texture2;
uniform sampler2D u_texture3;

uniform vec3 skyColor;

struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float shininess;
}; 
  
uniform Material material;

struct DirLight {
    vec3 direction;
  
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};  
uniform DirLight dirLight;

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir, vec3 color)
{
    vec3 lightDir = normalize(-light.direction);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // combine results
    vec3 ambient  = light.ambient  * color;
    vec3 diffuse  = light.diffuse  * diff * color;
    vec3 specular = light.specular * spec * color;
    return (ambient * 0.01 + diffuse * 0.5 + specular);
}  

struct PointLight {
    vec3 position;
    
    float constant;
    float linear;
    float quadratic;  

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
uniform PointLight pointLights[NR_POINT_LIGHTS];

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir, vec3 color)
{
    vec3 lightDir = normalize(light.position - fragPos);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // attenuation
    float distance    = length(light.position - fragPos);
    float attenuation = 400.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));  
    // combine results
    vec3 ambient  = light.ambient  * color;
    vec3 diffuse  = light.diffuse  * diff * color;
    vec3 specular = light.specular * spec * color;
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    return (ambient * 0.01 + diffuse * 0.5 + specular);
} 

uniform struct SpotLight {
   vec3 position;
   float coneAngle;
   vec3 coneDirection;

    float constant;
    float linear;
    float quadratic;  

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
uniform SpotLight spotLight;

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir, vec3 color)
{
    vec3 lightDir = normalize(light.position - fragPos);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // attenuation
    float distance    = length(light.position - fragPos);
    float attenuation = 500.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
	float lightToSurfaceAngle = degrees(acos(dot(-lightDir, normalize(light.coneDirection))));
	if(lightToSurfaceAngle > 50.0)
	{
		attenuation = 0.0;
	}
    // combine results
    vec3 ambient  = light.ambient  * color;
    vec3 diffuse  = light.diffuse  * diff * color;
    vec3 specular = light.specular * spec * color;
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    return (ambient * 0.0 + diffuse * 0.5 + specular);
}

void main()
{
	vec4 c_blend = texture2D(u_texture3, v_buv);

	vec4 c_rock = texture2D(u_texture0, v_uv);
	vec4 c_grass = texture2D(u_texture1, v_uv);
	vec4 c_dirt = texture2D(u_texture2, v_uv);
	vec4 c_final = c_blend.r * c_rock + c_blend.g * c_grass + c_blend.b * c_dirt;
    gl_FragColor = c_final;
	

	//////////lights
	vec3 FragPos = vec3(v_posL);
    vec3 norm = normal;
    vec3 viewDir = normalize(cameraPos - FragPos);

	// Directional lighting
    vec3 result = CalcDirLight(dirLight, norm, viewDir, vec3(gl_FragColor)) * 0.1;
    // Point lights
	for( int i = 0; i < NR_POINT_LIGHTS; i++ )
	{
		result += CalcPointLight(pointLights[i], norm, FragPos, viewDir, vec3(gl_FragColor)) * 2.0;
	}
    // Spot light
    result += CalcSpotLight(spotLight, norm, FragPos, viewDir, vec3(gl_FragColor));   

	gl_FragColor = vec4(result, 1.0);
	
	//fog
	gl_FragColor = mix(vec4(skyColor, 1.0), gl_FragColor, visibility);
}
