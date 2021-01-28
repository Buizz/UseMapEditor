function Brief(Type, ...)
    return {type = "Briefing", Type, ...}
end


function BriefingWait(Time)
    return Brief(1, Time)
end


function BriefingPlayWAV(WAVName, WavTime)
    WAVName = ParseString(WAVName)
    return Brief(2, WAVName, WavTime)
end


function BriefingTextMessage(Text, Time)
    Text = ParseString(Text)
    return Brief(3, Text, Time)
end


function BriefingMissionObjectives(Text)
    Text = ParseString(Text)
    return Brief(4, Text)
end


function BriefingShowPortrait(Unit, Slot)
    Unit = ParseUnit(Unit)
    return Brief(5, Unit, Slot)
end


function BriefingHidePortrait(Slot)
    return Brief(6, Slot)
end


function BriefingDisplaySpeakingPortrait(Slot, Time)
    return Brief(7, Slot, Time)
end


function BriefingTransmission(Text, Slot, Time, Modifier, Wave, WavTime)
    Text = ParseString(Text)
    Wave = ParseString(Wave)
    Modifier = ParseModifier(Modifier)
    return Brief(8, Text, Slot, Time, Modifier, Wave, WavTime)
end
