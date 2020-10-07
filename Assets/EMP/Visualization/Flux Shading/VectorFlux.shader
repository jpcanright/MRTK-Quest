Shader "EM/VectorFlux"
{
	Properties
	{
		_MainTex ("Color (RGB) Alpha (A)", 2D) = "white" {}
		_PositiveColor ("Positive Color", Color) = (1, 0, 0, 1)
		_NegativeColor ("Negative Color", Color) = (0, 1, 0, 1)
		_Opacity ("Opacity", float) = 0.2
		_ScaleFactor ("Scale Factor", float) = 0.03
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100
		
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
            
            float4 _PositiveColor;
            float4 _NegativeColor;
            float _Opacity;
            float _ScaleFactor;
            
            float4 _Charges[32];
            //float _ChargeStrengths[32];
            int _ChargeArrayLength;
            
			struct VertexData
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct InterpolatorsVertex
			{
				float2 uv : TEXCOORD0;
				float4 worldPos: TEXCOORD1;
				float4 vertex : SV_POSITION;
				float4 normal : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			InterpolatorsVertex vert (VertexData v)
			{
				InterpolatorsVertex o;
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal.xyz = v.normal;
				o.normal.w = 0;
				
				return o;
			}
			
			float3 ElectricField(float3 position)
            {
                float3 field = float3(0,0,0);
                for(int j = 0; j < _ChargeArrayLength; j++)
                {
                   field = field + ((position - _Charges[j].xyz) * _Charges[j].w) / (pow(distance(position, _Charges[j].xyz),3));
                }
                return field;
            }
			
			fixed4 frag (InterpolatorsVertex i) : SV_Target
			{
				
				float4 color;
				float3 field = ElectricField(i.worldPos.xyz);
				
				
				float flux = dot(normalize(mul(unity_ObjectToWorld,i.normal).xyz), field);
				
				if(flux < 0){
				    color = _NegativeColor * lerp(0, 0.5, -flux * _ScaleFactor);
				}
				else {
                    color = _PositiveColor * lerp(0, 0.5, flux  * _ScaleFactor);				    
				}
				
				color.a = _Opacity;
				
				return color;
			}
			ENDCG
		}
	}
}
