using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Eco.Mods.BulletinBoard
{
    using Shared.Localization;
    using Shared.Utils;

    using Gameplay.Players;
    using Gameplay.EcopediaRoot;

    public class BulletinBoardPageSettings : IEcopediaGeneratedData
    {
        public const string PAGE_ICON = "ContractBoardComponent";
        public const string PAGE_TITLE = "Bulletin Board";
        public const string CATEGORY_TITLE = "Bulletins";

        public IEnumerable<EcopediaPageReference> PagesWeSupplyDataFor()
            => BulletinBoardPlugin.Obj.Config.Channels.Select(channel => new EcopediaPageReference(PAGE_ICON, CATEGORY_TITLE, channel.ChannelName, Localizer.DoStr(channel.ChannelName)));

        public string GetEcopediaData(Player player, EcopediaPage page)
        {
            var allBulletins = BulletinBoardData.Obj.Bulletins.All<Bulletin>()
                .Where(bulletin => bulletin.Channel == page.Name);
            if (!allBulletins.Any())
            {
                return "No bulletins have been posted!";
            }
            var sb = new StringBuilder();
            foreach (var bulletin in allBulletins)
            {
                sb.AppendLine(bulletin.Description());
            }
            return sb.ToString();
        }
    }
}