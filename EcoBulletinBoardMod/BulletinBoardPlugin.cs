using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Eco.Mods.BulletinBoard
{
    using Core.Plugins.Interfaces;
    using Core.Utils;
    using Core.Systems;
    using Core.IoC;
    using Core.Serialization;
    using Core.Plugins;

    using Shared.Localization;
    using Shared.Math;
    using Shared.Utils;
    using Shared.Serialization;

    using Gameplay.Players;
    using Gameplay.Systems.Chat;
    using Gameplay.Systems.TextLinks;
    using Gameplay.Objects;
    using Gameplay.Components;
    using Gameplay.Economy;
    using Gameplay.EcopediaRoot;
    using Gameplay.Civics.Demographics;
    using Eco.Shared.Services;
    using System.ComponentModel;
    using Eco.Core.Controller;

    [Serialized]
    public class BulletinBoardData : Singleton<BulletinBoardData>, IStorage
    {
        public IPersistent StorageHandle { get; set; }

        [Serialized] public Registrar Bulletins = new Registrar();

        public void InitializeRegistrars()
        {
            this.Bulletins.Init(Localizer.DoStr("Bulletins"), true, typeof(Bulletin), BulletinBoardPlugin.Obj, Localizer.DoStr("Current Elections"));
        }

        public void Initialize()
        {
            
        }
    }

    [Localized]
    public class BulletinBoardConfig
    {
        [Localized, TypeConverter(typeof(ExpandableObjectConverter))]
        public class ChannelSettings
        {
            [LocDescription("The name of the bulletin board channel.")] public string ChannelName { get; set; } = "Default";
            public override string ToString() => string.Empty; // Do not use type's name as a value of this dropdown.
        }

        [LocDescription("Settings for each bulletin board channel.")]
        public IEnumerable<ChannelSettings> Channels { get; set; } = new ChannelSettings[] { new ChannelSettings() };
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
            switch (param) // Parse the name and trigger binded event.
            {
                case nameof(BulletinBoardConfig.Channels):
                    EcopediaManager.Build();
                    break;
            }
            this.SaveConfig();
        }

        #region Chat Commands

        [ChatCommand("Bulletins", ChatAuthorizationLevel.Admin)]
        public static void Bulletins() { }

        [ChatSubCommand("Bulletins", "Publishes a bulletin.", ChatAuthorizationLevel.Admin)]
        public static void Publish(User user, string text)
        {
            // TODO: Check if they have permissions to publish
            var bulletin = Bulletin.Publish(user, "Default", text);
            user.Msg(new LocString($"Published {bulletin.UILink()}."));
        }

        #endregion
    }
}