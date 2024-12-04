using Godot;
using System;

public partial class ValueProps : SelectionProp
{
    [Export] LineEdit lineEdit;

    public override void _Ready() {
        base._Ready();

        // Ensure lineEdit is assigned
        if (lineEdit == null)
        {
            GD.PrintErr("LineEdit is not assigned in the editor!");
            return;
        }

        lineEdit.TextChanged += OnTextChanged;
    }

    public override void HandleArtProperties(MoveableArt art) {
        SetEnabled(art.canSetValue);
        lineEdit.Text = art.canSetValue ? art.value : ""; // Use an empty string instead of null
    }

    private void OnTextChanged(string text) {
        if (Card.instance?.curSelectedArt != null) { // Null check for Card.instance
            Card.instance.curSelectedArt.ChangeValue(text);
        }
    }
}
