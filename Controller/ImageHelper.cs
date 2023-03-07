using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

public static class ImageHelper
{
    /*public static byte[] ImageToByteArray(Image imagem)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            imagem.Save(stream, imagem.RawFormat);
            return stream.ToArray();
        }
    }*/


    public static Image CreateCircularImage(Image imagem)
    {
        Bitmap bmp = new Bitmap(imagem.Width, imagem.Height);
        Graphics gfx = Graphics.FromImage(bmp);
        gfx.SmoothingMode = SmoothingMode.AntiAlias;
        gfx.Clear(Color.Transparent);
        gfx.DrawEllipse(new Pen(Color.Black), 0, 0, bmp.Width - 1, bmp.Height - 1);
        GraphicsPath path = new GraphicsPath();
        path.AddEllipse(0, 0, bmp.Width, bmp.Height);
        Region reg = new Region(path);
        gfx.SetClip(reg, CombineMode.Replace);
        gfx.DrawImage(imagem, 0, 0);
        return bmp;
    }
    public static Image ByteArrayToImage(byte[] byteArrayIn)
    {
        MemoryStream ms = new MemoryStream(byteArrayIn);
        Image returnImage = Image.FromStream(ms);
        return returnImage;
    }

    public static byte[] ImageToByteArray(Image image)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }
    }

}
