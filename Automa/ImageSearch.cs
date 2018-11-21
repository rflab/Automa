using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace Automa
{
    class ImageSearch
    {
        /// <summary>
        /// webで拾った部分画像検索を微妙にあいまいにしたもの
        /// </summary>
        /// <param name="src"></param>
        /// <param name="ptn"></param>
        /// <param name="err_threshold_rate"></param>
        /// <returns></returns>
        static public Point Search(Bitmap src, Bitmap ptn, double err_threshold_rate = 0.05)
        {
            BitmapData srcData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData ptnData = ptn.LockBits(new Rectangle(0, 0, ptn.Width, ptn.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] srcPix, ptnPix, srcLine, ptnLine;
            srcPix = new byte[src.Width * src.Height * 4];
            ptnPix = new byte[ptn.Width * ptn.Height * 4];
            srcLine = new byte[ptn.Width * 4];
            ptnLine = new byte[ptn.Width * 4];
            Marshal.Copy(srcData.Scan0, srcPix, 0, srcPix.Length);
            Marshal.Copy(ptnData.Scan0, ptnPix, 0, ptnPix.Length);
            Point agreePoint = Point.Empty;
            bool agree = true;
            
            int err_threshold = (int)(ptn.Width * 4 * err_threshold_rate);
            for (int y = 0; y < src.Height - ptn.Height; y++)
            {
                for (int x = 0; x < src.Width - ptn.Width; x++)
                {
                    for (int yy = 0; yy < ptn.Height; yy++)
                    {
                        agree = true;
                        System.Array.Copy(srcPix, (x + (yy + y) * src.Width) * 4, srcLine, 0, (srcLine.Length));
                        System.Array.Copy(ptnPix, yy * ptn.Width * 4, ptnLine, 0, (ptnLine.Length));
                        int err_count = 0;
                        for (int i=0; i<ptnLine.Length;i++)
                        {
                            if (srcLine[i] != ptnLine[i])
                                err_count++;
                        }

                        if (err_count > err_threshold) agree = false;
                        if (agree == false) break;
                    }
                    if (agree)
                    {
                        agreePoint = new Point(x, y);
                        break;
                    }
                }
                if (agree) break;
            }
            src.UnlockBits(srcData);
            ptn.UnlockBits(ptnData);
            return agreePoint;
        }

        /// <summary>
        /// webで拾った完全一致検索
        /// </summary>
        /// <param name="src"></param>
        /// <param name="ptn"></param>
        /// <returns></returns>
        static public Point Search_org(Bitmap src, Bitmap ptn)
        {
            BitmapData srcData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData ptnData = ptn.LockBits(new Rectangle(0, 0, ptn.Width, ptn.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] srcPix, ptnPix, srcLine, ptnLine;
            srcPix = new byte[src.Width * src.Height * 4];
            ptnPix = new byte[ptn.Width * ptn.Height * 4];
            srcLine = new byte[ptn.Width * 4];
            ptnLine = new byte[ptn.Width * 4];
            Marshal.Copy(srcData.Scan0, srcPix, 0, srcPix.Length);
            Marshal.Copy(ptnData.Scan0, ptnPix, 0, ptnPix.Length);
            Point agreePoint = Point.Empty;
            bool agree = true;

            for (int y = 0; y < src.Height - ptn.Height; y++)
            {
                for (int x = 0; x < src.Width - ptn.Width; x++)
                {
                    agree = true;
                    for (int yy = 0; yy < ptn.Height; yy++)
                    {
                        System.Array.Copy(srcPix, (x + (yy + y) * src.Width) * 4, srcLine, 0, (srcLine.Length));
                        System.Array.Copy(ptnPix, yy * ptn.Width * 4, ptnLine, 0, (ptnLine.Length));
                        if (srcLine.SequenceEqual(ptnLine) == false) agree = false;
                        if (agree == false) break;
                    }
                    if (agree)
                    {
                        agreePoint = new Point(x, y);
                        break;
                    }
                }
                if (agree) break;
            }
            src.UnlockBits(srcData);
            ptn.UnlockBits(ptnData);
            return agreePoint;
        }

        /// <summary>
        /// webで拾った画像検索
        /// </summary>
        /// <param name="src"></param>
        /// <param name="ptn"></param>
        /// <returns></returns>
        static public Point Search24_org(Bitmap src, Bitmap ptn)
        {
            BitmapData srcData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData ptnData = ptn.LockBits(new Rectangle(0, 0, ptn.Width, ptn.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            byte[] srcPix, ptnPix, srcLine, ptnLine;
            srcPix = new byte[src.Width * src.Height * 3];
            ptnPix = new byte[ptn.Width * ptn.Height * 3];
            srcLine = new byte[ptn.Width * 3];
            ptnLine = new byte[ptn.Width * 3];
            Marshal.Copy(srcData.Scan0, srcPix, 0, srcPix.Length);
            Marshal.Copy(ptnData.Scan0, ptnPix, 0, ptnPix.Length);
            Point agreePoint = Point.Empty;
            bool agree = true;

            for (int y = 0; y < src.Height - ptn.Height; y++)
            {
                for (int x = 0; x < src.Width - ptn.Width; x++)
                {
                    agree = true;
                    for (int yy = 0; yy < ptn.Height; yy++)
                    {
                        System.Array.Copy(srcPix, (x + (yy + y) * src.Width) * 3, srcLine, 0, (srcLine.Length));
                        System.Array.Copy(ptnPix, yy * ptn.Width * 3, ptnLine, 0, (ptnLine.Length));
                        if (srcLine.SequenceEqual(ptnLine) == false) agree = false;
                        if (agree == false) break;
                    }
                    if (agree)
                    {
                        agreePoint = new Point(x, y);
                        break;
                    }
                }
                if (agree) break;
            }
            src.UnlockBits(srcData);
            ptn.UnlockBits(ptnData);
            return agreePoint;
        }
    }
}