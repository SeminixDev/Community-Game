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

using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealthController : MonoBehaviour
{
  // The value we should set the enemy's health to
  [SerializeField]
  int startingHealth;

  [SyncVar] int health; //- the entity's current health value, should never be negative.

  bool respawning = false;  // used to prevent multiple coroutines from running.

  // Reduces the entity's health by the given damage value, sends `OnDeath` message if dead.
  public void TakeDamage(int damage)
  {
    if (health <= 0) return; //player should be dead, do nothing

    health -= damage; //apply the damage

    if(health <= 0) //check if the player has died
    {
      health = 0;
      gameObject.SendMessage("OnDeath");
      if (!respawning)
      {
        StartCoroutine(RespawnCoroutine(5)); //respawn after 5 sec
      }
    }
  }

// When respawn is called, the health is set back to startingHealth and OnRespawn message is sent.
  private void Respawn()
  {
    health = startingHealth;
    gameObject.SendMessage("OnRespawn");
  }

  IEnumerator RespawnCoroutine(int waitTime)
  {
    respawning = true;
    yield return new WaitForSeconds(waitTime);
    respawning = false;
    Respawn();
  }
}
