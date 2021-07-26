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
    using Shared.Networking;

    using Gameplay.Players;
    using Gameplay.Systems;
    using Gameplay.Systems.Chat;
    using Gameplay.Systems.TextLinks;
    using Gameplay.Objects;
    using Gameplay.Components;

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
        [LocDescription("Maximum number of characters for a bulletin message.")]
        public int MaxBulletinMessageLength { get; set; } = 1000;
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

        [ChatSubCommand("Bulletins", "Publishes a bulletin.", ChatAuthorizationLevel.User)]
        public static void Publish(User user, INetObject target)
        {
            var worldObject = target as WorldObject;
            if (worldObject == null)
            {
                user.Msg(Localizer.DoStr("Aim at a sign to publish the message on that sign."));
                return;
            }
            var customTextComponent = worldObject.GetComponent<CustomTextComponent>();
            if (customTextComponent == null)
            {
                user.Msg(Localizer.DoStr("Aim at a sign to publish the message on that sign."));
                return;
            }
            var message = customTextComponent.TextData?.Text;
            if (string.IsNullOrEmpty(message))
            {
                user.Msg(Localizer.DoStr("Message is too short."));
                return;
            }
            if (message.Length > Obj.Config.MaxBulletinMessageLength)
            {
                user.Msg(Localizer.DoStr("Message is too long."));
                return;
            }
            _ = PublishNewBulletin(user, customTextComponent.TextData.Text);
        }

        private static async Task PublishNewBulletin(User user, string message)
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
            var bulletin = BulletinBoardData.Obj.Bulletins.Add() as Bulletin;
            bulletin.Creator = user;
            bulletin.Channel = selectedChannel;
            bulletin.Text = message;
            bulletin.Publish();
            user.Msg(new LocString($"Published {bulletin.UILink()}."));
        }

        #endregion
    }
}