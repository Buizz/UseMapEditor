using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.FileData;

namespace Data.Map
{
    public partial class MapData
    {

        public enum Codetype
        {
            Unit,
            Upgrade,
            Tech,
            Sprite
        }

        public bool IsCustomUnitName(int index)
        {
            if(index >= 228)
            {
                return false;
            }


            string d = UNIx.STRING[index].String;
            return UNIx.STRING[index].IsLoaded;
        }
        public string GetUnitName(int index)
        {
            if (index >= 228)
            {
                string[] addstring = { "없음", "유닛과 건물", "유닛", "건물", "생산건물" };
                return addstring[index - 228];
            }



            string d = UNIx.STRING[index].String;
            if (UNIx.STRING[index].IsLoaded)
            {
                return d;
            }

            return "???";
        }
        public string GetMapUnitName(int index, bool IsTran = true)
        {
            if (index >= 228)
            {
                if (IsTran)
                {
                    string[] addstring = { "없음", "유닛과 건물", "유닛", "건물", "생산건물" };
                    return addstring[index - 228];
                }
                else
                {
                    string[] addstring = { "None", "Any unit", "Men", "Buildings", "Factories" };
                    return addstring[index - 228];
                }

            }
            string org;

            if (IsTran)
            {
                org = UseMapEditor.Global.WindowTool.GetStat_txt(index);
            }
            else
            {
                org = UseMapEditor.Global.WindowTool.GetEngStat_txt(index);
            }
            return org;
        }




