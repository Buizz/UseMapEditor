function Condition(Type, ...)
    return {Type, ...}
end


function CountdownTimer(Comparison, Time)
    Comparison = ParseComparison(Comparison)
    return Condition(1, Comparison, Time)
end


function Command(Player, Comparison, Number, Unit)
    Player = ParsePlayer(Player)
    Comparison = ParseComparison(Comparison)
    Unit = ParseUnit(Unit)
    return Condition(2, Player, Comparison, Number, Unit)
end


function Bring(Player, Comparison, Number, Unit, Location)
    Player = ParsePlayer(Player)
    Comparison = ParseComparison(Comparison)
    Unit = ParseUnit(Unit)
    Location = ParseLocation(Location)
    return Condition(3, Player, Comparison, Number, Unit, Location)
end


function Accumulate(Player, Comparison, Number, ResourceType)
    Player = ParsePlayer(Player)
    Comparison = ParseComparison(Comparison)
    ResourceType = ParseResource(ResourceType)
    return Condition(4, Player, Comparison, Number, ResourceType)
end


function Kills(Player, Comparison, Number, Unit)
    Player = ParsePlayer(Player)
    Comparison = ParseComparison(Comparison)
    Unit = ParseUnit(Unit)
    return Condition(5, Player, Comparison, Number, Unit)
end


function CommandMost(Unit)
    Unit = ParseUnit(Unit)
    return Condition(6, Unit)
end


function CommandMostAt(Unit, Location)
    Unit = ParseUnit(Unit)
    Location = ParseLocation(Location)
    return Condition(7, Unit, Location)
end


function MostKills(Unit)
    Unit = ParseUnit(Unit)
    return Condition(8, Unit)
end


function HighestScore(ScoreType)
    ScoreType = ParseScore(ScoreType)
    return Condition(9, ScoreType)
end


function MostResources(ResourceType)
    ResourceType = ParseResource(ResourceType)
    return Condition(10, ResourceType)
end


function Switch(Switch, State)
    Switch = ParseSwitchName(Switch)
    State = ParseSwitchState(State)
    return Condition(11, Switch, State)
end


function ElapsedTime(Comparison, Time)
    Comparison = ParseComparison(Comparison)
    return Condition(12, Comparison, Time)
end


function Briefing()
    return Condition(13)
end


function Opponents(Player, Comparison, Number)
    Player = ParsePlayer(Player)
    Comparison = ParseComparison(Comparison)
    return Condition(14, Player, Comparison, Number)
end


function Deaths(Player, Comparison, Number, Unit)
    Player = ParsePlayer(Player)
    Comparison = ParseComparison(Comparison)
    Unit = ParseUnit(Unit)
    return Condition(15, Player, Comparison, Number, Unit, 0, 0)
end


function DeathsX(Player, Comparison, Number, Unit, Mask)
    Player = ParsePlayer(Player)
    Comparison = ParseComparison(Comparison)
    Unit = ParseUnit(Unit)
    return Condition(15, Player, Comparison, Number, Unit, Mask, 17235)
end


function Memory(Memory, Comparison, Number)
    Comparison = ParseModifier(Comparison)
    return Condition(24, Memory, Comparison, Number, 0, 0)
end


function MemoryX(Player, Comparison, Number, Mask)
    Comparison = ParseModifier(Comparison)
    return Condition(24, Memory, Comparison, Number, Mask, 17235)
end


function CommandLeast(Unit)
    Unit = ParseUnit(Unit)
    return Condition(16, Unit)
end


function CommandLeastAt(Unit, Location)
    Unit = ParseUnit(Unit)
    Location = ParseLocation(Location)
    return Condition(17, Unit, Location)
end


function LeastKills(Unit)
    Unit = ParseUnit(Unit)
    return Condition(18, Unit)
end


function LowestScore(ScoreType)
    ScoreType = ParseScore(ScoreType)
    return Condition(19, ScoreType)
end


function LeastResources(ResourceType)
    ResourceType = ParseResource(ResourceType)
    return Condition(20, ResourceType)
end


function Score(Player, ScoreType, Comparison, Number)
    Player = ParsePlayer(Player)
    ScoreType = ParseScore(ScoreType)
    Comparison = ParseComparison(Comparison)
    return Condition(21, Player, ScoreType, Comparison, Number)
end


function Always()
    return Condition(22)
end


function Never()
    return Condition(23)
end


