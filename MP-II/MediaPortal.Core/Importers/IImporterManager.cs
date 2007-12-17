﻿#region Copyright (C) 2007 Team MediaPortal

/*
    Copyright (C) 2007 Team MediaPortal
    http://www.team-mediaportal.com
 
    This file is part of MediaPortal II

    MediaPortal II is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal II is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal II.  If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MediaPortal.Core.MediaManager;

namespace MediaPortal.Core.Importers
{
  public interface IImporterManager
  {
    /// <summary>
    /// Registers a new importer with the importer manager
    /// </summary>
    /// <param name="importer">The importer.</param>
    void Register(IImporter importer);

    /// <summary>
    /// Unregisters an importer with the importer manager
    /// </summary>
    /// <param name="importer">The importer.</param>
    void UnRegister(IImporter importer);

    /// <summary>
    /// Gets a list of all registered importers.
    /// </summary>
    /// <value>The registered importers.</value>
    List<IImporter> Importers { get;}

    /// <summary>
    /// Gets the importer for the specific name
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    IImporter GetImporterByName(string name);

    /// <summary>
    /// Returns a list of importers supporting the extension
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    List<IImporter> GetImporterByExtension(string extension);

    /// <summary>
    /// Adds a new share which should be imported & watched.
    /// </summary>
    /// <param name="folder">The folder.</param>
    void AddShare(string folder);

    /// <summary>
    /// Removes a share 
    /// </summary>
    /// <param name="folder">The folder.</param>
    void RemoveShare(string folder);

    /// <summary>
    /// Returns a list of all share being watched.
    /// </summary>
    /// <value>List containing all shares.</value>
    List<string> Shares { get;}

    /// <summary>
    /// Forces a complete import to be done on the folder
    /// </summary>
    /// <remarks>
    /// the folder should already be added via AddShare()
    /// </remarks>
    /// <param name="folder">The folder.</param>
    void ForceImport(string folder);

    /// <summary>
    /// Gets the meta data for a folder
    /// </summary>
    /// <param name="folde">The folder.</param>
    /// <param name="items">The items.</param>
    void GetMetaDataFor(string folder, ref List<IAbstractMediaItem> items);

  }
}
