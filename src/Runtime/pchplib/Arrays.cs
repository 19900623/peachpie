﻿using Pchp.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pchp.Library
{
    #region Enumerations

    /// <summary>
    /// Type of sorting.
    /// </summary>
    public enum ComparisonMethod
    {
        /// <summary>Regular comparison.</summary>
        Regular = 0,

        /// <summary>Numeric comparison.</summary>
        Numeric = 1,

        /// <summary>String comparison.</summary>
        String = 2,

        /// <summary>String comparison respecting to locale.</summary>
        LocaleString = 5,

        /// <summary>Undefined comparison.</summary>
        Undefined = -1
    };

    /// <summary>
    /// Sort order.
    /// </summary>
    public enum SortingOrder
    {
        /// <summary>Descending</summary>
        Descending = 3,

        /// <summary>Ascending</summary>
        Ascending = 4,

        /// <summary>Undefined</summary>
        Undefined = -1
    }

    /// <summary>
    /// Whether or not the sort is case-sensitive.
    /// </summary>
    public enum LetterCase
    {
        /// <summary>Lower case.</summary>
        Lower = 0,

        /// <summary>Upper case.</summary>
        Upper = 1
    }

    #endregion

    /// <summary>
    /// Implements PHP array functions.
    /// </summary>
    public static class Arrays
    {
        #region Constants

        public const int SORT_REGULAR = (int)ComparisonMethod.Regular;
        public const int SORT_NUMERIC = (int)ComparisonMethod.Numeric;
        public const int SORT_STRING = (int)ComparisonMethod.String;
        public const int SORT_LOCALE_STRING = (int)ComparisonMethod.LocaleString;

        public const int SORT_DESC = (int)SortingOrder.Descending;
        public const int SORT_ASC = (int)SortingOrder.Ascending;

        public const int CASE_LOWER = (int)LetterCase.Lower;
        public const int CASE_UPPER = (int)LetterCase.Upper;

        #endregion

        #region reset, pos, prev, next, key, end, each

        /// <summary>
        /// Retrieves a value being pointed by an array intrinsic enumerator.
        /// </summary>
        /// <param name="array">The array which current value to return.</param>
        /// <returns><b>False</b>, if the intrinsic enumerator is behind the last item of <paramref name="array"/>, 
        /// otherwise the value being pointed by the enumerator (beware of values which are <b>false</b>!).</returns>
        /// <remarks>The value returned is dereferenced.</remarks>
        public static PhpValue current(IPhpEnumerable array)
        {
            if (array == null)
            {
                //PhpException.ReferenceNull("array");
                //return null;
                throw new ArgumentNullException();
            }

            if (array.IntrinsicEnumerator.AtEnd)
                return PhpValue.False;

            // TODO: dereferences result since enumerator doesn't do so:
            return array.IntrinsicEnumerator.CurrentValue;
        }

        /// <summary>
        /// Retrieves a value being pointed by an array intrinsic enumerator.
        /// </summary>
        /// <param name="array">The array which current value to return.</param>
        /// <returns>
        /// <b>False</b> if the intrinsic enumerator is behind the last item of <paramref name="array"/>, 
        /// otherwise the value being pointed by the enumerator (beware of values which are <b>false</b>!).
        /// </returns>
        /// <remarks>
        /// Alias of <see cref="current"/>. The value returned is dereferenced.
        /// </remarks>
        public static object pos(IPhpEnumerable array) => current(array);

        /// <summary>
        /// Retrieves a key being pointed by an array intrinsic enumerator.
        /// </summary>
        /// <param name="array">The array which current key to return.</param>
        /// <returns>
        /// <b>Null</b>, if the intrinsic enumerator is behind the last item of <paramref name="array"/>, 
        /// otherwise the key being pointed by the enumerator.
        /// </returns>
        public static PhpValue key(IPhpEnumerable array)
        {
            if (array == null)
            {
                //PhpException.ReferenceNull("array");
                //return null;
                throw new ArgumentNullException();
            }

            if (array.IntrinsicEnumerator.AtEnd)
                return PhpValue.Null;

            // note, key can't be of type PhpReference, hence no dereferencing follows:
            return array.IntrinsicEnumerator.CurrentKey.GetValue();
        }

        /// <summary>
        /// Advances array intrinsic enumerator one item forward.
        /// </summary>
        /// <param name="array">The array which intrinsic enumerator to advance.</param>
        /// <returns>
        /// The value being pointed by the enumerator after it has been advanced
        /// or <b>false</b> if the enumerator has moved behind the last item of <paramref name="array"/>.
        /// </returns>
        /// <remarks>The value returned is dereferenced.</remarks>
        /// <include file='Doc/Arrays.xml' path='docs/intrinsicEnumeration/*'/>
        public static PhpValue next(IPhpEnumerable array)
        {
            if (array == null)
            {
                //PhpException.ReferenceNull("array");
                //return null;
                throw new ArgumentNullException();
            }

            // moves to the next item and returns false if there is no such item:
            if (!array.IntrinsicEnumerator.MoveNext()) return PhpValue.False;

            // TODO: dereferences result since enumerator doesn't do so:
            return array.IntrinsicEnumerator.CurrentValue;
        }

        /// <summary>
        /// Moves array intrinsic enumerator one item backward.
        /// </summary>
        /// <param name="array">The array which intrinsic enumerator to move.</param>
        /// <returns>
        /// The value being pointed by the enumerator after it has been moved
        /// or <b>false</b> if the enumerator has moved before the first item of <paramref name="array"/>.
        /// </returns>
        /// <remarks>The value returned is dereferenced.</remarks>
        public static PhpValue prev(IPhpEnumerable array)
        {
            if (array == null)
            {
                //PhpException.ReferenceNull("array");
                //return null;
                throw new ArgumentNullException();
            }

            // moves to the previous item and returns false if there is no such item:
            // TODO: dereferences result since enumerator doesn't do so:
            return (array.IntrinsicEnumerator.MovePrevious())
                ? array.IntrinsicEnumerator.CurrentValue
                : PhpValue.False;
        }

        /// <summary>
        /// Moves array intrinsic enumerator so it will point to the last item of the array.
        /// </summary>
        /// <param name="array">The array which intrinsic enumerator to move.</param>
        /// <returns>The last value in the <paramref name="array"/> or <b>false</b> if <paramref name="array"/> 
        /// is empty.</returns>
        /// <remarks>The value returned is dereferenced.</remarks>
        public static PhpValue end(IPhpEnumerable array)
        {
            if (array == null)
            {
                //PhpException.ReferenceNull("array");
                //return null;
                throw new ArgumentNullException();
            }

            // moves to the last item and returns false if there is no such item:
            // TODO: dereferences result since enumerator doesn't do so:
            return (array.IntrinsicEnumerator.MoveLast())
                ? array.IntrinsicEnumerator.CurrentValue
                : PhpValue.False;
        }

        /// <summary>
        /// Moves array intrinsic enumerator so it will point to the first item of the array.
        /// </summary>
        /// <param name="array">The array which intrinsic enumerator to move.</param>
        /// <returns>The first value in the <paramref name="array"/> or <b>false</b> if <paramref name="array"/> 
        /// is empty.</returns>
        /// <remarks>The value returned is dereferenced.</remarks>
        public static PhpValue reset(IPhpEnumerable array)
        {
            if (array == null)
            {
                //PhpException.ReferenceNull("array");
                return PhpValue.Null;
            }

            // moves to the last item and returns false if there is no such item:
            // TODO: dereferences result since enumerator doesn't do so:
            return (array.IntrinsicEnumerator.MoveFirst())
                ? array.IntrinsicEnumerator.CurrentValue
                : PhpValue.False;
        }

        /// <summary>
        /// Retrieves the current entry and advances array intrinsic enumerator one item forward.
        /// </summary>
        /// <param name="array">The array which entry get and which intrinsic enumerator to advance.</param>
        /// <returns>
        /// The instance of <see cref="PhpArray"/>(0 =&gt; key, 1 =&gt; value, "key" =&gt; key, "value" =&gt; value)
        /// where key and value are pointed by the enumerator before it is advanced
        /// or <b>false</b> if the enumerator has been behind the last item of <paramref name="array"/>
        /// before the call.
        /// </returns>
        /// <include file='Doc/Arrays.xml' path='docs/intrinsicEnumeration/*'/>
        //[return: CastToFalse, PhpDeepCopy]
        public static PhpArray each(IPhpEnumerable array)
        {
            if (array == null)
            {
                //PhpException.ReferenceNull("array");
                return null;
            }

            if (array.IntrinsicEnumerator.AtEnd)
                return null;

            var entry = array.IntrinsicEnumerator.Current;
            array.IntrinsicEnumerator.MoveNext();

            // dereferences result since enumerator doesn't do so:
            var key = entry.Key;
            var value = entry.Value; // PhpVariable.Dereference(entry.Value);

            // creates the resulting array:
            PhpArray result = new PhpArray(2);
            result.Add(1, value);
            result.Add("value", value);
            result.Add(0, key);
            result.Add("key", key);

            // keys and values should be inplace deeply copied:
            // TODO: result.InplaceCopyOnReturn = true;
            return result;
        }

        #endregion
    }
}
