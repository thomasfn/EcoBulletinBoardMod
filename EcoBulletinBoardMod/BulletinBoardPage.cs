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
            => BulletinBoardData.Obj.BulletinChannels.All<BulletinChannel>().Select(channel => new EcopediaPageReference(PAGE_ICON, CATEGORY_TITLE, channel.Name, Localizer.DoStr(channel.Name)));

        public string GetEcopediaData(Player player, EcopediaPage page)
        {
            var allBulletins = BulletinBoardData.Obj.Bulletins.All<Bulletin>()
                .Where(bulletin => bulletin.IsPublished && bulletin.Channel?.Name == page.Name)
                .OrderByDescending(bulletin => bulletin.CreationTime);
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