using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.FileData;
using UseMapEditor.Tools;

namespace Data.Map
{
    public partial class MapData
    {

        //public ObservableCollection<TriggerItemBinding> TriggerItems = new ObservableCollection<TriggerItemBinding>();




        private void TriggerSave()
        {
            TRIG.Clear();
            for (int i = 0; i < Triggers.Count; i++)
            {
                RAWTRIGMBRF rAWTRIGMBRF = new RAWTRIGMBRF(Triggers[i]);

                TRIG.Add(rAWTRIGMBRF);
            }
            MBRF.Clear();
            for (int i = 0; i < Brifings.Count; i++)
            {
                RAWTRIGMBRF rAWTRIGMBRF = new RAWTRIGMBRF(Brifings[i]);
                for (int c = 0; c < 16; c++)
                {
                    rAWTRIGMBRF.conditions[c].condtype = 13;
                }

                MBRF.Add(rAWTRIGMBRF);
            }
        }

        private void TriggerLoad()
        {
            //트리거 데이터 로드
            for (int i = 0; i < TRIG.Count; i++)
            {
                CTrigger trigger = new CTrigger(this, TRIG[i], true);



                Triggers.Add(trigger);
            }
            for (int i = 0; i < MBRF.Count; i++)
            {
                CTrigger trigger = new CTrigger(this, MBRF[i], false);



                Brifings.Add(trigger);
            }
        }

        public List<RAWTRIGMBRF> TRIG = new List<RAWTRIGMBRF>();
        public List<RAWTRIGMBRF> MBRF = new List<RAWTRIGMBRF>();


        public ObservableCollection<CTrigger> Triggers = new ObservableCollection<CTrigger>();
        public ObservableCollection<CTrigger> Brifings = new ObservableCollection<CTrigger>();








        public class RAWTRIGMBRF
        {
            public RAWTRIGMBRF(CTrigger cTrigger)
            {
                for (int c = 0; c < 16; c++)
                {
                    RawCondition condition = new RawCondition();
                    if (cTrigger.conditions.Count > c)
                    {
                        TrigItem trigItem = cTrigger.conditions[c];

                        if(trigItem.type == 24)
                        {
                            long offset = trigItem.args[0].VALUE;
                            long player = (offset - 0x58A364) / 4;
                            long unitid = 0;
                            long modifier = trigItem.args[1].VALUE;
                            long value = trigItem.args[2].VALUE;

                            long mask = trigItem.args[3].VALUE;
                            long maskuse = trigItem.args[4].VALUE;

                            condition.values[1] = player;
                            condition.values[3] = unitid;
                            condition.values[2] = value;
                            condition.values[4] = modifier;
                            condition.values[0] = mask;
                            condition.values[8] = maskuse;
                            condition.valueSet();
                            condition.condtype = (byte)15;
                        }
                        else
                        {
                            for (int i = 0; i < trigItem.args.Count; i++)
                            {
                                int pos = trigItem.args[i].argDefine.pos;

                                condition.values[pos] = trigItem.args[i].GetCHKValue;
                            }
                            condition.valueSet();
                            condition.condtype = (byte)trigItem.type;
                        }



                    }

                    conditions[c] = condition;
                }
                for (int a = 0; a < 64; a++)
                {
                    RawAction action = new RawAction();
                    if (cTrigger.actions.Count > a)
                    {
                        TrigItem trigItem = cTrigger.actions[a];

                        if (trigItem.type == 57)
                        {
                            long offset = trigItem.args[0].VALUE;
                            long player = (offset - 0x58A364) / 4;
                            long unitid = 0;
                            long modifier = trigItem.args[1].VALUE;
                            long value = trigItem.args[2].VALUE;

                            long mask = trigItem.args[3].VALUE;
                            long maskuse = trigItem.args[4].VALUE;

                            action.values[4] = player;
                            action.values[6] = unitid;
                            action.values[5] = value;
                            action.values[8] = modifier;
                            action.values[0] = mask;
                            action.values[10] = maskuse;
                            action.valueSet();
                            action.acttype = (byte)45;
                        }
                        else
                        {
                            for (int i = 0; i < trigItem.args.Count; i++)
                            {
                                int pos = trigItem.args[i].argDefine.pos;

                                action.values[pos] = trigItem.args[i].GetCHKValue;
                            }
                            action.valueSet();
                            action.acttype = (byte)trigItem.type;
                        }
                    }

                    actions[a] = action;
                }


                exeflag = cTrigger.exeflag;
                playerlist = cTrigger.playerlist;
                trigindex = 0;
            }

