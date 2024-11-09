#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace AssetVariants
{
    internal class AVUpdater
    {
        private static readonly List<AV> changedAVs = new List<AV>();

        public static int timestamp;

        [InitializeOnLoadMethod]
        private static void Init()
        {
            ObjectChangeEvents.changesPublished += ChangesPublished;

            EditorApplication.update += Update;

            Undo.undoRedoPerformed += () =>
            {
                undoTimestamp = timestamp;
            };
        }

        public static int undoTimestamp = int.MinValue;

        public static void ChangesPublished(ref ObjectChangeEventStream stream)
        {
            changedAVs.Clear();
            try
            {
                int l = stream.length;
                for (int i = 0; i < l; i++)
                {
                    if (stream.GetEventType(i) == ObjectChangeKind.ChangeAssetObjectProperties)
                    {
                        ChangeAssetObjectPropertiesEventArgs args;
                        stream.GetChangeAssetObjectPropertiesEvent(i, out args); //Debug.Log(args.guid + " - " + args.instanceId);

                        var guid = args.guid.ToString();
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        var assets = AssetDatabase.LoadAllAssetsAtPath(path);
                        for (int ii = 0; ii < assets.Length; ii++)
                        {
                            var av = AV.Open(assets[ii]);
                            if (av != null)
                                changedAVs.Add(av);
                        }
                        var importer = AssetImporter.GetAtPath(path);
                        if (importer != null && importer.GetType() != typeof(AssetImporter)) //TODO:?
                        {
                            var av = AV.Open(importer);
                            if (av != null)
                                changedAVs.Add(av);
                        }
                    }
                }

                for (int i = 0; i < AVTargets.all.Count; i++)
                {
                    var avTargets = AVTargets.all[i];
                    if (avTargets.AVs != null)
                    {
                        for (int ii = 0; ii < avTargets.AVs.Length; ii++)
                        {
                            var av = avTargets.AVs[ii];
                            if (changedAVs.Contains(av))
                            {
                                avTargets.TryImmediatelyPropagate();
                                break;
                            }
                        }
                    }
                }

                for (int i = 0; i < changedAVs.Count; i++)
                {
                    var av = changedAVs[i];
                    if (av.Valid())
                    {
                        int rt;
                        if (!AV.revertingTimestamps.TryGetValue(av.asset, out rt))
                            rt = int.MinValue;
                        //Debug.Log(rt + " - " + timestamp);
                        if (rt + 1 < timestamp && undoTimestamp + 1 < timestamp)
                        {
                            av.TryDirtyImplicit();
                            av.childrenNeedUpdate = true;
                        }
                        //else
                        //Debug.LogWarning("!!!");
                    }
                }
            }
            finally
            {
                for (int i = 0; i < changedAVs.Count; i++)
                    changedAVs[i].Dispose();
                changedAVs.Clear();
            }
        }

        public static void Update()
        {
            timestamp++;
        }
    }
}
#endif
