using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseMapEditor.MonoGameControl
{


    public class Dxt1
    {
        public static Color DecompressBlock(int x, int y, int width, byte[] blob)
        {
            Color[] pixels = new Color[16];

            int off = (y * width / 4 + x) * (64 / 8) + 0x80;

            UInt16 c0, c1;
            int c0_r, c0_g, c0_b;
            int c1_r, c1_g, c1_b;
            int c2_r, c2_g, c2_b;
            int c3_r, c3_g, c3_b;
            UInt16 c0_lo = blob[off + 0];
            UInt16 c0_hi = blob[off + 1];
            UInt16 c1_lo = blob[off + 2];
            UInt16 c1_hi = blob[off + 3];
            c0 = (UInt16)((c0_hi << 8) | c0_lo);
            c1 = (UInt16)((c1_hi << 8) | c1_lo);

            c0_r = (c0 & 0b1111100000000000) >> 11;
            c0_g = (c0 & 0b0000011111100000) >> 5;
            c0_b = c0 & 0b0000000000011111;
            c1_r = (c1 & 0b1111100000000000) >> 11;
            c1_g = (c1 & 0b0000011111100000) >> 5;
            c1_b = c1 & 0b0000000000011111;

            if (c0 >= c1)
            {
                c2_r = (int)((2.0f * c0_r + c1_r) / 3.0f);
                c2_g = (int)((2.0f * c0_g + c1_g) / 3.0f);
                c2_b = (int)((2.0f * c0_b + c1_b) / 3.0f);
                c3_r = (int)((c0_r + 2.0f * c1_r) / 3.0f);
                c3_g = (int)((c0_g + 2.0f * c1_g) / 3.0f);
                c3_b = (int)((c0_b + 2.0f * c1_b) / 3.0f);
            }
            else
            {
                c2_r = (int)((c0_r + c1_r) / 2.0f);
                c2_g = (int)((c0_g + c1_g) / 2.0f);
                c2_b = (int)((c0_b + c1_b) / 2.0f);
                c3_r = c3_g = c3_b = 0;
            }

            Color[] lookup = new Color[]
            {
                new Color(c0_r / 32.0f, c0_g / 64.0f, c0_b / 32.0f),
                new Color(c1_r / 32.0f, c1_g / 64.0f, c1_b / 32.0f),
                new Color(c2_r / 32.0f, c2_g / 64.0f, c2_b / 32.0f),
                new Color(c3_r / 32.0f, c3_g / 64.0f, c3_b / 32.0f),
            };



            for (int by = 0; by < 4; by++)
            {
                for (int bx = 0; bx < 4; bx++)
                {
                    var code = (blob[off + by] << (bx * 2)) & 3;
                    pixels[by * 4 + bx] = lookup[code];
                }
            }

            int r = 0;
            int g = 0;
            int b = 0;
            for (int i = 0; i < 16; i++)
            {
                r += pixels[i].R;
                g += pixels[i].G;
                b += pixels[i].B;
            }




            return new Color(r/16,g/16,b/16, 0xFF);
        }

    }
}
