
/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; 
float4x4 matWorldView;
float4x4 matWorldViewProj;
float4x4 matInverseTransposeWorld;

float screen_dx;
float screen_dy;

float4 PosicionPJ;

texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
    Texture = (texDiffuseMap);
    ADDRESSU = WRAP;
    ADDRESSV = WRAP;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;
};

float time = 0;

/**************************************************************************************/
/* Olas */
/**************************************************************************************/

//Input Vertex Shader
struct VS_INPUT
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 Texcoord : TEXCOORD0;
};

//Output Vertex Shader (Input Pixel Shader)
struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float2 RealPos : TEXCOORD1;
    float4 Color : COLOR0;
};

//Vertex Shader: Proyecta la posicion a pantalla y deja lo demas igual.
VS_OUTPUT vs_main(VS_INPUT Input)
{
    VS_OUTPUT Output;
    Output.RealPos = mul(Input.Position, matWorld);

    Output.Position = mul(Input.Position, matWorldViewProj);
   
    Output.Texcoord = Input.Texcoord;

    Output.Color = Input.Color;

    return (Output);
}

float frecuencia = 5;
float factorY = 10;
float factorX = -5;
//float epsilon = 0.1;

float4 ps_main(VS_OUTPUT Input) : COLOR0
{	
	if(length(PosicionPJ - Input.RealPos) < 200){
		return tex2D(diffuseMap, Input.Texcoord);
	}
	else{
		return 0;
	}
}

// ------------------------------------------------------------------
technique Vision
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main();
        PixelShader = compile ps_3_0 ps_main();
    }
}