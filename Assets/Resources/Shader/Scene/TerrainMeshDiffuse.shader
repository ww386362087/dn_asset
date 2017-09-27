Shader "Custom/Terrain/MeshDiffuse" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

		SubShader
		{
			Tags{ "RenderType" = "Opaque" }
			LOD 150
			Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile __ CUSTOM_SHADOW_ON 
			#pragma target 3.0
			#include "UnityCG.cginc"
			float4x4 custom_World2Shadow;
			sampler2D _CustomShadowMapTexture;
			//sampler2D _ShadowMask;
			struct appdata_t {
				float4 vertex : POSITION;
				//float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
				float2 texcoord3 : TEXCOORD3;
			};

			struct v2f
			{
				half4 vertex : SV_POSITION;
				half2 tc_Control : TEXCOORD0;
				half2 uv2 : TEXCOORD1;
				half2 uv_Splat0 : TEXCOORD2;
				half2 uv_Splat1 : TEXCOORD3;
				half2 uv_Splat2 : TEXCOORD4;
				half2 uv_Splat3 : TEXCOORD5;
				// Not prefixing '_Contorl' with 'uv' allows a tighter packing of interpolators, which is necessary to support directional lightmap.
				UNITY_FOG_COORDS(6)
#ifdef CUSTOM_SHADOW_ON
				half4 _WorldPosViewZ : TEXCOORD7;
#endif

			};

			sampler2D _Control;
			float4 _Control_ST;
			float4 _Splat0_ST;
			float4 _Splat1_ST;
			float4 _Splat2_ST;
			float4 _Splat3_ST;
			sampler2D _Splat0, _Splat1, _Splat2, _Splat3;


			v2f vert(appdata_t v)
			{
				v2f o = (v2f)0;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.tc_Control = TRANSFORM_TEX(v.texcoord, _Control);
				o.uv_Splat0 = TRANSFORM_TEX(v.texcoord, _Splat0);
				o.uv_Splat1 = TRANSFORM_TEX(v.texcoord, _Splat1);
				o.uv_Splat2 = TRANSFORM_TEX(v.texcoord, _Splat2);
				o.uv_Splat3 = TRANSFORM_TEX(v.texcoord, _Splat3);
#ifdef CUSTOM_SHADOW_ON
				o._WorldPosViewZ.xyz = mul(unity_ObjectToWorld, v.vertex).xyz;
				o._WorldPosViewZ.w = -mul(UNITY_MATRIX_MV, v.vertex).z;
#endif
				o.uv2.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}

			fixed4 frag(v2f i) :SV_Target
			{
				half4 splat_control;
				half weight;
				fixed4 mixedDiffuse;
				splat_control = tex2D(_Control, i.tc_Control);
				weight = dot(splat_control, half4(1, 1, 1, 1));

				// Normalize weights before lighting and restore weights in final modifier functions so that the overal
				// lighting result can be correctly weighted.
				splat_control /= (weight + 1e-3f);

				mixedDiffuse = 0.0f;
				//mixedDiffuse += tex2D(_CustomShadowMapTexture, i.tc_Control);
				mixedDiffuse += splat_control.r * tex2D(_Splat0, i.uv_Splat0);
				mixedDiffuse += splat_control.g * tex2D(_Splat1, i.uv_Splat1);
				mixedDiffuse += splat_control.b * tex2D(_Splat2, i.uv_Splat2);
				mixedDiffuse += splat_control.a * tex2D(_Splat3, i.uv_Splat3);

				fixed4 final = fixed4(mixedDiffuse.rgb, weight);
				final.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2.xy)).rgb;
#ifdef CUSTOM_SHADOW_ON
				float4 shadow = mul(custom_World2Shadow, float4(i._WorldPosViewZ.xyz, 1));
				float4 coord = float4(shadow.xyz / shadow.w, 1);
				float depth = 1 - step(SAMPLE_DEPTH_TEXTURE(_CustomShadowMapTexture, coord.xy + float2(0.15, 0.0)), 0.99)*(final.r+ final.g+ final.b)*0.33;

				final.rgb *= float3(depth, depth, depth);
#endif
				UNITY_APPLY_FOG(i.fogCoord, final);
				return final;
			}
			ENDCG
		}
	}
}
