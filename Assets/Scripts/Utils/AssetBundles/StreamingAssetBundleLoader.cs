using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitedSolution;using UnityEngine;

namespace UnitedSolution
{
    public class StreamingAssetBundleLoader : SingletonBehaviour<StreamingAssetBundleLoader>
    { 
        public event Action<string> OnAssetBundle_Ready;

        private Dictionary<string, AssetBundle> bundleDict = new Dictionary<string, AssetBundle>();

        public string[] BundleNames
        {
            get
            {
                return bundleDict.Keys.ToArray();
            }
        }

        public AssetBundle this[string bundleName]
        {
            get
            {
                if (bundleDict.ContainsKey(bundleName))
                {
                    return bundleDict[bundleName];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (bundleDict.ContainsKey(bundleName))
                {
                    bundleDict[bundleName] = value;
                }
                else
                {
                    bundleDict.Add(bundleName, value);

                    if (OnAssetBundle_Ready != null)
                    {
                        OnAssetBundle_Ready(bundleName);
                    }
                }
            }
        }

        public virtual void Load<T>(string bundleName, string assetName, Action<T> onDone) where T : UnityEngine.Object
        {
            if (this[bundleName] == null) return;

            var request = this[bundleName].LoadAssetAsync<T>(assetName);

            if (request == null) return;

            Run.Coroutine(DoLoad(request), () =>
            {
                // Get the asset.
                var go = request.asset;

                if (go != null)
                {
                    onDone(go as T);
                }
                else
                {
                    onDone(null);
                }
            });
        }

        private IEnumerator DoLoad(AssetBundleRequest request)
        {
            yield return request;
        }

        public virtual void UnloadBundles()
        {
        }
    }
}
