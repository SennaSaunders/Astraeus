using TMPro;
using UnityEngine;

namespace Code.GUI.Loading {
    public class LoadingScreenController : MonoBehaviour {
        private GameObject _loadingIcon;

        private void Start() {
            _loadingIcon = GameObject.Find("LoadingIcon");
        }

        // Update is called once per frame
        private void Update() {
            _loadingIcon.transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime, Space.World);
        }

        public static void SetLoadingText(string text) {
            GameObject loadingTextObj = GameObject.Find("LoadingTxt");
            loadingTextObj.GetComponent<TextMeshProUGUI>().text = text;
        }
    }
}