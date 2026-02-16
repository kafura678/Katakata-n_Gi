Shader "UI/SuppressionRadialBlock"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _RedColor  ("Uncontrolled Color", Color) = (1,0,0,1)
        _BlueColor ("Controlled Color",   Color) = (0,0.6,1,1)

        _Progress  ("Suppression Progress (0-1)", Range(0,1)) = 0
        _EdgeWidth ("Edge Softness", Range(0.0,0.3)) = 0.0

        _Center ("Invasion Center (UV)", Vector) = (0.5,0.5,0,0)

        _Blocks   ("Blocks Per Axis", Range(4,256)) = 64
        _HardEdge ("Hard Edge (0=smooth, 1=hard)", Range(0,1)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color    : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
                fixed4 color  : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            fixed4 _RedColor;
            fixed4 _BlueColor;

            float  _Progress;
            float  _EdgeWidth;
            float2 _Center;

            float _Blocks;
            float _HardEdge;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseCol = tex2D(_MainTex, i.uv) * i.color;

                // ===== 設定 =====
                float blocks = max(1.0, _Blocks);

                // 縦横比補正（ブロックを正方形に見せる）
                float aspect = _MainTex_TexelSize.z / max(1.0, _MainTex_TexelSize.w);

                float2 uvA = float2(i.uv.x * aspect, i.uv.y);
                float2 cA  = float2(_Center.x * aspect, _Center.y);

                // ===== UV をブロック中心にスナップ =====
                float2 cellUV = floor(uvA * blocks);
                float2 uvQ = (cellUV + 0.5) / blocks;

                // ===== Center もブロック中心にスナップ =====
                float2 cellC = floor(cA * blocks);
                float2 centerQ = (cellC + 0.5) / blocks;

                // ===== 距離計算（ブロック ↔ ブロック）=====
                float dist = distance(uvQ, centerQ);

                // ===== 制圧率 → 半径 =====
                float radius = saturate(_Progress);

                // ===== 境界制御 =====
                float tSmooth = smoothstep(radius - _EdgeWidth, radius + _EdgeWidth, dist);
                float tHard   = step(radius, dist);
                float t = lerp(tSmooth, tHard, saturate(_HardEdge));

                // ===== 色ブレンド =====
                fixed4 col = lerp(_BlueColor, _RedColor, t);
                col.a *= baseCol.a;

                return col;
            }
            ENDCG
        }
    }
}