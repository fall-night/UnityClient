using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class LoadSceneHandler : MonoBehaviour
    {
        private string _sceneName;
        private bool _signal;
        
        public void Load(string sceneName)
        {
            this._signal = true;
            this._sceneName = sceneName;
        }

        private void Awake()
        {
            this._signal = false;
            this._sceneName = "";
        }
        
        private void Update()
        {
            if (!this._signal) return;
            
            SceneManager.LoadScene(this._sceneName);
        }
    }
}