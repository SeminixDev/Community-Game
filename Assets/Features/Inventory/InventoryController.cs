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

using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class InventoryController : NetworkBehaviour
{
  [SerializeField] List<InventorySlot> inventorySlots;

  /// <summary>
  /// Adds an item to any available inventory slot, returns amount added
  /// </summary>
  /// <param name="item"> Item to add </param>
  /// <param name="count"> The number of items to add </param>
  /// <returns> Added amount </returns>
  public int AddItem(Item item, int count)
  {
    // Check for bad params
    if (item == null || count <= 0)
      return 0;

    // Get the first inventory slot that's empty or already contains the same item and is not filled
    var inventorySlot = inventorySlots.FirstOrDefault
      (slot => slot.IsEmpty || slot.item == item && slot.Count < slot.item.stackSize);

    // No available slot found 
    if (inventorySlot == null)
      return 0;

    // Add item to the slot by calling overload
    int amountAdded = AddItem(item, count, GetIndex(inventorySlot));

    // There is enough space for all of the items
    if (amountAdded >= count)
      return amountAdded;

    // There is not enough space, recursively add item to next available slot and count the number added
    count -= amountAdded;
    return amountAdded + AddItem(item, count);
  }

  /// <summary>
  /// Adds an item to the given slot, returns the amount added
  /// </summary>
  /// <param name="item"> Item to add </param>
  /// <param name="count"> The number of items to add </param>
  /// <param name="index"> Index of the slot to add to (starts at 0) </param>
  /// <returns> Amount added </returns>
  public int AddItem(Item item, int count, int index)
  {
    // Check for bad params
    if (item == null || count <= 0)
      return 0;
    
    // Get the slot and return 0 if wrong item or invalid slot
    var inventorySlot = GetSlot(index);
    if (inventorySlot == null || inventorySlot.item != item && inventorySlot.item != null)
      return 0;

    // Replace the item in case slot contains null item and calculate space left in the slot
    inventorySlot.item = item;
    int spaceLeft = item.stackSize - inventorySlot.Count;
    
    // There is enough space for all of the items, return full count
    if (spaceLeft >= count)
    {
      inventorySlot.Count += count;
      return count;
    }
    
    // There is not enough space, return how much space was filled
    inventorySlot.Count += spaceLeft;
    return spaceLeft;
  }

  /// <summary>
  /// Removes items from any available inventory slot, returns amount removed
  /// </summary>
  /// <param name="item"> Item to remove </param>
  /// <param name="count"> The number of items to remove </param>
  /// <returns> Amount removed </returns>
  public int RemoveItem(Item item, int count)
  {
    // Check for bad params
    if (item == null || count <= 0)
      return 0;

    // Get the last inventory slot that already contains the specified item
    var inventorySlot = inventorySlots.LastOrDefault(slot => slot.item == item);
    
    // No matching slot found 
    if (inventorySlot == null)
      return 0;

    // Remove items from slot by calling overload
    int amountRemoved = RemoveItem(item, count, GetIndex(inventorySlot));
    
    // There were enough items to remove
    if (amountRemoved >= count)
      return amountRemoved;
    
    // There are still items that need to be removed
    count -= amountRemoved;
    return amountRemoved + RemoveItem(item, count);
  }
  
  /// <summary>
  /// Removes items from a specific slot, returns the amount removed
  /// </summary>
  /// <param name="item"> Item to remove </param>
  /// <param name="count"> The number of items to remove </param>
  /// <param name="index"> Index of the slot to remove from (starts at 0) </param>
  /// <returns> Amount removed </returns>
  public int RemoveItem(Item item, int count, int index)
  {
    // Check for bad params
    if (item == null || count <= 0)
      return 0;
    
    // Get the slot and return 0 if wrong item or invalid slot
    var inventorySlot = GetSlot(index);
    if (inventorySlot == null || inventorySlot.item != item)
      return 0;

    // The slot has enough items to remove, return full count
    if (inventorySlot.Count >= count)
    {
      inventorySlot.Count -= count;
      return count;
    }
    
    // The slot does not have enough items to remove, return how many were able to be removed
    count = inventorySlot.Count;
    inventorySlot.Count = 0;
    return count;
  }

  /// <summary>
  /// Get an InventorySlot from the inventorySlots list based on the index
  /// </summary>
  /// <param name="index"> The index of an InventorySlot within the list </param>
  /// <returns> The slot in that specific index, null if doesn't exist </returns>
  public InventorySlot GetSlot(int index)
  {
    // If outside of range, return null
    if (index >= inventorySlots.Count || index < 0)
      return null;
    
    return inventorySlots[index];
  }

  /// <summary>
  /// Returns the index of a specific InventorySlot from the inventorySlots list
  /// </summary>
  /// <param name="slot"> The InventorySlot to get the index of </param>
  /// <returns> The index of the specified slot, -1 if not in list </returns>
  public int GetIndex(InventorySlot slot)
  {
    return inventorySlots.FindIndex(inventorySlot => inventorySlot == slot);
  }

  /// <summary>
  /// Returns the count of the given item in the inventory
  /// </summary>
  /// <param name="item"> Item to check the count of </param>
  /// <returns> Count of the given item </returns>
  public int GetCount(Item item) => inventorySlots.Where(slot => slot.item == item).Select(slot => slot.Count).Sum();
}
