
//-------------------------------------------------------------------------------
//Vertex and pixel shader demonstrating ambient, diffuse and specular lighting
// Ambient intensity values are pre baked
//------------------------------------------------------------------------------------------------------

//------------------------------------------------------------------------------------------------------
// Global params provided by the app
//------------------------------------------------------------------------------------------------------
uniform extern float4x4 gWVP;
uniform extern float4x4 gWorldIT;
uniform extern float4x4 gWorld;
uniform extern float4x4 gWorldViewIT;
uniform extern float4x4 gWorldView;
uniform extern float4	gCamPosW; 

uniform extern float4 gDiffuseMtrl;
uniform extern float4 gAmbMtrl;
uniform extern float4 gSpecMtrl;	

uniform extern float4 gLightCol;
uniform extern float4 gLightDir;

struct PointLight{

float4 gAmbLight;
float4 gSpecLight;
float4 gDiffuseLight; 
float4 gLightPosW;
float3 gAtten123;

};

uniform extern int numlights;

uniform extern PointLight light[8];

uniform extern texture gTex;

uniform extern float	gCenterX;
uniform extern float	gCenterY;
uniform extern float	gCenterZ;
uniform extern float	gRadius;
uniform extern float gMinY;
uniform extern float gMaxY;


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

float4 I_a = { 1.0f, 1.0f, 1.0f, 1.0f };    // ambient 
float4 I_d = { 1.0f, 1.0f, 1.0f, 1.0f };    // diffuse
float4 I_s = { 1.0f, 1.0f, 1.0f, 1.0f };    // specular
 


float  n   : MATERIALPOWER =16;                            // power
//---------------------------------------------------------------------------------------------------------------------------
// Input channel (vertex shader)(point lights)
//---------------------------------------------------------------------------------------------------------------------------
struct PInputVS
{
	float3 posL : POSITION0;
	float3 Norm : NORMAL;
	float2 tex0 : TEXCOORD0;
};


//---------------------------------------------------------------------------------------------------------------------------
// Output channel (vertex shader)(point light)
//---------------------------------------------------------------------------------------------------------------------------
struct POutputVS
{
	float4 posH : POSITION0;
	float4 posW: TEXCOORD0;
	float3 viewDir: TEXCOORD1;
	float3 normal: TEXCOORD2;
	float2 tex0 : TEXCOORD3;
	float4 LightVec: TEXCOORD4;

};


//---------------------------------------------------------------------------------------------------------------------------
// Vertex shader-POINT LIGHTS
//---------------------------------------------------------------------------------------------------------------------------

POutputVS PointLightVS(PInputVS input)
{
	//Zero out our output
	POutputVS outVS = (POutputVS)0;
	
	//Transform to homogeneous clipspace
	outVS.posH = mul(float4(input.posL, 1.0f), gWVP);
	
	//world position 
	outVS.posW = mul(float4(input.posL, 1.0f),gWorld);
	
	//normals
	outVS.normal =mul(input.Norm,gWorldIT).xyz;
	 
	
	// Pass on texture coordinates to be interpolated in rasterization.
	outVS.tex0 = input.tex0;
	
	//return output
	return outVS;
}


//---------------------------------------------------------------------------------------------------------------------------
// Input channel (pixel shader)(point light) 
//---------------------------------------------------------------------------------------------------------------------------
struct PInputPS
{
	float4 posH : POSITION0;
	float4 posW: TEXCOORD0;
	float3 viewDir: TEXCOORD1;
	float3 normal: TEXCOORD2;
	float2 tex0 : TEXCOORD3;
	float4 LightVec: TEXCOORD4;


};


//---------------------------------------------------------------------------------------------------------------------------
// Pixel shader (input channel):output channel - POINT LIGHTS
//---------------------------------------------------------------------------------------------------------------------------
float4 PointLightPS(PInputPS input): COLOR
{
	float			xCom;
	float			yCom;
	float			zCom;
	float grey;
	
	xCom = (input.posW.x - gCenterX)*(input.posW.x - gCenterX);
	yCom = (input.posW.y - gCenterY)*(input.posW.y - gCenterY);
	zCom = (input.posW.z - gCenterZ)*(input.posW.z - gCenterZ);
	
	
	float3 Color = float4(0.0f, 0.0f, 0.0f, 0.0f);
	float4 texColor = tex2D(TexS, input.tex0);
	float4 globalamb ={0.5, 0.5, 0.5, 1.0};
	
	for (int i = 0; i < (numlights); ++i)
	{
	
	//light vector
	input.LightVec = normalize(light[i].gLightPosW - input.posW);
	
	
	//calculate attenuation
	float dist = distance(light[i].gLightPosW, input.posW);
	float atten = light[i].gAtten123.x + dist*light[i].gAtten123.y + light[i].gAtten123.z*dist*dist;
	
	//normalise normal vector
	input.normal =normalize(input.normal).xyz;

	//Calculate diff, specular and ambient components of lighht
	float diff = max(0, dot(input.normal,input.LightVec));
	
	//specular component 
	input.viewDir=normalize(gCamPosW - input.posW);
	float3 reflectW=reflect(-input.LightVec,input.normal);
	float spec = pow(max(0, dot(reflectW, input.viewDir)), 32)/atten;
	
	float4 specular = gSpecMtrl*spec*light[i].gSpecLight;
	float4 diffuse = gDiffuseMtrl*diff*light[i].gDiffuseLight;
	float4 ambient =gAmbMtrl*globalamb*light[i].gAmbLight;
	
	
	Color += ambient*texColor+(diffuse*texColor + specular)/atten;
	
	}
	
	float4 finalCol = float4( Color, gDiffuseMtrl.a*texColor.a); 
	
	grey = (float3)dot(float3(finalCol.r,finalCol.g,finalCol.b), float3(0.212671f, 0.715160f, 0.072169f));
	
	//float factor = (xCom + zCom) / (1000 / (xCom + zCom));
	float dist = (xCom + zCom) / gRadius;
	
	float4 result;    
    result.r = (finalCol.r - (finalCol.r * dist)) + grey * dist;
    result.g = (finalCol.g - (finalCol.g * dist)) + grey * dist;
    result.b = (finalCol.b - (finalCol.b * dist)) + grey * dist;
    result.a = finalCol.a;
	
	if (((xCom + zCom) <= (gRadius)) && (input.posW.y < gMaxY) && (input.posW.y > gMinY))
	{
		//return finalCol;
		//return float4(final.r, final.g, final.b, 1.0f);
		return result;
	}
	else
	{
		return float4(grey, grey, grey, 1.0f);
	}
	
}

technique MyTech
{

//pass DirectionalLight
//{

	//	Lighting       = TRUE;
    //  SpecularEnable = TRUE;
	//	vertexShader = compile vs_3_0 PhongShaderVS();
	//	pixelShader = compile ps_3_0 SimpleLightPS();
	//	//specify render device states associated with the pass
	//	FillMode = Solid;
	
//}

	pass PointLights
	{ 
		SpecularEnable = TRUE;
		AlphaBlendEnable = TRUE;
		
		vertexShader = compile vs_3_0 PointLightVS();
		pixelShader = compile ps_3_0 PointLightPS();
		//specify render device states associated with the pass
		FillMode =  Solid;
		


	}	

}