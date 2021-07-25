using System;
using System.Text;
using System.Linq;

namespace Eco.Mods.BulletinBoard
{
    using Core.Controller;

    using Gameplay.Utils;
    using Gameplay.Systems.Tooltip;
    using Gameplay.Players;
    using Gameplay.Aliases;
    using Gameplay.EcopediaRoot;

    using Shared.Serialization;
    using Shared.Localization;
    using Shared.Utils;
    using Shared.Networking;

    [Serialized]
    public class BulletinChannel : SimpleEntry
    {
        [Eco, AllowNull] public IAlias AllowedToPublish { get; set; }

        [Tooltip(100)]
        public override LocString Description()
             => Localizer.Do($"Contains {BulletinBoardData.Obj.Bulletins.All().Cast<Bulletin>().Where(bulletin => bulletin.Channel == this).Count()} bulletins");

        public override LocString UILinkContent()
            => TextLoc.ItemIcon("ContractBoardComponent", Localizer.DoStr(this.Name));

        public override LocString LinkClickedTooltipContent(TooltipContext context)
            => Localizer.DoStr("Click to view");

        public override void OnLinkClicked(TooltipContext context)
        {
            // TODO: Figure out how to direct them to the channel's page in Ecopedia
        }

        public override void MarkDirty()
        {
            base.MarkDirty();
            EcopediaManager.Build();
        }

        public bool IsAllowedToPublish(User user)
        {
            if (AllowedToPublish == null) { return false; }
            return AllowedToPublish.Contains(user);
        }
    }
}
