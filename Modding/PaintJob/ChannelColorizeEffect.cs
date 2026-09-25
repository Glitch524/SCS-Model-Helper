using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace SCS_Mod_Helper.Modding.PaintJob; 
public class ChannelColorizeEffect : ShaderEffect {
	private static readonly PixelShader _pixelShader = new() {
		UriSource = new Uri(@"\Modding\PaintJob\ChannelColorizeEffect.ps", UriKind.Relative)
	};

	public ChannelColorizeEffect() {
		this.PixelShader = _pixelShader;
		UpdateShaderValue(InputProperty);
		UpdateShaderValue(TargetColorProperty);
	}

	public Brush Input {
		get => (Brush)GetValue(InputProperty); 
		set => SetValue(InputProperty, value);
	}
	public static readonly DependencyProperty InputProperty = 
		ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(ChannelColorizeEffect), 0);

	public Color TargetColor {
		get => (Color)GetValue(TargetColorProperty);
		set => SetValue(TargetColorProperty, value);
	}

	public static readonly DependencyProperty TargetColorProperty =
		DependencyProperty.Register("TargetColor", typeof(Color), typeof(ChannelColorizeEffect),
			new UIPropertyMetadata(Colors.Red, PixelShaderConstantCallback(0)));
}
