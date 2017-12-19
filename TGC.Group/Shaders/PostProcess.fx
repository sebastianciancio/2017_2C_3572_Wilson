/*
* Shaders para efectos de Post Procesadosss
*/
/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

float hora;
float time;

/**************************************************************************************/
/* DEFAULT */
/**************************************************************************************/


//Input del Vertex Shader
struct VS_INPUT_DEFAULT 
{
   float4 Position : POSITION0;
   float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_DEFAULT
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
};


//Vertex Shader
VS_OUTPUT_DEFAULT vs_default( VS_INPUT_DEFAULT Input )
{
   VS_OUTPUT_DEFAULT Output;

   //Proyectar posicion
   Output.Position = float4(Input.Position.xy, 0, 1);
   
   //Las Texcoord quedan igual
   Output.Texcoord = Input.Texcoord;

   return( Output );
}



//Textura del Render target 2D
texture render_target2D;
sampler RenderTarget = sampler_state
{
    Texture = (render_target2D);
    MipFilter = NONE;
    MinFilter = NONE;
    MagFilter = NONE;
};


//Input del Pixel Shader
struct PS_INPUT_DEFAULT 
{
   float2 Texcoord : TEXCOORD0;
};

//Pixel Shader
float4 ps_default( PS_INPUT_DEFAULT Input ) : COLOR0
{      
	float4 color = tex2D( RenderTarget, Input.Texcoord );
	return color;
}

technique DefaultTechnique
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_default();
	  PixelShader = compile ps_2_0 ps_default();
   }
}

/**************************************************************************************/
/* OSCURECER */
/**************************************************************************************/

//Pixel Shader de Oscurecer
float4 ps_oscurecer( PS_INPUT_DEFAULT Input ) : COLOR0
{     
	float lightFactor;
	
	if(hora > 0 && hora < 120) {
	    lightFactor = 0.0025 * hora + 0.4;
	}
	
	if(hora >= 120 && hora <= 240) {
		lightFactor = -0.0048 * hora + 1.27;
	}
	
	if(hora > 240 && hora < 300) {
		lightFactor = 0.0046 * hora - 1; 
	}

	float4 color = tex2D( RenderTarget, Input.Texcoord );
	
	float value = ((color.r + color.g + color.b) / 3) * lightFactor; 
	color.rgb = color.rgb * lightFactor + value * lightFactor;

	return color;
}

technique OscurecerTechnique
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_default();
	  PixelShader = compile ps_2_0 ps_oscurecer();
   }
}

/**************************************************************************************/
/* RAIN */
/**************************************************************************************/

float alarmaScaleFactor = 0.1;

//Textura alarma
texture textura_alarma;
sampler sampler_alarma = sampler_state
{
    Texture = (textura_alarma);
};

//Pixel Shader de Lluvia
float4 ps_rainMejorado( PS_INPUT_DEFAULT Input ) : COLOR0
{     
	//Obtener color segun textura
	//hacer desplazamiento de la lluvia
	float2 offset = (0, time * 500);
	
	float4 color = tex2D( RenderTarget, Input.Texcoord);
	
	//Obtener color de textura de alarma, escalado por un factor
	float4 color2 = tex2D( sampler_alarma, Input.Texcoord + offset );
	
	//Mezclar ambos texels
	return color+ color2;;
}

technique RainTechnique
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_default();
	  PixelShader = compile ps_2_0 ps_rainMejorado();
   }
}

/**************************************************************************************/
/* OSCURECER */
/**************************************************************************************/

//Pixel Shader de Oscurecer
float4 ps_oscurecer_plus_lluvia( PS_INPUT_DEFAULT Input ) : COLOR0
{     
	float lightFactor;
	
	if(hora > 0 && hora < 120) {
	    lightFactor = 0.0025 * hora + 0.4;
	}
	
	if(hora >= 120 && hora <= 240) {
		lightFactor = -0.0048 * hora + 1.27;
	}
	
	if(hora > 240 && hora < 300) {
		lightFactor = 0.0046 * hora - 1; 
	}

	float4 color = tex2D( RenderTarget, Input.Texcoord );
	
	float value = ((color.r + color.g + color.b) / 3) * lightFactor; 
	color.rgb = color.rgb * lightFactor + value * lightFactor;

	// end darken
	
	float2 offset = (0, time * 500);
	
	float4 color2 = tex2D( sampler_alarma, Input.Texcoord + offset );
	
	//Mezclar ambos texels
	return color+ color2;;
}

technique OscurecerAndRainTechnique
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_default();
	  PixelShader = compile ps_2_0 ps_oscurecer_plus_lluvia();
   }
}