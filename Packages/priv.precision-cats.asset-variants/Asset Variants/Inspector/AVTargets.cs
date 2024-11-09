#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;
using S = AssetVariants.AVSettings;

namespace AssetVariants
{
    internal class AVTargets
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            AssemblyReloadEvents.beforeAssemblyReload += () =>
            {
                for (int i = 0; i < all.Count; i++)
                    all[i].users.list.Clear();
                CleanupUsers();
            };

            //////// To prevent implicit overrides from being created before it can flush changes
            //////Selection.selectionChanged += () =>
            //////{
            //////    CleanupUsers();
            //////};

            EditorApplication.update += Update;
        }

        private readonly WeakList<SerializedObject> users = new WeakList<SerializedObject>();
        private static readonly FieldInfo nativeObjectPtrFI = typeof(SerializedObject).GetField("m_NativeObjectPtr", R.npi);
        public static void CleanupUsers()
        {
            for (int i = all.Count - 1; i >= 0; i--)
            {
                var avTargets = all[i];

                var users = avTargets.users.list;
                for (int ii = users.Count - 1; ii >= 0; ii--)
                {
                    var user = users[ii];
                    SerializedObject so;
                    if (!user.TryGetTarget(out so) || (IntPtr)nativeObjectPtrFI.GetValue(so) == IntPtr.Zero)
                        users.RemoveAt(ii);
                }

                if (avTargets.users.list.Count == 0)
                {
                    all.RemoveAt(i);

                    if (avTargets.AVs != null)
                    {
                        for (int ii = 0; ii < avTargets.AVs.Length; ii++)
                            avTargets.AVs[ii].Dispose(); // Decrements UserCount
                        avTargets.AVs = null;
                    }
                }
            }
        }

        public void UpdateAnyHasParent()
        {
            AnyHasParent = false;
            if (AVs != null)
            {
                for (int i = 0; i < AVs.Length; i++)
                {
                    if (AVs[i].data.HasParent)
                    {
                        AnyHasParent = true;
                        break;
                    }
                }
            }
        }

        public static AVTargets Get(SerializedObject so, bool reloadNullAVs = false)
        {
            AVTargets avTargets = null;
            for (int i = 0; i < all.Count; i++)
            {
                var a = all[i];
                if (a.users.Contains(so))
                {
                    if (a.AVs != null || !reloadNullAVs)
                        return a;

                    avTargets = a;
                    break;
                }
            }

            var targets = so.targetObjects;

            if (avTargets == null)
            {
                for (int i = 0; i < all.Count; i++)
                {
                    if (AVCommonHelper.TargetsAreEqual(all[i].targets, targets))
                    {
                        avTargets = all[i];
                        break;
                    }
                }

                if (avTargets == null)
                {
                    avTargets = new AVTargets(targets, null);
                    all.Add(avTargets);
                }

                avTargets.users.Add(so);
            }

            if (avTargets.AVs == null)
            {
                if (EditorApplication.isUpdating)
                    AV.skipValidateRelations = true;

                var avs = new AV[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    avs[i] = AV.Open(targets[i]); // Increments UserCount
                    if (avs[i] == null)
                    {
                        for (int ii = 0; ii < i; ii++)
                            avs[ii].Dispose();
                        avs = null;
                        break;
                    }

                    avs[i].ValidateRelations();
                }
                avTargets.AVs = avs;

                avTargets.UpdateAnyHasParent();

                AV.skipValidateRelations = false;
            }

            return avTargets;
        }

        public bool AnyHasParent { get; set; }

        public bool AnyMissingPath(string path)
        {
            for (int i = 0; i < AVs.Length; i++)
            {
                if (!AVs[i].GetOverridesCache().Contains(path))
                    return true;
            }
            return false;
        }
        public bool AnyContainsPath(string path)
        {
            for (int i = 0; i < AVs.Length; i++)
            {
                if (AVs[i].GetOverridesCache().Contains(path))
                    return true;
            }
            return false;
        }
        public bool AnyContainsChildrenPaths(string path, bool includeSelf)
        {
            for (int i = 0; i < AVs.Length; i++)
            {
                var oc = AVs[i].GetOverridesCache();
                if (oc.FastContainsChildrenPaths(path, includeSelf))
                    return true;
            }
            return false;
        }

        internal static readonly List<AVTargets> all = new List<AVTargets>();

        private AVTargets(Object[] targets, AV[] avs)
        {
            this.targets = targets;

            AVs = avs;

            var targetType = targets[0].GetType();
            propertyFilter = PropertyFilter.Get(targetType);
        }
        ~AVTargets()
        {
            int userCount = users.list.Count;
            if (userCount != 0)
                Debug.LogError("AVTargets - (userCount != 0) - Implementation bug, fix it Steffen - userCount: " + userCount + Helper.LOG_END);
        }

        public readonly Object[] targets;
        public AV[] AVs { get; private set; }
        public readonly PropertyFilter propertyFilter;

        private static readonly List<AV> descendantAVs = new List<AV>();
        private static void FindDescendantChain(AV descendantAV, AV av)
        {
            if (descendantAV.UserCount > 0)
            {
                var search = descendantAV;
                search.IncrementUserCount();
                while (search.Valid() && search != av)
                {
                    // Remove previous duplicates
                    while (descendantAVs.Remove(search)) { }

                    descendantAVs.Add(search);

                    search = AV.Open(search.LoadParentAsset());
                    if (search == null)
                    {
                        Debug.LogError("Traversed through " + descendantAV.asset.name + "'s ancestors and couldn't find " + av.asset.name + Helper.LOG_END);
                        return;
                    }
                }
                search.Dispose();
            }
            else
                Debug.LogWarning("?");
        }
        public void ImmediatelyPropagate()
        {
            descendantAVs.Clear();

            for (int i = 0; i < AVs.Length; i++)
            {
                var av = AVs[i];

                var descendants = av.GetDescendants();

                for (int ii = 0; ii < all.Count; ii++)
                {
                    var avTargets = all[ii];
                    if (avTargets != this)
                    {
                        for (int iii = 0; iii < avTargets.targets.Length; iii++)
                        {
                            if (descendants.Contains(avTargets.targets[iii]))
                                FindDescendantChain(avTargets.AVs[iii], av);
                        }
                    }
                }

                av.childrenNeedUpdate = true; // (Just for safety in case this is called elsewhere)
            }

            // Inverse order so that parents revert before children
            for (int i = descendantAVs.Count - 1; i >= 0; i--)
            {
                var descendantAV = descendantAVs[i];

                // This is fine because it's a descendant of an AV that certainly is childrenNeedUpdate=true.
                // This prevents dAV from immediately propagating to most/all of its descendants if it's only a middleman from av to descendantAV, with no previous UserCount.
                // That means only the necessary sequences of descendants get updated for now.
                bool prevChildrenNeedUpdate = descendantAV.childrenNeedUpdate;
                {
                    descendantAV.RevertAssetFromParentAsset();
                }
                descendantAV.childrenNeedUpdate = prevChildrenNeedUpdate;

                descendantAV.Dispose();
            }

            if ((descendantAVs.Count > 0) && Helper.LogUnnecessary)
                //Debug.Log(AVs[0].asset.name + " - " + AVUpdater.timestamp + "Immediately propagated to " + descendantAVs.Count + " necessary descendants." + Helper.LOG_END);
                Debug.Log("Immediately propagated to " + descendantAVs.Count + " necessary descendants." + Helper.LOG_END);

            descendantAVs.Clear();
        }
        public void TryImmediatelyPropagate()
        {
            if (S.S.immediatelyPropagateToOpenDescendants)
                ImmediatelyPropagate();
        }

        private static void Update()
        {
#if DEBUG_ASSET_VARIANTS
            string log = "";
            for (int i = 0; i < all.Count; i++)
            {
                if (i != 0) log += " + ";
                log += all[i].UserCount;
            }
            Debug.Log(users.Count + " = " + log + Helper.LOG_END);
#endif

            CleanupUsers();
        }

        public void SaveAllUserData()
        {
            Profiler.BeginSample("SaveUserDataWithUndo");
            for (int i = 0; i < AVs.Length; i++)
                AVs[i].SaveUserDataWithUndo();
            Profiler.EndSample();
        }

        public int GetOverriddenType(string path)
        {
            int overrideCount = 0;
            bool mixed = false;
            for (int i = 0; i < AVs.Length; i++)
            {
                if (!AVs[i].data.HasParent)
                {
                    mixed = true;
                    break;
                }
                if (AVs[i].GetOverridesCache().Contains(path))
                    overrideCount++;
                if (overrideCount != 0 && overrideCount != (i + 1))
                {
                    mixed = true;
                    break;
                }
            }

            if (mixed)
            {
                return -1;
            }
            else if (overrideCount == 0)
            {
                for (int i = 0; i < AVs.Length; i++)
                {
                    if (AVs[i].overridesCache.parentPaths.Contains(path))
                        return 2;
                }
                return 0;
            }
            else
            {
                return 1;
            }
        }
        public void ToggleOverriddenState(string path, bool overridden)
        {
            if (overridden)
            {
                if (Helper.LogUnnecessary)
                    Debug.Log("Removing override for: " + path + Helper.LOG_END);
            }
            else
            {
                if (Helper.LogUnnecessary)
                    Debug.Log("Creating override for: " + path + Helper.LOG_END);
            }

            for (int i = 0; i < AVs.Length; i++)
            {
                if (AVs[i].data.HasParent)
                {
                    var oc = AVs[i].GetOverridesCache();

                    oc.Remove(path);

                    if (!overridden) // Invert the current state
                    {
                        // Creating a new override
                        if (!S.currentlyAllowSubOverrides())
                            oc.RemoveChildrenPaths(path);
                        oc.Add(path);
                    }
                }
                else
                    Debug.LogWarning("Tried to set override state of property in non-variant asset." + Helper.LOG_END);
            }

            SaveAllUserData();

            if (RawViewWindow.CurrentlyDrawing)
                OverrideIndicator.OverridesMaybeChanged();

            if (overridden)
            {
                // If was overridden but no longer is, needs to revert
                Profiler.BeginSample("RevertAssetFromParentAsset");
                for (int i = 0; i < AVs.Length; i++)
                {
                    var av = AVs[i];
                    if (av.asset == null)
                    {
                        Debug.LogError(av.assetID.guid.ToPath() + " couldn't RevertAssetFromParentAsset() because its asset is reimporting." + Helper.LOG_END);
                        continue;
                    }
                    if (av.data.HasParent)
                        av.RevertAssetFromParentAsset(); //EditorUtility.SetDirty(targets[i]);

                    av.childrenNeedUpdate = true;
                }
                Profiler.EndSample();

                TryImmediatelyPropagate();

                UnityEditorInternal.InternalEditorUtility.RepaintAllViews(); //TODO: does this need to be in this if scope?
            }
        }

        //TODO: remove?
        public bool TryCreateImplicitOverride(string path)
        {
            bool dirty = false;

            for (int i = 0; i < AVs.Length; i++)
            {
                if (AVs[i].data.HasParent)
                {
                    var oc = AVs[i].GetOverridesCache();
                    if (!oc.IsOverridden(path))
                    {
                        oc.Add(path);
                        dirty = true;
                    }
                }
            }

            if (dirty)
            {
                if (Helper.LogUnnecessary)
                    Debug.Log("Implicitly created override for: " + path + Helper.LOG_END);

                SaveAllUserData();
            }

            return dirty;
        }
    }
}
#endif
