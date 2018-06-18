// ---------------------------------------------------------
// Ejemplo toon Shading
// ---------------------------------------------------------

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

float screen_dx; // tamaño de la pantalla en pixels
float screen_dy;
float KLum = 1; // factor de luminancia

//Input del Vertex Shader
struct VS_INPUT
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float4 Color : COLOR;
    float2 Texcoord : TEXCOORD0;
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

texture g_GlowMap;
sampler GlowMap =
sampler_state
{
    Texture = <g_GlowMap>;
    ADDRESSU = CLAMP;
    ADDRESSV = CLAMP;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;
};


// textura de 1 x 1 que tiene el valor promedio de luminance
texture g_Luminance;
sampler Luminance =
sampler_state
{
    Texture = <g_Luminance>;
    ADDRESSU = CLAMP;
    ADDRESSV = CLAMP;
    MINFILTER = POINT;
    MAGFILTER = POINT;
    MIPFILTER = POINT;
};

// textura de 1 x 1 que tiene el valor promedio de luminance
texture g_Luminance_ant;
sampler Luminance_ant =
sampler_state
{
    Texture = <g_Luminance_ant>;
    ADDRESSU = CLAMP;
    ADDRESSV = CLAMP;
    MINFILTER = POINT;
    MAGFILTER = POINT;
    MIPFILTER = POINT;
};

// Depth of field
float zn = 1; // near plane
float zf = 10000; // far plane
float zfoco = 300; // focus plane
float blur_k = 0.5; // factor de desenfoque

texture g_BlurFactor;
sampler BlurFactor =
sampler_state
{
    Texture = <g_BlurFactor>;
    MipFilter = Point;
    MinFilter = Point;
    MagFilter = Point;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float3 Norm : TEXCOORD1; // Normales
    float3 Pos : TEXCOORD2; // Posicion real 3d
};

//Vertex Shader
VS_OUTPUT vs_main(VS_INPUT Input)
{
    VS_OUTPUT Output;

	//Proyectar posicion
    Output.Position = mul(Input.Position, matWorldViewProj);

	//Las Texcoord quedan igual
    Output.Texcoord = Input.Texcoord;

	// Calculo la posicion real
    float4 pos_real = mul(Input.Position, matWorld);
    Output.Pos = float3(pos_real.x, pos_real.y, pos_real.z);

	// Transformo la normal y la normalizo
	//Output.Norm = normalize(mul(Input.Normal,matInverseTransposeWorld));
    Output.Norm = normalize(mul(Input.Normal, matWorld));
    return (Output);
}

//Pixel Shader
float4 ps_main(float3 Texcoord : TEXCOORD0, float3 N : TEXCOORD1,
	float3 Pos : TEXCOORD2) : COLOR0
{
	//Obtener el texel de textura
    float4 fvBaseColor = tex2D(diffuseMap, Texcoord);
	// aplico el factor de luminancia
    fvBaseColor.rgb *= KLum;
    return fvBaseColor;
}

technique DefaultTechnique
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main();
        PixelShader = compile ps_3_0 ps_main();
    }
}

void VSCopy(float4 vPos : POSITION, float2 vTex : TEXCOORD0, out float4 oPos : POSITION, out float2 oScreenPos : TEXCOORD0)
{
    oPos = vPos;
    oScreenPos = vTex;
    oPos.w = 1;
}

// Gaussian Blur

static const int kernel_r = 6;
static const int kernel_size = 13;
static const float Kernel[kernel_size] =
{
    0.002216, 0.008764, 0.026995, 0.064759, 0.120985, 0.176033, 0.199471, 0.176033, 0.120985, 0.064759, 0.026995, 0.008764, 0.002216,
};

void Blur(float2 screen_pos : TEXCOORD0, out float4 Color : COLOR)
{
    Color = 0;
	for (int i = 0; i < kernel_size; ++i){
		for (int j = 0; j < kernel_size; ++j){
			
			//float4 smooth = tex2D(RenderTarget, screen_pos);
			//if(smooth.r > 0.9 && smooth.g < 0.25){
				float4 distort = tex2D(RenderTarget, screen_pos + float2((float) (i - kernel_r) / screen_dx, (float) (j - kernel_r) / screen_dy)) * Kernel[i] * Kernel[j];
				Color += distort;
			//}
			//else{
			//	Color += smooth;
			//}
		}
	}
    Color.a = 1;
}

technique GaussianBlur
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 VSCopy();
        PixelShader = compile ps_3_0 Blur();
    }
}

static const float4 LUM_VECTOR = float4(.299, .587, .114, 0);
static const float MIDDLE_GRAY = 0.72f;
static const float LUM_WHITE = 1.5f;
float pupila_time = 0;
bool glow = true;
int tone_mapping_izq = 1;
int tone_mapping_der = 0;
bool pantalla_completa = true;

// --------------------------------------------------------------------------------
struct VS_OUTPUT2
{
    float4 Position : POSITION; // vertex position 
    float4 Pos : TEXCOORD0; // distancia a la camara
};