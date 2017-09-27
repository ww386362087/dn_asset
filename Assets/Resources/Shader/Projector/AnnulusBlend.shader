Shader "Custom/Projector/AnnulusBlend" 
{
	Properties 
	{
		_Color ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_Args("x:annulus width y:annulus radius ",Vector) = (1,0.5,1,0)		
	}

	Category 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off ZWrite Off Fog{ Mode Off }
	
		SubShader 
		{
			Pass 
			{		
				CGPROGRAM
				#include "../Include/Projector_Include.cginc"
				#pragma vertex vert
				#pragma fragment frag
				fixed4 _Color;
				half4 _Args;

				fixed4 frag (v2f i) : SV_Target
				{				
					half c= 1-floor(saturate( length(i.uv*2/max(_Args.y,0.0001))));
					half c1= 1-floor(saturate( length(i.uv*2/max((_Args.y-_Args.x),0.0001))));
					fixed4 col=_Color;
					col.a=(c-c1)*_Color.a;
					return col;
				}
				ENDCG 
			}
		}	
	}
}