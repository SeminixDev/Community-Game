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
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStringReplacement : MonoBehaviour
{
  Dictionary<RuntimePlatform, string> platformLookup = new Dictionary<RuntimePlatform, string>
  {
    {RuntimePlatform.WindowsEditor, "Windows"},
    {RuntimePlatform.WindowsPlayer, "Windows"},
    {RuntimePlatform.OSXEditor, "Mac OSX"},
    {RuntimePlatform.OSXPlayer, "Mac OSX"},
    {RuntimePlatform.LinuxEditor, "Linux"},
    {RuntimePlatform.LinuxPlayer, "Linux"},
  };
  
  void Start()
  {
    var text = GetComponentInChildren<TextMeshProUGUI>();
    if(text != null)
      text.text = Replace(text.text);
    var text2 = GetComponentInChildren<Text>();
    if (text2 != null)
      text2.text = Replace(text2.text);
  }

  string Replace(string input) => 
    input.Replace("PLATFORM", platformLookup[Application.platform]);
}
