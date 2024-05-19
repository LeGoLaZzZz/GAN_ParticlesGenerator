using System;
using SmartAttributes.MultiDraft.Attributes;

namespace SmartAttributes.MultiDraft.Editor
{
    public enum MultiAttributeDrawerOrder
    {
        First = 1,
        DefaultOrder = 5,
        Last = 10,
    }

    public abstract class AttributeDrawWorker
    {
        public MultiPropertyAttribute PropertyAttribute { get; private set; }


        protected AttributeDrawWorker(MultiPropertyAttribute propertyAttribute)
        {
            PropertyAttribute = propertyAttribute;
        }

        public virtual int GetOrder() => (int) MultiAttributeDrawerOrder.DefaultOrder;

        public virtual void DrawGUI(FieldGUI fieldGUI, Action callNextDrawer)
        {
        }
    }
}