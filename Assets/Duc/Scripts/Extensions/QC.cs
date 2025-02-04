using UnityEngine;
using DG.Tweening;
using System.IO;
using UnityEngine.SceneManagement;

public class QC : MonoBehaviour
{
    public static QC instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(this);
    }

    bool isAllowKeyCode = false;
    void Update()
    {
        if (isAllowKeyCode) return;
        

        if (!isAllowKeyCode && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.F))
        {
            isAllowKeyCode = true;
            Screen.SetResolution(1920, 1080, true);
            DOVirtual.DelayedCall(0.5f, delegate { isAllowKeyCode = false; });
            return;
        }
        if (!isAllowKeyCode && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.G))
        {
            isAllowKeyCode = true;
            Screen.SetResolution(1080, 1920, true);
            DOVirtual.DelayedCall(0.5f, delegate { isAllowKeyCode = false; });
            return;
        }
        if (!isAllowKeyCode && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.V))
        {
            isAllowKeyCode = true;
            Screen.SetResolution(1080, 1080, true);
            DOVirtual.DelayedCall(0.5f, delegate { isAllowKeyCode = false; });
            return;
        }
        if (!isAllowKeyCode && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.N))
        {
            isAllowKeyCode = true;
            Screen.SetResolution(1350, 1080, true);
            DOVirtual.DelayedCall(0.5f, delegate { isAllowKeyCode = false; });
            return;
        }
        if (!isAllowKeyCode && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.B))
        {
            isAllowKeyCode = true;
            Screen.SetResolution(1080, 1350, true);
            DOVirtual.DelayedCall(0.5f, delegate { isAllowKeyCode = false; });
            return;
        }
        if (!isAllowKeyCode && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.C))
        {
            isAllowKeyCode = true;
            PlayerPrefs.DeleteAll();
            Debug.Log("Clear all PlayerPrefs");
            var paths = Directory.GetFiles(Application.persistentDataPath);
            foreach (var path in paths)
            {
                if (path.Contains(".log")) continue;
                File.Delete(path);
                Debug.Log(string.Format("Clear file at: {0}", path));
            }
            paths = Directory.GetDirectories(Application.persistentDataPath);
            foreach (var path in paths)
            {
                if (path.Contains(".log")) continue;
                Directory.Delete(path, true);
                Debug.Log(string.Format("Clear Folder at: {0}", path));
            }
            DOVirtual.DelayedCall(0.5f, delegate { isAllowKeyCode = false; SceneManager.LoadScene(1); });
            return;
        }


        if (!isAllowKeyCode && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.F))
        {
            isAllowKeyCode = true;
            Screen.SetResolution(1920, 1080, false);
            DOVirtual.DelayedCall(0.5f, delegate { isAllowKeyCode = false; });
            return;
        }
        if (!isAllowKeyCode && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.G))
        {
            isAllowKeyCode = true;
            Screen.SetResolution(1080, 1920, false);
            DOVirtual.DelayedCall(0.5f, delegate { isAllowKeyCode = false; });
            return;
        }
        if (!isAllowKeyCode && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.V))
        {
            isAllowKeyCode = true;
            Screen.SetResolution(1080, 1080, false);
            DOVirtual.DelayedCall(0.5f, delegate { isAllowKeyCode = false; });
            return;
        }
        if (!isAllowKeyCode && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.N))
        {
            isAllowKeyCode = true;
            Screen.SetResolution(1350, 1080, false);
            DOVirtual.DelayedCall(0.5f, delegate { isAllowKeyCode = false; });
            return;
        }
        if (!isAllowKeyCode && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.B))
        {
            isAllowKeyCode = true;
            Screen.SetResolution(1080, 1350, false);
            DOVirtual.DelayedCall(0.5f, delegate { isAllowKeyCode = false; });
            return;
        }
    }
}
