//includes file

struct Lighting{

float4 gAmbLight;
float4 gSpecLight;
float4 gDiffuseLight; 
float4 gLightPosW;
float3 gAtten123;

};

uniform extern float4x4 gWVP;
uniform extern float4x4 gWorldIT;
uniform extern float4x4 gWorld;
uniform extern float4x4 gViewProj;
uniform extern float4	gCamPosW; 

uniform extern float4 gDiffuseMtrl;
uniform extern float4 gAmbMtrl;
uniform extern float4 gSpecMtrl;	

uniform extern texture gTex;//texture

sampler TexS = sampler_state
{
	Texture = <gTex>;
	MinFilter = Anisotropic;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	MaxAnisotropy = 8;
	AddressU=WRAP;
	AddressV=WRAP;
};

float DepthBias = 0.001;
float texelsize = 1.0/18432.0;
uniform extern texture gShadowMap;
uniform extern float4x4 gLightviewproj;
uniform extern bool withshadow;

sampler ShadowMapSampler = sampler_state
{
    Texture = <gShadowMap>;
    MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = NONE;
    AddressU = Clamp;
    AddressV = Clamp;
};


float  n   : MATERIALPOWER =16;                            // power

//-----------------------------------------------------------------
// Input channel (vertex shader)
//-----------------------------------------------------------------

struct InputVS
{
	float3 posH : POSITION0;
	float3 Norm : NORMAL;
	float2 tex0 : TEXCOORD0;
};

//-----------------------------------------------------------------
// Output channel (vertex shader)
//-----------------------------------------------------------------

struct OutputVS
{
	float4 posH : POSITION0;
	float4 posW: TEXCOORD0;
	float3 viewDir: TEXCOORD1;
	float3 normal: TEXCOORD2;
	float2 tex0 : TEXCOORD3;
	float4 LightVec: TEXCOORD4;
	

};


//-------------------------------------------------------------------
// Input channel (pixel shader)
//-------------------------------------------------------------------
struct InputPS
{
	float4 posH : POSITION0;
	float4 posW: TEXCOORD0;
	float3 viewDir: TEXCOORD1;
	float3 normal: TEXCOORD2;
	float2 tex0 : TEXCOORD3;
	float4 LightVec: TEXCOORD4;
	float3 col_amb: TEXCOORD5;
	float3 col_diff: TEXCOORD6;
	float3 col_spec: TEXCOORD7;
	float3 E: TEXCOORD8;
	float3 EV: TEXCOORD9; 

};


float3 CalculatePointLight(Lighting light, InputPS input, float4 texCol, float4 globalAmb)
{
	//calculate light vector	
	input.LightVec = normalize(light.gLightPosW - input.posW);
	
	//calculate attenuation
	float dist = distance(light.gLightPosW, input.posW);
	float atten = light.gAtten123.x + dist*light.gAtten123.y + light.gAtten123.z*dist*dist;
	
	//normalise normal vector
	input.normal =normalize(input.normal).xyz;

	//Calculate diff, specular and ambient components of light
	float diff = max(0, dot(input.normal,input.LightVec));
	
	//specular component 
	input.viewDir=normalize(gCamPosW - input.posW);
	float3 reflectW=reflect(-input.LightVec,input.normal);
	float spec = pow(max(0, dot(reflectW, input.viewDir)), 32)/atten;
	
	float4 specular = gSpecMtrl*spec*light.gSpecLight;
	float4 diffuse = gDiffuseMtrl*diff*light.gDiffuseLight;
	float4 ambient =gAmbMtrl*globalAmb*light.gAmbLight;
	
	float3 Color = ambient*texCol+(diffuse*texCol + specular)/atten;

	return Color;

}

float3 CalculatePointLightWShadow(Lighting light, InputPS input, float4 texCol, float4 globalAmb, float shadowdiff)
{
	//calculate light vector	
	input.LightVec = normalize(light.gLightPosW - input.posW);
	
	//calculate attenuation
	float dist = distance(light.gLightPosW, input.posW);
	float atten = light.gAtten123.x + dist*light.gAtten123.y + light.gAtten123.z*dist*dist;
	
	//normalise normal vector
	input.normal =normalize(input.normal).xyz;

	//Calculate diff, specular and ambient components of light
	float diff = max(0, dot(input.normal,input.LightVec));
	
	//specular component 
	input.viewDir=normalize(gCamPosW - input.posW);
	float3 reflectW=reflect(-input.LightVec,input.normal);
	float spec = pow(max(0, dot(reflectW, input.viewDir)), 32)/atten;
	
	float4 specular = gSpecMtrl*spec*light.gSpecLight;
	float4 diffuse = (gDiffuseMtrl*diff*light.gDiffuseLight)*shadowdiff;
	float4 ambient =gAmbMtrl*globalAmb*light.gAmbLight;
	
	float3 Color = ambient*texCol+(diffuse*texCol + specular)/atten;

	return Color;

}



//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///CREATE SHADOW MAP- VERTEX AND PIXEL SHADER
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


struct CreateShadowMap_VSOut
{
 float4  position: POSITION;
 float Depth: TEXCOORD0;
 
};  


CreateShadowMap_VSOut CreateShadowMapVS(float4 position: POSITION)
{
CreateShadowMap_VSOut Out;
if(withshadow){
Out.position = mul(position, mul(gWorld,gLightviewproj));
Out.Depth = Out.position.z/Out.position.w;
}
return Out;
} 

float4 CreateShadowMapPS(CreateShadowMap_VSOut input):COLOR
{

return float4(input.Depth, 0, 0, 0);

}

////////////////////////////////////////////////////////////////////
//Shadow map lookups											////
////////////////////////////////////////////////////////////////////

float ShadowMapLookup(sampler shadowMap, float2 texCoord, float depth)
{
	return (tex2D(shadowMap, texCoord).r + DepthBias < depth) ? 0.0f : 1.0f;
}

float ShadowMapLookup(sampler shadowMap, float2 texCoord, float2 offset, float depth)
{
	return (tex2D(shadowMap, texCoord + offset * texelsize).r + DepthBias < depth) ? 0.0f : 1.0f;
}
