/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
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

//Factor de translucidez
float alphaValue = 1;

/**************************************************************************************/
/* Pasto */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_Pasto
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_Pasto
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
};

//Vertex Shader
VS_OUTPUT_Pasto vs_Pasto(VS_INPUT_Pasto input)
{
	VS_OUTPUT_Pasto output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;

	return output;
}

//Input del Pixel Shader
struct PS_INPUT_Pasto
{
	float2 Texcoord : TEXCOORD0;
};

float nivel = 0; // Altura del pasto. Va del 0 la capa mas baja, al 1 la capa mas alta
float time = 0;
//Pixel Shader
float4 ps_Pasto(PS_INPUT_Pasto input) : COLOR0
{
    float4 color = tex2D(diffuseMap, input.Texcoord + sin(time) * nivel * .01f);
    if (color.r <= nivel)
        discard;
    color.r = 0;
    color.b /= 2;
    return color;
}


/*
* Technique Pasto
*/
technique Pasto
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_Pasto();
		PixelShader = compile ps_3_0 ps_Pasto();
	}
}
