//includes file

struct Lighting{

float4 gAmbLight;
float4 gSpecLight;
float4 gDiffuseLight; 
float4 gLightPosW;
float3 gAtten123;
int gtype;

};

shared float4x4 gWVP;
shared float4x4 gWorldIT;
shared float4x4 gWorld;
shared float4x4 gWorldViewIT;
shared float4x4 gWorldView;
shared float4	gCamPosW; 

shared float4 gDiffuseMtrl;
shared float4 gAmbMtrl;
shared float4 gSpecMtrl;	

float4 gLightCol = {1.0, 1.0, 1.0, 1.0};//dir light colour
float4 gLightDir= { -1.0f, -10.0f, 0.0f, 0.0f };//dir light direction

float4 I_a = { 1.0f, 1.0f, 1.0f, 1.0f };    // ambient 
float4 I_d = { 1.0f, 1.0f, 1.0f, 1.0f };    // diffuse
float4 I_s = { 1.0f, 1.0f, 1.0f, 1.0f };    // specular

shared texture gTex;//texture

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
	float3 col_amb: TEXCOORD5;
	float3 col_diff: TEXCOORD6;
	float3 col_spec: TEXCOORD7;
	float3 E: TEXCOORD8;
	float3 EV: TEXCOORD9; 
	

};

//-------------------------------------------------------------------
// Vertex shader
//-------------------------------------------------------------------

OutputVS LightsVS(InputVS input)
{
	//Zero out our output
	OutputVS outVS = (OutputVS)0;
	
	//Transform to homogeneous clipspace
	outVS.posH = mul(float4(input.posH, 1.0f), gWVP);
	
	//world position 
	outVS.posW = mul(float4(input.posH, 1.0f),gWorld);
	
	//normals
	outVS.normal =mul(input.Norm,gWorldIT).xyz;
	 
	// Pass on texture coordinates to be interpolated in rasterization.
	outVS.tex0 = input.tex0;
	
	//-----------------------------------------------
	
	//eye position //for dir light
	outVS.E = mul(input.posH,gWorldView).xyz;

	//eye vector//for dir light
	outVS.EV=normalize(outVS.E);
	
	//for dir light
	outVS.col_amb =  I_a*gAmbMtrl*gLightCol;
	outVS.col_diff = I_d*gDiffuseMtrl*gLightCol;
	outVS.col_spec = I_s*gSpecMtrl*gLightCol;
	
	//-----------------------------------------------
	
	//return output
	return outVS;
}

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


float3 CalculateDirLight(InputPS input, float4 texCol, float4 globalAmb)
{
	
	//transform light vector into viewspace and normalize light vector and normals
	float3 L= -normalize(mul(gLightDir,gWorldView));
	
	input.normal =normalize(input.normal).xyz;

	//Calculate diff, specular and ambient components of lighht
	float diff = max(0, dot(input.normal,L));
    
	//half angle vector used in approximation to specular component
	float3 H=normalize(input.EV+L);
    
	//specular component using Blinns half angle
	float spec = pow(max(0, dot(input.normal, H)), n);	
	
	float3 Color =input.col_diff*diff*texCol + input.col_amb*globalAmb*texCol+ input.col_spec*spec;

	return Color;

}



