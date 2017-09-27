Shader "Hidden/MobileDOFPro"
{ 
	Properties
	{ 
		_MainTex ("", 2D) = "white" {}

		_FocalDist ("Focal Dist", Float) = .1
	_FocalLength ("Focal Length", Float) = 0.02
	}

SubShader {
	ZTest Always Cull Off ZWrite Off Fog { Mode Off }

	CGINCLUDE	
		#pragma multi_compile  BLUR_RADIUS_5 BLUR_RADIUS_3
		#pragma multi_compile DOF_ALL_ON DOF_FAR_ONLY_ON DOF_NEAR_ONLY_ON

		//#pragma multi_compile FXPRO_HDR_ON FXPRO_HDR_OFF

		//#pragma target 3.0
		#pragma glsl
		 
		#include "FxProInclude.cginc"

		#define GAUSSIAN_KERNEL
	
		half _FocalDist;
		half _FocalLength;
		sampler2D _CameraDepthTexture;

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		half4 _MainTex_TexelSize;
		
		half _OneOverDepthScale;

		v2f_img vert_img_aa(appdata_img v)
		{
			v2f_img o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.texcoord;

			#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0)
				o.uv.y =  1 - o.uv.y; 
			#endif
			return o;
		}
	ENDCG 
	

		Pass { // pass 1
		name "dof_separable_simple"
				
		CGPROGRAM
			#pragma vertex vert_img_aa
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			
            sampler2D _COCTex;

			#define OUTPUT_COC_TO_ALPHA
			#ifdef OUTPUT_COC_TO_ALPHA
				#define IGNORE_ALPHA_CHANNEL
			#endif

			#include "SeparableBlur.cginc"

			fixed4 frag (v2f_img input) : COLOR  {
			fixed focalDist = _FocalDist;
				fixed curCOC = tex2D( _COCTex, input.uv ).r;
				fixed depth = Linear01Depth( tex2D(_CameraDepthTexture, input.uv).r );
					#ifdef DOF_FAR_ONLY_ON
					depth = max(depth, focalDist);
					#endif
					#ifdef DOF_NEAR_ONLY_ON
					depth = min(depth, focalDist);
					#endif
				
				depth=saturate( abs(depth - focalDist) / depth ) * _FocalLength / saturate(focalDist - _FocalLength);
				fixed4 res = BlurTex(_MainTex, input, depth);
				res.a=depth;
				/*#ifdef OUTPUT_COC_TO_ALPHA
				res.a = curCOC;
				#endif*/

				return res;
			}

		ENDCG
	}

}
}

