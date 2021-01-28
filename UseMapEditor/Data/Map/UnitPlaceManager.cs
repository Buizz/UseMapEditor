using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.Control;
using UseMapEditor.FileData;
using UseMapEditor.Task;

namespace Data.Map
{
    public partial class MapData
    {


        public void UNITListRemove(CUNIT cUNIT)
        {
            switch (cUNIT.unitID)
            {
                case 106:
                //커맨드 센터
                case 113:
                //팩토리
                case 114:
                //스타포트
                case 116:
                //사이언스
                case 107:
                //컴셋
                case 108:
                //사일로
                case 115:
                //컨트롤타워
                case 117:
                //커벌트옵스
                case 118:
                //피직스랩
                case 120:
                //머신샵
                case 134:
                    //나이더스 카날
                    uint linkedunit = cUNIT.linkedUnit;
                    CUNIT _cUNIT = UNIT.SingleOrDefault((x) => x.unitclass == linkedunit);

                    if (_cUNIT != null)
                    {
                        _cUNIT.linkFlag = 0;
                        _cUNIT.linkedUnit = 0;
                        _cUNIT.ImageReset();
                    }

                    break;
            }


            UNIT.Remove(cUNIT);
            mapEditor.MinimapUnitRefresh();
            mapEditor.IndexedUnitCancel();
        }

        public void UNITListAdd(CUNIT cUNIT)
        {
            uint c = 1;

            if (UNIT.Count != 0)
            {
                c = UNIT.Last().unitclass + 1;
                CUNIT _cUNIT = UNIT.SingleOrDefault((x) => x.unitclass == c);
                while (_cUNIT != null)
                {
                    _cUNIT = UNIT.SingleOrDefault((x) => x.unitclass == ++c);
                }
            }



            cUNIT.unitclass = c;
            //링크 유닛 관리.
            switch (cUNIT.unitID)
            {
                case 106:
                    //커맨드 센터
                    {
                        CUNIT addon = UNIT.SingleOrDefault((x) => (x.X == cUNIT.X + 96) & (x.Y == cUNIT.Y + 16) & ((x.unitID == 107) | (x.unitID == 108)));
                        if (addon != null)
                        {
                            addon.linkFlag = 0b1 << 10;
                            cUNIT.linkFlag = 0b1 << 10;

                            addon.linkedUnit = c;
                            cUNIT.linkedUnit = addon.unitclass;

                            addon.ImageReset();
                        }
                    }
                    break;
                case 113:
                    //팩토리
                    {
                        CUNIT addon = UNIT.SingleOrDefault((x) => (x.X == cUNIT.X + 96) & (x.Y == cUNIT.Y + 16) & (x.unitID == 120));
                        if (addon != null)
                        {
                            addon.linkFlag = 0b1 << 10;
                            cUNIT.linkFlag = 0b1 << 10;

                            addon.linkedUnit = c;
                            cUNIT.linkedUnit = addon.unitclass;

                            addon.ImageReset();
                        }
                    }
                    break;
                case 114:
                    //스타포트
                    {
                        CUNIT addon = UNIT.SingleOrDefault((x) => (x.X == cUNIT.X + 96) & (x.Y == cUNIT.Y + 16) & (x.unitID == 115));
                        if (addon != null)
                        {
                            addon.linkFlag = 0b1 << 10;
                            cUNIT.linkFlag = 0b1 << 10;

                            addon.linkedUnit = c;
                            cUNIT.linkedUnit = addon.unitclass;

                            addon.ImageReset();
                        }
                    }
                    break;
                case 116:
                    //사이언스
                    {
                        CUNIT addon = UNIT.SingleOrDefault((x) => (x.X == cUNIT.X + 96) & (x.Y == cUNIT.Y + 16) & ((x.unitID == 117) | (x.unitID == 118)));
                        if (addon != null)
                        {
                            addon.linkFlag = 0b1 << 10;
                            cUNIT.linkFlag = 0b1 << 10;

                            addon.linkedUnit = c;
                            cUNIT.linkedUnit = addon.unitclass;

                            addon.ImageReset();
                        }
                    }
                    break;
                case 107:
                //컴셋
                case 108:
                    //사일로
                    {
                        CUNIT addon = UNIT.SingleOrDefault((x) => (x.X == cUNIT.X - 96) & (x.Y == cUNIT.Y - 16) & (x.unitID == 106));
                        if (addon != null)
                        {
                            addon.linkFlag = 0b1 << 10;
                            cUNIT.linkFlag = 0b1 << 10;

                            addon.linkedUnit = c;
                            cUNIT.linkedUnit = addon.unitclass;

                            cUNIT.ImageReset();
                        }
                    }
                    break;
                case 115:
                    //컨트롤타워
                    {
                        CUNIT addon = UNIT.SingleOrDefault((x) => (x.X == cUNIT.X - 96) & (x.Y == cUNIT.Y - 16) & (x.unitID == 114));
                        if (addon != null)
                        {
                            addon.linkFlag = 0b1 << 10;
                            cUNIT.linkFlag = 0b1 << 10;

                            addon.linkedUnit = c;
                            cUNIT.linkedUnit = addon.unitclass;

                            cUNIT.ImageReset();
                        }
                    }
                    break;
                case 117:
                //커벌트옵스
                case 118:
                    //피직스랩
                    {
                        CUNIT addon = UNIT.SingleOrDefault((x) => (x.X == cUNIT.X - 96) & (x.Y == cUNIT.Y - 16) & (x.unitID == 116));
                        if (addon != null)
                        {
                            addon.linkFlag = 0b1 << 10;
                            cUNIT.linkFlag = 0b1 << 10;

                            addon.linkedUnit = c;
                            cUNIT.linkedUnit = addon.unitclass;

                            cUNIT.ImageReset();
                        }
                    }
                    break;
                case 120:
                    //머신샵
                    {
                        CUNIT addon = UNIT.SingleOrDefault((x) => (x.X == cUNIT.X - 96) & (x.Y == cUNIT.Y - 16) & (x.unitID == 113));
                        if (addon != null)
                        {
                            addon.linkFlag = 0b1 << 10;
                            cUNIT.linkFlag = 0b1 << 10;

                            addon.linkedUnit = c;
                            cUNIT.linkedUnit = addon.unitclass;

                            cUNIT.ImageReset();
                        }
                    }
                    break;
                case 134:
                    //나이더스 카날
                    if (UNIT.Count != 0)
                    {
                        CUNIT lastunit = UNIT.SingleOrDefault((x) => (x.linkedUnit == 0) & (x.unitID == 134));

                        if (lastunit != null)
                        {
                            lastunit.linkFlag = 0b1 << 9;
                            cUNIT.linkFlag = 0b1 << 9;

                            lastunit.linkedUnit = c;
                            cUNIT.linkedUnit = lastunit.unitclass;
                        }
                    }
                    break;
            }




            UNIT.Add(cUNIT);
            mapEditor.MinimapUnitRefresh();
            mapEditor.IndexedUnitCancel();
        }



