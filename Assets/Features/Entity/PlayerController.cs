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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EntityController
{
  // This is a reference to the local player
  public static PlayerController localPlayer;
  public override void OnStartLocalPlayer()
  {
    base.OnStartLocalPlayer();
    localPlayer = this;
  }

  public override void OnStartServer()
  {
    base.OnStartServer();
    PlayerManager.instance.AddPlayer(this);
  }

  public override void OnStopServer()
  {
    base.OnStopServer();
    PlayerManager.instance.RemovePlayer(this);
  }
}
