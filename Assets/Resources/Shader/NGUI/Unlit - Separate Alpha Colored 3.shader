Shader "Hidden/Unlit/Separate Alpha Colored 3" 
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
    	_Mask ("Mask", 2D) = "white" {}
	}

	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"
				#include "UI_Include.cginc"
				struct appdata_t
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
				};
	
				struct v2f
				{
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
					float4 worldPos : TEXCOORD1;
					float2 worldPos2 : TEXCOORD2;
					fixed4 color : COLOR;
				};
	
        		sampler2D _MainTex;
        		sampler2D _Mask;
				float4 _MainTex_ST;
			
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.color = v.color;
					o.worldPos.xy = Clip1(v.vertex.xy);
					o.worldPos.zw = Clip2(v.vertex.xy);
					o.worldPos2 = Clip3(v.vertex.xy);
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{					
					return UI3Clip(UISample(_MainTex, _Mask, i.color, i.texcoord), i.worldPos, i.worldPos2);
				}
			ENDCG
		}
	}	
}
