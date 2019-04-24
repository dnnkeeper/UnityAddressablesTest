using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AddressablesTest
{
    [System.Serializable]
    public class UnityEventFloat : UnityEvent<float>
    { }

    [System.Serializable]
    public class UnityEventString : UnityEvent<string>
    { }
    public class AddressableSceneLoader : MonoBehaviour
    {
        public string assetName = "NewScene";

        public LoadSceneMode loadSceneMode = LoadSceneMode.Single;

        AsyncOperationHandle<SceneInstance> downloadOperation;

        public UnityEvent OnStart;

        public UnityEventFloat onProgress;

        public UnityEvent onSuccess;

        public UnityEvent onFalied;

        public UnityEventString onInfoUpdate;

        public UnityEventString onProgressInfoUpdate;

        private Coroutine loadRoutine = null;

        float _progress;

        public float progress
        {
            get { return _progress; }
            set
            {
                if (_progress != value)
                {
                    _progress = value;

                    onProgress.Invoke(_progress);

                    onProgressInfoUpdate.Invoke((_progress * 100f).ToString()+"%");
                }
            }
        }

        public void ToggleLoading()
        {
            if (loadRoutine != null)
            {
                StopLoading();
            }
            else
            {
                StartLoading();
            }
        }

        public void Load(string sceneName)
        {
            this.assetName = sceneName;
            StartLoading();
        }

        public void StartLoading()
        {
            Debug.Log("StartLoading");

            if (loadRoutine != null)
            {
                StopLoading();
            }

            loadRoutine = StartCoroutine("LoadRoutine");
        }
        public void StopLoading()
        {
            Debug.Log("StopLoading");
            if (loadRoutine != null)
            {
                StopCoroutine(loadRoutine);
            }
        }

        private void OnDisable()
        {
            if (downloadOperation.IsValid())
            {
                Addressables.Release(downloadOperation);
            }
        }

        void InfoUpdate(string message)
        {
            Debug.Log(message, this);
            onInfoUpdate.Invoke(message);
        }

        IEnumerator LoadRoutine()
        {
            OnStart.Invoke();

            yield return null;

            if (downloadOperation.IsValid())
            {
                Addressables.Release(downloadOperation);
            }

            var resourceLocationDownload = Addressables.LoadResourceLocations(assetName);

            InfoUpdate("Find " + assetName);

            while (resourceLocationDownload.IsValid() && !resourceLocationDownload.IsDone)
            {
                Debug.Log("Awaiting asset location... " + resourceLocationDownload.PercentComplete * 100f);

                yield return null;
            }

            Debug.Log("resourceLocationDownload.IsDone: " + resourceLocationDownload.IsDone);

            Debug.Log("resourceLocationDownload.Status: " + resourceLocationDownload.Status);

            if (resourceLocationDownload.Status != AsyncOperationStatus.Succeeded)
            {
                var message = "Asset location for " + assetName + " has not been found!";
                InfoUpdate(message);
                yield break;
            }
            else
            {
                var message = "Asset location found! " + resourceLocationDownload.Result[0];
                InfoUpdate(message);
            }

            InfoUpdate("Downloading " + assetName);

            downloadOperation = Addressables.LoadScene(assetName, loadSceneMode, false);

            downloadOperation.Completed += DownloadOperation_Completed;

            while (downloadOperation.IsValid() && !downloadOperation.IsDone)
            {
                progress = downloadOperation.PercentComplete;

                yield return null;
            }

            progress = downloadOperation.PercentComplete;

            Debug.Log("Download operation is done ");

            yield return null;
        }


        private void DownloadOperation_Completed(AsyncOperationHandle<SceneInstance> operation)
        {
            if (operation.Status != AsyncOperationStatus.Succeeded)
            {
                onFalied.Invoke();

                var stat = "Downloading " + assetName + " FAILED! operation.Status: " + operation.Status.ToString();

                InfoUpdate(stat);
            }
            else
            {
                onSuccess.Invoke();

                var message = "Downloading '" + assetName + "' " + operation.Status.ToString();

                InfoUpdate(message);

                operation.Result.Activate();
            }
        }

    }
}