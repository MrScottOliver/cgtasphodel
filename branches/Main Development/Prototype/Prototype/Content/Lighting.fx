//-------------------------------------------------------------------------------
//Shader that includes directional and point lights (vertex shader and other functions in Includes.inc)
//------------------------------------------------------------------------------------------------------
#include "Includes.inc"


uniform extern int numlights;//number of lights

uniform extern Lighting light[8];


uniform extern float	gCenterX[10];
uniform extern float	gCenterY[10];
uniform extern float	gCenterZ[10];
uniform extern float	gRadius[10];
uniform extern float gMinY[10];
uniform extern float gMaxY[10];

uniform extern bool player;
uniform extern float health;


//---------------------------------------------------------------------------------------------------------------------------
// Pixel shader (input channel):output channel
//---------------------------------------------------------------------------------------------------------------------------
float4 LightsPS(InputPS input): COLOR
{
	float			xCom;
	float			yCom;
	float			zCom;
	float grey;
	
	float4 results[10];
	
	for(int k = 0; k < 10; k++)
	{
	
	xCom = (input.posW.x - gCenterX[k])*(input.posW.x - gCenterX[k]);
	yCom = (input.posW.y - gCenterY[k])*(input.posW.y - gCenterY[k]);
	zCom = (input.posW.z - gCenterZ[k])*(input.posW.z - gCenterZ[k]);
	
	
	float3 Color = {0.0f, 0.0f, 0.0f};
	float4 texColor = tex2D(TexS, input.tex0);
	float4 globalamb ={0.5, 0.5, 0.5, 1.0};
	
	
	for (int i = 0; i < (numlights); ++i)
	{
	Color += CalculatePointLight(light[i], input, texColor, globalamb);
	}
	
	float4 finalCol = float4( Color, gDiffuseMtrl.a*texColor.a); 
	

	grey = (float3)dot(float3(finalCol.r,finalCol.g,finalCol.b), float3(0.212671f, 0.715160f, 0.072169f));
	
	//float factor = (xCom + zCom) / (1000 / (xCom + zCom));
	float dist = (xCom + zCom) / gRadius[k];    
	
	if (((xCom + zCom) <= (gRadius[k])) && (input.posW.y < gMaxY[k]) && (input.posW.y > gMinY[k]))
	{
		results[k].r = (finalCol.r - (finalCol.r * dist)) + grey * dist;
		results[k].g = (finalCol.g - (finalCol.g * dist)) + grey * dist;
		results[k].b = (finalCol.b - (finalCol.b * dist)) + grey * dist;
		results[k].a = finalCol.a;
		//return finalCol;
		//return float4(final.r, final.g, final.b, 1.0f);
		//return result;
	}
	else
	{
		results[k] = float4(grey, grey, grey, 1.0f);
		//return float4(grey, grey, grey, 1.0f);
	}
	
	}
	
	float4 result;
	
	for( int k = 0; k < 10; k++ )
	{
		result.r += results[k].r;
		result.g += results[k].g;
		result.b += results[k].b;
	}
	
	result.r = result.r / 10;
	result.g = result.g / 10;
	result.b = result.b / 10;
	result.a = 1.0f;
	
	return result;
}

