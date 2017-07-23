Shader "CactusPack/Basic_Rimlight" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_RimColor("Rim Color", Color) = (0,0,0,0)
		_RimPower("Rim Power", Range(2, 10)) = 3
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert approxview exclude_path:prepass


		sampler2D _MainTex;
		fixed4 _RimColor;
		fixed _RimPower;

		struct Input {
			float2 uv_MainTex;
			fixed3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o) {

			fixed3 rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));	
			rim = pow(rim, _RimPower) * _RimColor;		

			fixed3 result = tex2D(_MainTex, IN.uv_MainTex) + rim;
			o.Albedo = result.rgb;
			//o.Emission = rim.rgb;
			o.Alpha = 1;
		}
		ENDCG
	} 
	//FallBack "Diffuse"
	FallBack "Mobile/VertexLit"
}
