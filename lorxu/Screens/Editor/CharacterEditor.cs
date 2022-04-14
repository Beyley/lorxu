using System;
using System.Drawing;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Engine.Engine.Helpers;
using Gtk;
using Silk.NET.Input;
using Color = Furball.Vixie.Backends.Shared.Color;

namespace lorxu.Screens.Editor; 

public class CharacterEditor : CompositeDrawable {
	private readonly FontFamily _family;
	private readonly Font       _font;

	public Bindable<Character> CurrentCharacter;

	public Bindable<float> PixelSize = new(20f);
	
	public static readonly Color        LineColor = Color.LightGray;
	private readonly       ColourPicker _colourPicker;

	public CharacterEditor(FontFamily font, FontType type, int codepoint) {
		this._family = font;
		this._font   = font.Fonts[type];

		this.SelectCodepoint(codepoint);

		this._colourPicker = new() {
			OriginType = OriginType.TopCenter
		};

		this.Resize(this, this.CurrentCharacter.Value.Size);
		
		this.CurrentCharacter.Value.OnBitmapResize += this.Resize;
		this.CurrentCharacter.Value.OnBitmapChange += this.Redraw;
		
	}
		
	private void Redraw(object _, EventArgs eventArgs) {
		this.Drawables.RemoveAll(x => x.Tags.Count > 0 && x.Tags[0] == "ispixel");
		
		TexturedDrawable square;
		for (int x = 0; x < this.CurrentCharacter.Value.Width; x++) {
			for (int y = 0; y < this.CurrentCharacter.Value.Width; y++) {
				System.Drawing.Color pxColor = this.CurrentCharacter.Value.Bitmap.GetPixel(x, y);
				Color                color   = new(pxColor.R, pxColor.G, pxColor.B, pxColor.A);

				square = new TexturedDrawable(FurballGame.WhitePixel, new Vector2(x, y) * this.PixelSize) {
					Scale         = new Vector2(this.PixelSize.Value),
					Depth         = 0.5f,
					ColorOverride = color,
					Tags = {
						"ispixel",
						x.ToString(), 
						y.ToString()
					}
				};
				
				square.OnClick += delegate(object? sender, (MouseButton button, Point pos) e) {
					TexturedDrawable square2 = (TexturedDrawable)sender;

					int x = int.Parse(square2.Tags[1]);
					int y = int.Parse(square2.Tags[2]);

					if(e.button == MouseButton.Left)
						this.CurrentCharacter.Value.SetPixel(x, y, this._colourPicker.SelectedColour.Value);
					else
						this.CurrentCharacter.Value.SetPixel(x, y, Color.Transparent);
				};
				
				this.Drawables.Add(square);
			}
		}

		this._sortDrawables = true;
	}
		
	private void Resize(object _, Size size) {
		this.Drawables.Clear();
		
		this.Drawables.Add(this._colourPicker);

		LinePrimitiveDrawable line;
		for (int x = 0; x <= this.CurrentCharacter.Value.Width; x++) {
			line = new LinePrimitiveDrawable(new(x * this.PixelSize, 0), new(x * this.PixelSize, this.CurrentCharacter.Value.Height * this.PixelSize), LineColor, 1f, false) {
				Depth       = 1.5f,
				Clickable   = false,
				CoverClicks = false
			};
			this.Drawables.Add(line);
		}
		for (int y = 0; y <= this.CurrentCharacter.Value.Height; y++) {
			line = new LinePrimitiveDrawable(new(0, y * this.PixelSize), new(this.CurrentCharacter.Value.Width * this.PixelSize, y * this.PixelSize), LineColor, 1f, false) {
				Depth       = 1.5f,
				Clickable   = false,
				CoverClicks = false
			};
			this.Drawables.Add(line);
		}

		this._colourPicker.Position = new(this.PixelSize * this.CurrentCharacter.Value.Width / 2f, this.PixelSize * this.CurrentCharacter.Value.Height + 10);

		this.Redraw(this, null);
	}

	public void SelectCodepoint(int codepoint) {
		if(!this._font.Characters.ContainsKey(codepoint))
			this._font.Characters.Add(codepoint, new Character(this._family));
			
		this.CurrentCharacter = new(this._font.Characters[codepoint]);
	}
}