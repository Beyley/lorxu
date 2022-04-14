using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Helpers;
using Silk.NET.Input;
using Color = Furball.Vixie.Backends.Shared.Color;

namespace lorxu.Screens.Editor; 

public class ColourPicker : CompositeDrawable {
	public static readonly List<Color> COLOURS = new() {
		Color.White,
		Color.LightGray,
		Color.Gray,
		new Color(Color.Gray.R - 30, Color.Gray.G - 30, Color.Gray.B - 30),
		Color.Black
	};

	public const float SIZE = 25f;

	public Bindable<Color> SelectedColour = new(Color.White);

	private readonly RectanglePrimitiveDrawable _selectionRect;
	
	public ColourPicker() {
		this.Clickable   = false;
		this.CoverClicks = false;
		
		this._selectionRect = new RectanglePrimitiveDrawable(Vector2.Zero, new(SIZE), 1, false) {
			ColorOverride = Color.Red,
			Visible       = false,
			Clickable     = false,
			CoverClicks   = false
		};
		
		TexturedDrawable tex;
		for (int i = 0; i < COLOURS.Count; i++) {
			Color colour = COLOURS[i];

			tex = new TexturedDrawable(FurballGame.WhitePixel, new(i * SIZE, 0)) {
				Scale         = new(SIZE),
				ColorOverride = colour,
				Tags = {
					i.ToString()
				}
			};
			
			tex.OnClick += delegate(object sender, (MouseButton button, Point pos) _) {
				TexturedDrawable tex = (TexturedDrawable)sender;
				
				this.SetSelection(int.Parse(tex.Tags[0]));
			};
			
			this.Drawables.Add(tex);
		}

		this.SetSelection(0);
		
		this.Drawables.Add(this._selectionRect);
	}

	public void SetSelection(int index) {
		this.SelectedColour.Value = COLOURS[index];

		this._selectionRect.Visible = true;
		this._selectionRect.MoveTo(new(index * SIZE, 0f), 150);
	}
}
