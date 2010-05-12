//-------------------------------------------------------------------------------
//Shader that includes point lights and shadows (helper functions in Includes.inc)
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

uniform extern float health;

uniform extern bool withlights;
uniform extern bool withgrey;

uniform extern float orbRadius;
uniform extern int orbnum;
uniform extern float3 OrbPos[20]; 

uniform extern bool reveal;
uniform extern float transformDegree; 

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
	
	
	//return output
	return outVS;
}

//---------------------------------------------------------------------------------------------------------------------------
// Pixel shader (input channel):output channel
//---------------------------------------------------------------------------------------------------------------------------

float4 LightsPSWithShadows(InputPS input): COLOR
{

	float3 Color = {0.0f, 0.0f, 0.0f};
	float4 texColor = tex2D(TexS, input.tex0);
	float4 globalamb ={0.5, 0.5, 0.5, 1.0};
	float4 finalCol = {0.0f, 0.0f, 0.0f,0.0f};

	if(withlights)
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
		///Point light calc									////
		////////////////////////////////////////////////////////
	
		for (int i = 0; i < (numlights); ++i)
		{
			Color += CalculatePointLightWShadow(light[i], input, texColor, globalamb, shadowdiff);
		}
	
		finalCol = float4( Color, gDiffuseMtrl.a*texColor.a);
	
	}
	else
	{
		finalCol = texColor;
	}
	
	//////////////////////////////////////////////////////////////
	//orbs////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////
	float4 screenPos = mul(input.posW, gViewProj);
    screenPos /= screenPos.w;
    
    for (int i=0; i<orbnum; i++)
     {
         float4 orbscreenPos = mul(float4(OrbPos[i],1),gViewProj);
         orbscreenPos /= orbscreenPos.w;
         
         float dist = distance(screenPos.xy, orbscreenPos.xy);
         float radius = orbRadius/distance(gCamPosW, OrbPos[i]);
         if (dist < radius)
         {
             finalCol.rgb += (radius-dist)*8.0f;

        }
    }
	
	////////////////////////////////////////////////////////////
	///calculations for colour change						 ///
	////////////////////////////////////////////////////////////	
	
if(withgrey){	
	
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

	grey = (float3)dot(float3(finalCol.r,finalCol.g,finalCol.b), float3(0.212671f, 0.715160f, 0.072169f));
	result.r = 0;
	result.g = 0;
	result.b = 0;
	result.a = finalCol.a;
	
	if(reveal && ((finalCol.r >= transformDegree || finalCol.g >= transformDegree) || finalCol.b >= transformDegree ))
	{
		return finalCol;
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
		
	if(counter == 0)
	{
		result.r = grey;
		result.g = grey;
		result.b = grey;
	}
	
	return result;
	}
	}
	
}
else //if not with grey effect
{
	return finalCol;
}
	
	
	
}

float4 LightsPSPlayer(InputPS input): COLOR
{

	float3 Color = {0.0f, 0.0f, 0.0f};
	float4 texColor = tex2D(TexS, input.tex0);
	float4 globalamb ={0.5, 0.5, 0.5, 1.0};
	float4 finalCol = {0.0f, 0.0f, 0.0f,0.0f};


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
		///Point light calc									////
		////////////////////////////////////////////////////////
	
		for (int i = 0; i < (numlights); ++i)
		{
			Color += CalculatePointLightWShadow(light[i], input, texColor, globalamb, shadowdiff);
		}
	
		finalCol = float4( Color, gDiffuseMtrl.a*texColor.a);
	
	
	////////////////////////////////////////////////////////////
	///calculations for colour change						 ///
	////////////////////////////////////////////////////////////	
		
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

	grey = (float3)dot(float3(finalCol.r,finalCol.g,finalCol.b), float3(0.212671f, 0.715160f, 0.072169f));
	result.r = 0;
	result.g = 0;
	result.b = 0;
	result.a = finalCol.a;
	
	float dist = 1 - ( health / 100 );
	result.r = ((finalCol.r - (finalCol.r * dist)) + grey * dist);
	result.g = ((finalCol.g - (finalCol.g * dist)) + grey * dist);
	result.b = ((finalCol.b - (finalCol.b * dist)) + grey * dist);
	result.a = finalCol.a;
	return result;
	
	
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

technique PlayerTech
{
	pass Player
	{
		SpecularEnable = TRUE;
		AlphaBlendEnable = TRUE;
		
		vertexShader = compile vs_3_0 LightsVS();
		pixelShader = compile ps_3_0 LightsPSPlayer();
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


