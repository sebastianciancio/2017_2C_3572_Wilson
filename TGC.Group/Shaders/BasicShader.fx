// ---------------------------------------------------------
// Ejemplo shader Minimo:
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
	ADDRESSU = MIRROR;
	ADDRESSV = MIRROR;
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
	float4 Position :        POSITION0;
	float2 Texcoord :        TEXCOORD0;
	float4 Color :			COLOR0;
};

//Vertex Shader
VS_OUTPUT vs_main(VS_INPUT Input)
{
	VS_OUTPUT Output;
	//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
	Output.Color = Input.Color;

	return(Output);
}

// Ejemplo de un vertex shader que anima la posicion de los vertices
// ------------------------------------------------------------------
VS_OUTPUT vs_main2(VS_INPUT Input)
{
	VS_OUTPUT Output;

	// Animar posicion

	//Input.Position.x += sin(time)*30*sign(Input.Position.x);
	//Input.Position.y += cos(time)*30*sign(Input.Position.y-20);
	//Input.Position.z += sin(time)*30*sign(Input.Position.z);


	Input.Position.z += (Input.Position.z / 1 *  sin(time) * 2 * 0.04 );
   	Input.Position.x += (Input.Position.x / 1 * cos(time) * 2 * 0.04 );
   	Input.Position.y += (Input.Position.y * sin(time) * cos(time) * 0.1 );


	// Animar posicion
/*
	float Y = Input.Position.y;
	float Z = Input.Position.z;
	Input.Position.y = Y * cos(time) - Z * sin(time);
	Input.Position.z = Z * cos(time) + Y * sin(time);
*/
	//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
	Output.Color = Input.Color;

	return(Output);
}

//Pixel Shader
float4 ps_main(float2 Texcoord: TEXCOORD0, float3 WPos : TEXCOORD1, float4 Color : COLOR0) : COLOR0
{



	float3 dx = ddx(WPos);
	float3 dy = ddy(WPos);
	float3 n = normalize(cross(dx, dy));
	float3 l = normalize(WPos - float3(1000,1000,0));
	//float k = 0.5 * abs(dot(n,l)) + 0.5;
	float k = 0.5 * WPos.y/2*120/450 + 0.5;

	//return float4(n,1);


	return tex2D(diffuseMap, Texcoord);


	// Obtener el texel de textura
	// diffuseMap es el sampler, Texcoord son las coordenadas interpoladas
	float4 fvBaseColor = tex2D(diffuseMap, Texcoord);
	// combino color y textura
	// en este ejemplo combino un 80% el color de la textura y un 20%el del vertice
	return 0.9*fvBaseColor + 0.1*Color;
}

// ------------------------------------------------------------------
technique RenderScene
{
	pass Pass_0
	{
		VertexShader = compile vs_2_0 vs_main2();
		PixelShader = compile ps_2_0 ps_main();
	}
}