using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Cyclone.CollisionDetection
{
    /// <summary>
    /// A helper structure that contains information for the detector to use
    /// in building its contact data.
    /// </summary>
    public class CollisionData
    {
        #region Public Properties and Fields

        /// <summary>
        /// The contacts array to write into.
        /// </summary>
        public IList<Contact> Contacts;

        /// <summary>
        /// Holds the maximum number of contacts the array can take.
        /// </summary>
        public int ContactsLeft { get; private set; }

        ///<summary> 
        /// Holds the number of contacts found so far. 
        ///</summary>
        public int ContactCount { get; private set; }

        /// <summary>
        /// Holds the friction value to write into any collisions.
        /// </summary>
        public double Friction;

        /// <summary>
        /// Holds the restitution value to write into any collisions.
        /// </summary>
        public double Restitution;

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if there are no more contacts.
        /// </summary>
        /// <returns></returns>
        public bool NoMoreContacts()
        {
            return ContactsLeft <= 0;
        }

        /// <summary>
        /// Checks if there are more contacts.
        /// </summary>
        /// <returns></returns>
        public bool HasMoreContacts()
        {
            return ContactsLeft > 0;
        }

        public Contact GetContact()
        {
            if (NoMoreContacts())
                throw new InvalidOperationException("No more contacts.");

            var contact = Contacts[ContactsLeft];
            AddContacts(1);
            return contact;
        }

        #endregion

        #region Private Methods

        ///<summary>
        /// Notifies the data that the given number of contacts have
        /// been added.
        ///</summary>
        private void AddContacts(int count)
        {
            // Reduce the number of contacts remaining, add number used
            ContactsLeft -= count;
            ContactCount += count;
        }

        #endregion
    }
}
