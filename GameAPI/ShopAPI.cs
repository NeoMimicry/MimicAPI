using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bifrost.ShopGroup;
using UnityEngine;

namespace MimicAPI.GameAPI
{
    public static class ShopAPI
    {
        public static ShopGroup_MasterData GetShopGroupData(int shopGroupID)
        {
            try
            {
                DataManager? dataManager = ManagerAPI.GetDataManager();
                if (dataManager == null)
                    return null;

                var excelDataManager = ReflectionHelper.GetFieldValue(dataManager, "ExcelDataManager");
                if (excelDataManager == null)
                    return null;

                var shopGroupDict = ReflectionHelper.GetFieldValue<Dictionary<int, ShopGroup_MasterData>>(excelDataManager, "ShopGroupDict");
                if (shopGroupDict == null || !shopGroupDict.ContainsKey(shopGroupID))
                    return null;

                return shopGroupDict[shopGroupID];
            }
            catch
            {
                return null;
            }
        }

        public static List<ShopGroup_MasterData> GetAllShopGroups()
        {
            try
            {
                DataManager? dataManager = ManagerAPI.GetDataManager();
                if (dataManager == null)
                    return new List<ShopGroup_MasterData>();

                var excelDataManager = ReflectionHelper.GetFieldValue(dataManager, "ExcelDataManager");
                if (excelDataManager == null)
                    return new List<ShopGroup_MasterData>();

                var shopGroupDict = ReflectionHelper.GetFieldValue<Dictionary<int, ShopGroup_MasterData>>(excelDataManager, "ShopGroupDict");
                if (shopGroupDict == null)
                    return new List<ShopGroup_MasterData>();

                return shopGroupDict.Values.ToList();
            }
            catch
            {
                return new List<ShopGroup_MasterData>();
            }
        }

