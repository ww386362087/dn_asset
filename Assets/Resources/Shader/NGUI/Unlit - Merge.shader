Shader "Unlit/Merge" 
{
	Properties
	{
		_MainTex0 ("Base0 (RGB)", 2D) = "white" {}
    	_Mask0 ("Mask0", 2D) = "white" {}
    	_MainTex1 ("Base1 (RGB)", 2D) = "white" {}
    	_Mask1 ("Mask1", 2D) = "white" {}
    	_MainTex2 ("Base2 (RGB)", 2D) = "white" {}
    	_Mask2 ("Mask2", 2D) = "white" {}
    	_MainTex3 ("Base3 (RGB)", 2D) = "white" {}
    	_Mask3 ("Mask3", 2D) = "white" {}
    	
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
		AlphaTest Off
		Fog { Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#include "UnityCG.cginc"
	
				struct appdata_t
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
				};
	
				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
					float4 worldPos : TEXCOORD1;
					float2 worldPos2 : TEXCOORD2;
					float4 m:TEXCOORD3;
					fixed  greyBlend:TEXCOORD4;

				};
	
        		sampler2D _MainTex0;
        		sampler2D _Mask0;
        		sampler2D _MainTex1;
        		sampler2D _Mask1;
        		sampler2D _MainTex2;
        		sampler2D _Mask2;
        		sampler2D _MainTex3;
        		sampler2D _Mask3;

        		float4 _MainTex0_ST;
				float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
				float4 _ClipArgs0 = float4(1000.0, 1000.0, 0.0, 1.0);
				float4 _ClipRange1 = float4(0.0, 0.0, 1.0, 1.0);
				float4 _ClipArgs1 = float4(1000.0, 1000.0, 0.0, 1.0);
				float4 _ClipRange2 = float4(0.0, 0.0, 1.0, 1.0);
				float4 _ClipArgs2 = float4(1000.0, 1000.0, 0.0, 1.0);
        		
				float4 _MainTex_ST;

				float2 Rotate (float2 v, float2 rot)
				{
					float2 ret;
					ret.x = v.x * rot.y - v.y * rot.x;
					ret.y = v.x * rot.x + v.y * rot.y;
					return ret;
				}
				
				float InRange(float2 uv,float4 ranges)
				{
					float2 value = step(uv,ranges.xy) * step(ranges.zw,uv);
					//float2 value = step(ranges.zw,uv);
					return value.x * value.y;
				}
				
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex0);
					o.color = v.color;
					
					//float4 theRange = float4(1.01,1.01,-0.01,-0.01);		
					float4 theRange = float4(1.0,1.0,0.0,0.0);		
									
					o.m.x = InRange(o.texcoord,theRange);
					theRange.xy += 2.0;
					theRange.zw += 2.0;
					o.m.y = InRange(o.texcoord,theRange);
					theRange.xy += 2.0;
					theRange.zw += 2.0;
					o.m.z = InRange(o.texcoord,theRange);
					theRange.xy += 2.0;
					theRange.zw += 2.0;
					o.m.w = InRange(o.texcoord,theRange);
					
					float3 greyMask = step(v.color.rgb,fixed3(0.001));
				    o.greyBlend = greyMask.r * greyMask.g * greyMask.b;

					o.worldPos.xy = v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;
					o.worldPos.zw = Rotate(v.vertex.xy, _ClipArgs1.zw) * _ClipRange1.zw + _ClipRange1.xy;
					//o.worldPos.zw = Rotate(v.vertex.xy, _ClipArgs1.zw) * float2(1.0/1000.0,1.0/1000.0);
					o.worldPos2 = Rotate(v.vertex.xy, _ClipArgs2.zw) * _ClipRange2.zw + _ClipRange2.xy;
					//o.worldPos2 = Rotate(v.vertex.xy, _ClipArgs2.zw) * float2(1.0/1000.0,1.0/1000.0) + _ClipRange2.xy;

					return o;
				}
				
				fixed4 GetSperateColor(sampler2D mainTex,sampler2D tex,v2f i)
				{
					fixed4 col; 
					
					col = tex2D(mainTex,i.texcoord);
					float grey = Luminance(col.rgb);
					
					col = col * i.color * (1.0 - i.greyBlend) + fixed4(grey,grey,grey,i.color.a) * i.greyBlend;
					col.a *= tex2D(tex,i.texcoord).a;
					return col;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
				    //int index = int(i.t.x);
				    i.texcoord = frac(i.texcoord);
				    
				    float4 finalColor = float4(0.0);
				    finalColor += GetSperateColor(_MainTex0,_Mask0,i) * i.m.x;
				    finalColor += GetSperateColor(_MainTex1,_Mask1,i) * i.m.y;
				    finalColor += GetSperateColor(_MainTex2,_Mask2,i) * i.m.z;
				    finalColor += GetSperateColor(_MainTex3,_Mask3,i) * i.m.w;
				    
					
				    // First clip region
				    float f = 1.0;
					float2 factor = (float2(1.0, 1.0) - abs(i.worldPos.xy)) * _ClipArgs0.xy;
					f = min(factor.x, factor.y);
					
					// Second clip region
					factor = (float2(1.0, 1.0) - abs(i.worldPos.zw)) * _ClipArgs1.xy;
					f = min(f, min(factor.x, factor.y));

					// Third clip region
					factor = (float2(1.0, 1.0) - abs(i.worldPos2)) * _ClipArgs2.xy;
					f = min(f, min(factor.x, factor.y));

					finalColor.a *= clamp(f, 0.0, 1.0);

					return finalColor;
				}
			ENDCG
		}
	}

	//SubShader
	//{
	//	LOD 100

	//	Tags
	//	{
	//		"Queue" = "Transparent"
	//		"IgnoreProjector" = "True"
	//		"RenderType" = "Transparent"
	//	}
		
	//	Pass
	//	{
	//		Cull Off
	//		Lighting Off
	//		ZWrite Off
	//		Fog { Mode Off }
	//		Offset -1, -1
	//		ColorMask RGB
	//		Blend SrcAlpha OneMinusSrcAlpha
	//		ColorMaterial AmbientAndDiffuse
			
	//		SetTexture [_MainTex]
	//		{
	//			Combine Texture * Primary
	//		}
	//	}
	//}
}
