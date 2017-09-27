Shader "Custom/Skin/Effect" 
{
	Properties 
	{
		_Tex0("Texture0", 2D) = "black" {}		
		_Tex1("Texture1", 2D) = "black" {}
		_Tex2("Texture2", 2D) = "black" {}			
		_Tex3("Texture3", 2D) = "black" {}		
		_Tex4("Texture4", 2D) = "black" {}		
		_Tex5("Texture5", 2D) = "black" {}		
		_Tex6("Texture6", 2D) = "black" {}		
		_Tex7("Texture7", 2D) = "black" {}
		_Color("x: y: z:Add Power w:alpha",Vector) = (0.0,0.0,0.0,1.0)
	}
	SubShader
	{
		Pass 
		{ 
            Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }			
			ZWrite On Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
                  
            CGPROGRAM 
			//define
			#define SKINTEX
			#define SKINTEX8
			//head
			#include "../Include/CommonHead_Include.cginc"
			#include "../Include/SkinBlend_Include.cginc"
			//vertex&fragment
			#pragma vertex vert
            #pragma fragment frag 

			fixed4 _Color;
			//custom frag fun
			fixed4 BasicColor(in v2f i, inout fixed4 mask)
			{
				fixed4 c = BlendColor(i);
				c.a = _Color.w;
				return c;
			}
			//include
            #include "../Include/CommonBasic_Include.cginc"
            ENDCG  
        }			
	}
}	

