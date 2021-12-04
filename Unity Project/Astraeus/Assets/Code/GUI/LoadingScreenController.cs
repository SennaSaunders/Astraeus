using System;
using System.Threading;
using UnityEngine;

namespace Code.GUI {
    public class LoadingScreenController : MonoBehaviour {
        // public void RunLoadingThread(Thread thread) {//needs a gui to switch to or a controller to use - make an interface like ILoadable that has a method to call when the thread is done
        //     thread.Start();
        // }
        // Update is called once per frame
        void Update() {
            transform.Rotate(new Vector3(0,30 ,0)* Time.deltaTime, Space.World);
        }
    }
}
