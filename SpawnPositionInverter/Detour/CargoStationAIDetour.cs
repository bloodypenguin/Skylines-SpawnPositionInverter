using System;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Math;
using UnityEngine;

namespace SpawnPositionInverter.Detour
{
    public class CargoStationAIDetour : CargoStationAI
    {
        private static RedirectCallsState _state1;
        private static RedirectCallsState _state2;

        private static readonly MethodInfo Method1 = typeof(CargoStationAI).GetMethod("CalculateSpawnPosition", new[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Randomizer).MakeByRefType(), typeof(VehicleInfo), typeof(Vector3).MakeByRefType(), typeof(Vector3).MakeByRefType() });
        private static readonly MethodInfo Method2 = typeof(CargoStationAI).GetMethod("CalculateUnspawnPosition", new[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Randomizer).MakeByRefType(), typeof(VehicleInfo), typeof(Vector3).MakeByRefType(), typeof(Vector3).MakeByRefType() });
        private static readonly MethodInfo Detour1 = typeof(CargoStationAIDetour).GetMethod("CalculateSpawnPosition", new[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Randomizer).MakeByRefType(), typeof(VehicleInfo), typeof(Vector3).MakeByRefType(), typeof(Vector3).MakeByRefType() });
        private static readonly MethodInfo Detour2 = typeof(CargoStationAIDetour).GetMethod("CalculateUnspawnPosition", new[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Randomizer).MakeByRefType(), typeof(VehicleInfo), typeof(Vector3).MakeByRefType(), typeof(Vector3).MakeByRefType() });


        private static bool _deployed;

        public static void Deploy()
        {
            if (_deployed)
            {
                return;
            }
            try
            {
                _state1 = RedirectionHelper.RedirectCalls(Method1, Detour1);
                _state2 = RedirectionHelper.RedirectCalls(Method2, Detour2);

            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            _deployed = true;
        }

        public static void Revert()
        {
            if (!_deployed)
            {
                return;
            }
            try
            {
                RedirectionHelper.RevertRedirect(Method1, _state1);
                RedirectionHelper.RevertRedirect(Method2, _state2);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            _deployed = false;
        }


        public override void CalculateSpawnPosition(ushort buildingID, ref Building data, ref Randomizer randomizer, VehicleInfo info, out Vector3 position, out Vector3 target)
        {
            if (info.m_vehicleType == VehicleInfo.VehicleType.Car)
            {
                if (info.m_class.m_service == ItemClass.Service.Industrial)
                {
                    if (InvertPositions(data))
                    {
                        position = data.CalculatePosition(m_truckUnspawnPosition);
                        target = data.CalculateSidewalkPosition(m_truckUnspawnPosition.x, 0.0f);
                    }
                    else
                    {
                        position = data.CalculatePosition(m_truckSpawnPosition);
                        target = data.CalculateSidewalkPosition(m_truckSpawnPosition.x, 0.0f);
                    }
                }
                else
                    BaseCalculateSpawnPosition(buildingID, ref data, ref randomizer, info, out position, out target);
            }
            else if (m_transportInfo != null && info.m_vehicleType == m_transportInfo.m_vehicleType)
            {
                position = data.CalculatePosition(m_spawnPosition);
                if (m_canInvertTarget && Singleton<SimulationManager>.instance.m_metaData.m_invertTraffic == SimulationMetaData.MetaBool.True)
                    target = data.CalculatePosition(m_spawnPosition * 2f - m_spawnTarget);
                else
                    target = data.CalculatePosition(m_spawnTarget);
            }
            else if (m_transportInfo2 != null && info.m_vehicleType == m_transportInfo2.m_vehicleType)
            {
                position = data.CalculatePosition(m_spawnPosition2);
                if (m_canInvertTarget && Singleton<SimulationManager>.instance.m_metaData.m_invertTraffic == SimulationMetaData.MetaBool.True)
                    target = data.CalculatePosition(m_spawnPosition2 * 2f - m_spawnTarget2);
                else
                    target = data.CalculatePosition(m_spawnTarget2);
            }
            else
                BaseCalculateSpawnPosition(buildingID, ref data, ref randomizer, info, out position, out target);
        }

        //private static int index = 0;

        public override void CalculateUnspawnPosition(ushort buildingID, ref Building data, ref Randomizer randomizer, VehicleInfo info, out Vector3 position, out Vector3 target)
        {
            if (info.m_vehicleType == VehicleInfo.VehicleType.Car)
            {
                if (info.m_class.m_service == ItemClass.Service.Industrial)
                {
                    if (InvertPositions(data) /*|| index % 2 == 1*/)
                    {
                        position = data.CalculatePosition(m_truckSpawnPosition);
                        target = data.CalculateSidewalkPosition(m_truckSpawnPosition.x, 0.0f);
                    }
                    else
                    {
                        position = data.CalculatePosition(m_truckUnspawnPosition);
                        target = data.CalculateSidewalkPosition(m_truckUnspawnPosition.x, 0.0f);
                    }
                    //index ++;
                }
                else
                {
                    BaseCalculateSpawnPosition(buildingID, ref data, ref randomizer, info, out position, out target);
                }
            }
            else if (m_transportInfo != null && info.m_vehicleType == m_transportInfo.m_vehicleType)
            {

                position = data.CalculatePosition(m_spawnPosition);
                if (m_canInvertTarget &&
                    Singleton<SimulationManager>.instance.m_metaData.m_invertTraffic !=
                    SimulationMetaData.MetaBool.True)
                {
                    target = data.CalculatePosition(m_spawnPosition * 2f - m_spawnTarget);
                }
                else
                {
                    target = data.CalculatePosition(m_spawnTarget);
                }
            }
            else if (m_transportInfo2 != null && info.m_vehicleType == m_transportInfo2.m_vehicleType)
            {

                position = data.CalculatePosition(m_spawnPosition2);
                if (m_canInvertTarget &&
                    Singleton<SimulationManager>.instance.m_metaData.m_invertTraffic !=
                    SimulationMetaData.MetaBool.True)
                {
                    target = data.CalculatePosition(m_spawnPosition2 * 2f - m_spawnTarget2);
                }
                else
                {
                    target = data.CalculatePosition(m_spawnTarget2);
                }
            }
            else
            {
                BaseCalculateUnspawnPosition(buildingID, ref data, ref randomizer, info, out position, out target);
            }
        }

        private static bool InvertPositions(Building data)
        {
            return (Singleton<SimulationManager>.instance.m_metaData.m_invertTraffic == SimulationMetaData.MetaBool.True && !Configuration.IsStationInverted(data)) || Configuration.IsStationInverted(data);
        }


        //PlayerBuildingAI implementation
        private static void BaseCalculateSpawnPosition(ushort buildingID, ref Building data, ref Randomizer randomizer, VehicleInfo info, out Vector3 position, out Vector3 target)
        {
            position = data.CalculateSidewalkPosition(0.0f, 3f);
            target = position;
        }

        //PlayerBuildingAI implementation
        private static void BaseCalculateUnspawnPosition(ushort buildingID, ref Building data, ref Randomizer randomizer, VehicleInfo info, out Vector3 position, out Vector3 target)
        {
            position = data.CalculateSidewalkPosition(0.0f, 3f);
            target = position;
        }
    }
}