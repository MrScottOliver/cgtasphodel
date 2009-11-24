//-------------------------------------------------------------------------------
//Vertex and pixel shader demonstrating ambient, diffuse and specular lighting
// Ambient intensity values are pre baked
//------------------------------------------------------------------------------------------------------

//------------------------------------------------------------------------------------------------------
// Global params provided by the app
//------------------------------------------------------------------------------------------------------
uniform extern float4x4 gWVP			: WORLDVIEWPROJ;
uniform extern float4x4 gWorldViewIT	: WVPIT;
uniform extern float4x4 gWorldView		: WV;

uniform extern float4 gDiffuseMtrl	: DIFFMTRL;
uniform extern float4 gAmbMtrl		: AMBMTRL;
uniform extern float4 gSpecMtrl		: SPECMTRL;
uniform extern float4 gLightCol		: LIGHTCOL;
uniform extern float4 gLightDir;

uniform extern texture gTex : TEX;

uniform extern float	gCenterX;
uniform extern float	gCenterY;
uniform extern float	gRadius;


sampler TexS = sampler_state
{
	Texture = <gTex>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	MaxAnisotropy = 8;
	AddressU  = WRAP;
    AddressV  = WRAP;
};

// light intensity
float4 I_a = { 1.0f, 1.0f, 1.0f, 1.0f };    // ambient 
float4 I_d = { 1.0f, 1.0f, 1.0f, 1.0f };    // diffuse
float4 I_s = { 1.0f, 1.0f, 1.0f, 1.0f };    // specular


float  n   : MATERIALPOWER = 16.0f;         // power
//---------------------------------------------------------------------------------------------------------------------------
// Input channel (vertex shader)
//---------------------------------------------------------------------------------------------------------------------------
struct InputVS
{
	float3 posH : POSITION0;
	float3 Norm : NORMAL;
	float2 tex0 : TEXCOORD0;
};
//---------------------------------------------------------------------------------------------------------------------------
// Output channel (vertex shader)
//---------------------------------------------------------------------------------------------------------------------------
struct OutputVS
{
	float4 posH : POSITION0;
	float3 col: COLOR0;
	float3 col_amb: TEXCOORD0;
	float3 col_diff: TEXCOORD1;
	float3 col_spec: TEXCOORD2;
	float3 EV: TEXCOORD3;	
	float3 N: TEXCOORD4;
	float3 E: TEXCOORD5;
	float2 tex0: TEXCOORD6;

};
//---------------------------------------------------------------------------------------------------------------------------
// Vertex shader
//---------------------------------------------------------------------------------------------------------------------------

OutputVS PhongShaderVS(InputVS input)
{
	//Zero out our output
	OutputVS outVS = (OutputVS)0;
	
	//Transform to homogeneous clipspace
	outVS.posH = mul(float4(input.posH, 1.0f), gWVP);
	//eye position 
	outVS.EV = mul(input.posH,gWorldView).xyz;
	//normals
	outVS.N =mul(input.Norm,gWorldViewIT).xyz;
	//eye vector
	outVS.E=normalize(outVS.EV);
		
	outVS.col_amb =  I_a*gAmbMtrl*gLightCol;
	outVS.col_diff = I_d*gDiffuseMtrl*gLightCol;
	outVS.col_spec = I_s*gSpecMtrl*gLightCol;
		
	// Pass on texture coordinates to be interpolated in rasterization.
	outVS.tex0 = input.tex0;

	
	//return output
	return outVS;
}
//---------------------------------------------------------------------------------------------------------------------------
// Input channel pixel shader
//---------------------------------------------------------------------------------------------------------------------------
struct InputPS
{
	float2 pos: VPOS;
	float4 col: COLOR0;
	float4 col_amb: TEXCOORD0;
	float4 col_diff: TEXCOORD1;
	float4 col_spec: TEXCOORD2;
	float3 EV: TEXCOORD3;
	float3 N: TEXCOORD4;
	float3 E: TEXCOORD5;
	float2 tex0: TEXCOORD6;
};
//---------------------------------------------------------------------------------------------------------------------------
// Pixel shader (input channel):output channel
//---------------------------------------------------------------------------------------------------------------------------
float4 PhongShaderPS(InputPS input): COLOR
{
	
	float			xCom;
	float			yCom;
	float grey;

	xCom = (input.pos.x - gCenterX)*(input.pos.x - gCenterX);
	yCom = (input.pos.y - gCenterY)*(input.pos.y - gCenterY);

	//transform light vector into viewspace and normalize
	float4 finalColour=0;
	

	float3 L=-normalize(mul(gLightDir,gWorldView));
	
	
	//half angle vector used in approximation to specular component
	float3 H=normalize(input.E+L);
	
	//ambient term	
	float4 ambient={0.4, 0.4, 0.4, 1.00f};


	//Calculate diff, specular and ambient components of lighht
	float diff = max(0, dot(input.N,L));
    
	//specular component using Blinns half angle
	//specular component using haf angle
	float spec = pow(max(0, dot(input.N, H)), n);	
	float4 texColor = tex2D(TexS, input.tex0);	
	
	float4 finalCol =input.col_amb + input.col_spec*spec + input.col_diff*diff*texColor;
	finalCol.a=1.0;
	//return finalCol;	

	if ((xCom + yCom) <= (gRadius))
	{
		return finalCol;
	}
	else
	{
		grey = (float3)dot(float3(finalCol.r,finalCol.g,finalCol.b), float3(0.212671f, 0.715160f, 0.072169f));
		return float4(grey, grey, grey, 1.0f);
		//return finalCol;
	}
}

float4 SimpleLightPS(InputPS input): COLOR
{
//transform light vector into viewspace and normalize light vector and normals
	float3 L=-normalize(mul(gLightDir,gWorldView));
	input.N =normalize(input.N).xyz;
	
	//half angle vector used in approximation to specular component
	float3 H=normalize(input.E+L);
	
	//ambient term	
	float4 ambient={0.7, 0.7, 0.7, 1.0};

	//Calculate diff, specular and ambient components of lighht
	float diff = max(0, dot(input.N,L));
    
	//specular component using Blinns half angle
	float spec = pow(max(0, dot(input.N, H)), n);		
	float4 texColor = tex2D(TexS, input.tex0);	
	
	float4 finalCol =input.col_diff*diff*texColor + input.col_amb*ambient*texColor+ input.col_spec*spec;  
	finalCol.a=0.9;
	
	return finalCol;

}


technique PhongShaderTech
{
	pass P0
	{ 
	    Lighting       = TRUE;
        SpecularEnable = TRUE;
		vertexShader = compile vs_2_0 PhongShaderVS();
		pixelShader = compile ps_3_0 PhongShaderPS();
		//specify render device states associated with the pass
		FillMode = Solid;// WireFrame;
	}
}

technique TestLightTech
{
	pass P0
	{ 
	    Lighting       = TRUE;
        SpecularEnable = TRUE;
		vertexShader = compile vs_2_0 PhongShaderVS();
		pixelShader = compile ps_3_0 SimpleLightPS();
		//specify render device states associated with the pass
		FillMode = Solid;// WireFrame;
	}
}




