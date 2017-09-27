Shader "Custom/Scene/CutoutDiffuseMaskLM_VMove" 
{
	Properties
	{
		_Color("Main Color",Color)=(1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Mask ("Mask (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		_PannerPara("X-位移距离 Y-速度",vector)=(1,1,1,1)
	}
	SubShader 
	{  
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"  }
		LOD 400    
	
		Pass 
		{
			Tags { "LightMode" = "Vertex" }
			ColorMask RGB
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			fixed4 _Color;
			sampler2D _MainTex;
			sampler2D _Mask;
			float4 _PannerPara;
			struct appdata_t 
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f 
			{
				half4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				half2 uv3 : TEXCOORD2;
				
			};
			
			half4 _MainTex_ST;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				float4 WP = mul(unity_ObjectToWorld, v.vertex);
				float mask = saturate(v.vertex.z * 1000);
				float M_x = sin(_Time.y*_PannerPara.y + mask)*mask*0.1*_PannerPara.x;
				float M_z = sin(_Time.y*_PannerPara.y*0.75 + mask)*mask*0.1*_PannerPara.x;
				o.vertex = mul(UNITY_MATRIX_VP, WP + float4(M_x, 0, M_z, 0));
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.uv3 = half2(mask, mask);
				return o;
			}
			half _Cutoff;
			fixed4 frag (v2f i) : SV_Target
			{				
				fixed alpha = tex2D (_Mask, i.texcoord).r;
				clip(alpha -_Cutoff);
				fixed4 col = tex2D(_MainTex, i.texcoord)*_Color;			
				col.a = alpha;		
				return col;
			}
			ENDCG 
		}
	Pass {  
	Tags { "LightMode" = "VertexLMRGBM" }
	Alphatest Greater [_Cutoff]
	AlphaToMask True
		ColorMask RGB
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				half2 uv2 : TEXCOORD1;
				float4 color : COLOR;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				half2 uv2 : TEXCOORD1;
				half2 uv3 : TEXCOORD2;
				
			};
			fixed4 _Color;
			sampler2D _MainTex;
			sampler2D _Mask;
			half _Cutoff;
			float4 _PannerPara;
			half4 _MainTex_ST;
			v2f vert (appdata_t v)
			{
				v2f o;
				float4 WP = mul(unity_ObjectToWorld, v.vertex);
				float mask = saturate(v.vertex.z * 1000);
				float M_x = sin(_Time.y*_PannerPara.y + mask)*mask*0.1*_PannerPara.x;
				float M_z = sin(_Time.y*_PannerPara.y*0.75 + mask)*mask*0.1*_PannerPara.x;
				o.vertex = mul(UNITY_MATRIX_VP, WP + float4(M_x, 0, M_z, 0));
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv3 = half2(mask, mask);
				o.uv2.xy = v.uv2.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed alpha = tex2D (_Mask, i.texcoord).r;
				clip(alpha -_Cutoff);
				fixed4 col = tex2D(_MainTex, i.texcoord);		
				col.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2.xy)).rgb*_Color.rgb;		
				col.a = alpha+0.01;
				if(i.uv3.x<=0)
				col = fixed4(i.uv3.x, 0, 0, 1);
				else
					col = fixed4(0, 0, 1, 1);
				return col;
			}
		ENDCG
	}	        


	Pass {  
	Tags { "LightMode" = "VertexLM"}
	Alphatest Greater [_Cutoff]
	AlphaToMask True
		ColorMask RGB
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				half2 uv2 : TEXCOORD1;
				float4 color : COLOR;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				half2 uv2 : TEXCOORD1;
				
			};
			fixed4 _Color;
			sampler2D _MainTex;
			sampler2D _Mask;
			// sampler2D unity_Lightmap;
			// float4 unity_LightmapST;
			half _Cutoff;
			float4 _PannerPara;
			v2f vert (appdata_t v)
			{
				v2f o;
				float4 WP = mul(unity_ObjectToWorld, v.vertex);
				float M_x = sin(_Time.y*_PannerPara.y + length(WP.xyz))*v.color.a*0.1*_PannerPara.x;
				float M_z = sin(_Time.y*_PannerPara.y*0.75 + length(WP.xyz))*v.color.a*0.1*_PannerPara.x;
				o.vertex = mul(UNITY_MATRIX_VP, WP + float4(M_x,0,M_z, 0));
				o.texcoord = v.texcoord;
				
				o.uv2.xy = v.uv2.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed alpha = tex2D (_Mask, i.texcoord).r;
				clip(alpha -_Cutoff);
				fixed4 col = tex2D(_MainTex, i.texcoord);		
				col.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2.xy)).rgb*_Color.rgb;		
				col.a = alpha+0.01;
				
				return col;
			}
		ENDCG
	}	      
} 
}