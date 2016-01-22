using ICities;
using SpawnPositionInverter.Detour;
using UnityEngine;

namespace SpawnPositionInverter
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private static GameObject _gameObject;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            CargoStationAIDetour.Deploy();
        }

        public override void OnReleased()
        {
            base.OnReleased();
            CargoStationAIDetour.Revert();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame)
            {
                return;
            }
            if (_gameObject != null)
            {
                return;
            }
            _gameObject = new GameObject("SpawnPositionInverterPanelExtender");
            _gameObject.AddComponent<GamePanelExtender>();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            Configuration.Reset();
            if (_gameObject == null)
            {
                return;
            }
            Object.Destroy(_gameObject);
            _gameObject = null;
        } 
    }
}