// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:False,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:False,qofs:0,qpre:3,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:32719,y:32712,varname:node_4013,prsc:2|diff-4378-OUT,emission-422-OUT,clip-2184-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:31369,y:31458,ptovrint:False,ptlb:Main_Color,ptin:_Main_Color,varname:node_1304,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:916,x:31369,y:31633,ptovrint:False,ptlb:Main_Tex,ptin:_Main_Tex,varname:node_916,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:4f5ac4ab66470d84f8bb468e8d57c777,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:4378,x:31770,y:31651,varname:node_4378,prsc:2|A-1304-RGB,B-916-RGB;n:type:ShaderForge.SFN_Tex2d,id:1612,x:30006,y:31993,ptovrint:False,ptlb:Clip_Tex,ptin:_Clip_Tex,varname:node_1612,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:415,x:30682,y:32602,ptovrint:False,ptlb:Clip_Str,ptin:_Clip_Str,varname:node_415,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Tex2d,id:3199,x:31557,y:33034,ptovrint:False,ptlb:SFX_Tex,ptin:_SFX_Tex,varname:node_3199,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1651-OUT;n:type:ShaderForge.SFN_Color,id:4458,x:31628,y:32646,ptovrint:False,ptlb:SFX_Color,ptin:_SFX_Color,varname:node_4458,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:422,x:31961,y:32852,varname:node_422,prsc:2|A-4458-RGB,B-3199-RGB,C-3575-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3575,x:32003,y:33164,ptovrint:False,ptlb:SFX_Str,ptin:_SFX_Str,varname:node_3575,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:8262,x:30257,y:32419,ptovrint:False,ptlb:Clip_B,ptin:_Clip_B,varname:node_8262,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:5765,x:30324,y:31761,ptovrint:False,ptlb:Clip_R,ptin:_Clip_R,varname:node_5765,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:3017,x:30282,y:32162,ptovrint:False,ptlb:Clip_G,ptin:_Clip_G,varname:node_3017,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:5406,x:30555,y:31744,varname:node_5406,prsc:2|A-1612-R,B-5765-OUT;n:type:ShaderForge.SFN_Multiply,id:6632,x:30560,y:32086,varname:node_6632,prsc:2|A-1612-G,B-3017-OUT;n:type:ShaderForge.SFN_Multiply,id:1549,x:30573,y:32393,varname:node_1549,prsc:2|A-1612-B,B-8262-OUT;n:type:ShaderForge.SFN_Add,id:2319,x:30997,y:32078,varname:node_2319,prsc:2|A-5406-OUT,B-6632-OUT,C-1549-OUT;n:type:ShaderForge.SFN_RemapRange,id:8919,x:31237,y:32363,varname:node_8919,prsc:2,frmn:0,frmx:1,tomn:0.6,tomx:-0.8|IN-415-OUT;n:type:ShaderForge.SFN_Add,id:4495,x:31578,y:32237,varname:node_4495,prsc:2|A-2319-OUT,B-8919-OUT;n:type:ShaderForge.SFN_Clamp01,id:2184,x:32247,y:32683,varname:node_2184,prsc:2|IN-4495-OUT;n:type:ShaderForge.SFN_Time,id:6823,x:30157,y:32894,varname:node_6823,prsc:2;n:type:ShaderForge.SFN_TexCoord,id:3489,x:30354,y:32894,varname:node_3489,prsc:2,uv:0;n:type:ShaderForge.SFN_Panner,id:6969,x:30736,y:33068,varname:node_6969,prsc:2,spu:0,spv:1|UVIN-3489-UVOUT,DIST-2788-OUT;n:type:ShaderForge.SFN_Panner,id:9092,x:30722,y:32768,varname:node_9092,prsc:2,spu:1,spv:0|UVIN-3489-UVOUT,DIST-5165-OUT;n:type:ShaderForge.SFN_Multiply,id:2788,x:30539,y:33095,varname:node_2788,prsc:2|A-4358-OUT,B-6823-T;n:type:ShaderForge.SFN_Multiply,id:5165,x:30520,y:32693,varname:node_5165,prsc:2|A-989-OUT,B-6823-T;n:type:ShaderForge.SFN_ValueProperty,id:4358,x:30306,y:33155,ptovrint:False,ptlb:SFX_V,ptin:_SFX_V,varname:node_4358,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:989,x:30273,y:32741,ptovrint:False,ptlb:SFX_U,ptin:_SFX_U,varname:node_989,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Lerp,id:1651,x:31003,y:32869,varname:node_1651,prsc:2|A-9092-UVOUT,B-6969-UVOUT,T-6284-OUT;n:type:ShaderForge.SFN_Vector1,id:6284,x:30897,y:33068,varname:node_6284,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Slider,id:9403,x:33238,y:33413,ptovrint:False,ptlb:Tex_Clips_copy_copy_copy_copy_copy_copy_copy_copy_copy,ptin:_Tex_Clips_copy_copy_copy_copy_copy_copy_copy_copy_copy,varname:_Tex_Clips_copy_copy_copy_copy_copy_copy_copy_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_RemapRange,id:4577,x:33576,y:33397,varname:node_4577,prsc:2,frmn:0,frmx:1,tomn:1,tomx:-1|IN-9403-OUT;n:type:ShaderForge.SFN_Add,id:8148,x:33745,y:33307,varname:node_8148,prsc:2|B-4577-OUT;n:type:ShaderForge.SFN_Clamp01,id:9739,x:33911,y:33324,varname:node_9739,prsc:2|IN-8148-OUT;proporder:1304-916-4458-3199-415-3575-989-4358-5765-3017-8262-1612;pass:END;sub:END;*/

