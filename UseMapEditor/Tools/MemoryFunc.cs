using UseMapEditor.FileData;

namespace UseMapEditor.Tools
{
    public class MemoryFunc
    {
        public static void ApplyMemoryFunc(TrigItem trigItem)
        {
            //0x58A364 + Player × 4 + Unit ID × 48

            long player = (int)trigItem.args[0].VALUE;
            long unit = (int)trigItem.args[3].VALUE;

            uint offset = (uint)(0x58A364 + player * 4 + unit * 48);

            if((0x58A364 <= offset) & (offset < 0x58CE24))
            {
                return;
            }


            trigItem.args[0].VALUE = offset;
            trigItem.args.RemoveAt(3);

            if (trigItem.name == "SetDeaths")
            {
                trigItem.type = 57;
                trigItem.triggerDefine = Global.WindowTool.triggerManger.Actions[58];
                trigItem.args[0].argDefine = trigItem.triggerDefine.argDefines[0];
                trigItem.args[0].ARGTYPE = TriggerManger.ArgType.OFFSET;
                trigItem.name = "SetMemory";
            }
            else if (trigItem.name == "Deaths")
            {
                trigItem.type = 24;
                trigItem.triggerDefine = Global.WindowTool.triggerManger.Actions[25];
                trigItem.args[0].argDefine = trigItem.triggerDefine.argDefines[0];
                trigItem.args[0].ARGTYPE = TriggerManger.ArgType.OFFSET;
                trigItem.name = "Memory";
            }
        }
        public static string GetTEPDeathText(TrigItem trigItem)
        {
            if (trigItem.args[5].VALUE == 17235)
            {
                return (trigItem.name + "X(" + trigItem.args[0].GetCode + ", " + trigItem.args[1].GetCode + ", " + trigItem.args[2].GetCode + ", " + trigItem.args[3].GetCode + ", " + trigItem.args[4].GetCode + ")");
            }
            else
            {
                return (trigItem.name + "(" + trigItem.args[0].GetCode + ", " + trigItem.args[1].GetCode + ", " + trigItem.args[2].GetCode + ", " + trigItem.args[3].GetCode + ")");
            }
        }

        public static string GetTEPMemoryText(TrigItem trigItem)
        {
            if (trigItem.args[4].VALUE == 17235)
            {
                return (trigItem.name + "X(" + trigItem.args[0].GetCode + ", " + trigItem.args[1].GetCode + ", " + trigItem.args[2].GetCode + ", " + trigItem.args[3].GetCode + ")");
            }
            else
            {
                return (trigItem.name + "(" + trigItem.args[0].GetCode + ", " + trigItem.args[1].GetCode + ", " + trigItem.args[2].GetCode + ")");
            }
        }

    }
}
