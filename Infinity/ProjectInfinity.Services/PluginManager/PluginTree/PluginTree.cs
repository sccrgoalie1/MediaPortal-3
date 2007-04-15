﻿#region Copyright (C) 2005-2007 Team MediaPortal

/* 
 *	Copyright (C) 2005-2007 Team MediaPortal
 *	http://www.team-mediaportal.com
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *   
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *   
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  http://www.gnu.org/copyleft/gpl.html
 *
 *  Code modified from SharpDevelop AddIn code
 *  Thanks goes to: Mike Krüger
 */

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using ProjectInfinity;
using ProjectInfinity.Logging;

namespace ProjectInfinity.Plugins
{
  /// <summary>
  ///  class containing the PluginTree. Contains methods for accessing tree nodes and building items.
  /// </summary>
  public class PluginTree : IPluginTree
  {
    #region Variables
    List<Plugin> _plugins = new List<Plugin>();
    PluginTreeNode _rootNode = new PluginTreeNode();
    Dictionary<string, IBuilder> _builders = new Dictionary<string, IBuilder>();

    // do we require conditions?
    //Dictionary<string, IConditionEvaluator> conditionEvaluators = new Dictionary<string, IConditionEvaluator>();
    #endregion

    #region Constructors/Destructors
    public PluginTree()
    {
      _builders.Add("Class", new ClassBuilder());
      _builders.Add("MenuItem", new MenuItemBuilder());
      //_builders.Add("Command", new CommandBuilder());
      //_builders.Add("Include", new IncludeBuilder());

      //conditionEvaluators.Add("Compare", new CompareConditionEvaluator());
      //conditionEvaluators.Add("Ownerstate", new OwnerStateConditionEvaluator());
    }
    #endregion

    #region Properties
    public IList<Plugin> Plugins
    {
      get { return _plugins.AsReadOnly(); }
    }

    public Dictionary<string, IBuilder> Builders
    {
      get { return _builders; }
    }

    //public Dictionary<string, IConditionEvaluator> ConditionEvaluators
    //{
    //  get
    //  {
    //    return conditionEvaluators;
    //  }
    //}
    #endregion

    #region Public Methods
    public bool IsTreeNode(string path)
    {
      if (path == null || path.Length == 0)
      {
        return true;
      }

      string[] splittedPath = path.Split('/');
      PluginTreeNode curPath = _rootNode;
      int i = 0;
      while (i < splittedPath.Length)
      {
        if (splittedPath[i] != string.Empty)
        {
          if (!curPath.ChildNodes.ContainsKey(splittedPath[i]))
          {
            return false;
          }
          curPath = curPath.ChildNodes[splittedPath[i]];
          ++i;
        }
      }
      return true;
    }

    public PluginTreeNode GetTreeNode(string path)
    {
      return GetTreeNode(path, true);
    }

    public PluginTreeNode GetTreeNode(string path, bool throwOnNotFound)
    {
      if (path == null || path.Length == 0)
      {
        return _rootNode;
      }
      string[] splittedPath = path.Split('/');
      PluginTreeNode curPath = _rootNode;
      int i = 0;
      while (i < splittedPath.Length)
      {
        if (splittedPath[i] != string.Empty)
        {
          if (!curPath.ChildNodes.ContainsKey(splittedPath[i]))
          {
            if (throwOnNotFound)
              throw new TreePathNotFoundException(path);
            else
              return null;
          }
          curPath = curPath.ChildNodes[splittedPath[i]];
        }
        ++i;
      }
      return curPath;
    }

    /// <summary>
    /// Builds a single item in the Plugin tree.
    /// </summary>
    public object BuildItem(string path, object caller)
    {
      int pos = path.LastIndexOf('/');
      string parent = path.Substring(0, pos);
      string child = path.Substring(pos + 1);
      PluginTreeNode node = GetTreeNode(parent);
      return node.BuildChildItem(child, caller, BuildItems(path, caller, false));
    }

    /// <summary>
    /// Builds the items in the path.
    /// </summary>
    /// <param name="path">A path in the Plugin tree.</param>
    /// <param name="caller">The owner used to create the objects.</param>
    /// <param name="throwOnNotFound">If true, throws an TreePathNotFoundException
    /// if the path is not found. If false, an empty ArrayList is returned when the
    /// path is not found.</param>
    public ArrayList BuildItems(string path, object caller, bool throwOnNotFound)
    {
      PluginTreeNode node = GetTreeNode(path, throwOnNotFound);
      if (node == null)
        return new ArrayList();
      else
        return node.BuildChildItems(caller);
    }

