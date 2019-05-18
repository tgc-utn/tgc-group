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
	float4 Color : COLOR;
};

struct VertexOutput
{
	float4 Position : POSITION0;
	float4 PositionForPixel : TEXCOORD0;
	float4 Color : COLOR;
};

VertexOutput main_vertex(VertexInput input)
{
	VertexOutput output;
	output.Position = mul(input.Position, matWorldViewProj);
	output.PositionForPixel = output.Position;
	output.Color = float4(1, 1, 1, 1);

	return output;
}

float4 main_pixel(VertexOutput input) : COLOR0
{
	return float4(1, 1, 1, 1);
}

technique FedeTechnique
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 main_vertex();
		PixelShader = compile ps_3_0 main_pixel();
	}
}
