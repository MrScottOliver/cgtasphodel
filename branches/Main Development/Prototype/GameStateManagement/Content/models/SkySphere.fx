uniform extern float4x4 mView;
uniform extern float4x4 mProjection;

void SkyboxVertexShader( float3 pos : POSITION0,
                         out float4 SkyPos : POSITION0,
                         out float3 SkyCoord : TEXCOORD0 )
{
    // Calculate rotation. Using a float3 result, so translation is ignored
    float3 rotatedPosition = mul(pos, mView);           
    // Calculate projection, moving all vertices to the far clip plane 
    // (w and z both 1.0)
    SkyPos = mul(float4(rotatedPosition, 1), mProjection).xyww;    

    SkyCoord = pos;
};
uniform extern texture gEnvMap;
sampler SkyboxS = sampler_state
{
    Texture = <gEnvMap>;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
};
float4 SkyboxPixelShader( float3 SkyCoord : TEXCOORD0 ) : COLOR
{
	float grey;
	
    // grab the pixel color value from the skybox cube map
    float4 finalCol = texCUBE(SkyboxS, SkyCoord);
    
    grey = (float3)dot(float3(finalCol.r,finalCol.g,finalCol.b), float3(0.212671f, 0.715160f, 0.072169f));
	return float4(grey, grey, grey, 1.0f);
};
technique SkyboxTechnique
{
    pass P0
    {
        vertexShader = compile vs_2_0 SkyboxVertexShader();
        pixelShader = compile ps_2_0 SkyboxPixelShader();

        // We're drawing the inside of a model
        CullMode = None;  
        // We don't want it to obscure objects with a Z < 1
        ZWriteEnable = false; 
    }
}
