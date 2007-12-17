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

using MediaPortal.Core.Database.Interfaces;

namespace MediaPortal.Database.Implementation.Sql
{
  public class SqlDatabaseBuilder : IDatabaseBuilder
  {
    #region IDatabaseBuilder Members

    private string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDatabaseBuilder"/> class.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    public SqlDatabaseBuilder(string connectionString)
    {
      _connectionString = connectionString;
    }

    /// <summary>
    /// Creates a new connection.
    /// </summary>
    /// <returns></returns>
    public IDatabaseConnection CreateConnection()
    {
      IDatabaseConnection connect = new SqlDatabaseConnection();
      connect.Open(_connectionString);
      return connect;
    }

    public IDatabaseConnection CreateConnection(bool systemDatabase)
    {
      IDatabaseConnection connect = new SqlDatabaseConnection();
      connect.Open(_connectionString, systemDatabase);
      return connect;
    }

    /// <summary>
    /// Creates a new command.
    /// </summary>
    /// <returns></returns>
    public IDatabaseCommand CreateCommand()
    {
      return new SqlDatabaseCommand();
    }

    /// <summary>
    /// Gets the name of the database.
    /// </summary>
    /// <value>The name of the database.</value>
    public string DatabaseName
    {
      get
      {
        int pos = _connectionString.ToLower().IndexOf("initial catalog=");
        pos += "initial catalog=".Length;
        int pos2 = _connectionString.IndexOf(";", pos);
        return _connectionString.Substring(pos, pos2 - pos);
      }
    }

    #endregion
  }
}