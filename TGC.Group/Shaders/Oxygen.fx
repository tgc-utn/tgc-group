struct VertexOutput
{
	float4 Position : POSITION;
	float Color : COLOR;
};
struct VertexInput
{
	float4 Position : POSITION;
	float4 Color: COLOR;
};

struct PixelInput
{
	float4 Position : POSITIONT;
	float4 Color: COLOR;
};

VertexOutput main_vertex(VertexInput input)
{
	VertexOutput output;
	output.Position = input.Position;
	output.Color = input.Color;
	return output;
}

float PI = 3.1415;

float signOf(float num)
{
	return sign(num) + !num;
}

float isGraterThanZero(float num)
{
	return sign(sign(num) + 1);
}

float isLowerThanZero(float num)
{
	return isGraterThanZero(-num);
}

bool eq(float4 a, float4 b)
{
	return abs(a - b) < 0.05;
}

float modulus(float num, float base)
{
	return (num % base + base) % base;
}

extern uniform float oxygen;

float4 main_pixel(PixelInput input) : COLOR
{
	float2 absolutePos = input.Color.rg;
	float2 center = float2(0.5, 0.5);
	float2 pos = absolutePos - center;
	float radialDist = sqrt(pow(pos.x, 2) + pow(pos.y, 2));

	float angleForColor = (PI * isGraterThanZero(pos.y) - signOf(pos.y) * asin(abs(pos.x) / radialDist));
	float primitiveAngle = (PI * isGraterThanZero(pos.y) - signOf(pos.y) * asin(abs(pos.x) / radialDist));
	float realAngle = modulus(primitiveAngle - isLowerThanZero(pos.x) * 2 * primitiveAngle, 2 * PI);
	float colorIntensity = angleForColor / PI;

	float transparency = (cos(radialDist * 15.6) * isGraterThanZero(radialDist * 15.6 - PI / 2)) * isLowerThanZero(realAngle - oxygen * 2 * PI);

	return float4(0, 1, colorIntensity, transparency);
}

technique OxygenTechnique
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 main_vertex();
		PixelShader = compile ps_3_0 main_pixel();
	}
};