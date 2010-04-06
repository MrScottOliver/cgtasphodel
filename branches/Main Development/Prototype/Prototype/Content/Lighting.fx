//-------------------------------------------------------------------------------
//Shader that includes directional and point lights (vertex shader and other functions in Includes.inc)
//------------------------------------------------------------------------------------------------------
#include "Includes.inc"



uniform extern int numlights;//number of lights

uniform extern Lighting light[8];


uniform extern float	gCenterX;
uniform extern float	gCenterY;
uniform extern float	gCenterZ;
uniform extern float	gRadius;
uniform extern float gMinY;
uniform extern float gMaxY;


//---------------------------------------------------------------------------------------------------------------------------
// Pixel shader (input channel):output channel
//---------------------------------------------------------------------------------------------------------------------------
float4 LightsPS(InputPS input): COLOR
{
	float			xCom;
	float			yCom;
	float			zCom;
	float grey;
	
	xCom = (input.posW.x - gCenterX)*(input.posW.x - gCenterX);
	yCom = (input.posW.y - gCenterY)*(input.posW.y - gCenterY);
	zCom = (input.posW.z - gCenterZ)*(input.posW.z - gCenterZ);
	
	
	float3 Color = {0.0f, 0.0f, 0.0f};
	float4 texColor = tex2D(TexS, input.tex0);
	float4 globalamb ={0.5, 0.5, 0.5, 1.0};
	
	
	for (int i = 0; i < (numlights); ++i)
	{
	
		if(light[i].gtype == 1)
		{
			Color += CalculatePointLight(light[i], input, texColor, globalamb);
		}
		//else if(light[i].gtype == 0)
		//{
		//	Color += CalculateDirLight(input, texColor, globalamb);
		//}
	
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


	pass PointLights
	{ 
		SpecularEnable = TRUE;
		AlphaBlendEnable = TRUE;
		
		vertexShader = compile vs_3_0 LightsVS();
		pixelShader = compile ps_3_0 LightsPS();
		//specify render device states associated with the pass
		FillMode =  Solid;
		


	}	

}


