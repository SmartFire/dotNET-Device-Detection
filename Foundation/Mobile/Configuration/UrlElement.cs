﻿/* *********************************************************************
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0.
 * 
 * If a copy of the MPL was not distributed with this file, You can obtain
 * one at http://mozilla.org/MPL/2.0/.
 * 
 * This Source Code Form is “Incompatible With Secondary Licenses”, as
 * defined by the Mozilla Public License, v. 2.0.
 * ********************************************************************* */

#region Usings

using System.Configuration;

#endregion

namespace FiftyOne.Foundation.Mobile.Configuration
{
    /// <summary>
    /// Used in the <c>web.config</c> to provide Urls and refresh periods to automatically
    /// update application data files such as 51Degrees.mobi data files.
    /// </summary>
    public sealed class UrlElement : ConfigurationElement
    {
        #region Constructors

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        [ConfigurationProperty("url", IsRequired = true)]
        [StringValidator(InvalidCharacters = "!@#$%^*()[]{};'\"|", MaxLength = 255)]
        public string Url
        {
            get { return (string) this["url"]; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this url should be used.
        /// </summary>
        [ConfigurationProperty("days", IsRequired = false, DefaultValue = 7)]
        public int Days
        {
            get { return (int) this["days"]; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this url should be used.
        /// </summary>
        [ConfigurationProperty("enabled", IsRequired = false, DefaultValue = true)]
        public bool Enabled
        {
            get { return (bool) this["enabled"]; }
        }

        #endregion
    }
}