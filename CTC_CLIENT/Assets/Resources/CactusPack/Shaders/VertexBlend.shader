Shader "CactusPack/VertexBlend" {
	Properties {
		_MainTex ("Base (RGBA)", 2D) = "white" {}
		_Layer1("Layer 1 (RGB)", 2D) ="black" {}
		_BlendMask("Blend Mask", 2D) = "white" {}
		_SelfIllum("Self Illumination", Color) = (0,0,0,0)
		_BlendSoft("Blend Softness", Range(0,0.8)) = 0.1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex, _Layer1, _BlendMask;
		half3 _SelfIllum;
		half _BlendSoft;

		struct Input {
			half2 uv_MainTex;
			half4 color : COLOR;
		};


		void surf (Input IN, inout SurfaceOutput o) {
			
			half3 base = tex2D(_MainTex, IN.uv_MainTex);
			half3 blend1 = tex2D(_Layer1, IN.uv_MainTex);
			half3 blendmask = tex2D(_BlendMask, IN.uv_MainTex);
			half transformed = ((1 - IN.color.g) - blendmask.r)/_BlendSoft;
			
			half mask = saturate(transformed);	
			half3 finresult = lerp(base, blend1, mask);	

			o.Albedo = finresult.rgb;
			o.Alpha = 1;
			o.Emission = _SelfIllum.rgb * finresult.rgb;			
		}
		ENDCG
	} 
	FallBack "Mobile/VertexLit"
}
