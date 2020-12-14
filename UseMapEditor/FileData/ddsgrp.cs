using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseMapEditor.FileData
{
    public class ddsgrp
    {
//        Header:
// u32 filesize
// u16 frame count
// u16 unknown(file version?) -- value appears to always be 0x1001 in the files I've seen.

//which is immediately followed by a series of File Entries

//File Entry:
// u32 unk -- always zero?
// u16 width
// u16 height
// u32 size
// u8[size] DDS file
    }
}
