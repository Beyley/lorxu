using System;
using System.Drawing;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Helpers;
using Silk.NET.Input;
using Color = Furball.Vixie.Backends.Shared.Color;

namespace lorxu.Screens.Editor; 

public class CharacterEditor : CompositeDrawable {
	private readonly FontFamily _family;
	private readonly Font       _font;

	public Bindable<Character> CurrentCharacter;

	public Bindable<float> PixelSize = new(20f);
	
	public static readonly Color            LineColor = Color.LightGray;
	private readonly       ColourPicker     _colourPicker;
	private readonly       UiButtonDrawable _sizeRight;
	private readonly       UiButtonDrawable _sizeLeft;
	private readonly       UiButtonDrawable _sizeUp;
	private readonly       UiButtonDrawable _sizeDown;

	public CharacterEditor(FontFamily font, FontType type, int codepoint) {
		this._family = font;
		this._font   = font.Fonts[type];

		this.SelectCodepoint(codepoint);

		this._colourPicker = new() {
			OriginType = OriginType.TopCenter
		};

		this._sizeRight = new UiButtonDrawable(Vector2.Zero, "Width+", FurballGame.DEFAULT_FONT, 25, Color.Blue, Color.White, Color.Black, Vector2.Zero);
		this._sizeLeft = new UiButtonDrawable(Vector2.Zero, "Width-", FurballGame.DEFAULT_FONT, 25, Color.Blue, Color.White, Color.Black, Vector2.Zero);
		this._sizeUp = new UiButtonDrawable(Vector2.Zero, "Height-", FurballGame.DEFAULT_FONT, 25, Color.Blue, Color.White, Color.Black, Vector2.Zero);
		this._sizeDown = new UiButtonDrawable(Vector2.Zero, "Height+", FurballGame.DEFAULT_FONT, 25, Color.Blue, Color.White, Color.Black, Vector2.Zero);
		
		this._sizeRight.OnClick += delegate {
			this.CurrentCharacter.Value.Resize(this.CurrentCharacter.Value.Width + 1, this.CurrentCharacter.Value.Height);
		};
		
		this._sizeLeft.OnClick += delegate {
			this.CurrentCharacter.Value.Resize(this.CurrentCharacter.Value.Width - 1, this.CurrentCharacter.Value.Height);
		};
		
		this._sizeUp.OnClick += delegate {
			this.CurrentCharacter.Value.Resize(this.CurrentCharacter.Value.Width, this.CurrentCharacter.Value.Height - 1);
		};
		
		this._sizeDown.OnClick += delegate {
			this.CurrentCharacter.Value.Resize(this.CurrentCharacter.Value.Width, this.CurrentCharacter.Value.Height + 1);
		};
		
		this.Resize(this, this.CurrentCharacter.Value.Size);
		
		this.CurrentCharacter.Value.OnBitmapResize += this.Resize;
		this.CurrentCharacter.Value.OnBitmapChange += this.Redraw;
		
	}
		
	private void Redraw(object _, EventArgs eventArgs) {
		this.Drawables.RemoveAll(x => x.Tags.Count > 0 && x.Tags[0] == "ispixel");
		
		TexturedDrawable square;
		for (int x = 0; x < this.CurrentCharacter.Value.Width; x++) {
			for (int y = 0; y < this.CurrentCharacter.Value.Height; y++) {
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

		this.Drawables.Add(this._sizeLeft);
		this.Drawables.Add(this._sizeRight);
		this.Drawables.Add(this._sizeUp);
		this.Drawables.Add(this._sizeDown);

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

		this._sizeLeft.Position  = new(this.CurrentCharacter.Value.Width * this.PixelSize + 10, 0);
		this._sizeRight.Position = new(this.CurrentCharacter.Value.Width * this.PixelSize + 10, this._sizeLeft.Position.Y  + this._sizeLeft.Size.Y);
		this._sizeUp.Position    = new(this.CurrentCharacter.Value.Width * this.PixelSize + 10, this._sizeRight.Position.Y + this._sizeRight.Size.Y);
		this._sizeDown.Position  = new(this.CurrentCharacter.Value.Width * this.PixelSize + 10, this._sizeUp.Position.Y  + this._sizeUp.Size.Y);
		
		this._colourPicker.Position = new(this.PixelSize * this.CurrentCharacter.Value.Width / 2f, this.PixelSize * this.CurrentCharacter.Value.Height + 10);

		this.Redraw(this, null);
	}

	public void SelectCodepoint(int codepoint) {
		if(!this._font.Characters.ContainsKey(codepoint))
			this._font.Characters.Add(codepoint, new Character(this._family));
			
		this.CurrentCharacter = new(this._font.Characters[codepoint]);
	}
}