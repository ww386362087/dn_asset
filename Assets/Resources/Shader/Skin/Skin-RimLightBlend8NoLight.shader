    Shader "Custom/Skin/RimlightBlend8NoLight" {  
        Properties {  
			_Tex0("Texture0", 2D) = "black" {}		
			_Tex1("Texture1", 2D) = "black" {}
			_Tex2("Texture2", 2D) = "black" {}			
			_Tex3("Texture3", 2D) = "black" {}		
			_Tex4("Texture4", 2D) = "black" {}		
			_Tex5("Texture5", 2D) = "black" {}		
			_Tex6("Texture6", 2D) = "black" {}		
			_Tex7("Texture7", 2D) = "black" {}	
		}  
        SubShader {  
            LOD 300            
            Pass {  
			Name "Basic"
            Tags { "RenderType" = "Opaque"}  
                  
                Cull Back  
				Lighting Off

                CGPROGRAM 
				//define
				#define SKINTEX
				#define SKINTEX8
				#define RIMLIGHT
				//head
				#include "../Include/CommonHead_Include.cginc"
				#include "../Include/SkinBlend_Include.cginc"
				//vertex&fragment
				#pragma vertex vert
                #pragma fragment frag 

				//custom frag fun
				fixed4 BasicColor(in v2f i)
				{
					fixed4 c = BlendColor(i);
					c.a = 1;
					return c;
				}
				//include
                #include "UnityCG.cginc"
                #include "../Include/CommonBasic_Include.cginc"
                ENDCG
            }
		}   
        FallBack "Unlit/Texture"  
    }  