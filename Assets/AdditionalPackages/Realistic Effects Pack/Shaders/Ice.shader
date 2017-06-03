Shader "Effects/Frozen" {
Properties {
	_Color ("Color", Color) = (1,1,1,1)
	_MainTex ("Ice (R) Overlay (GB) Texture", 2D) = "white" {}
	_CutOut ("Cutout", Range (0, 1)) = 0.3
}

SubShader {
Pass {

	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transperent"}
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off 
	//Lighting Off 
	ZWrite Off

	
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

sampler2D _MainTex;
fixed4 _Color;
float _CutOut;

struct appdata_t {
	float4 vertex : POSITION;
	float2 texcoord : TEXCOORD0;
};

struct v2f {
	float4 vertex : POSITION;
	float2 texcoord : TEXCOORD0;
};

float4 _MainTex_ST;

v2f vert (appdata_t v)
{
	v2f o;
	o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
	o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
	return o;
}

fixed4 frag (v2f i) : COLOR
{
	fixed4 tex = tex2D(_MainTex, i.texcoord);
	//fixed alpha = _CutOut > (1-tex.b) ? tex.g : saturate(tex.g - (1 - _CutOut));
	fixed alpha = saturate(tex.g - (1 - _CutOut)*_Color.a);
	alpha = alpha * alpha;
	fixed4 res = tex.r * _Color*3 + 0.5;
	res.a = alpha;
	return res;
}
	
	//fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	//o.Alpha = _CutOut > (1-tex.b) ? tex.g : saturate(tex.g - (1 - _CutOut));
	//o.Albedo = tex.r * _Color*3 + 0.5;

ENDCG
		}
	}
}