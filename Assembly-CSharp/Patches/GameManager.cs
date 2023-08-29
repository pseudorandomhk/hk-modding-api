using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using GlobalEnums;
using MonoMod;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable all
#pragma warning disable 1591, 649, 414, 169, CS0108, CS0626

namespace Modding.Patches
{
    [MonoModPatch("global::GameManager")]
    public class GameManager : global::GameManager
    {
        [MonoModIgnore]
        private static GameManager _instance;

        [MonoModReplace]
        public static GameManager get_instance() { 
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    Debug.LogError("Couldn't find a Game Manager, make sure one exists in the scene.");
                }
                else if (Application.isPlaying)
                {
                    DontDestroyOnLoad(_instance);
                }
            }
            return _instance;
        }

        public static GameManager UnsafeInstance => _instance;

        public extern void orig_OnApplicationQuit();

        public void OnApplicationQuit()
        {
            orig_OnApplicationQuit();
            ModHooks.OnApplicationQuit();
        }

        #region Transitions

        [MonoModIgnore]
        [MonoModPublic]
        private void BeginScene() { }

        public bool IsInSceneTransition { get; private set; }

        public extern void orig_LoadScene(string destScene);

        public void LoadScene(string destScene)
        {
            IsInSceneTransition = true;
            destScene = ModHooks.BeforeSceneLoad(destScene);

            orig_LoadScene(destScene);

            ModHooks.OnSceneChanged(destScene);
            IsInSceneTransition = false;
        }

        public class SceneLoadInfo
        {
            public string SceneName;
            public string EntryGateName;
            public float EntryDelay;
            public GatePosition HeroLeaveDirection;
            public TransitionPoint CallingGate;

            public SceneLoadInfo() { }

            public SceneLoadInfo(TransitionPoint callingGate)
            {
                this.SceneName = callingGate.targetScene;
                this.EntryGateName = callingGate.entryPoint;
                this.EntryDelay = 0f;
                this.HeroLeaveDirection = callingGate.GetGatePosition();
                this.CallingGate = callingGate;
            }
        }

        [MonoModIgnore]
        private TransitionPoint callingGate;

        [MonoModIgnore]
        private string targetScene;

        [MonoModIgnore]
        private float entryDelay;

        public IEnumerator TransitionSceneWithInfo(SceneLoadInfo info)
        {
            bool useGate = info.CallingGate != null;
            this.callingGate = useGate ? info.CallingGate : null;
            if (this.hero_ctrl.cState.superDashing)
            {
                this.hero_ctrl.exitedSuperDashing = true;
            }
            if (this.hero_ctrl.cState.spellQuake)
            {
                this.hero_ctrl.exitedQuake = true;
            }
            this.hero_ctrl.GetComponent<PlayMakerFSM>().SendEvent("HeroCtrl-LeavingScene");
            this.NoLongerFirstGame();
            this.SaveLevelState();
            this.SetState(GameState.EXITING_LEVEL);
            this.entryGateName = useGate ? info.CallingGate.entryPoint : info.EntryGateName;
            this.targetScene = ModHooks.BeforeSceneLoad(useGate ? info.CallingGate.targetScene : info.SceneName);
            if (useGate)
                this.hero_ctrl.LeaveScene(info.CallingGate);
            else
                ((HeroController)this.hero_ctrl).LeaveSceneByGatePos(info.HeroLeaveDirection);
            this.cameraCtrl.FreezeInPlace(true);
            this.cameraCtrl.FadeOut(CameraFadeType.LEVEL_TRANSITION);
            yield return new WaitForSeconds(0.5f);
            this.LeftScene(true);
            yield break;
        }

        [MonoModReplace]
        public IEnumerator TransitionScene(TransitionPoint gate) => TransitionSceneWithInfo(new SceneLoadInfo(gate));

        public void ChangeToSceneWithInfo(SceneLoadInfo info)
        {
            if (this.hero_ctrl != null)
            {
                this.hero_ctrl.proxyFSM.SendEvent("HeroCtrl-LeavingScene");
                this.hero_ctrl.transform.SetParent(null);
            }
            this.NoLongerFirstGame();
            this.SaveLevelState();
            this.SetState(GameState.EXITING_LEVEL);
            this.entryGateName = info.EntryGateName;
            this.targetScene = ModHooks.BeforeSceneLoad(info.SceneName);
            this.entryDelay = info.EntryDelay;
            this.cameraCtrl.FreezeInPlace(false);
            if (this.hero_ctrl != null)
            {
                this.hero_ctrl.ResetState();
            }
            this.LeftScene(false);
        }

        [MonoModReplace]
        public void ChangeToScene(string targetScene, string entryGateName, float pauseBeforeEnter)
        {
            ChangeToSceneWithInfo(new SceneLoadInfo
            {
                SceneName = targetScene,
                EntryGateName = entryGateName,
                EntryDelay = pauseBeforeEnter
            });
        }

        //public extern IEnumerator orig_TransitionScene(TransitionPoint gate);

        //public IEnumerator TransitionScene(TransitionPoint gate)
        //{
        //    string sceneName = ModHooks.BeforeSceneLoad(gate.targetScene);
        //    if (sceneName != gate.targetScene) throw new NotSupportedException("Modifying destination scene not yet supported");

        //    return orig_TransitionScene(gate);
        //}

        //public IEnumerator TransitionScene(string entryGateName, string targetScene, GatePosition gatePos)
        //{
        //    this.callingGate = null;
        //    if (this.hero_ctrl.cState.superDashing)
        //    {
        //        this.hero_ctrl.exitedSuperDashing = true;
        //    }
        //    if (this.hero_ctrl.cState.spellQuake)
        //    {
        //        this.hero_ctrl.exitedQuake = true;
        //    }
        //    this.hero_ctrl.GetComponent<PlayMakerFSM>().SendEvent("HeroCtrl-LeavingScene");
        //    this.NoLongerFirstGame();
        //    this.SaveLevelState();
        //    this.SetState(GameState.EXITING_LEVEL);
        //    this.entryGateName = entryGateName;
        //    this.targetScene = targetScene;
        //    ((HeroController)this.hero_ctrl).LeaveSceneByGatePos(gatePos);
        //    this.cameraCtrl.FreezeInPlace(true);
        //    this.cameraCtrl.FadeOut(CameraFadeType.LEVEL_TRANSITION);
        //    yield return new WaitForSeconds(0.5f);
        //    this.LeftScene(true);
        //    yield break;
        //}

        //private void ChangeToScene(string targetScene, string entryGateName, float pauseBeforeEnter, bool triggerHook)
        //{
        //    if (this.hero_ctrl != null)
        //    {
        //        this.hero_ctrl.proxyFSM.SendEvent("HeroCtrl-LeavingScene");
        //        this.hero_ctrl.transform.SetParent(null);
        //    }
        //    this.NoLongerFirstGame();
        //    this.SaveLevelState();
        //    this.SetState(GameState.EXITING_LEVEL);
        //    this.entryGateName = entryGateName;
        //    this.targetScene = triggerHook ? ModHooks.BeforeSceneLoad(targetScene) : targetScene;
        //    this.entryDelay = pauseBeforeEnter;
        //    this.cameraCtrl.FreezeInPlace(false);
        //    if (this.hero_ctrl != null)
        //    {
        //        this.hero_ctrl.ResetState();
        //    }
        //    this.LeftScene(false);
        //}

        //[MonoModReplace]
        //public void ChangeToScene(string targetScene, string entryGateName, float pauseBeforeEnter) =>
        //    ChangeToScene(targetScene, entryGateName, pauseBeforeEnter, true);

        //public void ChangeToSceneSilently(string targetScene, string entryGateName, float pauseBeforeEnter) =>
        //    ChangeToScene(targetScene, entryGateName, pauseBeforeEnter, false);

        #endregion

        public extern void orig_ClearSaveFile(int saveSlot);

        public void ClearSaveFile(int saveSlot)
        {
            ModHooks.OnSavegameClear(saveSlot);
            orig_ClearSaveFile(saveSlot);
            ModHooks.OnAfterSaveGameClear(saveSlot);
        }

        public extern IEnumerator orig_PlayerDead(float waitTime);

        public IEnumerator PlayerDead(float waitTime)
        {
            ModHooks.OnBeforePlayerDead();
            yield return orig_PlayerDead(waitTime);
            ModHooks.OnAfterPlayerDead();
        }

        [MonoModReplace]
        public string GetSaveFilename(int saveSlot)
        {
            string text = null;

            switch (saveSlot)
            {
                case 0:
                    text = "/user.dat";
                    break;
                case 1:
                    text = "/user1.dat";
                    break;
                case 2:
                    text = "/user2.dat";
                    break;
                case 3:
                    text = "/user3.dat";
                    break;
                case 4:
                    text = "/user4.dat";
                    break;
                default:
                    UnityEngine.Debug.LogError("Invalid save slot number specified.");
                    return null;
            }

            string modhook = ModHooks.GetSaveFileName(saveSlot);
            return string.IsNullOrEmpty(modhook) ? text : modhook;
        }

        #region SaveGame

        private ModSavegameData moddedData;

        [MonoModIgnore]
        private GameCameras gameCams;

        [MonoModIgnore]
        private float sessionPlayTimer;

        [MonoModIgnore]
        private float sessionStartTime;

        [MonoModIgnore]
        private float sessionTotalPlayTime;

        [MonoModIgnore]
        private float intervalStartTime;

        [MonoModIgnore]
        private extern void UpdateSessionPlayTime();

        private static string ModdedSavePath(int slot) => Path.Combine(
            Application.persistentDataPath,
            $"user{slot}.modded.json"
        );

        private UIManager _uiInstance;

        public UIManager ui
        {
            get
            {
                if (_uiInstance == null) _uiInstance = (UIManager)UIManager.instance;
                return _uiInstance;
            }
            private set => _uiInstance = value;
        }

        [MonoModReplace]
        public void SaveGame(int saveSlot)
        {
            if (saveSlot >= 0)
            {
                if (this.gameCams.saveIcon != null)
                {
                    this.gameCams.saveIcon.SendEvent("GAME SAVED");
                } else
                {
                    GameObject gameObject = GameObject.FindGameObjectWithTag("Save Icon");
                    if (gameObject != null)
                    {
                        PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(gameObject, "Checkpoint Control");
                        if (playMakerFSM != null)
                        {
                            playMakerFSM.SendEvent("GAME SAVED");
                        }
                    }
                }
                this.SaveLevelState();
                if (!this.gameConfig.disableSaveGame)
                {
                    if (this.achievementHandler != null)
                    {
                        this.achievementHandler.FlushRecordsToDisk();
                    } else
                    {
                        UnityEngine.Debug.LogError("Error saving achievements (PlayerAchievements is null)");
                    }
                    if (this.playerData != null)
                    {
                        if (this.gameState != GameState.PAUSED)
                        {
                            this.UpdateSessionPlayTime();
                        }
                        this.playerData.playTime += this.sessionTotalPlayTime;
                        this.sessionTotalPlayTime = 0f;
                        this.intervalStartTime = Time.realtimeSinceStartup;
                        this.playerData.version = "1.0.2.8";
                        this.playerData.profileID = saveSlot;
                        this.playerData.CountGameCompletion();
                    } else
                    {
                        UnityEngine.Debug.LogError("Error updating PlayerData before save (PlayerData is null)");
                    }
                    string saveFilename = this.GetSaveFilename(saveSlot);
                    string text = Application.persistentDataPath + saveFilename;
                    string text2 = Application.persistentDataPath + saveFilename + ".bak";
                    string text3 = Application.persistentDataPath + saveFilename + ".del";
                    bool flag = false;
                    if (File.Exists(text3))
                    {
                        try
                        {
                            File.Delete(text3);
                        } catch (Exception arg)
                        {
                            UnityEngine.Debug.LogError("Unable to remove pre-existing discarded save file: " + arg);
                            flag = false;
                        }
                    }
                    if (File.Exists(text2))
                    {
                        try
                        {
                            File.Move(text2, text3);
                            flag = true;
                        } catch (Exception arg2)
                        {
                            UnityEngine.Debug.LogError("Unable to move backup save to deletion state: " + arg2);
                            flag = false;
                        }
                    }
                    if (File.Exists(text))
                    {
                        try
                        {
                            File.Move(text, text2);
                        } catch (Exception arg3)
                        {
                            UnityEngine.Debug.LogError("Unable to move save game to backup file: " + arg3);
                            flag = false;
                        }
                    }
                    try
                    {
                        SaveGameData obj = new SaveGameData(this.playerData, this.sceneData);
                        ModHooks.OnBeforeSaveGameSave(obj);
                        if (this.moddedData == null)
                        {
                            this.moddedData = new ModSavegameData();
                        }
                        ModHooks.OnSaveLocalSettings(this.moddedData);

                        // save modded data
                        try
                        {
                            var path = ModdedSavePath(saveSlot);
                            string modded = JsonConvert.SerializeObject(
                                this.moddedData,
                                Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                    ContractResolver = ShouldSerializeContractResolver.Instance,
                                    TypeNameHandling = TypeNameHandling.Auto,
                                    Converters = JsonConverterTypes.ConverterTypes
                                }
                            );
                            if (File.Exists(path + ".bak")) File.Delete(path + ".bak");
                            if (File.Exists(path)) File.Move(path, path + ".bak");
                            using FileStream modFileStream = File.Create(path);
                            using var writer = new StreamWriter(modFileStream);
                            writer.Write(modded);
                        } catch (Exception e)
                        {
                            Logger.APILogger.LogError(e);
                        }

                        string text4 = null;

                        try
                        {
                            text4 = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings()
                            {
                                ContractResolver = ShouldSerializeContractResolver.Instance,
                                TypeNameHandling = TypeNameHandling.Auto,
                                Converters = JsonConverterTypes.ConverterTypes
                            });
                        } catch (Exception e)
                        {
                            Logger.LogError("Failed to serialize save using Json.NET, trying fallback.");

                            Logger.APILogger.LogError(e);

                            // If this dies, not much we can do about it.
                            text4 = JsonUtility.ToJson(obj);
                        }

                        string graph = StringEncrypt.EncryptData(text4);
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        FileStream fileStream = File.Create(Application.persistentDataPath + saveFilename);
                        if (this.gameConfig.useSaveEncryption)
                        {
                            binaryFormatter.Serialize(fileStream, graph);
                        } else
                        {
                            binaryFormatter.Serialize(fileStream, text4);
                        }
                        fileStream.Close();
                    } catch (Exception arg4)
                    {
                        UnityEngine.Debug.LogError("GM Save - There was an error saving the game: " + arg4);
                        flag = false;
                    }
                    if (flag)
                    {
                        try
                        {
                            File.Delete(text3);
                        } catch (Exception arg5)
                        {
                            UnityEngine.Debug.LogError("Unable to remove discarded save file: " + arg5);
                        }
                    }
                } else
                {
                    UnityEngine.Debug.Log("Saving game disabled. No save file written.");
                }
            } else
            {
                UnityEngine.Debug.LogError("Save game slot not valid: " + saveSlot);
            }
        }

        #endregion

        public extern void orig_SetupSceneRefs();

        public void SetupSceneRefs()
        {
            orig_SetupSceneRefs();


            if (IsGameplayScene())
            {
                GameObject go = GameCameras.instance.soulOrbFSM.gameObject.transform.Find("SoulOrb_fill").gameObject;
                GameObject liquid = go.transform.Find("Liquid").gameObject;
                tk2dSpriteAnimator tk2dsa = liquid.GetComponent<tk2dSpriteAnimator>();
                tk2dsa.GetClipByName("Fill").fps = 15 * 1.05f;
                tk2dsa.GetClipByName("Idle").fps = 10 * 1.05f;
                tk2dsa.GetClipByName("Shrink").fps = 15 * 1.05f;
                tk2dsa.GetClipByName("Drain").fps = 30 * 1.05f;
            }

        }

        #region LoadGame

        [MonoModReplace]
        public bool LoadGame(int saveSlot)
        {
            if (saveSlot >= 0)
            {
                string saveFilename = this.GetSaveFilename(saveSlot);
                if (!string.IsNullOrEmpty(saveFilename) && File.Exists(Application.persistentDataPath + saveFilename))
                {
                    try
                    {
                        try
                        {
                            var path = ModdedSavePath(saveSlot);
                            if (File.Exists(path))
                            {
                                using FileStream modFileStream = File.OpenRead(path);
                                using var reader = new StreamReader(modFileStream);
                                string modJson = reader.ReadToEnd();
                                this.moddedData = JsonConvert.DeserializeObject<ModSavegameData>(
                                    modJson,
                                    new JsonSerializerSettings()
                                    {
                                        ContractResolver = ShouldSerializeContractResolver.Instance,
                                        TypeNameHandling = TypeNameHandling.Auto,
                                        ObjectCreationHandling = ObjectCreationHandling.Replace,
                                        Converters = JsonConverterTypes.ConverterTypes
                                    }
                                );
                                if (this.moddedData == null)
                                {
                                    Logger.APILogger.LogError($"Loaded mod savegame data deserialized to null: {modJson}");
                                    this.moddedData = new ModSavegameData();
                                }
                            } else
                            {
                                this.moddedData = new ModSavegameData();
                            }
                        } catch (Exception e)
                        {
                            Logger.APILogger.LogError(e);
                            this.moddedData = new ModSavegameData();
                        }
                        ModHooks.OnLoadLocalSettings(this.moddedData);

                        string toDecrypt = string.Empty;
                        string json = string.Empty;
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        FileStream fileStream = File.Open(Application.persistentDataPath + saveFilename, FileMode.Open);
                        if (this.gameConfig.useSaveEncryption)
                        {
                            toDecrypt = (string)binaryFormatter.Deserialize(fileStream);
                        } else
                        {
                            json = (string)binaryFormatter.Deserialize(fileStream);
                        }
                        fileStream.Close();
                        if (this.gameConfig.useSaveEncryption)
                        {
                            json = StringEncrypt.DecryptData(toDecrypt);
                        }
                        SaveGameData saveGameData = JsonUtility.FromJson<SaveGameData>(json);
                        global::PlayerData instance = saveGameData.playerData;
                        SceneData instance2 = saveGameData.sceneData;
                        global::PlayerData.instance = instance;
                        this.playerData = instance;
                        SceneData.instance = instance2;
                        this.sceneData = instance2;
                        this.profileID = saveSlot;
                        this.inputHandler.RefreshPlayerData();
                        return true;
                    } catch (Exception ex)
                    {
                        UnityEngine.Debug.LogFormat("Error loading save file for slot {0}: {1}", new object[]
                        {
                        saveSlot,
                        ex
                        });
                        return false;
                    }
                }
                UnityEngine.Debug.Log("Save file not found for slot " + saveSlot);
                return false;
            }
            UnityEngine.Debug.LogError("Save game slot not valid: " + saveSlot);
            return false;
        }

