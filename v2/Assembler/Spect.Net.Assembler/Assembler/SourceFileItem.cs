using System;
using System.Collections.Generic;
using System.Linq;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class describes a source file item
    /// </summary>
    public class SourceFileItem
    {
        /// <summary>
        /// The name of the file
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// The optional parent that includes this item
        /// </summary>
        public SourceFileItem Parent { get; private set; }

        /// <summary>
        /// The source files included by this item
        /// </summary>
        public List<SourceFileItem> Includes { get; }

        /// <summary>
        /// Initializes a new instance with the specified name
        /// </summary>
        /// <param name="filename">Source file name</param>
        public SourceFileItem(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException(nameof(filename));
            }
            Filename = filename;
            Parent = null;
            Includes = new List<SourceFileItem>();
        }

        /// <summary>
        /// Adds the specified item to the "Includes" list
        /// </summary>
        /// <param name="childItem">Included source file item</param>
        /// <returns>
        /// True, if including the child item is OK;
        /// False, if the inclusion would create a circular reference,
        /// or the child is already is in the list
        /// </returns>
        public bool Include(SourceFileItem childItem)
        {
            var current = this;
            while (current != null)
            {
                if (string.Compare(current.Filename, childItem.Filename, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return false;
                }
                current = current.Parent;
            }
            if (ContainsInIncludeList(childItem)) return false;
            Includes.Add(childItem);
            childItem.Parent = this;
            return true;
        }

        /// <summary>
        /// Checks if this item already contains the specified child item in 
        /// its "Includes" list
        /// </summary>
        /// <param name="childItem">Child item to check</param>
        /// <returns>
        /// True, if this item contains the child item;
        /// otherwise, false
        /// </returns>
        public bool ContainsInIncludeList(SourceFileItem childItem) =>
            Includes.Any(c => string.Compare(c.Filename, childItem.Filename,
                StringComparison.OrdinalIgnoreCase) == 0);
    }
}