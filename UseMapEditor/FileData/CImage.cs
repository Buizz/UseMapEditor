using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Data.Map.MapData;

namespace UseMapEditor.FileData
{
    public class CImage
    {
        public Vector2 screen;

        public List<CImage> Parent;
        public int imageID;

        public int sortvalue;

        public ulong drawsort
        {
            get
            {
                ulong rv = (ulong)((Level << 56) + ((int)screen.Y << 16) + sortvalue);

                return rv;
            }
        }


        public int Level;
        public ImageDrawType drawType;
        public enum ImageDrawType
        {
            Normal,
            Clock,
            Shadow,
            Hallaction,
            PureSprite,
            UnitSprite,
            Doodad
        }

        public bool IsSelect;
        public bool IsHover;


        public bool IsUnitRect;
        public int Left;
        public int Up;
        public int Right;
        public int Down;





        public int Direction
        {
            get
            {
                if (!Turnable)
                {
                    return 0;
                }

                if (IsLeft)
                {
                    return 32 - turnFrame;
                }
                else
                {
                    return turnFrame;
                }
            }
            set
            {
                int rv = value;
                if(rv >= 32)
                {
                    rv %= 32;
                }
                else if(rv < 0)
                {
                    rv = 32 + rv;
                }


                if (rv > 16)
                {
                    turnFrame = 32 - rv;
                    IsLeft = true;
                }
                else
                {
                    turnFrame = rv;
                    IsLeft = false;
                }
                if(turnFrame < 0)
                {
                    turnFrame = 0;
                }

            }
        }
        public sbyte XOffset;
        public sbyte YOffset;


        public int Frame;
        public int turnFrame;
        public bool Turnable;



        private bool ForceLeft = false;
        private bool _isleft;
        public bool IsLeft
        {
            get
            {
                if (Turnable)
                {
                    return _isleft;
                }
                else
                {
                    if (!ForceLeft)
                    {
                        return false;
                    }
                    else
                    {
                        return ForceLeft;
                    }

                }
            }
            set
            {
                _isleft = value;
            }
        }




        public int startAngle;
        public int StartAnim;

        public int color;

        public CImage parentImage;


        public CImage(int _sortvalue, List<CImage> _Parent, int imagenum, int _startAngle, int _color, ImageDrawType _drawType = ImageDrawType.Normal, int _StartAnim = 0, CImage _parentImage = null, int level = 0, int x = 0, int y = 0, int flag = -1)
        {
            sortvalue = _sortvalue;
            Parent = _Parent;
            imageID = imagenum;
            color = _color;
            XOffset = (sbyte)x;
            YOffset = (sbyte)y;
            Level = level;
            drawType = _drawType;
            StartAnim = _StartAnim;
            startAngle = _startAngle;

            parentImage = _parentImage;

            Init();
        }


        public void Init()
        {
            int drawTypev = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.images, "Draw Function", imageID).Data;
            if (drawTypev == 10)
            {
                drawType = CImage.ImageDrawType.Shadow;
            }


            if (startAngle != -1)
            {
                Direction = startAngle;
            }
            else
            {
                Direction = Global.WindowTool.random.Next(0, 33);
            }

            int turn = (int)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.images, "Gfx Turns", imageID).Data;

            Turnable = (turn == 1);


