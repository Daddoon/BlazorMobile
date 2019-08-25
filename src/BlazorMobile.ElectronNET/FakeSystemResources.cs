using BlazorMobile.ElectronNET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

[assembly: Dependency(typeof(FakeSystemResources))]
namespace BlazorMobile.ElectronNET
{
    public class FakeResourceDictionary : IResourceDictionary
    {
        public event EventHandler<ResourcesChangedEventArgs> ValuesChanged;

        private List<KeyValuePair<string, object>> _values = new List<KeyValuePair<string, object>>();

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        public bool TryGetValue(string key, out object value)
        {
            value = null;

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class FakeSystemResources : ISystemResourcesProvider
    {
        private FakeResourceDictionary _resources = null;

        public FakeSystemResources()
        {
            _resources = new FakeResourceDictionary();
        }

        public IResourceDictionary GetSystemResources()
        {
            return _resources;
        }
    }
}
