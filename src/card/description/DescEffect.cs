using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class DescEffect : DescBase
{
	[Export] public RichTextLabel richText;

	[ExportGroup("Icons")]
	[Export] Texture2D hpIcon;
	[Export] Texture2D atkIcon;
	[Export] Texture2D diceIcon;

	string unprocessedText;

	int baseFontSize;
	public float userScale = 1.0f;
	public float systemScale = 1.0f;
	int curFontSize;

	float baseWidth;
	public float boundsMul = 1.0f;

	int baseLineSpacing;
	public int lineSpacingDelta;

	FontVariation fontVar;
	public int characterSpacing;

	public override void _Ready() {
		base._Ready();

		baseFontSize = richText.GetThemeFontSize("normal_font_size");
		baseLineSpacing = richText.GetThemeConstant("line_separation");
		baseWidth = richText.Size.X;

		var font = richText.GetThemeFont("normal_font").Duplicate();
		fontVar = (FontVariation) font;
	}

	public override void _Process(double delta) {
		base._Process(delta);
	}

	public void UpdateSize() {
		var height = richText.Size.Y;
		CustomMinimumSize = Size = new Vector2(0, height + padding);
		richText.Position = new Vector2(container.Size.X / 2 - richText.Size.X / 2, padding/2);
	}

	public void SetText(string text, bool textChanged = false) {
		unprocessedText = text;

		richText.Text = ProcessText(unprocessedText);

		ResetRichTextSize();

		if (textChanged) {
			EmitSignal(SignalName.OnAnySizeChange);
		}
	}

	string ProcessText(string text) {
		if (text == null) {
			return text;
		}

		// Remove empty lines with no text after
		var splitText = text.Split("\n");
		List<string> splitTextList = new();

		foreach (var line in splitText) {
			if (string.IsNullOrWhiteSpace(line)) {
				continue;
			}

			splitTextList.Add(line);
		}

		text = string.Join("\n", splitTextList.ToArray());

		// Center the text
		text = "[center]" + text;

		// Replace icons
		text = ReplaceIconsInText(text, "[HP]", hpIcon);
		text = ReplaceIconsInText(text, "[ATK]", atkIcon);
		text = ReplaceIconsInText(text, "[DICE]", diceIcon);

		return text;
	}

	string ReplaceIconsInText(string text, string key, Texture2D icon) {
		var path = icon.ResourcePath;

		int targetHeight = (int) (curFontSize);
		int iconHeight = icon.GetHeight();
		int iconWidth = icon.GetWidth();

		float ratio = (float) targetHeight / (float) iconHeight;

		int targetWidth = (int) (ratio * iconWidth);
		
		string newVal = $"[img=bottom,bottom,{targetWidth}x{targetHeight}]{path}[/img]";

		return text.Replace(key, newVal);
	}

	public void SetUserScale(float value) {
		userScale = value;

		ResetRichTextSize();

		EmitSignal(SignalName.OnAnySizeChange);
	}

	public void SetSystemScale(float value) {
		systemScale = value;

		ResetRichTextSize();
	}

	public override void SetPadding(int value) {
		base.SetPadding(value);

		UpdateSize();

		EmitSignal(SignalName.OnAnySizeChange);
	}

	public void SetBoundsMul(float value) {
		boundsMul = value;

		ResetRichTextSize();

		EmitSignal(SignalName.OnAnySizeChange);
	}

	public void SetLineSpacing(int value) {
		lineSpacingDelta = value;

		ResetRichTextSize();

		EmitSignal(SignalName.OnAnySizeChange);
	}

	public void SetCharacterSpacing(int value) {
		characterSpacing = value;

		ResetRichTextSize();

		EmitSignal(SignalName.OnAnySizeChange);
	}

	public void ResetRichTextSize() {
		var oldCurFontSize = curFontSize;

		curFontSize = (int) (baseFontSize * userScale * systemScale);
		richText.AddThemeFontSizeOverride("normal_font_size", curFontSize);

		var curLineSpacing = baseLineSpacing + lineSpacingDelta;
		richText.AddThemeConstantOverride("line_separation", curLineSpacing);

		fontVar.SpacingGlyph = characterSpacing;
		richText.AddThemeFontOverride("normal_font", fontVar);

		richText.Size = new Vector2(baseWidth * boundsMul, 0);

		if (oldCurFontSize != curFontSize) {
			SetText(unprocessedText);
		}

		SaveManager.instance.OnNeedSaveAction();	

		UpdateSize();
	}

	public override void Trash() {
		container.OnTextRemoved(this);
		
		base.Trash();
	}
}
