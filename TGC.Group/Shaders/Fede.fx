float4x4 matWorld;
float4x4 matWorldView;
float4x4 matWorldViewProj;
float4x4 matInverseTransposeWorld;

extern uniform float screen_dx = 1024;
extern uniform float screen_dy = 768;

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

struct VertexInput
{
	float4 Position : POSITION0;
	float4 TexCoord : TEXCOORD0;
	float4 Color : COLOR;
};

struct VertexOutput
{
	float4 Position : POSITION0;
	float4 TexCoord : TEXCOORD0;
	float4 PositionForPixel : TEXCOORD1;
	float4 Color : COLOR;
};

VertexOutput main_vertex(VertexInput input)
{
	VertexOutput output;
	output.Position = mul(input.Position, matWorldViewProj);
	output.TexCoord = input.TexCoord;
	output.PositionForPixel = input.Position;
	output.Color = input.Color;

	return output;
}

extern uniform float farness;
extern uniform float maxFarness;

float4 fromRGB(float r, float g, float b)
{
	return float4(r / 255.0, g / 255.0, b / 255.0, 1);
}

float4 main_pixel(VertexOutput input) : COLOR0
{
	float4 waterColor = fromRGB(90, 106, 165);
	float depth = (farness / maxFarness) * 1.5;
	float4 lightenedColor = waterColor * float4(depth, depth, depth, 1);
	float4 texColor = tex2D(diffuseMap, input.TexCoord);

	float k = 1;
	return lerp(texColor, lightenedColor, depth) * float4(k, k, k, 1);
	/*return lightenedColor;*/
}
//float4 main_pixel(VertexOutput input) : COLOR0
//{
//	return float4(
//		4 / 255.0,
//		56/ 255.0,
//		80/ 255.0, 1) * tex2D(diffuseMap, input.TexCoord);
//}
//float4 main_pixel(VertexOutput input) : COLOR0
//{
//	return tex2D(diffuseMap, input.TexCoord);
//}
technique FedeTechnique
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 main_vertex();
		PixelShader = compile ps_3_0 main_pixel();
	}
}
