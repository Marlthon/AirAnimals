using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLibs;
using ItemManager;
using ServerSync;
using CreatureManager;
using LocationManager;
using UnityEngine;
using UnityEngine.Serialization;
using Range = CreatureManager.Range;

namespace AirAnimals
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class AirAnimalsPlugin : BaseUnityPlugin
    {
        // Random update
        internal const string ModName = "AirAnimals";
        internal const string ModVersion = "0.2.8";
        internal const string Author = "marlthon";
        private const string ModGUID = Author + "." + ModName;
        private static string ConfigFileName = ModGUID + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;

        private readonly Harmony _harmony = new(ModGUID);

        private static Dictionary<GameObject, GameObject> carnes = new();

        public static readonly ManualLogSource FarmLogger =
            BepInEx.Logging.Logger.CreateLogSource(ModName);


        private static readonly ConfigSync ConfigSync = new(ModGUID)
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        // DEUS SEJA LOUVADO!
        public void Awake()
        {
            HarmonyCore.Instance.Init("AirAnimals");
            HarmonyCore.Instance.Ciano("Marlthon Mods");
            HarmonyCore.Instance.Verde("Download more mods at marlthon.com");

            // Needed for ServerSync to add locking of config toggle
            _serverConfigLocked = config("General", "Force Server Config", true, "Force Server Config");
            _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

            #region Harpy Eagle
            Creature AA_HarpyEagle = new Creature("airanimals", "AA_HarpyEagle")
                .ConfigureBiome(Heightmap.Biome.Meadows)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(2, 1000))
                .ConfigureForestSpawn(Forest.No)
                .ConfigureRequiredWeather(Weather.ClearSkies | Weather.MeadowsClearSkies | Weather.LightRain)
                .ConfigureSpawnChance(10)
                .ConfigureGroupSize(new Range(1, 1))
                .ConfigureSpawnInterval(1200)
                .ConfigureSpawnTime(SpawnTime.Day)
                .ConfigureMaximum(2)
                .EnableSpawning(true)
                .EnableStars(true);

            AA_HarpyEagle.Localize()
                .Portuguese_Brazilian("Harpia")
                .English("Harpy Eagle");

            AA_HarpyEagle.ConfigureDrops();
            AA_HarpyEagle.Drops["Feathers"].Amount = new Range(6, 8);
            AA_HarpyEagle.Drops["Feathers"].DropChance = 100f;
            AA_HarpyEagle.Drops["Entrails"].Amount = new Range(1, 1);
            AA_HarpyEagle.Drops["Entrails"].DropChance = 100f;
            #endregion

            #region Swan
            Creature AA_Swan = new Creature("airanimals", "AA_Swan")
                .ConfigureBiome(Heightmap.Biome.Meadows)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(-3, -1))
                .ConfigureRequiredOceanDepth(new Range(0, 0))
                .ConfigureForestSpawn(Forest.Both)
                .ConfigureRequiredWeather(Weather.ClearSkies | Weather.LightRain | Weather.ClearThunderStorm | Weather.MeadowsClearSkies)
                .ConfigureSpawnChance(10)
                .ConfigureGroupSize(new Range(1, 2))
                .ConfigureSpawnInterval(1000)
                .ConfigureSpawnTime(SpawnTime.Day)
                .ConfigureMaximum(2)
                .EnableSpawning(true)
                .EnableStars(true);

            AA_Swan.Localize()
                .Portuguese_Brazilian("Cisne")
                .English("Swan");

            AA_Swan.ConfigureDrops();
            AA_Swan.Drops["Feathers"].Amount = new Range(3, 4);
            AA_Swan.Drops["Feathers"].DropChance = 100f;
            #endregion

            #region Wild Duck
            Creature AA_WildDuck = new Creature("airanimals", "AA_WildDuck")
                .ConfigureBiome(Heightmap.Biome.Meadows | Heightmap.Biome.BlackForest | Heightmap.Biome.Plains)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(-3, -1))
                .ConfigureRequiredOceanDepth(new Range(0, 0))
                .ConfigureForestSpawn(Forest.Both)
                .ConfigureRequiredWeather(Weather.ClearSkies | Weather.LightRain | Weather.ClearThunderStorm | Weather.MeadowsClearSkies)
                .ConfigureSpawnChance(10)
                .ConfigureGroupSize(new Range(2, 4))
                .ConfigureSpawnInterval(1000)
                .ConfigureSpawnTime(SpawnTime.Day)
                .ConfigureMaximum(4)
                .EnableSpawning(true)
                .EnableStars(true);

            AA_WildDuck.Localize()
                .Portuguese_Brazilian("Marreco")
                .English("Wild Duck");

            AA_WildDuck.ConfigureDrops();
            AA_WildDuck.Drops["Feathers"].Amount = new Range(3, 4);
            AA_WildDuck.Drops["Feathers"].DropChance = 100f;
            #endregion

            #region BroadTail Hummingbird
            Creature AA_BroadTailHummingbird = new Creature("airanimals", "AA_BroadTailHummingbird")
                .ConfigureBiome(Heightmap.Biome.Meadows | Heightmap.Biome.BlackForest | Heightmap.Biome.Plains)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(2, 1000))
                .ConfigureForestSpawn(Forest.Both)
                .ConfigureRequiredWeather(Weather.ClearSkies | Weather.MeadowsClearSkies | Weather.BlackForestFog | Weather.Fog)
                .ConfigureSpawnChance(10)
                .ConfigureGroupSize(new Range(1, 2))
                .ConfigureSpawnInterval(1000)
                .ConfigureSpawnTime(SpawnTime.Day)
                .ConfigureMaximum(4)
                .EnableSpawning(true)
                .EnableStars(true);

            AA_BroadTailHummingbird.Localize()
                .Portuguese_Brazilian("Beija-flor")
                .English("Hummingbird");

            AA_BroadTailHummingbird.ConfigureDrops();
            AA_BroadTailHummingbird.Drops["Feathers"].Amount = new Range(2, 3);
            AA_BroadTailHummingbird.Drops["Feathers"].DropChance = 100f;
            AA_BroadTailHummingbird.Drops["Nectar"].Amount = new Range(1, 1);
            AA_BroadTailHummingbird.Drops["Nectar"].DropChance = 100f;
            #endregion

            #region Bald Eagle
            Creature AA_BaldEagle = new Creature("airanimals", "AA_BaldEagle")
                .ConfigureBiome(Heightmap.Biome.Plains)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(2, 1000))
                .ConfigureForestSpawn(Forest.No)
                .ConfigureRequiredWeather(Weather.ClearSkies | Weather.LightRain | Weather.Fog | Weather.Rain)
                .ConfigureSpawnChance(10)
                .ConfigureGroupSize(new Range(1, 1))
                .ConfigureSpawnInterval(1200)
                .ConfigureSpawnTime(SpawnTime.Day)
                .ConfigureMaximum(2)
                .EnableSpawning(true)
                .EnableStars(true);

            AA_BaldEagle.Localize()
                .Portuguese_Brazilian("Águia careca")
                .English("Bald Eagle");

            AA_BaldEagle.ConfigureDrops();
            AA_BaldEagle.Drops["Feathers"].Amount = new Range(6, 8);
            AA_BaldEagle.Drops["Feathers"].DropChance = 100f;
            AA_BaldEagle.Drops["Entrails"].Amount = new Range(1, 1);
            AA_BaldEagle.Drops["Entrails"].DropChance = 100f;
            #endregion

            #region Barn Owl
            Creature AA_BarnOwl = new Creature("airanimals", "AA_BarnOwl")
                .ConfigureBiome(Heightmap.Biome.Meadows | Heightmap.Biome.BlackForest | Heightmap.Biome.Plains)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(2, 1000))
                .ConfigureForestSpawn(Forest.Both)
                .ConfigureRequiredWeather(Weather.ClearSkies | Weather.MeadowsClearSkies | Weather.BlackForestFog | Weather.Fog)
                .ConfigureSpawnChance(10)
                .ConfigureGroupSize(new Range(1, 1))
                .ConfigureSpawnInterval(1200)
                .ConfigureSpawnTime(SpawnTime.Night)
                .ConfigureMaximum(2)
                .EnableSpawning(true)
                .EnableStars(true);

            AA_BarnOwl.Localize()
                .Portuguese_Brazilian("Coruja-das-torres")
                .English("Barn Owl");

            AA_BarnOwl.ConfigureDrops();
            AA_BarnOwl.Drops["Feathers"].Amount = new Range(4, 8);
            AA_BarnOwl.Drops["Feathers"].DropChance = 100f;
            AA_BarnOwl.Drops["Entrails"].Amount = new Range(1, 1);
            AA_BarnOwl.Drops["Entrails"].DropChance = 100f;
            #endregion

            #region Red Crowned Crane
            Creature AA_RedCrownedCrane = new Creature("airanimals", "AA_RedCrownedCrane")
                .ConfigureBiome(Heightmap.Biome.Meadows | Heightmap.Biome.Plains)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(1, 1000))
                .ConfigureForestSpawn(Forest.Both)
                .ConfigureRequiredWeather(Weather.ClearSkies | Weather.MeadowsClearSkies | Weather.LightRain | Weather.YagluthsMagicBlizzard)
                .ConfigureSpawnChance(10)
                .ConfigureGroupSize(new Range(1, 2))
                .ConfigureSpawnInterval(1200)
                .ConfigureSpawnTime(SpawnTime.Day)
                .ConfigureMaximum(2)
                .EnableSpawning(true)
                .EnableStars(true);

            AA_RedCrownedCrane.Localize()
                .Portuguese_Brazilian("Grou coroado vermelho")
                .English("Red Crowned Crane");

            AA_RedCrownedCrane.ConfigureDrops();
            AA_RedCrownedCrane.Drops["Feathers"].Amount = new Range(6, 8);
            AA_RedCrownedCrane.Drops["Feathers"].DropChance = 100f;
            #endregion

            #region Pigeon
            Creature AA_Pigeon = new Creature("airanimals", "AA_Pigeon")
                .ConfigureBiome(Heightmap.Biome.Meadows | Heightmap.Biome.BlackForest | Heightmap.Biome.Plains)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(2, 1000))
                .ConfigureForestSpawn(Forest.Both)
                .ConfigureRequiredWeather(Weather.ClearSkies | Weather.MeadowsClearSkies | Weather.BlackForestFog)
                .ConfigureSpawnChance(10)
                .ConfigureGroupSize(new Range(1, 4))
                .ConfigureSpawnInterval(1200)
                .ConfigureSpawnTime(SpawnTime.Day)
                .ConfigureMaximum(4)
                .EnableSpawning(true)
                .EnableStars(true);

            AA_Pigeon.Localize()
                .Portuguese_Brazilian("Pombo")
                .English("Pigeon");

            AA_Pigeon.ConfigureDrops();
            AA_Pigeon.Drops["Feathers"].Amount = new Range(3, 4);
            AA_Pigeon.Drops["Feathers"].DropChance = 100f;
            #endregion

            #region Secretary Bird
            Creature AA_SecretaryBird = new Creature("airanimals", "AA_SecretaryBird")
                .ConfigureBiome(Heightmap.Biome.Plains)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(2, 1000))
                .ConfigureForestSpawn(Forest.No)
                .ConfigureRequiredWeather(Weather.ClearSkies | Weather.LightRain)
                .ConfigureSpawnChance(10)
                .ConfigureGroupSize(new Range(1, 2))
                .ConfigureSpawnInterval(1200)
                .ConfigureSpawnTime(SpawnTime.Day)
                .ConfigureMaximum(2)
                .EnableSpawning(true)
                .EnableStars(true);

            AA_SecretaryBird.Localize()
                .Portuguese_Brazilian("Ave Secretária")
                .English("Secretary Bird");

            AA_SecretaryBird.ConfigureDrops();
            AA_SecretaryBird.Drops["Feathers"].Amount = new Range(6, 8);
            AA_SecretaryBird.Drops["Feathers"].DropChance = 100f;
            AA_SecretaryBird.Drops["Entrails"].Amount = new Range(1, 1);
            AA_SecretaryBird.Drops["Entrails"].DropChance = 100f;
            #endregion

            #region Archaeopteryx
            Creature AA_Archaeopteryx = new Creature("airanimals", "AA_Archaeopteryx")
                .ConfigureBiome(Heightmap.Biome.Swamp)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(1, 1000))
                .ConfigureForestSpawn(Forest.Both)
                .ConfigureRequiredWeather(Weather.None)
                .ConfigureSpawnChance(10)
                .ConfigureGroupSize(new Range(1, 2))
                .ConfigureSpawnInterval(1200)
                .ConfigureSpawnTime(SpawnTime.Always)
                .ConfigureMaximum(2)
                .EnableSpawning(true)
                .EnableStars(true);

            AA_Archaeopteryx.Localize()
                .Portuguese_Brazilian("Archaeopteryx")
                .English("Archaeopteryx");

            AA_Archaeopteryx.ConfigureDrops();
            AA_Archaeopteryx.Drops["Feathers"].Amount = new Range(6, 8);
            AA_Archaeopteryx.Drops["Feathers"].DropChance = 100f;
            AA_Archaeopteryx.Drops["Entrails"].Amount = new Range(1, 1);
            AA_Archaeopteryx.Drops["Entrails"].DropChance = 100f;
            #endregion

            #region ButterFly1
            Creature ButterFly1 = new Creature("airanimals", "ButterFly1")
                .ConfigureBiome(Heightmap.Biome.Meadows)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(1, 1000))
                .ConfigureForestSpawn(Forest.Both)
                .ConfigureSpawnChance(20)
                .ConfigureRequiredWeather(Weather.ClearSkies | Weather.MeadowsClearSkies)
                .ConfigureGroupSize(new Range(1, 2))
                .ConfigureSpawnInterval(1000)
                .ConfigureSpawnTime(SpawnTime.Day)
                .ConfigureMaximum(2)
                .EnableSpawning(true)
                .EnableStars(true);

            ButterFly1.Localize()
                .Portuguese_Brazilian("Viceroy")
                .English("Viceroy");

            ButterFly1.ConfigureDrops();
            ButterFly1.Drops["Nectar"].Amount = new Range(1, 1);
            ButterFly1.Drops["Nectar"].DropChance = 100f;
            #endregion

            #region ButterFly2
            Creature ButterFly2 = new Creature("airanimals", "ButterFly2")
                .ConfigureBiome(Heightmap.Biome.Meadows)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(1, 1000))
                .ConfigureForestSpawn(Forest.Both)
                .ConfigureSpawnChance(20)
                .ConfigureRequiredWeather(Weather.ClearSkies | Weather.MeadowsClearSkies)
                .ConfigureGroupSize(new Range(1, 2))
                .ConfigureSpawnInterval(1000)
                .ConfigureSpawnTime(SpawnTime.Day)
                .ConfigureMaximum(2)
                .EnableSpawning(true)
                .EnableStars(true);

            ButterFly2.Localize()
                .Portuguese_Brazilian("Horaces DuskyWing")
                .English("Horaces DuskyWing");

            ButterFly2.ConfigureDrops();
            ButterFly2.Drops["Nectar"].Amount = new Range(1, 1);
            ButterFly2.Drops["Nectar"].DropChance = 100f;
            #endregion

            #region ButterFly3
            Creature ButterFly3 = new Creature("airanimals", "ButterFly3")
                .ConfigureBiome(Heightmap.Biome.BlackForest)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(1, 1000))
                .ConfigureForestSpawn(Forest.Both)
                .ConfigureSpawnChance(20)
                .ConfigureRequiredWeather(Weather.ClearSkies | Weather.MeadowsClearSkies | Weather.BlackForestFog)
                .ConfigureGroupSize(new Range(1, 2))
                .ConfigureSpawnInterval(1000)
                .ConfigureSpawnTime(SpawnTime.Day)
                .ConfigureMaximum(2)
                .EnableSpawning(true)
                .EnableStars(true);

            ButterFly3.Localize()
                .Portuguese_Brazilian("RedSpoted Roxa")
                .English("RedSpoted Purple");

            ButterFly3.ConfigureDrops();
            ButterFly3.Drops["Nectar"].Amount = new Range(1, 1);
            ButterFly3.Drops["Nectar"].DropChance = 100f;
            #endregion

            #region ButterFly4
            Creature ButterFly4 = new Creature("airanimals", "ButterFly4")
                .ConfigureBiome(Heightmap.Biome.BlackForest)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(1, 1000))
                .ConfigureForestSpawn(Forest.Both)
                .ConfigureSpawnChance(20)
                .ConfigureRequiredWeather(Weather.ClearSkies | Weather.MeadowsClearSkies | Weather.BlackForestFog)
                .ConfigureGroupSize(new Range(1, 2))
                .ConfigureSpawnInterval(1000)
                .ConfigureSpawnTime(SpawnTime.Day)
                .ConfigureMaximum(2)
                .EnableSpawning(true)
                .EnableStars(true);

            ButterFly4.Localize()
                .Portuguese_Brazilian("RedBanded")
                .English("RedBanded");

            ButterFly4.ConfigureDrops();
            ButterFly4.Drops["Nectar"].Amount = new Range(1, 1);
            ButterFly4.Drops["Nectar"].DropChance = 100f;
            #endregion

            #region ButterFly5
            Creature ButterFly5 = new Creature("airanimals", "ButterFly5")
                .ConfigureBiome(Heightmap.Biome.Plains)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(1, 1000))
                .ConfigureForestSpawn(Forest.Both)
                .ConfigureSpawnChance(20)
                .ConfigureRequiredWeather(Weather.ClearSkies)
                .ConfigureGroupSize(new Range(1, 2))
                .ConfigureSpawnInterval(1000)
                .ConfigureSpawnTime(SpawnTime.Day)
                .ConfigureMaximum(1)
                .EnableSpawning(true)
                .EnableStars(true);

            ButterFly5.Localize()
                .Portuguese_Brazilian("CloudLess Sulphur")
                .English("CloudLess Sulphur");

            ButterFly5.ConfigureDrops();
            ButterFly5.Drops["Nectar"].Amount = new Range(1, 1);
            ButterFly5.Drops["Nectar"].DropChance = 100f;
            #endregion

            #region ButterFly6
            Creature ButterFly6 = new Creature("airanimals", "ButterFly6")
                .ConfigureBiome(Heightmap.Biome.Plains)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(1, 1000))
                .ConfigureForestSpawn(Forest.Both)
                .ConfigureSpawnChance(20)
                .ConfigureRequiredWeather(Weather.ClearSkies)
                .ConfigureGroupSize(new Range(1, 2))
                .ConfigureSpawnInterval(1000)
                .ConfigureSpawnTime(SpawnTime.Day)
                .ConfigureMaximum(2)
                .EnableSpawning(true)
                .EnableStars(true);

            ButterFly6.Localize()
                .Portuguese_Brazilian("Checkered White")
                .English("Checkered White");

            ButterFly6.ConfigureDrops();
            ButterFly6.Drops["Nectar"].Amount = new Range(1, 1);
            ButterFly6.Drops["Nectar"].DropChance = 100f;
            #endregion

            #region Beetle1
            Creature Beetle1 = new Creature("airanimals", "Beetle1")
                .ConfigureBiome(Heightmap.Biome.Meadows)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(2, 1000))
                .ConfigureForestSpawn(Forest.No)
                .ConfigureSpawnChance(20)
                .ConfigureRequiredWeather(Weather.ClearSkies | Weather.LightRain | Weather.MeadowsClearSkies)
                .ConfigureGroupSize(new Range(1, 1))
                .ConfigureSpawnInterval(1000)
                .ConfigureSpawnTime(SpawnTime.Day)
                .ConfigureMaximum(1)
                .EnableSpawning(true)
                .EnableStars(true);

            Beetle1.Localize()
                .Portuguese_Brazilian("Besouro Marrom")
                .English("Beetle Brown");

            Beetle1.ConfigureDrops();
            Beetle1.Drops["BeetleProtein"].Amount = new Range(1, 1);
            Beetle1.Drops["BeetleProtein"].DropChance = 100f;
            #endregion

            #region Beetle2
            Creature Beetle2 = new Creature("airanimals", "Beetle2")
                .ConfigureBiome(Heightmap.Biome.Meadows)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(2, 1000))
                .ConfigureForestSpawn(Forest.No)
                .ConfigureSpawnChance(20)
                .ConfigureRequiredWeather(Weather.ClearSkies | Weather.LightRain | Weather.MeadowsClearSkies)
                .ConfigureGroupSize(new Range(1, 1))
                .ConfigureSpawnInterval(1000)
                .ConfigureSpawnTime(SpawnTime.Day)
                .ConfigureMaximum(1)
                .EnableSpawning(true)
                .EnableStars(true);

            Beetle2.Localize()
                .Portuguese_Brazilian("Besouro Ouro")
                .English("Beetle Gold");

            Beetle2.ConfigureDrops();
            Beetle2.Drops["BeetleProtein"].Amount = new Range(1, 1);
            Beetle2.Drops["BeetleProtein"].DropChance = 100f;
            #endregion

            #region Beetle3
            Creature Beetle3 = new Creature("airanimals", "Beetle3")
                .ConfigureBiome(Heightmap.Biome.BlackForest)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(2, 1000))
                .ConfigureForestSpawn(Forest.No)
                .ConfigureSpawnChance(20)
                .ConfigureRequiredWeather(Weather.ClearSkies | Weather.LightRain | Weather.BlackForestFog)
                .ConfigureGroupSize(new Range(1, 1))
                .ConfigureSpawnInterval(1000)
                .ConfigureSpawnTime(SpawnTime.Day)
                .ConfigureMaximum(1)
                .EnableSpawning(true)
                .EnableStars(true);

            Beetle3.Localize()
                .Portuguese_Brazilian("Besouro Roxo")
                .English("Beetle Purple");

            Beetle3.ConfigureDrops();
            Beetle3.Drops["BeetleProtein"].Amount = new Range(1, 1);
            Beetle3.Drops["BeetleProtein"].DropChance = 100f;
            #endregion

            #region Beetle4
            Creature Beetle4 = new Creature("airanimals", "Beetle4")
                .ConfigureBiome(Heightmap.Biome.Plains)
                .ConfigureSpawnArea(CreatureManager.SpawnArea.Everywhere)
                .ConfigureRequiredGlobalKey(GlobalKey.None)
                .ConfigureRequiredAltitude(new Range(2, 1000))
                .ConfigureForestSpawn(Forest.No)
                .ConfigureSpawnChance(20)
                .ConfigureRequiredWeather(Weather.ClearSkies | Weather.LightRain)
                .ConfigureGroupSize(new Range(1, 1))
                .ConfigureSpawnInterval(1000)
                .ConfigureSpawnTime(SpawnTime.Day)
                .ConfigureMaximum(1)
                .EnableSpawning(true)
                .EnableStars(true);

            Beetle4.Localize()
                .Portuguese_Brazilian("Besouro Verde")
                .English("Beetle Green");

            Beetle4.ConfigureDrops();
            Beetle4.Drops["BeetleProtein"].Amount = new Range(1, 1);
            Beetle4.Drops["BeetleProtein"].DropChance = 100f;
            #endregion

            #region Registro de Prefabs e Efeitos


            Item nectar = new("airanimals", "Nectar"); //assetbundle name, Asset Name
            nectar.Name.Chinese("花蜜");
            nectar.Name.Danish("Nektar");
            nectar.Name.English("Nectar");
            nectar.Name.French("Nectar");
            nectar.Name.German("Nektar");
            nectar.Name.Icelandic("Nektar");
            nectar.Name.Italian("Nettare");
            nectar.Name.Japanese("蜜");
            nectar.Name.Norwegian("Nektar");
            nectar.Name.Polish("Nektar");
            nectar.Name.Portuguese_Brazilian("Néctar");
            nectar.Name.Russian("Нектар");
            nectar.Name.Spanish("Néctar");
            nectar.Name.Swedish("Nektar");
            nectar.Description.Chinese("花蜜非常适合让您保持精力充沛。");
            nectar.Description.Danish("Nektar er fremragende til at holde dig energisk.");
            nectar.Description.English("Nectar is excellent for keeping you energized.");
            nectar.Description.French("Le nectar est excellent pour vous garder énergisé.");
            nectar.Description.German("Nektar eignet sich hervorragend, um Sie mit Energie zu versorgen.");
            nectar.Description.Icelandic("Nektar er frábært til að halda þér orku.");
            nectar.Description.Italian("Il nettare è eccellente per mantenerti energico.");
            nectar.Description.Japanese("蜜はあなたを元気に保つのに最適です。");
            nectar.Description.Norwegian("Nektar er utmerket for å holde deg energisk.");
            nectar.Description.Polish("Nektar jest doskonały do utrzymania energii.");
            nectar.Description.Portuguese_Brazilian("O néctar é excelente para mantê-lo energizado.");
            nectar.Description.Russian("Нектар отлично подходит для поддержания энергии.");
            nectar.Description.Spanish("El néctar es excelente para mantenerte energizado.");
            nectar.Description.Swedish("Nektar är utmärkt för att hålla dig energisk.");


            Item beetleprotein = new("airanimals", "BeetleProtein"); //assetbundle name, Asset Name
            beetleprotein.Name.Chinese("甲虫蛋白");
            beetleprotein.Name.Danish("Billeprotein");
            beetleprotein.Name.English("Beetle protein");
            beetleprotein.Name.French("Protéine de coléoptère");
            beetleprotein.Name.German("Käfer-Protein");
            beetleprotein.Name.Icelandic("Beetle prótein");
            beetleprotein.Name.Italian("Proteina dello scarabeo");
            beetleprotein.Name.Japanese("カブトムシタンパク質");
            beetleprotein.Name.Norwegian("Beetle protein");
            beetleprotein.Name.Polish("Białko chrząszcza");
            beetleprotein.Name.Portuguese_Brazilian("Proteína do besouro");
            beetleprotein.Name.Russian("Белок жука");
            beetleprotein.Name.Spanish("Proteína de escarabajo");
            beetleprotein.Name.Swedish("Beetle protein");
            beetleprotein.Description.Chinese("甲虫蛋白非常适合让您保持精力充沛。");
            beetleprotein.Description.Danish("Beetleprotein er fremragende til at holde dig energisk.");
            beetleprotein.Description.English("Beetle protein is excellent for keeping you energized.");
            beetleprotein.Description.French("La protéine de coléoptère est excellente pour vous garder énergisé.");
            beetleprotein.Description.German("Käferprotein eignet sich hervorragend, um Sie mit Energie zu versorgen.");
            beetleprotein.Description.Icelandic("Beetle prótein er frábært til að halda þér orkugjafi.");
            beetleprotein.Description.Italian("La proteina dello scarabeo è eccellente per mantenerti energico.");
            beetleprotein.Description.Japanese("カブトムシタンパク質はあなたを元気に保つのに優れています。");
            beetleprotein.Description.Norwegian("Beetle protein er utmerket for å holde deg energisk.");
            beetleprotein.Description.Polish("Białko chrząszcza jest doskonałe do utrzymania energii.");
            beetleprotein.Description.Portuguese_Brazilian("A proteína do besouro é excelente para mantê-lo energizado.");
            beetleprotein.Description.Russian("Белок жука отлично подходит для поддержания энергии.");
            beetleprotein.Description.Spanish("La proteína del escarabajo es excelente para mantenerte energizado.");
            beetleprotein.Description.Swedish("Beetle protein är utmärkt för att hålla dig energisk.");
            
            // EFFECTS VFX 

            GameObject vfx_insect_death = ItemManager.PrefabManager.RegisterPrefab("airanimals", "vfx_insect_death");
            GameObject vfx_insect_deathb = ItemManager.PrefabManager.RegisterPrefab("airanimals", "vfx_insect_deathb");
            GameObject air_fx_backstab = ItemManager.PrefabManager.RegisterPrefab("airanimals", "air_fx_backstab");
            GameObject air_fx_crit = ItemManager.PrefabManager.RegisterPrefab("airanimals", "air_fx_crit");
            GameObject air_vfx_death = ItemManager.PrefabManager.RegisterPrefab("airanimals", "air_vfx_death");
            GameObject air_vfx_hit = ItemManager.PrefabManager.RegisterPrefab("airanimals", "air_vfx_hit");
            GameObject air_vfx_HitSparks = ItemManager.PrefabManager.RegisterPrefab("airanimals", "air_vfx_HitSparks");

            // SOUNDS SFX
            
            GameObject AirRagdoll = ItemManager.PrefabManager.RegisterPrefab("airanimals", "AirRagdoll");
            GameObject sfx_archa_alerted = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_archa_alerted");
            GameObject sfx_archa_atkfly = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_archa_atkfly");
            GameObject sfx_archa_attack = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_archa_attack");
            GameObject sfx_archa_death = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_archa_death");
            GameObject sfx_archa_hit = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_archa_hit");
            GameObject sfx_archa_Idle = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_archa_Idle");
            GameObject sfx_baldeagle_alerted = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_baldeagle_alerted");
            GameObject sfx_baldeagle_attack = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_baldeagle_attack");
            GameObject sfx_baldeagle_death = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_baldeagle_death");
            GameObject sfx_baldeagle_hit = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_baldeagle_hit");
            GameObject sfx_baldeagle_Idle = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_baldeagle_Idle");
            GameObject sfx_barnowl_alerted = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_barnowl_alerted");
            GameObject sfx_barnowl_attack = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_barnowl_attack");
            GameObject sfx_barnowl_death = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_barnowl_death");
            GameObject sfx_barnowl_hit = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_barnowl_hit");
            GameObject sfx_barnowl_Idle = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_barnowl_Idle");
            GameObject sfx_flybeetle = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_flybeetle");
            GameObject sfx_insect_death = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_insect_death");
            GameObject sfx_insect_deathb = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_insect_deathb");
            GameObject sfx_harpy_alerted = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_harpy_alerted");
            GameObject sfx_harpy_attack = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_harpy_attack");
            GameObject sfx_harpy_death = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_harpy_death");
            GameObject sfx_harpy_hit = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_harpy_hit");
            GameObject sfx_harpy_Idle = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_harpy_Idle");
            GameObject sfx_hummingbird_alerted = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_hummingbird_alerted");
            GameObject sfx_hummingbird_death = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_hummingbird_death");
            GameObject sfx_hummingbird_fly = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_hummingbird_fly");
            GameObject sfx_hummingbird_hit = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_hummingbird_hit");
            GameObject sfx_hummingbird_Idle = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_hummingbird_Idle");
            GameObject sfx_pigeon_alerted = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_pigeon_alerted");
            GameObject sfx_pigeon_death = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_pigeon_death");
            GameObject sfx_pigeon_flap = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_pigeon_flap");
            GameObject sfx_pigeon_hit = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_pigeon_hit");
            GameObject sfx_pigeon_Idle = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_pigeon_Idle");
            GameObject sfx_redcc_alerted = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_redcc_alerted");
            GameObject sfx_redcc_death = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_redcc_death");
            GameObject sfx_redcc_hit = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_redcc_hit");
            GameObject sfx_redcc_Idle = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_redcc_Idle");
            GameObject sfx_secretarybird_alerted = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_secretarybird_alerted");
            GameObject sfx_secretarybird_attack = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_secretarybird_attack");
            GameObject sfx_secretarybird_death = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_secretarybird_death");
            GameObject sfx_secretarybird_hit = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_secretarybird_hit");
            GameObject sfx_secretarybird_Idle = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_secretarybird_Idle");
            GameObject sfx_swan_alerted = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_swan_alerted");
            GameObject sfx_swan_death = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_swan_death");
            GameObject sfx_swan_Idle = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_swan_Idle");
            GameObject sfx_wildduck_alerted = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_wildduck_alerted");
            GameObject sfx_wildduck_death = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_wildduck_death");
            GameObject sfx_wildduck_hit = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_wildduck_hit");
            GameObject sfx_wildduck_Idle = ItemManager.PrefabManager.RegisterPrefab("airanimals", "sfx_wildduck_Idle");
            
            // EFFECTS ATK

            GameObject BaldEagle_Atk = ItemManager.PrefabManager.RegisterPrefab("airanimals", "BaldEagle_Atk");
            GameObject BaldEagle_AtkFly = ItemManager.PrefabManager.RegisterPrefab("airanimals", "BaldEagle_AtkFly");
            GameObject BarnOwl_AtkFly = ItemManager.PrefabManager.RegisterPrefab("airanimals", "BarnOwl_AtkFly");
            GameObject SecretaryBird_Atk = ItemManager.PrefabManager.RegisterPrefab("airanimals", "SecretaryBird_Atk");
            GameObject SecretaryBird_AtkFly = ItemManager.PrefabManager.RegisterPrefab("airanimals", "SecretaryBird_AtkFly");
            GameObject Archaeopteryxatk = ItemManager.PrefabManager.RegisterPrefab("airanimals", "Archaeopteryxatk");
            GameObject Archaeopteryxatk_fly = ItemManager.PrefabManager.RegisterPrefab("airanimals", "Archaeopteryxatk_fly");

            #endregion


            SetupWatcher();
            _harmony.PatchAll();
        }

        private void OnDestroy()
        {
            Config.Save();
        }

        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                FarmLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                FarmLogger.LogError($"There was an issue loading your {ConfigFileName}");
                FarmLogger.LogError("Please check your config entries for spelling and format!");
            }
        }


        #region ConfigOptions

        private static ConfigEntry<bool>? _serverConfigLocked;

        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);

            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        private ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        private class ConfigurationManagerAttributes
        {
            public bool? Browsable = false;
        }

        #endregion

 

        private static int GetZDO(int prefabHash)
        {
            int prefabCount = 0;
            foreach (List<ZDO> zdoList in ZDOMan.instance.m_objectsBySector)
            {
                if (zdoList == null) continue;

                for (int index = 0; index < zdoList.Count; ++index)
                {
                    ZDO zdo2 = zdoList[index];
                    if (zdo2.GetPrefab() == prefabHash)
                    {
                        prefabCount++;
                    }
                }
            }

            return prefabCount;
        }


        private static int GetPrefabCount(int prefabHash)
        {
            int prefabCount = 0;
            foreach (List<ZDO> zdoList in ZDOMan.instance.m_objectsBySector)
            {
                if (zdoList == null) continue;

                for (int index = 0; index < zdoList.Count; ++index)
                {
                    ZDO zdo2 = zdoList[index];
                    if (zdo2.GetPrefab() == prefabHash)
                    {
                        prefabCount++;
                    }
                }
            }

            return prefabCount;
        }


        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        static class TheFarmerZNetScene_AwakePost_Patch
        {
            static void Postfix(ZNetScene __instance)
            {
                if (__instance == null || __instance.m_prefabs is not { Count: > 0 }) return;

                // Obtenha estações de cozinha do ZNetScene
                CookingStation cookingStation =
                    __instance.GetPrefab("piece_cookingstation").GetComponent<CookingStation>();
                CookingStation cookingStationIron =
                    __instance.GetPrefab("piece_cookingstation_iron").GetComponent<CookingStation>();

                /*
                 Percorra a lista de carnes, pegue os pares de chave e valor
                 Chave é o item do qual queremos converter
                 Valor é o item para o qual queremos converter
                  */
                foreach (KeyValuePair<GameObject, GameObject> carne in carnes)
                {
                    cookingStation.m_conversion.Add(new CookingStation.ItemConversion()
                    {
                        m_cookTime = 25f,
                        m_from = __instance.GetPrefab(carne.Key.name).GetComponent<ItemDrop>(),
                        m_to = __instance.GetPrefab(carne.Value.name).GetComponent<ItemDrop>(),
                    });

                    cookingStationIron.m_conversion.Add(new CookingStation.ItemConversion()
                    {
                        m_cookTime = 25f,
                        m_from = __instance.GetPrefab(carne.Key.name).GetComponent<ItemDrop>(),
                        m_to = __instance.GetPrefab(carne.Value.name).GetComponent<ItemDrop>(),
                    });
                }
            }
        }
    }
}