    /// <summary>
    /// Builds the items in the path. Ensures that all items have the type T.
    /// Throws an exception if the path is not found.
    /// </summary>
    /// <param name="path">A path in the Plugin tree.</param>
    /// <param name="caller">The owner used to create the objects.</param>
    public List<T> BuildItems<T>(string path, object caller)
    {
      return BuildItems<T>(path, caller, true);
    }

    /// <summary>
    /// Builds the items in the path. Ensures that all items have the type T.
    /// </summary>
    /// <param name="path">A path in the Plugin tree.</param>
    /// <param name="caller">The owner used to create the objects.</param>
    /// <param name="throwOnNotFound">If true, throws an TreePathNotFoundException
    /// if the path is not found. If false, an empty ArrayList is returned when the
    /// path is not found.</param>
    public List<T> BuildItems<T>(string path, object caller, bool throwOnNotFound)
    {
      PluginTreeNode node = GetTreeNode(path, throwOnNotFound);
      if (node == null)
        return new List<T>();
      else
        return node.BuildChildItems<T>(caller);
    }

    public void InsertPlugin(Plugin Plugin)
    {
      if (Plugin.Enabled)
      {
        foreach (ExtensionPath path in Plugin.Paths.Values)
        {
          AddExtensionPath(path);
        }

        //foreach (PluginRuntime runtime in Plugin.Runtimes)
        //{
        //  if (runtime.IsActive)
        //  {
        //    foreach (LazyLoadDoozer doozer in runtime.DefinedDoozers)
        //    {
        //      if (PluginTree.Builder.ContainsKey(builder.Name))
        //      {
        //        throw new PluginLoadException("Duplicate builder: " + builder.Name);
        //      }
        //      PluginTree.Builders.Add(doozer.Name, builder);
        //    }
        //    //foreach (LazyConditionEvaluator condition in runtime.DefinedConditionEvaluators)
        //    //{
        //    //  if (PluginTree.ConditionEvaluators.ContainsKey(condition.Name))
        //    //  {
        //    //    throw new PluginLoadException("Duplicate condition evaluator: " + condition.Name);
        //    //  }
        //    //  PluginTree.ConditionEvaluators.Add(condition.Name, condition);
        //    //}
        //  }
        //}

        //string PluginRoot = Path.GetDirectoryName(Plugin.FileName);
        //foreach(string bitmapResource in Plugin.BitmapResources)
        //{
        //  string path = Path.Combine(PluginRoot, bitmapResource);
        //  ResourceManager resourceManager = ResourceManager.CreateFileBasedResourceManager(Path.GetFileNameWithoutExtension(path), Path.GetDirectoryName(path), null);
        //  ResourceService.RegisterNeutralImages(resourceManager);
        //}

        //foreach(string stringResource in Plugin.StringResources)
        //{
        //  string path = Path.Combine(PluginRoot, stringResource);
        //  ResourceManager resourceManager = ResourceManager.CreateFileBasedResourceManager(Path.GetFileNameWithoutExtension(path), Path.GetDirectoryName(path), null);
        //  ResourceService.RegisterNeutralStrings(resourceManager);
        //}
      }
      _plugins.Add(Plugin);
    }

    public void RemovePlugin(Plugin Plugin)
    {
      if (Plugin.Enabled)
      {
        throw new ArgumentException("Cannot remove enabled Plugins at runtime.");
      }
      Plugins.Remove(Plugin);
    }

    // used by Load(): disables a Plugin and removes it from the dictionaries.
    void DisablePlugin(Plugin Plugin, Dictionary<string, Version> dict, Dictionary<string, Plugin> PluginDict)
    {
      Plugin.Enabled = false;
      //Plugin.Action = PluginAction.DependencyError;
      foreach (string name in Plugin.Manifest.Identities.Keys)
      {
        dict.Remove(name);
        PluginDict.Remove(name);
      }
    }

