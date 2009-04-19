using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using log4net;
using System.Globalization;
using System.Reflection;
using ch.froorider.codeheap.Persistence;
using System.Xml.Serialization;

namespace ch.froorider.codeheap.Domain
{
    /// <summary>The list containing all Observers, which want to be informed when the BO has changed.</summary>
    public delegate void BusinessObjectChangedHandler(BusinessObject changedBO);

    /// <summary>
    /// Template for all Business objects. Defines the general methods, events etc. which are common for all concrete BO's.
    /// </summary>
    [Serializable]
    public abstract class BusinessObject : INotifyPropertyChanged
    {
        #region events/fields/properites

        [NonSerialized]
        private ILog logger = LogManager.GetLogger(typeof(BusinessObject));

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the object identifier. The idenitfier is a string (MD5 Hash) which can be used to identify 
        /// the object in a list or so.
        /// The value of the string can be set only inside the NGoogleCalendar .dll
        /// </summary>
        /// <value>The object identifier as a string.</value>
        [XmlIgnore]
        public string ObjectIdentifier { get; internal set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessObject"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is needed for the use with XmlSerializer.
        /// </remarks>
        public BusinessObject()
        {
        }

        #endregion

        #region public methods

        /// <summary>
        /// Returns a string represenation of this object. All values stored in the properties
        /// are listed in the string in the format 'Name:xxxxx Value:yyyyyy'.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            Type type = this.GetType();
            builder.AppendFormat(CultureInfo.InvariantCulture, ">> Information of business object: '{0}'|", type.Name);
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo currentProperty in properties)
            {
                //Only use the property if it's not an set-only property
                if (currentProperty.CanRead)
                {
                    object propiValue = currentProperty.GetValue(this, null) ?? string.Empty;
                    BusinessObject bo = propiValue as BusinessObject;

                    if (bo != null)
                    {
                        //Recursive call on subclasses
                        builder.AppendFormat(CultureInfo.InvariantCulture, "Name: '{0}' Value: '{1}'",
                            currentProperty.Name, bo.ToString());
                    }
                    else
                    {
                        builder.AppendFormat(CultureInfo.InvariantCulture, "Name: '{0}' Value: '{1}'",
                            currentProperty.Name, currentProperty.GetValue(this, null));
                    }

                    builder.Append("|");
                }
            }

            //Remove the final "|" from the end of the string
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 1, 1);
            }


            return builder.ToString();
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is BusinessObject)) return false;
            BusinessObject toCompare = obj as BusinessObject;
            return this.ArePropertiesEqual(toCompare);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            string hashString = MD5HashGenerator.GenerateKey(this);
            try
            {
                return Convert.ToInt32(hashString);
            }
            catch (FormatException fe)
            {
                return base.GetHashCode();
            }
        }

        #endregion

        #region protected members

        /// <summary>
        /// This method compares the parameter business object with this object for equality. 
        /// All properties are checked exept following cases:
        /// - Properites which are marked with the XmlIgnore - Attribute
        /// 
        /// This properties have to be checked manually in the Equals - method
        /// </summary>
        /// <param name="toCompare">The business object which shall be compared with this one.</param>
        /// <returns>True if the parameter object is not null, has the same type and the properties are equal.</returns>
        protected bool ArePropertiesEqual(BusinessObject toCompare)
        {
            // Get a list of all properties
            PropertyInfo[] properties = this.GetType().GetProperties();

            // iterate thru all properties and compare them
            foreach (PropertyInfo pi in properties)
            {
                // Do not compare the property if its marked as XmlIgnore 
                object[] ignoreList = pi.GetCustomAttributes(typeof(XmlIgnoreAttribute), false);
                if (ignoreList != null && ignoreList.Length == 1) { continue; };

                // Check aggregat objects
                if (pi.PropertyType.IsSubclassOf(typeof(BusinessObject)))
                {
                    BusinessObject originalAggregat = pi.GetValue(this, null) as BusinessObject;
                    BusinessObject toCompareAggregat = pi.GetValue(toCompare, null) as BusinessObject;

                    if ((originalAggregat != null) & (toCompareAggregat != null))
                    {
                        if (!originalAggregat.Equals(toCompareAggregat))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                    continue;
                }

                // check all other properties
                object originalPropertyValue = pi.GetValue(this, null);
                object toComparePropertyValue = pi.GetValue(toCompare, null);

                if ((originalPropertyValue != null) & (toComparePropertyValue != null))
                {
                    if (!originalPropertyValue.Equals(toComparePropertyValue))
                    {
                        logger.Debug("Values of property: " + pi.Name + " are not equal. Value 1: " + originalPropertyValue + " Value 2: " + toComparePropertyValue);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Notify all observers that a property of this business object has changed.
        /// </summary>
        /// <param name="info"></param>
        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }
}
