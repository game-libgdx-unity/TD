using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnitedSolution;using UnityEngine;

namespace UnitedSolution
{
    public class AssetBundleDownloader : SingletonBehaviour<AssetBundleDownloader>
    {
        [Serializable]
        public class BundleInfo
        {
            public string Name;
            public bool IsRequired;
        } 

        public event Action<string> OnAssetBundleDownloader_LoadComplete;
        public event Action<Exception> OnAssetBundleDownloader_LoadFail;
        public event Action OnAssetBundleDownloader_LoadRequiredComplete;

        public string HostUrl;
        public string FilePath;

        public BundleInfo[] BundleInfos;

        public AssetBundle this[string bundleName]
        {
            get
            {
                if (bundles.ContainsKey(bundleName))
                {
                    return bundles[bundleName];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (bundles.ContainsKey(bundleName))
                {
                    bundles[bundleName] = value;
                }
                else
                {
                    bundles.Add(bundleName, value);
                }
            }
        }

        protected bool enableCacheMode = true;
        public bool EnableCacheMode
        {
            get
            {
                return enableCacheMode;
            }
            set
            {
                enableCacheMode = value;
            }
        }

        protected Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();

        protected int requiredBundleLoadCount;

        void Start()
        {
            OnAssetBundleDownloader_LoadComplete += HandleAssetBundleDownloader_LoadComplete;
        }

        public void Initialize(string downloadUrl, string filePath)
        {
            HostUrl = downloadUrl;
            FilePath = filePath;
        }

        private void HandleAssetBundleDownloader_LoadComplete(string assetBundle)
        {
            var requiredBundleCount = BundleInfos.Count(bi => bi.IsRequired);
            if (requiredBundleLoadCount < requiredBundleCount)
            {
                requiredBundleLoadCount++;
                if (requiredBundleLoadCount == requiredBundleCount)
                {
                    if (OnAssetBundleDownloader_LoadRequiredComplete != null)
                    {
                        OnAssetBundleDownloader_LoadRequiredComplete();
                    }
                }
            }
        }

        /// <summary>
        /// Start download all asset bundles
        /// Only call after register success
        /// </summary>
        public void StartDownloadRequiredBundles()
        {
            StartCoroutine(DoDownload(BundleInfos
                                        .Where(bi => bi.IsRequired)
                                        .Select(bi => bi.Name)
                                        .ToArray()));
        }

        public void StartDownloadOptionalBundles()
        {
            StartCoroutine(DoDownload(BundleInfos
                                        .Where(bi => !bi.IsRequired)
                                        .Select(bi => bi.Name)
                                        .ToArray()));
        }

        protected IEnumerator DoDownload(params string[] bundleNames)
        {
            yield return null;

            string filePath;
            byte[] data;
            for (int index = 0; index < bundleNames.Length; index++)
            {
                filePath = Path.Combine(FilePath, bundleNames[index]);
                Debug.Log("File Path: " + filePath);
                // Load from cache
                if (enableCacheMode && File.Exists(filePath))
                {
                    Debug.Log("File Path Exist!!!!!");
                    using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                    {
                        data = reader.ReadBytes((int)reader.BaseStream.Length);
                        reader.Close();
                    }
                }
                // Download and save to cache
                else
                {
                    string fullPath = HostUrl + "/" + bundleNames[index];
                    Debug.Log("File Path NOT Exist!!!!! " + fullPath);

                    WWW encryptedBundle = new WWW(fullPath);

                    yield return encryptedBundle;
                    if (encryptedBundle.error != null)
                    {
                        if (OnAssetBundleDownloader_LoadFail != null)
                        {
                            OnAssetBundleDownloader_LoadFail(new Exception(encryptedBundle.error));
                        }
                        yield break;
                    }

                    data = encryptedBundle.bytes;

                    if (enableCacheMode)
                    {
                        using (var stream = File.Open(filePath, FileMode.OpenOrCreate))
                        {
                            stream.Write(data, 0, data.Length);
                            stream.Close();
                        }
                    }
                }

                new Thread(new ParameterizedThreadStart(DecryptBundle))
                                .Start(new KeyValuePair<string, byte[]>(bundleNames[index], data));
            }
        }

        /// <summary>
        /// Decrypt asset bundle
        /// </summary>
        /// <param name="bundlePairObject"></param>
        protected void DecryptBundle(object bundlePairObject)
        {
            var bundlePair = (KeyValuePair<string, byte[]>)bundlePairObject;
            var bundleName = bundlePair.Key;
            var byteData = bundlePair.Value;
            try
            {
                UIThreadInvoker.Instance.Invoke(() =>
                {
                    StartCoroutine(CreateBundleInMemory(bundleName, byteData));
                });
            }
            catch (Exception ex)
            {
                if (OnAssetBundleDownloader_LoadFail != null)
                {
                    OnAssetBundleDownloader_LoadFail(ex);
                }
            }
        }

        /// <summary>
        /// Create bundle in memory for used
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="rawData"></param>
        /// <returns></returns>
        protected IEnumerator CreateBundleInMemory(string bundleName, byte[] rawData)
        {
            AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(rawData);

            yield return assetBundleCreateRequest;

            this[bundleName] = assetBundleCreateRequest.assetBundle;

            yield return null;

            if (OnAssetBundleDownloader_LoadComplete != null)
            {
                OnAssetBundleDownloader_LoadComplete(bundleName);
            }
        }

        public void UnLoadAllBundles()
        {
            foreach (var pair in bundles)
            {
                if (pair.Value != null)
                {
                    pair.Value.Unload(true);
                }
            }
        }
    }
}
