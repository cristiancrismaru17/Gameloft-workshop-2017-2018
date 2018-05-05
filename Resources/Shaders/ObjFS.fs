precision mediump float;

varying vec4 v_posL;
varying vec2 v_uv;
varying vec3 v_color;
varying vec3 normal;
varying float visibility;
uniform vec3 cameraPos;

uniform vec3 skyColor;
uniform sampler2D u_texture0;

struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float shininess;
}; 
uniform Material material;

#define NR_POINT_LIGHTS 10

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
    return (ambient * 0.01 + diffuse * 0.5 + specular);
}

void main()
{
	//texture
    gl_FragColor = texture2D(u_texture0, v_uv);

	//////////lights
	vec3 FragPos = vec3(v_posL);
    vec3 norm = normal;
    vec3 viewDir = normalize(cameraPos - FragPos);

	// Directional lighting
    vec3 result = CalcDirLight(dirLight, norm, viewDir, vec3(gl_FragColor)) * 0.0;
    // Point lights
	for( int i = 0; i < NR_POINT_LIGHTS; i++ )
	{
		result += CalcPointLight(pointLights[i], norm, FragPos, viewDir, vec3(gl_FragColor));
	}
    // Spot light
    result += CalcSpotLight(spotLight, norm, FragPos, viewDir, vec3(gl_FragColor));   

	gl_FragColor = vec4(result, 1.0);

	//fog
	gl_FragColor = mix(vec4(skyColor, 1.0), gl_FragColor, visibility);
}
