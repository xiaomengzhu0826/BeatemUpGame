using Godot;
using System;

public partial class HealthBar : Control
{
	private ColorRect _contentBackground;
	private ColorRect _whiteBorder;
	private TextureRect _healthGaughe;

	public override void _Ready()
	{
		_contentBackground = GetNode<ColorRect>("ContentBackground");
		_whiteBorder = GetNode<ColorRect>("WhiteBorder");
		_healthGaughe = GetNode<TextureRect>("TextureRect");
	}

	public void Refresh(int currentHealth, int maxHealth)
	{
		_whiteBorder.Scale = new Vector2(maxHealth+2, _whiteBorder.Scale.Y);
		_contentBackground.Scale = new Vector2(maxHealth, _contentBackground.Scale.Y);
		_healthGaughe.Scale = new Vector2(currentHealth, _healthGaughe.Scale.Y);
	}
}
