using System;
using System.Text;

namespace Eco.Mods.BulletinBoard
{
    using Core.Controller;

    using Gameplay.Utils;
    using Gameplay.Components;
    using Gameplay.Systems.Tooltip;
    using Gameplay.Players;
    using Gameplay.Systems.Chat;
    using Gameplay.Civics.Demographics;

    using Shared.Serialization;
    using Shared.Localization;
    using Shared.Utils;
    using Shared.Services;

    using Simulation.Time;

    [Serialized]
    public class Bulletin : SimpleEntry
    {
        [Serialized, SyncToView, TooltipChildren] public string Channel { get; set; } = "Default";

        [Serialized, SyncToView, TooltipChildren] public TextItemData TextData { get; set; } = new TextItemData();

        [SyncToView]
        public override LocString Description()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{UILinkContent()} published by {Creator?.UILinkContent() ?? "nobody"} at {TimeFormatter.FormatDateLong(CreationTime)} ({TimeFormatter.FormatTimeSince(CreationTime, WorldTime.Seconds)})");
            sb.AppendLine(TextData.Text);
            return sb.ToStringLoc();
        }

        public static Bulletin Publish(User publisher, string channel, string text)
        {
            var bulletin = BulletinBoardData.Obj.Bulletins.Add() as Bulletin;
            bulletin.Creator = publisher;
            bulletin.Channel = channel;
            bulletin.TextData.Text = text;
            bulletin.NotifyPropertyChanged(bulletin, nameof(bulletin.TextData));
            bulletin.SaveInRegistrar();
            ChatManager.ServerMessageToAlias(
                new LocString($"{publisher.UILinkContent()} has published a new bulletin under '{channel}', check it out on the Ecopedia!"),
                DemographicManager.Obj.Get(SpecialDemographics.Everyone),
                category: MessageCategory.Mail
            );
            return bulletin;
        }
    }
}
