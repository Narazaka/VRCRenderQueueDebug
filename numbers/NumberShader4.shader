Shader "NumberShader4"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "black" {}
        _TexSizeX("Tex Size X", int) = 32
        _TexSizeY("Tex Size Y", int) = 32
        _UseSizeX("Use Region X", int) = 32
        _UseSizeY("Use Region Y", int) = 32
        _Col("Column Count", int) = 4
        _Row("Row Count", int) = 3
        _Number1000("Number1000", int) = 0
        _Number100("Number100", int) = 0
        _Number10("Number10", int) = 0
        _Number1("Number1", int) = 0
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
            float _TexSizeX;
            float _TexSizeY;
            float _UseSizeX;
            float _UseSizeY;
            float _Col;
            float _Row;
            float _Number1000;
            float _Number100;
            float _Number10;
            float _Number1;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                uint col = (uint)_Col;
                uint row = (uint)_Row;
                uint number1000 = (uint)_Number1000;
                uint number100 = (uint)_Number100;
                uint number10 = (uint)_Number10;
                uint number1 = (uint)_Number1;
                float2 texOffsets[4] = {
                    float2(number1000 % col, row - 1 - number1000 / col),
                    float2(number100 % col, row - 1 - number100 / col),
                    float2(number10 % col, row - 1 - number10 / col),
                    float2(number1 % col, row - 1 - number1 / col),
                };
                int position = (int)(i.uv.x * 4);
                half2x2 preScaleMatrix = half2x2(4, 0, 0, 1);
                i.uv = mul(i.uv, preScaleMatrix);
                i.uv += texOffsets[position] - float2(position, 0);
                half2x2 scaleMatrix = half2x2(1.0 / col, 0, 0, 1.0 / row);
                i.uv = mul(i.uv, scaleMatrix);
                // sample the texture
                float useX = _UseSizeX / _TexSizeX;
                float useY = _UseSizeY / _TexSizeY;
                float useYOffset = 1 - useY;
                i.uv = mul(i.uv, half2x2(useX, 0, 0, useY)) + float2(0, useYOffset);
                fixed4 rcol = tex2D(_MainTex, i.uv);
                return rcol;
            }
            ENDCG
        }
    }
}
