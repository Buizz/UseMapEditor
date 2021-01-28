function Action(Type, ...)
    return {type = "Action", Type, ...}
end


function Victory()
    return Action(1)
end


function Defeat()
    return Action(2)
end


function PreserveTrigger()
    return Action(3)
end


function Wait(Time)
    return Action(4, Time)
end


function PauseGame()
    return Action(5)
end


function UnpauseGame()
    return Action(6)
end


function Transmission(Unit, Where, WAVName, TimeModifier, Time, Text, AlwaysDisplay)
    if AlwaysDisplay == nil then
        AlwaysDisplay = 4
    end

    Unit = ParseUnit(Unit)
    Where = ParseLocation(Where)
    WAVName = ParseString(WAVName)
    TimeModifier = ParseModifier(TimeModifier)
    Text = ParseString(Text)
    return Action(7, Unit, Where, WAVName, TimeModifier, Time, Text, AlwaysDisplay)
end

-- Location Text    Wav TotDuration 0   DurationMod UnitType    7   NumericMod  20

function PlayWAV(WAVName)
    WAVName = ParseString(WAVName)
    return Action(8, WAVName)
end


function DisplayText(Text, AlwaysDisplay)
    if AlwaysDisplay == nil then AlwaysDisplay = 4 end
    Text = ParseString(Text)
    return Action(9, Text, AlwaysDisplay)
end


function CenterView(Where)
    Where = ParseLocation(Where)
    return Action(10, Where)
end


function CreateUnitWithProperties(Count, Unit, Where, Player, Properties)
    Unit = ParseUnit(Unit)
    Where = ParseLocation(Where)
    Player = ParsePlayer(Player)
    Properties = ParseUPRP(Properties)
    return Action(11, Count, Unit, Where, Player, Properties)
end


function SetMissionObjectives(Text)
    Text = ParseString(Text)
    return Action(12, Text)
end


function SetSwitch(Switch, State)
    Switch = ParseSwitchName(Switch)
    State = ParseSwitchAction(State)
    return Action(13, Switch, State)
end


function SetCountdownTimer(TimeModifier, Time)
    TimeModifier = ParseModifier(TimeModifier)
    return Action(14, TimeModifier, Time)
end


function RunAIScript(Script)
    Script = ParseAIScript(Script)
    return Action(15, Script)
end


function RunAIScriptAt(Script, Where)
    Script = ParseAIScript(Script)
    Where = ParseLocation(Where)
    return Action(16, Script, Where)
end


function LeaderBoardControl(Unit, Label)
    Unit = ParseUnit(Unit)
    Label = ParseString(Label)
    return Action(17, Unit, Label)
end


function LeaderBoardControlAt(Unit, Location, Label)
    Unit = ParseUnit(Unit)
    Location = ParseLocation(Location)
    Label = ParseString(Label)
    return Action(18, Unit, Location, Label)
end


function LeaderBoardResources(ResourceType, Label)
    ResourceType = ParseResource(ResourceType)
    Label = ParseString(Label)
    return Action(19, ResourceType, Label)
end


function LeaderBoardKills(Unit, Label)
    Unit = ParseUnit(Unit)
    Label = ParseString(Label)
    return Action(20, Unit, Label)
end


function LeaderBoardScore(ScoreType, Label)
    ScoreType = ParseScore(ScoreType)
    Label = ParseString(Label)
    return Action(21, ScoreType, Label)
end


function KillUnit(Unit, Player)
    Unit = ParseUnit(Unit)
    Player = ParsePlayer(Player)
    return Action(22, Unit, Player)
end


function KillUnitAt(Count, Unit, Where, ForPlayer)
    Count = ParseCount(Count)
    Unit = ParseUnit(Unit)
    Where = ParseLocation(Where)
    ForPlayer = ParsePlayer(ForPlayer)
    return Action(23, Count, Unit, Where, ForPlayer)
end


