using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrototypeMod
{
    [HarmonyPatch(typeof(AfterRaidScreen), nameof(AfterRaidScreen.UnloadToCargoButtonOnOnClick))]
    public static class AfterRaidScreen_UnloadToCargoButtonOnOnClick_Patch
    {
        public static bool Prefix(AfterRaidScreen __instance, CommonButton obj, int clickCount)
        {
            //Use the original unload if the shift key is pressed.
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                return true;
            }

            //WARNING COPY:  This is a full copy of the original code.
            //The goal is to exclude the vest from the unload all command.

            List<BasePickupItem> list = new List<BasePickupItem>();

            //Origional code:
            //foreach (ItemStorage storage in __instance._merc.CreatureData.Inventory.Storages)
            //{
            //    list.AddRange(storage.Items);
            //}

            // ---- Start New Code
            //Just filter out the Vest Storage
            List<ItemStorage> storages = __instance._merc.CreatureData.Inventory.Storages
                .Where(x => x != __instance._merc.CreatureData.Inventory.VestStore)
                .ToList();

            foreach (ItemStorage storage in storages)
            {
                list.AddRange(storage.Items);
            }

            // ---- End New Code

            foreach (BasePickupItem item in list)
            {
                item.Storage.Remove(item);
                item.Storage = null;
                MagnumCargoSystem.AddCargo(__instance._magnumCargo, __instance._spaceTime, item, __instance._activeShipCargo);
            }
            __instance._merc.CreatureData.Inventory.RecalculateCurrentWeight();
            __instance._merc.CreatureData.RefreshResists();
            __instance.RefreshView();

            return false;
        }


    }
}
