Shader "Custom/Common"
{
	Properties
	{
	}
	SubShader
	{
		
		LOD 1000
		Pass
		{
			Name "META"
			Tags{  "RenderType"="Opaque" "LightMode" = "Meta" }
			CGPROGRAM
			#pragma vertex vert_meta
			#pragma fragment frag_meta

			#include "Lighting.cginc"
			#include "UnityMetaPass.cginc"

			struct v2f
			{
				float4 pos:SV_POSITION;
				float2 uv:TEXCOORD1;
				float3 worldPos:TEXCOORD0;
			};

			uniform fixed4 _Color;
			uniform sampler2D _MainTex;
			uniform sampler2D _Mask;
			v2f vert_meta(appdata_full v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f,o);
				o.pos = UnityMetaVertexPosition(v.vertex,v.texcoord1.xy,v.texcoord2.xy,unity_LightmapST,unity_DynamicLightmapST);
				o.uv = v.texcoord.xy;
				return o;
			}

			fixed4 frag_meta(v2f IN) :SV_Target
			{
				UnityMetaInput metaIN;
				UNITY_INITIALIZE_OUTPUT(UnityMetaInput,metaIN);
				metaIN.Albedo = tex2D(_MainTex,IN.uv).rgb * _Color.rgb;
				metaIN.Emission = 0;
				return UnityMetaFragment(metaIN);
			}
			ENDCG
		}
		Pass
		{
			Name "CASTSHADOW"
			Tags{  "RenderType"="Opaque" "LightMode" = "ShadowCaster" }
			ZWrite On ZTest LEqual Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"

			struct v2f { 
				V2F_SHADOW_CASTER;
			};

			v2f vert( appdata_base v )
			{
				v2f o;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			float4 frag( v2f i ) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
		Pass
		{
			Name "CASTSHADOWCUTOUT"
			Tags{  "RenderType"="TransparentCutout" "LightMode" = "ShadowCaster" }
			ZWrite On ZTest LEqual Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"

			struct v2f { 
				V2F_SHADOW_CASTER;
				float2  uv : TEXCOORD1;
			};

			v2f vert( appdata_base v )
			{
				v2f o;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				o.uv = v.texcoord;
				return o;
			}
			sampler2D _Mask;
			float4 frag( v2f i ) : SV_Target
			{
				fixed4 mask = tex2D( _Mask, i.uv );
				clip( mask.r- 0.3 );
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
}
