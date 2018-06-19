//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state {
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

#define SMAP_SIZE 1024
#define EPSILON 0.05f

float g_shadowMapping;

//Matrix Pallette para skinning
static const int MAX_MATRICES = 26;
float4x3 bonesMatWorldArray[MAX_MATRICES];


//Input del Vertex Shader
struct VS_INPUT_DIFFUSE_MAP {
	float4 Position : POSITION0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
	float3 Normal :   NORMAL0;
	float3 Tangent : TANGENT0;
	float3 Binormal : BINORMAL0;
	float4 BlendWeights : BLENDWEIGHT;
	float4 BlendIndices : BLENDINDICES;
};

//Output del Vertex Shader
struct VS_OUTPUT_DIFFUSE_MAP {
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 Texcoord : TEXCOORD0;
	float3 WorldNormal : TEXCOORD1;
	float3 WorldTangent	: TEXCOORD2;
	float3 WorldBinormal : TEXCOORD3;
	float2 Depth : TEXCOORD4;
};

float4x4 g_mViewLightProj;
float4x4 g_mProjLight;
float3 g_vLightPos; // posicion de la luz (en World Space) = pto que representa patch emisor Bj
float3 g_vLightDir; // Direcion de la luz (en World Space) = normal al patch Bj

texture g_txShadow; // textura para el shadow map

sampler2D g_samShadow = sampler_state {
	Texture = <g_txShadow>;
	MinFilter = Point;
	MagFilter = Point;
	MipFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

//Vertex Shader
VS_OUTPUT_DIFFUSE_MAP vs_DiffuseMap(VS_INPUT_DIFFUSE_MAP input) {
	VS_OUTPUT_DIFFUSE_MAP output;

	//Pasar indices de float4 a array de int
	int BlendIndicesArray[4] = (int[4])input.BlendIndices;

	//Skinning de posicion
	float3 skinPosition = mul(input.Position, bonesMatWorldArray[BlendIndicesArray[0]]) * input.BlendWeights.x;;
	skinPosition += mul(input.Position, bonesMatWorldArray[BlendIndicesArray[1]]) * input.BlendWeights.y;
	skinPosition += mul(input.Position, bonesMatWorldArray[BlendIndicesArray[2]]) * input.BlendWeights.z;
	skinPosition += mul(input.Position, bonesMatWorldArray[BlendIndicesArray[3]]) * input.BlendWeights.w;

	//Skinning de normal
	float3 skinNormal = mul(input.Normal, (float3x3)bonesMatWorldArray[BlendIndicesArray[0]]) * input.BlendWeights.x;
	skinNormal += mul(input.Normal, (float3x3)bonesMatWorldArray[BlendIndicesArray[1]]) * input.BlendWeights.y;
	skinNormal += mul(input.Normal, (float3x3)bonesMatWorldArray[BlendIndicesArray[2]]) * input.BlendWeights.z;
	skinNormal += mul(input.Normal, (float3x3)bonesMatWorldArray[BlendIndicesArray[3]]) * input.BlendWeights.w;
	output.WorldNormal = normalize(skinNormal);

	//Skinning de tangent
	float3 skinTangent = mul(input.Tangent, (float3x3)bonesMatWorldArray[BlendIndicesArray[0]]) * input.BlendWeights.x;
	skinTangent += mul(input.Tangent, (float3x3)bonesMatWorldArray[BlendIndicesArray[1]]) * input.BlendWeights.y;
	skinTangent += mul(input.Tangent, (float3x3)bonesMatWorldArray[BlendIndicesArray[2]]) * input.BlendWeights.z;
	skinTangent += mul(input.Tangent, (float3x3)bonesMatWorldArray[BlendIndicesArray[3]]) * input.BlendWeights.w;
	output.WorldTangent = normalize(skinTangent);

	//Skinning de binormal
	float3 skinBinormal = mul(input.Binormal, (float3x3)bonesMatWorldArray[BlendIndicesArray[0]]) * input.BlendWeights.x;
	skinBinormal += mul(input.Binormal, (float3x3)bonesMatWorldArray[BlendIndicesArray[1]]) * input.BlendWeights.y;
	skinBinormal += mul(input.Binormal, (float3x3)bonesMatWorldArray[BlendIndicesArray[2]]) * input.BlendWeights.z;
	skinBinormal += mul(input.Binormal, (float3x3)bonesMatWorldArray[BlendIndicesArray[3]]) * input.BlendWeights.w;
	output.WorldBinormal = normalize(skinBinormal);

	output.Depth = float2(0, 0);
	if (g_shadowMapping < 1) {
		output.Position = mul(float4(skinPosition.xyz, 1.0), matWorldViewProj);
		output.Depth = float2(0, 0);
	} else {
		output.Position = mul(float4(skinPosition.xyz, 1.0), matWorld);
		output.Position = mul(output.Position, g_mViewLightProj);
		output.Depth.xy = output.Position.zw;
	}

	//Enviar color directamente
	output.Color = input.Color;

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;

	return output;
}

//Input del Pixel Shader
struct PS_DIFFUSE_MAP
{
	float4 Color : COLOR0;
	float2 Texcoord : TEXCOORD0;
	float2 Depth : TEXCOORD4;
};

//Pixel Shader
float4 ps_DiffuseMap(PS_DIFFUSE_MAP input) : COLOR0 {
	if (g_shadowMapping < 1) {
		return tex2D(diffuseMap, input.Texcoord) * input.Color;
	} else {
		float c = input.Depth.x / input.Depth.y;
		return float4(c, c, c, 1);
	}
	
}

/*
* Technique DIFFUSE_MAP
*/
technique DIFFUSE_MAP
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_DiffuseMap();
		PixelShader = compile ps_3_0 ps_DiffuseMap();
	}
}