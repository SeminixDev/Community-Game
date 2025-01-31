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

using UnityEngine;

/// <summary>
/// Global access. Not persistent across scenes. Removes duplicates. Does not get created automatically.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
  public static T Instance { get; private set; }

  protected virtual void Awake()
  {
    Initialize();
  }
  
  protected virtual void Initialize()
  {
    if (Instance != null && Instance != this)
      Destroy(gameObject);
    else
      Instance = this as T;
  }
}
