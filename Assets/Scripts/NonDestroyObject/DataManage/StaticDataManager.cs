using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Utils;

namespace NonDestroyObject.DataManage
{
    
    /// <summary>
    /// 서버로 부터 받아온 데이터를 관리하는 매니저
    /// </summary>
    [Serializable]
    public class StaticDataManager
    {
        [SerializeField] private List<StaticDataVersion> currentVersions;
        [SerializeField] private List<StaticDataVersion> serverVersions;

        public List<GatchaProbability> GatchaProbabilitys => gatchaProbabilitys;
        [SerializeField] private List<GatchaProbability> gatchaProbabilitys;
        
        [SerializeField] private List<EquipItemInfo> equipmentItemInfos;
        public EquipItemInfo GetEquipItemInfo(int rare, int option)
        {
            var totalLength = equipmentItemInfos.Count;
            var totalOptions = (int)(totalLength / 6);

            var equipItemInfo = equipmentItemInfos[totalOptions * (rare - 1) + option];
            
            return equipItemInfo;
        }
        
        [SerializeField] private List<EnhanceInfo> enhanceInfos;
        public EnhanceInfo GetEnhanceInfo(int rare)
        {
            var enhanceInfo = enhanceInfos[rare - 1]; 
            return enhanceInfo;
        }
        
        public void Start()
        {
            currentVersions = new List<StaticDataVersion>();
            var versionJson = JsonSL.LoadJson(JsonTitle.StaticDataVersion);
            if (versionJson != String.Empty)
                currentVersions = JsonConvert.DeserializeObject<List<StaticDataVersion>>(versionJson);
            
            var gatchaProbabilityJson = JsonSL.LoadJson(JsonTitle.GatchaProbability);
            if (gatchaProbabilityJson != String.Empty)
                gatchaProbabilitys = JsonConvert.DeserializeObject<List<GatchaProbability>>(gatchaProbabilityJson);
            
            var equipmentItemInfosJson = JsonSL.LoadJson(JsonTitle.EquipItemInfo);
            if (equipmentItemInfosJson != String.Empty)
                equipmentItemInfos = JsonConvert.DeserializeObject<List<EquipItemInfo>>(equipmentItemInfosJson);
            
            var gatchaProbabilitysJson = JsonSL.LoadJson(JsonTitle.EnhanceInfo);
            if (gatchaProbabilitysJson != String.Empty)
                enhanceInfos = JsonConvert.DeserializeObject<List<EnhanceInfo>>(gatchaProbabilitysJson);
        }

        private StaticDataVersion GetCurrentVersion(string title)
        {
            foreach (var version in currentVersions)
            {
                if (version.title == title)
                    return version;
            }
            return null;
        }

        public async UniTask GetStaticDatasFromServer()
        {
            // Get StaticDataVersion First
            serverVersions = new List<StaticDataVersion>();
            
            var reqList = new List<string>();
            reqList.Add(JsonTitle.StaticDataVersion.ToString());
            var res = await NetworkManager.Instance.GetStaticDataJsonList(reqList);

            if (res.Success)
            {
                serverVersions = JsonConvert.DeserializeObject<List<StaticDataVersion>>(res.Data[0]);
            }
            
            if (serverVersions is { Count: 0 })
                return;

            // After Get StaticDataVersion, Get All the Others inside of StaticDataVersion
            List<string> unVerionedStaticDataTitleList = new List<string>();
            
            // Check If need Update
            foreach (var serverVersion in serverVersions)
            {
                foreach (var staticDataTitle in Enum.GetValues(typeof(JsonTitle)))
                {
                    if ((int)staticDataTitle < (int)JsonTitle.GatchaProbability)
                        continue;
                    
                    var titleString = staticDataTitle.ToString();
                    var currVersion = GetCurrentVersion(titleString);
                    if (
                            (
                                currVersion == null && 
                                serverVersion.title == titleString &&
                                !unVerionedStaticDataTitleList.Contains(titleString)
                            )
                            || 
                            (
                                titleString == serverVersion.title && 
                                currVersion.versionNum < serverVersion.versionNum &&
                                currVersion.updatedDate < serverVersion.updatedDate
                            )
                        )
                    {
                        unVerionedStaticDataTitleList.Add(titleString);
                    }
                }
            }

            if (unVerionedStaticDataTitleList.Count == 0)
                return;
            
            // Send Request to Server
            var staticDataJsonResult = await NetworkManager.Instance.GetStaticDataJsonList(unVerionedStaticDataTitleList);
            
            if (!staticDataJsonResult.Success)
                return;
            
            var staticDatas = staticDataJsonResult.Data;
            // Update Local Data And Load
            for (int i = 0; i < staticDatas.Count; i++)
            {
                switch (unVerionedStaticDataTitleList[i])
                {
                    case nameof(JsonTitle.GatchaProbability):
                        gatchaProbabilitys = JsonConvert.DeserializeObject<List<GatchaProbability>>(staticDatas[i]);
                        
                        var gatchaProbJsonString = JsonConvert.SerializeObject(gatchaProbabilitys);
                        JsonSL.SaveJson(JsonTitle.GatchaProbability, gatchaProbJsonString).Forget();
                        break;
                    
                    case nameof(JsonTitle.EquipItemInfo):
                        equipmentItemInfos = JsonConvert.DeserializeObject<List<EquipItemInfo>>(staticDatas[i]);
                        
                        var equipmentItemInfosJsonString = JsonConvert.SerializeObject(equipmentItemInfos);
                        JsonSL.SaveJson(JsonTitle.EquipItemInfo, equipmentItemInfosJsonString).Forget();
                        break;
                    
                    case nameof(JsonTitle.EnhanceInfo):
                        enhanceInfos = JsonConvert.DeserializeObject<List<EnhanceInfo>>(staticDatas[i]);
                        
                        var enhanceInfosJsonString = JsonConvert.SerializeObject(enhanceInfos);
                        JsonSL.SaveJson(JsonTitle.EnhanceInfo, enhanceInfosJsonString).Forget();
                        break;
                }
            }
            
            var toJsonString = JsonConvert.SerializeObject(serverVersions);
            JsonSL.SaveJson(JsonTitle.StaticDataVersion, toJsonString).Forget();
        }
    }
}
