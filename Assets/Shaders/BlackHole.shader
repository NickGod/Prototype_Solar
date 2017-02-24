Shader "Custom/BlackHole" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.5
		_RimColor("Rim Color", Color) = (1,1,1,1)
	}
		SubShader{
			Tags { "RenderType" = "opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Lambert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		};
		sampler2D _MainTex;
		float4 _RimColor;
		float _RimPower;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput o) {
			float2 new_uv = IN.uv_MainTex;
			new_uv.x *=  1+0.3*_SinTime.b;
			new_uv.y *= 1+0.3*_CosTime.b;
			o.Albedo = 0.2 *tex2D(_MainTex, new_uv).rgb;
			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