            public RAWTRIGMBRF(BinaryReader br)
            {
                for (int c = 0; c < 16; c++)
                {
                    RawCondition condition = new RawCondition();

                    condition.locid = br.ReadUInt32();//u32: Location number for the condition (1 based -- 0 refers to No Location), EUD Bitmask for a Death condition if the MaskFlag is set to "SC"
                    condition.player = br.ReadUInt32();//u32: Group that the condition applies to
                    condition.amount = br.ReadUInt32();//u32: Qualified number(how many/resource amount)
                    condition.unitid = br.ReadUInt16();//u16: Unit ID condition applies to
                    condition.comparison = br.ReadByte();//u8: Numeric comparison, switch state
                    condition.condtype = br.ReadByte();//u8: Condition byte
                    condition.restype = br.ReadByte();//u8: Resource type, score type, Switch number(0-based)
                    condition.flags = br.ReadByte();//u8: Flags
                    condition.maskflag = br.ReadUInt16();//u16: MaskFlag: set to "SC" (0x53, 0x43) when using the bitmask for EUDs, 0 otherwise

                    if(condition.condtype > 23)
                    {
                        condition.condtype = 0;
                    }

                    condition.values[0] = condition.locid;
                    condition.values[1] = condition.player;
                    condition.values[2] = condition.amount;
                    condition.values[3] = condition.unitid;
                    condition.values[4] = condition.comparison;
                    condition.values[5] = condition.condtype;
                    condition.values[6] = condition.restype;
                    condition.values[7] = condition.flags;
                    condition.values[8] = condition.maskflag;

                    conditions[c] = condition;
                }
                for (int a = 0; a < 64; a++)
                {
                    RawAction action = new RawAction();

                    action.locid1 = br.ReadUInt32();//u32: Location - source location in "Order" and "Move Unit", dest location in "Move Location" (1 based -- 0 refers to No Location), EUD Bitmask for a Death action if the MaskFlag is set to "SC"
                    action.strid = br.ReadUInt32();//u32: String number for trigger text(0 means no string)
                    action.wavid = br.ReadUInt32();//u32: WAV string number(0 means no string)
                    action.time = br.ReadUInt32();//u32: Seconds/milliseconds of time
                    action.player1 = br.ReadUInt32();//u32: First(or only) Group/Player affected.
                    action.player2 = br.ReadUInt32();//u32: Second group affected, secondary location (1-based), CUWP #, number, AI script (4-byte string), switch (0-based #)
                    action.unitid = br.ReadUInt16();//u16: Unit type, score type, resource type, alliance status
                    action.acttype = br.ReadByte();//u8: Action byte
                    action.amount = br.ReadByte();//u8: Number of units(0 means All Units), action state, unit order, number modifier
                    action.flags = br.ReadByte();//u8: Flags
                    action.padding = br.ReadByte();//u8: Padding
                    action.maskflag = br.ReadUInt16();//u16 (2 bytes): MaskFlag: set to "SC"(0x53, 0x43) when using the bitmask for EUDs, 0 otherwise

                    if (action.acttype > 57)
                    {
                        action.acttype = 0;
                    }


                    action.values[0] = action.locid1;
                    action.values[1] = action.strid;
                    action.values[2] = action.wavid;
                    action.values[3] = action.time;
                    action.values[4] = action.player1;
                    action.values[5] = action.player2;
                    action.values[6] = action.unitid;
                    action.values[7] = action.acttype;
                    action.values[8] = action.amount;
                    action.values[9] = action.flags;
                    action.values[10] = action.maskflag;


                    actions[a] = action;
                }

                exeflag = br.ReadUInt32();
                playerlist = br.ReadBytes(27);
                trigindex = br.ReadByte();
            }

            //function Condition(locid, player, amount, unitid, comparison, condtype, restype, flags)
            //return {locid, player, amount, unitid, comparison, condtype, restype, flags}
            //end

            //function Action(locid1, strid, wavid, time, player1, player2, unitid, acttype, amount, flags)
            //return {locid1, strid, wavid, time, player1, player2, unitid, acttype, amount}
            //end

            //2400byte
            //16 Conditions(20 byte struct)
            public RawCondition[] conditions = new RawCondition[16];
            public class RawCondition
            {
                //Every trigger has 16 of the following format, even if only one condition is used.See the appendix for information on which items are used for what conditions.
                public uint locid;//u32: Location number for the condition (1 based -- 0 refers to No Location), EUD Bitmask for a Death condition if the MaskFlag is set to "SC"
                public uint player;//u32: Group that the condition applies to
                public uint amount;//u32: Qualified number(how many/resource amount)
                public ushort unitid;//u16: Unit ID condition applies to
                public byte comparison;//u8: Numeric comparison, switch state
                public byte condtype;//u8: Condition byte
                public byte restype;//u8: Resource type, score type, Switch number(0-based)
                public byte flags;//u8: Flags
                                  //Bit 0 - Unknown/unused
                                  //Bit 1 - Enabled flag.If on, the trigger action/condition is disabled/ignored
                                  //Bit 2 - Always display flag.
                                  //Bit 3 - Unit properties is used. (Note: This is used in *.trg files)
                                  //Bit 4 - Unit type is used.Cleared in "Offset + Mask" EUD conditions.May not be necessary otherwise?
                                  //Bit 5-7 - Unknown/unused
                public ushort maskflag;//u16: MaskFlag: set to "SC" (0x53, 0x43) when using the bitmask for EUDs, 0 otherwise

