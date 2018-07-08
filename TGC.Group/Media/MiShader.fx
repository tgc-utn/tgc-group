//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

float screen_dx = 1024;
float screen_dy = 768;

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

texture g_RenderTarget;
sampler RenderTarget =
sampler_state
{
	Texture = <g_RenderTarget>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

texture g_ImagenOriginal;
sampler ImagenOriginal =
sampler_state
{
	Texture = <g_ImagenOriginal>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

float time = 0;

/**************************************************************************************/
/* RenderScene */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float2 RealPos : TEXCOORD1;
    float4 Color : COLOR0;
};

float a = 0;

//Vertex Shader
VS_OUTPUT vs_main(VS_INPUT Input)
{
    VS_OUTPUT Output;
	
	Output.RealPos = Input.Position;
	
	//Proyectar posicion
    Output.Position = mul(Input.Position, matWorldViewProj);
   
	//Propago las coordenadas de textura
    Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
    Output.Color = Input.Color;

    return (Output);
}

float frecuencia = 10;
//Pixel Shader
float4 ps_main(VS_OUTPUT Input) : COLOR0
{
    float4 a;
	a.r = 1;
	a.g = 0.4;
	a.b = 0.6;
	a.a = 1;
    return a;
}

void VSCopy(float4 vPos : POSITION, float2 vTex : TEXCOORD0, out float4 oPos : POSITION, out float2 oScreenPos : TEXCOORD0)
{
	oPos = vPos;
	oScreenPos = vTex;
	oPos.w = 1;
}


static const int kernel_r = 6;
static const int kernel_size = 13;
static const float Kernel[kernel_size] =
{
	0.012216, 0.018764, 0.036995, 0.074759, 0.130985, 0.186033, 0.209471, 0.186033, 0.130985, 0.074759, 0.036995, 0.018764, 0.012216,
};

void Blur(float2 screen_pos : TEXCOORD0, out float4 Color : COLOR)
{
	Color = 0;
	for (int i = 0; i < kernel_size; ++i)
		for (int j = 0; j < kernel_size; ++j)
			Color += tex2D(RenderTarget, screen_pos + float2((float)(i - kernel_r) / screen_dx, (float)(j - kernel_r) / screen_dy)) * Kernel[i] * Kernel[j];
	Color.a = 1;
}

void Merge(float2 screen_pos : TEXCOORD0, out float4 Color : COLOR)
{
	float4 ColorBlur = tex2D(RenderTarget, screen_pos);
	ColorBlur.a = 1;
	if (ColorBlur.r == 1 && ColorBlur.g == 0.4 && ColorBlur.b == 0.6) {
		Color = ColorBlur;
	}
	else {
			//ColorOriginal 
		Color = tex2D(ImagenOriginal, screen_pos);
	}
}


// ------------------------------------------------------------------
technique Rojo
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main();
        PixelShader = compile ps_3_0 ps_main();
    }
}

technique Blur
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 VSCopy();
        PixelShader = compile ps_3_0 Blur();
    }
}

technique Bloom
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 VSCopy();
		PixelShader = compile ps_3_0 Merge();
	}
}

technique Directo
{
	pass Pass_0
	{

	}
}