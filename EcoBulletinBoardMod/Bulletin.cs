using System;
using System.Text;

namespace Eco.Mods.BulletinBoard
{
    using Core.Controller;

    using Gameplay.Utils;
    using Gameplay.Systems.Tooltip;
    using Gameplay.Systems.Chat;
    using Gameplay.Systems.TextLinks;
    using Gameplay.Civics.Demographics;

    using Shared.Serialization;
    using Shared.Localization;
    using Shared.Utils;
    using Shared.Services;
    using Shared.Networking;

    using Simulation.Time;

    [Serialized]
    public class Bulletin : SimpleEntry
    {
        [Serialized] public bool IsPublished { get; set; } = false;

        [Serialized] public BulletinChannel Channel { get; set; } = null;

        [Eco] public string Text { get; set; } = "";

        [SyncToView]
        public override LocString Description()
        {
            var sb = new StringBuilder();
            if (IsPublished)
            {
                
                sb.AppendLine($"{UILinkContent()} published by {Creator?.UILink() ?? "nobody"} to {Channel?.UILink()} at {TimeFormatter.FormatDateLong(CreationTime)} ({TimeFormatter.FormatTimeSince(CreationTime, WorldTime.Seconds)} ago)");
            }
            else
            {
                sb.AppendLine($"{UILinkContent()} (draft) by {Creator?.UILink() ?? "nobody"}");
            }
            sb.AppendLine(Text);
            return sb.ToStringLoc();
        }

        public void Publish()
        {
            if (IsPublished) { return; }
            IsPublished = true;
            CreationTime = WorldTime.Seconds;
            SaveInRegistrar();
            ChatManager.ServerMessageToAlias(
                new LocString($"{Creator.UILink()} has published a new bulletin under '{Channel?.UILink()}', check it out on the Ecopedia!"),
                DemographicManager.Obj.Get(SpecialDemographics.Everyone),
                category: MessageCategory.Mail
            );
        }
    }
}
