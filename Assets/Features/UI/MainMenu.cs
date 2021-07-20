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
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : Singleton<MainMenu>
{
  void Start()
  {
    UIMenuFade.Instance.FadeIn();
  }

  public void NewGame()
  {
    UIMenuFade.Instance.FadeOut(() =>
    {
      SceneManager.LoadScene("Overworld");  
    });
  }
  
  public void LoadGame()
  {
    
  }
  
  public void JoinGame()
  {
    
  }
  
  public void Settings()
  {
    
  }
  
  public void Exit()
  {
#if UNITY_EDITOR
    EditorApplication.isPlaying = false;
    return;
#else
    Application.Quit();
#endif
  }
}
