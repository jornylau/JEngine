//
// ClassBindMgr.cs
//
// Author:
//       JasonXuDeveloper（傑） <jasonxudeveloper@gmail.com>
//
// Copyright (c) 2020 JEngine
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JEngine.Core
{
    public class ClassBindMgr : MonoBehaviour
    {
        public static void Instantiate()
        {
            _instance = new GameObject("ClassBindMgr").AddComponent<ClassBindMgr>();
            DontDestroyOnLoad(_instance);
        }

        private static ClassBindMgr _instance;
        public static List<Scene> LoadedScenes;
        private static List<ClassBind> cbs;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(this);
            }
            
            LoadedScenes = new List<Scene>(0);
            LoadedScenes.Add(SceneManager.GetActiveScene());
            cbs = new List<ClassBind>(0);

            SceneManager.sceneLoaded += (scene, mode) =>
            {
                LoadedScenes.Add(scene);
                DoBind();
            };
            
            SceneManager.sceneUnloaded+=(scene) =>
            {
                LoadedScenes.Remove(scene);
            };
            DoBind();
        }

        public static void DoBind(List<ClassBind> cbs)
        {
            foreach (var cb in cbs)
            {
                //先添加
                foreach (_ClassBind _class in cb.ScriptsToBind)
                {
                    if (_class == null || _class.Added)
                    {
                        continue;
                    }

                    cb.AddClass(_class);
                }
            }

            foreach (var cb in cbs)
            {
                //再赋值
                foreach (_ClassBind _class in cb.ScriptsToBind)
                {
                    if (_class == null || _class.BoundData)
                    {
                        continue;
                    }

                    cb.SetVal(_class);
                }
            }

            //激活
            foreach (var cb in cbs)
            {
                foreach (_ClassBind _class in cb.ScriptsToBind)
                {
                    if (_class == null ||_class.Activated)
                    {
                        continue;
                    }

                    cb.Active(_class);
                }
            }
        }
        public static void DoBind()
        {
            cbs = Tools.FindObjectsOfTypeAll<ClassBind>();
            DoBind(cbs);
        }
    }
}