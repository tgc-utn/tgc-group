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

texture g_txCubeMap;
samplerCUBE cubeMap = sampler_state
{
    Texture = (g_txCubeMap);
    ADDRESSU = WRAP;
    ADDRESSV = WRAP;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;
};

//Factor de translucidez
float alphaValue = 1;



/**************************************************************************************/
/* BlinnPhong */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_BLINN
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float4 Color : COLOR;
    float2 Texcoord : TEXCOORD0;
    float2 NormalCoord : TEXCOORD1;
};

//Output del Vertex Shader
struct VS_OUTPUT_BLINN
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float3 WorldNormal : TEXCOORD1;
    float3 LightVec : TEXCOORD2;
    float3 HalfAngleVec : TEXCOORD3;
    float3 ViewVec : TEXCOORD4;
    float3 WorldPosition : TEXCOORD5;
};

float3 eyePosition; // Posicion camara
float3 lightPosition; // Posicion luz
texture normal_map;
sampler2D normalMap =
sampler_state
{
    Texture = <normal_map>;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;
};

//Vertex Shader
VS_OUTPUT_BLINN vs_BlinnPhong(VS_INPUT_BLINN input)
{    
    VS_OUTPUT_BLINN output;

	//Proyectar posicion
    output.Position = mul(input.Position, matWorldViewProj);

	//Enviar Texcoord directamente
    output.Texcoord = input.Texcoord;
    
	/* Pasar normal a World-Space
	Solo queremos rotarla, no trasladarla ni escalarla.
	Por eso usamos matInverseTransposeWorld en vez de matWorld */
    output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;

	//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
    float3 worldPosition = output.Position.xyz;
    output.LightVec = lightPosition - worldPosition;

	//ViewVec (V): vector que va desde el vertice hacia la camara.
    output.ViewVec = eyePosition.xyz - worldPosition;

	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
    output.HalfAngleVec = output.ViewVec + output.LightVec;
    
	//Posicion pasada a World-Space
    output.WorldPosition = mul(input.Position, matWorld).xyz;

    return output;
}

//Input del Pixel Shader
struct PS_BLINN
{
    float2 Texcoord : TEXCOORD0;
    float3 WorldNormal : TEXCOORD1;
    float3 LightVec : TEXCOORD2;
    float3 HalfAngleVec : TEXCOORD3;
    float3 ViewVec : TEXCOORD4;
    float3 WorldPosition : TEXCOORD5;
};

float3 lightColor;
float Ka;
float Kd;
float Ks;
float shininess;

//Pixel Shader
float4 ps_BlinnPhong(PS_BLINN input) : COLOR0
{     
	//Normalizar vectores
    float3 Nn = normalize(input.WorldNormal + tex2D(normalMap, input.Texcoord).xyz); // Esto no es asi, pero bueno...
    float3 Ln = normalize(input.LightVec);
    float3 Hn = normalize(input.HalfAngleVec);
    float3 Vn = normalize(input.ViewVec);
    lightColor = normalize(lightColor);

	//Obtener texel de la textura
    float4 texelColor = tex2D(diffuseMap, input.Texcoord);
    
	//Obtener texel de CubeMap
    float3 R = reflect(Vn, Nn);
    float3 reflectionColor = texCUBE(cubeMap, R).rgb;
    texelColor.rgb = texelColor.rgb * 1 + reflectionColor.rgb * .2;
    //return reflectionColor.xyzz;

	//Componente Diffuse: N dot L
    float3 n_dot_l = dot(Nn, Ln);
    float3 diffuseLight = lightColor * max(0.0, n_dot_l); //Controlamos que no de negativo

	//Componente Specular: (N dot H)^exp
    float3 n_dot_h = dot(Nn, Hn);
    float3 specularLight = n_dot_l <= 0.0
			? float3(0.0, 0.0, 0.0)
			: lightColor * pow(max(0.0, n_dot_h), shininess);

    return float4(texelColor.xyz * Ka + diffuseLight * Kd + specularLight * Ks, texelColor.a);
}

/*
* Technique DIFFUSE_MAP
*/
technique BlinnPhong
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_BlinnPhong();
        PixelShader = compile ps_3_0 ps_BlinnPhong();
    }
}


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
        VertexShader = compile vs_3_0 vs_BlinnPhong();
        //VertexShader = compile vs_3_0 vs_Pasto();
        PixelShader = compile ps_3_0 ps_Pasto();
    }
}