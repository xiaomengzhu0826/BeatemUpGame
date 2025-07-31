using Godot;
using System;

public partial class ActivableControl : HBoxContainer
{
	[Signal] public delegate void OnValueChangedEventHandler(int value);

	[Export] protected string _text;
	[Export] protected Color _colorDefault;
	[Export] protected Color _colorActive;
	[Export] protected int _maxValue;
	[Export] protected int _minValue;

	protected Label _label;
	protected bool _isActive;
	[Export] public int _currentValue;

	public override void _Ready()
	{
		_label = GetNode<Label>("Label");
		_label.Text = _text;
		SetValue(_currentValue);
	}

	public void SetActive(bool active)
	{
		_isActive = active;
		foreach (Control child in GetChildren())
		{
			child.Modulate = _isActive ? _colorActive : _colorDefault;
		}
	}

	public void SetValue(int value)
	{
		_currentValue = Mathf.Clamp(value, _minValue, _maxValue);
		//GD.Print(_currentValue);
		CallDeferred(MethodName.Refresh);
		//Refresh();
		EmitSignal(SignalName.OnValueChanged,_currentValue);
	}

    protected virtual void Refresh()
    {
       
    }

}
