// ---------------------------------------------------------
// Sombras en el image space con la tecnica de Shadows Map
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

#define SMAP_SIZE 1024
#define EPSILON 0.05f

float time = 0;

float4x4 g_mViewLightProj;
float4x4 g_mProjLight;
float3   g_vLightPos;  // posicion de la luz (en World Space) = pto que representa patch emisor Bj
float3   g_vLightDir;  // Direcion de la luz (en World Space) = normal al patch Bj

float4x4 g_mViewLightProjSalto;
float3   g_vLightPosSalto;  // posicion de la luz (en World Space) = pto que representa patch emisor Bj
float3   g_vLightDirSalto;  // Direcion de la luz (en World Space) = normal al patch Bj

texture  g_txShadowSalto;	// textura para el shadow map
sampler2D g_samShadowSalto =
sampler_state
{
	Texture = <g_txShadowSalto>;
	MinFilter = Point;
	MagFilter = Point;
	MipFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

texture  g_txShadow;	// textura para el shadow map
sampler2D g_samShadow =
sampler_state
{
	Texture = <g_txShadow>;
	MinFilter = Point;
	MagFilter = Point;
	MipFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position :        POSITION0;
	float2 Texcoord :        TEXCOORD0;
	float3 Norm :			TEXCOORD1;		// Normales
	float3 Pos :   			TEXCOORD2;		// Posicion real 3d
};

//-----------------------------------------------------------------------------
// Vertex Shader que implementa un shadow map
//-----------------------------------------------------------------------------
void VertShadow(float4 Pos : POSITION,
	float3 Normal : NORMAL,
	out float4 oPos : POSITION,
	out float2 Depth : TEXCOORD0)
{
	// transformacion estandard
	oPos = mul(Pos, matWorld);					// uso el del mesh
	oPos = mul(oPos, g_mViewLightProj);		// pero visto desde la pos. de la luz

	// devuelvo: profundidad = z/w
	Depth.xy = oPos.zw;
}

//-----------------------------------------------------------------------------
// Pixel Shader para el shadow map, dibuja la "profundidad"
//-----------------------------------------------------------------------------
void PixShadow(float2 Depth : TEXCOORD0, out float4 Color : COLOR)
{
	Color = Depth.x / Depth.y;
}

////////////////////////////////////////////////////////////////////////////////////////
//-----------------------------------------------------------------------------
// Vertex Shader que implementa un shadow map
//-----------------------------------------------------------------------------
void VertShadowSalto(float4 Pos : POSITION,
	float3 Normal : NORMAL,
	out float4 oPos : POSITION,
	out float2 Depth : TEXCOORD0)
{
	// transformacion estandard
	oPos = mul(Pos, matWorld);					// uso el del mesh
	oPos = mul(oPos, g_mViewLightProjSalto);		// pero visto desde la pos. de la luz

	// devuelvo: profundidad = z/w
	Depth.xy = oPos.zw;
}

//-----------------------------------------------------------------------------
// Pixel Shader para el shadow map, dibuja la "profundidad"
//-----------------------------------------------------------------------------
void PixShadowSalto(float2 Depth : TEXCOORD0, out float4 Color : COLOR)
{
	Color = Depth.x / Depth.y;
}

technique RenderShadow
{
	pass p0
	{
		VertexShader = compile vs_3_0 VertShadow();
		PixelShader = compile ps_3_0 PixShadow();
	}
}

technique RenderShadowSalto
{
	pass p0
	{
		VertexShader = compile vs_3_0 VertShadowSalto();
		PixelShader = compile ps_3_0 PixShadow();
	}
}

//-----------------------------------------------------------------------------
// Vertex Shader para dibujar la escena pp dicha con sombras
//-----------------------------------------------------------------------------
void VertScene(float4 iPos : POSITION,
	float2 iTex : TEXCOORD0,
	float3 iNormal : NORMAL,
	out float4 oPos : POSITION,
	out float2 Tex : TEXCOORD0,
	out float4 vPos : TEXCOORD1,
	out float3 vNormal : TEXCOORD2,
	out float4 vPosLight : TEXCOORD3,
	out float4 vPosLightSalto : TEXCOORD4
)
{
	// transformo al screen space
	oPos = mul(iPos, matWorldViewProj);

	// propago coordenadas de textura
	Tex = iTex;

	// propago la normal
	vNormal = mul(iNormal, (float3x3)matWorldView);

	// propago la posicion del vertice en World space
	vPos = mul(iPos, matWorld);
	
	// propago la posicion del vertice en el espacio de proyeccion de la luz
	vPosLight = mul(vPos, g_mViewLightProj);
	
	vPosLightSalto = mul(vPos, g_mViewLightProjSalto);
}

//-----------------------------------------------------------------------------
// Pixel Shader para dibujar la escena
//-----------------------------------------------------------------------------
float4 PixScene(float2 Tex : TEXCOORD0,
	float4 vPos : TEXCOORD1,
	float3 vNormal : TEXCOORD2,
	float4 vPosLight : TEXCOORD3,
	float4 vPosLightSalto : TEXCOORD4
) :COLOR
{
	float3 vLight = normalize(float3(vPos - g_vLightPos));
	float cono = dot(vLight, g_vLightDir);
	float4 K = 0.0;
	float4 color_base = tex2D(diffuseMap, Tex);

	if (cono > 0.7 && vPosLight.z < 650)
	{
		//coordenada de textura CT
		float2 CT = 0.5 * vPosLight.xy / vPosLight.w + float2(0.5, 0.5);
		CT.y = 1.0f - CT.y;
		
		// sin ningun aa. conviene con smap size >= 512
		float I = (tex2D(g_samShadow, CT) + EPSILON < vPosLight.z / vPosLight.w) ? 0.0f : 1.0f;

		if (cono < 0.8)
			I *= 1 - (0.8 - cono) * 10;

		K = I;
	}

	color_base.rgb *= 0.6 + 0.5*K;
	
	float3 vLightSalto = normalize(float3(vPos - g_vLightPosSalto));
	float conoSalto = dot(vLightSalto, g_vLightDirSalto);
	float4 KSalto = 0.0;
	float4 color_baseSalto = tex2D(diffuseMap, Tex);

	if (conoSalto > 0.95)
	{
		//coordenada de textura CT
		float2 CT = 0.5 * vPosLightSalto.xy / vPosLightSalto.w + float2(0.5, 0.5);
		CT.y = 1.0f - CT.y;
		
		float I = (tex2D(g_samShadowSalto, CT) + EPSILON < vPosLightSalto.z / vPosLightSalto.w) ? 0.0f : 0.1f;
		KSalto = I;
		
		if (I  == 0)
			color_base.rgb *= 0.3;
		/*else
			color_base.rgb = 1;*/
	}
	
	return color_base;
}

//-----------------------------------------------------------------------------
// Vertex Shader para dibujar la escena pp dicha con sombras
//-----------------------------------------------------------------------------
void VertSceneSalto(float4 iPos : POSITION,
	float2 iTex : TEXCOORD0,
	float3 iNormal : NORMAL,
	out float4 oPos : POSITION,
	out float2 Tex : TEXCOORD0,
	out float4 vPos : TEXCOORD1,
	out float3 vNormal : TEXCOORD2,
	out float4 vPosLight : TEXCOORD3
)
{
	// transformo al screen space
	oPos = mul(iPos, matWorldViewProj);

	// propago coordenadas de textura
	Tex = iTex;

	// propago la normal
	vNormal = mul(iNormal, (float3x3)matWorldView);

	// propago la posicion del vertice en World space
	vPos = mul(iPos, matWorld);
	// propago la posicion del vertice en el espacio de proyeccion de la luz
	vPosLight = mul(vPos, g_mViewLightProjSalto);
}

//-----------------------------------------------------------------------------
// Pixel Shader para dibujar la escena
//-----------------------------------------------------------------------------
float4 PixSceneSalto (float2 Tex : TEXCOORD0,
	float4 vPos : TEXCOORD1,
	float3 vNormal : TEXCOORD2,
	float4 vPosLight : TEXCOORD3
) :COLOR
{
	float3 vLight = normalize(float3(vPos - g_vLightPosSalto));
	float cono = dot(vLight, g_vLightDirSalto);
	float4 K = 0.0;
	float4 color_base = tex2D(diffuseMap, Tex);

	if (cono > 0.9)
	{
		//coordenada de textura CT
		float2 CT = 0.5 * vPosLight.xy / vPosLight.w + float2(0.5, 0.5);
		CT.y = 1.0f - CT.y;
		
		float I = (tex2D(g_samShadowSalto, CT) + EPSILON < vPosLight.z / vPosLight.w) ? 0.0f : 0.1f;
		K = I;
		
		if (I  > 0)
		{
			K = 0;
			color_base.rgb *= 0.6 + K;
		}
		else
			color_base.rgb *= 0;
	}
	else
		color_base.rgb *= 0.6f;
	
	return color_base;
}

technique RenderScene
{
	pass p0
	{
		VertexShader = compile vs_3_0 VertScene();
		PixelShader = compile ps_3_0 PixScene();
	}
}

technique RenderScene2
{
	pass p0
	{
		VertexShader = compile vs_3_0 VertSceneSalto();
		PixelShader = compile ps_3_0 PixSceneSalto();
	}
}