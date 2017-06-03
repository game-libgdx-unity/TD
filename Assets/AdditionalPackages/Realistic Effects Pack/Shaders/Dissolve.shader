Shader "Effects/Dissolve" {
	Properties {
        _Color ("Color", Color) = (1,1,1,1)
		_CoreColor ("Core Color", Color) = (1,1,1,1)
		_Core("Core Texture", 2D) = "white" {}
        _Mask("Mask To Dissolve", 2D) = "white" {}
		_ColorStrength("Core Color Strength", Float) = 100
        _Range ("Range [0-2]", Range(0, 2)) = 0
    }

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	Cull Off 
	Lighting Off 
	ZWrite Off 
	Fog { Color (0,0,0,0) }
	
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
		
			#include "UnityCG.cginc"

			sampler2D _Core;
			sampler2D _Mask;
			half4 _Color;
			half4 _CoreColor;
			float _Range;
			float _ColorStrength;
			
			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				float2 uv_Core : TEXCOORD0;
				float2 uv_Mask : TEXCOORD1;
			};
			
			float4 _Core_ST;
			float4 _Mask_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv_Core = TRANSFORM_TEX(v.texcoord, _Core);
				o.uv_Mask = TRANSFORM_TEX(v.texcoord, _Mask);
				return o;
			}

			sampler2D _CameraDepthTexture;
			float _InvFade;
			
			fixed4 frag (v2f i) : COLOR
			{
				half4 core = tex2D (_Core, i.uv_Core) * _CoreColor * _ColorStrength;
				half4 mask = tex2D (_Mask, i.uv_Mask);
             
				fixed delta = 2 - _Range;
				fixed4 col = core * mask.a - delta;
				fixed4 res;
				if(mask.a >= _Range - 2)
				res = core/_ColorStrength + col;
				else 
				res = 0;
				
				fixed4 emission;
				if(res.r < 0) 
				emission = 0;
				else 
				emission = res;
				emission.a = saturate(core.a * _Range);

				return emission;
			}
			ENDCG 
		}
	}	
}
}
