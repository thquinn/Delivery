Shader "thquinn/SpriteGradientShader"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_LightColor ("Light", Color) = (1,1,1,1)
		_DarkColor ("Dark", Color) = (0,0,0,1)
		_GradientScale ("Gradient Scale", float) = 1
	}

		SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
		"RenderType"="Transparent" 
		"PreviewType"="Plane"
		"CanUseSpriteAtlas"="True"
			"DisableBatching"="True"
	}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

			struct appdata_t
	{
		float4 vertex   : POSITION;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex   : SV_POSITION;
		float2 texcoord  : TEXCOORD0;
		float2 localcoord : TEXCOORD1;
	};

	fixed4 _LightColor, _DarkColor;
	float _GradientScale;

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.vertex = UnityObjectToClipPos(IN.vertex);
		OUT.texcoord = IN.texcoord;
		float4x4 objectToWorldRotation = unity_ObjectToWorld;
		objectToWorldRotation[0][3] = 0;
		objectToWorldRotation[1][3] = 0;
		objectToWorldRotation[2][3] = 0;
		OUT.localcoord = mul(objectToWorldRotation, IN.vertex);
		return OUT;
	}

	sampler2D _MainTex;

	fixed4 SampleSpriteTexture (float2 uv)
	{
		fixed4 color = tex2D (_MainTex, uv);
		return color;
	}

	fixed4 frag(v2f IN) : SV_Target
	{
		float g = IN.localcoord.x + IN.localcoord.y;
		g = (g - _GradientScale * .5) / _GradientScale;
		g = clamp(-g, 0, 1);
		fixed4 gColor = lerp(_LightColor, _DarkColor, g);
		fixed4 c = SampleSpriteTexture(IN.texcoord) * gColor;
		c.rgb *= c.a;
		return c;
	}
		ENDCG
	}
	}
}