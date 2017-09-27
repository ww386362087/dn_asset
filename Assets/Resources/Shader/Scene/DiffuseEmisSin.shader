Shader "Custom/Scene/DiffuseEmisSin" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Mask ("Mask (B)", 2D) = "white" {}
		_Emission("EmisColor(RGB) Intensity(A)",COLOR)=(0,0,0,1)
		_EmisVector("X:Speed Y:Min  Z:Distance  W:Direction   ",vector)=(1,1,0,1)
	}


	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		Pass 
		{
			Tags { "LightMode" = "Vertex" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define WP
			#define MASKTEX
			#define EMISSION
			#pragma multi_compile_fog
			#include "../Include/SceneHead_Include.cginc"
			fixed4 _Emission;
			half4 _EmisVector;
			fixed3 EmissionColor(v2f i,fixed4 col,fixed4 mask)
			{
				float sinPos= sin(_Time.y*_EmisVector.x+lerp(i.worldPos.x,i.worldPos.z,saturate( _EmisVector.w))*0.01*_EmisVector.z)*0.5+0.5;
				fixed3 emissive = _Emission.rgb*_Emission.a*mask.b*min((sinPos+_EmisVector.y),1);
				return col.rgb + emissive;
			}
			#include "../Include/Scene_Include.cginc"
			ENDCG 
		}

		Pass 
		{
			Tags { "LightMode" = "VertexLMRGBM" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define WP
			#define MASKTEX
			#define LM
			#define EMISSION
			#pragma multi_compile_fog
			#include "../Include/SceneHead_Include.cginc"
			fixed4 _Emission;
			half4 _EmisVector;
			fixed3 EmissionColor(v2f i,fixed4 col,fixed4 mask)
			{
				float sinPos= sin(_Time.y*_EmisVector.x+lerp(i.worldPos.x,i.worldPos.z,saturate( _EmisVector.w))*0.01*_EmisVector.z)*0.5+0.5;
				fixed3 emissive = _Emission.rgb*_Emission.a*mask.b*min((sinPos+_EmisVector.y),1);
				return col.rgb + emissive;
			}	
			#include "../Include/Scene_Include.cginc"
			ENDCG 
		}
		Pass 
		{  
			//Moblie
			Tags { "LightMode" = "VertexLM" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define WP
			#define MASKTEX
			#define LM
			#define EMISSION
			#pragma multi_compile_fog
			#include "../Include/SceneHead_Include.cginc"
			fixed4 _Emission;
			half4 _EmisVector;
			fixed3 EmissionColor(v2f i,fixed4 col,fixed4 mask)
			{
				float sinPos= sin(_Time.y*_EmisVector.x+lerp(i.worldPos.x,i.worldPos.z,saturate( _EmisVector.w))*0.01*_EmisVector.z)*0.5+0.5;
				fixed3 emissive = _Emission.rgb*_Emission.a*mask.b*min((sinPos+_EmisVector.y),1);
				return col.rgb + emissive;
			}	
			#include "../Include/Scene_Include.cginc"		
			ENDCG
		}
		
		UsePass "Custom/Common/META"
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 100		
		Pass 
		{  
			//Moblie
			Tags { "LightMode" = "VertexLM" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define MASKTEX
			#define LM
			#pragma multi_compile_fog
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"		
			ENDCG
		}
		
		UsePass "Custom/Common/META"
	}
}
