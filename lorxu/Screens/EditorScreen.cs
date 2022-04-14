using System;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using lorxu.Screens.Editor;

namespace lorxu.Screens; 

public class EditorScreen : Screen {
	private CharacterEditor   _characterEditor;
	private CodepointSelector _codepointSelector;

	private readonly FontFamily   _family;
	private readonly Font         _font;
	private readonly FontType     _type;

	public EditorScreen(FontFamily family, FontType type) {
		this._family = family;
		this._type   = type;

		this._font = family.Fonts[type];
	}
		
	public override void Initialize() {
		base.Initialize();

		TexturedDrawable background = new TexturedDrawable(FurballGame.WhitePixel, Vector2.Zero) {
			Scale = new(1280, 720),
			ColorOverride = new(40, 40, 40),
			Depth = 2f
		};
		this.Manager.Add(background);
		
		this._characterEditor = new CharacterEditor(this._family, this._type, 0x0041) {
			Position = new(10)
		};
		this.Manager.Add(this._characterEditor);
	}
}