            iscriptID = (int)Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.images, "Iscript ID", imageID).Data;


            iscriptOffset = Global.WindowTool.iscript.iscriptEntries[iscriptID].AnimHeader[0];
        }





        public int iscriptID;
        public int iscriptOffset;

        public TimeSpan WaitTimer;
        public DateTime LastTime;

        public int Isint;
        Iscript iscript = Global.WindowTool.iscript;
        private uint unitclass;
        private List<CImage> images;
        private int dir;
        private byte player;

        public void PlayScript()
        {
            if (WaitTimer != null)
            {
                if(DateTime.Now < LastTime.Add(WaitTimer))
                {
                    return;
                }
            }



            byte opcode = iscript.ReadByte(ref iscriptOffset);
            List<uint> values = new List<uint>();
            if(opcode >= 69)
            {
                return;
            }
            
            if((opcode == 0x19) & (opcode == 0x1C))
            {
                int valuecount = iscript.ReadByte(ref iscriptOffset);

                for (int i = 0; i < valuecount; i++)
                {
                    values.Add(iscript.ReadUint16(ref iscriptOffset));
                }
            }
            else
            {
                for (int i = 0; i < iscript.Opcodedata[opcode].parmsize.Length; i++)
                {
                    switch (iscript.Opcodedata[opcode].parmsize[i])
                    {
                        case 8:
                            values.Add(iscript.ReadByte(ref iscriptOffset));
                            break;
                        case 16:
                            values.Add(iscript.ReadUint16(ref iscriptOffset));
                            break;
                        case 32:
                            values.Add(iscript.ReadUint32(ref iscriptOffset));
                            break;
                    }
                }
            }
            int t;
            int t2;
            Random random = Global.WindowTool.random;


            switch (opcode)
            {
                case 0x00:
                    //playfram          0x00 - u16<frame#> - displays a particular frame, adjusted for direction.
                    Frame = (int)values[0];
                    break;
                case 0x01:
                    //playframtile      0x01 - u16<frame#> - displays a particular frame dependent on tileset.
                    
                    break;
                case 0x02:
                    //sethorpos         0x02 - u8<x> - sets the current horizontal offset of the current image overlay.
                    XOffset = (sbyte)values[0];
                    break;
                case 0x03:
                    //setvertpos        0x03 - u8<y> - sets the vertical position of an image overlay.
                    YOffset = (sbyte)values[0];
                    break;

                case 0x04:
                    //setpos            0x04 - u8<x> u8<y> - sets the current horizontal and vertical position of the current image overlay.
                    XOffset = (sbyte)values[0];
                    YOffset = (sbyte)values[1];
                    break;
                case 0x05:
                    //wait              0x05 - u8<#ticks> - pauses script execution for a specific number of ticks.
                    WaitTimer = TimeSpan.FromMilliseconds(values[0] * 24);
                    LastTime = DateTime.Now;
                    break;
                case 0x06:
                    //waitrand          0x06 - u8<#ticks1> u8<#ticks2> - pauses script execution for a random number of ticks given two possible wait times. 
                    WaitTimer = TimeSpan.FromMilliseconds(random.Next((int)values[0], (int)values[1]) * 24);
                    LastTime = DateTime.Now;
                    break;
                case 0x07:
                    //goto              0x07 - u16<labelname> - unconditionally jumps to a specific code block.
                    iscriptOffset = (int)values[0];
                    break;
                case 0x08:
                    //imgol             0x08 - u16<image#> u8<x> u8<y> - displays an active image overlay at an animation level higher than the current image overlay at a specified offset position.
                    if (imageID == 271 & values[0] == 273)
                    {
                        Parent.Add(new CImage(sortvalue, Parent, (int)values[0], Direction, color, _drawType: drawType, _StartAnim: StartAnim, x: (int)values[1], y: (int)values[2], level: Level + 2));
                    }
                    else
                    {
                        Parent.Add(new CImage(sortvalue, Parent, (int)values[0], Direction, color, _drawType: drawType, x: (int)values[1], y: (int)values[2], level: Level + 2));
                    }
                    break;
                case 0x09:
                    //imgul             0x09 - u16<image#> u8<x> u8<y> - displays an active image overlay at an animation level lower than the current image overlay at a specified offset position.

                    Parent.Add(new CImage(sortvalue, Parent, (int)values[0], Direction, color, _drawType: drawType, x: (int)values[1], y: (int)values[2], level: Level - 2, _parentImage: this, flag: opcode));

                    break;
                case 0x0a:
                    //imgolorig         0x0a - u16<image#> - displays an active image overlay at an animation level higher than the current image overlay at the relative origin offset position.
                    Parent.Add(new CImage(sortvalue, Parent, (int)values[0], Direction, color, _drawType: drawType, x: XOffset, y: YOffset, level: Level + 2));

                    break;
                case 0x0b:
                    //switchul          0x0b - <image#> - only for powerups. Hypothesised to replace the image overlay that was first created by the current image overlay.
                    
                    break;
                case 0x0c:
                    //__0c              0x0c - no parameters - unknown.
                    
                    break;
                case 0x0d:
                    //imgoluselo        0x0d - <image#> <x> <y> - displays an active image overlay at an animation level higher than the current image overlay, using a LO* file to determine the offset position.
                    
                    break;
                case 0x0e:
                    //imguluselo        0x0e - <image#> <x> <y> - displays an active image overlay at an animation level lower than the current image overlay, using a LO* file to determine the offset position.
                    
                    break;
                case 0x0f:
                    //sprol             0x0f - <sprite#> <x> <y> - spawns a sprite one animation level above the current image overlay at a specific offset position.
                    
                    break;
                case 0x10:
                    //highsprol         0x10 - <sprite#> <x> <y> - spawns a sprite at the highest animation level at a specific offset position.
                    
                    break;
                case 0x11:
                    //lowsprul          0x11 - <sprite#> <x> <y> - spawns a sprite at the lowest animation level at a specific offset position.
                    
                    break;
                case 0x12:
                    //uflunstable       0x12 - <flingy# - creates an flingy with restrictions; supposedly crashes in most cases.
                    
                    break;
                case 0x13:
                    //spruluselo        0x13 - <sprite#> <x> <y> - spawns a sprite one animation level below the current image overlay at a specific offset position. The new sprite inherits the direction of the current sprite. Requires LO* file for unknown reason.
                    
                    break;
                case 0x14:
                    //sprul             0x14 - <sprite#> <x> <y> - spawns a sprite one animation level below the current image overlay at a specific offset position. The new sprite inherits the direction of the current sprite.
                    
                    break;
                case 0x15:
                    //sproluselo        0x15 - <sprite#> <overlay#> - spawns a sprite one animation level above the current image overlay, using a specified LO* file for the offset position information. The new sprite inherits the direction of the current sprite.
                    
                    break;
                case 0x16:
                    //end               0x16 - no parameters - destroys the current active image overlay, also removing the current sprite if the image overlay is the last in one in the current sprite.
                    Parent.Remove(this);
                    break;
                case 0x17:
                    //setflipstate      0x17 - <flipstate> - sets flip state of the current image overlay.
                    ForceLeft = (values[0] == 1);
                    break;
                case 0x18:
                    //playsnd           0x18 - <sound#> - plays a sound.
                    
                    break;
                case 0x19:
                    //playsndrand       0x19 - <#sounds> <sound#> <...> - plays a random sound from a list.
                    
                    break;
                case 0x1a:
                    //playsndbtwn       0x1a - <firstsound#> <lastsound#> - plays a random sound between two inclusive sfxdata.dat entry IDs.
                    
                    break;
                case 0x1b:
                    //domissiledmg      0x1b - no parameters - causes the damage of a weapon flingy to be applied according to its weapons.dat entry.
                    
                    break;
                case 0x1c:
                    //attackmelee       0x1c - <#sounds> <sound#> <...> - applies damage to target without creating a flingy and plays a sound.
                    
                    break;
                case 0x1d:
                    //followmaingraphic 0x1d - no parameters - causes the current image overlay to display the same frame as the parent image overlay.
                    if (parentImage != null)
                    {
                        Frame = parentImage.Frame;
                        Direction = parentImage.Direction;
                    }
                    break;
                case 0x1e:
                    //randcondjmp       0x1e - <randchance#> <labelname> - random jump, chance of performing jump depends on the parameter.
                    t = random.Next(0, 255);

                    if (t <= values[0])
                    {
                        iscriptOffset = (int)values[1];
                    }
                    break;
                case 0x1f:
                    //turnccwise        0x1f - <turnamount> - turns the flingy counterclockwise by a particular amount.
                    t = (int)(Direction - values[0]);
                    if (t < 0)
                    {
                        Direction = 32 + t;
                    }
                    else
                    {
                        Direction = t;
                    }
                    break;
                case 0x20:
                    //turncwise         0x20 - <turnamount> - turns the flingy clockwise by a particular amount.
                    Direction -= (int)values[0];
                    break;
                case 0x21:
                    //turn1cwise        0x21 - no parameters - turns the flingy clockwise by one direction unit.
                    Direction += 1;
                    break;
                case 0x22:
                    //turnrand          0x22 - <turnamount> - turns the flingy a specified amount in a random direction, with a heavy bias towards turning clockwise.
                    t = random.Next(0, 255);
                    t = t % 2;

                    if (t == 0)
                    {
                        Direction += (int)values[0];
                    }
                    else
                    {
                        Direction -= (int)values[0];
                    }
                    break;
                case 0x23:
                    //setspawnframe     0x23 - <direction> - in specific situations, performs a natural rotation to the given direction.
                    
                    break;
                case 0x24:
                    //sigorder          0x24 - <signal#> - allows the current unit's order to proceed if it has paused for an animation to be completed.
                    
                    break;
                case 0x25:
                    //attackwith        0x25 - <ground = 1, air = 2> - attack with either the ground or air weapon depending on a parameter.
                    
                    break;
                case 0x26:
                    //attack            0x26 - no parameters - attack with either the ground or air weapon depending on target.
                    
                    break;
                case 0x27:
                    //castspell         0x27 - no parameters - identifies when a spell should be cast in a spellcasting animation. The spell is determined by the unit's current order.
                    
                    break;
                case 0x28:
                    //useweapon         0x28 - <weapon#> - makes the unit use a specific weapons.dat ID on its target.
                    
                    break;
                case 0x29:
                    //move              0x29 - <movedistance> - sets the unit to move forward a certain number of pixels at the end of the current tick.
                    
                    break;
                case 0x2a:
                    //gotorepeatattk    0x2a - no parameters - signals to StarCraft that after this point, when the unit's cooldown time is over, the repeat attack animation can be called.
                    
                    break;
                case 0x2b:
                    //engframe          0x2b - <frame#> - plays a particular frame, often used in engine glow animations.
                    
                    break;
                case 0x2c:
                    //engset            0x2c - <frameset#> - plays a particular frame set, often used in engine glow animations.
                    
                    break;
                case 0x2d:
                    //__2d              0x2d - no parameters - hypothesised to hide the current image overlay until the next animation.
                    
                    break;
                case 0x2e:
                    //nobrkcodestart    0x2e - no parameters - holds the processing of player orders until a nobrkcodeend is encountered.
                    
                    break;
                case 0x2f:
                    //nobrkcodeend      0x2f - no parameters - allows the processing of player orders after a nobrkcodestart instruction.
                    
                    break;
                case 0x30:
                    //ignorerest        0x30 - no parameters - conceptually, this causes the script to stop until the next animation is called.
                    
                    break;
                case 0x31:
                    //attkshiftproj     0x31 - <distance> - creates the weapon flingy at a particular distance in front of the unit.
                    
                    break;
                case 0x32:
                    //tmprmgraphicstart 0x32 - no parameters - sets the current image overlay state to hidden.
                    
                    break;
                case 0x33:
                    //tmprmgraphicend   0x33 - no parameters - sets the current image overlay state to visible.
                    
                    break;
                case 0x34:
                    //setfldirect       0x34 - <direction> - sets the current direction of the flingy.
                    
                    break;
                case 0x35:
                    //call              0x35 - <labelname> - calls a code block.
                    
                    break;
                case 0x36:
                    //return            0x36 - no parameters - returns from call.
                    iscriptOffset = Global.WindowTool.iscript.iscriptEntries[iscriptID].AnimHeader[0];
                    break;
                case 0x37:
                    //setflspeed        0x37 - <speed> - sets the flingy.dat speed of the current flingy.
                    
                    break;
                case 0x38:
                    //creategasoverlays 0x38 - <gasoverlay#> - creates gas image overlays at offsets specified by LO* files.
                    
                    break;
                case 0x39:
                    //pwrupcondjmp      0x39 - <labelname> - jumps to a code block if the current unit is a powerup and it is currently picked up.
                    
                    break;
                case 0x3a:
                    //trgtrangecondjmp  0x3a - <distance> <labelname> - jumps to a block depending on the distance to the target.
                    
                    break;
                case 0x3b:
                    //trgtarccondjmp    0x3b - <angle1> <angle2> <labelname> - jumps to a block depending on the current angle of the target.
                    
                    break;
                case 0x3c:
                    //curdirectcondjmp  0x3c - <angle1> <angle2> <labelname> - only for units. Jump to a code block if the current sprite is facing a particular direction.
                    
                    break;
                case 0x3d:
                    //imgulnextid       0x3d - <x> <y> - displays an active image overlay at the shadow animation level at a specified offset position. The image overlay that will be displayed is the one that is after the current image overlay in images.dat.
                    Parent.Add(new CImage(sortvalue ,Parent, imageID + 1, Direction, color, x: (int)values[0], y: (int)values[1], level: Level - 1, _parentImage: this, flag: opcode));

                    break;
                case 0x3e:
                    //__3e              0x3e - no parameters - unknown.
                    
                    break;
                case 0x3f:
                    //liftoffcondjmp    0x3f - <labelname> - jumps to a code block when the current unit that is a building that is lifted off.
                    
                    break;
                case 0x40:
                    //warpoverlay       0x40 - <frame#> - hypothesised to display the current image overlay's frame clipped to the outline of the parent image overlay.
                    
                    break;
                case 0x41:
                    //orderdone         0x41 - <signal#> - most likely used with orders that continually repeat, like the Medic's healing and the Valkyrie's afterburners (which no longer exist), to clear the sigorder flag to stop the order.
                    
                    break;
                case 0x42:
                    //grdsprol          0x42 - <sprite#> <x> <y> - spawns a sprite one animation level above the current image overlay at a specific offset position, but only if the current sprite is over ground-passable terrain.
                    
                    break;
                case 0x43:
                    //__43              0x43 - no parameters - unknown.
                    
                    break;
                case 0x44:
                    //dogrddamage       0x44 - no parameters - applies damage like domissiledmg when on ground-unit-passable terrain.
                    
                    break;
            }


            if (Isint == 0)
            {
                switch (imageID)
                {
                    case 61:
                    case 64:
                    case 66:
                    case 68:
                    case 70:
                    case 72:
                    case 74:
                    case 76:
                    case 78:
                    case 80:
                    case 82:
                    case 84:
                    case 86:
                    case 88:
                    case 90:
                    case 91:
                    case 93:
                    case 95:
                    case 97:
                    case 99:
                    case 278:
                    case 296:
                    case 307:
                    case 923:
                    case 934:
                        iscriptOffset = getiscriptOffset(iscriptID, 16);
                        Isint = 1;
                        break;
                    case 271:
                        if (iscriptOffset == 31162)
                        {
                            iscriptOffset = getiscriptOffset(iscriptID, 16);
                            Isint = 1;
                        }
                            
                        break;
                }
            }
            else if (Isint == 1)
            {
                switch (imageID)
                {
                    case 271:
                        if(iscriptOffset == 31162)
                        {
                            iscriptOffset = getiscriptOffset(iscriptID, StartAnim);
                            Isint = 2;
                        }
                        //iscriptOffset = Global.WindowTool.iscript.iscriptEntries[iscriptID].AnimHeader[StartAnim];
                        //Isint = 2;
                        break;
                }
            }
            if (Isint == 0)
            {
                if ((imageID == 271 & iscriptOffset == 31162) | imageID != 271)
                {
                    if (StartAnim != 0)
                    {
                        iscriptOffset = getiscriptOffset(iscriptID, StartAnim);
                        Isint = 1;
                    }
                }

            }
        }


        private int getiscriptOffset(int iscriptID, int StartAnim)
        {
            ushort[] header = Global.WindowTool.iscript.iscriptEntries[iscriptID].AnimHeader;
            if (header.Length > StartAnim)
            {
                return header[StartAnim];
            }
            else
            {
                return header[0];
            }

        }
    }
}
