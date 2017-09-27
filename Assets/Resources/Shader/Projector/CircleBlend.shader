Shader "Custom/Projector/CircleBlend" 
{
	Properties 
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_OutlineColor ("Outline Color", Color) = (1,1,1,1)
		_Arg("x:color transition y:color scale z:outline width w: outline scale",Vector) = (1.1,1,0.1,1)
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

				fixed4 _TintColor;
				fixed4 _OutlineColor;
				half4 _Arg;


				fixed4 frag (v2f i) : SV_Target
				{				
					half circle = saturate((0.25 - ( i.uv.x* i.uv.x+ i.uv.y* i.uv.y))*4);
					half mask = sign(circle);
					half dis = _Arg.y*(_Arg.x-circle);
					half cull = saturate(_Arg.z-circle)*_Arg.w;
					fixed4 color = _TintColor;
					color.a *=dis;
					return (color+ cull*_OutlineColor)*mask ;
				}
				ENDCG 
			}
		}	
	}
}