        public static bool AddShop(int shopGroupID, ShopGroup_MasterData shopData)
        {
            if (shopData == null)
                return false;

            try
            {
                DataManager? dataManager = ManagerAPI.GetDataManager();
                if (dataManager == null)
                    return false;

                var excelDataManager = ReflectionHelper.GetFieldValue(dataManager, "ExcelDataManager");
                if (excelDataManager == null)
                    return false;

                var shopGroupDict = ReflectionHelper.GetFieldValue<Dictionary<int, ShopGroup_MasterData>>(excelDataManager, "ShopGroupDict");
                if (shopGroupDict == null)
                    return false;

                if (shopGroupDict.ContainsKey(shopGroupID))
                    return false;

                shopGroupDict[shopGroupID] = shopData;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool DeleteShop(int shopGroupID)
        {
            try
            {
                DataManager? dataManager = ManagerAPI.GetDataManager();
                if (dataManager == null)
                    return false;

                var excelDataManager = ReflectionHelper.GetFieldValue(dataManager, "ExcelDataManager");
                if (excelDataManager == null)
                    return false;

                var shopGroupDict = ReflectionHelper.GetFieldValue<Dictionary<int, ShopGroup_MasterData>>(excelDataManager, "ShopGroupDict");
                if (shopGroupDict == null || !shopGroupDict.ContainsKey(shopGroupID))
                    return false;

                shopGroupDict.Remove(shopGroupID);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool UpdateShop(int shopGroupID, ShopGroup_MasterData updatedData)
        {
            if (updatedData == null)
                return false;

            try
            {
                DataManager? dataManager = ManagerAPI.GetDataManager();
                if (dataManager == null)
                    return false;

                var excelDataManager = ReflectionHelper.GetFieldValue(dataManager, "ExcelDataManager");
                if (excelDataManager == null)
                    return false;

                var shopGroupDict = ReflectionHelper.GetFieldValue<Dictionary<int, ShopGroup_MasterData>>(excelDataManager, "ShopGroupDict");
                if (shopGroupDict == null || !shopGroupDict.ContainsKey(shopGroupID))
                    return false;

                shopGroupDict[shopGroupID] = updatedData;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool UpdateShopItem(int shopGroupID, int itemSlot, int itemMasterID, int itemPrice)
        {
            if (itemSlot < 1 || itemSlot > 9)
                return false;

            try
            {
                ShopGroup_MasterData shopData = GetShopGroupData(shopGroupID);
                if (shopData == null)
                    return false;

                string masterIDField = $"item{itemSlot}_masterid";
                string priceField = $"item{itemSlot}_price";

                ReflectionHelper.SetFieldValue(shopData, masterIDField, itemMasterID);
                ReflectionHelper.SetFieldValue(shopData, priceField, itemPrice);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool RemoveShopItem(int shopGroupID, int itemSlot)
        {
            if (itemSlot < 1 || itemSlot > 9)
                return false;

            try
            {
                ShopGroup_MasterData shopData = GetShopGroupData(shopGroupID);
                if (shopData == null)
                    return false;

                string masterIDField = $"item{itemSlot}_masterid";
                string priceField = $"item{itemSlot}_price";
                string valListField = $"ShopGroup_item{itemSlot}_valval";

                ReflectionHelper.SetFieldValue(shopData, masterIDField, 0);
                ReflectionHelper.SetFieldValue(shopData, priceField, 0);

                var valList = ReflectionHelper.GetFieldValue(shopData, valListField);
                if (valList is System.Collections.IList list)
                {
                    list.Clear();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static int GetShopItemMasterID(int shopGroupID, int itemSlot)
        {
            if (itemSlot < 1 || itemSlot > 9)
                return 0;

            try
            {
                ShopGroup_MasterData shopData = GetShopGroupData(shopGroupID);
                if (shopData == null)
                    return 0;

                string masterIDField = $"item{itemSlot}_masterid";
                return ReflectionHelper.GetFieldValue<int>(shopData, masterIDField);
            }
            catch
            {
                return 0;
            }
        }

        public static int GetShopItemPrice(int shopGroupID, int itemSlot)
        {
            if (itemSlot < 1 || itemSlot > 9)
                return 0;

            try
            {
                ShopGroup_MasterData shopData = GetShopGroupData(shopGroupID);
                if (shopData == null)
                    return 0;

                string priceField = $"item{itemSlot}_price";
                return ReflectionHelper.GetFieldValue<int>(shopData, priceField);
            }
            catch
            {
                return 0;
            }
        }

        public static List<int> GetAllShopItemMasterIDs(int shopGroupID)
        {
            var itemIDs = new List<int>();

            try
            {
                ShopGroup_MasterData shopData = GetShopGroupData(shopGroupID);
                if (shopData == null)
                    return itemIDs;

                for (int i = 1; i <= 9; i++)
                {
                    int masterID = GetShopItemMasterID(shopGroupID, i);
                    if (masterID != 0)
                    {
                        itemIDs.Add(masterID);
                    }
                }
            }
            catch { }

            return itemIDs;
        }

        public static bool ShopExists(int shopGroupID)
        {
            try
            {
                DataManager? dataManager = ManagerAPI.GetDataManager();
                if (dataManager == null)
                    return false;

                var excelDataManager = ReflectionHelper.GetFieldValue(dataManager, "ExcelDataManager");
                if (excelDataManager == null)
                    return false;

                var shopGroupDict = ReflectionHelper.GetFieldValue<Dictionary<int, ShopGroup_MasterData>>(excelDataManager, "ShopGroupDict");
                if (shopGroupDict == null)
                    return false;

                return shopGroupDict.ContainsKey(shopGroupID);
            }
            catch
            {
                return false;
            }
        }

        public static int GetShopCount()
        {
            try
            {
                DataManager? dataManager = ManagerAPI.GetDataManager();
                if (dataManager == null)
                    return 0;

                var excelDataManager = ReflectionHelper.GetFieldValue(dataManager, "ExcelDataManager");
                if (excelDataManager == null)
                    return 0;

                var shopGroupDict = ReflectionHelper.GetFieldValue<Dictionary<int, ShopGroup_MasterData>>(excelDataManager, "ShopGroupDict");
                if (shopGroupDict == null)
                    return 0;

                return shopGroupDict.Count;
            }
            catch
            {
                return 0;
            }
        }

        public static ShopGroup_MasterData CreateNewShopGroup(int id)
        {
            try
            {
                var newShop = new ShopGroup_MasterData();
                ReflectionHelper.SetFieldValue(newShop, "id", id);

                for (int i = 1; i <= 9; i++)
                {
                    string masterIDField = $"item{i}_masterid";
                    string priceField = $"item{i}_price";
                    string valListField = $"ShopGroup_item{i}_valval";

                    ReflectionHelper.SetFieldValue(newShop, masterIDField, 0);
                    ReflectionHelper.SetFieldValue(newShop, priceField, 0);
                }

                return newShop;
            }
            catch
            {
                return null;
            }
        }

        public static bool ClearShopItems(int shopGroupID)
        {
            try
            {
                ShopGroup_MasterData shopData = GetShopGroupData(shopGroupID);
                if (shopData == null)
                    return false;

                for (int i = 1; i <= 9; i++)
                {
                    RemoveShopItem(shopGroupID, i);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
