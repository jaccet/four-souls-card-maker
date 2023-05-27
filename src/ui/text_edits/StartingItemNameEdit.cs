using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;

public partial class StartingItemNameEdit : LineEdit
{
	public void OnTextChanged(string text) {
		Card.instance.SetStartingItemName(text);
	}

	// --- SAVE HANDLING ---
	public virtual Dictionary Save() {
		var dict = new Dictionary();

		dict.Add("Value", Text);

		return dict;
	}

	public virtual void Load(Dictionary data) {
        Text = (string) data["Value"];
		OnTextChanged(Text);
    }
}