Shader "Custom/Effect/Rol" {
    Properties {
        _Main_Color ("Main_Color", Color) = (1,1,1,1)
        _Main_Tex ("Main_Tex", 2D) = "white" {}
        _SFX_Color ("SFX_Color", Color) = (0.5,0.5,0.5,1)
        _SFX_Tex ("SFX_Tex", 2D) = "white" {}
        _Clip_Str ("Clip_Str", Range(0, 1)) = 1
        _SFX_Str ("SFX_Str", Float ) = 0
        _SFX_U ("SFX_U", Float ) = 0
        _SFX_V ("SFX_V", Float ) = 0
		_Clip("Clip", Vector) = (1,0,0,0)
        _Clip_Tex ("Clip_Tex", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {            
			Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            uniform float4 _Main_Color;
            uniform sampler2D _Main_Tex; 
			uniform float4 _Main_Tex_ST;
            uniform sampler2D _Clip_Tex; 
			uniform float4 _Clip_Tex_ST;
            uniform float _Clip_Str;
            uniform sampler2D _SFX_Tex;
			uniform float4 _SFX_Tex_ST;
            uniform float4 _SFX_Color;
            uniform float _SFX_Str;
			half4 _Clip;
            uniform float _SFX_V;
            uniform float _SFX_U;

            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
				half2 uv0 : TEXCOORD0;
				half2 uv1 : TEXCOORD1;
				half2 uv2 : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = TRANSFORM_TEX(v.texcoord0, _Main_Tex);
				o.uv1 = lerp(half2(o.uv0.x+ _SFX_U*_Time.g, o.uv0.y), half2(o.uv0.x, o.uv0.y+ _SFX_V*_Time.g), 0.5);
				o.uv1 = TRANSFORM_TEX(o.uv1, _SFX_Tex);
				o.uv2 = TRANSFORM_TEX(o.uv0, _Clip_Tex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR 
			{
				fixed4 _Main_Tex_var = tex2D(_Main_Tex, i.uv0);
				fixed4 _SFX_Tex_var = tex2D(_SFX_Tex, i.uv1);
				fixed4 _Clip_Tex_var = tex2D(_Clip_Tex,i.uv2);
				

				float c = dot(_Clip_Tex_var.rgb, _Clip.rgb)+_Clip_Str*-1.4+0.1;
				c = step(0.01, c);
				//clip(c);

				fixed3 diffuseColor = _Main_Color.rgb*_Main_Tex_var.rgb;
				fixed3 emissive = _SFX_Color.rgb*_SFX_Tex_var.rgb*_SFX_Str;
				fixed3 finalColor = diffuseColor + emissive;
                return fixed4(finalColor,c);
            }
            ENDCG
        }        
    }
}
