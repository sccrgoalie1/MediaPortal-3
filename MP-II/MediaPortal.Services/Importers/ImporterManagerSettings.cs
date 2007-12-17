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
using MediaPortal.Core.Settings;

namespace MediaPortal.Services.Importers
{
  [Serializable]
  public class Share
  {
    public string Folder;
    public DateTime LastImport;
  };

  [Serializable]
  public class ImporterManagerSettings
  {
    public List<Share> _shares = new List<Share>();

    [Setting(SettingScope.Global, "")]
    public List<Share> Shares
    {
      get { return _shares; }
      set { _shares = value; }
    }
  }
}
