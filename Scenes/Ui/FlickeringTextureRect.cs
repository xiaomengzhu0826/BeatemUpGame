using Godot;
using System;

public partial class FlickeringTextureRect : TextureRect
{
	[Export] private int _durationFlicker;
	[Export] private int _totalFlickers;

	private int _flickersLeft = 0;
	private Texture2D _image = null;
	private bool _isFlickering;
	private float _timeStartLastFlicker = Time.GetTicksMsec();

	public override void _Ready()
	{
		_image = Texture;
		Texture = null;
	}

	public override void _Process(double delda)
	{
		if (_isFlickering && (Time.GetTicksMsec() - _timeStartLastFlicker) > _durationFlicker)
		{
			if (Texture == null)
			{
				if (_flickersLeft == 0)
				{
					_isFlickering = false;
				}
				else
				{
					_flickersLeft -= 1;
					Texture = _image;
				}
			}
			else
			{
				Texture = null;
			}
			_timeStartLastFlicker=Time.GetTicksMsec();
		}
	}

	public void StartFlickering()
	{
		_flickersLeft = _totalFlickers;
		_isFlickering = true;
		_timeStartLastFlicker = Time.GetTicksMsec();
		SoundManager.Instance.Play(SoundManager.SoundType.GOGOGO);
	}


}
