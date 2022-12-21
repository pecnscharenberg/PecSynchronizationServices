/*
 * Copyright (C) 2018 Pheinex LLC
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace PecSynchronizationServices
{
    abstract public class ObservableObject : INotifyPropertyChanged
    {
        private ISet<string> propertyNames;
        
        protected bool ThrowOnInvalidPropertyName { get { return true; } }

        protected ISet<string> PropertyNames
        {
            get
            {
                if (propertyNames == null)
                {
                    propertyNames = new HashSet<string>();
                    foreach (var property in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        propertyNames.Add(property.Name);
                    }

                }
                return propertyNames;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void VerifyPropertyName(string propertyName)
        {
            if (propertyName == "") return;

            if (!PropertyNames.Contains(propertyName))
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool SetValue<T>(ref T field, T newValue, [CallerMemberName] string propertyName = "")
        {
            if(!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                OnPropertyChanged(propertyName);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