float4 LightsPSWithShadows(InputPS input): COLOR
{

	///////////////////////////////////////////////////////
	///calculations for shadow mapping					///
	///////////////////////////////////////////////////////
	float4 lightingPos = mul(input.posW, gLightviewproj);
	
	float2 shadowtexcoord = 0.5*lightingPos.xy/lightingPos.w + float2(0.5,0.5);
	
	shadowtexcoord.y =  1.0f - shadowtexcoord.y;
	
	float depth = lightingPos.z / lightingPos.w;
	
	float shadowdiff = 0.0f;
    
    shadowdiff += ShadowMapLookup(ShadowMapSampler, shadowtexcoord, float2(0.0f, 0.0f), depth);
	shadowdiff += ShadowMapLookup(ShadowMapSampler, shadowtexcoord, float2(1.0f, 0.0f), depth);
	shadowdiff += ShadowMapLookup(ShadowMapSampler, shadowtexcoord, float2(2.0f, 0.0f), depth);
	
	shadowdiff += ShadowMapLookup(ShadowMapSampler, shadowtexcoord, float2(0.0f, 1.0f), depth);
	shadowdiff += ShadowMapLookup(ShadowMapSampler, shadowtexcoord, float2(1.0f, 1.0f), depth);
	shadowdiff += ShadowMapLookup(ShadowMapSampler, shadowtexcoord, float2(2.0f, 1.0f), depth);
	
	shadowdiff += ShadowMapLookup(ShadowMapSampler, shadowtexcoord, float2(0.0f, 2.0f), depth);
    shadowdiff += ShadowMapLookup(ShadowMapSampler, shadowtexcoord, float2(1.0f, 2.0f), depth);
	shadowdiff += ShadowMapLookup(ShadowMapSampler, shadowtexcoord, float2(2.0f, 2.0f), depth);
	
	shadowdiff /= 9.0f;
   
	////////////////////////////////////////////////////////
	

	float			xCom;
	float			yCom;
	float			zCom;
	float grey;
	float4 result;
	int counter = 0;
	
	result.r = 0;
    result.g = 0;
    result.b = 0;
    result.a = 1.0f;
	
	float3 Color = {0.0f, 0.0f, 0.0f};
	float4 texColor = tex2D(TexS, input.tex0);
	float4 globalamb ={0.5, 0.5, 0.5, 1.0};
	
	for (int i = 0; i < (numlights); ++i)
	{
		Color += CalculatePointLightWShadow(light[i], input, texColor, globalamb, shadowdiff);
	}
	
	float4 finalCol = float4( Color, gDiffuseMtrl.a*texColor.a);
	
	////////////////////////////////////////////////////////////
	///calculations for colour change						 ///
	////////////////////////////////////////////////////////////	

	grey = (float3)dot(float3(finalCol.r,finalCol.g,finalCol.b), float3(0.212671f, 0.715160f, 0.072169f));
	result.r = 0;
	result.g = 0;
	result.b = 0;
	result.a = finalCol.a;
	
	if(player)
	{
		float dist = 1 - ( health / 100 );
		result.r = ((finalCol.r - (finalCol.r * dist)) + grey * dist);
		result.g = ((finalCol.g - (finalCol.g * dist)) + grey * dist);
		result.b = ((finalCol.b - (finalCol.b * dist)) + grey * dist);
		result.a = finalCol.a;
		return result;
	}
	else
	{
	
	for(int k = 0; k < 10; k++)
	{
		xCom = (input.posW.x - gCenterX[k])*(input.posW.x - gCenterX[k]);
		yCom = (input.posW.y - gCenterY[k])*(input.posW.y - gCenterY[k]);
		zCom = (input.posW.z - gCenterZ[k])*(input.posW.z - gCenterZ[k]);
		
		float dist = (xCom + zCom) / gRadius[k];
		
		if (((xCom + zCom) <= (gRadius[k])) && (input.posW.y < gMaxY[k]) && (input.posW.y > gMinY[k]))
		{
			if ( (result.r < ((finalCol.r - (finalCol.r * dist)) + grey * dist)) )
			result.r = ((finalCol.r - (finalCol.r * dist)) + grey * dist);
			if ( (result.g < ((finalCol.g - (finalCol.g * dist)) + grey * dist)) )
			result.g = ((finalCol.g - (finalCol.g * dist)) + grey * dist);
			if ( (result.b < ((finalCol.b - (finalCol.b * dist)) + grey * dist)) )
			result.b = ((finalCol.b - (finalCol.b * dist)) + grey * dist);
			//result.a = finalCol.a;
			counter++;
		}
	}
		
	if(counter == 0)
	{
		result.r = grey;
		result.g = grey;
		result.b = grey;
	}
	
	return result;
	}
}

technique MyTech
{
	pass PointLights
	{ 
		SpecularEnable = TRUE;
		AlphaBlendEnable = TRUE;
		
		vertexShader = compile vs_3_0 LightsVS();
		pixelShader = compile ps_3_0 LightsPSWithShadows();
		//specify render device states associated with the pass
		FillMode =  Solid;
	}	
}

technique CreateShadowMapTech
{
	pass CreateShadowMap
	{
		vertexShader = compile vs_2_0 CreateShadowMapVS();
		pixelShader = compile ps_2_0 CreateShadowMapPS();
	}

}