function RemoveUnit(Unit, Player)
    Unit = ParseUnit(Unit)
    Player = ParsePlayer(Player)
    return Action(24, Unit, Player)
end


function RemoveUnitAt(Count, Unit, Where, ForPlayer)
    Count = ParseCount(Count)
    Unit = ParseUnit(Unit)
    Where = ParseLocation(Where)
    ForPlayer = ParsePlayer(ForPlayer)
    return Action(25, Count, Unit, Where, ForPlayer)
end


function SetResources(Player, Modifier, Amount, ResourceType)
    Player = ParsePlayer(Player)
    Modifier = ParseModifier(Modifier)
    ResourceType = ParseResource(ResourceType)
    return Action(26, Player, Modifier, Amount, ResourceType)
end


function SetScore(Player, Modifier, Amount, ScoreType)
    Player = ParsePlayer(Player)
    Modifier = ParseModifier(Modifier)
    ScoreType = ParseScore(ScoreType)
    return Action(27, Player, Modifier, Amount, ScoreType)
end


function MinimapPing(Where)
    Where = ParseLocation(Where)
    return Action(28, Where)
end


function TalkingPortrait(Unit, Time)
    Unit = ParseUnit(Unit)
    return Action(29, Unit, Time)
end


function MuteUnitSpeech()
    return Action(30)
end


function UnMuteUnitSpeech()
    return Action(31)
end


function LeaderBoardComputerPlayers(State)
    State = ParsePropState(State)
    return Action(32, State)
end


function LeaderBoardGoalControl(Goal, Unit, Label)
    Unit = ParseUnit(Unit)
    Label = ParseString(Label)
    return Action(33, Goal, Unit, Label)
end


function LeaderBoardGoalControlAt(Goal, Unit, Location, Label)
    Unit = ParseUnit(Unit)
    Location = ParseLocation(Location)
    Label = ParseString(Label)
    return Action(34, Goal, Unit, Location, Label)
end


function LeaderBoardGoalResources(Goal, ResourceType, Label)
    ResourceType = ParseResource(ResourceType)
    Label = ParseString(Label)
    return Action(35, Goal, ResourceType, Label)
end


function LeaderBoardGoalKills(Goal, Unit, Label)
    Unit = ParseUnit(Unit)
    Label = ParseString(Label)
    return Action(36, Goal, Unit, Label)
end


function LeaderBoardGoalScore(Goal, ScoreType, Label)
    ScoreType = ParseScore(ScoreType)
    Label = ParseString(Label)
    return Action(37, Goal, ScoreType, Label)
end


function MoveLocation(Location, OnUnit, Owner, DestLocation)
    Location = ParseLocation(Location)
    OnUnit = ParseUnit(OnUnit)
    Owner = ParsePlayer(Owner)
    DestLocation = ParseLocation(DestLocation)
    return Action(38, Location, OnUnit, Owner, DestLocation)
end


function MoveUnit(Count, UnitType, Owner, StartLocation, DestLocation)
    Count = ParseCount(Count)
    UnitType = ParseUnit(UnitType)
    Owner = ParsePlayer(Owner)
    StartLocation = ParseLocation(StartLocation)
    DestLocation = ParseLocation(DestLocation)
    return Action(39, Count, UnitType, Owner, StartLocation, DestLocation)
end


function LeaderBoardGreed(Goal)
    return Action(40, Goal)
end


function SetNextScenario(ScenarioName)
    ScenarioName = ParseString(ScenarioName)
    return Action(41, ScenarioName)
end


function SetDoodadState(State, Unit, Owner, Where)
    State = ParsePropState(State)
    Unit = ParseUnit(Unit)
    Owner = ParsePlayer(Owner)
    Where = ParseLocation(Where)
    return Action(42, State, Unit, Owner, Where)
end


function SetInvincibility(State, Unit, Owner, Where)
    State = ParsePropState(State)
    Unit = ParseUnit(Unit)
    Owner = ParsePlayer(Owner)
    Where = ParseLocation(Where)
    return Action(43, State, Unit, Owner, Where)
