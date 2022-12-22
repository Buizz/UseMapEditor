using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.Control;
using static Data.Map.MapData;

namespace UseMapEditor.Task.Events
{
    public class UnitPropertyEvent : TaskEvent
    {
        public struct UnitData
        {
            public ushort _X;
            public ushort _Y;
            public ushort _unitID;
            public byte _player;
            public bool _hpvalid;
            public byte _hitPoints;
            public bool _shvalid;
            public byte _shieldPoints;
            public bool _envalid;
            public byte _energyPoints;
            public bool _resvalid;
            public uint _resoruceAmount;
            public bool _hangarvalid;
            public ushort _hangar;
            public bool _cloakvalid;
            public bool _cloakstate;
            public bool _burrowvalid;
            public bool _burrowstate;
            public bool _tranvalid;
            public bool _buildstate;
            public bool _hallvalid;
            public bool _hallstate;
            public bool _invinvalid;
            public bool _invincstate;

            public UnitData(CUNIT cUNIT)
            {
                _X = cUNIT.X;
                _Y = cUNIT.Y;
                _unitID = cUNIT.unitID;
                _player = cUNIT.player;
                _hpvalid = cUNIT.hpvalid;
                _hitPoints = cUNIT.hitPoints;
                _shvalid = cUNIT.shvalid;
                _shieldPoints = cUNIT.shieldPoints;
                _envalid = cUNIT.envalid;
                _energyPoints = cUNIT.energyPoints;
                _resvalid = cUNIT.resvalid;
                _resoruceAmount = cUNIT.resoruceAmount;
                _hangarvalid = cUNIT.hangarvalid;
                _hangar = cUNIT.hangar;
                _cloakvalid = cUNIT.cloakvalid;
                _cloakstate = cUNIT.cloakstate;
                _burrowvalid = cUNIT.burrowvalid;
                _burrowstate = cUNIT.burrowstate;
                _tranvalid = cUNIT.tranvalid;
                _buildstate = cUNIT.buildstate;
                _hallvalid = cUNIT.hallvalid;
                _hallstate = cUNIT.hallstate;
                _invinvalid = cUNIT.invinvalid;
                _invincstate = cUNIT.invincstate;
            }

            public void SetData(CUNIT cUNIT)
            {
                cUNIT.X = _X;
                cUNIT.Y = _Y;
                cUNIT.unitID = _unitID;
                cUNIT.player = _player;
                cUNIT.hpvalid = _hpvalid;
                cUNIT.hitPoints = _hitPoints;
                cUNIT.shvalid = _shvalid;
                cUNIT.shieldPoints = _shieldPoints;
                cUNIT.envalid = _envalid;
                cUNIT.energyPoints = _energyPoints;
                cUNIT.resvalid = _resvalid;
                cUNIT.resoruceAmount = _resoruceAmount;
                cUNIT.hangarvalid = _hangarvalid;
                cUNIT.hangar = _hangar;
                cUNIT.cloakvalid = _cloakvalid;
                cUNIT.cloakstate = _cloakstate;
                cUNIT.burrowvalid = _burrowvalid;
                cUNIT.burrowstate = _burrowstate;
                cUNIT.tranvalid = _tranvalid;
                cUNIT.buildstate = _buildstate;
                cUNIT.hallvalid = _hallvalid;
                cUNIT.hallstate = _hallstate;
                cUNIT.invinvalid = _invinvalid;
                cUNIT.invincstate = _invincstate;
            }
        }



        MapEditor mapEditor;
        private CUNIT cUNIT;
        private UnitData NewCUNIT;
        private UnitData OldCUNIT;

        public UnitPropertyEvent(MapEditor mapEditor, CUNIT cUNIT ,UnitData NewCUNIT, UnitData OldCUNIT)
        {
            this.mapEditor = mapEditor;
            this.cUNIT = cUNIT;

            this.NewCUNIT = NewCUNIT;
            this.OldCUNIT = OldCUNIT;
        }


        public override void Redo()
        {
            //NEW데이터로 덮기
            NewCUNIT.SetData(cUNIT);
            mapEditor.MinimapRefresh();
        }

        public override void Undo()
        {
            //OLD데이터로 덮기
            OldCUNIT.SetData(cUNIT);
            mapEditor.MinimapRefresh();
        }

        public override void Complete()
        {
        }
    }
}
