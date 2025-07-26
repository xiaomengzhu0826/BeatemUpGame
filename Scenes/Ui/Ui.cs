using Godot;
using System;

public partial class Ui : CanvasLayer
{

	[Export] private int _durationHealthbarVisible;

	private HealthBar _playerHealthbar;
	private HealthBar _enemyHealthbar;
	private TextureRect _enmeyAvatar;
	private ComboIndicator _comboIndicator;
	private ScoreIndicator _scoreIndicator;
	private FlickeringTextureRect _goIndicator;

	private float _timeStartHealthbarVisible = Time.GetTicksMsec();

	public override void _Ready()
	{
		_playerHealthbar = GetNode<HealthBar>("UiContainer/PlayerHealthBar");
		_enemyHealthbar = GetNode<HealthBar>("UiContainer/EnemyHealthBar");
		_enmeyAvatar = GetNode<TextureRect>("UiContainer/EnemyAvatar");
		_comboIndicator = GetNode<ComboIndicator>("UiContainer/ComboIndicator");
		_scoreIndicator = GetNode<ScoreIndicator>("UiContainer/ScoreIndicator");
		_goIndicator= GetNode<FlickeringTextureRect>("UiContainer/GoIndicator");

		_comboIndicator.OnComboReset += OnComboReset;
		SignalManager.Instance.OnHealthChange += OnHealthChange;
		SignalManager.Instance.OnCheckPointCompelete += OnCheckPointCompelete;

		_enmeyAvatar.Visible = false;
		_enemyHealthbar.Visible = false;
	}

	private void OnComboReset(int score)
	{
		_scoreIndicator.AddCombo(score);
	}

	public override void _Process(double delta)
	{
		if (_enemyHealthbar.Visible && Time.GetTicksMsec() - _timeStartHealthbarVisible > _durationHealthbarVisible)
		{
			_enmeyAvatar.Visible = false;
			_enemyHealthbar.Visible = false;
		}
	}

	private void OnHealthChange(Character.Type characterType, int currentHealth, int maxHealth)
	{
		if (characterType == Character.Type.PLAYER)
		{
			_playerHealthbar.Refresh(currentHealth, maxHealth);
		}
		else
		{
			_timeStartHealthbarVisible = Time.GetTicksMsec();
			_enmeyAvatar.Texture = PrefabManager.AVATAR_PREFAB[characterType];
			_enemyHealthbar.Refresh(currentHealth, maxHealth);
			_enmeyAvatar.Visible = true;
			_enemyHealthbar.Visible = true;
		}
	}
	
	private void OnCheckPointCompelete()
    {
		_goIndicator.StartFlickering();
    }

}
