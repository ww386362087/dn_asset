Shader "Custom/Projector/RectBlend" 
{
	Properties 
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_OutlineColor ("Outline Color", Color) = (1,1,1,1)
		_Arg("x:color edge y:color center z:outline width w: outline scale",Vector) = (2.5,0.5,0.03,10)
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
					half xDis = saturate(0.5 - abs(i.uv.x));
					half yDis = saturate(0.5 - abs(i.uv.y));
					half rect = xDis*yDis;
					half mask = sign(rect);
					half cull = (saturate(_Arg.z - xDis) + saturate(_Arg.z - yDis))*_Arg.w;
					xDis = 0.5 -xDis;
					half dis = _Arg.y+_Arg.x*xDis*xDis;				
					fixed4 color = _TintColor;
					color.a *=dis;
				
					return (color+ cull*_OutlineColor)*mask;
				}
				ENDCG 
			}
		}	
	}
}