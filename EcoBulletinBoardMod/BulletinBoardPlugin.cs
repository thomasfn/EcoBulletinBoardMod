using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Eco.Mods.BulletinBoard
{
    using Core.Plugins.Interfaces;
    using Core.Utils;
    using Core.Systems;
    using Core.Serialization;
    using Core.Plugins;
    using Core.Controller;

    using Shared.Localization;
    using Shared.Utils;
    using Shared.Serialization;

    using Gameplay.Players;
    using Gameplay.Systems;
    using Gameplay.Systems.Chat;
    using Gameplay.Systems.TextLinks;

    [Serialized]
    public class BulletinBoardData : Singleton<BulletinBoardData>, IStorage
    {
        public IPersistent StorageHandle { get; set; }

        [Serialized] public Registrar BulletinChannels = new Registrar();

        [Serialized] public Registrar Bulletins = new Registrar();

        public void InitializeRegistrars()
        {
            this.BulletinChannels.Init(Localizer.DoStr("Bulletin Channels"), true, typeof(BulletinChannel), BulletinBoardPlugin.Obj, Localizer.DoStr("Bulletin Channels"));
            this.Bulletins.Init(Localizer.DoStr("Bulletins"), true, typeof(Bulletin), BulletinBoardPlugin.Obj, Localizer.DoStr("Bulletins"));
        }

        public void Initialize()
        {
            
        }
    }

    [Localized]
    public class BulletinBoardConfig
    {
        
    }

    [Localized, LocDisplayName(nameof(BulletinBoardPlugin)), Priority(PriorityAttribute.High)]
    public class BulletinBoardPlugin : Singleton<BulletinBoardPlugin>, IModKitPlugin, IInitializablePlugin, IChatCommandHandler, ISaveablePlugin, IContainsRegistrars, IConfigurablePlugin
    {
        [NotNull] private readonly BulletinBoardData data;

        public IPluginConfig PluginConfig => config;

        private PluginConfig<BulletinBoardConfig> config;
        public BulletinBoardConfig Config => config.Config;

        public BulletinBoardPlugin()
        {
            data = StorageManager.LoadOrCreate<BulletinBoardData>("BulletinBoard");
            config = new PluginConfig<BulletinBoardConfig>("BulletinBoard");
        }

        public void Initialize(TimedTask timer) => data.Initialize();
        public void InitializeRegistrars(TimedTask timer) => data.InitializeRegistrars();
        public string GetDisplayText() => string.Empty;
        public string GetStatus() => string.Empty;
        public override string ToString() => Localizer.DoStr("BulletinBoard");
        public void SaveAll() => StorageManager.Obj.MarkDirty(data);

        public object GetEditObject() => Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

        public void OnEditObjectChanged(object o, string param)
        {
            this.SaveConfig();
        }

        #region Chat Commands

        [ChatCommand("Bulletins", ChatAuthorizationLevel.User)]
        public static void Bulletins() { }

        [ChatSubCommand("Bulletins", "Creates a new bulletin.", ChatAuthorizationLevel.User)]
        public static void New(User user)
        {
            _ = CreateNewBulletin(user);
        }

        private static async Task CreateNewBulletin(User user)
        {
            // Grab all channels and see if the user is able to publish to any of them
            var allChannels = BulletinBoardData.Obj.BulletinChannels
                .All<BulletinChannel>();
            var allowedChannels = allChannels.Where(channel => channel.IsAllowedToPublish(user)).ToArray();
            if (allowedChannels.Length == 0)
            {
                user.Msg(Localizer.Do($"You are not allowed to publish any bulletins!"));
                return;
            }

            // Give them a choice of channels
            var optionIndex = await user.Player.OptionBox(Localizer.DoStr("Choose channel to publish bulletin to:"), new List<string>(allowedChannels.Select(channel => channel.Name)));
            if (optionIndex < 0 || optionIndex >= allowedChannels.Length) { return; }
            var selectedChannel = allowedChannels[optionIndex];
            if (selectedChannel.IsDestroyed) { return; }
            if (!selectedChannel.IsAllowedToPublish(user))
            {
                user.Msg(Localizer.Do($"You are not allowed to publish bulletins to {selectedChannel.UILink()}!"));
            }

            // See if they already have an "open" bulletin, e.g. one that they started writing earlier but didn't finish
            var bulletin = BulletinBoardData.Obj.Bulletins
                .All<Bulletin>()
                .SingleOrDefault(bulletin => bulletin.Creator == user && !bulletin.IsPublished);
            if (bulletin == null)
            {
                bulletin = BulletinBoardData.Obj.Bulletins.Add() as Bulletin;
                bulletin.Creator = user;
            }
            bulletin.Channel = selectedChannel;

            // Begin edit operation
            var viewEditor = ViewEditor.Edit(user, bulletin, Obj, OnUserFinishedEditingBulletin, $"Publish", Color.Green);
        }

        private static void OnUserFinishedEditingBulletin(IController controller)
        {
            var bulletin = controller as Bulletin;
            if (bulletin == null) { return; }
            if (bulletin.IsPublished) { return; }
            bulletin.Publish();
            bulletin.Creator?.Msg(new LocString($"Published {bulletin.UILink()}."));
        }

        #endregion
    }
}