        public ObservableCollection<CUNIT> UNIT = new ObservableCollection<CUNIT>();
        public class CUNIT : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged(string info)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(info));
                }
            }


            [JsonIgnore]
            public System.Windows.Media.SolidColorBrush _BackGround;
            [JsonIgnore]
            public System.Windows.Media.SolidColorBrush BackGround
            {
                get
                {
                    return _BackGround;
                }
                set
                {
                    _BackGround = value;
                    OnPropertyChanged("BackGround");
                }
            }



            [JsonIgnore]
            private MapEditor mapEditor;
            public void SetMapEditor(MapEditor mapEditor)
            {
                this.mapEditor = mapEditor;
            }

            [JsonIgnore]
            public string NAME
            {
                get
                {
                    return mapEditor.mapdata.GetCodeName(Codetype.Unit, unitID);
                }
                set
                {

                }
            }

            [JsonIgnore]
            private int _index;
            [JsonIgnore]
            public int INDEX
            {
                get
                {
                    return _index;
                }
                set
                {
                    _index = value;
                    OnPropertyChanged("INDEX");
                }
            }



            [JsonIgnore]
            public int indexof
            {
                get
                {
                    return mapEditor.mapdata.UNIT.IndexOf(this);
                }
                set
                {

                }
            }


            public CUNIT(BinaryReader br)
            {
                unitclass = br.ReadUInt32();
                X = br.ReadUInt16();
                Y = br.ReadUInt16();
                unitID = br.ReadUInt16();
                linkFlag = br.ReadUInt16();
                validstatusFlag = br.ReadUInt16();
                validunitFlag = br.ReadUInt16();
                player = br.ReadByte();
                hitPoints = br.ReadByte();
                shieldPoints = br.ReadByte();
                energyPoints = br.ReadByte();
                resoruceAmount = br.ReadUInt32();
                hangar = br.ReadUInt16();
                stateFlag = br.ReadUInt16();
                unused = br.ReadUInt32();
                linkedUnit = br.ReadUInt32();
                ImageReset();
            }

            public CUNIT()
            {
                //unitclass = (uint)UseMapEditor.Global.WindowTool.random.Next(0, 65535);
            }

            public CUNIT(CUNIT cUNIT)
            {
                //unitclass = cUNIT.unitclass;
                X = cUNIT.X;
                Y = cUNIT.Y;
                unitID = cUNIT.unitID;
                //linkFlag = cUNIT.linkFlag;
                linkFlag = 0;
                validstatusFlag = cUNIT.validstatusFlag;
                validunitFlag = cUNIT.validunitFlag;
                player = cUNIT.player;
                hitPoints = cUNIT.hitPoints;
                shieldPoints = cUNIT.shieldPoints;
                energyPoints = cUNIT.energyPoints;
                resoruceAmount = cUNIT.resoruceAmount;
                hangar = cUNIT.hangar;
                stateFlag = cUNIT.stateFlag;
                unused = cUNIT.unused;
                //linkedUnit = cUNIT.linkedUnit;
                linkedUnit = 0;
                ImageReset();
            }


            public void ImageReset()
            {
                Images.Clear();


                int Graphics = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Graphics", unitID).Data;
                int Level = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Elevation Level", unitID).Data;
                int Sprite = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.flingy, "Sprite", Graphics).Data;
                int ImageID = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.sprites, "Image File", Sprite).Data;


                BoxWidth = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "StarEdit Placement Box Width", unitID).Data;
                BoxHeight = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "StarEdit Placement Box Height", unitID).Data;


                int Dir = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Unit Direction", unitID).Data;

                if (Dir == 32)
                {
                    Dir = -1;
                }

                CImage.DrawType drawType = CImage.DrawType.Normal;

                int startanim = 0;
                if ((stateFlag & 0b1) > 0)
                {
                    //Unit is cloaked
                    if (cloakvalid)
                    {
                        drawType = CImage.DrawType.Clock;
                    }
                }
                if ((stateFlag & 0b10) > 0)
                {
                    //Unit is burrowed
                    if (burrowvalid)
                    {
                        startanim = 25;
                    }
                }
                if ((stateFlag & 0b100) > 0)
                {
                    //Building is in transit
                    if (tranvalid)
                    {
                        startanim = 18;
                    }
                }
                if ((stateFlag & 0b1000) > 0)
                {
                    //Unit is hallucinated
                    if (hallvalid)
                    {
                        drawType = CImage.DrawType.Hallaction;
                    }
                }





                int StatusFlag = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Special Ability Flags", unitID).Data;
                if ((StatusFlag & 0b10) > 0)
                {
                    if ((linkFlag & (0b1 << 10)) > 0)
                    {
                        //Bit 10 - Addon Link
                        startanim = 17;
                    }
                    else
                    {
                        startanim = 18;
                    }
                }



                CImage p = new CImage((int)unitclass, Images, ImageID, Dir, player, _drawType: drawType, level: Level, _StartAnim: startanim);
                Images.Add(p);
                int Subunit = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Subunit 1", unitID).Data;
                if (Subunit != 228)
                {
                    Graphics = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Graphics", Subunit).Data;
                    Level = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.units, "Elevation Level", Subunit).Data;
                    Sprite = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.flingy, "Sprite", Graphics).Data;
                    ImageID = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.sprites, "Image File", Sprite).Data;

                    if (ImageID == 254)
                    {
                        Dir += 16;
                    }
                    Images.Add(new CImage((int)unitclass, Images, ImageID, Dir, player, _parentImage: p, _drawType: drawType, level: Level + 1, _StartAnim: startanim));
                }
            }



            [JsonIgnore]
            public int BoxWidth;
            [JsonIgnore]
            public int BoxHeight;

            [JsonIgnore]
            public uint unitclass;//u32: The unit's class instance (sort of a "serial number")
            public ushort X { get; set; }//U16: X coordinate of unit
            public ushort Y { get; set; }//U16: Y coordinate of unit
            private ushort _unitID;//u16: Unit ID
            public ushort unitID
            {
                get
                {
                    return _unitID;
                }
                set
                {
                    if(value < 228)
                    {
                        _unitID = value;
                        OnPropertyChanged("NAME");
                        Images.Clear();
                    }
                }
            }


            [JsonIgnore]
            public ushort linkFlag;//u16: Type of relation to another building (i.e. add-on, nydus link)
                                   //Bit 9 - Nydus Link
                                   //Bit 10 - Addon Link
            public ushort validstatusFlag;//u16: Flags of special properties which can be applied to the unit and are valid:
                                          //Bit 0 - Cloak is valid
                                          //Bit 1 - Burrow is valid
                                          //Bit 2 - In transit is valid
                                          //Bit 3 - Hallucinated is valid
                                          //Bit 4 - Invincible is valid
                                          //Bit 5-15 - Unused
            public ushort validunitFlag;//u16: Out of the elements of the unit data, the properties which can be changed by the map maker:
                                        //Bit 0 - Owner player is valid (the unit is not a critter, start location, etc.; not a neutral unit)
                                        //Bit 1 - HP is valid
                                        //Bit 2 - Shields is valid
                                        //Bit 3 - Energy is valid (unit is a wraith, etc.)
                                        //Bit 4 - Resource amount is valid (unit is a mineral patch, vespene geyser, etc.)
                                        //Bit 5 - Amount in hangar is valid (unit is a reaver, carrier, etc.)
                                        //Bit 6-15 - Unused


            [JsonIgnore]
            public bool cloakvalid
            {
                get
                {
                    return ((validstatusFlag & (0b1 << 0)) > 0);
                }
                set
                {
                    if (value)
                    {
                        validstatusFlag |= (ushort)(0b1 << 0);
                    }
                    else
                    {
                        validstatusFlag &= unchecked((ushort)~(0b1 << 0));
                    }
                    Images.Clear();
                }
            }
            [JsonIgnore]
            public bool burrowvalid
            {
                get
                {
                    return ((validstatusFlag & (0b1 << 1)) > 0);
                }
                set
                {
                    if (value)
                    {
                        validstatusFlag |= (ushort)(0b1 << 1);
                    }
                    else
                    {
                        validstatusFlag &= unchecked((ushort)~(0b1 << 1));
                    }
                    Images.Clear();
                }
            }
            [JsonIgnore]
            public bool tranvalid
            {
                get
                {
                    return ((validstatusFlag & (0b1 << 2)) > 0);
                }
                set
                {
                    if (value)
                    {
                        validstatusFlag |= (ushort)(0b1 << 2);
                    }
                    else
                    {
                        validstatusFlag &= unchecked((ushort)~(0b1 << 2));
                    }
                    Images.Clear();
                }
            }
            [JsonIgnore]
            public bool hallvalid
            {
                get
                {
                    return ((validstatusFlag & (0b1 << 3)) > 0);
                }
                set
                {
                    if (value)
                    {
                        validstatusFlag |= (ushort)(0b1 << 3);
                    }
                    else
                    {
                        validstatusFlag &= unchecked((ushort)~(0b1 << 3));
                    }
                    Images.Clear();
                }
            }
            [JsonIgnore]
            public bool invinvalid
            {
                get
                {
                    return ((validstatusFlag & (0b1 << 4)) > 0);
                }
                set
                {
                    if (value)
                    {
                        validstatusFlag |= (ushort)(0b1 << 4);
                    }
                    else
                    {
                        validstatusFlag &= unchecked((ushort)~(0b1 << 4));
                    }
                }
            }



            [JsonIgnore]
            public bool hpvalid
            {
                get
                {
                    return ((validunitFlag & (0b1 << 1)) > 0);
                }
                set
                {
                    if (value)
                    {
                        validunitFlag |= (ushort)(0b1 << 1);
                    }
                    else
                    {
                        validunitFlag &= unchecked((ushort)~(0b1 << 1));
                    }
                }
            }
            [JsonIgnore]
            public bool shvalid
            {
                get
                {
                    return ((validunitFlag & (0b1 << 2)) > 0);
                }
                set
                {
                    if (value)
                    {
                        validunitFlag |= (ushort)(0b1 << 2);
                    }
                    else
                    {
                        validunitFlag &= unchecked((ushort)~(0b1 << 2));
                    }
                }
            }
            [JsonIgnore]
            public bool envalid
            {
                get
                {
                    return ((validunitFlag & (0b1 << 3)) > 0);
                }
                set
                {
                    if (value)
                    {
                        validunitFlag |= (ushort)(0b1 << 3);
                    }
                    else
                    {
                        validunitFlag &= unchecked((ushort)~(0b1 << 3));
                    }
                }
            }
            [JsonIgnore]
            public bool resvalid
            {
                get
                {
                    return ((validunitFlag & (0b1 << 4)) > 0);
                }
                set
                {
                    if (value)
                    {
                        validunitFlag |= (ushort)(0b1 << 4);
                    }
                    else
                    {
                        validunitFlag &= unchecked((ushort)~(0b1 << 4));
                    }
                }
            }
            [JsonIgnore]
            public bool hangarvalid
            {
                get
                {
                    return ((validunitFlag & (0b1 << 5)) > 0);
                }
                set
                {
                    if (value)
                    {
                        validunitFlag |= (ushort)(0b1 << 5);
                    }
                    else
                    {
                        validunitFlag &= unchecked((ushort)~(0b1 << 5));
                    }
                }
            }







            public byte player { get; set; }//u8: Player number of owner (0-based)
            public byte hitPoints { get; set; }//u8: Hit points % (1-100)
            public byte shieldPoints { get; set; }//u8: Shield points % (1-100)
            public byte energyPoints { get; set; }//u8: Energy points % (1-100)
            public uint resoruceAmount { get; set; }//u32: Resource amount
            public ushort hangar { get; set; }//u16: Number of units in hangar
            public ushort stateFlag;//u16: Unit state flags
                                    //Bit 0 - Unit is cloaked
                                    //Bit 1 - Unit is burrowed
                                    //Bit 2 - Building is in transit
                                    //Bit 3 - Unit is hallucinated
                                    //Bit 4 - Unit is invincible
                                    //Bit 5-15 - Unused



            [JsonIgnore]
            public bool cloakstate
            {
                get
                {
                    return ((stateFlag & (0b1 << 0)) > 0);
                }
                set
                {
                    if (value)
                    {
                        stateFlag |= (ushort)(0b1 << 0);
                    }
                    else
                    {
                        stateFlag &= unchecked((ushort)~(0b1 << 0));
                    }
                    Images.Clear();
                }
            }
            [JsonIgnore]
            public bool burrowstate
            {
                get
                {
                    return ((stateFlag & (0b1 << 1)) > 0);
                }
                set
                {
                    if (value)
                    {
                        stateFlag |= (ushort)(0b1 << 1);
                    }
                    else
                    {
                        stateFlag &= unchecked((ushort)~(0b1 << 1));
                    }
                    Images.Clear();
                }
            }
            [JsonIgnore]
            public bool buildstate
            {
                get
                {
                    return ((stateFlag & (0b1 << 2)) > 0);
                }
                set
                {
                    if (value)
                    {
                        stateFlag |= (ushort)(0b1 << 2);
                    }
                    else
                    {
                        stateFlag &= unchecked((ushort)~(0b1 << 2));
                    }
                    Images.Clear();
                }
            }
            [JsonIgnore]
            public bool hallstate
            {
                get
                {
                    return ((stateFlag & (0b1 << 3)) > 0);
                }
                set
                {
                    if (value)
                    {
                        stateFlag |= (ushort)(0b1 << 3);
                    }
                    else
                    {
                        stateFlag &= unchecked((ushort)~(0b1 << 3));
                    }
                    Images.Clear();
                }
            }
            [JsonIgnore]
            public bool invincstate
            {
                get
                {
                    return ((stateFlag & (0b1 << 4)) > 0);
                }
                set
                {
                    if (value)
                    {
                        stateFlag |= (ushort)(0b1 << 4);
                    }
                    else
                    {
                        stateFlag &= unchecked((ushort)~(0b1 << 4));
                    }
                }
            }


            public uint unused;//u32: Unused
            [JsonIgnore]
            public uint linkedUnit;//u32: Class instance of the unit to which this unit is related to (i.e. via an add-on, nydus link, etc.). It is "0" if the unit is not linked to any other unit.


            [JsonIgnore]
            public List<CImage> Images = new List<CImage>();
        }
    }
}
