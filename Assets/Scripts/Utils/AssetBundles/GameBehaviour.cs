using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnitedSolution;
using UnityEngine.SceneManagement;

public class GameBehaviour : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

        AssetBundleDownloader.Instance.EnableCacheMode = false;

        AssetBundleDownloader.Instance.Initialize("file://" + Application.streamingAssetsPath, "aaa");

        var requiredBundles = AssetBundleDownloader.Instance.BundleInfos.Where(bi => bi.IsRequired).ToArray();
        IDictionary<string, float> bundlesDownloadedSize = new Dictionary<string, float>(requiredBundles.Length);
        List<string> bundles = new List<string>(requiredBundles.Length);
        foreach (var bundle in requiredBundles)
        {
            bundlesDownloadedSize.Add(bundle.Name, 0);
            bundles.Add(bundle.Name);
        }
        //AssetBundleDownloader.Instance.OnAssetBundleDownloader_DownloadProgress += ((bundleName, bundleProgress, totalBundlesSize) =>
        //{
        //    Debug.Log(bundleName + " _ " + bundleProgress);
        //        //Caculate download progress
        //        if (bundlesDownloadedSize.ContainsKey(bundleName))
        //    {
        //        bundlesDownloadedSize[bundleName] = bundleProgress * bundlesSize[bundleName];
        //    }
        //    float totalDownloadedSize = bundlesDownloadedSize.Sum(p => p.Value);

        //    LoadingPanel.Instance.UpdateDownloadProgress(totalDownloadedSize / totalBundlesSize);
        //        //Caculate downloaded size
        //        LoadingPanel.Instance.UpdateDownloadedSize((long)totalDownloadedSize, totalBundlesSize);
        //        //Caculate download speed
        //        float interval = (float)(DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds - lastTimeUpdateProgress);
        //    LoadingPanel.Instance.UpdateDownloadSpeed(totalDownloadedSize / interval);
        //});

        AssetBundleDownloader.Instance.OnAssetBundleDownloader_LoadRequiredComplete += () =>
        {
            //StartGame();
            Debug.Log("Download bundle aaa successfully");
            SceneManager.LoadScene("TestObjectPool");

        };
        AssetBundleDownloader.Instance.OnAssetBundleDownloader_LoadFail += HandleAssetBundleDownloader_LoadFail;
        AssetBundleDownloader.Instance.StartDownloadRequiredBundles();
        //AssetBundleDownloader.Instance.OnAssetBundleDownloader_LoadRequiredComplete += StartGame;
    }

    private void HandleAssetBundleDownloader_LoadFail(Exception obj)
    {
    }
}
