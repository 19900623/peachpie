﻿using Pchp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Peachpie.Library.PDO
{
    partial class PDO
    {
        string _errorSqlState;
        string _errorCode;
        string _errorMessage;

        /// <summary>
        /// Clears the error.
        /// </summary>
        [PhpHidden]
        internal void ClearError()
        {
            _errorSqlState = null;
            _errorCode = null;
            _errorMessage = null;
        }

        /// <summary>
        /// Handles the error.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <exception cref="Peachpie.Library.PDO.PDOException">
        /// </exception>
        [PhpHidden]
        internal void HandleError(System.Exception ex)
        {
            // fill errorInfo
            m_driver.HandleException(ex, out _errorSqlState, out _errorCode, out _errorMessage);

            //
            PDO_ERRMODE mode = (PDO_ERRMODE)this.m_attributes[PDO_ATTR.ATTR_ERRMODE].ToLong();
            switch (mode)
            {
                case PDO_ERRMODE.ERRMODE_SILENT:
                    break;
                case PDO_ERRMODE.ERRMODE_WARNING:
                    _ctx.Throw(PhpError.E_WARNING, ex.Message);
                    break;
                case PDO_ERRMODE.ERRMODE_EXCEPTION:
                    if (ex is Pchp.Library.Spl.Exception)
                    {
                        var pex = (Pchp.Library.Spl.Exception)ex;
                        throw new PDOException(pex.Message, pex.getCode(), pex);
                    }
                    else
                    {
                        throw new PDOException(ex.GetType().Name + ": " + ex.Message);
                    }
            }
        }

        /// <inheritDoc />
        public string errorCode() => _errorCode;

        /// <inheritDoc />
        public PhpArray errorInfo() => new PhpArray(3)
        {
            _errorSqlState, _errorCode, _errorMessage,
        };
    }
}
