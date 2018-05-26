using UnityEngine;

namespace Game
{
    static public class Client
    {
        public static Transform SceneRoot { get; private set; }
        public static Transform UIRoot { get; private set; }
        public static Camera UICamera { get; private set; }
        public static Camera SceneCamera { get; private set; }

        public static void Init()
        {
            SceneRoot = GameObject.Find("SceneRoot").transform;
            UIRoot = GameObject.Find("UIRoot").transform;
            UICamera = GameObject.Find("UICamera").GetComponent<Camera>();
            SceneCamera = GameObject.Find("SceneCamera").GetComponent<Camera>();
        }
    }
}