#endregion

        #region GetSaveStatsForSlot

        [MonoModReplace]
        public SaveStats GetSaveStatsForSlot(int saveSlot)
        {
            if (saveSlot > 0)
            {
                string saveFilename = this.GetSaveFilename(saveSlot);
                if (!string.IsNullOrEmpty(saveFilename) && File.Exists(Application.persistentDataPath + saveFilename))
                {
                    try
                    {
                        string toDecrypt = string.Empty;
                        string json = string.Empty;
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        FileStream fileStream = File.Open(Application.persistentDataPath + saveFilename, FileMode.Open);
                        if (this.gameConfig.useSaveEncryption)
                        {
                            toDecrypt = (string)binaryFormatter.Deserialize(fileStream);
                        } else
                        {
                            json = (string)binaryFormatter.Deserialize(fileStream);
                        }
                        fileStream.Close();
                        if (this.gameConfig.useSaveEncryption)
                        {
                            json = StringEncrypt.DecryptData(toDecrypt);
                        }

                        SaveGameData saveGameData;
                        try
                        {
                            saveGameData = JsonConvert.DeserializeObject<SaveGameData>(json, new JsonSerializerSettings()
                            {
                                ContractResolver = ShouldSerializeContractResolver.Instance,
                                TypeNameHandling = TypeNameHandling.Auto,
                                ObjectCreationHandling = ObjectCreationHandling.Replace,
                                Converters = JsonConverterTypes.ConverterTypes
                            });
                        } catch (Exception)
                        {
                            // Not a huge deal, this happens on saves with mod data which haven't been converted yet.
                            Logger.APILogger.LogWarn($"Failed to get save stats for slot {saveSlot} using Json.NET, falling back");

                            saveGameData = JsonUtility.FromJson<SaveGameData>(json);
                        }

                        global::PlayerData playerData = saveGameData.playerData;
                        return new SaveStats(playerData.maxHealthBase, playerData.geo, playerData.mapZone, playerData.playTime, playerData.MPReserveMax, playerData.permadeathMode, playerData.completionPercentage, playerData.unlockedCompletionRate);
                    } catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError(string.Concat(new object[]
                        {
                        "Error while loading save file for slot ",
                        saveSlot,
                        " Exception: ",
                        ex
                        }));
                        return null;
                    }
                }
                return null;
            }
            UnityEngine.Debug.LogError("Save game slot not valid: " + saveSlot);
            return null;
        }

        #endregion

        #region LoadSceneAdditive

        [MonoModIgnore]
        private bool tilemapDirty;

        [MonoModIgnore]
        private bool waitForManualLevelStart;

        [MonoModIgnore]
        public event GameManager.DestroyPooledObjects DestroyPersonalPools;

        [MonoModIgnore]
        public event GameManager.UnloadLevel UnloadingLevel;

        [MonoModIgnore]
        public event GameManager.LevelReady NextLevelReady;

        [MonoModIgnore]
        private extern void ManualLevelStart();

        [MonoModIgnore]
        public Scene nextScene { get; private set; }

        private IEnumerator OnBeforeAdditiveLoad(string scene)
        {
            foreach (var modInstance in ModLoader.ModInstances.Where(x => x.Mod != null))
            {
                float wait;
                try
                {
                    wait =  modInstance.Mod.BeforeAdditiveLoad(scene);
                }
                catch (Exception ex)
                {
                    Logger.APILogger.LogError($"Error in {nameof(modInstance.Mod.BeforeAdditiveLoad)}\n{ex}");
                    continue;
                }

                yield return new WaitForSeconds(wait);
            }
        }

        [MonoModReplace]
        public IEnumerator LoadSceneAdditive(string destScene)
        {
            Debug.Log("Loading " + destScene);
            IsInSceneTransition = true;
            destScene = ModHooks.BeforeSceneLoad(destScene);
            this.tilemapDirty = true;
            this.startedOnThisScene = false;
            this.nextSceneName = destScene;
            this.waitForManualLevelStart = true;
            if (this.DestroyPersonalPools != null)
            {
                this.DestroyPersonalPools();
            }
            if (this.UnloadingLevel != null)
            {
                this.UnloadingLevel();
            }
            string exitingScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            this.nextScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(destScene);
            yield return OnBeforeAdditiveLoad(destScene);
            AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(destScene, LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = true;
            yield return asyncOperation;
            UnityEngine.SceneManagement.SceneManager.UnloadScene(exitingScene);
            ModHooks.OnSceneChanged(destScene);
            IsInSceneTransition = false;
            this.RefreshTilemapInfo(destScene);
            this.ManualLevelStart();
            if (this.NextLevelReady != null)
            {
                this.NextLevelReady();
            }
            Debug.Log("Done Loading " + destScene);
            yield break;
        }

        #endregion

        #region LoadFirstScene

        [MonoModReplace]
        public IEnumerator LoadFirstScene()
        {
            yield return new WaitForEndOfFrame();
            this.entryGateName = "top1";
            this.SetState(GameState.PLAYING);
            this.ui.ConfigureMenu();
            this.LoadScene("Tutorial_01");
            ModHooks.OnNewGame();
            yield break;
        }

        #endregion

        #region PauseToDynamicMenu

        [MonoModIgnore]
        private bool timeSlowed;

        // code has been copied from PauseGameToggle
        public IEnumerator PauseToggleDynamicMenu(MenuScreen screen, bool allowUnpause = false)
        {
            if (!this.playerData.GetBool(nameof(PlayerData.disablePause)) && !this.timeSlowed)
            {
                if (this.gameState == GameState.PLAYING)
                {
                    this.gameCams.StopCameraShake();
                    this.inputHandler.PreventPause();
                    this.inputHandler.StopUIInput();
                    this.actorSnapshotPaused.TransitionTo(0f);
                    this.isPaused = true;
                    this.SetState(GameState.PAUSED);
                    this.ui.AudioGoToPauseMenu(0.2f);
                    this.ui.UIPauseToDynamicMenu(screen);
                    this.hero_ctrl.Pause();
                    this.UpdateSessionPlayTime();
                    this.gameCams.MoveMenuToHUDCamera();
                    this.SetTimeScale(0f);
                    yield return new WaitForSecondsRealtime(0.8f);
                    GC.Collect();
                    this.inputHandler.AllowPause();
                }
                else if (allowUnpause && this.gameState == GameState.PAUSED)
                {
                    this.gameCams.ResumeCameraShake();
                    this.inputHandler.PreventPause();
                    this.actorSnapshotUnpaused.TransitionTo(0f);
                    this.isPaused = false;
                    this.ui.AudioGoToGameplay(0.2f);
                    this.ui.SetState(UIState.PLAYING);
                    this.SetState(GameState.PLAYING);
                    this.hero_ctrl.UnPause();
                    this.intervalStartTime = Time.realtimeSinceStartup;
                    this.SetTimeScale(1f);
                    yield return new WaitForSecondsRealtime(0.8f);
                    this.inputHandler.AllowPause();
                }
            }
            yield break;
        }
        #endregion

        #region ReturnToMainMenu

        [MonoModReplace]
        public IEnumerator ReturnToMainMenu() => ReturnToMainMenu(true);

        public IEnumerator ReturnToMainMenu(bool save)
        {
            this.StoryRecord_quit();
            this.TimePasses();
            this.cameraCtrl.FreezeInPlace(true);
            this.cameraCtrl.FadeOut(CameraFadeType.JUST_FADE);
            yield return base.StartCoroutine(this.timeTool.TimeScaleIndependentWaitForSeconds(2f));
            this.ui.AudioGoToGameplay(0f);
            this.ResetSemiPersistentItems();
            if (save)
            {
                this.SaveGame(this.profileID);
            }
            if (this.hero_ctrl != null)
            {
                UnityEngine.Object.Destroy(this.hero_ctrl.gameObject);
            }
            UnityEngine.Object.Destroy(this.ui.gameObject);
            UnityEngine.Object.Destroy(this.gameCams.gameObject);
            this.playerData.Reset();
            this.sceneData.Reset();
            UnityEngine.Object.Destroy(base.gameObject);
            Time.timeScale = 1f;
            this.LoadScene("Menu_Title");
            yield break;
        }

        #endregion

        #region SetTimeScale

        [MonoModReplace]
        private IEnumerator SetTimeScale(float newTimeScale, float duration)
        {
            float lastTimeScale = TimeController.GenericTimeScale;
            for (float timer = 0f; timer < duration; timer += Time.unscaledDeltaTime)
            {
                float t = timer / duration;
                this.SetTimeScale(Mathf.Lerp(lastTimeScale, newTimeScale, t));
                yield return null;
            }
            yield break;
        }

        [MonoModReplace]
        private void SetTimeScale(float newTimeScale)
        {
            TimeController.GenericTimeScale = ((newTimeScale > 0.01f) ? newTimeScale : 0f);
        }

        #endregion

        private void UpdateUIStateFromGameState()
        {
            if (ui != null)
            {
                ui.SetUIStartState(gameState);
                return;
            }
            ui = FindObjectOfType<UIManager>();
            if (ui != null)
            {
                ui.SetUIStartState(gameState);
                return;
            }
            Debug.LogError("GM: Could not find the UI manager in this scene.");
        }

        [MonoModIgnore]
        private bool respawningHero;

        public bool RespawningHero
        {
            get => this.respawningHero;
            set => this.respawningHero = value;
        }

        public void RefreshOvercharm()
        {
            this.playerData.overcharmed = this.playerData.charmSlotsFilled > this.playerData.charmSlots;
        }
    }
}