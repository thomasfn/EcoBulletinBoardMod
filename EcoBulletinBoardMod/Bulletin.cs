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
        [Serialized, SyncToView] public bool IsPublished { get; set; } 

        [Serialized, SyncToView] public BulletinChannel Channel { get; set; }

        [Serialized, SyncToView] public string Text { get; set; }

        [Tooltip(100)]
        public override LocString Description()
        {
            var sb = new StringBuilder();
            if (IsPublished)
            {
                
                sb.AppendLine($"Published to {Channel?.UILink()} at {TimeFormatter.FormatDateLong(CreationTime)} ({TimeFormatter.FormatTimeSince(CreationTime, WorldTime.Seconds)} ago)");
            }
            else
            {
                sb.AppendLine($"Unpublished");
            }
            sb.AppendLine(Text);
            return sb.ToStringLoc();
        }

        public override LocString UILinkContent()
            => TextLoc.ItemIcon("AuthComponent", Localizer.DoStr(this.Name));

        public LocString FormatForBoard()
        {
            var sb = new StringBuilder();
            if (IsPublished)
            {

                sb.AppendLine($"{this.UILink()} published by {Creator?.UILink() ?? "nobody"} at {TimeFormatter.FormatDateLong(CreationTime)} ({TimeFormatter.FormatTimeSince(CreationTime, WorldTime.Seconds)} ago)");
            }
            else
            {
                sb.AppendLine($"{this.UILink()} (unpublished) by {Creator?.UILink() ?? "nobody"}");
            }
            sb.AppendLine(Text);
            return sb.ToStringLoc();
        }

        public void Publish()
        {
            if (IsPublished || Channel == null) { return; }
            IsPublished = true;
            CreationTime = WorldTime.Seconds;
            SaveInRegistrar();
            ChatManager.ServerMessageToAlias(
                new LocString($"{Creator?.UILink() ?? "Someone"} has published {this.UILink()} to {Channel.UILink()}, check it out on the Ecopedia!"),
                DemographicManager.Obj.Get(SpecialDemographics.Everyone),
                category: MessageCategory.Mail
            );
        }
    }
}
