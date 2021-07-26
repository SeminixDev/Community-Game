// Copyright 2021 Boppy Games, LLC
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Mirror;
using NaughtyAttributes;
using UnityEngine;

public class InventoryTest : MonoBehaviour
{
  [SerializeField] InventoryController inventoryController;
  [SerializeField] Item item;
  [SerializeField] int count;
  [SerializeField] int slotIndex;

  [Button()]
  void AddItemToAnySlot()
  {
    int addedCount = inventoryController.AddItem(item, count);
    Debug.Log($"Added {addedCount}x {item} to inventory");
  }

  [Button()]
  void AddItemToSpecifiedSlot()
  {
    int addedCount = inventoryController.AddItem(item, count, slotIndex);
    Debug.Log($"Added {addedCount}x {item} to slot index {slotIndex} in inventory");
  }

  [Button()]
  void RemoveItemFromAnySlot()
  {
    int removedCount = inventoryController.RemoveItem(item, count);
    Debug.Log($"Removed {removedCount}x {item} from inventory");
  }

  [Button()]
  void RemoveItemFromSpecifiedSlot()
  {
    int removedCount = inventoryController.RemoveItem(item, count, slotIndex);
    Debug.Log($"Removed {removedCount}x {item} from slot index {slotIndex} in inventory");
  }
}
