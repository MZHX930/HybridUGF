//绘制透明圆
Shader "Custom/GuideHighlight"
{
    Properties
    {
        // 基础属性
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {} // 精灵纹理（支持运行时修改）
        _Color ("Tint", Color) = (1,1,1,1)                            // 颜色叠加
        _ColorMask ("Color Mask", Float) = 15            // 颜色通道掩码（默认RGBA）
        // 自定义参数
        _Center("Center",vector) = (0,0,0,0)  // 圆心坐标（屏幕像素坐标）
        _Radius("Radius",Range(0,2000))=150    // 圆半径（像素）
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"         // 透明渲染队列
            "IgnoreProjector"="True"      // 禁用投影
            "RenderType"="Transparent"    // 渲染类型标识
            "PreviewType"="Plane"         // 在预览窗口显示为平面
            "CanUseSpriteAtlas"="True"    // 支持精灵图集
        }

        // 模板测试配置
        Stencil
        {
            Ref [_Stencil]                // 参考值
            Comp [_StencilComp]          // 比较函数
            Pass [_StencilOp]            // 通过时的操作
            ReadMask [_StencilReadMask]   // 读取掩码
            WriteMask [_StencilWriteMask] // 写入掩码
        }

        // 渲染状态
        Cull Off          // 关闭背面剔除
        Lighting Off      // 关闭光照
        ZWrite Off        // 关闭深度写入
        Blend SrcAlpha OneMinusSrcAlpha // 标准Alpha混合
        ColorMask [_ColorMask] // 颜色通道控制

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc" // UI专用函数

            // 编译指令
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT    // 启用UI矩形裁剪
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP    // 启用Alpha裁剪

            // 顶点输入结构
            struct appdata_t
            {
                float4 vertex   : POSITION;  // 本地空间顶点位置
                float4 color    : COLOR;     // 顶点颜色
                float2 texcoord : TEXCOORD0; // 纹理坐标
                UNITY_VERTEX_INPUT_INSTANCE_ID // 实例ID（用于GPU实例化）
            };

            // 顶点输出结构
            struct v2f
            {
                float4 vertex   : SV_POSITION; // 裁剪空间位置
                fixed4 color    : COLOR;        // 颜色信息
                float2 texcoord  : TEXCOORD0;  // 纹理坐标
                float4 worldPosition : TEXCOORD1; // 本地空间位置（注意：实际是本地空间）
                float4  mask : TEXCOORD2;       // 裁剪遮罩数据
                // UNITY_VERTEX_OUTPUT_STEREO // VR单眼渲染支持
            };

            // 着色器参数
            sampler2D _MainTex;        // 主纹理
            fixed4 _Color;             // 颜色叠加
            fixed4 _TextureSampleAdd;  // 纹理采样附加值（图集使用）
            float4 _ClipRect;          // UI裁剪矩形
            float4 _MainTex_ST;         // 纹理缩放偏移
            
            // 自定义参数
            float2 _Center;  // 圆心坐标（注意坐标系取决于使用环境）
            float _Radius;   // 控制过渡效果的阈值

            // 顶点着色器
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v); // 初始化实例ID
                
                // 顶点位置变换
                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex; // 注意：这里实际存储的是本地空间坐标
                OUT.vertex = vPosition;
                
                // 纹理坐标变换
                OUT.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                
                // 颜色计算
                OUT.color = v.color * _Color;
                return OUT;
            }

            // 片段着色器
            fixed4 frag(v2f IN) : SV_Target
            {
                // 基础颜色采样
                half4 color = IN.color * (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);
                
                // 计算当前片元到圆心的距离
                float checkDistance = distance(IN.worldPosition.xy, _Center.xy);
                color.a *= step(_Radius, checkDistance); // 圆外不透明，圆内透明

                return color;
            }
            ENDCG
        }
    }
}