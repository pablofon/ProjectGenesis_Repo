Shader"Custom/PlayerShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Multiplier("Multiplier", Range(0, 1)) = 1
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
LOD100

            Pass
            {
Cull Off // Disable backface culling

Blend SrcAlpha
OneMinusSrcAlpha
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
#include "UnityCG.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
};

sampler2D _MainTex;
float4 _MainTex_ST;
fixed4 _Color;
float _Multiplier;

v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    fixed4 col = tex2D(_MainTex, i.uv);
    col.rgb = lerp(col.rgb, _Color.rgb, _Multiplier);
    return col;
}
                ENDCG
            }
        }
}