using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Drawing;

namespace System.IO.Tools
{
    

    [Serializable]
    public class CustomMenuStructure : ISerializable
    {
        public string Text { get; set; }
        public string Tooltip { get; set; }
        public uint ID { get; set; }
        public string Command { get; set; }
        public List<CustomMenuStructure> Items = new List<CustomMenuStructure>();
        public bool IsFolder { get { return Items.Count != 0; } }
        public bool Checked { get; protected set; }
        public Bitmap Icon { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is string)
                return String.Equals(Text, obj as string, StringComparison.InvariantCultureIgnoreCase);

            return base.Equals(obj);            
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public CustomMenuStructure this[string text]
        {
            get
            {
                foreach (CustomMenuStructure item in Items)
                    if (item.Equals(text)) return item;
                return null;
            }
        }

        #region Constructor
        public CustomMenuStructure(string text, uint id)
        {
            Checked = false;

            if (text.EndsWith("[*]"))
            {
                Text = text.Substring(0, text.Length - 3);
                Checked = true;
            }
            else Text = text;            
            ID = id;
        }

        public CustomMenuStructure()
        {

        }
        #endregion

        #region ISerializable Members

        public CustomMenuStructure(SerializationInfo info, StreamingContext context)
        {
            SerializationInfoEnumerator enumerator = info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                switch (enumerator.Name)
                {
                    case "Text": Text = enumerator.Current.Value as string; break;
                    case "Tooltip": Tooltip = enumerator.Current.Value as string; break;
                    case "ID": ID = (uint)enumerator.Current.Value; break;
                    case "Command": Command = enumerator.Current.Value as string; break;
                    case "Checked": Checked = (bool)enumerator.Current.Value; break;
                    case "Items": Items = enumerator.Current.Value as List<CustomMenuStructure>; break;
                    case "Icon": Icon = enumerator.Current.Value as Bitmap; break;
                }
            }
            //Text = info.GetString("Text");
            //Tooltip = info.GetString("Tooltip");
            //ID = info.GetUInt32("ID");
            //Command = info.GetString("Command");
            //Checked = info.GetBoolean("Checked");
            //Items = (List<CustomMenuStructure>)info.GetValue("Items", typeof(List<CustomMenuStructure>));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Text", Text);
            info.AddValue("Tooltip", Tooltip);
            info.AddValue("ID", ID);
            info.AddValue("Command", Command);
            info.AddValue("Checked", Checked);
            info.AddValue("Items", Items);
            if (Icon != null)
                info.AddValue("Icon", Icon);
        }

        #endregion
    }
}
