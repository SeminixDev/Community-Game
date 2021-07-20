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
using UnityEngine;
using UnityEngine.UI;

public class UIMenuFade : Singleton<UIMenuFade>
{
  [SerializeField] Image fadeImage;
  [SerializeField] FloatValue menuFadeDurationValue;

  enum State
  {
    FadingIn,
    FadingOut,
  }

  State targetState;
  
  // Current fade value, 0.0f is faded in, 1.0f is faded out.
  float fadeValue;
  Action onFadedOut;
  Action onFadedIn;

  protected override void Awake()
  {
    base.Awake();
    var color = fadeImage.color;
    color.a = 1.0f;
    fadeImage.color = color;
    fadeValue = 1.0f;
  }

  /// <summary>
  /// Fades out to black.
  /// </summary>
  public void FadeOut(Action onFadedOut = null)
  {
    this.onFadedOut = onFadedOut;
    targetState = State.FadingOut;
  }

  /// <summary>
  /// Fades into transparent/clear.
  /// </summary>
  public void FadeIn(Action onFadedIn = null)
  {
    this.onFadedIn = onFadedIn;
    targetState = State.FadingIn;
  }

  void Update()
  {
    switch (targetState)
    {
      case State.FadingIn:
        fadeValue = Mathf.Clamp01(fadeValue - Time.deltaTime / menuFadeDurationValue.value);
        if (Mathf.Approximately(fadeValue, 0.0f))
        {
          onFadedIn?.Invoke();
          onFadedIn = null;
        }
        break;
      case State.FadingOut:
        fadeValue = Mathf.Clamp01(fadeValue + Time.deltaTime / menuFadeDurationValue.value);
        if (Mathf.Approximately(fadeValue, 1.0f))
        {
          onFadedOut?.Invoke();
          onFadedOut = null;
        }
        break;
    }

    // Fade the screen color
    var color = fadeImage.color;
    color.a = Mathf.Clamp01(fadeValue);
    fadeImage.color = color;
    fadeImage.raycastTarget = !Mathf.Approximately(color.a, 0.0f);
  }
}
