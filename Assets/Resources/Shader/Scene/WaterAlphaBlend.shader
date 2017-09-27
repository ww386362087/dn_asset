Shader "Custom/Scene/WaterAlphaBlend" 
{
	Properties 
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_Cap ("cap", 2D) = "" {}
		_WaterColor("Water Color",Color)=(1,1,1,1)
		_Vector("x:U_Speed  y:V_Speed   z:WarpScale   w:ReflWarpScale",vector)=(1,1,1,0.25)
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

		Pass
		{


			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Cap;
			half4 _MainTex_ST;
			half4 _Cap_ST;
			half4 _Vector;
			fixed4 _WaterColor;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float3 normal: NORMAL;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f 
			{
				float4 vertex : SV_POSITION;
				half2 cap : TEXCOORD2;
				fixed4 color : TEXCOORD5;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD4;
				UNITY_FOG_COORDS(1)
			};


			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				half2 capCoord;
				capCoord.x = dot(UNITY_MATRIX_IT_MV[0].xyz, v.normal);
				capCoord.y = dot(UNITY_MATRIX_IT_MV[1].xyz, v.normal);
				o.cap = capCoord * 0.5 + 0.5;
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.texcoord1 = TRANSFORM_TEX(v.texcoord,_Cap);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 MainT = tex2D(_MainTex, i.texcoord + _Vector.xy*_Time.x);
				fixed4 mc = tex2D(_Cap, i.texcoord1 + MainT.xy*_Vector.z);

				fixed4 col = mc* _WaterColor;;
				col.a = i.color.r*(mc.x + mc.y + mc.z) / 3;
				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); // fog towards black due to our blend mode
				return col;
			}
			ENDCG
		}
	}
}
