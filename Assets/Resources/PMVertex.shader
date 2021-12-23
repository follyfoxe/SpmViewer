Shader "PM/PMVertex"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "PMHelper.cginc"

            pm_v2f vert (pm_appdata v)
            {
                pm_v2f o;
                PM_INITVERTEX(o, v)
                return o;
            }

            fixed4 frag (pm_v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}