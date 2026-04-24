using BaseLib.Abstracts;
using Forest_Sr.BardCode.Character;
using Godot;
using MegaCrit.Sts2.Core.Models.Relics;
using System;

namespace Forest_Sr.BardCode.Character;

public partial class BardRelicPool : CustomRelicPoolModel
{

    public override string EnergyColorName => Bard.CharacterId;

    public override Color LabOutlineColor => Bard.Color;
}