                public long[] values = new long[9];
                public void valueSet()
                {
                    locid = (uint)values[0];
                    player = (uint)values[1];
                    amount = (uint)values[2];
                    unitid = (ushort)values[3];
                    comparison = (byte)values[4];
                    condtype = (byte)values[5];
                    restype = (byte)values[6];
                    flags = (byte)values[7];
                    maskflag = (ushort)values[8];
                }
            }


            public RawAction[] actions = new RawAction[64];
            //64 Actions(32 byte struct)
            public class RawAction
            {
                //Immediately following the 16 conditions, there are 64 actions.There will always be 64 of the following structure, even if some of them are unused.
                public uint locid1;//u32: Location - source location in "Order" and "Move Unit", dest location in "Move Location" (1 based -- 0 refers to No Location), EUD Bitmask for a Death action if the MaskFlag is set to "SC"
                public uint strid;//u32: String number for trigger text(0 means no string)
                public uint wavid;//u32: WAV string number(0 means no string)
                public uint time;//u32: Seconds/milliseconds of time
                public uint player1;//u32: First(or only) Group/Player affected.
                public uint player2;//u32: Second group affected, secondary location (1-based), CUWP #, number, AI script (4-byte string), switch (0-based #)
                public ushort unitid;//u16: Unit type, score type, resource type, alliance status
                public byte acttype;//u8: Action byte
                public byte amount;//u8: Number of units(0 means All Units), action state, unit order, number modifier
                public byte flags;//u8: Flags
                                  //Bit 0 - Ignore a wait/transmission once.
                                  //Bit 1 - Enabled flag. If on, the trigger action/condition is disabled.
                                  //Bit 2 - Always display flag - when not set: if the user has turned off subtitles (see sound options) the text will not display, when set: text will always display
                                  //Bit 3 - Unit properties is used.Staredit uses this for *.trg files.
                                  //Bit 4 - Unit type is used.Cleared in "Offset + Mask" EUD actions.
                                  //Bit 5-7 - Unknown/unused
                public byte padding;//u8: Padding
                public ushort maskflag;//u16 (2 bytes): MaskFlag: set to "SC"(0x53, 0x43) when using the bitmask for EUDs, 0 otherwise

                public long[] values = new long[11];
                public void valueSet()
                {
                    locid1 = (uint)values[0];
                    strid = (uint)values[1];
                    wavid = (uint)values[2];
                    time = (uint)values[3];
                    player1 = (uint)values[4];
                    player2 = (uint)values[5];
                    unitid = (ushort)values[6];
                    acttype = (byte)values[7];
                    amount = (byte)values[8];
                    flags = (byte)values[9];
                    maskflag = (ushort)values[10];
                }
            }




            //Player Execution
            //Following the 16 conditions and 64 actions, every trigger also has this structure
            public uint exeflag;//u32: execution flags
            //Bit 0 - All conditions are met, executing actions, cleared on the next trigger loop.
            //Bit 1 - Ignore the following actions: Defeat, Draw.
            //Bit 2 - Preserve trigger. (Can replace Preserve Trigger action)
            //Bit 3 - Ignore execution.
            //Bit 4 - Ignore all of the following actions for this trigger until the next trigger loop: Wait, PauseGame, Transmission, PlayWAV, DisplayTextMessage, CenterView, MinimapPing, TalkingPortrait, and MuteUnitSpeech.
            //Bit 5 - This trigger has paused the game, ignoring subsequent calls to Pause Game(Unpause Game clears this flag only in the same trigger), may automatically call unpause at the end of action execution ?
            //Bit 6 - Wait skipping disabled for this trigger, cleared on next trigger loop.
            //Bit 7 - 31 - Unknown / unused
            public byte[] playerlist = new byte[27];//u8[27]: 1 byte for each player in the #List of Players/Group IDs
            //00 - Trigger is not executed for player
            //01 - Trigger is executed for player
            public byte trigindex;//u8: Index of the current action, in StarCraft this is incremented after each action is executed, trigger execution ends when this is 64(Max Actions) or an action is encountered with Action byte as 0

        }
    }
}
