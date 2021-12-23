Shader "PM/PMBlend"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Cull Off
        ZWrite Off

        Blend OneMinusDstColor One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "PMHelper.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            pm_v2f_uv vert (pm_appdata_uv v)
            {
                pm_v2f_uv o;
                PM_INITVERTEX(o, v)
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv.y = 1.0f - o.uv.y;
                return o;
            }

            fixed4 frag (pm_v2f_uv i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                return col;
            }
            ENDCG
        }
    }
}