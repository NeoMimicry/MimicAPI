using MelonLoader;
using MimicAPI.TestMod;
using UnityEngine;

[assembly: MelonInfo(typeof(Loader), "MimicAPI.TestMod", "0.2.0", "MimicAPI")]
[assembly: MelonGame("ReLUGames", "MIMESIS")]

namespace MimicAPI.TestMod
{
    public class Loader : MelonMod
    {
        private GameObject gui;

        public override void OnInitializeMelon() { }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (gui == null)
            {
                gui = new GameObject(nameof(TestGUI));
                gui.AddComponent<TestGUI>();
                GameObject.DontDestroyOnLoad(gui);
            }
        }
    }
}
