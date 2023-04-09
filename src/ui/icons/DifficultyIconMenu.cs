using Godot;
using System;

public partial class DifficultyIconMenu : IconMenu
{
	public override void _Ready() {
		base._Ready();

		linkedArt = Card.instance.diffIcon;
		linkedArt.trashCallable = new Callable(this, "ResetSelection");
		
		UpdateItems();

		customTextureCallback = new Callable(this, "SetCustomDifficultyIcon");
	}

	public override void OnItemSelected(long index) {
		base.OnItemSelected(index);
		
		if (index == customId) {
			return;
		}

		var selectedIcon = cardTypes[(int) index] as DifficultyIcon;

		Card.instance.SetDifficultyIcon(selectedIcon);
	}

	public void SetCustomDifficultyIcon(string path, Texture2D texture) {
		customTexturePath = path;
		Card.instance.SetCustomDifficultyIcon(texture);
	}
}