end


function CreateUnit(Number, Unit, Where, ForPlayer)
    Unit = ParseUnit(Unit)
    Where = ParseLocation(Where)
    ForPlayer = ParsePlayer(ForPlayer)
    return Action(44, Number, Unit, Where, ForPlayer)
end


function SetDeaths(Player, Modifier, Number, Unit)
    Player = ParsePlayer(Player)
    Modifier = ParseModifier(Modifier)
    Unit = ParseUnit(Unit)
    return Action(45, Player, Modifier, Number, Unit, 0, 0)
end


function SetDeathsX(Player, Modifier, Number, Unit, Mask)
    Player = ParsePlayer(Player)
    Modifier = ParseModifier(Modifier)
    Unit = ParseUnit(Unit)
    return Action(45, Player, Modifier, Number, Unit, Mask, 17235)
end


function SetMemory(Memory, Modifier, Number)
    Modifier = ParseModifier(Modifier)
    return Action(58, Memory, Modifier, Number, 0, 0)
end


function SetMemoryX(Player, Modifier, Number, Mask)
    Modifier = ParseModifier(Modifier)
    return Action(58, Memory, Modifier, Number, Mask, 17235)
end


function Order(Unit, Owner, StartLocation, OrderType, DestLocation)
    Unit = ParseUnit(Unit)
    Owner = ParsePlayer(Owner)
    StartLocation = ParseLocation(StartLocation)
    OrderType = ParseOrder(OrderType)
    DestLocation = ParseLocation(DestLocation)
    return Action(46, Unit, Owner, StartLocation, OrderType, DestLocation)
end


function Comment(Text)
    Text = ParseString(Text)
    return Action(47, Text)
end


function GiveUnits(Count, Unit, Owner, Where, NewOwner)
    Count = ParseCount(Count)
    Unit = ParseUnit(Unit)
    Owner = ParsePlayer(Owner)
    Where = ParseLocation(Where)
    NewOwner = ParsePlayer(NewOwner)
    return Action(48, Count, Unit, Owner, Where, NewOwner)
end


function ModifyUnitHitPoints(Count, Unit, Owner, Where, Percent)
    Count = ParseCount(Count)
    Unit = ParseUnit(Unit)
    Owner = ParsePlayer(Owner)
    Where = ParseLocation(Where)
    return Action(49, Count, Unit, Owner, Where, Percent)
end


function ModifyUnitEnergy(Count, Unit, Owner, Where, Percent)
    Count = ParseCount(Count)
    Unit = ParseUnit(Unit)
    Owner = ParsePlayer(Owner)
    Where = ParseLocation(Where)
    return Action(50, Count, Unit, Owner, Where, Percent)
end


function ModifyUnitShields(Count, Unit, Owner, Where, Percent)
    Count = ParseCount(Count)
    Unit = ParseUnit(Unit)
    Owner = ParsePlayer(Owner)
    Where = ParseLocation(Where)
    return Action(51, Count, Unit, Owner, Where, Percent)
end


function ModifyUnitResourceAmount(Count, Owner, Where, NewValue)
    Count = ParseCount(Count)
    Owner = ParsePlayer(Owner)
    Where = ParseLocation(Where)
    return Action(52, Count, Owner, Where, NewValue)
end


function ModifyUnitHangarCount(Add, Count, Unit, Owner, Where)
    Count = ParseCount(Count)
    Unit = ParseUnit(Unit)
    Owner = ParsePlayer(Owner)
    Where = ParseLocation(Where)
    return Action(53, Add, Count, Unit, Owner, Where)
end


function PauseTimer()
    return Action(54)
end


function UnpauseTimer()
    return Action(55)
end


function Draw()
    return Action(56)
end


function SetAllianceStatus(Player, Status)
    Player = ParsePlayer(Player)
    Status = ParseAllyStatus(Status)
    return Action(57, Player, Status)
end

