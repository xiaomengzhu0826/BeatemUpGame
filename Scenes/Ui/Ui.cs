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
	private StageTransition _stageTransition;

	private float _timeStartHealthbarVisible = Time.GetTicksMsec();
	private PackedScene _optionsScreenPrefab = GD.Load<PackedScene>("res://Scenes/Ui/OptionsScreen.tscn");
	private PackedScene _deathScreenPrefab = GD.Load<PackedScene>("res://Scenes/Ui/DeathScreen.tscn");
	private PackedScene _gameOverPrefab = GD.Load<PackedScene>("res://Scenes/Ui/GameOverScreen.tscn");
	private OptionsScreen _optionsScreen = null;
	private DeathScreen _deathScreen = null;
	private GameOverScreen _gameOverScreen = null;

	public override void _Ready()
	{
		_playerHealthbar = GetNode<HealthBar>("UiContainer/PlayerHealthBar");
		_enemyHealthbar = GetNode<HealthBar>("UiContainer/EnemyHealthBar");
		_enmeyAvatar = GetNode<TextureRect>("UiContainer/EnemyAvatar");
		_comboIndicator = GetNode<ComboIndicator>("UiContainer/ComboIndicator");
		_scoreIndicator = GetNode<ScoreIndicator>("UiContainer/ScoreIndicator");
		_goIndicator = GetNode<FlickeringTextureRect>("UiContainer/GoIndicator");
		_stageTransition=GetNode<StageTransition>("%StageTransition");

		_comboIndicator.OnComboReset += OnComboReset;
		SignalManager.Instance.OnHealthChange += OnHealthChange;
		SignalManager.Instance.OnCheckPointCompelete += OnCheckPointCompelete;
		SignalManager.Instance.OnStageCompelete += OnStageCompelete;

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
		HandleInput();
	}

	private void HandleInput()
	{
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			if (_optionsScreen == null)
			{
				_optionsScreen = (OptionsScreen)_optionsScreenPrefab.Instantiate();
				_optionsScreen.OnExit += Unpause;
				AddChild(_optionsScreen);
				GetTree().Paused = true;
			}
			else
			{
				Unpause();
			}
		}
	}

	private void Unpause()
	{
		_optionsScreen.QueueFree();
		_optionsScreen = null;
		GetTree().Paused = false;
	}

	private void OnHealthChange(Character.Type characterType, int currentHealth, int maxHealth)
	{
		if (characterType == Character.Type.PLAYER)
		{
			_playerHealthbar.Refresh(currentHealth, maxHealth);
			if (currentHealth == 0 && _deathScreen == null)
			{
				_deathScreen = (DeathScreen)_deathScreenPrefab.Instantiate();
				_deathScreen.OnGameOver += OnGameOver;
				AddChild(_deathScreen);
			}
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

	private void OnGameOver()
	{
		if (_gameOverScreen == null)
		{
			_gameOverScreen = (GameOverScreen)_gameOverPrefab.Instantiate();
			_gameOverScreen.SetScore(_scoreIndicator._realScore);
			AddChild(_gameOverScreen);
		}
	}

	private void OnCheckPointCompelete(CheckPoint checkPoint)
	{
		_goIndicator.StartFlickering();
	}
	
	private void OnStageCompelete()
    {
		_stageTransition.StartTransition();
    }

}