        string[] DefUnitDict = {
    "Terran Marine",
    "Terran Ghost",
    "Terran Vulture",
    "Terran Goliath",
    "Goliath Turret",
    "Terran Siege Tank (Tank Mode)",
    "Tank Turret type   1",
    "Terran SCV",
    "Terran Wraith",
    "Terran Science Vessel",
    "Gui Montag (Firebat)",
    "Terran Dropship",
    "Terran Battlecruiser",
    "Vulture Spider Mine",
    "Nuclear Missile",
    "Terran Civilian",
    "Sarah Kerrigan (Ghost)",
    "Alan Schezar (Goliath)",
    "Alan Turret",
    "Jim Raynor (Vulture)",
    "Jim Raynor (Marine)",
    "Tom Kazansky (Wraith)",
    "Magellan (Science Vessel)",
    "Edmund Duke (Siege Tank)",
    "Duke Turret type   1",
    "Edmund Duke (Siege Mode)",
    "Duke Turret type   2",
    "Arcturus Mengsk (Battlecruiser)",
    "Hyperion (Battlecruiser)",
    "Norad II (Battlecruiser)",
    "Terran Siege Tank (Siege Mode)",
    "Tank Turret type   2",
    "Terran Firebat",
    "Scanner Sweep",
    "Terran Medic",
    "Zerg Larva",
    "Zerg Egg",
    "Zerg Zergling",
    "Zerg Hydralisk",
    "Zerg Ultralisk",
    "Zerg Broodling",
    "Zerg Drone",
    "Zerg Overlord",
    "Zerg Mutalisk",
    "Zerg Guardian",
    "Zerg Queen",
    "Zerg Defiler",
    "Zerg Scourge",
    "Torrasque (Ultralisk)",
    "Matriarch (Queen)",
    "Infested Terran",
    "Infested Kerrigan (Infested Terran)",
    "Unclean One (Defiler)",
    "Hunter Killer (Hydralisk)",
    "Devouring One (Zergling)",
    "Kukulza (Mutalisk)",
    "Kukulza (Guardian)",
    "Yggdrasill (Overlord)",
    "Terran Valkyrie",
    "Cocoon",
    "Protoss Corsair",
    "Protoss Dark Templar",
    "Zerg Devourer",
    "Protoss Dark Archon",
    "Protoss Probe",
    "Protoss Zealot",
    "Protoss Dragoon",
    "Protoss High Templar",
    "Protoss Archon",
    "Protoss Shuttle",
    "Protoss Scout",
    "Protoss Arbiter",
    "Protoss Carrier",
    "Protoss Interceptor",
    "Dark Templar (Hero)",
    "Zeratul (Dark Templar)",
    "Tassadar/Zeratul (Archon)",
    "Fenix (Zealot)",
    "Fenix (Dragoon)",
    "Tassadar (Templar)",
    "Mojo (Scout)",
    "Warbringer (Reaver)",
    "Gantrithor (Carrier)",
    "Protoss Reaver",
    "Protoss Observer",
    "Protoss Scarab",
    "Danimoth (Arbiter)",
    "Aldaris (Templar)",
    "Artanis (Scout)",
    "Rhynadon (Badlands)",
    "Bengalaas (Jungle)",
    "Unused type   1",
    "Unused type   2",
    "Scantid (Desert)",
    "Kakaru (Twilight)",
    "Ragnasaur (Ashworld)",
    "Ursadon (Ice World)",
    "Lurker Egg",
    "Raszagal (Corsair)",
    "Samir Duran (Ghost)",
    "Alexei Stukov (Ghost)",
    "Map Revealer",
    "Gerard DuGalle (Ghost)",
    "Zerg Lurker",
    "Infested Duran",
    "Disruption Field",
    "Terran Command Center",
    "Terran Comsat Station",
    "Terran Nuclear Silo",
    "Terran Supply Depot",
    "Terran Refinery",
    "Terran Barracks",
    "Terran Academy",
    "Terran Factory",
    "Terran Starport",
    "Terran Control Tower",
    "Terran Science Facility",
    "Terran Covert Ops",
    "Terran Physics Lab",
    "Unused Terran Bldg type   1",
    "Terran Machine Shop",
    "Unused Terran Bldg type   2",
    "Terran Engineering Bay",
    "Terran Armory",
    "Terran Missile Turret",
    "Terran Bunker",
    "Norad II (Crashed Battlecruiser)",
    "Ion Cannon",
    "Uraj Crystal",
    "Khalis Crystal",
    "Infested Command Center",
    "Zerg Hatchery",
    "Zerg Lair",
    "Zerg Hive",
    "Zerg Nydus Canal",
    "Zerg Hydralisk Den",
    "Zerg Defiler Mound",
    "Zerg Greater Spire",
    "Zerg Queen's Nest",
    "Zerg Evolution Chamber",
    "Zerg Ultralisk Cavern",
    "Zerg Spire",
    "Zerg Spawning Pool",
    "Zerg Creep Colony",
    "Zerg Spore Colony",
    "Unused Zerg Bldg",
    "Zerg Sunken Colony",
    "Zerg Overmind (With Shell)",
    "Zerg Overmind",
    "Zerg Extractor",
    "Mature Crysalis",
    "Zerg Cerebrate",
    "Zerg Cerebrate Daggoth",
    "Unused Zerg Building2",
    "Protoss Nexus",
    "Protoss Robotics Facility",
    "Protoss Pylon",
    "Protoss Unused type   1",
    "Unused Protoss Building1",
    "Protoss Observatory",
    "Protoss Gateway",
    "Protoss Unused type   2",
    "Protoss Photon Cannon",
    "Protoss Citadel of Adun",
    "Protoss Cybernetics Core",
    "Protoss Templar Archives",
    "Protoss Forge",
    "Protoss Stargate",
    "Stasis Cell/Prison",
    "Protoss Fleet Beacon",
    "Protoss Arbiter Tribunal",
    "Protoss Robotics Support Bay",
    "Protoss Shield Battery",
    "Khaydarin Crystal Formation",
    "Protoss Temple",
    "Xel'Naga Temple",
    "Mineral Field (Type 1)",
    "Mineral Field (Type 2)",
    "Mineral Field (Type 3)",
    "Cave",
    "Cave-in",
    "Cantina",
    "Mining Platform",
    "Independent Command Center",
    "Independent Starport",
    "Jump Gate",
    "Ruins",
    "Kyadarin Crystal Formation",
    "Vespene Geyser",
    "Warp Gate",
    "Psi Disrupter",
    "Zerg Marker",
    "Terran Marker",
    "Protoss Marker",
    "Zerg Beacon",
    "Terran Beacon",
    "Protoss Beacon",
    "Zerg Flag Beacon",
    "Terran Flag Beacon",
    "Protoss Flag Beacon",
    "Power Generator",
    "Overmind Cocoon",
    "Dark Swarm",
    "Floor Missile Trap",
    "Floor Hatch (UNUSED)",
    "Left Upper Level Door",
    "Right Upper Level Door",
    "Left Pit Door",
    "Right Pit Door",
    "Floor Gun Trap",
    "Left Wall Missile Trap",
    "Left Wall Flame Trap",
    "Right Wall Missile Trap",
    "Right Wall Flame Trap",
    "Start Location",
    "Flag",
    "Young Chrysalis",
    "Psi Emitter",
    "Data Disc",
    "Khaydarin Crystal",
    "Mineral Chunk (Type 1)",
    "Mineral Chunk (Type 2)",
    "Vespene Orb (Protoss Type 1)",
    "Vespene Orb (Protoss Type 2)",
    "Vespene Sac (Zerg Type 1)",
    "Vespene Sac (Zerg Type 2)",
    "Vespene Tank (Terran Type 1)",
    "Vespene Tank (Terran Type 2)",
    "Unused unit 228",
    "Any unit",
    "Men",
    "Buildings",
    "Factories"
};
        public string GetEditorUnitName(int index)
        {            
            return DefUnitDict[index];
        }









        public string GetCodeName(Codetype codetype, int index)
        {
            int label;
            switch (codetype)
            {
                case Codetype.Unit:
                    if (index >= 228)
                    {
                        string[] addstring = { "없음", "유닛과 건물", "유닛", "건물", "생산건물" };
                        return addstring[index - 228];
                    }
                    string d = UNIx.STRING[index].String;
                    string org = UseMapEditor.Global.WindowTool.GetStat_txt(index);
                    if (UNIx.STRING[index].IsLoaded)
                    {
                        return d + "\n" + org;
                    }

                    return org;
                case Codetype.Upgrade:
                    label = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.upgrades, "Label", index).Data - 1;


                    return UseMapEditor.Global.WindowTool.GetStat_txt(label);
                case Codetype.Tech:
                    label = (int)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.techdata, "Label", index).Data - 1;


                    return UseMapEditor.Global.WindowTool.GetStat_txt(label);
                case Codetype.Sprite:
                    label = (ushort)UseMapEditor.Global.WindowTool.scdata.datFile.Values(DatFile.DatFiles.sprites, "Image File", index).Data;

                    return UseMapEditor.Global.WindowTool.imagename[label];
            }
            return "???";
        }

    }
}
