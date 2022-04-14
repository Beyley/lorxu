using System;
using System.Drawing;
using System.Drawing.Imaging;
using Color = Furball.Vixie.Backends.Shared.Color;

namespace lorxu; 

public class Character {
	public Bitmap  Bitmap;
	public Padding Padding;
		
	public int Width  => this.Bitmap.Width;
	public int Height => this.Bitmap.Height;

	public Size Size => this.Bitmap.Size;

	public event EventHandler       OnBitmapChange;
	public event EventHandler<Size> OnBitmapResize;
		
	public Character(FontFamily family) {
		this.Bitmap = new(family.PxSize, family.PxSize, PixelFormat.Format32bppArgb);
	}
	
	public void SetPixel(int x, int y, Color color) {
		this.Bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B));
		
		this.OnBitmapChange?.Invoke(this, EventArgs.Empty);
	}

	public void Resize(int width, int height) {
		this.Bitmap = new Bitmap(this.Bitmap, width, height);
		
		this.OnBitmapResize?.Invoke(this, new Size(width, height));
	}
}