Shader "Custom/Scene/WaterWarpPlane" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_BumpTex ("Bump (A)", 2D) = "bump" {}
		_Cube ("Cubemap", CUBE) = "" {}
		_WaterColor("Water Color",Color)=(1,1,1,1)
		_Vector("x:U_Speed  y:V_Speed z:WarpScale w:ReflWarpScale",vector)=(1,1,1,0.25)
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass 
		{
			Tags { "LightMode" = "Always" }
			CGPROGRAM
			#pragma multi_compile_fog 
			#pragma vertex vert
			#pragma fragment frag
			

			#include "UnityCG.cginc"
			sampler2D _MainTex;
			sampler2D _BumpTex;
			samplerCUBE _Cube; 
			struct appdata_t 
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float3 normal: NORMAL;
				fixed4 color :COLOR;
			};

			struct v2f 
			{
				half4 vertex : SV_POSITION;
				half2 uv : TEXCOORD0;
				half2 uvBump : TEXCOORD3;
				half3 view : TEXCOORD1;
				half3 normal : TEXCOORD4;
				fixed4 color : TEXCOORD5;
				UNITY_FOG_COORDS(2)
			};
			
			half4 _MainTex_ST;
			half4 _BumpTex_ST;
			half4 _Vector;
			fixed4 _WaterColor;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.uvBump = TRANSFORM_TEX(v.texcoord1,_BumpTex);
				o.view= normalize( WorldSpaceViewDir( v.vertex)) ;
				o.normal = normalize(mul((float3x3)unity_ObjectToWorld,v.normal));
				o.color=v.color; //x:ÅÝÄ­ÇøÓò  y£ºÉî¶È
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 bump =   tex2D (_BumpTex, i.uvBump+float2(_Vector.x,_Vector.y)*_Time.x)*2-1; 
				fixed4 MainT_Warp = tex2D (_MainTex, i.uv+bump.zz*_Vector.z*0.2*(i.color.y)); 
				fixed4 ccc=lerp(_WaterColor,1,1-i.color.y);
				fixed4 col=lerp(MainT_Warp*ccc,MainT_Warp*_WaterColor*10*_WaterColor.a,i.color.y);
				half3 refluv=reflect(-i.view,normalize( i.normal+float3(bump.x,1,bump.y)*_Vector.z*_Vector.w));
				fixed4 mc = texCUBE (_Cube, refluv);
				fixed ndv=max(0.0001,dot(i.normal,i.view.xyz));
				fixed paomo= 1-(1-i.color.x)*(bump.z*0.5+0.5);
				return lerp(1.2,lerp(mc,col,ndv*0.6+0.3),paomo*paomo);
			}
			ENDCG 
		}
	}
}
