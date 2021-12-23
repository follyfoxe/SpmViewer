struct pm_appdata
{
    float4 vertex : POSITION;
    fixed4 color : COLOR0;
};

struct pm_appdata_uv
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    fixed4 color : COLOR0;
};

struct pm_v2f
{
    float4 vertex : SV_POSITION;
    fixed4 color : COLOR0;
};

struct pm_v2f_uv
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
    fixed4 color : COLOR0;
};

#define PM_INITVERTEX(o, v) \
o.vertex = UnityObjectToClipPos(v.vertex); \
o.color = v.color;