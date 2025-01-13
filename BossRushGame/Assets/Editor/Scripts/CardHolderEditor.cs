using Game.Bosses.Cards;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Game.Editor.Scripts
{
    [CustomEditor(typeof(DeckHolder))]
    public class CardHolderEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var element = new VisualElement();
            InspectorElement.FillDefaultInspector(element, serializedObject, this);

            var obj = (DeckHolder)target;
            
            element.Add(new Button(() => obj.RemoveCard()) {text = "Remove card"});
            element.Add(new Button(() => obj.AddCard()) {text = "Add card"});

            return element;
        }
    }
}