using System;
using System.Collections;
using System.Xml;
using System.Xml.Linq;
using OpenTap.Tui.Views;
using Terminal.Gui;

namespace OpenTap.Tui.PropEditProviders
{
    public class KeyEventProvider : IPropEditProvider
    {
        public int Order { get; }
        public View Edit(AnnotationCollection annotation)
        {
            var keyMapSetting = annotation.Get<IObjectValueAnnotation>();
            if (keyMapSetting.Value is KeyEvent keyEvent)
            {
                var keyMapBindingView = new KeyMapBindingView(keyEvent);
                keyMapBindingView.Closing += changed => 
                {
                    try
                    {
                        if (changed && keyMapBindingView.NewKeyMap != null)
                            keyMapSetting.Value = keyMapBindingView.NewKeyMap;
                    }
                    catch (Exception exception)
                    {
                        TUI.Log.Error($"{exception.Message} {DefaultExceptionMessages.DefaultExceptionMessage}");
                        TUI.Log.Debug(exception);
                    }
                };

                return keyMapBindingView;
            }

            return null;
        }
    }
    
    public class KeyEventSerializer : TapSerializerPlugin
    {
        public override bool Deserialize(XElement node, ITypeData t, Action<object> setter)
        {
            if (t is TypeData t2 && t2.Type == typeof(KeyEvent))
            {
                var keyNode = node.Element("Key");
                var modifierNode = node.Element("Modifiers");

                if (keyNode?.Value == null || modifierNode?.Value == null)
                    return false;
                
                var keyValue = (Key)Enum.Parse(typeof(Key), keyNode.Value);
                var modifierValue = toModifiers(modifierNode.Value);

                setter(new KeyEvent(keyValue, modifierValue));
                
                return true;
            }

            return false;
        }

        public override bool Serialize(XElement node, object obj, ITypeData _expectedType)
        {
            if (_expectedType is TypeData expectedType2 && expectedType2.Type is Type expectedType && obj is KeyEvent keyEvent)
            {
                var key = new XElement("Key");
                var modifier = new XElement("Modifiers");
                bool keyok = Serializer.Serialize(key, keyEvent.Key, TypeData.FromType(typeof(Key)));
                
                // if (keyEvent.KeyValue >= 1 && keyEvent.KeyValue <= 26) // CTRL is pressed
                //     keyEvent.keyModifiers.Ctrl = true;
                //
                // modifier.Value = fromModifiers(keyEvent.keyModifiers);
                if (!keyok)
                    return false;
                node.Add(key);
                node.Add(modifier);
                return true;
            }
            
            return false;
        }

        public double Order => 0;

        public static string fromModifiers(KeyModifiers modifiers)
        {
            var bits = new char[6];
            bits[0] = modifiers.Shift ? '1' : '0';
            bits[1] = modifiers.Alt ? '1' : '0';
            bits[2] = modifiers.Ctrl ? '1' : '0';
            bits[3] = modifiers.Capslock ? '1' : '0';
            bits[4] = modifiers.Numlock ? '1' : '0';
            bits[5] = modifiers.Scrolllock ? '1' : '0';
            return new string(bits);
        }

        public static KeyModifiers toModifiers(string bits)
        {
            var modifiers = new KeyModifiers();
            modifiers.Shift = bits[0] == '1';
            modifiers.Alt = bits[1] == '1';
            modifiers.Ctrl = bits[2] == '1';
            modifiers.Capslock = bits[3] == '1';
            modifiers.Numlock = bits[4] == '1';
            modifiers.Scrolllock = bits[5] == '1';
            return modifiers;
        }
    }
}