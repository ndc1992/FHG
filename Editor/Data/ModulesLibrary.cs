﻿using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HT.ModuleManager
{
    /// <summary>
    /// 模块库
    /// </summary>
    [Serializable]
    internal sealed class ModulesLibrary : IDisposable
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName;
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email;
        /// <summary>
        /// 项目路径
        /// </summary>
        public string ProjectPath;
        /// <summary>
        /// 所有模块
        /// </summary>
        public List<Module> Modules = new List<Module>();
        
        /// <summary>
        /// 模块库
        /// </summary>
        /// <param name="defines">模块的定义</param>
        public ModulesLibrary(string[] defines)
        {
            UserName = EditorPrefs.GetString(Utility.UserNameKey, "");
            Email = EditorPrefs.GetString(Utility.EmailKey, "");
            ProjectPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));

            for (int i = 0; i < defines.Length; i++)
            {
                string[] paths = defines[i].Split('|');
                CreateNullModule(paths[0], paths[1]);
            }
        }
        
        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose()
        {
            DisposeAllModule();
        }
        /// <summary>
        /// 刷新状态
        /// </summary>
        public void RefreshState()
        {
            for (int i = 0; i < Modules.Count; i++)
            {
                Modules[i].RefreshState();
            }
        }
        /// <summary>
        /// 打开一个模块
        /// </summary>
        /// <param name="path">模块的本地路径</param>
        public void OpenModule(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Utility.LogError("Open module failed! the module path is empty!");
                return;
            }

            if (!path.Contains(ProjectPath))
            {
                Utility.LogError("Open module failed! only modules in the current project can be opened!");
                return;
            }

            if (Modules.Exists((repo) => { return repo.Path == path; }))
            {
                Utility.LogError(string.Format("Open module failed! {0} is already opend!", path));
                return;
            }

            if (Directory.Exists(path) && Repository.IsValid(path))
            {
                Modules.Add(new Module(path, null));
            }
            else
            {
                Utility.LogError(string.Format("Open module failed! {0} is not a valid git repository!", path));
            }
        }
        /// <summary>
        /// 创建一个空模块
        /// </summary>
        /// <param name="path">模块的本地路径</param>
        /// <param name="remotePath">模块的远端路径</param>
        public void CreateNullModule(string path, string remotePath)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(remotePath))
            {
                Utility.LogError("Open module failed! the module path or remotePath is empty!");
                return;
            }

            if (!path.Contains(ProjectPath))
            {
                Utility.LogError("Open module failed! only modules in the current project can be opened!");
                return;
            }

            if (Modules.Exists((repo) => { return repo.Path == path; }))
            {
                Utility.LogError(string.Format("Open module failed! {0} is already opend!", path));
                return;
            }

            Modules.Add(new Module(path, remotePath));
        }
        /// <summary>
        /// 移除一个模块
        /// </summary>
        /// <param name="module">模块</param>
        public void RemoveModule(Module module)
        {
            if (Modules.Contains(module))
            {
                module.Dispose();
                Modules.Remove(module);
            }
        }
        /// <summary>
        /// 移除一个模块
        /// </summary>
        /// <param name="moduleIndex">模块索引</param>
        public void RemoveModule(int moduleIndex)
        {
            if (moduleIndex >= 0 && moduleIndex < Modules.Count)
            {
                Modules[moduleIndex].Dispose();
                Modules.RemoveAt(moduleIndex);
            }
        }
        /// <summary>
        /// 移除所有模块
        /// </summary>
        public void DisposeAllModule()
        {
            for (int i = 0; i < Modules.Count; i++)
            {
                Modules[i].Dispose();
            }
            Modules.Clear();
        }
        /// <summary>
        /// 拉取所有模块
        /// </summary>
        public void PullAll()
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Email))
            {
                Utility.LogError("Pull all failed! UserName or Email can not be empty! click the button 'Credentials' in the upper right corner!");
                return;
            }

            for (int i = 0; i < Modules.Count; i++)
            {
                Modules[i].Pull(UserName, Email);
            }
        }
        /// <summary>
        /// 拉取指定模块
        /// </summary>
        /// <param name="module">模块</param>
        public void Pull(Module module)
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Email))
            {
                Utility.LogError("Pull failed! UserName or Email can not be empty! click the button 'Credentials' in the upper right corner!");
                return;
            }

            module.Pull(UserName, Email);
        }
        /// <summary>
        /// 克隆指定模块
        /// </summary>
        /// <param name="module">模块</param>
        public void Clone(Module module)
        {
            module.Clone();
        }
        /// <summary>
        /// 设置凭据
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="email">邮箱</param>
        public void SetCredentials(string userName, string email)
        {
            UserName = userName;
            Email = email;

            EditorPrefs.SetString(Utility.UserNameKey, userName);
            EditorPrefs.SetString(Utility.EmailKey, email);
        }
    }
}