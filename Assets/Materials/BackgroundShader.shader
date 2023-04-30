Shader "thquinn/BackgroundShader"
{
    Properties
    {
        _BGColor ("BG Color", Color) = (1,1,1,1)
        _GridColor ("Grid Color", Color) = (1,1,1,1)
        _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        _GrainTex ("Grain (RGB)", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
#pragma vertex vert
#pragma fragment frag
            // make fog work
#pragma multi_compile_fog

#include "UnityCG.cginc"

        struct appdata
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct v2f
    {
        float4 vertex : SV_POSITION;
        float4 worldSpacePos : TEXCOORD3;
    };

    fixed4 _BGColor, _GridColor, _EdgeColor;
    sampler2D _GrainTex;
    float _ArenaRadius = 100;

    v2f vert (appdata v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.worldSpacePos = mul(unity_ObjectToWorld, v.vertex);
        return o;
    }

    float2 rotateUV(float2 uv, float rotation)
    {
        float mid = 0.5;
        return float2(
            cos(rotation) * (uv.x - mid) + sin(rotation) * (uv.y - mid) + mid,
            cos(rotation) * (uv.y - mid) - sin(rotation) * (uv.x - mid) + mid
            );
    }
    fixed4 frag (v2f i) : SV_Target
    {
        // Primary grid.
        float2 gridUV = i.worldSpacePos;
        gridUV *= .1;
        gridUV = rotateUV(gridUV, .75);
        gridUV = frac(gridUV);
        float tx = abs(gridUV.x - .5);
        float ty = abs(gridUV.y - .5);
        float t = max(tx, ty);
        t = smoothstep(.475, .5, t);
        // Secondary grid.
        float2 grid2UV = i.worldSpacePos;
        grid2UV *= .1;
        grid2UV.x -=_WorldSpaceCameraPos.x * .01 + _Time * .05;
        grid2UV = rotateUV(grid2UV, .75);
        grid2UV = frac(grid2UV);
        float t2x = abs(grid2UV.x - .5);
        float t2y = abs(grid2UV.y - .5);
        float t2 = max(t2x, t2y);
        t2 = smoothstep(.475, .5, t2);
        // Grain.
        float2 grainUV = rotateUV(i.worldSpacePos * .02, 1);
        float grain = 1 - tex2D(_GrainTex, grainUV);
        // Combine.
        fixed4 c = lerp(_BGColor, _GridColor, t + t2 * .4 + grain * 3);
        // Edge fade.
        float distance = length(i.worldSpacePos.xy);
        float arenaRadius = max(100, _ArenaRadius);
        float edgeT = smoothstep(arenaRadius, arenaRadius + 20, distance);
        c = lerp(c, _EdgeColor, edgeT);
        return c;
    }
        ENDCG
    }
    }
}
