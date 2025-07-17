using Godot;
using System;

public partial class Ui : CanvasLayer
{
	private HealthBar _playerHealthbar;
	private HealthBar _enemyHealthbar;
	private TextureRect _enmeyAvatar;

	public override void _Ready()
	{
		_playerHealthbar = GetNode<HealthBar>("UiContainer/PlayerHealthBar");
		_enemyHealthbar = GetNode<HealthBar>("UiContainer/EnemyHealthBar");
		_enmeyAvatar = GetNode<TextureRect>("UiContainer/EnemyAvatar");

		SignalManager.Instance.OnHealthChange += OnHealthChange;
	}

	private void OnHealthChange(Character.Type characterType, int currentHealth, int maxHealth)
	{
		if (characterType == Character.Type.PLAYER)
		{
			_playerHealthbar.Refresh(currentHealth, maxHealth);
		}
    }

}