    public void Load(List<string> PluginFiles, List<string> disabledPlugins)
    {
      List<Plugin> list = new List<Plugin>();
      Dictionary<string, Version> dict = new Dictionary<string, Version>();
      Dictionary<string, Plugin> PluginDict = new Dictionary<string, Plugin>();
      foreach (string fileName in PluginFiles)
      {
        Plugin Plugin;
        try
        {
          Plugin = Plugin.Load(fileName);
        }
        catch (PluginLoadException ex)
        {
          ServiceScope.Get<ILogger>().Error(ex);
          if (ex.InnerException != null)
          {
            ServiceScope.Get<ILogger>().Error("Error loading Plugin " + fileName + ":\n" + ex.InnerException.Message);
          }
          else
          {
            ServiceScope.Get<ILogger>().Error("Error loading Plugin " + fileName + ":\n" + ex.Message);
          }
          Plugin = new Plugin();
          //Plugin.CustomErrorMessage = ex.Message;
        }
        //if (Plugin.Action == PluginAction.CustomError)
        //{
        //  list.Add(Plugin);
        //  continue;
        //}
        Plugin.Enabled = true;
        if (disabledPlugins != null && disabledPlugins.Count > 0)
        {
          foreach (string name in Plugin.Manifest.Identities.Keys)
          {
            if (disabledPlugins.Contains(name))
            {
              Plugin.Enabled = false;
              break;
            }
          }
        }
        if (Plugin.Enabled)
        {
          foreach (KeyValuePair<string, Version> pair in Plugin.Manifest.Identities)
          {
            if (dict.ContainsKey(pair.Key))
            {
              //MessageService.ShowError("Name '" + pair.Key + "' is used by " + "'" + PluginDict[pair.Key].FileName + "' and '" + fileName + "'");
              Plugin.Enabled = false;
              //Plugin.Action = PluginAction.InstalledTwice;
              break;
            }
            else
            {
              dict.Add(pair.Key, pair.Value);
              PluginDict.Add(pair.Key, Plugin);
            }
          }
        }
        list.Add(Plugin);
      }
    checkDependencies:
      for (int i = 0; i < list.Count; i++)
      {
        Plugin Plugin = list[i];
        if (!Plugin.Enabled) continue;

        Version versionFound;

        foreach (PluginReference reference in Plugin.Manifest.Conflicts)
        {
          if (reference.Check(dict, out versionFound))
          {
            //MessageService.ShowError(Plugin.Name + " conflicts with " + reference.ToString() + " and has been disabled.");
            DisablePlugin(Plugin, dict, PluginDict);
            goto checkDependencies; // after removing one Plugin, others could break
          }
        }
        foreach (PluginReference reference in Plugin.Manifest.Dependencies)
        {
          if (!reference.Check(dict, out versionFound))
          {
            if (versionFound != null)
            {
              //MessageService.ShowError(Plugin.Name + " has not been loaded because it requires " + reference.ToString() + ", but version " + versionFound.ToString() + " is installed.");
            }
            else
            {
              //MessageService.ShowError(Plugin.Name + " has not been loaded because it requires " + reference.ToString() + ".");
            }
            DisablePlugin(Plugin, dict, PluginDict);
            goto checkDependencies; // after removing one Plugin, others could break
          }
        }
      }
      foreach (Plugin plugin in list)
      {
        InsertPlugin(plugin);
      }
    }
    #endregion

    #region Private Methods
    private PluginTreeNode CreatePath(PluginTreeNode localRoot, string path)
    {
      if (path == null || path.Length == 0)
      {
        return localRoot;
      }
      string[] splittedPath = path.Split('/');
      PluginTreeNode curPath = localRoot;
      int i = 0;
      while (i < splittedPath.Length)
      {
        if (splittedPath[i] != string.Empty)
        {
          if (!curPath.ChildNodes.ContainsKey(splittedPath[i]))
          {
            curPath.ChildNodes[splittedPath[i]] = new PluginTreeNode();
          }
          curPath = curPath.ChildNodes[splittedPath[i]];
        }
        ++i;
      }

      return curPath;
    }

    private void AddExtensionPath(ExtensionPath path)
    {
      PluginTreeNode treePath = CreatePath(_rootNode, path.Name);
      foreach (NodeItem item in path.Items)
      {
        treePath.Items.Add(item);
      }
    }
    #endregion
  }
}
